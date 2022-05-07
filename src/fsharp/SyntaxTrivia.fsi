// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace rec FSharp.Compiler.SyntaxTrivia

open FSharp.Compiler.Text

[<RequireQualifiedAccess; NoEquality; NoComparison>]
type IdentTrivia =
    /// The ident originally had a different notation.
    /// Example: a + b
    /// The operator ident will be compiled into "op_Addition", while the original notation was "+"
    | OriginalNotation of text: string
    /// The ident originally had a different notation and parenthesis
    /// Example: let (>=>) a b = ...
    /// The operator ident will be compiled into "op_GreaterEqualsGreater", while the original notation was ">=>" and had parenthesis
    | OriginalNotationWithParen of leftParenRange: range * text: string * rightParenRange: range
    /// The ident had parenthesis
    /// Example: let (|Odd|Even|) = ...
    /// The active pattern ident will be "|Odd|Even|", while originally there were parenthesis.
    | HasParenthesis of leftParenRange: range * rightParenRange: range

[<RequireQualifiedAccess; NoEquality; NoComparison>]
type ConditionalDirectiveTrivia =
    | If of expr: IfDirectiveExpression * range: range
    | Else of range: range
    | EndIf of range: range

[<RequireQualifiedAccess; NoEquality; NoComparison>]
type IfDirectiveExpression =
    | And of IfDirectiveExpression * IfDirectiveExpression
    | Or of IfDirectiveExpression * IfDirectiveExpression
    | Not of IfDirectiveExpression
    | Ident of string

[<RequireQualifiedAccess; NoEquality; NoComparison>]
type CommentTrivia =
    | LineComment of range: range
    | BlockComment of range: range

/// Represents additional information for ParsedImplFileInput
[<NoEquality; NoComparison>]
type ParsedImplFileInputTrivia =
    { /// Preprocessor directives of type #if, #else or #endif
      ConditionalDirectives: ConditionalDirectiveTrivia list
      /// Represent code comments found in the source file
      CodeComments: CommentTrivia list }

/// Represents additional information for ParsedSigFileInputTrivia
[<NoEquality; NoComparison>]
type ParsedSigFileInputTrivia =
    { /// Preprocessor directives of type #if, #else or #endif
      ConditionalDirectives: ConditionalDirectiveTrivia list
      /// Represent code comments found in the source file
      CodeComments: CommentTrivia list }

/// Represents additional information for SynExpr.TryWith
[<NoEquality; NoComparison>]
type SynExprTryWithTrivia =
    { /// The syntax range of the `try` keyword.
      TryKeyword: range
      /// The syntax range from the beginning of the `try` keyword till the end of the `with` keyword.
      TryToWithRange: range
      /// The syntax range of the `with` keyword
      WithKeyword: range
      /// The syntax range from the beginning of the `with` keyword till the end of the TryWith expression.
      WithToEndRange: range }

/// Represents additional information for SynExpr.TryFinally
[<NoEquality; NoComparison>]
type SynExprTryFinallyTrivia =
    { /// The syntax range of the `try` keyword.
      TryKeyword: range
      /// The syntax range of the `finally` keyword
      FinallyKeyword: range }

/// Represents additional information for SynExpr.IfThenElse
[<NoEquality; NoComparison>]
type SynExprIfThenElseTrivia =
    { /// The syntax range of the `if` keyword.
      IfKeyword: range
      /// Indicates if the `elif` keyword was used
      IsElif: bool
      /// The syntax range of the `then` keyword.
      ThenKeyword: range
      /// The syntax range of the `else` keyword.
      ElseKeyword: range option
      /// The syntax range from the beginning of the `if` keyword till the end of the `then` keyword.
      IfToThenRange: range }

/// Represents additional information for SynExpr.Lambda
[<NoEquality; NoComparison>]
type SynExprLambdaTrivia =
    { /// The syntax range of the `->` token.
      ArrowRange: range option }
    static member Zero: SynExprLambdaTrivia

/// Represents additional information for SynExpr.LetOrUse
[<NoEquality; NoComparison>]
type SynExprLetOrUseTrivia =
    { /// The syntax range of the `in` keyword.
      InKeyword: range option }

/// Represents additional information for SynExpr.LetOrUseBang
[<NoEquality; NoComparison>]
type SynExprLetOrUseBangTrivia =
    { /// The syntax range of the `=` token.
      EqualsRange: range option }
    static member Zero: SynExprLetOrUseBangTrivia

