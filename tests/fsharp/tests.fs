﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Tests.Core

open System
open System.IO
open System.Reflection
open System.Reflection.PortableExecutable
open NUnit.Framework
open TestFramework
open Scripting
open SingleTest
open HandleExpects
open FSharp.Test

#if NETCOREAPP
// Use these lines if you want to test CoreCLR
let FSC_OPTIMIZED = FSC_NETCORE (true, false)
let FSC_DEBUG = FSC_NETCORE (false, false)
let FSC_BUILDONLY optimized = FSC_NETCORE (optimized, true)
let FSI = FSI_NETCORE
#else
let FSC_OPTIMIZED = FSC_NETFX (true, false)
let FSC_DEBUG = FSC_NETFX (false, false)
let FSC_BUILDONLY optimized = FSC_NETFX (optimized, true)
let FSI = FSI_NETFX
#endif
// ^^^^^^^^^^^^ To run these tests in F# Interactive , 'build net40', then send this chunk, then evaluate body of a test ^^^^^^^^^^^^

let inline getTestsDirectory dir = getTestsDirectory __SOURCE_DIRECTORY__ dir
let singleTestBuildAndRun = getTestsDirectory >> singleTestBuildAndRun
let singleTestBuildAndRunVersion = getTestsDirectory >> singleTestBuildAndRunVersion
let testConfig = getTestsDirectory >> testConfig


[<NonParallelizable; SetUICulture("en-US"); SetCulture("en-US")>]
module CoreTests =


#if !NETCOREAPP
    [<Test>]
    let ``subtype-langversion-preview-checknulls`` () =
        let cfg = testConfig "core/subtype"

        use testOkFile = fileguard cfg "test.ok"

        fsc cfg "%s -o:test-checknulls.exe -g --langversion:preview --checknulls" cfg.fsc_flags ["test.fsx"]

        exec cfg ("." ++ "test-checknulls.exe") ""

        testOkFile.CheckExists()

    [<Test>]
    let ``subtype-langversion-preview-no-checknulls`` () =
        let cfg = testConfig "core/subtype"

        use testOkFile = fileguard cfg "test.ok"

        fsc cfg "%s -o:test-no-checknulls.exe -g --langversion:preview --checknulls-" cfg.fsc_flags ["test.fsx"]

        exec cfg ("." ++ "test-no-checknulls.exe") ""

        testOkFile.CheckExists()

    [<Test>]
    let ``subtype-langversion-46`` () =
        let cfg = testConfig "core/subtype"

        use testOkFile = fileguard cfg "test.ok"

        fsc cfg "%s -o:test-langversion-46.exe -g --langversion:4.6" cfg.fsc_flags ["test.fsx"]

        exec cfg ("." ++ "test-langversion-46.exe") ""

        testOkFile.CheckExists()

    // This test stays in FsharpSuite for a later migration phases, it uses hardcoded #r to a C# compiled cslib.dll inside
    [<Test>]
    let ``quotes-FSC-FSC_DEBUG`` () = singleTestBuildAndRun "core/quotes" FSC_DEBUG

    [<Test>]
    let ``quotes-FSC-BASIC`` () = singleTestBuildAndRun "core/quotes" FSC_OPTIMIZED

    [<Test>]
    let ``quotes-FSI-BASIC`` () = singleTestBuildAndRun "core/quotes" FSI
#endif


    [<Test>]
    let ``SDKTests`` () =
        let cfg = testConfig "SDKTests"
        exec cfg cfg.DotNetExe ("msbuild " + Path.Combine(cfg.Directory, "AllSdkTargetsTests.proj") + " /p:Configuration=" + cfg.BUILD_CONFIG)

#if !NETCOREAPP
    [<Test>]
    let ``attributes-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/attributes" FSC_OPTIMIZED

    [<Test>]
    let ``attributes-FSI`` () = singleTestBuildAndRun "core/attributes" FSI

    [<Test>]
    let span () =

        let cfg = testConfig "core/span"

        let cfg = { cfg with fsc_flags = sprintf "%s --preferreduilang:en-US --test:StackSpan" cfg.fsc_flags}

        begin
            use testOkFile = fileguard cfg "test.ok"

            singleNegTest cfg "test"

            fsc cfg "%s -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

            // Execution is disabled until we can be sure .NET 4.7.2 is on the machine
            //exec cfg ("." ++ "test.exe") ""

            //testOkFile.CheckExists()
        end

        begin
            use testOkFile = fileguard cfg "test2.ok"

            singleNegTest cfg "test2"

            fsc cfg "%s -o:test2.exe -g" cfg.fsc_flags ["test2.fsx"]

            // Execution is disabled until we can be sure .NET 4.7.2 is on the machine
            //exec cfg ("." ++ "test.exe") ""

            //testOkFile.CheckExists()
        end

        begin
            use testOkFile = fileguard cfg "test3.ok"

            singleNegTest cfg "test3"

            fsc cfg "%s -o:test3.exe -g" cfg.fsc_flags ["test3.fsx"]

            // Execution is disabled until we can be sure .NET 4.7.2 is on the machine
            //exec cfg ("." ++ "test.exe") ""

            //testOkFile.CheckExists()
        end

    [<Test>]
    let asyncStackTraces () =
        let cfg = testConfig "core/asyncStackTraces"

        use testOkFile = fileguard cfg "test.ok"

        fsc cfg "%s -o:test.exe -g --tailcalls- --optimize-" cfg.fsc_flags ["test.fsx"]

        exec cfg ("." ++ "test.exe") ""

        testOkFile.CheckExists()

    [<Test>]
    let ``state-machines-non-optimized`` () = 
        let cfg = testConfig "core/state-machines"

        use testOkFile = fileguard cfg "test.ok"

        fsc cfg "%s -o:test.exe -g --tailcalls- --optimize- --langversion:preview" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test.exe"

        exec cfg ("." ++ "test.exe") ""

        testOkFile.CheckExists()

    [<Test>]
    let ``state-machines-optimized`` () = 
        let cfg = testConfig "core/state-machines"

        use testOkFile = fileguard cfg "test.ok"

        fsc cfg "%s -o:test.exe -g --tailcalls+ --optimize+ --langversion:preview" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test.exe"

        exec cfg ("." ++ "test.exe") ""

        testOkFile.CheckExists()

    [<Test>]
    let ``state-machines neg-resumable-01`` () =
        let cfg = testConfig "core/state-machines"
        singleVersionedNegTest cfg "preview" "neg-resumable-01"


    [<Test>]
    let ``state-machines neg-resumable-02`` () =
        let cfg = testConfig "core/state-machines"
        singleVersionedNegTest cfg "preview" "neg-resumable-02"

    [<Test>]
    let ``lots-of-conditionals``() =
        let cfg = testConfig "core/large/conditionals"
        use testOkFile = fileguard cfg "test.ok"
        fsc cfg "%s -o:test.exe " cfg.fsc_flags ["LargeConditionals-200.fs"]
        exec cfg ("." ++ "test.exe") ""
        testOkFile.CheckExists()

    [<Test>]
    let ``lots-of-conditionals-maxtested``() =
        let cfg = testConfig "core/large/conditionals"
        use testOkFile = fileguard cfg "test.ok"
        fsc cfg "%s -o:test.exe " cfg.fsc_flags ["LargeConditionals-maxtested.fs"]
        exec cfg ("." ++ "test.exe") ""
        testOkFile.CheckExists()

    [<Test>]
    let ``lots-of-lets``() =
        let cfg = testConfig "core/large/lets"
        use testOkFile = fileguard cfg "test.ok"
        fsc cfg "%s -o:test.exe " cfg.fsc_flags ["LargeLets-500.fs"]
        exec cfg ("." ++ "test.exe") ""
        testOkFile.CheckExists()

    [<Test>]
    let ``lots-of-lets-maxtested``() =
        let cfg = testConfig "core/large/lets"
        use testOkFile = fileguard cfg "test.ok"
        fsc cfg "%s -o:test.exe " cfg.fsc_flags ["LargeLets-maxtested.fs"]
        exec cfg ("." ++ "test.exe") ""
        testOkFile.CheckExists()

    [<Test>]
    let ``lots-of-lists``() =
        let cfg = testConfig "core/large/lists"
        use testOkFile = fileguard cfg "test.ok"
        fsc cfg "%s -o:test-500.exe " cfg.fsc_flags ["LargeList-500.fs"]
        exec cfg ("." ++ "test-500.exe") ""
        testOkFile.CheckExists()

    [<Test>]
    let ``lots-of-matches``() =
        let cfg = testConfig "core/large/matches"
        use testOkFile = fileguard cfg "test.ok"
        fsc cfg "%s -o:test.exe " cfg.fsc_flags ["LargeMatches-200.fs"]
        exec cfg ("." ++ "test.exe") ""
        testOkFile.CheckExists()

    [<Test>]
    let ``lots-of-matches-maxtested``() =
        let cfg = testConfig "core/large/matches"
        use testOkFile = fileguard cfg "test.ok"
        fsc cfg "%s -o:test.exe " cfg.fsc_flags ["LargeMatches-maxtested.fs"]
        exec cfg ("." ++ "test.exe") ""
        testOkFile.CheckExists()

    [<Test>]
    let ``lots-of-sequential-and-let``() =
        let cfg = testConfig "core/large/mixed"
        use testOkFile = fileguard cfg "test.ok"
        fsc cfg "%s -o:test.exe " cfg.fsc_flags ["LargeSequentialLet-500.fs"]
        exec cfg ("." ++ "test.exe") ""
        testOkFile.CheckExists()

    [<Test>]
    let ``lots-of-sequential-and-let-maxtested``() =
        let cfg = testConfig "core/large/mixed"
        use testOkFile = fileguard cfg "test.ok"
        fsc cfg "%s -o:test.exe " cfg.fsc_flags ["LargeSequentialLet-maxtested.fs"]
        exec cfg ("." ++ "test.exe") ""
        testOkFile.CheckExists()

    [<Test>]
    let ``lots-of-sequential``() =
        let cfg = testConfig "core/large/sequential"
        use testOkFile = fileguard cfg "test.ok"
        fsc cfg "%s -o:test.exe " cfg.fsc_flags ["LargeSequential-500.fs"]
        exec cfg ("." ++ "test.exe") ""
        testOkFile.CheckExists()

    [<Test>]
    let ``lots-of-sequential-maxtested``() =
        let cfg = testConfig "core/large/sequential"
        use testOkFile = fileguard cfg "test.ok"
        fsc cfg "%s -o:test.exe " cfg.fsc_flags ["LargeSequential-maxtested.fs"]
        exec cfg ("." ++ "test.exe") ""
        testOkFile.CheckExists()

