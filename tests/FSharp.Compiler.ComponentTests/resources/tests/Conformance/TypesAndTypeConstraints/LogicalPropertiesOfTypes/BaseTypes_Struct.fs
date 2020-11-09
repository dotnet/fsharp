// #Conformance #TypeConstraints 
#light

// Test the base types of F# types: Struct

let baseTypeName<'a> = match typeof<'a> with
                       | null -> null
                       | x when x.BaseType <> null -> x.BaseType.Name
                       | x                         -> null

type StructType =
    struct
        val dummy : byte    // we need at least 1 member in a struct!
    end

if baseTypeName<StructType> <> "ValueType" then exit 1 else exit 0

