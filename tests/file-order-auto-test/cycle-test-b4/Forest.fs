module CycleTest.ForestMod

type ForestT = ForestT of CycleTest.TreeMod.TreeT list

let maxDepth (ForestT trees) =
    if List.isEmpty trees then 0
    else trees |> List.map CycleTest.TreeMod.depth |> List.max
