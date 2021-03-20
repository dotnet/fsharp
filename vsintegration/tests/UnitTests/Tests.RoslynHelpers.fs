namespace Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text

[<AbstractClass;Sealed>]
type RoslynTestHelpers private () =

    static member CreateDocument (filePath, text: SourceText) =
        let workspace = new AdhocWorkspace()
        let proj = workspace.AddProject("test.fsproj", LanguageNames.CSharp)

        let docInfo =
            let docId = DocumentId.CreateNewId(proj.Id)
            DocumentInfo.Create(docId,
                filePath,
                loader=TextLoader.From(text.Container, VersionStamp.Create()),
                filePath=filePath)

        workspace.AddDocument(docInfo)

    static member CreateDocument (filePath, code: string) =
        let text = SourceText.From(code)
        RoslynTestHelpers.CreateDocument(filePath, text), text

