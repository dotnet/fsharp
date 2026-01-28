// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Migrated from: tests/fsharpqa/Source/Stress/
// These tests verify the compiler can handle stress conditions like deeply nested expressions.
//
// CodeGeneratorFor2766.fsx - Tests parser handling of deeply nested unbalanced parens
// SeqExprCapacity.fsx - Tests compiler handling of large sequential expressions

namespace Conformance.Stress

open Xunit
open FSharp.Test.Compiler
open FSharp.Test
open System.Text

module StressTests =

    // ============================================================================
    // CodeGeneratorFor2766 - Deeply nested unbalanced parens (parser stress test)
    // ============================================================================

    /// Generate deeply nested unbalanced parentheses code
    /// From original CodeGeneratorFor2766.fsx
    let generateDeeplyNestedParens (depth: int) =
        let sb = StringBuilder()
        sb.AppendLine("// Stress test for FSharp1.0#2766 - Internal error on parser when given unbalanced and deeply nested parens") |> ignore
        sb.AppendLine("let x = (1 + ") |> ignore
        
        for i in 0..depth-1 do
            for _ in 0..i do
                sb.Append("    ") |> ignore
            sb.AppendLine("(1 +") |> ignore
        
        sb.ToString()

    /// Test that parser handles deeply nested parens (expected to fail with parse error)
    [<Fact>]
    let ``Stress - CodeGeneratorFor2766 - deeply nested unbalanced parens`` () =
        // Generate code with depth of 100 (reduced from 500 for test speed)
        let generatedCode = generateDeeplyNestedParens 100
        
        FSharp generatedCode
        |> asExe
        |> compile
        // This should fail with a parse error (unexpected end of input)
        |> shouldFail
        |> withDiagnosticMessageMatches "Unexpected end of input"

    /// Test with smaller depth
    [<Fact>]
    let ``Stress - CodeGeneratorFor2766 - nested parens depth 50`` () =
        let generatedCode = generateDeeplyNestedParens 50
        
        FSharp generatedCode
        |> asExe
        |> compile
        |> shouldFail

    // ============================================================================
    // SeqExprCapacity - Large sequential expressions (optimizer stress test)
    // ============================================================================

    /// Generate code with many sequential expressions
    /// From original SeqExprCapacity.fsx
    let generateSeqExprCapacity (count: int) =
        let sb = StringBuilder()
        sb.AppendLine("// Stress test for sequential expression capacity") |> ignore
        sb.AppendLine("let f () = ") |> ignore
        sb.AppendLine("    let i = 0") |> ignore
        sb.AppendLine("    let nestedFunction() = 0") |> ignore
        
        for i in 0..count-1 do
            let exprBody = 
                match i % 20 with
                | 0 -> "    printfn \"Hello, World\""
                | 1 -> "    1 + 3 * (int 4.3) |> ignore"
                | 2 -> "    let nestedFunction() = i + nestedFunction()"
                | _ -> "    do ()"
            sb.AppendLine(exprBody) |> ignore
        
        sb.AppendLine("    0") |> ignore
        sb.AppendLine("") |> ignore
        sb.AppendLine("[<EntryPoint>]") |> ignore
        sb.AppendLine("let main _ = f()") |> ignore
        sb.ToString()

    /// Test that compiler handles large sequential expression blocks
    [<Fact>]
    let ``Stress - SeqExprCapacity - 500 sequential expressions`` () =
        let generatedCode = generateSeqExprCapacity 500
        
        FSharp generatedCode
        |> asExe
        |> compile
        |> shouldSucceed

    /// Test with 1000 sequential expressions
    [<Fact>]
    let ``Stress - SeqExprCapacity - 1000 sequential expressions`` () =
        let generatedCode = generateSeqExprCapacity 1000
        
        FSharp generatedCode
        |> asExe
        |> compile
        |> shouldSucceed

    /// Test compileAndRun with sequential expressions
    [<Fact>]
    let ``Stress - SeqExprCapacity - run 200 sequential expressions`` () =
        let generatedCode = generateSeqExprCapacity 200
        
        FSharp generatedCode
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    // ============================================================================
    // Additional stress tests
    // ============================================================================

    /// Test deeply nested let bindings
    [<FactForNETCOREAPP>]
    let ``Stress - deeply nested let bindings`` () =
        let sb = StringBuilder()
        sb.AppendLine("let f () =") |> ignore
        
        for i in 0..99 do
            sb.AppendLine($"    let x{i} = {i}") |> ignore
        
        sb.AppendLine("    x99") |> ignore
        sb.AppendLine("") |> ignore
        sb.AppendLine("[<EntryPoint>]") |> ignore
        sb.AppendLine("let main _ = f()") |> ignore
        
        FSharp (sb.ToString())
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    /// Test deeply nested match expressions
    [<FactForNETCOREAPP>]
    let ``Stress - deeply nested match expressions`` () =
        let sb = StringBuilder()
        sb.AppendLine("[<EntryPoint>]") |> ignore
        sb.AppendLine("let main _ =") |> ignore
        sb.AppendLine("    let x = 0") |> ignore
        
        for i in 0..49 do
            sb.AppendLine($"    match x with") |> ignore
            sb.AppendLine($"    | {i} -> {i}") |> ignore
            sb.AppendLine($"    | _ ->") |> ignore
        
        sb.AppendLine("    0") |> ignore
        
        FSharp (sb.ToString())
        |> asExe
        |> compile
        |> shouldSucceed

    /// Test deeply nested if-then-else
    [<FactForNETCOREAPP>]
    let ``Stress - deeply nested if-then-else`` () =
        let sb = StringBuilder()
        sb.AppendLine("[<EntryPoint>]") |> ignore
        sb.AppendLine("let main _ =") |> ignore
        sb.AppendLine("    let x = 0") |> ignore
        
        for i in 0..99 do
            sb.AppendLine($"    if x = {i} then {i}") |> ignore
            sb.AppendLine($"    else") |> ignore
        
        sb.AppendLine("    0") |> ignore
        
        FSharp (sb.ToString())
        |> asExe
        |> compile
        |> shouldSucceed

    /// Test many type definitions
    [<FactForNETCOREAPP>]
    let ``Stress - many type definitions`` () =
        let sb = StringBuilder()
        
        for i in 0..49 do
            sb.AppendLine($"type T{i} = {{ Value{i}: int }}") |> ignore
        
        sb.AppendLine("") |> ignore
        sb.AppendLine("[<EntryPoint>]") |> ignore
        sb.AppendLine("let main _ =") |> ignore
        sb.AppendLine("    let t = { Value0 = 0 }") |> ignore
        sb.AppendLine("    t.Value0") |> ignore
        
        FSharp (sb.ToString())
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    /// Test many discriminated union cases
    [<FactForNETCOREAPP>]
    let ``Stress - many discriminated union cases`` () =
        let sb = StringBuilder()
        sb.AppendLine("type LargeUnion =") |> ignore
        
        for i in 0..99 do
            sb.AppendLine($"    | Case{i}") |> ignore
        
        sb.AppendLine("") |> ignore
        sb.AppendLine("[<EntryPoint>]") |> ignore
        sb.AppendLine("let main _ =") |> ignore
        sb.AppendLine("    let u = Case0") |> ignore
        sb.AppendLine("    match u with") |> ignore
        sb.AppendLine("    | Case0 -> 0") |> ignore
        sb.AppendLine("    | _ -> 1") |> ignore
        
        FSharp (sb.ToString())
        |> asExe
        |> compileAndRun
        |> shouldSucceed
