// #Warnings
//<Expects status="Error" id="FS0001">All branches of an 'if' expression must have the same type.</Expects>

let x = 1
if x = 1 then true
else
    if x = 2 then "A"
    else "B"
    
exit 0