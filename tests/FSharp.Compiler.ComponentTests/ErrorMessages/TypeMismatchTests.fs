// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ErrorMessages.ComponentTests

open Xunit
open FSharp.Test.Utilities
open FSharp.Compiler.SourceCodeServices

module ``Type Mismatch`` =

    [<Fact>]
    let ``return Instead Of return!``() =
        CompilerAssert.TypeCheckSingleError
            """
let rec foo() = async { return foo() }
            """
            FSharpErrorSeverity.Error
            1
            (2, 32, 2, 37)
            "Type mismatch. Expecting a\n    ''a'    \nbut given a\n    'Async<'a>'    \nThe types ''a' and 'Async<'a>' cannot be unified. Consider using 'return!' instead of 'return'."

    [<Fact>]
    let ``yield Instead Of yield!``() =
        CompilerAssert.TypeCheckSingleError
            """
type Foo() =
  member this.Yield(x) = [x]

let rec f () = Foo() { yield f ()}
            """
            FSharpErrorSeverity.Error
            1
            (5, 30, 5, 34)
            "Type mismatch. Expecting a\n    ''a'    \nbut given a\n    ''a list'    \nThe types ''a' and ''a list' cannot be unified. Consider using 'yield!' instead of 'yield'."

    [<Fact>]
    let ``Ref Cell Instead Of Not``() =
        CompilerAssert.TypeCheckSingleError
            """
let x = true
if !x then
    printfn "hello"
            """
            FSharpErrorSeverity.Error
            1
            (3, 5, 3, 6)
            ("This expression was expected to have type\n    'bool ref'    \nbut here has type\n    'bool'    " + System.Environment.NewLine + "The '!' operator is used to dereference a ref cell. Consider using 'not expr' here.")

    [<Fact>]
    let ``Ref Cell Instead Of Not 2``() =
        CompilerAssert.TypeCheckSingleError
            """
let x = true
let y = !x
            """
            FSharpErrorSeverity.Error
            1
            (3, 10, 3, 11)
            ("This expression was expected to have type\n    ''a ref'    \nbut here has type\n    'bool'    " + System.Environment.NewLine + "The '!' operator is used to dereference a ref cell. Consider using 'not expr' here.")

    [<Fact>]
    let ``Guard Has Wrong Type``() =
        CompilerAssert.TypeCheckWithErrors
            """
let x = 1
match x with
| 1 when "s" -> true
| _ -> false
            """
            [|
                FSharpErrorSeverity.Error, 1, (4, 10, 4, 13), "A pattern match guard must be of type 'bool', but this 'when' expression is of type 'string'."
                FSharpErrorSeverity.Warning, 20, (3, 1, 5, 13), "The result of this expression has type 'bool' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'."
            |]

    [<Fact>]
    let ``Runtime Type Test In Pattern``() =
        CompilerAssert.TypeCheckWithErrors
            """
open System.Collections.Generic

let orig = Dictionary<obj,obj>()

let c =
  match orig with
  | :? IDictionary<obj,obj> -> "yes"
  | _ -> "no"
            """
            [|
                FSharpErrorSeverity.Warning, 67, (8, 5, 8, 28), "This type test or downcast will always hold"
                FSharpErrorSeverity.Error, 193, (8, 5, 8, 28), "Type constraint mismatch. The type \n    'IDictionary<obj,obj>'    \nis not compatible with type\n    'Dictionary<obj,obj>'    \n"
            |]

    [<Fact>]
    let ``Runtime Type Test In Pattern 2``() =
        CompilerAssert.TypeCheckWithErrors
            """
open System.Collections.Generic

let orig = Dictionary<obj,obj>()

let c =
  match orig with
  | :? IDictionary<obj,obj> as y -> "yes" + y.ToString()
  | _ -> "no"
            """
            [|
                FSharpErrorSeverity.Warning, 67, (8, 5, 8, 28), "This type test or downcast will always hold"
                FSharpErrorSeverity.Error, 193, (8, 5, 8, 28), "Type constraint mismatch. The type \n    'IDictionary<obj,obj>'    \nis not compatible with type\n    'Dictionary<obj,obj>'    \n"
            |]

    [<Fact>]
    let ``Override Errors``() =
        CompilerAssert.TypeCheckWithErrors
            """
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
            [|
                FSharpErrorSeverity.Error, 856, (8, 16, 8, 22), "This override takes a different number of arguments to the corresponding abstract member. The following abstract members were found:" + System.Environment.NewLine + "   abstract member Base.Member : int * string -> string"
                FSharpErrorSeverity.Error, 856, (12, 16, 12, 22), "This override takes a different number of arguments to the corresponding abstract member. The following abstract members were found:" + System.Environment.NewLine + "   abstract member Base.Member : int * string -> string"
                FSharpErrorSeverity.Error, 1, (16, 24, 16, 34), "This expression was expected to have type\n    'int'    \nbut here has type\n    'string'    "
            |]
