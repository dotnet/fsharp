// #Regression #Conformance #TypesAndModules 
// Type abbreviation
// Recursive definition: using measures
// This is regression test for FSHARP1.0:3784
//<Expects id="FS0953" span="(7,18-7,20)" status="error">This type definition involves an immediate cyclic reference through an abbreviation</Expects>
#light
[<Measure>] type Kg = Kg        // cyclic

exit 1
