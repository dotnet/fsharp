module LanguageServer.ProjectContextsProtocolTests

open System
open Xunit

open Microsoft.VisualStudio.LanguageServer.Protocol
open FSharp.Test.ProjectGeneration.WorkspaceHelpers
open FSharp.Compiler.CodeAnalysis.ProjectSnapshot

open LanguageServer.ProtocolHelpers

#nowarn "57"

[<Fact>]
let ``GetProjectContexts response format matches VS expectations`` () =
    task {
        let! client = initializeLanguageServer None
        let projectName = "TestProject"
        let fileOnDisk = setupMultiProjectFile client cleanCode [ projectName ]
        do! openDocument client fileOnDisk cleanCode 1

        let! result = getProjectContexts client fileOnDisk
        Assert.NotNull(result)
        Assert.Equal(1, result.ProjectContexts.Length)
        Assert.Equal(0, result.DefaultIndex)

        let ctx = result.ProjectContexts[0]
        Assert.Equal(projectName, ctx.Label)
        Assert.Equal(VSProjectKind.FSharp, ctx.Kind)
        Assert.False(String.IsNullOrWhiteSpace(ctx.Id), "Project context Id should not be empty")
        Assert.Contains("|", ctx.Id)
        let parts = ctx.Id.Split('|', 2)
        Assert.True(Guid.TryParse(parts[0]) |> fst, $"First part of Id should be a valid GUID, got: '{parts[0]}'")
        Assert.False(String.IsNullOrWhiteSpace(parts[1]), "Second part of Id (debug name) should not be empty")
    }

[<Fact>]
let ``GetProjectContexts returns empty list for file not in any project`` () =
    task {
        let! client = initializeLanguageServer None
        let fileOnDisk = sourceFileOnDisk cleanCode
        do! openDocument client fileOnDisk cleanCode 1

        let! result = getProjectContexts client fileOnDisk
        Assert.NotNull(result)
        Assert.NotNull(result.ProjectContexts)
        Assert.Equal(0, result.ProjectContexts.Length)
        Assert.Equal(0, result.DefaultIndex)
    }

[<Fact>]
let ``GetProjectContexts returns multiple FSharp contexts with distinct labels and ids for file in multiple projects`` () =
    task {
        let! client = initializeLanguageServer None
        let fileOnDisk = setupMultiProjectFile client sharedModuleContent [ "ProjectA"; "ProjectB" ]

        do! openDocument client fileOnDisk sharedModuleContent 1

        let! result = getProjectContexts client fileOnDisk
        Assert.NotNull(result)
        Assert.True(result.ProjectContexts.Length >= 2, $"Expected at least 2 project contexts but got {result.ProjectContexts.Length}")
        Assert.Equal(0, result.DefaultIndex)
        Assert.All(result.ProjectContexts, fun c -> Assert.Equal(VSProjectKind.FSharp, c.Kind))
        let labels = result.ProjectContexts |> Array.map (fun c -> c.Label) |> Array.distinct
        Assert.Equal(result.ProjectContexts.Length, labels.Length)
        let ids = result.ProjectContexts |> Array.map (fun c -> c.Id) |> Array.distinct
        Assert.Equal(result.ProjectContexts.Length, ids.Length)
    }

[<Fact>]
let ``GetProjectContexts returns results for file not opened via didOpen`` () =
    task {
        let! client = initializeLanguageServer None
        let fileOnDisk = setupSingleFileProject client cleanCode

        let! result = getProjectContexts client fileOnDisk
        Assert.NotNull(result)
        Assert.NotNull(result.ProjectContexts)
        Assert.Equal(1, result.ProjectContexts.Length)
        Assert.Equal(VSProjectKind.FSharp, result.ProjectContexts[0].Kind)
    }
