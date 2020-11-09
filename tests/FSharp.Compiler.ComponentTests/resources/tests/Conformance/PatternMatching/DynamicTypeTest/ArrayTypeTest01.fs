// #Conformance #PatternMatching #TypeTests 
#light

// Verify ability to specify an integer array in a dynamic type test

let isIntArray (o: obj) =
     match o with
     | :? (int[])        -> 1
     | :? (string array) -> 2
     | _                 -> 3

if isIntArray (box [| 0; 1 |]) <> 1 then exit 1
if isIntArray (box [| "xx" |]) <> 2 then exit 1
if isIntArray (box [1 .. 100]) <> 3 then exit 1

exit 0
