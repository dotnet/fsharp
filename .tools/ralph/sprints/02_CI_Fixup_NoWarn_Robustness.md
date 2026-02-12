---
---
# Sprint: CI Fixup - Use --nowarn:75 in OtherFlags instead of NoWarn element

## Context - WHY this sprint exists

CI builds 1289940, 1290234, and 1290542 all failed with the same error across IcedTasks and FsToolkit regression tests:

```
FSC : error FS0075: The command-line option 'times' is for test purposes only
```

The original Sprint 01 fix added `<NoWarn>$(NoWarn);0075</NoWarn>` as a separate MSBuild element. This approach is **fragile**: if any `.fsproj` in the third-party repo sets `<NoWarn>3391</NoWarn>` without including `$(NoWarn)`, it **overrides** the Directory.Build.props value entirely, losing the `0075` suppression. The build then fails because `TreatWarningsAsErrors` promotes FS0075 to an error.

The robust fix: pass `--nowarn:75` directly inside `<OtherFlags>` before `--times`. The F# MSBuild task appends `OtherFlags` last on the FSC command line, so it cannot be overridden by project-level MSBuild properties.

## Description

### Files Modified
- `eng/scripts/PrepareRepoForRegressionTesting.fsx` - Replace `<NoWarn>` element approach with `--nowarn:75` in OtherFlags

### Changes Made (3 code paths)

1. **New OtherFlags creation** (line ~82): Changed from:
   ```fsharp
   otherFlags.InnerText <- "$(OtherFlags) --times"
   // + separate NoWarn element
   ```
   To:
   ```fsharp
   otherFlags.InnerText <- "$(OtherFlags) --nowarn:75 --times"
   // No separate NoWarn element needed
   ```

2. **Existing OtherFlags with --times** (line ~110): Changed from adding a `<NoWarn>` sibling element to modifying the OtherFlags value in-place:
   ```fsharp
   otherFlagsWithTimes.InnerText <- otherFlagsWithTimes.InnerText.Replace("--times", "--nowarn:75 --times")
   ```

3. **Create-new Directory.Build.props** (line ~119): Changed the template string to use `--nowarn:75 --times` in OtherFlags instead of a separate NoWarn element.

### Why --nowarn:75 in OtherFlags is robust

- `<NoWarn>` is an MSBuild property. If a .fsproj sets `<NoWarn>3391</NoWarn>` (without `$(NoWarn)`), it replaces the Directory.Build.props value.
- `<OtherFlags>` uses `$(OtherFlags)` which is additive - project files typically don't set OtherFlags at all.
- `--nowarn:75` on the FSC command line is processed by the compiler directly and cannot be overridden by project-level MSBuild NoWarn.
- In the Fsc MSBuild task (src/FSharp.Build/Fsc.fs), OtherFlags is appended at line 360, after NoWarn (line 294). The F# compiler evaluates all options before reporting diagnostics, so the order within OtherFlags doesn't matter.

### Local Verification

Test that was run to confirm the fix:
```bash
# Create test project with TreatWarningsAsErrors AND project-level NoWarn override
mkdir -p /tmp/TestFix/proj
cat > /tmp/TestFix/proj/Directory.Build.props << 'EOF'
<Project>
  <PropertyGroup>
    <OtherFlags>$(OtherFlags) --nowarn:75 --times</OtherFlags>
  </PropertyGroup>
</Project>
EOF
cat > /tmp/TestFix/proj/test.fsproj << 'EOF'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <NoWarn>3391</NoWarn>
  </PropertyGroup>
  <ItemGroup><Compile Include="Lib.fs" /></ItemGroup>
</Project>
EOF
echo 'module Lib' > /tmp/TestFix/proj/Lib.fs
echo 'let x = 42' >> /tmp/TestFix/proj/Lib.fs
cd /tmp/TestFix/proj && dotnet build
# Result: Build succeeded. 0 Warning(s), 0 Error(s)
```

## Definition of Done
- `eng/scripts/PrepareRepoForRegressionTesting.fsx` uses `--nowarn:75` in OtherFlags value (not a separate NoWarn element) for all 3 code paths
- Build succeeds for projects with `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` even when they set `<NoWarn>` without `$(NoWarn)`
- Changes committed with descriptive message
