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

    // Issue #422: FS1182 false positive in query expressions - FIXED
    // The fix marks synthetic lambda parameters in query translation as compiler-generated,
    // which prevents the FS1182 warning from firing for query variables.
    [<Fact>]
    let ``Query expression variable with underscore prefix should not warn FS1182`` () =
        FSharp """
module Test

let result = 
    query { for _x in [1;2;3] do
            select 1 }
        """
        |> withOptions ["--warnon:FS1182"]
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Query expression variable used in select should not warn FS1182`` () =
        FSharp """
module Test

let result = 
    query { for x in [1;2;3] do
            where (x > 2)
            select x }
        """
        |> withOptions ["--warnon:FS1182"]
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // Issue #422: DoD test - variable used in where doesn't trigger FS1182
    [<Fact>]
    let ``Query variable used in where does not trigger FS1182 - issue 422`` () =
        FSharp """
module Test

let result = 
    query { for x in [1;2;3;4;5] do
            where (x > 0)
            select 1 }
        """
        |> withOptions ["--warnon:FS1182"]
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // Issue #422: DoD test - variable used in let binding doesn't trigger FS1182
    [<Fact>]
    let ``Query variable used in let binding does not trigger FS1182 - issue 422`` () =
        FSharp """
module Test

let result = 
    query { for x in [1;2;3] do
            let y = x * 2
            select y }
        """
        |> withOptions ["--warnon:FS1182"]
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // Issue #422: DoD test - join variable used elsewhere doesn't trigger FS1182
    [<Fact>]
    let ``Join variable used in select does not trigger FS1182 - issue 422`` () =
        FSharp """
module Test

let data1 = [1;2;3]
let data2 = [(1, "one"); (2, "two"); (3, "three")]

let result = 
    query { for x in data1 do
            join (y, name) in data2 on (x = y)
            select name }
        """
        |> withOptions ["--warnon:FS1182"]
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // Issue #422: Multiple query variables in different clauses
    [<Fact>]
    let ``Multiple query variables in nested for do not trigger FS1182 - issue 422`` () =
        FSharp """
module Test

let result = 
    query { for a in [1;2;3] do
            for b in [4;5;6] do
            where (a < b)
            select (a + b) }
        """
        |> withOptions ["--warnon:FS1182"]
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore
