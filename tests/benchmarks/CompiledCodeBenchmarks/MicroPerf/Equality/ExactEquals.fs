namespace Equality

open BenchmarkDotNet.Attributes

[<Struct>]
type Struct(v: int, u: int) =
    member _.V = v
    member _.U = u

[<Struct>]
type Union = Union of int * int

[<Struct>]
type Record = { V: int; U: int }

[<Struct>]
type GenericStruct<'T>(v: 'T, u: int) =
    member _.V = v
    member _.U = u

[<Struct>]
type GenericRecord<'T> = { V: 'T; U: int }

[<Struct>]
type GenericUnion<'T> = GenericUnion of 'T * int

[<Struct>]
type TinyStruct(v: int) =
    member _.V = v

[<Struct>]
type TinyUnion = TinyUnion of int

[<Struct>]
type TinyRecord = { V: int }

[<MemoryDiagnoser>]
type ExactEquals_EqualityTests() =
    
    [<Benchmark>]
    member _.Struct() =
        Struct(1, 2) = Struct(2, 3)

    [<Benchmark>]
    member _.StructUnion() =
        Union (1, 2) = Union (2, 3)

    [<Benchmark>]
    member _.StructRecord() =
        { V = 1; U = 2 } = { V = 2; U = 3 }
    
    [<Benchmark>]
    member _.GenericStruct() =
        Struct(1, 2) = Struct(2, 3)

    [<Benchmark>]
    member _.GenericStructUnion() =
        GenericUnion (1, 2) = GenericUnion (2, 3)

    [<Benchmark>]
    member _.GenericStructRecord() =
        { V = 1; U = 2 } = { V = 2; U = 3 } 

    [<Benchmark>]
    member _.TinyStruct() =
        TinyStruct 1 = TinyStruct 2

    [<Benchmark>]
    member _.TinyStructUnion() =
        TinyUnion 1 = TinyUnion 2

    [<Benchmark>]
    member _.TinyStructRecord() =
        { V = 1 } = { V = 2 }

[<MemoryDiagnoser>]
type ExactEquals_Fslib() =

    [<Benchmark>]
    member _.ValueOption_Some() =
        ValueSome (1, 2) = ValueSome (2, 3)

    [<Benchmark>]
    member _.ValueOption_None() =
        ValueNone = ValueNone

    [<Benchmark>]
    member _.Result_Ok() =
        Ok (1, 2) = Ok (2, 3)

    [<Benchmark>]
    member _.Result_Error() =
        Error (1, 2) = Error (2, 3)

[<MemoryDiagnoser>]
type ExactEquals_Arrays() =

    let array = Array.init 1000 (fun n -> Struct(n, n))
    let existingElement = Struct (1, 1)
    let nonexistingElement = Struct (-1, -1)

    [<Benchmark>]
    member _.ArrayContainsExisting() =
        array |> Array.contains existingElement

    [<Benchmark>]
    member _.ArrayContainsNonexisting() =
        array |> Array.contains nonexistingElement

    [<Benchmark>]
    member _.ArrayExistsExisting() =
        array |> Array.exists ((=) existingElement)

    [<Benchmark>]
    member _.ArrayExistsNonexisting() =
        array |> Array.exists ((=) nonexistingElement)

    [<Benchmark>]
    member _.ArrayTryFindExisting() =
        array |> Array.tryFind ((=) existingElement)

    [<Benchmark>]
    member _.ArrayTryFindNonexisting() =
        array |> Array.tryFind ((=) nonexistingElement)

    [<Benchmark>]
    member _.ArrayTryFindIndexExisting() =
        array |> Array.tryFindIndex ((=) existingElement)

    [<Benchmark>]
    member _.ArrayTryFindIndexNonexisting() =
        array |> Array.tryFindIndex ((=) nonexistingElement)
