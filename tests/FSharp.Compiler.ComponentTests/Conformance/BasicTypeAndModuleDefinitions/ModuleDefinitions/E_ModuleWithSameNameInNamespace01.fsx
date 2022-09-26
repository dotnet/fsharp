// #Regression #Conformance #TypesAndModules #Modules 
// Can't have 2 modules with the same name in the same namespace
//<Expects id="FS0037" span="(9,1-9,18)" status="error">Duplicate definition of type, exception or module 'module'</Expects>

namespace N
module ``module`` = 
            let f x = x + 1

module ``module`` =
            let g x = x + 1
