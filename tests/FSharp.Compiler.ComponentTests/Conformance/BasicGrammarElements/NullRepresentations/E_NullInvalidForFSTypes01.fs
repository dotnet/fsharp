// #Regression #Conformance #TypesAndModules 
#light

// Verify that 'null' is not a valid value for F# types

//<Expects id="FS0043" status="error">The type 'int list' does not have 'null' as a proper value</Expects>
//<Expects id="FS0043" status="error">The type 'DU' does not have 'null' as a proper value</Expects>
//<Expects id="FS0043" status="error">The type 'RecType' does not have 'null' as a proper value</Expects>

type RecType = {RTag1 : int * string; RTag2 : int list option}

type DU =
    | Tag1 of RecType
    | Tag2

let lst = [1 .. 10]
let du  = Tag2
let arc = {RTag1 = (1, "one"); RTag2 = Some([1;2;3])}

if lst = null then exit 1
if du  = null then exit 1
if arc = null then exit 1

// Should have compile errors
exit 1
