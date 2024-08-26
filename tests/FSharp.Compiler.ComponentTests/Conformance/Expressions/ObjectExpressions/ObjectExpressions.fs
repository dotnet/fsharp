// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ObjectExpressionsTests =

    [<Fact>]
    let ``Object expression cannot implement unnamed interface`` () =
        Fsx """
type IFirst =
    abstract member MyMember: unit -> int

let x = { new _ with member this.MyMember() = 42 }
        """
         |> withLangVersion90
         |> typecheck
         |> shouldFail
         |> withDiagnostics [
             (Error 772, Line 5, Col 11, Line 5, Col 16, "'new' must be used with a named type")
         ]
         
    [<Fact>]
    let ``Object expression can not implement a class class end`` () =
        Fsx """
type Class() = class end

let implementer = { new Class()  }
        """
         |> withLangVersion90
         |> typecheck
         |> shouldFail
         |> withDiagnostics [
             (Error 738, Line 4, Col 19, Line 4, Col 35, "Invalid object expression. Objects without overrides or interfaces should use the expression form 'new Type(args)' without braces.")
         ]
         
    [<Fact>]
    let ``Object expression can not implement a class without members`` () =
        Fsx """
type Class() =
    member this.Do() = ()

let implementer = { new Class()  }
        """
         |> withLangVersion90
         |> typecheck
         |> shouldFail
         |> withDiagnostics [
             (Error 738, Line 5, Col 19, Line 5, Col 35, "Invalid object expression. Objects without overrides or interfaces should use the expression form 'new Type(args)' without braces.")
         ]
  
    [<Fact>]
    let ``Object expression implementing an interface without members`` () =
        Fsx """
type IFirst = interface end

let implementer() ={ new IFirst  }
        """
         |> withLangVersion90
         |> typecheck
         |> shouldSucceed

    [<Fact>]
    let ``Object expression can construct an abstract class and also implement interfaces with and without abstract members.`` () =
        Fsx """
type IFirst = interface end

type ISecond =
    abstract member M : unit -> unit
    
[<AbstractClass>]
type MyClass() = class end

{ new MyClass() with
    member x.ToString() = "OK"
    
  interface IFirst
  
  interface ISecond with
      member this.M() = () } |> ignore
        """
         |> withLangVersion90
         |> typecheck
         |> shouldSucceed
         
    [<Fact>]
    let ``Object expression can construct an abstract class(missing with...) and also implement interfaces with and without abstract members.`` () =
        Fsx """
type IFirst = interface end

type ISecond =
    abstract member M : unit -> unit
    
[<AbstractClass>]
type MyClass() = class end

{ new MyClass() interface IFirst
  
  interface ISecond with
      member this.M() = () } |> ignore
        """
         |> withLangVersion90
         |> typecheck
         |> shouldSucceed
         
    [<Fact>]
    let ``Object expression can construct an abstract class(missing with... and interface in the next line) and also implement interfaces with and without abstract members.`` () =
        Fsx """
type IFirst = interface end

type ISecond =
    abstract member M : unit -> unit
    
[<AbstractClass>]
type MyClass() = class end

{ new MyClass()
    interface IFirst
  
  interface ISecond with
      member this.M() = () } |> ignore
        """
         |> withLangVersion90
         |> typecheck
         |> shouldSucceed
         
    [<Fact>]
    let ``Verifies that the object expression built type has the interface.`` () =
        Fsx """
type IFirst = interface end

type ISecond =
    abstract member M : unit -> unit
    
[<AbstractClass>]
type MyClass() =
    interface ISecond with
        member this.M() = printfn "It works"

let expr = { new MyClass() interface IFirst }
(expr:> ISecond).M()
        """
         |> withLangVersion90
         |> compileExeAndRun
         |> shouldSucceed
         |> withStdOutContainsAllInOrder [
           "It works"
        ]
         
    [<Fact>]
    let ``Parameterized object expression implementing an interface with members`` () =
        Fsx """
type AsString =
  abstract member asString: unit -> string

type SomeDu =
  | A of int
  | B of string
  | C of float

let implSomeDU someDu = 
  { new AsString with
      member self.asString () =
          match someDu with
          | A a -> string a
          | B b -> b
          | C c -> string c }
        """
         |> withLangVersion90
         |> typecheck
         |> shouldSucceed
         
    [<Fact>]
    let ``Object expression cannot implement an abstract class having no abstract members.`` () =
        Fsx """
[<AbstractClass>]
type Foo() = class end

let foo = { new Foo() }

let foo1 = new Foo()

// hacky workaround
let foo2 = { new Foo() with member __.ToString() = base.ToString() }
        """
         |> withLangVersion90
         |> typecheck
         |> shouldFail
         |> withDiagnostics [
                (Error 738, Line 5, Col 11, Line 5, Col 24, "Invalid object expression. Objects without overrides or interfaces should use the expression form 'new Type(args)' without braces.");
                (Error 759, Line 7, Col 12, Line 7, Col 21, "Instances of this type cannot be created since it has been marked abstract or not all methods have been given implementations. Consider using an object expression '{ new ... with ... }' instead.")
         ]         
    
    [<Fact>]
    let ``Object expression can not implement an abstract class having no abstract members`` () =
        Fsx """
[<AbstractClass>]
type AbstractClass() =
    do printfn "AbstractClass constructor"
    
let res = { new AbstractClass() }
        """
         |> withLangVersion90
         |> typecheck
         |> shouldFail
         |> withDiagnostics [
             (Error 738, Line 6, Col 11, Line 6, Col 34, "Invalid object expression. Objects without overrides or interfaces should use the expression form 'new Type(args)' without braces.")
         ]
         
    [<Fact>]
    let ``Object expression can not implement an abstract class having abstract members with default implementation`` () =
        Fsx """
[<AbstractClass>]
type AbstractClass() =
    abstract member M : unit -> unit
    default this.M() = printfn "Im a default implementation"
    
let res = { new AbstractClass() }
        """
         |> withLangVersion90
         |> typecheck
         |> shouldFail
         |> withDiagnostics [
             (Error 738, Line 7, Col 11, Line 7, Col 34, "Invalid object expression. Objects without overrides or interfaces should use the expression form 'new Type(args)' without braces.")
         ]

    [<Fact>]
    let ``Object expression can implement an abstract class(overriding a member) having abstract members with default implementation`` () =
        Fsx """
[<AbstractClass>]
type AbstractClass() =
    abstract member M : unit -> unit
    default this.M() = printfn "Im a default implementation"
    
let res = { new AbstractClass() with
                override this.ToString() = "ConcreteMethod" }
        """
         |> withLangVersion90
         |> typecheck
         |> shouldSucceed
         
    [<Fact>]
    let ``Object expression can not implement an abstract class having abstract members`` () =
        Fsx """
[<AbstractClass>]
type AbstractClass() =
    do printfn "AbstractClass constructor"
    abstract member M : unit -> unit
    
let res = { new AbstractClass() }
        """
         |> withLangVersion90
         |> typecheck
         |> shouldFail
         |> withDiagnostics [
            (Error 365, Line 7, Col 11, Line 7, Col 34, "No implementation was given for 'abstract AbstractClass.M: unit -> unit'");
            (Error 738, Line 7, Col 11, Line 7, Col 34, "Invalid object expression. Objects without overrides or interfaces should use the expression form 'new Type(args)' without braces.")
         ]
      
    [<Fact>]
    let ``Object expression can implement an abstract class and interface having no abstract members.`` () =
        Fsx """
type IFirst = interface end

[<AbstractClass>]
type MyClass() = class end

{ new MyClass() with
    member x.ToString() = "OK"
    
  interface IFirst } |> ignore
        """
         |> withLangVersion90
         |> typecheck
         |> shouldSucceed

    [<Fact>]
    let ``Object expression shows error when object expression does not implement all abstract members of the abstract class`` () =
        Fsx """
type ISecond =
    abstract member M : unit -> unit
    
[<AbstractClass>]
type MyClass() =
    abstract member M : unit -> unit
    interface ISecond with
        member this.M() = printfn "It works"

let res = { new MyClass() }
        """
         |> withLangVersion90
         |> typecheck
         |> shouldFail
         |> withDiagnostics [
            (Error 365, Line 11, Col 11, Line 11, Col 28, "No implementation was given for 'abstract MyClass.M: unit -> unit'");
            (Error 738, Line 11, Col 11, Line 11, Col 28, "Invalid object expression. Objects without overrides or interfaces should use the expression form 'new Type(args)' without braces.")
         ] 
         
    [<Fact>]
    let ``C# abstract class with protected constructor can not be implemented by F# object expression`` () =

        let csharp =
            CSharp
                """
namespace CSLib
{
    using System;
    public abstract class Animal
    {
        protected Animal()
        {
            Console.WriteLine("Animal is created");
        }
    }
}
"""
            |> withName "CSLib"

        let fsharp =
            FSharp
                """
module FSLib
open CSLib

let res = { new Animal() }
"""
            |> withLangVersion90
            |> withName "FSLib"
            |> withReferences [ csharp ]

        fsharp
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 738, Line 5, Col 11, Line 5, Col 27, "Invalid object expression. Objects without overrides or interfaces should use the expression form 'new Type(args)' without braces.")
        ]
        
    [<Fact>]
    let ``C# abstract class with protected constructor and abstract method and default implementation can be implemented by F# object expression unless the abstract method is implemented.`` () =
        let csharp =
            CSharp
                """
namespace CSLib
{
    using System;
    public abstract class Animal
    {
        protected Animal()
        {
            Console.WriteLine("Animal is created");
        }
        
        public abstract void M();
    }
}
"""
            |> withName "CSLib"

        let fsharp =
            FSharp
                """
module FSLib
open CSLib

let res = { new Animal() }
"""
            |> withLangVersion90
            |> withName "FSLib"
            |> withReferences [ csharp ]

        fsharp
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 365, Line 5, Col 11, Line 5, Col 27, "No implementation was given for 'Animal.M() : unit'");
            (Error 738, Line 5, Col 11, Line 5, Col 27, "Invalid object expression. Objects without overrides or interfaces should use the expression form 'new Type(args)' without braces.")
        ]
        
    [<Fact>]
    let ``C# abstract class with protected constructor and abstract method can not be implemented by F# object expression unless the abstract method is implemented.`` () =
        let csharp =
            CSharp
                """
namespace CSLib
{
    using System;
    public abstract class Animal
    {
        protected Animal()
        {
            Console.WriteLine("Animal is created");
        }
        
        public abstract void M();
    }
}
"""
            |> withName "CSLib"

        let fsharp =
            FSharp
                """
module FSLib
open CSLib

let res = { new Animal() }
"""
            |> withLangVersion90
            |> withName "FSLib"
            |> withReferences [ csharp ]

        fsharp
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 365, Line 5, Col 11, Line 5, Col 27, "No implementation was given for 'Animal.M() : unit'");
            (Error 738, Line 5, Col 11, Line 5, Col 27, "Invalid object expression. Objects without overrides or interfaces should use the expression form 'new Type(args)' without braces.")
        ]

    [<Fact>]
    let ``Error when object expression does not implement all abstract members of the abstract class`` () =
        Fsx """
[<AbstractClass>]
type B() = 
    abstract M : int -> float
    abstract M : string -> unit
and [<AbstractClass>]
    C() = 
    inherit B()
    static let v = { new C() with 
                         member x.M(a:int) : float  = 1.0 }
    default x.M(a:int) : float  = 1.0
    
let y = { new C() with 
              member x.M(a:int) : float  = 1.0 }
        """
         |> withLangVersion90
         |> typecheck
         |> shouldFail
         |> withDiagnostics [
             (Error 365, Line 9, Col 20, Line 10, Col 60, "No implementation was given for 'abstract B.M: string -> unit'")
             (Error 365, Line 13, Col 9, Line 14, Col 49, "No implementation was given for 'abstract B.M: string -> unit'")
         ]

    [<Fact>]
    let ``Error when object expression does not implement all abstract members of a generic abstract class`` () =
        Fsx """
[<AbstractClass>]
type BaseHashtable<'Entry, 'Key>(initialCapacity) =
    abstract member Next : entries : 'Entry array -> int

[<Struct>]    
type StrongToWeakEntry<'Value when 'Value : not struct> =
    val mutable public next : int

let f() = { new BaseHashtable<_,_>(2) with
            override this.Next (entries:StrongToWeakEntry<_> array) = 1
            override this.Next entries = 1
          }
        """
         |> withLangVersion90
         |> typecheck
         |> shouldFail
         |> withDiagnostics [
             (Error 359, Line 10, Col 11, Line 13, Col 12, "More than one override implements 'Next: StrongToWeakEntry<'a> array -> int when 'a: not struct'")
         ]
         
    [<Fact>]
    let ``Object expression cannot implement an interface when it contains a method with no types that can refer to the type for which the implementation is being used`` () =
        Fsx """
type AsString =
    abstract member asString: unit -> string
    
type AsString2<'T> =
    abstract member asString: 'T -> string

type SomeDu =
    | A of int
    | B of string
    | C of float
    interface AsString
        with member self.asString () =
                match self with
                | A a -> string a
                | B b -> b
                | C c -> string c
    interface AsString2<SomeDu>
        with member self.asString x =
                if not (x = self) then failwith "argument must be the member container"
                match self with
                | A a -> string a
                | B b -> b
                | C c -> string c

//this expression was expected to have type AsString but here has type SomeDU            
let implSomeDU = 
    {new AsString with
        member self.asString () =
            match self with
            | A a -> string a
            | B b -> b
            | C c -> string c}
        """
         |> withLangVersion90
         |> typecheck
         |> shouldFail
         |> withDiagnostics [
            Error 1, Line 31, Col 15, Line 31, Col 18, "This expression was expected to have type
    'AsString'    
but here has type
    'SomeDu'    "
            Error 1, Line 32, Col 15, Line 32, Col 18, "This expression was expected to have type
    'AsString'    
but here has type
    'SomeDu'    "
            Error 1, Line 33, Col 15, Line 33, Col 18, "This expression was expected to have type
    'AsString'    
but here has type
    'SomeDu'    "
            Warning 25, Line 30, Col 19, Line 30, Col 23, "Incomplete pattern matches on this expression."]

    [<Fact>]
    let ``Object expression implementing multiple interfaces`` () =
        Fsx """
// Define two interfaces
type IFirst =
  abstract F : unit -> unit
  abstract G : unit -> unit

type ISecond =
  inherit IFirst
  abstract H : unit -> unit
  abstract J : unit -> unit

// This object expression implements both interfaces.
let implementer() =
    { new ISecond with
        member this.H() = ()
        member this.J() = ()
      interface IFirst with
        member this.F() = ()
        member this.G() = () }
        """
         |> withLangVersion90
         |> typecheck
         |> shouldSucceed
         
    [<Fact>]
    let ``Object expression implementing an interface`` () =
        Fsx """
type IOperation =
    abstract member Execute: unit -> unit
    abstract member Execute1: unit -> bool
    abstract member Property: int
    
let objExpr =
    { new IOperation with
        member this.Execute() = ()
        member this.Execute1() = false
        member this.Property = 0 }
        """
         |> withLangVersion90
         |> typecheck
         |> shouldSucceed

    [<Fact>]
    let ``Produce an error when one leaves out keyword "static" when implementing IWSAM in an object expression`` () =
        Fsx """
type IOperation =
    static abstract member Execute: unit -> unit
    abstract member Execute2: unit -> unit
    
let objExpr =
    { new IOperation with
        member _.Execute() = ()
        member _.Execute2() = () }
        """
         |> withOptions [ "--nowarn:3536" ; "--nowarn:3535" ]
         |> withLangVersion90
         |> typecheck
         |> shouldFail
         |> withDiagnostics [
            (Error 3860, Line 7, Col 11, Line 7, Col 21, "Object expressions cannot implement interfaces with static abstract members or declare static members.")
            (Error 17, Line 8, Col 18, Line 8, Col 25, "The member 'Execute: unit -> unit' does not have the correct type to override the corresponding abstract method. Non-static member is expected.");
            (Error 783, Line 7, Col 11, Line 7, Col 21, "At least one override did not correctly implement its corresponding abstract member")
         ]
        
    [<FactForNETCOREAPP>]
    let ``Produce an error when one leaves keyword "static" when implementing IWSAM in an object expression`` () =
        Fsx """
type IOperation =
        static abstract member Execute: unit -> unit
        abstract member Execute2: unit -> unit

let _ =
    { new IOperation with
        static member Execute() = ()
        member _.Execute2() = ()
    }
        """
         |> withOptions [ "--nowarn:3536" ; "--nowarn:3535" ]
         |> withLangVersion90
         |> typecheck
         |> shouldFail
         |> withDiagnostics [
            (Error 3860, Line 7, Col 11, Line 7, Col 21, "Object expressions cannot implement interfaces with static abstract members or declare static members.");
            (Error 3860, Line 8, Col 23, Line 8, Col 30, "Object expressions cannot implement interfaces with static abstract members or declare static members.")
         ]
         
    [<FactForNETCOREAPP>]
    let ``Produce an error when one leaves keyword "static" when implementing multiple IWSAM in an object expression`` () =
        Fsx """
type IOperation =
        static abstract member Execute: unit -> unit
        static abstract member Execute2: unit -> unit

let _ =
    { new IOperation with
        static member Execute() = ()
        static member Execute2() = ()
    }
        """
         |> withOptions [ "--nowarn:3536" ; "--nowarn:3535" ]
         |> withLangVersion90
         |> typecheck
         |> shouldFail
         |> withDiagnostics [
            (Error 3860, Line 7, Col 11, Line 7, Col 21, "Object expressions cannot implement interfaces with static abstract members or declare static members.")
            (Error 3860, Line 8, Col 23, Line 8, Col 30, "Object expressions cannot implement interfaces with static abstract members or declare static members.");
            (Error 3860, Line 9, Col 23, Line 9, Col 31, "Object expressions cannot implement interfaces with static abstract members or declare static members.")
         ]
         
    [<FactForNETCOREAPP>]
    let ``Produce an error when one leaves keyword "static" when implementing static and not static members in an object expression`` () =
        Fsx """
type IOperation =
        static abstract member Execute: unit -> unit
        abstract member Execute1: unit -> unit
        static abstract member Execute2: unit -> unit
        abstract member Execute3: bool
        
let _ =
    { new IOperation with
        static member Execute() = ()
        member this.Execute1() = ()
        static member Execute2() = ()
        member this.Execute3 = true
    }
        """
         |> withOptions [ "--nowarn:3536" ; "--nowarn:3535" ]
         |> withLangVersion90
         |> typecheck
         |> shouldFail
         |> withDiagnostics [
            (Error 3860, Line 9, Col 11, Line 9, Col 21, "Object expressions cannot implement interfaces with static abstract members or declare static members.")
            (Error 3860, Line 10, Col 23, Line 10, Col 30, "Object expressions cannot implement interfaces with static abstract members or declare static members.");
            (Error 3860, Line 12, Col 23, Line 12, Col 31, "Object expressions cannot implement interfaces with static abstract members or declare static members.");
         ]
         
    [<FactForNETCOREAPP>]
    let ``No error when implementing only instance members from IWSAM in an object expression`` () =
        Fsx """
type ILogger =
    abstract member Log: string -> unit
    static abstract member Execute: string -> unit
        
let consoleLogger =
    { new ILogger with
        member this.Log(message: string) =
            printfn "%s" message
    }
    
consoleLogger.Log("Hello World")
        """
         |> withOptions [ "--nowarn:3536" ; "--nowarn:3535" ]
         |> withLangVersion90
         |> compile
         |> shouldFail
         |> withSingleDiagnostic (Error 3860, Line 7, Col 11, Line 7, Col 18, "Object expressions cannot implement interfaces with static abstract members or declare static members.")
         
    [<FactForNETCOREAPP>]
    let ``No error when implementing only instance members from IWSAM(Interface attribute) in an object expression`` () =
        Fsx """
[<Interface>]
type ILogger =
    abstract member Log: string -> unit
    static abstract member Execute: string -> unit
        
let consoleLogger =
    { new ILogger with
        member this.Log(message: string) =
            printfn "%s" message
    }
    
consoleLogger.Log("Hello World")
        """
         |> withOptions [ "--nowarn:3536" ; "--nowarn:3535" ]
         |> withLangVersion90
         |> compile
         |> shouldFail
         |> withSingleDiagnostic (Error 3860, Line 8, Col 11, Line 8, Col 18, "Object expressions cannot implement interfaces with static abstract members or declare static members.")
         
    [<FactForNETCOREAPP>]
    let ``No error when implementing only instance members from a type(Interface attribute) in an object expression`` () =
        Fsx """
[<Interface>]
type ILogger =
    abstract member Log: string -> unit
    static member Execute x = x

let consoleLogger =
    { new ILogger with
        member this.Log(message: string) =
            printf "%A" message
    }
    
consoleLogger.Log("Hello World")
        """
         |> withLangVersion90
         |> compileExeAndRun
         |> shouldSucceed

    [<Fact>]
    let ``Error reporting ambiguous override method in object expression`` () =
        Fsx """
type IExample =
    abstract member Overloaded : string -> bool
    abstract member Overloaded : int -> bool

let failingExample x =
    { new IExample with
        member __.Overloaded (_ : string) = x
        member __.Overloaded (_ : int)    = x }
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3213, Line 8, Col 19, Line 8, Col 29, "The member 'Overloaded: string -> 'a' matches multiple overloads of the same method.
Please restrict it to one of the following:
   Overloaded: int -> bool
   Overloaded: string -> bool.")
            (Error 3213, Line 9, Col 19, Line 9, Col 29, "The member 'Overloaded: int -> 'a' matches multiple overloads of the same method.
Please restrict it to one of the following:
   Overloaded: int -> bool
   Overloaded: string -> bool.")
            (Error 358, Line 8, Col 19, Line 8, Col 29, "The override for 'Overloaded: int -> bool' was ambiguous")
            (Error 358, Line 8, Col 19, Line 8, Col 29, "The override for 'Overloaded: string -> bool' was ambiguous")
            (Error 783, Line 7, Col 11, Line 7, Col 19, "At least one override did not correctly implement its corresponding abstract member")
        ]
    
module AllowObjectExpressionWithoutOverrides =
    [<Fact>]
    let ``Object expression can implement a class class end`` () =
        Fsx """
