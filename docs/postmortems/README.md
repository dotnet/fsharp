# Postmortems

Detailed write-ups of bugs that were hard to diagnose, had non-obvious root causes, or taught us something worth preserving. Each document captures the symptoms, root cause, fix, and timeline so that future contributors can recognize similar patterns early.

These are referenced from [agentic instructions](../../.github/instructions/) and serve as deeper reading — the instructions tell you *what* to do, the postmortems explain *why* the rules exist.

## Index

- [`regression-fs0229-bstream-misalignment.md`](regression-fs0229-bstream-misalignment.md) — a conditional write with an unconditional read shifted the pickle B-stream, producing `FS0229` when reading older metadata.
- [`regression-legacy-inline-metadata-dynamic-invocation.md`](regression-legacy-inline-metadata-dynamic-invocation.md) — a new inline-flag case reused a serialized bit pattern that already meant "required inline" in F# 5 binaries, breaking cross-assembly SRTP at runtime.
