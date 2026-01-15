# Project Plan: F# Compiler Test Suite Migration for LangVersion 8.0+ Minimum

## Executive Summary

The F# compiler now enforces a minimum language version of 8.0 (with an environment variable escape hatch). This change breaks existing tests that explicitly specify language versions below 8.0. This plan outlines a systematic approach to migrate all affected tests. 

---

## Phase 0: Discovery & Inventory

### Task 0.1: Identify All Test Projects and Frameworks

**Objective:** Create a complete inventory of test infrastructure in the repository.

**Actions:**
1. Search for all test project files (`.fsproj`, `.csproj`) in the repository
2. Document which test frameworks are in use (xUnit, NUnit, FsUnit, custom harnesses)
3. Note any custom test runners or infrastructure (e.g., `fsharpqa`, `FSharp.Test. Utilities`)

**Verification:** Create a markdown table listing each test project, its framework, and location.

---

### Task 0.2: Search for All LangVersion References in Tests

**Objective:** Find every test file that explicitly references a language version. 

**Search Patterns to Execute:**
```
--langversion
LangVersion
languageVersion
LanguageVersion
langVersion
/langversion: 
--langversion:
```

**Scope:** All directories under: 
- `tests/`
- `vsintegration/tests/`
- Any other test-related paths

**Output:** A spreadsheet/CSV with columns:
| File Path | Line Number | LangVersion Value | Context (snippet) | Test Type (positive/negative) | Framework |

---

### Task 0.3: Categorize Affected Tests

For each test found, categorize into one of these buckets: 

