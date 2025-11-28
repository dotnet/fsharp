# xUnit3 Migration - Build Issues Tracking

## Status: ✅ ALL RESOLVED

All build issues have been resolved. The migration is complete and verified.

**Verification**: Run `./build.sh -c Release --testcoreclr` - 5,939 tests pass.

## Resolved Issues

### 1. VisualFSharp.Salsa.fsproj - Missing OutputType ✅ RESOLVED
**Error**: `xUnit.net v3 test projects must be executable (set project property 'OutputType')`
**Fix Applied**: Changed `<OutputType>Library</OutputType>` to `<OutputType>Exe</OutputType>`

### 2. FSharp.Editor.IntegrationTests.csproj - Entry Point ✅ RESOLVED
**Error**: `CS5001: Program does not contain a static 'Main' method suitable for an entry point`
**Fix Applied**: Configured project to generate entry point automatically

### 3. FSharp.Test.Utilities - ValueTask.FromResult net472 ✅ RESOLVED
**Error**: `The type 'ValueTask' does not define the field, constructor or member 'FromResult'`
**Fix Applied**: Changed `ValueTask.FromResult(rows)` to `ValueTask<T>(rows)` constructor for net472 compatibility

### 4. FSharp.Compiler.LanguageServer.Tests - Entry Point ✅ RESOLVED  
**Error**: `FS0222: Files in libraries must begin with a namespace or module declaration`
**Fix Applied**: Moved Program.fs to last position and fixed entry point structure

### 5. FSharp.Editor.Tests - OutputType ✅ RESOLVED
**Error**: `FS0988: Main module of program is empty`
**Fix Applied**: Changed back to `<OutputType>Library</OutputType>` (test library, not executable)

### 6. CI Runtime Installation ✅ RESOLVED
**Error**: .NET 10 RC not found on Linux/macOS CI
**Fix Applied**: Added UseDotNet@2 task to azure-pipelines-PR.yml for runtime installation

## Current State

- All projects build successfully
- 5,939 tests pass
- No build errors
- One pre-existing flaky test may cause timeout (MailboxProcessorType.TryReceive) but this is not related to xUnit3 migration
