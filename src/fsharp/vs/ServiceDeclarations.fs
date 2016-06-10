// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Open up the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.
//--------------------------------------------------------------------------

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open System
open System.Collections.Generic
open System.IO
open System.Text

open Microsoft.FSharp.Core.Printf
open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.AbstractIL.IL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library  
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics 

open Microsoft.FSharp.Compiler.AccessibilityLogic
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.Layout
open Microsoft.FSharp.Compiler.Lib
open Microsoft.FSharp.Compiler.PrettyNaming
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.Tastops
open Microsoft.FSharp.Compiler.TcGlobals 
open Microsoft.FSharp.Compiler.Infos
open Microsoft.FSharp.Compiler.NameResolution
open Microsoft.FSharp.Compiler.InfoReader
open Microsoft.FSharp.Compiler.SourceCodeServices.ItemDescriptionIcons 

module EnvMisc2 =
    let maxMembers   = GetEnvInteger "FCS_MaxMembersInQuickInfo" 10

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

/// Describe a comment as either a block of text or a file+signature reference into an intellidoc file.
[<RequireQualifiedAccess>]
type FSharpXmlDoc =
    | None
    | Text of string
    | XmlDocFileSignature of (*File and Signature*) string * string

/// A single data tip display element
[<RequireQualifiedAccess>]
type FSharpToolTipElement = 
    | None
    /// A single type, method, etc with comment.
    | Single of (* text *) string * FSharpXmlDoc
    /// A single parameter, with the parameter name.
    | SingleParameter of (* text *) string * FSharpXmlDoc * string
    /// For example, a method overload group.
    | Group of ((* text *) string * FSharpXmlDoc) list
    /// An error occurred formatting this element
    | CompositionError of string

/// Information for building a data tip box.
//
// Note: this type does not hold any handles to compiler data structure.
type FSharpToolTipText = 
    /// A list of data tip elements to display.
    | FSharpToolTipText of FSharpToolTipElement list  


[<AutoOpen>]
module internal ItemDescriptionsImpl = 

    let isFunction g typ =
        let _,tau = tryDestForallTy g typ
        isFunTy g tau 

     
    let OutputFullName isDecl ppF fnF os r = 
      // Only display full names in quick info, not declaration text
      if not isDecl then 
        match ppF r with 
        | None -> ()
        | Some _ -> 
            bprintf os "\n\n%s: %s" (FSComp.SR.typeInfoFullName()) (fnF r)
          
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
        |   ProvidedProp(_,pi,_) -> definitionLocationOfProvidedItem pi
#endif
        |   _ -> pinfo.ArbitraryValRef |> Option.map (rangeOfValRef preferFlag)

    let rangeOfMethInfo (g:TcGlobals) preferFlag (minfo:MethInfo) = 
        match minfo with
#if EXTENSIONTYPING 
        |   ProvidedMeth(_,mi,_,_) -> definitionLocationOfProvidedItem mi
#endif
        |   DefaultStructCtor(_, AppTy g (tcref, _)) -> Some(rangeOfEntityRef preferFlag tcref)
        |   _ -> minfo.ArbitraryValRef |> Option.map (rangeOfValRef preferFlag)

    let rangeOfEventInfo preferFlag (einfo:EventInfo) = 
        match einfo with
