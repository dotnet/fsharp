// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.VisualStudio.FSharp.Editor.SourceTransformer

open Microsoft.VisualStudio.FSharp.Editor.FormatConfig
open Microsoft.VisualStudio.FSharp.Editor.SourceParser
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Range

[<RequireQualifiedAccess>]
module List = 
    let inline atMostOne xs =
        match xs with
        | [] | [_] -> true
        | _ -> false

/// Check whether an expression should be broken into multiple lines. 
/// Notice that order of patterns matters due to non-disjoint property.
let rec multiline = function
    | ConstExpr _
    | NullExpr
    | OptVar _
    | SequentialSimple _ ->
        false

    | ObjExpr _
    | While _
    | For _
    | ForEach _
    | TryWith _
    | TryFinally _
    | Sequentials _
    | IfThenElse _ ->
        true

    | Paren e
    | SingleExpr(_, e)
    | TypedExpr(_, e, _)
    | CompExpr(_, e)
    | ArrayOrListOfSeqExpr(_, e)
    | DesugaredLambda(_, e)
    | Lambda(e, _)
    | TypeApp(e, _)
    | LongIdentSet(_, e)
    | DotGet(e, _)
    | TraitCall(_, _, e) ->
        multiline e

    | Quote(e1, e2, _)
    | JoinIn(e1, e2)
    | DotSet(e1, _, e2)
    | LetOrUseBang(_, _, e1, e2) ->
        multiline e1 || multiline e2

    | Tuple es ->
        List.exists multiline es

    // An infix app is multiline if it contains at least two new line infix ops
    | InfixApps(e, es) ->
        multiline e
        || not (List.atMostOne (List.filter (fst >> NewLineInfixOps.Contains) es))
        || List.exists (snd >> multiline) es
    
    | App(e1, es) ->
        multiline e1 || List.exists multiline es
    | DotIndexedGet(e, _) ->
        multiline e

    | DotIndexedSet(e1, _, e2) ->
        multiline e1 || multiline e2

    | MatchLambda(cs, _) ->
        not (List.atMostOne cs)
    | Match(e, cs) ->
        not (List.isEmpty cs) || multiline e
    | LetOrUse(_, _, bs, e) ->
        not (List.isEmpty bs) || multiline e

    // An array or a list is multiline if there are at least two elements
    | ArrayOrList(_, es, _) ->
        not (List.atMostOne es)

    // A record is multiline if there is at least two fields present
    | Record(_, xs, _) ->
        let fields = xs |> List.choose ((|RecordFieldName|) >> snd) 
        not (List.atMostOne fields) || List.exists multiline fields

    // Default mode is single-line
    | _ -> false

let checkNewLine e es = 
    match es with
    | (s, _) :: _ :: _ -> NewLineInfixOps.Contains s
    | _ -> multiline e

/// Check if the expression already has surrounding parentheses
let hasParenthesis = function
    | Paren _
    | ConstExpr(Const "()", _)
    | Tuple _ -> true
    | _ -> false

let hasParenInPat = function
    | PatParen _ -> true
    | _ -> false

let genConst (Unresolved(c, r, s)) =
    let r' = c.Range r
    fun ctx -> 
        if ctx.Config.StrictMode then
            str s ctx
        else
            let s' = defaultArg (lookup r' ctx) s
            str s' ctx

/// Check whether a range starting with a specified token
let startWith prefix (r : range) ctx = 
    lookup r ctx |> Option.exists (String.startsWithOrdinal prefix)

// A few active patterns for printing purpose

let rec (|DoExprAttributesL|_|) = function
    | DoExpr _ | Attributes _  as x::DoExprAttributesL(xs, ys) -> Some(x::xs, ys)
    | DoExpr _ | Attributes _ as x::ys -> Some([x], ys)
    | _ -> None

let rec (|HashDirectiveL|_|) = function
    | HashDirective _ as x::HashDirectiveL(xs, ys) -> Some(x::xs, ys)
    | HashDirective _ as x::ys -> Some([x], ys)
    | _ -> None

