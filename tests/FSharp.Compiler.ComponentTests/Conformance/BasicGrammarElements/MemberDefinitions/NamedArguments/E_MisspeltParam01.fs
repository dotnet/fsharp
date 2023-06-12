// #Regression #Conformance #DeclarationElements #MemberDefinitions #NamedArguments 
// Verify error when you misspell named parameters
//<Expects id="FS0508" status="error" span="(5,9)">No accessible member or object constructor named 'ProcessStartInfo' takes 0 arguments\. The named argument 'Argument' doesn't correspond to any argument or settable return property for any overload</Expects>

let _ = new System.Diagnostics.ProcessStartInfo(FileName = "test", Argument = "testarg")
