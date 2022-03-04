// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.ErrorMessages

open Xunit
open FSharp.Test.Compiler

module ``Classes`` =

    [<Fact>]
    let ``Tuple In Abstract Method``() =
        FSharp """
type IInterface =
    abstract Function : (int32 * int32) -> unit

let x =
  { new IInterface with
        member this.Function (i, j) = ()
  }
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 768, Line 7, Col 16, Line 7, Col 36, "The member 'Function' does not accept the correct number of arguments. 1 argument(s) are expected, but 2 were given. The required signature is 'member IInterface.Function : (int32 * int32) -> unit'.\nA tuple type is required for one or more arguments. Consider wrapping the given arguments in additional parentheses or review the definition of the interface.")
            (Error 17,  Line 7, Col 21, Line 7, Col 29, "The member 'Function : 'a * 'b -> unit' does not have the correct type to override the corresponding abstract method. The required signature is 'Function : (int32 * int32) -> unit'.")
            (Error 783, Line 6, Col 9,  Line 6, Col 19, "At least one override did not correctly implement its corresponding abstract member")]

    [<Fact>]
    let ``Wrong Arity``() =
        FSharp """
type MyType() =
   static member MyMember(arg1, arg2:int ) = ()
   static member MyMember(arg1, arg2:byte) = ()


MyType.MyMember("", 0, 0)
        """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 503, Line 7, Col 1, Line 7, Col 26,
                                 "A member or object constructor 'MyMember' taking 3 arguments is not accessible from this code location. All accessible versions of method 'MyMember' take 2 arguments.")

    [<Fact>]
    let ``Method Is Not Static``() =
        FSharp """
type Class1() =
    member this.X() = "F#"

let x = Class1.X()
        """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 3214, Line 5, Col 9, Line 5, Col 17, "Method or object constructor 'X' is not static")

    [<Fact>]
    let ``Matching Method With Same Name Is Not Abstract``() =
        FSharp """
type Foo(x : int) =
  member v.MyX() = x

let foo =
    { new Foo(3)
        with
        member v.MyX() = 4 }
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 767, Line 8, Col 16, Line 8, Col 23, "The type Foo contains the member 'MyX' but it is not a virtual or abstract method that is available to override or implement.")
            (Error 17,  Line 8, Col 18, Line 8, Col 21, "The member 'MyX : unit -> int' does not have the correct type to override any given virtual method")
            (Error 783, Line 6, Col 11, Line 6, Col 14, "At least one override did not correctly implement its corresponding abstract member")]

    [<Fact>]
    let ``No Matching Abstract Method With Same Name``() =
        FSharp """
type IInterface =
    abstract MyFunction : int32 * int32 -> unit
    abstract SomeOtherFunction : int32 * int32 -> unit

let x =
  { new IInterface with
      member this.Function (i, j) = ()
  }
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 767, Line 8, Col 14, Line 8, Col 34, "The member 'Function' does not correspond to any abstract or virtual method available to override or implement. Maybe you want one of the following:" + System.Environment.NewLine + "   MyFunction")
            (Error 17,  Line 8, Col 19, Line 8, Col 27, "The member 'Function : 'a * 'b -> unit' does not have the correct type to override any given virtual method")
            (Error 366, Line 7, Col 3,  Line 9, Col 4,  "No implementation was given for those members: " + System.Environment.NewLine + "\t'abstract member IInterface.MyFunction : int32 * int32 -> unit'" + System.Environment.NewLine + "\t'abstract member IInterface.SomeOtherFunction : int32 * int32 -> unit'" + System.Environment.NewLine + "Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
            (Error 783, Line 7, Col 9,  Line 7, Col 19, "At least one override did not correctly implement its corresponding abstract member")]

    [<Fact>]
    let ``Member Has Multiple Possible Dispatch Slots``() =
        FSharp """
type IOverload =
    abstract member Bar : int -> int
    abstract member Bar : double -> int

type Overload =
    interface IOverload with
        override __.Bar _ = 1
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 366,  Line 7, Col 15, Line 7, Col 24, "No implementation was given for those members: " + System.Environment.NewLine + "\t'abstract member IOverload.Bar : double -> int'" + System.Environment.NewLine + "\t'abstract member IOverload.Bar : int -> int'" + System.Environment.NewLine + "Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
            (Error 3213, Line 8, Col 21, Line 8, Col 24, "The member 'Bar<'a0> : 'a0 -> int' matches multiple overloads of the same method.\nPlease restrict it to one of the following:" + System.Environment.NewLine + "   Bar : double -> int" + System.Environment.NewLine + "   Bar : int -> int.")]

    [<Fact>]
    let ``Do Cannot Have Visibility Declarations``() =
        FSharp """
type X() =
    do ()
    private do ()
    static member Y() = 1
        """
        |> parse
        |> shouldFail
        |> withDiagnostics [
            (Error 531, Line 4, Col 5,  Line 4, Col 12, "Accessibility modifiers should come immediately prior to the identifier naming a construct")
            (Error 512, Line 4, Col 13, Line 4, Col 18, "Accessibility modifiers are not permitted on 'do' bindings, but 'Private' was given.")
            (Error 222, Line 2, Col 1,  Line 3, Col 1,  "Files in libraries or multiple-file applications must begin with a namespace or module declaration, e.g. 'namespace SomeNamespace.SubNamespace' or 'module SomeNamespace.SomeModule'. Only the last source file of an application may omit such a declaration.")]
