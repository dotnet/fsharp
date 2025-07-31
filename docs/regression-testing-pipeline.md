# F# Compiler Regression Testing Pipeline

This document describes the F# compiler regression testing pipeline implemented in `azure-pipelines-regression-test.yml`.

## Purpose

The regression testing pipeline helps catch F# compiler regressions by building popular third-party F# libraries with the freshly built compiler from this repository. This provides early detection of breaking changes that might affect real-world F# projects.

## How It Works

### Pipeline Workflow

1. **Build F# Compiler**: The pipeline first builds the F# compiler and packages from the current source code using the standard build process.

2. **Test Against Third-Party Libraries**: For each library in the test matrix:
   - Checkout the third-party repository at a specific commit
   - Inject the `UseLocalCompiler.Directory.Build.props` configuration to use the locally built compiler
   - Run the library's build script
   - Capture detailed build logs and artifacts

3. **Report Results**: Success/failure status is reported with detailed logs for diagnosis.

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

To add a new library to the test matrix:

1. **Choose a Library**: Select a representative F# library that uses features you want to test.

2. **Find a Stable Commit**: Choose a specific commit SHA that is known to build successfully with the current F# compiler.

3. **Update the Matrix**: Add an entry to the `testMatrix` parameter in the pipeline:

```yaml
parameters:
- name: testMatrix
  type: object
  default:
  - repo: fsprojects/FSharpPlus
    commit: f614035b75922aba41ed6a36c2fc986a2171d2b8
    buildScript: build.cmd
    displayName: FSharpPlus
  - repo: your-org/your-library    # Add your library here
    commit: abc123def456...         # Specific commit SHA
    buildScript: build.sh           # Build script (build.cmd, build.sh, etc.)
    displayName: YourLibrary        # Human-readable name
```

4. **Test the Configuration**: Verify that your library builds correctly with the current compiler before adding it to the matrix.

## Pipeline Configuration

### Triggers

The pipeline is triggered by:
- **Branches**: main, release/*, feature/*
- **Paths**: Changes to compiler source code (src/Compiler/, src/fsc/, src/FSharp.Core/, src/FSharp.Build/)
- **Exclusions**: Documentation and non-compiler changes

### Build Environment

- **OS**: Windows (windows.vs2022.amd64.open)
- **Pool**: Uses the standard public build pool (`DncEngPublicBuildPool`)
- **Timeout**: 60 minutes per job
- **.NET SDK**: Automatically installs the required preview SDK

### Artifacts

The pipeline publishes several artifacts for analysis:
- **FSharpCompilerArtifacts**: Complete F# compiler build output
- **UseLocalCompilerProps**: Configuration file for using local compiler
- **{Library}_BuildOutput**: Complete build output from each tested library
- **{Library}_BinaryLogs**: MSBuild binary logs when available

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