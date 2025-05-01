namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module GeneratedIterators =

    let verifyCompilation compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withNoOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyILBaseline

    // SOURCE=GenIter01.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd GenIter01.exe"	# GenIter01.fs -
    [<Theory; FileInlineData("GenIter01.fs")>]
    let ``GenIter01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=GenIter02.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd GenIter02.exe"	# GenIter02.fs -
    [<Theory; FileInlineData("GenIter02.fs")>]
    let ``GenIter02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=GenIter03.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd GenIter03.exe"	# GenIter03.fs -
    [<Theory; FileInlineData("GenIter03.fs")>]
    let ``GenIter03_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=GenIter04.fs   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd GenIter04.exe"	# GenIter04.fs -
    [<Theory; FileInlineData("GenIter04.fs", Realsig=BooleanOptions.Both)>]
    let ``GenIter04_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation
