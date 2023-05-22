// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CompilerOptions

open Xunit
open FSharp.Test.Compiler


//# Sanity check - simply check that the option is valid
module flaterrors =

    //# Functional: the option does what it is meant to do
    //ReqENU	SOURCE=E_MultiLine01.fs								# E_MultiLine01.fs
    [<Fact>]
    let ``E_MultiLine01_fs`` () =
        Fs """List.rev {1..10}"""
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 1, Col 11, Line 1, Col 16, "This expression was expected to have type\n    ''a list'    \nbut here has type\n    'seq<'b>'    ")
            (Error 1, Line 1, Col 11, Line 1, Col 16, "This expression was expected to have type\n    ''a list'    \nbut here has type\n    'seq<int>'    ")
            (Warning 20, Line 1, Col 1, Line 1, Col 17, "The result of this expression has type ''a list' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")
        ]

    // SOURCE=E_MultiLine02.fs SCFLAGS="--flaterrors"					# E_MultiLine02.fs
    [<Fact>]
    let ``E_MultiLine02_fs`` () =
        Fs """List.rev {1..10} |> ignore"""
        |> asExe
        |> withOptions ["--flaterrors"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 1, Col 11, Line 1, Col 16, "This expression was expected to have type\029    ''a list'    \029but here has type\029    'seq<'b>'    ")
            (Error 1, Line 1, Col 11, Line 1, Col 16, "This expression was expected to have type\029    ''a list'    \029but here has type\029    'seq<int>'    ")
        ]

    // SOURCE=E_MultiLine03.fs SCFLAGS="--flaterrors"					# E_MultiLine03.fs
    [<Fact>]
    let ``E_MultiLine03_fs`` () =
        Fs """let a = b"""
        |> withOptions ["--flaterrors"]
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 1, Col 9, Line 1, Col 10, """The value or constructor 'b' is not defined.""")
        ]


//# Functional: the option does what it is meant to do
//ReqENU	SOURCE=E_MultiLine01.fs								# E_MultiLine01.fs
//	SOURCE=E_MultiLine02.fs SCFLAGS="--flaterrors"					# E_MultiLine02.fs
//	SOURCE=E_MultiLine03.fs SCFLAGS="--flaterrors"					# E_MultiLine03.fs

//# In combination with --nologo, --out
//	SOURCE=E_MultiLine02.fs SCFLAGS="--flaterrors --nologo"				# Combined01
//	SOURCE=E_MultiLine03.fs SCFLAGS="--out:E_MultiLine03.exe --flaterrors"		# Combined02

//# Last one wins... (multiple-usage)
//	SOURCE=E_MultiLine02.fs COMPILE_ONLY=1 SCFLAGS="--flaterrors --flaterrors"	# MultipleUse

//# Option is case sentitive
//	SOURCE=E_MultiLine04.fs COMPILE_ONLY=1 SCFLAGS="--FlatErrors"			# CaseSensitive01
//	SOURCE=E_MultiLine04.fs COMPILE_ONLY=1 SCFLAGS="--FLATERRORS"			# CaseSensitive02

//# Mispelled options
//	SOURCE=E_MultiLine04.fs COMPILE_ONLY=1 SCFLAGS="-flaterrors"			# Mispelled01
//	SOURCE=E_MultiLine04.fs COMPILE_ONLY=1 SCFLAGS="--flaterrors+"			# Mispelled02
