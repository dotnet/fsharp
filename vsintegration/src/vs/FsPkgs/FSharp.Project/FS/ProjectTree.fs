namespace Microsoft.VisualStudio.FSharp.ProjectSystem

open System

module PathUtils =
    
    let (|SubUri|_|) (baseUri: Uri) (uri: Uri) =
        if baseUri.IsBaseOf(uri)
        then Some (baseUri.MakeRelativeUri(uri))
        else None

    let displayPathFrom (baseUri: Uri) (uri: Uri) =
        match Uri(baseUri,uri) with
        | SubUri baseUri (relativeUri) -> relativeUri
        | outOfDirUri when outOfDirUri.IsAbsoluteUri -> 
            Uri(outOfDirUri.Segments |> Seq.last, UriKind.Relative)
        | u -> failwithf "unexpected uri '%O' from base '%O' and uri '%O'" u baseUri uri

    let ensureTrailingDirectorySeparator path = 
        IO.Path.Combine(path, ".").TrimEnd(".".ToCharArray())

module MSBuildEvaluationHelpers =
    open Microsoft.Build.Evaluation

    let metadata name (item: ProjectItem) =
        if item.HasMetadata(name)
        then Some (item.GetMetadataValue(name))
        else None

    let linkMetadata (item: ProjectItem) =
        item
        |> metadata ProjectFileConstants.Link
        |> Option.bind (fun x -> if String.IsNullOrWhiteSpace(x) then None else Some x)
        |> Option.map (fun x -> Uri(x, UriKind.Relative))

    let projectDir (msbuildProject: Project) =
        Uri(msbuildProject.DirectoryPath |> PathUtils.ensureTrailingDirectorySeparator)

    let projectName (project: Project) =
        project.FullPath |> IO.Path.GetFileNameWithoutExtension

module MSBuildConstructionHelpers =
    open Microsoft.Build.Construction

    let metadata name (item: ProjectItemElement) =
        let found =
            [ for m in item.Metadata do
                // REVIEW: this ignores Condition, doesn't seem reasonable to do anything else
                if m.Name = name then
                    yield m.Value ]
        found
        |> List.tryPick Some
        |> Option.map Microsoft.Build.Evaluation.ProjectCollection.Unescape

    let linkMetadata (item: ProjectItemElement) =
        item
        |> metadata ProjectFileConstants.Link
        |> Option.bind (fun x -> if String.IsNullOrWhiteSpace(x) then None else Some x)
        |> Option.map (fun x -> Uri(x, UriKind.Relative))

    let includePath (item: ProjectItemElement) =
        item.Include
        |> Microsoft.Build.Evaluation.ProjectCollection.Unescape
        |> fun x -> Uri(x, UriKind.RelativeOrAbsolute)

module MSBuildProjectItem =
    open Microsoft.Build.Evaluation
    open Microsoft.Build.Construction
    open MSBuildEvaluationHelpers

    let ofMSBuildItemElement (item: ProjectItemElement) =
        let link = MSBuildConstructionHelpers.linkMetadata item
        let includePath = MSBuildConstructionHelpers.includePath item
        let t =
            match item.ItemType with
            | ProjectFileConstants.COMReference -> 
                MSBuildProjectItem.ComReference
            | "ComFileReference" -> 
                MSBuildProjectItem.ComFileReference
            | ProjectFileConstants.Compile -> 
                MSBuildProjectItem.Compile
            | ProjectFileConstants.Content -> 
                MSBuildProjectItem.Content
            | ProjectFileConstants.EmbeddedResource -> 
                MSBuildProjectItem.EmbeddedResource
            | ProjectFileConstants.Folder -> 
                MSBuildProjectItem.Folder
            | "NativeReference" -> 
                MSBuildProjectItem.NativeReference
            | ProjectFileConstants.None -> 
                MSBuildProjectItem.None
            | ProjectFileConstants.ProjectReference -> 
                MSBuildProjectItem.ProjectReference
            | ProjectFileConstants.Reference -> 
                MSBuildProjectItem.Reference
            | s -> 
                MSBuildProjectItem.Item s
        { Item = item; Content = t; Link = link; Include = includePath }

