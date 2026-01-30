// #Regression #Conformance #LexFilter #ReqNOMT 
// Regression test for FSHARP1.0:1078
// The #light is default in fsi.exe
//<Expects status="success"></Expects>
let SimpleSample() =
    let x = 10 + 12 - 3 
    let y = x * 2 + 1  
    let r1,r2 = x/3, x%3 
    (x,y,r1,r2)

exit 0;;
