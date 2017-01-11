// #Warnings
//<Expects status="Error" span="(7,11,7,25)" id="FS0039">The value, constructor, namespace or type 'longe name' is not defined. Maybe you want one of the following: longer name$</Expects>

module N =
    let ``longer name`` = "hallo"

let x = N.``longe name``


exit 0