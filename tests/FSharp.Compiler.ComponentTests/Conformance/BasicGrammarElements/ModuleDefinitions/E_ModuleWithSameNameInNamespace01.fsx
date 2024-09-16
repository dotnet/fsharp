// #Regression #Conformance #TypesAndModules #Modules 
// Can't have 2 modules with the same name in the same namespace


namespace N
module ``module`` = 
            let f x = x + 1

module ``module`` =
            let g x = x + 1
