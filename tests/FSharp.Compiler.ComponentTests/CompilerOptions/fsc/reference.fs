// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CompilerOptions.fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module reference =

    let source =
        FSharp """
            namespace Hello.World
        """

    //SOURCE=EscapeChars01.fs SCFLAGS="-r:a\\b\\n.dll" 			# EscapeChars01.fs (-r:)
    [<Fact>]
    let ``EscapeChars01`` () =
        source
        |> asFs
        |> withOptions ["-r:a\\b\\n.dll"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 84, Line 1, Col 1, Line 1, Col 1, """Assembly reference 'a\b\n.dll' was not found or is invalid""")
        ]

    //SOURCE=EscapeChars02.fs SCFLAGS="-r:.\\r.dll -r:..\\n\\r\\a.dll" 	# EscapeChars02.fs (-r: )
    [<Fact>]
    let ``EscapeChars02a`` () =
        source
        |> asFs
        |> withOptions ["-r:.\\r.dll -r:..\\n\\r\\a.dll"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 84, Line 1, Col 1, Line 1, Col 1, """Assembly reference '.\r.dll -r:..\n\r\a.dll' was not found or is invalid""")
        ]

    //SOURCE=EscapeChars02.fs SCFLAGS="-r .\\r.dll -r:..\\n\\r\\a.dll" 	# EscapeChars02.fs (-r )
    [<Fact>]
    let ``EscapeChars02b`` () =
        source
        |> asFs
        |> withOptions ["-r:.\\r.dll -r:..\\n\\r\\a.dll"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 84, Line 1, Col 1, Line 1, Col 1, """Assembly reference '.\r.dll -r:..\n\r\a.dll' was not found or is invalid""")
        ]

    //SOURCE=MissingDLL.fs SCFLAGS="-r:MissingDLL.dll" 	# MissingDLL.fs
    [<Fact>]
    let ``MissingDLL_fs_a`` () =
        source
        |> asFs
        |> withOptions ["-r:MissingDLL.dll"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 84, Line 1, Col 1, Line 1, Col 1, """Assembly reference 'MissingDLL.dll' was not found or is invalid""")
        ]

    //SOURCE=MissingEXE.fs SCFLAGS="-r:MissingEXE.exe" 	# MissingEXE.fs
    [<Fact>]
    let ``MissingEXE_fs_a`` () =
        source
        |> asFs
        |> withOptions ["-r:MissingEXE.exe"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 84, Line 1, Col 1, Line 1, Col 1, """Assembly reference 'MissingEXE.exe' was not found or is invalid""")
        ]

    //SOURCE=MissingDLL.fsx SCFLAGS="-r:MissingDLL.dll"  FSIMODE=EXEC	# MissingDLL.fsx (-r:)
    [<Fact>]
    let ``MissingDLL_fsx_a`` () =
        source
        |> asFsx
        |> withOptions ["-r:MissingDLL.dll"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 84, Line 1, Col 1, Line 1, Col 1, """Assembly reference 'MissingDLL.dll' was not found or is invalid""")
        ]

    //SOURCE=MissingDLL.fsx SCFLAGS="-r MissingDLL.dll"  FSIMODE=EXEC	# MissingDLL.fsx (-r )
    [<Fact>]
    let ``MissingDLL_fsx_b`` () =
        source
        |> asFsx
        |> withOptions ["-r:MissingDLL.dll"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 84, Line 1, Col 1, Line 1, Col 1, """Assembly reference 'MissingDLL.dll' was not found or is invalid""")
        ]

    //SOURCE=MissingEXE.fsx SCFLAGS="-r:MissingEXE.exe"  FSIMODE=EXEC	# MissingEXE.fsx (-r:)
    [<Fact>]
    let ``MissingEXE_fsx_a`` () =
        source
        |> asFsx
        |> withOptions ["-r:MissingEXE.exe"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 84, Line 1, Col 1, Line 1, Col 1, """Assembly reference 'MissingEXE.exe' was not found or is invalid""")
        ]

    //SOURCE=MissingEXE.fsx SCFLAGS="-r MissingEXE.exe"  FSIMODE=EXEC	# MissingEXE.fsx (-r )
    [<Fact>]
    let ``MissingEXE_fsx_b`` () =
        source
        |> asFsx
        |> withOptions ["-r:MissingEXE.exe"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 84, Line 1, Col 1, Line 1, Col 1, """Assembly reference 'MissingEXE.exe' was not found or is invalid""")
        ]


