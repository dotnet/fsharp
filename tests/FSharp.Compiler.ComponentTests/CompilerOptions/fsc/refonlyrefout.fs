// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module RefOnlyRefOut =

    // Test refout/refonly alongside standalone
    [<Fact>]
    let ``fsc --refonly --standalone``() =
        FSharp """
        """
        |> asExe
        |> withOptions ["--refonly"; "--standalone"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 2030, Line 0, Col 1, Line 0, Col 1, "Invalid use of emitting a reference assembly, do not use '--standalone or --staticlink' with '--refonly or --refout'.")
        ]

    [<Fact>]
    let ``fsc --standalone --refonly ``() =
        FSharp """
        """
        |> asExe
        |> withOptions ["--standalone"; "--refonly"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 2030, Line 0, Col 1, Line 0, Col 1, "Invalid use of emitting a reference assembly, do not use '--standalone or --staticlink' with '--refonly or --refout'.")
        ]

    [<Fact>]
    let ``fsc --refout:_ --standalone``() =
        FSharp """
        """
        |> asExe
        |> withOptions ["--refout:."; "--standalone"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 2030, Line 0, Col 1, Line 0, Col 1, "Invalid use of emitting a reference assembly, do not use '--standalone or --staticlink' with '--refonly or --refout'.")
        ]

    [<Fact>]
    let ``fsc --standalone --refout:_``() =
        FSharp """
        """
        |> asExe
        |> withOptions ["--standalone"; "--refout:."]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 2030, Line 0, Col 1, Line 0, Col 1, "Invalid use of emitting a reference assembly, do not use '--standalone or --staticlink' with '--refonly or --refout'.")
        ]

    // Test refout/refonly alongside staticlink
    [<Fact>]
    let ``fsc --refonly --staticlink:_``() =
        FSharp """
        """
        |> asExe
        |> withOptions ["--refonly"; "--staticlink:."]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 2030, Line 0, Col 1, Line 0, Col 1, "Invalid use of emitting a reference assembly, do not use '--standalone or --staticlink' with '--refonly or --refout'.")
        ]

    [<Fact>]
    let ``fsc --staticlink:_ --refonly ``() =
        FSharp """
        """
        |> asExe
        |> withOptions ["--staticlink:."; "--refonly"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 2030, Line 0, Col 1, Line 0, Col 1, "Invalid use of emitting a reference assembly, do not use '--standalone or --staticlink' with '--refonly or --refout'.")
        ]

    [<Fact>]
    let ``fsc --refout:_ --staticlink:_``() =
        FSharp """
        """
        |> asExe
        |> withOptions ["--refout:."; "--staticlink:."]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 2030, Line 0, Col 1, Line 0, Col 1, "Invalid use of emitting a reference assembly, do not use '--standalone or --staticlink' with '--refonly or --refout'.")
        ]

    [<Fact>]
    let ``fsc --staticlink:_ --refout:_``() =
        FSharp """
        """
        |> asExe
        |> withOptions ["--staticlink:."; "--refout:."]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 2030, Line 0, Col 1, Line 0, Col 1, "Invalid use of emitting a reference assembly, do not use '--standalone or --staticlink' with '--refonly or --refout'.")
        ]