#endif



#if !NETCOREAPP

    // Requires winforms will not run on coreclr
    [<Test>]
    let controlWpf () = singleTestBuildAndRun "core/controlwpf" FSC_OPTIMIZED

    // These tests are enabled for .NET Framework
    [<Test>]
    let ``anon-FSC_OPTIMIZED``() =
        let cfg = testConfig "core/anon"

        fsc cfg "%s -a -o:lib.dll" cfg.fsc_flags ["lib.fs"]

        peverify cfg "lib.dll"

        fsc cfg "%s -r:lib.dll" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test.exe"

        begin
            use testOkFile = fileguard cfg "test.ok"

            exec cfg ("." ++ "test.exe") ""

            testOkFile.CheckExists()
        end

        begin
            use testOkFile = fileguard cfg "test.ok"

            fsi cfg "-r:lib.dll" ["test.fsx"]

            testOkFile.CheckExists()
        end

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
    //        let cfg = testConfig' ()
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
    //        let cfg = testConfig' ()
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

        copy cfg ("orig" ++ "b.dll") ("split" ++ "b.dll")

        copy cfg ("orig" ++ "c.dll") ("split" ++ "c.dll")

        fsc cfg """-o:orig\test.exe -r:orig\b.dll -r:orig\a.dll""" ["test.fs"]

        fsc cfg """-o:split\test.exe -r:split\b.dll -r:split\a-part1.dll -r:split\a.dll""" ["test.fs"]

        fsc cfg """-o:split\test-against-c.exe -r:split\c.dll -r:split\a-part1.dll -r:split\a.dll""" ["test.fs"]

        peverify cfg ("split" ++ "a-part1.dll")

        peverify cfg ("split" ++ "b.dll")

        peverify cfg ("split" ++ "c.dll")

    [<Test>]
    let xmldoc () =
        let cfg = testConfig "core/xmldoc"

        fsc cfg "%s -a --doc:lib.xml -o:lib.dll -g" cfg.fsc_flags ["lib.fs"]
        let outFile = "lib.xml"
        let expectedFile = "lib.xml.bsl"

        if not (fileExists cfg expectedFile) then
            copy cfg outFile expectedFile

        let diffs = fsdiff cfg outFile expectedFile
        match diffs with
        | "" -> ()
        | _ -> Assert.Fail (sprintf "'%s' and '%s' differ; %A" outFile expectedFile diffs)

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

        csc cfg """/nologo /target:library /r:"%s" /r:lib.dll /out:lib2.dll /langversion:7.2""" cfg.FSCOREDLLPATH ["lib2.cs"]

        csc cfg """/nologo /target:library /r:"%s" /out:lib3.dll  /langversion:7.2""" cfg.FSCOREDLLPATH ["lib3.cs"]

        // all features available in preview
        fsc cfg "%s -r:lib.dll -r:lib2.dll -r:lib3.dll -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test.exe"

        exec cfg ("." ++ "test.exe") ""

        // Same with library references the other way around
        fsc cfg "%s -r:lib.dll -r:lib3.dll -r:lib2.dll -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test.exe"

        exec cfg ("." ++ "test.exe") ""

        // Same without the reference to lib.dll - testing an incomplete reference set, but only compiling a subset of the code
        fsc cfg "%s --define:NO_LIB_REFERENCE -r:lib3.dll -r:lib2.dll -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test.exe"

        exec cfg ("." ++ "test.exe") ""

        // some features missing in 4.7
        for version in ["4.7"] do
            let outFile = "compilation.langversion.old.output.txt"
            let expectedFile = "compilation.langversion.old.output.bsl"
            fscBothToOutExpectFail cfg outFile "%s -r:lib.dll -r:lib2.dll -r:lib3.dll -o:test.exe -g --nologo --langversion:%s" cfg.fsc_flags version ["test.fsx"]

            let diffs = fsdiff cfg outFile expectedFile
            match diffs with
            | "" -> ()
            | _ -> Assert.Fail (sprintf "'%s' and '%s' differ; %A" outFile expectedFile diffs)

        // check error messages for some cases
        let outFile = "compilation.errors.output.txt"
        let expectedFile = "compilation.errors.output.bsl"
        fscBothToOutExpectFail cfg outFile "%s -r:lib.dll -r:lib2.dll -r:lib3.dll -o:test.exe -g --nologo  --define:CHECK_ERRORS" cfg.fsc_flags ["test.fsx"]

        let diffs = fsdiff cfg outFile expectedFile
        match diffs with
        | "" -> ()
        | _ -> Assert.Fail (sprintf "'%s' and '%s' differ; %A" outFile expectedFile diffs)

    [<Test>]
    let ``fsi-reference`` () =

        let cfg = testConfig "core/fsi-reference"

        begin
            use testOkFile = fileguard cfg "test.ok"
            fsc cfg @"--target:library -o:ImplementationAssembly\ReferenceAssemblyExample.dll" ["ImplementationAssembly.fs"]
            fsc cfg @"--target:library -o:ReferenceAssembly\ReferenceAssemblyExample.dll" ["ReferenceAssembly.fs"]
            fsiStdin cfg "test.fsx" "" []
            testOkFile.CheckExists()
        end

    [<Test>]
    let ``fsi-reload`` () =
        let cfg = testConfig "core/fsi-reload"

        begin
            use testOkFile = fileguard cfg "test.ok"
            fsiStdin cfg "test1.ml"  " --langversion:5.0 --mlcompatibility --maxerrors:1" []
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
    let ``genericmeasures-FSC_NETFX_TEST_ROUNDTRIP_AS_DLL`` () = singleTestBuildAndRun "core/genericmeasures" FSC_NETFX_TEST_ROUNDTRIP_AS_DLL


    [<Test>]
    let hiding () =
        let cfg = testConfig "core/hiding"

        fsc cfg "%s -a --optimize --langversion:5.0 --mlcompatibility -o:lib.dll" cfg.fsc_flags ["lib.mli";"lib.ml";"libv.ml"]

        peverify cfg "lib.dll"

        fsc cfg "%s -a --optimize --langversion:5.0 --mlcompatibility -r:lib.dll -o:lib2.dll" cfg.fsc_flags ["lib2.mli";"lib2.ml";"lib3.ml"]

        peverify cfg "lib2.dll"

        fsc cfg "%s --optimize --langversion:5.0 --mlcompatibility -r:lib.dll -r:lib2.dll -o:client.exe" cfg.fsc_flags ["client.ml"]

        peverify cfg "client.exe"

    [<Test>]
    let ``innerpoly-FSC_NETFX_TEST_ROUNDTRIP_AS_DLL`` () = singleTestBuildAndRun "core/innerpoly"  FSC_NETFX_TEST_ROUNDTRIP_AS_DLL

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

    // Debug with
    //     ..\..\..\..\debug\net40\bin\fsi.exe --nologo < test.fsx >a.out 2>a.err
    // then
    ///    windiff output.stdout.bsl a.out
    let runPrintingTest flag baseFile =
       let diffFileOut = baseFile + ".stdout.txt"
       let expectedFileOut = baseFile + ".stdout.bsl"
       let diffFileErr = baseFile + ".stderr.txt"
       let expectedFileErr = baseFile + ".stderr.bsl"
       let cfg = testConfig "core/printing"

       if requireENCulture () then

        let ``fsi <a >b 2>c`` =
            // "%FSI%" %fsc_flags_errors_ok%  --nologo                                    <test.fsx >z.raw.output.test.default.txt 2>&1
            let ``exec <a >b 2>c`` (inFile, outFile, errFile) p =
                Command.exec cfg.Directory cfg.EnvironmentVariables { Output = OutputAndError(Overwrite(outFile), Overwrite(errFile)); Input = Some(RedirectInput(inFile)); } p
                >> checkResult
            Printf.ksprintf (fun flags (inFile, outFile, errFile) -> Commands.fsi (``exec <a >b 2>c`` (inFile, outFile, errFile)) cfg.FSI flags [])

        let fsc_flags_errors_ok = ""

        let rawFileOut = tryCreateTemporaryFileName ()
        let rawFileErr = tryCreateTemporaryFileName ()
        ``fsi <a >b 2>c`` "%s --nologo --preferreduilang:en-US %s" fsc_flags_errors_ok flag ("test.fsx", rawFileOut, rawFileErr)

        let removeCDandHelp fromFile toFile =
            File.ReadAllLines fromFile
            |> Array.filter (fun s -> not (s.Contains(cfg.Directory)))
            |> Array.filter (fun s -> not (s.Contains("--help' for options")))
            |> Array.filter (fun s -> not (s.Contains("[Loading")))
            |> Array.filter (fun s -> not (s.Contains("Binding session")))
            |> (fun lines -> File.WriteAllLines(getfullpath cfg toFile, lines))

        removeCDandHelp rawFileOut diffFileOut
        removeCDandHelp rawFileErr diffFileErr

        let withDefault defaultFile toFile =
            if not (fileExists cfg toFile) then copy cfg defaultFile toFile

        expectedFileOut |> withDefault diffFileOut
        expectedFileErr |> withDefault diffFileErr

        match fsdiff cfg diffFileOut expectedFileOut with
        | "" -> ()
        | diffs -> Assert.Fail (sprintf "'%s' and '%s' differ; %A" diffFileOut expectedFileOut diffs)

        match fsdiff cfg diffFileErr expectedFileErr with
        | "" -> ()
        | diffs -> Assert.Fail (sprintf "'%s' and '%s' differ; %A" diffFileErr expectedFileErr diffs)

    [<Test>]
    let ``printing`` () =
         runPrintingTest "--multiemit- --debug+" "output"

    // F# 5.0 changed some things printing output
    [<Test>]
    let ``printing-langversion47`` () =
         runPrintingTest "--langversion:4.7" "output.47"

    // Output should not change with optimization off
    [<Test>]
    let ``printing-optimizeoff`` () =
         runPrintingTest "--multiemit- --debug+ --optimize-" "output"

    // Legacy one-dynamic-assembly emit is the default for .NET Framework, which these tests are using
    // Turning that off enables multi-assembly-emit.  The printing test is useful for testing multi-assembly-emit
    // as it feeds in many incremental fragments into stdin of the FSI process.
    [<Test>]
    let ``printing-multiemit`` () =
         runPrintingTest "--multiemit+ --debug+" "output.multiemit"

    // Multi-assembly-emit establishes some slightly different rules regarding internals, and this
    // needs to be tested with optimizations off.  The output should not change.
    [<Test>]
    let ``printing-multiemit-optimizeoff`` () =
         runPrintingTest "--multiemit+ --debug+ --optimize-" "output.multiemit"

    [<Test>]
    let ``printing-width-1000`` () =
         runPrintingTest "--use:preludePrintSize1000.fsx" "output.1000"

    [<Test>]
    let ``printing-width-200`` () =  
         runPrintingTest "--use:preludePrintSize200.fsx" "output.200"

    [<Test>]
    let ``printing-off`` () =
         runPrintingTest "--use:preludeShowDeclarationValuesFalse.fsx" "output.off"

    [<Test>]
    let ``printing-quiet`` () =
         runPrintingTest "--quiet" "output.quiet"

    type SigningType =
        | DelaySigned
        | PublicSigned
        | FullSigned
        | NotSigned

    let signedtest(programId:string, args:string, expectedSigning:SigningType) =

        let cfg = testConfig "core/signedtests"
        let newFlags = cfg.fsc_flags + " " + args

        let exefile = programId + ".exe"
        fsc cfg "%s -o:%s" newFlags exefile ["test.fs"]

        let assemblyPath = Path.Combine(cfg.Directory, exefile)
        let assemblyName = AssemblyName.GetAssemblyName(assemblyPath)
        let publicKeyToken = assemblyName.GetPublicKeyToken()
        let isPublicKeyTokenPresent = not (Array.isEmpty publicKeyToken)
        use exeStream = new FileStream(assemblyPath, FileMode.Open)
        let peHeader = PEHeaders(exeStream)
        let isSigned = peHeader.CorHeader.Flags.HasFlag(CorFlags.StrongNameSigned)
        let actualSigning =
            match isSigned, isPublicKeyTokenPresent with
            | true, true-> SigningType.PublicSigned
            | true, false -> failwith "unreachable"
            | false, true -> SigningType.DelaySigned
            | false, false -> SigningType.NotSigned

        exeStream.Dispose()
        if isSigned then
            // We think the assembly is signed let's run sn -vr to verify the hashes
            sn cfg " %s    -q -vf" "" assemblyPath

        Assert.AreEqual(expectedSigning, actualSigning)

    [<Test; Category("signedtest")>]
    let ``signedtest-1`` () = signedtest("test-unsigned", "", SigningType.NotSigned)

    [<Test; Category("signedtest")>]
    let ``signedtest-2`` () = signedtest("test-sha1-full-cl", "--keyfile:sha1full.snk", SigningType.PublicSigned)

    [<Test; Category("signedtest")>]
    let ``signedtest-3`` () = signedtest("test-sha256-full-cl", "--keyfile:sha256full.snk", SigningType.PublicSigned)

    [<Test; Category("signedtest")>]
    let ``signedtest-4`` () = signedtest("test-sha512-full-cl", "--keyfile:sha512full.snk", SigningType.PublicSigned)

    [<Test; Category("signedtest")>]
    let ``signedtest-5`` () = signedtest("test-sha1024-full-cl", "--keyfile:sha1024full.snk", SigningType.PublicSigned)

    [<Test; Category("signedtest")>]
    let ``signedtest-6`` () = signedtest("test-sha1-delay-cl", "--keyfile:sha1delay.snk --delaysign", SigningType.DelaySigned)

    [<Test; Category("signedtest")>]
    let ``signedtest-7`` () = signedtest("test-sha256-delay-cl", "--keyfile:sha256delay.snk --delaysign", SigningType.DelaySigned)

    [<Test; Category("signedtest")>]
    let ``signedtest-8`` () = signedtest("test-sha512-delay-cl", "--keyfile:sha512delay.snk --delaysign", SigningType.DelaySigned)

    [<Test; Category("signedtest")>]
    let ``signedtest-9`` () = signedtest("test-sha1024-delay-cl", "--keyfile:sha1024delay.snk --delaysign", SigningType.DelaySigned)

    // Test SHA1 key full signed  Attributes
    [<Test; Category("signedtest")>]
    let ``signedtest-10`` () = signedtest("test-sha1-full-attributes", "--define:SHA1", SigningType.PublicSigned)

    // Test SHA1 key delayl signed  Attributes
    [<Test; Category("signedtest")>]
    let ``signedtest-11`` () = signedtest("test-sha1-delay-attributes", "--keyfile:sha1delay.snk --define:SHA1 --define:DELAY", SigningType.DelaySigned)

    [<Test; Category("signedtest")>]
    let ``signedtest-12`` () = signedtest("test-sha256-full-attributes", "--define:SHA256", SigningType.PublicSigned)

    // Test SHA 256 bit key delay signed  Attributes
    [<Test; Category("signedtest")>]
    let ``signedtest-13`` () = signedtest("test-sha256-delay-attributes", "--define:SHA256 --define:DELAY", SigningType.DelaySigned)

    // Test SHA 512 bit key fully signed  Attributes
    [<Test; Category("signedtest")>]
    let ``signedtest-14`` () = signedtest("test-sha512-full-attributes", "--define:SHA512", SigningType.PublicSigned)

    // Test SHA 512 bit key delay signed Attributes
    [<Test; Category("signedtest")>]
    let ``signedtest-15`` () = signedtest("test-sha512-delay-attributes", "--define:SHA512 --define:DELAY", SigningType.DelaySigned)

    // Test SHA 1024 bit key fully signed  Attributes
    [<Test; Category("signedtest")>]
    let ``signedtest-16`` () = signedtest("test-sha1024-full-attributes", "--define:SHA1024", SigningType.PublicSigned)

    // Test fully signed with pdb generation
    [<Test; Category("signedtest")>]
    let ``signedtest-17`` () = signedtest("test-sha1-full-cl", "-g --keyfile:sha1full.snk", SigningType.PublicSigned)

