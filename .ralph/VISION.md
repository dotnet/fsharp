# FSharpQA Migration - VISION (Updated 2026-01-15)

## High-Level Goal
Migrate tests from the legacy `tests/fsharpqa` Perl-based test suite to `tests/FSharp.Compiler.ComponentTests` using the existing test infrastructure.

## Current State (As of 2026-01-15)

### Completed Packages (4 of 4 Diagnostics packages in progress)
All 4 Diagnostics packages have test files and resources created. Status:

| Package | env.lst Tests | Resources Migrated | Test File | Source Cleanup |
|---------|---------------|-------------------|-----------|----------------|
| DIAG-ASYNC | 15 | 19 files | async.fs (197 lines) | **Pending** - source files still in fsharpqa |
| DIAG-NONTERM | 36 | 38 files | NONTERM.fs (385 lines) | **Pending** - only env.lst remains |
| DIAG-PARSINGEOF | 12 | 12 files | ParsingAtEOF.fs (121 lines) | **Pending** - only env.lst remains |
| DIAG-GENERAL | ~55 (50 unique) | 26 files | General.fs (270 lines) | **Pending** - 29+ files need migration |

### Git Commits Made (on branch `fsharpqa_migration`)
1. `8e3f32799` - Add migration tracking documents
2. `4434e00a7` - Complete DIAG-ASYNC migration: 15 env.lst tests to async.fs
3. `e1cbf72e6` - Migrate DIAG-NONTERM: 36 tests
4. `e0393d899` - Migrate DIAG-PARSINGEOF tests
5. `3b730eb05` - Migrate first 25 tests from DIAG-GENERAL

### Remaining Work for Diagnostics

1. **DIAG-GENERAL (Priority 1)** - 29+ more files to migrate:
   - W_redefineOperator03-10.fs (6 files)
   - E_matrix_*.fs (4 files)
   - E_ExpressionHasType_FullPath01.fs
   - Multi-file tests: E_ConsiderAddingSealedAttribute01.fsi + .fs
   - FSI mode tests: X-DontWarnOnImplicitModule01.fsx, .fsscript
   - Missing source file tests: E_MissingSourceFile01-04.fs
   - All other E_*.fs and W_*.fs files

2. **Cleanup (Priority 2)** - Delete source folders after verification:
   - `tests/fsharpqa/Source/Diagnostics/async/` - delete after tests pass
   - `tests/fsharpqa/Source/Diagnostics/NONTERM/` - delete env.lst
   - `tests/fsharpqa/Source/Diagnostics/ParsingAtEOF/` - delete env.lst
   - `tests/fsharpqa/Source/Diagnostics/General/` - delete after all migrated

## Key Design Decisions

### 1. Use Existing Infrastructure (Don't Reinvent)
- **Compiler.fs** provides all compilation helpers
- **DirectoryAttribute** allows batch-testing with Includes filter
- **No new test framework needed**

### 2. Git-Move Source Files (Preserve History)
- `git mv` source files unchanged to `resources/tests/[path]/`
- Preserves line numbers and clean PR review

### 3. Test Pattern
```fsharp
[<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"file.fs"|])>]
let ``test name`` compilation =
    compilation
    |> withOptions ["--test:ErrorRanges"]
    |> typecheck
    |> shouldFail
    |> withErrorCode NNNN
    |> ignore
```

## Important Notes

### SKIP_VERSION_SUPPORTED_CHECK
When running tests, set `SKIP_VERSION_SUPPORTED_CHECK=1` to avoid version compatibility failures with langversion tests.

### Build/Test Commands
```bash
# Build
dotnet build tests/FSharp.Compiler.ComponentTests -c Release

# Run specific tests
SKIP_VERSION_SUPPORTED_CHECK=1 dotnet test tests/FSharp.Compiler.ComponentTests -c Release --filter "FullyQualifiedName~Diagnostics.General"
```

### Multi-file Tests
For tests with `SOURCE="file1.fsi file2.fs"`:
```fsharp
|> withAdditionalSourceFile (SourceFromPath (resourcePath + "/file1.fsi"))
```

## Lessons Learned

1. **bash posix_spawnp errors** - Can occur with many sessions; use view/glob/grep instead of ls
2. **Pre-existing test failures** - 294 tests fail on main; these are NOT caused by migration
3. **Small batches work better** - Commit after each 20-30 files migrated
4. **Obsolete warnings** - Some legacy tests (e.g. W_IndexedPropertySetter01.fs with FS0191) test for warnings that no longer exist in modern F# compiler. These should NOT be migrated - they should be skipped/deleted.
