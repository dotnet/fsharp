/// Utilities for running benchmarks for different versions of the codebase.
/// Used in '../runner.ipynb'
module HistoricalBenchmark.Runner

open System
open System.Text.RegularExpressions
open LibGit2Sharp
open Newtonsoft.Json.Linq
open NuGet.Configuration
open NuGet.Packaging.Core
open NuGet.Packaging.Signing
open NuGet.Protocol
open System.Threading
open NuGet.Protocol.Core.Types
open NuGet.Common
open NuGet.Versioning
open System.IO
open System.Diagnostics
open Plotly.NET
open Newtonsoft.Json
open Plotly.NET.GenericChart

[<RequireQualifiedAccess>]
module Utils =
    let runProcess name args workingDir (envVariables : (string * string) list) =
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
        |> List.iter (info.EnvironmentVariables.Add)
        printfn $"Running '{name} {args}' in '{workingDir}'"
        let p = Process.Start(info)
        let o = p.StandardOutput.ReadToEnd()
        let errors = p.StandardError.ReadToEnd()
        p.WaitForExit()
        if p.ExitCode <> 0 then
            let msg = $"Process {name} {args} failed: {errors}."
            printfn $"{msg}. Its full output: {o}"
            failwith msg

module Build =
    type MSBuildProps = (string * string) list
        
    module MSBuildProps =
        let private makeVersionDefines (v : NuGetVersion) =
            if v.Major < 30 then "SERVICE_13_0_0"
            elif v.Major < 40 then "SERVICE_30_0_0"
            else ""
        let makeProject (path : string) =
            [
                "FcsReferenceType", "project"
                "FcsProjectPath", path
                "DefineConstants", ""
            ]
        let makeNuGet (version : NuGetVersion) =
            [
                "FcsReferenceType", "nuget"
                "FcsNuGetVersion", version.OriginalVersion
                "DefineConstants", makeVersionDefines version
            ]
        let makeDll (fcsDllPath : string) (fsharpCoreDllPath : string) =
            [
                "FcsReferenceType", "dll"
                "FcsDllPath", fcsDllPath
                "FSharpCoreDllPath", fsharpCoreDllPath
                "DefineConstants", "" // This means the 'revision' method doesn't support codebases prior to v40 
            ]

    let buildFCS (dir : string) =
        Utils.runProcess "cmd" $"/C build.cmd -c Release -noVisualStudio" dir []

    let runBenchmark (outputDir : string) (msBuildProps : MSBuildProps) =
        let dir = Path.Combine(__SOURCE_DIRECTORY__, "../")
        Utils.runProcess "dotnet" $"build -c Release HistoricalBenchmark.fsproj" dir msBuildProps
        Utils.runProcess "dotnet" $"run -c Release --no-build --project HistoricalBenchmark.fsproj -- --filter *DecentlySizedStandAloneFileBenchmark* -a {outputDir}" dir msBuildProps

/// Benchmarking current codebase
[<RequireQualifiedAccess>]
module Local =
    let build () =
        Build.buildFCS @"..\..\..\..\"
    
    let benchmark (outputDir : string) =
        let msBuildArgs = Build.MSBuildProps.makeProject @"..\..\..\..\src\Compiler\FSharp.Compiler.Service.fsproj"
        Build.runBenchmark outputDir msBuildArgs

