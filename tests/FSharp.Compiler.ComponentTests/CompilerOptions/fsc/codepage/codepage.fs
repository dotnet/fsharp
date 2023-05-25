// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CompilerOptions.fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open System
open System.IO

module codepage =

    let libCodepage1250 =
        FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  "libCodepage.fs"))
        |> withCodepage "1250"
        |> withName "libCodepage1250"
        |> asLibrary

    let libCodepage1251 =
        FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  "libCodepage.fs"))
        |> withCodepage "1251"
        |> withName "libCodepage1251"
        |> asLibrary

    let secondLibCodepage1250 =
        FsFromPath (Path.Combine(__SOURCE_DIRECTORY__,  "libCodepage.fs"))
        |> withCodepage "1250"
        |> withName "secondLibCodepage1250"
        |> asLibrary


#if NETFRAMEWORK

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ReferenceBoth.fs"|])>]
    let ``Reference assembly compiled with same codepages`` compilation =
        compilation
        |> asExe
        |> withReferences [ libCodepage1250; secondLibCodepage1250 ]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 11, Col 13, Line 11, Col 14, "The value, constructor, namespace or type 'б' is not defined.")
        ]

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ReferenceBoth.fs"|])>]
    let ``Reference assembly compiled with different codepages`` compilation =
        compilation
        |> asExe
        |> withReferences [ libCodepage1250; libCodepage1251 ]
        |> compileAndRun
        |> shouldSucceed
#endif

    //# Boundary case
    //	SOURCE=E_NoDataForEncoding65535.fs SCFLAGS="--codepage:65535"		# E_NoDataForEncoding65535.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_InvalidArgument.fs"|])>]
    let ``Reference assembly compiled with no data for codepage`` compilation =
        compilation
        |> withCodepage "65535"
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 193, Line 1, Col 1, Line 1, Col 1, "No data is available for encoding 65535. For information on defining a custom encoding, see the documentation for the Encoding.RegisterProvider method.")
        ]

    //# Boundary case
    //	SOURCE=Zero.fs                     SCFLAGS="--codepage:0x0"		# Zero.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Zero.fs"|])>]
    let ``Reference assembly compiled with zero for codepage`` compilation =
        compilation
        |> withNoWarn 52
        |> withCodepage "0x0"
        |> asExe
        |> compileAndRun
        |> shouldSucceed


