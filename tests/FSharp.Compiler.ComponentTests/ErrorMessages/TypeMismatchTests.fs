// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace ErrorMessages

open Xunit
open FSharp.Test.Compiler

module ``Type Mismatch`` =

    module ``Different tuple lengths`` =

        [<Fact>]
        let ``Known type on the left``() =
            FSharp """
let x a b c : int * int = a, b, c
            """
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 1, Line 2, Col 27, Line 2, Col 34,
                 "Type mismatch. Expecting a tuple of length 2 of type\n    int * int    \nbut given a tuple of length 3 of type\n    'a * 'b * 'c    \n")
            ]

        [<Fact>]
        let ``Type annotation propagates to the error message``() =
            FSharp """
let x a b (c: string) : int * int = a, b, c
let y a (b: string) c : int * int = a, b, c
            """
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 1, Line 2, Col 37, Line 2, Col 44,
                 "Type mismatch. Expecting a tuple of length 2 of type\n    int * int    \nbut given a tuple of length 3 of type\n    'a * 'b * string    \n")
                (Error 1, Line 3, Col 37, Line 3, Col 44,
                 "Type mismatch. Expecting a tuple of length 2 of type\n    int * int    \nbut given a tuple of length 3 of type\n    'a * string * 'b    \n")
            ]

        [<Fact>]
        let ``Known type on the right``() =
            FSharp """
let x : int * string = 1, ""
let a, b, c = x
            """
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 1, Line 3, Col 15, Line 3, Col 16,
                 "Type mismatch. Expecting a tuple of length 3 of type\n    'a * 'b * 'c    \nbut given a tuple of length 2 of type\n    int * string    \n")
            ]

        [<Fact>]
        let ``Known types on both sides``() =
            FSharp """
let x: int * int * int = 1, ""
let x: int * string * int = "", 1
let x: int * int = "", "", 1
            """
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 1, Line 2, Col 26, Line 2, Col 31,
                 "Type mismatch. Expecting a tuple of length 3 of type\n    int * int * int    \nbut given a tuple of length 2 of type\n    int * string    \n")
                (Error 1, Line 3, Col 29, Line 3, Col 34,
                 "Type mismatch. Expecting a tuple of length 3 of type\n    int * string * int    \nbut given a tuple of length 2 of type\n    string * int    \n")
                (Error 1, Line 4, Col 20, Line 4, Col 29,
                 "Type mismatch. Expecting a tuple of length 2 of type\n    int * int    \nbut given a tuple of length 3 of type\n    string * string * int    \n")
            ]

        [<Fact>]
        let ``Patterns minimal`` () =
            FSharp """
let test (x : int * string * char) =
    match x with
    | 10, "20"      -> true
    | _ -> false
            """
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 1, Line 4, Col 7, Line 4, Col 15,
                 "Type mismatch. Expecting a tuple of length 3 of type\n    int * string * char    \nbut given a tuple of length 2 of type\n    int * string    \n")
            ]

        [<Fact>]
        let ``Patterns with inference`` () =
            FSharp """
let test x =
    match x with
    |  0,  "1", '2' -> true
    | 10, "20"      -> true
    |     "-1", '0' -> true
    | 99,       '9' -> true
    | _ -> false
            """
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 1, Line 5, Col 7, Line 5, Col 15,
                 "Type mismatch. Expecting a tuple of length 3 of type\n    int * string * char    \nbut given a tuple of length 2 of type\n    int * string    \n")
                (Error 1, Line 6, Col 11, Line 6, Col 20,
                 "Type mismatch. Expecting a tuple of length 3 of type\n    int * string * char    \nbut given a tuple of length 2 of type\n    string * char    \n")
                (Error 1, Line 7, Col 7, Line 7, Col 20,
                 "Type mismatch. Expecting a tuple of length 3 of type\n    int * string * char    \nbut given a tuple of length 2 of type\n    int * char    \n")
            ]

        [<Fact>]
        let ``Else branch context``() =
            FSharp """
let f1(a, b: string, c) =
    if true then (1, 2) else (a, b, c)
            """
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 1, Line 3, Col 31, Line 3, Col 38,
                 "All branches of an 'if' expression must return values implicitly convertible to the type of the first branch, which here is a tuple of length 2 of type\n    int * int    \nThis branch returns a tuple of length 3 of type\n    'a * string * 'b    \n")
            ]

        [<Fact>]
        let ``Match branch context``() =
            FSharp """
let f x =
    match x with
    | 0 -> 0, 0, 0
    | _ -> "a", "a"
                   """
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 1, Line 5, Col 12, Line 5, Col 20,
                 "All branches of a pattern match expression must return values implicitly convertible to the type of the first branch, which here is a tuple of length 3 of type\n    int * int * int    \nThis branch returns a tuple of length 2 of type\n    string * string    \n")
            ]

        [<Fact>]
        let ``If context`` () =
            FSharp """
let y : bool * int * int =
    if true then "A", "B"
    else "B", "C"
                   """
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 1, Line 3, Col 18, Line 3, Col 26,
                 "The 'if' expression needs to return a tuple of length 3 of type\n    bool * int * int    \nto satisfy context type requirements. It currently returns a tuple of length 2 of type\n    string * string    \n")
                (Error 1, Line 4, Col 10, Line 4, Col 18,
                "All branches of an 'if' expression must return values implicitly convertible to the type of the first branch, which here is a tuple of length 3 of type\n    bool * int * int    \nThis branch returns a tuple of length 2 of type\n    string * string    \n")
            ]

        [<Fact>]
        let ``Array context`` () =
            FSharp """
let f x y = [| 1, 2; x, "a", y |]
                   """
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 1, Line 2, Col 22, Line 2, Col 31,
                 "All elements of an array must be implicitly convertible to the type of the first element, which here is a tuple of length 2 of type\n    int * int    \nThis element is a tuple of length 3 of type\n    'a * string * 'b    \n")
            ]

        [<Fact>]
        let ``List context`` () =
            FSharp """
let f x y = [ 1, 2; x, "a", y ]
                   """
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 1, Line 2, Col 21, Line 2, Col 30,
                 "All elements of a list must be implicitly convertible to the type of the first element, which here is a tuple of length 2 of type\n    int * int    \nThis element is a tuple of length 3 of type\n    'a * string * 'b    \n")
            ]

    [<Fact>]
    let ``return Instead Of return!``() =
        FSharp """
let rec foo() = async { return foo() }
        """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 2, Col 32, Line 2, Col 37,
                                 "Type mismatch. Expecting a\n    ''a'    \nbut given a\n    'Async<'a>'    \nThe types ''a' and 'Async<'a>' cannot be unified. Consider using 'return!' instead of 'return'.")

    [<Fact>]
    let ``yield Instead Of yield!``() =
        FSharp """
type Foo() =
  member this.Yield(x) = [x]

let rec f () = Foo() { yield f ()}
        """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 5, Col 30, Line 5, Col 34,
                                 "Type mismatch. Expecting a\n    ''a'    \nbut given a\n    ''a list'    \nThe types ''a' and ''a list' cannot be unified. Consider using 'yield!' instead of 'yield'.")

    [<Fact>]
    let ``Ref Cell Instead Of Not``() =
        FSharp """
let x = true
if !x then
    printfn "hello"
        """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 3, Col 5, Line 3, Col 6,
                                 ("This expression was expected to have type\n    'bool ref'    \nbut here has type\n    'bool'    " + System.Environment.NewLine + "The '!' operator is used to dereference a ref cell. Consider using 'not expr' here."))

    [<Fact>]
    let ``Ref Cell Instead Of Not 2``() =
        FSharp """
let x = true
let y = !x
        """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 3, Col 10, Line 3, Col 11,
                                 ("This expression was expected to have type\n    ''a ref'    \nbut here has type\n    'bool'    " + System.Environment.NewLine + "The '!' operator is used to dereference a ref cell. Consider using 'not expr' here."))

    [<Fact>]
    let ``Guard Has Wrong Type``() =
        FSharp """
let x = 1
match x with
| 1 when "s" -> true
| _ -> false
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error   1,  Line 4, Col 10, Line 4, Col 13, "A pattern match guard must be of type 'bool', but this 'when' expression is of type 'string'.")
            (Warning 20, Line 3, Col 1,  Line 5, Col 13, "The result of this expression has type 'bool' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.")]

    [<Fact>]
    let ``Runtime Type Test In Pattern``() =
        FSharp """
