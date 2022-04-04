namespace FSharp.Compiler.ComponentTests.EmittedIL

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ComputationExpressions =

    let computationExprLibrary =
        FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  "ComputationExprLibrary.fs"))
        |> withName "ComputationExprLibrary"

    let verifyCompilation compilation =
        compilation
        |> asFs
        |> withOptions ["--test:EmitFeeFeeAs100001"]
        |> asExe
        |> withReferences [computationExprLibrary]
        |> withNoOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyILBaseline

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ComputationExpr01.fs"|])>]
    let ``ComputationExpr01_fs`` compilation =
        compilation
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ComputationExpr02.fs"|])>]
    let ``ComputationExpr02_fs`` compilation =
        compilation
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ComputationExpr03.fs"|])>]
    let ``ComputationExpr03_fs`` compilation =
        compilation
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ComputationExpr04.fs"|])>]
    let ``ComputationExpr04_fs`` compilation =
        compilation
        |> verifyCompilation 

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ComputationExpr05.fs"|])>]
    let ``ComputationExpr05_fs`` compilation =
        compilation
        |> verifyCompilation 

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ComputationExpr06.fs"|])>]
    let ``ComputationExpr06_fs`` compilation =
        compilation
        |> verifyCompilation 

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ComputationExpr07.fs"|])>]
    let ``ComputationExpr07_fs`` compilation =
        compilation
        |> verifyCompilation 