#endif

#if !NETCOREAPP
    [<Test>]
    let quotes () =
        let cfg = testConfig "core/quotes"


        csc cfg """/nologo  /target:library /out:cslib.dll""" ["cslib.cs"]

        fsc cfg "%s  -o:test.exe -r cslib.dll -g" cfg.fsc_flags ["test.fsx"]

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

        fsc cfg "%s -a --langversion:5.0 --mlcompatibility -o:crlf.dll -g" cfg.fsc_flags ["crlf.ml"]

        fsc cfg "%s --langversion:5.0 --mlcompatibility -o:toplet.exe -g" cfg.fsc_flags ["toplet.ml"]

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
        fsc cfg "%s --version:1.2.3 --keyfile:key.snk --langversion:5.0 --mlcompatibility -a --optimize -o:library.dll" cfg.fsc_flags ["library.fsi"; "library.fs"]

        peverify cfg "library.dll"

        // Compiling C# Library
        csc cfg "/target:library /keyfile:key.snk /out:librarycs.dll" ["librarycs.cs"]

        peverify cfg "librarycs.dll"

        // Compiling F# main referencing C# and F# libraries
        fsc cfg "%s --version:1.2.3 --keyfile:key.snk --optimize --langversion:5.0 --mlcompatibility -r:library.dll -r:librarycs.dll -o:main.exe" cfg.fsc_flags ["main.fs"]

        peverify cfg "main.exe"

        // Run F# main. Quick test!
        exec cfg ("." ++ "main.exe") ""


    // Repro for https://github.com/dotnet/fsharp/issues/1298
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

    // Repro for https://github.com/dotnet/fsharp/issues/2679
    [<Test>]
    let ``add files with same name from different folders`` () =
        let cfg = testConfig "core/samename"

        log "== Compiling F# Code with files with same name in different folders"
        fsc cfg "%s -o:test.exe" cfg.fsc_flags ["folder1/a.fs"; "folder1/b.fs"; "folder2/a.fs"; "folder2/b.fs"]

        peverify cfg "test.exe"

        exec cfg ("." ++ "test.exe") ""

    [<Test>]
    let ``add files with same name from different folders including signature files`` () =
        let cfg = testConfig "core/samename"

        log "== Compiling F# Code with files with same name in different folders including signature files"
        fsc cfg "%s -o:test.exe" cfg.fsc_flags ["folder1/a.fsi"; "folder1/a.fs"; "folder1/b.fsi"; "folder1/b.fs"; "folder2/a.fsi"; "folder2/a.fs"; "folder2/b.fsi"; "folder2/b.fs"]

        peverify cfg "test.exe"

        exec cfg ("." ++ "test.exe") ""

    [<Test>]
    let ``add files with same name from different folders including signature files that are not synced`` () =
        let cfg = testConfig "core/samename"

        log "== Compiling F# Code with files with same name in different folders including signature files"
        fsc cfg "%s -o:test.exe" cfg.fsc_flags ["folder1/a.fsi"; "folder1/a.fs"; "folder1/b.fs"; "folder2/a.fsi"; "folder2/a.fs"; "folder2/b.fsi"; "folder2/b.fs"]

        peverify cfg "test.exe"

        exec cfg ("." ++ "test.exe") ""

    [<Test>]
    let ``libtest-FSI_NETFX_STDIN`` () = singleTestBuildAndRun "core/libtest" FSI_NETFX_STDIN

    [<Test>]
    let ``libtest-unoptimized codegen`` () = singleTestBuildAndRun "core/libtest" FSC_DEBUG

    [<Test>]
    let ``libtest-FSC_NETFX_TEST_ROUNDTRIP_AS_DLL`` () = singleTestBuildAndRun "core/libtest" FSC_NETFX_TEST_ROUNDTRIP_AS_DLL

    [<Test>]
    let ``libtest-langversion-preview-checknulls`` () =
        let cfg = testConfig "core/libtest"

        use testOkFile = fileguard cfg "test.ok"

        fsc cfg "%s -o:test-checknulls.exe -g --langversion:preview --checknulls" cfg.fsc_flags ["test.fsx"]

        exec cfg ("." ++ "test-checknulls.exe") ""

        testOkFile.CheckExists()

 
    [<Test>]
    let ``libtest-langversion-46`` () =
        let cfg = testConfig "core/libtest"

        use testOkFile = fileguard cfg "test.ok"

        fsc cfg "%s -o:test-langversion-46.exe -g --langversion:4.6" cfg.fsc_flags ["test.fsx"]

        exec cfg ("." ++ "test-langversion-46.exe") ""

        testOkFile.CheckExists()

    [<Test>]
    let ``no-warn-2003-tests`` () =
        // see https://github.com/dotnet/fsharp/issues/3139
        let cfg = testConfig "core/versionAttributes"
        let stdoutPath = "out.stdout.txt" |> getfullpath cfg
        let stderrPath = "out.stderr.txt" |> getfullpath cfg
        let stderrBaseline = "out.stderr.bsl" |> getfullpath cfg
        let stdoutBaseline = "out.stdout.bsl" |> getfullpath cfg
        let echo text =
            Commands.echoAppendToFile cfg.Directory text stdoutPath
            Commands.echoAppendToFile cfg.Directory text stderrPath

        File.WriteAllText(stdoutPath, "")
        File.WriteAllText(stderrPath, "")

        echo "Test 1================================================="
        fscAppend cfg stdoutPath stderrPath "--nologo" ["NoWarn2003.fs"]

        echo "Test 2================================================="
        fscAppend cfg stdoutPath stderrPath "--nologo" ["NoWarn2003_2.fs"]

        echo "Test 3================================================="
        fscAppend cfg stdoutPath stderrPath "--nologo" ["Warn2003_1.fs"]

        echo "Test 4================================================="
        fscAppend cfg stdoutPath stderrPath "--nologo" ["Warn2003_2.fs"]

        echo "Test 5================================================="
        fscAppend cfg stdoutPath stderrPath "--nologo" ["Warn2003_3.fs"]

        echo "Test 6================================================="
        fscAppend cfg stdoutPath stderrPath "--nologo --nowarn:2003" ["Warn2003_1.fs"]

        echo "Test 7================================================="
        fscAppend cfg stdoutPath stderrPath "--nologo --nowarn:2003" ["Warn2003_2.fs"]

        echo "Test 8================================================="
        fscAppend cfg stdoutPath stderrPath "--nologo --nowarn:2003" ["Warn2003_3.fs"]

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

        fscAppend cfg stdoutPath stderrPath "--nologo -r \"%s\"" cfg.FSharpCompilerInteractiveSettings ["usesfsi.fsx"]

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



