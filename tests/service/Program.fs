

open System

[<EntryPoint>]
let main argv = 
    FSharp.Compiler.Service.Tests.DepResolving.Test()
    0
    //Environment.CurrentDirectory <- "c:/projekty/fsharp/heuristic/src/Compiler"
    //FSharp.Compiler.Service.Tests.RunCompiler.runCompiler()