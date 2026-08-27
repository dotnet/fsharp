namespace EmittedIL.Inlining

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

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

   [<Fact>]
   let ``Recursive group with nested module reorders let rec inline dependencies`` () =
       FSharp """
module rec MixedLetRec

let consumer (x: int) = worker x

module Separator =
   let marker = 0

let rec worker (x: int) : int = helper x
and inline helper (x: int) : int = x + x
"""
       |> assertCompiles

   [<Fact>]
   let ``Recursive group member depends on inline value in later nested module`` () =
       FSharp """
module rec MixedGroup

type Builder() =
   member _.Run(x: int) = Helper.twice x

module Helper =
   let inline twice (x: int) = x + x

let result = Builder().Run 21
"""
       |> assertCompiles

   [<Fact>]
   let ``Deep recursive inline expression compiles`` () =
       let nestedExpression =
           [ 1 .. 512 ]
           |> List.fold (fun body _ -> $"if true then ({body}) else 0") "0"

       FSharp $"""
module DeepRecursiveInlineExpression

let rec inline evaluate value =
   {nestedExpression}

let _ = evaluate 0
"""
       |> assertCompiles
