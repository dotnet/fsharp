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
open System.Threading.Tasks
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers
"""

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``runtime async sequence isolates ordinary sources`` optimized =
    let body = """
let factory (work: Task<int>) = __runtimeAsyncSequence(fun () -> seq {
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
                Assert.Equal(0x2008, int method.ImplAttributes)
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
[<InlineData("preview", 3920, "let opaque (recipe: unit -> seq<int>) = __runtimeAsyncSequence recipe")>]
[<InlineData("preview", 3920, "let tail (input: seq<int>) = __runtimeAsyncSequence(fun () -> seq { yield 1; yield! input })")>]
[<InlineData("preview", 3920, "let handling () = __runtimeAsyncSequence(fun () -> seq { try yield 1 with _ -> yield 2 })")>]
[<InlineData("preview", 3916, "let nested () = __runtimeAsyncSequence(fun () -> seq { for n in seq { yield AsyncHelpers.Await(Task.FromResult 1) } do yield n })")>]
[<InlineData("9.0", 3350, "let sequence () = __runtimeAsyncSequence(fun () -> seq { yield 1 })")>]
let ``runtime async sequence rejects unsupported entries`` (language: string, code: int, body: string) =
    FSharp(header + body)
    |> withFSharpCoreShippedNet
    |> withLangVersion language
    |> compile
    |> shouldFail
    |> withErrorCode code
#endif
