// #Regression #Conformance #SignatureFiles #Namespaces 
// Regression test for FSHARP1.0:4937
// Naive attempt to make an abbreviation
//<Expects status="error" id="FS0883" span="(6,18-6,24)">Invalid namespace, module, type or union case name</Expects>
//<Expects status="error" id="FS0053" span="(6,18-6,24)">Discriminated union cases and exception labels must be uppercase identifiers</Expects>
type glob_abbr = global
