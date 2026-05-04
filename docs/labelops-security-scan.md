# PR Tooling Safety Check

**What this is:** An hourly scan that labels open PRs with what development phases they affect — restore, build, bootstrap, test, design-time, or AI agent config. Helps maintainers know what they're touching before building or testing locally or in Copilot.

**What this is NOT:** Not a code review. Not a merge-readiness signal. Not a guarantee of safety. Not a replacement for human review. A clean label means "no interesting infrastructure files touched" — it says nothing about code quality.

## State machine

| What you see | What it means |
|---|---|
| **No label** | Scan hasn't run yet. Treat as unscanned. |
| **`AI-Tooling-Check-Clean`** | Scanned — PR touches only regular source/test/doc files. |
| **One or more `⚠️ Affects-*`** | Scanned — PR touches files in those phases. Review with care. |

Trusted authors (`T-Gro`, `abonie`, `dotnet-bot`, `dotnet-maestro`, `copilot`, `copilot-swe-agent`, `github-actions`, `github-actions[bot]`) get `AI-Tooling-Check-Clean` immediately without diff analysis.

## Labels

| Label | What it means |
|---|---|
| `⚠️ Affects-Build-Infra` | Scripts, .props, .targets, MSBuild tasks, global.json |
| `⚠️ Affects-Restore` | NuGet packages, feeds, version pinning, dependency manager |
| `⚠️ Affects-Bootstrap` | PROTO compiler chain, lexer/parser generators |
| `⚠️ Affects-Compiler-Output` | IL emission, codegen, typed tree serialization |
| `⚠️ Affects-Test-Infra` | Test framework utilities (not test cases) |
| `⚠️ Affects-Design-Time` | Type providers, dependency manager, IDE integration |
| `⚠️ Prompt-Injection-Risk` | AI agent instructions, skills, workflows, or SAGE-class injection patterns in diff text |
| `⚠️ Scope-Review-Needed` | Diff does more than title/description claims |
| `AI-Tooling-Check-Clean` | None of the above triggered |

## Methodology

Based on established security frameworks and threat models:

| Source | What it covers |
|--------|---------------|
| [Microsoft — MSBuild Security Best Practices](https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-security-best-practices) | Build infra, NuGet package execution, parent-folder imports |
| [MITRE ATT&CK T1127.001](https://attack.mitre.org/techniques/T1127/001/) | MSBuild inline task code execution |
| [OWASP LLM Top 10 2025](https://genai.owasp.org/llm-top-10/) | Prompt injection (LLM01), excessive agency (LLM06) |
| [OWASP AI Agent Security Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/AI_Agent_Security_Cheat_Sheet.html) | Tool abuse, goal hijacking, supply chain attacks |
| [GitHub — Security Architecture of Agentic Workflows](https://github.blog/ai-and-ml/generative-ai/under-the-hood-security-architecture-of-github-agentic-workflows/) | Safe outputs, agent isolation, zero-secret agents |
| [OpenAI — Safety in Building Agents](https://developers.openai.com/api/docs/guides/agent-builder-safety) | Structured outputs, prompt injection via tool calls |
| [Anthropic — Computer Use Security](https://platform.claude.com/docs/en/agents-and-tools/tool-use/computer-use-tool) | Network egress control, filesystem isolation |
| [Peli's Agent Factory — Security Workflows](https://github.github.com/gh-aw/blog/2026-01-13-meet-the-workflows-security-compliance/) | Daily malicious code scan pattern |
| [Gen Digital SAGE — Prompt Injection Rules](https://github.com/gendigitalinc/sage/blob/main/threats/prompt-injection.yaml) | 9-family prompt injection heuristic taxonomy (CLT-PI-001–081) |

## Setup (one-time)

```bash
gh label create "AI-Tooling-Check-Clean" --repo dotnet/fsharp --color 0e8a16 \
  --description "Tooling check: no interesting infrastructure files touched"
gh label create "⚠️ Affects-Build-Infra" --repo dotnet/fsharp --color d93f0b \
  --description "Tooling check: PR touches build infrastructure"
gh label create "⚠️ Affects-Restore" --repo dotnet/fsharp --color d93f0b \
  --description "Tooling check: PR touches NuGet packages or feeds"
gh label create "⚠️ Affects-Bootstrap" --repo dotnet/fsharp --color b60205 \
  --description "Tooling check: PR touches compiler bootstrap chain"
gh label create "⚠️ Affects-Compiler-Output" --repo dotnet/fsharp --color d93f0b \
  --description "Tooling check: PR touches IL emission or codegen"
gh label create "⚠️ Affects-Test-Infra" --repo dotnet/fsharp --color d93f0b \
  --description "Tooling check: PR touches test framework infrastructure"
gh label create "⚠️ Affects-Design-Time" --repo dotnet/fsharp --color d93f0b \
  --description "Tooling check: PR touches type providers or dependency manager"
gh label create "⚠️ Prompt-Injection-Risk" --repo dotnet/fsharp --color d93f0b \
  --description "Tooling check: PR modifies AI agent instructions or contains injection patterns"
gh label create "⚠️ Scope-Review-Needed" --repo dotnet/fsharp --color fbca04 \
  --description "Tooling check: PR scope exceeds title/description"
```

## Workflow

[`.github/workflows/labelops-pr-security-scan.md`](../.github/workflows/labelops-pr-security-scan.md)
