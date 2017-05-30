// #Warnings
//<Expects status="Error" id="FS0001">The 'if' expression needs to return type 'bool'</Expects>

let x = 1
if x = 1 then true
else
    if x = 2 then "A"
    else "B"
    
exit 0