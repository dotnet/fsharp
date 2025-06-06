// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Open up the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.
//--------------------------------------------------------------------------

namespace FSharp.Compiler.EditorServices

open FSharp.Compiler.NicePrint
open Internal.Utilities.Library  
open Internal.Utilities.Library.Extras
open FSharp.Compiler 
open FSharp.Compiler.AbstractIL.Diagnostics 
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.EditorServices
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Infos
open FSharp.Compiler.InfoReader
open FSharp.Compiler.NameResolution
open FSharp.Compiler.Symbols
open FSharp.Compiler.Symbols.SymbolHelpers
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Text.Layout
open FSharp.Compiler.Text.LayoutRender
open FSharp.Compiler.Text.TaggedText
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps

/// A single data tip display element
[<RequireQualifiedAccess>]
type ToolTipElementData = 
    {
      Symbol: FSharpSymbol option
      MainDescription: TaggedText[]
      XmlDoc: FSharpXmlDoc
      TypeMapping: TaggedText[] list
      Remarks: TaggedText[] option
      ParamName : string option }

    static member internal Create(layout, xml, ?typeMapping, ?paramName, ?remarks, ?symbol) = 
        { MainDescription=layout; XmlDoc=xml; TypeMapping=defaultArg typeMapping []; ParamName=paramName; Remarks=remarks; Symbol = symbol }

/// A single data tip display element
[<RequireQualifiedAccess>]
type ToolTipElement = 
    | None

    /// A single type, method, etc with comment. May represent a method overload group.
    | Group of elements: ToolTipElementData list

    /// An error occurred formatting this element
    | CompositionError of errorText: string

    static member Single(layout, xml, ?typeMapping, ?paramName, ?remarks, ?symbol) = 
        Group [ ToolTipElementData.Create(layout, xml, ?typeMapping=typeMapping, ?paramName=paramName, ?remarks=remarks, ?symbol = symbol) ]

/// Information for building a data tip box.
type ToolTipText = 
    /// A list of data tip elements to display.
    | ToolTipText of ToolTipElement list

[<RequireQualifiedAccess>]
type CompletionItemKind =
    | SuggestedName
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
      Unresolved: UnresolvedSymbol option
      CustomInsertText: string voption
      CustomDisplayText: string voption }
    member x.Item = x.ItemWithInst.Item

