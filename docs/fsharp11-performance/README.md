# F# 11 performance article and data

Start with [Performance improvements in the F# 11 compiler](presentation/article.md): a six-workload allocation table, explanatory charts, visible .NET Framework coverage, retained IDE heap, application controls and contributing PRs. [Chart data and reproduction](presentation/README.md) derive the presentation from the frozen evidence below without replacing it.

The [detailed three-compiler article](third-wave/article.md) compares SDK 10.0.100, SDK 11 RC1, and a self-hosted Release/R2R compiler from the pinned VMR RC2 source. RC2 and the pinned VMR main have identical production sources. The third payload is a local build on .NET 11 RC1, not an official RC2 SDK. [New matrices, raw observations and reproduction](third-wave/README.md) are separate from the historical data below.

The [completed .NET Framework / .NET 10 / .NET 11 factorial](framework/README.md) adds the same two netstandard2.0 compiler/Core payloads on all three runtimes: 672 scalar observations, 36 instrumentation controls and 864 application launch/cases. Its [full tables](framework/tables.md) keep the IL-only runtime comparison separate from SDK/R2R results.

## Preserved first campaign

[The original measured article](article.md) compares **SDK 10.0.100** with **11.0.100-rc.1.26425.128**, never 10.0.4xx. The campaign ran serially on a shared Windows VM.

| Dataset | Coverage | Data |
|---|---|---|
| Full compilation | Six workloads, 9 or 15 pairs each, plus two warmups per arm | [Matrix](compilation-matrix.csv) |
| IDE-style checking | Two real production dependency graphs, 2 and 3 projects | [Matrix](ide-matrix.csv) |
| Runtime/DATAS controls | Two FCS comparisons, 15 pairs each | [Matrix](controls.csv) |
| Generated programs | Eight kernels, six sizes, two compiler/Core variants, three independent suite launches | [All launch results](generated-program-matrix.csv) |
| Allocation profiles | Two accepted Oxpecker traces, type identities resolved by module and metadata token | [Reports](profiles) |
| Shipping CLI equivalence | All 12 compiler/workload combinations emitted byte-identical DLLs through FCS and CLI | [Hashes](cli-equivalence.json) |

[results.json](results.json) contains scalar observations, warmups, rejected early Core observations, paired statistics, and derived program results. Full BenchmarkDotNet iterations and provenance are in [programs](programs). [Input snapshots](inputs), [calibration/tool evidence](provenance.json), and [compatibility decisions](compatibility.json) accompany the [experiment manifest](experiment.json).

The [executed protocol](experiment-design.md) states exactly what each metric covers. [The harness instructions](../../tests/benchmarks/FSharp11/README.md) describe acquisition, preparation, collection, and regeneration.

## Reading the story correctly

RC1 allocates less in all six compilation workloads, but peak RAM does not uniformly improve. The IDE-style graphs retain less managed memory. The selected length-four application kernels have unchanged B/op in all three launches.

The requested RC1 payload predates broad collection inlining, partial-application closure elimination, and cross-project imported-assembly sharing. The [52-PR contributor inventory](contributions.csv), covering auduchinok and T-Gro from April 24 through September 24, 2026, separates 12 entries in RC1's recorded source ancestry from 40 entries outside it. The later VMR audit also identified three experimental-branch merges outside RC2's ancestry. Do not attribute absent work to either binary.

Timings remain descriptive: continuous host-load screening was not collected. All scalar runs are retained, and peak-memory dispersion is published rather than assumed away. Larger FCS allocation traces failed the integrity gate and are excluded; the accepted Oxpecker traces report zero lost events. Large raw traces and preparation logs remain in the persistent artifact directory recorded in `provenance.json`, rather than being added as large repository binaries.
