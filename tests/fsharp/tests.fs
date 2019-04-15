// To run these tests in F# Interactive , 'build net40', then send this chunk, then evaluate body of a test vvvvvvvvvvvvvvvv
#if INTERACTIVE
#r @"../../packages/NUnit.3.5.0/lib/net45/nunit.framework.dll"
#load "../../src/scripts/scriptlib.fsx" 
#load "test-framework.fs" 
#load "single-test.fs"
#else
module ``FSharp-Tests``
#endif

open System
open System.IO
open System.Reflection
open System.Reflection.PortableExecutable
open NUnit.Framework
open TestFramework
open Scripting
open SingleTest

// ^^^^^^^^^^^^ To run these tests in F# Interactive , 'build', then send this chunk, then evaluate body of a test ^^^^^^^^^^^^
//
// To run using command line use
//   msbuild tests\fsharp\FSharpSuite.Tests.fsproj /p:Configuration=Release
//   dotnet test tests/fsharp/FSharpSuite.Tests.fsproj --no-build -v:n -c Release
// To run selective tests:
//   dotnet test tests/fsharp/FSharpSuite.Tests.fsproj --no-build -v:n -c Release --filter Name=Test1 
//   dotnet test tests/fsharp/FSharpSuite.Tests.fsproj --no-build -v:n -c Release --filter TestCategory=netfx 
// etc.
//
// NOTE: Some of these tests fail in DEBUG mode (when the Debug configuration of the F# compiler is invoked, e.g. due to stack overflow)

[<Category("coreclr")>]
module CoreTests = 
    // These tests are enabled for .NET Framework and .NET Core
    [<Test>]
    let ``access-coreclr``() = singleTestBuildAndRun "core/access" FSC_CORECLR

    [<Test>]
    let ``access-coreclr-script``() = singleTestBuildAndRun "core/access" FSI_CORECLR_SCRIPT

    [<Test>]
    let ``apporder-coreclr`` () = singleTestBuildAndRun "core/apporder" FSC_CORECLR

    [<Test>]
    let ``apporder-coreclr-script`` () = singleTestBuildAndRun "core/apporder" FSI_CORECLR_SCRIPT

    [<Test>]
    let ``array-coreclr`` () = singleTestBuildAndRun "core/array" FSC_CORECLR

    [<Test>]
    let ``array-coreclr-script`` () = singleTestBuildAndRun "core/array" FSI_CORECLR_SCRIPT

    [<Test>]
    let ``comprehensions-coreclr`` () = singleTestBuildAndRun "core/comprehensions" FSC_CORECLR

    [<Test>]
    let ``comprehensions-coreclr-script`` () = singleTestBuildAndRun "core/comprehensions" FSI_CORECLR_SCRIPT

    [<Test>]
    let ``comprehensionshw-coreclr`` () = singleTestBuildAndRun "core/comprehensions-hw" FSC_CORECLR

    [<Test>]
    let ``comprehensionshw-coreclr-script`` () = singleTestBuildAndRun "core/comprehensions-hw" FSI_CORECLR_SCRIPT

    [<Test>]
    let ``genericmeasures-coreclr-script`` () = singleTestBuildAndRun "core/genericmeasures" FSI_CORECLR_SCRIPT

    [<Test>]
    let ``genericmeasures-coreclr`` () = singleTestBuildAndRun "core/genericmeasures" FSC_CORECLR

    [<Test>]
    let ``innerpoly-coreclr-script`` () = singleTestBuildAndRun "core/innerpoly" FSI_CORECLR_SCRIPT

    [<Test>]
    let ``innerpoly-coreclr`` () = singleTestBuildAndRun "core/innerpoly" FSC_CORECLR

    [<Test>]
    let ``namespaceAttributes-coreclr`` () = singleTestBuildAndRun "core/namespaces" FSC_CORECLR

    [<Test>]
    let ``lazy test-coreclr`` () = singleTestBuildAndRun "core/lazy" FSC_CORECLR

    [<Test>]
    let ``lazy test-coreclr-script`` () = singleTestBuildAndRun "core/lazy" FSI_CORECLR_SCRIPT

    [<Test>]
    let ``letrec-coreclr`` () = singleTestBuildAndRun "core/letrec" FSC_CORECLR

    [<Test>]
    let ``letrec-coreclr-script`` () = singleTestBuildAndRun "core/letrec" FSI_CORECLR_SCRIPT

    [<Test>]
    let ``letrec (mutrec variations part one) coreclr`` () = singleTestBuildAndRun "core/letrec-mutrec" FSC_CORECLR

    [<Test>]
    let ``letrec (mutrec variations part one) FSI_CORECLR_SCRIPT`` () = singleTestBuildAndRun "core/letrec-mutrec" FSI_CORECLR_SCRIPT

    [<Test>]
    let ``libtest-coreclr`` () = singleTestBuildAndRun "core/libtest" FSC_CORECLR

    [<Test>]
    let ``lift-coreclr`` () = singleTestBuildAndRun "core/lift" FSC_CORECLR

    [<Test>]
    let ``map-coreclr`` () = singleTestBuildAndRun "core/map" FSC_CORECLR

    [<Test>]
    let ``measures-coreclr-script`` () = singleTestBuildAndRun "core/measures" FSI_CORECLR_SCRIPT

    [<Test>]
    let ``measures-coreclr`` () = singleTestBuildAndRun "core/measures" FSC_CORECLR

    [<Test>]
    let ``nested-coreclr`` () = singleTestBuildAndRun "core/nested" FSC_CORECLR

    [<Test>]
    let ``members-ops-coreclr`` () = singleTestBuildAndRun "core/members/ops" FSC_CORECLR

    [<Test>]
    let ``members-ops-mutrec-coreclr`` () = singleTestBuildAndRun "core/members/ops-mutrec" FSC_CORECLR

    [<Test>]
    let ``seq-coreclr`` () = singleTestBuildAndRun "core/seq" FSC_CORECLR

    [<Test>]
    let ``math-numbers-coreclr`` () = singleTestBuildAndRun "core/math/numbers" FSC_CORECLR

    [<Test>]
    let ``members-ctree-coreclr`` () = singleTestBuildAndRun "core/members/ctree" FSC_CORECLR

    [<Test>]
    let ``members-factors-coreclr`` () = singleTestBuildAndRun "core/members/factors" FSC_CORECLR

    [<Test>]
    let ``members-factors-mutrec-coreclr`` () = singleTestBuildAndRun "core/members/factors-mutrec" FSC_CORECLR

    [<Test>]
    let ``graph-coreclr`` () = singleTestBuildAndRun "perf/graph" FSC_CORECLR

    [<Test>]
    let ``nbody-coreclr`` () = singleTestBuildAndRun "perf/nbody" FSC_CORECLR

    [<Test>]
    let ``letrec-mutrec2-coreclr`` () = singleTestBuildAndRun "core/letrec-mutrec2" FSC_CORECLR

    [<Test>]
    let ``printf-coreclr`` () = singleTestBuildAndRun "core/printf" FSC_CORECLR

    [<Test>]
    let ``tlr-coreclr`` () = singleTestBuildAndRun "core/tlr" FSC_CORECLR

    [<Test>]
    let ``subtype-coreclr`` () = singleTestBuildAndRun "core/subtype" FSC_CORECLR

    [<Test>]
    let ``syntax-coreclr`` () = singleTestBuildAndRun "core/syntax" FSC_CORECLR

    [<Test>]
    let ``test int32-coreclr`` () = singleTestBuildAndRun "core/int32" FSC_CORECLR

    [<Test>]
    let ``control-coreclr`` () = singleTestBuildAndRun "core/control" FSC_CORECLR

    [<Test>]
    let ``controlChamenos-coreclr`` () = singleTestBuildAndRun "core/controlChamenos" FSC_CORECLR

    [<Test>]
    let ``controlMailbox-coreclr`` () = singleTestBuildAndRun "core/controlMailbox" FSC_CORECLR

    [<Test>]
    let ``csext-coreclr`` () = singleTestBuildAndRun "core/csext" FSC_CORECLR

    [<Test>]
    let ``fscenum-coreclr`` () = singleTestBuildAndRun "core/enum" FSC_CORECLR

    [<Test>]
    let ``longnames-coreclr`` () = singleTestBuildAndRun "core/longnames" FSC_CORECLR

    [<Test>]
    let ``math-numbersVS2008-coreclr`` () = singleTestBuildAndRun "core/math/numbersVS2008" FSC_CORECLR

    [<Test>]
    let ``patterns-coreclr`` () = singleTestBuildAndRun "core/patterns" FSC_CORECLR

    [<Test>]
    let ``reflect-coreclr`` () = singleTestBuildAndRun "core/reflect" FSC_CORECLR

[<Category("coreclr")>]
module RegressionTests = 

    [<Test>]
    let ``literal-value-bug-2-coreclr`` () = singleTestBuildAndRun "regression/literal-value-bug-2" FSC_CORECLR

    [<Test>]
    let ``OverloadResolution-bug-coreclr`` () = singleTestBuildAndRun "regression/OverloadResolution-bug" FSC_CORECLR

    [<Test>]
    let ``struct-tuple-bug-1-coreclr`` () = singleTestBuildAndRun "regression/struct-tuple-bug-1" FSC_CORECLR

    [<Test >]
    let ``tuple-bug-1-coreclr`` () = singleTestBuildAndRun "regression/tuple-bug-1" FSC_CORECLR

    [<Test>]
    let ``regression-26-coreclr`` () = singleTestBuildAndRun "regression/26" FSC_CORECLR

    [<Test >]
    let ``regression-321-coreclr`` () = singleTestBuildAndRun "regression/321" FSC_CORECLR

    [<Test >]
    let ``regression-86-coreclr`` () = singleTestBuildAndRun "regression/86" FSC_CORECLR

