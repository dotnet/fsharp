module ``FSharp-Tests-Core``

open System
open System.IO
open NUnit.Framework

open NUnitConf
open PlatformHelpers
open FSharpTestSuiteTypes
open FSharpTestSuiteAsserts

let testContext = FSharpTestSuite.testContext

let singleTestBuildAndRun p = check (attempt {
    let { Directory = dir; Config = cfg } = testContext ()
        
    do! SingleTestBuild.singleTestBuild cfg dir p
        
    do! SingleTestRun.singleTestRun cfg dir p
    })


module Access =
    [<Test; FSharpSuiteScriptPermutations("core/access")>]
    let access p = singleTestBuildAndRun p

module Apporder = 
    [<Test; FSharpSuiteScriptPermutations("core/apporder")>]
    let apporder p = singleTestBuildAndRun p

module Array = 
    [<Test; FSharpSuiteScriptPermutations("core/array")>]
    let array p = singleTestBuildAndRun p

module Attributes = 
    [<Test; FSharpSuiteScriptPermutations("core/attributes")>]
    let attributes p = singleTestBuildAndRun p

module Comprehensions = 
    [<Test; FSharpSuiteScriptPermutations("core/comprehensions")>]
    let comprehensions p = singleTestBuildAndRun p

module ComprehensionsHw = 
    [<Test; FSharpSuiteScriptPermutations("core/comprehensions-hw")>]
    let comprehensions p = singleTestBuildAndRun p

module Control = 
    [<Test; FSharpSuiteFscCodePermutation("core/control")>]
    let control p = singleTestBuildAndRun p

    [<Test; FSharpSuiteFscCodePermutation("core/control")>]
    let ``control --tailcalls`` p = check  (attempt {
        let { Directory = dir; Config = cfg } = testContext ()
        
        do! SingleTestBuild.singleTestBuild cfg dir p
        
        do! SingleTestRun.singleTestRun {cfg with fsi_flags = " --tailcalls" } dir p
        })

module ControlChamenos =
    [<Test; FSharpSuiteFscCodePermutation("core/controlChamenos")>]
    let controlChamenos p = check  (attempt {
        let { Directory = dir; Config = cfg } = testContext ()
        
        do! SingleTestBuild.singleTestBuild cfg dir p
        
        do! SingleTestRun.singleTestRun { cfg with fsi_flags = " --tailcalls" }  dir p
        })

module ControlMailbox =
    [<Test; FSharpSuiteFscCodePermutation("core/controlMailbox")>]
    let controlMailbox p = singleTestBuildAndRun p

    [<Test; FSharpSuiteFscCodePermutation("core/controlMailbox")>]
    let ``controlMailbox --tailcalls`` p = check  (attempt {
        let { Directory = dir; Config = cfg } = testContext ()
        
        do! SingleTestBuild.singleTestBuild cfg dir p
        
        do! SingleTestRun.singleTestRun { cfg with fsi_flags = " --tailcalls" }  dir p
        })

module ControlWpf = 
    [<Test; FSharpSuiteFscCodePermutation("core/controlwpf")>]
    let controlWpf p = singleTestBuildAndRun p

module Csext = 
    [<Test; FSharpSuiteScriptPermutations("core/csext")>]
    let csext p = singleTestBuildAndRun p

module Events = 

    let build cfg dir = attempt {
        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None} p >> checkResult
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)
        let peverify = Commands.peverify exec cfg.PEVERIFY "/nologo"
        let csc = Printf.ksprintf (Commands.csc exec cfg.CSC)

        // "%FSC%" %fsc_flags% -a -o:test.dll -g test.fs
        do! fsc "%s -a -o:test.dll -g" cfg.fsc_flags ["test.fs"]

        // "%PEVERIFY%" test.dll
        do! peverify "test.dll"

        // %CSC% /r:"%FSCOREDLLPATH%" /reference:test.dll /debug+ testcs.cs
        do! csc """/r:"%s" /reference:test.dll /debug+""" cfg.FSCOREDLLPATH ["testcs.cs"]

        // "%PEVERIFY%" testcs.exe
        do! peverify "testcs.exe"
        }

    let run cfg dir = attempt {
        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None} p >> checkResult
        let fsi = Printf.ksprintf (Commands.fsi exec cfg.FSI)
        let fileguard = (Commands.getfullpath dir) >> FileGuard.create

        use testOkFile = fileguard "test.ok"

        // %CLIX% "%FSI%" test.fs && (
        do! fsi "" ["test.fs"]

        // dir test.ok > NUL 2>&1 ) || (
        // @echo :FSI failed;
        // goto Error
        // set ERRORMSG=%ERRORMSG% FSI failed;
        // )
        do! testOkFile |> NUnitConf.checkGuardExists

        // %CLIX% .\testcs.exe
        do! exec ("."/"testcs.exe") ""
        }

    [<Test; FSharpSuiteTest("core/events")>]
    let events () = check  (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        do! build cfg dir
        
        do! run cfg dir
        })


module ``FSI-Shadowcopy`` = 

    [<Test>]
    // "%FSI%" %fsi_flags%                          < test1.fsx
    [<FSharpSuiteTestCase("core/fsi-shadowcopy", "")>]
    // "%FSI%" %fsi_flags%  --shadowcopyreferences- < test1.fsx
    [<FSharpSuiteTestCase("core/fsi-shadowcopy", "--shadowcopyreferences-")>]
    let ``shadowcopy disabled`` (flags: string) = check  (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        let ``exec <`` l p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = Some(RedirectInput(l)) } p >> checkResult
        let ``fsi <`` = Printf.ksprintf (fun flags l -> Commands.fsi (``exec <`` l) cfg.FSI flags [])
        let fileguard = (Commands.getfullpath dir) >> FileGuard.create

        // if exist test1.ok (del /f /q test1.ok)
        use testOkFile = fileguard "test1.ok"

        do! ``fsi <`` "%s %s" cfg.fsi_flags flags "test1.fsx"

        // if NOT EXIST test1.ok goto SetError
        do! testOkFile |> NUnitConf.checkGuardExists
        })

    [<Test>]
    // "%FSI%" %fsi_flags%  /shadowcopyreferences+  < test2.fsx
    [<FSharpSuiteTestCase("core/fsi-shadowcopy", "/shadowcopyreferences+")>]
    // "%FSI%" %fsi_flags%  --shadowcopyreferences  < test2.fsx
    [<FSharpSuiteTestCase("core/fsi-shadowcopy", "--shadowcopyreferences")>]
    let ``shadowcopy enabled`` (flags: string) = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        let ``exec <`` l p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = Some(RedirectInput(l)) } p >> checkResult
        let ``fsi <`` = Printf.ksprintf (fun flags l -> Commands.fsi (``exec <`` l) cfg.FSI flags [])
        let fileguard = (Commands.getfullpath dir) >> FileGuard.create

        // if exist test2.ok (del /f /q test2.ok)
        use testOkFile = fileguard "test2.ok"

        // "%FSI%" %fsi_flags%  /shadowcopyreferences+  < test2.fsx
        do! ``fsi <`` "%s %s" cfg.fsi_flags flags "test2.fsx"

        // if NOT EXIST test2.ok goto SetError
        do! testOkFile |> NUnitConf.checkGuardExists
        })

    

module Forwarders = 

    [<Test; FSharpSuiteTest("core/forwarders")>]
    let forwarders () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)
        let peverify = Commands.peverify exec cfg.PEVERIFY "/nologo"
        let csc = Printf.ksprintf (Commands.csc exec cfg.CSC)
        let copy_y f = Commands.copy_y dir f >> checkResult
        let mkdir = Commands.mkdir_p dir

        // mkdir orig
        mkdir "orig"
        // mkdir split
        mkdir "split"

        // %CSC% /nologo  /target:library /out:orig\a.dll /define:PART1;PART2 a.cs
        do! csc """/nologo  /target:library /out:orig\a.dll /define:PART1;PART2""" ["a.cs"]

        // %CSC% /nologo  /target:library /out:orig\b.dll /r:orig\a.dll b.cs 
        do! csc """/nologo  /target:library /out:orig\b.dll /r:orig\a.dll""" ["b.cs"]

        // "%FSC%" -a -o:orig\c.dll -r:orig\b.dll -r:orig\a.dll c.fs
        do! fsc """-a -o:orig\c.dll -r:orig\b.dll -r:orig\a.dll""" ["c.fs"]

        // %CSC% /nologo  /target:library /out:split\a-part1.dll /define:PART1;SPLIT a.cs  
        do! csc """/nologo  /target:library /out:split\a-part1.dll /define:PART1;SPLIT""" ["a.cs"]

        // %CSC% /nologo  /target:library /r:split\a-part1.dll /out:split\a.dll /define:PART2;SPLIT a.cs
        do! csc """/nologo  /target:library /r:split\a-part1.dll /out:split\a.dll /define:PART2;SPLIT""" ["a.cs"]

        // copy /y orig\b.dll split\b.dll
        do! copy_y ("orig"/"b.dll") ("split"/"b.dll")
        // copy /y orig\c.dll split\c.dll
        do! copy_y ("orig"/"c.dll") ("split"/"c.dll")

        // "%FSC%" -o:orig\test.exe -r:orig\b.dll -r:orig\a.dll test.fs
        do! fsc """-o:orig\test.exe -r:orig\b.dll -r:orig\a.dll""" ["test.fs"]

        // "%FSC%" -o:split\test.exe -r:split\b.dll -r:split\a-part1.dll -r:split\a.dll test.fs
        do! fsc """-o:split\test.exe -r:split\b.dll -r:split\a-part1.dll -r:split\a.dll""" ["test.fs"]

        // "%FSC%" -o:split\test-against-c.exe -r:split\c.dll -r:split\a-part1.dll -r:split\a.dll test.fs
        do! fsc """-o:split\test-against-c.exe -r:split\c.dll -r:split\a-part1.dll -r:split\a.dll""" ["test.fs"]

        // pushd split
        // "%PEVERIFY%" a-part1.dll
        do! peverify ("split"/"a-part1.dll")

        // REM "%PEVERIFY%" a.dll
        // REM   @if ERRORLEVEL 1 goto Error

        // "%PEVERIFY%" b.dll
        do! peverify ("split"/"b.dll")

        // "%PEVERIFY%" c.dll
        do! peverify ("split"/"c.dll")

        // "%PEVERIFY%" test.exe
        do! peverify ("split"/"test.exe")

        // "%PEVERIFY%" test-against-c.exe
        do! peverify ("split"/"test-against-c.exe")

        // popd

        })

module FsFromCs = 

    let build cfg dir = attempt {
        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)
        let peverify = Commands.peverify exec cfg.PEVERIFY "/nologo"
        let csc = Printf.ksprintf (Commands.csc exec cfg.CSC)
        let fsc_flags = cfg.fsc_flags

        // "%FSC%" %fsc_flags% -a --doc:lib.xml -o:lib.dll -g lib.fs
        do! fsc "%s -a --doc:lib.xml -o:lib.dll -g" fsc_flags ["lib.fs"]

        // "%PEVERIFY%" lib.dll
        do! peverify "lib.dll"

        // %CSC% /nologo /r:"%FSCOREDLLPATH%" /r:System.Core.dll /r:lib.dll /out:test.exe test.cs 
        do! csc """/nologo /r:"%s" /r:System.Core.dll /r:lib.dll /out:test.exe""" cfg.FSCOREDLLPATH ["test.cs"]

        // "%FSC%" %fsc_flags% -a --doc:lib--optimize.xml -o:lib--optimize.dll -g lib.fs
        do! fsc """%s -a --doc:lib--optimize.xml -o:lib--optimize.dll -g""" fsc_flags ["lib.fs"]

        // "%PEVERIFY%" lib--optimize.dll
        do! peverify "lib--optimize.dll"

        // %CSC% 
        do! csc """/nologo /r:"%s"  /r:System.Core.dll /r:lib--optimize.dll    /out:test--optimize.exe""" cfg.FSCOREDLLPATH ["test.cs"]
        
        }

    let run cfg dir = attempt {
        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult

        // %CLIX% .\test.exe
        do! exec ("."/"test.exe") ""

        // %CLIX% .\test--optimize.exe
        do! exec ("."/"test--optimize.exe") ""

        }

    [<Test; FSharpSuiteTest("core/fsfromcs")>]
    let fsfromcs () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        do! build cfg dir

        do! run cfg dir
                
        })

