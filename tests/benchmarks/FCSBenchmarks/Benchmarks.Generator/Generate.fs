module Benchmarks.Generator.Generate

open System
open System.Diagnostics
open System.IO
open System.Runtime.CompilerServices
open System.Threading
open Benchmarks.Common.Dtos
open CommandLine
open CommandLine.Text
open FSharp.Compiler.CodeAnalysis
open Ionide.ProjInfo
open Ionide.ProjInfo.Types
open Microsoft.Extensions.Logging
open Newtonsoft.Json

let private loggerFactory = LoggerFactory.Create(
    fun builder ->
        builder.AddSimpleConsole(fun options ->
            options.IncludeScopes <- false
            options.SingleLine <- true
            options.TimestampFormat <- "HH:mm:ss.fff"
        )
        |> ignore
)

type internal Marker = interface end
let private log = loggerFactory.CreateLogger<Marker>()

/// General utilities
[<RequireQualifiedAccess>]
module Utils =
    let runProcess name args workingDir (envVariables : (string * string) list) (printOutput : bool) =
        let info = ProcessStartInfo()
        info.WindowStyle <- ProcessWindowStyle.Hidden
        info.Arguments <- args
        info.FileName <- name
        info.UseShellExecute <- false
        info.WorkingDirectory <- workingDir
        info.RedirectStandardError <- true
        info.RedirectStandardOutput <- true
        info.RedirectStandardInput <- true
        info.CreateNoWindow <- true
        
        envVariables
        |> List.iter (fun (k, v) -> info.EnvironmentVariables[k] <- v)
        
        log.LogInformation $"Running '{name} {args}' in '{workingDir}'"
        let p = Process.Start(info)
        let o = p.StandardOutput.ReadToEnd()
        let errors = p.StandardError.ReadToEnd()
        p.WaitForExit()
        if p.ExitCode <> 0 then
            let msg = $"Process {name} {args} failed: {errors}."
            log.LogError $"{msg}. Its full output:"
            Thread.Sleep(100) // A rather hacky way to make sure that the above log is flushed before the long message below
            Console.ForegroundColor <- ConsoleColor.Gray
            printfn $"{o}"
            Console.ResetColor()
            failwith msg
        else if printOutput then
            log.LogInformation "Full output of the process:"
            Thread.Sleep(100) // A rather hacky way to make sure that the above log is flushed before the long message below
            Console.ForegroundColor <- ConsoleColor.DarkGray
            printfn $"{o}"
            Console.ResetColor()

/// Handling Git operations
[<RequireQualifiedAccess>]
module Git =
    open LibGit2Sharp
    
    let clone (dir : string) (gitUrl : string) : Repository =
        if Directory.Exists dir then
            failwith $"{dir} already exists for code root"
        log.LogInformation $"Fetching '{gitUrl}' in '{dir}'..."
        Repository.Init(dir) |> ignore
        let repo = new Repository(dir)
        let remote = repo.Network.Remotes.Add("origin", gitUrl)
        repo.Network.Fetch(remote.Name, [])
        repo
        
    let checkout (repo : Repository) (revision : string) : unit =
        log.LogInformation $"Checkout revision {revision} in {repo.Info.Path}"
        Commands.Checkout(repo, revision) |> ignore

/// Preparing a codebase based on a 'RepoSpec'
[<RequireQualifiedAccess>]
module RepoSetup =
    open LibGit2Sharp

    [<CLIMutable>]
    type RepoSpec =
        {
            Name : string
            GitUrl : string
            Revision : string
        }
            with override this.ToString() = $"{this.Name} - {this.GitUrl} at revision {this.Revision}"
        
    type Config =
        {
            BaseDir : string
        }
    
    let revisionDir (config : Config) (spec : RepoSpec) =
        Path.Combine(config.BaseDir, spec.Name, spec.Revision)
    
    let prepare (config : Config) (spec : RepoSpec) =
        log.LogInformation $"Checking out {spec}"
        let dir = revisionDir config spec
        if Repository.IsValid dir |> not then
            use repo = Git.clone dir spec.GitUrl
            Git.checkout repo spec.Revision
            repo
        else
            log.LogInformation $"{dir} already exists - will assume the correct repository is already checked out"
            new Repository(dir)

