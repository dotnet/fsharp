# LabelOps — PR Security Scan

Hourly scan of open PRs from external contributors. Reads the diff (text only — never builds or checks out) and labels PRs with risk categories so maintainers know what they're dealing with before building locally or in Copilot.

## Labels

| Label | Meaning |
|---|---|
| `⚠️ Affects-Build-Infra` | PR modifies files that execute during build (scripts, .props, .targets). Building this PR runs the contributor's code. |
| `⚠️ Affects-Compiler-Output` | PR modifies IL emission or code generation. Compiled output may differ from expectations. |
| `⚠️ Affects-Bootstrap` | PR modifies the compiler bootstrap chain. The compiler builds itself — this is the highest-risk category. |
| `⚠️ Prompt-Injection-Risk` | PR modifies AI agent instructions, skills, or workflows. |
| `⚠️ Package-Supply-Chain` | PR adds or changes NuGet packages or feeds. |
| `⚠️ Scope-Review-Needed` | PR does clearly more than its title/description claims. |
| `AI-Security-Scan-Clean` | No risk indicators found. Applied so future scans skip the PR. |

## Trusted authors (skipped)

`T-Gro`, `abonie`, `dotnet-bot`, `dotnet-maestro`, `dotnet-maestro[bot]`, `copilot`, `github-actions[bot]`

## Setup (one-time)

```bash
gh label create "AI-Security-Scan-Clean" --repo dotnet/fsharp --color 0e8a16 \
  --description "LabelOps: security scan found no risk indicators"
gh label create "⚠️ Affects-Build-Infra" --repo dotnet/fsharp --color d93f0b \
  --description "LabelOps: PR modifies build infrastructure"
gh label create "⚠️ Affects-Compiler-Output" --repo dotnet/fsharp --color d93f0b \
  --description "LabelOps: PR modifies IL emission or codegen"
gh label create "⚠️ Affects-Bootstrap" --repo dotnet/fsharp --color b60205 \
  --description "LabelOps: PR modifies compiler bootstrap chain"
gh label create "⚠️ Prompt-Injection-Risk" --repo dotnet/fsharp --color d93f0b \
  --description "LabelOps: PR modifies AI agent instructions or skills"
gh label create "⚠️ Package-Supply-Chain" --repo dotnet/fsharp --color d93f0b \
  --description "LabelOps: PR adds or changes NuGet packages"
gh label create "⚠️ Scope-Review-Needed" --repo dotnet/fsharp --color fbca04 \
  --description "LabelOps: PR scope exceeds title/description"
```

## Workflow

[`.github/workflows/labelops-pr-security-scan.md`](../.github/workflows/labelops-pr-security-scan.md)
