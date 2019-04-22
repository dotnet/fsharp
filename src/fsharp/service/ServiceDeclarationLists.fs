// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Open up the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.
//--------------------------------------------------------------------------

namespace FSharp.Compiler.SourceCodeServices

open FSharp.Compiler 
open FSharp.Compiler.AbstractIL.IL 
open FSharp.Compiler.AbstractIL.Internal.Library  
open FSharp.Compiler.AbstractIL.Diagnostics 
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.Layout
open FSharp.Compiler.Layout.TaggedTextOps
open FSharp.Compiler.Lib
open FSharp.Compiler.PrettyNaming
open FSharp.Compiler.Range
open FSharp.Compiler.Tast
open FSharp.Compiler.Tastops
open FSharp.Compiler.Infos
open FSharp.Compiler.NameResolution
open FSharp.Compiler.InfoReader

[<AutoOpen>]
module EnvMisc3 =
    /// dataTipSpinWaitTime limits how long we block the UI thread while a tooltip pops up next to a selected item in an IntelliSense completion list.
    /// This time appears to be somewhat amortized by the time it takes the VS completion UI to actually bring up the tooltip after selecting an item in the first place.
    let dataTipSpinWaitTime = GetEnvInteger "FCS_ToolTipSpinWaitTime" 5000


[<Sealed>]
/// Represents one parameter for one method (or other item) in a group. 
type FSharpMethodGroupItemParameter(name: string, canonicalTypeTextForSorting: string, display: layout, isOptional: bool) = 

    /// The name of the parameter.
    member __.ParameterName = name

    /// A key that can be used for sorting the parameters, used to help sort overloads.
    member __.CanonicalTypeTextForSorting = canonicalTypeTextForSorting

    /// The structured representation for the parameter including its name, its type and visual indicators of other
    /// information such as whether it is optional.
    member __.StructuredDisplay = display

    /// The text to display for the parameter including its name, its type and visual indicators of other
    /// information such as whether it is optional.
    member __.Display = showL display

    /// Is the parameter optional
    member __.IsOptional = isOptional

