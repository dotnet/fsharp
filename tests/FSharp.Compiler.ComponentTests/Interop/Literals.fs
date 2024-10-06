// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Interop

open Xunit
open FSharp.Test.Compiler

module ``Literals interop`` =

    [<Fact>]
    let ``Instantiate F# decimal literal from C#`` () =
        let FSLib =
            FSharp """
namespace Interop.FS

module DecimalLiteral =
  [<Literal>]
  let x = 7m
        """
          |> withName "FSLib"

        let app =
            CSharp """
using System;
using Interop.FS;
public class C {
    public Decimal y = DecimalLiteral.x;
}
        """
          |> withReferences [FSLib]
          |> withName "CSharpApp"

        app
        |> compile
        |> shouldSucceed