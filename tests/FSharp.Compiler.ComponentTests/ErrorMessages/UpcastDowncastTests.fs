// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ErrorMessages.ComponentTests

open Xunit
open FSharp.Test.Utilities
open FSharp.Compiler.SourceCodeServices

module ``Upcast and Downcast`` =

    [<Fact>]
    let ``Downcast Instead Of Upcast``() =
        CompilerAssert.TypeCheckSingleError
            """
open System.Collections.Generic

let orig = Dictionary<obj,obj>() :> IDictionary<obj,obj>
let c = orig :> Dictionary<obj,obj>
            """
            FSharpErrorSeverity.Error
            193
            (5, 9, 5, 36)
            "Type constraint mismatch. The type \n    'IDictionary<obj,obj>'    \nis not compatible with type\n    'Dictionary<obj,obj>'    \n"

    [<Fact>]
    let ``Upcast Instead Of Downcast``() =
        CompilerAssert.TypeCheckWithErrors
            """
open System.Collections.Generic

let orig = Dictionary<obj,obj>()
let c = orig :?> IDictionary<obj,obj>
            """
            [|
                FSharpErrorSeverity.Warning, 67, (5, 9, 5, 38), "This type test or downcast will always hold"
                FSharpErrorSeverity.Error, 3198, (5, 9, 5, 38), "The conversion from Dictionary<obj,obj> to IDictionary<obj,obj> is a compile-time safe upcast, not a downcast. Consider using the :> (upcast) operator instead of the :?> (downcast) operator."
            |]

    [<Fact>]
    let ``Upcast Function Instead Of Downcast``() =
        CompilerAssert.TypeCheckWithErrors
            """
open System.Collections.Generic

let orig = Dictionary<obj,obj>()
let c : IDictionary<obj,obj> = downcast orig
            """
            [|
                FSharpErrorSeverity.Warning, 67, (5, 32, 5, 45), "This type test or downcast will always hold"
                FSharpErrorSeverity.Error, 3198, (5, 32, 5, 45), "The conversion from Dictionary<obj,obj> to IDictionary<obj,obj> is a compile-time safe upcast, not a downcast. Consider using 'upcast' instead of 'downcast'."
            |]
