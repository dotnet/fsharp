// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ErrorMessages.ComponentTests

open Xunit
open FSharp.Test.Utilities
open FSharp.Compiler.SourceCodeServices

module ``Access Of Type Abbreviation`` =

    [<Fact>]
    let ``Private type produces warning when trying to export``() =
        CompilerAssert.TypeCheckSingleError
            """
module Library =
  type private Hidden = Hidden of unit
  type Exported = Hidden
            """
            FSharpErrorSeverity.Warning
            44
            (4, 8, 4, 16)
            ("This construct is deprecated. The type 'Hidden' is less accessible than the value, member or type 'Exported' it is used in." + System.Environment.NewLine + "As of F# 4.1, the accessibility of type abbreviations is checked at compile-time. Consider changing the accessibility of the type abbreviation. Ignoring this warning might lead to runtime errors.")

    [<Fact>]
    let ``Internal type passes when abbrev is internal``() =
        CompilerAssert.Pass
            """
module Library =
  type internal Hidden = Hidden of unit
  type internal Exported = Hidden
            """

    [<Fact>]
    let ``Internal type produces warning when trying to export``() =
        CompilerAssert.TypeCheckSingleError
            """
module Library =
  type internal Hidden = Hidden of unit
  type Exported = Hidden
            """
            FSharpErrorSeverity.Warning
            44
            (4, 8, 4, 16)
            ("This construct is deprecated. The type 'Hidden' is less accessible than the value, member or type 'Exported' it is used in." + System.Environment.NewLine + "As of F# 4.1, the accessibility of type abbreviations is checked at compile-time. Consider changing the accessibility of the type abbreviation. Ignoring this warning might lead to runtime errors.")

    [<Fact>]
    let ``Private type produces warning when abbrev is internal``() =
        CompilerAssert.TypeCheckSingleError
            """
module Library =
  type private Hidden = Hidden of unit
  type internal Exported = Hidden
            """
            FSharpErrorSeverity.Warning
            44
            (4, 17, 4, 25)
            ("This construct is deprecated. The type 'Hidden' is less accessible than the value, member or type 'Exported' it is used in." + System.Environment.NewLine + "As of F# 4.1, the accessibility of type abbreviations is checked at compile-time. Consider changing the accessibility of the type abbreviation. Ignoring this warning might lead to runtime errors.")

    [<Fact>]
    let ``Private type passes when abbrev is private``() =
        CompilerAssert.Pass
            """
module Library =
  type private Hidden = Hidden of unit
  type private Exported = Hidden
            """

    [<Fact>]
    let ``Default access type passes when abbrev is default``() =
        CompilerAssert.Pass
            """
module Library =
  type Hidden = Hidden of unit
  type Exported = Hidden
            """
