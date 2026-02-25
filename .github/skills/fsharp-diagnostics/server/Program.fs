module FSharpDiagServer.Program

open System

[<EntryPoint>]
let main argv =
    let mutable repoRoot = Environment.CurrentDirectory

    let mutable i = 0

    while i < argv.Length do
        match argv.[i] with
        | "--repo-root" when i + 1 < argv.Length ->
            repoRoot <- argv.[i + 1]
            i <- i + 2
        | other ->
            eprintfn $"Unknown argument: {other}"
            i <- i + 1

    // Resolve to absolute path
    repoRoot <- IO.Path.GetFullPath(repoRoot)

    let config: Server.ServerConfig =
        {
            RepoRoot = repoRoot
            IdleTimeoutMinutes = 240.0
        }

    eprintfn $"[fsharp-diag] Starting server for {repoRoot}"
    eprintfn $"[fsharp-diag] Socket: {Server.deriveSocketPath repoRoot}"

    Server.startServer config |> Async.RunSynchronously
    0
