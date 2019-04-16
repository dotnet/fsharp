// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Open up the compiler as an incremental service for parsing, 
// type checking and intellisense-like environment-reporting.
//--------------------------------------------------------------------------

namespace FSharp.Compiler.SourceCodeServices

open System
open System.Collections.Generic
open System.IO

open Microsoft.FSharp.Core.Printf
open FSharp.Compiler 
open FSharp.Compiler.AbstractIL.Internal.Library  
open FSharp.Compiler.AbstractIL.Diagnostics 

open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.Ast
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.Layout
open FSharp.Compiler.Layout.TaggedTextOps
open FSharp.Compiler.Lib
open FSharp.Compiler.PrettyNaming
open FSharp.Compiler.Range
open FSharp.Compiler.Tast
open FSharp.Compiler.Tastops
open FSharp.Compiler.TcGlobals 
open FSharp.Compiler.Infos
open FSharp.Compiler.NameResolution
open FSharp.Compiler.InfoReader
open FSharp.Compiler.CompileOps

module EnvMisc2 =
    let maxMembers = GetEnvInteger "FCS_MaxMembersInQuickInfo" 10

//----------------------------------------------------------------------------
// Object model for diagnostics

[<RequireQualifiedAccess>]
type FSharpErrorSeverity = 
    | Warning 
    | Error

type FSharpErrorInfo(fileName, s: pos, e: pos, severity: FSharpErrorSeverity, message: string, subcategory: string, errorNum: int) = 
    member __.StartLine = Line.toZ s.Line
    member __.StartLineAlternate = s.Line
    member __.EndLine = Line.toZ e.Line
    member __.EndLineAlternate = e.Line
    member __.StartColumn = s.Column
    member __.EndColumn = e.Column
    member __.Severity = severity
    member __.Message = message
    member __.Subcategory = subcategory
    member __.FileName = fileName
    member __.ErrorNumber = errorNum
    member __.WithStart newStart = FSharpErrorInfo(fileName, newStart, e, severity, message, subcategory, errorNum)
    member __.WithEnd newEnd = FSharpErrorInfo(fileName, s, newEnd, severity, message, subcategory, errorNum)
    override __.ToString()= sprintf "%s (%d,%d)-(%d,%d) %s %s %s" fileName (int s.Line) (s.Column + 1) (int e.Line) (e.Column + 1) subcategory (if severity=FSharpErrorSeverity.Warning then "warning" else "error")  message
            
    /// Decompose a warning or error into parts: position, severity, message, error number
    static member CreateFromException(exn, isError, fallbackRange: range, suggestNames: bool) =
        let m = match GetRangeOfDiagnostic exn with Some m -> m | None -> fallbackRange 
        let msg = bufs (fun buf -> OutputPhasedDiagnostic buf exn false suggestNames)
        let errorNum = GetDiagnosticNumber exn
        FSharpErrorInfo(m.FileName, m.Start, m.End, (if isError then FSharpErrorSeverity.Error else FSharpErrorSeverity.Warning), msg, exn.Subcategory(), errorNum)
        
    /// Decompose a warning or error into parts: position, severity, message, error number
    static member CreateFromExceptionAndAdjustEof(exn, isError, fallbackRange: range, (linesCount: int, lastLength: int), suggestNames: bool) =
        let r = FSharpErrorInfo.CreateFromException(exn, isError, fallbackRange, suggestNames)
                
        // Adjust to make sure that errors reported at Eof are shown at the linesCount        
        let startline, schange = min (r.StartLineAlternate, false) (linesCount, true)
        let endline, echange = min (r.EndLineAlternate, false)   (linesCount, true)
        
        if not (schange || echange) then r
        else
            let r = if schange then r.WithStart(mkPos startline lastLength) else r
            if echange then r.WithEnd(mkPos  endline (1 + lastLength)) else r

    
