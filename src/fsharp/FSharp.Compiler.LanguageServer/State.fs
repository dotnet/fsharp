// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.LanguageServer

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Diagnostics
open System.IO
open System.Text.RegularExpressions
open FSharp.Compiler.SourceCodeServices
open StreamJsonRpc

module internal Solution =
    // easy unit testing
    let getProjectPaths (solutionContent: string) (solutionDir: string) =
        // This looks scary, but is much more lightweight than carrying along MSBuild just to have it parse the solution file.
        //
        // A valid line in .sln looks like:
        //   Project("{F2A71F9B-5D33-465A-A702-920D77279786}") = "ConsoleApp2", "ConsoleApp2\ConsoleApp2.fsproj", "{60A4BE67-7E03-4200-AD38-B0E5E8E049C1}"
        // and we're hoping to extract this: ------------------------------------^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
        //
        // therefore:
        //                    ^Project                           text 'Project' at the start of the line
        //                            .*                         any number of characters
        //                              \""                      double quote character (it's doubled up to escape from the raw string literal here)
        //                                 (                     start of capture group
        //                                  [^\""]               not a quote
        //                                        *              many of those
        //                                         \.fsproj      literal string ".fsproj"
        //                                                 )     end of capture group
        //                                                  \""  double quote
        let pattern = Regex(@"^Project.*\""([^\""]*\.fsproj)\""")
        let lines = solutionContent.Split('\n')
        let relativeProjects =
            lines
            |> Array.map pattern.Match
            |> Array.filter (fun m -> m.Success)
            |> Array.map (fun m -> m.Groups.[1].Value)
            // .sln files by convention uses backslashes, which might not be appropriate at runtime
            |> Array.map (fun p -> p.Replace('\\', Path.DirectorySeparatorChar))
        let projects =
            relativeProjects
            |> Array.map (fun p -> if Path.IsPathRooted(p) then p else Path.Combine(solutionDir, p))
        projects

type State() =

    let checker = FSharpChecker.Create()

    let sourceFileToProjectMap = ConcurrentDictionary<string, FSharpProjectOptions>()

    let shutdownEvent = new Event<_>()
    let exitEvent = new Event<_>()
    let cancelEvent = new Event<_>()
    let projectInvalidatedEvent = new Event<_>()

    let fileChanged (args: FileSystemEventArgs) =
        match sourceFileToProjectMap.TryGetValue args.FullPath with
        | true, projectOptions -> projectInvalidatedEvent.Trigger(projectOptions)
        | false, _ -> ()
    let fileRenamed (args: RenamedEventArgs) =
        match sourceFileToProjectMap.TryGetValue args.FullPath with
        | true, projectOptions -> projectInvalidatedEvent.Trigger(projectOptions)
        | false, _ -> ()
    let fileWatcher = new FileSystemWatcher()
    do fileWatcher.IncludeSubdirectories <- true
    do fileWatcher.Changed.Add(fileChanged)
    do fileWatcher.Created.Add(fileChanged)
    do fileWatcher.Deleted.Add(fileChanged)
    do fileWatcher.Renamed.Add(fileRenamed)

    let execProcess (name: string) (args: string) =
        let startInfo = ProcessStartInfo(name, args)
        eprintfn "executing: %s %s" name args
        startInfo.CreateNoWindow <- true
        startInfo.RedirectStandardOutput <- true
        startInfo.UseShellExecute <- false
        let lines = List<string>()
        use proc = new Process()
        proc.StartInfo <- startInfo
        proc.OutputDataReceived.Add(fun args -> lines.Add(args.Data))
        proc.Start() |> ignore
        proc.BeginOutputReadLine()
        proc.WaitForExit()
        lines.ToArray()

    let linesWithPrefixClean (prefix: string) (lines: string[]) =
        lines
        |> Array.filter (isNull >> not)
        |> Array.map (fun line -> line.TrimStart(' '))
        |> Array.filter (fun line -> line.StartsWith(prefix))
        |> Array.map (fun line -> line.Substring(prefix.Length))

    let getProjectOptions (rootDir: string) =
        if isNull rootDir then [||]
        else
            fileWatcher.Path <- rootDir
            fileWatcher.EnableRaisingEvents <- true

            /// This function is meant to be temporary.  Until we figure out what a language server for a project
            /// system looks like, we have to guess based on the files we find in the root.
            let getProjectOptions (projectPath: string) =
                let projectDir = Path.GetDirectoryName(projectPath)
                let normalizePath (path: string) =
                    if Path.IsPathRooted(path) then path
                    else Path.Combine(projectDir, path)

                // To avoid essentially re-creating a copy of MSBuild alongside this tool, we instead fake a design-
                // time build with this project.  The output of building this helper project is text that's easily
                // parsable.  See the helper project for more information.
                let reporterProject = Path.Combine(Path.GetDirectoryName(typeof<State>.Assembly.Location), "FSharp.Compiler.LanguageServer.DesignTime.proj")
                let detectedTfmSentinel = "DetectedTargetFramework="
                let detectedCommandLineArgSentinel = "DetectedCommandLineArg="

                let execTfmReporter =
                    sprintf "build \"%s\" \"/p:ProjectFile=%s\"" reporterProject projectPath
                    |> execProcess "dotnet"

                let execArgReporter (tfm: string) =
                    sprintf "build \"%s\" \"/p:ProjectFile=%s\" \"/p:TargetFramework=%s\"" reporterProject projectPath tfm
                    |> execProcess "dotnet"

                // find the target frameworks
                let targetFrameworks =
                    execTfmReporter
                    |> linesWithPrefixClean detectedTfmSentinel

                let getArgs (tfm: string) =
                    execArgReporter tfm
                    |> linesWithPrefixClean detectedCommandLineArgSentinel

                let tfmAndArgs =
                    targetFrameworks
                    |> Array.map (fun tfm -> tfm, getArgs tfm)

                let separateArgs (args: string[]) =
                    args
                    |> Array.partition (fun a -> a.StartsWith("-"))
                    |> (fun (options, files) ->
                        let normalizedFiles = files |> Array.map normalizePath
                        options, normalizedFiles)

                // TODO: for now we're only concerned with the first TFM
                let _tfm, args = Array.head tfmAndArgs

                let otherOptions, sourceFiles = separateArgs args

                let projectOptions: FSharpProjectOptions =
                    { ProjectFileName = projectPath
                      ProjectId = None
                      SourceFiles = sourceFiles
                      OtherOptions = otherOptions
                      ReferencedProjects = [||] // TODO: populate from @(ProjectReference)
                      IsIncompleteTypeCheckEnvironment = false
                      UseScriptResolutionRules = false
                      LoadTime = DateTime.Now
                      UnresolvedReferences = None
                      OriginalLoadReferences = []
                      ExtraProjectInfo = None
                      Stamp = None }
                projectOptions
            let topLevelProjects = Directory.GetFiles(rootDir, "*.fsproj")
            let watchableProjectPaths =
                match topLevelProjects with
                | [||] ->
                    match Directory.GetFiles(rootDir, "*.sln") with
                    // TODO: what to do with multiple .sln or a combo of .sln/.fsproj?
                    | [| singleSolution |] ->
                        let content = File.ReadAllText(singleSolution)
                        let solutionDir = Path.GetDirectoryName(singleSolution)
                        Solution.getProjectPaths content solutionDir
                    | _ -> [||]
                | _ -> topLevelProjects
            let watchableProjectOptions =
                watchableProjectPaths
                |> Array.map getProjectOptions

            // associate source files with project options
            let watchFile file projectOptions =
                sourceFileToProjectMap.AddOrUpdate(file, projectOptions, fun _ _ -> projectOptions)

            for projectOptions in watchableProjectOptions do
                // watch .fsproj
                watchFile projectOptions.ProjectFileName projectOptions |> ignore
                // TODO: watch .deps.json
                for sourceFile in projectOptions.SourceFiles do
                    let sourceFileFullPath =
                        if Path.IsPathRooted(sourceFile) then sourceFile
                        else
                            let projectDir = Path.GetDirectoryName(projectOptions.ProjectFileName)
                            Path.Combine(projectDir, sourceFile)
                    watchFile sourceFileFullPath projectOptions |> ignore

            watchableProjectOptions

    member __.Checker = checker

    /// Initialize the LSP at the specified location.  According to the spec, `rootUri` is to be preferred over `rootPath`.
    member __.Initialize (rootPath: string) (rootUri: DocumentUri) (computeDiagnostics: FSharpProjectOptions -> unit) =
        let rootDir =
            if not (isNull rootUri) then Uri(rootUri).LocalPath
            else rootPath
        let projectOptions = getProjectOptions rootDir
        projectInvalidatedEvent.Publish.Add computeDiagnostics // compute diagnostics on project invalidation
        for projectOption in projectOptions do
            computeDiagnostics projectOption // compute initial set of diagnostics

    [<CLIEvent>]
    member __.Shutdown = shutdownEvent.Publish

    [<CLIEvent>]
    member __.Exit = exitEvent.Publish

    [<CLIEvent>]
    member __.Cancel = cancelEvent.Publish

    [<CLIEvent>]
    member __.ProjectInvalidated = projectInvalidatedEvent.Publish

    member __.DoShutdown() = shutdownEvent.Trigger()

    member __.DoExit() = exitEvent.Trigger()

    member __.DoCancel() = cancelEvent.Trigger()

    member __.InvalidateAllProjects() =
        for projectOptions in sourceFileToProjectMap.Values do
            projectInvalidatedEvent.Trigger(projectOptions)

    member val Options = Options.Default() with get, set

    member val JsonRpc: JsonRpc option = None with get, set
