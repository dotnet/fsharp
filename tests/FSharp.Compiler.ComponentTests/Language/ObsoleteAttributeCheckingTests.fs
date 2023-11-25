namespace Language

open Xunit
open FSharp.Test.Compiler

module ObsoleteAttributeCheckingTests =
    
    [<Fact>]
    let ``Obsolete attribute is not taken into account when used on on a member and and instantiate the type`` () =
        Fsx """
open System

type C() =
    [<Obsolete("Use B instead", true)>]
    member _.Update() = ()

let c = C()
        """
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        
    [<Fact>]
    let ``Obsolete attribute warning taken into account when used instantiating a type`` () =
        Fsx """
open System
[<Obsolete("Use B instead")>]
type C() =
    member _.Update() = ()

let c = C()
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 44, Line 7, Col 9, Line 7, Col 10, "This construct is deprecated. Use B instead")
        ]
        
    [<Fact>]
    let ``Obsolete attribute error taken into account when used instantiating a type`` () =
        Fsx """
open System
[<Obsolete("Use B instead", true)>]
type C() =
    member _.Update() = ()

let c = C()
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 7, Col 9, Line 7, Col 10, "This construct is deprecated. Use B instead")
        ]

    [<Fact>]
    let ``Obsolete attribute is taken into account when used on a member and invoking the member`` () =
        Fsx """
open System

type C() =
    [<Obsolete("Use B instead", true)>]
    member _.Update() = ()

let c = C()
c.Update()
        """
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 9, Col 1, Line 9, Col 9, "This construct is deprecated. Use B instead")
        ]

    [<Fact>]
    let ``Obsolete attribute is taken into account when used on type and invoking the member`` () =
        Fsx """
open System

[<Obsolete("Use B instead", true)>]
type C() =
    member _.Update() = ()

let c = C()
c.Update()
        """
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 8, Col 9, Line 8, Col 10, "This construct is deprecated. Use B instead");
            (Error 101, Line 9, Col 1, Line 9, Col 9, "This construct is deprecated. Use B instead")
        ]

    [<Fact>]
    let ``Obsolete attribute is taken into account when used on struct type and invoking the member`` () =
        Fsx """
open System

[<Obsolete("Use B instead", true)>]
[<Struct>]
type C =
    member _.Update() = ()

let c = C()
c.Update()
        """
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 9, Col 9, Line 9, Col 10, "This construct is deprecated. Use B instead");
            (Error 101, Line 10, Col 1, Line 10, Col 9, "This construct is deprecated. Use B instead")
        ]

    [<Fact>]
    let ``Obsolete attribute is taken into account when used on struct type and instantiate the type`` () =
        Fsx """
open System

[<Obsolete("Use B instead", true)>]
[<Struct>]
type C =
    member _.Update() = ()

let c = C()
        """
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 9, Col 9, Line 9, Col 10, "This construct is deprecated. Use B instead")
        ]

    [<Fact>]
    let ``Obsolete attribute is taken into account when used on a struct member and invoking the member`` () =
        Fsx """
open System

[<Struct>]
type C =
    [<Obsolete("Use B instead", true)>]
    member _.Update() = ()

let c = C()
c.Update()
        """
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 10, Col 1, Line 10, Col 9, "This construct is deprecated. Use B instead")
        ]

    [<Fact>]
    let ``Obsolete attribute is taken into account when used on a record property`` () =
        Fsx """
open System

type C = 
    { [<Obsolete("Use B instead", true)>] X: int } 

let c = { X = 0 }
        """
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 7, Col 11, Line 7, Col 12, "This construct is deprecated. Use B instead")
        ]

    [<Fact>]
    let ``Obsolete attribute is taken into account when used on a record and member invocation`` () =
        Fsx """
open System

[<Obsolete("Use B instead", true)>]
type C = 
    {  X : int }
    
    static member Update() = ()
    
C.Update()
        """
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 10, Col 1, Line 10, Col 2, "This construct is deprecated. Use B instead")
        ]
        
    [<Fact>]
    let ``Obsolete attribute is taken into account when used on a record member and method invocation`` () =
        Fsx """
open System

type C = 
    {  X : int }
    [<Obsolete("Use B instead", true)>]
    static member Update() = ()
    
C.Update()
        """
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 9, Col 1, Line 9, Col 9, "This construct is deprecated. Use B instead")
        ]
        
    [<Fact>]
    let ``Obsolete attribute error is taken into account when used on a struct du and invocation`` () =
        Fsx """
open System
[<Struct>]
[<Obsolete("Use B instead", true)>]
type Color =
    | Red 
    | Green
    
let c = Color.Red
        """
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 9, Col 9, Line 9, Col 14, "This construct is deprecated. Use B instead")
        ]

    [<Fact>]
    let ``Obsolete attribute error is taken into account when used on a du and invocation`` () =
        Fsx """
open System
[<Obsolete("Use B instead", true)>]
type Color =
    | Red 
    | Green
    
let c = Color.Red
        """
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 8, Col 9, Line 8, Col 14, "This construct is deprecated. Use B instead")
        ]

    [<Fact>]
    let ``Obsolete attribute error is taken into account when used on a du field and invocation`` () =
        Fsx """
open System
type Color =
    | [<Obsolete("Use B instead", true)>] Red 
    | Green
    
let c = Color.Red
        """
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 7, Col 9, Line 7, Col 18, "This construct is deprecated. Use B instead")
        ]

    [<Fact>]
    let ``Obsolete attribute warning is taken into account when used on a du field and invocation`` () =
        Fsx """
open System
type Color =
    | [<Obsolete("Use B instead")>] Red 
    | Green
    
let c = Color.Red
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 44, Line 7, Col 9, Line 7, Col 18, "This construct is deprecated. Use B instead")
        ]

    [<Fact>]
    let ``Obsolete attribute error is taken into account when used on an enum and invocation`` () =
        Fsx """
