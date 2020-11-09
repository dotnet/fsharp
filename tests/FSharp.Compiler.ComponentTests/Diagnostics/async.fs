// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Diagnostics

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module async =

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/async)
    //<Expects status="error" span="(4,18-4,19)" id="FS0001">This expression was expected to have type.    'Async<'a>'    .but here has type.    'int'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"LetBangNonAsync.fs"|])>]
    let ``async - LetBangNonAsync.fs - --warnaserror+ --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "'    .but here has type.    'int'"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/async)
    //<Expects status="error" span="(6,33-6,39)" id="FS0001">Type mismatch\. Expecting a.+''a'.+but given a.+'Async<'a>'.*The types ''a' and 'Async<'a>' cannot be unified.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"MissingBangForLoop01.fs"|])>]
    let ``async - MissingBangForLoop01.fs - --warnaserror+ --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "' cannot be unified."

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/async)
    //<Expects status="error" span="(7,54-7,61)" id="FS0001">Type mismatch\. Expecting a.    ''a'    .but given a.    'Async<'a>'    .The types ''a' and 'Async<'a>' cannot be unified.</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"MissingBangForLoop02.fs"|])>]
    let ``async - MissingBangForLoop02.fs - --warnaserror+ --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "' cannot be unified."

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/async)
    //<Expects status="error" span="(4,9-4,10)" id="FS0020">The result of this expression has type 'int' and is implicitly ignored\. Consider using 'ignore' to discard this value explicitly, e\.g\. 'expr \|> ignore', or 'let' to bind the result to a name, e\.g\. 'let result = expr'.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"MissingIgnore.fs"|])>]
    let ``async - MissingIgnore.fs - --warnaserror+ --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0020
        |> withDiagnosticMessageMatches " ignore', or 'let' to bind the result to a name, e\.g\. 'let result = expr'.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/async)
    //<Expects status="error" span="(5,39-5,45)" id="FS0020">The result of this expression has type 'Async<'a>' and is implicitly ignored\. Consider using 'ignore' to discard this value explicitly, e\.g\. 'expr \|> ignore', or 'let' to bind the result to a name, e\.g\. 'let result = expr'.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"MissingReturnBangForLoop01.fs"|])>]
    let ``async - MissingReturnBangForLoop01.fs - --warnaserror+ --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0020
        |> withDiagnosticMessageMatches " ignore', or 'let' to bind the result to a name, e\.g\. 'let result = expr'.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/async)
    //<Expects status="error" span="(5,26-5,32)" id="FS0020">The result of this expression has type 'Async<'a>' and is implicitly ignored\. Consider using 'ignore' to discard this value explicitly, e\.g\. 'expr \|> ignore', or 'let' to bind the result to a name, e\.g\. 'let result = expr'.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"MissingReturnBangForLoop02.fs"|])>]
    let ``async - MissingReturnBangForLoop02.fs - --warnaserror+ --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0020
        |> withDiagnosticMessageMatches " ignore', or 'let' to bind the result to a name, e\.g\. 'let result = expr'.$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/async)
    //<Expects status="error" span="(6,50-6,52)" id="FS0001">This expression was expected to have type.    'Async<'a>'    .but here has type.    'unit'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"MissingReturnBangForLoop03.fs"|])>]
    let ``async - MissingReturnBangForLoop03.fs - --warnaserror+ --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "'    .but here has type.    'unit'"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/async)
    //<Expects status="error" span="(8,44-8,46)" id="FS0001">This expression was expected to have type.    'Async<'a>'    .but here has type.    'unit'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"MissingReturnBangForLoop04.fs"|])>]
    let ``async - MissingReturnBangForLoop04.fs - --warnaserror+ --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "'    .but here has type.    'unit'"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/async)
    //<Expects status="error" span="(4,17-4,18)" id="FS0001">This expression was expected to have type.    'Async<'a>'    .but here has type.    'int'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"ReturnBangNonAsync01.fs"|])>]
    let ``async - ReturnBangNonAsync01.fs - --warnaserror+ --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "'    .but here has type.    'int'"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/async)
    //<Expects status="error" span="(5,17-5,18)" id="FS0001">This expression was expected to have type.    'Async<'a>'    .but here has type.    'int'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"ReturnBangNonAsync02.fs"|])>]
    let ``async - ReturnBangNonAsync02.fs - --warnaserror+ --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "'    .but here has type.    'int'"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/async)
    //<Expects status="error" span="(5,18-5,19)" id="FS0001">This expression was expected to have type.    'unit'    .but here has type.    'int'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"ReturnBangNonAsync_For.fs"|])>]
    let ``async - ReturnBangNonAsync_For.fs - --warnaserror+ --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "This expression was expected to have type.    'unit'    .but here has type.    'int'"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/async)
    //<Expects status="error" span="(7,18-7,19)" id="FS0001">All branches of an 'if' expression must return values of the same type as the first branch</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"ReturnBangNonAsync_IfThenElse.fs"|])>]
    let ``async - ReturnBangNonAsync_IfThenElse.fs - --warnaserror+ --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "All branches of an 'if' expression must return values of the same type as the first branch"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/async)
    //<Expects status="error" span="(7,11-7,12)" id="FS0001">This expression was expected to have type.    'unit'    .but here has type.    'int'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"ReturnBangNonAsync_TryFinally.fs"|])>]
    let ``async - ReturnBangNonAsync_TryFinally.fs - --warnaserror+ --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "This expression was expected to have type.    'unit'    .but here has type.    'int'"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/async)
    //<Expects status="error" span="(7,18-7,20)" id="FS0001">This expression was expected to have type.    'int'    .but here has type.    'unit'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"ReturnBangNonAsync_TryWith.fs"|])>]
    let ``async - ReturnBangNonAsync_TryWith.fs - --warnaserror+ --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "This expression was expected to have type.    'int'    .but here has type.    'unit'"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/async)
    //<Expects status="error" span="(6,19-6,20)" id="FS0001">This expression was expected to have type.    'Async<unit>'    .but here has type.    'int'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"ReturnBangNonAsync_While.fs"|])>]
    let ``async - ReturnBangNonAsync_While.fs - --warnaserror+ --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "'    .but here has type.    'int'"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/async)
    //<Expects status="error" span="(7,18-7,19)" id="FS0001">This expression was expected to have type.    'unit'    .but here has type.    'int'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"UsingReturnInAWhileLoop.fs"|])>]
    let ``async - UsingReturnInAWhileLoop.fs - --warnaserror+ --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "This expression was expected to have type.    'unit'    .but here has type.    'int'"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/async)
    //<Expects status="error" span="(6,9-6,21)" id="FS0001">Type mismatch\. Expecting a.    'Async<int>'    .but given a.    'Async<unit>'    .The type 'int' does not match the type 'unit'$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"UsingReturnInIfThenElse.fs"|])>]
    let ``async - UsingReturnInIfThenElse.fs - --warnaserror+ --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "'    .The type 'int' does not match the type 'unit'$"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/async)
    //<Expects status="error" span="(14,16-14,28)" id="FS1228">'use!' bindings must be of the form 'use! <var> = <expr>'$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"UseBindingWrongForm01.fs"|])>]
    let ``async - UseBindingWrongForm01.fs - --warnaserror+ --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1228
        |> withDiagnosticMessageMatches "'$"

