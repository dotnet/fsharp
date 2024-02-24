// #Conformance #PatternMatching #ActivePatterns 
#light

// Verify ability to use wildcards when matching discriminated unions

type Foo =
    | A of int
    | B of string * int

let test x =
    match x with
    | A(1) | B(_,1) -> 1
    | A(2) | B(_,2) -> 2
    | B(_, _) -> -1
    | A(_)    -> -2

if test (A(1))    <> 1 then exit 1
if test (B("",1)) <> 1 then exit 1

if test (A(2))    <> 2 then exit 1
if test (B("",2)) <> 2 then exit 1

if test (A(42))    <> -2 then exit 1
if test (B("",42)) <> -1 then exit 1

exit 0

