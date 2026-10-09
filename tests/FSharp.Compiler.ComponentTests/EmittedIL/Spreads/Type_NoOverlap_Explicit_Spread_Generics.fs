[<NoEquality; NoComparison; DefaultAugmentation(false)>]
type R1<'a> = { A : 'a }
[<NoEquality; NoComparison; DefaultAugmentation(false)>]
type R2<'a> = { B : 'a }
[<NoEquality; NoComparison; DefaultAugmentation(false)>]
type R3<'a> = { ...R1<'a>; ...R2<'a> }
