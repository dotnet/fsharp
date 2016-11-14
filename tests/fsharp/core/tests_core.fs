module ``FSharp-Tests-Core``

open System
open System.IO
open NUnit.Framework

open NUnitConf
open PlatformHelpers
open FSharpTestSuiteTypes
open SingleTest

[<Test>]
let ``access fsi``() = singleTestBuildAndRun "core/access" FSI_FILE

[<Test>]
let ``access fsc optimized``() = singleTestBuildAndRun "core/access" FSC_OPT_PLUS_DEBUG

[<Test>]
let apporder () = singleTestBuildAndRun "core/apporder" FSC_OPT_PLUS_DEBUG

[<Test>]
let array () = singleTestBuildAndRun "core/array" FSC_OPT_PLUS_DEBUG

[<Test>]
let attributes () = singleTestBuildAndRun "core/attributes" FSC_OPT_PLUS_DEBUG

[<Test>]
let byrefs () = check  (attempt {

    let cfg = FSharpTestSuite.testConfig "core/byrefs"

    use testOkFile = fileguard cfg "test.ok"

    do! fsc cfg "%s -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

    do! exec cfg ("."/"test.exe") ""

    do! testOkFile |> NUnitConf.checkGuardExists

    do! fsi cfg "" ["test.fsx"]

    do! testOkFile |> NUnitConf.checkGuardExists

    })

[<Test>]
let comprehensions () = singleTestBuildAndRun "core/comprehensions" FSC_OPT_PLUS_DEBUG

[<Test>]
let comprehensionshw () = singleTestBuildAndRun "core/comprehensions-hw" FSC_OPT_PLUS_DEBUG

[<Test>]
let control () = singleTestBuildAndRun "core/control" FSC_OPT_PLUS_DEBUG

[<Test>]
let ``control --tailcalls`` () = check  (attempt {
    let cfg = FSharpTestSuite.testConfig "core/control"
        
    do! singleTestBuildAndRunAux {cfg with fsi_flags = " --tailcalls" } FSC_OPT_PLUS_DEBUG
    })

[<Test>]
let controlChamenos () = check  (attempt {
    let cfg = FSharpTestSuite.testConfig "core/controlChamenos"
        
    do! singleTestBuildAndRunAux {cfg with fsi_flags = " --tailcalls" } FSC_OPT_PLUS_DEBUG
    })

[<Test>]
let controlMailbox () = singleTestBuildAndRun "core/controlMailbox" FSC_OPT_PLUS_DEBUG

[<Test>]
let ``controlMailbox --tailcalls`` () = check  (attempt {
    let cfg = FSharpTestSuite.testConfig "core/controlMailbox"
        
    do! singleTestBuildAndRunAux {cfg with fsi_flags = " --tailcalls" } FSC_OPT_PLUS_DEBUG
    })

[<Test>]
let controlWpf () = singleTestBuildAndRun "core/controlwpf" FSC_OPT_PLUS_DEBUG

[<Test>]
let csext () = singleTestBuildAndRun "core/csext" FSC_OPT_PLUS_DEBUG


