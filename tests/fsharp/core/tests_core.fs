module ``FSharp-Tests-Core``

open System
open System.IO
open NUnit.Framework

open NUnitConf
open PlatformHelpers
open FSharpTestSuiteTypes
open SingleTest

[<Test; FSharpSuiteScriptPermutations("core/access")>]
let access p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("core/apporder")>]
let apporder p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("core/array")>]
let array p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("core/attributes")>]
let attributes p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("core/byrefs")>]
let byrefs _ = check  (attempt {

    let cfg = FSharpTestSuite.testConfig ()

    use testOkFile = fileguard cfg "test.ok"

    do! fsc cfg "%s -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

    do! exec cfg ("."/"test.exe") ""

    do! testOkFile |> NUnitConf.checkGuardExists

    do! fsi cfg "" ["test.fsx"]

    do! testOkFile |> NUnitConf.checkGuardExists

    })

[<Test; FSharpSuiteScriptPermutations("core/comprehensions")>]
let comprehensions p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("core/comprehensions-hw")>]
let comprehensionshw p = singleTestBuildAndRun p

[<Test; FSharpSuiteFscCodePermutation("core/control")>]
let control p = singleTestBuildAndRun p

[<Test; FSharpSuiteFscCodePermutation("core/control")>]
let ``control --tailcalls`` p = check  (attempt {
    let cfg = FSharpTestSuite.testConfig ()
        
    do! singleTestBuild cfg p
        
    do! singleTestRun {cfg with fsi_flags = " --tailcalls" } p
    })

[<Test; FSharpSuiteFscCodePermutation("core/controlChamenos")>]
let controlChamenos p = check  (attempt {
    let cfg = FSharpTestSuite.testConfig ()
        
    do! singleTestBuild cfg p
        
    do! singleTestRun { cfg with fsi_flags = " --tailcalls" } p
    })

[<Test; FSharpSuiteFscCodePermutation("core/controlMailbox")>]
let controlMailbox p = singleTestBuildAndRun p

[<Test; FSharpSuiteFscCodePermutation("core/controlMailbox")>]
let ``controlMailbox --tailcalls`` p = check  (attempt {
    let cfg = FSharpTestSuite.testConfig ()
        
    do! singleTestBuild cfg p
        
    do! singleTestRun { cfg with fsi_flags = " --tailcalls" } p
    })

[<Test; FSharpSuiteFscCodePermutation("core/controlwpf")>]
let controlWpf p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("core/csext")>]
let csext p = singleTestBuildAndRun p


[<Test; FSharpSuiteTest("core/events")>]
let events () = check  (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    // "%FSC%" %fsc_flags% -a -o:test.dll -g test.fs
    do! fsc cfg "%s -a -o:test.dll -g" cfg.fsc_flags ["test.fs"]

    do! peverify cfg "test.dll"

    // %CSC% /r:"%FSCOREDLLPATH%" /reference:test.dll /debug+ testcs.cs
    do! csc cfg """/r:"%s" /reference:test.dll /debug+""" cfg.FSCOREDLLPATH ["testcs.cs"]

    do! peverify cfg "testcs.exe"
        
    use testOkFile = fileguard cfg "test.ok"

    // "%FSI%" test.fs && (
    do! fsi cfg "" ["test.fs"]

    do! testOkFile |> NUnitConf.checkGuardExists

    // .\testcs.exe
    do! exec cfg ("."/"testcs.exe") ""
    })

//
// Shadowcopy does not work for public signed assemblies
// =====================================================
//
//module ``FSI-Shadowcopy`` = 
//
//    [<Test>]
//    // "%FSI%" %fsi_flags%                          < test1.fsx
//    [<FSharpSuiteTestCase("core/fsi-shadowcopy", "")>]
//    // "%FSI%" %fsi_flags%  --shadowcopyreferences- < test1.fsx
//    [<FSharpSuiteTestCase("core/fsi-shadowcopy", "--shadowcopyreferences-")>]
//    let ``shadowcopy disabled`` (flags: string) = check  (attempt {
//        let cfg = FSharpTestSuite.testConfig ()
//
//
//
//
//
//        // if exist test1.ok (del /f /q test1.ok)
//        use testOkFile = fileguard cfg "test1.ok"
//
//        do! ``fsi <`` cfg "%s %s" cfg.fsi_flags flags "test1.fsx"
//
//        // if NOT EXIST test1.ok goto SetError
//        do! testOkFile |> NUnitConf.checkGuardExists
//        })
//
//    [<Test>]
//    // "%FSI%" %fsi_flags%  /shadowcopyreferences+  < test2.fsx
//    [<FSharpSuiteTestCase("core/fsi-shadowcopy", "/shadowcopyreferences+")>]
//    // "%FSI%" %fsi_flags%  --shadowcopyreferences  < test2.fsx
//    [<FSharpSuiteTestCase("core/fsi-shadowcopy", "--shadowcopyreferences")>]
//    let ``shadowcopy enabled`` (flags: string) = check (attempt {
//        let cfg = FSharpTestSuite.testConfig ()
//
//
//
//
//
//        // if exist test2.ok (del /f /q test2.ok)
//        use testOkFile = fileguard cfg "test2.ok"
//
//        // "%FSI%" %fsi_flags%  /shadowcopyreferences+  < test2.fsx
//        do! ``fsi <`` cfg "%s %s" cfg.fsi_flags flags "test2.fsx"
//
//        // if NOT EXIST test2.ok goto SetError
//        do! testOkFile |> NUnitConf.checkGuardExists
//        })

    

[<Test; FSharpSuiteTest("core/forwarders")>]
let forwarders () = check (attempt {
        let cfg = FSharpTestSuite.testConfig ()


        let copy_y f = Commands.copy_y cfg.Directory f >> checkResult
        let mkdir = Commands.mkdir_p cfg.Directory

        // mkdir orig
        mkdir "orig"
        // mkdir split
        mkdir "split"

        // %CSC% /nologo  /target:library /out:orig\a.dll /define:PART1;PART2 a.cs
        do! csc cfg """/nologo  /target:library /out:orig\a.dll /define:PART1;PART2""" ["a.cs"]

        // %CSC% /nologo  /target:library /out:orig\b.dll /r:orig\a.dll b.cs 
        do! csc cfg """/nologo  /target:library /out:orig\b.dll /r:orig\a.dll""" ["b.cs"]

        // "%FSC%" -a -o:orig\c.dll -r:orig\b.dll -r:orig\a.dll c.fs
        do! fsc cfg """-a -o:orig\c.dll -r:orig\b.dll -r:orig\a.dll""" ["c.fs"]

        // %CSC% /nologo  /target:library /out:split\a-part1.dll /define:PART1;SPLIT a.cs  
        do! csc cfg """/nologo  /target:library /out:split\a-part1.dll /define:PART1;SPLIT""" ["a.cs"]

        // %CSC% /nologo  /target:library /r:split\a-part1.dll /out:split\a.dll /define:PART2;SPLIT a.cs
        do! csc cfg """/nologo  /target:library /r:split\a-part1.dll /out:split\a.dll /define:PART2;SPLIT""" ["a.cs"]

        // copy /y orig\b.dll split\b.dll
        do! copy_y ("orig"/"b.dll") ("split"/"b.dll")
        // copy /y orig\c.dll split\c.dll
        do! copy_y ("orig"/"c.dll") ("split"/"c.dll")

        // "%FSC%" -o:orig\test.exe -r:orig\b.dll -r:orig\a.dll test.fs
        do! fsc cfg """-o:orig\test.exe -r:orig\b.dll -r:orig\a.dll""" ["test.fs"]

        // "%FSC%" -o:split\test.exe -r:split\b.dll -r:split\a-part1.dll -r:split\a.dll test.fs
        do! fsc cfg """-o:split\test.exe -r:split\b.dll -r:split\a-part1.dll -r:split\a.dll""" ["test.fs"]

        // "%FSC%" -o:split\test-against-c.exe -r:split\c.dll -r:split\a-part1.dll -r:split\a.dll test.fs
        do! fsc cfg """-o:split\test-against-c.exe -r:split\c.dll -r:split\a-part1.dll -r:split\a.dll""" ["test.fs"]

        do! peverify cfg ("split"/"a-part1.dll")

        do! peverify cfg ("split"/"b.dll")

        do! peverify cfg ("split"/"c.dll")

        do! peverify cfg ("split"/"test.exe")

        do! peverify cfg ("split"/"test-against-c.exe")

        })

