// #Regression #Conformance #TypesAndModules #Modules 
// Can't have 2 modules with the same name in the same namespace
// Compile with the corresponding 02b.fsx file


namespace N
module ``module`` = 
            let f x = x + 1
