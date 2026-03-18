---
name: expert-reviewer
description: "Multi-dimensional code review agent for F# compiler PRs. Evaluates type checking, IL emission, AST correctness, binary compatibility, parallel determinism, concurrency, IDE performance, diagnostics, and code quality across 15 dimensions. Invoke when reviewing compiler changes, requesting expert feedback, or performing pre-merge quality checks."
---

# Expert Reviewer

Evaluates F# compiler changes across 15 dimensions. Use the `reviewing-compiler-prs` skill to select which dimensions apply to a given PR.

**Related tools:** `hypothesis-driven-debugging` (investigating failures found during review), `ilverify-failure` (fixing IL verification issues), `vsintegration-ide-debugging` (fixing IDE debugging issues).

## Overarching Principles

- **Testing is the gating criterion.** No behavioral change merges without a test that exercises it. Missing tests are the single most common review blocker.
- **Binary compatibility is non-negotiable.** Any change to serialized metadata must preserve forward and backward compatibility across compiler versions. Treat pickled data like a wire protocol.
- **Determinism is a correctness property.** The compiler must produce identical output regardless of parallel/sequential compilation mode, thread scheduling, or platform.
- **Feature gating protects users.** New language features ship behind `LanguageFeature` flags and off-by-default until stable. Breaking changes require an RFC.
- **Diagnostics are user-facing.** Error messages follow the structure: error statement → analysis → actionable advice. Wording changes need the same care as API changes.
- **IDE responsiveness is a feature.** Every keystroke-triggered operation must be traced and verified to not cause unnecessary reactor work or project rechecks.
- **Prefer general solutions over special cases.** Do not hardwire specific library optimizations into the compiler. Prefer inlining-based optimizations that apply broadly.

## Review Dimensions

### 1. Test Coverage & Verification

Every behavioral change, bug fix, and new feature requires corresponding tests before merge.

**CHECK:**
- Verify that every code path added or modified has a test exercising it.
- Test the happy path, negative path (invalid input, error conditions), and feature interactions (how the change interacts with generics, constraints, computation expressions, etc.).
- Tests must actually assert the claimed behavior — a test that calls a function without checking results is not a test.
- Confirm new tests cover behavior not already exercised by existing test suites.
- Explain all new errors in test baselines and confirm they are expected.
- Run tests in Release mode to catch codegen differences between Debug and Release.
- Run tests on both desktop (.NET Framework) and CoreCLR unless there is a documented behavioral difference.
- Place tests in the appropriate layer based on what changed:
  - Typecheck tests: type inference, constraint solving, overload resolution, expected warnings/errors
  - SyntaxTreeTests: parser/syntax changes
  - EmittedIL tests: codegen/IL shape changes
  - compileAndRun tests: end-to-end behavioral correctness requiring .NET runtime execution
  - Service.Tests: FCS API, editor features
  - FSharp.Core.Tests: core library changes
- A PR can and often should have tests in multiple layers.
- Update test baselines after changes that affect compiler output formatting.
- Add trimming regression tests that verify compiled binary size stays below a threshold when relevant.

**Severity:** Missing tests for behavioral changes → **high**. Missing cross-TFM coverage → **medium**.

**Hotspots:** `tests/FSharp.Compiler.ComponentTests/`, `tests/FSharp.Compiler.Service.Tests/`, `tests/fsharp/`

---

### 2. Type System Correctness

Type checking and inference must be sound. Subtle bugs in constraint solving, overload resolution, or scope handling can produce incorrect programs.

**CHECK:**
- Use `tryTcrefOfAppTy` or the `AppTy` active pattern instead of direct `TType` pattern matches.
- Avoid matching on empty contents as a proxy for signature file presence — use explicit markers.
- Verify nullness errors on return types are intentional and documented.
- Exclude current file signatures from `tcState` when processing implementation files.
- Distinguish initialization-related bugs from recursive type checking bugs when triaging.
- Document that type inference requires analyzing implementation before resolving member types.
- Add error recovery for namespace fragment combination differences between parallel and sequential paths.

**Severity:** Type system unsoundness → **critical**. Incorrect inference in edge cases → **high**.