module FsFromFsViaCs = 

    let build cfg dir = attempt {
        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)
        let peverify = Commands.peverify exec cfg.PEVERIFY "/nologo"
        let csc = Printf.ksprintf (Commands.csc exec cfg.CSC)
        let fsc_flags = cfg.fsc_flags

        // "%FSC%" %fsc_flags% -a -o:lib.dll -g lib.fs
        do! fsc "%s -a -o:lib.dll -g" fsc_flags ["lib.fs"]

        // "%PEVERIFY%" lib.dll
        do! peverify "lib.dll"

        // %CSC% /nologo /target:library /r:"%FSCOREDLLPATH%" /r:lib.dll /out:lib2.dll lib2.cs 
        do! csc """/nologo /target:library /r:"%s" /r:lib.dll /out:lib2.dll""" cfg.FSCOREDLLPATH ["lib2.cs"]

        // "%FSC%" %fsc_flags% -r:lib.dll -r:lib2.dll -o:test.exe -g test.fsx
        do! fsc "%s -r:lib.dll -r:lib2.dll -o:test.exe -g" fsc_flags ["test.fsx"]

        // "%PEVERIFY%" test.exe 
        do! peverify "test.exe"

        }

    let run cfg dir = attempt {
        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult

        // %CLIX% .\test.exe
        do! exec ("."/"test.exe") ""

        }

    [<Test; FSharpSuiteTest("core/fsfromfsviacs")>]
    let fsfromfsviacs () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        do! build cfg dir

        do! run cfg dir
                
        })


module ``FSI-reload`` = 

    [<Test; FSharpSuiteTest("core/fsi-reload")>]
    let ``fsi-reload`` () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let ``exec <`` l p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = Some(RedirectInput(l)) } p >> checkResult
        let ``fsi <`` = Printf.ksprintf (fun flags l -> Commands.fsi (``exec <`` l) cfg.FSI flags [])
        let fsi = Printf.ksprintf (Commands.fsi exec cfg.FSI)
        let fileguard = (Commands.getfullpath dir) >> FileGuard.create
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)

        /////// build.bat ///////

        // REM  NOTE that this test does not do anything.
        // REM  PEVERIFY not needed

        /////// run.bat  ////////

        do! attempt {
            // if exist test.ok (del /f /q test.ok)
            use testOkFile = fileguard "test.ok"
            // "%FSI%" %fsi_flags%  --maxerrors:1 < test1.ml
            do! ``fsi <`` "%s  --maxerrors:1" cfg.fsi_flags "test1.ml"
            // if NOT EXIST test.ok goto SetError
            do! testOkFile |> NUnitConf.checkGuardExists
            }
                
        do! attempt {
            // if exist test.ok (del /f /q test.ok)
            use testOkFile = fileguard "test.ok"
            // "%FSI%" %fsi_flags%  --maxerrors:1 load1.fsx
            do! fsi "%s  --maxerrors:1" cfg.fsi_flags ["load1.fsx"]
            // if NOT EXIST test.ok goto SetError
            do! testOkFile |> NUnitConf.checkGuardExists
            }

        do! attempt {
            // if exist test.ok (del /f /q test.ok)
            use testOkFile = fileguard "test.ok"
            // "%FSI%" %fsi_flags%  --maxerrors:1 load2.fsx
            do! fsi "%s  --maxerrors:1" cfg.fsi_flags ["load2.fsx"]
            // if NOT EXIST test.ok goto SetError
            do! testOkFile |> NUnitConf.checkGuardExists
            }

        // REM Check we can also compile, for sanity's sake
        // "%FSC%" load1.fsx
        do! fsc "" ["load1.fsx"]

        // REM Check we can also compile, for sanity's sake
        // "%FSC%" load2.fsx
        do! fsc "" ["load2.fsx"]

        })


module fsiAndModifiers = 

    let build cfg dir = attempt {
        let ``exec <`` l p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = Some(RedirectInput(l)) } p >> checkResult
        let ``fsi <`` = Printf.ksprintf (fun flags l -> Commands.fsi (``exec <`` l) cfg.FSI flags [])
        let del = Commands.rm dir
        let exist = Commands.fileExists dir >> Option.isSome

        // if exist TestLibrary.dll (del /f /q TestLibrary.dll)
        do if exist "TestLibrary.dll" then del "TestLibrary.dll"

        // "%FSI%" %fsi_flags%  --maxerrors:1 < prepare.fsx
        do! ``fsi <`` "%s  --maxerrors:1" cfg.fsi_flags "prepare.fsx"

        }

    let run cfg dir = attempt {
        let ``exec <`` l p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = Some(RedirectInput(l)) } p >> checkResult
        let ``fsi <`` = Printf.ksprintf (fun flags l -> Commands.fsi (``exec <`` l) cfg.FSI flags [])
        let fileguard = (Commands.getfullpath dir) >> FileGuard.create

        // if exist test.ok (del /f /q test.ok)
        use testOkFile = fileguard "test.ok"
        
        // "%FSI%" %fsi_flags%  --maxerrors:1 < test.fsx
        do! ``fsi <`` "%s  --maxerrors:1" cfg.fsi_flags "test.fsx"

        // if NOT EXIST test.ok goto SetError
        do! testOkFile |> NUnitConf.checkGuardExists

        }

    [<Test; FSharpSuiteTest("core/fsiAndModifiers")>]
    let fsiAndModifiers () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        do! build cfg dir

        do! run cfg dir
                
        })

module GenericMeasures = 

    [<Test; FSharpSuiteCodeAndSignaturePermutations("core/genericmeasures")>]
    let genericmeasures p = singleTestBuildAndRun p

module Hiding = 

    [<Test; FSharpSuiteTest("core/hiding")>]
    let hiding () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)
        let peverify = Commands.peverify exec cfg.PEVERIFY "/nologo"
        let fsc_flags = cfg.fsc_flags

        // "%FSC%" %fsc_flags% -a --optimize -o:lib.dll lib.mli lib.ml libv.ml
        do! fsc "%s -a --optimize -o:lib.dll" fsc_flags ["lib.mli";"lib.ml";"libv.ml"]

        // "%PEVERIFY%" lib.dll
        do! peverify "lib.dll"

        // "%FSC%" %fsc_flags% -a --optimize -r:lib.dll -o:lib2.dll lib2.mli lib2.ml lib3.ml
        do! fsc "%s -a --optimize -r:lib.dll -o:lib2.dll" fsc_flags ["lib2.mli";"lib2.ml";"lib3.ml"]

        // "%PEVERIFY%" lib2.dll
        do! peverify "lib2.dll"

        // "%FSC%" %fsc_flags% --optimize -r:lib.dll -r:lib2.dll -o:client.exe client.ml
        do! fsc "%s --optimize -r:lib.dll -r:lib2.dll -o:client.exe" fsc_flags ["client.ml"]

        // "%PEVERIFY%" client.exe
        do! peverify "client.exe"

        })


module Innerpoly = 

    [<Test; FSharpSuiteCodeAndSignaturePermutations("core/innerpoly")>]
    let innerpoly p = singleTestBuildAndRun p
        
        
module ``test int32`` = 

    [<Test; FSharpSuiteScriptPermutations("core/int32")>]
    let int32 p = singleTestBuildAndRun p


module QueriesCustomQueryOps = 

    let build cfg dir = attempt {
        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)
        let peverify = Commands.peverify exec cfg.PEVERIFY "/nologo"
        let csc = Printf.ksprintf (Commands.csc exec cfg.CSC)
        let fsc_flags = cfg.fsc_flags

        // "%FSC%" %fsc_flags% -o:test.exe -g test.fsx
        do! fsc """%s -o:test.exe -g""" fsc_flags ["test.fsx"]

        // "%PEVERIFY%" test.exe 
        do! peverify "test.exe"

        // "%FSC%" %fsc_flags% --optimize -o:test--optimize.exe -g test.fsx
        do! fsc """%s --optimize -o:test--optimize.exe -g""" fsc_flags ["test.fsx"]

        // "%PEVERIFY%" test--optimize.exe 
        do! peverify "test--optimize.exe"

        // call ..\..\single-neg-test.bat negativetest
        do! SingleNegTest.singleNegTest cfg dir "negativetest"
        
        }

    let run cfg dir = attempt {
        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsi = Printf.ksprintf (Commands.fsi exec cfg.FSI)
        let fileguard = (Commands.getfullpath dir) >> FileGuard.create

        // echo TestC
        log "TestC"
        do! attempt {
            // if exist test.ok (del /f /q test.ok)
            use testOkFile = fileguard "test.ok"

            // "%FSI%" %fsi_flags% test.fsx
            do! fsi "%s" cfg.fsi_flags ["test.fsx"]

            // if NOT EXIST test.ok goto SetError
            do! testOkFile |> NUnitConf.checkGuardExists
        }

        // echo TestD
        log "TestD"
        do! attempt {
            // if exist test.ok (del /f /q test.ok)
            use testOkFile = fileguard "test.ok"

            // %CLIX% test.exe
            do! exec ("."/"test.exe") ""

            // if NOT EXIST test.ok goto SetError
            do! testOkFile |> NUnitConf.checkGuardExists
            }

        do! attempt {
            // if exist test.ok (del /f /q test.ok)
            use testOkFile = fileguard "test.ok"

            // %CLIX% test--optimize.exe
            do! exec ("."/"test--optimize.exe") ""

            // if NOT EXIST test.ok goto SetError
            do! testOkFile |> NUnitConf.checkGuardExists
            }

        }

    [<Test; FSharpSuiteTest("core/queriesCustomQueryOps")>]
    let queriesCustomQueryOps () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        do! build cfg dir

        do! run cfg dir
                
        })

module Printing = 

    // "%FSI%" %fsc_flags_errors_ok%  --nologo --use:preludePrintSize200.fsx      <test.fsx >z.raw.output.test.200.txt     2>&1 
    // findstr /v "%CD%" z.raw.output.test.200.txt     | findstr /v -C:"--help' for options" > z.output.test.200.txt
    // if NOT EXIST z.output.test.200.bsl     COPY z.output.test.200.txt     z.output.test.200.bsl
    // %PRDIFF% z.output.test.200.txt     z.output.test.200.bsl     > z.output.test.200.diff
    [<Test>]
    [<FSharpSuiteTestCase("core/printing", "", "z.output.test.default.stdout.txt", "z.output.test.default.stdout.bsl", "z.output.test.default.stderr.txt", "z.output.test.default.stderr.bsl")>]
    [<FSharpSuiteTestCase("core/printing", "--use:preludePrintSize1000.fsx", "z.output.test.1000.stdout.txt", "z.output.test.1000.stdout.bsl", "z.output.test.1000.stderr.txt", "z.output.test.1000.stderr.bsl")>]
    [<FSharpSuiteTestCase("core/printing", "--use:preludePrintSize200.fsx", "z.output.test.200.stdout.txt", "z.output.test.200.stdout.bsl", "z.output.test.200.stderr.txt", "z.output.test.200.stderr.bsl")>]
    [<FSharpSuiteTestCase("core/printing", "--use:preludeShowDeclarationValuesFalse.fsx", "z.output.test.off.stdout.txt", "z.output.test.off.stdout.bsl", "z.output.test.off.stderr.txt", "z.output.test.off.stderr.bsl")>]
    [<FSharpSuiteTestCase("core/printing", "--quiet", "z.output.test.quiet.stdout.txt", "z.output.test.quiet.stdout.bsl", "z.output.test.quiet.stderr.txt", "z.output.test.quiet.stderr.bsl")>]
    let printing flag diffFileOut expectedFileOut diffFileErr expectedFileErr = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        do! requireENCulture ()

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let peverify = Commands.peverify exec cfg.PEVERIFY "/nologo"
        let copy from' = Commands.copy_y dir from' >> checkResult
        let fileExists = Commands.fileExists dir >> Option.isSome
        let getfullpath = Commands.getfullpath dir

        let ``fsi <a >b 2>c`` =
            // "%FSI%" %fsc_flags_errors_ok%  --nologo                                    <test.fsx >z.raw.output.test.default.txt 2>&1
            let ``exec <a >b 2>c`` (inFile, outFile, errFile) p = 
                Command.exec dir cfg.EnvironmentVariables { Output = OutputAndError(Overwrite(outFile), Overwrite(errFile)); Input = Some(RedirectInput(inFile)); } p 
                >> checkResult
            Printf.ksprintf (fun flags (inFile, outFile, errFile) -> Commands.fsi (``exec <a >b 2>c`` (inFile, outFile, errFile)) cfg.FSI flags [])
        
        let fsdiff a b = 
            let ``exec >`` f p = Command.exec dir cfg.EnvironmentVariables { Output = Output(Overwrite(f)); Input = None} p >> checkResult
            let diffFile = Path.ChangeExtension(a, ".diff")
            Commands.fsdiff (``exec >`` diffFile) cfg.FSDIFF a b

        let fsc_flags_errors_ok = ""

        // echo == Plain
        // "%FSI%" %fsc_flags_errors_ok%  --nologo                                    <test.fsx >z.raw.output.test.default.txt 2>&1
        // echo == PrintSize 1000
        // "%FSI%" %fsc_flags_errors_ok%  --nologo --use:preludePrintSize1000.fsx     <test.fsx >z.raw.output.test.1000.txt    2>&1 
        // echo == PrintSize 200
        // "%FSI%" %fsc_flags_errors_ok%  --nologo --use:preludePrintSize200.fsx      <test.fsx >z.raw.output.test.200.txt     2>&1 
        // echo == ShowDeclarationValues off
        // "%FSI%" %fsc_flags_errors_ok%  --nologo --use:preludeShowDeclarationValuesFalse.fsx <test.fsx >z.raw.output.test.off.txt     2>&1
        // echo == Quiet
        // "%FSI%" %fsc_flags_errors_ok% --nologo --quiet                              <test.fsx >z.raw.output.test.quiet.txt   2>&1
        let rawFileOut = Path.GetTempFileName()
        let rawFileErr = Path.GetTempFileName()
        do! ``fsi <a >b 2>c`` "%s --nologo %s" fsc_flags_errors_ok flag ("test.fsx", rawFileOut, rawFileErr)

        // REM REVIEW: want to normalise CWD paths, not suppress them.
        let ``findstr /v`` text = Seq.filter (fun (s: string) -> not <| s.Contains(text))
        let removeCDandHelp from' to' =
            File.ReadLines from' |> (``findstr /v`` dir) |> (``findstr /v`` "--help' for options") |> (fun lines -> File.WriteAllLines(getfullpath to', lines))

        // findstr /v "%CD%" z.raw.output.test.default.txt | findstr /v -C:"--help' for options" > z.output.test.default.txt
        // findstr /v "%CD%" z.raw.output.test.1000.txt    | findstr /v -C:"--help' for options" > z.output.test.1000.txt
        // findstr /v "%CD%" z.raw.output.test.200.txt     | findstr /v -C:"--help' for options" > z.output.test.200.txt
        // findstr /v "%CD%" z.raw.output.test.off.txt     | findstr /v -C:"--help' for options" > z.output.test.off.txt
        // findstr /v "%CD%" z.raw.output.test.quiet.txt   | findstr /v -C:"--help' for options" > z.output.test.quiet.txt
        removeCDandHelp rawFileOut diffFileOut
        removeCDandHelp rawFileErr diffFileErr

        let withDefault default' to' =
            if not (fileExists to') then Some (copy default' to') else None

        do! expectedFileOut |> withDefault diffFileOut
        do! expectedFileErr |> withDefault diffFileErr

        do! fsdiff diffFileOut expectedFileOut
        do! fsdiff diffFileErr expectedFileErr

        ignore "printed to log"


        })

