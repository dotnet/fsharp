// #Conformance #TypeConstraints 
#light

// Test the base types of F# types: Class 

let baseTypeName<'a> = match typeof<'a> with
                       | null -> null
                       | x when x.BaseType <> null -> x.BaseType.Name
                       | x                         -> null

type ClassType = 
    class
    end

if baseTypeName<ClassType> <> "Object" then exit 1 else exit 0

