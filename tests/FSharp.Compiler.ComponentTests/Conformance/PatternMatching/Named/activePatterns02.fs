// #Conformance #PatternMatching #ActivePatterns 
#light

// Multi-case active pattern

let (|Even|Odd|) x = if x % 2 = 0 then Even else Odd

let isEven x = match x with Even -> true | Odd -> false

if isEven 1 <> false then exit 1
if isEven 2 <> true  then exit 1

exit 0
