// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Open up the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open System
open System.IO
open System.Text
open System.Threading
open System.Runtime
open System.Collections.Generic
open System.Collections.Concurrent

open Microsoft.FSharp.Core.Printf
open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.AbstractIL
open Microsoft.FSharp.Compiler.AbstractIL.IL
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics 
open Microsoft.FSharp.Compiler.AbstractIL.Internal  
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library  

open Microsoft.FSharp.Compiler.AccessibilityLogic
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.CompileOps
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.Lib
open Microsoft.FSharp.Compiler.ReferenceResolver
open Microsoft.FSharp.Compiler.PrettyNaming
open Microsoft.FSharp.Compiler.Parser
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.Lexhelp
open Microsoft.FSharp.Compiler.Layout
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.Tastops
open Microsoft.FSharp.Compiler.Tastops.DebugPrint
open Microsoft.FSharp.Compiler.TcGlobals 
open Microsoft.FSharp.Compiler.Infos
open Microsoft.FSharp.Compiler.InfoReader
open Microsoft.FSharp.Compiler.NameResolution
open Microsoft.FSharp.Compiler.TypeChecker
open Microsoft.FSharp.Compiler.SourceCodeServices.ItemDescriptionsImpl 

open Internal.Utilities
open Internal.Utilities.Collections

type internal Layout = Internal.Utilities.StructuredFormat.Layout

[<AutoOpen>]
module EnvMisc =
    let getToolTipTextSize = GetEnvInteger "FCS_RecentForegroundTypeCheckCacheSize" 5
    let maxTypeCheckErrorsOutOfProjectContext = GetEnvInteger "FCS_MaxErrorsOutOfProjectContext" 3
    let braceMatchCacheSize = GetEnvInteger "FCS_BraceMatchCacheSize" 5
    let parseFileInProjectCacheSize = GetEnvInteger "FCS_ParseFileInProjectCacheSize" 2
    let incrementalTypeCheckCacheSize = GetEnvInteger "FCS_IncrementalTypeCheckCacheSize" 5

    let projectCacheSizeDefault   = GetEnvInteger "FCS_ProjectCacheSizeDefault" 3
    let frameworkTcImportsCacheStrongSize = GetEnvInteger "FCS_frameworkTcImportsCacheStrongSizeDefault" 8
    let maxMBDefault =  GetEnvInteger "FCS_MaxMB" 1000000 // a million MB = 1TB = disabled
    //let maxMBDefault = GetEnvInteger "FCS_maxMB" (if sizeof<int> = 4 then 1700 else 3400)

    /// Maximum time share for a piece of background work before it should (cooperatively) yield
    /// to enable other requests to be serviced. Yielding means returning a continuation function
    /// (via an Eventually<_> value of case NotYetDone) that can be called as the next piece of work. 
    let maxTimeShareMilliseconds = 
        match System.Environment.GetEnvironmentVariable("FCS_MaxTimeShare") with 
        | null | "" -> 50L
        | s -> int64 s


//----------------------------------------------------------------------------
// Methods
//--------------------------------------------------------------------------

[<Sealed>]
type FSharpMethodGroupItemParameter(name: string, canonicalTypeTextForSorting: string, display: Layout, isOptional: bool) = 
    member __.ParameterName = name
    member __.CanonicalTypeTextForSorting = canonicalTypeTextForSorting
    member __.StructuredDisplay = display
    member __.Display = showL display
    member __.IsOptional = isOptional

/// Format parameters for Intellisense completion
module internal Params = 
    let printCanonicalizedTypeName g (denv:DisplayEnv) tau =
        // get rid of F# abbreviations and such
        let strippedType = stripTyEqnsWrtErasure EraseAll g tau
        // pretend no namespaces are open
        let denv = denv.SetOpenPaths([])
        // now printing will see a .NET-like canonical representation, that is good for sorting overloads into a reasonable order (see bug 94520)
        NicePrint.stringOfTy denv strippedType

    let ParamOfRecdField g denv f =
        FSharpMethodGroupItemParameter(
          name = f.rfield_id.idText,
          canonicalTypeTextForSorting = printCanonicalizedTypeName g denv f.rfield_type,
          display = NicePrint.prettyLayoutOfTy denv f.rfield_type,
          isOptional=false)
    
    let ParamOfUnionCaseField g denv isGenerated (i : int) f = 
        let initial = ParamOfRecdField g denv f
        let display = if isGenerated i f then initial.StructuredDisplay else NicePrint.layoutOfParamData denv (ParamData(false, false, NotOptional, NoCallerInfo, Some f.rfield_id, ReflectedArgInfo.None, f.rfield_type)) 
        FSharpMethodGroupItemParameter(
          name=initial.ParameterName, 
          canonicalTypeTextForSorting=initial.CanonicalTypeTextForSorting, 
          display=display,
          isOptional=false)

    let ParamOfParamData g denv (ParamData(_isParamArrayArg, _isOutArg, optArgInfo, _callerInfoInfo, nmOpt, _reflArgInfo, pty) as paramData) =
        FSharpMethodGroupItemParameter(
          name = (match nmOpt with None -> "" | Some pn -> pn.idText),
          canonicalTypeTextForSorting = printCanonicalizedTypeName g denv pty,
          display = NicePrint.layoutOfParamData denv paramData,
          isOptional=optArgInfo.IsOptional)

    // TODO this code is similar to NicePrint.fs:formatParamDataToBuffer, refactor or figure out why different?
    let ParamsOfParamDatas g denv (paramDatas:ParamData list) rty = 
        let paramInfo,paramTypes = 
            paramDatas 
            |> List.map (fun (ParamData(isParamArrayArg, _isOutArg, optArgInfo, _callerInfoInfo, nmOpt, _reflArgInfo, pty)) -> 
                let isOptArg = optArgInfo.IsOptional
                match nmOpt, isOptArg, tryDestOptionTy denv.g pty with 
                // Layout an optional argument 
                | Some id, true, ptyOpt -> 
                    let nm = id.idText
                    // detect parameter type, if ptyOpt is None - this is .NET style optional argument
                    let pty = defaultArg ptyOpt pty
                    (nm, isOptArg, SepL.questionMark ^^ (wordL (TaggedTextOps.tagParameter nm))),  pty
                // Layout an unnamed argument 
                | None, _,_ -> 
                    ("", isOptArg, emptyL), pty
                // Layout a named argument 
                | Some id,_,_ -> 
                    let nm = id.idText
                    let prefix = 
                        if isParamArrayArg then
                            NicePrint.PrintUtilities.layoutBuiltinAttribute denv denv.g.attrib_ParamArrayAttribute ^^
                            wordL (TaggedTextOps.tagParameter nm) ^^
                            RightL.colon
                            //sprintf "%s %s: " (NicePrint.PrintUtilities.layoutBuiltinAttribute denv denv.g.attrib_ParamArrayAttribute |> showL) nm 
                        else 
                            wordL (TaggedTextOps.tagParameter nm) ^^
                            RightL.colon
                            //sprintf "%s: " nm
                    (nm,isOptArg, prefix),pty)
            |> List.unzip
        let paramTypeAndRetLs,_ = NicePrint.layoutPrettifiedTypes denv (paramTypes@[rty])
        let paramTypeLs,_ = List.frontAndBack  paramTypeAndRetLs
        (paramInfo,paramTypes,paramTypeLs) |||> List.map3 (fun (nm,isOptArg,paramPrefix) tau tyL -> 
            FSharpMethodGroupItemParameter(
              name = nm,
              canonicalTypeTextForSorting = printCanonicalizedTypeName g denv tau,
              display = paramPrefix ^^ tyL,
              isOptional=isOptArg
            ))

    let ParamsOfTypes g denv args rtau = 
        let ptausL, _ = NicePrint.layoutPrettifiedTypes denv (args@[rtau]) 
        let argsL,_ = List.frontAndBack ptausL 
        let mkParam (tau,tyL) =
            FSharpMethodGroupItemParameter(
              name = "",
              canonicalTypeTextForSorting = printCanonicalizedTypeName g denv tau,
              display =  tyL,
              isOptional=false
            )
        (args,argsL) ||> List.zip |> List.map mkParam

#if EXTENSIONTYPING

    let (|ItemIsProvidedType|_|) g item =
        match item with
        | Item.Types(_name,tys) ->
            match tys with
            | [AppTy g (tyconRef,_typeInst)] ->
                if tyconRef.IsProvidedErasedTycon || tyconRef.IsProvidedGeneratedTycon then
                    Some tyconRef
                else
                    None
            | _ -> None
        | _ -> None

    let (|ItemIsProvidedTypeWithStaticArguments|_|) m g item =
        match item with
        | Item.Types(_name,tys) ->
            match tys with
            | [AppTy g (tyconRef,_typeInst)] ->
                if tyconRef.IsProvidedErasedTycon || tyconRef.IsProvidedGeneratedTycon then
                    let typeBeforeArguments = 
                        match tyconRef.TypeReprInfo with 
                        | TProvidedTypeExtensionPoint info -> info.ProvidedType
                        | _ -> failwith "unreachable"
                    let staticParameters = typeBeforeArguments.PApplyWithProvider((fun (typeBeforeArguments,provider) -> typeBeforeArguments.GetStaticParameters(provider)), range=m) 
                    let staticParameters = staticParameters.PApplyArray(id, "GetStaticParameters",m)
                    Some staticParameters
                else
                    None
            | _ -> None
        | _ -> None


    let (|ItemIsProvidedMethodWithStaticArguments|_|) item =
        match item with
        // Prefer the static parameters from the uninstantiated method info
        | Item.MethodGroup(_,_,Some minfo) ->
            match minfo.ProvidedStaticParameterInfo  with 
            | Some (_,staticParameters) -> Some staticParameters
            | _ -> None
        | Item.MethodGroup(_,[minfo],_) ->
            match minfo.ProvidedStaticParameterInfo  with 
            | Some (_,staticParameters) -> Some staticParameters
            | _ -> None
        | _ -> None

    let (|ItemIsWithStaticArguments|_|) m g item =
        match item with
        | ItemIsProvidedTypeWithStaticArguments m g staticParameters -> Some staticParameters
        | ItemIsProvidedMethodWithStaticArguments staticParameters -> Some staticParameters
        | _ -> None
#endif

    let StaticParamsOfItem (infoReader:InfoReader) m denv d = 
        let amap = infoReader.amap
        let g = infoReader.g
        match d with
#if EXTENSIONTYPING
        | ItemIsWithStaticArguments m g staticParameters ->
            staticParameters 
                |> Array.map (fun sp -> 
                    let typ = Import.ImportProvidedType amap m (sp.PApply((fun x -> x.ParameterType),m))
                    let spKind = NicePrint.prettyLayoutOfTy denv typ
                    let spName = sp.PUntaint((fun sp -> sp.Name), m)
                    let spOpt = sp.PUntaint((fun sp -> sp.IsOptional), m)
                    FSharpMethodGroupItemParameter(
                      name = spName,
                      canonicalTypeTextForSorting = showL spKind,
                      display = (if spOpt then SepL.questionMark else emptyL) ^^ wordL (TaggedTextOps.tagParameter spName) ^^ RightL.colon ^^ spKind,
                      //display = sprintf "%s%s: %s" (if spOpt then "?" else "") spName spKind,
                      isOptional=spOpt))
#endif
        | _ -> [| |]

    let rec ParamsOfItem (infoReader:InfoReader) m denv d = 
        let amap = infoReader.amap
        let g = infoReader.g
        match d with
        | Item.Value vref -> 
            let getParamsOfTypes() = 
                let _, tau = vref.TypeScheme
                if isFunTy denv.g tau then 
                    let arg,rtau = destFunTy denv.g tau 
                    let args = tryDestRefTupleTy denv.g arg 
                    ParamsOfTypes g denv args rtau
                else []
            match vref.ValReprInfo with
            | None -> 
                // ValReprInfo = None i.e. in let bindings defined in types or in local functions
                // in this case use old approach and return only information about types
                getParamsOfTypes ()
            | Some valRefInfo ->
                // ValReprInfo will exist for top-level syntactic functions
                // per spec: binding is considered to define a syntactic function if it is either a function or its immediate right-hand-side is a anonymous function
                let (_, argInfos,  returnTy, _) = GetTopValTypeInFSharpForm  g valRefInfo vref.Type m
                match argInfos with
                | [] -> 
                    // handles cases like 'let foo = List.map'
                    getParamsOfTypes() 
                | argInfo::_ ->
                    // result 'paramDatas' collection corresponds to the first argument of curried function
                    // i.e. let func (a : int) (b : int) = a + b
                    // paramDatas will contain information about a and returnTy will be: int -> int
                    // This is good enough as we don't provide ways to display info for the second curried argument
                    let paramDatas = 
                        argInfo
                        |> List.map ParamNameAndType.FromArgInfo
                        |> List.map (fun (ParamNameAndType(nmOpt, pty)) -> ParamData(false, false, NotOptional, NoCallerInfo, nmOpt, ReflectedArgInfo.None, pty))
                    ParamsOfParamDatas g denv paramDatas returnTy
        | Item.UnionCase(ucr,_)   -> 
            match ucr.UnionCase.RecdFields with
            | [f] -> [ParamOfUnionCaseField g denv NicePrint.isGeneratedUnionCaseField -1 f]
            | fs -> fs |> List.mapi (ParamOfUnionCaseField g denv NicePrint.isGeneratedUnionCaseField)
        | Item.ActivePatternCase(apref)   -> 
            let v = apref.ActivePatternVal 
            let _,tau = v.TypeScheme
            let args, _ = stripFunTy denv.g tau 
            ParamsOfTypes g denv args tau
        | Item.ExnCase(ecref)     -> 
            ecref |> recdFieldsOfExnDefRef |> List.mapi (ParamOfUnionCaseField g denv NicePrint.isGeneratedExceptionField) 
        | Item.Property(_,pinfo :: _) -> 
            let paramDatas = pinfo.GetParamDatas(amap,m)
            let rty = pinfo.GetPropertyType(amap,m) 
            ParamsOfParamDatas g denv paramDatas rty
        | Item.CtorGroup(_,(minfo :: _)) 
        | Item.MethodGroup(_,(minfo :: _),_) -> 
            let paramDatas = minfo.GetParamDatas(amap, m, minfo.FormalMethodInst) |> List.head
            let rty = minfo.GetFSharpReturnTy(amap, m, minfo.FormalMethodInst)
            ParamsOfParamDatas g denv paramDatas rty
        | Item.CustomBuilder (_,vref) -> ParamsOfItem infoReader m denv (Item.Value vref)
        | Item.TypeVar _ -> []

        | Item.CustomOperation (_,usageText, Some minfo) -> 
            match usageText() with 
            | None -> 
                let argNamesAndTys = ItemDescriptionsImpl.ParamNameAndTypesOfUnaryCustomOperation g minfo 
                let _, argTys, _ = PrettyTypes.PrettifyTypesN g (argNamesAndTys |> List.map (fun (ParamNameAndType(_,ty)) -> ty))
                let paramDatas = (argNamesAndTys, argTys) ||> List.map2 (fun (ParamNameAndType(nmOpt, _)) argTy -> ParamData(false, false, NotOptional, NoCallerInfo, nmOpt, ReflectedArgInfo.None,argTy))
                let rty = minfo.GetFSharpReturnTy(amap, m, minfo.FormalMethodInst)
                ParamsOfParamDatas g denv paramDatas rty
            | Some _ -> 
                [] // no parameter data available for binary operators like 'zip', 'join' and 'groupJoin' since they use bespoke syntax 

        | Item.FakeInterfaceCtor _ -> []
        | Item.DelegateCtor delty -> 
            let (SigOfFunctionForDelegate(_, _, _, fty)) = GetSigOfFunctionForDelegate infoReader delty m AccessibleFromSomeFSharpCode
            ParamsOfParamDatas g denv [ParamData(false, false, NotOptional, NoCallerInfo, None, ReflectedArgInfo.None, fty)] delty
        |  _ -> []


/// A single method for Intellisense completion
[<Sealed; NoEquality; NoComparison>]
// Note: instances of this type do not hold any references to any compiler resources.
type FSharpMethodGroupItem(description: FSharpToolTipText<Layout>, xmlDoc: FSharpXmlDoc, typeText: Layout, parameters: FSharpMethodGroupItemParameter[], hasParameters: bool, hasParamArrayArg: bool, staticParameters: FSharpMethodGroupItemParameter[]) = 
    member __.StructuredDescription = description
    member __.Description = Tooltips.ToFSharpToolTipText description
    member __.XmlDoc = xmlDoc
    member __.StructuredTypeText = typeText
    member __.TypeText = showL typeText
    member __.Parameters = parameters
    member __.HasParameters = hasParameters
    member __.HasParamArrayArg = hasParamArrayArg
    // Does the type name or method support a static arguments list, like TP<42,"foo"> or conn.CreateCommand<42, "foo">(arg1, arg2)?
    member __.StaticParameters = staticParameters


/// A table of methods for Intellisense completion
//
// Note: this type does not hold any strong references to any compiler resources, nor does evaluating any of the properties execute any
// code on the compiler thread.  
[<Sealed>]
type FSharpMethodGroup( name: string, unsortedMethods: FSharpMethodGroupItem[] ) = 
    // BUG 413009 : [ParameterInfo] takes about 3 seconds to move from one overload parameter to another
    // cache allows to avoid recomputing parameterinfo for the same item
#if !FX_NO_WEAKTABLE
    static let methodOverloadsCache = System.Runtime.CompilerServices.ConditionalWeakTable()
#endif

    let methods = 
        unsortedMethods 
        // Methods with zero arguments show up here as taking a single argument of type 'unit'.  Patch them now to appear as having zero arguments.
        |> Array.map (fun meth -> 
            let parms = meth.Parameters
            if parms.Length = 1 && parms.[0].CanonicalTypeTextForSorting="Microsoft.FSharp.Core.Unit" then 
                FSharpMethodGroupItem(meth.StructuredDescription, meth.XmlDoc, meth.StructuredTypeText, [||], true, meth.HasParamArrayArg, meth.StaticParameters) 
            else 
                meth)
        // Fix the order of methods, to be stable for unit testing.
        |> Array.sortBy (fun meth -> 
            let parms = meth.Parameters
            parms.Length, (parms |> Array.map (fun p -> p.CanonicalTypeTextForSorting)))
    member x.MethodName = name
    member x.Methods = methods

    static member Create(infoReader:InfoReader,m,denv,items:Item list) = 
        let g = infoReader.g
        if isNil items then new FSharpMethodGroup("", [| |]) else
        let name = items.Head.DisplayName 
        let getOverloadsForItem item =
#if !FX_NO_WEAKTABLE
            match methodOverloadsCache.TryGetValue item with
            | true, overloads -> overloads
            | false, _ ->
#endif
                let items =
                    match item with 
                    | Item.CtorGroup(nm,cinfos) -> List.map (fun minfo -> Item.CtorGroup(nm,[minfo])) cinfos 
                    | Item.FakeInterfaceCtor _
                    | Item.DelegateCtor _ -> [item]
                    | Item.NewDef _ 
                    | Item.ILField _ -> []
                    | Item.Event _ -> []
                    | Item.RecdField(rfinfo) -> 
                        if isFunction g rfinfo.FieldType then [item] else []
                    | Item.Value v -> 
                        if isFunction g v.Type then [item] else []
                    | Item.UnionCase(ucr,_) -> 
                        if not ucr.UnionCase.IsNullary then [item] else []
                    | Item.ExnCase(ecr) -> 
                        if isNil (recdFieldsOfExnDefRef ecr) then [] else [item]
                    | Item.Property(_,pinfos) -> 
                        let pinfo = List.head pinfos 
                        if pinfo.IsIndexer then [item] else []
#if EXTENSIONTYPING
                    | Params.ItemIsWithStaticArguments m g _ -> [item] // we pretend that provided-types-with-static-args are method-like in order to get ParamInfo for them
#endif
                    | Item.MethodGroup(nm,minfos,orig) -> minfos |> List.map (fun minfo -> Item.MethodGroup(nm,[minfo],orig)) 
                    | Item.CustomOperation(_name, _helpText, _minfo) -> [item]
                    | Item.TypeVar _ -> []
                    | Item.CustomBuilder _ -> []
                    | _ -> []

                let methods = 
                    items |> Array.ofList |> Array.map (fun item -> 
                        FSharpMethodGroupItem(
                          description = FSharpToolTipText [FormatStructuredDescriptionOfItem true infoReader m denv item],
                          typeText = FormatStructuredReturnTypeOfItem infoReader m denv item,
                          xmlDoc = GetXmlCommentForItem infoReader m item,
                          parameters = (Params.ParamsOfItem infoReader m denv item |> Array.ofList),
                          hasParameters = (match item with Params.ItemIsProvidedTypeWithStaticArguments m g _ -> false | _ -> true),
                          hasParamArrayArg = (match item with Item.CtorGroup(_,[meth]) | Item.MethodGroup(_,[meth],_) -> meth.HasParamArrayArg(infoReader.amap, m, meth.FormalMethodInst) | _ -> false),
                          staticParameters = Params.StaticParamsOfItem infoReader m denv item
                        ))
