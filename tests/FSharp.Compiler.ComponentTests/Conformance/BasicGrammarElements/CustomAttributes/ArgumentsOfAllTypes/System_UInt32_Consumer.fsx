
module C_System_UInt32 =
    // Consume the attribute from the F# assembly
    [<System_UInt32.T_System_UInt32.A1_System_UInt32(typeof<list<int>>, typedefof<list<int>>)>]
    type T() = member __.M() = true
    
    let t = new T()
    if t.M() then 0 else failwith "Failed: 1"

