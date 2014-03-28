// #Conformance #TypeConstraints 
#light

// Test the base types of F# types: Array

let baseTypeName<'a> = match typeof<'a> with
                       | null -> null
                       | x when x.BaseType <> null -> x.BaseType.Name
                       | x                         -> null

type ArrayType = (int * string) array

if baseTypeName<ArrayType> <> "Array" then exit 1 else exit 0