#if !FX_NO_WEAKTABLE
                methodOverloadsCache.Add(item, methods)
#endif
                methods
        let methods = [| for item in items do yield! getOverloadsForItem item |]

        new FSharpMethodGroup(name, methods)

//----------------------------------------------------------------------------
// Scopes. 
//--------------------------------------------------------------------------

[<RequireQualifiedAccess>]
type (*internal*) FSharpFindDeclFailureReason = 
    // generic reason: no particular information about error
    | Unknown
    // source code file is not available
    | NoSourceCode
    // trying to find declaration of ProvidedType without TypeProviderDefinitionLocationAttribute
    | ProvidedType of string
    // trying to find declaration of ProvidedMember without TypeProviderDefinitionLocationAttribute
    | ProvidedMember of string

type FSharpFindDeclResult = 
    /// declaration not found + reason
    | DeclNotFound of FSharpFindDeclFailureReason
    /// found declaration
    | DeclFound of range


/// This type is used to describe what was found during the name resolution.
/// (Depending on the kind of the items, we may stop processing or continue to find better items)
[<RequireQualifiedAccess>]
[<NoEquality; NoComparison>]
type internal NameResResult = 
    | Members of (Item list * DisplayEnv * range)
    | Cancel of DisplayEnv * range
    | Empty
    | TypecheckStaleAndTextChanged
    

[<RequireQualifiedAccess>]
type ResolveOverloads = 
|   Yes
|   No

[<RequireQualifiedAccess>]
type GetPreciseCompletionListFromExprTypingsResult =
    | NoneBecauseTypecheckIsStaleAndTextChanged
    | NoneBecauseThereWereTypeErrors
    | None
    | Some of (Item list * DisplayEnv * range)

type Names = string list 

[<Sealed>]
type FSharpSymbolUse(g:TcGlobals, denv: DisplayEnv, symbol:FSharpSymbol, itemOcc, range: range) = 
    member __.Symbol  = symbol
    member __.DisplayContext  = FSharpDisplayContext(fun _ -> denv)
    member x.IsDefinition = x.IsFromDefinition
    member __.IsFromDefinition = (match itemOcc with ItemOccurence.Binding -> true | _ -> false)
    member __.IsFromPattern = (match itemOcc with ItemOccurence.Pattern -> true | _ -> false)
    member __.IsFromType = (match itemOcc with ItemOccurence.UseInType -> true | _ -> false)
    member __.IsFromAttribute = (match itemOcc with ItemOccurence.UseInAttribute -> true | _ -> false)
    member __.IsFromDispatchSlotImplementation = (match itemOcc with ItemOccurence.Implemented -> true | _ -> false)
    member __.IsFromComputationExpression = 
        match symbol.Item, itemOcc with 
        // 'seq' in 'seq { ... }' gets colored as keywords
        | (Item.Value vref), ItemOccurence.Use when valRefEq g g.seq_vref vref ->  true
        // custom builders, custom operations get colored as keywords
        | (Item.CustomBuilder _ | Item.CustomOperation _), ItemOccurence.Use ->  true
        | _ -> false

    member __.FileName = range.FileName
    member __.Range = Range.toZ range
    member __.RangeAlternate = range

[<RequireQualifiedAccess>]
type SemanticClassificationType =
    | ReferenceType
    | ValueType
    | UnionCase
    | Function
    | Property
    | MutableVar
    | Module
    | Printf
    | ComputationExpression
    | IntrinsicType
    | Enumeration
    | Interface

