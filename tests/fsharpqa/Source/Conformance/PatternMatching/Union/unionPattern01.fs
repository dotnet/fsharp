// #Conformance #PatternMatching #Unions 
#light

type Foo = 
    | A of int * string
    | B of string * int
    
let test x = 
    match x with
    | A (x, "cheese")
    | B (_, x) when x > 0 
       -> x
    | _ -> 0

if test (A(100, "")) <> 0 then exit 1
if test (A(100, "cheese")) <> 100 then exit 1

if test (B("", -100)) <> 0 then exit 1
if test (B("", 42)) <> 42 then exit 1
             
exit 0
