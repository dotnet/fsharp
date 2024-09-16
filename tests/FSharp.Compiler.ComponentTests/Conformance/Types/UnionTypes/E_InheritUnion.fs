// #Regression #Conformance #TypesAndModules #Unions 
// Verify error when inherit from union types



type DiscUnion = A of int | B of string


type Foo() =
    inherit DiscUnion

    member this.Stuff = 1


exit 1
