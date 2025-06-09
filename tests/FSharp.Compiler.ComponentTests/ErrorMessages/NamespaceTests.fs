// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module ErrorMessages.NamespaceTests

open Xunit
open FSharp.Test.Compiler

[<Fact>]
let ``Value bindings 01`` () =
    Fsx """
namespace TestNamespace

let x = 1 
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 201, Line 4, Col 5, Line 4, Col 6, "Namespaces cannot contain values. Consider using a module to hold your value declarations.")
        ]

[<Fact>]
let ``Value bindings 02 - Multiple`` () =
    Fsx """
namespace TestNamespace

let x = 1 
let y = 2
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 201, Line 4, Col 5, Line 4, Col 6, "Namespaces cannot contain values. Consider using a module to hold your value declarations.")
            (Error 201, Line 5, Col 5, Line 5, Col 6, "Namespaces cannot contain values. Consider using a module to hold your value declarations.")
        ]

[<Fact>]
let ``Function bindings`` () =
    Fsx """
namespace TestNamespace

let add x y = x + y 
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 201, Line 4, Col 5, Line 4, Col 12, "Namespaces cannot contain values. Consider using a module to hold your value declarations.")
        ]

[<Fact>]
let ``Multiple namespaces`` () =
    Fsx """
namespace Namespace1

let x = 1

namespace Namespace2

let y = 2

namespace Namespace3

let z = 3 
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 201, Line 4, Col 5, Line 4, Col 6, "Namespaces cannot contain values. Consider using a module to hold your value declarations.")
            (Error 201, Line 8, Col 5, Line 8, Col 6, "Namespaces cannot contain values. Consider using a module to hold your value declarations.")
            (Error 201, Line 12, Col 5, Line 12, Col 6, "Namespaces cannot contain values. Consider using a module to hold your value declarations.")
        ]

[<Fact>]
let ``Do expressions`` () =
    Fsx """
namespace TestNamespace

do printfn "test"
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 201, Line 4, Col 1, Line 4, Col 18, "Namespaces cannot contain values. Consider using a module to hold your value declarations.")
        ]