/// Test split table for parallel CI.
/// Edit the batch assignments below, then run:
///   dotnet fsi eng/tests/TestSplit.fsx <batchNumber> [desktop|coreclr]
/// to get the dotnet test commands for that batch.
/// The platform argument controls which projects are included (default: all).
///
/// Validation mode (used in CI to catch unregistered test projects):
///   dotnet fsi eng/tests/TestSplit.fsx --validate

let totalBatches = 3
let residualBatch = 1 // uses negation filter; catches unlisted atoms + future namespaces

// MTP --filter-namespace uses starts-with matching on the test namespace.
// Unlisted atoms go to the residual batch automatically via --filter-not-namespace.

let componentTestsAtoms =
    [// atom                    batch
        "CompilerOptions",      1
        "CompilerService",      1
        "Conformance",          1
        "ConstraintSolver",     1
        "Debugger",             1
        "FSharpChecker",        1
        "Import",               1
        "Miscellaneous",        1
        "Scripting",            1
        "Signatures",           1
        "TypeChecks",           1

        "CompilerDirectives",   2
        "Diagnostics",          2
        "EmittedIL",            2
        "ErrorMessages",        2
        "Globalization",        2
        "Interop",              2
        "InteractiveSession",   2
        "Language",             2
        "Libraries",            2
        "XmlComments",          2

        // Batch 3 is reserved for FSharpSuite.Tests (desktop-only); no component atoms.
    ]

// Platform tags: "all" = both desktop and coreclr, "desktop" = net472 only
let otherProjects =
    [// project path                                                                         batch  platform
        "tests/FSharp.Build.UnitTests/FSharp.Build.UnitTests.fsproj",                        1,     "all"
        "tests/FSharp.Core.UnitTests/FSharp.Core.UnitTests.fsproj",                          2,     "all"
        "tests/FSharp.Compiler.Service.Tests/FSharp.Compiler.Service.Tests.fsproj",          2,     "all"
        "tests/FSharp.Compiler.Private.Scripting.UnitTests/FSharp.Compiler.Private.Scripting.UnitTests.fsproj", 2, "all"
        "tests/fsharp/FSharpSuite.Tests.fsproj",                                             3,     "desktop"
    ]

// ── excluded projects ──
// Projects under tests/ that are intentionally not run in batched CI.
// If you add a new test project under tests/, either add it to otherProjects above
// or to one of these exclusion lists. Run --validate to check.

let componentTests = "tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj"

// Directory prefixes: any .fsproj under these paths is excluded from validation.
let excludedPrefixes =
    [ "tests/benchmarks/"           // benchmark suites, run separately
      "tests/AheadOfTime/"          // AOT/trimming test apps, run separately
      "tests/EndToEndBuildTests/"   // E2E type-provider build tests, run separately
      "tests/service/data/"         // test data projects consumed by other tests
      "tests/projects/"             // test helper projects (e.g. CompilerCompat)
      "tests/fsharp/core/"          // legacy test data
    ]

// Individual projects excluded by exact path.
let excludedProjects =
    [ "tests/FSharp.Test.Utilities/FSharp.Test.Utilities.fsproj"  // shared test utility library, not a test project
      "tests/FSharp.Compiler.LanguageServer.Tests/FSharp.Compiler.LanguageServer.Tests.fsproj"  // LSP tests, run in a dedicated CI job
    ]

// ── validation mode ──

let isValidateMode =
    match fsi.CommandLineArgs with
    | [| _; "--validate" |] -> true
    | _ -> false

if isValidateMode then
    let repoRoot =
        let scriptDir = System.IO.Path.GetDirectoryName(__SOURCE_DIRECTORY__)
        System.IO.Path.GetFullPath(System.IO.Path.Combine(scriptDir, ".."))

    let allProjects =
        System.IO.Directory.GetFiles(System.IO.Path.Combine(repoRoot, "tests"), "*.fsproj", System.IO.SearchOption.AllDirectories)
        |> Array.map (fun p -> p.Substring(repoRoot.Length + 1).Replace("\\", "/"))
        |> Array.sort

    let knownProjects =
        set [
            yield componentTests
            for (proj, _, _) in otherProjects do yield proj
            yield! excludedProjects
        ]

    let isExcludedByPrefix (proj: string) =
        excludedPrefixes |> List.exists proj.StartsWith

    let unaccounted =
        allProjects
        |> Array.filter (fun p -> not (knownProjects.Contains p) && not (isExcludedByPrefix p))

    if unaccounted.Length > 0 then
        eprintfn "ERROR: The following test projects are not registered in TestSplit.fsx."
        eprintfn "Add them to otherProjects (to run in batched CI), excludedProjects, or excludedPrefixes."
        for p in unaccounted do
            eprintfn "  %s" p
        exit 1
    else
        printfn "All %d test projects are accounted for." allProjects.Length
        exit 0

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
    | _ -> failwith "Usage: dotnet fsi eng/tests/TestSplit.fsx <batchNumber> [desktop|coreclr] | --validate"

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

let batchHasComponentAtoms =
    componentTestsAtoms |> List.exists (fun (_, b) -> b = batch)

let filterArgs =
    if batch = residualBatch then
        let atoms = otherBatchesAtoms |> String.concat " "
        $"--filter-not-namespace {atoms}"
    else
        let atoms = atomsForBatch batch |> String.concat " "
        $"--filter-namespace {atoms}"

// Output format contract: each line must be "dotnet test <project> --no-build -c Release [filterargs]".
// Consumers: Build.ps1 parses via regex, build.sh parses via sed. Keep in sync if changing format.
if batchHasComponentAtoms || batch = residualBatch then
    printfn $"dotnet test {componentTests} --no-build -c Release {filterArgs}"

for (proj, _, tag) in otherProjects |> List.filter (fun (_, b, tag) -> b = batch && matchesPlatform tag) do
    printfn $"dotnet test {proj} --no-build -c Release"
