// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.EditorServices

open System
open System.IO
open System.Collections.Generic
open System.Text.RegularExpressions
open Internal.Utilities.Library
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Position
open FSharp.Compiler.Text.Range

module SourceFileImpl =
    let IsSignatureFile (file: string) =
        let ext = Path.GetExtension file
        0 = String.Compare(".fsi", ext, StringComparison.OrdinalIgnoreCase)

    /// Additional #defines that should be in place when editing a file in a file editor such as VS.
    let GetImplicitConditionalDefinesForEditing (isInteractive: bool) =
        if isInteractive then
            [ "INTERACTIVE"; "EDITING" ] // This is still used by the foreground parse
        else
            [ "COMPILED"; "EDITING" ]

type CompletionPath = string list * string option // plid * residue

[<RequireQualifiedAccess>]
type FSharpInheritanceOrigin =
    | Class
    | Interface
    | Unknown

[<RequireQualifiedAccess>]
type InheritanceContext =
    | Class
    | Interface
    | Unknown

[<RequireQualifiedAccess>]
type RecordContext =
    | CopyOnUpdate of range: range * path: CompletionPath
    | Constructor of typeName: string
    | Empty
    | New of path: CompletionPath * isFirstField: bool
    | Declaration of isInIdentifier: bool

[<RequireQualifiedAccess>]
type PatternContext =
    /// <summary>Completing union case field pattern (e.g. fun (Some v| ) -> ) or fun (Some (v| )) -> ). In theory, this could also be parameterized active pattern usage.</summary>
    /// <param name="fieldIndex">Position in the tuple. <see cref="None">None</see> if there is no tuple, with only one field outside of parentheses - `Some v|`</param>
    /// <param name="isTheOnlyField">True when completing the first field in the tuple and no other field is bound - `Case (a|)` but not `Case (a|, b)`</param>
    /// <param name="caseIdRange">Range of the case identifier</param>
    | PositionalUnionCaseField of fieldIndex: int option * isTheOnlyField: bool * caseIdRange: range

    /// Completing union case field pattern (e.g. fun (Some (Value = v| )) -> )
    | NamedUnionCaseField of fieldName: string * caseIdRange: range

    /// Completing union case field identifier in a pattern (e.g. fun (Case (field1 = a; fie| )) -> )
    | UnionCaseFieldIdentifier of referencedFields: string list * caseIdRange: range

    /// Completing a record field identifier in a pattern (e.g. fun { Field1 = a; Fie| } -> )
    | RecordFieldIdentifier of referencedFields: (string * range) list

    /// Any other position in a pattern that does not need special handling
    | Other

[<RequireQualifiedAccess; NoComparison; Struct>]
type MethodOverrideCompletionContext =
    | Class
    | Interface of mInterfaceName: range
    | ObjExpr of mExpr: range

[<RequireQualifiedAccess>]
type CompletionContext =
    /// Completion context cannot be determined due to errors
    | Invalid

    /// Completing something after the inherit keyword
    | Inherit of context: InheritanceContext * path: CompletionPath

    /// Completing records field
    | RecordField of context: RecordContext

    | RangeOperator

    /// Completing named parameters\setters in parameter list of attributes\constructor\method calls
    /// end of name ast node * list of properties\parameters that were already set
    | ParameterList of pos * HashSet<string>

    /// Completing an attribute name, outside of the constructor
    | AttributeApplication

    | OpenDeclaration of isOpenType: bool

    /// Completing a type annotation (e.g. foo (x: |))
    /// Completing a type application (e.g. typeof<str| >)
    | Type

    /// Completing union case fields declaration (e.g. 'A of stri|' but not 'B of tex|: string')
    | UnionCaseFieldsDeclaration

    /// Completing a type abbreviation (e.g. type Long = int6|)
    /// or a single case union without a bar (type SomeUnion = Abc|)
    | TypeAbbreviationOrSingleCaseUnion

    /// Completing a pattern in a match clause, member/let binding or lambda
    | Pattern of context: PatternContext

    /// Completing a method override (e.g. override this.ToStr|)
    | MethodOverride of
        ctx: MethodOverrideCompletionContext *
        enclosingTypeNameRange: range *
        spacesBeforeOverrideKeyword: int *
        hasThis: bool *
        isStatic: bool

type ShortIdent = string

type ShortIdents = ShortIdent[]

type MaybeUnresolvedIdent = { Ident: ShortIdent; Resolved: bool }

type ModuleKind =
    {
        IsAutoOpen: bool
        HasModuleSuffix: bool
    }

[<RequireQualifiedAccess>]
type EntityKind =
    | Attribute
    | Type
    | FunctionOrValue of isActivePattern: bool
    | Module of ModuleKind

    override x.ToString() = sprintf "%A" x

type InsertionContextEntity =
    {
        FullRelativeName: string
        Qualifier: string
        Namespace: string option
        FullDisplayName: string
        LastIdent: ShortIdent
    }

    override x.ToString() = sprintf "%A" x

type ScopeKind =
    | Namespace
    | TopModule
    | NestedModule
    | OpenDeclaration
    | HashDirective

    override x.ToString() = sprintf "%A" x

type InsertionContext = { ScopeKind: ScopeKind; Pos: pos }

type FSharpModule = { Idents: ShortIdents; Range: range }

type OpenStatementInsertionPoint =
    | TopLevel
    | Nearest

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Entity =
    let getRelativeNamespace (targetNs: ShortIdents) (sourceNs: ShortIdents) =
        let rec loop index =
            if index > targetNs.Length - 1 then sourceNs[index..]
            // target namespace is not a full parent of source namespace, keep the source ns as is
            elif index > sourceNs.Length - 1 then sourceNs
            elif targetNs[index] = sourceNs[index] then loop (index + 1)
            else sourceNs[index..]

        if sourceNs.Length = 0 || targetNs.Length = 0 then
            sourceNs
        else
            loop 0

    let cutAutoOpenModules (autoOpenParent: ShortIdents option) (candidateNs: ShortIdents) =
        let nsCount =
            match autoOpenParent with
            | Some parent when parent.Length > 0 -> min (parent.Length - 1) candidateNs.Length
            | _ -> candidateNs.Length

        candidateNs[0 .. nsCount - 1]

    let tryCreate
        (
            targetNamespace: ShortIdents option,
            targetScope: ShortIdents,
            partiallyQualifiedName: MaybeUnresolvedIdent[],
            requiresQualifiedAccessParent: ShortIdents option,
            autoOpenParent: ShortIdents option,
            candidateNamespace: ShortIdents option,
            candidate: ShortIdents
        ) =
        match candidate with
        | [||] -> [||]
        | _ ->
            partiallyQualifiedName
            |> Array.heads
            // long ident must contain an unresolved part, otherwise we show false positive suggestions like
            // "open System" for `let _ = System.DateTime.Naaaw`. Here only "Naaw" is unresolved.
            |> Array.filter (fun x -> x |> Array.exists (fun x -> not x.Resolved))
            |> Array.choose (fun parts ->
                let parts = parts |> Array.map (fun x -> x.Ident)

                if not (candidate |> Array.endsWith parts) then
                    None
                else
                    let identCount = parts.Length

                    let fullOpenableNs, restIdents =
                        let openableNsCount =
                            match requiresQualifiedAccessParent with
                            | Some parent -> min parent.Length candidate.Length
                            | None -> candidate.Length

                        candidate[0 .. openableNsCount - 2], candidate[openableNsCount - 1 ..]

                    let openableNs = cutAutoOpenModules autoOpenParent fullOpenableNs

                    let getRelativeNs ns =
                        match targetNamespace, candidateNamespace with
                        | Some targetNs, Some candidateNs when candidateNs = targetNs -> getRelativeNamespace targetScope ns
                        | None, _ -> getRelativeNamespace targetScope ns
                        | _ -> ns

                    let relativeNs = getRelativeNs openableNs

                    match relativeNs, restIdents with
                    | [||], [||] -> None
                    | [||], [| _ |] -> None
                    | _ ->
                        let fullRelativeName = Array.append (getRelativeNs fullOpenableNs) restIdents

                        let ns =
                            match relativeNs with
                            | [||] -> None
                            | _ when identCount > 1 && relativeNs.Length >= identCount ->
                                Some(relativeNs[0 .. relativeNs.Length - identCount] |> String.concat ".")
                            | _ -> Some(relativeNs |> String.concat ".")

                        let qualifier =
                            if fullRelativeName.Length > 1 && fullRelativeName.Length >= identCount then
                                fullRelativeName[0 .. fullRelativeName.Length - identCount]
                            else
                                fullRelativeName

                        Some
                            {
                                FullRelativeName = String.concat "." fullRelativeName //.[0..fullRelativeName.Length - identCount - 1]
                                Qualifier = String.concat "." qualifier
                                Namespace = ns
                                FullDisplayName =
                                    match restIdents with
                                    | [| _ |] -> ""
                                    | _ -> String.concat "." restIdents
                                LastIdent = Array.tryLast restIdents |> Option.defaultValue ""
                            })

