// #Warnings
//<Expects status="Warning" span="(8,5)" id="FS0020">The result of this equality expression has type 'bool' and is implicitly discarded. Consider using 'let' to bind the result to a name, e.g. 'let result = expression'.</Expects>

let x = 10
let y = 20

let changeX() =
    y * x = 20
    y = 30
    
exit 0