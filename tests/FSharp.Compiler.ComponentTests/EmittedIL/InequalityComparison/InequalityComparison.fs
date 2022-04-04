namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module InequalityComparison =

    let verifyCompilation compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withNoOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyILBaseline

    // SOURCE=InequalityComparison01.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd InequalityComparison01.exe"	# x <= y
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"InequalityComparison01.fs"|])>]
    let ``InequalityComparison01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=InequalityComparison02.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd InequalityComparison02.exe"	# x >= y
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"InequalityComparison02.fs"|])>]
    let ``InequalityComparison02_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=InequalityComparison03.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd InequalityComparison03.exe"	# x < y
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"InequalityComparison03.fs"|])>]
    let ``InequalityComparison03_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=InequalityComparison04.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd InequalityComparison04.exe"	# x > y
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"InequalityComparison04.fs"|])>]
    let ``InequalityComparison04_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=InequalityComparison05.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd InequalityComparison05.exe"	# if (x > y) then ... else ...
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"InequalityComparison05.fs"|])>]
    let ``InequalityComparison05_fs`` compilation =
        compilation
        |> verifyCompilation

