module CycleTest.Program

[<EntryPoint>]
let main _argv =
    let t = CycleTest.TreeMod.Branch(CycleTest.ForestMod.ForestT [ CycleTest.TreeMod.Leaf 1; CycleTest.TreeMod.Leaf 2 ])
    printfn "Tree depth: %d" (CycleTest.TreeMod.depth t)
    0
