[<AutoOpen>]
module internal Microsoft.VisualStudio.FSharp.Editor.CodeAnalysisExtensions

open Microsoft.CodeAnalysis
open FSharp.Compiler.Text
open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.IO
open System.Runtime.CompilerServices

/// Whether the name is rooted, for a name that need not be a path this platform can spell: an imported
/// declaration can carry one written on another operating system, and `Path.IsPathRooted` throws on it
/// where `.NET Framework` runs - which is where the editor runs.
let isRootedPath (path: string) =
    try
        Path.IsPathRooted path
    with :? ArgumentException ->
        false

/// The tail a document's path must end with to be the file a relative name denotes: the name with its `.` and
/// `..` segments resolved, in this platform's separator and anchored on one, so that it matches whole
/// directories, never the tail of one. A name that climbs above where it starts has no such tail: what it
/// denotes depends on the root, which the assembly never records.
let mappedNameTail (fileName: string) =
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

/// Whether the path ends with such a tail. The tail is built once for the name; each document of the
/// solution is then compared against it where its own path lies.
let endsWithMappedName (path: string) (tail: string) =
    match path with
    | null -> false
    // Paths, not identifiers: the file systems this runs on do not case them.
    | path -> path.AsSpan().EndsWith(tail.AsSpan(), StringComparison.OrdinalIgnoreCase)

/// Whether the file name a compiler range carries is the file at this path. A build that maps its source
/// paths (`DeterministicSourcePaths`) leaves that name relative to a root the assembly never records, so a
/// relative one is matched by its tail rather than resolved against the process's current directory —
/// which is not that root, and belongs to whatever last set it.
let isTheFileAt (path: string) (fileName: string) =
    match path, fileName with
    | null, _
    | _, null -> false
    | path, rooted when isRootedPath rooted -> String.Equals(Path.GetFullPathSafe rooted, path, StringComparison.OrdinalIgnoreCase)
    | path, relative ->
        match mappedNameTail relative with
        | ValueSome tail -> endsWithMappedName path tail
        | ValueNone -> false

/// The documents of a solution whose path ends the way a mapped name does, kept per solution instance: a search
/// asks for the same name once per project, and a solution is replaced rather than mutated.
let private documentsByMappedName =
    ConditionalWeakTable<Solution, ConcurrentDictionary<string, DocumentId list>>()

/// The documents whose path ends the way a name a path map left relative does. They are candidates only: the
/// path of an unrelated file can end the same way.
let private candidatesForMappedName (solution: Solution) (relative: string) =
    // The scan walks every document of the solution, and Find All References asks for the same name
    // once per project it searches, so the answer - including the empty one - is kept for the solution.
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

let private compilesAssembly (assembly: string) (project: Project) =
    String.Equals(project.AssemblyName, assembly, StringComparison.OrdinalIgnoreCase)

let private readsAssembly (reader: Project) =
    let assemblies =
        lazy
            (HashSet(
                seq {
                    reader.AssemblyName

                    for referenced in reader.GetAllProjectsThisProjectDependsOn() do
                        referenced.AssemblyName

                    for reference in reader.MetadataReferences do
                        match reference with
                        | :? PortableExecutableReference as file when not (isNull file.FilePath) ->
                            Path.GetFileNameWithoutExtension file.FilePath
                        | _ -> ()
                },
                StringComparer.OrdinalIgnoreCase
            ))

    fun (project: Project) -> assemblies.Value.Contains project.AssemblyName

let private documentsOfMappedName (solution: Solution) (relative: string) (isDeclarer: Project -> bool) =
    let candidates =
        candidatesForMappedName solution relative
        |> List.filter (fun documentId -> isDeclarer (solution.GetProject documentId.ProjectId))

    match candidates with
    | [] -> []
    | first :: others ->
        let path = solution.GetDocument(first).FilePath

        let isTheFile (documentId: DocumentId) =
            String.Equals(solution.GetDocument(documentId).FilePath, path, StringComparison.OrdinalIgnoreCase)

        if List.forall isTheFile others then candidates else []

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

    /// The documents of the file a compiler range names. A name a path map left relative only ends the way a path
    /// does, so it names the one such file of the declaring assembly: `declaringAssembly`, else any `readBy` reads.
    member self.GetDocumentIdsWithFSharpFileName(fileName: string, ?declaringAssembly: string, ?readBy: Project) =
        match fileName with
        | null -> []
        | rooted when isRootedPath rooted -> self.GetDocumentIdsWithFilePath(Path.GetFullPathSafe rooted) |> List.ofSeq
        | relative ->
            match declaringAssembly, readBy with
            | Some assembly, _ -> documentsOfMappedName self relative (compilesAssembly assembly)
            | None, Some reader -> documentsOfMappedName self relative (readsAssembly reader)
            | None, None -> []

    /// Try to retrieve the corresponding DocumentId for the range's file in the solution
    /// and if a projectId is provided, only try to find the document within that project
    /// or a project referenced by that project. Without `declaringAssembly`, a name a path map left relative
    /// is looked for in the assemblies that project reads.
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
