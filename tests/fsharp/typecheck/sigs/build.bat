@if "%_echo%"=="" echo off

setlocal

REM Configure the sample, i.e. where to find the F# compiler and C# compiler.
call %~d0%~p0..\..\..\config.bat

call ..\..\single-neg-test.bat neg97
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg96
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg95
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg46
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg91
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg94
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --target:exe -o:pos22.exe  pos22.fs 
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" pos22.exe
@if ERRORLEVEL 1 goto Error

pos22.exe
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg93
@if ERRORLEVEL 1 goto Error

"%FSC%" --noframework -r:"%FSCOREDLLPATH%" -r:"%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\mscorlib.dll" -r:"%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Core.dll" -r:"%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Data.dll" -r:"%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.dll" -r:"%X86_PROGRAMFILES%\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Numerics.dll" -a -o:pos21.dll  pos21.fs
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg92
@if ERRORLEVEL 1 goto Error


"%FSC%" %fsc_flags% --target:exe -o:pos20.exe  pos20.fs 
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" pos20.exe
@if ERRORLEVEL 1 goto Error

pos20.exe
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --target:exe -o:pos19.exe  pos19.fs 
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" pos19.exe
@if ERRORLEVEL 1 goto Error

pos19.exe
@if ERRORLEVEL 1 goto Error


"%FSC%" %fsc_flags% --target:exe -o:pos18.exe  pos18.fs 
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" pos18.exe
@if ERRORLEVEL 1 goto Error

pos18.exe
@if ERRORLEVEL 1 goto Error


"%FSC%" %fsc_flags% --target:exe -o:pos16.exe  pos16.fs 
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" pos16.exe
@if ERRORLEVEL 1 goto Error

pos16.exe
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --target:exe -o:pos17.exe  pos17.fs 
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" pos17.exe
@if ERRORLEVEL 1 goto Error

pos17.exe
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --target:exe -o:pos15.exe  pos15.fs 
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" pos15.exe
@if ERRORLEVEL 1 goto Error

pos15.exe
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --target:exe -o:pos14.exe  pos14.fs 
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" pos14.exe
@if ERRORLEVEL 1 goto Error

pos14.exe
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% --target:exe -o:pos13.exe  pos13.fs 
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" pos13.exe
@if ERRORLEVEL 1 goto Error

pos13.exe
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% -a -o:pos12.dll  pos12.fs 
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% -a -o:pos11.dll  pos11.fs 
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% -a -o:pos10.dll  pos10.fs 
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" pos10.dll
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg90
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg89
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg88
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% -a -o:pos09.dll  pos09.fs 
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" pos09.dll
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg87
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg86
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg85
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg84
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg83
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg82
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg81
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg80
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg79
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg78
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg77
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg76
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg75
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg74
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg73
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg72
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg71
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg70
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg69
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg68
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg67
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg66
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg65
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg64
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg61
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg63
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg62
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg20
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg24
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg32
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg37
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg37_a
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg60
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg59
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg58
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg57
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg56
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg56_a
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg56_b
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg55
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg54
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg53
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg52
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg51
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg50
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg49
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg48
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg47
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg10
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg10_a
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg45
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg44
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg43
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg38
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg39
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg40
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg41
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg42
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% -a -o:pos07.dll  pos07.fs 
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" pos07.dll
@if ERRORLEVEL 1 goto Error


"%FSC%" %fsc_flags% -a -o:pos08.dll  pos08.fs 
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" pos08.dll
@if ERRORLEVEL 1 goto Error


"%FSC%" %fsc_flags% -a -o:pos06.dll  pos06.fs 
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" pos06.dll
@if ERRORLEVEL 1 goto Error


"%FSC%" %fsc_flags% -a -o:pos03.dll  pos03.fs 
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" pos03.dll
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% -a -o:pos03a.dll  pos03a.fsi pos03a.fs 
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" pos03a.dll
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg34
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg33
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg30
@if ERRORLEVEL 1 goto Error


call ..\..\single-neg-test.bat neg31
@if ERRORLEVEL 1 goto Error



call ..\..\single-neg-test.bat neg29
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg28
@if ERRORLEVEL 1 goto Error


call ..\..\single-neg-test.bat neg07
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg_byref_20
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg_byref_1
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg_byref_2
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg_byref_3
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg_byref_4
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg_byref_5
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg_byref_6
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg_byref_7
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg_byref_8
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg_byref_10
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg_byref_11
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg_byref_12
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg_byref_13
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg_byref_14
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg_byref_15
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg_byref_16
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg_byref_17
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg_byref_18
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg_byref_19
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg_byref_21
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg_byref_22
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg_byref_23
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg36
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg17
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg26
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg27
@if ERRORLEVEL 1 goto Error


call ..\..\single-neg-test.bat neg25
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg03
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg23
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg22
@if ERRORLEVEL 1 goto Error


call ..\..\single-neg-test.bat neg21
@if ERRORLEVEL 1 goto Error




call ..\..\single-neg-test.bat neg04
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg05
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg06
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg06_a
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg06_b
@if ERRORLEVEL 1 goto Error


  call ..\..\single-neg-test.bat neg08
  @if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg09
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg11
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg12
@if ERRORLEVEL 1 goto Error


call ..\..\single-neg-test.bat neg13
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg14
@if ERRORLEVEL 1 goto Error


call ..\..\single-neg-test.bat neg16
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg18
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg19
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg01
@if ERRORLEVEL 1 goto Error

call ..\..\single-neg-test.bat neg02
@if ERRORLEVEL 1 goto Error



call ..\..\single-neg-test.bat neg15
@if ERRORLEVEL 1 goto Error

echo Some random positive cases found while developing the negative tests
"%FSC%" %fsc_flags% -a -o:pos01a.dll  pos01a.fsi pos01a.fs 
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" pos01a.dll
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% -a -o:pos02.dll  pos02.fs 
@if ERRORLEVEL 1 goto Error

"%PEVERIFY%" pos02.dll
@if ERRORLEVEL 1 goto Error
call ..\..\single-neg-test.bat neg35
@if ERRORLEVEL 1 goto Error

"%FSC%" %fsc_flags% -a -o:pos05.dll  pos05.fs
@if ERRORLEVEL 1 goto Error

REM --------Exit points------------------------

:Ok
echo Built fsharp %~f0 ok.
endlocal
exit /b 0

:Skip
echo Skipped %~f0
endlocal
exit /b 0


:Error
endlocal
exit /b %ERRORLEVEL%

