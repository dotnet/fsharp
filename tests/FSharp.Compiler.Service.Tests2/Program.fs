module FSharp.Compiler.Service.Tests2.Program

open System
open FSharp.Compiler.Service.Tests

let runCompiler () =
    Environment.CurrentDirectory <- "c:/projekty/fsharp/heuristic/src/Compiler"
    RunCompiler.runCompiler()

[<EntryPoint>]
let main _ = 
    //runCompiler ()
    //TestDepResolving.TestHardcodedFiles()
    //TestDepResolving.TestProject(@"C:\projekty\fsharp\heuristic\tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj")
    //TestDepResolving.TestProject(@"C:\projekty\fsharp\fsharp_main\src\Compiler\FSharp.Compiler.Service.fsproj")
    RunCompiler.runGrapher()
    0