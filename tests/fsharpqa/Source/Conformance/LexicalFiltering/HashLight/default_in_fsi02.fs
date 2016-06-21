// #Conformance #LexFilter #ReqNOMT 
//<Expects status="notin">#light</Expects>
module TestModule 
let SimpleSample() =
    let x = 10 + 12 - 3 
    let y = x * 2 + 1  
    let r1,r2 = x/3, x%3 
    (x,y,r1,r2)
