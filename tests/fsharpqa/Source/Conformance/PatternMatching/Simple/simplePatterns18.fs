// #Conformance #PatternMatching 
#light

// Pattern match a short unicode literal

type Suit =
    | Spade
    | Club
    | Heart
    | Diamond

let spade, club, heart, diamond = '\u2660', '\u2663', '\u2665', '\u2666'

let getSuit c =
    match c with 
    | '\u2660' -> Some(Spade)
    | '\u2663' -> Some(Club)
    | '\u2665' -> Some(Heart)
    | '\u2666' -> Some(Diamond)
    | _ -> None

if getSuit 'a' <> None then exit 1
if getSuit 'z' <> None then exit 1

if getSuit spade   <> Some(Spade)   then exit 1
if getSuit club    <> Some(Club)    then exit 1
if getSuit heart   <> Some(Heart)   then exit 1
if getSuit diamond <> Some(Diamond) then exit 1

exit 0