module ProjectTree =

    /// <summary>
    /// gets the location that will be displayed in the solution explorer.
    /// </summary>
    let displayPath projectDir (link, includePath) =
        // Gets the <... Include="path"> path for this item, except if the item is a link, then
        // gets the <Link>path</Link> value instead.
        let path : Uri =
            match link with
            | None -> includePath
            | Some p -> p
        
        path |> PathUtils.displayPathFrom projectDir

    let displayHierarchyPath (item, path: Uri) =
        let asTree s (name: string) =
            let normalizedName = name.TrimEnd([| '/' |]) |> Uri.UnescapeDataString
            let node =
                match s with
                | None -> 
                    RoseTree.Node (ProjectTreeNode.Item (normalizedName, item), [])
                | Some l -> 
                    RoseTree.Node (ProjectTreeNode.Folder (normalizedName), [l])
            Some node

        let toAbsoluteUri (r: Uri) = Uri(Uri("sln:///"), r)
        
        (toAbsoluteUri path).Segments
        |> List.ofArray
        |> List.tail
        |> List.rev
        |> List.fold asTree None
        |> Option.get

    let label node =
        match node with
        | ProjectTreeNode.Folder n -> n
        | ProjectTreeNode.Item (n, _) -> n

    let nodeName node =
        match node with
        | ProjectTreeNode.Folder n -> n.ToUpperInvariant()
        | ProjectTreeNode.Item (n, _) -> n.ToUpperInvariant()

    let normalizeFolder x y =
        if (nodeName x) <> (nodeName y) then None
        else 
            match x, y with
            | ProjectTreeNode.Folder(_), ProjectTreeNode.Folder(_) ->
                Some(x, [ y ])
            | ProjectTreeNode.Folder(_), ProjectTreeNode.Item(_, { Content = MSBuildProjectItem.Folder }) ->
                Some(x, [ y ])
            | ProjectTreeNode.Item(_, { Content = MSBuildProjectItem.Folder }), ProjectTreeNode.Folder(_) ->
                Some(y, [ x ])
            | ProjectTreeNode.Item(_, itemN), ProjectTreeNode.Item(_, itemM) -> 
                match itemN, itemM with
                | { Content = MSBuildProjectItem.Folder }, { Content = MSBuildProjectItem.Folder } -> 
                    Some(x, [ y ])
                | _ -> None
            | _ -> None

    let alreadyRenderedFolderNodeKey node = 
        let name = nodeName node
        match node with
        | ProjectTreeNode.Folder _ -> Some name
        | ProjectTreeNode.Item (_, { Content = MSBuildProjectItem.Folder }) -> Some name
        | _ -> None

    let checkAlreadyRenderedFolder getKey tree =
        let colorDuplicated visited node =
            match node with
            | Node ((keyX,x,_),_) ->
                let withSameKey (Node((keyY,_,_),_)) =
                    match keyX, keyY with
                    | Some a, Some b -> a = b
                    | _ -> false
                if visited |> List.exists withSameKey
                then Node((keyX,x,true), []) :: visited
                else node :: visited

        let findDuplicated l (path,(_,x,invalid)) =
            if invalid
            then (path,x) :: l
            else l

        tree
        |> RoseTree.map (fun node -> (getKey node), node, false)
        |> RoseTree.mapForest (List.fold colorDuplicated [])
        |> RoseTree.addPath
        |> RoseTree.fold findDuplicated []
        |> List.map (fun (path,x) -> 
                        let p = path |> List.map (fun (_,b,_) -> b)
                        p,x )

    let root = Node(ProjectTreeNode.Folder "/", [])

    let createTree getPath items =

        let unmergedProjectTree =
            items
            |> List.map (function x -> (x, (getPath x : Uri)))
            |> List.map displayHierarchyPath
            |> List.fold RoseTree.appendTree root

        let mergeFolders removedNodes x y =
            match normalizeFolder x y with
            | None -> None
            | Some (normalized,removed) ->
                removed |> List.iter removedNodes
                Some (normalized)

        let uselessFolders = ResizeArray()
        let trackRemovedFolders node =
            match node with
            | ProjectTreeNode.Item (_, x) -> 
                match x with
                | { Content = MSBuildProjectItem.Folder } -> uselessFolders.Add(x)
                | _ -> ()
            | ProjectTreeNode.Folder _ -> ()

        let projectTree =
            unmergedProjectTree
            |> RoseTree.mapForest (RoseTree.foldSiblings (mergeFolders trackRemovedFolders))

        projectTree, (uselessFolders |> List.ofSeq)