open System

[<Obsolete("Use B instead", true)>]
type Color =
    | Red = 0
    | Green = 1
    
let c = Color.Red
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 9, Col 9, Line 9, Col 14, "This construct is deprecated. Use B instead")
        ]
     
    [<Fact>]
    let ``Obsolete attribute error is taken into account when used on an enum field and invocation`` () =
        Fsx """
open System

type Color =
    | [<Obsolete("Use B instead", true)>] Red = 0
    | Green = 1
    
let c = Color.Red
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 8, Col 9, Line 8, Col 18, "This construct is deprecated. Use B instead")
        ]

    [<Fact>]
    let ``Obsolete attribute warning is taken into account when used on an enum field and invocation`` () =
        Fsx """
open System

type Color =
    | [<Obsolete("Use B instead")>] Red = 0
    | Green = 1
    
let c = Color.Red
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 44, Line 8, Col 9, Line 8, Col 18, "This construct is deprecated. Use B instead")
        ]

    [<Fact>]
    let ``Obsolete attribute is taken into account when used on an type and use extension method`` () =
        Fsx """

open System
open System.Runtime.CompilerServices

[<Obsolete("Use B instead", true)>]
type Button = { Text : string }

[<Extension>]
type ButtonExtensions =

    [<Extension>]
    static member inline text(this: Button, text: string) =
        { this with Text = text }
        """
        |> ignoreWarnings
        |> withOptions ["--nowarn:3560"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 13, Col 37, Line 13, Col 43, "This construct is deprecated. Use B instead")
        ]

    [<Fact>]
    let ``Obsolete attribute is taken into account when used on an type property and use extension method`` () =
        Fsx """

open System
open System.Runtime.CompilerServices

type Button = { [<Obsolete("Use B instead", true)>] Text : string }

[<Extension>]
type ButtonExtensions =

    [<Extension>]
    static member inline text(this: Button, text: string) =
        { this with Text = text }
        """
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 13, Col 21, Line 13, Col 25, "This construct is deprecated. Use B instead")
        ]

    [<Fact>]
    let ``Obsolete attribute is taken into account when used on an type and property and use extension method`` () =
        Fsx """
open System
open System.Runtime.CompilerServices

[<Obsolete("Use B instead", true)>]
type Button = { [<Obsolete("Use B instead", true)>] Text : string }

[<Extension>]
type ButtonExtensions =

    [<Extension>]
    static member inline text(this: Button, text: string) =
        { this with Text = text }
        """
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 12, Col 37, Line 12, Col 43, "This construct is deprecated. Use B instead");
            (Error 101, Line 13, Col 21, Line 13, Col 25, "This construct is deprecated. Use B instead")
        ]

    [<Fact>]
    let ``Obsolete attribute is taken into account when used on an type property and set via module`` () =
        Fsx """
open System
open System.Runtime.CompilerServices

type Button = { [<Obsolete("Use B instead", true)>] Text : string }

module Button =
    
    let set text = { Text = text  }
        """
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 9, Col 22, Line 9, Col 26, "This construct is deprecated. Use B instead")
        ]


    [<Fact>]
    let ``Obsolete attribute is taken into account when used on an type and set property via module`` () =
        Fsx """
open System
open System.Runtime.CompilerServices

 [<Obsolete("Use B instead", true)>]
type Button = { [<Obsolete("Use B instead", true)>] Text : string }

module Button =
    
    let set text = { Text = text  }
        """
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 10, Col 22, Line 10, Col 26, "This construct is deprecated. Use B instead")
        ]

    [<Fact>]
    let ``Obsolete attribute is taken into account when used on an type property and set property via module using an extension method`` () =
        Fsx """
open System
open System.Runtime.CompilerServices

type Button = { [<Obsolete("Use B instead", true)>] Text : string }

module Button =
    
    let set text = { Text = text  }
[<Extension>]
type ButtonExtensions =

    [<Extension>]
    static member inline text(this: Button, text: string) =
        Button.set text
        """
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 9, Col 22, Line 9, Col 26, "This construct is deprecated. Use B instead")
        ]

    [<Fact>]
    let ``Obsolete attribute is taken into account when used on an module and set property via module using an extension method`` () =
        Fsx """
open System
open System.Runtime.CompilerServices

type Button = { Text : string }

[<Obsolete("Use B instead", true)>]
module Button =
    
    let set text = { Text = text  }

[<Extension>]
type ButtonExtensions =

    [<Extension>]
    static member inline text(this: Button, text: string) =
        Button.set text
        """
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 17, Col 9, Line 17, Col 15, "This construct is deprecated. Use B instead")
        ]

    [<Fact>]
    let ``Obsolete attribute is taken into account when used on an module and function and set property via module using an extesnion method`` () =
        Fsx """
open System
open System.Runtime.CompilerServices

type Button = { Text : string }

[<Obsolete("Use B instead", true)>]
module Button =
    
    [<Obsolete("Use B instead", true)>]
    let set text = { Text = text  }

[<Extension>]
type ButtonExtensions =

    [<Extension>]
    static member inline text(this: Button, text: string) =
        Button.set text
        """
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 18, Col 9, Line 18, Col 15, "This construct is deprecated. Use B instead")
        ]

    [<Fact>]
    let ``Obsolete attribute is taken into account when used on an moudle function and set property via module using an extesnion method`` () =
        Fsx """
open System
open System.Runtime.CompilerServices

type Button = { Text : string }

module Button =
    
    [<Obsolete("Use B instead", true)>]
    let set text = { Text = text  }

[<Extension>]
type ButtonExtensions =

    [<Extension>]
    static member inline text(this: Button, text: string) =
        Button.set text
        """
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 17, Col 9, Line 17, Col 19, "This construct is deprecated. Use B instead")
        ]

    [<Fact>]
    let ``Obsolete attribute is taken into account when used on an type extensions and used on an instance`` () =
        Fsx """
open System
open System.Runtime.CompilerServices

type Button = { Text : string }

[<Extension>]
[<Obsolete("Use B instead", true)>]
type ButtonExtensions =

    [<Extension>]
    static member inline text(this: Button, text: string) =
        { this with Text = text }
        
let b = { Text = "Hello" }
b.text("Hello 2") |> ignore
        """
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Obsolete attribute is taken into account when used on an type extensions static function and used on an instance`` () =
        Fsx """
open System
open System.Runtime.CompilerServices

type Button = { Text : string }

[<Extension>]
type ButtonExtensions =

    [<Extension>]
    [<Obsolete("Use B instead", true)>]
    static member inline text(this: Button, text: string) =
        { this with Text = text }
        
let b = { Text = "Hello" }
b.text("Hello 2") |> ignore
        """
        |> ignoreWarnings
        |> withOptions ["--nowarn:3560"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 16, Col 1, Line 16, Col 7, "This construct is deprecated. Use B instead")
        ]
    
    [<Fact>]
    let ``Obsolete attribute error is taken into account when used in one the record properties`` () =
        Fsx """
open System
type MyType = { [<Obsolete("Deprecated Field", true)>] DeprecatedField: string ; JustField: string }
let a = { DeprecatedField= "23" ; JustField = "" } 
        """
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 4, Col 11, Line 4, Col 26, "This construct is deprecated. Deprecated Field")
        ]
        
    // This should only report one warning but instead show two. Related issue https://github.com/dotnet/fsharp/issues/14203
    [<Fact>]
    let ``Obsolete attribute warning is taken into account when used in one the record properties`` () =
        Fsx """
open System
type MyType = { [<Obsolete("Deprecated Field")>] DeprecatedField: string ; JustField: string }
let a = { DeprecatedField= "23" ; JustField = "" } 
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 44, Line 4, Col 11, Line 4, Col 26, "This construct is deprecated. Deprecated Field")
        ]
    
    [<Fact>]
    let ``Obsolete attribute warning is taken into account when used in one the class with a setter`` () =
        Fsx """
