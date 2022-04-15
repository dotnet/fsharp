// #Regression #Conformance #TypesAndModules #Exceptions 
// Regression test for FSHARP1.0:3769
// Verify that F# exceptions are accessible from the Reflection API

exception E of int * int * int
exception E' of string

let p = Microsoft.FSharp.Reflection.FSharpType.IsExceptionRepresentation(typeof<E>)
let p' = Microsoft.FSharp.Reflection.FSharpType.IsExceptionRepresentation(typeof<E'>)
let p'' = Microsoft.FSharp.Reflection.FSharpType.IsExceptionRepresentation(typeof<System.Exception>)

if not(p && p' && (not p'')) then failwith "Failed: 1"
