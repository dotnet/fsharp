// #Regression #Conformance #PatternMatching #TypeConstraints 
// Regression test for FSHARP1.0:1525
// type constraints on pattern matching are deprecated and are now errors (used to be warnings)
// As of Beta2, we are not even giving the deprecation error.



type A() = class
           end
         
type B() = class
            inherit A()
         end

type C() = class
            inherit B()
         end

let test1 (x:#A) = 
  match x with
  | (o :> B) -> o.GetHashCode()
