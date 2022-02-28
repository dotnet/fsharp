// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
module FSharp.Compiler.ComponentTests.Conformance.Printing

    open Xunit
    open FSharp.Test
    open FSharp.Test.Compiler
    open System.IO

    let CompileAndRunAsFsx compilation =
        compilation
        |> asFsx
        |> withNoWarn 988
        |> runFsi
        |> shouldSucceed

    let CompileAndRunAsFs compilation =
        compilation
        |> asFs
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Printing", Includes=[|"array2D_01.fsx"|])>]
    let ``array2D_01_fsx`` compilation =
        compilation |> CompileAndRunAsFsx

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Printing", Includes=[|"array2D_01b.fsx"|])>]
    let ``array2D_01b_fsx`` compilation =
        compilation |> CompileAndRunAsFsx

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Printing", Includes=[|"array2D.blit_01.fsx"|])>]
    let ``array2D_blit_01_fsx`` compilation =
        compilation |> CompileAndRunAsFsx

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Printing", Includes=[|"Choice01.fsx"|])>]
    let ``Choice01_fsx`` compilation =
        compilation |> CompileAndRunAsFsx

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Printing", Includes=[|"DisposeOnSprintfA.fs"|])>]
    let ``DisposeOnSprintfA_fs`` compilation =
        compilation |> CompileAndRunAsFs

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Printing", Includes=[|"UnitsOfMeasureIdentifiersRoundTrip02.fsx"|])>]
    let ``UnitsOfMeasureIdentifiersRoundTrip02_fsx`` compilation =

        let library =
            Fsx ( loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__,  "../../resources/tests/Conformance/Printing/UnitsOfMeasureIdentifiersRoundTrip02.fs")))
            |> asLibrary
            |> withName "UnitsOfMeasureIdentifiersRoundTrip02"
            |> ignoreWarnings

        compilation
        |> withReferences [ library ]
        |> CompileAndRunAsFsx

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Printing", Includes=[|"UnitsOfMeasuresGenericSignature01.fsx"|])>]
    let ``UnitsOfMeasuresGenericSignature01_fsx`` compilation =
        compilation |> CompileAndRunAsFsx

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Printing", Includes=[|"UnitsOfMeasuresGenericSignature02.fsx"|])>]
    let ``UnitsOfMeasuresGenericSignature02_fsx`` compilation =
        compilation |> CompileAndRunAsFsx

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Printing", Includes=[|"Quotation01.fs"|])>]
    let ``Quotation01_fs`` compilation =
        compilation |> CompileAndRunAsFsx

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Printing", Includes=[|"SignatureWithOptionalArgs01.fsx"|])>]
    let ``SignatureWithOptionalArgs01_fsx`` compilation =
        compilation |> CompileAndRunAsFsx

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Printing", Includes=[|"VariantTypes01.fs"|])>]
    let ``VariantTypes01_fs`` compilation =
        compilation |> CompileAndRunAsFsx

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Printing", Includes=[|"CustomExceptions01.fs"|])>]
    let ``CustomExceptions01_fs`` compilation =
        compilation |> CompileAndRunAsFs

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Printing", Includes=[|"CustomExceptions02.fsx"|])>]
    let ``CustomExceptions02_fsx`` compilation =
        compilation |>
        CompileAndRunAsFsx

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Printing", Includes=[|"LazyValues01NetFx4.fsx"|])>]
    let ``LazyValues01NetFx4_fsx`` compilation =
        compilation |> CompileAndRunAsFs

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Printing", Includes=[|"LazyValues02NetFx4.fsx"|])>]
    let ``LazyValues02NetFx4_fsx`` compilation =
        compilation |> CompileAndRunAsFsx

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Printing", Includes=[|"LazyValues03NetFx4.fsx"|])>]
    let ``LazyValues03NetFx4_fsx`` compilation =
        compilation |> CompileAndRunAsFsx

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Printing", Includes=[|"WidthForAFormatter.fs"|])>]
    let ``WidthForAFormatter_fs`` compilation =
        compilation |> CompileAndRunAsFsx

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Printing", Includes=[|"ToStringOnCollections.fs"|])>]
    let ``ToStringOnCollections_fs`` compilation =
        compilation |> CompileAndRunAsFsx

    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Printing", Includes=[|"ParamArrayInSignatures.fsx"|])>]
    let ``ParamArrayInSignatures_fsx`` compilation =
        compilation |> CompileAndRunAsFsx