// A scope represents everything we get back from the typecheck of a file.
// It acts like an in-memory database about the file.
// It is effectively immutable and not updated: when we re-typecheck we just drop the previous
// scope object on the floor and make a new one.
[<Sealed>]
type TypeCheckInfo
          (// Information corresponding to miscellaneous command-line options (--define, etc).
           _sTcConfig: TcConfig,
           g: TcGlobals,
           // The signature of the assembly being checked, up to and including the current file
           ccuSig: ModuleOrNamespaceType,
           thisCcu: CcuThunk,
           tcImports: TcImports,
           tcAccessRights: AccessorDomain,
           projectFileName: string ,
           mainInputFileName: string ,
           sResolutions: TcResolutions,
           sSymbolUses: TcSymbolUses,
           // This is a name resolution environment to use if no better match can be found.
           sFallback: NameResolutionEnv,
           loadClosure : LoadClosure option,
           reactorOps : IReactorOperations,
           checkAlive : (unit -> bool),
           textSnapshotInfo:obj option) = 

    let textSnapshotInfo = defaultArg textSnapshotInfo null
    let (|CNR|) (cnr:CapturedNameResolution) =
        (cnr.Pos, cnr.Item, cnr.ItemOccurence, cnr.DisplayEnv, cnr.NameResolutionEnv, cnr.AccessorDomain, cnr.Range)

    // These strings are potentially large and the editor may choose to hold them for a while.
    // Use this cache to fold together data tip text results that are the same. 
    // Is not keyed on 'Names' collection because this is invariant for the current position in 
    // this unchanged file. Keyed on lineStr though to prevent a change to the currently line
    // being available against a stale scope.
    let getToolTipTextCache = AgedLookup<CompilationThreadToken, int*int*string, FSharpToolTipText<Layout>>(getToolTipTextSize,areSame=(fun (x,y) -> x = y))
    
    let amap = tcImports.GetImportMap()
    let infoReader = new InfoReader(g,amap)
    let ncenv = new NameResolver(g,amap,infoReader,NameResolution.FakeInstantiationGenerator)
    
    /// Find the most precise naming environment for the given line and column
    let GetBestEnvForPos cursorPos  =
        
        let mutable bestSoFar = None

        // Find the most deeply nested enclosing scope that contains given position
        sResolutions.CapturedEnvs |> ResizeArray.iter (fun (possm,env,ad) -> 
            if rangeContainsPos possm cursorPos then
                match bestSoFar with 
                | Some (bestm,_,_) -> 
                    if rangeContainsRange bestm possm then 
                      bestSoFar <- Some (possm,env,ad)
                | None -> 
                    bestSoFar <- Some (possm,env,ad))

        let mostDeeplyNestedEnclosingScope = bestSoFar 
        
        // Look for better subtrees on the r.h.s. of the subtree to the left of where we are 
        // Should really go all the way down the r.h.s. of the subtree to the left of where we are 
        // This is all needed when the index is floating free in the area just after the environment we really want to capture 
        // We guarantee to only refine to a more nested environment.  It may not be strictly  
        // the right environment, but will alwauys be at least as rich 

        let bestAlmostIncludedSoFar = ref None 

        sResolutions.CapturedEnvs |> ResizeArray.iter (fun (possm,env,ad) -> 
            // take only ranges that strictly do not include cursorPos (all ranges that touch cursorPos were processed during 'Strict Inclusion' part)
            if rangeBeforePos possm cursorPos && not (posEq possm.End cursorPos) then 
                let contained = 
                    match mostDeeplyNestedEnclosingScope with 
                    | Some (bestm,_,_) -> rangeContainsRange bestm possm 
                    | None -> true 
                
                if contained then 
                    match  !bestAlmostIncludedSoFar with 
                    | Some (rightm:range,_,_) -> 
                        if posGt possm.End rightm.End || 
                          (posEq possm.End rightm.End && posGt possm.Start rightm.Start) then
                            bestAlmostIncludedSoFar := Some (possm,env,ad)
                    | _ -> bestAlmostIncludedSoFar := Some (possm,env,ad))
        
        let resEnv = 
            match !bestAlmostIncludedSoFar with 
            | Some (_m,env,ad) -> 
                env,ad
            | None -> 
                match mostDeeplyNestedEnclosingScope with 
                | Some (_m,env,ad) -> 
                    env,ad
                | None -> 
                    (sFallback,AccessibleFromSomeFSharpCode)
        let pm = mkRange mainInputFileName cursorPos cursorPos 

        resEnv,pm

    /// The items that come back from ResolveCompletionsInType are a bit
    /// noisy. Filter a few things out.
    ///
    /// e.g. prefer types to constructors for FSharpToolTipText 
    let FilterItemsForCtors filterCtors items = 
        let items = items |> List.filter (function (Item.CtorGroup _) when filterCtors = ResolveTypeNamesToTypeRefs -> false | _ -> true) 
        items
        
    
    // Filter items to show only valid & return Some if there are any
    let ReturnItemsOfType items g denv (m:range) filterCtors hasTextChangedSinceLastTypecheck f =
        let items = 
            items 
            |> RemoveDuplicateItems g
            |> RemoveExplicitlySuppressed g
            |> FilterItemsForCtors filterCtors

        if not (isNil items) then
            if hasTextChangedSinceLastTypecheck(textSnapshotInfo, m) then
                NameResResult.TypecheckStaleAndTextChanged // typecheck is stale, wait for second-chance IntelliSense to bring up right result
            else
                f(items, denv, m) 
        else NameResResult.Empty

    let GetCapturedNameResolutions endOfNamesPos resolveOverloads =

        let quals = 
            match resolveOverloads with 
            | ResolveOverloads.Yes -> sResolutions.CapturedNameResolutions 
            | ResolveOverloads.No -> sResolutions.CapturedMethodGroupResolutions

        let quals = quals |> ResizeArray.filter (fun cnr ->  posEq cnr.Pos endOfNamesPos)
        
        quals

    /// Looks at the exact name resolutions that occurred during type checking
    /// If 'membersByResidue' is specified, we look for members of the item obtained 
    /// from the name resolution and filter them by the specified residue (?)
    let GetPreciseItemsFromNameResolution(line, colAtEndOfNames, membersByResidue, filterCtors, resolveOverloads, hasTextChangedSinceLastTypecheck) = 
        let endOfNamesPos = mkPos line colAtEndOfNames

        // Logic below expects the list to be in reverse order of resolution
        let items = GetCapturedNameResolutions endOfNamesPos resolveOverloads |> ResizeArray.toList |> List.rev

        match items, membersByResidue with 
        
        // If we're looking for members using a residue, we'd expect only
        // a single item (pick the first one) and we need the residue (which may be "")
        | CNR(_,Item.Types(_,(typ::_)),_,denv,nenv,ad,m)::_, Some _ -> 
            let items = ResolveCompletionsInType ncenv nenv (ResolveCompletionTargets.All(ConstraintSolver.IsApplicableMethApprox g amap m)) m ad true typ 
            ReturnItemsOfType items g denv m filterCtors hasTextChangedSinceLastTypecheck NameResResult.Members 
        
        // Value reference from the name resolution. Primarily to disallow "let x.$ = 1"
        // In most of the cases, value references can be obtained from expression typings or from environment,
        // so we wouldn't have to handle values here. However, if we have something like:
        //   let varA = "string"
        //   let varA = if b then 0 else varA.
        // then the expression typings get confused (thinking 'varA:int'), so we use name resolution even for usual values.
        
        | CNR(_, Item.Value(vref), occurence, denv, nenv, ad, m)::_, Some _ ->
            if (occurence = ItemOccurence.Binding || occurence = ItemOccurence.Pattern) then 
              // Return empty list to stop further lookup - for value declarations
              NameResResult.Cancel(denv, m)
            else 
              // If we have any valid items for the value, then return completions for its type now.
              // Adjust the type in case this is the 'this' pointer stored in a reference cell.
              let ty = StripSelfRefCell(g, vref.BaseOrThisInfo, vref.TauType) 
              // patch accessibility domain to remove protected members if accessing NormalVal
              let ad = 
                match vref.BaseOrThisInfo, ad with
                | ValBaseOrThisInfo.NormalVal, AccessibleFrom(paths, Some tcref) ->
                    let tcref = generalizedTyconRef tcref
                    // check that type of value is the same or subtype of tcref
                    // yes - allow access to protected members
                    // no - strip ability to access protected members
                    if Microsoft.FSharp.Compiler.TypeRelations.TypeFeasiblySubsumesType 0 g amap m tcref Microsoft.FSharp.Compiler.TypeRelations.CanCoerce ty then
                        ad
                    else
                        AccessibleFrom(paths, None)
                | _ -> ad

              let items = ResolveCompletionsInType ncenv nenv (ResolveCompletionTargets.All(ConstraintSolver.IsApplicableMethApprox g amap m)) m ad false ty
              ReturnItemsOfType items g denv m filterCtors hasTextChangedSinceLastTypecheck NameResResult.Members
        
        // No residue, so the items are the full resolution of the name
        | CNR(_,_,_,denv,_,_,m) :: _, None -> 
            let items = items |> List.map (fun (CNR(_,item,_,_,_,_,_)) -> item) 
                              // "into" is special magic syntax, not an identifier or a library call.  It is part of capturedNameResolutions as an 
                              // implementation detail of syntax coloring, but we should not report name resolution results for it, to prevent spurious QuickInfo.
                              |> List.filter (function Item.CustomOperation(CustomOperations.Into,_,_) -> false | _ -> true) 
            ReturnItemsOfType items g denv m filterCtors hasTextChangedSinceLastTypecheck NameResResult.Members
        | _ , _ -> NameResResult.Empty
    
    let CollectParameters (methods: MethInfo list) amap m: Item list = 
        methods
        |> List.collect (fun meth ->
            match meth.GetParamDatas(amap, m, meth.FormalMethodInst) with
            | x::_ -> x |> List.choose(fun (ParamData(_isParamArray, _isOut, _optArgInfo, _callerInfoInfo, name, _, ty)) -> 
                match name with
                | Some n -> Some (Item.ArgName(n, ty, Some (ArgumentContainer.Method meth)))
                | None -> None
                )
            | _ -> []
        )

    let GetNamedParametersAndSettableFields endOfExprPos hasTextChangedSinceLastTypecheck =
        let cnrs = GetCapturedNameResolutions endOfExprPos ResolveOverloads.No |> ResizeArray.toList |> List.rev
        let result =
            match cnrs with
            | CNR(_, Item.CtorGroup(_, ((ctor::_) as ctors)), _, denv, nenv, ad, m)::_ ->
                let props = ResolveCompletionsInType ncenv nenv ResolveCompletionTargets.SettablePropertiesAndFields m ad false ctor.EnclosingType
                let parameters = CollectParameters ctors amap m
                Some (denv, m, props @ parameters)
            | CNR(_, Item.MethodGroup(_, methods, _), _, denv, nenv, ad, m)::_ ->
                let props = 
                    methods
                    |> List.collect (fun meth ->
                        let retTy = meth.GetFSharpReturnTy(amap, m, meth.FormalMethodInst)
                        ResolveCompletionsInType ncenv nenv ResolveCompletionTargets.SettablePropertiesAndFields m ad false retTy
                    )
                let parameters = CollectParameters methods amap m
                Some (denv, m, props @ parameters)
            | _ -> 
                None
        match result with
        | None -> 
            NameResResult.Empty
        | Some (denv, m, result) -> 
            ReturnItemsOfType result g denv m TypeNameResolutionFlag.ResolveTypeNamesToTypeRefs hasTextChangedSinceLastTypecheck NameResResult.Members
    
    /// finds captured typing for the given position
    let GetExprTypingForPosition(endOfExprPos) = 
        let quals = 
            sResolutions.CapturedExpressionTypings 
            |> Seq.filter (fun (pos,typ,denv,_,_,_) -> 
                    // We only want expression types that end at the particular position in the file we are looking at.
                    let isLocationWeCareAbout = posEq pos endOfExprPos
                    // Get rid of function types.  True, given a 2-arg curried function "f x y", it is legal to do "(f x).GetType()",
                    // but you almost never want to do this in practice, and we choose not to offer up any intellisense for 
                    // F# function types.
                    let isFunction = isFunTy denv.g typ
                    isLocationWeCareAbout && not isFunction)
            |> Seq.toArray

        let thereWereSomeQuals = not (Array.isEmpty quals)
        // filter out errors

        let quals = quals 
                    |> Array.filter (fun (_,typ,denv,_,_,_) -> not (isTyparTy denv.g typ && (destTyparTy denv.g typ).IsFromError))
        thereWereSomeQuals, quals
    
    /// obtains captured typing for the given position
    /// if type of captured typing is record - returns list of record fields
    let GetRecdFieldsForExpr(r : range) = 
        let _, quals = GetExprTypingForPosition(r.End)
        let bestQual = 
            match quals with
            | [||] -> None
            | quals ->  
                quals |> Array.tryFind (fun (_,_,_,_,_,rq) -> 
                                            ignore(r)  // for breakpoint
                                            posEq r.Start rq.Start)
        match bestQual with
        | Some (_,typ,denv,_nenv,ad,m) when isRecdTy denv.g typ ->
            let items = NameResolution.ResolveRecordOrClassFieldsOfType ncenv m ad typ false
            Some (items, denv, m)
        | _ -> None

    /// Looks at the exact expression types at the position to the left of the 
    /// residue then the source when it was typechecked.
    let GetPreciseCompletionListFromExprTypings(parseResults:FSharpParseFileResults, endOfExprPos, filterCtors, hasTextChangedSinceLastTypecheck: (obj * range -> bool)) = 
        
        let thereWereSomeQuals, quals = GetExprTypingForPosition(endOfExprPos)

        match quals with
        | [| |] -> 
            if thereWereSomeQuals then
                GetPreciseCompletionListFromExprTypingsResult.NoneBecauseThereWereTypeErrors 
            else
                GetPreciseCompletionListFromExprTypingsResult.None
        | _ ->
            let bestQual, textChanged = 
                match parseResults.ParseTree with
                | Some(input) -> 
                    match UntypedParseImpl.GetRangeOfExprLeftOfDot(endOfExprPos,Some(input)) with   // TODO we say "colAtEndOfNames" everywhere, but that's not really a good name ("foo  .  $" hit Ctrl-Space at $)
                    | Some( exprRange) ->
                        if hasTextChangedSinceLastTypecheck(textSnapshotInfo, exprRange) then
                            None, true // typecheck is stale, wait for second-chance IntelliSense to bring up right result
                        else
                            // See bug 130733.  We have an up-to-date sync parse, and know the exact range of the prior expression.
                            // The quals all already have the same ending position, so find one with a matching starting position, if it exists.
                            // If not, then the stale typecheck info does not have a capturedExpressionTyping for this exact expression, and the
                            // user can wait for typechecking to catch up and second-chance intellisense to give the right result.
                            let qual = 
                                quals |> Array.tryFind (fun (_,_,_,_,_,r) -> 
                                                            ignore(r)  // for breakpoint
                                                            posEq exprRange.Start r.Start)
                            qual, false
                    | None -> 
                        // TODO In theory I think we should never get to this code path; it would be nice to add an assert.
                        // In practice, we do get here in some weird cases like "2.0 .. 3.0" and hitting Ctrl-Space in between the two dots of the range operator.
                        // I wasn't able to track down what was happening in those weird cases, not worth worrying about, it doesn't manifest as a product bug or anything.
                        None, false
                | _ -> None, false

            match bestQual with
            | Some bestQual ->
                let (_,typ,denv,nenv,ad,m) = bestQual 
                let items = ResolveCompletionsInType ncenv nenv (ResolveCompletionTargets.All(ConstraintSolver.IsApplicableMethApprox g amap m)) m ad false typ 
                let items = items |> RemoveDuplicateItems g
                let items = items |> RemoveExplicitlySuppressed g
                let items = items |> FilterItemsForCtors filterCtors 
                GetPreciseCompletionListFromExprTypingsResult.Some(items,denv,m)
            | None -> 
                if textChanged then GetPreciseCompletionListFromExprTypingsResult.NoneBecauseTypecheckIsStaleAndTextChanged
                else GetPreciseCompletionListFromExprTypingsResult.None

    /// Find items in the best naming environment.
    let GetEnvironmentLookupResolutions(cursorPos, plid, filterCtors, showObsolete) = 
        let (nenv,ad),m = GetBestEnvForPos cursorPos
        let items = NameResolution.ResolvePartialLongIdent ncenv nenv (ConstraintSolver.IsApplicableMethApprox g amap m) m ad plid showObsolete
        let items = items |> RemoveDuplicateItems g 
        let items = items |> RemoveExplicitlySuppressed g
        let items = items |> FilterItemsForCtors filterCtors 
         
        items, nenv.DisplayEnv, m 

    /// Find record fields in the best naming environment.
    let GetClassOrRecordFieldsEnvironmentLookupResolutions(cursorPos, plid, (_residue : string option)) = 
        let (nenv, ad),m = GetBestEnvForPos cursorPos
        let items = NameResolution.ResolvePartialLongIdentToClassOrRecdFields ncenv nenv m ad plid false
        let items = items |> RemoveDuplicateItems g 
        let items = items |> RemoveExplicitlySuppressed g
        items, nenv.DisplayEnv,m 

    /// Resolve a location and/or text to items.
    //   Three techniques are used
    //        - look for an exact known name resolution from type checking
    //        - use the known type of an expression, e.g. (expr).Name, to generate an item list  
    //        - lookup an entire name in the name resolution environment, e.g. A.B.Name, to generate an item list
    //
    // The overall aim is to resolve as accurately as possible based on what we know from type inference
    
    let GetBaseClassCandidates = function
        | Item.ModuleOrNamespaces _ -> true
        | Item.Types(_, ty::_) when (isClassTy g ty) && not (isSealedTy g ty) -> true
        | _ -> false   

    let GetInterfaceCandidates = function
        | Item.ModuleOrNamespaces _ -> true
        | Item.Types(_, ty::_) when (isInterfaceTy g ty) -> true
        | _ -> false   

    // Return only items with the specified name
    let FilterDeclItemsByResidue residue (items: Item list) = 
        items |> List.filter (fun item -> 
            let n1 =  item.DisplayName 
            match item with
            | Item.Types _ | Item.CtorGroup _ -> residue + "Attribute" = n1 || residue = n1
            | _ -> residue = n1 )
            
    /// Post-filter items to make sure they have precisely the right name
    /// This also checks that there are some remaining results 
    /// exactMatchResidueOpt = Some _ -- means that we are looking for exact matches
    let FilterRelevantItemsBy (exactMatchResidueOpt : _ option) check (items, denv, m) =
            
        // can throw if type is in located in non-resolved CCU: i.e. bigint if reference to System.Numerics is absent
        let safeCheck item = try check item with _ -> false
                                                
        // Are we looking for items with precisely the given name?
        if not (isNil items) && exactMatchResidueOpt.IsSome then
            let items = items |> FilterDeclItemsByResidue exactMatchResidueOpt.Value |> List.filter safeCheck 
            if not (isNil items) then Some(items, denv, m) else None        
        else 
            // When (items = []) we must returns Some([],..) and not None
            // because this value is used if we want to stop further processing (e.g. let x.$ = ...)
            let items = items |> List.filter safeCheck
            Some(items, denv, m) 

    /// Post-filter items to make sure they have precisely the right name
    /// This also checks that there are some remaining results 
    let (|FilterRelevantItems|_|) exactMatchResidueOpt orig =
        FilterRelevantItemsBy exactMatchResidueOpt (fun _ -> true) orig

    
    /// Find the first non-whitespace postion in a line prior to the given character
    let FindFirstNonWhitespacePosition (lineStr: string) i = 
        if i >= lineStr.Length then None
        else
        let mutable p = i
        while p >= 0 && System.Char.IsWhiteSpace(lineStr.[p]) do
            p <- p - 1
        if p >= 0 then Some p else None
        

    let GetDeclaredItems (parseResultsOpt: FSharpParseFileResults option, lineStr: string, origLongIdentOpt, colAtEndOfNamesAndResidue, residueOpt, line, loc, filterCtors,resolveOverloads, hasTextChangedSinceLastTypecheck, isInRangeOperator) =
 
            // Are the last two chars (except whitespaces) = ".."
            let isLikeRangeOp = 
                match FindFirstNonWhitespacePosition lineStr (colAtEndOfNamesAndResidue - 1) with
                | Some x when x >= 1 && lineStr.[x] = '.' && lineStr.[x - 1] = '.' -> true
                | _ -> false

            // if last two chars are .. and we are not in range operator context - no completion
            if isLikeRangeOp && not isInRangeOperator then None else
                                    
            // Try to use the exact results of name resolution during type checking to generate the results
            // This is based on position (i.e. colAtEndOfNamesAndResidue). This is not used if a residueOpt is given.
            let nameResItems = 
                match residueOpt with 
                | None -> GetPreciseItemsFromNameResolution(line, colAtEndOfNamesAndResidue, None, filterCtors,resolveOverloads, hasTextChangedSinceLastTypecheck)
                | Some residue ->
                    // deals with cases when we have spaces between dot and\or identifier, like A  . $
                    // if this is our case - then wen need to locate end position of the name skipping whitespaces
                    // this allows us to handle cases like: let x . $ = 1 

                    // colAtEndOfNamesAndResidue is 1-based so at first we need to convert it to 0-based 
                    //
                    // TODO: this code would be a lot simpler if we just passed in colAtEndOfNames in 
                    // the first place. colAtEndOfNamesAndResidue serves no purpose. The cracking below is
                    // inaccurate and incomplete in any case since it only works on a single line.
                    match FindFirstNonWhitespacePosition lineStr (colAtEndOfNamesAndResidue - 1) with
                    | Some p when lineStr.[p] = '.' ->
                        match FindFirstNonWhitespacePosition lineStr (p - 1) with
                        | Some colAtEndOfNames ->                 
                            let colAtEndOfNames = colAtEndOfNames + 1 // convert 0-based to 1-based
                            GetPreciseItemsFromNameResolution(line, colAtEndOfNames, Some(residue), filterCtors,resolveOverloads, hasTextChangedSinceLastTypecheck)
                        | None -> NameResResult.Empty
                    | _ -> NameResResult.Empty        
        
            // Normalize to form A.B.C.D where D is the residue. It may be empty for "A.B.C."
            // residueOpt = Some when we are looking for the exact match
            let plid, exactMatchResidueOpt = 
                match origLongIdentOpt, residueOpt with
                | None, _ -> [], None
                | Some(origLongIdent), Some _ -> origLongIdent, None
                | Some(origLongIdent), None ->
                    System.Diagnostics.Debug.Assert(not (isNil origLongIdent), "origLongIdent is empty")
                    // note: as above, this happens when we are called for "precise" resolution - (F1 keyword, data tip etc..)
                    let plid, residue = List.frontAndBack origLongIdent
                    plid, Some residue
                
            match nameResItems with            
            | NameResResult.TypecheckStaleAndTextChanged -> None // second-chance intellisense will try again
            | NameResResult.Cancel(denv,m) -> Some([], denv, m)
            | NameResResult.Members(FilterRelevantItems exactMatchResidueOpt items) -> 
                // lookup based on name resolution results successful
                Some items
            | _ ->
        
            match origLongIdentOpt with
            | None -> None
            | Some _ -> 

                // Try to use the type of the expression on the left to help generate a completion list
                let qualItems, thereIsADotInvolved = 
                    match parseResultsOpt with
                    | None -> 
                        // Note, you will get here if the 'reason' is not CompleteWord/MemberSelect/DisplayMemberList, as those are currently the 
                        // only reasons we do a sync parse to have the most precise and likely-to-be-correct-and-up-to-date info.  So for example,
                        // if you do QuickInfo hovering over A in "f(x).A()", you will only get a tip if typechecking has a name-resolution recorded
                        // for A, not if merely we know the capturedExpressionTyping of f(x) and you very recently typed ".A()" - in that case, 
                        // you won't won't get a tip until the typechecking catches back up.
                        GetPreciseCompletionListFromExprTypingsResult.None, false
                    | Some parseResults -> 

                    match UntypedParseImpl.TryFindExpressionASTLeftOfDotLeftOfCursor(mkPos line colAtEndOfNamesAndResidue,parseResults.ParseTree) with
                    | Some(pos,_) ->
                        GetPreciseCompletionListFromExprTypings(parseResults, pos, filterCtors, hasTextChangedSinceLastTypecheck), true
                    | None -> 
                        // Can get here in a case like: if "f xxx yyy" is legal, and we do "f xxx y"
                        // We have no interest in expression typings, those are only useful for dot-completion.  We want to fallback
                        // to "Use an environment lookup as the last resort" below
                        GetPreciseCompletionListFromExprTypingsResult.None, false

                match qualItems,thereIsADotInvolved with            
                | GetPreciseCompletionListFromExprTypingsResult.Some(FilterRelevantItems exactMatchResidueOpt items), _
                        // Initially we only use the expression typings when looking up, e.g. (expr).Nam or (expr).Name1.Nam
                        // These come through as an empty plid and residue "". Otherwise we try an environment lookup
                        // and then return to the qualItems. This is because the expression typings are a little inaccurate, primarily because
                        // it appears we're getting some typings recorded for non-atomic expressions like "f x"
                        when (match plid with [] -> true | _ -> false)  -> 
                    // lookup based on expression typings successful
                    Some items
                | GetPreciseCompletionListFromExprTypingsResult.NoneBecauseThereWereTypeErrors, _ ->
                    // There was an error, e.g. we have "<expr>." and there is an error determining the type of <expr>  
                    // In this case, we don't want any of the fallback logic, rather, we want to produce zero results.
                    None
                | GetPreciseCompletionListFromExprTypingsResult.NoneBecauseTypecheckIsStaleAndTextChanged, _ ->         
                    // we want to report no result and let second-chance intellisense kick in
                    None
                | _, true when (match plid with [] -> true | _ -> false)  -> 
                    // If the user just pressed '.' after an _expression_ (not a plid), it is never right to show environment-lookup top-level completions.
                    // The user might by typing quickly, and the LS didn't have an expression type right before the dot yet.
                    // Second-chance intellisense will bring up the correct list in a moment.
                    None
                | _ ->         

                // Use an environment lookup as the last resort
                let envItems =  GetEnvironmentLookupResolutions(mkPos line loc, plid, filterCtors, residueOpt.IsSome)
                match nameResItems, envItems, qualItems with            
            
                // First, use unfiltered name resolution items, if they're not empty
                | NameResResult.Members(items, denv, m), _, _ when not (isNil items) -> 
                    // lookup based on name resolution results successful
                    Some(items, denv, m)                
            
                // If we have nonempty items from environment that were resolved from a type, then use them... 
                // (that's better than the next case - here we'd return 'int' as a type)
                | _, FilterRelevantItems exactMatchResidueOpt (items, denv, m), _ when not (isNil items) ->
                    // lookup based on name and environment successful
                    Some(items, denv, m)

                // Try again with the qualItems
                | _, _, GetPreciseCompletionListFromExprTypingsResult.Some(FilterRelevantItems exactMatchResidueOpt items) ->
                    Some(items)
                
                | _ -> None


    /// Get the auto-complete items at a particular location.
    let GetDeclItemsForNamesAtPosition(ctok: CompilationThreadToken, parseResultsOpt: FSharpParseFileResults option, origLongIdentOpt: string list option, residueOpt:string option, line:int, lineStr:string, colAtEndOfNamesAndResidue, filterCtors, resolveOverloads, hasTextChangedSinceLastTypecheck: (obj * range -> bool)) = 
        RequireCompilationThread ctok // the operations in this method need the reactor thread

        let loc = 
            match colAtEndOfNamesAndResidue with
            | pastEndOfLine when pastEndOfLine >= lineStr.Length -> lineStr.Length
            | atDot when lineStr.[atDot] = '.' -> atDot + 1
            | atStart when atStart = 0 -> 0
            | otherwise -> otherwise - 1

        // Look for a "special" completion context
        match UntypedParseImpl.TryGetCompletionContext(mkPos line colAtEndOfNamesAndResidue, parseResultsOpt, lineStr) with

        // Invalid completion locations
        | Some CompletionContext.Invalid -> None

        // Completion at 'inherit C(...)"
        | Some (CompletionContext.Inherit(InheritanceContext.Class, (plid, _))) ->
            GetEnvironmentLookupResolutions(mkPos line loc, plid, filterCtors, false) 
            |> FilterRelevantItemsBy None GetBaseClassCandidates

        // Completion at 'interface ..."
        | Some (CompletionContext.Inherit(InheritanceContext.Interface, (plid, _))) ->
            GetEnvironmentLookupResolutions(mkPos line loc, plid, filterCtors, false) 
            |> FilterRelevantItemsBy None GetInterfaceCandidates

        // Completion at 'implement ..."
        | Some (CompletionContext.Inherit(InheritanceContext.Unknown, (plid, _))) ->
            GetEnvironmentLookupResolutions(mkPos line loc, plid, filterCtors, false) 
            |> FilterRelevantItemsBy None (fun t -> GetBaseClassCandidates t || GetInterfaceCandidates t)

        // Completion at ' { XXX = ... } "
        | Some(CompletionContext.RecordField(RecordContext.New(plid, residue))) ->
            Some(GetClassOrRecordFieldsEnvironmentLookupResolutions(mkPos line loc, plid, residue))

        // Completion at ' { XXX = ... with ... } "
        | Some(CompletionContext.RecordField(RecordContext.CopyOnUpdate(r, (plid, residue)))) -> 
            match GetRecdFieldsForExpr(r) with
            | None -> 
                GetClassOrRecordFieldsEnvironmentLookupResolutions(mkPos line loc, plid, residue)
                |> Some
            | x -> x

        // Completion at ' { XXX = ... with ... } "
        | Some(CompletionContext.RecordField(RecordContext.Constructor(typeName))) ->
            Some(GetClassOrRecordFieldsEnvironmentLookupResolutions(mkPos line loc, [typeName], None))

        // Completion at ' SomeMethod( ... ) ' with named arguments 
        | Some(CompletionContext.ParameterList (endPos, fields)) ->
            let results = GetNamedParametersAndSettableFields endPos hasTextChangedSinceLastTypecheck

            let declaredItems = GetDeclaredItems (parseResultsOpt, lineStr, origLongIdentOpt, colAtEndOfNamesAndResidue, residueOpt, line, loc, filterCtors, resolveOverloads, hasTextChangedSinceLastTypecheck, false)

            match results with
            | NameResResult.Members(items, denv, m) -> 
                let filtered = 
                    items 
                    |> RemoveDuplicateItems g
                    |> RemoveExplicitlySuppressed g
                    |> List.filter (fun m -> not (fields.Contains m.DisplayName))
                match declaredItems with
                | None -> Some (items, denv, m)
                | Some (declItems, declaredDisplayEnv, declaredRange) -> Some (filtered @ declItems, declaredDisplayEnv, declaredRange)
            | _ -> declaredItems

        | Some(CompletionContext.AttributeApplication) ->
            GetDeclaredItems (parseResultsOpt, lineStr, origLongIdentOpt, colAtEndOfNamesAndResidue, residueOpt, line, loc, filterCtors, resolveOverloads, hasTextChangedSinceLastTypecheck, false)
            |> Option.map (fun (items, denv, r) -> 
                items 
                |> List.filter (function
                    | Item.Types _
                    | Item.ModuleOrNamespaces _ -> true
                    | _ -> false), denv, r)

        // Other completions
        | cc ->
            let isInRangeOperator = (match cc with Some (CompletionContext.RangeOperator) -> true | _ -> false)
            GetDeclaredItems (parseResultsOpt, lineStr, origLongIdentOpt, colAtEndOfNamesAndResidue, residueOpt, line, loc, filterCtors,resolveOverloads, hasTextChangedSinceLastTypecheck, isInRangeOperator)

    /// Return 'false' if this is not a completion item valid in an interface file.
    let IsValidSignatureFileItem item =
        match item with
        | Item.Types _ | Item.ModuleOrNamespaces _ -> true
        | _ -> false

    /// Check if we are at an "open" declaration
    let IsAtOpenDeclaration (parseResults, pos: pos) = 
        // visitor to see if we are in an "open" declaration in the parse tree
        let visitor = { new AstTraversal.AstVisitorBase<bool>() with
                            override this.VisitExpr(_path, _traverseSynExpr, defaultTraverse, expr) = None  // don't need to keep going, 'open' declarations never appear inside Exprs
                            override this.VisitModuleDecl(defaultTraverse, decl) =
                                match decl with
                                | SynModuleDecl.Open(_longIdent, m) -> 
                                    // in theory, this means we're "in an open"
                                    // in practice, because the parse tree/walkers do not handle attributes well yet, need extra check below to ensure not e.g. $here$
                                    //     open System
                                    //     [<Attr$
                                    //     let f() = ()
                                    // inside an attribute on the next item
                                    let pos = mkPos pos.Line (pos.Column - 1) // -1 because for e.g. "open System." the dot does not show up in the parse tree
                                    if rangeContainsPos m pos then  
                                        Some true
                                    else
                                        None
                                | _ -> defaultTraverse decl }
        match AstTraversal.Traverse(pos, parseResults, visitor) with
        | None -> false
        | Some res -> res

    /// If an AST is available, then determine if we are at a "special" position in the AST such as an "open".  If so restrict 
    /// or augment the autocompletes available at that point.
    let FilterAutoCompletesBasedOnParseContext (parseResultsOpt: FSharpParseFileResults option) (pos:pos) items = 
        match parseResultsOpt |> Option.bind (fun parseResults -> parseResults.ParseTree) with
        | None -> items
        | Some parseTree -> 
            if IsAtOpenDeclaration (parseTree, pos) then 
                items |> List.filter (function Item.ModuleOrNamespaces _ -> true | _ -> false)
            else 
                items


    static let keywordTypes = Lexhelp.Keywords.keywordTypes

    member x.IsRelativeNameResolvable(cursorPos: pos, plid: string list, item: Item) : bool =
    /// Determines if a long ident is resolvable at a specific point.
        ErrorScope.Protect
            Range.range0
            (fun () ->
                /// Find items in the best naming environment.
                let (nenv, ad), m = GetBestEnvForPos cursorPos
                NameResolution.IsItemResolvable ncenv nenv m ad plid item)
            (fun _ -> false)
        
        //let items = NameResolution.ResolvePartialLongIdent ncenv nenv (fun _ _ -> true) m ad plid true
        //items |> List.exists (ItemsAreEffectivelyEqual g item)

    /// Get the auto-complete items at a location
    member x.GetDeclarations (ctok, parseResultsOpt, line, lineStr, colAtEndOfNamesAndResidue, qualifyingNames, partialName, hasTextChangedSinceLastTypecheck) =
        let isInterfaceFile = SourceFileImpl.IsInterfaceFile mainInputFileName
        ErrorScope.Protect Range.range0 
            (fun () -> 
                match GetDeclItemsForNamesAtPosition(ctok, parseResultsOpt, Some qualifyingNames, Some partialName, line, lineStr, colAtEndOfNamesAndResidue, ResolveTypeNamesToCtors, ResolveOverloads.Yes, hasTextChangedSinceLastTypecheck) with
                | None -> FSharpDeclarationListInfo.Empty  
                | Some (items, denv, m) -> 
                    let items = items |> FilterAutoCompletesBasedOnParseContext parseResultsOpt (mkPos line colAtEndOfNamesAndResidue)
                    let items = if isInterfaceFile then items |> List.filter IsValidSignatureFileItem else items
                    FSharpDeclarationListInfo.Create(infoReader,m,denv,items,reactorOps,checkAlive))
            (fun msg -> FSharpDeclarationListInfo.Error msg)

    /// Get the symbols for auto-complete items at a location
    member x.GetDeclarationListSymbols (ctok, parseResultsOpt, line, lineStr, colAtEndOfNamesAndResidue, qualifyingNames, partialName, hasTextChangedSinceLastTypecheck) =
        let isInterfaceFile = SourceFileImpl.IsInterfaceFile mainInputFileName
        ErrorScope.Protect Range.range0 
            (fun () -> 
                match GetDeclItemsForNamesAtPosition(ctok, parseResultsOpt, Some qualifyingNames, Some partialName, line, lineStr, colAtEndOfNamesAndResidue, ResolveTypeNamesToCtors, ResolveOverloads.Yes, hasTextChangedSinceLastTypecheck) with
                | None -> List.Empty  
                | Some (items, _denv, _m) -> 
                    let items = items |> FilterAutoCompletesBasedOnParseContext parseResultsOpt (mkPos line colAtEndOfNamesAndResidue)
                    let items = if isInterfaceFile then items |> List.filter IsValidSignatureFileItem else items

                    //do filtering like Declarationset
                    let items = items |> RemoveExplicitlySuppressed g
                    
                    // Sort by name. For things with the same name, 
                    //     - show types with fewer generic parameters first
                    //     - show types before over other related items - they usually have very useful XmlDocs 
                    let items = 
                        items |> List.sortBy (fun d -> 
                            let n = 
                                match d with  
                                | Item.Types (_,(TType_app(tcref,_) :: _)) -> 1 + tcref.TyparsNoRange.Length
                                // Put delegate ctors after types, sorted by #typars. RemoveDuplicateItems will remove FakeInterfaceCtor and DelegateCtor if an earlier type is also reported with this name
                                | Item.FakeInterfaceCtor (TType_app(tcref,_)) 
                                | Item.DelegateCtor (TType_app(tcref,_)) -> 1000 + tcref.TyparsNoRange.Length
                                // Put type ctors after types, sorted by #typars. RemoveDuplicateItems will remove DefaultStructCtors if a type is also reported with this name
                                | Item.CtorGroup (_, (cinfo :: _)) -> 1000 + 10 * (tcrefOfAppTy g cinfo.EnclosingType).TyparsNoRange.Length 
                                | _ -> 0
                            (d.DisplayName,n))

                    // Remove all duplicates. We've put the types first, so this removes the DelegateCtor and DefaultStructCtor's.
                    let items = items |> RemoveDuplicateItems g

                    if verbose then dprintf "service.ml: mkDecls: %d found groups after filtering\n" (List.length items); 

                    // Group by display name
                    let items = items |> List.groupBy (fun d -> d.DisplayName) 

                    // Filter out operators (and list)
                    let items = 
                        // Check whether this item looks like an operator.
                        let isOpItem(nm,item) = 
                            match item with 
                            | [Item.Value _]
                            | [Item.MethodGroup(_,[_],_)] -> IsOperatorName nm
                            | [Item.UnionCase _] -> IsOperatorName nm
                            | _ -> false              

                        let isFSharpList nm = (nm = "[]") // list shows up as a Type and a UnionCase, only such entity with a symbolic name, but want to filter out of intellisense

                        items |> List.filter (fun (nm,items) -> not (isOpItem(nm,items)) && not(isFSharpList nm)) 


                    let items = 
                        // Filter out duplicate names
                        items |> List.map (fun (_nm,itemsWithSameName) -> 
                            match itemsWithSameName with
                            | [] -> failwith "Unexpected empty bag"
                            | items ->
                                items 
                                |> List.map (fun item -> let symbol = FSharpSymbol.Create(g, thisCcu, tcImports, item)
                                                         FSharpSymbolUse(g, _denv, symbol, ItemOccurence.Use, _m)))

                    //end filtering
                    items)
            (fun _msg -> [])
            
    /// Get the "reference resolution" tooltip for at a location
    member scope.GetReferenceResolutionStructuredToolTipText(ctok, line,col) = 

        RequireCompilationThread ctok // the operations in this method need the reactor thread but the reasons why are not yet grounded

        let pos = mkPos line col
        let isPosMatch(pos, ar:AssemblyReference) : bool = 
            let isRangeMatch = (Range.rangeContainsPos ar.Range pos) 
            let isNotSpecialRange = (ar.Range <> rangeStartup) && (ar.Range <> range0) && (ar.Range <> rangeCmdArgs)
            let isMatch = isRangeMatch && isNotSpecialRange
            isMatch      
        
        let dataTipOfReferences() = 
            let matches =
                match loadClosure with
                | None -> []
                | Some(loadClosure) -> 
                    loadClosure.References
                        |> List.map snd
                        |> List.concat 
                        |> List.filter(fun ar->isPosMatch(pos, ar.originalReference))

            match matches with 
            | resolved::_ // Take the first seen
            | [resolved] -> 
                let tip = wordL (TaggedTextOps.tagStringLiteral((resolved.prepareToolTip ()).TrimEnd([|'\n'|])))
                FSharpStructuredToolTipText.FSharpToolTipText [FSharpStructuredToolTipElement.Single(tip ,FSharpXmlDoc.None)]

            | [] -> FSharpStructuredToolTipText.FSharpToolTipText []
                                    
        ErrorScope.Protect Range.range0 
            dataTipOfReferences
            (fun err -> FSharpToolTipText [FSharpStructuredToolTipElement.CompositionError err])

    // GetToolTipText: return the "pop up" (or "Quick Info") text given a certain context.
    member x.GetStructuredToolTipText(ctok, line, lineStr, colAtEndOfNames, names) = 
        let Compute() = 
            ErrorScope.Protect Range.range0 
                (fun () -> 
                    match GetDeclItemsForNamesAtPosition(ctok, None,Some(names),None,line,lineStr,colAtEndOfNames,ResolveTypeNamesToCtors,ResolveOverloads.Yes,fun _ -> false) with
                    | None -> FSharpToolTipText []
                    | Some(items, denv, m) ->
                         FSharpToolTipText(items |> List.map (FormatStructuredDescriptionOfItem false infoReader m denv )))
                (fun err -> FSharpToolTipText [FSharpStructuredToolTipElement.CompositionError err])
               
        // See devdiv bug 646520 for rationale behind truncating and caching these quick infos (they can be big!)
        let key = line,colAtEndOfNames,lineStr
        match getToolTipTextCache.TryGet (ctok, key) with 
        | Some res -> res
        | None ->
             let res = Compute()
             getToolTipTextCache.Put(ctok, key,res)
             res

    // GetToolTipText: return the "pop up" (or "Quick Info") text given a certain context.
    member x.GetToolTipText ctok line lineStr colAtEndOfNames names = 
        x.GetStructuredToolTipText(ctok, line, lineStr, colAtEndOfNames, names)
        |> Tooltips.ToFSharpToolTipText

    member x.GetF1Keyword (ctok, line, lineStr, colAtEndOfNames, names) : string option =
       ErrorScope.Protect Range.range0
            (fun () ->
                match GetDeclItemsForNamesAtPosition(ctok, None, Some names, None, line, lineStr, colAtEndOfNames, ResolveTypeNamesToCtors, ResolveOverloads.No, fun _ -> false) with // F1 Keywords do not distiguish between overloads
                | None -> None
                | Some (items, _, _) ->
                    match items with
                    | [] -> None
                    | [item] ->
                        GetF1Keyword item                        
                    | _ ->
                        // handle new Type()
                        let allTypes, constr, typ =
                            List.fold 
                                (fun (allTypes,constr,typ) item ->
                                    match item, constr, typ with
                                    |   (Item.Types _) as t, _, None  -> allTypes, constr, Some t
                                    |   (Item.Types _), _, _ -> allTypes, constr, typ
                                    |   (Item.CtorGroup _), None, _ -> allTypes, Some item, typ
                                    |   _ -> false, None, None) 
                                (true,None,None) items
                        match allTypes, constr, typ with
                        |   true, Some (Item.CtorGroup(_, _) as item), _    
                                -> GetF1Keyword item                        
                        |   true, _, Some typ
                                -> GetF1Keyword typ
                        |   _ -> None
            )    
            (fun _ -> None)

    member scope.GetMethods (ctok, line, lineStr, colAtEndOfNames, namesOpt) =
        ErrorScope.Protect Range.range0 
            (fun () -> 
                match GetDeclItemsForNamesAtPosition(ctok, None,namesOpt,None,line,lineStr,colAtEndOfNames,ResolveTypeNamesToCtors,ResolveOverloads.No, fun _ -> false) with
                | None -> FSharpMethodGroup("",[| |])
                | Some (items, denv, m) -> FSharpMethodGroup.Create(infoReader,m,denv,items))
            (fun msg -> 
                FSharpMethodGroup(msg,[| |]))

    member scope.GetMethodsAsSymbols (ctok, line, lineStr, colAtEndOfNames, names) =
        match GetDeclItemsForNamesAtPosition (ctok, None,Some(names), None, line, lineStr, colAtEndOfNames, ResolveTypeNamesToCtors, ResolveOverloads.No, fun _ -> false) with
        | None | Some ([], _, _) -> None
        | Some (items, denv, m) ->
            let allItems =
                items
                |> List.collect (fun item ->
                    match item with 
                    | Item.MethodGroup(nm,minfos,orig) -> minfos |> List.map (fun minfo -> Item.MethodGroup(nm,[minfo],orig))  
                    | Item.CtorGroup(nm,cinfos) -> cinfos |> List.map (fun minfo -> Item.CtorGroup(nm,[minfo])) 
                    | Item.FakeInterfaceCtor _
                    | Item.DelegateCtor _ -> [item]
                    | Item.NewDef _ 
                    | Item.ILField _ -> []
                    | Item.Event _ -> []
                    | Item.RecdField(rfinfo) -> if isFunction g rfinfo.FieldType then [item] else []
                    | Item.Value v -> if isFunction g v.Type then [item] else []
                    | Item.UnionCase(ucr,_) -> if not ucr.UnionCase.IsNullary then [item] else []
                    | Item.ExnCase(ecr) -> if isNil (recdFieldsOfExnDefRef ecr) then [] else [item]
                    | Item.Property(_,pinfos) -> 
                        let pinfo = List.head pinfos 
                        if pinfo.IsIndexer then [item] else []
#if EXTENSIONTYPING
                    | Params.ItemIsWithStaticArguments m g _ -> [item] // we pretend that provided-types-with-static-args are method-like in order to get ParamInfo for them
#endif
                    | Item.CustomOperation(_name, _helpText, _minfo) -> [item]
                    | Item.TypeVar _ -> []
                    | Item.CustomBuilder _ -> []
                    | _ -> [] )

            let symbols = allItems |> List.map (fun item -> FSharpSymbol.Create(g, thisCcu, tcImports, item))
            Some (symbols, denv, m)

    member scope.GetDeclarationLocation (ctok, line, lineStr, colAtEndOfNames, names, preferFlag) =
          match GetDeclItemsForNamesAtPosition (ctok, None,Some(names), None, line, lineStr, colAtEndOfNames, ResolveTypeNamesToCtors,ResolveOverloads.Yes, fun _ -> false) with
          | None
          | Some ([], _, _) -> FSharpFindDeclResult.DeclNotFound FSharpFindDeclFailureReason.Unknown
          | Some (item :: _ , _, _) -> 

              // For IL-based entities, switch to a different item. This is because
              // rangeOfItem, ccuOfItem don't work on IL methods or fields.
              //
              // Later comment: to be honest, they aren't going to work on these new items either.
              // This is probably old code from when we supported 'go to definition' generating IL metadata.
              let item =
                  match item with
                  | Item.MethodGroup (_, (ILMeth (_,ilinfo,_)) :: _, _) 
                  | Item.CtorGroup (_, (ILMeth (_,ilinfo,_)) :: _) -> Item.Types ("", [ ilinfo.ApparentEnclosingType ])
                  | Item.ILField (ILFieldInfo (typeInfo, _)) -> Item.Types ("", [ typeInfo.ToType ])
                  | Item.ImplicitOp(_, {contents = Some(TraitConstraintSln.FSMethSln(_, vref, _))}) -> Item.Value(vref)
                  | _                                         -> item

              let fail defaultReason = 
                  match item with            
