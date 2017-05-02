// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Open up the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.
//--------------------------------------------------------------------------

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open System
open System.Collections.Generic
open System.IO

open Microsoft.FSharp.Core.Printf
open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.AbstractIL.IL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library  
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics 

open Microsoft.FSharp.Compiler.AccessibilityLogic
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.Layout
open Microsoft.FSharp.Compiler.Layout.TaggedTextOps
open Microsoft.FSharp.Compiler.Lib
open Microsoft.FSharp.Compiler.PrettyNaming
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.Tastops
open Microsoft.FSharp.Compiler.TcGlobals 
open Microsoft.FSharp.Compiler.Infos
open Microsoft.FSharp.Compiler.NameResolution
open Microsoft.FSharp.Compiler.InfoReader

type internal Layout = layout

module EnvMisc2 =
    let maxMembers = GetEnvInteger "FCS_MaxMembersInQuickInfo" 10

    /// dataTipSpinWaitTime limits how long we block the UI thread while a tooltip pops up next to a selected item in an IntelliSense completion list.
    /// This time appears to be somewhat amortized by the time it takes the VS completion UI to actually bring up the tooltip after selecting an item in the first place.
    let dataTipSpinWaitTime = GetEnvInteger "FCS_ToolTipSpinWaitTime" 300

//----------------------------------------------------------------------------
// Display characteristics of typechecking items
//--------------------------------------------------------------------------

/// Interface that defines methods for comparing objects using partial equality relation
type IPartialEqualityComparer<'T> = 
    inherit IEqualityComparer<'T>
    /// Can the specified object be tested for equality?
    abstract InEqualityRelation : 'T -> bool

