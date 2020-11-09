// #Conformance #LexicalAnalysis 
#light

// Verify nested conditional compilation flags

module Nested01

#if UNDEFINED1
exit 1
#else
    #if UNDEFINED2
        exit 1
    #else
        #if UNDEFINED3
            exit 1
        #else
            #if UNDEFINED4
                exit 1
            #else
                #if UNDEFINED5
                    exit 1
                #else
                    #if UNDEFINED6
                        exit 1
                    #else
                        #if UNDEFINED7
                            exit 1
                        #else
                            #if UNDEFINED8
                                exit 1
                            #else
                                #if UNDEFINED9
                                    exit 1
                                #else
                                    #if UNDEFINED10
                                        exit 1
                                    #else
                                        (*Active Code10*)let legitCode10 = 10
                                    #endif
                                         (*Active Code9*)let legitCode9 = 9
                                #endif
                                         (*Active Code8*)let legitCode8 = 8
                            #endif
                                         (*Active Code7*)let legitCode7 = 7
                        #endif
                                         (*Active Code6*)let legitCode6 = 6
                    #endif
                                         (*Active Code5*)let legitCode5 = 5
                #endif
                                         (*Active Code4*)let legitCode4 = 4
            #endif
                                         (*Active Code3*)let legitCode3 = 3
        #endif
                                         (*Active Code2*)let legitCode2 = 2
    #endif
                                         (*Active Code1*)let legitCode1 = 1
    
#endif

#if DEFINED1
                                         (*Active CodeA*)let legitCodeA = 6
    #if DEFINED2
                                         (*Active CodeB*)
                                                         let legitCodeB = 7
    #else
        exit 1
    #endif
#else
    exit 1
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
                                                                         legitCode8
                                                                         legitCode9
                                                                         legitCode10
                                                                         legitCodeA
                                                                         legitCodeB
                                                                     ]
                                                        
                                                         exit 0
