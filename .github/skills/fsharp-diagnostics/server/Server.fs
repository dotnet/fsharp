module FSharpDiagServer.Server

open System
open System.IO
open System.Net.Sockets
open System.Security.Cryptography
open System.Text
open System.Text.Json
open System.Threading
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
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

                    | "findRefs" ->
                        let file = Path.GetFullPath(doc.RootElement.GetProperty("file").GetString())
                        let line = doc.RootElement.GetProperty("line").GetInt32()
                        let col = doc.RootElement.GetProperty("col").GetInt32()
                        if not (File.Exists file) then
                            return $"ERROR: file not found: {file}"
                        else
                        let! optionsResult = getOptions ()
                        match optionsResult with
                        | Error msg -> return $"ERROR: {msg}"
                        | Ok options ->
                            let sourceText = SourceText.ofString (File.ReadAllText(file))
                            let version = File.GetLastWriteTimeUtc(file).Ticks |> int
                            let! _, checkAnswer = checker.ParseAndCheckFileInProject(file, version, sourceText, options)
                            match checkAnswer with
                            | FSharpCheckFileAnswer.Aborted -> return "ERROR: check aborted"
                            | FSharpCheckFileAnswer.Succeeded checkResults ->
                                let sourceLines = File.ReadAllLines file
                                let lineText = sourceLines.[line - 1]
                                let isIdChar c = Char.IsLetterOrDigit(c) || c = '_' || c = '\''
                                let mutable endCol = col
                                while endCol < lineText.Length && isIdChar lineText.[endCol] do endCol <- endCol + 1
                                let mutable startCol = col
                                while startCol > 0 && isIdChar lineText.[startCol - 1] do startCol <- startCol - 1
                                let name = lineText.[startCol..endCol - 1]
                                if name.Length = 0 then
                                    return "ERROR: no identifier at that position"
                                else
                                match checkResults.GetSymbolUseAtLocation(line, endCol, lineText, [name]) with
                                | None -> return $"ERROR: no symbol found for '{name}' at {line}:{col}"
                                | Some symbolUse ->
                                    let! projectResults = checker.ParseAndCheckProject(options)
                                    // Collect related symbols: for DU types, also search union cases
                                    let targetNames = ResizeArray<string>()
                                    targetNames.Add(symbolUse.Symbol.FullName)
                                    match symbolUse.Symbol with
                                    | :? FSharpEntity as ent when ent.IsFSharpUnion ->
                                        for uc in ent.UnionCases do targetNames.Add(uc.FullName)
                                    | _ -> ()
                                    let uses =
                                        projectResults.GetAllUsesOfAllSymbols()
                                        |> Array.filter (fun u -> targetNames.Contains(u.Symbol.FullName))
                                    let root = config.RepoRoot.TrimEnd('/') + "/"
                                    let rel (p: string) = if p.StartsWith(root) then p.Substring(root.Length) else p
                                    let lines =
                                        uses |> Array.map (fun u ->
                                            let kind = if u.IsFromDefinition then "DEF" elif u.IsFromType then "TYPE" else "USE"
                                            $"{kind} {rel u.Range.FileName}:{u.Range.StartLine},{u.Range.StartColumn}")
                                        |> Array.distinct
                                    let sym = symbolUse.Symbol
                                    let header = $"Symbol: {sym.DisplayName} ({sym.GetType().Name}) — {lines.Length} references"
                                    return header + "\n" + (lines |> String.concat "\n")

                    | "typeHints" ->
                        let file = Path.GetFullPath(doc.RootElement.GetProperty("file").GetString())
                        let startLine = doc.RootElement.GetProperty("startLine").GetInt32()
                        let endLine = doc.RootElement.GetProperty("endLine").GetInt32()
                        if not (File.Exists file) then
                            return $"ERROR: file not found: {file}"
                        else
                        let! optionsResult = getOptions ()
                        match optionsResult with
                        | Error msg -> return $"ERROR: {msg}"
                        | Ok options ->
                            let sourceText = SourceText.ofString (File.ReadAllText(file))
                            let version = File.GetLastWriteTimeUtc(file).Ticks |> int
                            let! _, checkAnswer = checker.ParseAndCheckFileInProject(file, version, sourceText, options)
                            match checkAnswer with
                            | FSharpCheckFileAnswer.Aborted -> return "ERROR: check aborted"
                            | FSharpCheckFileAnswer.Succeeded checkResults ->
                                let allSymbols = checkResults.GetAllUsesOfAllSymbolsInFile()
                                let sourceLines = File.ReadAllLines(file)
                                // Collect type annotations per line: (name: Type)
                                let annotations = System.Collections.Generic.Dictionary<int, ResizeArray<string>>()
                                let addHint line hint =
                                    if not (annotations.ContainsKey line) then annotations.[line] <- ResizeArray()
                                    annotations.[line].Add(hint)
                                let tagsToStr (tags: FSharp.Compiler.Text.TaggedText[]) =
                                    tags |> Array.map (fun t -> t.Text) |> String.concat ""
                                for su in allSymbols do
                                    let r = su.Range
                                    if r.StartLine >= startLine && r.StartLine <= endLine && su.IsFromDefinition then
                                        match su.Symbol with
                                        | :? FSharpMemberOrFunctionOrValue as mfv ->
                                            match mfv.GetReturnTypeLayout(su.DisplayContext) with
                                            | Some tags ->
                                                let typeStr = tagsToStr tags
                                                // Format as F# type annotation: (name: Type)
                                                addHint r.StartLine $"({mfv.DisplayName}: {typeStr})"
                                            | None ->
                                                // Fallback: try FullType
                                                try addHint r.StartLine $"({mfv.DisplayName}: {mfv.FullType.Format(su.DisplayContext)})"
                                                with _ -> ()
                                        | :? FSharpField as fld ->
                                            try addHint r.StartLine $"({fld.DisplayName}: {fld.FieldType.Format(su.DisplayContext)})"
                                            with _ -> ()
                                        | _ -> ()
                                // Render lines with inline type comments
                                let sb = StringBuilder()
                                for i in startLine .. endLine do
                                    if i >= 1 && i <= sourceLines.Length then
                                        let line = sourceLines.[i - 1]
                                        match annotations.TryGetValue(i) with
                                        | true, hints ->
                                            let comment = hints |> Seq.distinct |> String.concat "  " 
                                            sb.AppendLine($"{line}  // {comment}") |> ignore
                                        | _ ->
                                            sb.AppendLine(line) |> ignore
                                return sb.ToString().TrimEnd()

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
