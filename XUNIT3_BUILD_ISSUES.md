# xUnit3 Migration - Build Issues Tracking

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
**Fix Applied**: Removed custom Program.fs and let xUnit3 generate entry point automatically

### 5. FSharp.Editor.Tests - OutputType ✅ RESOLVED
**Error**: `FS0988: Main module of program is empty`
**Fix Applied**: Changed back to `<OutputType>Library</OutputType>` (test library, not executable)

### 6. CI Runtime Installation ✅ RESOLVED
**Error**: .NET 10 RC not found on Linux/macOS CI
**Fix Applied**: Added UseDotNet@2 task to azure-pipelines-PR.yml for runtime installation

### 7. TestConsole Initialization ✅ RESOLVED
**Error**: MailboxProcessor race condition tests crashing test host
**Root Cause**: Without the custom `FSharpXunitFramework`, `TestConsole.install()` was never being called. This caused issues with test execution since the console redirection infrastructure was not initialized.
**Fix Applied**: Added static initialization to `NotThreadSafeResourceCollection` class and `XUnitSetup` module to ensure `TestConsole.install()` is called before tests run.

### 8. German Culture Leak in TransparentCompiler CI Leg ✅ RESOLVED
**Error**: Tests expecting English error messages received German messages instead (e.g., "Dieser Ausdruck sollte den folgenden Typ aufweisen" instead of "This expression was expected to have type")
**Affected CI Leg**: Only `WindowsCompressedMetadata transparent_compiler_release`
**Root Cause**: `PreferredUiLangTests` runs FSI sessions with `--preferreduilang:de-DE`, which sets both `Thread.CurrentThread.CurrentUICulture` and the static `GraphNode.culture` variable to German. Neither was restored after the FSI session completed. The bug only manifested on the `transparent_compiler_release` leg because:
- **BackgroundCompiler** (used without `TEST_TRANSPARENT_COMPILER`) calls `GraphNode.SetPreferredUILang(tcConfig.preferredUiLang)` during `CheckOneFile`, which resets culture based on project options (typically `--preferreduilang:en-US`), masking the leak
- **TransparentCompiler** (used with `TEST_TRANSPARENT_COMPILER=1`) does NOT call `SetPreferredUILang`, so the German culture persists permanently in `GraphNode.culture` and affects all subsequent async computations

**Fix Applied**: 
- `CompilerAssert.RunScriptWithOptionsAndReturnResult`: Added try/finally to save/restore both `CurrentUICulture` and `GraphNode.culture`
- `ScriptHelpers.FSharpScript.Eval`: Added save/restore for `CurrentUICulture`
- `BuildGraph.fsi`: Exposed `GraphNode.culture` mutable field to allow test utilities to save/restore it