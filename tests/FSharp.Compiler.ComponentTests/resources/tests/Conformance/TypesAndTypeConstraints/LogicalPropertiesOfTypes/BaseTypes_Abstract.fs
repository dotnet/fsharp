// #Conformance #TypeConstraints 
#light

// Test the base types of F# types: AbstractClass
[<AbstractClass>]
type AbstractType =
    class
    end

let baseTypeName<'a> = match typeof<'a> with
                       | null -> null
                       | x when x.BaseType <> null -> x.BaseType.Name
                       | x                         -> null

if baseTypeName<AbstractType> <> "Object" then exit 1 else exit 0
