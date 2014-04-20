
set GACUTIL=%ADMIN_PIPE% "%GACUTILEXE32%"

goto :Label%2 %1

:LabelYYY
set FSC=%~1

"%FSC%" -g -a --keyfile:keyfile.snk                                           MyTPDesignTime.fs
"%FSC%" -g -a --keyfile:keyfile.snk --define:WITHDLLEXT -r:MyTPDesignTime.dll MyTPRuntime.fs
%GACUTIL% /if MyTPDesignTime.dll
"%FSC%" -g -a --define:VER1111 --keyfile:keyfile2.snk                         MyTPDesignTime.fs
goto :EOF

:LabelYYN
set FSC=%~1

"%FSC%" -g -a --keyfile:keyfile.snk                                           MyTPDesignTime.fs
"%FSC%" -g -a --keyfile:keyfile.snk --define:WITHDLLEXT -r:MyTPDesignTime.dll MyTPRuntime.fs
%GACUTIL% /if MyTPDesignTime.dll
del MyTPDesignTime.dll
goto :EOF

:LabelYNY
set FSC=%~1

"%FSC%" -g -a --keyfile:keyfile.snk                                           MyTPDesignTime.fs
"%FSC%" -g -a --keyfile:keyfile.snk --define:WITHDLLEXT -r:MyTPDesignTime.dll MyTPRuntime.fs
%GACUTIL% /uf MyTPDesignTime
goto :EOF


:LabelYNN
set FSC=%~1

"%FSC%" -g -a --keyfile:keyfile.snk                                           MyTPDesignTime.fs
"%FSC%" -g -a --keyfile:keyfile.snk --define:WITHDLLEXT -r:MyTPDesignTime.dll MyTPRuntime.fs
%GACUTIL% /uf MyTPDesignTime
del MyTPDesignTime.dll
goto :EOF


:LabelNFYY
set FSC=%~1

"%FSC%" -g -a --keyfile:keyfile.snk                                           MyTPDesignTime.fs
"%FSC%" -g -a --keyfile:keyfile.snk --define:FULLNAME -r:MyTPDesignTime.dll MyTPRuntime.fs
%GACUTIL% /if MyTPDesignTime.dll
goto :EOF

:LabelNFYN
set FSC=%~1

"%FSC%" -g -a --keyfile:keyfile.snk                                           MyTPDesignTime.fs
"%FSC%" -g -a --keyfile:keyfile.snk --define:FULLNAME -r:MyTPDesignTime.dll MyTPRuntime.fs
%GACUTIL% /if MyTPDesignTime.dll
del MyTPDesignTime.dll
goto :EOF


:LabelNFNY
set FSC=%~1

"%FSC%" -g -a --keyfile:keyfile.snk                                           MyTPDesignTime.fs
"%FSC%" -g -a --keyfile:keyfile.snk --define:FULLNAME -r:MyTPDesignTime.dll MyTPRuntime.fs
%GACUTIL% /uf MyTPDesignTime
goto :EOF

:LabelNFNN
set FSC=%~1

"%FSC%" -g -a --keyfile:keyfile.snk                                           MyTPDesignTime.fs
"%FSC%" -g -a --keyfile:keyfile.snk --define:FULLNAME -r:MyTPDesignTime.dll MyTPRuntime.fs
%GACUTIL% /uf MyTPDesignTime
del MyTPDesignTime.dll
goto :EOF


:LabelNYY
set FSC=%~1

"%FSC%" -g -a --keyfile:keyfile.snk                                           MyTPDesignTime.fs
"%FSC%" -g -a --keyfile:keyfile.snk --define:NODLLEXT -r:MyTPDesignTime.dll MyTPRuntime.fs
%GACUTIL% /if MyTPDesignTime.dll
"%FSC%" -g -a --define:VER1111 --keyfile:keyfile2.snk                         MyTPDesignTime.fs
goto :EOF

REM == bug 381797
:LabelNYN
set FSC=%~1

"%FSC%" -g -a --keyfile:keyfile.snk                                           MyTPDesignTime.fs
"%FSC%" -g -a --keyfile:keyfile.snk --define:NODLLEXT -r:MyTPDesignTime.dll MyTPRuntime.fs
%GACUTIL% /if MyTPDesignTime.dll
del MyTPDesignTime.dll
goto :EOF

:LabelNNY
set FSC=%~1

"%FSC%" -g -a --keyfile:keyfile.snk                                           MyTPDesignTime.fs
"%FSC%" -g -a --keyfile:keyfile.snk --define:NODLLEXT -r:MyTPDesignTime.dll MyTPRuntime.fs
%GACUTIL% /uf MyTPDesignTime
goto :EOF

:LabelNNN
set FSC=%~1

"%FSC%" -g -a --keyfile:keyfile.snk                                           MyTPDesignTime.fs
"%FSC%" -g -a --keyfile:keyfile.snk --define:NODLLEXT -r:MyTPDesignTime.dll MyTPRuntime.fs
%GACUTIL% /uf MyTPDesignTime
del MyTPDesignTime.dll
goto :EOF

