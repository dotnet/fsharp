// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
#light

// Verify error if the type doesn't support an indexer
//<Expects id="FS0039" status="error">The type 'Foo' does not define the field, constructor or member 'Item'</Expects>

type Foo(x : int) =
    member this.Value = x


let t = new Foo(42)

let _ = t.[10]