type Class() = class end

let implementer = { new Class()  }
        """
         |> withLangVersionPreview
         |> typecheck
         |> shouldSucceed
         
    [<Fact>]
    let ``Object expression can implement a class without members`` () =
        Fsx """
type Class() =
    member this.Do() = ()

let implementer = { new Class()  }
        """
         |> withLangVersionPreview
         |> typecheck
         |> shouldSucceed

    [<Fact>]
    let ``Object expression cannot implement unnamed interface`` () =
        Fsx """
type IFirst =
    abstract member MyMember: unit -> int

let x = { new _ with member this.MyMember() = 42 }
        """
         |> withLangVersionPreview
         |> typecheck
         |> shouldFail
         |> withDiagnostics [
             (Error 772, Line 5, Col 11, Line 5, Col 16, "'new' must be used with a named type")
         ]
         
    [<Fact>]
    let ``Verifies that the object expression built type has the interface`` () =
        Fsx """
type IFirst = interface end

type ISecond =
    abstract member M : unit -> unit

[<AbstractClass>]
type MyClass() =
    interface ISecond with
        member this.M() = printfn "It works"

let expr = { new MyClass() }
(expr:> ISecond).M()
        """
         |> withLangVersionPreview
         |> compileExeAndRun
         |> shouldSucceed
         |> withStdOutContainsAllInOrder [
           "It works"
        ]
         
    [<Fact>]
    let ``Object expression implementing an interface without members`` () =
        Fsx """
