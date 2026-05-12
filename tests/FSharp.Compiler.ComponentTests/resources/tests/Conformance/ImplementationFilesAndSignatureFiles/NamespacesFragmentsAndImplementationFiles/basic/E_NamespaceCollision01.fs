// #Regression #Conformance #SignatureFiles #Namespaces 
// Verify error if the same fully-qualified type is defined twice
//<Expects id="FS0249" status="error" span="(13,6)">Two type definitions named 'DU' occur in namespace 'A\.B\.C' in two parts of this assembly</Expects>

namespace A.B.C

type DU = 
    | X of float
    | Y of float

namespace A.B.C

type DU = 
    | X' of float
    | Y' of float

