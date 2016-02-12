// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open Internal.Utilities.Debug
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.Ast

[<Sealed>]
type FSharpNoteworthyParamInfoLocations(longId : string list, 
                                        longIdRange: range,
                                        openParenLocation : pos, 
                                        tupleEndLocations : pos list, 
                                        isThereACloseParen : bool, 
                                        namedParamNames : string list) =

    let tupleEndLocations = Array.ofList tupleEndLocations
    let namedParamNames = Array.ofList namedParamNames
    let namedParamNames =
        if (tupleEndLocations.Length = namedParamNames.Length) then
            namedParamNames
        else
            // in cases like    TP<   or   TP<42,   there is no 'arbitrary type' that represents the last missing static argument
            // this is ok, but later code in the UI layer will expect these lengths to match
            // so just fill in a blank named param to represent the final missing param
            // (compare to    f(   or   f(42,   where the parser injects a fake "AbrExpr" to represent the missing argument)
            assert(tupleEndLocations.Length = namedParamNames.Length + 1)
            [| yield! namedParamNames; yield null |]  // "null" is representation of a non-named param
    member this.LongId = longId
    member this.LongIdStartLocation = longIdRange.Start
    member this.LongIdEndLocation = longIdRange.End
    member this.OpenParenLocation = openParenLocation
    member this.TupleEndLocations = tupleEndLocations
    member this.IsThereACloseParen = isThereACloseParen
    member this.NamedParamNames = namedParamNames

