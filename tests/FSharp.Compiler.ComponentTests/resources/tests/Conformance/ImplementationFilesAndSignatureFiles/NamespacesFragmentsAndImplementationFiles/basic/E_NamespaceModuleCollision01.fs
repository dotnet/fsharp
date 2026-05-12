// #Regression #Conformance #SignatureFiles #Namespaces 
//<Expects id="FS0247" status="error" span="(14,13)">The name 'A\.B' is used as both a namespace and a module in this assembly\. Rename one of them to avoid the conflict\.</Expects>

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
