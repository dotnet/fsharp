// #Conformance #DeclarationElements #Modules 
#light

module ``Some Crazy Identifier !@#`` = Microsoft.FSharp.Collections.List

    
let result = ``Some Crazy Identifier !@#``.sum [1; 2; 3]

if result <> 6 then failwith "Failed: 1"