/// Benchmarking a version of FCS from Nuget
[<RequireQualifiedAccess>]
module NuGet =
    let private source = "https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-public/nuget/v3/index.json"
    let private packageId = "FSharp.Compiler.Service"
    
    let private getAllVersionsMetadata () =
        let cache = new SourceCacheContext()
        let repo = Repository.Factory.GetCoreV3(source);
        let resource = repo.GetResourceAsync<PackageMetadataResource>() |> Async.AwaitTask |> Async.RunSynchronously
        resource.GetMetadataAsync(packageId, true, false, cache, NullLogger.Instance, CancellationToken.None)
        |> Async.AwaitTask
        |> Async.RunSynchronously
        |> Seq.toList
    
    let allVersionsMetadata = lazy getAllVersionsMetadata
    
    /// Fetch a given version of FCS into the global packages folder
    let private downloadPackage (version : NuGetVersion) =
        let logger = NullLogger.Instance
        let cancellationToken = CancellationToken.None
        
        let settings = Settings.LoadDefaultSettings(null)
        let globalPackagesFolder = SettingsUtility.GetGlobalPackagesFolder(settings);

        let cache = new SourceCacheContext();
        let repository = Repository.Factory.GetCoreV3(source);
        let resource = repository.GetResourceAsync<FindPackageByIdResource>() |> Async.AwaitTask |> Async.RunSynchronously

        // Download the package
        use packageStream = new MemoryStream()
        resource.CopyNupkgToStreamAsync(packageId, version, packageStream, cache, logger, cancellationToken)
        |> Async.AwaitTask |> Async.RunSynchronously
        |> ignore

        packageStream.Seek(0, SeekOrigin.Begin)
        |> ignore

        // Add it to the global package folder
        GlobalPackagesFolderUtility.AddPackageAsync(source,
            PackageIdentity(packageId, version),
            packageStream,
            globalPackagesFolder,
            Guid.Empty,
            ClientPolicyContext.GetClientPolicy(settings, logger),
            logger,
            cancellationToken
        )
        |> Async.AwaitTask |> Async.RunSynchronously
        |> ignore
        
    let resolvePackageRevision (v : NuGetVersion) =
        printfn $"Downloading FCS package version {v} to find out its revision"
        downloadPackage v
        let settings = Settings.LoadDefaultSettings(null)
        let globalPackagesFolder = SettingsUtility.GetGlobalPackagesFolder(settings)
        let path = Path.Combine(globalPackagesFolder, packageId, v.OriginalVersion, "lib", "netstandard2.0", "FSharp.Compiler.Service.dll")
        let vi = FileVersionInfo.GetVersionInfo(path)
        let pv = vi.ProductVersion
        match Regex.Match(pv, "\+([a-zA-Z0-9]+)$") with
        | r when r.Success -> r.Groups[1].Value
        | _ -> failwith $"Couldn't match product version {pv}"
            
    let findVersionMetadata (version : string) =
        allVersionsMetadata.Value()
        |> List.find (fun v -> v.Identity.Version.OriginalVersion = version)

    let findVersionDate (version : NuGetVersion) =
        version.OriginalVersion
        |> findVersionMetadata
        |> fun metadata -> metadata.Published.Value.UtcDateTime
        
    let getMetadataVersion (metadata : IPackageSearchMetadata) =
        metadata.Identity.Version
        
    let findVersion (versionString : string) =
        versionString
        |> findVersionMetadata
        |> getMetadataVersion
        
    let benchmark (outputDir : string) (version : NuGetVersion) =
        Build.runBenchmark outputDir (Build.MSBuildProps.makeNuGet version)

type RunConfig =
    {
        /// Used to identify a set of BDN artifacts - set to a unique value unless you want to reuse (partial) results from previous runs
        Time : DateTime
        /// Used to store checkouts and BDN artifacts - set to a valid local absolute path
        BaseDir : string
        /// How many revisions should be checked out and built in parallel
        Parallelism : int
        /// Name to suffx the benchmark result files with
        ResultsSuffix : string
    }
    with
        member this.CheckoutBaseDir = Path.Combine(this.BaseDir, "checkouts")
        member this.BDNOutputBaseDir = Path.Combine(this.BaseDir, "bdns")

module RunConfig =
    let makeDefault () =
        {
            Time = DateTime.UtcNow
            BaseDir = "."
            Parallelism = 1
            ResultsSuffix = "results"
        }

