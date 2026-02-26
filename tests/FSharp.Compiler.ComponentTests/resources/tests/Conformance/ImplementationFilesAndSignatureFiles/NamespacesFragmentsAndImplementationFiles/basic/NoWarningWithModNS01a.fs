// #Conformance #SignatureFiles #Namespaces 
// Verify no warning if every file has a module or namespace

//<Expects status="notin">namespace</Expects>
//<Expects status="notin">module</Expects>
//<Expects status="notin">SomeNamespace.SubNamespace</Expects>
//<Expects status="notin">SomeNamespace.SomeModule</Expects>

namespace NoWarningWithModNS01a

type DU = A | B
