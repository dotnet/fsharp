namespace EmittedIL.RealInternalSignature

open Xunit
open System.IO
open FSharp.Test
open FSharp.Test.Compiler

module QueryExpressionStepping =

    // SOURCE=Utils.fs                       SCFLAGS="-a -r:System.Xml.Linq" # Utils.fs
    let utilsLibrary =
        FsFromPath (Path.Combine(__SOURCE_DIRECTORY__, "Utils.fs"))
        |> withName "Utils"

    let verifyCompilation compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withNoOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> withReferences [utilsLibrary]
        |> ignoreWarnings
        |> verifyILBaseline

    // SOURCE=Linq101Aggregates01.fs         SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Aggregates01.exe"        # Linq101Aggregates01.fs - CodeGen
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"Linq101Aggregates01.fs"|])>]
    let ``Linq101Aggregates01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"Linq101Aggregates01.fs"|])>]
    let ``Linq101Aggregates01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=Linq101ElementOperators01.fs   SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101ElementOperators01.exe"  # Linq101ElementOperators01.fs - CodeGen
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"Linq101ElementOperators01.fs"|])>]
    let ``Linq101ElementOperators01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation
    // SOURCE=Linq101ElementOperators01.fs   SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101ElementOperators01.exe"  # Linq101ElementOperators01.fs - CodeGen
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"Linq101ElementOperators01.fs"|])>]
    let ``Linq101ElementOperators01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=Linq101Grouping01.fs           SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Grouping01.exe"          # Linq101Grouping01.fs - CodeGen
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"Linq101Grouping01.fs"|])>]
    let ``Linq101Grouping01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=Linq101Grouping01.fs           SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Grouping01.exe"          # Linq101Grouping01.fs - CodeGen
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"Linq101Grouping01.fs"|])>]
    let ``Linq101Grouping01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=Linq101Joins01.fs              SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Joins01.exe"             # Linq101Joins01.fs - CodeGen
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"Linq101Joins01.fs"|])>]
    let ``Linq101Joins01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=Linq101Joins01.fs              SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Joins01.exe"             # Linq101Joins01.fs - CodeGen
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"Linq101Joins01.fs"|])>]
    let ``Linq101Joins01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=Linq101Ordering01.fs           SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Ordering01.exe"          # Linq101Ordering01.fs - CodeGen
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"Linq101Ordering01.fs"|])>]
    let ``Linq101Ordering01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=Linq101Ordering01.fs           SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Ordering01.exe"          # Linq101Ordering01.fs - CodeGen
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"Linq101Ordering01.fs"|])>]
    let ``Linq101Ordering01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=Linq101Partitioning01.fs       SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Partitioning01.exe"      # Linq101Partitioning01.fs - CodeGen
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"Linq101Partitioning01.fs"|])>]
    let ``Linq101Partitioning01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=Linq101Partitioning01.fs       SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Partitioning01.exe"      # Linq101Partitioning01.fs - CodeGen
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"Linq101Partitioning01.fs"|])>]
    let ``Linq101Partitioning01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=Linq101Quantifiers01.fs        SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Quantifiers01.exe"       # Linq101Quantifiers01.fs - CodeGen
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"Linq101Quantifiers01.fs"|])>]
    let ``Linq101Quantifiers01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"Linq101Quantifiers01.fs"|])>]
    let ``Linq101Quantifiers01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

        // SOURCE=Linq101Select01.fs             SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Select01.exe"            # Linq101Select01.fs - CodeGen
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"Linq101Select01.fs"|])>]
    let ``Linq101Select01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

        // SOURCE=Linq101Select01.fs             SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Select01.exe"            # Linq101Select01.fs - CodeGen
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"Linq101Select01.fs"|])>]
    let ``Linq101Select01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=Linq101SetOperators01.fs       SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101SetOperators01.exe"      # Linq101SetOperators01.fs - CodeGen
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"Linq101SetOperators01.fs"|])>]
    let ``Linq101SetOperators01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=Linq101SetOperators01.fs       SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101SetOperators01.exe"      # Linq101SetOperators01.fs - CodeGen
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"Linq101SetOperators01.fs"|])>]
    let ``Linq101SetOperators01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=Linq101Where01.fs              SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Where01.exe"             # Linq101Where01.fs - CodeGen
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"Linq101Where01.fs"|])>]
    let ``Linq101Where01_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=Linq101Where01.fs              SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Where01.exe"             # Linq101Where01.fs - CodeGen
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"Linq101Where01.fs"|])>]
    let ``Linq101Where01_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation
