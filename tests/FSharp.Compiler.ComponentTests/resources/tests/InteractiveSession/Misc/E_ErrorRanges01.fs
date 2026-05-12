// #Regression #NoMT #FSI 
// Regression test for FSharp1.0:2815 - fsi.exe underlines one too many characters for error spans (off by 1 error)
//<Expects id="FS0025" status="warning">Incomplete pattern matches on this expression.</Expects>

type Suit =
    | Club
    | Heart

type Card =
    | Ace of Suit
    | ValueCard of int * Suit

let test card =
    match card with
    | ValueCard(5, Club) -> true

exit 0
