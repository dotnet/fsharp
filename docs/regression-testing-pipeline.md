# F# Compiler Regression Testing

This document describes the F# compiler regression testing functionality implemented as a reusable Azure DevOps template in `eng/templates/regression-test-jobs.yml` and integrated into the main PR pipeline (`azure-pipelines-PR.yml`).

## Purpose

The regression testing helps catch F# compiler regressions by building popular third-party F# libraries with the freshly built compiler from this repository. This provides early detection of breaking changes that might affect real-world F# projects.

## How It Works

### Integration with PR Pipeline

The regression tests are automatically run as part of every PR build, depending on the `EndToEndBuildTests` job for the F# compiler artifacts.

### Template-Based Architecture

The regression testing logic is implemented as a reusable Azure DevOps template that can be consumed by multiple pipelines:

- **Template Location**: `eng/templates/regression-test-jobs.yml`
- **Integration**: Called from `azure-pipelines-PR.yml` 
- **Dependencies**: Depends on `EndToEndBuildTests` job for compiler artifacts

### Workflow

1. **Build F# Compiler**: The `EndToEndBuildTests` job builds the F# compiler and publishes required artifacts
2. **Matrix Execution**: For each library in the test matrix (running in parallel):
   - Checkout the third-party repository at a specific commit
   - Pin `global.json` to the exact SDK that built the local compiler
   - Inject `UseLocalCompiler.Directory.Build.props` via `CustomAfterDirectoryBuildProps`
   - Build the library using its standard build script
   - Publish MSBuild binary logs for analysis
3. **Report Results**: Success/failure status is reported with build logs for diagnosis

### Key Features

- **Reproducible Testing**: Uses specific commit SHAs for third-party libraries to ensure consistent results
- **Matrix Configuration**: Supports testing multiple libraries with different build requirements
- **Detailed Logging**: Captures comprehensive build logs, binary logs, and environment information
- **Artifact Publishing**: Publishes build outputs for analysis when builds fail

## Current Test Matrix

The pipeline currently tests against:

| Library | Repository | Commit | Build Script | Purpose |
|---------|------------|--------|--------------|---------|
| FSharpPlus | fsprojects/FSharpPlus | f614035b75922aba41ed6a36c2fc986a2171d2b8 | build.cmd | Tests advanced F# language features |

## Adding New Libraries

To add a new library to the test matrix, update the template invocation in `azure-pipelines-PR.yml`:

```yaml
# F# Compiler Regression Tests using third-party libraries
- template: /eng/templates/regression-test-jobs.yml
  parameters:
    testMatrix:
    - repo: fsprojects/FSharpPlus
      commit: f614035b75922aba41ed6a36c2fc986a2171d2b8
      buildScript: build.cmd
      displayName: FSharpPlus
    - repo: your-org/your-library    # Add your library here
      commit: abc123def456...         # Specific commit SHA
      buildScript: build.sh           # Build script (build.cmd, build.sh, etc.)
      displayName: YourLibrary        # Human-readable name
```

Each test matrix entry requires:
- **repo**: GitHub repository in `owner/name` format
- **commit**: Specific commit SHA for reproducible results
- **buildScript**: Build command to execute — a `dotnet ...` command or a script file (`build.cmd`/`build.sh`); `;;` separates commands run sequentially, fail-fast
- **displayName**: Human-readable name for the job
- **expectLocalCore** (optional): set `true` when the repo has projects that take the implicit `FSharp.Core`; the job then fails unless the locally built FSharp.Core is actually restored — a tripwire for a silently broken shim

## Pipeline Configuration

### Triggers

Regression tests run automatically as part of PR builds when:
- **PR Pipeline**: Triggered by pull requests to main branches  
- **Dependencies**: Runs after `EndToEndBuildTests` completes successfully
- **Parallel Execution**: Each repository in the test matrix runs as a separate job in parallel

### Build Environment

