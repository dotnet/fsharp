namespace FSharp.Compiler.Scripting.UnitTests

open System
open FSharp.Compiler.Interactive.Shell
open FSharp.Test.ScriptHelpers
open Xunit

type FSharpPlusRegressionTests() =

    [<Fact>]
    member _.``FSharpPlus Regression Test 1``() =
        use script = new FSharpScript()
        let code = """
#r "nuget: FSharpPlus, 1.8.0"

open FSharpPlus

let y: seq<_> = monad.plus {
    for x in seq [1..3] do
        for y in seq [10; 20] do
            return (x, y)
}
"""
        let result, errors = script.Eval(code)
        if errors.Length > 0 then
            let msg = errors |> Array.map (fun e -> e.Message) |> String.concat "\n"
            Assert.Fail($"Script failed with errors:\n{msg}")
        
        match result with
        | Ok(_) -> ()
        | Error(ex) -> Assert.Fail($"Script failed with exception: {ex}")

    [<Fact>]
    member _.``FSharpPlus Regression Test 2``() =
        use script = new FSharpScript()
        let code = """
#r "nuget: FSharpPlus, 1.8.0"

open FSharpPlus
open FSharpPlus.Data

type AsyncResult<'T, 'E> = ResultT<Async<Result<'T, 'E>>>

type ResultTBuilder<'``monad<Result<'t, 'e>>``>() =
  inherit Builder<ResultT<'``monad<Result<'t, 'e>>``>>()

  member inline _.For (x: ResultT<'``Monad<Result<'T, 'E>>``>, f: 'T -> ResultT<'``Monad<Result<'U, 'E>>``>) = x >>= f : ResultT<'``Monad<Result<'U, 'E>>``>

  [<CustomOperation("lift", IsLikeZip=true)>]
  member inline _.Lift (x: ResultT<'``Monad<Result<'T, 'E>>``>, m: '``Monad<'U>``, f: 'T -> 'U -> 'V) =
    x >>= fun a ->
      lift m |> ResultT.bind (fun b ->
        result (f a b) : ResultT<'``Monad<Result<'V, 'E>>``>)

let resultT<'``Monad<Result<'T, 'E>>``> = new ResultTBuilder<'``Monad<Result<'T, 'E>>``>()

let sampleWorkflow2 =
  monad {
    let! x = Some 1
    let! y = Some 2
    return x + y
  }

let test2 () =
  resultT {
    let! x = ResultT.hoist (Ok 1)
    lift y in sampleWorkflow2
    return x + y
  }
"""
        let result, errors = script.Eval(code)
        if errors.Length > 0 then
            let msg = errors |> Array.map (fun e -> e.Message) |> String.concat "\n"
            Assert.Fail($"Script failed with errors:\n{msg}")
        
        match result with
        | Ok(_) -> ()
        | Error(ex) -> Assert.Fail($"Script failed with exception: {ex}")
