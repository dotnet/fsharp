[<NoEquality; NoComparison; DefaultAugmentation(false)>]
type R1 = { A : int; B : int }
[<NoEquality; NoComparison; DefaultAugmentation(false)>]
type R2 = { C : int; D : int }
[<NoEquality; NoComparison; DefaultAugmentation(false)>]
type R3 = { ...R1; ...R2 }
[<NoEquality; NoComparison; DefaultAugmentation(false)>]
type R4 = { ...R2; ...R1 }