[<Test; FSharpSuiteTest("core/fsfromcs")>]
let fsfromcs () = check (attempt {
        let cfg = FSharpTestSuite.testConfig ()

        // "%FSC%" %fsc_flags% -a --doc:lib.xml -o:lib.dll -g lib.fs
        do! fsc cfg "%s -a --doc:lib.xml -o:lib.dll -g" cfg.fsc_flags ["lib.fs"]

        do! peverify cfg "lib.dll"

        // %CSC% /nologo /r:"%FSCOREDLLPATH%" /r:System.Core.dll /r:lib.dll /out:test.exe test.cs 
        do! csc cfg """/nologo /r:"%s" /r:System.Core.dll /r:lib.dll /out:test.exe""" cfg.FSCOREDLLPATH ["test.cs"]

        // "%FSC%" %fsc_flags% -a --doc:lib--optimize.xml -o:lib--optimize.dll -g lib.fs
        do! fsc cfg """%s -a --doc:lib--optimize.xml -o:lib--optimize.dll -g""" cfg.fsc_flags ["lib.fs"]

        do! peverify cfg "lib--optimize.dll"

        // %CSC% 
        do! csc cfg """/nologo /r:"%s"  /r:System.Core.dll /r:lib--optimize.dll    /out:test--optimize.exe""" cfg.FSCOREDLLPATH ["test.cs"]

        // .\test.exe
        do! exec cfg ("."/"test.exe") ""

        // .\test--optimize.exe
        do! exec cfg ("."/"test--optimize.exe") ""
                
        })

[<Test; FSharpSuiteTest("core/fsfromfsviacs")>]
let fsfromfsviacs () = check (attempt {
        let cfg = FSharpTestSuite.testConfig ()

        do! fsc cfg "%s -a -o:lib.dll -g" cfg.fsc_flags ["lib.fs"]

        do! peverify cfg "lib.dll"

        do! csc cfg """/nologo /target:library /r:"%s" /r:lib.dll /out:lib2.dll""" cfg.FSCOREDLLPATH ["lib2.cs"]

        do! csc cfg """/nologo /target:library /r:"%s" /out:lib3.dll""" cfg.FSCOREDLLPATH ["lib3.cs"]

        do! fsc cfg "%s -r:lib.dll -r:lib2.dll -r:lib3.dll -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

        do! peverify cfg "test.exe"

        // Same with library references the other way around
        do! fsc cfg "%s -r:lib.dll -r:lib3.dll -r:lib2.dll -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

        do! peverify cfg "test.exe"

        do! exec cfg ("."/"test.exe") ""
                
        })


[<Test; FSharpSuiteTest("core/fsi-reload")>]
let ``fsi-reload`` () = check (attempt {
        let cfg = FSharpTestSuite.testConfig ()

        /////// build.bat ///////

        // REM  NOTE that this test does not do anything.
        // REM  PEVERIFY not needed

        /////// run.bat  ////////

        do! attempt {

            use testOkFile = fileguard cfg "test.ok"
            // "%FSI%" %fsi_flags%  --maxerrors:1 < test1.ml
            do! ``fsi <`` cfg "%s  --maxerrors:1" cfg.fsi_flags "test1.ml"
    
            do! testOkFile |> NUnitConf.checkGuardExists
            }
                
        do! attempt {

            use testOkFile = fileguard cfg "test.ok"
            // "%FSI%" %fsi_flags%  --maxerrors:1 load1.fsx
            do! fsi cfg "%s  --maxerrors:1" cfg.fsi_flags ["load1.fsx"]
    
            do! testOkFile |> NUnitConf.checkGuardExists
            }

        do! attempt {

            use testOkFile = fileguard cfg "test.ok"
            // "%FSI%" %fsi_flags%  --maxerrors:1 load2.fsx
            do! fsi cfg "%s  --maxerrors:1" cfg.fsi_flags ["load2.fsx"]
    
            do! testOkFile |> NUnitConf.checkGuardExists
            }

        // "%FSC%" load1.fsx
        do! fsc cfg "" ["load1.fsx"]

        // "%FSC%" load2.fsx
        do! fsc cfg "" ["load2.fsx"]

        })


[<Test; FSharpSuiteTest("core/fsiAndModifiers")>]
let fsiAndModifiers () = check (attempt {
        let cfg = FSharpTestSuite.testConfig ()

        // if exist TestLibrary.dll (del /f /q TestLibrary.dll)
        do if fileExists cfg "TestLibrary.dll" then rm cfg "TestLibrary.dll"

        // "%FSI%" %fsi_flags%  --maxerrors:1 < prepare.fsx
        do! ``fsi <`` cfg "%s  --maxerrors:1" cfg.fsi_flags "prepare.fsx"

        use testOkFile = fileguard cfg "test.ok"
        
        // "%FSI%" %fsi_flags%  --maxerrors:1 < test.fsx
        do! ``fsi <`` cfg "%s  --maxerrors:1" cfg.fsi_flags "test.fsx"

        do! testOkFile |> NUnitConf.checkGuardExists
                
        })

[<Test; FSharpSuiteCodeAndSignaturePermutations("core/genericmeasures")>]
let genericmeasures p = singleTestBuildAndRun p

[<Test; FSharpSuiteTest("core/hiding")>]
let hiding () = check (attempt {
        let cfg = FSharpTestSuite.testConfig ()

        // "%FSC%" %fsc_flags% -a --optimize -o:lib.dll lib.mli lib.ml libv.ml
        do! fsc cfg "%s -a --optimize -o:lib.dll" cfg.fsc_flags ["lib.mli";"lib.ml";"libv.ml"]

        do! peverify cfg "lib.dll"

        // "%FSC%" %fsc_flags% -a --optimize -r:lib.dll -o:lib2.dll lib2.mli lib2.ml lib3.ml
        do! fsc cfg "%s -a --optimize -r:lib.dll -o:lib2.dll" cfg.fsc_flags ["lib2.mli";"lib2.ml";"lib3.ml"]

        do! peverify cfg "lib2.dll"

        // "%FSC%" %fsc_flags% --optimize -r:lib.dll -r:lib2.dll -o:client.exe client.ml
        do! fsc cfg "%s --optimize -r:lib.dll -r:lib2.dll -o:client.exe" cfg.fsc_flags ["client.ml"]

        do! peverify cfg "client.exe"

        })


[<Test; FSharpSuiteCodeAndSignaturePermutations("core/innerpoly")>]
let innerpoly p = singleTestBuildAndRun p
        
[<Test; FSharpSuiteScriptPermutations("core/int32")>]
let ``test int32`` p = singleTestBuildAndRun p

[<Test; FSharpSuiteTest("core/queriesCustomQueryOps")>]
let queriesCustomQueryOps () = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    // "%FSC%" %fsc_flags% -o:test.exe -g test.fsx
    do! fsc cfg """%s -o:test.exe -g""" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "test.exe"

    // "%FSC%" %fsc_flags% --optimize -o:test--optimize.exe -g test.fsx
    do! fsc cfg """%s --optimize -o:test--optimize.exe -g""" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "test--optimize.exe"

    do! singleNegTest cfg "negativetest"

    do! attempt {
        use testOkFile = fileguard cfg "test.ok"

        do! fsi cfg "%s" cfg.fsi_flags ["test.fsx"]

        do! testOkFile |> NUnitConf.checkGuardExists
    }

    do! attempt {
        use testOkFile = fileguard cfg "test.ok"

        do! exec cfg ("."/"test.exe") ""

        do! testOkFile |> NUnitConf.checkGuardExists
        }

    do! attempt {
        use testOkFile = fileguard cfg "test.ok"

        do! exec cfg ("."/"test--optimize.exe") ""

        do! testOkFile |> NUnitConf.checkGuardExists
        }
                
    })

[<Test>]
[<FSharpSuiteTestCase("core/printing", "", "z.output.test.default.stdout.txt", "z.output.test.default.stdout.bsl", "z.output.test.default.stderr.txt", "z.output.test.default.stderr.bsl")>]
[<FSharpSuiteTestCase("core/printing", "--use:preludePrintSize1000.fsx", "z.output.test.1000.stdout.txt", "z.output.test.1000.stdout.bsl", "z.output.test.1000.stderr.txt", "z.output.test.1000.stderr.bsl")>]
[<FSharpSuiteTestCase("core/printing", "--use:preludePrintSize200.fsx", "z.output.test.200.stdout.txt", "z.output.test.200.stdout.bsl", "z.output.test.200.stderr.txt", "z.output.test.200.stderr.bsl")>]
[<FSharpSuiteTestCase("core/printing", "--use:preludeShowDeclarationValuesFalse.fsx", "z.output.test.off.stdout.txt", "z.output.test.off.stdout.bsl", "z.output.test.off.stderr.txt", "z.output.test.off.stderr.bsl")>]
[<FSharpSuiteTestCase("core/printing", "--quiet", "z.output.test.quiet.stdout.txt", "z.output.test.quiet.stdout.bsl", "z.output.test.quiet.stderr.txt", "z.output.test.quiet.stderr.bsl")>]
let printing flag diffFileOut expectedFileOut diffFileErr expectedFileErr = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    do! requireENCulture ()

    let copy from' = Commands.copy_y cfg.Directory from' >> checkResult

    let ``fsi <a >b 2>c`` =
        // "%FSI%" %fsc_flags_errors_ok%  --nologo                                    <test.fsx >z.raw.output.test.default.txt 2>&1
        let ``exec <a >b 2>c`` (inFile, outFile, errFile) p = 
            Command.exec cfg.Directory cfg.EnvironmentVariables { Output = OutputAndError(Overwrite(outFile), Overwrite(errFile)); Input = Some(RedirectInput(inFile)); } p 
            >> checkResult
        Printf.ksprintf (fun flags (inFile, outFile, errFile) -> Commands.fsi (``exec <a >b 2>c`` (inFile, outFile, errFile)) cfg.FSI flags [])
        
    let fsdiff a b = 
        let ``exec >`` f p = Command.exec cfg.Directory cfg.EnvironmentVariables { Output = Output(Overwrite(f)); Input = None} p >> checkResult
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
        File.ReadLines from' |> (``findstr /v`` cfg.Directory) |> (``findstr /v`` "--help' for options") |> (fun lines -> File.WriteAllLines(getfullpath cfg to', lines))

    removeCDandHelp rawFileOut diffFileOut
    removeCDandHelp rawFileErr diffFileErr

    let withDefault default' to' =
        if not (fileExists cfg to') then Some (copy default' to') else None

    do! expectedFileOut |> withDefault diffFileOut
    do! expectedFileErr |> withDefault diffFileErr

    do! fsdiff diffFileOut expectedFileOut
    do! fsdiff diffFileErr expectedFileErr

    })

