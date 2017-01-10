// #Warnings
//<Expects status="Error" id="FS0039">The value or constructor 'N' is not defined.$</Expects>

module N =
    let name = "hallo"

type T =
    static member myMember = 1

let x = N.


exit 0