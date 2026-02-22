// #Regression #Conformance #SignatureFiles #Namespaces 
//<Expects id="FS0247" status="error" span="(14,13)">A namespace and a module named 'A\.B' both occur in two parts of this assembly</Expects>

namespace A

    module B =
    
        module C =
        
            type DU =
                | X' of float
                | Y' of float
                
namespace A.B.C

    type DU =
        | X of float
        | Y of float
