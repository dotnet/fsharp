namespace Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text

[<AbstractClass;Sealed>]
type RoslynTestHelpers private () =

    static member CreateDocument (text: SourceText) =
        let workspace = new AdhocWorkspace()
        let proj = workspace.AddProject("test", LanguageNames.CSharp)

        let docInfo =
            let docId = DocumentId.CreateNewId(proj.Id)
            DocumentInfo.Create(docId,
                "test.fs",
                loader=TextLoader.From(text.Container, VersionStamp.Create()),
                filePath="""C:\test.fs""")

        workspace.AddDocument(docInfo)

    static member CreateDocument (code: string) =
        let text = SourceText.From(code)
        RoslynTestHelpers.CreateDocument(text), text

