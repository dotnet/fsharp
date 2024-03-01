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

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"ComputationExpr01.fs"|])>]
    let ``ComputationExpr01_realsig=true`` compilation =
        compilation
        |> withRealInternalSignature
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"ComputationExpr01.fs"|])>]
    let ``ComputationExpr01_realsig=false`` compilation =
        compilation
        |> withoutRealInternalSignature
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"ComputationExpr02.fs"|])>]
    let ``ComputationExpr02_realsig=true`` compilation =
        compilation
        |> withRealInternalSignature
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"ComputationExpr02.fs"|])>]
    let ``ComputationExpr02_realsig=false`` compilation =
        compilation
        |> withoutRealInternalSignature
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"ComputationExpr03.fs"|])>]
    let ``ComputationExpr03_realsig=true`` compilation =
        compilation
        |> withRealInternalSignature
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"ComputationExpr03.fs"|])>]
    let ``ComputationExpr03_realsig=false`` compilation =
        compilation
        |> withoutRealInternalSignature
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"ComputationExpr04.fs"|])>]
    let ``ComputationExpr04_realsig=true`` compilation =
        compilation
        |> withRealInternalSignature
        |> verifyCompilation 

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"ComputationExpr04.fs"|])>]
    let ``ComputationExpr04_realsig=false`` compilation =
        compilation
        |> withoutRealInternalSignature
        |> verifyCompilation 

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"ComputationExpr05.fs"|])>]
    let ``ComputationExpr05_realsig=true`` compilation =
        compilation
        |> withRealInternalSignature
        |> verifyCompilation 

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"ComputationExpr05.fs"|])>]
    let ``ComputationExpr05_realsig=false`` compilation =
        compilation
        |> withoutRealInternalSignature
        |> verifyCompilation 

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"ComputationExpr06.fs"|])>]
    let ``ComputationExpr06_realsig=true`` compilation =
        compilation
        |> withRealInternalSignature
        |> verifyCompilation 

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"ComputationExpr06.fs"|])>]
    let ``ComputationExpr06_realsig=false`` compilation =
        compilation
        |> withoutRealInternalSignature
        |> verifyCompilation 

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"ComputationExpr07.fs"|])>]
    let ``ComputationExpr07_realsig=true`` compilation =
        compilation
        |> withRealInternalSignature
        |> verifyCompilation 

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"ComputationExpr07.fs"|])>]
    let ``ComputationExpr07_realsig=false`` compilation =
        compilation
        |> withoutRealInternalSignature
        |> withLangVersionPreview // TODO https://github.com/dotnet/fsharp/issues/16739: Remove this when LanguageFeature.LowerIntegralRangesToFastLoops is out of preview.
        |> verifyCompilation 
