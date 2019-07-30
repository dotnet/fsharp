// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Compiler.SourceCodeServices

[<TestFixture>]
module ``Wrong syntax in for loop`` =

    [<Test>]
    let ``Equals instead of in``() =
        CompilerAssert.TypeCheckSingleError
            """
for i = 0 .. 100 do
    ()

exit 0
            """
            FSharpErrorSeverity.Warning
            3215
            (6, 5, 6, 11)
            "Unexpected symbol '=' in expression. Did you intend to use 'for x in y .. z do' instead?"
