# Presentation reproduction

Read [Performance improvements in the F# 11 compiler](article.md). The main table includes all six compilation workloads; three charts explain SDK allocation, the separate portable/Framework comparison, and retained IDE managed heap. All eight application kernels at length four remain visible, including unchanged controls.

## Regenerate without running benchmarks

Requires Node.js 18 or newer, with no packages or network access. From the repository root:

```powershell
node .\docs\fsharp11-performance\presentation\generate.mjs
node .\docs\fsharp11-performance\presentation\generate.mjs --check
```

The generator resolves inputs relative to itself, so it also works from another current directory. It only writes `article.md`, `compiler-allocation.svg`, `runtime-allocation.svg`, `ide-retained-heap.svg` and `chart-data.json` in this directory. Authored prose stays outside `generated` markers in the article; keep those markers when editing. `--check` writes nothing and fails on stale output, missing data, changed sample counts, inconsistent published medians or wrong portable percentage denominators.

`chart-data.json` preserves exact medians, display-unit values, normalized chart values and reduction percentages, with JSON pointers back to each source value. It includes SHA-256 hashes of input evidence bytes and a hash of the generator after LF normalization, plus payload identities, counts, contribution groups and the explicitly discussed RAM/CPU regressions. Generated text is compared after LF normalization so checking works with either Git checkout line-ending convention. There are no timestamps, locale-dependent number formats, random numbers, or moving source references.

Scalar values come from each campaign's `scalar_statistics` and are cross-checked against its 12 non-warmup observations per cell. Application medians are checked against the three launch records. Reductions use **100 times (1 minus the newer median divided by the older median)**; positive means less allocation/retention. They are not medians of round-paired ratios. Framework and modern portable comparisons each use their own older-runtime baseline. No means across projects or pooled cohorts are reported.

The SVGs have white backgrounds, explicit units, zero baselines, direct values, accessible titles/descriptions and companion Markdown tables. SDK 10/old is slate, SDK 11 RC1 is blue, local/new is teal. The portable chart uses slate tracks at 100 and teal new-payload bars; panel headings identify runtimes rather than introducing another color meaning.

Presentation verification independently recalculated 252 scalar/application values and 36 reduction denominators, matched the displayed values to the existing CSVs, and checked all 42 SVG bar lengths against the exact data. All three SVGs were rasterized with resvg and visually inspected for clipping, overlap, labeling and scale. This was an SVG-render review, not a screenshot of an editor or browser. The optional review renderer is not needed to regenerate or check the committed artifacts.

## Frozen inputs, separate experiments

The evidence checkpoint is `bc90ee6637fc9c7065da173bd36c8b06e2a8d6d4`. The [SDK/R2R methodology](../third-wave/README.md) and [portable-payload methodology](../framework/README.md) describe original acquisition and measurement. This presentation does not rebuild payloads, rerun measurements, republish old data or repair historical paths. The [original campaign](../README.md#preserved-first-campaign) also remains intact.

The SDK cohort changes compiler/Core, runtime and packaging together; its local payload is not an official RC2 SDK. The portable cohort holds each IL-only netstandard2.0 compiler/Core payload fixed across three runtime families. These cohorts must stay separate. Exact source commits, runtime versions, raw records, output-identity caveats and the contributor ancestry audit are linked from the article.
