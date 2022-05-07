module Neg133
open System
type T =
    static member (y) = 0

type U =
    static member (y: int) = 0

type A() = class end

type B1() =
    inherit A()

type B2() =
    inherit A()

[<Sealed>]
type C1() =
    inherit B1()

[<Sealed>]
type C2() =
    inherit B2()

let TestOneColumnOfTypeTestsWithUnSealedClassTypes_Redundant1(x: obj) =
    match x with
    | :? A -> 1
    | :? B1 -> 2 // expect - never matched 
    | _ -> 3

let TestOneColumnOfTypeTestsWithUnSealedClassTypes_Redundant2(x: obj) =
    match x with
    | :? A -> 1
    | :? C1 -> 2 // expect - never matched 
    | _ -> 3

let TestOneColumnOfTypeTestsWithUnSealedClassTypes_Redundant3(x: obj) =
    match x with
    | :? B1 -> 1
    | :? C1 -> 2 // expect - never matched 
    | _ -> 3

let TestColumnOfTypeTestsWithNullTrueValue_Redundant(x: obj) =
    match x with
    | :? option<int> -> 0x200
    | null -> 0x100  // expect - never matched
    | _ -> 0x500

let Misc_Redundant2(x: ValueType) =
    match x with
    | a -> 2
    | :? Enum & (:? ConsoleKey) -> 1  // expect - never matched

let Misc_Redundant3(x: ValueType) =
    match x with
    | :? Enum -> 2
    | :? Enum -> 1  // expect - never matched
    | g -> 3

let Misc_Redundant4(x: obj) =
    match x with
    | :? ValueType -> 1
    | :? Enum -> 2  // expect - never matched
    | g -> 3

let Misc_Redundant5(x: obj) =
    match x with
    | :? IComparable -> 1  
    | :? string -> 2  // expect - never matched
    | g -> 3

module AmbiguousIntrinsicExtension =
    type A<'T1, 'T2> = class end
    type A<'T1, 'T2, 'T3> = class end

    type A<'T1> with member x.Foo = 123