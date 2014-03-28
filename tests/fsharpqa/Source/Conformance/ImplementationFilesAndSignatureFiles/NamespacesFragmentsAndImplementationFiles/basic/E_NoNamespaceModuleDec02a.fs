// #Regression #Conformance #SignatureFiles #Namespaces 
// This testcase will emit a warning even though it contains a module, becase
// it is a nested module and not a 'real' module

//<Expects status="error" span="(7,1)" id="FS0222">Files in libraries or multiple-file applications must begin with a namespace or module declaration, e\.g\. 'namespace SomeNamespace\.SubNamespace' or 'module SomeNamespace\.SomeModule'\. Only the last source file of an application may omit such a declaration\.$</Expects>

module ANestedModule =

    type Car = Flying | Hybrid | GasGuzzler

