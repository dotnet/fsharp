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
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"AbstractClass.fs"|])>]
    let ``AbstractClass_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> asExe
        |> verifyCompilation

    // SOURCE=AbstractClass.fs             SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd AbstractClass.exe"	# AbstractClass.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"AbstractClass.fs"|])>]
    let ``AbstractClass_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> asExe
        |> verifyCompilation

    // SOURCE=AnonRecd.fs                  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd AnonRecd.exe"	# AnonRecd.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AnonRecd.fs"|])>]
    let ``AnonRecd_fs`` compilation =
        compilation
        |> asExe
        |> verifyCompilation

    // SOURCE=CodeGenRenamings01.fs        SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd CodeGenRenamings01.exe"	# CodeGenRenamings01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__,  BaselineSuffix=".RealInternalSignatureOn",Includes=[|"CodeGenRenamings01.fs"|])>]
    let ``CodeGenRenamings01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> withLangVersionPreview // TODO https://github.com/dotnet/fsharp/issues/16739: Remove this when LanguageFeature.LowerIntegralRangesToFastLoops is out of preview.
        |> asExe
        |> verifyCompilation

    // SOURCE=CodeGenRenamings01.fs        SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd CodeGenRenamings01.exe"	# CodeGenRenamings01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"CodeGenRenamings01.fs"|])>]
    let ``CodeGenRenamings01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> asExe
        |> verifyCompilation

    // SOURCE=ArgumentNamesInClosures01.fs SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ArgumentNamesInClosures01.dll"	# ArgumentNamesInClosures01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"ArgumentNamesInClosures01.fs"|])>]
    let ``ArgumentNamesInClosures01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> asExe
        |> verifyCompilation

    // SOURCE=ArgumentNamesInClosures01.fs SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ArgumentNamesInClosures01.dll"	# ArgumentNamesInClosures01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"ArgumentNamesInClosures01.fs"|])>]
    let ``ArgumentNamesInClosures01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> asExe
        |> verifyCompilation

    // SOURCE=Decimal01.fs                 SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Decimal01.exe"			# Decimal01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"Decimal01.fs"|])>]
    let ``Decimal01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> asExe
        |> verifyCompilation

    // SOURCE=Decimal01.fs                 SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Decimal01.exe"			# Decimal01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"Decimal01.fs"|])>]
    let ``Decimal01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> asExe
        |> verifyCompilation

    // SOURCE=EntryPoint01.fs              SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd EntryPoint01.exe"		# EntryPoint01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"EntryPoint01.fs"|])>]
    let ``EntryPoint01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> asExe
        |> verifyCompilation

    // SOURCE=EntryPoint01.fs              SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd EntryPoint01.exe"		# EntryPoint01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"EntryPoint01.fs"|])>]
    let ``EntryPoint01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> asExe
        |> verifyCompilation

    // SOURCE=EqualsOnUnions01.fs          SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd EqualsOnUnions01.exe"		# EqualsOnUnions01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"EqualsOnUnions01.fs"|])>]
    let ``EqualsOnUnions01_fs`` compilation =
        compilation
        |> asExe
        |> verifyCompilation

    // SOURCE=ForLoop01.fs                 SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ForLoop01.exe"	# ForLoop01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"ForLoop01.fs"|])>]
    let ``ForLoop01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> withLangVersionPreview // TODO https://github.com/dotnet/fsharp/issues/16739: Remove this when LanguageFeature.LowerIntegralRangesToFastLoops is out of preview.
        |> asExe
        |> verifyCompilation

    // SOURCE=ForLoop01.fs                 SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ForLoop01.exe"	# ForLoop01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"ForLoop01.fs"|])>]
    let ``ForLoop01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> asExe
        |> verifyCompilation

    // SOURCE=ForLoop02.fs                 SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ForLoop02.exe"	# ForLoop02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"ForLoop02.fs"|])>]
    let ``ForLoop02_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> asExe
        |> verifyCompilation

    // SOURCE=ForLoop02.fs                 SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ForLoop02.exe"	# ForLoop02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"ForLoop02.fs"|])>]
    let ``ForLoop02_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> asExe
        |> verifyCompilation

    // SOURCE=ForLoop03.fs                 SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ForLoop03.exe"	# ForLoop03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"ForLoop03.fs"|])>]
    let ``ForLoop03_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> asExe
        |> verifyCompilation

    // SOURCE=ForLoop03.fs                 SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ForLoop03.exe"	# ForLoop03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"ForLoop03.fs"|])>]
    let ``ForLoop03_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> asExe
        |> verifyCompilation

    // SOURCE=NoBoxingOnDispose01.fs       SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd NoBoxingOnDispose01.exe"		# NoBoxingOnDispose01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NoBoxingOnDispose01.fs"|])>]
    let ``NoBoxingOnDispose01_fs`` compilation =
        compilation
        |> asExe
        |> verifyCompilation

    //SOURCE=IfThenElse01.fs              SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd IfThenElse01.dll"		# IfThenElse01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"IfThenElse01.fs"|])>]
    let ``IfThenElse01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> asExe
        |> verifyCompilation

    //SOURCE=IfThenElse01.fs              SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd IfThenElse01.dll"		# IfThenElse01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"IfThenElse01.fs"|])>]
    let ``IfThenElse01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> asExe
        |> verifyCompilation

    // SOURCE=LetIfThenElse01.fs           SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd LetIfThenElse01.exe"			# LetIfThenElse01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"LetIfThenElse01.fs"|])>]
    let ``LetIfThenElse01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> asExe
        |> verifyCompilation

    // SOURCE=LetIfThenElse01.fs           SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd LetIfThenElse01.exe"			# LetIfThenElse01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"LetIfThenElse01.fs"|])>]
    let ``LetIfThenElse01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> asExe
        |> verifyCompilation

    // SOURCE=Lock01.fs                    SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Lock01.exe"	# Lock01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"Lock01.fs"|])>]
    let ``Lock01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> asExe
        |> verifyCompilation

    // SOURCE=Lock01.fs                    SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Lock01.exe"	# Lock01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"Lock01.fs"|])>]
    let ``Lock01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> asExe
        |> verifyCompilation

    // SOURCE=ModuleWithExpression01.fs    SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ModuleWithExpression01.exe"	# ModuleWithExpression01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"ModuleWithExpression01.fs"|])>]
    let ``ModuleWithExpression01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> asExe
        |> verifyCompilation

    // SOURCE=ModuleWithExpression01.fs    SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ModuleWithExpression01.exe"	# ModuleWithExpression01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"ModuleWithExpression01.fs"|])>]
    let ``ModuleWithExpression01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> asExe
        |> verifyCompilation

    // SOURCE=NonEscapingArguments02.fs    SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd NonEscapingArguments02.dll"	# NonEscapingArguments02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NonEscapingArguments02.fs"|])>]
    let ``NonEscapingArguments02_fs`` compilation =
        compilation
        |> asExe
        |> verifyCompilation

    // SOURCE=Seq_for_all01.fs             SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Seq_for_all01.exe"		# Seq_for_all01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"Seq_for_all01.fs"|])>]
    let ``Seq_for_all01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> asExe
        |> verifyCompilation

    // SOURCE=Seq_for_all01.fs             SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Seq_for_all01.exe"		# Seq_for_all01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"Seq_for_all01.fs"|])>]
    let ``Seq_for_all01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> asExe
        |> verifyCompilation

    // SOURCE=StructsAsArrayElements01.fs  SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd StructsAsArrayElements01.dll"		# StructsAsArrayElements01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"StructsAsArrayElements01.fs"|])>]
    let ``StructsAsArrayElements01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> asExe
        |> verifyCompilation

    // SOURCE=StructsAsArrayElements01.fs  SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd StructsAsArrayElements01.dll"		# StructsAsArrayElements01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"StructsAsArrayElements01.fs"|])>]
    let ``StructsAsArrayElements01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> asExe
        |> verifyCompilation

    // SOURCE=PreserveSig.fs               SCFLAGS="-a -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd PreserveSig.dll"		# PreserveSig.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"PreserveSig.fs"|])>]
    let ``PreserveSig_fs`` compilation =
        compilation
        |> asExe
        |> verifyCompilation

    // # The name of this test is a bit misleading for legacy reasons: it used to test the --no-generate-filter-blocks option, which is now gone
    // SOURCE=TryWith_NoFilterBlocks01.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TryWith_NoFilterBlocks01.exe"						# TryWith_NoFilterBlocks01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"TryWith_NoFilterBlocks01.fs"|])>]
    let ``TryWith_NoFilterBlocks01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> asExe
        |> verifyCompilation

    // # The name of this test is a bit misleading for legacy reasons: it used to test the --no-generate-filter-blocks option, which is now gone
    // SOURCE=TryWith_NoFilterBlocks01.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd TryWith_NoFilterBlocks01.exe"						# TryWith_NoFilterBlocks01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"TryWith_NoFilterBlocks01.fs"|])>]
    let ``TryWith_NoFilterBlocks01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> asExe
        |> verifyCompilation

    // SOURCE=Structs01.fs                 SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Structs01.exe"		# Structs01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Structs01.fs"|])>]
    let ``Structs01_fs`` compilation =
        compilation
        |> asExe
        |> verifyCompilation

    // SOURCE=Structs02.fs                 SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Structs02.exe"		# Structs02.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Structs02.fs"|])>]
    let ``Structs02_fs`` compilation =
        compilation
        |> asExe
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Structs02_asNetStandard20.fs"|])>]
    let ``Structs02_asNetStandard20_fs`` compilation =
        compilation
        |>asLibrary
        |>asNetStandard20
        |>verifyCompilation

    // SOURCE=Marshal.fs                   SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Marshal.exe"  # Marshal.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Marshal.fs"|])>]
    let ``Marshal_fs`` compilation =
        compilation
        |> asExe
        |> verifyCompilation

    // SOURCE=MethodImplNoInline02.fs      SCFLAGS="-O" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplNoInline02.exe"						# MethodImplNoInline02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"MethodImplNoInline02.fs"|])>]
    let ``MethodImplNoInline02_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> asExe
        |> verifyCompilation

    // SOURCE=MethodImplNoInline02.fs      SCFLAGS="-O" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplNoInline02.exe"						# MethodImplNoInline02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"MethodImplNoInline02.fs"|])>]
    let ``MethodImplNoInline02_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> asExe
        |> verifyCompilation

    // SOURCE=CustomAttributeGenericParameter01.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd CustomAttributeGenericParameter01.exe"	# CustomAttributeGenericParameter01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"CustomAttributeGenericParameter01.fs"|])>]
    let ``CustomAttributeGenericParameter01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> asExe
        |> verifyCompilation

    // SOURCE=CustomAttributeGenericParameter01.fs SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd CustomAttributeGenericParameter01.exe"	# CustomAttributeGenericParameter01.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"CustomAttributeGenericParameter01.fs"|])>]
    let ``CustomAttributeGenericParameter01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> asExe
        |> verifyCompilation

    // SOURCE=GenericTypeStaticField.fs  SCFLAGS="-g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd GenericTypeStaticField.exe"	# GenericTypeStaticField.fs -
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"GenericTypeStaticField.fs"|])>]
    let ``GenericTypeStaticField_fs`` compilation =
        compilation
        |> asExe
        |> verifyCompilation

    // SOURCE=GenericTypeStaticField.fs  SCFLAGS="-g --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd GenericTypeStaticField.exe"	# GenericTypeStaticField.fs -
    // SOURCE=GeneralizationOnUnions01.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd GeneralizationOnUnions01.exe"	# GeneralizationOnUnions01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"GeneralizationOnUnions01.fs"|])>]
    let ``GeneralizationOnUnions01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> asExe
        |> verifyCompilation

    // SOURCE=GeneralizationOnUnions01.fs  SCFLAGS="-g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd GeneralizationOnUnions01.exe"	# GeneralizationOnUnions01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"GeneralizationOnUnions01.fs"|])>]
    let ``GeneralizationOnUnions01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> asExe
        |> verifyCompilation
