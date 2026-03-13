// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace ConstraintSolver

open Xunit
open FSharp.Test.Compiler

module MemberConstraints =

    [<Fact>]
    let ``Invalid member constraint with ErrorRanges``() = // Regression test for FSharp1.0:2262
        FSharp """
 let inline length (x: ^a) : int = (^a : (member Length : int with get, set) (x, ()))
        """
        |> withErrorRanges
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 697, Line 2, Col 43, Line 2, Col 76, "Invalid constraint")

    [<Fact>]
    let ``We can overload operators on a type and not add all the extra jazz such as inlining and the ^ operator.``() =

        FSharp """
type Foo(x : int) =
    member this.Val = x

    static member (-->) ((src : Foo), (target : Foo)) = new Foo(src.Val + target.Val)
    static member (-->) ((src : Foo), (target : int)) = new Foo(src.Val + target)

    static member (+) ((src : Foo), (target : Foo)) = new Foo(src.Val + target.Val)
    static member (+) ((src : Foo), (target : int)) = new Foo(src.Val + target)

let x = Foo(3) --> 4
let y = Foo(3) --> Foo(4)
let x2 = Foo(3) + 4
let y2 = Foo(3) + Foo(4)

if x.Val <> 7 then failwith "x.Val <> 7"
elif y.Val <> 7 then  failwith "y.Val <> 7"
elif x2.Val <> 7 then  failwith "x2.Val <> 7"
elif y2.Val <> 7 then  failwith "x.Val <> 7"
else ()
"""
        |> asExe
        |> compile
        |> run
        |> shouldSucceed

    [<Fact>]
    let ``Respect nowarn 957 for extension method`` () =
        FSharp """        
module Foo

type DataItem<'data> =
    { Identifier: string
      Label: string
      Data: 'data }

    static member Create<'data>(identifier: string, label: string, data: 'data) =
        { DataItem.Identifier = identifier
          DataItem.Label = label
          DataItem.Data = data }

#nowarn "957"

type DataItem< ^input> with

    static member inline Create(item: ^input) =
        let stringValue: string = (^input: (member get_StringValue: unit -> string) (item))
        let friendlyStringValue: string = (^input: (member get_FriendlyStringValue: unit -> string) (item))

        DataItem.Create< ^input>(stringValue, friendlyStringValue, item)
"""
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Indirect constraint by operator`` () =
        FSharp """
List.average [42] |> ignore
"""
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic
            (Error 1, Line 2, Col 15, Line 2, Col 17, "'List.average' does not support the type 'int', because the latter lacks the required (real or built-in) member 'DivideByInt'")

    [<Fact>]
    let ``Direct constraint by named (pseudo) operator`` () =
        FSharp """
abs -1u |> ignore
"""
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic
            (Error 1, Line 2, Col 6, Line 2, Col 8, "The type 'uint32' does not support the operator 'abs'")

    [<Fact>]
    let ``Direct constraint by simple operator`` () =
        FSharp """
"" >>> 1 |> ignore
"""
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic
            (Error 1, Line 2, Col 1, Line 2, Col 3, "The type 'string' does not support the operator '>>>'")

    [<Fact>]
    let ``Direct constraint by pseudo operator`` () =
        FSharp """
ignore ["1" .. "42"]
"""
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic
            (Error 1, Line 2, Col 9, Line 2, Col 12, "The type 'string' does not support the operator 'op_Range'")

    [<Fact>]
    let ``SRTP can resolve C# class instance field getter`` () =
        let csLib =
            CSharp """
public class Foo {
    public string Id = "hello";
}
            """
            |> withName "csLib"

        let fsApp =
            FSharp """
open System

let inline getId (x: ^T) = (^T : (member Id : string) x)

[<EntryPoint>]
let main _ =
    printf "%s" (getId (Foo()))
    0
            """
            |> asExe
            |> withLangVersionPreview
            |> withReferences [ csLib ]

        fsApp |> compileAndRun |> shouldSucceed |> verifyOutput "hello"

    [<Fact>]
    let ``SRTP can resolve C# class instance field setter`` () =
        let csLib =
            CSharp """
public class Foo {
    public string Id = "initial";
}
            """
            |> withName "csLib"

        let fsApp =
            FSharp """
open System

let inline setId (x: ^T) (v: string) = (^T : (member set_Id : string -> unit) (x, v))
let inline getId (x: ^T) = (^T : (member Id : string) x)

[<EntryPoint>]
let main _ =
    let foo = Foo()
    setId foo "mutated"
    printf "%s" (getId foo)
    0
            """
            |> asExe
            |> withLangVersionPreview
            |> withReferences [ csLib ]

        fsApp |> compileAndRun |> shouldSucceed |> verifyOutput "mutated"

    [<Fact>]
    let ``SRTP can resolve C# struct instance field getter`` () =
        let csLib =
            CSharp """
public struct Point {
    public int X;
    public int Y;
    public Point(int x, int y) { X = x; Y = y; }
}
            """
            |> withName "csLib"

        let fsApp =
            FSharp """
open System

let inline getX (x: ^T) = (^T : (member X : int) x)

[<EntryPoint>]
let main _ =
    let p = Point(3, 4)
    printf "%d" (getX p)
    0
            """
            |> asExe
            |> withLangVersionPreview
            |> withReferences [ csLib ]

        fsApp |> compileAndRun |> shouldSucceed |> verifyOutput "3"

    [<Fact>]
    let ``SRTP can resolve C# struct instance field setter`` () =
        let csLib =
            CSharp """
public struct Point {
    public int X;
    public int Y;
    public Point(int x, int y) { X = x; Y = y; }
}
            """
            |> withName "csLib"

        let fsApp =
            FSharp """
open System

let inline setX (x: ^T) (v: int) = (^T : (member set_X : int -> unit) (x, v))

[<EntryPoint>]
let main _ =
    let mutable p = Point(3, 4)
    setX p 99
    printf "ok"
    0
            """
            |> asExe
            |> withLangVersionPreview
            |> withReferences [ csLib ]

        fsApp |> compileAndRun |> shouldSucceed |> verifyOutput "ok"

    [<Fact>]
    let ``SRTP C# struct field setter on mutable binding creates defensive copy`` () =
        // On a mutable binding, inline expansion of the SRTP setter copies the struct
        // value into an immutable parameter binding. The mutation applies to the copy,
        // not the original. This is the expected struct value-type semantics for SRTP
        // inline expansion — contrast with the immutable binding test where the compiler
        // aliases instead of copying, allowing mutation to persist.
        let csLib =
            CSharp """
public struct Point {
    public int X;
    public int Y;
    public Point(int x, int y) { X = x; Y = y; }
}
            """
            |> withName "csLib"

        let fsApp =
            FSharp """
open System

let inline setX (x: ^T) (v: int) = (^T : (member set_X : int -> unit) (x, v))

[<EntryPoint>]
let main _ =
    let mutable p = Point(3, 4)
    setX p 99
    printf "%d" p.X
    0
            """
            |> asExe
            |> withLangVersionPreview
            |> withReferences [ csLib ]

        fsApp |> compileAndRun |> shouldSucceed |> verifyOutput "3"

    [<Fact>]
    let ``SRTP can resolve C# class static field getter`` () =
        let csLib =
            CSharp """
public class Counter {
    public static int Count = 42;
}
            """
            |> withName "csLib"

        let fsApp =
            FSharp """
open System

let inline getCount<'T when ^T : (static member Count : int)> () = (^T : (static member Count : int) ())

[<EntryPoint>]
let main _ =
    printf "%d" (getCount<Counter> ())
    0
            """
            |> asExe
            |> withLangVersionPreview
            |> withReferences [ csLib ]

        fsApp |> compileAndRun |> shouldSucceed |> verifyOutput "42"

    [<Fact>]
    let ``SRTP can resolve C# class static field setter`` () =
        let csLib =
            CSharp """
public class Counter {
    public static int Count = 0;
}
            """
            |> withName "csLib"

        let fsApp =
            FSharp """
open System

let inline setCount<'T when ^T : (static member set_Count : int -> unit)> (v: int) = (^T : (static member set_Count : int -> unit) v)
let inline getCount<'T when ^T : (static member Count : int)> () = (^T : (static member Count : int) ())

[<EntryPoint>]
let main _ =
    setCount<Counter> 99
    printf "%d" (getCount<Counter> ())
    0
            """
            |> asExe
            |> withLangVersionPreview
            |> withReferences [ csLib ]

        fsApp |> compileAndRun |> shouldSucceed |> verifyOutput "99"

    [<Fact>]
    let ``SRTP can resolve generic C# class field`` () =
        let csLib =
            CSharp """
public class Wrapper<T> {
    public T Value;
    public Wrapper(T val) { Value = val; }
}
            """
            |> withName "csLib"

        let fsApp =
            FSharp """
open System

let inline getValue (x: ^T) = (^T : (member Value : 'R) x)

[<EntryPoint>]
let main _ =
    let ws = Wrapper<string>("hello")
    let wi = Wrapper<int>(42)
    printf "%s %d" (getValue ws) (getValue wi)
    0
            """
            |> asExe
            |> withLangVersionPreview
            |> withReferences [ csLib ]

        fsApp |> compileAndRun |> shouldSucceed |> verifyOutput "hello 42"

    [<Fact>]
    let ``SRTP inline function works with both C# field and F# record`` () =
        let csLib =
            CSharp """
public class CSharpObj {
    public string Name = "csharp";
}
            """
            |> withName "csLib"

        let fsApp =
            FSharp """
open System

type FSharpRec = { Name: string }

let inline getName (x: ^T) = (^T : (member Name : string) x)

[<EntryPoint>]
let main _ =
    let cs = CSharpObj()
    let fs = { Name = "fsharp" }
    printf "%s %s" (getName cs) (getName fs)
    0
            """
            |> asExe
            |> withLangVersionPreview
            |> withReferences [ csLib ]

        fsApp |> compileAndRun |> shouldSucceed |> verifyOutput "csharp fsharp"

    [<Fact>]
    let ``F# record field SRTP still works with langversion preview`` () =
        let fsApp =
            FSharp """
open System

type MyRec = { Value: int }

let inline getValue (x: ^T) = (^T : (member Value : int) x)

[<EntryPoint>]
let main _ =
    let r = { Value = 123 }
    printf "%d" (getValue r)
    0
            """
            |> asExe
            |> withLangVersionPreview

        fsApp |> compileAndRun |> shouldSucceed |> verifyOutput "123"

    [<Fact>]
    let ``SRTP can resolve C# readonly field getter`` () =
        let csLib =
            CSharp """
public class Config {
    public readonly string Key = "secret";
}
            """
            |> withName "csLib"

        let fsApp =
            FSharp """
open System

let inline getKey (x: ^T) = (^T : (member Key : string) x)

[<EntryPoint>]
let main _ =
    printf "%s" (getKey (Config()))
    0
            """
            |> asExe
            |> withLangVersionPreview
            |> withReferences [ csLib ]

        fsApp |> compileAndRun |> shouldSucceed |> verifyOutput "secret"

    [<Fact>]
    let ``SRTP rejects C# readonly field setter`` () =
        let csLib =
            CSharp """
public class Config {
    public readonly string Key = "secret";
}
            """
            |> withName "csLib"

        FSharp """
open System

let inline setKey (x: ^T) (v: string) = (^T : (member set_Key : string -> unit) (x, v))

let main () =
    setKey (Config()) "changed"
        """
        |> withLangVersionPreview
        |> withReferences [ csLib ]
        |> compile
        |> shouldFail

    [<Fact>]
    let ``SRTP does not resolve C# private field`` () =
        let csLib =
            CSharp """
public class Secret {
    private string Hidden = "nope";
}
            """
            |> withName "csLib"

        FSharp """
open System

let inline getHidden (x: ^T) = (^T : (member Hidden : string) x)

let main () =
    getHidden (Secret()) |> ignore
        """
        |> withLangVersionPreview
        |> withReferences [ csLib ]
        |> compile
        |> shouldFail

    [<Fact>]
    let ``SRTP does not resolve C# const literal field`` () =
        let csLib =
            CSharp """
public class Constants {
    public const int MaxValue = 100;
}
            """
            |> withName "csLib"

        FSharp """
open System

let inline getMax<'T when ^T : (static member MaxValue : int)> () = (^T : (static member MaxValue : int) ())

let main () =
    getMax<Constants> () |> ignore
        """
        |> withLangVersionPreview
        |> withReferences [ csLib ]
        |> compile
        |> shouldFail

    [<Fact>]
    let ``SRTP resolves property and field with same inline function`` () =
        let csLib =
            CSharp
                """
public class HasProperty {
    public string Id { get { return "property"; } }
}

public class HasField {
    public string Id = "field";
}
            """
            |> withName "csLib"

        FSharp
            """
open System

let inline getId (x: ^T) = (^T : (member Id : string) x)

[<EntryPoint>]
let main _ =
    printf "%s,%s" (getId (HasProperty())) (getId (HasField()))
    0
            """
        |> asExe
        |> withLangVersionPreview
        |> withReferences [ csLib ]
        |> compileAndRun
        |> shouldSucceed
        |> verifyOutput "property,field"

    [<Fact>]
    let ``SRTP method wins over field when both exist on same type`` () =
        let csLib =
            CSharp
                """
public class HasBoth {
    public int Value = 100;
    public int get_Value() { return 42; }
}
            """
            |> withName "csLib"

        FSharp
            """
open System

let inline getValue (x: ^T) = (^T : (member get_Value : unit -> int) x)

[<EntryPoint>]
let main _ =
    printf "%d" (getValue (HasBoth()))
    0
            """
        |> asExe
        |> withLangVersionPreview
        |> withReferences [ csLib ]
        |> compileAndRun
        |> shouldSucceed
        |> verifyOutput "42"

    [<Fact>]
    let ``SRTP field type mismatch causes error`` () =
        let csLib =
            CSharp """
public class Foo {
    public string Name = "hello";
}
            """
            |> withName "csLib"

        FSharp """
open System

let inline getName (x: ^T) = (^T : (member Name : int) x)

let main () =
    getName (Foo()) |> ignore
        """
        |> withLangVersionPreview
        |> withReferences [ csLib ]
        |> compile
        |> shouldFail

    [<Fact>]
    let ``SRTP does not resolve special-name field`` () =
        // C# enum's value__ field is public but marked specialname+rtspecialname in IL.
        // The solver's fieldSearch must filter it out via `not ilfinfo.IsSpecialName`.
        let csLib =
            CSharp """
public enum Color {
    Red = 1,
    Green = 2,
    Blue = 3
}
            """
            |> withName "csLib"

        FSharp """
open System

let inline getUnderlying (x: ^T) = (^T : (member value__ : int) x)

let main () =
    getUnderlying Color.Red |> ignore
        """
        |> withLangVersionPreview
        |> withReferences [ csLib ]
        |> compile
        |> shouldFail

    [<Fact>]
    let ``SRTP C# struct field setter on immutable binding mutates value`` () =
        // Due to how SRTP inline expansion works, setting a C# struct field via
        // SRTP mutates the original value even on an immutable binding. This happens
        // because: (1) C# structs with mutable fields are incorrectly assumed
        // "readonly" by isRecdOrUnionOrStructTyconRefDefinitelyMutable (which only
        // checks F# fields), causing CanTakeAddressOfImmutableVal to succeed, and
        // (2) the readonly flag returned by mkExprAddrOfExpr is ignored in the
        // ILFieldSln codegen. This test documents the current behavior.
        let csLib =
            CSharp """
public struct Point {
    public int X;
    public int Y;
    public Point(int x, int y) { X = x; Y = y; }
}
            """
            |> withName "csLib"

        let fsApp =
            FSharp """
open System

let inline setX (x: ^T) (v: int) = (^T : (member set_X : int -> unit) (x, v))
let inline getX (x: ^T) : int = (^T : (member X : int) x)

[<EntryPoint>]
let main _ =
    let p = Point(3, 4)
    setX p 99
    printf "%d" (getX p)
    0
            """
            |> asExe
            |> withLangVersionPreview
            |> withReferences [ csLib ]

        fsApp |> compileAndRun |> shouldSucceed |> verifyOutput "99"