type Class1() =
  let mutable internalValue = 1
  member _.A with get () = internalValue
  [<System.Obsolete("member A is deprecated")>] member _.A with set (v: int) = internalValue <- v

let class1 = new Class1()
let value1 = class1.A
let value2 = class1.A <- 12
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 44, Line 9, Col 14, Line 9, Col 22, "This construct is deprecated. member A is deprecated");
            (Warning 44, Line 9, Col 14, Line 9, Col 28, "This construct is deprecated. member A is deprecated")
        ]
        
    [<Fact>]
    let ``Obsolete attribute error is taken into account when used in one the class with a setter`` () =
        Fsx """
type Class1() =
  let mutable internalValue = 1
  member _.A with get () = internalValue
  [<System.Obsolete("member A is deprecated", true)>] member _.A with set (v: int) = internalValue <- v

let class1 = new Class1()
let value1 = class1.A
let value2 = class1.A <- 12
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 9, Col 14, Line 9, Col 22, "This construct is deprecated. member A is deprecated")
        ]
        
    [<Fact>]
    let ``Obsolete attribute warning is taken into account when used in one the class with a getter`` () =
        Fsx """
type Class1() =
  let mutable internalValue = 1
  [<System.Obsolete("member A is deprecated")>] member _.A with get () = internalValue
  member _.A with set (v: int) = internalValue <- v

