# Postmortems

Detailed write-ups of bugs that were hard to diagnose, had non-obvious root causes, or taught us something worth preserving. Each document captures the symptoms, root cause, fix, and timeline so that future contributors can recognize similar patterns early.

These are referenced from [agentic instructions](../../.github/instructions/) and serve as deeper reading — the instructions tell you *what* to do, the postmortems explain *why* the rules exist.

## Index

- [`regression-fs0229-bstream-misalignment.md`](regression-fs0229-bstream-misalignment.md) — a conditional write with an unconditional read shifted the pickle B-stream, producing `FS0229` when reading older metadata.
- [`regression-legacy-inline-metadata-dynamic-invocation.md`](regression-legacy-inline-metadata-dynamic-invocation.md) — a new inline-flag case reused a serialized bit pattern that already meant "required inline" in F# 5 binaries, breaking cross-assembly SRTP at runtime.
- [`regression-sourcebuild-cpm-runtime-version-floor.md`](regression-sourcebuild-cpm-runtime-version-floor.md) — renaming the CPM runtime-package pins to computed `$(System*CentralVersion)` aliases with a floor defeated source-build's `$(System*Version)` override, causing prebuilt/`NU1109` failures in the VMR that fsharp CI could not see.
- [`regression-parse-tree-fidelity-return-attributes.md`](regression-parse-tree-fidelity-return-attributes.md) — a semantic lowering moved into the parser made `SynBinding.attributes` drop `[<return: X>]`, so tools reading the untyped tree silently deleted attributes the source visibly had.
