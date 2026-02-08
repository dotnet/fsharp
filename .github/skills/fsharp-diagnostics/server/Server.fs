module FSharpDiagServer.Server

open System
open System.IO
open System.Net.Sockets
open System.Security.Cryptography
open System.Text
open System.Text.Json
open System.Threading
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Text

let private sockDir =
    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".fsharp-diag")

let private deriveHash (repoRoot: string) =
    SHA256.HashData(Encoding.UTF8.GetBytes(repoRoot))
    |> Convert.ToHexString
    |> fun s -> s.Substring(0, 16).ToLowerInvariant()

let deriveSocketPath repoRoot = Path.Combine(sockDir, $"{deriveHash repoRoot}.sock")
let deriveMetaPath repoRoot = Path.Combine(sockDir, $"{deriveHash repoRoot}.meta.json")
let deriveLogPath repoRoot = Path.Combine(sockDir, $"{deriveHash repoRoot}.log")

type ServerConfig = { RepoRoot: string; IdleTimeoutMinutes: float }

let startServer (config: ServerConfig) =
    async {
        let socketPath = deriveSocketPath config.RepoRoot
        let metaPath = deriveMetaPath config.RepoRoot
        let fsproj = Path.Combine(config.RepoRoot, "src/Compiler/FSharp.Compiler.Service.fsproj")
        Directory.CreateDirectory(sockDir) |> ignore
        if File.Exists(socketPath) then File.Delete(socketPath)

        let checker = FSharpChecker.Create(projectCacheSize = 3, useTransparentCompiler = true)
        let projectMgr = ProjectManager.ProjectManager(checker)
        let mutable lastActivity = DateTimeOffset.UtcNow
        let cts = new CancellationTokenSource()

        let getOptions () = projectMgr.ResolveProjectOptions(fsproj)

        let handleRequest (json: string) =
            async {
                lastActivity <- DateTimeOffset.UtcNow
                try
                    let doc = JsonDocument.Parse(json)
                    let command = doc.RootElement.GetProperty("command").GetString()

                    match command with
                    | "ping" ->
                        return $"""{{ "status":"ok", "pid":{Environment.ProcessId} }}"""

                    | "parseOnly" ->
                        let file = doc.RootElement.GetProperty("file").GetString()
                        if not (File.Exists file) then
                            return $"""{{ "error":"file not found: {file}" }}"""
                        else
                        let sourceText = SourceText.ofString (File.ReadAllText(file))
                        // Use project options for correct --langversion, --define etc
                        let! optionsResult = getOptions ()
                        let parsingArgs =
                            match optionsResult with
                            | Ok o -> o.OtherOptions |> Array.toList
                            | _ -> []
                        let parsingOpts, _ = checker.GetParsingOptionsFromCommandLineArgs(file :: parsingArgs)
                        let! parseResults = checker.ParseFile(file, sourceText, parsingOpts)
                        return DiagnosticsFormatter.formatFile parseResults.Diagnostics

                    | "check" ->
                        let file = Path.GetFullPath(doc.RootElement.GetProperty("file").GetString())
                        if not (File.Exists file) then
                            return $"""{{ "error":"file not found: {file}" }}"""
                        else
                        let! optionsResult = getOptions ()
                        match optionsResult with
                        | Error msg ->
                            return $"ERROR: {msg}"
                        | Ok options ->
                            let sourceText = SourceText.ofString (File.ReadAllText(file))
                            let version = File.GetLastWriteTimeUtc(file).Ticks |> int
                            let! parseResults, checkAnswer = checker.ParseAndCheckFileInProject(file, version, sourceText, options)
                            let diags =
                                match checkAnswer with
                                | FSharpCheckFileAnswer.Succeeded r -> Array.append parseResults.Diagnostics r.Diagnostics
                                | FSharpCheckFileAnswer.Aborted -> parseResults.Diagnostics
                                |> Array.distinctBy (fun d -> d.StartLine, d.Start.Column, d.ErrorNumberText)
                            return DiagnosticsFormatter.formatFile diags

                    | "checkProject" ->
                        let! optionsResult = getOptions ()
                        match optionsResult with
                        | Error msg ->
                            return $"ERROR: {msg}"
                        | Ok options ->
                            let! results = checker.ParseAndCheckProject(options)
                            return DiagnosticsFormatter.formatProject config.RepoRoot results.Diagnostics

                    | "shutdown" ->
                        cts.Cancel()
                        return """{ "status":"shutting_down" }"""

                    | other -> return $"ERROR: unknown command: {other}"
                with ex ->
                    return $"ERROR: {ex.Message}"
            }

        File.WriteAllText(metaPath, $"""{{ "repoRoot":"{config.RepoRoot}", "pid":{Environment.ProcessId} }}""")

        use listener = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified)
        listener.Bind(UnixDomainSocketEndPoint(socketPath))
        listener.Listen(10)
        File.SetUnixFileMode(socketPath, UnixFileMode.UserRead ||| UnixFileMode.UserWrite ||| UnixFileMode.UserExecute)
        eprintfn $"[fsharp-diag] Listening on {socketPath} (pid {Environment.ProcessId})"

        // Idle timeout
        Async.Start(
            async {
                while not cts.Token.IsCancellationRequested do
                    do! Async.Sleep(60_000 * 60)
                    if (DateTimeOffset.UtcNow - lastActivity).TotalMinutes > config.IdleTimeoutMinutes then
                        eprintfn "[fsharp-diag] Idle timeout"; cts.Cancel()
            }, cts.Token)

        try
            while not cts.Token.IsCancellationRequested do
                let! client = listener.AcceptAsync(cts.Token).AsTask() |> Async.AwaitTask
                Async.Start(
                    async {
                        try
                            use client = client
                            use stream = new NetworkStream(client)
                            use reader = new StreamReader(stream)
                            use writer = new StreamWriter(stream, AutoFlush = true)
                            let! line = reader.ReadLineAsync() |> Async.AwaitTask
                            if line <> null && line.Length > 0 then
                                let! response = handleRequest line
                                do! writer.WriteLineAsync(response) |> Async.AwaitTask
                        with ex -> eprintfn $"[fsharp-diag] Client error: {ex.Message}"
                    }, cts.Token)
        with
        | :? OperationCanceledException -> ()
        | ex -> eprintfn $"[fsharp-diag] Error: {ex.Message}"

        try File.Delete(socketPath) with _ -> ()
        try File.Delete(metaPath) with _ -> ()
        eprintfn "[fsharp-diag] Shut down."
    }
