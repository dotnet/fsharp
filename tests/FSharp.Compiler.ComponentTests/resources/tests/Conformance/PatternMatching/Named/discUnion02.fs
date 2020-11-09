// #Conformance #PatternMatching #ActivePatterns 
#light

// Verify ability to use a constant when matching discriminated unions

type Foo =
    | A of int
    | B of string * int
    
let test x =
    match x with
    | A(42)   -> -42
    | A(x)    -> x
    | B("FSharp", _) -> "FSharp".Length * -1
    | B(str, _)      -> str.Length
    
if test (A(10))      <> 10 then exit 1
if test (B("aaa",1)) <> 3  then exit 1
    
if test (A(42))          <> -42 then exit 1
if test (B("FSharp",2))  <> -6 then exit 1

exit 0

