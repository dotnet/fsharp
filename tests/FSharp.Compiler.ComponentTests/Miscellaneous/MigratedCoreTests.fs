// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.ComponentTests.Miscellaneous.FsharpSuiteMigrated.CoreTests

open Xunit
open FSharp.Test
open FSharp.Test.ScriptHelpers 
open TestFrameworkAdapter    
      

// These tests are enabled for .NET Framework and .NET Core
[<Fact>]
let ``access-FSC_DEBUG``() = singleTestBuildAndRun "core/access" FSC_DEBUG

[<Fact>]
let ``access-FSC_OPTIMIZED``() = singleTestBuildAndRun "core/access" FSC_OPTIMIZED

[<Fact>]
let ``access-FSI``() = singleTestBuildAndRun "core/access" FSI

[<Fact>]
let ``apporder-FSC_DEBUG`` () = singleTestBuildAndRun "core/apporder" FSC_DEBUG

[<Fact>]
let ``apporder-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/apporder" FSC_OPTIMIZED

[<Fact>]
let ``apporder-FSI`` () = singleTestBuildAndRun "core/apporder" FSI

[<Fact>]
let ``array-FSC_DEBUG-5.0`` () = singleTestBuildAndRunVersion "core/array" FSC_DEBUG LangVersion.V50

[<Fact>]
let ``array-FSC_OPTIMIZED-5.0`` () = singleTestBuildAndRunVersion "core/array" FSC_OPTIMIZED LangVersion.V50

[<Fact>]
let ``array-FSI-5.0`` () = singleTestBuildAndRunVersion "core/array" FSI LangVersion.V50

[<Fact>]
let ``array-FSC_OPTIMIZED-preview`` () = singleTestBuildAndRunVersion "core/array" FSC_OPTIMIZED LangVersion.Preview

[<Fact>]
let ``array-no-dot-FSC_DEBUG`` () = singleTestBuildAndRunVersion "core/array-no-dot" FSC_DEBUG LangVersion.Preview

[<Fact>]
let ``array-no-dot-FSC_OPTIMIZED`` () = singleTestBuildAndRunVersion "core/array-no-dot" FSC_OPTIMIZED LangVersion.Preview

[<Fact>]
let ``array-no-dot-FSI`` () = singleTestBuildAndRunVersion "core/array-no-dot" FSI LangVersion.Preview

[<Fact>]
let ``array-no-dot-warnings-langversion-default`` () =     
    singleVersionedNegTest "core/array-no-dot-warnings" LangVersion.Latest "test-langversion-default"

[<Fact>]
let ``array-no-dot-warnings-langversion-5_0`` () =       
    singleVersionedNegTest "core/array-no-dot-warnings" LangVersion.V50 "test-langversion-5.0"

[<Fact>]
let ``ref-ops-deprecation-langversion-preview`` () =
    singleVersionedNegTest "core/ref-ops-deprecation" LangVersion.Preview "test-langversion-preview"

[<Fact>]
let ``auto-widen-version-5_0``() = 
    singleVersionedNegTest "core/auto-widen/5.0" LangVersion.V50 "test"

[<Fact>]
let ``auto-widen-version-FSC_DEBUG-preview``() =
    singleTestBuildAndRunVersion "core/auto-widen/preview" FSC_DEBUG LangVersion.Preview

[<Fact>]
let ``auto-widen-version-FSC_OPTIMIZED-preview``() =
    singleTestBuildAndRunVersion "core/auto-widen/preview" FSC_OPTIMIZED LangVersion.Preview

[<Fact>]
let ``auto-widen-minimal``() =
    singleTestBuildAndRunVersion "core/auto-widen/minimal" FSC_OPTIMIZED LangVersion.V70

[<Fact>]
let ``auto-widen-version-preview-warns-on``() = 
    singleVersionedNegTestAux "core/auto-widen/preview" ["--warnon:3388";"--warnon:3389";"--warnon:3395";"--warnaserror+";"--define:NEGATIVE"] LangVersion.Preview "test"

[<Fact>]
let ``auto-widen-version-preview-default-warns``() = 
    singleVersionedNegTestAux "core/auto-widen/preview-default-warns" ["--warnaserror+";"--define:NEGATIVE"] LangVersion.Preview "test"

