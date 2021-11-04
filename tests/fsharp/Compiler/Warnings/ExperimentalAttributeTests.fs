// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test
open FSharp.Compiler.Diagnostics

[<TestFixture>]
module ``Validate ExperimentalAttribute and LanguageVersion`` =

    let experimentalSource = """
module TestModule =

    [<ExperimentalAttribute("Preview library feature, requires '--langversion:preview'")>]
    let getString = "A string"

    if getString = "A string" then ()
"""

    [<Test>]
    let ``ExperimentalAttribute nowarn when preview specified``() =
        CompilerAssert.PassWithOptions
            [| "--langversion:preview" |]
            experimentalSource

    [<Test>]
    let ``ExperimentalAttribute warn when preview not specified``() =
        CompilerAssert.TypeCheckSingleError
            experimentalSource
            FSharpDiagnosticSeverity.Warning
            57
            (7, 8, 7, 17)
            "Preview library feature, requires '--langversion:preview'. This warning can be disabled using '--nowarn:57' or '#nowarn \"57\"'."
