---
applyTo:
  - "src/Compiler/Checking/CheckBasics.{fs,fsi}"
  - "src/Compiler/Checking/ConstraintSolver.{fs,fsi}"
  - "src/Compiler/Checking/NameResolution.{fs,fsi}"
  - "src/Compiler/Checking/TraitConstraintScope.{fs,fsi}"
  - "src/Compiler/Checking/QuotationTranslator.{fs,fsi}"
  - "src/Compiler/Checking/infos.{fs,fsi}"
  - "src/Compiler/TypedTree/TypedTree.{fs,fsi}"
  - "src/Compiler/TypedTree/TypedTreeOps.Remap.{fs,fsi}"
  - "src/Compiler/TypedTree/TypedTreeOps.ExprConstruction.{fs,fsi}"
  - "src/Compiler/TypedTree/TypedTreePickle.{fs,fsi}"
  - "src/Compiler/Optimize/Optimizer.{fs,fsi}"
---

# Extension-SRTP evidence

Selected witnesses belong to individual applications, not only to their types or source ranges.

- Keep selected evidence on the supporting cells of each application. Read these cells before normalization or unification removes them.
- For an unresolved same-assembly trait, use the definition scope.
- For an unresolved imported trait, use the consumer scope.
- For an imported selected witness, keep the selection.
- When the compiler copies types or values, remap the captured scope identities with them.
- Do not pickle a captured `TcEnv`. Metadata can store a selected witness only.
- Keep quotation witness order and built-in operator priority.
- For extension-constraint calls, a cache must distinguish selected witnesses. If its key cannot distinguish these selections, exclude such calls.

Test the combined case: an unopened consumer, competing witnesses, and imported calls with equal types.

See the [postmortem](../../docs/postmortems/regression-extension-srtp-evidence-across-files.md) for the violated evidence contract.
