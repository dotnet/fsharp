// #Conformance #TypesAndModules #Modules 
#light

// Sanity check AutoOpen

[<AutoOpen>]
module M =
    let x = 0


// While processing the anonomous module (this file) nested module M
// will automatically be opened, bringing x into scope.

exit x
