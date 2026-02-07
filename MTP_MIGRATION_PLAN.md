# Microsoft.Testing.Platform Migration Plan

**Date:** January 21, 2026  
**Status:** In Progress
**Target Completion:** TBD

## Executive Summary

This document outlines the plan to migrate the F# compiler repository from VSTest to Microsoft.Testing.Platform (MTP) for test execution. The migration will improve test performance, reduce dependencies, and align with the modern .NET testing infrastructure.

## Key Facts from xUnit.net v3 MTP Documentation

1. **Built-in Support:** xUnit.net v3 has native MTP support. No additional packages needed beyond `xunit.v3`.

2. **VSTest Compatibility:** `xunit.runner.visualstudio` and `Microsoft.NET.Test.Sdk` do NOT interfere with MTP. Keep them for backward compatibility until all environments support MTP.

3. **Two Modes:**
   - **Command-line mode:** Enabled via `<UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>`
   - **dotnet test mode:** Enabled via `global.json` test runner setting (.NET 10+) or `<TestingPlatformDotnetTestSupport>` (.NET 8/9)

4. **Report Naming:** MTP doesn't support placeholders in filenames. Reports go to `TestResults/` directory with auto-generated names unless specified (filename only, no path).

5. **No Separator in .NET 10:** Unlike .NET 8/9, .NET 10+ doesn't require `--` separator before MTP arguments.

6. **xUnit-Specific TRX:** Use `--report-xunit-trx` (not `--report-trx`) for xUnit-flavored TRX files.

## Current State

### Test Infrastructure
- **Framework:** xUnit v3.1.0
- **Runner:** VSTest via `xunit.runner.visualstudio` v3.1.4
- **SDK Version:** .NET 10.0.100-rc.2
- **Test Projects:** 9 main test projects (`.Tests`, `.ComponentTests`, `.UnitTests`)
- **Project Type:** `<OutputType>Exe</OutputType>` (xUnit3 requirement - already met ✅)
- **Total Tests:** ~13,000+ (CoreCLR + Desktop frameworks)

### Package References (tests/Directory.Build.props)
```xml
<PackageReference Include="xunit.v3" Version="$(XunitVersion)" />
<PackageReference Include="xunit.v3.runner.console" Version="$(XunitRunnerConsoleVersion)" />
<!-- MTP extension for hang detection -->
<PackageReference Include="Microsoft.Testing.Extensions.HangDump" Version="$(MicrosoftTestingExtensionsHangDumpVersion)" />
```

**Note:** xUnit v3 includes Microsoft.Testing.Platform (MTP) as a transitive dependency. No explicit MTP package reference is needed. The `Microsoft.TestPlatform` package is the older VSTest package, not MTP.

### Build Scripts
- **Windows:** `eng/Build.ps1` - `TestUsingMSBuild` function
- **Unix:** `eng/build.sh` - `Test` function
- **CI:** Azure Pipelines (azure-pipelines-PR.yml)

## Benefits of Migration

### Performance
- **Faster Startup:** No external runner process overhead
- **Faster Discovery:** Native framework integration
- **Faster Execution:** Direct test execution without VSTest protocol translation
- **Estimated Improvement:** 20-40% faster test runs (based on community reports)

### Modern Features
- **Better Diagnostics:** Improved error messages and test output with clearer formatting
- **Extensibility:** Easier to add custom test extensions and reporters via MTP extension system
- **Protocol:** Modern JSON-RPC instead of legacy JSON
- **Multiple Report Formats:** Built-in support for TRX, xUnit XML, JUnit XML, NUnit XML, HTML, and CTRF JSON

### Simplification
- **Fewer Dependencies:** Remove `xunit.runner.visualstudio` and potentially `Microsoft.NET.Test.Sdk`
- **Portable Tests:** Test assemblies can be run directly as executables
- **Cross-Platform:** Better consistency across Windows, Linux, and macOS

### Future-Proofing
- **Active Development:** MTP is the future of .NET testing
- **Better Support:** New features will target MTP first
- **Community Adoption:** Major frameworks (xUnit, NUnit, MSTest) all support MTP

## Migration Phases

### Phase 1: Preparation & Validation (2 weeks)

#### Prerequisites
- [x] Verify .NET 10 SDK is used (currently: 10.0.100-rc.2 ✅)
- [x] Verify xUnit v3 is used (currently: 3.1.0 ✅)
- [x] Verify `<OutputType>Exe</OutputType>` (already set ✅)
- [ ] Review all test projects for compatibility
- [ ] Document current test execution times (baseline metrics)
- [ ] Verify all CI/CD environments support MTP

