module FcsSmokeTest.Program

open FSharp.Compiler.CodeAnalysis
open System.IO

let testDir =
    let d = Path.Combine(Path.GetTempPath(), "fcs-fileorder-test")
    if Directory.Exists d then Directory.Delete(d, true)
    Directory.CreateDirectory d |> ignore
    d

let writeFile name (content: string) =
    let path = Path.Combine(testDir, name)
    File.WriteAllText(path, content)
    path

let bPath = writeFile "FileB.fs" """module Test.B
let value = 42
"""

let aPath = writeFile "FileA.fs" """module Test.A
let useB () = Test.B.value + 1
"""

let mainPath = writeFile "Main.fs" """module Test.Main
[<EntryPoint>]
let main _ =
    printfn "%d" (Test.A.useB ())
    0
"""

let checker = FSharpChecker.Create()

let sourceFiles = [| aPath; bPath; mainPath |]

let buildOptions baseFlags =
    { ProjectFileName = Path.Combine(testDir, "test.fsproj")
      ProjectId = None
      SourceFiles = sourceFiles
      OtherOptions = baseFlags
      ReferencedProjects = [||]
      IsIncompleteTypeCheckEnvironment = false
      UseScriptResolutionRules = false
      LoadTime = System.DateTime.Now
      UnresolvedReferences = None
      OriginalLoadReferences = []
      Stamp = None }

[<EntryPoint>]
let main _ =
    printfn "=== Checking WITHOUT --file-order-auto (expect FAIL on wrong order) ==="
    let resultManual =
        checker.ParseAndCheckProject(buildOptions [|
            "--targetprofile:netcore"
        |])
        |> Async.RunSynchronously

    printfn "  diagnostics: %d" resultManual.Diagnostics.Length
    let manualErrors =
        resultManual.Diagnostics
        |> Array.filter (fun d -> d.Severity = FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity.Error)
    printfn "  errors: %d" manualErrors.Length
    if manualErrors.Length = 0 then
        printfn "  UNEXPECTED: manual mode produced no errors — test setup may be wrong"

    printfn ""
    printfn "=== Checking WITH --file-order-auto+ (expect PASS) ==="
    let resultAuto =
        checker.ParseAndCheckProject(buildOptions [|
            "--targetprofile:netcore"
            "--file-order-auto+"
        |])
        |> Async.RunSynchronously

    printfn "  diagnostics: %d" resultAuto.Diagnostics.Length
    let autoErrors =
        resultAuto.Diagnostics
        |> Array.filter (fun d -> d.Severity = FSharp.Compiler.Diagnostics.FSharpDiagnosticSeverity.Error)
    printfn "  errors: %d" autoErrors.Length
    for d in resultAuto.Diagnostics |> Array.truncate 5 do
        printfn "    %A %s:%d: %s" d.Severity (Path.GetFileName d.FileName) d.StartLine d.Message

    if autoErrors.Length = 0 then
        printfn "  PASS: auto file order in FCS resolved the dependency"
        0
    else
        printfn "  FAIL: auto file order did not work in FCS"
        1