#if EXTENSIONTYPING
                  | Params.ItemIsProvidedType g (tcref) -> FSharpFindDeclResult.DeclNotFound (FSharpFindDeclFailureReason.ProvidedType(tcref.DisplayName))
                  | Item.CtorGroup(name, ProvidedMeth(_)::_)
                  | Item.MethodGroup(name, ProvidedMeth(_)::_, _)
                  | Item.Property(name, ProvidedProp(_)::_) -> FSharpFindDeclResult.DeclNotFound (FSharpFindDeclFailureReason.ProvidedMember(name))
                  | Item.Event(ProvidedEvent(_) as e) -> FSharpFindDeclResult.DeclNotFound (FSharpFindDeclFailureReason.ProvidedMember(e.EventName))
                  | Item.ILField(ProvidedField(_) as f) -> FSharpFindDeclResult.DeclNotFound (FSharpFindDeclFailureReason.ProvidedMember(f.FieldName))
#endif
                  | _ -> FSharpFindDeclResult.DeclNotFound defaultReason

              match rangeOfItem g preferFlag item with
              | None   -> fail FSharpFindDeclFailureReason.Unknown 
              | Some itemRange -> 

                  let projectDir = Filename.directoryName (if projectFileName = "" then mainInputFileName else projectFileName)
                  let filename = fileNameOfItem g (Some projectDir) itemRange item
                  if FileSystem.SafeExists filename then 
                      FSharpFindDeclResult.DeclFound (mkRange filename itemRange.Start itemRange.End)
                  else 
                      fail FSharpFindDeclFailureReason.NoSourceCode // provided items may have TypeProviderDefinitionLocationAttribute that binds them to some location

    member scope.GetSymbolUseAtLocation (ctok, line, lineStr, colAtEndOfNames, names) =
        match GetDeclItemsForNamesAtPosition (ctok, None,Some(names), None, line, lineStr, colAtEndOfNames, ResolveTypeNamesToCtors, ResolveOverloads.Yes, fun _ -> false) with
        | None | Some ([], _, _) -> None
        | Some (item :: _ , denv, m) -> 
            let symbol = FSharpSymbol.Create(g, thisCcu, tcImports, item)
            Some (symbol, denv, m)

    member scope.PartialAssemblySignature() = FSharpAssemblySignature(g, thisCcu, tcImports, None, ccuSig)

    member scope.AccessRights =  tcAccessRights

    member scope.GetReferencedAssemblies() = 
        [ for x in tcImports.GetImportedAssemblies() do 
                yield FSharpAssembly(g, tcImports, x.FSharpViewOfMetadata) ]

    // Not, this does not have to be a SyncOp, it can be called from any thread
    member scope.GetFormatSpecifierLocations() = 
         sSymbolUses.GetFormatSpecifierLocations() 

    // Not, this does not have to be a SyncOp, it can be called from any thread
    member scope.GetSemanticClassification(range: range option) : (range * SemanticClassificationType) [] =
        let (|LegitTypeOccurence|_|) = function
            | ItemOccurence.UseInType
            | ItemOccurence.UseInAttribute
            | ItemOccurence.Use _
            | ItemOccurence.Binding _
            | ItemOccurence.Pattern _ -> Some()
            | _ -> None

        let inline (|OptionalArgumentAttribute|_|) ttype =
            match ttype with
            | TType.TType_app(tref, _) when tref.Stamp = g.attrib_OptionalArgumentAttribute.TyconRef.Stamp -> Some()
            | _ -> None

        let resolutions =
            match range with
            | Some range ->
                sResolutions.CapturedNameResolutions
                |> Seq.filter (fun cnr -> rangeContainsPos range cnr.Range.Start || rangeContainsPos range cnr.Range.End)
            | None -> 
                sResolutions.CapturedNameResolutions :> seq<_>

        resolutions
        |> Seq.choose (fun cnr ->
            match cnr with
            // 'seq' in 'seq { ... }' gets colored as keywords
            | CNR(_, (Item.Value vref), ItemOccurence.Use, _, _, _, m) when valRefEq g g.seq_vref vref ->
                Some (m, SemanticClassificationType.ComputationExpression)
            
            | CNR(_, (Item.Value vref), _, _, _, _, m) when isFunction g vref.Type ->
                if vref.IsPropertyGetterMethod || vref.IsPropertySetterMethod then
                    Some (m, SemanticClassificationType.Property)
                elif not (IsOperatorName vref.DisplayName) then
                    Some (m, SemanticClassificationType.Function)
                else None
            | CNR(_, (Item.Value vref), _, _, _, _, m) when vref.IsMutable ->
                Some (m, SemanticClassificationType.MutableVar)
            // todo here we should check if a `vref` is of type `ref`1`
            // (the commented code does not work)

            //| CNR(_, (Item.Value vref), _, _, _, _, m) ->
            //    match vref.TauType with
            //    | TType.TType_app(tref, _) ->  //  g.refcell_tcr_canon.t _refcell_tcr_canon canon.Deref.type vref ->
            //        if g.refcell_tcr_canon.Deref.Stamp = tref.Deref.Stamp then
            //            Some (m, SemanticClassificationType.MutableVar)
            //        else None
            //    | _ -> None
            | CNR(_, Item.RecdField rfinfo, _, _, _, _, m) when rfinfo.RecdField.IsMutable && rfinfo.LiteralValue.IsNone -> 
                Some (m, SemanticClassificationType.MutableVar)
            | CNR(_, Item.MethodGroup(_, _, _), _, _, _, _, m) ->
                Some (m, SemanticClassificationType.Function)
            // custom builders, custom operations get colored as keywords
            | CNR(_, (Item.CustomBuilder _ | Item.CustomOperation _), ItemOccurence.Use, _, _, _, m) ->
                Some (m, SemanticClassificationType.ComputationExpression)
            // well known type aliases get colored as keywords
            | CNR(_, (Item.Types (n, _)), _, _, _, _, m) when keywordTypes.Contains(n) ->
                Some (m, SemanticClassificationType.IntrinsicType)
            // types get colored as types when they occur in syntactic types or custom attributes
            // typevariables get colored as types when they occur in syntactic types custom builders, custom operations get colored as keywords
            | CNR(_, Item.Types (_, [OptionalArgumentAttribute]), LegitTypeOccurence, _, _, _, _) -> None
            | CNR(_, Item.CtorGroup(_, [MethInfo.FSMeth(_, OptionalArgumentAttribute, _, _)]), LegitTypeOccurence, _, _, _, _) -> None
            | CNR(_, Item.Types(_, types), LegitTypeOccurence, _, _, _, m) when types |> List.exists (isInterfaceTy g) -> 
                Some (m, SemanticClassificationType.Interface)
            | CNR(_, Item.Types(_, types), LegitTypeOccurence, _, _, _, m) when types |> List.exists (isStructTy g) -> 
                Some (m, SemanticClassificationType.ValueType)
            | CNR(_, Item.Types _, LegitTypeOccurence, _, _, _, m) -> 
                Some (m, SemanticClassificationType.ReferenceType)
            | CNR(_, (Item.TypeVar _ | Item.UnqualifiedType _ | Item.CtorGroup _), LegitTypeOccurence, _, _, _, m) ->
                Some (m, SemanticClassificationType.ReferenceType)
            | CNR(_, Item.ModuleOrNamespaces refs, LegitTypeOccurence, _, _, _, m) when refs |> List.exists (fun x -> x.IsModule) ->
                Some (m, SemanticClassificationType.ReferenceType)
            | CNR(_, (Item.ActivePatternCase _ | Item.UnionCase _ | Item.ActivePatternResult _), _, _, _, _, m) ->
                Some (m, SemanticClassificationType.UnionCase)
            | _ -> None)
        |> Seq.toArray
        |> Array.append (sSymbolUses.GetFormatSpecifierLocations() |> Array.map (fun m -> m, SemanticClassificationType.Printf))

    member x.ScopeResolutions = sResolutions
    member x.ScopeSymbolUses = sSymbolUses
    member x.TcGlobals = g
    member x.TcImports = tcImports
    member x.CcuSig = ccuSig
    member x.ThisCcu = thisCcu

module internal Parser = 

        // We'll need number of lines for adjusting error messages at EOF
    let GetFileInfoForLastLineErrors (source: string) = 
        // number of lines in the source file
        let lastLine = (source |> Seq.sumBy (fun c -> if c = '\n' then 1 else 0)) + 1
        // length of the last line
        let lastLineLength = source.Length - source.LastIndexOf("\n",StringComparison.Ordinal) - 1
        lastLine, lastLineLength
         
    let ReportError (tcConfig:TcConfig, allErrors, mainInputFileName, fileInfo, (exn, sev)) = 
        [ let isError = (sev = FSharpErrorSeverity.Error) || ReportWarningAsError (tcConfig.globalWarnLevel, tcConfig.specificWarnOff, tcConfig.specificWarnOn, tcConfig.specificWarnAsError, tcConfig.specificWarnAsWarn, tcConfig.globalWarnAsError) exn                
          if (isError || ReportWarning (tcConfig.globalWarnLevel, tcConfig.specificWarnOff, tcConfig.specificWarnOn) exn) then 
            let oneError trim exn = 
                [ // We use the first line of the file as a fallbackRange for reporting unexpected errors.
                  // Not ideal, but it's hard to see what else to do.
                  let fallbackRange = rangeN mainInputFileName 1
                  let ei = FSharpErrorInfo.CreateFromExceptionAndAdjustEof (exn, isError, trim, fallbackRange, fileInfo)
                  if allErrors || (ei.FileName=mainInputFileName) || (ei.FileName=Microsoft.FSharp.Compiler.TcGlobals.DummyFileNameForRangesWithoutASpecificLocation) then
                      yield ei ]
                      
            let mainError,relatedErrors = SplitRelatedDiagnostics exn 
            yield! oneError false mainError
            for e in relatedErrors do 
                yield! oneError true e ]

    let CreateErrorInfos (tcConfig:TcConfig, allErrors, mainInputFileName, errors) = 
        let fileInfo = (Int32.MaxValue, Int32.MaxValue)
        [| for (exn,isError) in errors do 
              yield! ReportError (tcConfig, allErrors, mainInputFileName, fileInfo, (exn, isError)) |]
                            

    /// Error handler for parsing & type checking while processing a single file
    type ErrorHandler(reportErrors, mainInputFileName, tcConfig: TcConfig, source: string) =
        let mutable tcConfig = tcConfig
        let errorsAndWarningsCollector = new ResizeArray<_>()
        let mutable errorCount = 0
         
        // We'll need number of lines for adjusting error messages at EOF
        let fileInfo = GetFileInfoForLastLineErrors source
         
        // This function gets called whenever an error happens during parsing or checking
        let diagnosticSink sev (exn:PhasedDiagnostic) = 
            // Sanity check here. The phase of an error should be in a phase known to the language service.
            let exn =
                if not(exn.IsPhaseInCompile()) then
                    // Reaching this point means that the error would be sticky if we let it prop up to the language service.
                    // Assert and recover by replacing phase with one known to the language service.
                    System.Diagnostics.Debug.Assert(false, sprintf "The subcategory '%s' seen in an error should not be seen by the language service" (exn.Subcategory()))
                    {exn with Phase=BuildPhase.TypeCheck}
                else exn
            if reportErrors then 
                let report exn = 
                    for ei in ReportError (tcConfig, false, mainInputFileName, fileInfo, (exn, sev)) do
                        errorsAndWarningsCollector.Add ei
                        if sev = FSharpErrorSeverity.Error then 
                            errorCount <- errorCount + 1
                      
                match exn with
#if EXTENSIONTYPING
                | {Exception = (:? TypeProviderError as tpe)} ->
                    tpe.Iter (fun e ->
                        let newExn = {exn with Exception = e}
                        report newExn
                    )
