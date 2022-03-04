// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.ErrorMessages

open Xunit
open FSharp.Test.Compiler

module ``Upcast and Downcast`` =

    [<Fact>]
    let ``Downcast Instead Of Upcast``() =
        FSharp """
open System.Collections.Generic

let orig = Dictionary<obj,obj>() :> IDictionary<obj,obj>
let c = orig :> Dictionary<obj,obj>
        """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 193, Line 5, Col 9, Line 5, Col 36,
                                 "Type constraint mismatch. The type \n    'IDictionary<obj,obj>'    \nis not compatible with type\n    'Dictionary<obj,obj>'    \n")

    [<Fact>]
    let ``Upcast Instead Of Downcast``() =
        FSharp """
open System.Collections.Generic

let orig = Dictionary<obj,obj>()
let c = orig :?> IDictionary<obj,obj>
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 67,   Line 5, Col 9, Line 5, Col 38, "This type test or downcast will always hold")
            (Error   3198, Line 5, Col 9, Line 5, Col 38, "The conversion from Dictionary<obj,obj> to IDictionary<obj,obj> is a compile-time safe upcast, not a downcast. Consider using the :> (upcast) operator instead of the :?> (downcast) operator.")]

    [<Fact>]
    let ``Upcast Function Instead Of Downcast``() =
        FSharp """
open System.Collections.Generic

let orig = Dictionary<obj,obj>()
let c : IDictionary<obj,obj> = downcast orig
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 67,   Line 5, Col 32, Line 5, Col 45, "This type test or downcast will always hold")
            (Error   3198, Line 5, Col 32, Line 5, Col 45, "The conversion from Dictionary<obj,obj> to IDictionary<obj,obj> is a compile-time safe upcast, not a downcast. Consider using 'upcast' instead of 'downcast'.")]
