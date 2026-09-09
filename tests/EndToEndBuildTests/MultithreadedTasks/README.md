# SDK multithreaded task integration

Use the pinned repository SDK, not Visual Studio MSBuild or a system `dotnet`.
The host must expose `IMultiThreadableTask`, `TaskEnvironment`, and `ToolTask.TaskEnvironment`.
The validated SDK is `11.0.100-rc.1.26420.103`, with runtime MSBuild `18.11.0.42103`.
The `Microsoft.Build.*` package version (`18.12.0-1.26454.5`) is not the runtime host version.
This does not establish support for all MSBuild 18.x hosts.
The production tasks retain their parameterless constructors and existing base types.

Build the local products:

```sh
./build.sh -c Release --mt true
```

On Windows, use `.\build.cmd -configuration Release -noVisualStudio`.

Run the focused task tests:

```sh
./eng/common/dotnet.sh test --project tests/FSharp.Build.UnitTests/FSharp.Build.UnitTests.fsproj -c Release --report-spekt-xunit --report-spekt-xunit-filename FSharp.Build.UnitTests.Linux-MT.xml --results-directory artifacts/TestResults/Release
```

Run the SDK E2E from the repository root:

```sh
./eng/common/dotnet.sh fsi tests/EndToEndBuildTests/MultithreadedTasks/run.fsx -- --configuration Release --repetitions 3
```

On Windows, replace `./eng/common/dotnet.sh` with `.\eng\common\dotnet.cmd`.
CI runs this harness as required steps in the existing `Linux` and Windows `EndToEndBuildTests` jobs.
Both reuse their built products and publish the SDK evidence. There are no separate MT jobs.
The existing Linux and macOS build jobs use Arcade's `--mt true` option.
The three `Plain_Build_*` SDK jobs pass `-mt` directly to MSBuild.
VS/MSBuild.exe jobs and Arcade-managed source-build configuration are unchanged.
No repository-wide environment override or replacement for Arcade's MT controls is added.

Use `--repetitions 10` for local stress runs.
Each repetition compares `/m:4 -mt:false` and `/m:4 -mt` builds of 16 independent SDK F# executables.
The child processes clear `MSBUILDFORCEMULTITHREADED` so an enclosing CI setting cannot override the MP control.
An inherited `MSBUILDENABLEMULTITHREADED` default is overridden by the explicit mode switch.
Each clean build is followed by an unchanged incremental build. Fsi validation reruns, but Fsc must not execute.
The minimum project count is four.
The harness needs access to the repository NuGet feeds for self-contained runtime and ILLink packs.
It adds no package references.

The fixture uses relative source, resource, intermediate, and output paths from an unrelated invocation directory.
It redirects the tasks, targets, Fsc, Fsi, and FSharp.Core to local products.
It loads `artifacts/bin/FSharp.Build/<configuration>/netstandard2.0/FSharp.Build.dll` directly, not the compiler directory's potentially stale copy.
It checks these nine tasks: Fsc, Fsi, WriteCodeFragment, FSharpEmbedResourceText, FSharpEmbedResXSource, CreateFSharpManifestResourceName, MapSourceRoots, GenerateILLinkSubstitutions, and SubstituteText.

Evidence includes:

- Exact task assembly paths from task-start events, loaded-assembly checks, and the assembly SHA-256.
- Runtime API checks and assembly paths/file versions from each execution host, not the NuGet reference assemblies.
- Four live Fsi processes at a barrier, overlapping Fsc intervals, and project-specific environment and resource assertions.
- One MT project-process PID matching the SDK routing caller, versus multiple MP PIDs.
- Mandatory out-of-process routing diagnostics for an unmarked control task, with none for the nine migrated tasks.
- Matching artifact hashes and executed resource checks across clean and incremental builds, with zero incremental Fsc executions.
- Expected malformed-resource and type errors, with baseline-identical, runnable sibling projects.
- Self-contained `PublishTrimmed` builds in both modes, with substitutions enabled and disabled.
  Enabled substitutions must remove all F# metadata resources. The `DisableILLinkSubstitutions=true` controls must retain the original metadata.
  All four published executables must pass resource checks.

Generated ILLink XML alone is not evidence of trimming. The publish checks establish actual ILLink consumption.
A trimming failure fails the harness, even when the generated XML and build hashes match.
Missing concurrency, task-loading, or routing evidence fails the run.
Logs, binlogs, event records, generated fixtures, and hash manifests remain under `artifacts/MultithreadedTasks/<timestamp>/`.

The Windows SDK E2E step uses the SDK host. This harness does not start Visual Studio or exercise its HostObject integration.