[<Fact>]
let ``comprehensions-FSC_DEBUG`` () = singleTestBuildAndRun "core/comprehensions" FSC_DEBUG

[<Fact>]
let ``comprehensions-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/comprehensions" FSC_OPTIMIZED

[<Fact>]
let ``comprehensions-FSI`` () = singleTestBuildAndRun "core/comprehensions" FSI

[<Fact>]
let ``comprehensionshw-FSC_DEBUG`` () = singleTestBuildAndRun "core/comprehensions-hw" FSC_DEBUG

[<Fact>]
let ``comprehensionshw-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/comprehensions-hw" FSC_OPTIMIZED

[<Fact>]
let ``comprehensionshw-FSI`` () = singleTestBuildAndRun "core/comprehensions-hw" FSI

[<Fact>]
let ``genericmeasures-FSC_DEBUG`` () = singleTestBuildAndRun "core/genericmeasures" FSC_DEBUG

[<Fact>]
let ``genericmeasures-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/genericmeasures" FSC_OPTIMIZED

[<Fact>]
let ``genericmeasures-FSI`` () = singleTestBuildAndRun "core/genericmeasures" FSI

[<Fact>]
let ``innerpoly-FSC_DEBUG`` () = singleTestBuildAndRun "core/innerpoly"  FSC_DEBUG

[<Fact>]
let ``innerpoly-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/innerpoly" FSC_OPTIMIZED

[<Fact>]
let ``innerpoly-FSI`` () = singleTestBuildAndRun "core/innerpoly" FSI

[<Fact>]
let ``namespaceAttributes-FSC_DEBUG`` () = singleTestBuildAndRun "core/namespaces" COMPILED_EXE_APP

[<Fact>]
let ``namespaceAttributes-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/namespaces" COMPILED_EXE_APP

[<Fact>]
let ``unicode2-FSC_DEBUG`` () = singleTestBuildAndRun "core/unicode" FSC_DEBUG // TODO: fails on coreclr

[<Fact>]
let ``unicode2-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/unicode" FSC_OPTIMIZED // TODO: fails on coreclr

[<Fact>]
let ``unicode2-FSI`` () = singleTestBuildAndRun "core/unicode" FSI

[<Fact>]
let ``lazy test-FSC_DEBUG`` () = singleTestBuildAndRun "core/lazy" FSC_DEBUG

[<Fact>]
let ``lazy test-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/lazy" FSC_OPTIMIZED

[<Fact>]
let ``lazy test-FSI`` () = singleTestBuildAndRun "core/lazy" FSI

[<Fact>]
let ``letrec-FSC_DEBUG`` () = singleTestBuildAndRun "core/letrec" FSC_DEBUG

[<Fact>]
let ``letrec-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/letrec" FSC_OPTIMIZED

[<Fact>]
let ``letrec-FSI`` () = singleTestBuildAndRun "core/letrec" FSI

[<Fact>]
let ``letrec (mutrec variations part one) FSC_DEBUG`` () = singleTestBuildAndRun "core/letrec-mutrec" FSC_DEBUG

[<Fact>]
let ``letrec (mutrec variations part one) FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/letrec-mutrec" FSC_OPTIMIZED

[<Fact>]
let ``letrec (mutrec variations part one) FSI`` () = singleTestBuildAndRun "core/letrec-mutrec" FSI

[<Fact>]
let ``libtest-FSC_DEBUG`` () = singleTestBuildAndRun "core/libtest" FSC_DEBUG

[<Fact>]
let ``libtest-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/libtest" FSC_OPTIMIZED

[<Fact>]
let ``libtest-FSI`` () = singleTestBuildAndRun "core/libtest" FSI

[<Fact>]
let ``lift-FSC_DEBUG`` () = singleTestBuildAndRun "core/lift" FSC_DEBUG

[<Fact>]
let ``lift-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/lift" FSC_OPTIMIZED

[<Fact>]
let ``lift-FSI`` () = singleTestBuildAndRun "core/lift" FSI

