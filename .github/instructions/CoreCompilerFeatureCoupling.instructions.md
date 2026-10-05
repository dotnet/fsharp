---
applyTo:
  - "src/FSharp.Core/prim-types.{fs,fsi}"
  - "src/FSharp.Core/list.{fs,fsi}"
  - "src/FSharp.Core/array.{fs,fsi}"
  - "src/Compiler/Facilities/LanguageFeatures.{fs,fsi}"
  - "src/Compiler/Checking/PostInferenceChecks.fs"
  - "src/Compiler/Checking/Expressions/CheckExpressions.fs"
  - "src/Compiler/Checking/SignatureConformance.fs"
  - "src/Compiler/Optimize/Optimizer.fs"
  - "src/Compiler/TypedTree/TypedTree.{fs,fsi}"
  - "src/Compiler/TypedTree/TypedTreePickle.{fs,fsi}"
  - "src/Compiler/TypedTree/TypedTreeOps.Attributes.fs"
  - "src/Compiler/TypedTree/WellKnownAttribs.{fs,fsi}"
  - "tests/FSharp.Compiler.ComponentTests/EmittedIL/OptimizeClosureIfNotInlined.fs"
  - "tests/FSharp.Compiler.ComponentTests/Conformance/Signatures/SignatureEnforcedAttributes.fs"
---

# FSharp.Core/compiler feature coupling

FSharp.Core is built by multiple compiler generations. An attribute or optimization flag used by Core can be ordinary metadata to the stage-1 compiler but a language-gated construct to the freshly built compiler.

- When FSharp.Core unconditionally defines or applies a compiler-recognized construct, assign its `LanguageFeature` to the lowest language version used to build that source. Do not leave it preview-only when Proto builds Core at a stable version; otherwise make the Core usage conditional.
- Test successful declaration at the assigned version and FS3350 at the preceding version. If imported metadata consumption is intentionally version-independent, compile the library at the assigned version and consume it at an older language version.
- Rebuild Proto FSharp.Core at the assigned language version with the freshly built Release compiler. A stage-1 or ordinary source-build is insufficient because an older compiler can treat a new attribute as ordinary metadata.
- For serialized optimization flags, preserve old-reader layout and correctness. Verify that an older compiler can consume the newer Core, and assess the performance fallback when it ignores the new optimization metadata.

See `docs/postmortems/near-miss-core-language-feature-gate-skew.md` for the stage-2 failure that established this contract.
