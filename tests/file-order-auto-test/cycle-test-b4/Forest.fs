module CycleTest.ForestMod

// Uses sibling module TreeMod via namespace-relative reference.
type ForestT = ForestT of TreeMod.TreeT list

let maxDepth (ForestT trees) =
    if List.isEmpty trees then 0
    else trees |> List.map TreeMod.depth |> List.max
