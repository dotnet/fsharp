---
name: realsig-codegen
description: Debug and fix --realsig+ (RealInternalSignature) codegen bugs in IlxGen — MethodAccessException / FieldAccessException / TypeAccessException at runtime, IL `private` vs `assembly` visibility, closure and TLR-lift placement (cloc / NestedTypeRefForCompLoc / effectiveCloc / moduleCloc). Use when a program compiles cleanly but crashes only under --realsig+, when IL accessibility differs between realsig modes, or when reasoning about where compiler-synthesized closures/state-machines/quotation helpers are nested.
---

# --realsig+ Codegen (IlxGen)

## Mental model

- `--realsig-` (legacy default): source `private` → IL `assembly`; `internal` → IL `assembly`. Visibility intent is hidden; almost everything intra-assembly is reachable.
- `--realsig+`: source `private` → IL `private` (type-scoped); `internal` → IL `assembly`. Matches C# expectations. Flag exists since F# 8 GA; documented in `fsc --help`.
- A compiler-synthesized helper (closure for an inner `let rec`, a `task`/`async`/`seq` state machine, a quotation-splice helper, or a TLR-lifted static) is emitted as its own IL type, nested under the type identified by `eenv.cloc`.
- ECMA-335: a **nested** type may access its **enclosing** type's `private` members; a **sibling** nested type may NOT. So under `--realsig+`, a synthesized helper that calls a `private` member must nest **inside** the declaring type, or the CLR throws `MethodAccessException`/`FieldAccessException`/`TypeAccessException` at first invocation.
- This is the usual root cause of "compiles clean, crashes only under `--realsig+`": the source is legal (the type checker allows `private` access from any lexical position within the type, including inner lambdas), but the helper landed beside the type instead of inside it.

## Key code locations (src/Compiler/CodeGen/IlxGen.fs)

