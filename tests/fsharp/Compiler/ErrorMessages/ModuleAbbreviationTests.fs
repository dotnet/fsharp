// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.TestHelpers
open FSharp.Compiler.SourceCodeServices

[<TestFixture>]
module ``Module Abbreviations`` =

    [<Test>]
    let ``Public Module Abbreviation``() =
        CompilerAssert.TypeCheckSingleError
            """
module public L1 = List
            """
            FSharpErrorSeverity.Error
            536
            (2, 1, 2, 7)
            "The 'Public' accessibility attribute is not allowed on module abbreviation. Module abbreviations are always private."