
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

let commits = Commits.GetSamples()
let pulls = Pulls.GetSamples()

//let buildSpecs = [ for pr in pulls -> (pr.Head.Repo.CloneUrl, pr.Head.Sha, pr.Base.Sha, pr.Head.Ref, pr.Number) ]

let buildSpecs = 
  [ 
    ("https://github.com/dsyme/visualfsharp.git","53d633d6dba0d8f5fcd80f47f588d21cd7a2cff9", "0247247d480340c27ce7f7de9b2fbc3b7c598b03", "no-casts", 1308);
    ("https://github.com/forki/visualfsharp.git", "d0ab5fec77482e1280578f47e3257cf660d7f1b2", "0247247d480340c27ce7f7de9b2fbc3b7c598b03", "foreach_optimization", 1303);
    (repo, "0247247d480340c27ce7f7de9b2fbc3b7c598b03", "0247247d480340c27ce7f7de9b2fbc3b7c598b03", "master", 0);
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


//build(repo, "0247247d480340c27ce7f7de9b2fbc3b7c598b03", "master", 102)
//build(repo, "830a1d4379454d8876fd57b13c16e033c1a7eb1c", "master", 101)
//build(repo, "f47a6bb73e51ce43596c793a1e0a887664ffcfd7", "master", 100)

       
for info in buildSpecs do 
    try 
        build info
    with e -> 
        printfn "ERROR: %A - %s" info e.Message
       

#endif
