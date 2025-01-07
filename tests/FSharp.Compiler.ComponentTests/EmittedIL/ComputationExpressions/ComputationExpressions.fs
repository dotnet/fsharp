namespace EmittedIL.RealInternalSignature

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ComputationExpressions =

    let withRealInternalSignature compilation =
        compilation
        |> withOptions ["--realsig+"]

    let withoutRealInternalSignature compilation =
        compilation
        |> withOptions ["--realsig-"]

    let computationExprLibrary =
        FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  "ComputationExprLibrary.fs"))
        |> withName "ComputationExprLibrary"
        |> withoutRealInternalSignature

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

    [<Theory; FileInlineData("ComputationExpr01.fs", Realsig=BooleanOptions.Both)>]
    let ``ComputationExpr01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; FileInlineData("ComputationExpr02.fs", Realsig=BooleanOptions.Both)>]
    let ``ComputationExpr02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; FileInlineData("ComputationExpr03.fs", Realsig=BooleanOptions.Both)>]
    let ``ComputationExpr03_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    [<Theory; FileInlineData("ComputationExpr04.fs", Realsig=BooleanOptions.Both)>]
    let ``ComputationExpr04_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation 

    [<Theory; FileInlineData("ComputationExpr05.fs", Realsig=BooleanOptions.Both)>]
    let ``ComputationExpr05_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation 

    [<Theory; FileInlineData("ComputationExpr06.fs", Realsig=BooleanOptions.Both)>]
    let ``ComputationExpr06_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation 

    [<Theory; FileInlineData("ComputationExpr07.fs", Realsig=BooleanOptions.Both)>]
    let ``ComputationExpr07_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation 
