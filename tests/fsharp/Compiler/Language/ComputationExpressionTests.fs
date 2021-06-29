namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test.Utilities
open FSharp.Compiler.SourceCodeServices

[<TestFixture>]
module ComputationExpressionTests =

    let ``complex CE with source member and applicatives`` ceUsage =
        sprintf """
module Code
type ResultBuilder() =
    member __.Return value = Ok value
    member __.ReturnFrom (result: Result<_,_>) = result
    member x.Zero() = x.Return ()
    member __.Bind(r: Result<'t,_>, binder: 't -> Result<_,_>) = match r with | Ok r' -> binder r' | Error e -> e
    member __.Delay(gen: unit -> Result<_,_>) = gen
    member __.Run(gen: unit -> Result<_,_>) = gen()
    member _.BindReturn(x: Result<'t,_>, f) = Result.map f x
    member inline _.Source(result : Result<_,_>) : Result<_,_> = result

let result = ResultBuilder()

module Result =
    let zip x1 x2 =
        match x1,x2 with
        | Ok x1res, Ok x2res -> Ok (x1res, x2res)
        | Error e, _ -> Error e
        | _, Error e -> Error e

    let ofChoice c =
        match c with
        | Choice1Of2 x -> Ok x
        | Choice2Of2 x -> Error x

    let fold onOk onError r =
        match r with
        | Ok x -> onOk x
        | Error y -> onError y

module Async =
    let inline singleton value = value |> async.Return
    let inline bind f x = async.Bind(x, f)
    let inline map f x = x |> bind (f >> singleton)
    let zip a1 a2 = async {
        let! r1 = a1
        let! r2 = a2
        return r1,r2
    }

module AsyncResult =
    let zip x1 x2 =
        Async.zip x1 x2
        |> Async.map(fun (r1, r2) -> Result.zip r1 r2)

    let foldResult onSuccess onError ar =
        Async.map (Result.fold onSuccess onError) ar

type AsyncResultBuilder() =

    member __.Return (value: 'T) : Async<Result<'T, 'TError>> =
      async.Return <| result.Return value

    member inline __.ReturnFrom
        (asyncResult: Async<Result<'T, 'TError>>)
        : Async<Result<'T, 'TError>> =
      asyncResult

    member __.Zero () : Async<Result<unit, 'TError>> =
      async.Return <| result.Zero ()

    member inline __.Bind
        (asyncResult: Async<Result<'T, 'TError>>,
         binder: 'T -> Async<Result<'U, 'TError>>)
        : Async<Result<'U, 'TError>> =
      async {
        let! result = asyncResult
        match result with
        | Ok x -> return! binder x
        | Error x -> return Error x
      }

    member __.Delay
        (generator: unit -> Async<Result<'T, 'TError>>)
        : Async<Result<'T, 'TError>> =
      async.Delay generator

    member this.Combine
        (computation1: Async<Result<unit, 'TError>>,
         computation2: Async<Result<'U, 'TError>>)
        : Async<Result<'U, 'TError>> =
      this.Bind(computation1, fun () -> computation2)

    member __.TryWith
        (computation: Async<Result<'T, 'TError>>,
         handler: System.Exception -> Async<Result<'T, 'TError>>)
        : Async<Result<'T, 'TError>> =
      async.TryWith(computation, handler)

    member __.TryFinally
        (computation: Async<Result<'T, 'TError>>,
         compensation: unit -> unit)
        : Async<Result<'T, 'TError>> =
      async.TryFinally(computation, compensation)

    member __.Using
        (resource: 'T when 'T :> System.IDisposable,
         binder: 'T -> Async<Result<'U, 'TError>>)
        : Async<Result<'U, 'TError>> =
      async.Using(resource, binder)

    member this.While
        (guard: unit -> bool, computation: Async<Result<unit, 'TError>>)
        : Async<Result<unit, 'TError>> =
      if not <| guard () then this.Zero ()
      else this.Bind(computation, fun () -> this.While (guard, computation))

    member this.For
        (sequence: #seq<'T>, binder: 'T -> Async<Result<unit, 'TError>>)
        : Async<Result<unit, 'TError>> =
      this.Using(sequence.GetEnumerator (), fun enum ->
        this.While(enum.MoveNext,
          this.Delay(fun () -> binder enum.Current)))

    member inline __.BindReturn(x: Async<Result<'T,'U>>, f) = async.Bind(x, fun r -> Result.map f r |> async.Return)
    member inline __.MergeSources(t1: Async<Result<'T,'U>>, t2: Async<Result<'T1,'U>>) =
        AsyncResult.zip t1 t2

    member inline _.Source(result : Async<Result<_,_>>) : Async<Result<_,_>> = result

[<AutoOpen>]
module ARExts =
    type AsyncResultBuilder with
        /// <summary>
        /// Needed to allow `for..in` and `for..do` functionality
        /// </summary>
        member inline __.Source(s: #seq<_>) = s

        /// <summary>
        /// Method lets us transform data types into our internal representation.
        /// </summary>
        member inline _.Source(result : Result<_,_>) : Async<Result<_,_>> = Async.singleton result

        /// <summary>
        /// Method lets us transform data types into our internal representation.
        /// </summary>
        member inline _.Source(choice : Choice<_,_>) : Async<Result<_,_>> =
          choice
          |> Result.ofChoice
          |> Async.singleton

        /// <summary>
        /// Method lets us transform data types into our internal representation.
        /// </summary>
        member inline __.Source(asyncComputation : Async<_>) : Async<Result<_,_>> = asyncComputation |> Async.map Ok

let asyncResult = AsyncResultBuilder()

%s"""     ceUsage

    [<Test>]
    let ``do-bang can be used with nested CE expressions``() =
        let code = ``complex CE with source member and applicatives`` """
asyncResult {
    let! something = asyncResult { return 5 }
    do! asyncResult {
        return ()
    }
    return something
}
|> AsyncResult.foldResult id (fun (_err: string) -> 10)
|> Async.RunSynchronously
|> printfn "%d"
"""
        CompilerAssert.Pass code

    [<Test>]
    let ``match-bang should apply source transformations to its inputs`` () =
        let code = ``complex CE with source member and applicatives`` """
asyncResult {
    // if the source transformation is not applied, the match will not work,
    // because match! is only defined in terms of let!, and the only
    // bind overload provided takes AsyncResult as its input.
    match! Ok 5 with
    | 5 -> return "ok"
    | n -> return! (Error (sprintf "boo %d" n))
}
|> AsyncResult.foldResult id (fun (err: string) -> err)
|> Async.RunSynchronously
|> printfn "%s"
"""
        CompilerAssert.Pass code
