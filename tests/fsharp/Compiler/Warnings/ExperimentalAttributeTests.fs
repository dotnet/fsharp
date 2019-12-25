// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Compiler.SourceCodeServices

[<TestFixture>]
module ``Validate ExperimentalAttribute and LanguagVersion `` =

    let experimentalSource = """
module TestModule =

    [<ExperimentalAttribute("Experimental attribute message")>]
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
            FSharpErrorSeverity.Warning
            57
            (7, 8, 7, 17)
            "Experimental attribute message. This warning can be disabled using '--nowarn:57' or '#nowarn \"57\"'."
