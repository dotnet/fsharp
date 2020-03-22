// #Regression #Conformance #ObjectOrientedTypes #TypeExtensions 
// Regression test for FSHARP1.0:3473
// Signature checking issue with extension of a type defined in a C# dll
//<Expects status="success"></Expects>
#light

module Experiment.Test

/// F# version of Experiment.Test2
type Test<'a,'b>(a:'a, b:'b) =
    member t.ValA = a
    member t.ValB = b        

