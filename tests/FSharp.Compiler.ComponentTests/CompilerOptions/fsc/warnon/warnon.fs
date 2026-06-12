// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module CompilerWarnOn =

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warnon)
    //<Expects status="warning" span="(18,11-18,12)" id="FS1182">The value 'n' is unused$</Expects>
    [<Theory; FileInlineData("warnon01.fs")>]
    let ``warnon - warnon01_fs - --warnon:1182 --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation 
        |> asFsx
        |> withOptions ["--warnon:1182"; "--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withWarningCode 1182
        |> withDiagnosticMessageMatches "The value 'n' is unused$"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warnon)
    //<Expects status="warning" span="(18,11-18,12)" id="FS1182">The value 'n' is unused$</Expects>
    [<Theory; FileInlineData("warnon01.fs")>]
    let ``warnon - warnon01_fs - --warnon:NU0001;FS1182;NU0001 --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation 
        |> asFsx
        |> withOptions ["--warnon:NU0001;FS1182;NU0001"; "--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withWarningCode 1182
        |> withDiagnosticMessageMatches "The value 'n' is unused$"
        |> ignore

    [<Fact>]
    let ``warnon unused public function hidden by signature file`` () =
        let signatureFile: SourceCodeFileKind =
            SourceCodeFileKind.Create(
                "Library.fsi",
                """
module Foo

val add: a:int -> b:int -> int
    """     )

        let implementationFile =
            SourceCodeFileKind.Create(
                "Library.fs",
                """
module Foo

let add a b = a + b
let subtract a b = a - b
    """         )

        fsFromString signatureFile
        |> FS
        |> withAdditionalSourceFile implementationFile
        |> withOptions ["--warnon:FS1182"]
        |> asLibrary
        |> compile
        |> withWarningCode 1182
        |> withDiagnosticMessageMatches "The value 'subtract' is unused$"
        |> ignore

    [<Fact>]
    let ``Don't warnon unused public function`` () =
        let implementationFile =
            SourceCodeFileKind.Create(
                "Library.fs",
                """
module Foo

let add a b = a + b
let subtract a b = a - b
    """         )

        fsFromString implementationFile
        |> FS
        |> withOptions ["--warnon:FS1182"]
        |> asLibrary
        |> compile
        |> withDiagnostics []
        |> ignore

    [<Fact>]
    let ``Don't warnon unused public function hidden by signature file that starts with an underscore`` () =
        let signatureFile: SourceCodeFileKind =
            SourceCodeFileKind.Create(
                "Library.fsi",
                """
module Foo

val add: a:int -> b:int -> int
    """     )

        let implementationFile =
            SourceCodeFileKind.Create(
                "Library.fs",
                """
module Foo

let add a b = a + b
let _subtract a b = a - b
    """         )

        fsFromString signatureFile
        |> FS
        |> withAdditionalSourceFile implementationFile
        |> withOptions ["--warnon:FS1182"]
        |> asLibrary
        |> compile
        |> withDiagnostics []
        |> ignore

    [<Fact>]
    let ``Type extensions are not included in this warnon check`` () =
        let signatureFile: SourceCodeFileKind =
            SourceCodeFileKind.Create(
                "Library.fsi",
                """
module Foo
    """     )

        let implementationFile =
            SourceCodeFileKind.Create(
                "Library.fs",
                """
module Foo

type System.Int32 with
    member x.Bar () = x + 1
    """         )

        fsFromString signatureFile
        |> FS
        |> withAdditionalSourceFile implementationFile
        |> withOptions ["--warnon:FS1182"]
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // -------------------------------------------------------------------------
    // FS1182 tests for `let`-bound functions inside class types (issue #13849).
    // Today, FS1182 fires for unused `let`-bound values inside classes, but not
    // for unused `let`-bound functions inside classes. These tests pin the
    // expected post-fix behavior. The tests marked "RED" must FAIL today and
    // pass once the fix lands; the tests marked "GUARD" must already pass.
    // -------------------------------------------------------------------------

    // RED: primary repro from the issue.
    [<Fact>]
    let ``Unused let function in class reports FS1182`` () =
        FSharp """
module M
type T() =
    let f _ = ()
"""
        |> withOptions ["--warnon:FS1182"]
        |> asLibrary
        |> compile
        |> withWarningCode 1182
        |> withDiagnosticMessageMatches "The value 'f' is unused"
        |> ignore

    // RED: variants of unused let-function in classes.
    // Note: the recursive variant (`let rec f x = ... f (x-1)`) is intentionally
    // omitted from this Theory because F# treats the self-reference inside a
    // `rec` binding as a use, so it does not warn; that behavior is unchanged by
    // the fix and is covered separately below as a negative regression guard.
    [<Theory>]
    [<InlineData("module M\ntype T() = let f x = x + 1")>]
    [<InlineData("module M\ntype T() = let f (x:int) (y:int) = x + y")>]
    [<InlineData("module M\ntype T() = let f x y = x + y")>]
    let ``Unused let function variants in class report FS1182`` (source: string) =
        FSharp source
        |> withOptions ["--warnon:FS1182"]
        |> asLibrary
        |> compile
        |> withWarningCode 1182
        |> withDiagnosticMessageMatches "The value 'f' is unused"
        |> ignore

    // GUARD: a recursive let-function in a class that only references itself.
    // F# considers the self-reference inside `let rec` as a use, so no FS1182
    // is expected (today and after the fix). Documented as a regression guard.
    [<Fact>]
    let ``Recursive let function in class with only self-reference does NOT report FS1182`` () =
        FSharp """
module M
type T() =
    let rec f x = if x = 0 then 1 else f (x-1)
"""
        |> withOptions ["--warnon:FS1182"]
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> withDiagnostics []
        |> ignore

    // GUARD: used let-function in a class must not warn (today and after fix).
    [<Fact>]
    let ``Used let function in class does NOT report FS1182`` () =
        FSharp """
module M
type T() =
    let f x = x + 1
    member _.GetValue() = f 42
"""
        |> withOptions ["--warnon:FS1182"]
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> withDiagnostics []
        |> ignore

    // GUARD: unused let-value in a class already warns today.
    [<Fact>]
    let ``Unused let value in class still reports FS1182`` () =
        FSharp """
module M
type T() =
    let x = 42
"""
        |> withOptions ["--warnon:FS1182"]
        |> asLibrary
        |> compile
        |> withWarningCode 1182
        |> withDiagnosticMessageMatches "The value 'x' is unused"
        |> ignore

    // GUARD: module-level unused functions must NOT warn (they are public surface).
    // This guards against accidentally widening the fix beyond class-let bindings.
    [<Fact>]
    let ``Module-level unused function does not get new false positive`` () =
        FSharp """
module M
let f x = x + 1
"""
        |> withOptions ["--warnon:FS1182"]
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> withDiagnostics []
        |> ignore

    // GUARD: underscore-prefixed unused let-function in a class must NOT warn.
    [<Fact>]
    let ``Underscore-prefixed let function in class does NOT report FS1182`` () =
        FSharp """
module M
type T() =
    let _f x = x + 1
"""
        |> withOptions ["--warnon:FS1182"]
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> withDiagnostics []
        |> ignore

    // RED: mixed used/unused let-functions in a class - only unused one warns.
    [<Fact>]
    let ``Mixed used and unused let functions in class`` () =
        FSharp """
module M
type T() =
    let used x = x + 1
    let unused x = x * 2
    member _.GetValue() = used 42
"""
        |> withOptions ["--warnon:FS1182"]
        |> asLibrary
        |> compile
        |> withWarningCode 1182
        |> withDiagnosticMessageMatches "The value 'unused' is unused"
        |> ignore

    // RED: static let function in a class - same code path with isStatic=true.
    [<Fact>]
    let ``Unused static let function in class reports FS1182`` () =
        FSharp """
module M
type T() =
    static let f x = x + 1
"""
        |> withOptions ["--warnon:FS1182"]
        |> asLibrary
        |> compile
        |> withWarningCode 1182
        |> withDiagnosticMessageMatches "The value 'f' is unused"
        |> ignore