module Quotes = 

    let build cfg dir = attempt {
        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)
        let peverify = Commands.peverify exec cfg.PEVERIFY "/nologo"
        let fsc_flags = cfg.fsc_flags
        let csc = Printf.ksprintf (Commands.csc exec cfg.CSC)

        //missing csc
        do! csc """/nologo  /target:library /out:cslib.dll""" ["cslib.cs"]

        // "%FSC%" %fsc_flags% -o:test.exe -r cslib.dll -g test.fsx
        do! fsc "%s -o:test.exe -r cslib.dll -g" fsc_flags ["test.fsx"]

        // "%PEVERIFY%" test.exe 
        do! peverify "test.exe"

        // "%FSC%" %fsc_flags% -o:test-with-debug-data.exe --quotations-debug+ -r cslib.dll -g test.fsx
        do! fsc "%s -o:test-with-debug-data.exe --quotations-debug+ -r cslib.dll -g" fsc_flags ["test.fsx"]

        // "%PEVERIFY%" test-with-debug-data.exe 
        do! peverify "test-with-debug-data.exe"

        // "%FSC%" %fsc_flags% --optimize -o:test--optimize.exe -r cslib.dll -g test.fsx
        do! fsc "%s --optimize -o:test--optimize.exe -r cslib.dll -g" fsc_flags ["test.fsx"]

        // "%PEVERIFY%" test--optimize.exe 
        do! peverify "test--optimize.exe"
        
        }

    let run cfg dir = attempt {
        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsi = Printf.ksprintf (Commands.fsi exec cfg.FSI)
        let fileguard = (Commands.getfullpath dir) >> FileGuard.create

        do! attempt {
            // if exist test.ok (del /f /q test.ok)
            use testOkFile = fileguard "test.ok"

            // "%FSI%" %fsi_flags% -r cslib.dll test.fsx
            do! fsi "%s -r cslib.dll" cfg.fsi_flags ["test.fsx"]
            
            // if NOT EXIST test.ok goto SetError
            do! testOkFile |> NUnitConf.checkGuardExists
            }

        do! attempt {
            // if exist test.ok (del /f /q test.ok)
            use testOkFile = fileguard "test.ok"

            // %CLIX% test.exe
            do! exec ("."/"test.exe") ""

            // if NOT EXIST test.ok goto SetError
            do! testOkFile |> NUnitConf.checkGuardExists
            }

        do! attempt {
            // if exist test.ok (del /f /q test.ok)
            use testOkFile = fileguard "test.ok"

            // %CLIX% test-with-debug-data.exe
            do! exec ("."/"test-with-debug-data.exe") ""

            // if NOT EXIST test.ok goto SetError
            do! testOkFile |> NUnitConf.checkGuardExists
            }

        do! attempt {
            // if exist test.ok (del /f /q test.ok)
            use testOkFile = fileguard "test.ok"

            // %CLIX% test--optimize.exe
            do! exec ("."/"test--optimize.exe") ""

            // if NOT EXIST test.ok goto SetError
            do! testOkFile |> NUnitConf.checkGuardExists
            }

        }

    [<Test; FSharpSuiteTest("core/quotes")>]
    let quotes () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        do! build cfg dir

        do! run cfg dir
                
        })


module Namespaces = 

    [<Test; FSharpSuiteCodeAndSignaturePermutations("core/namespaces")>]
    let attributes p = singleTestBuildAndRun p

module Parsing = 

    [<Test; FSharpSuiteTest("core/parsing")>]
    let parsing () = check  (attempt {
        let { Directory = dir; Config = cfg } = testContext ()
        
        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)
        let peverify = Commands.peverify exec cfg.PEVERIFY "/nologo"
        let fsc_flags = cfg.fsc_flags

        // "%FSC%" %fsc_flags% -a -o:crlf.dll -g crlf.ml
        do! fsc "%s -a -o:crlf.dll -g" fsc_flags ["crlf.ml"]

        // "%FSC%" %fsc_flags% -o:toplet.exe -g toplet.ml
        do! fsc "%s -o:toplet.exe -g" fsc_flags ["toplet.ml"]

        // "%PEVERIFY%" toplet.exe
        do! peverify "toplet.exe"

        }) 

module Unicode = 

    let build cfg dir = attempt {
        
        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)
        let fsc_flags = cfg.fsc_flags

        // REM just checking the files actually parse/compile for now....

        // "%FSC%" %fsc_flags% -a -o:kanji-unicode-utf8-nosig-codepage-65001.dll -g kanji-unicode-utf8-nosig-codepage-65001.fs
        do! fsc "%s -a -o:kanji-unicode-utf8-nosig-codepage-65001.dll -g" fsc_flags ["kanji-unicode-utf8-nosig-codepage-65001.fs"]

        // "%FSC%" %fsc_flags% -a -o:kanji-unicode-utf8-nosig-codepage-65001.dll -g kanji-unicode-utf8-nosig-codepage-65001.fs
        do! fsc "%s -a -o:kanji-unicode-utf8-nosig-codepage-65001.dll -g" fsc_flags ["kanji-unicode-utf8-nosig-codepage-65001.fs"]

        let codepage = attempt {
            // "%FSC%" %fsc_flags% -a -o:kanji-unicode-utf16.dll -g kanji-unicode-utf16.fs
            do! fsc "%s -a -o:kanji-unicode-utf16.dll -g" fsc_flags ["kanji-unicode-utf16.fs"]

            // "%FSC%" %fsc_flags% -a --codepage:65000 -o:kanji-unicode-utf7-codepage-65000.dll -g kanji-unicode-utf7-codepage-65000.fs
            do! fsc "%s -a --codepage:65000 -o:kanji-unicode-utf7-codepage-65000.dll -g" fsc_flags ["kanji-unicode-utf7-codepage-65000.fs"]
            }

        // REM check non-utf8 and --codepage flag for bootstrapped fsc.exe
        // if NOT "%FSC:fscp=X%" == "%FSC%" (
        do! if not <| cfg.FSC.Contains("fscp") then codepage else Success

        // "%FSC%" %fsc_flags% -a -o:kanji-unicode-utf8-withsig-codepage-65001.dll -g kanji-unicode-utf8-withsig-codepage-65001.fs
        do! fsc "%s -a -o:kanji-unicode-utf8-withsig-codepage-65001.dll -g" fsc_flags ["kanji-unicode-utf8-withsig-codepage-65001.fs"]
        }

    let run cfg dir = attempt {
        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsi = Printf.ksprintf (Commands.fsi exec cfg.FSI)
        let fsi_flags = cfg.fsi_flags

        // if exist test.ok (del /f /q test.ok)
        ignore "unused"
        // "%FSI%" %fsi_flags% --utf8output kanji-unicode-utf8-nosig-codepage-65001.fs
        do! fsi "%s --utf8output" fsi_flags ["kanji-unicode-utf8-nosig-codepage-65001.fs"]

        // if exist test.ok (del /f /q test.ok)
        ignore "unused"
        // "%FSI%" %fsi_flags% --utf8output --codepage:65001 kanji-unicode-utf8-withsig-codepage-65001.fs
        do! fsi "%s --utf8output --codepage:65001" fsi_flags ["kanji-unicode-utf8-withsig-codepage-65001.fs"]

        // if exist test.ok (del /f /q test.ok)
        ignore "unused"
        // "%FSI%" %fsi_flags% --utf8output kanji-unicode-utf8-withsig-codepage-65001.fs
        do! fsi "%s --utf8output" fsi_flags ["kanji-unicode-utf8-withsig-codepage-65001.fs"]

        // if exist test.ok (del /f /q test.ok)
        ignore "unused"
        // "%FSI%" %fsi_flags% --utf8output --codepage:65000  kanji-unicode-utf7-codepage-65000.fs
        do! fsi "%s --utf8output --codepage:65000" fsi_flags ["kanji-unicode-utf7-codepage-65000.fs"]

        // if exist test.ok (del /f /q test.ok)
        ignore "unused"
        // "%FSI%" %fsi_flags% --utf8output kanji-unicode-utf16.fs
        do! fsi "%s --utf8output" fsi_flags ["kanji-unicode-utf16.fs"]
        }


    [<Test; FSharpSuiteTest("core/unicode")>]
    let unicode () = check  (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        do! build cfg dir  
        do! run cfg dir
        }) 

    [<Test; FSharpSuiteScriptPermutations("core/unicode")>]
    let unicode2 p = singleTestBuildAndRun p

module InternalsVisible =

    [<Test; FSharpSuiteTest("core/internalsvisible")>]
    let internalsvisible () = check  (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)
        let peverify = Commands.peverify exec cfg.PEVERIFY "/nologo"
        let csc = Printf.ksprintf (Commands.csc exec cfg.CSC)
        let fsc_flags = cfg.fsc_flags

        // REM Test internals visible

        // echo == Compiling F# Library
        log "== Compiling F# Library"
        // "%FSC%" %fsc_flags% --version:1.2.3 --keyfile:key.snk -a --optimize -o:library.dll library.fsi library.fs
        do! fsc "%s --version:1.2.3 --keyfile:key.snk -a --optimize -o:library.dll" fsc_flags ["library.fsi"; "library.fs"]

        // echo == Verifying F# Library
        log "== Verifying F# Library"

        // "%PEVERIFY%" library.dll
        do! peverify "library.dll"

        // echo == Compiling C# Library
        log "== Compiling C# Library"
        // %CSC% /target:library /keyfile:key.snk /out:librarycs.dll librarycs.cs
        do! csc "/target:library /keyfile:key.snk /out:librarycs.dll" ["librarycs.cs"]

        // echo == Verifying C# Library
        log "== Verifying C# Library"
        // "%PEVERIFY%" librarycs.dll
        do! peverify "librarycs.dll"

        // echo == Compiling F# main referencing C# and F# libraries
        log "== Compiling F# main referencing C# and F# libraries"
        // "%FSC%" %fsc_flags% --version:1.2.3 --keyfile:key.snk --optimize -r:library.dll -r:librarycs.dll -o:main.exe main.fs
        do! fsc "%s --version:1.2.3 --keyfile:key.snk --optimize -r:library.dll -r:librarycs.dll -o:main.exe" fsc_flags ["main.fs"]

        // echo == Verifying F# main
        log "== Verifying F# main"
        // "%PEVERIFY%" main.exe
        do! peverify "main.exe"

        // echo == Run F# main. Quick test!
        log "== Run F# main. Quick test!"
        // main.exe
        do! exec ("."/"main.exe") ""
        }) 


