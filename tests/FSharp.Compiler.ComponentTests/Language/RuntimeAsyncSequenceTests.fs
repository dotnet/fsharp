module Language.RuntimeAsyncSequenceTests

open System.IO
open System.Text.RegularExpressions
open FSharp.Test
open FSharp.Test.Compiler
open Xunit

#if NETCOREAPP
let private source name = Path.Combine(__SOURCE_DIRECTORY__, "RuntimeAsync", name)
let private preview compilation = compilation |> withLangVersionPreview |> withFSharpCoreShippedNet
let private optimize enabled compilation = if enabled then withOptimize compilation else withOptions ["--optimize-"] compilation

[<Theory>]
[<InlineData(false, false)>]
[<InlineData(false, true)>]
[<InlineData(true, false)>]
[<InlineData(true, true)>]
let ``runtime async sequence library executes`` (crossAssembly: bool, optimized: bool) =
    let builder = FsFromPath(source "RuntimeAsyncSequenceBuilder.fs") |> preview |> optimize optimized |> asLibrary
    (if crossAssembly then
         FsFromPath(source "RuntimeAsyncSequence.fs") |> withReferences [builder]
     else
         builder |> withAdditionalSourceFile (SourceFromPath(source "RuntimeAsyncSequence.fs")))
    |> preview
    |> optimize optimized
    |> compileExeAndRun
    |> shouldSucceed

let private header = """
module M
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers
"""

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``runtime async input evaluation precedes throwing projections`` optimized =
    FSharp """
module PrefixOrder
open System
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

let events = ResizeArray<string>()
[<NoCompilerInlining; MethodImpl(MethodImplOptions.NoInlining)>]
let source () = events.Add "source"; Task.FromResult 42
type Holder = { Pair: int * int }
[<NoCompilerInlining>]
let probe (holder: Holder) =
    StateMachineHelpers.__runtimeAsyncReturn (
        let input = source()
        (fst holder.Pair, AsyncHelpers.Await input))

[<EntryPoint>]
let main _ =
    for holder, shouldThrow in
        [{ Pair = (1, 2) }, false
         { Pair = Unchecked.defaultof<int * int> }, true
         Unchecked.defaultof<Holder>, true] do
        events.Clear()
        let mutable threw = false
        try
            let result = (probe holder).GetAwaiter().GetResult()
            if result <> (1, 42) then failwith "result"
        with :? NullReferenceException -> threw <- true
        if threw <> shouldThrow then failwith "exception"
        if List.ofSeq events <> ["source"] then failwith "source must precede projection"
    0
"""
    |> preview
    |> optimize optimized
    |> compileExeAndRun
    |> shouldSucceed

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``runtime async sequence pipe input compiles`` optimized =
    FSharp """
module RuntimeAsyncSequencePipeInput

open System.Threading
open System.Threading.Tasks
open System.Collections.Generic
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers

let consume (source: IAsyncEnumerable<int>) : Task<int> =
    StateMachineHelpers.__runtimeAsyncReturn (
        let iterator = source.GetAsyncEnumerator(CancellationToken.None)
        let moved = AsyncHelpers.Await(iterator.MoveNextAsync())
        if moved then iterator.Current else -1)

let run () : Task<int> =
    StateMachineHelpers.__runtimeAsyncReturn (
        AsyncHelpers.Await (
            __runtimeAsyncSequence (fun _ -> seq { yield 42 })
            |> consume))

[<EntryPoint>]
let main _ = if run().GetAwaiter().GetResult() = 42 then 0 else 1
"""
    |> preview
    |> optimize optimized
    |> compileExeAndRun
    |> shouldSucceed

