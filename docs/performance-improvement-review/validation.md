# Performance-improvement review: validation

Start at [the delivery audit](#sprint-12-delivery-audit) and [sprint-11 final acceptance](#sprint-11-bounded-semantic-validation). The RED record below is preserved apart from this navigation note. Earlier results are in [GREEN and post-refactor replay](#green-and-post-refactor-replay); [C5](#c5-final-candidate-freeze) and [its complete 33-case result](#bench-01-synthetic-contract-only-not-real-holdout-coverage) retain the actual execution history. Earlier `not run`, fallback-only discovery and [sprint-10 residual obligations](#sprint-10-reconciliation-and-residual-manifest) describe their historical freezes, not current status. Historical "verified human" labels are superseded by the [source-attribution audit](evidence.md#named-conversation-audit), not by changed case expectations.

**RED creation recorded:** `2026-09-22T11:04:34Z`, against repository revision `ae294143e5890033fcb717956050a91a6f2ed6e8`. Freeze version **RED-1** includes the independently verified pre-freeze corrections recorded below. This freezes source-derived expectations before a new reviewer exists. It is corpus validation, **not a failed candidate execution**. No candidate reviewer, benchmark, compiler build or compiler test is run in this sprint. Source acquisition remains bounded by [corpus.md](corpus.md)'s **2026-04-22 through 2026-09-22** window and `2026-09-22T09:28:34Z` cutoff; later capture times do not extend it.

## Outcomes

- `pass`: the applicable claim/check is supported by the available evidence; not a universal certification.
- `needs evidence`: a specific missing measurement, reproducibility detail, or behavior proof prevents a conclusion; not proof of a defect.
- `risk/blocker`: concrete code, observed regression, or a substantiated failure scenario establishes a material risk. Cite the mechanism and evidence.
- `not applicable`: this check does not apply to the actual claim/change; give a short reason.

G1-G8 mean the exact eight groups in [evidence.md](evidence.md). Outcomes below apply only to named checks. A structural/test-shape `pass` is not an overall PR approval or an assertion that the tests were executed here.

## Packet boundary and immutable inputs

Give the candidate **only** the selected fenced `text` packet below, the PR/family/code coordinates/capture/hash fields of its source-pin row (not corpus links), and the diff/source files reconstructed at those pins. Do not give it this document's oracle matrix, coverage/audit sections, evidence rules' source discussion, corpus source notes, PR decisions, review threads, later fixes, or live GitHub discussion access. The runner may fetch pinned Git objects but not mutable PR pages. This keeps the held-back objection/answer out of the reviewer input. Full diffs are reconstructible, not copied into the matrix:

```powershell
git --no-pager diff --no-ext-diff --no-textconv --full-index --binary $mergeBase $head
git --no-pager show "${head}:src/Compiler/AbstractIL/ilwrite.fs"
```

Git object syntax uses repository-relative `/`; filesystem paths use `\`. Use the **merge base for that historical head**, not today's main or a final head's merge base. Descriptions are captured final-stage snapshots, with original body hashes/capture times in the corpus. Earlier edited descriptions cannot be recovered: historical code-only pairs deliberately contain **no PR description or benchmark table**. They score only the designated code/behavior/scope check; missing descriptions in those curated slices are not claims that the real PR lacked a description. Never combine a final description with old code and pretend it was available then.

Source-pin keys are evaluator bookkeeping, not outcome hints. For final-stage packets, the exact corpus description/diff hashes remain the raw-snapshot references; the compact packet is a faithful extraction, not a verbatim historical body. SHA-256 packet hashes below cover the fenced text's UTF-8 bytes with LF line endings and a final LF. Future changes must create a new case/version rather than quietly overwrite these oracles.

| Pin | PR / family | Stage and code head | Merge base | Description capture / source snapshot |
| --- | --- | --- | --- | --- |
| A | #20352 / FAMILY-inline-copy | Sole submitted head `39167b8148d900fe003aea4cd80f95643fdb691f` | `c251d06dc38db89fa6df30b0b17a233e0ffe403d` | 2026-09-22T09:38:11Z; [corpus source](corpus.md#pr-20352-source-record), body `beec79d023bacc943775799a935dc72881228ce22a05d37a664f1f056e7c4d6c` |
| B | #20363 / FAMILY-inline-copy | Final `7679c374c71954ab3fe3e8a99c211e965ea14e07` | `3ec76a9b9ac738f45012735fce800c831236415d` | 2026-09-22T09:38:13Z; [corpus source](corpus.md#pr-20363-source-record), body `36e93794ee1867aefc0e3f0c5655931a327025cbaf1f95d1e0e8f9bf8992d400` |
| C | #20348 / FAMILY-self-profile | Original review `2a9e33aef0bf3df3bab894b332e8f0b55d7d9e08` | `7b487c5eebd27e92465c775fcee8880f848c3512` | Code-only; original description unavailable, not replaced with current body. |
| D | #20348 / FAMILY-self-profile | Final `95479116c5693f9b96727cc2ac88e965982e621e` | `cecab991fb8c8e47058c0fad5426235b99f3a293` | 2026-09-22T10:44:55Z; [supplement](corpus.md#red-stage-source-completeness-supplement), body `c2833efa181272ea57c4782ca2392a2167dd7a006f52d75358604d6b6c6a00e7` |
| E | #20261 / FAMILY-il-import-retention | Final `9d8662d93106e401d67e480e5fb0d812b04cb57c` | `4824d92223908a73c008a7e5b015377394f1ce5f` | 2026-09-22T09:37:54Z; [corpus source](corpus.md#pr-20261-source-record), body `c72a761e6fbebeb3c87112011553475d8ee703471bd22a26d81640db6f6a0739` |
| F | #20353 / FAMILY-self-profile | Original review `b77c6c6828547768dcdfed1a04347331fdd12e5c` | `cecab991fb8c8e47058c0fad5426235b99f3a293` | Code-only; original description unavailable. |
| G | #20353 / FAMILY-self-profile | Final `604669d1a6fd688f6ee0cd1daff3e7020192a080` | `a29233c3e369b7fd1fd6727b5ea7fad656f12e0d` | 2026-09-22T10:46:40Z; [supplement](corpus.md#red-stage-source-completeness-supplement), body `feec10f69bf3e0bddec79e70055f5c87f937d10e56013fba4320f1890e587159` |
| H | #20349 / FAMILY-self-profile | Original review `b0770fe05ee57d56bbb292277cc35f20b3ec81f7` | `c251d06dc38db89fa6df30b0b17a233e0ffe403d` | Code-only; original description unavailable. |
| I | #20349 / FAMILY-self-profile | Final `cc98f598d445d5f5c1a2bf962ebd3d82fc599c5c` | `a29233c3e369b7fd1fd6727b5ea7fad656f12e0d` | 2026-09-22T10:45:31Z; [supplement](corpus.md#red-stage-source-completeness-supplement), body `24566be697793a66875afaf691a814a3e0338513e90d47b848998c8865c3b5f2` |
| J | #19689 / FAMILY-ce-method-cache | Final `0356d321ea15fc963e68d272cd5785363a30e086` | `2feeea6baa880c86a6ea9b65387c6e9509f5943f` | 2026-09-22T09:34:52Z; [corpus source](corpus.md#pr-19689-source-record), body `c2b2fb94d8431b454931bcfe994fcd8fd017f30cb811004880e3aa9fc94fd480` |
| K | #20416 / FAMILY-il-import-retention | Final `552a152bbcb7d50c94c3d9d97b49200affc94eb9` | `7ad874a23f007e5209787f87f2bbf59acfb7875c` | 2026-09-22T09:38:24Z; [corpus source](corpus.md#pr-20416-source-record), body `3121d91255ca976ebf80b037d05593ca2e0ff3a07f1b4e6a00b892ffc2ac2846` |
| L | #20244 / FAMILY-pattern-memo | Final `da1d79e3b7be82b430df95491dc9bcba5e904960` | `1361727543589b7a2997cbae48302025cc93564a` | 2026-09-22T09:37:54Z; [corpus source](corpus.md#pr-20244-source-record), body `6ac636243a5d5c598f44cbaeaf096817003ba2d7a9114319dfe082d8bc6b39ca` |
| M | #20296 / FAMILY-il-import-retention | Final `86ece46d9e5a7ae67aca866749ae4f377dab5ff1` | `cecab991fb8c8e47058c0fad5426235b99f3a293` | 2026-09-22T09:37:59Z; [corpus source](corpus.md#pr-20296-source-record), body `dfa516b5ad4ea0b9ed5120fe6999fcd94a1dc3c6140383abbbaf73c30cfb1dbd` |

### P01

```text
Pin A. Final captured description; no review decisions supplied.
Claim: fuse copyExpr and remarkExpr to avoid the second full tree rebuild.
65,880 LOC / 120 files, --optimize+: allocated MB/compile 10,673 -> 10,352.
Description reports byte-identical emitted IL/PDBs on all checked workloads.
No repetition count, variability, machine/tool versions, workload revision or runnable command supplied.
Diff: optimizer call sites plus pervasive optional range/debug-point handling in the remapper and an exported internal signature; no added tests.
```

### P02

```text
Pin B. Final captured description.
Claim: fuse generic inlining copy and type instantiation using remapExpr/mkInstRemap.
Release net11.0, 65,880 LOC, GC.GetTotalAllocatedBytes(true), 3 runs x 4 iterations.
build | allocated MB/compile
base | 10,662
changed | 10,105
Description reports 671 EmittedIL/Optimizations passes, six PrintFunction environment failures also on base, and differential IL/PDB identity for listed generic/loop/closure/byref/SRTP cases.
Reflected-definition resource-name tag changes with the unique-number count; payload hash reportedly unchanged.
Two diff hunks in one localized optimizer function plus release note; remapping implementation unchanged.
No raw runs, dispersion, hardware/SDK identity, workload revision or runnable benchmark command supplied.
```

### P03

```text
Pin C. Historical code-only scope check, not a reconstructed PR description.
SortTableRows packs Val/Tag/original position into int64 keys to preserve stable ordering.
It fills zero-created key and result arrays with indexed loops; System.Array.Sort sorts the keys.
Inspect the complete pinned diff and SortTableRows callers. No timing evidence is supplied in this slice.
```

### P04

```text
Pin D. Final captured description and code.
Four self-profiled compiler hot-path changes: metadata sorting, postponed target emission, nullness reuse, type-hierarchy common-subexpression elimination.
Metadata sort: 65,536 rows, 10.6 -> 2.5 ms and 6.55 -> 1.05 MB allocated; 64 rows, 2.99 -> 0.43 us.
Nullness application: about 153,000 allocations removed per 65k-LOC compile; sampled own CPU 0.52 -> 0.38 s.
Hierarchy loop exclusive CPU 214 -> 160 ms. Target-emission microbench claims 2.6-7x speed and about half allocations.
Description claims output identity. Repetitions/error, environment, exact workload/compiler benchmark commits and runnable commands absent.
SortTableRows uses array comprehensions for keys/results at this head.
```

### P05

```text
Pin E. Final captured description. Claim: less retained memory after ParseAndCheckProject.
Mean of 3 runs in fresh processes per project.
Project | before MB | after MB
consoleapp | 33.25 | 31.11
Oxpecker | 69.65 | 65.77
IcedTasks | 47.10 | 44.93
FsToolkit | 69.97 | 67.86
Fantomas.Benchmarks | 64.26 | 61.33
Prime | 100.99 | 99.44
Fantomas.Core | 107.74 | 106.21
Fantomas.Core.Tests | 140.20 | 137.61
FSharp.Common | 261.81 | 251.68
fcs | 1226.73 | 1223.99
FSharp.Common has 489 references. Description reports total allocation down 1-34 MB/project and analysis time unchanged within noise.
Two tables per reader: string-heap cache grows from zero; computed-name interning removed.
Author reports about 3.0 MB fixed buckets versus at most 0.24 MB duplicated names (2,077 strings).
No machine/runtime/tool versions, spread/raw runs, executable command, subject revisions, benchmark binary commit mapping or precise retained-heap collection protocol supplied.
```

### P06

```text
Pin F. Historical code-only behavior/test check. No historical description recovered.
GetParamAttribs memoizes IL parameter attributes per ImportMap with WeakMap/MemoizationTable.
Key equality uses physical ILMethodDef and extension-view flag, ignoring the supplied range; instantiated declaring types bypass memoization.
Added test compares instance/static Enumerable.Select results using "if a <> b then failwith".
The test pipeline ends with compile |> shouldSucceed.
Inspect OptionalArgInfo.FromILParameter, including metadata import and error location.
```

### P07

```text
Pin G. Final description/code.
Claim: eliminate repeated IL parameter-attribute computation on overload-heavy compilation.
ComputeILMethodParamAttribs allocated 477 MB -> 0; description also says "~810 MB total".
fsc --times table: 99.75% hits; 5,341 adds; 0 updates; 2,167,099 hits; 5,341 misses; 0 evictions.
Output reportedly byte-identical. No elapsed-time table, variability, environment or executable workload command supplied.
Cache uses physical ILMethodDef plus extension flag and ignores range; instantiated declaring types bypass memoization.
Instance/static extension-call test now ends with compileExeAndRun |> shouldSucceed.
```

### P08

```text
Pin H. Historical code-only behavior check; historical description not recovered.
applyBrFixups replaces sorting by offset with List.rev.
RecordReqdBrFixups prepends the current code position, then emits an instruction byte and placeholders.
No execution result is supplied in this slice.
```

### P09

```text
Pin I. Final captured description/code.
Branch fixups: replace sorting by offset with List.rev.
65,880 LOC / 120 files, net11 Release; about 46,465 calls/compile, 88% empty, longest list 25.
step | before | after
time ms/compile | 0.595 | 0.240
allocated KB/compile | 1444 | 634
Author reports 139,370 method-body order comparisons, zero mismatches and zero ties.
RecordReqdBrFixups now documents strictly increasing recorded positions and asserts them in Debug.
No raw measurements, variability, machine/SDK details or reproduction command supplied.
```

### P10

```text
Pin J. Final captured description/code.
Centralizes CE builder-method lookup in a per-expression dictionary, routes method-presence checks through it and adds missing-member diagnostics tests.
Lookup changes AtMostOneResult to AllResults and substitutes the builder range for per-call lookup ranges.
Description includes a Release net10.0/osx-arm64 build log and two passing component tests.
No before/after performance measurements, workload or repetitions are supplied; build duration is not a comparative benchmark.
No explicit elapsed-time speedup is claimed.
```

### P11

```text
Pin K. Final captured description/code.
Claim: share imported project CCUs rather than pickle/unpickle each consumer.
Solution | projects | before retained MB | after retained MB
ReSharper.FSharp | 10 | 385.94 | 304.64
Fantomas | 8 | 350.42 | 275.79
FSharp.Compiler.Service | 14 | 879.43 | 818.39
FsToolkit.ErrorHandling | 8 | 87.77 | 69.36
Oxpecker | 16 | 131.67 | 114.24
Prime | 5 | 112.22 | 97.93
IcedTasks | 7 | 96.97 | 92.37
consoleapp | 1 | 29.38 | 29.37
Author says handover alone moves the FCS solution by 2 MB, not 61 MB; most saving comes from dropping language version from the import reuse key.
Checker creation flags changed relative to earlier numbers; exact flags, repetitions/error, environment and commands absent.
Final code creates unresolved thunks for names absent from the consumer; captured description still says missing names keep the producer's CCU.
Final diff includes sharing/language-setting/race changes and tests for concurrent consumers, diagnostics and imported surfaces.
```

### P12

```text
Pin L. Final captured description/code. Claim: avoid guarded-or-pattern compilation blowup.
End-to-end compilation with --optimize+; author states about 2.4 s fixed startup.
N | before time / image size | after time / image size
6 | 3.7 s / 8 KB | 2.6 s / 8 KB, identical IL
8 | 2.5 s / 17 KB | 3.8 s / 12 KB
12 | 3.2 s / 183 KB | 2.5 s / 25 KB
16 | 9.6 s / 2.9 MB | 2.8 s / 61 KB
20 | >90 s, did not finish | 3.3 s / 126 KB
24 | stack overflow | 3.9 s / 221 KB
Emitted-runtime shape: promoted --optimize+ uses static calls/no allocation; --optimize- uses a local FSharpFunc closure and one call/allocation per shared residual.
Byref-like results are not promoted. Ordinary nonpromoted matches reportedly keep identical deterministic IL; promoted quotations intentionally expose joinThunk.
Author reports matching active-pattern evaluation counts for two inputs at N=6/8/16. Tests assert execution, guard bindings, byref/rethrow and quotation behavior.
No generated-program elapsed-runtime benchmark, repetitions/error, environment or exact benchmark compiler SHAs supplied.
```

### P13

```text
Pin M. Final captured description/code. Claim: share imported assembly CCUs across projects.
Real whole-solution options graph; all projects checked and held.
Solution | projects | retained off MB | retained on MB
ReSharper.FSharp | 10 | 1547.2 | 1002.2
Oxpecker | 16 | 226.8 | 190.4
FsToolkit.ErrorHandling | 8 | 218.3 | 206.7
IcedTasks | 7 | 194.6 | 184.5
Fantomas | 8 | 665.7 | 658.3
Prime | 5 | 215.1 | 212.9
FCS.slnx | 14 | 2444.9 | 2443.7
consoleapp | 1 | 30.9 | 30.9
Warm ParseAndCheckProject time: median 9 samples across processes, rotated order.
ReSharper 13.28 -> 12.10 s (-8.9%); Fantomas 3.88 -> 3.90 s (+0.7%, author says inside spread).
Retained repetition count, numerical time spread, raw runs, hardware/SDK and exact benchmark commit/command mapping not supplied.
Final code uses typed dependency-closure/configuration keys, weak entries and per-entry strong dependency ownership; default on with opt-out.
```

### S01

```text
Request-required synthetic control. No real PR, commits or human source.
Description: "Faster."
No benchmark numbers, workload, configuration, environment or code evidence supplied.
```

### S02

```text
Request-required synthetic control. No real PR or human source.
Claim limited to generated closure count for one unchanged input/flags.
Supplied before/after IL inventories show 12 -> 0 generated closure classes.
Supplied executed assertions cover the input's values, evaluation order and exceptions.
No runtime-speed claim or runtime benchmark; no new retained state.
```

### S03

```text
Request-required synthetic control, not real measured data.
Repeated identical requests: median time 100 -> 80 ms, 30 runs, spread 1 ms on each side.
Changed code appends every request graph to a process-rooted list; no removal, cap or release path.
Post-full-GC retained MB for 100/200/400 requests: baseline 20/20/20; changed 120/220/420.
All request objects are otherwise dead; heap-root trace points to the list.
```

### S04

```text
Request-required synthetic control. No real PR.
Only a spelling correction in a Markdown guide; diff contains no code, configuration, generated files or performance claim.
```

### S05

```text
Request-required synthetic control, not real measured data.
Claim: faster compilation, same input and Release binaries.
30 comparable fresh-process runs per side: baseline mean 100.0 ms, 95% CI [98,102]; changed mean 99.8 ms, 95% CI [97.8,101.8].
No further distribution or practical-workload benefit evidence is supplied.
```

### S06

```text
Request-required synthetic control, not real measured data.
Claim: allocation improvement, not speed.
Same input/flags and machine; 30 runs on identified baseline/changed binaries, executable command and raw runs supplied.
allocated B/op | baseline 256 (all runs) | changed 128 (all runs)
elapsed ns/op | baseline 100 +/- 2 | changed 100 +/- 2
post-release retained bytes and GC pause distribution do not regress.
Supplied behavior assertions pass; no ordering/API/generated-output change.
```

### S07

```text
Request-required synthetic control for generated-program runtime, not a real benchmark run.
Fixture binaries runtime-base/runtime-changed use identical F# input, Release optimization and runtime.
The stipulated benchmark report identifies Windows x64, .NET 11, the compiler binary hashes and "fixture-run --input range-32767 --repeat 30" with all raw results.
Same 32,767-element input: baseline 280 us mean (95% CI 275-285); changed 80 us (95% CI 78-82); allocated 393,680 -> 131,088 B/op.
Generated IL intentionally differs. Supplied runtime assertions preserve result, evaluation order and exceptions, including empty and singleton input.
The supplied memory/GC checks show no relevant regression. No compiler-build speedup is claimed.
```

## Frozen expectation matrix

Human URLs below are **evaluator-only**. Context from unverified user-account prose supplies a request-required control grounded in actual code, not a human-derived oracle. No PR lifecycle state is passed to the candidate as a shortcut.

| Case | Split/family/PR/revision | Claim and coverage tags | Human oracle URL or request-required control | Frozen input/evidence available to reviewer | Expected row/outcome | Exact evidence needed or prohibited finding | Actual run/result |
| --- | --- | --- | --- | --- | --- | --- | --- |
| DEV-01 | development / inline-copy / #20352 / A | compiler allocation; withdrawal; broad scope | [Author withdrawal](https://github.com/dotnet/fsharp/pull/20352#issuecomment-5422336205), PERF-08 | P01 + A diff/context | G7 `needs evidence` | Request narrower approach or justification of pervasive range/debug handling against the gain. No fabricated maintainer rejection, false measurement or correctness bug. | not run (RED expectation frozen) |
| DEV-02 | development / inline-copy / #20363 / B | real reported allocation win; narrower alternative | Request-required PERF-02/08/10; same family as DEV-01 | P02 + B | G7 `pass` for localized helper reuse; G2/G4/G8 `needs evidence` for reproducibility | Do not repeat the broad-remapper objection. Request missing raw spread/environment/command, not repetitions already supplied. Do not assert all bytes identical despite resource-name caveat. | not run (RED expectation frozen) |
| DEV-03 | development / self-profile / #20348 / C | scope; simpler idiom; before review | [Suggestion](https://github.com/dotnet/fsharp/pull/20348#discussion_r3854812488), PERF-09 | P03 + C only | v2: G7 `not applicable` for the manual-loop performance-necessity comparison subcheck | This slice makes no claim that manual filling is necessary for speed. A simpler-comprehension suggestion is optional and non-blocking; no suggestion is also acceptable. Do not require comparative timing, assert equivalent/superior speed, or invent a syntax-only correctness blocker. | not run (RED expectation frozen) |
| DEV-04 | development / self-profile / #20348 / D | resolved scope; compiler microbench/CPU | [Acceptance](https://github.com/dotnet/fsharp/pull/20348#discussion_r3860828838), PERF-09; request PERF-02/03 | P04 + D | G7 `pass` for that resolved simplification; G2/G3/G8 `needs evidence` | Do not request already-adopted comprehensions. Ask for tabular reproducible measurements/variability; no whole-build speedup from isolated CPU samples. | not run (RED expectation frozen) |
| DEV-05 | development / il-import-retention / #20261 / E | strong table; retained versus allocated; noise-level time | Request-required PERF-02/03/04/05; uncertain/model-associated review excluded | P05 + E | G3 `pass` for retained-memory metric alignment; G2/G4/G8 `needs evidence` | Recognize ten-row table and three fresh-process runs. Ask for precise collection protocol, versions/commits, commands and spread. Do not claim missing table/repetitions or require a time win. | not run (RED expectation frozen) |
| DEV-06 | development / self-profile / #20353 / F | cache; behavior proof; compile-only test | Request-required PERF-06/11; [attribution-limited source](https://github.com/dotnet/fsharp/pull/20353#discussion_r3881817308) | P06 + F | G6/G8 `needs evidence` | Execute the runtime conditional, not merely compile it; inspect omitted-range error paths. Missing proof is not a demonstrated runtime failure. | not run (RED expectation frozen) |
| DEV-07 | development / self-profile / #20353 / G | resolved runtime-test shape; allocation/hit ratio | Request-required PERF-03/06/11; [resolution context](https://github.com/dotnet/fsharp/pull/20353#discussion_r3914681725) | P07 + G | G8 `pass` only for executing-test shape; G2/G6 `needs evidence` | No repeated compile-only finding. Request result/reproduction details and range-sensitive malformed-metadata proof if claiming diagnostic equivalence. Hit ratio is not elapsed-time evidence. | not run (RED expectation frozen) |
| DEV-08 | development / self-profile / #20349 / H | ordering; invariant; before review | Request-required PERF-07; [attribution-limited request](https://github.com/dotnet/fsharp/pull/20349#discussion_r3881120590) | P08 + H | G6 `needs evidence` | Document or assert producer ordering and check ties/empty input. Do not claim List.rev is wrong when producer order supports it. | not run (RED expectation frozen) |
| DEV-09 | development / self-profile / #20349 / I | resolved invariant; real reported time/allocation win | Request-required PERF-07; [resolution context](https://github.com/dotnet/fsharp/pull/20349#discussion_r3914836899) | P09 + I | G6 `pass` for invariant documentation/assertion; G2/G8 `needs evidence` | Stop the missing-producer-invariant finding; retain only actually missing measurement/reproduction details. Do not claim the debug assertion or differential run was executed here. | not run (RED expectation frozen) |
| DEV-10 | development / ce-method-cache / #19689 / J | prose-only caching; closed-unmerged; other author | Request-required PERF-01/02/06/12; no human closure explanation exists in packet | P10 + J | G1/G2 `needs evidence` for performance justification | Name workload/cost and before/after evidence if presenting cache as an improvement. Do not turn build logs into speed data, invent a claimed time win, or infer performance rejection from closure. | not run (RED expectation frozen) |
| DEV-11 | development / il-import-retention / #20416 / K | FCS; multiple changes; causal attribution; stale description | Request-required PERF-02/10; author table is not a human oracle | P11 + K | G1/G7 `pass` for explicitly limiting handover attribution; G2/G8 `needs evidence` | Recognize 2 MB versus 61 MB isolation already supplied; ask for exact flags, baseline/changed benchmark mapping, spread/commands and correction of stale fallback description. Do not attribute all 61 MB to handover or repeat fixed producer-fallback code as current. | not run (RED expectation frozen) |
| DEV-12 | development / pattern-memo / #20244 / L | compiler win; generated closure/runtime trade-off; smaller-input slowdown | Request-required PERF-03/04/05/06; author-reported table, not human oracle | P12 + L | G3/G6 `pass` for distinguishing output modes/proof shape; G4/G5/G8 `needs evidence` | Retain N=8 slowdown and unoptimized closure allocation; request repetitions/spread and relevant emitted-runtime cost evidence. Do not assert a measured user-program speedup, require identical IL for promoted code, or invent a binder-stamp miscompile. | not run (RED expectation frozen) |
| DEV-13 | development / il-import-retention / #20296 / M | strong FCS timing/retention tables; repeated work; non-win retained | Request-required PERF-02/03/04/05; auduchinok author data, provenance not upgraded | P13 + M | G3 `pass` for separating metrics/methodology; G2/G4/G8 `needs evidence` | Recognize nine rotated timing samples; ask for numerical spread and actual missing retention/reproduction metadata. Preserve Fantomas +0.7% as reported inside spread; no universal -35.2% memory or -8.9% speed claim. | not run (RED expectation frozen) |
| DEV-14 | synthetic development / SYN-faster / no PR | weak prose; unsupported finding control | request-required synthetic control, PERF-01/02/12 | S01 only | G1/G2/G8 `needs evidence` | Request named workload, before/after timing table, commits/config, repetitions/error/environment. Prohibit invented correctness defect. | not run (RED expectation frozen) |
| DEV-15 | synthetic development / SYN-closures / no PR | closure structural claim; no measured speed | request-required synthetic control, PERF-03/06/11 | S02 only | G3/G6 `pass` for supplied structural/behavior proof; elapsed-speed check `not applicable` | Do not assert measured speedup or require runtime timing when the claim is limited to closure count. | not run (RED expectation frozen) |
| DEV-16 | synthetic development / SYN-retention / no PR | one good metric masks unbounded retained state | request-required synthetic control, PERF-05/12 | S03 only | G5 `risk/blocker` | Cite process-rooted append-only list and linear post-GC growth; measured time win does not remove the lifetime risk. Not merely needs evidence. | not run (RED expectation frozen) |
| DEV-17 | synthetic development / SYN-docs / no PR | genuine docs-only negative control | request-required synthetic control, PERF-01/12 | S04 only | G2-G5 metric checks `not applicable` | State no performance/code change; no compiler/GC benchmark demand. | not run (RED expectation frozen) |
| DEV-18 | synthetic development / SYN-noise / no PR | noise-level speed claim | request-required synthetic control, PERF-04 | S05 only | G4 `needs evidence` | Ask for enough data to resolve the claimed effect/practical value. Overlapping intervals alone do not prove equality or a regression; no arbitrary percentage threshold. | not run (RED expectation frozen) |
| DEV-19 | synthetic development / SYN-allocation / no PR | allocation win with unchanged elapsed time | request-required synthetic control, PERF-03/05 | S06 only | G3/G4/G5 `pass` for the stated supplied claim | No demand that elapsed time improve; no end-to-end speed claim or universal certification. | not run (RED expectation frozen) |
| DEV-20 | synthetic development / SYN-runtime / no PR | measured generated-program runtime gain; intentionally different IL | request-required synthetic control, PERF-02/03/06 | S07 only | G3/G6 `pass` for the stipulated runtime/semantic evidence | Do not demand byte-identical IL or advertise a faster compiler. The positive timing dataset is synthetic and was not executed. | not run (RED expectation frozen) |

## Coverage and limits

The cases separate reported real improvements from reproduced results: **none of these benchmarks was rerun here**. Paired versions of #20348, #20349 and #20353 stay within `FAMILY-self-profile`; #20352/#20363 stay within `FAMILY-inline-copy`. Their pairs are not independent family votes or holdout coverage.

| Required category | Cases / rules | Boundary |
| --- | --- | --- |
| Need/workload, weak prose, description/table quality | DEV-01/02/04/05/10/14; PERF-01/02 | Build/test output is not comparative performance evidence. |
| Strong tables, real reported wins, noise | DEV-02/05/09/12/13/18/19; PERF-03/04 | Strong presentation does not fill absent variability, protocol or binary provenance. |
| Resource trade-offs; good metric masking regression | DEV-05/12/13/16/19; PERF-05 | Real tables preserve worse/flat cells; demonstrated unbounded-retention blocker is synthetic, not a real human rejection. |
| Compiler/FCS, cold/warm, repeated/incremental work | DEV-04/05/11/12/13; PERF-01/03/05 | Fresh-process project checks do not establish incremental/cache lifetime behavior. |
| Closure/generated-program runtime gains and costs | DEV-12/15/20; PERF-03/05/06 | Real #20244 supports runtime shape/trade-offs, not measured user-program speed. Positive elapsed-runtime control DEV-20 is explicitly synthetic. |
| Withdrawal/scope/helper reuse and resolved findings | DEV-01/02/03/04/08/09; PERF-07/08/09/10 | Author withdrawal is verified; no independent maintainer performance rejection is invented. |
| Behavior/diagnostics/runtime assertions | DEV-06/07/08/09/11; PERF-06/07/11 | Unchanged APIs and successful compilation are not universal semantic proof. |
| Unsupported findings and applicability | DEV-10/14/15/17/19; PERF-12 | Closure counts do not imply speed, closure does not imply rejection, docs do not need benchmarks. |

No verified human-derived precedent is claimed for a numerical significance threshold, thread-pool/starvation diagnosis, peak-memory protocol or comprehensive API/compatibility matrix. These remain conditional request-required rules, not omissions. No unverified/model-associated review is used to manufacture real regression/rejection coverage. Synthetic controls cover the specified decision boundaries, not untouched real-world holdout performance.

**Additional runtime-source search:** on 2026-09-22, `gh api --method GET search/issues --paginate --slurp` exhausted these exact queries, all with `per_page=100` and `incomplete_results=false`: `repo:dotnet/fsharp is:pr created:2026-04-22..2026-09-22 "ns" in:body` (4), the same with `"BenchmarkDotNet"` (1), and with `"runtime"` (77), one page each. New non-bot descriptions #20500/#20007/#19672/#19713/#19714 concern correctness/docs/platform support; #20605 fixes concurrency in a test-only runtime builder but supplies no comparative runtime measurements. Dependency-update bots supply no human measurement evidence. Previously inventoried/reserved descriptions were not used for new answers, and no reserved feedback was opened. No new unreserved measured-runtime case was established; DEV-20 fills that positive boundary honestly rather than consuming a holdout.

## Local audit

Candidate results remain `not run (RED expectation frozen)` regardless of corpus-audit success. The creation commit is the commit first adding this file (`git log --diff-filter=A -- docs\performance-improvement-review\validation.md`); the parent revision and RED recording time above deliberately do not masquerade as a self-referential commit hash.

### Packet hashes

| Packet | SHA-256 |
| --- | --- |
| P01 | `e08dee7997e9436902dc59d995447040d27ef5b3b17799ee516822018449cc1f` |
| P02 | `10b6555413209b8b4eb30842c3a37ba186bf0a23bdca86d06c0890a6d79e69e7` |
| P03 | `6f22f926034f14e3992d2398319b40680caf14c952231cbcf34b77ec83d8950d` |
| P04 | `053dd3a71d38ccb3b05cd14f73a21cd6269828e64e9f938ed8a9ed5f9531e40d` |
| P05 | `2d1ea6e520883e8d370d8a20793504bc6f0f125fcc0646bc26a0978cc0fa3a20` |
| P06 | `b2e781847671b55f709c27a4974da6247b93ee63639c0b066071b926609528e5` |
| P07 | `407637e3feb770ca22c1994e902fb69547473effc8ad72ec9e4d0bf22c73ce49` |
| P08 | `f23a9ecb687cb4efa71d18ff2848793ac89f7e827343d2107547556504ab4bff` |
| P09 | `3ea419fdc2435d4519f70205ea0a970c9d5ca33a6b7fd65b80fcf79b4bcdee35` |
| P10 | `7dec86a848d89a04853d9f480d7fe4986aa20344a7421118545a483d69ae12a1` |
| P11 | `b183381a12d35e03730a7831c82fbdced2eb383bf1dccc3dfbbb4f36710054a4` |
| P12 | `a90a9cb03eed72017bb2e376716958472b84a4b67d647debad4654e012a86878` |
| P13 | `600f168c2eeefba739315685b1fa878768904deef5dda207b4c52f37d018398e` |
| S01 | `133e2bd370963042a95ca1609827d4b07a9bca5465eb1182790f3a8fc4d114b2` |
| S02 | `684fbcef1590a409a16999f99dbd977922cbcaf40e2374567757629a614fc94e` |
| S03 | `8e9b35903673991f691b5e0d9414470848927c5f87db03e68ba79bddd8afa1bc` |
| S04 | `68a0f5f9b03752baa197cd6e50a42347c4f2d2fd2422583453bc8e63760c1ffd` |
| S05 | `0ad6e29ed963a8f666c6492036d65bdf49e5927e79a41fa4c84f1e040e4a36a5` |
| S06 | `2350211d18cf7a766d62fb34333a4c3e923ed29913888ab95d0c8922a6ad8a5c` |
| S07 | `5013c61f564bafd2b3c68b6a8557c2041d51aa2721a6128e4ab63d3ecc5e184b` |

Historical code-only diff hashes: C `e363eb3317a2d35e1cbf18ac207fd4f018f0febd81029fd8ea3f5487c1fec3cd`; F `4586d191ad39d6e3c676e747acdf42b2bcb2632b4f857381f0e42ca86cc72351`; H `6a117c9153590f23f4edbf86031b0f0de58842bda227efe5d4bd223de8d13da2`. The remaining final diff hashes are in the corresponding corpus records/supplement. These were recomputed from Git, not inferred from final PR file lists.

### Executed corpus checks

Checks below concern sources and frozen fixtures, not candidate performance. Temporary collectors/checkers and raw responses remain in session `28a5997c-c817-48a9-8f00-4a45e40edbf0`, `files`, not in the repository.

| Check actually executed | Observed result |
| --- | --- |
| Complete development reading | All 40 descriptions/final diffs, review-stage code differences and surrounding implementation, issue comments, reviews/decisions, inline replies and resolved/outdated threads inspected. The separate 29-PR mining pass covered 155 paths / 9,196 diff lines and verified 515 historical artifact hashes; parent inspected the remaining 11 packets. No new human-composition provenance established in those 29; unverified/model exclusions retained. |
| `python ...\files\audit_red.py` at 2026-09-22T11:02:57Z | All 40 description hashes and full Git diff hashes matched; 196 paths agree with metadata/files/Git; all original/review commit objects resolve. 115 reviews, 86 issue comments, 48 unique inline comments and 30 threads reconcile; outer/nested cursors exhausted and REST/GraphQL inline identities equal. |
| Dates and supplemental collection | No included development pull update after `2026-09-22T09:28:34Z`; raw original source audit checked prose/edit timestamps too. The six new packets exhaust all REST channels and both GraphQL pagination levels. Separate GraphQL description refetch matches all six bodies and preserves edit metadata. |
| Split isolation | 485 unique inventory PRs / 321 disjoint families; 40 included development / 82 included holdout. Exactly the six self-profile PRs removed from the reserve; all other 94 reserved rows byte-for-byte unchanged from parent revision. No rule, case answer or calibration uses a remaining holdout family. |
| Structural fixture checks | Unique PERF-01..12, all eight ordered group headings, unique DEV-01..20, valid rule/case/packet references, all four outcome definitions and all candidate rows unexecuted. Full SHA/merge-base coordinates for 13 real packets resolve; code-only review diffs hashed separately. Table column counts, fences, relative documentation links and anchors checked. |
| Semantic metric/category review | Conditional mapping covers elapsed/latency/throughput, memory footprint/peak/retained, allocations, GC, generated artifacts/closures, CPU, threads/concurrency, startup/build and workload measures. Reviewed each missing-evidence expectation against its packet; preserved supplied repetitions, worse/flat rows, intentionally changed output, and author-versus-reviewer boundaries. No significance threshold or universal benchmark matrix introduced. |
| Source-link checks | Every cited PR/comment/review URL in the two new documents resolves to the matching acquired API identity/channel, not merely a well-formed URL. Raw benchmark links not supplied by the source remain missing; no claim that unposted results were verified. |
| `git diff --check` | Passed during document construction; no production/test/project/agent/workflow changes. Compiler build/test and formatting sweep intentionally not applicable to this documentation-only sprint. |
| Independent source verification, 2026-09-22T10:58:36Z-11:06:52Z | A separate verifier re-fetched the two human observations and related development comments, inspected original/final code, recomputed historical merge bases, and checked the packet/oracle paraphrases. Confirmed roles, tentative wording, actual fixes, metric boundaries and all 20 packet contents. Found the two corrections below; no new human attribution established. L/M used archived bodies/local Git after fresh GETs became rate-limited; resolved/outdated flags came from complete captured threads, not a refreshed transition history. |
| Post-correction rerun, 2026-09-22T11:13:00Z | `audit_red.py` passed again with the corrected P02 hash, all 20 packet hashes, 20 unexecuted cases, 12 rules, source identities/revisions and unchanged holdouts. `git diff --check` and `git diff --cached --check` passed; staged paths were exactly corpus.md, evidence.md and validation.md in this directory. |

**Pre-freeze correction trail (no candidate executed):** P02 v2 corrects the author's one-hunk shorthand to two diff hunks in one function; its superseded draft hash was `b1db180a161b316c68cccadbcba9ad77e6c26ac355e10b28d3f5c3bf3f36d392`. DEV-03 v2 replaces the draft's conditional `needs evidence` oracle with the named `not applicable` subcheck: P03 never defended manual loops as necessary for speed, and the actual reviewer made a tentative non-gating suggestion. P03's input/hash is unchanged. These are source-validation corrections before initial commit, not adjustments to make a candidate pass. Future input/oracle changes require another explicit version.

## GREEN and post-refactor replay

**Executed 2026-09-22**, from prerequisite commit `1eeae24016d` (RED creation revision/time above unchanged). The [implemented agent](../../.github/agents/performance-improvement-review.md) supplements the existing reviewer; no compiler, benchmark, build, API or test implementation changed. No benchmark or compiler test was executed. The input remains **RED-1, P02 v2, DEV-03 v2**: all 20 packet hashes, 13 historical base/head pairs and 20 oracle rows were checked against the prerequisite commit and remain unchanged.

### Invocation and isolation

The session's callable custom-agent catalog did **not** contain `performance-improvement-review`. All runs used the explicitly permitted **isolated `general-purpose` task fallback**, loading the complete saved candidate instructions; named-agent discovery is **not claimed**. Runtime identity: GitHub Copilot CLI **1.0.87**, **GPT-6 Astra / `gpt-6-astra`**, inherited runtime preference with no model override. Each invocation had a fresh context and one case, never a batch of cases or a prior review.

Each reviewer received the exact candidate snapshot, a rule/metric extract retaining rule IDs, conditional requirements and human/request provenance but excluding historical discussion/resolutions, its exact fenced packet, sanitized source-pin coordinates, and the complete pinned Git diff. Reviewers inspected surrounding implementation/tests/signatures with `git show` at those pins. Synthetic artifacts were explicitly stipulated inputs, not invented real executions. Reviewers were prohibited from reading this matrix, the corpus, full evidence source discussion, other cases/results, later fixes, live PR pages or holdout feedback. Their only write was the review transcript in session storage; there were no GitHub writes or repository changes by reviewers.

Verbose transcripts, candidate snapshots, extracted inputs, full diffs, packet/diff hashes and audit JSON remain under session `cf1b8168-aaf3-4d7a-93ad-058713c498d3`, `files\<run>\DEV-XX\`. Task names are `<run>-NN`; the single output-format retry is `refactor-final-09-retry`. Repository summaries below preserve the actual conclusions and evidence requests without depending solely on that archive.

| Candidate | Exact file SHA-256 | Run directories and UTC interval | Actual invocations |
| --- | --- | --- | --- |
| C0, initial | `0993f6ab4d8efb194805c9dc4b8a04eb091f2a1fa9cc0beabaa7a8416b034d8f` | `green`, 11:28:09-11:42:58 | All 20; calibration failures retained, not declared GREEN |
| C1, calibrated GREEN | `9e1b4a3a13a2242370ecc1eda4246e971a40ad4d06a33303cb89106f87bc6582` | `calibration`, 11:44:01-11:46:53; `green-final`, 11:47:05-11:56:36 | Affected DEV-01/03/08, then all 20 |
| C2, first compaction | `48709f7ac1e7fb04c6ff1c2b343a8e2b3fb0d8d7f184909140424d850d751ed9` | `refactor`, 11:58:23-12:05:50 | DEV-01 through DEV-14; stopped on a reproduced DEV-08 miss, not represented as a full pass |
| C3, final compact candidate | `4c5d4bd0000fac52c0e1e5de9825e95891dd364c8e301200cd7952406c667954` | `refactor-check`, 12:06:44-12:08:59; `refactor-final`, 12:09:16-12:22:19 | Affected DEV-08, then all 20; DEV-09 additionally rerun for Markdown validity |

There were **79 actual invocations**, including failed/partial calibration and the formatting retry, not 79 independent cases. Run starts are packet/candidate freeze timestamps; ends are saved-output timestamps, not benchmark durations. G below means C1's complete `green-final` run; R means C3's complete `refactor-final` run, with the valid DEV-09 retry. The C3 hash is the candidate reserved for subsequent blind evaluation; no reserved feedback was opened to tune it.

### Calibration changes and evaluator decisions

| Observed output | Evaluation and correction |
| --- | --- |
| C0 DEV-08 accepted the consumer comment and explicitly said no producer comment/assertion was needed. | **Miss.** Pin H documents ordering at `applyBrFixups`, not `RecordReqdBrFixups`. The frozen source request targets the producer. C1 distinguishes source equivalence from preserving the producer contract; affected replay and full GREEN caught it. |
| C0 DEV-03 offered an optional comprehension suggestion without an explicit not-applicable comparison; several cases combined sound metric selection with missing methodology. | No invented speed claim, but insufficiently explicit scoped outcomes. C1 separates supported subchecks and unclaimed syntax comparisons; it does not require comparative timing for a mere readability suggestion. |
| C0 DEV-01 promoted a quotation-cache traversal difference to a blocker without establishing a harmful observable result. | **Unsupported escalation.** Pinned code confirms the difference, not its material harm. C1 requires caller/precondition/lifetime/consequence tracing; later outputs request targeted equivalence proof without a blocker. |
| C2 DEV-08 again treated the consumer comment as sufficient; C2 DEV-06 also emitted an unescaped pipe in its table. | C2 was rejected. C3 explicitly makes the producer-contract subcheck `needs evidence` when neither producer documentation nor assertion exists, while permitting a separate equivalence `pass`. The affected replay and all-case replay passed that boundary. |
| C3 DEV-09's first transcript used an unescaped F# pipeline inside a table cell. | Semantically correct but invalid five-column GFM. Preserved as `output-first.md`, SHA-256 prefix `6d531d13ae6c`; fresh isolated retry under the **same C3 instructions**, with a GFM pipe-escaping reminder, produced the accepted output. No expected answer or previous transcript was supplied. |
| DEV-06/07, DEV-11 and DEV-13 discovered additional risks from pinned code. | Evaluator re-read the mechanisms below. These are **source-supported extensions**, not literal matches to every frozen row label and not new human-derived rules. No oracle was rewritten; no historical test failure or observed reproduction is claimed. |

Additional risks were evaluated against code, not against approval/closure status or later fixes:

- **S1, DEV-06/07:** `infos.fs`'s optional/default-less parameter path imports with the current range, but the new comparer ignores range. `MemoizationTable` stores a lazy before forcing it (`Utilities/illib.fs:984-994`); `DiagnosticsLogger.fs:481-498` reports then throws `ReportedError`, which recovery suppresses. Base import caching stores only successful values (`Checking/import.fs:161-167`). The reviewers traced call-site lookup before argument processing and the second same-map/method/view lookup: a failed lazy can replay without the second diagnostic. This is a substantiated error-path scenario, beyond merely asking whether dropping range is safe. See [original pinned cache](https://github.com/dotnet/fsharp/blob/b77c6c6828547768dcdfed1a04347331fdd12e5c/src/Compiler/Checking/infos.fs#L696-L713) and [final pinned cache](https://github.com/dotnet/fsharp/blob/604669d1a6fd688f6ee0cd1daff3e7020192a080/src/Compiler/Checking/infos.fs#L694-L708). No successful-program miscompile or executed failure is asserted.
- **S2, DEV-11:** [pinned import publication](https://github.com/dotnet/fsharp/blob/552a152bbcb7d50c94c3d9d97b49200affc94eb9/src/Compiler/Driver/CompilerImports.fs#L590-L624) initializes pending publication only for the claim owner, while dependency binding later runs only for contexts with pending entries (`2908-2917`). G traced a losing concurrent importer with an uninitialized resolver and non-framework dependency; R traced cancellation/failure between acquisition and publication, leaving a claim that later losers cannot publish. The evaluator confirmed both control-flow paths, including per-build rather than batch cleanup. Preconditions and lack of execution are stated; these are not claims of a measured leak/hang. Which additional scenario a run selects is variable.
- **S3, DEV-13:** [pinned key construction](https://github.com/dotnet/fsharp/blob/86ece46d9e5a7ae67aca866749ae4f377dab5ff1/src/Compiler/Driver/CompilerImports.fs#L533-L574) hashes the reachable set, leaves `CcuName=None` for IL, and omits root identity. Two eligible cyclic IL roots have the same set/key. Last-writer publication (`599-600`) followed by another project's cache hit (`2288`) can return the wrong assembly CCU; `RegisterCcu` uses that returned assembly name (`1503-1510`). Eligibility does not exclude the cycle. This concrete source-derived collision supports risk, without claiming an executed repro.

These extensions do not turn the source corpus into a set of verified historical defects or human objections. The expected missing-evidence checks remain required, and were still found. C1/C3 did not invent blockers in the no-data, structural-only, noise, docs-only or allocation-only controls.

### Durable actual-result matrix

Every G/R review contains the eight ordered groups and separates proof, unknowns and action. The entries below summarize the **scored subchecks**, not blanket verdicts for entire PRs. `G/R` denotes agreement on the described conclusion; differences are explicit. The final column identifies candidate/run and the first 12 hex characters of the actual transcript SHA-256 for audit correlation.

| Case ID | Expected outcome/evidence | Actual outcome and concise finding | Supporting source | Match/miss/unsupported finding | Candidate hash/revision and run metadata |
| --- | --- | --- | --- | --- | --- |
| DEV-01 | G7 `needs evidence`: narrow or justify pervasive remarking against gain | G/R: `needs evidence`; 321 MB/compile (~3.01%) is plausible, not demonstrated significant. Ask for repetitions, spread, protocol/binaries and a narrower approach or shared-remapper justification. Quotation-splice/attribute traversal differences need equivalence tests, not an asserted defect. | P01/A; PERF-02/04/06/08; pinned `Remapping.fs` quotation/attribute paths versus standalone `remarkExpr`; human withdrawal URL in frozen matrix | Match; C0 unsupported escalation corrected | G C1 `ab85da6200f6`; R C3 `19f34d7c8fe0` |
| DEV-02 | G7 `pass`; G2/G4/G8 `needs evidence` | G/R: localized helper reuse `pass`; 557 MB/compile (5.22%) saving is reported. Credit 3 x 4 iterations, 671 passes and output comparisons; request missing dispersion, boundaries, environment/binary/workload provenance and executed optimized quotation/reflected-definition coverage. Resource-tag change alone is not breakage. | P02/B; PERF-02/08/10/11; `Optimizer.fs:3893-3912`, unchanged remapper and resource producer/consumer | Match; no repeated broad-remapper objection | G C1 `56b41a616dff`; R C3 `7f28a864a0f9` |
| DEV-03 | Manual-loop necessity comparison `not applicable`; no timing prerequisite | G: explicit G7 `not applicable` subrow. R: G7 overall `needs evidence` for bundled attribution, but explicitly says no syntax-speed claim exists and that comparison is **not applicable** as an approval prerequisite. Comprehensions remain optional; stable packing is supported within its bounds. | P03/C; PERF-09/10; original `SortTableRows` and callers; tentative human suggestion in frozen matrix | Semantic match; R nests the not-applicable subcheck in the action cell rather than a separate row; no oracle change | G C1 `84610e5b3c45`; R C3 `2266f73bb3cd` |
| DEV-04 | Resolved idiom `pass`; methodology/reproduction `needs evidence` | G/R: comprehensions already adopted, localized mechanisms `pass`; metric choice supported but reproducible table/configuration/error/results missing. No whole-build inference from CPU samples. R additionally asks to expose the descending-fold contract at `IntMap.fold`, separately from the resolved syntax concern. | P04/D; PERF-02/03/07/09; `ilwrite.fs`, `IlxGen.fs`, `IntMap.fold` implementation chain | Match on frozen checks; extra producer-contract request is evidence-seeking, not a blocker or repetition of the comprehension suggestion | G C1 `813eb9808e72`; R C3 `7c68ea2d319e` |
| DEV-05 | Retention metric `pass`; collection/provenance/spread `needs evidence` | G/R: recognize ten-row table and three fresh-process runs. Retained-memory metric `pass`; request collection/root boundaries, spread, environment/binary mapping, commands, and repeated-reader trade-offs. Report 1.53-10.13 MB savings without demanding a time win. | P05/E; PERF-02/03/04/05; per-reader cache/interner changes and existing reader tests | Match; neither table nor repetitions falsely reported absent | G C1 `d1fd4a7687d4`; R C3 `1a3ad6dc283a` |
| DEV-06 | Execute conditional and inspect omitted-range paths; missing proof is not an observed failure | G/R: G8 `needs evidence`, because compile/shouldSucceed never executes the conditional. Successful key guards `pass`; ask for hit/miss allocation and lifecycle evidence. G6 also reports **source-derived** cached-failure `risk/blocker` S1, with a two-range/repeated-check diagnostic test request. | P06/F; PERF-03/05/06/11; `ExtensionMethodTests.fs:848-860`, `Compiler.fs` execution helpers; S1 above | Required concerns caught; supported extension, not literal G6-label match. No fabricated executed failure | G C1 `fb231abf7b39`; R C3 `c9dc818da0ab` |
| DEV-07 | Executing test shape `pass`; metadata/range proof `needs evidence` | G/R: `compileExeAndRun` test shape `pass`, no compile-only accusation. Allocation metric is appropriate; reconcile 477 MB versus ~810 MB and supply repeatability/provenance. Hit ratio is not timing. S1 adds error-path `risk/blocker`, independent of reported successful output identity. | P07/G; PERF-02/03/06/11; final extension test, comparer/lazy/import/error path | Required concerns caught; supported S1 extension, not proof from precedent | G C1 `a559ef0ca5e9`; R C3 `b7e17d0501d6` |
| DEV-08 | Producer invariant `needs evidence`; do not call reversal wrong | G/R: separate current ordering equivalence `pass` from producer-contract `needs evidence`. Prepend plus byte emission establishes distinct descending offsets, including switches; consumer comment does not document/assert the producer obligation. Ask for producer comment/assertion and targeted execution, not a sorting fallback. | P08/H; PERF-07/11; `RecordReqdBrFixups`, byte-buffer advancement and consumer; frozen source request | Match after both calibration corrections; no claim of observed ordering failure | G C1 `d525656ab7d4`; R C3 `5a6708fc01a6` |
| DEV-09 | Producer invariant `pass`; measurement/reproduction `needs evidence` | G/R: producer comment and Debug assertion already satisfy the contract; 139,370 reported comparisons credited, not claimed executed. Report 0.355 ms and 810 KB/compile savings; request repetitions/error, measurement boundaries, binary/environment/commands and targeted results. | P09/I; PERF-02/07/11; producer `ilwrite.fs:1671-1687`, existing branch/switch IL/PDB tests | Match; formatting-only retry under unchanged C3 accepted | G C1 `3edb6313e7a5`; R C3 `99383303316a` |
| DEV-10 | Workload/value and comparative evidence `needs evidence`; no invented speed claim | G/R: build log/two diagnostic tests are not comparative performance evidence. Request workload and before/after data, `AllResults` and range equivalence checks. G additionally traces completion-callback retention; R does not establish a leak. No closure-based rejection or explicit time-win claim is invented. | P10/J; PERF-01/02/05/06; CE lookup/discovery and diagnostic tests | Match; auxiliary risk-assessment scope differs without an unsupported blocker | G C1 `ca73e4d97c09`; R C3 `800258dbf8b0` |
| DEV-11 | Credit 2 MB versus 61 MB isolation; request matched flags/provenance and stale-prose correction | G/R: credit the bounded handover contribution and relevant workloads; ask for matched flags, repetitions/spread, lifecycle and targeted results. Code uses unresolved thunks, not producer fallback. G gives scoped attribution `pass`; R discusses it within G7 `needs evidence` for maintenance justification, not as missing attribution. Different S2 source scenarios add `risk/blocker`. | P11/K; PERF-02/05/06/10; pinned handover tests/binding and S2 publication paths | Frozen concerns caught; R row grouping differs; supported additional risks, no claim all 61 MB comes from handover | G C1 `e807235d4d39`; R C3 `f383830e4961` |
| DEV-12 | Distinguish compilation/runtime/proof shape; preserve N=8 slowdown and closure trade-off | G/R: primary compiler metrics and inspected safeguards `pass`; G4/G5/G8 `needs evidence`. Preserve 2.5->3.8 s at N=8, ask for spread and both-mode emitted-runtime costs. Credit executable assertions; rethrow is compile-only and quotation rendering is not execution. No identical-promoted-IL demand or invented binder miscompile. | P12/L; PERF-03/04/05/06/11; pattern memoization, new guard tests and quotation rendering tests | Match; reported large-input wins are not confused with generated-program speed | G C1 `7a70078438b2`; R C3 `9178866ffff4` |
| DEV-13 | Separate retention/time `pass`; missing numerical spread/retention protocol/provenance `needs evidence` | G/R: credit nine rotated timing samples and whole-solution held-memory data. Preserve Fantomas +0.7% as reported, request numerical spread rather than assert regression. No universal 35.2%/8.9% gain. S3 adds a concrete cyclic-root cache-key `risk/blocker`; no-sharing "costs nothing" also needs support. | P13/M; PERF-02/03/04/05/06; S3 key/hit/registration chain | Required metric concerns match; independently supported code-risk extension | G C1 `7fd4c51df3ca`; R C3 `75acd7a3504f` |
| DEV-14 | G1/G2/G8 `needs evidence`; no invented defect | G/R: "Faster" needs operation/workload, pinned diff, controlled before/after time, repetitions/error/environment/binary provenance, and scoped behavior evidence. Missing resources/code are unknown, not an established defect. | S01; request-required PERF-01/02/12 | Match; no unsupported blocker | G C1 `1930b873dc79`; R C3 `bbac760fbc1f` |
| DEV-15 | Structural/semantic `pass`; elapsed-speed check `not applicable` | G/R: inventories support 12->0 closure classes and executed assertions support values/order/exceptions. Timing/GC policy is not applicable to the count claim; no runtime benchmark demand or measured speed/allocation assertion. Unseen implementation is not certified. | S02; request-required PERF-03/06/11 | Match; legitimate passes and inapplicability | G C1 `db2e3f0d99f9`; R C3 `7d3259f3a691` |
| DEV-16 | G5 `risk/blocker` with causal retention proof | G/R: 100->80 ms latency does not excuse process-rooted append-only ownership. Post-GC 120/220/420 MB versus 20/20/20 and the root trace establish ~1 MB/request unbounded retention. Require bounded/released ownership, repeated retention checks and remeasurement, not merely more evidence of a possible leak. | S03; request-required PERF-05/12 | Match; supported blocker, explicitly synthetic | G C1 `2d0b257b7128`; R C3 `e2d4f3a3bcfa` |
| DEV-17 | G2-G5 `not applicable` | G/R: spelling-only Markdown change has no performance claim or executable/configuration effect. G1-G6 `not applicable`, scope/recommendation `pass`; no compiler, GC or benchmark demand. | S04; request-required PERF-01/12 | Match; no unsupported defect or universal metric demand | G C1 `5472e9f9514f`; R C3 `43c2c87d8e80` |
| DEV-18 | G4 `needs evidence`, no threshold/equality/regression inference | G/R: 0.2 ms/0.2% lower mean with supplied uncertainty is insufficient. Request difference uncertainty/run distributions and practical value. Overlapping intervals alone prove neither equality nor regression; no arbitrary minimum percentage or run count. | S05; request-required PERF-04 | Match; appropriate narrow metric `pass` with significance gap | G C1 `8e312ef6278a`; R C3 `e313052e1270` |
| DEV-19 | G3/G4/G5 `pass`; unchanged time allowed | G/R: 256->128 B/op across 30 stipulated runs supports 50% allocation reduction; timing stays 100 +/- 2 ns/op. Retention/GC/behavior stipulations pass; workload value can still need evidence. No required time improvement or broader speed claim. | S06; request-required PERF-03/05 | Match; supported allocation-only win accepted | G C1 `cae53a267375`; R C3 `c47b10351267` |
| DEV-20 | Runtime metric/semantic `pass`; changed IL permitted | G/R: 280->80 us (71.4%) and 393,680->131,088 B/op (66.7%) support the stipulated generated-program claim. Executed result/order/exception and empty/singleton assertions plus memory/GC checks pass. Different IL is intentional; no compiler-build speedup inferred. | S07; request-required PERF-02/03/06 | Match; positive control remains explicitly synthetic, not a rerun benchmark | G C1 `6dfc951f9d0b`; R C3 `a862dfc08ad8` |

### Compaction and local checks

C1 had **11,290 bytes / 1,397 whitespace-delimited words**; C3 has **9,634 bytes / 1,169 words** (14.7% fewer bytes, 16.3% fewer words). The duplicate gate list/output-group list became one ordered guidance table. Detailed historical provenance stays in the linked evidence/corpus documents. No metric family, metadata requirement, read-only boundary, behavior/lifetime gate, or outcome definition was removed. C2's producer-contract regression was measured and corrected, not assumed away.

| Check actually executed | Result |
| --- | --- |
| Candidate identity/frontmatter | Exact unique `performance-improvement-review` name; only `name` and focused quoted `description`; no model/tool fields. SHA-256 matches C3. Callable catalog lacked the new name, so discovery remains fallback-only as documented. |
| Relative links and references | All ten relative links in the agent and small skill addition resolve. PERF-01..12 remain in evidence; DEV-01..20, packets and source pins reconcile. Markdown links deliberately use URL separators; filesystem operations use Windows paths. |
| Actual review shape | All 20 G and 20 accepted R transcripts have G1-G8 in order, full group names, the four exact outcome strings, and nonempty proof/unknown/action columns. Rejected C2 and first C3 DEV-09 malformed tables are retained as failures, not edited into passes. |
| Semantic checks | Evaluator read actual requests/proof and compared frozen checks. Required concerns and all seven controls were caught. Group-level presentation varies; the matrix records nested subchecks and source-supported risk extensions rather than claiming 40 exact verdict matches. No unsupported blocker remains in accepted outputs. |
| Read-only/holdout boundary | No benchmark commands, compiler builds/tests, code edits or GitHub posts by reviewer instances. No expected matrix, held-back objections, later fixes or reserved feedback supplied. Corpus/evidence files unchanged; existing reserve remains intact. |
| Integration | Skill diff is only the four-line Performance claims heading/reference. Dimension selection, dispatch and non-performance paths are unchanged; no new skill or permanent harness/package. |
| Whitespace/surface | `git diff --check` passed. Only the new agent, skill reference and this validation update changed, all within the allowed five-file surface; source corrections were unnecessary. |
| Product validation applicability | Artifact-only sprint: compiler build/test, `dotnet fantomas .` and product release notes are not applicable under the explicit sprint limits and release-note sink map. Actual reviewer replays and artifact checks above are the validation. |

## Blind holdout freeze

Recorded **2026-09-22T13:22:53Z**, before opening reserved feedback. Candidate C3 SHA-256 is `4c5d4bd0000fac52c0e1e5de9825e95891dd364c8e301200cd7952406c667954`, at repository revision `ac7bd61c998f0bade4aeaa00e676b87bedb6397e`. The evidence table remains at `1eeae24016db78d6236636017992f5ff655c67be`, file SHA-256 `b8e0a424f05c3fdc91f3d9aef900be56142537daaa469811122618ca0bb01ec3`. The historical evidence cutoff remains `2026-09-22T09:28:34Z`; retrieval time does not extend it.

The reserve and category assignments were committed in `ae294143e5890033fcb717956050a91a6f2ed6e8`, before drafting the candidate. The RED correction in `1eeae24016d` moved the entire exposed self-profile family to development. The 20 development packets and their expectations/runs above are preserved. The 94 reserved rows comprise 82 performance candidates, five earlier/context rows and seven real applicability controls; they are **not 94 executed holdouts**. Neither the inline-copy nor the IL-import-retention nor the self-profile family is eligible for independent holdout credit.

The current runtime catalog lists the named `performance-improvement-review` agent. Actual invocation and source acquisition results will be recorded separately; catalog presence and this freeze are not a test pass.

**Isolation correction before holdout execution:** the required complement-boundary document, [expert-reviewer.md](../../.github/agents/expert-reviewer.md), already contains #20388's precise covariant-empty-array failure and repro. Git history verifies that example at the pre-drafting checkout; it was introduced by `fce51f6d26358cb5c2431f84c9884aceba7bb154` (#20511), on 2026-09-11. Its being an existing-agent example rather than human evidence does **not** make the example unseen. Conservatively promote all of `FAMILY-core-empty-array` to development; do not credit #20388 as an independent holdout. No C3 rule changed in response to this discovery. Other selected untouched families must establish behavior/API-neutral coverage.

### Final development regression

**Actually executed:** 20 fresh **named `performance-improvement-review` task** invocations, `final-01` through `final-20`. Packet freeze: `2026-09-22T13:24:57Z`; first completed output: `13:26:17Z` (DEV-17); last: `13:55:55Z`. Runtime: Copilot CLI 1.0.87, GPT-6 Astra (`gpt-6-astra`), inherited configuration, no model override. This is not the earlier fallback run. C3, all 20 RED packet hashes and all historical diffs are unchanged. Every invocation received only C3, the same sanitized rule/metric extract, its own input and pinned diff/source access; no oracle, other review, reserved feedback or later fix.

Raw transcripts and machine-readable identities are outside the repository in session `36233952-cdbf-4056-984e-1fe93e32d7a3`, `files\final-regression\DEV-XX\output.md`. The table preserves scored actual findings; transcript SHA-256 prefixes identify exact outputs. Source pins, full input hashes, expected subchecks and withheld URLs remain in the frozen matrix above. No tests or benchmarks in the PRs were executed; these are reviewer executions.

| Case / input / source | Frozen expectation | Actual C3 result and evidence request | Evaluator result | Transcript SHA-256 prefix |
| --- | --- | --- | --- | --- |
| DEV-01 / P01 / A, #20352 | G7 narrow or justify shared-remapper complexity | `needs evidence`: justify common-remapper maintenance against 321 MB/compile (~3.01%), supply spread/provenance and traversal-equivalence assertions. Quotation/attribute traversal differences are not called a defect. | Match; no unsupported blocker | `e093204886c6` |
| DEV-02 / P02 / B, #20363 | Localized reuse passes; missing reproducibility does not | Scope/helper reuse `pass`; credit 3 x 4 iterations, 671 reported passes and 557 MB/compile. Request raw variability and actual binaries/configuration. Resource-name change alone is not breakage. | Match | `25cddffd5dbc` |
| DEV-03 / P03 / C, #20348 | Syntax-speed comparison not applicable | Explicit `not applicable` comprehension-versus-loop subcheck; bounded stable sorting and current producer ordering `pass`. Code-only slice needs measurements, not proof that a syntax preference is faster. | Match | `5e12bdfb31fe` |
| DEV-04 / P04 / D, #20348 | Recognize adopted idiom; ask for methodology | Comprehensions already present; no comparison prerequisite. Scoped equivalence/scope `pass`; metadata, spread and targeted results `needs evidence`. Separately requests producer-boundary ordering documentation. | Match; no stale syntax finding | `0412c81e262b` |
| DEV-05 / P05 / E, #20261 | Retention metric supported; protocol/spread missing | Credits ten rows and three fresh-process runs; reports 1.53-10.13 MB differences without demanding faster time. Requests collection/root boundaries, matched binaries, numerical spread and repeated-reader trade-offs. | Match | `78b6fe810afb` |
| DEV-06 / P06 / F, #20353 | Execute conditional; inspect omitted-range error paths | Compile-only conditional `needs evidence`; successful key partition/scope `pass`. Source-derived failed-lazy diagnostic suppression adds S1 `risk/blocker`, with two-location/repeated-check assertions requested. | Required checks caught; same supported S1 extension, not an observed failure | `87803be25be2` |
| DEV-07 / P07 / G, #20353 | Recognize executing test; retain measurement/range gaps | Executing harness `pass`; reconcile 477 MB versus ~810 MB and identify boundaries/repetitions. Hit rate is not time; same S1 failure-caching scenario adds `risk/blocker`. | Match on frozen concerns; S1 persists independently of test-shape repair | `f32e0cad700c` |
| DEV-08 / P08 / H, #20349 | Producer contract needs evidence; reversal not inherently wrong | Current reverse/sort equivalence `pass`, including ties and empty/singleton cases; producer documentation/assertion `needs evidence`. Consumer comment is insufficient for that separate contract. | Match | `165db233c047` |
| DEV-09 / P09 / I, #20349 | Stop resolved producer-contract objection | Producer comment/Debug assertion `pass`; credit reported 139,370 comparisons. Request repetitions, boundaries, binary/environment provenance and targeted results; no repeated missing-contract finding. | Match | `d13ceea52912` |
| DEV-10 / P10 / J, #19689 | Request value/comparison, not fabricated speed/rejection | Build logs and two passing diagnostic tests are not performance comparisons. Requests workload, `AllResults`/range equivalence and callback-lifetime evidence; establishes neither leak nor measured slowdown. | Match | `35e4c9f83176` |
| DEV-11 / P11 / K, #20416 | Credit contribution isolation and resolved fallback; ask for metadata | Explicit attribution `pass`: about 2 MB handover, not all 61.04 MB. Correct unresolved-thunk behavior credited. Requests matched flags/spread/lifecycle results; S2 abandoned publication claim adds source-derived cancellation `risk/blocker`. | Required concerns caught; supported S2 extension | `16460db896c5` |
| DEV-12 / P12 / L, #20244 | Keep small-input slowdown and both output modes | Credits large-input compiler benefit and executable assertion shapes. Retains N=8's 2.5->3.8 s and unoptimized closure trade-off; requests variability and emitted-runtime cost. No identical-IL or measured runtime-win claim. | Match | `f131e7e62ef4` |
| DEV-13 / P13 / M, #20296 | Separate retention/time; preserve flat/worse workloads | Credits nine rotated samples; Fantomas +0.7% is reported inside spread, not an established regression. Requests numerical spread, retention/reproduction/lifecycle data. S3 cyclic-root identity adds `risk/blocker`; API-version policy is a separate evidence request. | Required concerns caught; supported S3 extension | `816e5db31280` |
| DEV-14 / S01 | Weak prose needs evidence, not a defect | All groups `needs evidence`; name operation/workload, comparable metrics and implementation. No invented leak, regression or correctness blocker. | Match | `09ab19e0809c` |
| DEV-15 / S02 | Structural/semantic proof passes; no timing prerequisite | Stipulated 12->0 closure classes and executed behavior assertions `pass`; runtime timing not required. Missing user value/diff/provenance limits broader endorsement only. | Match | `b0a13d754251` |
| DEV-16 / S03 | Demonstrated retention regression is material risk | G5 `risk/blocker`: root-traced append-only ownership plus 120/220/420 MB versus flat 20 MB, about 1 MB/request. A 20% time improvement does not excuse it; require bounded/released ownership. | Match; synthetic, not human precedent | `f8fe4b459539` |
| DEV-17 / S04 | No-claim docs have no performance metric gate | All groups `not applicable`, with no compiler/GC/runtime measurement request. Accepts spelling-only stipulated scope without claiming an underlying patch was inspected. | Match on required inapplicability; harmless G7/G8 presentation differs from earlier runs | `0ea08661dd35` |
| DEV-18 / S05 | Noise-level claim needs evidence, no threshold | G4 `needs evidence`: 0.2 ms/0.2% difference requires difference uncertainty and practical value. Overlapping intervals prove neither equality nor regression. | Match | `a2806ba6a10b` |
| DEV-19 / S06 | Accept allocation-only win despite flat time | G3/G4/G5 `pass`: 256->128 B/op across 30 stipulated runs; no demand for a timing improvement. Remaining value/diff questions do not invalidate that claim. | Match | `89fee4b3f9aa` |
| DEV-20 / S07 | Accept runtime/semantic proof with changed IL | Runtime/behavior/resource checks credit stipulated 280->80 us and 393,680->131,088 B/op. Intentional IL changes accepted; missing broader value/implementation/method details do not become blockers. | Match | `18b292ee9980` |

**Evaluator score:** 20/20 cases meet their frozen semantic acceptance checks, zero missed required concerns, zero unsupported blocker findings. This is a small development regression suite, **not independent holdout precision/recall**. The outputs contain 224 scoped table rows: 85 `pass`, 116 `needs evidence`, 17 `not applicable`, six `risk/blocker`. Six risk rows represent five case-level findings in DEV-06/07/11/13/16; DEV-16's final recommendation repeats its G5 finding. These reduce to four mechanisms (S1 in two revisions, S2, S3, synthetic retention), not six independent bugs.

The evaluator checked the new explicit caller/precondition/consequence traces against the S1-S3 mechanisms documented above. The new S1 example is a nongeneric declaring type's generic method with an optional parameter but no metadata constant; it distinguishes the existing first-call import error from the new suppression on a cached failure. S2 states cancellation after successful construction but before publication, and the observable loss of sharing on later retries until clearing, **not** a fabricated hang or leaked CCU graph. S3 states eligible mutually referencing IL roots and loss of one assembly's type environment. No oracle was revised and C3 was not repaired.

All 20 saved tables actually passed the ordered full-name G1-G8, five-column, nonempty proof/unknown/action and exact-outcome checks. Diff hashes matched the frozen inputs. The earlier 79-invocation archive was also re-audited successfully, including its preserved rejected C2 formatting failure; no old output was relabeled as this run.

### Maintenance procedure

Freeze a new date window and exact cutoff before discovery; retain source bodies, pagination totals/cursors, edit/provenance metadata and base/head/review revisions. Normalize related attempts and stacks by PR family, quarantine automated or uncertain prose, and reserve fresh families before drafting. Add the input and source-backed oracle **before** changing a rule. Freeze candidate/evidence/input hashes, execute each case in an oracle-free context, and record actual output, invocation identity and evaluator misses/unsupported findings. A rule change prompted by a holdout promotes that family to development and requires untouched replacements. Rerun the complete cases, check links/discovery and split/coverage accounting, and retain raw transcripts outside the repository. Do not convert unavailable categories or synthetic controls into real-human holdout credit.

## C3 blind execution and retained failure

The first blind execution is **not an acceptance pass**. On 2026-09-22, ten fresh named `performance-improvement-review` invocations (`blind-c3-01` through `blind-c3-10`) used the unchanged C3 and sanitized rule extract above. Runtime: Copilot CLI 1.0.87, GPT-6 Astra (`gpt-6-astra`), inherited configuration, no override. Outputs were saved from `14:23:10Z` through `14:29:29Z` in session `c8f07943-58a7-4cdd-9a76-ce0b8d398079`, `files\blind-c3-NN.md`. Each context received one historical packet, full diff and pinned source access; no oracle, source report, withheld feedback, later fix or other result. The frozen evaluator oracle predates these runs: SHA-256 `ddfc494f450358cf527cdbf08c45066d9fca111efc84aa2aa74b8399400f59dd`, in the preceding session's `files\holdout-study\oracles.md`.

**Independence correction:** HOLD-01/02 (#20388) are development regressions, not holdouts. Its final ancillary closure-characterization change also connects `FAMILY-inline-closures`; conservatively treat HOLD-03 (#20367) and that entire family as development. This overrides the earlier source collector's eight-connected-family holdout count. The initial execution therefore contains **three development cases in one connected family, and seven blind cases in seven other families**. Account-authored but unverified prose and explicitly model-attributed feedback do not become human precedent merely because the reviewer agrees.

### Frozen source expectations

These are compact transcriptions of the pre-execution oracle, not expectations inferred from output. Source composition remains uncertain unless explicitly model-attributed below. All ten cases are real-PR **request-origin controls**, not ten verified human objections. `H` identifies the exact input revision in the identity table below.

| Case | PR/family | Coverage | Withheld source or request-origin control | Input available at reviewed stage | Expected outcome and evidence request | Prohibited unsupported finding |
| --- | --- | --- | --- | --- | --- | --- |
| HOLD-01 | #20388 / core-empty-array; development | API-neutral, behavior, allocation | [Covariance comment](https://github.com/dotnet/fsharp/pull/20388#discussion_r3969367913), attached to an [explicit AI review](https://github.com/dotnet/fsharp/pull/20388#pullrequestreview-5155443411); request-origin compatibility oracle | Historical code-only full diff, old signatures, singleton tests; no later description | `risk/blocker`: empty covariant `string[]` passed as `obj[]` loses runtime type in copy/conversion, making a formerly successful downcast fail. Request covariance/runtime-type and freshness-contract coverage. | No measured speed regression or claim nonempty copies all fail. |
| HOLD-02 | #20388 / core-empty-array; development | Mixed-scope, API-neutral, behavior | Same compatibility control; [documentation concern](https://github.com/dotnet/fsharp/pull/20388#discussion_r3990215068) and [resolution](https://github.com/dotnet/fsharp/pull/20388#discussion_r4005486031), uncertain composition | Final description, nine-file diff, guarded copies, revised signatures, runtime-type tests | Specific repair `pass`: runtime-type guard, four copy/conversion cases and explicit empty sharing. Quantified allocation/nonempty cost `needs evidence`; distinguish ancillary test edits and obsolete opt-out description. | Do not repeat the earlier covariance defect or claim unchanged fresh-array docs; no attribution of all files to the win. |
| HOLD-03 | #20367 / inline-closures; development | Generated artifacts, compiler allocation | [Author description](https://github.com/dotnet/fsharp/pull/20367); empty approval supplies no substantive human oracle | Full inline/signature diff; sampled 240 + 172 MB to zero; +1.5 KB DLL; deterministic-output assertion | Surgicality/control flow `pass`; sampling, repetitions, variability, binaries/input/environment/commands `needs evidence`. Distinguish sampled zero, closure elimination, compiler DLL growth and output identity. | No mandatory speedup for an allocation-only claim, leak inference or universal identical-IL gate. |
| HOLD-04 | #20544 / string-concat | Mixed-scope, no-claim applicability | [Description](https://github.com/dotnet/fsharp/pull/20544) explicitly attributes generation to Claude; request-origin control | Complete 30-file literal cleanup and compatibility context | Unclaimed speedup/benchmark gate `not applicable`; separate shipping constants from docs/tests/scripts. Verify referenced Core/SDK availability without asserting a failure. | No assumption all `.fs` changes execute or every changed path makes shipping code faster. |
| HOLD-05 | #20444 / tc-reuse-tests | Test-only, no-claim applicability | [PR](https://github.com/dotnet/fsharp/pull/20444), author **DedSec256**, not auduchinok; reviews uncertain/model-associated | Eight structural-edit cache-characterization tests, utility and cache-key context | No shipping benchmark requirement; purposeful executing assertions `pass`. Recognize explicit output name isolates assembly-name invalidation; implicit-name full recheck is characterization, not a new regression. | No claimed compiler speedup, newly fixed invalidation or universal runtime metrics. |
| HOLD-06 | #20436 / prompt-docs | Docs-only, no-claim applicability | [PR](https://github.com/dotnet/fsharp/pull/20436); empty approval is not a rule | Complete seven-Markdown-file guidance diff and description | Performance metrics `not applicable`; inspect textual scope/consistency without executing embedded prompt instructions. | No compiler speedup, benchmark requirement or fabricated source/runtime change. |
| HOLD-07 | #20119 / vs-work-reuse | IDE latency, API-neutral, behavior, lifetime | [Retention concern](https://github.com/dotnet/fsharp/pull/20119#discussion_r3707051130) belongs to an [AI review](https://github.com/dotnet/fsharp/pull/20119#pullrequestreview-4847803981); [race withdrawal](https://github.com/dotnet/fsharp/pull/20119#discussion_r3764799831) has uncertain composition | Final one-file diff/body, reactor, delayed-reader ownership and callers; no later replacement | `risk/blocker` for per-version strong dictionary with no per-version eviction; request repeated-edit/close retention proof. CPU/latency and hit/invalidation evidence missing; distinguish dependent version from description. Mailbox serialization defeats the alleged first-use race. | No observed OOM, unsupported race or assumption later author prose repaired this head. |
| HOLD-08 | #20587 / ci-turnaround | Build time, parallel-resource trade-off | [PR](https://github.com/dotnet/fsharp/pull/20587); no captured feedback | Linux matrix/template, full splitter and SDK-integration context | Turnaround `needs evidence`: comparable critical-path/job timings, repetitions/spread, queue/capacity, batch balance and total compute; verify equal coverage and SDK integration once in batch 1. | No invented twofold win, skipped tests, hung pipeline or universal runtime allocation measurements. |
| HOLD-09 | #18509 / vector-sum | Generated-program runtime, tables, behavior, older-active | [Modern runtime without SIMD request](https://github.com/dotnet/fsharp/pull/18509#issuecomment-3188320978), earlier 2025 context, composition unverified; [overload question](https://github.com/dotnet/fsharp/pull/18509#discussion_r2155137460) at `20d258fcb6bb5fa01408483c7a4928596adcf46f` | Final sum-only diff, author tables with units/error/environment, separate Framework results, tests/harness | Credit strong tables. Request **same modern runtime with hardware intrinsics disabled**, non-array sequence shapes, precise job/binary/revision mapping, final reproduction, arithmetic/exception proof and public-helper justification. | No stale average exception or direct-LINQ Framework slowdown charged to the final fallback; closure is not rejection proof. |
| HOLD-10 | #20507 / rich-text-api | auduchinok, API-addition, no-claim applicability | [PR](https://github.com/dotnet/fsharp/pull/20507); empty body, robot-marked/empty approvals | Four-file immutable display-context API addition, signature/surface baseline and consumers | Metrics `not applicable`; additive immutable wiring `pass`; request true/false/default formatting assertions without inventing failure. | No API-neutral label, invented speedup/global mutation or benchmark gate. |

### Actual outputs and evaluator comparison

All inputs below use C3 and the frozen pre-holdout rules. Packets live in preceding session `36233952-cdbf-4056-984e-1fe93e32d7a3`, `files\holdout-study\inputs\HOLD-NN`; `suite.json` additionally hashes every source file and coordinate manifest. HOLD-01..09 were frozen at `2026-09-22T13:38:40Z`, HOLD-10 at `13:55:31Z`. HOLD-08's subsequently supplied pinned TestSplit context is recorded in the pre-execution manifest; no oracle text was supplied. Historical diffs are reproducible from the merge base/head below; compact claims and exact required evidence are preserved above rather than relying on archive paths alone.

| Case | Head / historical merge base | Input SHA-256 / full diff SHA-256 |
| --- | --- | --- |
| HOLD-01 | `1748e9e10df376dfbe2913c9a1f5bd639604aeff` / `ca80019e9dddd2b31b1e6de72eec0bf669b828c3` | `48032622dad14f7b8c830431c2414e205d1d6fe74d141cee7fe5a4aecd364aab` / `fbdddc57b9539a9e93fe577985157a22f28b98daeac3a41f7c6e5897942c046c` |
| HOLD-02 | `cd3b1fa9127c5e391e25d804355ab5e11b2cb67f` / `c9213bc6f2ebae94a59d563319483c6ce8c527c7` | `253b3c16f06add382385915be0017d967fe1363b8945a638b2b9372c96945271` / `2a3499ecc47460d8d8488d8e2f62ace51ea32229aa18c5132940bc14093d1828` |
| HOLD-03 | `2951feb63ba84fe6cb61e8066614c7a8d9628001` / `e631ed7f4f210cb3dade65b1d6341bb7106e51bb` | `bbb310d0c7fc8b97f29daf1c31a6f3f0b39997669cdd855bda426cacaa4eb9ae` / `357c13060e6546c625af9ae4c9910641a2d511f3df562203fac465d9fd0b7562` |
| HOLD-04 | `65e050d94d470213b55910dd7eccaad27ec79f5e` / `17f8eb7d0691d30d6fc0dbcc7977ac15f9e7d5d2` | `850ca9381f8c8778063a030b904fc4ea91fccfd5ab3f98e9b9151fb0806118dd` / `fb30d3b9542d84dbbfbd8cf1156a34ce3bcb1550f7fe6f4b1402361527ff7bcb` |
| HOLD-05 | `b12cb3628a924d5ac8e5d77295aa805b662c63b6` / `a29233c3e369b7fd1fd6727b5ea7fad656f12e0d` | `5059588b0a45d762a37083ed40bcb20fc5b8263746a99088517afd2200cfecdc` / `6683ed3f9acbf0f51307ecafefb4e9d2d466ebb530eb9c9ba1a678b8c5c153d8` |
| HOLD-06 | `8576a9b2c6eb8b80e95f050c6b5fb685f5166146` / `ca80019e9dddd2b31b1e6de72eec0bf669b828c3` | `fa2f3b3622ee415d8208de8163f8fd0ef1ef6497f2c829388107b42055282322` / `e80f60dab25f1fe3dec798c30f8196e9c63915d2fa3be888e129e23801fdafdc` |
| HOLD-07 | `25f732737ad94832d2e56dd6b8d133531d3e0a31` / `20382698ea0985a650248838dc5a21e50c6203d9` | `2946fad8082fb7a7ebc09d031b98a2004f904afb674d1176b01f9f89cfbdd741` / `152b75f9bf13ca41186104e63f2190d5a9c730646880b7a9dfe8357b777f2590` |
| HOLD-08 | `a4e67d6eb582ae21b2718a20f6408ffa225fcec1` / `b561c33dbf43796cd3e342223b8212c009053d40` | `0df2f71c513d06997c2ed62eca9ddfb29e0828eadeb3a3b65f0f6579d628cf35` / `42af43b1a18b7507973b99f0e42ba9a34d30f5e58b70e7030624d457dd103520` |
| HOLD-09 | `7e5c5d16de76ecc0b87f8da9bed6b07438d65573` / `e84544a66c3085efd3e3f1eee00beb77708dd8f3` | `d10258f7b2962daf4e1b2fd70bd7c579269b76be5f17898efbb413859d31a993` / `474a03b49c82fc9080321043724666ab9a6d6acd35ccf15e8338e770ebf0dcee` |
| HOLD-10 | `ece9ed9c27947a3e4297e7614266d960424bc542` / `4bb6faf078b467ee53ab6006e47b928a94c65cf1` | `9f7281bb98ede2c73036d941255ac39a7513b6489e7865c271e6b8e3ef0ee975` / `876511990b2ca6de973bc74a574853b9012d5f75ecfffd4104c2e69bb557d6d6` |

The evaluator read the outputs separately from reviewer execution. Exact transcript hash prefixes below refer to unedited saved output, not hand-authored replacements.

| Case | Actual checklist result/finding and requested evidence | Match/miss/unsupported finding | Transcript SHA-256 prefix |
| --- | --- | --- | --- |
| HOLD-01 | G6 `risk/blocker`: `Array.copy<obj>` changes runtime `string[]` to `obj[]`, causing `InvalidCastException` on downcast; traces Seq and insertion paths. Requests covariance and freshness-contract coverage; no measured regression asserted. | Match, development only | `269ff0b62d1ee` |
| HOLD-02 | G6 repair `pass`; credits exact-type guards, four runtime-type/independence tests and revised sharing docs. G2 flags absent `zeroCreateUncheckedNonEmpty` and overbroad "allocate nothing"; requests focused allocation/nonempty checks and actual results. | Match; no stale blocker | `b88acd6ba0deb` |
| HOLD-03 | Credits narrow target, control flow, metric and surgicality. Distinguishes sampled zero from elimination and allocation traffic from retention; requests structural attribution, binary/workload/variation/profiling evidence and bounded overhead comparison, not a speedup. | Match, development only | `c597d4a78e5e` |
| HOLD-04 | No quantitative metric gate; identifies three shipping constant `ToString` changes versus examples/scripts/tests. Requests resolved Core identity/smoke evidence for standalone invocations as `needs evidence`, not failure. | Match; no unsupported blocker | `751b245f456a` |
| HOLD-05 | Credits Started-event assertions, implicit-name invalidation and executing workflow; treats missing targeted run results separately. No production optimization, speedup or runtime-benchmark prerequisite. | Match | `d22d32a8a089` |
| HOLD-06 | G1-G5 `not applicable`. G6/G8 request clarification of changed model-selection policy against the no-behavior-change description; no benchmarks or compiler tests requested. | Match; textual consistency request is supported by the diff, not a runtime blocker | `c0270f8281d6` |
| HOLD-07 | G5 `risk/blocker` traces strongly retained historical versions/readers; explicitly avoids first-use race because mailbox awaits creation. Requests CPU/latency, dependent-version/retry proof and helper reuse. Also reports a project-removal cleanup gap. | Match; additional removal scenario independently source-verified below, not inferred from agreement | `42f3d34d1a1f` |
| HOLD-08 | G1-G5 `needs evidence` for critical-path/queue/job timings and aggregate agent-minutes; credits source partition, once-only SDK integration and template reuse. Requests actual runner union/count/results without inventing skipped tests. | Match | `4c500d93cf315` |
| HOLD-09 | Credits existing timing/error/environment tables and clear array contrasts, preserves noisier list cases, recognizes Framework fallback and removed averages. Requests other types/short/lazy cases, binary mapping, arithmetic/exception/API evidence. **Does not request the same modern runtime with intrinsics disabled.** | **Miss: one required measurement condition.** Generic runtime coverage and Framework fallback are not a substitute. Original output retained; no retroactive oracle relaxation. | `f3313b85f413f` |
| HOLD-10 | G1-G4 `not applicable`; immutable additive wiring and surface declaration `pass`; requests explicit/default formatting execution only. | Match | `fa0391c0f8c3` |

The first-run semantic score is **9/10 matches, one miss**; among initially eligible independent cases, **6/7 matches, one miss**. This is not a precision/recall estimate or final acceptance. The seven blind families cannot be silently relabeled as fresh if subsequent tuning uses their results. No verified-human-composition credit is added to the evidence rules by this run.

**Additional finding verification:** at HOLD-07's head, an F# options request constructs a C# dependency's delayed reference before the parent can bail out (`FSharpProjectOptionsManager.fs:416-439,475-479`). This inserts an emit-cache entry without a C# options-cache entry or successful emission. `ProjectRemoved` posts `ClearOptions`, but removal is guarded by options-cache membership (`:588-596,653-659`); the sweep enumerates only `lastSuccessfulCompilations` (`:479-505`). Thus ordinary removal/sweeps can leave the strong entry rooted. Global/solution clearing releases it; a later successful delayed callback could also make it eligible for sweeping. This supports the narrow cleanup finding, **not** an observed leak size, OOM, or assertion every retained entry still owns a compilation. The independent source evaluator did not read the candidate output.

The ten saved outputs passed actual ordered/full-name G1-G8 and five-column/nonempty-proof/unknown/action checks, using only the four permitted outcome strings: **106 scoped rows = 42 pass + 46 needs evidence + 16 not applicable + 2 risk/blocker**. The two risk rows cover covariance and historical-version retention, with the additional supported removal subcase; **zero unsupported blocker findings** were established. The ten input file-set hashes and all twenty preceding final-regression transcript hashes/shape checks also passed. Output length ranges from 1,017 to 2,110 whitespace-delimited words; tables are structured but not uniformly terse. Shape checks do not erase the semantic miss.

### Coverage and remaining acceptance gaps

| Requirement | Actual executed evidence | Qualification |
| --- | --- | --- |
| Mixed scope | HOLD-04 separates executable constants from docs/tests/scripts; HOLD-02 separates ancillary tests | HOLD-04 was blind; HOLD-02 is exposed development. No attribution of all paths to a win. |
| Test-only | HOLD-05 recognizes event-based cache characterization and executing assertions | Real PR, no performance claim; no runtime benchmark gate. |
| Benchmark-only | No qualifying real PR found in the [804-PR path screen](corpus.md#benchmark-only-applicability-search) | **Unavailable, not verified.** Product-plus-benchmark #18509 and synthetic cases do not fill it. |
| Docs-only | HOLD-06 excludes performance metrics while identifying a textual policy/description mismatch | Real no-claim control. A documentation-only numerical-performance assertion has no real selected example. |
| Generated code/artifacts | HOLD-03 distinguishes compiler closure allocation from runtime speed; HOLD-09 separates generated-program sum paths and output behavior | HOLD-03 is development; HOLD-09 has the retained fallback-measurement miss. |
| API-neutral / behavior-sensitive | HOLD-07 traces lifecycle and cache-hit/diagnostic equivalence despite unchanged signatures | Real blind control with supported source-derived retention risk, not executed failure. HOLD-10 is explicitly an API **addition**, not neutral. |
| Strong tables / prose-only gaps | DEV-05/13 and HOLD-09 credit supplied tables; DEV-14, HOLD-08 identify specific missing evidence | Earlier author measurements are not rerun measurements. No universal numeric threshold. |
| Meaningful versus noise-level wins | DEV-18 preserves uncertainty; DEV-19 accepts flat-time allocation saving; HOLD-09 credits large array effects without generalizing noisy list data | DEV-18/19 are synthetic contract controls. Real tables remain author-reported and provenance-qualified. |
| Hidden resource regression | DEV-16 detects stipulated retention despite faster time; HOLD-07 independently traces real strong ownership | DEV-16 is synthetic; HOLD-07 does not invent numeric retained bytes or measured OOM. |
| Allocation/closure improvement | DEV-15/19 and HOLD-03 distinguish counts, allocations, time and live memory | Synthetic positive boundaries versus real author-sampled closure claim are not interchangeable. |
| Compiler-time versus runtime | DEV-12 retains small-input compiler slowdown and unoptimized closure cost; HOLD-09 evaluates sum runtime separately | No compiler benchmark is substituted for generated-program execution. |
| Withdrawn / narrower scope / behavior proof | DEV-01 preserves #20352's author withdrawal and narrow-scope request; DEV-03 preserves tentative #20348 simpler-idiom suggestion; paired DEV-06/07 and DEV-08/09 recognize repairs | Two verified human-derived families in development, not new holdout human votes. Closed #18509 is not treated as a rejected optimization. |
| Complete final regression | Twenty unchanged C3 development replays pass; ten additional C3 executions yield nine semantic matches and one miss | **Not a final acceptance pass.** No repaired-candidate replay or replacement-holdout success is implied. |

The conditional contract still includes time; total/peak/retained memory; allocations; closures/generated artifacts; CPU; GC; threads; startup/build costs; behavior/API/compatibility; lifetime; surgical isolation and reviewability. C3 requests before/after units, commits/configurations, workloads/input sizes, repetitions, meaningful error/spread, environment/tooling and commands/links, with adjacent metadata permitted. Actual controls show these are conditional, not a universal metric matrix. No production build, test run, formatting sweep or release-note entry is required for these documentation/prompt paths.

## C4 repair and replacement holdout

C3's HOLD-09 output is retained above. The sole prompt repair adds a conditional PERF-03 sentence: compare representative supported capability-dependent fast/fallback paths on the **same runtime**; another runtime's results do not establish fallback cost. No metric, outcome, group, behavior gate or provenance label was removed. The old sanitized rule extract is unchanged; the full C4 instructions supply this additional requirement.

The source evaluator independently confirmed the original oracle, without reading the reviewer output: [T-Gro's request](https://github.com/dotnet/fsharp/pull/18509#issuecomment-3188320978), `2025-08-14T12:42:51Z`, explicitly names .NET 9 with `DOTNET_EnableHWIntrinsic=0`. At final `7e5c5d16de76ecc0b87f8da9bed6b07438d65573`, `array.fs:1581-1614` and `seq.fs:1467-1500` choose the old loop on Framework but LINQ on modern runtimes. Disabling intrinsics on .NET 9 therefore does not select the Framework fallback. The [author's later assertion](https://github.com/dotnet/fsharp/pull/18509#issuecomment-3477773230) refers to a [.NET Framework 4.8.1 table](https://github.com/dotnet/fsharp/pull/18509#issuecomment-2834590096), not the requested modern scalar path. The oracle was not relaxed; source reinspection confirms a candidate miss rather than an oracle mistake. These are earlier, composition-unverified sources, not additional human-derived votes.

| Frozen artifact | Identity |
| --- | --- |
| C4 candidate | SHA-256 `b2625d5c24ee35919f881095ff6336d47a28bfb533228aab4d8642fefda9e132`; frozen `2026-09-22T14:40:05Z` |
| Repository coordinate | `ac7bd61c998f0bade4aeaa00e676b87bedb6397e` plus the explicitly hashed working-tree candidate/evidence changes, not a claim that C4 was already committed there |
| Evidence table including post-C3 correction | SHA-256 `7d620d26c62703fcb6d9e4cc453b64ded8f0cb64c94df50fbde2b71b06c9c237` |
| Sanitized rule extract | SHA-256 `60874a1849722ee4369a839d8a431565069f98aa7588f044582ccf4f3a485e14` |
| Archive | Current session `c8f07943-58a7-4cdd-9a76-ce0b8d398079`, `files\c4-regression`; pre-repair snapshots and outputs remain untouched |

**No holdout laundering:** #18509 and all of `FAMILY-vector-sum` become development. HOLD-01/02/03 remain exposed development. HOLD-04/05/06/07/08/10 are retained evaluation cases whose findings did not tune C4; their C4 runs are explicitly replays, not first-seen holdouts. A new family, `FAMILY-string-spans`, supplies the replacement for the affected **conditional fast/fallback generated-program measurement** category. It does not supply a second independent SIMD-disabled example; that narrower subcategory remains unavailable in the source-screened shortlist.

### REPL-01: independent source oracle and actual result

Metadata-only reservation of #20358 predates candidate drafting. The source evaluator selected it from the predeclared #20358/#19804/#20275 shortlist based on the code/claim, not candidate behavior. Its string-construction mechanism is distinct from vector sum, empty-array sharing and inline closures. A common broad optimization umbrella issue does not make those changes duplicate mechanisms; that link was disclosed to the evaluator and omitted from reviewer navigation. No REPL-01 feedback or expected answer informed C4.

| Case / PR / family / revision | Coverage and source | Frozen expected outcome and required evidence | Prohibited unsupported finding |
| --- | --- | --- | --- |
| REPL-01 / #20358 / FAMILY-string-spans / `5aa9391f43d7e533d9815b4fe9245df4881833f8`, base `e631ed7f4f210cb3dade65b1d6341bb7106e51bb` | Generated-program library allocation, capability/input-dependent paths, API-neutral behavior. [Author description](https://github.com/dotnet/fsharp/pull/20358) claims heap-allocation reduction on .NET Standard 2.1+; three comments, no reviews/threads. Request-origin control; no verified-human precedent. | `needs evidence` for actual Core asset/binary/configuration and before/after allocations with relevant time/stack trade-offs, repetitions/spread and commands. Distinguish `NETSTANDARD2_1_OR_GREATER` from the separately shipped net asset; a modern runtime does not prove the span path ran. Cover map/mapi sizes; filter around 512 and 40,000 with varying selectivity; replicate short/general branches around counts 4/5. Credit unchanged signatures and existing null/empty/callback/Unicode/large tests, but request new boundary/executed-asset proof. | No all-size/allocation-free or compiler-speed claim; no SIMD-toggle requirement; no invented pointer escape, corruption or measured stack failure. Source guard/coverage uncertainty is not an observed test failure. |

Input text SHA-256: `9fec7df157b5c59c3c20abf10acfb304980477613ebe993bacc09b7a13b322fd`; complete diff SHA-256: `6efc6f9d9c230e223ad89b91a80270ccb0997de113528d1db0b17aec83a4044b`; evaluator-only oracle SHA-256: `e8a0f40ff5f622ef3995ca0554ff07baca5299f9476f8c35e155799e4d08ebe6`. Canonical UTF-8/LF input/oracle bytes match the independently prepared payloads; initial Windows CRLF materialization was normalized **before execution**, without changing content. The source bundle and all fourteen reviewer-member hashes were frozen at `2026-09-22T15:01:02Z`, `files\replacement-study\freeze.json`; the oracle is stored separately. The bundle contains complete base/head changed files, unchanged signatures, NativePtr implementation/signature, existing String tests and build/test asset routing, not discussions.

**Actually executed:** fresh named `performance-improvement-review`, task `c4-repl-01`, same CLI/model as the other runs; only C4, the sanitized rule extract, the single frozen reviewer bundle and access to its pinned Git objects were supplied. Saved output SHA-256 `6a3d04e9709e0e13db50bd9cb607ef1444addd663450ea44263b7fa24fe0c304`, `files\c4-regression\REPL-01.md`.

**Actual finding:** the reviewer identifies the exact temporary-buffer eliminations and unchanged larger-filter/short-replicate paths; requests B/op by branch and same-runtime identified binaries, warmup and repeated-run metadata. It catches the `NETSTANDARD2_1_OR_GREATER`-only guard, separate shipped-net asset and modern unit-test routing, distinguishing surface-area loading from behavioral execution. It requests threshold/selectivity/count tests, callback/overflow proof and fixed 1,024-byte stack-budget justification, while crediting existing null/empty/side-effect/Unicode tests and source-level lifetime/order invariants. It explicitly excludes hardware-intrinsic toggles, compiler timing and unrelated concurrency metrics. Final recommendation: `evidence is insufficient`; **no blocker**.

**Evaluator result:** match on every selected oracle concern, zero misses, zero unsupported blockers; **one fresh case in one fresh family**, not a broad precision/recall result. The actual table has fourteen scoped rows: five `pass`, seven `needs evidence`, two `not applicable`; ordered full group names, exact outcomes and all five evidence columns pass. This proves the broader conditional path/asset rule on a new real family, not the unavailable second SIMD-specific case.

### C4 complete replay: retained development regression

C4 was actually run in thirty fresh named contexts on DEV-01..20 and HOLD-01..10, followed by one formatting-only retry and REPL-01: **32 invocations, 31 distinct cases**. Outputs completed from `2026-09-22T14:45:36Z` through `15:06:06Z`. Inputs and source pins were unchanged. A separate evaluator read every actual output and checked added blockers against pinned sources; it did not invoke reviewers or rewrite expectations.

The original thirty cases scored **29/30 semantically, 28/30 including formatting**. The sole semantic miss is **HOLD-01**, already-exposed development: C4 notices the covariant array's runtime type changes but asks whether compatibility policy covers it, omitting the formerly valid downcast's `InvalidCastException` and the required `risk/blocker` classification. Original transcript SHA-256 `e77e0636b313d5c8fcfcdfc400ae9a7fa744f84d979b23ccd4ea9cf9d37bdc9c` remains unedited. This is not an oracle ambiguity: the evaluator checked base/head clone/empty-array branches and the unchanged public copy contract. Intentional sharing does not establish permission for the separate runtime-type failure.

DEV-03's original semantics matched, but an unescaped array-comprehension pipe made one Markdown row seven cells. Its original SHA-256 is `94929ba57689a46ace5860f998386f54817cee62559c0c20e302fb3223cd7bb1`. Fresh same-input/C4 task `c4-dev-03-format-retry` received only a generic pipe-escaping reminder, not prior findings or the oracle. Its thirteen-row output passes format and semantics, SHA-256 `c701caa348ed7def763acf977f79cc57a78e92a462674fbff73ea6df0b131d99`. The retry does not erase the original failure or count as a new case.

C4's HOLD-09 now explicitly requests the **same modern runtime with intrinsics enabled/disabled**, credits the supplied tables and does not substitute Framework data; SHA-256 `e39250b5c8824679f52a89e4ebc6d37d9a6a9bd9db13a03c4a304ee6e24fee11`. All other required DEV/HOLD checks matched their frozen semantic expectations, including both verified-human development rules, supported narrow wins, no-claim controls and resolved concerns. No unsupported blocker was established. With the formatting retry and successful REPL-01, C4 still scored **30/31 cases**, not acceptance. Full original output hashes and concise per-case findings are retained in session `files\c4-scoring.md`; the final-candidate table below records the subsequent full replay separately.

## C5 final-candidate freeze

C5 adds only the PERF-06/12 classification clarification: a source-established failure for previously valid callers is material risk, not merely an unspecified-policy question; distinguish explicitly intended behavior from separate collateral effects. The repair responds to **development HOLD-01**, not REPL-01. No case oracle, input, metric rule or source attribution changed. C4's failure and the successful but limited replacement result remain above.

Frozen at `2026-09-22T15:09:59Z`, repository `ac7bd61c998f0bade4aeaa00e676b87bedb6397e` plus hashed working-tree changes: candidate SHA-256 **`65207b555cd16d94fb34b7c8186dbabac41717b516b25ed0f668d261b2c7049b`**; evidence SHA-256 `5d88a405b2eae701c09a074679650b14dfd5ea5607562449e6a268519ac91e9e`; sanitized rule extract unchanged at `60874a1849722ee4369a839d8a431565069f98aa7588f044582ccf4f3a485e14`.

**Hash encoding:** candidate/evidence and transcript identities hash the exact saved bytes, including Windows CRLF. Git stores the prompt with LF; do not mistake that normalization for a prompt change. The LF candidate SHA-256 values are C3 `3fd85a737e95e49354a1e83c7caec1d05a99aaeff25b47277f601094c215e571`, C4 `097018ca8a565aba01d36a893df712a55dec68f3294941318cf06e7092816a92`, and **C5 `7bfed4f87f5ccff6b2a33c5a31cf35544b5bd2f2f78a433594daad0a965aceb6`**. C5 evidence's LF hash is `b854b1d68c534fab1c488c43b79954887aeb234bfde91313325876bb9ac52510`. Earlier fenced RED packets and the separately specified REPL/MIX input/oracle hashes use their explicitly documented UTF-8/LF convention.

**Actual full execution:** 31 fresh named tasks, `c5-dev-01`..`c5-dev-20`, `c5-hold-01`..`c5-hold-10`, and `c5-repl-01`, all with one packet and the same isolation/read-only boundary. Runtime remained Copilot CLI 1.0.87 / GPT-6 Astra (`gpt-6-astra`), inherited without override. Generic pipe escaping was reminded uniformly; no previous substantive answer was supplied. Outputs completed `15:12:24Z` through `15:23:37Z`, archived in current session `files\c5-regression`. This is a new complete run, not C4 results renamed.

All 31 output structures passed locally on first attempt: **327 scoped rows = 121 pass + 165 needs evidence + 32 not applicable + 9 risk/blocker**; ordered full G1-G8 names, five nonempty columns and exact outcomes. Output lengths are 585-1,821 whitespace-delimited words. Input/archived-result identities were rechecked. These structure counts are separate from semantic scoring below.

### Final semantic replay results

**31/31 frozen cases match, zero missed required concerns, zero established unsupported blocker findings.** This bounded result is not universal PR approval, review precision/recall, benchmark reproduction or complete-category coverage. The set is 24 real historical inputs from 20 distinct PRs plus seven labeled synthetic controls. Twenty-four cases are development/regression controls (including the seven synthetic cases); seven cases remain in evaluation families. Six of those seven were first run on C3, and REPL-01 was first run on C4. C5 is explicitly a complete replay, not 31 new independent holdouts.

The table preserves concise actual findings from both full repaired-candidate runs. Inputs, source URLs, required evidence and prohibited findings are the unchanged DEV/HOLD matrices and REPL-01 oracle above. SHA columns are prefixes of exact unedited transcript hashes; full hashes remain in `c4-scoring.md` / `c5-scoring.md` and their transcript files outside the repository. C4's DEV-03 prefix identifies its original formatting failure; the separately recorded retry is not hidden.

| Case / source pin | Actual C4 finding/request; SHA-256 prefix | Actual C5 checklist proof/request; SHA-256 prefix | C5 evaluator |
| --- | --- | --- | --- |
| DEV-01 / P01/A | 321 MB report credited; narrow/justify shared-remapper cost, ask spread/equivalence; `cf0993f931db` | G7 `needs evidence` for workload/value versus pervasive policy and narrower reuse; no quotation defect inferred from traversal differences. `c255e58e2b29` | Match |
| DEV-02 / P02/B | Local scope passes; credit 3 x 4 iterations/671 reported passes; request artifacts; `24031be0879e` | G7 `pass`; credits 557 MB and supplied repetition/test claims; requests raw spread/binaries/commands, not already-present repetitions. Resource-name drift is not a quotation failure. `272dddc79e96` | Match |
| DEV-03 / P03/C | Optional comprehension suggestion, syntax comparison N/A; original malformed table retained; `94929ba57689` | Explicit syntax-speed `not applicable`; bounded stable ordering passes; sorting value, attribution and output execution remain evidence requests. Valid table. `d8069224148b` | Match |
| DEV-04 / P04/D | Adopted comprehensions recognized; protocol/error/target cost needed; `615f67f9aa35` | Recognizes final idiom and separate localized mechanisms; asks actual benchmark/binary/protocol and targeted results, not whole-build gain from sampled CPU. `19c28293e66e` | Match |
| DEV-05 / P05/E | Credits ten rows/three fresh processes; requests roots/GC/spread; `263b1d778646` | Retention metric/table/repetitions credited; asks live-root/collection boundaries, individual spread and repeat-check trade-offs, not faster time. `202f7f9a4f8c` | Match |
| DEV-06 / P06/F | Compile-only assertion gap plus supported S1 failure-caching risk; `58beac6a9a93` | Requests executing/strengthening the conditional; source-traced S1 failed lazy suppresses later-range diagnostic, requiring two-site proof. `b13b23dae0ee` | Match; supported extension |
| DEV-07 / P07/G | Executing harness passes; 477/~810 MB reconciliation and S1 remain; `4446cad4a203` | Credits `compileExeAndRun`, asks allocation boundaries/spread/results; same S1 source risk remains independently of repaired test shape. `bdfe1b267e1b` | Match |
| DEV-08 / P08/H | Reversal equivalence passes; producer comment/assertion needed; `e0da1ce654f7` | Source proves strict positions/empty/switch cases; explicitly distinguishes missing producer contract from consumer comment, without alleging miscompile. `1dcfe8b80d67` | Match |
| DEV-09 / P09/I | Producer repair and 139,370 comparisons credited; methodology gaps only; `6d3cc98fc0f6` | Comment/Debug assertion `pass`; asks spread/instrumentation/binary/results, neither claiming assertion execution nor repeating resolved objection. `6dcfe58838ab` | Match |
| DEV-10 / P10/J | CE workload and lookup/allocation proof requested; no explicit speed claim; `eed22dc4fec1` | Build log is not comparison; asks applicable CE checking/allocation, range and retained-environment proof, not fabricated leak or rejection. `08e2e4d04458` | Match |
| DEV-11 / P11/K | ~2 MB versus ~61 MB attribution and unresolved-thunk repair credited; no additional S2 in this run; `41b25c90afca` | Same attribution/repair `pass`; asks matched flags/roots/spread/lifecycle. S2 losing importer with uninitialized dependency context adds source-supported risk. `d11d2ab89756` | Match; supported extension |
| DEV-12 / P12/L | Retains N=8 slowdown and unoptimized closure cost; request both-mode runtime/spread; `18de711aa020` | Retains 2.5->3.8 s and censored failures; credits executable guard/byref assertions; asks generated-call trade-offs, not identical promoted IL. `1c8e3a2d7447` | Match |
| DEV-13 / P13/M | Nine rotated samples and flat/worse cases credited; S3 cyclic-root risk; `44bca056ead9` | Separates retention/warm time, asks spread/lifecycle, does not declare Fantomas regression. Adds S3 and conditional old-client/new-assembly Create ABI risk. `516a8afffcb0` | Match; supported extensions |
| DEV-14 / S01 | All groups need specific evidence, no invented defect; `0346a5fcb866` | Names missing operation/workload/implementation, comparable metrics and reproduction; unknowns stay unknown. `89bb826ef5c9` | Match |
| DEV-15 / S02 | Stipulated 12->0 closures/semantics pass; no timing prerequisite; `c582148560cc` | Accepts structural/behavior stipulations; no runtime, GC or timing-error-bar demand for a count-only claim. `deb2c7abebb2` | Match; synthetic |
| DEV-16 / S03 | Rooted ~1 MB/request growth is material despite time win; `1d2a83b4ecbe` | `risk/blocker` from append-only ownership and 100/200/400 MB stipulated post-GC excess; asks bounded/released ownership, no observed OOM claim. `9dceb9510d67` | Match; synthetic |
| DEV-17 / S04 | Spelling-only metric gates N/A; `18181bfa22b5` | No compiler/runtime measurement or test campaign; scoped stipulated edit passes without pretending to inspect an underlying patch. `fe5941e4df5c` | Match; synthetic |
| DEV-18 / S05 | Difference uncertainty/value needed; overlapping intervals not equality; `bf55bf4199e4` | Credits supplied means/CIs/30 runs; requests difference uncertainty and practical value, no arbitrary threshold/regression. `c83849729323` | Match; synthetic |
| DEV-19 / S06 | 256->128 B/op passes despite flat time; `ef930fca0271` | G3/G4/G5 `pass` on allocation/resource stipulations; credits command/raw results, no timing-win requirement. `950f1f82e88c` | Match; synthetic |
| DEV-20 / S07 | Runtime/semantic/resource stipulations pass; changed IL allowed; `9e0282eb9e25` | Credits 280->80 us and 393,680->131,088 B/op; no identical-IL or compiler-speed inference. Broader endorsement remains bounded. `aa171074db5b` | Match; synthetic |
| HOLD-01 / #20388 old | Changed runtime type noticed but concrete risk demoted; **miss**; `e77e0636b313` | `risk/blocker` with concrete `Copy<object>`/`string[]` downcast failure and conversion/insertion traces; separately asks intended-sharing docs and allocation proof. `d3ce4248a748` | Match; development repair |
| HOLD-02 / #20388 final | Guards, sharing docs and tests pass; missing opt-out/provenance/nonempty evidence; `a0d2ed102989` | Explicitly credits covariance/doc repairs and ancillary scope. Separately traces old freshness-dependent ConditionalWeakTable caller failure; measurement gaps remain. `c02f9f892abc` | Match; additional supported risk, adjudication below |
| HOLD-03 / #20367 | Inline/control flow pass; sampling/binary/structural proof requested; `7cc0433404cb` | Credits narrow declarations/warning flow; separates sampled 412 MB, +1.5 KB DLL and produced-output identity; no speedup prerequisite. `8c97f8c69c74` | Match; development |
| HOLD-04 / #20544 | No claimed metric gate; SDK/Core resolution evidence needed; `df2683aeca4f` | Distinguishes three shipping debug strings from docs/tests/scripts; requests actual standalone resolution/results without inventing host failure. `587654b0a871` | Match |
| HOLD-05 / #20444 | Executing characterization recognized; no shipping benchmark; `3f6f9c71764a` | Traces eight event-list assertions, warm observation and implicit-name invalidation; targeted results needed, no claimed production optimization. `6a31b4d38d86` | Match |
| HOLD-06 / #20436 | Metrics N/A; model-policy/description consistency request; `a51606af4ca3` | G1-G5 N/A; asks intent clarification, not resource/quality defect or compiler/agent benchmark. Embedded instructions remain data. `8fc2519f5c68` | Match |
| HOLD-07 / #20119 | Strong history/removal risk traced; mailbox race excluded; `26aff4a43382` | Same source-supported ownership/removal paths; asks bounded roots, CPU/IDE measurements and retry/invalidation tests, no measured OOM or stale-symbol claim. `ad91f256a2dc` | Match |
| HOLD-08 / #20587 | Critical path, queue, capacity, compute and test-union evidence requested; `d0404a3c3917` | Credits source partition/template/SDK batch 1; requests full-job timings and actual union/publication, no invented twofold win or skipped tests. `e2396dee3dbc` | Match |
| HOLD-09 / #18509 | Exact modern-runtime intrinsics-disabled condition caught; `e39250b5c882` | Same-runtime enabled/disabled request retained with reason; credits tables, distinguishes Framework fallback and removed averages, asks final binary/numeric/API proof. `24fcfd1866aa` | Match; development replay |
| HOLD-10 / #20507 | Additive immutable API passes; true/false/default results needed; `a2ca9e8f2895` | No metric gate; credits signature/baseline/defaults and copied context; asks formatting/composition results, no global-mutation claim. `db635b93d0d6` | Match |
| REPL-01 / #20358 | Asset/threshold/stack oracle caught, no blocker; `6a3d04e9709e` | Repeats asset-aware same-runtime B/H requirement, threshold/selectivity and replicate controls, 1,024-byte stack trade-off and existing tests; no SIMD/pointer-defect invention. `0df8e8d9e547` | Match; replacement-family replay |

**Scoring correction, not oracle relaxation:** the evaluator initially recorded C5 as 30/31, treating HOLD-02's freshness-risk extension as prohibited. Reinspection of the original pre-execution oracle established that its `pass` is **specifically** for repaired covariance/documentation. It prohibits repeating the old covariance defect or claiming unchanged freshness docs, not every additional source-supported compatibility finding. C5 explicitly says the covariance mechanism is resolved and sharing is intentional/disclosed. Base copy clones two distinct empty arrays; final copy returns the same singleton, so the second addition as a key to one live ConditionalWeakTable changes from success to a duplicate-key exception. That constructed caller failure is source-supported, not an executed field report. New documentation establishes the intended sharing contract, not a blanket waiver for existing callers.

The initial erroneous score and this correction are retained in `c5-scoring.md/.json`; candidate, original oracle and every output are unchanged, and no retry or independent-case credit was added. The compact transcription above initially said "No stale covariance/freshness blocker," which was too broad; it now accurately states the original oracle's prohibition on repeating the covariance defect or claiming unchanged freshness **documentation**. This is a source-backed transcription/scoring correction, not a new oracle. Preservation is a supported remedy; the source does **not** establish that it is the only permissible release decision or a universal policy veto. The combined `risk/blocker` category includes this substantiated compatibility risk without inventing such policy. This adjudication follows the same scoped-check/independently-verified-extension rule used for S1-S3, rather than changing an oracle to fit a result.

The nine risk rows occur in eight cases. S1 repeats across two revisions; S2 uses the already-verified losing-importer path; S3 is separate from the conditional ABI finding. HOLD-07 includes historical retention and removal-cleanup subcases. No row count is presented as an independent-bug or human-vote count. Added ABI risk is scoped to an old compiled Create caller actually binding the new assembly without its old method signature, not an assertion that every deployment breaks or FCS has a universal cross-version compatibility guarantee. All source-derived failures remain distinct from observed executions.

### MIX-01: full-evidence mixed-scope extension

HOLD-04 is a useful mixed-file **no-claim** control, not the stronger test of attributing a real performance claim separately from cleanup. This additional case closes that distinction without changing C5. #20455 was reserved as mixed-scope before drafting; its named-record cleanup and `option`-to-`voption` allocation mechanism are separable within the editor subsystem. It belongs to the existing `FAMILY-vs-work-reuse`, **not a new independent family**. The case was first executed only after C5 was frozen.

| Case / PR / revision | Source and input | Pre-execution expected outcome/evidence | Prohibited unsupported finding |
| --- | --- | --- | --- |
| MIX-01 / #20455 / `003b29d39482aabf8c3a0497dea7fc3b384125fb`; base `17f8eb7d0691d30d6fc0dbcc7977ac15f9e7d5d2` | [Author benchmark](https://github.com/dotnet/fsharp/pull/20455#issuecomment-5696275398), xperiandri, `2026-09-16T10:51:20Z`; composition unverified. Full final diff/product context and all author tables, methodology and complete harness supplied. Request-origin mixed-scope control. | Credit five-way ablation: tuple/reference-record write allocation both 377 B versus shipped reference-record/voption 369 B on the Framework job, and 376/376/368 B on net10. Separate readability from the 8 B allocation mechanism; assess actual serialized hit/remove-and-readd paths versus synthetic existing-key Update. Credit supplied runtime/environment/error metadata and 1,000-operation normalization. Request only material remaining provenance, repeat/timing or real-path fidelity gaps. | No allocation-free or whole-IDE speed claim; no missing-Framework/table/ablation claim; no automatic production regression from the 5.09x Update row, or unoptimized-child conclusion from the DEBUG host banner. |

**Pre-execution packet correction:** the first unexecuted draft omitted the author benchmark. Before freezing or running it, the evaluator restored that entire available measurement artifact and adjusted the oracle to credit it. V1 input/oracle hashes `11306c2c00a6811dd5fa09bf7a897fe4f5c096ec577895bb741a5541ffa90ba4` / `5c6e8b9236e51c598724863548f87711b3c886d4cf62e607dc8b5ffcdae0aaa3` remain in the preserved evaluator transcript; neither was ever run. Withheld review objections are not a reason to hide author measurements. This correction was independent of candidate output.

Executed v2 was frozen at `2026-09-22T16:14:56Z`, `files\mixed-study\freeze.json`: input SHA-256 `a20d2a38438fd2c05cf0c9d37d1565db7cdd7cdf1e837bb50a009cedf9f6a77b`; diff `0465ec192f53352ae798550282e04cbe65aba3230ef4909adee915e064c9d5c6`; complete 25,152-byte author benchmark `2a218f35a2f7c2dc6257325fa1ce40ae4c2896407e9f23577604bf7c33f9016f`; separate evaluator oracle `7d273f83ae8c05bb89b7dfcbc866dcb1736986add69753218ef0faf7e66debd2`. All eight reviewer payloads are hashed; no feedback/history/oracle directory was exposed.

**Actual named run:** `c5-mix-01`, same CLI/model, isolated input and pinned-source-only boundary as the 31-case replay. Output `files\c5-regression\MIX-01.md`, SHA-256 **`3d75dec0a93a7f7c0fbb5f5c0038c91f7c3a5dd85da259547d8dfcf778ff09f9`**. The reviewer gives G4 `pass` to the modeled 8 B/op (~2.1%) saving and G7 `pass` to supplied attribution/cleanup. It explicitly distinguishes the Framework 4.8.1 job from the net472 editor and secondary .NET 10 comparison, credits the supplied tables/harness/configuration, and does not claim the DEBUG host proves an unoptimized benchmark.

It traces mailbox serialization and the actual removal/insertion sites, so the 5.09x synthetic Update row is **not** called an observed IDE regression. It retains the 674.98->733.73 ns hit means (~8.7% increase) and the author's reported 20-25% second-run movement without inferring equality or a reliable regression. Specific `needs evidence` requests concern the missing repeat/results/provenance, timing-parity qualification, production frequency, and the harness's always-successful stub versus no-view/subscription-failure paths. Existing lifetime/control-flow mapping passes; targeted execution remains unverified, not a fabricated correctness failure. No whole-IDE trace, compiler-wide benchmark matrix or blocker is demanded.

**Parent evaluator score:** all six frozen oracle requirements matched, zero misses and zero unsupported blockers; the actual ten-row table passes shape with five `pass` and five `needs evidence` rows. The complete final C5 suite is therefore **32/32 semantic matches**, **337 valid scoped rows (126 pass, 170 needs evidence, 32 not applicable, 9 risk/blocker)**, with no output retry on C5. It contains 25 real historical inputs from 21 PRs plus seven synthetic controls; eight evaluation cases are in seven families. MIX-01 adds a first-seen C5 case, not a new family or an extra vector-repair replacement. No new verified-human-composition precedent is claimed, and benchmark-only coverage remains unavailable.

### BENCH-01: synthetic contract only, not real holdout coverage

The absent real benchmark-only category is not silently replaced. This separately labeled **request-origin synthetic control** tests the output contract without supplying any real-PR or human-precedent credit. It changes only an existing benchmark's `[<Params(4096)>]` to the four parameters below: empty, short, retained existing and larger input. No implementation, target, dependency or operation changes; no speedup, allocation reduction, real-world representativeness or statistical result is claimed.

```fsharp
open BenchmarkDotNet.Attributes

[<MemoryDiagnoser>]
type SumBenchmarks() =
    let mutable data: int[] = [||]

    [<Params(0, 128, 4096, 65536)>]
    member val Size = 0 with get, set

    [<GlobalSetup>]
    member this.Setup() =
        data <- Array.init this.Size (fun i -> i % 1024)

    [<Benchmark>]
    member _.Sum() =
        Array.sum data
```

The unchanged runner is stipulated as Release net10.0 / BenchmarkDotNet 0.15.8 with its ordinary generated harness and MemoryDiagnoser. A Dry smoke completion of all four parameters is **stipulated synthetic data**, not a run performed here and not statistical evidence. The separately frozen oracle requires recognition of benchmark-only scope, setup exclusion/result consumption and expanded boundaries; no product speedup/table/significance gate, fabricated resource defect or claim that Dry proves timing validity is allowed.

Freeze `2026-09-22T16:39:09Z`: input SHA-256 `ceb1e223d8e14585af8c8da9d059078cb46e47e2f3fd6f2e3c1d1d0327006de9`; evaluator-only oracle `168231e15905c8306884a0d525737d22a2cf9131fef64a9584b216eb8e0f60b2` (exact UTF-8/CRLF bytes). Fresh named task `c5-bench-01` received only C5, the sanitized rules and this one fixture, not the oracle or prior outputs. Saved output SHA-256 **`68f5b6bbc361e7f9234c30d26bf321b6ab8b45a291268cd7b092aa671a9d6589`**, `files\c5-regression\BENCH-01.md`.

**Actual result:** seven `pass` rows and G4 `not applicable`. The reviewer accepts benchmark input-coverage value, recognizes that the performance-table gate does not apply, credits setup exclusion/returned-result consumption and preserves the Dry/statistical distinction. It does not demand hardware, repetitions, whole-build/IDE metrics or an actual execution certificate for the stipulated coverage-only fixture. It explicitly endorses only broader benchmark coverage, **not shipped performance**. Parent evaluator: all required checks match, no unsupported finding.

**Final executed set: 33/33 semantic matches, zero misses, zero established unsupported blockers; all 33 output structures pass.** There are **345 scoped rows: 133 pass, 170 needs evidence, 33 not applicable, nine risk/blocker**. The set contains 25 real historical inputs from 21 PRs and eight synthetic controls; eight evaluation cases remain in seven families. The real benchmark-only holdout count is still **zero**, and no new verified-human-composition source has been added. These bounded local successes do not certify unavailable real-category coverage.

## Sprint-10 reconciliation and residual manifest

**Bounded reconciliation complete, not unconditional reviewer acceptance.** Starting/current HEAD is `ac7bd61c998f0bade4aeaa00e676b87bedb6397e`. The four starting dirty files (agent, corpus, evidence, validation) were intentional C5 work and remain unstaged; this sprint edits only the three documents. Process command lines/ancestry were checked before editing: no competing writer to this worktree was identified; other active implementors belonged to other worktrees and were left alone. No acquisition restart, new reviewer batch, compiler build/test, package/format sweep, staging, commit or GitHub write occurred. The original window remains `2026-04-22T00:00:00Z` through `2026-09-22T09:28:34Z`.

| Acceptance item | Reconciled current evidence | Residual / boundary |
| --- | --- | --- |
| C5 identity and execution | All 33 saved review outputs and actual named task launch/completion records exist; 31 output hashes match `c5-scoring.json`, MIX-01/BENCH-01 match their separate recorded results. Candidate/extract unchanged. | Saved semantic score is **33/33** (31-case evaluator plus two parent-scored extensions), not a new semantic rescore or human-source audit. No missing output or unawaited delegated work. |
| Inputs, oracles and shape | Twenty DEV packet/diff identities, ten complete HOLD file sets, twenty earlier C3 output hashes/shapes and all REPL/MIX/BENCH payload/oracle freezes checked. Case-only C5 shape: **33/33, 345 rows = 133 pass + 170 needs evidence + 33 not applicable + 9 risk/blocker**. | The required `"c5-regression\*.md"` check selects **35 files** and exits 1 only for non-review `candidate.md`/`rules.md`; unchanged checker with `"c5-regression\*-*.md"` exits 0 on the exact 33 case IDs. Preserve both reports; no archive repair or reviewer retry is needed. |
| Available source coverage | [94 reserved PRs](corpus.md#final-source-coverage-accounting), completion-report hashes/totals and pinned source identities reconcile with archives. Six named conversations audited from original envelopes and relevant pins. | Prior reading credit is documented, not independently repeated. Deleted/private material, two missing Azure payloads and historical mutable edits remain unavailable; no available essential archive was missing. |
| Provenance and rule semantics | Circular request-attested authorship removed; account-attributed reviewer feedback, author rationale/measurements, explicit automation and uncertainty distinguished. All eight groups, twelve PERF rules and conditional gates remain. | **Attribution-only edits; zero semantic rule deltas.** C5's unqualified human-derived provenance wording still needs alignment; stronger human-composition grounding is not established by replay or by the named envelopes. |
| Coverage and eligibility | [Real/synthetic/result columns](corpus.md#effective-applicability-coverage) cover every required applicability category. Effective inventory **63 development / 59 holdout candidates**; executed evaluation subset **eight cases / seven families**. | Real benchmark-only **0**; BENCH-01 is synthetic and matches. Fixtures are permitted: neither relabel it a real holdout nor make the exhausted category an automatic failure. All eight evaluation cases are now seen, not fresh. |

### Frozen identities and archive coordinates

Archive root **R**: `C:\Users\tomasgrosup\.config\daily-monitor\copilot-microsoft\session-state`. **A** = `R\36233952-cdbf-4056-984e-1fe93e32d7a3\files`; **B** = `R\c8f07943-58a7-4cdd-9a76-ce0b8d398079\files`; **S** = `R\295cd124-c927-411b-84e7-ada0988de119\files`. Raw data/scripts/oracles/transcripts remain there, not in the repository. S retains exact starting snapshots, `starting-identities.json`, both shape reports, `reconciliation.json`, `c5-run-identities.json` and final `ending-identities.json`. The latter records the final validation-document digest externally to avoid a self-referential hash.

| Artifact | Starting SHA-256: exact bytes / UTF-8 LF | Current identity |
| --- | --- | --- |
| C5 candidate | `65207b555cd16d94fb34b7c8186dbabac41717b516b25ed0f668d261b2c7049b` / `7bfed4f87f5ccff6b2a33c5a31cf35544b5bd2f2f78a433594daad0a965aceb6` | Both unchanged; exact bytes equal `B\c5-regression\candidate.md`. |
| Evidence | `5d88a405b2eae701c09a074679650b14dfd5ea5607562449e6a268519ac91e9e` / `b854b1d68c534fab1c488c43b79954887aeb234bfde91313325876bb9ac52510` | Exact `3540183d17220b229452e08bab21296f451c041648e9fd2d5063b97f8a1f71f1`; LF `264cffe830df251463bb93bf6084c3080dc7523ddf8f52d5537a09936f97dc93`. New attribution/source mappings were **not** supplied to the saved C5 runs. |
| Corpus | `588d04abb0dceb82a718a8873d448edc5d0946b20151b325d8da56d8c8c83cca` / `673632df42626faba07a870575ac12ad02c0b7bafe5b076d676a16acd82af4b5` | Exact `b010afd635ed27ca005317254749580b43a834abd0f336504b0b34745f09c1a5`; LF `bf3467bd37c6a4e5259917bcb703cb71b08a7070da4a4806e9830dca0ae41147`. |
| Validation | `126f674ba3b0906aae85e8832b1f0850a51b9a50a237f0b62607a59b05723691` / `bf98943934d97683806453085d6d16a5767016f4ab09e61335f0a2d7ed567294` | Current exact/LF identities in `S\ending-identities.json`; original packets/oracles unchanged. |
| Compiler-review skill | `6d4329bbf9be0c6e114fc0830f8f4e24fc89238d1e1a34cf25461c6b35e145d6` / `ba01c3dd62b717dccebea642e6cf85059b23c2401f3e2e58ef763c2dc828e63d` | Both unchanged; read, not tuned. |

Exact hashes include saved CRLF; normalized hashes replace CRLF with LF before SHA-256, without other text changes. C5's **actual sanitized extract** `B\c5-regression\rules.md` remains `60874a1849722ee4369a839d8a431565069f98aa7588f044582ccf4f3a485e14`. The **effective decision-rule projection** before/after reconciliation is `0c226f5ae36666f41d31644c16db4967bfa0f25ab8928d0091ddefd81a5ad7b8`: UTF-8 canonical JSON (`ensure_ascii=False`, separators `(',', ':')`), keys `rules` then `metrics`; twelve stripped PERF rows projected to ID/rule/evidence/limits (columns 1/6/7/8), then nine metric rows projected to their first three columns. `S\reconcile_artifacts.py` defines this source/provenance-excluding projection; it is not a substitute for the full candidate/extract identity or a claim that new full-context source notes were model-tested.

### Exact case inputs, original expectations and run identities

The manifest below enumerates **all 33 cases** without replacing their original detailed expectations. For every case, its actual run name is `c5-` plus the lowercase case ID, its unchanged output is `B\c5-regression\<CASE>.md`, and its result/hash is in [the 31-case C5 table](#final-semantic-replay-results) or the linked extension section. `B`'s parent `events.jsonl` confirms all 33 launches/completions with Copilot CLI 1.0.87 / `gpt-6-astra`, no model override. `S\c5-run-identities.json` records exact tool-call IDs, timestamps, prompts and completion records, SHA-256 `797f5c84b65543ddc1a3f76400c6563bada75f4888c73ffb8bcef4cd5cdbd0c8`. The twenty DEV tasks launched `15:11:35.052Z`, ten HOLD plus REPL at `15:18:44.206Z`, MIX at `16:15:08.300Z`, BENCH at `16:39:28.178Z` on 2026-09-22. Parent tool-completion times are not child output-save times.

| Case IDs | Exact frozen input / original oracle | Actual result and effective eligibility |
| --- | --- | --- |
| DEV-01, DEV-02, DEV-03, DEV-04, DEV-05, DEV-06, DEV-07, DEV-08, DEV-09, DEV-10, DEV-11, DEV-12, DEV-13 | `A\final-regression\<CASE>\input.md` and `diff.patch`; P01-P13 / pins A-M in [the original expectation matrix](#frozen-expectation-matrix). `manifest.json` SHA-256 `d9461f76f5fa0a946e2cf8ccc78bb3795cbc6ec0e2f6dec0795355672de5f5aa`. P02-v2 / DEV-03-v2 remain the pre-execution corrections. | Recorded 13/13 matches; historical real-PR **development**, not holdouts. Required outcomes remain scoped: missing proof is not a blocker; resolved scope/test/invariant concerns pass. Supported additional source risks remain separately adjudicated. |
| DEV-14, DEV-15, DEV-16, DEV-17, DEV-18, DEV-19, DEV-20 | Same DEV directory/manifest, S01-S07; [original matrix](#frozen-expectation-matrix) and [packet hashes](#packet-hashes). Respectively: needs evidence; structural/semantic pass without timing; retention risk/blocker; metric N/A; significance needs evidence; flat-time allocation pass; generated-runtime/semantic pass. | Recorded 7/7 matches; **synthetic development**, no real-source or holdout credit. |
| HOLD-01, HOLD-02, HOLD-03, HOLD-04, HOLD-05, HOLD-06, HOLD-07, HOLD-08, HOLD-09, HOLD-10 | `A\holdout-study\inputs\<CASE>`; `suite.json` SHA-256 `57258d83db61fdcb97a74ae1f026ec152bf1d32b83fb1fbe530eca3328a63e8c`; original `oracles.md` `ddfc494f450358cf527cdbf08c45066d9fca111efc84aa2aa74b8399400f59dd`. [Original scoped expectations](#frozen-source-expectations) and [base/head/input/diff identities](#actual-outputs-and-evaluator-comparison) are unchanged. Static `reviewer_executed=false` in the freeze is pre-execution metadata, superseded by actual run records, not rewritten. | Recorded 10/10 C5 matches. HOLD-01/02/03 are the connected core-empty-array/inline-closures development group (also includes realsig-TLR); HOLD-09/vector-sum was promoted after the C3 miss. HOLD-04/05/06/07/08/10 remain six previously evaluated cases/families. |
| REPL-01 | `B\replacement-study\reviewer\REPL-01`, separate `evaluator\REPL-01.oracle.md`; [original oracle and coordinates](#repl-01-independent-source-oracle-and-actual-result). Freeze manifest `e72c0ed233657ed9f82d1da6c5dfc87a18fcfb2e549381cff05cb1e5ba6a2b13` hashes all 14 payloads and oracle `e8a0f40ff5f622ef3995ca0554ff07baca5299f9476f8c35e155799e4d08ebe6`. | Recorded match: asset/branch-sensitive evidence requests, existing tests credited, no invented SIMD/pointer defect. String-spans evaluation family; **first run C4**, C5 replay. Freeze's C4 candidate field is historical, not the C5 invocation identity. |
| MIX-01 | Executed v2 only: `B\mixed-study\reviewer\MIX-01` and `evaluator\MIX-01.oracle.md`; [six original requirements](#mix-01-full-evidence-mixed-scope-extension). Freeze manifest `69662d0a1d93ef37ce8fbc0a39c152eb507bc1028758cfdc67142404b4e328d0` hashes all eight payloads and oracle `7d273f83ae8c05bb89b7dfcbc866dcb1736986add69753218ef0faf7e66debd2`. | Recorded match: credit author ablation, isolate cleanup/allocation, no synthetic Update-to-IDE regression inference. First seen on C5; same VS-work-reuse family as HOLD-07, **not an eighth family**. Unexecuted v1 remains historical. |
| BENCH-01 | `B\benchmark-control\reviewer\input.md`, `evaluator\oracle.md`; [original stipulations](#bench-01-synthetic-contract-only-not-real-holdout-coverage). Freeze manifest `ee0a94609deeb4da3779b08548c666e45f55aa32c1c2b54db5ccdf62734d5418`; input/oracle exact hashes remain above. | Recorded match: coverage-only benefit, no quantitative product claim, Dry is smoke only. **Synthetic contract control**, never a real historical holdout. |

The 31-case score record hashes are `B\c5-scoring.json` `3496045f204d033d8424919c13cacc6fbba55aea2cb173c79adcd792fd2a29eb` and `.md` `25f11aafa9d92f092cc5917fbdfbf441dd2f2edaf1e398f28c73c6ca72a8f3fe`. They are **not 33-case files**: MIX/BENCH add the separately preserved parent scores above. The C3 HOLD-09 miss, C4 HOLD-01 miss, C4 DEV-03 formatting failure/retry and C5 HOLD-02 scoring correction all remain recorded; no oracle was changed to fit an output.

### Only remaining or affected checks for sprint 11

**No semantic repair or missing C5 case is identified by this reconciliation.** Do not reacquire 94 PRs, rerun the 804-PR search, widen the window or create a real-benchmark-only requirement. Existing fixtures satisfy that applicability boundary; the original eight evaluation cases remain the executed subset, not unused replacements.

The precise unresolved acceptance item is **unqualified verified-human grounding**: the named envelopes establish participants, concerns and resolutions, not unaided composition. In particular #20352 comment `5422336205` has no review envelope and #20348 comments `3854812488`/`3860828838` have empty parent reviews `5021077157`/`5028138073`; neither contains a composition attestation. No missing envelope can be fixed by another bulk search. A stronger retained claim would need source-specific attribution beyond these records; do not invent an external attestation policy or treat style/User status as proof.

**Proposed provenance-only delta, not applied to the candidate here:** replace its description's "verified human review precedents" with "source-attributed performance-review precedents"; replace its "verified rules/metric map" label with "rules/metric map"; replace "Only PERF-08/09 are human-derived + request-required; other gates are request-required. Preserve these labels." with "All gates are request-required. Source-attributed reviewer feedback and author rationale support narrow rules with the evidence document's composition limits; neither is verified by account type or replay." In the compiler-review skill, "verified evidence resources" should become "evidence and provenance resources." Apply the same attribution correction to any **new** sanitized extract, never overwrite the archived one. No PERF rule meaning, metric condition, outcome or oracle change is proposed.

The affected provenance sentinels are **DEV-01/02/03/04** (PERF-08/09 scope, narrower alternative, optional syntax and resolved idiom). If sprint 11 changes candidate/extract wording, freeze both new identities before running those exact unchanged inputs; compare their original scoped outcomes and check that attribution uncertainty is not turned into a missing performance prerequisite. This is development replay, not new holdout credit. The added #20353/#20287/#20296/#19995 mappings explain already-required rules; **DEV-06/07/13** are existing source-related controls, not newly failing cases, and #20287/#19995 have no dedicated executed packet. DEV-05's #20261 source-label correction (unsupported model association -> uncertain composition) likewise changes neither its input/oracle nor its human-evidence credit. There is no basis here for a new semantic rule or a blanket replay mandate. A later semantic edit must name its exact rule delta and affected cases before execution; no such edit is authorized by a passing old C5 score.

## Sprint-11 bounded semantic validation

**Accepted after input-isolation repair: C6-S's selected complete replay matches 36/36 frozen concerns, with zero unsupported findings.** C6's original shared PERF-11 extract disclosed a later executable-test repair to historical pre-fix DEV-06; its score and artifacts remain historical, not clean-isolation acceptance. C6-S uses the unchanged candidate with a sanitized extract and actual new outputs, including strict-isolation replacements described below. The set covers 28 real historical inputs from 23 PRs and eight synthetic controls. Eight previously seen evaluation cases remain in seven families; no new independent holdout or verified-human-composition credit is claimed. Source-attributed feedback is usable with the [recorded composition limits](evidence.md#named-conversation-audit); replay does not authenticate authorship.

Archive **T** = `R\b8acc633-d504-4744-b6c6-467dd7b37ea1\files`, using R from [archive coordinates](#frozen-identities-and-archive-coordinates). **P** = `T\bounded-validation`, **F** = `T\c6-validation`. Each has frozen candidate/rules, separately stored evaluator oracles, isolated reviewer packets, verbatim outputs, invocation envelopes and per-case status/hashes. `T\final-checks.json` and `ending-identities.json` record final checks and document identities outside the repository. Private orchestration is not a deliverable.

### Recovery, measured miss and final freeze

The original C5 candidate/extract, 33 transcripts and packet identities were verified, including the corrected MIX-01 author benchmark. The case-only checker again passed **33 outputs / 345 rows**. They were accepted as the existing C5 full execution: there was no recovery-triggered 33-run batch. Sprint 10 changed attribution, not decisions. Its provenance-only substitutions were applied to candidate **C5-P**, the sanitized extract and the integration link label; four unchanged development sentinels passed. Two source conversations lacked dedicated executions, so three source-selected development inputs were frozen before outputs, below.

**C5-P scored 6/7, not acceptance.** SRC-02 credited automatic listener removal but missed remaining `cacheId`/filtering and always-on Debug `Stats.Incr` cost. Generic external-listener comparability did not satisfy that concern. Independent evaluation checked the original source and oracle; `P\source-evaluation.md` retains the miss, two other source matches and independently supported additional risks. Original SRC-02 output SHA-256 is `91ce9f2e6d65f3b7887097cf4dcdb6179a9b226742bdcdc0530aba84d4a46ac1`. No oracle, packet or output was rewritten.

**C6's sole decision-rule change is conditional PERF-05 instrumentation assessment:** credit repaired ownership separately from remaining collection costs; request relevant enabled/disabled evidence or justify bounded accounting without banning useful diagnostics or prescribing a directive. The [#19995 source mapping](evidence.md#5-regressions-and-resource-trade-offs) supports that repair. A fresh SRC-02 focused run passed (`F\focused\SRC-02.md`, SHA-256 `02c5179e9d8d70ff0b7880cb366a07f2e126796fd9cfb03f2e164862c77fd2b3`), then all 36 cases ran once under the unchanged C6 freeze. The already-exposed cache-listener family remains development; this repair consumed no evaluation family and requires no new replacement. REPL-01 retains only its original independent-family credit.

| Freeze / execution | Identity and actual scope |
| --- | --- |
| C5-P, `2026-09-22T17:58:04.746466Z` | Candidate UTF-8/LF `08e484753187881209155442bb05157ca30aef636f3c319372d85d75535b2708`; rules `5c792b62db3fc6b6400f974e2f489940d1a8a264666f00ca756f8bc9ab795510`; manifest `4673852e63b43190a00869d1eb21200bba052f786e704ddf1d6e7a8ffed93de5`. Seven executions: four development replays, three first-executed development controls; no retries. |
| C6, `2026-09-22T18:12:29.272712Z` | Repository candidate exact bytes `69452a977cbde32afacd0dc4e2dc9f12ae96cd3ee4d98f890122dc56ad079e0f`; normalized LF and actual supplied candidate `3d3b2a2d87420715e0f983020da07e55a1f8392219577657ce8ef29ee8a3b7fb`; rules `2affb8050e84a80228798bbdc6b9e167e168664f473a8bda781e1ba78a6556d7`. |
| C6 manifest and effective rules | `F\freeze.json` `0c6191487aec8504adad2980d0db4920457368f6e03f1f585edc237b7e1be0ef`; effective projection `c53483cab14c11796410ab52f99f8d7911828e753f490197696d77915241c937`. Only PERF-05 changes from sprint 10's projection; metric rows and other eleven rules are unchanged. All 402 reviewer members and eight evaluator files match the freeze. |
| Original C6 execution | Named `performance-improvement-review`, Copilot CLI 1.0.87 / `gpt-6-astra`, inherited without override or fallback. Full-suite launches start `2026-09-22T18:16:42.944Z`; all complete by `18:32:53.285Z`. **37 C6 invocations = one focused check + 36 replays**, not 37 distinct cases. No original-output retry or stale-result substitution; later isolation rejection remains above. |
| Invocation and evaluation records | `F\invocation-events.json` `098d3a4f4ba9e5bfa64786e7bf539423214df5c7dd4c3b6b3ed76a6e26ec7c2d` stores exact launch/completion IDs, prompts, model and timestamps. Independent full-output `F\scoring.json` `500d9be853a43b423aab1a7b86e25a9b83a14f5d0dfe4d42ce675cfd293243c4`; `.md` `daefd7ff5155058b14dd1bea11d24fcd1dbdf95d723457d2fe816ca4ed198b54`. |

Each original reviewer ran in a separate one-case context with full candidate instructions, rule/metric extract, complete pinned diff and relevant source/signature/test context. Direct discussion/oracle/other-output reads were excluded, but the extract was not fully sanitized: PERF-11 said "Recognize the final test executes it" regardless of the selected historical revision. Hash and shape checks did not detect this content leak. Additional code reads were limited to that packet's Git pins. Full author measurements were retained, including MIX-01's 25,152-byte artifact. Reviewers only saved their own unedited private outputs; evaluators only saved private scores. The orchestrator alone edited repository documents. Neither author-reported nor synthetic measurements/tests were executed here.

### Input-isolation repair and sanitized replay

**C6-S** uses the unchanged C6 candidate and general decision gates, with a case-neutral extract. PERF-11 now credits runtime assertions only when the supplied pinned test invokes them; the extract also removes residual thread/family references and the case-specific 321 MB figure. No packet, source pin, benchmark, oracle or prior output changed. This correction supersedes earlier "sanitized" and "later fixes excluded" descriptions wherever the older extract was supplied; their observed scores remain historical, not clean-isolation acceptance.

Archive **U** = `R\7b621641-31f6-4465-9fb7-e0cca80e32f2\files`; **D** = `U\c6-sanitized`. The first affected DEV-06 run detected the nonexecuting conditional, then all 36 cases ran under this freeze. Tool-trace inspection additionally found generic compiler-review skill loads in that focused run and DEV-06/08/10/11. Although the skill contained no withheld case fact, these five contexts were excluded rather than silently accepted. A frozen wrapper clarification identifies artifact-only calibration, without changing reviewer rules or hinting outcomes. One clean focused run and four clean replacements completed; original outputs remain untouched.

| Identity / execution | Actual scope |
| --- | --- |
| C6-S freeze, `2026-09-22T19:09:22.064505Z` | `D\freeze.json` SHA-256 `05a9e997292f66dd0efb708ebce0f68957a551b7270dd49b4fc4425c1668a759`; candidate LF `3d3b2a2d87420715e0f983020da07e55a1f8392219577657ce8ef29ee8a3b7fb`; sanitized extract `72c94967b79861ad210f105ce3f601eb77d31b5be6057adb46e2a93aa3654bea`. All 402 packet members and eight oracle files equal F. |
| Isolation retry freeze, `2026-09-22T19:32:18.437499Z` | `D\isolation-retries\freeze.json` `669d091916eb955b1e3c9f9d2b3b9d2f62ff8f946cbee1cf27cfd57c55157fec`; exclusions selected by tool-boundary violation, not semantic score. |
| Actual reviewer execution | Named `performance-improvement-review`, Copilot CLI 1.0.87 / `gpt-6-astra`, inherited configuration, no override/fallback. Full batch launched `19:14:10.139Z`; final selected replacement completed `19:41:52.307Z` on 2026-09-22. **42 invocations = 36 original replays + one original focused run + five isolation retries**; 36 distinct cases, zero first-seen cases. |
| Selected transcripts | `D\selection.json` `0a0fe2364a6aef616034647fbd9506de638be87c5f7c67d28170bb357feda905` records every selected path, input/payload/oracle/output hash, invocation, agent ID and timestamp. DEV-06/08/10/11 use `isolation-retries\outputs`; all others use `outputs`. The focused run is separate, never substituted for DEV-06. |
| Input-boundary regression checks | The old extract fails the history guard; the new one passes. `U\isolation-tool-audit.json` retains all 1,041 reviewer tool calls. Every selected context has zero skill loads, unauthorized source reads or writes; temporary overflow reads resolve to that same context's pinned command output. Exact saved transcript bytes match the recorded single output-creation patch. MIX-01 retains its 25,152-byte author benchmark. |
| Semantic evaluation | Original batch `D\scoring.json` `67b55d2f9bee1ec6acddd8c97179368cad718cac105f67028d92b165aeb77715`; replacements `D\isolation-retries\scoring.json` `e0c553f909e8993003a0828fe673b1c547f5f273618357b7316675db4152b7ad`. Selected aggregate `D\selected-scoring.json` `381bd679b366639cf90607abbdc8b9ca64e4751145fbcf0b7f647109fc139392`: **36 matches, zero misses, zero unsupported findings**. |

**Evaluator limitation:** a locator search exposed historical DEV-01 result lines to the evaluator, not to any reviewer. It then checked the actual outputs, frozen concerns and pinned mechanisms rather than importing those scores. Original evaluator records and the selected aggregate preserve this caveat; no blinded-evaluator claim is made. This does not change reviewer isolation, source provenance, case eligibility or first-seen counts.

### Added source-backed oracles

All three were selected from the named sprint-10 conversations, not from candidate outputs or a reserve-wide search. Exact source comments and the oracles were frozen in P before execution and copied unchanged to F. These are **development**, not three new holdouts or certified-human votes. Existing DEV-01/02/03/04 cover scope/optional syntax; DEV-07 and DEV-13 actually cover the elapsed/allocation distinction and resolved typed/shared-settings key.

| Case / historical input | Evaluator-only source and frozen concern / required evidence | Oracle SHA-256 |
| --- | --- | --- |
| SRC-01 / #20287 final; base `e631ed7f4f210cb3dade65b1d6341bb7106e51bb`, head `9252af630067c59e8fd087380a5d52ecd95c15a6` | [Reader question](https://github.com/dotnet/fsharp/pull/20287#discussion_r3811931627), [qualification](https://github.com/dotnet/fsharp/pull/20287#discussion_r3811945510), [answer](https://github.com/dotnet/fsharp/pull/20287#discussion_r3811998107), originally at `63df5b0fb83a5439dcad47672c01df9b6ad6fe57`. Credit wrapper saving/tables/runs; shared readers do not imply one releasable closure per owner. Any different last-owner risk needs a concrete caller/root/consequence; no universal clearing policy. | `a9c49a55cc982c4da0398829af5152ee0b4fc1fd73259818d8d115c38cd0510d` |
| SRC-02 / #19995 original code-only; base `9ae5fc5cfe81ec24f980fea30972be382f83f9b8`, head `f96b70e1c112af68b5e14fd75fca4bc6ca7308c2` | [Tag cost](https://github.com/dotnet/fsharp/pull/19995#discussion_r3480852672), [narrower gate](https://github.com/dotnet/fsharp/pull/19995#discussion_r3490903438). Credit automatic listener repair, assess remaining tags/filtering/direct Debug counters and request relevant exporter/on-off cost evidence or bounded alternative. No measured slowdown or mandatory directive. | `8f200f1df42b149975c576e677d47b9112b90b777d5586f5e139de9f752d3fff` |
| SRC-03 / #19995 final; base `ea3908edab08f1009d0bde1883778797c785170a`, head `bd2148ba7aff3a9f365be52a4e4769a56fef8acf` | [Stats alternative](https://github.com/dotnet/fsharp/pull/19995#discussion_r3534273208), [aggregation accepted](https://github.com/dotnet/fsharp/pull/19995#discussion_r3534528513), [author rationale](https://github.com/dotnet/fsharp/pull/19995#discussion_r3536631511). Recognize removed listeners/IDs, disposable aggregation and serialized deltas; credit author measurements but catch their stale direct-Stats account. Request final-binary/lifecycle evidence, not mandatory per-instance Stats. | `e5549a09cd164500ef16baa0a89c5083299e3b2fd8b147c8b38ef836d667744a` |

### Final case matrix

Source/oracle coordinates and required evidence remain in [the original DEV matrix](#frozen-expectation-matrix), [exact HOLD oracle](#frozen-source-expectations), [REPL-01](#repl-01-independent-source-oracle-and-actual-result), [MIX-01](#mix-01-full-evidence-mixed-scope-extension), [BENCH-01](#bench-01-synthetic-contract-only-not-real-holdout-coverage) and the SRC table above. Every row below uses **C6-S's selected actual output**, not the original C6 score. Hashes are prefixes of **input.md / exact unedited output**; D's freeze, selection and selected score contain full identities. The earlier C6 matrix/score remains in `F\scoring.json/.md`. Outcome words describe scoped checks, not overall PR approval.

| Case / source-oracle | Expected evidence and actual scoped outcome | Input / output SHA-256 prefixes | Evaluation |
| --- | --- | --- | --- |
| DEV-01 / P01/A | Needs evidence for remapper breadth/value and equivalence; 321 MB credited, no invented traversal defect. | `f30a42d043fe` / `da56eb23ea28` | Match |
| DEV-02 / P02/B | Local reuse passes; supplied repetitions/tests credited; spread/binaries/commands still needed. | `04f2be685989` / `015ad056bad4` | Match |
| DEV-03 / P03/C | Syntax-speed comparison not applicable; clarity optional, no timing prerequisite. | `e3e314d6164d` / `f01922c023d3` | Match |
| DEV-04 / P04/D | Adopted comprehensions/local scope pass; protocol/error/results needed, no sampled-CPU-to-build inference. | `5d433db57e19` / `b12a6c2d9aa8` | Match |
| DEV-05 / P05/E | Retention/table/three processes credited; roots/GC/spread needed, not faster time. | `96752a4b76cb` / `904e296f2499` | Match |
| DEV-06 / P06/F | Detects nonexecuting conditional without later-fix hint; separate source-supported failed-lazy diagnostic risk. | `cbe4f8ce667f` / `bdf29defb5eb` | Match |
| DEV-07 / P07/G | Executing harness passes; net allocation/range proof needed; no stale compile-only finding. | `698c223c1c0e` / `445e802fe799` | Match |
| DEV-08 / P08/H | Current order passes; producer contract still needs comment/assertion, not consumer prose. | `f3d8d7074d08` / `1cfd955cf5b0` | Match |
| DEV-09 / P09/I | Producer repair/reported comparisons credited; methodology/execution still needed. | `b57a4ab99b24` / `2b5cb493dcf3` | Match |
| DEV-10 / P10/J | Needs CE workload/lookup/range evidence; build log is not comparative speed. | `1ff359bc5315` / `aa08164d94f5` | Match |
| DEV-11 / P11/K | 2 MB versus 61 MB and unresolved-thunk repair credited; matched flags/lifecycle needed; abandoned-claim risk checked. | `a752ee8caa27` / `23d7857cf4ab` | Match |
| DEV-12 / P12/L | Retains N=8 slowdown/censored failures and both emitted modes; runtime trade-offs needed, not identical promoted IL. | `271f6a93a744` / `9cdd548d470a` | Match |
| DEV-13 / P13/M | Retention/warm time and nine rotated samples credited; spread/lifecycle needed; root-key and conditional ABI risks checked. | `2bd641750a29` / `3fd1f80337c9` | Match |
| DEV-14 / S01 | Bare Faster needs workload/implementation/measurements; no invented defect. | `bace772e62d2` / `f410f1987ada` | Match |
| DEV-15 / S02 | Stipulated closure-count/semantics pass; reproduction gaps separate, no timing/GC prerequisite. | `d4116d5c6f15` / `41a36e5e5b56` | Match |
| DEV-16 / S03 | Stipulated rooted linear retention is risk despite latency win; no observed OOM claim. | `97d8213c82e4` / `641c8453f2ea` | Match |
| DEV-17 / S04 | Spelling-only no-claim metric gates not applicable. | `d18f8996f565` / `db31fcefec5e` | Match |
| DEV-18 / S05 | Difference uncertainty/value needed; supplied intervals/runs credited, no arbitrary threshold. | `3fe07019f75f` / `c2c197a70bf2` | Match |
| DEV-19 / S06 | Allocation/resource evidence passes despite flat time; protocol/value gaps remain scoped. | `408cd21095cd` / `ccde16e2e95d` | Match |
| DEV-20 / S07 | Runtime/semantic stipulations pass; intentionally different IL allowed. | `ad89a9c1f23f` / `22e0537bf152` | Match |
| HOLD-01 / #20388 original | Concrete covariant downcast failure is risk, not policy uncertainty. | `48032622dad1` / `51d6b30ccf67` | Match |
| HOLD-02 / #20388 final | Covariance/docs repairs pass; measurement gaps remain, no universal freshness/CWT policy imposed. | `253b3c16f06a` / `a4f610dd55f1` | Match |
| HOLD-03 / #20367 | Narrow inline/control flow passes; sampled allocation/binary/output provenance needed. | `bbb310d0c7fc` / `9d0fa839eb46` | Match |
| HOLD-04 / #20544 | No speed gate; SDK-script host/Core availability remains a qualified execution question. | `850ca9381f8c` / `78af0560ad5e` | Match |
| HOLD-05 / #20444 | Test-only characterization/assertion shape passes; targeted results, not product benchmarks. | `5059588b0a45` / `d78a1405ebf6` | Match |
| HOLD-06 / #20436 | Docs-only metric gates not applicable; editorial/product-compatibility scope passes, no embedded-command execution. | `fa2f3b3622ee` / `be6d1fff3705` | Match |
| HOLD-07 / #20119 | Strong historical-reader ownership and project-removal gap are risks; serialized-first-use race excluded. | `2946fad8082f` / `f81c4450b5de` | Match |
| HOLD-08 / #20587 | Static partition credited; critical path/queue/compute/actual test-union evidence needed. | `0df2f71c513d` / `65b3d8857772` | Match |
| HOLD-09 / #18509 | Same-runtime intrinsics-disabled measurement explicitly required; Framework/table evidence credited; conditional float32 risk separate. | `d10258f7b296` / `36141c557536` | Match |
| HOLD-10 / #20507 | Additive immutable API passes; formatting-option execution needed, no performance gate. | `9f7281bb98ed` / `a5f6336b4a79` | Match |
| REPL-01 / #20358 | Core asset/fallback/branch measurements needed; existing semantics/tests credited, no invented pointer/SIMD defect. | `9fec7df157b5` / `9764c745caf6` | Match |
| MIX-01 / #20455 | Author ablation/8 B saving pass; timing-parity and absent-subscription evidence needed, synthetic Update not an IDE regression. | `a20d2a38438f` / `7416dda0cc92` | Match |
| BENCH-01 / fixture | Benchmark coverage passes; no product speed claim or Dry-as-statistics inference. | `ceb1e223d8e1` / `ab73d7e68471` | Match |
| SRC-01 / #20287 | Shared-reader premise qualified; tables/noise credited; distinct last-owner retention risk substantiated. | `165c4145309b` / `e232550b744f` | Match |
| SRC-02 / #19995 original | Ownership repair passes; remaining tags/filter/counters/exporter costs need evidence, not a mandatory gate. | `ad43e41cb999` / `8a89212316fe` | Match |
| SRC-03 / #19995 final | Aggregation/serialization repairs credited; stale prose/final-binary evidence gap and public-listener removal distinguished. | `05851a3f32cd` / `7718b4c8baea` | Match |

The evaluator checked pinned callers/preconditions/consequences for additional risks. DEV-11 identifies **abandoned publication ownership after a later batch import fails**, not C5's losing-importer-context trace. SRC-01 requires the forced store to become the callback/context's last owner; the shared-reader workload does not establish per-wrapper release savings. SRC-03's removed public disposable listener is separate from the repaired automatic leak. Diagnostic-cache, cyclic sharing-key, conditional Create binding and editor retention/removal traces remain in D's selected score. HOLD-02 raises no CWT/freshness blocker in this replay; F's earlier conditional finding remains historical. These are source-derived conditional failures, not executed field results, measured regressions or universal compatibility policies.

HOLD-09's additional float32 risk received separate dependency verification: the packet names .NET 9.0.4, whose [pinned LINQ source](https://github.com/dotnet/runtime/blob/f57e6dc747158ab7ade4e62a75a6750d16b771e8/src/libraries/System.Linq/src/System/Linq/Sum.cs#L18-L78) uses a Double accumulator for Single inputs. Under the stated ordinary-rounding preconditions, `[2^24; 1; -2^24]` analytically changes from 0 to 1 versus the pinned old Single fold, independently of SIMD. Exact API/source bytes and the addendum are in `D\evaluations\HOLD-09-runtime-source.*`; addendum SHA-256 `96f21db44542fd88e9e94afc132cb4bc552c144a104d31d1c006c8847354d1c0`. This is source/mathematical proof, not an executed result, new human precedent, changed oracle or enlarged corpus.

### Final acceptance and coverage checks

| Obligation | Actual result |
| --- | --- |
| Frozen final semantics and complete replay | C6 candidate/effective rules unchanged; C6-S extract, all payload/oracle identities and 36 selected actual outputs verified. Source-checked evaluation: 36 matches / zero misses / zero unsupported findings, with the disclosed evaluator-blindness limit. C5/C6 historical outputs are not substituted. |
| Source-backed concerns and preserved failures | All six named conversations have actual scoped outputs. Source-attributed feedback, author rationale, request-origin and synthetic evidence stay separate. C3 HOLD-09, C4 HOLD-01, C4 formatting retry, C5 scoring correction and C5-P SRC-02 miss remain auditable. |
| Isolation, eligibility and first-seen accounting | **86 sprint-11 reviewer invocations: 44 in iteration 1 plus 42 in iteration 2.** The selected 36 and clean focused sentinel have sealed one-case inputs; five additional contexts remain excluded. Only SRC-01/02/03 were first-executed in sprint 11, all development, in iteration 1. All iteration-2 cases are replays. |
| Family separation and replacements | Core-empty-array/inline-closures/realsig-TLR/vector-sum stay development; MIX shares VS-work-reuse. The instrumentation repair used exposed development, and the isolation repair changed no decision rule. No tuned holdout was relabeled independent; REPL-01 retains only its original family credit. |
| Required applicability | [Actual coverage](corpus.md#effective-applicability-coverage) includes real mixed/test/docs/generated/API/behavior/lifecycle controls and strong/weak/noise/narrow-win cases. BENCH-01 supplies synthetic benchmark-only coverage; real benchmark-only credit is **zero**, with no repeated search. |
| Actual output contract | All 36 selected single tables have eight ordered full groups, five nonempty columns, only four permitted outcomes and an allowed final recommendation. **364 rows: 137 pass, 184 needs evidence, 32 not applicable, 11 risk/blocker.** Semantic scoring separately verifies concrete proof/unknown/action, qualifications and recommendations; row totals are observations, not acceptance targets. |
| Local integrity and boundaries | Relative links/anchors, rule/case/source references, frozen inputs/outputs, historical oracles and `git diff --check` pass. Iteration 2 edits only validation/corpus within the five-file delivery surface; candidate, skill and evidence stay unchanged. HEAD/index unchanged; no staging/commit/push/GitHub write, compiler build/test, benchmark or formatting sweep. All required reviewer/evaluator work is complete; final checks and ending identities are in U. |

## Sprint-12 delivery audit

**Audited 2026-09-22:** C6-S remains the accepted replay; no reviewer semantics, input, oracle, selection or score changed during delivery. The audit reused the existing shape, anchor, effective-rule and C6-S closure assertions read-only, replacing archive writes with equality checks. Historical C3/C5 fixed-hash checks were not misapplied to C6. Checks and pre-edit identities are in `R\9168de79-7d16-4602-9ba9-74a3162b550d\files\audit_delivery.py`, `audit-results.json` and `starting-identities.json`; no private artifact is a repository deliverable.

| Acceptance item | Verified final artifact/result |
| --- | --- |
| Evidence-first review and measurement contract | Agent G1-G4 / PERF-01..04 require value/workload, before/after tables with units, baseline/changed binaries and configuration, repetitions, variability, environment and reproduction. Conditional metric maps cover time, memory, allocation/GC, closures, CPU/concurrency, startup/build and workload measures. DEV-05/12/13/15/18/19/20, HOLD-09, REPL-01 and MIX-01 demonstrate scoped credit and specific requests rather than universal demands. |
| Behavior, resources and surgical risk | G5-G8 / PERF-05..12 retain API/compatibility, lifecycle, repeated work, isolation, helper reuse and scope gates. DEV-01..04/06..11, HOLD-01/02/07 and SRC-01..03 preserve resolved concerns and distinguish missing proof from substantiated caller/resource failures. Eight ordered groups, four outcomes, proof/unknown/action fields and final recommendations are intact. |
| Actual regression and isolation | Every selected case has its frozen expectation, verbatim transcript, successful named invocation and matching score: **36 cases, 402 packet members, eight oracle files; 36 recorded semantic matches, zero misses/unsupported findings**. Recomputed shape is **364 rows: 137 pass, 184 needs evidence, 32 not applicable, 11 risk/blocker**. Four full-suite isolation replacements and the separate focused sentinel remain correctly selected; failed/excluded executions and the evaluator-exposure caveat remain visible. This is artifact verification, not a new replay or semantic rescore. |
| Final identities | Candidate LF **`3d3b2a2d87420715e0f983020da07e55a1f8392219577657ce8ef29ee8a3b7fb`**, effective rules `c53483cab14c11796410ab52f99f8d7911828e753f490197696d77915241c937`, sanitized extract `72c94967b79861ad210f105ce3f601eb77d31b5be6057adb46e2a93aa3654bea`. Candidate, input/payload/oracle/output and invocation hashes match C6-S's freeze/selection/score records; original RED packets and expectations are unchanged. |
| Discovery, references and sources | Unique two-field YAML name/description verified; the current catalog exposes `performance-improvement-review`, and archived C6-S calls actually used that name. The skill's aggregate change is only the four-line discovery reference, with general dispatch unchanged. **72 relative links/anchors**, twelve unique PERF definitions, 36 unique final case IDs and **259 cited comment/review URLs against matching archived API IDs** pass. The first source-index pass omitted activity/context channel files; including those existing captures resolved all seven lookup gaps without retrieval or source edits. |
| Coverage and provenance | 485 inventory PRs, 94 reserved-source ledgers/diffs/channel hashes and the 804-entry benchmark screen reconcile. Effective included split remains **63 development / 59 holdout candidates**, 319 connected groups; executed evaluation coverage is eight previously seen cases in seven families. The suite has 28 real inputs from 23 PRs plus eight synthetic controls. Named reviewer feedback, author measurements/rationale, explicit automation, uncertain composition and request-origin controls remain distinct; no unaided-human-composition certification is inferred. |
| Delivery scope and local checks | Aggregate and pending changes are limited to the five authorized paths; prior commits are preserved. No required reviewer/evaluator writer remains active. `git diff --check` and `git diff --cached --check` pass; explicit-path staging produced exactly the five authorized files, and the staged candidate matches the normalized identity above. The agent, skill, corpus and evidence bytes are unchanged during this audit; only this document received navigation/status additions. No compiler build/test, benchmark reproduction, formatting sweep, product release note, push or GitHub post is part of this Markdown-only delivery. |

Real benchmark-only PR coverage remains **zero**; BENCH-01 is synthetic contract coverage, not human grounding or a real holdout. Deleted/private evidence, mutable historical edits, timeline-total anomalies and unavailable Azure payloads retain their recorded limits. Replays do not create independent holdouts, and source-derived failure scenarios are not executed product failures.
