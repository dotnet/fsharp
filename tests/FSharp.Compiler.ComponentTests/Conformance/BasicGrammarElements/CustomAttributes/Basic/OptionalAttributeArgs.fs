// #Regression #Conformance #DeclarationElements #Attributes
// Regression test for https://github.com/dotnet/fsharp/issues/8353
// Verify that custom attributes with [<Optional>] parameters (no DefaultParameterValue) compile for all value types
// and that the emitted default values are correct at runtime.

open System
open System.Runtime.InteropServices

type BoolAttribute(name: string, flag: bool) =
    inherit Attribute()
    member _.Flag = flag
    new([<Optional>] flag: bool) = BoolAttribute("", flag)

type IntAttribute(name: string, value: int) =
    inherit Attribute()
    member _.Value = value
    new([<Optional>] value: int) = IntAttribute("", value)

type ByteAttribute(name: string, value: byte) =
    inherit Attribute()
    member _.Value = value
    new([<Optional>] value: byte) = ByteAttribute("", value)

type SByteAttribute(name: string, value: sbyte) =
    inherit Attribute()
    member _.Value = value
    new([<Optional>] value: sbyte) = SByteAttribute("", value)

type Int16Attribute(name: string, value: int16) =
    inherit Attribute()
    member _.Value = value
    new([<Optional>] value: int16) = Int16Attribute("", value)

type Int64Attribute(name: string, value: int64) =
    inherit Attribute()
    member _.Value = value
    new([<Optional>] value: int64) = Int64Attribute("", value)

type UInt16Attribute(name: string, value: uint16) =
    inherit Attribute()
    member _.Value = value
    new([<Optional>] value: uint16) = UInt16Attribute("", value)

type UInt32Attribute(name: string, value: uint32) =
    inherit Attribute()
    member _.Value = value
    new([<Optional>] value: uint32) = UInt32Attribute("", value)

type UInt64Attribute(name: string, value: uint64) =
    inherit Attribute()
    member _.Value = value
    new([<Optional>] value: uint64) = UInt64Attribute("", value)

type FloatAttribute(name: string, value: float) =
    inherit Attribute()
    member _.Value = value
    new([<Optional>] value: float) = FloatAttribute("", value)

type SingleAttribute(name: string, value: float32) =
    inherit Attribute()
    member _.Value = value
    new([<Optional>] value: float32) = SingleAttribute("", value)

type CharAttribute(name: string, value: char) =
    inherit Attribute()
    member _.Value = value
    new([<Optional>] value: char) = CharAttribute("", value)

[<Bool>]
type T1() = class end

[<Int>]
type T2() = class end

[<Byte>]
type T3() = class end

[<Float>]
type T4() = class end

[<Single>]
type T5() = class end

[<Char>]
type T6() = class end

[<SByte>]
type T7() = class end

[<Int16>]
type T8() = class end

[<Int64>]
type T9() = class end

[<UInt16>]
type T10() = class end

[<UInt32>]
type T11() = class end

[<UInt64>]
type T12() = class end

// Verify default values at runtime via reflection
let inline getAttr<'a when 'a :> Attribute> (t: Type) = t.GetCustomAttributes(typeof<'a>, false).[0] :?> 'a

let check (name: string) (actual: 'a) (expected: 'a) =
    if actual <> expected then
        failwithf "%s: expected %A but got %A" name expected actual

[<EntryPoint>]
let main _ =
    check "bool" (getAttr<BoolAttribute>(typeof<T1>)).Flag false
    check "int" (getAttr<IntAttribute>(typeof<T2>)).Value 0
    check "byte" (getAttr<ByteAttribute>(typeof<T3>)).Value 0uy
    check "float" (getAttr<FloatAttribute>(typeof<T4>)).Value 0.0
    check "single" (getAttr<SingleAttribute>(typeof<T5>)).Value 0.0f
    check "char" (getAttr<CharAttribute>(typeof<T6>)).Value '\000'
    check "sbyte" (getAttr<SByteAttribute>(typeof<T7>)).Value 0y
    check "int16" (getAttr<Int16Attribute>(typeof<T8>)).Value 0s
    check "int64" (getAttr<Int64Attribute>(typeof<T9>)).Value 0L
    check "uint16" (getAttr<UInt16Attribute>(typeof<T10>)).Value 0us
    check "uint32" (getAttr<UInt32Attribute>(typeof<T11>)).Value 0u
    check "uint64" (getAttr<UInt64Attribute>(typeof<T12>)).Value 0UL
    0

