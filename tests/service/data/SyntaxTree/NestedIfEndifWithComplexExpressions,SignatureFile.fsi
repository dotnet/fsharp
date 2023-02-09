namespace Foobar

val v : int =
    #if !DEBUG
        #if FOO && BAR
            #if MEH || HMM
                9
            #endif
        #endif
    #endif
    10