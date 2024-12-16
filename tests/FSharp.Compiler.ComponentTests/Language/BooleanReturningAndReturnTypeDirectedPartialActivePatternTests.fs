// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Because of shared fsi session.
[<FSharp.Test.RunTestCasesInSequence>]
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
        (Error 3350, Line 1, Col 6, Line 1, Col 17, "Feature 'Boolean-returning and return-type-directed partial active patterns' is not available in F# 8.0. Please use language version 9.0 or greater.")
        (Error 3350, Line 2, Col 6, Line 2, Col 20, "Feature 'Boolean-returning and return-type-directed partial active patterns' is not available in F# 8.0. Please use language version 9.0 or greater.")
    ]

[<Fact>]
let ``Cannot receive result from bool active pattern`` () =
    FSharp """#nowarn "20"
let (|IsA|_|) x = x = "A"

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
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        (Error 3868, Line 5, Col 3, Line 5, Col 13, "This active pattern does not expect any arguments, i.e., it should be used like 'IsA' instead of 'IsA x'.")
        (Error 3868, Line 9, Col 3, Line 9, Col 13, "This active pattern does not expect any arguments, i.e., it should be used like 'IsA' instead of 'IsA x'.")
        (Error 0039, Line 9, Col 17, Line 9, Col 23, "The value or constructor 'result' is not defined. Maybe you want one of the following:
   Result")
        (Error 3868, Line 13, Col 3, Line 13, Col 30, "This active pattern does not expect any arguments, i.e., it should be used like 'IsA' instead of 'IsA x'.")
    ]