(*

[<Test>]
[<FSharpSuiteTestCase("","test-unsigned.bsl")>]
[<FSharpSuiteTestCase("--keyfile:sha1full.snk", "test-sha1-full-cl.bsl")>]
[<FSharpSuiteTestCase("--keyfile:sha256full.snk", "test-sha256-full-cl.bsl")>]
[<FSharpSuiteTestCase("--keyfile:sha512full.snk", "test-sha512-full-cl.bsl")>]
[<FSharpSuiteTestCase("--keyfile:sha1024full.snk", "test-sha1024-full-cl.bsl")>]
[<FSharpSuiteTestCase("--keyfile:sha1delay.snk --delaysign:yes", "test-sha1-delay-cl.bsl")>]
[<FSharpSuiteTestCase("--keyfile:sha256delay.snk --delaysign:yes", "test-sha256-delay-cl.bsl")>]
[<FSharpSuiteTestCase("--keyfile:sha512delay.snk --delaysign:yes", "test-sha512-delay-cl.bsl")>]
// Test dumpbin with SHA 1024 bit key public signed CL
[<FSharpSuiteTestCase("--keyfile:sha1024delay.snk --delaysign:yes", "test-sha1024-delay-cl.bsl")>]
// Test SHA1 key full signed  Attributes
[<FSharpSuiteTestCase("--define:SHA1","test-sha1-full-attributes.bsl")>]
// Test SHA1 key delayl signed  Attributes
[<FSharpSuiteTestCase("--keyfile:sha1delay.snk true --define:SHA1 --define:DELAY", "test-sha1-delay-attributes.bsl")>]
[<FSharpSuiteTestCase("--define:SHA256", "test-sha256-full-attributes.bsl")>]
// Test SHA 256 bit key delay signed  Attributes
[<FSharpSuiteTestCase("--define:SHA256 --define:DELAY", "test-sha256-delay-attributes.bsl")>]
// Test SHA 512 bit key fully signed  Attributes
[<FSharpSuiteTestCase("--define:SHA512", "test-sha512-full-attributes.bsl")>]
// Test SHA 512 bit key delay signed Attributes
[<FSharpSuiteTestCase("--define:SHA512 --define:DELAY", "test-sha512-delay-attributes.bsl")>]
// Test SHA 1024 bit key fully signed  Attributes
[<FSharpSuiteTestCase("--define:SHA1024", "test-sha1024-full-attributes.bsl")>]
// Test dumpbin with SHA 1024 bit key public signed CL
[<FSharpSuiteTestCase("--keyfile:sha1024delay.snk true", "test-sha1024-public-cl.bsl")>]
let signedtest args bslfile = attempt {
    
        let cfg = FSharpTestSuite.testConfig ()
        let cfg = { cfg with fsc_flags=cfg.fsc_flags + " " + args }
        let ``exec >>`` f p = Command.exec cfg.Directory cfg.EnvironmentVariables { Output = Output(Append(f)); Input = None} p 


        let ``sn >>`` outfile = ``exec >>`` outfile cfg.SN
        let fsdiff a b = 
            let diffFile = Path.ChangeExtension(a, ".diff")
            Commands.fsdiff (``exec >>`` diffFile) cfg.FSDIFF a b

        let outfile = Path.ChangeExtension(bslfile,"sn.out") 
        let exefile = Path.ChangeExtension(bslfile,"exe") 
        do File.WriteAllLines(outfile,["sn -q stops all output except error messages";
                                       "if the output is a valid file no output is produced."
                                       "delay-signed and unsigned produce error messages."])

        do! fsc cfg "%s -o:%s" cfg.fsc_flags exefile ["test.fs"]
        do! ``sn >>`` outfile ("-q -vf "+exefile) |> (fun _ -> Success)
        do! fsdiff outfile bslfile |> checkResult
        }
*)

[<Test; FSharpSuiteTest("core/quotes")>]
let quotes () = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    do! csc cfg """/nologo  /target:library /out:cslib.dll""" ["cslib.cs"]

    // "%FSC%" %fsc_flags% -o:test.exe -r cslib.dll -g test.fsx
    do! fsc cfg "%s -o:test.exe -r cslib.dll -g" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "test.exe"

    // "%FSC%" %fsc_flags% -o:test-with-debug-data.exe --quotations-debug+ -r cslib.dll -g test.fsx
    do! fsc cfg "%s -o:test-with-debug-data.exe --quotations-debug+ -r cslib.dll -g" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "test-with-debug-data.exe"

    // "%FSC%" %fsc_flags% --optimize -o:test--optimize.exe -r cslib.dll -g test.fsx
    do! fsc cfg "%s --optimize -o:test--optimize.exe -r cslib.dll -g" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "test--optimize.exe"

    do! attempt {
        use testOkFile = fileguard cfg "test.ok"

        // "%FSI%" %fsi_flags% -r cslib.dll test.fsx
        do! fsi cfg "%s -r cslib.dll" cfg.fsi_flags ["test.fsx"]
            

        do! testOkFile |> NUnitConf.checkGuardExists
        }

    do! attempt {

        use testOkFile = fileguard cfg "test.ok"

    
        do! exec cfg ("."/"test.exe") ""


        do! testOkFile |> NUnitConf.checkGuardExists
        }

    do! attempt {

        use testOkFile = fileguard cfg "test.ok"

        // test-with-debug-data.exe
        do! exec cfg ("."/"test-with-debug-data.exe") ""


        do! testOkFile |> NUnitConf.checkGuardExists
        }

    do! attempt {

        use testOkFile = fileguard cfg "test.ok"

        // test--optimize.exe
        do! exec cfg ("."/"test--optimize.exe") ""


        do! testOkFile |> NUnitConf.checkGuardExists
        }
                
    })


[<Test; FSharpSuiteCodeAndSignaturePermutations("core/namespaces")>]
let namespaceAttributes p = singleTestBuildAndRun p

[<Test; FSharpSuiteTest("core/parsing")>]
let parsing () = check  (attempt {
    let cfg = FSharpTestSuite.testConfig ()
        
    // "%FSC%" %fsc_flags% -a -o:crlf.dll -g crlf.ml
    do! fsc cfg "%s -a -o:crlf.dll -g" cfg.fsc_flags ["crlf.ml"]

    // "%FSC%" %fsc_flags% -o:toplet.exe -g toplet.ml
    do! fsc cfg "%s -o:toplet.exe -g" cfg.fsc_flags ["toplet.ml"]

    do! peverify cfg "toplet.exe"

    }) 

