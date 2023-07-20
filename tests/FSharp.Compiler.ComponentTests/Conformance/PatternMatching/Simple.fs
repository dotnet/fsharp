// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Simple =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    //<Expects id="FS0025" span="(92,13-92,14)" status="warning">Incomplete pattern matches on this expression. For example, the value 'Result \(_\)' may indicate a case not covered by the pattern\(s\)</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"W_Incomplete01.fs"|])>]
    let ``Simple - W_Incomplete01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0025
        |> withDiagnosticMessageMatches "Incomplete pattern matches on this expression. For example, the value 'Result \(_\)' may indicate a case not covered by the pattern\(s\)"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    //<Expects id="FS0026" span="(32,11-32,13)" status="warning">This rule will never be matched</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"W_Incomplete02.fs"|])>]
    let ``Simple - W_Incomplete02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withWarningCode 0026
        |> withDiagnosticMessageMatches "This rule will never be matched"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Simple)
    //<Expects id="FS0049" span="(10,16-10,19)" status="warning">Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Simple", Includes=[|"W_BindCaptialIdent.fs"|])>]
    let ``Simple - W_BindCaptialIdent_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withWarningCode 0049
        |> withDiagnosticMessageMatches "Uppercase variable identifiers should not generally be used in patterns, and may indicate a missing open declaration or a misspelt pattern name"
        |> ignore

    [<Fact>]
    let ``As patterns``() =
        Fsx """
        let (|Id|) = id
        let a = [1..4]
        match a with
        | 1 | 1 as b::(Id 2 as c as c2)::[d as 3; Id e & Id _ as Id 4] as Id f when b = 1 && c = 2 && c2 = 2 && d = 3 && e = 4 && a = f -> ()
        | _ -> failwith "Match failed"
        """
        |> asExe
        |> withLangVersion60
        |> compileExeAndRun
        |> shouldSucceed


    [<Theory>]
    [<InlineData("DateTime", "DateTime.Now")>]
    [<InlineData("int", "1")>]
    [<InlineData("Guid", "(Guid.NewGuid())")>]
    [<InlineData("Byte", "0x1")>]
    [<InlineData("Decimal", "1m")>]
    let ``Test type matching for subtypes and interfaces`` typ value =
        Fsx $"""
open System
let classify (o: obj) =
    match o with
    | :? {typ} as d when d = Unchecked.defaultof<_> -> "default"
    | :? IFormattable -> "formattable"
    | _ -> "not a {typ}"

let res = classify {value}
if res <> "formattable" then
    failwith $"Unexpected result: {{res}}"
         """
         |> asExe
         |> compileAndRun
         |> shouldSucceed


    [<Fact>]
    let ``Enum incompleteness check should not hide an issue with outer DU pattern matching with nowarn:104 `` () = 
        Fsx """
type E = A = 0

type Ex =
    | ExA of int * E
    | ExB of int
    
let flub ex =
    match ex with
    | ExA(_, E.A) -> ()
    
flub (ExB 3)
        """
        |> withNoWarn 104        
        |> typecheck
        |> shouldFail
        |> withDiagnostics [Warning 25, Line 9, Col 11, Line 9, Col 13, "Incomplete pattern matches on this expression. For example, the value 'ExB (_)' may indicate a case not covered by the pattern(s)."]

    [<Fact>]
    let ``Enum incompleteness check in nested scenarios should report all warnings`` () = 
        Fsx """
type E =
    | FieldA = 1
    | FieldB = 2

type U =
    | CaseA
    | CaseB of E

match CaseA with
| CaseB E.FieldA -> ()
| CaseB E.FieldB -> ()
        """     
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
                Warning 104, Line 10, Col 7, Line 10, Col 12, "Enums may take values outside known cases. For example, the value 'CaseB (enum<E> (0))' may indicate a case not covered by the pattern(s)."
                Warning 25, Line 10, Col 7, Line 10, Col 12, "Incomplete pattern matches on this expression. For example, the value 'CaseA' may indicate a case not covered by the pattern(s)."]
   