#if INCLUDE_NETFX_TESTS
[<Category("netfx")>]
module NetFxTests =
    [<Test>]
    let ``literal-value-bug-2-script-netfx`` () = singleTestBuildAndRun "regression/literal-value-bug-2" FSI_NETFX_SCRIPT

    [<Test>]
    let ``OverloadResolution-bug-script-netfx`` () = singleTestBuildAndRun "regression/OverloadResolution-bug" FSI_NETFX_SCRIPT

    [<Test >]
    let ``struct-tuple-bug-1-script-netfx`` () = singleTestBuildAndRun "regression/struct-tuple-bug-1" FSI_NETFX_SCRIPT

    [<Test>]
    let ``access-netfx``() = singleTestBuildAndRun "core/access" FSC_NETFX

    [<Test>]
    let ``access-script-netfx``() = singleTestBuildAndRun "core/access" FSI_NETFX_SCRIPT

    [<Test>]
    let ``apporder-netfx`` () = singleTestBuildAndRun "core/apporder" FSC_NETFX

    [<Test>]
    let ``array-netfx`` () = singleTestBuildAndRun "core/array" FSC_NETFX

    [<Test>]
    let ``array-script-netfx`` () = singleTestBuildAndRun "core/array" FSI_NETFX_SCRIPT

    [<Test>]
    let ``comprehensions-netfx`` () = singleTestBuildAndRun "core/comprehensions" FSC_NETFX

    [<Test>]
    let ``apporder-script-netfx`` () = singleTestBuildAndRun "core/apporder" FSI_NETFX_SCRIPT

    [<Test>]
    let ``comprehensions-script-netfx`` () = singleTestBuildAndRun "core/comprehensions" FSI_NETFX_SCRIPT

    [<Test>]
    let ``comprehensionshw-netfx`` () = singleTestBuildAndRun "core/comprehensions-hw" FSC_NETFX

    [<Test>]
    let ``comprehensionshw-script-netfx`` () = singleTestBuildAndRun "core/comprehensions-hw" FSI_NETFX_SCRIPT

    [<Test>]
    let ``genericmeasures-netfx`` () = singleTestBuildAndRun "core/genericmeasures" FSC_NETFX

    [<Test>]
    let ``genericmeasures-script-netfx`` () = singleTestBuildAndRun "core/genericmeasures" FSI_NETFX_SCRIPT

    [<Test>]
    let ``innerpoly-netfx`` () = singleTestBuildAndRun "core/innerpoly" FSC_NETFX

    [<Test>]
    let ``namespaceAttributes-netfx`` () = singleTestBuildAndRun "core/namespaces" FSC_NETFX

    [<Test>]
    let ``unicode2-netfx`` () = singleTestBuildAndRun "core/unicode" FSC_NETFX // TODO: fails on coreclr

    [<Test>]
    let ``unicode2-script-netfx`` () = singleTestBuildAndRun "core/unicode" FSI_NETFX_SCRIPT

    [<Test>]
    let ``lazy test-netfx`` () = singleTestBuildAndRun "core/lazy" FSC_NETFX

    [<Test>]
    let ``lazy test-script-netfx`` () = singleTestBuildAndRun "core/lazy" FSI_NETFX_SCRIPT

    [<Test>]
    let ``letrec-netfx`` () = singleTestBuildAndRun "core/letrec" FSC_NETFX

    [<Test>]
    let ``letrec-script-netfx`` () = singleTestBuildAndRun "core/letrec" FSI_NETFX_SCRIPT

    [<Test>]
    let ``letrec (mutrec variations part one)-netfx`` () = singleTestBuildAndRun "core/letrec-mutrec" FSC_NETFX

    [<Test>]
    let ``letrec (mutrec variations part one)-script-netfx`` () = singleTestBuildAndRun "core/letrec-mutrec" FSI_NETFX_SCRIPT

    [<Test>]
    let ``innerpoly-script-netfx`` () = singleTestBuildAndRun "core/innerpoly" FSI_NETFX_SCRIPT

    [<Test>]
    let ``measures-script-netfx`` () = singleTestBuildAndRun "core/measures" FSI_NETFX_SCRIPT

    [<Test>]
    let ``members-ops-netfx`` () = singleTestBuildAndRun "core/members/ops" FSC_NETFX

    [<Test>]
    let ``members-ops-mutrec-netfx`` () = singleTestBuildAndRun "core/members/ops-mutrec" FSC_NETFX

    [<Test>]
    let ``libtest-netfx`` () = singleTestBuildAndRun "core/libtest" FSC_NETFX

    [<Test>]
    let ``lift-netfx`` () = singleTestBuildAndRun "core/lift" FSC_NETFX

    [<Test>]
    let ``map-netfx`` () = singleTestBuildAndRun "core/map" FSC_NETFX

    [<Test>]
    let ``measures-netfx`` () = singleTestBuildAndRun "core/measures" FSC_NETFX

    [<Test>]
    let ``nested-netfx`` () = singleTestBuildAndRun "core/nested" FSC_NETFX

    [<Test>]
    let ``seq-netfx`` () = singleTestBuildAndRun "core/seq" FSC_NETFX

    [<Test>]
    let ``math-numbers-netfx`` () = singleTestBuildAndRun "core/math/numbers" FSC_NETFX

    [<Test>]
    let ``members-ctree-netfx`` () = singleTestBuildAndRun "core/members/ctree" FSC_NETFX

    [<Test>]
    let ``members-factors-netfx`` () = singleTestBuildAndRun "core/members/factors" FSC_NETFX

    [<Test>]
    let ``members-factors-mutrec-netfx`` () = singleTestBuildAndRun "core/members/factors-mutrec" FSC_NETFX

    [<Test>]
    let ``graph-netfx`` () = singleTestBuildAndRun "perf/graph" FSC_NETFX

    [<Test>]
    let ``nbody-netfx`` () = singleTestBuildAndRun "perf/nbody" FSC_NETFX

    [<Test>]
    let ``letrec-mutrec2-netfx`` () = singleTestBuildAndRun "core/letrec-mutrec2" FSC_NETFX

    [<Test>]
    let ``printf-netfx`` () = singleTestBuildAndRun "core/printf" FSC_NETFX

    [<Test>]
    let ``tlr-netfx`` () = singleTestBuildAndRun "core/tlr" FSC_NETFX

    [<Test>]
    let ``subtype-netfx`` () = singleTestBuildAndRun "core/subtype" FSC_NETFX

    [<Test>]
    let ``syntax-netfx`` () = singleTestBuildAndRun "core/syntax" FSC_NETFX

    [<Test>]
    let ``test int32-netfx`` () = singleTestBuildAndRun "core/int32" FSC_NETFX

    [<Test>]
    let ``attributes-netfx`` () = singleTestBuildAndRun "core/attributes" FSC_NETFX

    [<Test>]
    let ``attributes-script-netfx`` () = singleTestBuildAndRun "core/attributes" FSI_NETFX_SCRIPT

    [<Test>]
    let ``control-netfx`` () = singleTestBuildAndRun "core/control" FSC_NETFX

    [<Test>]
    let ``controlChamenos-netfx`` () =  singleTestBuildAndRun "core/controlChamenos" FSC_NETFX

    [<Test>]
    let ``controlMailbox-netfx`` () = singleTestBuildAndRun "core/controlMailbox" FSC_NETFX

    [<Test>]
    let ``csext-netfx`` () = singleTestBuildAndRun "core/csext" FSC_NETFX

    [<Test>]
    let ``fscenum-netfx`` () = singleTestBuildAndRun "core/enum" FSC_NETFX

