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
module Values =
    let Duplicate = 1
    let Unique = 2
module Other =
    let Duplicate = 3
"""

let private check (document: Document) =
    let _, results =
        document.GetFSharpParseAndCheckResultsAsync("AssemblyContentProviderTests")
        |> CancellableTask.runSynchronouslyWithoutCancellation

    Assert.Empty results.Diagnostics
    results

let private valuesIn root (provider: AssemblyContentProvider) results =
    provider.GetAllEntitiesInProjectAndReferencedAssemblies results
    |> Array.filter (fun symbol -> symbol.Symbol :? FSharpMemberOrFunctionOrValue && symbol.CleanedIdents[0] = root)

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``repeated editor document requests reuse result and current value symbols`` transparent =
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
    let first = valuesIn "Catalogue" provider firstResult
    let second = valuesIn "Catalogue" provider secondResult
    Assert.Equal(3, first.Length)

    Assert.Equal(
        2,
        first
        |> Array.filter (fun symbol -> Array.last symbol.CleanedIdents = "Duplicate")
        |> Array.length
    )

    Assert.NotSame(
        provider.GetAllEntitiesInProjectAndReferencedAssemblies firstResult,
        provider.GetAllEntitiesInProjectAndReferencedAssemblies secondResult
    )

    Array.iter2 (fun before after -> Assert.Same(before, after)) first second

[<Fact>]
let ``edited document result cannot reuse an old value catalogue`` () =
    let document = RoslynTestHelpers.GetFsDocument code
    let provider = AssemblyContentProvider()
    let original = check document
    let before = valuesIn "Catalogue" provider original

    let edited =
        document.WithText(SourceText.From(code.Replace("Duplicate", "Renamed")))

    let changed = check edited
    Assert.NotSame(original, changed)
    let after = valuesIn "Catalogue" provider changed

    Assert.Equal<string array>(
        [| "Duplicate"; "Duplicate"; "Unique" |],
        before
        |> Array.map (fun symbol -> Array.last symbol.CleanedIdents)
        |> Array.sort
    )

    Assert.Equal<string array>(
        [| "Renamed"; "Renamed"; "Unique" |],
        after |> Array.map (fun symbol -> Array.last symbol.CleanedIdents) |> Array.sort
    )

    Array.iter2 (fun expected actual -> Assert.Same(expected, actual)) before (valuesIn "Catalogue" provider original)

[<Fact>]
let ``concurrent first catalogue requests publish one symbol set`` () =
    let results = check (RoslynTestHelpers.GetFsDocument code)
    let provider = AssemblyContentProvider()
    use start = new ManualResetEventSlim(false)

    let requests =
        Array.init 8 (fun _ ->
            Task.Run(fun () ->
                start.Wait()
                valuesIn "Catalogue" provider results))

    start.Set()
    let outputs = Task.WhenAll(requests).GetAwaiter().GetResult()
    Assert.Equal(3, outputs[0].Length)

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

    cache
    |> ValueOption.iter (fun provider -> valuesIn "Catalogue" provider results |> ignore)

    let weak = WeakReference(results)
    checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
    weak

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``project catalogue cache does not keep an unrooted result alive`` cacheEnabled =
    let provider = AssemblyContentProvider()
    let weak = unrootedResult (if cacheEnabled then ValueSome provider else ValueNone)

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
            FSharp $"module Referenced\nlet {name} = 1"
            |> withName $"CatalogueReference{name}"
            |> compile
            |> shouldSucceed

        let document =
            RoslynTestHelpers.GetFsDocument(code, customProjectOption = $"-r:{reference.OutputPath.Value}")

        let values = valuesIn "Referenced" provider (check document)
        Assert.Contains(values, fun symbol -> Array.last symbol.CleanedIdents = name)
        Assert.DoesNotContain(values, fun symbol -> Array.last symbol.CleanedIdents = absent)