let rec (|SigHashDirectiveL|_|) = function
    | SigHashDirective _ as x::SigHashDirectiveL(xs, ys) -> Some(x::xs, ys)
    | SigHashDirective _ as x::ys -> Some([x], ys)
    | _ -> None

let rec (|ModuleAbbrevL|_|) = function
    | ModuleAbbrev _ as x::ModuleAbbrevL(xs, ys) -> Some(x::xs, ys)
    | ModuleAbbrev _ as x::ys -> Some([x], ys)
    | _ -> None

let rec (|SigModuleAbbrevL|_|) = function
    | SigModuleAbbrev _ as x::SigModuleAbbrevL(xs, ys) -> Some(x::xs, ys)
    | SigModuleAbbrev _ as x::ys -> Some([x], ys)
    | _ -> None

let rec (|OpenL|_|) = function
    | Open _ as x::OpenL(xs, ys) -> Some(x::xs, ys)
    | Open _ as x::ys -> Some([x], ys)
    | _ -> None

let rec (|SigOpenL|_|) = function
    | SigOpen _ as x::SigOpenL(xs, ys) -> Some(x::xs, ys)
    | SigOpen _ as x::ys -> Some([x], ys)
    | _ -> None

let rec (|MDOpenL|_|) = function
    | MDOpen _ as x::MDOpenL(xs, ys) -> Some(x::xs, ys)
    | MDOpen _ as x::ys -> Some([x], ys)
    | _ -> None

let rec (|SigValL|_|) = function
    | SigVal _ as x::SigValL(xs, ys) -> Some(x::xs, ys)
    | SigVal _ as x::ys -> Some([x], ys)
    | _ -> None

/// Omit a break before an expression if the expression is small and it is already one line in the text
let checkPreserveBreakForExpr e (ctx : Context) =
    multiline e || ctx.Comments.ContainsKey(e.Range.Start) || ctx.Directives.ContainsKey(e.Range.Start)

/// Omit a break before an expression if the expression is small 
let checkBreakForExpr e =
    multiline e 

let (|OneLinerExpr|_|) (e:Ast.SynExpr) =
    if checkBreakForExpr e then None else Some e

let (|OneLinerBinding|MultilineBinding|) b =
    match b with
    | LetBinding([], PreXmlDoc [||], _, _, _, _, OneLinerExpr _)
    | DoBinding([], PreXmlDoc [||], OneLinerExpr _)
    | MemberBinding([], PreXmlDoc [||], _, _, _, _, OneLinerExpr _)
    | PropertyBinding([], PreXmlDoc [||], _, _, _, _, OneLinerExpr _) 
    | ExplicitCtor([], PreXmlDoc [||], _, _, OneLinerExpr _, _) -> 
        OneLinerBinding b

    | _ -> MultilineBinding b

let rec (|OneLinerLetL|_|) = function
    | Let(OneLinerBinding _) as x::OneLinerLetL(xs, ys) -> Some(x::xs, ys)
    | Let(OneLinerBinding _) as x::ys -> Some([x], ys)
    | _ -> None

let (|MultilineModuleDecl|_|) = function
    | DoExpr _
    | Attributes _
    | HashDirective _
    | ModuleAbbrev _
    | Open _
    | Let(OneLinerBinding _) -> None
    | md -> Some md

let (|SigMultilineModuleDecl|_|) = function
    | SigHashDirective _
    | SigModuleAbbrev _
    | SigVal _
    | SigOpen _ -> None
    | md -> Some md

let rec (|MultilineModuleDeclL|_|) = function
    | MultilineModuleDecl x::MultilineModuleDeclL(xs, ys) -> Some(x::xs, ys)
    | MultilineModuleDecl x::ys -> Some([x], ys)
    | _ -> None

let rec (|SigMultilineModuleDeclL|_|) = function
    | SigMultilineModuleDecl x::SigMultilineModuleDeclL(xs, ys) -> Some(x::xs, ys)
    | SigMultilineModuleDecl x::ys -> Some([x], ys)
    | _ -> None

