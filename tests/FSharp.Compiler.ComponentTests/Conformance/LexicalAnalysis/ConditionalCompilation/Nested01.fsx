// #Conformance #LexicalAnalysis 
#light

// Verify nested conditional compilation flags

module Nested01

#if UNDEFINED1
ignore 1
#else
    #if UNDEFINED2
        ignore 1
    #else
        #if UNDEFINED3
            ignore 1
        #else
            #if UNDEFINED4
                ignore 1
            #else
                #if UNDEFINED5
                    ignore 1
                #else
                    (*Active Code5*)let legitCode5 = 5
                #endif
                (*Active Code4*)    let legitCode4 = 4
            #endif
            (*Active Code3*)        let legitCode3 = 3
        #endif

        (*Active Code2*)            let legitCode2 = 2
    #endif
    (*Active Code1*)                let legitCode1 = 1
    
#endif

#if DEFINED1
    (*Active Code6*)                let legitCode6 = 6
    #if DEFINED2
        (*Active Code7*)
                                    let legitCode7 = 7
    #else
        ignore 1
    #endif
#else
    ignore 1
#endif

// If the code wasn't enabled, this would cause syntax errors
                                    let test = [
                                                    legitCode1
                                                    legitCode2
                                                    legitCode3
                                                    legitCode4
                                                    legitCode5
                                                    legitCode6
                                                    legitCode7
                                                ]
    
                                    ignore 0
