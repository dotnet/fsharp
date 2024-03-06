namespace Equality

open BenchmarkDotNet.Attributes

type SmallNonGenericTuple = SmallNonGenericTuple of int * string

type SmallGenericTuple<'a> = SmallGenericTuple of int * 'a

type BigNonGenericTuple = BigNonGenericTuple of int * string * byte * int * string * byte

type BigGenericTuple<'a> = BigGenericTuple of int * 'a * byte * int * 'a * byte

[<Struct>]
type SmallNonGenericTupleStruct = SmallNonGenericTupleStruct of int * string

[<Struct>]
type SmallGenericTupleStruct<'a> = SmallGenericTupleStruct of int * 'a

[<Struct>]
type BigNonGenericTupleStruct = BigNonGenericTupleStruct of int * string * byte * int * string * byte

[<Struct>]
type BigGenericTupleStruct<'a> = BigGenericTupleStruct of int * 'a * byte * int * 'a * byte

type ReferenceTuples() =

    let numbers = Array.init 1000 id

    [<Benchmark>]
    member _.SmallNonGenericTuple() =
        numbers
        |> Array.countBy (fun n -> SmallNonGenericTuple(n, string n))

    [<Benchmark>]
    member _.SmallGenericTuple() =
        numbers
        |> Array.countBy (fun n -> SmallGenericTuple(n, string n))

    [<Benchmark>]
    member _.BigNonGenericTuple() =
        numbers
        |> Array.countBy (fun n -> BigNonGenericTuple(n, string n, byte n, n, string n, byte n))

    [<Benchmark>]
    member _.BigGenericTuple() =
        numbers
        |> Array.countBy (fun n -> BigGenericTuple(n, string n, byte n, n, string n, byte n))

    [<Benchmark>]
    member _.SmallNonGenericTupleStruct() =
        numbers
        |> Array.countBy (fun n -> SmallNonGenericTupleStruct(n, string n))

    [<Benchmark>]
    member _.SmallGenericTupleStruct() =
        numbers
        |> Array.countBy (fun n -> SmallGenericTupleStruct(n, string n))

    [<Benchmark>]
    member _.BigNonGenericTupleStruct() =
        numbers
        |> Array.countBy (fun n -> BigNonGenericTupleStruct(n, string n, byte n, n, string n, byte n))

    [<Benchmark>]
    member _.BigGenericTupleStruct() =
        numbers
        |> Array.countBy (fun n -> BigGenericTupleStruct(n, string n, byte n, n, string n, byte n))
