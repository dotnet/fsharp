// #Conformance #DeclarationElements #MemberDefinitions #Overloading 
// Verify OverloadIDAttribute should is not required on all overloads

type Test1()  = 
    member x.ChangeText(s: string) = ()
    member x. ChangeText(i: int) = ()

//case 2:
type Test2()  = 
    member x.ChangeText(s: string) = ()
    member x.ChangeText(i: int) = ()
