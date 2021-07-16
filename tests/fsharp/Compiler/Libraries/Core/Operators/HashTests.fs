// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Compiler.Diagnostics
open FSharp.Test

[<TestFixture>]
module ``Hash Tests`` =

    [<Test>]
    let ``Hash of function values``() =
        // Regression test for FSHARP1.0:5436
        // You should not be able to hash F# function values
        // Note: most positive cases already covered under fsharp\typecheck\sigs
        // I'm adding this simple one since I did not see it there.
        
        CompilerAssert.TypeCheckSingleError
            """
hash id |> ignore
            """
            FSharpDiagnosticSeverity.Error
            1
            (2, 6, 2, 8)
            "The type '('a -> 'a)' does not support the 'equality' constraint because it is a function type"

    [<Test>]
    let ``Unchecked hash of function values``() =
        // Regression test for FSHARP1.0:5436
        // You should not be able to hash F# function values
        // Note: most positive cases already covered under fsharp\typecheck\sigs
        // I'm adding this simple one since I did not see it there.

        // This is ok (unchecked)
        CompilerAssert.TypeCheckWithErrors
            """
Unchecked.hash id |> ignore
            """
            [||]