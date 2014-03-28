// #Conformance #TypesAndModules #Modules 
#light

// Verify ability to fully qualify module identifiers

module Test =

    module A =
        module B = 
            module C =
                module D =
                    let counter =
                        let x = ref 0
                        (fun () -> x := !x + 1; !x)

    open A.B.C.D
    if counter() <> 1 then exit 1

    if A.B.C.D.counter() <> 2 then exit 1


    exit 0
