// #Conformance #PatternMatching #ActivePatterns 
#light

// Test ability to specify multiple active patterns in the same pattern match

let (|A|_|) x = if x = 'A' then Some() else None
let (|B|_|) x = if x = 'B' then Some() else None
let (|C|_|) x = if x = 'C' then Some() else None
let (|D|_|) x = if x = 'D' then Some() else None
let (|E|_|) x = if x = 'E' then Some() else None
let (|F|_|) x = if x = 'F' then Some() else None
let (|G|_|) x = if x = 'G' then Some() else None
let (|H|_|) x = if x = 'H' then Some() else None
let (|I|_|) x = if x = 'I' then Some() else None
let (|J|_|) x = if x = 'J' then Some() else None
let (|K|_|) x = if x = 'K' then Some() else None
let (|L|_|) x = if x = 'L' then Some() else None
let (|M|_|) x = if x = 'M' then Some() else None
let (|N|_|) x = if x = 'N' then Some() else None
let (|O|_|) x = if x = 'O' then Some() else None
let (|P|_|) x = if x = 'P' then Some() else None
let (|Q|_|) x = if x = 'Q' then Some() else None
let (|R|_|) x = if x = 'R' then Some() else None
let (|S|_|) x = if x = 'S' then Some() else None
let (|T|_|) x = if x = 'T' then Some() else None
let (|U|_|) x = if x = 'U' then Some() else None
let (|V|_|) x = if x = 'V' then Some() else None
let (|W|_|) x = if x = 'W' then Some() else None
let (|X|_|) x = if x = 'X' then Some() else None
let (|Y|_|) x = if x = 'Y' then Some() else None
let (|Z|_|) x = if x = 'Z' then Some() else None

let isLetter x =
    match x with
    | A | B | C | D | E 
    | F | G | H | I | J
    | K | L | M | N | O
    | P | Q | R | S | T
    | U | V | W | X | Y
    | Z -> true
    | _ -> false

let nonLetters = ['.'; ';'; '<'; '>'] |> List.map isLetter
let letters    = ['A' .. 'Z']         |> List.map isLetter

nonLetters |> List.iter (fun r -> if r = true  then exit 1)
letters    |> List.iter (fun r -> if r = false then exit 1)

exit 0