/// Represents additional information for SynExpr.Match
[<NoEquality; NoComparison>]
type SynExprMatchTrivia =
    { /// The syntax range of the `match` keyword
      MatchKeyword: range
      /// The syntax range of the `with` keyword
      WithKeyword: range }

/// Represents additional information for SynExpr.MatchBang
[<NoEquality; NoComparison>]
type SynExprMatchBangTrivia =
    { /// The syntax range of the `match!` keyword
      MatchBangKeyword: range
      /// The syntax range of the `with` keyword
      WithKeyword: range }

/// Represents additional information for SynMatchClause
[<NoEquality; NoComparison>]
type SynMatchClauseTrivia =
    { /// The syntax range of the `->` token.
      ArrowRange: range option
      /// The syntax range of the `|` token.
      BarRange: range option }
    static member Zero: SynMatchClauseTrivia

/// Represents additional information for
[<NoEquality; NoComparison>]
type SynEnumCaseTrivia =
    { /// The syntax range of the `|` token.
      BarRange: range option
      /// The syntax range of the `=` token.
      EqualsRange: range }

/// Represents additional information for SynUnionCase
[<NoEquality; NoComparison>]
type SynUnionCaseTrivia =
    { /// The syntax range of the `|` token.
      BarRange: range option }

/// Represents additional information for SynPat.Or
[<NoEquality; NoComparison>]
type SynPatOrTrivia =
    { /// The syntax range of the `|` token.
      BarRange: range }

/// Represents additional information for SynTypeDefn
[<NoEquality; NoComparison>]
type SynTypeDefnTrivia =
    { /// The syntax range of the `type` keyword.
      TypeKeyword: range option
      /// The syntax range of the `=` token.
      EqualsRange: range option
      /// The syntax range of the `with` keyword
      WithKeyword: range option }
    static member Zero: SynTypeDefnTrivia

/// Represents additional information for SynBinding
[<NoEquality; NoComparison>]
type SynBindingTrivia =
    { /// The syntax range of the `let` keyword.
      LetKeyword: range option
      /// The syntax range of the `=` token.
      EqualsRange: range option }
    static member Zero: SynBindingTrivia

/// Represents additional information for SynMemberFlags
[<NoEquality; NoComparison>]
type SynMemberFlagsTrivia =
    { /// The syntax range of the `member` keyword
      MemberRange: range option
      /// The syntax range of the `override` keyword
      OverrideRange: range option
      /// The syntax range of the `abstract` keyword
      AbstractRange: range option
      /// The syntax range of the `member` keyword
      StaticRange: range option
      /// The syntax range of the `default` keyword
      DefaultRange: range option }
    static member Zero: SynMemberFlagsTrivia

/// Represents additional information for SynExprAndBang
[<NoEquality; NoComparison>]
type SynExprAndBangTrivia =
    { /// The syntax range of the `=` token.
      EqualsRange: range
      /// The syntax range of the `in` keyword.
      InKeyword: range option }

/// Represents additional information for SynModuleDecl.NestedModule
[<NoEquality; NoComparison>]
type SynModuleDeclNestedModuleTrivia =
    { /// The syntax range of the `module` keyword
      ModuleKeyword: range option
      /// The syntax range of the `=` token.
      EqualsRange: range option }
    static member Zero: SynModuleDeclNestedModuleTrivia

/// Represents additional information for SynModuleSigDecl.NestedModule
[<NoEquality; NoComparison>]
type SynModuleSigDeclNestedModuleTrivia =
    { /// The syntax range of the `module` keyword
      ModuleKeyword: range option
      /// The syntax range of the `=` token.
      EqualsRange: range option }
    static member Zero: SynModuleSigDeclNestedModuleTrivia

/// Represents additional information for SynModuleOrNamespace
[<NoEquality; NoComparison>]
type SynModuleOrNamespaceTrivia =
    { /// The syntax range of the `module` keyword
      ModuleKeyword: range option
      /// The syntax range of the `namespace` keyword
      NamespaceKeyword: range option }

/// Represents additional information for SynModuleOrNamespaceSig
[<NoEquality; NoComparison>]
type SynModuleOrNamespaceSigTrivia =
    { /// The syntax range of the `module` keyword
      ModuleKeyword: range option
      /// The syntax range of the `namespace` keyword
      NamespaceKeyword: range option }

/// Represents additional information for SynValSig
[<NoEquality; NoComparison>]
type SynValSigTrivia =
    { /// The syntax range of the `val` keyword
      ValKeyword: range option
      /// The syntax range of the `with` keyword
      WithKeyword: range option }
    static member Zero: SynValSigTrivia
