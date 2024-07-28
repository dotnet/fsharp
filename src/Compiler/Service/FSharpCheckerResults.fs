// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Open up the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.

namespace FSharp.Compiler.CodeAnalysis

open System
open System.Collections.Generic
open System.Diagnostics
open System.IO
open System.Reflection
open System.Threading
open FSharp.Compiler.IO
open FSharp.Compiler.NicePrint
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open FSharp.Core.Printf
open FSharp.Compiler
open FSharp.Compiler.Syntax
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.CheckExpressions
open FSharp.Compiler.CheckDeclarations
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerDiagnostics
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.EditorServices
open FSharp.Compiler.EditorServices.DeclarationListHelpers
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open FSharp.Compiler.Infos
open FSharp.Compiler.InfoReader
open FSharp.Compiler.Lexhelp
open FSharp.Compiler.NameResolution
open FSharp.Compiler.OptimizeInputs
open FSharp.Compiler.Parser
open FSharp.Compiler.ParseAndCheckInputs
open FSharp.Compiler.ParseHelpers
open FSharp.Compiler.ScriptClosure
open FSharp.Compiler.Symbols
open FSharp.Compiler.Symbols.SymbolHelpers
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Layout
open FSharp.Compiler.Text.Position
open FSharp.Compiler.Text.Range
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps

open Internal.Utilities
open Internal.Utilities.Collections
open FSharp.Compiler.AbstractIL.ILBinaryReader
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Internal.Utilities.Hashing

type FSharpUnresolvedReferencesSet = FSharpUnresolvedReferencesSet of UnresolvedAssemblyReference list

[<RequireQualifiedAccess>]
type DocumentSource =
    | FileSystem
    | Custom of (string -> Async<ISourceText option>)

[<Sealed>]
type DelayedILModuleReader =
    val private name: string
    val private gate: obj
    val mutable private getStream: (CancellationToken -> Stream option)
    val mutable private result: ILModuleReader

    new(name, getStream) =
        {
            name = name
            gate = obj ()
            getStream = getStream
            result = Unchecked.defaultof<_>
        }

    member this.OutputFile = this.name

    member this.TryGetILModuleReader() =
        // fast path
        match box this.result with
        | null ->
            cancellable {
                let! ct = Cancellable.token ()

                return
                    lock this.gate (fun () ->
                        // see if we have a result or not after the lock so we do not evaluate the stream more than once
                        match box this.result with
                        | null ->
                            try
                                let streamOpt = this.getStream ct

                                match streamOpt with
                                | Some stream ->
                                    let ilReaderOptions: ILReaderOptions =
                                        {
                                            pdbDirPath = None
                                            reduceMemoryUsage = ReduceMemoryFlag.Yes
                                            metadataOnly = MetadataOnlyFlag.Yes
                                            tryGetMetadataSnapshot = fun _ -> None
                                        }

                                    let ilReader = OpenILModuleReaderFromStream this.name stream ilReaderOptions
                                    this.result <- ilReader
                                    this.getStream <- Unchecked.defaultof<_> // clear out the function so we do not hold onto anything
                                    Some ilReader
                                | _ -> None
                            with ex ->
                                Trace.TraceInformation("FCS: Unable to get an ILModuleReader: {0}", ex)
                                None
                        | _ -> Some this.result)
            }
        | _ -> cancellable.Return(Some this.result)

[<RequireQualifiedAccess; NoComparison; CustomEquality>]
type FSharpReferencedProject =
    | FSharpReference of projectOutputFile: string * options: FSharpProjectOptions
    | PEReference of getStamp: (unit -> DateTime) * delayedReader: DelayedILModuleReader
    | ILModuleReference of projectOutputFile: string * getStamp: (unit -> DateTime) * getReader: (unit -> ILModuleReader)

    member this.OutputFile =
        match this with
        | FSharpReference(projectOutputFile = projectOutputFile)
        | ILModuleReference(projectOutputFile = projectOutputFile) -> projectOutputFile
        | PEReference(delayedReader = reader) -> reader.OutputFile

    override this.Equals(o) =
        match o with
        | :? FSharpReferencedProject as o ->
            match this, o with
            | FSharpReference(projectOutputFile1, options1), FSharpReference(projectOutputFile2, options2) ->
                projectOutputFile1 = projectOutputFile2
                && FSharpProjectOptions.AreSameForChecking(options1, options2)
            | PEReference(getStamp1, reader1), PEReference(getStamp2, reader2) ->
                reader1.OutputFile = reader2.OutputFile && (getStamp1 ()) = (getStamp2 ())
            | ILModuleReference(projectOutputFile1, getStamp1, _), ILModuleReference(projectOutputFile2, getStamp2, _) ->
                projectOutputFile1 = projectOutputFile2 && (getStamp1 ()) = (getStamp2 ())
            | _ -> false
        | _ -> false

    override this.GetHashCode() = this.OutputFile.GetHashCode()

// NOTE: may be better just to move to optional arguments here
and FSharpProjectOptions =
    {
        ProjectFileName: string
        ProjectId: string option
        SourceFiles: string[]
        OtherOptions: string[]
        ReferencedProjects: FSharpReferencedProject[]
        IsIncompleteTypeCheckEnvironment: bool
        UseScriptResolutionRules: bool
        LoadTime: DateTime
        UnresolvedReferences: FSharpUnresolvedReferencesSet option
        OriginalLoadReferences: (range * string * string) list
        Stamp: int64 option
    }

    static member UseSameProject(options1, options2) =
        match options1.ProjectId, options2.ProjectId with
        | Some(projectId1), Some(projectId2) when
            not (String.IsNullOrWhiteSpace(projectId1))
            && not (String.IsNullOrWhiteSpace(projectId2))
            ->
            projectId1 = projectId2
        | Some _, Some _
        | None, None -> options1.ProjectFileName = options2.ProjectFileName
        | _ -> false

    static member AreSameForChecking(options1, options2) =
        match options1.Stamp, options2.Stamp with
        | Some x, Some y -> (x = y)
        | _ ->
            FSharpProjectOptions.UseSameProject(options1, options2)
            && options1.SourceFiles = options2.SourceFiles
            && options1.OtherOptions = options2.OtherOptions
            && options1.UnresolvedReferences = options2.UnresolvedReferences
            && options1.OriginalLoadReferences = options2.OriginalLoadReferences
            && options1.ReferencedProjects.Length = options2.ReferencedProjects.Length
            && options1.ReferencedProjects = options2.ReferencedProjects
            && options1.LoadTime = options2.LoadTime

    member po.ProjectDirectory = Path.GetDirectoryName(po.ProjectFileName)

    override this.ToString() =
        "FSharpProjectOptions(" + this.ProjectFileName + ")"

[<AutoOpen>]
module internal FSharpCheckerResultsSettings =

    let getToolTipTextSize = GetEnvInteger "FCS_GetToolTipTextCacheSize" 5

    let maxTypeCheckErrorsOutOfProjectContext =
        GetEnvInteger "FCS_MaxErrorsOutOfProjectContext" 3

    // Look for DLLs in the location of the service DLL first.
    let defaultFSharpBinariesDir =
        FSharpEnvironment
            .BinFolderOfDefaultFSharpCompiler(Some(Path.GetDirectoryName(typeof<IncrementalBuilder>.Assembly.Location)))
            .Value

[<Sealed>]
type FSharpSymbolUse(denv: DisplayEnv, symbol: FSharpSymbol, inst: TyparInstantiation, itemOcc, range: range) =

    member _.Symbol = symbol

    member _.GenericArguments =
        let cenv = symbol.SymbolEnv

        inst
        |> List.map (fun (v, ty) -> FSharpGenericParameter(cenv, v), FSharpType(cenv, ty))

    member _.DisplayContext = FSharpDisplayContext(fun _ -> denv)

    member x.IsDefinition = x.IsFromDefinition

    member _.IsFromDefinition = itemOcc = ItemOccurence.Binding

    member _.IsFromPattern = itemOcc = ItemOccurence.Pattern

    member _.IsFromType = itemOcc = ItemOccurence.UseInType

    member _.IsFromAttribute = itemOcc = ItemOccurence.UseInAttribute

    member _.IsFromDispatchSlotImplementation = itemOcc = ItemOccurence.Implemented

    member _.IsFromUse = itemOcc = ItemOccurence.Use

    member _.IsFromComputationExpression =
        match symbol.Item, itemOcc with
        // 'seq' in 'seq { ... }' gets colored as keywords
        | Item.Value vref, ItemOccurence.Use when valRefEq denv.g denv.g.seq_vref vref -> true
        // custom builders, custom operations get colored as keywords
        | (Item.CustomBuilder _ | Item.CustomOperation _), ItemOccurence.Use -> true
        | _ -> false

    member _.IsFromOpenStatement = itemOcc = ItemOccurence.Open

    member _.FileName = range.FileName

    member _.Range = range

    member this.IsPrivateToFileAndSignatureFile =

        let couldBeParameter, declarationLocation =
            match this.Symbol with
            | :? FSharpParameter as p -> true, Some p.DeclarationLocation
            | :? FSharpMemberOrFunctionOrValue as m when not m.IsModuleValueOrMember -> true, Some m.DeclarationLocation
            | _ -> false, None

        let thisIsSignature = SourceFileImpl.IsSignatureFile this.Range.FileName

        let signatureLocation = this.Symbol.SignatureLocation

        couldBeParameter
        && (thisIsSignature
            || (signatureLocation.IsSome && signatureLocation <> declarationLocation))

    member this.IsPrivateToFile =

        let isPrivate =
            match this.Symbol with
            | _ when this.IsPrivateToFileAndSignatureFile -> false
            | :? FSharpMemberOrFunctionOrValue as m when not m.IsModuleValueOrMember ->
                // local binding or parameter
                true
            | :? FSharpMemberOrFunctionOrValue as m ->
                let fileSignatureLocation =
                    m.DeclaringEntity |> Option.bind (fun e -> e.SignatureLocation)

                let fileDeclarationLocation =
                    m.DeclaringEntity |> Option.map (fun e -> e.DeclarationLocation)

                let fileHasSignatureFile = fileSignatureLocation <> fileDeclarationLocation

                fileHasSignatureFile && not m.HasSignatureFile || m.Accessibility.IsPrivate
            | :? FSharpEntity as m -> m.Accessibility.IsPrivate
            | :? FSharpParameter -> true
            | :? FSharpGenericParameter -> true
            | :? FSharpUnionCase as m -> m.Accessibility.IsPrivate
            | :? FSharpField as m -> m.Accessibility.IsPrivate
            | :? FSharpActivePatternCase as m -> m.Accessibility.IsPrivate
            | _ -> false

        let declarationLocation =
            match this.Symbol.SignatureLocation with
            | Some x -> Some x
            | _ ->
                match this.Symbol.DeclarationLocation with
                | Some x -> Some x
                | _ -> this.Symbol.ImplementationLocation

        let declaredInTheFile =
            match declarationLocation with
            | Some declRange -> declRange.FileName = this.Range.FileName
            | _ -> false

        isPrivate && declaredInTheFile

    override _.ToString() =
        sprintf "%O, %O, %O" symbol itemOcc range

/// This type is used to describe what was found during the name resolution.
/// (Depending on the kind of the items, we may stop processing or continue to find better items)
[<RequireQualifiedAccess; NoEquality; NoComparison>]
type NameResResult =
    | Members of (ItemWithInst list * DisplayEnv * range)
    | Cancel of DisplayEnv * range
    | Empty

[<RequireQualifiedAccess>]
type ResolveOverloads =
    | Yes
    | No

[<RequireQualifiedAccess>]
type ExprTypingsResult =
    | NoneBecauseTypecheckIsStaleAndTextChanged
    | NoneBecauseThereWereTypeErrors
    | None
    | Some of (ItemWithInst list * DisplayEnv * range) * TType

type Names = string list

