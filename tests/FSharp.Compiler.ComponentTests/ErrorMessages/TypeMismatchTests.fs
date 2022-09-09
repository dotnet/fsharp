// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.ErrorMessages

open Xunit
open FSharp.Test.Compiler

module ``Type Mismatch`` =

    module ``Different tuple lengths`` =
        
        [<Fact>]
        let ``Known type on the left``() =
            FSharp """
let x: int * int * int = 1, ""
let x: int * string * int = "", 1
let x: int * int = "", "", 1
            """
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 1, Line 2, Col 26, Line 2, Col 31,
                 "Type mismatch. Expecting a\n    'tuple of length 3 (int * int * int)'    \nbut given a\n    'tuple of length 2'    \n")
                (Error 1, Line 3, Col 29, Line 3, Col 34,
                 "Type mismatch. Expecting a\n    'tuple of length 3 (int * string * int)'    \nbut given a\n    'tuple of length 2'    \n")
                (Error 1, Line 4, Col 20, Line 4, Col 29,
                 "Type mismatch. Expecting a\n    'tuple of length 2 (int * int)'    \nbut given a\n    'tuple of length 3'    \n")
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
                 "Type mismatch. Expecting a\n    'tuple of length 3'    \nbut given a\n    'tuple of length 2 (int * string)'    \n")
            ]
            
        // TODO
        let ``Else branch context``() =
            FSharp """
let f1(a, b, c) =
    if true then (1, 2) else (a, b, c)
            """
            |> typecheck
            |> shouldFail
            |> withDiagnostics [
                (Error 1, Line 3, Col 30, Line 3, Col 39,
                 "All branches of an 'if' expression must return values implicitly convertible to the type of the first branch, which here is 'tuple of length 2 (int * int)'. This branch returns a value of type 'tuple of length 3'.")
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
