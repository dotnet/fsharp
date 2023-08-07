// #Conformance #PatternMatching #Tuples 
#light

let tupleStartsWithOne x =
    match x with
    | 1, _ -> true
    | _, _ -> false

if tupleStartsWithOne (1, 0) <> true then exit 1
if tupleStartsWithOne (0, 1) <> false then exit 1

exit 0
