// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ErrorMessages.ComponentTests

open Xunit
open FSharp.Test.Utilities
open FSharp.Compiler.SourceCodeServices


module ``Wrong syntax in for loop`` =

    [<Fact>]
    let ``Equals instead of in``() =
        CompilerAssert.ParseWithErrors
            """
module X
for i = 0 .. 100 do
    ()
            """
            [|FSharpErrorSeverity.Error, 3215, (3, 7, 3, 8), "Unexpected symbol '=' in expression. Did you intend to use 'for x in y .. z do' instead?" |]
