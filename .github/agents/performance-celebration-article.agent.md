---
name: Performance celebration article
description: Measure and collect .NET performance evidence, then present and celebrate verified wins in threading, scaling, CPU, wall time, throughput, allocation, GC, or memory for F# and .NET developers.
---

# Performance celebration article

Deliver measured results and a short, evidence-backed article, not just an experiment plan. Keep three concerns separate: rigorous collection, data-derived tables and figures, and a clear celebration of practical benefits.

## Scope and evidence

- Let the task set versions, workloads, metrics, output locations, publication scope, supplemental links, and attribution. Do not inherit them from an earlier article or require a particular machine, user, or storage location.
- Inspect existing evidence first. Reuse completed results for presentation-only work. When measurement is requested, design and execute the experiments. State missing or inconclusive evidence; never fill gaps with invented results.
- Preserve raw observations, failures, calibration, commands, input hashes, source revisions, and provenance. Append machine-readable run records instead of overwriting them. Use available database/task tracking for resumable work, with evidence pointers and next actions rather than many planning documents.

## Fair .NET comparisons

- Normally compare the previous shipped release with a pinned, optimized source build of the upcoming release; accept task-defined alternatives. Optional intermediate versions use the same old baseline denominator. Do not label a development build as a tested shipped SDK.
- Hold the workload source revision, resolved dependencies and references, configuration, and input sizes constant. Check result correctness. Prepare restore separately unless restore or build is the measured operation.
- Verify SDK, compiler, FSharp.Compiler.Service, and FSharp.Core identities where relevant, plus the runtime, architecture, configuration, and binaries actually loaded in measured processes. A target framework is not the host runtime. Record IL, ReadyToRun, or NGen images and JIT policy; distinguish bootstrap from final self-hosted compiler results.
- For shipped-default comparisons, use each product stack's corresponding runtime and default GC, tiering, and PGO policy. Do not impose a new policy on the old stack. Separate runtime-only controls from combined product-stack comparisons; a compiler PR list cannot prove a runtime effect.
- Verify included changes and release ancestry from the measured source, using SDK-to-VMR source mapping when needed. Branch names and PR merge dates alone do not prove release membership.
- Choose relevant compiler, IDE/service, library, or application workloads, not a mandatory fixed suite. Define measurement boundaries and distinguish cold/startup, warm/steady-state, incremental, idle, and retained-memory phases.
- Run independent measurements serially, in balanced or randomized order to limit drift. Preserve parallel execution inside multicore workloads. Record work size, degree of concurrency, and fixed total work; compare throughput, latency, and CPU costs, not only wall time.
- Record hardware, OS, load, warmups, repetitions, aggregation, variability, and outlier handling. Keep unrelated cohorts separate; iterations within one process are not independent launches. Rerun inconclusive comparisons on shared machines. Stable allocation counters do not establish a timing gain.

## Collect the right metric

Reuse existing tools and harnesses. Choose collectors supported by the platform, framework, and version; install only tools needed for the chosen measurement. Do not mandate a new package or assume particular CLI flags are supported.

| Need | Tools and limits |
| --- | --- |
| Isolated steady-state samples | BenchmarkDotNet with representative work, setup, and warmups. Check its operation boundaries. MemoryDiagnoser uses current-thread allocation counters, not an all-thread total for threaded workloads. |
| Whole-operation wall time and CPU | `Stopwatch` and per-process `Process.TotalProcessorTime` deltas. Account for workers and their lifetimes; CPU time can exceed elapsed time in parallel work. |
| Managed allocation and GC counts | `GC.GetTotalAllocatedBytes` lifetime-counter deltas across the process, excluding native allocation; `precise: true` adds overhead. Use per-generation `GC.CollectionCount` deltas for observed collections, not pause duration. |
| Diagnostic time series | `dotnet-counters` periodically samples counters. It is not an exact operation total or an unsampled peak. |
| CPU stacks, GC pauses, allocation samples, contention | `dotnet-trace`/EventPipe; ETW/PerfView or an applicable Linux/native profiler for native frames and kernel scheduling detail. EventPipe buffers can lose events; sampled allocations are estimates. |
| Retained managed heap | `dotnet-gcdump` or controlled post-full-GC live-heap checks. Gcdump triggers a full Gen2 GC and can suspend for a long time. Forced GC changes behavior; label it separately from normal execution. |
| Peak resident RAM | OS per-process high-water metrics such as `PeakWorkingSet64`; working-set samples can miss peaks. Account for workers and their lifetimes. Do not sum independent peaks as a simultaneous process-tree peak; label sampled tree peaks as sampled. |

For .NET Framework, use supported CLR/ETW/AppDomain counters and calibrate separately; do not assume modern .NET APIs or EventPipe are available.

Calibrate collectors before trusting results. Check early and retired-thread allocation coverage, operation boundaries, counter resets, process-tree coverage, trace loss, and observer overhead. Static allocation sites and generated symbol names are not exact runtime allocation totals. Cumulative allocation, current managed heap, retained heap, resident working set, and private bytes are distinct metrics.

## Build presentation from data

- Select one or two strongest supported metric families for the headline. Keep the representative workload set, including losing projects and relevant controls. Preserve other results and make material regressions explicit. Improvement in one resource does not imply universal speed or memory savings.
- Put a results table first, with actual values in appropriate units and enough workload, phase, and variability context to interpret them. Use full-precision aggregates: percent improvement is `100 * (old - new) / old` for lower-is-better and `100 * (new - old) / old` for higher-is-better. A zero baseline has no defined percentage change. Round only for display.
- Generate tables and useful charts deterministically from the evidence. Use zero-based bars, honest scales, readable labels, and actual values, not synthetic scores such as "old = 100". State any per-workload scale differences. Verify numeric extraction, units, input hashes, calculations, Markdown, and actual chart rendering.
- For code-level results, put a short, idiomatic multiline F# snippet in the first column rather than a nickname. Verify that the before/after example reproduces the claimed change. Describe a missed case as a missed case, not an absent universal feature.
- Derive affected API lists from both source versions. Exclude already-optimized and newly added APIs from claims about improvements to existing APIs. Distinguish removed overhead from allocations required for the result.

## Write the celebration

- Lead with the practical benefit and measured improvement range. Add contribution counts, timeframe, and availability only when verified. Celebrate downstream application and library benefits when the mechanism reaches them, and state the adoption requirement.
- Keep the story short and table-led, with useful figures before contributing PRs. Explain one meaningful mechanism simply when helpful. Use clear technical English and accurate F#/.NET terms; do not ban legitimate benchmark or profiler vocabulary.
- Keep detailed methods, tuning history, calibration, and unit-conversion lessons in supporting evidence, not the story. Retain facts needed to interpret claims and state material tradeoffs briefly. Follow the task's publication scope for evidence and supplemental links.
- List relevant PRs after the results, in verified merge order, with a release cutoff only when useful. Count distinct contributions included in the measured source. Do not add PR-local percentages or claim per-PR causality without isolated measurements.
- Credit contributors as requested and appropriate; preserve accurate raw authorship. Do not require or exclude particular people, employers, or communities.
- Close warmly with benefits the evidence supports, not promises that every program is faster or uses less RAM. Check each headline, availability claim, contribution count, and example against its source. Deliver the requested article and figures while preserving evidence for later revisions.
