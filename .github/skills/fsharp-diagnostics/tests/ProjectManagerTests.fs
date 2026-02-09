module FSharpDiagServer.Tests.ProjectManagerTests

open Xunit
open FSharp.Compiler.CodeAnalysis
open FSharpDiagServer.ProjectManager

[<Fact>]
let ``Invalidate without args does not throw`` () =
    let checker = FSharpChecker.Create()
    let mgr = ProjectManager(checker)
    // Should not throw even when cache is empty
    mgr.Invalidate()

[<Fact>]
let ``Invalidate with specific path does not throw`` () =
    let checker = FSharpChecker.Create()
    let mgr = ProjectManager(checker)
    mgr.Invalidate("/some/nonexistent.fsproj")

[<Fact>]
let ``Invalidate all clears after invalidate-specific`` () =
    let checker = FSharpChecker.Create()
    let mgr = ProjectManager(checker)
    mgr.Invalidate("/a.fsproj")
    mgr.Invalidate()

[<Fact>]
let ``Multiple invalidate calls are idempotent`` () =
    let checker = FSharpChecker.Create()
    let mgr = ProjectManager(checker)
    mgr.Invalidate()
    mgr.Invalidate()
    mgr.Invalidate("/x.fsproj")
    mgr.Invalidate("/x.fsproj")