//    [<Test>]
//    let fsienum () = singleTestBuildAndRun "core/enum" FSI_NETFX_SCRIPT

    [<Test>]
    let ``longnames-netfx`` () = singleTestBuildAndRun "core/longnames" FSC_NETFX

    [<Test>]
    let ``math-numbersVS2008-netfx`` () = singleTestBuildAndRun "core/math/numbersVS2008" FSC_NETFX

    [<Test>]
    let ``patterns-netfx`` () = singleTestBuildAndRun "core/patterns" FSC_NETFX

    [<Test>]
    let ``reflect-netfx`` () = singleTestBuildAndRun "core/reflect" FSC_NETFX

    [<Test>]
    let ``regression-literal-value-bug-2-netfx`` () = singleTestBuildAndRun "regression/literal-value-bug-2" FSC_NETFX

    [<Test>]
    let ``regression-literal-value-bug-2-script-netfx`` () = singleTestBuildAndRun "regression/literal-value-bug-2" FSI_NETFX_SCRIPT

    [<Test>]
    let ``regression-OverloadResolution-bug-netfx`` () = singleTestBuildAndRun "regression/OverloadResolution-bug" FSC_NETFX

    [<Test>]
    let ``regression-OverloadResolution-bug-script-netfx`` () = singleTestBuildAndRun "regression/OverloadResolution-bug" FSI_NETFX_SCRIPT

    [<Test>]
    let ``regression-struct-tuple-bug-1-netfx`` () = singleTestBuildAndRun "regression/struct-tuple-bug-1" FSC_NETFX

    [<Test >]
    let ``regression-tuple-bug-1-netfx`` () = singleTestBuildAndRun "regression/tuple-bug-1" FSC_NETFX

    [<Test>]
    let ``regression-26-netfx`` () = singleTestBuildAndRun "regression/26" FSC_NETFX

    [<Test >]
    let ``regression-321-netfx`` () = singleTestBuildAndRun "regression/321" FSC_NETFX

    [<Test >]
    let ``regression-86-netfx`` () = singleTestBuildAndRun "regression/86" FSC_NETFX

    [<Test >]
    let ``regression-struct-tuple-bug-1-script-netfx`` () = singleTestBuildAndRun "regression/struct-tuple-bug-1" FSI_NETFX_SCRIPT

    [<Test>]
    let ``byrefs-netfx`` () = 

        let cfg = testConfig "core/byrefs"

        begin
            use testOkFile = fileguard cfg "test.ok"

            fsc cfg "%s -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

            singleNegTest cfg "test"

            exec cfg ("." ++ "test.exe") ""

            testOkFile.CheckExists()
        end

        begin
            use testOkFile = fileguard cfg "test2.ok"

            fsc cfg "%s -o:test2.exe -g" cfg.fsc_flags ["test2.fsx"]

            singleNegTest { cfg with fsc_flags = sprintf "%s --warnaserror-" cfg.fsc_flags } "test2"

            exec cfg ("." ++ "test2.exe") ""

            testOkFile.CheckExists()
        end

        begin
            csc cfg """/langversion:7.2 /nologo /target:library /out:cslib3.dll""" ["cslib3.cs"]

            use testOkFile = fileguard cfg "test3.ok"

            fsc cfg "%s -r:cslib3.dll -o:test3.exe -g" cfg.fsc_flags ["test3.fsx"]

            singleNegTest { cfg with fsc_flags = sprintf "%s -r:cslib3.dll" cfg.fsc_flags } "test3"

            exec cfg ("." ++ "test3.exe") ""

            testOkFile.CheckExists()
        end

    [<Test>]
    let ``span-netfx`` () = 

        let cfg = testConfig "core/span"

        let cfg = { cfg with fsc_flags = sprintf "%s --test:StackSpan" cfg.fsc_flags}

        begin
            use testOkFile = fileguard cfg "test.ok"

            singleNegTest cfg "test"

            fsc cfg "%s -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

            // Execution is disabled until we can be sure .NET 4.7.2 is on the machine and System.Memory available
            //exec cfg ("." ++ "test.exe") ""
            //
            //testOkFile.CheckExists()
        end

        begin
            use testOkFile = fileguard cfg "test2.ok"

            singleNegTest cfg "test2"

            fsc cfg "%s -o:test2.exe -g" cfg.fsc_flags ["test2.fsx"]

            // Execution is disabled until we can be sure .NET 4.7.2 is on the machine and System.Memory available
            //exec cfg ("." ++ "test.exe") ""
            //
            //testOkFile.CheckExists()
        end

        begin
            use testOkFile = fileguard cfg "test3.ok"

            singleNegTest cfg "test3"

            fsc cfg "%s -o:test3.exe -g" cfg.fsc_flags ["test3.fsx"]

            // Execution is disabled until we can be sure .NET 4.7.2 is on the machine and System.Memory available
            //exec cfg ("." ++ "test.exe") ""
            //
            //testOkFile.CheckExists()
        end

    [<Test>]
    let ``asyncStackTraces-netfx`` () = 
        let cfg = testConfig "core/asyncStackTraces"

        use testOkFile = fileguard cfg "test.ok"

        fsc cfg "%s -o:test.exe -g --tailcalls- --optimize-" cfg.fsc_flags ["test.fsx"]

        exec cfg ("." ++ "test.exe") ""

        testOkFile.CheckExists()

    [<Test>]
    let ``lots-of-conditionals-netfx``() = 
        let cfg = testConfig "core/large/conditionals"
        use testOkFile = fileguard cfg "test.ok"
        fsc cfg "%s -o:test.exe " cfg.fsc_flags ["LargeConditionals-200.fs"]
        exec cfg ("." ++ "test.exe") ""
        testOkFile.CheckExists()

    [<Test>]
    let ``lots-of-conditionals-maxtested-netfx``() = 
        let cfg = testConfig "core/large/conditionals"
        use testOkFile = fileguard cfg "test.ok"
        fsc cfg "%s -o:test.exe " cfg.fsc_flags ["LargeConditionals-maxtested.fs"]
        exec cfg ("." ++ "test.exe") ""
        testOkFile.CheckExists()

    [<Test>]
    let ``lots-of-lets-netfx``() = 
        let cfg = testConfig "core/large/lets"
        use testOkFile = fileguard cfg "test.ok"
        fsc cfg "%s -o:test.exe " cfg.fsc_flags ["LargeLets-500.fs"]
        exec cfg ("." ++ "test.exe") ""
        testOkFile.CheckExists()

    [<Test>]
    let ``lots-of-lets-maxtested-netfx``() = 
        let cfg = testConfig "core/large/lets"
        use testOkFile = fileguard cfg "test.ok"
        fsc cfg "%s -o:test.exe " cfg.fsc_flags ["LargeLets-maxtested.fs"]
        exec cfg ("." ++ "test.exe") ""
        testOkFile.CheckExists()

    [<Test>]
    let ``lots-of-lists-netfx``() = 
        let cfg = testConfig "core/large/lists"
        use testOkFile = fileguard cfg "test.ok"
        fsc cfg "%s -o:test-500.exe " cfg.fsc_flags ["LargeList-500.fs"]
        exec cfg ("." ++ "test-500.exe") ""
        testOkFile.CheckExists()

    [<Test>]
    let ``lots-of-matches-netfx``() = 
        let cfg = testConfig "core/large/matches"
        use testOkFile = fileguard cfg "test.ok"
        fsc cfg "%s -o:test.exe " cfg.fsc_flags ["LargeMatches-200.fs"]
        exec cfg ("." ++ "test.exe") ""
        testOkFile.CheckExists()

    [<Test>]
    let ``lots-of-matches-maxtested-netfx``() = 
        let cfg = testConfig "core/large/matches"
        use testOkFile = fileguard cfg "test.ok"
        fsc cfg "%s -o:test.exe " cfg.fsc_flags ["LargeMatches-maxtested.fs"]
        exec cfg ("." ++ "test.exe") ""
        testOkFile.CheckExists()

    [<Test>]
    let ``lots-of-sequential-and-let-netfx``() = 
        let cfg = testConfig "core/large/mixed"
        use testOkFile = fileguard cfg "test.ok"
        fsc cfg "%s -o:test.exe " cfg.fsc_flags ["LargeSequentialLet-500.fs"]
        exec cfg ("." ++ "test.exe") ""
        testOkFile.CheckExists()

    [<Test>]
    let ``lots-of-sequential-and-let-maxtested-netfx``() = 
        let cfg = testConfig "core/large/mixed"
        use testOkFile = fileguard cfg "test.ok"
        fsc cfg "%s -o:test.exe " cfg.fsc_flags ["LargeSequentialLet-maxtested.fs"]
        exec cfg ("." ++ "test.exe") ""
        testOkFile.CheckExists()

    [<Test>]
    let ``lots-of-sequential-netfx``() = 
        let cfg = testConfig "core/large/sequential"
        use testOkFile = fileguard cfg "test.ok"
        fsc cfg "%s -o:test.exe " cfg.fsc_flags ["LargeSequential-500.fs"]
        exec cfg ("." ++ "test.exe") ""
        testOkFile.CheckExists()

    [<Test>]
    let ``lots-of-sequential-maxtested-netfx``() = 
        let cfg = testConfig "core/large/sequential"
        use testOkFile = fileguard cfg "test.ok"
        fsc cfg "%s -o:test.exe " cfg.fsc_flags ["LargeSequential-maxtested.fs"]
        exec cfg ("." ++ "test.exe") ""
        testOkFile.CheckExists()

    // Requires winforms will not run on coreclr
    [<Test>]
    let ``controlWpf-netfx`` () = singleTestBuildAndRun "core/controlwpf" FSC_NETFX


    // These tests are enabled for .NET Framework and .NET Core
    [<Test>]
    let ``anon-netfx``() = 
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
    let ``events-netfx`` () = 
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
    let ``forwarders-netfx`` () = 
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
    let ``fsfromcs-netfx`` () = 
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
    let ``fsfromfsviacs-netfx`` () = 
        let cfg = testConfig "core/fsfromfsviacs"

        fsc cfg "%s -a -o:lib.dll -g" cfg.fsc_flags ["lib.fs"]

        peverify cfg "lib.dll"

        csc cfg """/nologo /target:library /r:"%s" /r:lib.dll /out:lib2.dll /langversion:7.2""" cfg.FSCOREDLLPATH ["lib2.cs"]

        csc cfg """/nologo /target:library /r:"%s" /out:lib3.dll  /langversion:7.2""" cfg.FSCOREDLLPATH ["lib3.cs"]

        fsc cfg "%s -r:lib.dll -r:lib2.dll -r:lib3.dll -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test.exe"

        exec cfg ("." ++ "test.exe") ""

        // Same with library references the other way around
        fsc cfg "%s -r:lib.dll -r:lib3.dll -r:lib2.dll -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test.exe"

        exec cfg ("." ++ "test.exe") ""

        // Same without the reference to lib.dll - testing an incomplete reference set, but only compiling a subset of the code
        fsc cfg "%s -r:System.Runtime.dll --noframework --define:NO_LIB_REFERENCE -r:lib3.dll -r:lib2.dll -o:test.exe -g" cfg.fsc_flags ["test.fsx"]

        peverify cfg "test.exe"

        exec cfg ("." ++ "test.exe") ""

    [<Test>]
    let ``fsi-reference-netfx`` () = 

        let cfg = testConfig "core/fsi-reference"

        begin
            use testOkFile = fileguard cfg "test.ok"
            fsc cfg @"--target:library -o:ImplementationAssembly\ReferenceAssemblyExample.dll" ["ImplementationAssembly.fs"]
            fsc cfg @"--target:library -o:ReferenceAssembly\ReferenceAssemblyExample.dll" ["ReferenceAssembly.fs"]
            fsiStdin cfg "test.fsx" "" []
            testOkFile.CheckExists()
        end

    [<Test>]
    let ``fsi-reload-netfx`` () = 
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
    let ``fsiAndModifiers-netfx`` () = 
        let cfg = testConfig "core/fsiAndModifiers"

        do if fileExists cfg "TestLibrary.dll" then rm cfg "TestLibrary.dll"

        fsiStdin cfg "prepare.fsx" "--maxerrors:1" []

        use testOkFile = fileguard cfg "test.ok"

        fsiStdin cfg "test.fsx" "--maxerrors:1"  []

        testOkFile.CheckExists()
                
    [<Test>]
    let ``genericmeasures-dll-netfx`` () = singleTestBuildAndRun "core/genericmeasures" FSC_NETFX_AS_DLL

    [<Test>]
    let ``hiding-netfx`` () = 
        let cfg = testConfig "core/hiding"

        fsc cfg "%s -a --optimize -o:lib.dll" cfg.fsc_flags ["lib.mli";"lib.ml";"libv.ml"]

        peverify cfg "lib.dll"

        fsc cfg "%s -a --optimize -r:lib.dll -o:lib2.dll" cfg.fsc_flags ["lib2.mli";"lib2.ml";"lib3.ml"]

        peverify cfg "lib2.dll"

        fsc cfg "%s --optimize -r:lib.dll -r:lib2.dll -o:client.exe" cfg.fsc_flags ["client.ml"]

        peverify cfg "client.exe"

    [<Test>]
    let ``innerpoly-dll-netfx`` () = singleTestBuildAndRun "core/innerpoly"  FSC_NETFX_AS_DLL       

    [<Test>]
    let ``queriesCustomQueryOps-netfx`` () = 
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
    //     fsi.exe --nologo < test.fsx >a.out 2>a.err
    // then 
    ///    windiff z.output.test.default.stdout.bsl a.out
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
        let ``findstr /v`` (text: string) = Seq.filter (fun (s: string) -> not <| s.Contains(text))
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
    let ``printing-1-netfx`` () = 
         printing "" "z.output.test.default.stdout.txt" "z.output.test.default.stdout.bsl" "z.output.test.default.stderr.txt" "z.output.test.default.stderr.bsl"

    [<Test>]
    let ``printing-2-netfx`` () = 
         printing "--use:preludePrintSize1000.fsx" "z.output.test.1000.stdout.txt" "z.output.test.1000.stdout.bsl" "z.output.test.1000.stderr.txt" "z.output.test.1000.stderr.bsl"

    [<Test>]
    let ``printing-3-netfx`` () = 
         printing "--use:preludePrintSize200.fsx" "z.output.test.200.stdout.txt" "z.output.test.200.stdout.bsl" "z.output.test.200.stderr.txt" "z.output.test.200.stderr.bsl"

    [<Test>]
    let ``printing-4-netfx`` () = 
         printing "--use:preludeShowDeclarationValuesFalse.fsx" "z.output.test.off.stdout.txt" "z.output.test.off.stdout.bsl" "z.output.test.off.stderr.txt" "z.output.test.off.stderr.bsl"

    [<Test>]
    let ``printing-5-netfx`` () = 
         printing "--quiet" "z.output.test.quiet.stdout.txt" "z.output.test.quiet.stdout.bsl" "z.output.test.quiet.stderr.txt" "z.output.test.quiet.stderr.bsl"

    type SigningType =
        | DelaySigned
        | PublicSigned
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

        Assert.AreEqual(expectedSigning, actualSigning)

    [<Test; Category("signedtest")>]
    let ``signedtest-1-netfx`` () = signedtest("test-unsigned", "", SigningType.NotSigned)

    [<Test; Category("signedtest")>]
    let ``signedtest-2-netfx`` () = signedtest("test-sha1-full-cl", "--keyfile:sha1full.snk", SigningType.PublicSigned)

    [<Test; Category("signedtest")>]
    let ``signedtest-3-netfx`` () = signedtest("test-sha256-full-cl", "--keyfile:sha256full.snk", SigningType.PublicSigned)

    [<Test; Category("signedtest")>]
    let ``signedtest-4-netfx`` () = signedtest("test-sha512-full-cl", "--keyfile:sha512full.snk", SigningType.PublicSigned)

    [<Test; Category("signedtest")>]
    let ``signedtest-5-netfx`` () = signedtest("test-sha1024-full-cl", "--keyfile:sha1024full.snk", SigningType.PublicSigned)

    [<Test; Category("signedtest")>]
    let ``signedtest-6-netfx`` () = signedtest("test-sha1-delay-cl", "--keyfile:sha1delay.snk --delaysign", SigningType.DelaySigned)

    [<Test; Category("signedtest")>]
    let ``signedtest-7-netfx`` () = signedtest("test-sha256-delay-cl", "--keyfile:sha256delay.snk --delaysign", SigningType.DelaySigned)

    [<Test; Category("signedtest")>]
    let ``signedtest-8-netfx`` () = signedtest("test-sha512-delay-cl", "--keyfile:sha512delay.snk --delaysign", SigningType.DelaySigned)

    [<Test; Category("signedtest")>]
    let ``signedtest-9-netfx`` () = signedtest("test-sha1024-delay-cl", "--keyfile:sha1024delay.snk --delaysign", SigningType.DelaySigned)

    // Test SHA1 key full signed  Attributes
    [<Test; Category("signedtest")>]
    let ``signedtest-10-netfx`` () = signedtest("test-sha1-full-attributes", "--define:SHA1", SigningType.PublicSigned)

    // Test SHA1 key delayl signed  Attributes
    [<Test; Category("signedtest")>]
    let ``signedtest-11-netfx`` () = signedtest("test-sha1-delay-attributes", "--keyfile:sha1delay.snk --define:SHA1 --define:DELAY", SigningType.DelaySigned)

    [<Test; Category("signedtest")>]
    let ``signedtest-12-netfx`` () = signedtest("test-sha256-full-attributes", "--define:SHA256", SigningType.PublicSigned)

    // Test SHA 256 bit key delay signed  Attributes
    [<Test; Category("signedtest")>]
    let ``signedtest-13-netfx`` () = signedtest("test-sha256-delay-attributes", "--define:SHA256 --define:DELAY", SigningType.DelaySigned)

    // Test SHA 512 bit key fully signed  Attributes
    [<Test; Category("signedtest")>]
    let ``signedtest-14-netfx`` () = signedtest("test-sha512-full-attributes", "--define:SHA512", SigningType.PublicSigned)

    // Test SHA 512 bit key delay signed Attributes
    [<Test; Category("signedtest")>]
    let ``signedtest-15-netfx`` () = signedtest("test-sha512-delay-attributes", "--define:SHA512 --define:DELAY", SigningType.DelaySigned)

    // Test SHA 1024 bit key fully signed  Attributes
    [<Test; Category("signedtest")>]
    let ``signedtest-16-netfx`` () = signedtest("test-sha1024-full-attributes", "--define:SHA1024", SigningType.PublicSigned)

    [<Test>]
    let ``quotes-script-netfx`` () = singleTestBuildAndRun "core/quotes" FSI_NETFX_SCRIPT

    [<Test>]
    let ``quotes-netfx`` () = 
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
    let ``parsing-netfx`` () = 
        let cfg = testConfig "core/parsing"
        
        fsc cfg "%s -a -o:crlf.dll -g" cfg.fsc_flags ["crlf.ml"]

        fsc cfg "%s -o:toplet.exe -g" cfg.fsc_flags ["toplet.ml"]

        peverify cfg "toplet.exe"

    [<Test>]
    let ``unicode-netfx`` () = 
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
    let ``internalsvisible-netfx`` () = 
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
    let ``fileorder-netfx`` () = 
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

    // Repro for https://github.com/Microsoft/visualfsharp/issues/2679
    [<Test>]
    let ``add files with same name from different folders-netfx`` () = 
        let cfg = testConfig "core/samename"

        log "== Compiling F# Code with files with same name in different folders"
        fsc cfg "%s -o:test.exe" cfg.fsc_flags ["folder1/a.fs"; "folder1/b.fs"; "folder2/a.fs"; "folder2/b.fs"]

        peverify cfg "test.exe"

        exec cfg ("." ++ "test.exe") ""

    [<Test>]
    let ``add files with same name from different folders including signature files-netfx`` () =
        let cfg = testConfig "core/samename"

        log "== Compiling F# Code with files with same name in different folders including signature files"
        fsc cfg "%s -o:test.exe" cfg.fsc_flags ["folder1/a.fsi"; "folder1/a.fs"; "folder1/b.fsi"; "folder1/b.fs"; "folder2/a.fsi"; "folder2/a.fs"; "folder2/b.fsi"; "folder2/b.fs"]

        peverify cfg "test.exe"

        exec cfg ("." ++ "test.exe") ""

    [<Test>]
    let ``add files with same name from different folders including signature files that are not synced-netfx`` () =
        let cfg = testConfig "core/samename"

        log "== Compiling F# Code with files with same name in different folders including signature files"
        fsc cfg "%s -o:test.exe" cfg.fsc_flags ["folder1/a.fsi"; "folder1/a.fs"; "folder1/b.fs"; "folder2/a.fsi"; "folder2/a.fs"; "folder2/b.fsi"; "folder2/b.fs"]

        peverify cfg "test.exe"

        exec cfg ("." ++ "test.exe") ""

    [<Test>]
    let ``libtest-script-stdin-netfx`` () = singleTestBuildAndRun "core/libtest" FSI_NETFX_SCRIPT_STDIN

    [<Test>]
    let ``libtest-debug-netfx`` () = singleTestBuildAndRun "core/libtest" FSC_NETFX_DEBUG

    [<Test>]
    let ``libtest-dll-netfx`` () = singleTestBuildAndRun "core/libtest" FSC_NETFX_AS_DLL

    [<Test>]
    let ``libtest-netfx-scriptnetfx`` () = singleTestBuildAndRun "core/libtest" FSI_NETFX_SCRIPT

    [<Test>]
    let ``letrec-mutrec2-script-netfx`` () = singleTestBuildAndRun "core/letrec-mutrec2" FSI_NETFX_SCRIPT

    [<Test>]
    let ``recordResolution-netfx`` () = singleTestBuildAndRun "core/recordResolution" FSC_NETFX

    [<Test>]
    let ``no-warn-2003-tests-netfx`` () =
        // see https://github.com/Microsoft/visualfsharp/issues/3139
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
    let ``load-script-netfx`` () = 
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

    [<Test>]
    let ``measures-dll-netfx`` () = singleTestBuildAndRun "core/measures" FSC_NETFX_AS_DLL

    // Requires winforms will not run on coreclr
    [<Test>]
    let ``members-basics-script-netfx`` () = singleTestBuildAndRun "core/members/basics" FSI_NETFX_SCRIPT

    // Requires winforms will not run on coreclr
    [<Test>]
    let ``members-basics-netfx`` () = singleTestBuildAndRun "core/members/basics" FSC_NETFX

    // Requires winforms will not run on coreclr
    [<Test>]
    let ``members-basics-dll-netfx`` () = singleTestBuildAndRun "core/members/basics" FSC_NETFX_AS_DLL

    // Requires winforms will not run on coreclr
    [<Test>]
    let ``members-basics-hw-netfx`` () = singleTestBuildAndRun "core/members/basics-hw" FSC_NETFX

    // Requires winforms will not run on coreclr
    [<Test>]
    let ``members-basics-hw-mutrec-netfx`` () = singleTestBuildAndRun "core/members/basics-hw-mutrec" FSC_NETFX

    [<Test>]
    let ``members-incremental-netfx`` () = singleTestBuildAndRun "core/members/incremental" FSC_NETFX

    [<Test>]
    let ``members-incremental-hw-netfx`` () = singleTestBuildAndRun "core/members/incremental-hw" FSC_NETFX

    [<Test>]
    let ``members-incremental-hw-mutrec-netfx`` () = singleTestBuildAndRun "core/members/incremental-hw-mutrec" FSC_NETFX

    [<Test>]
    let ``pinvoke-netfx`` () = 
        let cfg = testConfig "core/pinvoke"

        fsc cfg "%s -o:test.exe -g" cfg.fsc_flags ["test.fsx"]
   
        peverifyWithArgs cfg "/nologo /MD" "test.exe"
                
    [<Test>]
    let ``fsi_load-netfx`` () = 
        let cfg = testConfig "core/fsi-load"

        use testOkFile = fileguard cfg "test.ok"

        fsi cfg "%s" cfg.fsi_flags ["test.fsx"]

        testOkFile.CheckExists()
                
    [<Test>]
    let ``queriesLeafExpressionConvert-netfx`` () = 
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
    let ``queriesNullableOperators-netfx`` () = 
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
    let ``queriesOverIEnumerable-netfx`` () = 
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
    let ``queriesOverIQueryable-netfx`` () = 
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
    let ``quotesDebugInfo-netfx`` () = 
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
    let ``quotesInMultipleModules-netfx`` () = 
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

    [<Test>]
    let ``refnormalization-netfx`` () = 
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
    let ``testResources-netfx`` () = 
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
    let ``topinit-netfx`` () = 
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
    let ``unitsOfMeasure-netfx`` () = 
        let cfg = testConfig "core/unitsOfMeasure"

        fsc cfg "%s --optimize- -o:test.exe -g" cfg.fsc_flags ["test.fs"]

        peverify cfg "test.exe"

        use testOkFile = fileguard cfg "test.ok"

        exec cfg ("." ++ "test.exe") ""

        testOkFile.CheckExists()
                
    [<Test>]
    let ``verify-netfx`` () = 
        let cfg = testConfig "core/verify"

        peverifyWithArgs cfg "/nologo" cfg.FSharpBuild

       // peverifyWithArgs cfg "/nologo /MD" (getDirectoryName(cfg.FSC) ++ "FSharp.Compiler.dll")

        peverifyWithArgs cfg "/nologo" cfg.FSI

        peverifyWithArgs cfg "/nologo" cfg.FSharpCompilerInteractiveSettings

        fsc cfg "%s -o:xmlverify.exe -g" cfg.fsc_flags ["xmlverify.fs"]

        peverifyWithArgs cfg "/nologo" "xmlverify.exe"

    // This test is disabled in coreclr builds dependent on fixing : https://github.com/Microsoft/visualfsharp/issues/2600
    [<Test>]
    let ``bundle-netfx`` () = 
        let cfg = testConfig "tools/bundle"

        fsc cfg "%s --progress --standalone -o:test-one-fsharp-module.exe -g" cfg.fsc_flags ["test-one-fsharp-module.fs"]
   
        peverify cfg "test-one-fsharp-module.exe"
   
        fsc cfg "%s -a -o:test_two_fsharp_modules_module_1.dll -g" cfg.fsc_flags ["test_two_fsharp_modules_module_1.fs"]
   
        peverify cfg "test_two_fsharp_modules_module_1.dll"
   
        fsc cfg "%s --standalone -r:test_two_fsharp_modules_module_1.dll -o:test_two_fsharp_modules_module_2.exe -g" cfg.fsc_flags ["test_two_fsharp_modules_module_2.fs"]
   
        peverify cfg "test_two_fsharp_modules_module_2.exe"
   
        fsc cfg "%s -a --standalone -r:test_two_fsharp_modules_module_1.dll -o:test_two_fsharp_modules_module_2_as_dll.dll -g" cfg.fsc_flags ["test_two_fsharp_modules_module_2.fs"]
   
        peverify cfg "test_two_fsharp_modules_module_2_as_dll.dll"

    [<Test>]
    let ``eval-netfx`` () = singleTestBuildAndRun "tools/eval" FSC_NETFX

    [<Test>]
    let ``SRTP doesn't handle calling member hiding hinherited members-netfx`` () =
        let cfg = testConfig "regression/5531" 

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

    // This test is disabled in coreclr builds dependent on fixing : https://github.com/Microsoft/visualfsharp/issues/2600
    [<Test>]
    let ``regression-655-netfx`` () = 
        let cfg = testConfig "regression/655"

        fsc cfg "%s -a -o:pack.dll" cfg.fsc_flags ["xlibC.ml"]

        peverify cfg "pack.dll"

        fsc cfg "%s    -o:test.exe -r:pack.dll" cfg.fsc_flags ["main.fs"]

        peverify cfg "test.exe"

        use testOkFile = fileguard cfg "test.ok"

        exec cfg ("." ++ "test.exe") ""

        testOkFile.CheckExists()
                
    // This test is disabled in coreclr builds dependent on fixing : https://github.com/Microsoft/visualfsharp/issues/2600
    [<Test >]
    let ``regression-656-netfx`` () = 
        let cfg = testConfig "regression/656"

        fsc cfg "%s -o:pack.exe" cfg.fsc_flags ["misc.fs mathhelper.fs filehelper.fs formshelper.fs plot.fs traj.fs playerrecord.fs trackedplayers.fs form.fs"]

        peverify cfg  "pack.exe"

    // Requires WinForms
    [<Test>]
    let ``regression-83-netfx`` () = singleTestBuildAndRun "regression/83" FSC_NETFX

    [<Test >]
    let ``regressiono-84-netfx`` () = singleTestBuildAndRun "regression/84" FSC_NETFX

    [<Test >]
    let ``regression-85-netfx`` () = 
        let cfg = testConfig "regression/85"

        fsc cfg "%s -r:Category.dll -a -o:petshop.dll" cfg.fsc_flags ["Category.ml"]

        peverify cfg "petshop.dll"

    // This test is disabled in coreclr builds dependent on fixing : https://github.com/Microsoft/visualfsharp/issues/2600
    [<Test>]
    let ``regression-struct-measure-bug-1-netfx`` () = 
        let cfg = testConfig "regression/struct-measure-bug-1"

        fsc cfg "%s --optimize- -o:test.exe -g" cfg.fsc_flags ["test.fs"]

        peverify cfg "test.exe"

    [<Test>]
    let ``optimize-functionSizes-netfx`` () = 
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
    let ``optimize-totalSizes-netfx`` () = 
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
    let ``optimize-hasEffect-netfx`` () = 
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
    let ``optimize-noNeedToTailcall-netfx`` () = 
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
    let ``optimize-inline-netfx`` () = 
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
    let ``optimize-stats-netfx`` () = 
        let cfg = testConfig "optimize/stats"

        ildasm cfg "/out=FSharp.Core.il" cfg.FSCOREDLLPATH

        let fscore = File.ReadLines(getfullpath cfg "FSharp.Core.il") |> Seq.toList

        let contains (text: string) (s: string) = if s.Contains(text) then 1 else 0

        let typeFunc = fscore |> List.sumBy (contains "extends Microsoft.FSharp.TypeFunc")
        let classes = fscore |> List.sumBy (contains ".class")
        let methods = fscore |> List.sumBy (contains ".method")
        let fields = fscore |> List.sumBy (contains ".field")

        let date = DateTime.Today.ToString("dd/MM/yyyy") // 23/11/2006
        let time = DateTime.Now.ToString("HH:mm:ss.ff") // 16:03:23.40
        let m = sprintf "%s, %s, Microsoft.FSharp-TypeFunc, %d, Microsoft.FSharp-classes, %d,  Microsoft.FSharp-methods, %d, ,  Microsoft.FSharp-fields, %d,  " date time typeFunc classes methods fields

        log "now:"
        log "%s" m

