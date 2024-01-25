namespace EmittedIL.RealInternalSignature

open Xunit
open System.IO
open FSharp.Test
open FSharp.Test.Compiler

module StaticInit =

    let verifyCompilation compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withNoOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyILBaseline

    // SOURCE=LetBinding01.fs SCFLAGS="   -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd LetBinding01.exe"			# LetBinding01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"LetBinding01.fs"|])>]
    let ``LetBinding01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=LetBinding01.fs SCFLAGS="   -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd LetBinding01.exe"			# LetBinding01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"LetBinding01.fs"|])>]
    let ``LetBinding01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=StaticInit_Struct01.fs SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd StaticInit_Struct01.dll"	# StaticInit_Struct01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"StaticInit_Struct01.fs"|])>]
    let ``StaticInit_Struct01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=StaticInit_Struct01.fs SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd StaticInit_Struct01.dll"	# StaticInit_Struct01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"StaticInit_Struct01.fs"|])>]
    let ``StaticInit_Struct01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=StaticInit_Class01.fs SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd StaticInit_Class01.dll"	# StaticInit_Class01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"StaticInit_Class01.fs"|])>]
    let ``StaticInit_Class01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=StaticInit_Class01.fs SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd StaticInit_Class01.dll"	# StaticInit_Class01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"StaticInit_Class01.fs"|])>]
    let ``StaticInit_Class01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=StaticInit_Module01.fs SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd StaticInit_Module01.dll"	# StaticInit_Module01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"StaticInit_Module01.fs"|])>]
    let ``StaticInit_Module01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=StaticInit_Module01.fs SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd StaticInit_Module01.dll"	# StaticInit_Module01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"StaticInit_Module01.fs"|])>]
    let ``StaticInit_Module01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation
