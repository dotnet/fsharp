namespace FSharp.Compiler.ComponentTests.Conformance

open Xunit
open FSharp.Test.Compiler

module MultiCaseUnionStructTypes =
  
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 1`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A
    | B of string
        """
        |> compile
        |> shouldSucceed
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 2`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A
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
    | A
    | B of string
    | C
        """
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 4`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A
    | B of b: string
    | C
        """
        |> compile
        |> shouldSucceed
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 5`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A
    | B of b: string
    | C of bool
        """
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 6`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A
    | B
    | C of bool
        """
        |> compile
        |> shouldSucceed
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 7`` () =
        Fsx """
namespace Foo
[<Struct>]
type NotATree =
    | Empty
    | Children of struct (int * string)
        """
        |> compile
        |> shouldSucceed
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 8`` () =
        Fsx """
namespace Foo
[<Struct>]
type NotATree =
    | Empty
    | Children of struct (int * string)
        """
        |> compile
        |> shouldSucceed
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 9`` () =
        Fsx """
namespace Foo
[<Struct>]
type NotATree =
    | Empty
    | Children of a: struct (int * string)
        """
        |> compile
        |> shouldSucceed
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 10`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of int
    | B of string
    | C of string
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 12, Line 5, Col 15, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
        ]
        
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 11`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of a: int
    | B of string
    | C of string
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 12, Line 5, Col 13, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 6, Col 12, Line 6, Col 18, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
        ]
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 12`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of int
    | B of b: string
    | C of string
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 12, Line 5, Col 15, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 6, Col 12, Line 6, Col 13, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
        ]  
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 13`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of a: int
    | B of b: string
    | C of string
        """
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 14`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of int
    | B of b: string
    | C of c: string
        """
        |> compile
        |> shouldSucceed    
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 15`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of Item: int
    | B of string
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 12, Line 5, Col 16, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
        ]
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 16`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of Item: int
    | B of Item: string
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 12, Line 5, Col 16, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
        ]
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 17`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of Item: int
    | B of item : string
        """
        |> compile
        |> shouldSucceed
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 18`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of item: int
    | B of item: string
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 12, Line 5, Col 16, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
        ]
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 19`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of Item: int * string
    | B of string
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 12, Line 5, Col 16, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 5, Col 24, Line 5, Col 30, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
        ]
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 20`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of Item: int * item: string
    | B of string
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 12, Line 5, Col 16, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 5, Col 24, Line 5, Col 28, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
        ]
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 21`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of Item: int * item: string
    | B of item: string
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 12, Line 5, Col 16, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 5, Col 24, Line 5, Col 28, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
        ]
            
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 22`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of Item: int * string
    | B of item: string
        """
        |> compile
        |> shouldSucceed
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 23`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of item: string * int
    | B of string
        """
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 24`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of item: string * item: int
    | B of string
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3176, Line 5, Col 27, Line 5, Col 31, "Named field 'item' is used more than once.")
        ]
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 25`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of Item: string * Item: int
    | B of string
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3176, Line 5, Col 27, Line 5, Col 31, "Named field 'Item' is used more than once.")
        ]

    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 26`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion = A of a: int | B of b:string
        """
        |> compile
        |> shouldSucceed
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 27`` () =
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
    let ``If a union type has more than one case and is a struct, field must be given unique name 28`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of a: int * a1: string
    | B of string
        """
        |> compile
        |> shouldSucceed
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 29`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of int * string
    | B of string
        """
        |> compile
        |> shouldSucceed
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 30`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of a: int * string
    | B of string
        """
        |> compile
        |> shouldSucceed
   
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 31`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of int * a1: string
    | B of string
        """
        |> compile
        |> shouldSucceed
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 32`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of int
    | B of b: string
        """
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 33`` () =
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
    let ``If a union type has more than one case and is a struct, field must be given unique name 34`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion = A of int | B of string
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 4, Col 25, Line 4, Col 28, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
        ]
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 35`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of a: int
    | B of string
        """
        |> compile
        |> shouldSucceed
         
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 36`` () =
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
        ]
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 37`` () =
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
            (Error 3204, Line 5, Col 12, Line 5, Col 13, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 5, Col 21, Line 5, Col 27, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 6, Col 12, Line 6, Col 18, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
        ]
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 38`` () =
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
            (Error 3204, Line 5, Col 18, Line 5, Col 19, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 6, Col 12, Line 6, Col 18, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
        ]
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 39`` () =
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
            (Error 3204, Line 5, Col 12, Line 5, Col 13, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 5, Col 21, Line 5, Col 23, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 6, Col 12, Line 6, Col 18, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
        ]
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 40`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of a: int * a1: string
    | B of b: string
    | C of string
        """
        |> compile
        |> shouldSucceed
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 41`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of int * string
    | B of string
    | C of c: string
        """
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 42`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of int * string
    | B of b: string
    | C of string
        """
        |> compile
        |> shouldSucceed
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 43`` () =
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
            (Error 3204, Line 5, Col 12, Line 5, Col 13, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 5, Col 21, Line 5, Col 27, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 6, Col 12, Line 6, Col 18, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 6, Col 21, Line 6, Col 22, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 7, Col 12, Line 7, Col 13, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
            (Error 3204, Line 7, Col 33, Line 7, Col 35, "If a union type has more than one case and is a struct, then all fields within the union type must be given unique field names. For example: 'type A = B of b: int | C of c: int' (unique field names 'b' and 'c' assigned).")
        ]
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 44`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A of int * string
    | B of b1: string * b: string
    | C of c: string * c1: string * c3: int
        """
        |> compile
        |> shouldSucceed
        
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
