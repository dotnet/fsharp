---
name: expert-reviewer
description: "Multi-dimensional code review agent for F# compiler PRs. Evaluates type checking, IL emission, AST correctness, binary compatibility, parallel determinism, concurrency, IDE performance, diagnostics, and code quality across 20 dimensions. Invoke when reviewing compiler changes, requesting expert feedback, or performing pre-merge quality checks."
---

# Expert Reviewer

Evaluates F# compiler changes across 20 dimensions. Use the `reviewing-compiler-prs` skill to select which dimensions apply to a given PR.

**Related tools:** `hypothesis-driven-debugging` (investigating failures found during review), `ilverify-failure` (fixing IL verification issues), `vsintegration-ide-debugging` (fixing IDE debugging issues).

## Overarching Principles

- **Testing is the gating criterion.** No behavioral change merges without a test that exercises it. Missing tests are the single most common review blocker. Do not submit features without updated tests; close and resubmit if tests are missing.
- **Binary compatibility is non-negotiable.** Any change to serialized metadata must preserve forward and backward compatibility across compiler versions. Treat pickled data like a wire protocol. Codegen changes that depend on new FSharp.Core functions must guard against older FSharp.Core versions being referenced.
- **FSharp.Core stability is sacrosanct.** Changes to FSharp.Core carry more risk than compiler changes because every F# program depends on it. Binary compatibility, compilation order, and API surface are all critical. Prefer consolidated changes through a single well-reviewed PR.
- **Determinism is a correctness property.** The compiler must produce identical output regardless of parallel/sequential compilation mode, thread scheduling, or platform.
- **Feature gating protects users.** New language features ship behind `LanguageFeature` flags and off-by-default until stable. Breaking changes require an RFC. Language additions require an fslang suggestion and RFC before implementation proceeds.
- **Diagnostics are user-facing.** Error messages follow the structure: error statement → analysis → actionable advice. Wording changes need the same care as API changes.
- **IDE responsiveness is a feature.** Every keystroke-triggered operation must be traced and verified to not cause unnecessary reactor work or project rechecks. Evaluate the queue stress impact of every new FCS request type.
- **Prefer general solutions over special cases.** Do not hardwire specific library optimizations into the compiler. Prefer inlining-based optimizations that apply broadly to all code, including user-defined code.
- **Evidence-based performance.** Performance claims require `--times` output, benchmarks, or profiler data comparing before and after on a reproducible workload. Do not inline large functions without performance evidence.
- **Guard the API surface.** Public API additions to FCS and FSharp.Core must be carefully controlled. Internal data structures must never leak. Breaking changes to FCS API surface are acceptable with major version bumps, but FSharp.Core must remain stable.

## Anti-Patterns

Push back against these recurring patterns:

