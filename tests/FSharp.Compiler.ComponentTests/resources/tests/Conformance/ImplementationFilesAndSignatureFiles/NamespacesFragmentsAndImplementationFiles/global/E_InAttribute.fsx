// #Regression #Conformance #SignatureFiles #Namespaces 
// Regression test for FSHARP1.0:4937
// Usage of 'global' - global in an attribute
//<Expects status="error" id="FS1126" span="(7,19-7,25)">'global' may only be used as the first name in a qualified path</Expects>
//<Expects status="error" id="FS0267" span="(7,19-7,25)">This is not a valid constant expression or custom attribute value</Expects>

[<System.Obsolete(global)>]
type C = struct
         end