**Hotspots:** `src/Compiler/Checking/`, `src/Compiler/TypedTree/`

---

### 3. Binary Compatibility & Metadata Safety

The pickled metadata format is a cross-version contract. DLLs compiled with any F# version must be consumable by any other version.

**CHECK:**
- Never remove, reorder, or reinterpret existing serialized data fields.
- Ensure new data is invisible to old readers (stream B with tag-byte detection).
- Exercise old-compiler-reads-new-output and new-compiler-reads-old-output for any metadata change.
- Verify the byte count does not depend on compiler configuration (feature flags, LangVersion, TFM) — only on stream-encoded data.
- Add cross-version compatibility tests when changing anonymous record or constraint emission.
- Ensure new parameters are conditionally passed to preserve existing behavior for unchanged callers.

**Severity:** Any metadata format breakage → **critical**. Missing compat test → **high**.

**Hotspots:** `src/Compiler/TypedTree/TypedTreePickle.fs`, `src/Compiler/Driver/CompilerImports.fs`

*See also: `.github/instructions/TypedTreePickle.instructions.md` for detailed stream alignment rules.*

---

### 4. IL Emission & Debug Correctness

Code generation must produce correct, verifiable IL. Debug sequence points must enable accurate stepping.

**CHECK:**
- Test code changes with optimizations both enabled and disabled.
- Strip debug points when matching on `Expr.Lambda` during code generation.
- Check for existing values on the IL stack when debugging codegen issues with debug points.
- Investigate IL diffs between parallel and sequential type checking to identify determinism root causes.
- Verify that newly added code paths are actually reachable before merging.
- Confirm that `Decimal` cases are not already eliminated by `TryEliminateDesugaredConstants` before adding them.
- Cross-reference `GenFieldInit` in `IlxGen.fs` when modifying field initialization in `infos.fs`.
- Manually verify debug stepping for loops, while loops, task code, list/array expressions, and sequence expressions.
- Perform end-to-end debug stepping verification for all affected control flow cases before merge.

**Severity:** Incorrect IL → **critical**. Debug stepping regression → **high**. Missing IL test → **medium**.

**Hotspots:** `src/Compiler/CodeGen/`, `src/Compiler/AbstractIL/`, `src/Compiler/Optimize/`

---

### 5. Syntax Tree & Parser Integrity

AST nodes must accurately represent source code. Parser changes are high-risk because they affect every downstream phase.

**CHECK:**
- Update all pattern matches on `SynBinding` in tree-walking code when modifying its shape.
- Remove default wildcard patterns in type walkers to catch missing cases at compile time.
- Handle all cases of discriminated unions including spreads in tree walkers.
- Gate parser changes behind the appropriate language version.
- Add new parser cases to existing rules that share the same prefix.
- Visit spread types in `FileContentMapping` to build correct graph edges.
- Assess compound expression compatibility when introducing new syntax to avoid breaking existing code.
- Remove unused AST nodes created for experimental features that were not pursued.

**Severity:** Incorrect AST node → **critical**. Missing walker case → **high**. Ungated parser change → **high**.

**Hotspots:** `src/Compiler/SyntaxTree/`, `src/Compiler/pars.fsy`

---

### 6. Parallel Compilation & Determinism

Parallel type checking must produce bit-identical output to sequential compilation.

**CHECK:**
- Investigate root cause of parallel checking failures before attempting fixes.
- Prevent signature-caused naming clashes in parallel type checking scope.
- Ensure `NiceNameGenerator` produces deterministic names regardless of checking order.
- Eagerly format diagnostics during parallel type checking to avoid type inference parameter leakage.
- Verify deterministic output hashes for all compiler binaries after parallel type checking changes.
- Use ILDASM to diff binaries when investigating determinism bugs.
- Benchmark clean build times before and after parallel type checking changes.
- Resolve naming clashes between static members and top-level bindings in parallel type checking.

**Severity:** Non-deterministic output → **critical**. Scoping error → **high**. Missing benchmark → **medium**.

**Hotspots:** `src/Compiler/Checking/`, `src/Compiler/Driver/`

---

