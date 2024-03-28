module MicroPerf.Program

open System
open System.Reflection
open BenchmarkDotNet.Configs
open BenchmarkDotNet.Jobs
open BenchmarkDotNet.Running

let config =
    DefaultConfig.Instance
        .AddJob(Job.Default.WithId("Current").WithArguments([|MsBuildArgument "/p:BUILDING_USING_DOTNET=true"|]).AsBaseline())
        .AddJob(Job.Default.WithId("Preview").WithArguments([|MsBuildArgument "/p:BUILDING_USING_DOTNET=true"|]).WithCustomBuildConfiguration "Preview")
        .WithOptions(ConfigOptions.JoinSummary)
        .HideColumns("BuildConfiguration")

ignore (BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(Environment.GetCommandLineArgs(), config))
