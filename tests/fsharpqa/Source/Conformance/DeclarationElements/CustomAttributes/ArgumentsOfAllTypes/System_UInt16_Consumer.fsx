
module C_System_UInt16 =
    // Consume the attribute from the F# assembly
    [<System_UInt16.T_System_UInt16.A1_System_UInt16(typeof<list<int>>, typedefof<list<int>>)>]
    type T() = member __.M() = true
    
    let t = new T()
    exit <| if t.M() then 0 else 1

