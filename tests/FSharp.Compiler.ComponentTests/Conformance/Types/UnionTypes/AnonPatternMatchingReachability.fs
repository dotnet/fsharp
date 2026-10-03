// Anonymous Union Types
// Reachable and unknown type-test relationships must not be reported as unreachable

let valueType (x: (int|string)) =
    match x with
    | :? System.ValueType -> 0
    | :? string -> 1

let partialValueType (x: (System.ValueType|string)) =
    match x with
    | :? System.Enum -> 0
    | :? string -> 1
    | _ -> 2

type Base() = class end
type IResource =
    abstract member Use: unit -> unit

type Derived() =
    inherit Base()
    interface IResource with
        member _.Use() = ()

let unknownClass (x: (Base|string)) =
    match x with
    | :? IResource -> 0
    | _ -> 1

let unknownAfterTypeTest (x: (Base|string)) =
    match x with
    | :? Base -> 0
    | :? IResource -> 1
    | _ -> 2

type Left = interface end
type Right = interface end
type LeftImpl() = interface Left

let unknownInterfaces (x: (Left|Right)) =
    match x with
    | :? IResource -> 0
    | _ -> 1

let unknownGeneric<'T> (x: ('T|string)) =
    match x with
    | :? System.ValueType -> 0
    | _ -> 1

let unknownFlexible<'T when 'T :> Base> (x: ('T|string)) =
    match x with
    | :? IResource -> 0
    | _ -> 1

type Erased<'T> = 'T

let unknownErased<'T> (x: (Erased<'T>|string)) =
    match x with
    | :? System.ValueType -> 0
    | _ -> 1

let unknownNullness (x: (IResource|string|null)) =
    match x with
    | :? System.ICloneable -> 0
    | null -> 1
    | _ -> 2

if not (valueType 42 = 0) then failwith "Test failed"
if not (valueType "hello" = 1) then failwith "Test failed"
if not (partialValueType System.DayOfWeek.Monday = 0) then failwith "Test failed"
if not (partialValueType "hello" = 1) then failwith "Test failed"
if not (unknownClass (Derived()) = 0) then failwith "Test failed"
if not (unknownClass "hello" = 1) then failwith "Test failed"
if not (unknownAfterTypeTest (Derived()) = 0) then failwith "Test failed"
if not (unknownAfterTypeTest "hello" = 2) then failwith "Test failed"
if not (unknownInterfaces (LeftImpl()) = 1) then failwith "Test failed"
if not (unknownGeneric 42 = 0) then failwith "Test failed"
if not (unknownGeneric (Derived()) = 1) then failwith "Test failed"
if not (unknownFlexible (Derived()) = 0) then failwith "Test failed"
if not (unknownErased 42 = 0) then failwith "Test failed"
if not (unknownErased (Derived()) = 1) then failwith "Test failed"
if not (unknownNullness null = 1) then failwith "Test failed"
