// #Warnings
//<Expects status="Error" span="(7,10)" id="FS0001">All branches of an 'if' expression must return the same type. This expression was expected to have type 'string' but here has type 'int'.</Expects>

let test = 100
let y =
    if test > 10 then "test"
    else 123
    
exit 0