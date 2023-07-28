namespace Conformance.Types

open Xunit
open FSharp.Test.Compiler

module UnionStructTypes =

    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 1`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion =
    | A
    | B of string
        """
        |> typecheck
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
        |> typecheck
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
        |> typecheck
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
        |> typecheck
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
        |> typecheck
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
        |> typecheck
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
        |> typecheck
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
        |> typecheck
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
        |> typecheck
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
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 12, Line 5, Col 15, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
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
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 12, Line 5, Col 13, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
            (Error 3204, Line 6, Col 12, Line 6, Col 18, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
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
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 12, Line 5, Col 15, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
            (Error 3204, Line 6, Col 12, Line 6, Col 13, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
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
        |> typecheck
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
        |> typecheck
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
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 12, Line 5, Col 16, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
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
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 12, Line 5, Col 16, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
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
        |> typecheck
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
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 12, Line 5, Col 16, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
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
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 12, Line 5, Col 16, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
            (Error 3204, Line 5, Col 24, Line 5, Col 30, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
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
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 12, Line 5, Col 16, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
            (Error 3204, Line 5, Col 24, Line 5, Col 28, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
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
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 12, Line 5, Col 16, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
            (Error 3204, Line 5, Col 24, Line 5, Col 28, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
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
        |> typecheck
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
        |> typecheck
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
        |> typecheck
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
        |> typecheck
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
        |> typecheck
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
        |> typecheck
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
        |> typecheck
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
        |> typecheck
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
        |> typecheck
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
        |> typecheck
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
        |> typecheck
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
        |> typecheck
        |> shouldSucceed
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name 34`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion = A of int | B of string
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 4, Col 25, Line 4, Col 28, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
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
        |> typecheck
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
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 12, Line 5, Col 15, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
            (Error 3204, Line 5, Col 18, Line 5, Col 24, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
            (Error 3204, Line 6, Col 12, Line 6, Col 18, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
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
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 12, Line 5, Col 13, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
            (Error 3204, Line 5, Col 21, Line 5, Col 27, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
            (Error 3204, Line 6, Col 12, Line 6, Col 18, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
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
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 12, Line 5, Col 15, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
            (Error 3204, Line 5, Col 18, Line 5, Col 19, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
            (Error 3204, Line 6, Col 12, Line 6, Col 18, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
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
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 12, Line 5, Col 13, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
            (Error 3204, Line 5, Col 21, Line 5, Col 23, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
            (Error 3204, Line 6, Col 12, Line 6, Col 18, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
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
        |> typecheck
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
        |> typecheck
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
        |> typecheck
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
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3204, Line 5, Col 12, Line 5, Col 13, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
            (Error 3204, Line 5, Col 21, Line 5, Col 27, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
            (Error 3204, Line 6, Col 12, Line 6, Col 18, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
            (Error 3204, Line 6, Col 21, Line 6, Col 22, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
            (Error 3204, Line 7, Col 12, Line 7, Col 13, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
            (Error 3204, Line 7, Col 33, Line 7, Col 35, "If a multicase union type is a struct, then all union cases must have unique names. For example: 'type A = B of b: int | C of c: int'.")
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
        |> typecheck
        |> shouldSucceed
        
    [<Fact>]
    let ``If a union type has more than one case and is a struct, field must be given unique name involves an immediate cyclic reference`` () =
        Fsx """
namespace Foo
[<Struct>]
type StructUnion = A of X:int | B of Y:StructUnion
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 954, Line 4, Col 6, Line 4, Col 17, "This type definition involves an immediate cyclic reference through a struct field or inheritance relation")
        ]


    let createMassiveStructDuProgram countOfCases =
        let codeSb = 
            System.Text.StringBuilder("""
module Foo
[<Struct;NoEquality;NoComparison>]
type StructUnion = 
"""         )

        let basicTypes = [|"";"";"int";"string";"byte";"System.Uri";"int[]";"option<int>";"voption<int>";"System.Uri[]"|]
        
        for i=1 to countOfCases do
            let t = basicTypes[i%basicTypes.Length]
            if t = "" then 
                codeSb.AppendLine($"  | Case{i}") |> ignore
            else
                codeSb.AppendLine($"  | Case{i} of field1_{i}:{t} * field2_{i}:{t}") |> ignore
        
        codeSb.AppendLine($"""
[<EntryPoint>]
let main _argv = 
    printf "%%A" (Case{countOfCases} (Unchecked.defaultof<_>,Unchecked.defaultof<_>))
    0""") |> ignore

        Fs (codeSb.ToString())
        

    [<InlineData(5)>]
    [<InlineData(15)>]
    [<InlineData(65)>]
    [<Theory>]
    let ``Struct DU compilation does not embarassingly fail when having many data-less cases`` (countOfCases:int) =
        createMassiveStructDuProgram countOfCases
        |> asExe
        |> compile
        |> run
        |> shouldSucceed
        |> verifyOutput $"Case{countOfCases} (null, null)"


    [<Fact>]
    let ``Single case DU wrapper works`` ()  =
        Fsx """
namespace Foo

[<Struct;NoEquality;NoComparison>]
type DuWithData = SingleCase of field:int
        """
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Single case DU marker works`` ()  =
        Fsx """
namespace Foo
[<Struct;NoEquality;NoComparison>]
type TagOnlyDu = SingleCaseDuCase
        """
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Generic struct DU works`` ()  =
        Fsx """
namespace Foo
[<Struct;NoEquality;NoComparison>]
type GenericStructDu<'T> = EmptyFirst | SingleVal of f:'T | DoubleVal of f2:'T * int
        """
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Struct DU ValueOption keeps working`` ()  =
        Fsx """
module VOTests

let nothing = ValueNone
let someInt = ValueSome 42
let someSomeInt = ValueSome (ValueSome 2112)

let matchOnVO arg =
    match arg with
    | ValueNone -> 333
    | ValueSome 42 -> 666
    | ValueSome anyOther -> 999

let result1 = matchOnVO nothing
let result2 = matchOnVO someInt

printf $"{result1};{result2}"

        """
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> verifyOutput "333;666"

    [<Fact>]
    let ``Custom ValueOption keeps working`` () = 
        Fsx """
module XX
open System

[<NoEquality; NoComparison>]
[<Struct>]
type ThisIsMyValueOptionType<'T> =
    | MyValueNone 
    | MyValueSome of Item:'T

    static member None = MyValueNone
    static member Some (value) = MyValueSome(value)

        
        
let x = ThisIsMyValueOptionType<int>.Some(42)
[<EntryPoint>]
let main args =
    printf "%A" x
    0 """
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> verifyOutput "MyValueSome 42"

    [<Literal>]
    let sysDiagnostics = 
    #if NETCOREAPP
        "[runtime]System.Diagnostics"
    #else
        "System.Diagnostics"
    #endif

    [<Fact>]
    let ``Struct DU compilation - have a look at IL for massive cases`` () =
        createMassiveStructDuProgram 15
        |> asExe
        |> compile
        |> verifyIL [(*This is case-agnostic constructor used for data-less cases, just fills in the _tag property*)"""
            instance void  .ctor(int32 _tag) cil managed
    {
      .custom instance void """+sysDiagnostics+""".CodeAnalysis.DynamicDependencyAttribute::.ctor(valuetype """+sysDiagnostics+""".CodeAnalysis.DynamicallyAccessedMemberTypes,
                                                                                                              class [runtime]System.Type) = ( 01 00 60 06 00 00 0F 46 6F 6F 2B 53 74 72 75 63   
                                                                                                                                                     74 55 6E 69 6F 6E 00 00 )                         
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 Foo/StructUnion::_tag
      IL_0007:  ret
    } """;(*This is getter for a data-less case, just calling into the constructor above*)"""
            get_Case11() cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 0A 00 00 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldc.i4.s   10
      IL_0002:  newobj     instance void Foo/StructUnion::.ctor(int32)
      IL_0007:  ret
    }""";(*This is a 'maker method' New{CaseName} used for cases which do have fields associated with them, + the _tag gets initialized*)"""
            NewCase3(string _field1_3,
                     string _field2_3) cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 02 00 00 00 00 00 ) 
      
      .maxstack  3
      .locals init (valuetype Foo/StructUnion V_0)
      IL_0000:  ldloca.s   V_0
      IL_0002:  initobj    Foo/StructUnion
      IL_0008:  ldloca.s   V_0
      IL_000a:  ldc.i4.2
      IL_000b:  stfld      int32 Foo/StructUnion::_tag
      IL_0010:  ldloca.s   V_0
      IL_0012:  ldarg.0
      IL_0013:  stfld      string Foo/StructUnion::_field1_3
      IL_0018:  ldloca.s   V_0
      IL_001a:  ldarg.1
      IL_001b:  stfld      string Foo/StructUnion::_field2_3
      IL_0020:  ldloc.0
      IL_0021:  ret
    } 

    .method public hidebysig instance bool 
            get_IsCase3() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 Foo/StructUnion::get_Tag()
      IL_0006:  ldc.i4.2
      IL_0007:  ceq
      IL_0009:  ret
    } 
"""]
