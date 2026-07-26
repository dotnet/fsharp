---
name: binlog-analysis
description: >-
  Triage a build / compile / restore / WarnAsError failure from its MSBuild
  binary log. Fetches the binlog (a local build's, or a failed dotnet/fsharp
  Azure DevOps PR build's published artifact) and analyzes it live via the
  `binlog-mcp` MCP server — structured errors, root-cause diagnosis, and an
  MSBuild perf X-ray. NOT for test failures or CheckCodeFormatting: a build
  binlog has no errors there.
---

# Binlog Analysis (via the binlog-mcp MCP server)

Boil a failed build down to root causes. This skill does two small things and
delegates the heavy lifting to an MCP server:

1. **Fetch** the build's `*.binlog` — from your local build, or from a failed
   `fsharp-ci` Azure DevOps PR build (downloads the published artifact).
2. **Hand the path to the `binlog-mcp` MCP server** (`Microsoft.AITools.BinlogMcp`),
   which queries the binlog live (≈38 tools): structured errors, categorized root
   causes, and an MSBuild X-ray (target/task/analyzer timings, incrementality,
   double-writes, …).

Because the analysis lives in the MCP server, this skill stays tiny — and gets
better automatically as that server gains features.

## When to use

- A local build (with `-bl`) or a failed `fsharp-ci` PR build broke and you need
  to know **why** — compile / restore / analyzer / WarnAsError errors, or build
  perf.

## When NOT to use

- **Test** failures or **CheckCodeFormatting**. The build binlog has no errors in
  that case: `binlog_overview` will show the build succeeded / 0 errors — stop and
  use `pr-build-status` / `flaky-test-detector` instead.

## Step 1 — get the binlog path

```pwsh
# Local: newest *.binlog under <repo>/artifacts/log (build first, e.g. ./build.sh --binaryLog)
pwsh .github/skills/binlog-analysis/scripts/Get-Binlog.ps1

# Local: a specific file, directory, or glob
pwsh .github/skills/binlog-analysis/scripts/Get-Binlog.ps1 -BinlogPath artifacts/log/Debug/Build.binlog

# Azure DevOps: latest FAILED fsharp-ci build for a PR (downloads + keeps the binlog)
pwsh .github/skills/binlog-analysis/scripts/Get-Binlog.ps1 -PrNumber 19941

# Azure DevOps: explicit build id; -AllLegs for every leg; -Json for a path list
pwsh .github/skills/binlog-analysis/scripts/Get-Binlog.ps1 -BuildId 1462217 -Json
```

It prints the resolved `*.binlog` path(s) (Azure DevOps artifacts are downloaded
to a temp folder and kept so the MCP can read them).

## Step 2 — analyze via the binlog-mcp MCP tools

With each path, call the MCP server (the argument is `binlog_file`):

- `binlog_overview` — build status + error/warning counts. **Call this first** to
  decide whether there's anything to analyze.
- `binlog_diagnose` — categorized root causes + next-step hints.
- `binlog_errors` / `binlog_warnings` — structured diagnostics (code / file / line
  / column / project).
- `binlog_search` — free-form drill-down.
- Perf: `binlog_expensive_targets` / `binlog_expensive_tasks` /
  `binlog_expensive_analyzers`, `binlog_incremental_analysis`,
  `binlog_double_writes`, `binlog_target_graph`.

> **Multi-targeting note.** Today `binlog_errors` returns one row per target
> framework, so a single source error in a multi-TFM project (e.g.
> FSharp.Compiler.Service → `net10.0;netstandard2.0`) appears once per TFM. A
> lossless dedup (`code,file,line` → set of TFMs) is proposed upstream in
> `dotnet-microsoft/ai-tools`; when it lands, this skill gets the deduped view for
> free — no change here.

## Prerequisites

- The **binlog-mcp MCP server** registered with your agent. The tool is pinned in
  the repo's `.config/dotnet-tools.json` (`Microsoft.AITools.BinlogMcp`); run
  `dotnet tool restore` once, then register it as an MCP server. For example,
  Copilot CLI (`~/.copilot/mcp-config.json`):
  ```jsonc
  { "mcpServers": { "binlog-mcp": {
      "command": "dotnet", "args": ["tool", "run", "binlog-mcp"],
      "tools": ["*"], "deferTools": "auto" } } }
  ```
  (gh-aw: a `mcp-servers:` block; VS Code: `.vscode/mcp.json`. Telemetry is on by
  default — opt out with `DOTNET_CLI_TELEMETRY_OPTOUT=1` if desired.)
- PowerShell 7+ (`pwsh`) and a .NET 10 SDK (already required by the repo).
- Azure DevOps modes need network access to `dev.azure.com`.
