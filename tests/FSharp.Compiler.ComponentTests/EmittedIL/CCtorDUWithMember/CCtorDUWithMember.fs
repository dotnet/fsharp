namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module CCtorDUWithMember =

    let verifyCompilation compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withNoOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyBaseline
        |> verifyILBaseline

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"CCtorDUWithMember01a.fs"|])>]
    let ``CCtorDUWithMember01a_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> asFs
        |> withRealInternalSignatureOn
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "CCtorDUWithMember01.fs"))
        |> verifyCompilation 

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"CCtorDUWithMember01a.fs"|])>]
    let ``CCtorDUWithMember01a_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> asFs
        |> withRealInternalSignatureOff
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "CCtorDUWithMember01.fs"))
        |> verifyCompilation 

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"CCtorDUWithMember02a.fs"|])>]
    let ``CCtorDUWithMember02a_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> asFs
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "CCtorDUWithMember02.fs"))
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"CCtorDUWithMember02a.fs"|])>]
    let ``CCtorDUWithMember02a_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> asFs
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "CCtorDUWithMember02.fs"))
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"CCtorDUWithMember03a.fs"|])>]
    let ``CCtorDUWithMember03a_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> asFs
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "CCtorDUWithMember03.fs"))
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"CCtorDUWithMember03a.fs"|])>]
    let ``CCtorDUWithMember03a_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> asFs
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "CCtorDUWithMember03.fs"))
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[| "CCtorDUWithMember04a.fs" |])>]
    let ``CCtorDUWithMember04a_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> asFs
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "CCtorDUWithMember04.fs"))
        |> verifyCompilation 

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[| "CCtorDUWithMember04a.fs" |])>]
    let ``CCtorDUWithMember04a_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> asFs
        |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "CCtorDUWithMember04.fs"))
        |> verifyCompilation 
