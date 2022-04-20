
module C_System_Int64 =
    // Consume the attribute from the F# assembly
    [<System_Int64.T_System_Int64.A1_System_Int64(typeof<list<int>>, typedefof<list<int>>)>]
    type T() = member __.M() = true
    
    let t = new T()
    exit <| if t.M() then 0 else 1

