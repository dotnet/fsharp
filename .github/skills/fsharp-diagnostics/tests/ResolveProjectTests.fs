module FSharpDiagServer.Tests.ResolveProjectTests

open Xunit
open FSharpDiagServer.ProjectRouting

let private fcs root =
    System.IO.Path.Combine(root, "src/Compiler/FSharp.Compiler.Service.fsproj")

let private componentTests root =
    System.IO.Path.Combine(root, "tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj")

[<Theory>]
[<InlineData("/repo", "/repo/src/Compiler/SyntaxTree/SyntaxTree.fs", true)>]
[<InlineData("/repo", "/repo/src/Compiler/Checking/Foo.fs", true)>]
[<InlineData("/repo", "/repo/some/other/path/File.fs", true)>]
[<InlineData("/repo/", "/repo/tests/FSharp.Compiler.ComponentTests/SomeTest.fs", false)>]
[<InlineData("/repo", "/repo/tests/FSharp.Compiler.ComponentTests/SomeTest.fs", false)>]
[<InlineData("/repo", "/repo/tests/FSharp.Compiler.ComponentTests/Language/SubDir/Test.fs", false)>]
[<InlineData("/repo", "/other/path/File.fs", true)>]
let ``resolveProject routes files to correct fsproj`` (repoRoot: string, filePath: string, expectFcs: bool) =
    let result = resolveProject repoRoot filePath
    let expected = if expectFcs then fcs repoRoot else componentTests repoRoot
    Assert.Equal(expected, result)

[<Fact>]
let ``resolveProject with trailing slash on repoRoot matches ComponentTests`` () =
    let result = resolveProject "/repo/" "/repo/tests/FSharp.Compiler.ComponentTests/X.fs"
    Assert.Equal(componentTests "/repo/", result)

[<Fact>]
let ``resolveProject defaults to FCS for vsintegration path`` () =
    let result = resolveProject "/repo" "/repo/vsintegration/src/FSharpEditor.fs"
    Assert.Equal(fcs "/repo", result)

[<Fact>]
let ``resolveProject defaults to FCS for FSharp.Core path`` () =
    let result = resolveProject "/repo" "/repo/src/FSharp.Core/Array.fs"
    Assert.Equal(fcs "/repo", result)

[<Fact>]
let ``resolveProject with repoRoot substring in path does not double-strip`` () =
    // Ensure that if repoRoot appears as a substring later in the path, it isn't incorrectly stripped
    let result = resolveProject "/repo" "/repo/tests/FSharp.Compiler.ComponentTests/repo/Test.fs"
    Assert.Equal(componentTests "/repo", result)