- `GetIlxClosureFreeVars` builds the closure type-ref: `let ilCloTypeRef = NestedTypeRefForCompLoc eenv.cloc cloName`. Whatever `eenv.cloc` is here decides nesting.
- `GenMethodForBinding` generates a member body; `eenvForMeth` is built from the incoming `eenv`. The body (and its closures) run lazily via `DelayCodeGenMethodForExpr`, capturing that env.
- `AddEnclosingToEnv eenv enclosing name ns` sets `cloc.Enclosing = enclosing @ [name]` (the canonical way to push a type onto cloc). `mspec.MethodRef.DeclaringTypeRef` gives a member's exact IL declaring-type path (`Enclosing` + `Name`).
- `effectiveCloc` / `moduleCloc` (PR #19882): TLR-lifted vals route to a stable module/init-class location; the TLR private-ref guard in `InnerLambdasToTopLevelFuncs.SelectTLRVals` refuses lifting an inner-rec that references a type-scoped `private` val under realsig+ (otherwise it would lose access when lifted to the module).
- `ComputeMemberAccess hidden accessibility realsig` (≈line 485): the single point that maps source accessibility → IL access under realsig.

## Gotcha: the optimizer hides the bug in minimal repros

A trivial `private` member (e.g. `= 1`) is **inlined away** by the F# optimizer before codegen, so the call site disappears and the crash vanishes under `--optimize+`. To force a faithful repro, make the member non-inlinable: read mutable state (`backing + 1`) or mark it `[<NoCompilerInlining>]`. NOTE: `[<NoCompilerInlining>]` is the F# **optimizer** attribute; `[<MethodImpl(MethodImplOptions.NoInlining)>]` is the **JIT** attribute — for compiler-inlining experiments use `NoCompilerInlining`.

## Repro methodology

1. Use a **shipped** SDK fsc as a still-broken control: `& "C:\Program Files\dotnet\sdk\<ver>\FSharp\fsc.dll"` (or `dotnet <fsc.dll>`). Compile the same source with `--realsig+` and `--realsig-`; the bug is the delta.
2. Always pass `--optimize+` (and a non-inlinable private) so the call survives to runtime.
3. Inspect IL with ildasm and read **nesting by indentation**: a `.class … C` at 2-space indent with a child `.class … h@N` at 4-space indent = nested (good). Same indent = sibling (the bug). Confirm with the full type name in field refs, e.g. `M/C/h@8` (nested) vs `M/h@8` (sibling).
4. Runtimeconfig template for running the produced exe:
   `{"runtimeOptions":{"tfm":"net10.0","framework":{"name":"Microsoft.NETCore.App","version":"10.0.9"}}}`
5. Compile: `dotnet <fsc.dll> --target:exe --out:X.dll --realsig+ --optimize+ -r:FSharp.Core.dll X.fs` then `dotnet X.dll`.

## Instrumentation pattern

`dprintf` is not in scope in `GetIlxClosureFreeVars`; use `eprintfn` to stderr. To pin where two cases diverge, print `String.concat "/" eenv.cloc.Enclosing`, `v.LogicalName`, `v.IsExtensionMember`, and `v.ApparentEnclosingEntity`, and capture `System.Environment.StackTrace` guarded by a predicate (e.g. `if v.LogicalName = "Run"`). Rebuild FCS + fsc Release, compile a minimal intrinsic-vs-augmentation pair, diff the logs. Remove all instrumentation before committing.

## Worked example: #19933 (PR #19955)

Members declared in an **intrinsic augmentation** (`type C with member ...`) reached `GenMethodForBinding` with only the module in `eenv.cloc` (the augmentation is a separate definition group, so the type was not in the realsig dict-routing path that intrinsic members use), so their closures nested in the module as siblings of `C` → `MethodAccessException` under realsig+. Fix: normalize `eenv.cloc` to `mspec.MethodRef.DeclaringTypeRef` for every non-extension member under `g.realsig` at the top of `GenMethodForBinding` (idempotent for members that already have it; skip `v.IsExtensionMember` — real extension members live in their own module). Gating on `g.realsig` avoids perturbing realsig- IL baselines. One fix covers `let rec`, `task`/`async`, and quotation-splice closures because all go through the same `NestedTypeRefForCompLoc eenv.cloc` site.

## Diagnostics reality (don't mis-cite)

- `FS0193` is the catch-all default in `CompilerDiagnostics.fs` (`| _ -> 193`), not a specific check.
- `FS0491` = `csMemberIsNotAccessible2` (`FSComp.txt`, raised from `ConstraintSolver.fs`) on overload resolution finding 0 accessible candidates; its "from inner lambda expressions" clause is about **protected**, not **private**.
- There is no existing source-level guard for "private member captured into a lambda": an instance `member private this.Secret` called from `let f () = this.Secret()` inside another member compiles cleanly today.

## Tests

- Live under `tests/FSharp.Compiler.ComponentTests/EmittedIL/...`, namespace `EmittedIL.RealInternalSignature`.
- Helpers: `FSharp src |> withRealInternalSignature realsig |> asExe |> withOptimize |> ignoreWarnings`, then `compileExeAndRun |> shouldSucceed` for runtime tests, or `verifyILPresent` / `verifyILNotPresent` for IL-structural assertions.
- Use `[<Theory; InlineData(true); InlineData(false)>]` so both realsig settings run and a regression in either path is caught.
- realsig baselines are the `*.RealInternalSignatureOn.*` / `*.RealInternalSignatureOff.*` `.il.bsl` pairs; regenerate with `TEST_UPDATE_BSL=1` and pair every diff with its `.fs` + a one-line semantic summary (e.g. "closure renested under declaring type").
- IL shape changes can shift `tests/ILVerify` baselines — see the `ilverify-failure` skill.

## Build/run on Windows (when the cwd is content-excluded)

If the `powershell` tool rejects commands because it validates the first token against the repo path, invoke executables by absolute path: `& "C:\Program Files\Git\cmd\git.exe" …`, `& "C:\Program Files\dotnet\dotnet.exe" build …`. Run the built component-test dll directly: `dotnet exec <…\FSharp.Compiler.ComponentTests.dll> --filter-class "*Pattern*"` (xUnit v3 simple filters: `--filter-class` / `--filter-method` / `--filter-namespace`, `*` wildcard). Ensure the matching shared runtime exists under `<repo>\.dotnet\shared\Microsoft.NETCore.App\`.