#if EXTENSIONTYPING 
        | ProvidedEvent (_,ei,_) -> definitionLocationOfProvidedItem ei
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
            |> Option.orElse (fun () -> minfo.DeclaringEntityRef |> computeCcuOfTyconRef)


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
                |> Option.orElse (fun () -> pinfo.EnclosingType |> tcrefOfAppTy g |> computeCcuOfTyconRef))

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
    let rec GetXmlDocHelpSigOfItemForLookup (infoReader:InfoReader) m d = 
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

    /// Produce an XmlComment with a signature or raw text.
    let GetXmlComment (xmlDoc:XmlDoc) (infoReader:InfoReader) m d = 
        let result = 
            match xmlDoc with 
            | XmlDoc [| |] -> ""
            | XmlDoc l -> 
                bufs (fun os -> 
                    bprintf os "\n"; 
                    l |> Array.iter (fun (s:string) -> 
                        // Note: this code runs for local/within-project xmldoc tooltips, but not for cross-project or .XML
                        bprintf os "\n%s" s))

        let xml = if String.IsNullOrEmpty result then FSharpXmlDoc.None else FSharpXmlDoc.Text result
        match xml with
        | FSharpXmlDoc.None -> GetXmlDocHelpSigOfItemForLookup infoReader m d
        | _ -> xml

    let mutable ToolTipFault  = None
    
    /// Output a method info
    let FormatOverloadsToList (infoReader:InfoReader) m denv d minfos : FSharpToolTipElement = 
        let formatOne minfo = 
            let text = bufs (fun os -> NicePrint.formatMethInfoToBufferFreeStyle  infoReader.amap m denv os minfo)
            let xml = GetXmlComment (if minfo.HasDirectXmlComment then minfo.XmlDoc else XmlDoc [||]) infoReader m d 
            text,xml

        ToolTipFault |> Option.iter (fun msg -> 
           let exn = Error((0,msg),range.Zero)
           let ph = PhasedError.Create(exn, BuildPhase.TypeCheck)
           phasedError ph)
 
        FSharpToolTipElement.Group(minfos |> List.map formatOne)

        
    let pubpath_of_vref         (v:ValRef) = v.PublicPath        
    let pubpath_of_tcref        (x:TyconRef) = x.PublicPath


    // Wrapper type for use by the 'partialDistinctBy' function
    [<StructuralEquality; NoComparison>]
    type WrapType<'T> = Wrap of 'T
    
    // Like Seq.distinctBy but only filters out duplicates for some of the elements
    let partialDistinctBy (per:IPartialEqualityComparer<_>) seq =
        // Wrap a Wrap _ around all keys in case the key type is itself a type using null as a representation
        let dict = new Dictionary<WrapType<'T>,obj>(per)
        seq |> List.filter (fun v -> 
            let v = Wrap(v)
            if (per.InEqualityRelation(v)) then 
                if dict.ContainsKey(v) then false else (dict.[v] <- null; true)
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
              | Wrap(Item.Types(_,[_])) -> true
              | Wrap(Item.ILField(ILFieldInfo _)) -> true
              | Wrap(Item.RecdField _) -> true
              | Wrap(Item.SetterArg _) -> true
              | Wrap(Item.TypeVar _) -> true
              | Wrap(Item.CustomOperation _) -> true
              | Wrap(Item.ModuleOrNamespaces(_ :: _)) -> true
              | Wrap(Item.MethodGroup _) -> true
              | Wrap(Item.Value _ | Item.CustomBuilder _) -> true
              | Wrap(Item.ActivePatternCase _) -> true
              | Wrap(Item.DelegateCtor _) -> true
              | Wrap(Item.UnionCase _) -> true
              | Wrap(Item.ExnCase _) -> true              
              | Wrap(Item.Event _) -> true
              | Wrap(Item.Property _) -> true
              | Wrap(Item.CtorGroup _) -> true
              | _ -> false
              
          member x.Equals(item1, item2) = 
            // This may explore assemblies that are not in the reference set.
            // In this case just bail out and assume items are not equal
            protectAssemblyExploration false (fun () -> 
              let equalTypes(ty1, ty2) =
                  if isAppTy g ty1 && isAppTy g ty2 then tyconRefEq g (tcrefOfAppTy g ty1) (tcrefOfAppTy g ty2) 
                  else typeEquiv g ty1 ty2
              match item1,item2 with 
              | Wrap(Item.DelegateCtor(ty1)), Wrap(Item.DelegateCtor(ty2)) -> equalTypes(ty1, ty2)
              | Wrap(Item.Types(dn1,[ty1])), Wrap(Item.Types(dn2,[ty2])) -> 
                  // Bug 4403: We need to compare names as well, because 'int' and 'Int32' are physically the same type, but we want to show both
                  dn1 = dn2 && equalTypes(ty1, ty2) 
              
              // Prefer a type to a DefaultStructCtor, a DelegateCtor and a FakeInterfaceCtor 
              | Wrap(ItemWhereTypIsPreferred(ty1)), Wrap(ItemWhereTypIsPreferred(ty2)) -> equalTypes(ty1, ty2) 

              | Wrap(Item.ExnCase(tcref1)), Wrap(Item.ExnCase(tcref2)) -> tyconRefEq g tcref1 tcref2
              | Wrap(Item.ILField(ILFieldInfo(_, fld1))), Wrap(Item.ILField(ILFieldInfo(_, fld2))) -> 
                  fld1 === fld2 // reference equality on the object identity of the AbstractIL metadata blobs for the fields
              | Wrap(Item.CustomOperation (_,_,Some minfo1)), Wrap(Item.CustomOperation (_,_,Some minfo2)) -> 
                    MethInfo.MethInfosUseIdenticalDefinitions minfo1 minfo2
              | Wrap(Item.TypeVar (nm1,tp1)), Wrap(Item.TypeVar (nm2,tp2)) -> 
                    (nm1 = nm2) && typarRefEq tp1 tp2
              | Wrap(Item.ModuleOrNamespaces(modref1 :: _)), Wrap(Item.ModuleOrNamespaces(modref2 :: _)) -> fullDisplayTextOfModRef modref1 = fullDisplayTextOfModRef modref2
              | Wrap(Item.SetterArg(id1,_)), Wrap(Item.SetterArg(id2,_)) -> (id1.idRange, id1.idText) = (id2.idRange, id2.idText)
              | Wrap(Item.MethodGroup(_, meths1,_)), Wrap(Item.MethodGroup(_, meths2,_)) -> 
                  Seq.zip meths1 meths2 |> Seq.forall (fun (minfo1, minfo2) ->
                    MethInfo.MethInfosUseIdenticalDefinitions minfo1 minfo2)
              | Wrap(Item.Value vref1 | Item.CustomBuilder (_,vref1)), Wrap(Item.Value vref2 | Item.CustomBuilder (_,vref2)) -> valRefEq g vref1 vref2
              | Wrap(Item.ActivePatternCase(APElemRef(_apinfo1, vref1, idx1))), Wrap(Item.ActivePatternCase(APElemRef(_apinfo2, vref2, idx2))) ->
                  idx1 = idx2 && valRefEq g vref1 vref2
              | Wrap(Item.UnionCase(UnionCaseInfo(_, ur1),_)), Wrap(Item.UnionCase(UnionCaseInfo(_, ur2),_)) -> g.unionCaseRefEq ur1 ur2
              | Wrap(Item.RecdField(RecdFieldInfo(_, RFRef(tcref1, n1)))), Wrap(Item.RecdField(RecdFieldInfo(_, RFRef(tcref2, n2)))) -> 
                  (tyconRefEq g tcref1 tcref2) && (n1 = n2) // there is no direct function as in the previous case
              | Wrap(Item.Property(_, pi1s)), Wrap(Item.Property(_, pi2s)) -> 
                  List.zip pi1s pi2s |> List.forall(fun (pi1, pi2) -> PropInfo.PropInfosUseIdenticalDefinitions pi1 pi2)
              | Wrap(Item.Event(evt1)), Wrap(Item.Event(evt2)) -> EventInfo.EventInfosUseIdenticalDefintions evt1 evt2
              | Wrap(Item.CtorGroup(_, meths1)), Wrap(Item.CtorGroup(_, meths2)) -> 
                  Seq.zip meths1 meths2 
                  |> Seq.forall (fun (minfo1, minfo2) -> MethInfo.MethInfosUseIdenticalDefinitions minfo1 minfo2)
              | _ -> false)
              
          member x.GetHashCode item =
            // This may explore assemblies that are not in the reference set.
            // In this case just bail out and use a random hash code
            protectAssemblyExploration 1027 (fun () -> 
              match item with 
              | Wrap(ItemWhereTypIsPreferred ty) -> 
                  if isAppTy g ty then hash (tcrefOfAppTy g ty).Stamp
                  else 1010
              | Wrap(Item.ILField(ILFieldInfo(_, fld))) -> 
                  System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode fld // hash on the object identity of the AbstractIL metadata blob for the field
              | Wrap(Item.TypeVar (nm,_tp)) -> hash nm
              | Wrap(Item.CustomOperation (_,_,Some minfo)) -> minfo.ComputeHashCode()
              | Wrap(Item.CustomOperation (_,_,None)) -> 1
              | Wrap(Item.ModuleOrNamespaces(modref :: _)) -> hash (fullDisplayTextOfModRef modref)          
              | Wrap(Item.SetterArg(id,_)) -> hash (id.idRange, id.idText)
              | Wrap(Item.MethodGroup(_, meths,_)) -> meths |> List.fold (fun st a -> st + a.ComputeHashCode()) 0
              | Wrap(Item.CtorGroup(name, meths)) -> name.GetHashCode() + (meths |> List.fold (fun st a -> st + a.ComputeHashCode()) 0)
              | Wrap(Item.Value vref | Item.CustomBuilder (_,vref)) -> hash vref.LogicalName
              | Wrap(Item.ActivePatternCase(APElemRef(_apinfo, vref, idx))) -> hash (vref.LogicalName, idx)
              | Wrap(Item.ExnCase(tcref)) -> hash tcref.Stamp
              | Wrap(Item.UnionCase(UnionCaseInfo(_, UCRef(tcref, n)),_)) -> hash(tcref.Stamp, n)
              | Wrap(Item.RecdField(RecdFieldInfo(_, RFRef(tcref, n)))) -> hash(tcref.Stamp, n)
              | Wrap(Item.Event evt) -> evt.ComputeHashCode()
              | Wrap(Item.Property(_name, pis)) -> hash (pis |> List.map (fun pi -> pi.ComputeHashCode()))
              | _ -> failwith "unreachable") }
    
    // Remove items containing the same module references
    let RemoveDuplicateModuleRefs modrefs  = 
        modrefs |> partialDistinctBy 
                      { new IPartialEqualityComparer<WrapType<ModuleOrNamespaceRef>> with
                          member x.InEqualityRelation _ = true
                          member x.Equals(Wrap(item1), Wrap(item2)) = (fullDisplayTextOfModRef item1 = fullDisplayTextOfModRef item2)
                          member x.GetHashCode(Wrap(item)) = hash item.Stamp  }

    /// Remove all duplicate items
    let RemoveDuplicateItems g items = 
        items |> partialDistinctBy (ItemDisplayPartialEquality g) 

    /// Filter types that are explicitly suppressed from the IntelliSense (such as uppercase "FSharpList", "Option", etc.)
    let RemoveExplicitlySuppressed g items = 
      items |> List.filter (fun item ->
        // This may explore assemblies that are not in the reference set.
        // In this case just assume the item is not suppressed.
        protectAssemblyExploration true (fun () -> 
         match item with 
         | Item.Types(it, [ty]) -> 
             g.suppressed_types |> List.forall (fun supp -> 
                if isAppTy g ty then 
                  // check if they are the same logical type (after removing all abbreviations)
                  let tcr1 = tcrefOfAppTy g ty
                  let tcr2 = tcrefOfAppTy g (generalizedTyconRef supp) 
                  not(tyconRefEq g tcr1 tcr2 && 
                      // check the display name is precisely the one we're suppressing
                      it = supp.DisplayName)
                else true ) 
         | _ -> true ))
    
    let SimplerDisplayEnv denv _isDecl = 
        { denv with suppressInlineKeyword=true; 
                    shortConstraints=true; 
                    showConstraintTyparAnnotations=false; 
                    abbreviateAdditionalConstraints=false;
                    suppressNestedTypes=true;
                    maxMembers=Some EnvMisc2.maxMembers }

    let rec FullNameOfItem g d = 
        let denv = DisplayEnv.Empty(g)
        match d with
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
        | Item.Types(_,typ:: _) -> bufs (fun os -> NicePrint.outputTyconRef denv os (tcrefOfAppTy g typ))
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
    let rec FormatItemDescriptionToToolTipElement isDecl (infoReader:InfoReader) m denv d = 
        let g = infoReader.g
        let amap = infoReader.amap
        let denv = SimplerDisplayEnv denv isDecl 
        match d with
        | Item.ImplicitOp(_, { contents = Some(TraitConstraintSln.FSMethSln(_, vref, _)) }) -> 
            // operator with solution
            FormatItemDescriptionToToolTipElement isDecl infoReader m denv (Item.Value vref)
        | Item.Value vref | Item.CustomBuilder (_,vref) ->            
            let text = 
                bufs (fun os -> 
                    NicePrint.outputQualifiedValOrMember denv os vref.Deref 
                    OutputFullName isDecl pubpath_of_vref fullDisplayTextOfValRef os vref)

            let xml = GetXmlComment (if (valRefInThisAssembly g.compilingFslib vref) then vref.XmlDoc else XmlDoc [||]) infoReader m d 
            FSharpToolTipElement.Single(text, xml)

        // Union tags (constructors)
        | Item.UnionCase(ucinfo,_) -> 
            let uc = ucinfo.UnionCase 
            let rty = generalizedTyconRef ucinfo.TyconRef
            let recd = uc.RecdFields 
            let text = 
                bufs (fun os -> 
                    bprintf os "%s "  (FSComp.SR.typeInfoUnionCase())
                    NicePrint.outputTyconRef denv os ucinfo.TyconRef
                    bprintf os ".%s: "  
                        (DecompileOpName uc.Id.idText) 
                    if not (isNil recd) then
                        NicePrint.outputUnionCases denv os recd
                        os.Append (" -> ") |> ignore
                    NicePrint.outputTy denv os rty )


            let xml = GetXmlComment (if (tyconRefUsesLocalXmlDoc g.compilingFslib ucinfo.TyconRef) then uc.XmlDoc else XmlDoc [||]) infoReader m d 
            FSharpToolTipElement.Single(text, xml)

        // Active pattern tag inside the declaration (result)             
        | Item.ActivePatternResult(apinfo, ty, idx, _) ->
            let items = apinfo.ActiveTags
            let text = bufs (fun os -> 
                bprintf os "%s %s: " (FSComp.SR.typeInfoActivePatternResult()) (List.item idx items) 
                NicePrint.outputTy denv os ty)
            let xml = GetXmlComment (XmlDoc [||]) infoReader m d
            FSharpToolTipElement.Single(text, xml)

        // Active pattern tags 
        // XmlDoc is never emitted to xml doc files for these
        | Item.ActivePatternCase apref -> 
            let v = apref.ActivePatternVal
            // Format the type parameters to get e.g. ('a -> 'a) rather than ('?1234 -> '?1234)
            let _,tau = v.TypeScheme
            // REVIEW: use _cxs here
            let _, ptau, _cxs = PrettyTypes.PrettifyTypes1 denv.g tau
            let text = 
                bufs (fun os -> 
                    bprintf os "%s %s: " (FSComp.SR.typeInfoActiveRecognizer())
                        apref.Name
                    NicePrint.outputTy denv os ptau 
                    OutputFullName isDecl pubpath_of_vref fullDisplayTextOfValRef os v)

            let xml = GetXmlComment v.XmlDoc infoReader m d 
            FSharpToolTipElement.Single(text, xml)

        // F# exception names
        | Item.ExnCase ecref -> 
            let text =  bufs (fun os -> 
                NicePrint.outputExnDef denv os ecref.Deref 
                OutputFullName isDecl pubpath_of_tcref fullDisplayTextOfExnRef os ecref)
            let xml = GetXmlComment (if (tyconRefUsesLocalXmlDoc g.compilingFslib ecref) then ecref.XmlDoc else XmlDoc [||]) infoReader m d 
            FSharpToolTipElement.Single(text, xml)

        // F# record field names
        | Item.RecdField rfinfo ->
            let rfield = rfinfo.RecdField
            let _, ty, _cxs = PrettyTypes.PrettifyTypes1 g rfinfo.FieldType
            let text = 
                bufs (fun os -> 
                    NicePrint.outputTyconRef denv os rfinfo.TyconRef
                    bprintf os ".%s: "  
                        (DecompileOpName rfield.Name) 
                    NicePrint.outputTy denv os ty;
                    match rfinfo.LiteralValue with 
                    | None -> ()
                    | Some lit -> 
                       try bprintf os " = %s" (Layout.showL ( NicePrint.layoutConst denv.g ty lit )) with _ -> ())

            let xml = GetXmlComment (if (tyconRefUsesLocalXmlDoc g.compilingFslib rfinfo.TyconRef) then rfield.XmlDoc else XmlDoc [||]) infoReader m d 
            FSharpToolTipElement.Single(text, xml)

        // Not used
        | Item.NewDef id -> 
            let dataTip = bufs (fun os -> bprintf os "%s %s" (FSComp.SR.typeInfoPatternVariable()) id.idText)
            FSharpToolTipElement.Single(dataTip, GetXmlComment (XmlDoc [||]) infoReader m d)

        // .NET fields
        | Item.ILField finfo ->
            let dataTip = bufs (fun os -> 
                bprintf os "%s " (FSComp.SR.typeInfoField()) 
                NicePrint.outputILTypeRef denv os finfo.ILTypeRef
                bprintf os ".%s" finfo.FieldName;
                match finfo.LiteralValue with 
                | None -> ()
                | Some v -> 
                   try bprintf os " = %s" (Layout.showL ( NicePrint.layoutConst denv.g (finfo.FieldType(infoReader.amap, m)) (TypeChecker.TcFieldInit m v) )) 
                   with _ -> ())
            FSharpToolTipElement.Single(dataTip, GetXmlComment (XmlDoc [||]) infoReader m d)

        // .NET events
        | Item.Event einfo ->
            let rty = PropTypOfEventInfo infoReader m AccessibleFromSomewhere einfo
            let _,rty, _cxs = PrettyTypes.PrettifyTypes1 g rty
            let text = 
                bufs (fun os -> 
                    // REVIEW: use _cxs here
                    bprintf os "%s " (FSComp.SR.typeInfoEvent()) 
                    NicePrint.outputTyconRef denv os (tcrefOfAppTy g einfo.EnclosingType) 
                    bprintf os ".%s: " einfo.EventName
                    NicePrint.outputTy denv os rty)

            let xml = GetXmlComment (if einfo.HasDirectXmlComment  then einfo.XmlDoc else XmlDoc [||]) infoReader m d 

            FSharpToolTipElement.Single(text, xml)

        // F# and .NET properties
        | Item.Property(_,pinfos) -> 
            let pinfo = pinfos.Head
            let rty = pinfo.GetPropertyType(amap,m) 
            let rty = if pinfo.IsIndexer then mkRefTupledTy g (pinfo.GetParamTypes(amap, m)) --> rty else  rty 
            let _, rty, _ = PrettyTypes.PrettifyTypes1 g rty
            let text =
                bufs (fun os -> 
                    bprintf os "%s " (FSComp.SR.typeInfoProperty())
                    NicePrint.outputTyconRef denv os (tcrefOfAppTy g pinfo.EnclosingType)
                    bprintf os ".%s: " pinfo.PropertyName  
                    NicePrint.outputTy denv os rty)

            let xml = GetXmlComment (if pinfo.HasDirectXmlComment then pinfo.XmlDoc else XmlDoc [||]) infoReader m d 

            FSharpToolTipElement.Single(text, xml)

        // Custom operations in queries
        | Item.CustomOperation (customOpName,usageText,Some minfo) -> 

            // Build 'custom operation: where (bool)
            //        
            //        Calls QueryBuilder.Where'
            let text =
                bufs (fun os -> 
                    bprintf os "%s: " (FSComp.SR.typeInfoCustomOperation()) 
                    match usageText() with 
                    | Some t -> 
                        bprintf os "%s" t
                    | None -> 
                        let argTys = ParamNameAndTypesOfUnaryCustomOperation g minfo |> List.map (fun (ParamNameAndType(_,ty)) -> ty)
                        let _, argTys, _ = PrettyTypes.PrettifyTypesN g argTys 

                        bprintf os "%s" customOpName
                        for argTy in argTys do
                            bprintf os " ("
                            NicePrint.outputTy denv os argTy
                            bprintf os ")" 
                    bprintf os "\n\n%s "
                        (FSComp.SR.typeInfoCallsWord()) 
                    NicePrint.outputTyconRef denv os (tcrefOfAppTy g minfo.EnclosingType)
                    bprintf os ".%s " 
                        minfo.DisplayName)

            let xml = GetXmlComment (if minfo.HasDirectXmlComment then minfo.XmlDoc else XmlDoc [||]) infoReader m d 

            FSharpToolTipElement.Single(text, xml)

        // F# constructors and methods
        | Item.CtorGroup(_,minfos) 
        | Item.MethodGroup(_,minfos,_) ->
            FormatOverloadsToList infoReader m denv d minfos
        
        // The 'fake' zero-argument constructors of .NET interfaces.
        // This ideally should never appear in intellisense, but we do get here in repros like:
        //     type IFoo = abstract F : int
        //     type II = IFoo  // remove 'type II = ' and quickly hover over IFoo before it gets squiggled for 'invalid use of interface type'
        // and in that case we'll just show the interface type name.
        | Item.FakeInterfaceCtor typ ->
           let _, typ, _ = PrettyTypes.PrettifyTypes1 g typ
           let text = bufs (fun os -> NicePrint.outputTyconRef denv os (tcrefOfAppTy g typ))
           FSharpToolTipElement.Single(text, GetXmlComment (XmlDoc [||]) infoReader m d)
        
        // The 'fake' representation of constructors of .NET delegate types
        | Item.DelegateCtor delty -> 
           let _, delty, _cxs = PrettyTypes.PrettifyTypes1 g delty
           let (SigOfFunctionForDelegate(_, _, _, fty)) = GetSigOfFunctionForDelegate infoReader delty m AccessibleFromSomewhere
           let text = bufs (fun os -> 
                         NicePrint.outputTyconRef denv os (tcrefOfAppTy g delty)
                         bprintf os "("
                         NicePrint.outputTy denv os fty
                         bprintf os ")")
           let xml = GetXmlComment (XmlDoc [||]) infoReader m d
           FSharpToolTipElement.Single(text, xml)

        // Types.
        | Item.Types(_,((TType_app(tcref,_)):: _)) -> 
            let text = 
                bufs (fun os -> 
                    let denv = { denv with shortTypeNames = true  }
                    NicePrint.outputTycon denv infoReader AccessibleFromSomewhere m (* width *) os tcref.Deref
                    OutputFullName isDecl pubpath_of_tcref fullDisplayTextOfTyconRef os tcref)
  
            let xml = GetXmlComment (if (tyconRefUsesLocalXmlDoc g.compilingFslib tcref) then tcref.XmlDoc else XmlDoc [||]) infoReader m d 
            FSharpToolTipElement.Single(text, xml)

        // F# Modules and namespaces
        | Item.ModuleOrNamespaces((modref :: _) as modrefs) -> 
            let os = StringBuilder()
            let modrefs = modrefs |> RemoveDuplicateModuleRefs
            let definiteNamespace = modrefs |> List.forall (fun modref -> modref.IsNamespace)
            let kind = 
                if definiteNamespace then FSComp.SR.typeInfoNamespace()
                elif modrefs |> List.forall (fun modref -> modref.IsModule) then FSComp.SR.typeInfoModule()
                else FSComp.SR.typeInfoNamespaceOrModule()
            bprintf os "%s %s" kind (if definiteNamespace then fullDisplayTextOfModRef modref else modref.DemangledModuleOrNamespaceName)
            if not definiteNamespace then
                let namesToAdd = 
                    ([],modrefs) 
                    ||> Seq.fold (fun st modref -> 
                        match fullDisplayTextOfParentOfModRef modref with 
                        | Some(txt) -> txt::st 
                        | _ -> st) 
                    |> Seq.mapi (fun i x -> i,x) 
                    |> Seq.toList
                if nonNil namesToAdd then 
                    bprintf os "\n"
                for i, txt in namesToAdd do
                    bprintf os "\n%s" ((if i = 0 then FSComp.SR.typeInfoFromFirst else FSComp.SR.typeInfoFromNext) txt)
                let xml = GetXmlComment (if (entityRefInThisAssembly g.compilingFslib modref) then modref.XmlDoc else XmlDoc [||]) infoReader m d 
                FSharpToolTipElement.Single(os.ToString(), xml)
            else
                FSharpToolTipElement.Single(os.ToString(), GetXmlComment (XmlDoc [||]) infoReader m d)

        // Named parameters
        | Item.ArgName (id, argTy, argContainer) -> 
            let _, argTy, _ = PrettyTypes.PrettifyTypes1 g argTy
            let text = bufs (fun os -> 
                          bprintf os "%s %s : " (FSComp.SR.typeInfoArgument()) id.idText 
                          NicePrint.outputTy denv os argTy)

            let xmldoc = match argContainer with
                         | Some(ArgumentContainer.Method (minfo)) ->
                               if minfo.HasDirectXmlComment then minfo.XmlDoc else XmlDoc [||] 
                         | Some(ArgumentContainer.Type(tcref)) ->
                               if (tyconRefUsesLocalXmlDoc g.compilingFslib tcref) then tcref.XmlDoc else XmlDoc [||]
                         | Some(ArgumentContainer.UnionCase(ucinfo)) ->
                               if (tyconRefUsesLocalXmlDoc g.compilingFslib ucinfo.TyconRef) then ucinfo.UnionCase.XmlDoc else XmlDoc [||]
                         | _ -> XmlDoc [||]
            let xml = GetXmlComment xmldoc infoReader m d
            FSharpToolTipElement.SingleParameter(text, xml, id.idText)
            
        | Item.SetterArg (_, item) -> 
            FormatItemDescriptionToToolTipElement isDecl infoReader m denv item
        |  _ -> 
            FSharpToolTipElement.None


    // Format the return type of an item
    let rec FormatItemReturnTypeToBuffer (infoReader:InfoReader) m denv os d = 
        let isDecl = false
        let g = infoReader.g
        let amap = infoReader.amap
        let denv = {SimplerDisplayEnv denv isDecl with useColonForReturnType=true}
        match d with
        | Item.Value vref | Item.CustomBuilder (_,vref) -> 
            let _, tau = vref.TypeScheme
            (* Note: prettify BEFORE we strip to make sure params look the same as types *)
            if isFunTy g tau then 
              let dtau,rtau = destFunTy g tau
              let ptausL,tpcsL = NicePrint.layoutPrettifiedTypes denv [dtau;rtau]
              let _,prtauL = List.frontAndBack ptausL
              bprintf os ": "
              bufferL os prtauL
              bprintf os " "
              bufferL os tpcsL
            else
              bufferL os (NicePrint.layoutPrettifiedTypeAndConstraints denv [] tau) 
        | Item.UnionCase(ucinfo,_) -> 
            let rty = generalizedTyconRef ucinfo.TyconRef
            NicePrint.outputTy denv os rty
        | Item.ActivePatternCase(apref) -> 
            let v = apref.ActivePatternVal
            let _, tau = v.TypeScheme
            let _, res = stripFunTy g tau
            let apinfo = Option.get (TryGetActivePatternInfo v)
            let apnames = apinfo.Names
            let aparity = apnames.Length
            
            let rty = if aparity <= 1 then res else List.item apref.CaseIndex (argsOfAppTy g res)
            NicePrint.outputTy denv os rty
        | Item.ExnCase _ -> 
            bufferL os (NicePrint.layoutPrettifiedTypeAndConstraints denv [] g.exn_ty) 
        | Item.RecdField(rfinfo) ->
            bufferL os (NicePrint.layoutPrettifiedTypeAndConstraints denv [] rfinfo.FieldType);
        | Item.ILField(finfo) ->
            bufferL os (NicePrint.layoutPrettifiedTypeAndConstraints denv [] (finfo.FieldType(amap,m)))
        | Item.Event(einfo) ->
            bufferL os (NicePrint.layoutPrettifiedTypeAndConstraints denv [] (PropTypOfEventInfo infoReader m AccessibleFromSomewhere einfo))
        | Item.Property(_,pinfos) -> 
            let pinfo = List.head pinfos
            let rty = pinfo.GetPropertyType(amap,m) 
            let layout = (NicePrint.layoutPrettifiedTypeAndConstraints denv [] rty)
            bufferL os layout
        | Item.CustomOperation (_,_,Some minfo)
        | Item.MethodGroup(_,(minfo :: _),_) 
        | Item.CtorGroup(_,(minfo :: _)) -> 
            let rty = minfo.GetFSharpReturnTy(amap, m, minfo.FormalMethodInst)
            bufferL os (NicePrint.layoutPrettifiedTypeAndConstraints denv [] rty) 
        | Item.FakeInterfaceCtor typ 
        | Item.DelegateCtor typ -> 
           bufferL os (NicePrint.layoutPrettifiedTypeAndConstraints denv [] typ) 
        | Item.TypeVar _ -> ()
            
        | _ -> ()

    let rec GetF1Keyword d : string option = 
        let rec unwindTypeAbbrev (tcref : TyconRef) =
            match tcref.TypeAbbrev with
            |   None -> Some tcref
            |   Some typ ->
                    match typ with
                    |   TType_app(tcref, _) -> unwindTypeAbbrev tcref
                    |   _ -> None
        
        let getKeywordForValRef (vref : ValRef) =
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
             
        match d with
        | Item.Value vref | Item.CustomBuilder (_,vref) -> getKeywordForValRef vref
        | Item.ActivePatternCase apref -> apref.ActivePatternVal |> getKeywordForValRef

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
        | Item.Types(_,((TType_app(tcref,_)) :: _)) 
        | Item.DelegateCtor(TType_app(tcref,_))
        | Item.FakeInterfaceCtor(TType_app(tcref,_))
        | Item.UnqualifiedType (tcref::_)
        | Item.ExnCase tcref -> 
            unwindTypeAbbrev tcref |> Option.map ticksAndArgCountTextOfTyconRef 

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
                   // per spec, extension members in F1 keywords are qualified with definition class
                   match vref.ActualParent with 
                   | Parent tcref ->
                        (tcref |> ticksAndArgCountTextOfTyconRef)+"."+vref.PropertyName|> Some                     
                   | ParentNone -> None
                | None -> None
