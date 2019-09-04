// #Regression #NoMT #Import 
// Regression test for FSharp1.0:3882 - C# internals are not visible to F# when C# uses the InternalsVisibleTo attribute
// Regression test for DevDiv:90622
// Test that even if we have InternalsVisibleTo() attribute for this friendly assembly, private members won't be still accessible
//<Expects status="error" span="(8,6)" id="FS1092">The type 'CalcBeta' is not accessible from this code location$</Expects>
//<Expects status="error" span="(8,2)" id="FS0509">Method or object constructor 'CalcBeta' not found$</Expects>
//<Expects status="error" span="(8,18)" id="FS0039">The type 'CalcBeta' does not define the field, constructor or member 'Mod'</Expects>
(new CalcBeta()).Mod(1,1) |> ignore

exit 1
