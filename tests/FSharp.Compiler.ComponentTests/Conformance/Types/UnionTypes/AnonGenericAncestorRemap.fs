// Anonymous Union Types
// Regression test for common ancestor remapping

let f<'T> (x: (System.Collections.Generic.IEnumerable<'T> | 'T list)) = x
f<int> [1;2;3] |> ignore