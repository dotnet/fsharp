# FSharpQA Migration - VISION (Updated 2026-01-17)

## High-Level Goal
Migrate tests from the legacy `tests/fsharpqa` Perl-based test suite to `tests/FSharp.Compiler.ComponentTests` using the existing test infrastructure.

## Current Status Summary

### ✅ Completed Migrations
| Category | Tests | Status |
|----------|-------|--------|
| Diagnostics (all 4 packages) | 138 | ✅ Complete, folder deleted |
| CompilerOptions/fsc | All | ✅ Complete (no env.lst files remain) |
| CompilerOptions/fsi/langversion | 3 | ✅ Complete |
| CompilerOptions/fsi/nologo | 2 | ✅ Complete |
| ObjectOrientedTypeDefinitions (partial) | 218 | ✅ Tests passing, 7 env.lst remain |

### 🚫 Migration Blockers
These tests cannot be migrated due to framework limitations:
- **FSI help tests** (4 tests): `-?`, `--help`, `/?` cause session crash before output
- **FSI highentropyva** (1 test): Unrecognized options crash session
- **FSI subsystemversion** (1 test): Same issue
- **langversion:4.7 tests** (5 tests): Test framework doesn't correctly apply older langversions

### 📋 ObjectOrientedTypeDefinitions Status
**Migrated (7 folders deleted):**
- ExplicitFields, ExplicitObjectConstructors (empty)
- ImplicitObjectConstructors, AsDeclarations, ValueRestriction
- MemberDeclarations, TypeExtensions/intrinsic

**Remaining (7 folders with C# interop/complex deps):**
- InterfaceTypes: 8 tests with C# interop
- ClassTypes/LetDoDeclarations: 1 WPF test
- ClassTypes/InheritsDeclarations: 3 tests with C# interop
- AbstractMembers: 4 tests with C# interop
- DelegateTypes: 1 test with C# interop
- TypeExtensions/basic: 5 tests with C# interop
- TypeExtensions/optional: 17 tests with library dependencies

### 📋 Remaining Work (~1,800 tests)

| Category | env.lst files | Est. Tests | Priority |
|----------|--------------|------------|----------|
| Conformance/Expressions | Many | ~380 | Medium |
| Conformance/ObjectOrientedTypeDefinitions | Many | ~356 | Medium |
| Conformance/TypeForwarding | 6 | ~303 | High (complex C# interop) |
| Conformance/LexicalAnalysis | Many | ~180 | Low |
| Conformance/InferenceProcedures | Many | ~124 | Medium |
| Conformance/TypesAndTypeConstraints | 4 | ~96 | Medium |
| Conformance/ImplementationFilesAndSignatureFiles | Many | ~69 | Low |
| Conformance/DeclarationElements | 2 | ~34 | Medium |
| Conformance/LexicalFiltering | 5 | ~28 | Low |
| Conformance/SpecialAttributesAndTypes | Few | ~14 | Low |
| Conformance/Signatures | Few | ~11 | Low |
| InteractiveSession | 2 | ~169 | High (complex FSI tests) |
| Import | 1 | ~103 | High (C#/VB interop) |
| Misc | 1 | ~31 | Low |
| Libraries | 3 | ~6 | Low |
| Stress | 1 | ~4 | Low |
| MultiTargeting | 1 | ~3 | Low |

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
[<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/[path]", Includes=[|"file.fs"|])>]
let ``test name`` compilation =
    compilation
    |> withOptions ["--test:ErrorRanges"]
    |> typecheck
    |> shouldFail
    |> withErrorCode NNNN
    |> ignore
```

## Important Build/Test Commands
```bash
# Build with proper env vars
BUILDING_USING_DOTNET=true SKIP_VERSION_SUPPORTED_CHECK=1 dotnet build tests/FSharp.Compiler.ComponentTests -c Debug

# Run specific tests  
BUILDING_USING_DOTNET=true SKIP_VERSION_SUPPORTED_CHECK=1 dotnet test tests/FSharp.Compiler.ComponentTests -c Debug -f net10.0 --filter "FullyQualifiedName~[TestName]"
```

## Reference Documents
- **Master Instructions**: `/Users/tomasgrosup/code/fsharp/FSHARPQA_MIGRATION.md`
- **Feature Mapping**: `/Users/tomasgrosup/code/fsharp/FEATURE_MAPPING.md`
- **Test Framework Additions**: `/Users/tomasgrosup/code/fsharp/TEST_FRAMEWORK_ADDITIONS.md`

## Lessons Learned

1. **FSI session limitations** - Cannot test FSI command-line option rejection with `runFsi` since session creation throws exceptions
2. **Pre-existing test failures** - 294 tests fail on main; these are NOT caused by migration
3. **Small batches work better** - Commit after each 20-30 files migrated
4. **TypeForwarding tests** - Complex C# interop, need careful handling
5. **Import tests** - Many require C#/VB compilation helpers
6. **Always use env vars** - `BUILDING_USING_DOTNET=true` and `SKIP_VERSION_SUPPORTED_CHECK=1` required

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
5. **FSI session limitations** - Cannot test FSI command-line option rejection with `runFsi` since session creation throws exceptions before output can be captured