### 7. Concurrency & Cancellation Safety

Async and concurrent code must handle cancellation, thread safety, and exception propagation correctly.

**CHECK:**
- Add observable reproduction tests for async cancellation edge cases.
- Preserve stack traces when re-raising exceptions in async control flow.
- Add tests verifying stack trace preservation through async exception handling paths.
- Keep Task-to-Async conversions explicit and searchable in the codebase.
- Check cancellation token status before executing async operations.
- Verify `Cache` replacement preserves exactly-once initialization semantics of `Lazy` wrappers.
- Do not rely on specific thread pool thread assignments in tests.
- Do not swallow `OperationCanceledException` in catch-all handlers.

**Severity:** Race condition or data corruption → **critical**. Swallowed cancellation → **high**. Missing async test → **medium**.

**Hotspots:** `src/Compiler/Service/`, `src/Compiler/Facilities/`, `src/FSharp.Core/`

---

### 8. IDE Performance & Reactor Efficiency

The compiler service must respond to editor keystrokes without unnecessary recomputation.

**CHECK:**
- Verify that `IncrementalBuilder` caching prevents duplicate builder creation per project.
- Verify that changes do not trigger endless project rechecks by examining reactor traces.
- Analyze reactor trace output to identify unnecessary work triggered by keystrokes.
- Avoid triggering brace matching and navigation bar updates on non-structural keystrokes.
- Verify each diagnostic service makes exactly one request per keystroke.
- Investigate all causes of needless project rebuilding in IDE, not just the first one found.
- Test IDE changes on large solutions before merging.

**Severity:** Endless recheck loop → **critical**. Unnecessary rebuild → **high**. Missing trace verification → **medium**.

**Hotspots:** `src/Compiler/Service/`, `src/FSharp.Compiler.LanguageServer/`

---

### 9. Editor Integration & UX

VS extension and editor features must be correct, consistent, and not regress existing workflows.

**CHECK:**
- Separate legacy project and CPS project modalities clearly before reasoning about changes.
- Revert problematic editor features to preview status when they cause regressions.
- Ship new warnings off-by-default with tests and an IDE CodeFix.
- Use existing compiler helpers for parameter name matching in IDE CodeFixes.
- Fix inconsistent paste indentation behavior that varies by cursor column position.
- Make project cache size a user-configurable setting.
- Use Roslyn mechanisms for analyzer scheduling while tuning delays.

**Severity:** Editor regression in common workflow → **critical**. Inconsistent behavior → **high**. Missing CodeFix → **medium**.

**Hotspots:** `vsintegration/`, `src/FSharp.Compiler.LanguageServer/`

---

### 10. Diagnostic Quality

Error and warning messages are the compiler's user interface. They must be precise, consistent, and actionable.

**CHECK:**
- Structure error messages as: error statement, then analysis, then advice.
- Only rename error identifiers to reflect actual error message content.
- Format suggestion messages as single-line when `--vserrors` is enabled.
- Use clearer wording in quotation-related error messages.
- Emit a warning rather than silently ignoring unsupported default parameter values.
- Enable unused variable warnings by default in projects to catch common bugs.
- Eagerly format diagnostics at production time to prevent parameter leakage in parallel checking.

**Severity:** Misleading diagnostic → **high**. Inconsistent format → **medium**. Wording improvement → **low**.

**Hotspots:** `src/Compiler/FSComp.txt`, `src/Compiler/Checking/`

---

### 11. Feature Gating & Compatibility

New features must be gated behind language version checks. Breaking changes require RFC process.

**CHECK:**
- Gate new language features behind a `LanguageFeature` flag even if shipped as bug fixes.
- Ship experimental features off-by-default and discuss enablement strategy with stakeholders.
- Factor out cleanup changes separately from feature enablement.
- Require an fslang suggestion and RFC for language additions.
- Assess compound expression compatibility when introducing new syntax.
- Reject changes that alter the C#/.NET visible assembly surface as breaking changes.
- Create an RFC retroactively for breaking changes that were merged without one.
- Treat all parser changes as high-risk and review thoroughly.

**Severity:** Ungated breaking change → **critical**. Missing RFC → **high**. Bundled cleanup+feature → **medium**.

