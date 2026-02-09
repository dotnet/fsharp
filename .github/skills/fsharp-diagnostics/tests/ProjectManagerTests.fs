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
let ``Invalidate specific path removes only that project`` () =
    let mgr = createManager ()
    mgr.InjectTestEntry("/a.fsproj", dummyOptions "/a.fsproj")
    mgr.InjectTestEntry("/b.fsproj", dummyOptions "/b.fsproj")
    Assert.Equal(2, mgr.CacheCount)

    mgr.Invalidate("/a.fsproj")

    Assert.Equal(1, mgr.CacheCount)
    Assert.False(mgr.HasCachedProject("/a.fsproj"))
    Assert.True(mgr.HasCachedProject("/b.fsproj"))

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
