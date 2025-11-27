# TODO: Azure DevOps Template for F# Compiler Regression Testing

## Overview
Implement a reusable Azure DevOps template for F# compiler regression testing, integrated with the existing PR pipeline infrastructure.

## Tasks

### Infrastructure Setup
- [x] Analyze existing PR pipeline structure (`azure-pipelines-PR.yml`)
- [x] Review previous PR #18803 implementation details
- [x] Understand `UseLocalCompiler.Directory.Build.props` configuration

### Implementation
- [x] Create `eng/templates/` directory
- [x] Create `eng/templates/regression-test-jobs.yml` template
  - [x] Define parameters for testMatrix
  - [x] Implement job that depends on EndToEndBuildTests
  - [x] Add artifact download steps (FSharpCompilerFscArtifacts, FSharpCoreArtifacts, UseLocalCompilerProps)
  - [x] Add third-party repo checkout step
  - [x] Add .NET SDK installation step
  - [x] Add Directory.Build.props setup step referencing standalone F# script
  - [x] Add environment reporting step
  - [x] Add build execution step
  - [x] Add artifact publishing step
  - [x] Add result reporting step
  - [x] Add optional imageOverride per tested repo

### F# Script for Repository Setup
- [x] Create standalone `eng/scripts/PrepareRepoForRegressionTesting.fsx`
- [x] Test script locally with FSharpPlus repository
- [x] Handle both existing and missing Directory.Build.props cases

### Integration
- [x] Update `azure-pipelines-PR.yml`:
  - [x] Modify EndToEndBuildTests to publish focused artifacts
  - [x] Remove strategy/matrix section from EndToEndBuildTests
  - [x] Add artifact publishing tasks for fsc, FSharp.Core, and UseLocalCompiler props
  - [x] Move template invocation to stage level (outside common template)
  - [x] Add template invocation with FSharpPlus test matrix

### Documentation
- [x] Create `docs/regression-testing-pipeline.md`
  - [x] Purpose and overview
  - [x] How it works
  - [x] Current test matrix
  - [x] Adding new libraries
  - [x] Pipeline configuration
  - [x] Troubleshooting
  - [x] Technical details

### Validation
- [x] Verify YAML syntax is valid
- [x] Verify template structure matches Azure DevOps best practices
- [x] Ensure Release configuration is used throughout
- [x] Test F# script locally with FSharpPlus

## References
- Previous PR: https://github.com/dotnet/fsharp/pull/18803
- Files: `eng/templates/regression-test-jobs.yml`, `azure-pipelines-PR.yml`, `docs/regression-testing-pipeline.md`, `eng/scripts/PrepareRepoForRegressionTesting.fsx`