[<AutoOpen>]
module DeclarationListHelpers =
    let mutable ToolTipFault = None

    let emptyToolTip = ToolTipText []

    /// Generate the structured tooltip for a method info
    let FormatOverloadsToList (infoReader: InfoReader) m denv (item: ItemWithInst) minfos symbol (width: int option) : ToolTipElement =
        ToolTipFault |> Option.iter (fun msg -> 
           let exn = Error((0, msg), range0)
           let ph = PhasedDiagnostic.Create(exn, BuildPhase.TypeCheck)
           simulateError ph)
        
        let layouts = 
            [ for minfo in minfos -> 
                let prettyTyparInst, layout = NicePrint.prettyLayoutOfMethInfoFreeStyle infoReader m denv item.TyparInstantiation minfo
                let xml = GetXmlCommentForMethInfoItem infoReader m item.Item minfo
                let tpsL = FormatTyparMapping denv prettyTyparInst
                let layout = PrintUtilities.squashToWidth width layout
                let layout = toArray layout
                let tpsL = List.map toArray tpsL
                ToolTipElementData.Create(layout, xml, tpsL, ?symbol = symbol) ]
 
        ToolTipElement.Group layouts
        
    let CompletionItemDisplayPartialEquality g = 
        let itemComparer = ItemDisplayPartialEquality g
  
        { new IPartialEqualityComparer<CompletionItem> with
            member x.InEqualityRelation item = itemComparer.InEqualityRelation item.Item
            member x.Equals(item1, item2) = nullSafeEquality item1 item2 (fun item1 item2 -> itemComparer.Equals(item1.Item, item2.Item))
            member x.GetHashCode item = itemComparer.GetHashCode(item.Item) }

    /// Remove all duplicate items
    let RemoveDuplicateCompletionItems g items =     
        if isNil items then items else
        items |> IPartialEqualityComparer.partialDistinctBy (CompletionItemDisplayPartialEquality g) 

    /// Filter types that are explicitly suppressed from the IntelliSense (such as uppercase "FSharpList", "Option", etc.)
    let RemoveExplicitlySuppressedCompletionItems (g: TcGlobals) (items: CompletionItem list) = 
        items |> List.filter (fun item -> not (IsExplicitlySuppressed g item.Item))

    // Remove items containing the same module references
    let RemoveDuplicateModuleRefs modrefs  = 
        modrefs |> IPartialEqualityComparer.partialDistinctBy 
                      { new IPartialEqualityComparer<ModuleOrNamespaceRef> with
                          member x.InEqualityRelation _ = true
                          member x.Equals(item1, item2) = nullSafeEquality item1 item2 (fun item1 item2 -> fullDisplayTextOfModRef item1 = fullDisplayTextOfModRef item2)
                          member x.GetHashCode item = hash item.Stamp  }

    let OutputFullName displayFullName ppF fnF r = 
      // Only display full names in quick info, not declaration lists or method lists
      if not displayFullName then 
        match ppF r with 
        | None -> emptyL
        | Some _ -> wordL (tagText (FSComp.SR.typeInfoFullName())) ^^ RightL.colon ^^ (fnF r)
      else emptyL

    let pubpathOfValRef (v: ValRef) = v.PublicPath        

    let pubpathOfTyconRef (x: TyconRef) = x.PublicPath

    /// Output the quick info information of a language item
    let rec FormatItemDescriptionToToolTipElement displayFullName (infoReader: InfoReader) ad m denv (item: ItemWithInst) symbol (width: int option) = 
        let g = infoReader.g
        let amap = infoReader.amap
        let denv = {SimplerDisplayEnv denv with showCsharpCodeAnalysisAttributes = true } 
        let xml = GetXmlCommentForItem infoReader m item.Item

        match item.Item with
        | Item.ImplicitOp(_, { contents = Some(TraitConstraintSln.FSMethSln(vref=vref)) }) -> 
            // operator with solution
            FormatItemDescriptionToToolTipElement displayFullName infoReader ad m denv { item with Item = Item.Value vref } symbol width

        | Item.Value vref | Item.CustomBuilder (_, vref) ->            
            let prettyTyparInst, resL = NicePrint.layoutQualifiedValOrMember denv infoReader item.TyparInstantiation vref
            let remarks = OutputFullName displayFullName pubpathOfValRef fullDisplayTextOfValRefAsLayout vref
            let tpsL = FormatTyparMapping denv prettyTyparInst
            let tpsL = List.map toArray tpsL
            let resL = PrintUtilities.squashToWidth width resL
            let resL = toArray resL
            let remarks = toArray remarks
            ToolTipElement.Single(resL, xml, tpsL, remarks=remarks, ?symbol = symbol)

        // Union tags (constructors)
        | Item.UnionCase(ucinfo, _) -> 
            let uc = ucinfo.UnionCase 
            let unionTy = generalizedTyconRef g ucinfo.TyconRef
            let recd = uc.RecdFields 
            let layout = 
                wordL (tagText (FSComp.SR.typeInfoUnionCase())) ^^
                NicePrint.layoutTyconRef denv ucinfo.TyconRef ^^
                sepL (tagPunctuation ".") ^^
                wordL (tagUnionCase (ConvertValLogicalNameToDisplayNameCore uc.Id.idText) |> mkNav uc.DefinitionRange) ^^
                RightL.colon ^^
                (if List.isEmpty recd then emptyL else NicePrint.layoutUnionCases denv infoReader ucinfo.TyconRef recd ^^ WordL.arrow) ^^
                NicePrint.layoutType denv unionTy
            let layout = PrintUtilities.squashToWidth width layout
            let layout = toArray layout
            ToolTipElement.Single (layout, xml, ?symbol = symbol)

        // Active pattern tag inside the declaration (result)             
        | Item.ActivePatternResult(apinfo, ty, idx, _) ->
            let items = apinfo.ActiveTags
            let layout = 
                wordL (tagText (FSComp.SR.typeInfoActivePatternResult())) ^^
                wordL (tagActivePatternResult (List.item idx items) |> mkNav apinfo.Range) ^^
                RightL.colon ^^
                NicePrint.layoutType denv ty
            let layout = PrintUtilities.squashToWidth width layout
            let layout = toArray layout
            ToolTipElement.Single (layout, xml, ?symbol = symbol)

        // Active pattern tags 
        | Item.ActivePatternCase apref -> 
            let v = apref.ActivePatternVal
            let vTauTy = v.TauType
            // REVIEW: use _cxs here
            let (prettyTyparInst, prettyTy), _cxs = PrettyTypes.PrettifyInstAndType denv.g (item.TyparInstantiation, vTauTy)
            let remarks = OutputFullName displayFullName pubpathOfValRef fullDisplayTextOfValRefAsLayout v
            let layout =
                wordL (tagText (FSComp.SR.typeInfoActiveRecognizer())) ^^
                wordL (tagActivePatternCase apref.DisplayName |> mkNav v.DefinitionRange) ^^
                RightL.colon ^^
                NicePrint.layoutType denv prettyTy
            let layout = PrintUtilities.squashToWidth width layout

            let tpsL = FormatTyparMapping denv prettyTyparInst

            let layout = toArray layout
            let tpsL = List.map toArray tpsL
            let remarks = toArray remarks
            ToolTipElement.Single (layout, xml, tpsL, remarks=remarks, ?symbol = symbol)

        // F# exception names
        | Item.ExnCase ecref -> 
            let layout = NicePrint.layoutExnDef denv infoReader ecref
            let layout = PrintUtilities.squashToWidth width layout
            let remarks = OutputFullName displayFullName pubpathOfTyconRef fullDisplayTextOfExnRefAsLayout ecref
            let layout = toArray layout
            let remarks = toArray remarks
            ToolTipElement.Single (layout, xml, remarks=remarks, ?symbol = symbol)

        | Item.RecdField rfinfo when rfinfo.TyconRef.IsFSharpException ->
            let ty, _ = PrettyTypes.PrettifyType g rfinfo.FieldType
            let id = rfinfo.DisplayName
            let layout =
                wordL (tagText (FSComp.SR.typeInfoArgument())) ^^
                wordL (tagParameter id) ^^
                RightL.colon ^^
                NicePrint.layoutType denv ty
            let layout = PrintUtilities.squashToWidth width layout
            let layout = toArray layout
            ToolTipElement.Single (layout, xml, paramName = id, ?symbol = symbol)

        // F# record field names
        | Item.RecdField rfinfo ->
            let rfield = rfinfo.RecdField
            let ty, _cxs = PrettyTypes.PrettifyType g rfinfo.FieldType
            let layout = 
                NicePrint.layoutTyconRef denv rfinfo.TyconRef ^^
                SepL.dot ^^
                wordL (tagRecordField rfield.DisplayName |> mkNav rfield.DefinitionRange) ^^
                RightL.colon ^^
                NicePrint.layoutType denv ty ^^
                (
                    match rfinfo.LiteralValue with
                    | None -> emptyL
                    | Some lit -> try WordL.equals ^^  NicePrint.layoutConst denv.g ty lit with _ -> emptyL
                )
            let layout = PrintUtilities.squashToWidth width layout
            let layout = toArray layout
            ToolTipElement.Single (layout, xml, ?symbol = symbol)

        | Item.UnionCaseField (ucinfo, fieldIndex) ->
            let rfield = ucinfo.UnionCase.GetFieldByIndex(fieldIndex)
            let fieldTy, _ = PrettyTypes.PrettifyType g rfield.rfield_type
            let id = rfield.Id
            let layout =
                wordL (tagText (FSComp.SR.typeInfoArgument())) ^^
                wordL (tagParameter id.idText) ^^
                RightL.colon ^^
                NicePrint.layoutType denv fieldTy
            let layout = PrintUtilities.squashToWidth width layout
            let layout = toArray layout
            ToolTipElement.Single (layout, xml, paramName = id.idText, ?symbol = symbol)

        // Not used
        | Item.NewDef id -> 
            let layout = 
                wordL (tagText (FSComp.SR.typeInfoPatternVariable())) ^^
                wordL (tagUnknownEntity id.idText)
            let layout = PrintUtilities.squashToWidth width layout
            let layout = toArray layout
            ToolTipElement.Single (layout, xml, ?symbol = symbol)

        // .NET fields
        | Item.ILField finfo ->
            let layout = 
                wordL (tagText (FSComp.SR.typeInfoField())) ^^
                NicePrint.layoutType denv finfo.ApparentEnclosingAppType ^^
                SepL.dot ^^
                wordL (tagField finfo.FieldName) ^^
                RightL.colon ^^
                NicePrint.layoutType denv (finfo.FieldType(amap, m)) ^^
                (
                    match finfo.LiteralValue with
                    | None -> emptyL
                    | Some v ->
                        WordL.equals ^^
                        try NicePrint.layoutConst denv.g (finfo.FieldType(infoReader.amap, m)) (CheckExpressions.TcFieldInit m v) with _ -> emptyL
                )
            let layout = PrintUtilities.squashToWidth width layout
            let layout = toArray layout
            ToolTipElement.Single (layout, xml, ?symbol = symbol)

        // .NET events
        | Item.Event einfo ->
            let eventTy = PropTypeOfEventInfo infoReader m AccessibleFromSomewhere einfo
            let eventTy, _cxs = PrettyTypes.PrettifyType g eventTy
            let layout =
                wordL (tagText (FSComp.SR.typeInfoEvent())) ^^
                NicePrint.layoutTyconRef denv einfo.ApparentEnclosingTyconRef ^^
                SepL.dot ^^
                wordL (tagEvent einfo.EventName) ^^
                RightL.colon ^^
                NicePrint.layoutType denv eventTy
            let layout = PrintUtilities.squashToWidth width layout
            let layout = toArray layout
            ToolTipElement.Single (layout, xml, ?symbol = symbol)

        // F# and .NET properties
        | Item.Property(info = pinfo :: _) -> 
            let layout = NicePrint.prettyLayoutOfPropInfoFreeStyle  g amap m denv pinfo
            let layout = PrintUtilities.squashToWidth width layout
            let layout = toArray layout
            ToolTipElement.Single (layout, xml, ?symbol = symbol)

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

            let layout = PrintUtilities.squashToWidth width layout
            let layout = toArray layout
            ToolTipElement.Single (layout, xml, ?symbol = symbol)

        // F# constructors and methods
        | Item.CtorGroup(_, minfos) 
        | Item.MethodGroup(_, minfos, _) ->
            FormatOverloadsToList infoReader m denv item minfos symbol width

        // The 'fake' representation of constructors of .NET delegate types
        | Item.DelegateCtor delTy -> 
           let delTy, _cxs = PrettyTypes.PrettifyType g delTy
           let (SigOfFunctionForDelegate(_, _, _, delFuncTy)) = GetSigOfFunctionForDelegate infoReader delTy m AccessibleFromSomewhere
           let layout =
               NicePrint.layoutTyconRef denv (tcrefOfAppTy g delTy) ^^
               LeftL.leftParen ^^
               NicePrint.layoutType denv delFuncTy ^^
               RightL.rightParen
           let layout = PrintUtilities.squashToWidth width layout
           let layout = toArray layout
           ToolTipElement.Single(layout, xml, ?symbol = symbol)

        // Types.
        | Item.Types(_, TType_app(tcref, _, _) :: _)
        | Item.UnqualifiedType (tcref :: _) -> 
            let denv = { denv with
                            // tooltips are space-constrained, so use shorter names
                            shortTypeNames = true
                            // tooltips are space-constrained, so don't include xml doc comments
                            // on types/members. The doc comments for the actual member will still
                            // be shown in the tip.
                            showDocumentation = false  }
            let layout = NicePrint.layoutTyconDefn denv infoReader ad m (* width *) tcref.Deref
            let layout = PrintUtilities.squashToWidth width layout
            let remarks = OutputFullName displayFullName pubpathOfTyconRef fullDisplayTextOfTyconRefAsLayout tcref
            let layout = toArray layout
            let remarks = toArray remarks
            ToolTipElement.Single (layout, xml, remarks=remarks, ?symbol = symbol)

        // Type variables
        | Item.TypeVar (_, typar) ->
            let layout = NicePrint.prettyLayoutOfTypar denv typar
            let layout = PrintUtilities.squashToWidth width layout
            ToolTipElement.Single (toArray layout, xml, ?symbol = symbol)

        // Traits
        | Item.Trait traitInfo ->
            let denv = { denv with shortConstraints = false}
            let layout = NicePrint.prettyLayoutOfTrait denv traitInfo
            let layout = PrintUtilities.squashToWidth width layout
            ToolTipElement.Single (toArray layout, xml, ?symbol = symbol)

        // F# Modules and namespaces
        | Item.ModuleOrNamespaces(modref :: _ as modrefs) -> 
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
                let layout = PrintUtilities.squashToWidth width layout
                let layout = toArray layout
                ToolTipElement.Single (layout, xml, ?symbol = symbol)
            else
                let layout = PrintUtilities.squashToWidth width layout
                let layout = toArray layout
                ToolTipElement.Single (layout, xml, ?symbol = symbol)

        | Item.AnonRecdField(anon, argTys, i, _) -> 
            let argTy = argTys[i]
            let nm = anon.DisplayNameByIdx i
            let argTy, _ = PrettyTypes.PrettifyType g argTy
            let layout =
                wordL (tagText (FSComp.SR.typeInfoAnonRecdField())) ^^
                wordL (tagRecordField nm) ^^
                RightL.colon ^^
                NicePrint.layoutType denv argTy
            let layout = PrintUtilities.squashToWidth width layout
            let layout = toArray layout
            ToolTipElement.Single (layout, FSharpXmlDoc.None, ?symbol = symbol)
            
        // Named parameters
        | Item.OtherName (ident = Some id; argType = argTy) -> 
            let argTy, _ = PrettyTypes.PrettifyType g argTy
            let layout =
                wordL (tagText (FSComp.SR.typeInfoArgument())) ^^
                wordL (tagParameter id.idText) ^^
                RightL.colon ^^
                NicePrint.layoutType denv argTy
            let layout = PrintUtilities.squashToWidth width layout
            let layout = toArray layout
            ToolTipElement.Single (layout, xml, paramName = id.idText, ?symbol = symbol)
            
        | Item.SetterArg (_, item) -> 
            FormatItemDescriptionToToolTipElement displayFullName infoReader ad m denv (ItemWithNoInst item) symbol width

        | Item.OtherName (ident = None)
    
        // TODO: give a decent tooltip for implicit operators that include the resolution of the operator
        //
        //type C() = 
        //    static member (++++++) (x: C, y: C) = C()
        //
        //let f (x: C) =
        //    x ++++++ x
        //
        // Here hovering over "++++++" in "f" could give a tooltip saying what the thing is and what it has resolved to.
        //
        //
        | Item.ImplicitOp _ 

        // TODO: consider why we aren't getting Item.Types for generic type parameters
        //    let F<'T>() = new System.Collections.Generic.List<'T>()
        | Item.Types (_, [TType_var _]) 

        // TODO: consider why we aren't getting Item.Types for units of measure
        | Item.Types (_, [TType_measure _]) 

        // TODO: consider whether we ever get Item.Types with more than one element
        | Item.Types (_, _ :: _ :: _) 

        // We don't expect Item.Types with an anonymous record type, function types etc.
        | Item.Types (_, [TType_anon _]) 
        | Item.Types (_, [TType_fun _]) 
        | Item.Types (_, [TType_forall _]) 
        | Item.Types (_, [TType_tuple _]) 
        | Item.Types (_, [TType_ucase _]) 

        // We don't expect these cases
        | Item.Types (_, []) 
        | Item.Property (info = []) 
        | Item.UnqualifiedType []
        | Item.ModuleOrNamespaces []
        | Item.CustomOperation (_, _, None) ->  ToolTipElement.None 

    /// Format the structured version of a tooltip for an item
    let FormatStructuredDescriptionOfItem isDecl infoReader ad m denv item symbol width = 
        DiagnosticsScope.Protect m 
            (fun () -> FormatItemDescriptionToToolTipElement isDecl infoReader ad m denv item symbol width)
            ToolTipElement.CompositionError

