// #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
#light

// Verify creating a propery named Item creates an indexer

type Foo() = 
    member this.Item (x : decimal) = 42


let t = new Foo()

if t.[1.0M] <> 42 then failwith "Failed: 1"
