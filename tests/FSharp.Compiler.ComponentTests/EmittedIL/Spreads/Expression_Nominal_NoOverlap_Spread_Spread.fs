[<NoEquality; NoComparison; DefaultAugmentation(false)>]
type R1 = { A : int; B : int }
[<NoEquality; NoComparison; DefaultAugmentation(false)>]
type R2 = { C : int; D : int }
[<NoEquality; NoComparison; DefaultAugmentation(false)>]
type R3 = { A : int; B : int; C : int; D : int }

let r1 = { A = 1; B = 2 }
let r2 = { C = 3; D = 4 }
let r3 = { ...r1; ...r2 }
let r3' = { ...r2; ...r3 }

let r1' = {| A = 1; B = 2 |}
let r2' = {| C = 3; D = 4 |}
let r3'' = { ...r1; ...r2 }
let r3''' = { ...r2; ...r3 }
