// #Conformance #TypeConstraints 
#light

// Test the base types of F# types: Record

let baseTypeName<'a> = match typeof<'a> with
                       | null -> null
                       | x when x.BaseType <> null -> x.BaseType.Name
                       | x                         -> null

// Record
type RecordType = { A : int; B : string }

if baseTypeName<RecordType> <> "Object" then exit 1 else exit 0
