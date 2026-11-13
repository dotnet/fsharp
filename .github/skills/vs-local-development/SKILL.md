---
name: vs-local-development
description: Guide for local Visual Studio extension development, building, deploying, and testing the VS extension (VisualFSharp) in the experimental instance.
license: MIT
---

# Local Visual Studio Extension Development

This skill provides guidance on building, deploying, and iterating on the Visual Studio extension locally using the experimental instance of Visual Studio.

## Prerequisites

- Visual Studio 2026 installed with the **Visual Studio extension development** workload
- The `vs-mcp-testing-tools` skill must be loaded to understand how to use the MCP tools for testing after deployment

## Rebuild & Deploy Workflow

After making code changes to the VS extension, follow these steps **in order** to rebuild and deploy:

### 1. Close All Experimental VS Instances

Run the helper script to close any running experimental instances:

```powershell
.github/skills/vs-local-development/scripts/close-exp-instances.ps1
```

This kills all `devenv.exe` processes launched with `/rootsuffix`.

### 2. Rebuild the Solution

Run the build script:

```powershell
./Build.cmd -c Debug
```

### 3. Deploy the VSIX

Use the deploy script to install the VSIX, wait for the installer to finish, and clear caches in one step:

```powershell
.github/skills/vs-local-development/scripts/deploy-to-vs.ps1 "artifacts\VSSetup\Debug\VisualFSharpDebug.vsix"
```

This script:
- Installs the VSIX into the experimental hive (`/rootSuffix:exp`)

**Shell requirement:** `VSIXInstaller.exe` and `devenv.exe` must be on `PATH`. Run from a **Visual Studio Developer PowerShell** / **Developer Command Prompt**, or use the discovery snippet below.

### Finding devenv.exe

VS installation folders use **numeric version identifiers** (e.g., `18`), not marketing years (e.g., `2026`), and may be under `Program Files` or `Program Files (x86)`. Editions vary (`IntPreview`, `Enterprise`, `Preview`, `Community`, etc.).

Use this snippet to find `devenv.exe` dynamically:

```powershell
$devenv = Get-ChildItem "${env:ProgramFiles}\Microsoft Visual Studio","${env:ProgramFiles(x86)}\Microsoft Visual Studio" `
    -Filter "devenv.exe" -Recurse -ErrorAction SilentlyContinue |
    Where-Object { $_.FullName -match 'Common7\\IDE\\devenv\.exe$' } |
    Select-Object -First 1 -ExpandProperty FullName

if (-not $devenv) { Write-Error "devenv.exe not found"; exit 1 }
Write-Host "Using: $devenv"
```

Then source the Developer environment and deploy:

```powershell
$vsDir = Split-Path (Split-Path (Split-Path $devenv))   # …\Common7\IDE → VS root
& "$vsDir\Common7\Tools\VsDevCmd.bat"
.github/skills/vs-local-development/scripts/deploy-to-vs.ps1 "artifacts\VSSetup\Debug\VisualFSharpDebug.vsix"
```

### 4. Reopen VS

The extension is now installed. Open the experimental instance normally:

```powershell
devenv /rootsuffix exp
```

## Quick Reference (Copy-Paste)

Full sequence for a rebuild cycle:

```powershell
# 1. Stop experimental instances
.github/skills/vs-local-development/scripts/close-exp-instances.ps1

# 2. Rebuild
./Build.cmd -c Debug

# 3. Deploy VSIX (install, wait, clear caches)
.github/skills/vs-local-development/scripts/deploy-to-vs.ps1 "artifacts\VSSetup\Debug\VisualFSharpDebug.vsix"

# 4. Reopen VS with the updated extension
devenv /rootsuffix exp
```

## Key Concepts

### Experimental Instance

Visual Studio supports **experimental instances** (`/rootsuffix exp`) — isolated environments with separate extension registrations and settings. This allows developing and testing extensions without affecting your main VS installation.

## Troubleshooting

| Problem | Solution |
|---|---|
| VSIX install fails | Ensure all experimental VS instances are closed (`.github/skills/vs-local-development/scripts/close-exp-instances.ps1`) |
| Extension not appearing in VS | Re-run `deploy-to-vs.ps1` or manually run `devenv /rootsuffix exp /clearcache` and `/updateconfiguration` |
| Old version still loaded | Delete `artifacts/` and rebuild |
| `devenv.exe` not found | VS folders use numeric versions (e.g., `18`), not years. Use the discovery snippet in the Deploy section above |

## When to Use This Skill

Use this skill when:

- Making changes to the VS extension (`VisualFSharpFull` or `VisualFSharpDebug`)
- Needing to test extension changes locally in Visual Studio
- Troubleshooting VSIX deployment to the experimental instance
- Setting up a local VS extension development loop

## Related

- [vs-mcp-testing-tools](../vs-mcp-testing-tools/SKILL.md) — for automating VS via MCP tools after the extension is deployed