[<Test; FSharpSuiteTest("core/unicode")>]
let unicode () = check  (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    // "%FSC%" %fsc_flags% -a -o:kanji-unicode-utf8-nosig-codepage-65001.dll -g kanji-unicode-utf8-nosig-codepage-65001.fs
    do! fsc cfg "%s -a -o:kanji-unicode-utf8-nosig-codepage-65001.dll -g" cfg.fsc_flags ["kanji-unicode-utf8-nosig-codepage-65001.fs"]

    // "%FSC%" %fsc_flags% -a -o:kanji-unicode-utf8-nosig-codepage-65001.dll -g kanji-unicode-utf8-nosig-codepage-65001.fs
    do! fsc cfg "%s -a -o:kanji-unicode-utf8-nosig-codepage-65001.dll -g" cfg.fsc_flags ["kanji-unicode-utf8-nosig-codepage-65001.fs"]

    let codepage = attempt {
        // "%FSC%" %fsc_flags% -a -o:kanji-unicode-utf16.dll -g kanji-unicode-utf16.fs
        do! fsc cfg "%s -a -o:kanji-unicode-utf16.dll -g" cfg.fsc_flags ["kanji-unicode-utf16.fs"]

        // "%FSC%" %fsc_flags% -a --codepage:65000 -o:kanji-unicode-utf7-codepage-65000.dll -g kanji-unicode-utf7-codepage-65000.fs
        do! fsc cfg "%s -a --codepage:65000 -o:kanji-unicode-utf7-codepage-65000.dll -g" cfg.fsc_flags ["kanji-unicode-utf7-codepage-65000.fs"]
        }

    // REM check non-utf8 and --codepage flag for bootstrapped fsc.exe
    // if NOT "%FSC:fscp=X%" == "%FSC%" (
    do! if not <| cfg.FSC.Contains("fscp") then codepage else Success

    // "%FSC%" %fsc_flags% -a -o:kanji-unicode-utf8-withsig-codepage-65001.dll -g kanji-unicode-utf8-withsig-codepage-65001.fs
    do! fsc cfg "%s -a -o:kanji-unicode-utf8-withsig-codepage-65001.dll -g" cfg.fsc_flags ["kanji-unicode-utf8-withsig-codepage-65001.fs"]

    // "%FSI%" %fsi_flags% --utf8output kanji-unicode-utf8-nosig-codepage-65001.fs
    do! fsi cfg "%s --utf8output" cfg.fsi_flags ["kanji-unicode-utf8-nosig-codepage-65001.fs"]

    // "%FSI%" %fsi_flags% --utf8output --codepage:65001 kanji-unicode-utf8-withsig-codepage-65001.fs
    do! fsi cfg "%s --utf8output --codepage:65001" cfg.fsi_flags ["kanji-unicode-utf8-withsig-codepage-65001.fs"]

    // "%FSI%" %fsi_flags% --utf8output kanji-unicode-utf8-withsig-codepage-65001.fs
    do! fsi cfg "%s --utf8output" cfg.fsi_flags ["kanji-unicode-utf8-withsig-codepage-65001.fs"]

    // "%FSI%" %fsi_flags% --utf8output --codepage:65000  kanji-unicode-utf7-codepage-65000.fs
    do! fsi cfg "%s --utf8output --codepage:65000" cfg.fsi_flags ["kanji-unicode-utf7-codepage-65000.fs"]

    // "%FSI%" %fsi_flags% --utf8output kanji-unicode-utf16.fs
    do! fsi cfg "%s --utf8output" cfg.fsi_flags ["kanji-unicode-utf16.fs"]
    }) 

[<Test; FSharpSuiteScriptPermutations("core/unicode")>]
let unicode2 p = singleTestBuildAndRun p

[<Test; FSharpSuiteTest("core/internalsvisible")>]
let internalsvisible () = check  (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    // Compiling F# Library
    do! fsc cfg "%s --version:1.2.3 --keyfile:key.snk -a --optimize -o:library.dll" cfg.fsc_flags ["library.fsi"; "library.fs"]

    do! peverify cfg "library.dll"

    // Compiling C# Library
    do! csc cfg "/target:library /keyfile:key.snk /out:librarycs.dll" ["librarycs.cs"]

    do! peverify cfg "librarycs.dll"

    // Compiling F# main referencing C# and F# libraries
    do! fsc cfg "%s --version:1.2.3 --keyfile:key.snk --optimize -r:library.dll -r:librarycs.dll -o:main.exe" cfg.fsc_flags ["main.fs"]

    do! peverify cfg "main.exe"

    // Run F# main. Quick test!
    do! exec cfg ("."/"main.exe") ""
    }) 


// Repro for https://github.com/Microsoft/visualfsharp/issues/1298
[<Test; FSharpSuiteTest("core/fileorder")>]
let fileorder () = check  (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    log "== Compiling F# Library and Code, when empty file libfile2.fs IS NOT included"
    do! fsc cfg "%s -a --optimize -o:lib.dll " cfg.fsc_flags ["libfile1.fs"]

    do! peverify cfg "lib.dll"

    do! fsc cfg "%s -r:lib.dll -o:test.exe" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "test.exe"

    do! exec cfg ("."/"test.exe") ""

    log "== Compiling F# Library and Code, when empty file libfile2.fs IS included"
    do! fsc cfg "%s -a --optimize -o:lib2.dll " cfg.fsc_flags ["libfile1.fs"; "libfile2.fs"]

    do! peverify cfg "lib2.dll"

    do! fsc cfg "%s -r:lib2.dll -o:test2.exe" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "test2.exe"

    do! exec cfg ("."/"test2.exe") ""
    }) 


[<Test; FSharpSuiteTest("core/interop")>]
let interop () = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    let cfg = 
        { cfg with 
            EnvironmentVariables = 
                cfg.EnvironmentVariables
                |> Map.add "FSCOREDLLPATH" cfg.FSCOREDLLPATH
                |> Map.add "FSCOREDLLNETCORE78PATH" cfg.FSCOREDLLNETCORE78PATH }

    do! msbuild cfg "" ["PCL.fsproj"]

    do! msbuild cfg "" ["User.fsproj"]

    do! peverify cfg "User.exe"

    do! exec cfg ("."/"User.exe") ""
                
    })

[<Test; FSharpSuiteScriptPermutations("core/lazy")>]
let ``lazy test`` p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("core/letrec")>]
let letrec p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("core/letrec-mutrec")>]
let ``letrec (mutrec variations part one)`` p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("core/letrec-mutrec2")>]
let ``letrec (mutrec variations part two)`` p = singleTestBuildAndRun p

[<Test; FSharpSuiteAllPermutations("core/libtest")>]
let libtest p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("core/lift")>]
let lift p = singleTestBuildAndRun p


module ``Load-Script`` = 

    [<Test; FSharpSuiteTest("core/load-script")>]
    let ``load-script`` () = check (attempt {
        let cfg = FSharpTestSuite.testConfig ()

        // script > out.txt 2>&1
        let stdoutPath = "out.stdout.txt" |> getfullpath cfg
        let stderrPath = "out.stderr.txt" |> getfullpath cfg
        let stderrBaseline = "out.stderr.bsl" |> getfullpath cfg 
        let stdoutBaseline = "out.stdout.bsl" |> getfullpath cfg 

        let type_append_tofile from = Commands.type_append_tofile cfg.Directory from stdoutPath
        let echo text = Commands.echo_append_tofile cfg.Directory text stdoutPath

        File.WriteAllText(stdoutPath, "")
        File.WriteAllText(stderrPath, "")

        // del 3.exe 2>nul 1>nul
        do if fileExists cfg "3.exe" then getfullpath cfg "3.exe" |> File.Delete
        // type 1.fsx 2.fsx 3.fsx
        ["1.fsx"; "2.fsx"; "3.fsx"] |> List.iter type_append_tofile

        echo "Test 1================================================="
        // "%FSC%" 3.fsx --nologo
        do! fscToOutIgnoreExitCode cfg stdoutPath stderrPath "--nologo" ["3.fsx"]
        // 3.exe
        do! execToOutAndIgnoreExitCode cfg stdoutPath stderrPath ("."/"3.exe") ""
        // del 3.exe
        rm cfg "3.exe"

        echo "Test 2================================================="
        // "%FSI%" 3.fsx
        do! fsiToOutIgnoreExitCode cfg stdoutPath stderrPath "" ["3.fsx"]

        echo "Test 3================================================="
        // "%FSI%" --nologo < pipescr
        do! fsiFromInToOutIgnoreExitCode cfg stdoutPath stderrPath "--nologo" "pipescr"
        // echo.

        echo "Test 4================================================="
        // "%FSI%" usesfsi.fsx
        do! fsiToOutIgnoreExitCode cfg stdoutPath stderrPath "" ["usesfsi.fsx"]

        echo "Test 5================================================="
        // "%FSC%" usesfsi.fsx --nologo
        do! fscToOutIgnoreExitCode cfg stdoutPath stderrPath "--nologo" ["usesfsi.fsx"]

        echo "Test 6================================================="
        // "%FSC%" usesfsi.fsx --nologo -r FSharp.Compiler.Interactive.Settings
        do! fscToOutIgnoreExitCode cfg stdoutPath stderrPath "--nologo -r FSharp.Compiler.Interactive.Settings" ["usesfsi.fsx"]

        echo "Test 7================================================="
        // "%FSI%" 1.fsx 2.fsx 3.fsx
        do! fsiToOutIgnoreExitCode cfg stdoutPath stderrPath "" ["1.fsx";"2.fsx";"3.fsx"]

        echo "Test 8================================================="
        // "%FSI%" 3.fsx 2.fsx 1.fsx
        do! fsiToOutIgnoreExitCode cfg stdoutPath stderrPath "" ["3.fsx";"2.fsx";"1.fsx"]

        echo "Test 9================================================="
        // "%FSI%" multiple-load-1.fsx
        do! fsiToOutIgnoreExitCode cfg stdoutPath stderrPath "" ["multiple-load-1.fsx"]

        echo "Test 10================================================="
        // "%FSI%" multiple-load-2.fsx
        do! fsiToOutIgnoreExitCode cfg stdoutPath stderrPath "" ["multiple-load-2.fsx"]

        echo "Test 11================================================="
        // "%FSC%" FlagCheck.fs --nologo
        do! fscToOutIgnoreExitCode cfg stdoutPath stderrPath "--nologo" ["FlagCheck.fs"]
        // FlagCheck.exe
        do! execToOutAndIgnoreExitCode cfg stdoutPath stderrPath ("."/"FlagCheck.exe") ""
        // rm cfg FlagCheck.exe
        rm cfg "FlagCheck.exe"

        echo "Test 12================================================="
        // "%FSC%" FlagCheck.fsx  --nologo
        do! fscToOutIgnoreExitCode cfg stdoutPath stderrPath "-o FlagCheckScript.exe --nologo" ["FlagCheck.fsx"]
        // FlagCheck.exe
        do! execToOutAndIgnoreExitCode cfg stdoutPath stderrPath ("."/"FlagCheckScript.exe") ""
        // rm cfg FlagCheck.exe
        rm cfg "FlagCheckScript.exe"

        echo "Test 13================================================="
        // "%FSI%" load-FlagCheckFs.fsx
        do! fsiToOutIgnoreExitCode cfg stdoutPath stderrPath "" ["load-FlagCheckFs.fsx"]

        echo "Test 14================================================="
        // "%FSI%" FlagCheck.fsx
        do! fsiToOutIgnoreExitCode cfg stdoutPath stderrPath "" ["FlagCheck.fsx"]

        echo "Test 15================================================="
        // "%FSI%" ProjectDriver.fsx
        do! fsiToOutIgnoreExitCode cfg stdoutPath stderrPath "" ["ProjectDriver.fsx"]

        echo "Test 16================================================="
        // "%FSC%" ProjectDriver.fsx --nologo
        do! fscToOutIgnoreExitCode cfg stdoutPath stderrPath "--nologo" ["ProjectDriver.fsx"]

        do! execToOutAndIgnoreExitCode cfg stdoutPath stderrPath ("."/"ProjectDriver.exe") ""

        rm cfg "ProjectDriver.exe"

        echo "Test 17================================================="
        // "%FSI%" load-IncludeNoWarn211.fsx
        do! fsiToOutIgnoreExitCode cfg stdoutPath stderrPath "" ["load-IncludeNoWarn211.fsx"]

        echo "Done =================================================="

        // if NOT EXIST out.bsl COPY out.txt
        ignore "useless, first run, same as use an empty file"

        let normalizePaths f =
            let text = File.ReadAllText(f)
            let dummyPath = @"D:\staging\staging\src\tests\fsharp\core\load-script"
            let contents = System.Text.RegularExpressions.Regex.Replace(text, System.Text.RegularExpressions.Regex.Escape(cfg.Directory), dummyPath)
            File.WriteAllText(f, contents)

        normalizePaths stdoutPath
        normalizePaths stderrPath

        let! diffs = fsdiff cfg stdoutPath stdoutBaseline

        do! match diffs with
            | [] -> Success
            | _ -> NUnitConf.genericError (sprintf "'%s' and '%s' differ; %A" stdoutPath stdoutBaseline diffs)

        let! diffs = fsdiff cfg stderrPath stderrBaseline

        do! match diffs with
            | [] -> Success
            | _ -> NUnitConf.genericError (sprintf "'%s' and '%s' differ; %A" stderrPath stderrBaseline diffs)

        })

