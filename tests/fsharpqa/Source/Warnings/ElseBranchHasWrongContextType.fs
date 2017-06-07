// #Warnings
//<Expects status="Error" id="FS0001">The 'if' expression needs to have type 'bool'</Expects>

let x = 1
let y : bool = 
    if x = 2 then "A"
    else "B"
    
exit 0