//# Negative cases
    //	SOURCE=E_OutOfRangeArgument01.fs  SCFLAGS="--codepage:65536"		# E_OutOfRangeArgument01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_InvalidArgument.fs"|])>]
    let ``OutOfRangeArgument01_fs`` compilation =
        compilation
        |> asExe
        |> withCodepage "65536"
        |> compile
        |> shouldFail
        |> withDiagnostics [
#if NETFRAMEWORK
            (Error 1000, Line 0, Col 1, Line 0, Col 1, "Problem with codepage '65536': Valid values are between 0 and 65535, inclusive.
Parameter name: codepage")
#else
            (Error 1000, Line 0, Col 1, Line 0, Col 1, "Problem with codepage '65536': Valid values are between 0 and 65535, inclusive. (Parameter 'codepage')")
#endif
        ]

    //	SOURCE=E_OutOfRangeArgument01.fs   SCFLAGS="--codepage:65536" FSIMODE=EXEC		# E_OutOfRangeArgument01.fs-fsi
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_InvalidArgument.fs"|])>]
    let ``E_OutOfRangeArgument01_fsx`` compilation =
        compilation
        |> asFsx
        |> withCodepage "65536"
        |> compile
        |> shouldFail
        |> withDiagnostics [
#if NETFRAMEWORK
            (Error 1000, Line 0, Col 1, Line 0, Col 1, "Problem with codepage '65536': Valid values are between 0 and 65535, inclusive.
Parameter name: codepage")
#else
            (Error 1000, Line 0, Col 1, Line 0, Col 1, "Problem with codepage '65536': Valid values are between 0 and 65535, inclusive. (Parameter 'codepage')")
#endif
        ]

    //	SOURCE=E_NegativeArgument01.fs    SCFLAGS="--codepage:-1"		# E_NegativeArgument01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_InvalidArgument.fs"|])>]
    let ``E_NegativeArgument01_fs`` compilation =
        compilation
        |> asExe
        |> withCodepage "-1"
        |> compile
        |> shouldFail
        |> withDiagnostics [
#if NETFRAMEWORK
            (Error 1000, Line 0, Col 1, Line 0, Col 1, "Problem with codepage '-1': Valid values are between 0 and 65535, inclusive.
Parameter name: codepage")
#else
            (Error 1000, Line 0, Col 1, Line 0, Col 1, "Problem with codepage '-1': Valid values are between 0 and 65535, inclusive. (Parameter 'codepage')")
#endif
        ]

    //	SOURCE=E_NegativeArgument01.fs     SCFLAGS="--codepage:-1" FSIMODE=EXEC		# E_NegativeArgument01.fs-fsi
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_InvalidArgument.fs"|])>]
    let ``E_NegativeArgument01_fsx`` compilation =
        compilation
        |> asFsx
        |> withCodepage "-1"
        |> compile
        |> shouldFail
        |> withDiagnostics [
#if NETFRAMEWORK
            (Error 1000, Line 0, Col 1, Line 0, Col 1, "Problem with codepage '-1': Valid values are between 0 and 65535, inclusive.
Parameter name: codepage")
#else
            (Error 1000, Line 0, Col 1, Line 0, Col 1, "Problem with codepage '-1': Valid values are between 0 and 65535, inclusive. (Parameter 'codepage')")
#endif
        ]

    //	SOURCE=E_NotAValidInteger01.fs    SCFLAGS="--codepage:invalidinteger"		# E_NotAValidInteger01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_InvalidArgument.fs"|])>]
    let ``E_NotAValidInteger01_fs`` compilation =
        compilation
        |> asExe
        |> withCodepage "invalidinteger"
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 241, Line 0, Col 1, Line 0, Col 1, "'invalidinteger' is not a valid integer argument")
        ]

    //	SOURCE=E_NotAValidInteger01.fs     SCFLAGS="--codepage:invalidinteger" FSIMODE=EXEC		# E_NotAValidInteger01.fs-fsi
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_InvalidArgument.fs"|])>]
    let ``E_NotAValidInteger01_fsx`` compilation =
        compilation
        |> asFsx
        |> withCodepage "invalidinteger"
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 241, Line 0, Col 1, Line 0, Col 1, "'invalidinteger' is not a valid integer argument")
        ]

    //	SOURCE=E_RequiresParameter01.fs   TAILFLAGS="--codepage" 	# E_RequiresParameter01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_InvalidArgument.fs"|])>]
    let ``E_RequiresParameter01_fs`` compilation =
        compilation
        |> asExe
        |> withOptions ["--codepage"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 224, Line 0, Col 1, Line 0, Col 1, "Option requires parameter: --codepage:<n>")
        ]

//	SOURCE=E_RequiresParameter01.fs   TAILFLAGS="--codepage"  FSIMODE=EXEC 	# E_RequiresParameter01.fs-fsi
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_InvalidArgument.fs"|])>]
    let ``E_RequiresParameter01_fsx`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--codepage"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 224, Line 0, Col 1, Line 0, Col 1, "Option requires parameter: --codepage:<n>")
        ]

    //	SOURCE=E_DefaultCodePage02.fsx     COMPILE_ONLY=1   			# E_DefaultCodePage02.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"libCodepage.fs"|])>]
    let ``libCodepage_fs`` compilation =
        compilation
        |> asLibrary
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 7, Col 10, Line 7, Col 11, "Unexpected character '�' in type name")
            (Error 10, Line 9, Col 14, Line 9, Col 17, "Unexpected keyword 'end' in implementation file")
        ]
