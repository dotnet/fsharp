REM ==
REM == Latest sources \\clrdrop\drops\CLRv4\PUCLR\sources\<build>\QA\CLR\Testsrc\Desktop\tools\PartialTrustRunner
REM ==
csc /out:PTRunner.exe PartialTrustRunner.cs ptrunnerlib.cs /keyfile:ptkey.snk