[<Test; FSharpSuiteScriptPermutations("core/longnames")>]
let longnames p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("core/map")>]
let map p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("core/math/numbers")>]
let numbers p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("core/math/numbersVS2008")>]
let numbersVS2008 p = singleTestBuildAndRun p

[<Test; FSharpSuiteCodeAndSignaturePermutations("core/measures")>]
let measures p = singleTestBuildAndRun p

[<Test; FSharpSuiteCodeAndSignaturePermutations("core/members/basics")>]
let Basics p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("core/members/basics-hw")>]
let BasicsHw p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("core/members/basics-hw-mutrec")>]
let BasicsHwMutrec p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("core/members/ctree")>]
let ctree p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("core/members/factors")>]
let factors p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("core/members/factors-mutrec")>]
let factorsMutrec p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("core/members/incremental")>]
let incremental p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("core/members/incremental-hw")>]
let incrementalHw p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("core/members/incremental-hw-mutrec")>]
let incrementalHwMutrec p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("core/members/ops")>]
let ops p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("core/members/ops-mutrec")>]
let opsMutrec p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("core/nested")>]
let nested p = singleTestBuildAndRun p

[<Test; Category("Expensive"); FSharpSuiteTest("core/netcore/netcore259")>]
let ``Run all tests using PCL profie 259 FSHarp.Core``() = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    let envVars =
        cfg.EnvironmentVariables
        |> Map.add "FSCOREDLLNETCORE259PATH" cfg.FSCOREDLLNETCORE259PATH

    let exec p = Command.exec cfg.Directory envVars { Output = Inherit; Input = None; } p >> checkResult
    let msbuild = Printf.ksprintf (Commands.msbuild exec (cfg.MSBUILD.Value))

    // "%MSBUILDTOOLSPATH%\msbuild.exe" ..\netcore.sln /p:Configuration=Debug /p:TestProfile=Profile259 /t:Rebuild
    do! msbuild "/p:Configuration=Debug /p:TestProfile=Profile259 /t:Rebuild" [".."/"netcore.sln"]

    // set CONTROL_FAILURES_LOG=%~dp0..\ConsoleApplication1\bin\Debug\Profile259\control_failures.log
    let setLog = Map.add "CONTROL_FAILURES_LOG" (getfullpath cfg ".."/"ConsoleApplication1"/"bin"/"Debug"/"Profile259"/"control_failures.log")

    let exec p = Command.exec cfg.Directory (cfg.EnvironmentVariables |> setLog) { Output = Inherit; Input = None; } p >> checkResult

    // ..\ConsoleApplication1\bin\Debug\Profile259\PortableTestEntry.exe
    do! exec (".."/"ConsoleApplication1"/"bin"/"Debug"/"Profile259"/"PortableTestEntry.exe") ""
                
    })


[<Test; Category("Expensive"); FSharpSuiteTest("core/netcore/netcore7")>]
let ``Run all tests using PCL profie 7 FSHarp.Core`` () = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    let envVars =
        cfg.EnvironmentVariables
        |> Map.add "FSCOREDLLNETCOREPATH" cfg.FSCOREDLLNETCOREPATH

    let exec p = Command.exec cfg.Directory envVars { Output = Inherit; Input = None; } p >> checkResult
    let msbuild = Printf.ksprintf (Commands.msbuild exec (cfg.MSBUILD.Value))

    // "%MSBUILDTOOLSPATH%\msbuild.exe" ..\netcore.sln /p:Configuration=Debug /p:TestProfile=Profile7 /t:Rebuild
    do! msbuild "/p:Configuration=Debug /p:TestProfile=Profile7 /t:Rebuild" [".."/"netcore.sln"]

    // set CONTROL_FAILURES_LOG=%~dp0..\ConsoleApplication1\bin\Debug\profile7\control_failures.log
    let setLog = Map.add "CONTROL_FAILURES_LOG" (getfullpath cfg ".."/"ConsoleApplication1"/"bin"/"Debug"/"profile7"/"control_failures.log")
   
    let exec p = Command.exec cfg.Directory (cfg.EnvironmentVariables |> setLog) { Output = Inherit; Input = None; } p >> checkResult

    // ..\ConsoleApplication1\bin\Debug\profile7\PortableTestEntry.exe
    do! exec (".."/"ConsoleApplication1"/"bin"/"Debug"/"profile7"/"PortableTestEntry.exe") ""
                
    })

[<Test; Category("Expensive"); FSharpSuiteTest("core/netcore/netcore78")>]
let ``Run all tests using PCL profie 78 FSHarp.Core``() = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    let envVars =
        cfg.EnvironmentVariables
        |> Map.add "FSCOREDLLNETCORE78PATH" cfg.FSCOREDLLNETCORE78PATH

    let exec p = Command.exec cfg.Directory envVars { Output = Inherit; Input = None; } p >> checkResult
    let msbuild = Printf.ksprintf (Commands.msbuild exec (cfg.MSBUILD.Value))

    // "%MSBUILDTOOLSPATH%\msbuild.exe" ..\netcore.sln /p:Configuration=Debug /p:TestProfile=Profile78 /t:Rebuild
    do! msbuild "/p:Configuration=Debug /p:TestProfile=Profile78 /t:Rebuild" [".."/"netcore.sln"]

    // set CONTROL_FAILURES_LOG=%~dp0..\ConsoleApplication1\bin\Debug\profile78\control_failures.log
    let setLog = Map.add "CONTROL_FAILURES_LOG" (getfullpath cfg ".."/"ConsoleApplication1"/"bin"/"Debug"/"profile78"/"control_failures.log")
   
    let exec p = Command.exec cfg.Directory (cfg.EnvironmentVariables |> setLog) { Output = Inherit; Input = None; } p >> checkResult

    // ..\ConsoleApplication1\bin\Debug\profile78\PortableTestEntry.exe
    do! exec (".."/"ConsoleApplication1"/"bin"/"Debug"/"profile78"/"PortableTestEntry.exe") ""
                
    })

