// #Conformance #DeclarationElements #MemberDefinitions #Overloading 
// As of Beta2, we don't need OverloadIDs anymore!
//<Expects status="success"></Expects>
type Foo() =
    member this.SomeMethod (x:int)    = true
    member this.SomeMethod (y:string) = false


let f = new Foo()

if f.SomeMethod(10) && (not (f.SomeMethod(""))) then () else failwith "Failed: 1"
