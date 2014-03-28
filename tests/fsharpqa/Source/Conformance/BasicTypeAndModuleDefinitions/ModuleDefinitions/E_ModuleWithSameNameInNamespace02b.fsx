// #Conformance #TypesAndModules #Modules 
// Can't have 2 modules with the same name in the same namespace
// Compile with the corresponding 02a.fsx file
#light

namespace N
module ``module`` = 
            let g x = x + 1