[<AutoOpen>]
module internal NoteworthyParamInfoLocationsImpl =

    let isStaticArg a =
        match a with
        | SynType.StaticConstant _ | SynType.StaticConstantExpr _ | SynType.StaticConstantNamed _ -> true
        | SynType.LongIdent _ -> true // NOTE: this is not a static constant, but it is a prefix of incomplete code, e.g. "TP<42,Arg3" is a prefix of "TP<42,Arg3=6>" and Arg3 shows up as a LongId
        | _ -> false

    let rec digOutIdentFromApp synExpr =
        // we found it, dig out ident
        match synExpr with
        | SynExpr.Ident(id) -> Some ([id.idText], id.idRange)
        | SynExpr.LongIdent(_, LongIdentWithDots(lid,_), _, lidRange) -> Some (lid |> List.map textOfId, lidRange)
        | SynExpr.DotGet(_expr, _dotm, LongIdentWithDots(lid,_), range) -> Some (lid |> List.map textOfId, range)
        | SynExpr.TypeApp(synExpr, _, _synTypeList, _commas, _, _, _range) -> digOutIdentFromApp synExpr 
        | _ -> None

    let digOutIdentFromStaticArg synType =
        match synType with 
        | SynType.StaticConstantNamed(SynType.LongIdent(LongIdentWithDots([id],_)),_,_) -> id.idText 
        | SynType.LongIdent(LongIdentWithDots([id],_)) -> id.idText // NOTE: again, not a static constant, but may be a prefix of a Named in incomplete code
        | _ -> null 

    let getNamedParamName e =
        match e with
        // f(x=4)
        | SynExpr.App(ExprAtomicFlag.NonAtomic, _,
                        SynExpr.App(ExprAtomicFlag.NonAtomic, true,
                                    SynExpr.Ident op, 
                                    SynExpr.Ident n, 
                                    _range),
                        _, _) when op.idText="op_Equality" -> n.idText
        // f(?x=4)
        | SynExpr.App(ExprAtomicFlag.NonAtomic, _,
                        SynExpr.App(ExprAtomicFlag.NonAtomic, true,
                                    SynExpr.Ident op, 
                                    SynExpr.LongIdent(true(*isOptional*),LongIdentWithDots([n],_),_ref,_lidrange), _range), 
                        _, _) when op.idText="op_Equality" -> n.idText
        | _ -> null

    let getTypeName(synType) =
        match synType with
        | SynType.LongIdent(LongIdentWithDots(ids,_)) -> ids |> List.map textOfId
        | _ -> [""] // TODO type name for other cases, see also unit test named "ParameterInfo.LocationOfParams.AfterQuicklyTyping.CallConstructorViaLongId.Bug94333"

    // This method returns a tuple, where the second element is
    //     Some(cache)    if the implementation called 'traverseSynExpr expr', then 'cache' is the result of that call
    //     None           otherwise
    // so that callers can avoid recomputing 'traverseSynExpr expr' if it's already been done.  This is very important for perf, 
    // see bug 345385.
    let rec astFindNoteworthyParamInfoLocationsSynExprExactParen traverseSynExpr pos expr =
        let handleSingleArg(synExpr, parenRange, rpRangeOpt : _ option) =
            let inner = traverseSynExpr synExpr
            match inner with
            | None ->
                if AstTraversal.rangeContainsPosEdgesExclusive parenRange pos then
                    let r = parenRange.Start, [parenRange.End, getNamedParamName synExpr], rpRangeOpt.IsSome
                    Trace.PrintLine("LanguageServiceParamInfo", fun () -> sprintf "Found single arg paren range %+A from %+A" r expr)
                    Some r, None
                else
                    None, None
            | _ -> None, None

        match expr with 
        | SynExprParen((SynExpr.Tuple(synExprList, commaRanges, _tupleRange) as synExpr), _lpRange, rpRangeOpt, parenRange) -> // tuple argument
            let inner = traverseSynExpr synExpr
            match inner with
            | None ->
                if AstTraversal.rangeContainsPosEdgesExclusive parenRange pos then
                    let r = parenRange.Start, ((synExprList,commaRanges@[parenRange]) ||> List.map2 (fun e c -> c.End, getNamedParamName e)), rpRangeOpt.IsSome
                    Trace.PrintLine("LanguageServiceParamInfo", fun () -> sprintf "Found paren tuple ranges %+A from %+A" r expr)
                    Some r, None
                else
                    None, None
            | _ -> None, None

        | SynExprParen(SynExprParen(SynExpr.Tuple(_,_,_),_,_,_) as synExpr, _, rpRangeOpt, parenRange) -> // f((x,y)) is special, single tuple arg
            handleSingleArg(synExpr,parenRange,rpRangeOpt)

        // dig into multiple parens
        | SynExprParen(SynExprParen(_,_,_,_) as synExpr, _, _, _parenRange) -> 
            let r,_cacheOpt = astFindNoteworthyParamInfoLocationsSynExprExactParen traverseSynExpr pos synExpr
            r, None

        | SynExprParen(synExpr, _lpRange, rpRangeOpt, parenRange) -> // single argument
            handleSingleArg(synExpr,parenRange,rpRangeOpt)

        | SynExpr.ArbitraryAfterError(_debugStr, range) -> // single argument when e.g. after open paren you hit EOF
            if AstTraversal.rangeContainsPosEdgesExclusive range pos then
                let r = range.Start, [range.End, null], false  
                Trace.PrintLine("LanguageServiceParamInfo", fun () -> sprintf "Found ArbitraryAfterError range %+A from %+A" r expr)
                Some r, None
            else
                None, None

        | SynExpr.Const(SynConst.Unit, unitRange) ->
            if AstTraversal.rangeContainsPosEdgesExclusive unitRange pos then
                let r = unitRange.Start, [unitRange.End, null], true 
                Trace.PrintLine("LanguageServiceParamInfo", fun () -> sprintf "Found unit range %+A from %+A" r expr)
                Some r, None
            else
                None, None

        | e -> 
            let inner = traverseSynExpr e
            match inner with
            | None ->
                if AstTraversal.rangeContainsPosEdgesExclusive e.Range pos then
                    // any other expression doesn't start with parens, so if it was the target of an App, then it must be a single argument e.g. "f x"
                    let r = e.Range.Start, [e.Range.End, null], false
                    Trace.PrintLine("LanguageServiceParamInfo", fun () -> sprintf "Found non-parenthesized single arg range %+A from %+A" r expr)
                    Some r, Some inner
                else
                    None, Some inner
            | _ -> None, Some inner

    let traverseInput(pos,parseTree) : FSharpNoteworthyParamInfoLocations option =

        AstTraversal.Traverse(pos,parseTree, { new AstTraversal.AstVisitorBase<_>() with
        member this.VisitExpr(_path, traverseSynExpr, defaultTraverse, expr) =
            let expr = expr // fix debug locals
            match expr with
            | SynExpr.New(_, synType, synExpr, _range) -> // TODO walk SynType
                let constrArgsResult,cacheOpt = astFindNoteworthyParamInfoLocationsSynExprExactParen traverseSynExpr pos synExpr
                match constrArgsResult,cacheOpt with
                | Some(parenLoc,args,isThereACloseParen), _ ->
                    let typename = getTypeName synType
                    let r = FSharpNoteworthyParamInfoLocations(typename, synType.Range, parenLoc, args |> List.map fst, isThereACloseParen, args |> List.map snd)
                    Trace.PrintLine("LanguageServiceParamInfo", fun () -> sprintf "Found 'new' call with ranges %+A from %+A" r expr)
                    Some(r)
                | None, Some(cache) ->
                    cache
                | _ ->
                    traverseSynExpr synExpr
            | SynExpr.App(_exprAtomicFlag, isInfix, synExpr, synExpr2, _range) ->
                let fResult = traverseSynExpr synExpr
                match fResult with
                | Some(_) -> fResult
                | _ ->
                    let xResult,cacheOpt = astFindNoteworthyParamInfoLocationsSynExprExactParen traverseSynExpr pos synExpr2
                    match xResult,cacheOpt with
                    | Some(parenLoc,args,isThereACloseParen),_ ->
                        // we found it, dig out ident
                        match digOutIdentFromApp synExpr with
                        | Some(lid,lidRange) -> 
                            assert(isInfix = (posLt parenLoc lidRange.End))
                            if isInfix then
                                // This seems to be an infix operator, since the start of the argument is a position earlier than the end of the long-id being applied to it.
                                // For now, we don't support infix operators.
                                Trace.PrintLine("LanguageServiceParamInfo", fun () -> sprintf "Found apparent infix operator, ignoring dug-out ident from %+A" expr)
                                None
                            else
                                let r = FSharpNoteworthyParamInfoLocations(lid, lidRange, parenLoc, args |> List.map fst, isThereACloseParen, args |> List.map snd)
                                Trace.PrintLine("LanguageServiceParamInfo", fun () -> sprintf "Found app with ranges %+A from %+A" r expr)
                                Some r
                        | x ->
                            ignore(x)
                            None
                    | None, Some(cache) -> cache
                    | _ -> traverseSynExpr synExpr2

            | SynExpr.TypeApp(synExpr, openm, tyArgs, commas, closemOpt, _, wholem) as seta ->
                match traverseSynExpr synExpr with
                | Some _ as r -> r
                | None -> 
                    let typeArgsm = mkRange openm.FileName openm.Start wholem.End 
                    if AstTraversal.rangeContainsPosEdgesExclusive typeArgsm pos && tyArgs |> List.forall isStaticArg then
                        let r = FSharpNoteworthyParamInfoLocations(["dummy"], // TODO synExpr, but LongId?
                                                             synExpr.Range, 
                                                             openm.Start, 
                                                             [ for c in commas -> c.End
                                                               yield wholem.End ], 
                                                             closemOpt.IsSome, 
                                                             tyArgs |> List.map digOutIdentFromStaticArg)
                        Trace.PrintLine("LanguageServiceParamInfo", fun () -> sprintf "Found SynExpr.TypeApp with ranges %+A from %+A" r seta)
                        Some r
                    else
                        None

            | _ -> defaultTraverse expr

        member this.VisitTypeAbbrev(tyAbbrevRhs,_m) =
            match tyAbbrevRhs with
            | SynType.App(SynType.LongIdent(LongIdentWithDots(lid,_) as lidwd), Some(openm), args, commas, closemOpt, _pf, wholem) ->
                let lidm = lidwd.Range
                let betweenTheBrackets = mkRange wholem.FileName openm.Start wholem.End
                if AstTraversal.rangeContainsPosEdgesExclusive betweenTheBrackets pos && args |> List.forall isStaticArg then
                    let r = FSharpNoteworthyParamInfoLocations(lid |> List.map textOfId, lidm, openm.Start, 
                                                            [ for c in commas -> c.End
                                                              yield wholem.End ], 
                                                            closemOpt.IsSome, 
                                                            args |> List.map digOutIdentFromStaticArg)
                    Trace.PrintLine("LanguageServiceParamInfo", fun () -> sprintf "Found type abbrev ty-app with ranges %+A from %+A" r tyAbbrevRhs)
                    Some r
                else
                    None
            | _ ->
                None

        member this.VisitImplicitInherit(defaultTraverse, ty, expr, m) =
            match defaultTraverse expr with
            | Some _ as r -> r
            | None ->
                let inheritm = mkRange m.FileName m.Start m.End 
                if AstTraversal.rangeContainsPosEdgesExclusive inheritm pos then
                    // inherit ty(expr)    ---   treat it like an application (constructor call)
                    let xResult,_cacheOpt = astFindNoteworthyParamInfoLocationsSynExprExactParen defaultTraverse pos expr
                    match xResult with
                    | Some(parenLoc,args,isThereACloseParen) ->
                        // we found it, dig out ident
                        let typename = getTypeName ty
                        let r = FSharpNoteworthyParamInfoLocations(typename, ty.Range, parenLoc, args |> List.map fst, isThereACloseParen, args |> List.map snd)
                        Trace.PrintLine("LanguageServiceParamInfo", fun () -> sprintf "Found app with ranges %+A from %+A" r expr)
                        Some r
                    | _ -> None
                else None
        })

type FSharpNoteworthyParamInfoLocations with 
    static member Find(pos,parseTree) =
        match traverseInput(pos,parseTree) with
        | Some nwpl as r -> 
#if DEBUG
            let ranges = nwpl.LongIdStartLocation :: nwpl.LongIdEndLocation :: nwpl.OpenParenLocation :: (nwpl.TupleEndLocations |> Array.toList)
            let sorted = ranges |> List.sortWith (fun a b -> posOrder.Compare(a,b)) |> Seq.toList
            assert(ranges = sorted)
#else
            ignore nwpl
#endif                        
            r
        | _ -> None

