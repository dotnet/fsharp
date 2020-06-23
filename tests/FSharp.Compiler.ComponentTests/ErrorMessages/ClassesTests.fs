// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ErrorMessages.ComponentTests

open Xunit
open FSharp.Test.Utilities
open FSharp.Compiler.SourceCodeServices

module ``Classes`` =

    [<Fact>]
    let ``Tuple In Abstract Method``() =
        CompilerAssert.TypeCheckWithErrors
            """
type IInterface =
    abstract Function : (int32 * int32) -> unit

let x =
  { new IInterface with
        member this.Function (i, j) = ()
  }
            """
            [|
                FSharpErrorSeverity.Error, 768, (7, 16, 7, 36), "The member 'Function' does not accept the correct number of arguments. 1 argument(s) are expected, but 2 were given. The required signature is 'member IInterface.Function : (int32 * int32) -> unit'.\nA tuple type is required for one or more arguments. Consider wrapping the given arguments in additional parentheses or review the definition of the interface."
                FSharpErrorSeverity.Error, 17, (7, 21, 7, 29), "The member 'Function : 'a * 'b -> unit' does not have the correct type to override the corresponding abstract method. The required signature is 'Function : (int32 * int32) -> unit'."
                FSharpErrorSeverity.Error, 783, (6, 9, 6, 19), "At least one override did not correctly implement its corresponding abstract member"
            |]

    [<Fact>]
    let ``Wrong Arity``() =
        CompilerAssert.TypeCheckSingleError
            """
type MyType() =
   static member MyMember(arg1, arg2:int ) = ()
   static member MyMember(arg1, arg2:byte) = ()


MyType.MyMember("", 0, 0)
            """
            FSharpErrorSeverity.Error
            503
            (7, 1, 7, 26)
            "A member or object constructor 'MyMember' taking 3 arguments is not accessible from this code location. All accessible versions of method 'MyMember' take 2 arguments."

    [<Fact>]
    let ``Method Is Not Static``() =
        CompilerAssert.TypeCheckSingleError
            """
type Class1() =
    member this.X() = "F#"

let x = Class1.X()
            """
            FSharpErrorSeverity.Error
            3214
            (5, 9, 5, 17)
            "Method or object constructor 'X' is not static"

    [<Fact>]
    let ``Matching Method With Same Name Is Not Abstract``() =
        CompilerAssert.TypeCheckWithErrors
            """
type Foo(x : int) =
  member v.MyX() = x

let foo =
    { new Foo(3)
        with
        member v.MyX() = 4 }
            """
            [|
                FSharpErrorSeverity.Error, 767, (8, 16, 8, 23), "The type Foo contains the member 'MyX' but it is not a virtual or abstract method that is available to override or implement."
                FSharpErrorSeverity.Error, 17, (8, 18, 8, 21), "The member 'MyX : unit -> int' does not have the correct type to override any given virtual method"
                FSharpErrorSeverity.Error, 783, (6, 11, 6, 14), "At least one override did not correctly implement its corresponding abstract member"
            |]

    [<Fact>]
    let ``No Matching Abstract Method With Same Name``() =
        CompilerAssert.TypeCheckWithErrors
            """
type IInterface =
    abstract MyFunction : int32 * int32 -> unit
    abstract SomeOtherFunction : int32 * int32 -> unit

let x =
  { new IInterface with
      member this.Function (i, j) = ()
  }
            """
            [|
                FSharpErrorSeverity.Error, 767, (8, 14, 8, 34), "The member 'Function' does not correspond to any abstract or virtual method available to override or implement. Maybe you want one of the following:" + System.Environment.NewLine + "   MyFunction"
                FSharpErrorSeverity.Error, 17, (8, 19, 8, 27), "The member 'Function : 'a * 'b -> unit' does not have the correct type to override any given virtual method"
                FSharpErrorSeverity.Error, 366, (7, 3, 9, 4), "No implementation was given for those members: " + System.Environment.NewLine + "\t'abstract member IInterface.MyFunction : int32 * int32 -> unit'" + System.Environment.NewLine + "\t'abstract member IInterface.SomeOtherFunction : int32 * int32 -> unit'" + System.Environment.NewLine + "Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'."
                FSharpErrorSeverity.Error, 783, (7, 9, 7, 19), "At least one override did not correctly implement its corresponding abstract member"
            |]

    [<Fact>]
    let ``Member Has Multiple Possible Dispatch Slots``() =
        CompilerAssert.TypeCheckWithErrors
            """
type IOverload =
    abstract member Bar : int -> int
    abstract member Bar : double -> int

type Overload =
    interface IOverload with
        override __.Bar _ = 1
            """
            [|
                FSharpErrorSeverity.Error, 366, (7, 15, 7, 24), "No implementation was given for those members: " + System.Environment.NewLine + "\t'abstract member IOverload.Bar : double -> int'" + System.Environment.NewLine + "\t'abstract member IOverload.Bar : int -> int'" + System.Environment.NewLine + "Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'."
                FSharpErrorSeverity.Error, 3213, (8, 21, 8, 24), "The member 'Bar<'a0> : 'a0 -> int' matches multiple overloads of the same method.\nPlease restrict it to one of the following:" + System.Environment.NewLine + "   Bar : double -> int" + System.Environment.NewLine + "   Bar : int -> int."
            |]

    [<Fact>]
    let ``Do Cannot Have Visibility Declarations``() =
        CompilerAssert.ParseWithErrors
            """
type X() =
    do ()
    private do ()
    static member Y() = 1
            """
            [|
                FSharpErrorSeverity.Error, 531, (4, 5, 4, 12), "Accessibility modifiers should come immediately prior to the identifier naming a construct"
                FSharpErrorSeverity.Error, 512, (4, 13, 4, 18), "Accessibility modifiers are not permitted on 'do' bindings, but 'Private' was given."
                FSharpErrorSeverity.Error, 222, (2, 1, 3, 1), "Files in libraries or multiple-file applications must begin with a namespace or module declaration, e.g. 'namespace SomeNamespace.SubNamespace' or 'module SomeNamespace.SomeModule'. Only the last source file of an application may omit such a declaration."
            |]