[<Category("netfx")>]
module TypecheckTests = 

    [<Test>]
    let ``full-rank-arrays-netfx`` () = 
        let cfg = testConfig "typecheck/full-rank-arrays"
        SingleTest.singleTestBuildAndRunWithExtraRef cfg "full-rank-arrays.dll" FSC_NETFX

    // Converter is not coming back until dotnet standard 2.0
    [<Test>]
    let ``misc-netfx`` () = singleTestBuildAndRun "typecheck/misc" FSC_NETFX

    [<Test>]
    let ``sigs pos26-netfx`` () = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos26.exe" cfg.fsc_flags ["pos26.fsi"; "pos26.fs"]
        peverify cfg "pos26.exe"

    [<Test>]
    let ``sigs pos25-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos25.exe" cfg.fsc_flags ["pos25.fs"]
        peverify cfg "pos25.exe"

    [<Test>]
    let ``sigs pos27-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos27.exe" cfg.fsc_flags ["pos27.fs"]
        peverify cfg "pos27.exe"

    [<Test>]
    let ``sigs pos28-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos28.exe" cfg.fsc_flags ["pos28.fs"]
        peverify cfg "pos28.exe"

    [<Test>]
    let ``sigs pos29-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos29.exe" cfg.fsc_flags ["pos29.fsi"; "pos29.fs"; "pos29.app.fs"]
        peverify cfg "pos29.exe"

    [<Test>]
    let ``sigs pos30-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos30.exe --warnaserror+" cfg.fsc_flags ["pos30.fs"]
        peverify cfg "pos30.exe"

    [<Test>]
    let ``sigs pos24-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos24.exe" cfg.fsc_flags ["pos24.fs"]
        peverify cfg "pos24.exe"

    [<Test>]
    let ``sigs pos31-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos31.exe --warnaserror" cfg.fsc_flags ["pos31.fsi"; "pos31.fs"]
        peverify cfg "pos31.exe"

    [<Test>]
    let ``sigs pos32-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:library -o:pos32.dll --warnaserror" cfg.fsc_flags ["pos32.fs"]
        peverify cfg "pos32.dll"

    [<Test>]
    let ``sigs pos23-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos23.exe" cfg.fsc_flags ["pos23.fs"]
        peverify cfg "pos23.exe"
        exec cfg ("." ++ "pos23.exe") ""

    [<Test>]
    let ``sigs pos20-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos20.exe" cfg.fsc_flags ["pos20.fs"]
        peverify cfg "pos20.exe"
        exec cfg ("." ++ "pos20.exe") ""

    [<Test>]
    let ``sigs pos19-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos19.exe" cfg.fsc_flags ["pos19.fs"]
        peverify cfg "pos19.exe"
        exec cfg ("." ++ "pos19.exe") ""

    [<Test>]
    let ``sigs pos18-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos18.exe" cfg.fsc_flags ["pos18.fs"]
        peverify cfg "pos18.exe"
        exec cfg ("." ++ "pos18.exe") ""

    [<Test>]
    let ``sigs pos16-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos16.exe" cfg.fsc_flags ["pos16.fs"]
        peverify cfg "pos16.exe"
        exec cfg ("." ++ "pos16.exe") ""

    [<Test>]
    let ``sigs pos17-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos17.exe" cfg.fsc_flags ["pos17.fs"]
        peverify cfg "pos17.exe"
        exec cfg ("." ++ "pos17.exe") ""

    [<Test>]
    let ``sigs pos15-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos15.exe" cfg.fsc_flags ["pos15.fs"]
        peverify cfg "pos15.exe"
        exec cfg ("." ++ "pos15.exe") ""

    [<Test>]
    let ``sigs pos14-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos14.exe" cfg.fsc_flags ["pos14.fs"]
        peverify cfg "pos14.exe"
        exec cfg ("." ++ "pos14.exe") ""

    [<Test>]
    let ``sigs pos13-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s --target:exe -o:pos13.exe" cfg.fsc_flags ["pos13.fs"]
        peverify cfg "pos13.exe"
        exec cfg ("." ++ "pos13.exe") ""

    [<Test>]
    let ``sigs pos12 -netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s -a -o:pos12.dll" cfg.fsc_flags ["pos12.fs"]

    [<Test>]
    let ``sigs pos11-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s -a -o:pos11.dll" cfg.fsc_flags ["pos11.fs"]

    [<Test>]
    let ``sigs pos10-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s -a -o:pos10.dll" cfg.fsc_flags ["pos10.fs"]
        peverify cfg "pos10.dll"

    [<Test>]
    let ``sigs pos09-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s -a -o:pos09.dll" cfg.fsc_flags ["pos09.fs"]
        peverify cfg "pos09.dll"

    [<Test>]
    let ``sigs pos07-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s -a -o:pos07.dll" cfg.fsc_flags ["pos07.fs"]
        peverify cfg "pos07.dll"

    [<Test>]
    let ``sigs pos08-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s -a -o:pos08.dll" cfg.fsc_flags ["pos08.fs"]
        peverify cfg "pos08.dll"

    [<Test>]
    let ``sigs pos06-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s -a -o:pos06.dll" cfg.fsc_flags ["pos06.fs"]
        peverify cfg "pos06.dll"

    [<Test>]
    let ``sigs pos03-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s -a -o:pos03.dll" cfg.fsc_flags ["pos03.fs"]
        peverify cfg "pos03.dll"
        fsc cfg "%s -a -o:pos03a.dll" cfg.fsc_flags ["pos03a.fsi"; "pos03a.fs"]
        peverify cfg "pos03a.dll"

    [<Test>]
    let ``sigs pos01a-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s -a -o:pos01a.dll" cfg.fsc_flags ["pos01a.fsi"; "pos01a.fs"]
        peverify cfg "pos01a.dll"

    [<Test>]
    let ``sigs pos02-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s -a -o:pos02.dll" cfg.fsc_flags ["pos02.fs"]
        peverify cfg "pos02.dll"

    [<Test>]
    let ``sigs pos05-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        fsc cfg "%s -a -o:pos05.dll" cfg.fsc_flags ["pos05.fs"]

    [<Test>] 
    let ``type check neg01-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg01"

    [<Test>] 
    let ``type check neg02-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg02"

    [<Test>] 
    let ``type check neg03-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg03"

    [<Test>] 
    let ``type check neg04-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg04"

    [<Test>] 
    let ``type check neg05-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg05"

    [<Test>] 
    let ``type check neg06-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg06"

    [<Test>] 
    let ``type check neg06_a-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg06_a"

    [<Test>] 
    let ``type check neg06_b-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg06_b"

    [<Test>] 
    let ``type check neg07-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg07"

    [<Test>] 
    let ``type check neg08-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg08"

    [<Test>] 
    let ``type check neg09-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg09"

    [<Test>] 
    let ``type check neg10-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg10"

    [<Test>] 
    let ``type check neg10_a-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg10_a"

    [<Test>] 
    let ``type check neg11-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg11"

    [<Test>] 
    let ``type check neg12-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg12"

    [<Test>] 
    let ``type check neg13-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg13"

    [<Test>] 
    let ``type check neg14-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg14"

    [<Test>] 
    let ``type check neg15-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg15"

    [<Test>] 
    let ``type check neg16-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg16"

    [<Test>] 
    let ``type check neg17-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg17"

    [<Test>] 
    let ``type check neg18-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg18"

    [<Test>] 
    let ``type check neg19-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg19"

    [<Test>] 
    let ``type check neg20-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg20"

    [<Test>] 
    let ``type check neg21-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg21"

    [<Test>] 
    let ``type check neg22-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg22"

    [<Test>] 
    let ``type check neg23-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg23"

    [<Test>] 
    let ``type check neg24-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg24"

    [<Test>] 
    let ``type check neg25-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg25"

    [<Test>] 
    let ``type check neg26-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg26"

    [<Test>] 
    let ``type check neg27-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg27"

    [<Test>] 
    let ``type check neg28-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg28"

    [<Test>] 
    let ``type check neg29-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg29"

    [<Test>] 
    let ``type check neg30-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg30"

    [<Test>] 
    let ``type check neg31-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg31"

    [<Test>] 
    let ``type check neg32-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg32"

    [<Test>] 
    let ``type check neg33-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg33"

    [<Test>] 
    let ``type check neg34-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg34"

    [<Test>] 
    let ``type check neg35-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg35"

    [<Test>] 
    let ``type check neg36-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg36"

    [<Test>] 
    let ``type check neg37-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg37"

    [<Test>] 
    let ``type check neg37_a-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg37_a"

    [<Test>] 
    let ``type check neg38-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg38"

    [<Test>] 
    let ``type check neg39-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg39"

    [<Test>] 
    let ``type check neg40-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg40"

    [<Test>] 
    let ``type check neg41-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg41"

    [<Test>] 
    let ``type check neg42-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg42"

    [<Test>] 
    let ``type check neg43-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg43"

    [<Test>] 
    let ``type check neg44-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg44"

    [<Test>] 
    let ``type check neg45-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg45"

    [<Test>] 
    let ``type check neg46-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg46"

    [<Test>] 
    let ``type check neg47-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg47"

    [<Test>] 
    let ``type check neg48-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg48"

    [<Test>] 
    let ``type check neg49-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg49"

    [<Test>] 
    let ``type check neg50-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg50"

    [<Test>] 
    let ``type check neg51-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg51"

    [<Test>] 
    let ``type check neg52-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg52"

    [<Test>] 
    let ``type check neg53-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg53"

    [<Test>] 
    let ``type check neg54-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg54"

    [<Test>] 
    let ``type check neg55-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg55"

    [<Test>] 
    let ``type check neg56-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg56"

    [<Test>] 
    let ``type check neg56_a-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg56_a"

    [<Test>] 
    let ``type check neg56_b-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg56_b"

    [<Test>] 
    let ``type check neg57-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg57"

    [<Test>] 
    let ``type check neg58-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg58"

    [<Test>] 
    let ``type check neg59-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg59"

    [<Test>] 
    let ``type check neg60-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg60"

    [<Test>] 
    let ``type check neg61-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg61"

    [<Test>] 
    let ``type check neg62-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg62"

    [<Test>] 
    let ``type check neg63-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg63"

    [<Test>] 
    let ``type check neg64-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg64"

    [<Test>] 
    let ``type check neg65-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg65"

    [<Test>] 
    let ``type check neg66-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg66"

    [<Test>] 
    let ``type check neg67-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg67"

    [<Test>] 
    let ``type check neg68-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg68"

    [<Test>] 
    let ``type check neg69-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg69"

    [<Test>] 
    let ``type check neg70-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg70"

    [<Test>] 
    let ``type check neg71-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg71"

    [<Test>] 
    let ``type check neg72-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg72"

    [<Test>] 
    let ``type check neg73-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg73"

    [<Test>] 
    let ``type check neg74-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg74"

    [<Test>] 
    let ``type check neg75-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg75"

    [<Test>] 
    let ``type check neg76-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg76"

    [<Test>] 
    let ``type check neg77-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg77"

    [<Test>] 
    let ``type check neg78-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg78"

    [<Test>] 
    let ``type check neg79-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg79"

    [<Test>] 
    let ``type check neg80-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg80"

    [<Test>] 
    let ``type check neg81-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg81"

    [<Test>] 
    let ``type check neg82-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg82"

    [<Test>] 
    let ``type check neg83-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg83"

    [<Test>] 
    let ``type check neg84-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg84"

    [<Test>] 
    let ``type check neg85-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg85"

    [<Test>] 
    let ``type check neg86-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg86"

    [<Test>] 
    let ``type check neg87-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg87"

    [<Test>] 
    let ``type check neg88-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg88"

    [<Test>] 
    let ``type check neg89-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg89"

    [<Test>] 
    let ``type check neg90-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg90"

    [<Test>] 
    let ``type check neg91-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg91"

    [<Test>] 
    let ``type check neg92-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg92"

    [<Test>] 
    let ``type check neg93-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg93"

    [<Test>] 
    let ``type check neg94-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg94"

    [<Test>] 
    let ``type check neg95-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg95"

    [<Test>] 
    let ``type check neg96-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg96"

    [<Test>] 
    let ``type check neg97-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg97"

    [<Test>] 
    let ``type check neg98-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg98"

    [<Test>] 
    let ``type check neg99-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg99"

    [<Test>] 
    let ``type check neg100-netfx``() = 
        let cfg = testConfig "typecheck/sigs"
        let cfg = { cfg with fsc_flags = cfg.fsc_flags + " --warnon:3218" }
        singleNegTest cfg "neg100"

    [<Test>] 
    let ``type check neg101-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg101"

    [<Test>]
    let ``type check neg102-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg102"

    [<Test>]
    let ``type check neg103-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg103"

    [<Test>]
    let ``type check neg104-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg104"

    [<Test>]
    let ``type check neg106-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg106"

    [<Test>]
    let ``type check neg107-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg107"

    [<Test>]
    let ``type check neg108-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg108"

    [<Test>]
    let ``type check neg109-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg109"

    [<Test>]
    let ``type check neg110-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg110"

    [<Test>]
    let ``type check neg111-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg111"

    [<Test>] 
    let ``type check neg113-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg113"

    [<Test>] 
    let ``type check neg114-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg114"

    [<Test>] 
    let ``type check neg115-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg115"

    [<Test>] 
    let ``type check neg_anon_1-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg_anon_1"

    [<Test>] 
    let ``type check neg_anon_2-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg_anon_2"

    [<Test>] 
    let ``type check neg_issue_3752-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg_issue_3752"

    [<Test>] 
    let ``type check neg_byref_1-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg_byref_1"

    [<Test>] 
    let ``type check neg_byref_2-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg_byref_2"

    [<Test>] 
    let ``type check neg_byref_3-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg_byref_3"

    [<Test>] 
    let ``type check neg_byref_4-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg_byref_4"

    [<Test>] 
    let ``type check neg_byref_5-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg_byref_5"

    [<Test>] 
    let ``type check neg_byref_6-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg_byref_6"

    [<Test>] 
    let ``type check neg_byref_7-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg_byref_7"

    [<Test>] 
    let ``type check neg_byref_8-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg_byref_8"

    [<Test>] 
    let ``type check neg_byref_10-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg_byref_10"

    [<Test>] 
    let ``type check neg_byref_11-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg_byref_11"

    [<Test>] 
    let ``type check neg_byref_12-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg_byref_12"

    [<Test>] 
    let ``type check neg_byref_13-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg_byref_13"

    [<Test>] 
    let ``type check neg_byref_14-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg_byref_14"

    [<Test>] 
    let ``type check neg_byref_15-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg_byref_15"

    [<Test>] 
    let ``type check neg_byref_16-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg_byref_16"

    [<Test>] 
    let ``type check neg_byref_17-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg_byref_17"

    [<Test>] 
    let ``type check neg_byref_18-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg_byref_18"

    [<Test>] 
    let ``type check neg_byref_19-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg_byref_19"

    [<Test>] 
    let ``type check neg_byref_20-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg_byref_20"

    [<Test>] 
    let ``type check neg_byref_21-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg_byref_21"

    [<Test>] 
    let ``type check neg_byref_22-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg_byref_22"

    [<Test>] 
    let ``type check neg_byref_23-netfx``() = singleNegTest (testConfig "typecheck/sigs") "neg_byref_23"