| Category | Description | Action |
|----------|-------------|--------|
| **A:  Positive Feature Test (old version)** | Tests that a feature works when introduced (e.g., `langversion: 7` for F# 7 features) | Migrate to `preview` or `default` |
| **B: Negative Version Gate Test** | Tests that verify a feature FAILS on older versions | **DELETE** |
| **C: Version Range Test** | Tests that run across multiple versions via attributes | Keep only 8.0+ versions |
| **D:  Incidental Old Version** | Test uses old langversion for unrelated reasons | Investigate & update |
| **E: ML-Compat Related** | Tests for ML compatibility features | **DELETE** (if solely ML-compat) |
| **F: Critical/Complex** | Essential tests that need careful rework | Flag for detailed analysis |

---

## Phase 1: Low-Risk Deletions (Category B & E)

### Task 1.1: Delete Negative Version Gate Tests

**Objective:** Remove tests whose sole purpose is verifying that features fail on old versions.

**Identification Criteria:**
- Test name contains "fail", "error", "not supported", "requires"
- Test expects compilation errors related to language version
- Baseline files expect error messages about unsupported features

**Process:**
1. Create a branch:  `cleanup/remove-negative-langversion-tests`
2. Delete identified test files
3. Delete associated baseline files (`.bsl`, `.expected`)
4. Run full test suite to ensure no dependencies broken
5. PR with clear description listing deleted tests

**Verification:** `dotnet test` passes for affected test projects

---

### Task 1.2: Delete ML-Compat Tests

**Objective:** Remove tests that exist solely for ML compatibility. 

**Identification Criteria:**
- Test path contains `mlcompat` or `ML`
- Test references `--mlcompatibility` flag
- Comments indicate ML compatibility purpose

**Process:** Same as Task 1.1, separate branch:  `cleanup/remove-mlcompat-tests`

---

## Phase 2: Simple Migrations (Category A)

### Task 2.1: Migrate Positive Feature Tests to Preview/Default

**Objective:** Update tests that specify old langversions for "feature introduced in X" testing.

**Rules:**
| Original Version | New Version | Rationale |
|-----------------|-------------|-----------|
| `4.6`, `4.7`, `5.0`, `6.0`, `7.0` | `preview` or `default` | Feature is now stable |
| `8.0`, `9.0`, `10.0` | Keep as-is | Already supported |
| `preview` | Keep as-is | Still preview features |

**Process:**
1. Create branch: `migrate/update-langversion-positive-tests`
2. For each file, find-and-replace langversion values
3. **Run tests after EACH file change** (critical for isolation)
4. Commit each file separately with message: `Update langversion in <filename>:  <old> → <new>`

**Verification:** Each commit should pass CI independently

---

### Task 2.2: Update Project File LangVersion Properties

**Objective:** Find `.fsproj` files with `<LangVersion>` below 8.0

**Search:**
```xml
<LangVersion>4.6</LangVersion>
<LangVersion>4.7</LangVersion>
<LangVersion>5</LangVersion>
<LangVersion>5.0</LangVersion>
<LangVersion>6</LangVersion>
<LangVersion>6.0</LangVersion>
<LangVersion>7</LangVersion>
<LangVersion>7.0</LangVersion>
```

**Process:** Update to `<LangVersion>default</LangVersion>` or remove if not needed

---

## Phase 3:  Parameterized/Multi-Version Tests (Category C)

### Task 3.1: Identify Parameterized Test Patterns

**Objective:** Find tests that run the same code with multiple language versions.

**Patterns to Search:**
```fsharp
[<Theory>]
[<InlineData(... langversion...)>]
[<TestCase>]
[<MemberData>]
yield! [ ... langversion ... ]
for langVersion in [ ... ]
```

**Process:**
1. List all parameterized tests with version parameters
2. Remove test cases for versions < 8.0
3. Keep test cases for 8.0, 9.0, 10.0, preview, default, latest

---

### Task 3.2: Update Test Data Generators

**Objective:** Fix any test data generators that produce old langversions. 

**Search for:**
- Functions returning lists of language versions
- Constants defining version arrays
- Baseline file generators

**Example Fix:**
```fsharp
// Before
let testVersions = [ "5.0"; "6.0"; "7.0"; "8.0"; "9.0" ]

// After  
let testVersions = [ "8.0"; "9.0"; "10.0" ]
```

---

## Phase 4: Complex Cases (Category D & F)

### Task 4.1: Triage "Incidental Old Version" Tests

**Objective:** Understand WHY each test uses an old langversion. 

**For Each Test:**
1. Read the test code and any comments
2. Check git blame for commit that added the version
3. Find the linked issue/PR if any
4. Document in a tracking issue: 
   - Why was this version chosen?
   - What is the test actually testing?
   - Can it work with `default`/`preview`?

---

### Task 4.2: Create Individual Issues for Complex Tests

**Objective:** Don't block the main migration on complex cases.

**For each complex test:**
1. Create a GitHub issue with:
   - File path
   - Current behavior
   - Why it's complex
   - Proposed solution (if known)
2. Tag with `area-tests`, `help-wanted`, `langversion-migration`
3. Link to master tracking issue

---

### Task 4.3: Rework Critical Tests

**Objective:** Ensure essential test coverage is maintained.

**Process:**
1. Understand what the test is validating
2. Determine if the same validation can be done with langversion 8.0+
3. If YES:  rewrite the test
4. If NO:  document what coverage is being lost
5. Consider if new tests are needed for the same functionality

---

## Phase 5: Cleanup & Verification

### Task 5.1: Remove Dead Baseline Files

**Objective:** Clean up orphaned `.bsl`, `.expected`, `.err` files.

**Process:**
1. List all baseline files in test directories
2. For each, verify the corresponding test still exists
3. Delete orphaned files

---

### Task 5.2: Full Test Suite Execution

**Objective:** Verify nothing is broken. 

**Commands to Run:**
```bash
# Build everything
dotnet build -c Release

# Run all tests
dotnet test --no-build -c Release

# Run specific test suites that are known to use langversions heavily
dotnet test tests/FSharp. Compiler.ComponentTests
dotnet test tests/FSharp. Compiler.UnitTests
```

---

### Task 5.3: CI Pipeline Verification

**Objective:** Ensure all CI checks pass.

1. Push to a PR
2. Verify all matrix builds pass (Windows, Linux, macOS)
3. Verify all test legs pass
4. Check for any new warnings

---

## Execution Guidelines

### Work in Small Batches

```
ONE FILE → TEST → COMMIT → REPEAT
```

Never change multiple unrelated tests in the same commit.

### Branch Strategy

```
main
  └── feature/langversion-migration-master
        ├── cleanup/remove-negative-langversion-tests
        ├── cleanup/remove-mlcompat-tests
        ├── migrate/update-langversion-positive-tests
        ├── migrate/parameterized-tests
        └── fix/complex-test-<name>
```

Merge each sub-branch independently after CI passes.

### Test Commands Reference

```bash
# Run a single test file
dotnet test --filter "FullyQualifiedName~TestClassName"

# Run with verbose output
dotnet test -v detailed

# Run tests in a specific project
dotnet test tests/FSharp. Compiler.ComponentTests/FSharp. Compiler.ComponentTests. fsproj
```

---

## Risk Mitigation

| Risk | Mitigation |
|------|------------|
| Breaking unrelated tests | Small commits, run tests after each change |
| Missing coverage | Document what was removed, create follow-up issues |
| Complex tests block progress | Create issues, don't block main migration |
| CI flakiness | Re-run failed jobs, compare with main branch |
| Reviewer fatigue | Small PRs, clear descriptions, categorized changes |

---

## Success Criteria

- [ ] No test file references langversion < 8.0 (except in test infrastructure code)
- [ ] All CI checks pass on main branch
- [ ] Tracking issues created for any deferred work
- [ ] Documentation updated if any public-facing behavior changed

---

## Appendix: Likely High-Impact Directories

Based on typical F# repo structure, focus search on: 

1. `tests/FSharp.Compiler.ComponentTests/`
2. `tests/FSharp. Compiler.UnitTests/`
3. `tests/fsharp/`
4. `tests/fsharpqa/`
5. `vsintegration/tests/`

---

## Tracking

Form a TODO list after running the initial assessment.
