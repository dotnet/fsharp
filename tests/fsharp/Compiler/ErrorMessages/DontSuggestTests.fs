// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Compiler.SourceCodeServices

[<TestFixture>]
module ``Don't Suggest`` =

    [<Test>]
    let ``Dont Suggest Completely Wrong Stuff``() =
        CompilerAssert.TypeCheckSingleError
            """
let _ = Path.GetFullPath "images"
            """
            FSharpErrorSeverity.Error
            39
            (2, 9, 2, 13)
            "The value, namespace, type or module 'Path' is not defined."

    [<Test>]
    let ``Dont Suggest When Things Are Open``() =
        CompilerAssert.ParseWithErrors
            """
module N =
    let name = "hallo"

type T =
    static member myMember = 1

let x = N.
            """
            [|
                FSharpErrorSeverity.Error, 599, (8, 10, 8, 11), "Missing qualification after '.'"
            |]

    [<Test>]
    let ``Dont Suggest Intentionally Unused Variables``() =
        CompilerAssert.TypeCheckSingleError
            """
let hober xy _xyz = xyz
            """
            FSharpErrorSeverity.Error
            39
            (2, 21, 2, 24)
            "The value or constructor 'xyz' is not defined."
