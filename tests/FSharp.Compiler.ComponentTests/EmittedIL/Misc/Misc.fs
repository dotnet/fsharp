namespace EmittedIL.RealInternalSignature

open Xunit
open System.IO
open FSharp.Test
open FSharp.Test.Compiler

module Misc =

    let verifyCompilation compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> withNoOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyILBaseline

    // SOURCE=AbstractClass.fs             SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd AbstractClass.exe"	# AbstractClass.fs
    [<Theory; FileInlineData("AbstractClass.fs", Realsig=BooleanOptions.Both)>]
    let ``AbstractClass_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> verifyCompilation

    // SOURCE=AnonRecd.fs                  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd AnonRecd.exe"	# AnonRecd.fs
    [<Theory; FileInlineData("AnonRecd.fs")>]
    let ``AnonRecd_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> verifyCompilation

    // SOURCE=CodeGenRenamings01.fs        SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd CodeGenRenamings01.exe"	# CodeGenRenamings01.fs -
    [<Theory; FileInlineData("CodeGenRenamings01.fs", Realsig=BooleanOptions.Both)>]
    let ``CodeGenRenamings01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> verifyCompilation

    // SOURCE=ArgumentNamesInClosures01.fs SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ArgumentNamesInClosures01.dll"	# ArgumentNamesInClosures01.fs -
    [<Theory; FileInlineData("ArgumentNamesInClosures01.fs", Realsig=BooleanOptions.Both)>]
    let ``ArgumentNamesInClosures01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> verifyCompilation

    // SOURCE=Decimal01.fs                 SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Decimal01.exe"			# Decimal01.fs
    [<Theory; FileInlineData("Decimal01.fs", Realsig=BooleanOptions.Both)>]
    let ``Decimal01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> verifyCompilation

    // SOURCE=EntryPoint01.fs              SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd EntryPoint01.exe"		# EntryPoint01.fs
    [<Theory; FileInlineData("EntryPoint01.fs", Realsig=BooleanOptions.Both)>]
    let ``EntryPoint01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> verifyCompilation

    // SOURCE=EqualsOnUnions01.fs          SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd EqualsOnUnions01.exe"		# EqualsOnUnions01.fs -
    [<Theory; FileInlineData("EqualsOnUnions01.fs")>]
    let ``EqualsOnUnions01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> verifyCompilation

    // SOURCE=NoBoxingOnDispose01.fs       SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd NoBoxingOnDispose01.exe"		# NoBoxingOnDispose01.fs
    [<Theory; FileInlineData("NoBoxingOnDispose01.fs")>]
    let ``NoBoxingOnDispose01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> verifyCompilation

    //SOURCE=IfThenElse01.fs              SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd IfThenElse01.dll"		# IfThenElse01.fs
    [<Theory; FileInlineData("IfThenElse01.fs", Realsig=BooleanOptions.Both)>]
    let ``IfThenElse01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> verifyCompilation

    // SOURCE=LetIfThenElse01.fs           SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd LetIfThenElse01.exe"			# LetIfThenElse01.fs -
    [<Theory; FileInlineData("LetIfThenElse01.fs", Realsig=BooleanOptions.Both)>]
    let ``LetIfThenElse01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> verifyCompilation

    // SOURCE=Lock01.fs                    SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Lock01.exe"	# Lock01.fs -
    [<Theory; FileInlineData("Lock01.fs", Realsig=BooleanOptions.Both)>]
    let ``Lock01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> verifyCompilation

    // SOURCE=ModuleWithExpression01.fs    SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ModuleWithExpression01.exe"	# ModuleWithExpression01.fs
    [<Theory; FileInlineData("ModuleWithExpression01.fs", Realsig=BooleanOptions.Both)>]
    let ``ModuleWithExpression01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> verifyCompilation

    // SOURCE=NonEscapingArguments02.fs    SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd NonEscapingArguments02.dll"	# NonEscapingArguments02.fs
    [<Theory; FileInlineData("NonEscapingArguments02.fs")>]
    let ``NonEscapingArguments02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> verifyCompilation

    // SOURCE=Seq_for_all01.fs             SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Seq_for_all01.exe"		# Seq_for_all01.fs
    [<Theory; FileInlineData("Seq_for_all01.fs", Realsig=BooleanOptions.Both)>]
    let ``Seq_for_all01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> verifyCompilation

    // SOURCE=StructsAsArrayElements01.fs  SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd StructsAsArrayElements01.dll"		# StructsAsArrayElements01.fs -
    [<Theory; FileInlineData("StructsAsArrayElements01.fs", Realsig=BooleanOptions.Both)>]
    let ``StructsAsArrayElements01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> verifyCompilation

    // SOURCE=PreserveSig.fs               SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd PreserveSig.dll"		# PreserveSig.fs -
    [<Theory; FileInlineData("PreserveSig.fs")>]
    let ``PreserveSig_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> verifyCompilation

    // # The name of this test is a bit misleading for legacy reasons: it used to test the --no-generate-filter-blocks option, which is now gone
    // SOURCE=TryWith_NoFilterBlocks01.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TryWith_NoFilterBlocks01.exe"						# TryWith_NoFilterBlocks01.fs
    [<Theory; FileInlineData("TryWith_NoFilterBlocks01.fs", Realsig=BooleanOptions.Both)>]
    let ``TryWith_NoFilterBlocks01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> verifyCompilation

    // SOURCE=Structs01.fs                 SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Structs01.exe"		# Structs01.fs -
    [<Theory; FileInlineData("Structs01.fs")>]
    let ``Structs01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> verifyCompilation

    // SOURCE=Structs02.fs                 SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Structs02.exe"		# Structs02.fs -
    [<Theory; FileInlineData("Structs02.fs")>]
    let ``Structs02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> verifyCompilation

    [<Theory; FileInlineData("Structs02_asNetStandard20.fs")>]
    let ``Structs02_asNetStandard20_fs`` compilation =
        compilation
        |> getCompilation
        |>asLibrary
        |>asNetStandard20
        |>verifyCompilation

    // SOURCE=Marshal.fs                   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Marshal.exe"  # Marshal.fs
    [<Theory; FileInlineData("Marshal.fs")>]
    let ``Marshal_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> verifyCompilation

    // SOURCE=MethodImplNoInline02.fs      SCFLAGS="-O" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplNoInline02.exe"						# MethodImplNoInline02.fs
    [<Theory; FileInlineData("MethodImplNoInline02.fs", Realsig=BooleanOptions.Both)>]
    let ``MethodImplNoInline02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> verifyCompilation

    // SOURCE=CustomAttributeGenericParameter01.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd CustomAttributeGenericParameter01.exe"	# CustomAttributeGenericParameter01.fs -
    [<Theory; FileInlineData("CustomAttributeGenericParameter01.fs", Realsig=BooleanOptions.Both)>]
    let ``CustomAttributeGenericParameter01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> verifyCompilation

    // SOURCE=GenericTypeStaticField.fs  SCFLAGS="-g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd GenericTypeStaticField.exe"	# GenericTypeStaticField.fs -
    [<Theory; FileInlineData("GenericTypeStaticField.fs")>]
    let ``GenericTypeStaticField_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> verifyCompilation

    // SOURCE=GenericTypeStaticField.fs  SCFLAGS="-g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd GenericTypeStaticField.exe"	# GenericTypeStaticField.fs -
    // SOURCE=GeneralizationOnUnions01.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd GeneralizationOnUnions01.exe"	# GeneralizationOnUnions01.fs
    [<Theory; FileInlineData("GeneralizationOnUnions01.fs", Realsig=BooleanOptions.Both)>]
    let ``GeneralizationOnUnions01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> verifyCompilation
