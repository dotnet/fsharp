// #Regression #Conformance #SignatureFiles #Namespaces 
// This testcase will emit a warning even though it contains a module, because
// it is a nested module and not a 'real' module

//<Expects status="error" span="(7,1)" id="FS0222">Files in libraries or multiple-file applications must begin with a namespace or module declaration. When using a module declaration at the start of a file the '=' sign is not allowed. If this is a top-level module, consider removing the = to resolve this error.</Expects>

module ANestedModule =

    type Car = Flying | Hybrid | GasGuzzler

