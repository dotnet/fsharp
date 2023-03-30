
let v =
    #if !DEBUG
        #if FOO && BAR
            #if MEH || HMM
                printfn "oh some logging"
            #endif
        #endif
    #endif

    ()
