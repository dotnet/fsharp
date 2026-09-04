// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Navigation
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.OLE.Interop
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols

open CancellableTasks

#nowarn "57" // the transparent-compiler FCS surface (UsesTransparentCompiler, project snapshots) is experimental

/// Serves Object Browser and Class View for F#. Both windows consume the same library; the shell
/// distinguishes them with `LLF_TRUENESTING` on every request.
///
/// Producing a project's symbols means a full `ParseAndCheckProject`, which is far too slow to run
/// inside a synchronous COM call. Instead a node whose project has not been checked yet renders a
/// placeholder, the check runs in the background, and the update counters bring the shell back for
/// the real content.
[<Sealed>]
type internal FSharpObjectBrowserLibrary(workspace: VisualStudioWorkspace, libraryGuid: Guid) as this =

    let navInfoFactory = FSharpNavInfoFactory libraryGuid

    // Resolved on first use: the description pane is only ever filled from the UI thread.
    let documentationBuilder =
        lazy XmlDocumentation.CreateDocumentationBuilder(ServiceProvider.GlobalProvider.XMLMemberIndexService)

    let counterGate = obj ()
    let mutable packageVersion = 0u
    let mutable contentVersion = 0u

    let symbols = ConcurrentDictionary<ProjectId, ProjectSymbols>()
    let checking = ConcurrentDictionary<ProjectId, bool>()
    let stale = ConcurrentDictionary<ProjectId, bool>()

    /// A whole-project check is heavy, and expanding a few nodes in a row queues several; keep the
    /// checker from being flooded while still making progress on the ones the user is looking at.
    let checkThrottle = new SemaphoreSlim(2)

    let bumpContent () =
        lock counterGate (fun () -> contentVersion <- contentVersion + 1u)

    let bumpEverything () =
        lock counterGate (fun () ->
            packageVersion <- packageVersion + 1u
            contentVersion <- contentVersion + 1u)

    let tryProject (projectId: ProjectId) =
        match workspace.CurrentSolution.GetProject projectId with
        | null -> ValueNone
        | project -> ValueSome project

    let projectName projectId =
        match tryProject projectId with
        | ValueSome project -> project.Name
        | ValueNone -> ""

    // Cached because the shell asks for display data once per visible row.
    let projectIcons =
        ConcurrentDictionary<ProjectId, struct (nativeint * uint16) voption>()

    /// Reads the icon the project's own hierarchy hands Solution Explorer. Main thread only, which
    /// is where the shell asks for display data.
    let readProjectIcon (projectId: ProjectId) =
        let tryProperty (hierarchy: IVsHierarchy) (propertyId: __VSHPROPID) =
            let mutable value = null

            if ErrorHandler.Succeeded(hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, int propertyId, &value)) then
                match value with
                | null -> ValueNone
                | value -> ValueSome value
            else
                ValueNone

        match workspace.GetHierarchy projectId with
        | null -> ValueNone
        | hierarchy ->
            match tryProperty hierarchy __VSHPROPID.VSHPROPID_IconImgList, tryProperty hierarchy __VSHPROPID.VSHPROPID_IconIndex with
            | ValueSome imageList, ValueSome iconIndex ->
                try
                    match nativeint (Convert.ToInt64 imageList) with
                    | 0n -> ValueNone
                    | handle -> ValueSome(struct (handle, uint16 (Convert.ToInt32 iconIndex)))
                with _ ->
                    ValueNone
            | _ -> ValueNone

    let tryGetProjectIcon projectId =
        projectIcons.GetOrAdd(projectId, readProjectIcon)

    /// Reference rows keyed by project. The editor's own option cache cannot serve this: it is a
    /// ConditionalWeakTable keyed on the Roslyn `Project` instance, and every workspace change hands
    /// out fresh instances, so reading it from a later solution snapshot always misses.
    let referenceRows = ConcurrentDictionary<ProjectId, ObjectListItem[]>()

    let mutable warmingReferences = 0

    let referenceItemsOf (project: Project) =
        match referenceRows.TryGetValue project.Id with
        | true, rows -> rows
        | _ ->
            match ProjectCache.Projects.TryGetValue project with
            | true, (_, _, _, options) ->
                let rows = ObjectBrowserItems.referenceItemsOfOptions project.Id options
                referenceRows[project.Id] <- rows
                rows
            | _ -> Array.empty

    /// Options are what the editor loads for every keystroke, so this is cheap next to a check.
    /// Options are what the editor loads for every keystroke, so this is cheap next to a check.
    /// `TryGetOptionsByProject` is the no-exception form: a project whose options the project system
    /// has not delivered yet answers ValueNone, where `GetFSharpCompilationOptionsAsync` would raise
    /// `OperationCanceledException` — once per project, per pass, which floods the debugger.
    ///
    /// A project that is not ready is left uncached and retried, because the root list is rebuilt
    /// only when the package counter moves and nothing else would come back for it.
    let warmReferenceOptions (projects: Project[]) =
        let notWarmed () =
            projects
            |> Array.filter (fun project -> not (referenceRows.ContainsKey project.Id))

        if
            not (Array.isEmpty (notWarmed ()))
            && Interlocked.CompareExchange(&warmingReferences, 1, 0) = 0
        then
            backgroundTask {
                try
                    let optionsManager =
                        workspace.Services.GetService<IFSharpWorkspaceService>().FSharpProjectOptionsManager

                    let mutable attempt = 0
                    let mutable pending = notWarmed ()

                    while attempt < 20 && not (Array.isEmpty pending) do
                        let mutable loaded = false

                        for project in pending do
                            match!
                                optionsManager.TryGetOptionsByProject(project, CancellationToken.None)
                                |> Async.StartAsTask
                            with
                            | ValueSome(_, options) ->
                                referenceRows[project.Id] <- ObjectBrowserItems.referenceItemsOfOptions project.Id options
                                loaded <- true
                            | ValueNone -> ()

                        // One refresh per pass; the root list rebuilds on the package counter.
                        if loaded then
                            bumpEverything ()

                        attempt <- attempt + 1
                        pending <- notWarmed ()

                        if not (Array.isEmpty pending) then
                            do! Task.Delay(TimeSpan.FromSeconds 3.)
                finally
                    Volatile.Write(&warmingReferences, 0)
            }
            |> ignore

    let computeSymbols (project: Project) =
        backgroundTask {
            let checker = project.Solution.GetFSharpWorkspaceService().Checker

            let! results =
                cancellableTask {
                    if checker.UsesTransparentCompiler then
                        // The snapshot path shares the cache the editing features populate.
                        let! snapshot = project.GetFSharpProjectSnapshot()
                        return! checker.ParseAndCheckProject(snapshot, userOpName = nameof FSharpObjectBrowserLibrary)
                    else
                        let! _, _, _, (options: FSharpProjectOptions) = project.GetFSharpCompilationOptionsAsync()
                        return! checker.ParseAndCheckProject(options, userOpName = nameof FSharpObjectBrowserLibrary)
                }
                |> CancellableTask.startWithoutCancellation

            return
                {
                    Types = ObjectBrowserItems.typesOfSignature results.AssemblySignature
                    ReferencedAssemblies = results.ProjectContext.GetReferencedAssemblies() |> List.toArray
                }
        }

    /// At most one check per project is in flight. The stale mark is consumed *before* computing,
    /// so an edit arriving mid-check re-marks the project and is picked up by the next check.
    /// A failed check leaves the placeholder in place without bumping the counters — retrying is
    /// driven by the next edit or expansion, not by a refresh loop.
    let startCheck projectId =
        match tryProject projectId with
        | ValueSome project when checking.TryAdd(projectId, true) ->
            backgroundTask {
                try
                    try
                        // A re-check after an edit is delayed a little so a typing burst coalesces
                        // into one ParseAndCheckProject instead of one per keystroke.
                        if symbols.ContainsKey projectId then
                            do! Task.Delay(TimeSpan.FromSeconds 2.)

                        do! checkThrottle.WaitAsync()

                        try
                            stale.TryRemove projectId |> ignore
                            let! computed = computeSymbols project

                            // Keep the result only while the project is still part of the solution;
                            // anything else would resurrect symbols for a project that is gone.
                            if (tryProject projectId).IsSome then
                                symbols[projectId] <- computed
                                bumpContent ()
                        finally
                            checkThrottle.Release() |> ignore
                    // A project whose options are not ready raises this: a "not yet", not a failure,
                    // retried on the next edit or expansion.
                    with
                    | :? OperationCanceledException -> ()
                    | ex -> Trace.TraceError($"F# Object Browser: checking '{projectName projectId}' failed: {ex}")
                finally
                    checking.TryRemove projectId |> ignore
            }
            |> ignore
        | _ -> ()

    let tryGetProjectSymbols projectId =
        match symbols.TryGetValue projectId with
        | true, projectSymbols ->
            if stale.ContainsKey projectId then
                startCheck projectId

            ValueSome projectSymbols
        | _ ->
            startCheck projectId
            ValueNone

    let dropAll () =
        symbols.Clear()
        stale.Clear()
        projectIcons.Clear()
        referenceRows.Clear()
        bumpEverything ()

    let onWorkspaceChanged (args: WorkspaceChangeEventArgs) =
        match args.Kind with
        | WorkspaceChangeKind.DocumentChanged
        | WorkspaceChangeKind.DocumentAdded
        | WorkspaceChangeKind.DocumentRemoved ->
            match args.ProjectId with
            | null -> ()
            // Only the first edit of a burst moves the counters; the rest ride the check already queued.
            | projectId when stale.TryAdd(projectId, true) -> bumpContent ()
            | _ -> ()
        // `ProjectChanged` fires for anything inside a project and, on a large solution, arrives in
        // the hundreds. The root list rides the package counter and the shell throws the whole tree
        // away whenever it moves, so only events that actually change the set of root nodes may
        // touch it; everything else refreshes content and leaves the tree standing.
        | WorkspaceChangeKind.ProjectChanged ->
            match args.ProjectId with
            | null -> ()
            | projectId ->
                symbols.TryRemove projectId |> ignore
                bumpContent ()
        | WorkspaceChangeKind.ProjectAdded
        | WorkspaceChangeKind.ProjectReloaded
        | WorkspaceChangeKind.ProjectRemoved ->
            match args.ProjectId with
            | null -> dropAll ()
            | projectId ->
                symbols.TryRemove projectId |> ignore
                stale.TryRemove projectId |> ignore
                projectIcons.TryRemove projectId |> ignore
                referenceRows.TryRemove projectId |> ignore
                bumpEverything ()
        // Only a solution actually going away invalidates everything. `SolutionChanged` fires for
        // ordinary solution-level churn — during load and restore it fires repeatedly, and treating
        // it as a reset threw away every check that was in flight.
        | WorkspaceChangeKind.SolutionCleared
        | WorkspaceChangeKind.SolutionReloaded
        | WorkspaceChangeKind.SolutionRemoved -> dropAll ()
        | WorkspaceChangeKind.SolutionAdded
        | WorkspaceChangeKind.SolutionChanged -> bumpEverything ()
        | _ -> ()

    let subscription = workspace.WorkspaceChanged.Subscribe onWorkspaceChanged

    // Navigation

    let tryEnclosingEntity (symbol: FSharpSymbol) =
        match symbol with
        | :? FSharpEntity as entity -> ValueSome entity
        | :? FSharpUnionCase as case -> ValueSome case.DeclaringEntity
        | :? FSharpMemberOrFunctionOrValue as value ->
            match value.ApparentEnclosingEntity with
            | Some entity -> ValueSome entity
            | None -> ValueNone
        | :? FSharpField as field ->
            match field.DeclaringEntity with
            | Some entity -> ValueSome entity
            | None -> ValueNone
        | _ -> ValueNone

    let showMetadata (item: ObjectListItem) (symbol: FSharpSymbol) =
        match tryEnclosingEntity symbol with
        | ValueNone -> ()
        | ValueSome entity ->

            match entity.TryGetMetadataText() with
            | None -> ()
            | Some text ->
                let metadataAsSource =
                    workspace.Services.GetService<IFSharpWorkspaceService>().MetadataAsSource

                let references =
                    match tryProject item.ProjectId with
                    | ValueSome project -> project.MetadataReferences :> seq<_>
                    | ValueNone -> Seq.empty

                let projectInfo, documentInfo =
                    MetadataAsSource.generateTemporaryDocument (
                        AssemblyIdentity(entity.Assembly.QualifiedName),
                        entity.DisplayName,
                        references
                    )

                metadataAsSource.ShowDocument(projectInfo, documentInfo.FilePath, SourceText.From(text.ToString()))
                |> ignore

    let goToSource (item: ObjectListItem) =
        match ObjectBrowserItems.trySymbol item with
        | ValueNone -> ()
        | ValueSome symbol ->
            ThreadHelper.JoinableTaskFactory.RunAsync(fun () ->
                task {
                    do! ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync()

                    try
                        let inSource =
                            match symbol.DeclarationLocation with
                            | None -> ValueNone
                            | Some range ->
                                match workspace.CurrentSolution.TryGetDocumentFromFSharpRange(range, item.ProjectId) with
                                | Some document -> ValueSome(document, range)
                                | None -> ValueNone

                        match inSource with
                        | ValueNone -> showMetadata item symbol
                        | ValueSome(document, range) ->
                            let! text = document.GetTextAsync CancellationToken.None

                            match RoslynHelpers.TryFSharpRangeToTextSpan(text, range) with
                            | ValueNone -> ()
                            | ValueSome span ->
                                workspace.Services
                                    .GetService<IFSharpDocumentNavigationService>()
                                    .TryNavigateToSpan(workspace, document.Id, span, CancellationToken.None)
                                |> ignore
                    with _ ->
                        ()
                }
                :> Task)
            |> ignore

    // Search

    /// ValueNone while the project is still being checked, so the caller can say so rather than
    /// reporting "no matches" for a project it has not looked at yet.
    let searchProject projectId childKind searchText =
        match tryGetProjectSymbols projectId with
        | ValueNone -> ValueNone
        | ValueSome projectSymbols ->
            let library = projectName projectId

            let candidates =
                match childKind with
                | ObjectListKind.Namespaces -> ObjectBrowserItems.namespaceItems projectId library projectSymbols.Types
                | ObjectListKind.Members ->
                    projectSymbols.Types
                    |> Array.collect (ObjectBrowserItems.searchMemberItems projectId library searchText)
                | _ -> ObjectBrowserItems.typeItems projectId library projectSymbols.Types

            ValueSome(candidates |> Array.filter (ObjectBrowserItems.matchesSearch searchText))

    let search childKind (projectIds: ProjectId[]) (searchText: string) =
        if String.IsNullOrEmpty searchText || Array.isEmpty projectIds then
            Array.empty
        else
            let results =
                projectIds
                |> Array.map (fun projectId -> searchProject projectId childKind searchText)

            [|
                if results |> Array.exists _.IsNone then
                    ObjectBrowserItems.pendingItem projectIds[0]

                for result in results do
                    match result with
                    | ValueSome matches -> yield! matches
                    | ValueNone -> ()
            |]

    // Root list

    let fsharpProjects () =
        workspace.CurrentSolution.Projects
        |> Seq.filter (fun project -> project.IsFSharp && not project.IsFSharpMiscellaneousOrMetadata)
        |> Seq.sortBy _.Name
        |> Seq.toArray

    let rootItems flags =
        let projects = fsharpProjects ()
        warmReferenceOptions projects

        [|
            for project in projects do
                ObjectBrowserItems.projectItem project.Id project.Name

            // Object Browser lists referenced assemblies at the top level; Class View nests them
            // under the project that owns them instead. Metadata references make this instant -
            // no check is involved until a reference node is expanded.
            if LibraryList.isObjectBrowser flags then
                let seen = HashSet<string>(StringComparer.OrdinalIgnoreCase)

                for project in projects do
                    for item in referenceItemsOf project do
                        if seen.Add item.DisplayText then
                            item
        |]

    let supportedCategoryFields category =
        if category = int LIB_CATEGORY.LC_MEMBERTYPE then
            ValueSome(
                uint32 _LIBCAT_MEMBERTYPE.LCMT_METHOD
                ||| uint32 _LIBCAT_MEMBERTYPE.LCMT_PROPERTY
                ||| uint32 _LIBCAT_MEMBERTYPE.LCMT_EVENT
                ||| uint32 _LIBCAT_MEMBERTYPE.LCMT_FIELD
                ||| uint32 _LIBCAT_MEMBERTYPE.LCMT_CONSTANT
                ||| uint32 _LIBCAT_MEMBERTYPE.LCMT_ENUMITEM
                ||| uint32 _LIBCAT_MEMBERTYPE.LCMT_OPERATOR
            )
        elif
            category = int LIB_CATEGORY.LC_MEMBERACCESS
            || category = int LIB_CATEGORY.LC_CLASSACCESS
        then
            ValueSome(
                uint32 _LIBCAT_MEMBERACCESS.LCMA_PUBLIC
                ||| uint32 _LIBCAT_MEMBERACCESS.LCMA_PRIVATE
                ||| uint32 _LIBCAT_MEMBERACCESS.LCMA_PROTECTED
                ||| uint32 _LIBCAT_MEMBERACCESS.LCMA_PACKAGE
            )
        elif category = int _LIB_CATEGORY2.LC_MEMBERINHERITANCE then
            ValueSome(
                uint32 _LIBCAT_MEMBERINHERITANCE.LCMI_IMMEDIATE
                ||| uint32 _LIBCAT_MEMBERINHERITANCE.LCMI_INHERITED
            )
        elif category = int LIB_CATEGORY.LC_CLASSTYPE then
            ValueSome(
                uint32 _LIBCAT_CLASSTYPE.LCCT_NSPC
                ||| uint32 _LIBCAT_CLASSTYPE.LCCT_CLASS
                ||| uint32 _LIBCAT_CLASSTYPE.LCCT_INTERFACE
                ||| uint32 _LIBCAT_CLASSTYPE.LCCT_STRUCT
                ||| uint32 _LIBCAT_CLASSTYPE.LCCT_ENUM
                ||| uint32 _LIBCAT_CLASSTYPE.LCCT_DELEGATE
                ||| uint32 _LIBCAT_CLASSTYPE.LCCT_MODULE
                ||| uint32 _LIBCAT_CLASSTYPE.LCCT_EXCEPTION
            )
        elif category = int LIB_CATEGORY.LC_ACTIVEPROJECT then
            ValueSome(uint32 _LIBCAT_ACTIVEPROJECT.LCAP_SHOWALWAYS)
        elif category = int LIB_CATEGORY.LC_LISTTYPE then
            ValueSome(
                uint32 _LIB_LISTTYPE.LLT_CLASSES
                ||| uint32 _LIB_LISTTYPE.LLT_NAMESPACES
                ||| uint32 _LIB_LISTTYPE.LLT_MEMBERS
                ||| uint32 _LIB_LISTTYPE.LLT_HIERARCHY
                ||| uint32 _LIB_LISTTYPE.LLT_PACKAGE
            )
        elif category = int LIB_CATEGORY.LC_VISIBILITY then
            ValueSome(uint32 _LIBCAT_VISIBILITY.LCV_VISIBLE ||| uint32 _LIBCAT_VISIBILITY.LCV_HIDDEN)
        elif category = int _LIB_CATEGORY2.LC_HIERARCHYTYPE then
            ValueSome(
                uint32 _LIBCAT_HIERARCHYTYPE.LCHT_PROJECTREFERENCES
                ||| uint32 _LIBCAT_HIERARCHYTYPE.LCHT_BASESANDINTERFACES
                ||| uint32 _LIBCAT_HIERARCHYTYPE.LCHT_UNKNOWN
            )
        elif category = int _LIB_CATEGORY2.LC_PHYSICALCONTAINERTYPE then
            ValueSome(
                uint32 _LIBCAT_PHYSICALCONTAINERTYPE.LCPT_PROJECT
                ||| uint32 _LIBCAT_PHYSICALCONTAINERTYPE.LCPT_PROJECTREFERENCE
            )
        else
            ValueNone

    let createNavInfo (nodes: SYMBOL_DESCRIPTION_NODE[]) count =
        if count = 0 || nodes[0].dwType <> uint32 _LIB_LISTTYPE.LLT_PACKAGE then
            ValueNone
        else
            let referenceOwner, library, first =
                if count > 1 && nodes[1].dwType = uint32 _LIB_LISTTYPE.LLT_PACKAGE then
                    ValueSome nodes[0].pszName, nodes[1].pszName, 2
                else
                    ValueNone, nodes[0].pszName, 1

            let namespaceParts = ResizeArray()
            let classParts = ResizeArray()
            let mutable memberName = ValueNone

            for i in first .. count - 1 do
                let node = nodes[i]

                if node.dwType = uint32 _LIB_LISTTYPE.LLT_NAMESPACES then
                    namespaceParts.Add node.pszName
                elif node.dwType = uint32 _LIB_LISTTYPE.LLT_CLASSES then
                    classParts.Add node.pszName
                elif node.dwType = uint32 _LIB_LISTTYPE.LLT_MEMBERS then
                    memberName <- ValueSome node.pszName

            let join (parts: ResizeArray<string>) =
                if parts.Count = 0 then
                    ValueNone
                else
                    ValueSome(String.Join(".", parts))

            ValueSome(navInfoFactory.Create(library, referenceOwner, join namespaceParts, join classParts, memberName))

    interface IDisposable with
        member _.Dispose() =
            subscription.Dispose()
            checkThrottle.Dispose()

    interface IVsCoTaskMemFreeMyStrings

    interface IObjectBrowserHost with

        member _.NavInfoFactory = navInfoFactory

        member _.CommandTarget = this :> IOleCommandTarget

        member _.UpdateCounter listKind =
            lock counterGate (fun () ->
                match listKind with
                | ObjectListKind.Projects
                | ObjectListKind.References -> packageVersion
                | _ -> contentVersion)

        member _.ProjectName projectId = projectName projectId

        member _.TryGetProjectIcon projectId = tryGetProjectIcon projectId

        member _.TryGetProjectSymbols projectId = tryGetProjectSymbols projectId

        member _.ReferenceItems projectId =
            match tryProject projectId with
            | ValueSome project ->
                warmReferenceOptions [| project |]
                referenceItemsOf project
            | ValueNone -> Array.empty

        member _.GoToSource item = goToSource item

        member _.FillDescription(item, _options, description) =
            try
                ObjectBrowserDescription.fill navInfoFactory documentationBuilder.Value item description
                true
            with _ ->
                false

        member _.Search(childKind, item, searchText) =
            search childKind [| item.ProjectId |] searchText

    interface IVsSimpleLibrary2 with

        member _.GetGuid(pguidLib: byref<Guid>) =
            pguidLib <- libraryGuid
            VSConstants.S_OK

        member _.GetLibFlags2(pgrfFlags: byref<uint32>) =
            pgrfFlags <-
                uint32 _LIB_FLAGS.LF_PROJECT
                ||| uint32 _LIB_FLAGS.LF_EXPANDABLE
                ||| uint32 _LIB_FLAGS2.LF_SUPPORTSFILTERING
                ||| uint32 _LIB_FLAGS2.LF_SUPPORTSBASETYPES
                ||| uint32 _LIB_FLAGS2.LF_SUPPORTSINHERITEDMEMBERS
                ||| uint32 _LIB_FLAGS2.LF_SUPPORTSPRIVATEMEMBERS
                ||| uint32 _LIB_FLAGS2.LF_SUPPORTSPROJECTREFERENCES

            VSConstants.S_OK

        member _.GetSupportedCategoryFields2(category, pgrfCatField: byref<uint32>) =
            match supportedCategoryFields category with
            | ValueSome fields ->
                pgrfCatField <- fields
                VSConstants.S_OK
            | ValueNone ->
                pgrfCatField <- 0u
                VSConstants.E_NOTIMPL

        member _.GetSeparatorStringWithOwnership(pbstrSeparator: byref<string>) =
            pbstrSeparator <- "."
            VSConstants.S_OK

        member _.UpdateCounter(pCurUpdate: byref<uint32>) =
            pCurUpdate <- lock counterGate (fun () -> packageVersion)
            VSConstants.S_OK

        member _.GetList2(listType, flags, pobSrch: VSOBSEARCHCRITERIA2[], ppIVsSimpleObjectList2: byref<IVsSimpleObjectList2>) =
            let listKind = LibraryList.ofListType listType

            let produce =
                if LibraryList.isFindSymbol flags then
                    let searchText = LibraryList.searchTextOf pobSrch
                    ValueSome(fun () -> search listKind (fsharpProjects () |> Array.map _.Id) searchText)
                elif listKind.IsProjects then
                    ValueSome(fun () -> rootItems flags)
                else
                    ValueNone

            match produce with
            | ValueSome produce ->
                ppIVsSimpleObjectList2 <- FSharpObjectList(listKind, flags, this :> IObjectBrowserHost, ValueNone, ValueNone, produce)

                VSConstants.S_OK
            | ValueNone ->
                ppIVsSimpleObjectList2 <- null
                VSConstants.E_NOTIMPL

        member _.CreateNavInfo(rgSymbolNodes: SYMBOL_DESCRIPTION_NODE[], ulcNodes, ppNavInfo: byref<IVsNavInfo>) =
            match createNavInfo rgSymbolNodes (int ulcNodes) with
            | ValueSome navInfo ->
                ppNavInfo <- navInfo
                VSConstants.S_OK
            | ValueNone ->
                ppNavInfo <- null
                VSConstants.E_INVALIDARG

        member _.LoadState(_pIStream, _lptType) = VSConstants.S_OK

        member _.SaveState(_pIStream, _lptType) = VSConstants.S_OK

        member _.GetBrowseContainersForHierarchy(_pHierarchy, _celt, _rgBrowseContainers, _pcActual) = VSConstants.E_NOTIMPL

        member _.AddBrowseContainer(_pcdComponent, pgrfOptions: byref<uint32>, pbstrComponentAdded: byref<string>) =
            pgrfOptions <- 0u
            pbstrComponentAdded <- null
            VSConstants.E_NOTIMPL

        member _.RemoveBrowseContainer(_dwReserved, _pszLibName) = VSConstants.E_NOTIMPL

    // The shell still Queries for the pre-`Simple` library interfaces on some code paths. Roslyn
    // answers them with stubs so the QueryInterface succeeds; without that the Object Browser keeps
    // the library at arm's length and never asks a node for its children.
    interface IVsLibrary2 with

        member _.GetSupportedCategoryFields2(_category, pgrfCatField: byref<uint32>) =
            pgrfCatField <- 0u
            VSConstants.E_NOTIMPL

        member _.GetList2(_listType, _flags, _pobSrch, ppIVsObjectList2: byref<IVsObjectList2>) =
            ppIVsObjectList2 <- null
            VSConstants.E_NOTIMPL

        member _.GetLibList(_lptType, ppList: byref<IVsLiteTreeList>) =
            ppList <- null
            VSConstants.E_NOTIMPL

        member _.GetLibFlags2(pgrfFlags: byref<uint32>) =
            pgrfFlags <- 0u
            VSConstants.E_NOTIMPL

        member _.UpdateCounter(pCurUpdate: byref<uint32>) =
            pCurUpdate <- 0u
            VSConstants.E_NOTIMPL

        member _.GetGuid(ppguidLib: byref<nativeint>) =
            ppguidLib <- IntPtr.Zero
            VSConstants.E_NOTIMPL

        member _.GetSeparatorString(_pszSeparator: nativeint) = VSConstants.E_NOTIMPL

        member _.LoadState(_pIStream, _lptType) = VSConstants.E_NOTIMPL

        member _.SaveState(_pIStream, _lptType) = VSConstants.E_NOTIMPL

        member _.GetBrowseContainersForHierarchy(_pHierarchy, _celt, _rgBrowseContainers, _pcActual) = VSConstants.E_NOTIMPL

        member _.AddBrowseContainer(_pcdComponent, pgrfOptions: byref<uint32>, _pbstrComponentAdded: string[]) =
            pgrfOptions <- 0u
            VSConstants.E_NOTIMPL

        member _.RemoveBrowseContainer(_dwReserved, _pszLibName) = VSConstants.E_NOTIMPL

        member _.CreateNavInfo(_rgSymbolNodes, _ulcNodes, ppNavInfo: byref<IVsNavInfo>) =
            ppNavInfo <- null
            VSConstants.E_NOTIMPL

    interface IVsLibrary with

        member _.GetSupportedCategoryFields(_category, pCatField: byref<uint32>) =
            pCatField <- 0u
            VSConstants.E_NOTIMPL

        member _.GetList(_listType, _flags, _pobSrch, pplist: byref<IVsObjectList>) =
            pplist <- null
            VSConstants.E_NOTIMPL

        member _.GetLibList(_lptType, pplist: byref<IVsLiteTreeList>) =
            pplist <- null
            VSConstants.E_NOTIMPL

        member _.GetLibFlags(pfFlags: byref<uint32>) =
            pfFlags <- 0u
            VSConstants.E_NOTIMPL

        member _.UpdateCounter(pCurUpdate: byref<uint32>) =
            pCurUpdate <- 0u
            VSConstants.E_NOTIMPL

        member _.GetGuid(ppguidLib: byref<Guid>) =
            ppguidLib <- libraryGuid
            VSConstants.S_OK

        member _.GetSeparatorString(_pszSeparator: string[]) = VSConstants.E_NOTIMPL

        member _.LoadState(_pIStream, _lptType) = VSConstants.E_NOTIMPL

        member _.SaveState(_pIStream, _lptType) = VSConstants.E_NOTIMPL

        member _.GetBrowseContainersForHierarchy(_pHierarchy, _celt, _rgBrowseContainers, _pcActual) = VSConstants.E_NOTIMPL

        member _.AddBrowseContainer(_pcdComponent, pgrfOptions: byref<uint32>, pbstrComponentAdded: byref<string>) =
            pgrfOptions <- 0u
            pbstrComponentAdded <- null
            VSConstants.E_NOTIMPL

        member _.RemoveBrowseContainer(_dwReserved, _pszLibName) = VSConstants.E_NOTIMPL

    /// One library, and it is this one.
    interface IVsLibraryMgr with

        member _.GetCount(pnCount: byref<uint32>) =
            pnCount <- 1u
            VSConstants.S_OK

        member _.GetLibraryAt(nLibIndex, ppLibrary: byref<IVsLibrary>) =
            if nLibIndex = 0u then
                ppLibrary <- this
                VSConstants.S_OK
            else
                ppLibrary <- null
                VSConstants.E_INVALIDARG

        member _.GetNameAt(_nLibIndex, _pszName) = VSConstants.E_NOTIMPL

        member _.ToggleCheckAt(_nLibIndex) = VSConstants.E_NOTIMPL

        member _.GetCheckAt(_nLibIndex, _pstate) = VSConstants.E_NOTIMPL

        member _.SetLibraryGroupEnabled(_lpt, _fEnable) = VSConstants.E_NOTIMPL

    /// The shell owns the Class View context menus; we only have to be a target it can route through.
    interface IOleCommandTarget with

        member _.QueryStatus(_pguidCmdGroup: byref<Guid>, _cCmds, _prgCmds: OLECMD[], _pCmdText) =
            int Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED

        member _.Exec(_pguidCmdGroup: byref<Guid>, _nCmdID, _nCmdexecopt, _pvaIn, _pvaOut) =
            int Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED
