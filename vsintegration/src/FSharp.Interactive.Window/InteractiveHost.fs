// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Interactive

open System
open System.Collections.Generic
open System.Diagnostics
open System.IO
open System.IO.Pipes
open System.Text
open System.Threading
open System.Threading.Tasks

open StreamJsonRpc

open FSharp.Compiler.Interactive.Protocol

type InteractiveHostOptions =
    {
        /// `dotnet fsi` takes its SDK from the `global.json` nearest this folder.
        InitialWorkingDirectory: string
        UserArguments: string
        DebugMode: bool
        LanguageVersionPreview: bool
        UICultureLcid: int
    }

[<RequireQualifiedAccess>]
type internal FsiOrigin =
    | Override
    | Sdk
    | VisualStudio

    static member OverrideVariable = "FSHARP_INTERACTIVE_PATH"

    member this.Description =
        match this with
        | Override -> FsiOrigin.OverrideVariable
        | Sdk -> ".NET SDK"
        | VisualStudio -> "Visual Studio"

type internal FsiCandidate =
    {
        Origin: FsiOrigin
        Executable: string
        LeadingArguments: string list
    }

    member this.DescribeEarlyExit(output: string, exitCode: int) =
        if output.IndexOf("install or update .NET", StringComparison.Ordinal) >= 0 then
            $"The F# Interactive from {this.Origin.Description} needs a .NET runtime that is not installed; see the message above."
        else
            let commandLine = String.Join(" ", this.Executable :: this.LeadingArguments)

            $"The F# Interactive from {this.Origin.Description} ({commandLine}) exited with code {exitCode} before the session was established."

module internal FsiLocator =

    let private hostExecutable = "dotnet.exe"

    let private hostsOnSearchPath () =
        match Environment.GetEnvironmentVariable "PATH" with
        | null -> Seq.empty
        | searchPath ->
            searchPath.Split([| Path.PathSeparator |], StringSplitOptions.RemoveEmptyEntries)
            |> Seq.choose (fun directory ->
                try
                    Some(Path.Combine(directory.Trim(' ', '"'), hostExecutable))
                with :? ArgumentException ->
                    None)

    let private machineWideHost () =
        let programFiles =
            match Environment.GetEnvironmentVariable "ProgramW6432" with
            | path when not (String.IsNullOrEmpty path) -> path
            | _ -> Environment.GetFolderPath Environment.SpecialFolder.ProgramFiles

        Path.Combine(programFiles, "dotnet", hostExecutable)

    let private findDotnetHost () =
        seq {
            Environment.GetEnvironmentVariable "DOTNET_HOST_PATH"
            yield! hostsOnSearchPath ()
        }
        |> Seq.tryFind File.Exists
        |> Option.defaultWith machineWideHost

    /// Laid out by Microsoft.FSharp.Compiler.MSBuild.csproj and VisualFSharpDebug.csproj.
    let private bundledRelativePath = Path.Combine("Tools", "Interactive", "fsi.dll")

    let private repositoryHostOrDefault (fsiPath: string) =
        let rec search (directory: DirectoryInfo | null) =
            match directory with
            | null -> findDotnetHost ()
            | directory ->
                let candidate = Path.Combine(directory.FullName, ".dotnet", hostExecutable)

                if File.Exists candidate then
                    candidate
                else
                    search directory.Parent

        search (DirectoryInfo(Path.GetDirectoryName fsiPath))

    let private candidate origin (executable: string) leadingArguments =
        {
            Origin = origin
            Executable = executable
            LeadingArguments = leadingArguments
        }

    let private onHost origin (host: string) leadingArguments =
        if File.Exists host then
            Result.Ok(candidate origin host leadingArguments)
        else
            Result.Error(VFSIstrings.SR.couldNotFindFsiExe host)

    let private tryOverride () =
        match Environment.GetEnvironmentVariable FsiOrigin.OverrideVariable with
        | path when not (String.IsNullOrWhiteSpace path) && File.Exists path ->
            if Path.GetExtension(path).Equals(".dll", StringComparison.OrdinalIgnoreCase) then
                ValueSome(onHost FsiOrigin.Override (repositoryHostOrDefault path) [ "exec"; path ])
            else
                ValueSome(Result.Ok(candidate FsiOrigin.Override path []))
        | _ -> ValueNone

    let private tryBundled (host: string) =
        match Path.GetDirectoryName(typeof<FsiCandidate>.Assembly.Location) with
        | null -> ValueNone
        | directory ->
            let fsi = Path.Combine(directory, bundledRelativePath)

            if File.Exists fsi then
                ValueSome(candidate FsiOrigin.VisualStudio host [ "exec"; fsi ])
            else
                ValueNone

    let candidates () =
        match tryOverride () with
        | ValueSome overridden -> overridden |> Result.map List.singleton
        | ValueNone ->
            let host = findDotnetHost ()

            onHost FsiOrigin.Sdk host [ "fsi" ]
            |> Result.map (fun sdk -> [ sdk; yield! tryBundled host |> ValueOption.toList ])

