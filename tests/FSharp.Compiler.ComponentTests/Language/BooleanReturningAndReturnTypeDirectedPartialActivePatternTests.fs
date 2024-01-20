// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module Language.BooleanReturningAndReturnTypeDirectedPartialActivePatternTests

open Xunit
open FSharp.Test.Compiler
open FSharp.Test.ScriptHelpers

let fsiSession = getSessionForEval [||] LangVersion.Preview

let runCode = evalInSharedSession fsiSession

[<Fact>]
let ``Partial struct active pattern returns ValueOption`1 without [<return:Struct>]`` () =
    FSharp "let (|P1|_|) x = ValueNone"
    |> withLangVersionPreview
    |> typecheck
    |> shouldSucceed

[<Fact>]
let ``Partial struct active pattern returns bool`` () =
    FSharp "let (|P1|_|) x = false"
    |> withLangVersionPreview
    |> typecheck
    |> shouldSucceed
    
[<Fact>]
let ``Single case active pattern returning bool should success`` () =
    FSharp """
let (|IsA|) x = x = "A"
let (IsA r) = "A"
    """
    |> withLangVersionPreview
    |> typecheck
    |> shouldSucceed
    
[<Fact>]
let ``Partial struct active pattern results can be retrieved`` () =
    Fsx """
let fail msg =
    printfn "%s" msg
    failwith msg

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
    |> withLangVersionPreview
    |> runCode
    |> shouldSucceed

// negative tests

[<Fact>]
let ``bool active pattern (-langversion:8.0)`` () =
    FSharp """let (|OddBool|_|) x = x % 2 = 1
let (|OddVOption|_|) x = if x % 2 = 1 then ValueSome() else ValueNone
        """
    |> withLangVersion80
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Warning 3350, Line 1, Col 4, Line 1, Col 19, "Feature 'Boolean-returning and return-type-directed partial active patterns' is not available in F# 8.0. Please use language version 'PREVIEW' or greater.")
        (Warning 3350, Line 2, Col 4, Line 2, Col 22, "Feature 'Boolean-returning and return-type-directed partial active patterns' is not available in F# 8.0. Please use language version 'PREVIEW' or greater.")
    ]

[<Fact>]
let ``Can not receive result from bool active pattern`` () =
    FSharp """let (|IsA|_|) x = x = "A"

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
    |> withLangVersionPreview
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Error 1, Line 4, Col 2, Line 4, Col 12,
         "This expression was expected to have type
    'string -> bool'    
but here has type
    'bool'    ")
        (Error 39, Line 4, Col 6, Line 4, Col 12,
         "The value or constructor 'result' is not defined. Maybe you want one of the following:
   Result")
        (Error 1, Line 8, Col 2, Line 8, Col 12,
         "This expression was expected to have type
    'string -> bool'    
but here has type
    'bool'    ")
        (Error 39, Line 8, Col 6, Line 8, Col 12,
         "The value or constructor 'result' is not defined. Maybe you want one of the following:
   Result")
        (Error 1, Line 12, Col 2, Line 12, Col 29,
         "This expression was expected to have type
    'string -> bool'    
but here has type
    'bool'    ")
    ]
