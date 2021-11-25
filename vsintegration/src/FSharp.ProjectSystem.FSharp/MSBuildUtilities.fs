// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.ProjectSystem

open Microsoft.Build.Construction
open Microsoft.Build.Evaluation
open Microsoft.VisualStudio.FSharp.ProjectSystem
open Microsoft.VisualStudio.Shell
open System.Xml
open System.Collections.Generic 
open System.Diagnostics
open System.IO
open System

// The Dev9 MSBuild OM is not very good for re-ordering items.  This class
// abstracts over some MSBuild-maniupulation intentions.  The implementation
// will differ from Dev9 to Dev10.  For Dev9, use reflection to access internal
// members that make the reordering tasks much easier to implement.
// (The Dev9 support has since been cut, so this abstraction layer is now a legacy artifact.)
type internal MSBuildUtilities() =


    static let EnumerateItems(big : ProjectItemGroupElement) =
        big.Items

    static let GetItemType(item : ProjectItemElement) =
        item.ItemType

    /// Normalize path directory separator characters to the OS defacto
    static let NormalizePath(path : string) =
        path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
    
    // Gets the <... Include="path"> path for this item, except if the item is a link, then
    // gets the <Link>path</Link> value instead.
    // In other words, gets the location that will be displayed in the solution explorer.
    static let GetInclude(item : ProjectItem) =
        let strPath = item.GetMetadataValue(ProjectFileConstants.Link)
        if String.IsNullOrEmpty(strPath) then
            item.EvaluatedInclude
        else
            strPath

    static let GetUnescapedUnevaluatedInclude(item : ProjectItemElement) =
        let mutable foundLink = None
        for m in item.Metadata do
            // REVIEW: this ignores Condition, doesn't seem reasonable to do anything else
            if m.Name = ProjectFileConstants.Link && not(String.IsNullOrEmpty(m.Value)) then
                foundLink <- Some(m.Value)
        let escaped =
            match foundLink with
            | None -> item.Include
            | Some(link) -> link
        ProjectCollection.Unescape(escaped) |> NormalizePath

    static let MattersForOrdering(bi : ProjectItemElement) =
        not (bi.ItemType = ProjectFileConstants.ProjectReference || bi.ItemType = ProjectFileConstants.Reference)

    // if 'path' is as in <... Include="path">, determine the relative path of the folder that contains this
    static let ComputeFolder(path : string, projectUrl : Url) =
        Path.GetDirectoryName(PackageUtilities.MakeRelativeIfRooted(path, projectUrl)) + Path.DirectorySeparatorChar.ToString()

    static let FolderComparer = StringComparer.OrdinalIgnoreCase

    static let Same(x : ProjectItemElement, y : ProjectItemElement) =
        Object.ReferenceEquals(x,y)

    // Does two things:
    //  - Gets rid of unnecessary <Folder>s (e.g. non-empty ones - only empty <Folder>s need explicit representation in project file
    //  - Throws if the project cannot be rendered in the solution explorer (e.g. A\foo.fs, B\bar.fs, A\qux.fs would make folder 'A' get rendered twice -> illegal) and throwIfCannotRender=true
    static let EnsureProperFolderLogic (_msbuildProject : Project) (big : ProjectItemGroupElement) (projectNode : ProjectNode) throwIfCannotRender =
        let pathsComparer = { new IEqualityComparer<List<string>> with
                                    member this.Equals(x, y) =
                                        if x.Count <> y.Count then
                                            false
                                        else
                                            let mutable result = true
                                            for i in 0 .. x.Count - 1 do
                                                if FolderComparer.Compare(x.[i], y.[i]) <> 0 then
                                                    result <- false
                                            result
                                    member this.GetHashCode(x) =
                                        if x.Count = 0 then 
                                            0
                                        else
                                            x.[x.Count - 1].GetHashCode() }
        let explicitFolders = new Dictionary<List<string>, (ProjectItemElement * int)>( pathsComparer )   // int is number of items under folder
        let Inc( pathParts : List<string> ) =
            if explicitFolders.ContainsKey(pathParts) then
                let bi, oldCount = explicitFolders.[pathParts]
                explicitFolders.[pathParts] <- (bi, oldCount + 1)

        let alreadyRenderedFolders = new HashSet<List<string>>( pathsComparer)

        let IsPrefix(x : List<string>, y : List<string>) =
            if x.Count > y.Count then
                false
            elif x.Count = 0 then
                true
            else
                let mutable result = true
                for i in 0 .. x.Count - 1 do
                    if FolderComparer.Compare(x.[i], y.[i]) <> 0 then
                        result <- false
                result
        
        let SameList(x : List<string>, y : List<string>) =
            if x.Count <> y.Count then
                false
            else
                let mutable r = true
                for i in 0 .. x.Count - 1 do
                    if x.[i] <> y.[i] then
                        r <- false
                r
        
        let mutable curPathParts = new List<string>()
        for bi in EnumerateItems(big) do
            let path = ComputeFolder(GetUnescapedUnevaluatedInclude(bi), projectNode.BaseURI)
            let pathParts = new List<_>(path.Split( [| '\\' |], StringSplitOptions.RemoveEmptyEntries))
            while not( IsPrefix(curPathParts, pathParts) ) do
                // pop folder
                curPathParts.RemoveAt(curPathParts.Count - 1)   // e.g. transition from A\B\C\foo.fs to A\
            while not(SameList(curPathParts, pathParts)) do
                // push folder
                Inc(curPathParts)
                curPathParts.Add(pathParts.[curPathParts.Count]) // e.g. transition from A\ to A\D\E\bar.fs
                if not(alreadyRenderedFolders.Add(new List<string>(curPathParts))) && throwIfCannotRender then
                    raise <| new InvalidOperationException(String.Format(FSharpSR.ProjectRenderFolderMultiple(), projectNode.ProjectFile, bi.Include))
            Inc(curPathParts)
            if bi.ItemType = ProjectFileConstants.Folder then
                explicitFolders.Add(new List<_>(pathParts), (bi,0))

        // remove non-empty folders
        for kvp in explicitFolders do
            let bi, count = kvp.Value
            if count <> 0 then
                big.RemoveChild(bi)
                                        
        big

    /// Partition items into an <ItemGroup> that matters for ordering and one that doesn't.  Return the former.
    /// Throw if cannot render.
    static let Rearrange(msbuildProject : Project, projectNode : ProjectNode) =
        let newBig = msbuildProject.Xml.CreateItemGroupElement()    // all the items we care about
        let otherBig = msbuildProject.Xml.CreateItemGroupElement()  // items that don't matter for ordering
        let mutable otherBigHasItems = false
        let mutable otherBis = []
        let mutable newBis = []
        for big in msbuildProject.Xml.ItemGroups do
            for bi in big.Items do
                big.RemoveChild(bi)
                if not( MattersForOrdering(bi) ) then
                    otherBis <- bi :: otherBis
                    otherBigHasItems <- true
                elif bi.ItemType <> ProjectFileConstants.Folder then // we handle folders our own way
                    newBis <- bi :: newBis
            msbuildProject.Xml.RemoveChild(big)
        msbuildProject.Xml.AppendChild(newBig)
        if otherBigHasItems then
            msbuildProject.Xml.AppendChild(otherBig)
        for i in List.rev newBis do
            newBig.AppendChild(i)
        for i in List.rev otherBis do
            otherBig.AppendChild(i)
        EnsureProperFolderLogic msbuildProject newBig projectNode true
            
    /// If necessary, partition items into an <ItemGroup> that matters for ordering and one that doesn't.  Return the former.
    /// Throw if cannot render and throwIfCannotRender=true.
    static let EnsureValid (msbuildProject : Project) (projectNode : ProjectNode) (throwIfCannotRender:bool) =
        let mutable priorGroupWithAtLeastOneItemThatMattersForOrdering = None
        let mutable needToRearrange = false
        let mutable bigsToRemove = []
        for big in msbuildProject.Xml.ItemGroups do
                let mutable thisGroupHasAtLeastOneItemThatMattersForOrdering = false
                let mutable thisGroupHasAtLeastOneItemThatDoesNotMatterForOrdering = false
                for bi in EnumerateItems(big) do
                    if MattersForOrdering(bi) then
                        thisGroupHasAtLeastOneItemThatMattersForOrdering <- true
                    else
                        thisGroupHasAtLeastOneItemThatDoesNotMatterForOrdering <- true
                if thisGroupHasAtLeastOneItemThatMattersForOrdering && thisGroupHasAtLeastOneItemThatDoesNotMatterForOrdering then
                    needToRearrange <- true  // cannot be mixed
                elif thisGroupHasAtLeastOneItemThatMattersForOrdering then
                    if priorGroupWithAtLeastOneItemThatMattersForOrdering.IsSome then
                        needToRearrange <- true
                    else
                        priorGroupWithAtLeastOneItemThatMattersForOrdering <- Some(big)
                elif not( thisGroupHasAtLeastOneItemThatDoesNotMatterForOrdering ) then
                    bigsToRemove <- big :: bigsToRemove  // no items at all
        for big in bigsToRemove do
            msbuildProject.Xml.RemoveChild(big)
        if needToRearrange then
            Rearrange(msbuildProject, projectNode)
        else
            match priorGroupWithAtLeastOneItemThatMattersForOrdering with
            | Some(g) -> EnsureProperFolderLogic msbuildProject g projectNode throwIfCannotRender
            | None -> msbuildProject.Xml.AddItemGroup()
            
    static member ThrowIfNotValidAndRearrangeIfNecessary (projectNode : ProjectNode) =
        EnsureValid projectNode.BuildProject projectNode true |> ignore
        
    static member private MoveFileAboveHelper(item : ProjectItemElement, itemToMoveAbove : ProjectItemElement, big : ProjectItemGroupElement, _projectNode : ProjectNode) =  
        // TODO wildcards?
        big.RemoveChild(item)
        big.InsertBeforeChild(item, itemToMoveAbove)
        
    static member private MoveFileBelowHelper(item : ProjectItemElement, itemToMoveBelow : ProjectItemElement, big : ProjectItemGroupElement, _projectNode : ProjectNode) =  
        // TODO wildcards?
        big.RemoveChild(item)
        big.InsertAfterChild(item, itemToMoveBelow)
        
    /// Move a file node to its correct place in the build project.
    /// Its correct place is defined by where it sits in the hierarchy relative
    /// to other files.
    static member SyncWithHierarchy(fileNode : FileNode) =
        let projectNode = fileNode.ProjectMgr
        let msbuildProject = projectNode.BuildProject
        let big = EnsureValid msbuildProject projectNode false
        
        let itemToMove = fileNode.ItemNode.Item.Xml
        big.RemoveChild itemToMove
            
        let precedingFile =
            projectNode.AllDescendants
            |> Seq.choose (function :? FileNode as fileNode -> Some fileNode | _ -> None)
            |> Seq.takeWhile ((<>) fileNode)
            |> Seq.tryLast
            
        match precedingFile with
        | None ->
            // if there is no preceding file, it must be because we're
            // the first file in the hierarchy - so put us first
            big.PrependChild itemToMove
        | Some precedingFile ->
            big.InsertAfterChild (itemToMove, precedingFile.ItemNode.Item.Xml)
        
        // The project file should now be in a valid state
        MSBuildUtilities.ThrowIfNotValidAndRearrangeIfNecessary projectNode
        
    /// Given a HierarchyNode, compute the last BuildItem if we want to move something after it
    static member private FindLast(toMoveAfter : HierarchyNode, projectNode : ProjectNode) =
        match toMoveAfter with
        | :? FileNode -> toMoveAfter.ItemNode.Item.Xml
        | :? FolderNode -> 
            // find the last item in this folder
            let folder = ComputeFolder(toMoveAfter.Url, projectNode.BaseURI)
            let msbuildProject = projectNode.BuildProject
            let big = EnsureValid msbuildProject projectNode true
            let mutable result = null
            for bi in EnumerateItems(big) do
                let curFolder = ComputeFolder(GetUnescapedUnevaluatedInclude(bi), projectNode.BaseURI)
                if curFolder.StartsWith(folder, StringComparison.OrdinalIgnoreCase) then
                    result <- bi
            Debug.Assert(result <> null, "did not find item corresponding to " + toMoveAfter.Caption)
            result
        | _ -> Debug.Assert(false, "should not ever get here"); failwith "Could not move down"

    /// Given a HierarchyNode, compute the first BuildItem if we want to move something before it
    static member private FindFirst(toMoveBefore : HierarchyNode, projectNode : ProjectNode) =
        if toMoveBefore.ItemNode.Item <> null then
            toMoveBefore.ItemNode.Item.Xml
        else
            match toMoveBefore with
            | :? FolderNode -> ()
            | _ -> Debug.Assert(false, "something is wrong, virtual non-folder?")
            // find the first item in this folder
            let folder = ComputeFolder(toMoveBefore.Url, projectNode.BaseURI)
            let msbuildProject = projectNode.BuildProject
            let big = EnsureValid msbuildProject projectNode true
            let mutable result = null
            for bi in EnumerateItems(big) do
                let curFolder = ComputeFolder(GetUnescapedUnevaluatedInclude(bi), projectNode.BaseURI)
                if result = null && curFolder.StartsWith(folder, StringComparison.OrdinalIgnoreCase) then
                    result <- bi
            Debug.Assert(result <> null, "did not find item corresponding to " + toMoveBefore.Caption)
            result

    static member MoveFileUp(toMove : HierarchyNode, toMoveBefore : HierarchyNode, projectNode : ProjectNode) =
        Debug.Assert(toMove.ItemNode.Item <> null, "something is wrong - virtual file?")
        let itemToMove = toMove.ItemNode.Item
        let itemToMoveBefore = MSBuildUtilities.FindFirst(toMoveBefore, projectNode)
        // TODO EnsureValid call below is extra work
        MSBuildUtilities.MoveFileAboveHelper(itemToMove.Xml, itemToMoveBefore, (EnsureValid projectNode.BuildProject projectNode true), projectNode)

    static member MoveFileDown(toMove : HierarchyNode, toMoveAfter : HierarchyNode, projectNode : ProjectNode) =
        Debug.Assert(toMove.ItemNode.Item <> null, "something is wrong - virtual file?")
        let itemToMove = toMove.ItemNode.Item
        let itemToMoveAfter = MSBuildUtilities.FindLast(toMoveAfter, projectNode)
        // TODO EnsureValid call below is extra work
        MSBuildUtilities.MoveFileBelowHelper(itemToMove.Xml, itemToMoveAfter, (EnsureValid projectNode.BuildProject projectNode true), projectNode)

    /// Move <Folder> and all its subitems up one "solution hierarchy" slot in the list of items.
    static member private MoveFolderUpHelper(folderToBeMoved : string, itemToMoveBefore : ProjectItemElement, projectNode : ProjectNode) =
        let msbuildProject = projectNode.BuildProject
        let big = EnsureValid msbuildProject projectNode true
        let mutable index = 0
        let mutable itemToMoveBeforeIndex = -1
        let itemsToMove = 
            [for bi in EnumerateItems(big) do
                let curFolder = ComputeFolder(GetUnescapedUnevaluatedInclude(bi), projectNode.BaseURI)
                if curFolder.StartsWith(folderToBeMoved, StringComparison.OrdinalIgnoreCase) then
                    yield (bi, index)
                if Same(bi, itemToMoveBefore) then
                    itemToMoveBeforeIndex <- index
                index <- index + 1]
        if itemToMoveBeforeIndex = -1 then
            Debug.Assert(false, sprintf "did not find item to move before <%s Include=\"%s\">" (GetItemType itemToMoveBefore) (GetUnescapedUnevaluatedInclude itemToMoveBefore))
        for (item,i) in itemsToMove do
            Debug.Assert(i <> 0, "item is already at top")
            Debug.Assert(itemToMoveBeforeIndex < i, "not moving up")
            big.RemoveChild(item)
            big.InsertBeforeChild(item, itemToMoveBefore)

    static member MoveFolderUp(toMove : HierarchyNode, toMoveBefore : HierarchyNode, projectNode : ProjectNode) =
        let folderToBeMoved = ComputeFolder(toMove.Url, projectNode.BaseURI)
        let itemToMoveBefore = MSBuildUtilities.FindFirst(toMoveBefore, projectNode)
        MSBuildUtilities.MoveFolderUpHelper(folderToBeMoved, itemToMoveBefore, projectNode)

    /// Move <Folder> and all its subitems down one "solution hierarchy" slot in the list of items.
    static member private MoveFolderDownHelper(folderToBeMoved : string, itemToMoveAfter : ProjectItemElement, projectNode : ProjectNode) =
        let msbuildProject = projectNode.BuildProject
        let big = EnsureValid msbuildProject projectNode true
        let mutable index = 0
        let mutable itemToMoveAfterIndex = -1
        let itemsToMove = 
            [for bi in EnumerateItems(big) do
                let curFolder = ComputeFolder(GetUnescapedUnevaluatedInclude(bi), projectNode.BaseURI)
                if curFolder.StartsWith(folderToBeMoved, StringComparison.OrdinalIgnoreCase) then
                    yield (bi, index)
                if Same(bi, itemToMoveAfter) then
                    itemToMoveAfterIndex <- index
                index <- index + 1]
        if itemToMoveAfterIndex = -1 then
            Debug.Assert(false, sprintf "did not find item to move after <%s Include=\"%s\">" (GetItemType itemToMoveAfter) (GetUnescapedUnevaluatedInclude itemToMoveAfter))
        for (item,i) in List.rev itemsToMove do
            Debug.Assert(i <> index - 1, "item is already at bottom")
            Debug.Assert(itemToMoveAfterIndex > i, "not moving down")
            big.RemoveChild(item)
            big.InsertAfterChild(item, itemToMoveAfter)

    static member MoveFolderDown(toMove : HierarchyNode, toMoveAfter : HierarchyNode, projectNode : ProjectNode) =
        let folderToBeMoved = ComputeFolder(toMove.Url, projectNode.BaseURI)
        let itemToMoveAfter = MSBuildUtilities.FindLast(toMoveAfter, projectNode)
        MSBuildUtilities.MoveFolderDownHelper(folderToBeMoved, itemToMoveAfter, projectNode)

    // this method is only for DEBUG and unit tests
    static member AllVisibleItemFilenames(projectNode : ProjectNode) =
        // TODO this logic is slightly broken, should probably use MSBuildProject.ItemIsVisible
        let msbuildProject = projectNode.BuildProject
        [for bi in MSBuildProject.GetStaticItemsInOrder(msbuildProject) do
            if obj.ReferenceEquals(bi.Xml.ContainingProject, msbuildProject.Xml) then // ignore imported items
                if not (projectNode.FilterItemTypeToBeAddedToHierarchy(bi.ItemType)) then  // ignore references, etc (is this right for folders?)
                    if not (0=System.String.Compare(bi.GetMetadataValue("Visible"), "false", System.StringComparison.OrdinalIgnoreCase)) then
                        if not (bi.ItemType = ProjectFileConstants.Folder) then // skip folders, just want filenames
                            yield Path.GetFileName(GetInclude(bi))]
