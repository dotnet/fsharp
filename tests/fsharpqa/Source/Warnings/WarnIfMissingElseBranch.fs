// #Warnings
//<Expects status="Error" span="(6,4)" id="FS0001">You have not supplied the "else" case for this expression. If / then is an expression and so it must always return a result of the same type in all cases.</Expects>

let x = 10
let y =
   if x > 10 then "test"
    
exit 0