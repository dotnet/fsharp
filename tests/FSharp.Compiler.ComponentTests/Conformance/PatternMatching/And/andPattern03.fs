// #Conformance #PatternMatching #PatternMatchingGuards 
#light

open System

let (|ToInt|) (x: string) =
    let (parsed, result) = Int32.TryParse(x)
    if parsed then result
    else           -1

let (|ToStr|) (x : int) = x.ToString()

let test input =
    match input with
    | ToInt (ToStr "1")           -> 1
    | ToInt 2 & ToInt (ToStr "2") -> 2
    | ToInt (ToStr "3") & ToInt (ToStr (ToInt (ToStr "3"))) -> 3
    | _ -> -1

if test "1" <> 1 then exit 1
if test "2" <> 2 then exit 2
if test "3" <> 3 then exit 3

exit 0




