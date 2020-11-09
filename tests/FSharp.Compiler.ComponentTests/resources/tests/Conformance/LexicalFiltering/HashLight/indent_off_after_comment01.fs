// #Regression #Conformance #LexFilter 
// Regression test for FSHARP1.0:1078
// The opposit of #light is (for now) #indent "off"
//<Expects status="success"></Expects>
#indent "off"
module M
let SimpleSample() =
    let x = 10 + 12 - 3 in
    let y = x * 2 + 1 in 
    let r1,r2 = x/3, x%3 in
    (x,y,r1,r2)
