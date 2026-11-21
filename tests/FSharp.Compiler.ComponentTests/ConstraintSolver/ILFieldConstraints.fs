// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace ConstraintSolver

open Xunit
open FSharp.Test.Compiler

// https://github.com/fsharp/fslang-suggestions/issues/1323
module ILFieldConstraints =

    let private csLib =
        CSharp
            """
namespace CsLib
{
    public class Foo { public string Id = "hello"; }
    public class Counter { public static int Count = 42; }
    public struct Point
    {
        public int X;
        public int Y;
        public Point(int x, int y) { X = x; Y = y; }
    }
    public class Wrapper<T>
    {
        public T Value;
        public Wrapper(T value) { Value = value; }
    }
    public class Base { public string Name = "base"; }
    public class Derived : Base { }
    public class GenericBase<T> { public T Data; }
    public class IntDerived : GenericBase<int> { public IntDerived() { Data = 7; } }
    public class Config
    {
        public readonly string Key = "secret";
        public const int Max = 100;
        public static readonly string Empty = "empty";
    }
    public class Secret { private string Hidden = "nope"; internal string Inner = "inner"; }
    public class HasProperty { public string Id { get { return "property"; } } }
    public class BaseWithIdField { public string Id = "field"; }
    public class HidesIdFieldWithProperty : BaseWithIdField { public new string Id { get { return "property"; } } }
    public class HasBoth
    {
        public int Value = 100;
        public int get_Value() { return 42; }
    }
    public class VolatileHolder { public volatile int Flag; }
    public class Legacy { [System.Obsolete("Use New")] public int Old = 1; }
    public enum Color { Red = 1, Green = 2 }
}
"""
        |> withName "CsLib"

    let private fsWithCsLib source =
        FSharp source
        |> withLangVersionPreview
        |> withReferences [ csLib ]

    let private runWithCsLib source =
        fsWithCsLib source
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Instance and static fields solve getter and setter constraints`` () =
        runWithCsLib
            """
open CsLib

let inline getId (x: ^T) = (^T: (member Id: string) x)
let inline setId (x: ^T) (v: string) = (^T: (member set_Id: string -> unit) (x, v))
let inline setIdProp (x: ^T) (v: string) = (^T: (member Id: string with set) (x, v))
let inline getCount<'T when 'T: (static member Count: int)> () = 'T.Count
let inline setCount<^T when ^T: (static member set_Count: int -> unit)> (v: int) = (^T: (static member set_Count: int -> unit) v)

let foo = Foo()
let before = getId foo
setId foo "mutated"
let afterSet = getId foo
setIdProp foo "property-syntax"
let before2 = getCount<Counter> ()
setCount<Counter> 99
printf "%s %s %s %d %d" before afterSet (getId foo) before2 (getCount<Counter> ())
"""
        |> verifyOutput "hello mutated property-syntax 42 99"

    [<Fact>]
    let ``Struct, generic and inherited fields solve getter constraints`` () =
        runWithCsLib
            """
open CsLib

let inline getX (x: ^T) = (^T: (member X: int) x)
let inline getValue (x: ^T) = (^T: (member Value: 'R) x)
let inline getName (x: ^T) = (^T: (member Name: string) x)
let inline getData (x: ^T) = (^T: (member Data: int) x)

printf "%d %s %d %s %d" (getX (Point(3, 4))) (getValue (Wrapper "w")) (getValue (Wrapper 5)) (getName (Derived())) (getData (IntDerived()))
"""
        |> verifyOutput "3 w 5 base 7"

    [<Fact>]
    let ``Readonly, static readonly and const fields solve getter constraints`` () =
        runWithCsLib
            """
open CsLib

let inline getKey (x: ^T) = (^T: (member Key: string) x)
let inline getMax<'T when 'T: (static member Max: int)> () = 'T.Max
let inline getEmpty<'T when 'T: (static member Empty: string)> () = 'T.Empty
let inline maxValue<'T when 'T: (static member MaxValue: 'T)> () = 'T.MaxValue
let inline red<'T when 'T: (static member Red: 'T)> () = 'T.Red

printf "%s %d %s %d %d %M %A" (getKey (Config())) (getMax<Config> ()) (getEmpty<Config> ()) (maxValue<int> ()) (maxValue<byte> ()) (maxValue<decimal> ()) (red<Color> ())
"""
        |> verifyOutput "secret 100 empty 2147483647 255 79228162514264337593543950335 Red"

    [<Fact>]
    let ``The same inline function works with fields, properties and F# records`` () =
        runWithCsLib
            """
open CsLib

type FSharpRec = { Id: string }

let inline getId (x: ^T) = (^T: (member Id: string) x)

printf "%s %s %s" (getId (Foo())) (getId (HasProperty())) (getId { Id = "record" })
"""
        |> verifyOutput "hello property record"

    [<Fact>]
    let ``Methods and properties win over fields of the same name`` () =
        runWithCsLib
            """
open CsLib

let inline getValue (x: ^T) = (^T: (member get_Value: unit -> int) x)
let inline getId (x: ^T) = (^T: (member Id: string) x)

printf "%d %s" (getValue (HasBoth())) (getId (HidesIdFieldWithProperty()))
"""
        |> verifyOutput "42 property"

    // https://github.com/dotnet/fsharp/issues/8899
    [<Fact>]
    let ``Field setter works on an object returned by a function`` () =
        runWithCsLib
            """
open CsLib

let inline setId (x: ^T) (v: string) = (^T: (member Id: string with set) (x, v))

let foo = Foo()
let set (get: unit -> Foo) v = setId (get ()) v
set (fun () -> foo) "via-function"
setId (id foo) (foo.Id + "-and-id")
printf "%s" foo.Id
"""
        |> verifyOutput "via-function-and-id"

    [<Theory>]
    [<InlineData("Point(3, 4)")>]
    [<InlineData("FSharpPoint()")>]
    let ``Struct field setter is rejected like an F# struct field setter`` (construction: string) =
        fsWithCsLib
            $"""
module M
open CsLib

[<Struct>]
type FSharpPoint =
    [<DefaultValue>] val mutable X: int

let inline setX (x: ^T) (v: int) = (^T: (member set_X: int -> unit) (x, v))

let mutable p = {construction}
setX p 99
"""
        |> asLibrary
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 256, Line 12, Col 1, Line 12, Col 10, "A value must be mutable in order to mutate the contents or take the address of a value type, e.g. 'let mutable x = ...'")

    [<Theory>]
    [<InlineData(true)>]
    [<InlineData(false)>]
    let ``Inline functions whose constraints fields solve can be used from another assembly`` (optimize: bool) =
        let fsLib =
            FSharp
                """
module FsLib
open CsLib

let inline getValue (x: ^T) = (^T: (member Value: 'R) x)
let inline getWrapped (w: Wrapper<'a>) = getValue w
let inline getId (x: ^T) = (^T: (member Id: string) x)
let inline getFooId (foo: Foo) = getId foo
"""
            |> withName "FsLib"
            |> withLangVersionPreview
            |> withOptimization optimize
            |> withReferences [ csLib ]

        FSharp
            """
open CsLib

printf "%s %d" (FsLib.getFooId (Foo())) (FsLib.getWrapped (Wrapper 3))
"""
        |> withReferences [ csLib; fsLib ]
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> verifyOutput "hello 3"

    [<Fact>]
    let ``Field solutions are used by witnesses of quotations`` () =
        runWithCsLib
            """
open CsLib
open FSharp.Quotations.Patterns

let inline getId (x: ^T) = (^T: (member Id: string) x)

match <@ getId (Foo()) @> with
| CallWithWitnesses(_, _, _, [ witness ], _) -> printf "%A" witness
| q -> printf "unexpected %A" q
"""
        |> verifyOutput "Lambda (arg0_0, FieldGet (Some (arg0_0), Id))"

    [<Fact>]
    let ``Volatile fields are accessed with the volatile prefix`` () =
        fsWithCsLib
            """
module M
open CsLib

let inline getFlag (x: ^T) = (^T: (member Flag: int) x)
let inline setFlag (x: ^T) (v: int) = (^T: (member set_Flag: int -> unit) (x, v))

let roundTrip (h: VolatileHolder) =
    setFlag h 1
    getFlag h
"""
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> verifyILContains
            [ """
      IL_0002:  volatile.
      IL_0004:  stfld      int32 modreq([runtime]System.Runtime.CompilerServices.IsVolatile) [CsLib]CsLib.VolatileHolder::Flag
      IL_0009:  ldarg.0
      IL_000a:  volatile.
      IL_000c:  ldfld      int32 modreq([runtime]System.Runtime.CompilerServices.IsVolatile) [CsLib]CsLib.VolatileHolder::Flag
      IL_0011:  ret""" ]

    [<Fact>]
    let ``Obsolete fields report a warning`` () =
        fsWithCsLib
            """
module M
open CsLib

let inline getOld (x: ^T) = (^T: (member Old: int) x)
let old = getOld (Legacy())
"""
        |> asLibrary
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Warning 44, Line 6, Col 19, Line 6, Col 27, "This construct is deprecated. Use New")

    [<Fact>]
    let ``Setting a readonly field reports a dedicated error`` () =
        fsWithCsLib
            """
module M
open CsLib

let inline setKey (x: ^T) (v: string) = (^T: (member set_Key: string -> unit) (x, v))
let inline setMax<^T when ^T: (static member set_Max: int -> unit)> (v: int) = (^T: (static member set_Max: int -> unit) v)

setKey (Config()) "changed"
setMax<Config> 1
"""
        |> asLibrary
        |> compile
        |> shouldFail
        |> withDiagnostics
            [ (Error 193, Line 8, Col 9, Line 8, Col 17, "The type 'Config' has a field 'Key', but the field is read-only")
              (Error 1, Line 9, Col 1, Line 9, Col 7, "The type 'Config' has a field 'Max', but the field is read-only") ]

    [<Fact>]
    let ``Inaccessible, special-name and mismatched fields do not solve constraints`` () =
        fsWithCsLib
            """
module M
open CsLib

let inline getHidden (x: ^T) = (^T: (member Hidden: string) x)
let inline getInner (x: ^T) = (^T: (member Inner: string) x)
let inline getUnderlying (x: ^T) = (^T: (member value__: int) x)
let inline getIdAsInt (x: ^T) = (^T: (member Id: int) x)
let inline getStaticId<'T when 'T: (static member Id: string)> () = 'T.Id

getHidden (Secret()) |> ignore
getInner (Secret()) |> ignore
getUnderlying Color.Red |> ignore
getIdAsInt (Foo()) |> ignore
getStaticId<Foo> () |> ignore
"""
        |> asLibrary
        |> compile
        |> shouldFail
        |> withDiagnostics
            [ (Error 193, Line 11, Col 12, Line 11, Col 20, "The type 'Secret' does not support the operator 'get_Hidden'")
              (Error 193, Line 12, Col 11, Line 12, Col 19, "The type 'Secret' does not support the operator 'get_Inner'")
              (Error 1, Line 13, Col 15, Line 13, Col 24, "The type 'Color' does not support the operator 'get_value__'")
              (Error 193, Line 14, Col 13, Line 14, Col 18, "The type 'int' does not match the type 'string'")
              (Error 1, Line 15, Col 1, Line 15, Col 12, "The type 'Foo' does not support the operator 'get_Id'") ]

    [<Fact>]
    let ``Fields do not solve constraints without the preview language version`` () =
        FSharp
            """
module M
open CsLib

let inline getId (x: ^T) = (^T: (member Id: string) x)
getId (Foo()) |> ignore
"""
        |> withLangVersion10
        |> withReferences [ csLib ]
        |> asLibrary
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 193, Line 6, Col 8, Line 6, Col 13, "The type 'Foo' does not support the operator 'get_Id'")