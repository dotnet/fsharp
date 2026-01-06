module TestLibrary

let inline classifyString (s: string) =
    match s with
    | "" -> "empty"
    | null -> "null"
    | _ -> "other"

let inline testEmptyStringOnly (s: string) =
    match s with
    | "" -> 1
    | _ -> 0
