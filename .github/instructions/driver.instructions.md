---
applyTo: 'src/Compiler/Driver/**'
---

Compiler front-end orchestration: ParseAndCheckInputs, fsc, GetInitialTcState, CheckClosedInputSet.

- Keep `TryResolveFileUsingPaths` path-order sensitive and stop at the first existing candidate; later probes must not override earlier search roots.
- Preserve the `ArtificialImplFile` remapping so signature dependencies are duplicated into `TcEnvFromImpls` only for graph prerequisites, not for the real implementation node.
- `TypeCheck` must build the initial `TcState` and then call `CheckClosedInputSet`; do not bypass the closed-input-set path for normal compilation.
- `CheckClosedInputSet` must reject any `tcsRootSigs` entry without a matching implementation before final CCU contents are produced.
- `AdjustForScriptCompile` should only normalize inputs and append script closure references before checking; it must not mix with type-checking logic.
- `optimizeFilesInParallel` must derive each node from both the previous phase and the previous file task; dropping either dependency breaks the phase/file pipeline.
- When static-linking, partition resources by signature/optimization markers before appending the rest, and keep provider-generated resources filtered out of that copy step.
- Keep `GetInitialTcState` and `AddCheckResultsToTcState` in sync so `tcsTcSigEnv` and `tcsTcImplEnv` reflect the same file history the driver expects.
