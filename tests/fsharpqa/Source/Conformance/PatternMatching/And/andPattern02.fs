// #Conformance #PatternMatching #PatternMatchingGuards 
#light

// Verify multiple pattern parts can introduce new identifiers
type Foo =
    | A of int
    | B of string * int
    
let (|ToString|) x = match x with A x | B(_,x) -> x.ToString()

let test v =
    match v with
    | A(x) & ToString (vToStr) when x = vToStr.Length -> true
    | B(str, value) & ToString (vToStr) when str = vToStr -> true
    | _ -> false
    
if test (A(1))          <> true then exit 1
if test (B("123", 123)) <> true then exit 1

if test (A(2))          <> false then exit 1
if test (B("100", 10))  <> false then exit 1

exit 0
