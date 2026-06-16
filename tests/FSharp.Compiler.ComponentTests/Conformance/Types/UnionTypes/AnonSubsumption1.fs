// #Conformance #TypesAndModules #Unions 
// Anonymous Union Types
// System.ValueType is a supertype of all the primitive types, so an anonymous union of primitive types should be subsumed by System.ValueType
//<Expects status="success"></Expects>

let id (x: System.ValueType): System.ValueType = x

let y: (int8 | int16) = 1s
id y |> ignore