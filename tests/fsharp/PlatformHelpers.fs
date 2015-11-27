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
    | ErrorLevel of int

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

        cmdArgs.RedirectOutput
        |> Option.map (fun f -> (fun (ea: DataReceivedEventArgs) -> ea.Data |> f)) 
        |> Option.iter (fun newOut ->
            processInfo.RedirectStandardOutput <- true
            p.OutputDataReceived.Add newOut
        )

        cmdArgs.RedirectError 
        |> Option.map (fun f -> (fun (ea: DataReceivedEventArgs) -> ea.Data |> f)) 
        |> Option.iter (fun newErr ->
            processInfo.RedirectStandardError <- true
            p.ErrorDataReceived.Add newErr
        )

        cmdArgs.RedirectInput
        |> Option.iter (fun _ -> p.StartInfo.RedirectStandardInput <- true)

        let exitedAsync (proc: Process) =
            let tcs = new System.Threading.Tasks.TaskCompletionSource<int>();
            p.Exited.Add (fun s -> 
                tcs.TrySetResult(proc.ExitCode) |> ignore
                proc.Dispose())
            tcs.Task

        p.Start() |> ignore
    
        cmdArgs.RedirectOutput |> Option.iter (fun _ -> p.BeginOutputReadLine())
        cmdArgs.RedirectError |> Option.iter (fun _ -> p.BeginErrorReadLine())

        cmdArgs.RedirectInput
        |> Option.map (fun input -> async {
            let inputWriter = p.StandardInput
            do! inputWriter.FlushAsync () |> Async.AwaitIAsyncResult |> Async.Ignore
            input inputWriter
            do! inputWriter.FlushAsync () |> Async.AwaitIAsyncResult |> Async.Ignore
            inputWriter.Close ()
            })
        |> Option.iter Async.Start

        let exitCode = p |> exitedAsync |> Async.AwaitTask |> Async.RunSynchronously

        match exitCode with
        | 0 -> Success
        | err -> ErrorLevel err



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

let processor = new AttemptBuilder()


let log format = Printf.ksprintf (printfn "%s") format

type OutPipe (mailbox: MailboxProcessor<_>) =
    member x.Post msg = mailbox.Post(msg)
    interface System.IDisposable with
        member x.Dispose () = 
            async {
                while mailbox.CurrentQueueLength > 0 do
                    let timeout = System.TimeSpan.FromMilliseconds(50.0)
                    do! Async.Sleep (timeout.TotalMilliseconds |> int)
            } |> Async.RunSynchronously

let redirectTo (writer: TextWriter) =
    let mailbox = MailboxProcessor.Start(fun inbox -> 
        let rec loop () = async {
            let! (msg : string) = inbox.Receive ()
            do! writer.WriteLineAsync(msg) |> (Async.AwaitIAsyncResult >> Async.Ignore)
            return! loop () }
        loop ())
    new OutPipe (mailbox)

let redirectToLog () = redirectTo System.Console.Out

let inline (/) a (b: string) = System.IO.Path.Combine(a,b)