/// A TypeCheckInfo represents everything we get back from the typecheck of a file.
/// It acts like an in-memory database about the file.
/// It is effectively immutable and not updated: when we re-typecheck we just drop the previous
/// scope object on the floor and make a new one.
[<Sealed>]
type internal TypeCheckInfo
    (
        _sTcConfig: TcConfig,
        g: TcGlobals,
        ccuSigForFile: ModuleOrNamespaceType,
        thisCcu: CcuThunk,
        tcImports: TcImports,
        tcAccessRights: AccessorDomain,
        projectFileName: string,
        mainInputFileName: string,
        projectOptions: FSharpProjectOptions,
        sResolutions: TcResolutions,
        sSymbolUses: TcSymbolUses,
        sFallback: NameResolutionEnv,
        loadClosure: LoadClosure option,
        implFileOpt: CheckedImplFile option,
        openDeclarations: OpenDeclaration[]
    ) =

    // These strings are potentially large and the editor may choose to hold them for a while.
    // Use this cache to fold together data tip text results that are the same.
    // Is not keyed on 'Names' collection because this is invariant for the current position in
    // this unchanged file. Keyed on lineStr though to prevent a change to the currently line
    // being available against a stale scope.
    let getToolTipTextCache =
        AgedLookup<AnyCallerThreadToken, int * int * string * int option, ToolTipText>(
            getToolTipTextSize,
            areSimilar = (fun (x, y) -> x = y)
        )

    let amap = tcImports.GetImportMap()
    let infoReader = InfoReader(g, amap)
    let ncenv = NameResolver(g, amap, infoReader, FakeInstantiationGenerator)
    let cenv = SymbolEnv(g, thisCcu, Some ccuSigForFile, tcImports, amap, infoReader)

    /// Find the most precise naming environment for the given line and column
    let GetBestEnvForPos cursorPos =

        let mutable bestSoFar = None

        // Find the most deeply nested enclosing scope that contains given position
        sResolutions.CapturedEnvs
        |> ResizeArray.iter (fun (mPossible, env, ad) ->
            if rangeContainsPos mPossible cursorPos then
                match bestSoFar with
                | Some(bestm, _, _) ->
                    if rangeContainsRange bestm mPossible then
                        bestSoFar <- Some(mPossible, env, ad)
                | None -> bestSoFar <- Some(mPossible, env, ad))

        let mostDeeplyNestedEnclosingScope = bestSoFar

        // Look for better subtrees on the r.h.s. of the subtree to the left of where we are
        // Should really go all the way down the r.h.s. of the subtree to the left of where we are
        // This is all needed when the index is floating free in the area just after the environment we really want to capture
        // We guarantee to only refine to a more nested environment.  It may not be strictly
        // the right environment, but will always be at least as rich

        let mutable bestAlmostIncludedSoFar = None

        sResolutions.CapturedEnvs
        |> ResizeArray.iter (fun (mPossible, env, ad) ->
            // take only ranges that strictly do not include cursorPos (all ranges that touch cursorPos were processed during 'Strict Inclusion' part)
            if rangeBeforePos mPossible cursorPos && not (posEq mPossible.End cursorPos) then
                let contained =
                    match mostDeeplyNestedEnclosingScope with
                    | Some(bestm, _, _) -> rangeContainsRange bestm mPossible
                    | None -> true

                if contained then
                    match bestAlmostIncludedSoFar with
                    | Some(mRight: range, _, _) ->
                        if
                            posGt mPossible.End mRight.End
                            || (posEq mPossible.End mRight.End && posGt mPossible.Start mRight.Start)
                        then
                            bestAlmostIncludedSoFar <- Some(mPossible, env, ad)
                    | _ -> bestAlmostIncludedSoFar <- Some(mPossible, env, ad))

        let resEnv =
            match bestAlmostIncludedSoFar, mostDeeplyNestedEnclosingScope with
            | Some(_, env, ad), None -> env, ad
            | Some(_, almostIncludedEnv, ad), Some(_, mostDeeplyNestedEnv, _) when
                almostIncludedEnv.eFieldLabels.Count >= mostDeeplyNestedEnv.eFieldLabels.Count
                ->
                almostIncludedEnv, ad
            | _ ->
                match mostDeeplyNestedEnclosingScope with
                | Some(_, env, ad) -> env, ad
                | None -> sFallback, AccessibleFromSomeFSharpCode

        let pm = mkRange mainInputFileName cursorPos cursorPos

        resEnv, pm

    /// The items that come back from ResolveCompletionsInType are a bit
    /// noisy. Filter a few things out.
    ///
    /// e.g. prefer types to constructors for ToolTipText
    let FilterItemsForCtors filterCtors (items: ItemWithInst list) =
        let items =
            items
            |> List.filter (fun item ->
                match item.Item with
                | Item.CtorGroup _ when filterCtors = ResolveTypeNamesToTypeRefs -> false
                | _ -> true)

        items

    // Filter items to show only valid & return Some if there are any
    let ReturnItemsOfType (items: ItemWithInst list) g denv (m: range) filterCtors =
        let items =
            items
            |> RemoveDuplicateItems g
            |> RemoveExplicitlySuppressed g
            |> FilterItemsForCtors filterCtors

        if not (isNil items) then
            NameResResult.Members(items, denv, m)
        else
            NameResResult.Empty

    let GetCapturedNameResolutions (endOfNamesPos: pos) resolveOverloads =
        let filter (endPos: pos) items =
            items
            |> ResizeArray.filter (fun (cnr: CapturedNameResolution) ->
                let range = cnr.Range
                range.EndLine = endPos.Line && range.EndColumn = endPos.Column)

        match resolveOverloads with
        | ResolveOverloads.Yes -> filter endOfNamesPos sResolutions.CapturedNameResolutions

        | ResolveOverloads.No ->
            let items = filter endOfNamesPos sResolutions.CapturedMethodGroupResolutions

            if items.Count <> 0 then
                items
            else
                filter endOfNamesPos sResolutions.CapturedNameResolutions

    /// Looks at the exact name resolutions that occurred during type checking
    /// If 'membersByResidue' is specified, we look for members of the item obtained
    /// from the name resolution and filter them by the specified residue (?)
    let GetPreciseItemsFromNameResolution (line, colAtEndOfNames, membersByResidue, filterCtors, resolveOverloads) =
        let endOfNamesPos = mkPos line colAtEndOfNames

        // Logic below expects the list to be in reverse order of resolution
        let cnrs =
            GetCapturedNameResolutions endOfNamesPos resolveOverloads
            |> ResizeArray.toList
            |> List.rev

        match cnrs, membersByResidue with

        // Exact resolution via SomeType.$ or SomeType<int>.$
        //
        // If we're looking for members using a residue, we'd expect only
        // a single item (pick the first one) and we need the residue (which may be "")
        | CNR(_, ItemOccurence.InvalidUse, _, _, _, _) :: _, _ -> NameResResult.Empty

        | CNR(Item.Types(_, ty :: _), _, denv, nenv, ad, m) :: _, Some _ ->
            let targets =
                ResolveCompletionTargets.All(ConstraintSolver.IsApplicableMethApprox g amap m)

            let items = ResolveCompletionsInType ncenv nenv targets m ad true ty
            let items = List.map ItemWithNoInst items
            ReturnItemsOfType items g denv m filterCtors

        // Exact resolution via 'T.$
        | CNR(Item.TypeVar(_, tp), _, denv, nenv, ad, m) :: _, Some _ ->
            let targets =
                ResolveCompletionTargets.All(ConstraintSolver.IsApplicableMethApprox g amap m)

            let items = ResolveCompletionsInType ncenv nenv targets m ad true (mkTyparTy tp)
            let items = List.map ItemWithNoInst items
            ReturnItemsOfType items g denv m filterCtors

        // Value reference from the name resolution. Primarily to disallow "let x.$ = 1"
        // In most of the cases, value references can be obtained from expression typings or from environment,
        // so we wouldn't have to handle values here. However, if we have something like:
        //   let varA = "string"
        //   let varA = if b then 0 else varA.
        // then the expression typings get confused (thinking 'varA:int'), so we use name resolution even for usual values.

        | CNR(Item.Value(vref), occurence, denv, nenv, ad, m) :: _, Some _ ->
            if occurence = ItemOccurence.Binding || occurence = ItemOccurence.Pattern then
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
                        let thisTy = generalizedTyconRef g tcref
                        // check that type of value is the same or subtype of tcref
                        // yes - allow access to protected members
                        // no - strip ability to access protected members
                        if TypeRelations.TypeFeasiblySubsumesType 0 g amap m thisTy TypeRelations.CanCoerce ty then
                            ad
                        else
                            AccessibleFrom(paths, None)
                    | _ -> ad

                let targets =
                    ResolveCompletionTargets.All(ConstraintSolver.IsApplicableMethApprox g amap m)

                let items = ResolveCompletionsInType ncenv nenv targets m ad false ty
                let items = List.map ItemWithNoInst items
                ReturnItemsOfType items g denv m filterCtors

        // No residue, so the items are the full resolution of the name
        | CNR(_, _, denv, _, _, m) :: _, None ->
            let items =
                cnrs
                |> List.map (fun cnr -> cnr.ItemWithInst)
                // "into" is special magic syntax, not an identifier or a library call.  It is part of capturedNameResolutions as an
                // implementation detail of syntax coloring, but we should not report name resolution results for it, to prevent spurious QuickInfo.
                |> List.filter (fun item ->
                    match item.Item with
                    | Item.CustomOperation(CustomOperations.Into, _, _) -> false
                    | _ -> true)

            ReturnItemsOfType items g denv m filterCtors
        | _, _ -> NameResResult.Empty

    let TryGetTypeFromNameResolution (line, colAtEndOfNames, membersByResidue, resolveOverloads) =
        let endOfNamesPos = mkPos line colAtEndOfNames

        let items =
            GetCapturedNameResolutions endOfNamesPos resolveOverloads
            |> ResizeArray.toList
            |> List.rev

        match items, membersByResidue with
        | CNR(Item.Types(_, ty :: _), _, _, _, _, _) :: _, Some _ -> Some ty
        | CNR(Item.Value(vref), occurence, _, _, _, _) :: _, Some _ ->
            if (occurence = ItemOccurence.Binding || occurence = ItemOccurence.Pattern) then
                None
            else
                Some(StripSelfRefCell(g, vref.BaseOrThisInfo, vref.TauType))
        | _, _ -> None

    /// Build a CompletionItem
    let CompletionItemWithMoreSetting
        (ty: TyconRef voption)
        (assemblySymbol: AssemblySymbol voption)
        minorPriority
        insertText
        displayText
        (item: ItemWithInst)
        =
        let kind =
            match item.Item with
            | Item.DelegateCtor _
            | Item.CtorGroup _ -> CompletionItemKind.Method false
            | Item.MethodGroup(_, minfos, _) ->
                match minfos with
                | [] -> CompletionItemKind.Method false
                | minfo :: _ -> CompletionItemKind.Method minfo.IsExtensionMember
            | Item.AnonRecdField _
            | Item.RecdField _
            | Item.UnionCaseField _
            | Item.Property _ -> CompletionItemKind.Property
            | Item.Event _ -> CompletionItemKind.Event
            | Item.ILField _
            | Item.Value _ -> CompletionItemKind.Field
            | Item.CustomOperation _ -> CompletionItemKind.CustomOperation
            // These items are not given a completion kind. This could be reviewed
            | Item.ActivePatternResult _
            | Item.ExnCase _
            | Item.ImplicitOp _
            | Item.ModuleOrNamespaces _
            | Item.Trait _
            | Item.TypeVar _
            | Item.Types _
            | Item.UnionCase _
            | Item.UnqualifiedType _
            | Item.NewDef _
            | Item.SetterArg _
            | Item.CustomBuilder _
            | Item.OtherName _
            | Item.ActivePatternCase _ -> CompletionItemKind.Other

        let isUnresolved =
            match assemblySymbol with
            | ValueSome x -> Some x.UnresolvedSymbol
            | _ -> None

        let ty =
            match ty with
            | ValueSome x -> Some x
            | _ -> None

        {
            ItemWithInst = item
            MinorPriority = minorPriority
            Kind = kind
            IsOwnMember = false
            Type = ty
            Unresolved = isUnresolved
            CustomInsertText = insertText
            CustomDisplayText = displayText
        }

    let CompletionItem (ty: TyconRef voption) (assemblySymbol: AssemblySymbol voption) (item: ItemWithInst) =
        CompletionItemWithMoreSetting ty assemblySymbol 0 ValueNone ValueNone item

    let CollectParameters (methods: MethInfo list) amap m : Item list =
        methods
        |> List.collect (fun meth ->
            match meth.GetParamDatas(amap, m, meth.FormalMethodInst) with
            | x :: _ ->
                x
                |> List.choose (fun (ParamData(_isParamArray, _isInArg, _isOutArg, _optArgInfo, _callerInfo, name, _, ty)) ->
                    match name with
                    | Some id -> Some(Item.OtherName(Some id, ty, None, Some(ArgumentContainer.Method meth), id.idRange))
                    | None -> None)
            | _ -> [])

    let GetNamedParametersAndSettableFields endOfExprPos =
        let cnrs =
            GetCapturedNameResolutions endOfExprPos ResolveOverloads.No
            |> ResizeArray.toList
            |> List.rev

        let result =
            match cnrs with
            | CNR(Item.CtorGroup(_, (ctor :: _ as ctors)), _, denv, nenv, ad, m) :: _ ->
                let props =
                    ResolveCompletionsInType
                        ncenv
                        nenv
                        ResolveCompletionTargets.SettablePropertiesAndFields
                        m
                        ad
                        false
                        ctor.ApparentEnclosingType

                let parameters = CollectParameters ctors amap m
                let items = props @ parameters
                Some(denv, m, items)
            | CNR(Item.MethodGroup(_, methods, _), _, denv, nenv, ad, m) :: _ ->
                let props =
                    methods
                    |> List.collect (fun meth ->
                        let retTy = meth.GetFSharpReturnType(amap, m, meth.FormalMethodInst)
                        ResolveCompletionsInType ncenv nenv ResolveCompletionTargets.SettablePropertiesAndFields m ad false retTy)

                let parameters = CollectParameters methods amap m
                let items = props @ parameters
                Some(denv, m, items)
            | _ -> None

        match result with
        | None -> NameResResult.Empty
        | Some(denv, m, items) ->
            let items = List.map ItemWithNoInst items
            ReturnItemsOfType items g denv m TypeNameResolutionFlag.ResolveTypeNamesToTypeRefs

    /// finds captured typing for the given position
    let GetExprTypingForPosition endOfExprPos =
        let quals =
            sResolutions.CapturedExpressionTypings
            |> Seq.filter (fun (ty, nenv, _, m) ->
                // We only want expression types that end at the particular position in the file we are looking at.
                posEq m.End endOfExprPos
                &&

                // Get rid of function types.  True, given a 2-arg curried function "f x y", it is legal to do "(f x).GetType()",
                // but you almost never want to do this in practice, and we choose not to offer up any intellisense for
                // F# function types.
                not (isFunTy nenv.DisplayEnv.g ty))
            |> Seq.toArray

        // filter out errors

        let quals =
            quals
            |> Array.filter (fun (ty, nenv, _, _) ->
                let denv = nenv.DisplayEnv
                not (isTyparTy denv.g ty && (destTyparTy denv.g ty).IsFromError))

        let thereWereSomeQuals = not (Array.isEmpty quals)
        thereWereSomeQuals, quals

    /// Returns the list of available record fields, taking into account potential nesting
    let GetRecdFieldsForCopyAndUpdateExpr (identRange: range, plid: string list) =
        let rec dive ty (denv: DisplayEnv) ad m plid isPastTypePrefix wasPathEmpty =
            if isRecdTy denv.g ty then
                let fields =
                    ncenv.InfoReader.GetRecordOrClassFieldsOfType(None, ad, m, ty)
                    |> List.filter (fun rfref -> not rfref.IsStatic && IsFieldInfoAccessible ad rfref)

                match plid with
                | [] ->
                    if wasPathEmpty || isPastTypePrefix then
                        Some(fields |> List.map Item.RecdField, denv, m)
                    else
                        None
                | id :: rest ->
                    match fields |> List.tryFind (fun f -> f.LogicalName = id) with
                    | Some f -> dive f.FieldType denv ad m rest true wasPathEmpty
                    | _ ->
                        // Field name can be optionally qualified.
                        // If we haven't matched a field name yet, keep peeling off the prefix.
                        if isPastTypePrefix then
                            Some([], denv, m)
                        else
                            dive ty denv ad m rest false wasPathEmpty
            else
                match tryDestAnonRecdTy denv.g ty with
                | ValueSome(anonInfo, tys) ->
                    match plid with
                    | [] ->
                        let items =
                            [
                                for i in 0 .. anonInfo.SortedIds.Length - 1 do
                                    Item.AnonRecdField(anonInfo, tys, i, anonInfo.SortedIds[i].idRange)
                            ]

                        Some(items, denv, m)
                    | id :: rest ->
                        match anonInfo.SortedNames |> Array.tryFindIndex (fun x -> x = id) with
                        | Some i -> dive tys[i] denv ad m rest true wasPathEmpty
                        | _ -> Some([], denv, m)
                | ValueNone -> Some([], denv, m)

        match
            GetExprTypingForPosition identRange.End
            |> snd
            |> Array.tryFind (fun (_, _, _, rq) -> posEq identRange.Start rq.Start)
        with
        | Some(ty, nenv, ad, m) -> dive ty nenv.DisplayEnv ad m plid false plid.IsEmpty
        | _ -> None

    /// Looks at the exact expression types at the position to the left of the
    /// residue then the source when it was typechecked.
    let GetPreciseCompletionListFromExprTypings (parseResults: FSharpParseFileResults, endOfExprPos, filterCtors) =

        let thereWereSomeQuals, quals = GetExprTypingForPosition(endOfExprPos)

        match quals with
        | [||] ->
            if thereWereSomeQuals then
                ExprTypingsResult.NoneBecauseThereWereTypeErrors
            else
                ExprTypingsResult.None
        | _ ->
            let bestQual, textChanged =
                let input = parseResults.ParseTree

                match ParsedInput.GetRangeOfExprLeftOfDot(endOfExprPos, input) with // TODO we say "colAtEndOfNames" everywhere, but that's not really a good name ("foo  .  $" hit Ctrl-Space at $)
                | Some(exprRange) ->
                    // We have an up-to-date sync parse, and know the exact range of the prior expression.
                    // The quals all already have the same ending position, so find one with a matching starting position, if it exists.
                    // If not, then the stale typecheck info does not have a capturedExpressionTyping for this exact expression, and the
                    // user can wait for typechecking to catch up and second-chance intellisense to give the right result.
                    let qual =
                        quals
                        |> Array.tryFind (fun (_, _, _, r) ->
                            ignore (r) // for breakpoint
                            posEq exprRange.Start r.Start)

                    qual, false
                | None ->
                    // TODO In theory I think we should never get to this code path; it would be nice to add an assert.
                    // In practice, we do get here in some weird cases like "2.0 .. 3.0" and hitting Ctrl-Space in between the two dots of the range operator.
                    // I wasn't able to track down what was happening in those weird cases, not worth worrying about, it doesn't manifest as a product bug or anything.
                    None, false

            match bestQual with
            | Some bestQual ->
                let ty, nenv, ad, m = bestQual

                let targets =
                    ResolveCompletionTargets.All(ConstraintSolver.IsApplicableMethApprox g amap m)

                let items = ResolveCompletionsInType ncenv nenv targets m ad false ty
                let items = items |> List.map ItemWithNoInst
                let items = items |> RemoveDuplicateItems g
                let items = items |> RemoveExplicitlySuppressed g
                let items = items |> FilterItemsForCtors filterCtors
                ExprTypingsResult.Some((items, nenv.DisplayEnv, m), ty)
            | None ->
                if textChanged then
                    ExprTypingsResult.NoneBecauseTypecheckIsStaleAndTextChanged
                else
                    ExprTypingsResult.None

    /// Find items in the best naming environment.
    let GetEnvironmentLookupResolutions (nenv, ad, m, plid, filterCtors, showObsolete) =
        let items =
            ResolvePartialLongIdent ncenv nenv (ConstraintSolver.IsApplicableMethApprox g amap m) m ad plid showObsolete

        let items = items |> List.map ItemWithNoInst
        let items = items |> RemoveDuplicateItems g
        let items = items |> RemoveExplicitlySuppressed g
        let items = items |> FilterItemsForCtors filterCtors
        (items, nenv.DisplayEnv, m)

    /// Find items in the best naming environment.
    let GetEnvironmentLookupResolutionsAtPosition (cursorPos, plid, filterCtors, showObsolete) =
        let (nenv, ad), m = GetBestEnvForPos cursorPos
        GetEnvironmentLookupResolutions(nenv, ad, m, plid, filterCtors, showObsolete)

    /// Find record fields in the best naming environment.
    let GetClassOrRecordFieldsEnvironmentLookupResolutions (cursorPos, plid, fieldsOnly) =
        let (nenv, ad), m = GetBestEnvForPos cursorPos

        let items =
            ResolvePartialLongIdentToClassOrRecdFields ncenv nenv m ad plid false fieldsOnly

        let items = items |> List.map ItemWithNoInst
        let items = items |> RemoveDuplicateItems g
        let items = items |> RemoveExplicitlySuppressed g
        items, nenv.DisplayEnv, m

    /// Is the item suitable for completion at "inherits $"
    let IsInheritsCompletionCandidate item =
        match item with
        | Item.ModuleOrNamespaces _ -> true
        | Item.Types(_, ty :: _) when isClassTy g ty && not (isSealedTy g ty) -> true
        | _ -> false

    /// Is the item suitable for completion at "interface $"
    let IsInterfaceCompletionCandidate item =
        match item with
        | Item.ModuleOrNamespaces _ -> true
        | Item.Types(_, ty :: _) when isInterfaceTy g ty -> true
        | _ -> false

    /// Is the item suitable for completion in a pattern
    let IsPatternCandidate (item: CompletionItem) =
        match item.Item with
        | Item.Value v -> v.LiteralValue.IsSome
        | Item.ILField field -> field.LiteralValue.IsSome
        | Item.ActivePatternCase _
        | Item.ExnCase _
        | Item.ModuleOrNamespaces _
        | Item.Types _
        | Item.UnionCase _ -> true
        | _ -> false

    /// Is the item suitable for completion in a type application or type annotation
    let IsTypeCandidate (item: CompletionItem) =
        match item.Item with
        | Item.ModuleOrNamespaces _
        | Item.Types _
        | Item.TypeVar _
        | Item.UnqualifiedType _
        | Item.ExnCase _ -> true
        | _ -> false

    /// Return only items with the specified name, modulo "Attribute" for type completions
    let FilterDeclItemsByResidue (getItem: 'a -> Item) residue (items: 'a list) =
        let attributedResidue = residue + "Attribute"

        let nameMatchesResidue name =
            (residue = name) || (attributedResidue = name)

        items
        |> List.filter (fun x ->
            let item = getItem x
            let n1 = item.DisplayName

            match item with
            | Item.Types _ -> nameMatchesResidue n1
            | Item.CtorGroup(_, meths) ->
                nameMatchesResidue n1
                || meths
                   |> List.exists (fun meth ->
                       let tcref = meth.ApparentEnclosingTyconRef
#if !NO_TYPEPROVIDERS
                       tcref.IsProvided
                       ||
#endif
                       nameMatchesResidue tcref.DisplayName)
            | Item.Value v when (v.IsPropertyGetterMethod || v.IsPropertySetterMethod) -> residue = v.Id.idText || residue = n1
            | _ -> residue = n1)

    /// Post-filter items to make sure they have precisely the right name
    /// This also checks that there are some remaining results
    /// exactMatchResidueOpt = Some _ -- means that we are looking for exact matches
    let FilterRelevantItemsBy (getItem: 'a -> Item) (exactMatchResidueOpt: string option) check (items: 'a list, denv, m) =
        // can throw if type is in located in non-resolved CCU: i.e. bigint if reference to System.Numerics is absent
        let inline safeCheck item =
            try
                check item
            with _ ->
                false

        // Are we looking for items with precisely the given name?
        if isNil items then
            // When (items = []) we must returns Some([],..) and not None
            // because this value is used if we want to stop further processing (e.g. let x.$ = ...)
            Some(items, denv, m)
        else
            match exactMatchResidueOpt with
            | Some exactMatchResidue ->
                let items =
                    items
                    |> FilterDeclItemsByResidue getItem exactMatchResidue
                    |> List.filter safeCheck

                if not (isNil items) then Some(items, denv, m) else None
            | _ ->
                let items = items |> List.filter safeCheck
                Some(items, denv, m)

    /// Post-filter items to make sure they have precisely the right name
    /// This also checks that there are some remaining results
    let (|FilterRelevantItems|_|) getItem exactMatchResidueOpt orig =
        FilterRelevantItemsBy getItem exactMatchResidueOpt (fun _ -> true) orig

    /// Find the first non-whitespace position in a line prior to the given character
    let FindFirstNonWhitespacePosition (lineStr: string) i =
        if i >= lineStr.Length then
            None
        else
            let mutable p = i

            while p >= 0 && Char.IsWhiteSpace(lineStr[p]) do
                p <- p - 1

            if p >= 0 then Some p else None

    let DefaultCompletionItem item = CompletionItem ValueNone ValueNone item

    let CompletionItemSuggestedName displayName =
        {
            ItemWithInst = ItemWithNoInst(Item.NewDef(Ident(displayName, range0)))
            MinorPriority = 0
            Type = None
            Kind = CompletionItemKind.SuggestedName
            IsOwnMember = false
            Unresolved = None
            CustomInsertText = ValueNone
            CustomDisplayText = ValueNone
        }

    let getItem (x: ItemWithInst) = x.Item

    let getItem2 (x: CompletionItem) = x.Item

    /// Checks whether the suggested name is unused.
    /// In the future we could use an increasing numeric suffix for conflict resolution
    let CreateCompletionItemForSuggestedPatternName (pos: pos) name =
        if String.IsNullOrWhiteSpace name then
            None
        else
            let name = String.lowerCaseFirstChar name

            let unused =
                sResolutions.CapturedNameResolutions
                |> ResizeArray.forall (fun r ->
                    match r.Item with
                    | Item.Value vref when r.Pos.Line = pos.Line -> vref.DisplayName <> name
                    | _ -> true)

            if unused then
                Some(CompletionItemSuggestedName name)
            else
                None

    /// Suggest name based on type
    let SuggestNameBasedOnType (g: TcGlobals) pos ty =
        match stripTyparEqns ty with
        | TType_app(tyconRef = tcref) when tcref.IsTypeAbbrev && (tcref.IsLocalRef || not (ccuEq g.fslibCcu tcref.nlr.Ccu)) ->
            // Respect user-defined aliases
            CreateCompletionItemForSuggestedPatternName pos tcref.DisplayName
        | ty ->
            if isNumericType g ty then
                CreateCompletionItemForSuggestedPatternName pos "num"
            else
                match tryTcrefOfAppTy g ty with
                | ValueSome tcref when not (tyconRefEq g g.system_Object_tcref tcref) ->
                    CreateCompletionItemForSuggestedPatternName pos tcref.DisplayName
                | _ -> None

    /// Suggest names based on field name and type, add them to the list
    let SuggestNameForUnionCaseFieldPattern g caseIdPos fieldPatternPos (uci: UnionCaseInfo) indexOrName isTheOnlyField completions =
        let field =
            match indexOrName with
            | Choice1Of2 index ->
                match uci.UnionCase.RecdFieldsArray, index with
                | [| field |], None ->
                    // Index is None when parentheses were not used, i.e. `| Some v ->` - suggest a name only when the case has a single field
                    Some field
                | [| _ |], Some _ when not isTheOnlyField ->
                    // When completing `| Some (a| , b)`, we're binding the first tuple element, not the sole case field
                    None
                | _, None -> None
                | arr, Some index -> arr |> Array.tryItem index
            | Choice2Of2 name -> uci.UnionCase.RecdFieldsArray |> Array.tryFind (fun x -> x.DisplayName = name)

        field
        |> Option.map (fun field ->
            let ty =
                // If the field type is generic, suggest a name based on the solution
                if isTyparTy g field.FormalType then
                    sResolutions.CapturedNameResolutions
                    |> ResizeArray.tryPick (fun r ->
                        match r.Item with
                        | Item.Value vref when r.Pos = fieldPatternPos -> Some vref.Type
                        | _ -> None)
                    |> Option.defaultValue field.FormalType
                else
                    field.FormalType

            let fieldName =
                // If the field has not been given an explicit name, do not suggest the generated one
                if field.rfield_name_generated then
                    ""
                else
                    field.DisplayName

            completions
            |> List.prependIfSome (SuggestNameBasedOnType g caseIdPos ty)
            |> List.prependIfSome (CreateCompletionItemForSuggestedPatternName caseIdPos fieldName))
        |> Option.defaultValue completions

    /// Gets all methods that a type can override, but has not yet done so.
    let GetOverridableMethods pos ctx (typeNameRange: range) spacesBeforeOverrideKeyword hasThis isStatic =
        let checkImplementedSlotDeclareType ty slots =
            slots
            |> Option.map (List.exists (fun (TSlotSig(declaringType = ty2)) -> typeEquiv g ty ty2))
            |> Option.defaultValue false

        let isMethodOverridable superTy alreadyOverridden (candidate: MethInfo) =
            not candidate.IsFinal
            && not (
                alreadyOverridden
                |> ResizeArray.exists (fun i ->
                    MethInfosEquivByNameAndSig EraseNone true g amap range0 candidate i
                    && (tyconRefEq g candidate.DeclaringTyconRef i.DeclaringTyconRef
                        || checkImplementedSlotDeclareType superTy (Option.attempt (fun () -> i.ImplementedSlotSignatures))))
            )

        let isMethodOptionOverridable superTy alreadyOverridden candidate =
            candidate
            |> ValueOption.map (fun i -> isMethodOverridable superTy alreadyOverridden i)
            |> ValueOption.defaultValue false

        let isPropertyOverridable superTy alreadyOverridden (candidate: PropInfo) =
            if candidate.IsVirtualProperty then
                let getterOverridden, setterOverridden =
                    alreadyOverridden
                    |> List.filter (fun i ->
                        PropInfosEquivByNameAndSig EraseNone g amap range0 candidate i
                        && (tyconRefEq g candidate.DeclaringTyconRef i.DeclaringTyconRef
                            || checkImplementedSlotDeclareType superTy (Option.attempt (fun () -> i.ImplementedSlotSignatures))))
                    |> List.fold
                        (fun (getterOverridden, setterOverridden) i -> getterOverridden || i.HasGetter, setterOverridden || i.HasSetter)
                        (false, false)

                not getterOverridden, not setterOverridden
            else
                false, false

        let rec checkOrGenerateArgName (nameSet: HashSet<_>) name =
            let name = if String.IsNullOrEmpty name then "arg" else name

            if nameSet.Add(name) then
                name
            else
                checkOrGenerateArgName nameSet $"{name}_{nameSet.Count}"

        let (nenv, ad), m = GetBestEnvForPos pos
        let denv = nenv.DisplayEnv

        let checkMethAbstractAndGetImplementBody (meth: MethInfo) implementBody =
            if meth.IsAbstract then
                if nenv.DisplayEnv.openTopPathsSorted.Force() |> List.contains [ "System" ] then
                    "raise (NotImplementedException())"
                else
                    "raise (System.NotImplementedException())"
            else
                implementBody

        let newlineIndent =
            Environment.NewLine + String.make (spacesBeforeOverrideKeyword + 4) ' '

        let getOverridableMethods superTy (overriddenMethods: MethInfo list) overriddenProperties =
            // Do not check a method with same name twice
            //type AA() =
            //    abstract a: unit -> unit
            //    default _.a() = printfn "A"
            //type BB() =
            //    inherit AA()
            //    member _.a() = printfn "B" (* This method covered the `AA.a` *)
            //type CC() =
            //    inherit BB()
            //    override | (* Here should not suggest to override `AA.a` *)
            let checkedMethods = ResizeArray(overriddenMethods)

            let isInterface = isInterfaceTy g superTy

            // reuse between props and methods
            let argNames = HashSet()

            let overridableProps =
                let generatePropertyOverrideBody (prop: PropInfo) getterMeth setterMeth =
                    argNames.Clear()

                    let parameters =
                        prop.GetParamNamesAndTypes(amap, m)
                        |> List.map (fun (ParamNameAndType(name, ty)) ->
                            let name =
                                name
                                |> Option.map _.idText
                                |> Option.defaultValue String.Empty
                                |> checkOrGenerateArgName argNames

                            $"{name}: {stringOfTy denv ty}")
                        |> String.concat ", "

                    let retTy = prop.GetPropertyType(amap, m)
                    let retTy = stringOfTy denv retTy

                    let getter, getterWithBody =
                        match getterMeth with
                        | ValueSome meth ->
                            let implementBody =
                                checkMethAbstractAndGetImplementBody
                                    meth
                                    ($"base.{prop.DisplayName}" + (if prop.IsIndexer then $"({parameters})" else ""))

                            let getter = $"get ({parameters}): {retTy}"
                            getter, $"{getter} = {implementBody}"
                        | _ -> String.Empty, String.Empty

                    let setter, setterWithBody =
                        match setterMeth with
                        | ValueSome meth ->
                            let argValue = checkOrGenerateArgName argNames "value"

                            let implementBody =
                                checkMethAbstractAndGetImplementBody
                                    meth
                                    ($"base.{prop.DisplayName}"
                                     + (if prop.IsIndexer then $"({parameters})" else String.Empty)
                                     + $" <- {argValue}")

                            let parameters = if prop.IsIndexer then $"({parameters}) " else String.Empty
                            let setter = $"set {parameters}({argValue}: {retTy})"
                            setter, $"{setter} = {implementBody}"
                        | _ -> String.Empty, String.Empty

                    let keywordAnd =
                        if getterMeth.IsNone || setterMeth.IsNone then
                            String.Empty
                        else
                            " and "

                    let this = if hasThis || prop.IsStatic then String.Empty else "this."

                    let getterWithBody =
                        if String.IsNullOrWhiteSpace getterWithBody then
                            String.Empty
                        else
                            getterWithBody + newlineIndent

                    let name = $"{prop.DisplayName} with {getter}{keywordAnd}{setter}"

                    let textInCode =
                        this
                        + prop.DisplayName
                        + newlineIndent
                        + "with "
                        + getterWithBody
                        + keywordAnd
                        + setterWithBody

                    name, textInCode

                GetIntrinsicPropInfoWithOverriddenPropOfType
                    infoReader
                    None
                    ad
                    TypeHierarchy.AllowMultiIntfInstantiations.No
                    FindMemberFlag.PreferOverrides
                    range0
                    superTy
                |> List.choose (fun struct (prop, baseProp) ->
                    let getterMeth =
                        if prop.HasGetter then
                            ValueSome prop.GetterMethod
                        else
                            baseProp |> ValueOption.map _.GetterMethod

                    let setterMeth =
                        if prop.HasSetter then
                            ValueSome prop.SetterMethod
                        else
                            baseProp |> ValueOption.map _.SetterMethod

                    let isGetterOverridable, isSetterOverridable =
                        isPropertyOverridable superTy overriddenProperties prop

                    let isGetterOverridable =
                        isGetterOverridable
                        && isMethodOptionOverridable superTy checkedMethods getterMeth

                    let isSetterOverridable =
                        isSetterOverridable
                        && isMethodOptionOverridable superTy checkedMethods setterMeth

                    let canPick =
                        prop.IsStatic = isStatic && (isGetterOverridable || isSetterOverridable)

                    getterMeth |> ValueOption.iter checkedMethods.Add
                    setterMeth |> ValueOption.iter checkedMethods.Add

                    if not canPick then
                        None
                    else
                        let getterMeth = if isGetterOverridable then getterMeth else ValueNone
                        let setterMeth = if isSetterOverridable then setterMeth else ValueNone
                        let name, textInCode = generatePropertyOverrideBody prop getterMeth setterMeth

                        Item.Property(
                            name,
                            [
                                prop
                                if baseProp.IsSome then
                                    baseProp.Value
                            ],
                            None
                        )
                        |> ItemWithNoInst
                        |> CompletionItemWithMoreSetting ValueNone ValueNone -1 (ValueSome textInCode) (ValueSome name)
                        |> Some)

            let overridableMeths =
                let generateMethodOverrideBody (meth: MethInfo) =
                    argNames.Clear()

                    let parameters =
                        meth.GetParamNames()
                        |> List.zip (meth.GetParamTypes(amap, m, meth.FormalMethodInst))
                        |> List.map (fun (types, names) ->
                            let names =
                                names
                                |> List.zip types
                                |> List.map (fun (ty, name) ->
                                    let name =
                                        name |> Option.defaultValue String.Empty |> checkOrGenerateArgName argNames

                                    $"{name}: {stringOfTy denv ty}")
                                |> String.concat ", "

                            $"({names})")
                        |> String.concat " "

                    let retTy = meth.GetFSharpReturnType(amap, m, meth.FormalMethodInst)

                    let name = $"{meth.DisplayName} {parameters}: {stringOfTy denv retTy}"

                    let textInCode =
                        let nameWithThis =
                            if hasThis || not meth.IsInstance then
                                $"{name} = "
                            else
                                $"this.{name} = "

                        let implementBody =
                            checkMethAbstractAndGetImplementBody meth $"base.{meth.DisplayName}{parameters}"

                        nameWithThis + newlineIndent + implementBody

                    name, textInCode

                GetIntrinsicMethInfosOfType
                    infoReader
                    None
                    ad
                    TypeHierarchy.AllowMultiIntfInstantiations.No
                    FindMemberFlag.PreferOverrides
                    range0
                    superTy
                |> List.choose (fun meth ->
                    let canPick =
                        meth.IsInstance <> isStatic
                        && isMethodOverridable superTy checkedMethods meth
                        && (not isInterface
                            || not (tyconRefEq g meth.DeclaringTyconRef g.system_Object_tcref))

                    checkedMethods.Add meth

                    if not canPick then
                        None
                    else
                        let name, textInCode = generateMethodOverrideBody meth

                        Item.MethodGroup(name, [ meth ], None)
                        |> ItemWithNoInst
                        |> CompletionItemWithMoreSetting ValueNone ValueNone -1 (ValueSome textInCode) (ValueSome name)
                        |> Some)

            overridableProps @ overridableMeths

        let getTyFromTypeNamePos (endPos: pos) =
            let nameResItems =
                GetPreciseItemsFromNameResolution(endPos.Line, endPos.Column, None, ResolveTypeNamesToTypeRefs, ResolveOverloads.Yes)

            match nameResItems with
            | NameResResult.Members(ls, _, _) ->
                ls
                |> List.tryPick (function
                    | { Item = Item.Types(_, ty :: _) } -> Some ty
                    | _ -> None)
            | _ -> None

        let ctx =
            match ctx with
            | MethodOverrideCompletionContext.Class ->
                sResolutions.CapturedNameResolutions
                |> ResizeArray.tryPick (fun r ->
                    match r.Item with
                    | Item.Types(_, ty :: _) when equals r.Range typeNameRange && isAppTy g ty ->
                        let superTy =
                            (tcrefOfAppTy g ty).TypeContents.tcaug_super
                            |> Option.defaultValue g.obj_ty_noNulls

                        Some(ty, superTy)
                    | _ -> None)

            | MethodOverrideCompletionContext.Interface mTy ->
                sResolutions.CapturedNameResolutions
                |> ResizeArray.tryPick (fun r ->
                    match r.Item with
                    | Item.Types(_, ty :: _) when equals r.Range typeNameRange && isAppTy g ty ->
                        let superTy = getTyFromTypeNamePos mTy.End |> Option.defaultValue g.obj_ty_noNulls
                        Some(ty, superTy)
                    | _ -> None)
            | MethodOverrideCompletionContext.ObjExpr m ->
                let _, quals = GetExprTypingForPosition(m.End)

                quals
                |> Array.tryFind (fun (_, _, _, r) -> posEq m.Start r.Start)
                |> Option.map (fun (ty, _, _, _) -> ty, getTyFromTypeNamePos typeNameRange.End |> Option.defaultValue g.obj_ty_noNulls)

        match ctx with
        | Some(ty, superTy) ->
            let overriddenMethods =
                GetImmediateIntrinsicMethInfosWithExplicitImplOfType (None, ad) g amap typeNameRange ty
                |> List.filter (fun x -> x.IsDefiniteFSharpOverride)

            let overriddenProperties =
                GetImmediateIntrinsicPropInfosWithExplicitImplOfType (None, ad) g amap typeNameRange ty
                |> List.filter (fun x -> x.IsDefiniteFSharpOverride)

            let overridableMethods =
                getOverridableMethods superTy overriddenMethods overriddenProperties

            Some(overridableMethods, denv, m)
        | _ -> None

    /// Gets all field identifiers of a union case that can be referred to in a pattern.
    let GetUnionCaseFields caseIdRange alreadyReferencedFields =
        sResolutions.CapturedNameResolutions
        |> ResizeArray.tryPick (fun r ->
            match r.Item with
            | Item.UnionCase(uci, _) when equals r.Range caseIdRange ->
                uci.UnionCase.RecdFields
                |> List.indexed
                |> List.choose (fun (index, field) ->
                    if List.contains field.LogicalName alreadyReferencedFields then
                        None
                    else
                        Item.UnionCaseField(uci, index)
                        |> ItemWithNoInst
                        |> DefaultCompletionItem
                        |> Some)
                |> Some
            | _ -> None)

    let GetCompletionsForUnionCaseField pos indexOrName caseIdRange isTheOnlyField declaredItems =
        let declaredItems =
            declaredItems
            |> Option.bind (FilterRelevantItemsBy getItem2 None IsPatternCandidate)

        // When the user types `fun (Case (x| )) ->`, we do not yet know whether the intention is to use positional or named arguments,
        // so let's show options for both.
        let fields indexOrName isTheOnlyField (uci: UnionCaseInfo) =
            match indexOrName, isTheOnlyField with
            | Choice1Of2(Some 0), true ->
                uci.UnionCase.RecdFields
                |> List.mapi (fun index _ -> Item.UnionCaseField(uci, index) |> ItemWithNoInst |> DefaultCompletionItem)
            | _ -> []

        sResolutions.CapturedNameResolutions
        |> ResizeArray.tryPick (fun r ->
            match r.Item with
            | Item.UnionCase(uci, _) when equals r.Range caseIdRange ->
                let list =
                    declaredItems
                    |> Option.map p13
                    |> Option.defaultValue []
                    |> List.append (fields indexOrName isTheOnlyField uci)

                Some(SuggestNameForUnionCaseFieldPattern g caseIdRange.End pos uci indexOrName isTheOnlyField list, r.DisplayEnv, r.Range)
            | _ -> None)
        |> Option.orElse declaredItems

    let GetCompletionsForRecordField pos referencedFields declaredItems =
        declaredItems
        |> Option.map (fun (items: CompletionItem list, denv, range) ->
            let fields =
                // Try to find a name resolution for any of the referenced fields, and through it access all available fields of the record
                referencedFields
                |> List.tryPick (fun (_, fieldRange) ->
                    sResolutions.CapturedNameResolutions
                    |> ResizeArray.tryPick (fun cnr ->
                        match cnr.Item with
                        | Item.RecdField info when equals cnr.Range fieldRange ->
                            info.TyconRef.AllFieldAsRefList
                            |> List.choose (fun field ->
                                if
                                    referencedFields
                                    |> List.exists (fun (fieldName, _) -> fieldName = field.DisplayName)
                                then
                                    None
                                else
                                    FreshenRecdFieldRef ncenv field.Range field |> Item.RecdField |> Some)
                            |> Some
                        | _ -> None))
                |> Option.defaultWith (fun () ->
                    // Fall back to showing all record field names in scope
                    let (nenv, _), _ = GetBestEnvForPos pos
                    getRecordFieldsInScope nenv)
                |> List.map (ItemWithNoInst >> DefaultCompletionItem)

            let items =
                items
                |> List.filter (fun item ->
                    match item.Item with
                    | Item.ModuleOrNamespaces _ -> true
                    | Item.Types(_, ty :: _) -> isRecdTy g ty
                    | _ -> false)
                |> List.append fields

            items, denv, range)

    let GetDeclaredItems
        (
            parseResultsOpt: FSharpParseFileResults option,
            lineStr: string,
            origLongIdentOpt,
            colAtEndOfNamesAndResidue,
            residueOpt,
            lastDotPos,
            line,
            loc,
            filterCtors,
            resolveOverloads,
            isInRangeOperator,
            allSymbols: unit -> AssemblySymbol list
        ) =

        // Are the last two chars (except whitespaces) = ".."
        let isLikeRangeOp =
            match FindFirstNonWhitespacePosition lineStr (colAtEndOfNamesAndResidue - 1) with
            | Some x when x >= 1 && lineStr[x] = '.' && lineStr[x - 1] = '.' -> true
            | _ -> false

        // if last two chars are .. and we are not in range operator context - no completion
        if isLikeRangeOp && not isInRangeOperator then
            None
        else

            // Try to use the exact results of name resolution during type checking to generate the results
            // This is based on position (i.e. colAtEndOfNamesAndResidue). This is not used if a residueOpt is given.
            let nameResItems =
                match residueOpt with
                | None -> GetPreciseItemsFromNameResolution(line, colAtEndOfNamesAndResidue, None, filterCtors, resolveOverloads)
                | Some residue ->
                    // Deals with cases when we have spaces between dot and\or identifier, like A  . $
                    // if this is our case - then we need to locate end position of the name skipping whitespaces
                    // this allows us to handle cases like: let x . $ = 1
                    let lastPos =
                        lastDotPos
                        |> Option.orElseWith (fun _ -> FindFirstNonWhitespacePosition lineStr (colAtEndOfNamesAndResidue - 1))

                    match lastPos with
                    | Some p when lineStr[p] = '.' ->
                        match FindFirstNonWhitespacePosition lineStr (p - 1) with
                        | Some colAtEndOfNames ->
                            let colAtEndOfNames = colAtEndOfNames + 1 // convert 0-based to 1-based
                            GetPreciseItemsFromNameResolution(line, colAtEndOfNames, Some(residue), filterCtors, resolveOverloads)
                        | None -> NameResResult.Empty
                    | _ -> NameResResult.Empty

            // Normalize to form A.B.C.D where D is the residue. It may be empty for "A.B.C."
            // residueOpt = Some when we are looking for the exact match
            let plid, exactMatchResidueOpt =
                match origLongIdentOpt, residueOpt with
                | None, _ -> [], None
                | Some(origLongIdent), Some _ -> origLongIdent, None
                | Some(origLongIdent), None ->
                    Debug.Assert(not (isNil origLongIdent), "origLongIdent is empty")
                    // note: as above, this happens when we are called for "precise" resolution - (F1 keyword, data tip etc..)
                    let plid, residue = List.frontAndBack origLongIdent
                    plid, Some residue

            let pos = mkPos line loc
            let (nenv, ad), m = GetBestEnvForPos pos

            let getType () =
                match TryToResolveLongIdentAsType ncenv nenv m plid with
                | Some x -> tryTcrefOfAppTy g x
                | None ->
                    match
                        lastDotPos
                        |> Option.orElseWith (fun _ -> FindFirstNonWhitespacePosition lineStr (colAtEndOfNamesAndResidue - 1))
                    with
                    | Some p when lineStr[p] = '.' ->
                        match FindFirstNonWhitespacePosition lineStr (p - 1) with
                        | Some colAtEndOfNames ->
                            let colAtEndOfNames = colAtEndOfNames + 1 // convert 0-based to 1-based

                            match TryGetTypeFromNameResolution(line, colAtEndOfNames, residueOpt, resolveOverloads) with
                            | Some x -> tryTcrefOfAppTy g x
                            | _ -> ValueNone
                        | None -> ValueNone
                    | _ -> ValueNone

            match nameResItems with
            | NameResResult.Cancel(denv, m) -> Some([], denv, m)
            | NameResResult.Members(FilterRelevantItems getItem exactMatchResidueOpt (items, denv, m)) ->
                // lookup based on name resolution results successful
                Some(items |> List.map (CompletionItem (getType ()) ValueNone), denv, m)
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
                            ExprTypingsResult.None, false
                        | Some parseResults ->

                            let leftOfDot =
                                ParsedInput.TryFindExpressionASTLeftOfDotLeftOfCursor(
                                    mkPos line colAtEndOfNamesAndResidue,
                                    parseResults.ParseTree
                                )

                            match leftOfDot with
                            | Some(pos, _) -> GetPreciseCompletionListFromExprTypings(parseResults, pos, filterCtors), true
                            | None ->
                                // Can get here in a case like: if "f xxx yyy" is legal, and we do "f xxx y"
                                // We have no interest in expression typings, those are only useful for dot-completion.  We want to fallback
                                // to "Use an environment lookup as the last resort" below
                                ExprTypingsResult.None, false

                    match qualItems, thereIsADotInvolved with
                    | ExprTypingsResult.Some(FilterRelevantItems getItem exactMatchResidueOpt (items, denv, m), ty), _ when
                        // Initially we only use the expression typings when looking up, e.g. (expr).Nam or (expr).Name1.Nam
                        // These come through as an empty plid and residue "". Otherwise we try an environment lookup
                        // and then return to the qualItems. This is because the expression typings are a little inaccurate, primarily because
                        // it appears we're getting some typings recorded for non-atomic expressions like "f x"
                        isNil plid
                        ->
                        // lookup based on expression typings successful
                        Some(items |> List.map (CompletionItem (tryTcrefOfAppTy g ty) ValueNone), denv, m)
                    | ExprTypingsResult.NoneBecauseThereWereTypeErrors, _ ->
                        // There was an error, e.g. we have "<expr>." and there is an error determining the type of <expr>
                        // In this case, we don't want any of the fallback logic, rather, we want to produce zero results.
                        None
                    | ExprTypingsResult.NoneBecauseTypecheckIsStaleAndTextChanged, _ ->
                        // we want to report no result and let second-chance intellisense kick in
                        None
                    | _, true when isNil plid ->
                        // If the user just pressed '.' after an _expression_ (not a plid), it is never right to show environment-lookup top-level completions.
                        // The user might by typing quickly, and the LS didn't have an expression type right before the dot yet.
                        // Second-chance intellisense will bring up the correct list in a moment.
                        None
                    | _ ->
                        // Use an environment lookup as the last resort
                        let envItems, denv, m =
                            GetEnvironmentLookupResolutions(nenv, ad, m, plid, filterCtors, residueOpt.IsSome)

                        let envResult =
                            match nameResItems, (envItems, denv, m), qualItems with

                            // First, use unfiltered name resolution items, if they're not empty
                            | NameResResult.Members(items, denv, m), _, _ when not (isNil items) ->
                                // lookup based on name resolution results successful
                                ValueSome(items |> List.map (CompletionItem (getType ()) ValueNone), denv, m)

                            // If we have nonempty items from environment that were resolved from a type, then use them...
                            // (that's better than the next case - here we'd return 'int' as a type)
                            | _, FilterRelevantItems getItem exactMatchResidueOpt (items, denv, m), _ when not (isNil items) ->
                                // lookup based on name and environment successful
                                ValueSome(items |> List.map (CompletionItem (getType ()) ValueNone), denv, m)

                            // Try again with the qualItems
                            | _, _, ExprTypingsResult.Some(FilterRelevantItems getItem exactMatchResidueOpt (items, denv, m), ty) ->
                                ValueSome(items |> List.map (CompletionItem (tryTcrefOfAppTy g ty) ValueNone), denv, m)

                            | _ -> ValueNone

                        let globalResult =
                            match origLongIdentOpt with
                            | None
                            | Some [] ->
                                let globalItems =
                                    allSymbols ()
                                    |> List.filter (fun x ->
                                        not x.Symbol.IsExplicitlySuppressed
                                        &&

                                        match x.Symbol with
                                        | :? FSharpMemberOrFunctionOrValue as m when
                                            m.IsConstructor && filterCtors = ResolveTypeNamesToTypeRefs
                                            ->
                                            false
                                        | _ -> true)

                                let getItem (x: AssemblySymbol) = x.Symbol.Item

                                match globalItems, denv, m with
                                | FilterRelevantItems getItem exactMatchResidueOpt (globalItemsFiltered, denv, m) when
                                    not (isNil globalItemsFiltered)
                                    ->
                                    globalItemsFiltered
                                    |> List.map (fun globalItem ->
                                        CompletionItem (getType ()) (ValueSome globalItem) (ItemWithNoInst globalItem.Symbol.Item))
                                    |> fun r -> ValueSome(r, denv, m)
                                | _ -> ValueNone
                            | _ -> ValueNone // do not return unresolved items after dot

                        match envResult, globalResult with
                        | ValueSome(items, denv, m), ValueSome(gItems, _, _) -> Some(items @ gItems, denv, m)
                        | ValueSome x, ValueNone -> Some x
                        | ValueNone, ValueSome y -> Some y
                        | ValueNone, ValueNone -> None

    let toCompletionItems (items: ItemWithInst list, denv: DisplayEnv, m: range) =
        items |> List.map DefaultCompletionItem, denv, m

    /// Find record fields in the best naming environment.
    let GetEnvironmentLookupResolutionsIncludingRecordFieldsAtPosition cursorPos plid envItems =
        // An empty record expression may be completed into something like these:
        // { XXX = ... }
        // { xxx with XXX ... }
        // Provide both expression items in scope and available record fields.
        let (nenv, _), m = GetBestEnvForPos cursorPos

        let fieldItems, _, _ =
            GetClassOrRecordFieldsEnvironmentLookupResolutions(cursorPos, plid, true)

        let fieldCompletionItems, _, _ as fieldsResult =
            (fieldItems, nenv.DisplayEnv, m) |> toCompletionItems

        match envItems with
        | Some(items, denv, m) -> Some(fieldCompletionItems @ items, denv, m)
        | _ -> Some(fieldsResult)

    /// Get the auto-complete items at a particular location.
    let GetDeclItemsForNamesAtPosition
        (
            parseResultsOpt: FSharpParseFileResults option,
            origLongIdentOpt: string list option,
            residueOpt: string option,
            lastDotPos: int option,
            line: int,
            lineStr: string,
            colAtEndOfNamesAndResidue,
            filterCtors,
            resolveOverloads,
            completionContextAtPos: (pos * CompletionContext option) option,
            getAllSymbols: unit -> AssemblySymbol list
        ) : (CompletionItem list * DisplayEnv * CompletionContext option * range) option =

        let loc =
            match colAtEndOfNamesAndResidue with
            | pastEndOfLine when pastEndOfLine >= lineStr.Length -> lineStr.Length
            | atDot when lineStr[atDot] = '.' -> atDot + 1
            | atStart when atStart = 0 -> 0
            | otherwise -> otherwise - 1

        let getDeclaredItemsNotInRangeOpWithAllSymbols () =
            GetDeclaredItems(
                parseResultsOpt,
                lineStr,
                origLongIdentOpt,
                colAtEndOfNamesAndResidue,
                residueOpt,
                lastDotPos,
                line,
                loc,
                filterCtors,
                resolveOverloads,
                false,
                getAllSymbols
            )

        let pos = mkPos line colAtEndOfNamesAndResidue

        // Look for a "special" completion context
        let completionContext =
            // If the completion context we have computed higher up the stack is for the same position,
            // reuse it, otherwise recompute
            match completionContextAtPos with
            | Some(contextForPos, context) when contextForPos = pos -> context
            | _ ->
                parseResultsOpt
                |> Option.map (fun x -> x.ParseTree)
                |> Option.bind (fun parseTree -> ParsedInput.TryGetCompletionContext(pos, parseTree, lineStr))

        let res =
            match completionContext with
            // Invalid completion locations
            | Some CompletionContext.Invalid -> None

            // Completion at 'inherit C(...)"
            | Some(CompletionContext.Inherit(InheritanceContext.Class, (plid, _))) ->
                GetEnvironmentLookupResolutionsAtPosition(mkPos line loc, plid, filterCtors, false)
                |> FilterRelevantItemsBy getItem None (getItem >> IsInheritsCompletionCandidate)
                |> Option.map toCompletionItems

            // Completion at 'interface ..."
            | Some(CompletionContext.Inherit(InheritanceContext.Interface, (plid, _))) ->
                GetEnvironmentLookupResolutionsAtPosition(mkPos line loc, plid, filterCtors, false)
                |> FilterRelevantItemsBy getItem None (getItem >> IsInterfaceCompletionCandidate)
                |> Option.map toCompletionItems

            // Completion at 'implement ..."
            | Some(CompletionContext.Inherit(InheritanceContext.Unknown, (plid, _))) ->
                GetEnvironmentLookupResolutionsAtPosition(mkPos line loc, plid, filterCtors, false)
                |> FilterRelevantItemsBy
                    getItem
                    None
                    (getItem
                     >> (fun t -> IsInheritsCompletionCandidate t || IsInterfaceCompletionCandidate t))
                |> Option.map toCompletionItems

            // Completion at ' { XXX = ... } "
            | Some(CompletionContext.RecordField(RecordContext.New((plid, _), isFirstField))) ->
                if isFirstField then
                    let cursorPos = mkPos line loc

                    let envItems =
                        GetDeclaredItems(
                            parseResultsOpt,
                            lineStr,
                            origLongIdentOpt,
                            colAtEndOfNamesAndResidue,
                            residueOpt,
                            lastDotPos,
                            line,
                            loc,
                            filterCtors,
                            resolveOverloads,
                            false,
                            fun () -> []
                        )

                    GetEnvironmentLookupResolutionsIncludingRecordFieldsAtPosition cursorPos plid envItems
                else
                    // { x. } can be either record construction or computation expression. Try to get all visible record fields first
                    match
                        GetClassOrRecordFieldsEnvironmentLookupResolutions(mkPos line loc, plid, false)
                        |> toCompletionItems
                    with
                    | [], _, _ ->
                        // no record fields found, return completion list as if we were outside any computation expression
                        GetDeclaredItems(
                            parseResultsOpt,
                            lineStr,
                            origLongIdentOpt,
                            colAtEndOfNamesAndResidue,
                            residueOpt,
                            lastDotPos,
                            line,
                            loc,
                            filterCtors,
                            resolveOverloads,
                            false,
                            fun () -> []
                        )
                    | result -> Some(result)

            // Completion at '{ ... }'
            | Some(CompletionContext.RecordField RecordContext.Empty) ->
                let cursorPos = mkPos line loc

                let envItems =
                    GetDeclaredItems(
                        parseResultsOpt,
                        lineStr,
                        origLongIdentOpt,
                        colAtEndOfNamesAndResidue,
                        residueOpt,
                        lastDotPos,
                        line,
                        loc,
                        filterCtors,
                        resolveOverloads,
                        false,
                        fun () -> []
                    )

                GetEnvironmentLookupResolutionsIncludingRecordFieldsAtPosition cursorPos [] envItems

            // Completion at ' { XXX = ... with ... } "
            | Some(CompletionContext.RecordField(RecordContext.CopyOnUpdate(identRange, (plid, _)))) ->
                match GetRecdFieldsForCopyAndUpdateExpr(identRange, plid) with
                | None ->
                    Some(GetClassOrRecordFieldsEnvironmentLookupResolutions(mkPos line loc, plid, false))
                    |> Option.map toCompletionItems
                | Some(items, denv, m) -> Some(List.map ItemWithNoInst items, denv, m) |> Option.map toCompletionItems

            // Completion at ' { XXX = ... with ... } "
            | Some(CompletionContext.RecordField(RecordContext.Constructor(typeName))) ->
                GetClassOrRecordFieldsEnvironmentLookupResolutions(mkPos line loc, [ typeName ], false)
                |> toCompletionItems
                |> Some

            // No completion at '...: string'
            | Some(CompletionContext.RecordField(RecordContext.Declaration true)) -> None

            // Completion at ' SomeMethod( ... ) ' or ' [<SomeAttribute( ... )>] ' with named arguments
            | Some(CompletionContext.ParameterList(endPos, fields)) ->
                let results = GetNamedParametersAndSettableFields endPos

                let declaredItems = getDeclaredItemsNotInRangeOpWithAllSymbols ()

                match results with
                | NameResResult.Members(items, denv, m) ->
                    let filtered =
                        items
                        |> RemoveDuplicateItems g
                        |> RemoveExplicitlySuppressed g
                        |> List.filter (fun item -> not (fields.Contains item.Item.DisplayName))
                        |> List.map (fun item ->
                            {
                                ItemWithInst = item
                                Kind = CompletionItemKind.Argument
                                MinorPriority = 0
                                IsOwnMember = false
                                Type = None
                                Unresolved = None
                                CustomInsertText = ValueNone
                                CustomDisplayText = ValueNone
                            })

                    match declaredItems with
                    | None -> Some(toCompletionItems (items, denv, m))
                    | Some(declItems, declaredDisplayEnv, declaredRange) -> Some(filtered @ declItems, declaredDisplayEnv, declaredRange)
                | _ -> declaredItems

            | Some(CompletionContext.AttributeApplication) ->
                getDeclaredItemsNotInRangeOpWithAllSymbols ()
                |> Option.map (fun (items, denv, m) ->
                    items
                    |> List.filter (fun cItem ->
                        match cItem.Item with
                        | Item.ModuleOrNamespaces _ -> true
                        | _ when IsAttribute infoReader cItem.Item -> true
                        | _ -> false),
                    denv,
                    m)

            | Some(CompletionContext.OpenDeclaration isOpenType) ->
                getDeclaredItemsNotInRangeOpWithAllSymbols ()
                |> Option.map (fun (items, denv, m) ->
                    items
                    |> List.filter (fun x ->
                        match x.Item with
                        | Item.ModuleOrNamespaces _ -> true
                        | Item.Types _ when isOpenType -> true
                        | _ -> false),
                    denv,
                    m)

            // Completion at '(x: ...)"
            | Some CompletionContext.Type
            // Completion at  '| Case1 of ...'
            | Some CompletionContext.UnionCaseFieldsDeclaration
            // Completion at 'type Long = int6...' or 'type SomeUnion = Abc...'
            | Some CompletionContext.TypeAbbreviationOrSingleCaseUnion
            // Completion at 'Field1: ...'
            | Some(CompletionContext.RecordField(RecordContext.Declaration false)) ->
                getDeclaredItemsNotInRangeOpWithAllSymbols ()
                |> Option.bind (FilterRelevantItemsBy getItem2 None IsTypeCandidate)

            | Some(CompletionContext.Pattern patternContext) ->
                match patternContext with
                | PatternContext.UnionCaseFieldIdentifier(referencedFields, caseIdRange) ->
                    GetUnionCaseFields caseIdRange referencedFields
                    |> Option.map (fun completions ->
                        let (nenv, _ad), m = GetBestEnvForPos pos
                        completions, nenv.DisplayEnv, m)
                | PatternContext.PositionalUnionCaseField(fieldIndex, isTheOnlyField, caseIdRange) ->
                    getDeclaredItemsNotInRangeOpWithAllSymbols ()
                    |> GetCompletionsForUnionCaseField pos (Choice1Of2 fieldIndex) caseIdRange isTheOnlyField
                | PatternContext.NamedUnionCaseField(fieldName, caseIdRange) ->
                    getDeclaredItemsNotInRangeOpWithAllSymbols ()
                    |> GetCompletionsForUnionCaseField pos (Choice2Of2 fieldName) caseIdRange false
                | PatternContext.RecordFieldIdentifier referencedFields ->
                    getDeclaredItemsNotInRangeOpWithAllSymbols ()
                    |> GetCompletionsForRecordField pos referencedFields
                | PatternContext.Other ->
                    getDeclaredItemsNotInRangeOpWithAllSymbols ()
                    |> Option.bind (FilterRelevantItemsBy getItem2 None IsPatternCandidate)

            | Some(CompletionContext.MethodOverride(ctx, enclosingTypeNameRange, spacesBeforeOverrideKeyword, hasThis, isStatic)) ->
                GetOverridableMethods pos ctx enclosingTypeNameRange spacesBeforeOverrideKeyword hasThis isStatic

            // Other completions
            | cc ->
                match residueOpt |> Option.bind Seq.tryHead with
                | Some ''' ->
                    // The last token in
                    //    let x = 'E
                    // is Ident with text "'E", however it's either unfinished char literal or generic parameter.
                    // We should not provide any completion in the former case, and we don't provide it for the latter one for now
                    // because providing generic parameters list is context aware, which we don't have here (yet).
                    None
                | _ ->
                    let isInRangeOperator =
                        (match cc with
                         | Some CompletionContext.RangeOperator -> true
                         | _ -> false)

                    GetDeclaredItems(
                        parseResultsOpt,
                        lineStr,
                        origLongIdentOpt,
                        colAtEndOfNamesAndResidue,
                        residueOpt,
                        lastDotPos,
                        line,
                        loc,
                        filterCtors,
                        resolveOverloads,
                        isInRangeOperator,
                        getAllSymbols
                    )

        res |> Option.map (fun (items, denv, m) -> items, denv, completionContext, m)

    /// Return 'false' if this is not a completion item valid in an interface file.
    let IsValidSignatureFileItem item =
        match item with
        | Item.TypeVar _
        | Item.Types _
        | Item.ModuleOrNamespaces _ -> true
        | _ -> false

    /// Find the most precise display context for the given line and column.
    member _.GetBestDisplayEnvForPos cursorPos = GetBestEnvForPos cursorPos

    member _.GetVisibleNamespacesAndModulesAtPosition(cursorPos: pos) : ModuleOrNamespaceRef list =
        let (nenv, ad), m = GetBestEnvForPos cursorPos
        GetVisibleNamespacesAndModulesAtPoint ncenv nenv OpenQualified m ad

    /// Determines if a long ident is resolvable at a specific point.
    member _.IsRelativeNameResolvable(cursorPos: pos, plid: string list, item: Item) : bool =
        DiagnosticsScope.Protect
            range0
            (fun () ->
                /// Find items in the best naming environment.
                let (nenv, ad), m = GetBestEnvForPos cursorPos
                IsItemResolvable ncenv nenv m ad plid item)
            (fun msg ->
                Trace.TraceInformation(sprintf "FCS: recovering from error in IsRelativeNameResolvable: '%s'" msg)
                false)

    /// Determines if a long ident is resolvable at a specific point.
    member scope.IsRelativeNameResolvableFromSymbol(cursorPos: pos, plid: string list, symbol: FSharpSymbol) : bool =
        scope.IsRelativeNameResolvable(cursorPos, plid, symbol.Item)

    /// Get the auto-complete items at a location
    member _.GetDeclarations(parseResultsOpt, line, lineStr, partialName, completionContextAtPos, getAllEntities) =
        let isSigFile = SourceFileImpl.IsSignatureFile mainInputFileName

        DiagnosticsScope.Protect
            range0
            (fun () ->

                let declItemsOpt =
                    GetDeclItemsForNamesAtPosition(
                        parseResultsOpt,
                        Some partialName.QualifyingIdents,
                        Some partialName.PartialIdent,
                        partialName.LastDotPos,
                        line,
                        lineStr,
                        partialName.EndColumn + 1,
                        ResolveTypeNamesToCtors,
                        ResolveOverloads.Yes,
                        completionContextAtPos,
                        getAllEntities
                    )

                match declItemsOpt with
                | None -> DeclarationListInfo.Empty
                | Some(items, denv, ctx, m) ->
                    let items =
                        if isSigFile then
                            items |> List.filter (fun x -> IsValidSignatureFileItem x.Item)
                        else
                            items

                    let getAccessibility item =
                        FSharpSymbol.Create(cenv, item).Accessibility

                    let currentNamespaceOrModule =
                        parseResultsOpt
                        |> Option.map (fun x -> x.ParseTree)
                        |> Option.map (fun parsedInput ->
                            ParsedInput.GetFullNameOfSmallestModuleOrNamespaceAtPoint(mkPos line 0, parsedInput))

                    let isAttributeApplication =
                        match ctx with
                        | Some CompletionContext.AttributeApplication -> true
                        | _ -> false

                    DeclarationListInfo.Create(
                        infoReader,
                        tcAccessRights,
                        m,
                        denv,
                        getAccessibility,
                        items,
                        currentNamespaceOrModule,
                        isAttributeApplication
                    ))
            (fun msg ->
                Trace.TraceInformation(sprintf "FCS: recovering from error in GetDeclarations: '%s'" msg)
                DeclarationListInfo.Error msg)

    /// Get the symbols for auto-complete items at a location
    member _.GetDeclarationListSymbols(parseResultsOpt, line, lineStr, partialName, getAllEntities) =
        let isSigFile = SourceFileImpl.IsSignatureFile mainInputFileName

        DiagnosticsScope.Protect
            range0
            (fun () ->

                let declItemsOpt =
                    GetDeclItemsForNamesAtPosition(
                        parseResultsOpt,
                        Some partialName.QualifyingIdents,
                        Some partialName.PartialIdent,
                        partialName.LastDotPos,
                        line,
                        lineStr,
                        partialName.EndColumn + 1,
                        ResolveTypeNamesToCtors,
                        ResolveOverloads.Yes,
                        None,
                        getAllEntities
                    )

                match declItemsOpt with
                | None -> List.Empty
                | Some(items, denv, _, m) ->
                    let items =
                        if isSigFile then
                            items |> List.filter (fun x -> IsValidSignatureFileItem x.Item)
                        else
                            items

                    //do filtering like Declarationset
                    let items = items |> RemoveExplicitlySuppressedCompletionItems g

                    // Sort by name. For things with the same name,
                    //     - show types with fewer generic parameters first
                    //     - show types before over other related items - they usually have very useful XmlDocs
                    let items =
                        items
                        |> List.sortBy (fun d ->
                            let n =
                                match d.Item with
                                | Item.Types(_, AbbrevOrAppTy(tcref, _) :: _) -> 1 + tcref.TyparsNoRange.Length
                                // Put delegate ctors after types, sorted by #typars. RemoveDuplicateItems will remove FakeInterfaceCtor and DelegateCtor if an earlier type is also reported with this name
                                | Item.DelegateCtor(AbbrevOrAppTy(tcref, _)) -> 1000 + tcref.TyparsNoRange.Length
                                // Put type ctors after types, sorted by #typars. RemoveDuplicateItems will remove DefaultStructCtors if a type is also reported with this name
                                | Item.CtorGroup(_, cinfo :: _) -> 1000 + 10 * cinfo.DeclaringTyconRef.TyparsNoRange.Length
                                | _ -> 0

                            (d.Item.DisplayName, n))

                    // Remove all duplicates. We've put the types first, so this removes the DelegateCtor and DefaultStructCtor's.
                    let items = items |> RemoveDuplicateCompletionItems g

                    // Group by compiled name for types, display name for functions
                    // (We don't want types with the same display name to be grouped as overloads)
                    let items =
                        items
                        |> List.groupBy (fun d ->
                            match d.Item with
                            | Item.Types(_, AbbrevOrAppTy(tcref, _) :: _)
                            | Item.ExnCase tcref -> tcref.LogicalName
                            | Item.UnqualifiedType(tcref :: _)
                            | Item.DelegateCtor(AbbrevOrAppTy(tcref, _)) -> tcref.CompiledName
                            | Item.CtorGroup(_, cinfo :: _) -> cinfo.ApparentEnclosingTyconRef.CompiledName
                            | _ -> d.Item.DisplayName)

                    // Filter out operators (and list)
                    let items =
                        // Check whether this item looks like an operator.
                        let isOpItem (nm, item: CompletionItem list) =
                            match item |> List.map (fun x -> x.Item) with
                            | [ Item.Value _ ]
                            | [ Item.MethodGroup(_, [ _ ], _) ] -> IsOperatorDisplayName nm
                            | [ Item.UnionCase _ ] -> IsOperatorDisplayName nm
                            | _ -> false

                        let isFSharpList nm = (nm = "[]") // list shows up as a Type and a UnionCase, only such entity with a symbolic name, but want to filter out of intellisense

                        items
                        |> List.filter (fun (nm, items) -> not (isOpItem (nm, items)) && not (isFSharpList nm))

                    let items =
                        // Filter out duplicate names
                        items
                        |> List.map (fun (_nm, itemsWithSameName) ->
                            match itemsWithSameName with
                            | [] -> failwith "Unexpected empty bag"
                            | items ->
                                items
                                |> List.map (fun item ->
                                    let symbol = FSharpSymbol.Create(cenv, item.Item)
                                    FSharpSymbolUse(denv, symbol, item.ItemWithInst.TyparInstantiation, ItemOccurence.Use, m)))

                    //end filtering
                    items)
            (fun msg ->
                Trace.TraceInformation(sprintf "FCS: recovering from error in GetDeclarationListSymbols: '%s'" msg)
                [])

    /// Get the "reference resolution" tooltip for at a location
    member _.GetReferenceResolutionStructuredToolTipText(line, col, width) =

        let pos = mkPos line col

        let isPosMatch (pos, ar: AssemblyReference) : bool =
            let isRangeMatch = (rangeContainsPos ar.Range pos)

            let isNotSpecialRange =
                not (equals ar.Range rangeStartup)
                && not (equals ar.Range range0)
                && not (equals ar.Range rangeCmdArgs)

            let isMatch = isRangeMatch && isNotSpecialRange
            isMatch

        let dataTipOfReferences () =
            let matches =
                match loadClosure with
                | None -> []
                | Some(loadClosure) ->
                    loadClosure.References
                    |> List.collect snd
                    |> List.filter (fun ar -> isPosMatch (pos, ar.originalReference))

            match matches with
            | resolved :: _ // Take the first seen
            | [ resolved ] ->
                let tip =
                    wordL (TaggedText.tagStringLiteral ((resolved.prepareToolTip ()).TrimEnd([| '\n' |])))

                let tip = PrintUtilities.squashToWidth width tip

                let tip = LayoutRender.toArray tip
                ToolTipText.ToolTipText [ ToolTipElement.Single(tip, FSharpXmlDoc.None) ]

            | [] ->
                let matches =
                    match loadClosure with
                    | None -> None
                    | Some(loadClosure) ->
                        loadClosure.PackageReferences
                        |> Array.tryFind (fun (m, _) -> rangeContainsPos m pos)

                match matches with
                | None -> emptyToolTip
                | Some(_, lines) ->
                    let lines =
                        lines
                        |> List.filter (fun line -> not (line.StartsWithOrdinal("//")) && not (String.IsNullOrEmpty line))

                    ToolTipText.ToolTipText
                        [
                            for line in lines ->
                                let tip = wordL (TaggedText.tagStringLiteral line)
                                let tip = PrintUtilities.squashToWidth width tip
                                let tip = LayoutRender.toArray tip
                                ToolTipElement.Single(tip, FSharpXmlDoc.None)
                        ]

        DiagnosticsScope.Protect range0 dataTipOfReferences (fun err ->
            Trace.TraceInformation(sprintf "FCS: recovering from error in GetReferenceResolutionStructuredToolTipText: '%s'" err)
            ToolTipText [ ToolTipElement.CompositionError err ])

    member _.GetDescription(symbol: FSharpSymbol, inst: (FSharpGenericParameter * FSharpType) list, displayFullName, m: range) =
        let (nenv, accessorDomain), _ = GetBestEnvForPos m.Start
        let denv = nenv.DisplayEnv

        let item = symbol.Item
        let inst = inst |> List.map (fun (typar, t) -> typar.TypeParameter, t.Type)

        let itemWithInst =
            {
                ItemWithInst.Item = item
                ItemWithInst.TyparInstantiation = inst
            }

        let toolTipElement =
            FormatStructuredDescriptionOfItem displayFullName infoReader accessorDomain m denv itemWithInst (Some symbol) None

        ToolTipText [ toolTipElement ]

    // GetToolTipText: return the "pop up" (or "Quick Info") text given a certain context.
    member _.GetStructuredToolTipText(line, lineStr, colAtEndOfNames, names, width) =
        let Compute () =
            DiagnosticsScope.Protect
                range0
                (fun () ->
                    let declItemsOpt =
                        GetDeclItemsForNamesAtPosition(
                            None,
                            Some names,
                            None,
                            None,
                            line,
                            lineStr,
                            colAtEndOfNames,
                            ResolveTypeNamesToCtors,
                            ResolveOverloads.Yes,
                            None,
                            (fun () -> [])
                        )

                    match declItemsOpt with
                    | None -> emptyToolTip
                    | Some(items, denv, _, m) ->
                        match items with
                        | [ { Kind = CompletionItemKind.Property } as prop
                            { Kind = CompletionItemKind.Field }
                            { Kind = CompletionItemKind.Field } ] ->
                            let symbol = FSharpSymbol.Create(cenv, prop.Item)

                            let tt =
                                FormatStructuredDescriptionOfItem
                                    false
                                    infoReader
                                    tcAccessRights
                                    m
                                    denv
                                    prop.ItemWithInst
                                    (Some symbol)
                                    width

                            ToolTipText([ tt ])
                        | _ ->
                            ToolTipText(
                                items
                                |> List.map (fun x ->
                                    let symbol = Some(FSharpSymbol.Create(cenv, x.Item))
                                    FormatStructuredDescriptionOfItem false infoReader tcAccessRights m denv x.ItemWithInst symbol width)
                            ))
                (fun err ->
                    Trace.TraceInformation(sprintf "FCS: recovering from error in GetStructuredToolTipText: '%s'" err)
                    ToolTipText [ ToolTipElement.CompositionError err ])

        // See devdiv bug 646520 for rationale behind truncating and caching these quick infos (they can be big!)
        let key = line, colAtEndOfNames, lineStr, width

        match getToolTipTextCache.TryGet(AnyCallerThread, key) with
        | Some res -> res
        | None ->
            let res = Compute()
            getToolTipTextCache.Put(AnyCallerThread, key, res)
            res

    member _.GetF1Keyword(line, lineStr, colAtEndOfNames, names) : string option =
        DiagnosticsScope.Protect
            range0
            (fun () ->

                let declItemsOpt =
                    GetDeclItemsForNamesAtPosition(
                        None,
                        Some names,
                        None,
                        None,
                        line,
                        lineStr,
                        colAtEndOfNames,
                        ResolveTypeNamesToCtors,
                        ResolveOverloads.No,
                        None,
                        (fun () -> [])
                    )

                match declItemsOpt with
                | None -> None
                | Some(items: CompletionItem list, _, _, _) ->
                    match items with
                    | [] -> None
                    | [ item ] -> GetF1Keyword g item.Item
                    | _ ->
                        // For "new Type()" it seems from the code below that multiple items are returned.
                        // It combine the information from these items preferring a constructor if present.
                        let allTypes, constr, ty =
                            ((true, None, None), items)
                            ||> List.fold (fun (allTypes, constr, ty) (item: CompletionItem) ->
                                match item.Item, constr, ty with
                                | Item.Types _ as t, _, None -> allTypes, constr, Some t
                                | Item.Types _, _, _ -> allTypes, constr, ty
                                | Item.CtorGroup _, None, _ -> allTypes, Some item.Item, ty
                                | _ -> false, None, None)

                        match allTypes, constr, ty with
                        | true, Some item, _ -> GetF1Keyword g item
                        | true, _, Some item -> GetF1Keyword g item
                        | _ -> GetF1Keyword g items.Head.Item)
            (fun msg ->
                Trace.TraceInformation(sprintf "FCS: recovering from error in GetF1Keyword: '%s'" msg)
                None)

    member _.GetMethods(line, lineStr, colAtEndOfNames, namesOpt) =
        DiagnosticsScope.Protect
            range0
            (fun () ->

                let declItemsOpt =
                    GetDeclItemsForNamesAtPosition(
                        None,
                        namesOpt,
                        None,
                        None,
                        line,
                        lineStr,
                        colAtEndOfNames,
                        ResolveTypeNamesToCtors,
                        ResolveOverloads.No,
                        None,
                        (fun () -> [])
                    )

                match declItemsOpt with
                | None -> MethodGroup("", [||])
                | Some(items, denv, _, m) ->
                    // GetDeclItemsForNamesAtPosition returns Items.Types and Item.CtorGroup for `new T(|)`,
                    // the Item.Types is not needed here as it duplicates (at best) parameterless ctor.
                    let ctors =
                        items
                        |> List.filter (fun x ->
                            match x.Item with
                            | Item.CtorGroup _ -> true
                            | _ -> false)

                    let items =
                        match ctors with
                        | [] -> items
                        | ctors -> ctors

                    MethodGroup.Create(infoReader, tcAccessRights, m, denv, items |> List.map (fun x -> x.ItemWithInst)))
            (fun msg ->
                Trace.TraceInformation(sprintf "FCS: recovering from error in GetMethods: '%s'" msg)
                MethodGroup(msg, [||]))

    member _.GetMethodsAsSymbols(line, lineStr, colAtEndOfNames, names) =
        DiagnosticsScope.Protect
            range0
            (fun () ->
                let declItemsOpt =
                    GetDeclItemsForNamesAtPosition(
                        None,
                        Some names,
                        None,
                        None,
                        line,
                        lineStr,
                        colAtEndOfNames,
                        ResolveTypeNamesToCtors,
                        ResolveOverloads.No,
                        None,
                        (fun () -> [])
                    )

                match declItemsOpt with
                | None
                | Some([], _, _, _) -> None
                | Some(items, denv, _, m) ->
                    let allItems =
                        items
                        |> List.collect (fun item -> SelectMethodGroupItems2 g m item.ItemWithInst)

                    let symbols =
                        allItems |> List.map (fun item -> FSharpSymbol.Create(cenv, item.Item), item)

                    Some(symbols, denv, m))
            (fun msg ->
                Trace.TraceInformation(sprintf "FCS: recovering from error in GetMethodsAsSymbols: '%s'" msg)
                None)

    member _.GetDeclarationLocation(line, lineStr, colAtEndOfNames, names, preferFlag) =
        DiagnosticsScope.Protect
            range0
            (fun () ->

                let declItemsOpt =
                    GetDeclItemsForNamesAtPosition(
                        None,
                        Some names,
                        None,
                        None,
                        line,
                        lineStr,
                        colAtEndOfNames,
                        ResolveTypeNamesToCtors,
                        ResolveOverloads.Yes,
                        None,
                        (fun () -> [])
                    )

                match declItemsOpt with
                | None
                | Some([], _, _, _) -> FindDeclResult.DeclNotFound(FindDeclFailureReason.Unknown "")
                | Some(item :: _, _, _, _) ->
                    let getTypeVarNames (ilinfo: ILMethInfo) =
                        let classTypeParams =
                            ilinfo.DeclaringTyconRef.ILTyconRawMetadata.GenericParams
                            |> List.map (fun paramDef -> paramDef.Name)

                        let methodTypeParams = ilinfo.FormalMethodTypars |> List.map (fun ty -> ty.Name)
                        classTypeParams @ methodTypeParams |> Array.ofList

                    // Detect external references.  Currently this only labels references to .NET assemblies as external - F#
                    // references from nuget packages are not labelled as external.
                    let result =
                        match item.Item with
                        | Item.CtorGroup(_, ILMeth(_, ilinfo, _) :: _) ->
                            match ilinfo.MetadataScope with
                            | ILScopeRef.Assembly assemblyRef ->
                                let typeVarNames = getTypeVarNames ilinfo

                                FindDeclExternalParam.tryOfILTypes typeVarNames ilinfo.ILMethodRef.ArgTypes
                                |> Option.map (fun args ->
                                    let externalSym =
                                        FindDeclExternalSymbol.Constructor(ilinfo.ILMethodRef.DeclaringTypeRef.FullName, args)

                                    FindDeclResult.ExternalDecl(assemblyRef.Name, externalSym))
                            | _ -> None

                        | Item.MethodGroup(name, ILMeth(_, ilinfo, _) :: _, _) ->
                            match ilinfo.MetadataScope with
                            | ILScopeRef.Assembly assemblyRef ->
                                let typeVarNames = getTypeVarNames ilinfo

                                FindDeclExternalParam.tryOfILTypes typeVarNames ilinfo.ILMethodRef.ArgTypes
                                |> Option.map (fun args ->
                                    let externalSym =
                                        FindDeclExternalSymbol.Method(
                                            ilinfo.ILMethodRef.DeclaringTypeRef.FullName,
                                            name,
                                            args,
                                            ilinfo.ILMethodRef.GenericArity
                                        )

                                    FindDeclResult.ExternalDecl(assemblyRef.Name, externalSym))
                            | _ -> None

                        | Item.Property(name, ILProp propInfo :: _, _) ->
                            let methInfo =
                                if propInfo.HasGetter then Some propInfo.GetterMethod
                                elif propInfo.HasSetter then Some propInfo.SetterMethod
                                else None

                            match methInfo with
                            | Some methInfo ->
                                match methInfo.MetadataScope with
                                | ILScopeRef.Assembly assemblyRef ->
                                    let externalSym =
                                        FindDeclExternalSymbol.Property(methInfo.ILMethodRef.DeclaringTypeRef.FullName, name)

                                    Some(FindDeclResult.ExternalDecl(assemblyRef.Name, externalSym))
                                | _ -> None
                            | None -> None

                        | Item.ILField(ILFieldInfo(typeInfo, fieldDef)) when not typeInfo.TyconRefOfRawMetadata.IsLocalRef ->
                            match typeInfo.ILScopeRef with
                            | ILScopeRef.Assembly assemblyRef ->
                                let externalSym =
                                    FindDeclExternalSymbol.Field(typeInfo.ILTypeRef.FullName, fieldDef.Name)

                                Some(FindDeclResult.ExternalDecl(assemblyRef.Name, externalSym))
                            | _ -> None

                        | Item.Event(ILEvent(ILEventInfo(typeInfo, eventDef))) when not typeInfo.TyconRefOfRawMetadata.IsLocalRef ->
                            match typeInfo.ILScopeRef with
                            | ILScopeRef.Assembly assemblyRef ->
                                let externalSym =
                                    FindDeclExternalSymbol.Event(typeInfo.ILTypeRef.FullName, eventDef.Name)

                                Some(FindDeclResult.ExternalDecl(assemblyRef.Name, externalSym))
                            | _ -> None

                        | Item.Types(_, ty :: _) ->
                            match stripTyparEqns ty with
                            | TType_app(tr, _, _) ->
                                if tr.IsLocalRef then
                                    None
                                else
                                    match tr.TypeReprInfo, tr.PublicPath with
                                    | TILObjectRepr(TILObjectReprData(ILScopeRef.Assembly assemblyRef, _, _)), Some(PubPath parts) ->
                                        let fullName = parts |> String.concat "."
                                        Some(FindDeclResult.ExternalDecl(assemblyRef.Name, FindDeclExternalSymbol.Type fullName))
                                    | _ -> None
                            | _ -> None

                        | _ -> None

                    match result with
                    | Some x -> x
                    | None ->
                        match rangeOfItem g preferFlag item.Item with
                        | Some itemRange ->
                            let projectDir =
                                FileSystem.GetDirectoryNameShim(
                                    if String.IsNullOrEmpty(projectFileName) then
                                        mainInputFileName
                                    else
                                        projectFileName
                                )

                            let range = fileNameOfItem g (Some projectDir) itemRange item.Item
                            mkRange range itemRange.Start itemRange.End |> FindDeclResult.DeclFound
                        | None ->
                            match item.Item with
#if !NO_TYPEPROVIDERS
                            // provided items may have TypeProviderDefinitionLocationAttribute that binds them to some location
                            | Item.CtorGroup(name, ProvidedMeth _ :: _)
                            | Item.MethodGroup(name, ProvidedMeth _ :: _, _)
                            | Item.Property(name, ProvidedProp _ :: _, _) -> FindDeclFailureReason.ProvidedMember name
                            | Item.Event(ProvidedEvent _ as e) -> FindDeclFailureReason.ProvidedMember e.EventName
                            | Item.ILField(ProvidedField _ as f) -> FindDeclFailureReason.ProvidedMember f.FieldName
                            | ItemIsProvidedType g tcref -> FindDeclFailureReason.ProvidedType tcref.DisplayName
#endif
                            | _ -> FindDeclFailureReason.Unknown ""
                            |> FindDeclResult.DeclNotFound)
            (fun msg ->
                Trace.TraceInformation(sprintf "FCS: recovering from error in GetDeclarationLocation: '%s'" msg)
                FindDeclResult.DeclNotFound(FindDeclFailureReason.Unknown msg))

    member _.GetSymbolUseAtLocation(line, lineStr, colAtEndOfNames, names) =
        DiagnosticsScope.Protect
            range0
            (fun () ->
                let declItemsOpt =
                    GetDeclItemsForNamesAtPosition(
                        None,
                        Some names,
                        None,
                        None,
                        line,
                        lineStr,
                        colAtEndOfNames,
                        ResolveTypeNamesToCtors,
                        ResolveOverloads.Yes,
                        None,
                        (fun () -> [])
                    )

                match declItemsOpt with
                | None
                | Some([], _, _, _) -> None
                | Some(item :: _, denv, _, m) ->
                    let symbol = FSharpSymbol.Create(cenv, item.Item)
                    Some(symbol, item.ItemWithInst, denv, m))
            (fun msg ->
                Trace.TraceInformation(sprintf "FCS: recovering from error in GetSymbolUseAtLocation: '%s'" msg)
                None)

    member _.GetSymbolUsesAtLocation(line, lineStr, colAtEndOfNames, names) =
        DiagnosticsScope.Protect
            range0
            (fun () ->
                let declItemsOpt =
                    GetDeclItemsForNamesAtPosition(
                        None,
                        Some names,
                        None,
                        None,
                        line,
                        lineStr,
                        colAtEndOfNames,
                        ResolveTypeNamesToCtors,
                        ResolveOverloads.Yes,
                        None,
                        (fun () -> [])
                    )

                match declItemsOpt with
                | None -> List.empty
                | Some(items, denv, _, m) ->
                    items
                    |> List.map (fun item ->
                        let symbol = FSharpSymbol.Create(cenv, item.Item)
                        symbol, item.ItemWithInst, denv, m))
            (fun msg ->
                Trace.TraceInformation(sprintf "FCS: recovering from error in GetSymbolUsesAtLocation: '%s'" msg)
                List.empty)

    member _.PartialAssemblySignatureForFile =
        FSharpAssemblySignature(g, thisCcu, ccuSigForFile, tcImports, None, ccuSigForFile)

    member _.AccessRights = tcAccessRights

    member _.ProjectOptions = projectOptions

    member _.GetReferencedAssemblies() =
        [
            for x in tcImports.GetImportedAssemblies() do
                FSharpAssembly(g, tcImports, x.FSharpViewOfMetadata)
        ]

    member _.GetFormatSpecifierLocationsAndArity() =
        sSymbolUses.GetFormatSpecifierLocationsAndArity()

    member _.GetSemanticClassification(range: range option) : SemanticClassificationItem[] =
        sResolutions.GetSemanticClassification(g, amap, sSymbolUses.GetFormatSpecifierLocationsAndArity(), range)

    /// The resolutions in the file
    member _.ScopeResolutions = sResolutions

    /// The uses of symbols in the analyzed file
    member _.ScopeSymbolUses = sSymbolUses

    member _.TcGlobals = g

    member _.TcImports = tcImports

    /// The inferred signature of the file
    member _.CcuSigForFile = ccuSigForFile

    /// The assembly being analyzed
    member _.ThisCcu = thisCcu

    member _.ImplementationFile = implFileOpt

    /// All open declarations in the file, including auto open modules
    member _.OpenDeclarations = openDeclarations

    member _.SymbolEnv = cenv

    override _.ToString() =
        "TypeCheckInfo(" + mainInputFileName + ")"

type FSharpParsingOptions =
    {
        SourceFiles: string[]
        ApplyLineDirectives: bool
        ConditionalDefines: string list
        DiagnosticOptions: FSharpDiagnosticOptions
        LangVersionText: string
        IsInteractive: bool
        IndentationAwareSyntax: bool option
        StrictIndentation: bool option
        CompilingFSharpCore: bool
        IsExe: bool
    }

    member x.LastFileName =
        Debug.Assert(not (Array.isEmpty x.SourceFiles), "Parsing options don't contain any file")
        Array.last x.SourceFiles

    static member Default =
        {
            SourceFiles = Array.empty
            ApplyLineDirectives = false
            ConditionalDefines = []
            DiagnosticOptions = FSharpDiagnosticOptions.Default
            LangVersionText = LanguageVersion.Default.VersionText
            IsInteractive = false
            IndentationAwareSyntax = None
            StrictIndentation = None
            CompilingFSharpCore = false
            IsExe = false
        }

    static member FromTcConfig(tcConfig: TcConfig, sourceFiles, isInteractive: bool) =
        {
            SourceFiles = sourceFiles
            ApplyLineDirectives = tcConfig.applyLineDirectives
            ConditionalDefines = tcConfig.conditionalDefines
            DiagnosticOptions = tcConfig.diagnosticsOptions
            LangVersionText = tcConfig.langVersion.VersionText
            IsInteractive = isInteractive
            IndentationAwareSyntax = tcConfig.indentationAwareSyntax
            StrictIndentation = tcConfig.strictIndentation
            CompilingFSharpCore = tcConfig.compilingFSharpCore
            IsExe = tcConfig.target.IsExe
        }

    static member FromTcConfigBuilder(tcConfigB: TcConfigBuilder, sourceFiles, isInteractive: bool) =
        {
            SourceFiles = sourceFiles
            ApplyLineDirectives = tcConfigB.applyLineDirectives
            ConditionalDefines = tcConfigB.conditionalDefines
            DiagnosticOptions = tcConfigB.diagnosticsOptions
            LangVersionText = tcConfigB.langVersion.VersionText
            IsInteractive = isInteractive
            IndentationAwareSyntax = tcConfigB.indentationAwareSyntax
            StrictIndentation = tcConfigB.strictIndentation
            CompilingFSharpCore = tcConfigB.compilingFSharpCore
            IsExe = tcConfigB.target.IsExe
        }

module internal ParseAndCheckFile =

    /// Diagnostics handler for parsing & type checking while processing a single file
    type DiagnosticsHandler
        (
            reportErrors,
            mainInputFileName,
            diagnosticsOptions: FSharpDiagnosticOptions,
            sourceText: ISourceText,
            suggestNamesForErrors: bool,
            flatErrors: bool
        ) =
        let mutable options = diagnosticsOptions
        let diagnosticsCollector = ResizeArray<_>()
        let mutable errorCount = 0

        // We'll need number of lines for adjusting error messages at EOF
        let fileInfo = sourceText.GetLastCharacterPosition()

        let collectOne severity diagnostic =
            // 1. Extended diagnostic data should be created after typechecking because it requires a valid SymbolEnv
            // 2. Diagnostic message should be created during the diagnostic sink, because after typechecking
            //    the formatting of types in it may change (for example, 'a to obj)
            //
            // So we'll create a diagnostic later, but cache the FormatCore message now
            diagnostic.Exception.Data["CachedFormatCore"] <- diagnostic.FormatCore(flatErrors, suggestNamesForErrors)
            diagnosticsCollector.Add(struct (diagnostic, severity))

            if severity = FSharpDiagnosticSeverity.Error then
                errorCount <- errorCount + 1

        // This function gets called whenever an error happens during parsing or checking
        let diagnosticSink severity (diagnostic: PhasedDiagnostic) =
            // Sanity check here. The phase of an error should be in a phase known to the language service.
            let diagnostic =
                if not (diagnostic.IsPhaseInCompile()) then
                    // Reaching this point means that the error would be sticky if we let it prop up to the language service.
                    // Assert and recover by replacing phase with one known to the language service.
                    Trace.TraceInformation(
                        sprintf
                            "The subcategory '%s' seen in an error should not be seen by the language service"
                            (diagnostic.Subcategory())
                    )

                    { diagnostic with
                        Phase = BuildPhase.TypeCheck
                    }
                else
                    diagnostic

            if reportErrors then
                match diagnostic with
#if !NO_TYPEPROVIDERS
                | {
                      Exception = :? TypeProviderError as tpe
                  } -> tpe.Iter(fun exn -> collectOne severity { diagnostic with Exception = exn })
#endif
                | _ -> collectOne severity diagnostic

        let diagnosticsLogger =
            { new DiagnosticsLogger("DiagnosticsHandler") with
                member _.DiagnosticSink(exn, severity) = diagnosticSink severity exn
                member _.ErrorCount = errorCount
            }

        // Public members
        member _.DiagnosticsLogger = diagnosticsLogger

        member _.ErrorCount = errorCount

        member _.DiagnosticOptions
            with set opts = options <- opts

        member _.AnyErrors = errorCount > 0

        member _.CollectedPhasedDiagnostics =
            [|
                for struct (diagnostic, severity) in diagnosticsCollector -> diagnostic, severity
            |]

        member _.CollectedDiagnostics(symbolEnv: SymbolEnv option) =
            [|
                for struct (diagnostic, severity) in diagnosticsCollector do
                    yield!
                        DiagnosticHelpers.ReportDiagnostic(
                            options,
                            false,
                            mainInputFileName,
                            fileInfo,
                            diagnostic,
                            severity,
                            suggestNamesForErrors,
                            flatErrors,
                            symbolEnv
                        )
            |]

    let getLightSyntaxStatus fileName options =
        let indentationAwareSyntaxOnByDefault =
            List.exists (FileSystemUtils.checkSuffix fileName) FSharpIndentationAwareSyntaxFileSuffixes

        let indentationSyntaxStatus =
            if indentationAwareSyntaxOnByDefault then
                (options.IndentationAwareSyntax <> Some false)
            else
                (options.IndentationAwareSyntax = Some true)

        IndentationAwareSyntaxStatus(indentationSyntaxStatus, true)

    let createLexerFunction fileName options lexbuf (errHandler: DiagnosticsHandler) (ct: CancellationToken) =
        let indentationSyntaxStatus = getLightSyntaxStatus fileName options

        // If we're editing a script then we define INTERACTIVE otherwise COMPILED.
        // Since this parsing for intellisense we always define EDITING.
        let conditionalDefines =
            SourceFileImpl.GetImplicitConditionalDefinesForEditing options.IsInteractive
            @ options.ConditionalDefines

        // Note: we don't really attempt to intern strings across a large scope.
        let lexResourceManager = LexResourceManager()

        // When analyzing files using ParseOneFile, i.e. for the use of editing clients, we do not apply line directives.
        // TODO(pathmap): expose PathMap on the service API, and thread it through here
        let lexargs =
            mkLexargs (
                conditionalDefines,
                indentationSyntaxStatus,
                lexResourceManager,
                [],
                errHandler.DiagnosticsLogger,
                PathMap.empty,
                options.ApplyLineDirectives
            )

        let tokenizer =
            LexFilter.LexFilter(indentationSyntaxStatus, options.CompilingFSharpCore, Lexer.token lexargs true, lexbuf, false)

        if ct.CanBeCanceled then
            (fun _ ->
                ct.ThrowIfCancellationRequested()
                tokenizer.GetToken())
        else
            (fun _ -> tokenizer.GetToken())

    let createLexbuf langVersion strictIndentation sourceText =
        UnicodeLexing.SourceTextAsLexbuf(true, LanguageVersion(langVersion), strictIndentation, sourceText)

    let matchBraces
        (
            sourceText: ISourceText,
            fileName,
            options: FSharpParsingOptions,
            userOpName: string,
            suggestNamesForErrors: bool,
            ct: CancellationToken
        ) =
        // Make sure there is an DiagnosticsLogger installed whenever we do stuff that might record errors, even if we ultimately ignore the errors
        let delayedLogger = CapturingDiagnosticsLogger("matchBraces")
        use _ = UseDiagnosticsLogger delayedLogger
        use _ = UseBuildPhase BuildPhase.Parse

        Trace.TraceInformation("FCS: {0}.{1} ({2})", userOpName, "matchBraces", fileName)

        let matchingBraces = ResizeArray<_>()

        usingLexbufForParsing (createLexbuf options.LangVersionText options.StrictIndentation sourceText, fileName) (fun lexbuf ->
            let errHandler =
                DiagnosticsHandler(false, fileName, options.DiagnosticOptions, sourceText, suggestNamesForErrors, false)

            let lexfun = createLexerFunction fileName options lexbuf errHandler ct

            let parenTokensBalance t1 t2 =
                match t1, t2 with
                | LPAREN, RPAREN
                | LPAREN, RPAREN_IS_HERE
                | LBRACE _, RBRACE _
                | LBRACE_BAR, BAR_RBRACE
                | LBRACE _, RBRACE_IS_HERE
                | INTERP_STRING_BEGIN_PART _, INTERP_STRING_END _
                | INTERP_STRING_BEGIN_PART _, INTERP_STRING_PART _
                | INTERP_STRING_PART _, INTERP_STRING_PART _
                | INTERP_STRING_PART _, INTERP_STRING_END _
                | SIG, END
                | STRUCT, END
                | LBRACK_BAR, BAR_RBRACK
                | LBRACK, RBRACK
                | LBRACK_LESS, GREATER_RBRACK
                | BEGIN, END -> true
                | LQUOTE q1, RQUOTE q2 -> q1 = q2
                | _ -> false

            let rec matchBraces stack =
                match lexfun lexbuf, stack with
                | tok2, (tok1, braceOffset, m1) :: stackAfterMatch when parenTokensBalance tok1 tok2 ->
                    let m2 = lexbuf.LexemeRange

                    // For INTERP_STRING_PART and INTERP_STRING_END grab the one character
                    // range that corresponds to the "}" at the start of the token
                    let m2Start =
                        match tok2 with
                        | INTERP_STRING_PART _
                        | INTERP_STRING_END _ ->
                            mkFileIndexRange
                                m2.FileIndex
                                (mkPos m2.Start.Line (m2.Start.Column - braceOffset))
                                (mkPos m2.Start.Line (m2.Start.Column + 1))
                        | _ -> m2

                    matchingBraces.Add(m1, m2Start)

                    // INTERP_STRING_PART corresponds to both "} ... {" i.e. both the completion
                    // of a match and the start of a potential new one.
                    let stackAfterMatch =
                        match tok2 with
                        | INTERP_STRING_PART _ ->
                            let m2End =
                                mkFileIndexRange m2.FileIndex (mkPos m2.End.Line (max (m2.End.Column - 1 - braceOffset) 0)) m2.End

                            (tok2, braceOffset, m2End) :: stackAfterMatch
                        | _ -> stackAfterMatch

                    matchBraces stackAfterMatch

                | LPAREN | LBRACE _ | LBRACK | LBRACE_BAR | LBRACK_BAR | LQUOTE _ | LBRACK_LESS as tok, _ ->
                    matchBraces ((tok, 0, lexbuf.LexemeRange) :: stack)

                // INTERP_STRING_BEGIN_PART corresponds to $"... {" at the start of an interpolated string
                //
                // INTERP_STRING_PART corresponds to "} ... {" in the middle of an interpolated string (in
                //   this case it msut not have matched something on the stack, e.g. an incomplete '[' in the
                //   interpolation expression)
                //
                // Either way we start a new potential match at the last character
                | INTERP_STRING_BEGIN_PART _ | INTERP_STRING_PART _ as tok, _ ->
                    let braceOffset =
                        match tok with
                        | INTERP_STRING_BEGIN_PART(_, SynStringKind.TripleQuote, (LexerContinuation.Token(_, (_, _, dl, _, _) :: _))) ->
                            dl - 1
                        | _ -> 0

                    let m = lexbuf.LexemeRange

                    let m2 =
                        mkFileIndexRange m.FileIndex (mkPos m.End.Line (max (m.End.Column - 1 - braceOffset) 0)) m.End

                    matchBraces ((tok, braceOffset, m2) :: stack)

                | (EOF _ | LEX_FAILURE _), _ -> ()
                | _ -> matchBraces stack

            matchBraces [])

        matchingBraces.ToArray()

    let parseFile
        (
            sourceText: ISourceText,
            fileName,
            options: FSharpParsingOptions,
            userOpName: string,
            suggestNamesForErrors: bool,
            flatErrors: bool,
            identCapture: bool,
            ct: CancellationToken
        ) =
        Trace.TraceInformation("FCS: {0}.{1} ({2})", userOpName, "parseFile", fileName)

        use act =
            Activity.start "ParseAndCheckFile.parseFile" [| Activity.Tags.fileName, fileName |]

        let errHandler =
            DiagnosticsHandler(true, fileName, options.DiagnosticOptions, sourceText, suggestNamesForErrors, flatErrors)

        use _ = UseDiagnosticsLogger errHandler.DiagnosticsLogger

        use _ = UseBuildPhase BuildPhase.Parse

        let parseResult =
            usingLexbufForParsing (createLexbuf options.LangVersionText options.StrictIndentation sourceText, fileName) (fun lexbuf ->

                let lexfun = createLexerFunction fileName options lexbuf errHandler ct

                let isLastCompiland =
                    fileName.Equals(options.LastFileName, StringComparison.CurrentCultureIgnoreCase)
                    || IsScript(fileName)

                let isExe = options.IsExe

                try
                    ParseInput(
                        lexfun,
                        options.DiagnosticOptions,
                        errHandler.DiagnosticsLogger,
                        lexbuf,
                        None,
                        fileName,
                        (isLastCompiland, isExe),
                        identCapture,
                        Some userOpName
                    )
                with
                | :? OperationCanceledException -> reraise ()
                | e ->
                    errHandler.DiagnosticsLogger.StopProcessingRecovery e range0 // don't re-raise any exceptions, we must return None.
                    EmptyParsedInput(fileName, (isLastCompiland, isExe)))

        errHandler.CollectedDiagnostics(None), parseResult, errHandler.AnyErrors

    let ApplyLoadClosure
        (
            tcConfig,
            parsedMainInput,
            mainInputFileName,
            loadClosure: LoadClosure option,
            tcImports: TcImports,
            backgroundDiagnostics
        ) =

        // If additional references were brought in by the preprocessor then we need to process them
        match loadClosure with
        | Some loadClosure ->
            // Play unresolved references for this file.
            tcImports.ReportUnresolvedAssemblyReferences(loadClosure.UnresolvedReferences)

            // If there was a loadClosure, replay the errors and warnings from resolution, excluding parsing
            loadClosure.LoadClosureRootFileDiagnostics |> List.iter diagnosticSink

            let fileOfBackgroundError (diagnostic: PhasedDiagnostic, _) =
                match diagnostic.Range with
                | Some m -> Some m.FileName
                | None -> None

            let sameFile file hashLoadInFile =
                match file with
                | None -> false
                | Some file -> (0 = String.Compare(hashLoadInFile, file, StringComparison.OrdinalIgnoreCase))

            //  walk the list of #loads and keep the ones for this file.
            let hashLoadsInFile =
                loadClosure.SourceFiles |> List.filter (fun (_, ms) -> ms <> []) // #loaded file, ranges of #load

            let hashLoadBackgroundDiagnostics, otherBackgroundDiagnostics =
                backgroundDiagnostics
                |> Array.partition (fun backgroundError ->
                    hashLoadsInFile
                    |> List.exists (fst >> sameFile (fileOfBackgroundError backgroundError)))

            // Create single errors for the #load-ed files.
            // Group errors and warnings by file name.
            let hashLoadBackgroundDiagnosticsGroupedByFileName =
                hashLoadBackgroundDiagnostics
                |> Array.map (fun err -> fileOfBackgroundError err, err)
                |> Array.groupBy fst // fileWithErrors, error list

            //  Join the sets and report errors.
            //  It is by-design that these messages are only present in the language service. A true build would report the errors at their
            //  spots in the individual source files.
            for fileOfHashLoad, rangesOfHashLoad in hashLoadsInFile do
                for file, errorGroupedByFileName in hashLoadBackgroundDiagnosticsGroupedByFileName do
                    if sameFile file fileOfHashLoad then
                        for rangeOfHashLoad in rangesOfHashLoad do // Handle the case of two #loads of the same file
                            let diagnostics =
                                errorGroupedByFileName |> Array.map (fun (_, (pe, f)) -> pe.Exception, f) // Strip the build phase here. It will be replaced, in total, with TypeCheck

                            let errors =
                                [
                                    for err, severity in diagnostics do
                                        if severity = FSharpDiagnosticSeverity.Error then
                                            err
                                ]

                            let warnings =
                                [
                                    for err, severity in diagnostics do
                                        if severity = FSharpDiagnosticSeverity.Warning then
                                            err
                                ]

                            let infos =
                                [
                                    for err, severity in diagnostics do
                                        if severity = FSharpDiagnosticSeverity.Info then
                                            err
                                ]

                            let message = HashLoadedSourceHasIssues(infos, warnings, errors, rangeOfHashLoad)

                            if isNil errors && isNil warnings then warning message
                            elif isNil errors then warning message
                            else errorR message

            // Replay other background errors.
            for diagnostic, severity in otherBackgroundDiagnostics do
                match severity with
                | FSharpDiagnosticSeverity.Info -> informationalWarning diagnostic.Exception
                | FSharpDiagnosticSeverity.Warning -> warning diagnostic.Exception
                | FSharpDiagnosticSeverity.Error -> errorR diagnostic.Exception
                | FSharpDiagnosticSeverity.Hidden -> ()

        | None ->
            // For non-scripts, check for disallow #r and #load.
            ApplyMetaCommandsFromInputToTcConfig(
                tcConfig,
                parsedMainInput,
                Path.GetDirectoryName mainInputFileName,
                tcImports.DependencyProvider
            )
            |> ignore

    // Type check a single file against an initial context, gleaning both errors and intellisense information.
    let CheckOneFile
        (
            parseResults: FSharpParseFileResults,
            sourceText: ISourceText,
            mainInputFileName: string,
            projectOptions: FSharpProjectOptions,
            projectFileName: string,
            tcConfig: TcConfig,
            tcGlobals: TcGlobals,
            tcImports: TcImports,
            tcState: TcState,
            moduleNamesDict: ModuleNamesDict,
            loadClosure: LoadClosure option,
            backgroundDiagnostics: (PhasedDiagnostic * FSharpDiagnosticSeverity)[],
            suggestNamesForErrors: bool
        ) =

        cancellable {
            use _ =
                Activity.start
                    "ParseAndCheckFile.CheckOneFile"
                    [|
                        Activity.Tags.fileName, mainInputFileName
                        Activity.Tags.length, sourceText.Length.ToString()
                    |]

            let parsedMainInput = parseResults.ParseTree

            // Initialize the error handler
            let errHandler =
                DiagnosticsHandler(
                    true,
                    mainInputFileName,
                    tcConfig.diagnosticsOptions,
                    sourceText,
                    suggestNamesForErrors,
                    tcConfig.flatErrors
                )

            use _ = UseDiagnosticsLogger errHandler.DiagnosticsLogger

            use _unwindBP = UseBuildPhase BuildPhase.TypeCheck

            // Apply nowarns to tcConfig (may generate errors, so ensure diagnosticsLogger is installed)
            let tcConfig =
                ApplyNoWarnsToTcConfig(tcConfig, parsedMainInput, Path.GetDirectoryName mainInputFileName)

            // update the error handler with the modified tcConfig
            errHandler.DiagnosticOptions <- tcConfig.diagnosticsOptions

            // If additional references were brought in by the preprocessor then we need to process them
            ApplyLoadClosure(tcConfig, parsedMainInput, mainInputFileName, loadClosure, tcImports, backgroundDiagnostics)

            // Typecheck the real input.
            let sink = TcResultsSinkImpl(tcGlobals, sourceText = sourceText)

            let! resOpt =
                cancellable {
                    try
                        let checkForErrors () =
                            (parseResults.ParseHadErrors || errHandler.ErrorCount > 0)

                        let parsedMainInput, _moduleNamesDict =
                            DeduplicateParsedInputModuleName moduleNamesDict parsedMainInput

                        // Typecheck is potentially a long running operation. We chop it up here with an Eventually continuation and, at each slice, give a chance
                        // for the client to claim the result as obsolete and have the typecheck abort.

                        use _unwind =
                            new CompilationGlobalsScope(errHandler.DiagnosticsLogger, BuildPhase.TypeCheck)

                        let! result =
                            CheckOneInputAndFinish(
                                checkForErrors,
                                tcConfig,
                                tcImports,
                                tcGlobals,
                                None,
                                TcResultsSink.WithSink sink,
                                tcState,
                                parsedMainInput
                            )

                        return result
                    with e ->
                        errorR e

                        let mty =
                            Construct.NewEmptyModuleOrNamespaceType(ModuleOrNamespaceKind.Namespace true)

                        return ((tcState.TcEnvFromSignatures, EmptyTopAttrs, [], [ mty ]), tcState)
                }

            // Play background errors and warnings for this file.
            do
                for err, severity in backgroundDiagnostics do
                    diagnosticSink (err, severity)

            let (tcEnvAtEnd, _, implFiles, ccuSigsForFiles), tcState = resOpt

            let symbolEnv = SymbolEnv(tcGlobals, tcState.Ccu, Some tcState.CcuSig, tcImports)
            let errors = errHandler.CollectedDiagnostics(Some symbolEnv)

            let res =
                TypeCheckInfo(
                    tcConfig,
                    tcGlobals,
                    List.head ccuSigsForFiles,
                    tcState.Ccu,
                    tcImports,
                    tcEnvAtEnd.AccessRights,
                    projectFileName,
                    mainInputFileName,
                    projectOptions,
                    sink.GetResolutions(),
                    sink.GetSymbolUses(),
                    tcEnvAtEnd.NameEnv,
                    loadClosure,
                    List.tryHead implFiles,
                    sink.GetOpenDeclarations()
                )

            return errors, res
        }

[<Sealed>]
type FSharpProjectContext(thisCcu: CcuThunk, assemblies: FSharpAssembly list, ad: AccessorDomain, projectOptions: FSharpProjectOptions) =

    member _.ProjectOptions = projectOptions

    member _.GetReferencedAssemblies() = assemblies

    member _.AccessibilityRights = FSharpAccessibilityRights(thisCcu, ad)

/// A live object of this type keeps the background corresponding background builder (and type providers) alive (through reference-counting).
//
// Note: objects returned by the methods of this type do not require the corresponding background builder to be alive.
[<Sealed>]
type FSharpCheckFileResults
    (
        fileName: string,
        errors: FSharpDiagnostic[],
        scopeOptX: TypeCheckInfo option,
        dependencyFiles: string[],
        builderX: IncrementalBuilder option,
        keepAssemblyContents: bool
    ) =

    // Here 'details' keeps 'builder' alive
    let details =
        match scopeOptX with
        | None -> None
        | Some scopeX -> Some(scopeX, builderX)

    static let emptyFindDeclResult =
        FindDeclResult.DeclNotFound(FindDeclFailureReason.Unknown "")

    member _.Diagnostics = errors

    member _.HasFullTypeCheckInfo = details.IsSome

    member _.TryGetCurrentTcImports() =
        match details with
        | None -> None
        | Some(scope, _builderOpt) -> Some scope.TcImports

    /// Intellisense autocompletions
    member _.GetDeclarationListInfo(parsedFileResults, line, lineText, partialName, ?getAllEntities, ?completionContextAtPos) =
        let getAllEntities = defaultArg getAllEntities (fun () -> [])

        match details with
        | None -> DeclarationListInfo.Empty
        | Some(scope, _builderOpt) ->
            scope.GetDeclarations(parsedFileResults, line, lineText, partialName, completionContextAtPos, getAllEntities)

    member _.GetDeclarationListSymbols(parsedFileResults, line, lineText, partialName, ?getAllEntities) =
        let getAllEntities = defaultArg getAllEntities (fun () -> [])

        match details with
        | None -> []
        | Some(scope, _builderOpt) -> scope.GetDeclarationListSymbols(parsedFileResults, line, lineText, partialName, getAllEntities)

    member _.GetKeywordTooltip(names: string list) =
        ToolTipText.ToolTipText
            [
                for kw in names do
                    match Tokenization.FSharpKeywords.KeywordsDescriptionLookup kw with
                    | None -> ()
                    | Some kwDescription ->
                        let kwText = kw |> TaggedText.tagKeyword |> wordL |> LayoutRender.toArray
                        yield ToolTipElement.Single(kwText, FSharpXmlDoc.FromXmlText(Xml.XmlDoc([| kwDescription |], range.Zero)))
            ]

    /// Resolve the names at the given location to give a data tip
    member _.GetToolTip(line, colAtEndOfNames, lineText, names, tokenTag, width) =
        match tokenTagToTokenId tokenTag with
        | TOKEN_IDENT ->
            match details with
            | None -> emptyToolTip
            | Some(scope, _builderOpt) -> scope.GetStructuredToolTipText(line, lineText, colAtEndOfNames, names, width)
        | TOKEN_STRING
        | TOKEN_STRING_TEXT ->
            match details with
            | None -> emptyToolTip
            | Some(scope, _builderOpt) -> scope.GetReferenceResolutionStructuredToolTipText(line, colAtEndOfNames, width)
        | _ -> emptyToolTip

    member _.GetDescription(symbol: FSharpSymbol, inst: (FSharpGenericParameter * FSharpType) list, displayFullName, range: range) =
        match details with
        | None -> emptyToolTip
        | Some(scope, _builderOpt) -> scope.GetDescription(symbol, inst, displayFullName, range)

    member _.GetF1Keyword(line, colAtEndOfNames, lineText, names) =
        match details with
        | None -> None
        | Some(scope, _builderOpt) -> scope.GetF1Keyword(line, lineText, colAtEndOfNames, names)

    // Resolve the names at the given location to a set of methods
    member _.GetMethods(line, colAtEndOfNames, lineText, names) =
        match details with
        | None -> MethodGroup.Empty
        | Some(scope, _builderOpt) -> scope.GetMethods(line, lineText, colAtEndOfNames, names)

    member _.GetDeclarationLocation(line, colAtEndOfNames, lineText, names, ?preferFlag) =
        match details with
        | None -> emptyFindDeclResult
        | Some(scope, _builderOpt) -> scope.GetDeclarationLocation(line, lineText, colAtEndOfNames, names, preferFlag)

    member this.GetSymbolUseAtLocation(line, colAtEndOfNames, lineText, names) =
        this.GetSymbolUsesAtLocation(line, colAtEndOfNames, lineText, names)
        |> List.tryHead

    member _.GetSymbolUsesAtLocation(line, colAtEndOfNames, lineText, names) =
        match details with
        | None -> List.empty
        | Some(scope, _builderOpt) ->
            scope.GetSymbolUsesAtLocation(line, lineText, colAtEndOfNames, names)
            |> List.map (fun (sym, itemWithInst, denv, m) ->
                FSharpSymbolUse(denv, sym, itemWithInst.TyparInstantiation, ItemOccurence.Use, m))

    member _.GetMethodsAsSymbols(line, colAtEndOfNames, lineText, names) =
        match details with
        | None -> None
        | Some(scope, _builderOpt) ->
            scope.GetMethodsAsSymbols(line, lineText, colAtEndOfNames, names)
            |> Option.map (fun (symbols, denv, m) ->
                symbols
                |> List.map (fun (sym, itemWithInst) -> FSharpSymbolUse(denv, sym, itemWithInst.TyparInstantiation, ItemOccurence.Use, m)))

    member _.GetSymbolAtLocation(line, colAtEndOfNames, lineStr, names) =
        match details with
        | None -> None
        | Some(scope, _builderOpt) ->
            scope.GetSymbolUseAtLocation(line, lineStr, colAtEndOfNames, names)
            |> Option.map (fun (sym, _, _, _) -> sym)

    member info.GetFormatSpecifierLocations() =
        info.GetFormatSpecifierLocationsAndArity() |> Array.map fst

    member _.GetFormatSpecifierLocationsAndArity() =
        match details with
        | None -> [||]
        | Some(scope, _builderOpt) -> scope.GetFormatSpecifierLocationsAndArity()

    member _.GetSemanticClassification(range: range option) =
        match details with
        | None -> [||]
        | Some(scope, _builderOpt) -> scope.GetSemanticClassification(range)

    member _.PartialAssemblySignature =
        match details with
        | None -> failwith "not available"
        | Some(scope, _builderOpt) -> scope.PartialAssemblySignatureForFile

    member _.ProjectContext =
        match details with
        | None -> failwith "not available"
        | Some(scope, _builderOpt) ->
            FSharpProjectContext(scope.ThisCcu, scope.GetReferencedAssemblies(), scope.AccessRights, scope.ProjectOptions)

    member _.DependencyFiles = dependencyFiles

    member _.GetAllUsesOfAllSymbolsInFile(?cancellationToken: CancellationToken) =
        match details with
        | None -> Seq.empty
        | Some(scope, _builderOpt) ->
            let cenv = scope.SymbolEnv

            seq {
                for symbolUseChunk in scope.ScopeSymbolUses.AllUsesOfSymbols do
                    for symbolUse in symbolUseChunk do
                        cancellationToken |> Option.iter (fun ct -> ct.ThrowIfCancellationRequested())

                        if symbolUse.ItemOccurence <> ItemOccurence.RelatedText then
                            let symbol = FSharpSymbol.Create(cenv, symbolUse.ItemWithInst.Item)
                            let inst = symbolUse.ItemWithInst.TyparInstantiation
                            FSharpSymbolUse(symbolUse.DisplayEnv, symbol, inst, symbolUse.ItemOccurence, symbolUse.Range)
            }

    member _.GetUsesOfSymbolInFile(symbol: FSharpSymbol, ?cancellationToken: CancellationToken) =
        match details with
        | None -> [||]
        | Some(scope, _builderOpt) ->
            [|
                for symbolUse in
                    scope.ScopeSymbolUses.GetUsesOfSymbol(symbol.Item)
                    |> Seq.distinctBy (fun symbolUse -> symbolUse.ItemOccurence, symbolUse.Range) do
                    cancellationToken |> Option.iter (fun ct -> ct.ThrowIfCancellationRequested())

                    if symbolUse.ItemOccurence <> ItemOccurence.RelatedText then
                        let inst = symbolUse.ItemWithInst.TyparInstantiation
                        FSharpSymbolUse(symbolUse.DisplayEnv, symbol, inst, symbolUse.ItemOccurence, symbolUse.Range)
            |]

    member _.GetVisibleNamespacesAndModulesAtPoint(pos: pos) =
        match details with
        | None -> [||]
        | Some(scope, _builderOpt) -> scope.GetVisibleNamespacesAndModulesAtPosition(pos) |> List.toArray

    member _.IsRelativeNameResolvable(cursorPos: pos, plid: string list, item: Item) =
        match details with
        | None -> true
        | Some(scope, _builderOpt) -> scope.IsRelativeNameResolvable(cursorPos, plid, item)

    member _.IsRelativeNameResolvableFromSymbol(cursorPos: pos, plid: string list, symbol: FSharpSymbol) =
        match details with
        | None -> true
        | Some(scope, _builderOpt) -> scope.IsRelativeNameResolvableFromSymbol(cursorPos, plid, symbol)

    member _.GetDisplayContextForPos(cursorPos: pos) =
        match details with
        | None -> None
        | Some(scope, _builderOpt) ->
            let (nenv, _), _ = scope.GetBestDisplayEnvForPos cursorPos
            Some(FSharpDisplayContext(fun _ -> nenv.DisplayEnv))

    member _.GenerateSignature(?pageWidth: int) =
        match details with
        | None -> None
        | Some(scope, _builderOpt) ->
            scope.ImplementationFile
            |> Option.map (fun implFile ->
                let denv = DisplayEnv.InitialForSigFileGeneration scope.TcGlobals
                let infoReader = InfoReader(scope.TcGlobals, scope.TcImports.GetImportMap())
                let (CheckedImplFile(contents = mexpr)) = implFile

                let ad =
                    match scopeOptX with
                    | Some scope -> scope.AccessRights
                    | _ -> AccessibleFromSomewhere

                let layout =
                    NicePrint.layoutImpliedSignatureOfModuleOrNamespace true denv infoReader ad range0 mexpr

                match pageWidth with
                | None -> layout
                | Some pageWidth -> Display.squashTo pageWidth layout
                |> LayoutRender.showL
                |> SourceText.ofString)

    member internal _.CalculateSignatureHash() =
        let visibility = Fsharp.Compiler.SignatureHash.PublicAndInternal

        match details with
        | None -> failwith "Typechecked details not available for CalculateSignatureHash() operation."
        | Some(scope, _builderOpt) ->
            scope.ImplementationFile
            |> Option.map (fun implFile ->
                Fsharp.Compiler.SignatureHash.calculateSignatureHashOfFiles [ implFile ] scope.TcGlobals visibility)

    member _.ImplementationFile =
        if not keepAssemblyContents then
            invalidOp
                "The 'keepAssemblyContents' flag must be set to true on the FSharpChecker in order to access the checked contents of assemblies"

        scopeOptX
        |> Option.map (fun scope ->
            let cenv =
                SymbolEnv(scope.TcGlobals, scope.ThisCcu, Some scope.CcuSigForFile, scope.TcImports)

            scope.ImplementationFile
            |> Option.map (fun implFile -> FSharpImplementationFileContents(cenv, implFile)))
        |> Option.defaultValue None

    member _.OpenDeclarations =
        scopeOptX
        |> Option.map (fun scope ->
            let cenv = scope.SymbolEnv

            scope.OpenDeclarations
            |> Array.map (fun x ->
                let modules = x.Modules |> List.map (fun x -> FSharpEntity(cenv, x))
                let types = x.Types |> List.map (fun x -> FSharpType(cenv, x))
                FSharpOpenDeclaration(x.Target, x.Range, modules, types, x.AppliedScope, x.IsOwnNamespace)))
        |> Option.defaultValue [||]

    override _.ToString() =
        "FSharpCheckFileResults(" + fileName + ")"

    static member MakeEmpty(fileName: string, creationErrors: FSharpDiagnostic[], keepAssemblyContents) =
        FSharpCheckFileResults(fileName, creationErrors, None, [||], None, keepAssemblyContents)

    static member JoinErrors
        (
            isIncompleteTypeCheckEnvironment,
            creationErrors: FSharpDiagnostic[],
            parseErrors: FSharpDiagnostic[],
            tcErrors: FSharpDiagnostic[]
        ) =
        [|
            yield! creationErrors
            yield! parseErrors
            if isIncompleteTypeCheckEnvironment then
                yield! Seq.truncate maxTypeCheckErrorsOutOfProjectContext tcErrors
            else
                yield! tcErrors
        |]

    static member Make
        (
            mainInputFileName: string,
            projectFileName,
            tcConfig,
            tcGlobals,
            isIncompleteTypeCheckEnvironment: bool,
            builder: IncrementalBuilder option,
            projectOptions,
            dependencyFiles,
            creationErrors: FSharpDiagnostic[],
            parseErrors: FSharpDiagnostic[],
            tcErrors: FSharpDiagnostic[],
            keepAssemblyContents,
            ccuSigForFile,
            thisCcu,
            tcImports,
            tcAccessRights,
            sResolutions,
            sSymbolUses,
            sFallback,
            loadClosure,
            implFileOpt,
            openDeclarations
        ) =

        let tcFileInfo =
            TypeCheckInfo(
                tcConfig,
                tcGlobals,
                ccuSigForFile,
                thisCcu,
                tcImports,
                tcAccessRights,
                projectFileName,
                mainInputFileName,
                projectOptions,
                sResolutions,
                sSymbolUses,
                sFallback,
                loadClosure,
                implFileOpt,
                openDeclarations
            )

        let errors =
            FSharpCheckFileResults.JoinErrors(isIncompleteTypeCheckEnvironment, creationErrors, parseErrors, tcErrors)

        FSharpCheckFileResults(mainInputFileName, errors, Some tcFileInfo, dependencyFiles, builder, keepAssemblyContents)

    static member CheckOneFile
        (
            parseResults: FSharpParseFileResults,
            sourceText: ISourceText,
            mainInputFileName: string,
            projectFileName: string,
            tcConfig: TcConfig,
            tcGlobals: TcGlobals,
            tcImports: TcImports,
            tcState: TcState,
            moduleNamesDict: ModuleNamesDict,
            loadClosure: LoadClosure option,
            backgroundDiagnostics: (PhasedDiagnostic * FSharpDiagnosticSeverity)[],
            isIncompleteTypeCheckEnvironment: bool,
            projectOptions: FSharpProjectOptions,
            builder: IncrementalBuilder option,
            dependencyFiles: string[],
            creationErrors: FSharpDiagnostic[],
            parseErrors: FSharpDiagnostic[],
            keepAssemblyContents: bool,
            suggestNamesForErrors: bool
        ) =
        cancellable {
            let! tcErrors, tcFileInfo =
                ParseAndCheckFile.CheckOneFile(
                    parseResults,
                    sourceText,
                    mainInputFileName,
                    projectOptions,
                    projectFileName,
                    tcConfig,
                    tcGlobals,
                    tcImports,
                    tcState,
                    moduleNamesDict,
                    loadClosure,
                    backgroundDiagnostics,
                    suggestNamesForErrors
                )

            let errors =
                FSharpCheckFileResults.JoinErrors(isIncompleteTypeCheckEnvironment, creationErrors, parseErrors, tcErrors)

            let results =
                FSharpCheckFileResults(mainInputFileName, errors, Some tcFileInfo, dependencyFiles, builder, keepAssemblyContents)

            return results
        }

[<Sealed>]
// 'details' is an option because the creation of the tcGlobals etc. for the project may have failed.
type FSharpCheckProjectResults
    (
        projectFileName: string,
        tcConfigOption: TcConfig option,
        keepAssemblyContents: bool,
        diagnostics: FSharpDiagnostic[],
        details:
            (TcGlobals *
            TcImports *
            CcuThunk *
            ModuleOrNamespaceType *
            Choice<IncrementalBuilder, Async<TcSymbolUses seq>> *
            TopAttribs option *
            (unit -> IRawFSharpAssemblyData option) *
            ILAssemblyRef *
            AccessorDomain *
            CheckedImplFile list option *
            string[] *
            FSharpProjectOptions) option
    ) =

    let getDetails () =
        match details with
        | None ->
            invalidOp (
                "The project has no results due to critical errors in the project options. Check the HasCriticalErrors before accessing the detailed results. Errors: "
                + String.concat "\n" [ for e in diagnostics -> e.Message ]
            )
        | Some d -> d

    let getTcConfig () =
        match tcConfigOption with
        | None ->
            invalidOp (
                "The project has no results due to critical errors in the project options. Check the HasCriticalErrors before accessing the detailed results. Errors: "
                + String.concat "\n" [ for e in diagnostics -> e.Message ]
            )
        | Some d -> d

    member _.Diagnostics = diagnostics

    member _.HasCriticalErrors = details.IsNone

    member _.AssemblySignature =
        let tcGlobals, tcImports, thisCcu, ccuSig, _, topAttribs, _, _, _, _, _, _ =
            getDetails ()

        FSharpAssemblySignature(tcGlobals, thisCcu, ccuSig, tcImports, topAttribs, ccuSig)

    // TODO: Looks like we don't need this
    member _.TypedImplementationFiles =
        if not keepAssemblyContents then
            invalidOp
                "The 'keepAssemblyContents' flag must be set to true on the FSharpChecker in order to access the checked contents of assemblies"

        let tcGlobals, tcImports, thisCcu, _, _, _, _, _, _, tcAssemblyExpr, _, _ =
            getDetails ()

        let mimpls =
            match tcAssemblyExpr with
            | None -> []
            | Some mimpls -> mimpls

        tcGlobals, thisCcu, tcImports, mimpls

    member info.AssemblyContents =
        if not keepAssemblyContents then
            invalidOp
                "The 'keepAssemblyContents' flag must be set to true on the FSharpChecker in order to access the checked contents of assemblies"

        let tcGlobals, tcImports, thisCcu, ccuSig, _, _, _, _, _, tcAssemblyExpr, _, _ =
            getDetails ()

        let mimpls =
            match tcAssemblyExpr with
            | None -> []
            | Some mimpls -> mimpls

        FSharpAssemblyContents(tcGlobals, thisCcu, Some ccuSig, tcImports, mimpls)

    member _.GetOptimizedAssemblyContents() =
        if not keepAssemblyContents then
            invalidOp
                "The 'keepAssemblyContents' flag must be set to true on the FSharpChecker in order to access the checked contents of assemblies"

        let tcGlobals, tcImports, thisCcu, ccuSig, _, _, _, _, _, tcAssemblyExpr, _, _ =
            getDetails ()

        let mimpls =
            match tcAssemblyExpr with
            | None -> []
            | Some mimpls -> mimpls

        let outfile = "" // only used if tcConfig.writeTermsToFiles is true
        let importMap = tcImports.GetImportMap()
        let optEnv0 = GetInitialOptimizationEnv(tcImports, tcGlobals)
        let tcConfig = getTcConfig ()
        let isIncrementalFragment = false
        let tcVal = LightweightTcValForUsingInBuildMethodCall tcGlobals

        let optimizedImpls, _optimizationData, _ =
            ApplyAllOptimizations(tcConfig, tcGlobals, tcVal, outfile, importMap, isIncrementalFragment, optEnv0, thisCcu, mimpls)

        let mimpls =
            match optimizedImpls with
            | CheckedAssemblyAfterOptimization files -> files |> List.map (fun implFile -> implFile.ImplFile)

        FSharpAssemblyContents(tcGlobals, thisCcu, Some ccuSig, tcImports, mimpls)

    // Not, this does not have to be a SyncOp, it can be called from any thread
    // TODO: this should be async
    member _.GetUsesOfSymbol(symbol: FSharpSymbol, ?cancellationToken: CancellationToken) =
        let _, _, _, _, builderOrSymbolUses, _, _, _, _, _, _, _ = getDetails ()

        let results =
            match builderOrSymbolUses with
            | Choice1Of2 builder ->
                builder.SourceFiles
                |> Array.ofList
                |> Array.collect (fun x ->
                    match builder.GetCheckResultsForFileInProjectEvenIfStale x with
                    | Some partialCheckResults ->
                        match partialCheckResults.TryPeekTcInfoWithExtras() with
                        | Some(_, tcInfoExtras) -> tcInfoExtras.TcSymbolUses.GetUsesOfSymbol symbol.Item
                        | _ -> [||]
                    | _ -> [||])
                |> Array.toSeq
            | Choice2Of2 task ->
                Async.RunSynchronously(
                    async {
                        let! tcSymbolUses = task

                        return
                            seq {
                                for symbolUses in tcSymbolUses do
                                    yield! symbolUses.GetUsesOfSymbol symbol.Item
                            }
                    },
                    ?cancellationToken = cancellationToken
                )

        results
        |> Seq.filter (fun symbolUse -> symbolUse.ItemOccurence <> ItemOccurence.RelatedText)
        |> Seq.distinctBy (fun symbolUse -> symbolUse.ItemOccurence, symbolUse.Range)
        |> Seq.map (fun symbolUse ->
            cancellationToken |> Option.iter (fun ct -> ct.ThrowIfCancellationRequested())
            let inst = symbolUse.ItemWithInst.TyparInstantiation
            FSharpSymbolUse(symbolUse.DisplayEnv, symbol, inst, symbolUse.ItemOccurence, symbolUse.Range))
        |> Seq.toArray

    // Not, this does not have to be a SyncOp, it can be called from any thread
    // TODO: this should be async
    member _.GetAllUsesOfAllSymbols(?cancellationToken: CancellationToken) =
        let tcGlobals, tcImports, thisCcu, ccuSig, builderOrSymbolUses, _, _, _, _, _, _, _ =
            getDetails ()

        let cenv = SymbolEnv(tcGlobals, thisCcu, Some ccuSig, tcImports)

        let tcSymbolUses =
            match builderOrSymbolUses with
            | Choice1Of2 builder ->
                builder.SourceFiles
                |> Array.ofList
                |> Array.map (fun x ->
                    match builder.GetCheckResultsForFileInProjectEvenIfStale x with
                    | Some partialCheckResults ->
                        match partialCheckResults.TryPeekTcInfoWithExtras() with
                        | Some(_, tcInfoExtras) -> tcInfoExtras.TcSymbolUses
                        | _ -> TcSymbolUses.Empty
                    | _ -> TcSymbolUses.Empty)
                |> Array.toSeq
            | Choice2Of2 tcSymbolUses -> Async.RunSynchronously(tcSymbolUses, ?cancellationToken = cancellationToken)

        [|
            for r in tcSymbolUses do
                for symbolUseChunk in r.AllUsesOfSymbols do
                    for symbolUse in symbolUseChunk do
                        cancellationToken |> Option.iter (fun ct -> ct.ThrowIfCancellationRequested())

                        if symbolUse.ItemOccurence <> ItemOccurence.RelatedText then
                            let symbol = FSharpSymbol.Create(cenv, symbolUse.ItemWithInst.Item)
                            let inst = symbolUse.ItemWithInst.TyparInstantiation
                            FSharpSymbolUse(symbolUse.DisplayEnv, symbol, inst, symbolUse.ItemOccurence, symbolUse.Range)
        |]

    member _.ProjectContext =
        let tcGlobals, tcImports, thisCcu, _, _, _, _, _, ad, _, _, projectOptions =
            getDetails ()

        let assemblies =
            tcImports.GetImportedAssemblies()
            |> List.map (fun x -> FSharpAssembly(tcGlobals, tcImports, x.FSharpViewOfMetadata))

        FSharpProjectContext(thisCcu, assemblies, ad, projectOptions)

    member _.DependencyFiles =
        let _tcGlobals, _, _, _, _, _, _, _, _, _, dependencyFiles, _ = getDetails ()
        dependencyFiles

    member _.AssemblyFullName =
        let _tcGlobals, _, _, _, _, _, _, ilAssemRef, _, _, _, _ = getDetails ()
        ilAssemRef.QualifiedName

    override _.ToString() =
        "FSharpCheckProjectResults(" + projectFileName + ")"

type FsiInteractiveChecker(legacyReferenceResolver, tcConfig: TcConfig, tcGlobals: TcGlobals, tcImports: TcImports, tcState) =

    let keepAssemblyContents = false

    member _.ParseAndCheckInteraction(sourceText: ISourceText, ?userOpName: string) =
        cancellable {
            let userOpName = defaultArg userOpName "Unknown"
            let fileName = Path.Combine(tcConfig.implicitIncludeDir, "stdin.fsx")
            let suggestNamesForErrors = true // Will always be true, this is just for readability
            // Note: projectSourceFiles is only used to compute isLastCompiland, and is ignored if Build.IsScript(mainInputFileName) is true (which it is in this case).
            let parsingOptions =
                FSharpParsingOptions.FromTcConfig(tcConfig, [| fileName |], true)

            let! ct = Cancellable.token ()

            let parseErrors, parsedInput, anyErrors =
                ParseAndCheckFile.parseFile (
                    sourceText,
                    fileName,
                    parsingOptions,
                    userOpName,
                    suggestNamesForErrors,
                    tcConfig.flatErrors,
                    tcConfig.captureIdentifiersWhenParsing,
                    ct
                )

            let dependencyFiles = [||] // interactions have no dependencies

            let parseResults =
                FSharpParseFileResults(parseErrors, parsedInput, parseHadErrors = anyErrors, dependencyFiles = dependencyFiles)

            let backgroundDiagnostics = [||]
            let reduceMemoryUsage = ReduceMemoryFlag.Yes
            let assumeDotNetFramework = (tcConfig.primaryAssembly = PrimaryAssembly.Mscorlib)

            let applyCompilerOptions tcConfigB =
                let fsiCompilerOptions = CompilerOptions.GetCoreFsiCompilerOptions tcConfigB
                CompilerOptions.ParseCompilerOptions(ignore, fsiCompilerOptions, [])

            let loadClosure =
                LoadClosure.ComputeClosureOfScriptText(
                    legacyReferenceResolver,
                    defaultFSharpBinariesDir,
                    fileName,
                    sourceText,
                    CodeContext.Editing,
                    tcConfig.useSimpleResolution,
                    tcConfig.useFsiAuxLib,
                    tcConfig.useSdkRefs,
                    tcConfig.sdkDirOverride,
                    LexResourceManager(),
                    applyCompilerOptions,
                    assumeDotNetFramework,
                    tryGetMetadataSnapshot = (fun _ -> None),
                    reduceMemoryUsage = reduceMemoryUsage,
                    dependencyProvider = tcImports.DependencyProvider
                )

            let projectOptions =
                {
                    ProjectFileName = "script.fsproj"
                    ProjectId = None
                    SourceFiles = [||]
                    OtherOptions = [||]
                    ReferencedProjects = [||]
                    IsIncompleteTypeCheckEnvironment = false
                    UseScriptResolutionRules = false
                    LoadTime = DateTime.Now
                    UnresolvedReferences = None
                    OriginalLoadReferences = []
                    Stamp = None
                }

            let! tcErrors, tcFileInfo =
                ParseAndCheckFile.CheckOneFile(
                    parseResults,
                    sourceText,
                    fileName,
                    projectOptions,
                    projectOptions.ProjectFileName,
                    tcConfig,
                    tcGlobals,
                    tcImports,
                    tcState,
                    Map.empty,
                    Some loadClosure,
                    backgroundDiagnostics,
                    suggestNamesForErrors
                )

            let errors = Array.append parseErrors tcErrors

            let typeCheckResults =
                FSharpCheckFileResults(fileName, errors, Some tcFileInfo, dependencyFiles, None, false)

            let details =
                (tcGlobals,
                 tcImports,
                 tcFileInfo.ThisCcu,
                 tcFileInfo.CcuSigForFile,
                 Choice2Of2(tcFileInfo.ScopeSymbolUses |> Seq.singleton |> async.Return),
                 None,
                 (fun () -> None),
                 mkSimpleAssemblyRef "stdin",
                 tcState.TcEnvFromImpls.AccessRights,
                 None,
                 dependencyFiles,
                 projectOptions)

            let projectResults =
                FSharpCheckProjectResults(fileName, Some tcConfig, keepAssemblyContents, errors, Some details)

            return parseResults, typeCheckResults, projectResults
        }

/// The result of calling TypeCheckResult including the possibility of abort and background compiler not caught up.
[<RequireQualifiedAccess>]
type public FSharpCheckFileAnswer =
    /// Aborted because cancellation caused an abandonment of the operation
    | Aborted

    /// Success
    | Succeeded of FSharpCheckFileResults
