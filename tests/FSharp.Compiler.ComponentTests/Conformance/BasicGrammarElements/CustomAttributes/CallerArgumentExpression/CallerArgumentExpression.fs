// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open Xunit
open FSharp.Test.Compiler
open FSharp.Test

module CustomAttributes_CallerArgumentExpression =
    [<FactForNETCOREAPP>]
    let ``Can consume CallerArgumentExpression in BCL methods`` () =
      let path = __SOURCE_DIRECTORY__ ++ "test script.fsx"
      FsFromPath path
      |> withLangVersionPreview
      |> asExe
      |> compileAndRun
      |> shouldSucceed
      |> ignore

    [<FactForNETCOREAPP>]
    let ``Can define methods using CallerArgumentExpression with C#-style optional arguments`` () =
        FSharp """let assertEqual a b = if a <> b then failwithf "not equal: %A and %A" a b
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
type A() =
  static member aa (
    a,
    [<CallerMemberName; Optional; DefaultParameterValue "no value">]b: string, 
    [<CallerLineNumber; Optional; DefaultParameterValue 0>]c: int, 
    [<CallerArgumentExpressionAttribute("a"); Optional; DefaultParameterValue "no value">]e: string) = 
    a,b,c,e

let stringABC = "abc"
assertEqual (A.aa(stringABC)) ("abc", ".cctor", 13, "stringABC")
assertEqual (A.aa(a = stringABC)) ("abc", ".cctor", 14, "stringABC")
        """
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<FactForNETCOREAPP>]
    let ``Can define methods using CallerArgumentExpression with F#-style optional arguments`` () =
        FSharp """let assertEqual a b = if a <> b then failwithf "not equal: %A and %A" a b
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
type A() =
  static member aa (
    a,
    [<CallerMemberName>] ?b: string, 
    [<CallerLineNumber>] ?c: int, 
    [<CallerArgumentExpressionAttribute("a")>] ?e: string) = 
    let b = defaultArg b "no value"
    let c = defaultArg c 0
    let e = defaultArg e "no value"
    a,b,c,e

let stringABC = "abc"
assertEqual (A.aa(stringABC)) ("abc", ".cctor", 16, "stringABC")
assertEqual (A.aa(a = stringABC)) ("abc", ".cctor", 17, "stringABC")
        """
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<FactForNETCOREAPP>]
    let ``Can define methods using CallerArgumentExpression with F#-style optional arguments of voption`` () =
        FSharp """let assertEqual a b = if a <> b then failwithf "not equal: %A and %A" a b
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
type A() =
  static member aa (
    a,
    [<CallerMemberName; Struct>] ?b: string, 
    [<CallerLineNumber; Struct>] ?c: int, 
    [<CallerArgumentExpressionAttribute("a"); Struct>] ?e: string) = 
    let b = defaultValueArg b "no value"
    let c = defaultValueArg c 0
    let e = defaultValueArg e "no value"
    a,b,c,e

let stringABC = "abc"
assertEqual (A.aa(stringABC)) ("abc", ".cctor", 16, "stringABC")
assertEqual (A.aa(a = stringABC)) ("abc", ".cctor", 17, "stringABC")
        """
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore
        
    [<FactForNETCOREAPP>]
    let ``Can define in F# - with #line`` () =
        FSharp """let assertEqual a b = if a <> b then failwithf "not equal: %A and %A" a b
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

let path = System.IO.Path.Combine(__SOURCE_DIRECTORY__, "test.fs")

# 1 "test.fs"
type A() =
  static member aa (
    a,
    [<CallerArgumentExpressionAttribute("a"); Optional; DefaultParameterValue "no value">]e: string) = 
    a,e
    
  static member B (``ab c``, [<CallerArgumentExpression "ab c">]?n) =
    defaultArg n "no value"

let stringABC = "abc"
assertEqual (A.aa(stringABC)) ("abc", "stringABC")
# 1 "test.fs"
assertEqual (A.aa(stringABC : string)) ("abc", "stringABC : string")
# 1 "test.fs"
assertEqual (A.aa(a = (stringABC : string))) ("abc", "(stringABC : string)")


A.B("abc"
#line 1
: string)
|> assertEqual "\"abc\"
#line 1
: string"


A.B((+) 1
#line 1
        123)
|> assertEqual "(+) 1
#line 1
        123"


A.B(#line 1
  (+) 1
        123)
|> assertEqual "(+) 1
        123"
        """
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<FactForNETCOREAPP>]
    let ``Can define methods using CallerArgumentExpression receiving special parameter names`` () =
      FSharp """let assertEqual a b = if a <> b then failwithf "not equal: %A and %A" a b
open System.Runtime.CompilerServices

type A() =
  static member B (``ab c``, [<CallerArgumentExpression "ab c">]?n) =
    defaultArg n "no value"
      
assertEqual (A.B("abc")) "\"abc\""
        """
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore
        
    [<FactForNETCOREAPP>]
    let ``test Warns when cannot find the referenced parameter or self-referential`` () =
      FSharp """let assertEqual a b = if a <> b then failwithf "not equal: %A and %A" a b
open System.Runtime.CompilerServices

type A() =
  static member A (``ab c``, [<CallerArgumentExpression "abc">]?n) =
    defaultArg n "no value"
  static member B (``ab c``, [<CallerArgumentExpression "n">]  ?n) =
    defaultArg n "no value"
        """
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
          (Warning 3883,Line 5, Col 65, Line 5, Col 66, "The [<CallerArgumentExpression>] on this parameter will have no effect because it's applied with an invalid parameter name.")
          (Warning 3882,Line 7, Col 65 , Line 7, Col 66, "The [<CallerArgumentExpression>] on this parameter will have no effect because it's self-referential.")
        ]
        
    [<FactForNETCOREAPP>]
    let ``test Warns when overridden by other caller infos`` () =
      FSharp """let assertEqual a b = if a <> b then failwithf "not equal: %A and %A" a b
open System.Runtime.CompilerServices

type A() =
  static member B (a, [<CallerArgumentExpression "a"; CallerMemberName>] ?n) =
    defaultArg n "no value"
  static member C (a, [<CallerArgumentExpression "a"; CallerFilePath>] ?n) =
    defaultArg n "no value"
  static member D (a, [<CallerArgumentExpression "a"; CallerMemberName; CallerFilePath>] ?n) =
    defaultArg n "no value"
    
let f () =
  let filename = System.IO.Path.Combine(__SOURCE_DIRECTORY__, "test.fs")
  #line 1 "test.fs"
  A.B(1) |> assertEqual "f"
  A.C(1) |> assertEqual filename
  A.D(1) |> assertEqual filename
f()
        """
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
          (Warning 3884, Line 5, Col 75, Line 5, Col 76, "The [<CallerArgumentExpression>] on this parameter will have no effect because it's overridden by the [<CallerMemberName>].")
          (Warning 3884, Line 7, Col 73, Line 7, Col 74, "The [<CallerArgumentExpression>] on this parameter will have no effect because it's overridden by the [<CallerFilePath>].")
          (Warning 3206, Line 9, Col 55, Line 9, Col 71, "The CallerMemberNameAttribute applied to parameter 'n' will have no effect. It is overridden by the CallerFilePathAttribute.")
          (Warning 3884, Line 9, Col 91, Line 9, Col 92, "The [<CallerArgumentExpression>] on this parameter will have no effect because it's overridden by the [<CallerFilePath>].")
        ]

    [<FactForNETCOREAPP>]
    let ``test Errors`` () =
      FSharp """let assertEqual a b = if a <> b then failwithf "not equal: %A and %A" a b
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

type A() =
  static member A (``ab c``, [<CallerArgumentExpression "ab c">] n) =
    defaultArg n "no value"
  static member B (``ab c``, [<CallerArgumentExpression "ab c">]?n) =
    defaultArg n 123
  static member C (``ab c``, [<CallerArgumentExpression "ab c"; Optional; DefaultParameterValue 0>] n: int) =
    n
        """
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
          (Error 1247,Line 6, Col 66, Line 6, Col 67, "'CallerArgumentExpression \"ab c\"' can only be applied to optional arguments")
          (Error 1246,Line 8, Col 66, Line 8, Col 67, "'CallerArgumentExpression \"ab c\"' must be applied to an argument of type 'string', but has been applied to an argument of type 'int'")
          (Error 1246,Line 10, Col 101, Line 10, Col 102, "'CallerArgumentExpression \"ab c\"' must be applied to an argument of type 'string', but has been applied to an argument of type 'int'")
        ]
        
    [<Fact>]
    let ``User can define the CallerArgumentExpression`` () =
      FSharp """namespace System.Runtime.CompilerServices

open System

[<AttributeUsage(AttributeTargets.Parameter, AllowMultiple=false, Inherited=false)>]
type CallerArgumentExpressionAttribute(parameterName: string) =
  inherit Attribute()

  member val ParameterName = parameterName
  
namespace global
module A =
  let assertEqual a b = if a <> b then failwithf "not equal: %A and %A" a b
  open System.Runtime.CompilerServices

  type A() =
    static member B (``ab c``, [<CallerArgumentExpression "ab c">]?n) =
      defaultArg n "no value"
  
  A.B "abc" |> assertEqual "\"abc\""  
  A.B ("abc": string) |> assertEqual "\"abc\": string"
  A.B ("abc": (* comments *) string) |> assertEqual "\"abc\": (* comments *) string"
"""
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore
        
    [<FactForNETCOREAPP>]
    let ``Can use in Computation Expression`` =
      FSharp """let assertEqual a b = if a <> b then failwithf "not equal: %A and %A" a b
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

type Builder() =
    member self.Bind(
        x, f,
        [<CallerArgumentExpression "x">] ?exp : string,
        [<CallerArgumentExpression "f">] ?exp2 : string) =
        (f x, $"f={exp2.Value}, x={exp.Value}")

    member self.Return(x, [<CallerArgumentExpression "x">] ?exp : string) =
        (x, $"x={exp.Value}")

let b = Builder()
b { do! () } |> assertEqual (((), "x=do!"), "f=do!, x=()")
b { let! a = 123 in return a } |> assertEqual ((123, "x=a"), "f=return a, x=123")

b {
    let! a = 123
    let! b = 456
    return a + b
} |> assertEqual
  (((579, "x=a + b"), "f=return a + b, x=456"),
   "f=let! b = 456
    return a + b, x=123")
"""
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore
      
    [<Fact>]
    let ``Can use in Computation Expression 2`` () =
      FSharp """namespace System.Runtime.CompilerServices

open System

[<AttributeUsage(AttributeTargets.Parameter, AllowMultiple=false, Inherited=false)>]
type CallerArgumentExpressionAttribute(parameterName: string) =
  inherit Attribute()

  member val ParameterName = parameterName
  
namespace global
module A =
  let assertEqual a b = if a <> b then failwithf "not equal: %A and %A" a b
  open System.Runtime.CompilerServices

  type A() =
    static member B (``ab c``, [<CallerArgumentExpression "ab c">]?n) =
      defaultArg n "no value"

  async { return assertEqual(A.B "abc") "\"abc\"" } |> Async.RunSynchronously
      """
      |> withLangVersionPreview
      |> asExe
      |> compileAndRun
      |> shouldSucceed
      |> ignore
   
    [<FactForNETCOREAPP>]
    let ``Can use with Delegate and Quotation`` =
      FSharp """let assertEqual a b = if a <> b then failwithf "not equal: %A and %A" a b
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

type A =
  delegate of
    a: int *
    [<CallerArgumentExpression "a">] ?expr: string *
    [<CallerArgumentExpression "a"; Optional; DefaultParameterValue "">] expr2: string
      -> string * string option
let a = A (fun a expr expr2 -> expr2, expr)
a.Invoke(123 - 7) |> assertEqual ("123 - 7", Some "123 - 7")

open Microsoft.FSharp.Quotations.Patterns
match <@ a.Invoke(123 - 7) @> with
| Call(_, _, [_; Value (:? (string * string option) as value, _)]) -> assertEqual ("123 - 7", Some "123 - 7") value
| _ -> failwith "fail"
"""
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore
      
    [<FactForNETCOREAPP>]
    let ``Can use with Interface and Object Expression`` =
      FSharp """let assertEqual a b = if a <> b then failwithf "not equal: %A and %A" a b
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

type Interface1 =
  abstract member M:
    a: int *
    [<CallerArgumentExpression "a">] ?expr: string *
    [<CallerArgumentExpression "a"; Optional; DefaultParameterValue "">] expr2: string
      -> string * string option

{new Interface1 with
    member this.M(a, expr, expr2) = expr2, expr}.M(123 - 7) |> assertEqual ("123 - 7", Some "123 - 7")
"""
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore
      
    [<FactForNETCOREAPP>]
    let ``Only method calls with direct arguments will get the argument expression - test the warning`` () =
        FSharp """module Lib
let assertEqual a b = if a <> b then failwithf "not equal: %A and %A" a b
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

type A() =
  static member B (``ab c``, [<CallerArgumentExpression "ab c"; Optional; DefaultParameterValue "no value">]n: string) =
    n
    
A.B "abc" |> assertEqual "\"abc\""

(A.B) "abc" |> assertEqual "no value"
"abc" |> A.B |> assertEqual "no value"
A.B <| "abc" |> assertEqual "no value"
let f = A.B
f "abc" |> assertEqual "no value"
        """ 
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
          (Information 3885, Line 12, Col 2, Line 12, Col 5, "This usage blocks passing string representations of arguments to parameters annotated with [<CallerArgumentExpression>]. The default values of these parameters will be passed. Only the usages like `Method(arguments)` can capture the string representation of arguments. You can disable this warning by using '#nowarn \"3885\"' or '--nowarn:3885'.")
          (Information 3885, Line 13, Col 10, Line 13, Col 13, "This usage blocks passing string representations of arguments to parameters annotated with [<CallerArgumentExpression>]. The default values of these parameters will be passed. Only the usages like `Method(arguments)` can capture the string representation of arguments. You can disable this warning by using '#nowarn \"3885\"' or '--nowarn:3885'.")
          (Information 3885, Line 14, Col 1, Line 14, Col 4, "This usage blocks passing string representations of arguments to parameters annotated with [<CallerArgumentExpression>]. The default values of these parameters will be passed. Only the usages like `Method(arguments)` can capture the string representation of arguments. You can disable this warning by using '#nowarn \"3885\"' or '--nowarn:3885'.")
          (Information 3885, Line 15, Col 9, Line 15, Col 12, "This usage blocks passing string representations of arguments to parameters annotated with [<CallerArgumentExpression>]. The default values of these parameters will be passed. Only the usages like `Method(arguments)` can capture the string representation of arguments. You can disable this warning by using '#nowarn \"3885\"' or '--nowarn:3885'.")
        ]
        |> ignore
        
    [<FactForNETCOREAPP>]
    let ``Only method calls with direct arguments will get the argument expression`` () =
        FSharp """module Lib
let assertEqual a b = if a <> b then failwithf "not equal: %A and %A" a b
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

type A() =
  static member B (``ab c``, [<CallerArgumentExpression "ab c"; Optional; DefaultParameterValue "no value">]n: string) =
    n
    
A.B "abc" |> assertEqual "\"abc\""

(A.B) "abc" |> assertEqual "no value"
"abc" |> A.B |> assertEqual "no value"
A.B <| "abc" |> assertEqual "no value"
let f = A.B
f "abc" |> assertEqual "no value"
        """ 
        |> withLangVersionPreview
        |> withOptions ["/nowarn:3885"]
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore
        
  
    [<FactForNETCOREAPP>]
    let ``Can use with extension methods`` =
      FSharp """let assertEqual a b = if a <> b then failwithf "not equal: %A and %A" a b
open System.Runtime.CompilerServices

// type System.Object with
//   member this.A([<CallerArgumentExpression "this">] ?arg: string) = arg
  
[<Extension>]
type B =
    [<Extension>]
    static member C (this: #obj, [<CallerArgumentExpression "this">] ?arg: string) = arg

// (1 + 2).A() |> assertEqual (Some "1 + 2")
(1 + 2).C() |> assertEqual (Some "1 + 2")
"""
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore
        
    (* ------------ C# Interop tests ------------- *)
    [<FactForNETCOREAPP>]
    let ``C# can consume methods using CallerArgumentExpression receiving special parameter names`` () =
        let fs =
          FSharp """module Lib
let assertEqual a b = if a <> b then failwithf "not equal: %A and %A" a b
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

type A() =
  static member B (``ab c``, [<CallerArgumentExpression "ab c"; Optional; DefaultParameterValue "no value">]n: string) =
    n
        """ 
          |> withLangVersionPreview

        CSharp """Lib.assertEqual(Lib.A.B("abc"), "\"abc\"");"""
        |> withName "CSLib"
        |> withReferences [fs]
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore
        
    [<FactForDESKTOP>]
    let ``Can recognize CallerArgumentExpression defined in C#`` () =
      let cs =
        CSharp """using System.Runtime.CompilerServices;
        
namespace Test
{
    public static class AInCs
    {
        public static string B(int param, [CallerArgumentExpression("param")] string expr = null) => expr;
        public static string C<T>(this T param, [CallerArgumentExpression("param")] string expr = null) => expr;
    }
}

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class CallerArgumentExpressionAttribute : Attribute
    {
        public CallerArgumentExpressionAttribute(string param)
        {
            Param = param;
        }

        public string Param { get; }
    }
}
"""

      FSharp """let assertEqual a b = if a <> b then failwithf "not equal: %A and %A" a b
open System.Runtime.CompilerServices
open Test

type A() =
  static member B (``ab c``, [<CallerArgumentExpression "ab c">]?n) =
    defaultArg n "no value"

A.B "abc" |> assertEqual "\"abc\""
AInCs.B (123 - 7) |> assertEqual "123 - 7"
(123 - 7).C() |> assertEqual "123 - 7"
      """ 
      |> withLangVersionPreview
      |> withReferences [cs]
      |> asExe
      |> compileAndRun
      |> shouldSucceed
      |> ignore
        
    (* ------------ FSI tests ------------- *)
    
    [<FactForNETCOREAPP>]
    let ``Check in fsi`` () =
      let path = __SOURCE_DIRECTORY__ ++ "test script.fsx"
      FsxFromPath path
      |> withLangVersionPreview
      |> runFsi
      |> shouldSucceed
      |> ignore


    [<FactForNETCOREAPP>]
    let ``Check fsi #load`` () =
      let path = __SOURCE_DIRECTORY__ ++ "test script.fsx"
      Fsx $"""#load @"{path}" """
      |> withLangVersionPreview
      |> runFsi
      |> shouldSucceed
      |> ignore
      