#if !NETCOREAPP
    [<Test>]
    let ``measures-FSC_NETFX_TEST_ROUNDTRIP_AS_DLL`` () = singleTestBuildAndRun "core/measures" FSC_NETFX_TEST_ROUNDTRIP_AS_DLL

    [<Test>]
    let ``members-basics-FSC_NETFX_TEST_ROUNDTRIP_AS_DLL`` () = singleTestBuildAndRun "core/members/basics" FSC_NETFX_TEST_ROUNDTRIP_AS_DLL

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



#if !NETCOREAPP
    [<Test>]
    let refnormalization () =
        let cfg = testConfig "core/refnormalization"

        // Prepare by building multiple versions of the test assemblies
        fsc cfg @"%s --target:library -o:version1\DependentAssembly.dll -g --version:1.0.0.0 --keyfile:keyfile.snk" cfg.fsc_flags [@"DependentAssembly.fs"]
        fsc cfg @"%s --target:library -o:version1\AscendentAssembly.dll -g --version:1.0.0.0 --keyfile:keyfile.snk -r:version1\DependentAssembly.dll" cfg.fsc_flags [@"AscendentAssembly.fs"]

        fsc cfg @"%s --target:library -o:version2\DependentAssembly.dll -g --version:2.0.0.0" cfg.fsc_flags [@"DependentAssembly.fs"]
        fsc cfg @"%s --target:library -o:version2\AscendentAssembly.dll -g --version:2.0.0.0 -r:version2\DependentAssembly.dll" cfg.fsc_flags [@"AscendentAssembly.fs"]

        //TestCase1
        // Build a program that references v2 of ascendent and v1 of dependent.
        // Note that, even though ascendent v2 references dependent v2, the reference is marked as v1.
        use TestOk = fileguard cfg "test.ok"
        fsc cfg @"%s -o:test1.exe -r:version1\DependentAssembly.dll -r:version2\AscendentAssembly.dll --optimize- -g" cfg.fsc_flags ["test.fs"]
        exec cfg ("." ++ "test1.exe") "DependentAssembly-1.0.0.0 AscendentAssembly-2.0.0.0"
        TestOk.CheckExists()

        //TestCase2
        // Build a program that references v1 of ascendent and v2 of dependent.
        // Note that, even though ascendent v1 references dependent v1, the reference is marked as v2 which was passed in.
        use TestOk = fileguard cfg "test.ok"
        fsc cfg @"%s -o:test2.exe -r:version2\DependentAssembly.dll -r:version1\AscendentAssembly.dll --optimize- -g" cfg.fsc_flags ["test.fs"]
        exec cfg ("." ++ "test2.exe") "DependentAssembly-2.0.0.0 AscendentAssembly-1.0.0.0"
        TestOk.CheckExists()

        //TestCase3
        // Build a program that references v1 of ascendent and v1 and v2 of dependent.
        // Verifies that compiler uses first version of a duplicate assembly passed on command line.
        use TestOk = fileguard cfg "test.ok"
        fsc cfg @"%s -o:test3.exe -r:version1\DependentAssembly.dll -r:version2\DependentAssembly.dll -r:version1\AscendentAssembly.dll --optimize- -g" cfg.fsc_flags ["test.fs"]
        exec cfg ("." ++ "test3.exe") "DependentAssembly-1.0.0.0 AscendentAssembly-1.0.0.0"
        TestOk.CheckExists()


    [<Test>]
    let testResources () =
        let cfg = testConfig "core/resources"

        fsc cfg "%s --langversion:5.0 --mlcompatibility --resource:Resources.resources -o:test-embed.exe -g" cfg.fsc_flags ["test.fs"]

        peverify cfg "test-embed.exe"

        fsc cfg "%s --langversion:5.0 --mlcompatibility --linkresource:Resources.resources -o:test-link.exe -g" cfg.fsc_flags ["test.fs"]

        peverify cfg "test-link.exe"

        fsc cfg "%s --langversion:5.0 --mlcompatibility --resource:Resources.resources,ResourceName.resources -o:test-embed-named.exe -g" cfg.fsc_flags ["test.fs"]

        peverify cfg "test-embed-named.exe"

        fsc cfg "%s --langversion:5.0 --mlcompatibility --linkresource:Resources.resources,ResourceName.resources -o:test-link-named.exe -g" cfg.fsc_flags ["test.fs"]

        peverify cfg "test-link-named.exe"

        exec cfg ("." ++ "test-embed.exe") ""

        exec cfg ("." ++ "test-link.exe") ""

        exec cfg ("." ++ "test-link-named.exe") "ResourceName"

        exec cfg ("." ++ "test-embed-named.exe") "ResourceName"

    [<Test>]
    let topinit () =
        let cfg = testConfig "core/topinit"

        fsc cfg "%s --realsig- --optimize -o both69514.exe -g" cfg.fsc_flags ["lib69514.fs"; "app69514.fs"]

        peverify cfg "both69514.exe"

        fsc cfg "%s --realsig- --optimize- -o both69514-noopt.exe -g" cfg.fsc_flags ["lib69514.fs"; "app69514.fs"]

        peverify cfg "both69514-noopt.exe"

        fsc cfg "%s --realsig- --optimize -a -g" cfg.fsc_flags ["lib69514.fs"]

        peverify cfg "lib69514.dll"

        fsc cfg "%s --realsig- --optimize -r:lib69514.dll -g" cfg.fsc_flags ["app69514.fs"]

        peverify cfg "app69514.exe"

        fsc cfg "%s --realsig- --optimize- -o:lib69514-noopt.dll -a -g" cfg.fsc_flags ["lib69514.fs"]

        peverify cfg "lib69514-noopt.dll"

        fsc cfg "%s --realsig- --optimize- -r:lib69514-noopt.dll -o:app69514-noopt.exe -g" cfg.fsc_flags ["app69514.fs"]

        peverify cfg "app69514-noopt.exe"

        fsc cfg "%s --realsig- --optimize- -o:lib69514-noopt-withsig.dll -a -g" cfg.fsc_flags ["lib69514.fsi"; "lib69514.fs"]

        peverify cfg "lib69514-noopt-withsig.dll"

        fsc cfg "%s --realsig- --optimize- -r:lib69514-noopt-withsig.dll -o:app69514-noopt-withsig.exe -g" cfg.fsc_flags ["app69514.fs"]

        peverify cfg "app69514-noopt-withsig.exe"

        fsc cfg "%s --realsig- -o:lib69514-withsig.dll -a -g" cfg.fsc_flags ["lib69514.fsi"; "lib69514.fs"]

        peverify cfg "lib69514-withsig.dll"

        fsc cfg "%s -r:lib69514-withsig.dll --realsig- -o:app69514-withsig.exe -g" cfg.fsc_flags ["app69514.fs"]

        peverify cfg "app69514-withsig.exe"

        fsc cfg "%s -o:lib.dll --realsig- -a --langversion:5.0 --mlcompatibility -g" cfg.fsc_flags ["lib.ml"]

        peverify cfg "lib.dll"

        csc cfg """/nologo /r:"%s" /r:lib.dll /out:test.exe """ cfg.FSCOREDLLPATH ["test.cs"]

        fsc cfg "%s --optimize -o:lib--optimize.dll --realsig- -a  --langversion:5.0 --mlcompatibility -g" cfg.fsc_flags ["lib.ml"]

        peverify cfg "lib--optimize.dll"

        csc cfg """/nologo /r:"%s" /r:lib--optimize.dll /out:test--optimize.exe""" cfg.FSCOREDLLPATH ["test.cs"]

        let dicases = ["flag_deterministic_init1.fs"; "lib_deterministic_init1.fs"; "flag_deterministic_init2.fs"; "lib_deterministic_init2.fs"; "flag_deterministic_init3.fs"; "lib_deterministic_init3.fs"; "flag_deterministic_init4.fs"; "lib_deterministic_init4.fs"; "flag_deterministic_init5.fs"; "lib_deterministic_init5.fs"; "flag_deterministic_init6.fs"; "lib_deterministic_init6.fs"; "flag_deterministic_init7.fs"; "lib_deterministic_init7.fs"; "flag_deterministic_init8.fs"; "lib_deterministic_init8.fs"; "flag_deterministic_init9.fs"; "lib_deterministic_init9.fs"; "flag_deterministic_init10.fs"; "lib_deterministic_init10.fs"; "flag_deterministic_init11.fs"; "lib_deterministic_init11.fs"; "flag_deterministic_init12.fs"; "lib_deterministic_init12.fs"; "flag_deterministic_init13.fs"; "lib_deterministic_init13.fs"; "flag_deterministic_init14.fs"; "lib_deterministic_init14.fs"; "flag_deterministic_init15.fs"; "lib_deterministic_init15.fs"; "flag_deterministic_init16.fs"; "lib_deterministic_init16.fs"; "flag_deterministic_init17.fs"; "lib_deterministic_init17.fs"; "flag_deterministic_init18.fs"; "lib_deterministic_init18.fs"; "flag_deterministic_init19.fs"; "lib_deterministic_init19.fs"; "flag_deterministic_init20.fs"; "lib_deterministic_init20.fs"; "flag_deterministic_init21.fs"; "lib_deterministic_init21.fs"; "flag_deterministic_init22.fs"; "lib_deterministic_init22.fs"; "flag_deterministic_init23.fs"; "lib_deterministic_init23.fs"; "flag_deterministic_init24.fs"; "lib_deterministic_init24.fs"; "flag_deterministic_init25.fs"; "lib_deterministic_init25.fs"; "flag_deterministic_init26.fs"; "lib_deterministic_init26.fs"; "flag_deterministic_init27.fs"; "lib_deterministic_init27.fs"; "flag_deterministic_init28.fs"; "lib_deterministic_init28.fs"; "flag_deterministic_init29.fs"; "lib_deterministic_init29.fs"; "flag_deterministic_init30.fs"; "lib_deterministic_init30.fs"; "flag_deterministic_init31.fs"; "lib_deterministic_init31.fs"; "flag_deterministic_init32.fs"; "lib_deterministic_init32.fs"; "flag_deterministic_init33.fs"; "lib_deterministic_init33.fs"; "flag_deterministic_init34.fs"; "lib_deterministic_init34.fs"; "flag_deterministic_init35.fs"; "lib_deterministic_init35.fs"; "flag_deterministic_init36.fs"; "lib_deterministic_init36.fs"; "flag_deterministic_init37.fs"; "lib_deterministic_init37.fs"; "flag_deterministic_init38.fs"; "lib_deterministic_init38.fs"; "flag_deterministic_init39.fs"; "lib_deterministic_init39.fs"; "flag_deterministic_init40.fs"; "lib_deterministic_init40.fs"; "flag_deterministic_init41.fs"; "lib_deterministic_init41.fs"; "flag_deterministic_init42.fs"; "lib_deterministic_init42.fs"; "flag_deterministic_init43.fs"; "lib_deterministic_init43.fs"; "flag_deterministic_init44.fs"; "lib_deterministic_init44.fs"; "flag_deterministic_init45.fs"; "lib_deterministic_init45.fs"; "flag_deterministic_init46.fs"; "lib_deterministic_init46.fs"; "flag_deterministic_init47.fs"; "lib_deterministic_init47.fs"; "flag_deterministic_init48.fs"; "lib_deterministic_init48.fs"; "flag_deterministic_init49.fs"; "lib_deterministic_init49.fs"; "flag_deterministic_init50.fs"; "lib_deterministic_init50.fs"; "flag_deterministic_init51.fs"; "lib_deterministic_init51.fs"; "flag_deterministic_init52.fs"; "lib_deterministic_init52.fs"; "flag_deterministic_init53.fs"; "lib_deterministic_init53.fs"; "flag_deterministic_init54.fs"; "lib_deterministic_init54.fs"; "flag_deterministic_init55.fs"; "lib_deterministic_init55.fs"; "flag_deterministic_init56.fs"; "lib_deterministic_init56.fs"; "flag_deterministic_init57.fs"; "lib_deterministic_init57.fs"; "flag_deterministic_init58.fs"; "lib_deterministic_init58.fs"; "flag_deterministic_init59.fs"; "lib_deterministic_init59.fs"; "flag_deterministic_init60.fs"; "lib_deterministic_init60.fs"; "flag_deterministic_init61.fs"; "lib_deterministic_init61.fs"; "flag_deterministic_init62.fs"; "lib_deterministic_init62.fs"; "flag_deterministic_init63.fs"; "lib_deterministic_init63.fs"; "flag_deterministic_init64.fs"; "lib_deterministic_init64.fs"; "flag_deterministic_init65.fs"; "lib_deterministic_init65.fs"; "flag_deterministic_init66.fs"; "lib_deterministic_init66.fs"; "flag_deterministic_init67.fs"; "lib_deterministic_init67.fs"; "flag_deterministic_init68.fs"; "lib_deterministic_init68.fs"; "flag_deterministic_init69.fs"; "lib_deterministic_init69.fs"; "flag_deterministic_init70.fs"; "lib_deterministic_init70.fs"; "flag_deterministic_init71.fs"; "lib_deterministic_init71.fs"; "flag_deterministic_init72.fs"; "lib_deterministic_init72.fs"; "flag_deterministic_init73.fs"; "lib_deterministic_init73.fs"; "flag_deterministic_init74.fs"; "lib_deterministic_init74.fs"; "flag_deterministic_init75.fs"; "lib_deterministic_init75.fs"; "flag_deterministic_init76.fs"; "lib_deterministic_init76.fs"; "flag_deterministic_init77.fs"; "lib_deterministic_init77.fs"; "flag_deterministic_init78.fs"; "lib_deterministic_init78.fs"; "flag_deterministic_init79.fs"; "lib_deterministic_init79.fs"; "flag_deterministic_init80.fs"; "lib_deterministic_init80.fs"; "flag_deterministic_init81.fs"; "lib_deterministic_init81.fs"; "flag_deterministic_init82.fs"; "lib_deterministic_init82.fs"; "flag_deterministic_init83.fs"; "lib_deterministic_init83.fs"; "flag_deterministic_init84.fs"; "lib_deterministic_init84.fs"; "flag_deterministic_init85.fs"; "lib_deterministic_init85.fs"]

        fsc cfg "%s --optimize- -o test_deterministic_init.exe --realsig- " cfg.fsc_flags (dicases @ ["test_deterministic_init.fs"])

        peverify cfg "test_deterministic_init.exe"

        fsc cfg "%s --optimize -o test_deterministic_init--optimize.exe --realsig- " cfg.fsc_flags (dicases @ ["test_deterministic_init.fs"])

        peverify cfg "test_deterministic_init--optimize.exe"

        fsc cfg "%s --optimize- -a -o test_deterministic_init_lib.dll --realsig- " cfg.fsc_flags dicases

        peverify cfg "test_deterministic_init_lib.dll"

        fsc cfg "%s --optimize- -r test_deterministic_init_lib.dll -o test_deterministic_init_exe.exe --realsig- " cfg.fsc_flags ["test_deterministic_init.fs"]

        peverify cfg "test_deterministic_init_exe.exe"

        fsc cfg "%s --realsig- --optimize -a -o test_deterministic_init_lib--optimize.dll --realsig- " cfg.fsc_flags dicases

        peverify cfg "test_deterministic_init_lib--optimize.dll"

        fsc cfg "%s --realsig- --optimize -r test_deterministic_init_lib--optimize.dll -o test_deterministic_init_exe--optimize.exe --realsig- " cfg.fsc_flags ["test_deterministic_init.fs"]

        peverify cfg "test_deterministic_init_exe--optimize.exe"

        let static_init_cases = [ "test0.fs"; "test1.fs"; "test2.fs"; "test3.fs"; "test4.fs"; "test5.fs"; "test6.fs" ]

        fsc cfg "%s --optimize- -o test_static_init.exe --realsig- " cfg.fsc_flags (static_init_cases @ ["static-main.fs"])

        peverify cfg "test_static_init.exe"

        fsc cfg "%s --optimize -o test_static_init--optimize.exe --realsig- " cfg.fsc_flags (static_init_cases @ [ "static-main.fs" ])

        peverify cfg "test_static_init--optimize.exe"

        fsc cfg "%s --optimize- -a -o test_static_init_lib.dll --realsig- " cfg.fsc_flags static_init_cases

        peverify cfg "test_static_init_lib.dll"

        fsc cfg "%s --optimize- -r test_static_init_lib.dll -o test_static_init_exe.exe --realsig- " cfg.fsc_flags ["static-main.fs"]

        peverify cfg "test_static_init_exe.exe"

        fsc cfg "%s --optimize -a -o test_static_init_lib--optimize.dll --realsig- " cfg.fsc_flags static_init_cases

        peverify cfg "test_static_init_lib--optimize.dll"

        fsc cfg "%s --realsig- --optimize -r test_static_init_lib--optimize.dll --realsig- -o test_static_init_exe--optimize.exe" cfg.fsc_flags ["static-main.fs"]

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

        peverifyWithArgs cfg "/nologo" (cfg.FSharpBuild)

       // peverifyWithArgs cfg "/nologo /MD" (getDirectoryName(cfg.FSC) ++ "FSharp.Compiler.dll")

        peverifyWithArgs cfg "/nologo" (cfg.FSI)

        peverifyWithArgs cfg "/nologo" (cfg.FSharpCompilerInteractiveSettings)

        fsc cfg "%s -o:xmlverify.exe -g" cfg.fsc_flags ["xmlverify.fs"]

        peverifyWithArgs cfg "/nologo" "xmlverify.exe"
        
        
    [<Test>]
    let ``property setter in method or constructor`` () =
        let cfg = testConfig "core/members/set-only-property"
        csc cfg @"%s /target:library /out:cs.dll" cfg.csc_flags ["cs.cs"]
        vbc cfg @"%s /target:library /out:vb.dll" cfg.vbc_flags ["vb.vb"]
        fsc cfg @"%s /target:library /out:fs.dll" cfg.fsc_flags ["fs.fs"]
        singleNegTest cfg "calls"

