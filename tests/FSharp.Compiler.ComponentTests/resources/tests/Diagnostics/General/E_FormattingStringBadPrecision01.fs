// #Regression #Diagnostics 
// Regression test for Dev10:907600
//<Expects status="error" span="(5,9-5,13)" id="FS0741">Unable to parse format string 'Bad precision in format specifier'$</Expects>
// Unable to parse format string 'Bad precision in format specifier'
printfn "%*" 'a'

// This has nothing to do with the test above... but I liked it
let f = printfn "%*c" 1
f('x')