[<AutoOpen>]
module internal DescriptionListsImpl = 

    let isFunction g ty =
        let _, tau = tryDestForallTy g ty
        isFunTy g tau 
   
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
                NicePrint.layoutOfParamData denv (ParamData(false, false, false, NotOptional, NoCallerInfo, Some f.Id, ReflectedArgInfo.None, f.FormalType)) 
        FSharpMethodGroupItemParameter(
          name=initial.ParameterName,
          canonicalTypeTextForSorting=initial.CanonicalTypeTextForSorting,
          display=display,
          isOptional=false)

    let ParamOfParamData g denv (ParamData(_isParamArrayArg, _isInArg, _isOutArg, optArgInfo, _callerInfo, nmOpt, _reflArgInfo, pty) as paramData) =
        FSharpMethodGroupItemParameter(
          name = (match nmOpt with None -> "" | Some pn -> pn.idText),
          canonicalTypeTextForSorting = printCanonicalizedTypeName g denv pty,
          display = NicePrint.layoutOfParamData denv paramData,
          isOptional=optArgInfo.IsOptional)

    // TODO this code is similar to NicePrint.fs:formatParamDataToBuffer, refactor or figure out why different?
    let PrettyParamsOfParamDatas g denv typarInst (paramDatas:ParamData list) rty = 
        let paramInfo, paramTypes = 
            paramDatas 
            |> List.map (fun (ParamData(isParamArrayArg, _isInArg, _isOutArg, optArgInfo, _callerInfo, nmOpt, _reflArgInfo, pty)) -> 
                let isOptArg = optArgInfo.IsOptional
                match nmOpt, isOptArg, tryDestOptionTy denv.g pty with 
                // Layout an optional argument 
                | Some id, true, ptyOpt -> 
                    let nm = id.idText
                    // detect parameter type, if ptyOpt is None - this is .NET style optional argument
                    let pty = match ptyOpt with ValueSome x -> x | _ -> pty
                    (nm, isOptArg, SepL.questionMark ^^ (wordL (TaggedTextOps.tagParameter nm))),  pty
                // Layout an unnamed argument 
                | None, _, _ -> 
                    ("", isOptArg, emptyL), pty
                // Layout a named argument 
                | Some id, _, _ -> 
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
                    (nm, isOptArg, prefix), pty)
            |> List.unzip

        // Prettify everything
        let prettyTyparInst, (prettyParamTys, _prettyRetTy), (prettyParamTysL, prettyRetTyL), prettyConstraintsL = 
            NicePrint.prettyLayoutOfInstAndSig denv (typarInst, paramTypes, rty)

        // Remake the params using the prettified versions
        let prettyParams = 
          (paramInfo, prettyParamTys, prettyParamTysL) |||> List.map3 (fun (nm, isOptArg, paramPrefix) tau tyL -> 
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
            (prettyParamTys, prettyParamTysL) 
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
                          

#if !NO_EXTENSIONTYPING

    /// Get the set of static parameters associated with an item
    let StaticParamsOfItem (infoReader:InfoReader) m denv item = 
        let amap = infoReader.amap
        let g = infoReader.g
        match item with
        | SymbolHelpers.ItemIsWithStaticArguments m g staticParameters ->
            staticParameters 
                |> Array.map (fun sp -> 
                    let ty = Import.ImportProvidedType amap m (sp.PApply((fun x -> x.ParameterType), m))
                    let spKind = NicePrint.prettyLayoutOfType denv ty
                    let spName = sp.PUntaint((fun sp -> sp.Name), m)
                    let spOpt = sp.PUntaint((fun sp -> sp.IsOptional), m)
                    FSharpMethodGroupItemParameter(
                      name = spName,
                      canonicalTypeTextForSorting = showL spKind,
                      display = (if spOpt then SepL.questionMark else emptyL) ^^ wordL (TaggedTextOps.tagParameter spName) ^^ RightL.colon ^^ spKind,
                      //display = sprintf "%s%s: %s" (if spOpt then "?" else "") spName spKind,
                      isOptional=spOpt))
        | _ -> [| |]
#endif

    /// Get all the information about parameters and "prettify" the types by choosing nice type variable
    /// names.  This is similar to the other variations on "show me an item" code. This version is
    /// is used when presenting groups of methods (see FSharpMethodGroup).  It is possible these different
    /// versions could be better unified.
    let rec PrettyParamsAndReturnTypeOfItem (infoReader:InfoReader) m denv (item: ItemWithInst) = 
        let amap = infoReader.amap
        let g = infoReader.g
        let denv = { SymbolHelpers.SimplerDisplayEnv denv with useColonForReturnType=true}
        match item.Item with
        | Item.Value vref -> 
            let getPrettyParamsOfTypes() = 
                let tau = vref.TauType
                match tryDestFunTy denv.g tau with
                | ValueSome(arg, rtau) ->
                    let args = tryDestRefTupleTy denv.g arg 
                    let _prettyTyparInst, prettyParams, prettyRetTyL, _prettyConstraintsL = PrettyParamsOfTypes g denv item.TyparInst args rtau
                    // FUTURE: prettyTyparInst is the pretty version of the known instantiations of type parameters in the output. It could be returned
                    // for display as part of the method group
                    prettyParams, prettyRetTyL
                | _ -> 
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
                | firstCurriedArgInfo :: _ ->
                    // result 'paramDatas' collection corresponds to the first argument of curried function
                    // i.e. let func (a : int) (b : int) = a + b
                    // paramDatas will contain information about a and retTy will be: int -> int
                    // This is good enough as we don't provide ways to display info for the second curried argument
                    let firstCurriedParamDatas = 
                        firstCurriedArgInfo
                        |> List.map ParamNameAndType.FromArgInfo
                        |> List.map (fun (ParamNameAndType(nmOpt, pty)) -> ParamData(false, false, false, NotOptional, NoCallerInfo, nmOpt, ReflectedArgInfo.None, pty))

                    // Adjust the return type so it only strips the first argument
                    let curriedRetTy = 
                        match tryDestFunTy denv.g vref.TauType with
                        | ValueSome(_, rtau) -> rtau
                        | _ -> lastRetTy

                    let _prettyTyparInst, prettyFirstCurriedParams, prettyCurriedRetTyL, prettyConstraintsL = PrettyParamsOfParamDatas g denv item.TyparInst firstCurriedParamDatas curriedRetTy
                    
                    let prettyCurriedRetTyL = prettyCurriedRetTyL ^^ SepL.space ^^ prettyConstraintsL

                    prettyFirstCurriedParams, prettyCurriedRetTyL

        | Item.UnionCase(ucinfo, _)   -> 
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

        | Item.AnonRecdField(_anonInfo, tys, i, _) ->
            let _prettyTyparInst, prettyRetTyL = NicePrint.prettyLayoutOfUncurriedSig denv item.TyparInst [] tys.[i]
            [], prettyRetTyL

        | Item.ILField finfo ->
            let _prettyTyparInst, prettyRetTyL = NicePrint.prettyLayoutOfUncurriedSig denv item.TyparInst [] (finfo.FieldType(amap, m))
            [], prettyRetTyL

        | Item.Event einfo ->
            let _prettyTyparInst, prettyRetTyL = NicePrint.prettyLayoutOfUncurriedSig denv item.TyparInst [] (PropTypOfEventInfo infoReader m AccessibleFromSomewhere einfo)
            [], prettyRetTyL

        | Item.Property(_, pinfo :: _) -> 
            let paramDatas = pinfo.GetParamDatas(amap, m)
            let rty = pinfo.GetPropertyType(amap, m) 

            let _prettyTyparInst, prettyParams, prettyRetTyL, _prettyConstraintsL = PrettyParamsOfParamDatas g denv item.TyparInst paramDatas rty
            // FUTURE: prettyTyparInst is the pretty version of the known instantiations of type parameters in the output. It could be returned
            // for display as part of the method group
            prettyParams, prettyRetTyL

        | Item.CtorGroup(_, (minfo :: _)) 
        | Item.MethodGroup(_, (minfo :: _), _) -> 
            let paramDatas = minfo.GetParamDatas(amap, m, minfo.FormalMethodInst) |> List.head
            let rty = minfo.GetFSharpReturnTy(amap, m, minfo.FormalMethodInst)
            let _prettyTyparInst, prettyParams, prettyRetTyL, _prettyConstraintsL = PrettyParamsOfParamDatas g denv item.TyparInst paramDatas rty
            // FUTURE: prettyTyparInst is the pretty version of the known instantiations of type parameters in the output. It could be returned
            // for display as part of the method group
            prettyParams, prettyRetTyL

        | Item.CustomBuilder (_, vref) -> 
            PrettyParamsAndReturnTypeOfItem infoReader m denv { item with Item = Item.Value vref }

        | Item.TypeVar _ -> 
            [], emptyL

        | Item.CustomOperation (_, usageText, Some minfo) -> 
            match usageText() with 
            | None -> 
                let argNamesAndTys = SymbolHelpers.ParamNameAndTypesOfUnaryCustomOperation g minfo 
                let argTys, _ = PrettyTypes.PrettifyTypes g (argNamesAndTys |> List.map (fun (ParamNameAndType(_, ty)) -> ty))
                let paramDatas = (argNamesAndTys, argTys) ||> List.map2 (fun (ParamNameAndType(nmOpt, _)) argTy -> ParamData(false, false, false, NotOptional, NoCallerInfo, nmOpt, ReflectedArgInfo.None, argTy))
                let rty = minfo.GetFSharpReturnTy(amap, m, minfo.FormalMethodInst)
                let _prettyTyparInst, prettyParams, prettyRetTyL, _prettyConstraintsL = PrettyParamsOfParamDatas g denv item.TyparInst paramDatas rty

                // FUTURE: prettyTyparInst is the pretty version of the known instantiations of type parameters in the output. It could be returned
                // for display as part of the method group
                prettyParams, prettyRetTyL

            | Some _ -> 
                let rty = minfo.GetFSharpReturnTy(amap, m, minfo.FormalMethodInst)
                let _prettyTyparInst, prettyRetTyL = NicePrint.prettyLayoutOfUncurriedSig denv item.TyparInst [] rty
                [], prettyRetTyL  // no parameter data available for binary operators like 'zip', 'join' and 'groupJoin' since they use bespoke syntax 

        | Item.FakeInterfaceCtor ty -> 
            let _prettyTyparInst, prettyRetTyL = NicePrint.prettyLayoutOfUncurriedSig denv item.TyparInst [] ty
            [], prettyRetTyL

        | Item.DelegateCtor delty -> 
            let (SigOfFunctionForDelegate(_, _, _, fty)) = GetSigOfFunctionForDelegate infoReader delty m AccessibleFromSomewhere

            // No need to pass more generic type information in here since the instanitations have already been applied
            let _prettyTyparInst, prettyParams, prettyRetTyL, _prettyConstraintsL = PrettyParamsOfParamDatas g denv item.TyparInst [ParamData(false, false, false, NotOptional, NoCallerInfo, None, ReflectedArgInfo.None, fty)] delty

            // FUTURE: prettyTyparInst is the pretty version of the known instantiations of type parameters in the output. It could be returned
            // for display as part of the method group
            prettyParams, prettyRetTyL

        |  _ -> 
            [], emptyL


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
            | TILObjectRepr (TILObjectReprData (_, _, td)) -> 
                if td.IsClass        then FSharpGlyph.Class
                elif td.IsStruct     then FSharpGlyph.Struct
                elif td.IsInterface  then FSharpGlyph.Interface
                elif td.IsEnum       then FSharpGlyph.Enum
                else                      FSharpGlyph.Delegate
            | TAsmRepr _ -> FSharpGlyph.Typedef
            | TMeasureableRepr _-> FSharpGlyph.Typedef 
#if !NO_EXTENSIONTYPING
            | TProvidedTypeExtensionPoint _-> FSharpGlyph.Typedef 
            | TProvidedNamespaceExtensionPoint  _-> FSharpGlyph.Typedef  
#endif
            | TNoRepr -> FSharpGlyph.Class  
         
         /// Find the glyph for the given type representation.
         let typeToGlyph ty = 
            match tryDestAppTy denv.g ty with
            | ValueSome tcref -> tcref.TypeReprInfo |> reprToGlyph
            | _ ->
                if isStructTupleTy denv.g ty then FSharpGlyph.Struct
                elif isRefTupleTy denv.g ty then FSharpGlyph.Class
                elif isFunction denv.g ty then FSharpGlyph.Delegate
                elif isTyparTy denv.g ty then FSharpGlyph.Struct
                else FSharpGlyph.Typedef
            
         // This may explore assemblies that are not in the reference set,
         // e.g. for type abbreviations to types not in the reference set. 
         // In this case just use GlyphMajor.Class.
         protectAssemblyExploration FSharpGlyph.Class (fun () ->
            match item with 
            | Item.Value(vref) | Item.CustomBuilder (_, vref) -> 
                  if isFunction denv.g vref.Type then FSharpGlyph.Method
                  elif vref.LiteralValue.IsSome then FSharpGlyph.Constant
                  else FSharpGlyph.Variable
            | Item.Types(_, ty :: _) -> typeToGlyph (stripTyEqns denv.g ty)    
            | Item.UnionCase _
            | Item.ActivePatternCase _ -> FSharpGlyph.EnumMember   
            | Item.ExnCase _ -> FSharpGlyph.Exception   
            | Item.AnonRecdField _ -> FSharpGlyph.Field
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
                    elif tydef.IsStruct then FSharpGlyph.Struct
                    else FSharpGlyph.Class
                else FSharpGlyph.Class
            | Item.ModuleOrNamespaces(modref :: _) -> 
                  if modref.IsNamespace then FSharpGlyph.NameSpace else FSharpGlyph.Module
            | Item.ArgName _ -> FSharpGlyph.Variable
            | Item.SetterArg _ -> FSharpGlyph.Variable
            | _ -> FSharpGlyph.Error)


    /// Get rid of groups of overloads an replace them with single items.
    /// (This looks like it is doing the a similar thing as FlattenItems, this code 
    /// duplication could potentially be removed)
    let AnotherFlattenItems g m item =
        match item with 
        | Item.CtorGroup(nm, cinfos) -> List.map (fun minfo -> Item.CtorGroup(nm, [minfo])) cinfos 
        | Item.FakeInterfaceCtor _
        | Item.DelegateCtor _ -> [item]
        | Item.NewDef _ 
        | Item.ILField _ -> []
        | Item.Event _ -> []
        | Item.RecdField(rfinfo) -> 
            if isFunction g rfinfo.FieldType then [item] else []
        | Item.Value v -> 
            if isFunction g v.Type then [item] else []
        | Item.UnionCase(ucr, _) -> 
            if not ucr.UnionCase.IsNullary then [item] else []
        | Item.ExnCase(ecr) -> 
            if isNil (recdFieldsOfExnDefRef ecr) then [] else [item]
        | Item.Property(_, pinfos) -> 
            let pinfo = List.head pinfos 
            if pinfo.IsIndexer then [item] else []
#if !NO_EXTENSIONTYPING
        | SymbolHelpers.ItemIsWithStaticArguments m g _ -> 
            // we pretend that provided-types-with-static-args are method-like in order to get ParamInfo for them
            [item] 
#endif
        | Item.MethodGroup(nm, minfos, orig) -> minfos |> List.map (fun minfo -> Item.MethodGroup(nm, [minfo], orig)) 
        | Item.CustomOperation(_name, _helpText, _minfo) -> [item]
        | Item.TypeVar _ -> []
        | Item.CustomBuilder _ -> []
        | _ -> []


/// An intellisense declaration
[<Sealed>]
type FSharpDeclarationListItem(name: string, nameInCode: string, fullName: string, glyph: FSharpGlyph, info, accessibility: FSharpAccessibility option,
                               kind: CompletionItemKind, isOwnMember: bool, priority: int, isResolved: bool, namespaceToOpen: string option) =

    let mutable descriptionTextHolder: FSharpToolTipText<_> option = None
    let mutable task = null

    member __.Name = name
    member __.NameInCode = nameInCode

    member __.StructuredDescriptionTextAsync = 
        let userOpName = "ToolTip"
        match info with
        | Choice1Of2 (items: CompletionItem list, infoReader, m, denv, reactor:IReactorOperations, checkAlive) -> 
            // reactor causes the lambda to execute on the background compiler thread, through the Reactor
            reactor.EnqueueAndAwaitOpAsync (userOpName, "StructuredDescriptionTextAsync", name, fun ctok -> 
                RequireCompilationThread ctok
                // This is where we do some work which may touch TAST data structures owned by the IncrementalBuilder - infoReader, item etc. 
                // It is written to be robust to a disposal of an IncrementalBuilder, in which case it will just return the empty string. 
                // It is best to think of this as a "weak reference" to the IncrementalBuilder, i.e. this code is written to be robust to its
                // disposal. Yes, you are right to scratch your head here, but this is ok.
                cancellable.Return(
                    if checkAlive() then 
                        FSharpToolTipText(items |> List.map (fun x -> SymbolHelpers.FormatStructuredDescriptionOfItem true infoReader m denv x.ItemWithInst))
                    else 
                        FSharpToolTipText [ FSharpStructuredToolTipElement.Single(wordL (tagText (FSComp.SR.descriptionUnavailable())), FSharpXmlDoc.None) ]))
            | Choice2Of2 result -> 
                async.Return result

    member decl.DescriptionTextAsync = 
        decl.StructuredDescriptionTextAsync
        |> Tooltips.Map Tooltips.ToFSharpToolTipText

    member decl.StructuredDescriptionText = 
      ErrorScope.Protect Range.range0 
       (fun () -> 
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
                task.Wait EnvMisc3.dataTipSpinWaitTime  |> ignore
                match descriptionTextHolder with 
                | Some text -> text
                | None -> FSharpToolTipText [ FSharpStructuredToolTipElement.Single(wordL (tagText (FSComp.SR.loadingDescription())), FSharpXmlDoc.None) ]

            | Choice2Of2 result -> 
                result
       )
       (fun err -> FSharpToolTipText [FSharpStructuredToolTipElement.CompositionError err])
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
    static let fsharpNamespace = [|"Microsoft"; "FSharp"|]

    member __.Items = declarations
    member __.IsForType = isForType
    member __.IsError = isError

    // Make a 'Declarations' object for a set of selected items
    static member Create(infoReader:InfoReader, m, denv, getAccessibility, items: CompletionItem list, reactor, currentNamespaceOrModule: string[] option, isAttributeApplicationContext: bool, checkAlive) = 
        let g = infoReader.g
        let isForType = items |> List.exists (fun x -> x.Type.IsSome)
        let items = items |> SymbolHelpers.RemoveExplicitlySuppressedCompletionItems g
        
        let tyconRefOptEq tref1 tref2 =
            match tref1, tref2 with
            | Some tref1, tref2 -> tyconRefEq g tref1 tref2
            | _ -> false

        // Adjust items priority. Sort by name. For things with the same name,
        //     - show types with fewer generic parameters first
        //     - show types before over other related items - they usually have very useful XmlDocs 
        let _, _, items = 
            items 
            |> List.map (fun x ->
                match x.Item with
                | Item.Types (_, (TType_app(tcref, _) :: _)) -> { x with MinorPriority = 1 + tcref.TyparsNoRange.Length }
                // Put delegate ctors after types, sorted by #typars. RemoveDuplicateItems will remove FakeInterfaceCtor and DelegateCtor if an earlier type is also reported with this name
                | Item.FakeInterfaceCtor (TType_app(tcref, _)) 
                | Item.DelegateCtor (TType_app(tcref, _)) -> { x with MinorPriority = 1000 + tcref.TyparsNoRange.Length }
                // Put type ctors after types, sorted by #typars. RemoveDuplicateItems will remove DefaultStructCtors if a type is also reported with this name
                | Item.CtorGroup (_, (cinfo :: _)) -> { x with MinorPriority = 1000 + 10 * cinfo.DeclaringTyconRef.TyparsNoRange.Length }
                | Item.MethodGroup(_, minfo :: _, _) -> { x with IsOwnMember = tyconRefOptEq x.Type minfo.DeclaringTyconRef }
                | Item.Property(_, pinfo :: _) -> { x with IsOwnMember = tyconRefOptEq x.Type pinfo.DeclaringTyconRef }
                | Item.ILField finfo -> { x with IsOwnMember = tyconRefOptEq x.Type finfo.DeclaringTyconRef }
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
            |> SymbolHelpers.RemoveDuplicateCompletionItems g
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

        // Filter out operators, active patterns (as values) and the empty list
        let items = 
            // Check whether this item looks like an operator.
            let isOperatorItem(name, items: CompletionItem list) = 
                match items |> List.map (fun x -> x.Item) with
                | [Item.Value _ | Item.MethodGroup _ | Item.UnionCase _] -> IsOperatorName name
                | _ -> false              
            
            let isActivePatternItem (items: CompletionItem list) =
                match items |> List.map (fun x -> x.Item) with
                | [Item.Value vref] -> IsActivePatternName vref.CompiledName
                | _ -> false
            
            items |> List.filter (fun (displayName, items) -> 
                not (isOperatorItem(displayName, items)) && 
                not (displayName = "[]") && // list shows up as a Type and a UnionCase, only such entity with a symbolic name, but want to filter out of intellisense
                not (isActivePatternItem items))
                    
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
                    let glyph = GlyphOfItem(denv, item.Item)

                    let name, nameInCode =
                        if displayName.StartsWithOrdinal("( ") && displayName.EndsWithOrdinal(" )") then
                            let cleanName = displayName.[2..displayName.Length - 3]
                            cleanName,
                            if IsOperatorName displayName then cleanName else "``" + cleanName + "``"
                        else 
                            displayName,
                            match item.Unresolved with
                            | Some _ -> displayName
                            | None -> Lexhelp.Keywords.QuoteIdentifierIfNeeded displayName

                    let isAttributeItem = lazy (SymbolHelpers.IsAttribute infoReader item.Item)

                    let cutAttributeSuffix (name: string) =
                        if isAttributeApplicationContext && name <> "Attribute" && name.EndsWithOrdinal("Attribute") && isAttributeItem.Value then
                            name.[0..name.Length - "Attribute".Length - 1]
                        else name

                    let name = cutAttributeSuffix name
                    let nameInCode = cutAttributeSuffix nameInCode
                    
                    let fullName = 
                        match item.Unresolved with
                        | Some x -> x.FullName
                        | None -> SymbolHelpers.FullNameOfItem g item.Item
                    
                    let namespaceToOpen = 
                        item.Unresolved 
                        |> Option.map (fun x -> x.Namespace)
                        |> Option.bind (fun ns ->
                            if ns |> Array.startsWith fsharpNamespace then None
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
                            | ns -> Some (System.String.Join(".", ns)))

                    FSharpDeclarationListItem(
                        name, nameInCode, fullName, glyph, Choice1Of2 (items, infoReader, m, denv, reactor, checkAlive), getAccessibility item.Item,
                        item.Kind, item.IsOwnMember, item.MinorPriority, item.Unresolved.IsNone, namespaceToOpen))

        new FSharpDeclarationListInfo(Array.ofList decls, isForType, false)
    
    static member Error msg = 
        new FSharpDeclarationListInfo(
                [| FSharpDeclarationListItem("<Note>", "<Note>", "<Note>", FSharpGlyph.Error, Choice2Of2 (FSharpToolTipText [FSharpStructuredToolTipElement.CompositionError msg]),
                                             None, CompletionItemKind.Other, false, 0, false, None) |], false, true)
    
    static member Empty = FSharpDeclarationListInfo([| |], false, false)



/// Represents one method (or other item) in a method group. The item may represent either a method or 
/// a single, non-overloaded item such as union case or a named function value.
// Note: instances of this type do not hold any references to any compiler resources.
[<Sealed; NoEquality; NoComparison>]
type FSharpMethodGroupItem(description: FSharpToolTipText<layout>, xmlDoc: FSharpXmlDoc,
                           returnType: layout, parameters: FSharpMethodGroupItemParameter[],
                           hasParameters: bool, hasParamArrayArg: bool, staticParameters: FSharpMethodGroupItemParameter[]) = 

    /// The structured description representation for the method (or other item)
    member __.StructuredDescription = description

    /// The formatted description text for the method (or other item)
    member __.Description = Tooltips.ToFSharpToolTipText description

    /// The documentation for the item
    member __.XmlDoc = xmlDoc

    /// The The structured description representation for the method (or other item)
    member __.StructuredReturnTypeText = returnType

    /// The formatted type text for the method (or other item)
    member __.ReturnTypeText = showL returnType

    /// The parameters of the method in the overload set
    member __.Parameters = parameters

    /// Does the method support an arguments list?  This is always true except for static type instantiations like TP<42, "foo">.
    member __.HasParameters = hasParameters

    /// Does the method support a params list arg?
    member __.HasParamArrayArg = hasParamArrayArg

    /// Does the type name or method support a static arguments list, like TP<42, "foo"> or conn.CreateCommand<42, "foo">(arg1, arg2)?
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
    static let methodOverloadsCache = System.Runtime.CompilerServices.ConditionalWeakTable<ItemWithInst, FSharpMethodGroupItem[]>()
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
                            
                        let description = FSharpToolTipText [SymbolHelpers.FormatStructuredDescriptionOfItem true infoReader m denv { item with Item = flatItem }]

                        let hasParamArrayArg = 
                            match flatItem with 
                            | Item.CtorGroup(_, [meth]) 
                            | Item.MethodGroup(_, [meth], _) -> meth.HasParamArrayArg(infoReader.amap, m, meth.FormalMethodInst) 
                            | _ -> false

                        let hasStaticParameters = 
                            match flatItem with 
#if !NO_EXTENSIONTYPING
                            | SymbolHelpers.ItemIsProvidedTypeWithStaticArguments m g _ -> false 
#endif
                            | _ -> true

                        FSharpMethodGroupItem(
                          description = description,
                          returnType = prettyRetTyL,
                          xmlDoc = SymbolHelpers.GetXmlCommentForItem infoReader m flatItem,
                          parameters = (prettyParams |> Array.ofList),
                          hasParameters = hasStaticParameters,
                          hasParamArrayArg = hasParamArrayArg,
#if !NO_EXTENSIONTYPING
                          staticParameters = StaticParamsOfItem infoReader m denv flatItem
#else
                          staticParameters = [| |]
#endif
                        ))
#if !FX_NO_WEAKTABLE
                methodOverloadsCache.Add(item, methods)
#endif
                yield! methods 
           |]

        new FSharpMethodGroup(name, methods)



