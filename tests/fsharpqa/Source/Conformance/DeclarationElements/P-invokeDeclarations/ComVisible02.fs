// #Regression #Conformance #DeclarationElements #PInvoke 
// Verify the generated constructors aren't usable from F#
//<Expects id="FS1133" status="error" span="(8,12)">No constructors are available for the type 'r'</Expects>

[<System.Runtime.InteropServices.ComVisible(true)>]
type r = { x : int ; y : string }

let test = new r()
