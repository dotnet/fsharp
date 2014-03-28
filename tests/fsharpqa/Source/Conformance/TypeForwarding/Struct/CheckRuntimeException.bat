is32bitruntime.exe
IF ERRORLEVEL 1 (recomp /p:%1 /ee:%3) ELSE (recomp /p:%1 /ee:%2)