#if EXTENSIONTYPING
            | ProvidedEvent _ -> None 
#endif
        | Item.CtorGroup(_,minfos) ->
            match minfos with 
            | [] -> None
            | FSMeth(_, _, vref, _) :: _ ->
                   // per spec, extension members in F1 keywords are qualified with definition class
                   match vref.ActualParent with
                   | Parent tcref ->
                        (tcref |> ticksAndArgCountTextOfTyconRef) + ".#ctor"|> Some
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
        | Item.SetterArg (_, propOrField) -> GetF1Keyword propOrField 
        | Item.MethodGroup(_,[],_) 
        | Item.CustomOperation (_,_,None)   // "into"
        | Item.NewDef _ // "let x$yz = ..." - no keyword
        | Item.ArgName _ // no keyword on named parameters 
        | Item.TypeVar _ 
        | Item.ImplicitOp _
        | Item.ActivePatternResult _ // "let (|Foo|Bar|) = .. Fo$o ..." - no keyword
            ->  None

    let FormatDescriptionOfItem isDecl (infoReader:InfoReader)  m denv d : FSharpToolTipElement = 
        ErrorScope.Protect m 
            (fun () -> FormatItemDescriptionToToolTipElement isDecl infoReader m denv d)
            (fun err -> FSharpToolTipElement.CompositionError(err))
        
    let FormatReturnTypeOfItem (infoReader:InfoReader) m denv d = 
        ErrorScope.Protect m (fun () -> bufs (fun buf -> FormatItemReturnTypeToBuffer infoReader m denv buf d)) (fun err -> err)

    // Compute the index of the VS glyph shown with an item in the Intellisense menu
    let GlyphOfItem(denv,d) = 

         /// Find the glyph for the given representation.    
         let reprToGlyph repr = 
            match repr with
            | TFSharpObjectRepr om -> 
                match om.fsobjmodel_kind with 
                | TTyconClass -> iIconGroupClass
                | TTyconInterface -> iIconGroupInterface
                | TTyconStruct -> iIconGroupStruct
                | TTyconDelegate _ -> iIconGroupDelegate
                | TTyconEnum _ -> iIconGroupEnum
            | TRecdRepr _ -> iIconGroupType
            | TUnionRepr _ -> iIconGroupUnion
            | TILObjectRepr(_,_,td) -> 
                match td.tdKind with 
                | ILTypeDefKind.Class -> iIconGroupClass
                | ILTypeDefKind.ValueType -> iIconGroupStruct
                | ILTypeDefKind.Interface -> iIconGroupInterface
                | ILTypeDefKind.Enum -> iIconGroupEnum
                | ILTypeDefKind.Delegate -> iIconGroupDelegate
            | TAsmRepr _ -> iIconGroupTypedef
            | TMeasureableRepr _-> iIconGroupTypedef 
