[<AutoOpen>]
module internal Microsoft.VisualStudio.FSharp.Editor.CodeAnalysisExtensions

open Microsoft.CodeAnalysis


type Project with

    /// The list all projects within the same solution that reference this project.
    member this.GetDependentProjects () =
        this.Solution.GetProjectDependencyGraph().GetProjectsThatDirectlyDependOnThisProject(this.Id)
        |> Seq.map this.Solution.GetProject

type Solution with 

    /// Try to get a document inside the solution using the document's name
    member self.TryGetDocumentNamed docName =
        self.Projects |> Seq.tryPick (fun proj ->
            proj.Documents |> Seq.tryFind (fun doc -> doc.Name = docName))

    /// Try to get a project inside the solution using the project's id
    member self.TryGetProject (projId:ProjectId) =
        if self.ContainsProject projId then Some (self.GetProject projId) else None

    member self.GetDependentProjects (projectId:ProjectId) =
        self.GetProject(projectId).GetDependentProjects()

