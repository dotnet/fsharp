// #Regression #Conformance #PatternMatching #TypeConstraints 
// Regression test for FSHARP1.0:1525
// type constraints on pattern matching are deprecated and are now errors (used to be warnings)
// As of Beta2, we are not even giving the deprecation error.
//<Expects id="FS0010" span="(21,8-21,10)" status="error">Unexpected symbol ':>' in pattern. Expected '\)' or other token.</Expects>
//<Expects id="FS0583" span="(21,5-21,6)" status="error">Unmatched '\('$</Expects>
//<Expects id="FS0222" span="(8,1-9,1)" status="error">Files in libraries or multiple-file applications must begin with a namespace or module declaration, e.g. 'namespace SomeNamespace.SubNamespace' or 'module SomeNamespace.SomeModule'. Only the last source file of an application may omit such a declaration.</Expects>
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
