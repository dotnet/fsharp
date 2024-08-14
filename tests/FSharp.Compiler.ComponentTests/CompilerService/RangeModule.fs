// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerService

open Xunit
open System
open FSharp.Test.Compiler

module RangeModule =

    let useValidateRoundTripTemplate (insertedTestCase: string) =
        let exceptionMessage = """raise (new Exception($"Error round tripping '{label}': expected: '{expected}' actual: '{actual}'\n        arguments: startline '{startline}' startcolumn '{startcolumn}' endline '{endline}' endcolumn '{endcolumn}\n        Range Object: {r.ToString()}\n        Range Properties: r.StartLine '{r.StartLine}'  r.StartColumn '{r.StartColumn}' r.EndLine '{r.EndLine}' r.EndColumn '{r.EndColumn}'\n"))"""
        FSharp $"""
open System
open FSharp.Compiler.Text
open Range
open Position

let getRangeWithFileName filename startline startcolumn endline endcolumn =
    mkRange filename (mkPos startline startcolumn) (mkPos endline endcolumn)

// Get a range object given start and end columns and lines
let getRange startline startcolumn endline endcolumn = 
    getRangeWithFileName "DefaultTestFileName" startline startcolumn endline endcolumn
        
// If the range object sets the values to a value different to what is passed in then validRangeValuesUnchanged throws
// Exceptions at runtime indicate failure
let validRangeValuesUnchanged startline startcolumn endline endcolumn =
    let r = getRange startline startcolumn endline endcolumn
    let validateRoundTrip label expected actual =
        if expected <> actual then {exceptionMessage}

    validateRoundTrip "StartColumn" startcolumn r.StartColumn
    validateRoundTrip "EndColumn" endcolumn r.EndColumn

    validateRoundTrip "StartLine" startline r.StartLine
    validateRoundTrip "EndLine"   endline   r.EndLine


let MaximumColumnValue = 4194303
let MaximumStartLineValue = 2147483647
let MaximumEndLineValue = 134217727
{insertedTestCase}
"""

    [<Fact>]
    let ``Validate valid Smoke test Min and Max values stored in Range`` () =
        useValidateRoundTripTemplate """
validRangeValuesUnchanged 0 0 MaximumEndLineValue MaximumColumnValue
"""
        |> withReferenceFSharpCompilerService
        |> asExe
        |> compileAndRun
        |> shouldSucceed


    [<Fact>]
    let ``Validate valid EndColumn values are unchanged by storing in Range`` () =
        useValidateRoundTripTemplate """
validRangeValuesUnchanged 0 0 0 MaximumColumnValue
"""
        |> withReferenceFSharpCompilerService
        |> asExe
        |> compileAndRun
        |> shouldSucceed


    [<Fact>]
    let ``Validate valid EndLine values are unchanged by storing in Range`` () =
        useValidateRoundTripTemplate """
validRangeValuesUnchanged 0 0 MaximumEndLineValue 0
"""
        |> withReferenceFSharpCompilerService
        |> asExe
        |> compileAndRun
        |> shouldSucceed


    [<Fact>]
    let ``Validate valid StartColumn values are unchanged by storing in Range`` () =
        useValidateRoundTripTemplate """
// Start Column only
validRangeValuesUnchanged 0 MaximumColumnValue 0 0
"""
        |> withReferenceFSharpCompilerService
        |> asExe
        |> compileAndRun
        |> shouldSucceed


    [<Fact>]
    let ``Validate valid StartLine values are unchanged by storing in Range`` () =
        useValidateRoundTripTemplate """
validRangeValuesUnchanged (MaximumStartLineValue - MaximumEndLineValue) 0 MaximumStartLineValue 0
"""
        |> withReferenceFSharpCompilerService
        |> asExe
        |> compileAndRun
        |> shouldSucceed
