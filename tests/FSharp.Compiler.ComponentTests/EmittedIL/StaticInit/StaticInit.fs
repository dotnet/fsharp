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
    [<Theory; FileInlineData("LetBinding01.fs", Realsig=BooleanOptions.Both)>]
    let ``LetBinding01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=StaticInit_Struct01.fs SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd StaticInit_Struct01.dll"	# StaticInit_Struct01.fs -
    [<Theory; FileInlineData("StaticInit_Struct01.fs", Realsig=BooleanOptions.Both)>]
    let ``StaticInit_Struct01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=StaticInit_Class01.fs SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd StaticInit_Class01.dll"	# StaticInit_Class01.fs -
    [<Theory; FileInlineData("StaticInit_Class01.fs", Realsig=BooleanOptions.Both)>]
    let ``StaticInit_Class01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=StaticInit_Module01.fs SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd StaticInit_Module01.dll"	# StaticInit_Module01.fs -
    [<Theory; FileInlineData("StaticInit_Module01.fs", Realsig=BooleanOptions.Both)>]
    let ``StaticInit_Module01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation
