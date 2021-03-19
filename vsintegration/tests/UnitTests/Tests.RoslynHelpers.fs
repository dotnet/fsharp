namespace Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text

[<AbstractClass;Sealed>]
type RoslynTestHelpers private () =

    static member CreateDocument (text: SourceText) =
        let workspace = new AdhocWorkspace()
        let proj = workspace.AddProject("testProject", LanguageNames.CSharp)

        let docInfo =
            let docId = DocumentId.CreateNewId(proj.Id)
            DocumentInfo.Create(docId,
                "testFile.fs",
                loader=TextLoader.From(text.Container, VersionStamp.Create()),
                filePath="testFile.fs")

        workspace.AddDocument(docInfo)

    static member CreateDocument (code: string) =
        let text = SourceText.From(code)
        RoslynTestHelpers.CreateDocument(text), text

