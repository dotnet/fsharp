[<AutoOpen>]
module internal Microsoft.VisualStudio.FSharp.Editor.CodeAnalysisExtensions

open Microsoft.CodeAnalysis
open FSharp.Compiler.Text
open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.IO
open System.Runtime.CompilerServices

/// A compiler range's file name. A build with a path map (DeterministicSourcePaths) leaves it relative to a root the
/// assembly never records, so it only gives the tail of a document's path. .NET Framework throws on a name written for another OS.
let (|RootedFileName|MappedFileName|) (fileName: string) =
    try
        if Path.IsPathRooted fileName then
            RootedFileName
        else
            MappedFileName
    with :? ArgumentException ->
        MappedFileName

/// None where the name climbs above its start: what it denotes depends on the root a path map never records.
let private mappedNameTail (fileName: string) =
    let segments = ResizeArray<string>()
    let mutable climbsAboveItsStart = false

    for segment in fileName.Split([| '/'; '\\' |], StringSplitOptions.RemoveEmptyEntries) do
        match segment with
        | "." -> ()
        | ".." when segments.Count > 0 -> segments.RemoveAt(segments.Count - 1)
        | ".." -> climbsAboveItsStart <- true
        | segment -> segments.Add segment

    if climbsAboveItsStart || segments.Count = 0 then
        ValueNone
    else
        let separator = string Path.DirectorySeparatorChar
        ValueSome $"{separator}{String.Join(separator, segments)}"

let private endsWithMappedName (path: string) (tail: string) =
    path.AsSpan().EndsWith(tail.AsSpan(), StringComparison.OrdinalIgnoreCase)

let isTheFileAt (path: string) (fileName: string) =
    match fileName with
    | null -> false
    | RootedFileName -> String.Equals(Path.GetFullPathSafe fileName, path, StringComparison.OrdinalIgnoreCase)
    | MappedFileName -> mappedNameTail fileName |> ValueOption.exists (endsWithMappedName path)

/// One scan of the solution's documents per name: every link of a tooltip and every project searched asks again.
let private documentsByMappedName =
    ConditionalWeakTable<Solution, ConcurrentDictionary<string, DocumentId list>>()

let private candidatesForMappedName (solution: Solution) (relative: string) =
    let byName =
        documentsByMappedName.GetValue(solution, fun _ -> ConcurrentDictionary(StringComparer.OrdinalIgnoreCase))

    byName.GetOrAdd(
        relative,
        fun relative ->
            match mappedNameTail relative with
            | ValueNone -> []
            | ValueSome tail ->
                [
                    for project in solution.Projects do
                        for document in project.Documents do
                            if endsWithMappedName document.FilePath tail then
                                document.Id
                ]
    )

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

let private assembliesReadBy (reader: Project) =
    [
        reader.AssemblyName

        for referenced in reader.GetAllProjectsThisProjectDependsOn() do
            referenced.AssemblyName

        for reference in reader.MetadataReferences do
            match reference with
            | :? PortableExecutableReference as file ->
                match file.FilePath with
                | null -> ()
                | path -> Path.GetFileNameWithoutExtension path
            | _ -> ()
    ]

let private documentsOfMappedName (solution: Solution) (relative: string) (declaringAssemblies: string list) =
    let isDeclaredByAny (documentId: DocumentId) =
        let assembly = solution.GetProject(documentId.ProjectId).AssemblyName

        declaringAssemblies
        |> List.exists (fun declarer -> String.Equals(declarer, assembly, StringComparison.OrdinalIgnoreCase))

    let candidates =
        candidatesForMappedName solution relative |> List.filter isDeclaredByAny

    let distinctFiles =
        HashSet(
            candidates
            |> List.map (fun documentId -> solution.GetDocument(documentId).FilePath),
            StringComparer.OrdinalIgnoreCase
        )

    if distinctFiles.Count = 1 then candidates else []

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

    /// The documents of a range's file; for a mapped name, the one file ending with it in `declaringAssembly` or one `readBy` reads.
    member self.GetDocumentIdsWithFSharpFileName(fileName: string, ?declaringAssembly: string, ?readBy: Project) =
        match fileName with
        | null -> []
        | RootedFileName -> self.GetDocumentIdsWithFilePath(Path.GetFullPathSafe fileName) |> List.ofSeq
        | MappedFileName ->
            match declaringAssembly, readBy with
            | Some assembly, _ -> documentsOfMappedName self fileName [ assembly ]
            | None, Some reader -> documentsOfMappedName self fileName (assembliesReadBy reader)
            | None, None -> []

    /// Try to retrieve the corresponding DocumentId for the range's file in the solution
    /// and if a projectId is provided, only try to find the document within that project
    /// or a project referenced by that project
    member self.TryGetDocumentIdFromFSharpRange(range: range, ?projectId: ProjectId, ?declaringAssembly: string) =

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

        self.GetDocumentIdsWithFSharpFileName(
            range.FileName,
            ?declaringAssembly = declaringAssembly,
            ?readBy = (projectId |> Option.bind self.TryGetProject)
        )
        |> matchingDoc

    /// Try to retrieve the corresponding Document for the range's file in the solution
    /// and if a projectId is provided, only try to find the document within that project
    /// or a project referenced by that project
    member self.TryGetDocumentFromFSharpRange(range: range, ?projectId: ProjectId, ?declaringAssembly: string) =
        self.TryGetDocumentIdFromFSharpRange(range, ?projectId = projectId, ?declaringAssembly = declaringAssembly)
        |> Option.map self.GetDocument
