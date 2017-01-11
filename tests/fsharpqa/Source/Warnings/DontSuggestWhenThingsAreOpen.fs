// #Warnings
//<Expects status="Error" id="FS0599">Missing qualification after '.'</Expects>

module N =
    let name = "hallo"

type T =
    static member myMember = 1

let x = N.


exit 0