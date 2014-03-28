// #Import 
// Feature test for Bug51669
// Make sure F# can import C# extension methods on F# types

module M

open BaseEmFs
open EmLibCs

let foo = FooA()
foo.M1A() |> ignore
foo.M2A("hello") |> ignore
foo.M3A(5) |> ignore
