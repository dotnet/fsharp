// #Warnings
//<Expects status="Error" span="(7,10)" id="FS0001">All branches of an 'if' expression must return values of the same type as the first branch, which here is 'string'. This branch returns a value of type 'int'.</Expects>

let test = 100
let y =
    if test > 10 then "test"
    else 123
    
exit 0
