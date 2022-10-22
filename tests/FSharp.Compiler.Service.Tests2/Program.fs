module FSharp.Compiler.Service.Tests2.Program

open System

let runCompiler () =
    Environment.CurrentDirectory <- "c:/projekty/fsharp/heuristic/src/Compiler"
    FSharp.Compiler.Service.Tests.RunCompiler.runCompiler()

[<EntryPoint>]
let main _ = 
    TestDepResolving.TestProject(@"C:\projekty\fsharp\heuristic\tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj")
    // runCompiler ()
    0