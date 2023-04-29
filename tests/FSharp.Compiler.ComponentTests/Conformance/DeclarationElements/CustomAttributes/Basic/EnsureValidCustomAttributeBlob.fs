// #Regression #Conformance #DeclarationElements #Attributes 
#light

// FSB 3565, verify typeof<_> and typedefof<_> can be used in
// attributes. Also, verify that their behaivor is the same in
// attributes as in normal usage.

open System
open System.Reflection

let runTest() = 
    for a in typeof<List<int>>.GetConstructor().GetCustomAttributes() do
        Console.WriteLine(a)
    true

// Actually run our test
if runTest() <> true then failwith "Failed: 1"

