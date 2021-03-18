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
                LanguageNames.CSharp
            )
        workspace.AddDocument(projId, "test.fs", text)

    static member CreateDocument (code: string) =
        let text = SourceText.From(code)
        RoslynTestHelpers.CreateDocument(text), text