[<Fact>]
let ``map-FSC_DEBUG`` () = singleTestBuildAndRun "core/map" FSC_DEBUG

[<Fact>]
let ``map-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/map" FSC_OPTIMIZED

[<Fact>]
let ``map-FSI`` () = singleTestBuildAndRun "core/map" FSI

[<Fact>]
let ``measures-FSC_DEBUG`` () = singleTestBuildAndRun "core/measures" FSC_DEBUG

[<Fact>]
let ``measures-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/measures" FSC_OPTIMIZED

[<Fact>]
let ``measures-FSI`` () = singleTestBuildAndRun "core/measures" FSI

[<Fact>]
let ``nested-FSC_DEBUG`` () = singleTestBuildAndRun "core/nested" FSC_DEBUG

[<Fact>]
let ``nested-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/nested" FSC_OPTIMIZED

[<Fact>]
let ``nested-FSI`` () = singleTestBuildAndRun "core/nested" FSI

[<FactForDESKTOP>]
let ``members-basics-hw`` () = singleTestBuildAndRun "core/members/basics-hw" FSC_OPTIMIZED

[<FactForDESKTOP>]
let ``members-basics-hw-mutrec`` () = singleTestBuildAndRun "core/members/basics-hw-mutrec" FSC_OPTIMIZED

[<FactForDESKTOP>]
let ``members-incremental-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/members/incremental" FSC_OPTIMIZED

[<FactForDESKTOP>]
let ``members-incremental-FSI`` () = singleTestBuildAndRun "core/members/incremental" FSI

[<FactForDESKTOP>]
let ``members-incremental-hw-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/members/incremental-hw" FSC_OPTIMIZED

[<FactForDESKTOP>]
let ``members-incremental-hw-FSI`` () = singleTestBuildAndRun "core/members/incremental-hw" FSI

[<FactForDESKTOP>]
let ``members-incremental-hw-mutrec-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/members/incremental-hw-mutrec" FSC_OPTIMIZED

[<Fact>]
let ``members-ops-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/members/ops" FSC_OPTIMIZED

[<Fact>]
let ``members-ops-FSC_DEBUG`` () = singleTestBuildAndRun "core/members/ops" FSC_DEBUG

[<Fact>]
let ``members-ops-FSI`` () = singleTestBuildAndRun "core/members/ops" FSI

[<Fact>]
let ``members-ops-mutrec-FSC_DEBUG`` () = singleTestBuildAndRun "core/members/ops-mutrec" FSC_DEBUG

[<Fact>]
let ``members-ops-mutrec-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/members/ops-mutrec" FSC_OPTIMIZED

[<Fact>]
let ``members-ops-mutrec-FSI`` () = singleTestBuildAndRun "core/members/ops-mutrec" FSI

[<Fact>]
let ``seq-FSC_DEBUG`` () = singleTestBuildAndRun "core/seq" FSC_DEBUG

[<Fact>]
let ``seq-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/seq" FSC_OPTIMIZED

[<Fact>]
let ``seq-FSI`` () = singleTestBuildAndRun "core/seq" FSI

[<Fact>]
let ``math-numbers-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/math/numbers" FSC_OPTIMIZED

[<Fact>]
let ``math-numbers-FSI`` () = singleTestBuildAndRun "core/math/numbers" FSI

[<Fact>]
let ``members-ctree-FSC_DEBUG`` () = singleTestBuildAndRun "core/members/ctree" FSC_DEBUG

[<Fact>]
let ``members-ctree-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/members/ctree" FSC_OPTIMIZED

[<Fact>]
let ``members-ctree-FSI`` () = singleTestBuildAndRun "core/members/ctree" FSI

[<Fact>]
let ``members-factors-FSC_DEBUG`` () = singleTestBuildAndRun "core/members/factors" FSC_DEBUG

[<Fact>]
let ``members-factors-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/members/factors" FSC_OPTIMIZED

[<Fact>]
let ``members-factors-FSI`` () = singleTestBuildAndRun "core/members/factors" FSI

[<Fact>]
let ``members-factors-mutrec-FSC_DEBUG`` () = singleTestBuildAndRun "core/members/factors-mutrec" FSC_DEBUG

