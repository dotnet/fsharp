// #Warnings
//<Expects status="Error" id="FS0001">All branches of an 'if' expression must return values of the same type as the first branch, which here is 'bool'. This branch returns a value of type 'string'.</Expects>

let x = 1
if x = 1 then true
else
    if x = 2 then "A"
    else "B"
    
exit 0