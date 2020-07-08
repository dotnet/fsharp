// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework

[<TestFixture>]
module ``PreComputedTupleConstructor Tests`` =

    [<Test>]
    let ``PreComputedTupleConstructor of int and string``() =
        // Regression test for FSHARP1.0:5113
        // MT DCR: Reflection.FSharpValue.PreComputeTupleConstructor fails when executed for NetFx 2.0 by a Dev10 compiler

        let testDelegate = TestDelegate (fun () -> 
            Reflection.FSharpValue.PreComputeTupleConstructor(typeof<int * string>) [| box 12; box "text" |] |> ignore)

        Assert.DoesNotThrow testDelegate |> ignore

    [<Test>]
    let ``PreComputedTupleConstructor with wrong order of arguments``() =
        // Regression test for FSHARP1.0:5113
        // MT DCR: Reflection.FSharpValue.PreComputeTupleConstructor fails when executed for NetFx 2.0 by a Dev10 compiler

        let testDelegate = TestDelegate (fun () -> 
            Reflection.FSharpValue.PreComputeTupleConstructor(typeof<int * string>) [| box "text"; box 12; |] |> ignore)

        Assert.Throws<System.ArgumentException> testDelegate |> ignore