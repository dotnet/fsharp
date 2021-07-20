// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.ErrorMessages

open Xunit
open FSharp.Test.Compiler

module ``Don't Suggest`` =

    [<Fact>]
    let ``Dont Suggest Completely Wrong Stuff``() =
        FSharp """
let _ = Path.GetFullPath "images"
        """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 39, Line 2, Col 9, Line 2, Col 13,
                                 "The value, namespace, type or module 'Path' is not defined. Maybe you want one of the following:" + System.Environment.NewLine + "   Math")

    [<Fact>]
    let ``Dont Suggest When Things Are Open``() =
        FSharp """
module N =
    let name = "hallo"

type T =
    static member myMember = 1

let x = N.
        """
        |> parse
        |> shouldFail
        |> withDiagnostics [
            (Error 599, Line 8, Col 10, Line 8, Col 11, "Missing qualification after '.'")
            (Error 222, Line 2, Col 1,  Line 3, Col 1,  "Files in libraries or multiple-file applications must begin with a namespace or module declaration. When using a module declaration at the start of a file the '=' sign is not allowed. If this is a top-level module, consider removing the = to resolve this error.")]

    [<Fact>]
    let ``Dont Suggest Intentionally Unused Variables``() =
        FSharp """
let hober xy _xyz = xyz
        """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 39, Line 2, Col 21, Line 2, Col 24, "The value or constructor 'xyz' is not defined.")
