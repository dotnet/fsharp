// #Regression #Conformance #SignatureFiles #Namespaces 
// Regression test for FSHARP1.0:4937
// Usage of 'global' - global is a keyword
//<Expects status="error" id="FS0010" span="(6,12-6,13)">Unexpected symbol '=' in binding\. Expected '\.' or other token</Expects>

let global = 1
