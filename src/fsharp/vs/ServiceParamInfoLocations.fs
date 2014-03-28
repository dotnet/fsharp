// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Open up the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.
//--------------------------------------------------------------------------

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open Internal.Utilities.Debug
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.Ast

[<Sealed>]
type internal NoteworthyParamInfoLocations(longId : string list, longIdStartLocation : int*int, longIdEndLocation : int*int, openParenLocation : int*int, 
                                           tupleEndLocations : (int*int)[], isThereACloseParen : bool, namedParamNames : string[]) =
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
    member this.LongIdStartLocation = longIdStartLocation
    member this.LongIdEndLocation = longIdEndLocation
    member this.OpenParenLocation = openParenLocation
    member this.TupleEndLocations = tupleEndLocations
    member this.IsThereACloseParen = isThereACloseParen
    member this.NamedParamNames = namedParamNames

module internal NoteworthyParamInfoLocationsImpl =

    let isStaticArg a =
        match a with
        | SynType.StaticConstant _ | SynType.StaticConstantExpr _ | SynType.StaticConstantNamed _ -> true
        | SynType.LongIdent _ -> true // NOTE: this is not a static constant, but it is a prefix of incomplete code, e.g. "TP<42,Arg3" is a prefix of "TP<42,Arg3=6>" and Arg3 shows up as a LongId
        | _ -> false

    let traverseInput(line,col,parseTree) : NoteworthyParamInfoLocations option =
        let pos = Pos.fromVS line col  // line was 0-based, need 1-based

        let rec digOutIdentStartEndFromAnApp synExpr =
            // we found it, dig out ident
            match synExpr with
            | SynExpr.Ident(id) ->
                let r = [id.idText], [(id.idRange.StartLine, id.idRange.StartColumn+1); (id.idRange.EndLine, id.idRange.EndColumn+1)] // +1 because col are 0-based, but want 1-based
                Trace.PrintLine("LanguageServiceParamInfo", fun () -> sprintf "Dug out ident at range %+A from %+A" r synExpr)
                Some r
            | SynExpr.LongIdent(_, LongIdentWithDots(lid,_), _, lidRange) -> 
                let r = (lid |> List.map (fun id -> id.idText)), [(lidRange.StartLine, lidRange.StartColumn+1); (lidRange.EndLine, lidRange.EndColumn+1)] // +1 because col are 0-based, but want 1-based
                Trace.PrintLine("LanguageServiceParamInfo", fun () -> sprintf "Dug out ident at range %+A from %+A" r synExpr)
                Some r
            | SynExpr.DotGet(_expr, _dotm, LongIdentWithDots(lid,_), range) -> 
                let r = (lid |> List.map (fun id -> id.idText)), [(range.StartLine, range.StartColumn+1); (range.EndLine, range.EndColumn+1)] // +1 because col are 0-based, but want 1-based
                Trace.PrintLine("LanguageServiceParamInfo", fun () -> sprintf "Dug out ident at range %+A from %+A" r synExpr)
                Some r
            | SynExpr.TypeApp(synExpr, _, _synTypeList, _commas, _, _, _range) -> // TODO?
                match digOutIdentStartEndFromAnApp synExpr with
                | Some(_lid, [(_sl,_sc); (_el,_ec)]) as r->
                    r  // Note: we record the ident-end after the ident but before the typeargs, e.g. $ not ^ in "foo$<int,string>^("
                | x -> 
                    ignore(x)
                    None
            | x ->
                ignore(x)
                None

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

        let rec astFindNoteworthyParamInfoLocationsSynExprExactParen traverseSynExpr expr =
            // This method returns a tuple, where the second element is
            //     Some(cache)    if the implementation called 'traverseSynExpr expr', then 'cache' is the result of that call
            //     None           otherwise
            // so that callers can avoid recomputing 'traverseSynExpr expr' if it's already been done.  This is very important for perf, 
            // see bug 345385.
            let handleSingleArg(synExpr, parenRange, rpRangeOpt : _ option) =
                let inner = traverseSynExpr synExpr
                match inner with
                | None ->
                    if AstTraversal.rangeContainsPosEdgesExclusive parenRange pos then
                        let r = (parenRange.StartLine, parenRange.StartColumn+1), 
                                    [parenRange.EndLine, parenRange.EndColumn+1, getNamedParamName synExpr],  // +1 because col are 0-based, but want 1-based
                                    rpRangeOpt.IsSome
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
                        let r = (parenRange.StartLine, parenRange.StartColumn+1), 
                                  ((synExprList,commaRanges@[parenRange]) ||> List.map2 (fun e c -> c.EndLine, c.EndColumn+1, getNamedParamName e)),  // +1 because col are 0-based, but want 1-based
                                  rpRangeOpt.IsSome
                        Trace.PrintLine("LanguageServiceParamInfo", fun () -> sprintf "Found paren tuple ranges %+A from %+A" r expr)
                        Some r, None
                    else
                        None, None
                | _ -> None, None
            | SynExprParen(SynExprParen(SynExpr.Tuple(_,_,_),_,_,_) as synExpr, _, rpRangeOpt, parenRange) -> // f((x,y)) is special, single tuple arg
                handleSingleArg(synExpr,parenRange,rpRangeOpt)
            | SynExprParen(SynExprParen(_,_,_,_) as synExpr, _, _, _parenRange) -> // dig into multiple parens
                let r,_cacheOpt = astFindNoteworthyParamInfoLocationsSynExprExactParen traverseSynExpr synExpr
                r, None
            | SynExprParen(synExpr, _lpRange, rpRangeOpt, parenRange) -> // single argument
                handleSingleArg(synExpr,parenRange,rpRangeOpt)
            | SynExpr.ArbitraryAfterError(_debugStr, range) -> // single argument when e.g. after open paren you hit EOF
                if AstTraversal.rangeContainsPosEdgesExclusive range pos then
                    let r = (range.StartLine, range.StartColumn+1), [range.EndLine, range.EndColumn+1, null], false  // +1 because col are 0-based, but want 1-based
                    Trace.PrintLine("LanguageServiceParamInfo", fun () -> sprintf "Found ArbitraryAfterError range %+A from %+A" r expr)
                    Some r, None
                else
                    None, None
            | SynExpr.Const(SynConst.Unit, unitRange) ->
                if AstTraversal.rangeContainsPosEdgesExclusive unitRange pos then
                    let r = (unitRange.StartLine, unitRange.StartColumn+1), [unitRange.EndLine, unitRange.EndColumn+1, null], true  // +1 because col are 0-based, but want 1-based
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
                        let r = (e.Range.StartLine, e.Range.StartColumn+1), [e.Range.EndLine, e.Range.EndColumn+1, null], false  // +1 because col are 0-based, but want 1-based
                        Trace.PrintLine("LanguageServiceParamInfo", fun () -> sprintf "Found non-parenthesized single arg range %+A from %+A" r expr)
                        Some r, Some inner
                    else
                        None, Some inner
                | _ -> None, Some inner

        let getTypeName(synType) =
            match synType with
            | SynType.LongIdent(LongIdentWithDots(ids,_)) -> ids |> List.map (fun id -> id.idText)
            | _ -> [""] // TODO type name for other cases, see also unit test named "ParameterInfo.LocationOfParams.AfterQuicklyTyping.CallConstructorViaLongId.Bug94333"

        AstTraversal.Traverse(line,col,parseTree, { new AstTraversal.AstVisitorBase<_>() with
        member this.VisitExpr(_path, traverseSynExpr, defaultTraverse, expr) =
            let expr = expr // fix debug locals
            match expr with
            | SynExpr.New(_, synType, synExpr, _range) -> // TODO walk SynType
                let constrArgsResult,cacheOpt = astFindNoteworthyParamInfoLocationsSynExprExactParen traverseSynExpr synExpr
                match constrArgsResult,cacheOpt with
                | Some(parenLoc,args,isThereACloseParen), _ ->
                    let typename = getTypeName synType
                    let r = NoteworthyParamInfoLocations(typename, (synType.Range.StartLine, synType.Range.StartColumn+1), (synType.Range.EndLine, synType.Range.EndColumn+1), // +1 because col are 0-based, but want 1-based
                                                         parenLoc, args |> Seq.map (fun (l,c,_n) -> l,c) |> Seq.toArray, isThereACloseParen, args |> Seq.map (fun (_l,_c,n) -> n) |> Seq.toArray)
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
                    let xResult,cacheOpt = astFindNoteworthyParamInfoLocationsSynExprExactParen traverseSynExpr synExpr2
                    match xResult,cacheOpt with
                    | Some(parenLoc,args,isThereACloseParen),_ ->
                        // we found it, dig out ident
                        match digOutIdentStartEndFromAnApp synExpr with
                        | Some(lid,[lidStart; lidEnd]) -> 
                            assert(isInfix = (parenLoc < lidEnd))
                            if isInfix then
                                // This seems to be an infix operator, since the start of the argument is a position earlier than the end of the long-id being applied to it.
                                // For now, we don't support infix operators.
                                Trace.PrintLine("LanguageServiceParamInfo", fun () -> sprintf "Found apparent infix operator, ignoring dug-out ident from %+A" expr)
                                None
                            else
                                let r = NoteworthyParamInfoLocations(lid, lidStart, lidEnd, parenLoc, args |> Seq.map (fun (l,c,_n) -> l,c) |> Seq.toArray, 
                                                                     isThereACloseParen, args |> Seq.map (fun (_l,_c,n) -> n) |> Seq.toArray)
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
                        // +1s because col are 0-based, but want 1-based
                        let r = NoteworthyParamInfoLocations(["dummy"], // TODO synExpr, but LongId?
                                                             (synExpr.Range.StartLine, synExpr.Range.StartColumn+1),
                                                             (synExpr.Range.EndLine, synExpr.Range.EndColumn+1), 
                                                             (openm.StartLine, openm.StartColumn+1), 
                                                             commas |> Seq.map (fun c -> c.EndLine, c.EndColumn+1) 
                                                                    |> (fun cs -> Seq.append cs [(wholem.EndLine, wholem.EndColumn+1)] )
                                                                    |> Seq.toArray, 
                                                             Option.isSome closemOpt, 
                                                             tyArgs |> Seq.map (function 
                                                                              | SynType.StaticConstantNamed(SynType.LongIdent(LongIdentWithDots([id],_)),_,_) -> id.idText 
                                                                              | SynType.LongIdent(LongIdentWithDots([id],_)) -> id.idText // NOTE: again, not a static constant, but may be a prefix of a Named in incomplete code
                                                                              | _ -> null
                                                                              ) |> Seq.toArray)
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
                    // +1s because col are 0-based, but want 1-based
                    let r = NoteworthyParamInfoLocations(lid |> List.map (fun id -> id.idText), 
                                                            (lidm.StartLine, lidm.StartColumn+1),
                                                            (lidm.EndLine, lidm.EndColumn+1), 
                                                            (openm.StartLine, openm.StartColumn+1), 
                                                            commas |> Seq.map (fun c -> c.EndLine, c.EndColumn+1) 
                                                                |> (fun cs -> Seq.append cs [(wholem.EndLine, wholem.EndColumn+1)] )
                                                                |> Seq.toArray, 
                                                            Option.isSome closemOpt, 
                                                            args |> Seq.map (function 
                                                                            | SynType.StaticConstantNamed(SynType.LongIdent(LongIdentWithDots([id],_)),_,_) -> id.idText 
                                                                            | SynType.LongIdent(LongIdentWithDots([id],_)) -> id.idText // NOTE: again, not a static constant, but may be a prefix of a Named in incomplete code
                                                                            | _ -> null
                                                                            ) |> Seq.toArray)
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
                    // inherit ty(expr)    ---   treate it like an application (constructor call)
                    let xResult,_cacheOpt = astFindNoteworthyParamInfoLocationsSynExprExactParen defaultTraverse expr
                    match xResult with
                    | Some(parenLoc,args,isThereACloseParen) ->
                        // we found it, dig out ident
                        let typename = getTypeName ty
                        let r = NoteworthyParamInfoLocations(typename, (ty.Range.StartLine, ty.Range.StartColumn+1), (ty.Range.EndLine, ty.Range.EndColumn+1), // +1 because col are 0-based, but want 1-based
                                                             parenLoc, args |> Seq.map (fun (l,c,_n) -> l,c) |> Seq.toArray, 
                                                             isThereACloseParen, args |> Seq.map (fun (_l,_c,n) -> n) |> Seq.toArray)
                        Trace.PrintLine("LanguageServiceParamInfo", fun () -> sprintf "Found app with ranges %+A from %+A" r expr)
                        Some r
                    | _ -> None
                else None
        })

    let FindNoteworthyParamInfoLocations(line,col,parseTree) =
        match traverseInput(line,col,parseTree) with
        | Some(nwpl) as r-> 
#if DEBUG
            let ranges = nwpl.LongIdStartLocation :: nwpl.LongIdEndLocation :: nwpl.OpenParenLocation :: (nwpl.TupleEndLocations |> Array.toList)
            let sorted = ranges |> Seq.sort |> Seq.toList
            assert(ranges = sorted)
#else
            ignore nwpl
#endif                        
            r
        | _ -> None

