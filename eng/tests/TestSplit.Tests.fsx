open System
open System.Diagnostics
open System.IO
open System.Text.RegularExpressions

let run arguments =
    let start = ProcessStartInfo(Environment.ProcessPath, RedirectStandardOutput = true, RedirectStandardError = true)
    for argument in [ "fsi"; Path.Combine(__SOURCE_DIRECTORY__, "TestSplit.fsx") ] @ arguments do
        start.ArgumentList.Add argument
    use child = Process.Start start
    let output = child.StandardOutput.ReadToEndAsync()
    let errors = child.StandardError.ReadToEndAsync()
    child.WaitForExit()
    child.ExitCode, output.Result.Trim(), errors.Result

let commands arguments =
    let code, output, errors = run arguments
    if code <> 0 then failwith $"Selection {arguments} failed: {errors}"
    if output = "" then [] else
    [ for line in output.Split('\n') do
        let matched = Regex.Match(line.TrimEnd(), @"^dotnet test (\S+) --no-build -c Release(?: (.*))?$")
        if not matched.Success then failwith $"Unexpected command: {line}"
        yield matched.Groups[1].Value, matched.Groups[2].Value ]

let project name = $"tests/{name}/{name}.fsproj"
let componentTests = project "FSharp.Compiler.ComponentTests"
let build = project "FSharp.Build.UnitTests"
let core = project "FSharp.Core.UnitTests"
let service = project "FSharp.Compiler.Service.Tests"
let scripting = project "FSharp.Compiler.Private.Scripting.UnitTests"
let server = project "FSharp.Compiler.Interactive.Server.Tests"
let legacy = "tests/fsharp/FSharpSuite.Tests.fsproj"

for platform, exclusive in [ "coreclr", server; "desktop", legacy ] do
    let aggregate = commands [ "--all"; platform ]
    let expected = [ componentTests; build; core; service; scripting; exclusive ]
    if aggregate <> (expected |> List.map (fun p -> p, "")) then
        failwith $"Incorrect unbatched selection for {platform}: {aggregate}"

    let batches = [ for batch in 1..3 -> commands [ string batch; platform ] ]
    let expectedBatches =
        [ [ componentTests; build ]
          [ componentTests; core; service; scripting ] @ (if platform = "coreclr" then [ server ] else [])
          if platform = "desktop" then [ legacy ] else [] ]
    if (batches |> List.map (List.map fst)) <> expectedBatches then
        failwith $"Incorrect batch membership for {platform}: {batches}"

    let residualFilter = batches[0].Head |> snd
    let inclusionFilter = batches[1].Head |> snd
    if not (residualFilter.StartsWith "--filter-not-namespace ")
       || not (inclusionFilter.StartsWith "--filter-namespace ")
       || residualFilter.Replace("--filter-not-namespace", "--filter-namespace") <> inclusionFilter then
        failwith $"Component filters no longer partition the suite: {batches}"
    for batch in batches do
        for p, filter in batch do
            if p <> componentTests && filter <> "" then failwith $"Unexpected filter on {p}: {filter}"
    if (batches |> List.collect (List.map fst) |> Set.ofList) <> Set.ofList expected then
        failwith $"Batched and aggregate coverage differ for {platform}"

for batch in 1..3 do
    let legacyCommands = commands [ string batch ]
    if legacyCommands <> commands [ string batch; "all" ] then
        failwith "The omitted platform must retain its all-platform behavior"
    let combined =
        commands [ string batch; "coreclr" ] @ commands [ string batch; "desktop" ] |> Set.ofList
    if Set.ofList legacyCommands <> combined then failwith $"Missing projects in all-platform batch {batch}"

for arguments in
    [ []; [ "--all" ]; [ "--all"; "all" ]; [ "--all"; "unknown" ]
      [ "0" ]; [ "4" ]; [ "-1" ]; [ "text" ]; [ "1"; "unknown" ]
      [ "1"; "coreclr"; "extra" ]; [ "--validate"; "extra" ] ] do
    let code, _, errors = run arguments
    if code = 0 || String.IsNullOrWhiteSpace errors then
        failwith $"Invalid arguments did not produce an error: {arguments}"

let code, output, errors = run [ "--validate" ]
if code <> 0 || not (output.Contains "test projects are accounted for") then
    failwith $"Manifest registration validation failed: {output} {errors}"

printfn "TestSplit command-contract tests passed."
