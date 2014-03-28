// #Conformance #TypeConstraints 
#light

// Test the base types of F# types: Variable

let baseTypeName<'a> = match typeof<'a> with
                       | null -> null
                       | x when x.BaseType <> null -> x.BaseType.Name
                       | x                         -> null

let varType<'a> = typeof<'a>

let tyVarType = varType
if tyVarType.Name <> "Object" then exit 1 else exit 0
