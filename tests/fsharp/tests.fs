#if INTERACTIVE
//#r @"../../release/net40/bin/FSharp.Compiler.dll"
#r @"../../packages/NUnit.3.5.0/lib/net45/nunit.framework.dll"
#load "../../src/scripts/scriptlib.fsx" 
#load "test-framework.fs" 
#load "single-test.fs"
#else
module ``FSharp-Tests-Core``
#endif

open System
open System.IO
open System.Reflection
open NUnit.Framework
open TestFramework
open Scripting
open SingleTest

#if FSHARP_SUITE_DRIVES_CORECLR_TESTS
// Use these lines if you want to test CoreCLR
let FSC_BASIC = FSC_CORECLR
let FSI_BASIC = FSI_CORECLR
#else
let FSC_BASIC = FSC_OPT_PLUS_DEBUG
let FSI_BASIC = FSI_FILE
#endif

module CoreTests = 

    // These tests are enabled for .NET Framework and .NET Core
    [<Test>]
    let ``access-FSC_BASIC``() = singleTestBuildAndRun "core/access" FSC_BASIC
// All tests below here are known to pass for .NET Core but not yet enabled due to CI problems
    [<Test>]
    let ``access-FSI_BASIC``() = singleTestBuildAndRun "core/access" FSI_BASIC

    [<Test>]
    let ``apporder-FSC_BASIC`` () = singleTestBuildAndRun "core/apporder" FSC_BASIC

    [<Test>]
    let ``apporder-FSI_BASIC`` () = singleTestBuildAndRun "core/apporder" FSI_BASIC

    [<Test>]
    let ``array-FSC_BASIC`` () = singleTestBuildAndRun "core/array" FSC_BASIC

    [<Test>]
    let ``array-FSI_BASIC`` () = singleTestBuildAndRun "core/array" FSI_BASIC

    [<Test>]
    let ``comprehensions-FSC_BASIC`` () = singleTestBuildAndRun "core/comprehensions" FSC_BASIC

    [<Test>]
    let ``comprehensions-FSI_BASIC`` () = singleTestBuildAndRun "core/comprehensions" FSI_BASIC

    [<Test>]
    let ``comprehensionshw-FSC_BASIC`` () = singleTestBuildAndRun "core/comprehensions-hw" FSC_BASIC

    [<Test>]
    let ``comprehensionshw-FSI_BASIC`` () = singleTestBuildAndRun "core/comprehensions-hw" FSI_BASIC

    [<Test>]
    let ``genericmeasures-FSI_BASIC`` () = singleTestBuildAndRun "core/genericmeasures" FSI_BASIC

    [<Test>]
    let ``genericmeasures-FSC_BASIC`` () = singleTestBuildAndRun "core/genericmeasures" FSC_BASIC

    [<Test>]
    let ``innerpoly-FSI_BASIC`` () = singleTestBuildAndRun "core/innerpoly" FSI_BASIC

    [<Test>]
    let ``innerpoly-FSC_BASIC`` () = singleTestBuildAndRun "core/innerpoly" FSC_BASIC

    [<Test; Category("namespaces")>]
    let namespaceAttributes () = singleTestBuildAndRun "core/namespaces" FSC_BASIC

    [<Test>]
    let unicode2 () = singleTestBuildAndRun "core/unicode" FSC_BASIC

    [<Test>]
    let ``unicode2-FSI_BASIC`` () = singleTestBuildAndRun "core/unicode" FSI_BASIC

    [<Test>]
    let ``lazy test-FSC_BASIC`` () = singleTestBuildAndRun "core/lazy" FSC_BASIC

    [<Test>]
    let ``lazy test-FSI_BASIC`` () = singleTestBuildAndRun "core/lazy" FSI_BASIC

    [<Test>]
    let ``letrec-FSC_BASIC`` () = singleTestBuildAndRun "core/letrec" FSC_BASIC

    [<Test>]
    let ``letrec-FSI_BASIC`` () = singleTestBuildAndRun "core/letrec" FSI_BASIC

    [<Test>]
    let ``letrec (mutrec variations part one) FSC_BASIC`` () = singleTestBuildAndRun "core/letrec-mutrec" FSC_BASIC

    [<Test>]
    let ``letrec (mutrec variations part one) FSI_BASIC`` () = singleTestBuildAndRun "core/letrec-mutrec" FSI_BASIC

    [<Test>]
    let ``libtest-FSC_BASIC`` () = singleTestBuildAndRun "core/libtest" FSC_BASIC

    [<Test>]
    let lift () = singleTestBuildAndRun "core/lift" FSC_BASIC

    [<Test>]
    let map () = singleTestBuildAndRun "core/map" FSC_BASIC

    [<Test>]
    let ``measures-FSI_BASIC`` () = singleTestBuildAndRun "core/measures" FSI_BASIC

    [<Test>]
    let ``measures-FSC_BASIC`` () = singleTestBuildAndRun "core/measures" FSC_BASIC

    [<Test>]
    let nested () = singleTestBuildAndRun "core/nested" FSC_BASIC

    [<Test>]
    let ``members-ops`` () = singleTestBuildAndRun "core/members/ops" FSC_BASIC

    [<Test>]
    let ``members-ops-mutrec`` () = singleTestBuildAndRun "core/members/ops-mutrec" FSC_BASIC

    [<Test>]
    let seq () = singleTestBuildAndRun "core/seq" FSC_BASIC

    [<Test>]
    let ``math-numbers`` () = singleTestBuildAndRun "core/math/numbers" FSC_BASIC


    [<Test>]
    let ``members-ctree`` () = singleTestBuildAndRun "core/members/ctree" FSC_BASIC

    [<Test>]
    let ``members-factors`` () = singleTestBuildAndRun "core/members/factors" FSC_BASIC

    [<Test>]
    let ``members-factors-mutrec`` () = singleTestBuildAndRun "core/members/factors-mutrec" FSC_BASIC

    [<Test>]
    let graph () = singleTestBuildAndRun "perf/graph" FSC_BASIC

    [<Test>]
    let nbody () = singleTestBuildAndRun "perf/nbody" FSC_BASIC

    [<Test>]
    let ``letrec (mutrec variations part two) FSC_BASIC`` () = singleTestBuildAndRun "core/letrec-mutrec2" FSC_BASIC

    [<Test>]
    let printf () = singleTestBuildAndRun "core/printf" FSC_BASIC

    [<Test>]
    let tlr () = singleTestBuildAndRun "core/tlr" FSC_BASIC

    [<Test>]
    let subtype () = singleTestBuildAndRun "core/subtype" FSC_BASIC

    [<Test>]
    let syntax () = singleTestBuildAndRun "core/syntax" FSC_BASIC

    [<Test>]
    let ``test int32`` () = singleTestBuildAndRun "core/int32" FSC_BASIC

#if !FSHARP_SUITE_DRIVES_CORECLR_TESTS
    [<Test>]
    let ``quotes-FSC-BASIC`` () = singleTestBuildAndRun "core/quotes" FSC_BASIC

    [<Test>]
    let ``attributes-FSC_BASIC`` () = singleTestBuildAndRun "core/attributes" FSC_BASIC

    [<Test>]
    let ``attributes-FSI_BASIC`` () = singleTestBuildAndRun "core/attributes" FSI_BASIC
#endif

#if !FSHARP_SUITE_DRIVES_CORECLR_TESTS
    [<Test>]
    let byrefs () = 

        let cfg = testConfig "core/byrefs"

        use testOkFile = fileguard cfg "test.ok"

        fsc cfg "%s -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

        exec cfg ("." ++ "test.exe") ""

        testOkFile.CheckExists()

        fsi cfg "" ["test.fsx"]

        testOkFile.CheckExists()
#endif

    [<Test>]
    let control () = singleTestBuildAndRun "core/control" FSC_BASIC

    [<Test>]
    let ``control --tailcalls`` () = 
        let cfg = testConfig "core/control"
        singleTestBuildAndRunAux {cfg with fsi_flags = " --tailcalls" } FSC_BASIC


    [<Test>]
    let controlChamenos () = 
        let cfg = testConfig "core/controlChamenos"
        
        singleTestBuildAndRunAux {cfg with fsi_flags = " --tailcalls" } FSC_BASIC


    [<Test>]
    let controlMailbox () = singleTestBuildAndRun "core/controlMailbox" FSC_BASIC

    [<Test>]
    let ``controlMailbox --tailcalls`` () = 
        let cfg = testConfig "core/controlMailbox"
        
        singleTestBuildAndRunAux {cfg with fsi_flags = " --tailcalls" } FSC_BASIC


#if !FSHARP_SUITE_DRIVES_CORECLR_TESTS
    // Requires winforms will not run on coreclr
    [<Test>]
    let controlWpf () = singleTestBuildAndRun "core/controlwpf" FSC_BASIC
#endif

    [<Test>]
    let csext () = singleTestBuildAndRun "core/csext" FSC_BASIC


