---
name: ilverify-failure
description: Fix ILVerify baseline failures when IL shape changes (codegen, new types, method signatures). Use when CI fails on ILVerify job.
---

# ILVerify Baseline

## When to Use
IL shape changed (codegen, new types, method signatures) and ILVerify CI job fails.

## Offset-Only Differences
Changes that only shift IL byte offsets (`[offset 0x...]`) — for example adding code above an existing error site — are detected automatically and **do not fail CI**. A warning is printed suggesting a baseline update. No action is required, but refreshing baselines keeps them accurate.

## Update Baselines
```bash
TEST_UPDATE_BSL=1 pwsh tests/ILVerify/ilverify.ps1
```
Or use the `/run ilverify` PR comment command to update baselines via CI.

## Baselines Location
`tests/ILVerify/*.bsl`

## Verify
Re-run without `TEST_UPDATE_BSL=1`, should pass.
