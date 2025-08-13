// Expected: Multiple warnings for module and exception
module Module

type Interleaved() =
    member _.X = 1
    module M1 = 
        let y = 2
    member _.Y = 2
    exception E1 of int
    member _.Z = 3
