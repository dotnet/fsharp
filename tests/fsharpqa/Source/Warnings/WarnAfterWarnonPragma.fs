// #Warnings
//<Expects status="Warning" span="(8,11)" id="FS0025">Incomplete pattern matches on this expression.</Expects>

#nowarn "25"
let foo = function [] -> true
#warnon "25"

let bar = function [] -> true

exit 0
