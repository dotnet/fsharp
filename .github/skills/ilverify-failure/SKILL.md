---
name: ilverify-failure
description: Fix ILVerify baseline failures when IL shape changes (codegen, new types, method signatures). Use when CI fails on ILVerify job.
---

# ILVerify Baseline

## When to Use
IL shape changed (codegen, new types, method signatures) and ILVerify CI job fails.

## Update Baselines
```bash
TEST_UPDATE_BSL=1 pwsh tests/ILVerify/ilverify.ps1
```

## Baselines Location
`tests/ILVerify/*.bsl`

## Verify
Re-run without `TEST_UPDATE_BSL=1`, should pass.
