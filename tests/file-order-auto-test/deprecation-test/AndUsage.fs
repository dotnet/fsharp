module AndUsage

type Tree =
    | Leaf
    | Branch of Forest
and Forest = Tree list

type Even =
    | EZero
    | ESucc of Odd
and Odd = OSucc of Even

let _sample : Tree = Leaf
let _other  : Even = EZero
