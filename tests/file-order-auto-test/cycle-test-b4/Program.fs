module CycleTest.Program

open CycleTest

[<EntryPoint>]
let main _argv =
    let t = TreeMod.Branch(ForestMod.ForestT [ TreeMod.Leaf 1; TreeMod.Leaf 2 ])
    printfn "Tree depth: %d" (TreeMod.depth t)
    0
