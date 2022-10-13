namespace FSharp.Compiler.ComponentTests.Conformance

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module MultiCaseUnionStructTypes =

    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name. 1`` () =
        Fsx """
namespace Foo
[<Struct>]
type A =
    | B of a: int
    | C of c: string
    | D of string
        """
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name. 2`` () =
        Fsx """
namespace Foo
[<Struct>]
type A =
    | B of a: int * string
    | C of c: string
    | D of string
        """
        |> compile
        |> shouldSucceed
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name. 3`` () =
        Fsx """
namespace Foo
[<Struct>]
type A =
    | B of a: int * x: string
    | C of c: string
    | D of string
        """
        |> compile
        |> shouldSucceed
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name. 4`` () =
        Fsx """
namespace Foo
[<Struct>]
type A =
    | B of int
    | C of string
    | D of string
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
           (Error 3204, Line 5, Col 12, Line 5, Col 15, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique names.")
           (Error 3204, Line 6, Col 12, Line 6, Col 18, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique names.")
           (Error 3204, Line 7, Col 12, Line 7, Col 18, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique names.")
        ]
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name. 5`` () =
        Fsx """
namespace Foo
[<Struct>]
type A =
    | B of a: int
    | C of string
    | D of string
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
           (Error 3204, Line 6, Col 12, Line 6, Col 18, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique names.")
           (Error 3204, Line 7, Col 12, Line 7, Col 18, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique names.")
        ]