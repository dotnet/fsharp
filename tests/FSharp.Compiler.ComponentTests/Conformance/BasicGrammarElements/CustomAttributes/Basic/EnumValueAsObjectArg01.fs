// #Conformance #DeclarationElements #Attributes
// Regression test for https://github.com/dotnet/fsharp/issues/995
// An enum assigned to an attribute argument of type 'obj' must keep its enum type in the
// emitted metadata, instead of being stored as the underlying int32.

open System

type MyAttribute() =
    inherit Attribute()
    let mutable prop : obj = null
    member _.Prop
        with get () : obj = prop
        and  set (value: obj) = prop <- value

type MyEnum =
    | A = 1
    | B = 2

// An enum with a non-int32 underlying type, to exercise the encoded value width.
// 'Big' exceeds Int32.MaxValue so a truncating decode would corrupt the value.
type LongEnum =
    | P = 1L
    | Q = 2L
    | Big = 5_000_000_000L

[<My(Prop = MyEnum.B)>]
type MyClass = class end

[<My(Prop = LongEnum.Q)>]
type MyClassLong = class end

[<My(Prop = LongEnum.Big)>]
type MyClassBig = class end

let propOf<'T> () = (typeof<'T>.GetCustomAttributes(false)[0] :?> MyAttribute).Prop

let intProp = propOf<MyClass> ()
if intProp.GetType() <> typeof<MyEnum> then failwith "MyEnum type was lost"
if Convert.ToString(intProp, Globalization.CultureInfo.InvariantCulture) <> "B" then failwith "expected \"B\""

let longProp = propOf<MyClassLong> ()
if longProp.GetType() <> typeof<LongEnum> then failwith "LongEnum type was lost"
if Convert.ToString(longProp, Globalization.CultureInfo.InvariantCulture) <> "Q" then failwith "expected \"Q\""

let bigProp = propOf<MyClassBig> ()
if bigProp.GetType() <> typeof<LongEnum> then failwith "LongEnum type was lost"
if Convert.ToInt64(bigProp, Globalization.CultureInfo.InvariantCulture) <> 5_000_000_000L then failwith "expected 5_000_000_000L"
