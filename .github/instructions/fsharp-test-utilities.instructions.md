---
applyTo: 'tests/FSharp.Test.Utilities/**'
---

Compiler test infrastructure: the FSharp.Test.Utilities `Compiler` DSL drives compile/run assertions and baseline (.bsl/.err) comparison, refreshed via TEST_UPDATE_BSL.

- Set `TEST_UPDATE_BSL` only when intentionally refreshing baselines, and re-run the same test to rewrite the expected file.
- Compose `verifyBaselines` from both `verifyBaseline` and `verifyILBaseline`; never verify source baselines without also checking IL when a test expects both.
- Use `Compiler.checkBaseline` and `Compiler.verifyBaselines` to keep .bsl/.err outputs normalized and comparable.
- Use the binding-specific reflection entry points (`getMethod`, `getPrivateInstanceMethod`, `getPublicInstanceMethod`) instead of ad hoc lookup so access flags stay explicit.
- Use `ExecuteAux` `beforeExecute` hook to copy dependencies before launch, especially when `newProcess` is set, or the spawned app may miss local assemblies.
- Verify IL through `checkIL` and `verifyIL` rather than raw string comparison so normalization and fragment matching stay stable across ILDASM variants.
