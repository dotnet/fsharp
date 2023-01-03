[<RequireQualifiedAccess>]
module internal Continuation

let rec sequence<'a, 'ret> (recursions: (('a -> 'ret) -> 'ret) list) (finalContinuation: 'a list -> 'ret) : 'ret =
    match recursions with
    | [] -> [] |> finalContinuation
    | recurse :: recurses -> recurse (fun ret -> sequence recurses (fun rets -> ret :: rets |> finalContinuation))
