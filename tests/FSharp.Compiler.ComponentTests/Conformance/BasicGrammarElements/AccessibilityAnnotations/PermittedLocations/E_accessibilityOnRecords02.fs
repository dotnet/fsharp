// #Regression #Conformance #DeclarationElements #Accessibility 
// Regression test for FSHARP1.0:1537
// The warning is emitted _and_ the code compiles just fine.
// As of 1/16/2009, the warning is now an error - so this code does not compile!
//<Expects id="FS0575" span="(8,13-8,28)" status="error">Accessibility modifiers are not permitted on record fields\. Use 'type R = internal \.\.\.' or 'type R = private \.\.\.' to give an accessibility to the whole representation</Expects>

module M
 type R = { private i : int }   // err
 let r = { i = 10 }
