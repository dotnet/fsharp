// #Conformance #PatternMatching 
#light

// Verify 'as pattern' construct

let test x = 
    match x with
    | (1, _) | (_, 2) as result 
        -> if fst result <> 1 && 
              snd result <> 2 then 
               false
           else
               true
    | _ -> false

if test (1, 0) <> true  then exit 1
if test (0, 2) <> true  then exit 1
if test (3, 3) <> false then exit 1

exit 0
