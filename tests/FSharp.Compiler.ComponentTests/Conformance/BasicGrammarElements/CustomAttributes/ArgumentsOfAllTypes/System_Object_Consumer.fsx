
module C_System_Object =
    // Consume the attribute from the F# assembly
    [<System_Object.T_System_Object.A1_System_Object(typeof<list<int>>, typedefof<list<int>>)>]
    type T() = member __.M() = true
    
    let t = new T()
    if t.M() then 0 else failwith "Failed: 1"

