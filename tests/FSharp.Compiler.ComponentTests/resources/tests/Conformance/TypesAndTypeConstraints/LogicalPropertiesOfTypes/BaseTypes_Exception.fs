// #Conformance #TypeConstraints 
#light

// Test the base types of F# types: F#-style Exception

let baseTypeName<'a> = match typeof<'a> with
                       | null -> null
                       | x when x.BaseType <> null -> x.BaseType.Name
                       | x                         -> null

exception ExceptionType of int * string

let ex1 = ExceptionType(1, "one")
if ex1.GetType().BaseType.Name <> "Exception" then exit 1 else exit 0
