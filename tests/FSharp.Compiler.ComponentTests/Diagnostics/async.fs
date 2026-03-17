// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace ComponentTests.Diagnostics

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module async =

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/async)
    //<Expects status="error" span="(6,33-6,39)" id="FS0001">Type mismatch\. Expecting a.+''a'.+but given a.+'Async<'a>'.*The types ''a' and 'Async<'a>' cannot be unified.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"MissingBangForLoop01.fs"|])>]
    let ``async - MissingBangForLoop01_fs - --warnaserror+ --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> compile
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "' cannot be unified."
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/async)
    //<Expects status="error" span="(7,54-7,61)" id="FS0001">Type mismatch\. Expecting a.    ''a'    .but given a.    'Async<'a>'    .The types ''a' and 'Async<'a>' cannot be unified.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"MissingBangForLoop02.fs"|])>]
    let ``async - MissingBangForLoop02_fs - --warnaserror+ --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> compile
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "' cannot be unified."
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/async)
    //<Expects status="error" span="(7,18-7,19)" id="FS0001">All branches of an 'if' expression must return values implicitly convertible to the type of the first branch</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"ReturnBangNonAsync_IfThenElse.fs"|])>]
    let ``async - ReturnBangNonAsync_IfThenElse_fs - --warnaserror+ --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> compile
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "All branches of an 'if' expression must return values implicitly convertible to the type of the first branch"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/async)
    //<Expects status="error" span="(14,16-14,28)" id="FS1228">'use!' bindings must be of the form 'use! <var> = <expr>'$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"UseBindingWrongForm01.fs"|])>]
    let ``async - UseBindingWrongForm01_fs - --warnaserror+ --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> compile
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "IDisposable"
        |> ignore

    // FS0025 - Incomplete pattern match
    //<Expects status="error" id="FS0025" span="(10,10-10,22)">Incomplete pattern matches</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"IncompleteMatchInAsync01.fs"|])>]
    let ``async - IncompleteMatchInAsync01_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> compile
        |> shouldFail
        |> withErrorCode 0025
        |> withDiagnosticMessageMatches "Incomplete pattern"
        |> ignore

    // FS0001 - let! with non-async value
    //<Expects status="error" span="(4,18-4,19)" id="FS0001">This expression was expected to have type 'Async<'a>' but here has type 'int'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"LetBangNonAsync.fs"|])>]
    let ``async - LetBangNonAsync_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> compile
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "Async<"
        |> ignore

    // FS0020 - Missing ignore
    //<Expects status="error" span="(4,9-4,10)" id="FS0020">The result of this expression has type 'int' and is implicitly ignored</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"MissingIgnore.fs"|])>]
    let ``async - MissingIgnore_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> compile
        |> shouldFail
        |> withErrorCode 0020
        |> withDiagnosticMessageMatches "implicitly ignored"
        |> ignore

    // FS0020 - Missing return! in for loop (variant 1)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"MissingReturnBangForLoop01.fs"|])>]
    let ``async - MissingReturnBangForLoop01_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> compile
        |> shouldFail
        |> withErrorCode 0020
        |> withDiagnosticMessageMatches "implicitly ignored"
        |> ignore

    // FS0020 - Missing return! in for loop (variant 2)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"MissingReturnBangForLoop02.fs"|])>]
    let ``async - MissingReturnBangForLoop02_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> compile
        |> shouldFail
        |> withErrorCode 0020
        |> withDiagnosticMessageMatches "implicitly ignored"
        |> ignore

    // FS0020 - Missing return! in for loop (variant 3)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"MissingReturnBangForLoop03.fs"|])>]
    let ``async - MissingReturnBangForLoop03_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> compile
        |> shouldFail
        |> withErrorCode 0020
        |> withDiagnosticMessageMatches "implicitly ignored"
        |> ignore

    // FS0020 - Missing return! in for loop (variant 4)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"MissingReturnBangForLoop04.fs"|])>]
    let ``async - MissingReturnBangForLoop04_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> compile
        |> shouldFail
        |> withErrorCode 0020
        |> withDiagnosticMessageMatches "implicitly ignored"
        |> ignore

    // FS0001 - return! with non-async in for loop
    //<Expects status="error" span="(5,18-5,19)" id="FS0001">This expression was expected to have type 'unit' but here has type 'int'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"ReturnBangNonAsync_For.fs"|])>]
    let ``async - ReturnBangNonAsync_For_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> compile
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "unit"
        |> ignore

    // FS0001 - return! with non-async in try/finally
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"ReturnBangNonAsync_TryFinally.fs"|])>]
    let ``async - ReturnBangNonAsync_TryFinally_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> compile
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "unit"
        |> ignore

    // FS0001 - return! with non-async in try/with
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"ReturnBangNonAsync_TryWith.fs"|])>]
    let ``async - ReturnBangNonAsync_TryWith_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> compile
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "int"
        |> ignore

    // FS0001 - return! with non-async in while loop
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"ReturnBangNonAsync_While.fs"|])>]
    let ``async - ReturnBangNonAsync_While_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> compile
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "Async<unit>"
        |> ignore

    // FS0001 - return! with non-async (variant 1)
    //<Expects status="error" span="(4,17-4,18)" id="FS0001">This expression was expected to have type 'Async<'a>' but here has type 'int'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"ReturnBangNonAsync01.fs"|])>]
    let ``async - ReturnBangNonAsync01_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> compile
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "Async<"
        |> ignore

    // FS0001 - return! with non-async (variant 2)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"ReturnBangNonAsync02.fs"|])>]
    let ``async - ReturnBangNonAsync02_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> compile
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "Async<"
        |> ignore

    // FS0001 - Using return in a while loop
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"UsingReturnInAWhileLoop.fs"|])>]
    let ``async - UsingReturnInAWhileLoop_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> compile
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "unit"
        |> ignore

    // FS0193 - Using return in if-then-else (type constraint mismatch)
    //<Expects status="error" span="(6,9-6,21)" id="FS0193">Type constraint mismatch</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"UsingReturnInIfThenElse.fs"|])>]
    let ``async - UsingReturnInIfThenElse_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> compile
        |> shouldFail
        |> withErrorCode 0193
        |> withDiagnosticMessageMatches "Type constraint mismatch"
        |> ignore

