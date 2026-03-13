/// Test split table for parallel CI.
/// Edit the batch assignments below, then run:
///   dotnet fsi eng/tests/TestSplit.fsx <batchNumber> [desktop|coreclr]
/// to get the dotnet test commands for that batch.
/// The platform argument controls which projects are included (default: all).

let totalBatches = 3
let residualBatch = 3 // uses negation filter; catches unlisted atoms + future namespaces

// MTP --filter-namespace uses starts-with matching on the test namespace.
// Unlisted atoms go to the residual batch automatically via --filter-not-namespace.

let componentTestsAtoms =
    [// atom                    batch
        "CompilerDirectives",   1
        "CompilerService",      1
        "ErrorMessages",        1
        "FSharpChecker",        1
        "Import",               1
        "Language",             1
        "Miscellaneous",        1
        "XmlComments",          1

        "EmittedIL",            2
        "Interop",              2
        "Libraries",            2
        "Globalization",        2
        "InteractiveSession",   2

        "CompilerOptions",      3
        "Conformance",          3
        "Diagnostics",          3
        "Signatures",           3
        "ConstraintSolver",     3
        "Debugger",             3
        "Scripting",            3
        "TypeChecks",           3
    ]

// Platform tags: "all" = both desktop and coreclr, "desktop" = net472 only
let otherProjects =
    [// project path                                                                         batch  platform
        "tests/FSharp.Core.UnitTests/FSharp.Core.UnitTests.fsproj",                          1,     "all"
        "tests/FSharp.Compiler.Service.Tests/FSharp.Compiler.Service.Tests.fsproj",          2,     "all"
        "tests/FSharp.Compiler.Private.Scripting.UnitTests/FSharp.Compiler.Private.Scripting.UnitTests.fsproj", 2, "all"
        "tests/FSharp.Build.UnitTests/FSharp.Build.UnitTests.fsproj",                        3,     "all"
        "tests/fsharp/FSharpSuite.Tests.fsproj",                                             3,     "desktop"
    ]

// ── filter generation ──

let batch, platform =
    match fsi.CommandLineArgs with
    | [| _; n |] ->
        let v = int n
        if v < 1 || v > totalBatches then failwith $"Batch number must be between 1 and {totalBatches}, got {v}"
        v, "all"
    | [| _; n; p |] ->
        let v = int n
        if v < 1 || v > totalBatches then failwith $"Batch number must be between 1 and {totalBatches}, got {v}"
        v, p
    | _ -> failwith "Usage: dotnet fsi eng/tests/TestSplit.fsx <batchNumber> [desktop|coreclr]"

let matchesPlatform tag =
    tag = "all" || tag = platform || platform = "all"

let expandAtom atom =
    [ atom
      $"FSharp.Compiler.ComponentTests.{atom}"
      $"ComponentTests.{atom}" ]

let atomsForBatch b =
    componentTestsAtoms
    |> List.filter (fun (_, ba) -> ba = b)
    |> List.collect (fst >> expandAtom)
    |> List.distinct
    |> List.sort

let otherBatchesAtoms =
    componentTestsAtoms
    |> List.filter (fun (_, b) -> b <> batch)
    |> List.collect (fst >> expandAtom)
    |> List.distinct
    |> List.sort

let filterArgs =
    if batch = residualBatch then
        let atoms = otherBatchesAtoms |> String.concat " "
        $"--filter-not-namespace {atoms}"
    else
        let atoms = atomsForBatch batch |> String.concat " "
        $"--filter-namespace {atoms}"

let componentTests = "tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj"

// Output format contract: each line must be "dotnet test <project> --no-build -c Release [filterargs]".
// Consumers: Build.ps1 parses via regex, build.sh parses via sed. Keep in sync if changing format.
printfn $"dotnet test {componentTests} --no-build -c Release {filterArgs}"

for (proj, _, tag) in otherProjects |> List.filter (fun (_, b, tag) -> b = batch && matchesPlatform tag) do
    printfn $"dotnet test {proj} --no-build -c Release"