type IFirst = interface end

let implementer() ={ new IFirst  }
        """
         |> withLangVersionPreview
         |> typecheck
         |> shouldSucceed
         
    [<Fact>]
    let ``Object expression can implement an abstract class having abstract members with default implementation`` () =
        Fsx """
[<AbstractClass>]
type AbstractClass() =
    abstract member M : unit -> unit
    default this.M() = printfn "Im a default implementation"
    
let res = { new AbstractClass() }
        """
         |> withLangVersionPreview
         |> typecheck
         |> shouldSucceed

    [<Fact>]
    let ``Object expression can implement an abstract class(overriding a member) having abstract members with default implementation`` () =
        Fsx """
[<AbstractClass>]
type AbstractClass() =
    abstract member M : unit -> unit
    default this.M() = printfn "Im a default implementation"
    
let res = { new AbstractClass() with
                override this.ToString() = "ConcreteMethod" }
        """
         |> withLangVersionPreview
         |> typecheck
         |> shouldSucceed
         
    [<Fact>]
    let ``Object expression can implement an abstract class with a protected constructor`` () =
        Fsx """
[<AbstractClass>]
type AbstractClass() =
    do printfn "AbstractClass constructor"
    
let res = { new AbstractClass() }
        """
        |> withLangVersionPreview
        |> compileExeAndRun
        |> withStdOutContainsAllInOrder [
            "AbstractClass constructor"
        ]
        
    [<Fact>]
    let ``Object expression can not implement an abstract class having abstract members, unless the abstract members are implemented`` () =
        Fsx """
