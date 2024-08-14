// #Regression #Conformance #DeclarationElements #Modules 
#light

//<Expects id="FS0039" status="error">The field, constructor or member 'sum' is not defined</Expects>


// Define a function
let ListMod x = [x]

// Use a module abbreviation with the same name
module ListMod = Microsoft.FSharp.Collections.List

// Error: function wins due to name resolution rules
if ListMod.sum [1; 2; 3] <> 6 then failwith "Failed: 1"