/// Represents one parameter for one method (or other item) in a group.
[<Sealed>]
type MethodGroupItemParameter(name: string, canonicalTypeTextForSorting: string, display: TaggedText[], isOptional: bool) = 

    /// The name of the parameter.
    member _.ParameterName = name

    /// A key that can be used for sorting the parameters, used to help sort overloads.
    member _.CanonicalTypeTextForSorting = canonicalTypeTextForSorting

    /// The text to display for the parameter including its name, its type and visual indicators of other
    /// information such as whether it is optional.
    member _.Display = display

    /// Is the parameter optional
    member _.IsOptional = isOptional

[<AutoOpen>]
module internal DescriptionListsImpl = 

    let isFunction g ty =
        let _, tauTy = tryDestForallTy g ty
        isFunTy g tauTy 
   
    let printCanonicalizedTypeName g (denv:DisplayEnv) tauTy =
        // get rid of F# abbreviations and such
        let strippedTy = stripTyEqnsWrtErasure EraseAll g tauTy
        // pretend no namespaces are open
        let denv = denv.SetOpenPaths([])
        // now printing will see a .NET-like canonical representation, that is good for sorting overloads into a reasonable order (see bug 94520)
        NicePrint.stringOfTy denv strippedTy

    let PrettyParamOfRecdField g denv (f: RecdField) =
        let display = NicePrint.prettyLayoutOfType denv f.FormalType
        let display = toArray display
        MethodGroupItemParameter(
          name = f.DisplayNameCore,
          canonicalTypeTextForSorting = printCanonicalizedTypeName g denv f.FormalType,
          display = display,
          isOptional=false)
    
    let PrettyParamOfUnionCaseField g denv isGenerated (i: int) (f: RecdField) = 
        let initial = PrettyParamOfRecdField g denv f
        let display = 
            if isGenerated i f then 
                initial.Display 
            else 
                let display = NicePrint.layoutOfParamData denv (ParamData(false, false, false, NotOptional, NoCallerInfo, Some f.Id, ReflectedArgInfo.None, f.FormalType)) 
                toArray display

        MethodGroupItemParameter(
          name=initial.ParameterName,
          canonicalTypeTextForSorting=initial.CanonicalTypeTextForSorting,
          display=display,
          isOptional=false)

    let ParamOfParamData g denv (ParamData(_isParamArrayArg, _isInArg, _isOutArg, optArgInfo, _callerInfo, nmOpt, _reflArgInfo, pty) as paramData) =
        let display = NicePrint.layoutOfParamData denv paramData
        let display = toArray display
        MethodGroupItemParameter(
          name = (match nmOpt with None -> "" | Some pn -> pn.idText),
          canonicalTypeTextForSorting = printCanonicalizedTypeName g denv pty,
          display = display,
          isOptional=optArgInfo.IsOptional)

    // TODO this code is similar to NicePrint.fs:formatParamDataToBuffer, refactor or figure out why different?
    let PrettyParamsOfParamDatas g denv typarInst (paramDatas:ParamData list) paramTy = 
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
                    (nm, isOptArg, SepL.questionMark ^^ (wordL (tagParameter nm))),  pty
                // Layout an unnamed argument 
                | None, _, _ -> 
                    ("", isOptArg, emptyL), pty
                // Layout a named argument 
                | Some id, _, _ -> 
                    let nm = id.idText
                    let prefix = 
                        if isParamArrayArg then
                            NicePrint.PrintUtilities.layoutBuiltinAttribute denv denv.g.attrib_ParamArrayAttribute ^^
                            wordL (tagParameter nm) ^^
                            RightL.colon
                            //sprintf "%s %s: " (NicePrint.PrintUtilities.layoutBuiltinAttribute denv denv.g.attrib_ParamArrayAttribute |> showL) nm 
                        else 
                            wordL (tagParameter nm) ^^
                            RightL.colon
                            //sprintf "%s: " nm
                    (nm, isOptArg, prefix), pty)
            |> List.unzip

        // Prettify everything
        let prettyTyparInst, (prettyParamTys, _prettyRetTy), (prettyParamTysL, prettyRetTyL), prettyConstraintsL = 
            NicePrint.prettyLayoutOfInstAndSig denv (typarInst, paramTypes, paramTy)

        // Remake the params using the prettified versions
        let prettyParams = 
          (paramInfo, prettyParamTys, prettyParamTysL) |||> List.map3 (fun (nm, isOptArg, paramPrefix) tauTy tyL -> 
            let display = paramPrefix ^^ tyL
            let display = toArray display
            MethodGroupItemParameter(
              name = nm,
              canonicalTypeTextForSorting = printCanonicalizedTypeName g denv tauTy,
              display = display,
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
            ||> List.map2 (fun paramTy tyL ->
                let display = toArray tyL
                MethodGroupItemParameter(
                    name = "",
                    canonicalTypeTextForSorting = printCanonicalizedTypeName g denv paramTy,
                    display =  display,
                    isOptional=false
                ))

        // Return the results
        prettyTyparInst, parameters, prettyRetTyL, prettyConstraintsL
                          

#if !NO_TYPEPROVIDERS

    /// Get the set of static parameters associated with an item
    let StaticParamsOfItem (infoReader:InfoReader) m denv item = 
        let amap = infoReader.amap
        let g = infoReader.g
        match item with
        | ItemIsWithStaticArguments m g staticParameters ->
            staticParameters 
                |> Array.map (fun sp -> 
                    let ty = Import.ImportProvidedType amap m (sp.PApply((fun x -> x.ParameterType), m))
                    let spKind = NicePrint.prettyLayoutOfType denv ty
                    let spName = sp.PUntaint((fun sp -> sp.Name), m)
                    let spOpt = sp.PUntaint((fun sp -> sp.IsOptional), m)
                    let display = (if spOpt then SepL.questionMark else emptyL) ^^ wordL (tagParameter spName) ^^ RightL.colon ^^ spKind
                    let display = toArray display
                    MethodGroupItemParameter(
                      name = spName,
                      canonicalTypeTextForSorting = showL spKind,
                      display = display,
                      //display = sprintf "%s%s: %s" (if spOpt then "?" else "") spName spKind,
                      isOptional=spOpt))
        | _ -> [| |]
#endif

    /// Get all the information about parameters and "prettify" the types by choosing nice type variable
    /// names.  This is similar to the other variations on "show me an item" code. This version is
    /// is used when presenting groups of methods (see MethodGroup).  It is possible these different
    /// versions could be better unified.
    let rec PrettyParamsAndReturnTypeOfItem (infoReader:InfoReader) m denv (item: ItemWithInst) = 
        let amap = infoReader.amap
        let g = infoReader.g
        let denv = { SimplerDisplayEnv denv with useColonForReturnType=true}
        match item.Item with
        | Item.Value vref -> 

            let getPrettyParamsOfTypes() = 
                let vTauTy = vref.TauType
                match tryDestFunTy denv.g vTauTy with
                | ValueSome(arg, retTy) ->
                    let args = tryDestRefTupleTy denv.g arg 
                    let _prettyTyparInst, prettyParams, prettyRetTyL, _prettyConstraintsL = PrettyParamsOfTypes g denv item.TyparInstantiation args retTy
                    // FUTURE: prettyTyparInst is the pretty version of the known instantiations of type parameters in the output. It could be returned
                    // for display as part of the method group
                    prettyParams, prettyRetTyL
                | _ -> 
                    let _prettyTyparInst, prettyTyL = NicePrint.prettyLayoutOfUncurriedSig denv item.TyparInstantiation [] vTauTy
                    [], prettyTyL

            match vref.ValReprInfo with
            | None -> 
                // ValReprInfo = None i.e. in let bindings defined in types or in local functions
                // in this case use old approach and return only information about types
                getPrettyParamsOfTypes ()

            | Some valReprInfo ->
                // ValReprInfo will exist for top-level syntactic functions
                // per spec: binding is considered to define a syntactic function if it is either a function or its immediate right-hand-side is a anonymous function
                let _, argInfos,  lastRetTy, _ = GetValReprTypeInFSharpForm  g valReprInfo vref.Type m
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
                        |> List.map (ParamNameAndType.FromArgInfo >> fun (ParamNameAndType(nmOpt, pty)) -> ParamData(false, false, false, NotOptional, NoCallerInfo, nmOpt, ReflectedArgInfo.None, pty))

                    // Adjust the return type so it only strips the first argument
                    let curriedRetTy = 
                        match tryDestFunTy denv.g vref.TauType with
                        | ValueSome(_, retTy) -> retTy
                        | _ -> lastRetTy

                    let _prettyTyparInst, prettyFirstCurriedParams, prettyCurriedRetTyL, prettyConstraintsL = PrettyParamsOfParamDatas g denv item.TyparInstantiation firstCurriedParamDatas curriedRetTy
                    
                    let prettyCurriedRetTyL = prettyCurriedRetTyL ^^ SepL.space ^^ prettyConstraintsL

                    prettyFirstCurriedParams, prettyCurriedRetTyL

        | Item.UnionCase(ucinfo, _)   -> 
            let prettyParams = 
                match ucinfo.UnionCase.RecdFields with
                | [f] -> [PrettyParamOfUnionCaseField g denv NicePrint.isGeneratedUnionCaseField -1 f]
                | fs -> fs |> List.mapi (PrettyParamOfUnionCaseField g denv NicePrint.isGeneratedUnionCaseField)
            let unionTy = generalizedTyconRef g ucinfo.TyconRef
            let rtyL = NicePrint.layoutType denv unionTy
            prettyParams, rtyL

        | Item.ActivePatternCase(apref)   -> 
            let v = apref.ActivePatternVal 
            let vTauTy = v.TauType
            let args, resTy = stripFunTy denv.g vTauTy 

            let apinfo = Option.get (TryGetActivePatternInfo v)
            let aparity = apinfo.ActiveTags.Length
            
            let caseTy = if aparity <= 1 then resTy else (argsOfAppTy g resTy)[apref.CaseIndex]

            let _prettyTyparInst, prettyParams, prettyRetTyL, _prettyConstraintsL = PrettyParamsOfTypes g denv item.TyparInstantiation args caseTy
            // FUTURE: prettyTyparInst is the pretty version of the known instantiations of type parameters in the output. It could be returned
            // for display as part of the method group
            prettyParams, prettyRetTyL

        | Item.ExnCase ecref -> 
            let prettyParams = ecref |> recdFieldsOfExnDefRef |> List.mapi (PrettyParamOfUnionCaseField g denv NicePrint.isGeneratedExceptionField) 
            let _prettyTyparInst, prettyRetTyL = NicePrint.prettyLayoutOfUncurriedSig denv item.TyparInstantiation [] g.exn_ty
            prettyParams, prettyRetTyL

        | Item.RecdField rfinfo ->
            let _prettyTyparInst, prettyRetTyL = NicePrint.prettyLayoutOfUncurriedSig denv item.TyparInstantiation [] rfinfo.FieldType
            [], prettyRetTyL

        | Item.AnonRecdField(_anonInfo, tys, i, _) ->
            let _prettyTyparInst, prettyRetTyL = NicePrint.prettyLayoutOfUncurriedSig denv item.TyparInstantiation [] tys[i]
            [], prettyRetTyL

        | Item.ILField finfo ->
            let _prettyTyparInst, prettyRetTyL = NicePrint.prettyLayoutOfUncurriedSig denv item.TyparInstantiation [] (finfo.FieldType(amap, m))
            [], prettyRetTyL

        | Item.Event einfo ->
            let _prettyTyparInst, prettyRetTyL = NicePrint.prettyLayoutOfUncurriedSig denv item.TyparInstantiation [] (PropTypeOfEventInfo infoReader m AccessibleFromSomewhere einfo)
            [], prettyRetTyL

        | Item.Property(info = pinfo :: _) -> 
            let paramDatas = pinfo.GetParamDatas(amap, m)
            let propTy = pinfo.GetPropertyType(amap, m) 

            let _prettyTyparInst, prettyParams, prettyRetTyL, _prettyConstraintsL = PrettyParamsOfParamDatas g denv item.TyparInstantiation paramDatas propTy
            // FUTURE: prettyTyparInst is the pretty version of the known instantiations of type parameters in the output. It could be returned
            // for display as part of the method group
            prettyParams, prettyRetTyL

        | Item.CtorGroup(_, minfo :: _) 
        | Item.MethodGroup(_, minfo :: _, _) -> 
            let paramDatas = minfo.GetParamDatas(amap, m, minfo.FormalMethodInst) |> List.head
            let retTy = minfo.GetFSharpReturnType(amap, m, minfo.FormalMethodInst)
            let _prettyTyparInst, prettyParams, prettyRetTyL, _prettyConstraintsL = PrettyParamsOfParamDatas g denv item.TyparInstantiation paramDatas retTy
            // FUTURE: prettyTyparInst is the pretty version of the known instantiations of type parameters in the output. It could be returned
            // for display as part of the method group
            prettyParams, prettyRetTyL

        | Item.Trait traitInfo -> 
            let paramDatas =
                [ for pty in traitInfo.GetLogicalArgumentTypes(g) do
                    ParamData(false, false, false, OptionalArgInfo.NotOptional, CallerInfo.NoCallerInfo, None, ReflectedArgInfo.None, pty) ]
            let retTy = traitInfo.GetReturnType(g)
            let _prettyTyparInst, prettyParams, prettyRetTyL, _prettyConstraintsL = PrettyParamsOfParamDatas g denv item.TyparInstantiation paramDatas retTy
            prettyParams, prettyRetTyL

        | Item.CustomBuilder (_, vref) -> 
            PrettyParamsAndReturnTypeOfItem infoReader m denv { item with Item = Item.Value vref }

        | Item.TypeVar _ -> 
            [], emptyL

        | Item.CustomOperation (_, usageText, Some minfo) -> 
            match usageText() with 
            | None -> 
                let argNamesAndTys = ParamNameAndTypesOfUnaryCustomOperation g minfo 
                let argTys, _ = PrettyTypes.PrettifyTypes g (argNamesAndTys |> List.map (fun (ParamNameAndType(_, ty)) -> ty))
                let paramDatas = (argNamesAndTys, argTys) ||> List.map2 (fun (ParamNameAndType(nmOpt, _)) argTy -> ParamData(false, false, false, NotOptional, NoCallerInfo, nmOpt, ReflectedArgInfo.None, argTy))
                let retTy = minfo.GetFSharpReturnType(amap, m, minfo.FormalMethodInst)
                let _prettyTyparInst, prettyParams, prettyRetTyL, _prettyConstraintsL = PrettyParamsOfParamDatas g denv item.TyparInstantiation paramDatas retTy

                // FUTURE: prettyTyparInst is the pretty version of the known instantiations of type parameters in the output. It could be returned
                // for display as part of the method group
                prettyParams, prettyRetTyL

            | Some _ -> 
                let retTy = minfo.GetFSharpReturnType(amap, m, minfo.FormalMethodInst)
                let _prettyTyparInst, prettyRetTyL = NicePrint.prettyLayoutOfUncurriedSig denv item.TyparInstantiation [] retTy
                [], prettyRetTyL  // no parameter data available for binary operators like 'zip', 'join' and 'groupJoin' since they use bespoke syntax 

        | Item.DelegateCtor delTy -> 
            let (SigOfFunctionForDelegate(_, _, _, delFuncTy)) = GetSigOfFunctionForDelegate infoReader delTy m AccessibleFromSomewhere

            // No need to pass more generic type information in here since the instantiations have already been applied
            let _prettyTyparInst, prettyParams, prettyRetTyL, _prettyConstraintsL = PrettyParamsOfParamDatas g denv item.TyparInstantiation [ParamData(false, false, false, NotOptional, NoCallerInfo, None, ReflectedArgInfo.None, delFuncTy)] delTy

            // FUTURE: prettyTyparInst is the pretty version of the known instantiations of type parameters in the output. It could be returned
            // for display as part of the method group
            prettyParams, prettyRetTyL

        | Item.CustomOperation _ // TODO: consider whether this should report parameter help
        | Item.ActivePatternResult _  // TODO: consider whether this should report parameter help
        | Item.UnqualifiedType _
        | Item.UnionCaseField _
        | Item.Types _
        | Item.SetterArg _
        | Item.NewDef _
        | Item.ModuleOrNamespaces _
        | Item.ImplicitOp _
        | Item.OtherName _
        | Item.MethodGroup(_, [], _)
        | Item.CtorGroup(_,[])
        | Item.Property(info = []) -> 
            [], emptyL


    /// Compute the index of the VS glyph shown with an item in the Intellisense menu
    let GlyphOfItem(denv, item) : FSharpGlyph = 
         /// Find the glyph for the given representation.    
         let reprToGlyph repr = 
            match repr with
            | TFSharpTyconRepr om -> 
                match om.fsobjmodel_kind with 
                | TFSharpUnion -> FSharpGlyph.Union
                | TFSharpRecord -> FSharpGlyph.Type
                | TFSharpClass -> FSharpGlyph.Class
                | TFSharpInterface -> FSharpGlyph.Interface
                | TFSharpStruct -> FSharpGlyph.Struct
                | TFSharpDelegate _ -> FSharpGlyph.Delegate
                | TFSharpEnum -> FSharpGlyph.Enum
            | TILObjectRepr (TILObjectReprData (_, _, td)) -> 
                if td.IsClass        then FSharpGlyph.Class
                elif td.IsStruct     then FSharpGlyph.Struct
                elif td.IsInterface  then FSharpGlyph.Interface
                elif td.IsEnum       then FSharpGlyph.Enum
                else                      FSharpGlyph.Delegate
            | TAsmRepr _ -> FSharpGlyph.Typedef
            | TMeasureableRepr _-> FSharpGlyph.Typedef 
#if !NO_TYPEPROVIDERS
            | TProvidedTypeRepr _-> FSharpGlyph.Typedef 
            | TProvidedNamespaceRepr  _-> FSharpGlyph.Typedef  
#endif
            | TNoRepr -> FSharpGlyph.Class  
         
         /// Find the glyph for the given type representation.
         let typeToGlyph ty = 
            match tryTcrefOfAppTy denv.g ty with
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
            | Item.ActivePatternResult _
            | Item.ImplicitOp _
            | Item.ActivePatternCase _ -> FSharpGlyph.EnumMember
            | Item.ExnCase _ -> FSharpGlyph.Exception   
            | Item.AnonRecdField _ -> FSharpGlyph.Field
            | Item.RecdField _ -> FSharpGlyph.Field
            | Item.UnionCaseField _ -> FSharpGlyph.Field
            | Item.ILField _ -> FSharpGlyph.Field
            | Item.Event _ -> FSharpGlyph.Event   
            | Item.Property _ -> FSharpGlyph.Property   
            | Item.CtorGroup _ 
            | Item.DelegateCtor _ 
            | Item.CustomOperation _ -> FSharpGlyph.Method
            | Item.MethodGroup (_, minfos, _) when minfos |> List.forall (fun minfo -> minfo.IsExtensionMember) -> FSharpGlyph.ExtensionMethod
            | Item.MethodGroup _ -> FSharpGlyph.Method
            | Item.Trait _ -> FSharpGlyph.Method
            | Item.TypeVar _ -> FSharpGlyph.TypeParameter
            | Item.Types _  -> FSharpGlyph.Class
            | Item.UnqualifiedType (tcref :: _) -> 
                if tcref.IsEnumTycon || tcref.IsILEnumTycon then FSharpGlyph.Enum
                elif tcref.IsFSharpException then FSharpGlyph.Exception
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
            | Item.NewDef _
            | Item.OtherName _
            | Item.SetterArg _ -> FSharpGlyph.Variable

            // These empty lists are not expected to occur
            | Item.ModuleOrNamespaces []
            | Item.UnqualifiedType [] ->
                FSharpGlyph.Error
            )


    /// Select the items that participate in a MethodGroup.
    let SelectMethodGroupItems g m item =
#if NO_TYPEPROVIDERS
        ignore m
#endif
        match item with 
        | Item.CtorGroup(nm, cinfos) -> List.map (fun minfo -> Item.CtorGroup(nm, [minfo])) cinfos 
        | Item.Trait traitInfo ->
            if traitInfo.GetLogicalArgumentTypes(g).IsEmpty then [] else [item]
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
        | Item.Property(info = pinfos) -> 
            let pinfo = List.head pinfos 
            if pinfo.IsIndexer then [item] else []
#if !NO_TYPEPROVIDERS
        | ItemIsWithStaticArguments m g _ -> 
            // we pretend that provided-types-with-static-args are method-like in order to get ParamInfo for them
            [item] 
#endif
        | Item.MethodGroup(nm, minfos, orig) -> minfos |> List.map (fun minfo -> Item.MethodGroup(nm, [minfo], orig)) 
        | Item.CustomOperation _ -> [item]
        // These are not items that can participate in a method group
        | Item.TypeVar _
        | Item.CustomBuilder _
        | Item.ActivePatternCase _
        | Item.AnonRecdField _
        | Item.OtherName _
        | Item.ImplicitOp _
        | Item.ModuleOrNamespaces _
        | Item.SetterArg _
        | Item.Types _
        | Item.UnionCaseField _
        | Item.UnqualifiedType _
        | Item.ActivePatternResult _ -> []

/// An intellisense declaration
[<Sealed>]
type DeclarationListItem(textInDeclList: string, textInCode: string, fullName: string, glyph: FSharpGlyph, info, accessibility: FSharpAccessibility,
                               kind: CompletionItemKind, isOwnMember: bool, priority: int, isResolved: bool, namespaceToOpen: string option) =

    member _.Name = textInDeclList

    member _.NameInList = textInDeclList

    member _.NameInCode = textInCode

    member _.Description = 
        match kind, info with
        | CompletionItemKind.SuggestedName, _ ->
            ToolTipText [ ToolTipElement.Single ([| tagText (FSComp.SR.suggestedName()) |], FSharpXmlDoc.None) ]
        | _, Choice1Of2 (items: CompletionItem list, infoReader, ad, m, denv) -> 
            ToolTipText(items |> List.map (fun x -> FormatStructuredDescriptionOfItem true infoReader ad m denv x.ItemWithInst None None))
        | _, Choice2Of2 result -> 
            result

    member _.Glyph = glyph 

    member _.Accessibility = accessibility

    member _.Kind = kind

    member _.IsOwnMember = isOwnMember

    member _.MinorPriority = priority

    member _.FullName = fullName

    member _.IsResolved = isResolved

    member _.NamespaceToOpen = namespaceToOpen

/// A table of declarations for Intellisense completion 
[<Sealed>]
type DeclarationListInfo(declarations: DeclarationListItem[], isForType: bool, isError: bool) = 
    static let fsharpNamespace = [|"Microsoft"; "FSharp"|]

    static let empty = DeclarationListInfo ([| |], false, false)

    // Check whether this item looks like an operator.
    static let isOperatorItem name (items: CompletionItem list) =
        match items with
        | [item] ->
            match item.Item with
            | Item.Value _ | Item.MethodGroup _ | Item.UnionCase _ -> IsOperatorDisplayName name
            | _ -> false
        | _ -> false              

    static let isActivePatternItem (items: CompletionItem list) =
        match items with
        | [item] ->
            match item.Item with
            | Item.Value vref -> IsActivePatternName vref.DisplayNameCoreMangled
            | _ -> false
        | _ -> false

    member _.Items = declarations

    member _.IsForType = isForType

    member _.IsError = isError

    // Make a 'Declarations' object for a set of selected items
    static member Create(infoReader:InfoReader, ad, m: range, denv, getAccessibility: Item -> FSharpAccessibility, items: CompletionItem list, currentNamespace: string[] option, isAttributeApplicationContext: bool) = 
        let g = infoReader.g
        let isForType = items |> List.exists (fun x -> x.Type.IsSome || (x.Item |> function Item.AnonRecdField _ -> true | _ -> false))
        let items = items |> RemoveExplicitlySuppressedCompletionItems g
        
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
                | Item.Types (_, TType_app(tcref, _, _) :: _) when isInterfaceTyconRef tcref -> { x with MinorPriority = 1000 + tcref.TyparsNoRange.Length }
                | Item.Types (_, TType_app(tcref, _, _) :: _) -> { x with MinorPriority = 1 + tcref.TyparsNoRange.Length }
                // Put delegate ctors after types, sorted by #typars. RemoveDuplicateItems will remove FakeInterfaceCtor and DelegateCtor if an earlier type is also reported with this name
                | Item.DelegateCtor (TType_app(tcref, _, _)) -> { x with MinorPriority = 1000 + tcref.TyparsNoRange.Length }
                // Put type ctors after types, sorted by #typars. RemoveDuplicateItems will remove DefaultStructCtors if a type is also reported with this name
                | Item.CtorGroup (_, cinfo :: _) -> { x with MinorPriority = 1000 + 10 * cinfo.DeclaringTyconRef.TyparsNoRange.Length }
                | Item.MethodGroup(_, minfo :: _, _) -> { x with IsOwnMember = tyconRefOptEq x.Type minfo.DeclaringTyconRef }
                | Item.Property(info = pinfo :: _) -> { x with IsOwnMember = tyconRefOptEq x.Type pinfo.DeclaringTyconRef }
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
        let supportsPreferExtsMethodsOverProperty = denv.g.langVersion.SupportsFeature Features.LanguageFeature.PreferExtensionMethodOverPlainProperty

        // Group by full name for unresolved items and by display name for resolved ones.
        let decls = 
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
                | None when x.CustomDisplayText.IsSome -> x.CustomDisplayText.Value
                | None -> x.Item.DisplayName
            )
            |> List.map (
                let textInDeclList item =
                    match item.Unresolved with
                    | Some u -> u.DisplayName
                    | None when item.CustomDisplayText.IsSome -> item.CustomDisplayText.Value
                    | None -> item.Item.DisplayNameCore
                let textInCode (item: CompletionItem) =
                    match item.Item with
                    | Item.TypeVar (name, typar) -> (if typar.StaticReq = Syntax.TyparStaticReq.None then "'" else " ^") + name
                    | _ ->
                        match item.Unresolved, item.CustomInsertText with
                        | Some u, _ -> u.DisplayName
                        | None, ValueSome textInCode -> textInCode
                        | None, _ -> item.Item.DisplayName
                if not supportsPreferExtsMethodsOverProperty then
                    // we don't pay the cost of filtering specific to RFC-1137
                    // nor risk a change in behaviour for the intellisense item list
                    // if the feature is disabled
                    fun (_, items) ->
                      let item = items.Head
                      [textInDeclList item, textInCode item, items]
                else
                    // RFC-1137 shenanigans:
                    // due to not desiring to merge Property and extension Method bearing same name,
                    // but still merging extension method if it tries to shadow other stuff than a Property
                    // we proceed with a pre-scan to see if we hit the particular case, in which case we partition
                    // items to be split and those that were initially remaining grouped.
                    // If we don't hit the specific case, or have a single entry, we keep the same logic as originally
                    // N.B: due to the logic returning 1 to N instead of 1, the next stage of the pipeline is List.concat
                    // introduced for this RFC
                    let hasBothPropertiesAndExtensionMethods items =
                        let rec inner hasProperty hasExtensionMethod items =
                            if hasProperty && hasExtensionMethod then
                                true
                            else
                                match items with
                                | [] -> hasProperty && hasExtensionMethod
                                | item :: tail when item.Kind = CompletionItemKind.Property                 -> inner true hasExtensionMethod tail 
                                | item :: tail when item.Kind = CompletionItemKind.Method(isExtension=true) -> inner hasProperty true tail
                                | _ :: tail                                                                 -> inner hasProperty hasExtensionMethod tail
                        inner false false items
                    function
                        | _, ([_] as items) 
                        | _,items when not (hasBothPropertiesAndExtensionMethods items) ->
                            let item = items.Head
                            [textInDeclList item, textInCode item, items]
                        | _, items (* RFC-1137 we have both property and extension method ...*) ->
                            let toSplit, together =
                                items
                                |> List.partition
                                    (fun item ->
                                        match item.Kind with
                                        | CompletionItemKind.Property | CompletionItemKind.Method(isExtension=true) -> true
                                        | _ -> false
                                    )
                            [
                                let rec createSublists list =
                                    match list with
                                    | [] -> []
                                    | _ :: tail -> list :: createSublists tail
                                    
                                // we use createSublists here so the `items` sent down the
                                // pipeline have their first element being the actual
                                // item, in order for the glyph to be the correct one
                                // notice how the later stage uses `GlyphOfItem(denv,items.Head)`
                                for items in createSublists toSplit do
                                    let item = items.Head
                                    textInDeclList item, textInCode item, items
                                if not together.IsEmpty then
                                  let item = together.Head
                                  textInDeclList item, textInCode item, items
                            ]
                 )
            // RFC-1137: concat previous result
            |> List.concat 
            // Filter out operators, active patterns (as values)
            |> List.filter (fun (_textInDeclList, textInCode, items) -> 
                not (isOperatorItem textInCode items) && 
                not (isActivePatternItem items))

            |> List.map (fun (textInDeclList, textInCode, itemsWithSameFullName) -> 
                let items =
                    match itemsWithSameFullName |> List.partition (fun x -> x.Unresolved.IsNone) with
                    | [], unresolved -> unresolved
                    // if there are resolvable items, throw out unresolved to prevent duplicates like `Set` and `FSharp.Collections.Set`.
                    | resolved, _ -> resolved 
                    
                let item = items.Head
                let glyph = GlyphOfItem(denv, item.Item)

                let cutAttributeSuffix (name: string) =
                    if isAttributeApplicationContext && name <> "Attribute" && name.EndsWithOrdinal("Attribute") && IsAttribute infoReader item.Item then
                        name[0..name.Length - "Attribute".Length - 1]
                    else name

                let textInDeclList = cutAttributeSuffix textInDeclList
                let textInCode = cutAttributeSuffix textInCode
                    
                let fullName = 
                    match item.Unresolved with
                    | Some x -> x.FullName
                    | None -> FullNameOfItem g item.Item
                    
                let namespaceToOpen = 
                    item.Unresolved 
                    |> Option.map (fun x -> x.Namespace)
                    |> Option.bind (fun ns ->
                        if ns |> Array.startsWith fsharpNamespace then None
                        else Some ns)
                    |> Option.map (fun ns ->
                        match currentNamespace with
                        | Some currentNs ->
                            if ns |> Array.startsWith currentNs then
                                ns[currentNs.Length..]
                            else ns
                        | None -> ns)
                    |> Option.bind (function
                        | [||] -> None
                        | ns -> Some (System.String.Join(".", ns)))

                DeclarationListItem(
                    textInDeclList, textInCode, fullName, glyph, Choice1Of2 (items, infoReader, ad, m, denv), getAccessibility item.Item,
                    item.Kind, item.IsOwnMember, item.MinorPriority, item.Unresolved.IsNone, namespaceToOpen))

        DeclarationListInfo(Array.ofList decls, isForType, false)
    
    static member Error message = 
        DeclarationListInfo(
                [| DeclarationListItem("<Note>", "<Note>", "<Note>", FSharpGlyph.Error, Choice2Of2 (ToolTipText [ToolTipElement.CompositionError message]),
                                             FSharpAccessibility(taccessPublic), CompletionItemKind.Other, false, 0, false, None) |], false, true)
    
    static member Empty = empty



