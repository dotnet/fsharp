namespace Equality

open System
open BenchmarkDotNet.Attributes

type SomeEnum = 
    | Case0 = 0
    | Case1 = 1
    | Case2 = 2

[<MemoryDiagnoser>]
type ValueTypes() =

    let numbers = Array.init 1000 id
    let now = DateTimeOffset.Now

    let createFSharpStruct (x: int) = 
        struct (x % 7, x % 7)

    let createFSharpEnum (x: int) =
        enum<SomeEnum>(x % 3)

    let createCSharpStruct (x: int) = 
        now.AddMinutes x

    let createCSharpEnum (x: int) =
        enum<System.DayOfWeek>(x % 7)

    [<Benchmark>]
    member _.FSharpStruct() = numbers |> Array.countBy createFSharpStruct

    [<Benchmark>]
    member _.FSharpEnum() = numbers |> Array.countBy createFSharpEnum

    [<Benchmark>]
    member _.CSharpStruct() = numbers |> Array.countBy createCSharpStruct

    [<Benchmark>]
    member _.CSharpEnum() = numbers |> Array.countBy createCSharpEnum
