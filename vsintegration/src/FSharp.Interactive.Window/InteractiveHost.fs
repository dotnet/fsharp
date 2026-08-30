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

type InteractiveHostPlatform =
    | NetCore
    | NetFramework64
    | NetFramework32
    | NetFrameworkArm64

    member this.Description =
        match this with
        | NetCore -> ".NET"
        | NetFramework64 -> ".NET Framework (64-bit)"
        | NetFramework32 -> ".NET Framework (32-bit)"
        | NetFrameworkArm64 -> ".NET Framework (Arm64)"

    member this.CommandLineName =
        match this with
        | NetCore -> "core"
        | NetFramework64 -> "64"
        | NetFramework32 -> "32"
        | NetFrameworkArm64 -> "arm64"

    static member TryParse(name: string) =
        match name.Trim().ToLowerInvariant() with
        | "core"
        | "net" -> Some NetCore
        | "64"
        | "framework64" -> Some NetFramework64
        | "32"
        | "framework32" -> Some NetFramework32
        | "arm64" -> Some NetFrameworkArm64
        | _ -> None

type InteractiveHostOptions =
    {
        Platform: InteractiveHostPlatform

        /// Directory holding the desktop fsi executables shipped in the extension.
        HostDirectory: string

        InitialWorkingDirectory: string

        /// The user's own arguments, from Tools, Options.
        UserArguments: string

        ShadowCopyReferences: bool
        DebugMode: bool
        LanguageVersionPreview: bool
        UICultureLcid: int
    }

module internal FsiLocator =

    let private desktopExecutableName platform =
        match platform with
        | NetFramework32 -> "fsi.exe"
        | NetFrameworkArm64 -> "fsiArm64.exe"
        | _ -> "fsiAnyCpu.exe"

    let findDotnetHost () =
        match Environment.GetEnvironmentVariable "DOTNET_HOST_PATH" with
        | path when not (String.IsNullOrEmpty path) && File.Exists path -> path
        | _ ->

        let programFiles =
            match Environment.GetEnvironmentVariable "ProgramW6432" with
            | path when not (String.IsNullOrEmpty path) -> path
            | _ -> Environment.GetFolderPath Environment.SpecialFolder.ProgramFiles

        Path.Combine(programFiles, "dotnet", "dotnet.exe")

    /// Names an F# Interactive to run instead of the one the platform would resolve to.
    ///
    /// The protocol needs an fsi that understands `--fsi-server-jsonrpc`. The extension does not
    /// carry one, and the fsi resolved from an installed SDK is only as new as that SDK, so a build
    /// of fsi from this repository has to be named explicitly until the option ships.
    [<Literal>]
    let OverrideVariable = "FSHARP_INTERACTIVE_PATH"

    /// A build of fsi from a repository runs on the .NET that repository provisions, which is often
    /// newer than any machine-wide install, so look for that host beside it before falling back.
    let private hostFor (fsiPath: string) =
        let executable =
            if Environment.OSVersion.Platform = PlatformID.Win32NT then
                "dotnet.exe"
            else
                "dotnet"

        let rec search (directory: DirectoryInfo) =
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
                    Some(Result.Ok(host, [ "exec"; path ]))
                else
                    Some(Result.Error(VFSIstrings.SR.couldNotFindFsiExe host))
            else
                Some(Result.Ok(path, []))
        | _ -> None

    let locate (options: InteractiveHostOptions) =
        match tryOverride () with
        | Some result -> result
        | None ->

        match options.Platform with
        | NetCore ->
            let host = findDotnetHost ()

            if File.Exists host then
                Result.Ok(host, [ "fsi" ])
            else
                Result.Error(VFSIstrings.SR.couldNotFindFsiExe host)

        | platform ->
            let candidate = Path.Combine(options.HostDirectory, desktopExecutableName platform)

            if File.Exists candidate then
                Result.Ok(candidate, [])
            else
                Result.Error(VFSIstrings.SR.couldNotFindFsiExe candidate)

