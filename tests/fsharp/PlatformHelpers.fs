module PlatformHelpers

open System.IO
open System.Diagnostics

let log format = printfn format


type FilePath = string

type CmdResult = 
    | Success
    | ErrorLevel of string * int

type CmdArguments = 
    { RedirectOutput : (string -> unit) option
      RedirectError : (string -> unit) option
      RedirectInput : (StreamWriter -> unit) option }

module Process =

    let processExePath baseDir exe =
        if Path.IsPathRooted(exe) then exe
        else 
            match Path.GetDirectoryName(exe) with
            | "" -> exe
            | _ -> Path.Combine(baseDir,exe) |> Path.GetFullPath

    let exec cmdArgs (workDir: FilePath) envs (path: FilePath) arguments =

        let exePath = path |> processExePath workDir
        let processInfo = new ProcessStartInfo(exePath, arguments)
        processInfo.CreateNoWindow <- true
        processInfo.UseShellExecute <- false
        processInfo.WorkingDirectory <- workDir

#if FX_PORTABLE_OR_NETSTANDARD
        ignore envs  // work out what to do about this
#else
        envs
        |> Map.iter (fun k v -> processInfo.EnvironmentVariables.[k] <- v)
#endif

        let p = new Process()
        p.EnableRaisingEvents <- true
        p.StartInfo <- processInfo

        cmdArgs.RedirectOutput|> Option.iter (fun f ->
            processInfo.RedirectStandardOutput <- true
            p.OutputDataReceived.Add (fun ea -> if ea.Data <> null then f ea.Data)
        )

        cmdArgs.RedirectError |> Option.iter (fun f ->
            processInfo.RedirectStandardError <- true
            p.ErrorDataReceived.Add (fun ea -> if ea.Data <> null then f ea.Data)
        )

        cmdArgs.RedirectInput
        |> Option.iter (fun _ -> p.StartInfo.RedirectStandardInput <- true)

        p.Start() |> ignore
    
        cmdArgs.RedirectOutput |> Option.iter (fun _ -> p.BeginOutputReadLine())
        cmdArgs.RedirectError |> Option.iter (fun _ -> p.BeginErrorReadLine())

        cmdArgs.RedirectInput |> Option.iter (fun input -> 
           async {
            let inputWriter = p.StandardInput
            do! inputWriter.FlushAsync () |> Async.AwaitIAsyncResult |> Async.Ignore
            input inputWriter
            do! inputWriter.FlushAsync () |> Async.AwaitIAsyncResult |> Async.Ignore
            inputWriter.Dispose ()
           } 
           |> Async.Start)

        p.WaitForExit() 

        match p.ExitCode with
        | 0 -> Success
        | err -> 
            let msg = sprintf "Error running command '%s' with args '%s' in directory '%s'" exePath arguments workDir 
            ErrorLevel (msg, err)



type OutPipe (writer: TextWriter) =
    member x.Post (msg:string) = lock writer (fun () -> writer.WriteLine(msg))
    interface System.IDisposable with 
       member __.Dispose() = writer.Flush()
     
let redirectTo (writer: TextWriter) = new OutPipe (writer)

let redirectToLog () = redirectTo System.Console.Out

let inline (++) a (b: string) = System.IO.Path.Combine(a,b)
let inline (/) a b = a ++ b  

let splitAtFirst (c: char -> bool) (s: string) =
    let rec helper x (rest: string) =
        match x with
        | [] -> rest, None
        | x :: xs when c(x) -> rest, Some (xs |> Array.ofList |> System.String)
        | x :: xs -> helper xs (rest + x.ToString())
    helper (s.ToCharArray() |> List.ofArray) ""