#### Tasks
1. **Audit Test Projects**
   - List all test projects and their configurations
   - Identify any custom test adapters or extensions
   - Check for any VSTest-specific dependencies in test code

2. **Environment Validation**
   - Test MTP support in Azure Pipelines agents
   - Test MTP support in local developer environments
   - Test MTP support in VS 2022 and VS Code

3. **Create Test Branch**
   - Create `feature/mtp-migration` branch
   - Set up parallel CI runs (VSTest vs MTP comparison)

### Phase 2: Pilot Migration (1-2 weeks)

#### Select Pilot Project
Choose a small, stable test project for initial migration:
- **Recommended:** `FSharp.Build.UnitTests` (smallest, least dependencies)
- **Alternative:** `FSharp.Core.UnitTests` (critical but well-isolated)

#### Configuration Changes

**Option A: For command-line MTP experience only**

Add to PropertyGroup in test project file:
```xml
<PropertyGroup>
  <UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>
</PropertyGroup>
```

This enables MTP when running `dotnet run` but keeps VSTest for `dotnet test`.

**Option B: For `dotnet test` with MTP (requires Option A)**

Since the repository uses .NET 10 SDK, add to `global.json`:
```json
{
  "sdk": { ... },
  "test": {
    "runner": "Microsoft.Testing.Platform"
  },
  ...
}
```

**Note:** For .NET SDK 8/9 (not applicable here), you would use:
```xml
<TestingPlatformDotnetTestSupport>true</TestingPlatformDotnetTestSupport>
```

#### Package Updates

**Important:** Keep these packages during migration for backward compatibility:
```xml
<!-- Keep for VSTest fallback during migration -->
<PackageReference Include="xunit.runner.visualstudio" Version="$(XunitRunnerVisualStudioVersion)" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="$(MicrosoftNETTestSdkVersion)" />
```

These allow VSTest to work in environments that don't support MTP yet. They do not interfere with MTP when enabled.

**Note:** xUnit.net v3 has built-in MTP support. No additional packages are needed beyond `xunit.v3` which is already referenced.

#### Build Script Updates

**File:** `eng/Build.ps1`

With .NET 10 and MTP configured in `global.json`, `dotnet test` automatically uses MTP. The build script changes are minimal:

```powershell
function TestUsingMSBuild([string] $testProject, [string] $targetFramework, [string] $settings = "") {
    # ... existing code ...
    
    # MTP report options (when global.json has MTP enabled)
    # No -- separator needed in .NET 10+
    $testLogPath = "$ArtifactsDir\TestResults\$configuration\${projectName}_${targetFramework}$testBatchSuffix.trx"
    
    $args = "test $testProject -c $configuration -f $targetFramework"
    $args += " --report-xunit-trx --report-xunit-trx-filename $testLogPath"
    
    # ... rest of function ...
}
```

**Key Changes:**
- Replace `--logger "trx;LogFileName=..."` with `--report-xunit-trx --report-xunit-trx-filename ...`
- No `--` separator needed in .NET 10+
- Placeholders like `{assembly}` and `{framework}` work natively with MTP (no manual expansion needed)

#### Validation Criteria
- [ ] All pilot project tests pass
- [ ] Test execution time is same or better
- [ ] Test logs are generated correctly
- [ ] CI integration works
- [ ] No regressions in test output or reporting

### Phase 3: Incremental Rollout (3-4 weeks)

#### Migration Order (by risk/complexity)

**Week 1: Small/Stable Projects**
1. FSharp.Build.UnitTests ✅ (pilot)
2. FSharp.Compiler.Private.Scripting.UnitTests
3. FSharp.Core.UnitTests

**Week 2: Medium Projects**
4. FSharp.Compiler.Service.Tests
5. FSharpSuite.Tests

**Week 3: Large/Complex Projects**
6. FSharp.Compiler.ComponentTests (largest, most tests)
7. FSharp.Compiler.LanguageServer.Tests

**Week 4: Special Cases**
8. End-to-end test projects
9. Integration test projects

#### Per-Project Checklist
- [ ] Add MTP configuration to project file
- [ ] Run tests locally (both frameworks)
- [ ] Run tests in CI
- [ ] Compare execution times
- [ ] Verify test logs and artifacts
- [ ] Update project documentation
- [ ] Get team sign-off

### Phase 4: Clean-Up & Optimization (1 week)

#### Remove VSTest Dependencies

