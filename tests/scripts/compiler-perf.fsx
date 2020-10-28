
#r "nuget: FSharp.Data, 3.3.3"

open System
open System.IO
open System.Diagnostics
open FSharp.Data


[<Literal>]
let repo = "https://github.com/dotnet/fsharp"
[<Literal>]
let repoApi = "https://api.github.com/repos/dotnet/fsharp"
type Commits = JsonProvider< const (repoApi + "/commits")>

type Pulls = JsonProvider< const (repoApi + "/pulls")>

//type Comments = JsonProvider< "https://api.github.com/repos/dotnet/fsharp/issues/848/comments">
//let comments = Comments.GetSamples()

let commits = Commits.GetSamples()
let pulls = Pulls.GetSamples()

let repoHeadSha = commits.[0].Sha

// Do performance testing on all open PRs that have [CompilerPerf] in the title
let buildSpecs = 
    [ //for pr in pulls do
      //   if pr.Title.Contains("[CompilerPerf]") then
      //       yield (pr.Head.Repo.CloneUrl, repoHeadSha, pr.Head.Ref, pr.Number) 
      //    ("https://github.com/dsyme/fsharp.git","53d633d6dba0d8f5fcd80f47f588d21cd7a2cff9", repoHeadSha, "no-casts", 1308);
      //yield ("https://github.com/forki/fsharp.git", "d0ab5fec77482e1280578f47e3257cf660d7f1b2", repoHeadSha, "foreach_optimization", 1303);
      yield (repo, "81d1d918740e9ba3cb2eb063b6f28c3139ca9cfa", "81d1d918740e9ba3cb2eb063b6f28c3139ca9cfa", 0);
      yield (repo, "81d1d918740e9ba3cb2eb063b6f28c3139ca9cfa", "1d36c75225436f8a7d30c4691f20d6118b657fec", 0);
      yield (repo, "81d1d918740e9ba3cb2eb063b6f28c3139ca9cfa", "2e4096153972abedae142da85cac2ffbcf57fe0a", 0);
      yield (repo, "81d1d918740e9ba3cb2eb063b6f28c3139ca9cfa", "af6ff33b5bc15951a6854bdf3b226db8f0e28b56", 0);
    ]


let time f = 
   let start = DateTime.UtcNow
   let res = f()
   let finish = DateTime.UtcNow
   res, finish - start

let runProc filename args startDir = 
    let timer = Stopwatch.StartNew()
    let procStartInfo = 
        ProcessStartInfo(
            UseShellExecute = false,
            FileName = filename,
            Arguments = args
        )
    match startDir with | Some d -> procStartInfo.WorkingDirectory <- d | _ -> ()

    let p = new Process(StartInfo = procStartInfo)
    let started = 
        try
            p.Start()
        with | ex ->
            ex.Data.Add("filename", filename)
            reraise()
    if not started then
        failwithf "Failed to start process %s" filename
    printfn "Started %s with pid %i" p.ProcessName p.Id
    p.WaitForExit()
    timer.Stop()
    printfn "Finished %s after %A milliseconds" filename timer.ElapsedMilliseconds
    p.ExitCode

let exec cmd args dir = 
    printfn "%s> %s %s" dir cmd args
    let result = runProc cmd args (Some dir)
    if result <> 0 then failwith (sprintf "FAILED: %s> %s %s" dir cmd args)

/// Build a specific version of the repo, run compiler perf tests and record the result
let build(cloneUrl, baseSha, ref, prNumber) =
    let branch = "build-" + string prNumber + "-" + baseSha + "-" + ref 
    let dirBase = __SOURCE_DIRECTORY__ 
    let dirBuild = "current"
    let dir = Path.Combine(dirBase, dirBuild) // "build-" + ref + "-" + sha.[0..7]
    //printfn "cloning %s branch %s into %s" cloneUrl ref dir
    if not (Directory.Exists dir) then 
       exec "git" ("clone " + repo + " " + dirBuild) dirBase  |> ignore
    let result = exec "git"  "reset --merge" dir
    let result = exec "git" "checkout main" dir
    let result = exec "git" "clean -xfd artifacts src vsintegration tests" dir
    let result = exec "git" ("checkout -B " + branch + " " + baseSha) dir
    let result = exec "git" ("pull  " + cloneUrl + " " + ref) dir
    let result, buildTime = time (fun () -> exec "cmd" "/C build.cmd -c Release" dir )
    let result, ngenTime = time (fun () -> exec "ngen" @"install artifacts\bin\fsc\Release\net472\fsc.exe" dir )

    let runPhase (test:string)  (flags:string)=
        printfn "copying compiler-perf-%s.cmd to %s" test dir
        File.Copy(sprintf "compiler-perf-%s.cmd" test,Path.Combine(dir,sprintf "compiler-perf-%s.cmd" test),true)
        printfn "running compiler-perf-%s.cmd in %s" test dir
        let result, time = time (fun () -> exec "cmd" (sprintf "/C compiler-perf-%s.cmd %s" test flags) dir )
        //File.Copy(Path.Combine(dir,sprintf "compiler-perf-%s.log" test),Path.Combine(dirBase,sprintf "compiler-perf-%s-%s.log" test branch),true)
        time.TotalSeconds 

    let runScenario name = 

        let parseonly = runPhase name "/parseonly"
        let checkonly = runPhase name  "/typecheckonly"
        let nooptimize = runPhase name "/optimize- /debug-"
        let debug = runPhase name "/optimize- /debug+"
        let optimize = runPhase name "/optimize+ /debug-"

        let times = 
            [ (sprintf "%s-startup-to-parseonly" name, parseonly)
              (sprintf "%s-parseonly-to-checkonly" name, checkonly - parseonly)
              (sprintf "%s-checkonly-to-nooptimize" name, nooptimize - checkonly)
              (sprintf "%s-checkonly-to-optimize" name, optimize - checkonly)
              (sprintf "%s-nooptimize-to-debug" name, debug - nooptimize) ]

        let timesHeaderText = (String.concat " " (List.map fst times))
        let timesText = (times |> List.map snd |> List.map (sprintf "%0.2f               ") |> String.concat " ")
        timesHeaderText, timesText

    let timesHeaderText, timesText = runScenario "bigfiles"

    let logFile = "compiler-perf-results.txt"
    let logHeader = sprintf "url ref  base computer build %s" timesHeaderText
    let computer = System.Environment.GetEnvironmentVariable("COMPUTERNAME")
    let logLine = sprintf "%s %-28s %s %s %0.2f %s" cloneUrl ref baseSha computer buildTime.TotalSeconds timesText
    let existing = if File.Exists logFile then File.ReadAllLines(logFile) else [| logHeader |]
    printfn "writing results %s"  logLine

    File.WriteAllLines(logFile, [| yield! existing; yield logLine |])
    ()


for info in buildSpecs do 
    try 
        build info
    with e -> 
        printfn "ERROR: %A - %s" info e.Message
       