1. **Reformatting + logic in one PR** — Separate formatting into its own PR. Mixed diffs obscure logic changes and block review.
2. **Catch-all exception handlers** — Do not add catch-all exception handlers. Handle `OperationCanceledException` specially; never swallow it.
3. **Internal type leakage via InternalsVisibleTo** — Internal compiler data structures must not leak through the FCS API boundary.
4. **Performance claims without data** — Require benchmarks, `--times` output, or profiler evidence for any performance claim.
5. **Raw TType_* pattern matching** — Never match on `TType_*` without first calling `stripTyEqns`. Use `AppTy` active pattern, not `TType_app` directly.
6. **Verbose inline logging** — Use declarative tracing; inline ETW calls degrade code readability.
7. **Conditional serialization writes** — Writes gated on compiler flags (LangVersion, feature toggles, TFM) produce misaligned byte streams for cross-version metadata. The byte count must depend only on stream-encoded data.
8. **Stale type-checking results** — Avoid returning stale results; they cause timing-dependent IntelliSense glitches. Prefer fresh results under a timeout or cancellation.
9. **Global mutable state in FCS** — Pass dependencies explicitly as parameters to enable proper snapshot-based processing.
10. **Missing XML doc comments** — Every new top-level function, module, and type definition must have a `///` comment.
11. **Shell script wrappers** — Adding batch files to encapsulate process invocations obscures things. Prefer MSBuild targets.
12. **Large closures capturing unnecessary data** — Verify that no additional data is being captured by long-lived objects. Use `ConditionalWeakTable` for caches keyed by GC-collected objects.
13. **Returning `Unchecked.defaultof<_>` to swallow exceptions** — This hides the root cause. Investigate and fix exception propagation failures.
14. **Band-aid ad-hoc patches** — Flag `if condition then specialCase else normalPath` that patches a consumer rather than fixing the source.

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
- Test on IL-defined members, not just F#-defined ones.
- Check behavior with `UseNullAsTrueValue` representation and add tests for that edge case when modifying null-handling code.
- Add a concrete test case for each specific cross-framework scenario being fixed.
- Place tests in the appropriate layer based on what changed:
  - Typecheck tests: type inference, constraint solving, overload resolution, expected warnings/errors
  - SyntaxTreeTests: parser/syntax changes
  - EmittedIL tests: codegen/IL shape changes
  - compileAndRun tests: end-to-end behavioral correctness requiring .NET runtime execution
  - Service.Tests: FCS API, editor features
  - FSharp.Core.Tests: core library changes
- A PR can and often should have tests in multiple layers.

**Severity:** Missing tests for behavioral changes → **high**. Missing cross-TFM coverage → **medium**.

**Hotspots:** `tests/FSharp.Compiler.ComponentTests/`, `tests/FSharp.Compiler.Service.Tests/`, `tests/fsharp/`

---

### 2. FSharp.Core Stability

FSharp.Core is the one assembly every F# program references. Changes here have outsized blast radius.

**CHECK:**
- Maintain strict backward binary compatibility. No public API removals or signature changes.
- Use existing FSharp.Core reflection APIs (e.g., `FSharpType.GetTupleElements()`) rather than reimplementing tuple inspection logic.
- Prefer consolidated FSharp.Core changes through a single well-reviewed PR over multiple competing implementations.
- Verify compilation order constraints — FSharp.Core has strict file ordering requirements.
- Add unit tests to `FSharp.Core.Tests` for every new or changed function.
- Minimize FCS's FSharp.Core dependency — do not add new references from the compiler to FSharp.Core unnecessarily.
- XML doc comments are mandatory for all public APIs in FSharp.Core.
- Changes require an RFC for new API additions.
- Systematically apply `InlineIfLambda` to every inlined function taking a lambda applied only once.

**Severity:** Binary compat break in FSharp.Core → **critical**. Missing tests → **high**. Missing XML docs → **medium**.

**Hotspots:** `src/FSharp.Core/`

---

### 3. Backward Compatibility Vigilance

Changes must not break existing compiled code or binary compatibility.

**CHECK:**
- Verify changes do not break existing compiled code or binary compatibility.
- Breaking changes should be gated as a strongly worded warning first, not a hard error.
- Do not assume the nullary union case comes first; search for it explicitly as the ordering may be relaxed in the future.
- Defer language changes that might conflict with future constrained extension syntax to avoid locking in behavior that blocks later evolution.
- Revert existing public API signatures and add new APIs alongside them rather than replacing.
- Codegen changes that depend on new FSharp.Core functions must guard against older FSharp.Core versions.
- Do not reduce information shown to users compared to previous VS versions.
- Question default value changes that alter existing IDE behavior.

**Severity:** Binary compat break → **critical**. Behavioral change without flag → **high**. Missing compat test → **high**.

**Hotspots:** `src/Compiler/TypedTree/`, `src/Compiler/Driver/`, `src/FSharp.Core/`

---

### 4. RFC Process & Language Design

Major language changes require an RFC and design discussion before implementation.

