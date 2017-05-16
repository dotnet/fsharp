// #Warnings
//<Expects status="Error" span="(8,11,8,25)" id="FS0039">The value, constructor, namespace or type 'longe name' is not defined.</Expects>
//<Expects>Maybe you want one of the following:\s+longer name</Expects>

module N =
    let ``longer name`` = "hallo"

let x = N.``longe name``


exit 0