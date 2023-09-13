// #Conformance #PatternMatching #TypeTests 
#light

// Verify you can use a dynamic type test against unit

let isUnit (x : obj) =
     match x with
     | :? unit -> true
     | _ -> false


let test1 = isUnit (box ())
let test2 = isUnit (box 42)

if test1 <> true  then exit 1
if test2 <> false then exit 1

exit 0