[<Fact>]
let ``members-factors-mutrec-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/members/factors-mutrec" FSC_OPTIMIZED

[<Fact>]
let ``members-factors-mutrec-FSI`` () = singleTestBuildAndRun "core/members/factors-mutrec" FSI

[<Fact>]
let ``graph-FSC_DEBUG`` () = singleTestBuildAndRunVersion "perf/graph" FSC_DEBUG LangVersion.SupportsMl

[<Fact>]
let ``graph-FSC_OPTIMIZED`` () = singleTestBuildAndRunVersion "perf/graph" FSC_OPTIMIZED LangVersion.SupportsMl

[<Fact>]
let ``graph-FSI`` () = singleTestBuildAndRunVersion "perf/graph" FSI LangVersion.SupportsMl

[<Fact>]
let ``nbody-FSC_DEBUG`` () = singleTestBuildAndRunVersion "perf/nbody" FSC_DEBUG LangVersion.SupportsMl

[<Fact>]
let ``nbody-FSC_OPTIMIZED`` () = singleTestBuildAndRunVersion "perf/nbody" FSC_OPTIMIZED LangVersion.SupportsMl

[<Fact>]
let ``nbody-FSI`` () = singleTestBuildAndRunVersion "perf/nbody" FSI LangVersion.SupportsMl

[<Fact>]
let ``forexpression-FSC_DEBUG`` () = singleTestBuildAndRun "core/forexpression" FSC_DEBUG

[<Fact>]
let ``forexpression-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/forexpression" FSC_OPTIMIZED

[<Fact>]
let ``forexpression-FSI`` () = singleTestBuildAndRun "core/forexpression" FSI

[<Fact>]
let ``letrec (mutrec variations part two) FSC_DEBUG`` () = singleTestBuildAndRun "core/letrec-mutrec2" FSC_DEBUG

[<Fact>]
let ``letrec (mutrec variations part two) FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/letrec-mutrec2" FSC_OPTIMIZED

[<Fact>]
let ``letrec (mutrec variations part two) FSI`` () = singleTestBuildAndRun "core/letrec-mutrec2" FSI

[<Fact>]
let ``printf`` () = singleTestBuildAndRunVersion "core/printf" FSC_OPTIMIZED LangVersion.Preview

[<Fact>]
let ``printf-interpolated`` () = singleTestBuildAndRunVersion "core/printf-interpolated" FSC_OPTIMIZED LangVersion.Preview

[<Fact>]
let ``tlr-FSC_DEBUG`` () = singleTestBuildAndRun "core/tlr" FSC_DEBUG

[<Fact>]
let ``tlr-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/tlr" FSC_OPTIMIZED

[<Fact>]
let ``tlr-FSI`` () = singleTestBuildAndRun "core/tlr" FSI

[<Fact>]
let ``subtype-FSC_DEBUG`` () = singleTestBuildAndRun "core/subtype" FSC_DEBUG

[<Fact>]
let ``subtype-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/subtype" FSC_OPTIMIZED

[<Fact>]
let ``subtype-FSI`` () = singleTestBuildAndRun "core/subtype" FSI

[<Fact>]
let ``syntax-FSC_DEBUG`` () = singleTestBuildAndRun "core/syntax" FSC_DEBUG

[<Fact>]
let ``syntax-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/syntax" FSC_OPTIMIZED

[<Fact>]
let ``syntax-FSI`` () = singleTestBuildAndRun "core/syntax" FSI

[<Fact>]
let ``test int32-FSC_DEBUG`` () = singleTestBuildAndRun "core/int32" FSC_DEBUG

[<Fact>]
let ``test int32-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/int32" FSC_OPTIMIZED

[<Fact>]
let ``test int32-FSI`` () = singleTestBuildAndRun "core/int32" FSI

// This test stays in FsharpSuite for desktop framework for a later migration phases, it uses hardcoded #r to a C# compiled cslib.dll inside
[<FactForNETCOREAPP>]
let ``quotes-FSC-FSC_DEBUG`` () = singleTestBuildAndRun "core/quotes" FSC_DEBUG

[<FactForNETCOREAPP>]
let ``quotes-FSC-BASIC`` () = singleTestBuildAndRun "core/quotes" FSC_OPTIMIZED