let class1 = new Class1()
let value1 = class1.A
let value2 = class1.A <- 12
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
           (Warning 44, Line 8, Col 14, Line 8, Col 22, "This construct is deprecated. member A is deprecated");
           (Warning 44, Line 9, Col 14, Line 9, Col 22, "This construct is deprecated. member A is deprecated")
        ]
        
    [<Fact>]
    let ``Obsolete attribute error is taken into account when used in one the class with a getter`` () =
        Fsx """
type Class1() =
  let mutable internalValue = 1
  [<System.Obsolete("member A is deprecated", true)>] member _.A with get () = internalValue
  member _.A with set (v: int) = internalValue <- v

let class1 = new Class1()
let value1 = class1.A
let value2 = class1.A <- 12
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 8, Col 14, Line 8, Col 22, "This construct is deprecated. member A is deprecated")
            (Error 101, Line 9, Col 14, Line 9, Col 22, "This construct is deprecated. member A is deprecated")
        ]
    
    [<Fact>]
    let ``Obsolete attribute warning is taken into account when a module is is marked as [<AutoOpen>] but not when calling a function`` () =
        Fsx """
[<System.Obsolete("This is obsolete")>]
[<AutoOpen>]
module MyModule =
    let testFun () = printfn "test"

open MyModule

testFun ()
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 44, Line 7, Col 6, Line 7, Col 14, "This construct is deprecated. This is obsolete")
        ]
        
    [<Fact>]
    let ``Obsolete attribute error is taken into account when a module is is marked as [<AutoOpen>] but not when calling a function`` () =
        Fsx """
[<System.Obsolete("This is obsolete", true)>]
[<AutoOpen>]
module MyModule =
    let testFun () = printfn "test"

open MyModule

testFun ()
        """
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 7, Col 6, Line 7, Col 14, "This construct is deprecated. This is obsolete")
        ]
        
    [<Fact>]
    let ``Obsolete attribute warning is taken into account when used on an C# struct`` () =
        let CSLib =
            CSharp """
using System;
[Obsolete("Struct is obsolete")]
public struct ObsoleteStruct
{
}
        """ |> withName "CSLib"

        let app =
            FSharp """
module ObsoleteStruct.FS
let s1: ObsoleteStruct = ObsoleteStruct()
        """ |> withReferences [CSLib]

        app
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 44, Line 3, Col 9, Line 3, Col 23, "This construct is deprecated. Struct is obsolete");
            (Warning 44, Line 3, Col 26, Line 3, Col 40, "This construct is deprecated. Struct is obsolete")
        ]

    [<Fact>]
    let ``Obsolete attribute error is taken into account when used on an C# struct`` () =
        let CSLib =
            CSharp """
using System;
[Obsolete("Struct is obsolete", true)]
public struct ObsoleteStruct
{
}
        """ |> withName "CSLib"

        let app =
            FSharp """
module ObsoleteStruct.FS
let s1: ObsoleteStruct = ObsoleteStruct()
        """ |> withReferences [CSLib]

        app
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 3, Col 9, Line 3, Col 23, "This construct is deprecated. Struct is obsolete");
            (Error 101, Line 3, Col 26, Line 3, Col 40, "This construct is deprecated. Struct is obsolete")
        ]

    [<Fact>]
    let ``Obsolete attribute warning is taken into account when used on an C# class`` () =
        let CSLib =
            CSharp """
using System;
[Obsolete("Class is obsolete")]
public class ObsoleteClass
{
}
        """ |> withName "CSLib"

        let app =
            FSharp """
module ObsoleteClass.FS
let c1: ObsoleteClass = null
let c2 = ObsoleteClass()
        """ |> withReferences [CSLib]

        app
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 44, Line 3, Col 9, Line 3, Col 22, "This construct is deprecated. Class is obsolete");
            (Warning 44, Line 4, Col 10, Line 4, Col 23, "This construct is deprecated. Class is obsolete")
        ]

    [<Fact>]
    let ``Obsolete attribute error is taken into account when used on an C# class`` () =
        let CSLib =
            CSharp """
using System;
[Obsolete("Class is obsolete", true)]
public class ObsoleteClass
{
}
        """ |> withName "CSLib"

        let app =
            FSharp """
module ObsoleteClass.FS
let c1: ObsoleteClass = null
let c2 = ObsoleteClass()
        """ |> withReferences [CSLib]

        app
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 3, Col 9, Line 3, Col 22, "This construct is deprecated. Class is obsolete");
            (Error 101, Line 4, Col 10, Line 4, Col 23, "This construct is deprecated. Class is obsolete")
        ]
        
    [<Fact>]
    let ``Obsolete attribute warning is taken into account when used on an C# interface`` () =
        let CSLib =
            CSharp """
using System;
[Obsolete("Interface is obsolete")]
public interface IObsoleteInterface
{
    int P { get; }
}
        """ |> withName "CSLib"

        let app =
            FSharp """
module ObsoleteInterface.FS
let i1: IObsoleteInterface = null
let i2 = { new IObsoleteInterface with
             member this.P = 1 }
        """ |> withReferences [CSLib]

        app
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 44, Line 3, Col 9, Line 3, Col 27, "This construct is deprecated. Interface is obsolete");
            (Warning 44, Line 4, Col 16, Line 4, Col 34, "This construct is deprecated. Interface is obsolete")
        ]

    [<Fact>]
    let ``Obsolete attribute error is taken into account when used on an C# interface`` () =
        let CSLib =
            CSharp """
using System;
[Obsolete("Interface is obsolete", true)]
public interface IObsoleteInterface
{
    int P { get; }
}
        """ |> withName "CSLib"

        let app =
            FSharp """
module ObsoleteInterface.FS
let i1: IObsoleteInterface = null
let i2 = { new IObsoleteInterface with
             member this.P = 1 }
        """ |> withReferences [CSLib]

        app
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 3, Col 9, Line 3, Col 27, "This construct is deprecated. Interface is obsolete");
            (Error 101, Line 4, Col 16, Line 4, Col 34, "This construct is deprecated. Interface is obsolete")
        ]

    [<Fact>]
    let ``Obsolete attribute warning is taken into account when used on an C# delegate`` () =
        let CSLib =
            CSharp """
using System;
[Obsolete("Delegate is obsolete")]
public delegate void ObsoleteDelegate();
        """ |> withName "CSLib"

        let app =
            FSharp """
module ObsoleteDelegate.FS
let d1: ObsoleteDelegate = null
let d2 = ObsoleteDelegate(fun _ -> ())
let d3 = new ObsoleteDelegate(fun _ -> ())
        """ |> withReferences [CSLib]

        app
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 44, Line 3, Col 9, Line 3, Col 25, "This construct is deprecated. Delegate is obsolete");
            (Warning 44, Line 4, Col 10, Line 4, Col 26, "This construct is deprecated. Delegate is obsolete");
            (Warning 44, Line 4, Col 10, Line 4, Col 39, "This construct is deprecated. Delegate is obsolete");
            (Warning 44, Line 5, Col 14, Line 5, Col 30, "This construct is deprecated. Delegate is obsolete");
            (Warning 44, Line 5, Col 10, Line 5, Col 43, "This construct is deprecated. Delegate is obsolete")
        ]

    [<Fact>]
    let ``Obsolete attribute error is taken into account when used on an C# delegate`` () =
        let CSLib =
            CSharp """
using System;
[Obsolete("Delegate is obsolete", true)]
public delegate void ObsoleteDelegate();
        """ |> withName "CSLib"

        let app =
            FSharp """
module ObsoleteDelegate.FS
let d1: ObsoleteDelegate = null
let d2 = ObsoleteDelegate(fun _ -> ())
let d3 = new ObsoleteDelegate(fun _ -> ())
        """ |> withReferences [CSLib]

        app
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 3, Col 9, Line 3, Col 25, "This construct is deprecated. Delegate is obsolete");
            (Error 101, Line 4, Col 10, Line 4, Col 26, "This construct is deprecated. Delegate is obsolete");
            (Error 101, Line 5, Col 14, Line 5, Col 30, "This construct is deprecated. Delegate is obsolete")
        ]

    [<Fact>]
    let ``Obsolete attribute warning is taken into account when used on an C# static fields and methods`` () =
        let CSLib =
            CSharp """
using System;
public class Class
{
    [Obsolete("Field is obsolete")] public static readonly int ObsoleteField = 1;

    [Obsolete("Method is obsolete")]
    public static void ObsoleteMethod()
    {
    }

    [Obsolete("Property is obsolete")] public static int ObsoleteProperty => 1;

    [Obsolete("Event is obsolete")] public static event EventHandler ObsoleteEvent;
}
        """ |> withName "CSLib"

        let app =
            FSharp """
module StaticFieldAndMethods.FS
Class.ObsoleteField |> ignore
Class.ObsoleteMethod()
Class.ObsoleteProperty |> ignore
Class.ObsoleteEvent |> ignore
        """ |> withReferences [CSLib]

        app
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 44, Line 3, Col 1, Line 3, Col 20, "This construct is deprecated. Field is obsolete");
            (Warning 44, Line 4, Col 1, Line 4, Col 21, "This construct is deprecated. Method is obsolete");
            (Warning 44, Line 5, Col 1, Line 5, Col 23, "This construct is deprecated. Property is obsolete")
            (Warning 44, Line 6, Col 1, Line 6, Col 20, "This construct is deprecated. Event is obsolete")
        ]

    [<Fact>]
    let ``Obsolete attribute error is taken into account when used on an C# static fields and methods`` () =
        let CSLib =
            CSharp """
using System;
public class Class
{
    [Obsolete("Field is obsolete", true)] public static readonly int ObsoleteField = 1;

    [Obsolete("Method is obsolete", true)]
    public static void ObsoleteMethod()
    {
    }

    [Obsolete("Property is obsolete", true)] public static int ObsoleteProperty => 1;

    [Obsolete("Event is obsolete", true)] public static event EventHandler ObsoleteEvent;
}
        """ |> withName "CSLib"

        let app =
            FSharp """
module StaticFieldAndMethods.FS
Class.ObsoleteField |> ignore
Class.ObsoleteMethod()
Class.ObsoleteProperty |> ignore
Class.ObsoleteEvent |> ignore
        """ |> withReferences [CSLib]

        app
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 3, Col 1, Line 3, Col 20, "This construct is deprecated. Field is obsolete");
            (Error 101, Line 4, Col 1, Line 4, Col 21, "This construct is deprecated. Method is obsolete");
            (Error 101, Line 5, Col 1, Line 5, Col 23, "This construct is deprecated. Property is obsolete")
            (Error 101, Line 6, Col 1, Line 6, Col 20, "This construct is deprecated. Event is obsolete")
        ]
        
    [<Fact>]
    let ``Obsolete attribute warning is taken into account when used in one the record properties with a static member when one of them is not used`` () =
        Fsx """
open System
type MyType =
    { Field1 : string
      [<Obsolete("Use Field2 instead")>]
      Field2 : string }
    static member Empty =
      { Field1 = null
        Field2 = null }
let x = { MyType.Empty with Field1 = "field1" }
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 44, Line 9, Col 9, Line 9, Col 15, "This construct is deprecated. Use Field2 instead")
        ]

    [<Fact>]
    let ``Obsolete attribute warning is taken into account when used in one the record properties when one of them is not used`` () =
        Fsx """
