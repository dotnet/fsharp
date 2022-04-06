namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open System.IO
open FSharp.Test
open FSharp.Test.Compiler

module Tuples =

    let verifyCompilation compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withNoOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyILBaseline

    // SOURCE=Tuple01.fs      SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Tuple01.exe"		# Tuple01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Tuple01.fs"|])>]
    let ``Tuple01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=Tuple02.fs      SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Tuple02.exe"		# Tuple02.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Tuple02.fs"|])>]
    let ``Tuple02_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=Tuple03.fs      SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Tuple03.exe"		# Tuple03.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Tuple03.fs"|])>]
    let ``Tuple03_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=Tuple04.fs      SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Tuple04.exe"		# Tuple04.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Tuple04.fs"|])>]
    let ``Tuple04_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=Tuple05.fs      SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Tuple05.exe"		# Tuple05.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Tuple05.fs"|])>]
    let ``Tuple05_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=Tuple06.fs      SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Tuple06.exe"		# Tuple06.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Tuple06.fs"|])>]
    let ``Tuple06_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=Tuple07.fs      SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Tuple07.exe"		# Tuple07.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Tuple07.fs"|])>]
    let ``Tuple07_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=Tuple08.fs      SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Tuple08.exe"		# Tuple08.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Tuple08.fs"|])>]
    let ``Tuple08_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=OptionalArg01.fs      SCFLAGS="-g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd OptionalArg01.exe"		# OptionalArg01.fs - test optimizatons
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"OptionalArg01.fs"|])>]
    let ``OptionalArg01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=TupleMonster.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TupleMonster.exe"		# TupleMonster.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TupleMonster.fs"|])>]
    let ``TupleMonster_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=TupleElimination.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TupleElimination.exe"		# TupleElimination.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TupleElimination.fs"|])>]
    let ``TupleElimination_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=ValueTupleAliasConstructor.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ValueTupleAliasConstructor.exe"		# ValueTupleAliasConstructor.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ValueTupleAliasConstructor.fs"|])>]
    let ``ValueTupleAliasConstructor_fs`` compilation =
        compilation
        |> verifyCompilation
