module FSharpDiagServer.Tests.ProjectManagerTests

open Xunit
open FSharp.Compiler.CodeAnalysis
open FSharpDiagServer.ProjectManager

let private createManager () =
    let checker = FSharpChecker.Create()
    ProjectManager(checker)

let private dummyOptions projPath =
    { FSharpProjectOptions.ProjectFileName = projPath
      ProjectId = None
      SourceFiles = [||]
      OtherOptions = [||]
      ReferencedProjects = [||]
      IsIncompleteTypeCheckEnvironment = false
      UseScriptResolutionRules = false
      LoadTime = System.DateTime.MinValue
      UnresolvedReferences = None
      OriginalLoadReferences = []
      Stamp = None }

[<Fact>]
let ``New manager has empty cache`` () =
    let mgr = createManager ()
    Assert.Equal(0, mgr.CacheCount)

[<Fact>]
let ``InjectTestEntry populates cache`` () =
    let mgr = createManager ()
    mgr.InjectTestEntry("/a.fsproj", dummyOptions "/a.fsproj")
    Assert.Equal(1, mgr.CacheCount)
    Assert.True(mgr.HasCachedProject("/a.fsproj"))

[<Fact>]
let ``Invalidate all clears entire cache`` () =
    let mgr = createManager ()
    mgr.InjectTestEntry("/a.fsproj", dummyOptions "/a.fsproj")
    mgr.InjectTestEntry("/b.fsproj", dummyOptions "/b.fsproj")

    mgr.Invalidate()

    Assert.Equal(0, mgr.CacheCount)
    Assert.False(mgr.HasCachedProject("/a.fsproj"))
    Assert.False(mgr.HasCachedProject("/b.fsproj"))

[<Fact>]
let ``Invalidate nonexistent path leaves cache unchanged`` () =
    let mgr = createManager ()
    mgr.InjectTestEntry("/a.fsproj", dummyOptions "/a.fsproj")

    mgr.Invalidate("/nonexistent.fsproj")

    Assert.Equal(1, mgr.CacheCount)
    Assert.True(mgr.HasCachedProject("/a.fsproj"))

[<Fact>]
let ``Invalidate on empty cache is idempotent`` () =
    let mgr = createManager ()
    mgr.Invalidate()
    mgr.Invalidate("/x.fsproj")
    Assert.Equal(0, mgr.CacheCount)

[<Fact>]
let ``HasCachedProject normalizes paths`` () =
    let mgr = createManager ()
    mgr.InjectTestEntry("/a/../b/c.fsproj", dummyOptions "/b/c.fsproj")
    Assert.True(mgr.HasCachedProject("/b/c.fsproj"))

[<Fact>]
let ``Invalidate normalizes path before removal`` () =
    let mgr = createManager ()
    mgr.InjectTestEntry("/b/c.fsproj", dummyOptions "/b/c.fsproj")
    Assert.Equal(1, mgr.CacheCount)

    mgr.Invalidate("/a/../b/c.fsproj")

    Assert.Equal(0, mgr.CacheCount)
    Assert.False(mgr.HasCachedProject("/b/c.fsproj"))

[<Fact>]
let ``InjectTestEntry overwrites existing entry for same normalized path`` () =
    let mgr = createManager ()
    mgr.InjectTestEntry("/a.fsproj", dummyOptions "/a.fsproj")
    mgr.InjectTestEntry("/a.fsproj", dummyOptions "/a.fsproj")
    Assert.Equal(1, mgr.CacheCount)

[<Fact>]
let ``Multiple distinct projects coexist in cache`` () =
    let mgr = createManager ()
    mgr.InjectTestEntry("/project1.fsproj", dummyOptions "/project1.fsproj")
    mgr.InjectTestEntry("/project2.fsproj", dummyOptions "/project2.fsproj")
    mgr.InjectTestEntry("/project3.fsproj", dummyOptions "/project3.fsproj")
    Assert.Equal(3, mgr.CacheCount)
    Assert.True(mgr.HasCachedProject("/project1.fsproj"))
    Assert.True(mgr.HasCachedProject("/project2.fsproj"))
    Assert.True(mgr.HasCachedProject("/project3.fsproj"))

