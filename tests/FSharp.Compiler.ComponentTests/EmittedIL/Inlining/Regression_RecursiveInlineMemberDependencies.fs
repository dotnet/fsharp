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

   let private assertRuns optimize source =
       source
       |> (if optimize then withOptimize else withNoOptimize)
       |> compileExeAndRun
       |> shouldSucceed
       |> ignore

   [<Theory; InlineData(true); InlineData(false)>]
   let ``Issue 21016 - member uses inline operator through FSharp.Core operator`` optimize =
       FSharp """
module Issue21016

type X(y: int) =
   member x.Y = y
   static member inline (*) ((a: int), (b: X)) = X(a * b.Y)
   member x.multiply (a: int) = a * x

if (X(3).multiply 2).Y <> 6 then failwith "unexpected result"
"""
       |> assertRuns optimize

   [<Theory; InlineData(true); InlineData(false)>]
   let ``Member uses inline operator declared after it`` optimize =
       FSharp """
module InlineOperatorDeclaredLater

type Y(v: int) =
   member x.Twice() : Y = x + x
   member x.V = v
   static member inline (+) (a: Y, b: Y) = Y(a.V + b.V)

if (Y(4).Twice()).V <> 8 then failwith "unexpected result"
"""
       |> assertRuns optimize

   [<Theory; InlineData(true); InlineData(false)>]
   let ``Member uses inline operator of its base type`` optimize =
       FSharp """
module InlineOperatorOfBaseType

type Base(v: int) =
   member x.V = v
   static member inline (+) (a: Base, b: Base) = Derived(a.V + b.V)

and Derived(v: int) =
   inherit Base(v)
   member x.Plus(o: Derived) = x + o

if (Derived(1).Plus(Derived 2)).V <> 3 then failwith "unexpected result"
"""
       |> assertRuns optimize

   [<Theory; InlineData(true); InlineData(false)>]
   let ``Member uses inline operators through a generic collection function`` optimize =
       FSharp """
module InlineOperatorsThroughCollection

type Z(v: int) =
   member x.V = v
   static member Sum(xs: Z list) = List.sum xs
   static member inline (+) (a: Z, b: Z) = Z(a.V + b.V)
   static member inline Zero = Z 0

if (Z.Sum [ Z 1; Z 2; Z 3 ]).V <> 6 then failwith "unexpected result"
"""
       |> assertRuns optimize

   [<Theory; InlineData(true); InlineData(false)>]
   let ``Local let rec binding uses later inline binding`` optimize =
       FSharp """
module LocalLetRec

let run () =
   let rec a x = b x + 1
   and inline b x = x * 2
   a 3

[<EntryPoint>]
let main _ =
   if run () <> 7 then failwith "unexpected result"
   0
"""
       |> assertRuns optimize

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
   let ``Trait-witness dependency into later nested module compiles`` () =
            FSharp """
module rec TraitWitnessOnlyEdge

type User() =
   member _.Run() =
      let inline invoke (x: ^T) =
         (^T: (static member DoIt: ^T -> int) x)

      invoke (Helpers.Intersp())

module Helpers =
   type Intersp() =
      static member inline DoIt(x: Intersp) = 42
"""
         |> assertCompiles

   [<Fact>]
   let ``Unresolved trait-witness dependency from nested module compiles`` () =
       FSharp """
module rec NestedTraitWitnessOnlyEdge

module Helpers =
   type Intersp() =
      static member inline DoIt(x: Intersp) = 42

module Consumer =
   type User() =
      member _.Run() =
         let inline invoke (x: ^T) =
            (^T: (static member DoIt: ^T -> int) x)

         invoke (Helpers.Intersp())
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
