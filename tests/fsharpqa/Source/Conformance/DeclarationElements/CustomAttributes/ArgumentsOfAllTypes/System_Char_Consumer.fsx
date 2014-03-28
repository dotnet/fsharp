
module C_System_Char =
    // Consume the attribute from the F# assembly
    [<System_Char.T_System_Char.A1_System_Char(typeof<list<int>>, typedefof<list<int>>)>]
    type T() = member __.M() = true
    
    let t = new T()
    exit <| if t.M() then 0 else 1