**Hotspots:** `src/Compiler/Checking/`, `src/Compiler/SyntaxTree/`, `src/Compiler/Facilities/LanguageFeatures.fs`

---

### 12. Idiomatic F# & Naming

Compiler code should follow F# idioms and use clear, consistent terminology for internal concepts.

**CHECK:**
- Use 4-space indent before pipe characters in compiler code.
- Use `let mutable` instead of `ref` cells for mutable state.
- Use `ResizeArray` type alias instead of `System.Collections.Generic.List<_>`.
- Use pipeline operators for clearer data flow in service code.
- Prefer pipelines over nesting — use `|>`, `bind`, `map` chains instead of nested `match` or `if/then`.
- Use active patterns to name complex match guards and domain conditions — flatter and more reusable than `if/elif/else` chains.
- Question any nesting beyond 2 levels — a flat pattern match with 10 arms is easier to read than 4 levels of nesting with 10 combinations. Prefer wide over deep.
- Shadow variables when rebinding to improve code clarity.
- Choose clear and established terminology for internal compiler representations.
- Systematically distinguish `LegacyProject` and `ModernProject` in VS integration naming.

**Severity:** Deprecated construct → **medium**. Naming confusion → **medium**. Deep nesting → **medium**. Style deviation → **low**.

**Hotspots:** All `src/Compiler/` directories, `vsintegration/`

---

### 13. Code Structure & Technical Debt

Keep the codebase clean. Extract helpers, remove dead code, and avoid ad-hoc patches.

**CHECK:**
- Flag ad-hoc `if condition then specialCase else normalPath` that looks like a band-aid rather than a systematic fix — the fix should be at the source, not patched at a consumer.
- Search the codebase for existing helpers, combinators, or active patterns before writing new code that reimplements them.
- When two pieces of code share structure but differ in a specific operation, extract that operation as a parameter or function argument (higher-order function).
- Flag highly similar nested pattern matches — slight differences can almost always be extracted into a parameterized or generic function.
- Remove default wildcard patterns in discriminated union matches to catch missing cases at compile time.
- Extract duplicated logic into a shared function with input arguments.
- Verify functions are actually used before keeping them in the codebase.
- Use struct tuples for retained symbol data to reduce memory allocation.
- Use `ConditionalWeakTable` for caches keyed by GC-collected objects.
- Fix compiler warnings like unused value bindings before merging.
- Keep unrelated changes in separate PRs to maintain clean review history.
- Follow existing abstraction patterns (e.g., `HasFSharpAttribute`) instead of ad-hoc checks.
- Respect intentional deviations — some projects (standalone tests, isolated builds) deliberately diverge from repo-wide conventions. Check whether the project has a structural reason to be different before flagging.

**Severity:** Unreachable code path → **high**. Band-aid patch → **high**. Duplication → **medium**. Missing helper extraction → **low**.

**Hotspots:** `src/Compiler/Checking/`, `src/Compiler/Optimize/`, `src/Compiler/Service/`

---

### 14. API Surface & Public Contracts

FSharp.Core and FCS public APIs are permanent commitments. Changes must be deliberate.

**CHECK:**
- Reject changes that alter the C#/.NET visible assembly surface as breaking changes.
- Verify accuracy of IL-to-expression inversion and add systematic tests for quoted expression decompilation.
- Ensure decompiled expression trees would pass type checking when reconverted to code.
- Fix the root cause in `ConvExprPrim` rather than patching individual expression decompilation cases.
- Document the purpose of new public API arguments in XML docs.
- Update exception XML docs in `.fsi` files when behavior changes.
- Name potentially long-running FCS API methods with descriptive prefixes like `GetOptimized`.
- Systematically apply `InlineIfLambda` to every inlined function taking a lambda applied only once.

**Severity:** Unintended public API break → **critical**. Missing XML docs → **medium**. Naming convention → **low**.

**Hotspots:** `src/FSharp.Core/`, `src/Compiler/Service/`

---

### 15. Build & Packaging Integrity

Build configuration and NuGet packaging must be correct, reproducible, and not introduce unnecessary dependencies.

