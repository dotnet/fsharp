namespace Equality

open System
open BenchmarkDotNet.Attributes

[<MemoryDiagnoser>]
type OptionsAndCo() =

    let numbers = Array.init 1000 id

    let createOption x = 
        match x with
        | x when x % 2 = 0 -> Some x
        | _ -> None

    let createValueOption x = 
        match x with
        | x when x % 2 = 0 -> ValueSome x
        | _ -> ValueNone

    let createResult x = 
        match x with
        | x when x % 2 = 0 -> Ok x
        | x -> Error x

    let createNullable x = 
        match x with
        | x when x % 2 = 0 -> Nullable x
        | _ -> Nullable 42

    [<Benchmark>]
    member _.Option() = 
        numbers |> Array.countBy createOption

    [<Benchmark>]
    member _.ValueOption() = 
        numbers |> Array.countBy createValueOption

    [<Benchmark>]
    member _.Result() = 
        numbers |> Array.countBy createResult

    [<Benchmark>]
    member _.Nullable() = 
        numbers |> Array.countBy createNullable
