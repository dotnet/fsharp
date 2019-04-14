$paketurl="https://github.com/fsprojects/Paket/releases/download/5.201.1/paket.exe"
$paketdir = Join-Path $PSScriptRoot ".paket"
$paketpath = Join-Path $paketdir "paket.exe"

# Enable TLS 1.2 and TLS 1.1 as Security Protocols
[Net.ServicePointManager]::SecurityProtocol = `
    [Net.SecurityProtocolType]::Tls12,
    [Net.SecurityProtocolType]::Tls11;
    
if (-not (Test-Path "$paketpath")) {
    mkdir "$paketdir"
    Invoke-WebRequest -Uri $paketurl -OutFile "$paketpath"
}
