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
   let ``Cross-assembly inline overload consumers compile`` () =
       let library =
           FSharpWithFileName "Library.fs" """
module LibraryImpl

type ValidationBuilder() =
   member inline this.Bind(value: int, binder: int -> int) : int =
       binder (this.Source value)

   member inline this.Bind(value: string, binder: string -> int) : int =
       binder (this.Source value)

   member inline this.Bind(value: bool, binder: bool -> int) : int =
       binder (this.Source value)

   member inline this.Source(value: int) : int = value
   member inline this.Source(value: string) : string = value
   member inline this.Source(value: bool) : bool = value
"""
           |> withOutputType CompileOutput.Library
           |> withName "Library"
           |> withOptimize
           |> withOptions [ "--nowarn:75" ]
           |> ignoreWarnings

       FSharpWithFileName "Consumer.fs" """
module Consumer

open LibraryImpl

let run (builder: ValidationBuilder) =
   builder.Bind(1, fun x -> x + 1)
"""
       |> withReferences [ library ]
       |> withOptimize
       |> withAdditionalSourceFiles [
           FsSourceWithFileName "Consumer2.fs" """
module Consumer2

open LibraryImpl

let run (builder: ValidationBuilder) =
   builder.Bind("hello", fun x -> x.Length)
""";
           FsSourceWithFileName "Consumer3.fs" """
module Consumer3

open LibraryImpl

let run (builder: ValidationBuilder) =
   builder.Bind(true, fun x -> if x then 1 else 0)
"""
       ]
       |> compile
       |> shouldSucceed
       |> ignore

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
