// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SyntaxTrivia

open FSharp.Compiler.Text

[<RequireQualifiedAccess; NoEquality; NoComparison>]
type IdentTrivia =
    | OriginalNotation of text: string
    | OriginalNotationWithParen of leftParenRange: range * text: string * rightParenRange: range
    | HasParenthesis of leftParenRange: range * rightParenRange: range

[<RequireQualifiedAccess; NoEquality; NoComparison>]
type ConditionalDirectiveTrivia =
    | If of expr: IfDirectiveExpression * range: range
    | Else of range: range
    | EndIf of range: range

and [<RequireQualifiedAccess; NoEquality; NoComparison>] IfDirectiveExpression =
    | And of IfDirectiveExpression * IfDirectiveExpression
    | Or of IfDirectiveExpression * IfDirectiveExpression
    | Not of IfDirectiveExpression
    | Ident of string

[<RequireQualifiedAccess; NoEquality; NoComparison>]
type CommentTrivia =
    | LineComment of range: range
    | BlockComment of range: range

[<NoEquality; NoComparison>]
type ParsedImplFileInputTrivia =
    {
        ConditionalDirectives: ConditionalDirectiveTrivia list
        CodeComments: CommentTrivia list
    }

[<NoEquality; NoComparison>]
type ParsedSigFileInputTrivia =
    {
        ConditionalDirectives: ConditionalDirectiveTrivia list
        CodeComments: CommentTrivia list
    }

[<NoEquality; NoComparison>]
type SynExprTryWithTrivia =
    {
        TryKeyword: range
        TryToWithRange: range
        WithKeyword: range
        WithToEndRange: range
    }

[<NoEquality; NoComparison>]
type SynExprTryFinallyTrivia =
    {
        TryKeyword: range
        FinallyKeyword: range
    }

[<NoEquality; NoComparison>]
type SynExprIfThenElseTrivia =
    {
        IfKeyword: range
        IsElif: bool
        ThenKeyword: range
        ElseKeyword: range option
        IfToThenRange: range
    }

[<NoEquality; NoComparison>]
type SynExprLambdaTrivia =
    {
        ArrowRange: range option
    }

    static member Zero: SynExprLambdaTrivia = { ArrowRange = None }

[<NoEquality; NoComparison>]
type SynExprLetOrUseTrivia = { InKeyword: range option }

[<NoEquality; NoComparison>]
type SynExprLetOrUseBangTrivia =
    {
        EqualsRange: range option
    }

    static member Zero: SynExprLetOrUseBangTrivia = { EqualsRange = None }

[<NoEquality; NoComparison>]
type SynExprMatchTrivia =
    {
        MatchKeyword: range
        WithKeyword: range
    }

[<NoEquality; NoComparison>]
type SynExprMatchBangTrivia =
    {
        MatchBangKeyword: range
        WithKeyword: range
    }

[<NoEquality; NoComparison>]
type SynMatchClauseTrivia =
    {
        ArrowRange: range option
        BarRange: range option
    }

    static member Zero: SynMatchClauseTrivia = { ArrowRange = None; BarRange = None }

[<NoEquality; NoComparison>]
type SynEnumCaseTrivia =
    {
        BarRange: range option
        EqualsRange: range
    }

[<NoEquality; NoComparison>]
type SynUnionCaseTrivia = { BarRange: range option }

[<NoEquality; NoComparison>]
type SynPatOrTrivia = { BarRange: range }

[<NoEquality; NoComparison>]
type SynPatListConsTrivia = { ColonColonRange: range }

[<NoEquality; NoComparison>]
type SynTypeDefnTrivia =
    {
        TypeKeyword: range option
        EqualsRange: range option
        WithKeyword: range option
    }

    static member Zero: SynTypeDefnTrivia =
        {
            TypeKeyword = None
            EqualsRange = None
            WithKeyword = None
        }

[<NoEquality; NoComparison>]
type SynTypeDefnSigTrivia =
    {
        TypeKeyword: range option
        EqualsRange: range option
        WithKeyword: range option
    }

    static member Zero: SynTypeDefnSigTrivia =
        {
            TypeKeyword = None
            EqualsRange = None
            WithKeyword = None
        }

