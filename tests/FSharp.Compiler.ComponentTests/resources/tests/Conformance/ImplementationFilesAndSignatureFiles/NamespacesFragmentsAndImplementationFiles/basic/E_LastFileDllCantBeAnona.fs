// #Regression #Conformance #SignatureFiles #Namespaces 
// Verify no warning about the second file in the project being
// an anonymous module, since the project compiles to an EXE

//<Expects status="error" span="(7,1)" id="FS0222">Files in libraries or multiple-file applications must begin with a namespace or module declaration, e\.g\. 'namespace SomeNamespace\.SubNamespace' or 'module SomeNamespace\.SomeModule'\. Only the last source file of an application may omit such a declaration\.$</Expects>

namespace SomeNamespace

type A = B | C