#if EXTENSIONTYPING
            | TProvidedTypeExtensionPoint _-> iIconGroupTypedef 
            | TProvidedNamespaceExtensionPoint  _-> iIconGroupTypedef  
#endif
            | TNoRepr -> iIconGroupClass  
         
         /// Find the glyph for the given type representation.
         let typeToGlyph typ = 
            if isAppTy denv.g typ then 
                let tcref = tcrefOfAppTy denv.g typ
                tcref.TypeReprInfo |> reprToGlyph 
            elif isAnyTupleTy denv.g typ then iIconGroupStruct
            elif isFunction denv.g typ then iIconGroupDelegate
            elif isTyparTy denv.g typ then iIconGroupStruct
            else iIconGroupTypedef

            
         /// Find the glyph for the given value representation.
         let ValueToGlyph typ = 
            if isFunction denv.g typ then iIconGroupMethod
            else iIconGroupConstant
              
         /// Find the major glyph of the given named item.       
         let namedItemToMajorGlyph item = 
            // This may explore assemblies that are not in the reference set,
            // e.g. for type abbreviations to types not in the reference set. 
            // In this case just use iIconGroupClass.
           protectAssemblyExploration  iIconGroupClass (fun () ->
              match item with 
              | Item.Value(vref) | Item.CustomBuilder (_,vref) -> ValueToGlyph(vref.Type)
              | Item.Types(_,typ::_) -> typeToGlyph (stripTyEqns denv.g typ)    
              | Item.UnionCase _
              | Item.ActivePatternCase _ -> iIconGroupEnumMember   
              | Item.ExnCase _ -> iIconGroupException   
              | Item.RecdField _ -> iIconGroupFieldBlue   
              | Item.ILField _ -> iIconGroupFieldBlue    
              | Item.Event _ -> iIconGroupEvent   
              | Item.Property _ -> iIconGroupProperty   
              | Item.CtorGroup _ 
              | Item.DelegateCtor _ 
              | Item.FakeInterfaceCtor _
              | Item.CustomOperation _
              | Item.MethodGroup _  -> iIconGroupMethod   
              | Item.TypeVar _ 
              | Item.Types _ -> iIconGroupClass   
              | Item.ModuleOrNamespaces(modref::_) -> 
                    if modref.IsNamespace then iIconGroupNameSpace else iIconGroupModule
              | Item.ArgName _ -> iIconGroupVariable
              | Item.SetterArg _ -> iIconGroupVariable
              | _ -> iIconGroupError)

         /// Find the minor glyph of the given named item.       
         let namedItemToMinorGlyph item = 
            // This may explore assemblies that are not in the reference set,
            // e.g. for type abbreviations to types not in the reference set. 
            // In this case just use iIconItemNormal.
           protectAssemblyExploration  iIconItemNormal (fun () ->
             match item with 
              | Item.Value(vref) when isFunction denv.g vref.Type -> iIconItemSpecial
              | _ -> iIconItemNormal)

         (6 * namedItemToMajorGlyph d) + namedItemToMinorGlyph d

     
