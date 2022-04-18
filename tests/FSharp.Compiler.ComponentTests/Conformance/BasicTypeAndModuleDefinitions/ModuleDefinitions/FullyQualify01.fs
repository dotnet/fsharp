// #Conformance #TypesAndModules #Modules 
#light

// Verify ability to fully qualify module identifiers

module Test =

    module A =
        module B = 
            module C =
                module D =
                    let counter =
                        let mutable x = 0
                        (fun () -> x <- x + 1; x)

    open A.B.C.D
    if counter() <> 1 then failwith "Failed 1"

    if A.B.C.D.counter() <> 2 then failwith "Failed 2"
