// #Conformance #ControlFlow #Exceptions 
#light

// Verify simple try with blogs work

let testPassed =
    try
        failwith "epicfail"
        false
    with
    | Failure "non-epicfail" -> false
    | Failure "epicfail"     -> true
    | _                      -> false

if not testPassed then exit 1

exit 0
