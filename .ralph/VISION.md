# F# Test Suite LangVersion 8.0+ Migration - Vision

## High-Level Goal

Migrate all F# compiler test files to be compatible with the new minimum language version requirement of 8.0. This involves removing or updating all references to language versions below 8.0 (4.6, 4.7, 5.0, 6.0, 7.0) in test files, then removing obsolete infrastructure.

## Previous Attempt Lessons (2026-01-15)

1. **Environment issues**: Previous runs encountered `posix_spawnp failed` errors preventing bash commands
2. **Build baseline**: Must use `SKIP_VERSION_SUPPORTED_CHECK=1` to pass existing tests
3. **Atomic changes**: Each subtask must leave the build passing - do NOT remove infrastructure before usages
4. **Some work completed**: StaticClassTests.fs and ComponentTests/Language files already migrated

## Current Remaining Work (grep analysis 2026-01-15)

### withLangVersionXX helper usages (by file count):
- **22 files** still use `withLangVersion(46|47|50|60|70)` (excluding Compiler.fs infra)
- Most are in ComponentTests (15 files) and tests/fsharp (6 files)

### LangVersion.VXX enum usages:
- **5 files** use `LangVersion.V(46|47|50|60|70)`
- Includes ScriptHelpers.fs (infrastructure) and 4 test files

### Raw langversion strings in env.lst:
- **13 fsharpqa env.lst files** have old langversion strings

## Migration Strategy

1. **Migrate test files directory-by-directory** (atomic, passes build after each)
2. **ComponentTests first** (15 files across subdirs)
3. **tests/fsharp second** (6 files including 2 large files)
4. **fsharpqa env.lst files** (13 files, text replacement)
5. **FSharp.Compiler.Service.Tests** (2 files)
6. **Scripting tests** (1 file)
7. **Infrastructure removal LAST** (Compiler.fs, ScriptHelpers.fs)

## Migration Rules

- `withLangVersion70` → `withLangVersion80` (or `withLangVersionPreview` if testing latest features)
- `withLangVersion60` → `withLangVersion80`
- `withLangVersion50` → `withLangVersion80`
- `withLangVersion46/47` → **DELETE the test** (negative version gate tests are obsolete)
- `LangVersion.V70` → `LangVersion.V80`
- `--langversion:5.0` → `--langversion:8.0` (in env.lst)

## Key Constraint

Tests FAIL due to langversion check without env var. Use:
```bash
SKIP_VERSION_SUPPORTED_CHECK=1 dotnet test ...
```

## Success Criteria

- [ ] Zero matches for `withLangVersion(46|47|50|60|70)` outside Compiler.fs
- [ ] Zero matches for `LangVersion\.V(46|47|50|60|70)` outside ScriptHelpers.fs  
- [ ] Zero matches for `langversion:(4\.|5\.|6\.|7\.)` in env.lst (except invalid version tests)
- [ ] Infrastructure helpers removed from Compiler.fs and ScriptHelpers.fs
- [ ] All tests pass with `SKIP_VERSION_SUPPORTED_CHECK=1`
