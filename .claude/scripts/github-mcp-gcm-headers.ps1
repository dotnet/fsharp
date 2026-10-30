$ErrorActionPreference = 'Stop'
$env:GCM_INTERACTIVE = 'never'
$env:GIT_TERMINAL_PROMPT = '0'

$token = $null
try {
    $credential = "protocol=https`nhost=github.com`n`n" | git credential fill 2>$null
    $token = ($credential | Where-Object { $_ -like 'password=*' }) -replace '^password=', ''
}
catch {
}

if ($token) {
    @{ Authorization = "Bearer $token" } | ConvertTo-Json -Compress
}
else {
    '{}'
}
