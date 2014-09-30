// simple elevation script - runs the specified command line as admin
// usage: fsi --exec elevate.fsx <program> [args]

open System
open System.Diagnostics

let program = fsi.CommandLineArgs.[1]
let args = fsi.CommandLineArgs |> Seq.skip 2 |> String.concat " "
let startInfo = ProcessStartInfo(FileName = program, Arguments = args, Verb = "runas")
let proc = new Process(StartInfo = startInfo)

printfn "Elevating: %s %s" program args
try
    proc.Start() |> ignore
    if proc.WaitForExit(30*60*1000) then
        Environment.Exit(proc.ExitCode)
with e -> eprintfn "%s" (e.ToString())
Environment.Exit(1)
