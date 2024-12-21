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
    [<Theory; FileInlineData("Linq101Aggregates01.fs", Realsig=BooleanOptions.Both)>]
    let ``Linq101Aggregates01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Linq101ElementOperators01.fs   SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101ElementOperators01.exe"  # Linq101ElementOperators01.fs - CodeGen
    [<Theory; FileInlineData("Linq101ElementOperators01.fs", Realsig=BooleanOptions.Both)>]
    let ``Linq101ElementOperators01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Linq101Grouping01.fs           SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Grouping01.exe"          # Linq101Grouping01.fs - CodeGen
    [<Theory; FileInlineData("Linq101Grouping01.fs", Realsig=BooleanOptions.Both)>]
    let ``Linq101Grouping01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Linq101Joins01.fs              SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Joins01.exe"             # Linq101Joins01.fs - CodeGen
    [<Theory; FileInlineData("Linq101Joins01.fs", Realsig=BooleanOptions.Both)>]
    let ``Linq101Joins01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Linq101Ordering01.fs           SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Ordering01.exe"          # Linq101Ordering01.fs - CodeGen
    [<Theory; FileInlineData("Linq101Ordering01.fs", Realsig=BooleanOptions.Both)>]
    let ``Linq101Ordering01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Linq101Partitioning01.fs       SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Partitioning01.exe"      # Linq101Partitioning01.fs - CodeGen
    [<Theory; FileInlineData("Linq101Partitioning01.fs", Realsig=BooleanOptions.Both)>]
    let ``Linq101Partitioning01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Linq101Quantifiers01.fs        SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Quantifiers01.exe"       # Linq101Quantifiers01.fs - CodeGen
    [<Theory; FileInlineData("Linq101Quantifiers01.fs", Realsig=BooleanOptions.Both)>]
    let ``Linq101Quantifiers01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Linq101Select01.fs             SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Select01.exe"            # Linq101Select01.fs - CodeGen
    [<Theory; FileInlineData("Linq101Select01.fs", Realsig=BooleanOptions.Both)>]
    let ``Linq101Select01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Linq101SetOperators01.fs       SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101SetOperators01.exe"      # Linq101SetOperators01.fs - CodeGen
    [<Theory; FileInlineData("Linq101SetOperators01.fs", Realsig=BooleanOptions.Both)>]
    let ``Linq101SetOperators01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=Linq101Where01.fs              SCFLAGS="-r:Utils.dll -g --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Linq101Where01.exe"             # Linq101Where01.fs - CodeGen
    [<Theory; FileInlineData("Linq101Where01.fs", Realsig=BooleanOptions.Both)>]
    let ``Linq101Where01_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation
