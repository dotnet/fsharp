// Anonymous Union Types
// Regression test for common ancestor remapping

type C<'T>(value: 'T) =
    member _.Value = value

type A<'T>(value: 'T) =
    inherit C<'T>(value)

type B<'T>(value: 'T) =
    inherit C<'T>(value)

let f<'T> (x: (A<'T> | B<'T>)) = x
let a = A<int>(42)
f a |> ignore