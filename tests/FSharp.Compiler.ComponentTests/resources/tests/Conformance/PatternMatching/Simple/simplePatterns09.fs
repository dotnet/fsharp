// #Conformance #PatternMatching 
#light

let sumArray array =
    match array with
    | [| |]          -> 0
    | [|x|]          -> x
    | [|x; y|]       -> x + y
    | [|x; y; z|]    -> x + y + z
    | [|w; x; y; z|] -> w + x + y + z
    | _ -> failwith "too large"
    
if sumArray [| |]       <> 0 then exit 1
if sumArray [|1|]       <> 1 then exit 1
if sumArray [|1;2|]     <> 3 then exit 1
if sumArray [|1;2;3|]   <> 6 then exit 1
if sumArray [|1;2;3;4|] <> 10 then exit 1

exit 0
