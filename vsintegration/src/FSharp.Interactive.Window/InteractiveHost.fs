// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Interactive

open System
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
        /// Where the session starts, and so which `global.json` names its SDK: the solution folder
        /// while a solution is open.
        InitialWorkingDirectory: string

        /// The user's own arguments, from Tools, Options.
        UserArguments: string

        DebugMode: bool
        LanguageVersionPreview: bool
        UICultureLcid: int
    }

/// Where the F# Interactive behind a session came from.
[<RequireQualifiedAccess>]
type internal FsiOrigin =
    /// Named by `FSHARP_INTERACTIVE_PATH`.
    | Override
    /// `dotnet fsi`, the SDK the working directory's `global.json` selects.
    | Sdk
    /// The .NET build of fsi installed with Visual Studio, for SDKs that predate its JSON-RPC server.
    | VisualStudio

    member this.Description =
        match this with
        | Override -> "FSHARP_INTERACTIVE_PATH"
        | Sdk -> ".NET SDK"
        | VisualStudio -> "Visual Studio"

/// One way to start F# Interactive.
type internal FsiCandidate =
    {
        Origin: FsiOrigin
        Executable: string
        LeadingArguments: string list
    }

module internal FsiLocator =

    let private hostExecutable =
        if Environment.OSVersion.Platform = PlatformID.Win32NT then
            "dotnet.exe"
        else
            "dotnet"

    /// The `dotnet` a shell would run: the one Visual Studio was told about, else the first on
    /// `PATH`, else the machine-wide install.
    let findDotnetHost () =
        match Environment.GetEnvironmentVariable "DOTNET_HOST_PATH" with
        | path when not (String.IsNullOrEmpty path) && File.Exists path -> path
        | _ ->

        let onPath =
            match Environment.GetEnvironmentVariable "PATH" with
            | null -> None
            | searchPath ->
                searchPath.Split([| Path.PathSeparator |], StringSplitOptions.RemoveEmptyEntries)
                |> Array.choose (fun directory ->
                    // An entry with characters a path cannot hold is somebody else's problem.
                    try
                        Some(Path.Combine(directory.Trim(' ', '"'), hostExecutable))
                    with _ ->
                        None)
                |> Array.tryFind File.Exists

        match onPath with
        | Some host -> host
        | None ->

        let programFiles =
            match Environment.GetEnvironmentVariable "ProgramW6432" with
            | path when not (String.IsNullOrEmpty path) -> path
            | _ -> Environment.GetFolderPath Environment.SpecialFolder.ProgramFiles

        Path.Combine(programFiles, "dotnet", hostExecutable)

    /// Names the one F# Interactive to run, instead of the SDK's or the one installed with Visual
    /// Studio: a build from a repository, for developing either end of the protocol.
    [<Literal>]
    let OverrideVariable = "FSHARP_INTERACTIVE_PATH"

    /// Where the Microsoft.FSharp.Compiler setup package puts the .NET build of fsi, relative to this
    /// assembly's folder.
    let private bundledRelativePath = Path.Combine("Tools", "Interactive", "fsi.dll")

    /// A build of fsi from a repository runs on the .NET that repository provisions, which is often
    /// newer than any machine-wide install, so look for that host beside it before falling back.
    let private hostFor (fsiPath: string) =
        let executable = hostExecutable

        let rec search (directory: DirectoryInfo | null) =
            match directory with
            | null -> findDotnetHost ()
            | directory ->
                let candidate = Path.Combine(directory.FullName, ".dotnet", executable)

                if File.Exists candidate then
                    candidate
                else
                    search directory.Parent

        search (DirectoryInfo(Path.GetDirectoryName fsiPath))

    let private tryOverride () =
        match Environment.GetEnvironmentVariable OverrideVariable with
        | path when not (String.IsNullOrWhiteSpace path) && File.Exists path ->
            if Path.GetExtension(path).Equals(".dll", StringComparison.OrdinalIgnoreCase) then
                let host = hostFor path

                if File.Exists host then
                    ValueSome(
                        Result.Ok
                            {
                                Origin = FsiOrigin.Override
                                Executable = host
                                LeadingArguments = [ "exec"; path ]
                            }
                    )
                else
                    ValueSome(Result.Error(VFSIstrings.SR.couldNotFindFsiExe host))
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

    /// What to try, in order. Apart from the override this is `dotnet fsi` first: the host resolves the
    /// SDK from the `global.json` nearest the directory the session starts in, so a session started in
    /// the solution folder runs the same compiler bits as `dotnet build` there. An SDK too old for the
    /// JSON-RPC server rejects the option and exits, and the fsi installed with Visual Studio is next.
    let candidates () =
        match tryOverride () with
        | ValueSome(Result.Ok candidate) -> Result.Ok [ candidate ]
        | ValueSome(Result.Error message) -> Result.Error message
        | ValueNone ->

        let host = findDotnetHost ()

        if File.Exists host then
            Result.Ok
                [
                    {
                        Origin = FsiOrigin.Sdk
                        Executable = host
                        LeadingArguments = [ "fsi" ]
                    }
                    yield! tryBundled host |> ValueOption.toList
                ]
        else
            Result.Error(VFSIstrings.SR.couldNotFindFsiExe host)

/// One live F# Interactive process together with the control channel to it.
[<Sealed>]
type internal RemoteSession(origin: FsiOrigin, session: Process, pipe: Stream, rpc: JsonRpc, initialization: InitializeResult) =

    member _.Origin = origin
    member _.Process = session
    member _.Rpc = rpc
    member _.Initialization = initialization

    /// The process evaluating code, which under `dotnet fsi` is not the one that was launched.
    member _.EvaluatingProcessId = initialization.processId

    member _.IsAlive =
        try
            not session.HasExited
        with _ ->
            false

    member _.Dispose() =
        // Closing the channel is how the session learns its host is gone; killing covers one that
        // is wedged and no longer reading.
        try
            rpc.Dispose()
        with _ ->
            ()

        try
            pipe.Dispose()
        with _ ->
            ()

        try
            if not session.HasExited then
                session.Kill()
        with _ ->
            ()

        try
            session.Dispose()
        with _ ->
            ()

/// Owns the F# Interactive process behind the window.
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

    // The F# Interactive that last started in each working directory, tried first next time, so a
    // reset does not pay again for an SDK that turned out to predate the JSON-RPC server. Paths on
    // Windows are case-insensitive.
    let workedIn =
        System.Collections.Generic.Dictionary<string, FsiOrigin>(StringComparer.OrdinalIgnoreCase)

    // Read as characters rather than lines: a script prompting with `printf "name? "` writes no
    // newline, and waiting for one would hide the prompt.
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

    let quoteIfNeeded (argument: string) =
        // .NET Framework has no Contains overload taking a comparison, hence IndexOf.
        if
            argument.IndexOf(" ", StringComparison.Ordinal) >= 0
            && not (argument.StartsWith("\"", StringComparison.Ordinal))
        then
            $"\"{argument}\""
        else
            argument

    let createStartInfo (options: InteractiveHostOptions) (candidate: FsiCandidate) (pipeName: string) =
        let arguments = ResizeArray<string>()
        let addSwitch (switch: string) = arguments.Add(quoteIfNeeded switch)

        for argument in candidate.LeadingArguments do
            addSwitch argument

        addSwitch "--nologo"
        addSwitch $"--fsi-server-output-codepage:{Encoding.UTF8.CodePage}"
        addSwitch $"--fsi-server-input-codepage:{Encoding.UTF8.CodePage}"
        addSwitch $"--fsi-server-lcid:{options.UICultureLcid}"

        // A command-line fragment holding any number of switches, so it goes on unquoted and before
        // the switches the window insists on for debugging.
        if not (String.IsNullOrWhiteSpace options.UserArguments) then
            arguments.Add(options.UserArguments.Trim())

        if options.DebugMode then
            addSwitch "--optimize-"
            addSwitch "--debug+"

        if options.LanguageVersionPreview then
            addSwitch "--langversion:preview"

        // Last, because for each of these the last occurrence wins: the user's own arguments must not
        // be able to move the pipe or name another process as the owner, which would leave the session
        // running after this one closes. The owner is named on the command line, so that a session
        // whose host dies before the handshake exits instead of waiting on the pipe forever.
        addSwitch $"{CommandLine.ServerOption}{pipeName}"
        addSwitch $"--fsi-server-client-pid:{clientProcessId}"

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

    /// One attempt to start a session with one F# Interactive. What the process prints is held back
    /// until the handshake succeeds: an SDK too old for the server prints an error the user need not
    /// see when the next candidate starts fine. `Error(exitedEarly, message, release)` hands the
    /// held output back to the caller, who shows it only when nothing else is left to try.
    let startAttemptAsync (options: InteractiveHostOptions) (candidate: FsiCandidate) (cancellationToken: CancellationToken) =
        task {
            let sessionId = Guid.NewGuid().ToString "N"
            let pipeName = $"FSharpInteractive.{sessionId}"
            let startInfo = createStartInfo options candidate pipeName

            let outputLock = obj ()
            let held = ResizeArray<struct (bool * string)>()
            let mutable holding = true

            let report isError (text: string) =
                lock outputLock (fun () ->
                    if holding then
                        held.Add(struct (isError, text))
                        ValueNone
                    else
                        ValueSome text)
                |> ValueOption.iter (if isError then errorOutputReceived.Trigger else outputReceived.Trigger)

            let release () =
                let pending =
                    lock outputLock (fun () ->
                        holding <- false
                        let pending = held.ToArray()
                        held.Clear()
                        pending)

                for struct (isError, text) in pending do
                    if isError then
                        errorOutputReceived.Trigger text
                    else
                        outputReceived.Trigger text

            let heldText () =
                lock outputLock (fun () -> String.Join("", held |> Seq.map (fun struct (_, text) -> text)))

            let session = new Process(StartInfo = startInfo, EnableRaisingEvents = true)

            // Without this a session that dies before the handshake leaves the connect below
            // waiting forever. Attached before the start, so an exit that comes first is not missed.
            use exitedDuringConnect = new CancellationTokenSource()

            session.Exited.Add(fun _ ->
                try
                    exitedDuringConnect.Cancel()
                with _ ->
                    ())

            let started =
                try
                    session.Start()
                with _ ->
                    false

            if not started then
                return Result.Error(false, VFSIstrings.SR.couldNotFindFsiExe candidate.Executable, ignore)
            else

            pump session.StandardOutput (report false)
            pump session.StandardError (report true)

            use connectCancellation =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, exitedDuringConnect.Token)

            // The session admits only its own user to the pipe. The .NET Framework client cannot ask
            // for the same check of the server's identity (PipeOptions.CurrentUserOnly is .NET Core
            // 2.1+), so the unguessable pipe name is what stands between the window and a squatter.
            let pipe =
                new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous)

            try
                do! pipe.ConnectAsync connectCancellation.Token

                let rpc = new JsonRpc(new HeaderDelimitedMessageHandler(pipe, new JsonMessageFormatter()))
                rpc.StartListening()

                let! handshake =
                    rpc.InvokeWithCancellationAsync<InitializeResult>(Methods.Initialize, cancellationToken = cancellationToken)

                let remote = RemoteSession(candidate.Origin, session, pipe, rpc, handshake)

                session.Exited.Add(fun _ ->
                    let wasCurrent =
                        lock stateLock (fun () ->
                            match current with
                            | ValueSome running when obj.ReferenceEquals(running, remote) ->
                                current <- ValueNone
                                true
                            | _ -> false)

                    if wasCurrent then
                        processExited.Trigger(
                            try
                                session.ExitCode
                            with _ ->
                                0
                        ))

                release ()
                return Result.Ok remote
            with e ->
                pipe.Dispose()

                try
                    if not session.HasExited then
                        session.Kill()
                with _ ->
                    ()

                // A session that exits before the handshake usually rejected the command line: an
                // fsi too old to know the protocol option, or a runtime that is not installed.
                let exitedEarly = session.HasExited

                // The pumps may still be draining what the process wrote just before it exited.
                if exitedEarly then
                    do! Task.Delay 200

                let detail =
                    if not exitedEarly then
                        e.Message
                    elif heldText().IndexOf("install or update .NET", StringComparison.Ordinal) >= 0 then
                        $"The F# Interactive from {candidate.Origin.Description} needs a .NET runtime that is not installed; see the message above."
                    else
                        let commandLine = String.Join(" ", candidate.Executable :: candidate.LeadingArguments)

                        $"The F# Interactive from {candidate.Origin.Description} ({commandLine}) exited with code {session.ExitCode} before the session was established."

                return Result.Error(exitedEarly, detail, release)
        }

    /// Try each F# Interactive in turn, the one that last worked here first, and keep the first that
    /// completes the handshake. Only a process that exits before the handshake moves on to the next:
    /// any other failure is a problem with this session, not with the choice of fsi.
    let startAsync (options: InteractiveHostOptions) (cancellationToken: CancellationToken) =
        task {
            match FsiLocator.candidates () with
            | Result.Error message -> return Result.Error message
            | Result.Ok candidates ->

            let key = options.InitialWorkingDirectory

            let ordered =
                match lock workedIn (fun () -> workedIn.TryGetValue key) with
                | true, origin ->
                    [
                        yield! candidates |> List.filter (fun c -> c.Origin = origin)
                        yield! candidates |> List.filter (fun c -> c.Origin <> origin)
                    ]
                | _ -> candidates

            // A loop rather than a recursive task: `let rec` inside resumable code cannot be compiled
            // statically (FS3511).
            let failures = ResizeArray<string>()
            let mutable remaining = ordered
            let mutable outcome = ValueNone

            while outcome.IsNone do
                match remaining with
                | [] -> outcome <- ValueSome(Result.Error(String.Join(Environment.NewLine, failures)))
                | candidate :: rest ->
                    remaining <- rest

                    match! startAttemptAsync options candidate cancellationToken with
                    | Result.Ok remote ->
                        lock workedIn (fun () -> workedIn[key] <- candidate.Origin)
                        sessionStarted.Trigger remote
                        outcome <- ValueSome(Result.Ok remote)
                    | Result.Error(exitedEarly, detail, release) ->
                        lock workedIn (fun () -> workedIn.Remove key |> ignore)
                        failures.Add detail

                        if not exitedEarly || rest.IsEmpty then
                            release ()
                            outcome <- ValueSome(Result.Error(String.Join(Environment.NewLine, failures)))

            return outcome.Value
        }

    member _.OutputReceived = outputReceived.Publish

    member _.ErrorOutputReceived = errorOutputReceived.Publish

    /// Raised when the session goes away without being asked to.
    member _.ProcessExited = processExited.Publish

    /// Raised with the handshake of every session that comes up, so the window can say what it is
    /// talking to.
    member _.SessionStarted = sessionStarted.Publish

    member _.IsRunning =
        lock stateLock (fun () -> current |> ValueOption.exists (fun session -> session.IsAlive))

    member _.EvaluatingProcessId =
        lock stateLock (fun () -> current |> ValueOption.map (fun session -> session.EvaluatingProcessId))

    member _.Initialization =
        lock stateLock (fun () -> current |> ValueOption.map (fun session -> session.Initialization))

    member private _.TryCurrent() =
        lock stateLock (fun () -> current |> ValueOption.filter (fun session -> session.IsAlive))

    /// Starting is serialised: two callers arriving together would each launch an fsi, and one of
    /// the two would be killed moments later having done nothing but start up.
    member this.EnsureStartedAsync(options, ?cancellationToken) : Task<Result<RemoteSession, string>> =
        let cancellationToken = defaultArg cancellationToken CancellationToken.None

        task {
            match this.TryCurrent() with
            | ValueSome running -> return Result.Ok running
            | ValueNone ->
                do! startGate.WaitAsync cancellationToken

                try
                    match this.TryCurrent() with
                    | ValueSome running -> return Result.Ok running
                    | ValueNone ->
                        match! startAsync options cancellationToken with
                        | Result.Error message -> return Result.Error message
                        | Result.Ok started ->
                            let previous =
                                lock stateLock (fun () ->
                                    if disposed then
                                        ValueNone
                                    else
                                        let previous = current
                                        current <- ValueSome started
                                        ValueSome previous)

                            match previous with
                            | ValueNone ->
                                started.Dispose()
                                return Result.Error "The interactive window was closed while the session was starting."
                            | ValueSome previous ->
                                previous |> ValueOption.iter (fun session -> session.Dispose())
                                return Result.Ok started
                finally
                    startGate.Release() |> ignore
        }

    member this.ResetAsync(options, ?cancellationToken) =
        let previous =
            lock stateLock (fun () ->
                let previous = current
                current <- ValueNone
                previous)

        previous |> ValueOption.iter (fun session -> session.Dispose())

        this.EnsureStartedAsync(options, ?cancellationToken = cancellationToken)

    member private this.InvokeAsync(method: string, parameters: obj, cancellationToken) =
        task {
            match this.TryCurrent() with
            | ValueNone -> return Result.Error "No F# Interactive session is running."
            | ValueSome session ->
                try
                    let! result =
                        session.Rpc.InvokeWithParameterObjectAsync<ExecutionResult>(method, parameters, cancellationToken)

                    return Result.Ok result
                with e ->
                    return Result.Error e.Message
        }

    /// `sourcePath` and `startLine` make the session report diagnostics against the user's own file
    /// when the text came from an editor selection.
    member this.ExecuteAsync(code: string, ?sourcePath: string, ?startLine: int, ?cancellationToken) =
        let request =
            {
                code = code
                sourcePath = Option.toObj sourcePath
                startLine =
                    match startLine with
                    | Some line -> Nullable line
                    | None -> Nullable()
            }

        this.InvokeAsync(Methods.Execute, request, defaultArg cancellationToken CancellationToken.None)

    member this.ExecuteFileAsync(path: string, ?cancellationToken) =
        this.InvokeAsync(Methods.ExecuteFile, { path = path }, defaultArg cancellationToken CancellationToken.None)

    member this.SetPathsAsync(includePaths: string[], workingDirectory: string, ?cancellationToken) =
        this.InvokeAsync(
            Methods.SetPaths,
            {
                includePaths = includePaths
                workingDirectory = workingDirectory
            },
            defaultArg cancellationToken CancellationToken.None
        )

    member this.InterruptAsync() =
        task {
            match this.TryCurrent() with
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
            let previous =
                lock stateLock (fun () ->
                    disposed <- true
                    let previous = current
                    current <- ValueNone
                    previous)

            previous |> ValueOption.iter (fun session -> session.Dispose())
            startGate.Dispose()
