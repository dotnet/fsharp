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
[<InlineData("/repo", "/repo/vsintegration/src/FSharpEditor.fs", true)>]
[<InlineData("/repo", "/repo/src/FSharp.Core/Array.fs", true)>]
// Edge case: "repo" substring inside ComponentTests path should not confuse stripping
[<InlineData("/repo", "/repo/tests/FSharp.Compiler.ComponentTests/repo/Test.fs", false)>]
// Non-ComponentTests test project should fall back to FCS
[<InlineData("/repo", "/repo/tests/FSharp.Core.UnitTests/SomeTest.fs", true)>]
let ``resolveProject routes files to correct fsproj`` (repoRoot: string, filePath: string, expectFcs: bool) =
    let result = resolveProject repoRoot filePath
    let expected = if expectFcs then fcs repoRoot else componentTests repoRoot
    Assert.Equal(expected, result)