[<FactForNETCOREAPP>]
let ``quotes-FSI-BASIC`` () = singleTestBuildAndRun "core/quotes" FSI

[<Fact>]
let ``recordResolution-FSC_DEBUG`` () = singleTestBuildAndRun "core/recordResolution" FSC_DEBUG

[<Fact>]
let ``recordResolution-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/recordResolution" FSC_OPTIMIZED

[<Fact>]
let ``recordResolution-FSI`` () = singleTestBuildAndRun "core/recordResolution" FSI

// This test has hardcoded expectations about current synchronization context
// Will be moved out of FsharpSuite.Tests in a later phase for desktop framework
[<FactForNETCOREAPP>]
let ``control-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/control" FSC_OPTIMIZED

[<FactForNETCOREAPP>]
let ``control-FSI`` () = singleTestBuildAndRun "core/control" FSI

[<FactForNETCOREAPP>]
let ``control --tailcalls`` () =
    let cfg =  "core/control"
    singleTestBuildAndRunAux cfg  ["--tailcalls"] FSC_OPTIMIZED

[<Fact>]
let ``controlChamenos-FSC_OPTIMIZED`` () =
    let cfg =  "core/controlChamenos"
    singleTestBuildAndRunAux cfg  ["--tailcalls"] FSC_OPTIMIZED

[<Fact>]
let ``controlChamenos-FSI`` () =
    let cfg =  "core/controlChamenos"
    singleTestBuildAndRunAux cfg  ["--tailcalls"] FSI

[<Fact>]
let ``controlMailbox-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/controlMailbox" FSC_OPTIMIZED

[<Fact>]
let ``controlMailbox-FSI`` () = singleTestBuildAndRun "core/controlMailbox" FSI

[<Fact>]
let ``controlMailbox --tailcalls`` () =
    let cfg =  "core/controlMailbox"
    singleTestBuildAndRunAux cfg  ["--tailcalls"] FSC_OPTIMIZED

[<Fact>]
let ``csext-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/csext" FSC_OPTIMIZED

[<Fact>]
let ``csext-FSI`` () = singleTestBuildAndRun "core/csext" FSI

[<Fact>]
let ``enum-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/enum" FSC_OPTIMIZED

[<Fact>]
let ``enum-FSI`` () = singleTestBuildAndRun "core/enum" FSI

[<Fact>]
let ``longnames-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/longnames" FSC_OPTIMIZED

[<Fact>]
let ``longnames-FSI`` () = singleTestBuildAndRun "core/longnames" FSI

[<Fact>]
let ``math-numbersVS2008-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/math/numbersVS2008" FSC_OPTIMIZED

[<Fact>]
let ``math-numbersVS2008-FSI`` () = singleTestBuildAndRun "core/math/numbersVS2008" FSI

[<Fact>]
let ``patterns-FSC_OPTIMIZED`` () = singleTestBuildAndRunVersion "core/patterns" FSC_OPTIMIZED LangVersion.Preview

// This requires --multiemit on by default, which is not the case for .NET Framework
[<FactForNETCOREAPP>]
let ``patterns-FSI`` () = singleTestBuildAndRun "core/patterns" FSI

[<Fact>]
let ``fsi_load-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/fsi-load" FSC_OPTIMIZED

[<Fact>]
let ``fsi_load-FSI`` () = singleTestBuildAndRun "core/fsi-load" FSI

[<Fact>]
let ``reflect-FSC_OPTIMIZED`` () = singleTestBuildAndRun "core/reflect" FSC_OPTIMIZED

[<Fact>]
let ``reflect-FSI`` () = singleTestBuildAndRun "core/reflect" FSI

module PInvokeTests = 
    open System.Runtime.InteropServices
    let isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)

    [<Fact()>]
    let ``pinvoke-FSC_OPTIMIZED`` () =   
        if isWindows then
            singleTestBuildAndRun "core/pinvoke" FSC_OPTIMIZED

    [<Fact>]
    let ``pinvoke-FSI`` () =
        if isWindows then
            singleTestBuildAndRun "core/pinvoke" FSI