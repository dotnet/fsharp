
module MixedRangeVersionTest
        
let a = seq { yield! seq { 1..10 }; 19 }
let b = [-3; yield! [1..10]]
let c = [|-3; yield! [|1..10|]; 19|]

let d = seq { 1..10; 19 }
let e = [-3; 1..10]
let f = [|-3; 1..10; 19|]