[<Test; Category("Expensive"); FSharpSuiteTest("core/portable")>]
let ``Run all tests using PCL profie 47 FSHarp.Core`` () = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    let envVars =
        cfg.EnvironmentVariables
        |> Map.add "FSCOREDLLPORTABLEPATH" cfg.FSCOREDLLPORTABLEPATH

    let exec p = Command.exec cfg.Directory envVars { Output = Inherit; Input = None; } p >> checkResult
    let msbuild = Printf.ksprintf (Commands.msbuild exec (cfg.MSBUILD.Value))

    // "%MSBUILDTOOLSPATH%\msbuild.exe" portablelibrary1.sln /p:Configuration=Debug
    do! msbuild "/p:Configuration=Debug" ["portablelibrary1.sln"]

    // set CONTROL_FAILURES_LOG=%~dp0\control_failures.log
    let setLog = Map.add "CONTROL_FAILURES_LOG" (getfullpath cfg "control_failures.log")

    let exec p = Command.exec cfg.Directory (cfg.EnvironmentVariables |> setLog) { Output = Inherit; Input = None; } p >> checkResult
           
    // .\ConsoleApplication1\bin\Debug\PortableTestEntry.exe
    do! exec ("."/"ConsoleApplication1"/"bin"/"Debug"/"PortableTestEntry.exe") ""
                
    })


[<Test; FSharpSuiteScriptPermutations("core/patterns")>]
let patterns p = singleTestBuildAndRun p

[<Test; FSharpSuiteTest("core/pinvoke")>]
let pinvoke () = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    do! fsc cfg "%s -o:test.exe -g" cfg.fsc_flags ["test.fsx"]
   
    do! peverifyWithArgs cfg "/nologo /MD" "test.exe"
                
    })


let printfPrmutations () = [ FSharpSuiteTestCaseData("core/printf", FSC_BASIC) ]

[<Test; TestCaseSource("printfPrmutations")>]
let printf p = singleTestBuildAndRun p


[<Test; FSharpSuiteTest("core/queriesLeafExpressionConvert")>]
let queriesLeafExpressionConvert () = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    // "%FSC%" %fsc_flags% -o:test.exe -g test.fsx
    do! fsc cfg "%s -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "test.exe"

    // "%FSC%" %fsc_flags% --optimize -o:test--optimize.exe -g test.fsx
    do! fsc cfg "%s --optimize -o:test--optimize.exe -g" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "test--optimize.exe"

    use testOkFile = fileguard cfg "test.ok"
    // "%FSI%" %fsi_flags% test.fsx
    do! fsi cfg "%s" cfg.fsi_flags ["test.fsx"]

    do! testOkFile |> NUnitConf.checkGuardExists

    use testOkFile2 = fileguard cfg "test.ok"

    do! exec cfg ("."/"test.exe") ""

    do! testOkFile2 |> NUnitConf.checkGuardExists

    use testOkFile3 = fileguard cfg "test.ok"
    // test--optimize.exe
    do! exec cfg ("."/"test--optimize.exe") ""

    do! testOkFile3 |> NUnitConf.checkGuardExists
                
    })


[<Test; FSharpSuiteTest("core/queriesNullableOperators")>]
let queriesNullableOperators () = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    // "%FSC%" %fsc_flags% -o:test.exe -g test.fsx
    do! fsc cfg "%s -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "test.exe"

    // "%FSC%" %fsc_flags% --optimize -o:test--optimize.exe -g test.fsx
    do! fsc cfg "%s --optimize -o:test--optimize.exe -g" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "test--optimize.exe"

    use testOkFile = fileguard cfg "test.ok"
    do! fsi cfg "%s" cfg.fsi_flags ["test.fsx"]
    do! testOkFile |> NUnitConf.checkGuardExists

    use testOkFile2 = fileguard cfg "test.ok"
    do! exec cfg ("."/"test.exe") ""
    do! testOkFile2 |> NUnitConf.checkGuardExists

    use testOkFile3 = fileguard cfg "test.ok"
    do! exec cfg ("."/"test--optimize.exe") ""
    do! testOkFile3 |> NUnitConf.checkGuardExists
                
    })

[<Test; FSharpSuiteTest("core/queriesOverIEnumerable")>]
let queriesOverIEnumerable () = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    // "%FSC%" %fsc_flags% -o:test.exe -g test.fsx
    do! fsc cfg "%s -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "test.exe" 

    // "%FSC%" %fsc_flags% --optimize -o:test--optimize.exe -g test.fsx
    do! fsc cfg "%s --optimize -o:test--optimize.exe -g" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "test--optimize.exe"

    use testOkFile = fileguard cfg "test.ok"
    // "%FSI%" %fsi_flags% test.fsx
    do! fsi cfg "%s" cfg.fsi_flags ["test.fsx"]

    do! testOkFile |> NUnitConf.checkGuardExists

    use testOkFile2 = fileguard cfg "test.ok"

    do! exec cfg ("."/"test.exe") ""

    do! testOkFile2 |> NUnitConf.checkGuardExists



    use testOkFile3 = fileguard cfg "test.ok"
    // test--optimize.exe
    do! exec cfg ("."/"test--optimize.exe") ""

    do! testOkFile3 |> NUnitConf.checkGuardExists
                
    })

[<Test; FSharpSuiteTest("core/queriesOverIQueryable")>]
let queriesOverIQueryable () = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    // "%FSC%" %fsc_flags% -o:test.exe -g test.fsx
    do! fsc cfg "%s -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "test.exe"

    // "%FSC%" %fsc_flags% --optimize -o:test--optimize.exe -g test.fsx
    do! fsc cfg "%s --optimize -o:test--optimize.exe -g" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "test--optimize.exe"


    use testOkFile = fileguard cfg "test.ok"
    // "%FSI%" %fsi_flags% test.fsx
    do! fsi cfg "%s" cfg.fsi_flags ["test.fsx"]

    do! testOkFile |> NUnitConf.checkGuardExists


    use testOkFile2 = fileguard cfg "test.ok"

    do! exec cfg ("."/"test.exe") ""

    do! testOkFile2 |> NUnitConf.checkGuardExists


    use testOkFile3 = fileguard cfg "test.ok"
    // test--optimize.exe
    do! exec cfg ("."/"test--optimize.exe") ""

    do! testOkFile3 |> NUnitConf.checkGuardExists
                
    })


[<Test; FSharpSuiteTest("core/quotesDebugInfo")>]
let quotesDebugInfo () = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    // "%FSC%" %fsc_flags% --quotations-debug+ --optimize -o:test.exe -g test.fsx
    do! fsc cfg "%s --quotations-debug+ --optimize -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "test.exe"

    // "%FSC%" %fsc_flags% --quotations-debug+ --optimize -o:test--optimize.exe -g test.fsx
    do! fsc cfg "%s --quotations-debug+ --optimize -o:test--optimize.exe -g" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "test--optimize.exe"


    use testOkFile = fileguard cfg "test.ok"
    // "%FSI%" %fsi_flags% --quotations-debug+ test.fsx
    do! fsi cfg "%s --quotations-debug+" cfg.fsi_flags ["test.fsx"]

    do! testOkFile |> NUnitConf.checkGuardExists


    use testOkFile2 = fileguard cfg "test.ok"

    do! exec cfg ("."/"test.exe") ""

    do! testOkFile2 |> NUnitConf.checkGuardExists



    use testOkFile3 = fileguard cfg "test.ok"
    // test--optimize.exe
    do! exec cfg ("."/"test--optimize.exe") ""

    do! testOkFile3 |> NUnitConf.checkGuardExists
                
    })

[<Test; FSharpSuiteTest("core/quotesInMultipleModules")>]
let quotesInMultipleModules () = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    // "%FSC%" %fsc_flags% -o:module1.dll --target:library module1.fsx
    do! fsc cfg "%s -o:module1.dll --target:library" cfg.fsc_flags ["module1.fsx"]

    do! peverify cfg "module1.dll"

    // "%FSC%" %fsc_flags% -o:module2.exe -r:module1.dll module2.fsx
    do! fsc cfg "%s -o:module2.exe -r:module1.dll" cfg.fsc_flags ["module2.fsx"]

    do! peverify cfg "module2.exe"
    
    // "%FSC%" %fsc_flags% --staticlink:module1 -o:module2-staticlink.exe -r:module1.dll module2.fsx
    do! fsc cfg "%s --staticlink:module1 -o:module2-staticlink.exe -r:module1.dll" cfg.fsc_flags ["module2.fsx"]

    do! peverify cfg "module2-staticlink.exe"

    // "%FSC%" %fsc_flags% -o:module1-opt.dll --target:library --optimize module1.fsx
    do! fsc cfg "%s -o:module1-opt.dll --target:library --optimize" cfg.fsc_flags ["module1.fsx"]

    do! peverify cfg "module1-opt.dll"

    // "%FSC%" %fsc_flags% -o:module2-opt.exe -r:module1-opt.dll --optimize module2.fsx
    do! fsc cfg "%s -o:module2-opt.exe -r:module1-opt.dll --optimize" cfg.fsc_flags ["module2.fsx"]

    do! peverify cfg "module2-opt.exe"


    use testOkFile = fileguard cfg "test.ok"
    // "%FSI%" %fsi_flags% -r module1.dll module2.fsx
    do! fsi cfg "%s -r module1.dll" cfg.fsi_flags ["module2.fsx"]

    do! testOkFile |> NUnitConf.checkGuardExists


    use testOkFile = fileguard cfg "test.ok"
    // module2.exe
    do! exec cfg ("."/"module2.exe") ""

    do! testOkFile |> NUnitConf.checkGuardExists


    use testOkFile = fileguard cfg "test.ok"
    // module2-opt.exe
    do! exec cfg ("."/"module2-opt.exe") ""

    do! testOkFile |> NUnitConf.checkGuardExists


    use testOkFile = fileguard cfg "test.ok"
    // module2-staticlink.exe
    do! exec cfg ("."/"module2-staticlink.exe") ""

    do! testOkFile |> NUnitConf.checkGuardExists
                
    })