- **OS**: Windows by default (`$(WindowsMachineQueueName)`); matrix entries can override to Linux. The scripts are OS-agnostic
- **Pool**: Standard public build pool (`$(DncEngPublicBuildPool)`)
- **Timeout**: 120 minutes per regression test job
- **.NET SDK**: Each test repo's `global.json` is pinned to the SDK that built the local compiler, so `fsc.dll` and its host runtime line up

### Artifacts

The regression tests publish focused artifacts for analysis:
- **FSharpCompilerArtifacts**: F# compiler build output (from `EndToEndBuildTests`)
- **UseLocalCompilerProps**: Configuration file for using local compiler (from `EndToEndBuildTests`)
- **{LibraryName}_BinaryLogs**: MSBuild binary logs from each tested library for efficient diagnosis

## Troubleshooting Build Failures

When a regression test fails:

1. **Check the Job Summary**: Look at the final status report for high-level information.

2. **Download Build Logs**: Download the published artifacts to examine detailed build output.

3. **Compare Compiler Changes**: Review what changes were made to the compiler that might affect the failing library.

4. **Local Reproduction**: Use the `UseLocalCompiler.Directory.Build.props` file to reproduce the issue locally.

### Local Testing

To reproduce a regression locally, on any OS, without editing the library:

1. Build the compiler and pack FSharp.Core in your `dotnet/fsharp` checkout: `./build.sh -c Release -pack` (`Build.cmd` on Windows).
2. Clone the library at the failing commit and build it against your local build:
   ```
   git clone --recursive https://github.com/<owner>/<repo>.git TestRepo
   cd TestRepo && git checkout <commit>
   # If TestRepo's global.json pins a different SDK, align sdk.version with <fsharp-repo>/global.json
   # (allowPrerelease: true, rollForward: disable) — the clone is disposable, as in CI.
   dotnet fsi <fsharp-repo>/eng/scripts/BuildWithLocalFSharp.fsx --build-script '<the repo build command>'
   ```

`BuildWithLocalFSharp.fsx` runs the same command CI runs, from the current directory, without touching the repo's sources. Add `--verify` to fail unless every project consumes the local FSharp.Core; the script header lists the other options. Because the local package keeps a fixed `-dev` version, the script evicts it from the global NuGet cache before each run so a rebuild is never served stale; pass `--nuget-packages <dir>` to use an isolated cache when running several builds concurrently or against a repo that redirects its packages folder.

## Best Practices

### For Library Selection

- **Coverage**: Choose libraries that exercise different F# language features
- **Popularity**: Include widely-used libraries that represent real-world usage
- **Stability**: Use libraries with stable build processes and minimal external dependencies
- **Diversity**: Include libraries with different build systems and target frameworks

### For Maintenance

- **Regular Updates**: Periodically update commit SHAs to newer stable versions
- **Monitor Dependencies**: Watch for changes in third-party library build requirements
- **Baseline Management**: Update baselines when intentional breaking changes are made

## Technical Details

### UseLocalCompiler.Directory.Build.props

This MSBuild props file redirects projects to the locally built F# compiler (and, for the matrix, the locally built FSharp.Core) instead of the SDK version. It is organised into gates so it can be injected into unmodified repos as well as imported directly by in-repo tests — see the `Gate 1/2/3` comments in the file. Its companion `UseLocalCompiler.Directory.Build.targets` is injected via `CustomAfterDirectoryBuildTargets` (after the target repo's project body) so the local FSharp.Core version wins over the repo's own reference, whether implicit, an explicit `PackageReference Include`, a `PackageReference Update`, or a central `PackageVersion` (Central Package Management).

## Future Enhancements

Potential improvements to the pipeline:

1. **Performance Testing**: Measure compilation times and memory usage
2. **Multiple Target Frameworks**: Test libraries across different .NET versions
3. **Parallel Execution**: Run library tests in parallel for faster feedback
4. **Automatic Bisection**: Automatically identify which commit introduced a regression
5. **Integration with GitHub**: Post regression test results as PR comments
