// #Warnings
//<Expects status="Error" span="(6,19)" id="FS0001">The 'if' expression is missing an 'else' branch. The 'then' branch has type 'string'. Because 'if' is an expression, and not a statement, add an 'else' branch which returns a value of the same type.</Expects>

let x = 10
let y =
   if x > 10 then "test"
    
exit 0