module FSharpDiagServer.ProjectManager

open System.IO
open FSharp.Compiler.CodeAnalysis

type ProjectManager(checker: FSharpChecker) =
    let mutable cached: (System.DateTime * FSharpProjectOptions) option = None
    let gate = obj ()

    let isSourceFile (s: string) =
        not (s.StartsWith("-"))
        && (s.EndsWith(".fs", System.StringComparison.OrdinalIgnoreCase)
            || s.EndsWith(".fsi", System.StringComparison.OrdinalIgnoreCase))

    member _.ResolveProjectOptions(fsprojPath: string) =
        async {
            let fsprojMtime = File.GetLastWriteTimeUtc(fsprojPath)
            let current =
                lock gate (fun () ->
                    match cached with
                    | Some(mtime, opts) when mtime = fsprojMtime -> Some opts
                    | Some _ -> cached <- None; None
                    | None -> None)

            match current with
            | Some opts -> return Ok opts
            | None ->
                let! dtbResult = DesignTimeBuild.run fsprojPath DesignTimeBuild.defaultConfig

                match dtbResult with
                | Error msg -> return Error msg
                | Ok dtb ->
                    let projDir = Path.GetDirectoryName(fsprojPath)

                    let resolve (s: string) =
                        if Path.IsPathRooted(s) then s else Path.GetFullPath(Path.Combine(projDir, s))

                    let resolvedArgs =
                        dtb.CompilerArgs
                        |> Array.map (fun a -> if isSourceFile a then resolve a else a)

                    let sourceFiles = resolvedArgs |> Array.filter isSourceFile
                    let flagsOnly = resolvedArgs |> Array.filter (not << isSourceFile)
                    let opts = checker.GetProjectOptionsFromCommandLineArgs(fsprojPath, flagsOnly)
                    let options = { opts with SourceFiles = sourceFiles }
                    lock gate (fun () -> cached <- Some(fsprojMtime, options))
                    return Ok options
        }

    member _.Invalidate() = lock gate (fun () -> cached <- None)
