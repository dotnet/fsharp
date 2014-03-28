// #Regression #Conformance #LexFilter 
#indent "off"
// Regression test for FSHARP1.0:1078
// The opposit of #light is (for now) #indent "off"
//<Expects id="FS0240" status="error" span="(7,1)">The signature file 'Indent_off_01' does not have a corresponding implementation file\. If an implementation file exists then check the 'module' and 'namespace' declarations in the signature and implementation files match</Expects>

type R
