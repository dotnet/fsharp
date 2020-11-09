// #Conformance #SyntacticSugar 
#light

// Sanity check slices
let letters = [| 'a' .. 'z' |]

if letters.[0..2] <> [| 'a'; 'b'; 'c' |] then exit 1
if letters.[23..] <> [| 'x'; 'y'; 'z' |] then exit 1
if letters.[ ..2] <> [| 'a'; 'b'; 'c' |] then exit 1
if letters.[*] <> [|'a'; 'b'; 'c'; 'd'; 'e'; 
                    'f'; 'g'; 'h'; 'i'; 'j'; 
                    'k'; 'l'; 'm'; 'n'; 'o';
                    'p'; 'q'; 'r'; 's'; 't'; 
                    'u'; 'v'; 'w'; 'x'; 'y'; 'z'|] then exit 1

exit 0