[<Fact>]
let ``Invalidate specific project preserves others`` () =
    let mgr = createManager ()
    mgr.InjectTestEntry("/x.fsproj", dummyOptions "/x.fsproj")
    mgr.InjectTestEntry("/y.fsproj", dummyOptions "/y.fsproj")
    mgr.InjectTestEntry("/z.fsproj", dummyOptions "/z.fsproj")

    mgr.Invalidate("/y.fsproj")

    Assert.Equal(2, mgr.CacheCount)
    Assert.True(mgr.HasCachedProject("/x.fsproj"))
    Assert.False(mgr.HasCachedProject("/y.fsproj"))
    Assert.True(mgr.HasCachedProject("/z.fsproj"))

[<Fact>]
let ``ResolveProjectOptions returns Error for nonexistent project`` () =
    let mgr = createManager ()
    // DTB run will throw because the working directory doesn't exist;
    // verify the error propagates without crashing the manager.
    let ex =
        Assert.ThrowsAny<exn>(fun () ->
            mgr.ResolveProjectOptions("/nonexistent/path/project.fsproj")
            |> Async.RunSynchronously
            |> ignore)
    Assert.NotNull(ex)

[<Fact>]
let ``Concurrent InjectTestEntry and Invalidate do not corrupt cache`` () =
    let mgr = createManager ()
    let iterations = 100
    let tasks =
        [| for i in 0 .. iterations - 1 do
            async {
                let path = $"/concurrent_{i}.fsproj"
                mgr.InjectTestEntry(path, dummyOptions path)
                mgr.Invalidate(path)
            } |]
    tasks |> Async.Parallel |> Async.RunSynchronously |> ignore
    // After all inject+invalidate pairs, cache should be empty
    Assert.Equal(0, mgr.CacheCount)

[<Fact>]
let ``Concurrent InjectTestEntry from multiple threads`` () =
    let mgr = createManager ()
    let count = 50
    let tasks =
        [| for i in 0 .. count - 1 do
            async {
                let path = $"/parallel_{i}.fsproj"
                mgr.InjectTestEntry(path, dummyOptions path)
            } |]
    tasks |> Async.Parallel |> Async.RunSynchronously |> ignore
    Assert.Equal(count, mgr.CacheCount)

[<Fact>]
let ``Invalidate specific during concurrent reads preserves other entries`` () =
    let mgr = createManager ()
    for i in 0 .. 9 do
        mgr.InjectTestEntry($"/stable_{i}.fsproj", dummyOptions $"/stable_{i}.fsproj")
    let tasks =
        [| for i in 0 .. 4 do
            async {
                mgr.Invalidate($"/stable_{i}.fsproj")
            }
           for i in 5 .. 9 do
            async {
                Assert.True(mgr.HasCachedProject($"/stable_{i}.fsproj"))
            } |]
    tasks |> Async.Parallel |> Async.RunSynchronously |> ignore
    // First 5 removed, last 5 remain
    Assert.Equal(5, mgr.CacheCount)

[<Fact>]
let ``ResolveProjectOptions error does not pollute cache`` () =
    let mgr = createManager ()
    mgr.InjectTestEntry("/good.fsproj", dummyOptions "/good.fsproj")
    // Attempting to resolve a nonexistent project should not affect existing cache
    try
        mgr.ResolveProjectOptions("/nonexistent/bad.fsproj")
        |> Async.RunSynchronously
        |> ignore
    with _ -> ()
    Assert.Equal(1, mgr.CacheCount)
    Assert.True(mgr.HasCachedProject("/good.fsproj"))
