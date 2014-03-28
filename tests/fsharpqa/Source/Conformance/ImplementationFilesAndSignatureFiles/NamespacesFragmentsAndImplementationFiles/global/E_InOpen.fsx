// #Regression #Conformance #SignatureFiles #Namespaces 
//
// Try to open the global namespace -> error
//<Expects status="error" id="FS1126" span="(10,6-10,12)">'global' may only be used as the first name in a qualified path</Expects>

// OK
open global.Microsoft

// Err
open global