module Interop = 

    let build cfg dir = attempt {
        let envVars =
            cfg.EnvironmentVariables
            |> Map.add "FSCOREDLLPATH" cfg.FSCOREDLLPATH
            |> Map.add "FSCOREDLLNETCORE78PATH" cfg.FSCOREDLLNETCORE78PATH

        let exec p = Command.exec dir envVars { Output = Inherit; Input = None; } p >> checkResult
        let peverify = Commands.peverify exec cfg.PEVERIFY "/nologo"
        let msbuild = Printf.ksprintf (Commands.msbuild exec (cfg.MSBUILD.Value))

        // rd /S /Q obj
        // del /f /q *.pdb *.xml *.config *.dll *.exe

        // "%MSBUILDTOOLSPATH%\msbuild.exe" PCL.fsproj
        do! msbuild "" ["PCL.fsproj"]

        // "%MSBUILDTOOLSPATH%\msbuild.exe" User.fsproj
        do! msbuild "" ["User.fsproj"]

        // %PEVERIFY% User.exe
        do! peverify "User.exe"

        }

    let run cfg dir = attempt {
        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult

        do! exec ("."/"User.exe") ""
        }

    [<Test; FSharpSuiteTest("core/interop")>]
    let interop () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        do! build cfg dir

        do! run cfg dir
                
        })

module ``test lazy`` = 

    [<Test; FSharpSuiteScriptPermutations("core/lazy")>]
    let ``lazy`` p = singleTestBuildAndRun p

module letrec = 

    [<Test; FSharpSuiteScriptPermutations("core/letrec")>]
    let letrec p = singleTestBuildAndRun p

    [<Test; FSharpSuiteScriptPermutations("core/letrec-mutrec")>]
    let ``letrec (mutrec variations part one)`` p = singleTestBuildAndRun p

    [<Test; FSharpSuiteScriptPermutations("core/letrec-mutrec2")>]
    let ``letrec (mutrec variations part two)`` p = singleTestBuildAndRun p

module LibTest = 

    [<Test; FSharpSuiteAllPermutations("core/libtest")>]
    let libtest p = singleTestBuildAndRun p

module Lift = 

    [<Test; FSharpSuiteScriptPermutations("core/lift")>]
    let lift p = singleTestBuildAndRun p


module ``Load-Script`` = 

    let ``script > a 2>b`` cfg dir (stdout,stderr) = attempt {

        let stdoutPath = stdout |> Commands.getfullpath dir
        let stderrPath = stderr |> Commands.getfullpath dir

        let alwaysSuccess _ = Success ()

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = OutputAndError(Append(stdoutPath), Append(stderrPath)); Input = None; } p >> alwaysSuccess
        let ``exec <`` l p = Command.exec dir cfg.EnvironmentVariables { Output = OutputAndError(Append(stdoutPath), Append(stderrPath)); Input = Some(RedirectInput(l)) } p >> alwaysSuccess
        
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)
        let type_append_tofile from = Commands.type_append_tofile dir from stdoutPath
        let echo text = Commands.echo_append_tofile dir text stdoutPath
        let fsi = Printf.ksprintf (Commands.fsi exec cfg.FSI)
        let ``fsi <`` = Printf.ksprintf (fun flags l -> Commands.fsi (``exec <`` l) cfg.FSI flags [])
        let fileExists = Commands.fileExists dir >> Option.isSome
        let del = Commands.rm dir
        let getfullpath = Commands.getfullpath dir

        File.WriteAllText(stdoutPath, "")
        File.WriteAllText(stderrPath, "")

        // del 3.exe 2>nul 1>nul
        do if fileExists "3.exe" then getfullpath "3.exe" |> File.Delete
        // type 1.fsx 2.fsx 3.fsx
        ["1.fsx"; "2.fsx"; "3.fsx"] |> List.iter type_append_tofile
        // echo Test 1=================================================
        echo "Test 1================================================="
        // "%FSC%" 3.fsx --nologo
        do! fsc "--nologo" ["3.fsx"]
        // 3.exe
        do! exec ("."/"3.exe") ""
        // del 3.exe
        del "3.exe"
        // echo Test 2=================================================
        echo "Test 2================================================="
        // "%FSI%" 3.fsx
        do! fsi "" ["3.fsx"]
        // echo Test 3=================================================
        echo "Test 3================================================="
        // "%FSI%" --nologo < pipescr
        do! ``fsi <`` "--nologo" "pipescr"
        // echo.
        // echo Test 4=================================================
        echo "Test 4================================================="
        // "%FSI%" usesfsi.fsx
        do! fsi "" ["usesfsi.fsx"]
        // echo Test 5=================================================
        echo "Test 5================================================="
        // "%FSC%" usesfsi.fsx --nologo
        do! fsc "--nologo" ["usesfsi.fsx"]
        // echo Test 6=================================================
        echo "Test 6================================================="
        // "%FSC%" usesfsi.fsx --nologo -r FSharp.Compiler.Interactive.Settings
        do! fsc "--nologo -r FSharp.Compiler.Interactive.Settings" ["usesfsi.fsx"]
        // echo Test 7=================================================
        echo "Test 7================================================="
        // "%FSI%" 1.fsx 2.fsx 3.fsx
        do! fsi "" ["1.fsx";"2.fsx";"3.fsx"]
        // echo Test 8=================================================
        echo "Test 8================================================="
        // "%FSI%" 3.fsx 2.fsx 1.fsx
        do! fsi "" ["3.fsx";"2.fsx";"1.fsx"]
        // echo Test 9=================================================
        echo "Test 9================================================="
        // "%FSI%" multiple-load-1.fsx
        do! fsi "" ["multiple-load-1.fsx"]
        // echo Test 10=================================================
        echo "Test 10================================================="
        // "%FSI%" multiple-load-2.fsx
        do! fsi "" ["multiple-load-2.fsx"]
        // echo Test 11=================================================
        echo "Test 11================================================="
        // "%FSC%" FlagCheck.fs --nologo
        do! fsc "--nologo" ["FlagCheck.fs"]
        // FlagCheck.exe
        do! exec ("."/"FlagCheck.exe") ""
        // del FlagCheck.exe
        del "FlagCheck.exe"
        // echo Test 12=================================================
        echo "Test 12================================================="
        // "%FSC%" FlagCheck.fsx  --nologo
        do! fsc "-o FlagCheckScript.exe --nologo" ["FlagCheck.fsx"]
        // FlagCheck.exe
        do! exec ("."/"FlagCheckScript.exe") ""
        // del FlagCheck.exe
        del "FlagCheckScript.exe"
        // echo Test 13=================================================
        echo "Test 13================================================="
        // "%FSI%" load-FlagCheckFs.fsx
        do! fsi "" ["load-FlagCheckFs.fsx"]
        // echo Test 14=================================================
        echo "Test 14================================================="
        // "%FSI%" FlagCheck.fsx
        do! fsi "" ["FlagCheck.fsx"]
        // echo Test 15=================================================
        echo "Test 15================================================="
        // "%FSI%" ProjectDriver.fsx
        do! fsi "" ["ProjectDriver.fsx"]
        // echo Test 16=================================================
        echo "Test 16================================================="
        // "%FSC%" ProjectDriver.fsx --nologo
        do! fsc "--nologo" ["ProjectDriver.fsx"]
        // ProjectDriver.exe
        do! exec ("."/"ProjectDriver.exe") ""
        // del ProjectDriver.exe
        del "ProjectDriver.exe"
        // echo Test 17=================================================
        echo "Test 17================================================="
        // "%FSI%" load-IncludeNoWarn211.fsx
        do! fsi "" ["load-IncludeNoWarn211.fsx"]
        // echo Done ==================================================
        echo "Done =================================================="
        }

    let build cfg dir = attempt {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let getfullpath = Commands.getfullpath dir

        let fsdiff a b = attempt {
            let out = new ResizeArray<string>()
            let redirectOutputToFile path args =
                log "%s %s" path args
                use toLog = redirectToLog ()
                Process.exec { RedirectOutput = Some (function null -> () | s -> out.Add(s)); RedirectError = Some toLog.Post; RedirectInput = None; } dir cfg.EnvironmentVariables path args
            do! (Commands.fsdiff redirectOutputToFile cfg.FSDIFF a b) |> (fun _ -> Success ())
            return out.ToArray() |> List.ofArray
            }


        // script > out.txt 2>&1
        do! ``script > a 2>b`` cfg dir ("out.stdout.txt", "out.stderr.txt")

        // if NOT EXIST out.bsl COPY out.txt
        ignore "useless, first run, same as use an empty file"

        let normalizePaths f =
            let text = File.ReadAllText(f)
            let dummyPath = @"D:\staging\staging\src\tests\fsharp\core\load-script"
            let contents = System.Text.RegularExpressions.Regex.Replace(text, System.Text.RegularExpressions.Regex.Escape(dir), dummyPath)
            File.WriteAllText(f, contents)

        normalizePaths (getfullpath "out.stdout.txt")
        normalizePaths (getfullpath "out.stderr.txt")

        let! diffs = fsdiff (getfullpath "out.stdout.txt") (getfullpath "out.stdout.bsl")

        do! match diffs with
            | [] -> Success
            | l -> NUnitConf.genericError (sprintf "'%s' and '%s' differ; %A" (getfullpath "out.stdout.txt") (getfullpath "out.stdout.bsl") diffs)

        let! diffs = fsdiff (getfullpath "out.stderr.txt") (getfullpath "out.stderr.bsl")

        do! match diffs with
            | [] -> Success
            | l -> NUnitConf.genericError (sprintf "'%s' and '%s' differ; %A" (getfullpath "out.stderr.txt") (getfullpath "out.stderr.bsl") diffs)
        }

    [<Test; FSharpSuiteTest("core/load-script")>]
    let ``load-script`` () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        do! build cfg dir

        })

module LongNames = 

    [<Test; FSharpSuiteScriptPermutations("core/longnames")>]
    let longnames p = singleTestBuildAndRun p


module ``test map`` = 

    [<Test; FSharpSuiteScriptPermutations("core/map")>]
    let map p = singleTestBuildAndRun p

module Math =
    //TODO math/lalgebra does not have build.bat/run.bat and #r "FSharp.Math.Providers.dll"
    
    module Numbers = 

        [<Test; FSharpSuiteScriptPermutations("core/math/numbers")>]
        let numbers p = singleTestBuildAndRun p


    module numbersVS2008 = 

        [<Test; FSharpSuiteScriptPermutations("core/math/numbersVS2008")>]
        let numbersVS2008 p = singleTestBuildAndRun p



module Measures = 

    [<Test; FSharpSuiteCodeAndSignaturePermutations("core/measures")>]
    let measures p = singleTestBuildAndRun p

module Members =

    module Basics = 

        [<Test; FSharpSuiteCodeAndSignaturePermutations("core/members/basics")>]
        let Basics p = singleTestBuildAndRun p

        [<Test; FSharpSuiteScriptPermutations("core/members/basics-hw")>]
        let BasicsHw p = singleTestBuildAndRun p

        [<Test; FSharpSuiteScriptPermutations("core/members/basics-hw-mutrec")>]
        let BasicsHwMutrec p = singleTestBuildAndRun p

    module Ctree = 

        [<Test; FSharpSuiteScriptPermutations("core/members/ctree")>]
        let ctree p = singleTestBuildAndRun p

    module Factors = 

        [<Test; FSharpSuiteScriptPermutations("core/members/factors")>]
        let factors p = singleTestBuildAndRun p

        [<Test; FSharpSuiteScriptPermutations("core/members/factors-mutrec")>]
        let factorsMutrec p = singleTestBuildAndRun p

    module Incremental = 

        [<Test; FSharpSuiteScriptPermutations("core/members/incremental")>]
        let incremental p = singleTestBuildAndRun p

        [<Test; FSharpSuiteScriptPermutations("core/members/incremental-hw")>]
        let incrementalHw p = singleTestBuildAndRun p

        [<Test; FSharpSuiteScriptPermutations("core/members/incremental-hw-mutrec")>]
        let incrementalHwMutrec p = singleTestBuildAndRun p

    module Ops =

        [<Test; FSharpSuiteScriptPermutations("core/members/ops")>]
        let ops p = singleTestBuildAndRun p

        [<Test; FSharpSuiteScriptPermutations("core/members/ops-mutrec")>]
        let opsMutrec p = singleTestBuildAndRun p


module Nested = 

    [<Test; FSharpSuiteScriptPermutations("core/nested")>]
    let nested p = singleTestBuildAndRun p