/// One live F# Interactive process together with the control channel to it.
[<Sealed>]
type internal RemoteSession(session: Process, pipe: Stream, rpc: JsonRpc, initialization: InitializeResult) =

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
    let mutable current: RemoteSession option = None
    let mutable disposed = false

    let outputReceived = Event<string>()
    let errorOutputReceived = Event<string>()
    let processExited = Event<int>()

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
        if argument.Contains " " && not (argument.StartsWith "\"") then
            "\"" + argument + "\""
        else
            argument

    let createStartInfo (options: InteractiveHostOptions) (pipeName: string) =
        match FsiLocator.locate options with
        | Result.Error message -> Result.Error message
        | Result.Ok(executable, leadingArguments) ->

        let arguments = ResizeArray<string>()
        let addSwitch (switch: string) = arguments.Add(quoteIfNeeded switch)

        for argument in leadingArguments do
            addSwitch argument

        addSwitch "--nologo"
        addSwitch ("--fsi-server-jsonrpc:" + pipeName)
        addSwitch $"--fsi-server-output-codepage:{Encoding.UTF8.CodePage}"
        addSwitch $"--fsi-server-input-codepage:{Encoding.UTF8.CodePage}"
        addSwitch $"--fsi-server-lcid:{options.UICultureLcid}"

        // A command-line fragment holding any number of switches, so it goes on unquoted and before
        // the switches the window insists on for debugging.
        if not (String.IsNullOrWhiteSpace options.UserArguments) then
            arguments.Add(options.UserArguments.Trim())

        if options.Platform <> NetCore then
            addSwitch (
                if options.ShadowCopyReferences then
                    "--shadowcopyreferences+"
                else
                    "--shadowcopyreferences-"
            )

        if options.DebugMode then
            addSwitch "--optimize-"
            addSwitch "--debug+"

        if options.LanguageVersionPreview then
            addSwitch "--langversion:preview"

        let startInfo =
            ProcessStartInfo(
                FileName = executable,
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

        Result.Ok startInfo

    let startAsync (options: InteractiveHostOptions) (cancellationToken: CancellationToken) =
        task {
            let pipeName = "FSharpInteractive." + Guid.NewGuid().ToString "N"

            match createStartInfo options pipeName with
            | Result.Error message -> return Result.Error message
            | Result.Ok startInfo ->

            let session = new Process(StartInfo = startInfo, EnableRaisingEvents = true)

            if not (session.Start()) then
                return Result.Error(VFSIstrings.SR.couldNotFindFsiExe startInfo.FileName)
            else

            pump session.StandardOutput outputReceived.Trigger
            pump session.StandardError errorOutputReceived.Trigger

            // Without this a session that dies before the handshake leaves the connect below
            // waiting out its whole timeout.
            use exitedDuringConnect = new CancellationTokenSource()

            session.Exited.Add(fun _ ->
                try
                    exitedDuringConnect.Cancel()
                with _ ->
                    ())

            use connectCancellation =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, exitedDuringConnect.Token)

            let pipe =
                new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous)

            try
                do! pipe.ConnectAsync connectCancellation.Token

                let rpc = new JsonRpc(new HeaderDelimitedMessageHandler(pipe, new JsonMessageFormatter()))
                rpc.StartListening()

                let! handshake =
                    rpc.InvokeWithParameterObjectAsync<InitializeResult>(
                        Methods.Initialize,
                        { clientProcessId = clientProcessId },
                        cancellationToken
                    )

                let remote = RemoteSession(session, pipe, rpc, handshake)

                session.Exited.Add(fun _ ->
                    let wasCurrent =
                        lock stateLock (fun () ->
                            match current with
                            | Some running when obj.ReferenceEquals(running, remote) ->
                                current <- None
                                true
                            | _ -> false)

                    if wasCurrent then
                        processExited.Trigger(
                            try
                                session.ExitCode
                            with _ ->
                                0
                        ))

                return Result.Ok remote
            with e ->
                pipe.Dispose()

                try
                    if not session.HasExited then
                        session.Kill()
                with _ ->
                    ()

                // A session that exits before the handshake usually rejected the command line —
                // most often an fsi too old to know the protocol option.
                let detail =
                    if session.HasExited then
                        $"{startInfo.FileName} exited with code {session.ExitCode} before the session was established. "
                        + $"If it does not support '--fsi-server-jsonrpc', set {FsiLocator.OverrideVariable} to an fsi that does."
                    else
                        e.Message

                return Result.Error detail
        }

    member _.OutputReceived = outputReceived.Publish

    member _.ErrorOutputReceived = errorOutputReceived.Publish

    /// Raised when the session goes away without being asked to.
    member _.ProcessExited = processExited.Publish

    member _.IsRunning =
        lock stateLock (fun () -> current |> Option.exists (fun session -> session.IsAlive))

    member _.EvaluatingProcessId =
        lock stateLock (fun () -> current |> Option.map (fun session -> session.EvaluatingProcessId))

    member _.Initialization =
        lock stateLock (fun () -> current |> Option.map (fun session -> session.Initialization))

    member private _.TryCurrent() =
        lock stateLock (fun () -> current |> Option.filter (fun session -> session.IsAlive))

    /// Starting is serialised: two callers arriving together would each launch an fsi, and one of
    /// the two would be killed moments later having done nothing but start up.
    member this.EnsureStartedAsync(options, ?cancellationToken) : Task<Result<RemoteSession, string>> =
        let cancellationToken = defaultArg cancellationToken CancellationToken.None

        task {
            match this.TryCurrent() with
            | Some running -> return Result.Ok running
            | None ->
                do! startGate.WaitAsync cancellationToken

                try
                    match this.TryCurrent() with
                    | Some running -> return Result.Ok running
                    | None ->
                        match! startAsync options cancellationToken with
                        | Result.Error message -> return Result.Error message
                        | Result.Ok started ->
                            let previous =
                                lock stateLock (fun () ->
                                    if disposed then
                                        None
                                    else
                                        let previous = current
                                        current <- Some started
                                        Some previous)

                            match previous with
                            | None ->
                                started.Dispose()
                                return Result.Error "The interactive window was closed while the session was starting."
                            | Some previous ->
                                previous |> Option.iter (fun session -> session.Dispose())
                                return Result.Ok started
                finally
                    startGate.Release() |> ignore
        }

    member this.ResetAsync(options, ?cancellationToken) =
        let previous =
            lock stateLock (fun () ->
                let previous = current
                current <- None
                previous)

        previous |> Option.iter (fun session -> session.Dispose())

        this.EnsureStartedAsync(options, ?cancellationToken = cancellationToken)

    member private this.InvokeAsync(method: string, parameters: obj, cancellationToken) =
        task {
            match this.TryCurrent() with
            | None -> return Result.Error "No F# Interactive session is running."
            | Some session ->
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
            | None -> return false
            | Some session ->
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
                    current <- None
                    previous)

            previous |> Option.iter (fun session -> session.Dispose())
            startGate.Dispose()
