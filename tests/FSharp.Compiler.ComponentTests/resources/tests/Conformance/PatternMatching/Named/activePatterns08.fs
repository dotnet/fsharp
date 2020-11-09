// #Conformance #PatternMatching #ActivePatterns 
#light

// Verify able to mix active patterns with 'regular' patterns.

let (|Odd|Even|) x = if x % 2 = 0 then Even else Odd
let (|MultipleOf2|_|) x = if x % 2 = 0 then Some () else None
let (|MultipleOfN|_|) (n:int) (x:int) = if x % n = 0 then Some () else None

let test x = 
    match x with
    | MultipleOfN 33 () -> 0
    | Odd & MultipleOfN 5 () -> 1
    | Odd & 3             -> 2
    | MultipleOfN 4 _  | MultipleOfN 8 _  | MultipleOfN 16 _
      -> 3
    | 46 -> 5
    | MultipleOf2 & Even -> 4
    | Odd -> 6
    | Even -> 7

if test 33 <> 0 then exit 1

if test 5  <> 1 then exit 1
if test 15 <> 1 then exit 1

if test 3 <> 2 then exit 1

if test  4 <> 3 then exit 1
if test  8 <> 3 then exit 1
if test 16 <> 3 then exit 1

if test 262 <> 4 then exit 1

if test 46 <> 5 then exit 1

if test 101 <> 6 then exit 1
if test 102 <> 4 then exit 1    // MulOf2 & Even dominates just Even, since it comes first

exit 0
