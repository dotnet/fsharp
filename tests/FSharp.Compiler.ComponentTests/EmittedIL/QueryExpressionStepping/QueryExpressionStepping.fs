namespace FSharp.Compiler.ComponentTests.EmittedIL

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
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Linq101Aggregates01.fs"|])>]
    let ``Linq101Aggregates01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=Linq101ElementOperators01.fs   SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101ElementOperators01.exe"  # Linq101ElementOperators01.fs - CodeGen
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Linq101ElementOperators01.fs"|])>]
    let ``Linq101ElementOperators01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=Linq101Grouping01.fs           SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Grouping01.exe"          # Linq101Grouping01.fs - CodeGen
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Linq101Grouping01.fs"|])>]
    let ``Linq101Grouping01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=Linq101Joins01.fs              SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Joins01.exe"             # Linq101Joins01.fs - CodeGen
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Linq101Joins01.fs"|])>]
    let ``Linq101Joins01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=Linq101Ordering01.fs           SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Ordering01.exe"          # Linq101Ordering01.fs - CodeGen
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Linq101Ordering01.fs"|])>]
    let ``Linq101Ordering01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=Linq101Partitioning01.fs       SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Partitioning01.exe"      # Linq101Partitioning01.fs - CodeGen
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Linq101Partitioning01.fs"|])>]
    let ``Linq101Partitioning01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=Linq101Quantifiers01.fs        SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Quantifiers01.exe"       # Linq101Quantifiers01.fs - CodeGen
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Linq101Quantifiers01.fs"|])>]
    let ``Linq101Quantifiers01_fs`` compilation =
        compilation
        |> verifyCompilation

        // SOURCE=Linq101Select01.fs             SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Select01.exe"            # Linq101Select01.fs - CodeGen
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Linq101Select01.fs"|])>]
    let ``Linq101Select01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=Linq101SetOperators01.fs       SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101SetOperators01.exe"      # Linq101SetOperators01.fs - CodeGen
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Linq101SetOperators01.fs"|])>]
    let ``Linq101SetOperators01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=Linq101Where01.fs              SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Where01.exe"             # Linq101Where01.fs - CodeGen
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Linq101Where01.fs"|])>]
    let ``Linq101Where01_fs`` compilation =
        compilation
        |> verifyCompilation
