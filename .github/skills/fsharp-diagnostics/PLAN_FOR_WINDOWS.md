# Windows Support Plan for FastBuildFromCache

## Strategy

Use **Unix Domain Sockets (UDS) on all platforms**, including Windows.  
Windows 10 1803+ supports `AF_UNIX`. .NET's `UnixDomainSocketEndPoint` works cross-platform since .NET Core 3.0+.  
This keeps the server transport code unchanged — the work is a new PowerShell client script and minor path fixes.

**Why not Named Pipes?** Would require a transport abstraction in the server (different accept-loop lifecycle for `NamedPipeServerStream` vs `Socket`), doubling transport code for no benefit when UDS works.

**Why not a .NET client tool?** Chicken-and-egg: the client must exist before FCS builds, but would itself need building first. Also adds ~150ms JIT startup per invocation.

**Prerequisite:** `pwsh` (PowerShell 7+). Windows PowerShell 5.1 lacks `UnixDomainSocketEndPoint`. The MSBuild targets use `ContinueOnError="true"`, so missing `pwsh` gracefully falls back to normal `fsc`.

---

## Changes

### 1. `eng/targets/FastBuildFromCache.targets`

Replace hardcoded `bash` with OS-conditional properties and a single `<Exec>`:

```xml
<_FastBuildScript Condition="'$(OS)'!='Windows_NT'">...get-fsharp-errors.sh</_FastBuildScript>
<_FastBuildScript Condition="'$(OS)'=='Windows_NT'">...get-fsharp-errors.ps1</_FastBuildScript>
<_FastBuildInterpreter Condition="'$(OS)'!='Windows_NT'">bash</_FastBuildInterpreter>
<_FastBuildInterpreter Condition="'$(OS)'=='Windows_NT'">pwsh -NoProfile -File</_FastBuildInterpreter>
```

Single Exec: `Command="$(_FastBuildInterpreter) &quot;$(_FastBuildScript)&quot; ..."`

### 2. NEW: `scripts/get-fsharp-errors.ps1`

PowerShell Core port of `get-fsharp-errors.sh` (~100-120 lines). Key translations:

| Bash | PowerShell |
|------|------------|
| `shasum -a 256` | `[System.Security.Cryptography.SHA256]::HashData()` |
| `nc -U "$sock"` | `[System.Net.Sockets.Socket]` + `[UnixDomainSocketEndPoint]` + `NetworkStream` + `StreamReader/Writer` |
| `nohup dotnet run ... &` | `Start-Process dotnet -ArgumentList ... -NoNewWindow` |
| `[ -S "$sock" ]` | `Test-Path $sock` |
| `set -euo pipefail` | `$ErrorActionPreference = 'Stop'; Set-StrictMode -Version Latest` |
| `$HOME/.fsharp-diag` | `Join-Path $env:USERPROFILE '.fsharp-diag'` |

Same JSON protocol, same command-line interface (`--compile`, `--parse-only`, etc.).

### 3. `server/Server.fs`

Two one-line fixes:

- **`File.SetUnixFileMode`** (throws `PlatformNotSupportedException` on Windows):
  ```fsharp
  if not (OperatingSystem.IsWindows()) then File.SetUnixFileMode(socketPath, ...)
  ```

- **`TrimEnd('/')`** (doesn't strip `\` on Windows paths):
  ```fsharp
  config.RepoRoot.TrimEnd('/', '\\') + "/"
  ```

### 4. `server/ProjectRouting.fs`

- `TrimEnd('/')` → `TrimEnd('/', '\\')`
- `StringComparison.Ordinal` → `StringComparison.OrdinalIgnoreCase` for path prefix checks
- Normalize relative path: `.Replace('\\', '/')` before pattern matching against `"tests/"`, `"src/"` etc.

### 5. `server/DiagnosticsFormatter.fs`

- `TrimEnd('/')` → `TrimEnd('/', '\\')`
- `StringComparison.OrdinalIgnoreCase` for `path.StartsWith(root)`

---

## Critical: Path Normalization Before Hashing

The socket path is derived from `SHA256(repoRoot)`. Client and server **must hash the exact same string** or they'll look for different sockets.

Problem: `git rev-parse --show-toplevel` returns `C:/Users/foo/fsharp` on Windows (forward slashes), but .NET's `Directory.GetCurrentDirectory()` returns `C:\Users\foo\fsharp` (backslashes).

**Rule:** Before hashing, normalize to: forward slashes, no trailing separator.  
Apply this in both the PS1 script and `deriveSocketPath` in Server.fs.

---

## What Does NOT Need Changing

- **Socket transport in Server.fs** — `Socket(AddressFamily.Unix)` + `UnixDomainSocketEndPoint` works on Windows
- **FileSystemWatcher** — cross-platform in .NET
- **FSharpDiagServer.fsproj** — `net10.0` SDK project, fully cross-platform
- **Program.fs** — no OS-specific code
- **All product code** (`service.fs`, `FSharpCheckerResults.fs`, `CompilerImports.fs`, `fsc.fs`) — already cross-platform

## Testing Checklist

- [ ] `pwsh` can connect to server via UDS on Windows
- [ ] Server spawns correctly via `Start-Process` from PS1
- [ ] Socket path matches between PS1 client and server (hash normalization)
- [ ] `dotnet test ... /p:FastBuildFromCache=true` works end-to-end on Windows
- [ ] Graceful fallback when `pwsh` is not installed
- [ ] No-change build is a no-op (MSBuild incremental skip works on Windows)
