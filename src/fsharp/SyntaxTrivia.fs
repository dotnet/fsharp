// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SyntaxTrivia

open FSharp.Compiler.Text

[<RequireQualifiedAccess; NoEquality; NoComparison>]
type ConditionalDirectiveTrivia =
    | IfDirectiveTrivia of expr:IfDirectiveExpression * range:range
    | ElseDirectiveTrivia of range:range
    | EndIfDirectiveTrivia of range:range

and [<RequireQualifiedAccess; NoEquality; NoComparison>] IfDirectiveExpression =
    | IfdefAnd of IfDirectiveExpression * IfDirectiveExpression
    | IfdefOr of IfDirectiveExpression * IfDirectiveExpression
    | IfdefNot of IfDirectiveExpression
    | IfdefId of string

[<NoEquality; NoComparison>]
type ParsedImplFileInputTrivia =
    { ConditionalDirectives: ConditionalDirectiveTrivia list }

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
type SynExprLetOrUseBangTrivia =
    { EqualsRange: range option }
    static member Zero: SynExprLetOrUseBangTrivia =
        { EqualsRange = None }

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

[<NoEquality; NoComparison>]
type SynBindingTrivia =
    { LetKeyword: range option
      EqualsRange: range option }
    static member Zero: SynBindingTrivia =
        { LetKeyword = None
          EqualsRange = None }

[<NoEquality; NoComparison>]
type SynMemberFlagsTrivia =
    { MemberRange: range option
      OverrideRange: range option
      AbstractRange: range option
      StaticRange: range option
      DefaultRange: range option }
    static member Zero: SynMemberFlagsTrivia =
        { MemberRange = None
          OverrideRange = None
          AbstractRange = None
          StaticRange = None
          DefaultRange = None }

[<NoEquality; NoComparison>]
type SynExprAndBangTrivia =
    { EqualsRange: range
      InKeyword: range option }

[<NoEquality; NoComparison>]
type SynModuleDeclNestedModuleTrivia =
    { ModuleKeyword: range option
      EqualsRange: range option }
    static member Zero: SynModuleDeclNestedModuleTrivia = { ModuleKeyword = None; EqualsRange = None }

[<NoEquality; NoComparison>]
type SynModuleSigDeclNestedModuleTrivia =
    { ModuleKeyword: range option
      EqualsRange: range option }
    static member Zero: SynModuleSigDeclNestedModuleTrivia = { ModuleKeyword = None; EqualsRange = None }
