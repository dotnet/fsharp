# Presentation reproduction

Read [Performance improvements in the F# 11 compiler](article.md). The comparison is the released SDK 10.0.100 compiler versus source-built F# 11, with RC1 as an intermediate compiler data point. Three charts show compiler allocation, the same old/new comparison on .NET Framework, and retained IDE managed heap. The callback table compares only old and new across all eight length-four kernels, including unchanged controls.

## Regenerate without running benchmarks

Requires Node.js 18 or newer, with no packages or network access. From the repository root:

```powershell
node .\docs\fsharp11-performance\presentation\generate.mjs
node .\docs\fsharp11-performance\presentation\generate.mjs --check
```

The generator resolves inputs relative to itself, so it also works from another current directory. It only writes `article.md`, `compiler-allocation.svg`, `runtime-allocation.svg`, `ide-retained-heap.svg` and `chart-data.json` in this directory. Authored prose stays outside `generated` markers in the article; keep those markers when editing. `--check` writes nothing and fails on stale output, missing data, changed sample counts, inconsistent published medians or wrong portable percentage denominators.

`chart-data.json` preserves exact byte medians, decimal GB/MB display values and reduction percentages, with JSON pointers back to each source value. It includes SHA-256 hashes of input evidence bytes and a hash of the generator after LF normalization, plus payload identities, counts, the chronological PR inventory and the explicitly discussed RAM/CPU regressions. Generated text is compared after LF normalization so checking works with either Git checkout line-ending convention. There are no generated timestamps, locale-dependent number formats, random numbers, or moving source references.

Scalar values come from each campaign's `scalar_statistics` and are cross-checked against its 12 non-warmup observations per cell. Application medians are checked against the three launch records. Reductions use **100 times (1 minus the candidate median divided by the released compiler median)**; positive means less allocation/retention. Both RC1 and source-built percentages use the released baseline. They are not medians of round-paired ratios. The Framework comparison keeps both compiler generations on Framework. No means across projects or pooled cohorts are reported.

The SVGs have white backgrounds, zero baselines, directly labeled GB/MB values, accessible titles/descriptions and companion Markdown tables. GB means 1,000,000,000 bytes; MB means 1,000,000 bytes. The original raw datasets and their GiB/MiB tables are unchanged. SDK 10/old is slate, RC1 is blue, and source-built/new is teal.

Only allocation **bar widths** scale per project: each project's largest value fills the available width, and the other versions use that same within-project scale. Displayed values are never converted into an index. The IDE chart uses a shared 0-600 MB scale. The Framework chart has only the released and source-built compiler on Framework, not the full runtime-control matrix.

The opening PR count is calculated from audited mainline ancestry and merge dates, not all search hits. Every distinct mainline PR is listed separately in merge order, with one checked RC1 membership cut-line; experimental-branch merges are excluded from that count. The wider runtime controls, application sizes and all historical observations remain linked rather than being rewritten to match the narrower presentation.

## Frozen inputs, separate experiments

The evidence checkpoint is `bc90ee6637fc9c7065da173bd36c8b06e2a8d6d4`. The [SDK/R2R methodology](../third-wave/README.md) and [portable-payload methodology](../framework/README.md) describe original acquisition and measurement. This presentation does not rebuild payloads, rerun measurements, republish old data or repair historical paths. The [original campaign](../README.md#preserved-first-campaign) also remains intact.

The SDK cohort changes compiler/Core, runtime and packaging together; its local payload is not an official RC2 SDK. The portable cohort holds each IL-only netstandard2.0 compiler/Core payload fixed across three runtime families. These cohorts must stay separate. Exact source commits, runtime versions, raw records, output-identity caveats and the contributor ancestry audit are linked from the article.
