// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.InferenceProcedures

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ByrefSafetyAnalysis =
    let private captureDiagnostic line startCol endCol =
        (Error 3235,
         Line line,
         Col startCol,
         Line line,
         Col endCol,
         "A Span or IsByRefLike value returned from the expression cannot be used at ths point. This is to ensure the address of the local value does not escape its scope.")

    let private issue20803Provider optimize =
        FsFromPath(__SOURCE_DIRECTORY__ ++ "Issue20803Provider.fsi")
        |> withAdditionalSourceFiles [ SourceFromPath(__SOURCE_DIRECTORY__ ++ "Issue20803Provider.fs") ]
        |> withName "Issue20803Provider"
        |> withOptimization optimize

    let private verifyRejectedApplication optimize diagnostics compilation =
        compilation
        |> withOptimization optimize
        |> compile
        |> shouldFail
        |> withDiagnostics diagnostics

    [<TheoryForNETCOREAPP; InlineData(false); InlineData(true)>]
    let ``Issue 20803 - constructed span partial application`` optimize =
        FSharp
            """module Consumer
open System
let add (s: Span<int>) (x: int) = s.Length + x
let bad = add (Span<int> [| 1; 2; 3 |])
"""
        |> asLibrary
        |> verifyRejectedApplication optimize [ captureDiagnostic 4 16 39 ]

    [<TheoryForNETCOREAPP; InlineData(false); InlineData(true)>]
    let ``Issue 20803 - inferred curried arity`` optimize =
        let provider = issue20803Provider optimize

        let inferred =
            FSharp "module Arity\nlet f (s: System.Span<int>) = fun () -> s.Length"

        for compilation, moduleName, names in [ provider, "Provider", [ "add"; "f" ]; inferred, "Arity", [ "f" ] ] do
            let assembly =
                compilation
                |> withOptimization optimize
                |> compile
                |> shouldSucceed
                |> withDiagnostics []
                |> getAssembly

            for name in names do
                let methodInfo = assembly.GetType(moduleName).GetMethod(name)

                let attribute =
                    methodInfo.GetCustomAttributes(typeof<CompilationArgumentCountsAttribute>, false)
                    |> Assert.Single
                    :?> CompilationArgumentCountsAttribute

                Assert.Equal<int>([| 1; 1 |], attribute.Counts)

        FSharp
            """module Consumer
open System
let f (s: Span<int>) = fun () -> s.Length
let bad = f (Span<int> [| 1; 2; 3 |])
"""
        |> verifyRejectedApplication optimize [ captureDiagnostic 4 14 37 ]

    [<TheoryForNETCOREAPP; InlineData(false); InlineData(true)>]
    let ``Issue 20803 - imported measurable representation`` optimize =
        let provider = issue20803Provider optimize
        provider |> compile |> shouldSucceed |> withDiagnostics [] |> ignore

        FSharp "module Consumer\nlet add (s: System.Span<int>) x = s.Length + x\nlet bad = add (Provider.make ())"
        |> withReferences [ provider ]
        |> verifyRejectedApplication optimize [ captureDiagnostic 3 16 32 ]

    [<TheoryForNETCOREAPP;
      InlineData(false, false);
      InlineData(false, true);
      InlineData(true, false);
      InlineData(true, true)>]
    let ``Issue 20803 - signature and imported curried applications`` optimize separateAssembly =
        let consumer =
            "module Consumer\nlet p = Provider.add (System.Span<int>.Empty)\nlet q = Provider.f (System.Span<int>.Empty)"

        let provider = issue20803Provider optimize

        let compilation =
            if separateAssembly then
                FSharp consumer |> withReferences [ provider ]
            else
                provider
                |> withAdditionalSourceFiles [ FsSourceWithFileName "Consumer.fs" consumer ]

        compilation
        |> verifyRejectedApplication optimize [ captureDiagnostic 2 23 45; captureDiagnostic 3 21 43 ]

    [<TheoryForNETCOREAPP;
      InlineData(false, "ReadOnlySpan<int>", "ReadOnlySpan<int>.Empty", 16, 39);
      InlineData(true, "ReadOnlySpan<int>", "ReadOnlySpan<int>.Empty", 16, 39);
      InlineData(false, "Span<int>", "Span<int>.Empty", 16, 31);
      InlineData(true, "Span<int>", "Span<int>.Empty", 16, 31);
      InlineData(false, "Span<int>", "make ()", 16, 23);
      InlineData(true, "Span<int>", "make ()", 16, 23);
      InlineData(false, "Span<int>", "genericSpan<1> ()", 16, 33);
      InlineData(true, "Span<int>", "genericSpan<1> ()", 16, 33);
      InlineData(false, "Span<int>", "recursiveSpan<1> ()", 16, 35);
      InlineData(true, "Span<int>", "recursiveSpan<1> ()", 16, 35);
      InlineData(false, "Span<int>", "let n = 1 in Span<int> [| n |]", 20, 21);
      InlineData(true, "Span<int>", "let n = 1 in Span<int> [| n |]", 20, 21);
      InlineData(false, "Span<int>", "ignore 1; Span<int>.Empty", 16, 41);
      InlineData(true, "Span<int>", "ignore 1; Span<int>.Empty", 16, 41);
      InlineData(false, "Small", "Small(3)", 16, 24);
      InlineData(true, "Small", "Small(3)", 16, 24)>]
    let ``Issue 20803 - computed byref-like arguments`` optimize ty argument startCol endCol =
        FSharp
            $"""module Consumer
open System
open System.Runtime.CompilerServices
[<Struct; IsByRefLike>]
type Small(value: int) =
    member _.Length = value
let make () = Span<int>([| 1; 2; 3 |])
[<MeasureAnnotatedAbbreviation>]
type TaggedSpan<[<Measure>] 'u> = Span<int>
let genericSpan<[<Measure>] 'u> () : TaggedSpan<'u> = Unchecked.defaultof<_>
let rec recursiveSpan<[<Measure>] 'u> () : TaggedSpan<'u> =
    if false then otherSpan () else Unchecked.defaultof<_>
and otherSpan () = recursiveSpan ()
let add (s: {ty}) x = s.Length + x
let bad = add ({argument})
"""
        |> verifyRejectedApplication optimize [ captureDiagnostic 15 startCol endCol ]

    [<TheoryForNETCOREAPP; InlineData(false); InlineData(true)>]
    let ``Issue 20803 - later curried groups`` optimize =
        FSharp
            """module Consumer
open System
let later x (s: Span<int>) y = x + s.Length + y
let p = later 1 (Span<int>.Empty)
"""
        |> verifyRejectedApplication optimize [ captureDiagnostic 4 18 33 ]

    [<TheoryForNETCOREAPP; InlineData(false); InlineData(true)>]
    let ``Issue 20803 - byref-like record partial application`` optimize =
        FSharp
            """module Consumer
open System.Runtime.CompilerServices
[<Struct; IsByRefLike; NoEquality; NoComparison>]
type Small = { Value: int }
let add (r: Small) x = r.Value + x
let good = add { Value = 3 } 4
let bad = add { Value = 3 }
"""
        |> verifyRejectedApplication optimize [ captureDiagnostic 7 15 28 ]

    [<TheoryForNETCOREAPP; InlineData(false); InlineData(true)>]
    let ``Issue 20803 - byref-like union partial application`` optimize =
        FSharp
            """module Consumer
open System.Runtime.CompilerServices
[<Struct; IsByRefLike; NoEquality; NoComparison>]
type Small = Small of int
let add (Small value) x = value + x
let good = add (Small 3) 4
let bad = add (Small 3)
"""
        |> verifyRejectedApplication optimize [ captureDiagnostic 7 16 23 ]

    [<TheoryForNETCOREAPP; InlineData(false); InlineData(true)>]
    let ``Issue 20803 - byref-like field partial application`` optimize =
        FSharp
            """module Consumer
open System
open System.Runtime.CompilerServices
[<Struct; IsByRefLike; NoEquality; NoComparison>]
type Holder = { Item: Span<int> }
let add (s: Span<int>) x = s.Length + x
let good (h: Holder) = add h.Item 1
let bad (h: Holder) = add h.Item
"""
        |> verifyRejectedApplication optimize [ captureDiagnostic 8 27 33 ]

    [<TheoryForNETCOREAPP; InlineData(false); InlineData(true)>]
    let ``Issue 20803 - existing variable and tuple diagnostics`` optimize =
        FSharp
            """module Consumer
open System
let add (s: Span<int>) x = s.Length + x
let local () =
    let s = Span<int>.Empty
    add s
let tupled (s: Span<int>, x) y = s.Length + x + y
let bad = tupled (Span<int>.Empty, 1)
let byref (x: inref<int>) y = x + y
let address () =
    let x = 1
    byref &x
"""
        |> verifyRejectedApplication
            optimize
            [ (Error 418, Line 6, Col 9, Line 6, Col 10, "The byref typed value 's' cannot be used at this point")
              (Error 412,
               Line 8,
               Col 19,
               Line 8,
               Col 37,
               "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
              (Error 421,
               Line 12,
               Col 12,
               Line 12,
               Col 13,
               "The address of the variable 'x' cannot be used at this point") ]

    [<TheoryForNETCOREAPP; InlineData(false); InlineData(true)>]
    let ``Issue 20803 - existing receiver and inner lambda diagnostics`` optimize =
        FSharp
            """module Consumer
open System
open System.Runtime.CompilerServices
[<Struct; IsByRefLike>]
type Small(value: int) =
    member _.Add x = value + x
let receiver () =
    let mutable s = Small(3)
    s.Add
let inner (s: Span<int>) =
    let g () = s.Length
    g
"""
        |> verifyRejectedApplication
            optimize
            [ (Error 406,
               Line 9,
               Col 5,
               Line 9,
               Col 10,
               "The byref-typed variable 'objectArg' is used in an invalid way. Byrefs cannot be captured by closures or passed to inner functions.")
              (Error 406,
               Line 11,
               Col 16,
               Line 11,
               Col 24,
               "The byref-typed variable 's' is used in an invalid way. Byrefs cannot be captured by closures or passed to inner functions.") ]

    [<TheoryForNETCOREAPP; InlineData(false); InlineData(true)>]
    let ``Issue 20803 - legal synchronous applications`` optimize =
        FsFromPath(__SOURCE_DIRECTORY__ ++ "Issue20803Positive.fs")
        |> withReferences [ issue20803Provider optimize ]
        |> withOptimization optimize
        |> withNoWarn 20
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "Issue 20803 controls passed"

    [<FactForNETCOREAPP>]
    let ``Issue 20803 - FSI legal synchronous applications`` () =
        let provider = __SOURCE_DIRECTORY__ ++ "Issue20803Provider.fs"
        let consumer = __SOURCE_DIRECTORY__ ++ "Issue20803Positive.fs"

        Fsx
            $"""#load @"{provider}"
#load @"{consumer}"
7"""
        |> withNoWarn 20
        |> eval
        |> shouldSucceed
        |> withEvalValueEquals 7

    [<FactForNETCOREAPP>]
    let ``Issue 20803 - FSI capture diagnostic`` () =
        Fsx "let add (s: System.Span<int>) x = s.Length + x\nlet bad = add (System.Span<int>.Empty)"
        |> withOptions [ "--typecheck-only" ]
        |> eval
        |> shouldFail
        |> withDiagnostics [ captureDiagnostic 2 16 38 ]

    [<Theory; InlineData(false); InlineData(true)>]
    let ``Issue 20803 - ordinary higher-order arguments`` optimize =
        FSharp
            """module Consumer
type R<'T> = { Value: 'T }
[<Struct>]
type S<'T> = { Value: 'T }
type Alias<'T> = 'T
type FunctionAlias<'T> = 'T -> 'T
type ListAlias<'T> = 'T list
type U<'T> = U of 'T
let invoke (f: (int -> int) -> int) = f (fun (x: int) -> x)
let array (f: int[] -> int) = f [| 1; 2; 3 |]
let bytes (f: byte[] -> byte) = f "abc"B
let uint16s (f: uint16[] -> uint16) = f [| 1us; 2us; 3us |]
let record (f: R<int> -> int) = f { Value = 1 }
let structRecord (f: S<int> -> int) = f { Value = 2 }
let anonRecord (f: {| Value: int |} -> int) = f {| Value = 3 |}
let structAnonRecord (f: struct {| Value: int |} -> int) = f struct {| Value = 4 |}
let option (f: int option -> int) = f (Some 1)
let list (f: int list -> int) = f [1; 2; 3]
let valueOption (f: int voption -> int) = f (ValueSome 2)
let wrappedLambda (f: (int -> int) -> int) = f (let n = 1 in fun x -> x + n)
let wrappedArray (f: int[] -> int) = f (let n = 1 in [| n |])
let sequentialArray (f: int[] -> int) = f (ignore 1; [| 2 |])
let recursiveLambda (f: (int -> int) -> int) = f (let rec loop x = if x = 0 then 0 else loop (x - 1) in fun x -> loop x)
let identity (f: int -> int) = f (id 1)
let mapped (f: int list -> int) xs = f (List.map id xs)
let field (f: int -> int) (r: R<int>) = f r.Value
let structField (f: int -> int) (r: S<int>) = f r.Value
let genericField (f: 'T -> 'T) (r: R<'T>) = f r.Value
let nested (f: int -> int) = f (id id 1)
let functionField (f: int -> int) (r: R<int -> int>) = f (r.Value 1)
let wrappedApplication (f: int -> int) = f (let n = 1 in id n)
let aliasField (f: int -> int) (r: R<Alias<int>>) = f r.Value
let aliasFunction (f: int -> int) (r: R<FunctionAlias<int>>) = f (r.Value 1)
let aliasList (f: int list -> int) (r: R<ListAlias<int>>) = f r.Value
let unionField (f: int -> int) (u: U<int>) = f (let (U value) = u in value)
let add (r: R<int>) x = r.Value + x
let partial = add { Value = 3 }
let addAnon (r: {| Value: int |}) x = r.Value + x
let partialAnon = addAnon {| Value = 5 |}
let addStructAnon (r: struct {| Value: int |}) x = r.Value + x
let partialStructAnon = addStructAnon struct {| Value = 6 |}
let addOption (r: int option) x = Option.get r + x
let partialOption = addOption (Some 7)
let partialWrapped = add (let n = 8 in { Value = n })
if invoke (fun f -> f 42) <> 42 || array Array.sum <> 6
   || bytes Array.head <> 97uy || uint16s Array.sum <> 6us
   || record (fun r -> r.Value) <> 1 || structRecord (fun r -> r.Value) <> 2
   || anonRecord (fun r -> r.Value) <> 3 || structAnonRecord (fun r -> r.Value) <> 4
   || partialAnon 7 <> 12 || partialStructAnon 8 <> 14
   || option Option.get <> 1 || list List.sum <> 6 || valueOption ValueOption.get <> 2
   || wrappedLambda (fun f -> f 4) <> 5 || wrappedArray Array.sum <> 1
   || sequentialArray Array.sum <> 2 || recursiveLambda (fun f -> f 3) <> 0
   || identity id <> 1 || mapped List.sum [1; 2; 3] <> 6
   || field id { Value = 3 } <> 3 || structField id { Value = 4 } <> 4
   || genericField id { Value = "value" } <> "value"
   || nested id <> 1 || functionField id { Value = id } <> 1 || wrappedApplication id <> 1
   || aliasField id { Value = 2 } <> 2 || aliasFunction id { Value = id } <> 1
   || aliasList List.sum { Value = [1; 2; 3] } <> 6
   || unionField id (U 5) <> 5
   || partialOption 8 <> 15 || partialWrapped 9 <> 17
   || partial 4 <> 7 then
    failwith "higher-order arguments"
"""
        |> withOptimization optimize
        |> compileExeAndRun
        |> shouldSucceed

    let withPrelude =
        withReferences [
            FsFromPath (__SOURCE_DIRECTORY__ ++ "Prelude.fs")
            |> withName "Prelude"
        ]

    let verifyCompile compilation =
        compilation
        |> asExe
        |> withNoWarn 3370
        |> withOptions ["--test:ErrorRanges"]
        |> withPrelude
        |> compile

    let verifyCompileAndRun compilation =
        compilation
        |> asExe
        |> withNoWarn 3370
        |> withOptions ["--test:ErrorRanges"]
        |> withPrelude
        |> compileAndRun

    // SOURCE=Migrated02.fs SCFLAGS="--test:ErrorRanges"                                  # Migrated02.fs
    [<Theory; FileInlineData("MigratedTest02.fs")>]
    let``MigratedTest02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun |> shouldSucceed

    // SOURCE=E_Migrated02.fs SCFLAGS="--test:ErrorRanges"                                # E_Migrated02.fs
    [<Theory; FileInlineData("E_MigratedTest02.fs")>]
    let``E_Migrated02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 193, Line 165, Col 9, Line 165, Col 22, "This expression is a function value, i.e. is missing arguments. Its type is byref<int> -> unit.")
            (Warning 193, Line 183, Col 9, Line 183, Col 20, "This expression is a function value, i.e. is missing arguments. Its type is int -> byref<int> -> unit.")
            (Warning 193, Line 198, Col 9, Line 198, Col 24, "This expression is a function value, i.e. is missing arguments. Its type is inref<int> * int -> unit.")
            (Warning 20, Line 206, Col 9, Line 206, Col 18, "The result of this expression has type 'byref<int> * int' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
            (Error 3209, Line 13, Col 18, Line 13, Col 19, "The address of the variable 'y' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
            (Error 3209, Line 20, Col 18, Line 20, Col 19, "The address of the variable 'z' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
            (Error 3209, Line 29, Col 14, Line 29, Col 15, "The address of the variable 'x' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
            (Error 3209, Line 33, Col 14, Line 33, Col 15, "The address of the variable 'y' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
            (Error 3209, Line 40, Col 14, Line 40, Col 15, "The address of the variable 'x' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
            (Error 3209, Line 43, Col 14, Line 43, Col 15, "The address of the variable 'y' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
            (Error 3209, Line 52, Col 18, Line 52, Col 19, "The address of the variable 'z' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
            (Error 3209, Line 53, Col 10, Line 53, Col 11, "The address of the variable 'y' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
            (Error 3228, Line 63, Col 14, Line 63, Col 29, "The address of a value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
            (Error 3228, Line 71, Col 14, Line 71, Col 29, "The address of a value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
            (Error 412, Line 77, Col 13, Line 77, Col 14, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 77, Col 17, Line 77, Col 29, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 421, Line 77, Col 28, Line 77, Col 29, "The address of the variable 'x' cannot be used at this point")
            (Error 425, Line 77, Col 17, Line 77, Col 29, "The type of a first-class function cannot contain byrefs")
            (Error 3209, Line 96, Col 53, Line 96, Col 54, "The address of the variable 'x' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
            (Error 3209, Line 108, Col 41, Line 108, Col 42, "The address of the variable 'x' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.")
            (Error 438, Line 117, Col 23, Line 117, Col 33, "Duplicate method. The method 'TestMethod' has the same name and signature as another method in type 'NegativeTests.TestNegativeOverloading'.")
            (Error 438, Line 115, Col 23, Line 115, Col 33, "Duplicate method. The method 'TestMethod' has the same name and signature as another method in type 'NegativeTests.TestNegativeOverloading'.")
            (Error 438, Line 113, Col 23, Line 113, Col 33, "Duplicate method. The method 'TestMethod' has the same name and signature as another method in type 'NegativeTests.TestNegativeOverloading'.")
            (Error 412, Line 121, Col 18, Line 121, Col 22, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 123, Col 18, Line 123, Col 23, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 3301, Line 125, Col 9, Line 125, Col 14, "The function or method has an invalid return type 'byref<int> * int'. This is not permitted by the rules of Common IL.")
            (Error 412, Line 125, Col 34, Line 125, Col 39, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 418, Line 125, Col 35, Line 125, Col 36, "The byref typed value 'x' cannot be used at this point")
            (Error 3301, Line 127, Col 9, Line 127, Col 14, "The function or method has an invalid return type 'byref<int> -> unit'. This is not permitted by the rules of Common IL.")
            (Error 412, Line 129, Col 14, Line 129, Col 15, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 3300, Line 131, Col 17, Line 131, Col 18, "The parameter 'x' has an invalid type '(byref<int> -> unit) * int'. This is not permitted by the rules of Common IL.")
            (Error 3300, Line 133, Col 17, Line 133, Col 18, "The parameter 'x' has an invalid type 'byref<int> -> unit'. This is not permitted by the rules of Common IL.")
            (Error 3300, Line 133, Col 41, Line 133, Col 42, "The parameter 'y' has an invalid type 'byref<int> * int'. This is not permitted by the rules of Common IL.")
            (Error 3300, Line 139, Col 36, Line 139, Col 39, "The parameter 'tup' has an invalid type 'inref<int> * int'. This is not permitted by the rules of Common IL.")
            (Error 412, Line 140, Col 13, Line 140, Col 33, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 3300, Line 142, Col 37, Line 142, Col 38, "The parameter 'x' has an invalid type 'byref<int> -> unit'. This is not permitted by the rules of Common IL.")
            (Error 3300, Line 144, Col 37, Line 144, Col 38, "The parameter 'x' has an invalid type 'byref<int> option'. This is not permitted by the rules of Common IL.")
            (Error 3300, Line 146, Col 17, Line 146, Col 18, "The parameter 'x' has an invalid type 'byref<int> option'. This is not permitted by the rules of Common IL.")
            (Error 412, Line 151, Col 13, Line 151, Col 14, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 151, Col 17, Line 151, Col 30, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 3301, Line 154, Col 9, Line 154, Col 15, "The function or method has an invalid return type 'byref<int> -> unit'. This is not permitted by the rules of Common IL.")
            (Error 412, Line 155, Col 9, Line 155, Col 22, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 158, Col 13, Line 158, Col 14, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 160, Col 13, Line 160, Col 26, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 165, Col 9, Line 165, Col 22, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 169, Col 13, Line 169, Col 14, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 169, Col 17, Line 169, Col 28, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 3301, Line 172, Col 9, Line 172, Col 15, "The function or method has an invalid return type 'int -> byref<int> -> unit'. This is not permitted by the rules of Common IL.")
            (Error 412, Line 173, Col 9, Line 173, Col 20, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 176, Col 13, Line 176, Col 14, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 178, Col 13, Line 178, Col 24, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 183, Col 9, Line 183, Col 20, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 187, Col 13, Line 187, Col 14, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 187, Col 17, Line 187, Col 32, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 425, Line 187, Col 17, Line 187, Col 32, "The type of a first-class function cannot contain byrefs")
            (Error 412, Line 191, Col 13, Line 191, Col 14, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 193, Col 13, Line 193, Col 28, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 198, Col 9, Line 198, Col 24, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 3301, Line 201, Col 9, Line 201, Col 15, "The function or method has an invalid return type 'byref<int> * int'. This is not permitted by the rules of Common IL.")
            (Error 412, Line 203, Col 10, Line 203, Col 15, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 421, Line 203, Col 11, Line 203, Col 12, "The address of the variable 'x' cannot be used at this point")
            (Error 412, Line 206, Col 9, Line 206, Col 18, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 210, Col 13, Line 210, Col 14, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 210, Col 17, Line 210, Col 26, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
        ]

    // SOURCE=Migrated03.fs SCFLAGS="--test:ErrorRanges"                                  # Migrated03.fs
    [<Theory; FileInlineData("MigratedTest03.fs")>]
    let``MigratedTest03_fs`` compilation =
        let csharpLib =
            CSharpFromPath (__SOURCE_DIRECTORY__ ++ "MigratedTest03CSharpLib.cs")
            |> withName "CSharpLib3"

        compilation
        |> getCompilation
        |> withReferences [ csharpLib ]
        |> withOptions ["--nowarn:3370"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=E_Migrated03.fs SCFLAGS="--test:ErrorRanges"                                # E_Migrated03.fs
    [<Theory; FileInlineData("E_MigratedTest03.fs")>]
    let``E_Migrated03_fs`` compilation =
        let csharpLib =
            CSharpFromPath (__SOURCE_DIRECTORY__ ++ "MigratedTest03CSharpLib.cs")
            |> withName "CSharpLib3"

        compilation
        |> getCompilation
        |> asExe
        |> withReferences [ csharpLib ]
        |> withNoWarn 52
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3237, Line 23, Col 18, Line 23, Col 28, "Cannot call the byref extension method 'Test2. 'this' parameter requires the value to be mutable or a non-readonly byref type.")
            (Error 1, Line 24, Col 9, Line 24, Col 11, "Type mismatch. Expecting a
    'byref<DateTime>'    
but given a
    'inref<DateTime>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'")
            (Error 3237, Line 28, Col 9, Line 28, Col 20, "Cannot call the byref extension method 'Change. 'this' parameter requires the value to be mutable or a non-readonly byref type.")
            (Error 3237, Line 33, Col 19, Line 33, Col 30, "Cannot call the byref extension method 'Test2. 'this' parameter requires the value to be mutable or a non-readonly byref type.")
            (Error 3237, Line 39, Col 9, Line 39, Col 21, "Cannot call the byref extension method 'Change. 'this' parameter requires the value to be mutable or a non-readonly byref type.")
            (Error 3239, Line 43, Col 17, Line 43, Col 29, "Cannot partially apply the extension method 'NotChange' because the first parameter is a byref type.")
            (Error 3239, Line 44, Col 17, Line 44, Col 24, "Cannot partially apply the extension method 'Test' because the first parameter is a byref type.")
            (Error 3239, Line 45, Col 17, Line 45, Col 26, "Cannot partially apply the extension method 'Change' because the first parameter is a byref type.")
            (Error 3239, Line 46, Col 17, Line 46, Col 25, "Cannot partially apply the extension method 'Test2' because the first parameter is a byref type.")
        ]

    [<Theory; FileInlineData("TryGetValue.fs")>]
    let``TryGetValue_fs`` compilation =
        compilation
        |> getCompilation
        |> withPrelude |> compileExeAndRun |> shouldSucceed

    [<Theory; FileInlineData("CompareExchange.fs")>]
    let``CompareExchange_fs`` compilation =
        compilation
        |> getCompilation
        |> withPrelude
        |> withOptions ["--nowarn:3370"]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("ByRefParam.fs")>]
    let``ByRefParam_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun |> shouldSucceed

    [<Theory; FileInlineData("ByRefParam_ExplicitOutAttribute.fs")>]
    let``ByRefParam_ExplicitOutAttribute_fs`` compilation =
        compilation
        |> getCompilation
        |> withPrelude
        |> withOptions ["--nowarn:3370"] |> compileExeAndRun |> shouldSucceed

    [<Theory; FileInlineData("ByRefParam_ExplicitInAttribute.fs")>]
    let``ByRefParam_ExplicitInAttribute_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun |> shouldSucceed

    [<Theory; FileInlineData("ByRefReturn.fs")>]
    let``ByRefReturn_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("Slot_ByRefReturn.fs")>]
    let``Slot_ByRefReturn_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("InRefReturn.fs")>]
    let``InRefReturn_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("Slot_InRefReturn.fs")>]
    let``Slot_InRefReturn_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("OutRefParam.fs")>]
    let``OutRefParam_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("OutRefParam_ExplicitOutAttribute.fs")>]
    let``OutRefParam_ExplicitOutAttribute_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("Slot_OutRefParam.fs")>]
    let``Slot_OutRefParam_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("ByRefParam_OverloadedTest_ExplicitOutAttribute.fs")>]
    let``ByRefParam_OverloadedTest_ExplicitOutAttribute_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("OutRefParam_Overloaded_ExplicitOutAttribute.fs")>]
    let``OutRefParam_Overloaded_ExplicitOutAttribute_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("OutRefParam_Overloaded.fs")>]
    let``OutRefParam_Overloaded_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("InRefParam_ExplicitInAttribute.fs")>]
    let``InRefParam_ExplicitInAttribute_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("InRefParam_ExplicitInAttributeDateTime.fs")>]
    let``InRefParam_ExplicitInAttributeDateTime_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("InRefParam.fs")>]
    let``InRefParam`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("InRefParamOverload_ExplicitAddressOfAtCallSite.fs")>]
    let``InRefParamOverload_ExplicitAddressOfAtCallSite`` compilation =
        compilation
        |> getCompilation
        |> withNoWarn 52
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("InRefParamOverload_ImplicitAddressOfAtCallSite.fs")>]
    let``InRefParamOverload_ImplicitAddressOfAtCallSite`` compilation =
        compilation
        |> getCompilation
        |> withNoWarn 52
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("InRefParamOverload_ImplicitAddressOfAtCallSite2.fs")>]
    let``InRefParamOverload_ImplicitAddressOfAtCallSite2`` compilation =
        compilation
        |> getCompilation
        |> withNoWarn 52
        |> verifyCompileAndRun
        |> shouldSucceed

    // TODO: Delete this, move into feature branch, or finish this. See: https://github.com/dotnet/fsharp/pull/7989#discussion_r369841104
#if IMPLICIT_ADDRESS_OF
    module FeatureImplicitAddressOf =
        [<Theory; Directory(__SOURCE_DIRECTORY__ + "/ImplicitAddressOf", Includes=[|"InRefParam_DateTime.fs"|])>]
        let``InRefParam_DateTime`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
            
        [<Theory; Directory(__SOURCE_DIRECTORY__ + "/ImplicitAddressOf", Includes=[|"InRefParam_DateTime_ImplicitAddressOfAtCallSite.fs"|])>]
        let``InRefParam_DateTime_ImplicitAddressOfAtCallSite`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
            
        [<Theory; Directory(__SOURCE_DIRECTORY__ + "/ImplicitAddressOf", Includes=[|"InRefParam_DateTime_ImplicitAddressOfAtCallSite2.fs"|])>]
        let``InRefParam_DateTime_ImplicitAddressOfAtCallSite2`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
            
            
        [<Theory; Directory(__SOURCE_DIRECTORY__ + "/ImplicitAddressOf", Includes=[|"InRefParam_DateTime_ImplicitAddressOfAtCallSite3.fs"|])>]
        let``InRefParam_DateTime_ImplicitAddressOfAtCallSite3`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
            
        [<Theory; Directory(__SOURCE_DIRECTORY__ + "/ImplicitAddressOf", Includes=[|"InRefParam_DateTime_ImplicitAddressOfAtCallSite4.fs"|])>]
        let``InRefParam_DateTime_ImplicitAddressOfAtCallSite4`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed
#endif

    [<Theory; FileInlineData("InRefParam_Generic_ExplicitAddressOfAttCallSite1.fs")>]
    let``InRefParam_Generic_ExplicitAddressOfAttCallSite1`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("InRefParam_Generic_ExplicitAddressOfAttCallSite2.fs")>]
    let``InRefParam_Generic_ExplicitAddressOfAttCallSite2`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
        |> shouldSucceed

    module ByrefReturn =
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestImmediateReturn.fs"|])>]
        let``TestImmediateReturn`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestMatchReturn.fs"|])>]
        let``TestMatchReturn`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestConditionalReturn.fs"|])>]
        let``TestConditionalReturn`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestTryWithReturn.fs"|])>]
        let``TestTryWithReturn`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestTryFinallyReturn.fs"|])>]
        let``TestTryFinallyReturn`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestOneArgument.fs"|])>]
        let``TestOneArgument`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestTwoArguments.fs"|])>]
        let``TestTwoArguments`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestRecordParam.fs"|])>]
        let``TestRecordParam`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestRecordParam2.fs"|])>]
        let``TestRecordParam2`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestClassParamMutableField.fs"|])>]
        let``TestClassParamMutableField`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestArrayParam.fs"|])>]
        let``TestArrayParam`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestStructParam.fs"|])>]
        let``TestStructParam`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestInterfaceMethod.fs"|])>]
        let``TestInterfaceMethod`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestInterfaceProperty.fs"|])>]
        let``TestInterfaceProperty`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestDelegateMethod.fs"|])>]
        let``TestDelegateMethod`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestBaseCall.fs"|])>]
        let``TestBaseCall`` compilation =
            compilation |> withNoWarn 988 |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturn", Includes=[|"TestDelegateMethod2.fs"|])>]
        let``TestDelegateMethod2`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

    module ByrefReturnMember =
        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestImmediateReturn.fs"|])>]
        let``TestImmediateReturn`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestMatchReturn.fs"|])>]
        let``TestMatchReturn`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestConditionalReturn.fs"|])>]
        let``TestConditionalReturn`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestTryWithReturn.fs"|])>]
        let``TestTryWithReturn`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestOneArgument.fs"|])>]
        let``TestOneArgument`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestOneArgumentInRefReturned.fs"|])>]
        let``TestOneArgumentInRefReturned`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestOneArgumentOutRef.fs"|])>]
        let``TestOneArgumentOutRef`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestTwoArguments.fs"|])>]
        let``TestTwoArguments`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestRecordParam.fs"|])>]
        let``TestRecordParam`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestRecordParam2.fs"|])>]
        let``TestRecordParam2`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestClassParamMutableField.fs"|])>]
        let``TestClassParamMutableField`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestArrayParam.fs"|])>]
        let``TestArrayParam`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestArrayParam.fs"|])>]
        let``TestStructParam`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestInterfaceMethod.fs"|])>]
        let``TestInterfaceMethod`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestInterfaceProperty.fs"|])>]
        let``TestInterfaceProperty`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestDelegateMethod.fs"|])>]
        let``TestDelegateMethod`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestBaseCall.fs"|])>]
        let``TestBaseCall`` compilation =
            compilation |> withNoWarn 988 |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestDelegateMethod2.fs"|])>]
        let``TestDelegateMethod2`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"ByRefExtensionMethods1.fs"|])>]
        let``ByRefExtensionMethods1`` compilation =
            compilation |> withNoWarn 52 |> verifyCompileAndRun |> shouldSucceed

        // [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"ByRefExtensionMethodsOverloading.fs"|])>]
        // let``ByRefExtensionMethodsOverloading`` compilation =
        //     compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestReadOnlyAddressOfStaticField.fs"|])>]
        let``TestReadOnlyAddressOfStaticField`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestAssignToReturnByref.fs"|])>]
        let``TestAssignToReturnByref`` compilation =
            compilation |> withNoWarn 52 |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestAssignToReturnByref2.fs"|])>]
        let``TestAssignToReturnByref2`` compilation =
            compilation |> withNoWarn 52 |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"BaseCallByref.fs"|])>]
        let``BaseCallByref`` compilation =
            compilation |> withNoWarn 988 |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"Bug820.fs"|])>]
        let``Bug820`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"Bug820b.fs"|])>]
        let``Bug820b`` compilation =
            compilation |> withNoWarn 988 |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestNameModuleGeneric.fs"|])>]
        let``TestNameModuleGeneric`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestNameModuleNonGeneric.fs"|])>]
        let``TestNameModuleNonGeneric`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestNameModuleNonGenericSubsume.fs"|])>]
        let``TestNameModuleNonGenericSubsume`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"GenericTestNameRecursive.fs"|])>]
        let``GenericTestNameRecursive`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"NonGenericTestNameRecursiveInClass.fs"|])>]
        let``NonGenericTestNameRecursiveInClass`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"NonGenericTestNameRecursiveInClassSubsume.fs"|])>]
        let``NonGenericTestNameRecursiveInClassSubsume`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"StaticGenericTestNameRecursiveInClass.fs"|])>]
        let``StaticGenericTestNameRecursiveInClass`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"StaticNonGenericTestNameRecursiveInClass.fs"|])>]
        let``StaticNonGenericTestNameRecursiveInClass`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestInRefMutation.fs"|])>]
        let``TestInRefMutation`` compilation =
            compilation |> withNoWarn 52 |> withNoWarn 20 |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"MutateInRef3.fs"|])>]
        let``MutateInRef3`` compilation =
            compilation |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"MatrixOfTests.fs"|])>]
        let``MatrixOfTests`` compilation =
            compilation |> withNoWarn 988 |> verifyCompileAndRun |> shouldSucceed

        [<Theory; Directory(__SOURCE_DIRECTORY__, "ByrefReturnMember", Includes=[|"TestStructRecord.fs"|])>]
        let``TestStructRecord`` compilation =
            compilation |> withNoWarn 988 |> withNoWarn 3560 |> verifyCompileAndRun |> shouldSucceed

    [<Theory; FileInlineData("NoTailcallToByrefsWithModReq.fs")>]
    let``NoTailcallToByrefsWithModReq`` compilation =
        compilation
        |> getCompilation
        |> withNoWarn 20
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("E_CantTakeAddressOfExpressionReturningReferenceType.fs")>]
    let``E_CantTakeAddressOfExpressionReturningReferenceType`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3236, Line 15, Col 17, Line 15, Col 32, "Cannot take the address of the value returned from the expression. Assign the returned value to a let-bound value before taking the address.")
            (Error 1, Line 15, Col 17, Line 15, Col 32, "Type mismatch. Expecting a
    'byref<float array>'    
but given a
    'inref<float array>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'")
            (Error 3236, Line 20, Col 17, Line 20, Col 25, "Cannot take the address of the value returned from the expression. Assign the returned value to a let-bound value before taking the address.")
        ]



    [<Fact>]
    let ``E_WriteToInRef`` () =
        FSharp """let f1 (x: inref<int>) = x <- 1"""
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3224, Line 1, Col 26, Line 1, Col 32, "The byref pointer is readonly, so this write is not permitted.")
        ]

    [<Fact>]
    let ``E_WriteToInRefStructInner`` () =
        FSharp """let f1 (x: inref<S>) = x.X <- 1"""
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 1, Col 18, Line 1, Col 19, "The type 'S' is not defined.")
            (Error 72, Line 1, Col 24, Line 1, Col 27, "The type of this expression could not be inferred before accessing its members. Add a type annotation, e.g. (expr: SomeType), to constrain the type before this point.")
        ]

    [<Fact>]
    let ``E_InRefToByRef`` () =
        FSharp """
let f1 (x: byref<'T>) = 1
let f2 (x: inref<'T>) = f1 &x    // not allowed 
"""
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 3, Col 28, Line 3, Col 30, "Type mismatch. Expecting a
    'byref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'")
        ]

    [<Fact>]
    let ``E_InRefToOutRef`` () =
        FSharp """
let f1 (x: outref<'T>) = 1
let f2 (x: inref<'T>) = f1 &x     // not allowed
"""
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 3, Col 28, Line 3, Col 30, "Type mismatch. Expecting a
    'outref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.Out' does not match the type 'ByRefKinds.In'")
        ]

    [<Fact>]
    let ``E_InRefToByRefClassMethod`` () =
        FSharp """
type C() = 
    static member f1 (x: byref<'T>) = 1
let f2 (x: inref<'T>) = C.f1 &x // not allowed
"""
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 4, Col 30, Line 4, Col 32, "Type mismatch. Expecting a
    'byref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'")
        ]

    [<Fact>]
    let ``E_InRefToOutRefClassMethod`` () =
        FSharp """
type C() = 
    static member f1 (x: byref<'T>) = 1
let f2 (x: inref<'T>) = C.f1 &x // not allowed
"""
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 4, Col 30, Line 4, Col 32, "Type mismatch. Expecting a
    'byref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'")
        ]

    [<Fact>]
    let ``E_InRefToByRefClassMethod2`` () =
        FSharp """
type C() = 
    static member f1 (x: byref<'T>) = 1
let f2 (x: inref<'T>) = C.f1(&x) // not allowed
"""
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 4, Col 30, Line 4, Col 32, "Type mismatch. Expecting a
    'byref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'")
        ]

    [<Fact>]
    let ``E_InRefToOutRefClassMethod2`` () =
        FSharp """
type C() = 
    static member f1 (x: outref<'T>) = 1 // not allowed (not yet)
let f2 (x: inref<'T>) = C.f1(&x) // not allowed
"""
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 4, Col 30, Line 4, Col 32, "Type mismatch. Expecting a
    'outref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.Out' does not match the type 'ByRefKinds.In'")
        ]

    [<Fact>]
    let ``E_UseOfLibraryOnly`` () =
        FSharp """
type C() = 
    static member f1 (x: byref<'T, 'U>) = 1
"""
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1204, Line 3, Col 26, Line 3, Col 39, "This construct is for use in the FSharp.Core library and should not be used directly")
        ]

    [<Fact>]
    let ``E_CantTakeAddress`` () =
        FSharp """
let test1 () =
    let x = &1 // not allowed
    let y = &2 // not allowed
    x + y

let test2_helper (x: byref<int>) = x
let test2 () =
    let mutable x = 1
    let y = &test2_helper &x // not allowed
    ()
"""
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3236, Line 3, Col 13, Line 3, Col 15, "Cannot take the address of the value returned from the expression. Assign the returned value to a let-bound value before taking the address.");
            (Error 3236, Line 4, Col 13, Line 4, Col 15, "Cannot take the address of the value returned from the expression. Assign the returned value to a let-bound value before taking the address.");
            (Error 3236, Line 10, Col 13, Line 10, Col 29, "Cannot take the address of the value returned from the expression. Assign the returned value to a let-bound value before taking the address.")
        ]

    [<Fact>]
    let ``E_InRefParam_DateTime`` () =
        FSharp """
type C() = 
    static member M(x: inref<System.DateTime>) = x
let w = System.DateTime.Now
let v =  C.M(w) // not allowed
check "cweweoiwe51btw" v w
"""
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 5, Col 14, Line 5, Col 15, "This expression was expected to have type
    'inref<System.DateTime>'    
