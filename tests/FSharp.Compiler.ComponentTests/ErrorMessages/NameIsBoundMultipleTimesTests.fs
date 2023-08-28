// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace ErrorMessages

open Xunit
open FSharp.Test.Compiler


module NameIsBoundMultipleTimes =
    [<Fact>]
    let ``Name is bound multiple times is not reported in 'as' pattern``() =
        Fsx """
let f1 a a = ()
let f2 (a, b as c) c = ()
let f3 (a, b as c) a = ()
let f4 (a, b, c as d) a c = ()
let f5 (a, b, c as d) a d = ()
"""
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 38, Line 2, Col 10, Line 2, Col 11, "'a' is bound twice in this pattern")
            (Error 38, Line 3, Col 20, Line 3, Col 21, "'c' is bound twice in this pattern")
            (Error 38, Line 4, Col 20, Line 4, Col 21, "'a' is bound twice in this pattern")
            (Error 38, Line 5, Col 23, Line 5, Col 24, "'a' is bound twice in this pattern")
            (Error 38, Line 6, Col 23, Line 6, Col 24, "'a' is bound twice in this pattern")
            (Error 38, Line 6, Col 25, Line 6, Col 26, "'d' is bound twice in this pattern")
        ]
        
    [<Fact>]
    let ``CI Failure``() =
        Fsx """
let GetMethodRefInfoAsMethodRefOrDef isAlwaysMethodDef cenv env (nm, ty, cc, args, ret, varargs, genarity as minfo) =
    ()

let mdorTag = GetMethodRefInfoAsMethodRefOrDef false false false (false, "", "", "", "", "", 0)

"""
        |> typecheck
        |> shouldSucceed
        
    [<Fact>]
    let ``CI Failure 2``() =
        Fsx """
open System
[<AbstractClass>]
type PrintfEnv<'State, 'Residue, 'Result>(state: 'State) =
    member _.State = state

    abstract Finish: unit -> 'Result

    abstract Write: string -> unit
    
    /// Write the result of a '%t' format.  If this is a string it is written. If it is a 'unit' value
    /// the side effect has already happened
    abstract WriteT: 'Residue -> unit

    member env.WriteSkipEmpty(s: string) = 
        if not (String.IsNullOrEmpty s) then 
            env.Write s

    member env.RunSteps (args: obj[], argTys: Type[], steps: string []) =
        let mutable argIndex = 0
        let mutable tyIndex = 0

        env.Finish()

let StringBuilderPrintfEnv<'Result>(k, buf) = 
        { new PrintfEnv<Text.StringBuilder, unit, 'Result>(buf) with
            override _.Finish() : 'Result = k ()
            override _.Write(s: string) = ignore(buf.Append s)
            override _.WriteT(()) = () }
"""
        |> typecheck
        |> shouldSucceed