[<AbstractClass>]
type AbstractClass() =
    do printfn "AbstractClass constructor"
    abstract member M : unit -> unit
    
let res = { new AbstractClass() }
        """
         |> withLangVersionPreview
         |> typecheck
         |> shouldFail
         |> withDiagnostics [
             (Error 365, Line 7, Col 11, Line 7, Col 34, "No implementation was given for 'abstract AbstractClass.M: unit -> unit'")
         ]

    [<Fact>]
    let ``Object expression can implement an abstract class having no abstract members, only if the object expression has an override`` () =
        Fsx """
[<AbstractClass>]
type Foo() = class end

let foo = { new Foo() }

let foo2 = { new Foo() with member __.ToString() = base.ToString() }
        """
         |> withLangVersionPreview
         |> typecheck
         |> shouldSucceed
         
    [<Fact>]
    let ``Object expression can not implement an abstract class and interface having no abstract members`` () =
        Fsx """
type IFirst = interface end

[<AbstractClass>]
type MyClass() = class end

{ new MyClass() with
    member x.ToString() = "OK"
    
  interface IFirst } |> ignore
        """
         |> withLangVersionPreview
         |> typecheck
         |> shouldSucceed
         
    [<Fact>]
    let ``Object expression can implement an abstract class having no abstract members. But trying to instantiate an abstract class will fail`` () =
        Fsx """