[<NoEquality; NoComparison; RequireQualifiedAccess>]
type SynLeadingKeyword =
    | Let of letRange: range
    | LetRec of letRange: range * recRange: range
    | And of andRange: range
    | Use of useRange: range
    | UseRec of useRange: range * recRange: range
    | Extern of externRange: range
    | Member of memberRange: range
    | MemberVal of memberRange: range * valRange: range
    | Override of overrideRange: range
    | OverrideVal of overrideRange: range * valRange: range
    | Abstract of abstractRange: range
    | AbstractMember of abstractRange: range * memberRange: range
    | StaticMember of staticRange: range * memberRange: range
    | StaticMemberVal of staticRange: range * memberRange: range * valRange: range
    | StaticAbstract of staticRange: range * abstractRange: range
    | StaticAbstractMember of staticRange: range * abstractMember: range * memberRange: range
    | StaticVal of staticRange: range * valRange: range
    | StaticLet of staticRange: range * letRange: range
    | StaticLetRec of staticRange: range * letRange: range * recRange: range
    | StaticDo of staticRange: range * doRange: range
    | Default of defaultRange: range
    | DefaultVal of defaultRange: range * valRange: range
    | Val of valRange: range
    | New of newRange: range
    | Do of doRange: range
    | Synthetic

    member this.Range =
        match this with
        | Let m
        | And m
        | Use m
        | Extern m
        | Member m
        | Override m
        | Abstract m
        | Default m
        | Val m
        | New m
        | Do m -> m
        | LetRec (m1, m2)
        | UseRec (m1, m2)
        | AbstractMember (m1, m2)
        | StaticMember (m1, m2)
        | StaticAbstract (m1, m2)
        | StaticAbstractMember (m1, _, m2)
        | StaticVal (m1, m2)
        | StaticLet (m1, m2)
        | StaticLetRec (m1, _, m2)
        | StaticDo (m1, m2)
        | DefaultVal (m1, m2)
        | MemberVal (m1, m2)
        | OverrideVal (m1, m2)
        | StaticMemberVal (m1, _, m2) -> Range.unionRanges m1 m2
        | Synthetic -> Range.Zero

[<NoEquality; NoComparison>]
type SynBindingTrivia =
    {
        LeadingKeyword: SynLeadingKeyword
        EqualsRange: range option
    }

    static member Zero: SynBindingTrivia =
        {
            LeadingKeyword = SynLeadingKeyword.Synthetic
            EqualsRange = None
        }

[<NoEquality; NoComparison>]
type SynExprAndBangTrivia =
    {
        EqualsRange: range
        InKeyword: range option
    }

[<NoEquality; NoComparison>]
type SynModuleDeclNestedModuleTrivia =
    {
        ModuleKeyword: range option
        EqualsRange: range option
    }

    static member Zero: SynModuleDeclNestedModuleTrivia =
        {
            ModuleKeyword = None
            EqualsRange = None
        }

[<NoEquality; NoComparison>]
type SynModuleSigDeclNestedModuleTrivia =
    {
        ModuleKeyword: range option
        EqualsRange: range option
    }

    static member Zero: SynModuleSigDeclNestedModuleTrivia =
        {
            ModuleKeyword = None
            EqualsRange = None
        }

[<NoEquality; NoComparison>]
type SynModuleOrNamespaceTrivia =
    {
        ModuleKeyword: range option
        NamespaceKeyword: range option
    }

[<NoEquality; NoComparison>]
type SynModuleOrNamespaceSigTrivia =
    {
        ModuleKeyword: range option
        NamespaceKeyword: range option
    }

[<NoEquality; NoComparison>]
type SynValSigTrivia =
    {
        LeadingKeyword: SynLeadingKeyword
        WithKeyword: range option
        EqualsRange: range option
    }

    static member Zero: SynValSigTrivia =
        {
            LeadingKeyword = SynLeadingKeyword.Synthetic
            WithKeyword = None
            EqualsRange = None
        }

[<NoEquality; NoComparison>]
type SynTypeFunTrivia = { ArrowRange: range }

[<NoEquality; NoComparison>]
type SynMemberGetSetTrivia =
    {
        WithKeyword: range
        GetKeyword: range option
        AndKeyword: range option
        SetKeyword: range option
    }

[<NoEquality; NoComparison>]
type SynArgPatsNamePatPairsTrivia = { ParenRange: range }

[<NoEquality; NoComparison>]
type SynMemberDefnAutoPropertyTrivia =
    {
        LeadingKeyword: SynLeadingKeyword
        WithKeyword: range option
        EqualsRange: range option
        GetSetKeyword: range option
    }

[<NoEquality; NoComparison>]
type SynFieldTrivia =
    {
        LeadingKeyword: SynLeadingKeyword option
    }

    static member Zero: SynFieldTrivia = { LeadingKeyword = None }

type SynTypeOrTrivia = { OrKeyword: range }