/// An intellisense declaration
[<Sealed>]
type FSharpDeclarationListItem(name, glyph:int, info) =
    let mutable descriptionTextHolder:FSharpToolTipText option = None
    let mutable task = null

    member decl.Name = name

    member decl.DescriptionTextAsync = 
            match info with
            | Choice1Of2 (items, infoReader, m, denv, reactor:IReactorOperations, checkAlive) -> 
                    // reactor causes the lambda to execute on the background compiler thread, through the Reactor
                    reactor.EnqueueAndAwaitOpAsync ("DescriptionTextAsync", fun _ct -> 
                          // This is where we do some work which may touch TAST data structures owned by the IncrementalBuilder - infoReader, item etc. 
                          // It is written to be robust to a disposal of an IncrementalBuilder, in which case it will just return the empty string. 
                          // It is best to think of this as a "weak reference" to the IncrementalBuilder, i.e. this code is written to be robust to its
                          // disposal. Yes, you are right to scratch your head here, but this is ok.
                              if checkAlive() then FSharpToolTipText(items |> Seq.toList |> List.map (FormatDescriptionOfItem true infoReader m denv))
                              else FSharpToolTipText [ FSharpToolTipElement.Single(FSComp.SR.descriptionUnavailable(), FSharpXmlDoc.None) ])
            | Choice2Of2 result -> 
                async.Return result

    member decl.DescriptionText = 
        match descriptionTextHolder with
        | Some descriptionText -> descriptionText
        | None ->
            match info with
            | Choice1Of2 _ -> 

                // The dataTipSpinWaitTime limits how long we block the UI thread while a tooltip pops up next to a selected item in an IntelliSense completion list.
                // This time appears to be somewhat amortized by the time it takes the VS completion UI to actually bring up the tooltip after selecting an item in the first place.
                if task = null then
                    // kick off the actual (non-cooperative) work
                    task <- System.Threading.Tasks.Task.Factory.StartNew(fun() -> 
                        let text = decl.DescriptionTextAsync |> Async.RunSynchronously
                        descriptionTextHolder <- Some text) 

                // The dataTipSpinWaitTime limits how long we block the UI thread while a tooltip pops up next to a selected item in an IntelliSense completion list.
                // This time appears to be somewhat amortized by the time it takes the VS completion UI to actually bring up the tooltip after selecting an item in the first place.
                task.Wait EnvMisc2.dataTipSpinWaitTime  |> ignore
                match descriptionTextHolder with 
                | Some text -> text
                | None -> FSharpToolTipText [ FSharpToolTipElement.Single(FSComp.SR.loadingDescription(), FSharpXmlDoc.None) ]

            | Choice2Of2 result -> 
                result

    member decl.Glyph = glyph      
      
