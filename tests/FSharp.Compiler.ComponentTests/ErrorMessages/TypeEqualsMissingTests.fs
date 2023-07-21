﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace ErrorMessages

open Xunit
open FSharp.Test
open FSharp.Compiler.Diagnostics


module ``Type definition missing equals`` =

    [<Fact>]
    let ``Missing equals in DU``() =
        CompilerAssert.TypeCheckSingleError
            """
type X | A | B
            """
            FSharpDiagnosticSeverity.Error
            10 
            (2, 8, 2, 9)
            "Unexpected symbol '|' in type definition. Expected '=' or other token."
