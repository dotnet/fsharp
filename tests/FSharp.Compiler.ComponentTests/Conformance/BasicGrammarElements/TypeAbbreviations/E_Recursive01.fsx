// #Regression #Conformance #TypesAndModules 
// Type abbreviation
// Recursive definition: using measures
// This is regression test for FSHARP1.0:3784


[<Measure>] type Kg = Kg        // cyclic

exit 1