[<Test; FSharpSuiteScriptPermutations("core/reflect")>]
let reflect p = singleTestBuildAndRun p

[<Test; FSharpSuiteTest("core/resources")>]
let testResources () = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    // REM Note that you have a VS SDK dependence here.
    // "%RESGEN%" /compile Resources.resx
    do! resgen cfg "/compile" ["Resources.resx"]

    // "%FSC%" %fsc_flags%  --resource:Resources.resources -o:test-embed.exe -g test.fs
    do! fsc cfg "%s  --resource:Resources.resources -o:test-embed.exe -g" cfg.fsc_flags ["test.fs"]

    do! peverify cfg "test-embed.exe"

    // "%FSC%" %fsc_flags%  --linkresource:Resources.resources -o:test-link.exe -g test.fs      
    do! fsc cfg "%s  --linkresource:Resources.resources -o:test-link.exe -g" cfg.fsc_flags ["test.fs"]

    do! peverify cfg "test-link.exe"

    // "%FSC%" %fsc_flags%  --resource:Resources.resources,ResourceName.resources -o:test-embed-named.exe -g test.fs      
    do! fsc cfg "%s  --resource:Resources.resources,ResourceName.resources -o:test-embed-named.exe -g" cfg.fsc_flags ["test.fs"]

    do! peverify cfg "test-embed-named.exe"

    // "%FSC%" %fsc_flags%  --linkresource:Resources.resources,ResourceName.resources -o:test-link-named.exe -g test.fs      
    do! fsc cfg "%s  --linkresource:Resources.resources,ResourceName.resources -o:test-link-named.exe -g" cfg.fsc_flags ["test.fs"]

    do! peverify cfg "test-link-named.exe"

    do! exec cfg ("."/"test-embed.exe") ""

    do! exec cfg ("."/"test-link.exe") ""

    do! exec cfg ("."/"test-link-named.exe") "ResourceName"

    do! exec cfg ("."/"test-embed-named.exe") "ResourceName"

                
    })


[<Test; FSharpSuiteScriptPermutations("core/seq")>]
let seq p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("core/subtype")>]
let subtype p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("core/syntax")>]
let syntax p = singleTestBuildAndRun p

[<Test; FSharpSuiteScriptPermutations("core/tlr")>]
let tlr p = singleTestBuildAndRun p

