# Performance-improvement review: frozen RED cases

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
