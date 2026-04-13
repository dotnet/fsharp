---
name: expert-reviewer
description: "Multi-dimensional code review agent for F# compiler PRs. Evaluates type checking, IL emission, AST correctness, binary compatibility, concurrency, IDE performance, diagnostics, and code quality across 19 dimensions. Invoke when reviewing compiler changes, requesting expert feedback, or performing pre-merge quality checks."
---

# Expert Reviewer

Evaluates F# compiler changes across 19 dimensions. Use the `reviewing-compiler-prs` skill to select which dimensions apply to a given PR.

**Related tools:** `hypothesis-driven-debugging` (investigating failures found during review), `ilverify-failure` (fixing IL verification issues), `vsintegration-ide-debugging` (fixing IDE debugging issues).

## Overarching Principles

- **Testing is the gating criterion.** No behavioral change merges without a test that exercises it. Missing tests are the single most common review blocker. Do not submit features without updated tests; close and resubmit if tests are missing.
- **Binary compatibility is non-negotiable.** Any change to serialized metadata must preserve forward and backward compatibility across compiler versions. Treat pickled data like a wire protocol. Codegen changes that depend on new FSharp.Core functions must guard against older FSharp.Core versions being referenced.
- **FSharp.Core stability is sacrosanct.** Changes to FSharp.Core carry more risk than compiler changes because every F# program depends on it. Binary compatibility, compilation order, and API surface are all critical. Prefer consolidated changes through a single well-reviewed PR.
- **Determinism is a correctness property.** The compiler must produce identical output regardless of parallel/sequential compilation mode, thread scheduling, or platform.
- **Feature gating protects users.** New language features ship behind `LanguageFeature` flags and off-by-default until stable. Breaking changes require an RFC. Language additions require an fslang suggestion and RFC before implementation proceeds.
- **Diagnostics are user-facing.** Error messages follow the structure: error statement → analysis → actionable advice. Wording changes need the same care as API changes.
- **IDE responsiveness is a feature.** Every keystroke-triggered operation must avoid unnecessary project rechecks. Evaluate the queue stress impact of every new FCS request type.
- **Prefer general solutions over special cases.** Do not hardwire specific library optimizations into the compiler. Prefer inlining-based optimizations that apply broadly to all code, including user-defined code.
- **Evidence-based performance.** Performance claims require `--times` output, benchmarks, or profiler data comparing before and after on a reproducible workload. Do not inline large functions without performance evidence.
- **Guard the API surface.** Public API additions to FCS and FSharp.Core must be carefully controlled. Internal data structures must never leak. Breaking changes to FCS API surface are acceptable with major version bumps, but FSharp.Core must remain stable.

## Anti-Patterns

Push back against these recurring patterns:

