type Attr1Attribute () = inherit System.Attribute ()
type Attr2Attribute () = inherit System.Attribute ()

[<NoEquality; NoComparison; DefaultAugmentation(false)>]
type R1 = { [<Attr1>] A : int; [<Attr1>] B : int }
[<NoEquality; NoComparison; DefaultAugmentation(false)>]
type R2 = { ...R1; [<Attr2>] A : string }
