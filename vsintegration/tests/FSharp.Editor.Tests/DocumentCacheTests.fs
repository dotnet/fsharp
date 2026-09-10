// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open System.Threading
open Xunit
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Editor.Tests.Helpers

type DocumentCacheTests() =
    // Two files of one project: `Other` compiles ahead of `Target`, so a change to `Other` bumps the
    // project's dependent semantic version without touching `Target`'s own text.
    let openTwoFileProject () =
        let projectId = ProjectId.CreateNewId()
        let otherPath = "C:\Other.fs"
        let targetPath = "C:\Target.fs"

        let otherInfo =
            RoslynTestHelpers.CreateDocumentInfo projectId otherPath "let value = 1"

        let targetInfo =
            RoslynTestHelpers.CreateDocumentInfo projectId targetPath "let read () = 1"

        let projectInfo =
            RoslynTestHelpers.CreateProjectInfo projectId "C:\test.fsproj" [ otherInfo; targetInfo ]

        let solution = RoslynTestHelpers.CreateSolution [ projectInfo ]

        { RoslynTestHelpers.DefaultProjectOptions with
            SourceFiles = [| otherPath; targetPath |]
        }
        |> RoslynTestHelpers.SetProjectOptions projectId solution

        otherInfo.Id, targetInfo.Id, solution.Workspace

    let tryGetValue (cache: DocumentCache<string>) document =
        (cache.TryGetValueAsync document CancellationToken.None).GetAwaiter().GetResult()

    let setValue (cache: DocumentCache<string>) document value =
        (cache.SetAsync (document, value) CancellationToken.None).GetAwaiter().GetResult()

    // A document's own text is not the whole story a cached value depends on: an edit elsewhere in
    // the project can change what its names mean while its text stands still.
    [<Fact>]
    member _.``A cached value is invalidated when the project's dependent semantic version changes``() =
        use cache = new DocumentCache<string>("DocumentCacheTests")
        let otherId, targetId, workspace = openTwoFileProject ()

        let targetDocument () =
            workspace.CurrentSolution.GetDocument targetId

        setValue cache (targetDocument ()) "cached"

        Assert.True((tryGetValue cache (targetDocument ())).IsSome, "The value must be readable before anything changes.")

        Assert.True(
            workspace.TryApplyChanges(workspace.CurrentSolution.WithDocumentText(otherId, SourceText.From "let value = 2")),
            "The workspace has to accept the change to the other file."
        )

        Assert.True(
            (tryGetValue cache (targetDocument ())).IsNone,
            "A change to another file of the project must invalidate the cached value."
        )

    [<Fact>]
    member _.``A cached value survives when nothing about the project has changed``() =
        use cache = new DocumentCache<string>("DocumentCacheTests")
        let _, targetId, workspace = openTwoFileProject ()

        let targetDocument () =
            workspace.CurrentSolution.GetDocument targetId

        setValue cache (targetDocument ()) "cached"

        Assert.Equal(ValueSome "cached", tryGetValue cache (targetDocument ()))