module private SessionProcess =

    let quietly (action: unit -> unit) =
        try
            action ()
        with _ ->
            ()

    let kill (session: Process) =
        quietly (fun () ->
            if not session.HasExited then
                session.Kill())

    let pumpDrainMilliseconds = 200

    let pumpWithoutWaitingForNewline (reader: StreamReader) (report: string -> unit) =
        let read () =
            let buffer = Array.zeroCreate<char> 1024
            let mutable count = reader.Read(buffer, 0, buffer.Length)

            while count > 0 do
                report (String(buffer, 0, count))
                count <- reader.Read(buffer, 0, buffer.Length)

        Thread((fun () -> quietly read), IsBackground = true).Start()

    let private quoteIfNeeded (argument: string) =
        if
            argument.IndexOf ' ' >= 0
            && not (argument.StartsWith("\"", StringComparison.Ordinal))
        then
            $"\"{argument}\""
        else
            argument

    let startInfo (options: InteractiveHostOptions) (candidate: FsiCandidate) (pipeName: string) (clientProcessId: int) =
        let arguments =
            [
                yield! candidate.LeadingArguments |> List.map quoteIfNeeded
                "--nologo"
                $"--fsi-server-output-codepage:{Encoding.UTF8.CodePage}"
                $"--fsi-server-input-codepage:{Encoding.UTF8.CodePage}"
                $"--fsi-server-lcid:{options.UICultureLcid}"

                if not (String.IsNullOrWhiteSpace options.UserArguments) then
                    options.UserArguments.Trim()

                if options.DebugMode then
                    "--optimize-"
                    "--debug+"

                if options.LanguageVersionPreview then
                    "--langversion:preview"

                // After the user's arguments: fsi takes the last occurrence of a --fsi-server-* switch.
                $"{CommandLine.ServerOption}{pipeName}"
                $"--fsi-server-client-pid:{clientProcessId}"
            ]

        let startInfo =
            ProcessStartInfo(
                FileName = candidate.Executable,
                Arguments = String.Join(" ", arguments),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            )

        if Directory.Exists options.InitialWorkingDirectory then
            startInfo.WorkingDirectory <- options.InitialWorkingDirectory

        startInfo

[<Sealed>]
type internal RemoteSession(origin: FsiOrigin, session: Process, pipe: Stream, rpc: JsonRpc, initialization: InitializeResult) =

    member _.Origin = origin
    member _.Rpc = rpc
    member _.Initialization = initialization

    member _.ExitCode =
        try
            session.ExitCode
        with _ ->
            0

    member _.IsAlive =
        try
            not session.HasExited
        with _ ->
            false

    member _.Dispose() =
        SessionProcess.quietly rpc.Dispose
        SessionProcess.quietly pipe.Dispose
        SessionProcess.kill session
        SessionProcess.quietly session.Dispose

[<Sealed>]
type private OutputHeldUntilHandshake(forward: bool -> string -> unit) =

    let held = ResizeArray<struct (bool * string)>()
    let mutable holding = true

    member _.Report (isError: bool) (text: string) =
        let passes =
            lock held (fun () ->
                if holding then
                    held.Add(struct (isError, text))

                not holding)

        if passes then
            forward isError text

    member _.Release() =
        let pending =
            lock held (fun () ->
                holding <- false
                let pending = held.ToArray()
                held.Clear()
                pending)

        for struct (isError, text) in pending do
            forward isError text

    member _.Text =
        lock held (fun () -> String.Join("", held |> Seq.map (fun struct (_, text) -> text)))

