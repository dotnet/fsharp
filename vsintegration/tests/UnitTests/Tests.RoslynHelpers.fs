namespace Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn

open System
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text

[<AbstractClass;Sealed>]
type RoslynTestHelpers private () =

    static member CreateDocument (filePath, text: SourceText) =
        let workspace = new AdhocWorkspace()
        let projInfo =
            let projId = ProjectId.CreateNewId()
            ProjectInfo.Create(
                projId,
                VersionStamp.Create(DateTime.UtcNow),
                "test.fsproj", 
                "test.dll", 
                LanguageNames.CSharp
            )
        let proj = workspace.AddProject(projInfo)

        let docInfo =
            let docId = DocumentId.CreateNewId(proj.Id)
            DocumentInfo.Create(
                docId,
                filePath,
                loader=TextLoader.From(text.Container, VersionStamp.Create(DateTime.UtcNow)),
                filePath=filePath,
                sourceCodeKind=SourceCodeKind.Regular)

        workspace.AddDocument(docInfo)

    static member CreateDocument (filePath, code: string) =
        let text = SourceText.From(code)
        RoslynTestHelpers.CreateDocument(filePath, text), text