/// Benchmarking a specific git revision of FCS
[<RequireQualifiedAccess>]
module Git =
    open LibGit2Sharp
            
    let clone (path : string) : Repository =
        printfn $"Fetching 'dotnet/fsharp.git' in '{path}'..."
        Repository.Init(path) |> ignore
        let repo = new Repository(path)
        let remote = repo.Network.Remotes.Add("origin", "https://github.com/dotnet/fsharp.git")
        repo.Network.Fetch(remote.Name, [])
        repo
        
    let checkout (commit : string) (repo : Repository) : unit =
        printfn $"Checkout {commit} in {repo.Info.Path}"
        Commands.Checkout(repo, commit) |> ignore
    
    let revisionDir (baseDir : string) (revision : string) =
        Path.Combine(baseDir, revision)
    
    let prepareRevisionCheckout (baseDir : string) (revision : string) =
        let dir = revisionDir baseDir revision
        if Repository.IsValid dir |> not then
            printfn $"Checking out revision {revision} in {dir}"
            try Directory.Delete dir with _ -> ()
            use repo = clone dir
            checkout revision repo
        else
            printfn $"{revision} already checked out in {dir}"
    
    let private fcsDllPath (checkoutDir : string) =
        Path.Combine(checkoutDir, "artifacts/bin/FSharp.Compiler.Service/Release/netstandard2.0/FSharp.Compiler.Service.dll")
        
    let private fsharpCoreDllPath (rootDir : string) =
        (fcsDllPath rootDir).Replace("FSharp.Compiler.Service.dll", "FSharp.Core.dll")
    
    let checkoutContainsBuiltFcs (checkoutDir : string) =
        File.Exists(fcsDllPath checkoutDir)
    
    let private prepareRevisionBuild (checkoutsBaseDir : string) (revision : string) =
        let dir = revisionDir checkoutsBaseDir revision
        if checkoutContainsBuiltFcs dir |> not then
            printfn $"'{fcsDllPath dir}' doesn't exist - building revision {revision} in {dir}..."
            Build.buildFCS dir
        else
            printfn $"{revision} already built in {dir}"
    
    let checkoutAndBuild (checkoutsBaseDir : string) (revision : string) =
        prepareRevisionCheckout checkoutsBaseDir revision
        prepareRevisionBuild checkoutsBaseDir revision
    
    let private prepareMainRepo (config : RunConfig) =
        let dir = revisionDir config.CheckoutBaseDir "main"
        if Directory.Exists dir then new Repository(dir)
        else clone dir
    
    let findCommitsBetweenInclusive (config : RunConfig) (older : Commit) (newer : Commit) =
        let repo = prepareMainRepo config
        let filter : CommitFilter = CommitFilter(IncludeReachableFrom=newer, ExcludeReachableFrom=older)
        repo.Commits.QueryBy(filter)
        |> Seq.toList
        |> fun l ->
            if l |> List.contains older then l
            else l @ [older] 
    
    /// Some revisions don't build - exclude them
    let excludeBadCommits (badCommits : string list) (commits : Commit list) =
        let knownBadCommits = badCommits
        let goodCommits = commits |> List.filter (fun c -> knownBadCommits |> List.contains c.Sha |> not)
        printfn $"Resolved {goodCommits.Length} valid commits out of {goodCommits.Length}"
        goodCommits
    
    let benchmark (checkoutsBaseDir : string) (bdnOutputDir : string) (commitHash: string) =
        checkoutAndBuild checkoutsBaseDir commitHash
        let root = revisionDir checkoutsBaseDir commitHash
        let fcsDll = fcsDllPath root
        let fsharpCoreDll = fsharpCoreDllPath root
        
        printfn $"Benchmarking revision {commitHash}, fcsDllPath={fcsDll} fsharpCoreDll={fsharpCoreDll}"
        if File.Exists(fcsDll) |> not then
            failwith $"{fcsDll} doesn't exist"
        if File.Exists(fsharpCoreDll) |> not then
            failwith $"{fsharpCoreDll} doesn't exist"
        let msBuildArgs = Build.MSBuildProps.makeDll fcsDll fsharpCoreDll
        Build.runBenchmark bdnOutputDir msBuildArgs
    
    let findCommit (config : RunConfig) (revision : string) =
        let repo = prepareMainRepo config
        repo.Lookup<Commit>(revision)    
    
    let findCommitDate (config : RunConfig) (revision : string) =
        let commit = findCommit config revision
        commit.Committer.When

