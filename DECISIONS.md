# Design Decisions

This document records significant design decisions made during the implementation of the F# Compiler Regression Testing pipeline.

## Decision 1: Template-Based Architecture

**Context**: Need to implement regression testing that can be reused across pipelines.

**Options Considered**:
1. Inline job definitions in azure-pipelines-PR.yml
2. Reusable Azure DevOps template in eng/templates/

**Decision**: Use reusable template approach.

**Rationale**:
- Follows Azure DevOps best practices
- Reduces code duplication
- Makes it easy to extend with new libraries
- Consistent with existing patterns in the repository (eng/common/templates/)

---

## Decision 2: Optimized Artifact Publishing

**Context**: Need to share compiler artifacts between jobs.

**Options Considered**:
1. Publish entire artifacts folder (~1.8GB)
2. Publish only essential directories (fsc and FSharp.Core) (~79MB)

**Decision**: Publish only essential directories.

**Rationale**:
- Reduces artifact size from 1.8GB to ~79MB
- Faster artifact upload/download
- Contains all necessary components for regression testing
- Matches approach in previous PR #18803

---

## Decision 3: Using F# Script for Directory.Build.props Setup

**Context**: Need to inject UseLocalCompiler.Directory.Build.props import into third-party repos.

**Options Considered**:
1. Pure PowerShell XML manipulation
2. F# script with proper XML handling
3. Simple file replacement

**Decision**: Use F# script with XML handling.

**Rationale**:
- Properly handles existing Directory.Build.props files
- Correctly inserts import at beginning of Project element
- Native F# tooling in an F# project
- Matches approach in previous PR #18803

---

## Decision 4: Specific Commit SHAs for Third-Party Libraries

**Context**: Need reproducible regression tests.

**Options Considered**:
1. Use main/master branch
2. Use specific commit SHAs

**Decision**: Use specific commit SHAs.

**Rationale**:
- Ensures reproducible test results
- Protects against breaking changes in third-party libraries
- Allows controlled updates when ready
- Standard practice for regression testing

---

## Decision 5: Removal of Strategy Matrix from EndToEndBuildTests

**Context**: The original EndToEndBuildTests job had a matrix for regular vs experimental features.

**Options Considered**:
1. Keep the matrix and publish artifacts only from one configuration
2. Remove the matrix entirely
3. Publish artifacts from both configurations

**Decision**: Remove the matrix entirely (per previous PR approach).

**Rationale**:
- Simplifies artifact publishing
- Regression tests need consistent baseline
- Both configurations were building with empty experimental flag anyway
- Matches approach in previous PR #18803

---

## Decision 6: Use net10.0 Target Framework in Template

**Context**: The pipeline needs to reference the correct .NET target framework for the compiler artifacts.

**Options Considered**:
1. Hardcode net9.0 (as in PR #18803)
2. Use net10.0 (per current UseLocalCompiler.Directory.Build.props)
3. Make it configurable

**Decision**: Use net10.0 to match the current project state.

**Rationale**:
- The current repository uses .NET 10 SDK
- UseLocalCompiler.Directory.Build.props references net10.0 paths
- The artifacts/bin/fsc/Release/net10.0 directory exists
- PR #18803 used net9.0 because it was based on an older version of the repository
- Using net10.0 ensures compatibility with the current build output

---
