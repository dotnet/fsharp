
module C_System_DateTimeKind =
    // Consume the attribute from the F# assembly
    [<System_DateTimeKind.T_System_DateTimeKind.A1_System_DateTimeKind(typeof<list<int>>, typedefof<list<int>>)>]
    type T() = member __.M() = true
    
    let t = new T()
    exit <| if t.M() then 0 else 1