/// A table of declarations for Intellisense completion 
[<Sealed>]
type FSharpDeclarationListInfo(declarations: FSharpDeclarationListItem[]) = 

    member self.Items = declarations
    
    // Make a 'Declarations' object for a set of selected items
    static member Create(infoReader:InfoReader, m, denv, items, reactor, checkAlive) = 
        let g = infoReader.g
         
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
                | [Item.MethodGroup(_,[_],_)] -> 
                    (IsOpName nm) && nm.[0]='(' && nm.[nm.Length-1]=')'
                | [Item.UnionCase _] -> IsOpName nm
                | _ -> false              

            let isFSharpList nm = (nm = "[]") // list shows up as a Type and a UnionCase, only such entity with a symbolic name, but want to filter out of intellisense

            items |> List.filter (fun (nm,items) -> not (isOpItem(nm,items)) && not(isFSharpList nm)) 


        let decls = 
            // Filter out duplicate names
            items |> List.map (fun (nm,itemsWithSameName) -> 
                match itemsWithSameName with
                | [] -> failwith "Unexpected empty bag"
                | items -> 
                    new FSharpDeclarationListItem(nm, GlyphOfItem(denv,items.Head), Choice1Of2 (items, infoReader, m, denv, reactor, checkAlive)))

        new FSharpDeclarationListInfo(Array.ofList decls)

    
    static member Error msg = new FSharpDeclarationListInfo([| new FSharpDeclarationListItem("<Note>", 0, Choice2Of2 (FSharpToolTipText [FSharpToolTipElement.CompositionError msg])) |] )
    static member Empty = new FSharpDeclarationListInfo([| |])

