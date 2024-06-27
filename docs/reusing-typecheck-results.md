---
title: Reusing typecheck results
category: Compiler Internals
categoryindex: 200
index: 42
---

# Reusing typecheck results between compiler and tooling runs

Caching and reusing typecheck results between compiler and tooling runs will be an optimization technique aimed at improving the performance and efficiency of the F# development experience.

### Why is it important

- **Performance**. Recomputing type information for large projects is time-consuming. By caching the results, subsequent runs can skip this step, leading to faster builds.

- **IDE responsiveness**. IDEs can provide faster IntelliSense, code navigation, and other features if they can quickly access cached type information.

- **Error reduction**. Consistent typechecking results help avoid scenarios where slight differences between runs could cause different errors or warnings to be produced.

### General considerations

- The technique will be implemented separately for the compiler and tooling, for better granularity and trackability.

- Caching data will likely be unsophisticated, the harder part will be to understand what data can be reused and how.

## Detailed design: compiler layer

### Caching

- Cache data to the "obj" folder, next to other intermediary compilation information.
- Do this during Phase 6 of the compilation, next to writing binaries (`main6`).
- Store the data either in binary or in text, as with other things in "obj". The decision can maybe be made based on the speed of serialization/deserialization of different formats.

### Reusing

- Start reading data asynchronously in the beginning of compilation, around importing assemblies (beginning of `main1`).
- Await the data just before the type check (middle of `main1`).
- If there is no data, do type check as usual.
- If there is some data, see what can be reused.

### What data to store?

- Start with storing full typecheck data.
- Later parsing data can be added if we find a good scenario for having just that. 

### How to reuse data?

- Be defensive - if there's not 100% guarantee something can be reused, better just invalidate it.
- We need to understand how to reuse data **partially**. Probably can be deduced based on AST diff in parsing?

### How to test this?

- Have a feature flag for this: applying it to current type checking tests shouldn't make any difference in results.
- Unit tests: original code + partial typecheck results + reusing them = original code + fresh typecheck

### How to bench this?

- Add benchmarks to the FCSBenchmarks project
- A good inspiration can be workflow-style tests for updating files done by @0101 in Transparent Compiler

---

## Detailed design: tooling layer

-- TO BE DONE --