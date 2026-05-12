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
   - Install appropriate .NET SDK version using the repository's `global.json`
   - Setup `Directory.Build.props` to import `UseLocalCompiler.Directory.Build.props`
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
- **buildScript**: Build script to execute (e.g., `build.cmd`, `build.sh`)
- **displayName**: Human-readable name for the job

## Pipeline Configuration

### Triggers

Regression tests run automatically as part of PR builds when:
- **PR Pipeline**: Triggered by pull requests to main branches  
- **Dependencies**: Runs after `EndToEndBuildTests` completes successfully
- **Parallel Execution**: Each repository in the test matrix runs as a separate job in parallel

### Build Environment

- **OS**: Windows (using `$(WindowsMachineQueueName)`)
- **Pool**: Standard public build pool (`$(DncEngPublicBuildPool)`)
- **Timeout**: 60 minutes per regression test job
- **.NET SDK**: Automatically detects and installs SDK version from each repository's `global.json`

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

To test a library locally with your F# compiler build:

1. Build the F# compiler: `.\Build.cmd -c Release -pack`

2. In the third-party library directory, create a `Directory.Build.props`:
   ```xml
   <Project>
     <Import Project="path/to/UseLocalCompiler.Directory.Build.props" />
   </Project>
   ```

3. Update the `LocalFSharpCompilerPath` in `UseLocalCompiler.Directory.Build.props` to point to your F# repository.

4. Set environment variables:
   ```cmd
   set LoadLocalFSharpBuild=true
   set LocalFSharpCompilerConfiguration=Release
   ```

5. Run the library's build script.

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

This MSBuild props file configures projects to use the locally built F# compiler instead of the SDK version. Key settings:

- `LocalFSharpCompilerPath`: Points to the F# compiler artifacts
- `DotnetFscCompilerPath`: Path to the fsc.dll compiler
- `DisableImplicitFSharpCoreReference`: Ensures local FSharp.Core is used

### Path Handling

The pipeline dynamically updates paths in the props file using PowerShell:
```powershell
$content -replace 'LocalFSharpCompilerPath.*MSBuildThisFileDirectory.*', 'LocalFSharpCompilerPath>$(Pipeline.Workspace)/FSharpCompiler<'
```

This ensures the correct path is used in the Azure DevOps environment.

## Future Enhancements

Potential improvements to the pipeline:

1. **Performance Testing**: Measure compilation times and memory usage
2. **Multiple Target Frameworks**: Test libraries across different .NET versions
3. **Parallel Execution**: Run library tests in parallel for faster feedback
4. **Automatic Bisection**: Automatically identify which commit introduced a regression
5. **Integration with GitHub**: Post regression test results as PR comments
