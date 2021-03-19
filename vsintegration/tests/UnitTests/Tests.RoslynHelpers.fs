namespace Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text

[<AbstractClass;Sealed>]
type RoslynTestHelpers private () =

    static member CreateDocument (text: SourceText) =
        let workspace = new AdhocWorkspace()
        let proj = workspace.AddProject("test", LanguageNames.CSharp)
        workspace.AddDocument(proj.Id, "test.fs", text)

    static member CreateDocument (code: string) =
        let text = SourceText.From(code)
        RoslynTestHelpers.CreateDocument(text), text