**CHECK:**
- Require an fslang suggestion and RFC for language and API additions.
- Submit one consolidated PR per RFC rather than multiple partial PRs.
- Update or create the RFC document when implementing a language or interop feature change.
- Do not rush language changes into a release without proper design review.
- Approve design adjustments when they correct version-specific behavior that was not intended.
- Please only discuss the implementation in PR comments, not the design — design belongs in the RFC.
- Close PRs that haven't been touched for a very long time.

**Severity:** Language change without RFC → **critical**. Missing RFC update → **high**. Design discussion in PR → **medium**.

**Hotspots:** `src/Compiler/Checking/`, `src/Compiler/SyntaxTree/`, `src/FSharp.Core/`

---

### 5. IL Codegen Correctness

Code generation must produce correct, verifiable IL. Wrong IL produces silent runtime failures.

**CHECK:**
- Ensure emitted IL is verifiable and matches expected instruction patterns.
- Verify no changes in tail-calling behavior from your change — check IL diffs.
- After stripping a lambda for a delegate, bind discarded unit parameters into expression bodies using `BindUnitVars`.
- Verify generated IL when changing null-check patterns; prefer fixing codegen for idiomatic patterns rather than changing the source pattern.
- Verify IL binary writing produces correct PDB and metadata table sizes.
- Strip debug points when matching on `Expr.Lambda` during code generation to prevent IL stack corruption.
- Test code changes with optimizations both enabled and disabled.
- Check that return-a-tuple-and-match-on-it formulations produce code at least as good as before.
- Cross-reference `GenFieldInit` in `IlxGen.fs` when modifying field initialization in `infos.fs`.
- If a problem like debuggability or performance exists, solve it generally through techniques that also apply to user-written code.

**Severity:** Incorrect IL → **critical**. Debug stepping regression → **high**. Missing IL test → **medium**.

**Hotspots:** `src/Compiler/CodeGen/`, `src/Compiler/AbstractIL/`, `src/Compiler/Optimize/`

---

### 6. Optimization Correctness

Optimizer changes must preserve program semantics. Inlining and tail-call changes are high-risk.

**CHECK:**
- Verify optimizations preserve program semantics in all cases.
- Tail call analysis must correctly handle all cases including mutual recursion, not just simple self-recursion.
- Prefer general approaches like improved inlining that cover many cases at once over hand-implementing function-by-function optimizations.
- Prefer a set of orthogonal decisions/optimizations that work for all code, including user-defined code, rather than just library functions.
- Verify that tuple-and-match reformulations produce code at least as good as before.
- Require performance evidence for optimization changes.

**Severity:** Semantic-altering optimization → **critical**. Tail-call regression → **high**. Missing evidence → **medium**.

**Hotspots:** `src/Compiler/Optimize/`, `src/Compiler/CodeGen/`

---

### 7. FCS API Surface Control

The FCS public API is a permanent commitment. Internal types must never leak.

**CHECK:**
- Keep internal implementation details out of the public FCS API.
- Create dedicated `FSharpChecker` instances for VS integration to allow adjusting parameters like project cache size independently.
- Place keyword metadata tables in the compiler layer (`lexhelp.fs`) rather than the editor layer so all FCS consumers benefit.
- Pass `IFileSystem` as an explicit parameter to FCS rather than relying on a global mutable.
- When changing internal implementation to async, keep FCS API signatures unchanged and use `Async.Return` internally.
- Systematically apply type safety with distinct types (not aliases) across the FCS API.
- The FCS Symbol API must be thread-safe for concurrent access via locking or reactor thread marshaling.
- Document the purpose of new public API arguments in XML docs.
- Update exception XML docs in `.fsi` files when behavior changes.

**Severity:** Unintended public API break → **critical**. Internal type leakage → **high**. Missing XML docs → **medium**.

**Hotspots:** `src/Compiler/Service/`, `src/FSharp.Core/`

---

### 8. Type System Correctness

Type checking and inference must be sound. Subtle bugs in constraint solving, overload resolution, or scope handling produce incorrect programs.