open System
type MyType =
    { Field1 : string
      [<Obsolete("Use Field2 instead")>]
      Field2 : string }
let field1 = { Field1 = "field1" ; Field2 = "Field2" }
let field2 = { field1 with Field1 = "Field1" }
let field3 = { field1 with Field2 = "Field2" }
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 44, Line 7, Col 36, Line 7, Col 42, "This construct is deprecated. Use Field2 instead")
            (Warning 44, Line 9, Col 28, Line 9, Col 34, "This construct is deprecated. Use Field2 instead")
        ]

    [<Fact>]
    let ``Obsolete attribute error is taken into account when used in one the record properties with a static member when one of them is not used`` () =
        Fsx """
open System
type MyType =
    { Field1 : string
      [<Obsolete("Use Field2 instead", true)>]
      Field2 : string }
    static member Empty =
      { Field1 = null
        Field2 = null }
let x = { MyType.Empty with Field1 = "field1" }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 9, Col 9, Line 9, Col 15, "This construct is deprecated. Use Field2 instead")
        ]

    [<Fact>]
    let ``Obsolete attribute error is taken into account when used in a nested record properties with a static member when one of them is not used`` () =
        Fsx """
open System
type MyType =
    { Field1 : string
      [<Obsolete("Use Field1 instead", true)>]
      Field2 : string }
    static member Empty =
      { Field1 = null
        Field2 = null }
