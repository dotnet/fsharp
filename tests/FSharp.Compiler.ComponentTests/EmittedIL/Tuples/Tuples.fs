namespace EmittedIL.RealInternalSignature

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
    [<Theory; FileInlineData("Tuple01.fs")>]
    let ``Tuple01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Tuple02.fs      SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Tuple02.exe"		# Tuple02.fs -
    [<Theory; FileInlineData("Tuple02.fs", Realsig=BooleanOptions.Both)>]
    let ``Tuple02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Tuple03.fs      SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Tuple03.exe"		# Tuple03.fs -
    [<Theory; FileInlineData("Tuple03.fs", Realsig=BooleanOptions.Both)>]
    let ``Tuple03_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Tuple04.fs      SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Tuple04.exe"		# Tuple04.fs -
    [<Theory; FileInlineData("Tuple04.fs", Realsig=BooleanOptions.Both)>]
    let ``Tuple04_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Tuple05.fs      SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Tuple05.exe"		# Tuple05.fs -
    [<Theory; FileInlineData("Tuple05.fs", Realsig=BooleanOptions.Both)>]
    let ``Tuple05_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Tuple06.fs      SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Tuple06.exe"		# Tuple06.fs -
    [<Theory; FileInlineData("Tuple06.fs", Realsig=BooleanOptions.Both)>]
    let ``Tuple06_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Tuple07.fs      SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Tuple07.exe"		# Tuple07.fs -
    [<Theory; FileInlineData("Tuple07.fs", Realsig=BooleanOptions.Both)>]
    let ``Tuple07_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Tuple08.fs      SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Tuple08.exe"		# Tuple08.fs -
    [<Theory; FileInlineData("Tuple08.fs", Realsig=BooleanOptions.Both)>]
    let ``Tuple08_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=OptionalArg01.fs      SCFLAGS="-g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd OptionalArg01.exe"		# OptionalArg01.fs - test optimizations
    [<Theory; FileInlineData("OptionalArg01.fs")>]
    let ``OptionalArg01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=TupleMonster.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TupleMonster.exe"		# TupleMonster.fs -
    [<Theory; FileInlineData("TupleMonster.fs", Realsig=BooleanOptions.Both)>]
    let ``TupleMonster_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=TupleElimination.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TupleElimination.exe"		# TupleElimination.fs -
    [<Theory; FileInlineData("TupleElimination.fs")>]
    let ``TupleElimination_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ValueTupleAliasConstructor.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ValueTupleAliasConstructor.exe"		# ValueTupleAliasConstructor.fs -
    [<Theory; FileInlineData("ValueTupleAliasConstructor.fs", Realsig=BooleanOptions.Both)>]
    let ``ValueTupleAliasConstructor_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation
