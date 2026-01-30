// #Conformance #TypeConstraints 
// Struct Records should have the ValueType supertype constraint

[<Struct>]
type StructRec = {
    Dummy: int
} 

let nullable = System.Nullable<StructRec>()
