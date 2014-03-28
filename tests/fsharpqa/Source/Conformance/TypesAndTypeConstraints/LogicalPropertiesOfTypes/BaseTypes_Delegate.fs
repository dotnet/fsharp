// #Conformance #TypeConstraints 
#light

// Test the base types of F# types: Delegate

let baseTypeName<'a> = match typeof<'a> with
                       | null -> null
                       | x when x.BaseType <> null -> x.BaseType.Name
                       | x                         -> null

type StructType =
    struct
        val dummy : byte    // we need at least 1 member in a struct!
    end
    
type DelegateType = delegate of (obj * StructType) -> int

if baseTypeName<DelegateType> <> "MulticastDelegate" then exit 1 else exit 0

