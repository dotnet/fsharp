module FSharp.Editor.Tests.AssemblyContentProviderTests

open System
open System.IO
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open FSharp.Editor.Tests.Helpers
open FSharp.Test
open FSharp.Test.Compiler
open Xunit

let private code =
    """module Catalogue
module Patterns =
    let (|Even|Odd|) value = if value % 2 = 0 then Even else Odd
module Other =
    let (|Even|_|) value = Some value
"""

let private check (document: Document) =
    let _, results =
        document.GetFSharpParseAndCheckResultsAsync("AssemblyContentProviderTests")
        |> CancellableTask.runSynchronouslyWithoutCancellation

    Assert.Empty results.Diagnostics
    results

let private currentCases (provider: AssemblyContentProvider) results =
    provider.GetAllEntitiesInProjectAndReferencedAssemblies results
    |> Array.filter (fun symbol ->
        symbol.Symbol :? FSharpActivePatternCase
        && symbol.CleanedIdents[0] = "Catalogue")

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``repeated editor document requests reuse result and current case symbols`` transparent =
    let document =
        RoslynTestHelpers.GetFsDocument(
            code,
            customEditorOptions =
                { AdvancedOptions.Default with
                    UseTransparentCompiler = transparent
                    TransparentCompilerSnapshotReuse = true
                }
        )

    let provider = AssemblyContentProvider()
    let firstResult = check document
    let secondResult = check document
    Assert.Same(firstResult, secondResult)
    let first = currentCases provider firstResult
    let second = currentCases provider secondResult
    Assert.Equal(3, first.Length)

    Assert.Equal(
        2,
        first
        |> Array.filter (fun symbol -> Array.last symbol.CleanedIdents = "Even")
        |> Array.length
    )

    Assert.NotSame(first, second)
    Array.iter2 (fun before after -> Assert.Same(before, after)) first second

[<Fact>]
let ``edited document result cannot reuse an old case catalogue`` () =
    let document = RoslynTestHelpers.GetFsDocument code
    let provider = AssemblyContentProvider()
    let original = check document
    let before = currentCases provider original

    let edited =
        document.WithText(Microsoft.CodeAnalysis.Text.SourceText.From(code.Replace("Even", "Renamed")))

    let changed = check edited
    Assert.NotSame(original, changed)
    let after = currentCases provider changed

    Assert.Equal<string array>(
        [| "Even"; "Even"; "Odd" |],
        before
        |> Array.map (fun symbol -> Array.last symbol.CleanedIdents)
        |> Array.sort
    )

    Assert.Equal<string array>(
        [| "Odd"; "Renamed"; "Renamed" |],
        after |> Array.map (fun symbol -> Array.last symbol.CleanedIdents) |> Array.sort
    )

    Array.iter2 (fun expected actual -> Assert.Same(expected, actual)) before (currentCases provider original)

[<Fact>]
let ``concurrent first catalogue requests publish one symbol set`` () =
    let results = check (RoslynTestHelpers.GetFsDocument code)
    let provider = AssemblyContentProvider()
    use start = new ManualResetEventSlim(false)

    let requests =
        Array.init 8 (fun _ ->
            Task.Run(fun () ->
                start.Wait()
                currentCases provider results))

    start.Set()
    let outputs = Task.WhenAll(requests).GetAwaiter().GetResult()

    for actual in outputs do
        Array.iter2 (fun expected value -> Assert.Same(expected, value)) outputs[0] actual

[<MethodImpl(MethodImplOptions.NoInlining)>]
let private unrootedResult cache =
    let checker = FSharpChecker.Create()
    let file = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.fs")

    let options =
        checker.GetProjectOptionsFromCommandLineArgs(
            Path.ChangeExtension(file, ".fsproj"),
            [|
                "--noframework"
                "--targetprofile:netcore"
                "--target:library"
                for reference in TargetFrameworkUtil.currentReferences do
                    $"-r:{reference}"
            |]
        )

    let options =
        { options with
            SourceFiles = [| file |]
        }

    let _, answer =
        checker.ParseAndCheckFileInProject(file, 0, FSharp.Compiler.Text.SourceText.ofString code, options)
        |> Async.RunSynchronously

    let results =
        match answer with
        | FSharpCheckFileAnswer.Succeeded results -> results
        | FSharpCheckFileAnswer.Aborted -> failwith "Weak-key fixture was aborted"

    Assert.Empty results.Diagnostics
    cache |> Option.iter (fun provider -> currentCases provider results |> ignore)
    let weak = WeakReference(results)
    checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
    weak

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``project catalogue cache does not keep an unrooted result alive`` cacheEnabled =
    let provider = AssemblyContentProvider()
    let weak = unrootedResult (if cacheEnabled then Some provider else None)

    for _ in 1..5 do
        GC.Collect()
        GC.WaitForPendingFinalizers()
        GC.Collect()

    Assert.False weak.IsAlive
    GC.KeepAlive provider

[<Fact>]
let ``reference changes remain visible with a reused catalogue provider`` () =
    let provider = AssemblyContentProvider()

    for name, absent in [ "Before", "After"; "After", "Before" ] do
        let reference =
            FSharp $"module Referenced\nlet (|{name}|_|) value = Some value"
            |> withName $"CatalogueReference{name}"
            |> compile
            |> shouldSucceed

        let document =
            RoslynTestHelpers.GetFsDocument(code, customProjectOption = $"-r:{reference.OutputPath.Value}")

        let symbols =
            provider.GetAllEntitiesInProjectAndReferencedAssemblies(check document)

        let cases =
            symbols
            |> Array.filter (fun symbol ->
                symbol.Symbol :? FSharpActivePatternCase
                && symbol.CleanedIdents[0] = "Referenced")

        Assert.Contains(cases, fun symbol -> Array.last symbol.CleanedIdents = name)
        Assert.DoesNotContain(cases, fun symbol -> Array.last symbol.CleanedIdents = absent)
