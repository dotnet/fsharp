// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open System
open Xunit
open FSharp.Test.ScriptHelpers
open FSharp.Compiler.Diagnostics

module FSharpPlusRegressionTests =

    /// <summary>
    /// Regression test for FSharpPlus issue #613 - monad.plus usage scenario.
    /// This test reproduces a consumer-side failure where using monad.plus in F# 9
    /// causes compilation issues. The code should compile successfully.
    /// Issue: https://github.com/fsprojects/FSharpPlus/issues/613
    /// </summary>
    [<Theory>]
    [<InlineData("8.0")>]
    [<InlineData("preview")>]
    [<InlineData("preview", "--checknulls+")>]
    let ``monad.plus usage should compile successfully`` (langVersion: string, [<ParamArray>] additionalArgs: string[]) =
        let allArgs = Array.concat [[| "--langversion:" + langVersion |]; additionalArgs]
        use script = new FSharpScript(additionalArgs = allArgs, quiet = true)
        
        let code = """
// Simulated monad.plus pattern from FSharpPlus
// This pattern uses statically resolved type parameters (SRTP) for ad-hoc polymorphism
type MonadPlusClass =
    static member inline MPlus (x: option<'a>, y: option<'a>) = 
        match x with
        | Some _ -> x
        | None -> y
    
    static member inline MPlus (x: list<'a>, y: list<'a>) = x @ y

// Generic mplus function using SRTP to dispatch to appropriate implementation
let inline mplus (x: ^M) (y: ^M) : ^M =
    ((^MonadPlusClass or ^M) : (static member MPlus : ^M * ^M -> ^M) (x, y))

// Direct usage with concrete types
let testOption() =
    let result : int option = mplus (Some 1) (Some 2)
    printfn "Option result = %A" result

let testList() =
    let result : int list = mplus [1; 2] [3; 4]
    printfn "List result = %A" result

testOption()
testList()
"""
        
        let evalResult, diagnostics = script.Eval(code)
        
        // The code should compile successfully
        match evalResult with
        | Ok _ -> 
            // Filter out informational diagnostics
            let errors = diagnostics |> Array.filter (fun d -> 
                d.Severity = FSharpDiagnosticSeverity.Error)
            Assert.Empty(errors)
        | Error ex -> 
            Assert.True(false, sprintf "Evaluation failed with exception: %s\nDiagnostics: %A" ex.Message diagnostics)

    /// <summary>
    /// Regression test for FSharpPlus issue #613 - custom ResultTBuilder scenario.
    /// This test reproduces a consumer-side failure where defining a custom ResultTBuilder
    /// in F# 9 causes compilation issues. The code should compile successfully.
    /// Issue: https://github.com/fsprojects/FSharpPlus/issues/613
    /// </summary>
    [<Theory>]
    [<InlineData("8.0")>]
    [<InlineData("preview")>]
    [<InlineData("preview", "--checknulls+")>]
    let ``custom ResultTBuilder should compile successfully`` (langVersion: string, [<ParamArray>] additionalArgs: string[]) =
        let allArgs = Array.concat [[| "--langversion:" + langVersion |]; additionalArgs]
        use script = new FSharpScript(additionalArgs = allArgs, quiet = true)
        
        let code = """
// Custom ResultTBuilder pattern from FSharpPlus
type ResultTBuilder() =
    member inline _.Return(x: 'T) : Result<'T, 'Error> = Ok x
    
    member inline _.ReturnFrom(m: Result<'T, 'Error>) : Result<'T, 'Error> = m
    
    member inline _.Bind(m: Result<'T, 'Error>, f: 'T -> Result<'U, 'Error>) : Result<'U, 'Error> =
        match m with
        | Ok x -> f x
        | Error e -> Error e
    
    member inline _.Zero() : Result<unit, 'Error> = Ok ()
    
    member inline _.Combine(m1: Result<unit, 'Error>, m2: Result<'T, 'Error>) : Result<'T, 'Error> =
        match m1 with
        | Ok () -> m2
        | Error e -> Error e
    
    member inline _.Delay(f: unit -> Result<'T, 'Error>) : unit -> Result<'T, 'Error> = f
    
    member inline _.Run(f: unit -> Result<'T, 'Error>) : Result<'T, 'Error> = f()

let resultT = ResultTBuilder()

// Usage example
let compute x y =
    resultT {
        let! a = Ok x
        let! b = Ok y
        return a + b
    }

// Apply the function to avoid value restriction
let testResult : Result<int, string> = compute 5 10

// Verify result
printfn "testResult = %A" testResult
"""
        
        let evalResult, diagnostics = script.Eval(code)
        
        // The code should compile successfully
        match evalResult with
        | Ok _ -> 
            // Filter out informational diagnostics
            let errors = diagnostics |> Array.filter (fun d -> 
                d.Severity = FSharpDiagnosticSeverity.Error)
            Assert.Empty(errors)
        | Error ex -> 
            Assert.True(false, sprintf "Evaluation failed with exception: %s\nDiagnostics: %A" ex.Message diagnostics)