let field1 = { MyType.Empty with Field1 = "" }
let field3 = { field1 with Field2 = "Field2" }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 9, Col 9, Line 9, Col 15, "This construct is deprecated. Use Field1 instead")
            (Error 101, Line 11, Col 28, Line 11, Col 34, "This construct is deprecated. Use Field1 instead")
        ]

    [<Fact>]
    let ``Obsolete attribute error is taken into account when used in one the record with default value and properties with a static member when one of them is not used`` () =
        Fsx """
open System
type MyType =
    { Field1 : string
      [<Obsolete("Use Field2 instead", true); DefaultValue>]
      Field2 : string }
    static member Empty =
      { Field1 = null }
let x = { MyType.Empty with Field1 = "field1" }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Obsolete attribute error is taken into account when used in one the record properties when one of them is not used`` () =
        Fsx """
open System
type MyType =
    { Field1 : string
      [<Obsolete("Use Field2 instead", true)>]
      Field2 : string }
let field1 = { Field1 = "field1" ; Field2 = "Field2" }
let field2 = { field1 with Field1 = "Field1" }
let field3 = { field1 with Field2 = "Field2" }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 7, Col 36, Line 7, Col 42, "This construct is deprecated. Use Field2 instead")
            (Error 101, Line 9, Col 28, Line 9, Col 34, "This construct is deprecated. Use Field2 instead")
        ]


    [<Fact>]
    let ``Obsolete attribute error is taken into account when used in one the nested record properties and used in nested copy update`` () =
        Fsx """
open System
type InnerType = { [<Obsolete("Use Field2 instead", true)>] ccont:int }
type OuterType = { Name : string; [<Obsolete("Use Field2 instead", true)>] Ctx : InnerType}
let myInner = { ccont = 5 }
let myOuter = { Name = "Hi"; Ctx = myInner}
let modified = { myOuter with Ctx = { myOuter.Ctx with ccont = 5 } }
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 5, Col 17, Line 5, Col 22, "This construct is deprecated. Use Field2 instead")
            (Error 101, Line 6, Col 30, Line 6, Col 33, "This construct is deprecated. Use Field2 instead")
            (Error 101, Line 7, Col 31, Line 7, Col 34, "This construct is deprecated. Use Field2 instead")
        ]

    [<Fact>]
    let ``Obsolete attribute warning is taken into account when used in an object expression`` () =
        Fsx """
