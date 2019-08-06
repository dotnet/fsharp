// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Compiler.SourceCodeServices

[<TestFixture>]
module ``Wrong syntax in for loop`` =

    [<Test>]
    let ``Equals instead of in``() =
        CompilerAssert.ParseWithErrors
            """
module X
for i = 0 .. 100 do
    ()
            """
            [|FSharpErrorSeverity.Error, 3215, (3, 7, 3, 8), "Unexpected symbol '=' in expression. Did you intend to use 'for x in y .. z do' instead?" |]
