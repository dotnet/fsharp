
module C_System_Type =
    // Consume the attribute from the F# assembly
    [<System_Type.T_System_Type.A1_System_Type(typeof<list<int>>, typedefof<list<int>>)>]
    type T() = member __.M() = true
    
    let t = new T()
    if t.M() then 0 else failwith "Failed: 1"