[<Test; FSharpSuiteTest("core/topinit")>]
let topinit () = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    do! fsc cfg "%s --optimize -o both69514.exe -g" cfg.fsc_flags ["lib69514.fs"; "app69514.fs"]

    do! peverify cfg "both69514.exe"

    do! fsc cfg "%s --optimize- -o both69514-noopt.exe -g" cfg.fsc_flags ["lib69514.fs"; "app69514.fs"]

    do! peverify cfg "both69514-noopt.exe"

    do! fsc cfg "%s --optimize -a -g" cfg.fsc_flags ["lib69514.fs"]

    do! peverify cfg "lib69514.dll"

    do! fsc cfg "%s --optimize -r:lib69514.dll -g" cfg.fsc_flags ["app69514.fs"]

    do! peverify cfg "app69514.exe"

    do! fsc cfg "%s --optimize- -o:lib69514-noopt.dll -a -g" cfg.fsc_flags ["lib69514.fs"]

    do! peverify cfg "lib69514-noopt.dll"

    do! fsc cfg "%s --optimize- -r:lib69514-noopt.dll -o:app69514-noopt.exe -g" cfg.fsc_flags ["app69514.fs"]

    do! peverify cfg "app69514-noopt.exe"

    do! fsc cfg "%s --optimize- -o:lib69514-noopt-withsig.dll -a -g" cfg.fsc_flags ["lib69514.fsi"; "lib69514.fs"]

    do! peverify cfg "lib69514-noopt-withsig.dll"

    do! fsc cfg "%s --optimize- -r:lib69514-noopt-withsig.dll -o:app69514-noopt-withsig.exe -g" cfg.fsc_flags ["app69514.fs"]

    do! peverify cfg "app69514-noopt-withsig.exe"

    do! fsc cfg "%s -o:lib69514-withsig.dll -a -g" cfg.fsc_flags ["lib69514.fsi"; "lib69514.fs"]

    do! peverify cfg "lib69514-withsig.dll"

    do! fsc cfg "%s -r:lib69514-withsig.dll -o:app69514-withsig.exe -g" cfg.fsc_flags ["app69514.fs"]

    do! peverify cfg "app69514-withsig.exe"

    do! fsc cfg "%s -o:lib.dll -a -g" cfg.fsc_flags ["lib.ml"]

    do! peverify cfg "lib.dll"

    do! csc cfg """/nologo /r:"%s" /r:lib.dll /out:test.exe """ cfg.FSCOREDLLPATH ["test.cs"]

    do! fsc cfg "%s --optimize -o:lib--optimize.dll -a -g" cfg.fsc_flags ["lib.ml"]

    do! peverify cfg "lib--optimize.dll"

    do! csc cfg """/nologo /r:"%s" /r:lib--optimize.dll /out:test--optimize.exe""" cfg.FSCOREDLLPATH ["test.cs"]

    let dicases = ["flag_deterministic_init1.fs"; "lib_deterministic_init1.fs"; "flag_deterministic_init2.fs"; "lib_deterministic_init2.fs"; "flag_deterministic_init3.fs"; "lib_deterministic_init3.fs"; "flag_deterministic_init4.fs"; "lib_deterministic_init4.fs"; "flag_deterministic_init5.fs"; "lib_deterministic_init5.fs"; "flag_deterministic_init6.fs"; "lib_deterministic_init6.fs"; "flag_deterministic_init7.fs"; "lib_deterministic_init7.fs"; "flag_deterministic_init8.fs"; "lib_deterministic_init8.fs"; "flag_deterministic_init9.fs"; "lib_deterministic_init9.fs"; "flag_deterministic_init10.fs"; "lib_deterministic_init10.fs"; "flag_deterministic_init11.fs"; "lib_deterministic_init11.fs"; "flag_deterministic_init12.fs"; "lib_deterministic_init12.fs"; "flag_deterministic_init13.fs"; "lib_deterministic_init13.fs"; "flag_deterministic_init14.fs"; "lib_deterministic_init14.fs"; "flag_deterministic_init15.fs"; "lib_deterministic_init15.fs"; "flag_deterministic_init16.fs"; "lib_deterministic_init16.fs"; "flag_deterministic_init17.fs"; "lib_deterministic_init17.fs"; "flag_deterministic_init18.fs"; "lib_deterministic_init18.fs"; "flag_deterministic_init19.fs"; "lib_deterministic_init19.fs"; "flag_deterministic_init20.fs"; "lib_deterministic_init20.fs"; "flag_deterministic_init21.fs"; "lib_deterministic_init21.fs"; "flag_deterministic_init22.fs"; "lib_deterministic_init22.fs"; "flag_deterministic_init23.fs"; "lib_deterministic_init23.fs"; "flag_deterministic_init24.fs"; "lib_deterministic_init24.fs"; "flag_deterministic_init25.fs"; "lib_deterministic_init25.fs"; "flag_deterministic_init26.fs"; "lib_deterministic_init26.fs"; "flag_deterministic_init27.fs"; "lib_deterministic_init27.fs"; "flag_deterministic_init28.fs"; "lib_deterministic_init28.fs"; "flag_deterministic_init29.fs"; "lib_deterministic_init29.fs"; "flag_deterministic_init30.fs"; "lib_deterministic_init30.fs"; "flag_deterministic_init31.fs"; "lib_deterministic_init31.fs"; "flag_deterministic_init32.fs"; "lib_deterministic_init32.fs"; "flag_deterministic_init33.fs"; "lib_deterministic_init33.fs"; "flag_deterministic_init34.fs"; "lib_deterministic_init34.fs"; "flag_deterministic_init35.fs"; "lib_deterministic_init35.fs"; "flag_deterministic_init36.fs"; "lib_deterministic_init36.fs"; "flag_deterministic_init37.fs"; "lib_deterministic_init37.fs"; "flag_deterministic_init38.fs"; "lib_deterministic_init38.fs"; "flag_deterministic_init39.fs"; "lib_deterministic_init39.fs"; "flag_deterministic_init40.fs"; "lib_deterministic_init40.fs"; "flag_deterministic_init41.fs"; "lib_deterministic_init41.fs"; "flag_deterministic_init42.fs"; "lib_deterministic_init42.fs"; "flag_deterministic_init43.fs"; "lib_deterministic_init43.fs"; "flag_deterministic_init44.fs"; "lib_deterministic_init44.fs"; "flag_deterministic_init45.fs"; "lib_deterministic_init45.fs"; "flag_deterministic_init46.fs"; "lib_deterministic_init46.fs"; "flag_deterministic_init47.fs"; "lib_deterministic_init47.fs"; "flag_deterministic_init48.fs"; "lib_deterministic_init48.fs"; "flag_deterministic_init49.fs"; "lib_deterministic_init49.fs"; "flag_deterministic_init50.fs"; "lib_deterministic_init50.fs"; "flag_deterministic_init51.fs"; "lib_deterministic_init51.fs"; "flag_deterministic_init52.fs"; "lib_deterministic_init52.fs"; "flag_deterministic_init53.fs"; "lib_deterministic_init53.fs"; "flag_deterministic_init54.fs"; "lib_deterministic_init54.fs"; "flag_deterministic_init55.fs"; "lib_deterministic_init55.fs"; "flag_deterministic_init56.fs"; "lib_deterministic_init56.fs"; "flag_deterministic_init57.fs"; "lib_deterministic_init57.fs"; "flag_deterministic_init58.fs"; "lib_deterministic_init58.fs"; "flag_deterministic_init59.fs"; "lib_deterministic_init59.fs"; "flag_deterministic_init60.fs"; "lib_deterministic_init60.fs"; "flag_deterministic_init61.fs"; "lib_deterministic_init61.fs"; "flag_deterministic_init62.fs"; "lib_deterministic_init62.fs"; "flag_deterministic_init63.fs"; "lib_deterministic_init63.fs"; "flag_deterministic_init64.fs"; "lib_deterministic_init64.fs"; "flag_deterministic_init65.fs"; "lib_deterministic_init65.fs"; "flag_deterministic_init66.fs"; "lib_deterministic_init66.fs"; "flag_deterministic_init67.fs"; "lib_deterministic_init67.fs"; "flag_deterministic_init68.fs"; "lib_deterministic_init68.fs"; "flag_deterministic_init69.fs"; "lib_deterministic_init69.fs"; "flag_deterministic_init70.fs"; "lib_deterministic_init70.fs"; "flag_deterministic_init71.fs"; "lib_deterministic_init71.fs"; "flag_deterministic_init72.fs"; "lib_deterministic_init72.fs"; "flag_deterministic_init73.fs"; "lib_deterministic_init73.fs"; "flag_deterministic_init74.fs"; "lib_deterministic_init74.fs"; "flag_deterministic_init75.fs"; "lib_deterministic_init75.fs"; "flag_deterministic_init76.fs"; "lib_deterministic_init76.fs"; "flag_deterministic_init77.fs"; "lib_deterministic_init77.fs"; "flag_deterministic_init78.fs"; "lib_deterministic_init78.fs"; "flag_deterministic_init79.fs"; "lib_deterministic_init79.fs"; "flag_deterministic_init80.fs"; "lib_deterministic_init80.fs"; "flag_deterministic_init81.fs"; "lib_deterministic_init81.fs"; "flag_deterministic_init82.fs"; "lib_deterministic_init82.fs"; "flag_deterministic_init83.fs"; "lib_deterministic_init83.fs"; "flag_deterministic_init84.fs"; "lib_deterministic_init84.fs"; "flag_deterministic_init85.fs"; "lib_deterministic_init85.fs"] 

    do! fsc cfg "%s --optimize- -o test_deterministic_init.exe" cfg.fsc_flags (dicases @ ["test_deterministic_init.fs"])

    do! peverify cfg "test_deterministic_init.exe"

    do! fsc cfg "%s --optimize -o test_deterministic_init--optimize.exe" cfg.fsc_flags (dicases @ ["test_deterministic_init.fs"])

    do! peverify cfg "test_deterministic_init--optimize.exe"

    do! fsc cfg "%s --optimize- -a -o test_deterministic_init_lib.dll" cfg.fsc_flags dicases

    do! peverify cfg "test_deterministic_init_lib.dll"

    do! fsc cfg "%s --optimize- -r test_deterministic_init_lib.dll -o test_deterministic_init_exe.exe" cfg.fsc_flags ["test_deterministic_init.fs"]

    do! peverify cfg "test_deterministic_init_exe.exe"

    do! fsc cfg "%s --optimize -a -o test_deterministic_init_lib--optimize.dll" cfg.fsc_flags dicases

    do! peverify cfg "test_deterministic_init_lib--optimize.dll"

    do! fsc cfg "%s --optimize -r test_deterministic_init_lib--optimize.dll -o test_deterministic_init_exe--optimize.exe" cfg.fsc_flags ["test_deterministic_init.fs"]

    do! peverify cfg "test_deterministic_init_exe--optimize.exe"

    let static_init_cases = [ "test0.fs"; "test1.fs"; "test2.fs"; "test3.fs"; "test4.fs"; "test5.fs"; "test6.fs" ]

    do! fsc cfg "%s --optimize- -o test_static_init.exe" cfg.fsc_flags (static_init_cases @ ["static-main.fs"])

    do! peverify cfg "test_static_init.exe"

    do! fsc cfg "%s --optimize -o test_static_init--optimize.exe" cfg.fsc_flags (static_init_cases @ [ "static-main.fs" ])

    do! peverify cfg "test_static_init--optimize.exe"

    do! fsc cfg "%s --optimize- -a -o test_static_init_lib.dll" cfg.fsc_flags static_init_cases

    do! peverify cfg "test_static_init_lib.dll"

    do! fsc cfg "%s --optimize- -r test_static_init_lib.dll -o test_static_init_exe.exe" cfg.fsc_flags ["static-main.fs"]

    do! peverify cfg "test_static_init_exe.exe"

    do! fsc cfg "%s --optimize -a -o test_static_init_lib--optimize.dll" cfg.fsc_flags static_init_cases

    do! peverify cfg "test_static_init_lib--optimize.dll"

    do! fsc cfg "%s --optimize -r test_static_init_lib--optimize.dll -o test_static_init_exe--optimize.exe" cfg.fsc_flags ["static-main.fs"]

    do! peverify cfg "test_static_init_exe--optimize.exe"

    do! exec cfg ("."/"test.exe") ""

    do! exec cfg ("."/"test--optimize.exe") ""

    do! exec cfg ("."/"test_deterministic_init.exe") ""

    do! exec cfg ("."/"test_deterministic_init--optimize.exe") ""

    do! exec cfg ("."/"test_deterministic_init_exe.exe") ""

    do! exec cfg ("."/"test_deterministic_init_exe--optimize.exe") ""

    do! exec cfg ("."/"test_static_init.exe") ""

    do! exec cfg ("."/"test_static_init--optimize.exe") ""

    do! exec cfg ("."/"test_static_init_exe.exe") ""

    do! exec cfg ("."/"test_static_init_exe--optimize.exe") ""
                
    })

[<Test; FSharpSuiteTest("core/unitsOfMeasure")>]
let unitsOfMeasure () = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    do! fsc cfg "%s --optimize- -o:test.exe -g" cfg.fsc_flags ["test.fs"]

    do! peverify cfg "test.exe"

    use testOkFile = fileguard cfg "test.ok"

    do! exec cfg ("."/"test.exe") ""

    do! testOkFile |> NUnitConf.checkGuardExists
                
    })


[<Test; FSharpSuiteTest("core/verify")>]
let verify () = check (attempt {
    let cfg = FSharpTestSuite.testConfig ()

    do! peverifyWithArgs cfg "/nologo" (cfg.FSCBinPath/"FSharp.Build.dll")

    // REM Use /MD because this contains some P/Invoke code  
    do! peverifyWithArgs cfg "/nologo /MD" (cfg.FSCBinPath/"FSharp.Compiler.dll")

    do! peverifyWithArgs cfg "/nologo" (cfg.FSCBinPath/"fsi.exe")

    do! peverifyWithArgs cfg "/nologo" (cfg.FSCBinPath/"FSharp.Compiler.Interactive.Settings.dll")

    // "%FSC%" %fsc_flags% -o:xmlverify.exe -g xmlverify.fs
    do! fsc cfg "%s -o:xmlverify.exe -g" cfg.fsc_flags ["xmlverify.fs"]

    do! peverifyWithArgs cfg "/nologo" "xmlverify.exe"
    })
