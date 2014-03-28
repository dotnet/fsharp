set GACUTIL=%ADMIN_PIPE% "%GACUTILEXE32%"

goto :Label%1

:LabelYYY
:LabelYYN
:LabelNFYY
:LabelNFYN
:LabelNYY
:LabelNYN
%GACUTIL% /uf MyTPDesignTime
goto :EOF