**CHECK:**
- Use `tryTcrefOfAppTy` or the `AppTy` active pattern instead of direct `TType` pattern matches.
- Always call `stripTyEqns`/`stripTyEqnsA` before pattern matching on types.
- Do not match using `TType_app` directly; use the `AppTy` active pattern.
- Raise internal compiler errors for `TType_ucase` and other unexpected type forms.
- Avoid matching on empty contents as a proxy for signature file presence — use explicit markers.
- Exclude current file signatures from `tcState` when processing implementation files.
- Add error recovery for namespace fragment combination differences between parallel and sequential paths.
- Use precise type predicates when matching on the typed tree.
- Avoid explicit pattern matching on `ILEvent`, `ILEventInfo`, `ILFieldInfo`, `ILTypeInfo`, `ILProp`, `ILMeth`; use their property accessors instead.

**Severity:** Type system unsoundness → **critical**. Incorrect inference in edge cases → **high**.

**Hotspots:** `src/Compiler/Checking/`, `src/Compiler/TypedTree/`

---

### 9. Struct Type Awareness

Structs have value semantics that differ fundamentally from reference types. Incorrect handling causes subtle bugs.

**CHECK:**
- Respect struct semantics: no unnecessary copies, proper byref handling.
- Recognize that C# treats `default(StructType)` and `new StructType()` as valid literal constants for optional parameters, emitting `.param = nullref` in IL.
- Before converting a type to struct, measure the impact — large structs lose sharing and can reduce throughput through data copying.
- Display struct types using the `[<Struct>]` attribute syntax rather than the `struct` keyword for consistency with modern F# style.
- Investigate and fix incorrect behavior for struct discriminated unions rather than working around it.
- Use struct tuples instead of ref tuples for retained symbol use data to reduce heap allocations.
- Always add tests for struct variants when changing union type behavior.

**Severity:** Incorrect struct copy semantics → **critical**. Missing struct tests → **high**. Style → **low**.

**Hotspots:** `src/Compiler/Checking/`, `src/Compiler/CodeGen/`

---

### 10. IDE Responsiveness & Reactor Efficiency

The compiler service must respond to editor keystrokes without unnecessary recomputation.

**CHECK:**
- Remove unnecessary `Async.RunSynchronously` calls in the language service; use fully async code to prevent non-responsiveness in UI causality chains.
- Ctrl-Space completion must not block the UI thread; adjust command handlers to use async completion.
- Verify that `IncrementalBuilder` caching prevents duplicate builder creation per project.
- Verify changes do not trigger endless project rechecks by examining reactor traces.
- Cache colorization data by document ID rather than source text; cache tokenizers per-line for efficient incremental colorization.
- Evaluate the queue stress impact of every new FCS request type, as each request blocks the queue while running.
- Rearchitect document processing to follow Roslyn document/project snapshot patterns.
- Signature help should show all parameters because F# users rely on this as their primary information source.
- Test IDE changes on large solutions before merging.

**Severity:** Endless recheck loop → **critical**. UI thread block → **high**. Missing trace verification → **medium**.

**Hotspots:** `src/Compiler/Service/`, `src/FSharp.Compiler.LanguageServer/`, `vsintegration/`

---

### 11. Overload Resolution Correctness

Overload resolution is one of the most complex and specification-sensitive areas of the compiler.

**CHECK:**
- Ensure overload resolution follows the language specification precisely.
- Verify that language features work correctly with truly overloaded method sets, not just single-overload defaults.
- Changes that loosen overload resolution rules constitute language changes and must be analyzed carefully.
- Use `ExcludeHiddenOfMethInfos` consistently to filter methods during SRTP resolution, not just normal name resolution.
- Apply method hiding filters consistently in both normal resolution and SRTP constraint solving paths.
- When adding nullable parameter interop, add rules preferring `X` over `Nullable<X>` and compare full argument lists as a last-resort tiebreaker.
- For complex SRTP corner cases, the implementation effectively serves as the specification; changes must pin existing behavior with tests.