[<Category("netfx")>]
module FscTests =                 
    [<Test>]
    let ``should be raised if AssemblyInformationalVersion has invalid version-netfx``() = 
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
    let ``should set file version info on generated file-netfx``() = 
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

[<Category("netfx")>]
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
    let ``should use correct fallback-netfx``() =
      
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

[<Category("netfx")>]
module GeneratedSignatureTests =

    let generatedSignatureTest dir =
        let cfg = testConfig dir
        use cleanup = cleanUpFSharpCore cfg

        let source1 = 
            ["test.ml"; "test.fs"; "test.fsx"] 
            |> List.rev
            |> List.tryFind (fileExists cfg)

        source1 |> Option.iter (fun from -> copy_y cfg from "tmptest.fs")

        log "Generated signature file..."
        fsc cfg "%s --sig:tmptest.fsi" cfg.fsc_flags ["tmptest.fs"]
        if File.Exists("FSharp.Core.dll") then log "found fsharp.core.dll after build" else log "found fsharp.core.dll after build"

        log "Compiling against generated signature file..."
        fsc cfg "%s -o:tmptest1.exe" cfg.fsc_flags ["tmptest.fsi";"tmptest.fs"]
        if File.Exists("FSharp.Core.dll") then log "found fsharp.core.dll after build" else log "found fsharp.core.dll after build"

        log "Verifying built .exe..."
        peverify cfg "tmptest1.exe"

    [<Test; Ignore("incorrect signature file generated, test has been disabled a long time")>]
    let ``libtest-GENERATED_SIGNATURE`` () = generatedSignatureTest "core/libtest"

    [<Test>]
    let ``members-basics-GENERATED_SIGNATURE-netfx``() = generatedSignatureTest "core/members/basics"

    [<Test; Ignore("Flaky w.r.t. PEVerify.  https://github.com/Microsoft/visualfsharp/issues/2616")>]
    let ``access-GENERATED_SIGNATURE-netfx``() = generatedSignatureTest "core/access"

    [<Test>]
    let ``array-GENERATED_SIGNATURE-netfx``() = generatedSignatureTest "core/array"

    [<Test; Ignore("incorrect signature file generated, test has been disabled a long time")>]
    let ``genericmeasures-GENERATED_SIGNATURE-netfx``() = generatedSignatureTest "core/genericmeasures"

    [<Test>]
    let ``innerpoly-GENERATED_SIGNATURE-netfx``() = generatedSignatureTest "core/innerpoly"

    [<Test>]
    let ``measures-GENERATED_SIGNATURE-netfx``() = generatedSignatureTest "core/measures"

