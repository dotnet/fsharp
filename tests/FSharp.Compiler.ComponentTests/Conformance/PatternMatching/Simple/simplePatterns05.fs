// #Conformance #PatternMatching 
#light

type Alphabet = 
    | A         | B         | C         | D         | E 
    | F         | G         | H         | I         | J
    | K         | L         | M         | N         | O
    | P         | Q         | R         | S         | T
    | U         | V         | W         | X         | Y
    | Z
    
let isVowel letter =
    match letter with
    | A    | E  | I
    | O    | U 
        -> true
    | Y -> true     // Only sometimes
    | _ -> false
    
if isVowel A <> true then exit 1
if isVowel E <> true then exit 1
if isVowel I <> true then exit 1
if isVowel O <> true then exit 1
if isVowel U <> true then exit 1
if isVowel Y <> true then exit 1
if isVowel C <> false then exit 1

exit 0
