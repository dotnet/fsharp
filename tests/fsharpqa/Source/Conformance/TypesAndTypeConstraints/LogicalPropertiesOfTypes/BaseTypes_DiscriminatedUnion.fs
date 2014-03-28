// #Conformance #TypeConstraints 
#light

// Test the base types of F# types: Discriminated Union

let baseTypeName<'a> = match typeof<'a> with
                       | null -> null
                       | x when x.BaseType <> null -> x.BaseType.Name
                       | x                         -> null

type UnionType =
    | A of int
    | B of string

if baseTypeName<UnionType> <> "Object" then exit 1 else exit 0
