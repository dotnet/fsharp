// #Conformance #TypeConstraints 
#light

// Test the base types of F# types: Derived Class

let baseTypeName<'a> = match typeof<'a> with
                       | null -> null
                       | x when x.BaseType <> null -> x.BaseType.Name
                       | x                         -> null

// Abstract 
[<AbstractClass>]
type AbstractType =
    class
    end

type DerivedClassType =
    class
        inherit AbstractType
    end

if baseTypeName<DerivedClassType> <> "AbstractType" then exit 1 else exit 0
