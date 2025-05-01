
module C_System_Int16 =
    // Consume the attribute from the F# assembly
    [<System_Int16.T_System_Int16.A1_System_Int16(typeof<list<int>>, typedefof<list<int>>)>]
    type T() = member __.M() = true
    
    let t = new T()
    if t.M() then 0 else failwith "Failed: 1"

