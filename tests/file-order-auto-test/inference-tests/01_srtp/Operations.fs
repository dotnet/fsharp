module SrtpTest.Operations

open SrtpTest.Types

/// SRTP: inline function that works on any type with (+) and Zero
let inline sum (items: ^a list) : ^a =
    items |> List.fold (fun acc x -> acc + x) LanguagePrimitives.GenericZero

let inline dot (a: Vector2D) (b: Vector2D) =
    a.X * b.X + a.Y * b.Y
