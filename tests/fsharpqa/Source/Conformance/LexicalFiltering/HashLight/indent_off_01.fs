// #Regression #Conformance #LexFilter 
#indent "off"
// Regression test for FSHARP1.0:1078
// The opposit of #light is (for now) #indent "off"
//<Expects id="FS0003" status="error">This value is not a function and cannot be applied</Expects>

let SimpleSample() =
    let x = 10 + 12 - 3 in
    let y = x * 2 + 1 in 
    let r1,r2 = x/3, x%3 in
    (x,y,r1,r2)

exit 0