#if !FSHARP_SUITE_DRIVES_CORECLR_TESTS
    [<Test>]
    let events () = 
        let cfg = testConfig "core/events"

        fsc cfg "%s -a -o:test.dll -g" cfg.fsc_flags ["test.fs"]

        peverify cfg "test.dll"

        csc cfg """/r:"%s" /reference:test.dll /debug+""" cfg.FSCOREDLLPATH ["testcs.cs"]

        peverify cfg "testcs.exe"
        
        use testOkFile = fileguard cfg "test.ok"

        fsi cfg "" ["test.fs"]

        testOkFile.CheckExists()

        exec cfg ("." ++ "testcs.exe") ""


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
    //    let ``shadowcopy disabled`` (flags: string) = 
    //        let cfg = testConfig ()
    //
    //
    //
    //
    //
    //        // if exist test1.ok (del /f /q test1.ok)
    //        use testOkFile = fileguard cfg "test1.ok"
    //
    //        fsiStdin cfg "%s %s" cfg.fsi_flags flags "test1.fsx"
    //
    //        // if NOT EXIST test1.ok goto SetError
    //        testOkFile.CheckExists()
    //    
    //
    //    [<Test>]
    //    // "%FSI%" %fsi_flags%  /shadowcopyreferences+  < test2.fsx
    //    [<FSharpSuiteTestCase("core/fsi-shadowcopy", "/shadowcopyreferences+")
    //    // "%FSI%" %fsi_flags%  --shadowcopyreferences  < test2.fsx
    //    [<FSharpSuiteTestCase("core/fsi-shadowcopy", "--shadowcopyreferences")
    //    let ``shadowcopy enabled`` (flags: string) = 
    //        let cfg = testConfig ()
    //
    //
    //
    //
    //
    //        // if exist test2.ok (del /f /q test2.ok)
    //        use testOkFile = fileguard cfg "test2.ok"
    //
    //        // "%FSI%" %fsi_flags%  /shadowcopyreferences+  < test2.fsx
    //        fsiStdin cfg "%s %s" cfg.fsi_flags flags "test2.fsx"
    //
    //        // if NOT EXIST test2.ok goto SetError
    //        testOkFile.CheckExists()
    //    


    [<Test>]
    let forwarders () = 
        let cfg = testConfig "core/forwarders"

        mkdir cfg "orig"
        mkdir cfg "split"

        csc cfg """/nologo  /target:library /out:orig\a.dll /define:PART1;PART2""" ["a.cs"]

        csc cfg """/nologo  /target:library /out:orig\b.dll /r:orig\a.dll""" ["b.cs"]

        fsc cfg """-a -o:orig\c.dll -r:orig\b.dll -r:orig\a.dll""" ["c.fs"]

        csc cfg """/nologo  /target:library /out:split\a-part1.dll /define:PART1;SPLIT""" ["a.cs"]

        csc cfg """/nologo  /target:library /r:split\a-part1.dll /out:split\a.dll /define:PART2;SPLIT""" ["a.cs"]

        copy_y cfg ("orig" ++ "b.dll") ("split" ++ "b.dll")

        copy_y cfg ("orig" ++ "c.dll") ("split" ++ "c.dll")

        fsc cfg """-o:orig\test.exe -r:orig\b.dll -r:orig\a.dll""" ["test.fs"]

        fsc cfg """-o:split\test.exe -r:split\b.dll -r:split\a-part1.dll -r:split\a.dll""" ["test.fs"]

        fsc cfg """-o:split\test-against-c.exe -r:split\c.dll -r:split\a-part1.dll -r:split\a.dll""" ["test.fs"]

        peverify cfg ("split" ++ "a-part1.dll")

        peverify cfg ("split" ++ "b.dll")

        peverify cfg ("split" ++ "c.dll")

    [<Test>]
    let fsfromcs () = 
        let cfg = testConfig "core/fsfromcs"

        fsc cfg "%s -a --doc:lib.xml -o:lib.dll -g" cfg.fsc_flags ["lib.fs"]

        peverify cfg "lib.dll"

        csc cfg """/nologo /r:"%s" /r:System.Core.dll /r:lib.dll /out:test.exe""" cfg.FSCOREDLLPATH ["test.cs"]

        fsc cfg """%s -a --doc:lib--optimize.xml -o:lib--optimize.dll -g""" cfg.fsc_flags ["lib.fs"]

        peverify cfg "lib--optimize.dll"

        csc cfg """/nologo /r:"%s"  /r:System.Core.dll /r:lib--optimize.dll    /out:test--optimize.exe""" cfg.FSCOREDLLPATH ["test.cs"]

        exec cfg ("." ++ "test.exe") ""

        exec cfg ("." ++ "test--optimize.exe") ""
                
    [<Test>]
    let fsfromfsviacs () = 
        let cfg = testConfig "core/fsfromfsviacs"

        fsc cfg "%s -a -o:lib.dll -g" cfg.fsc_flags ["lib.fs"]

        peverify cfg "lib.dll"

        csc cfg """/nologo /target:library /r:"%s" /r:lib.dll /out:lib2.dll""" cfg.FSCOREDLLPATH ["lib2.cs"]

        csc cfg """/nologo /target:library /r:"%s" /out:lib3.dll""" cfg.FSCOREDLLPATH ["lib3.cs"]

        fsc cfg "%s -r:lib.dll -r:lib2.dll -r:lib3.dll -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test.exe"

        // Same with library references the other way around
        fsc cfg "%s -r:lib.dll -r:lib3.dll -r:lib2.dll -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test.exe"

        exec cfg ("." ++ "test.exe") ""
                
    [<Test>]
    let ``fsi-reload`` () = 
        let cfg = testConfig "core/fsi-reload"

        begin
            use testOkFile = fileguard cfg "test.ok"
            fsiStdin cfg "test1.ml"  "--maxerrors:1" []
            testOkFile.CheckExists()
        end
                
        begin
            use testOkFile = fileguard cfg "test.ok"
            fsi cfg "%s  --maxerrors:1" cfg.fsi_flags ["load1.fsx"]
            testOkFile.CheckExists()
        end

        begin
            use testOkFile = fileguard cfg "test.ok"
            fsi cfg "%s  --maxerrors:1" cfg.fsi_flags ["load2.fsx"]
            testOkFile.CheckExists()
        end

        fsc cfg "" ["load1.fsx"]

        fsc cfg "" ["load2.fsx"]


    [<Test>]
    let fsiAndModifiers () = 
        let cfg = testConfig "core/fsiAndModifiers"

        do if fileExists cfg "TestLibrary.dll" then rm cfg "TestLibrary.dll"

        fsiStdin cfg "prepare.fsx" "--maxerrors:1" []

        use testOkFile = fileguard cfg "test.ok"
        
        fsiStdin cfg "test.fsx" "--maxerrors:1"  []

        testOkFile.CheckExists()
                


    [<Test>]
    let ``genericmeasures-AS_DLL`` () = singleTestBuildAndRun "core/genericmeasures" AS_DLL


    [<Test>]
    let hiding () = 
        let cfg = testConfig "core/hiding"

        fsc cfg "%s -a --optimize -o:lib.dll" cfg.fsc_flags ["lib.mli";"lib.ml";"libv.ml"]

        peverify cfg "lib.dll"

        fsc cfg "%s -a --optimize -r:lib.dll -o:lib2.dll" cfg.fsc_flags ["lib2.mli";"lib2.ml";"lib3.ml"]

        peverify cfg "lib2.dll"

        fsc cfg "%s --optimize -r:lib.dll -r:lib2.dll -o:client.exe" cfg.fsc_flags ["client.ml"]

        peverify cfg "client.exe"

    [<Test>]
    let ``innerpoly-AS_DLL`` () = singleTestBuildAndRun "core/innerpoly"  AS_DLL       

    [<Test>]
    let queriesCustomQueryOps () = 
        let cfg = testConfig "core/queriesCustomQueryOps"

        fsc cfg """%s -o:test.exe -g""" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test.exe"

        fsc cfg """%s --optimize -o:test--optimize.exe -g""" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test--optimize.exe"

        singleNegTest cfg "negativetest"

        begin
            use testOkFile = fileguard cfg "test.ok"

            fsi cfg "%s" cfg.fsi_flags ["test.fsx"]

            testOkFile.CheckExists()
        end

        begin
            use testOkFile = fileguard cfg "test.ok"

            exec cfg ("." ++ "test.exe") ""

            testOkFile.CheckExists()
        end

        begin
            use testOkFile = fileguard cfg "test.ok"

            exec cfg ("." ++ "test--optimize.exe") ""

            testOkFile.CheckExists()
        end
                


    let printing flag diffFileOut expectedFileOut diffFileErr expectedFileErr = 
       let cfg = testConfig "core/printing"

       if requireENCulture () then

        let copy from' = Commands.copy_y cfg.Directory from' >> checkResult

        let ``fsi <a >b 2>c`` =
            // "%FSI%" %fsc_flags_errors_ok%  --nologo                                    <test.fsx >z.raw.output.test.default.txt 2>&1
            let ``exec <a >b 2>c`` (inFile, outFile, errFile) p = 
                Command.exec cfg.Directory cfg.EnvironmentVariables { Output = OutputAndError(Overwrite(outFile), Overwrite(errFile)); Input = Some(RedirectInput(inFile)); } p 
                >> checkResult
            Printf.ksprintf (fun flags (inFile, outFile, errFile) -> Commands.fsi (``exec <a >b 2>c`` (inFile, outFile, errFile)) cfg.FSI flags [])

        let fsc_flags_errors_ok = ""

        let rawFileOut = Path.GetTempFileName()
        let rawFileErr = Path.GetTempFileName()
        ``fsi <a >b 2>c`` "%s --nologo %s" fsc_flags_errors_ok flag ("test.fsx", rawFileOut, rawFileErr)

        // REM REVIEW: want to normalise CWD paths, not suppress them.
        let ``findstr /v`` text = Seq.filter (fun (s: string) -> not <| s.Contains(text))
        let removeCDandHelp from' to' =
            File.ReadLines from' |> (``findstr /v`` cfg.Directory) |> (``findstr /v`` "--help' for options") |> (fun lines -> File.WriteAllLines(getfullpath cfg to', lines))

        removeCDandHelp rawFileOut diffFileOut
        removeCDandHelp rawFileErr diffFileErr

        let withDefault default' to' =
            if not (fileExists cfg to') then copy default' to'

        expectedFileOut |> withDefault diffFileOut
        expectedFileErr |> withDefault diffFileErr

        
        match fsdiff cfg diffFileOut expectedFileOut with
        | "" -> ()
        | diffs -> Assert.Fail (sprintf "'%s' and '%s' differ; %A" diffFileOut expectedFileOut diffs)

        match fsdiff cfg diffFileErr expectedFileErr with
        | "" -> ()
        | diffs -> Assert.Fail (sprintf "'%s' and '%s' differ; %A" diffFileErr expectedFileErr diffs)

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


    let signedtest(args,bslfile) = 
    
        let cfg = testConfig "core/signedtests"
        let cfg = { cfg with fsc_flags=cfg.fsc_flags + " " + args }

        let outfile = Path.ChangeExtension(bslfile,"sn.out") 
        let exefile = Path.ChangeExtension(bslfile,"exe") 
        do File.WriteAllLines(getfullpath cfg outfile,
                              ["sn -q stops all output except error messages                "
                               "if the output is a valid file no output is produced.       "
                               "delay-signed and unsigned produce error messages.          "])

        fsc cfg "%s -o:%s" cfg.fsc_flags exefile ["test.fs"]
        sn cfg outfile ("-q -vf "+exefile) 
        let diffs = fsdiff cfg outfile bslfile 

        match diffs with
        | "" -> ()
        | _ -> Assert.Fail (sprintf "'%s' and '%s' differ; %A" outfile bslfile diffs)

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
#endif

#if !FSHARP_SUITE_DRIVES_CORECLR_TESTS
    [<Test>]
    let ``quotes-FSI-BASIC`` () = singleTestBuildAndRun "core/quotes" FSI_BASIC

    [<Test>]
    let quotes () = 
        let cfg = testConfig "core/quotes"

        csc cfg """/nologo  /target:library /out:cslib.dll""" ["cslib.cs"]

        fsc cfg "%s -o:test.exe -r cslib.dll -g" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test.exe"

        begin
            use testOkFile = fileguard cfg "test.ok"
            exec cfg ("." ++ "test.exe") ""
            testOkFile.CheckExists()
        end

        fsc cfg "%s -o:test-with-debug-data.exe --quotations-debug+ -r cslib.dll -g" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test-with-debug-data.exe"

        fsc cfg "%s --optimize -o:test--optimize.exe -r cslib.dll -g" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test--optimize.exe"

        begin
            use testOkFile = fileguard cfg "test.ok"

            fsi cfg "%s -r cslib.dll" cfg.fsi_flags ["test.fsx"]

            testOkFile.CheckExists()
        end

        begin
            use testOkFile = fileguard cfg "test.ok"
            exec cfg ("." ++ "test-with-debug-data.exe") ""
            testOkFile.CheckExists()
        end

        begin
            use testOkFile = fileguard cfg "test.ok"
            exec cfg ("." ++ "test--optimize.exe") ""
            testOkFile.CheckExists()
        end

    [<Test; Category("parsing")>]
    let parsing () = 
        let cfg = testConfig "core/parsing"
        
        fsc cfg "%s -a -o:crlf.dll -g" cfg.fsc_flags ["crlf.ml"]

        fsc cfg "%s -o:toplet.exe -g" cfg.fsc_flags ["toplet.ml"]

        peverify cfg "toplet.exe"

    [<Test>]
    let unicode () = 
        let cfg = testConfig "core/unicode"

        fsc cfg "%s -a -o:kanji-unicode-utf8-nosig-codepage-65001.dll -g" cfg.fsc_flags ["kanji-unicode-utf8-nosig-codepage-65001.fs"]

        fsc cfg "%s -a -o:kanji-unicode-utf8-nosig-codepage-65001.dll -g" cfg.fsc_flags ["kanji-unicode-utf8-nosig-codepage-65001.fs"]

        fsc cfg "%s -a -o:kanji-unicode-utf16.dll -g" cfg.fsc_flags ["kanji-unicode-utf16.fs"]

        fsc cfg "%s -a --codepage:65000 -o:kanji-unicode-utf7-codepage-65000.dll -g" cfg.fsc_flags ["kanji-unicode-utf7-codepage-65000.fs"]
        
        fsc cfg "%s -a -o:kanji-unicode-utf8-withsig-codepage-65001.dll -g" cfg.fsc_flags ["kanji-unicode-utf8-withsig-codepage-65001.fs"]

        fsi cfg "%s --utf8output" cfg.fsi_flags ["kanji-unicode-utf8-nosig-codepage-65001.fs"]

        fsi cfg "%s --utf8output --codepage:65001" cfg.fsi_flags ["kanji-unicode-utf8-withsig-codepage-65001.fs"]

        fsi cfg "%s --utf8output" cfg.fsi_flags ["kanji-unicode-utf8-withsig-codepage-65001.fs"]

        fsi cfg "%s --utf8output --codepage:65000" cfg.fsi_flags ["kanji-unicode-utf7-codepage-65000.fs"]

        fsi cfg "%s --utf8output" cfg.fsi_flags ["kanji-unicode-utf16.fs"]
 

    [<Test>]
    let internalsvisible () = 
        let cfg = testConfig "core/internalsvisible"

        // Compiling F# Library
        fsc cfg "%s --version:1.2.3 --keyfile:key.snk -a --optimize -o:library.dll" cfg.fsc_flags ["library.fsi"; "library.fs"]

        peverify cfg "library.dll"

        // Compiling C# Library
        csc cfg "/target:library /keyfile:key.snk /out:librarycs.dll" ["librarycs.cs"]

        peverify cfg "librarycs.dll"

        // Compiling F# main referencing C# and F# libraries
        fsc cfg "%s --version:1.2.3 --keyfile:key.snk --optimize -r:library.dll -r:librarycs.dll -o:main.exe" cfg.fsc_flags ["main.fs"]

        peverify cfg "main.exe"

        // Run F# main. Quick test!
        exec cfg ("." ++ "main.exe") ""
 

    // Repro for https://github.com/Microsoft/visualfsharp/issues/1298
    [<Test>]
    let fileorder () = 
        let cfg = testConfig "core/fileorder"

        log "== Compiling F# Library and Code, when empty file libfile2.fs IS NOT included"
        fsc cfg "%s -a --optimize -o:lib.dll " cfg.fsc_flags ["libfile1.fs"]

        peverify cfg "lib.dll"

        fsc cfg "%s -r:lib.dll -o:test.exe" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test.exe"

        exec cfg ("." ++ "test.exe") ""

        log "== Compiling F# Library and Code, when empty file libfile2.fs IS included"
        fsc cfg "%s -a --optimize -o:lib2.dll " cfg.fsc_flags ["libfile1.fs"; "libfile2.fs"]

        peverify cfg "lib2.dll"

        fsc cfg "%s -r:lib2.dll -o:test2.exe" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test2.exe"

        exec cfg ("." ++ "test2.exe") ""
 
    [<Test>]
    let ``libtest-FSI_STDIN`` () = singleTestBuildAndRun "core/libtest" FSI_STDIN

    [<Test; Ignore("incorrect signature file generated, test has been disabled a long time")>]
    let ``libtest-GENERATED_SIGNATURE`` () = singleTestBuildAndRun "core/libtest" GENERATED_SIGNATURE

    [<Test>]
    let ``libtest-FSC_OPT_MINUS_DEBUG`` () = singleTestBuildAndRun "core/libtest" FSC_OPT_MINUS_DEBUG

    [<Test>]
    let ``libtest-AS_DLL`` () = singleTestBuildAndRun "core/libtest" AS_DLL

    [<Test>]
    let ``libtest-FSI_BASIC`` () = singleTestBuildAndRun "core/libtest" FSI_BASIC

    [<Test>]
    let ``letrec (mutrec variations part two) FSI_BASIC`` () = singleTestBuildAndRun "core/letrec-mutrec2" FSI_BASIC

    [<Test>]
    let recordResolution () = singleTestBuildAndRun "core/recordResolution" FSC_OPT_PLUS_DEBUG

    [<Test>]
    let ``load-script`` () = 
        let cfg = testConfig "core/load-script"

        let stdoutPath = "out.stdout.txt" |> getfullpath cfg
        let stderrPath = "out.stderr.txt" |> getfullpath cfg
        let stderrBaseline = "out.stderr.bsl" |> getfullpath cfg 
        let stdoutBaseline = "out.stdout.bsl" |> getfullpath cfg 

        let appendToFile from = Commands.appendToFile cfg.Directory from stdoutPath
        let echo text = Commands.echoAppendToFile cfg.Directory text stdoutPath

        File.WriteAllText(stdoutPath, "")
        File.WriteAllText(stderrPath, "")

        do if fileExists cfg "3.exe" then getfullpath cfg "3.exe" |> File.Delete

        ["1.fsx"; "2.fsx"; "3.fsx"] |> List.iter appendToFile

        echo "Test 1================================================="

        fscAppend cfg stdoutPath stderrPath "--nologo" ["3.fsx"]

        execAppendIgnoreExitCode cfg stdoutPath stderrPath ("." ++ "3.exe") ""

        rm cfg "3.exe"

        echo "Test 2================================================="

        fsiAppendIgnoreExitCode cfg stdoutPath stderrPath "" ["3.fsx"]

        echo "Test 3================================================="

        fsiStdinAppendBothIgnoreExitCode cfg stdoutPath stderrPath "pipescr" "--nologo" []

        echo "Test 4================================================="

        fsiAppendIgnoreExitCode cfg stdoutPath stderrPath "" ["usesfsi.fsx"]

        echo "Test 5================================================="

        fscAppendIgnoreExitCode cfg stdoutPath stderrPath "--nologo" ["usesfsi.fsx"]

        echo "Test 6================================================="

        fscAppend cfg stdoutPath stderrPath "--nologo -r FSharp.Compiler.Interactive.Settings" ["usesfsi.fsx"]

        echo "Test 7================================================="

        fsiAppendIgnoreExitCode cfg stdoutPath stderrPath "" ["1.fsx";"2.fsx";"3.fsx"]

        echo "Test 8================================================="

        fsiAppendIgnoreExitCode cfg stdoutPath stderrPath "" ["3.fsx";"2.fsx";"1.fsx"]

        echo "Test 9================================================="

        fsiAppendIgnoreExitCode cfg stdoutPath stderrPath "" ["multiple-load-1.fsx"]

        echo "Test 10================================================="

        fsiAppendIgnoreExitCode cfg stdoutPath stderrPath "" ["multiple-load-2.fsx"]

        echo "Test 11================================================="

        fscAppend cfg stdoutPath stderrPath "--nologo" ["FlagCheck.fs"]

        execAppendIgnoreExitCode cfg stdoutPath stderrPath ("." ++ "FlagCheck.exe") ""

        rm cfg "FlagCheck.exe"

        echo "Test 12================================================="

        fscAppend cfg stdoutPath stderrPath "-o FlagCheckScript.exe --nologo" ["FlagCheck.fsx"]

        execAppendIgnoreExitCode cfg stdoutPath stderrPath ("." ++ "FlagCheckScript.exe") ""

        rm cfg "FlagCheckScript.exe"

        echo "Test 13================================================="

        fsiAppendIgnoreExitCode cfg stdoutPath stderrPath "" ["load-FlagCheckFs.fsx"]

        echo "Test 14================================================="

        fsiAppendIgnoreExitCode cfg stdoutPath stderrPath "" ["FlagCheck.fsx"]

        echo "Test 15================================================="

        fsiAppendIgnoreExitCode cfg stdoutPath stderrPath "" ["ProjectDriver.fsx"]

        echo "Test 16================================================="

        fscAppend cfg stdoutPath stderrPath "--nologo" ["ProjectDriver.fsx"]

        execAppendIgnoreExitCode cfg stdoutPath stderrPath ("." ++ "ProjectDriver.exe") ""

        rm cfg "ProjectDriver.exe"

        echo "Test 17================================================="

        fsiAppendIgnoreExitCode cfg stdoutPath stderrPath "" ["load-IncludeNoWarn211.fsx"]

        echo "Done =================================================="

        // an extra case
        fsiExpectFail cfg "" ["loadfail3.fsx"]

        let normalizePaths f =
            let text = File.ReadAllText(f)
            let dummyPath = @"D:\staging\staging\src\tests\fsharp\core\load-script"
            let contents = System.Text.RegularExpressions.Regex.Replace(text, System.Text.RegularExpressions.Regex.Escape(cfg.Directory), dummyPath)
            File.WriteAllText(f, contents)

        normalizePaths stdoutPath
        normalizePaths stderrPath

        let diffs = fsdiff cfg stdoutPath stdoutBaseline

        match diffs with
        | "" -> ()
        | _ -> Assert.Fail (sprintf "'%s' and '%s' differ; %A" stdoutPath stdoutBaseline diffs)

        let diffs2 = fsdiff cfg stderrPath stderrBaseline

        match diffs2 with
        | "" -> ()
        | _ -> Assert.Fail (sprintf "'%s' and '%s' differ; %A" stderrPath stderrBaseline diffs2)
#endif

    [<Test>]
    let longnames () = singleTestBuildAndRun "core/longnames" FSC_BASIC

    [<Test>]
    let ``math-numbersVS2008`` () = singleTestBuildAndRun "core/math/numbersVS2008" FSC_BASIC

#if !FSHARP_SUITE_DRIVES_CORECLR_TESTS
    [<Test>]
    let ``measures-AS_DLL`` () = singleTestBuildAndRun "core/measures" AS_DLL

    // Requires winforms will not run on coreclr
    [<Test>]
    let ``members-basics-FSI_BASIC`` () = singleTestBuildAndRun "core/members/basics" FSI_BASIC

    // Requires winforms will not run on coreclr
    [<Test>]
    let ``members-basics-FSC_BASIC`` () = singleTestBuildAndRun "core/members/basics" FSC_BASIC

    // Requires winforms will not run on coreclr
    [<Test>]
    let ``members-basics-AS_DLL`` () = singleTestBuildAndRun "core/members/basics" AS_DLL

    // Requires winforms will not run on coreclr
    [<Test>]
    let ``members-basics-hw`` () = singleTestBuildAndRun "core/members/basics-hw" FSC_BASIC

    // Requires winforms will not run on coreclr
    [<Test>]
    let ``members-basics-hw-mutrec`` () = singleTestBuildAndRun "core/members/basics-hw-mutrec" FSC_BASIC

    [<Test>]
    let ``members-incremental`` () = singleTestBuildAndRun "core/members/incremental" FSC_BASIC

    [<Test>]
    let ``members-incremental-hw`` () = singleTestBuildAndRun "core/members/incremental-hw" FSC_BASIC

    [<Test>]
    let ``members-incremental-hw-mutrec`` () = singleTestBuildAndRun "core/members/incremental-hw-mutrec" FSC_BASIC

#endif

    [<Test>]
    let patterns () = singleTestBuildAndRun "core/patterns" FSC_BASIC

#if !FSHARP_SUITE_DRIVES_CORECLR_TESTS
    [<Test>]
    let pinvoke () = 
        let cfg = testConfig "core/pinvoke"

        fsc cfg "%s -o:test.exe -g" cfg.fsc_flags ["test.fsx"]
   
        peverifyWithArgs cfg "/nologo /MD" "test.exe"
                
    [<Test>]
    let queriesLeafExpressionConvert () = 
        let cfg = testConfig "core/queriesLeafExpressionConvert"

        fsc cfg "%s -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test.exe"

        fsc cfg "%s --optimize -o:test--optimize.exe -g" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test--optimize.exe"

        use testOkFile = fileguard cfg "test.ok"

        fsi cfg "%s" cfg.fsi_flags ["test.fsx"]

        testOkFile.CheckExists()

        use testOkFile2 = fileguard cfg "test.ok"

        exec cfg ("." ++ "test.exe") ""

        testOkFile2.CheckExists()

        use testOkFile3 = fileguard cfg "test.ok"

        exec cfg ("." ++ "test--optimize.exe") ""

        testOkFile3.CheckExists()
                

    [<Test>]
    let queriesNullableOperators () = 
        let cfg = testConfig "core/queriesNullableOperators"

        fsc cfg "%s -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test.exe"

        fsc cfg "%s --optimize -o:test--optimize.exe -g" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test--optimize.exe"

        use testOkFile = fileguard cfg "test.ok"
        fsi cfg "%s" cfg.fsi_flags ["test.fsx"]
        testOkFile.CheckExists()

        use testOkFile2 = fileguard cfg "test.ok"
        exec cfg ("." ++ "test.exe") ""
        testOkFile2.CheckExists()

        use testOkFile3 = fileguard cfg "test.ok"
        exec cfg ("." ++ "test--optimize.exe") ""
        testOkFile3.CheckExists()
                
    [<Test>]
    let queriesOverIEnumerable () = 
        let cfg = testConfig "core/queriesOverIEnumerable"

        fsc cfg "%s -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test.exe" 

        fsc cfg "%s --optimize -o:test--optimize.exe -g" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test--optimize.exe"

        use testOkFile = fileguard cfg "test.ok"

        fsi cfg "%s" cfg.fsi_flags ["test.fsx"]

        testOkFile.CheckExists()

        use testOkFile2 = fileguard cfg "test.ok"

        exec cfg ("." ++ "test.exe") ""

        testOkFile2.CheckExists()

        use testOkFile3 = fileguard cfg "test.ok"

        exec cfg ("." ++ "test--optimize.exe") ""

        testOkFile3.CheckExists()
                
    [<Test>]
    let queriesOverIQueryable () = 
        let cfg = testConfig "core/queriesOverIQueryable"

        fsc cfg "%s -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test.exe"

        fsc cfg "%s --optimize -o:test--optimize.exe -g" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test--optimize.exe"

        use testOkFile = fileguard cfg "test.ok"
        fsi cfg "%s" cfg.fsi_flags ["test.fsx"]

        testOkFile.CheckExists()


        use testOkFile2 = fileguard cfg "test.ok"

        exec cfg ("." ++ "test.exe") ""

        testOkFile2.CheckExists()


        use testOkFile3 = fileguard cfg "test.ok"
        exec cfg ("." ++ "test--optimize.exe") ""

        testOkFile3.CheckExists()


    [<Test>]
    let quotesDebugInfo () = 
        let cfg = testConfig "core/quotesDebugInfo"

        fsc cfg "%s --quotations-debug+ --optimize -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test.exe"

        fsc cfg "%s --quotations-debug+ --optimize -o:test--optimize.exe -g" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test--optimize.exe"

        use testOkFile = fileguard cfg "test.ok"
        fsi cfg "%s --quotations-debug+" cfg.fsi_flags ["test.fsx"]

        testOkFile.CheckExists()


        use testOkFile2 = fileguard cfg "test.ok"

        exec cfg ("." ++ "test.exe") ""

        testOkFile2.CheckExists()

        use testOkFile3 = fileguard cfg "test.ok"

        exec cfg ("." ++ "test--optimize.exe") ""

        testOkFile3.CheckExists()


    [<Test>]
    let quotesInMultipleModules () = 
        let cfg = testConfig "core/quotesInMultipleModules"

        fsc cfg "%s -o:module1.dll --target:library" cfg.fsc_flags ["module1.fsx"]

        peverify cfg "module1.dll"

        fsc cfg "%s -o:module2.exe -r:module1.dll" cfg.fsc_flags ["module2.fsx"]

        peverify cfg "module2.exe"
    
        fsc cfg "%s --staticlink:module1 -o:module2-staticlink.exe -r:module1.dll" cfg.fsc_flags ["module2.fsx"]

        peverify cfg "module2-staticlink.exe"

        fsc cfg "%s -o:module1-opt.dll --target:library --optimize" cfg.fsc_flags ["module1.fsx"]

        peverify cfg "module1-opt.dll"

        fsc cfg "%s -o:module2-opt.exe -r:module1-opt.dll --optimize" cfg.fsc_flags ["module2.fsx"]

        peverify cfg "module2-opt.exe"

        use testOkFile = fileguard cfg "test.ok"

        fsi cfg "%s -r module1.dll" cfg.fsi_flags ["module2.fsx"]

        testOkFile.CheckExists()


        use testOkFile = fileguard cfg "test.ok"

        exec cfg ("." ++ "module2.exe") ""

        testOkFile.CheckExists()

        use testOkFile = fileguard cfg "test.ok"

        exec cfg ("." ++ "module2-opt.exe") ""

        testOkFile.CheckExists()

        use testOkFile = fileguard cfg "test.ok"

        exec cfg ("." ++ "module2-staticlink.exe") ""

        testOkFile.CheckExists()
#endif
                
    [<Test>]
    let reflect () = singleTestBuildAndRun "core/reflect" FSC_BASIC

#if !FSHARP_SUITE_DRIVES_CORECLR_TESTS
    [<Test>]
    let testResources () = 
        let cfg = testConfig "core/resources"

        fsc cfg "%s  --resource:Resources.resources -o:test-embed.exe -g" cfg.fsc_flags ["test.fs"]

        peverify cfg "test-embed.exe"

        fsc cfg "%s  --linkresource:Resources.resources -o:test-link.exe -g" cfg.fsc_flags ["test.fs"]

        peverify cfg "test-link.exe"

        fsc cfg "%s  --resource:Resources.resources,ResourceName.resources -o:test-embed-named.exe -g" cfg.fsc_flags ["test.fs"]

        peverify cfg "test-embed-named.exe"

        fsc cfg "%s  --linkresource:Resources.resources,ResourceName.resources -o:test-link-named.exe -g" cfg.fsc_flags ["test.fs"]

        peverify cfg "test-link-named.exe"

        exec cfg ("." ++ "test-embed.exe") ""

        exec cfg ("." ++ "test-link.exe") ""

        exec cfg ("." ++ "test-link-named.exe") "ResourceName"

        exec cfg ("." ++ "test-embed-named.exe") "ResourceName"

    [<Test>]
    let topinit () = 
        let cfg = testConfig "core/topinit"

        fsc cfg "%s --optimize -o both69514.exe -g" cfg.fsc_flags ["lib69514.fs"; "app69514.fs"]

        peverify cfg "both69514.exe"

        fsc cfg "%s --optimize- -o both69514-noopt.exe -g" cfg.fsc_flags ["lib69514.fs"; "app69514.fs"]

        peverify cfg "both69514-noopt.exe"

        fsc cfg "%s --optimize -a -g" cfg.fsc_flags ["lib69514.fs"]

        peverify cfg "lib69514.dll"

        fsc cfg "%s --optimize -r:lib69514.dll -g" cfg.fsc_flags ["app69514.fs"]

        peverify cfg "app69514.exe"

        fsc cfg "%s --optimize- -o:lib69514-noopt.dll -a -g" cfg.fsc_flags ["lib69514.fs"]

        peverify cfg "lib69514-noopt.dll"

        fsc cfg "%s --optimize- -r:lib69514-noopt.dll -o:app69514-noopt.exe -g" cfg.fsc_flags ["app69514.fs"]

        peverify cfg "app69514-noopt.exe"

        fsc cfg "%s --optimize- -o:lib69514-noopt-withsig.dll -a -g" cfg.fsc_flags ["lib69514.fsi"; "lib69514.fs"]

        peverify cfg "lib69514-noopt-withsig.dll"

        fsc cfg "%s --optimize- -r:lib69514-noopt-withsig.dll -o:app69514-noopt-withsig.exe -g" cfg.fsc_flags ["app69514.fs"]

        peverify cfg "app69514-noopt-withsig.exe"

        fsc cfg "%s -o:lib69514-withsig.dll -a -g" cfg.fsc_flags ["lib69514.fsi"; "lib69514.fs"]

        peverify cfg "lib69514-withsig.dll"

        fsc cfg "%s -r:lib69514-withsig.dll -o:app69514-withsig.exe -g" cfg.fsc_flags ["app69514.fs"]

        peverify cfg "app69514-withsig.exe"

        fsc cfg "%s -o:lib.dll -a -g" cfg.fsc_flags ["lib.ml"]

        peverify cfg "lib.dll"

        csc cfg """/nologo /r:"%s" /r:lib.dll /out:test.exe """ cfg.FSCOREDLLPATH ["test.cs"]

        fsc cfg "%s --optimize -o:lib--optimize.dll -a -g" cfg.fsc_flags ["lib.ml"]

        peverify cfg "lib--optimize.dll"

        csc cfg """/nologo /r:"%s" /r:lib--optimize.dll /out:test--optimize.exe""" cfg.FSCOREDLLPATH ["test.cs"]

        let dicases = ["flag_deterministic_init1.fs"; "lib_deterministic_init1.fs"; "flag_deterministic_init2.fs"; "lib_deterministic_init2.fs"; "flag_deterministic_init3.fs"; "lib_deterministic_init3.fs"; "flag_deterministic_init4.fs"; "lib_deterministic_init4.fs"; "flag_deterministic_init5.fs"; "lib_deterministic_init5.fs"; "flag_deterministic_init6.fs"; "lib_deterministic_init6.fs"; "flag_deterministic_init7.fs"; "lib_deterministic_init7.fs"; "flag_deterministic_init8.fs"; "lib_deterministic_init8.fs"; "flag_deterministic_init9.fs"; "lib_deterministic_init9.fs"; "flag_deterministic_init10.fs"; "lib_deterministic_init10.fs"; "flag_deterministic_init11.fs"; "lib_deterministic_init11.fs"; "flag_deterministic_init12.fs"; "lib_deterministic_init12.fs"; "flag_deterministic_init13.fs"; "lib_deterministic_init13.fs"; "flag_deterministic_init14.fs"; "lib_deterministic_init14.fs"; "flag_deterministic_init15.fs"; "lib_deterministic_init15.fs"; "flag_deterministic_init16.fs"; "lib_deterministic_init16.fs"; "flag_deterministic_init17.fs"; "lib_deterministic_init17.fs"; "flag_deterministic_init18.fs"; "lib_deterministic_init18.fs"; "flag_deterministic_init19.fs"; "lib_deterministic_init19.fs"; "flag_deterministic_init20.fs"; "lib_deterministic_init20.fs"; "flag_deterministic_init21.fs"; "lib_deterministic_init21.fs"; "flag_deterministic_init22.fs"; "lib_deterministic_init22.fs"; "flag_deterministic_init23.fs"; "lib_deterministic_init23.fs"; "flag_deterministic_init24.fs"; "lib_deterministic_init24.fs"; "flag_deterministic_init25.fs"; "lib_deterministic_init25.fs"; "flag_deterministic_init26.fs"; "lib_deterministic_init26.fs"; "flag_deterministic_init27.fs"; "lib_deterministic_init27.fs"; "flag_deterministic_init28.fs"; "lib_deterministic_init28.fs"; "flag_deterministic_init29.fs"; "lib_deterministic_init29.fs"; "flag_deterministic_init30.fs"; "lib_deterministic_init30.fs"; "flag_deterministic_init31.fs"; "lib_deterministic_init31.fs"; "flag_deterministic_init32.fs"; "lib_deterministic_init32.fs"; "flag_deterministic_init33.fs"; "lib_deterministic_init33.fs"; "flag_deterministic_init34.fs"; "lib_deterministic_init34.fs"; "flag_deterministic_init35.fs"; "lib_deterministic_init35.fs"; "flag_deterministic_init36.fs"; "lib_deterministic_init36.fs"; "flag_deterministic_init37.fs"; "lib_deterministic_init37.fs"; "flag_deterministic_init38.fs"; "lib_deterministic_init38.fs"; "flag_deterministic_init39.fs"; "lib_deterministic_init39.fs"; "flag_deterministic_init40.fs"; "lib_deterministic_init40.fs"; "flag_deterministic_init41.fs"; "lib_deterministic_init41.fs"; "flag_deterministic_init42.fs"; "lib_deterministic_init42.fs"; "flag_deterministic_init43.fs"; "lib_deterministic_init43.fs"; "flag_deterministic_init44.fs"; "lib_deterministic_init44.fs"; "flag_deterministic_init45.fs"; "lib_deterministic_init45.fs"; "flag_deterministic_init46.fs"; "lib_deterministic_init46.fs"; "flag_deterministic_init47.fs"; "lib_deterministic_init47.fs"; "flag_deterministic_init48.fs"; "lib_deterministic_init48.fs"; "flag_deterministic_init49.fs"; "lib_deterministic_init49.fs"; "flag_deterministic_init50.fs"; "lib_deterministic_init50.fs"; "flag_deterministic_init51.fs"; "lib_deterministic_init51.fs"; "flag_deterministic_init52.fs"; "lib_deterministic_init52.fs"; "flag_deterministic_init53.fs"; "lib_deterministic_init53.fs"; "flag_deterministic_init54.fs"; "lib_deterministic_init54.fs"; "flag_deterministic_init55.fs"; "lib_deterministic_init55.fs"; "flag_deterministic_init56.fs"; "lib_deterministic_init56.fs"; "flag_deterministic_init57.fs"; "lib_deterministic_init57.fs"; "flag_deterministic_init58.fs"; "lib_deterministic_init58.fs"; "flag_deterministic_init59.fs"; "lib_deterministic_init59.fs"; "flag_deterministic_init60.fs"; "lib_deterministic_init60.fs"; "flag_deterministic_init61.fs"; "lib_deterministic_init61.fs"; "flag_deterministic_init62.fs"; "lib_deterministic_init62.fs"; "flag_deterministic_init63.fs"; "lib_deterministic_init63.fs"; "flag_deterministic_init64.fs"; "lib_deterministic_init64.fs"; "flag_deterministic_init65.fs"; "lib_deterministic_init65.fs"; "flag_deterministic_init66.fs"; "lib_deterministic_init66.fs"; "flag_deterministic_init67.fs"; "lib_deterministic_init67.fs"; "flag_deterministic_init68.fs"; "lib_deterministic_init68.fs"; "flag_deterministic_init69.fs"; "lib_deterministic_init69.fs"; "flag_deterministic_init70.fs"; "lib_deterministic_init70.fs"; "flag_deterministic_init71.fs"; "lib_deterministic_init71.fs"; "flag_deterministic_init72.fs"; "lib_deterministic_init72.fs"; "flag_deterministic_init73.fs"; "lib_deterministic_init73.fs"; "flag_deterministic_init74.fs"; "lib_deterministic_init74.fs"; "flag_deterministic_init75.fs"; "lib_deterministic_init75.fs"; "flag_deterministic_init76.fs"; "lib_deterministic_init76.fs"; "flag_deterministic_init77.fs"; "lib_deterministic_init77.fs"; "flag_deterministic_init78.fs"; "lib_deterministic_init78.fs"; "flag_deterministic_init79.fs"; "lib_deterministic_init79.fs"; "flag_deterministic_init80.fs"; "lib_deterministic_init80.fs"; "flag_deterministic_init81.fs"; "lib_deterministic_init81.fs"; "flag_deterministic_init82.fs"; "lib_deterministic_init82.fs"; "flag_deterministic_init83.fs"; "lib_deterministic_init83.fs"; "flag_deterministic_init84.fs"; "lib_deterministic_init84.fs"; "flag_deterministic_init85.fs"; "lib_deterministic_init85.fs"] 

        fsc cfg "%s --optimize- -o test_deterministic_init.exe" cfg.fsc_flags (dicases @ ["test_deterministic_init.fs"])

        peverify cfg "test_deterministic_init.exe"

        fsc cfg "%s --optimize -o test_deterministic_init--optimize.exe" cfg.fsc_flags (dicases @ ["test_deterministic_init.fs"])

        peverify cfg "test_deterministic_init--optimize.exe"

        fsc cfg "%s --optimize- -a -o test_deterministic_init_lib.dll" cfg.fsc_flags dicases

        peverify cfg "test_deterministic_init_lib.dll"

        fsc cfg "%s --optimize- -r test_deterministic_init_lib.dll -o test_deterministic_init_exe.exe" cfg.fsc_flags ["test_deterministic_init.fs"]

        peverify cfg "test_deterministic_init_exe.exe"

        fsc cfg "%s --optimize -a -o test_deterministic_init_lib--optimize.dll" cfg.fsc_flags dicases

        peverify cfg "test_deterministic_init_lib--optimize.dll"

        fsc cfg "%s --optimize -r test_deterministic_init_lib--optimize.dll -o test_deterministic_init_exe--optimize.exe" cfg.fsc_flags ["test_deterministic_init.fs"]

        peverify cfg "test_deterministic_init_exe--optimize.exe"

        let static_init_cases = [ "test0.fs"; "test1.fs"; "test2.fs"; "test3.fs"; "test4.fs"; "test5.fs"; "test6.fs" ]

        fsc cfg "%s --optimize- -o test_static_init.exe" cfg.fsc_flags (static_init_cases @ ["static-main.fs"])

        peverify cfg "test_static_init.exe"

        fsc cfg "%s --optimize -o test_static_init--optimize.exe" cfg.fsc_flags (static_init_cases @ [ "static-main.fs" ])

        peverify cfg "test_static_init--optimize.exe"

        fsc cfg "%s --optimize- -a -o test_static_init_lib.dll" cfg.fsc_flags static_init_cases

        peverify cfg "test_static_init_lib.dll"

        fsc cfg "%s --optimize- -r test_static_init_lib.dll -o test_static_init_exe.exe" cfg.fsc_flags ["static-main.fs"]

        peverify cfg "test_static_init_exe.exe"

        fsc cfg "%s --optimize -a -o test_static_init_lib--optimize.dll" cfg.fsc_flags static_init_cases

        peverify cfg "test_static_init_lib--optimize.dll"

        fsc cfg "%s --optimize -r test_static_init_lib--optimize.dll -o test_static_init_exe--optimize.exe" cfg.fsc_flags ["static-main.fs"]

        peverify cfg "test_static_init_exe--optimize.exe"

        exec cfg ("." ++ "test.exe") ""

        exec cfg ("." ++ "test--optimize.exe") ""

        exec cfg ("." ++ "test_deterministic_init.exe") ""

        exec cfg ("." ++ "test_deterministic_init--optimize.exe") ""

        exec cfg ("." ++ "test_deterministic_init_exe.exe") ""

        exec cfg ("." ++ "test_deterministic_init_exe--optimize.exe") ""

        exec cfg ("." ++ "test_static_init.exe") ""

        exec cfg ("." ++ "test_static_init--optimize.exe") ""

        exec cfg ("." ++ "test_static_init_exe.exe") ""

        exec cfg ("." ++ "test_static_init_exe--optimize.exe") ""
                
    [<Test>]
    let unitsOfMeasure () = 
        let cfg = testConfig "core/unitsOfMeasure"

        fsc cfg "%s --optimize- -o:test.exe -g" cfg.fsc_flags ["test.fs"]

        peverify cfg "test.exe"

        use testOkFile = fileguard cfg "test.ok"

        exec cfg ("." ++ "test.exe") ""

        testOkFile.CheckExists()
                
    [<Test>]
    let verify () = 
        let cfg = testConfig "core/verify"

        peverifyWithArgs cfg "/nologo" (cfg.FSCBinPath ++ "FSharp.Build.dll")

       // peverifyWithArgs cfg "/nologo /MD" (cfg.FSCBinPath ++ "FSharp.Compiler.dll")

        peverifyWithArgs cfg "/nologo" (cfg.FSCBinPath ++ "fsi.exe")

        peverifyWithArgs cfg "/nologo" (cfg.FSCBinPath ++ "FSharp.Compiler.Interactive.Settings.dll")

        fsc cfg "%s -o:xmlverify.exe -g" cfg.fsc_flags ["xmlverify.fs"]

        peverifyWithArgs cfg "/nologo" "xmlverify.exe"
#endif

module ToolsTests = 

    [<Test>]
    let bundle () = 
        let cfg = testConfig "tools/bundle"

        fsc cfg "%s --progress --standalone -o:test-one-fsharp-module.exe -g" cfg.fsc_flags ["test-one-fsharp-module.fs"]
   
        peverify cfg "test-one-fsharp-module.exe"
   
        fsc cfg "%s -a -o:test_two_fsharp_modules_module_1.dll -g" cfg.fsc_flags ["test_two_fsharp_modules_module_1.fs"]
   
        peverify cfg "test_two_fsharp_modules_module_1.dll"
   
        fsc cfg "%s --standalone -r:test_two_fsharp_modules_module_1.dll -o:test_two_fsharp_modules_module_2.exe -g" cfg.fsc_flags ["test_two_fsharp_modules_module_2.fs"]
   
        peverify cfg "test_two_fsharp_modules_module_2.exe"
   
        fsc cfg "%s -a --standalone -r:test_two_fsharp_modules_module_1.dll -o:test_two_fsharp_modules_module_2_as_dll.dll -g" cfg.fsc_flags ["test_two_fsharp_modules_module_2.fs"]
   
        peverify cfg "test_two_fsharp_modules_module_2_as_dll.dll"

#if !FSHARP_SUITE_DRIVES_CORECLR_TESTS
    [<Test>]
    let eval () = singleTestBuildAndRun "tools/eval" FSC_BASIC
#endif


module RegressionTests = 

    [<Test >]
    let ``struct-tuple-bug-1-FSC_BASIC`` () = singleTestBuildAndRun "regression/struct-tuple-bug-1" FSC_BASIC

    [<Test >]
    let ``tuple-bug-1`` () = singleTestBuildAndRun "regression/tuple-bug-1" FSC_BASIC

    [<Test>]
    let ``26`` () = singleTestBuildAndRun "regression/26" FSC_BASIC

    [<Test >]
    let ``321`` () = singleTestBuildAndRun "regression/321" FSC_BASIC

    [<Test>]
    let ``655`` () = 
        let cfg = testConfig "regression/655"

        fsc cfg "%s -a -o:pack.dll" cfg.fsc_flags ["xlibC.ml"]

        peverify cfg "pack.dll"

        fsc cfg "%s    -o:test.exe -r:pack.dll" cfg.fsc_flags ["main.fs"]

        peverify cfg "test.exe"

        use testOkFile = fileguard cfg "test.ok"

        exec cfg ("." ++ "test.exe") ""

        testOkFile.CheckExists()
                
    [<Test >]
    let ``656`` () = 
        let cfg = testConfig "regression/656"

        fsc cfg "%s -o:pack.exe" cfg.fsc_flags ["misc.fs mathhelper.fs filehelper.fs formshelper.fs plot.fs traj.fs playerrecord.fs trackedplayers.fs form.fs"]

        peverify cfg  "pack.exe"
                
#if !FSHARP_SUITE_DRIVES_CORECLR_TESTS
    // Requires WinForms
    [<Test>]
    let ``83`` () = singleTestBuildAndRun "regression/83" FSC_BASIC
#endif
    [<Test >]
    let ``84`` () = singleTestBuildAndRun "regression/84" FSC_BASIC

    [<Test >]
    let ``85`` () = 
        let cfg = testConfig "regression/85"

        fsc cfg "%s -r:Category.dll -a -o:petshop.dll" cfg.fsc_flags ["Category.ml"]

        peverify cfg "petshop.dll"

    [<Test >]
    let ``86`` () = singleTestBuildAndRun "regression/86" FSC_BASIC

    [<Test >]
    let ``struct-tuple-bug-1-FSI_BASIC`` () = singleTestBuildAndRun "regression/struct-tuple-bug-1" FSI_BASIC

    [<Test>]
    let ``struct-measure-bug-1`` () = 
        let cfg = testConfig "regression/struct-measure-bug-1"

        fsc cfg "%s --optimize- -o:test.exe -g" cfg.fsc_flags ["test.fs"]

        peverify cfg "test.exe"

#if !FSHARP_SUITE_DRIVES_CORECLR_TESTS
module OptimizationTests =

    [<Test>]
    let functionSizes () = 
        let cfg = testConfig "optimize/analyses"

        let outFile = "sizes.FunctionSizes.output.test.txt"
        let expectedFile = "sizes.FunctionSizes.output.test.bsl"

        log "== FunctionSizes"
        fscBothToOut cfg outFile "%s --nologo -O --test:FunctionSizes" cfg.fsc_flags ["sizes.fs"] 

        let diff = fsdiff cfg outFile expectedFile

        match diff with
        | "" -> ()
        | _ ->
            Assert.Fail (sprintf "'%s' and '%s' differ; %A" (getfullpath cfg outFile) (getfullpath cfg expectedFile) diff)


    [<Test>]
    let totalSizes () = 
        let cfg = testConfig "optimize/analyses"

        let outFile = "sizes.TotalSizes.output.test.txt"
        let expectedFile = "sizes.TotalSizes.output.test.bsl"

        log "== TotalSizes"
        fscBothToOut cfg outFile "%s --nologo -O --test:TotalSizes" cfg.fsc_flags ["sizes.fs"] 

        let diff = fsdiff cfg outFile expectedFile

        match diff with
        | "" -> ()
        | _ -> Assert.Fail (sprintf "'%s' and '%s' differ; %A" (getfullpath cfg outFile) (getfullpath cfg expectedFile) diff)


    [<Test>]
    let hasEffect () = 
        let cfg = testConfig "optimize/analyses"

        let outFile = "effects.HasEffect.output.test.txt"
        let expectedFile = "effects.HasEffect.output.test.bsl"

        log "== HasEffect"
        fscBothToOut cfg outFile "%s --nologo -O --test:HasEffect" cfg.fsc_flags ["effects.fs"] 

        let diff = fsdiff cfg outFile expectedFile

        match diff with
        | "" -> ()
        | _ -> Assert.Fail (sprintf "'%s' and '%s' differ; %A" (getfullpath cfg outFile) (getfullpath cfg expectedFile) diff)


    [<Test>]
    let noNeedToTailcall () = 
        let cfg = testConfig "optimize/analyses"

        let outFile = "tailcalls.NoNeedToTailcall.output.test.txt"
        let expectedFile = "tailcalls.NoNeedToTailcall.output.test.bsl"

        log "== NoNeedToTailcall"
        fscBothToOut cfg outFile "%s --nologo -O --test:NoNeedToTailcall" cfg.fsc_flags ["tailcalls.fs"] 

        let diff = fsdiff cfg outFile expectedFile

        match diff with
        | "" -> ()
        | _ -> Assert.Fail (sprintf "'%s' and '%s' differ; %A" (getfullpath cfg outFile) (getfullpath cfg expectedFile) diff)


    [<Test>]
    let ``inline`` () = 
        let cfg = testConfig "optimize/inline"

        fsc cfg "%s -g --optimize- --target:library -o:lib.dll" cfg.fsc_flags ["lib.fs"; "lib2.fs"]

        fsc cfg "%s -g --optimize- --target:library -o:lib3.dll -r:lib.dll " cfg.fsc_flags ["lib3.fs"]

        fsc cfg "%s -g --optimize- -o:test.exe -r:lib.dll -r:lib3.dll" cfg.fsc_flags ["test.fs "]

        fsc cfg "%s --optimize --target:library -o:lib--optimize.dll -g" cfg.fsc_flags ["lib.fs"; "lib2.fs"]

        fsc cfg "%s --optimize --target:library -o:lib3--optimize.dll -r:lib--optimize.dll -g" cfg.fsc_flags ["lib3.fs"]

        fsc cfg "%s --optimize -o:test--optimize.exe -g -r:lib--optimize.dll  -r:lib3--optimize.dll" cfg.fsc_flags ["test.fs "]

        ildasm cfg "/nobar /out=test.il" "test.exe"

        ildasm cfg "/nobar /out=test--optimize.il" "test--optimize.exe"

        let ``test--optimize.il`` = 
            File.ReadLines (getfullpath cfg "test--optimize.il")
            |> Seq.filter (fun line -> line.Contains(".locals init"))
            |> List.ofSeq

        match ``test--optimize.il`` with
            | [] -> ()
            | lines -> 
                Assert.Fail (sprintf "Error: optimizations not removed.  Relevant lines from IL file follow: %A" lines)

        let numElim = 
            File.ReadLines (getfullpath cfg "test.il")
            |> Seq.filter (fun line -> line.Contains(".locals init"))
            |> Seq.length

        log "Ran ok - optimizations removed %d textual occurrences of optimizable identifiers from target IL" numElim 

    [<Test>]
    let stats () = 
        let cfg = testConfig "optimize/stats"

        ildasm cfg "/nobar /out=FSharp.Core.il" cfg.FSCOREDLLPATH

        let fscore = File.ReadLines(getfullpath cfg "FSharp.Core.il") |> Seq.toList

        let contains text (s: string) = if s.Contains(text) then 1 else 0

        let typeFunc = fscore |> List.sumBy (contains "extends Microsoft.FSharp.TypeFunc")
        let classes = fscore |> List.sumBy (contains ".class")
        let methods = fscore |> List.sumBy (contains ".method")
        let fields = fscore |> List.sumBy (contains ".field")

        let date = DateTime.Today.ToString("dd/MM/yyyy") // 23/11/2006
        let time = DateTime.Now.ToString("HH:mm:ss.ff") // 16:03:23.40
        let m = sprintf "%s, %s, Microsoft.FSharp-TypeFunc, %d, Microsoft.FSharp-classes, %d,  Microsoft.FSharp-methods, %d, ,  Microsoft.FSharp-fields, %d,  " date time typeFunc classes methods fields

        log "now:"
        log "%s" m
#endif

module TypecheckTests = 
    [<Test>]
    let ``full-rank-arrays`` () = 
        let cfg = testConfig "typecheck/full-rank-arrays"
        SingleTest.singleTestBuildAndRunWithCopyDlls cfg "full-rank-arrays.dll" FSC_BASIC

#if !FX_NO_CONVERTER
    // Converter is not coming back until dotnet standard 2.0
    [<Test>]
    let misc () = singleTestBuildAndRun "typecheck/misc" FSC_BASIC
#endif

#if !FSHARP_SUITE_DRIVES_CORECLR_TESTS
    [<Test>]
    let ``sigs pos24`` () = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos24.exe" cfg.fsc_flags ["pos24.fs"]
        peverify cfg "pos24.exe"

    [<Test>]
    let ``sigs pos23`` () = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos23.exe" cfg.fsc_flags ["pos23.fs"]
        peverify cfg "pos23.exe"
        exec cfg ("." ++ "pos23.exe") ""

    [<Test>]
    let ``sigs pos20`` () = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos20.exe" cfg.fsc_flags ["pos20.fs"]
        peverify cfg "pos20.exe"
        exec cfg ("." ++ "pos20.exe") ""

    [<Test>]
    let ``sigs pos19`` () = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos19.exe" cfg.fsc_flags ["pos19.fs"]
        peverify cfg "pos19.exe"
        exec cfg ("." ++ "pos19.exe") ""

    [<Test>]
    let ``sigs pos18`` () = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos18.exe" cfg.fsc_flags ["pos18.fs"]
        peverify cfg "pos18.exe"
        exec cfg ("." ++ "pos18.exe") ""

    [<Test>]
    let ``sigs pos16`` () = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos16.exe" cfg.fsc_flags ["pos16.fs"]
        peverify cfg "pos16.exe"
        exec cfg ("." ++ "pos16.exe") ""

    [<Test>]
    let ``sigs pos17`` () = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos17.exe" cfg.fsc_flags ["pos17.fs"]
        peverify cfg "pos17.exe"
        exec cfg ("." ++ "pos17.exe") ""

    [<Test>]
    let ``sigs pos15`` () = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos15.exe" cfg.fsc_flags ["pos15.fs"]
        peverify cfg "pos15.exe"
        exec cfg ("." ++ "pos15.exe") ""

    [<Test>]
    let ``sigs pos14`` () = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos14.exe" cfg.fsc_flags ["pos14.fs"]
        peverify cfg "pos14.exe"
        exec cfg ("." ++ "pos14.exe") ""

    [<Test>]
    let ``sigs pos13`` () = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos13.exe" cfg.fsc_flags ["pos13.fs"]
        peverify cfg "pos13.exe"
        exec cfg ("." ++ "pos13.exe") ""

    [<Test>]
    let ``sigs pos12 `` () = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s -a -o:pos12.dll" cfg.fsc_flags ["pos12.fs"]

    [<Test>]
    let ``sigs pos11`` () = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s -a -o:pos11.dll" cfg.fsc_flags ["pos11.fs"]

    [<Test>]
    let ``sigs pos10`` () = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s -a -o:pos10.dll" cfg.fsc_flags ["pos10.fs"]
        peverify cfg "pos10.dll"

    [<Test>]
    let ``sigs pos09`` () = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s -a -o:pos09.dll" cfg.fsc_flags ["pos09.fs"]
        peverify cfg "pos09.dll"

    [<Test>]
    let ``sigs pos07`` () = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s -a -o:pos07.dll" cfg.fsc_flags ["pos07.fs"]
        peverify cfg "pos07.dll"

    [<Test>]
    let ``sigs pos08`` () = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s -a -o:pos08.dll" cfg.fsc_flags ["pos08.fs"]
        peverify cfg "pos08.dll"

    [<Test>]
    let ``sigs pos06`` () = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s -a -o:pos06.dll" cfg.fsc_flags ["pos06.fs"]
        peverify cfg "pos06.dll"

    [<Test>]
    let ``sigs pos03`` () = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s -a -o:pos03.dll" cfg.fsc_flags ["pos03.fs"]
        peverify cfg "pos03.dll"
        fsc cfg "%s -a -o:pos03a.dll" cfg.fsc_flags ["pos03a.fsi"; "pos03a.fs"]
        peverify cfg "pos03a.dll"

    [<Test>]
    let ``sigs pos01a`` () = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s -a -o:pos01a.dll" cfg.fsc_flags ["pos01a.fsi"; "pos01a.fs"]
        peverify cfg "pos01a.dll"

    [<Test>]
    let ``sigs pos02`` () = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s -a -o:pos02.dll" cfg.fsc_flags ["pos02.fs"]
        peverify cfg "pos02.dll"

    [<Test>]
    let ``sigs pos05`` () = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s -a -o:pos05.dll" cfg.fsc_flags ["pos05.fs"]

    [<Test>] 
    let ``type check neg01`` () = singleNegTest (testConfig "typecheck/sigs") "neg01"

    [<Test>] 
    let ``type check neg02`` () = singleNegTest (testConfig "typecheck/sigs") "neg02"

    [<Test>] 
    let ``type check neg03`` () = singleNegTest (testConfig "typecheck/sigs") "neg03"

    [<Test>] 
    let ``type check neg04`` () = singleNegTest (testConfig "typecheck/sigs") "neg04"

    [<Test>] 
    let ``type check neg05`` () = singleNegTest (testConfig "typecheck/sigs") "neg05"

    [<Test>] 
    let ``type check neg06`` () = singleNegTest (testConfig "typecheck/sigs") "neg06"

    [<Test>] 
    let ``type check neg06_a`` () = singleNegTest (testConfig "typecheck/sigs") "neg06_a"

    [<Test>] 
    let ``type check neg06_b`` () = singleNegTest (testConfig "typecheck/sigs") "neg06_b"

    [<Test>] 
    let ``type check neg07`` () = singleNegTest (testConfig "typecheck/sigs") "neg07"

    [<Test>] 
    let ``type check neg08`` () = singleNegTest (testConfig "typecheck/sigs") "neg08"

    [<Test>] 
    let ``type check neg09`` () = singleNegTest (testConfig "typecheck/sigs") "neg09"

    [<Test>] 
    let ``type check neg10`` () = singleNegTest (testConfig "typecheck/sigs") "neg10"

    [<Test>] 
    let ``type check neg10_a`` () = singleNegTest (testConfig "typecheck/sigs") "neg10_a"

    [<Test>] 
    let ``type check neg11`` () = singleNegTest (testConfig "typecheck/sigs") "neg11"

    [<Test>] 
    let ``type check neg12`` () = singleNegTest (testConfig "typecheck/sigs") "neg12"

    [<Test>] 
    let ``type check neg13`` () = singleNegTest (testConfig "typecheck/sigs") "neg13"

    [<Test>] 
    let ``type check neg14`` () = singleNegTest (testConfig "typecheck/sigs") "neg14"

    [<Test>] 
    let ``type check neg15`` () = singleNegTest (testConfig "typecheck/sigs") "neg15"

    [<Test>] 
    let ``type check neg16`` () = singleNegTest (testConfig "typecheck/sigs") "neg16"

    [<Test>] 
    let ``type check neg17`` () = singleNegTest (testConfig "typecheck/sigs") "neg17"

    [<Test>] 
    let ``type check neg18`` () = singleNegTest (testConfig "typecheck/sigs") "neg18"

    [<Test>] 
    let ``type check neg19`` () = singleNegTest (testConfig "typecheck/sigs") "neg19"

    [<Test>] 
    let ``type check neg20`` () = singleNegTest (testConfig "typecheck/sigs") "neg20"

    [<Test>] 
    let ``type check neg21`` () = singleNegTest (testConfig "typecheck/sigs") "neg21"

    [<Test>] 
    let ``type check neg22`` () = singleNegTest (testConfig "typecheck/sigs") "neg22"

    [<Test>] 
    let ``type check neg23`` () = singleNegTest (testConfig "typecheck/sigs") "neg23"

    [<Test>] 
    let ``type check neg24`` () = singleNegTest (testConfig "typecheck/sigs") "neg24"

    [<Test>] 
    let ``type check neg25`` () = singleNegTest (testConfig "typecheck/sigs") "neg25"

    [<Test>] 
    let ``type check neg26`` () = singleNegTest (testConfig "typecheck/sigs") "neg26"

    [<Test>] 
    let ``type check neg27`` () = singleNegTest (testConfig "typecheck/sigs") "neg27"

    [<Test>] 
    let ``type check neg28`` () = singleNegTest (testConfig "typecheck/sigs") "neg28"

    [<Test>] 
    let ``type check neg29`` () = singleNegTest (testConfig "typecheck/sigs") "neg29"

    [<Test>] 
    let ``type check neg30`` () = singleNegTest (testConfig "typecheck/sigs") "neg30"

    [<Test>] 
    let ``type check neg31`` () = singleNegTest (testConfig "typecheck/sigs") "neg31"

    [<Test>] 
    let ``type check neg32`` () = singleNegTest (testConfig "typecheck/sigs") "neg32"

    [<Test>] 
    let ``type check neg33`` () = singleNegTest (testConfig "typecheck/sigs") "neg33"

    [<Test>] 
    let ``type check neg34`` () = singleNegTest (testConfig "typecheck/sigs") "neg34"

    [<Test>] 
    let ``type check neg35`` () = singleNegTest (testConfig "typecheck/sigs") "neg35"

    [<Test>] 
    let ``type check neg36`` () = singleNegTest (testConfig "typecheck/sigs") "neg36"

    [<Test>] 
    let ``type check neg37`` () = singleNegTest (testConfig "typecheck/sigs") "neg37"

    [<Test>] 
    let ``type check neg37_a`` () = singleNegTest (testConfig "typecheck/sigs") "neg37_a"

    [<Test>] 
    let ``type check neg38`` () = singleNegTest (testConfig "typecheck/sigs") "neg38"

    [<Test>] 
    let ``type check neg39`` () = singleNegTest (testConfig "typecheck/sigs") "neg39"

    [<Test>] 
    let ``type check neg40`` () = singleNegTest (testConfig "typecheck/sigs") "neg40"

    [<Test>] 
    let ``type check neg41`` () = singleNegTest (testConfig "typecheck/sigs") "neg41"

    [<Test>] 
    let ``type check neg42`` () = singleNegTest (testConfig "typecheck/sigs") "neg42"

    [<Test>] 
    let ``type check neg43`` () = singleNegTest (testConfig "typecheck/sigs") "neg43"

    [<Test>] 
    let ``type check neg44`` () = singleNegTest (testConfig "typecheck/sigs") "neg44"

    [<Test>] 
    let ``type check neg45`` () = singleNegTest (testConfig "typecheck/sigs") "neg45"

    [<Test>] 
    let ``type check neg46`` () = singleNegTest (testConfig "typecheck/sigs") "neg46"

    [<Test>] 
    let ``type check neg47`` () = singleNegTest (testConfig "typecheck/sigs") "neg47"

    [<Test>] 
    let ``type check neg48`` () = singleNegTest (testConfig "typecheck/sigs") "neg48"

    [<Test>] 
    let ``type check neg49`` () = singleNegTest (testConfig "typecheck/sigs") "neg49"

    [<Test>] 
    let ``type check neg50`` () = singleNegTest (testConfig "typecheck/sigs") "neg50"

    [<Test>] 
    let ``type check neg51`` () = singleNegTest (testConfig "typecheck/sigs") "neg51"

    [<Test>] 
    let ``type check neg52`` () = singleNegTest (testConfig "typecheck/sigs") "neg52"

    [<Test>] 
    let ``type check neg53`` () = singleNegTest (testConfig "typecheck/sigs") "neg53"

    [<Test>] 
    let ``type check neg54`` () = singleNegTest (testConfig "typecheck/sigs") "neg54"

    [<Test>] 
    let ``type check neg55`` () = singleNegTest (testConfig "typecheck/sigs") "neg55"

    [<Test>] 
    let ``type check neg56`` () = singleNegTest (testConfig "typecheck/sigs") "neg56"

    [<Test>] 
    let ``type check neg56_a`` () = singleNegTest (testConfig "typecheck/sigs") "neg56_a"

    [<Test>] 
    let ``type check neg56_b`` () = singleNegTest (testConfig "typecheck/sigs") "neg56_b"

    [<Test>] 
    let ``type check neg57`` () = singleNegTest (testConfig "typecheck/sigs") "neg57"

    [<Test>] 
    let ``type check neg58`` () = singleNegTest (testConfig "typecheck/sigs") "neg58"

    [<Test>] 
    let ``type check neg59`` () = singleNegTest (testConfig "typecheck/sigs") "neg59"

    [<Test>] 
    let ``type check neg60`` () = singleNegTest (testConfig "typecheck/sigs") "neg60"

    [<Test>] 
    let ``type check neg61`` () = singleNegTest (testConfig "typecheck/sigs") "neg61"

    [<Test>] 
    let ``type check neg62`` () = singleNegTest (testConfig "typecheck/sigs") "neg62"

    [<Test>] 
    let ``type check neg63`` () = singleNegTest (testConfig "typecheck/sigs") "neg63"

    [<Test>] 
    let ``type check neg64`` () = singleNegTest (testConfig "typecheck/sigs") "neg64"

    [<Test>] 
    let ``type check neg65`` () = singleNegTest (testConfig "typecheck/sigs") "neg65"

    [<Test>] 
    let ``type check neg66`` () = singleNegTest (testConfig "typecheck/sigs") "neg66"

    [<Test>] 
    let ``type check neg67`` () = singleNegTest (testConfig "typecheck/sigs") "neg67"

    [<Test>] 
    let ``type check neg68`` () = singleNegTest (testConfig "typecheck/sigs") "neg68"

    [<Test>] 
    let ``type check neg69`` () = singleNegTest (testConfig "typecheck/sigs") "neg69"

    [<Test>] 
    let ``type check neg70`` () = singleNegTest (testConfig "typecheck/sigs") "neg70"

    [<Test>] 
    let ``type check neg71`` () = singleNegTest (testConfig "typecheck/sigs") "neg71"

    [<Test>] 
    let ``type check neg72`` () = singleNegTest (testConfig "typecheck/sigs") "neg72"

    [<Test>] 
    let ``type check neg73`` () = singleNegTest (testConfig "typecheck/sigs") "neg73"

    [<Test>] 
    let ``type check neg74`` () = singleNegTest (testConfig "typecheck/sigs") "neg74"

    [<Test>] 
    let ``type check neg75`` () = singleNegTest (testConfig "typecheck/sigs") "neg75"

    [<Test>] 
    let ``type check neg76`` () = singleNegTest (testConfig "typecheck/sigs") "neg76"

    [<Test>] 
    let ``type check neg77`` () = singleNegTest (testConfig "typecheck/sigs") "neg77"

    [<Test>] 
    let ``type check neg78`` () = singleNegTest (testConfig "typecheck/sigs") "neg78"

    [<Test>] 
    let ``type check neg79`` () = singleNegTest (testConfig "typecheck/sigs") "neg79"

    [<Test>] 
    let ``type check neg80`` () = singleNegTest (testConfig "typecheck/sigs") "neg80"

    [<Test>] 
    let ``type check neg81`` () = singleNegTest (testConfig "typecheck/sigs") "neg81"

    [<Test>] 
    let ``type check neg82`` () = singleNegTest (testConfig "typecheck/sigs") "neg82"

    [<Test>] 
    let ``type check neg83`` () = singleNegTest (testConfig "typecheck/sigs") "neg83"

    [<Test>] 
    let ``type check neg84`` () = singleNegTest (testConfig "typecheck/sigs") "neg84"

    [<Test>] 
    let ``type check neg85`` () = singleNegTest (testConfig "typecheck/sigs") "neg85"

    [<Test>] 
    let ``type check neg86`` () = singleNegTest (testConfig "typecheck/sigs") "neg86"

    [<Test>] 
    let ``type check neg87`` () = singleNegTest (testConfig "typecheck/sigs") "neg87"

    [<Test>] 
    let ``type check neg88`` () = singleNegTest (testConfig "typecheck/sigs") "neg88"

    [<Test>] 
    let ``type check neg89`` () = singleNegTest (testConfig "typecheck/sigs") "neg89"

    [<Test>] 
    let ``type check neg90`` () = singleNegTest (testConfig "typecheck/sigs") "neg90"

    [<Test>] 
    let ``type check neg91`` () = singleNegTest (testConfig "typecheck/sigs") "neg91"

    [<Test>] 
    let ``type check neg92`` () = singleNegTest (testConfig "typecheck/sigs") "neg92"

    [<Test>] 
    let ``type check neg93`` () = singleNegTest (testConfig "typecheck/sigs") "neg93"

    [<Test>] 
    let ``type check neg94`` () = singleNegTest (testConfig "typecheck/sigs") "neg94"

    [<Test>] 
    let ``type check neg95`` () = singleNegTest (testConfig "typecheck/sigs") "neg95"

    [<Test>] 
    let ``type check neg96`` () = singleNegTest (testConfig "typecheck/sigs") "neg96"

    [<Test>] 
    let ``type check neg97`` () = singleNegTest (testConfig "typecheck/sigs") "neg97"

    [<Test>] 
    let ``type check neg_byref_1`` () = singleNegTest (testConfig "typecheck/sigs") "neg_byref_1"

    [<Test>] 
    let ``type check neg_byref_2`` () = singleNegTest (testConfig "typecheck/sigs") "neg_byref_2"

    [<Test>] 
    let ``type check neg_byref_3`` () = singleNegTest (testConfig "typecheck/sigs") "neg_byref_3"

    [<Test>] 
    let ``type check neg_byref_4`` () = singleNegTest (testConfig "typecheck/sigs") "neg_byref_4"

    [<Test>] 
    let ``type check neg_byref_5`` () = singleNegTest (testConfig "typecheck/sigs") "neg_byref_5"

    [<Test>] 
    let ``type check neg_byref_6`` () = singleNegTest (testConfig "typecheck/sigs") "neg_byref_6"

    [<Test>] 
    let ``type check neg_byref_7`` () = singleNegTest (testConfig "typecheck/sigs") "neg_byref_7"

    [<Test>] 
    let ``type check neg_byref_8`` () = singleNegTest (testConfig "typecheck/sigs") "neg_byref_8"

    [<Test>] 
    let ``type check neg_byref_10`` () = singleNegTest (testConfig "typecheck/sigs") "neg_byref_10"

    [<Test>] 
    let ``type check neg_byref_11`` () = singleNegTest (testConfig "typecheck/sigs") "neg_byref_11"

    [<Test>] 
    let ``type check neg_byref_12`` () = singleNegTest (testConfig "typecheck/sigs") "neg_byref_12"

    [<Test>] 
    let ``type check neg_byref_13`` () = singleNegTest (testConfig "typecheck/sigs") "neg_byref_13"

    [<Test>] 
    let ``type check neg_byref_14`` () = singleNegTest (testConfig "typecheck/sigs") "neg_byref_14"

    [<Test>] 
    let ``type check neg_byref_15`` () = singleNegTest (testConfig "typecheck/sigs") "neg_byref_15"

    [<Test>] 
    let ``type check neg_byref_16`` () = singleNegTest (testConfig "typecheck/sigs") "neg_byref_16"

    [<Test>] 
    let ``type check neg_byref_17`` () = singleNegTest (testConfig "typecheck/sigs") "neg_byref_17"

    [<Test>] 
    let ``type check neg_byref_18`` () = singleNegTest (testConfig "typecheck/sigs") "neg_byref_18"

    [<Test>] 
    let ``type check neg_byref_19`` () = singleNegTest (testConfig "typecheck/sigs") "neg_byref_19"

    [<Test>] 
    let ``type check neg_byref_20`` () = singleNegTest (testConfig "typecheck/sigs") "neg_byref_20"

    [<Test>] 
    let ``type check neg_byref_21`` () = singleNegTest (testConfig "typecheck/sigs") "neg_byref_21"

    [<Test>] 
    let ``type check neg_byref_22`` () = singleNegTest (testConfig "typecheck/sigs") "neg_byref_22"

    [<Test>] 
    let ``type check neg_byref_23`` () = singleNegTest (testConfig "typecheck/sigs") "neg_byref_23"

module FscTests =                 
    [<Test>]
    let ``should be raised if AssemblyInformationalVersion has invalid version`` () = 
        let cfg = testConfig (Commands.createTempDir())

        let code  =
            """
namespace CST.RI.Anshun
open System.Reflection
[<assembly: AssemblyVersion("4.5.6.7")>]
[<assembly: AssemblyInformationalVersion("45.2048.main1.2-hotfix (upgrade Second Chance security)")>]
()
            """

        File.WriteAllText(cfg.Directory ++ "test.fs", code)

        fsc cfg "%s --nologo -o:lib.dll --target:library" cfg.fsc_flags ["test.fs"]

        let fv = Diagnostics.FileVersionInfo.GetVersionInfo(Commands.getfullpath cfg.Directory "lib.dll")

        fv.ProductVersion |> Assert.areEqual "45.2048.main1.2-hotfix (upgrade Second Chance security)"

        (fv.ProductMajorPart, fv.ProductMinorPart, fv.ProductBuildPart, fv.ProductPrivatePart) 
        |> Assert.areEqual (45,2048,0,0)


    [<Test>]
    let ``should set file version info on generated file`` () = 
        let cfg = testConfig (Commands.createTempDir())

        let code =
            """
namespace CST.RI.Anshun
open System.Reflection
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
[<assembly: AssemblyTitle("CST.RI.Anshun.TreloarStation")>]
[<assembly: AssemblyDescription("Assembly is a part of Restricted Intelligence of Anshun planet")>]
[<assembly: AssemblyConfiguration("RELEASE")>]
[<assembly: AssemblyCompany("Compressed Space Transport")>]
[<assembly: AssemblyProduct("CST.RI.Anshun")>]
[<assembly: AssemblyCopyright("Copyright \u00A9 Compressed Space Transport 2380")>]
[<assembly: AssemblyTrademark("CST \u2122")>]
[<assembly: AssemblyVersion("12.34.56.78")>]
[<assembly: AssemblyFileVersion("99.88.77.66")>]
[<assembly: AssemblyInformationalVersion("17.56.2912.14")>]
()
            """

        File.WriteAllText(cfg.Directory ++ "test.fs", code)

        do fsc cfg "%s --nologo -o:lib.dll --target:library" cfg.fsc_flags ["test.fs"]

        let fv = System.Diagnostics.FileVersionInfo.GetVersionInfo(Commands.getfullpath cfg.Directory "lib.dll")
        fv.CompanyName |> Assert.areEqual "Compressed Space Transport"
        fv.FileVersion |> Assert.areEqual "99.88.77.66"
        
        (fv.FileMajorPart, fv.FileMinorPart, fv.FileBuildPart, fv.FilePrivatePart)
        |> Assert.areEqual (99,88,77,66)
        
        fv.ProductVersion |> Assert.areEqual "17.56.2912.14"
        (fv.ProductMajorPart, fv.ProductMinorPart, fv.ProductBuildPart, fv.ProductPrivatePart) 
        |> Assert.areEqual (17,56,2912,14)
        
        fv.LegalCopyright |> Assert.areEqual "Copyright \u00A9 Compressed Space Transport 2380"
        fv.LegalTrademarks |> Assert.areEqual "CST \u2122"
#endif

#if !FX_PORTABLE_OR_NETSTANDARD
module ProductVersionTest =

    let informationalVersionAttrName = typeof<System.Reflection.AssemblyInformationalVersionAttribute>.FullName
    let fileVersionAttrName = typeof<System.Reflection.AssemblyFileVersionAttribute>.FullName

    let fallbackTestData () =
        let defAssemblyVersion = (1us,2us,3us,4us)
        let defAssemblyVersionString = let v1,v2,v3,v4 = defAssemblyVersion in sprintf "%d.%d.%d.%d" v1 v2 v3 v4
        [ defAssemblyVersionString, None, None, defAssemblyVersionString
          defAssemblyVersionString, (Some "5.6.7.8"), None, "5.6.7.8"
          defAssemblyVersionString, (Some "5.6.7.8" ), (Some "22.44.66.88"), "22.44.66.88"
          defAssemblyVersionString, None, (Some "22.44.66.88" ), "22.44.66.88" ]
        |> List.map (fun (a,f,i,e) -> (a, f, i, e))

    [<Test>]
    let ``should use correct fallback``() =
      
       for (assemblyVersion, fileVersion, infoVersion, expected) in fallbackTestData () do
        let cfg = testConfig (Commands.createTempDir())
        let dir = cfg.Directory

        printfn "Directory: %s" dir

        let code =
            let globalAssembly (attr: Type) attrValue =
                sprintf """[<assembly: %s("%s")>]""" attr.FullName attrValue

            let attrs =
                [ assemblyVersion |> (globalAssembly typeof<AssemblyVersionAttribute> >> Some)
                  fileVersion |> Option.map (globalAssembly typeof<AssemblyFileVersionAttribute>)
                  infoVersion |> Option.map (globalAssembly typeof<AssemblyInformationalVersionAttribute>) ]
                |> List.choose id

            sprintf """
namespace CST.RI.Anshun
%s
()
            """ (attrs |> String.concat Environment.NewLine)

        File.WriteAllText(cfg.Directory ++ "test.fs", code)

        fsc cfg "%s --nologo -o:lib.dll --target:library" cfg.fsc_flags ["test.fs"]

        let fileVersionInfo = Diagnostics.FileVersionInfo.GetVersionInfo(Commands.getfullpath cfg.Directory "lib.dll")

        fileVersionInfo.ProductVersion |> Assert.areEqual expected

module GeneratedSignatureTests =
    [<Test>]
    let ``members-basics-GENERATED_SIGNATURE`` () = singleTestBuildAndRun "core/members/basics" GENERATED_SIGNATURE

    [<Test>]
    let ``access-GENERATED_SIGNATURE``() = singleTestBuildAndRun "core/access" GENERATED_SIGNATURE

    [<Test>]
    let ``array-GENERATED_SIGNATURE``() = singleTestBuildAndRun "core/array" GENERATED_SIGNATURE

    [<Test; Ignore("incorrect signature file generated, test has been disabled a long time")>]
    let ``genericmeasures-GENERATED_SIGNATURE`` () = singleTestBuildAndRun "core/genericmeasures" GENERATED_SIGNATURE

    [<Test>]
    let ``innerpoly-GENERATED_SIGNATURE`` () = singleTestBuildAndRun "core/innerpoly" GENERATED_SIGNATURE

    [<Test>]
    let ``measures-GENERATED_SIGNATURE`` () = singleTestBuildAndRun "core/measures" GENERATED_SIGNATURE
#endif
