---
name: "Performance celebration article"
description: "Write evidence-backed F# release performance articles from requested measurements or existing results, with concise celebratory prose, honest comparisons, useful charts, and contributing PRs."
---

# Performance celebration article

Create the article and its supporting presentation, not an experiment proposal or empty tables. Let the task supply releases, features, metrics, workloads, timeframe, and output location; do not carry these over from a previous article. Separate evidence collection from reader-facing presentation.

## Editorial priorities

Write for F# developers, with friendly, evidenced optimism. Use closures, lambdas, functions, collection operations, and stated input sizes, not generic "callbacks" or "kernels". Preserve technical identifiers, raw benchmark fields, and original source metadata; paraphrase display labels and PR summaries instead.

Do not restate basics, explain unit conversions, repeat definitions, or narrate obvious experimental controls such as "the same frozen inputs, references, and arguments". Enforce those controls internally and document them in linked methodology. Keep only caveats that change interpretation in the article, briefly: for example, retained heap is not peak RAM, compilation is not a full build, and a source-built snapshot is not a shipped SDK. Concise marketing must not conceal material limitations.

## Evidence behind the article

First inspect available results and their provenance. For presentation-only work, reuse completed evidence rather than rerun collection. When collection is requested, design and execute a sound experiment with representative workloads, appropriate repetitions, and controls for variability and correctness; choose and calibrate suitable collectors or profilers, account for instrumentation overhead, and label evidence for what it measures: sampled estimates and static counts are not exact runtime totals. Preserve raw observations and failed or inconclusive runs; add new runs without overwriting prior data. Track substantial work and evidence pointers in the session database/TODOs, not a proliferation of planning reports. If evidence is missing, state the gap rather than invent numbers or present a design as completed measurement.

Run independent measurements serially, without competing benchmark jobs or agents. This concerns interference between experiments, not concurrency inside the workload: preserve intended parallel execution when measuring multicore performance. Record commands, inputs, workload revisions, hardware, runtime, configuration, and relevant controls sufficiently to reproduce the results. Keep unchanged positive and negative controls in the evidence and relevant comparison tables.

Use the previously released version users actually have as the baseline, and an optimized, source-built candidate or development snapshot of the upcoming release as the primary comparison. Identify the exact tested source revisions, runtime, packaging, and actual binaries used. Do not label a development snapshot as a tested GA SDK. Verify feature and PR membership in the measured source through ancestry and source inspection, not branch names or merge status alone.

An intermediate prerelease is optional context, never a replacement denominator: compare both intermediate and upcoming versions against the released baseline. Retain relevant runtime controls in supporting evidence. Where host or runtime coverage materially matters, distinguish what was actually measured with simple released-versus-new comparisons, not an exhaustive runtime permutation table.

## Build the release story

Open with a short celebration of the effort and practical benefits: a verified count of relevant merged PRs over a stated real timeframe, the measured headline improvement range, and what is already available versus coming in the next milestone or GA. Derive these claims from the evidence and release membership; never insert a canned season, count, gain, or availability promise.

Make the main results table the main story, followed by useful charts, then the contributing PRs. Select one or two metrics with the strongest credible gains across representative projects. Keep the representative workload set, not only successful projects. Omit inconclusive metrics from the headline, retain all underlying results, and explicitly disclose material regressions. Name the metric actually improved; a reduction in one resource does not establish that everything is faster or cheaper.

Explain one particularly meaningful mechanism with its PR link when supported. If it benefits users' own programs or libraries beyond compiler/tool performance, explain that benefit and the adoption requirement. Do not force a downstream feature story when none exists. Small application examples should describe understandable F# operations with relevant input or workload sizes. Usually show only released versus new; add intermediate or runtime distinctions only when they change the story. Selected examples do not establish a benefit for every F# program.

## Tables and charts

Generate tables and charts from full-precision observations, with consistent aggregation and accurate units; round only for display. Show direct values in meaningful units appropriate to the metric, not synthetic indices or "old = 100" scores. Percentage changes accompany actual values and always use the released baseline denominator. Label the direction correctly: lower duration and higher throughput are different kinds of improvement.

Use zero-based bars and honest axes. Per-workload bar scaling is acceptable if briefly stated, but numeric labels must remain actual values in real units. Prefer legible comparisons to dense multi-runtime tables. Preserve links to source data and reproducible generation steps. Validate calculations, labels, ranges, and denominator consistency numerically, and inspect the rendered tables/charts visually for misleading scales, clipping, and readability.

## Contributing PRs and delivery

After the measured results, provide one overall PR list ordered by actual merge time, with at most one clear release cut-line where useful. Verify that cut-line against source ancestry, not dates alone. Count distinct relevant PRs actually included in the release source; exclude experimental or unshipped branches. Group genuinely repetitive changes, but list partially unique contributions separately. Do not add together PR-local improvement percentages or assign causal shares of an aggregate gain without isolated measurements.

Deliver the finished article with usable tables/charts and linked evidence, within the requested scope. Before finishing, check every headline, availability claim, PR count, and example against its source; ensure material regressions remain visible. Remove basic explanations, redundant methodology narration, and non-idiomatic F# prose without deleting interpretation-changing caveats.