[<Sealed>]
type internal InteractiveHostClient(clientProcessId: int) =

    let stateLock = obj ()
    let startGate = new SemaphoreSlim(1, 1)
    let mutable current: RemoteSession voption = ValueNone
    let mutable disposed = false

    let outputReceived = Event<string>()
    let errorOutputReceived = Event<string>()
    let sessionExitedUnexpectedly = Event<int>()
    let sessionStarted = Event<RemoteSession>()

    let lastWorkingOrigin = Dictionary<string, FsiOrigin>(StringComparer.OrdinalIgnoreCase)

    let forward isError (text: string) =
        if isError then
            errorOutputReceived.Trigger text
        else
            outputReceived.Trigger text

    let tryCurrent () =
        lock stateLock (fun () -> current |> ValueOption.filter _.IsAlive)

    let takeCurrent () =
        lock stateLock (fun () ->
            let previous = current
            current <- ValueNone
            previous)

    let reportExit (remote: RemoteSession) =
        let wasCurrent =
            lock stateLock (fun () ->
                match current with
                | ValueSome running when obj.ReferenceEquals(running, remote) ->
                    current <- ValueNone
                    true
                | _ -> false)

        if wasCurrent then
            sessionExitedUnexpectedly.Trigger remote.ExitCode

    let startAttemptAsync (options: InteractiveHostOptions) (candidate: FsiCandidate) (cancellationToken: CancellationToken) =
        task {
            let unguessablePipeName = $"FSharpInteractive.{Guid.NewGuid():N}"
            let output = OutputHeldUntilHandshake forward

            let session =
                new Process(StartInfo = SessionProcess.startInfo options candidate unguessablePipeName clientProcessId, EnableRaisingEvents = true)

            // Before Start: Exited is not raised for handlers added after the exit.
            use exitedDuringConnect = new CancellationTokenSource()
            session.Exited.Add(fun _ -> SessionProcess.quietly exitedDuringConnect.Cancel)

            let started =
                try
                    session.Start()
                with _ ->
                    false

            if not started then
                return Result.Error(false, VFSIstrings.SR.couldNotFindFsiExe candidate.Executable, ignore)
            else
                SessionProcess.pumpWithoutWaitingForNewline session.StandardOutput (output.Report false)
                SessionProcess.pumpWithoutWaitingForNewline session.StandardError (output.Report true)

                use connectCancellation =
                    CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, exitedDuringConnect.Token)

                // No CurrentUserOnly on net472: the unguessable pipe name is the only guard.
                let pipe =
                    new NamedPipeClientStream(".", unguessablePipeName, PipeDirection.InOut, PipeOptions.Asynchronous)

                try
                    do! pipe.ConnectAsync connectCancellation.Token

                    let rpc =
                        new JsonRpc(new HeaderDelimitedMessageHandler(pipe, new JsonMessageFormatter()))

                    rpc.StartListening()

                    let! handshake =
                        rpc.InvokeWithCancellationAsync<InitializeResult>(Methods.Initialize, cancellationToken = cancellationToken)

                    let remote = RemoteSession(candidate.Origin, session, pipe, rpc, handshake)
                    session.Exited.Add(fun _ -> reportExit remote)
                    output.Release()
                    return Result.Ok remote
                with e ->
                    pipe.Dispose()
                    SessionProcess.kill session

                    let exitedEarly = session.HasExited

                    if exitedEarly then
                        do! Task.Delay SessionProcess.pumpDrainMilliseconds

                    let detail =
                        if exitedEarly then
                            candidate.DescribeEarlyExit(output.Text, session.ExitCode)
                        else
                            e.Message

                    return Result.Error(exitedEarly, detail, output.Release)
        }

    let startAsync (options: InteractiveHostOptions) (cancellationToken: CancellationToken) =
        task {
            match FsiLocator.candidates () with
            | Result.Error message -> return Result.Error message
            | Result.Ok candidates ->
                let key = options.InitialWorkingDirectory

                let mutable remaining =
                    match lock lastWorkingOrigin (fun () -> lastWorkingOrigin.TryGetValue key) with
                    | true, origin -> candidates |> List.sortBy (fun candidate -> candidate.Origin <> origin)
                    | _ -> candidates

                // A loop because `let rec` inside `task` is FS3511.
                let failures = ResizeArray<string>()
                let mutable started = ValueNone

                while started.IsNone && not remaining.IsEmpty do
                    let candidate = remaining.Head
                    remaining <- remaining.Tail

                    match! startAttemptAsync options candidate cancellationToken with
                    | Result.Ok remote ->
                        lock lastWorkingOrigin (fun () -> lastWorkingOrigin[key] <- candidate.Origin)
                        sessionStarted.Trigger remote
                        started <- ValueSome remote
                    | Result.Error(exitedEarly, detail, release) ->
                        lock lastWorkingOrigin (fun () -> lastWorkingOrigin.Remove key |> ignore)
                        failures.Add detail

                        if not exitedEarly || remaining.IsEmpty then
                            release ()
                            remaining <- []

                return
                    match started with
                    | ValueSome remote -> Result.Ok remote
                    | ValueNone -> Result.Error(String.Join(Environment.NewLine, failures))
        }

    let installUnlessExited (started: RemoteSession) =
        let struct (previous, failure) =
            lock stateLock (fun () ->
                if disposed then
                    struct (ValueNone, ValueSome "The interactive window was closed while the session was starting.")
                else
                    let previous = takeCurrent ()

                    if started.IsAlive then
                        current <- ValueSome started
                        struct (previous, ValueNone)
                    else
                        struct (previous, ValueSome $"F# Interactive exited with code {started.ExitCode} as the session started."))

        previous |> ValueOption.iter _.Dispose()

        match failure with
        | ValueNone -> Result.Ok started
        | ValueSome message ->
            started.Dispose()
            Result.Error message

    member _.OutputReceived = outputReceived.Publish

    member _.ErrorOutputReceived = errorOutputReceived.Publish

    member _.SessionExitedUnexpectedly = sessionExitedUnexpectedly.Publish

    member _.SessionStarted = sessionStarted.Publish

    member _.EnsureStartedAsync(options, ?cancellationToken) : Task<Result<RemoteSession, string>> =
        let cancellationToken = defaultArg cancellationToken CancellationToken.None

        task {
            match tryCurrent () with
            | ValueSome running -> return Result.Ok running
            | ValueNone ->
                do! startGate.WaitAsync cancellationToken

                try
                    match tryCurrent () with
                    | ValueSome running -> return Result.Ok running
                    | ValueNone ->
                        let! outcome = startAsync options cancellationToken
                        return outcome |> Result.bind installUnlessExited
                finally
                    startGate.Release() |> ignore
        }

    member this.ResetAsync(options, ?cancellationToken) =
        takeCurrent () |> ValueOption.iter _.Dispose()
        this.EnsureStartedAsync(options, ?cancellationToken = cancellationToken)

    member _.ExecuteAsync(code: string, origin: struct (string * int) voption, ?cancellationToken) =
        let request =
            {
                code = code
                sourcePath = origin |> ValueOption.map (fun struct (path, _) -> path) |> ValueOption.toObj
                startLine =
                    origin
                    |> ValueOption.map (fun struct (_, line) -> line)
                    |> ValueOption.toNullable
            }

        task {
            match tryCurrent () with
            | ValueNone -> return Result.Error "No F# Interactive session is running."
            | ValueSome session ->
                try
                    let! result =
                        session.Rpc.InvokeWithParameterObjectAsync<ExecutionResult>(
                            Methods.Execute,
                            request,
                            defaultArg cancellationToken CancellationToken.None
                        )

                    return Result.Ok result
                with e ->
                    return Result.Error e.Message
        }

    member _.InterruptAsync() =
        task {
            match tryCurrent () with
            | ValueNone -> return false
            | ValueSome session ->
                try
                    let! result = session.Rpc.InvokeAsync<InterruptResult>(Methods.Interrupt)
                    return result.interrupted
                with _ ->
                    return false
        }

    interface IDisposable with
        member _.Dispose() =
            lock stateLock (fun () ->
                disposed <- true
                takeCurrent ())
            |> ValueOption.iter _.Dispose()

            startGate.Dispose()
