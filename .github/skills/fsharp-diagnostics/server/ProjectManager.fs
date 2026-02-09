module FSharpDiagServer.ProjectManager

open System.Collections.Generic
open System.IO
open FSharp.Compiler.CodeAnalysis

type ProjectManager(checker: FSharpChecker) =
    let cache = Dictionary<string, System.DateTime * FSharpProjectOptions>()
    let gate = obj ()

    let isSourceFile (s: string) =
        not (s.StartsWith("-"))
        && (s.EndsWith(".fs", System.StringComparison.OrdinalIgnoreCase)
            || s.EndsWith(".fsi", System.StringComparison.OrdinalIgnoreCase))

    let normalize (path: string) = Path.GetFullPath(path)

    member _.ResolveProjectOptions(fsprojPath: string) =
        async {
            let key = normalize fsprojPath
            let fsprojMtime = File.GetLastWriteTimeUtc(key)
            let current =
                lock gate (fun () ->
                    match cache.TryGetValue(key) with
                    | true, (mtime, opts) when mtime = fsprojMtime -> Some opts
                    | true, _ -> cache.Remove(key) |> ignore; None
                    | false, _ -> None)

            match current with
            | Some opts -> return Ok opts
            | None ->
                let! dtbResult = DesignTimeBuild.run fsprojPath DesignTimeBuild.defaultConfig

                match dtbResult with
                | Error msg -> return Error msg
                | Ok dtb ->
                    let projDir = Path.GetDirectoryName(key)

                    let resolve (s: string) =
                        if Path.IsPathRooted(s) then s else Path.GetFullPath(Path.Combine(projDir, s))

                    let resolvedArgs =
                        dtb.CompilerArgs
                        |> Array.map (fun a -> if isSourceFile a then resolve a else a)

                    let sourceFiles = resolvedArgs |> Array.filter isSourceFile
                    let flagsOnly = resolvedArgs |> Array.filter (not << isSourceFile)
                    let opts = checker.GetProjectOptionsFromCommandLineArgs(key, flagsOnly)
                    let options = { opts with SourceFiles = sourceFiles }
                    lock gate (fun () -> cache.[key] <- (fsprojMtime, options))
                    return Ok options
        }

    member _.Invalidate(?fsprojPath: string) =
        lock gate (fun () ->
            match fsprojPath with
            | Some p -> cache.Remove(normalize p) |> ignore
            | None -> cache.Clear())

    member _.CacheCount = lock gate (fun () -> cache.Count)

    member _.HasCachedProject(fsprojPath: string) =
        lock gate (fun () -> cache.ContainsKey(normalize fsprojPath))

    member _.InjectTestEntry(fsprojPath: string, options: FSharpProjectOptions) =
        let key = normalize fsprojPath
        lock gate (fun () -> cache.[key] <- (System.DateTime.MinValue, options))
