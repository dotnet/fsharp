// Expected: Multiple warnings for each inappropriately nested construct
module Module

type A =
    | A
    type B = A
        module C = ()
            exception D
                module E = C
                    let f () = ()
                        open System
                            module G =
                                module H = E
