
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

//let buildSpecs = [ for pr in pulls -> (pr.Head.Repo.CloneUrl, pr.Head.Sha, pr.Head.Ref, pr.Number) ]

let buildSpecs = 
  [
    ("https://github.com/forki/visualfsharp.git", "d0ab5fec77482e1280578f47e3257cf660d7f1b2", "foreach_optimization", 1303);

    (repo, "0247247d480340c27ce7f7de9b2fbc3b7c598b03", "master", 0);
   (*
   ("https://github.com/eriawan/visualfsharp.git",
    "bac253ffc07c8aa26585576ca66c4a5d825bc4bf", "Add_VSSDK_notes_DEVGUIDE",
    1302);
   ("https://github.com/dsyme/visualfsharp.git",
    "9147fc09f3c5cca4dce7c3ba5f75a720542e5527", "fix-105", 1300);
   ("https://github.com/forki/visualfsharp.git",
    "70d144eee0d13f776d79c2a559b8aa762a4c372a", "inference", 1297);
   ("https://github.com/dsyme/visualfsharp.git",
    "4caab335c3ea2a60de08c85651d7253b74a37f9c", "fix-12", 1295);
   ("https://github.com/forki/visualfsharp.git",
    "a09df1a98209326c60f840d0d7ab58c7cbf0c8a1", "abbrev", 1289);
   ("https://github.com/forki/visualfsharp.git",
    "989427119e71533cff51a82f688878aa7c017faa", "wrongarity", 1286);
   ("https://github.com/forki/visualfsharp.git",
    "5eceaf1f68567fee0ae4a5d734254e2525013861", "record-1280", 1283);
   ("https://github.com/dsyme/visualfsharp.git",
    "599c840eda2449f8e6bb97cb940d1e55f4afef14", "srtp-1", 1278);
   ("https://github.com/forki/visualfsharp.git",
    "3e7ddeeffe6e454d5107e291151a73aa4e741b4f", "no-matching", 1230);
   ("https://github.com/forki/visualfsharp.git",
    "ebe9a8ab483966d44d2877c1c7a5e6433d0adec4", "tuple", 1229);
   ("https://github.com/forki/visualfsharp.git",
    "57ca8135f3f13c26ba9dc3c69f8e6948e9f8225a", "prop", 1228);
   ("https://github.com/forki/visualfsharp.git",
    "a159356ae20747c5ead7742035828e2ef57365ce", "ctors", 1219);
   ("https://github.com/KevinRansom/visualfsharp.git",
    "50fe9b27b62a11dcbf382315fc7bbb442982114a", "crossgen", 1197);
   ("https://github.com/enricosada/visualfsharp.git",
    "c0636f1926439c6e962ff2aba1c47f54e22eed21", "remove_fsharp_suite_bats",
    1137);
   ("https://github.com/forki/visualfsharp.git",
    "1a32f22034ccf172861f6259779a336bb19227c5", "assignment-warning", 1115);
   ("https://github.com/forki/visualfsharp.git",
    "f15245b005c4f27209db774dadc3b1cb9f87decb", "sigdata", 1098);
   ("https://github.com/enricosada/visualfsharp.git",
    "9e72d0c1b22f2a8d91e7e7e047def11637acb773", "revert_the_revert_of_fssrgen",
    1064);
   ("https://github.com/dsyme/visualfsharp.git",
    "69be32f55941a6022751d8d56acdc904d34db86e", "tuple-spike", 1043);
   ("https://github.com/enricosada/visualfsharp.git",
    "a721cd703dee2095dbc691f0e79b94507cdf9990", "use_fslexyacc_package", 981);
   ("https://github.com/wallymathieu/visualfsharp.git",
    "4abfbdbf1bf2440cb10e05b5310e70d0ae2521fb", "fsharp_core_result", 964);
   ("https://github.com/manofstick/visualfsharp.git",
    "c2dcf2342d976f124a82b065092d4b86d8f678ac", "stickman-513-recursive-types",
    961);
   ("https://github.com/forki/visualfsharp.git",
    "fc4f0cb7c02c754f8b88014c85cfdffd2ca59c11", "interpolation", 921);
   ("https://github.com/forki/visualfsharp.git",
    "bd88baba7dbf97b554e541c9e3c0af85b8ceb6de", "nameof2", 908);
   ("https://github.com/buybackoff/visualfsharp.git",
    "b4a6cc1d320fe2ffd3ac08ad7f931e7f4d5893c1", "foreach_optimization", 886);
   ("https://github.com/7sharp9/visualfsharp.git",
    "d300c96015f6b1774aa8a25336e3b837eb9386a8", "intrinsic-extensions", 882);
   ("https://github.com/enricosada/visualfsharp.git",
    "54f5ffa34c24b15ee445440f701541971ba3f56c", "fsharpqa_nunit", 872);
   ("https://github.com/mpetruska/visualfsharp.git",
    "c77ef28228c61164bd7c76db7d2f1d1bc8831639", "fix-for-629", 848) *) ]

(*
[ for c in commits ->  c.Sha ]
[ for p in pulls ->  p.Url, p.PatchUrl ]
[ for p in pulls ->  p.Url, p.StatusesUrl ]
[ for p in pulls ->  p.Url, p.Base.Repo.CloneUrl ]
[ for p in pulls ->  p.Url, p.Head.Repo.CloneUrl, p.Head.Ref ]
*)

let pr1 = pulls.[0]

let time f = 
   let start = DateTime.UtcNow
   let res = f()
   let finish = DateTime.UtcNow
   res, finish - start


let exec cmd args dir = 
    printfn "%s> %s %s" dir cmd args
    let result = Shell.Exec(cmd,args,dir) 
    if result <> 0 then failwith (sprintf "FAILED: %s> %s %s" dir cmd args)

let build(cloneUrl,sha:string,ref,prNumber) =
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
    let logHeader = sprintf "url ref  sha build %s" timesHeaderText
    let logLine = sprintf "%s %s %s %0.2f %s" cloneUrl ref sha buildTime.TotalSeconds timesText
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
