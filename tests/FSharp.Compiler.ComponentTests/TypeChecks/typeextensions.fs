module TypeChecks.typeextensions
open System.IO
open Xunit
open FSharp.Test.Compiler


[<Fact>]
let ``issue.16034`` () =
    let scriptPath = Path.Combine(
        __SOURCE_DIRECTORY__
        , "typeextensions"
        , "issue.16034"
        , "issue.16034.fsx"
        )
    RealFsxFromPath scriptPath
    |> withBaseLine
    |> verifyILBaseline
    |> compileAndRun
    |> verifyOutputWithDefaultBaseline
    
[<Fact>]
let ``issue.16034.check1`` () =
     
    let scriptPath = Path.Combine(
        __SOURCE_DIRECTORY__
        , "typeextensions"
        , "issue.16034"
        , "issue.16034.check1.fsx"
        )
    RealFsxFromPath scriptPath
    |> withBaseLine
    |> verifyBaseline

[<Fact>]
let ``issue.16034.check2`` () =
    let scriptPath = Path.Combine(
        __SOURCE_DIRECTORY__
        , "typeextensions"
        , "issue.16034"
        , "issue.16034.check2.fsx"
        )
    RealFsxFromPath scriptPath
    |> withBaseLine
    |> verifyBaseline
    
    