module FSharpDiagServer.ProjectRouting

open System.IO

let resolveProject (repoRoot: string) (filePath: string) =
    let rel = filePath.Replace(repoRoot.TrimEnd('/') + "/", "")

    if rel.StartsWith("tests/FSharp.Compiler.ComponentTests/") then
        Path.Combine(repoRoot, "tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj")
    else
        Path.Combine(repoRoot, "src/Compiler/FSharp.Compiler.Service.fsproj")
