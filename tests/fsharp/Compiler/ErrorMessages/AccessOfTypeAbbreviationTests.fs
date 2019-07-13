// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Compiler.SourceCodeServices

[<TestFixture>]
module ``Access Of Type Abbreviation`` =

    [<Test>]
    let ``Test``() =
        CompilerAssert.TypeCheckSingleError
            """
module Library =
  type private Hidden = Hidden of unit
  type Exported = Hidden
            """
            FSharpErrorSeverity.Warning
            44
            (4, 8, 4, 16)
            "The type 'Hidden' is less accessible than the value, member or type 'Exported' it is used in"

    [<Test>]
    let ``Test2``() =
        CompilerAssert.Pass
            """
module Library =
  type internal Hidden = Hidden of unit
  type internal Exported = Hidden
            """

    [<Test>]
    let ``Test3``() =
        CompilerAssert.TypeCheckSingleError
            """
module Library =
  type internal Hidden = Hidden of unit
  type Exported = Hidden
            """
            FSharpErrorSeverity.Warning
            44
            (4, 8, 4, 16)
            "The type 'Hidden' is less accessible than the value, member or type 'Exported' it is used in"

    [<Test>]
    let ``Test4``() =
        CompilerAssert.TypeCheckSingleError
            """
module Library =
  type private Hidden = Hidden of unit
  type internal Exported = Hidden
            """
            FSharpErrorSeverity.Warning
            44
            (4, 8, 4, 16)
            "The type 'Hidden' is less accessible than the value, member or type 'Exported' it is used in"

    [<Test>]
    let ``Test5``() =
        CompilerAssert.Pass
            """
module Library =
  type private Hidden = Hidden of unit
  type private Exported = Hidden
            """

    [<Test>]
    let ``Test6``() =
        CompilerAssert.Pass
            """
module Library =
  type Hidden = Hidden of unit
  type Exported = Hidden
            """