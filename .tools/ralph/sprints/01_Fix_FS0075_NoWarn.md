---
---
# Sprint: Fix FS0075 NoWarn for --times in regression tests

## Context

PR #19273 injects `--times` into `<OtherFlags>` in third-party repos via `eng/scripts/PrepareRepoForRegressionTesting.fsx`. The `--times` flag is an internal F# compiler option (defined in `src/Compiler/Driver/CompilerOptions.fs` under `internalFlags`). When used, it emits **FS0075** as a warning: "The command-line option 'times' is for test purposes only".

Third-party repos like IcedTasks and FsToolkit.ErrorHandling have `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` in their `Directory.Build.props`, which promotes FS0075 to an error, causing all regression test builds to fail.

The fix: when injecting the `--times` flag, also inject `<NoWarn>$(NoWarn);0075</NoWarn>` to suppress the warning.

## Description

### Files to Modify
- `eng/scripts/PrepareRepoForRegressionTesting.fsx` - Add `<NoWarn>` element alongside the `<OtherFlags>` element

### Implementation Steps

1. Open `eng/scripts/PrepareRepoForRegressionTesting.fsx`

2. Find the section that creates the `<OtherFlags>` element (around line 81-82):
```fsharp
let otherFlags = doc.CreateElement("OtherFlags")
otherFlags.InnerText <- "$(OtherFlags) --times"
propertyGroup.AppendChild(otherFlags) |> ignore
```

3. Right after the `propertyGroup.AppendChild(otherFlags)` line, add a `<NoWarn>` element:
```fsharp
let noWarn = doc.CreateElement("NoWarn")
noWarn.InnerText <- "$(NoWarn);0075"
propertyGroup.AppendChild(noWarn) |> ignore
```

4. Find the section that creates a NEW `Directory.Build.props` (when one doesn't exist, around line 111):
```fsharp
let newContent = sprintf "<Project>\n  <Import Project=\"%s\" />\n  <PropertyGroup>\n    <OtherFlags>$(OtherFlags) --times</OtherFlags>\n  </PropertyGroup>\n</Project>\n" absolutePropsPath
```
Change it to:
```fsharp
let newContent = sprintf "<Project>\n  <Import Project=\"%s\" />\n  <PropertyGroup>\n    <OtherFlags>$(OtherFlags) --times</OtherFlags>\n    <NoWarn>$(NoWarn);0075</NoWarn>\n  </PropertyGroup>\n</Project>\n" absolutePropsPath
```

### What to Avoid
- Do NOT modify any compiler source files
- Do NOT modify the pipeline YAML
- Do NOT modify ExtractTimingsFromBinlog.fsx
- Do NOT change the OtherFlags value itself

### Expected Behavior
When the `PrepareRepoForRegressionTesting.fsx` script runs, the resulting `Directory.Build.props` should contain both:
```xml
<PropertyGroup>
  <OtherFlags>$(OtherFlags) --times</OtherFlags>
  <NoWarn>$(NoWarn);0075</NoWarn>
</PropertyGroup>
```

This suppresses warning FS0075 so that repos with `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` don't fail.

### Local Reproduction

To verify the fix works, run this from the repo root (needs a built compiler first via `./build.sh -c Release`):

```bash
cd /tmp
git clone --depth 1 https://github.com/TheAngryByrd/IcedTasks.git TestIcedTasks
cd TestIcedTasks
git checkout 863bf91cdee93d8c4c875bb5d321dd92eb20d5a9
rm -f global.json
dotnet fsi <path-to-fsharp-repo>/eng/scripts/PrepareRepoForRegressionTesting.fsx "<path-to-fsharp-repo>/UseLocalCompiler.Directory.Build.props"
# Verify Directory.Build.props contains both OtherFlags with --times AND NoWarn with 0075
cat Directory.Build.props
```

## Definition of Done
- `eng/scripts/PrepareRepoForRegressionTesting.fsx` adds `<NoWarn>$(NoWarn);0075</NoWarn>` in the same PropertyGroup as OtherFlags, for both the "modify existing" and "create new" code paths
- The script produces valid XML when run against a repo with an existing Directory.Build.props
- The script produces valid XML when run against a repo without a Directory.Build.props
- Changes committed with descriptive message