/// Represents one method (or other item) in a method group. The item may represent either a method or 
/// a single, non-overloaded item such as union case or a named function value.
// Note: instances of this type do not hold any references to any compiler resources.
[<Sealed; NoEquality; NoComparison>]
type MethodGroupItem(description: ToolTipText, xmlDoc: FSharpXmlDoc,
                           returnType: TaggedText[], parameters: MethodGroupItemParameter[],
                           hasParameters: bool, hasParamArrayArg: bool, staticParameters: MethodGroupItemParameter[]) = 

    /// The description representation for the method (or other item)
    member _.Description = description

    /// The documentation for the item
    member _.XmlDoc = xmlDoc

    /// The return type text for the method (or other item)
    member _.ReturnTypeText = returnType

    /// The parameters of the method in the overload set
    member _.Parameters = parameters

    /// Does the method support an arguments list?  This is always true except for static type instantiations like TP<42, "foo">.
    member _.HasParameters = hasParameters

    /// Does the method support a params list arg?
    member _.HasParamArrayArg = hasParamArrayArg

    /// Does the type name or method support a static arguments list, like TP<42, "foo"> or conn.CreateCommand<42, "foo">(arg1, arg2)?
    member _.StaticParameters = staticParameters


/// A table of methods for Intellisense completion
//
// Note: this type does not hold any strong references to any compiler resources, nor does evaluating any of the properties execute any
// code on the compiler thread.  
[<Sealed>]
type MethodGroup( name: string, unsortedMethods: MethodGroupItem[] ) = 
    // BUG 413009 : [ParameterInfo] takes about 3 seconds to move from one overload parameter to another
    // cache allows to avoid recomputing parameterinfo for the same item