open System
type IFirst =
  [<Obsolete("Use G instead")>]
  abstract F : unit -> unit
  abstract G : unit -> unit

let implementer =
    { new IFirst with
        member this.F() = ()
        member this.G() = () }
    
let f (x: IFirst) = x.F()
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 44, Line 13, Col 21, Line 13, Col 24, "This construct is deprecated. Use G instead")
            (Warning 44, Line 13, Col 21, Line 13, Col 26, "This construct is deprecated. Use G instead")
        ]

    [<Fact>]
    let ``Obsolete attribute error is taken into account when used in an object expression`` () =
        Fsx """
open System
type IFirst =
  [<Obsolete("Use G instead", true)>]
  abstract F : unit -> unit
  abstract G : unit -> unit

let implementer =
    { new IFirst with
        member this.F() = ()
        member this.G() = () }
    
let f (x: IFirst) = x.F()
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 13, Col 21, Line 13, Col 24, "This construct is deprecated. Use G instead")
        ]
        
    [<Fact>]
    let ``Obsolete attribute warning is taken into account when used in interface that is used in an object expression`` () =
        Fsx """
open System
[<Obsolete("Use G instead")>]
type IFirst =
  abstract F : unit -> unit
  abstract G : unit -> unit

let implementer =
    { new IFirst with
        member this.F() = ()
        member this.G() = () }
    
let f (x: IFirst) = x.F()
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 44, Line 9, Col 11, Line 9, Col 17, "This construct is deprecated. Use G instead")
            (Warning 44, Line 13, Col 11, Line 13, Col 17, "This construct is deprecated. Use G instead")
            (Warning 44, Line 13, Col 21, Line 13, Col 24, "This construct is deprecated. Use G instead")
        ]
        
    [<Fact>]
    let ``Obsolete attribute error is taken into account when used in interface that is used in an object expression`` () =
        Fsx """
open System
[<Obsolete("Use G instead", true)>]
type IFirst =
  abstract F : unit -> unit
  abstract G : unit -> unit

let implementer =
    { new IFirst with
        member this.F() = ()
        member this.G() = () }
    
let f (x: IFirst) = x.F()
        """
        |> ignoreWarnings
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 9, Col 11, Line 9, Col 17, "This construct is deprecated. Use G instead")
            (Error 101, Line 13, Col 11, Line 13, Col 17, "This construct is deprecated. Use G instead")
            (Error 72, Line 13, Col 21, Line 13, Col 24, "Lookup on object of indeterminate type based on information prior to this program point. A type annotation may be needed prior to this program point to constrain the type of the object. This may allow the lookup to be resolved.")
        ]
