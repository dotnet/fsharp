// #Regression #Conformance #SignatureFiles 
// Regression for FSHARP1.0:5852
// Overly strict checks when a type is implicitly hidden by signature

module Module1

type internal Z() = member x.P = 1
type Q1 = { field : Z } // this is ok because Q1 is implicitly internal
type Q2 = ABC of Z           
type Q3() = inherit Z()
type Q4 = delegate of Z -> Z
[<AbstractClass>]
type Q5() = 
    abstract M : Z -> Z
