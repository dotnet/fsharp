// #Regression #Conformance #PatternMatching #ActivePatterns 
// Verify parameterized partial active patterns which return unit don't force you to 
// bind the result.
// FSB 3502


let (|DivisibleByTwo|_|) x =
    if x % 2 = 0 
    then Some()
    else None

let (|DivisibleByX|_|) x y =
    if y % x = 0 
    then Some()
    else None

// Without this fix, you'd have to bind the result of 'DivisibleByX' so you'd write DivisibleByX 3 (), which is lame.
let divisibleBy x =     
    match x with
    | DivisibleByTwo & DivisibleByX 3 & DivisibleByX 4 -> [2; 3; 4]
    | DivisibleByTwo &                  DivisibleByX 4 -> [2;    4]
    | DivisibleByTwo & DivisibleByX 3                  -> [2; 3;  ]

    | DivisibleByX 3 & DivisibleByX 4 -> [3; 4]
    |                  DivisibleByX 4 -> [   4]
    | DivisibleByX 3                  -> [3;  ]

    | _ -> []

if divisibleBy 12 <> [2; 3; 4] then exit 1
if divisibleBy 16 <> [2; 4]    then exit 2

exit 0
