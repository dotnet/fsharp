[<AutoOpen>]
module internal Microsoft.VisualStudio.FSharp.Editor.CodeAnalysisExtensions

open Microsoft.CodeAnalysis
open FSharp.Compiler.Text
open System
open System.IO

/// Whether the file name a compiler range carries is the file at this path. A build that maps its source
/// paths (`DeterministicSourcePaths`) leaves that name relative to a root the assembly never records, so a
/// relative one is matched by its tail rather than resolved against the process's current directory —
/// which is not that root, and belongs to whatever last set it.
let isTheFileAt (path: string) (fileName: string) =
    // Paths, not identifiers: the file systems this runs on do not case them.
    let comparison = StringComparison.OrdinalIgnoreCase

    match path, fileName with
    | null, _
    | _, null -> false
    | path, rooted when Path.IsPathRooted rooted -> String.Equals(Path.GetFullPathSafe rooted, path, comparison)
    | path, relative ->
        let separator = string Path.DirectorySeparatorChar

        let fromTheRoot =
            relative.Split([| '/'; '\\' |], StringSplitOptions.RemoveEmptyEntries)
            |> Array.filter (fun segment -> segment <> ".")
            |> String.concat separator

        // Anchored on a separator so that a name matches whole directories, never the tail of one.
        path.EndsWith($"{separator}{fromTheRoot}", comparison)

type Project with

    /// Returns the projectIds of all projects within the same solution that directly reference this project
    member this.GetDependentProjectIds() =
        this.Solution.GetProjectDependencyGraph().GetProjectsThatDirectlyDependOnThisProject this.Id

    /// Returns all projects within the same solution that directly reference this project.
    member this.GetDependentProjects() =
        this.Solution.GetProjectDependencyGraph().GetProjectsThatDirectlyDependOnThisProject this.Id
        |> Seq.map this.Solution.GetProject

    /// Returns the ProjectIds of all of the projects that this project directly or transitively depends on
    member this.GetProjectIdsOfAllProjectsThisProjectDependsOn() =
        let graph = this.Solution.GetProjectDependencyGraph()

        let transitiveDependencies =
            graph.GetProjectsThatThisProjectTransitivelyDependsOn this.Id

        let directDependencies = graph.GetProjectsThatThisProjectDirectlyDependsOn this.Id
        Seq.append directDependencies transitiveDependencies

    /// The list all of the projects that this project directly or transitively depends on
    member this.GetAllProjectsThisProjectDependsOn() =
        this.GetProjectIdsOfAllProjectsThisProjectDependsOn()
        |> Seq.map this.Solution.GetProject

type Solution with

    /// Checks if the file path is associated with a document in the solution.
    member self.ContainsDocumentWithFilePath filePath =
        self.GetDocumentIdsWithFilePath(filePath).IsEmpty |> not

    /// Try to get a document inside the solution using the document's name
    member self.TryGetDocumentNamed docName =
        self.Projects
        |> Seq.tryPick (fun proj -> proj.Documents |> Seq.tryFind (fun doc -> doc.Name = docName))

    /// Try to find the document corresponding to the provided filepath within this solution
    member self.TryGetDocumentFromPath filePath =
        // It's crucial to normalize file path here (specifically, remove relative parts),
        // otherwise Roslyn does not find documents.
        self.GetDocumentIdsWithFilePath(Path.GetFullPath filePath)
        |> ImmutableArray.tryHeadV
        |> ValueOption.map (fun docId -> self.GetDocument docId)

    /// Try to get a project inside the solution using the project's id
    member self.TryGetProject(projId: ProjectId) =
        if self.ContainsProject projId then
            Some(self.GetProject projId)
        else
            None

    /// Returns the projectIds of all projects within this solution that directly reference the provided project
    member self.GetDependentProjects(projectId: ProjectId) =
        self.GetProjectDependencyGraph().GetProjectsThatDirectlyDependOnThisProject projectId
        |> Seq.map self.GetProject

    /// Returns the projectIds of all projects within this solution that directly reference the provided project
    member self.GetDependentProjectIds(projectId: ProjectId) =
        self.GetProjectDependencyGraph().GetProjectsThatDirectlyDependOnThisProject projectId

    /// Returns the ProjectIds of all of the projects that directly or transitively depends on
    member self.GetProjectIdsOfAllProjectReferences(projectId: ProjectId) =
        let graph = self.GetProjectDependencyGraph()

        let transitiveDependencies =
            graph.GetProjectsThatThisProjectTransitivelyDependsOn projectId

        let directDependencies = graph.GetProjectsThatThisProjectDirectlyDependsOn projectId
        Seq.append directDependencies transitiveDependencies

    /// Returns all of the projects that this project that directly or transitively depends on
    member self.GetAllProjectsThisProjectDependsOn(projectId: ProjectId) =
        self.GetProjectIdsOfAllProjectReferences projectId |> Seq.map self.GetProject

    /// The documents whose file is the one a compiler range names. A name a path map left relative
    /// reaches no document through the workspace's index, which is keyed by the paths on disk, so it
    /// is matched against those paths one by one instead.
    member self.GetDocumentIdsWithFSharpFileName(fileName: string) =
        match fileName with
        | null -> []
        | rooted when Path.IsPathRooted rooted -> self.GetDocumentIdsWithFilePath(Path.GetFullPathSafe rooted) |> List.ofSeq
        | relative ->
            [
                for project in self.Projects do
                    for document in project.Documents do
                        if relative |> isTheFileAt document.FilePath then
                            document.Id
            ]

    /// Try to retrieve the corresponding DocumentId for the range's file in the solution
    /// and if a projectId is provided, only try to find the document within that project
    /// or a project referenced by that project
    member self.TryGetDocumentIdFromFSharpRange(range: range, ?projectId: ProjectId) =

        let checkProjectId (docId: DocumentId) =
            if projectId.IsSome then
                docId.ProjectId = projectId.Value
            else
                false
        //The same file may be present in many projects. We choose one from current or referenced project.
        let rec matchingDoc =
            function
            | [] -> None
            | [ docId ] -> Some docId
            | (docId: DocumentId) :: _ when checkProjectId docId -> Some docId
            | docId :: tail ->
                match projectId with
                | Some projectId ->
                    if self.GetDependentProjectIds docId.ProjectId |> Seq.contains projectId then
                        Some docId
                    else
                        matchingDoc tail
                | None -> Some docId

        self.GetDocumentIdsWithFSharpFileName range.FileName |> matchingDoc

    /// Try to retrieve the corresponding Document for the range's file in the solution
    /// and if a projectId is provided, only try to find the document within that project
    /// or a project referenced by that project
    member self.TryGetDocumentFromFSharpRange(range: range, ?projectId: ProjectId) =
        match projectId with
        | Some projectId -> self.TryGetDocumentIdFromFSharpRange(range, projectId)
        | None -> self.TryGetDocumentIdFromFSharpRange range
        |> Option.map self.GetDocument
