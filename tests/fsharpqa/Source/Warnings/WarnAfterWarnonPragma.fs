// #Warnings
//<Expects status="Warning" span="(8,11)" id="FS0025">The result of this equality expression has type 'bool' and is implicitly discarded. Consider using 'let' to bind the result to a name, e.g. 'let result = expression'.</Expects>

#nowarn "25"
let foo = function [] -> true
#warnon "25"

let bar = function [] -> true

exit 0
