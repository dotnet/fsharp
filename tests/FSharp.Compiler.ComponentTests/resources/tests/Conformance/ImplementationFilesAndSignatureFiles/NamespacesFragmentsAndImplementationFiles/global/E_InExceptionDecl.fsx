// #Regression #Conformance #SignatureFiles #Namespaces 
// Regression test for FSHARP1.0:4937
// Usage of 'global' - global in an exception
//<Expects status="error" id="FS0010" span="(7,11-7,17)">Unexpected keyword 'global' in exception definition\. Expected identifier or other token</Expects>


exception global of int