**File:** `tests/Directory.Build.props`

Before:
```xml
<PackageReference Include="xunit.runner.visualstudio" Version="$(XunitRunnerVisualStudioVersion)" />
```

After:
```xml
<!-- VSTest packages removed - using MTP exclusively -->
```

**File:** `eng/Versions.props`

Remove (only after ALL development environments support MTP):
```xml
<XunitRunnerVisualStudioVersion>3.1.4</XunitRunnerVisualStudioVersion>
```

**Important:** According to xUnit.net documentation, keep these until you can be certain all supported versions of development environments are using MTP instead of VSTest. Supporting VSTest is separate from (and does not interfere with) MTP support.

#### Build Script Cleanup

Remove VSTest-specific code paths and simplify to MTP-only execution.

#### Update Documentation
- [ ] Update TESTGUIDE.md with MTP instructions
- [ ] Update DEVGUIDE.md with new test commands
- [ ] Update CI/CD documentation
- [ ] Update onboarding documentation

### Phase 5: Monitoring & Stabilization (2 weeks)

#### Monitoring
- Track test execution times (should improve)
- Monitor CI reliability (should be same or better)
- Watch for any test flakiness
- Collect developer feedback

#### Rollback Plan
If critical issues arise:
1. Revert `global.json` test runner setting
2. Re-enable `xunit.runner.visualstudio` in Directory.Build.props
3. Restore VSTest-specific build script logic
4. Investigate issues before re-attempting migration

#### Success Metrics
- [ ] All tests pass consistently
- [ ] No increase in test flakiness
- [ ] Test execution time improved or neutral
- [ ] No CI/CD regressions
- [ ] Positive developer feedback
- [ ] All documentation updated

## Technical Details

### .NET 10 MTP Integration

Since the repository uses .NET 10 SDK, MTP integration is native:

**Advantages:**
- No `--` separator needed for MTP arguments
- Native `dotnet test` support
- Better IDE integration

**Configuration:**
```json
// global.json
{
  "test": {
    "runner": "Microsoft.Testing.Platform"
  }
}
```

### Command-Line Changes

**Before (VSTest):**
```bash
dotnet test --configuration Release --framework net10.0 --logger "trx;LogFileName=results.trx"
```

**After (MTP with .NET 10):**
```bash
# xUnit.net uses --report-xunit-trx (not just --report-trx)
dotnet test --configuration Release --framework net10.0 --report-xunit-trx --report-xunit-trx-filename results.trx
```

**Other MTP Report Options:**
```bash
# Generate xUnit XML v2+ format
dotnet test --report-xunit --report-xunit-filename results.xml

# Generate HTML report
dotnet test --report-xunit-html --report-xunit-html-filename results.html

# Generate JUnit XML format
dotnet test --report-junit --report-junit-filename results.xml

# Generate CTRF JSON format
dotnet test --report-ctrf --report-ctrf-filename results.json
```

### Logger Configuration

**TRX Reports (xUnit.net specific):**
- MTP uses `--report-xunit-trx` instead of `--logger trx`
- Filename: `--report-xunit-trx-filename filename.trx` (filename only, no path)
- **Default location:** `TestResults/` directory under output folder
- **Default filename:** `{Username}_{MachineName}_{DateTime}.trx` if not specified
- Can override location with: `--results-directory <directory>`
- **No native placeholder support** in filename (contrary to earlier assumptions)

**Important:** xUnit.net generates xUnit-flavored TRX files, not standard TRX. Use `--report-xunit-trx` not `--report-trx`.

**Console Output:**
- Built into MTP by default
- No need to specify `--logger console` (different from VSTest)

### CI/CD Integration (Azure Pipelines)

**Current Command:**
```yaml
- script: eng\CIBuildNoPublish.cmd -testDesktop -configuration Release -testBatch $(System.JobPositionInPhase)
```

**After Migration:**
Once `global.json` is configured with MTP, the command stays the same. The build script detects MTP and uses appropriate arguments.

```yaml
# No change needed - global.json controls runner choice
- script: eng\CIBuildNoPublish.cmd -testDesktop -configuration Release -testBatch $(System.JobPositionInPhase)
```

**Test Results Publishing:**
```yaml
- task: PublishTestResults@2
  inputs:
    testResultsFormat: 'VSTest'  # TRX files use VSTest format
    testResultsFiles: '**/TestResults/**/*.trx'
```
No changes needed - xUnit.net TRX files are compatible with Azure Pipelines.

## Risk Assessment

