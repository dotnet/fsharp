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
type SynExprDotLambdaTrivia =
    {
        UnderscoreRange: range
        DotRange: range
    }

[<NoEquality; NoComparison>]
type SynExprLetOrUseTrivia =
    {
        InKeyword: range option
    }

    static member Zero: SynExprLetOrUseTrivia = { InKeyword = None }

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
type SynExprAnonRecdTrivia = { OpeningBraceRange: range }

[<NoEquality; NoComparison>]
type SynExprSequentialTrivia =
    {
        SeparatorRange: range option
    }

    static member val Zero = { SeparatorRange = None }

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

[<NoEquality; NoComparison; RequireQualifiedAccess>]
type SynTypeDefnLeadingKeyword =
    | Type of range
    | And of range
    | StaticType of staticRange: range * typeRange: range
    | Synthetic

    member this.Range =
        match this with
        | SynTypeDefnLeadingKeyword.Type range
        | SynTypeDefnLeadingKeyword.And range -> range
        | SynTypeDefnLeadingKeyword.StaticType(staticRange, typeRange) -> Range.unionRanges staticRange typeRange
        | SynTypeDefnLeadingKeyword.Synthetic -> failwith "Getting range from synthetic keyword"

[<NoEquality; NoComparison>]
type SynTypeDefnTrivia =
    {
        LeadingKeyword: SynTypeDefnLeadingKeyword
        EqualsRange: range option
        WithKeyword: range option
    }

    static member Zero: SynTypeDefnTrivia =
        {
            LeadingKeyword = SynTypeDefnLeadingKeyword.Synthetic
            EqualsRange = None
            WithKeyword = None
        }

[<NoEquality; NoComparison>]
type SynTypeDefnSigTrivia =
    {
        LeadingKeyword: SynTypeDefnLeadingKeyword
        EqualsRange: range option
        WithKeyword: range option
    }

    static member Zero: SynTypeDefnSigTrivia =
        {
            LeadingKeyword = SynTypeDefnLeadingKeyword.Synthetic
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
    | Static of staticRange: range
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
        | Do m
        | Static m -> m
        | LetRec(m1, m2)
        | UseRec(m1, m2)
        | AbstractMember(m1, m2)
        | StaticMember(m1, m2)
        | StaticAbstract(m1, m2)
        | StaticAbstractMember(m1, _, m2)
        | StaticVal(m1, m2)
        | StaticLet(m1, m2)
        | StaticLetRec(m1, _, m2)
        | StaticDo(m1, m2)
        | DefaultVal(m1, m2)
        | MemberVal(m1, m2)
        | OverrideVal(m1, m2)
        | StaticMemberVal(m1, _, m2) -> Range.unionRanges m1 m2
        | Synthetic -> Range.Zero

[<NoEquality; NoComparison>]
type SynBindingTrivia =
    {
        LeadingKeyword: SynLeadingKeyword
        InlineKeyword: range option
        EqualsRange: range option
    }

    static member Zero: SynBindingTrivia =
        {
            LeadingKeyword = SynLeadingKeyword.Synthetic
            InlineKeyword = None
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

[<NoEquality; NoComparison; RequireQualifiedAccess>]
type SynModuleOrNamespaceLeadingKeyword =
    | Module of moduleRange: range
    | Namespace of namespaceRange: range
    | None

[<NoEquality; NoComparison>]
type SynModuleOrNamespaceTrivia =
    {
        LeadingKeyword: SynModuleOrNamespaceLeadingKeyword
    }

[<NoEquality; NoComparison>]
type SynModuleOrNamespaceSigTrivia =
    {
        LeadingKeyword: SynModuleOrNamespaceLeadingKeyword
    }

[<NoEquality; NoComparison>]
type SynValSigTrivia =
    {
        LeadingKeyword: SynLeadingKeyword
        InlineKeyword: range option
        WithKeyword: range option
        EqualsRange: range option
    }

    static member Zero: SynValSigTrivia =
        {
            LeadingKeyword = SynLeadingKeyword.Synthetic
            InlineKeyword = None
            WithKeyword = None
            EqualsRange = None
        }

[<NoEquality; NoComparison>]
type SynTypeFunTrivia = { ArrowRange: range }

[<NoEquality; NoComparison>]
type SynMemberGetSetTrivia =
    {
        InlineKeyword: range option
        WithKeyword: range
        GetKeyword: range option
        AndKeyword: range option
        SetKeyword: range option
    }

[<NoEquality; NoComparison>]
type SynMemberDefnImplicitCtorTrivia = { AsKeyword: range option }

[<NoEquality; NoComparison>]
type SynArgPatsNamePatPairsTrivia = { ParenRange: range }

[<NoEquality; NoComparison>]
type GetSetKeywords =
    | Get of range
    | Set of range
    | GetSet of get: range * set: range

    member x.Range =
        match x with
        | Get m
        | Set m -> m
        | GetSet(mG, mS) ->
            if Range.rangeBeforePos mG mS.Start then
                Range.unionRanges mG mS
            else
                Range.unionRanges mS mG

[<NoEquality; NoComparison>]
type SynMemberDefnAutoPropertyTrivia =
    {
        LeadingKeyword: SynLeadingKeyword
        WithKeyword: range option
        EqualsRange: range option
        GetSetKeywords: GetSetKeywords option
    }

[<NoEquality; NoComparison>]
type SynMemberDefnAbstractSlotTrivia =
    {
        GetSetKeywords: GetSetKeywords option
    }

    static member Zero = { GetSetKeywords = None }

[<NoEquality; NoComparison>]
type SynFieldTrivia =
    {
        LeadingKeyword: SynLeadingKeyword option
        MutableKeyword: range option
    }

    static member Zero: SynFieldTrivia =
        {
            LeadingKeyword = None
            MutableKeyword = None
        }

[<NoEquality; NoComparison>]
type SynTypeOrTrivia = { OrKeyword: range }

[<NoEquality; NoComparison>]
type SynBindingReturnInfoTrivia = { ColonRange: range option }

[<NoEquality; NoComparison>]
type SynMemberSigMemberTrivia =
    {
        GetSetKeywords: GetSetKeywords option
    }

    static member Zero: SynMemberSigMemberTrivia = { GetSetKeywords = None }

[<NoEquality; NoComparison>]
type SynTyparDeclTrivia =
    {
        AmpersandRanges: range list
    }

    static member Zero: SynTyparDeclTrivia = { AmpersandRanges = [] }

[<NoEquality; NoComparison>]
type SynMeasureConstantTrivia =
    {
        LessRange: range
        GreaterRange: range
    }
