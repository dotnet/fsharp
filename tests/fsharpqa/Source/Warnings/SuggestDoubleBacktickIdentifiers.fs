// #Warnings
//<Expects status="Error" span="(7,11,7,25)" id="FS0039">The value, constructor, namespace or type 'longe name' is not defined.</Expects>

module N =
    let ``longer name`` = "hallo"

let x = N.``longe name``


exit 0