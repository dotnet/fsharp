module MixedRangeVersionTest
        
let a: int list = [ yield 1..10 ]
let a1: int list = [ yield! 1..10 ]

let b: int list = [ yield [1..10] ]
let b: int list = [ yield! [1..10] ]

let c: int seq = seq { yield seq { 1..10 } }
let c1: int seq = seq { yield! seq { 1..10 } }

let d: int list = [ if true then yield 1 else yield 1..10]
let d1: int list = [ if true then yield 1 else yield! 1..10]
let d2: int list = [ if true then yield 1..10]
let d3: int list = [ if true then yield! 1..10]

let e: int list =  [if true then yield 1 else yield [1..10]]
let e1: int list =  [if true then yield 1 else yield! [1..10]]
let e2 : int list =  [if true then yield [1..10]]
let e3 : int list =  [if true then yield! [1..10]]

let g: int seq = seq { if true then yield 1 else yield seq { 1..10 } }
let g1: int seq = seq { if true then yield 1 else yield! seq { 1..10 } }
let g2: int seq = seq { if true then yield seq { 1..10 } }
let g3: int seq = seq { if true then yield! seq { 1..10 } }

let h: int list = [ match true with | true -> yield 1 | false -> yield 1..10]
let h1: int list = [ match true with | true -> yield 1 | false -> yield! 1..10]

let i: int list = [ match true with | true -> yield 1 | false -> yield [1..10]]
let i1: int list = [ match true with | true -> yield 1 | false -> yield! [1..10]]

let j: int seq = seq { match true with | true -> yield 1 | false -> yield seq { 1..10 } }
let j1: int seq = seq { match true with | true -> yield 1 | false -> yield! seq { 1..10 } }