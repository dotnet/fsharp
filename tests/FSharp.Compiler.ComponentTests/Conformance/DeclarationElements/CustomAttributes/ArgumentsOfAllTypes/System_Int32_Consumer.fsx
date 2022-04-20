
module C_System_Int32 =
    // Consume the attribute from the F# assembly
    [<System_Int32.T_System_Int32.A1_System_Int32(typeof<list<int>>, typedefof<list<int>>)>]
    type T() = member __.M() = true
    
    let t = new T()
    exit <| if t.M() then 0 else 1

