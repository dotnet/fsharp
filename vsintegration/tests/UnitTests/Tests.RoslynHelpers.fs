namespace Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text

[<AbstractClass;Sealed>]
type RoslynTestHelpers private () =

    static member CreateDocument (text: SourceText) =
        let workspace = new AdhocWorkspace()
        let sol = workspace.CurrentSolution

        let projId = ProjectId.CreateNewId()
        let projInfo = 
            ProjectInfo.Create(
                projId,
                VersionStamp.Create(),
                "test",
                "test",
                "F#"
            )
        let docId = DocumentId.CreateNewId(projId)
        let loader = TextLoader.From(text.Container, VersionStamp.Create())
        let docInfo = DocumentInfo.Create(docId, "test.fs", sourceCodeKind=SourceCodeKind.Regular, loader=loader, isGenerated=true)

        let projInfo = projInfo.WithDocuments([docInfo])

        let sol = sol.AddProject(projInfo)

        sol.GetProject(projId).Documents |> Seq.find (fun x -> x.Id.Equals(docId))

    static member CreateDocument (code: string) =
        let text = SourceText.From(code)
        RoslynTestHelpers.CreateDocument(text), text

