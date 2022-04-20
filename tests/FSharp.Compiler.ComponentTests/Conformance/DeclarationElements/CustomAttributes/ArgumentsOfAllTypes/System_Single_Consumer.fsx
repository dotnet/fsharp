
module C_System_Single =
    // Consume the attribute from the F# assembly
    [<System_Single.T_System_Single.A1_System_Single(typeof<list<int>>, typedefof<list<int>>)>]
    type T() = member __.M() = true
    
    let t = new T()
    exit <| if t.M() then 0 else 1

