// #Conformance #PatternMatching 
#light

// Verify a match failure exception is thrown if a match is not found.

let VerifyThrows f x =
    let result =
        try
            let _ = f x
            false
        with
        | :? MatchFailureException -> true
        | _ -> false

    if result = true 
    then ()
    else exit 1
    
// Test 1
let test1 = (fun () -> let 1 = 2 in let x = 1 in x)
VerifyThrows test1 ()

// Test 2
VerifyThrows (function x when x <> 0 -> true) 0

exit 0