#if !FX_NO_WEAKTABLE
    static let methodOverloadsCache = System.Runtime.CompilerServices.ConditionalWeakTable<ItemWithInst, MethodGroupItem[]>()
#endif

    static let empty = MethodGroup ("", [| |])

    let methods = 
        unsortedMethods 
        // Methods with zero arguments show up here as taking a single argument of type 'unit'.  Patch them now to appear as having zero arguments.
        |> Array.map (fun meth -> 
            let params_ = meth.Parameters
            if params_.Length = 1 && params_[0].CanonicalTypeTextForSorting="Microsoft.FSharp.Core.Unit" then 
                MethodGroupItem(meth.Description, meth.XmlDoc, meth.ReturnTypeText, [||], true, meth.HasParamArrayArg, meth.StaticParameters) 
            else 
                meth)
        // Fix the order of methods, to be stable for unit testing.
        |> Array.sortBy (fun meth -> 
            let params_ = meth.Parameters
            params_.Length, (params_ |> Array.map (fun p -> p.CanonicalTypeTextForSorting)))

    member _.MethodName = name

    member _.Methods = methods

    static member Create (infoReader: InfoReader, ad, m, denv, items:ItemWithInst list) = 
        let g = infoReader.g
        if isNil items then empty else
        let name = items.Head.Item.DisplayName 

        let methods = 
          [| for item in items do 
#if !FX_NO_WEAKTABLE
               match methodOverloadsCache.TryGetValue item with
               | true, res -> yield! res
               | false, _ ->
#endif
                let flatItems = SelectMethodGroupItems g  m item.Item

                let methods = 
                    flatItems |> Array.ofList |> Array.map (fun flatItem -> 
                        let prettyParams, prettyRetTyL = 
                            DiagnosticsScope.Protect m 
                                (fun () -> PrettyParamsAndReturnTypeOfItem infoReader m denv  { item with Item = flatItem })
                                (fun err -> [], wordL (tagText err))
                            
                        let description = ToolTipText [FormatStructuredDescriptionOfItem true infoReader ad m denv { item with Item = flatItem } None None]

                        let hasParamArrayArg = 
                            match flatItem with 
                            | Item.CtorGroup(_, [meth]) 
                            | Item.MethodGroup(_, [meth], _) -> meth.HasParamArrayArg(infoReader.amap, m, meth.FormalMethodInst) 
                            | _ -> false

                        let hasStaticParameters = 
                            match flatItem with 
#if !NO_TYPEPROVIDERS
                            | ItemIsProvidedTypeWithStaticArguments m g _ -> false 
#endif
                            | _ -> true

                        let prettyRetTyL = toArray prettyRetTyL
                        MethodGroupItem(
                          description = description,
                          returnType = prettyRetTyL,
                          xmlDoc = GetXmlCommentForItem infoReader m flatItem,
                          parameters = (prettyParams |> Array.ofList),
                          hasParameters = hasStaticParameters,
                          hasParamArrayArg = hasParamArrayArg,
#if !NO_TYPEPROVIDERS
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

        MethodGroup(name, methods)

    static member internal Empty = empty
