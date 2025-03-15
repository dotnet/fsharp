// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open Xunit
open FSharp.Test.Compiler
open FSharp.Test

module CustomAttributes_CallerArgumentExpression =

    [<FactForNETCOREAPP>]
    let ``Can consume CallerArgumentExpression in BCL methods`` () =
        FSharp """let assertEqual a b = if a <> b then failwithf "not equal: %A and %A" a b
try System.ArgumentException.ThrowIfNullOrWhiteSpace(Seq.init 50 (fun _ -> " ")
  (* comment *) 
  |> String.concat " ")
with :? System.ArgumentException as ex -> 
  assertEqual true (ex.Message.Contains("(Parameter 'Seq.init 50 (fun _ -> \" \")\n  (* comment *) \n  |> String.concat \" \""))
  

try System.ArgumentException.ThrowIfNullOrWhiteSpace(argument = (Seq.init 11 (fun _ -> " ")
  (* comment *) 
  |> String.concat " "))
with :? System.ArgumentException as ex -> 
  assertEqual true (ex.Message.Contains("(Parameter '(Seq.init 11 (fun _ -> \" \")\n  (* comment *) \n  |> String.concat \" \")"))
"""
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<FactForNETCOREAPP>]
    let ``Can define methods using CallerArgumentExpression in F#`` () =
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
    let ``Can define methods using CallerArgumentExpression F# with F#-style optional arguments`` () =
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

let stringABC = "abc"
assertEqual (A.aa(stringABC)) ("abc", ".cctor", 11, System.IO.Path.Combine(__SOURCE_DIRECTORY__, "test.fs"), "stringABC")
# 1 "test.fs"
assertEqual (A.aa(stringABC : string)) ("abc", ".cctor", 1, System.IO.Path.Combine(__SOURCE_DIRECTORY__, "test.fs"), "stringABC : string")
# 1 "test.fs"
assertEqual (A.aa(a = (stringABC : string))) ("abc", ".cctor", 1, System.IO.Path.Combine(__SOURCE_DIRECTORY__, "test.fs"), "(stringABC : string)")
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
    let ``#line can be inserted in the argument`` () =
      FSharp """let assertEqual a b = if a <> b then failwithf "not equal: %A and %A" a b
open System.Runtime.CompilerServices

type A() =
  static member B (``ab c``, [<CallerArgumentExpression "ab c">]?n) =
    defaultArg n "no value"

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
    let ``test Warns when cannot find the referenced parameter`` () =
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
    let ``Can self define CallerArgumentExpression`` () =
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
      
  A.B("abc"
#line 1
  : string)
  |> assertEqual "\"abc\"
#line 1
  : string"
  
  A.B "abc" |> assertEqual "\"abc\""  
  A.B ("abc": string) |> assertEqual "\"abc\": string"
"""
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore
        