open Prelude

// Test a simple out argument
module TryGetValueTests = 
    let d = dict [ (3,4) ]
    let mutable res = 9
    let v =  d.TryGetValue(3, &res)
    check "cweweoiwekl1" v true
    check "cweweoiwekl2" res 4
    let v2 =  d.TryGetValue(5, &res)
    check "cweweoiwekl3" v2 false
    check "cweweoiwekl4" res 4