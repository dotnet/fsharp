namespace FSharp.Compiler.ComponentTests.Conformance

open Xunit
open FSharp.Test.Compiler

module MultiCaseUnionStructTypes =
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 1`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion = A of a: int | B of b:string
        """
        |> compile
        |> shouldSucceed
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 2`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of a: int
    | B of b: string
        """
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 3`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of a: int * a1: string
    | B of b: string
        """
        |> compile
        |> shouldSucceed
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 4`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion = A of int | B of string
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 4, Col 25, Line 4, Col 28, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 4, Col 36, Line 4, Col 42, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
        ]
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 5`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of a: int
    | B of string
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 6, Col 12, Line 6, Col 18, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
        ]
         
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 6`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of int * string
    | B of string
    | C of string
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 12, Line 5, Col 15, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 5, Col 18, Line 5, Col 24, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 6, Col 12, Line 6, Col 18, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 7, Col 12, Line 7, Col 18, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
        ]
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 7`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of a: int * string
    | B of string
    | C of string
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 21, Line 5, Col 27, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 6, Col 12, Line 6, Col 18, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 7, Col 12, Line 7, Col 18, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
        ]
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 8`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of int * a: string
    | B of string
    | C of string
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 12, Line 5, Col 15, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 6, Col 12, Line 6, Col 18, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 7, Col 12, Line 7, Col 18, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
        ]
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 9`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of a: int * a1: string
    | B of string
    | C of string
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 6, Col 12, Line 6, Col 18, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 7, Col 12, Line 7, Col 18, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
        ]
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 10`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of a: int * a1: string
    | B of b: string
    | C of string
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 7, Col 12, Line 7, Col 18, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
        ]
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 11`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of int * string
    | B of string
    | C of c: string
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 12, Line 5, Col 15, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 5, Col 18, Line 5, Col 24, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 6, Col 12, Line 6, Col 18, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
        ]

    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 12`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of int * string
    | B of b: string
    | C of string
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 12, Line 5, Col 15, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 5, Col 18, Line 5, Col 24, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 7, Col 12, Line 7, Col 18, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
        ]
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 13`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of a: int * string
    | B of string * b: string
    | C of c: string * string * c3: int
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 21, Line 5, Col 27, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 6, Col 12, Line 6, Col 18, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 7, Col 24, Line 7, Col 30, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
        ]
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name. involves an immediate cyclic reference`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion = A of X:int | B of Y:StructUnion
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 954, Line 4, Col 6, Line 4, Col 17, "This type definition involves an immediate cyclic reference through a struct field or inheritance relation")
        ]
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 14`` () =
        Fsx """
namespace Foo
[<Struct>]
type NotATree =
    | Empty
    | Children of struct (int * string)
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 6, Col 19, Line 6, Col 40, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
        ]
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 15`` () =
        Fsx """
namespace Foo
[<Struct>]
type NotATree =
    | Empty
    | Children of a: struct (int * string)
        """
        |> compile
        |> shouldSucceed