module NetCore =

    module Netcore259 = 

        let build cfg dir = attempt {
            // IF /I "%INSTALL_SKU%" NEQ "ULTIMATE" (
            //     echo Test not supported except on Ultimate
            //     exit /b 0
            // )
            do! requireVSUltimate cfg

            let envVars =
                cfg.EnvironmentVariables
                |> Map.add "FSCOREDLLNETCORE259PATH" cfg.FSCOREDLLNETCORE259PATH

            let exec p = Command.exec dir envVars { Output = Inherit; Input = None; } p >> checkResult
            let msbuild = Printf.ksprintf (Commands.msbuild exec (cfg.MSBUILD.Value))

            // "%MSBUILDTOOLSPATH%\msbuild.exe" ..\netcore.sln /p:Configuration=Debug /p:TestProfile=Profile259 /t:Rebuild
            do! msbuild "/p:Configuration=Debug /p:TestProfile=Profile259 /t:Rebuild" [".."/"netcore.sln"]

            }

        let run cfg dir = attempt {
            let getfullpath = Commands.getfullpath dir

            // IF /I "%INSTALL_SKU%" NEQ "ULTIMATE" (
            //     echo Test not supported except on Ultimate
            //     exit /b 0
            // )
            do! requireVSUltimate cfg

            // set CONTROL_FAILURES_LOG=%~dp0..\ConsoleApplication1\bin\Debug\Profile259\control_failures.log
            let setLog = Map.add "CONTROL_FAILURES_LOG" (getfullpath ".."/"ConsoleApplication1"/"bin"/"Debug"/"Profile259"/"control_failures.log")

            let exec p = Command.exec dir (cfg.EnvironmentVariables |> setLog) { Output = Inherit; Input = None; } p >> checkResult

            // ..\ConsoleApplication1\bin\Debug\Profile259\PortableTestEntry.exe
            do! exec (".."/"ConsoleApplication1"/"bin"/"Debug"/"Profile259"/"PortableTestEntry.exe") ""

            }

        [<Test; FSharpSuiteTest("core/netcore/netcore259")>]
        let netcore259 () = check (attempt {
            let { Directory = dir; Config = cfg } = testContext ()

            do! build cfg dir

            do! run cfg dir
                
            })


    module Netcore7 = 

        let build cfg dir = attempt {
            // IF /I "%INSTALL_SKU%" NEQ "ULTIMATE" (
            //     echo Test not supported except on Ultimate
            //     exit /b 0
            // )
            do! requireVSUltimate cfg

            let envVars =
                cfg.EnvironmentVariables
                |> Map.add "FSCOREDLLNETCOREPATH" cfg.FSCOREDLLNETCOREPATH

            let exec p = Command.exec dir envVars { Output = Inherit; Input = None; } p >> checkResult
            let msbuild = Printf.ksprintf (Commands.msbuild exec (cfg.MSBUILD.Value))

            // "%MSBUILDTOOLSPATH%\msbuild.exe" ..\netcore.sln /p:Configuration=Debug /p:TestProfile=Profile7 /t:Rebuild
            do! msbuild "/p:Configuration=Debug /p:TestProfile=Profile7 /t:Rebuild" [".."/"netcore.sln"]
            }

        let run cfg dir = attempt {
            let getfullpath = Commands.getfullpath dir

            // IF /I "%INSTALL_SKU%" NEQ "ULTIMATE" (
            //     echo Test not supported except on Ultimate
            //     exit /b 0
            // )
            do! requireVSUltimate cfg
   
            // set CONTROL_FAILURES_LOG=%~dp0..\ConsoleApplication1\bin\Debug\profile7\control_failures.log
            let setLog = Map.add "CONTROL_FAILURES_LOG" (getfullpath ".."/"ConsoleApplication1"/"bin"/"Debug"/"profile7"/"control_failures.log")
   
            let exec p = Command.exec dir (cfg.EnvironmentVariables |> setLog) { Output = Inherit; Input = None; } p >> checkResult

            // ..\ConsoleApplication1\bin\Debug\profile7\PortableTestEntry.exe
            do! exec (".."/"ConsoleApplication1"/"bin"/"Debug"/"profile7"/"PortableTestEntry.exe") ""
            }

        [<Test; FSharpSuiteTest("core/netcore/netcore7")>]
        let netcore7 () = check (attempt {
            let { Directory = dir; Config = cfg } = testContext ()

            do! build cfg dir

            do! run cfg dir
                
            })

    module Netcore78 = 

        let build cfg dir = attempt {
            // IF /I "%INSTALL_SKU%" NEQ "ULTIMATE" (
            //     echo Test not supported except on Ultimate
            //     exit /b 0
            // )
            do! requireVSUltimate cfg

            let envVars =
                cfg.EnvironmentVariables
                |> Map.add "FSCOREDLLNETCORE78PATH" cfg.FSCOREDLLNETCORE78PATH

            let exec p = Command.exec dir envVars { Output = Inherit; Input = None; } p >> checkResult
            let msbuild = Printf.ksprintf (Commands.msbuild exec (cfg.MSBUILD.Value))

            // "%MSBUILDTOOLSPATH%\msbuild.exe" ..\netcore.sln /p:Configuration=Debug /p:TestProfile=Profile78 /t:Rebuild
            do! msbuild "/p:Configuration=Debug /p:TestProfile=Profile78 /t:Rebuild" [".."/"netcore.sln"]
            }

        let run cfg dir = attempt {
            let getfullpath = Commands.getfullpath dir

            // IF /I "%INSTALL_SKU%" NEQ "ULTIMATE" (
            //     echo Test not supported except on Ultimate
            //     exit /b 0
            // )
            do! requireVSUltimate cfg
   
            // set CONTROL_FAILURES_LOG=%~dp0..\ConsoleApplication1\bin\Debug\profile78\control_failures.log
            let setLog = Map.add "CONTROL_FAILURES_LOG" (getfullpath ".."/"ConsoleApplication1"/"bin"/"Debug"/"profile78"/"control_failures.log")
   
            let exec p = Command.exec dir (cfg.EnvironmentVariables |> setLog) { Output = Inherit; Input = None; } p >> checkResult


            // ..\ConsoleApplication1\bin\Debug\profile78\PortableTestEntry.exe
            do! exec (".."/"ConsoleApplication1"/"bin"/"Debug"/"profile78"/"PortableTestEntry.exe") ""
            }

        [<Test; FSharpSuiteTest("core/netcore/netcore78")>]
        let netcore78 () = check (attempt {
            let { Directory = dir; Config = cfg } = testContext ()

            do! build cfg dir

            do! run cfg dir
                
            })


module Patterns = 

    [<Test; FSharpSuiteScriptPermutations("core/patterns")>]
    let patterns p = singleTestBuildAndRun p

module Pinvoke = 

    [<Test; FSharpSuiteTest("core/pinvoke")>]
    let pinvoke () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let peverify = Commands.peverify exec cfg.PEVERIFY
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)

        // "%FSC%" %fsc_flags% -o:test.exe -g test.fsx
        do! fsc "%s -o:test.exe -g" cfg.fsc_flags ["test.fsx"]
   
        // REM The IL is unverifiable code
        // "%PEVERIFY%" /MD test.exe
        do! peverify "/MD" "test.exe"
                
        })



module Portable = 

    let build cfg dir = attempt {
        let envVars =
            cfg.EnvironmentVariables
            |> Map.add "FSCOREDLLPORTABLEPATH" cfg.FSCOREDLLPORTABLEPATH

        let exec p = Command.exec dir envVars { Output = Inherit; Input = None; } p >> checkResult
        let msbuild = Printf.ksprintf (Commands.msbuild exec (cfg.MSBUILD.Value))

        do! requireVSUltimate cfg
           
        // "%MSBUILDTOOLSPATH%\msbuild.exe" portablelibrary1.sln /p:Configuration=Debug
        do! msbuild "/p:Configuration=Debug" ["portablelibrary1.sln"]

        }

    let run cfg dir = attempt {
        let getfullpath = Commands.getfullpath dir

        // IF /I "%INSTALL_SKU%" NEQ "ULTIMATE" (
        //     echo Test not supported except on Ultimate
        //     exit /b 0
        // )
        do! requireVSUltimate cfg
           
        // set CONTROL_FAILURES_LOG=%~dp0\control_failures.log
        let setLog = Map.add "CONTROL_FAILURES_LOG" (getfullpath "control_failures.log")

        let exec p = Command.exec dir (cfg.EnvironmentVariables |> setLog) { Output = Inherit; Input = None; } p >> checkResult
           
        // .\ConsoleApplication1\bin\Debug\PortableTestEntry.exe
        do! exec ("."/"ConsoleApplication1"/"bin"/"Debug"/"PortableTestEntry.exe") ""

        }

    [<Test; FSharpSuiteTest("core/portable")>]
    let portable () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        do! build cfg dir

        do! run cfg dir
                
        })


module ``test printf`` = 
    let permutations () = [ FSharpSuiteTestCaseData("core/printf", FSC_BASIC) ]

    [<Test; TestCaseSource("permutations")>]
    let printf p = singleTestBuildAndRun p


module QueriesLeafExpressionConvert = 

    let build cfg dir = attempt {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)
        let peverify = Commands.peverify exec cfg.PEVERIFY "/nologo"
        let fsc_flags = cfg.fsc_flags

        // "%FSC%" %fsc_flags% -o:test.exe -g test.fsx
        do! fsc "%s -o:test.exe -g" fsc_flags ["test.fsx"]

        // "%PEVERIFY%" test.exe 
        do! peverify "test.exe"

        // "%FSC%" %fsc_flags% --optimize -o:test--optimize.exe -g test.fsx
        do! fsc "%s --optimize -o:test--optimize.exe -g" fsc_flags ["test.fsx"]

        // "%PEVERIFY%" test--optimize.exe 
        do! peverify "test--optimize.exe"

        }

    let run cfg dir = attempt {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsi = Printf.ksprintf (Commands.fsi exec cfg.FSI)
        let fileguard = (Commands.getfullpath dir) >> FileGuard.create

        // REM fsi.exe testing
        // echo TestC
        log "TestC"

        // if exist test.ok (del /f /q test.ok)
        use testOkFile = fileguard "test.ok"
        // "%FSI%" %fsi_flags% test.fsx
        do! fsi "%s" cfg.fsi_flags ["test.fsx"]
        // if NOT EXIST test.ok goto SetError
        do! testOkFile |> NUnitConf.checkGuardExists

        // REM fsc.exe testing

        // echo TestD
        log "TestD"

        // if exist test.ok (del /f /q test.ok)
        use testOkFile2 = fileguard "test.ok"
        // %CLIX% test.exe
        do! exec ("."/"test.exe") ""
        // if NOT EXIST test.ok goto SetError
        do! testOkFile2 |> NUnitConf.checkGuardExists



        // if exist test.ok (del /f /q test.ok)
        use testOkFile3 = fileguard "test.ok"
        // %CLIX% test--optimize.exe
        do! exec ("."/"test--optimize.exe") ""
        // if NOT EXIST test.ok goto SetError
        do! testOkFile3 |> NUnitConf.checkGuardExists

        }

    [<Test; FSharpSuiteTest("core/queriesLeafExpressionConvert")>]
    let queriesLeafExpressionConvert () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        do! build cfg dir

        do! run cfg dir
                
        })


module QueriesNullableOperators = 

    let build cfg dir = attempt {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let peverify = Commands.peverify exec cfg.PEVERIFY "/nologo"
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)
        let fsc_flags = cfg.fsc_flags

        // "%FSC%" %fsc_flags% -o:test.exe -g test.fsx
        do! fsc "%s -o:test.exe -g" fsc_flags ["test.fsx"]

        // "%PEVERIFY%" test.exe 
        do! peverify "test.exe"

        // "%FSC%" %fsc_flags% --optimize -o:test--optimize.exe -g test.fsx
        do! fsc "%s --optimize -o:test--optimize.exe -g" fsc_flags ["test.fsx"]

        // "%PEVERIFY%" test--optimize.exe 
        do! peverify "test--optimize.exe"

        }

    let run cfg dir = attempt {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsi = Printf.ksprintf (Commands.fsi exec cfg.FSI)
        let fileguard = (Commands.getfullpath dir) >> FileGuard.create

        // REM fsi.exe testing
        // echo TestC
        log "TestC"

        // if exist test.ok (del /f /q test.ok)
        use testOkFile = fileguard "test.ok"
        // "%FSI%" %fsi_flags% test.fsx
        do! fsi "%s" cfg.fsi_flags ["test.fsx"]
        // if NOT EXIST test.ok goto SetError
        do! testOkFile |> NUnitConf.checkGuardExists

        // REM fsc.exe testing
        // echo TestD
        log "TestD"

        // if exist test.ok (del /f /q test.ok)
        use testOkFile2 = fileguard "test.ok"
        // %CLIX% test.exe
        do! exec ("."/"test.exe") ""
        // if NOT EXIST test.ok goto SetError
        do! testOkFile2 |> NUnitConf.checkGuardExists

        // if exist test.ok (del /f /q test.ok)
        use testOkFile3 = fileguard "test.ok"
        // %CLIX% test--optimize.exe
        do! exec ("."/"test--optimize.exe") ""
        // if NOT EXIST test.ok goto SetError
        do! testOkFile3 |> NUnitConf.checkGuardExists

        }

    [<Test; FSharpSuiteTest("core/queriesNullableOperators")>]
    let queriesNullableOperators () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        do! build cfg dir

        do! run cfg dir
                
        })