[<Test>]
let events () = check  (attempt {
    let cfg = FSharpTestSuite.testConfig "core/events"

    do! fsc cfg "%s -a -o:test.dll -g" cfg.fsc_flags ["test.fs"]

    do! peverify cfg "test.dll"

    do! csc cfg """/r:"%s" /reference:test.dll /debug+""" cfg.FSCOREDLLPATH ["testcs.cs"]

    do! peverify cfg "testcs.exe"
        
    use testOkFile = fileguard cfg "test.ok"

    do! fsi cfg "" ["test.fs"]

    do! testOkFile |> NUnitConf.checkGuardExists

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
//    [<FSharpSuiteTestCase("core/fsi-shadowcopy", "")
//    // "%FSI%" %fsi_flags%  --shadowcopyreferences- < test1.fsx
//    [<FSharpSuiteTestCase("core/fsi-shadowcopy", "--shadowcopyreferences-")
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
//    [<FSharpSuiteTestCase("core/fsi-shadowcopy", "/shadowcopyreferences+")
//    // "%FSI%" %fsi_flags%  --shadowcopyreferences  < test2.fsx
//    [<FSharpSuiteTestCase("core/fsi-shadowcopy", "--shadowcopyreferences")
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

    

[<Test>]
let forwarders () = check (attempt {
    let cfg = FSharpTestSuite.testConfig "core/forwarders"

    mkdir cfg "orig"
    mkdir cfg "split"

    do! csc cfg """/nologo  /target:library /out:orig\a.dll /define:PART1;PART2""" ["a.cs"]

    do! csc cfg """/nologo  /target:library /out:orig\b.dll /r:orig\a.dll""" ["b.cs"]

    do! fsc cfg """-a -o:orig\c.dll -r:orig\b.dll -r:orig\a.dll""" ["c.fs"]

    do! csc cfg """/nologo  /target:library /out:split\a-part1.dll /define:PART1;SPLIT""" ["a.cs"]

    do! csc cfg """/nologo  /target:library /r:split\a-part1.dll /out:split\a.dll /define:PART2;SPLIT""" ["a.cs"]

    do! copy_y cfg ("orig"/"b.dll") ("split"/"b.dll")

    do! copy_y cfg ("orig"/"c.dll") ("split"/"c.dll")

    do! fsc cfg """-o:orig\test.exe -r:orig\b.dll -r:orig\a.dll""" ["test.fs"]

    do! fsc cfg """-o:split\test.exe -r:split\b.dll -r:split\a-part1.dll -r:split\a.dll""" ["test.fs"]

    do! fsc cfg """-o:split\test-against-c.exe -r:split\c.dll -r:split\a-part1.dll -r:split\a.dll""" ["test.fs"]

    do! peverify cfg ("split"/"a-part1.dll")

    do! peverify cfg ("split"/"b.dll")

    do! peverify cfg ("split"/"c.dll")

    do! peverify cfg ("split"/"test.exe")

    do! peverify cfg ("split"/"test-against-c.exe")

    })

[<Test>]
let fsfromcs () = check (attempt {
    let cfg = FSharpTestSuite.testConfig "core/fsfromcs"

    do! fsc cfg "%s -a --doc:lib.xml -o:lib.dll -g" cfg.fsc_flags ["lib.fs"]

    do! peverify cfg "lib.dll"

    do! csc cfg """/nologo /r:"%s" /r:System.Core.dll /r:lib.dll /out:test.exe""" cfg.FSCOREDLLPATH ["test.cs"]

    do! fsc cfg """%s -a --doc:lib--optimize.xml -o:lib--optimize.dll -g""" cfg.fsc_flags ["lib.fs"]

    do! peverify cfg "lib--optimize.dll"

    do! csc cfg """/nologo /r:"%s"  /r:System.Core.dll /r:lib--optimize.dll    /out:test--optimize.exe""" cfg.FSCOREDLLPATH ["test.cs"]

    do! exec cfg ("."/"test.exe") ""

    do! exec cfg ("."/"test--optimize.exe") ""
                
    })

[<Test>]
let fsfromfsviacs () = check (attempt {
    let cfg = FSharpTestSuite.testConfig "core/fsfromfsviacs"

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


[<Test>]
let ``fsi-reload`` () = check (attempt {
    let cfg = FSharpTestSuite.testConfig "core/fsi-reload"

    do! attempt {

        use testOkFile = fileguard cfg "test.ok"

        do! ``fsi <`` cfg "%s  --maxerrors:1" cfg.fsi_flags "test1.ml"
    
        do! testOkFile |> NUnitConf.checkGuardExists
    }
                
    do! attempt {

        use testOkFile = fileguard cfg "test.ok"

        do! fsi cfg "%s  --maxerrors:1" cfg.fsi_flags ["load1.fsx"]
    
        do! testOkFile |> NUnitConf.checkGuardExists
        }

    do! attempt {

        use testOkFile = fileguard cfg "test.ok"

        do! fsi cfg "%s  --maxerrors:1" cfg.fsi_flags ["load2.fsx"]
    
        do! testOkFile |> NUnitConf.checkGuardExists
    }

    do! fsc cfg "" ["load1.fsx"]

    do! fsc cfg "" ["load2.fsx"]

    })


[<Test>]
let fsiAndModifiers () = check (attempt {
    let cfg = FSharpTestSuite.testConfig "core/fsiAndModifiers"

    do if fileExists cfg "TestLibrary.dll" then rm cfg "TestLibrary.dll"

    do! ``fsi <`` cfg "%s  --maxerrors:1" cfg.fsi_flags "prepare.fsx"

    use testOkFile = fileguard cfg "test.ok"
        
    do! ``fsi <`` cfg "%s  --maxerrors:1" cfg.fsi_flags "test.fsx"

    do! testOkFile |> NUnitConf.checkGuardExists
                
    })

[<Test>]
let genericmeasures () = 
    for p in codeAndInferencePermutations do
        singleTestBuildAndRun "core/genericmeasures" p

[<Test>]
let hiding () = check (attempt {
    let cfg = FSharpTestSuite.testConfig "core/hiding"

    do! fsc cfg "%s -a --optimize -o:lib.dll" cfg.fsc_flags ["lib.mli";"lib.ml";"libv.ml"]

    do! peverify cfg "lib.dll"

    do! fsc cfg "%s -a --optimize -r:lib.dll -o:lib2.dll" cfg.fsc_flags ["lib2.mli";"lib2.ml";"lib3.ml"]

    do! peverify cfg "lib2.dll"

    do! fsc cfg "%s --optimize -r:lib.dll -r:lib2.dll -o:client.exe" cfg.fsc_flags ["client.ml"]

    do! peverify cfg "client.exe"

    })


[<Test>]
let innerpoly () = 
    for p in codeAndInferencePermutations do
        singleTestBuildAndRun "core/innerpoly" p
        
[<Test>]
let ``test int32`` () = singleTestBuildAndRun "core/int32" FSC_OPT_PLUS_DEBUG

[<Test>]
let queriesCustomQueryOps () = check (attempt {
    let cfg = FSharpTestSuite.testConfig "core/queriesCustomQueryOps"

    do! fsc cfg """%s -o:test.exe -g""" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "test.exe"

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

let printing flag diffFileOut expectedFileOut diffFileErr expectedFileErr = check (attempt {
    let cfg = FSharpTestSuite.testConfig "core/printing"

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

[<Test>]
let ``printing-1`` () = 
     printing "" "z.output.test.default.stdout.txt" "z.output.test.default.stdout.bsl" "z.output.test.default.stderr.txt" "z.output.test.default.stderr.bsl"

[<Test>]
let ``printing-2`` () = 
     printing "--use:preludePrintSize1000.fsx" "z.output.test.1000.stdout.txt" "z.output.test.1000.stdout.bsl" "z.output.test.1000.stderr.txt" "z.output.test.1000.stderr.bsl"

[<Test>]
let ``printing-3`` () = 
     printing "--use:preludePrintSize200.fsx" "z.output.test.200.stdout.txt" "z.output.test.200.stdout.bsl" "z.output.test.200.stderr.txt" "z.output.test.200.stderr.bsl"

[<Test>]
let ``printing-4`` () = 
     printing "--use:preludeShowDeclarationValuesFalse.fsx" "z.output.test.off.stdout.txt" "z.output.test.off.stdout.bsl" "z.output.test.off.stderr.txt" "z.output.test.off.stderr.bsl"

[<Test>]
let ``printing-5`` () = 
     printing "--quiet" "z.output.test.quiet.stdout.txt" "z.output.test.quiet.stdout.bsl" "z.output.test.quiet.stderr.txt" "z.output.test.quiet.stderr.bsl"


let signedtest(args,bslfile) = check(attempt {
    
    let cfg = FSharpTestSuite.testConfig "core/signedtests"
    let cfg = { cfg with fsc_flags=cfg.fsc_flags + " " + args }

    let outfile = Path.ChangeExtension(bslfile,"sn.out") 
    let exefile = Path.ChangeExtension(bslfile,"exe") 
    do File.WriteAllLines(getfullpath cfg outfile,
                          ["sn -q stops all output except error messages                "
                           "if the output is a valid file no output is produced.       "
                           "delay-signed and unsigned produce error messages.          "])

    do! fsc cfg "%s -o:%s" cfg.fsc_flags exefile ["test.fs"]
    do! sn cfg outfile ("-q -vf "+exefile) 
    let! diffs = fsdiff cfg outfile bslfile 

    do! match diffs with
        | [] -> Success
        | _ -> NUnitConf.genericError (sprintf "'%s' and '%s' differ; %A" outfile bslfile diffs)
   })

[<Test; Category("signedtest")>]
let ``signedtest-1`` () = signedtest("","test-unsigned.bsl")

[<Test; Category("signedtest")>]
let ``signedtest-2`` () = signedtest("--keyfile:sha1full.snk", "test-sha1-full-cl.bsl")

[<Test; Category("signedtest")>]
let ``signedtest-3`` () = signedtest("--keyfile:sha256full.snk", "test-sha256-full-cl.bsl")

[<Test; Category("signedtest")>]
let ``signedtest-4`` () = signedtest("--keyfile:sha512full.snk", "test-sha512-full-cl.bsl")

[<Test; Category("signedtest")>]
let ``signedtest-5`` () = signedtest("--keyfile:sha1024full.snk", "test-sha1024-full-cl.bsl")

[<Test; Category("signedtest")>]
let ``signedtest-6`` () = signedtest("--keyfile:sha1delay.snk --delaysign", "test-sha1-delay-cl.bsl")

[<Test; Category("signedtest")>]
let ``signedtest-7`` () = signedtest("--keyfile:sha256delay.snk --delaysign", "test-sha256-delay-cl.bsl")

[<Test; Category("signedtest")>]
let ``signedtest-8`` () = signedtest("--keyfile:sha512delay.snk --delaysign", "test-sha512-delay-cl.bsl")

[<Test; Category("signedtest")>]
let ``signedtest-9`` () = signedtest("--keyfile:sha1024delay.snk --delaysign", "test-sha1024-delay-cl.bsl")

// Test SHA1 key full signed  Attributes
[<Test; Category("signedtest")>]
let ``signedtest-10`` () = signedtest("--define:SHA1","test-sha1-full-attributes.bsl")

// Test SHA1 key delayl signed  Attributes
[<Test; Category("signedtest")>]
let ``signedtest-11`` () = signedtest("--keyfile:sha1delay.snk --define:SHA1 --define:DELAY", "test-sha1-delay-attributes.bsl")

[<Test; Category("signedtest")>]
let ``signedtest-12`` () = signedtest("--define:SHA256", "test-sha256-full-attributes.bsl")

// Test SHA 256 bit key delay signed  Attributes
[<Test; Category("signedtest")>]
let ``signedtest-13`` () = signedtest("--define:SHA256 --define:DELAY", "test-sha256-delay-attributes.bsl")

// Test SHA 512 bit key fully signed  Attributes
[<Test; Category("signedtest")>]
let ``signedtest-14`` () = signedtest("--define:SHA512", "test-sha512-full-attributes.bsl")

// Test SHA 512 bit key delay signed Attributes
[<Test; Category("signedtest")>]
let ``signedtest-15`` () = signedtest("--define:SHA512 --define:DELAY", "test-sha512-delay-attributes.bsl")

// Test SHA 1024 bit key fully signed  Attributes
[<Test; Category("signedtest")>]
let ``signedtest-16`` () = signedtest("--define:SHA1024", "test-sha1024-full-attributes.bsl")

// Test dumpbin with SHA 1024 bit key public signed CL
[<Test; Category("signedtest")>]
let ``signedtest-17`` () = signedtest("--keyfile:sha1024delay.snk --publicsign", "test-sha1024-public-cl.bsl")

[<Test>]
let quotes () = check (attempt {
    let cfg = FSharpTestSuite.testConfig "core/quotes"

    do! csc cfg """/nologo  /target:library /out:cslib.dll""" ["cslib.cs"]

    do! fsc cfg "%s -o:test.exe -r cslib.dll -g" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "test.exe"

    do! fsc cfg "%s -o:test-with-debug-data.exe --quotations-debug+ -r cslib.dll -g" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "test-with-debug-data.exe"

    do! fsc cfg "%s --optimize -o:test--optimize.exe -r cslib.dll -g" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "test--optimize.exe"

    do! attempt {
        use testOkFile = fileguard cfg "test.ok"

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

        do! exec cfg ("."/"test-with-debug-data.exe") ""


        do! testOkFile |> NUnitConf.checkGuardExists
        }

    do! attempt {

        use testOkFile = fileguard cfg "test.ok"

        do! exec cfg ("."/"test--optimize.exe") ""


        do! testOkFile |> NUnitConf.checkGuardExists
        }
                
    })


[<Test; Category("namespaces")>]
let namespaceAttributes () = 
    for p in codeAndInferencePermutations do
         singleTestBuildAndRun "core/namespaces" p

[<Test; Category("parsing")>]
let parsing () = check  (attempt {
    let cfg = FSharpTestSuite.testConfig "core/parsing"
        
    do! fsc cfg "%s -a -o:crlf.dll -g" cfg.fsc_flags ["crlf.ml"]

    do! fsc cfg "%s -o:toplet.exe -g" cfg.fsc_flags ["toplet.ml"]

    do! peverify cfg "toplet.exe"

    }) 

[<Test; Category("unicode")>]
let unicode () = check  (attempt {
    let cfg = FSharpTestSuite.testConfig "core/unicode"

    do! fsc cfg "%s -a -o:kanji-unicode-utf8-nosig-codepage-65001.dll -g" cfg.fsc_flags ["kanji-unicode-utf8-nosig-codepage-65001.fs"]

    do! fsc cfg "%s -a -o:kanji-unicode-utf8-nosig-codepage-65001.dll -g" cfg.fsc_flags ["kanji-unicode-utf8-nosig-codepage-65001.fs"]

    let codepage = attempt {
        do! fsc cfg "%s -a -o:kanji-unicode-utf16.dll -g" cfg.fsc_flags ["kanji-unicode-utf16.fs"]

        do! fsc cfg "%s -a --codepage:65000 -o:kanji-unicode-utf7-codepage-65000.dll -g" cfg.fsc_flags ["kanji-unicode-utf7-codepage-65000.fs"]
        }

    do! if not <| cfg.FSC.Contains("fscp") then codepage else Success

    do! fsc cfg "%s -a -o:kanji-unicode-utf8-withsig-codepage-65001.dll -g" cfg.fsc_flags ["kanji-unicode-utf8-withsig-codepage-65001.fs"]

    do! fsi cfg "%s --utf8output" cfg.fsi_flags ["kanji-unicode-utf8-nosig-codepage-65001.fs"]

    do! fsi cfg "%s --utf8output --codepage:65001" cfg.fsi_flags ["kanji-unicode-utf8-withsig-codepage-65001.fs"]

    do! fsi cfg "%s --utf8output" cfg.fsi_flags ["kanji-unicode-utf8-withsig-codepage-65001.fs"]

    do! fsi cfg "%s --utf8output --codepage:65000" cfg.fsi_flags ["kanji-unicode-utf7-codepage-65000.fs"]

    do! fsi cfg "%s --utf8output" cfg.fsi_flags ["kanji-unicode-utf16.fs"]
    }) 

[<Test; Category("unicode")>]
let unicode2 () = singleTestBuildAndRun "core/unicode" FSC_OPT_PLUS_DEBUG

[<Test; Category("internalsvisible")>]
let internalsvisible () = check  (attempt {
    let cfg = FSharpTestSuite.testConfig "core/internalsvisible"

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
[<Test; Category("fileorder")>]
let fileorder () = check  (attempt {
    let cfg = FSharpTestSuite.testConfig "core/fileorder"

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


[<Test; Category("lazy")>]
let ``lazy test`` () = singleTestBuildAndRun "core/lazy" FSC_OPT_PLUS_DEBUG

[<Test; Category("letrec")>]
let letrec () = singleTestBuildAndRun "core/letrec" FSC_OPT_PLUS_DEBUG

[<Test; Category("letrec")>]
let ``letrec (mutrec variations part one)`` () = singleTestBuildAndRun "core/letrec-mutrec" FSC_OPT_PLUS_DEBUG

[<Test; Category("letrec")>]
let ``letrec (mutrec variations part two)`` () = singleTestBuildAndRun "core/letrec-mutrec2" FSC_OPT_PLUS_DEBUG

[<Test; Category("libtest")>]
let libtest () = 
    for p in allPermutations do
        singleTestBuildAndRun "core/libtest" p

[<Test; Category("lift")>]
let lift () = singleTestBuildAndRun "core/lift" FSC_OPT_PLUS_DEBUG


[<Test>]
let ``load-script`` () = check (attempt {
    let cfg = FSharpTestSuite.testConfig "core/load-script"

    let stdoutPath = "out.stdout.txt" |> getfullpath cfg
    let stderrPath = "out.stderr.txt" |> getfullpath cfg
    let stderrBaseline = "out.stderr.bsl" |> getfullpath cfg 
    let stdoutBaseline = "out.stdout.bsl" |> getfullpath cfg 

    let appendToFile from = Commands.appendToFile cfg.Directory from stdoutPath
    let echo text = Commands.echo_append_tofile cfg.Directory text stdoutPath

    File.WriteAllText(stdoutPath, "")
    File.WriteAllText(stderrPath, "")

    do if fileExists cfg "3.exe" then getfullpath cfg "3.exe" |> File.Delete

    ["1.fsx"; "2.fsx"; "3.fsx"] |> List.iter appendToFile

    echo "Test 1================================================="

    do! fscAppend cfg stdoutPath stderrPath "--nologo" ["3.fsx"]

    do! execAppendIgnoreExitCode cfg stdoutPath stderrPath ("."/"3.exe") ""

    rm cfg "3.exe"

    echo "Test 2================================================="

    do! fsiAppendIgnoreExitCode cfg stdoutPath stderrPath "" ["3.fsx"]

    echo "Test 3================================================="

    do! fsiFromInToOutIgnoreExitCode cfg stdoutPath stderrPath "--nologo" "pipescr"

    echo "Test 4================================================="

    do! fsiAppendIgnoreExitCode cfg stdoutPath stderrPath "" ["usesfsi.fsx"]

    echo "Test 5================================================="

    do! fscAppendIgnoreExitCode cfg stdoutPath stderrPath "--nologo" ["usesfsi.fsx"]

    echo "Test 6================================================="

    do! fscAppend cfg stdoutPath stderrPath "--nologo -r FSharp.Compiler.Interactive.Settings" ["usesfsi.fsx"]

    echo "Test 7================================================="

    do! fsiAppendIgnoreExitCode cfg stdoutPath stderrPath "" ["1.fsx";"2.fsx";"3.fsx"]

    echo "Test 8================================================="

    do! fsiAppendIgnoreExitCode cfg stdoutPath stderrPath "" ["3.fsx";"2.fsx";"1.fsx"]

    echo "Test 9================================================="

    do! fsiAppendIgnoreExitCode cfg stdoutPath stderrPath "" ["multiple-load-1.fsx"]

    echo "Test 10================================================="

    do! fsiAppendIgnoreExitCode cfg stdoutPath stderrPath "" ["multiple-load-2.fsx"]

    echo "Test 11================================================="

    do! fscAppend cfg stdoutPath stderrPath "--nologo" ["FlagCheck.fs"]

    do! execAppendIgnoreExitCode cfg stdoutPath stderrPath ("."/"FlagCheck.exe") ""

    rm cfg "FlagCheck.exe"

    echo "Test 12================================================="

    do! fscAppend cfg stdoutPath stderrPath "-o FlagCheckScript.exe --nologo" ["FlagCheck.fsx"]

    do! execAppendIgnoreExitCode cfg stdoutPath stderrPath ("."/"FlagCheckScript.exe") ""

    rm cfg "FlagCheckScript.exe"

    echo "Test 13================================================="

    do! fsiAppendIgnoreExitCode cfg stdoutPath stderrPath "" ["load-FlagCheckFs.fsx"]

    echo "Test 14================================================="

    do! fsiAppendIgnoreExitCode cfg stdoutPath stderrPath "" ["FlagCheck.fsx"]

    echo "Test 15================================================="

    do! fsiAppendIgnoreExitCode cfg stdoutPath stderrPath "" ["ProjectDriver.fsx"]

    echo "Test 16================================================="

    do! fscAppend cfg stdoutPath stderrPath "--nologo" ["ProjectDriver.fsx"]

    do! execAppendIgnoreExitCode cfg stdoutPath stderrPath ("."/"ProjectDriver.exe") ""

    rm cfg "ProjectDriver.exe"

    echo "Test 17================================================="

    do! fsiAppendIgnoreExitCode cfg stdoutPath stderrPath "" ["load-IncludeNoWarn211.fsx"]

    echo "Done =================================================="

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

[<Test; Category("longnames")>]
let longnames () = singleTestBuildAndRun "core/longnames" FSC_OPT_PLUS_DEBUG

[<Test; Category("map")>]
let map () = singleTestBuildAndRun "core/map" FSC_OPT_PLUS_DEBUG

[<Test; Category("numbers")>]
let numbers () = singleTestBuildAndRun "core/math/numbers" FSC_OPT_PLUS_DEBUG

[<Test; Category("numbersVS2008")>]
let numbersVS2008 () = singleTestBuildAndRun "core/math/numbersVS2008" FSC_OPT_PLUS_DEBUG

[<Test; Category("measures")>]
let measures () = 
    for p in codeAndInferencePermutations do
        singleTestBuildAndRun "core/measures" p

[<Test; Category("basics")>]
let basics () = 
    for p in codeAndInferencePermutations do
        singleTestBuildAndRun "core/members/basics" p

[<Test>]
let ``basics-hw`` () = singleTestBuildAndRun "core/members/basics-hw" FSC_OPT_PLUS_DEBUG

[<Test>]
let BasicsHwMutrec () = singleTestBuildAndRun "core/members/basics-hw-mutrec" FSC_OPT_PLUS_DEBUG

[<Test>]
let ctree () = singleTestBuildAndRun "core/members/ctree" FSC_OPT_PLUS_DEBUG

[<Test>]
let factors () = singleTestBuildAndRun "core/members/factors" FSC_OPT_PLUS_DEBUG

[<Test>]
let factorsMutrec () = singleTestBuildAndRun "core/members/factors-mutrec" FSC_OPT_PLUS_DEBUG

[<Test>]
let incremental () = singleTestBuildAndRun "core/members/incremental" FSC_OPT_PLUS_DEBUG

[<Test>]
let incrementalHw () = singleTestBuildAndRun "core/members/incremental-hw" FSC_OPT_PLUS_DEBUG

[<Test>]
let incrementalHwMutrec () = singleTestBuildAndRun "core/members/incremental-hw-mutrec" FSC_OPT_PLUS_DEBUG

[<Test>]
let ops () = singleTestBuildAndRun "core/members/ops" FSC_OPT_PLUS_DEBUG

[<Test>]
let opsMutrec () = singleTestBuildAndRun "core/members/ops-mutrec" FSC_OPT_PLUS_DEBUG

[<Test>]
let nested () = singleTestBuildAndRun "core/nested" FSC_OPT_PLUS_DEBUG

[<Test>]
let patterns () = singleTestBuildAndRun "core/patterns" FSC_OPT_PLUS_DEBUG

[<Test>]
let pinvoke () = check (attempt {
    let cfg = FSharpTestSuite.testConfig "core/pinvoke"

    do! fsc cfg "%s -o:test.exe -g" cfg.fsc_flags ["test.fsx"]
   
    do! peverifyWithArgs cfg "/nologo /MD" "test.exe"
                
    })


[<Test>]
let printf () = singleTestBuildAndRun "core/printf" FSC_BASIC


[<Test>]
let queriesLeafExpressionConvert () = check (attempt {
    let cfg = FSharpTestSuite.testConfig "core/queriesLeafExpressionConvert"

    do! fsc cfg "%s -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "test.exe"

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


[<Test>]
let queriesNullableOperators () = check (attempt {
    let cfg = FSharpTestSuite.testConfig "core/queriesNullableOperators"

    do! fsc cfg "%s -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "test.exe"

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

[<Test>]
let queriesOverIEnumerable () = check (attempt {
    let cfg = FSharpTestSuite.testConfig "core/queriesOverIEnumerable"

    do! fsc cfg "%s -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "test.exe" 

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

[<Test>]
let queriesOverIQueryable () = check (attempt {
    let cfg = FSharpTestSuite.testConfig "core/queriesOverIQueryable"

    do! fsc cfg "%s -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "test.exe"

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


[<Test>]
let quotesDebugInfo () = check (attempt {
    let cfg = FSharpTestSuite.testConfig "core/quotesDebugInfo"

    do! fsc cfg "%s --quotations-debug+ --optimize -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "test.exe"

    do! fsc cfg "%s --quotations-debug+ --optimize -o:test--optimize.exe -g" cfg.fsc_flags ["test.fsx"]

    do! peverify cfg "test--optimize.exe"

    use testOkFile = fileguard cfg "test.ok"
    do! fsi cfg "%s --quotations-debug+" cfg.fsi_flags ["test.fsx"]

    do! testOkFile |> NUnitConf.checkGuardExists


    use testOkFile2 = fileguard cfg "test.ok"

    do! exec cfg ("."/"test.exe") ""

    do! testOkFile2 |> NUnitConf.checkGuardExists

    use testOkFile3 = fileguard cfg "test.ok"

    do! exec cfg ("."/"test--optimize.exe") ""

    do! testOkFile3 |> NUnitConf.checkGuardExists
                
    })

[<Test>]
let quotesInMultipleModules () = check (attempt {
    let cfg = FSharpTestSuite.testConfig "core/quotesInMultipleModules"

    do! fsc cfg "%s -o:module1.dll --target:library" cfg.fsc_flags ["module1.fsx"]

    do! peverify cfg "module1.dll"

    do! fsc cfg "%s -o:module2.exe -r:module1.dll" cfg.fsc_flags ["module2.fsx"]

    do! peverify cfg "module2.exe"
    
    do! fsc cfg "%s --staticlink:module1 -o:module2-staticlink.exe -r:module1.dll" cfg.fsc_flags ["module2.fsx"]

    do! peverify cfg "module2-staticlink.exe"

    do! fsc cfg "%s -o:module1-opt.dll --target:library --optimize" cfg.fsc_flags ["module1.fsx"]

    do! peverify cfg "module1-opt.dll"

    do! fsc cfg "%s -o:module2-opt.exe -r:module1-opt.dll --optimize" cfg.fsc_flags ["module2.fsx"]

    do! peverify cfg "module2-opt.exe"

    use testOkFile = fileguard cfg "test.ok"

    do! fsi cfg "%s -r module1.dll" cfg.fsi_flags ["module2.fsx"]

    do! testOkFile |> NUnitConf.checkGuardExists


    use testOkFile = fileguard cfg "test.ok"

    do! exec cfg ("."/"module2.exe") ""

    do! testOkFile |> NUnitConf.checkGuardExists

    use testOkFile = fileguard cfg "test.ok"

    do! exec cfg ("."/"module2-opt.exe") ""

    do! testOkFile |> NUnitConf.checkGuardExists

    use testOkFile = fileguard cfg "test.ok"

    do! exec cfg ("."/"module2-staticlink.exe") ""

    do! testOkFile |> NUnitConf.checkGuardExists
                
    })

[<Test>]
let reflect () = singleTestBuildAndRun "core/reflect" FSC_OPT_PLUS_DEBUG

[<Test>]
let testResources () = check (attempt {
    let cfg = FSharpTestSuite.testConfig "core/resources"

    do! fsc cfg "%s  --resource:Resources.resources -o:test-embed.exe -g" cfg.fsc_flags ["test.fs"]

    do! peverify cfg "test-embed.exe"

    do! fsc cfg "%s  --linkresource:Resources.resources -o:test-link.exe -g" cfg.fsc_flags ["test.fs"]

    do! peverify cfg "test-link.exe"

    do! fsc cfg "%s  --resource:Resources.resources,ResourceName.resources -o:test-embed-named.exe -g" cfg.fsc_flags ["test.fs"]

    do! peverify cfg "test-embed-named.exe"

    do! fsc cfg "%s  --linkresource:Resources.resources,ResourceName.resources -o:test-link-named.exe -g" cfg.fsc_flags ["test.fs"]

    do! peverify cfg "test-link-named.exe"

    do! exec cfg ("."/"test-embed.exe") ""

    do! exec cfg ("."/"test-link.exe") ""

    do! exec cfg ("."/"test-link-named.exe") "ResourceName"

    do! exec cfg ("."/"test-embed-named.exe") "ResourceName"

                
    })


[<Test>]
let seq () = singleTestBuildAndRun "core/seq" FSC_OPT_PLUS_DEBUG

[<Test>]
let subtype () = singleTestBuildAndRun "core/subtype" FSC_OPT_PLUS_DEBUG

[<Test>]
let syntax () = singleTestBuildAndRun "core/syntax" FSC_OPT_PLUS_DEBUG

[<Test>]
let tlr () = singleTestBuildAndRun "core/tlr" FSC_OPT_PLUS_DEBUG

[<Test>]
let topinit () = check (attempt {
    let cfg = FSharpTestSuite.testConfig "core/topinit"

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

[<Test>]
let unitsOfMeasure () = check (attempt {
    let cfg = FSharpTestSuite.testConfig "core/unitsOfMeasure"

    do! fsc cfg "%s --optimize- -o:test.exe -g" cfg.fsc_flags ["test.fs"]

    do! peverify cfg "test.exe"

    use testOkFile = fileguard cfg "test.ok"

    do! exec cfg ("."/"test.exe") ""

    do! testOkFile |> NUnitConf.checkGuardExists
                
    })


[<Test>]
let verify () = check (attempt {
    let cfg = FSharpTestSuite.testConfig "core/verify"

    do! peverifyWithArgs cfg "/nologo" (cfg.FSCBinPath/"FSharp.Build.dll")

   // do! peverifyWithArgs cfg "/nologo /MD" (cfg.FSCBinPath/"FSharp.Compiler.dll")

    do! peverifyWithArgs cfg "/nologo" (cfg.FSCBinPath/"fsi.exe")

    do! peverifyWithArgs cfg "/nologo" (cfg.FSCBinPath/"FSharp.Compiler.Interactive.Settings.dll")

    do! fsc cfg "%s -o:xmlverify.exe -g" cfg.fsc_flags ["xmlverify.fs"]

    do! peverifyWithArgs cfg "/nologo" "xmlverify.exe"
    })

[<Test>]
let graph () = singleTestBuildAndRun "perf/graph" FSC_OPT_PLUS_DEBUG

[<Test>]
let nbody () = singleTestBuildAndRun "perf/nbody" FSC_OPT_PLUS_DEBUG