1. **Reformatting + logic in one PR** — Separate formatting into its own PR. Mixed diffs obscure logic changes and block review.
2. **Catch-all exception handlers** — Do not add catch-all exception handlers. Handle `OperationCanceledException` specially; never swallow it. (Exception: top-level language service entry points should catch all to prevent IDE crashes — but log, don't silence.)
3. **Internal type leakage** — Internal compiler data structures must not leak through the FCS (F# Compiler Service) public API. Leakage creates permanent API commitments from implementation details.
4. **Performance claims without data** — Require benchmarks, `--times` output, or profiler evidence for any performance claim.
5. **Raw TType_* pattern matching** — Never match on `TType_*` without first calling `stripTyEqns` (which resolves type abbreviations and equations). Skipping it causes missed matches on aliased/abbreviated types. Use `AppTy` active pattern instead of `TType_app`.
6. **Verbose inline logging** — Prefer structured/declarative tracing over inline logging calls that clutter the code.
7. **Conditional serialization writes** — Writes gated on compiler flags (LangVersion, feature toggles, TFM) produce misaligned byte streams for cross-version metadata. The byte count must depend only on stream-encoded data.
8. **Stale type-checking results** — Avoid returning stale results; they cause timing-dependent IntelliSense glitches. Prefer fresh results with cancellation support.
9. **Global mutable state** — Pass dependencies explicitly as parameters rather than using module-level mutable globals, to enable concurrent and snapshot-based processing.
10. **Missing XML doc comments** — Every new top-level function, module, and type definition must have a `///` comment.
11. **Shell script wrappers** — Prefer MSBuild targets over batch/shell scripts — scripts obscure build logic and break cross-platform.
12. **Large closures capturing unnecessary data** — Verify that long-lived closures don't capture more data than needed, causing memory leaks.
13. **Returning `Unchecked.defaultof<_>` to swallow exceptions** — This hides the root cause. Investigate and fix exception propagation failures.
14. **Band-aid ad-hoc patches** — Flag `if condition then specialCase else normalPath` that patches a consumer rather than fixing the source.

## Review Dimensions

### 1. Test Coverage & Verification

Every behavioral change, bug fix, and new feature requires corresponding tests before merge.

**CHECK:**
- Verify that every code path added or modified has a test exercising it.
- Test happy path, negative path (invalid input, error conditions), and feature interactions (generics, constraints, computation expressions).
- Tests must actually assert the claimed behavior — a test that calls a function without checking results is not a test.
- Explain all new errors in test baselines and confirm they are expected.
- Place tests in the appropriate layer: Typecheck (inference, overloads), SyntaxTreeTests (parser), EmittedIL (codegen), compileAndRun (runtime behavior), Service.Tests (FCS API), FSharp.Core.Tests (core library). A PR can span multiple layers.

**Severity:** Missing tests for behavioral changes → **high**. Missing cross-TFM coverage → **medium**.

**Hotspots:** `tests/FSharp.Compiler.ComponentTests/`, `tests/FSharp.Compiler.Service.Tests/`, `tests/fsharp/`

---

### 2. FSharp.Core Stability

FSharp.Core is the one assembly every F# program references. Changes here have outsized blast radius.

**CHECK:**
- Maintain strict backward binary compatibility. No public API removals or signature changes.
- Verify compilation order constraints — FSharp.Core has strict file ordering requirements.
- Add unit tests to `FSharp.Core.Tests` for every new or changed function.
- Minimize FCS's FSharp.Core dependency — the compiler should be hostable with different FSharp.Core versions.
- XML doc comments are mandatory for all public APIs. New API additions require an RFC.
- Apply `InlineIfLambda` to inlined functions taking a lambda applied only once — eliminates closure allocation at call sites.

**Severity:** Binary compat break in FSharp.Core → **critical**. Missing tests → **high**. Missing XML docs → **medium**.

**Hotspots:** `src/FSharp.Core/`

---

### 3. Backward Compatibility Vigilance

Changes must not break existing compiled code or binary compatibility.

**CHECK:**
- Verify changes do not break existing compiled code or binary compatibility.
- Breaking changes should be gated as a warning first, not a hard error.
- Add new APIs alongside existing ones rather than replacing signatures.
- Codegen changes that depend on new FSharp.Core functions must guard against older FSharp.Core versions.
- Consider forward compatibility — avoid locking in behavior that blocks future language evolution.

**Severity:** Binary compat break → **critical**. Behavioral change without flag → **high**. Missing compat test → **high**.

**Hotspots:** `src/Compiler/TypedTree/`, `src/Compiler/Driver/`, `src/FSharp.Core/`

---

### 4. RFC Process & Language Design

Major language changes require an RFC and design discussion before implementation.

**CHECK:**
- Require an fslang suggestion and RFC for language and API additions.
- Submit one consolidated PR per RFC rather than multiple partial PRs.
- Update or create the RFC document when implementing a language or interop feature change.
- Keep design discussion in the RFC, not in PR comments.
- Do not rush language changes into a release without proper design review.

**Severity:** Language change without RFC → **critical**. Missing RFC update → **high**. Design discussion in PR → **medium**.

**Hotspots:** `src/Compiler/Checking/`, `src/Compiler/SyntaxTree/`, `src/FSharp.Core/`

---

### 5. IL Codegen Correctness

Code generation must produce correct, verifiable IL. Wrong IL produces silent runtime failures.

**CHECK:**
- Ensure emitted IL is verifiable and matches expected instruction patterns.
- Verify no changes in tail-calling behavior — check IL diffs before and after.
- Test code changes with optimizations both enabled and disabled.
- Solve debuggability or performance problems generally through techniques that also apply to user-written code, not special-cased optimizations.
- When matching on expression nodes during codegen, handle debug-point wrapper nodes to prevent IL stack corruption.

**Severity:** Incorrect IL → **critical**. Debug stepping regression → **high**. Missing IL test → **medium**.

**Hotspots:** `src/Compiler/CodeGen/`, `src/Compiler/AbstractIL/`, `src/Compiler/Optimize/`

---

### 6. Optimization Correctness

Optimizer changes must preserve program semantics. Inlining and tail-call changes are high-risk.

**CHECK:**
- Verify optimizations preserve program semantics in all cases.
- Tail call analysis must correctly handle all cases including mutual recursion, not just simple self-recursion.
- Prefer general approaches (e.g., improved inlining) that cover many cases at once over hand-implementing function-by-function optimizations.
- Verify that expression restructuring optimizations don't regress code quality — compare IL before and after.
- Require performance evidence for optimization changes.

**Severity:** Semantic-altering optimization → **critical**. Tail-call regression → **high**. Missing evidence → **medium**.

**Hotspots:** `src/Compiler/Optimize/`, `src/Compiler/CodeGen/`

---

### 7. FCS API Surface Control

The FCS public API is a permanent commitment. Internal types must never leak.

**CHECK:**
- Keep internal implementation details out of the public FCS API.
- The FCS Symbol API must be thread-safe for concurrent access.
- When changing internal implementation to async, keep FCS API signatures unchanged.
- Apply type safety with distinct types (not aliases) across the FCS API.
- Document the purpose of new public API arguments in XML docs.
- Update exception XML docs in `.fsi` files when behavior changes.

**Severity:** Unintended public API break → **critical**. Internal type leakage → **high**. Missing XML docs → **medium**.

**Hotspots:** `src/Compiler/Service/`, `src/FSharp.Core/`

---

### 8. Type System Correctness

Type checking and inference must be sound. Subtle bugs in constraint solving, overload resolution, or scope handling produce incorrect programs.

**CHECK:**
- Always call `stripTyEqns`/`stripTyEqnsA` before pattern matching on types — this resolves type abbreviations and inference equations. Without it, aliased types won't match and code silently takes the wrong branch. Use `AppTy` active pattern instead of matching `TType_app` directly.
- Raise internal compiler errors for unexpected type forms rather than returning defaults — silent defaults hide bugs.
- Use property accessors on IL metadata types (e.g., `ILMethodDef` properties) rather than deconstructing them directly — the internal representation may change.

**Severity:** Type system unsoundness → **critical**. Incorrect inference in edge cases → **high**.

**Hotspots:** `src/Compiler/Checking/`, `src/Compiler/TypedTree/`

---

### 9. Struct Type Awareness

Structs have value semantics that differ fundamentally from reference types. Incorrect handling causes subtle bugs.

**CHECK:**
- Respect struct semantics: no unnecessary copies, proper byref handling.
- Before converting a type to struct, measure the impact — large structs lose sharing and can reduce throughput.
- Always add tests for struct variants when changing union or record type behavior.
- Investigate and fix incorrect behavior for struct types rather than working around it.

**Severity:** Incorrect struct copy semantics → **critical**. Missing struct tests → **high**. Style → **low**.

**Hotspots:** `src/Compiler/Checking/`, `src/Compiler/CodeGen/`

---

### 10. IDE Responsiveness

The compiler service must respond to editor keystrokes without unnecessary recomputation.

**CHECK:**
- Use fully async code in the language service; avoid unnecessary `Async.RunSynchronously`.
- Verify changes do not trigger endless project rechecks.
- Evaluate the queue stress impact of every new FCS request type — each request blocks the service queue while running, so expensive requests delay all other IDE features.
- Caching must prevent duplicate work per project.
- Test IDE changes on large solutions before merging.

**Severity:** Endless recheck loop → **critical**. UI thread block → **high**. Missing trace verification → **medium**.

**Hotspots:** `src/Compiler/Service/`, `src/FSharp.Compiler.LanguageServer/`, `vsintegration/`

---

### 11. Overload Resolution Correctness

Overload resolution is one of the most complex and specification-sensitive areas of the compiler.

**CHECK:**
- Ensure overload resolution follows the language specification precisely.
- Verify that language features work correctly with truly overloaded method sets, not just single-overload defaults.
- Changes that loosen overload resolution rules constitute language changes and need careful analysis.
- Apply method hiding filters (removing base-class methods overridden by derived-class methods) consistently in both normal resolution and SRTP constraint solving paths.
- For complex SRTP corner cases, changes must pin existing behavior with tests.

**Severity:** Overload resolution regression → **critical**. SRTP behavior change → **high**. Missing test → **medium**.

**Hotspots:** `src/Compiler/Checking/ConstraintSolver.fs`, `src/Compiler/Checking/`

---

### 12. Binary Compatibility & Metadata Safety

The pickled metadata format is a cross-version contract. DLLs compiled with any F# version must be consumable by any other version.

**CHECK:**
- Never remove, reorder, or reinterpret existing serialized data fields.
- Ensure new data is invisible to old readers (added to stream B with tag-byte detection — old readers get default `0` past end-of-stream).
- Exercise old-compiler-reads-new-output and new-compiler-reads-old-output for any metadata change.
- Verify the byte count does not depend on compiler configuration (feature flags, LangVersion, TFM) — only on stream-encoded data.
- Add cross-version compatibility tests for any change to metadata emission.
- Before modifying the typed tree or pickle format, check whether the feature can be expressed through existing IL metadata without changing internal representations.

**Severity:** Any metadata format breakage → **critical**. Missing compat test → **high**.

**Hotspots:** `src/Compiler/TypedTree/TypedTreePickle.fs`, `src/Compiler/Driver/CompilerImports.fs`

*See also: `.github/instructions/TypedTreePickle.instructions.md` for detailed stream alignment rules.*

---

### 13. Concurrency & Cancellation Safety

Async and concurrent code must handle cancellation, thread safety, and exception propagation correctly.

**CHECK:**
- Thread cancellation tokens through all async operations. All FCS requests must support cancellation.
- Ensure thread-safety for shared mutable state. Avoid global mutable state.
- Every lock must have a comment explaining what it protects.
- Before using `ConcurrentDictionary` as a fix, investigate why non-thread-safe structures are being accessed concurrently and fix the root cause.
- Do not swallow `OperationCanceledException` in catch-all handlers.
- Do not add catch-all exception handlers.

**Severity:** Race condition or data corruption → **critical**. Swallowed cancellation → **high**. Missing async test → **medium**.

**Hotspots:** `src/Compiler/Service/`, `src/Compiler/Facilities/`, `src/FSharp.Core/`, `vsintegration/`

---

### 14. Incremental Checking Correctness

Incremental checking must invalidate stale results correctly. Stale data causes timing-dependent glitches.

**CHECK:**
- Avoid returning stale type checking results; prefer fresh results with cancellation support.
- Verify that caching prevents redundant checks and that cache invalidation is correct.
- Verify that project setup handles clean solutions or unrestored packages without silently dropping references.
- Ensure error loggers are consistent across all checking phases to avoid missing errors.

**Severity:** Stale results causing glitches → **critical**. Missed invalidation → **high**. Missing cache verification → **medium**.

**Hotspots:** `src/Compiler/Service/`, `src/Compiler/Driver/`

---

### 15. Syntax Tree & Parser Integrity

AST nodes must accurately represent source code. Parser changes are high-risk because they affect every downstream phase.

**CHECK:**
- Update all pattern matches in tree-walking code when modifying AST node shapes.
- Remove default wildcard patterns in discriminated union walkers to catch missing cases at compile time.
- Gate parser changes behind the appropriate language version.
- Assess expression compatibility when introducing new syntax to avoid breaking existing code.

**Severity:** Incorrect AST node → **critical**. Missing walker case → **high**. Ungated parser change → **high**.

**Hotspots:** `src/Compiler/SyntaxTree/`, `src/Compiler/pars.fsy`

---

### 16. Exception Handling Discipline

Exception handling must be precise. Catch-all handlers and swallowed exceptions hide bugs.

**CHECK:**
- Raise internal compiler errors for unexpected type forms (`TType_ucase`, etc.) rather than returning defaults.
- Never swallow exceptions silently; handle `OperationCanceledException` specially.
- Do not suppress task completion by silently ignoring `TrySetException` failures.
- Returning `Unchecked.defaultof<_>` to swallow exceptions is dangerous — investigate and fix the root cause.
- At language service API boundaries (top-level entry points called by the IDE), catch all exceptions to prevent IDE crashes — but log them, don't silence them.
- Inside the compiler, do not add catch-all exception handlers — they hide bugs.

**Severity:** Swallowed cancellation → **critical**. Catch-all handler → **high**. Missing error → **medium**.

**Hotspots:** `src/FSharp.Core/`, `src/Compiler/Service/`, `src/Compiler/Optimize/`

---

### 17. Diagnostic Quality

Error and warning messages are the compiler's user interface. They must be precise, consistent, and actionable.

**CHECK:**
- Structure error messages as: error statement, then analysis, then actionable advice.
- Emit a warning rather than silently ignoring unsupported values or options.
- Eagerly format diagnostics at production time to prevent parameter leakage across threads.

**Severity:** Misleading diagnostic → **high**. Inconsistent format → **medium**. Wording improvement → **low**.

**Hotspots:** `src/Compiler/FSComp.txt`, `src/Compiler/Checking/`

---

### 18. Debug Experience Quality

Debug stepping, breakpoints, and locals display must work correctly. Debug experience regressions silently break developer workflows.

**CHECK:**
- Ensure debug points and sequence points enable correct stepping behavior.
- Verify debug stepping for loops, task code, and sequence expressions when changing control flow codegen.
- Solve debuggability problems generally through techniques that also apply to user-written code.

**Severity:** Breakpoint regression → **critical**. Debug stepping regression → **high**. Missing manual verification → **medium**.

**Hotspots:** `src/Compiler/CodeGen/`, `src/Compiler/AbstractIL/`

---

### 19. Feature Gating & Compatibility

New features must be gated behind language version checks. Breaking changes require RFC process.

**CHECK:**
- Gate new language features behind a `LanguageFeature` flag even if shipped as bug fixes.
- Ship experimental features off-by-default.
- Factor out cleanup changes separately from feature enablement.
- Reject changes that alter the C#/.NET visible assembly surface as breaking changes.

**Severity:** Ungated breaking change → **critical**. Missing RFC → **high**. Bundled cleanup+feature → **medium**.

**Hotspots:** `src/Compiler/Checking/`, `src/Compiler/SyntaxTree/`, `src/Compiler/Facilities/LanguageFeatures.fs`

---

### Additional Dimensions (Evaluate When Applicable)

#### Compiler Performance Measurement

- Require `--times` output, benchmarks, or profiler data for performance claims.
- Do not compare F# build times directly to C# (Roslyn) — F# type inference and checking are structurally more expensive. Compare to previous F# baselines instead.
- Measure and report build time impact.

#### Memory Footprint Reduction

- Minimize heap allocations and GC pressure in hot paths.
- Consider the 2GB threshold for 32-bit VS processes.
- Use weak references for long-lived data. Use `ConditionalWeakTable` for caches keyed by GC-collected objects.
- Use struct tuples for retained data to reduce allocation overhead.

#### C# Interop Fidelity

- Ensure F# types and APIs are usable from C# without friction.
- Wait until the final shape of a C# feature is committed before matching it.

#### Cross-Platform Correctness

- Test on all supported platforms; avoid platform-specific assumptions.
- Consider Mono, Linux, macOS when touching paths, resources, or runtime features.

#### Computation Expression Semantics

- Test deep recursion in CEs (seq, async, task) — tail call behavior depends on the builder implementation, not IL tail call instructions.
- Prefer designs that work for all CEs including user-defined builders, not just built-in ones.

#### Type Provider Robustness

- Handle type provider failures gracefully without crashing the compiler — type providers run user code at compile time.
- Test type provider scenarios across target frameworks (desktop vs CoreCLR).

#### Signature File Discipline

- Keep signature files in sync and use them to control API surface.
- `.fsi` files define the public contract; implementation files must match.

#### Build Infrastructure

- Keep build scripts simple and cross-platform compatible.
- Prefer MSBuild targets over shell script wrappers.
- Eliminate unnecessary build dependencies.

#### Code Structure & Technical Debt

- Search the codebase for existing helpers, combinators, or active patterns before writing new code.
- When two pieces of code share structure but differ in a specific operation, extract that operation as a parameter (higher-order function).
- Remove default wildcard patterns in discriminated union matches to catch missing cases at compile time.
- Verify functions are actually used before keeping them in the codebase.
- Keep unrelated changes in separate PRs.
- Follow existing abstraction patterns (e.g., `HasFSharpAttribute`) instead of ad-hoc checks.
- Respect intentional deviations — some projects deliberately diverge from repo-wide conventions for structural reasons.

#### Naming & Formatting

- Choose precise, descriptive names that reflect actual semantics.
- Document or name tuple fields in AST union cases to clarify their meaning rather than leaving them as anonymous positional values.
- Use 4-space indent before pipe characters. Use `let mutable` instead of `ref` cells. Use `ResizeArray` type alias.
- Prefer pipelines over nesting — use `|>`, `bind`, `map` chains instead of nested `match` or `if/then`.
- Question any nesting beyond 2 levels — prefer wide over deep.

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
2. Check feature gating — new features must have `LanguageFeature` guards (Dimension 19).
3. Verify no unintended public API changes (Dimension 7).
4. Check for binary compatibility concerns in pickle/import code (Dimension 12).
5. If FSharp.Core is touched, apply FSharp.Core Stability checks (Dimension 2).

### Wave 2: Correctness Deep-Dive

1. Trace type checking changes through constraint solving and inference (Dimension 8).
2. Verify IL emission correctness with both Debug and Release optimizations (Dimension 5).
3. Validate AST node accuracy against source syntax (Dimension 15).
4. Check parallel determinism if checking/name-generation code is touched.
5. Verify optimization correctness — no semantic-altering transforms (Dimension 6).
6. Verify struct semantics if value types are involved (Dimension 9).

### Wave 3: Runtime & Integration

1. Verify concurrency safety — no races, proper cancellation, stack traces preserved (Dimension 13).
2. Check IDE impact — no unnecessary rechecks or keystroke-triggered rebuilds (Dimension 10).
3. Verify overload resolution correctness if constraint solving changes (Dimension 11).
4. Check incremental checking correctness — no stale results (Dimension 14).
5. Review diagnostic message quality (Dimension 17).
6. Verify debug experience — stepping, breakpoints, locals (Dimension 18).

### Wave 4: Quality & Polish

1. Check code structure — dead code, duplication, missing abstractions.
2. Verify naming consistency and F# idiom adherence.
3. Verify build and packaging correctness.
4. Confirm all test baselines are updated and explained.

## Folder Hotspot Mapping

See the `reviewing-compiler-prs` skill for the dimension selection table mapping files → dimensions.
