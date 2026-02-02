---
name: ILVerify failure
description: Fix ilverify
---

# ILVerify Baseline

## When
IL shape changed (codegen, new types, method signatures)

## Update
```bash
TEST_UPDATE_BSL=1 pwsh tests/ILVerify/ilverify.ps1
```

## Baselines location
`tests/ILVerify/*.bsl`

## Verify
Re-run without `TEST_UPDATE_BSL=1`, should pass.
