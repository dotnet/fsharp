// #Regression #Diagnostics 
// Regression test for Dev10:907600
//<Expects status="error" span="(5,9-5,17)" id="FS0741">Unable to parse format string 'The # formatting modifier is invalid in F#'$</Expects>
// Unable to parse format string 'The # formatting modifier is invalid in F#'
printfn "%#110c"  'a' 
