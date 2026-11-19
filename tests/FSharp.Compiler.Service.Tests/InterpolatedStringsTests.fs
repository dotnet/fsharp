module FSharp.Compiler.Service.Tests.InterpolatedStringsTests

open System
open System.IO

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Text
open FSharp.Test
open FSharp.Test.Compiler

open Xunit

let private checkFile (checker: FSharpChecker) options fileName version source =
    Range.setTestSource fileName source
    match checker.ParseAndCheckFileInProject(fileName, version, SourceText.ofString source, options)
          |> Async.RunSynchronouslyImmediate with
    | parseResults, FSharpCheckFileAnswer.Succeeded checkResults ->
        Assert.Empty parseResults.Diagnostics
        checkResults
    | _, FSharpCheckFileAnswer.Aborted -> failwith "Foreground checking aborted"

let private checkSource (checker: FSharpChecker) source =
    let fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".fsx")
    let options =
        checker.GetProjectOptionsFromCommandLineArgs(
            Path.ChangeExtension(fileName, ".fsproj"),
            mkProjectCommandLineArgsSilent (Path.ChangeExtension(fileName, ".dll"), [])
        )
    checkFile checker { options with SourceFiles = [| fileName |] } fileName 0 source

[<Fact>]
let ``Issue 20752 - multiple fallback formats check in the foreground`` () =
    let source = "let flag = true\nlet s = $\"%b{flag} %b{flag}\""
    let parseResults, checkResults =
        parseAndCheckScriptWithOptions ("Issue20752.fsx", source, [| "--langversion:preview" |])

    Assert.Empty parseResults.Diagnostics
    Assert.Empty checkResults.Diagnostics
    let locations =
        checkResults.GetFormatSpecifierLocationsAndArity()
        |> Array.map (fun (range, arity) -> tups range, arity)

    Assert.Equal<((int * int) * (int * int)) * int>(
        [| ((2, 10), (2, 12)), 1; ((2, 19), (2, 21)), 1 |],
        locations
    )

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``Issue 20752 - foreground checking works with both checker implementations`` useTransparentCompiler =
    let checker = FSharpChecker.Create(useTransparentCompiler = useTransparentCompiler)
    let results = checkSource checker "let flag = true\nlet s = $\"%b{flag} %b{flag}\""
    Assert.Empty results.Diagnostics
    Assert.Equal<((int * int) * (int * int)) * int>(
        [| ((2, 10), (2, 12)), 1; ((2, 19), (2, 21)), 1 |],
        results.GetFormatSpecifierLocationsAndArity() |> Array.map (fun (range, arity) -> tups range, arity)
    )

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``Issue 20752 - real specifier ranges are retained without generated locations`` useTransparentCompiler =
    let checker = FSharpChecker.Create(useTransparentCompiler = useTransparentCompiler)
    let cases =
        [ "let s = $\"%5d{42} %.2f{1.25}\"", [| (1, 10, 13, 1); (1, 18, 22, 1) |]
          "let s = $@\"%b{true} %b{false}\"", [| (1, 11, 13, 1); (1, 20, 22, 1) |]
          "let s = $\"%b{true}\\n%b{false}\"", [| (1, 10, 12, 1); (1, 20, 22, 1) |]
          "let s = $\"%b{true}\n%b{false}\"", [| (1, 10, 12, 1); (2, 0, 2, 1) |]
          "let s = $\"\"\"%b{true} %b{false}\"\"\"", [| (1, 12, 14, 1); (1, 21, 23, 1) |]
          "let s = $$\"\"\"%%b{{true}} %%b{{false}}\"\"\"", [| (1, 13, 16, 1); (1, 25, 28, 1) |]
          "let s = $\"{{%%}} %b{true} %b{false}\"", [| (1, 12, 14, 0); (1, 17, 19, 1); (1, 26, 28, 1) |]
          "let s = sprintf \"%b %d\" true 42", [| (1, 17, 19, 1); (1, 20, 22, 1) |]
          "let s = $\"\"\"{sprintf \"%b %d\" true 42} %b{false}\"\"\"", [| (1, 38, 40, 1); (1, 22, 24, 1); (1, 25, 27, 1) |] ]

    for source, expected in cases do
        let results = checkSource checker source
        Assert.Empty results.Diagnostics
        Assert.Equal<((int * int) * (int * int)) * int>(
            expected |> Array.map (fun (line, startColumn, endColumn, arity) -> ((line, startColumn), (line, endColumn)), arity),
            results.GetFormatSpecifierLocationsAndArity() |> Array.map (fun (range, arity) -> tups range, arity)
        )

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``Issue 20752 - genuine hole errors are not hidden or duplicated`` useTransparentCompiler =
    let checker = FSharpChecker.Create(useTransparentCompiler = useTransparentCompiler)
    for source, code, startColumn, endColumn, message in
        [ "let s = $\"%b{42} %b{true}\"", 1, 13, 15, "'bool'"
          "let s = $\"\"\"%d{\"oops\"} %b{true}\"\"\"", 1, 15, 21, "'string'"
          "let s = $\"%b{missing} %b{true}\"", 39, 13, 20, "missing" ] do
        let results = checkSource checker source
        let diagnostic = Assert.Single results.Diagnostics
        Assert.Equal(code, diagnostic.ErrorNumber)
        Assert.Equal(FSharpDiagnosticSeverity.Error, diagnostic.Severity)
        assertRange (1, startColumn) (1, endColumn) diagnostic.Range
        Assert.Contains(message, diagnostic.Message)

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``Issue 20752 - incremental holes reuse the foreground checker`` useTransparentCompiler =
    let checker = FSharpChecker.Create(useTransparentCompiler = useTransparentCompiler)
    let fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".fsx")
    let options =
        checker.GetProjectOptionsFromCommandLineArgs(
            Path.ChangeExtension(fileName, ".fsproj"),
            mkProjectCommandLineArgsSilent (Path.ChangeExtension(fileName, ".dll"), [])
        )
    let options = { options with SourceFiles = [| fileName |] }
    for version, count in [ 1, 1; 2, 2; 3, 3; 4, 1 ] do
        let source = "let flag = true\nlet s = $\"" + String.concat " " (List.replicate count "%b{flag}") + "\""
        let results = checkFile checker options fileName version source
        Assert.Empty results.Diagnostics
        Assert.Equal<((int * int) * (int * int)) * int>(
            [| for i in 0 .. count - 1 -> ((2, 10 + 9 * i), (2, 12 + 9 * i)), 1 |],
            results.GetFormatSpecifierLocationsAndArity() |> Array.map (fun (range, arity) -> tups range, arity)
        )