module QueriesOverIEnumerable = 

    let build cfg dir = attempt {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let peverify = Commands.peverify exec cfg.PEVERIFY "/nologo"
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)

        // "%FSC%" %fsc_flags% -o:test.exe -g test.fsx
        do! fsc "%s -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

        // "%PEVERIFY%" test.exe
        do! peverify "test.exe" 

        // "%FSC%" %fsc_flags% --optimize -o:test--optimize.exe -g test.fsx
        do! fsc "%s --optimize -o:test--optimize.exe -g" cfg.fsc_flags ["test.fsx"]

        // "%PEVERIFY%" test--optimize.exe 
        do! peverify "test--optimize.exe"

        }

    let run cfg dir = attempt {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsi = Printf.ksprintf (Commands.fsi exec cfg.FSI)
        let fileguard = (Commands.getfullpath dir) >> FileGuard.create

        // REM fsi.exe testing
        //echo TestC
        log "TestC"

        // if exist test.ok (del /f /q test.ok)
        use testOkFile = fileguard "test.ok"
        // "%FSI%" %fsi_flags% test.fsx
        do! fsi "%s" cfg.fsi_flags ["test.fsx"]
        // if NOT EXIST test.ok goto SetError
        do! testOkFile |> NUnitConf.checkGuardExists


        // REM fsc.exe testing
        // echo TestD
        log "TestD"

        // if exist test.ok (del /f /q test.ok)
        use testOkFile2 = fileguard "test.ok"
        // %CLIX% test.exe
        do! exec ("."/"test.exe") ""
        // if NOT EXIST test.ok goto SetError
        do! testOkFile2 |> NUnitConf.checkGuardExists


        // if exist test.ok (del /f /q test.ok)
        use testOkFile3 = fileguard "test.ok"
        // %CLIX% test--optimize.exe
        do! exec ("."/"test--optimize.exe") ""
        // if NOT EXIST test.ok goto SetError
        do! testOkFile3 |> NUnitConf.checkGuardExists

        }

    [<Test; FSharpSuiteTest("core/queriesOverIEnumerable")>]
    let queriesOverIEnumerable () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        do! build cfg dir

        do! run cfg dir
                
        })



module QueriesOverIQueryable = 

    let build cfg dir = attempt {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let peverify = Commands.peverify exec cfg.PEVERIFY "/nologo"
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)

        // "%FSC%" %fsc_flags% -o:test.exe -g test.fsx
        do! fsc "%s -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

        // "%PEVERIFY%" test.exe 
        do! peverify "test.exe"

        // "%FSC%" %fsc_flags% --optimize -o:test--optimize.exe -g test.fsx
        do! fsc "%s --optimize -o:test--optimize.exe -g" cfg.fsc_flags ["test.fsx"]

        // "%PEVERIFY%" test--optimize.exe 
        do! peverify "test--optimize.exe"

        }

    let run cfg dir = attempt {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsi = Printf.ksprintf (Commands.fsi exec cfg.FSI)
        let fileguard = (Commands.getfullpath dir) >> FileGuard.create

        // REM fsi.exe testing
        // echo TestC
        log "TestC"

        // if exist test.ok (del /f /q test.ok)
        use testOkFile = fileguard "test.ok"
        // "%FSI%" %fsi_flags% test.fsx
        do! fsi "%s" cfg.fsi_flags ["test.fsx"]
        // if NOT EXIST test.ok goto SetError
        do! testOkFile |> NUnitConf.checkGuardExists

        // REM fsc.exe testing
        // echo TestD
        log "TestD"

        // if exist test.ok (del /f /q test.ok)
        use testOkFile2 = fileguard "test.ok"
        // %CLIX% test.exe
        do! exec ("."/"test.exe") ""
        // if NOT EXIST test.ok goto SetError
        do! testOkFile2 |> NUnitConf.checkGuardExists

        // if exist test.ok (del /f /q test.ok)
        use testOkFile3 = fileguard "test.ok"
        // %CLIX% test--optimize.exe
        do! exec ("."/"test--optimize.exe") ""
        // if NOT EXIST test.ok goto SetError
        do! testOkFile3 |> NUnitConf.checkGuardExists


        }

    [<Test; FSharpSuiteTest("core/queriesOverIQueryable")>]
    let queriesOverIQueryable () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        do! build cfg dir

        do! run cfg dir
                
        })



module QuotesDebugInfo = 

    let build cfg dir = attempt {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let peverify = Commands.peverify exec cfg.PEVERIFY "/nologo"
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)

        // "%FSC%" %fsc_flags% --quotations-debug+ --optimize -o:test.exe -g test.fsx
        do! fsc "%s --quotations-debug+ --optimize -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

        // "%PEVERIFY%" test.exe 
        do! peverify "test.exe"

        // "%FSC%" %fsc_flags% --quotations-debug+ --optimize -o:test--optimize.exe -g test.fsx
        do! fsc "%s --quotations-debug+ --optimize -o:test--optimize.exe -g" cfg.fsc_flags ["test.fsx"]

        // "%PEVERIFY%" test--optimize.exe 
        do! peverify "test--optimize.exe"

        }

    let run cfg dir = attempt {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsi = Printf.ksprintf (Commands.fsi exec cfg.FSI)
        let fileguard = (Commands.getfullpath dir) >> FileGuard.create

        // REM fsi.exe testing
        // echo TestC
        log "TestC"

        // if exist test.ok (del /f /q test.ok)
        use testOkFile = fileguard "test.ok"
        // "%FSI%" %fsi_flags% --quotations-debug+ test.fsx
        do! fsi "%s --quotations-debug+" cfg.fsi_flags ["test.fsx"]
        // if NOT EXIST test.ok goto SetError
        do! testOkFile |> NUnitConf.checkGuardExists

        // REM fsc.exe testing
        // echo TestD
        log "TestD"

        // if exist test.ok (del /f /q test.ok)
        use testOkFile2 = fileguard "test.ok"
        // %CLIX% test.exe
        do! exec ("."/"test.exe") ""
        // if NOT EXIST test.ok goto SetError
        do! testOkFile2 |> NUnitConf.checkGuardExists


        // if exist test.ok (del /f /q test.ok)
        use testOkFile3 = fileguard "test.ok"
        // %CLIX% test--optimize.exe
        do! exec ("."/"test--optimize.exe") ""
        // if NOT EXIST test.ok goto SetError
        do! testOkFile3 |> NUnitConf.checkGuardExists

        }

    [<Test; FSharpSuiteTest("core/quotesDebugInfo")>]
    let quotesDebugInfo () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        do! build cfg dir

        do! run cfg dir
                
        })



module QuotesInMultipleModules = 

    let build cfg dir = attempt {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)
        let peverify = Commands.peverify exec cfg.PEVERIFY "/nologo"

        // "%FSC%" %fsc_flags% -o:module1.dll --target:library module1.fsx
        do! fsc "%s -o:module1.dll --target:library" cfg.fsc_flags ["module1.fsx"]

        // "%PEVERIFY%" module1.dll 
        do! peverify "module1.dll"

        // "%FSC%" %fsc_flags% -o:module2.exe -r:module1.dll module2.fsx
        do! fsc "%s -o:module2.exe -r:module1.dll" cfg.fsc_flags ["module2.fsx"]

        // "%PEVERIFY%" module2.exe 
        do! peverify "module2.exe"
    
        // "%FSC%" %fsc_flags% --staticlink:module1 -o:module2-staticlink.exe -r:module1.dll module2.fsx
        do! fsc "%s --staticlink:module1 -o:module2-staticlink.exe -r:module1.dll" cfg.fsc_flags ["module2.fsx"]

        // "%PEVERIFY%" module2-staticlink.exe
        do! peverify "module2-staticlink.exe"

        // "%FSC%" %fsc_flags% -o:module1-opt.dll --target:library --optimize module1.fsx
        do! fsc "%s -o:module1-opt.dll --target:library --optimize" cfg.fsc_flags ["module1.fsx"]

        // "%PEVERIFY%" module1-opt.dll 
        do! peverify "module1-opt.dll"

        // "%FSC%" %fsc_flags% -o:module2-opt.exe -r:module1-opt.dll --optimize module2.fsx
        do! fsc "%s -o:module2-opt.exe -r:module1-opt.dll --optimize" cfg.fsc_flags ["module2.fsx"]

        // "%PEVERIFY%" module2-opt.exe 
        do! peverify "module2-opt.exe"

        }

    let run cfg dir = attempt {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsi = Printf.ksprintf (Commands.fsi exec cfg.FSI)
        let fileguard = (Commands.getfullpath dir) >> FileGuard.create

        // REM fsi.exe testing
        // echo TestC
        log "TestC"

        // if exist test.ok (del /f /q test.ok)
        use testOkFile = fileguard "test.ok"
        // "%FSI%" %fsi_flags% -r module1.dll module2.fsx
        do! fsi "%s -r module1.dll" cfg.fsi_flags ["module2.fsx"]
        // if NOT EXIST test.ok goto SetError
        do! testOkFile |> NUnitConf.checkGuardExists

        // REM fsc.exe testing
        // echo TestD
        log "TestD"

        // if exist test.ok (del /f /q test.ok)
        use testOkFile = fileguard "test.ok"
        // %CLIX% module2.exe
        do! exec ("."/"module2.exe") ""
        // if NOT EXIST test.ok goto SetError
        do! testOkFile |> NUnitConf.checkGuardExists

        // if exist test.ok (del /f /q test.ok)
        use testOkFile = fileguard "test.ok"
        // %CLIX% module2-opt.exe
        do! exec ("."/"module2-opt.exe") ""
        // if NOT EXIST test.ok goto SetError
        do! testOkFile |> NUnitConf.checkGuardExists

        // if exist test.ok (del /f /q test.ok)
        use testOkFile = fileguard "test.ok"
        // %CLIX% module2-staticlink.exe
        do! exec ("."/"module2-staticlink.exe") ""
        // if NOT EXIST test.ok goto SetError
        do! testOkFile |> NUnitConf.checkGuardExists

        }

    [<Test; FSharpSuiteTest("core/quotesInMultipleModules")>]
    let quotesInMultipleModules () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        do! build cfg dir

        do! run cfg dir
                
        })




module Reflect = 

    [<Test; FSharpSuiteScriptPermutations("core/reflect")>]
    let reflect p = singleTestBuildAndRun p


module ``test resources`` = 

    let build cfg dir = attempt {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)
        let peverify = Commands.peverify exec cfg.PEVERIFY "/nologo"
        let resgen = Printf.ksprintf (Commands.resgen exec cfg.RESGEN)

        // REM Note that you have a VS SDK dependence here.
        // "%RESGEN%" /compile Resources.resx
        do! resgen "/compile" ["Resources.resx"]

        // "%FSC%" %fsc_flags%  --resource:Resources.resources -o:test-embed.exe -g test.fs
        do! fsc "%s  --resource:Resources.resources -o:test-embed.exe -g" cfg.fsc_flags ["test.fs"]

        // "%PEVERIFY%" test-embed.exe 
        do! peverify "test-embed.exe"

        // "%FSC%" %fsc_flags%  --linkresource:Resources.resources -o:test-link.exe -g test.fs      
        do! fsc "%s  --linkresource:Resources.resources -o:test-link.exe -g" cfg.fsc_flags ["test.fs"]

        // "%PEVERIFY%" test-link.exe
        do! peverify "test-link.exe"

        // "%FSC%" %fsc_flags%  --resource:Resources.resources,ResourceName.resources -o:test-embed-named.exe -g test.fs      
        do! fsc "%s  --resource:Resources.resources,ResourceName.resources -o:test-embed-named.exe -g" cfg.fsc_flags ["test.fs"]

        // "%PEVERIFY%" test-embed-named.exe
        do! peverify "test-embed-named.exe"

        // "%FSC%" %fsc_flags%  --linkresource:Resources.resources,ResourceName.resources -o:test-link-named.exe -g test.fs      
        do! fsc "%s  --linkresource:Resources.resources,ResourceName.resources -o:test-link-named.exe -g" cfg.fsc_flags ["test.fs"]

        // "%PEVERIFY%" test-link-named.exe
        do! peverify "test-link-named.exe"

        }

    let run cfg dir = attempt {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult

        // %CLIX% .\test-embed.exe
        do! exec ("."/"test-embed.exe") ""

        // %CLIX% .\test-link.exe
        do! exec ("."/"test-link.exe") ""

        // %CLIX% .\test-link-named.exe ResourceName
        do! exec ("."/"test-link-named.exe") "ResourceName"

        // %CLIX% .\test-embed-named.exe ResourceName
        do! exec ("."/"test-embed-named.exe") "ResourceName"

        }

    [<Test; FSharpSuiteTest("core/resources")>]
    let resources () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        do! build cfg dir

        do! run cfg dir
                
        })


module ``test seq`` = 

    [<Test; FSharpSuiteScriptPermutations("core/seq")>]
    let seq p = singleTestBuildAndRun p


module Subtype = 

    [<Test; FSharpSuiteScriptPermutations("core/subtype")>]
    let subtype p = singleTestBuildAndRun p


module Syntax = 

    [<Test; FSharpSuiteScriptPermutations("core/syntax")>]
    let syntax p = singleTestBuildAndRun p



module Tlr = 

    [<Test; FSharpSuiteScriptPermutations("core/tlr")>]
    let tlr p = singleTestBuildAndRun p


