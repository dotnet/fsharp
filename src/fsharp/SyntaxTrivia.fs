// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SyntaxTrivia

open FSharp.Compiler.Text

[<NoEquality; NoComparison>]
type SynExprTryWithTrivia =
    { TryKeyword: range
      TryToWithRange: range
      WithKeyword: range
      WithToEndRange: range }

[<NoEquality; NoComparison>]
type SynExprTryFinallyTrivia =
    { TryKeyword: range
      FinallyKeyword: range }

[<NoEquality; NoComparison>]
type SynExprIfThenElseTrivia =
    { IfKeyword: range
      IsElif: bool
      ThenKeyword: range
      ElseKeyword: range option
      IfToThenRange: range }

[<NoEquality; NoComparison>]
type SynExprLambdaTrivia =
    { ArrowRange: range option }
    static member Zero: SynExprLambdaTrivia = { ArrowRange = None }

[<NoEquality; NoComparison>]
type SynExprLetOrUseTrivia =
    { InKeyword: range option }

[<NoEquality; NoComparison>]
type SynMatchClauseTrivia =
    { ArrowRange: range option
      BarRange: range option }
    static member Zero: SynMatchClauseTrivia = { ArrowRange = None; BarRange = None }

[<NoEquality; NoComparison>]
type SynEnumCaseTrivia =
    { BarRange: range option
      EqualsRange: range }

[<NoEquality; NoComparison>]
type SynUnionCaseTrivia = { BarRange: range option }

[<NoEquality; NoComparison>]
type SynPatOrTrivia = { BarRange: range }

[<NoEquality; NoComparison>]
type SynTypeDefnTrivia =
    { TypeKeyword: range option
      EqualsRange: range option
      WithKeyword: range option }
    static member Zero: SynTypeDefnTrivia =
        { TypeKeyword = None
          EqualsRange = None
          WithKeyword = None }