open System.Collections.Generic

let orig = Dictionary<obj,obj>()

let c =
  match orig with
  | :? IDictionary<obj,obj> -> "yes"
  | _ -> "no"
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 67,  Line 8, Col 5, Line 8, Col 28, "This type test or downcast will always hold")
            (Error   193, Line 8, Col 5, Line 8, Col 28, "Type constraint mismatch. The type \n    'IDictionary<obj,obj>'    \nis not compatible with type\n    'Dictionary<obj,obj>'    \n")]

    [<Fact>]
    let ``Runtime Type Test In Pattern 2``() =
        FSharp """
open System.Collections.Generic

let orig = Dictionary<obj,obj>()

let c =
  match orig with
  | :? IDictionary<obj,obj> as y -> "yes" + y.ToString()
  | _ -> "no"
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 67,  Line 8, Col 5, Line 8, Col 28, "This type test or downcast will always hold")
            (Error   193, Line 8, Col 5, Line 8, Col 28, "Type constraint mismatch. The type \n    'IDictionary<obj,obj>'    \nis not compatible with type\n    'Dictionary<obj,obj>'    \n")]

    [<Fact>]
    let ``Override Errors``() =
        FSharp """
type Base() =
    abstract member Member: int * string -> string
    default x.Member (i, s) = s

