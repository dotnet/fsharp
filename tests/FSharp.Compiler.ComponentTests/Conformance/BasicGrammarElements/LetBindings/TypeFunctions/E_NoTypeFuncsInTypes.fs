// #Regression #Conformance #DeclarationElements #LetBindings #TypeTests 
// Verify error associated with putting type functions inside types


type Foo() =
    member this.TypeFunc<'a> = typeof<'a>.Name
    member this.TypeFunc2<'a, 'b> = typeof<'a>.Name
