namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module GeneratedIterators =

    let verifyCompilation compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withLangVersionPreview // TODO https://github.com/dotnet/fsharp/issues/16739: Remove this when LanguageFeature.LowerIntegralRangesToFastLoops is out of preview.
        |> withNoOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyILBaseline

    // SOURCE=GenIter01.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd GenIter01.exe"	# GenIter01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"GenIter01.fs"|])>]
    let ``GenIter01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=GenIter02.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd GenIter02.exe"	# GenIter02.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"GenIter02.fs"|])>]
    let ``GenIter02_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=GenIter03.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd GenIter03.exe"	# GenIter03.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"GenIter03.fs"|])>]
    let ``GenIter03_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=GenIter04.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd GenIter04.exe"	# GenIter04.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"GenIter04_RealInternalSignatureOff.fs"|])>]
    let ``GenIter04_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=GenIter04.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd GenIter04.exe"	# GenIter04.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"GenIter04_RealInternalSignatureOn.fs"|])>]
    let ``GenIter04_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation
