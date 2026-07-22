namespace UseLocalCompiler.FSharp.Build.Tasks

open System
open System.IO
open Microsoft.Build.Framework
open Microsoft.Build.Utilities

type public Fsc() =
    inherit FSharp.Build.Fsc()

    static member private TryGetBuildContext() =
        try
            let assemblyDir = DirectoryInfo(Path.GetDirectoryName(typeof<Fsc>.Assembly.Location))

            if
                not (isNull assemblyDir.Parent)
                && not (isNull assemblyDir.Parent.Parent)
                && not (isNull assemblyDir.Parent.Parent.Parent)
                && not (isNull assemblyDir.Parent.Parent.Parent.Parent)
                && not (isNull assemblyDir.Parent.Parent.Parent.Parent.Parent)
                && StringComparer.OrdinalIgnoreCase.Equals(assemblyDir.Parent.Parent.Parent.Name, "bin")
                && StringComparer.OrdinalIgnoreCase.Equals(assemblyDir.Parent.Parent.Parent.Parent.Name, "artifacts")
            then
                Some(assemblyDir.Parent.Parent.Parent.Parent.Parent.FullName, assemblyDir.Parent.Name)
            else
                None
        with _ ->
            None

    static member private TryGetLocalDotnetFscCompilerPath(repoRoot: string, configuration: string) =
        let fscOutputRoot = Path.Combine(repoRoot, "artifacts", "bin", "fsc", configuration)

        if Directory.Exists fscOutputRoot then
            fscOutputRoot
            |> Directory.GetDirectories
            |> Array.choose (fun dir ->
                let candidate = Path.Combine(dir, "fsc.dll")

                if File.Exists candidate then
                    Some candidate
                else
                    None)
            |> function
                | [| candidate |] -> Some candidate
                | _ -> None
        else
            None

    static member private IsFSharpCoreReference(item: ITaskItem) =
        let itemName = Path.GetFileNameWithoutExtension(item.ItemSpec)
        StringComparer.OrdinalIgnoreCase.Equals(itemName, "FSharp.Core")

    static member private TryGetReferenceTargetFramework(item: ITaskItem) =
        try
            Path.GetDirectoryName(item.ItemSpec)
            |> Path.GetFileName
            |> function
                | null
                | "" -> None
                | tfm -> Some tfm
        with _ ->
            None

    static member private TryGetLocalFSharpCoreReference(responseFileCommands: string) =
        let referenceLines =
            responseFileCommands.Split([| '\r'; '\n' |], StringSplitOptions.RemoveEmptyEntries)
            |> Array.filter (fun line -> line.StartsWith("-r:", StringComparison.Ordinal))

        let fsharpCoreTargetFrameworks =
            referenceLines
            |> Array.map (fun line -> line.Substring(3).Trim([| '"' |]))
            |> Array.filter (fun path -> Fsc.IsFSharpCoreReference(TaskItem(path) :> ITaskItem))
            |> Array.choose (fun path -> Fsc.TryGetReferenceTargetFramework(TaskItem(path) :> ITaskItem))
            |> Array.distinct

        match Fsc.TryGetBuildContext(), fsharpCoreTargetFrameworks with
        | Some(repoRoot, configuration), [| fsharpCoreTargetFramework |] ->
            let localFSharpCorePath =
                Path.Combine(repoRoot, "artifacts", "bin", "FSharp.Core", configuration, fsharpCoreTargetFramework, "FSharp.Core.dll")

            if File.Exists localFSharpCorePath then
                Some localFSharpCorePath
            else
                None
        | _ -> None

    override _.GenerateCommandLineCommands() =
        match Fsc.TryGetBuildContext() with
        | Some(repoRoot, configuration) ->
            match Fsc.TryGetLocalDotnetFscCompilerPath(repoRoot, configuration) with
            | Some localDotnetFscCompilerPath -> localDotnetFscCompilerPath
            | None -> base.GenerateCommandLineCommands()
        | None -> base.GenerateCommandLineCommands()

    override _.GenerateResponseFileCommands() =
        let responseFileCommands = base.GenerateResponseFileCommands()

        match Fsc.TryGetLocalFSharpCoreReference(responseFileCommands) with
        | None -> responseFileCommands
        | Some localFSharpCorePath ->
            let mutable emittedLocalFSharpCoreReference = false

            responseFileCommands.Split([| '\r'; '\n' |], StringSplitOptions.RemoveEmptyEntries)
            |> Array.choose (fun line ->
                if line.StartsWith("-r:", StringComparison.Ordinal) then
                    let referencePath = line.Substring(3).Trim([| '"' |])

                    if Fsc.IsFSharpCoreReference(TaskItem(referencePath) :> ITaskItem) then
                        if emittedLocalFSharpCoreReference then
                            None
                        else
                            emittedLocalFSharpCoreReference <- true
                            Some("-r:" + localFSharpCorePath)
                    else
                        Some line
                else
                    Some line)
            |> String.concat Environment.NewLine
