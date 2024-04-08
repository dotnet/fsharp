
module C_System_Double =
    // Consume the attribute from the F# assembly
    [<System_Double.T_System_Double.A1_System_Double(typeof<list<int>>, typedefof<list<int>>)>]
    type T() = member __.M() = true
    
    let t = new T()
    if t.M() then 0 else failwith "Failed: 1"

