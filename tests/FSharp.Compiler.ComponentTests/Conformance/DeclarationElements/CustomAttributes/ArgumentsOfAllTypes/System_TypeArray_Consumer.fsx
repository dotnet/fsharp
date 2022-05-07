
module C_System_TypeArray =
    // Consume the attribute from the F# assembly
    [<System_TypeArray.T_System_TypeArray.A1_System_TypeArray(typeof<list<int>>, typedefof<list<int>>)>]
    type T() = member __.M() = true
    
    let t = new T()
    if t.M() then 0 else failwith "Failed: 1"

