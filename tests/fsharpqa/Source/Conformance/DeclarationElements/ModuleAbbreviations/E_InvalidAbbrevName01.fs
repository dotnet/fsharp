// #Regression #Conformance #DeclarationElements #Modules 
// Verify error when abbreviating a module to an invalid name
//<Expects status="error" span="(5,11-5,12)" id="FS0010">Unexpected symbol '\$' in definition\. Expected incomplete structured construct at or before this point or other token\.$</Expects>

module a__$       = Microsoft.FSharp.Collections.List