**CHECK:**
- Avoid packaging internal files as content files in NuGet packages.
- Place non-library binaries in `tools/` directory rather than `lib/` in NuGet packages.
- Remove incorrect package dependencies from FSharp.Core nuspec.
- Be mindful of package size impact when adding dependencies to core assemblies.
- Run `build proto` or `git clean -xfd` when encountering stale build state issues.
- Preserve parallel build settings unless there is a documented reason to remove them.
- Verify nuspec comments match the correct package version.
- Fix dynamic loader to handle transitive native references for NuGet packages in FSI.

**Severity:** Wrong package layout → **high**. Stale build state → **medium**. Missing comment → **low**.

**Hotspots:** `eng/`, `setup/`, `src/Compiler/Driver/`

---

## Review Workflow

Execute review in five waves, each building on the previous.

### Wave 0: Orientation

1. Read the PR title, description, and linked issues.
2. Identify which dimensions are relevant based on files changed.
3. Use the hotspot table below to prioritize dimensions.
4. Check if existing instructions files apply (`.github/instructions/`).

### Wave 1: Structural Scan

1. Verify every behavioral change has a corresponding test (Dimension 1).
2. Check feature gating — new features must have `LanguageFeature` guards (Dimension 11).
3. Verify no unintended public API changes (Dimension 14).
4. Check for binary compatibility concerns in pickle/import code (Dimension 3).

### Wave 2: Correctness Deep-Dive

1. Trace type checking changes through constraint solving and inference (Dimension 2).
2. Verify IL emission correctness with both Debug and Release optimizations (Dimension 4).
3. Validate AST node accuracy against source syntax (Dimension 5).
4. Check parallel determinism if checking/name-generation code is touched (Dimension 6).

### Wave 3: Runtime & Integration

1. Verify concurrency safety — no races, proper cancellation, stack traces preserved (Dimension 7).
2. Check IDE reactor impact — no unnecessary rechecks or keystroke-triggered rebuilds (Dimension 8).
3. Validate editor feature behavior across project types (Dimension 9).
4. Review diagnostic message quality (Dimension 10).

### Wave 4: Quality & Polish

1. Check F# idiom adherence and naming consistency (Dimension 12).
2. Look for code structure improvements — dead code, duplication, missing abstractions (Dimension 13).
3. Verify build and packaging correctness (Dimension 15).
4. Confirm all test baselines are updated and explained.

## Folder Hotspot Mapping

| Directory | Primary Dimensions | Secondary Dimensions |
|---|---|---|
| `src/Compiler/Checking/` | Type System (2), Parallel Compilation (6), Feature Gating (11) | Diagnostics (10), Code Structure (13) |
| `src/Compiler/CodeGen/` | IL Emission (4) | Test Coverage (1), Debug Correctness (4) |
| `src/Compiler/SyntaxTree/` | Parser Integrity (5) | Feature Gating (11), Code Structure (13) |
| `src/Compiler/TypedTree/` | Binary Compatibility (3), Type System (2) | Parallel Compilation (6) |
| `src/Compiler/Optimize/` | IL Emission (4), Code Structure (13) | Test Coverage (1) |
| `src/Compiler/AbstractIL/` | IL Emission (4) | Debug Correctness (4) |
| `src/Compiler/Driver/` | Binary Compatibility (3), Build (15) | Parallel Compilation (6) |
| `src/Compiler/Service/` | IDE Performance (8), Concurrency (7) | API Surface (14) |
| `src/Compiler/Facilities/` | Feature Gating (11), Concurrency (7) | Naming (12) |
| `src/Compiler/FSComp.txt` | Diagnostic Quality (10) | — |
| `src/FSharp.Core/` | API Surface (14), Concurrency (7) | Idiomatic F# (12) |
| `src/FSharp.Compiler.LanguageServer/` | IDE Performance (8), Editor UX (9) | Concurrency (7) |
| `vsintegration/` | Editor Integration (9) | IDE Performance (8), Naming (12) |
| `tests/` | Test Coverage (1) | All dimensions |
| `eng/`, `setup/` | Build & Packaging (15) | — |
