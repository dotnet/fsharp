// Testing: Module and exception interleaved with members
module Module

type Interleaved =
    member _.X = 1
    module M1 = 
        let y = 2
    member _.Y = 2
    exception E1
    member _.Z = 3