but here has type
    'System.DateTime'    ");
            (Error 39, Line 6, Col 1, Line 6, Col 6, "The value or constructor 'check' is not defined. Maybe you want one of the following:
   Checked
   Unchecked")
        ]

    [<Fact>]
    let ``E_ExtensionMethodOnByrefType`` () =
        FSharp """
type byref<'T> with
    member this.Test() = 1

type inref<'T> with
    member this.Test() = 1

type outref<'T> with
    member this.Test() = 1
"""
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3238, Line 2, Col 6, Line 2, Col 11, "Byref types are not allowed to have optional type extensions.")
            (Error 3238, Line 5, Col 6, Line 5, Col 11, "Byref types are not allowed to have optional type extensions.")
            (Error 3238, Line 8, Col 6, Line 8, Col 12, "Byref types are not allowed to have optional type extensions.")
        ]


    // SOURCE=E_ByrefAsArrayElement.fs SCFLAGS="--test:ErrorRanges"                       # E_ByrefAsArrayElement.fs
    [<Theory; FileInlineData("E_ByrefAsArrayElement.fs")>]
    let``E_ByrefAsArrayElement_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3300, Line 5, Col 18, Line 5, Col 19, "The parameter 'f' has an invalid type 'byref<int> -> 'a'. This is not permitted by the rules of Common IL.")
            (Error 424, Line 7, Col 6, Line 7, Col 13, "The address of an array element cannot be used at this point")
            (Error 412, Line 9, Col 19, Line 9, Col 20, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 11, Col 19, Line 11, Col 20, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
        ]

    // SOURCE=E_ByrefAsGenericArgument01.fs SCFLAGS="--test:ErrorRanges"                  # E_ByrefAsGenericArgument01.fs
    [<Theory; FileInlineData("E_ByrefAsGenericArgument01.fs")>]
    let``E_ByrefAsGenericArgument01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 431, Line 9, Col 5, Line 9, Col 9, "A byref typed value would be stored here. Top-level let-bound byref values are not permitted.")
            (Error 412, Line 9, Col 5, Line 9, Col 9, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 412, Line 9, Col 30, Line 9, Col 32, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
        ]

    // NoMT SOURCE=ByrefInFSI1.fsx FSIMODE=PIPE COMPILE_ONLY=1                            # ByrefInFSI1.fsx
    [<Theory; FileInlineData("ByrefInFSI1.fsx")>]
    let``ByrefInFSI1_fsx`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

    // SOURCE=E_ByrefUsedInInnerLambda01.fs SCFLAGS="--test:ErrorRanges"                  # E_ByrefUsedInInnerLambda01.fs
    [<Theory; FileInlineData("E_ByrefUsedInInnerLambda01.fs")>]
    let``E_ByrefUsedInInnerLambda01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 406, Line 12, Col 34, Line 12, Col 48, "The byref-typed variable 'byrefValue' is used in an invalid way. Byrefs cannot be captured by closures or passed to inner functions.")
        ]

    // SOURCE=E_ByrefUsedInInnerLambda02.fs SCFLAGS="--test:ErrorRanges"                  # E_ByrefUsedInInnerLambda02.fs
    [<Theory; FileInlineData("E_ByrefUsedInInnerLambda02.fs")>]
    let``E_ByrefUsedInInnerLambda02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 406, Line 11, Col 24, Line 11, Col 55, "The byref-typed variable 'byrefValue' is used in an invalid way. Byrefs cannot be captured by closures or passed to inner functions.")
        ]

    // SOURCE=E_ByrefUsedInInnerLambda03.fs SCFLAGS="--test:ErrorRanges"                  # E_ByrefUsedInInnerLambda03.fs
    [<Theory; FileInlineData("E_ByrefUsedInInnerLambda03.fs")>]
    let``E_ByrefUsedInInnerLambda03_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 406, Line 11, Col 24, Line 11, Col 60, "The byref-typed variable 'byrefValue' is used in an invalid way. Byrefs cannot be captured by closures or passed to inner functions.")
        ]

    // SOURCE=E_SetFieldToByref01.fs        SCFLAGS="--test:ErrorRanges"                  # E_SetFieldToByref01.fs
    [<Theory; FileInlineData("E_SetFieldToByref01.fs")>]
    let``E_SetFieldToByref01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 412, Line 11, Col 17, Line 11, Col 27, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 437, Line 10, Col 6, Line 10, Col 9, "A type would store a byref typed value. This is not permitted by Common IL.")
            (Error 412, Line 11, Col 50, Line 11, Col 54, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
        ]

    // SOURCE=E_SetFieldToByref02.fs        SCFLAGS="--test:ErrorRanges"                  # E_SetFieldToByref02.fs
    [<Theory; FileInlineData("E_SetFieldToByref02.fs")>]
    let``E_SetFieldToByref02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 431, Line 8, Col 9, Line 8, Col 17, "A byref typed value would be stored here. Top-level let-bound byref values are not permitted.")
        ]
    // SOURCE=E_SetFieldToByref03.fs        SCFLAGS="--test:ErrorRanges"                  # E_SetFieldToByref03.fs
    [<Theory; FileInlineData("E_SetFieldToByref03.fs")>]
    let``E_SetFieldToByref03_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 1178, Line 8, Col 6, Line 8, Col 21, "The struct, record or union type 'RecordWithByref' is not structurally comparable because the type 'byref<int>' does not satisfy the 'comparison' constraint. Consider adding the 'NoComparison' attribute to the type 'RecordWithByref' to clarify that the type is not comparable")
            (Error 412, Line 8, Col 25, Line 8, Col 26, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 437, Line 8, Col 6, Line 8, Col 21, "A type would store a byref typed value. This is not permitted by Common IL.")
        ]

    // SOURCE=E_SetFieldToByref04.fs        SCFLAGS="--test:ErrorRanges"                  # E_SetFieldToByref04.fs
    [<Theory; FileInlineData("E_SetFieldToByref04.fs")>]
    let``E_SetFieldToByref04_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 412, Line 14, Col 28, Line 14, Col 37, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 421, Line 14, Col 29, Line 14, Col 30, "The address of the variable 'x' cannot be used at this point")
            (Error 412, Line 19, Col 20, Line 19, Col 53, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
        ]

    // SOURCE=E_SetFieldToByref05.fs        SCFLAGS="--test:ErrorRanges"                  # E_SetFieldToByref05.fs
    [<Theory; FileInlineData("E_SetFieldToByref05.fs")>]
    let``E_SetFieldToByref05_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 1178, Line 8, Col 6, Line 8, Col 17, "The struct, record or union type 'DUWithByref' is not structurally comparable because the type 'byref<int>' does not satisfy the 'comparison' constraint. Consider adding the 'NoComparison' attribute to the type 'DUWithByref' to clarify that the type is not comparable")
            (Error 412, Line 9, Col 18, Line 9, Col 28, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
            (Error 437, Line 8, Col 6, Line 8, Col 17, "A type would store a byref typed value. This is not permitted by Common IL.")
            (Error 412, Line 9, Col 7, Line 9, Col 8, "A type instantiation involves a byref type. This is not permitted by the rules of Common IL.")
        ]

    // SOURCE=E_FirstClassFuncTakesByref.fs SCFLAGS="--test:ErrorRanges --flaterrors"     # E_FirstClassFuncTakesByref.fs
    [<Theory; FileInlineData("E_FirstClassFuncTakesByref.fs")>]
    let``E_FirstClassFuncTakesByref_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 8, Col 12, Line 8, Col 16, "This expression was expected to have type\n    'byref<'a>'    \nbut here has type\n    'int ref'    ")
        ]

    // SOURCE=E_StaticallyResolvedByRef01.fs SCFLAGS="--test:ErrorRanges"                 # E_StaticallyResolvedByRef01.fs
    [<Theory; FileInlineData("E_StaticallyResolvedByRef01.fs")>]
    let``E_StaticallyResolvedByRef01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
#if NETCOREAPP
            (Error 43, Line 11, Col 11, Line 11, Col 12, "The member or object constructor 'TryParse' does not take 1 argument(s). An overload was found taking 4 arguments.")
#else
            (Error 43, Line 11, Col 11, Line 11, Col 12, "The member or object constructor 'TryParse' does not take 1 argument(s). An overload was found taking 2 arguments.")
#endif
        ]

    // SOURCE=UseByrefInLambda01.fs                                                       # UseByrefInLambda01.fs
    [<Theory; FileInlineData("UseByrefInLambda01.fs")>]
    let``UseByrefInLambda01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed

#if NET7_0_OR_GREATER
// This constructor added in .NET 7: https://learn.microsoft.com/en-us/dotnet/api/system.span-1.-ctor?view=net-7.0#system-span-1-ctor(-0@)
    [<Theory; FileInlineData("ReturnFieldSetBySpan.fs")>]
    let``ReturnFieldSetBySpan_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed
        
    [<Theory; FileInlineData("ReturnSpan01.fs")>]
    let``ReturnSpan01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun
        |> shouldSucceed
#endif

#if NETCOREAPP
    [<Theory; FileInlineData("E_TopLevelByref.fs")>]
    let``E_TopLevelByref_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 431, Line 6, Col 5, Line 6, Col 13, "A byref typed value would be stored here. Top-level let-bound byref values are not permitted.")
        ]
        
    [<Theory; FileInlineData("E_SpanUsedInInnerLambda01.fs")>]
    let``E_SpanUsedInInnerLambda01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 406, Line 8, Col 34, Line 8, Col 45, "The byref-typed variable 'span' is used in an invalid way. Byrefs cannot be captured by closures or passed to inner functions.")
        ]
        
    [<Theory; FileInlineData("E_SpanUsedInInnerLambda02.fs")>]
    let``E_SpanUsedInInnerLambda02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 406, Line 8, Col 34, Line 8, Col 45, "The byref-typed variable 'span' is used in an invalid way. Byrefs cannot be captured by closures or passed to inner functions.")
        ]
#endif
