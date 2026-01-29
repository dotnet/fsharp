## Subtask 6 - Implement iteration 1 (2026-01-29 15:33:12)
-   **VERIFY_FAILED**

  The following issues need to be addressed:

  1. **`tests/ILVerify/ilverify.ps1` line 50**: Has hardcoded `net10.0` instead of using
  `$default_tfm`

  2. **Multiple EndToEndBuildTests `.fsproj` files**: Have hardcoded `net10.0` TargetFramework(s)
     - `tests/EndToEndBuildTests/BasicProvider/*.fsproj`
     - `tests/EndToEndBuildTests/ComboProvider/*.fsproj`

