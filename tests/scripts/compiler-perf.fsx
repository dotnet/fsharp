
#if FETCH_PACKAGES
open System
open System.IO
 
Environment.CurrentDirectory <- __SOURCE_DIRECTORY__
 
if not (File.Exists "paket.exe") then let url = "https://github.com/fsprojects/Paket/releases/download/3.4.0/paket.exe" in use wc = new Net.WebClient() in let tmp = Path.GetTempFileName() in wc.DownloadFile(url, tmp); File.Move(tmp,Path.GetFileName url);;
 
// Step 1. Resolve and install the packages
 
#r "paket.exe"
 
if not (Directory.Exists "script-packages") then Directory.CreateDirectory("script-packages") |> ignore
Paket.Dependencies.Install("""
source https://nuget.org/api/v2
nuget FSharp.Data
nuget FAKE
""","script-packages");;
 
#else



#I "script-packages/packages/FAKE/tools"
#I "script-packages/packages/FSharp.Data/lib/net40"
#r "script-packages/packages/FAKE/tools/FakeLib.dll"
#r "script-packages/packages/FSharp.Data/lib/net40/FSharp.Data.dll"

open System
open System.IO
open Fake
open Fake.Git
open FSharp.Data

Fake.Git.Information.describe

[<Literal>]
let repo = "https://github.com/Microsoft/visualfsharp"
[<Literal>]
let repoApi = "https://api.github.com/repos/Microsoft/visualfsharp"
type Commits = JsonProvider< const (repoApi + "/commits")>

type Pulls = JsonProvider< const (repoApi + "/pulls")>

//type Comments = JsonProvider< "https://api.github.com/repos/Microsoft/visualfsharp/issues/848/comments">
//let comments = Comments.GetSamples()

let commits = Commits.GetSamples()
let pulls = Pulls.GetSamples()

let repoHeadSha = commits.[0].Sha

// Do performance testing on all open PRs that have [CompilerPerf] in the title
let buildSpecs = 
    [ for pr in pulls do
         //let comments =  Comments.Load(pr.CommentsUrl) 
         if pr.Title.Contains("[CompilerPerf]") then
             yield (pr.Head.Repo.CloneUrl, pr.Head.Sha, repoHeadSha, pr.Head.Ref, pr.Number) 
      //    ("https://github.com/dsyme/visualfsharp.git","53d633d6dba0d8f5fcd80f47f588d21cd7a2cff9", repoHeadSha, "no-casts", 1308);
      //yield ("https://github.com/forki/visualfsharp.git", "d0ab5fec77482e1280578f47e3257cf660d7f1b2", repoHeadSha, "foreach_optimization", 1303);
      yield (repo, repoHeadSha, repoHeadSha, "master", 0);
    ]


let time f = 
   let start = DateTime.UtcNow
   let res = f()
   let finish = DateTime.UtcNow
   res, finish - start


let exec cmd args dir = 
    printfn "%s> %s %s" dir cmd args
    let result = Shell.Exec(cmd,args,dir) 
    if result <> 0 then failwith (sprintf "FAILED: %s> %s %s" dir cmd args)

/// Build a specific version of the repo, run compiler perf tests and record the result
let build(cloneUrl,sha:string,baseSha,ref,prNumber) =
    let branch = "build-" + string prNumber + "-" + ref + "-" + sha.[0..7]
    let dirBase = __SOURCE_DIRECTORY__ 
    let dirBuild = "current"
    let dir = Path.Combine(dirBase, dirBuild) // "build-" + ref + "-" + sha.[0..7]
    //printfn "cloning %s branch %s into %s" cloneUrl ref dir
    if not (Directory.Exists dir) then 
       exec "git" ("clone " + repo + " " + dirBuild) dirBase  |> ignore
    let result = exec "git"  "reset --merge" dir
    let result = exec "git" "checkout master" dir
    let result = exec "git" "clean -f -x" dir
    let result = exec "git" ("checkout -B " + branch + " master") dir
    let result = exec "git" ("pull  " + cloneUrl + " " + ref) dir
    let result, buildTime = time (fun () -> exec "cmd" "/C build.cmd release proto net40 notests" dir )
    let result, ngenTime = time (fun () -> exec "ngen" @"install Release\net40\bin\fsc.exe" dir )

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
    let logHeader = sprintf "url ref  sha base build %s" timesHeaderText
    let logLine = sprintf "%s %s %s %s %0.2f %s" cloneUrl ref sha baseSha buildTime.TotalSeconds timesText
    let existing = if File.Exists logFile then File.ReadAllLines(logFile) else [| logHeader |]
    printfn "writing results %s"  logLine

    File.WriteAllLines(logFile, [| yield! existing; yield logLine |])
    ()


for info in buildSpecs do 
    try 
        build info
    with e -> 
        printfn "ERROR: %A - %s" info e.Message
       

#endif