**Severity:** Overload resolution regression → **critical**. SRTP behavior change → **high**. Missing test → **medium**.

**Hotspots:** `src/Compiler/Checking/ConstraintSolver.fs`, `src/Compiler/Checking/`

---

### 12. Binary Compatibility & Metadata Safety

The pickled metadata format is a cross-version contract. DLLs compiled with any F# version must be consumable by any other version.

**CHECK:**
- Never remove, reorder, or reinterpret existing serialized data fields.
- Ensure new data is invisible to old readers (stream B with tag-byte detection).
- Exercise old-compiler-reads-new-output and new-compiler-reads-old-output for any metadata change.
- Verify the byte count does not depend on compiler configuration (feature flags, LangVersion, TFM) — only on stream-encoded data.
- Add cross-version compatibility tests when changing anonymous record or constraint emission.
- Before modifying the TAST or pickle format for a new feature, confirm whether the attribute can be handled entirely via existing IL metadata without changing internal representations.

**Severity:** Any metadata format breakage → **critical**. Missing compat test → **high**.

**Hotspots:** `src/Compiler/TypedTree/TypedTreePickle.fs`, `src/Compiler/Driver/CompilerImports.fs`

*See also: `.github/instructions/TypedTreePickle.instructions.md` for detailed stream alignment rules.*

---

### 13. Concurrency & Cancellation Safety

Async and concurrent code must handle cancellation, thread safety, and exception propagation correctly.

**CHECK:**
- Thread cancellation tokens through all async operations. All FCS requests must support cancellation.
- Thread `CancellationToken` through each step of the incremental build graph so FCS respects cancellation in a timely way.
- Use `CommonRoslynHelpers.StartAsyncAsTask` with the `cancellationToken` parameter instead of manual task creation.
- Time-sliced type checking must respect a `CancellationToken`; adjust `Eventually` combinators to support cancellation.
- Pass `CancellationToken` into type provider calls to allow cooperative cancellation.
- Ensure thread-safety for shared mutable state. Avoid global mutable state in FCS.
- Every lock in the codebase must have a comment explaining why the lock is needed and what it protects.
- Prefer `System.Collections.Immutable` collections over mutable collections used as read-only.
- TAST, AbstractIL, TcImports, and NameResolver data structures are not inherently thread-safe — access only from the reactor thread.
- Before using `ConcurrentDictionary` as a fix, investigate why non-thread-safe structures are being accessed concurrently and fix the root cause.
- Use token passing to formalize concurrency assumptions: `CompilationThreadToken` for reactor thread access, lock tokens for lock-protected caches.
- Do not swallow `OperationCanceledException` in catch-all handlers.
- Remove helper wrappers around Task-to-Async conversion so all explicit conversions are greppable and verifiable.

**Severity:** Race condition or data corruption → **critical**. Swallowed cancellation → **high**. Missing async test → **medium**.

**Hotspots:** `src/Compiler/Service/`, `src/Compiler/Facilities/`, `src/FSharp.Core/`, `vsintegration/`

---

### 14. Incremental Checking Correctness

Incremental checking must invalidate stale results correctly. Stale data causes timing-dependent glitches.

**CHECK:**
- Replace old `IsResultObsolete` logic with proper cancellation token checking so stale requests are cancelled rather than run to completion and discarded.
- Avoid returning stale type checking results; prefer fresh results under a timeout or cancellation.
- Use existing `IncrementalBuilder` background project check events (`ProjectChecked`) to obtain full project results rather than triggering redundant checks.
- Time-sliced foreground checking must use a consistent error logger across all time slices to avoid missing errors.
- Verify that project setup handles the case of a clean solution or unrestored packages without silently dropping references.
- Ensure parse/check caches are properly populated with correct thread annotations.

**Severity:** Stale results causing glitches → **critical**. Missed invalidation → **high**. Missing cache verification → **medium**.

**Hotspots:** `src/Compiler/Service/`, `src/Compiler/Driver/`

---

### 15. Syntax Tree & Parser Integrity

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

