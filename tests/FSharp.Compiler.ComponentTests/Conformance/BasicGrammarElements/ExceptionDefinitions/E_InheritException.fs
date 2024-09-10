// #Regression #Conformance #TypesAndModules #Exceptions 
// Verify error when trying to inherit from F# exception types



exception FSharpExn of int * string

type Foo() =
    inherit FSharpExn()
    member this.Value = 1