/// Gather PropertyGetSet in one printing call. 
/// Assume that PropertySet comes right after PropertyGet.
let (|PropertyWithGetSet|_|) = function
    | PropertyBinding(_, _, _, _, MFProperty PropertyGet, PatLongIdent(_, s1, _, _), _) as b1::bs -> 
        match bs with
        | PropertyBinding(_, _, _, _, MFProperty PropertySet, PatLongIdent(_, s2, _, _), _) as b2::bs when s1 = s2 -> 
            Some((b1, b2), bs)
        | _ -> None
    | _ -> None 

let (|PropertyWithGetSetMemberDefn|_|) = function
    | MDMember(x1)::MDMember(x2)::xs ->
        match [x1; x2] with
        | PropertyWithGetSet((x1, x2), []) -> Some((x1, x2), xs)
        | _ -> None
    | _ -> None

let (|OneLinerMemberDefn|MultilineMemberDefn|) md =
    match md with
    | MDOpen _
    | MDInherit _
    | MDValField _
    | MDImplicitCtor _
    | MDInterface(_, None)
    | MDAbstractSlot([], PreXmlDoc [||], _, _, _, _, _, _) 
    | MDImplicitInherit(_, OneLinerExpr _, _)
    | MDMember(OneLinerBinding _)
    | MDAutoProperty([], PreXmlDoc [||], _, _, OneLinerExpr _, _, _, _, _)
    | MDLetBindings(_, _, [OneLinerBinding _]) ->
        OneLinerMemberDefn md

    | _ -> MultilineMemberDefn md

let rec (|OneLinerMemberDefnL|_|) xs = 
    match xs with
    /// This pattern prevents PropertyWithGetSet to be taken separately
    | PropertyWithGetSetMemberDefn _ -> Some([], xs)
    | OneLinerMemberDefn x::OneLinerMemberDefnL(xs, ys) -> Some(x::xs, ys)
    | OneLinerMemberDefn x::ys -> Some([x], ys)
    | _ -> None

/// Gather all multiline member definitions. 
/// This should be used before one-liner pattern.
let rec (|MultilineMemberDefnL|_|) = function
    | PropertyWithGetSetMemberDefn((x1, x2), MultilineMemberDefnL(xs, ys)) -> Some(Pair(x1, x2)::xs, ys)
    | PropertyWithGetSetMemberDefn((x1, x2), ys) -> Some([Pair(x1, x2)], ys)
    | MultilineMemberDefn x::MultilineMemberDefnL(xs, ys) -> Some(Single x::xs, ys)
    | MultilineMemberDefn x::ys -> Some([Single x], ys)
    | _ -> None

let rec (|OneLinerBindingL|_|) xs =
    match xs with
    | PropertyWithGetSet _ -> Some([], xs)
    | OneLinerBinding x::OneLinerBindingL(xs, ys) -> Some(x::xs, ys)
    | OneLinerBinding x::ys -> Some([x], ys)
    | _ -> None

/// Gather all multiline bindings. 
/// This should be used before one-liner pattern.
let rec (|MultilineBindingL|_|) = function
    | PropertyWithGetSet((x1, x2), MultilineBindingL(xs, ys)) -> Some(Pair(x1, x2)::xs, ys)
    | PropertyWithGetSet((x1, x2), ys) -> Some([Pair(x1, x2)], ys)
    | MultilineBinding x::MultilineBindingL(xs, ys) -> Some(Single x::xs, ys)
    | MultilineBinding x::ys -> Some([Single x], ys)
    | _ -> None

let rec (|OneLinerLetOrUseL|_|) = function
    | (prefix, OneLinerBinding x)::OneLinerLetOrUseL(xs, ys) -> Some((prefix, x)::xs, ys)
    | (prefix, OneLinerBinding x)::ys -> Some([prefix, x], ys)
    | _ -> None

let rec (|MultilineLetOrUseL|_|) = function
    | (prefix, MultilineBinding x)::MultilineLetOrUseL(xs, ys) -> Some((prefix, x)::xs, ys)
    | (prefix, MultilineBinding x)::ys -> Some([prefix, x], ys)
    | _ -> None