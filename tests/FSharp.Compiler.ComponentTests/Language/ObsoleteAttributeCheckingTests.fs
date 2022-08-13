namespace FSharp.Compiler.ComponentTests.Language

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
    let ``Obsolete attribute is taken into account when used on type and and instantiate the type`` () =
        Fsx """
open System

[<Obsolete("Use B instead", true)>]
type C() =
  
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
            (Error 101, Line 7, Col 9, Line 7, Col 18, "This construct is deprecated. Use B instead")
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
    let ``Obsolete attribute is taken into account when used on an enum and invocation`` () =
        Fsx """
open System

[<Obsolete("Use B instead", true)>]
type Color =
    | Red = 0
    | Green = 1
    
let c = Color.Red
        """
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 9, Col 9, Line 9, Col 14, "This construct is deprecated. Use B instead")
        ]
        
    [<Fact>]
    let ``Obsolete attribute is taken into account when used on an enum entry and invocation`` () =
        Fsx """
open System

type Color =
    | [<Obsolete("Use B instead", true)>] Red = 0
    | Green = 1
    
let c = Color.Red
        """
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

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
            (Error 101, Line 13, Col 9, Line 13, Col 34, "This construct is deprecated. Use B instead")
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
            (Error 101, Line 13, Col 9, Line 13, Col 34, "This construct is deprecated. Use B instead")
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
            (Error 101, Line 9, Col 20, Line 9, Col 36, "This construct is deprecated. Use B instead")
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
            (Error 101, Line 10, Col 20, Line 10, Col 36, "This construct is deprecated. Use B instead")
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
            (Error 101, Line 9, Col 20, Line 9, Col 36, "This construct is deprecated. Use B instead")
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
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 101, Line 16, Col 1, Line 16, Col 7, "This construct is deprecated. Use B instead")
        ]
        
    [<Fact>]
    let ``Obsolete attribute is taken into account when used on an C# struct`` () =
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
