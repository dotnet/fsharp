namespace EmittedIL.Inlining

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Regression_RecursiveInlineMemberDependencies =

    let private parallelOptions = [ "--parallelcompilation+"; "--nowarn:75" ]
    let private sequentialOptions = [ "--parallelcompilation-"; "--nowarn:75" ]

    let private recursiveInlineMemberDependencySource =
        FSharp """
module LibraryImpl

open System.Threading.Tasks
open Microsoft.FSharp.Control

module Result =
   let ofChoice choice =
       match choice with
       | Choice1Of2 value -> Ok value
       | Choice2Of2 error -> Error error

module Async =
   let singleton value = async.Return value

type Validation<'ok, 'error> = Result<'ok, 'error>
type TaskValidation<'ok, 'error> = Task<Validation<'ok, 'error>>

type TaskValidationBuilderBase() =
   member inline _.Return(value: 'ok) : TaskValidation<'ok, 'error> =
       task { return Ok value }

   member inline _.ReturnFrom(taskValidation: TaskValidation<'ok, 'error>) : TaskValidation<'ok, 'error> =
       taskValidation

   member inline this.Bind
       (source: Validation<'okInput, 'error>, binder: 'okInput -> TaskValidation<'okOutput, 'error>)
       : TaskValidation<'okOutput, 'error> =
       task {
           let! result = this.Source source
           match result with
           | Ok value -> return! binder value
           | Error error -> return Error error
       }

   member inline this.Bind
       (source: Choice<'okInput, 'error>, binder: 'okInput -> TaskValidation<'okOutput, 'error>)
       : TaskValidation<'okOutput, 'error> =
       task {
           let! result = this.Source source
           match result with
           | Ok value -> return! binder value
           | Error error -> return Error error
       }

   member inline this.Bind
       (source: Async<'okInput>, binder: 'okInput -> TaskValidation<'okOutput, 'error>)
       : TaskValidation<'okOutput, 'error> =
       task {
           let! result = this.Source source
           match result with
           | Ok value -> return! binder value
           | Error error -> return Error error
       }

   member inline _.Delay(generator: unit -> TaskValidation<'ok, 'error>) : TaskValidation<'ok, 'error> =
       generator ()

   member inline this.Source(result: Validation<'ok, 'error>) : TaskValidation<'ok, 'error> =
       task { return result }

   member inline this.Source(choice: Choice<'ok, 'error>) : TaskValidation<'ok, 'error> =
       task {
           return
               choice
               |> Result.ofChoice
       }

   member inline this.Source(asyncComputation: Async<'ok>) : TaskValidation<'ok, 'error> =
       task {
           let! value = asyncComputation
           return Ok value
       }

type TaskValidationBuilder() =
   inherit TaskValidationBuilderBase()

let taskValidation = TaskValidationBuilder()
"""

    let private issue1565Example1 =
        FSharp """
module Issue1565Example1

let inline checkBounds f (g: 'b -> ^c) (tp: ^a) =
    let convertFrom = (^a: (static member name: string) ())
    let convertTo = (^c: (static member name : string) ())
    let value = (^a: (member Value: 'b) tp)

    if f value then
        g value
    else
        failwithf "Cannot convert from %s to %s." convertFrom convertTo

[<Struct>]
type ConverterA =
    val Value: sbyte
    new(v) = { Value = v }

    static member inline name with get () = "converter-a"

    static member inline convert(x: ConverterA): ConverterB =
        checkBounds ((>=) 0y) (byte >> ConverterB) x

and [<Struct>] ConverterB =
    val Value: byte
    new(v) = { Value = v }

    static member inline name with get () = "converter-b"
"""

    let private issue1565Example2 =
        FSharp """
module Issue1565Example2

[<System.Flags>]
type MyType =
    | Integer = 0b0001
    | Float = 0b0010

module Test =
    [<CustomEquality; NoComparison>]
    type SomeType =
        | Int of int64
        | Float of float

        override x.Equals other =
            match other with
            | :? SomeType as y ->
                match SomeType.getType x &&& SomeType.getType y with
                | MyType.Integer -> int64 x = int64 y
                | MyType.Float -> float x = float y
                | _ -> false
            | _ -> false

        override x.GetHashCode() =
            match x with
            | Int i -> hash i
            | Float f -> hash f

        static member inline op_Explicit(n: SomeType): float =
            match n with
            | Int i -> float i
            | Float f -> f

        static member inline op_Explicit(n: SomeType): int64 =
            match n with
            | Int i -> i
            | Float f -> int64 f

        static member inline getType x =
            match x with
            | Int _ -> MyType.Integer
            | Float _ -> MyType.Float
"""

    let private issue1565Example3 =
        FSharp """
module Test

type SomeType =
    | Int of int64
    | Float of float

    static member MyEquals(x, other: SomeType) =
        float x = float other

    static member inline op_Explicit(n: SomeType): float =
        match n with
        | Int i -> float i
        | Float f -> f

    static member inline op_Explicit(n: SomeType): int64 =
        match n with
        | Int i -> i
        | Float f -> int64 f
"""

    let private mkLibrary source options =
        source
        |> withOutputType CompileOutput.Library
        |> withName "Library"
        |> withOptimize
        |> withOptions options
        |> ignoreWarnings

    let private assertCompiles repeatCount source options =
        let library = mkLibrary source options

        for _i = 1 to repeatCount do
            library
            |> compile
            |> shouldSucceed
            |> ignore

    [<Fact>]
    let ``Recursive inline member dependencies compile under parallel compilation`` () =
        assertCompiles 30 recursiveInlineMemberDependencySource parallelOptions

    [<Fact>]
    let ``Recursive inline member dependencies compile under sequential compilation`` () =
        assertCompiles 1 recursiveInlineMemberDependencySource sequentialOptions

    [<Fact>]
    let ``Issue 1565 example 1 compiles under sequential compilation`` () =
        assertCompiles 1 issue1565Example1 sequentialOptions

    [<Fact>]
    let ``Issue 1565 example 2 compiles under sequential compilation`` () =
        assertCompiles 1 issue1565Example2 sequentialOptions

    [<Fact>]
    let ``Issue 1565 example 3 compiles under sequential compilation`` () =
        assertCompiles 1 issue1565Example3 sequentialOptions