module IPartialEqualityComparer = 
    let On f (c: IPartialEqualityComparer<_>) = 
          { new IPartialEqualityComparer<_> with 
                member __.InEqualityRelation x = c.InEqualityRelation (f x)
                member __.Equals(x, y) = c.Equals(f x, f y)
                member __.GetHashCode x = c.GetHashCode(f x) }
    


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
    static member Create(layout:'T,xml,?typeMapping,?paramName,?remarks) = 
        { MainDescription=layout; XmlDoc=xml; TypeMapping=defaultArg typeMapping []; ParamName=paramName; Remarks=remarks }

/// A single data tip display element
[<RequireQualifiedAccess>]
type FSharpToolTipElement<'T> = 
    | None

    /// A single type, method, etc with comment. May represent a method overload group.
    | Group of FSharpToolTipElementData<'T> list

    /// An error occurred formatting this element
    | CompositionError of string

    static member Single(layout,xml,?typeMapping,?paramName,?remarks) = 
        Group [ FSharpToolTipElementData<'T>.Create(layout,xml,?typeMapping=typeMapping,?paramName=paramName,?remarks=remarks) ]

/// A single data tip display element with where text is expressed as string
type FSharpToolTipElement = FSharpToolTipElement<string>

/// A single data tip display element with where text is expressed as <see cref="Layout"/>
type internal FSharpStructuredToolTipElement = FSharpToolTipElement<Layout>

/// Information for building a data tip box.
type FSharpToolTipText<'T> = 
    /// A list of data tip elements to display.
    | FSharpToolTipText of FSharpToolTipElement<'T> list  

// specialization that stores data as strings
type FSharpToolTipText = FSharpToolTipText<string>
// specialization that stores data as <see cref="Layout"/>
type internal FSharpStructuredToolTipText = FSharpToolTipText<Layout>

module internal Tooltips =
    let ToFSharpToolTipElement tooltip = 
        match tooltip with
        | FSharpStructuredToolTipElement.None -> 
            FSharpToolTipElement.None
        | FSharpStructuredToolTipElement.Group(l) -> 
            FSharpToolTipElement.Group(l |> List.map(fun x -> 
                { MainDescription=showL x.MainDescription
                  XmlDoc=x.XmlDoc
                  TypeMapping=List.map showL x.TypeMapping
                  ParamName=x.ParamName
                  Remarks= Option.map showL x.Remarks }))
        | FSharpStructuredToolTipElement.CompositionError(text) -> 
            FSharpToolTipElement.CompositionError(text)

    let ToFSharpToolTipText (FSharpStructuredToolTipText.FSharpToolTipText(text)) = 
        FSharpToolTipText(List.map ToFSharpToolTipElement text)
    
    let Map f a = async.Bind(a, f >> async.Return)

[<RequireQualifiedAccess>]
type CompletionItemKind =
    | Field
    | Property
    | Method of isExtension : bool
    | Event
    | Argument
    | Other

type UnresolvedSymbol =
    { DisplayName: string
      Namespace: string[] }

type CompletionItem =
    { ItemWithInst: ItemWithInst
      Kind: CompletionItemKind
      IsOwnMember: bool
      MinorPriority: int
      Type: TyconRef option
      Unresolved: UnresolvedSymbol option }
    member x.Item = x.ItemWithInst.Item
      

[<Sealed>]
type FSharpMethodGroupItemParameter(name: string, canonicalTypeTextForSorting: string, display: Layout, isOptional: bool) = 
    member __.ParameterName = name
    member __.CanonicalTypeTextForSorting = canonicalTypeTextForSorting
    member __.StructuredDisplay = display
    member __.Display = showL display
    member __.IsOptional = isOptional

[<AutoOpen>]
module internal ItemDescriptionsImpl = 

    let isFunction g typ =
        let _,tau = tryDestForallTy g typ
        isFunTy g tau 

    let OutputFullName isListItem ppF fnF r = 
      // Only display full names in quick info, not declaration lists or method lists
      if not isListItem then 
        match ppF r with 
        | None -> emptyL
        | Some _ -> wordL (tagText (FSComp.SR.typeInfoFullName())) ^^ RightL.colon ^^ (fnF r)
      else emptyL
          
    let rangeOfValRef preferFlag (vref:ValRef) =
        match preferFlag with 
        | None -> vref.Range 
        | Some false -> vref.DefinitionRange 
        | Some true -> vref.SigRange

    let rangeOfEntityRef preferFlag (eref:EntityRef) =
        match preferFlag with 
        | None -> eref.Range 
        | Some false -> eref.DefinitionRange 
        | Some true -> eref.SigRange

   
    let rangeOfPropInfo preferFlag (pinfo:PropInfo) =
        match pinfo with
#if EXTENSIONTYPING 
        |   ProvidedProp(_,pi,_) -> ComputeDefinitionLocationOfProvidedItem pi
#endif
        |   _ -> pinfo.ArbitraryValRef |> Option.map (rangeOfValRef preferFlag)

    let rangeOfMethInfo (g:TcGlobals) preferFlag (minfo:MethInfo) = 
        match minfo with
#if EXTENSIONTYPING 
        |   ProvidedMeth(_,mi,_,_) -> ComputeDefinitionLocationOfProvidedItem mi
#endif
        |   DefaultStructCtor(_, AppTy g (tcref, _)) -> Some(rangeOfEntityRef preferFlag tcref)
        |   _ -> minfo.ArbitraryValRef |> Option.map (rangeOfValRef preferFlag)

    let rangeOfEventInfo preferFlag (einfo:EventInfo) = 
        match einfo with
#if EXTENSIONTYPING 
        | ProvidedEvent (_,ei,_) -> ComputeDefinitionLocationOfProvidedItem ei
#endif
        | _ -> einfo.ArbitraryValRef |> Option.map (rangeOfValRef preferFlag)
      
    let rangeOfUnionCaseInfo preferFlag (ucinfo:UnionCaseInfo) =      
        match preferFlag with 
        | None -> ucinfo.UnionCase.Range 
        | Some false -> ucinfo.UnionCase.DefinitionRange 
        | Some true -> ucinfo.UnionCase.SigRange

    let rangeOfRecdFieldInfo preferFlag (rfinfo:RecdFieldInfo) =      
        match preferFlag with 
        | None -> rfinfo.RecdField.Range 
        | Some false -> rfinfo.RecdField.DefinitionRange 
        | Some true -> rfinfo.RecdField.SigRange

    let rec rangeOfItem (g:TcGlobals) preferFlag d = 
        match d with
        | Item.Value vref  | Item.CustomBuilder (_,vref) -> Some (rangeOfValRef preferFlag vref)
        | Item.UnionCase(ucinfo,_)     -> Some (rangeOfUnionCaseInfo preferFlag ucinfo)
        | Item.ActivePatternCase apref -> Some (rangeOfValRef preferFlag apref.ActivePatternVal)
        | Item.ExnCase tcref           -> Some tcref.Range
        | Item.RecdField rfinfo        -> Some (rangeOfRecdFieldInfo preferFlag rfinfo)
        | Item.Event einfo             -> rangeOfEventInfo preferFlag einfo
        | Item.ILField _               -> None
        | Item.Property(_,pinfos)      -> rangeOfPropInfo preferFlag pinfos.Head 
        | Item.Types(_,typs)     -> typs |> List.tryPick (tryNiceEntityRefOfTy >> Option.map (rangeOfEntityRef preferFlag))
        | Item.CustomOperation (_,_,Some minfo)  -> rangeOfMethInfo g preferFlag minfo
        | Item.TypeVar (_,tp)  -> Some tp.Range
        | Item.ModuleOrNamespaces(modrefs) -> modrefs |> List.tryPick (rangeOfEntityRef preferFlag >> Some)
        | Item.MethodGroup(_,minfos,_) 
        | Item.CtorGroup(_,minfos) -> minfos |> List.tryPick (rangeOfMethInfo g preferFlag)
        | Item.ActivePatternResult(APInfo _,_, _, m) -> Some m
        | Item.SetterArg (_,item) -> rangeOfItem g preferFlag item
        | Item.ArgName (id,_, _) -> Some id.idRange
        | Item.CustomOperation (_,_,implOpt) -> implOpt |> Option.bind (rangeOfMethInfo g preferFlag)
        | Item.ImplicitOp _ -> None
        | Item.NewDef id -> Some id.idRange
        | Item.UnqualifiedType tcrefs -> tcrefs |> List.tryPick (rangeOfEntityRef preferFlag >> Some)
        | Item.DelegateCtor typ 
        | Item.FakeInterfaceCtor typ -> typ |> tryNiceEntityRefOfTy |> Option.map (rangeOfEntityRef preferFlag)

    // Provided type definitions do not have a useful F# CCU for the purposes of goto-definition.
    let computeCcuOfTyconRef (tcref:TyconRef) = 
#if EXTENSIONTYPING
        if tcref.IsProvided then None else 
#endif
        ccuOfTyconRef tcref

    let ccuOfMethInfo (g:TcGlobals) (minfo:MethInfo) = 
        match minfo with
        | DefaultStructCtor(_, AppTy g (tcref, _)) -> computeCcuOfTyconRef tcref
        | _ -> 
            minfo.ArbitraryValRef 
            |> Option.bind ccuOfValRef 
            |> Option.orElseWith (fun () -> minfo.DeclaringEntityRef |> computeCcuOfTyconRef)


    let rec ccuOfItem (g:TcGlobals) d = 
        match d with
        | Item.Value vref | Item.CustomBuilder (_,vref) -> ccuOfValRef vref 
        | Item.UnionCase(ucinfo,_)             -> computeCcuOfTyconRef ucinfo.TyconRef
        | Item.ActivePatternCase apref         -> ccuOfValRef apref.ActivePatternVal
        | Item.ExnCase tcref                   -> computeCcuOfTyconRef tcref
        | Item.RecdField rfinfo                -> computeCcuOfTyconRef rfinfo.RecdFieldRef.TyconRef
        | Item.Event einfo                     -> einfo.EnclosingType  |> tcrefOfAppTy g |> computeCcuOfTyconRef
        | Item.ILField finfo                   -> finfo.EnclosingType |> tcrefOfAppTy g |> computeCcuOfTyconRef
        | Item.Property(_,pinfos)              -> 
            pinfos |> List.tryPick (fun pinfo -> 
                pinfo.ArbitraryValRef 
                |> Option.bind ccuOfValRef
                |> Option.orElseWith (fun () -> pinfo.EnclosingType |> tcrefOfAppTy g |> computeCcuOfTyconRef))

        | Item.ArgName (_,_,Some (ArgumentContainer.Method minfo))  -> ccuOfMethInfo g minfo

        | Item.MethodGroup(_,minfos,_)
        | Item.CtorGroup(_,minfos) -> minfos |> List.tryPick (ccuOfMethInfo g)
        | Item.CustomOperation (_,_,Some minfo)       -> ccuOfMethInfo g minfo

        | Item.Types(_,typs)             -> typs |> List.tryPick (tryNiceEntityRefOfTy >> Option.bind computeCcuOfTyconRef)

        | Item.ArgName (_,_,Some (ArgumentContainer.Type eref)) -> computeCcuOfTyconRef eref

        | Item.ModuleOrNamespaces(erefs) 
        | Item.UnqualifiedType(erefs) -> erefs |> List.tryPick computeCcuOfTyconRef 

        | Item.SetterArg (_,item) -> ccuOfItem g item
        | Item.TypeVar _  -> None
        | _ -> None

    /// Work out the source file for an item and fix it up relative to the CCU if it is relative.
    let fileNameOfItem (g:TcGlobals) qualProjectDir (m:range) h =
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
        | FSMeth(_,_,vref,_) -> 
            let argInfos = ArgInfosOfMember g vref |> List.concat 
            // Drop the first 'seq<T>' argument representing the computation space
            let argInfos = if argInfos.IsEmpty then [] else argInfos.Tail
            [ for (ty,argInfo) in argInfos do
                  let isPP = HasFSharpAttribute g g.attrib_ProjectionParameterAttribute argInfo.Attribs
                  // Strip the tuple space type of the type of projection parameters
                  let ty = if isPP && isFunTy g ty then rangeOfFunTy g ty else ty
                  yield ParamNameAndType(argInfo.Name, ty) ]
        | _ -> []

    // Find the name of the metadata file for this external definition 
    let metaInfoOfEntityRef (infoReader:InfoReader) m tcref = 
        let g = infoReader.g
        match tcref with 
        | ERefLocal _ -> None
        | ERefNonLocal nlref -> 
            // Generalize to get a formal signature 
            let formalTypars = tcref.Typars(m)
            let formalTypeInst = generalizeTypars formalTypars
            let formalTypeInfo = ILTypeInfo.FromType g (TType_app(tcref,formalTypeInst))
            Some(nlref.Ccu.FileName,formalTypars,formalTypeInfo)

    let mkXmlComment thing =
        match thing with
        | Some (Some(fileName), xmlDocSig) -> FSharpXmlDoc.XmlDocFileSignature(fileName, xmlDocSig)
        | _ -> FSharpXmlDoc.None

    let GetXmlDocSigOfEntityRef infoReader m (eref:EntityRef) = 
        if eref.IsILTycon then 
            match metaInfoOfEntityRef infoReader m eref  with
            | None -> None
            | Some (ccuFileName,_,formalTypeInfo) -> Some(ccuFileName,"T:"+formalTypeInfo.ILTypeRef.FullName)
        else
            let ccuFileName = libFileOfEntityRef eref
            let m = eref.Deref
            if m.XmlDocSig = "" then
                m.XmlDocSig <- XmlDocSigOfEntity eref
            Some (ccuFileName, m.XmlDocSig)

    let GetXmlDocSigOfScopedValRef g (tcref:TyconRef) (vref:ValRef) = 
        let ccuFileName = libFileOfEntityRef tcref
        let v = vref.Deref
        if v.XmlDocSig = "" then
            v.XmlDocSig <- XmlDocSigOfVal g (buildAccessPath vref.TopValActualParent.CompilationPathOpt) v
        Some (ccuFileName, v.XmlDocSig)                

    let GetXmlDocSigOfRecdFieldInfo (rfinfo:RecdFieldInfo) = 
        let tcref = rfinfo.TyconRef
        let ccuFileName = libFileOfEntityRef tcref 
        if rfinfo.RecdField.XmlDocSig = "" then
            rfinfo.RecdField.XmlDocSig <- XmlDocSigOfProperty [tcref.CompiledRepresentationForNamedType.FullName; rfinfo.Name]
        Some (ccuFileName, rfinfo.RecdField.XmlDocSig)            

    let GetXmlDocSigOfUnionCaseInfo (ucinfo:UnionCaseInfo) = 
        let tcref =  ucinfo.TyconRef
        let ccuFileName = libFileOfEntityRef tcref
        if  ucinfo.UnionCase.XmlDocSig = "" then
            ucinfo.UnionCase.XmlDocSig <- XmlDocSigOfUnionCase [tcref.CompiledRepresentationForNamedType.FullName; ucinfo.Name]
        Some (ccuFileName,  ucinfo.UnionCase.XmlDocSig)

    let GetXmlDocSigOfMethInfo (infoReader:InfoReader)  m (minfo:MethInfo) = 
        let amap = infoReader.amap
        match minfo with
        | FSMeth (g,_,vref,_) ->
            GetXmlDocSigOfScopedValRef g minfo.DeclaringEntityRef vref
        | ILMeth (g,ilminfo,_) ->            
            let actualTypeName = ilminfo.DeclaringTyconRef.CompiledRepresentationForNamedType.FullName
            let fmtps = ilminfo.FormalMethodTypars            
            let genArity = if fmtps.Length=0 then "" else sprintf "``%d" fmtps.Length

            match metaInfoOfEntityRef infoReader m ilminfo.DeclaringTyconRef  with 
            | None -> None
            | Some (ccuFileName,formalTypars,formalTypeInfo) ->
                let filminfo = ILMethInfo(g,formalTypeInfo.ToType,None,ilminfo.RawMetadata,fmtps) 
                let args = 
                    match ilminfo.IsILExtensionMethod with
                    | true -> filminfo.GetRawArgTypes(amap,m,minfo.FormalMethodInst)
                    | false -> filminfo.GetParamTypes(amap,m,minfo.FormalMethodInst)

                // http://msdn.microsoft.com/en-us/library/fsbx0t7x.aspx
                // If the name of the item itself has periods, they are replaced by the hash-sign ('#'). It is assumed that no item has a hash-sign directly in its name. For example, the fully qualified name of the String constructor would be "System.String.#ctor".
                let normalizedName = ilminfo.ILName.Replace(".","#")

                Some (ccuFileName,"M:"+actualTypeName+"."+normalizedName+genArity+XmlDocArgsEnc g (formalTypars,fmtps) args)
        | DefaultStructCtor _ -> None
#if EXTENSIONTYPING
        | ProvidedMeth _ -> None
#endif

    let GetXmlDocSigOfValRef g (vref:ValRef) =
        if not vref.IsLocalRef then
            let ccuFileName = vref.nlr.Ccu.FileName
            let v = vref.Deref
            if v.XmlDocSig = "" then
                v.XmlDocSig <- XmlDocSigOfVal g vref.TopValActualParent.CompiledRepresentationForNamedType.Name v
            Some (ccuFileName, v.XmlDocSig)
        else 
            None

    let GetXmlDocSigOfProp infoReader m pinfo =
        match pinfo with 
#if EXTENSIONTYPING
        | ProvidedProp _ -> None // No signature is possible. If an xml comment existed it would have been returned by PropInfo.XmlDoc in infos.fs
#endif
        | FSProp (g,typ,_,_) as fspinfo -> 
            let tcref = tcrefOfAppTy g typ
            match fspinfo.ArbitraryValRef with 
            | None -> None
            | Some vref -> GetXmlDocSigOfScopedValRef g tcref vref
        | ILProp(g, (ILPropInfo(tinfo,pdef))) -> 
            let tcref = tinfo.TyconRef
            match metaInfoOfEntityRef infoReader m tcref  with
            | Some (ccuFileName,formalTypars,formalTypeInfo) ->
                let filpinfo = ILPropInfo(formalTypeInfo,pdef)
                Some (ccuFileName,"P:"+formalTypeInfo.ILTypeRef.FullName+"."+pdef.Name+XmlDocArgsEnc g (formalTypars,[]) (filpinfo.GetParamTypes(infoReader.amap,m)))
            | _ -> None

    let GetXmlDocSigOfEvent infoReader m (einfo:EventInfo) =
        match einfo with
        | ILEvent(_,ilEventInfo) ->
            let tinfo = ilEventInfo.ILTypeInfo 
            let tcref = tinfo.TyconRef 
            match metaInfoOfEntityRef infoReader m tcref  with 
            | Some (ccuFileName,_,formalTypeInfo) -> 
                Some(ccuFileName,"E:"+formalTypeInfo.ILTypeRef.FullName+"."+einfo.EventName)
            | _ -> None
        | _ -> None

    let GetXmlDocSigOfILFieldInfo infoReader m (finfo:ILFieldInfo) =
        match metaInfoOfEntityRef infoReader m (tcrefOfAppTy infoReader.g finfo.EnclosingType) with
        | Some (ccuFileName,_,formalTypeInfo) ->
            Some(ccuFileName,"F:"+formalTypeInfo.ILTypeRef.FullName+"."+finfo.FieldName)
        | _ -> None

    /// This function gets the signature to pass to Visual Studio to use its lookup functions for .NET stuff. 
    let GetXmlDocHelpSigOfItemForLookup (infoReader:InfoReader) m d = 
        let g = infoReader.g
                
        match d with
        | Item.ActivePatternCase (APElemRef(_, vref, _))        
        | Item.Value vref | Item.CustomBuilder (_,vref) -> 
            mkXmlComment (GetXmlDocSigOfValRef g vref)
        | Item.UnionCase  (ucinfo,_) -> mkXmlComment (GetXmlDocSigOfUnionCaseInfo ucinfo)
        | Item.ExnCase tcref -> mkXmlComment (GetXmlDocSigOfEntityRef infoReader m tcref)
        | Item.RecdField rfinfo -> mkXmlComment (GetXmlDocSigOfRecdFieldInfo rfinfo)
        | Item.NewDef _ -> FSharpXmlDoc.None
        | Item.ILField finfo -> mkXmlComment (GetXmlDocSigOfILFieldInfo infoReader m finfo)
        | Item.Types(_,((TType_app(tcref,_)) :: _)) ->  mkXmlComment (GetXmlDocSigOfEntityRef infoReader m tcref)
        | Item.CustomOperation (_,_,Some minfo) -> mkXmlComment (GetXmlDocSigOfMethInfo infoReader  m minfo)
        | Item.TypeVar _  -> FSharpXmlDoc.None
        | Item.ModuleOrNamespaces(modref :: _) -> mkXmlComment (GetXmlDocSigOfEntityRef infoReader m modref)

        | Item.Property(_,(pinfo :: _)) -> mkXmlComment (GetXmlDocSigOfProp infoReader m pinfo)
        | Item.Event(einfo) -> mkXmlComment (GetXmlDocSigOfEvent infoReader m einfo)

        | Item.MethodGroup(_,minfo :: _,_) -> mkXmlComment (GetXmlDocSigOfMethInfo infoReader  m minfo)
        | Item.CtorGroup(_,minfo :: _) -> mkXmlComment (GetXmlDocSigOfMethInfo infoReader  m minfo)
        | Item.ArgName(_, _, Some argContainer) -> 
            match argContainer with 
            | ArgumentContainer.Method minfo -> mkXmlComment (GetXmlDocSigOfMethInfo infoReader m minfo)
            | ArgumentContainer.Type tcref -> mkXmlComment (GetXmlDocSigOfEntityRef infoReader m tcref)
            | ArgumentContainer.UnionCase ucinfo -> mkXmlComment (GetXmlDocSigOfUnionCaseInfo ucinfo)
        |  _ -> FSharpXmlDoc.None

    /// Produce an XmlComment with a signature or raw text, given the F# comment and the item
    let GetXmlCommentForItemAux (xmlDoc:XmlDoc option) (infoReader:InfoReader) m d = 
        let result = 
            match xmlDoc with 
            | None | Some (XmlDoc [| |]) -> ""
            | Some (XmlDoc l) -> 
                bufs (fun os -> 
                    bprintf os "\n"; 
                    l |> Array.iter (fun (s:string) -> 
                        // Note: this code runs for local/within-project xmldoc tooltips, but not for cross-project or .XML
                        bprintf os "\n%s" s))

        if String.IsNullOrEmpty result then 
            GetXmlDocHelpSigOfItemForLookup infoReader m d
        else
            FSharpXmlDoc.Text result

    let mutable ToolTipFault  = None
    
    let GetXmlCommentForMethInfoItem infoReader m d (minfo: MethInfo) = 
        GetXmlCommentForItemAux (if minfo.HasDirectXmlComment then Some minfo.XmlDoc else None) infoReader m d 

    let FormatTyparMapping denv (prettyTyparInst: TyparInst) = 
        [ for (tp,ty) in prettyTyparInst -> 
            wordL (tagTypeParameter ("'" + tp.DisplayName))  ^^ wordL (tagText (FSComp.SR.descriptionWordIs())) ^^ NicePrint.layoutType denv ty  ]

    /// Generate the structured tooltip for a method info
    let FormatOverloadsToList (infoReader:InfoReader) m denv (item: ItemWithInst) minfos : FSharpStructuredToolTipElement = 
        ToolTipFault |> Option.iter (fun msg -> 
           let exn = Error((0,msg),range.Zero)
           let ph = PhasedDiagnostic.Create(exn, BuildPhase.TypeCheck)
           simulateError ph)
        
        let layouts = 
            [ for minfo in minfos -> 
                let prettyTyparInst, layout = NicePrint.prettyLayoutOfMethInfoFreeStyle infoReader.amap m denv item.TyparInst minfo
                let xml = GetXmlCommentForMethInfoItem infoReader m item.Item minfo
                let tpsL = FormatTyparMapping denv prettyTyparInst
                FSharpToolTipElementData<_>.Create(layout, xml, tpsL) ]
 
        FSharpStructuredToolTipElement.Group(layouts)

        
    let pubpathOfValRef (v:ValRef) = v.PublicPath        
    let pubpathOfTyconRef (x:TyconRef) = x.PublicPath


    // Wrapper type for use by the 'partialDistinctBy' function
    [<StructuralEquality; NoComparison>]
    type WrapType<'T> = Wrap of 'T
    
    // Like Seq.distinctBy but only filters out duplicates for some of the elements
    let partialDistinctBy (per:IPartialEqualityComparer<'T>) seq =
        let wper = 
            { new IPartialEqualityComparer<WrapType<'T>> with
                member __.InEqualityRelation (Wrap x) = per.InEqualityRelation (x)
                member __.Equals(Wrap x, Wrap y) = per.Equals(x, y)
                member __.GetHashCode (Wrap x) = per.GetHashCode(x) }
        // Wrap a Wrap _ around all keys in case the key type is itself a type using null as a representation
        let dict = Dictionary<WrapType<'T>,obj>(wper)
        seq |> List.filter (fun v -> 
            let key = Wrap(v)
            if (per.InEqualityRelation(v)) then 
                if dict.ContainsKey(key) then false else (dict.[key] <- null; true)
            else true)

    let (|ItemWhereTypIsPreferred|_|) item = 
        match item with 
        | Item.DelegateCtor ty
        | Item.CtorGroup(_, [DefaultStructCtor(_,ty)])
        | Item.FakeInterfaceCtor ty
        | Item.Types(_,[ty])  -> Some ty
        | _ -> None

    /// Specifies functions for comparing 'Item' objects with respect to the user 
    /// (this means that some values that are not technically equal are treated as equal 
    ///  if this is what we want to show to the user, because we're comparing just the name
    //   for some cases e.g. when using 'fullDisplayTextOfModRef')
    let ItemDisplayPartialEquality g = 
      { new IPartialEqualityComparer<_> with   
          member x.InEqualityRelation item = 
              match item  with 
              | Item.Types(_,[_]) -> true
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
              let equalTypes(ty1, ty2) =
                  if isAppTy g ty1 && isAppTy g ty2 then tyconRefEq g (tcrefOfAppTy g ty1) (tcrefOfAppTy g ty2) 
                  else typeEquiv g ty1 ty2
              match item1,item2 with 
              | Item.DelegateCtor(ty1), Item.DelegateCtor(ty2) -> equalTypes(ty1, ty2)
              | Item.Types(dn1,[ty1]), Item.Types(dn2,[ty2]) -> 
                  // Bug 4403: We need to compare names as well, because 'int' and 'Int32' are physically the same type, but we want to show both
                  dn1 = dn2 && equalTypes(ty1, ty2) 
              
              // Prefer a type to a DefaultStructCtor, a DelegateCtor and a FakeInterfaceCtor 
              | ItemWhereTypIsPreferred(ty1), ItemWhereTypIsPreferred(ty2) -> equalTypes(ty1, ty2) 

              | Item.ExnCase(tcref1), Item.ExnCase(tcref2) -> tyconRefEq g tcref1 tcref2
              | Item.ILField(ILFieldInfo(_, fld1)), Item.ILField(ILFieldInfo(_, fld2)) -> 
                  fld1 === fld2 // reference equality on the object identity of the AbstractIL metadata blobs for the fields
              | Item.CustomOperation (_,_,Some minfo1), Item.CustomOperation (_,_,Some minfo2) -> 
                    MethInfo.MethInfosUseIdenticalDefinitions minfo1 minfo2
              | Item.TypeVar (nm1,tp1), Item.TypeVar (nm2,tp2) -> 
                    (nm1 = nm2) && typarRefEq tp1 tp2
              | Item.ModuleOrNamespaces(modref1 :: _), Item.ModuleOrNamespaces(modref2 :: _) -> fullDisplayTextOfModRef modref1 = fullDisplayTextOfModRef modref2
              | Item.SetterArg(id1,_), Item.SetterArg(id2,_) -> (id1.idRange, id1.idText) = (id2.idRange, id2.idText)
              | Item.MethodGroup(_, meths1,_), Item.MethodGroup(_, meths2,_) -> 
                  Seq.zip meths1 meths2 |> Seq.forall (fun (minfo1, minfo2) ->
                    MethInfo.MethInfosUseIdenticalDefinitions minfo1 minfo2)
              | (Item.Value vref1 | Item.CustomBuilder (_,vref1)), (Item.Value vref2 | Item.CustomBuilder (_,vref2)) -> valRefEq g vref1 vref2
              | Item.ActivePatternCase(APElemRef(_apinfo1, vref1, idx1)), Item.ActivePatternCase(APElemRef(_apinfo2, vref2, idx2)) ->
                  idx1 = idx2 && valRefEq g vref1 vref2
              | Item.UnionCase(UnionCaseInfo(_, ur1),_), Item.UnionCase(UnionCaseInfo(_, ur2),_) -> g.unionCaseRefEq ur1 ur2
              | Item.RecdField(RecdFieldInfo(_, RFRef(tcref1, n1))), Item.RecdField(RecdFieldInfo(_, RFRef(tcref2, n2))) -> 
                  (tyconRefEq g tcref1 tcref2) && (n1 = n2) // there is no direct function as in the previous case
              | Item.Property(_, pi1s), Item.Property(_, pi2s) -> 
                  List.zip pi1s pi2s |> List.forall(fun (pi1, pi2) -> PropInfo.PropInfosUseIdenticalDefinitions pi1 pi2)
              | Item.Event(evt1), Item.Event(evt2) -> EventInfo.EventInfosUseIdenticalDefintions evt1 evt2
              | Item.CtorGroup(_, meths1), Item.CtorGroup(_, meths2) -> 
                  List.zip meths1 meths2 
                  |> List.forall (fun (minfo1, minfo2) -> MethInfo.MethInfosUseIdenticalDefinitions minfo1 minfo2)
              | Item.UnqualifiedType(tcRefs1), Item.UnqualifiedType(tcRefs2) ->
                  List.zip tcRefs1 tcRefs2
                  |> List.forall (fun (tcRef1, tcRef2) -> tyconRefEq g tcRef1 tcRef2)
              | Item.Types(_,[TType.TType_app(tcRef1,_)]), Item.UnqualifiedType([tcRef2]) -> tyconRefEq g tcRef1 tcRef2
              | Item.UnqualifiedType([tcRef1]), Item.Types(_,[TType.TType_app(tcRef2,_)]) -> tyconRefEq g tcRef1 tcRef2
              | _ -> false)
              
          member x.GetHashCode item =
            // This may explore assemblies that are not in the reference set.
            // In this case just bail out and use a random hash code
            protectAssemblyExploration 1027 (fun () -> 
              match item with 
              | ItemWhereTypIsPreferred ty -> 
                  if isAppTy g ty then hash (tcrefOfAppTy g ty).Stamp
                  else 1010
              | Item.ILField(ILFieldInfo(_, fld)) -> 
                  System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode fld // hash on the object identity of the AbstractIL metadata blob for the field
              | Item.TypeVar (nm,_tp) -> hash nm
              | Item.CustomOperation (_,_,Some minfo) -> minfo.ComputeHashCode()
              | Item.CustomOperation (_,_,None) -> 1
              | Item.ModuleOrNamespaces(modref :: _) -> hash (fullDisplayTextOfModRef modref)          
              | Item.SetterArg(id,_) -> hash (id.idRange, id.idText)
              | Item.MethodGroup(_, meths,_) -> meths |> List.fold (fun st a -> st + a.ComputeHashCode()) 0
              | Item.CtorGroup(name, meths) -> name.GetHashCode() + (meths |> List.fold (fun st a -> st + a.ComputeHashCode()) 0)
              | (Item.Value vref | Item.CustomBuilder (_,vref)) -> hash vref.LogicalName
              | Item.ActivePatternCase(APElemRef(_apinfo, vref, idx)) -> hash (vref.LogicalName, idx)
              | Item.ExnCase(tcref) -> hash tcref.Stamp
              | Item.UnionCase(UnionCaseInfo(_, UCRef(tcref, n)),_) -> hash(tcref.Stamp, n)
              | Item.RecdField(RecdFieldInfo(_, RFRef(tcref, n))) -> hash(tcref.Stamp, n)
              | Item.Event evt -> evt.ComputeHashCode()
              | Item.Property(_name, pis) -> hash (pis |> List.map (fun pi -> pi.ComputeHashCode()))
              | Item.UnqualifiedType(tcref :: _) -> hash tcref.Stamp
              | _ -> failwith "unreachable") }

    let CompletionItemDisplayPartialEquality g = 
        let itemComparer = ItemDisplayPartialEquality g
  
        { new IPartialEqualityComparer<CompletionItem> with
            member x.InEqualityRelation item = itemComparer.InEqualityRelation item.Item
            member x.Equals(item1, item2) = itemComparer.Equals(item1.Item, item2.Item)
            member x.GetHashCode (item) = itemComparer.GetHashCode(item.Item) }

    let ItemWithTypeDisplayPartialEquality g = 
        let itemComparer = ItemDisplayPartialEquality g
        
        { new IPartialEqualityComparer<Item * _> with
            member x.InEqualityRelation ((item, _)) = itemComparer.InEqualityRelation item
            member x.Equals((item1, _), (item2, _)) = itemComparer.Equals(item1, item2)
            member x.GetHashCode ((item, _)) = itemComparer.GetHashCode(item) }
    
    // Remove items containing the same module references
    let RemoveDuplicateModuleRefs modrefs  = 
        modrefs |> partialDistinctBy 
                      { new IPartialEqualityComparer<ModuleOrNamespaceRef> with
                          member x.InEqualityRelation _ = true
                          member x.Equals(item1, item2) = (fullDisplayTextOfModRef item1 = fullDisplayTextOfModRef item2)
                          member x.GetHashCode(item) = hash item.Stamp  }

    /// Remove all duplicate items
    let RemoveDuplicateItems g (items: ItemWithInst list) = 
        items |> partialDistinctBy (IPartialEqualityComparer.On (fun item -> item.Item) (ItemDisplayPartialEquality g))

    /// Remove all duplicate items
    let RemoveDuplicateCompletionItems g items = 
        items |> partialDistinctBy (CompletionItemDisplayPartialEquality g) 

    let IsExplicitlySuppressed (g: TcGlobals) (item: Item) = 
        // This may explore assemblies that are not in the reference set.
        // In this case just assume the item is not suppressed.
        protectAssemblyExploration true (fun () -> 
         match item with 
         | Item.Types(it, [ty]) -> 
             g.suppressed_types 
             |> List.exists (fun supp -> 
                if isAppTy g ty then 
                  // check if they are the same logical type (after removing all abbreviations)
                  let tcr1 = tcrefOfAppTy g ty
                  let tcr2 = tcrefOfAppTy g (generalizedTyconRef supp) 
                  tyconRefEq g tcr1 tcr2 && 
                  // check the display name is precisely the one we're suppressing
                  it = supp.DisplayName
                else false) 
         | _ -> false)

    /// Filter types that are explicitly suppressed from the IntelliSense (such as uppercase "FSharpList", "Option", etc.)
    let RemoveExplicitlySuppressed (g: TcGlobals) (items: ItemWithInst list) = 
      items |> List.filter (fun item -> not (IsExplicitlySuppressed g item.Item))

    /// Filter types that are explicitly suppressed from the IntelliSense (such as uppercase "FSharpList", "Option", etc.)
    let RemoveExplicitlySuppressedCompletionItems (g: TcGlobals) (items: CompletionItem list) = 
      items |> List.filter (fun item -> not (IsExplicitlySuppressed g item.Item))

    let SimplerDisplayEnv denv = 
        { denv with suppressInlineKeyword=true; 
                    shortConstraints=true; 
                    showConstraintTyparAnnotations=false; 
                    abbreviateAdditionalConstraints=false;
                    suppressNestedTypes=true;
                    maxMembers=Some EnvMisc2.maxMembers }

    let rec FullNameOfItem g item = 
        let denv = DisplayEnv.Empty(g)
        match item with
        | Item.ImplicitOp(_, { contents = Some(TraitConstraintSln.FSMethSln(_, vref, _)) }) 
        | Item.Value vref | Item.CustomBuilder (_,vref) -> fullDisplayTextOfValRef vref
        | Item.UnionCase (ucinfo,_) -> fullDisplayTextOfUnionCaseRef  ucinfo.UnionCaseRef
        | Item.ActivePatternResult(apinfo, _ty, idx, _) -> apinfo.Names.[idx]
        | Item.ActivePatternCase apref -> FullNameOfItem g (Item.Value apref.ActivePatternVal)  + "." + apref.Name 
        | Item.ExnCase ecref -> fullDisplayTextOfExnRef ecref 
        | Item.RecdField rfinfo -> fullDisplayTextOfRecdFieldRef  rfinfo.RecdFieldRef
        | Item.NewDef id -> id.idText
        | Item.ILField finfo -> bufs (fun os -> NicePrint.outputILTypeRef denv os finfo.ILTypeRef; bprintf os ".%s" finfo.FieldName)
        | Item.Event einfo -> bufs (fun os -> NicePrint.outputTyconRef denv os (tcrefOfAppTy g einfo.EnclosingType); bprintf os ".%s" einfo.EventName)
        | Item.Property(_,(pinfo::_)) -> bufs (fun os -> NicePrint.outputTyconRef denv os (tcrefOfAppTy g pinfo.EnclosingType); bprintf os ".%s" pinfo.PropertyName)
        | Item.CustomOperation (customOpName,_,_) -> customOpName
        | Item.CtorGroup(_,minfo :: _) -> bufs (fun os -> NicePrint.outputTyconRef denv os minfo.DeclaringEntityRef)
        | Item.MethodGroup(_,_,Some minfo) -> bufs (fun os -> NicePrint.outputTyconRef denv os minfo.DeclaringEntityRef; bprintf os ".%s" minfo.DisplayName)        
        | Item.MethodGroup(_,minfo :: _,_) -> bufs (fun os -> NicePrint.outputTyconRef denv os minfo.DeclaringEntityRef; bprintf os ".%s" minfo.DisplayName)        
        | Item.UnqualifiedType (tcref :: _) -> bufs (fun os -> NicePrint.outputTyconRef denv os tcref)
        | Item.FakeInterfaceCtor typ 
        | Item.DelegateCtor typ 
        | Item.Types(_,typ:: _) -> 
            match tryDestAppTy g typ with
            | Some tcref -> bufs (fun os -> NicePrint.outputTyconRef denv os tcref)
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
        | Item.Types(_,[]) 
        | Item.CtorGroup(_,[]) 
        | Item.MethodGroup(_,[],_) 
        | Item.ModuleOrNamespaces []
        | Item.Property(_,[]) -> ""

    /// Output a the description of a language item
    let rec GetXmlCommentForItem (infoReader:InfoReader) m item = 
        let g = infoReader.g
        match item with
        | Item.ImplicitOp(_, { contents = Some(TraitConstraintSln.FSMethSln(_, vref, _)) }) -> 
            GetXmlCommentForItem infoReader m (Item.Value vref)

        | Item.Value vref | Item.CustomBuilder (_,vref) ->            
            GetXmlCommentForItemAux (if valRefInThisAssembly g.compilingFslib vref then Some vref.XmlDoc else None) infoReader m item 

        | Item.UnionCase(ucinfo,_) -> 
            GetXmlCommentForItemAux (if tyconRefUsesLocalXmlDoc g.compilingFslib ucinfo.TyconRef then Some ucinfo.UnionCase .XmlDoc else None) infoReader m item 

        | Item.ActivePatternCase apref -> 
            GetXmlCommentForItemAux (Some apref.ActivePatternVal.XmlDoc) infoReader m item 

        | Item.ExnCase ecref -> 
            GetXmlCommentForItemAux (if tyconRefUsesLocalXmlDoc g.compilingFslib ecref then Some ecref.XmlDoc else None) infoReader m item 

        | Item.RecdField rfinfo ->
            GetXmlCommentForItemAux (if tyconRefUsesLocalXmlDoc g.compilingFslib rfinfo.TyconRef then Some rfinfo.RecdField.XmlDoc else None) infoReader m item 

        | Item.Event einfo ->
            GetXmlCommentForItemAux (if einfo.HasDirectXmlComment  then Some einfo.XmlDoc else None) infoReader m item 

        | Item.Property(_,pinfos) -> 
            let pinfo = pinfos.Head
            GetXmlCommentForItemAux (if pinfo.HasDirectXmlComment then Some pinfo.XmlDoc else None) infoReader m item 

        | Item.CustomOperation (_,_,Some minfo) 
        | Item.CtorGroup(_,minfo :: _) 
        | Item.MethodGroup(_,minfo :: _,_) ->
            GetXmlCommentForMethInfoItem infoReader m item minfo

        | Item.Types(_,((TType_app(tcref,_)):: _)) -> 
            GetXmlCommentForItemAux (if tyconRefUsesLocalXmlDoc g.compilingFslib tcref then Some tcref.XmlDoc else None) infoReader m item 

        | Item.ModuleOrNamespaces((modref :: _) as modrefs) -> 
            let definiteNamespace = modrefs |> List.forall (fun modref -> modref.IsNamespace)
            if not definiteNamespace then
                GetXmlCommentForItemAux (if entityRefInThisAssembly g.compilingFslib modref then Some modref.XmlDoc else None) infoReader m item 
            else
                GetXmlCommentForItemAux None infoReader m item

        | Item.ArgName (_, _, argContainer) -> 
            let xmldoc = 
                match argContainer with
                | Some(ArgumentContainer.Method (minfo)) ->
                    if minfo.HasDirectXmlComment then Some minfo.XmlDoc else None 
                | Some(ArgumentContainer.Type(tcref)) ->
                    if (tyconRefUsesLocalXmlDoc g.compilingFslib tcref) then Some tcref.XmlDoc else None
                | Some(ArgumentContainer.UnionCase(ucinfo)) ->
                    if (tyconRefUsesLocalXmlDoc g.compilingFslib ucinfo.TyconRef) then Some ucinfo.UnionCase.XmlDoc else None
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
            | Item.Types(_,((TType_app(tcref,_)):: _)) -> 
                let ty = generalizedTyconRef tcref
                Infos.ExistsHeadTypeInEntireHierarchy g amap range0 ty g.tcref_System_Attribute
            | _ -> false
        with _ -> false

    /// Output the quick info information of a language item
    let rec FormatItemDescriptionToToolTipElement isListItem (infoReader:InfoReader) m denv (item: ItemWithInst) = 
        let g = infoReader.g
        let amap = infoReader.amap
        let denv = SimplerDisplayEnv denv 
        let xml = GetXmlCommentForItem infoReader m item.Item
        match item.Item with
        | Item.ImplicitOp(_, { contents = Some(TraitConstraintSln.FSMethSln(_, vref, _)) }) -> 
            // operator with solution
            FormatItemDescriptionToToolTipElement isListItem infoReader m denv { item with Item = Item.Value vref }

        | Item.Value vref | Item.CustomBuilder (_,vref) ->            
            let prettyTyparInst, resL = NicePrint.layoutQualifiedValOrMember denv item.TyparInst vref.Deref
            let remarks = OutputFullName isListItem pubpathOfValRef fullDisplayTextOfValRefAsLayout vref
            let tpsL = FormatTyparMapping denv prettyTyparInst
            FSharpStructuredToolTipElement.Single(resL, xml, tpsL, remarks=remarks)

        // Union tags (constructors)
        | Item.UnionCase(ucinfo,_) -> 
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
            let (prettyTyparInst,ptau), _cxs = PrettyTypes.PrettifyInstAndType denv.g (item.TyparInst, tau)
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
                NicePrint.layoutTyconRef denv (tcrefOfAppTy g einfo.EnclosingType) ^^
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
        | Item.CustomOperation (customOpName,usageText,Some minfo) -> 

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
                        let argTys = ParamNameAndTypesOfUnaryCustomOperation g minfo |> List.map (fun (ParamNameAndType(_,ty)) -> ty)
                        let argTys, _ = PrettyTypes.PrettifyTypes g argTys 
                        wordL (tagMethod customOpName) ^^ sepListL SepL.space (List.map (fun ty -> LeftL.leftParen ^^ NicePrint.layoutType denv ty ^^ SepL.rightParen) argTys)
                ) ^^
                SepL.lineBreak ^^ SepL.lineBreak  ^^
                wordL (tagText (FSComp.SR.typeInfoCallsWord())) ^^
                NicePrint.layoutTyconRef denv (tcrefOfAppTy g minfo.EnclosingType) ^^
                SepL.dot ^^
                wordL (tagMethod minfo.DisplayName)

            FSharpStructuredToolTipElement.Single (layout, xml)

        // F# constructors and methods
        | Item.CtorGroup(_,minfos) 
        | Item.MethodGroup(_,minfos,_) ->
            FormatOverloadsToList infoReader m denv item minfos
        
        // The 'fake' zero-argument constructors of .NET interfaces.
        // This ideally should never appear in intellisense, but we do get here in repros like:
        //     type IFoo = abstract F : int
        //     type II = IFoo  // remove 'type II = ' and quickly hover over IFoo before it gets squiggled for 'invalid use of interface type'
        // and in that case we'll just show the interface type name.
        | Item.FakeInterfaceCtor typ ->
           let typ, _ = PrettyTypes.PrettifyType g typ
           let layout = NicePrint.layoutTyconRef denv (tcrefOfAppTy g typ)
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
        | Item.Types(_,((TType_app(tcref,_)):: _))
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
                    ([],modrefs) 
                    ||> Seq.fold (fun st modref -> 
                        match fullDisplayTextOfParentOfModRef modref with 
                        | Some(txt) -> txt::st 
                        | _ -> st) 
                    |> Seq.mapi (fun i x -> i,x) 
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

    let printCanonicalizedTypeName g (denv:DisplayEnv) tau =
        // get rid of F# abbreviations and such
        let strippedType = stripTyEqnsWrtErasure EraseAll g tau
        // pretend no namespaces are open
        let denv = denv.SetOpenPaths([])
        // now printing will see a .NET-like canonical representation, that is good for sorting overloads into a reasonable order (see bug 94520)
        NicePrint.stringOfTy denv strippedType

    let PrettyParamOfRecdField g denv (f: RecdField) =
        FSharpMethodGroupItemParameter(
          name = f.Name,
          canonicalTypeTextForSorting = printCanonicalizedTypeName g denv f.FormalType,
          // Note: the instantiation of any type parameters is currently incorporated directly into the type
          // rather than being returned separately.
          display = NicePrint.prettyLayoutOfType denv f.FormalType,
          isOptional=false)
    
    let PrettyParamOfUnionCaseField g denv isGenerated (i: int) (f: RecdField) = 
        let initial = PrettyParamOfRecdField g denv f
        let display = 
            if isGenerated i f then 
                initial.StructuredDisplay 
            else 
                // TODO: in this case ucinst is ignored - it gives the instantiation of the type parameters of
                // the union type containing this case.
                NicePrint.layoutOfParamData denv (ParamData(false, false, NotOptional, NoCallerInfo, Some f.Id, ReflectedArgInfo.None, f.FormalType)) 
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
    let PrettyParamsOfParamDatas g denv typarInst (paramDatas:ParamData list) rty = 
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

        // Prettify everything
        let prettyTyparInst, (prettyParamTys, _prettyRetTy), (prettyParamTysL, prettyRetTyL), prettyConstraintsL = 
            NicePrint.prettyLayoutOfInstAndSig denv (typarInst, paramTypes, rty)

        // Remake the params using the prettified versions
        let prettyParams = 
          (paramInfo,prettyParamTys,prettyParamTysL) |||> List.map3 (fun (nm,isOptArg,paramPrefix) tau tyL -> 
            FSharpMethodGroupItemParameter(
              name = nm,
              canonicalTypeTextForSorting = printCanonicalizedTypeName g denv tau,
              display = paramPrefix ^^ tyL,
              isOptional=isOptArg
            ))

        prettyTyparInst, prettyParams, prettyRetTyL, prettyConstraintsL

    let PrettyParamsOfTypes g denv typarInst paramTys retTy = 

        // Prettify everything
        let prettyTyparInst, (prettyParamTys, _prettyRetTy), (prettyParamTysL, prettyRetTyL), prettyConstraintsL = 
            NicePrint.prettyLayoutOfInstAndSig denv  (typarInst, paramTys, retTy) 

        // Remake the params using the prettified versions
        let parameters = 
            (prettyParamTys,prettyParamTysL) 
            ||> List.zip 
            |> List.map (fun (tau, tyL) -> 
                FSharpMethodGroupItemParameter(
                    name = "",
                    canonicalTypeTextForSorting = printCanonicalizedTypeName g denv tau,
                    display =  tyL,
                    isOptional=false
                ))

        // Return the results
        prettyTyparInst, parameters, prettyRetTyL, prettyConstraintsL
                          

#if EXTENSIONTYPING

    /// Determine if an item is a provided type 
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

    /// Determine if an item is a provided type that has static parameters
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

    /// Determine if an item has static arguments
    let (|ItemIsWithStaticArguments|_|) m g item =
        match item with
        | ItemIsProvidedTypeWithStaticArguments m g staticParameters -> Some staticParameters
        | ItemIsProvidedMethodWithStaticArguments staticParameters -> Some staticParameters
        | _ -> None
#endif

    /// Get the set of static parameters associated with an item
    let StaticParamsOfItem (infoReader:InfoReader) m denv item = 
        let amap = infoReader.amap
        let g = infoReader.g
        match item with
#if EXTENSIONTYPING
        | ItemIsWithStaticArguments m g staticParameters ->
            staticParameters 
                |> Array.map (fun sp -> 
                    let typ = Import.ImportProvidedType amap m (sp.PApply((fun x -> x.ParameterType),m))
                    let spKind = NicePrint.prettyLayoutOfType denv typ
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

    /// Get all the information about parameters and "prettify" the types by choosing nice type variable
    /// names.  This is similar to the other variations on "show me an item" code. This version is
    /// is used when presenting groups of methods (see FSharpMethodGroup).  It is possible these different
    /// versions could be better unified.
    let rec PrettyParamsAndReturnTypeOfItem (infoReader:InfoReader) m denv (item: ItemWithInst) = 
        let amap = infoReader.amap
        let g = infoReader.g
        let denv = {SimplerDisplayEnv denv with useColonForReturnType=true}
        match item.Item with
        | Item.Value vref -> 
            let getPrettyParamsOfTypes() = 
                let tau = vref.TauType
                match tryDestFunTy denv.g tau with
                | Some(arg,rtau) ->
                    let args = tryDestRefTupleTy denv.g arg 
                    let _prettyTyparInst, prettyParams, prettyRetTyL, _prettyConstraintsL = PrettyParamsOfTypes g denv item.TyparInst args rtau
                    // FUTURE: prettyTyparInst is the pretty version of the known instantiations of type parameters in the output. It could be returned
                    // for display as part of the method group
                    prettyParams, prettyRetTyL
                | None -> 
                    let _prettyTyparInst, prettyTyL = NicePrint.prettyLayoutOfUncurriedSig denv item.TyparInst [] tau
                    [], prettyTyL

            match vref.ValReprInfo with
            | None -> 
                // ValReprInfo = None i.e. in let bindings defined in types or in local functions
                // in this case use old approach and return only information about types
                getPrettyParamsOfTypes ()

            | Some valRefInfo ->
                // ValReprInfo will exist for top-level syntactic functions
                // per spec: binding is considered to define a syntactic function if it is either a function or its immediate right-hand-side is a anonymous function
                let (_, argInfos,  lastRetTy, _) = GetTopValTypeInFSharpForm  g valRefInfo vref.Type m
                match argInfos with
                | [] -> 
                    // handles cases like 'let foo = List.map'
                    getPrettyParamsOfTypes() 
                | firstCurriedArgInfo::_ ->
                    // result 'paramDatas' collection corresponds to the first argument of curried function
                    // i.e. let func (a : int) (b : int) = a + b
                    // paramDatas will contain information about a and retTy will be: int -> int
                    // This is good enough as we don't provide ways to display info for the second curried argument
                    let firstCurriedParamDatas = 
                        firstCurriedArgInfo
                        |> List.map ParamNameAndType.FromArgInfo
                        |> List.map (fun (ParamNameAndType(nmOpt, pty)) -> ParamData(false, false, NotOptional, NoCallerInfo, nmOpt, ReflectedArgInfo.None, pty))

                    // Adjust the return type so it only strips the first argument
                    let curriedRetTy = 
                        match tryDestFunTy denv.g vref.TauType with
                        | Some(_,rtau) -> rtau
                        | None -> lastRetTy

                    let _prettyTyparInst, prettyFirstCurriedParams, prettyCurriedRetTyL, prettyConstraintsL = PrettyParamsOfParamDatas g denv item.TyparInst firstCurriedParamDatas curriedRetTy
                    
                    let prettyCurriedRetTyL = prettyCurriedRetTyL ^^ SepL.space ^^ prettyConstraintsL

                    prettyFirstCurriedParams, prettyCurriedRetTyL

        | Item.UnionCase(ucinfo,_)   -> 
            let prettyParams = 
                match ucinfo.UnionCase.RecdFields with
                | [f] -> [PrettyParamOfUnionCaseField g denv NicePrint.isGeneratedUnionCaseField -1 f]
                | fs -> fs |> List.mapi (PrettyParamOfUnionCaseField g denv NicePrint.isGeneratedUnionCaseField)
            let rty = generalizedTyconRef ucinfo.TyconRef
            let rtyL = NicePrint.layoutType denv rty
            prettyParams, rtyL

        | Item.ActivePatternCase(apref)   -> 
            let v = apref.ActivePatternVal 
            let tau = v.TauType
            let args, resTy = stripFunTy denv.g tau 

            let apinfo = Option.get (TryGetActivePatternInfo v)
            let aparity = apinfo.Names.Length
            
            let rty = if aparity <= 1 then resTy else (argsOfAppTy g resTy).[apref.CaseIndex]

            let _prettyTyparInst, prettyParams, prettyRetTyL, _prettyConstraintsL = PrettyParamsOfTypes g denv item.TyparInst args rty
            // FUTURE: prettyTyparInst is the pretty version of the known instantiations of type parameters in the output. It could be returned
            // for display as part of the method group
            prettyParams, prettyRetTyL

        | Item.ExnCase ecref -> 
            let prettyParams = ecref |> recdFieldsOfExnDefRef |> List.mapi (PrettyParamOfUnionCaseField g denv NicePrint.isGeneratedExceptionField) 
            let _prettyTyparInst, prettyRetTyL = NicePrint.prettyLayoutOfUncurriedSig denv item.TyparInst [] g.exn_ty
            prettyParams, prettyRetTyL

        | Item.RecdField rfinfo ->
            let _prettyTyparInst, prettyRetTyL = NicePrint.prettyLayoutOfUncurriedSig denv item.TyparInst [] rfinfo.FieldType
            [], prettyRetTyL

        | Item.ILField finfo ->
            let _prettyTyparInst, prettyRetTyL = NicePrint.prettyLayoutOfUncurriedSig denv item.TyparInst [] (finfo.FieldType(amap,m))
            [], prettyRetTyL

        | Item.Event einfo ->
            let _prettyTyparInst, prettyRetTyL = NicePrint.prettyLayoutOfUncurriedSig denv item.TyparInst [] (PropTypOfEventInfo infoReader m AccessibleFromSomewhere einfo)
            [], prettyRetTyL

        | Item.Property(_,pinfo :: _) -> 
            let paramDatas = pinfo.GetParamDatas(amap,m)
            let rty = pinfo.GetPropertyType(amap,m) 

            let _prettyTyparInst, prettyParams, prettyRetTyL, _prettyConstraintsL = PrettyParamsOfParamDatas g denv item.TyparInst paramDatas rty
            // FUTURE: prettyTyparInst is the pretty version of the known instantiations of type parameters in the output. It could be returned
            // for display as part of the method group
            prettyParams, prettyRetTyL

        | Item.CtorGroup(_,(minfo :: _)) 
        | Item.MethodGroup(_,(minfo :: _),_) -> 
            let paramDatas = minfo.GetParamDatas(amap, m, minfo.FormalMethodInst) |> List.head
            let rty = minfo.GetFSharpReturnTy(amap, m, minfo.FormalMethodInst)
            let _prettyTyparInst, prettyParams, prettyRetTyL, _prettyConstraintsL = PrettyParamsOfParamDatas g denv item.TyparInst paramDatas rty
            // FUTURE: prettyTyparInst is the pretty version of the known instantiations of type parameters in the output. It could be returned
            // for display as part of the method group
            prettyParams, prettyRetTyL

        | Item.CustomBuilder (_,vref) -> 
            PrettyParamsAndReturnTypeOfItem infoReader m denv { item with Item = Item.Value vref }

        | Item.TypeVar _ -> 
            [], emptyL

        | Item.CustomOperation (_,usageText, Some minfo) -> 
            match usageText() with 
            | None -> 
                let argNamesAndTys = ParamNameAndTypesOfUnaryCustomOperation g minfo 
                let argTys, _ = PrettyTypes.PrettifyTypes g (argNamesAndTys |> List.map (fun (ParamNameAndType(_,ty)) -> ty))
                let paramDatas = (argNamesAndTys, argTys) ||> List.map2 (fun (ParamNameAndType(nmOpt, _)) argTy -> ParamData(false, false, NotOptional, NoCallerInfo, nmOpt, ReflectedArgInfo.None,argTy))
                let rty = minfo.GetFSharpReturnTy(amap, m, minfo.FormalMethodInst)
                let _prettyTyparInst, prettyParams, prettyRetTyL, _prettyConstraintsL = PrettyParamsOfParamDatas g denv item.TyparInst paramDatas rty

                // FUTURE: prettyTyparInst is the pretty version of the known instantiations of type parameters in the output. It could be returned
                // for display as part of the method group
                prettyParams, prettyRetTyL

            | Some _ -> 
                let rty = minfo.GetFSharpReturnTy(amap, m, minfo.FormalMethodInst)
                let _prettyTyparInst, prettyRetTyL = NicePrint.prettyLayoutOfUncurriedSig denv item.TyparInst [] rty
                [], prettyRetTyL  // no parameter data available for binary operators like 'zip', 'join' and 'groupJoin' since they use bespoke syntax 

        | Item.FakeInterfaceCtor typ -> 
            let _prettyTyparInst, prettyRetTyL = NicePrint.prettyLayoutOfUncurriedSig denv item.TyparInst [] typ
            [], prettyRetTyL

        | Item.DelegateCtor delty -> 
            let (SigOfFunctionForDelegate(_, _, _, fty)) = GetSigOfFunctionForDelegate infoReader delty m AccessibleFromSomeFSharpCode

            // No need to pass more generic type information in here since the instanitations have already been applied
            let _prettyTyparInst, prettyParams, prettyRetTyL, _prettyConstraintsL = PrettyParamsOfParamDatas g denv item.TyparInst [ParamData(false, false, NotOptional, NoCallerInfo, None, ReflectedArgInfo.None, fty)] delty

            // FUTURE: prettyTyparInst is the pretty version of the known instantiations of type parameters in the output. It could be returned
            // for display as part of the method group
            prettyParams, prettyRetTyL

        |  _ -> 
            [], emptyL

    /// Get the "F1 Keyword" associated with an item, for looking up documentatio help indexes on the web
    let rec GetF1Keyword g item = 

        let getKeywordForMethInfo (minfo : MethInfo) =
            match minfo with 
            | FSMeth(_, _, vref, _) ->
                match vref.ActualParent with
                | Parent tcref ->
                    (tcref |> ticksAndArgCountTextOfTyconRef)+"."+vref.CompiledName|> Some
                | ParentNone -> None
                
            | ILMeth (_,minfo,_) ->
                let typeString = minfo.DeclaringTyconRef |> ticksAndArgCountTextOfTyconRef
                let paramString =
                    let nGenericParams = minfo.RawMetadata.GenericParams.Length 
                    if nGenericParams > 0 then "``"+(nGenericParams.ToString()) else ""
                sprintf "%s.%s%s" typeString minfo.RawMetadata.Name paramString |> Some

            | DefaultStructCtor _  -> None
#if EXTENSIONTYPING
            | ProvidedMeth _ -> None
#endif
             
        match item with
        | Item.Value vref | Item.CustomBuilder (_,vref) -> 
            let v = vref.Deref
            if v.IsModuleBinding then
                let tyconRef = v.TopValActualParent
                let paramsString =
                    match v.Typars with
                    |   [] -> ""
                    |   l -> "``"+(List.length l).ToString() 
                
                sprintf "%s.%s%s" (tyconRef |> ticksAndArgCountTextOfTyconRef) v.CompiledName paramsString |> Some
            else
                None

        | Item.ActivePatternCase apref -> 
            GetF1Keyword g (Item.Value apref.ActivePatternVal)

        | Item.UnionCase(ucinfo,_) -> 
            (ucinfo.TyconRef |> ticksAndArgCountTextOfTyconRef)+"."+ucinfo.Name |> Some

        | Item.RecdField rfi -> 
            (rfi.TyconRef |> ticksAndArgCountTextOfTyconRef)+"."+rfi.Name |> Some
        
        | Item.ILField finfo ->   
             match finfo with 
             | ILFieldInfo(tinfo, fdef) -> 
                 (tinfo.TyconRef |> ticksAndArgCountTextOfTyconRef)+"."+fdef.Name |> Some
#if EXTENSIONTYPING
             | ProvidedField _ -> None
#endif
        | Item.Types(_,((AppTy g (tcref,_)) :: _)) 
        | Item.DelegateCtor(AppTy g (tcref,_))
        | Item.FakeInterfaceCtor(AppTy g (tcref,_))
        | Item.UnqualifiedType (tcref::_)
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
#if EXTENSIONTYPING                
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

        | Item.Property(_,(pinfo :: _)) -> 
            match pinfo with 
            | FSProp(_, _, Some vref, _) 
            | FSProp(_, _, _, Some vref) -> 
                // per spec, extension members in F1 keywords are qualified with definition class
                match vref.ActualParent with 
                | Parent tcref ->
                    (tcref |> ticksAndArgCountTextOfTyconRef)+"."+vref.PropertyName|> Some                     
                | ParentNone -> None

            | ILProp(_, (ILPropInfo(tinfo,pdef))) -> 
                let tcref = tinfo.TyconRef
                (tcref |> ticksAndArgCountTextOfTyconRef)+"."+pdef.Name |> Some
            | FSProp _ -> None
#if EXTENSIONTYPING
            | ProvidedProp _ -> None
#endif
        | Item.Property(_,[]) -> None // Pathological case of the above
                   
        | Item.Event einfo -> 
            match einfo with 
            | ILEvent(_,ilEventInfo)  ->
                let tinfo = ilEventInfo.ILTypeInfo
                let tcref = tinfo.TyconRef 
                (tcref |> ticksAndArgCountTextOfTyconRef)+"."+einfo.EventName |> Some
            | FSEvent(_,pinfo,_,_) ->
                match pinfo.ArbitraryValRef with 
                | Some vref ->
                   // per spec, members in F1 keywords are qualified with definition class
                   match vref.ActualParent with 
                   | Parent tcref -> (tcref |> ticksAndArgCountTextOfTyconRef)+"."+vref.PropertyName|> Some                     
                   | ParentNone -> None
                | None -> None
#if EXTENSIONTYPING
            | ProvidedEvent _ -> None 
#endif
        | Item.CtorGroup(_,minfos) ->
            match minfos with 
            | [] -> None
            | FSMeth(_, _, vref, _) :: _ ->
                   match vref.ActualParent with
                   | Parent tcref -> (tcref |> ticksAndArgCountTextOfTyconRef) + ".#ctor"|> Some
                   | ParentNone -> None
            | (ILMeth (_,minfo,_)) :: _ ->
                let tcref = minfo.DeclaringTyconRef
                (tcref |> ticksAndArgCountTextOfTyconRef)+".#ctor" |> Some
            | (DefaultStructCtor (g,typ) :: _) ->  
                let tcref = tcrefOfAppTy g typ
                (ticksAndArgCountTextOfTyconRef tcref) + ".#ctor" |> Some
#if EXTENSIONTYPING
            | ProvidedMeth _::_ -> None
#endif
        | Item.CustomOperation (_,_,Some minfo) -> getKeywordForMethInfo minfo
        | Item.MethodGroup(_,_,Some minfo) -> getKeywordForMethInfo minfo
        | Item.MethodGroup(_,minfo :: _,_) -> getKeywordForMethInfo minfo
        | Item.SetterArg (_, propOrField) -> GetF1Keyword g propOrField 
        | Item.MethodGroup(_,[],_) 
        | Item.CustomOperation (_,_,None)   // "into"
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
            (fun err -> FSharpStructuredToolTipElement.CompositionError(err))

    /// Compute the index of the VS glyph shown with an item in the Intellisense menu
    let GlyphOfItem(denv, item) : FSharpGlyph = 
         /// Find the glyph for the given representation.    
         let reprToGlyph repr = 
            match repr with
            | TFSharpObjectRepr om -> 
                match om.fsobjmodel_kind with 
                | TTyconClass -> FSharpGlyph.Class
                | TTyconInterface -> FSharpGlyph.Interface
                | TTyconStruct -> FSharpGlyph.Struct
                | TTyconDelegate _ -> FSharpGlyph.Delegate
                | TTyconEnum _ -> FSharpGlyph.Enum
            | TRecdRepr _ -> FSharpGlyph.Type
            | TUnionRepr _ -> FSharpGlyph.Union
            | TILObjectRepr (TILObjectReprData (_,_,td)) -> 
                match td.tdKind with 
                | ILTypeDefKind.Class -> FSharpGlyph.Class
                | ILTypeDefKind.ValueType -> FSharpGlyph.Struct
                | ILTypeDefKind.Interface -> FSharpGlyph.Interface
                | ILTypeDefKind.Enum -> FSharpGlyph.Enum
                | ILTypeDefKind.Delegate -> FSharpGlyph.Delegate
            | TAsmRepr _ -> FSharpGlyph.Typedef
            | TMeasureableRepr _-> FSharpGlyph.Typedef 
#if EXTENSIONTYPING
            | TProvidedTypeExtensionPoint _-> FSharpGlyph.Typedef 
            | TProvidedNamespaceExtensionPoint  _-> FSharpGlyph.Typedef  
#endif
            | TNoRepr -> FSharpGlyph.Class  
         
         /// Find the glyph for the given type representation.
         let typeToGlyph typ = 
            if isAppTy denv.g typ then 
                let tcref = tcrefOfAppTy denv.g typ
                tcref.TypeReprInfo |> reprToGlyph 
            elif isStructTupleTy denv.g typ then FSharpGlyph.Struct
            elif isRefTupleTy denv.g typ then FSharpGlyph.Class
            elif isFunction denv.g typ then FSharpGlyph.Delegate
            elif isTyparTy denv.g typ then FSharpGlyph.Struct
            else FSharpGlyph.Typedef
            
         // This may explore assemblies that are not in the reference set,
         // e.g. for type abbreviations to types not in the reference set. 
         // In this case just use GlyphMajor.Class.
         protectAssemblyExploration FSharpGlyph.Class (fun () ->
            match item with 
            | Item.Value(vref) | Item.CustomBuilder (_,vref) -> 
                  if isFunction denv.g vref.Type then FSharpGlyph.Method
                  elif vref.LiteralValue.IsSome then FSharpGlyph.Constant
                  else FSharpGlyph.Variable
            | Item.Types(_,typ::_) -> typeToGlyph (stripTyEqns denv.g typ)    
            | Item.UnionCase _
            | Item.ActivePatternCase _ -> FSharpGlyph.EnumMember   
            | Item.ExnCase _ -> FSharpGlyph.Exception   
            | Item.RecdField _ -> FSharpGlyph.Field
            | Item.ILField _ -> FSharpGlyph.Field
            | Item.Event _ -> FSharpGlyph.Event   
            | Item.Property _ -> FSharpGlyph.Property   
            | Item.CtorGroup _ 
            | Item.DelegateCtor _ 
            | Item.FakeInterfaceCtor _
            | Item.CustomOperation _ -> FSharpGlyph.Method
            | Item.MethodGroup (_, minfos, _) when minfos |> List.forall (fun minfo -> minfo.IsExtensionMember) -> FSharpGlyph.ExtensionMethod
            | Item.MethodGroup _ -> FSharpGlyph.Method
            | Item.TypeVar _ 
            | Item.Types _  -> FSharpGlyph.Class
            | Item.UnqualifiedType (tcref :: _) -> 
                if tcref.IsEnumTycon || tcref.IsILEnumTycon then FSharpGlyph.Enum
                elif tcref.IsExceptionDecl then FSharpGlyph.Exception
                elif tcref.IsFSharpDelegateTycon then FSharpGlyph.Delegate
                elif tcref.IsFSharpInterfaceTycon then FSharpGlyph.Interface
                elif tcref.IsFSharpStructOrEnumTycon then FSharpGlyph.Struct
                elif tcref.IsModule then FSharpGlyph.Module
                elif tcref.IsNamespace then FSharpGlyph.NameSpace
                elif tcref.IsUnionTycon then FSharpGlyph.Union
                elif tcref.IsILTycon then 
                    let (TILObjectReprData (_, _, tydef)) = tcref.ILTyconInfo
                    if tydef.IsInterface then FSharpGlyph.Interface
                    elif tydef.IsDelegate then FSharpGlyph.Delegate
                    elif tydef.IsEnum then FSharpGlyph.Enum
                    elif tydef.IsStructOrEnum then FSharpGlyph.Struct
                    else FSharpGlyph.Class
                else FSharpGlyph.Class
            | Item.ModuleOrNamespaces(modref::_) -> 
                  if modref.IsNamespace then FSharpGlyph.NameSpace else FSharpGlyph.Module
            | Item.ArgName _ -> FSharpGlyph.Variable
            | Item.SetterArg _ -> FSharpGlyph.Variable
            | _ -> FSharpGlyph.Error)

    /// Get rid of groups of overloads an replace them with single items.
    let FlattenItems g m item =
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
        | ItemIsWithStaticArguments m g _ -> [item] // we pretend that provided-types-with-static-args are method-like in order to get ParamInfo for them
#endif
        | Item.CustomOperation(_name, _helpText, _minfo) -> [item]
        | Item.TypeVar _ -> []
        | Item.CustomBuilder _ -> []
        | _ -> []

    /// Get rid of groups of overloads an replace them with single items.
    /// (This looks like it is doing the a similar thing as FlattenItems, this code 
    /// duplication could potentially be removed)
    let AnotherFlattenItems g m item =
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
        | ItemIsWithStaticArguments m g _ -> 
            // we pretend that provided-types-with-static-args are method-like in order to get ParamInfo for them
            [item] 
#endif
        | Item.MethodGroup(nm,minfos,orig) -> minfos |> List.map (fun minfo -> Item.MethodGroup(nm,[minfo],orig)) 
        | Item.CustomOperation(_name, _helpText, _minfo) -> [item]
        | Item.TypeVar _ -> []
        | Item.CustomBuilder _ -> []
        | _ -> []

type FSharpAccessibility(a:Accessibility, ?isProtected) = 
    let isProtected = defaultArg isProtected  false

    let isInternalCompPath x = 
        match x with 
        | CompPath(ILScopeRef.Local,[]) -> true 
        | _ -> false

    let (|Public|Internal|Private|) (TAccess p) = 
        match p with 
        | [] -> Public 
        | _ when List.forall isInternalCompPath p  -> Internal 
        | _ -> Private

    member __.IsPublic = not isProtected && match a with Public -> true | _ -> false

    member __.IsPrivate = not isProtected && match a with Private -> true | _ -> false

    member __.IsInternal = not isProtected && match a with Internal -> true | _ -> false

    member __.IsProtected = isProtected

    member __.Contents = a

    override __.ToString() = 
        let (TAccess paths) = a
        let mangledTextOfCompPath (CompPath(scoref,path)) = getNameOfScopeRef scoref + "/" + textOfPath (List.map fst path)  
        String.concat ";" (List.map mangledTextOfCompPath paths)

/// An intellisense declaration
[<Sealed>]
type FSharpDeclarationListItem(name: string, nameInCode: string, fullName: string, glyph: FSharpGlyph, info, accessibility: FSharpAccessibility option,
                               kind: CompletionItemKind, isOwnMember: bool, priority: int, isResolved: bool, namespaceToOpen: string option) =

    let mutable descriptionTextHolder: FSharpToolTipText<_> option = None
    let mutable task = null

    member __.Name = name
    member __.NameInCode = nameInCode

    member __.StructuredDescriptionTextAsync = 
        match info with
        | Choice1Of2 (items: CompletionItem list, infoReader, m, denv, reactor:IReactorOperations, checkAlive) -> 
            // reactor causes the lambda to execute on the background compiler thread, through the Reactor
            reactor.EnqueueAndAwaitOpAsync ("StructuredDescriptionTextAsync", fun ctok -> 
                RequireCompilationThread ctok
                // This is where we do some work which may touch TAST data structures owned by the IncrementalBuilder - infoReader, item etc. 
                // It is written to be robust to a disposal of an IncrementalBuilder, in which case it will just return the empty string. 
                // It is best to think of this as a "weak reference" to the IncrementalBuilder, i.e. this code is written to be robust to its
                // disposal. Yes, you are right to scratch your head here, but this is ok.
                cancellable.Return(
                    if checkAlive() then 
                        FSharpToolTipText(items |> List.map (fun x -> ItemDescriptionsImpl.FormatStructuredDescriptionOfItem true infoReader m denv x.ItemWithInst))
                    else 
                        FSharpToolTipText [ FSharpStructuredToolTipElement.Single(wordL (tagText (FSComp.SR.descriptionUnavailable())), FSharpXmlDoc.None) ]))
            | Choice2Of2 result -> 
                async.Return result

    member decl.DescriptionTextAsync = 
        decl.StructuredDescriptionTextAsync
        |> Tooltips.Map Tooltips.ToFSharpToolTipText

    member decl.StructuredDescriptionText = 
        match descriptionTextHolder with
        | Some descriptionText -> descriptionText
        | None ->
            match info with
            | Choice1Of2 _ -> 

                // The dataTipSpinWaitTime limits how long we block the UI thread while a tooltip pops up next to a selected item in an IntelliSense completion list.
                // This time appears to be somewhat amortized by the time it takes the VS completion UI to actually bring up the tooltip after selecting an item in the first place.
                if isNull task then
                    // kick off the actual (non-cooperative) work
                    task <- System.Threading.Tasks.Task.Factory.StartNew(fun() -> 
                        let text = decl.StructuredDescriptionTextAsync |> Async.RunSynchronously
                        descriptionTextHolder <- Some text) 

                // The dataTipSpinWaitTime limits how long we block the UI thread while a tooltip pops up next to a selected item in an IntelliSense completion list.
                // This time appears to be somewhat amortized by the time it takes the VS completion UI to actually bring up the tooltip after selecting an item in the first place.
                task.Wait EnvMisc2.dataTipSpinWaitTime  |> ignore
                match descriptionTextHolder with 
                | Some text -> text
                | None -> FSharpToolTipText [ FSharpStructuredToolTipElement.Single(wordL (tagText (FSComp.SR.loadingDescription())), FSharpXmlDoc.None) ]

            | Choice2Of2 result -> 
                result

    member decl.DescriptionText = decl.StructuredDescriptionText |> Tooltips.ToFSharpToolTipText
    member __.Glyph = glyph 
    member __.Accessibility = accessibility
    member __.Kind = kind
    member __.IsOwnMember = isOwnMember
    member __.MinorPriority = priority
    member __.FullName = fullName
    member __.IsResolved = isResolved
    member __.NamespaceToOpen = namespaceToOpen

/// A table of declarations for Intellisense completion 
[<Sealed>]
type FSharpDeclarationListInfo(declarations: FSharpDeclarationListItem[], isForType: bool, isError: bool) = 
    member __.Items = declarations
    member __.IsForType = isForType
    member __.IsError = isError

    // Make a 'Declarations' object for a set of selected items
    static member Create(infoReader:InfoReader, m, denv, getAccessibility, items: CompletionItem list, reactor, currentNamespaceOrModule: string[] option, isAttributeApplicationContext: bool, checkAlive) = 
        let g = infoReader.g
        let isForType = items |> List.exists (fun x -> x.Type.IsSome)
        let items = items |> ItemDescriptionsImpl.RemoveExplicitlySuppressedCompletionItems g
        
        let tyconRefOptEq tref1 tref2 =
            match tref1 with
            | Some tref1 -> tyconRefEq g tref1 tref2
            | None -> false

        // Adjust items priority. Sort by name. For things with the same name, 
        //     - show types with fewer generic parameters first
        //     - show types before over other related items - they usually have very useful XmlDocs 
        let _, _, items = 
            items 
            |> List.map (fun x ->
                match x.Item with
                | Item.Types (_,(TType_app(tcref,_) :: _)) -> { x with MinorPriority = 1 + tcref.TyparsNoRange.Length }
                // Put delegate ctors after types, sorted by #typars. RemoveDuplicateItems will remove FakeInterfaceCtor and DelegateCtor if an earlier type is also reported with this name
                | Item.FakeInterfaceCtor (TType_app(tcref,_)) 
                | Item.DelegateCtor (TType_app(tcref,_)) -> { x with MinorPriority = 1000 + tcref.TyparsNoRange.Length }
                // Put type ctors after types, sorted by #typars. RemoveDuplicateItems will remove DefaultStructCtors if a type is also reported with this name
                | Item.CtorGroup (_, (cinfo :: _)) -> { x with MinorPriority = 1000 + 10 * (tcrefOfAppTy g cinfo.EnclosingType).TyparsNoRange.Length }
                | Item.MethodGroup(_, minfo :: _, _) -> { x with IsOwnMember = tyconRefOptEq x.Type minfo.DeclaringEntityRef }
                | Item.Property(_, pinfo :: _) -> { x with IsOwnMember = tyconRefOptEq x.Type (tcrefOfAppTy g pinfo.EnclosingType) }
                | Item.ILField finfo -> { x with IsOwnMember = tyconRefOptEq x.Type (tcrefOfAppTy g finfo.EnclosingType) }
                | _ -> x)
            |> List.sortBy (fun x -> x.MinorPriority)
            |> List.fold (fun (prevRealPrior, prevNormalizedPrior, acc) x ->
                if x.MinorPriority = prevRealPrior then
                    prevRealPrior, prevNormalizedPrior, x :: acc
                else
                    let normalizedPrior = prevNormalizedPrior + 1
                    x.MinorPriority, normalizedPrior, { x with MinorPriority = normalizedPrior } :: acc
                ) (0, 0, [])

        if verbose then dprintf "service.ml: mkDecls: %d found groups after filtering\n" (List.length items); 

        // Group by full name for unresolved items and by display name for resolved ones.
        let items = 
            items
            |> List.rev
            // Prefer items from file check results to ones from referenced assemblies via GetAssemblyContent ("all entities")
            |> List.sortBy (fun x -> x.Unresolved.IsSome) 
            // Remove all duplicates. We've put the types first, so this removes the DelegateCtor and DefaultStructCtor's.
            |> RemoveDuplicateCompletionItems g
            |> List.groupBy (fun x ->
                match x.Unresolved with
                | Some u -> 
                    match u.Namespace with
                    | [||] -> u.DisplayName
                    | ns -> (ns |> String.concat ".") + "." + u.DisplayName
                | None -> x.Item.DisplayName)
            |> List.map (fun (_, items) -> 
                let item = items.Head
                let name = 
                    match item.Unresolved with
                    | Some u -> u.DisplayName
                    | None -> item.Item.DisplayName
                name, items)

        // Filter out operators (and list)
        let items = 
            // Check whether this item looks like an operator.
            let isOperatorItem(name, items: CompletionItem list) = 
                match items |> List.map (fun x -> x.Item) with
                | [Item.Value _ | Item.MethodGroup _ | Item.UnionCase _] -> IsOperatorName name
                | _ -> false              
            let isFSharpList name = (name = "[]") // list shows up as a Type and a UnionCase, only such entity with a symbolic name, but want to filter out of intellisense
            items |> List.filter (fun (displayName, items) -> not (isOperatorItem(displayName, items)) && not (isFSharpList displayName)) 
                    
        let decls = 
            items 
            |> List.map (fun (displayName, itemsWithSameFullName) -> 
                match itemsWithSameFullName with
                | [] -> failwith "Unexpected empty bag"
                | _ ->
                    let items =
                        match itemsWithSameFullName |> List.partition (fun x -> x.Unresolved.IsNone) with
                        | [], unresolved -> unresolved
                        // if there are resolvable items, throw out unresolved to prevent duplicates like `Set` and `FSharp.Collections.Set`.
                        | resolved, _ -> resolved 
                    
                    let item = items.Head
                    let glyph = ItemDescriptionsImpl.GlyphOfItem(denv, item.Item)

                    let name, nameInCode =
                        if displayName.StartsWith "( " && displayName.EndsWith " )" then
                            let cleanName = displayName.[2..displayName.Length - 3]
                            cleanName, 
                            if IsOperatorName displayName then cleanName else "``" + cleanName + "``"
                        else 
                            displayName,
                            match item.Unresolved with
                            | Some _ -> displayName
                            | None -> Lexhelp.Keywords.QuoteIdentifierIfNeeded displayName

                    let isAttribute = ItemDescriptionsImpl.IsAttribute infoReader item.Item
                    
                    let cutAttributeSuffix (name: string) =
                        if isAttributeApplicationContext && isAttribute && name <> "Attribute" && name.EndsWith "Attribute" then
                            name.[0..name.Length - "Attribute".Length - 1]
                        else name

                    let name = cutAttributeSuffix name
                    let nameInCode = cutAttributeSuffix nameInCode
                    let fullName = ItemDescriptionsImpl.FullNameOfItem g item.Item
                    
                    let namespaceToOpen = 
                        item.Unresolved 
                        |> Option.map (fun x -> x.Namespace)
                        |> Option.bind (fun ns ->
                            if ns |> Array.startsWith [|"Microsoft"; "FSharp"|] then None
                            else Some ns)
                        |> Option.map (fun ns ->
                            match currentNamespaceOrModule with
                            | Some currentNs ->
                               if ns |> Array.startsWith currentNs then
                                 ns.[currentNs.Length..]
                               else ns
                            | None -> ns)
                        |> Option.bind (function
                            | [||] -> None
                            | ns -> Some (ns |> String.concat "."))

                    FSharpDeclarationListItem(
                        name, nameInCode, fullName, glyph, Choice1Of2 (items, infoReader, m, denv, reactor, checkAlive), getAccessibility item.Item, 
                        item.Kind, item.IsOwnMember, item.MinorPriority, item.Unresolved.IsNone, namespaceToOpen))

        new FSharpDeclarationListInfo(Array.ofList decls, isForType, false)
    
    static member Error msg = 
        new FSharpDeclarationListInfo(
                [| FSharpDeclarationListItem("<Note>", "<Note>", "<Note>", FSharpGlyph.Error, Choice2Of2 (FSharpToolTipText [FSharpStructuredToolTipElement.CompositionError msg]), 
                                             None, CompletionItemKind.Other, false, 0, false, None) |], false, true)
    
    static member Empty = FSharpDeclarationListInfo([| |], false, false)



/// A single method for Intellisense completion
[<Sealed; NoEquality; NoComparison>]
// Note: instances of this type do not hold any references to any compiler resources.
type FSharpMethodGroupItem(description: FSharpToolTipText<Layout>, xmlDoc: FSharpXmlDoc, returnType: Layout, parameters: FSharpMethodGroupItemParameter[], hasParameters: bool, hasParamArrayArg: bool, staticParameters: FSharpMethodGroupItemParameter[]) = 
    member __.StructuredDescription = description
    member __.Description = Tooltips.ToFSharpToolTipText description
    member __.XmlDoc = xmlDoc
    member __.StructuredReturnTypeText = returnType
    member __.ReturnTypeText = showL returnType
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
                FSharpMethodGroupItem(meth.StructuredDescription, meth.XmlDoc, meth.StructuredReturnTypeText, [||], true, meth.HasParamArrayArg, meth.StaticParameters) 
            else 
                meth)
        // Fix the order of methods, to be stable for unit testing.
        |> Array.sortBy (fun meth -> 
            let parms = meth.Parameters
            parms.Length, (parms |> Array.map (fun p -> p.CanonicalTypeTextForSorting)))

    member __.MethodName = name

    member __.Methods = methods

    static member Create (infoReader: InfoReader, m, denv, items:ItemWithInst list) = 
        let g = infoReader.g
        if isNil items then new FSharpMethodGroup("", [| |]) else
        let name = items.Head.Item.DisplayName 

        let methods = 
          [| for item in items do 
#if !FX_NO_WEAKTABLE
               match methodOverloadsCache.TryGetValue item with
               | true, res -> yield! res
               | false, _ ->
#endif
                let flatItems = AnotherFlattenItems g  m item.Item

                let methods = 
                    flatItems |> Array.ofList |> Array.map (fun flatItem -> 
                        let prettyParams, prettyRetTyL = 
                            ErrorScope.Protect m 
                                (fun () -> PrettyParamsAndReturnTypeOfItem infoReader m denv  { item with Item = flatItem })
                                (fun err -> [], wordL (tagText err))
                            
                        let description = FSharpToolTipText [FormatStructuredDescriptionOfItem true infoReader m denv { item with Item = flatItem }]

                        let hasParamArrayArg = 
                            match flatItem with 
                            | Item.CtorGroup(_,[meth]) 
                            | Item.MethodGroup(_,[meth],_) -> meth.HasParamArrayArg(infoReader.amap, m, meth.FormalMethodInst) 
                            | _ -> false

                        let hasStaticParameters = 
                            match flatItem with 
                            | ItemIsProvidedTypeWithStaticArguments m g _ -> false 
                            | _ -> true

                        FSharpMethodGroupItem(
                          description = description,
                          returnType = prettyRetTyL,
                          xmlDoc = GetXmlCommentForItem infoReader m flatItem,
                          parameters = (prettyParams |> Array.ofList),
                          hasParameters = hasStaticParameters,
                          hasParamArrayArg = hasParamArrayArg,
                          staticParameters = StaticParamsOfItem infoReader m denv flatItem
                        ))
#if !FX_NO_WEAKTABLE
                methodOverloadsCache.Add(item, methods)
#endif
                yield! methods 
           |]

        new FSharpMethodGroup(name, methods)



