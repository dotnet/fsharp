// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SyntaxTrivia

open FSharp.Compiler.Text

[<RequireQualifiedAccess; NoEquality; NoComparison>]
type ConditionalDirectiveTrivia =
    | IfDirectiveTrivia of expr:IfDirectiveExpression * range:range
    | ElseDirectiveTrivia of range:range
    | EndIfDirectiveTrivia of range:range

and IfDirectiveExpression =
    | IfdefAnd of IfDirectiveExpression * IfDirectiveExpression
    | IfdefOr of IfDirectiveExpression * IfDirectiveExpression
    | IfdefNot of IfDirectiveExpression
    | IfdefId of string

/// Represents additional information for ParsedImplFileInput
[<NoEquality; NoComparison>]
type ParsedImplFileInputTrivia =
    {
        /// Preprocessor directives of type #if, #else or #endif
        ConditionalDirectives: ConditionalDirectiveTrivia list
    }

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

/// Represents additional information for SynExpr.LetOrUse
[<NoEquality; NoComparison>]
type SynExprLetOrUseTrivia =
    {
        /// The syntax range of the `in` keyword.
        InKeyword: range option
    }

/// Represents additional information for SynExpr.LetOrUseBang
[<NoEquality; NoComparison>]
type SynExprLetOrUseBangTrivia =
    {
        /// The syntax range of the `=` token.
        EqualsRange: range option
    }
    static member Zero: SynExprLetOrUseBangTrivia

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

/// Represents additional information for SynBinding
[<NoEquality; NoComparison>]
type SynBindingTrivia =
    {
        /// The syntax range of the `let` keyword.
        LetKeyword: range option
        /// The syntax range of the `=` token.
        EqualsRange: range option
    }
    static member Zero: SynBindingTrivia

/// Represents additional information for SynMemberFlags
[<NoEquality; NoComparison>]
type SynMemberFlagsTrivia =
    {
        /// The syntax range of the `member` keyword
        MemberRange: range option
        /// The syntax range of the `override` keyword
        OverrideRange: range option
        /// The syntax range of the `abstract` keyword
        AbstractRange: range option
        /// The syntax range of the `member` keyword
        StaticRange: range option
        /// The syntax range of the `default` keyword
        DefaultRange: range option
    }
    static member Zero: SynMemberFlagsTrivia

/// Represents additional information for SynExprAndBang
[<NoEquality; NoComparison>]
type SynExprAndBangTrivia =
    {
        /// The syntax range of the `=` token.
        EqualsRange: range
        /// The syntax range of the `in` keyword.
        InKeyword: range option
    }

/// Represents additional information for SynModuleDecl.NestedModule
[<NoEquality; NoComparison>]
type SynModuleDeclNestedModuleTrivia =
    {
        /// The syntax range of the `module` keyword
        ModuleKeyword: range option
        /// The syntax range of the `=` token.
        EqualsRange: range option
    }
    static member Zero: SynModuleDeclNestedModuleTrivia

/// Represents additional information for SynModuleSigDecl.NestedModule
[<NoEquality; NoComparison>]
type SynModuleSigDeclNestedModuleTrivia =
    {
        /// The syntax range of the `module` keyword
        ModuleKeyword: range option
        /// The syntax range of the `=` token.
        EqualsRange: range option
    }
    static member Zero: SynModuleSigDeclNestedModuleTrivia
