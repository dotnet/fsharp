
module C_System_String =
    // Consume the attribute from the F# assembly
    [<System_String.T_System_String.A1_System_String(typeof<list<int>>, typedefof<list<int>>)>]
    type T() = member __.M() = true
    
    let t = new T()
    exit <| if t.M() then 0 else 1

