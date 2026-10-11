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
    /// The .NET fsi installed with Visual Studio, for SDKs that predate the JSON-RPC server.
    | VisualStudio

    /// Names the one F# Interactive to run: a repository's build, for developing either end of the protocol.
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

    let private hostExecutable =
        if Environment.OSVersion.Platform = PlatformID.Win32NT then
            "dotnet.exe"
        else
            "dotnet"

    let private hostsOnSearchPath () =
        match Environment.GetEnvironmentVariable "PATH" with
        | null -> Seq.empty
        | searchPath ->
            searchPath.Split([| Path.PathSeparator |], StringSplitOptions.RemoveEmptyEntries)
            |> Seq.choose (fun directory ->
                // Path.Combine rejects an entry holding characters a path cannot.
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

    /// Where the Microsoft.FSharp.Compiler setup package puts the .NET fsi, relative to this assembly.
    let private bundledRelativePath = Path.Combine("Tools", "Interactive", "fsi.dll")

    /// A repository's fsi runs on the .NET that repository provisions, often newer than any
    /// machine-wide install, so that host is looked for beside it first.
    let private hostFor (fsiPath: string) =
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

    let private onHost origin (host: string) leadingArguments =
        if File.Exists host then
            Result.Ok
                {
                    Origin = origin
                    Executable = host
                    LeadingArguments = leadingArguments
                }
        else
            Result.Error(VFSIstrings.SR.couldNotFindFsiExe host)

    let private tryOverride () =
        match Environment.GetEnvironmentVariable FsiOrigin.OverrideVariable with
        | path when not (String.IsNullOrWhiteSpace path) && File.Exists path ->
            if Path.GetExtension(path).Equals(".dll", StringComparison.OrdinalIgnoreCase) then
                ValueSome(onHost FsiOrigin.Override (hostFor path) [ "exec"; path ])
            else
                ValueSome(
                    Result.Ok
                        {
                            Origin = FsiOrigin.Override
                            Executable = path
                            LeadingArguments = []
                        }
                )
        | _ -> ValueNone

    let private tryBundled (host: string) =
        match Path.GetDirectoryName(typeof<FsiCandidate>.Assembly.Location) with
        | null -> ValueNone
        | directory ->
            let fsi = Path.Combine(directory, bundledRelativePath)

            if File.Exists fsi then
                ValueSome
                    {
                        Origin = FsiOrigin.VisualStudio
                        Executable = host
                        LeadingArguments = [ "exec"; fsi ]
                    }
            else
                ValueNone

    /// In the order to try them: an SDK too old for the JSON-RPC server rejects the option and
    /// exits, and the fsi installed with Visual Studio is next.
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

    let exitCode (session: Process) =
        try
            session.ExitCode
        with _ ->
            0

    // Read as characters, not lines: a script prompting with `printf "name? "` writes no newline,
    // and waiting for one would hide the prompt.
    let pump (reader: StreamReader) (report: string -> unit) =
        let thread =
            Thread(
                (fun () ->
                    let buffer = Array.zeroCreate<char> 1024

                    try
                        let rec loop () =
                            let count = reader.Read(buffer, 0, buffer.Length)

                            if count > 0 then
                                report (String(buffer, 0, count))
                                loop ()

                        loop ()
                    with _ ->
                        ()),
                IsBackground = true
            )

        thread.Start()

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

                // A fragment holding any number of switches, hence unquoted.
                if not (String.IsNullOrWhiteSpace options.UserArguments) then
                    options.UserArguments.Trim()

                if options.DebugMode then
                    "--optimize-"
                    "--debug+"

                if options.LanguageVersionPreview then
                    "--langversion:preview"

                // Last, where the last occurrence wins: the user's arguments must not move the pipe or
                // name another owner, which would leave the session running after this host closes.
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
    member _.ExitCode = SessionProcess.exitCode session

    member _.IsAlive =
        try
            not session.HasExited
        with _ ->
            false

    member _.Dispose() =
        // Closing the channel tells the session its host is gone; the kill covers one too wedged to notice.
        SessionProcess.quietly rpc.Dispose
        SessionProcess.quietly pipe.Dispose
        SessionProcess.kill session
        SessionProcess.quietly session.Dispose

/// What a starting session prints, kept back until its handshake: an SDK too old for the server
/// prints an error nobody needs to see once the next candidate has started.
[<Sealed>]
type private HeldOutput(forward: bool -> string -> unit) =

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
    let processExited = Event<int>()
    let sessionStarted = Event<RemoteSession>()

    // The origin that last started in each working directory, tried first next time: a reset does
    // not pay again for an SDK that turned out to predate the JSON-RPC server.
    let workedIn = Dictionary<string, FsiOrigin>(StringComparer.OrdinalIgnoreCase)

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
            processExited.Trigger remote.ExitCode

    let startAttemptAsync (options: InteractiveHostOptions) (candidate: FsiCandidate) (cancellationToken: CancellationToken) =
        task {
            let pipeName = $"FSharpInteractive.{Guid.NewGuid():N}"
            let output = HeldOutput forward

            let session =
                new Process(StartInfo = SessionProcess.startInfo options candidate pipeName clientProcessId, EnableRaisingEvents = true)

            // A session that dies before the handshake would leave the connect below waiting forever.
            // Attached before the start, so that an exit that comes first is not missed.
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
                SessionProcess.pump session.StandardOutput (output.Report false)
                SessionProcess.pump session.StandardError (output.Report true)

                use connectCancellation =
                    CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, exitedDuringConnect.Token)

                // The session admits only its own user to the pipe. A .NET Framework client cannot check
                // the server's identity in turn (PipeOptions.CurrentUserOnly is .NET Core 2.1+), so the
                // unguessable pipe name is what stands between the window and a squatter.
                let pipe =
                    new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous)

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

                    // An exit before the handshake usually means the command line was rejected: an fsi too
                    // old to know the protocol option, or a runtime that is not installed.
                    let exitedEarly = session.HasExited

                    // The pumps may still be draining what the process wrote just before it exited.
                    if exitedEarly then
                        do! Task.Delay 200

                    let detail =
                        if exitedEarly then
                            candidate.DescribeEarlyExit(output.Text, session.ExitCode)
                        else
                            e.Message

                    return Result.Error(exitedEarly, detail, output.Release)
        }

    /// Only a process that exits before the handshake moves on to the next candidate: any other
    /// failure is a problem with this session, not with the choice of fsi.
    let startAsync (options: InteractiveHostOptions) (cancellationToken: CancellationToken) =
        task {
            match FsiLocator.candidates () with
            | Result.Error message -> return Result.Error message
            | Result.Ok candidates ->
                let key = options.InitialWorkingDirectory

                let mutable remaining =
                    match lock workedIn (fun () -> workedIn.TryGetValue key) with
                    | true, origin -> candidates |> List.sortBy (fun candidate -> candidate.Origin <> origin)
                    | _ -> candidates

                // A loop, not a recursive task: `let rec` inside resumable code cannot be compiled
                // statically (FS3511).
                let failures = ResizeArray<string>()
                let mutable started = ValueNone

                while started.IsNone && not remaining.IsEmpty do
                    let candidate = remaining.Head
                    remaining <- remaining.Tail

                    match! startAttemptAsync options candidate cancellationToken with
                    | Result.Ok remote ->
                        lock workedIn (fun () -> workedIn[key] <- candidate.Origin)
                        sessionStarted.Trigger remote
                        started <- ValueSome remote
                    | Result.Error(exitedEarly, detail, release) ->
                        lock workedIn (fun () -> workedIn.Remove key |> ignore)
                        failures.Add detail

                        if not exitedEarly || remaining.IsEmpty then
                            release ()
                            remaining <- []

                return
                    match started with
                    | ValueSome remote -> Result.Ok remote
                    | ValueNone -> Result.Error(String.Join(Environment.NewLine, failures))
        }

    /// The exit handler reports an exit only for the current session, so a session that exited
    /// before this point is caught here instead, under the same lock.
    let install (started: RemoteSession) =
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

    /// Raised when the session goes away without being asked to.
    member _.ProcessExited = processExited.Publish

    member _.SessionStarted = sessionStarted.Publish

    /// Serialised: two callers arriving together would each launch an fsi, one of them only to be
    /// killed moments later.
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
                        return outcome |> Result.bind install
                finally
                    startGate.Release() |> ignore
        }

    member this.ResetAsync(options, ?cancellationToken) =
        takeCurrent () |> ValueOption.iter _.Dispose()
        this.EnsureStartedAsync(options, ?cancellationToken = cancellationToken)

    member _.ExecuteAsync(code: string, origin: struct (string * int) voption, ?cancellationToken) =
        let request =
            match origin with
            | ValueSome(struct (sourcePath, startLine)) ->
                {
                    code = code
                    sourcePath = sourcePath
                    startLine = Nullable startLine
                }
            | ValueNone ->
                {
                    code = code
                    sourcePath = null
                    startLine = Nullable()
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