module Topinit = 

    let build cfg dir = attempt {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)
        let peverify = Commands.peverify exec cfg.PEVERIFY "/nologo"
        let csc = Printf.ksprintf (Commands.csc exec cfg.CSC)
        let fsc_flags = cfg.fsc_flags


        // "%FSC%" %fsc_flags% --optimize -o both69514.exe -g lib69514.fs app69514.fs
        do! fsc "%s --optimize -o both69514.exe -g" fsc_flags ["lib69514.fs"; "app69514.fs"]

        // "%PEVERIFY%" both69514.exe
        do! peverify "both69514.exe"

        // "%FSC%" %fsc_flags% --optimize- -o both69514-noopt.exe -g lib69514.fs app69514.fs
        do! fsc "%s --optimize- -o both69514-noopt.exe -g" fsc_flags ["lib69514.fs"; "app69514.fs"]

        // "%PEVERIFY%" both69514-noopt.exe
        do! peverify "both69514-noopt.exe"


        // "%FSC%" %fsc_flags% --optimize -a -g lib69514.fs
        do! fsc "%s --optimize -a -g" fsc_flags ["lib69514.fs"]

        // "%PEVERIFY%" lib69514.dll
        do! peverify "lib69514.dll"

        // "%FSC%" %fsc_flags% --optimize -r:lib69514.dll -g app69514.fs
        do! fsc "%s --optimize -r:lib69514.dll -g" fsc_flags ["app69514.fs"]

        // "%PEVERIFY%" app69514.exe
        do! peverify "app69514.exe"

        // "%FSC%" %fsc_flags% --optimize- -o:lib69514-noopt.dll -a -g lib69514.fs
        do! fsc "%s --optimize- -o:lib69514-noopt.dll -a -g" fsc_flags ["lib69514.fs"]

        // "%PEVERIFY%" lib69514-noopt.dll
        do! peverify "lib69514-noopt.dll"

        // "%FSC%" %fsc_flags% --optimize- -r:lib69514-noopt.dll -o:app69514-noopt.exe -g app69514.fs
        do! fsc "%s --optimize- -r:lib69514-noopt.dll -o:app69514-noopt.exe -g" fsc_flags ["app69514.fs"]

        // "%PEVERIFY%" app69514-noopt.exe
        do! peverify "app69514-noopt.exe"


        // "%FSC%" %fsc_flags% --optimize- -o:lib69514-noopt-withsig.dll -a -g lib69514.fsi lib69514.fs
        do! fsc "%s --optimize- -o:lib69514-noopt-withsig.dll -a -g" fsc_flags ["lib69514.fsi"; "lib69514.fs"]

        // "%PEVERIFY%" lib69514-noopt-withsig.dll
        do! peverify "lib69514-noopt-withsig.dll"

        // "%FSC%" %fsc_flags% --optimize- -r:lib69514-noopt-withsig.dll -o:app69514-noopt-withsig.exe -g app69514.fs
        do! fsc "%s --optimize- -r:lib69514-noopt-withsig.dll -o:app69514-noopt-withsig.exe -g" fsc_flags ["app69514.fs"]

        // "%PEVERIFY%" app69514-noopt-withsig.exe
        do! peverify "app69514-noopt-withsig.exe"

        // "%FSC%" %fsc_flags% -o:lib69514-withsig.dll -a -g lib69514.fsi lib69514.fs
        do! fsc "%s -o:lib69514-withsig.dll -a -g" fsc_flags ["lib69514.fsi"; "lib69514.fs"]

        // "%PEVERIFY%" lib69514-withsig.dll
        do! peverify "lib69514-withsig.dll"

        // "%FSC%" %fsc_flags% -r:lib69514-withsig.dll -o:app69514-withsig.exe -g app69514.fs
        do! fsc "%s -r:lib69514-withsig.dll -o:app69514-withsig.exe -g" fsc_flags ["app69514.fs"]

        // "%PEVERIFY%" app69514-withsig.exe
        do! peverify "app69514-withsig.exe"


        // "%FSC%" %fsc_flags% -o:lib.dll -a -g lib.ml
        do! fsc "%s -o:lib.dll -a -g" fsc_flags ["lib.ml"]

        // "%PEVERIFY%" lib.dll
        do! peverify "lib.dll"

        // %CSC% /nologo /r:"%FSCOREDLLPATH%" /r:lib.dll /out:test.exe test.cs 
        do! csc """/nologo /r:"%s" /r:lib.dll /out:test.exe """ cfg.FSCOREDLLPATH ["test.cs"]

        // "%FSC%" %fsc_flags% --optimize -o:lib--optimize.dll -a -g lib.ml
        do! fsc "%s --optimize -o:lib--optimize.dll -a -g" fsc_flags ["lib.ml"]

        // "%PEVERIFY%" lib--optimize.dll
        do! peverify "lib--optimize.dll"

        // %CSC% /nologo /r:"%FSCOREDLLPATH%" /r:lib--optimize.dll /out:test--optimize.exe test.cs 
        do! csc """/nologo /r:"%s" /r:lib--optimize.dll /out:test--optimize.exe""" cfg.FSCOREDLLPATH ["test.cs"]

        // set dicases= flag_deterministic_init1.fs lib_deterministic_init1.fs flag_deterministic_init2.fs lib_deterministic_init2.fs flag_deterministic_init3.fs lib_deterministic_init3.fs flag_deterministic_init4.fs lib_deterministic_init4.fs flag_deterministic_init5.fs lib_deterministic_init5.fs flag_deterministic_init6.fs lib_deterministic_init6.fs flag_deterministic_init7.fs lib_deterministic_init7.fs flag_deterministic_init8.fs lib_deterministic_init8.fs flag_deterministic_init9.fs lib_deterministic_init9.fs flag_deterministic_init10.fs lib_deterministic_init10.fs flag_deterministic_init11.fs lib_deterministic_init11.fs flag_deterministic_init12.fs lib_deterministic_init12.fs flag_deterministic_init13.fs lib_deterministic_init13.fs flag_deterministic_init14.fs lib_deterministic_init14.fs flag_deterministic_init15.fs lib_deterministic_init15.fs flag_deterministic_init16.fs lib_deterministic_init16.fs flag_deterministic_init17.fs lib_deterministic_init17.fs flag_deterministic_init18.fs lib_deterministic_init18.fs flag_deterministic_init19.fs lib_deterministic_init19.fs flag_deterministic_init20.fs lib_deterministic_init20.fs flag_deterministic_init21.fs lib_deterministic_init21.fs flag_deterministic_init22.fs lib_deterministic_init22.fs flag_deterministic_init23.fs lib_deterministic_init23.fs flag_deterministic_init24.fs lib_deterministic_init24.fs flag_deterministic_init25.fs lib_deterministic_init25.fs flag_deterministic_init26.fs lib_deterministic_init26.fs flag_deterministic_init27.fs lib_deterministic_init27.fs flag_deterministic_init28.fs lib_deterministic_init28.fs flag_deterministic_init29.fs lib_deterministic_init29.fs flag_deterministic_init30.fs lib_deterministic_init30.fs flag_deterministic_init31.fs lib_deterministic_init31.fs flag_deterministic_init32.fs lib_deterministic_init32.fs flag_deterministic_init33.fs lib_deterministic_init33.fs flag_deterministic_init34.fs lib_deterministic_init34.fs flag_deterministic_init35.fs lib_deterministic_init35.fs flag_deterministic_init36.fs lib_deterministic_init36.fs flag_deterministic_init37.fs lib_deterministic_init37.fs flag_deterministic_init38.fs lib_deterministic_init38.fs flag_deterministic_init39.fs lib_deterministic_init39.fs flag_deterministic_init40.fs lib_deterministic_init40.fs flag_deterministic_init41.fs lib_deterministic_init41.fs flag_deterministic_init42.fs lib_deterministic_init42.fs flag_deterministic_init43.fs lib_deterministic_init43.fs flag_deterministic_init44.fs lib_deterministic_init44.fs flag_deterministic_init45.fs lib_deterministic_init45.fs flag_deterministic_init46.fs lib_deterministic_init46.fs flag_deterministic_init47.fs lib_deterministic_init47.fs flag_deterministic_init48.fs lib_deterministic_init48.fs flag_deterministic_init49.fs lib_deterministic_init49.fs flag_deterministic_init50.fs lib_deterministic_init50.fs flag_deterministic_init51.fs lib_deterministic_init51.fs flag_deterministic_init52.fs lib_deterministic_init52.fs flag_deterministic_init53.fs lib_deterministic_init53.fs flag_deterministic_init54.fs lib_deterministic_init54.fs flag_deterministic_init55.fs lib_deterministic_init55.fs flag_deterministic_init56.fs lib_deterministic_init56.fs flag_deterministic_init57.fs lib_deterministic_init57.fs flag_deterministic_init58.fs lib_deterministic_init58.fs flag_deterministic_init59.fs lib_deterministic_init59.fs flag_deterministic_init60.fs lib_deterministic_init60.fs flag_deterministic_init61.fs lib_deterministic_init61.fs flag_deterministic_init62.fs lib_deterministic_init62.fs flag_deterministic_init63.fs lib_deterministic_init63.fs flag_deterministic_init64.fs lib_deterministic_init64.fs flag_deterministic_init65.fs lib_deterministic_init65.fs flag_deterministic_init66.fs lib_deterministic_init66.fs flag_deterministic_init67.fs lib_deterministic_init67.fs flag_deterministic_init68.fs lib_deterministic_init68.fs flag_deterministic_init69.fs lib_deterministic_init69.fs flag_deterministic_init70.fs lib_deterministic_init70.fs flag_deterministic_init71.fs lib_deterministic_init71.fs flag_deterministic_init72.fs lib_deterministic_init72.fs flag_deterministic_init73.fs lib_deterministic_init73.fs flag_deterministic_init74.fs lib_deterministic_init74.fs flag_deterministic_init75.fs lib_deterministic_init75.fs flag_deterministic_init76.fs lib_deterministic_init76.fs flag_deterministic_init77.fs lib_deterministic_init77.fs flag_deterministic_init78.fs lib_deterministic_init78.fs flag_deterministic_init79.fs lib_deterministic_init79.fs flag_deterministic_init80.fs lib_deterministic_init80.fs flag_deterministic_init81.fs lib_deterministic_init81.fs flag_deterministic_init82.fs lib_deterministic_init82.fs flag_deterministic_init83.fs lib_deterministic_init83.fs flag_deterministic_init84.fs lib_deterministic_init84.fs flag_deterministic_init85.fs lib_deterministic_init85.fs
        let dicases = ["flag_deterministic_init1.fs"; "lib_deterministic_init1.fs"; "flag_deterministic_init2.fs"; "lib_deterministic_init2.fs"; "flag_deterministic_init3.fs"; "lib_deterministic_init3.fs"; "flag_deterministic_init4.fs"; "lib_deterministic_init4.fs"; "flag_deterministic_init5.fs"; "lib_deterministic_init5.fs"; "flag_deterministic_init6.fs"; "lib_deterministic_init6.fs"; "flag_deterministic_init7.fs"; "lib_deterministic_init7.fs"; "flag_deterministic_init8.fs"; "lib_deterministic_init8.fs"; "flag_deterministic_init9.fs"; "lib_deterministic_init9.fs"; "flag_deterministic_init10.fs"; "lib_deterministic_init10.fs"; "flag_deterministic_init11.fs"; "lib_deterministic_init11.fs"; "flag_deterministic_init12.fs"; "lib_deterministic_init12.fs"; "flag_deterministic_init13.fs"; "lib_deterministic_init13.fs"; "flag_deterministic_init14.fs"; "lib_deterministic_init14.fs"; "flag_deterministic_init15.fs"; "lib_deterministic_init15.fs"; "flag_deterministic_init16.fs"; "lib_deterministic_init16.fs"; "flag_deterministic_init17.fs"; "lib_deterministic_init17.fs"; "flag_deterministic_init18.fs"; "lib_deterministic_init18.fs"; "flag_deterministic_init19.fs"; "lib_deterministic_init19.fs"; "flag_deterministic_init20.fs"; "lib_deterministic_init20.fs"; "flag_deterministic_init21.fs"; "lib_deterministic_init21.fs"; "flag_deterministic_init22.fs"; "lib_deterministic_init22.fs"; "flag_deterministic_init23.fs"; "lib_deterministic_init23.fs"; "flag_deterministic_init24.fs"; "lib_deterministic_init24.fs"; "flag_deterministic_init25.fs"; "lib_deterministic_init25.fs"; "flag_deterministic_init26.fs"; "lib_deterministic_init26.fs"; "flag_deterministic_init27.fs"; "lib_deterministic_init27.fs"; "flag_deterministic_init28.fs"; "lib_deterministic_init28.fs"; "flag_deterministic_init29.fs"; "lib_deterministic_init29.fs"; "flag_deterministic_init30.fs"; "lib_deterministic_init30.fs"; "flag_deterministic_init31.fs"; "lib_deterministic_init31.fs"; "flag_deterministic_init32.fs"; "lib_deterministic_init32.fs"; "flag_deterministic_init33.fs"; "lib_deterministic_init33.fs"; "flag_deterministic_init34.fs"; "lib_deterministic_init34.fs"; "flag_deterministic_init35.fs"; "lib_deterministic_init35.fs"; "flag_deterministic_init36.fs"; "lib_deterministic_init36.fs"; "flag_deterministic_init37.fs"; "lib_deterministic_init37.fs"; "flag_deterministic_init38.fs"; "lib_deterministic_init38.fs"; "flag_deterministic_init39.fs"; "lib_deterministic_init39.fs"; "flag_deterministic_init40.fs"; "lib_deterministic_init40.fs"; "flag_deterministic_init41.fs"; "lib_deterministic_init41.fs"; "flag_deterministic_init42.fs"; "lib_deterministic_init42.fs"; "flag_deterministic_init43.fs"; "lib_deterministic_init43.fs"; "flag_deterministic_init44.fs"; "lib_deterministic_init44.fs"; "flag_deterministic_init45.fs"; "lib_deterministic_init45.fs"; "flag_deterministic_init46.fs"; "lib_deterministic_init46.fs"; "flag_deterministic_init47.fs"; "lib_deterministic_init47.fs"; "flag_deterministic_init48.fs"; "lib_deterministic_init48.fs"; "flag_deterministic_init49.fs"; "lib_deterministic_init49.fs"; "flag_deterministic_init50.fs"; "lib_deterministic_init50.fs"; "flag_deterministic_init51.fs"; "lib_deterministic_init51.fs"; "flag_deterministic_init52.fs"; "lib_deterministic_init52.fs"; "flag_deterministic_init53.fs"; "lib_deterministic_init53.fs"; "flag_deterministic_init54.fs"; "lib_deterministic_init54.fs"; "flag_deterministic_init55.fs"; "lib_deterministic_init55.fs"; "flag_deterministic_init56.fs"; "lib_deterministic_init56.fs"; "flag_deterministic_init57.fs"; "lib_deterministic_init57.fs"; "flag_deterministic_init58.fs"; "lib_deterministic_init58.fs"; "flag_deterministic_init59.fs"; "lib_deterministic_init59.fs"; "flag_deterministic_init60.fs"; "lib_deterministic_init60.fs"; "flag_deterministic_init61.fs"; "lib_deterministic_init61.fs"; "flag_deterministic_init62.fs"; "lib_deterministic_init62.fs"; "flag_deterministic_init63.fs"; "lib_deterministic_init63.fs"; "flag_deterministic_init64.fs"; "lib_deterministic_init64.fs"; "flag_deterministic_init65.fs"; "lib_deterministic_init65.fs"; "flag_deterministic_init66.fs"; "lib_deterministic_init66.fs"; "flag_deterministic_init67.fs"; "lib_deterministic_init67.fs"; "flag_deterministic_init68.fs"; "lib_deterministic_init68.fs"; "flag_deterministic_init69.fs"; "lib_deterministic_init69.fs"; "flag_deterministic_init70.fs"; "lib_deterministic_init70.fs"; "flag_deterministic_init71.fs"; "lib_deterministic_init71.fs"; "flag_deterministic_init72.fs"; "lib_deterministic_init72.fs"; "flag_deterministic_init73.fs"; "lib_deterministic_init73.fs"; "flag_deterministic_init74.fs"; "lib_deterministic_init74.fs"; "flag_deterministic_init75.fs"; "lib_deterministic_init75.fs"; "flag_deterministic_init76.fs"; "lib_deterministic_init76.fs"; "flag_deterministic_init77.fs"; "lib_deterministic_init77.fs"; "flag_deterministic_init78.fs"; "lib_deterministic_init78.fs"; "flag_deterministic_init79.fs"; "lib_deterministic_init79.fs"; "flag_deterministic_init80.fs"; "lib_deterministic_init80.fs"; "flag_deterministic_init81.fs"; "lib_deterministic_init81.fs"; "flag_deterministic_init82.fs"; "lib_deterministic_init82.fs"; "flag_deterministic_init83.fs"; "lib_deterministic_init83.fs"; "flag_deterministic_init84.fs"; "lib_deterministic_init84.fs"; "flag_deterministic_init85.fs"; "lib_deterministic_init85.fs"] 

        // "%FSC%" %fsc_flags% --optimize- -o test_deterministic_init.exe %dicases% test_deterministic_init.fs
        do! fsc "%s --optimize- -o test_deterministic_init.exe" fsc_flags (dicases @ ["test_deterministic_init.fs"])

        // "%PEVERIFY%" test_deterministic_init.exe
        do! peverify "test_deterministic_init.exe"

        // "%FSC%" %fsc_flags% --optimize -o test_deterministic_init--optimize.exe %dicases% test_deterministic_init.fs
        do! fsc "%s --optimize -o test_deterministic_init--optimize.exe" fsc_flags (dicases @ ["test_deterministic_init.fs"])

        // "%PEVERIFY%" test_deterministic_init--optimize.exe
        do! peverify "test_deterministic_init--optimize.exe"


        // "%FSC%" %fsc_flags% --optimize- -a -o test_deterministic_init_lib.dll %dicases% 
        do! fsc "%s --optimize- -a -o test_deterministic_init_lib.dll" fsc_flags dicases

        // "%PEVERIFY%" test_deterministic_init_lib.dll
        do! peverify "test_deterministic_init_lib.dll"

        // "%FSC%" %fsc_flags% --optimize- -r test_deterministic_init_lib.dll -o test_deterministic_init_exe.exe test_deterministic_init.fs
        do! fsc "%s --optimize- -r test_deterministic_init_lib.dll -o test_deterministic_init_exe.exe" fsc_flags ["test_deterministic_init.fs"]

        // "%PEVERIFY%" test_deterministic_init_exe.exe
        do! peverify "test_deterministic_init_exe.exe"

        // "%FSC%" %fsc_flags% --optimize -a -o test_deterministic_init_lib--optimize.dll %dicases% 
        do! fsc "%s --optimize -a -o test_deterministic_init_lib--optimize.dll" fsc_flags dicases

        // "%PEVERIFY%" test_deterministic_init_lib--optimize.dll
        do! peverify "test_deterministic_init_lib--optimize.dll"

        // "%FSC%" %fsc_flags% --optimize -r test_deterministic_init_lib--optimize.dll -o test_deterministic_init_exe--optimize.exe test_deterministic_init.fs
        do! fsc "%s --optimize -r test_deterministic_init_lib--optimize.dll -o test_deterministic_init_exe--optimize.exe" fsc_flags ["test_deterministic_init.fs"]

        // "%PEVERIFY%" test_deterministic_init_exe--optimize.exe
        do! peverify "test_deterministic_init_exe--optimize.exe"


        // set static_init_cases= test0.fs test1.fs test2.fs test3.fs test4.fs test5.fs test6.fs
        let static_init_cases = [ "test0.fs"; "test1.fs"; "test2.fs"; "test3.fs"; "test4.fs"; "test5.fs"; "test6.fs" ]

        // "%FSC%" %fsc_flags% --optimize- -o test_static_init.exe %static_init_cases% static-main.fs
        do! fsc "%s --optimize- -o test_static_init.exe" fsc_flags (static_init_cases @ ["static-main.fs"])

        // "%PEVERIFY%" test_static_init.exe
        do! peverify "test_static_init.exe"

        // "%FSC%" %fsc_flags% --optimize -o test_static_init--optimize.exe %static_init_cases% static-main.fs
        do! fsc "%s --optimize -o test_static_init--optimize.exe" fsc_flags (static_init_cases @ [ "static-main.fs" ])

        // "%PEVERIFY%" test_static_init--optimize.exe
        do! peverify "test_static_init--optimize.exe"


        // "%FSC%" %fsc_flags% --optimize- -a -o test_static_init_lib.dll %static_init_cases% 
        do! fsc "%s --optimize- -a -o test_static_init_lib.dll" fsc_flags static_init_cases

        // "%PEVERIFY%" test_static_init_lib.dll
        do! peverify "test_static_init_lib.dll"

        // "%FSC%" %fsc_flags% --optimize- -r test_static_init_lib.dll -o test_static_init_exe.exe static-main.fs
        do! fsc "%s --optimize- -r test_static_init_lib.dll -o test_static_init_exe.exe" fsc_flags ["static-main.fs"]

        // "%PEVERIFY%" test_static_init_exe.exe
        do! peverify "test_static_init_exe.exe"

        // "%FSC%" %fsc_flags% --optimize -a -o test_static_init_lib--optimize.dll %static_init_cases% 
        do! fsc "%s --optimize -a -o test_static_init_lib--optimize.dll" fsc_flags static_init_cases

        // "%PEVERIFY%" test_static_init_lib--optimize.dll
        do! peverify "test_static_init_lib--optimize.dll"

        // "%FSC%" %fsc_flags% --optimize -r test_static_init_lib--optimize.dll -o test_static_init_exe--optimize.exe static-main.fs
        do! fsc "%s --optimize -r test_static_init_lib--optimize.dll -o test_static_init_exe--optimize.exe" fsc_flags ["static-main.fs"]

        // "%PEVERIFY%" test_static_init_exe--optimize.exe
        do! peverify "test_static_init_exe--optimize.exe"

        }

    let run cfg dir = attempt {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult

        // %CLIX% .\test.exe
        do! exec ("."/"test.exe") ""

        // %CLIX% .\test--optimize.exe
        do! exec ("."/"test--optimize.exe") ""

        // %CLIX% .\test_deterministic_init.exe
        do! exec ("."/"test_deterministic_init.exe") ""

        // %CLIX% .\test_deterministic_init--optimize.exe
        do! exec ("."/"test_deterministic_init--optimize.exe") ""

        // %CLIX% .\test_deterministic_init_exe.exe
        do! exec ("."/"test_deterministic_init_exe.exe") ""

        // %CLIX% .\test_deterministic_init_exe--optimize.exe
        do! exec ("."/"test_deterministic_init_exe--optimize.exe") ""


        // %CLIX% .\test_static_init.exe
        do! exec ("."/"test_static_init.exe") ""

        // %CLIX% .\test_static_init--optimize.exe
        do! exec ("."/"test_static_init--optimize.exe") ""

        // %CLIX% .\test_static_init_exe.exe
        do! exec ("."/"test_static_init_exe.exe") ""

        // %CLIX% .\test_static_init_exe--optimize.exe
        do! exec ("."/"test_static_init_exe--optimize.exe") ""


        }

    [<Test; FSharpSuiteTest("core/topinit")>]
    let topinit () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        do! build cfg dir

        do! run cfg dir
                
        })