type Derived1() =
    inherit Base()
    override x.Member() = 5

type Derived2() =
    inherit Base()
    override x.Member (i : int) = "Hello"

type Derived3() =
    inherit Base()
    override x.Member (s : string, i : int) = sprintf "Hello %s" s
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
                (Error 856, Line 8,  Col 16, Line 8,  Col 22, "This override takes a different number of arguments to the corresponding abstract member. The following abstract members were found:" + System.Environment.NewLine + "   abstract Base.Member: int * string -> string")
                (Error 856, Line 12, Col 16, Line 12, Col 22, "This override takes a different number of arguments to the corresponding abstract member. The following abstract members were found:" + System.Environment.NewLine + "   abstract Base.Member: int * string -> string")
                (Error 1,   Line 16, Col 24, Line 16, Col 34, "This expression was expected to have type\n    'int'    \nbut here has type\n    'string'    ")]

    [<Fact>]
    let ``Interface member with tuple argument should give error message with better solution``() =
        FSharp """
type IFoo = 
  abstract member Bar: (int * int) -> int
  
type Foo =
  interface IFoo with
    member _.Bar (x, y) = x + y
"""
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 3577, Line 7, Col 14, Line 7, Col 17,
                                 """This override takes a tuple instead of multiple arguments. Try to add an additional layer of parentheses at the method definition (e.g. 'member _.Foo((x, y))'), or remove parentheses at the abstract method declaration (e.g. 'abstract member Foo: 'a * 'b -> 'c').""")

    [<Fact>]
    let ``Elements in computed lists, arrays and sequences``() =
        FSharp """
let f1 =
    [|
        if true then
            1
        "wrong" 
    |]

let f2: int list =
    [
        if true then
            "a"
        yield! [ 3; 4 ] 
    ]

let f3 =
    [
        if true then
            "a"
            "b"
        yield! [ 3; 4 ] 
    ]

let f4 =
    seq {
        1L
        let _ = ()
        2.5
        3L
    }
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 193,   Line 6,  Col 9,  Line 6,  Col 16, "Type constraint mismatch. The type \n    'string'    \nis not compatible with type\n    'int'    \n")
            (Error 193,   Line 12, Col 13, Line 12, Col 16, "Type constraint mismatch. The type \n    'string'    \nis not compatible with type\n    'int'    \n")
            (Error 193,   Line 21, Col 9,  Line 21, Col 24, "Type constraint mismatch. The type \n    'int list'    \nis not compatible with type\n    'string seq'    \n")
            (Error 193,   Line 28, Col 9,  Line 28, Col 12, "Type constraint mismatch. The type \n    'float'    \nis not compatible with type\n    'int64'    \n")
        ]

