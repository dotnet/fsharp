// #Regression #Conformance #DeclarationElements #MemberDefinitions #Overloading 
// Verify error when overloading a curried function


type C =
    [<OverloadID("1")>]
    static member DoStuff (x : obj) = ()
    [<OverloadID("2")>]
    static member DoStuff (x : string) = ()
    [<OverloadID("3")>]
    static member DoStuff (x : obj) (y : obj) = ()
    [<OverloadID("4")>]
    static member DoStuff (x : obj) (y : string) = ()
    

let emptySet1 : Set<int> = Set.empty
let emptySet2 : Set<int> = Set.empty
C.DoStuff(emptySet1 = emptySet2)
