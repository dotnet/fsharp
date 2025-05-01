// #Conformance #PatternMatching #Constants 
#light

let isBob x =
    match x with
    | "Bob" -> true
    | s when s < "Bob" -> false
    | t when t > "Bob" -> false
    | _ -> failwith "Shouldn't ever happen"
   
if isBob "Alan" <> false then exit 1
if isBob "Bob"  <> true  then exit 1
if isBob "Carl" <> false then exit 1

exit 0
