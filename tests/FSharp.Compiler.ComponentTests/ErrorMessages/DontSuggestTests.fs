// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ErrorMessages.ComponentTests

open Xunit
open FSharp.Test.Utilities
open FSharp.Compiler.SourceCodeServices

module ``Don't Suggest`` =

    [<Fact>]
    let ``Dont Suggest Completely Wrong Stuff``() =
        CompilerAssert.TypeCheckSingleError
            """
let _ = Path.GetFullPath "images"
            """
            FSharpErrorSeverity.Error
            39
            (2, 9, 2, 13)
            ("The value, namespace, type or module 'Path' is not defined. Maybe you want one of the following:" + System.Environment.NewLine + "   Math")

    [<Fact>]
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
                FSharpErrorSeverity.Error, 222, (2, 1, 3, 1), "Files in libraries or multiple-file applications must begin with a namespace or module declaration. When using a module declaration at the start of a file the '=' sign is not allowed. If this is a top-level module, consider removing the = to resolve this error."
            |]

    [<Fact>]
    let ``Dont Suggest Intentionally Unused Variables``() =
        CompilerAssert.TypeCheckSingleError
            """
let hober xy _xyz = xyz
            """
            FSharpErrorSeverity.Error
            39
            (2, 21, 2, 24)
            "The value or constructor 'xyz' is not defined."
