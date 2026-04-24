module CycleTest.TreeMod

type TreeT =
    | Leaf of int
    | Branch of CycleTest.ForestMod.ForestT

let depth (t: TreeT) =
    match t with
    | Leaf _ -> 1
    | Branch f -> 1 + CycleTest.ForestMod.maxDepth f