module UnitsOfMeasure = 

    let build cfg dir = attempt {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)
        let peverify = Commands.peverify exec cfg.PEVERIFY "/nologo"

        // "%FSC%" %fsc_flags% --optimize- -o:test.exe -g test.fs
        do! fsc "%s --optimize- -o:test.exe -g" cfg.fsc_flags ["test.fs"]

        // "%PEVERIFY%" test.exe
        do! peverify "test.exe"

        }

    let run cfg dir = attempt {

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fileguard = (Commands.getfullpath dir) >> FileGuard.create

        // if exist test.ok (del /f /q test.ok)
        use testOkFile = fileguard "test.ok"

        // %CLIX% .\test.exe
        do! exec ("."/"test.exe") ""

        do! testOkFile |> NUnitConf.checkGuardExists
        }

    [<Test; FSharpSuiteTest("core/unitsOfMeasure")>]
    let unitsOfMeasure () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        do! build cfg dir

        do! run cfg dir
                
        })



module Verify = 

    [<Test; FSharpSuiteTest("core/verify")>]
    let verify () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        let exec p = Command.exec dir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
        let fsc = Printf.ksprintf (Commands.fsc exec cfg.FSC)
        let peverify = Commands.peverify exec cfg.PEVERIFY "/nologo"
        let peverify' = Commands.peverify exec cfg.PEVERIFY
        let getfullpath = Commands.getfullpath dir

        // "%PEVERIFY%" "%FSCBinPath%\FSharp.Build.dll"
        do! peverify (cfg.FSCBinPath/"FSharp.Build.dll")

        // REM Use /MD because this contains some P/Invoke code  
        // "%PEVERIFY%" /MD "%FSCBinPath%\FSharp.Compiler.dll"
        do! peverify' "/MD" (cfg.FSCBinPath/"FSharp.Compiler.dll")

        // "%PEVERIFY%" "%FSCBinPath%\fsi.exe"
        do! peverify (cfg.FSCBinPath/"fsi.exe")

        // "%PEVERIFY%" "%FSCBinPath%\FSharp.Compiler.Interactive.Settings.dll"
        do! peverify (cfg.FSCBinPath/"FSharp.Compiler.Interactive.Settings.dll")

        // "%FSC%" %fsc_flags% -o:xmlverify.exe -g xmlverify.fs
        do! fsc "%s -o:xmlverify.exe -g" cfg.fsc_flags ["xmlverify.fs"]

        // "%PEVERIFY%" xmlverify.exe
        do! peverify "xmlverify.exe"
        })
