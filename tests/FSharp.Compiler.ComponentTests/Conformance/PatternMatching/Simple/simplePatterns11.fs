// #Conformance #PatternMatching 
#light

let sumEvenValues (tuple:int*int) =
    match tuple with
    | x, y when x % 2 = 0 && y % 2 = 0 -> x + y
    | x, _ when x % 2 = 0              -> x
    | _, y when y % 2 = 0              -> y
    | _, _ -> 0
    | _ -> 0
        
if sumEvenValues (2, 2) <> 4 then exit 1
if sumEvenValues (1, 5) <> 0 then exit 1

exit 0
