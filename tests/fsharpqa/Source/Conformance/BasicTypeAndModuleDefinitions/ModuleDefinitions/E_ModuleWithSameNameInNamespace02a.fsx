// #Regression #Conformance #TypesAndModules #Modules 
// Can't have 2 modules with the same name in the same namespace
// Compile with the corresponding 02b.fsx file
//<Expects id="FS0248" span="(7,8-7,18)" status="error">Two modules named 'N\.module' occur in two parts of this assembly</Expects>

namespace N
module ``module`` = 
            let f x = x + 1
