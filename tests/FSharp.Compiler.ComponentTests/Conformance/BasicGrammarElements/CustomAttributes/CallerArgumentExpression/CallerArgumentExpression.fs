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

    [<FactForNETCOREAPP(Skip = "Currently cannot get the original text range with #line")>]
    let ``Can define in F# - with #line`` () =
        FSharp """let assertEqual a b = if a <> b then failwithf "not equal: %A and %A" a b
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

let path = System.IO.Path.Combine(__SOURCE_DIRECTORY__, "test.fs")

# 1 "test.fs"
type A() =
  static member aa (
    a,
    [<CallerMemberName; Optional; DefaultParameterValue "no value">]b: string, 
    [<CallerLineNumber; Optional; DefaultParameterValue 0>]c: int, 
    [<CallerFilePath; Optional; DefaultParameterValue "no value">]d: string, 
    [<CallerArgumentExpressionAttribute("a"); Optional; DefaultParameterValue "no value">]e: string) = 
    a,b,c,d,e
    
  static member B (``ab c``, [<CallerArgumentExpression "ab c">]?n) =
    defaultArg n "no value"

let stringABC = "abc"
assertEqual (A.aa(stringABC)) ("abc", ".cctor", 11, path, "stringABC")
# 1 "test.fs"
assertEqual (A.aa(stringABC : string)) ("abc", ".cctor", 1, path, "stringABC : string")
# 1 "test.fs"
assertEqual (A.aa(a = (stringABC : string))) ("abc", ".cctor", 1, path, "(stringABC : string)")


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
          (Warning 3875,Line 5, Col 65, Line 5, Col 66, "The CallerArgumentExpression on this parameter will have no effect because it's applied with an invalid parameter name.")
          (Warning 3875,Line 7, Col 65 , Line 7, Col 66, "The CallerArgumentExpression on this parameter will have no effect because it's self-referential.")
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
    let ``Can use with Computation Expression`` =
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
public class AInCs
{
    public static string B(int param, [CallerArgumentExpression("param")] string expr = null) => expr;
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

type A() =
  static member B (``ab c``, [<CallerArgumentExpression "ab c">]?n) =
    defaultArg n "no value"

A.B "abc" |> assertEqual "\"abc\""
AInCs.B (123 - 7) |> assertEqual "123 - 7"
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
