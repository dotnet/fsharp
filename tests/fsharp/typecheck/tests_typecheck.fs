module ``FSharp-Tests-Typecheck``

open System
open System.IO
open NUnit.Framework

open FSharpTestSuiteTypes
open NUnitConf
open PlatformHelpers

let testContext = FSharpTestSuite.testContext


module ``Full-rank-arrays`` = 

    [<Test; FSharpSuiteScriptPermutations("typecheck/full-rank-arrays")>]
    let ``full-rank-arrays`` p = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let csc = Printf.ksprintf (Commands.csc exec cfg.CSC)
        
        // %CSC% /target:library /out:HighRankArrayTests.dll .\Class1.cs
        do! csc "/target:library /out:HighRankArrayTests.dll" ["Class1.cs"]

        do! SingleTestBuild.singleTestBuild cfg dir p
        
        do! SingleTestRun.singleTestRun cfg dir p
        })


module Misc = 

    [<Test; FSharpSuiteScriptPermutations("typecheck/misc")>]
    let misc p = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()
        
        do! SingleTestBuild.singleTestBuild cfg dir p
        
        do! SingleTestRun.singleTestRun cfg dir p
        })


module Sigs = 

    [<Test; FSharpSuiteTest("typecheck/sigs")>]
    let sigs () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)
        let peverify = Commands.peverify exec cfg.PEVERIFY "/nologo"
        let fsc_flags = cfg.fsc_flags

        let singleNegTest = SingleNegTest.singleNegTest cfg dir


        // "%FSC%" %fsc_flags% --target:exe -o:pos23.exe  pos23.fs 
        do! fsc "%s --target:exe -o:pos23.exe" fsc_flags ["pos23.fs"]
        // "%PEVERIFY%" pos23.exe
        do! peverify "pos23.exe"
        // pos23.exe
        do! exec ("."/"pos23.exe") ""

        // "%FSC%" %fsc_flags% --target:exe -o:pos20.exe  pos20.fs 
        do! fsc "%s --target:exe -o:pos20.exe" fsc_flags ["pos20.fs"]
        // "%PEVERIFY%" pos20.exe
        do! peverify "pos20.exe"
        // pos20.exe
        do! exec ("."/"pos20.exe") ""

        // "%FSC%" %fsc_flags% --target:exe -o:pos19.exe  pos19.fs 
        do! fsc "%s --target:exe -o:pos19.exe" fsc_flags ["pos19.fs"]
        // "%PEVERIFY%" pos19.exe
        do! peverify "pos19.exe"
        // pos19.exe
        do! exec ("."/"pos19.exe") ""

        // "%FSC%" %fsc_flags% --target:exe -o:pos18.exe  pos18.fs 
        do! fsc "%s --target:exe -o:pos18.exe" fsc_flags ["pos18.fs"]
        // "%PEVERIFY%" pos18.exe
        do! peverify "pos18.exe"
        // pos18.exe
        do! exec ("."/"pos18.exe") ""

        // "%FSC%" %fsc_flags% --target:exe -o:pos16.exe  pos16.fs 
        do! fsc "%s --target:exe -o:pos16.exe" fsc_flags ["pos16.fs"]
        // "%PEVERIFY%" pos16.exe
        do! peverify "pos16.exe"
        // pos16.exe
        do! exec ("."/"pos16.exe") ""

        // "%FSC%" %fsc_flags% --target:exe -o:pos17.exe  pos17.fs 
        do! fsc "%s --target:exe -o:pos17.exe" fsc_flags ["pos17.fs"]
        // "%PEVERIFY%" pos17.exe
        do! peverify "pos17.exe"
        // pos17.exe
        do! exec ("."/"pos17.exe") ""

        // "%FSC%" %fsc_flags% --target:exe -o:pos15.exe  pos15.fs 
        do! fsc "%s --target:exe -o:pos15.exe" fsc_flags ["pos15.fs"]
        // "%PEVERIFY%" pos15.exe
        do! peverify "pos15.exe"
        // pos15.exe
        do! exec ("."/"pos15.exe") ""

        // "%FSC%" %fsc_flags% --target:exe -o:pos14.exe  pos14.fs 
        do! fsc "%s --target:exe -o:pos14.exe" fsc_flags ["pos14.fs"]
        // "%PEVERIFY%" pos14.exe
        do! peverify "pos14.exe"
        // pos14.exe
        do! exec ("."/"pos14.exe") ""

        // "%FSC%" %fsc_flags% --target:exe -o:pos13.exe  pos13.fs
        do! fsc "%s --target:exe -o:pos13.exe" fsc_flags ["pos13.fs"]
        // "%PEVERIFY%" pos13.exe
        do! peverify "pos13.exe"
        // pos13.exe
        do! exec ("."/"pos13.exe") ""

        // "%FSC%" %fsc_flags% -a -o:pos12.dll  pos12.fs 
        do! fsc "%s -a -o:pos12.dll" fsc_flags ["pos12.fs"]

        // "%FSC%" %fsc_flags% -a -o:pos11.dll  pos11.fs 
        do! fsc "%s -a -o:pos11.dll" fsc_flags ["pos11.fs"]

        // "%FSC%" %fsc_flags% -a -o:pos10.dll  pos10.fs
        do! fsc "%s -a -o:pos10.dll" fsc_flags ["pos10.fs"]

        // "%PEVERIFY%" pos10.dll
        do! peverify "pos10.dll"

        // "%FSC%" %fsc_flags% -a -o:pos09.dll  pos09.fs
        do! fsc "%s -a -o:pos09.dll" fsc_flags ["pos09.fs"]

        // "%PEVERIFY%" pos09.dll
        do! peverify "pos09.dll"

        do! attempt.For (["neg95"; "neg94"; "neg93"; "neg92"; "neg91"; 
                          "neg90"; "neg89"; "neg88";
                          "neg87"; "neg86"; "neg85"; "neg84"; "neg83"; "neg82"; "neg81"; "neg80"; "neg79"; "neg78"; "neg77"; "neg76"; "neg75"; 
                          "neg74"; "neg73"; "neg72"; "neg71"; "neg70"; "neg69"; "neg68"; "neg67"; "neg66"; "neg65"; "neg64"; "neg61"; "neg63"; 
                          "neg62"; "neg20"; "neg24"; "neg32"; "neg37"; "neg37_a"; "neg60"; "neg59"; "neg58"; "neg57"; "neg56"; "neg56_a"; "neg56_b"; 
                          "neg55"; "neg54"; "neg53"; "neg52"; "neg51"; "neg50"; "neg49"; "neg48"; "neg47"; "neg46"; "neg10"; "neg10_a"; "neg45"; 
                          "neg44"; "neg43"; "neg38"; "neg39"; "neg40"; "neg41"; "neg42"], singleNegTest)

        // "%FSC%" %fsc_flags% -a -o:pos07.dll  pos07.fs 
        do! fsc "%s -a -o:pos07.dll" fsc_flags ["pos07.fs"]

        // "%PEVERIFY%" pos07.dll
        do! peverify "pos07.dll"

        // "%FSC%" %fsc_flags% -a -o:pos08.dll  pos08.fs 
        do! fsc "%s -a -o:pos08.dll" fsc_flags ["pos08.fs"]

        // "%PEVERIFY%" pos08.dll
        do! peverify "pos08.dll"

        // "%FSC%" %fsc_flags% -a -o:pos06.dll  pos06.fs 
        do! fsc "%s -a -o:pos06.dll" fsc_flags ["pos06.fs"]

        // "%PEVERIFY%" pos06.dll
        do! peverify "pos06.dll"


        // "%FSC%" %fsc_flags% -a -o:pos03.dll  pos03.fs 
        do! fsc "%s -a -o:pos03.dll" fsc_flags ["pos03.fs"]

        // "%PEVERIFY%" pos03.dll
        do! peverify "pos03.dll"

        // "%FSC%" %fsc_flags% -a -o:pos03a.dll  pos03a.fsi pos03a.fs 
        do! fsc "%s -a -o:pos03a.dll" fsc_flags ["pos03a.fsi"; "pos03a.fs"]

        // "%PEVERIFY%" pos03a.dll
        do! peverify "pos03a.dll"

        do! attempt.For(["neg34"; "neg33"; "neg30"; "neg31"; "neg29"; "neg28"; "neg07"; "neg_byref_20"; 
                         "neg_byref_1"; "neg_byref_2"; "neg_byref_3"; "neg_byref_4"; "neg_byref_5"; "neg_byref_6"; "neg_byref_7"; "neg_byref_8"; 
                         "neg_byref_10"; "neg_byref_11"; "neg_byref_12"; "neg_byref_13"; "neg_byref_14"; "neg_byref_15"; "neg_byref_16"; 
                         "neg_byref_17"; "neg_byref_18"; "neg_byref_19"; "neg_byref_21"; "neg_byref_22"; "neg_byref_23"; "neg36"; "neg17"; "neg26"; 
                         "neg27"; "neg25"; "neg03"; "neg23"; "neg22"; "neg21"; "neg04"; "neg05"; "neg06"; "neg06_a"; "neg06_b"; "neg08"; "neg09"; 
                         "neg11"; "neg12"; "neg13"; "neg14"; "neg16"; "neg18"; "neg19"; "neg01"; "neg02"; "neg15" ], singleNegTest)

        // echo Some random positive cases found while developing the negative tests
        // "%FSC%" %fsc_flags% -a -o:pos01a.dll  pos01a.fsi pos01a.fs
        do! fsc "%s -a -o:pos01a.dll" fsc_flags ["pos01a.fsi"; "pos01a.fs"]

        // "%PEVERIFY%" pos01a.dll
        do! peverify "pos01a.dll"

        // "%FSC%" %fsc_flags% -a -o:pos02.dll  pos02.fs
        do! fsc "%s -a -o:pos02.dll" fsc_flags ["pos02.fs"]

        // "%PEVERIFY%" pos02.dll
        do! peverify "pos02.dll"

        // call ..\..\single-neg-test.bat neg35
        do! singleNegTest "neg35"

        // "%FSC%" %fsc_flags% -a -o:pos05.dll  pos05.fs
        do! fsc "%s -a -o:pos05.dll" fsc_flags ["pos05.fs"]

        })
