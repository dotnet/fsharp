// #Warnings
//<Expects status="Error" id="FS0001">This 'if' expression is missing an 'else' branch. Because 'if' is an expression, and not a statement, add an 'else' branch which also returns a value of type 'string'.</Expects>

let x = 10
let y =
   if x > 10 then ("test")
    
exit 0