[<Category("netfx")>]
module TypeProviderTests = 
    [<Test>]
    let ``diamondAssembly-netfx`` () = 
        let cfg = testConfig "typeProviders/diamondAssembly"

        rm cfg "provider.dll"

        // Add a version flag to make this generate native resources. The native resources aren't important and 
        // can be dropped when the provided.dll is linked but we need to tolerate generated DLLs that have them
        fsc cfg "%s" "--out:provided.dll -a --version:0.0.0.1" [".." ++ "helloWorld" ++ "provided.fs"]

        fsc cfg "%s" "--out:provider.dll -a" [".." ++ "helloWorld" ++ "provider.fsx"]

        fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test1.dll -a" cfg.fsc_flags ["test1.fsx"]

        fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2a.dll -a -r:test1.dll" cfg.fsc_flags ["test2a.fsx"]

        fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2b.dll -a -r:test1.dll" cfg.fsc_flags ["test2b.fsx"]

        fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test3.exe -r:test1.dll -r:test2a.dll -r:test2b.dll" cfg.fsc_flags ["test3.fsx"]

        peverify cfg "test1.dll"

        peverify cfg "test2a.dll"

        peverify cfg "test2b.dll"

        peverify cfg "test3.exe"

        exec cfg ("." ++ "test3.exe") ""

        use testOkFile = fileguard cfg "test.ok"

        fsi cfg "%s" cfg.fsi_flags ["test3.fsx"]

        testOkFile.CheckExists()
                
    [<Test>]
    let ``globalNamespace-netfx`` () = 
        let cfg = testConfig "typeProviders/globalNamespace"

        csc cfg """/out:globalNamespaceTP.dll /debug+ /target:library /r:"%s" """ cfg.FSCOREDLLPATH ["globalNamespaceTP.cs"]

        fsc cfg "%s /debug+ /r:globalNamespaceTP.dll /optimize-" cfg.fsc_flags ["test.fsx"]
                
    let helloWorld p = 
        let cfg = testConfig "typeProviders/helloWorld"

        fsc cfg "%s" "--out:provided1.dll -g -a" [".." ++ "helloWorld" ++ "provided.fs"]

        fsc cfg "%s" "--out:provided2.dll -g -a" [".." ++ "helloWorld" ++ "provided.fs"]

        fsc cfg "%s" "--out:provided3.dll -g -a" [".." ++ "helloWorld" ++ "provided.fs"]

        fsc cfg "%s" "--out:provided4.dll -g -a" [".." ++ "helloWorld" ++ "provided.fs"]

        fsc cfg "%s" "--out:providedJ.dll -g -a" [".." ++ "helloWorld" ++ "providedJ.fs"]

        fsc cfg "%s" "--out:providedK.dll -g -a" [".." ++ "helloWorld" ++ "providedK.fs"]

        fsc cfg "%s" "--out:providedNullAssemblyName.dll -g -a" [".." ++ "helloWorld" ++ "providedNullAssemblyName.fsx"]

        fsc cfg "--out:provided.dll -a" [".." ++ "helloWorld" ++ "provided.fs"]

        fsc cfg "--out:providedJ.dll -a" [".." ++ "helloWorld" ++ "providedJ.fs"]

        fsc cfg "--out:providedK.dll -a" [".." ++ "helloWorld" ++ "providedK.fs"]

        fsc cfg "--out:provider.dll -a" ["provider.fsx"]

        SingleTest.singleTestBuildAndRunAux cfg p 


        rm cfg "provider_with_binary_compat_changes.dll"

        mkdir cfg "bincompat1"

        log "pushd bincompat1"
        let bincompat1 = getfullpath cfg "bincompat1"

        Directory.EnumerateFiles(bincompat1 ++ "..", "*.dll")
        |> Seq.iter (fun from -> Commands.copy_y bincompat1 from ("." ++ Path.GetFileName(from)) |> ignore)

        fscIn cfg bincompat1 "%s" "-g -a -o:test_lib.dll -r:provider.dll" [".." ++ "test.fsx"]

        fscIn cfg bincompat1 "%s" "-r:test_lib.dll -r:provider.dll" [".." ++ "testlib_client.fsx"]

        log "popd"

        mkdir cfg "bincompat2"
        
        log "pushd bincompat2"
        let bincompat2 = getfullpath cfg "bincompat2"

        Directory.EnumerateFiles(bincompat2 ++ ".." ++ "bincompat1", "*.dll")
        |> Seq.iter (fun from -> Commands.copy_y bincompat2 from ("." ++ Path.GetFileName(from)) |> ignore)

        fscIn cfg bincompat2 "%s" "--define:ADD_AN_OPTIONAL_STATIC_PARAMETER --define:USE_IMPLICIT_ITypeProvider2 --out:provider.dll -g -a" [".." ++ "provider.fsx"]

        fscIn cfg bincompat2 "-g -a -o:test_lib_recompiled.dll -r:provider.dll" [".." ++ "test.fsx"]

        fscIn cfg bincompat2 "%s" "--define:ADD_AN_OPTIONAL_STATIC_PARAMETER -r:test_lib.dll -r:provider.dll" [".." ++ "testlib_client.fsx"]

        peverify cfg (bincompat2 ++ "provider.dll")

        peverify cfg (bincompat2 ++ "test_lib.dll")

        peverify cfg (bincompat2 ++ "test_lib_recompiled.dll")

        peverify cfg (bincompat2 ++ "testlib_client.exe")

    [<Test>]
    let ``helloWorld fsc-netfx`` () = helloWorld FSC_NETFX

    [<Test>]
    let ``helloWorld fsi-netfx`` () = helloWorld FSI_NETFX_SCRIPT_STDIN


    [<Test>]
    let ``helloWorldCSharp-netfx`` () = 
        let cfg = testConfig "typeProviders/helloWorldCSharp"

        rm cfg "magic.dll"

        fsc cfg "%s" "--out:magic.dll -a --keyfile:magic.snk" ["magic.fs "]

        rm cfg "provider.dll"

        csc cfg """/out:provider.dll /target:library "/r:%s" /r:magic.dll""" cfg.FSCOREDLLPATH ["provider.cs"]

        fsc cfg "%s /debug+ /r:provider.dll /optimize-" cfg.fsc_flags ["test.fsx"]

        peverify cfg "magic.dll"

        peverify cfg "provider.dll"

        peverify cfg "test.exe"

        exec cfg ("." ++ "test.exe") ""
                

    let negTypeProviderTest (name:string) =
        let cfg = testConfig "typeProviders/negTests"
        let dir = cfg.Directory

        if requireENCulture () then

            let fileExists = Commands.fileExists dir >> Option.isSome

            rm cfg "provided.dll"

            fsc cfg "--out:provided.dll -a" [".." ++ "helloWorld" ++ "provided.fs"]

            rm cfg "providedJ.dll"

            fsc cfg "--out:providedJ.dll -a" [".." ++ "helloWorld" ++ "providedJ.fs"]

            rm cfg "providedK.dll"

            fsc cfg "--out:providedK.dll -a" [".." ++ "helloWorld" ++ "providedK.fs"]

            rm cfg "provider.dll"

            fsc cfg "--out:provider.dll -a" ["provider.fsx"]

            fsc cfg "--out:provider_providerAttributeErrorConsume.dll -a" ["providerAttributeError.fsx"]

            fsc cfg "--out:provider_ProviderAttribute_EmptyConsume.dll -a" ["providerAttribute_Empty.fsx"]

            rm cfg "helloWorldProvider.dll"

            fsc cfg "--out:helloWorldProvider.dll -a" [".." ++ "helloWorld" ++ "provider.fsx"]

            rm cfg "MostBasicProvider.dll"

            fsc cfg "--out:MostBasicProvider.dll -a" ["MostBasicProvider.fsx"]

            let preprocess name pref = 
                let dirp = (dir |> Commands.pathAddBackslash)
                do
                File.ReadAllText(sprintf "%s%s.%sbslpp" dirp name pref)
                    .Replace("<ASSEMBLY>", getfullpath cfg (sprintf "provider_%s.dll" name))
                    .Replace("<URIPATH>",sprintf "file:///%s" dirp)
                    |> fun txt -> File.WriteAllText(sprintf "%s%s.%sbsl" dirp name pref,txt)

            if name = "ProviderAttribute_EmptyConsume" || name = "providerAttributeErrorConsume" then ()
            else fsc cfg "--define:%s --out:provider_%s.dll -a" name name ["provider.fsx"]

            if fileExists (sprintf "%s.bslpp" name) then preprocess name "" 

            if fileExists (sprintf "%s.vsbslpp" name) then preprocess name "vs"

            SingleTest.singleNegTest cfg name

    [<Test>]
    let ``neg-type-provider-test-neg1``() = negTypeProviderTest "neg1"

    [<Test>]
    let ``neg-type-provider-test-neg2``() = negTypeProviderTest "neg2"

    [<Test>]
    let ``neg-type-provider-test-neg2c``() = negTypeProviderTest "neg2c"

    [<Test>]
    let ``neg-type-provider-test-neg2e``() = negTypeProviderTest "neg2e"

    [<Test>]
    let ``neg-type-provider-test-neg2g``() = negTypeProviderTest "neg2g"

    [<Test>]
    let ``neg-type-provider-test-neg2h``() = negTypeProviderTest "neg2h"    

    [<Test>]
    let ``neg-type-provider-test-neg4``() = negTypeProviderTest "neg4"

    [<Test>]
    let ``neg-type-provider-test-neg6``() = negTypeProviderTest "neg6"

    [<Test>]
    let ``neg-type-provider-test-InvalidInvokerExpression``() = negTypeProviderTest "InvalidInvokerExpression"

    [<Test>]
    let ``neg-type-provider-test-providerAttributeErrorConsume``() = negTypeProviderTest "providerAttributeErrorConsume"

    [<Test>]
    let ``neg-type-provider-test-ProviderAttribute_EmptyConsume``() = negTypeProviderTest "ProviderAttribute_EmptyConsume"

    [<Test>]
    let ``neg-type-provider-test-EVIL_PROVIDER_GetNestedNamespaces_Exception``() = negTypeProviderTest "EVIL_PROVIDER_GetNestedNamespaces_Exception"

    [<Test>]
    let ``neg-type-provider-test-EVIL_PROVIDER_NamespaceName_Exception``() = negTypeProviderTest "EVIL_PROVIDER_NamespaceName_Exception"

    [<Test>]
    let ``neg-type-provider-test-EVIL_PROVIDER_NamespaceName_Empty``() = negTypeProviderTest "EVIL_PROVIDER_NamespaceName_Empty"

    [<Test>]
    let ``neg-type-provider-test-EVIL_PROVIDER_GetTypes_Exception``() = negTypeProviderTest "EVIL_PROVIDER_GetTypes_Exception"

    [<Test>]
    let ``neg-type-provider-test-EVIL_PROVIDER_ResolveTypeName_Exception``() = negTypeProviderTest "EVIL_PROVIDER_ResolveTypeName_Exception"

    [<Test>]
    let ``neg-type-provider-test-EVIL_PROVIDER_GetNamespaces_Exception``() = negTypeProviderTest "EVIL_PROVIDER_GetNamespaces_Exception"

    [<Test>]
    let ``neg-type-provider-test-EVIL_PROVIDER_GetStaticParameters_Exception``() = negTypeProviderTest "EVIL_PROVIDER_GetStaticParameters_Exception"

    [<Test>]
    let ``neg-type-provider-test-EVIL_PROVIDER_GetInvokerExpression_Exception``() = negTypeProviderTest "EVIL_PROVIDER_GetInvokerExpression_Exception"

    [<Test>]
    let ``neg-type-provider-test-EVIL_PROVIDER_GetTypes_Null``() = negTypeProviderTest "EVIL_PROVIDER_GetTypes_Null"

    [<Test>]
    let ``neg-type-provider-test-EVIL_PROVIDER_ResolveTypeName_Null``() = negTypeProviderTest "EVIL_PROVIDER_ResolveTypeName_Null"

    [<Test>]
    let ``neg-type-provider-test-EVIL_PROVIDER_GetNamespaces_Null``() = negTypeProviderTest "EVIL_PROVIDER_GetNamespaces_Null"

    [<Test>]
    let ``neg-type-provider-test-EVIL_PROVIDER_GetStaticParameters_Null``() = negTypeProviderTest "EVIL_PROVIDER_GetStaticParameters_Null"

    [<Test>]
    let ``neg-type-provider-test-EVIL_PROVIDER_GetInvokerExpression_Null``() = negTypeProviderTest "EVIL_PROVIDER_GetInvokerExpression_Null"

    [<Test>]
    let ``neg-type-provider-test-EVIL_PROVIDER_DoesNotHaveConstructor``() = negTypeProviderTest "EVIL_PROVIDER_DoesNotHaveConstructor"

    [<Test>]
    let ``neg-type-provider-test-EVIL_PROVIDER_ConstructorThrows``() = negTypeProviderTest "EVIL_PROVIDER_ConstructorThrows"

    [<Test>]
    let ``neg-type-provider-test-EVIL_PROVIDER_ReturnsTypeWithIncorrectNameFromApplyStaticArguments``() = negTypeProviderTest "EVIL_PROVIDER_ReturnsTypeWithIncorrectNameFromApplyStaticArguments"

    let splitAssemblyTest subdir project =

        let cfg = testConfig project

        let clean() = 
            rm cfg "providerDesigner.dll"
            rmdir cfg "typeproviders"
            rmdir cfg "tools"
            rmdir cfg (".." ++ "typeproviders")
            rmdir cfg (".." ++ "tools")

        clean()

        fsc cfg "--out:provider.dll -a" ["provider.fs"]

        fsc cfg "--out:providerDesigner.dll -a" ["providerDesigner.fsx"]

        begin
            use testOk = fileguard cfg "test.ok"
            fsc cfg "" ["test.fsx"]
            exec cfg ("." ++ "test.exe") ""
            testOk.CheckExists()
        end

        begin
            use testOk = fileguard cfg "test.ok"
            fsi cfg "" ["test.fsx"]
            testOk.CheckExists()
        end

        begin
            use testOk = fileguard cfg "test.ok"
            fsiAnyCpu cfg "" ["test.fsx"]
            testOk.CheckExists()
        end

        // Do the same thing with different load locations for the type provider design-time component

        clean()

        // check a few load locations
        let someLoadPaths = 
            [ subdir ++ "fsharp41" ++ "net461"
              subdir ++ "fsharp41" ++ "net45"
              // include up one directory
              ".." ++ subdir ++ "fsharp41" ++ "net45"
              subdir ++ "fsharp41" ++ "netstandard2.0" ]

        for dir in someLoadPaths do

            clean()

            // put providerDesigner.dll into a different place
            mkdir cfg dir
            fsc cfg "--out:%s/providerDesigner.dll -a" dir ["providerDesigner.fsx"]

            begin
                use testOk = fileguard cfg "test.ok"
                fsc cfg "" ["test.fsx"]
                exec cfg ("." ++ "test.exe") ""
                testOk.CheckExists()
            end

        for dir in someLoadPaths do

            clean()

            // put providerDesigner.dll into a different place
            mkdir cfg dir
            fsc cfg "--out:%s/providerDesigner.dll -a" dir ["providerDesigner.fsx"]

            begin
                use testOk = fileguard cfg "test.ok"
                fsi cfg "" ["test.fsx"]
                testOk.CheckExists()
            end

        clean()

    [<Test>]
    let splitAssemblyTools () = splitAssemblyTest "tools" "typeProviders/splitAssemblyTools"

    [<Test>]
    let splitAssemblyTypeProviders () = splitAssemblyTest "typeproviders" "typeProviders/splitAssemblyTypeproviders"

    [<Test>]
    let wedgeAssembly () = 
        let cfg = testConfig "typeProviders/wedgeAssembly"

        rm cfg "provider.dll"

        rm cfg "provided.dll"

        fsc cfg "%s" "--out:provided.dll -a" [".." ++ "helloWorld" ++ "provided.fs"]

        rm cfg "providedJ.dll"

        fsc cfg "%s" "--out:providedJ.dll -a" [".." ++ "helloWorld" ++ "providedJ.fs"]

        rm cfg "providedK.dll"

        fsc cfg "%s" "--out:providedK.dll -a" [".." ++ "helloWorld" ++ "providedK.fs"]

        fsc cfg "%s" "--out:provider.dll -a" [".." ++ "helloWorld" ++ "provider.fsx"]

        fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2a.dll -a" cfg.fsc_flags ["test2a.fs"]

        fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2b.dll -a" cfg.fsc_flags ["test2b.fs"]

        fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test3.exe" cfg.fsc_flags ["test3.fsx"]

        fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2a-with-sig.dll -a" cfg.fsc_flags ["test2a.fsi"; "test2a.fs"]

        fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2b-with-sig.dll -a" cfg.fsc_flags ["test2b.fsi"; "test2b.fs"]

        fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test3-with-sig.exe --define:SIGS" cfg.fsc_flags ["test3.fsx"]

        fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2a-with-sig-restricted.dll -a" cfg.fsc_flags ["test2a-restricted.fsi"; "test2a.fs"]

        fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test2b-with-sig-restricted.dll -a"cfg.fsc_flags ["test2b-restricted.fsi"; "test2b.fs"]

        fsc cfg "%s --debug+ -r:provider.dll --optimize- -o:test3-with-sig-restricted.exe --define:SIGS_RESTRICTED" cfg.fsc_flags ["test3.fsx"]

        peverify cfg "test2a.dll"

        peverify cfg "test2b.dll"

        peverify cfg "test3.exe"

        exec cfg ("." ++ "test3.exe") ""
#endif
