// Expected: Warning for module after members
module Module

type ClassWithMembers() =
    member _.Method1() = 1
    member _.Method2() = 2
    
    module InvalidModule = 
        let helper = 10
