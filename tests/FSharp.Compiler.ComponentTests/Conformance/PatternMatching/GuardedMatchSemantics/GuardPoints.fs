module Test

let guard limit (values: int[]) = values[0] < limit
type Context = C0 of int[] | C1 | C2 | C3
let (|Observe|) values = values

let run token stack =
    match token, stack with
    | 0, C0 values :: tail when guard -1 values -> tail.Length
    | _, C0 (Observe values) :: (_ :: _) when guard 0 values -> 0
    | _, C0 (Observe values) :: (_ :: _) when guard 1 values -> 1
    | 1, _ :: tail -> tail.Length
    | 2, _ :: tail -> tail.Length + 1
    | 3, _ :: tail -> tail.Length + 2
    | (4 | 5), _ :: tail when token >= 0 -> tail.Length + 3
    | _ -> -1
