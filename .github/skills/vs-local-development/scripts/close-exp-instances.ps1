# Closes all devenv.exe processes launched with /rootsuffix (experimental instances).
Get-CimInstance Win32_Process -Filter "Name = 'devenv.exe'" |
    Where-Object { $_.CommandLine -match '/rootsuffix\s+\S+' } |
    ForEach-Object {
        Write-Host "Stopping devenv.exe (PID $($_.ProcessId)): $($_.CommandLine)"
        Stop-Process -Id $_.ProcessId -Force
    }
