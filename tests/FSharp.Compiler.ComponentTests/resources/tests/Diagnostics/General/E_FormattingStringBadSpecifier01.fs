// #Regression #Diagnostics 
// Regression test for Dev10:907600
//<Expects status="error" span="(5,9-5,17)" id="FS0741">Unable to parse format string 'Bad format specifier: '/''$</Expects>
// Unable to parse format string 'Bad format specifier: '/''
printfn "%/110c"  'a' 
