// #Regression #Conformance #TypeInference #Recursion 
// Regression test for FSHARP1.0:3541
// PEVerification failure with infinite recursion in explict constructor
// Note: as per Don's comment in 3541, we shoudl NOT disallow recursive call of own constructor
#light
open System

type A() = 
  new (x : string) = new A(x)
