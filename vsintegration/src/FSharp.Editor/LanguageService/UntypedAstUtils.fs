// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information
namespace Microsoft.VisualStudio.FSharp.Editor
open System
open System.Collections.Generic

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Ast

[<AutoOpen>]
module internal UntypedAstUtils =
    open Microsoft.FSharp.Compiler.Range

    type Range.range with
        member inline x.IsEmpty = x.StartColumn = x.EndColumn && x.StartLine = x.EndLine 

    type internal ShortIdent = string
    type internal Idents = ShortIdent[]

    let internal longIdentToArray (longIdent: Ident list): Idents =
        longIdent |> Seq.map string |> Seq.toArray   


    /// An recursive pattern that collect all sequential expressions to avoid StackOverflowException
    let rec (|Sequentials|_|) = function
        | SynExpr.Sequential(_, _, e, Sequentials es, _) ->
            Some(e::es)
        | SynExpr.Sequential(_, _, e1, e2, _) ->
            Some [e1; e2]
        | _ -> None

    let (|ConstructorPats|) = function
        | SynConstructorArgs.Pats ps -> ps
        | SynConstructorArgs.NamePatPairs(xs, _) -> List.map snd xs


    /// Returns all Idents and LongIdents found in an untyped AST.
    let internal getLongIdents (input: ParsedInput option) : IDictionary<Range.pos, Idents> =
        let identsByEndPos = Dictionary<Range.pos, Idents>()

        let addLongIdent (longIdent: Ident list) =
            let idents = longIdentToArray longIdent
            for ident in longIdent do
                identsByEndPos.[ident.idRange.End] <- idents

        let addLongIdentWithDots (LongIdentWithDots (longIdent, lids) as value) =
            match longIdentToArray longIdent with
            | [||] -> ()
            | [|_|] as idents -> identsByEndPos.[value.Range.End] <- idents
            | idents ->
                for dotRange in lids do
                    identsByEndPos.[Range.mkPos dotRange.EndLine (dotRange.EndColumn - 1)] <- idents
                identsByEndPos.[value.Range.End] <- idents

        let addIdent (ident: Ident) =
            identsByEndPos.[ident.idRange.End] <- [|ident.idText|]

        let rec walkImplFileInput (ParsedImplFileInput(_, _, _, _, _, moduleOrNamespaceList, _)) =
            List.iter walkSynModuleOrNamespace moduleOrNamespaceList

        and walkSynModuleOrNamespace (SynModuleOrNamespace(_, _, _, decls, _, attrs, _, _)) =
            List.iter walkAttribute attrs
            List.iter walkSynModuleDecl decls

        and walkAttribute (attr: SynAttribute) =
            addLongIdentWithDots attr.TypeName
            walkExpr attr.ArgExpr

        and walkTyparDecl (SynTyparDecl.TyparDecl (attrs, typar)) =
            List.iter walkAttribute attrs
            walkTypar typar

        and walkTypeConstraint = function
            | SynTypeConstraint.WhereTyparIsValueType (t, _)
            | SynTypeConstraint.WhereTyparIsReferenceType (t, _)
            | SynTypeConstraint.WhereTyparIsUnmanaged (t, _)
            | SynTypeConstraint.WhereTyparSupportsNull (t, _)
            | SynTypeConstraint.WhereTyparIsComparable (t, _)
            | SynTypeConstraint.WhereTyparIsEquatable (t, _) -> walkTypar t
            | SynTypeConstraint.WhereTyparDefaultsToType (t, ty, _)
            | SynTypeConstraint.WhereTyparSubtypeOfType (t, ty, _) -> walkTypar t; walkType ty
            | SynTypeConstraint.WhereTyparIsEnum (t, ts, _)
            | SynTypeConstraint.WhereTyparIsDelegate (t, ts, _) -> walkTypar t; List.iter walkType ts
            | SynTypeConstraint.WhereTyparSupportsMember (ts, sign, _) -> List.iter walkType ts; walkMemberSig sign

        and walkPat = function
            | SynPat.Tuple (pats, _)
            | SynPat.ArrayOrList (_, pats, _)
            | SynPat.Ands (pats, _) -> List.iter walkPat pats
            | SynPat.Named (pat, ident, _, _, _) ->
                walkPat pat
                addIdent ident
            | SynPat.Typed (pat, t, _) ->
                walkPat pat
                walkType t
            | SynPat.Attrib (pat, attrs, _) ->
                walkPat pat
                List.iter walkAttribute attrs
            | SynPat.Or (pat1, pat2, _) -> List.iter walkPat [pat1; pat2]
            | SynPat.LongIdent (ident, _, typars, ConstructorPats pats, _, _) ->
                addLongIdentWithDots ident
                typars
                |> Option.iter (fun (SynValTyparDecls (typars, _, constraints)) ->
                     List.iter walkTyparDecl typars
                     List.iter walkTypeConstraint constraints)
                List.iter walkPat pats
            | SynPat.Paren (pat, _) -> walkPat pat
            | SynPat.IsInst (t, _) -> walkType t
            | SynPat.QuoteExpr(e, _) -> walkExpr e
            | _ -> ()

        and walkTypar (Typar (_, _, _)) = ()

        and walkBinding (SynBinding.Binding (_, _, _, _, attrs, _, _, pat, returnInfo, e, _, _)) =
            List.iter walkAttribute attrs
            walkPat pat
            walkExpr e
            returnInfo |> Option.iter (fun (SynBindingReturnInfo (t, _, _)) -> walkType t)

        and walkInterfaceImpl (InterfaceImpl(_, bindings, _)) = List.iter walkBinding bindings

        and walkIndexerArg = function
            | SynIndexerArg.One e -> walkExpr e
            | SynIndexerArg.Two (e1, e2) -> List.iter walkExpr [e1; e2]

        and walkType = function
            | SynType.Array (_, t, _)
            | SynType.HashConstraint (t, _)
            | SynType.MeasurePower (t, _, _) -> walkType t
            | SynType.Fun (t1, t2, _)
            | SynType.MeasureDivide (t1, t2, _) -> walkType t1; walkType t2
            | SynType.LongIdent ident -> addLongIdentWithDots ident
            | SynType.App (ty, _, types, _, _, _, _) -> walkType ty; List.iter walkType types
            | SynType.LongIdentApp (_, _, _, types, _, _, _) -> List.iter walkType types
            | SynType.Tuple (ts, _) -> ts |> List.iter (fun (_, t) -> walkType t)
            | SynType.WithGlobalConstraints (t, typeConstraints, _) ->
                walkType t; List.iter walkTypeConstraint typeConstraints
            | _ -> ()

        and walkClause (Clause (pat, e1, e2, _, _)) =
            walkPat pat
            walkExpr e2
            e1 |> Option.iter walkExpr

        and walkSimplePats = function
            | SynSimplePats.SimplePats (pats, _) -> List.iter walkSimplePat pats
            | SynSimplePats.Typed (pats, ty, _) -> 
                walkSimplePats pats
                walkType ty

        and walkExpr = function
            | SynExpr.Paren (e, _, _, _)
            | SynExpr.Quote (_, _, e, _, _)
            | SynExpr.Typed (e, _, _)
            | SynExpr.InferredUpcast (e, _)
            | SynExpr.InferredDowncast (e, _)
            | SynExpr.AddressOf (_, e, _, _)
            | SynExpr.DoBang (e, _)
            | SynExpr.YieldOrReturn (_, e, _)
            | SynExpr.ArrayOrListOfSeqExpr (_, e, _)
            | SynExpr.CompExpr (_, _, e, _)
            | SynExpr.Do (e, _)
            | SynExpr.Assert (e, _)
            | SynExpr.Lazy (e, _)
            | SynExpr.YieldOrReturnFrom (_, e, _) -> walkExpr e
            | SynExpr.Lambda (_, _, pats, e, _) ->
                walkSimplePats pats
                walkExpr e
            | SynExpr.New (_, t, e, _)
            | SynExpr.TypeTest (e, t, _)
            | SynExpr.Upcast (e, t, _)
            | SynExpr.Downcast (e, t, _) -> walkExpr e; walkType t
            | SynExpr.Tuple (es, _, _)
            | Sequentials es
            | SynExpr.ArrayOrList (_, es, _) -> List.iter walkExpr es
            | SynExpr.App (_, _, e1, e2, _)
            | SynExpr.TryFinally (e1, e2, _, _, _)
            | SynExpr.While (_, e1, e2, _) -> List.iter walkExpr [e1; e2]
            | SynExpr.Record (_, _, fields, _) ->
                fields |> List.iter (fun ((ident, _), e, _) ->
                            addLongIdentWithDots ident
                            e |> Option.iter walkExpr)
            | SynExpr.Ident ident -> addIdent ident
            | SynExpr.ObjExpr(ty, argOpt, bindings, ifaces, _, _) ->
                argOpt |> Option.iter (fun (e, ident) ->
                    walkExpr e
                    ident |> Option.iter addIdent)
                walkType ty
                List.iter walkBinding bindings
                List.iter walkInterfaceImpl ifaces
            | SynExpr.LongIdent (_, ident, _, _) -> addLongIdentWithDots ident
            | SynExpr.For (_, ident, e1, _, e2, e3, _) ->
                addIdent ident
                List.iter walkExpr [e1; e2; e3]
            | SynExpr.ForEach (_, _, _, pat, e1, e2, _) ->
                walkPat pat
                List.iter walkExpr [e1; e2]
            | SynExpr.MatchLambda (_, _, synMatchClauseList, _, _) ->
                List.iter walkClause synMatchClauseList
            | SynExpr.Match (_, e, synMatchClauseList, _, _) ->
                walkExpr e
                List.iter walkClause synMatchClauseList
            | SynExpr.TypeApp (e, _, tys, _, _, _, _) ->
                List.iter walkType tys; walkExpr e
            | SynExpr.LetOrUse (_, _, bindings, e, _) ->
                List.iter walkBinding bindings; walkExpr e
            | SynExpr.TryWith (e, _, clauses, _, _, _, _) ->
                List.iter walkClause clauses;  walkExpr e
            | SynExpr.IfThenElse (e1, e2, e3, _, _, _, _) ->
                List.iter walkExpr [e1; e2]
                e3 |> Option.iter walkExpr
            | SynExpr.LongIdentSet (ident, e, _)
            | SynExpr.DotGet (e, _, ident, _) ->
                addLongIdentWithDots ident
                walkExpr e
            | SynExpr.DotSet (e1, idents, e2, _) ->
                walkExpr e1
                addLongIdentWithDots idents
                walkExpr e2
            | SynExpr.DotIndexedGet (e, args, _, _) ->
                walkExpr e
                List.iter walkIndexerArg args
            | SynExpr.DotIndexedSet (e1, args, e2, _, _, _) ->
                walkExpr e1
                List.iter walkIndexerArg args
                walkExpr e2
            | SynExpr.NamedIndexedPropertySet (ident, e1, e2, _) ->
                addLongIdentWithDots ident
                List.iter walkExpr [e1; e2]
            | SynExpr.DotNamedIndexedPropertySet (e1, ident, e2, e3, _) ->
                addLongIdentWithDots ident
                List.iter walkExpr [e1; e2; e3]
            | SynExpr.JoinIn (e1, _, e2, _) -> List.iter walkExpr [e1; e2]
            | SynExpr.LetOrUseBang (_, _, _, pat, e1, e2, _) ->
                walkPat pat
                List.iter walkExpr [e1; e2]
            | SynExpr.TraitCall (ts, sign, e, _) ->
                List.iter walkTypar ts
                walkMemberSig sign
                walkExpr e
            | SynExpr.Const (SynConst.Measure(_, m), _) -> walkMeasure m
            | _ -> ()

        and walkMeasure = function
            | SynMeasure.Product (m1, m2, _)
            | SynMeasure.Divide (m1, m2, _) -> walkMeasure m1; walkMeasure m2
            | SynMeasure.Named (longIdent, _) -> addLongIdent longIdent
            | SynMeasure.Seq (ms, _) -> List.iter walkMeasure ms
            | SynMeasure.Power (m, _, _) -> walkMeasure m
            | SynMeasure.Var (ty, _) -> walkTypar ty
            | SynMeasure.One
            | SynMeasure.Anon _ -> ()

        and walkSimplePat = function
            | SynSimplePat.Attrib (pat, attrs, _) ->
                walkSimplePat pat
                List.iter walkAttribute attrs
            | SynSimplePat.Typed(pat, t, _) ->
                walkSimplePat pat
                walkType t
            | _ -> ()

        and walkField (SynField.Field(attrs, _, _, t, _, _, _, _)) =
            List.iter walkAttribute attrs
            walkType t

        and walkValSig (SynValSig.ValSpfn(attrs, _, _, t, SynValInfo(argInfos, argInfo), _, _, _, _, _, _)) =
            List.iter walkAttribute attrs
            walkType t
            argInfo :: (argInfos |> List.concat)
            |> List.map (fun (SynArgInfo(attrs, _, _)) -> attrs)
            |> List.concat
            |> List.iter walkAttribute

        and walkMemberSig = function
            | SynMemberSig.Inherit (t, _)
            | SynMemberSig.Interface(t, _) -> walkType t
            | SynMemberSig.Member(vs, _, _) -> walkValSig vs
            | SynMemberSig.ValField(f, _) -> walkField f
            | SynMemberSig.NestedType(SynTypeDefnSig.TypeDefnSig (info, repr, memberSigs, _), _) ->
                let isTypeExtensionOrAlias =
                    match repr with
                    | SynTypeDefnSigRepr.Simple(SynTypeDefnSimpleRepr.TypeAbbrev _, _)
                    | SynTypeDefnSigRepr.ObjectModel(SynTypeDefnKind.TyconAbbrev, _, _)
                    | SynTypeDefnSigRepr.ObjectModel(SynTypeDefnKind.TyconAugmentation, _, _) -> true
                    | _ -> false
                walkComponentInfo isTypeExtensionOrAlias info
                walkTypeDefnSigRepr repr
                List.iter walkMemberSig memberSigs

        and walkMember = function
            | SynMemberDefn.AbstractSlot (valSig, _, _) -> walkValSig valSig
            | SynMemberDefn.Member (binding, _) -> walkBinding binding
            | SynMemberDefn.ImplicitCtor (_, attrs, pats, _, _) ->
                List.iter walkAttribute attrs
                List.iter walkSimplePat pats
            | SynMemberDefn.ImplicitInherit (t, e, _, _) -> walkType t; walkExpr e
            | SynMemberDefn.LetBindings (bindings, _, _, _) -> List.iter walkBinding bindings
            | SynMemberDefn.Interface (t, members, _) ->
                walkType t
                members |> Option.iter (List.iter walkMember)
            | SynMemberDefn.Inherit (t, _, _) -> walkType t
            | SynMemberDefn.ValField (field, _) -> walkField field
            | SynMemberDefn.NestedType (tdef, _, _) -> walkTypeDefn tdef
            | SynMemberDefn.AutoProperty (attrs, _, _, t, _, _, _, _, e, _, _) ->
                List.iter walkAttribute attrs
                Option.iter walkType t
                walkExpr e
            | _ -> ()

        and walkEnumCase (EnumCase(attrs, _, _, _, _)) = List.iter walkAttribute attrs

        and walkUnionCaseType = function
            | SynUnionCaseType.UnionCaseFields fields -> List.iter walkField fields
            | SynUnionCaseType.UnionCaseFullType (t, _) -> walkType t

        and walkUnionCase (SynUnionCase.UnionCase (attrs, _, t, _, _, _)) =
            List.iter walkAttribute attrs
            walkUnionCaseType t

        and walkTypeDefnSimple = function
            | SynTypeDefnSimpleRepr.Enum (cases, _) -> List.iter walkEnumCase cases
            | SynTypeDefnSimpleRepr.Union (_, cases, _) -> List.iter walkUnionCase cases
            | SynTypeDefnSimpleRepr.Record (_, fields, _) -> List.iter walkField fields
            | SynTypeDefnSimpleRepr.TypeAbbrev (_, t, _) -> walkType t
            | _ -> ()

        and walkComponentInfo isTypeExtensionOrAlias (ComponentInfo(attrs, typars, constraints, longIdent, _, _, _, _)) =
            List.iter walkAttribute attrs
            List.iter walkTyparDecl typars
            List.iter walkTypeConstraint constraints
            if isTypeExtensionOrAlias then
                addLongIdent longIdent

        and walkTypeDefnRepr = function
            | SynTypeDefnRepr.ObjectModel (_, defns, _) -> List.iter walkMember defns
            | SynTypeDefnRepr.Simple(defn, _) -> walkTypeDefnSimple defn
            | SynTypeDefnRepr.Exception _ -> ()

        and walkTypeDefnSigRepr = function
            | SynTypeDefnSigRepr.ObjectModel (_, defns, _) -> List.iter walkMemberSig defns
            | SynTypeDefnSigRepr.Simple(defn, _) -> walkTypeDefnSimple defn
            | SynTypeDefnSigRepr.Exception _ -> ()

        and walkTypeDefn (TypeDefn (info, repr, members, _)) =
            let isTypeExtensionOrAlias =
                match repr with
                | SynTypeDefnRepr.ObjectModel (SynTypeDefnKind.TyconAugmentation, _, _)
                | SynTypeDefnRepr.ObjectModel (SynTypeDefnKind.TyconAbbrev, _, _)
                | SynTypeDefnRepr.Simple (SynTypeDefnSimpleRepr.TypeAbbrev _, _) -> true
                | _ -> false
            walkComponentInfo isTypeExtensionOrAlias info
            walkTypeDefnRepr repr
            List.iter walkMember members

        and walkSynModuleDecl (decl: SynModuleDecl) =
            match decl with
            | SynModuleDecl.NamespaceFragment fragment -> walkSynModuleOrNamespace fragment
            | SynModuleDecl.NestedModule (info, _, modules, _, _) ->
                walkComponentInfo false info
                List.iter walkSynModuleDecl modules
            | SynModuleDecl.Let (_, bindings, _) -> List.iter walkBinding bindings
            | SynModuleDecl.DoExpr (_, expr, _) -> walkExpr expr
            | SynModuleDecl.Types (types, _) -> List.iter walkTypeDefn types
            | SynModuleDecl.Attributes (attrs, _) -> List.iter walkAttribute attrs
            | _ -> ()

        match input with
        | Some (ParsedInput.ImplFile input) ->
             walkImplFileInput input
        | _ -> ()
        //debug "%A" idents
        identsByEndPos :> _


    /// Get path to containing module/namespace of a given position
    let getModuleOrNamespacePath (pos: pos) (ast: ParsedInput) =
        let idents =
            match ast with
            | ParsedInput.ImplFile (ParsedImplFileInput(_, _, _, _, _, modules, _)) ->
                let rec walkModuleOrNamespace idents (decls, moduleRange) =
                    decls
                    |> List.fold (fun acc ->
                        function
                        | SynModuleDecl.NestedModule (componentInfo, _, nestedModuleDecls, _, nestedModuleRange) ->
                            if rangeContainsPos moduleRange pos then
                                let (ComponentInfo(_,_,_,longIdent,_,_,_,_)) = componentInfo
                                walkModuleOrNamespace (longIdent::acc) (nestedModuleDecls, nestedModuleRange)
                            else acc
                        | _ -> acc) idents

                modules
                |> List.fold (fun acc (SynModuleOrNamespace(longIdent, _, _, decls, _, _, _, moduleRange)) ->
                        if rangeContainsPos moduleRange pos then
                            walkModuleOrNamespace (longIdent::acc) (decls, moduleRange) @ acc
                        else acc) []
            | ParsedInput.SigFile(ParsedSigFileInput(_, _, _, _, modules)) ->
                let rec walkModuleOrNamespaceSig idents (decls, moduleRange) =
                    decls
                    |> List.fold (fun acc ->
                        function
                        | SynModuleSigDecl.NestedModule (componentInfo, _, nestedModuleDecls, nestedModuleRange) ->
                            if rangeContainsPos moduleRange pos then
                                let (ComponentInfo(_,_,_,longIdent,_,_,_,_)) = componentInfo
                                walkModuleOrNamespaceSig (longIdent::acc) (nestedModuleDecls, nestedModuleRange)
                            else acc
                        | _ -> acc) idents

                modules
                |> List.fold (fun acc (SynModuleOrNamespaceSig(longIdent, _, _, decls, _, _, _, moduleRange)) ->
                        if rangeContainsPos moduleRange pos then
                            walkModuleOrNamespaceSig (longIdent::acc) (decls, moduleRange) @ acc
                        else acc) []
        idents
        |> List.rev
        |> Seq.concat
        |> Seq.map (fun ident -> ident.idText)
        |> String.concat "."