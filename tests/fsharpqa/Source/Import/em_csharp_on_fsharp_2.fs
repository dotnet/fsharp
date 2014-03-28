// #Import 
// Feature test for Bug51669
// Make sure F# can import C# extension methods on F# types

module M

open BaseEmFs
open EmLibCs

let foo = FooB(10)
foo.M1B() |> ignore
foo.M2B("hello") |> ignore
foo.M3B(5) |> ignore

