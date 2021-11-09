// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.ErrorMessages

open Xunit
open FSharp.Test.Compiler
open FSharp.Compiler.Diagnostics

module ``Access Of Type Abbreviation`` =

    let warning44Message = "This construct is deprecated. The type 'Hidden' is less accessible than the value, member or type 'Exported' it is used in." + System.Environment.NewLine + "As of F# 4.1, the accessibility of type abbreviations is checked at compile-time. Consider changing the accessibility of the type abbreviation. Ignoring this warning might lead to runtime errors."

    [<Fact>]
    let ``Private type produces warning when trying to export``() =
        FSharp """
module Library =
  type private Hidden = Hidden of unit
  type Exported = Hidden
        """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 44, Line 4, Col 8, Line 4, Col 16, warning44Message)

    [<Fact>]
    let ``Internal type passes when abbrev is internal``() =
        FSharp """
module Library =
  type internal Hidden = Hidden of unit
  type internal Exported = Hidden
        """
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Internal type produces warning when trying to export``() =
       FSharp """
module Library =
  type internal Hidden = Hidden of unit
  type Exported = Hidden
        """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 44, Line 4, Col 8, Line 4, Col 16, warning44Message)

    [<Fact>]
    let ``Private type produces warning when abbrev is internal``() =
        FSharp """
module Library =
  type private Hidden = Hidden of unit
  type internal Exported = Hidden
        """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 44, Line 4, Col 17, Line 4, Col 25, warning44Message)

    [<Fact>]
    let ``Private type passes when abbrev is private``() =
        FSharp """
module Library =
  type private Hidden = Hidden of unit
  type private Exported = Hidden
         """
         |> typecheck
         |> shouldSucceed

    [<Fact>]
    let ``Default access type passes when abbrev is default``() =
        FSharp """
module Library =
  type Hidden = Hidden of unit
  type Exported = Hidden
        """
         |> typecheck
         |> shouldSucceed
