// #Regression #Conformance #SignatureFiles #Namespaces 
// Regression test for FSHARP1.0:4987
// Can't have both namespace and module in the same file
//<Expects status="error" span="(9,1-9,1)" id="FS0010">Incomplete structured construct at or before this point in definition\. Expected '=' or other token\.$</Expects>

namespace A.B.C

module M
