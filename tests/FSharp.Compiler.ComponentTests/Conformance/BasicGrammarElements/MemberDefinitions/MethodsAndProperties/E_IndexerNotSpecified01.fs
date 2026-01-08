// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 


// Verify error if the type doesn't support an indexer


type Foo(x : int) =
    member this.Value = x


let t = new Foo(42)

let _ = t.[10]
