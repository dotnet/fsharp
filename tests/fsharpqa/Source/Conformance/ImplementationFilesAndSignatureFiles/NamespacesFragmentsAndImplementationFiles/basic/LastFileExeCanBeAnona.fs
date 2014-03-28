// #Conformance #SignatureFiles #Namespaces 
// Verify no warning about the second file in the project being
// an anonymous module, since the project compiles to an EXE

//<Expects status="notin">namespace</Expects>
//<Expects status="notin">module</Expects>
//<Expects status="notin">SomeNamespace.SubNamespace</Expects>
//<Expects status="notin">SomeNamespace.SomeModule</Expects>

namespace SomeNamespace

type A = B | C
