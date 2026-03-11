module ILGenCodegen.Program

open System
open System.Reflection
open BenchmarkDotNet.Configs
open BenchmarkDotNet.Jobs
open BenchmarkDotNet.Running

let config =
    DefaultConfig.Instance
        .AddJob(
            Job.Default
                .WithId("SdkCompiler")
                .WithArguments([| MsBuildArgument "/p:BUILDING_USING_DOTNET=true" |])
                .AsBaseline())
        .AddJob(
            Job.Default
                .WithId("LocalCompiler")
                .WithArguments([| MsBuildArgument "/p:BUILDING_USING_DOTNET=true" |])
                .WithCustomBuildConfiguration "LocalCompiler")
        .WithOptions(ConfigOptions.JoinSummary)
        .HideColumns("BuildConfiguration")

ignore (
    BenchmarkSwitcher
        .FromAssembly(Assembly.GetExecutingAssembly())
        .Run(Environment.GetCommandLineArgs(), config)
)
