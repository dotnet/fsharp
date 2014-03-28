
[<EntryPoint>]
let main _ = 
    InstanceTests.checkAll()
    try printfn "trigger = %A" StaticTest1.trigger with _ -> printfn "Good, got a static initialization failure"
    StaticTest1.checkAll()
    try printfn "trigger = %A" StaticTest2.trigger with _ -> printfn "Good, got a static initialization failure"
    StaticTest2.checkAll()
    try printfn "trigger = %A" StaticTest3.trigger with _ -> printfn "Good, got a static initialization failure"
    StaticTest3.checkAll()
    GenericStaticTest4.checkAll()
    try printfn "trigger = %A" StaticTest5.trigger with _ -> printfn "Good, got a static initialization failure"
    StaticTest5.checkAll()
    GenericStaticTest6.checkAll()
    0

