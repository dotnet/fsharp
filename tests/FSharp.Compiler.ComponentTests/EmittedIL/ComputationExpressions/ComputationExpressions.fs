namespace EmittedIL.RealInternalSignature

open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ComputationExpressions =

    let withRealInternalSignature compilation =
        compilation
        |> withOptions ["--realsig-"]

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

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ComputationExpr01.fs"|])>]
    let ``ComputationExpr01_realsig=true`` compilation =
        compilation
        |> withRealInternalSignature
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ComputationExpr01.fs"|])>]
    let ``ComputationExpr01_realsig=false`` compilation =
        compilation
        |> withoutRealInternalSignature
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ComputationExpr02.fs"|])>]
    let ``ComputationExpr02_realsig=true`` compilation =
        compilation
        |> withRealInternalSignature
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ComputationExpr02.fs"|])>]
    let ``ComputationExpr02_realsig=false`` compilation =
        compilation
        |> withoutRealInternalSignature
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ComputationExpr03.fs"|])>]
    let ``ComputationExpr03_realsig=true`` compilation =
        compilation
        |> withRealInternalSignature
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ComputationExpr03.fs"|])>]
    let ``ComputationExpr03_realsig=false`` compilation =
        compilation
        |> withoutRealInternalSignature
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ComputationExpr04.fs"|])>]
    let ``ComputationExpr04_realsig=true`` compilation =
        compilation
        |> withRealInternalSignature
        |> verifyCompilation 

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ComputationExpr04.fs"|])>]
    let ``ComputationExpr04_realsig=false`` compilation =
        compilation
        |> withoutRealInternalSignature
        |> verifyCompilation 

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ComputationExpr05.fs"|])>]
    let ``ComputationExpr05_realsig=true`` compilation =
        compilation
        |> withRealInternalSignature
        |> verifyCompilation 

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ComputationExpr05.fs"|])>]
    let ``ComputationExpr05_realsig=false`` compilation =
        compilation
        |> withoutRealInternalSignature
        |> verifyCompilation 

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ComputationExpr06.fs"|])>]
    let ``ComputationExpr06_realsig=true`` compilation =
        compilation
        |> withRealInternalSignature
        |> verifyCompilation 

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ComputationExpr06.fs"|])>]
    let ``ComputationExpr06_realsig=false`` compilation =
        compilation
        |> withoutRealInternalSignature
        |> verifyCompilation 

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ComputationExpr07.fs"|])>]
    let ``ComputationExpr07_realsig=true`` compilation =
        compilation
        |> withRealInternalSignature
        |> verifyCompilation 

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ComputationExpr07.fs"|])>]
    let ``ComputationExpr07_realsig=false`` compilation =
        compilation
        |> withoutRealInternalSignature
        |> verifyCompilation 
