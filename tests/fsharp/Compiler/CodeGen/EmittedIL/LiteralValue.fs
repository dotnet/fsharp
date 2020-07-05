// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests.CodeGen.EmittedIL

open FSharp.Test.Utilities
open NUnit.Framework

[<TestFixture>]
module ``Literal Value`` =

    [<Test>]
    let ``Literal Value``() =
        CompilerAssert.CompileLibraryAndVerifyIL
            """
module LiteralValue

[<Literal>]
let x = 7

[<EntryPoint>]
let main _ =
    0
            """
            (fun verifier -> verifier.VerifyIL [
            """
.field public static literal int32 x = int32(0x00000007)
            """
            ])
