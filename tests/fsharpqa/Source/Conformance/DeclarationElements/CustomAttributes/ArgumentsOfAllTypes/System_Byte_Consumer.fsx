
module C_System_Byte =
    // Consume the attribute from the F# assembly
    [<System_Byte.T_System_Byte.A1_System_Byte(typeof<list<int>>, typedefof<list<int>>)>]
    type T() = member __.M() = true
    
    let t = new T()
    exit <| if t.M() then 0 else 1

