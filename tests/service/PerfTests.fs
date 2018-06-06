#if INTERACTIVE
#r "../../debug/fcs/net45/FSharp.Compiler.Service.dll" // note, run 'build fcs debug' to generate this, this DLL has a public API so can be used from F# Interactive
#r "../../packages/NUnit.3.5.0/lib/net45/nunit.framework.dll"
#load "FsUnit.fs"
#load "Common.fs"
#else
module FSharp.Compiler.Service.Tests.PerfTests
#endif


open NUnit.Framework
open FsUnit
open System
open System.IO
open System.Collections.Generic

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.SourceCodeServices

open FSharp.Compiler.Service.Tests.Common

// Create an interactive checker instance 
let internal checker = FSharpChecker.Create()

module internal Project1 = 
    open System.IO

    let fileNamesI = [ for i in 1 .. 10 -> (i, Path.ChangeExtension(Path.GetTempFileName(), ".fs")) ]
    let base2 = Path.GetTempFileName()
    let dllName = Path.ChangeExtension(base2, ".dll")
    let projFileName = Path.ChangeExtension(base2, ".fsproj")
    let fileSources = [ for (i,f) in fileNamesI -> (f, "module M" + string i) ]
    for (f,text) in fileSources do File.WriteAllText(f, text)
    let fileSources2 = [ for (i,f) in fileSources -> f ]

    let fileNames = [ for (_,f) in fileNamesI -> f ]
    let args = mkProjectCommandLineArgs (dllName, fileNames)
    let options = checker.GetProjectOptionsFromCommandLineArgs (projFileName, args)
    let parsingOptions, _ = checker.GetParsingOptionsFromCommandLineArgs(List.ofArray args)


[<Test>]
let ``Test request for parse and check doesn't check whole project`` () = 

    let backgroundParseCount = ref 0 
    let backgroundCheckCount = ref 0 
    checker.FileChecked.Add (fun x -> incr backgroundCheckCount)
    checker.FileParsed.Add (fun x -> incr backgroundParseCount)

    let pB, tB = FSharpChecker.GlobalForegroundParseCountStatistic, FSharpChecker.GlobalForegroundTypeCheckCountStatistic
    let parseResults1 = checker.ParseFile(Project1.fileNames.[5], Project1.fileSources2.[5], Project1.parsingOptions)  |> Async.RunSynchronously
    let pC, tC = FSharpChecker.GlobalForegroundParseCountStatistic, FSharpChecker.GlobalForegroundTypeCheckCountStatistic
    (pC - pB) |> shouldEqual 1
    (tC - tB) |> shouldEqual 0
    backgroundParseCount.Value |> shouldEqual 0
    backgroundCheckCount.Value |> shouldEqual 0
    let checkResults1 = checker.CheckFileInProject(parseResults1, Project1.fileNames.[5], 0, Project1.fileSources2.[5], Project1.options)  |> Async.RunSynchronously
    let pD, tD = FSharpChecker.GlobalForegroundParseCountStatistic, FSharpChecker.GlobalForegroundTypeCheckCountStatistic
    backgroundParseCount.Value |> shouldEqual 5
    backgroundCheckCount.Value |> shouldEqual 5
    (pD - pC) |> shouldEqual 0
    (tD - tC) |> shouldEqual 1

    let checkResults2 = checker.CheckFileInProject(parseResults1, Project1.fileNames.[7], 0, Project1.fileSources2.[7], Project1.options)  |> Async.RunSynchronously
    let pE, tE = FSharpChecker.GlobalForegroundParseCountStatistic, FSharpChecker.GlobalForegroundTypeCheckCountStatistic
    (pE - pD) |> shouldEqual 0
    (tE - tD) |> shouldEqual 1
    (backgroundParseCount.Value  <= 9) |> shouldEqual true // but note, the project does not get reparsed
    (backgroundCheckCount.Value  <= 9) |> shouldEqual true // only two extra typechecks of files

    // A subsequent ParseAndCheck of identical source code doesn't do any more anything
    let checkResults2 = checker.ParseAndCheckFileInProject(Project1.fileNames.[7], 0, Project1.fileSources2.[7], Project1.options)  |> Async.RunSynchronously
    let pF, tF = FSharpChecker.GlobalForegroundParseCountStatistic, FSharpChecker.GlobalForegroundTypeCheckCountStatistic
    (pF - pE) |> shouldEqual 0  // note, no new parse of the file
    (tF - tE) |> shouldEqual 0  // note, no new typecheck of the file
    (backgroundParseCount.Value <= 9) |> shouldEqual true // but note, the project does not get reparsed
    (backgroundCheckCount.Value <= 9) |> shouldEqual true // only two extra typechecks of files
    ()

