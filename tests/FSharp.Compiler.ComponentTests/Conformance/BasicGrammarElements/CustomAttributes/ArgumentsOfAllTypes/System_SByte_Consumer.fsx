
module C_System_SByte =
    // Consume the attribute from the F# assembly
    [<System_SByte.T_System_SByte.A1_System_SByte(typeof<list<int>>, typedefof<list<int>>)>]
    type T() = member __.M() = true
    
    let t = new T()
    if t.M() then 0 else failwith "Failed: 1"

