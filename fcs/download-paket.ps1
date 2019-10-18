$paketurl="https://github.com/fsprojects/Paket/releases/download/5.215.0/paket.exe"
$paketdir = Join-Path $PSScriptRoot ".paket"
$paketpath = Join-Path $paketdir "paket.exe"

# Enable TLS 1.2 and TLS 1.1 as Security Protocols
[Net.ServicePointManager]::SecurityProtocol = `
    [Net.SecurityProtocolType]::Tls12,
    [Net.SecurityProtocolType]::Tls11;

if (-not (Test-Path "$paketpath")) {
    if (-not (Test-Path "$paketdir")) {
        mkdir "$paketdir"
    }
    Invoke-WebRequest -Uri $paketurl -OutFile "$paketpath"
}
