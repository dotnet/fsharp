[<NoEquality; NoComparison; DefaultAugmentation(false)>]
type R1 = { A : int; B : int }

let r1 = { A = 1; B = 2 }
let r1' = { ...r1; A = 99 }
