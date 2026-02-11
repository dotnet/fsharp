// #Conformance #SignatureFiles #Namespaces 
// Normally this would produce a warning, but
// since this compiles an EXE one shouldn't be emitted.

//<Expects status="notin">namespace</Expects>
//<Expects status="notin">module</Expects>
//<Expects status="notin">SomeNamespace.SubNamespace</Expects>
//<Expects status="notin">SomeNamespace.SomeModule</Expects>

exit 0
