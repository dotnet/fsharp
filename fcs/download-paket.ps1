$paketurl="https://github.com/fsprojects/Paket/releases/download/5.201.1/paket.exe"
$paketdir = Join-Path $PSScriptRoot ".paket"
$paketpath = Join-Path $paketdir "paket.exe"
if (-not (Test-Path "$paketpath")) {
    mkdir "$paketdir"
    Invoke-WebRequest -Uri $paketurl -OutFile "$paketpath"
}
