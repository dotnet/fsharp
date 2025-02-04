// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open Xunit
open FSharp.Test.Compiler

module CustomAttributes_CallerArgumentExpression =

    [<Fact>]
    let ``Can consume CallerArgumentExpression in BCL methods`` () =
        FSharp """module Program
try System.ArgumentNullException.ThrowIfNullOrWhiteSpace(Seq.init 50 (fun _ -> " ")
  (* comment *) 
  |> String.concat " ")
with :? System.ArgumentException as ex -> 
  assert (ex.Message.Contains("(Parameter 'Seq.init 50 (fun _ -> \" \")\n  (* comment *) \n  |> String.concat \" \""))
"""
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Can define in F#`` () =
        FSharp """namespace global
module Program =
  open System.Runtime.CompilerServices
  type A() =
    static member aa (
      a,
      [<CallerMemberName; Optional; DefaultParameterValue "no value">]b: string, 
      [<CallerLineNumber; Optional; DefaultParameterValue 0>]c: int, 
      [<CallerFilePath; Optional; DefaultParameterValue "no value">]d: string, 
      [<CallerArgumentExpressionAttribute("a"); Optional; DefaultParameterValue "no value">]e: string) = 
      a,b,c,d,e

  let stringABC = "abc"
  assert (A.aa(stringABC) = ("abc", ".ctor", 14, "C:\Program.fs", "stringABC"))
        """
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Can define in F# with F#-style optional arguments`` () =
        FSharp """namespace global
module Program =
  open System.Runtime.CompilerServices
  type A() =
    static member aa (
      a,
      [<CallerMemberName>] ?b: string, 
      [<CallerLineNumber>] ?c: int, 
      [<CallerFilePath>] ?d: string, 
      [<CallerArgumentExpressionAttribute("a")>] ?e: string) = 
      let b = defaultArg b "no value"
      let c = defaultArg c 0
      let d = defaultArg d "no value"
      let e = defaultArg e "no value"
      a,b,c,d,e

  let stringABC = "abc"
  assert (A.aa(stringABC) = ("abc", ".ctor", 18, "C:\Program.fs", "stringABC"))
        """
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed
        |> ignore

        
    [<Fact>]
    let ``Can define in F# - with #line`` () =
        FSharp """namespace global
module Program =
# 1 "C:\\Program.fs"
  open System.Runtime.CompilerServices
  type A() =
    static member aa (
      a,
      [<CallerMemberName; Optional; DefaultParameterValue "no value">]b: string, 
      [<CallerLineNumber; Optional; DefaultParameterValue 0>]c: int, 
      [<CallerFilePath; Optional; DefaultParameterValue "no value">]d: string, 
      [<CallerArgumentExpressionAttribute("a"); Optional; DefaultParameterValue "no value">]e: string) = 
      a,b,c,d,e

  let stringABC = "abc"
  assert (A.aa(stringABC) = ("abc", ".ctor", 15, "C:\Program.fs", "stringABC"))
        """
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed
        |> ignore

