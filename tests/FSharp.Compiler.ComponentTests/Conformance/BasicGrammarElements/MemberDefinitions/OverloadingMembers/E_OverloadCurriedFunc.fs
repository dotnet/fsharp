// #Regression #Conformance #DeclarationElements #MemberDefinitions #Overloading 
// Verify error when overloading a curried function
//<Expects id="FS0816" status="error">One or more of the overloads of this method has curried arguments. Consider redesigning these members to take arguments in tupled form</Expects>

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