#endif
                | e -> report e
      
        let errorLogger = 
            { new ErrorLogger("ErrorHandler") with 
                member x.DiagnosticSink (exn, isError) = diagnosticSink (if isError then FSharpErrorSeverity.Error else FSharpErrorSeverity.Warning) exn
                member x.ErrorCount = errorCount }
      
      
        // Public members
        member x.ErrorLogger = errorLogger
        member x.CollectedDiagnostics = errorsAndWarningsCollector.ToArray()
        member x.ErrorCount = errorCount
        member x.TcConfig with set tc = tcConfig <- tc
        member x.AnyErrors = errorCount > 0


    /// ParseOneFile builds all the information necessary to report errors, match braces and build scopes 
    ///
    /// projectSourceFiles is only used to compute isLastCompiland, and is ignored if Build.IsScript(mainInputFileName)  is true.
    let ParseOneFile (ctok, source: string, matchBracesOnly: bool, reportErrors: bool, mainInputFileName: string, projectSourceFiles: string list, tcConfig: TcConfig) =

          // This function requires the compilation thread because we install error handlers, whose callbacks must
          // be invoked on the compilation thread, no other reason known to date.
          // We should check whether those are "real" reasons - we could for example make collecting errors thread safe.
          RequireCompilationThread ctok 

          // Initialize the error handler 
          let errHandler = new ErrorHandler(reportErrors, mainInputFileName, tcConfig, source)

          let source = source + "\n\n\n"
          let lexbuf = UnicodeLexing.StringAsLexbuf source

          // Collector for parens matching
          let matchPairRef = new ResizeArray<_>()

          use unwindEL = PushErrorLoggerPhaseUntilUnwind (fun _oldLogger -> errHandler.ErrorLogger)
          use unwindBP = PushThreadBuildPhaseUntilUnwind BuildPhase.Parse

          // Errors on while parsing project arguments 

          let parseResult = 

              // If we're editing a script then we define INTERACTIVE otherwise COMPILED. Since this parsing for intellisense we always
              // define EDITING
              let conditionalCompilationDefines =
                SourceFileImpl.AdditionalDefinesForUseInEditor(mainInputFileName) @ tcConfig.conditionalCompilationDefines 
        
              let lightSyntaxStatusInital = tcConfig.ComputeLightSyntaxInitialStatus (mainInputFileName)
              let lightSyntaxStatus = LightSyntaxStatus(lightSyntaxStatusInital,true)

              // Note: we don't really attempt to intern strings across a large scope
              let lexResourceManager = new Lexhelp.LexResourceManager()
              let lexargs = mkLexargs(mainInputFileName,
                                      conditionalCompilationDefines,
                                      lightSyntaxStatus,
                                      lexResourceManager,
                                      ref [],
                                      errHandler.ErrorLogger)
              Lexhelp.usingLexbufForParsing (lexbuf, mainInputFileName) (fun lexbuf -> 
                  try 
                    let skip = true
                    let tokenizer = LexFilter.LexFilter (lightSyntaxStatus, tcConfig.compilingFslib, Lexer.token lexargs skip, lexbuf)
                    let lexfun = tokenizer.Lexer
                    if matchBracesOnly then 
                        // Quick bracket matching parse  
                        let parenTokensBalance t1 t2 = 
                            match t1,t2 with 
                            | (LPAREN,RPAREN) 
                            | (LPAREN,RPAREN_IS_HERE) 
                            | (LBRACE,RBRACE) 
                            | (LBRACE,RBRACE_IS_HERE) 
                            | (SIG,END) 
                            | (STRUCT,END) 
                            | (LBRACK_BAR,BAR_RBRACK)
                            | (LBRACK,RBRACK)
                            | (LBRACK_LESS,GREATER_RBRACK)
                            | (BEGIN,END) -> true 
                            | (LQUOTE q1,RQUOTE q2) when q1 = q2 -> true 
                            | _ -> false
                        let rec matchBraces stack = 
                            match lexfun lexbuf,stack with 
                            | tok2,((tok1,m1) :: stack') when parenTokensBalance tok1 tok2-> 
                                if matchBracesOnly then 
                                    matchPairRef.Add (m1, lexbuf.LexemeRange)
                                matchBraces stack'
                            | ((LPAREN | LBRACE | LBRACK | LBRACK_BAR | LQUOTE _ | LBRACK_LESS) as tok),_ -> matchBraces ((tok,lexbuf.LexemeRange) :: stack)
                            | (EOF _ | LEX_FAILURE _),_ -> ()
                            | _ -> matchBraces stack

                        matchBraces []
                        None
                    else 
                        let isLastCompiland = 
                            projectSourceFiles.Length >= 1 && 
                            System.String.Compare(projectSourceFiles.[projectSourceFiles.Length-1],mainInputFileName,StringComparison.CurrentCultureIgnoreCase)=0
                        let isLastCompiland = isLastCompiland || CompileOps.IsScript(mainInputFileName)  
                        let isExe = tcConfig.target.IsExe
                        let parseResult = ParseInput(lexfun,errHandler.ErrorLogger,lexbuf,None,mainInputFileName,(isLastCompiland,isExe))
                        Some parseResult
                  with e -> 
                    errHandler.ErrorLogger.ErrorR(e)
                    None)
                

          errHandler.CollectedDiagnostics,
          matchPairRef.ToArray(),
          parseResult,
          errHandler.AnyErrors


    /// Indicates if the type check got aborted because it is no longer relevant.
    type TypeCheckAborted = Yes | No of TypeCheckInfo

    // Type check a single file against an initial context, gleaning both errors and intellisense information.
    let TypeCheckOneFile
          (parseResults: FSharpParseFileResults,
           source: string,
           mainInputFileName: string,
           projectFileName: string,
           tcConfig: TcConfig,
           tcGlobals: TcGlobals,
           tcImports: TcImports,
           tcState: TcState,
           loadClosure: LoadClosure option,
           // These are the errors and warnings seen by the background compiler for the entire antecedant 
           backgroundDiagnostics: (PhasedDiagnostic * FSharpErrorSeverity) list,    
           reactorOps: IReactorOperations,
           // Used by 'FSharpDeclarationListInfo' to check the IncrementalBuilder is still alive.
           checkAlive : (unit -> bool),
           textSnapshotInfo : obj option) = 
        
        async {
            match parseResults.ParseTree with 
            // When processing the following cases, we don't need to type-check
            | None -> return [||], TypeCheckAborted.Yes
                   
            // Run the type checker...
            | Some parsedMainInput ->
                // Initialize the error handler 
                let errHandler = new ErrorHandler(true, mainInputFileName, tcConfig, source)
                
                use _unwindEL = PushErrorLoggerPhaseUntilUnwind (fun _oldLogger -> errHandler.ErrorLogger)
                use _unwindBP = PushThreadBuildPhaseUntilUnwind BuildPhase.TypeCheck
            
                // Apply nowarns to tcConfig (may generate errors, so ensure errorLogger is installed)
                let tcConfig = ApplyNoWarnsToTcConfig (tcConfig, parsedMainInput,Path.GetDirectoryName mainInputFileName)
                        
                // update the error handler with the modified tcConfig
                errHandler.TcConfig <- tcConfig
            
                // Play background errors and warnings for this file.
                for (err,sev) in backgroundDiagnostics do
                    diagnosticSink (err, (sev = FSharpErrorSeverity.Error))
            
                // If additional references were brought in by the preprocessor then we need to process them
                match loadClosure with
                | Some loadClosure ->
                    // Play unresolved references for this file.
                    tcImports.ReportUnresolvedAssemblyReferences(loadClosure.UnresolvedReferences)
            
                    // If there was a loadClosure, replay the errors and warnings from resolution, excluding parsing
                    loadClosure.LoadClosureRootFileDiagnostics |> List.iter diagnosticSink
            
                    let fileOfBackgroundError err = (match GetRangeOfDiagnostic (fst err) with Some m-> m.FileName | None -> null)
                    let sameFile file hashLoadInFile = 
                        (0 = String.Compare(hashLoadInFile, file, StringComparison.OrdinalIgnoreCase))
            
                    //  walk the list of #loads and keep the ones for this file.
                    let hashLoadsInFile = 
                        loadClosure.SourceFiles 
                        |> List.filter(fun (_,ms) -> ms<>[]) // #loaded file, ranges of #load
            
                    let hashLoadBackgroundDiagnostics, otherBackgroundDiagnostics = 
                        backgroundDiagnostics 
                        |> List.partition (fun backgroundError -> 
                            hashLoadsInFile 
                            |>  List.exists (fst >> sameFile (fileOfBackgroundError backgroundError)))
            
                    // Create single errors for the #load-ed files.
                    // Group errors and warnings by file name.
                    let hashLoadBackgroundDiagnosticsGroupedByFileName = 
                        hashLoadBackgroundDiagnostics 
                        |> List.map(fun err -> fileOfBackgroundError err,err) 
                        |> List.groupByFirst  // fileWithErrors, error list
            
                    //  Join the sets and report errors. 
                    //  It is by-design that these messages are only present in the language service. A true build would report the errors at their
                    //  spots in the individual source files.
                    for (fileOfHashLoad, rangesOfHashLoad) in hashLoadsInFile do
                        for errorGroupedByFileName in hashLoadBackgroundDiagnosticsGroupedByFileName do
                            if sameFile (fst errorGroupedByFileName) fileOfHashLoad then
                                for rangeOfHashLoad in rangesOfHashLoad do // Handle the case of two #loads of the same file
                                    let diagnostics = snd errorGroupedByFileName |> List.map(fun (pe,f)->pe.Exception,f) // Strip the build phase here. It will be replaced, in total, with TypeCheck
                                    let errors = [ for (err,sev) in diagnostics do if sev = FSharpErrorSeverity.Error then yield err ]
                                    let warnings = [ for (err,sev) in diagnostics do if sev = FSharpErrorSeverity.Warning then yield err ]
                                    
                                    let message = HashLoadedSourceHasIssues(warnings,errors,rangeOfHashLoad)
                                    if errors=[] then warning(message)
                                    else errorR(message)
            
                    // Replay other background errors.
                    for (phasedError,sev) in otherBackgroundDiagnostics do
                        if sev = FSharpErrorSeverity.Warning then 
                            warning phasedError.Exception 
                        else errorR phasedError.Exception
            
                | None -> 
                    // For non-scripts, check for disallow #r and #load.
                    ApplyMetaCommandsFromInputToTcConfig (tcConfig, parsedMainInput,Path.GetDirectoryName mainInputFileName) |> ignore
                    
                // A problem arises with nice name generation, which really should only 
                // be done in the backend, but is also done in the typechecker for better or worse. 
                // If we don't do this the NNG accumulates data and we get a memory leak. 
                tcState.NiceNameGenerator.Reset()
                
                // Typecheck the real input.  
                let sink = TcResultsSinkImpl(tcGlobals, source = source)
                let! ct = Async.CancellationToken
            
                let! tcEnvAtEndOpt =
                    async {
                        try
                            let checkForErrors() = (parseResults.ParseHadErrors || errHandler.ErrorCount > 0)
                            // Typecheck is potentially a long running operation. We chop it up here with an Eventually continuation and, at each slice, give a chance
                            // for the client to claim the result as obsolete and have the typecheck abort.
                            
                            let! result = 
                                TypeCheckOneInputAndFinishEventually(checkForErrors, tcConfig, tcImports, tcGlobals, None, TcResultsSink.WithSink sink, tcState, parsedMainInput)
                                |> Eventually.repeatedlyProgressUntilDoneOrTimeShareOverOrCanceled maxTimeShareMilliseconds ct (fun ctok f -> f ctok)
                                |> Eventually.forceAsync  
                                    (fun work ->
                                        reactorOps.EnqueueAndAwaitOpAsync("TypeCheckOneFile", 
                                            fun ctok _ -> 
                                                // Reinstall the compilation globals each time we start or restart
                                                use unwind = new CompilationGlobalsScope (errHandler.ErrorLogger, BuildPhase.TypeCheck)
                                                work ctok))
                             
                            return result |> Option.map (fun ((tcEnvAtEnd, _, typedImplFiles), tcState) -> tcEnvAtEnd, typedImplFiles, tcState)
                        with
                        | e ->
                            errorR e
                            return Some(tcState.TcEnvFromSignatures, [], tcState)
                    }
                
                let errors = errHandler.CollectedDiagnostics
                
                match tcEnvAtEndOpt with
                | Some (tcEnvAtEnd, _typedImplFiles, tcState) ->
                    let scope = 
                        TypeCheckInfo(tcConfig, tcGlobals, 
                                    tcState.PartialAssemblySignature, 
                                    tcState.Ccu,
                                    tcImports,
                                    tcEnvAtEnd.AccessRights,
                                    //typedImplFiles,
                                    projectFileName, 
                                    mainInputFileName, 
                                    sink.GetResolutions(), 
                                    sink.GetSymbolUses(), 
                                    tcEnvAtEnd.NameEnv,
                                    loadClosure,
                                    reactorOps,
                                    checkAlive,
                                    textSnapshotInfo)     
                    return errors, TypeCheckAborted.No scope
                | None -> 
                    return errors, TypeCheckAborted.Yes
        }

type  UnresolvedReferencesSet = UnresolvedReferencesSet of UnresolvedAssemblyReference list

// NOTE: may be better just to move to optional arguments here
type FSharpProjectOptions =
    { 
      ProjectFileName: string
      ProjectFileNames: string[]
      OtherOptions: string[]
      ReferencedProjects: (string * FSharpProjectOptions)[]
      IsIncompleteTypeCheckEnvironment : bool
      UseScriptResolutionRules : bool      
      LoadTime : System.DateTime
      UnresolvedReferences : UnresolvedReferencesSet option
      OriginalLoadReferences: (range * string) list
      ExtraProjectInfo : obj option
    }
    member x.ProjectOptions = x.OtherOptions
    /// Whether the two parse options refer to the same project.
    static member AreSubsumable(options1,options2) =
        options1.ProjectFileName = options2.ProjectFileName          

    /// Compare two options sets with respect to the parts of the options that are important to parsing.
    static member AreSameForParsing(options1,options2) =
        options1.ProjectFileName = options2.ProjectFileName &&
        options1.OtherOptions = options2.OtherOptions &&
        options1.UnresolvedReferences = options2.UnresolvedReferences

    /// Compare two options sets with respect to the parts of the options that are important to building.
    static member AreSameForChecking(options1,options2) =
        options1.ProjectFileName = options2.ProjectFileName &&
        options1.ProjectFileNames = options2.ProjectFileNames &&
        options1.OtherOptions = options2.OtherOptions &&
        options1.UnresolvedReferences = options2.UnresolvedReferences &&
        options1.OriginalLoadReferences = options2.OriginalLoadReferences &&
        options1.ReferencedProjects.Length = options2.ReferencedProjects.Length &&
        Array.forall2 (fun (n1,a) (n2,b) -> n1 = n2 && FSharpProjectOptions.AreSameForChecking(a,b)) options1.ReferencedProjects options2.ReferencedProjects &&
        options1.LoadTime = options2.LoadTime

    /// Compute the project directory.
    member po.ProjectDirectory = System.IO.Path.GetDirectoryName(po.ProjectFileName)
    override this.ToString() =
        let files =
            let sb = new StringBuilder()
            this.ProjectFileNames |> Array.iter (fun file -> sb.AppendFormat("    {0}\n", file) |> ignore)
            sb.ToString()
        let options =
            let sb = new StringBuilder()
            this.OtherOptions |> Array.iter (fun op -> sb.AppendFormat("{0} ", op) |> ignore)
            sb.ToString()
        sprintf "OtherOptions(%s)\n  Files:\n%s  Options: %s" this.ProjectFileName files options
 

[<Sealed>] 
type FSharpProjectContext(thisCcu: CcuThunk, assemblies: FSharpAssembly list, ad: AccessorDomain) =

    /// Get the assemblies referenced
    member __.GetReferencedAssemblies() = assemblies

    member __.AccessibilityRights = FSharpAccessibilityRights(thisCcu, ad)


[<Sealed>]
// 'details' is an option because the creation of the tcGlobals etc. for the project may have failed.
type FSharpCheckProjectResults(_keepAssemblyContents, errors: FSharpErrorInfo[], details:(TcGlobals*TcImports*CcuThunk*ModuleOrNamespaceType*TcSymbolUses list*TopAttribs option*CompileOps.IRawFSharpAssemblyData option * ILAssemblyRef * AccessorDomain * TypedImplFile list option) option, reactorOps: IReactorOperations) =

    let getDetails() = 
        match details with 
        | None -> invalidOp ("The project has no results due to critical errors in the project options. Check the HasCriticalErrors before accessing the detaild results. Errors: " + String.concat "\n" [ for e in errors -> e.Message ])
        | Some d -> d

    member info.Errors = errors

    member info.HasCriticalErrors = details.IsNone

    member info.AssemblySignature =  
        let (tcGlobals, tcImports, thisCcu, ccuSig, _tcSymbolUses, topAttribs, _tcAssemblyData, _ilAssemRef, _ad, _tcAssemblyExpr) = getDetails()
        FSharpAssemblySignature(tcGlobals, thisCcu, tcImports, topAttribs, ccuSig)

    // member info.AssemblyContents =  
    //     if not keepAssemblyContents then invalidOp "The 'keepAssemblyContents' flag must be set to tru on the FSharpChecker in order to access the checked contents of assemblies"
    //     let (tcGlobals, tcImports, thisCcu, _ccuSig, _tcSymbolUses, _topAttribs, _tcAssemblyData, _ilAssemRef, _ad, tcAssemblyExpr) = getDetails()
    //     let mimpls = 
    //         match tcAssemblyExpr with 
    //         | None -> []
    //         | Some mimpls -> mimpls
    //     FSharpAssemblyContents(tcGlobals, thisCcu, tcImports, mimpls)

    // Not, this does not have to be a SyncOp, it can be called from any thread
    member info.GetUsesOfSymbol(symbol:FSharpSymbol) = 
        let (tcGlobals, _tcImports, _thisCcu, _ccuSig, tcSymbolUses, _topAttribs, _tcAssemblyData, _ilAssemRef, _ad, _tcAssemblyExpr) = getDetails()
        // This probably doesn't need to be run on the reactor since all data touched by GetUsesOfSymbol is immutable.
        reactorOps.EnqueueAndAwaitOpAsync("GetUsesOfSymbol", fun ctok _ct -> 
            DoesNotRequireCompilerThreadTokenAndCouldPossiblyBeMadeConcurrent  ctok

            [| for r in tcSymbolUses do yield! r.GetUsesOfSymbol(symbol.Item) |] 
            |> Seq.distinctBy (fun (itemOcc,_denv,m) -> itemOcc, m) 
            |> Seq.filter (fun (itemOcc,_,_) -> itemOcc <> ItemOccurence.RelatedText) 
            |> Seq.map (fun (itemOcc,denv,m) -> FSharpSymbolUse(tcGlobals, denv, symbol, itemOcc, m)) 
            |> Seq.toArray)

    // Not, this does not have to be a SyncOp, it can be called from any thread
    member info.GetAllUsesOfAllSymbols() = 
        let (tcGlobals, tcImports, thisCcu, _ccuSig, tcSymbolUses, _topAttribs, _tcAssemblyData, _ilAssemRef, _ad, _tcAssemblyExpr) = getDetails()
        // This probably doesn't need to be run on the reactor since all data touched by GetAllUsesOfSymbols is immutable.
        reactorOps.EnqueueAndAwaitOpAsync("GetAllUsesOfAllSymbols", fun ctok _ct -> 
            DoesNotRequireCompilerThreadTokenAndCouldPossiblyBeMadeConcurrent  ctok

            [| for r in tcSymbolUses do 
                  for (item,itemOcc,denv,m) in r.GetAllUsesOfSymbols() do
                   if itemOcc <> ItemOccurence.RelatedText then
                    let symbol = FSharpSymbol.Create(tcGlobals, thisCcu, tcImports, item)
                    yield FSharpSymbolUse(tcGlobals, denv, symbol, itemOcc, m) |]) 

    member info.ProjectContext = 
        let (tcGlobals, tcImports, thisCcu, _ccuSig, _tcSymbolUses, _topAttribs, _tcAssemblyData, _ilAssemRef, ad, _tcAssemblyExpr) = getDetails()
        let assemblies = 
            [ for x in tcImports.GetImportedAssemblies() do
                yield FSharpAssembly(tcGlobals, tcImports, x.FSharpViewOfMetadata) ]
        FSharpProjectContext(thisCcu, assemblies, ad) 

    member info.RawFSharpAssemblyData = 
        let (_tcGlobals, _tcImports, _thisCcu, _ccuSig, _tcSymbolUses, _topAttribs, tcAssemblyData, _ilAssemRef, _ad, _tcAssemblyExpr) = getDetails()
        tcAssemblyData

    member info.AssemblyFullName = 
        let (_tcGlobals, _tcImports, _thisCcu, _ccuSig, _tcSymbolUses, _topAttribs, _tcAssemblyData, ilAssemRef, _ad, _tcAssemblyExpr) = getDetails()
        ilAssemRef.QualifiedName

[<Sealed>]
/// A live object of this type keeps the background corresponding background builder (and type providers) alive (through reference-counting).
//
// There is an important property of all the objects returned by the methods of this type: they do not require 
// the corresponding background builder to be alive. That is, they are simply plain-old-data through pre-formatting of all result text.
type FSharpCheckFileResults(errors: FSharpErrorInfo[], scopeOptX: TypeCheckInfo option, builderX: IncrementalBuilder option, reactorOpsX:IReactorOperations) =

    // This may be None initially, or may be set to None when the object is disposed or finalized
    let mutable details = match scopeOptX with None -> None | Some scopeX -> Some (scopeX, builderX, reactorOpsX)

    let decrementer = 
        match details with 
        | Some (_,Some builder,_) -> 
            // Increment the usage count on the IncrementalBuilder. We want to keep the IncrementalBuilder and all associated
            // resources and type providers alive for the duration of the lifetime of this object.
            builder.IncrementUsageCount()
        | _ -> { new System.IDisposable with member x.Dispose() = () } 

    let mutable disposed = false

    let dispose() = 
       if not disposed then 
           disposed <- true 
           match details with 
           | Some (_,_,reactor) -> 
               // Make sure we run disposal in the reactor thread, since it may trigger type provider disposals etc.
               details <- None
               reactor.EnqueueOp ("Dispose", fun ctok -> 
                   RequireCompilationThread ctok
                   decrementer.Dispose())
           | _ -> () 

    // Run an operation that needs to be run in the reactor thread
    let reactorOp desc dflt f = 
      async {
        match details with
        | None -> 
            return dflt
        | Some (_ , Some builder, _) when not builder.IsAlive -> 
            System.Diagnostics.Debug.Assert(false,"unexpected dead builder") 
            return dflt
        | Some (scope, builderOpt, reactor) -> 
            // Ensure the builder doesn't get released while running operations asynchronously. 
            use _unwind = match builderOpt with Some builder -> builder.IncrementUsageCount() | None -> { new System.IDisposable with member __.Dispose() = () }
            let! res = reactor.EnqueueAndAwaitOpAsync(desc, fun ctok _ct ->  f ctok scope)
            return res
      }

    // Run an operation that can be called from any thread
    let threadSafeOp dflt f = 
        match details with
        | None -> 
            dflt()
        | Some (_ , Some builder, _) when not builder.IsAlive -> 
            System.Diagnostics.Debug.Assert(false,"unexpected dead builder") 
            dflt()
        | Some (scope, builderOpt, ops) -> 
            f(scope, builderOpt, ops)

    // At the moment we only dispose on finalize - we never explicitly dispose these objects. Explicitly disposing is not
    // really worth much since the underlying project builds are likely to still be in the incrementalBuilder cache.
    override info.Finalize() = dispose() 

    member info.Errors = errors

    member info.HasFullTypeCheckInfo = details.IsSome
    
    /// Intellisense autocompletions
    member info.GetDeclarationListInfo(parseResultsOpt, line, colAtEndOfNamesAndResidue, lineStr, qualifyingNames, partialName, ?hasTextChangedSinceLastTypecheck) = 
        let hasTextChangedSinceLastTypecheck = defaultArg hasTextChangedSinceLastTypecheck (fun _ -> false)
        reactorOp "GetDeclarations" FSharpDeclarationListInfo.Empty (fun ctok scope -> scope.GetDeclarations(ctok, parseResultsOpt, line, lineStr, colAtEndOfNamesAndResidue, qualifyingNames, partialName, hasTextChangedSinceLastTypecheck))

    member info.GetDeclarationListSymbols(parseResultsOpt, line, colAtEndOfNamesAndResidue, lineStr, qualifyingNames, partialName, ?hasTextChangedSinceLastTypecheck) = 
        let hasTextChangedSinceLastTypecheck = defaultArg hasTextChangedSinceLastTypecheck (fun _ -> false)
        reactorOp "GetDeclarationListSymbols" List.empty (fun ctok scope -> scope.GetDeclarationListSymbols(ctok, parseResultsOpt, line, lineStr, colAtEndOfNamesAndResidue, qualifyingNames, partialName, hasTextChangedSinceLastTypecheck))

    /// Resolve the names at the given location to give a data tip 
    member info.GetStructuredToolTipTextAlternate(line, colAtEndOfNames, lineStr, names, tokenTag) = 
        let dflt = FSharpToolTipText []
        match tokenTagToTokenId tokenTag with 
        | TOKEN_IDENT -> 
            reactorOp "GetToolTipText" dflt (fun ctok scope -> scope.GetStructuredToolTipText(ctok, line, lineStr, colAtEndOfNames, names))
        | TOKEN_STRING | TOKEN_STRING_TEXT -> 
            reactorOp "GetReferenceResolutionToolTipText" dflt (fun ctok scope -> scope.GetReferenceResolutionStructuredToolTipText(ctok, line, colAtEndOfNames) )
        | _ -> 
            async.Return dflt

    member info.GetToolTipTextAlternate(line, colAtEndOfNames, lineStr, names, tokenTag) = 
        info.GetStructuredToolTipTextAlternate(line, colAtEndOfNames, lineStr, names, tokenTag)
        |> Tooltips.Map Tooltips.ToFSharpToolTipText

    member info.GetF1KeywordAlternate (line, colAtEndOfNames, lineStr, names) =
        reactorOp "GetF1Keyword" None (fun ctok scope -> 
            scope.GetF1Keyword (ctok, line, lineStr, colAtEndOfNames, names))

    // Resolve the names at the given location to a set of methods
    member info.GetMethodsAlternate(line, colAtEndOfNames, lineStr, names) =
        let dflt = FSharpMethodGroup("",[| |])
        reactorOp "GetMethods" dflt (fun ctok scope -> 
            scope.GetMethods (ctok, line, lineStr, colAtEndOfNames, names))
            
    member info.GetDeclarationLocationAlternate (line, colAtEndOfNames, lineStr, names, ?preferFlag) = 
        let dflt = FSharpFindDeclResult.DeclNotFound FSharpFindDeclFailureReason.Unknown
        reactorOp "GetDeclarationLocation" dflt (fun ctok scope -> 
            scope.GetDeclarationLocation (ctok, line, lineStr, colAtEndOfNames, names, preferFlag))

    member info.GetSymbolUseAtLocation (line, colAtEndOfNames, lineStr, names) = 
        reactorOp "GetSymbolUseAtLocation" None (fun ctok scope -> 
            scope.GetSymbolUseAtLocation (ctok, line, lineStr, colAtEndOfNames, names)
            |> Option.map (fun (sym,denv,m) -> FSharpSymbolUse(scope.TcGlobals,denv,sym,ItemOccurence.Use,m)))

    member info.GetMethodsAsSymbols (line, colAtEndOfNames, lineStr, names) = 
        reactorOp "GetMethodsAsSymbols" None (fun ctok scope -> 
            scope.GetMethodsAsSymbols (ctok, line, lineStr, colAtEndOfNames, names)
            |> Option.map (fun (symbols,denv,m) ->
                symbols |> List.map (fun sym -> FSharpSymbolUse(scope.TcGlobals,denv,sym,ItemOccurence.Use,m))))

    member info.GetSymbolAtLocationAlternate (line, colAtEndOfNames, lineStr, names) = 
        reactorOp "GetSymbolUseAtLocation" None (fun ctok scope -> 
            scope.GetSymbolUseAtLocation (ctok, line, lineStr, colAtEndOfNames, names)
            |> Option.map (fun (sym,_,_) -> sym))

    member info.GetFormatSpecifierLocations() = 
        threadSafeOp 
           (fun () -> [| |]) 
           (fun (scope, _builder, _reactor) -> 
            // This operation is not asynchronous - GetFormatSpecifierLocations can be run on the calling thread
            scope.GetFormatSpecifierLocations())

    member info.GetSemanticClassification(range: range option) =
        threadSafeOp 
           (fun () -> [| |]) 
           (fun (scope, _builder, _reactor) -> 
            // This operation is not asynchronous - GetExtraColorizations can be run on the calling thread
            scope.GetSemanticClassification(range))
     
    member info.PartialAssemblySignature = 
        threadSafeOp 
            (fun () -> failwith "not available") 
            (fun (scope, _builder, _reactor) -> 
            // This operation is not asynchronous - PartialAssemblySignature can be run on the calling thread
            scope.PartialAssemblySignature())

    member info.ProjectContext = 
        threadSafeOp 
            (fun () -> failwith "not available") 
            (fun (scope, _builder, _reactor) -> 
               // This operation is not asynchronous - GetReferencedAssemblies can be run on the calling thread
                FSharpProjectContext(scope.ThisCcu, scope.GetReferencedAssemblies(), scope.AccessRights))

    member info.GetAllUsesOfAllSymbolsInFile() = 
        reactorOp "GetAllUsesOfAllSymbolsInFile" [| |] (fun ctok scope -> 

            DoesNotRequireCompilerThreadTokenAndCouldPossiblyBeMadeConcurrent  ctok

            [| for (item,itemOcc,denv,m) in scope.ScopeSymbolUses.GetAllUsesOfSymbols() do
                 if itemOcc <> ItemOccurence.RelatedText then
                  let symbol = FSharpSymbol.Create(scope.TcGlobals, scope.ThisCcu, scope.TcImports, item)
                  yield FSharpSymbolUse(scope.TcGlobals, denv, symbol, itemOcc, m) |])

    member info.GetUsesOfSymbolInFile(symbol:FSharpSymbol) = 
        reactorOp "GetUsesOfSymbolInFile" [| |] (fun ctok scope -> 

            DoesNotRequireCompilerThreadTokenAndCouldPossiblyBeMadeConcurrent  ctok

            [| for (itemOcc,denv,m) in scope.ScopeSymbolUses.GetUsesOfSymbol(symbol.Item) |> Seq.distinctBy (fun (itemOcc,_denv,m) -> itemOcc, m) do
                 if itemOcc <> ItemOccurence.RelatedText then
                  yield FSharpSymbolUse(scope.TcGlobals, denv, symbol, itemOcc, m) |])

    member info.IsRelativeNameResolvable(pos: pos, plid: string list, item: Item) : Async<bool> = 
        reactorOp "IsRelativeNameResolvable" true (fun ctok scope -> 
            DoesNotRequireCompilerThreadTokenAndCouldPossiblyBeMadeConcurrent  ctok
            scope.IsRelativeNameResolvable(pos, plid, item))
    
//----------------------------------------------------------------------------
// BackgroundCompiler
//

[<NoComparison>]
type FSharpCheckFileAnswer =
    | Aborted
    | Succeeded of FSharpCheckFileResults   
        

/// Callback that indicates whether a requested result has become obsolete.    
[<NoComparison;NoEquality>]
type (*internal*) IsResultObsolete = 
    | IsResultObsolete of (unit->bool)


[<AutoOpen>]
module Helpers = 
    
    /// Determine whether two (fileName,options) keys are identical w.r.t. affect on checking
    let AreSameForChecking2((fileName1: string, options1: FSharpProjectOptions), (fileName2, o2)) =
        (fileName1 = fileName2) 
        && FSharpProjectOptions.AreSameForChecking(options1,o2)
        
    /// Determine whether two (fileName,options) keys should be identical w.r.t. resource usage
    let AreSubsumable2((fileName1:string,o1:FSharpProjectOptions),(fileName2:string,o2:FSharpProjectOptions)) =
        (fileName1 = fileName2)
        && FSharpProjectOptions.AreSubsumable(o1,o2)

    /// Determine whether two (fileName,sourceText,options) keys should be identical w.r.t. parsing
    let AreSameForParsing3((fileName1: string, source1: string, options1: FSharpProjectOptions), (fileName2, source2, options2)) =
        (fileName1 = fileName2) 
        && FSharpProjectOptions.AreSameForParsing(options1,options2)
        && (source1 = source2)
        
    /// Determine whether two (fileName,sourceText,options) keys should be identical w.r.t. checking
    let AreSameForChecking3((fileName1: string, source1: string, options1: FSharpProjectOptions), (fileName2, source2, options2)) =
        (fileName1 = fileName2) 
        && FSharpProjectOptions.AreSameForChecking(options1,options2)
        && (source1 = source2)

    /// Determine whether two (fileName,sourceText,options) keys should be identical w.r.t. resource usage
    let AreSubsumable3((fileName1:string,_,o1:FSharpProjectOptions),(fileName2:string,_,o2:FSharpProjectOptions)) =
        (fileName1 = fileName2)
        && FSharpProjectOptions.AreSubsumable(o1,o2)

type FileName = string
type Source = string        
type FilePath = string
type ProjectPath = string
type FileVersion = int

type ParseCacheLockToken() = interface LockToken

// There is only one instance of this type, held in FSharpChecker
type BackgroundCompiler(referenceResolver, projectCacheSize, keepAssemblyContents, keepAllBackgroundResolutions) as self =
    // STATIC ROOT: FSharpLanguageServiceTestable.FSharpChecker.backgroundCompiler.reactor: The one and only Reactor
    let reactor = Reactor.Singleton
    let beforeFileChecked = Event<string * obj option>()
    let fileParsed = Event<string * obj option>()
    let fileChecked = Event<string * obj option>()
    let projectChecked = Event<string * obj option>()

    let mutable implicitlyStartBackgroundWork = true
    let reactorOps = 
        { new IReactorOperations with 
                member __.EnqueueAndAwaitOpAsync (desc, op) = reactor.EnqueueAndAwaitOpAsync (desc, op)
                member __.EnqueueOp (desc, op) = reactor.EnqueueOp (desc, op) }

    // STATIC ROOT: FSharpLanguageServiceTestable.FSharpChecker.backgroundCompiler.scriptClosureCache 
    /// Information about the derived script closure.
    let scriptClosureCache = 
        MruCache<CompilationThreadToken, FSharpProjectOptions, LoadClosure>(projectCacheSize, 
            areSame=FSharpProjectOptions.AreSameForChecking, 
            areSameForSubsumption=FSharpProjectOptions.AreSubsumable)

    let frameworkTcImportsCache = FrameworkImportsCache(frameworkTcImportsCacheStrongSize)

    /// CreateOneIncrementalBuilder (for background type checking). Note that fsc.fs also
    /// creates an incremental builder used by the command line compiler.
    let CreateOneIncrementalBuilder (ctok, options:FSharpProjectOptions, ct) = 

        let projectReferences =  
            [ for (nm,opts) in options.ReferencedProjects ->
                { new IProjectReference with 
                        member x.EvaluateRawContents() = 
                            let r = self.ParseAndCheckProjectImpl(opts, ctok, ct)
                            r.RawFSharpAssemblyData 
                        member x.GetLogicalTimeStamp() = 
                            self.GetLogicalTimeStampForProject(ctok, opts, ct)
                        member x.FileName = nm } ]

        let builderOpt, diagnostics = 
            IncrementalBuilder.TryCreateBackgroundBuilderForProjectOptions
                  (ctok, referenceResolver, frameworkTcImportsCache, scriptClosureCache.TryGet (ctok, options), Array.toList options.ProjectFileNames, 
                   Array.toList options.OtherOptions, projectReferences, options.ProjectDirectory, 
                   options.UseScriptResolutionRules, keepAssemblyContents, keepAllBackgroundResolutions, maxTimeShareMilliseconds)

        // We're putting the builder in the cache, so increment its count.
        let decrement = IncrementalBuilder.KeepBuilderAlive builderOpt

        match builderOpt with 
        | None -> ()
        | Some builder -> 

            // Register the behaviour that responds to CCUs being invalidated because of type
            // provider Invalidate events. This invalidates the configuration in the build.
            builder.ImportedCcusInvalidated.Add (fun _ -> 
                self.InvalidateConfiguration options)

            // Register the callback called just before a file is typechecked by the background builder (without recording
            // errors or intellisense information).
            //
            // This indicates to the UI that the file type check state is dirty. If the file is open and visible then 
            // the UI will sooner or later request a typecheck of the file, recording errors and intellisense information.
            builder.BeforeFileChecked.Add (fun file -> beforeFileChecked.Trigger(file, options.ExtraProjectInfo))
            builder.FileParsed.Add (fun file -> fileParsed.Trigger(file, options.ExtraProjectInfo))
            builder.FileChecked.Add (fun file -> fileChecked.Trigger(file, options.ExtraProjectInfo))
            builder.ProjectChecked.Add (fun () -> projectChecked.Trigger (options.ProjectFileName, options.ExtraProjectInfo))

        (builderOpt, diagnostics, decrement)

    // STATIC ROOT: FSharpLanguageServiceTestable.FSharpChecker.backgroundCompiler.incrementalBuildersCache. This root typically holds more 
    // live information than anything else in the F# Language Service, since it holds up to 3 (projectCacheStrongSize) background project builds
    // strongly.
    // 
    /// Cache of builds keyed by options.        
    let incrementalBuildersCache = 
        MruCache<CompilationThreadToken, FSharpProjectOptions, (IncrementalBuilder option * FSharpErrorInfo list * IDisposable)>
                (keepStrongly=projectCacheSize, keepMax=projectCacheSize, 
                 areSame =  FSharpProjectOptions.AreSameForChecking, 
                 areSameForSubsumption =  FSharpProjectOptions.AreSubsumable,
                 requiredToKeep=(fun (builderOpt,_,_) -> match builderOpt with None -> false | Some b -> b.IsBeingKeptAliveApartFromCacheEntry),
                 onDiscard = (fun (_, _, decrement) -> decrement.Dispose()))

    let getOrCreateBuilder (ctok, options, ct) =
        RequireCompilationThread ctok
        match incrementalBuildersCache.TryGet (ctok, options) with
        | Some b -> b
        | None -> 
            let b = CreateOneIncrementalBuilder (ctok, options, ct)
            incrementalBuildersCache.Set (ctok, options, b)
            b

    let parseCacheLock = Lock<ParseCacheLockToken>()
    

    // STATIC ROOT: FSharpLanguageServiceTestable.FSharpChecker.parseFileInProjectCache. Most recently used cache for parsing files.
    let parseFileInProjectCache = 
        MruCache<ParseCacheLockToken, _, _>(parseFileInProjectCacheSize, 
            areSame=AreSameForParsing3,
            areSameForSubsumption=AreSubsumable3)

    // STATIC ROOT: FSharpLanguageServiceTestable.FSharpChecker.parseAndCheckFileInProjectCachePossiblyStale 
    // STATIC ROOT: FSharpLanguageServiceTestable.FSharpChecker.parseAndCheckFileInProjectCache
    //
    /// Cache which holds recently seen type-checks.
    /// This cache may hold out-of-date entries, in two senses
    ///    - there may be a more recent antecedent state available because the background build has made it available
    ///    - the source for the file may have changed
    
    let parseAndCheckFileInProjectCachePossiblyStale = 
        MruCache<ParseCacheLockToken,string * FSharpProjectOptions, FSharpParseFileResults * FSharpCheckFileResults * int>
            (keepStrongly=incrementalTypeCheckCacheSize,
             areSame=AreSameForChecking2,
             areSameForSubsumption=AreSubsumable2)

    // Also keyed on source. This can only be out of date if the antecedent is out of date
    let parseAndCheckFileInProjectCache = 
        MruCache<ParseCacheLockToken,FileName * Source * FSharpProjectOptions, FSharpParseFileResults * FSharpCheckFileResults * FileVersion * DateTime>
            (keepStrongly=incrementalTypeCheckCacheSize,
             areSame=AreSameForChecking3,
             areSameForSubsumption=AreSubsumable3)

    /// Holds keys for files being currently checked. It's used to prevent checking same file in parallel (interliveing chunck queued to Reactor).
    let beingCheckedFileTable = 
        ConcurrentDictionary<FilePath * FSharpProjectOptions * FileVersion, unit>
            (HashIdentity.FromFunctions
                hash
                (fun (f1, o1, v1) (f2, o2, v2) -> f1 = f2 && v1 = v2 && FSharpProjectOptions.AreSameForChecking(o1, o2)))

    static let mutable foregroundParseCount = 0
    static let mutable foregroundTypeCheckCount = 0

    let MakeCheckFileResultsEmpty(creationErrors) = 
        FSharpCheckFileResults (Array.ofList creationErrors,None, None, reactorOps)

    let MakeCheckFileResults(options:FSharpProjectOptions, builder, scope, creationErrors, parseErrors, tcErrors) = 
        let errors = 
            [| yield! creationErrors 
               yield! parseErrors
               if options.IsIncompleteTypeCheckEnvironment then 
                    yield! Seq.truncate maxTypeCheckErrorsOutOfProjectContext tcErrors
               else 
                    yield! tcErrors |]
                
        FSharpCheckFileResults (errors, Some scope, Some builder, reactorOps)

    let MakeCheckFileAnswer(tcFileResult, options:FSharpProjectOptions, builder, creationErrors, parseErrors, tcErrors) = 
        match tcFileResult with 
        | Parser.TypeCheckAborted.Yes  ->  FSharpCheckFileAnswer.Aborted                
        | Parser.TypeCheckAborted.No scope -> FSharpCheckFileAnswer.Succeeded(MakeCheckFileResults(options, builder, scope, creationErrors, parseErrors, tcErrors))

    member bc.RecordTypeCheckFileInProjectResults(filename,options,parseResults,fileVersion,priorTimeStamp,checkAnswer,source) =        
        match checkAnswer with 
        | None
        | Some FSharpCheckFileAnswer.Aborted -> ()
        | Some (FSharpCheckFileAnswer.Succeeded typedResults) -> 
            foregroundTypeCheckCount <- foregroundTypeCheckCount + 1
            parseCacheLock.AcquireLock (fun ltok -> 
                parseAndCheckFileInProjectCachePossiblyStale.Set(ltok, (filename,options),(parseResults,typedResults,fileVersion))  
                
                Console.WriteLine(sprintf "parseAndCheckFileInProjectCache SET key = %+A" (filename,source,options))
                
                parseAndCheckFileInProjectCache.Set(ltok, (filename,source,options),(parseResults,typedResults,fileVersion,priorTimeStamp))
                parseFileInProjectCache.Set(ltok, (filename,source,options),parseResults))

    member bc.ImplicitlyStartCheckProjectInBackground(options) =        
        if implicitlyStartBackgroundWork then 
            bc.CheckProjectInBackground(options)   

    /// Parses the source file and returns untyped AST
    member bc.ParseFileInProject(filename:string, source,options:FSharpProjectOptions) =
        match parseCacheLock.AcquireLock (fun ctok -> parseFileInProjectCache.TryGet (ctok, (filename, source, options))) with 
        | Some parseResults -> async.Return parseResults
        | None -> 
        // Try this cache too (which might contain different entries)
        let cachedResults = parseCacheLock.AcquireLock (fun ctok -> parseAndCheckFileInProjectCache.TryGet(ctok,(filename,source,options)))
        match cachedResults with 
        | Some (parseResults, _checkResults,_,_) ->  async.Return parseResults
        | _ -> 
        reactor.EnqueueAndAwaitOpAsync("ParseFileInProject " + filename, fun ctok ct -> 
        
            // Try the caches again - it may have been filled by the time this operation runs
            match parseCacheLock.AcquireLock (fun ctok -> parseFileInProjectCache.TryGet (ctok, (filename, source, options))) with 
            | Some parseResults -> parseResults
            | None -> 
            let cachedResults = parseCacheLock.AcquireLock (fun ctok -> parseAndCheckFileInProjectCache.TryGet(ctok, (filename,source,options)))
            match cachedResults with 
            | Some (parseResults, _checkResults,_,_) ->  parseResults
            | _ -> 
            foregroundParseCount <- foregroundParseCount + 1
            let builderOpt,creationErrors,_ = getOrCreateBuilder (ctok, options, ct)
            use _unwind = IncrementalBuilder.KeepBuilderAlive builderOpt
            match builderOpt with
            | None -> FSharpParseFileResults(List.toArray creationErrors, None, true, [])
            | Some builder -> 
            // Do the parsing.
            let parseErrors, _matchPairs, inputOpt, anyErrors = 
               Parser.ParseOneFile (ctok, source, false, true, filename, builder.ProjectFileNames, builder.TcConfig)
                 
            let res = FSharpParseFileResults(parseErrors, inputOpt, anyErrors, builder.Dependencies )
            parseCacheLock.AcquireLock (fun ctok -> parseFileInProjectCache.Set (ctok, (filename, source, options), res))
            res 
        )

    /// Fetch the parse information from the background compiler (which checks w.r.t. the FileSystem API)
    member bc.GetBackgroundParseResultsForFileInProject(filename, options) =
        reactor.EnqueueAndAwaitOpAsync("GetBackgroundParseResultsForFileInProject " + filename, fun ctok ct -> 
            let builderOpt, creationErrors, _ = getOrCreateBuilder (ctok, options, ct)
            use _unwind = IncrementalBuilder.KeepBuilderAlive builderOpt
            match builderOpt with
            | None -> FSharpParseFileResults(List.toArray creationErrors, None, true, [])
            | Some builder -> 
            let inputOpt,_,_,parseErrors = builder.GetParseResultsForFile (ctok, filename, ct)
            let dependencyFiles = builder.Dependencies 
            let errors = [| yield! creationErrors; yield! Parser.CreateErrorInfos (builder.TcConfig, false, filename, parseErrors) |]
            FSharpParseFileResults(errors = errors, input = inputOpt, parseHadErrors = false, dependencyFiles = dependencyFiles)
        )

    member bc.MatchBraces(filename:string, source, options)=
        reactor.EnqueueAndAwaitOpAsync("MatchBraces " + filename, fun ctok ct -> 
            let builderOpt,_,_ = getOrCreateBuilder (ctok, options, ct)
            use _unwind = IncrementalBuilder.KeepBuilderAlive builderOpt
            match builderOpt with
            | None -> [| |]
            | Some builder -> 
            let _parseErrors, matchPairs, _inputOpt, _anyErrors = 
               Parser.ParseOneFile (ctok, source, true, false, filename, builder.ProjectFileNames, builder.TcConfig)
                 
            matchPairs
        )

    member bc.GetCachedCheckFileResult(builder: IncrementalBuilder,filename,source,options) =
            // Check the cache. We can only use cached results when there is no work to do to bring the background builder up-to-date
            let cachedResults = parseCacheLock.AcquireLock (fun ctok -> parseAndCheckFileInProjectCache.TryGet(ctok, (filename,source,options)))

            match cachedResults with 
//            | Some (parseResults, checkResults, _, _) when builder.AreCheckResultsBeforeFileInProjectReady(filename) -> 
            | Some (parseResults, checkResults,_,priorTimeStamp) 
                 when 
                    (match builder.GetCheckResultsBeforeFileInProjectIfReady filename with 
                    | None -> false
                    | Some(tcPrior) -> 
                        tcPrior.TimeStamp = priorTimeStamp &&
                        builder.AreCheckResultsBeforeFileInProjectReady(filename)) -> 
                Some (parseResults,checkResults)
            | _ -> None

    /// 1. Repeatedly try to get cached file check results or get file "lock". 
    /// 
    /// 2. If it've got cached results, returns them.
    ///
    /// 3. If it've not got the lock for 1 munute, returns `FSharpCheckFileAnswer.Aborted`.
    ///
    /// 4. Type checks the file.
    ///
    /// 5. Records results in `BackgroundCompiler` caches.
    ///
    /// 6. Starts whole project background compilation.
    ///
    /// 7. Releases the file "lock".
    member private bc.CheckOneFile
        (parseResults: FSharpParseFileResults,
         source: string,
         fileName: string,
         options: FSharpProjectOptions,
         textSnapshotInfo : obj option,
         fileVersion : int,
         builder : IncrementalBuilder,
         tcPrior : PartialCheckResults,
         creationErrors : FSharpErrorInfo list) = 
    
        async {
            let beingCheckedFileKey = fileName, options, fileVersion
            let stopwatch = Diagnostics.Stopwatch.StartNew()
            let rec loop() =
                async {
                    // results may appear while we were waiting for the lock, let's recheck if it's the case
                    let! cachedResults = 
                        reactor.EnqueueAndAwaitOpAsync(
                            "GetCachedCheckFileResult " + fileName, 
                            (fun ctok _ -> 
                                DoesNotRequireCompilerThreadTokenAndCouldPossiblyBeMadeConcurrent  ctok
                                bc.GetCachedCheckFileResult(builder, fileName, source, options)))
            
                    match cachedResults with
                    | Some (_, checkResults) -> return FSharpCheckFileAnswer.Succeeded checkResults
                    | None ->
                        if beingCheckedFileTable.TryAdd(beingCheckedFileKey, ()) then
                            try
                                // Get additional script #load closure information if applicable.
                                // For scripts, this will have been recorded by GetProjectOptionsFromScript.
                                let! loadClosure = reactor.EnqueueAndAwaitOpAsync("Try get from GetScriptClosureCache", (fun ctok _ -> scriptClosureCache.TryGet (ctok, options)))
                                let! tcErrors, tcFileResult = 
                                    Parser.TypeCheckOneFile(parseResults, source, fileName, options.ProjectFileName, tcPrior.TcConfig, tcPrior.TcGlobals, tcPrior.TcImports, 
                                                            tcPrior.TcState, loadClosure, tcPrior.Errors, reactorOps, (fun () -> builder.IsAlive), textSnapshotInfo)
                                let checkAnswer = MakeCheckFileAnswer(tcFileResult, options, builder, creationErrors, parseResults.Errors, tcErrors)
                                bc.RecordTypeCheckFileInProjectResults(fileName, options, parseResults, fileVersion, tcPrior.TimeStamp, Some checkAnswer, source)
                                return checkAnswer
                            finally
                                let dummy = ref ()
                                beingCheckedFileTable.TryRemove(beingCheckedFileKey, dummy) |> ignore
                        else 
                            do! Async.Sleep 100
                            if stopwatch.Elapsed > TimeSpan.FromMinutes 1. then 
                                return FSharpCheckFileAnswer.Aborted
                            else
                                return! loop()
                }
            return! loop()
        }

    /// Type-check the result obtained by parsing, but only if the antecedent type checking context is available. 
    member bc.CheckFileInProjectIfReady(parseResults: FSharpParseFileResults, filename, fileVersion, source, options, textSnapshotInfo: obj option) =
        let execWithReactorAsync action = reactor.EnqueueAndAwaitOpAsync("CheckFileInProjectIfReady " + filename, action)
        async {
            try
                let! cachedResults = execWithReactorAsync <| fun ctok _ ->
                    match incrementalBuildersCache.TryGetAny (ctok, options) with
                    | Some (Some builder, creationErrors, _) ->
                        match bc.GetCachedCheckFileResult(builder, filename, source, options) with
                        | Some (_, checkResults) -> Some (builder, creationErrors, Some (FSharpCheckFileAnswer.Succeeded checkResults))
                        | _ -> Some (builder, creationErrors, None)
                    | _ -> None // the builder wasn't ready
                        
                match cachedResults with
                | None -> return None
                | Some (_, _, Some x) -> return Some x
                | Some (builder, creationErrors, None) ->
                    let! tcPrior = 
                        execWithReactorAsync <| fun ctok _ -> 
                            DoesNotRequireCompilerThreadTokenAndCouldPossiblyBeMadeConcurrent  ctok
                            builder.GetCheckResultsBeforeFileInProjectIfReady filename
                    match tcPrior with
                    | Some tcPrior -> 
                        let! checkResults = bc.CheckOneFile(parseResults, source, filename, options, textSnapshotInfo, fileVersion, builder, tcPrior, creationErrors)
                        return Some checkResults
                    | None -> return None  // the incremental builder was not up to date
            finally 
                bc.ImplicitlyStartCheckProjectInBackground(options)
        }

    /// Type-check the result obtained by parsing. Force the evaluation of the antecedent type checking context if needed.
    member bc.CheckFileInProject(parseResults: FSharpParseFileResults, filename, fileVersion, source, options, textSnapshotInfo) =
        let execWithReactorAsync action = reactor.EnqueueAndAwaitOpAsync("CheckFileInProject " + filename, action)
        async {
            let! ct = Async.CancellationToken
            let! builderOpt,creationErrors,_ = execWithReactorAsync <| fun ctok _ -> getOrCreateBuilder (ctok, options, ct) // Q: Whis it it ok to ignore creationErrors in the build cache? A: These errors will be appended into the typecheck results
            use _unwind = IncrementalBuilder.KeepBuilderAlive builderOpt
            match builderOpt with
            | None -> return FSharpCheckFileAnswer.Succeeded (MakeCheckFileResultsEmpty(creationErrors))
            | Some builder -> 
                // Check the cache. We can only use cached results when there is no work to do to bring the background builder up-to-date
                let! cachedResults = 
                    execWithReactorAsync <| fun ctok _ -> 
                        DoesNotRequireCompilerThreadTokenAndCouldPossiblyBeMadeConcurrent  ctok
                        bc.GetCachedCheckFileResult(builder, filename, source, options)

                match cachedResults with
                | Some (_, checkResults) -> return FSharpCheckFileAnswer.Succeeded checkResults
                | _ ->
                    let! tcPrior = execWithReactorAsync <| fun ctok _ -> builder.GetCheckResultsBeforeFileInProject (ctok, filename, ct)
                    let! checkAnswer = bc.CheckOneFile(parseResults, source, filename, options, textSnapshotInfo, fileVersion, builder, tcPrior, creationErrors)
                    bc.ImplicitlyStartCheckProjectInBackground(options)
                    return checkAnswer
        }

    /// Parses and checks the source file and returns untyped AST and check results.
    member bc.ParseAndCheckFileInProject(filename:string, fileVersion, source, options:FSharpProjectOptions,textSnapshotInfo) =
        let execWithReactorAsync action = reactor.EnqueueAndAwaitOpAsync("ParseAndCheckFileInProject " + filename, action)
        async {
            let! ct = Async.CancellationToken
            let! builderOpt,creationErrors,_ = execWithReactorAsync <| fun ctok _ -> getOrCreateBuilder (ctok, options, ct) // Q: Whis it it ok to ignore creationErrors in the build cache? A: These errors will be appended into the typecheck results
            use _unwind = IncrementalBuilder.KeepBuilderAlive builderOpt
            match builderOpt with
            | None -> 
                let parseResults = FSharpParseFileResults(List.toArray creationErrors, None, true, [])
                return (parseResults, FSharpCheckFileAnswer.Aborted)

            | Some builder -> 
                let! cachedResults = 
                    execWithReactorAsync <| fun ctok _ -> 
                        DoesNotRequireCompilerThreadTokenAndCouldPossiblyBeMadeConcurrent  ctok
                        bc.GetCachedCheckFileResult(builder, filename, source, options)

                match cachedResults with 
                | Some (parseResults, checkResults) -> return parseResults, FSharpCheckFileAnswer.Succeeded checkResults
                | _ ->
                    // todo this blocks the Reactor queue until all files up to the current are type checked. It's OK while editing the file,
                    // but results with non cooperative blocking when a firts file from a project opened.
                    let! tcPrior = execWithReactorAsync <| fun ctok _ -> builder.GetCheckResultsBeforeFileInProject (ctok, filename, ct)
                    
                    // Do the parsing.
                    let! parseErrors, _matchPairs, inputOpt, anyErrors = 
                        execWithReactorAsync <| fun ctok _ ->
                            Parser.ParseOneFile (ctok, source, false, true, filename, builder.ProjectFileNames, builder.TcConfig)
                     
                    let parseResults = FSharpParseFileResults(parseErrors, inputOpt, anyErrors, builder.Dependencies)
                    let! checkResults = bc.CheckOneFile(parseResults, source, filename, options, textSnapshotInfo, fileVersion, builder, tcPrior, creationErrors)
                    bc.ImplicitlyStartCheckProjectInBackground(options)
                    return parseResults, checkResults
        }

    /// Fetch the check information from the background compiler (which checks w.r.t. the FileSystem API)
    member bc.GetBackgroundCheckResultsForFileInProject(filename,options) =
        reactor.EnqueueAndAwaitOpAsync("GetBackgroundCheckResultsForFileInProject " + filename, fun ctok ct -> 
            let (builderOpt, creationErrors, _) = getOrCreateBuilder (ctok, options, ct)
            use _unwind = IncrementalBuilder.KeepBuilderAlive builderOpt
            match builderOpt with
            | None -> 
                let parseResults = FSharpParseFileResults(Array.ofList creationErrors, None, true, [])
                let typedResults = MakeCheckFileResultsEmpty(creationErrors)
                (parseResults, typedResults)
            | Some builder -> 
                let (inputOpt, _, _, untypedErrors) = builder.GetParseResultsForFile (ctok, filename, ct)
                let tcProj = builder.GetCheckResultsAfterFileInProject (ctok, filename, ct)
                let untypedErrors = [| yield! creationErrors; yield! Parser.CreateErrorInfos (builder.TcConfig, false, filename, untypedErrors) |]
                let tcErrors = [| yield! creationErrors; yield! Parser.CreateErrorInfos (builder.TcConfig, false, filename, tcProj.Errors) |]
                let parseResults = FSharpParseFileResults(errors = untypedErrors, input = inputOpt, parseHadErrors = false, dependencyFiles = builder.Dependencies)
                let loadClosure = scriptClosureCache.TryGet (ctok, options) 
                let scope = 
                    TypeCheckInfo(tcProj.TcConfig, tcProj.TcGlobals, tcProj.TcState.PartialAssemblySignature, tcProj.TcState.Ccu, tcProj.TcImports, tcProj.TcEnvAtEnd.AccessRights,
                                  options.ProjectFileName, filename, 
                                  List.last tcProj.TcResolutions, 
                                  List.last tcProj.TcSymbolUses,
                                  tcProj.TcEnvAtEnd.NameEnv,
                                  loadClosure, reactorOps, (fun () -> builder.IsAlive), None)     
                let typedResults = MakeCheckFileResults(options, builder, scope, creationErrors, parseResults.Errors, tcErrors)
                (parseResults, typedResults)
            )


    /// Try to get recent approximate type check results for a file. 
    member bc.TryGetRecentCheckResultsForFile(filename: string, options:FSharpProjectOptions, source) =
        match source with 
        | Some sourceText -> 
            parseCacheLock.AcquireLock (fun ctok -> 
                match parseAndCheckFileInProjectCache.TryGet(ctok,(filename,sourceText,options)) with
                | Some (a,b,c,_) -> Some (a,b,c)
                | None -> None)
        | None -> parseCacheLock.AcquireLock (fun ctok -> parseAndCheckFileInProjectCachePossiblyStale.TryGet(ctok,(filename,options)))

    /// Parse and typecheck the whole project (the implementation, called recursively as project graph is evaluated)
    member private bc.ParseAndCheckProjectImpl(options, ctok, ct) : FSharpCheckProjectResults =
        let builderOpt,creationErrors,_ = getOrCreateBuilder (ctok, options, ct)
        use _unwind = IncrementalBuilder.KeepBuilderAlive builderOpt
        match builderOpt with 
        | None -> 
            FSharpCheckProjectResults (keepAssemblyContents, Array.ofList creationErrors, None, reactorOps)
        | Some builder -> 
            let (tcProj, ilAssemRef, tcAssemblyDataOpt, tcAssemblyExprOpt)  = builder.GetCheckResultsAndImplementationsForProject(ctok, ct)
            let errors = [| yield! creationErrors; yield! Parser.CreateErrorInfos (tcProj.TcConfig, true, Microsoft.FSharp.Compiler.TcGlobals.DummyFileNameForRangesWithoutASpecificLocation, tcProj.Errors) |]
            FSharpCheckProjectResults (keepAssemblyContents, errors, Some(tcProj.TcGlobals, tcProj.TcImports, tcProj.TcState.Ccu, tcProj.TcState.PartialAssemblySignature, tcProj.TcSymbolUses, tcProj.TopAttribs, tcAssemblyDataOpt, ilAssemRef, tcProj.TcEnvAtEnd.AccessRights, tcAssemblyExprOpt), reactorOps)

    /// Get the timestamp that would be on the output if fully built immediately
    member private bc.GetLogicalTimeStampForProject(ctok, options, ct) =
        let builderOpt,_creationErrors,_ = getOrCreateBuilder (ctok, options, ct)
        use _unwind = IncrementalBuilder.KeepBuilderAlive builderOpt
        match builderOpt with 
        | None -> None
        | Some builder -> Some (builder.GetLogicalTimeStampForProject(ctok))

    /// Keep the projet builder alive over a scope
    member bc.KeepProjectAlive(options) =
        reactor.EnqueueAndAwaitOpAsync("KeepProjectAlive " + options.ProjectFileName, fun ctok ct -> 
            let builderOpt,_creationErrors,_ = getOrCreateBuilder (ctok, options, ct)
            // This increments, and lets the caller decrement
            IncrementalBuilder.KeepBuilderAlive builderOpt)

    /// Parse and typecheck the whole project.
    member bc.ParseAndCheckProject(options) =
        reactor.EnqueueAndAwaitOpAsync("ParseAndCheckProject " + options.ProjectFileName, fun ctok ct -> bc.ParseAndCheckProjectImpl(options, ctok, ct))

    member bc.GetProjectOptionsFromScript(filename, source, ?loadedTimeStamp, ?otherFlags, ?useFsiAuxLib, ?assumeDotNetFramework, ?extraProjectInfo: obj) = 
        reactor.EnqueueAndAwaitOpAsync ("GetProjectOptionsFromScript " + filename, fun ctok _ct -> 
            // Do we add a reference to FSharp.Compiler.Interactive.Settings by default?
            let useFsiAuxLib = defaultArg useFsiAuxLib true
            // Do we assume .NET Framework references for scripts?
            let assumeDotNetFramework = defaultArg assumeDotNetFramework true
            let otherFlags = defaultArg otherFlags [| |]
            let useSimpleResolution = 
#if ENABLE_MONO_SUPPORT
                runningOnMono || otherFlags |> Array.exists (fun x -> x = "--simpleresolution")
#else
                true
#endif
            let loadedTimeStamp = defaultArg loadedTimeStamp DateTime.MaxValue // Not 'now', we don't want to force reloading
            let applyCompilerOptions tcConfigB  = 
                let collect _name = ()
                let fsiCompilerOptions = CompileOptions.GetCoreFsiCompilerOptions tcConfigB 
                CompileOptions.ParseCompilerOptions (collect, fsiCompilerOptions, Array.toList otherFlags)
            let loadClosure = LoadClosure.ComputeClosureOfSourceText(ctok, referenceResolver,filename, source, CodeContext.Editing, useSimpleResolution, useFsiAuxLib, new Lexhelp.LexResourceManager(), applyCompilerOptions, assumeDotNetFramework)
            let otherFlags = 
                [| yield "--noframework"; yield "--warn:3"; 
                   yield! otherFlags 
                   for r in loadClosure.References do yield "-r:" + fst r
                   for (code,_) in loadClosure.NoWarns do yield "--nowarn:" + code
                |]
            let options = 
                {
                    ProjectFileName = filename + ".fsproj" // Make a name that is unique in this directory.
                    ProjectFileNames = loadClosure.SourceFiles |> List.map fst |> List.toArray
                    OtherOptions = otherFlags 
                    ReferencedProjects= [| |]  
                    IsIncompleteTypeCheckEnvironment = false
                    UseScriptResolutionRules = true 
                    LoadTime = loadedTimeStamp
                    UnresolvedReferences = Some (UnresolvedReferencesSet(loadClosure.UnresolvedReferences))
                    OriginalLoadReferences = loadClosure.OriginalLoadReferences
                    ExtraProjectInfo=extraProjectInfo
                }
            scriptClosureCache.Set(ctok, options,loadClosure) // Save the full load closure for later correlation.
            options)
            
    member bc.InvalidateConfiguration(options : FSharpProjectOptions) =
        reactor.EnqueueOp("InvalidateConfiguration", fun ctok -> 
            // This operation can't currently be cancelled and is not async
            let ct = CancellationToken.None
            match incrementalBuildersCache.TryGetAny (ctok, options) with
            | None -> ()
            | Some (_oldBuilder, _, _) ->
                    // We do not need to decrement here - the onDiscard function is called each time an entry is pushed out of the build cache,
                    // including by SetAlternate.
                    let builderB, errorsB, decrementB = CreateOneIncrementalBuilder (ctok, options, ct)
                    incrementalBuildersCache.Set(ctok, options, (builderB, errorsB, decrementB))
            if implicitlyStartBackgroundWork then 
               bc.CheckProjectInBackground(options))

    member bc.NotifyProjectCleaned (options : FSharpProjectOptions) =
        reactor.EnqueueAndAwaitOpAsync("NotifyProjectCleaned", fun ctok _ct -> 
            match incrementalBuildersCache.TryGetAny (ctok, options) with
            | None -> ()
            | Some (builderOpt, _, _) ->
#if EXTENSIONTYPING
                builderOpt |> Option.iter (fun builder -> 
                    if builder.ThereAreLiveTypeProviders then
                        bc.InvalidateConfiguration(options))
#else
                ()
#endif
        )

    member bc.CheckProjectInBackground (options) =
        reactor.SetBackgroundOp (Some (fun ctok -> 
            // The individual steps of the background build can't currently be cancelled
            let ct = CancellationToken.None
            let builderOpt,_,_ = getOrCreateBuilder (ctok, options, ct)
            use _unwind = IncrementalBuilder.KeepBuilderAlive builderOpt
            match builderOpt with 
            | None -> false
            | Some builder -> builder.Step(ctok, ct)))

    member bc.StopBackgroundCompile   () =
        reactor.SetBackgroundOp(None)

    member bc.WaitForBackgroundCompile() =
        reactor.WaitForBackgroundOpCompletion() 

    member bc.CompleteAllQueuedOps() =
        reactor.CompleteAllQueuedOps() 

    member bc.ReactorOps  = reactorOps
    member bc.BeforeBackgroundFileCheck = beforeFileChecked.Publish
    member bc.FileParsed = fileParsed.Publish
    member bc.FileChecked = fileChecked.Publish
    member bc.ProjectChecked = projectChecked.Publish

    member bc.CurrentQueueLength = reactor.CurrentQueueLength

    member bc.ClearCachesAsync () =
        reactor.EnqueueAndAwaitOpAsync ("ClearCachesAsync", fun ctok _ct -> 
            parseCacheLock.AcquireLock (fun ltok -> 
                parseAndCheckFileInProjectCachePossiblyStale.Clear ltok
                parseAndCheckFileInProjectCache.Clear ltok
                parseFileInProjectCache.Clear ltok)
            incrementalBuildersCache.Clear ctok
            frameworkTcImportsCache.Clear ctok
            scriptClosureCache.Clear ctok)

    member bc.DownsizeCaches() =
        reactor.EnqueueAndAwaitOpAsync ("DownsizeCaches", fun ctok _ct -> 
            parseCacheLock.AcquireLock (fun ltok -> 
                parseAndCheckFileInProjectCachePossiblyStale.Resize(ltok, keepStrongly=1)
                parseAndCheckFileInProjectCache.Resize(ltok, keepStrongly=1)
                parseFileInProjectCache.Resize(ltok, keepStrongly=1))
            incrementalBuildersCache.Resize(ctok, keepStrongly=1, keepMax=1)
            frameworkTcImportsCache.Downsize(ctok)
            scriptClosureCache.Resize(ctok,keepStrongly=1, keepMax=1))
         
    member __.FrameworkImportsCache = frameworkTcImportsCache
    member __.ImplicitlyStartBackgroundWork with get() = implicitlyStartBackgroundWork and set v = implicitlyStartBackgroundWork <- v
    static member GlobalForegroundParseCountStatistic = foregroundParseCount
    static member GlobalForegroundTypeCheckCountStatistic = foregroundTypeCheckCount

//----------------------------------------------------------------------------
// FSharpChecker
//

[<Sealed>]
[<AutoSerializable(false)>]
// There is typically only one instance of this type in a Visual Studio process.
type FSharpChecker(referenceResolver, projectCacheSize, keepAssemblyContents, keepAllBackgroundResolutions) =

    let backgroundCompiler = BackgroundCompiler(referenceResolver, projectCacheSize, keepAssemblyContents, keepAllBackgroundResolutions)

    static let globalInstance = FSharpChecker.Create()
        
    // Parse using backgroundCompiler
    let ComputeBraceMatching(filename:string,source,options:FSharpProjectOptions) = 
        backgroundCompiler.MatchBraces(filename,source,options)
    
    // STATIC ROOT: FSharpLanguageServiceTestable.FSharpChecker.braceMatchCache. Most recently used cache for brace matching. Accessed on the
    // background UI thread, not on the compiler thread.
    //
    // This cache is safe for concurrent access because there is no onDiscard action for the items in the cache.
    let braceMatchCache = 
        MruCache<AnyCallerThreadToken, (string*string*FSharpProjectOptions),_>(braceMatchCacheSize,
            areSame=AreSameForParsing3,
            areSameForSubsumption=AreSubsumable3) 

    /// Instantiate an interactive checker.    
    static member Create(?projectCacheSize, ?keepAssemblyContents, ?keepAllBackgroundResolutions) = 
        let referenceResolver = MSBuildReferenceResolver.Resolver 
        let keepAssemblyContents = defaultArg keepAssemblyContents false
        let keepAllBackgroundResolutions = defaultArg keepAllBackgroundResolutions true
        let projectCacheSizeReal = defaultArg projectCacheSize projectCacheSizeDefault
        new FSharpChecker(referenceResolver, projectCacheSizeReal,keepAssemblyContents, keepAllBackgroundResolutions)

    member ic.MatchBracesAlternate(filename, source, options) =
        async { 
            match braceMatchCache.TryGet (AssumeAnyCallerThreadWithoutEvidence(), (filename, source, options)) with 
            | Some res -> return res
            | None -> 
                let! res = ComputeBraceMatching (filename, source, options)
                braceMatchCache.Set (AssumeAnyCallerThreadWithoutEvidence(), (filename, source, options), res)
                return res 
         }

    member ic.ParseFileInProject(filename, source, options) =
        backgroundCompiler.ParseFileInProject(filename, source, options)
        
    member ic.GetBackgroundParseResultsForFileInProject (filename,options) =
        backgroundCompiler.GetBackgroundParseResultsForFileInProject(filename,options)
        
    member ic.GetBackgroundCheckResultsForFileInProject (filename,options) =
        backgroundCompiler.GetBackgroundCheckResultsForFileInProject(filename,options)
        
    /// Try to get recent approximate type check results for a file. 
    member ic.TryGetRecentCheckResultsForFile(filename: string, options:FSharpProjectOptions, ?source) =
        backgroundCompiler.TryGetRecentCheckResultsForFile(filename,options,source)

    /// This function is called when the entire environment is known to have changed for reasons not encoded in the ProjectOptions of any project/compilation.
    /// For example, the type provider approvals file may have changed.
    member ic.InvalidateAll() =
        ic.ClearCaches()
            
    member ic.ClearCachesAsync() =
        let utok = AssumeAnyCallerThreadWithoutEvidence()
        braceMatchCache.Clear(utok)
        backgroundCompiler.ClearCachesAsync() 

    member ic.ClearCaches() =
        ic.ClearCachesAsync() |> Async.Start // this cache clearance is not synchronous, it will happen when the background op gets run

    /// This function is called when the entire environment is known to have changed for reasons not encoded in the ProjectOptions of any project/compilation.
    /// For example, the type provider approvals file may have changed.
    //
    // This is for unit testing only
    member ic.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients() =
        backgroundCompiler.CompleteAllQueuedOps() // flush AsyncOp
        ic.ClearCachesAsync() |> Async.RunSynchronously
        System.GC.Collect()
        System.GC.WaitForPendingFinalizers() 
        backgroundCompiler.CompleteAllQueuedOps() // flush AsyncOp
            
    /// This function is called when the configuration is known to have changed for reasons not encoded in the ProjectOptions.
    /// For example, dependent references may have been deleted or created.
    member ic.InvalidateConfiguration(options: FSharpProjectOptions) =
        backgroundCompiler.InvalidateConfiguration options

    /// This function is called when a project has been cleaned, and thus type providers should be refreshed.
    member ic.NotifyProjectCleaned(options: FSharpProjectOptions) =
        backgroundCompiler.NotifyProjectCleaned options
              
    /// Typecheck a source code file, returning a handle to the results of the 
    /// parse including the reconstructed types in the file.
    member ic.CheckFileInProjectIfReady(parseResults:FSharpParseFileResults, filename:string, fileVersion:int, source:string, options:FSharpProjectOptions,  ?textSnapshotInfo:obj) =        
        backgroundCompiler.CheckFileInProjectIfReady(parseResults,filename,fileVersion,source,options,textSnapshotInfo)
            
    /// Typecheck a source code file, returning a handle to the results of the 
    /// parse including the reconstructed types in the file.
    member ic.CheckFileInProject(parseResults:FSharpParseFileResults, filename:string, fileVersion:int, source:string, options:FSharpProjectOptions, ?textSnapshotInfo:obj) =        
        backgroundCompiler.CheckFileInProject(parseResults,filename,fileVersion,source,options,textSnapshotInfo)
            
    /// Typecheck a source code file, returning a handle to the results of the 
    /// parse including the reconstructed types in the file.
    member ic.ParseAndCheckFileInProject(filename:string, fileVersion:int, source:string, options:FSharpProjectOptions, ?textSnapshotInfo:obj) =        
        backgroundCompiler.ParseAndCheckFileInProject(filename, fileVersion, source, options, textSnapshotInfo)
            
    member ic.ParseAndCheckProject(options) =
        backgroundCompiler.ParseAndCheckProject(options)

    member ic.KeepProjectAlive(options) =
        backgroundCompiler.KeepProjectAlive(options)

    /// For a given script file, get the ProjectOptions implied by the #load closure
    member ic.GetProjectOptionsFromScript(filename, source, ?loadedTimeStamp, ?otherFlags, ?useFsiAuxLib, ?extraProjectInfo: obj) = 
        backgroundCompiler.GetProjectOptionsFromScript(filename,source,?loadedTimeStamp=loadedTimeStamp, ?otherFlags=otherFlags, ?useFsiAuxLib=useFsiAuxLib, ?extraProjectInfo=extraProjectInfo)
        
    member ic.GetProjectOptionsFromCommandLineArgs(projectFileName, argv, ?loadedTimeStamp, ?extraProjectInfo: obj) = 
        let loadedTimeStamp = defaultArg loadedTimeStamp DateTime.MaxValue // Not 'now', we don't want to force reloading
        { ProjectFileName = projectFileName
          ProjectFileNames = [| |] // the project file names will be inferred from the ProjectOptions
          OtherOptions = argv 
          ReferencedProjects= [| |]  
          IsIncompleteTypeCheckEnvironment = false
          UseScriptResolutionRules = false
          LoadTime = loadedTimeStamp
          UnresolvedReferences = None
          OriginalLoadReferences=[]
          ExtraProjectInfo=extraProjectInfo }

    /// Begin background parsing the given project.
    member ic.StartBackgroundCompile(options) = backgroundCompiler.CheckProjectInBackground(options) 

    /// Begin background parsing the given project.
    member ic.CheckProjectInBackground(options) = backgroundCompiler.CheckProjectInBackground(options) 

    /// Stop the background compile.
    member ic.StopBackgroundCompile() = backgroundCompiler.StopBackgroundCompile()

    /// Block until the background compile finishes.
    //
    // This is for unit testing only
    member ic.WaitForBackgroundCompile() = backgroundCompiler.WaitForBackgroundCompile()

    // Publish the ReactorOps from the background compiler for internal use
    member ic.ReactorOps = backgroundCompiler.ReactorOps
    member ic.CurrentQueueLength = backgroundCompiler.CurrentQueueLength


    member ic.BeforeBackgroundFileCheck  = backgroundCompiler.BeforeBackgroundFileCheck
    member ic.FileParsed  = backgroundCompiler.FileParsed
    member ic.FileChecked  = backgroundCompiler.FileChecked
    member ic.ProjectChecked = backgroundCompiler.ProjectChecked
    member ic.ImplicitlyStartBackgroundWork with get() = backgroundCompiler.ImplicitlyStartBackgroundWork and set v = backgroundCompiler.ImplicitlyStartBackgroundWork <- v
    member ic.PauseBeforeBackgroundWork with get() = Reactor.Singleton.PauseBeforeBackgroundWork and set v = Reactor.Singleton.PauseBeforeBackgroundWork <- v

    static member GlobalForegroundParseCountStatistic = BackgroundCompiler.GlobalForegroundParseCountStatistic
    static member GlobalForegroundTypeCheckCountStatistic = BackgroundCompiler.GlobalForegroundTypeCheckCountStatistic
          
    // Obsolete
    member ic.MatchBraces(filename, source, options) =
        ic.MatchBracesAlternate(filename, source, options) 
        |> Async.RunSynchronously
        |> Array.map (fun (a,b) -> Range.toZ a, Range.toZ b)

    member bc.ParseFile(filename, source, options) = 
        bc.ParseFileInProject(filename, source, options) 
        |> Async.RunSynchronously

    member bc.TypeCheckSource(parseResults, filename, fileVersion, source, options, textSnapshotInfo:obj) = 
        bc.CheckFileInProjectIfReady(parseResults, filename, fileVersion, source, options, textSnapshotInfo)
        |> Async.RunSynchronously

    member ic.GetCheckOptionsFromScriptRoot(filename, source, loadedTimeStamp) = 
        ic.GetProjectOptionsFromScript(filename, source, loadedTimeStamp, [| |]) 
        |> Async.RunSynchronously

    member ic.GetCheckOptionsFromScriptRoot(filename, source, loadedTimeStamp, otherFlags) = 
        ic.GetProjectOptionsFromScript(filename, source, loadedTimeStamp, otherFlags) 
        |> Async.RunSynchronously

    member ic.GetProjectOptionsFromScriptRoot(filename, source, ?loadedTimeStamp, ?otherFlags, ?useFsiAuxLib) = 
        ic.GetProjectOptionsFromScript(filename, source, ?loadedTimeStamp=loadedTimeStamp, ?otherFlags=otherFlags, ?useFsiAuxLib=useFsiAuxLib)
        |> Async.RunSynchronously

    member ic.FileTypeCheckStateIsDirty  = backgroundCompiler.BeforeBackgroundFileCheck

    static member Instance = globalInstance
    member internal __.FrameworkImportsCache = backgroundCompiler.FrameworkImportsCache

type FsiInteractiveChecker(reactorOps: IReactorOperations, tcConfig, tcGlobals, tcImports, tcState, loadClosure) =
    let keepAssemblyContents = false

    static member CreateErrorInfos (tcConfig, allErrors, mainInputFileName, errors) = 
        Parser.CreateErrorInfos(tcConfig, allErrors, mainInputFileName, errors)

    member __.ParseAndCheckInteraction (ctok, source) =
        async {
            let mainInputFileName = "stdin.fsx" 
            // Note: projectSourceFiles is only used to compute isLastCompiland, and is ignored if Build.IsScript(mainInputFileName) is true (which it is in this case).
            let projectSourceFiles = [ ]
            let parseErrors, _matchPairs, inputOpt, anyErrors = Parser.ParseOneFile (ctok, source, false, true, mainInputFileName, projectSourceFiles, tcConfig)
            let dependencyFiles = [] // interactions have no dependencies
            let parseResults = FSharpParseFileResults(parseErrors, inputOpt, parseHadErrors = anyErrors, dependencyFiles = dependencyFiles)
            
            let backgroundDiagnostics = []
            
            let! tcErrors, tcFileResult = 
                Parser.TypeCheckOneFile(parseResults,source,mainInputFileName,"project",tcConfig,tcGlobals,tcImports,  tcState,
                                        loadClosure,backgroundDiagnostics,reactorOps,(fun () -> true),None)
            
            return 
                match tcFileResult with 
                | Parser.TypeCheckAborted.No scope ->
                    let errors = [|  yield! parseErrors; yield! tcErrors |]
                    let typeCheckResults = FSharpCheckFileResults (errors,Some scope, None, reactorOps)   
                    let projectResults = FSharpCheckProjectResults (keepAssemblyContents, errors, Some(tcGlobals, tcImports, scope.ThisCcu, scope.CcuSig, [scope.ScopeSymbolUses], None, None, mkSimpleAssRef "stdin", tcState.TcEnvFromImpls.AccessRights, None), reactorOps)
                    parseResults, typeCheckResults, projectResults
                | _ -> 
                    failwith "unexpected aborted"
        }
                
//----------------------------------------------------------------------------
// CompilerEnvironment, DebuggerEnvironment
//

/// Information about the compilation environment
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module CompilerEnvironment =
    /// These are the names of assemblies that should be referenced for .fs, .ml, .fsi, .mli files that
    /// are not asscociated with a project
    let DefaultReferencesForOrphanSources(assumeDotNetFramework) = DefaultReferencesForScriptsAndOutOfProjectSources(assumeDotNetFramework)
    
    /// Publish compiler-flags parsing logic. Must be fast because its used by the colorizer.
    let GetCompilationDefinesForEditing(filename:string, compilerFlags : string list) =
        let defines = ref(SourceFileImpl.AdditionalDefinesForUseInEditor(filename))
        let MatchAndExtract(flag:string,prefix:string) =
            if flag.StartsWith(prefix) then 
                let sub = flag.Substring(prefix.Length)
                let trimmed = sub.Trim()
                defines := trimmed :: !defines
        let rec QuickParseDefines = function
            | hd :: tail ->
               MatchAndExtract(hd,"-d:")
               MatchAndExtract(hd,"--define:")
               QuickParseDefines tail
            | _ -> ()
        QuickParseDefines compilerFlags
        !defines
            
    /// Return true if this is a subcategory of error or warning message that the language service can emit
    let IsCheckerSupportedSubcategory(subcategory:string) =
        // Beware: This code logic is duplicated in DocumentTask.cs in the language service
        PhasedDiagnostic.IsSubcategoryOfCompile(subcategory)

/// Information about the debugging environment
module DebuggerEnvironment =
    /// Return the language ID, which is the expression evaluator id that the
    /// debugger will use.
    let GetLanguageID() =
        System.Guid(0xAB4F38C9u, 0xB6E6us, 0x43baus, 0xBEuy, 0x3Buy, 0x58uy, 0x08uy, 0x0Buy, 0x2Cuy, 0xCCuy, 0xE3uy)

module PrettyNaming =
    let IsIdentifierPartCharacter     x = Microsoft.FSharp.Compiler.PrettyNaming.IsIdentifierPartCharacter x
    let IsLongIdentifierPartCharacter x = Microsoft.FSharp.Compiler.PrettyNaming.IsLongIdentifierPartCharacter x
    let GetLongNameFromString         x = Microsoft.FSharp.Compiler.PrettyNaming.SplitNamesForILPath x
    let FormatAndOtherOverloadsString remainingOverloads = FSComp.SR.typeInfoOtherOverloads(remainingOverloads)
    let QuoteIdentifierIfNeeded id = Lexhelp.Keywords.QuoteIdentifierIfNeeded id
    let KeywordNames = Lexhelp.Keywords.keywordNames

