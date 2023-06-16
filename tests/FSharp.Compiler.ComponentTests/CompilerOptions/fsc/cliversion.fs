// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module cliversion =

    //#   SOURCE=E_fsc_cliversion.fs  SCFLAGS="--cliversion:2.0"                              # fsc --cliversion:2.0
    [<Fact>]
    let ``fsc --cliversion:2_0``() =
        FSharp """
        """
        |> asExe
        |> withOptions ["--cliversion:2.0"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 243, Line 0, Col 1, Line 0, Col 1, "Unrecognized option: '--cliversion'. Use '--help' to learn about recognized command line options.")
        ]

    //#   SOURCE=E_fsi_cliversion.fs  SCFLAGS="--cliversion:2.0" FSIMODE=EXEC COMPILE_ONLY=1  # fsi --cliversion:2.0
    [<Fact>]
    let ``fsi --cliversion:2_0``() =
        FSharp """
        """
        |> asFsx
        |> withOptions ["--cliversion:2.0"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 243, Line 0, Col 1, Line 0, Col 1, "Unrecognized option: '--cliversion'. Use '--help' to learn about recognized command line options.")
        ]

