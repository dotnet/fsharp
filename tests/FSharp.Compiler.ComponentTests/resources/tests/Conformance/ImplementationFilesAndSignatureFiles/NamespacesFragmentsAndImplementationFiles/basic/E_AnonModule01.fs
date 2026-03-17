// #Regression #Conformance #SignatureFiles #Namespaces 
// Test 'anonymous implementation files', which is a code file
// without a leading module or namespace declaration. The filename
// will be used first letter caps-else lower case, extension dropped.
//<Expects status="error" span="(6,1)" id="FS0222">Files in libraries or multiple-file applications must begin with a namespace or module declaration, e\.g\. 'namespace SomeNamespace\.SubNamespace' or 'module SomeNamespace\.SomeModule'\. Only the last source file of an application may omit such a declaration\.$</Expects>
let AnonModule01_Value = 10