[<Theory>]
[<InlineData(false, "value")>]
[<InlineData(true, "value")>]
[<InlineData(false, "value + 1")>]
[<InlineData(true, "value + 1")>]
[<InlineData(false, "value + value")>]
[<InlineData(true, "value + value")>]
let ``runtime async sequence keeps source calls adjacent to awaits`` (optimized, resultExpression) =
    let inputs =
        CSharp "public static class AwaitInputs { public static System.Threading.Tasks.Task<int> Next() => System.Threading.Tasks.Task.FromResult(42); }"
        |> withName "AwaitInputs"
    let builder = FsFromPath(source "RuntimeAsyncSequenceBuilder.fs") |> preview |> asLibrary
    let result =
        FSharp(header + "\nlet values () = RuntimeAsyncSequenceBuilder.runtimeAsyncSeq { let! value = AwaitInputs.Next() in yield " + resultExpression + " }")
        |> withReferences [inputs; builder]
        |> preview
        |> optimize optimized
        |> compile
        |> shouldSucceed
    match result with
    | CompilationResult.Success output ->
        let _, _, il = ILChecker.verifyILAndReturnActual [] output.OutputPath.Value []
        let start = il.IndexOf("AwaitInputs::Next()")
        let finish = il.IndexOf("AsyncHelpers::Await", start)
        Assert.True(start >= 0 && finish > start)
        Assert.DoesNotContain("stfld", il.Substring(start, finish - start))
        Assert.DoesNotContain("stloc", il.Substring(start, finish - start))
        Assert.DoesNotContain("ldloc", il.Substring(start, finish - start))
    | _ -> failwith "expected compiled sequence"

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``runtime async sequence isolates ordinary sources`` optimized =
    let body = """
let factory (work: Task<int>) = __runtimeAsyncSequence(fun _ -> seq {
    for value in seq { try yield 1 with _ -> yield 2 } do
        yield value + AsyncHelpers.Await work
})
"""
    let result = FSharp(header + body) |> preview |> optimize optimized |> compile |> shouldSucceed
    result |> withMetadataReader (fun md ->
        let names = [for handle in md.MethodDefinitions -> md.GetString(md.GetMethodDefinition(handle).Name)]
        Assert.Single(names |> List.filter ((=) "MoveNextAsync")) |> ignore
        Assert.Single(names |> List.filter ((=) "DisposeAsync")) |> ignore
        for handle in md.MethodDefinitions do
            let method = md.GetMethodDefinition handle
            let name = md.GetString method.Name
            if name = "MoveNextAsync" || name = "DisposeAsync" then
                Assert.Equal(0x2000, int method.ImplAttributes &&& 0x2000)
                let mutable signature = md.GetBlobReader method.Signature
                Assert.False(signature.ReadSignatureHeader().IsGeneric)
                Assert.Equal(0, signature.ReadCompressedInteger())
            elif name = "factory" || name = "GenerateNext" then
                Assert.Equal(0, int method.ImplAttributes &&& 0x2000))
    match result with
    | CompilationResult.Success output ->
        let _, _, il = ILChecker.verifyILAndReturnActual [] output.OutputPath.Value []
        let methodBody =
            Regex.Match(il, @"(?ms)^(?<indent>[ \t]*)\.method[^\n]*MoveNextAsync\(\)[^\n]*\n\k<indent>\{.*?^\k<indent>\}").Value
        Assert.Contains("ValueTask`1<bool>", methodBody)
        Assert.Contains("AsyncHelpers::Await<int32>", methodBody)
        Assert.DoesNotContain("__runtimeAsyncSequence", il)
    | _ -> failwith "expected compiled sequence"

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``runtime async sequence handles inline producer try with`` optimized =
    let body = """
let inline test () = seq {
    try
        yield 1
        failwith "error"
    with
    | _ ->
        yield -1
}

let sequence = __runtimeAsyncSequence test

[<EntryPoint>]
let main _ =
    let iterator = sequence.GetAsyncEnumerator()
    let first = iterator.MoveNextAsync().GetAwaiter().GetResult()
    let firstValue = iterator.Current
    let second = iterator.MoveNextAsync().GetAwaiter().GetResult()
    let secondValue = iterator.Current
    let finished = iterator.MoveNextAsync().GetAwaiter().GetResult()
    iterator.DisposeAsync().GetAwaiter().GetResult()
    if not first || firstValue <> 1 || not second || secondValue <> -1 || finished then
        failwith "unexpected sequence values"
    0
"""

    FSharp(header + body)
    |> preview
    |> optimize optimized
    |> compileExeAndRun
    |> shouldSucceed

[<Theory>]
[<InlineData("preview", 3922, "let opaque (recipe: unit -> seq<int>) = __runtimeAsyncSequence recipe")>]
[<InlineData("preview", 3922, "let tail (input: seq<int>) = __runtimeAsyncSequence(fun _ -> seq { yield 1; yield! input })")>]
[<InlineData("preview", 3918, "let nested () = __runtimeAsyncSequence(fun _ -> seq { for n in seq { yield AsyncHelpers.Await(Task.FromResult 1) } do yield n })")>]
[<InlineData("9.0", 3350, "let sequence () = __runtimeAsyncSequence(fun _ -> seq { yield 1 })")>]
let ``runtime async sequence rejects unsupported entries`` (language: string, code: int, body: string) =
    FSharp(header + body)
    |> withFSharpCoreShippedNet
    |> withLangVersion language
    |> compile
    |> shouldFail
    |> withErrorCode code
#endif