### High Risk Areas
1. **CI/CD Compatibility:** Some older CI agents may not support MTP
   - **Mitigation:** Test on actual CI infrastructure early
   - **Fallback:** Keep VSTest option available during transition

2. **Custom Test Extensions:** Any custom test discovery or execution logic may break
   - **Mitigation:** Audit for custom extensions in Phase 1
   - **Fallback:** Port extensions to MTP APIs

3. **Test Flakiness:** Migration may expose or create timing-related test issues
   - **Mitigation:** Run tests extensively before committing to migration
   - **Fallback:** Fix or disable flaky tests, investigate root cause

### Medium Risk Areas
1. **Developer Environment:** Some developers may have older tooling
   - **Mitigation:** Communicate requirements early, provide setup guide
   - **Fallback:** VSTest fallback option during transition

2. **Third-Party Tools:** Some test analysis tools may not support MTP yet
   - **Mitigation:** Verify tool compatibility in Phase 1
   - **Fallback:** Keep VSTest option for specific scenarios

### Low Risk Areas
1. **Test Code:** No changes required to test code itself
2. **Framework Support:** xUnit v3 fully supports MTP
3. **SDK Version:** .NET 10 has native MTP support

## Timeline Estimate

| Phase | Duration | Dependencies |
|-------|----------|--------------|
| Phase 1: Preparation | 2 weeks | None |
| Phase 2: Pilot | 1-2 weeks | Phase 1 complete |
| Phase 3: Rollout | 3-4 weeks | Phase 2 success |
| Phase 4: Clean-Up | 1 week | Phase 3 complete |
| Phase 5: Stabilization | 2 weeks | Phase 4 complete |
| **Total** | **9-11 weeks** | - |

## Decision Points

### Go/No-Go Criteria for Phase 2 → Phase 3
- [ ] Pilot project fully working
- [ ] Performance equal or better
- [ ] CI integration validated
- [ ] Team approval obtained
- [ ] No critical blockers identified

### Go/No-Go Criteria for Phase 4 (Remove VSTest)
- [ ] All test projects migrated
- [ ] 2+ weeks of stable CI runs
- [ ] All known issues resolved
- [ ] Developer feedback positive
- [ ] Rollback plan documented and tested

## Resources

### Documentation
- [Microsoft.Testing.Platform Overview](https://learn.microsoft.com/en-us/dotnet/core/testing/microsoft-testing-platform-intro)
- [VSTest to MTP Migration Guide](https://learn.microsoft.com/en-us/dotnet/core/testing/migrating-vstest-microsoft-testing-platform)
- [xUnit v3 MTP Integration](https://xunit.net/docs/getting-started/v3/microsoft-testing-platform)
- [.NET 10 dotnet test with MTP](https://devblogs.microsoft.com/dotnet/dotnet-test-with-mtp/)

### Support Channels
- GitHub Issues: [xunit/xunit](https://github.com/xunit/xunit/issues)
- Microsoft Q&A: [.NET Testing](https://learn.microsoft.com/en-us/answers/tags/371/dotnet-testing)

## Open Questions

1. **Q:** Do all Azure Pipelines agents support MTP?
   - **A:** Yes, if they have .NET 10 SDK. Verify specific agent images in Phase 1.

2. **Q:** Are there any custom test reporters that need updating?
   - **A:** TBD - audit in Phase 1. MTP has extension system for custom reporters.

3. **Q:** Should we support both VSTest and MTP during migration?
   - **A:** Yes - keep `xunit.runner.visualstudio` package during migration. They don't conflict.

4. **Q:** What is the rollback timeline if issues are discovered?
   - **A:** Immediate rollback via `global.json` change (remove test runner setting). No package changes needed during migration.

5. **Q:** Do we need to update from MTP v1 to v2?
   - **A:** No. xUnit.net v3.1.0 defaults to MTP v1. Can explicitly choose v2 via `xunit.v3.mtp-v2` package if needed.

6. **Q:** Will placeholders like {assembly} and {framework} work in filenames?
   - **A:** No - MTP doesn't support placeholders in report filenames. Must use fixed names or default auto-generated names.

## Approval & Sign-Off

| Role | Name | Status | Date |
|------|------|--------|------|
| Tech Lead | TBD | Pending | - |
| Build/CI Owner | TBD | Pending | - |
| Test Owner | TBD | Pending | - |
| Team Decision | TBD | Pending | - |

---

**Last Updated:** January 21, 2026  
**Next Review:** After Phase 1 completion