[<RequireQualifiedAccess>]
module Generate =
    
    type CheckAction =
        {
            FileName : string
            ProjectName : string
        }

    type CodebaseSourceType = Local | Git

    [<CLIMutable>]
    type CodebasePrepStep =
        {
            Command : string
            Args : string
        }
    
    [<CLIMutable>]
    type BenchmarkCase =
        {
            Repo : RepoSetup.RepoSpec
            LocalCodeRoot : string
            CodebasePrep : CodebasePrepStep list
            SlnRelative : string
            CheckActions : CheckAction list
        }
        
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let init (slnPath : string) =
        Init.init (DirectoryInfo(Path.GetDirectoryName slnPath)) None
    
    type Config =
        {
            CheckoutBaseDir : string
            RunnerProjectPath : string
        }
    
    type Codebase =
        | Local of string
        | Git of LibGit2Sharp.Repository
        with member this.Path = match this with | Local codeRoot -> codeRoot | Git repo -> repo.Info.WorkingDirectory
    
    let prepareCodebase (config : Config) (case : BenchmarkCase) : Codebase =
        use _ = log.BeginScope("PrepareCodebase")
        let codebase =
            match (case.Repo :> obj, case.LocalCodeRoot) with
            | null, null -> failwith "Either git repo or local code root details are required"
            | _, null ->
                let repo = RepoSetup.prepare {BaseDir = config.CheckoutBaseDir} case.Repo
                Codebase.Git repo
            | null, codeRoot ->
                Codebase.Local codeRoot
            | _, _ -> failwith $"Both git repo and local code root were provided - that's not supported"
        let sln = Path.Combine(codebase.Path, case.SlnRelative)
        log.LogInformation($"Running {case.CodebasePrep.Length} codebase prep steps...")
        case.CodebasePrep
        |> List.iteri (fun i step ->
            log.LogInformation($"Running codebase prep step [{i+1}/{case.CodebasePrep.Length}]")
            Utils.runProcess step.Command step.Args codebase.Path [] true
        )
        codebase
    
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let private doLoadOptions (toolsPath : ToolsPath) (sln : string) =
        // TODO allow customization of build properties
        let props = []
        let loader = WorkspaceLoader.Create(toolsPath, props)
        let vs = Microsoft.Build.Locator.MSBuildLocator.RegisterDefaults()        
        let projects = loader.LoadSln(sln, [], BinaryLogGeneration.Off) |> Seq.toList
        log.LogInformation $"{projects.Length} projects loaded"
        if projects.Length = 0 then
            failwith $"No projects were loaded from {sln} - this indicates an error in cracking the projects"
        
        let fsOptions =
            projects
            |> List.map (fun project -> Path.GetFileNameWithoutExtension(project.ProjectFileName), FCS.mapToFSharpProjectOptions project projects)
        fsOptions
        |> dict
    
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let private loadOptions (sln : string) =
        use _ = log.BeginScope("LoadOptions")
        log.LogInformation($"Constructing FSharpProjectOptions from {sln}")
        let toolsPath = init sln
        doLoadOptions toolsPath sln
    
    let private generateInputs (case : BenchmarkCase) (codeRoot : string) =
        let sln = Path.Combine(codeRoot, case.SlnRelative)
        let options = loadOptions sln
        
        log.LogInformation("Generating actions")
        let actions =
            case.CheckActions
            |> List.mapi (fun i {FileName = projectRelativeFileName; ProjectName = projectName} ->
                let project = options[projectName]
                let filePath = Path.Combine(Path.GetDirectoryName(project.ProjectFileName), projectRelativeFileName)
                let fileText = File.ReadAllText(filePath)
                BenchmarkAction.AnalyseFile {FileName = filePath; FileVersion = i; SourceText = fileText; Options = project}
            )
        
        let config : BenchmarkConfig =
            {
                BenchmarkConfig.ProjectCacheSize = 200
            }
            
        {
            BenchmarkInputs.Actions = actions
            BenchmarkInputs.Config = config
        }
    
    let private makeInputsPath (codeRoot : string) =
        let artifactsDir = Path.Combine(codeRoot, ".artifacts")
        let dateStr = DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss")
        Path.Combine(artifactsDir, $"{dateStr}.fcsinputs.json") 
    
    // These are the env variables that Ionide.ProjInfo seems to set (in-process).
    // We need to get rid of them so that the child 'dotnet run' process is using the right tools
    let private projInfoEnvVariables =
        [
            "MSBuildExtensionsPath"
            "DOTNET_ROOT"
            "MSBUILD_EXE_PATH"
            "DOTNET_HOST_PATH"
            "MSBuildSDKsPath"
        ]
    
    let private emptyProjInfoEnvironmentVariables () =
        projInfoEnvVariables
        |> List.map (fun var -> var, "")
    
    let private prepareAndRun (config : Config) (case : BenchmarkCase) (doRun : bool) (cleanup : bool) =
        let codebase = prepareCodebase config case
        let inputs = generateInputs case codebase.Path
        log.LogInformation("Serializing generated inputs")
        let serialized = serializeInputs inputs
        let inputsPath = makeInputsPath codebase.Path
        log.LogInformation $"Saving inputs as {inputsPath}"        
        Directory.CreateDirectory(Path.GetDirectoryName(inputsPath)) |> ignore
        File.WriteAllText(inputsPath, serialized)
        
        if doRun then
            use _ = log.BeginScope "Run"
            log.LogInformation "Starting the benchmark..."
            let workingDir = Path.GetDirectoryName(config.RunnerProjectPath)
            let envVariables = emptyProjInfoEnvironmentVariables() 
            log.LogInformation("Running the benchmark...")
            Utils.runProcess "dotnet" $"run -c Release -- {inputsPath}" workingDir envVariables true
        else
            log.LogInformation $"Not running the benchmark as requested"
            
        match codebase, cleanup with
        | Local _, _ -> ()
        | Git _, false -> ()
        | Git repo, true ->
            log.LogInformation $"Cleaning up checked out git repo {repo.Info.Path} as requested"
            Directory.Delete repo.Info.Path
    
    type Args =
        {
            [<CommandLine.Option('c', Default = ".artifacts", HelpText = "Base directory for git checkouts")>]
            CheckoutsDir : string
            [<CommandLine.Option('b', Default = "../Benchmarks.Runner/Benchmarks.Runner.fsproj", HelpText = "Path to the benchmark runner project - defaults to '../Benchmarks.Runner/Benchmarks.Runner.fsproj'")>]
            BenchmarkPath : string
            [<CommandLine.Option('i', Required = true, HelpText = "Path to the input file describing the benchmark")>]
            Input : string
            [<CommandLine.Option(Default = true, HelpText = "If set to false, prepares the benchmark and prints the commandline to run it, then exits")>]
            Run : bool
            [<CommandLine.Option(Default = false, HelpText = "If set, removes the checkout directory afterwards. Doesn't apply to local codebases")>]
            Cleanup : bool
        }
    
    let run (args : Args) =
        let config =
            {
                Config.CheckoutBaseDir = args.CheckoutsDir
                Config.RunnerProjectPath = Path.Combine(Environment.CurrentDirectory, args.BenchmarkPath)
            }
        let case =
            use _ = log.BeginScope("Read input")
            try
                let path = args.Input
                path
                |> File.ReadAllText
                |> JsonConvert.DeserializeObject<BenchmarkCase>
                |> fun case ->
                        let defaultCodebasePrep =
                            [
                                {
                                    CodebasePrepStep.Command = "dotnet"
                                    CodebasePrepStep.Args = $"restore {case.SlnRelative}"
                                }
                            ]
                        let codebasePrep =
                            match obj.ReferenceEquals(case.CodebasePrep, null) with
                            | true -> defaultCodebasePrep
                            | false -> case.CodebasePrep
                        
                        { case with CodebasePrep = codebasePrep }
            with e ->
                let msg = $"Failed to read inputs file: {e.Message}"
                log.LogCritical(msg)
                reraise()
        
        use _ = log.BeginScope("PrepareAndRun")
        prepareAndRun config case args.Run args.Cleanup
    
    let help result (errors : Error seq) =
        let helpText =
            let f (h:HelpText) =
                h.AdditionalNewLineAfterOption <- false
                h.Heading <- "FCS Benchmark Generator"
                h
            HelpText.AutoBuild(result, f, id)
        printfn $"{helpText}"
    
    [<EntryPoint>]
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let main args =
        printfn $"{Environment.CurrentDirectory}"
        let parseResult = Parser.Default.ParseArguments<Args> args
        parseResult
            .WithParsed(fun args -> run args)
            .WithNotParsed(fun errors -> help parseResult errors)
        |> ignore
        
        if parseResult.Tag = ParserResultType.Parsed then 0 else 1