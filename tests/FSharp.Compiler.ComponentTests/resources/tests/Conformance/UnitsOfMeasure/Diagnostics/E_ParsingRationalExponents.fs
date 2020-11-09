// #Regression #Conformance #UnitsOfMeasure #Diagnostics #RatExp
#light
//<Expects id="FS0620" span="(11,19-11,20)" status="error">Unexpected integer literal in unit-of-measure expression</Expects>
//<Expects id="FS0010" span="(12,20-12,21)" status="error">Unexpected symbol '\)' in binding\. Expected integer literal or other token</Expects>
//<Expects id="FS0010" span="(13,18-13,19)" status="error">Unexpected infix operator in binding\. Expected integer literal, '-' or other token</Expects>
 
[<Measure>] type kg
[<Measure>] type s

// Parentheses are required
let x2 = 2.0<kg^3/2>
let x4 = 2.0<kg^(3/)>
let x5 = 2.0<kg^(/2)>
