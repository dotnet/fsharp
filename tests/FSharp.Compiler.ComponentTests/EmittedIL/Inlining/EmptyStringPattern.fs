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

let inline testBundledNullAndEmpty (s: string) =
    match s with
    | null | "" -> 0
    | _ -> 1

let inline testBundledEmptyAndNull (s: string) =
    match s with
    | "" | null -> 0
    | _ -> 1

// Usage functions to show inlining in action
let useClassifyString s = classifyString s
let useTestEmptyStringOnly s = testEmptyStringOnly s  
let useBundledNullAndEmpty s = testBundledNullAndEmpty s
let useBundledEmptyAndNull s = testBundledEmptyAndNull s
