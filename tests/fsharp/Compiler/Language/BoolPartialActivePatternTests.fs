// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Compiler.Diagnostics
open FSharp.Test

[<TestFixture>]
module BoolPartialActivePatternTests =

    let private pass = CompilerAssert.PassWithOptions [| "--langversion:preview" |]
    let private fail = CompilerAssert.TypeCheckWithErrorsAndOptions [| "--langversion:preview" |]
    let private run src = CompilerAssert.CompileExeAndRunWithOptions(
        [| "--langversion:preview" |],
        ("""
let fail msg =
    printfn "%s" msg
    failwith msg
""" + src))

    [<Test>]
    let ``Partial struct active pattern returns ValueOption`1 without [<return:Struct>]`` () =
        pass "let (|P1|_|) x = ValueNone"

    [<Test>]
    let ``Partial struct active pattern returns bool`` () =
        pass "let (|P1|_|) x = false"
        
    [<Test>]
    let ``Single case active pattern returning bool should success`` () =
        pass """
let (|IsA|) x = x = "A"
let (IsA r) = "A"
        """
        
    [<Test>]
    let ``Partial struct active pattern results can be retrieved`` () =
        run """
let (|P1|_|) x = x <> 0
let (|EqualTo|_|) y x = x = y

match 0, 1 with
| P1, _ -> fail "unit"
| _, P1 -> ()
| _     -> fail "unit"

match "x" with
| EqualTo "y" -> fail "with argument"
| EqualTo "x" -> ()
| _ -> fail "with argument"
            """

// negative tests

    [<Test>]
    let ``bool active pattern (-langversion:8.0)`` () =
        CompilerAssert.TypeCheckWithErrorsAndOptions  [| "--langversion:8.0" |]
            """let (|OddBool|_|) x = x % 2 = 1
let (|OddVOption|_|) x = if x % 2 = 1 then ValueSome() else ValueNone
            """
            [|(FSharpDiagnosticSeverity.Error, 3350, (1, 5, 1, 20),
               "Feature 'bool representation for partial active patterns' is not available in F# 8.0. Please use language version 'PREVIEW' or greater.")
              (FSharpDiagnosticSeverity.Error, 3350, (2, 5, 2, 23),
               "Feature 'bool representation for partial active patterns' is not available in F# 8.0. Please use language version 'PREVIEW' or greater.")|]

    [<Test>]
    let ``Can not receive result from bool active pattern`` () =
        fail
            """let (|IsA|_|) x = x = "A"

match "A" with 
| IsA result -> "A" 
| _ -> "Not A"

match "A" with 
| IsA result -> result
| _ -> "Not A"

match "A" with 
| IsA "to match return value" -> "Matched"
| _ -> "not Matched"
"""
            [|(FSharpDiagnosticSeverity.Error, 1, (4, 3, 4, 13),
               "This expression was expected to have type
    'string -> bool'    
but here has type
    'bool'    ");
              (FSharpDiagnosticSeverity.Error, 39, (4, 7, 4, 13),
               "The value or constructor 'result' is not defined. Maybe you want one of the following:
   Result");
              (FSharpDiagnosticSeverity.Error, 1, (8, 3, 8, 13),
               "This expression was expected to have type
    'string -> bool'    
but here has type
    'bool'    ");
              (FSharpDiagnosticSeverity.Error, 39, (8, 7, 8, 13),
               "The value or constructor 'result' is not defined. Maybe you want one of the following:
   Result");
              (FSharpDiagnosticSeverity.Error, 1, (12, 3, 12, 30),
               "This expression was expected to have type
    'string -> bool'    
but here has type
    'bool'    ");
   |]
