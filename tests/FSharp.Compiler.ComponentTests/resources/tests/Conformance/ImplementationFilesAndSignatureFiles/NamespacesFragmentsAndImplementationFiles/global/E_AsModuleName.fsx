// #Regression #Conformance #SignatureFiles #Namespaces 
// Regression test for FSHARP1.0:4937, FSHARP1.0:5260, and FSHARP1.0:6146
// Usage of 'global' - global is a keyword
//<Expects status="error" span="(6,8-6,14)" id="FS0244">Invalid module or namespace name$</Expects>

module global
