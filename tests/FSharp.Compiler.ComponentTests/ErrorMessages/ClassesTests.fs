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
            (Error 768, Line 7, Col 16, Line 7, Col 36, "The member 'Function' does not accept the correct number of arguments. 1 argument(s) are expected, but 2 were given. The required signature is 'IInterface.Function: (int32 * int32) -> unit'.\nA tuple type is required for one or more arguments. Consider wrapping the given arguments in additional parentheses or review the definition of the interface.")
            (Error 17,  Line 7, Col 21, Line 7, Col 29, "The member 'Function: 'a * 'b -> unit' does not have the correct type to override the corresponding abstract method. The required signature is 'Function: (int32 * int32) -> unit'.")
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
            (Error 17,  Line 8, Col 18, Line 8, Col 21, "The member 'MyX: unit -> int' does not have the correct type to override any given virtual method")
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
            (Error 17,  Line 8, Col 19, Line 8, Col 27, "The member 'Function: 'a * 'b -> unit' does not have the correct type to override any given virtual method")
            (Error 366, Line 7, Col 3,  Line 9, Col 4,  "No implementation was given for those members: " + System.Environment.NewLine + "\t'abstract IInterface.MyFunction: int32 * int32 -> unit'" + System.Environment.NewLine + "\t'abstract IInterface.SomeOtherFunction: int32 * int32 -> unit'" + System.Environment.NewLine + "Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
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
            (Error 366,  Line 7, Col 15, Line 7, Col 24, "No implementation was given for those members: " + System.Environment.NewLine + "\t'abstract IOverload.Bar: double -> int'" + System.Environment.NewLine + "\t'abstract IOverload.Bar: int -> int'" + System.Environment.NewLine + "Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
            (Error 3213, Line 8, Col 21, Line 8, Col 24, "The member 'Bar<'a0> : 'a0 -> int' matches multiple overloads of the same method.\nPlease restrict it to one of the following:" + System.Environment.NewLine + "   Bar: double -> int" + System.Environment.NewLine + "   Bar: int -> int.")]

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
            (Error 222, Line 2, Col 1,  Line 3, Col 1,  "Files in libraries or multiple-file applications must begin with a namespace or module declaration, e.g. 'namespace SomeNamespace.SubNamespace' or 'module SomeNamespace.SomeModule'. Only the last source file of an application may omit such a declaration.")
        ]
        
    [<Fact>]
    let ``No abstract or interface member was found that corresponds to this override with lang preview``() =
        Fsx """
type A =
    abstract member M1: unit -> unit
    abstract member M2: unit -> unit 
    abstract member M3: unit -> unit 
    abstract member M4: unit -> unit
    
type B() =
    interface A with
        override this.M1() = ()
        override this.M2() = () // error is expected
        override this.M3() = () // error is expected
        override this.M4() = ()
    
type C() =
    inherit B()
    override this.M1() = ()
    override this.M2() = ()
    override this.M3() = ()
    override this.M4() = ()
    member this.M5() = ()
        """
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 855, Line 17, Col 19, Line 17, Col 21, "No abstract or interface member was found that corresponds to this override")
            (Error 855, Line 18, Col 19, Line 18, Col 21, "No abstract or interface member was found that corresponds to this override")
            (Error 855, Line 19, Col 19, Line 19, Col 21, "No abstract or interface member was found that corresponds to this override")
            (Error 855, Line 20, Col 19, Line 20, Col 21, "No abstract or interface member was found that corresponds to this override")
        ]
    
    [<Fact>]
    let ``No abstract or interface member was found that corresponds to this override with lang version70``() =
        Fsx """
type A =
    abstract member M1: unit -> unit
    abstract member M2: unit -> unit 
    abstract member M3: unit -> unit 
    abstract member M4: unit -> unit
    
type B() =
    interface A with
        override this.M1() = ()
        override this.M2() = () // error is expected
        override this.M3() = () // error is expected
        override this.M4() = ()
    
type C() =
    inherit B()
    override this.M1() = ()
    override this.M2() = ()
    override this.M3() = ()
    override this.M4() = ()
    member this.M5() = ()
        """
        |> withLangVersion70
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 855, Line 17, Col 19, Line 17, Col 21, "No abstract or interface member was found that corresponds to this override")
            (Error 855, Line 18, Col 19, Line 18, Col 21, "No abstract or interface member was found that corresponds to this override")
            (Error 855, Line 19, Col 19, Line 19, Col 21, "No abstract or interface member was found that corresponds to this override")
            (Error 855, Line 20, Col 19, Line 20, Col 21, "No abstract or interface member was found that corresponds to this override")
        ]
        
    [<Fact>]
    let ``Virtual member was found that corresponds to this override with lang version70`` () =
        let CSLib =
            CSharp """
public class A
{
    public virtual void M1() { }
    public virtual void M2() { }
    public virtual void M3() { }
    public virtual void M4() { }
}
        """ |> withName "CSLib"

        let app =
            FSharp """
module ClassTests
type B() =
    inherit A()
    override this.M1() = ()
    override this.M2() = ()
    override this.M3() = ()
    override this.M4() = ()
    member this.M5() = ()
        """ |> withReferences [CSLib]
        app
        |> withLangVersion70
        |> compile
        |> shouldSucceed
        
    [<Fact>]
    let ``Virtual member was found that corresponds to this override with lang preview`` () =
        let CSLib =
            CSharp """
public class A
{
    public virtual void M1() { }
    public virtual void M2() { }
    public virtual void M3() { }
    public virtual void M4() { }
}
        """ |> withName "CSLib"

        let app =
            FSharp """
module ClassTests
type B() =
    inherit A()
    override this.M1() = ()
    override this.M2() = ()
    override this.M3() = ()
    override this.M4() = ()
    member this.M5() = ()
        """ |> withReferences [CSLib]
        app
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed


    [<Fact>]
    let ``Virtual member was not found that corresponds to this override simple base class with lang version preview`` () =
        let CSLib =
            CSharp """
public class A
{
    public virtual void M1() { }
    public void M2() { }
    public virtual void M3() { }
}
        """ |> withName "CSLib"

        let app =
            FSharp """
module ClassTests
type B() =
    inherit A()
    
    override this.M1() = ()
    override this.M2() = () // error expected
    override this.M3() = ()
    member this.M4() = ()
        """ |> withReferences [CSLib]
        app
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 855, Line 7, Col 19, Line 7, Col 21, "No abstract or interface member was found that corresponds to this override")
        ]

    [<Fact>]
    let ``Virtual member was not found that corresponds to this override simple base class with lang version70`` () =
        let CSLib =
            CSharp """
public class A
{
    public virtual void M1() { }
    public void M2() { }
    public virtual void M3() { }
}
        """ |> withName "CSLib"

        let app =
            FSharp """
module ClassTests
type B() =
    inherit A()
    
    override this.M1() = ()
    override this.M2() = ()
    override this.M3() = ()
    member this.M4() = ()
        """ |> withReferences [CSLib]
        app
        |> withLangVersion70
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 855, Line 7, Col 19, Line 7, Col 21, "No abstract or interface member was found that corresponds to this override")
        ]
        
    [<Fact>]
    let ``Virtual member was not found that corresponds to this override nested base class with lang version preview`` () =
        let CSLib =
            CSharp """
public class A
{
    public virtual void M1() { }
    public virtual void M2() { }
    public virtual void M3() { }
    public virtual void M4() { }
}

public class B : A
{
    public override void M1() { }
    public void M2() { }
    public new void M3() { }
    public new virtual void M4() { }
}
        """ |> withName "CSLib"

        let app =
            FSharp """
module ClassTests
type C() =
    inherit B()
    
    override this.M1() = ()
    override this.M2() = () // error expected
    override this.M3() = () // error expected
    override this.M4() = ()
    member this.M5() = ()
        """ |> withReferences [CSLib]
        app
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 855, Line 7, Col 19, Line 7, Col 21, "No abstract or interface member was found that corresponds to this override")
            (Error 855, Line 8, Col 19, Line 8, Col 21, "No abstract or interface member was found that corresponds to this override")
        ]
        
    [<Fact>]
    let ``Virtual member was not found that corresponds to this override nested base class with lang version70`` () =
        let CSLib =
            CSharp """
public class A
{
    public virtual void M1() { }
    public virtual void M2() { }
    public virtual void M3() { }
    public virtual void M4() { }
}

public class B : A
{
    public override void M1() { }
    public void M2() { }
    public new void M3() { }
    public new virtual void M4() { }
}
        """ |> withName "CSLib"

        let app =
            FSharp """
module ClassTests
type C() =
    inherit B()
    
    override this.M1() = ()
    override this.M2() = () // error expected
    override this.M3() = () // error expected
    override this.M4() = ()
    member this.M5() = ()
        """ |> withReferences [CSLib]
        app
        |> withLangVersion70
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Virtual member was not found that corresponds to this override nested 2 base class with lang preview`` () =
        let CSLib =
            CSharp """
public class A
{
    public virtual void M1() { }
    public virtual void M2() { }
    public virtual void M3() { }
    public virtual void M4() { }
}

public class B : A
{
    public void M2() { }
    public new void M3() { }
}
        """ |> withName "CSLib"

        let app =
            FSharp """
module ClassTests
type C() =
    inherit B()
    
    override this.M1() = ()
    override this.M2() = () // error is expected
    override this.M3() = () // error is expected
    override this.M4() = ()
    member this.M5() = ()
        """ |> withReferences [CSLib]
        app
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 855, Line 7, Col 19, Line 7, Col 21, "No abstract or interface member was found that corresponds to this override")
            (Error 855, Line 8, Col 19, Line 8, Col 21, "No abstract or interface member was found that corresponds to this override")
        ]
        
    [<Fact>]
    let ``Virtual member was not found that corresponds to this override nested 2 base classes with lang version70`` () =
        let CSLib =
            CSharp """
public class A
{
    public virtual void M1() { }
    public virtual void M2() { }
    public virtual void M3() { }
    public virtual void M4() { }
}

public class B : A
{
    public void M2() { }
    public new void M3() { }
}
        """ |> withName "CSLib"

        let app =
            FSharp """
module ClassTests
type C() =
    inherit B()
    
    override this.M1() = ()
    override this.M2() = () // error is expected
    override this.M3() = () // error is expected
    override this.M4() = ()
    member this.M5() = ()
        """ |> withReferences [CSLib]
        app
        |> withLangVersion70
        |> compile
        |> shouldSucceed
        
    [<Fact>]
    let ``Virtual member was not found that corresponds to this override nested 2 base classes and mixed methods with lang preview`` () =
        let CSLib =
            CSharp """
public class A
{
    public virtual void M1() { }
    public void M2() { }
}

public class B : A
{
    public virtual void M3() { }
    public new void M4() { }
}
        """ |> withName "CSLib"

        let app =
            FSharp """
module ClassTests
type C() =
    inherit B()
    
    override this.M1() = ()
    override this.M2() = () // error is expected
    override this.M3() = ()
    override this.M4() = () // error is expected
    member this.M5() = ()
        """ |> withReferences [CSLib]
        app
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 855, Line 7, Col 19, Line 7, Col 21, "No abstract or interface member was found that corresponds to this override")
            (Error 855, Line 9, Col 19, Line 9, Col 21, "No abstract or interface member was found that corresponds to this override")
        ]

    [<Fact>]
    let ``Virtual member was not found that corresponds to this override nested 2 base classes and mixed methods with lang version70`` () =
        let CSLib =
            CSharp """
public class A
{
    public virtual void M1() { }
    public void M2() { }
}

public class B : A
{
    public virtual void M3() { }
    public new void M4() { }
}
        """ |> withName "CSLib"

        let app =
            FSharp """
module ClassTests
type C() =
    inherit B()
    
    override this.M1() = ()
    override this.M2() = () // error is expected
    override this.M3() = ()
    override this.M4() = ()  // error is expected
    member this.M5() = ()
        """ |> withReferences [CSLib]
        app
        |> withLangVersion70
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 855, Line 7, Col 19, Line 7, Col 21, "No abstract or interface member was found that corresponds to this override")
            (Error 855, Line 9, Col 19, Line 9, Col 21, "No abstract or interface member was found that corresponds to this override")
        ]

    [<Fact>]
    let ``Virtual member was found among virtual and non-virtual overloads with lang preview`` () =
        let CSLib =
            CSharp """
public class A
{
    public void M1(int i) { }
    public virtual void M1(string s) { }
}
        """ |> withName "CSLib"

        let app =
            FSharp """
module ClassTests

type Over () =
    inherit A ()

    override _.M1 (s: string) = ()
        """
        
        app
        |> withReferences [CSLib]
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed