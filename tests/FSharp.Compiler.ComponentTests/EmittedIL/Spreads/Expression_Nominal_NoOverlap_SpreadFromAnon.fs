[<NoEquality; NoComparison; DefaultAugmentation(false)>]
type R2 = { A : int; B : int; C : int }

let r2 = { ...{| A = 1; B = 2 |}; C = 3 }
