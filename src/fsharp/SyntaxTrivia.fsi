// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SyntaxTrivia

open FSharp.Compiler.Text

/// Represents additional information for SynExpr.TryWith
[<NoEquality; NoComparison>]
type SynExprTryWithTrivia =
    {
        /// The syntax range of the `try` keyword.
        TryKeyword: range
        /// The syntax range from the beginning of the `try` keyword till the end of the `with` keyword.
        TryToWithRange: range
        /// The syntax range of the `with` keyword
        WithKeyword: range
        /// The syntax range from the beginning of the `with` keyword till the end of the TryWith expression.
        WithToEndRange: range
    }

/// Represents additional information for SynExpr.TryFinally
[<NoEquality; NoComparison>]
type SynExprTryFinallyTrivia =
    {
        /// The syntax range of the `try` keyword.
        TryKeyword: range
        /// The syntax range of the `finally` keyword
        FinallyKeyword: range
    }

/// Represents additional information for SynExpr.IfThenElse
[<NoEquality; NoComparison>]
type SynExprIfThenElseTrivia =
    {
        /// The syntax range of the `if` keyword.
        IfKeyword: range
        /// Indicates if the `elif` keyword was used
        IsElif: bool
        /// The syntax range of the `then` keyword.
        ThenKeyword: range
        /// The syntax range of the `else` keyword.
        ElseKeyword: range option
        /// The syntax range from the beginning of the `if` keyword till the end of the `then` keyword.
        IfToThenRange: range
    }

/// Represents additional information for SynExpr.Lambda
[<NoEquality; NoComparison>]
type SynExprLambdaTrivia =
    {
        /// The syntax range of the `->` token.
        ArrowRange: range option
    }
    static member Zero: SynExprLambdaTrivia

/// Represents additional information for SynExpr.Lambda
[<NoEquality; NoComparison>]
type SynExprLetOrUseTrivia =
    {
        /// The syntax range of the `in` keyword.
        InKeyword: range option
    }

/// Represents additional information for SynMatchClause
[<NoEquality; NoComparison>]
type SynMatchClauseTrivia =
    {
        /// The syntax range of the `->` token.
        ArrowRange: range option
        /// The syntax range of the `|` token.
        BarRange: range option
    }
    static member Zero: SynMatchClauseTrivia

/// Represents additional information for 
[<NoEquality; NoComparison>]
type SynEnumCaseTrivia =
    {
        /// The syntax range of the `|` token.
        BarRange: range option
        /// The syntax range of the `=` token.
        EqualsRange: range
    }

/// Represents additional information for SynUnionCase
[<NoEquality; NoComparison>]
type SynUnionCaseTrivia =
    {
        /// The syntax range of the `|` token.
        BarRange: range option
    }

/// Represents additional information for SynPat.Or
[<NoEquality; NoComparison>]
type SynPatOrTrivia =
    {
        /// The syntax range of the `|` token.
        BarRange: range
    }

/// Represents additional information for SynTypeDefn
[<NoEquality; NoComparison>]
type SynTypeDefnTrivia =
    {
        /// The syntax range of the `type` keyword.
        TypeKeyword: range option
        /// The syntax range of the `=` token.
        EqualsRange: range option
        /// The syntax range of the `with` keyword
        WithKeyword: range option
    }
    static member Zero: SynTypeDefnTrivia