#endif

[<NonParallelizable>]
module VersionTests =
    [<Test>]
    let ``member-selfidentifier-version4_6``() = singleTestBuildAndRunVersion "core/members/self-identifier/version46" (FSC_BUILDONLY true) "4.6"

    [<Test>]
    let ``member-selfidentifier-version4_7``() = singleTestBuildAndRunVersion "core/members/self-identifier/version47" (FSC_BUILDONLY true) "4.7"

    [<Test>]
    let ``indent-version4_7``() = singleTestBuildAndRunVersion "core/indent/version47" (FSC_BUILDONLY true) "4.7"

    [<Test>]
    let ``nameof-version4_6``() = singleTestBuildAndRunVersion "core/nameof/version46" (FSC_BUILDONLY true) "4.6"

    [<Test>]
    let ``nameof-versionpreview``() = singleTestBuildAndRunVersion "core/nameof/preview" (FSC_BUILDONLY true) "preview"

    [<Test>]
    let ``nameof-execute``() = singleTestBuildAndRunVersion "core/nameof/preview" FSC_OPTIMIZED "preview"

    [<Test>]
    let ``nameof-fsi``() = singleTestBuildAndRunVersion "core/nameof/preview" FSI "preview"

#if !NETCOREAPP
[<NonParallelizable; SetUICulture("en-US"); SetCulture("en-US")>]
module ToolsTests =

    // This test is disabled in coreclr builds dependent on fixing : https://github.com/dotnet/fsharp/issues/2600
    [<Test>]
    let bundle () =
        let cfg = 
            testConfig "tools/bundle" 

        fsc cfg "%s --progress --langversion:5.0 --mlcompatibility --standalone -o:test-one-fsharp-module.exe -g" cfg.fsc_flags ["test-one-fsharp-module.fs"]

        peverify cfg "test-one-fsharp-module.exe"

        fsc cfg "%s -a --langversion:5.0 --mlcompatibility -o:test_two_fsharp_modules_module_1.dll -g" cfg.fsc_flags ["test_two_fsharp_modules_module_1.fs"]

        peverify cfg "test_two_fsharp_modules_module_1.dll"

        fsc cfg "%s --langversion:5.0 --mlcompatibility --standalone -r:test_two_fsharp_modules_module_1.dll -o:test_two_fsharp_modules_module_2.exe -g" cfg.fsc_flags ["test_two_fsharp_modules_module_2.fs"]

        peverify cfg "test_two_fsharp_modules_module_2.exe"

        fsc cfg "%s -a --langversion:5.0 --mlcompatibility --standalone -r:test_two_fsharp_modules_module_1.dll -o:test_two_fsharp_modules_module_2_as_dll.dll -g" cfg.fsc_flags ["test_two_fsharp_modules_module_2.fs"]

        peverify cfg "test_two_fsharp_modules_module_2_as_dll.dll"
