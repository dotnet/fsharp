# Vision: Centralize Product TFM into Single Source of Truth

## High-Level Goal
Extract the frequently-changing Target Framework Moniker (TFM) value (e.g., `net10.0`) from **dozens of scattered locations** across the repository into a single source of truth file: `eng/productTfm.txt`.

## Current State
The TFM (`net10.0`, `10.0`, `net10`, `10`) is hardcoded in ~35+ files:
- PowerShell scripts: `Build.ps1`, `build-utils.ps1`, `test-determinism.ps1`
- Bash scripts: `build.sh`
- MSBuild props: `Directory.Build.props`, `UseLocalCompiler.Directory.Build.props`
- Pipeline YAML: `regression-test-jobs.yml`
- Test code: `TestFramework.fs`, `single-test.fs`, `CompilerAssert.fs`, `ProjectGeneration.fs`, `Utilities.fs`
- Product code: `CompilerLocation.fs` (toolingCompatibleVersions list)
- Build tools: `checkpackages/*.fsproj` files

## Architecture

```
eng/productTfm.txt ("net10.0")
        │
        ├─► Directory.Build.props (reads file → sets MSBuild properties)
        │   └─► $(FSharpNetCoreProductDefaultTargetFramework)
        │   └─► $(FSharpNetCoreProductMajorVersion)
        │
        ├─► FSharpBuild.Directory.Build.targets (generates buildproperties.fs)
        │   └─► FSharp.BuildProperties.fsProductTfm
        │   └─► FSharp.BuildProperties.fsProductTfmMajorVersion
        │
        ├─► Scripts read file directly:
        │   └─► PowerShell: Get-Content eng/productTfm.txt
        │   └─► Bash: cat eng/productTfm.txt | tr -d '[:space:]'
        │
        ├─► Test code reads file at runtime:
        │   └─► TestFramework.productTfm (from eng/productTfm.txt)
        │
        └─► Product code uses generated constants:
            └─► CompilerLocation.fs uses fsProductTfmMajorVersion
```

## Key Design Decisions

1. **Single file format**: Plain text with just `net10.0`, trimmed on read
2. **MSBuild reads first**: `Directory.Build.props` reads the file and derives properties
3. **Code generation**: `_GenerateBuildPropertiesFile` target generates F# constants
4. **Scripts read directly**: PowerShell/Bash scripts read the file at runtime
5. **Tests read at runtime**: TestFramework reads from repo root for flexibility

## Constraints & Gotchas

1. **Path calculation**: Scripts need correct relative path from their location to `eng/productTfm.txt`
2. **Whitespace trimming**: All readers must trim the file content
3. **Major version extraction**: Derived from TFM by stripping `net` and `.0` suffix
4. **CompilerLocation.fs**: The list generation uses a loop from major version down to 5
5. **Test compatibility**: Some test files may need the TFM via MSBuild property vs TestFramework constant

## Files to Modify

| Category | Files |
|----------|-------|
| **New file** | `eng/productTfm.txt` |
| **MSBuild** | `Directory.Build.props`, `FSharpBuild.Directory.Build.targets` |
| **MSBuild users** | `UseLocalCompiler.Directory.Build.props`, checkpackages fsproj files |
| **Scripts** | `eng/Build.ps1`, `eng/build.sh`, `eng/build-utils.ps1` |
| **Product** | `src/Compiler/Facilities/CompilerLocation.fs` |
| **Tests** | `tests/FSharp.Test.Utilities/TestFramework.fs`, and all files referencing hardcoded TFM |

## Success Criteria
To change the product TFM, edit **one file**: `eng/productTfm.txt`. Done.
