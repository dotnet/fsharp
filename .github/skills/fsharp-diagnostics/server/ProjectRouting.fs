module FSharpDiagServer.ProjectRouting

open System.IO

/// Maps a source file path to the fsproj that owns it.
/// Falls back to FSharp.Compiler.Service for any unrecognized path.
let resolveProject (repoRoot: string) (filePath: string) =
    let root = repoRoot.TrimEnd('/') + "/"

    let rel =
        if filePath.StartsWith(root, System.StringComparison.Ordinal) then
            filePath.Substring(root.Length)
        else
            filePath

    if rel.StartsWith("tests/FSharp.Compiler.ComponentTests/", System.StringComparison.Ordinal) then
        Path.Combine(repoRoot, "tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj")
    else
        Path.Combine(repoRoot, "src/Compiler/FSharp.Compiler.Service.fsproj")