#endif

    [<Test>]
    let ``eval-FSC_OPTIMIZED`` () = singleTestBuildAndRun "tools/eval" FSC_OPTIMIZED
    [<Test>]
    let ``eval-FSI`` () = singleTestBuildAndRun "tools/eval" FSI

[<NonParallelizable>]
module RegressionTests =

    [<Test>]
    let ``literal-value-bug-2-FSC_OPTIMIZED`` () = singleTestBuildAndRun "regression/literal-value-bug-2" FSC_OPTIMIZED

    [<Test>]
    let ``literal-value-bug-2-FSI`` () = singleTestBuildAndRun "regression/literal-value-bug-2" FSI

    [<Test>]
    let ``OverloadResolution-bug-FSC_OPTIMIZED`` () = singleTestBuildAndRun "regression/OverloadResolution-bug" FSC_OPTIMIZED

    [<Test>]
    let ``OverloadResolution-bug-FSI`` () = singleTestBuildAndRun "regression/OverloadResolution-bug" FSI

    [<Test>]
    let ``struct-tuple-bug-1-FSC_OPTIMIZED`` () = singleTestBuildAndRun "regression/struct-tuple-bug-1" FSC_OPTIMIZED

    [<Test >]
    let ``tuple-bug-1-FSC_OPTIMIZED`` () = singleTestBuildAndRunVersion "regression/tuple-bug-1" FSC_OPTIMIZED "supports-ml"

    [<Test >]
    let ``12383-FSC_OPTIMIZED`` () = singleTestBuildAndRun "regression/12383" FSC_OPTIMIZED

    [<Test>]
    let ``13219-bug-FSI`` () = singleTestBuildAndRun "regression/13219" FSI

    [<Test >]
    let ``4715-optimized`` () =
        let cfg = testConfig "regression/4715"
        fsc cfg "%s -o:test.exe --optimize+" cfg.fsc_flags ["date.fs"; "env.fs"; "main.fs"]

    [<Test>]
    let ``multi-package-type-provider-test-FSI`` () = singleTestBuildAndRun "regression/13710" FSI

#if NETCOREAPP
    [<Test >]
    let ``Large inputs 12322 fsc.dll 64-bit fsc.dll .NET SDK generating optimized code`` () =
        let cfg = testConfig "regression/12322"
        let cfg = { cfg with fsc_flags = cfg.fsc_flags + " --debug:portable --define:PORTABLE_PDB" }
        singleTestBuildAndRunAux cfg (FSC_BUILDONLY true)

    [<Test >]
    let ``Large inputs 12322 fsc.dll 64-bit .NET SDK generating debug code`` () =
        let cfg = testConfig "regression/12322"
        let cfg = { cfg with fsc_flags = cfg.fsc_flags + " --debug:portable --define:PORTABLE_PDB" }
        singleTestBuildAndRunAux cfg (FSC_BUILDONLY false)

#else
    [<Test >]
    let ``Large inputs 12322 fsc.exe 32-bit .NET Framework generating optimized code, portable PDB`` () =
        let cfg = testConfig "regression/12322"
        let cfg = { cfg with fsc_flags = cfg.fsc_flags + " --debug:portable --define:PORTABLE_PDB" }
        singleTestBuildAndRunAux cfg (FSC_BUILDONLY true)

    [<Test >]
    let ``Large inputs 12322 fsc.exe 32-bit .NET Framework generating optimized code, full PDB`` () =
        let cfg = testConfig "regression/12322"
        let cfg = { cfg with fsc_flags = cfg.fsc_flags + " --debug:full" }
        singleTestBuildAndRunAux cfg (FSC_BUILDONLY true)

    [<Test >]
    let ``Large inputs 12322 fsc.exe 32-bit .NET Framework generating debug code portable PDB`` () =
        let cfg = testConfig "regression/12322"
        let cfg = { cfg with fsc_flags = cfg.fsc_flags + " --debug:portable --define:PORTABLE_PDB" }
        singleTestBuildAndRunAux cfg (FSC_BUILDONLY false)

    [<Test >]
    let ``Large inputs 12322 fsc.exe 32-bit .NET Framework generating debug code, full PDB`` () =
        let cfg = testConfig "regression/12322"
        let cfg = { cfg with fsc_flags = cfg.fsc_flags + " --debug:full" }
        singleTestBuildAndRunAux cfg (FSC_BUILDONLY false)

    [<Test >]
    let ``Large inputs 12322 fscAnyCpu.exe 64-bit .NET Framework generating optimized code, portable PDB`` () = 
        let cfg = testConfig "regression/12322"
        let cfg = { cfg with FSC = cfg.FSCANYCPU }
        let cfg = { cfg with fsc_flags = cfg.fsc_flags + " --debug:portable --define:PORTABLE_PDB" }
        singleTestBuildAndRunAux cfg (FSC_BUILDONLY true)

    [<Test >]
    let ``Large inputs 12322 fscAnyCpu.exe 64-bit .NET Framework generating optimized code, full PDB`` () = 
        let cfg = testConfig "regression/12322"
        let cfg = { cfg with FSC = cfg.FSCANYCPU }
        let cfg = { cfg with fsc_flags = cfg.fsc_flags + " --debug:full " }
        singleTestBuildAndRunAux cfg (FSC_BUILDONLY true)

    [<Test >]
    let ``12322 fscAnyCpu.exe 64-bit .NET Framework generating debug code, portable PDB`` () = 
        let cfg = testConfig "regression/12322"
        let cfg = { cfg with FSC = cfg.FSCANYCPU }
        let cfg = { cfg with fsc_flags = cfg.fsc_flags + " --debug:portable --define:PORTABLE_PDB" }
        singleTestBuildAndRunAux cfg (FSC_BUILDONLY false)

    [<Test >]
    let ``12322 fscAnyCpu.exe 64-bit .NET Framework generating debug code, full PDB`` () = 
        let cfg = testConfig "regression/12322"
        let cfg = { cfg with FSC = cfg.FSCANYCPU }
        let cfg = { cfg with fsc_flags = cfg.fsc_flags + " --debug:full" }
        singleTestBuildAndRunAux cfg (FSC_BUILDONLY false)
#endif

#if !NETCOREAPP

    [<Test>]
    let ``SRTP doesn't handle calling member hiding hinherited members`` () =
        let cfg = 
            testConfig "regression/5531"
       

        let outFile = "compilation.output.test.txt"
        let expectedFile = "compilation.output.test.bsl"

        fscBothToOut cfg outFile "%s --nologo -O" cfg.fsc_flags ["test.fs"]

        let diff = fsdiff cfg outFile expectedFile

        match diff with
        | "" -> ()
        | _ ->
            Assert.Fail (sprintf "'%s' and '%s' differ; %A" (getfullpath cfg outFile) (getfullpath cfg expectedFile) diff)

        let outFile2 = "output.test.txt"
        let expectedFile2 = "output.test.bsl"

        execBothToOut cfg (cfg.Directory) outFile2 (cfg.Directory ++ "test.exe") ""

        let diff2 = fsdiff cfg outFile2 expectedFile2
        match diff2 with
        | "" -> ()
        | _ ->
            Assert.Fail (sprintf "'%s' and '%s' differ; %A" (getfullpath cfg outFile2) (getfullpath cfg expectedFile2) diff2)
#endif

    [<Test>]
    let ``26`` () = singleTestBuildAndRunVersion "regression/26" FSC_OPTIMIZED "supports-ml"

    [<Test >]
    let ``321`` () = singleTestBuildAndRunVersion "regression/321" FSC_OPTIMIZED "supports-ml"

#if !NETCOREAPP
    // This test is disabled in coreclr builds dependent on fixing : https://github.com/dotnet/fsharp/issues/2600
    [<Test>]
    let ``655`` () =
        let cfg = testConfig "regression/655"

        fsc cfg "%s --langversion:5.0 --mlcompatibility -a -o:pack.dll" cfg.fsc_flags ["xlibC.ml"]

        peverify cfg "pack.dll"

        fsc cfg "%s --langversion:5.0 --mlcompatibility -o:test.exe -r:pack.dll" cfg.fsc_flags ["main.fs"]

        peverify cfg "test.exe"

        use testOkFile = fileguard cfg "test.ok"

        exec cfg ("." ++ "test.exe") ""

        testOkFile.CheckExists()

    // This test is disabled in coreclr builds dependent on fixing : https://github.com/dotnet/fsharp/issues/2600
    [<Test >]
    let ``656`` () =
        let cfg = testConfig "regression/656"

        fsc cfg "%s --langversion:5.0 --mlcompatibility -o:pack.exe" cfg.fsc_flags ["misc.fs mathhelper.fs filehelper.fs formshelper.fs plot.fs traj.fs playerrecord.fs trackedplayers.fs form.fs"]

        peverify cfg  "pack.exe"
#endif

#if !NETCOREAPP
    // Requires WinForms
    [<Test>]
    let ``83`` () = singleTestBuildAndRunVersion "regression/83" FSC_OPTIMIZED "supports-ml"

    [<Test >]
    let ``84`` () = singleTestBuildAndRunVersion "regression/84" FSC_OPTIMIZED "supports-ml"

    [<Test >]
    let ``85`` () =
        let cfg = testConfig "regression/85"

        fsc cfg "%s --langversion:5.0 --mlcompatibility -r:Category.dll -a -o:petshop.dll" cfg.fsc_flags ["Category.ml"]

        peverify cfg "petshop.dll"
#endif

    [<Test >]
    let ``86`` () = singleTestBuildAndRunVersion "regression/86" FSC_OPTIMIZED "supports-ml"

    [<Test >]
    let ``struct-tuple-bug-1-FSI`` () = singleTestBuildAndRun "regression/struct-tuple-bug-1" FSI

#if !NETCOREAPP
    // This test is disabled in coreclr builds dependent on fixing : https://github.com/dotnet/fsharp/issues/2600
    [<Test>]
    let ``struct-measure-bug-1`` () =
        let cfg = testConfig "regression/struct-measure-bug-1"

        fsc cfg "%s --optimize- -o:test.exe -g" cfg.fsc_flags ["test.fs"]

        peverify cfg "test.exe"

[<NonParallelizable>]
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

        peverify cfg "lib.dll "

        fsc cfg "%s -g --optimize- --target:library -o:lib3.dll -r:lib.dll " cfg.fsc_flags ["lib3.fs"]

        fsc cfg "%s -g --optimize- -o:test.exe -r:lib.dll -r:lib3.dll" cfg.fsc_flags ["test.fs "]

        fsc cfg "%s --optimize --target:library -o:lib--optimize.dll -g" cfg.fsc_flags ["lib.fs"; "lib2.fs"]

        fsc cfg "%s --optimize --target:library -o:lib3--optimize.dll -r:lib--optimize.dll -g" cfg.fsc_flags ["lib3.fs"]

        fsc cfg "%s --optimize -o:test--optimize.exe -g -r:lib--optimize.dll  -r:lib3--optimize.dll" cfg.fsc_flags ["test.fs "]

        ildasm cfg "/out=test.il" "test.exe"

        ildasm cfg "/out=test--optimize.il" "test--optimize.exe"

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

        ildasm cfg "/out=FSharp.Core.il" cfg.FSCOREDLLPATH

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

