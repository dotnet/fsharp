// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SyntaxTrivia

open FSharp.Compiler.Text

[<NoEquality; NoComparison>]
type SynExprTryWithTrivia =
    { TryKeyword: Range
      TryToWithRange: Range
      WithKeyword: Range
      WithToEndRange: Range }
