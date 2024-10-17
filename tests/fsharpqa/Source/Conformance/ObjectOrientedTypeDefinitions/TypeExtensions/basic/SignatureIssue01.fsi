// #Regression #Conformance #ObjectOrientedTypes #TypeExtensions 
// Regression test for FSHARP1.0:3473
// Signature checking issue with extension of a type defined in a C# dll
//<Expects status="success"></Expects>
#light

module Experiment.Extension

// extension for the F# type Test.Test
type Experiment.Test.Test<'a,'b>
     with
        member Copy: unit -> Experiment.Test.Test<'a2,'b>        

// extension for the C# type Test2
type Experiment.Test2<'a,'b>
     with
        member Copy: unit -> Experiment.Test2<'a2,'b>
        
