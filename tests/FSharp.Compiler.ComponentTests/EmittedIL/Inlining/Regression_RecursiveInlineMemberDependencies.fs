namespace EmittedIL.Inlining

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Regression_ParallelCrossAssemblyInlineOverloads =

    [<Fact>]
    let ``Cross-assembly overloaded inline Source members compile and run`` () =
       let library =
           (
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

   member inline _.Bind
       (source: TaskValidation<'okInput, 'error>, binder: 'okInput -> TaskValidation<'okOutput, 'error>)
       : TaskValidation<'okOutput, 'error> =
       task {
           let! result = source
           match result with
           | Ok value -> return! binder value
           | Error error -> return Error error
       }

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
               |> withAdditionalSourceFile (SourceCodeFileKind.Create("Library.Support.fs", """
module LibraryImplSupport

let taskValidation = LibraryImpl.taskValidation
"""))
               |> withOutputType CompileOutput.Library
               |> withName "Library"
               |> withOptimize
               |> withOptions ["--parallelcompilation+"; "--nowarn:75"]
               |> ignoreWarnings
           )

       let consumerSource =
           """module ConsumerImpl

open LibraryImpl
open LibraryImplSupport

let run () =
    taskValidation.Bind(
        Async.singleton 42,
        fun asyncValue ->
            taskValidation.Bind(
                Ok 42,
                fun resultValue ->
                    taskValidation.Bind(
                        Choice1Of2 42,
                        fun choiceValue ->
                            taskValidation.Return(asyncValue + resultValue + choiceValue))))
"""

       let consumerAdditionalSources =
           Array.init 12 (fun i ->
               let source = $"""module Consumer{i}

open LibraryImpl
open LibraryImplSupport

let run () =
    taskValidation.Bind(
        Async.singleton 42,
        fun asyncValue ->
            taskValidation.Bind(
                Ok 42,
                fun resultValue ->
                    taskValidation.Bind(
                        Choice1Of2 42,
                        fun choiceValue ->
                            taskValidation.Return(asyncValue + resultValue + choiceValue))))
"""

               SourceCodeFileKind.Create(sprintf "Consumer.%d.fs" i, source))
           |> Array.toList

       let consumer =
           (
               FSharp consumerSource
               |> withAdditionalSourceFiles consumerAdditionalSources
               |> withAdditionalSourceFile (SourceCodeFileKind.Create("Consumer.Support.fs", """
module ConsumerSupport

[<EntryPoint>]
let main _ =
   match (ConsumerImpl.run ()).GetAwaiter().GetResult() with
   | Ok value -> if value = 126 then 0 else 1
   | Error _ -> 1
"""))
               |> withOutputType CompileOutput.Exe
               |> withReferences [ library ]
               |> withOptimize
               |> withOptions ["--parallelcompilation+"; "--nowarn:75"]
               |> ignoreWarnings
           )

       for _i = 1 to 30 do
           consumer
           |> compile
           |> shouldSucceed
           |> ignore

module Regression_RecursiveInlineMemberDependencies =

   let private assertCompiles source =
       source
       |> withOptimize
       |> compile
       |> shouldSucceed
       |> ignore

   [<Fact>]
   let ``Inline members that depend on sibling member access compile`` () =
       FSharp """
module MemberAccessDependencyRepro

type ValidationBuilder() =
   member inline _.Return(value: int) : int = value

   member inline this.Bind(value: int, binder: int -> int) : int =
       let result = this.Source value
       binder result

   member inline this.Source(value: int) : int = value

let inline run (builder: ValidationBuilder) =
   builder.Bind(1, fun x -> x + 1)
"""
       |> assertCompiles

   [<Fact>]
   let ``Trait-witness inline overload consumers compile`` () =
       FSharp """
module TraitWitnessOverloadRepro

open System.Runtime.InteropServices

type Default1 = class end

type Intersperse =
   inherit Default1

   static member inline Intersperse (x: '``Collection<'T>``, e: 'T, [<Optional>]_impl: Default1) =
       x

   static member Intersperse (x: list<'T>, e: 'T, [<Optional>]_impl: Intersperse) =
       x

   static member inline Invoke (sep: 'T) (source: '``Collection<'T>``) =
       let inline call_2 (a: ^a, b: ^b, s) =
           ((^a or ^b): (static member Intersperse: _ * _ * _ -> _) (b, s, a))

       let inline call (a: 'a, b: 'b, s) =
           call_2 (a, b, s)

       call (Unchecked.defaultof<Intersperse>, source, sep) : '``Collection<'T>``

let _ = Intersperse.Invoke 0 [1]
"""
       |> assertCompiles

   [<Fact>]
   let ``Issue 1565 example 1 compiles`` () =
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
       |> assertCompiles

   [<Fact>]
   let ``Issue 1565 example 2 compiles`` () =
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
       |> assertCompiles

   [<Fact>]
   let ``Issue 1565 example 3 compiles`` () =
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
       |> assertCompiles
