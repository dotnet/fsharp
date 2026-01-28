// #Regression #Conformance #SignatureFiles #Namespaces 
// Verify error if anything other than '#' directives come before the first namespace decl
//<Expects id="FS0530" status="error" span="(7,1)">Only '#' compiler directives may occur prior to the first 'namespace' declaration</Expects>

#nowarn "25"

let x = 1  // <--------- ERROR

namespace M
