<#
get-fsharp-errors.ps1 - cross-platform client for the fsharp-diag-server.
Requires pwsh 7+ (AF_UNIX socket support on Windows 10 1803+).
#>

[CmdletBinding(PositionalBinding = $false)]
param(
    [switch]$ParseOnly,
    [switch]$CheckProject,
    [switch]$Ping,
    [switch]$Shutdown,
    [switch]$FindRefs,
    [switch]$TypeHints,
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$Rest
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$ScriptDir         = Split-Path -Parent $PSCommandPath
$ServerProject     = (Resolve-Path (Join-Path $ScriptDir '..' 'server')).Path
$SockDir           = Join-Path $HOME '.fsharp-diag'
$StartTimeoutSec   = 180   # > documented 70s cold start, covers slow nuget restore
$ConnectTimeoutMs  = 5000
$IoTimeoutMs       = 600000  # 10 min for checkProject; safe upper bound

function Show-Usage {
    @"
Usage:
  get-fsharp-errors.ps1 [-ParseOnly] <file.fs>
  get-fsharp-errors.ps1 -FindRefs   <file.fs> <line> <col>
  get-fsharp-errors.ps1 -TypeHints  <file.fs> <startLine> <endLine>
  get-fsharp-errors.ps1 -CheckProject | -Ping | -Shutdown
"@ | Out-Host
}

function Get-RepoRoot {
    # Server normalizes via Path.GetFullPath; client must do the same before hashing.
    $raw = try { (& git rev-parse --show-toplevel 2>$null) } catch { $null }
    if ([string]::IsNullOrWhiteSpace($raw)) { $raw = (Get-Location).Path }
    [System.IO.Path]::GetFullPath($raw.Trim())
}

function Get-Hash16([string]$s) {
    # Mirrors Server.fs deriveHash exactly.
    $bytes = [System.Text.Encoding]::UTF8.GetBytes($s)
    [System.Convert]::ToHexString(
        [System.Security.Cryptography.SHA256]::HashData($bytes)
    ).Substring(0, 16).ToLowerInvariant()
}

function Get-SocketPath([string]$root) { Join-Path $SockDir ((Get-Hash16 $root) + '.sock') }
function Get-LogPath   ([string]$root) { Join-Path $SockDir ((Get-Hash16 $root) + '.log') }
function Get-LockPath  ([string]$root) { Join-Path $SockDir ((Get-Hash16 $root) + '.startup.lock') }

function Resolve-AbsFile([string]$p) {
    # Lexical resolution - missing files reach the server's JSON not-found handler.
    if ([System.IO.Path]::IsPathRooted($p)) {
        [System.IO.Path]::GetFullPath($p)
    } else {
        [System.IO.Path]::GetFullPath((Join-Path (Get-Location).Path $p))
    }
}

function New-DiagSocket {
    New-Object System.Net.Sockets.Socket(
        [System.Net.Sockets.AddressFamily]::Unix,
        [System.Net.Sockets.SocketType]::Stream,
        [System.Net.Sockets.ProtocolType]::Unspecified)
}

function Send-Request([string]$sock, [hashtable]$payload, [int]$timeoutMs = $IoTimeoutMs) {
    $json = ($payload | ConvertTo-Json -Compress -Depth 4) + "`n"
    $bytes = [System.Text.Encoding]::UTF8.GetBytes($json)
    $client = New-DiagSocket
    try {
        $client.SendTimeout    = $timeoutMs
        $client.ReceiveTimeout = $timeoutMs
        $task = $client.ConnectAsync((New-Object System.Net.Sockets.UnixDomainSocketEndPoint($sock)))
        if (-not $task.Wait($ConnectTimeoutMs)) { throw "connect timed out after $ConnectTimeoutMs ms ($sock)" }
        [void]$client.Send($bytes)
        $client.Shutdown([System.Net.Sockets.SocketShutdown]::Send)
        # Stream UTF-8 across recv boundaries so multibyte chars don't fragment.
        $buf      = New-Object byte[] 65536
        $decoder  = [System.Text.Encoding]::UTF8.GetDecoder()
        $chars    = New-Object char[] $buf.Length
        $sb       = [System.Text.StringBuilder]::new()
        while (($n = $client.Receive($buf)) -gt 0) {
            $c = $decoder.GetChars($buf, 0, $n, $chars, 0)
            [void]$sb.Append($chars, 0, $c)
        }
        $sb.ToString()
    } finally { $client.Dispose() }
}

function Test-ServerAlive([string]$sock) {
    if (-not (Test-Path $sock)) { return $false }
    try { (Send-Request $sock @{ command = 'ping' } 2000) -match '"ok"' } catch { $false }
}

function Get-ServerBinaryPath {
    # Ask MSBuild for the configured output path (honors BaseOutputPath etc.). Project settings only - no build required.
    $p = & dotnet msbuild $ServerProject /p:Configuration=Release -getProperty:TargetPath 2>$null
    if ($LASTEXITCODE -eq 0 -and $p) { $p.Trim() } else { $null }
}

function Find-ServerBinary {
    $p = Get-ServerBinaryPath
    if ($p -and (Test-Path $p)) { $p } else { $null }
}

function Build-DiagServer {
    # Visible foreground build so the agent sees nuget restore + compile progress on a cold clone (can be 10+ min).
    Write-Host "[fsharp-diag] Building server (first call after clone can take 10+ min for nuget restore + FSharp.Compiler.Service build)..." -ForegroundColor Yellow
    $build = Start-Process -FilePath 'dotnet' `
        -ArgumentList @('build','-c','Release', $ServerProject) `
        -NoNewWindow -Wait -PassThru
    if ($build.ExitCode -ne 0) {
        throw "Server build failed (dotnet build exit $($build.ExitCode))."
    }
    $dll = Find-ServerBinary
    if (-not $dll) { throw "Build reported success but FSharpDiagServer.dll not found (MSBuild TargetPath: $(Get-ServerBinaryPath))." }
    $dll
}

function Start-DiagServer([string]$root, [string]$sock) {
    if (Test-ServerAlive $sock) { return }
    New-Item -ItemType Directory -Force -Path $SockDir | Out-Null
    $lockPath = Get-LockPath $root
    $lock = $null
    try {
        # Serialize startup so racing clients don't spawn duplicate servers.
        $lock = [System.IO.File]::Open($lockPath, [System.IO.FileMode]::OpenOrCreate,
                                        [System.IO.FileAccess]::Write, [System.IO.FileShare]::None)
        # Re-check after acquiring the lock - peer may have started a server while we waited.
        if (Test-ServerAlive $sock) { return }
        if (Test-Path $sock) { Remove-Item -Force $sock }

        # Ensure server binary exists. Build is foreground + visible so the agent sees progress.
        $dll = Find-ServerBinary
        if (-not $dll) { $dll = Build-DiagServer }

        $log = Get-LogPath $root
        $psi = New-Object System.Diagnostics.ProcessStartInfo
        $psi.FileName = 'dotnet'
        # ArgumentList (Collection<string>) handles per-platform quoting (incl. spaces in paths).
        # Launch via prebuilt dll so startup is bound by server init (~70s), not by build (~minutes).
        foreach ($a in @($dll, '--repo-root', $root)) { [void]$psi.ArgumentList.Add($a) }
        $psi.RedirectStandardOutput = $true
        $psi.RedirectStandardError  = $true
        $psi.UseShellExecute        = $false
        $psi.CreateNoWindow         = $true
        $proc = [System.Diagnostics.Process]::Start($psi)
        # Drain to log file so the child's pipes don't fill and block.
        $proc.StandardOutput.BaseStream.CopyToAsync([System.IO.File]::Create($log)) | Out-Null
        $proc.StandardError.BaseStream.CopyToAsync(
            [System.IO.File]::Create("$log.err")) | Out-Null
        # Poll for a LIVE server (file existence is insufficient - server may be mid-bind).
        $sw = [System.Diagnostics.Stopwatch]::StartNew()
        while ($sw.Elapsed.TotalSeconds -lt $StartTimeoutSec) {
            if (Test-ServerAlive $sock) { return }
            Start-Sleep -Milliseconds 500
        }
        throw "Server failed to start within ${StartTimeoutSec}s. Check log: $log"
    } finally {
        if ($lock) { $lock.Dispose(); Remove-Item -Force $lockPath -ErrorAction SilentlyContinue }
    }
}

function Assert-RequiredArg([int]$needed, [string]$cmd) {
    if (-not $Rest -or $Rest.Count -lt $needed) {
        Write-Error "$cmd requires $needed positional argument(s): see -? for usage." -ErrorAction Continue
        Show-Usage
        exit 1
    }
}

function ConvertTo-Int32Arg([string]$s, [string]$name) {
    $out = 0
    if (-not [int]::TryParse($s, [ref]$out)) {
        Write-Error "$name must be an integer, got '$s'" -ErrorAction Continue
        Show-Usage
        exit 1
    }
    $out
}

# --- main ---

$root = Get-RepoRoot
$sock = Get-SocketPath $root

# Validate args BEFORE spawning a 70s+ server.
$payload =
    if     ($Ping)         { @{ command = 'ping' } }
    elseif ($Shutdown)     { @{ command = 'shutdown' } }
    elseif ($CheckProject) { @{ command = 'checkProject' } }
    elseif ($ParseOnly)    { Assert-RequiredArg 1 '-ParseOnly'; @{ command = 'parseOnly'; file = (Resolve-AbsFile $Rest[0]) } }
    elseif ($FindRefs)     { Assert-RequiredArg 3 '-FindRefs';  @{ command = 'findRefs';  file = (Resolve-AbsFile $Rest[0]); line = (ConvertTo-Int32Arg $Rest[1] 'line'); col = (ConvertTo-Int32Arg $Rest[2] 'col') } }
    elseif ($TypeHints)    { Assert-RequiredArg 3 '-TypeHints'; @{ command = 'typeHints'; file = (Resolve-AbsFile $Rest[0]); startLine = (ConvertTo-Int32Arg $Rest[1] 'startLine'); endLine = (ConvertTo-Int32Arg $Rest[2] 'endLine') } }
    elseif ($Rest -and $Rest.Count -ge 1) { @{ command = 'check'; file = (Resolve-AbsFile $Rest[0]) } }
    else   { Show-Usage; exit 1 }

# Skip server start for -Shutdown (would be pointless) and ensure friendly error if absent.
if (-not $Shutdown) { Start-DiagServer $root $sock }

try {
    Send-Request $sock $payload
} catch {
    if ($Shutdown) {
        Write-Output '{ "status":"not_running" }'
    } else {
        Write-Error "Cannot reach diagnostics server at $sock`: $($_.Exception.Message)"
        exit 1
    }
}