[<AbstractClass>]
type Foo() = class end

let foo = { new Foo() }

let foo1 = new Foo()
        """
         |> withLangVersionPreview
         |> typecheck
         |> shouldFail
         |> withDiagnostics [
             (Error 759, Line 7, Col 12, Line 7, Col 21, "Instances of this type cannot be created since it has been marked abstract or not all methods have been given implementations. Consider using an object expression '{ new ... with ... }' instead.")
         ]
         
    [<Fact>]
    let ``Object expression shows error when object expression does not implement all abstract members of the abstract class`` () =
        Fsx """
type ISecond =
    abstract member M : unit -> unit
    
[<AbstractClass>]
type MyClass() =
    abstract member M : unit -> unit
    interface ISecond with
        member this.M() = printfn "It works"

let res = { new MyClass() }
        """
         |> withLangVersionPreview
         |> typecheck
         |> shouldFail
         |> withDiagnostics [
             (Error 365, Line 11, Col 11, Line 11, Col 28, "No implementation was given for 'abstract MyClass.M: unit -> unit'")
         ]  

    [<Fact>]
    let ``C# abstract class with protected constructor can be implemented by F# object expression`` () =

        let csharp =
            CSharp
                """
namespace CSLib
{
    using System;
    public abstract class Animal
    {
        protected Animal()
        {
            Console.WriteLine("Animal is created");
        }
    }
}
"""
            |> withName "CSLib"

        let fsharp =
            FSharp
                """
module FSLib
open CSLib

let res = { new Animal() }
"""
            |> withLangVersionPreview
            |> withName "FSLib"
            |> withReferences [ csharp ]

        fsharp
        |> compile
        |> shouldSucceed
        
    [<Fact>]
    let ``C# abstract class with protected constructor and abstract method can not be implemented by F# object expression unless the abstract method is implemented`` () =

        let csharp =
            CSharp
                """
namespace CSLib
{
    using System;
    public abstract class Animal
    {
        protected Animal()
        {
            Console.WriteLine("Animal is created");
        }
        
        public abstract void M();
    }
}
"""
            |> withName "CSLib"

        let fsharp =
            FSharp
                """
module FSLib
open CSLib

let res = { new Animal() }
"""
            |> withLangVersionPreview
            |> withName "FSLib"
            |> withReferences [ csharp ]

        fsharp
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 365, Line 5, Col 11, Line 5, Col 27, "No implementation was given for 'Animal.M() : unit'")
        ]
         