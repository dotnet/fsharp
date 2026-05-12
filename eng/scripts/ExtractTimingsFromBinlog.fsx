/// Script to extract F# compiler timing data from MSBuild binary logs
/// Usage: dotnet fsi ExtractTimingsFromBinlog.fsx <path-to-binlog-file>

#r "nuget: MSBuild.StructuredLogger"

open System
open Microsoft.Build.Logging.StructuredLogger

let binlogPath = 
    let args = Environment.GetCommandLineArgs()
    // When running with dotnet fsi, args are: [0]=dotnet; [1]=fsi.dll; [2]=script.fsx; [3...]=args
    let scriptArgs = args |> Array.skipWhile (fun a -> not (a.EndsWith(".fsx"))) |> Array.skip 1
    if scriptArgs.Length > 0 then
        scriptArgs.[0]
    else
        failwith "Usage: dotnet fsi ExtractTimingsFromBinlog.fsx <path-to-binlog-file>"

if not (IO.File.Exists(binlogPath)) then
    printfn "Binlog file not found: %s" binlogPath
    exit 1

let rec findProject (node: TreeNode) =
    match node with
    | null -> None
    | :? Project as p -> Some p.Name
    | _ -> findProject node.Parent

let build = BinaryLog.ReadBuild(binlogPath)

let isTimingLine (s: string) =
    s.StartsWith("|") || (s.Length > 10 && s.StartsWith("-") && s.TrimEnd() |> Seq.forall (fun c -> c = '-'))

let mutable foundFscTasks = false
let mutable foundTimingData = false

build.VisitAllChildren<Task>(fun task ->
    if task.Name = "Fsc" then
        foundFscTasks <- true
        
        let projectName = 
            match findProject task with
            | Some name -> name
            | None -> "Unknown Project"
        
        let timingMessages = 
            task.Children
            |> Seq.choose (function
                | :? Message as m when isTimingLine m.Text -> Some m.Text
                | _ -> None)
            |> Seq.toList
        
        if timingMessages.Length > 0 then
            foundTimingData <- true
            printfn "=== %s ===" projectName
            for msg in timingMessages do
                printfn "  %s" msg
            printfn ""
)

if not foundFscTasks then
    printfn "No Fsc tasks found in binlog."
elif not foundTimingData then
    printfn "Fsc tasks found but no timing data present. Was --times flag set?"

exit 0
