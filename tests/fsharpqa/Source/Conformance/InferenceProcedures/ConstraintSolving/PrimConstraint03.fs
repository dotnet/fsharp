// #Conformance #TypeInference #TypeConstraints 
#light

// Test primitive constraints

// Test ': null' constraints

let inline isNull<'a when 'a : null> (x : 'a) = 
    match x with
    | null -> "is null"
    | _    -> (x :> obj).ToString()

let runTest =  
    // Wrapping in try block to work around FSB 1989
    try
        if isNull null <> "is null" then exit 1
        if isNull "F#" <> "F#"      then exit 1
        true
    with _ -> exit 1

if runTest <> true then exit 1

exit 0
