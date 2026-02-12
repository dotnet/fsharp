// #Regression #Diagnostics 
// Regression test for FSHARP1.0:5183
//<Expects status="error" span="(5,1-5,4)" id="FS3123">Missing 'do' in 'for' expression\. Expected 'for <pat> in <expr> do <expr>'\.$</Expects>
//<Expects status="error" span="(5,7-5,9)" id="FS3100">Expected an expression after this point$</Expects>
for x in 