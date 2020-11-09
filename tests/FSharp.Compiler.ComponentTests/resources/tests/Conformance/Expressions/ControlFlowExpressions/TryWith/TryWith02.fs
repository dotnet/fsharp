// #Conformance #ControlFlow #Exceptions 
#light

// Verify simple try finally blogs work

let mutable finallyBlockHit = false

let testPassed =
    // Even though the finally block is exeucted, we still need to catch the exception :)
    try
        try
            failwith "epicfail"
            false
        finally
            finallyBlockHit <- true
            ()
    with
    | _ -> false

if not finallyBlockHit then exit 1

exit 0
