module CycleTest.TreeMod

// Uses sibling module ForestMod via namespace-relative reference (no `open`, no full prefix).
// This requires the dependency analyzer to try namespace-relative paths.
type TreeT =
    | Leaf of int
    | Branch of ForestMod.ForestT

let depth (t: TreeT) =
    match t with
    | Leaf _ -> 1
    | Branch f -> 1 + ForestMod.maxDepth f
