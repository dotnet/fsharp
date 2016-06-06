// #Regression #Conformance #ControlFlow #Exceptions 
#light

// Verify warning if a finally block does not return 'unit'
//<Expects id="FS0020" status="warning">The result of this expression is implicitly ignored</Expects>

let x : int =
    try
        try
            failwith "epicfail"
        finally
            true
        -1
    with
    | _ -> -1

exit 0