[<Fact>]
let ``Issue 20752 - foreground contexts and fallback fast-path combinations`` () =
    let sources =
        [ "open System\nlet log (msg: string) = Console.WriteLine msg\nlet flag = true\nlog $\"%b{flag}\"\nlog $\"%b{flag} %b{flag}\""
          "let flag = true\nlet s: string = $\"%b{flag} %b{flag}\""
          "let s = @$\"%b{true} %b{false}\""
          "let logInferred msg = printfn \"%s\" msg\nlogInferred $\"%b{true} %b{false}\""
          "let show x = $\"%A{x} %b{true}\"\nlet a = show 42\nlet b = show [1; 2]"
          "let s = $\"\"\"{$\"%b{true} %b{false}\"} %b{true}\"\"\""
          "let s = $\"\"\nlet percent = $\"%%\"\nlet one = $\"%b{true}\"\nlet two = $\"%b{true}%b{false}\"\nlet three = $\"%b{true}%b{false}%b{true}\"" ]
    for source in sources do
        let parseResults, results = parseAndCheckScriptPreview (Guid.NewGuid().ToString() + ".fsx", source)
        Assert.Empty parseResults.Diagnostics
        Assert.Empty results.Diagnostics

    let source =
        [| for fallback in [ "%b{true}"; "%A{[1; 2]}"; "%f{1.25}"; "%5d{42}"; "%.2f{1.25}" ] do
               for fast in [ "%s{\"x\"}"; "%c{'x'}"; "%d{42}"; "%i{42}"; "%M{1.25M}" ] do
                   for parts in [ [fallback; fast]; [fast; fallback]; [fast; fallback; fast] ] do
                       yield "let _ = $\"\"\"" + String.concat " " parts + "\"\"\"" |]
        |> String.concat "\n"
    let parseResults, results = parseAndCheckScriptPreview (Guid.NewGuid().ToString() + ".fsx", source)
    Assert.Empty parseResults.Diagnostics
    Assert.Empty results.Diagnostics

[<Fact>]
let ``Issue 20752 - ordered cross-file values agree with project checking`` () =
    let consumer = "module Consumer\nlet s = $\"%b{Flags.flag} %b{Flags.flag}\""
    let options =
        createProjectOptionsFromNamedSources
            [ "Flags.fs", "module Flags\nlet flag = true"; "Consumer.fs", consumer ]
            []
    let parseResults, results = parseAndCheckFile options.SourceFiles[1] consumer options
    Assert.Empty parseResults.Diagnostics
    Assert.Empty results.Diagnostics
    let projectResults = checker.ParseAndCheckProject(options) |> Async.RunSynchronouslyImmediate
    Assert.Empty projectResults.Diagnostics
    FSharp "module Flags\nlet flag = true"
    |> withAdditionalSourceFile (SourceCodeFileKind.Create("Consumer.fs", consumer))
    |> asLibrary
    |> withLangVersionPreview
    |> compile
    |> shouldSucceed

[<Fact>]
let ``Issue 20752 - actual assembly references check and run`` () =
    let library = FSharp "module Flags\nlet flag = true" |> asLibrary |> withName "Issue20752Flags"
    let compiled = library |> compile |> shouldSucceed
    let path = compiled.OutputPath |> Option.defaultWith (fun () -> failwith "Flags library produced no assembly")
    Assert.True(File.Exists path)
    let consumer = "module Consumer\nlet s = $\"%b{Flags.flag} %b{Flags.flag}\""
    let options = createProjectOptionsFromNamedSources [ "Consumer.fs", consumer ] [ "-r:" + path ]
    let parseResults, results = parseAndCheckFile options.SourceFiles[0] consumer options
    Assert.Empty parseResults.Diagnostics
    Assert.Empty results.Diagnostics
    FSharp (consumer + "\nif s <> \"true true\" then failwith s")
    |> withOptions [ "-r:" + path ]
    |> withOutputDirectory (Some (DirectoryInfo (Path.GetDirectoryName path)))
    |> withLangVersionPreview
    |> compileExeAndRun
    |> shouldSucceed
