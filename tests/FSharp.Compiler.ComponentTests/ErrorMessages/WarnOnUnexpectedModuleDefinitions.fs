// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace ErrorMessages

open Xunit
open FSharp.Test.Compiler

module ``Module definition is encountered inside a type definition`` =

    [<Fact>]
    let ``Warn when a module definition is encountered inside a type definition``() =
        Fsx """
type IFace =
    abstract F : int -> int
    module M =
        let f () = ()

type C () =
    member _.F () = 3
    module M2 =
        let f () = ()

type U =
    | A
    | B
    module M3 =
        let f () = ()

type R =
    { A : int }
    module M4 =
        let f () = ()

type A = A

module M4 = begin end
module M5 = begin end
module M6 = begin end

type B =
    | B
    module M7 = begin end
    module M8 = begin end
    module M9 = begin end

module ThisIsFine =
    let f () = ()

type D = D
    """
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 554, Line 31, Col 5, Line 31, Col 11, "Invalid declaration syntax")
            (Warning 554, Line 32, Col 5, Line 32, Col 11, "Invalid declaration syntax")
            (Warning 554, Line 33, Col 5, Line 33, Col 11, "Invalid declaration syntax")
            (Warning 554, Line 20, Col 5, Line 20, Col 11, "Invalid declaration syntax")
            (Warning 554, Line 15, Col 5, Line 15, Col 11, "Invalid declaration syntax")
            (Warning 554, Line 9, Col 5, Line 9, Col 11, "Invalid declaration syntax");
            (Warning 554, Line 4, Col 5, Line 4, Col 11, "Invalid declaration syntax")
        ]
        
    [<Fact>]
    let ``Don't warn when a module definition is encountered inside a type definition``() =
        Fsx """
type IFace =
    abstract F : int -> int
    module M =
        let f () = ()

type C () =
    member _.F () = 3
    module M2 =
        let f () = ()

type U =
    | A
    | B
    module M3 =
        let f () = ()

type R =
    { A : int }
    module M4 =
        let f () = ()

type A = A

module M5 = begin end
module M6 = begin end
module M7 = begin end

type B =
    | B
    module M8 = begin end
    module M9 = begin end
    module M10 = begin end

module ThisIsFine =
    let f () = ()

type D = D
    """
        |> withLangVersion10
        |> typecheck
        |> shouldSucceed
