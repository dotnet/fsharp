// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CompilerOptions.fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module warnon =

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warnon)
    //<Expects status="warning" span="(18,11-18,12)" id="FS1182">The value 'n' is unused$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"warnon01.fs"|])>]
    let ``warnon - warnon01.fs - --warnon:1182 --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnon:1182"; "--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withWarningCode 1182
        |> withDiagnosticMessageMatches "The value 'n' is unused$"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warnon)
    //<Expects status="warning" span="(18,11-18,12)" id="FS1182">The value 'n' is unused$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"warnon01.fs"|])>]
    let ``warnon - warnon01.fs - --warnon:NU0001;FS1182;NU0001 --test:ErrorRanges`` compilation =
        compilation
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
