// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open System.Threading
open Xunit
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Structure
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Editor.Tests.Helpers

type BlockStructureServiceTests() =

    let getBlockStructure (document: Document) =
        (FSharpBlockStructureService() :> IFSharpBlockStructureService)
            .GetBlockStructureAsync(document, CancellationToken.None)
            .GetAwaiter()
            .GetResult()

    // Outlining needs only a parse, so a project without options yet (Visual Studio still loading it)
    // must not answer with no regions and collapse everything the user had folded.
    [<Fact>]
    member _.``Block structure falls back to quick parsing options when project options are unavailable``() =
        let projectId = ProjectId.CreateNewId()

        let documentInfo =
            RoslynTestHelpers.CreateDocumentInfo projectId "test.fs" "module M\n\nlet f x =\n    let y = x + 1\n    y * 2\n"

        let projectInfo =
            RoslynTestHelpers.CreateProjectInfo projectId "test.fsproj" [ documentInfo ]

        let solution = RoslynTestHelpers.CreateSolution [ projectInfo ]
        let document = RoslynTestHelpers.GetSingleDocument solution

        let structure = getBlockStructure document

        Assert.NotEmpty structure.Spans
