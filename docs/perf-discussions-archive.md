This is just a typed version of [these notes](https://github.com/dotnet/fsharp/issues/16498), generated during perf discussions on summer of 2023. Can be used as a reference point.

---

# Comparisons
- OCaml
- Scala
- C#
- Rust

# Underlying problems
- LOH allocations
- Build does too much for deltas
- GC Gen 2

# Major problems
- Script start up CLI
- Build on Linux & Mac
- Glitches in test discovery
- Edit & test workflow
- Edit & run workflow
- Edit & check workflow
- Unnecessary rebuilds

# Hosted compiler problems
- Stamp overflow
- Non-deterministic
- Memory leaks
- Infinite loops
- Stay resident compiler
- Permission elevation

# Minor problems
- Benchmarking
- Squigglies
- Colorization
- Debug
- Press dot

# Incremental phases
- Incremental parsing file
- Incremental checking file
- Incremental optimization for deltas
- Cascading DLL builds
- Incremental DLL builds
- Incremental ILxGen for deltas
- Incremental assembly generation
- Incremental PDB generation for deltas

# Community guidelines
- Community leadership
- Community enablement for tool performance
- Performance acceptance criteria
- Performance running
- Docs for tooling performance