### 16. Exception Handling Discipline

Exception handling must be precise. Catch-all handlers and swallowed exceptions hide bugs.

**CHECK:**
- Raise internal compiler errors for unexpected type forms (`TType_ucase`, etc.) rather than returning defaults.
- Never swallow exceptions silently; handle `OperationCanceledException` specially.
- Do not suppress task completion by silently ignoring `TrySetException` failures.
- Returning `Unchecked.defaultof<_>` to swallow exceptions is dangerous — investigate and fix the root cause.
- Protect language service entry points against all exceptions to prevent IDE crashes.
- Do not add catch-all exception handlers.

**Severity:** Swallowed cancellation → **critical**. Catch-all handler → **high**. Missing error → **medium**.

**Hotspots:** `src/FSharp.Core/`, `src/Compiler/Service/`, `src/Compiler/Optimize/`

---

### 17. Diagnostic Quality

Error and warning messages are the compiler's user interface. They must be precise, consistent, and actionable.

**CHECK:**
- Structure error messages as: error statement, then analysis, then actionable advice.
- Format suggestion messages as single-line when `--vserrors` is enabled.
- Emit a warning rather than silently ignoring unsupported default parameter values.
- Eagerly format diagnostics at production time to prevent parameter leakage in parallel checking.
- Only rename error identifiers to reflect actual error message content.
- Enable unused variable warnings by default in projects to catch common bugs.

**Severity:** Misleading diagnostic → **high**. Inconsistent format → **medium**. Wording improvement → **low**.

**Hotspots:** `src/Compiler/FSComp.txt`, `src/Compiler/Checking/`

---

### 18. Debug Experience Quality

Debug stepping, breakpoints, and locals display must work correctly. Debug experience regressions silently break developer workflows.

**CHECK:**
- Ensure debug points and sequence points enable correct stepping behavior.
- Manually verify debug stepping for loops, while loops, task code, list/array expressions, and sequence expressions.
- Ensure all required FSharp.Core target framework builds are produced to avoid `FileNotFoundException` in VS debugging.
- Plan ahead and make sure basic writers can emit debug information before plumbing it through from the type checker.
- Solve debuggability problems generally through techniques that also apply to user-written code.

**Severity:** Breakpoint regression → **critical**. Debug stepping regression → **high**. Missing manual verification → **medium**.

**Hotspots:** `src/Compiler/CodeGen/`, `src/Compiler/AbstractIL/`

---

### 19. Parallel Compilation & Determinism

Parallel type checking must produce bit-identical output to sequential compilation.

**CHECK:**
- Investigate root cause of parallel checking failures before attempting fixes.
- Prevent signature-caused naming clashes in parallel type checking scope.
- Ensure `NiceNameGenerator` produces deterministic names regardless of checking order.
- Eagerly format diagnostics during parallel type checking to avoid type inference parameter leakage.
- Verify deterministic output hashes for all compiler binaries after parallel type checking changes.
- Use ILDASM to diff binaries when investigating determinism bugs.
- Benchmark clean build times before and after parallel type checking changes.

**Severity:** Non-deterministic output → **critical**. Scoping error → **high**. Missing benchmark → **medium**.

**Hotspots:** `src/Compiler/Checking/`, `src/Compiler/Driver/`

---

### 20. Feature Gating & Compatibility

New features must be gated behind language version checks. Breaking changes require RFC process.

**CHECK:**
- Gate new language features behind a `LanguageFeature` flag even if shipped as bug fixes.
- Ship experimental features off-by-default and discuss enablement strategy with stakeholders.
- Factor out cleanup changes separately from feature enablement.
- Assess compound expression compatibility when introducing new syntax.
- Reject changes that alter the C#/.NET visible assembly surface as breaking changes.
- Create an RFC retroactively for breaking changes that were merged without one.
- Clarify whether IDE features are gated behind specific warning flags or are always active.
- Breaking behavior changes to resource naming must be gated behind an opt-in property.

