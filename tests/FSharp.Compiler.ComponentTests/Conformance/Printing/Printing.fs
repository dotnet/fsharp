// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Printing

module printing =

    open Xunit
    open FSharp.Test
    open FSharp.Test.Compiler
    open System.IO

    let compileAndRunAsFsxShouldSucceed compilation =
        compilation
        |> asFsx
        |> withNoWarn 988
        |> runFsi
        |> shouldSucceed

    let compileAndRunAsExeShouldSucceed compilation =
        compilation
        |> asFs
        |> compileExeAndRun
        |> shouldSucceed

    [<FileInlineData("array2D.blit_01.fsx")>]
    [<FileInlineData("array2D_01.fsx")>]
    [<FileInlineData("array2D_01b.fsx")>]
    [<FileInlineData("BindingsWithValues01.fsx")>]
    [<FileInlineData("Choice01.fsx")>]
    [<FileInlineData("CustomExceptions01.fsx")>]
    [<FileInlineData("CustomExceptions02.fsx")>]
    [<FileInlineData("DisposeOnSprintfA.fsx")>]
    [<FileInlineData("LazyValues01.fsx")>]
    [<FileInlineData("LazyValues01NetFx4.fsx")>]
    [<FileInlineData("LazyValues02.fsx")>]
    [<FileInlineData("LazyValues02NetFx4.fsx")>]
    [<FileInlineData("LazyValues03.fsx")>]
    [<FileInlineData("LazyValues03NetFx4.fsx")>]
    [<FileInlineData("ParamArrayInSignatures.fsx")>]
    [<FileInlineData("Quotation01.fsx")>]
    [<FileInlineData("SignatureWithOptionalArgs01.fsx")>]
    [<FileInlineData("ToStringOnCollections.fsx")>]
    [<FileInlineData("UnitsOfMeasureIdentifiersRoundTrip01.fsx ")>]
    [<FileInlineData("UnitsOfMeasuresGenericSignature01.fsx")>]
    [<FileInlineData("UnitsOfMeasuresGenericSignature02.fsx")>]
    [<FileInlineData("VariantTypes01.fsx")>]
    [<FileInlineData("WidthForAFormatter.fsx")>]
    [<Theory>]
    let ``AsFsx`` compilation =
        compilation
        |> getCompilation
        |> compileAndRunAsFsxShouldSucceed

    [<FileInlineData("array2D.blit_01.fsx")>]
    [<FileInlineData("array2D_01.fsx")>]
    [<FileInlineData("array2D_01b.fsx")>]
    [<FileInlineData("BindingsWithValues01.fsx")>]
    [<FileInlineData("Choice01.fsx")>]
    [<FileInlineData("CustomExceptions01.fsx")>]
    [<FileInlineData("CustomExceptions02.fsx")>]
    [<FileInlineData("DisposeOnSprintfA.fsx")>]
    [<FileInlineData("LazyValues01.fsx")>]
    [<FileInlineData("LazyValues01NetFx4.fsx")>]
    [<FileInlineData("LazyValues02.fsx")>]
    [<FileInlineData("LazyValues02NetFx4.fsx")>]
    [<FileInlineData("LazyValues03.fsx")>]
    [<FileInlineData("LazyValues03NetFx4.fsx")>]
    [<FileInlineData("ParamArrayInSignatures.fsx")>]
    [<FileInlineData("Quotation01.fsx")>]
    [<FileInlineData("SignatureWithOptionalArgs01.fsx")>]
    [<FileInlineData("ToStringOnCollections.fsx")>]
    [<FileInlineData("UnitsOfMeasureIdentifiersRoundTrip01.fsx")>]
    [<FileInlineData("UnitsOfMeasuresGenericSignature01.fsx")>]
    [<FileInlineData("UnitsOfMeasuresGenericSignature02.fsx")>]
    [<FileInlineData("VariantTypes01.fsx")>]
    [<FileInlineData("WidthForAFormatter.fsx")>]
    [<Theory>]
    let ``AsExe`` compilation =
        compilation
        |> getCompilation
        |> compileAndRunAsFsxShouldSucceed

    [<Theory; FileInlineData("UnitsOfMeasureIdentifiersRoundTrip02.fsx")>]
    let ``UnitsOfMeasureIdentifiersRoundTrip02_fsx`` compilation =

        let library =
            Fsx ( loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__,  "UnitsOfMeasureIdentifiersRoundTrip02.fs")))
            |> asLibrary
            |> withName "UnitsOfMeasureIdentifiersRoundTrip02"
            |> ignoreWarnings

        compilation
        |> getCompilation
        |> withReferences [ library ]
        |> compileAndRunAsFsxShouldSucceed
