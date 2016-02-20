module PlatformHelpers

type ProcessorArchitecture = 
    | X86
    | IA64
    | AMD64
    | Unknown of string
    override this.ToString() = 
        match this with
        | X86 -> "x86"
        | IA64 -> "IA64"
        | AMD64 -> "AMD64"
        | Unknown arc -> arc

open System.IO

type FilePath = string

type CmdResult = 
    | Success
    | ErrorLevel of string * int

type CmdArguments = 
    { RedirectOutput : (string -> unit) option
      RedirectError : (string -> unit) option
      RedirectInput : (StreamWriter -> unit) option }

module Process =

    open System.Diagnostics

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

        envs
        |> Map.iter (fun k v -> processInfo.EnvironmentVariables.[k] <- v)

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
            inputWriter.Close ()
           } 
           |> Async.Start)

        p.WaitForExit() 

        match p.ExitCode with
        | 0 -> Success
        | err -> 
            let msg = sprintf "Error running command '%s' with args '%s' in directory '%s'" exePath arguments workDir 
            ErrorLevel (msg, err)



type Result<'S,'F> =
    | Success of 'S
    | Failure of 'F

type Attempt<'S,'F> = (unit -> Result<'S,'F>)

open System.Diagnostics

[<DebuggerStepThrough>]
let internal succeed x = (fun () -> Success x)

[<DebuggerStepThrough>]
let internal failed err = (fun () -> Failure err)

[<DebuggerStepThrough>]
let runAttempt (a: Attempt<_,_>) = a ()

[<DebuggerStepThrough>]
let delay f = (fun () -> f() |> runAttempt)

[<DebuggerStepThrough>]
let either successTrack failTrack (input : Attempt<_, _>) : Attempt<_, _> =
    match runAttempt input with
    | Success s -> successTrack s
    | Failure f -> failTrack f

[<DebuggerStepThrough>]
let bind successTrack = either successTrack failed

[<DebuggerStepThrough>] 
let fail failTrack result = either succeed failTrack result

[<DebuggerStepThrough>] 
type Attempt =
    static member Run x = runAttempt x

[<DebuggerStepThrough>] 
type AttemptBuilder() =
    member this.Bind(m : Attempt<_, _>, success) = bind success m
    member this.Bind(m : Result<_, _>, success) = bind success (fun () -> m)
    member this.Bind(m : Result<_, _> option, success) = 
        match m with
        | None -> this.Combine(this.Zero(), success)
        | Some x -> this.Bind(x, success)
    member this.Return(x) : Attempt<_, _> = succeed x
    member this.ReturnFrom(x : Attempt<_, _>) = x
    member this.Combine(v, f) : Attempt<_, _> = bind f v
    member this.Yield(x) = Success x
    member this.YieldFrom(x) = x
    member this.Delay(f) : Attempt<_, _> = delay f
    member this.Zero() : Attempt<_, _> = succeed ()
    member this.While(guard, body: Attempt<_, _>) =
        if not (guard()) 
        then this.Zero() 
        else this.Bind(body, fun () -> 
            this.While(guard, body))  

    member this.TryWith(body, handler) =
        try this.ReturnFrom(body())
        with e -> handler e

    member this.TryFinally(body, compensation) =
        try this.ReturnFrom(body())
        finally compensation() 

    member this.Using(disposable:#System.IDisposable, body) =
        let body' = fun () -> body disposable
        this.TryFinally(body', fun () -> 
            match disposable with 
                | null -> () 
                | disp -> disp.Dispose())

    member this.For(sequence:seq<'a>, body: 'a -> Attempt<_,_>) =
        this.Using(sequence.GetEnumerator(),fun enum -> 
            this.While(enum.MoveNext, 
                this.Delay(fun () -> body enum.Current)))

let attempt = new AttemptBuilder()

let log format = Printf.ksprintf (printfn "%s") format


type OutPipe (writer: TextWriter) =
    member x.Post (msg:string) = lock writer (fun () -> writer.WriteLine(msg))
    interface System.IDisposable with 
       member __.Dispose() = writer.Flush()
     
let redirectTo (writer: TextWriter) = new OutPipe (writer)

let redirectToLog () = redirectTo System.Console.Out

let inline (++) a (b: string) = System.IO.Path.Combine(a,b)
let inline (/) a b = a ++ b  //TODO deprecated

let splitAtFirst (c: char -> bool) (s: string) =
    let rec helper x (rest: string) =
        match x with
        | [] -> rest, None
        | x :: xs when c(x) -> rest, Some (xs |> Array.ofList |> System.String)
        | x :: xs -> helper xs (rest + x.ToString())
    helper (s.ToCharArray() |> List.ofArray) ""