[<RequireQualifiedAccess>]
type FCSVersion =
    | Local
    | NuGet of NuGetVersion
    | Git of string
    override this.ToString() =
        match this with
        | Local -> "local"
        | NuGet version -> $"v{version}"
        | Git revision -> $"git_{revision}"
    member this.ShortName() =
        match this with
        | Local -> "local"
        | NuGet version -> $"v{version}"
        | Git revision -> $"git_{revision.Substring(0, 8)}"

type RunResults =
    {
        MeanS : double
        AllocatedMB : double
    }
    
type Run =
    {
        Version : FCSVersion
        VersionTime : DateTime
        Results : RunResults
    }

let getVersionDate (config : RunConfig) (version : FCSVersion) =
    match version with
    | FCSVersion.Local -> DateTime.UtcNow
    | FCSVersion.Git revision -> (Git.findCommitDate config revision).UtcDateTime
    | FCSVersion.NuGet version -> NuGet.findVersionDate version

module Runner =
    let makeChart (results : Run list) =
        let results =
            results
            |> List.sortBy (fun r -> r.VersionTime)
        let x =
            results
            |> List.map (fun r ->
                let time = r.VersionTime.ToString("yyyy-MM-dd")
                $"{r.Version.ShortName()} - {time}"
            )
            
        let getMeanMS {Results = {MeanS = mean}} = Math.Round(mean / 1000000.0, 2)
        let getAllocatedMB { Results = {AllocatedMB = allocated}} = Math.Round(allocated / 1024.0 / 1024.0, 2)
                
        let meanMS = (x, (results |> List.map getMeanMS)) ||> List.zip
        let allocatedMB = (x, (results |> List.map getAllocatedMB)) ||> List.zip
        
        let meanLine = Chart.Line(meanMS, Name="Mean (ms)")
        let allocatedLine = Chart.Line(allocatedMB, Name="Allocated (MB)")

        Chart.combine([meanLine;allocatedLine])

    let exportEntriesAsJson (path : string) (entries : Run list) =
        printfn $"Saving {entries.Length} aggregate results as JSON in {path}"
        File.WriteAllText(path, JsonConvert.SerializeObject(entries, Formatting.Indented))
        
    let exportEntriesAsCsv (path : string) (entries : Run list) =
        printfn $"Saving {entries.Length} aggregate results as CSV in {path}"
        let header = "Version,MeanS,AllocatedMB"
        let csv =
            entries
            |> List.map (fun x -> $"{x.Version},{x.Results.MeanS},{x.Results.AllocatedMB}")
            |> fun rows -> header :: rows
            |> fun lines -> String.Join(Environment.NewLine, lines)
        File.WriteAllText(path, csv)
        
    let bdnBaseDir (config : RunConfig) =
        Path.Combine(config.BDNOutputBaseDir, config.Time.ToString("yyyy-MM-dd_HH-mm"))
    
    let exportEntries (config : RunConfig) (entries : Run list) : GenericChart =
        let dir = Path.Combine(bdnBaseDir config, config.ResultsSuffix)
        Directory.CreateDirectory dir |> ignore
        let pathWithoutExtension = Path.Combine(dir, "results")
        exportEntriesAsJson (pathWithoutExtension + ".json") entries
        exportEntriesAsCsv (pathWithoutExtension + ".csv") entries
        let chart = makeChart entries
        let htmlPath = pathWithoutExtension + ".html"
        printfn $"Saving chart for {entries.Length} results as HTML in {htmlPath}"
        Chart.saveHtml (htmlPath, false) chart
        chart
    
    let getBdnArtifactDir (bdnsBaseDir : string) (version : FCSVersion) =
        Path.Combine(bdnsBaseDir, $"{version}")

    let resultsJsonPath (bdnArtifactDir : string) =
        Path.Combine(bdnArtifactDir, @"results/HistoricalBenchmark.DecentlySizedStandAloneFileBenchmark-report.json")
    
    let readBDNJsonResults (bdnArtifactDir : string) =
        let json = File.ReadAllText(resultsJsonPath bdnArtifactDir)
        JsonConvert.DeserializeObject<JObject>(json)
    
    let extractResultsFromJson (summary : JObject) : RunResults =
        let benchmark = summary["Benchmarks"][0]
        let stats = benchmark["Statistics"]
        let mean = stats["Mean"].ToString() |> Double.Parse
        
        let metrics = benchmark["Metrics"] :?> JArray
        let am =
            let found =
                metrics
                |> Seq.find (fun m -> (m["Descriptor"]["Id"]).ToString() = "Allocated Memory")
            found["Value"].ToString() |> Double.Parse
            
        { MeanS = mean; AllocatedMB = am }
    
    let bdnResultExists (config : RunConfig) (v : FCSVersion) =
        let dir = getBdnArtifactDir (bdnBaseDir config) v
        let jsonReportPath = resultsJsonPath dir
        File.Exists jsonReportPath
    
    let benchmark (config : RunConfig) (version : FCSVersion) =
        let outputDir = getBdnArtifactDir (bdnBaseDir config) version
        if bdnResultExists config version then
            printfn $"Benchmark result for '{version}' already exists in: {outputDir}"
            Ok ()
        else
            printfn $"Benchmarking '{version}' - output dir: {outputDir}"
            try
                match version with
                | FCSVersion.Local ->
                    Local.benchmark outputDir
                | FCSVersion.Git revision ->
                    Git.benchmark config.CheckoutBaseDir outputDir revision
                | FCSVersion.NuGet version ->
                    NuGet.benchmark outputDir version
                printfn $"Benchmarking '{version}' done"
                Ok ()
            with e ->
                printfn $"Failed to benchmark {version}: {e.Message}"
                e.Message|> Error
            
    let loadResultsFromDisk (config : RunConfig) (v : FCSVersion) =
        try
            let bdnDir = getBdnArtifactDir (bdnBaseDir config) v
            {
                Version = v
                VersionTime = getVersionDate config v
                Results = readBDNJsonResults bdnDir |> extractResultsFromJson
            }
            |> Ok
        with e ->
            Error e.Message
    
    /// Prepare version but don't run any benchmarks
    let prepareVersion (config : RunConfig) (version : FCSVersion) =
        printfn $"*** Preparing version {version}"
        match version with
        | FCSVersion.Git revision ->
            async {
                try
                    Git.checkoutAndBuild config.CheckoutBaseDir revision
                with e ->
                    printfn $"*** {nameof(Git.prepareRevisionCheckout)} {revision} failed: {e.Message}"
            }
            |> Some
        | FCSVersion.Local
        | FCSVersion.NuGet _ -> None
    
    let prepareVersions (config : RunConfig) (parallelism : int) (versions : FCSVersion list) =
        printfn $"Preparing {versions.Length} versions"
        versions
        |> List.choose (prepareVersion config)
        |> List.toArray
        |> fun jobs -> Async.Parallel(jobs, parallelism)
        |> Async.RunSynchronously
        |> ignore
        
    let runAll (config : RunConfig) (versions : FCSVersion list) =
        printfn $"*** Preparing and running benchmarks for the following {versions.Length} versions:"
        versions |> List.iter (fun v -> printfn $"- {v}")
        prepareVersions config config.Parallelism versions
        let found, notFound = versions |> List.partition (bdnResultExists config)
        printfn $"*** {found.Length}/{versions.Length} versions have existing benchmark results - will only run benchmarks for the remaining {notFound.Length}"
        let results =
            versions
            |> List.mapi (fun i v ->
                printfn $"*** Benchmark {v} [{i+1}/{versions.Length}]"
                v, benchmark config v
            )
            |> List.choose (function (v, Ok()) -> Some v | _ -> None)
            |> List.map (loadResultsFromDisk config)
            |> List.choose (function Ok run -> Some run | _ -> None)
        printfn $"*** After running benchmarks {results.Length} reports are now available - exporting to files and showing a chart."
        exportEntries config results