module ParsedInput =

    /// A pattern that collects all sequential expressions to avoid StackOverflowException
    let internal (|Sequentials|_|) expr =

        let rec collect expr acc =
            match expr with
            | SynExpr.Sequential(expr1 = e1; expr2 = (SynExpr.Sequential _ as e2)) -> collect e2 (e1 :: acc)
            | SynExpr.Sequential(expr1 = e1; expr2 = e2) -> e2 :: e1 :: acc
            | _ -> acc

        match collect expr [] with
        | [] -> None
        | exprs -> Some(List.rev exprs)

    let emptyStringSet = HashSet<string>()

    let GetRangeOfExprLeftOfDot (pos: pos, parsedInput) =
        let CheckLongIdent (longIdent: LongIdent) =
            // find the longest prefix before the "pos" dot
            let mutable r = (List.head longIdent).idRange
            let mutable couldBeBeforeFront = true

            for i in longIdent do
                if posGeq pos i.idRange.End then
                    r <- unionRanges r i.idRange
                    couldBeBeforeFront <- false

            couldBeBeforeFront, r

        let visitor =
            { new SyntaxVisitorBase<_>() with
                member _.VisitExpr(_, traverseSynExpr, defaultTraverse, expr) =
                    let expr = expr // fix debugger locals

                    match expr with
                    | SynExpr.LongIdent(longDotId = SynLongIdent([ id ], [], [ Some _ ])) -> defaultTraverse (SynExpr.Ident(id))

                    | SynExpr.LongIdent(_, lid, _altNameRefCell, _) ->
                        let (SynLongIdent(longIdent, _, _)) = lid
                        let _, r = CheckLongIdent longIdent
                        Some r

                    | SynExpr.LongIdentSet(lid, synExpr, _) ->
                        let (SynLongIdent(longIdent, _, _)) = lid

                        if SyntaxTraversal.rangeContainsPosLeftEdgeInclusive synExpr.Range pos then
                            traverseSynExpr synExpr
                        else
                            let _, r = CheckLongIdent longIdent
                            Some r

                    | SynExpr.DotGet(synExpr, _dotm, lid, _) ->
                        let (SynLongIdent(longIdent, _, _)) = lid

                        if SyntaxTraversal.rangeContainsPosLeftEdgeInclusive synExpr.Range pos then
                            traverseSynExpr synExpr
                        else
                            let inFront, r = CheckLongIdent longIdent

                            if inFront then
                                Some synExpr.Range
                            else
                                // see comment below for SynExpr.DotSet
                                Some(unionRanges synExpr.Range r)

                    | SynExpr.Set(synExpr, synExpr2, range) ->
                        if SyntaxTraversal.rangeContainsPosLeftEdgeInclusive synExpr.Range pos then
                            traverseSynExpr synExpr
                        elif SyntaxTraversal.rangeContainsPosLeftEdgeInclusive synExpr2.Range pos then
                            traverseSynExpr synExpr2
                        else
                            Some range

                    | SynExpr.DotSet(synExpr, lid, synExpr2, _) ->
                        let (SynLongIdent(longIdent, _, _)) = lid

                        if SyntaxTraversal.rangeContainsPosLeftEdgeInclusive synExpr.Range pos then
                            traverseSynExpr synExpr
                        elif SyntaxTraversal.rangeContainsPosLeftEdgeInclusive synExpr2.Range pos then
                            traverseSynExpr synExpr2
                        else
                            let inFront, r = CheckLongIdent longIdent

                            if inFront then
                                Some synExpr.Range
                            else
                                // f(0).X.Y.Z
                                //       ^
                                //      -   r has this value
                                // ----     synExpr.Range has this value
                                // ------   we want this value
                                Some(unionRanges synExpr.Range r)

                    | SynExpr.DotNamedIndexedPropertySet(synExpr, lid, synExpr2, synExpr3, _) ->
                        let (SynLongIdent(longIdent, _, _)) = lid

                        if SyntaxTraversal.rangeContainsPosLeftEdgeInclusive synExpr.Range pos then
                            traverseSynExpr synExpr
                        elif SyntaxTraversal.rangeContainsPosLeftEdgeInclusive synExpr2.Range pos then
                            traverseSynExpr synExpr2
                        elif SyntaxTraversal.rangeContainsPosLeftEdgeInclusive synExpr3.Range pos then
                            traverseSynExpr synExpr3
                        else
                            let inFront, r = CheckLongIdent longIdent

                            if inFront then
                                Some synExpr.Range
                            else
                                Some(unionRanges synExpr.Range r)

                    // get this for e.g. "bar()."
                    | SynExpr.DiscardAfterMissingQualificationAfterDot(synExpr, _, _) ->
                        if SyntaxTraversal.rangeContainsPosLeftEdgeInclusive synExpr.Range pos then
                            traverseSynExpr synExpr
                        else
                            Some synExpr.Range

                    | SynExpr.FromParseError(synExpr, range) ->
                        if SyntaxTraversal.rangeContainsPosLeftEdgeInclusive synExpr.Range pos then
                            traverseSynExpr synExpr
                        else
                            Some range

                    | SynExpr.App(ExprAtomicFlag.NonAtomic, true, SynExpr.LongIdent(longDotId = SynLongIdent(id = [ ident ])), rhs, _) when
                        ident.idText = "op_ArrayLookup"
                        && not (SyntaxTraversal.rangeContainsPosLeftEdgeInclusive rhs.Range pos)
                        ->

                        match defaultTraverse expr with
                        | None ->
                            // (expr).(expr) is an ML-deprecated array lookup, but we want intellisense on the dot
                            // also want it for e.g. [|arr|].(0)
                            Some expr.Range
                        | x -> x // we found the answer deeper somewhere in the lhs

                    | SynExpr.Const(SynConst.Double _, range) -> Some range

                    | _ -> defaultTraverse expr
            }

        SyntaxTraversal.Traverse(pos, parsedInput, visitor)

    /// searches for the expression island suitable for the evaluation by the debugger
    let TryFindExpressionIslandInPosition (pos: pos, parsedInput) =
        let getLidParts (lid: LongIdent) =
            lid
            |> Seq.takeWhile (fun i -> posGeq pos i.idRange.Start)
            |> Seq.map (fun i -> i.idText)
            |> Seq.toList

        // tries to locate simple expression island
        // foundCandidate = false  means that we are looking for the candidate expression
        // foundCandidate = true - we found candidate (DotGet) and now drill down to the left part
        let rec TryGetExpression foundCandidate expr =
            match expr with
            | SynExpr.Paren(e, _, _, _) when foundCandidate -> TryGetExpression foundCandidate e

            | SynExpr.LongIdent(_isOptional, SynLongIdent(lid, _, _), _altNameRefCell, _m) -> getLidParts lid |> Some

            | SynExpr.DotGet(leftPart, _, SynLongIdent(lid, _, _), _) when (rangeContainsPos (rangeOfLid lid) pos) || foundCandidate ->
                // requested position is at the lid part of the DotGet
                // process left part and append result to the result of processing lid
                let leftPartResult = TryGetExpression true leftPart

                match leftPartResult with
                | Some leftPartResult -> [ yield! leftPartResult; yield! getLidParts lid ] |> Some
                | None -> None

            | SynExpr.FromParseError(synExpr, _) -> TryGetExpression foundCandidate synExpr

            | _ -> None

        let rec visitor =
            { new SyntaxVisitorBase<_>() with
                member _.VisitExpr(_, _traverseSynExpr, defaultTraverse, expr) =
                    if rangeContainsPos expr.Range pos then
                        match TryGetExpression false expr with
                        | Some parts -> parts |> String.concat "." |> Some
                        | _ -> defaultTraverse expr
                    else
                        None
            }

        SyntaxTraversal.Traverse(pos, parsedInput, visitor)

    let traverseLidOrElse pos (optExprIfLeftOfLongId: SynExpr option) (SynLongIdent(lid, dots, _) as lidwd) =
        let resultIfLeftOfLongId =
            match optExprIfLeftOfLongId with
            | None -> None
            | Some e -> Some(e.Range.End, posGeq lidwd.Range.Start pos)

        let dotSearch =
            dots
            |> List.mapi (fun i x -> i, x)
            |> List.rev
            |> List.tryFind (fun (_, m) -> posGt pos m.Start)

        match dotSearch with
        | None -> resultIfLeftOfLongId
        | Some(n, _) ->
            let flag =
                // foo.$
                (lid.Length = n + 1)
                ||
                // foo.$bar
                posGeq lid[n + 1].idRange.Start pos

            Some(lid[n].idRange.End, flag)

    // Given a cursor position here:
    //    f(x)   .   ident
    //                   ^
    // walk the AST to find the position here:
    //    f(x)   .   ident
    //       ^
    // On success, return Some (thatPos, boolTrueIfCursorIsAfterTheDotButBeforeTheIdentifier)
    // If there's no dot, return None, so for example
    //    foo
    //      ^
    // would return None
    // TODO would be great to unify this with GetRangeOfExprLeftOfDot above, if possible, as they are similar
    let TryFindExpressionASTLeftOfDotLeftOfCursor (pos, parsedInput) =
        let dive x = SyntaxTraversal.dive x
        let pick x = SyntaxTraversal.pick pos x

        let visitor =
            { new SyntaxVisitorBase<_>() with
                member _.VisitExpr(_, traverseSynExpr, defaultTraverse, expr) =
                    let pick = pick expr.Range
                    let traverseSynExpr, defaultTraverse, expr = traverseSynExpr, defaultTraverse, expr // for debugging: debugger does not get object expression params as local vars

                    if not (rangeContainsPos expr.Range pos) then
                        match expr with
                        | SynExpr.DiscardAfterMissingQualificationAfterDot(e, _, _m) ->
                            // This happens with e.g. "f(x)  .   $" when you bring up a completion list a few spaces after a dot.  The cursor is not 'in the parse tree',
                            // but the dive algorithm will dive down into this node, and this is the one case where we do want to give a result despite the cursor
                            // not properly being in a node.
                            match traverseSynExpr e with
                            | None -> Some(e.Range.End, false)
                            | r -> r
                        | _ ->
                            // This happens for e.g. "System.Console.[]$", where the ".[]" token is thrown away by the parser and we dive into the System.Console longId
                            // even though the cursor/dot is not in there.  In those cases we want to return None, because there is not really a dot completion before
                            // the cursor location.
                            None
                    else
                        match expr with
                        | SynExpr.LongIdent(longDotId = SynLongIdent([ id ], [], [ Some _ ])) -> defaultTraverse (SynExpr.Ident(id))
                        | SynExpr.LongIdent(_isOptional, lidwd, _altNameRefCell, _m) -> traverseLidOrElse pos None lidwd

                        | SynExpr.LongIdentSet(lidwd, exprRhs, _m) ->
                            [
                                dive lidwd lidwd.Range (traverseLidOrElse pos None)
                                dive exprRhs exprRhs.Range traverseSynExpr
                            ]
                            |> pick expr

                        | SynExpr.DotGet(exprLeft, mDot, lidwd, _m) ->
                            let afterDotBeforeLid = withStartEnd mDot.End lidwd.Range.Start mDot

                            [
                                dive exprLeft exprLeft.Range traverseSynExpr
                                dive exprLeft afterDotBeforeLid (fun e -> Some(e.Range.End, true))
                                dive lidwd lidwd.Range (traverseLidOrElse pos (Some exprLeft))
                            ]
                            |> pick expr

                        | SynExpr.DotSet(exprLeft, lidwd, exprRhs, _m) ->
                            [
                                dive exprLeft exprLeft.Range traverseSynExpr
                                dive lidwd lidwd.Range (traverseLidOrElse pos (Some exprLeft))
                                dive exprRhs exprRhs.Range traverseSynExpr
                            ]
                            |> pick expr

                        | SynExpr.Set(exprLeft, exprRhs, _m) ->
                            [
                                dive exprLeft exprLeft.Range traverseSynExpr
                                dive exprRhs exprRhs.Range traverseSynExpr
                            ]
                            |> pick expr

                        | SynExpr.NamedIndexedPropertySet(lidwd, exprIndexer, exprRhs, _m) ->
                            [
                                dive lidwd lidwd.Range (traverseLidOrElse pos None)
                                dive exprIndexer exprIndexer.Range traverseSynExpr
                                dive exprRhs exprRhs.Range traverseSynExpr
                            ]
                            |> pick expr

                        | SynExpr.DotNamedIndexedPropertySet(exprLeft, lidwd, exprIndexer, exprRhs, _m) ->
                            [
                                dive exprLeft exprLeft.Range traverseSynExpr
                                dive lidwd lidwd.Range (traverseLidOrElse pos (Some exprLeft))
                                dive exprIndexer exprIndexer.Range traverseSynExpr
                                dive exprRhs exprRhs.Range traverseSynExpr
                            ]
                            |> pick expr

                        | SynExpr.Const(SynConst.Double _, m) ->
                            if posEq m.End pos then
                                // the cursor is at the dot
                                Some(m.End, false)
                            else
                                // the cursor is left of the dot
                                None

                        | SynExpr.DiscardAfterMissingQualificationAfterDot(e, _, m) ->
                            match traverseSynExpr e with
                            | None ->
                                if posEq m.End pos then
                                    // the cursor is at the dot
                                    Some(e.Range.End, false)
                                else
                                    // the cursor is left of the dot
                                    None
                            | r -> r

                        | SynExpr.App(ExprAtomicFlag.NonAtomic, true, SynExpr.LongIdent(longDotId = SynLongIdent(id = [ ident ])), lhs, _m) when
                            ident.idText = "op_ArrayLookup"
                            && not (SyntaxTraversal.rangeContainsPosLeftEdgeInclusive lhs.Range pos)
                            ->
                            match defaultTraverse expr with
                            | None ->
                                // (expr).(expr) is an ML-deprecated array lookup, but we want intellisense on the dot
                                // also want it for e.g. [|arr|].(0)
                                Some(lhs.Range.End, false)
                            | x -> x // we found the answer deeper somewhere in the lhs
                        | _ -> defaultTraverse expr
            }

        SyntaxTraversal.Traverse(pos, parsedInput, visitor)

    let GetEntityKind (pos: pos, parsedInput: ParsedInput) : EntityKind option =
        let (|ConstructorPats|) pats =
            match pats with
            | SynArgPats.Pats ps -> ps
            | SynArgPats.NamePatPairs(pats = xs) -> List.map (fun (_, _, pat) -> pat) xs

        let inline isPosInRange range = rangeContainsPos range pos

        let inline ifPosInRange range f =
            if isPosInRange range then f () else None

        let rec walkImplFileInput (file: ParsedImplFileInput) =
            List.tryPick (walkSynModuleOrNamespace true) file.Contents

        and walkSynModuleOrNamespace isTopLevel inp =
            let (SynModuleOrNamespace(decls = decls; attribs = Attributes attrs; range = r)) =
                inp

            List.tryPick walkAttribute attrs
            |> Option.orElseWith (fun () -> ifPosInRange r (fun _ -> List.tryPick (walkSynModuleDecl isTopLevel) decls))

        and walkAttribute (attr: SynAttribute) =
            if isPosInRange attr.TypeName.Range then
                Some EntityKind.Attribute
            else
                None
            |> Option.orElseWith (fun () -> walkExprWithKind (Some EntityKind.Type) attr.ArgExpr)

        and walkTypar typar =
            let (SynTypar(ident, _, _)) = typar
            ifPosInRange ident.idRange (fun _ -> Some EntityKind.Type)

        and walkTyparDecl typarDecl =
            let (SynTyparDecl(Attributes attrs, typar, intersectionContraints, _)) = typarDecl

            List.tryPick walkAttribute attrs
            |> Option.orElseWith (fun () -> walkTypar typar)
            |> Option.orElseWith (fun () -> intersectionContraints |> List.tryPick walkType)

        and walkTypeConstraint cx =
            match cx with
            | SynTypeConstraint.WhereTyparDefaultsToType(t1, t2, _) -> walkTypar t1 |> Option.orElseWith (fun () -> walkType t2)
            | SynTypeConstraint.WhereTyparIsValueType(t, _) -> walkTypar t
            | SynTypeConstraint.WhereTyparIsReferenceType(t, _) -> walkTypar t
            | SynTypeConstraint.WhereTyparIsUnmanaged(t, _) -> walkTypar t
            | SynTypeConstraint.WhereTyparSupportsNull(t, _) -> walkTypar t
            | SynTypeConstraint.WhereTyparNotSupportsNull(genericName = t) -> walkTypar t
            | SynTypeConstraint.WhereTyparIsComparable(t, _) -> walkTypar t
            | SynTypeConstraint.WhereTyparIsEquatable(t, _) -> walkTypar t
            | SynTypeConstraint.WhereTyparSubtypeOfType(t, ty, _) -> walkTypar t |> Option.orElseWith (fun () -> walkType ty)
            | SynTypeConstraint.WhereTyparSupportsMember(TypesForTypar ts, sign, _) ->
                List.tryPick walkType ts |> Option.orElseWith (fun () -> walkMemberSig sign)
            | SynTypeConstraint.WhereTyparIsEnum(t, ts, _) -> walkTypar t |> Option.orElseWith (fun () -> List.tryPick walkType ts)
            | SynTypeConstraint.WhereTyparIsDelegate(t, ts, _) -> walkTypar t |> Option.orElseWith (fun () -> List.tryPick walkType ts)
            | SynTypeConstraint.WhereSelfConstrained(ts, _) -> walkType ts

        and walkPatWithKind (kind: EntityKind option) pat =
            match pat with
            | SynPat.Ands(pats, _) -> List.tryPick walkPat pats
            | SynPat.As(pat1, pat2, _) -> List.tryPick walkPat [ pat1; pat2 ]
            | SynPat.Typed(pat, t, _) -> walkPat pat |> Option.orElseWith (fun () -> walkType t)
            | SynPat.Attrib(pat, Attributes attrs, _) -> walkPat pat |> Option.orElseWith (fun () -> List.tryPick walkAttribute attrs)
            | SynPat.Or(pat1, pat2, _, _)
            | SynPat.ListCons(pat1, pat2, _, _) -> List.tryPick walkPat [ pat1; pat2 ]
            | SynPat.LongIdent(typarDecls = typars; argPats = ConstructorPats pats; range = r) ->
                ifPosInRange r (fun _ -> kind)
                |> Option.orElseWith (fun () ->
                    typars
                    |> Option.bind (fun (ValTyparDecls(typars, constraints, _)) ->
                        List.tryPick walkTyparDecl typars
                        |> Option.orElseWith (fun () -> List.tryPick walkTypeConstraint constraints)))
                |> Option.orElseWith (fun () -> List.tryPick walkPat pats)
            | SynPat.Tuple(elementPats = pats) -> List.tryPick walkPat pats
            | SynPat.Paren(pat, _) -> walkPat pat
            | SynPat.ArrayOrList(_, pats, _) -> List.tryPick walkPat pats
            | SynPat.IsInst(t, _) -> walkType t
            | SynPat.QuoteExpr(e, _) -> walkExpr e
            | _ -> None

        and walkPat = walkPatWithKind None

        and walkBinding bind =
            let (SynBinding(attributes = Attributes attrs; headPat = pat; returnInfo = returnInfo; expr = e)) =
                bind

            List.tryPick walkAttribute attrs
            |> Option.orElseWith (fun () -> walkPat pat)
            |> Option.orElseWith (fun () -> walkExpr e)
            |> Option.orElseWith (fun () ->
                match returnInfo with
                | Some(SynBindingReturnInfo(typeName = t)) -> walkType t
                | None -> None)

        and walkInterfaceImpl (SynInterfaceImpl(bindings = bindings)) = List.tryPick walkBinding bindings

        and walkType ty =
            match ty with
            | SynType.LongIdent ident ->
                // we protect it with try..with because System.Exception : rangeOfLidwd may raise
                // at FSharp.Compiler.Syntax.LongIdentWithDots.get_Range() in D:\j\workspace\release_ci_pa---3f142ccc\src\ast.fs: line 156
                try
                    ifPosInRange ident.Range (fun _ -> Some EntityKind.Type)
                with _ ->
                    None
            | SynType.App(ty, _, types, _, _, _, _) -> walkType ty |> Option.orElseWith (fun () -> List.tryPick walkType types)
            | SynType.LongIdentApp(_, _, _, types, _, _, _) -> List.tryPick walkType types
            | SynType.Tuple(path = segments) -> getTypeFromTuplePath segments |> List.tryPick walkType
            | SynType.Array(_, t, _) -> walkType t
            | SynType.Fun(argType = t1; returnType = t2) -> walkType t1 |> Option.orElseWith (fun () -> walkType t2)
            | SynType.WithGlobalConstraints(t, _, _) -> walkType t
            | SynType.WithNull(innerType = t)
            | SynType.HashConstraint(t, _) -> walkType t
            | SynType.Or(t1, t2, _, _) -> walkType t1 |> Option.orElseWith (fun () -> walkType t2)
            | SynType.MeasurePower(t, _, _) -> walkType t
            | SynType.Paren(t, _)
            | SynType.SignatureParameter(usedType = t) -> walkType t
            | SynType.StaticConstantExpr(e, _) -> walkExpr e
            | SynType.StaticConstantNamed(ident, value, _) -> List.tryPick walkType [ ident; value ]
            | SynType.Intersection(types = types) -> List.tryPick walkType types
            | SynType.StaticConstantNull _
            | SynType.Anon _
            | SynType.AnonRecd _
            | SynType.LongIdent _
            | SynType.Var _
            | SynType.StaticConstant _
            | SynType.FromParseError _ -> None

        and walkClause clause =
            let (SynMatchClause(pat = pat; whenExpr = e1; resultExpr = e2)) = clause

            walkPatWithKind (Some EntityKind.Type) pat
            |> Option.orElseWith (fun () -> walkExpr e2)
            |> Option.orElseWith (fun () -> Option.bind walkExpr e1)

        and walkExprWithKind parentKind expr =
            match expr with
            | SynExpr.LongIdent(longDotId = SynLongIdent(id = [ ident ]; trivia = [ Some _ ])) ->
                ifPosInRange ident.idRange (fun _ -> Some(EntityKind.FunctionOrValue false))

            | SynExpr.LongIdent(longDotId = SynLongIdent(dotRanges = dotRanges); range = r) ->
                match dotRanges with
                | [] when isPosInRange r ->
                    parentKind
                    |> Option.orElseWith (fun () -> Some(EntityKind.FunctionOrValue false))
                | firstDotRange :: _ ->
                    let firstPartRange =
                        mkRange "" r.Start (mkPos firstDotRange.StartLine (firstDotRange.StartColumn - 1))

                    if isPosInRange firstPartRange then
                        parentKind
                        |> Option.orElseWith (fun () -> Some(EntityKind.FunctionOrValue false))
                    else
                        None
                | _ -> None

            | SynExpr.LongIdentSet(expr = e)
            | SynExpr.DotGet(expr = e)
            | SynExpr.DotSet(targetExpr = e)
            | SynExpr.Set(targetExpr = e)
            | SynExpr.Lazy(expr = e)
            | SynExpr.DoBang(expr = e)
            | SynExpr.Do(expr = e)
            | SynExpr.Assert(expr = e)
            | SynExpr.ArrayOrListComputed(expr = e)
            | SynExpr.ComputationExpr(expr = e)
            | SynExpr.Lambda(body = e)
            | SynExpr.DotLambda(expr = e)
            | SynExpr.InferredUpcast(expr = e)
            | SynExpr.InferredDowncast(expr = e)
            | SynExpr.AddressOf(expr = e)
            | SynExpr.YieldOrReturn(expr = e)
            | SynExpr.YieldOrReturnFrom(expr = e)
            | SynExpr.Paren(expr = e)
            | SynExpr.Quote(quotedExpr = e)
            | SynExpr.Typed(expr = e) -> walkExprWithKind parentKind e

            | SynExpr.NamedIndexedPropertySet(expr1 = e1; expr2 = e2)
            | SynExpr.TryFinally(tryExpr = e1; finallyExpr = e2)
            | SynExpr.App(funcExpr = e1; argExpr = e2)
            | SynExpr.WhileBang(whileExpr = e1; doExpr = e2)
            | SynExpr.While(whileExpr = e1; doExpr = e2)
            | SynExpr.ForEach(enumExpr = e1; bodyExpr = e2)
            | SynExpr.DotIndexedGet(objectExpr = e1; indexArgs = e2)
            | SynExpr.DotIndexedSet(objectExpr = e1; indexArgs = e2)
            | SynExpr.JoinIn(lhsExpr = e1; rhsExpr = e2) ->
                walkExprWithKind parentKind e1
                |> Option.orElseWith (fun () -> walkExprWithKind parentKind e2)

            | SynExpr.New(expr = e; targetType = t)
            | SynExpr.TypeTest(expr = e; targetType = t)
            | SynExpr.Upcast(expr = e; targetType = t)
            | SynExpr.Downcast(expr = e; targetType = t) -> walkExprWithKind parentKind e |> Option.orElseWith (fun () -> walkType t)

            | Sequentials es
            | SynExpr.Tuple(exprs = es)
            | SynExpr.ArrayOrList(exprs = es) -> List.tryPick (walkExprWithKind parentKind) es

            | SynExpr.For(identBody = e1; toBody = e2; doBody = e3)
            | SynExpr.DotNamedIndexedPropertySet(targetExpr = e1; argExpr = e2; rhsExpr = e3) ->
                List.tryPick (walkExprWithKind parentKind) [ e1; e2; e3 ]

            | SynExpr.TryWith(tryExpr = e; withCases = clauses)
            | SynExpr.MatchBang(expr = e; clauses = clauses)
            | SynExpr.Match(expr = e; clauses = clauses) ->
                walkExprWithKind parentKind e
                |> Option.orElseWith (fun () -> List.tryPick walkClause clauses)

            | SynExpr.MatchLambda(matchClauses = clauses) -> List.tryPick walkClause clauses

            | SynExpr.Record(_, _, fields, r) ->
                ifPosInRange r (fun _ ->
                    fields
                    |> List.tryPick (fun (SynExprRecordField(expr = e)) -> e |> Option.bind (walkExprWithKind parentKind)))

            | SynExpr.ObjExpr(objType = ty; bindings = bindings; members = ms; extraImpls = ifaces) ->
                let bindings = unionBindingAndMembers bindings ms

                walkType ty
                |> Option.orElseWith (fun () -> List.tryPick walkBinding bindings)
                |> Option.orElseWith (fun () -> List.tryPick walkInterfaceImpl ifaces)

            | SynExpr.TypeApp(expr = e; typeArgs = tys) ->
                walkExprWithKind (Some EntityKind.Type) e
                |> Option.orElseWith (fun () -> List.tryPick walkType tys)

            | SynExpr.LetOrUse(bindings = bindings; body = e) ->
                List.tryPick walkBinding bindings
                |> Option.orElseWith (fun () -> walkExprWithKind parentKind e)

            | SynExpr.IfThenElse(ifExpr = e1; thenExpr = e2; elseExpr = e3) ->
                walkExprWithKind parentKind e1
                |> Option.orElseWith (fun () -> walkExprWithKind parentKind e2)
                |> Option.orElseWith (fun () ->
                    match e3 with
                    | None -> None
                    | Some e -> walkExprWithKind parentKind e)

            | SynExpr.Ident ident -> ifPosInRange ident.idRange (fun _ -> Some(EntityKind.FunctionOrValue false))

            | SynExpr.LetOrUseBang(rhs = e1; andBangs = es; body = e2) ->
                [
                    yield e1
                    for SynExprAndBang(body = eAndBang) in es do
                        yield eAndBang
                    yield e2
                ]
                |> List.tryPick (walkExprWithKind parentKind)

            | SynExpr.TraitCall(TypesForTypar ts, sign, e, _) ->
                List.tryPick walkType ts
                |> Option.orElseWith (fun () -> walkMemberSig sign)
                |> Option.orElseWith (fun () -> walkExprWithKind parentKind e)

            | _ -> None

        and walkExpr expr = walkExprWithKind None expr

        and walkSimplePat pat =
            match pat with
            | SynSimplePat.Attrib(pat, Attributes attrs, _) ->
                walkSimplePat pat
                |> Option.orElseWith (fun () -> List.tryPick walkAttribute attrs)
            | SynSimplePat.Typed(pat, t, _) -> walkSimplePat pat |> Option.orElseWith (fun () -> walkType t)
            | _ -> None

        and walkField synField =
            let (SynField(attributes = Attributes attrs; fieldType = t)) = synField
            List.tryPick walkAttribute attrs |> Option.orElseWith (fun () -> walkType t)

        and walkValSig synValSig =
            let (SynValSig(attributes = Attributes attrs; synType = t)) = synValSig
            List.tryPick walkAttribute attrs |> Option.orElseWith (fun () -> walkType t)

        and walkMemberSig membSig =
            match membSig with
            | SynMemberSig.Inherit(t, _) -> walkType t

            | SynMemberSig.Member(memberSig = vs) -> walkValSig vs

            | SynMemberSig.Interface(t, _) -> walkType t

            | SynMemberSig.ValField(f, _) -> walkField f

            | SynMemberSig.NestedType(nestedType = SynTypeDefnSig.SynTypeDefnSig(typeInfo = info; typeRepr = repr; members = memberSigs)) ->
                walkComponentInfo false info
                |> Option.orElseWith (fun () -> walkTypeDefnSigRepr repr)
                |> Option.orElseWith (fun () -> List.tryPick walkMemberSig memberSigs)

        and walkMember memb =
            match memb with
            | SynMemberDefn.AbstractSlot(slotSig = valSig) -> walkValSig valSig

            | SynMemberDefn.Member(binding, _) -> walkBinding binding

            | SynMemberDefn.GetSetMember(getBinding, setBinding, _, _) ->
                match getBinding, setBinding with
                | None, None -> None
                | Some binding, None
                | None, Some binding -> walkBinding binding
                | Some getBinding, Some setBinding -> walkBinding getBinding |> Option.orElseWith (fun () -> walkBinding setBinding)

            | SynMemberDefn.ImplicitCtor(attributes = Attributes attrs; ctorArgs = pat) ->
                List.tryPick walkAttribute attrs |> Option.orElseWith (fun _ -> walkPat pat)

            | SynMemberDefn.ImplicitInherit(t, e, _, _, _) -> walkType t |> Option.orElseWith (fun () -> walkExpr e)

            | SynMemberDefn.LetBindings(bindings, _, _, _) -> List.tryPick walkBinding bindings

            | SynMemberDefn.Interface(interfaceType = t; members = members) ->
                walkType t
                |> Option.orElseWith (fun () -> members |> Option.bind (List.tryPick walkMember))

            | SynMemberDefn.Inherit(baseType = Some baseType) -> walkType baseType
            | SynMemberDefn.Inherit(baseType = None) -> None
            | SynMemberDefn.ValField(fieldInfo = field) -> walkField field

            | SynMemberDefn.NestedType(tdef, _, _) -> walkTypeDefn tdef

            | SynMemberDefn.AutoProperty(attributes = Attributes attrs; typeOpt = t; synExpr = e) ->
                List.tryPick walkAttribute attrs
                |> Option.orElseWith (fun () -> Option.bind walkType t)
                |> Option.orElseWith (fun () -> walkExpr e)

            | _ -> None

        and walkEnumCase (SynEnumCase(attributes = Attributes attrs)) = List.tryPick walkAttribute attrs

        and walkUnionCaseType inp =
            match inp with
            | SynUnionCaseKind.Fields fields -> List.tryPick walkField fields
            | SynUnionCaseKind.FullType(t, _) -> walkType t

        and walkUnionCase synUnionCase =
            let (SynUnionCase(attributes = Attributes attrs; caseType = t)) = synUnionCase

            List.tryPick walkAttribute attrs
            |> Option.orElseWith (fun () -> walkUnionCaseType t)

        and walkTypeDefnSimple synTypeDefn =
            match synTypeDefn with
            | SynTypeDefnSimpleRepr.Enum(cases, _) -> List.tryPick walkEnumCase cases
            | SynTypeDefnSimpleRepr.Union(_, cases, _) -> List.tryPick walkUnionCase cases
            | SynTypeDefnSimpleRepr.Record(_, fields, _) -> List.tryPick walkField fields
            | SynTypeDefnSimpleRepr.TypeAbbrev(_, t, _) -> walkType t
            | _ -> None

        and walkComponentInfo isModule compInfo =
            let (SynComponentInfo(Attributes attrs, TyparsAndConstraints(typars, cs1), cs2, _, _, _, _, r)) =
                compInfo

            let constraints = cs1 @ cs2

            if isModule then
                None
            else
                ifPosInRange r (fun _ -> Some EntityKind.Type)
            |> Option.orElseWith (fun () ->
                List.tryPick walkAttribute attrs
                |> Option.orElseWith (fun () -> List.tryPick walkTyparDecl typars)
                |> Option.orElseWith (fun () -> List.tryPick walkTypeConstraint constraints))

        and walkTypeDefnRepr inp =
            match inp with
            | SynTypeDefnRepr.ObjectModel(_, defns, _) -> List.tryPick walkMember defns
            | SynTypeDefnRepr.Simple(defn, _) -> walkTypeDefnSimple defn
            | SynTypeDefnRepr.Exception _ -> None

        and walkTypeDefnSigRepr inp =
            match inp with
            | SynTypeDefnSigRepr.ObjectModel(_, defns, _) -> List.tryPick walkMemberSig defns
            | SynTypeDefnSigRepr.Simple(defn, _) -> walkTypeDefnSimple defn
            | SynTypeDefnSigRepr.Exception _ -> None

        and walkTypeDefn typeDefn =
            let (SynTypeDefn(typeInfo = info; typeRepr = repr; members = members)) = typeDefn

            walkComponentInfo false info
            |> Option.orElseWith (fun () -> walkTypeDefnRepr repr)
            |> Option.orElseWith (fun () -> List.tryPick walkMember members)

        and walkSynModuleDecl isTopLevel (decl: SynModuleDecl) =
            match decl with
            | SynModuleDecl.NamespaceFragment fragment -> walkSynModuleOrNamespace isTopLevel fragment

            | SynModuleDecl.NestedModule(moduleInfo = info; decls = modules; range = range) ->
                walkComponentInfo true info
                |> Option.orElseWith (fun () -> ifPosInRange range (fun _ -> List.tryPick (walkSynModuleDecl false) modules))

            | SynModuleDecl.Open _ -> None

            | SynModuleDecl.Let(_, bindings, _) -> List.tryPick walkBinding bindings

            | SynModuleDecl.Expr(expr, _) -> walkExpr expr

            | SynModuleDecl.Types(types, _) -> List.tryPick walkTypeDefn types

            | _ -> None

        match parsedInput with
        | ParsedInput.SigFile _ -> None
        | ParsedInput.ImplFile input -> walkImplFileInput input

    //--------------------------------------------------------------------------------------------
    // TryGetCompletionContext

    /// Matches the most nested [< and >] pair.
    let insideAttributeApplicationRegex =
        Regex(@"(?<=\[\<)(?<attribute>(.*?))(?=\>\])", RegexOptions.Compiled ||| RegexOptions.ExplicitCapture)

    // Categorise via attributes
    let (|Class|Interface|Struct|Unknown|Invalid|) synAttributes =
        let (|SynAttr|_|) name (attr: SynAttribute) =
            match attr with
            | { TypeName = SynLongIdent([ x ], _, _) } when x.idText = name -> Some()
            | _ -> None

        let rec getKind isClass isInterface isStruct attrs =
            match attrs with
            | [] -> isClass, isInterface, isStruct
            | SynAttr "Class" :: xs -> getKind true isInterface isStruct xs
            | SynAttr "AbstractClass" :: xs -> getKind true isInterface isStruct xs
            | SynAttr "Interface" :: xs -> getKind isClass true isStruct xs
            | SynAttr "Struct" :: xs -> getKind isClass isInterface true xs
            | _ :: xs -> getKind isClass isInterface isStruct xs

        match getKind false false false synAttributes with
        | false, false, false -> Unknown
        | true, false, false -> Class
        | false, true, false -> Interface
        | false, false, true -> Struct
        | _ -> Invalid

    let GetCompletionContextForInheritSynMember (compInfo, typeDefnKind: SynTypeDefnKind, completionPath) =

        let (SynComponentInfo(attributes = Attributes synAttributes)) = compInfo

        let success k =
            Some(CompletionContext.Inherit(k, completionPath))

        // if kind is specified - take it
        // if kind is non-specified
        //  - try to obtain it from attribute
        //      - if no attributes present - infer kind from members
        match typeDefnKind with
        | SynTypeDefnKind.Class ->
            match synAttributes with
            | Class
            | Unknown -> success InheritanceContext.Class
            | _ -> Some CompletionContext.Invalid // non-matching attributes
        | SynTypeDefnKind.Interface ->
            match synAttributes with
            | Interface
            | Unknown -> success InheritanceContext.Interface
            | _ -> Some CompletionContext.Invalid // non-matching attributes
        | SynTypeDefnKind.Struct ->
            // display nothing for structs
            Some CompletionContext.Invalid
        | SynTypeDefnKind.Unspecified ->
            match synAttributes with
            | Class -> success InheritanceContext.Class
            | Interface -> success InheritanceContext.Interface
            | Unknown ->
                // user do not specify kind explicitly or via attributes
                success InheritanceContext.Unknown
            | _ ->
                // unable to uniquely detect kind from the attributes - return invalid context
                Some CompletionContext.Invalid
        | _ -> None

    let (|Operator|_|) name e =
        match e with
        | SynExpr.App(ExprAtomicFlag.NonAtomic,
                      false,
                      SynExpr.App(ExprAtomicFlag.NonAtomic, true, SynExpr.LongIdent(longDotId = SynLongIdent(id = [ ident ])), lhs, _),
                      rhs,
                      _) when ident.idText = name -> Some(lhs, rhs)
        | _ -> None

    // checks if we are in a range operator
    let isAtRangeOp (p: SyntaxVisitorPath) =
        match p with
        | SyntaxNode.SynExpr(SynExpr.IndexRange _) :: _ -> true
        | _ -> false

    let (|Setter|_|) e =
        match e with
        | Operator "op_Equality" (SynExpr.Ident id, _) -> Some id
        | _ -> None

    let posAfterRangeAndBetweenSpaces (lineStr: string) (m: range) pos =
        let rec loop max i =
            if i >= lineStr.Length || i >= max then true
            elif Char.IsWhiteSpace lineStr[i] then loop max (i + 1)
            else false

        posGt pos m.End && pos.Line = m.End.Line && loop pos.Column m.End.Column

    let rangeContainsPosOrIsSpacesBetweenRangeAndPos (lineStr: string) m pos =
        rangeContainsPos m pos
        // pos is before m
        || posLt pos m.Start
        || posAfterRangeAndBetweenSpaces lineStr m pos

    let findSetters argList =
        match argList with
        | SynExpr.Paren(SynExpr.Tuple(false, parameters, _, _), _, _, _) ->
            let setters = HashSet()

            for p in parameters do
                match p with
                | Setter id -> ignore (setters.Add id.idText)
                | _ -> ()

            setters
        | _ -> emptyStringSet

    let endOfLastIdent (lid: SynLongIdent) =
        let last = List.last lid.LongIdent
        last.idRange.End

    let endOfClosingTokenOrLastIdent (mClosing: range option) (lid: SynLongIdent) =
        match mClosing with
        | Some m -> m.End
        | None -> endOfLastIdent lid

    let endOfClosingTokenOrIdent (mClosing: range option) (id: Ident) =
        match mClosing with
        | Some m -> m.End
        | None -> id.idRange.End

    let (|NewObjectOrMethodCall|_|) e =
        match e with
        | SynExpr.New(_, SynType.LongIdent typeName, arg, _) ->
            // new A()
            Some(endOfLastIdent typeName, findSetters arg)

        | SynExpr.New(_, SynType.App(StripParenTypes(SynType.LongIdent typeName), _, _, _, mGreaterThan, _, _), arg, _) ->
            // new A<_>()
            Some(endOfClosingTokenOrLastIdent mGreaterThan typeName, findSetters arg)

        | SynExpr.App(_, false, SynExpr.Ident id, arg, _) ->
            // A()
            Some(id.idRange.End, findSetters arg)

        | SynExpr.App(_, false, SynExpr.TypeApp(SynExpr.Ident id, _, _, _, mGreaterThan, _, _), arg, _) ->
            // A<_>()
            Some(endOfClosingTokenOrIdent mGreaterThan id, findSetters arg)

        | SynExpr.App(_, false, SynExpr.LongIdent(_, lid, _, _), arg, _) ->
            // A.B()
            Some(endOfLastIdent lid, findSetters arg)

        | SynExpr.App(_, false, SynExpr.TypeApp(SynExpr.LongIdent(_, lid, _, _), _, _, _, mGreaterThan, _, _), arg, _) ->
            // A.B<_>()
            Some(endOfClosingTokenOrLastIdent mGreaterThan lid, findSetters arg)
        | _ -> None

    let isOnTheRightOfComma pos (elements: SynExpr list) (commas: range list) current =
        let rec loop elements (commas: range list) =
            match elements with
            | x :: xs ->
                match commas with
                | c :: cs ->
                    if x === current then
                        posLt c.End pos || posEq c.End pos
                    else
                        loop xs cs
                | _ -> false
            | _ -> false

        loop elements commas

    let (|PartOfParameterList|_|) pos precedingArgument path =
        match path with
        | SyntaxNode.SynExpr(SynExpr.Paren _) :: SyntaxNode.SynExpr(NewObjectOrMethodCall args) :: _ ->
            if Option.isSome precedingArgument then None else Some args
        | SyntaxNode.SynExpr(SynExpr.Tuple(false, elements, commas, _)) :: SyntaxNode.SynExpr(SynExpr.Paren _) :: SyntaxNode.SynExpr(NewObjectOrMethodCall args) :: _ ->
            match precedingArgument with
            | None -> Some args
            | Some e ->
                // if expression is passed then
                // 1. find it in among elements of the tuple
                // 2. find corresponding comma
                // 3. check that current position is past the comma
                // this is used for cases like (a = something-here.) if the cursor is after .
                // in this case this is not object initializer completion context
                if isOnTheRightOfComma pos elements commas e then
                    Some args
                else
                    None
        | _ -> None

    let rec parseLidAux pos plid (parts: Ident list) (dots: range list) =
        match parts, dots with
        | [], _ -> Some(plid, None)
        | x :: xs, ds ->
            if rangeContainsPos x.idRange pos then
                // pos lies with the range of current identifier
                let s = x.idText.Substring(0, pos.Column - x.idRange.Start.Column)
                let residue = if s.Length <> 0 then Some s else None
                Some(plid, residue)
            elif posGt x.idRange.Start pos then
                // can happen if caret is placed after dot but before the existing identifier A. $ B
                // return accumulated plid with no residue
                Some(plid, None)
            else
                match ds with
                | [] ->
                    // pos lies after the id and no dots found - return accumulated plid and current id as residue
                    Some(plid, Some x.idText)
                | d :: ds ->
                    if posGeq pos d.End then
                        // pos lies after the dot - proceed to the next identifier
                        parseLidAux pos (x.idText :: plid) xs ds
                    else
                        // pos after the id but before the dot
                        // A $.B - return nothing
                        None

    let parseLid pos (SynLongIdent(lid, dots, _)) =
        match parseLidAux pos [] lid dots with
        | Some(parts, residue) -> Some((List.rev parts), residue)
        | None -> None

    /// Try to determine completion context at the given position within in an attribute using approximate analysis based on line text matching
    let TryGetCompletionContextOfAttributes (pos: pos, lineStr: string) : CompletionContext option =
        // Uncompleted attribute applications are not presented in the AST in any way. So, we have to parse source string.
        let cutLeadingAttributes (str: string) =
            // cut off leading attributes, i.e. we cut "[<A1; A2; >]" to " >]"
            match str.LastIndexOf ';' with
            | -1 -> str
            | idx when idx < str.Length -> str[idx + 1 ..].TrimStart()
            | _ -> ""

        let isLongIdent (lid: string) =
            lid |> Seq.forall (fun c -> IsIdentifierPartCharacter c || c = '.' || c = ':') // ':' may occur in "[<type: AnAttribute>]"

        // match the most nested paired [< and >] first
        let matches =
            insideAttributeApplicationRegex.Matches lineStr
            |> Seq.cast<Match>
            |> Seq.filter (fun m -> m.Index <= pos.Column && m.Index + m.Length >= pos.Column)
            |> Seq.toArray

        if matches.Length > 0 then
            matches
            |> Seq.tryPick (fun m ->
                let g = m.Groups["attribute"]
                let col = pos.Column - g.Index

                if col >= 0 && col < g.Length then
                    let str = g.Value.Substring(0, col).TrimStart() // cut other rhs attributes
                    let str = cutLeadingAttributes str

                    if isLongIdent str then
                        Some CompletionContext.AttributeApplication
                    else
                        None
                else
                    None)
        else
            // Paired [< and >] were not found, try to determine that we are after [< without closing >]
            match lineStr.LastIndexOf("[<", StringComparison.Ordinal) with
            | -1 -> None
            | openParenIndex when pos.Column >= openParenIndex + 2 ->
                let str = lineStr[openParenIndex + 2 .. pos.Column - 1].TrimStart()
                let str = cutLeadingAttributes str

                if isLongIdent str then
                    Some CompletionContext.AttributeApplication
                else
                    None
            | _ -> None

    // In member, function and lambda definitions (but not in match clauses) we suppress completions on outer identifiers
    //
    // fun x| ->
    // member _.X (a| ) =
    // let f x| =
    //
    // As soon as union case deconstruction is used, we *do* want to see completions on identifiers, in particular to suggest identifier names
    //
    // fun (SingleCase (v1, v| )) ->
    // member _.X (SingleCase (v1, v| )) =
    // let f (SingleCase (v1, v| )) =
    //
    let rec TryGetCompletionContextInPattern suppressIdentifierCompletions (pat: SynPat) previousContext pos =
        match pat with
        | SynPat.LongIdent(longDotId = id) when rangeContainsPos id.Range pos -> Some(CompletionContext.Pattern PatternContext.Other)
        | SynPat.LongIdent(argPats = SynArgPats.NamePatPairs(pats = pats; range = mPairs); longDotId = caseId; range = m) when
            rangeContainsPos m pos
            ->
            pats
            |> List.tryPick (fun (fieldId, _, pat) ->
                if rangeContainsPos fieldId.idRange pos then
                    let referencedFields = pats |> List.map (fun (id, _, _) -> id.idText)
                    Some(CompletionContext.Pattern(PatternContext.UnionCaseFieldIdentifier(referencedFields, caseId.Range)))
                else
                    let context = Some(PatternContext.NamedUnionCaseField(fieldId.idText, caseId.Range))
                    TryGetCompletionContextInPattern suppressIdentifierCompletions pat context pos)
            |> Option.orElseWith (fun () ->
                // Last resort - check for fun (Case (item1 = a; | )) ->
                // That is, pos is after the last pair and still within parentheses
                if rangeBeforePos mPairs pos then
                    let referencedFields = pats |> List.map (fun (id, _, _) -> id.idText)
                    Some(CompletionContext.Pattern(PatternContext.UnionCaseFieldIdentifier(referencedFields, caseId.Range)))
                else
                    None)
        | SynPat.LongIdent(argPats = SynArgPats.Pats pats; longDotId = id; range = m) when rangeContainsPos m pos ->
            match pats with

            // fun (Some v| ) ->
            | [ SynPat.Named _ ] -> Some(CompletionContext.Pattern(PatternContext.PositionalUnionCaseField(None, true, id.Range)))

            // fun (Case (| )) ->
            | [ SynPat.Paren(SynPat.Const(SynConst.Unit, _), m) ] when rangeContainsPos m pos ->
                Some(CompletionContext.Pattern(PatternContext.PositionalUnionCaseField(Some 0, true, id.Range)))

            // fun (Case (a| )) ->
            // This could either be the first positional field pattern or the user might want to use named pairs
            | [ SynPat.Paren(SynPat.Named _, _) ] ->
                Some(CompletionContext.Pattern(PatternContext.PositionalUnionCaseField(Some 0, true, id.Range)))

            // fun (Case (a| , b)) ->
            | [ SynPat.Paren(SynPat.Tuple(elementPats = pats) as pat, _) ] ->
                let context =
                    Some(PatternContext.PositionalUnionCaseField(Some 0, pats.Length = 1, id.Range))

                TryGetCompletionContextInPattern false pat context pos

            | _ ->
                pats
                |> List.tryPick (fun pat -> TryGetCompletionContextInPattern false pat None pos)
        | SynPat.Record(fieldPats = pats; range = m) when rangeContainsPos m pos ->
            pats
            |> List.tryPick (fun ((_, fieldId), _, pat) ->
                if rangeContainsPos fieldId.idRange pos then
                    let referencedFields = pats |> List.map (fun ((_, x), _, _) -> x.idText, x.idRange)
                    Some(CompletionContext.Pattern(PatternContext.RecordFieldIdentifier referencedFields))
                elif rangeContainsPos pat.Range pos then
                    TryGetCompletionContextInPattern false pat None pos
                else
                    None)
            |> Option.orElseWith (fun () ->
                // Last resort - check for fun { Field1 = a; F| } ->
                // That is, pos is after the last field and still within braces
                if
                    pats
                    |> List.forall (fun (_, mEquals, pat) ->
                        match mEquals, pat with
                        | Some mEquals, SynPat.Wild mPat -> rangeBeforePos mEquals pos && mPat.StartColumn <> mPat.EndColumn
                        | Some mEquals, _ -> rangeBeforePos mEquals pos
                        | _ -> false)
                then
                    let referencedFields = pats |> List.map (fun ((_, x), _, _) -> x.idText, x.idRange)
                    Some(CompletionContext.Pattern(PatternContext.RecordFieldIdentifier referencedFields))
                else
                    Some(CompletionContext.Pattern PatternContext.Other))
        | SynPat.Ands(pats = pats)
        | SynPat.ArrayOrList(elementPats = pats) ->
            pats
            |> List.tryPick (fun pat -> TryGetCompletionContextInPattern false pat None pos)
        | SynPat.Tuple(elementPats = pats; commaRanges = commas; range = m) ->
            pats
            |> List.indexed
            |> List.tryPick (fun (i, pat) ->
                let context =
                    match previousContext with
                    | Some(PatternContext.PositionalUnionCaseField(_, isTheOnlyField, caseIdRange)) ->
                        Some(PatternContext.PositionalUnionCaseField(Some i, isTheOnlyField, caseIdRange))
                    | _ ->
                        // No preceding LongIdent => this is a tuple deconstruction
                        None

                TryGetCompletionContextInPattern suppressIdentifierCompletions pat context pos)
            |> Option.orElseWith (fun () ->
                // Last resort - check for fun (Case (item1 = a, | )) ->
                // That is, pos is after the last comma and before the end of the tuple
                match previousContext, List.tryLast commas with
                | Some(PatternContext.PositionalUnionCaseField(_, isTheOnlyField, caseIdRange)), Some mComma when
                    rangeBeforePos mComma pos && rangeContainsPos m pos
                    ->
                    Some(
                        CompletionContext.Pattern(
                            PatternContext.PositionalUnionCaseField(Some(pats.Length - 1), isTheOnlyField, caseIdRange)
                        )
                    )
                | _ -> None)
        | SynPat.Named(range = m) when rangeContainsPos m pos ->
            if suppressIdentifierCompletions then
                Some CompletionContext.Invalid
            else
                previousContext
                |> Option.defaultValue PatternContext.Other
                |> CompletionContext.Pattern
                |> Some
        | SynPat.FromParseError(pat = pat)
        | SynPat.Attrib(pat = pat) -> TryGetCompletionContextInPattern suppressIdentifierCompletions pat previousContext pos
        | SynPat.Paren(pat, _) -> TryGetCompletionContextInPattern suppressIdentifierCompletions pat None pos
        | SynPat.ListCons(lhsPat = pat1; rhsPat = pat2)
        | SynPat.As(lhsPat = pat1; rhsPat = pat2)
        | SynPat.Or(lhsPat = pat1; rhsPat = pat2) ->
            TryGetCompletionContextInPattern suppressIdentifierCompletions pat1 None pos
            |> Option.orElseWith (fun () -> TryGetCompletionContextInPattern suppressIdentifierCompletions pat2 None pos)
        | SynPat.IsInst(_, m) when rangeContainsPos m pos -> Some CompletionContext.Type
        | SynPat.Wild m when rangeContainsPos m pos && m.StartColumn <> m.EndColumn -> Some CompletionContext.Invalid
        | SynPat.Typed(pat = pat; targetType = synType) ->
            if rangeContainsPos pat.Range pos then
                TryGetCompletionContextInPattern suppressIdentifierCompletions pat previousContext pos
            elif rangeContainsPos synType.Range pos then
                Some CompletionContext.Type
            else
                None
        | _ -> None

    /// Try to determine completion context for the given pair (row, columns)
    let TryGetCompletionContext (pos, parsedInput: ParsedInput, lineStr: string) : CompletionContext option =

        let visitor =
            { new SyntaxVisitorBase<_>() with
                member _.VisitExpr(path, _, defaultTraverse, expr) =

                    if isAtRangeOp path then
                        match defaultTraverse expr with
                        | None -> Some CompletionContext.RangeOperator // nothing was found - report that we were in the context of range operator
                        | x -> x // ok, we found something - return it
                    else
                        match expr with
                        // new A($)
                        | SynExpr.Const(SynConst.Unit, m) when rangeContainsPos m pos ->
                            match path with
                            | SyntaxNode.SynExpr(NewObjectOrMethodCall args) :: _ -> Some(CompletionContext.ParameterList args)
                            | _ -> defaultTraverse expr

                        // new (... A$)
                        | SynExpr.Ident id
                        | SynExpr.LongIdent(longDotId = SynLongIdent([ id ], [], [ Some _ ])) when id.idRange.End = pos ->
                            match path with
                            | PartOfParameterList pos None args -> Some(CompletionContext.ParameterList args)
                            | _ -> defaultTraverse expr

                        // new (A$ = 1)
                        // new (A = 1, $)
                        | Setter id when id.idRange.End = pos || rangeBeforePos expr.Range pos ->
                            let precedingArgument = if id.idRange.End = pos then None else Some expr

                            match path with
                            | PartOfParameterList pos precedingArgument args -> Some(CompletionContext.ParameterList args)
                            | _ -> defaultTraverse expr

                        | SynExpr.Record(None, None, [], _) -> Some(CompletionContext.RecordField RecordContext.Empty)

                        // Unchecked.defaultof<str$>
                        | SynExpr.TypeApp(typeArgsRange = range) when rangeContainsPos range pos -> Some CompletionContext.Type

                        // fun (Some v$ ) ->
                        | SynExpr.Lambda(parsedData = Some(pats, _)) ->
                            pats
                            |> List.tryPick (fun pat -> TryGetCompletionContextInPattern true pat None pos)
                            |> Option.orElseWith (fun () -> defaultTraverse expr)

                        // { new | }
                        | SynExpr.ComputationExpr(expr = SynExpr.ArbitraryAfterError _) when
                            lineStr.Trim().Split(' ') |> Array.contains "new"
                            ->
                            Some(CompletionContext.Inherit(InheritanceContext.Unknown, ([], None)))

                        | _ -> defaultTraverse expr

                member _.VisitRecordField(path, copyOpt, field) =
                    let contextFromTreePath completionPath =
                        // detect records usage in constructor
                        match path with
                        | SyntaxNode.SynExpr _ :: SyntaxNode.SynBinding _ :: SyntaxNode.SynMemberDefn _ :: SyntaxNode.SynTypeDefn(SynTypeDefn(
                            typeInfo = SynComponentInfo(longId = [ id ]))) :: _ -> RecordContext.Constructor(id.idText)

                        | SyntaxNode.SynExpr(SynExpr.Record(None, _, fields, _)) :: _ ->
                            let isFirstField =
                                match field, fields with
                                | Some contextLid, SynExprRecordField(fieldName = lid, _) :: _ -> contextLid.Range = lid.Range
                                | _ -> false

                            RecordContext.New(completionPath, isFirstField)

                        // Unfinished `{ xxx }` expression considered a record field by the tree visitor.
                        | SyntaxNode.SynExpr(SynExpr.ComputationExpr _) :: _ -> RecordContext.New(completionPath, true)

                        | _ -> RecordContext.New(completionPath, false)

                    match field with
                    | Some field ->
                        match parseLid pos field with
                        | Some completionPath ->
                            let recordContext =
                                match copyOpt with
                                | Some(s: SynExpr) -> RecordContext.CopyOnUpdate(s.Range, completionPath)
                                | None -> contextFromTreePath completionPath

                            Some(CompletionContext.RecordField recordContext)
                        | None -> None
                    | None ->
                        let recordContext =
                            match copyOpt with
                            | Some s -> RecordContext.CopyOnUpdate(s.Range, ([], None))
                            | None -> contextFromTreePath ([], None)

                        Some(CompletionContext.RecordField recordContext)

                member _.VisitInheritSynMemberDefn(_, componentInfo, typeDefnKind, synType, _, _) =
                    match synType with
                    | SynType.LongIdent lidwd ->
                        match parseLid pos lidwd with
                        | Some completionPath -> GetCompletionContextForInheritSynMember(componentInfo, typeDefnKind, completionPath)
                        | None -> Some CompletionContext.Invalid // A $ .B -> no completion list

                    | _ -> None

                member _.VisitBinding
                    (
                        path,
                        defaultTraverse,
                        (SynBinding(headPat = headPat; trivia = trivia; returnInfo = returnInfo) as synBinding)
                    ) =

                    let isOverrideOrMember leadingKeyword =
                        match leadingKeyword with
                        | SynLeadingKeyword.Override _
                        | SynLeadingKeyword.Member _ -> true
                        | _ -> false

                    let isStaticMember leadingKeyword =
                        match leadingKeyword with
                        | SynLeadingKeyword.StaticMember _ -> true
                        | _ -> false

                    let isMember leadingKeyword =
                        match leadingKeyword with
                        | SynLeadingKeyword.Member _ -> true
                        | _ -> false

                    let overrideContext path (mOverride: range) hasThis isStatic isMember =
                        match path with
                        | _ :: SyntaxNode.SynTypeDefn(SynTypeDefn(typeInfo = SynComponentInfo(longId = [ enclosingType ]))) :: _ when
                            not isMember
                            ->
                            Some(
                                CompletionContext.MethodOverride(
                                    MethodOverrideCompletionContext.Class,
                                    enclosingType.idRange,
                                    mOverride.StartColumn,
                                    hasThis,
                                    isStatic
                                )
                            )
                        | SyntaxNode.SynMemberDefn(SynMemberDefn.Interface(interfaceType = ty)) :: SyntaxNode.SynTypeDefn(SynTypeDefn(
                            typeInfo = SynComponentInfo(longId = [ enclosingType ]))) :: _
                        | _ :: SyntaxNode.SynMemberDefn(SynMemberDefn.Interface(interfaceType = ty)) :: SyntaxNode.SynTypeDefn(SynTypeDefn(
                            typeInfo = SynComponentInfo(longId = [ enclosingType ]))) :: _ ->
                            let ty =
                                match ty with
                                | SynType.App(typeName = ty) -> ty
                                | _ -> ty

                            Some(
                                CompletionContext.MethodOverride(
                                    MethodOverrideCompletionContext.Interface ty.Range,
                                    enclosingType.idRange,
                                    mOverride.StartColumn,
                                    hasThis,
                                    isStatic
                                )
                            )
                        | SyntaxNode.SynMemberDefn(SynMemberDefn.Interface(interfaceType = ty)) :: (SyntaxNode.SynExpr(SynExpr.ObjExpr _) as expr) :: _
                        | _ :: SyntaxNode.SynMemberDefn(SynMemberDefn.Interface(interfaceType = ty)) :: (SyntaxNode.SynExpr(SynExpr.ObjExpr _) as expr) :: _ ->
                            let ty =
                                match ty with
                                | SynType.App(typeName = ty) -> ty
                                | _ -> ty

                            Some(
                                CompletionContext.MethodOverride(
                                    MethodOverrideCompletionContext.ObjExpr expr.Range,
                                    ty.Range,
                                    mOverride.StartColumn,
                                    hasThis,
                                    isStatic
                                )
                            )
                        | SyntaxNode.SynExpr(SynExpr.ObjExpr(objType = ty)) as expr :: _ ->
                            let ty =
                                match ty with
                                | SynType.App(typeName = ty) -> ty
                                | _ -> ty

                            Some(
                                CompletionContext.MethodOverride(
                                    MethodOverrideCompletionContext.ObjExpr expr.Range,
                                    ty.Range,
                                    mOverride.StartColumn,
                                    hasThis,
                                    isStatic
                                )
                            )
                        | _ -> Some CompletionContext.Invalid

                    match returnInfo with
                    | Some(SynBindingReturnInfo(range = m)) when rangeContainsPosOrIsSpacesBetweenRangeAndPos lineStr m pos ->
                        Some CompletionContext.Type
                    | _ ->
                        match headPat with

                        // static member |
                        | SynPat.FromParseError _ when isStaticMember trivia.LeadingKeyword ->
                            overrideContext path trivia.LeadingKeyword.Range false true false

                        // override |
                        | SynPat.FromParseError _ when isOverrideOrMember trivia.LeadingKeyword && lineStr.[pos.Column - 1] = ' ' ->
                            overrideContext path trivia.LeadingKeyword.Range false false (isMember trivia.LeadingKeyword)

                        // override _.|
                        | SynPat.FromParseError _ when isOverrideOrMember trivia.LeadingKeyword ->
                            overrideContext path trivia.LeadingKeyword.Range true false (isMember trivia.LeadingKeyword)

                        // override this.|
                        | SynPat.Named(ident = SynIdent(ident = selfId)) when
                            isOverrideOrMember trivia.LeadingKeyword && selfId.idRange.End.IsAdjacentTo pos
                            ->
                            overrideContext path trivia.LeadingKeyword.Range true false (isMember trivia.LeadingKeyword)

                        // override this.ToStr|
                        | SynPat.LongIdent(longDotId = SynLongIdent(id = [ _; methodId ])) when
                            isOverrideOrMember trivia.LeadingKeyword
                            && rangeContainsPos methodId.idRange pos
                            ->
                            overrideContext path trivia.LeadingKeyword.Range true false (isMember trivia.LeadingKeyword)

                        // static member A|
                        | SynPat.LongIdent(longDotId = SynLongIdent(id = [ methodId ])) when
                            isStaticMember trivia.LeadingKeyword && rangeContainsPos methodId.idRange pos
                            ->
                            overrideContext path trivia.LeadingKeyword.Range false true false

                        | SynPat.LongIdent(longDotId = lidwd; argPats = SynArgPats.Pats pats; range = m) when rangeContainsPos m pos ->
                            if rangeContainsPos lidwd.Range pos then
                                // let fo|o x = ()
                                Some CompletionContext.Invalid
                            else
                                pats
                                |> List.tryPick (fun pat -> TryGetCompletionContextInPattern true pat None pos)
                                |> Option.orElseWith (fun () -> defaultTraverse synBinding)

                        | SynPat.Named(range = range)
                        | SynPat.As(_, SynPat.Named(range = range), _) when rangeContainsPos range pos ->
                            // let fo|o = 1
                            Some CompletionContext.Invalid

                        | _ -> defaultTraverse synBinding

                member _.VisitHashDirective(_, _directive, range) =
                    // No completions in a directive
                    if rangeContainsPos range pos then
                        Some CompletionContext.Invalid
                    else
                        None

                member _.VisitModuleOrNamespace(_, SynModuleOrNamespace(longId = idents)) =
                    match List.tryLast idents with
                    | Some lastIdent when
                        pos.Line = lastIdent.idRange.EndLine
                        && lastIdent.idRange.EndColumn >= 0
                        && pos.Column <= lineStr.Length
                        ->
                        let stringBetweenModuleNameAndPos =
                            lineStr[lastIdent.idRange.EndColumn .. pos.Column - 1]

                        if stringBetweenModuleNameAndPos |> Seq.forall (fun x -> x = ' ' || x = '.') then
                            // No completions in a top level a module or namespace identifier
                            Some CompletionContext.Invalid
                        else
                            None
                    | _ -> None

                member _.VisitComponentInfo(_, SynComponentInfo(range = range)) =
                    // No completions in component info (unless it's within an attribute)
                    // /// XmlDo|
                    // type R = class end
                    if rangeContainsPos range pos then
                        Some CompletionContext.Invalid
                    else
                        None

                member _.VisitLetOrUse(_, _, _, bindings, range) =
                    match bindings with
                    | [] when range.StartLine = pos.Line -> Some CompletionContext.Invalid
                    | _ -> None

                member _.VisitSimplePats(_, pat) =
                    // Lambdas and their patterns are processed above in VisitExpr,
                    // so VisitSimplePats is only called for primary constructors

                    let rec loop (pat: SynPat) =
                        if not (rangeContainsPos pat.Range pos) then
                            None
                        else

                            match pat with
                            // type C (x{caret} )
                            | SynPat.Named _
                            | SynPat.Const(SynConst.Unit, _) -> Some CompletionContext.Invalid

                            | SynPat.Attrib(pat, _, _)
                            | SynPat.Paren(pat, _) -> loop pat

                            | SynPat.Tuple(_, pats, _, _) -> List.tryPick loop pats

                            | SynPat.Typed(pat, synType, _) ->
                                // type C (x: int{caret}) ->
                                if rangeContainsPos synType.Range pos then
                                    Some CompletionContext.Type
                                else
                                    // type C (x{caret}: int) ->
                                    loop pat

                            | _ -> None

                    loop pat

                member _.VisitPat(_, defaultTraverse, pat) =
                    TryGetCompletionContextInPattern false pat None pos
                    |> Option.orElseWith (fun () -> defaultTraverse pat)

                member _.VisitModuleDecl(_, defaultTraverse, decl) =
                    match decl with
                    | SynModuleDecl.Open(target, m) ->
                        // in theory, this means we're "in an open"
                        // in practice, because the parse tree/visitors do not handle attributes well yet, need extra check below to ensure not e.g. $here$
                        //     open System
                        //     [<Attr$
                        //     let f() = ()
                        // inside an attribute on the next item
                        let pos = mkPos pos.Line (pos.Column - 1) // -1 because for e.g. "open System." the dot does not show up in the parse tree

                        if rangeContainsPos m pos then
                            let isOpenType =
                                match target with
                                | SynOpenDeclTarget.Type _ -> true
                                | SynOpenDeclTarget.ModuleOrNamespace _ -> false

                            Some(CompletionContext.OpenDeclaration isOpenType)
                        else
                            None

                    // module Namespace.Top
                    // module Nested
                    | SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo(longId = [ ident ])) when rangeContainsPos ident.idRange pos ->
                        Some CompletionContext.Invalid

                    | _ -> defaultTraverse decl

                member _.VisitType(_, defaultTraverse, ty) =
                    match ty with
                    | SynType.LongIdent _ when rangeContainsPos ty.Range pos -> Some CompletionContext.Type
                    | _ -> defaultTraverse ty

                member _.VisitRecordDefn(_, fields, range) =
                    fields
                    |> List.tryPick (fun (SynField(idOpt = idOpt; range = fieldRange; fieldType = fieldType)) ->
                        match idOpt, fieldType with
                        | Some id, _ when rangeContainsPos id.idRange pos ->
                            Some(CompletionContext.RecordField(RecordContext.Declaration true))
                        | _ when rangeContainsPos fieldRange pos -> Some(CompletionContext.RecordField(RecordContext.Declaration false))
                        | _, SynType.FromParseError _ -> Some(CompletionContext.RecordField(RecordContext.Declaration false))
                        | _ -> None)
                    // No completions in a record outside of all fields, except in attributes, which is established earlier in VisitAttributeApplication
                    |> Option.orElseWith (fun _ ->
                        if rangeContainsPos range pos then
                            Some CompletionContext.Invalid
                        else
                            None)

                member _.VisitUnionDefn(_, cases, _) =
                    cases
                    |> List.tryPick (fun (SynUnionCase(ident = SynIdent(id, _); caseType = caseType)) ->
                        if rangeContainsPos id.idRange pos then
                            // No completions in a union case identifier
                            Some CompletionContext.Invalid
                        else
                            match caseType with
                            | SynUnionCaseKind.Fields fieldCases ->
                                fieldCases
                                |> List.tryPick (fun (SynField(idOpt = fieldIdOpt; range = fieldRange)) ->
                                    match fieldIdOpt with
                                    // No completions in a union case field identifier
                                    | Some id when rangeContainsPos id.idRange pos -> Some CompletionContext.Invalid
                                    | _ ->
                                        if rangeContainsPos fieldRange pos then
                                            Some CompletionContext.UnionCaseFieldsDeclaration
                                        else
                                            None)
                            | _ -> None)

                member _.VisitEnumDefn(_, cases, _) =
                    cases
                    |> List.tryPick (fun (SynEnumCase(ident = SynIdent(ident = id))) ->
                        if rangeContainsPos id.idRange pos then
                            // No completions in an enum case identifier
                            Some CompletionContext.Invalid
                        else
                            // The value expression should still get completions
                            None)

                member _.VisitTypeAbbrev(_, _, range) =
                    if rangeContainsPos range pos then
                        Some CompletionContext.TypeAbbreviationOrSingleCaseUnion
                    else
                        None

                member _.VisitAttributeApplication(_, attributes) =
                    attributes.Attributes
                    |> List.tryPick (fun att ->
                        // [<Att|()>]
                        if rangeContainsPos att.TypeName.Range pos then
                            Some CompletionContext.AttributeApplication
                        // [<Att(M| = )>]
                        elif rangeContainsPos att.ArgExpr.Range pos then
                            Some(CompletionContext.ParameterList(att.TypeName.Range.End, findSetters att.ArgExpr))
                        else
                            None)

                override _.VisitInterfaceSynMemberDefnType(_, synType: SynType) =
                    match synType with
                    | SynType.FromParseError(range = m) when rangeContainsPosOrIsSpacesBetweenRangeAndPos lineStr m pos ->
                        Some(CompletionContext.Inherit(InheritanceContext.Interface, ([], None)))
                    | _ -> None
            }

        let ctxt = SyntaxTraversal.Traverse(pos, parsedInput, visitor)

        match ctxt with
        | Some _ -> ctxt
        | _ -> TryGetCompletionContextOfAttributes(pos, lineStr)

    //--------------------------------------------------------------------------------------------
    // TryGetInsertionContext

    /// Check if we are at an "open" declaration
    let GetFullNameOfSmallestModuleOrNamespaceAtPoint (pos: pos, parsedInput: ParsedInput) =
        let mutable path = []

        let visitor =
            { new SyntaxVisitorBase<bool>() with
                override this.VisitExpr(_, _traverseSynExpr, defaultTraverse, expr) =
                    // don't need to keep going, namespaces and modules never appear inside Exprs
                    None

                override this.VisitModuleOrNamespace(_, SynModuleOrNamespace(longId = longId; range = range)) =
                    if rangeContainsPos range pos then
                        path <- path @ longId

                    None // we should traverse the rest of the AST to find the smallest module
            }

        SyntaxTraversal.Traverse(pos, parsedInput, visitor) |> ignore
        path |> List.map (fun x -> x.idText) |> List.toArray

    let (|ConstructorPats|) pats =
        match pats with
        | SynArgPats.Pats ps -> ps
        | SynArgPats.NamePatPairs(pats = xs) -> List.map (fun (_, _, pat) -> pat) xs

    /// Returns all `Ident`s and `LongIdent`s found in an untyped AST.
    let getLongIdents (parsedInput: ParsedInput) : IDictionary<pos, LongIdent> =
        let identsByEndPos = Dictionary<pos, LongIdent>()

        let addLongIdent (longIdent: LongIdent) =
            for ident in longIdent do
                identsByEndPos[ident.idRange.End] <- longIdent

        let addLongIdentWithDots (SynLongIdent(longIdent, lids, _) as value) =
            match longIdent with
            | [] -> ()
            | [ _ ] as idents -> identsByEndPos[value.Range.End] <- idents
            | idents ->
                for dotRange in lids do
                    identsByEndPos[mkPos dotRange.EndLine (dotRange.EndColumn - 1)] <- idents

                identsByEndPos[value.Range.End] <- idents

        let addIdent (ident: Ident) =
            identsByEndPos[ident.idRange.End] <- [ ident ]

        let rec walkImplFileInput (file: ParsedImplFileInput) =
            List.iter walkSynModuleOrNamespace file.Contents

        and walkSynModuleOrNamespace (SynModuleOrNamespace(decls = decls; attribs = Attributes attrs)) =
            List.iter walkAttribute attrs
            List.iter walkSynModuleDecl decls

        and walkAttribute (attr: SynAttribute) =
            addLongIdentWithDots attr.TypeName
            walkExpr attr.ArgExpr

        and walkTyparDecl (SynTyparDecl.SynTyparDecl(Attributes attrs, typar, intersectionConstraints, _)) =
            List.iter walkAttribute attrs
            walkTypar typar
            List.iter walkType intersectionConstraints

        and walkTypeConstraint cx =
            match cx with
            | SynTypeConstraint.WhereTyparIsValueType(t, _)
            | SynTypeConstraint.WhereTyparIsReferenceType(t, _)
            | SynTypeConstraint.WhereTyparIsUnmanaged(t, _)
            | SynTypeConstraint.WhereTyparSupportsNull(t, _)
            | SynTypeConstraint.WhereTyparNotSupportsNull(genericName = t)
            | SynTypeConstraint.WhereTyparIsComparable(t, _)
            | SynTypeConstraint.WhereTyparIsEquatable(t, _) -> walkTypar t
            | SynTypeConstraint.WhereTyparDefaultsToType(t, ty, _)
            | SynTypeConstraint.WhereTyparSubtypeOfType(t, ty, _) ->
                walkTypar t
                walkType ty
            | SynTypeConstraint.WhereTyparIsEnum(t, ts, _)
            | SynTypeConstraint.WhereTyparIsDelegate(t, ts, _) ->
                walkTypar t
                List.iter walkType ts
            | SynTypeConstraint.WhereTyparSupportsMember(TypesForTypar ts, sign, _) ->
                List.iter walkType ts
                walkMemberSig sign
            | SynTypeConstraint.WhereSelfConstrained(ty, _) -> walkType ty

        and walkPat pat =
            match pat with
            | SynPat.Tuple(elementPats = pats)
            | SynPat.ArrayOrList(_, pats, _)
            | SynPat.Ands(pats, _) -> List.iter walkPat pats
            | SynPat.Named(SynIdent(ident, _), _, _, _) -> addIdent ident
            | SynPat.Typed(pat, t, _) ->
                walkPat pat
                walkType t
            | SynPat.Attrib(pat, Attributes attrs, _) ->
                walkPat pat
                List.iter walkAttribute attrs
            | SynPat.As(pat1, pat2, _)
            | SynPat.Or(pat1, pat2, _, _)
            | SynPat.ListCons(pat1, pat2, _, _) -> List.iter walkPat [ pat1; pat2 ]
            | SynPat.LongIdent(longDotId = ident; typarDecls = typars; argPats = ConstructorPats pats) ->
                addLongIdentWithDots ident

                typars
                |> Option.iter (fun (ValTyparDecls(typars, constraints, _)) ->
                    List.iter walkTyparDecl typars
                    List.iter walkTypeConstraint constraints)

                List.iter walkPat pats
            | SynPat.Paren(pat, _) -> walkPat pat
            | SynPat.IsInst(t, _) -> walkType t
            | SynPat.QuoteExpr(e, _) -> walkExpr e
            | _ -> ()

        and walkTypar (SynTypar _) = ()

        and walkBinding (SynBinding(attributes = Attributes attrs; headPat = pat; returnInfo = returnInfo; expr = e)) =
            List.iter walkAttribute attrs
            walkPat pat
            walkExpr e

            returnInfo
            |> Option.iter (fun (SynBindingReturnInfo(typeName = t)) -> walkType t)

        and walkInterfaceImpl (SynInterfaceImpl(bindings = bindings)) = List.iter walkBinding bindings

        and walkType synType =
            match synType with
            | SynType.Array(_, t, _)
            | SynType.HashConstraint(t, _)
            | SynType.MeasurePower(t, _, _)
            | SynType.WithNull(innerType = t)
            | SynType.Paren(t, _)
            | SynType.SignatureParameter(usedType = t) -> walkType t
            | SynType.Fun(argType = t1; returnType = t2)
            | SynType.Or(t1, t2, _, _) ->
                walkType t1
                walkType t2
            | SynType.LongIdent ident -> addLongIdentWithDots ident
            | SynType.App(ty, _, types, _, _, _, _) ->
                walkType ty
                List.iter walkType types
            | SynType.LongIdentApp(_, _, _, types, _, _, _) -> List.iter walkType types
            | SynType.Tuple(path = segment) -> getTypeFromTuplePath segment |> List.iter walkType
            | SynType.WithGlobalConstraints(t, typeConstraints, _) ->
                walkType t
                List.iter walkTypeConstraint typeConstraints
            | SynType.StaticConstantExpr(e, _) -> walkExpr e
            | SynType.StaticConstantNamed(ident, value, _) ->
                walkType ident
                walkType value
            | SynType.Intersection(types = types) -> List.iter walkType types
            | SynType.StaticConstantNull _
            | SynType.Anon _
            | SynType.AnonRecd _
            | SynType.Var _
            | SynType.StaticConstant _
            | SynType.FromParseError _ -> ()

        and walkClause (SynMatchClause(pat = pat; whenExpr = e1; resultExpr = e2)) =
            walkPat pat
            walkExpr e2
            e1 |> Option.iter walkExpr

        and walkSimplePats spats =
            match spats with
            | SynSimplePats.SimplePats(pats = pats) -> List.iter walkSimplePat pats

        and walkExpr expr =
            match expr with
            | SynExpr.Paren(expr = e)
            | SynExpr.Quote(quotedExpr = e)
            | SynExpr.Typed(expr = e)
            | SynExpr.InferredUpcast(expr = e)
            | SynExpr.InferredDowncast(expr = e)
            | SynExpr.AddressOf(expr = e)
            | SynExpr.DoBang(expr = e)
            | SynExpr.YieldOrReturn(expr = e)
            | SynExpr.ArrayOrListComputed(expr = e)
            | SynExpr.ComputationExpr(expr = e)
            | SynExpr.Do(expr = e)
            | SynExpr.Assert(expr = e)
            | SynExpr.Lazy(expr = e)
            | SynExpr.DotLambda(expr = e)
            | SynExpr.IndexFromEnd(expr = e)
            | SynExpr.YieldOrReturnFrom(expr = e) -> walkExpr e

            | SynExpr.Lambda(args = pats; body = e) ->
                walkSimplePats pats
                walkExpr e

            | SynExpr.New(expr = e; targetType = t)
            | SynExpr.TypeTest(expr = e; targetType = t)
            | SynExpr.Upcast(expr = e; targetType = t)
            | SynExpr.Downcast(expr = e; targetType = t) ->
                walkExpr e
                walkType t

            | SynExpr.Tuple(exprs = es)
            | Sequentials es
            | SynExpr.ArrayOrList(exprs = es) -> List.iter walkExpr es

            | SynExpr.JoinIn(lhsExpr = e1; rhsExpr = e2)
            | SynExpr.DotIndexedGet(objectExpr = e1; indexArgs = e2)
            | SynExpr.Set(targetExpr = e1; rhsExpr = e2)
            | SynExpr.App(funcExpr = e1; argExpr = e2)
            | SynExpr.TryFinally(tryExpr = e1; finallyExpr = e2)
            | SynExpr.WhileBang(whileExpr = e1; doExpr = e2)
            | SynExpr.While(whileExpr = e1; doExpr = e2) ->
                walkExpr e1
                walkExpr e2

            | SynExpr.Record(recordFields = fields) ->
                fields
                |> List.iter (fun (SynExprRecordField(fieldName = (ident, _); expr = e)) ->
                    addLongIdentWithDots ident
                    e |> Option.iter walkExpr)

            | SynExpr.Ident ident -> addIdent ident

            | SynExpr.ObjExpr(objType = ty; argOptions = argOpt; bindings = bindings; members = ms; extraImpls = ifaces) ->
                let bindings = unionBindingAndMembers bindings ms

                argOpt
                |> Option.iter (fun (e, ident) ->
                    walkExpr e
                    ident |> Option.iter addIdent)

                walkType ty
                List.iter walkBinding bindings
                List.iter walkInterfaceImpl ifaces

            | SynExpr.LongIdent(longDotId = ident) -> addLongIdentWithDots ident

            | SynExpr.For(ident = ident; identBody = e1; toBody = e2; doBody = e3) ->
                addIdent ident
                walkExpr e1
                walkExpr e2
                walkExpr e3

            | SynExpr.ForEach(pat = pat; enumExpr = e1; bodyExpr = e2) ->
                walkPat pat
                walkExpr e1
                walkExpr e2

            | SynExpr.MatchLambda(matchClauses = clauses) -> List.iter walkClause clauses

            | SynExpr.MatchBang(expr = e; clauses = clauses)
            | SynExpr.Match(expr = e; clauses = clauses) ->
                walkExpr e
                List.iter walkClause clauses

            | SynExpr.TypeApp(expr = e; typeArgs = tys) ->
                List.iter walkType tys
                walkExpr e

            | SynExpr.LetOrUse(bindings = bindings; body = e) ->
                List.iter walkBinding bindings
                walkExpr e

            | SynExpr.TryWith(tryExpr = e; withCases = clauses) ->
                List.iter walkClause clauses
                walkExpr e

            | SynExpr.IfThenElse(ifExpr = e1; thenExpr = e2; elseExpr = e3) ->
                walkExpr e1
                walkExpr e2
                e3 |> Option.iter walkExpr

            | SynExpr.LongIdentSet(longDotId = ident; expr = e)
            | SynExpr.DotGet(longDotId = ident; expr = e) ->
                addLongIdentWithDots ident
                walkExpr e

            | SynExpr.NamedIndexedPropertySet(longDotId = ident; expr1 = e1; expr2 = e2)
            | SynExpr.DotSet(targetExpr = e1; longDotId = ident; rhsExpr = e2) ->
                addLongIdentWithDots ident
                walkExpr e1
                walkExpr e2

            | SynExpr.IndexRange(expr1 = expr1; expr2 = expr2) ->
                match expr1 with
                | Some e -> walkExpr e
                | None -> ()

                match expr2 with
                | Some e -> walkExpr e
                | None -> ()

            | SynExpr.DotIndexedSet(objectExpr = e1; indexArgs = args; valueExpr = e2) ->
                walkExpr e1
                walkExpr args
                walkExpr e2

            | SynExpr.DotNamedIndexedPropertySet(targetExpr = e1; longDotId = ident; argExpr = e2; rhsExpr = e3) ->
                addLongIdentWithDots ident
                walkExpr e1
                walkExpr e2
                walkExpr e3

            | SynExpr.LetOrUseBang(pat = pat; rhs = e1; andBangs = es; body = e2) ->
                walkPat pat
                walkExpr e1

                for SynExprAndBang(pat = patAndBang; body = eAndBang) in es do
                    walkPat patAndBang
                    walkExpr eAndBang

                walkExpr e2

            | SynExpr.TraitCall(TypesForTypar ts, sign, e, _) ->
                List.iter walkType ts
                walkMemberSig sign
                walkExpr e

            | SynExpr.Const(constant = SynConst.Measure(synMeasure = m)) -> walkMeasure m

            | _ -> ()

        and walkMeasure measure =
            match measure with
            | SynMeasure.Product(measure1 = m1; measure2 = m2) ->
                walkMeasure m1
                walkMeasure m2
            | SynMeasure.Divide(measure1 = m1; measure2 = m2) ->
                m1 |> Option.iter walkMeasure
                walkMeasure m2
            | SynMeasure.Named(longIdent, _) -> addLongIdent longIdent
            | SynMeasure.Seq(ms, _) -> List.iter walkMeasure ms
            | SynMeasure.Paren(m, _)
            | SynMeasure.Power(measure = m) -> walkMeasure m
            | SynMeasure.Var(ty, _) -> walkTypar ty
            | SynMeasure.One _
            | SynMeasure.Anon _ -> ()

        and walkSimplePat spat =
            match spat with
            | SynSimplePat.Attrib(pat, Attributes attrs, _) ->
                walkSimplePat pat
                List.iter walkAttribute attrs
            | SynSimplePat.Typed(pat, t, _) ->
                walkSimplePat pat
                walkType t
            | _ -> ()

        and walkField (SynField(attributes = Attributes attrs; fieldType = t)) =
            List.iter walkAttribute attrs
            walkType t

        and walkValSig (SynValSig(attributes = Attributes attrs; synType = t; arity = SynValInfo(argInfos, argInfo))) =
            List.iter walkAttribute attrs
            walkType t

            argInfo :: (argInfos |> List.concat)
            |> List.collect (fun (SynArgInfo(Attributes attrs, _, _)) -> attrs)
            |> List.iter walkAttribute

        and walkMemberSig membSig =
            match membSig with
            | SynMemberSig.Inherit(t, _)
            | SynMemberSig.Interface(t, _) -> walkType t
            | SynMemberSig.Member(memberSig = vs) -> walkValSig vs
            | SynMemberSig.ValField(f, _) -> walkField f
            | SynMemberSig.NestedType(nestedType = typeDefn) ->
                let (SynTypeDefnSig(typeInfo = info; typeRepr = repr; members = memberSigs)) =
                    typeDefn

                let isTypeExtensionOrAlias =
                    match repr with
                    | SynTypeDefnSigRepr.Simple(SynTypeDefnSimpleRepr.TypeAbbrev _, _)
                    | SynTypeDefnSigRepr.ObjectModel(SynTypeDefnKind.Abbrev, _, _)
                    | SynTypeDefnSigRepr.ObjectModel(kind = SynTypeDefnKind.Augmentation _) -> true
                    | _ -> false

                walkComponentInfo isTypeExtensionOrAlias info
                walkTypeDefnSigRepr repr
                List.iter walkMemberSig memberSigs

        and walkMember memb =
            match memb with
            | SynMemberDefn.AbstractSlot(slotSig = valSig) -> walkValSig valSig
            | SynMemberDefn.Member(binding, _) -> walkBinding binding
            | SynMemberDefn.GetSetMember(getBinding, setBinding, _, _) ->
                Option.iter walkBinding getBinding
                Option.iter walkBinding setBinding
            | SynMemberDefn.ImplicitCtor(attributes = Attributes attrs; ctorArgs = pat) ->
                List.iter walkAttribute attrs
                walkPat pat
            | SynMemberDefn.ImplicitInherit(t, e, _, _, _) ->
                walkType t
                walkExpr e
            | SynMemberDefn.LetBindings(bindings, _, _, _) -> List.iter walkBinding bindings
            | SynMemberDefn.Interface(interfaceType = t; members = members) ->
                walkType t
                members |> Option.iter (List.iter walkMember)
            | SynMemberDefn.Inherit(baseType = Some baseType) -> walkType baseType
            | SynMemberDefn.Inherit(baseType = None) -> ()
            | SynMemberDefn.ValField(fieldInfo = field) -> walkField field
            | SynMemberDefn.NestedType(tdef, _, _) -> walkTypeDefn tdef
            | SynMemberDefn.AutoProperty(attributes = Attributes attrs; typeOpt = t; synExpr = e) ->
                List.iter walkAttribute attrs
                Option.iter walkType t
                walkExpr e
            | _ -> ()

        and walkEnumCase (SynEnumCase(attributes = Attributes attrs)) = List.iter walkAttribute attrs

        and walkUnionCaseType kind =
            match kind with
            | SynUnionCaseKind.Fields fields -> List.iter walkField fields
            | SynUnionCaseKind.FullType(t, _) -> walkType t

        and walkUnionCase (SynUnionCase(attributes = Attributes attrs; caseType = t)) =
            List.iter walkAttribute attrs
            walkUnionCaseType t

        and walkTypeDefnSimple typeDefn =
            match typeDefn with
            | SynTypeDefnSimpleRepr.Enum(cases, _) -> List.iter walkEnumCase cases
            | SynTypeDefnSimpleRepr.Union(_, cases, _) -> List.iter walkUnionCase cases
            | SynTypeDefnSimpleRepr.Record(_, fields, _) -> List.iter walkField fields
            | SynTypeDefnSimpleRepr.TypeAbbrev(_, t, _) -> walkType t
            | _ -> ()

        and walkComponentInfo isTypeExtensionOrAlias compInfo =
            let (SynComponentInfo(Attributes attrs, TyparsAndConstraints(typars, cs1), cs2, longIdent, _, _, _, _)) =
                compInfo

            let constraints = cs1 @ cs2
            List.iter walkAttribute attrs
            List.iter walkTyparDecl typars
            List.iter walkTypeConstraint constraints

            if isTypeExtensionOrAlias then
                addLongIdent longIdent

        and walkTypeDefnRepr inp =
            match inp with
            | SynTypeDefnRepr.ObjectModel(_, defns, _) -> List.iter walkMember defns
            | SynTypeDefnRepr.Simple(defn, _) -> walkTypeDefnSimple defn
            | SynTypeDefnRepr.Exception _ -> ()

        and walkTypeDefnSigRepr inp =
            match inp with
            | SynTypeDefnSigRepr.ObjectModel(_, defns, _) -> List.iter walkMemberSig defns
            | SynTypeDefnSigRepr.Simple(defn, _) -> walkTypeDefnSimple defn
            | SynTypeDefnSigRepr.Exception _ -> ()

        and walkTypeDefn typeDefn =
            let (SynTypeDefn(typeInfo = info; typeRepr = repr; members = members; implicitConstructor = implicitCtor)) =
                typeDefn

            let isTypeExtensionOrAlias =
                match repr with
                | SynTypeDefnRepr.ObjectModel(kind = SynTypeDefnKind.Augmentation _)
                | SynTypeDefnRepr.ObjectModel(SynTypeDefnKind.Abbrev, _, _)
                | SynTypeDefnRepr.Simple(SynTypeDefnSimpleRepr.TypeAbbrev _, _) -> true
                | _ -> false

            walkComponentInfo isTypeExtensionOrAlias info
            walkTypeDefnRepr repr
            List.iter walkMember members
            Option.iter walkMember implicitCtor

        and walkSynModuleDecl (decl: SynModuleDecl) =
            match decl with
            | SynModuleDecl.NamespaceFragment fragment -> walkSynModuleOrNamespace fragment
            | SynModuleDecl.NestedModule(moduleInfo = info; decls = modules) ->
                walkComponentInfo false info
                List.iter walkSynModuleDecl modules
            | SynModuleDecl.Let(_, bindings, _) -> List.iter walkBinding bindings
            | SynModuleDecl.Expr(expr, _) -> walkExpr expr
            | SynModuleDecl.Types(types, _) -> List.iter walkTypeDefn types
            | SynModuleDecl.Attributes(Attributes attrs, _) -> List.iter walkAttribute attrs
            | _ -> ()

        match parsedInput with
        | ParsedInput.ImplFile input -> walkImplFileInput input
        | _ -> ()
        //debug "%A" idents
        upcast identsByEndPos

    let GetLongIdentAt parsedInput pos =
        let idents = getLongIdents parsedInput

        match idents.TryGetValue pos with
        | true, idents -> Some idents
        | _ -> None

    type Scope =
        {
            ShortIdents: ShortIdents
            Kind: ScopeKind
        }

    let tryFindNearestPointAndModules currentLine (ast: ParsedInput) (insertionPoint: OpenStatementInsertionPoint) =
        // We ignore all diagnostics during this operation
        //
        // Based on an initial review, no diagnostics should be generated.  However the code should be checked more closely.
        use _ignoreAllDiagnostics = new DiagnosticsScope(false)

        let mutable result = None
        let mutable ns = None
        let modules = ResizeArray<FSharpModule>()

        let inline longIdentToIdents ident = ident |> Seq.map string |> Seq.toArray

        let addModule (longIdent: LongIdent, range: range) =
            modules.Add
                {
                    Idents = longIdentToIdents longIdent
                    Range = range
                }

        let doRange kind (scope: LongIdent) line col =
            if line <= currentLine then
                match result, insertionPoint with
                | None, _ ->
                    result <-
                        Some(
                            {
                                ShortIdents = longIdentToIdents scope
                                Kind = kind
                            },
                            mkPos line col,
                            false
                        )
                | Some(_, _, true), _ -> ()
                | Some(oldScope, oldPos, false), OpenStatementInsertionPoint.TopLevel when kind <> OpenDeclaration ->
                    result <- Some(oldScope, oldPos, true)
                | Some(oldScope, oldPos, _), _ ->
                    match kind, oldScope.Kind with
                    | (Namespace | NestedModule | TopModule), OpenDeclaration
                    | _ when oldPos.Line <= line ->
                        result <-
                            Some(
                                {
                                    ShortIdents =
                                        match scope with
                                        | [] -> oldScope.ShortIdents
                                        | _ -> longIdentToIdents scope
                                    Kind = kind
                                },
                                mkPos line col,
                                false
                            )
                    | _ -> ()

        let getMinColumn decls =
            match decls with
            | [] -> None
            | firstDecl :: _ ->
                match firstDecl with
                | SynModuleDecl.NestedModule(range = r)
                | SynModuleDecl.Let(range = r)
                | SynModuleDecl.Expr(range = r)
                | SynModuleDecl.Types(range = r)
                | SynModuleDecl.Exception(range = r)
                | SynModuleDecl.Open(range = r)
                | SynModuleDecl.HashDirective(range = r) -> Some r
                | _ -> None
                |> Option.map (fun r -> r.StartColumn)

        let rec walkImplFileInput (file: ParsedImplFileInput) =
            List.iter (walkSynModuleOrNamespace []) file.Contents

        and walkSynModuleOrNamespace (parent: LongIdent) modul =
            let (SynModuleOrNamespace(longId = ident; kind = kind; decls = decls; range = range)) =
                modul

            if range.EndLine >= currentLine then
                let isModule = kind.IsModule

                match isModule, parent, ident with
                | false, _, _ -> ns <- Some(longIdentToIdents ident)
                // top level module with "inlined" namespace like Ns1.Ns2.TopModule
                | true, [], _f :: _s :: _ ->
                    let ident = longIdentToIdents ident
                    ns <- Some ident[0 .. ident.Length - 2]
                | _ -> ()

                let fullIdent = parent @ ident

                let startLine = if isModule then range.StartLine else range.StartLine - 1

                let scopeKind =
                    match isModule, parent with
                    | true, [] -> TopModule
                    | true, _ -> NestedModule
                    | _ -> Namespace

                doRange scopeKind fullIdent startLine range.StartColumn
                addModule (fullIdent, range)
                List.iter (walkSynModuleDecl fullIdent) decls

        and walkSynModuleDecl (parent: LongIdent) (decl: SynModuleDecl) =
            match decl with
            | SynModuleDecl.NamespaceFragment fragment -> walkSynModuleOrNamespace parent fragment
            | SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo(longId = ident); decls = decls; range = range) ->
                let fullIdent = parent @ ident
                addModule (fullIdent, range)

                if range.EndLine >= currentLine then
                    let moduleBodyIndentation =
                        getMinColumn decls |> Option.defaultValue (range.StartColumn + 4)

                    doRange NestedModule fullIdent range.StartLine moduleBodyIndentation
                    List.iter (walkSynModuleDecl fullIdent) decls
            | SynModuleDecl.Open(_, range) -> doRange OpenDeclaration [] range.EndLine (range.StartColumn - 5)
            | SynModuleDecl.HashDirective(_, range) -> doRange HashDirective [] range.EndLine range.StartColumn
            | _ -> ()

        match ast with
        | ParsedInput.SigFile _ -> ()
        | ParsedInput.ImplFile input -> walkImplFileInput input

        let res =
            result
            |> Option.map (fun (scope, pos, _) ->
                let ns = ns |> Option.map longIdentToIdents
                scope, ns, mkPos (pos.Line + 1) pos.Column)

        let modules =
            modules
            |> Seq.filter (fun x -> x.Range.EndLine < currentLine)
            |> Seq.sortBy (fun x -> -x.Idents.Length)
            |> Seq.toList

        res, modules

    let findBestPositionToInsertOpenDeclaration (modules: FSharpModule list) scope pos (entity: ShortIdents) =
        match modules |> List.filter (fun x -> entity |> Array.startsWith x.Idents) with
        | [] -> { ScopeKind = scope.Kind; Pos = pos }
        | m :: _ ->
            //printfn "All modules: %A, Win module: %A" modules m
            let scopeKind =
                match scope.Kind with
                | TopModule -> NestedModule
                | x -> x

            {
                ScopeKind = scopeKind
                Pos = mkPos (Line.fromZ m.Range.EndLine) m.Range.StartColumn
            }

    let TryFindInsertionContext
        (currentLine: int)
        (parsedInput: ParsedInput)
        (partiallyQualifiedName: MaybeUnresolvedIdent[])
        (insertionPoint: OpenStatementInsertionPoint)
        =
        let res, modules =
            tryFindNearestPointAndModules currentLine parsedInput insertionPoint

        fun
            (requiresQualifiedAccessParent: ShortIdents option,
             autoOpenParent: ShortIdents option,
             entityNamespace: ShortIdents option,
             entity: ShortIdents) ->

            // We ignore all diagnostics during this operation
            //
            // Based on an initial review, no diagnostics should be generated.  However the code should be checked more closely.
            use _ignoreAllDiagnostics = new DiagnosticsScope(false)

            match res with
            | None -> [||]
            | Some(scope, ns, pos) ->
                let entities =
                    Entity.tryCreate (
                        ns,
                        scope.ShortIdents,
                        partiallyQualifiedName,
                        requiresQualifiedAccessParent,
                        autoOpenParent,
                        entityNamespace,
                        entity
                    )

                entities
                |> Array.map (fun e -> e, findBestPositionToInsertOpenDeclaration modules scope pos entity)

    /// Corrects insertion line number based on kind of scope and text surrounding the insertion point.
    let AdjustInsertionPoint (getLineStr: int -> string) ctx =
        let line =
            match ctx.ScopeKind with
            | ScopeKind.TopModule ->
                if ctx.Pos.Line > 1 then
                    // it's an implicit module without any open declarations
                    let line = getLineStr (ctx.Pos.Line - 2)

                    let isImplicitTopLevelModule =
                        not (line.StartsWithOrdinal("module") && not (line.EndsWithOrdinal("=")))

                    if isImplicitTopLevelModule then 1 else ctx.Pos.Line
                else
                    1

            | ScopeKind.Namespace ->
                // For namespaces the start line is start line of the first nested entity
                // If we are not on the first line, try to find opening namespace, and return line after it (in F# format)
                if ctx.Pos.Line > 1 then
                    [ 0 .. ctx.Pos.Line - 1 ]
                    |> List.mapi (fun i line -> i, getLineStr line)
                    |> List.tryPick (fun (i, lineStr) ->
                        if lineStr.StartsWithOrdinal("namespace") then
                            Some i
                        else
                            None)
                    |> function
                        // move to the next line below "namespace" and convert it to F# 1-based line number
                        | Some line -> line + 2
                        | None -> ctx.Pos.Line
                // If we are on 1st line in the namespace ctx, this line _should_ be the namespace declaration, check it and return next line.
                // Otherwise, return first line (which theoretically should not happen).
                else
                    let lineStr = getLineStr (ctx.Pos.Line - 1)

                    if lineStr.StartsWithOrdinal("namespace") then
                        ctx.Pos.Line + 1
                    else
                        ctx.Pos.Line

            | _ -> ctx.Pos.Line

        mkPos line ctx.Pos.Column

    let FindNearestPointToInsertOpenDeclaration
        (currentLine: int)
        (parsedInput: ParsedInput)
        (entity: ShortIdents)
        (insertionPoint: OpenStatementInsertionPoint)
        =
        match tryFindNearestPointAndModules currentLine parsedInput insertionPoint with
        | Some(scope, _, point), modules -> findBestPositionToInsertOpenDeclaration modules scope point entity
        | _ ->
            // we failed to find insertion point because ast is empty for some reason, return top left point in this case
            {
                ScopeKind = ScopeKind.TopModule
                Pos = mkPos 1 0
            }
