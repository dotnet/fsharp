// #Regression #Conformance #TypeInference 
// Regression test for FSHARP1.0:2364
// Clashing union case label and property
//<Expects id="FS0023" span="(14,17-14,27)" status="notin">The member 'ClashingID' can not be defined because the name 'ClashingID' clashes with the union case 'ClashingID' in this type or module</Expects>
//<Expects id="FS0812" span="(17,13-17,34)" status="error">The syntax 'expr\.id' may only be used with record labels, properties and fields</Expects>

module M=
 type Suit =
    | Spade

 type Card =
    | Ace   of Suit
    | ClashingID of int * Suit
    //member this.ClashingID = 2
    member this.NonClashingID = 2

 let _err = Ace(Spade).ClashingID        // Error
 let _ok  = Ace(Spade).NonClashingID     // OK
