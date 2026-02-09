module FSharpDiagServer.Tests.ResolveProjectTests

open Xunit
open FSharpDiagServer.ProjectRouting

[<Fact>]
let ``resolveProject maps FCS source file to FCS fsproj`` () =
    let repoRoot = "/repo"
    let file = "/repo/src/Compiler/SyntaxTree/SyntaxTree.fs"
    let result = resolveProject repoRoot file
    Assert.Equal(System.IO.Path.Combine(repoRoot, "src/Compiler/FSharp.Compiler.Service.fsproj"), result)

[<Fact>]
let ``resolveProject maps ComponentTests file to ComponentTests fsproj`` () =
    let repoRoot = "/repo"
    let file = "/repo/tests/FSharp.Compiler.ComponentTests/SomeTest.fs"
    let result = resolveProject repoRoot file
    Assert.Equal(
        System.IO.Path.Combine(repoRoot, "tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj"),
        result)

[<Fact>]
let ``resolveProject defaults unknown paths to FCS fsproj`` () =
    let repoRoot = "/repo"
    let file = "/repo/some/other/path/File.fs"
    let result = resolveProject repoRoot file
    Assert.Equal(System.IO.Path.Combine(repoRoot, "src/Compiler/FSharp.Compiler.Service.fsproj"), result)

[<Fact>]
let ``resolveProject handles trailing slash in repoRoot`` () =
    let repoRoot = "/repo/"
    let file = "/repo/tests/FSharp.Compiler.ComponentTests/SomeTest.fs"
    let result = resolveProject repoRoot file
    Assert.Equal(
        System.IO.Path.Combine(repoRoot, "tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj"),
        result)

[<Fact>]
let ``resolveProject handles repoRoot without trailing slash`` () =
    let repoRoot = "/repo"
    let file = "/repo/src/Compiler/Checking/Foo.fs"
    let result = resolveProject repoRoot file
    Assert.Equal(System.IO.Path.Combine(repoRoot, "src/Compiler/FSharp.Compiler.Service.fsproj"), result)

[<Fact>]
let ``resolveProject with nested ComponentTests subfolder`` () =
    let repoRoot = "/repo"
    let file = "/repo/tests/FSharp.Compiler.ComponentTests/Language/SubDir/Test.fs"
    let result = resolveProject repoRoot file
    Assert.Equal(
        System.IO.Path.Combine(repoRoot, "tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj"),
        result)
