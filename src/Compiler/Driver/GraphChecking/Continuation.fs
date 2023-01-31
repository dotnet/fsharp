[<RequireQualifiedAccess>]
module internal Continuation

let rec sequence<'T, 'TReturn> (recursions: (('T -> 'TReturn) -> 'TReturn) list) (finalContinuation: 'T list -> 'TReturn) : 'TReturn =
    match recursions with
    | [] -> finalContinuation []
    | andThenInner :: andThenInners ->
        fun (results: 'T list) ->
            fun (result: 'T) -> result :: results |> finalContinuation
            |> andThenInner
        |> sequence andThenInners
