namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open System.IO
open FSharp.Test
open FSharp.Test.Compiler

module Misc =

    let verifyCompilation compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withNoOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> withCulture "en-US"
        |> verifyILBaseline

    // SOURCE=AnonRecd.fs                  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd AnonRecd.exe"	# AnonRecd.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AnonRecd.fs"|])>]
    let ``AnonRecd_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=CodeGenRenamings01.fs        SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd CodeGenRenamings01.exe"	# CodeGenRenamings01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"CodeGenRenamings01.fs"|])>]
    let ``CodeGenRenamings01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=ArgumentNamesInClosures01.fs SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ArgumentNamesInClosures01.dll"	# ArgumentNamesInClosures01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ArgumentNamesInClosures01.fs"|])>]
    let ``ArgumentNamesInClosures01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=Decimal01.fs                 SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Decimal01.exe"			# Decimal01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Decimal01.fs"|])>]
    let ``Decimal01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=EntryPoint01.fs              SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd EntryPoint01.exe"		# EntryPoint01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"EntryPoint01.fs"|])>]
    let ``EntryPoint01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=EqualsOnUnions01.fs          SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd EqualsOnUnions01.exe"		# EqualsOnUnions01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"EqualsOnUnions01.fs"|])>]
    let ``EqualsOnUnions01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=ForLoop01.fs                 SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ForLoop01.exe"	# ForLoop01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ForLoop01.fs"|])>]
    let ``ForLoop01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=ForLoop02.fs                 SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ForLoop02.exe"	# ForLoop02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ForLoop02.fs"|])>]
    let ``ForLoop02_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=ForLoop03.fs                 SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ForLoop03.exe"	# ForLoop03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ForLoop02.fs"|])>]
    let ``ForLoop03_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=NoBoxingOnDispose01.fs       SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd NoBoxingOnDispose01.exe"		# NoBoxingOnDispose01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NoBoxingOnDispose01.fs"|])>]
    let ``NoBoxingOnDispose01_fs`` compilation =
        compilation
        |> verifyCompilation

    //SOURCE=IfThenElse01.fs              SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd IfThenElse01.dll"		# IfThenElse01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"IfThenElse01.fs"|])>]
    let ``IfThenElse01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=LetIfThenElse01.fs           SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd LetIfThenElse01.exe"			# LetIfThenElse01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"LetIfThenElse01.fs"|])>]
    let ``LetIfThenElse01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=Lock01.fs                    SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Lock01.exe"	# Lock01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Lock01.fs"|])>]
    let ``Lock01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=ModuleWithExpression01.fs    SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ModuleWithExpression01.exe"	# ModuleWithExpression01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ModuleWithExpression01.fs"|])>]
    let ``ModuleWithExpression01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=NonEscapingArguments02.fs    SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd NonEscapingArguments02.dll"	# NonEscapingArguments02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NonEscapingArguments02.fs"|])>]
    let ``NonEscapingArguments02_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=Seq_for_all01.fs             SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Seq_for_all01.exe"		# Seq_for_all01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Seq_for_all01.fs"|])>]
    let ``Seq_for_all01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=StructsAsArrayElements01.fs  SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd StructsAsArrayElements01.dll"		# StructsAsArrayElements01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"StructsAsArrayElements01.fs"|])>]
    let ``StructsAsArrayElements01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=PreserveSig.fs               SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd PreserveSig.dll"		# PreserveSig.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PreserveSig.fs"|])>]
    let ``PreserveSig_fs`` compilation =
        compilation
        |> verifyCompilation

    // # The name of this test is a bit misleading for legacy reasons: it used to test the --no-generate-filter-blocks option, which is now gone
    // SOURCE=TryWith_NoFilterBlocks01.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TryWith_NoFilterBlocks01.exe"						# TryWith_NoFilterBlocks01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TryWith_NoFilterBlocks01.fs"|])>]
    let ``TryWith_NoFilterBlocks01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=Structs01.fs                 SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Structs01.exe"		# Structs01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Structs01.fs"|])>]
    let ``Structs01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=Structs02.fs                 SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Structs02.exe"		# Structs02.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Structs02.fs"|])>]
    let ``Structs02_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=Marshal.fs                   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Marshal.exe"  # Marshal.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Marshal.fs"|])>]
    let ``Marshal_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=MethodImplNoInline.fs        SCFLAGS="-O" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplNoInline.exe"						# MethodImplNoInline.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ForLoop02.fs"|])>]
    let ``MethodImplNoInline_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=MethodImplNoInline02.fs      SCFLAGS="-O" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplNoInline02.exe"						# MethodImplNoInline02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"MethodImplNoInline02.fs"|])>]
    let ``MethodImplNoInline02_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=CustomAttributeGenericParameter01.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd CustomAttributeGenericParameter01.exe"	# CustomAttributeGenericParameter01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"CustomAttributeGenericParameter01.fs"|])>]
    let ``CustomAttributeGenericParameter01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=GenericTypeStaticField.fs  SCFLAGS="-g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd GenericTypeStaticField.exe"	# GenericTypeStaticField.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"GenericTypeStaticField.fs"|])>]
    let ``GenericTypeStaticField_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=GeneralizationOnUnions01.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd GeneralizationOnUnions01.exe"	# GeneralizationOnUnions01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"GeneralizationOnUnions01.fs"|])>]
    let ``GeneralizationOnUnions01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=AbstractClass.fs             SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd AbstractClass.exe"	# AbstractClass.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AbstractClass.fs"|])>]
    let ``AbstractClass_fs`` compilation =
        compilation
        |> verifyCompilation
