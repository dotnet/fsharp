// #Regression #Conformance #DeclarationElements #LetBindings #TypeTests 
// Verify error associated with putting type functions inside types


type Foo() =
    member this.TypeFunc<'a> = typeof<'a>.Name
