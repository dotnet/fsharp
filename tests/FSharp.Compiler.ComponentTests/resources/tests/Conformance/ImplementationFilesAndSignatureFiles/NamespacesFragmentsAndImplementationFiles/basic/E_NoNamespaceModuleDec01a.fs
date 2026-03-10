// #Regression #Conformance #SignatureFiles #Namespaces 
//<Expects status="error" span="(4,1)" id="FS0222">Files in libraries or multiple-file applications must begin with a namespace or module declaration, e\.g\. 'namespace SomeNamespace\.SubNamespace' or 'module SomeNamespace\.SomeModule'\. Only the last source file of an application may omit such a declaration\.$</Expects>

type Car = Flying | Hybrid | GasGuzzler

