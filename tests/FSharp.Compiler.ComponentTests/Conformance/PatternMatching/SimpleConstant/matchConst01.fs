// #Conformance #PatternMatching #Constants 
#light

let isZero x =
    match x with
    | 0 -> true
    | x when x < 0 -> false
    | x when x > 0 -> false
    | _ -> failwith "Shouldn't ever happen"
   
if isZero (-1) <> false then exit 1
if isZero  0 <> true  then exit 1
if isZero  1 <> false then exit 1

exit 0