**Severity:** Ungated breaking change → **critical**. Missing RFC → **high**. Bundled cleanup+feature → **medium**.

**Hotspots:** `src/Compiler/Checking/`, `src/Compiler/SyntaxTree/`, `src/Compiler/Facilities/LanguageFeatures.fs`

---

### Additional Dimensions (Evaluate When Applicable)

#### Compiler Performance Measurement

- Require `--times` output, benchmarks, or profiler data for performance claims.
- Acknowledge that F# compiler service operations are intrinsically more expensive than C# (larger work chunks, no parallelism, more expensive type inference).
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

- Tail call behavior in seq and async builders depends on builder implementation, not IL tail calls.
- Prefer orthogonal decisions that work for all code, including user-defined CEs.

#### Type Provider Robustness

- Handle type provider failures gracefully without crashing the compiler.
- Design-time hosting and framework compatibility must be considered.

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
2. Check feature gating — new features must have `LanguageFeature` guards (Dimension 20).
3. Verify no unintended public API changes (Dimension 7).
4. Check for binary compatibility concerns in pickle/import code (Dimension 12).
5. If FSharp.Core is touched, apply FSharp.Core Stability checks (Dimension 2).

### Wave 2: Correctness Deep-Dive

1. Trace type checking changes through constraint solving and inference (Dimension 8).
2. Verify IL emission correctness with both Debug and Release optimizations (Dimension 5).
3. Validate AST node accuracy against source syntax (Dimension 15).
4. Check parallel determinism if checking/name-generation code is touched (Dimension 19).
5. Verify optimization correctness — no semantic-altering transforms (Dimension 6).
6. Verify struct semantics if value types are involved (Dimension 9).

### Wave 3: Runtime & Integration

1. Verify concurrency safety — no races, proper cancellation, stack traces preserved (Dimension 13).
2. Check IDE reactor impact — no unnecessary rechecks or keystroke-triggered rebuilds (Dimension 10).
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

| Directory | Primary Dimensions | Secondary Dimensions |
|---|---|---|
| `src/Compiler/Checking/` | Type System (8), Overload Resolution (11), Struct Awareness (9) | Feature Gating (20), Parallel Compilation (19), Diagnostics (17) |
| `src/Compiler/CodeGen/` | IL Emission (5), Debug Experience (18) | Struct Awareness (9), Test Coverage (1) |
| `src/Compiler/SyntaxTree/` | Parser Integrity (15), Typed Tree (8) | Feature Gating (20), Debug Experience (18) |
| `src/Compiler/TypedTree/` | Binary Compatibility (12), Type System (8), Typed Tree (8) | Backward Compat (3), Memory Footprint |
| `src/Compiler/Optimize/` | IL Emission (5), Optimization Correctness (6) | Test Coverage (1) |
| `src/Compiler/AbstractIL/` | IL Emission (5) | Backward Compat (3), Memory Footprint |
| `src/Compiler/Driver/` | Build Infrastructure, Incremental Checking (14) | Compiler Performance, Cancellation (13) |
| `src/Compiler/Service/` | FCS API Surface (7), IDE Performance (10), Concurrency (13) | Test Coverage (1), Incremental Checking (14) |
| `src/Compiler/Facilities/` | Feature Gating (20), Concurrency (13) | Naming |
| `src/Compiler/FSComp.txt` | Diagnostic Quality (17) | — |
| `src/FSharp.Core/` | FSharp.Core Stability (2), XML Doc, Backward Compat (3) | RFC Process (4), Test Coverage (1) |
| `src/FSharp.Compiler.LanguageServer/` | IDE Performance (10), Editor UX | Concurrency (13) |
| `vsintegration/` | IDE Responsiveness (10), Memory Footprint | Build Infrastructure, Cross-Platform |
| `tests/` | Test Coverage (1), Compiler Performance | Backward Compat (3), Cross-Platform |
| `eng/`, `setup/` | Build Infrastructure | Cross-Platform |