/// Use to reset error and warning handlers            
[<Sealed>]
type ErrorScope()  = 
    let mutable errors = [] 
    let mutable firstError = None
    let unwindBP = PushThreadBuildPhaseUntilUnwind BuildPhase.TypeCheck
    let unwindEL =        
        PushErrorLoggerPhaseUntilUnwind (fun _oldLogger -> 
            { new ErrorLogger("ErrorScope") with 
                member x.DiagnosticSink(exn, isError) = 
                      let err = FSharpErrorInfo.CreateFromException(exn, isError, range.Zero, false)
                      errors <- err :: errors
                      if isError && firstError.IsNone then 
                          firstError <- Some err.Message
                member x.ErrorCount = errors.Length })
        
    member x.Errors = errors |> List.filter (fun error -> error.Severity = FSharpErrorSeverity.Error)
    member x.Warnings = errors |> List.filter (fun error -> error.Severity = FSharpErrorSeverity.Warning)
    member x.Diagnostics = errors
    member x.TryGetFirstErrorText() =
        match x.Errors with 
        | error :: _ -> Some error.Message
        | [] -> None
    
    interface IDisposable with
          member d.Dispose() = 
              unwindEL.Dispose() (* unwind pushes when ErrorScope disposes *)
              unwindBP.Dispose()

    member x.FirstError with get() = firstError and set v = firstError <- v
    
    /// Used at entry points to FSharp.Compiler.Service (service.fsi) which manipulate symbols and
    /// perform other operations which might expose us to either bona-fide F# error messages such 
    /// "missing assembly" (for incomplete assembly reference sets), or, if there is a compiler bug, 
    /// may hit internal compiler failures.
    ///
    /// In some calling cases, we get a chance to report the error as part of user text. For example
    /// if there is a "msising assembly" error while formatting the text of the description of an
    /// autocomplete, then the error message is shown in replacement of the text (rather than crashing Visual
    /// Studio, or swallowing the exception completely)
    static member Protect<'a> (m: range) (f: unit->'a) (err: string->'a): 'a = 
        use errorScope = new ErrorScope()
        let res = 
            try 
                Some (f())
            with e -> 
                // Here we only call errorRecovery to save the error message for later use by TryGetFirstErrorText.
                try 
                    errorRecovery e m
                with _ -> 
                    // If error recovery fails, then we have an internal compiler error. In this case, we show the whole stack
                    // in the extra message, should the extra message be used.
                    errorScope.FirstError <- Some (e.ToString())
                None
        match res with 
        | Some res -> res
        | None -> 
            match errorScope.TryGetFirstErrorText() with 
            | Some text -> err text
            | None -> err ""

/// An error logger that capture errors, filtering them according to warning levels etc.
type internal CompilationErrorLogger (debugName: string, options: FSharpErrorSeverityOptions) = 
    inherit ErrorLogger("CompilationErrorLogger("+debugName+")")
            
    let mutable errorCount = 0
    let diagnostics = new ResizeArray<_>()

    override x.DiagnosticSink(exn, isError) = 
        if isError || ReportWarningAsError options exn then
            diagnostics.Add(exn, FSharpErrorSeverity.Error)
            errorCount <- errorCount + 1
        else if ReportWarning options exn then
            diagnostics.Add(exn, FSharpErrorSeverity.Warning)

    override x.ErrorCount = errorCount

    member x.GetErrors() = diagnostics.ToArray()


/// This represents the global state established as each task function runs as part of the build.
///
/// Use to reset error and warning handlers.
type CompilationGlobalsScope(errorLogger: ErrorLogger, phase: BuildPhase) = 
    let unwindEL = PushErrorLoggerPhaseUntilUnwind(fun _ -> errorLogger)
    let unwindBP = PushThreadBuildPhaseUntilUnwind phase
    // Return the disposable object that cleans up
    interface IDisposable with
        member d.Dispose() =
            unwindBP.Dispose()         
            unwindEL.Dispose()

module ErrorHelpers =                            
    let ReportError (options, allErrors, mainInputFileName, fileInfo, (exn, sev), suggestNames) = 
        [ let isError = (sev = FSharpErrorSeverity.Error) || ReportWarningAsError options exn                
          if (isError || ReportWarning options exn) then 
            let oneError exn =
                [ // We use the first line of the file as a fallbackRange for reporting unexpected errors.
                  // Not ideal, but it's hard to see what else to do.
                  let fallbackRange = rangeN mainInputFileName 1
                  let ei = FSharpErrorInfo.CreateFromExceptionAndAdjustEof (exn, isError, fallbackRange, fileInfo, suggestNames)
                  if allErrors || (ei.FileName = mainInputFileName) || (ei.FileName = TcGlobals.DummyFileNameForRangesWithoutASpecificLocation) then
                      yield ei ]

            let mainError, relatedErrors = SplitRelatedDiagnostics exn 
            yield! oneError mainError
            for e in relatedErrors do 
                yield! oneError e ]

    let CreateErrorInfos (options, allErrors, mainInputFileName, errors, suggestNames) = 
        let fileInfo = (Int32.MaxValue, Int32.MaxValue)
        [| for (exn, isError) in errors do 
              yield! ReportError (options, allErrors, mainInputFileName, fileInfo, (exn, isError), suggestNames) |]
                            

//----------------------------------------------------------------------------
// Object model for tooltips and helpers for their generation from items

type public Layout = Internal.Utilities.StructuredFormat.Layout

/// Describe a comment as either a block of text or a file+signature reference into an intellidoc file.
[<RequireQualifiedAccess>]
type FSharpXmlDoc =
    | None
    | Text of string
    | XmlDocFileSignature of (*File and Signature*) string * string

/// A single data tip display element
[<RequireQualifiedAccess>]
type FSharpToolTipElementData<'T> = 
    { MainDescription:  'T 
      XmlDoc: FSharpXmlDoc
      /// typar insantiation text, to go after xml
      TypeMapping: 'T list
      Remarks: 'T option
      ParamName : string option }
    static member Create(layout:'T, xml, ?typeMapping, ?paramName, ?remarks) = 
        { MainDescription=layout; XmlDoc=xml; TypeMapping=defaultArg typeMapping []; ParamName=paramName; Remarks=remarks }

/// A single data tip display element
[<RequireQualifiedAccess>]
type FSharpToolTipElement<'T> = 
    | None

    /// A single type, method, etc with comment. May represent a method overload group.
    | Group of FSharpToolTipElementData<'T> list

    /// An error occurred formatting this element
    | CompositionError of string

    static member Single(layout, xml, ?typeMapping, ?paramName, ?remarks) = 
        Group [ FSharpToolTipElementData<'T>.Create(layout, xml, ?typeMapping=typeMapping, ?paramName=paramName, ?remarks=remarks) ]

/// A single data tip display element with where text is expressed as string
type public FSharpToolTipElement = FSharpToolTipElement<string>


/// A single data tip display element with where text is expressed as <see cref="Layout"/>
type public FSharpStructuredToolTipElement = FSharpToolTipElement<Layout>

/// Information for building a data tip box.
type FSharpToolTipText<'T> = 
    /// A list of data tip elements to display.
    | FSharpToolTipText of FSharpToolTipElement<'T> list  

type public FSharpToolTipText = FSharpToolTipText<string>
type public FSharpStructuredToolTipText = FSharpToolTipText<Layout>

module Tooltips =
    let ToFSharpToolTipElement tooltip = 
        match tooltip with
        | FSharpStructuredToolTipElement.None -> 
            FSharpToolTipElement.None
        | FSharpStructuredToolTipElement.Group l -> 
            FSharpToolTipElement.Group(l |> List.map(fun x -> 
                { MainDescription=showL x.MainDescription
                  XmlDoc=x.XmlDoc
                  TypeMapping=List.map showL x.TypeMapping
                  ParamName=x.ParamName
                  Remarks= Option.map showL x.Remarks }))
        | FSharpStructuredToolTipElement.CompositionError text -> 
            FSharpToolTipElement.CompositionError text

    let ToFSharpToolTipText (FSharpStructuredToolTipText.FSharpToolTipText text) = 
        FSharpToolTipText(List.map ToFSharpToolTipElement text)
    
    let Map f a = async.Bind(a, f >> async.Return)

[<RequireQualifiedAccess>]
type CompletionItemKind =
    | Field
    | Property
    | Method of isExtension : bool
    | Event
    | Argument
    | CustomOperation
    | Other

type UnresolvedSymbol =
    { FullName: string
      DisplayName: string
      Namespace: string[] }

type CompletionItem =
    { ItemWithInst: ItemWithInst
      Kind: CompletionItemKind
      IsOwnMember: bool
      MinorPriority: int
      Type: TyconRef option
      Unresolved: UnresolvedSymbol option }
    member x.Item = x.ItemWithInst.Item
      

[<AutoOpen>]
module internal SymbolHelpers = 

    let isFunction g ty =
        let _, tau = tryDestForallTy g ty
        isFunTy g tau 

    let OutputFullName isListItem ppF fnF r = 
      // Only display full names in quick info, not declaration lists or method lists
      if not isListItem then 
        match ppF r with 
        | None -> emptyL
        | Some _ -> wordL (tagText (FSComp.SR.typeInfoFullName())) ^^ RightL.colon ^^ (fnF r)
      else emptyL
          
    let rangeOfValRef preferFlag (vref: ValRef) =
        match preferFlag with 
        | None -> vref.Range 
        | Some false -> vref.DefinitionRange 
        | Some true -> vref.SigRange

    let rangeOfEntityRef preferFlag (eref: EntityRef) =
        match preferFlag with 
        | None -> eref.Range 
        | Some false -> eref.DefinitionRange 
        | Some true -> eref.SigRange

   
    let rangeOfPropInfo preferFlag (pinfo: PropInfo) =
        match pinfo with
#if !NO_EXTENSIONTYPING 
        |   ProvidedProp(_, pi, _) -> ComputeDefinitionLocationOfProvidedItem pi
#endif
        |   _ -> pinfo.ArbitraryValRef |> Option.map (rangeOfValRef preferFlag)

    let rangeOfMethInfo (g: TcGlobals) preferFlag (minfo: MethInfo) = 
        match minfo with
#if !NO_EXTENSIONTYPING 
        |   ProvidedMeth(_, mi, _, _) -> ComputeDefinitionLocationOfProvidedItem mi
#endif
        |   DefaultStructCtor(_, AppTy g (tcref, _)) -> Some(rangeOfEntityRef preferFlag tcref)
        |   _ -> minfo.ArbitraryValRef |> Option.map (rangeOfValRef preferFlag)

    let rangeOfEventInfo preferFlag (einfo: EventInfo) = 
        match einfo with
#if !NO_EXTENSIONTYPING 
        | ProvidedEvent (_, ei, _) -> ComputeDefinitionLocationOfProvidedItem ei
#endif
        | _ -> einfo.ArbitraryValRef |> Option.map (rangeOfValRef preferFlag)
      
    let rangeOfUnionCaseInfo preferFlag (ucinfo: UnionCaseInfo) =      
        match preferFlag with 
        | None -> ucinfo.UnionCase.Range 
        | Some false -> ucinfo.UnionCase.DefinitionRange 
        | Some true -> ucinfo.UnionCase.SigRange

    let rangeOfRecdFieldInfo preferFlag (rfinfo: RecdFieldInfo) =      
        match preferFlag with 
        | None -> rfinfo.RecdField.Range 
        | Some false -> rfinfo.RecdField.DefinitionRange 
        | Some true -> rfinfo.RecdField.SigRange

    let rec rangeOfItem (g: TcGlobals) preferFlag d = 
        match d with
        | Item.Value vref  | Item.CustomBuilder (_, vref) -> Some (rangeOfValRef preferFlag vref)
        | Item.UnionCase(ucinfo, _)     -> Some (rangeOfUnionCaseInfo preferFlag ucinfo)
        | Item.ActivePatternCase apref -> Some (rangeOfValRef preferFlag apref.ActivePatternVal)
        | Item.ExnCase tcref           -> Some tcref.Range
        | Item.AnonRecdField (_,_,_,m) -> Some m
        | Item.RecdField rfinfo        -> Some (rangeOfRecdFieldInfo preferFlag rfinfo)
        | Item.Event einfo             -> rangeOfEventInfo preferFlag einfo
        | Item.ILField _               -> None
        | Item.Property(_, pinfos)      -> rangeOfPropInfo preferFlag pinfos.Head 
        | Item.Types(_, tys)     -> tys |> List.tryPick (tryNiceEntityRefOfTyOption >> Option.map (rangeOfEntityRef preferFlag))
        | Item.CustomOperation (_, _, Some minfo)  -> rangeOfMethInfo g preferFlag minfo
        | Item.TypeVar (_, tp)  -> Some tp.Range
        | Item.ModuleOrNamespaces modrefs -> modrefs |> List.tryPick (rangeOfEntityRef preferFlag >> Some)
        | Item.MethodGroup(_, minfos, _) 
        | Item.CtorGroup(_, minfos) -> minfos |> List.tryPick (rangeOfMethInfo g preferFlag)
        | Item.ActivePatternResult(APInfo _, _, _, m) -> Some m
        | Item.SetterArg (_, item) -> rangeOfItem g preferFlag item
        | Item.ArgName (id, _, _) -> Some id.idRange
        | Item.CustomOperation (_, _, implOpt) -> implOpt |> Option.bind (rangeOfMethInfo g preferFlag)
        | Item.ImplicitOp (_, {contents = Some(TraitConstraintSln.FSMethSln(_, vref, _))}) -> Some vref.Range
        | Item.ImplicitOp _ -> None
        | Item.UnqualifiedType tcrefs -> tcrefs |> List.tryPick (rangeOfEntityRef preferFlag >> Some)
        | Item.DelegateCtor ty 
        | Item.FakeInterfaceCtor ty -> ty |> tryNiceEntityRefOfTyOption |> Option.map (rangeOfEntityRef preferFlag)
        | Item.NewDef _ -> None

    // Provided type definitions do not have a useful F# CCU for the purposes of goto-definition.
    let computeCcuOfTyconRef (tcref: TyconRef) = 
#if !NO_EXTENSIONTYPING
        if tcref.IsProvided then None else 
#endif
        ccuOfTyconRef tcref

    let ccuOfMethInfo (g: TcGlobals) (minfo: MethInfo) = 
        match minfo with
        | DefaultStructCtor(_, AppTy g (tcref, _)) -> computeCcuOfTyconRef tcref
        | _ -> 
            minfo.ArbitraryValRef 
            |> Option.bind ccuOfValRef 
            |> Option.orElseWith (fun () -> minfo.DeclaringTyconRef |> computeCcuOfTyconRef)


    let rec ccuOfItem (g: TcGlobals) d = 
        match d with
        | Item.Value vref | Item.CustomBuilder (_, vref) -> ccuOfValRef vref 
        | Item.UnionCase(ucinfo, _)             -> computeCcuOfTyconRef ucinfo.TyconRef
        | Item.ActivePatternCase apref         -> ccuOfValRef apref.ActivePatternVal
        | Item.ExnCase tcref                   -> computeCcuOfTyconRef tcref
        | Item.RecdField rfinfo                -> computeCcuOfTyconRef rfinfo.RecdFieldRef.TyconRef
        | Item.Event einfo                     -> einfo.DeclaringTyconRef |> computeCcuOfTyconRef
        | Item.ILField finfo                   -> finfo.DeclaringTyconRef |> computeCcuOfTyconRef
        | Item.Property(_, pinfos)              -> 
            pinfos |> List.tryPick (fun pinfo -> 
                pinfo.ArbitraryValRef 
                |> Option.bind ccuOfValRef
                |> Option.orElseWith (fun () -> pinfo.DeclaringTyconRef |> computeCcuOfTyconRef))

        | Item.ArgName (_, _, Some (ArgumentContainer.Method minfo))  -> ccuOfMethInfo g minfo

        | Item.MethodGroup(_, minfos, _)
        | Item.CtorGroup(_, minfos) -> minfos |> List.tryPick (ccuOfMethInfo g)
        | Item.CustomOperation (_, _, Some minfo)       -> ccuOfMethInfo g minfo

        | Item.Types(_, tys)             -> tys |> List.tryPick (tryNiceEntityRefOfTyOption >> Option.bind computeCcuOfTyconRef)

        | Item.ArgName (_, _, Some (ArgumentContainer.Type eref)) -> computeCcuOfTyconRef eref

        | Item.ModuleOrNamespaces erefs 
        | Item.UnqualifiedType erefs -> erefs |> List.tryPick computeCcuOfTyconRef 

        | Item.SetterArg (_, item) -> ccuOfItem g item
        | Item.AnonRecdField (info, _, _, _) -> Some info.Assembly
        | Item.TypeVar _  -> None
        | _ -> None

    /// Work out the source file for an item and fix it up relative to the CCU if it is relative.
    let fileNameOfItem (g: TcGlobals) qualProjectDir (m: range) h =
        let file = m.FileName 
        if verbose then dprintf "file stored in metadata is '%s'\n" file
        if not (FileSystem.IsPathRootedShim file) then 
            match ccuOfItem g h with 
            | Some ccu -> 
                Path.Combine(ccu.SourceCodeDirectory, file)
            | None -> 
                match qualProjectDir with 
                | None     -> file
                | Some dir -> Path.Combine(dir, file)
         else file

    /// Cut long filenames to make them visually appealing 
    let cutFileName s = if String.length s > 40 then String.sub s 0 10 + "..."+String.sub s (String.length s - 27) 27 else s

    let libFileOfEntityRef x =
        match x with
        | ERefLocal _ -> None
        | ERefNonLocal nlref -> nlref.Ccu.FileName      

    let ParamNameAndTypesOfUnaryCustomOperation g minfo = 
        match minfo with 
        | FSMeth(_, _, vref, _) -> 
            let argInfos = ArgInfosOfMember g vref |> List.concat 
            // Drop the first 'seq<T>' argument representing the computation space
            let argInfos = if argInfos.IsEmpty then [] else argInfos.Tail
            [ for (ty, argInfo) in argInfos do
                  let isPP = HasFSharpAttribute g g.attrib_ProjectionParameterAttribute argInfo.Attribs
                  // Strip the tuple space type of the type of projection parameters
                  let ty = if isPP && isFunTy g ty then rangeOfFunTy g ty else ty
                  yield ParamNameAndType(argInfo.Name, ty) ]
        | _ -> []

    // Find the name of the metadata file for this external definition 
    let metaInfoOfEntityRef (infoReader: InfoReader) m tcref = 
        let g = infoReader.g
        match tcref with 
        | ERefLocal _ -> None
        | ERefNonLocal nlref -> 
            // Generalize to get a formal signature 
            let formalTypars = tcref.Typars m
            let formalTypeInst = generalizeTypars formalTypars
            let ty = TType_app(tcref, formalTypeInst)
            if isILAppTy g ty then
                let formalTypeInfo = ILTypeInfo.FromType g ty
                Some(nlref.Ccu.FileName, formalTypars, formalTypeInfo)
            else None

    let mkXmlComment thing =
        match thing with
        | Some (Some fileName, xmlDocSig) -> FSharpXmlDoc.XmlDocFileSignature(fileName, xmlDocSig)
        | _ -> FSharpXmlDoc.None

    let GetXmlDocSigOfEntityRef infoReader m (eref: EntityRef) = 
        if eref.IsILTycon then 
            match metaInfoOfEntityRef infoReader m eref  with
            | None -> None
            | Some (ccuFileName, _, formalTypeInfo) -> Some(ccuFileName, "T:"+formalTypeInfo.ILTypeRef.FullName)
        else
            let ccuFileName = libFileOfEntityRef eref
            let m = eref.Deref
            if m.XmlDocSig = "" then
                m.XmlDocSig <- XmlDocSigOfEntity eref
            Some (ccuFileName, m.XmlDocSig)

    let GetXmlDocSigOfScopedValRef g (tcref: TyconRef) (vref: ValRef) = 
        let ccuFileName = libFileOfEntityRef tcref
        let v = vref.Deref
        if v.XmlDocSig = "" && v.HasDeclaringEntity then
            let ap = buildAccessPath vref.TopValDeclaringEntity.CompilationPathOpt
            let path =
                if vref.TopValDeclaringEntity.IsModule then
                    let sep = if ap.Length > 0 then "." else ""
                    ap + sep + vref.TopValDeclaringEntity.CompiledName
                else
                    ap
            v.XmlDocSig <- XmlDocSigOfVal g path v
        Some (ccuFileName, v.XmlDocSig)                

    let GetXmlDocSigOfRecdFieldInfo (rfinfo: RecdFieldInfo) = 
        let tcref = rfinfo.TyconRef
        let ccuFileName = libFileOfEntityRef tcref 
        if rfinfo.RecdField.XmlDocSig = "" then
            rfinfo.RecdField.XmlDocSig <- XmlDocSigOfProperty [tcref.CompiledRepresentationForNamedType.FullName; rfinfo.Name]
        Some (ccuFileName, rfinfo.RecdField.XmlDocSig)            

    let GetXmlDocSigOfUnionCaseInfo (ucinfo: UnionCaseInfo) = 
        let tcref =  ucinfo.TyconRef
        let ccuFileName = libFileOfEntityRef tcref
        if  ucinfo.UnionCase.XmlDocSig = "" then
            ucinfo.UnionCase.XmlDocSig <- XmlDocSigOfUnionCase [tcref.CompiledRepresentationForNamedType.FullName; ucinfo.Name]
        Some (ccuFileName, ucinfo.UnionCase.XmlDocSig)

    let GetXmlDocSigOfMethInfo (infoReader: InfoReader)  m (minfo: MethInfo) = 
        let amap = infoReader.amap
        match minfo with
        | FSMeth (g, _, vref, _) ->
            GetXmlDocSigOfScopedValRef g minfo.DeclaringTyconRef vref
        | ILMeth (g, ilminfo, _) ->            
            let actualTypeName = ilminfo.DeclaringTyconRef.CompiledRepresentationForNamedType.FullName
            let fmtps = ilminfo.FormalMethodTypars            
            let genArity = if fmtps.Length=0 then "" else sprintf "``%d" fmtps.Length

            match metaInfoOfEntityRef infoReader m ilminfo.DeclaringTyconRef  with 
            | None -> None
            | Some (ccuFileName, formalTypars, formalTypeInfo) ->
                let filminfo = ILMethInfo(g, formalTypeInfo.ToType, None, ilminfo.RawMetadata, fmtps) 
                let args = 
                    match ilminfo.IsILExtensionMethod with
                    | true -> filminfo.GetRawArgTypes(amap, m, minfo.FormalMethodInst)
                    | false -> filminfo.GetParamTypes(amap, m, minfo.FormalMethodInst)

                // http://msdn.microsoft.com/en-us/library/fsbx0t7x.aspx
                // If the name of the item itself has periods, they are replaced by the hash-sign ('#'). 
                // It is assumed that no item has a hash-sign directly in its name. For example, the fully 
                // qualified name of the String constructor would be "System.String.#ctor".
                let normalizedName = ilminfo.ILName.Replace(".", "#")

                Some (ccuFileName, "M:"+actualTypeName+"."+normalizedName+genArity+XmlDocArgsEnc g (formalTypars, fmtps) args)
        | DefaultStructCtor _ -> None
#if !NO_EXTENSIONTYPING
        | ProvidedMeth _ -> None
#endif

    let GetXmlDocSigOfValRef g (vref: ValRef) =
        if not vref.IsLocalRef then
            let ccuFileName = vref.nlr.Ccu.FileName
            let v = vref.Deref
            if v.XmlDocSig = "" && v.HasDeclaringEntity then
                v.XmlDocSig <- XmlDocSigOfVal g vref.TopValDeclaringEntity.CompiledRepresentationForNamedType.Name v
            Some (ccuFileName, v.XmlDocSig)
        else 
            None

    let GetXmlDocSigOfProp infoReader m (pinfo: PropInfo) =
        let g = pinfo.TcGlobals
        match pinfo with 
#if !NO_EXTENSIONTYPING
        | ProvidedProp _ -> None // No signature is possible. If an xml comment existed it would have been returned by PropInfo.XmlDoc in infos.fs
#endif
        | FSProp _ as fspinfo -> 
            match fspinfo.ArbitraryValRef with 
            | None -> None
            | Some vref -> GetXmlDocSigOfScopedValRef g pinfo.DeclaringTyconRef vref
        | ILProp(ILPropInfo(_, pdef)) -> 
            match metaInfoOfEntityRef infoReader m pinfo.DeclaringTyconRef with
            | Some (ccuFileName, formalTypars, formalTypeInfo) ->
                let filpinfo = ILPropInfo(formalTypeInfo, pdef)
                Some (ccuFileName, "P:"+formalTypeInfo.ILTypeRef.FullName+"."+pdef.Name+XmlDocArgsEnc g (formalTypars, []) (filpinfo.GetParamTypes(infoReader.amap, m)))
            | _ -> None

    let GetXmlDocSigOfEvent infoReader m (einfo: EventInfo) =
        match einfo with
        | ILEvent _ ->
            match metaInfoOfEntityRef infoReader m einfo.DeclaringTyconRef with 
            | Some (ccuFileName, _, formalTypeInfo) -> 
                Some(ccuFileName, "E:"+formalTypeInfo.ILTypeRef.FullName+"."+einfo.EventName)
            | _ -> None
        | _ -> None

    let GetXmlDocSigOfILFieldInfo infoReader m (finfo: ILFieldInfo) =
        match metaInfoOfEntityRef infoReader m finfo.DeclaringTyconRef with
        | Some (ccuFileName, _, formalTypeInfo) ->
            Some(ccuFileName, "F:"+formalTypeInfo.ILTypeRef.FullName+"."+finfo.FieldName)
        | _ -> None

    /// This function gets the signature to pass to Visual Studio to use its lookup functions for .NET stuff. 
    let GetXmlDocHelpSigOfItemForLookup (infoReader: InfoReader) m d = 
        let g = infoReader.g
                
        match d with
        | Item.ActivePatternCase (APElemRef(_, vref, _))        
        | Item.Value vref | Item.CustomBuilder (_, vref) -> 
            mkXmlComment (GetXmlDocSigOfValRef g vref)
        | Item.UnionCase  (ucinfo, _) -> mkXmlComment (GetXmlDocSigOfUnionCaseInfo ucinfo)
        | Item.ExnCase tcref -> mkXmlComment (GetXmlDocSigOfEntityRef infoReader m tcref)
        | Item.RecdField rfinfo -> mkXmlComment (GetXmlDocSigOfRecdFieldInfo rfinfo)
        | Item.NewDef _ -> FSharpXmlDoc.None
        | Item.ILField finfo -> mkXmlComment (GetXmlDocSigOfILFieldInfo infoReader m finfo)
        | Item.Types(_, ((TType_app(tcref, _)) :: _)) ->  mkXmlComment (GetXmlDocSigOfEntityRef infoReader m tcref)
        | Item.CustomOperation (_, _, Some minfo) -> mkXmlComment (GetXmlDocSigOfMethInfo infoReader  m minfo)
        | Item.TypeVar _  -> FSharpXmlDoc.None
        | Item.ModuleOrNamespaces(modref :: _) -> mkXmlComment (GetXmlDocSigOfEntityRef infoReader m modref)

        | Item.Property(_, (pinfo :: _)) -> mkXmlComment (GetXmlDocSigOfProp infoReader m pinfo)
        | Item.Event einfo -> mkXmlComment (GetXmlDocSigOfEvent infoReader m einfo)

        | Item.MethodGroup(_, minfo :: _, _) -> mkXmlComment (GetXmlDocSigOfMethInfo infoReader  m minfo)
        | Item.CtorGroup(_, minfo :: _) -> mkXmlComment (GetXmlDocSigOfMethInfo infoReader  m minfo)
        | Item.ArgName(_, _, Some argContainer) -> 
            match argContainer with 
            | ArgumentContainer.Method minfo -> mkXmlComment (GetXmlDocSigOfMethInfo infoReader m minfo)
            | ArgumentContainer.Type tcref -> mkXmlComment (GetXmlDocSigOfEntityRef infoReader m tcref)
            | ArgumentContainer.UnionCase ucinfo -> mkXmlComment (GetXmlDocSigOfUnionCaseInfo ucinfo)
        |  _ -> FSharpXmlDoc.None

    /// Produce an XmlComment with a signature or raw text, given the F# comment and the item
    let GetXmlCommentForItemAux (xmlDoc: XmlDoc option) (infoReader: InfoReader) m d = 
        let result = 
            match xmlDoc with 
            | None | Some (XmlDoc [| |]) -> ""
            | Some (XmlDoc l) -> 
                bufs (fun os -> 
                    bprintf os "\n"
                    l |> Array.iter (fun (s: string) -> 
                        // Note: this code runs for local/within-project xmldoc tooltips, but not for cross-project or .XML
                        bprintf os "\n%s" s))

        if String.IsNullOrEmpty result then 
            GetXmlDocHelpSigOfItemForLookup infoReader m d
        else
            FSharpXmlDoc.Text result

    let mutable ToolTipFault  = None
    
    let GetXmlCommentForMethInfoItem infoReader m d (minfo: MethInfo) = 
        if minfo.HasDirectXmlComment || minfo.XmlDoc.NonEmpty then
            GetXmlCommentForItemAux (Some minfo.XmlDoc) infoReader m d 
        else
            mkXmlComment (GetXmlDocSigOfMethInfo infoReader m minfo)

    let FormatTyparMapping denv (prettyTyparInst: TyparInst) = 
        [ for (tp, ty) in prettyTyparInst -> 
            wordL (tagTypeParameter ("'" + tp.DisplayName))  ^^ wordL (tagText (FSComp.SR.descriptionWordIs())) ^^ NicePrint.layoutType denv ty  ]

    /// Generate the structured tooltip for a method info
    let FormatOverloadsToList (infoReader: InfoReader) m denv (item: ItemWithInst) minfos : FSharpStructuredToolTipElement = 
        ToolTipFault |> Option.iter (fun msg -> 
           let exn = Error((0, msg), range.Zero)
           let ph = PhasedDiagnostic.Create(exn, BuildPhase.TypeCheck)
           simulateError ph)
        
        let layouts = 
            [ for minfo in minfos -> 
                let prettyTyparInst, layout = NicePrint.prettyLayoutOfMethInfoFreeStyle infoReader.amap m denv item.TyparInst minfo
                let xml = GetXmlCommentForMethInfoItem infoReader m item.Item minfo
                let tpsL = FormatTyparMapping denv prettyTyparInst
                FSharpToolTipElementData<_>.Create(layout, xml, tpsL) ]
 
        FSharpStructuredToolTipElement.Group layouts

        
    let pubpathOfValRef (v: ValRef) = v.PublicPath        
    let pubpathOfTyconRef (x: TyconRef) = x.PublicPath


    let (|ItemWhereTypIsPreferred|_|) item = 
        match item with 
        | Item.DelegateCtor ty
        | Item.CtorGroup(_, [DefaultStructCtor(_, ty)])
        | Item.FakeInterfaceCtor ty
        | Item.Types(_, [ty])  -> Some ty
        | _ -> None

    /// Specifies functions for comparing 'Item' objects with respect to the user 
    /// (this means that some values that are not technically equal are treated as equal 
    ///  if this is what we want to show to the user, because we're comparing just the name
    //   for some cases e.g. when using 'fullDisplayTextOfModRef')
    let ItemDisplayPartialEquality g = 
      { new IPartialEqualityComparer<_> with   
          member x.InEqualityRelation item = 
              match item  with 
              | Item.Types(_, [_]) -> true
              | Item.ILField(ILFieldInfo _) -> true
              | Item.RecdField _ -> true
              | Item.SetterArg _ -> true
              | Item.TypeVar _ -> true
              | Item.CustomOperation _ -> true
              | Item.ModuleOrNamespaces(_ :: _) -> true
              | Item.MethodGroup _ -> true
              | Item.Value _ | Item.CustomBuilder _ -> true
              | Item.ActivePatternCase _ -> true
              | Item.DelegateCtor _ -> true
              | Item.UnionCase _ -> true
              | Item.ExnCase _ -> true              
              | Item.Event _ -> true
              | Item.Property _ -> true
              | Item.CtorGroup _ -> true
              | Item.UnqualifiedType _ -> true
              | _ -> false
              
          member x.Equals(item1, item2) = 
            // This may explore assemblies that are not in the reference set.
            // In this case just bail out and assume items are not equal
            protectAssemblyExploration false (fun () -> 
              let equalHeadTypes(ty1, ty2) =
                  match tryDestAppTy g ty1 with
                  | ValueSome tcref1 ->
                    match tryDestAppTy g ty2 with
                    | ValueSome tcref2 -> tyconRefEq g tcref1 tcref2
                    | _ -> typeEquiv g ty1 ty2
                  | _ -> typeEquiv g ty1 ty2

              ItemsAreEffectivelyEqual g item1 item2 || 

              // Much of this logic is already covered by 'ItemsAreEffectivelyEqual'
              match item1, item2 with 
              | Item.DelegateCtor ty1, Item.DelegateCtor ty2 -> equalHeadTypes(ty1, ty2)
              | Item.Types(dn1, [ty1]), Item.Types(dn2, [ty2]) -> 
                  // Bug 4403: We need to compare names as well, because 'int' and 'Int32' are physically the same type, but we want to show both
                  dn1 = dn2 && equalHeadTypes(ty1, ty2) 
              
              // Prefer a type to a DefaultStructCtor, a DelegateCtor and a FakeInterfaceCtor 
              | ItemWhereTypIsPreferred ty1, ItemWhereTypIsPreferred ty2 -> equalHeadTypes(ty1, ty2) 

              | Item.ExnCase tcref1, Item.ExnCase tcref2 -> tyconRefEq g tcref1 tcref2
              | Item.ILField(ILFieldInfo(_, fld1)), Item.ILField(ILFieldInfo(_, fld2)) -> 
                  fld1 === fld2 // reference equality on the object identity of the AbstractIL metadata blobs for the fields
              | Item.CustomOperation (_, _, Some minfo1), Item.CustomOperation (_, _, Some minfo2) -> 
                    MethInfo.MethInfosUseIdenticalDefinitions minfo1 minfo2
              | Item.TypeVar (nm1, tp1), Item.TypeVar (nm2, tp2) -> 
                    (nm1 = nm2) && typarRefEq tp1 tp2
              | Item.ModuleOrNamespaces(modref1 :: _), Item.ModuleOrNamespaces(modref2 :: _) -> fullDisplayTextOfModRef modref1 = fullDisplayTextOfModRef modref2
              | Item.SetterArg(id1, _), Item.SetterArg(id2, _) -> Range.equals id1.idRange id2.idRange && id1.idText = id2.idText
              | Item.MethodGroup(_, meths1, _), Item.MethodGroup(_, meths2, _) -> 
                  Seq.zip meths1 meths2 |> Seq.forall (fun (minfo1, minfo2) ->
                    MethInfo.MethInfosUseIdenticalDefinitions minfo1 minfo2)
              | (Item.Value vref1 | Item.CustomBuilder (_, vref1)), (Item.Value vref2 | Item.CustomBuilder (_, vref2)) -> 
                  valRefEq g vref1 vref2
              | Item.ActivePatternCase(APElemRef(_apinfo1, vref1, idx1)), Item.ActivePatternCase(APElemRef(_apinfo2, vref2, idx2)) ->
                  idx1 = idx2 && valRefEq g vref1 vref2
              | Item.UnionCase(UnionCaseInfo(_, ur1), _), Item.UnionCase(UnionCaseInfo(_, ur2), _) -> 
                  g.unionCaseRefEq ur1 ur2
              | Item.RecdField(RecdFieldInfo(_, RFRef(tcref1, n1))), Item.RecdField(RecdFieldInfo(_, RFRef(tcref2, n2))) -> 
                  (tyconRefEq g tcref1 tcref2) && (n1 = n2) // there is no direct function as in the previous case
              | Item.Property(_, pi1s), Item.Property(_, pi2s) -> 
                  List.zip pi1s pi2s |> List.forall(fun (pi1, pi2) -> PropInfo.PropInfosUseIdenticalDefinitions pi1 pi2)
              | Item.Event evt1, Item.Event evt2 -> 
                  EventInfo.EventInfosUseIdenticalDefintions evt1 evt2
              | Item.AnonRecdField(anon1, _, i1, _), Item.AnonRecdField(anon2, _, i2, _) ->
                 Tastops.anonInfoEquiv anon1 anon2 && i1 = i2
              | Item.CtorGroup(_, meths1), Item.CtorGroup(_, meths2) -> 
                  List.zip meths1 meths2 
                  |> List.forall (fun (minfo1, minfo2) -> MethInfo.MethInfosUseIdenticalDefinitions minfo1 minfo2)
              | Item.UnqualifiedType tcRefs1, Item.UnqualifiedType tcRefs2 ->
                  List.zip tcRefs1 tcRefs2
                  |> List.forall (fun (tcRef1, tcRef2) -> tyconRefEq g tcRef1 tcRef2)
              | Item.Types(_, [TType.TType_app(tcRef1, _)]), Item.UnqualifiedType([tcRef2]) -> tyconRefEq g tcRef1 tcRef2
              | Item.UnqualifiedType([tcRef1]), Item.Types(_, [TType.TType_app(tcRef2, _)]) -> tyconRefEq g tcRef1 tcRef2
              | _ -> false)
              
          member x.GetHashCode item =
            // This may explore assemblies that are not in the reference set.
            // In this case just bail out and use a random hash code
            protectAssemblyExploration 1027 (fun () -> 
              match item with 
              | ItemWhereTypIsPreferred ty -> 
                  match tryDestAppTy g ty with
                  | ValueSome tcref -> hash tcref.LogicalName
                  | _ -> 1010
              | Item.ILField(ILFieldInfo(_, fld)) -> 
                  System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode fld // hash on the object identity of the AbstractIL metadata blob for the field
              | Item.TypeVar (nm, _tp) -> hash nm
              | Item.CustomOperation (_, _, Some minfo) -> minfo.ComputeHashCode()
              | Item.CustomOperation (_, _, None) -> 1
              | Item.ModuleOrNamespaces(modref :: _) -> hash (fullDisplayTextOfModRef modref)          
              | Item.SetterArg(id, _) -> hash (id.idRange, id.idText)
              | Item.MethodGroup(_, meths, _) -> meths |> List.fold (fun st a -> st + a.ComputeHashCode()) 0
              | Item.CtorGroup(name, meths) -> name.GetHashCode() + (meths |> List.fold (fun st a -> st + a.ComputeHashCode()) 0)
              | (Item.Value vref | Item.CustomBuilder (_, vref)) -> hash vref.LogicalName
              | Item.ActivePatternCase(APElemRef(_apinfo, vref, idx)) -> hash (vref.LogicalName, idx)
              | Item.ExnCase tcref -> hash tcref.LogicalName
              | Item.UnionCase(UnionCaseInfo(_, UCRef(tcref, n)), _) -> hash(tcref.Stamp, n)
              | Item.RecdField(RecdFieldInfo(_, RFRef(tcref, n))) -> hash(tcref.Stamp, n)
              | Item.AnonRecdField(anon, _, i, _) -> hash anon.SortedNames.[i]
              | Item.Event evt -> evt.ComputeHashCode()
              | Item.Property(_name, pis) -> hash (pis |> List.map (fun pi -> pi.ComputeHashCode()))
              | Item.UnqualifiedType(tcref :: _) -> hash tcref.LogicalName
              | _ -> failwith "unreachable") }

    let CompletionItemDisplayPartialEquality g = 
        let itemComparer = ItemDisplayPartialEquality g
  
        { new IPartialEqualityComparer<CompletionItem> with
            member x.InEqualityRelation item = itemComparer.InEqualityRelation item.Item
            member x.Equals(item1, item2) = itemComparer.Equals(item1.Item, item2.Item)
            member x.GetHashCode item = itemComparer.GetHashCode(item.Item) }

    let ItemWithTypeDisplayPartialEquality g = 
        let itemComparer = ItemDisplayPartialEquality g
        
        { new IPartialEqualityComparer<Item * _> with
            member x.InEqualityRelation ((item, _)) = itemComparer.InEqualityRelation item
            member x.Equals((item1, _), (item2, _)) = itemComparer.Equals(item1, item2)
            member x.GetHashCode ((item, _)) = itemComparer.GetHashCode item }
    
    // Remove items containing the same module references
    let RemoveDuplicateModuleRefs modrefs  = 
        modrefs |> IPartialEqualityComparer.partialDistinctBy 
                      { new IPartialEqualityComparer<ModuleOrNamespaceRef> with
                          member x.InEqualityRelation _ = true
                          member x.Equals(item1, item2) = (fullDisplayTextOfModRef item1 = fullDisplayTextOfModRef item2)
                          member x.GetHashCode item = hash item.Stamp  }

    /// Remove all duplicate items
    let RemoveDuplicateItems g (items: ItemWithInst list) = 
        items |> IPartialEqualityComparer.partialDistinctBy (IPartialEqualityComparer.On (fun item -> item.Item) (ItemDisplayPartialEquality g))

    /// Remove all duplicate items
    let RemoveDuplicateCompletionItems g items = 
        items |> IPartialEqualityComparer.partialDistinctBy (CompletionItemDisplayPartialEquality g) 

    let IsExplicitlySuppressed (g: TcGlobals) (item: Item) = 
        // This may explore assemblies that are not in the reference set.
        // In this case just assume the item is not suppressed.
        protectAssemblyExploration true (fun () -> 
         match item with 
         | Item.Types(it, [ty]) -> 
             isAppTy g ty &&
             g.suppressed_types 
             |> List.exists (fun supp -> 
                let generalizedSupp = generalizedTyconRef supp
                // check the display name is precisely the one we're suppressing
                isAppTy g generalizedSupp && it = supp.DisplayName &&
                // check if they are the same logical type (after removing all abbreviations)
                let tcr1 = tcrefOfAppTy g ty
                let tcr2 = tcrefOfAppTy g generalizedSupp
                tyconRefEq g tcr1 tcr2) 
         | _ -> false)

    /// Filter types that are explicitly suppressed from the IntelliSense (such as uppercase "FSharpList", "Option", etc.)
    let RemoveExplicitlySuppressed (g: TcGlobals) (items: ItemWithInst list) = 
      items |> List.filter (fun item -> not (IsExplicitlySuppressed g item.Item))

    /// Filter types that are explicitly suppressed from the IntelliSense (such as uppercase "FSharpList", "Option", etc.)
    let RemoveExplicitlySuppressedCompletionItems (g: TcGlobals) (items: CompletionItem list) = 
      items |> List.filter (fun item -> not (IsExplicitlySuppressed g item.Item))

    let SimplerDisplayEnv denv = 
        { denv with suppressInlineKeyword=true
                    shortConstraints=true
                    showConstraintTyparAnnotations=false
                    abbreviateAdditionalConstraints=false
                    suppressNestedTypes=true
                    maxMembers=Some EnvMisc2.maxMembers }

    let rec FullNameOfItem g item = 
        let denv = DisplayEnv.Empty g
        match item with
        | Item.ImplicitOp(_, { contents = Some(TraitConstraintSln.FSMethSln(_, vref, _)) }) 
        | Item.Value vref | Item.CustomBuilder (_, vref) -> fullDisplayTextOfValRef vref
        | Item.UnionCase (ucinfo, _) -> fullDisplayTextOfUnionCaseRef  ucinfo.UnionCaseRef
        | Item.ActivePatternResult(apinfo, _ty, idx, _) -> apinfo.Names.[idx]
        | Item.ActivePatternCase apref -> FullNameOfItem g (Item.Value apref.ActivePatternVal)  + "." + apref.Name 
        | Item.ExnCase ecref -> fullDisplayTextOfExnRef ecref 
        | Item.AnonRecdField(anon, _argTys, i, _) -> anon.SortedNames.[i]
        | Item.RecdField rfinfo -> fullDisplayTextOfRecdFieldRef  rfinfo.RecdFieldRef
        | Item.NewDef id -> id.idText
        | Item.ILField finfo -> bufs (fun os -> NicePrint.outputILTypeRef denv os finfo.ILTypeRef; bprintf os ".%s" finfo.FieldName)
        | Item.Event einfo -> bufs (fun os -> NicePrint.outputTyconRef denv os einfo.DeclaringTyconRef; bprintf os ".%s" einfo.EventName)
        | Item.Property(_, (pinfo :: _)) -> bufs (fun os -> NicePrint.outputTyconRef denv os pinfo.DeclaringTyconRef; bprintf os ".%s" pinfo.PropertyName)
        | Item.CustomOperation (customOpName, _, _) -> customOpName
        | Item.CtorGroup(_, minfo :: _) -> bufs (fun os -> NicePrint.outputTyconRef denv os minfo.DeclaringTyconRef)
        | Item.MethodGroup(_, _, Some minfo) -> bufs (fun os -> NicePrint.outputTyconRef denv os minfo.DeclaringTyconRef; bprintf os ".%s" minfo.DisplayName)        
        | Item.MethodGroup(_, minfo :: _, _) -> bufs (fun os -> NicePrint.outputTyconRef denv os minfo.DeclaringTyconRef; bprintf os ".%s" minfo.DisplayName)        
        | Item.UnqualifiedType (tcref :: _) -> bufs (fun os -> NicePrint.outputTyconRef denv os tcref)
        | Item.FakeInterfaceCtor ty 
        | Item.DelegateCtor ty 
        | Item.Types(_, ty :: _) -> 
            match tryDestAppTy g ty with
            | ValueSome tcref -> bufs (fun os -> NicePrint.outputTyconRef denv os tcref)
            | _ -> ""
        | Item.ModuleOrNamespaces((modref :: _) as modrefs) -> 
            let definiteNamespace = modrefs |> List.forall (fun modref -> modref.IsNamespace)
            if definiteNamespace then fullDisplayTextOfModRef modref else modref.DemangledModuleOrNamespaceName
        | Item.TypeVar (id, _) -> id
        | Item.ArgName (id, _, _) -> id.idText
        | Item.SetterArg (_, item) -> FullNameOfItem g item
        | Item.ImplicitOp(id, _) -> id.idText
        // unreachable 
        | Item.UnqualifiedType([]) 
        | Item.Types(_, []) 
        | Item.CtorGroup(_, []) 
        | Item.MethodGroup(_, [], _) 
        | Item.ModuleOrNamespaces []
        | Item.Property(_, []) -> ""

    /// Output a the description of a language item
    let rec GetXmlCommentForItem (infoReader: InfoReader) m item = 
        let g = infoReader.g
        match item with
        | Item.ImplicitOp(_, { contents = Some(TraitConstraintSln.FSMethSln(_, vref, _)) }) -> 
            GetXmlCommentForItem infoReader m (Item.Value vref)

        | Item.Value vref | Item.CustomBuilder (_, vref) ->            
            GetXmlCommentForItemAux (if valRefInThisAssembly g.compilingFslib vref || vref.XmlDoc.NonEmpty then Some vref.XmlDoc else None) infoReader m item 

        | Item.UnionCase(ucinfo, _) -> 
            GetXmlCommentForItemAux (if tyconRefUsesLocalXmlDoc g.compilingFslib ucinfo.TyconRef || ucinfo.UnionCase.XmlDoc.NonEmpty then Some ucinfo.UnionCase.XmlDoc else None) infoReader m item 

        | Item.ActivePatternCase apref -> 
            GetXmlCommentForItemAux (Some apref.ActivePatternVal.XmlDoc) infoReader m item 

        | Item.ExnCase ecref -> 
            GetXmlCommentForItemAux (if tyconRefUsesLocalXmlDoc g.compilingFslib ecref || ecref.XmlDoc.NonEmpty then Some ecref.XmlDoc else None) infoReader m item 

        | Item.RecdField rfinfo ->
            GetXmlCommentForItemAux (if tyconRefUsesLocalXmlDoc g.compilingFslib rfinfo.TyconRef || rfinfo.TyconRef.XmlDoc.NonEmpty then Some rfinfo.RecdField.XmlDoc else None) infoReader m item 

        | Item.Event einfo ->
            GetXmlCommentForItemAux (if einfo.HasDirectXmlComment || einfo.XmlDoc.NonEmpty then Some einfo.XmlDoc else None) infoReader m item 

        | Item.Property(_, pinfos) -> 
            let pinfo = pinfos.Head
            GetXmlCommentForItemAux (if pinfo.HasDirectXmlComment || pinfo.XmlDoc.NonEmpty then Some pinfo.XmlDoc else None) infoReader m item 

        | Item.CustomOperation (_, _, Some minfo) 
        | Item.CtorGroup(_, minfo :: _) 
        | Item.MethodGroup(_, minfo :: _, _) ->
            GetXmlCommentForMethInfoItem infoReader m item minfo

        | Item.Types(_, ((TType_app(tcref, _)) :: _)) -> 
            GetXmlCommentForItemAux (if tyconRefUsesLocalXmlDoc g.compilingFslib tcref  || tcref.XmlDoc.NonEmpty then Some tcref.XmlDoc else None) infoReader m item 

        | Item.ModuleOrNamespaces((modref :: _) as modrefs) -> 
            let definiteNamespace = modrefs |> List.forall (fun modref -> modref.IsNamespace)
            if not definiteNamespace then
                GetXmlCommentForItemAux (if entityRefInThisAssembly g.compilingFslib modref || modref.XmlDoc.NonEmpty  then Some modref.XmlDoc else None) infoReader m item 
            else
                GetXmlCommentForItemAux None infoReader m item

        | Item.ArgName (_, _, argContainer) -> 
            let xmldoc = 
                match argContainer with
                | Some(ArgumentContainer.Method minfo) ->
                    if minfo.HasDirectXmlComment || minfo.XmlDoc.NonEmpty  then Some minfo.XmlDoc else None 
                | Some(ArgumentContainer.Type tcref) ->
                    if tyconRefUsesLocalXmlDoc g.compilingFslib tcref || tcref.XmlDoc.NonEmpty  then Some tcref.XmlDoc else None
                | Some(ArgumentContainer.UnionCase ucinfo) ->
                    if tyconRefUsesLocalXmlDoc g.compilingFslib ucinfo.TyconRef || ucinfo.UnionCase.XmlDoc.NonEmpty then Some ucinfo.UnionCase.XmlDoc else None
                | _ -> None
            GetXmlCommentForItemAux xmldoc infoReader m item

        | Item.SetterArg (_, item) -> 
            GetXmlCommentForItem infoReader m item
        
        // In all these cases, there is no direct XML documentation from F# comments
        | Item.ActivePatternResult _ 
        | Item.NewDef _
        | Item.ILField _
        | Item.FakeInterfaceCtor _
        | Item.DelegateCtor _
        |  _ -> 
            GetXmlCommentForItemAux None infoReader m item

    let IsAttribute (infoReader: InfoReader) item =
        try
            let g = infoReader.g
            let amap = infoReader.amap
            match item with
            | Item.Types(_, ((TType_app(tcref, _)) :: _))
            | Item.UnqualifiedType(tcref :: _) ->
                let ty = generalizedTyconRef tcref
                Infos.ExistsHeadTypeInEntireHierarchy g amap range0 ty g.tcref_System_Attribute
            | _ -> false
        with _ -> false

    /// Output the quick info information of a language item
    let rec FormatItemDescriptionToToolTipElement isListItem (infoReader: InfoReader) m denv (item: ItemWithInst) = 
        let g = infoReader.g
        let amap = infoReader.amap
        let denv = SimplerDisplayEnv denv 
        let xml = GetXmlCommentForItem infoReader m item.Item
        match item.Item with
        | Item.ImplicitOp(_, { contents = Some(TraitConstraintSln.FSMethSln(_, vref, _)) }) -> 
            // operator with solution
            FormatItemDescriptionToToolTipElement isListItem infoReader m denv { item with Item = Item.Value vref }

        | Item.Value vref | Item.CustomBuilder (_, vref) ->            
            let prettyTyparInst, resL = NicePrint.layoutQualifiedValOrMember denv item.TyparInst vref.Deref
            let remarks = OutputFullName isListItem pubpathOfValRef fullDisplayTextOfValRefAsLayout vref
            let tpsL = FormatTyparMapping denv prettyTyparInst
            FSharpStructuredToolTipElement.Single(resL, xml, tpsL, remarks=remarks)

        // Union tags (constructors)
        | Item.UnionCase(ucinfo, _) -> 
            let uc = ucinfo.UnionCase 
            let rty = generalizedTyconRef ucinfo.TyconRef
            let recd = uc.RecdFields 
            let layout = 
                wordL (tagText (FSComp.SR.typeInfoUnionCase())) ^^
                NicePrint.layoutTyconRef denv ucinfo.TyconRef ^^
                sepL (tagPunctuation ".") ^^
                wordL (tagUnionCase (DecompileOpName uc.Id.idText) |> mkNav uc.DefinitionRange) ^^
                RightL.colon ^^
                (if List.isEmpty recd then emptyL else NicePrint.layoutUnionCases denv recd ^^ WordL.arrow) ^^
                NicePrint.layoutType denv rty
            FSharpStructuredToolTipElement.Single (layout, xml)

        // Active pattern tag inside the declaration (result)             
        | Item.ActivePatternResult(apinfo, ty, idx, _) ->
            let items = apinfo.ActiveTags
            let layout = 
                wordL (tagText ((FSComp.SR.typeInfoActivePatternResult()))) ^^
                wordL (tagActivePatternResult (List.item idx items) |> mkNav apinfo.Range) ^^
                RightL.colon ^^
                NicePrint.layoutType denv ty
            FSharpStructuredToolTipElement.Single (layout, xml)

        // Active pattern tags 
        | Item.ActivePatternCase apref -> 
            let v = apref.ActivePatternVal
            // Format the type parameters to get e.g. ('a -> 'a) rather than ('?1234 -> '?1234)
            let tau = v.TauType
            // REVIEW: use _cxs here
            let (prettyTyparInst, ptau), _cxs = PrettyTypes.PrettifyInstAndType denv.g (item.TyparInst, tau)
            let remarks = OutputFullName isListItem pubpathOfValRef fullDisplayTextOfValRefAsLayout v
            let layout =
                wordL (tagText (FSComp.SR.typeInfoActiveRecognizer())) ^^
                wordL (tagActivePatternCase apref.Name |> mkNav v.DefinitionRange) ^^
                RightL.colon ^^
                NicePrint.layoutType denv ptau

            let tpsL = FormatTyparMapping denv prettyTyparInst

            FSharpStructuredToolTipElement.Single (layout, xml, tpsL, remarks=remarks)

        // F# exception names
        | Item.ExnCase ecref -> 
            let layout = NicePrint.layoutExnDef denv ecref.Deref
            let remarks= OutputFullName isListItem pubpathOfTyconRef fullDisplayTextOfExnRefAsLayout ecref
            FSharpStructuredToolTipElement.Single (layout, xml, remarks=remarks)

        // F# record field names
        | Item.RecdField rfinfo ->
            let rfield = rfinfo.RecdField
            let ty, _cxs = PrettyTypes.PrettifyType g rfinfo.FieldType
            let layout = 
                NicePrint.layoutTyconRef denv rfinfo.TyconRef ^^
                SepL.dot ^^
                wordL (tagRecordField (DecompileOpName rfield.Name) |> mkNav rfield.DefinitionRange) ^^
                RightL.colon ^^
                NicePrint.layoutType denv ty ^^
                (
                    match rfinfo.LiteralValue with
                    | None -> emptyL
                    | Some lit -> try WordL.equals ^^  NicePrint.layoutConst denv.g ty lit with _ -> emptyL
                )
            FSharpStructuredToolTipElement.Single (layout, xml)

        // Not used
        | Item.NewDef id -> 
            let layout = 
                wordL (tagText (FSComp.SR.typeInfoPatternVariable())) ^^
                wordL (tagUnknownEntity id.idText)
            FSharpStructuredToolTipElement.Single (layout, xml)

        // .NET fields
        | Item.ILField finfo ->
            let layout = 
                wordL (tagText (FSComp.SR.typeInfoField())) ^^
                NicePrint.layoutILTypeRef denv finfo.ILTypeRef ^^
                SepL.dot ^^
                wordL (tagField finfo.FieldName) ^^
                RightL.colon ^^
                NicePrint.layoutType denv (finfo.FieldType(amap, m)) ^^
                (
                    match finfo.LiteralValue with
                    | None -> emptyL
                    | Some v ->
                        WordL.equals ^^
                        try NicePrint.layoutConst denv.g (finfo.FieldType(infoReader.amap, m)) (TypeChecker.TcFieldInit m v) with _ -> emptyL
                )
            FSharpStructuredToolTipElement.Single (layout, xml)

        // .NET events
        | Item.Event einfo ->
            let rty = PropTypOfEventInfo infoReader m AccessibleFromSomewhere einfo
            let rty, _cxs = PrettyTypes.PrettifyType g rty
            let layout =
                wordL (tagText (FSComp.SR.typeInfoEvent())) ^^
                NicePrint.layoutTyconRef denv einfo.ApparentEnclosingTyconRef ^^
                SepL.dot ^^
                wordL (tagEvent einfo.EventName) ^^
                RightL.colon ^^
                NicePrint.layoutType denv rty
            FSharpStructuredToolTipElement.Single (layout, xml)

        // F# and .NET properties
        | Item.Property(_, pinfo :: _) -> 
            let layout = NicePrint.prettyLayoutOfPropInfoFreeStyle  g amap m denv pinfo
            FSharpStructuredToolTipElement.Single (layout, xml)

        // Custom operations in queries
        | Item.CustomOperation (customOpName, usageText, Some minfo) -> 

            // Build 'custom operation: where (bool)
            //        
            //        Calls QueryBuilder.Where'
            let layout = 
                wordL (tagText (FSComp.SR.typeInfoCustomOperation())) ^^
                RightL.colon ^^
                (
                    match usageText() with
                    | Some t -> wordL (tagText t)
                    | None ->
                        let argTys = ParamNameAndTypesOfUnaryCustomOperation g minfo |> List.map (fun (ParamNameAndType(_, ty)) -> ty)
                        let argTys, _ = PrettyTypes.PrettifyTypes g argTys 
                        wordL (tagMethod customOpName) ^^ sepListL SepL.space (List.map (fun ty -> LeftL.leftParen ^^ NicePrint.layoutType denv ty ^^ SepL.rightParen) argTys)
                ) ^^
                SepL.lineBreak ^^ SepL.lineBreak  ^^
                wordL (tagText (FSComp.SR.typeInfoCallsWord())) ^^
                NicePrint.layoutTyconRef denv minfo.ApparentEnclosingTyconRef ^^
                SepL.dot ^^
                wordL (tagMethod minfo.DisplayName)

            FSharpStructuredToolTipElement.Single (layout, xml)

        // F# constructors and methods
        | Item.CtorGroup(_, minfos) 
        | Item.MethodGroup(_, minfos, _) ->
            FormatOverloadsToList infoReader m denv item minfos
        
        // The 'fake' zero-argument constructors of .NET interfaces.
        // This ideally should never appear in intellisense, but we do get here in repros like:
        //     type IFoo = abstract F : int
        //     type II = IFoo  // remove 'type II = ' and quickly hover over IFoo before it gets squiggled for 'invalid use of interface type'
        // and in that case we'll just show the interface type name.
        | Item.FakeInterfaceCtor ty ->
           let ty, _ = PrettyTypes.PrettifyType g ty
           let layout = NicePrint.layoutTyconRef denv (tcrefOfAppTy g ty)
           FSharpStructuredToolTipElement.Single(layout, xml)
        
        // The 'fake' representation of constructors of .NET delegate types
        | Item.DelegateCtor delty -> 
           let delty, _cxs = PrettyTypes.PrettifyType g delty
           let (SigOfFunctionForDelegate(_, _, _, fty)) = GetSigOfFunctionForDelegate infoReader delty m AccessibleFromSomewhere
           let layout =
               NicePrint.layoutTyconRef denv (tcrefOfAppTy g delty) ^^
               LeftL.leftParen ^^
               NicePrint.layoutType denv fty ^^
               RightL.rightParen
           FSharpStructuredToolTipElement.Single(layout, xml)

        // Types.
        | Item.Types(_, ((TType_app(tcref, _)) :: _))
        | Item.UnqualifiedType (tcref :: _) -> 
            let denv = { denv with shortTypeNames = true  }
            let layout = NicePrint.layoutTycon denv infoReader AccessibleFromSomewhere m (* width *) tcref.Deref
            let remarks = OutputFullName isListItem pubpathOfTyconRef fullDisplayTextOfTyconRefAsLayout tcref
            FSharpStructuredToolTipElement.Single (layout, xml, remarks=remarks)

        // F# Modules and namespaces
        | Item.ModuleOrNamespaces((modref :: _) as modrefs) -> 
            //let os = StringBuilder()
            let modrefs = modrefs |> RemoveDuplicateModuleRefs
            let definiteNamespace = modrefs |> List.forall (fun modref -> modref.IsNamespace)
            let kind = 
                if definiteNamespace then FSComp.SR.typeInfoNamespace()
                elif modrefs |> List.forall (fun modref -> modref.IsModule) then FSComp.SR.typeInfoModule()
                else FSComp.SR.typeInfoNamespaceOrModule()
            
            let layout = 
                wordL (tagKeyword kind) ^^
                (if definiteNamespace then tagNamespace (fullDisplayTextOfModRef modref) else (tagModule modref.DemangledModuleOrNamespaceName)
                 |> mkNav modref.DefinitionRange
                 |> wordL)
            if not definiteNamespace then
                let namesToAdd = 
                    ([], modrefs) 
                    ||> Seq.fold (fun st modref -> 
                        match fullDisplayTextOfParentOfModRef modref with 
                        | ValueSome txt -> txt :: st 
                        | _ -> st) 
                    |> Seq.mapi (fun i x -> i, x) 
                    |> Seq.toList
                let layout =
                    layout ^^
                    (
                        if not (List.isEmpty namesToAdd) then
                            SepL.lineBreak ^^
                            List.fold ( fun s (i, txt) ->
                                s ^^
                                SepL.lineBreak ^^
                                wordL (tagText ((if i = 0 then FSComp.SR.typeInfoFromFirst else FSComp.SR.typeInfoFromNext) txt))
                            ) emptyL namesToAdd 
                        else 
                            emptyL
                    )
                FSharpStructuredToolTipElement.Single (layout, xml)
            else
                FSharpStructuredToolTipElement.Single (layout, xml)

        | Item.AnonRecdField(anon, argTys, i, _) -> 
            let argTy = argTys.[i]
            let nm = anon.SortedNames.[i]
            let argTy, _ = PrettyTypes.PrettifyType g argTy
            let layout =
                wordL (tagText (FSComp.SR.typeInfoAnonRecdField())) ^^
                wordL (tagRecordField nm) ^^
                RightL.colon ^^
                NicePrint.layoutType denv argTy
            FSharpStructuredToolTipElement.Single (layout, FSharpXmlDoc.None)
            
        // Named parameters
        | Item.ArgName (id, argTy, _) -> 
            let argTy, _ = PrettyTypes.PrettifyType g argTy
            let layout =
                wordL (tagText (FSComp.SR.typeInfoArgument())) ^^
                wordL (tagParameter id.idText) ^^
                RightL.colon ^^
                NicePrint.layoutType denv argTy
            FSharpStructuredToolTipElement.Single (layout, xml, paramName = id.idText)
            
        | Item.SetterArg (_, item) -> 
            FormatItemDescriptionToToolTipElement isListItem infoReader m denv (ItemWithNoInst item)

        |  _ -> 
            FSharpStructuredToolTipElement.None

#if !NO_EXTENSIONTYPING

    /// Determine if an item is a provided type 
    let (|ItemIsProvidedType|_|) g item =
        match item with
        | Item.Types(_name, tys) ->
            match tys with
            | [AppTy g (tyconRef, _typeInst)] ->
                if tyconRef.IsProvidedErasedTycon || tyconRef.IsProvidedGeneratedTycon then
                    Some tyconRef
                else
                    None
            | _ -> None
        | _ -> None

    /// Determine if an item is a provided type that has static parameters
    let (|ItemIsProvidedTypeWithStaticArguments|_|) m g item =
        match item with
        | Item.Types(_name, tys) ->
            match tys with
            | [AppTy g (tyconRef, _typeInst)] ->
                if tyconRef.IsProvidedErasedTycon || tyconRef.IsProvidedGeneratedTycon then
                    let typeBeforeArguments = 
                        match tyconRef.TypeReprInfo with 
                        | TProvidedTypeExtensionPoint info -> info.ProvidedType
                        | _ -> failwith "unreachable"
                    let staticParameters = typeBeforeArguments.PApplyWithProvider((fun (typeBeforeArguments, provider) -> typeBeforeArguments.GetStaticParameters provider), range=m) 
                    let staticParameters = staticParameters.PApplyArray(id, "GetStaticParameters", m)
                    Some staticParameters
                else
                    None
            | _ -> None
        | _ -> None


    let (|ItemIsProvidedMethodWithStaticArguments|_|) item =
        match item with
        // Prefer the static parameters from the uninstantiated method info
        | Item.MethodGroup(_, _, Some minfo) ->
            match minfo.ProvidedStaticParameterInfo  with 
            | Some (_, staticParameters) -> Some staticParameters
            | _ -> None
        | Item.MethodGroup(_, [minfo], _) ->
            match minfo.ProvidedStaticParameterInfo  with 
            | Some (_, staticParameters) -> Some staticParameters
            | _ -> None
        | _ -> None

    /// Determine if an item has static arguments
    let (|ItemIsWithStaticArguments|_|) m g item =
        match item with
        | ItemIsProvidedTypeWithStaticArguments m g staticParameters -> Some staticParameters
        | ItemIsProvidedMethodWithStaticArguments staticParameters -> Some staticParameters
        | _ -> None

#endif

    /// Get the "F1 Keyword" associated with an item, for looking up documentatio help indexes on the web
    let rec GetF1Keyword g item = 

        let getKeywordForMethInfo (minfo : MethInfo) =
            match minfo with 
            | FSMeth(_, _, vref, _) ->
                match vref.DeclaringEntity with
                | Parent tcref ->
                    (tcref |> ticksAndArgCountTextOfTyconRef)+"."+vref.CompiledName|> Some
                | ParentNone -> None
                
            | ILMeth (_, minfo, _) ->
                let typeString = minfo.DeclaringTyconRef |> ticksAndArgCountTextOfTyconRef
                let paramString =
                    let nGenericParams = minfo.RawMetadata.GenericParams.Length 
                    if nGenericParams > 0 then "``"+(nGenericParams.ToString()) else ""
                sprintf "%s.%s%s" typeString minfo.RawMetadata.Name paramString |> Some

            | DefaultStructCtor _  -> None
#if !NO_EXTENSIONTYPING
            | ProvidedMeth _ -> None
#endif
             
        match item with
        | Item.Value vref | Item.CustomBuilder (_, vref) -> 
            let v = vref.Deref
            if v.IsModuleBinding && v.HasDeclaringEntity then
                let tyconRef = v.TopValDeclaringEntity
                let paramsString =
                    match v.Typars with
                    |   [] -> ""
                    |   l -> "``"+(List.length l).ToString() 
                
                sprintf "%s.%s%s" (tyconRef |> ticksAndArgCountTextOfTyconRef) v.CompiledName paramsString |> Some
            else
                None

        | Item.ActivePatternCase apref -> 
            GetF1Keyword g (Item.Value apref.ActivePatternVal)

        | Item.UnionCase(ucinfo, _) -> 
            (ucinfo.TyconRef |> ticksAndArgCountTextOfTyconRef)+"."+ucinfo.Name |> Some

        | Item.RecdField rfi -> 
            (rfi.TyconRef |> ticksAndArgCountTextOfTyconRef)+"."+rfi.Name |> Some
        
        | Item.AnonRecdField _ -> None 
        
        | Item.ILField finfo ->   
             match finfo with 
             | ILFieldInfo(tinfo, fdef) -> 
                 (tinfo.TyconRefOfRawMetadata |> ticksAndArgCountTextOfTyconRef)+"."+fdef.Name |> Some
#if !NO_EXTENSIONTYPING
             | ProvidedField _ -> None
#endif
        | Item.Types(_, ((AppTy g (tcref, _)) :: _)) 
        | Item.DelegateCtor(AppTy g (tcref, _))
        | Item.FakeInterfaceCtor(AppTy g (tcref, _))
        | Item.UnqualifiedType (tcref :: _)
        | Item.ExnCase tcref -> 
            // strip off any abbreviation
            match generalizedTyconRef tcref with 
            | AppTy g (tcref, _)  -> Some (ticksAndArgCountTextOfTyconRef tcref)
            | _ -> None

        // Pathological cases of the above
        | Item.Types _ 
        | Item.DelegateCtor _
        | Item.FakeInterfaceCtor _
        | Item.UnqualifiedType [] -> 
            None

        | Item.ModuleOrNamespaces modrefs -> 
            match modrefs with 
            | modref :: _ -> 
                // namespaces from type providers need to be handled separately because they don't have compiled representation
                // otherwise we'll fail at tast.fs
                match modref.Deref.TypeReprInfo with
#if !NO_EXTENSIONTYPING                
                | TProvidedNamespaceExtensionPoint _ -> 
                    modref.CompilationPathOpt
                    |> Option.bind (fun path ->
                        // works similar to generation of xml-docs at tastops.fs, probably too similar
                        // TODO: check if this code can be implemented using xml-doc generation functionality
                        let prefix = path.AccessPath |> Seq.map fst |> String.concat "."
                        let fullName = if prefix = "" then modref.CompiledName else prefix + "." + modref.CompiledName
                        Some fullName
                        )
#endif
                | _ -> modref.Deref.CompiledRepresentationForNamedType.FullName |> Some
            | [] ->  None // Pathological case of the above

        | Item.Property(_, (pinfo :: _)) -> 
            match pinfo with 
            | FSProp(_, _, Some vref, _) 
            | FSProp(_, _, _, Some vref) -> 
                // per spec, extension members in F1 keywords are qualified with definition class
                match vref.DeclaringEntity with 
                | Parent tcref ->
                    (tcref |> ticksAndArgCountTextOfTyconRef)+"."+vref.PropertyName|> Some                     
                | ParentNone -> None

            | ILProp(ILPropInfo(tinfo, pdef)) -> 
                let tcref = tinfo.TyconRefOfRawMetadata
                (tcref |> ticksAndArgCountTextOfTyconRef)+"."+pdef.Name |> Some
            | FSProp _ -> None
#if !NO_EXTENSIONTYPING
            | ProvidedProp _ -> None
#endif
        | Item.Property(_, []) -> None // Pathological case of the above
                   
        | Item.Event einfo -> 
            match einfo with 
            | ILEvent _  ->
                let tcref = einfo.DeclaringTyconRef
                (tcref |> ticksAndArgCountTextOfTyconRef)+"."+einfo.EventName |> Some
            | FSEvent(_, pinfo, _, _) ->
                match pinfo.ArbitraryValRef with 
                | Some vref ->
                   // per spec, members in F1 keywords are qualified with definition class
                   match vref.DeclaringEntity with 
                   | Parent tcref -> (tcref |> ticksAndArgCountTextOfTyconRef)+"."+vref.PropertyName|> Some                     
                   | ParentNone -> None
                | None -> None
#if !NO_EXTENSIONTYPING
            | ProvidedEvent _ -> None 
#endif
        | Item.CtorGroup(_, minfos) ->
            match minfos with 
            | [] -> None
            | FSMeth(_, _, vref, _) :: _ ->
                   match vref.DeclaringEntity with
                   | Parent tcref -> (tcref |> ticksAndArgCountTextOfTyconRef) + ".#ctor"|> Some
                   | ParentNone -> None
#if !NO_EXTENSIONTYPING
            | ProvidedMeth _ :: _ -> None
#endif
            | minfo :: _ ->
                let tcref = minfo.DeclaringTyconRef
                (tcref |> ticksAndArgCountTextOfTyconRef)+".#ctor" |> Some
        | Item.CustomOperation (_, _, Some minfo) -> getKeywordForMethInfo minfo
        | Item.MethodGroup(_, _, Some minfo) -> getKeywordForMethInfo minfo
        | Item.MethodGroup(_, minfo :: _, _) -> getKeywordForMethInfo minfo
        | Item.SetterArg (_, propOrField) -> GetF1Keyword g propOrField 
        | Item.MethodGroup(_, [], _) 
        | Item.CustomOperation (_, _, None)   // "into"
        | Item.NewDef _ // "let x$yz = ..." - no keyword
        | Item.ArgName _ // no keyword on named parameters 
        | Item.TypeVar _ 
        | Item.ImplicitOp _
        | Item.ActivePatternResult _ // "let (|Foo|Bar|) = .. Fo$o ..." - no keyword
            ->  None


    /// Format the structured version of a tooltip for an item
    let FormatStructuredDescriptionOfItem isListItem infoReader m denv item = 
        ErrorScope.Protect m 
            (fun () -> FormatItemDescriptionToToolTipElement isListItem infoReader m denv item)
            (fun err -> FSharpStructuredToolTipElement.CompositionError err)

    /// Get rid of groups of overloads an replace them with single items.
    let FlattenItems g (m: range) item =
        match item with 
        | Item.MethodGroup(nm, minfos, orig) -> minfos |> List.map (fun minfo -> Item.MethodGroup(nm, [minfo], orig))  
        | Item.CtorGroup(nm, cinfos) -> cinfos |> List.map (fun minfo -> Item.CtorGroup(nm, [minfo])) 
        | Item.FakeInterfaceCtor _
        | Item.DelegateCtor _ -> [item]
        | Item.NewDef _ 
        | Item.ILField _ -> []
        | Item.Event _ -> []
        | Item.RecdField rfinfo -> if isFunction g rfinfo.FieldType then [item] else []
        | Item.Value v -> if isFunction g v.Type then [item] else []
        | Item.UnionCase(ucr, _) -> if not ucr.UnionCase.IsNullary then [item] else []
        | Item.ExnCase ecr -> if isNil (recdFieldsOfExnDefRef ecr) then [] else [item]
        | Item.Property(_, pinfos) -> 
            let pinfo = List.head pinfos 
            if pinfo.IsIndexer then [item] else []
#if !NO_EXTENSIONTYPING
        | ItemIsWithStaticArguments m g _ -> [item] // we pretend that provided-types-with-static-args are method-like in order to get ParamInfo for them
#endif
        | Item.CustomOperation(_name, _helpText, _minfo) -> [item]
        | Item.TypeVar _ -> []
        | Item.CustomBuilder _ -> []
        | _ -> []

