// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module ErrorMessages.NamespaceTests

open Xunit
open FSharp.Test.Compiler

[<Fact>]
let ``Namespace cannot contain value bindings - multiple let bindings`` () =
    Fsx """
namespace TestNamespace

let x = 1 
let y = 2
let z = 3
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 201, Line 4, Col 5, Line 4, Col 6, "Namespaces cannot contain values. Consider using a module to hold your value declarations.")
            (Error 201, Line 5, Col 5, Line 5, Col 6, "Namespaces cannot contain values. Consider using a module to hold your value declarations.")
            (Error 201, Line 6, Col 5, Line 6, Col 6, "Namespaces cannot contain values. Consider using a module to hold your value declarations.")
        ]

[<Fact>]
let ``Namespace cannot contain function bindings`` () =
    Fsx """
namespace TestNamespace

let add x y = x + y 
let multiply x y = x * y
let divide x y = x / y
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 201, Line 4, Col 5, Line 4, Col 12, "Namespaces cannot contain values. Consider using a module to hold your value declarations.")
            (Error 201, Line 5, Col 5, Line 5, Col 17, "Namespaces cannot contain values. Consider using a module to hold your value declarations.")
            (Error 201, Line 6, Col 5, Line 6, Col 15, "Namespaces cannot contain values. Consider using a module to hold your value declarations.")
        ]

[<Fact>]
let ``Multiple namespaces with value bindings should all report errors`` () =
    Fsx """
namespace Namespace1

let x = 1

namespace Namespace2

let y = 2
do printfn "test"

namespace Namespace3

let z = 3 
let w = 4
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 201, Line 4, Col 5, Line 4, Col 6, "Namespaces cannot contain values. Consider using a module to hold your value declarations.")
            (Error 201, Line 8, Col 5, Line 8, Col 6, "Namespaces cannot contain values. Consider using a module to hold your value declarations.")
            (Error 201, Line 9, Col 1, Line 9, Col 18, "Namespaces cannot contain values. Consider using a module to hold your value declarations.")
            (Error 201, Line 13, Col 5, Line 13, Col 6, "Namespaces cannot contain values. Consider using a module to hold your value declarations.")
            (Error 201, Line 14, Col 5, Line 14, Col 6, "Namespaces cannot contain values. Consider using a module to hold your value declarations.")
        ]