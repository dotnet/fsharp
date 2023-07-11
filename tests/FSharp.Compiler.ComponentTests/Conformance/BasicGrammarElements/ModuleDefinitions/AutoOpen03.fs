// #Conformance #TypesAndModules #Modules 
#light

module A


module B =
    [<AutoOpen>]
    module C = 
       [<AutoOpen>]
       module D = 
           let x = 0
      
// This should cause a cascade...     
open B

// ... bringing x into scope
if x <> 0 then failwith "Failed: 1"