[<NonParallelizable>]
module TypecheckTests =
    [<Test>]
    let ``full-rank-arrays`` () =
        let cfg = testConfig "typecheck/full-rank-arrays"
        SingleTest.singleTestBuildAndRunWithCopyDlls cfg "full-rank-arrays.dll" FSC_OPTIMIZED

#if !NETCOREAPP

    [<Test>]
    let ``sigs pos26`` () =
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos26.exe" cfg.fsc_flags ["pos26.fsi"; "pos26.fs"]
        peverify cfg "pos26.exe"

    [<Test>]
    let ``sigs pos25`` () =
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos25.exe" cfg.fsc_flags ["pos25.fs"]
        peverify cfg "pos25.exe"

    [<Test>]
    let ``sigs pos27`` () =
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos27.exe" cfg.fsc_flags ["pos27.fs"]
        peverify cfg "pos27.exe"

    [<Test>]
    let ``sigs pos28`` () =
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos28.exe" cfg.fsc_flags ["pos28.fs"]
        peverify cfg "pos28.exe"

    [<Test>]
    let ``sigs pos29`` () =
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos29.exe" cfg.fsc_flags ["pos29.fsi"; "pos29.fs"; "pos29.app.fs"]
        peverify cfg "pos29.exe"

    [<Test>]
    let ``sigs pos30`` () =
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos30.exe --warnaserror+" cfg.fsc_flags ["pos30.fs"]
        peverify cfg "pos30.exe"

    [<Test>]
    let ``sigs pos24`` () =
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos24.exe" cfg.fsc_flags ["pos24.fs"]
        peverify cfg "pos24.exe"

    [<Test>]
    let ``sigs pos31`` () =
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos31.exe --warnaserror" cfg.fsc_flags ["pos31.fsi"; "pos31.fs"]
        peverify cfg "pos31.exe"

    [<Test>]
    let ``sigs pos32`` () =
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:library -o:pos32.dll --warnaserror" cfg.fsc_flags ["pos32.fs"]
        peverify cfg "pos32.dll"

    [<Test>]
    let ``sigs pos33`` () =
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:library -o:pos33.dll --warnaserror" cfg.fsc_flags ["pos33.fsi"; "pos33.fs"]
        peverify cfg "pos33.dll"

    [<Test>]
    let ``sigs pos34`` () =
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:library -o:pos34.dll --warnaserror" cfg.fsc_flags ["pos34.fs"]
        peverify cfg "pos34.dll"

    [<Test>]
    let ``sigs pos35`` () =
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:library -o:pos35.dll --warnaserror" cfg.fsc_flags ["pos35.fs"]
        peverify cfg "pos35.dll"

    [<Test>]
    let ``sigs pos36-srtp`` () =
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:library -o:pos36-srtp-lib.dll --warnaserror" cfg.fsc_flags ["pos36-srtp-lib.fs"]
        fsc cfg "%s --target:exe -r:pos36-srtp-lib.dll -o:pos36-srtp-app.exe --warnaserror" cfg.fsc_flags ["pos36-srtp-app.fs"]
        peverify cfg "pos36-srtp-lib.dll"
        peverify cfg "pos36-srtp-app.exe"
        exec cfg ("." ++ "pos36-srtp-app.exe") ""

    [<Test>]
    let ``sigs pos37`` () =
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:library -o:pos37.dll --warnaserror" cfg.fsc_flags ["pos37.fs"]
        peverify cfg "pos37.dll"

    [<Test>]
    let ``sigs pos38`` () =
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:library -o:pos38.dll --warnaserror" cfg.fsc_flags ["pos38.fs"]
        peverify cfg "pos38.dll"

    [<Test>]
    let ``sigs pos39`` () =
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos39.exe" cfg.fsc_flags ["pos39.fs"]
        peverify cfg "pos39.exe"
        exec cfg ("." ++ "pos39.exe") ""

    [<Test>]
    let ``sigs pos40`` () =
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --langversion:6.0 --target:exe -o:pos40.exe" cfg.fsc_flags ["pos40.fs"]
        peverify cfg "pos40.exe"
        exec cfg ("." ++ "pos40.exe") ""
        
    [<Test>]
    let ``sigs pos41`` () =
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:library -o:pos41.dll --warnaserror --langversion:preview" cfg.fsc_flags ["pos41.fs"]
        peverify cfg "pos41.dll"

    [<Test>]
    let ``sigs pos1281`` () =
        let cfg = testConfig "typecheck/sigs"
        // This checks that warning 25 "incomplete matches" is not triggered
        fsc cfg "%s --target:exe -o:pos1281.exe --warnaserror --nowarn:26" cfg.fsc_flags ["pos1281.fs"]
        peverify cfg "pos1281.exe"
        exec cfg ("." ++ "pos1281.exe") ""

    [<Test>]
    let ``sigs pos3294`` () =
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos3294.exe --warnaserror" cfg.fsc_flags ["pos3294.fs"]
        peverify cfg "pos3294.exe"
        exec cfg ("." ++ "pos3294.exe") ""

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
        fsc cfg "%s -a --langversion:5.0 --mlcompatibility -o:pos01a.dll" cfg.fsc_flags ["pos01a.fsi"; "pos01a.fs"]
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
    let ``type check neg03`` () = singleVersionedNegTest (testConfig "typecheck/sigs") "supports-ml*" "neg03"   

    [<Test>]
    let ``type check neg08`` () = singleNegTest (testConfig "typecheck/sigs") "neg08"

    [<Test>]
    let ``type check neg09`` () = singleNegTest (testConfig "typecheck/sigs") "neg09"

    [<Test>]
    let ``type check neg10`` () = singleNegTest (testConfig "typecheck/sigs") "neg10"   
  
    [<Test>]
    let ``type check neg14`` () = singleNegTest (testConfig "typecheck/sigs") "neg14"  

    [<Test>]
    let ``type check neg17`` () = singleNegTest (testConfig "typecheck/sigs") "neg17"

    [<Test>]
    let ``type check neg24 version 4_6`` () =
        let cfg = testConfig "typecheck/sigs/version46"
        // For some reason this warning is off by default in the test framework but in this case we are testing for it
        let cfg = { cfg with fsc_flags = cfg.fsc_flags.Replace("--nowarn:20", "") }
        singleVersionedNegTest cfg "4.6" "neg24"

    [<Test>]
    let ``type check neg24 version 4_7`` () =
        let cfg = testConfig "typecheck/sigs/version47"
        // For some reason this warning is off by default in the test framework but in this case we are testing for it
        let cfg = { cfg with fsc_flags = cfg.fsc_flags.Replace("--nowarn:20", "") }
        singleVersionedNegTest cfg "4.7" "neg24"

    [<Test>]
    let ``type check neg24 version preview`` () =
        let cfg = testConfig "typecheck/sigs"
        // For some reason this warning is off by default in the test framework but in this case we are testing for it
        let cfg = { cfg with fsc_flags = cfg.fsc_flags.Replace("--nowarn:20", "") }
        singleVersionedNegTest cfg "preview" "neg24"

    [<Test>]
    let ``type check neg27`` () = singleNegTest (testConfig "typecheck/sigs") "neg27"

    [<Test>]
    let ``type check neg31`` () = singleNegTest (testConfig "typecheck/sigs") "neg31"

    [<Test>]
    let ``type check neg33`` () = singleNegTest (testConfig "typecheck/sigs") "neg33"  

    [<Test>]
    let ``type check neg43`` () = singleNegTest (testConfig "typecheck/sigs") "neg43"   

#if !DEBUG // requires release version of compiler to avoid very deep stacks
    [<Test>]
    let ``type check neg45`` () = singleNegTest (testConfig "typecheck/sigs") "neg45"
#endif

    [<Test>]
    let ``type check neg49`` () = singleNegTest (testConfig "typecheck/sigs") "neg49"

    [<Test>]
    let ``type check neg56_a`` () = singleNegTest (testConfig "typecheck/sigs") "neg56_a"   

    [<Test>]
    let ``type check neg94`` () = singleNegTest (testConfig "typecheck/sigs") "neg94" 

    [<Test>]
    let ``type check neg100`` () =
        let cfg = testConfig "typecheck/sigs"
        let cfg = { cfg with fsc_flags = cfg.fsc_flags + " --warnon:3218" }
        singleNegTest cfg "neg100"

    [<Test>]
    let ``type check neg107`` () = singleNegTest (testConfig "typecheck/sigs") "neg107"
 
    [<Test>]
    let ``type check neg116`` () = singleNegTest (testConfig "typecheck/sigs") "neg116"

    [<Test>]
    let ``type check neg117`` () = singleNegTest (testConfig "typecheck/sigs") "neg117"        
    
    [<Test>]
    let ``type check neg134`` () = singleVersionedNegTest (testConfig "typecheck/sigs") "preview" "neg134"

    [<Test>]
    let ``type check neg135`` () = singleVersionedNegTest (testConfig "typecheck/sigs") "preview" "neg135"  

[<NonParallelizable>]
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
        |> Assert.areEqual (45, 2048, 0, 2)


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

#if !NETCOREAPP
[<NonParallelizable>]
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

#endif
