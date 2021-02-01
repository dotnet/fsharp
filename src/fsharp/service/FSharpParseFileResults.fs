// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO
open System.Collections.Generic
open System.Diagnostics
open Internal.Utilities.Library  
open Internal.Utilities.Library.Extras
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range

/// Methods for dealing with F# sources files.
module FSharpSourceFile =

    /// Source file extensions
    let private compilableExtensions = FSharpSigFileSuffixes @ FSharpImplFileSuffixes @ FSharpScriptFileSuffixes

    /// Single file projects extensions
    let private singleFileProjectExtensions = FSharpScriptFileSuffixes

    /// Whether or not this file is compilable
    let IsCompilable file =
        let ext = Path.GetExtension file
        compilableExtensions |> List.exists(fun e->0 = String.Compare(e, ext, StringComparison.OrdinalIgnoreCase))

    /// Whether or not this file should be a single-file project
    let MustBeSingleFileProject file =
        let ext = Path.GetExtension file
        singleFileProjectExtensions |> List.exists(fun e-> 0 = String.Compare(e, ext, StringComparison.OrdinalIgnoreCase))

module SourceFileImpl =
    let IsInterfaceFile file =
        let ext = Path.GetExtension file
        0 = String.Compare(".fsi", ext, StringComparison.OrdinalIgnoreCase)

    /// Additional #defines that should be in place when editing a file in a file editor such as VS.
    let AdditionalDefinesForUseInEditor(isInteractive: bool) =
        if isInteractive then ["INTERACTIVE";"EDITING"] // This is still used by the foreground parse
        else ["COMPILED";"EDITING"]
           
type FSharpCompletionPath = string list * string option // plid * residue

[<RequireQualifiedAccess>]
type FSharpInheritanceOrigin = 
    | Class
    | Interface
    | Unknown

[<RequireQualifiedAccess>]
type FSharpInheritanceContext = 
    | Class
    | Interface
    | Unknown

[<RequireQualifiedAccess>]
type FSharpRecordContext =
    | CopyOnUpdate of range: range * path: FSharpCompletionPath
    | Constructor of typeName: string
    | New of path: FSharpCompletionPath

[<RequireQualifiedAccess>]
type FSharpCompletionContext = 
    /// Completion context cannot be determined due to errors
    | Invalid

    /// Completing something after the inherit keyword
    | Inherit of context: FSharpInheritanceContext * path: FSharpCompletionPath

    /// Completing records field
    | RecordField of context: FSharpRecordContext

    | RangeOperator

    /// Completing named parameters\setters in parameter list of constructor\method calls
    /// end of name ast node * list of properties\parameters that were already set
    | ParameterList of pos * HashSet<string>

    | AttributeApplication

    | OpenDeclaration of isOpenType: bool

    /// Completing pattern type (e.g. foo (x: |))
    | PatternType

//----------------------------------------------------------------------------
// FSharpParseFileResults
//----------------------------------------------------------------------------

[<Sealed>]
type FSharpParseFileResults(diagnostics: FSharpDiagnostic[], input: ParsedInput option, parseHadErrors: bool, dependencyFiles: string[]) = 

    member _.Diagnostics = diagnostics

    member _.ParseHadErrors = parseHadErrors

    member _.ParseTree = input

    member scope.TryRangeOfNameOfNearestOuterBindingContainingPos pos =
        let tryGetIdentRangeFromBinding binding =
            match binding with
            | SynBinding.Binding (_, _, _, _, _, _, _, headPat, _, _, _, _) ->
                match headPat with
                | SynPat.LongIdent (longIdentWithDots, _, _, _, _, _) ->
                    Some longIdentWithDots.Range
                | SynPat.Named(_, ident, false, _, _) ->
                    Some ident.idRange
                | _ ->
                    None

        let rec walkBinding expr workingRange =
            match expr with

            // This lets us dive into subexpressions that may contain the binding we're after
            | SynExpr.Sequential (_, _, expr1, expr2, _) ->
                if rangeContainsPos expr1.Range pos then
                    walkBinding expr1 workingRange
                else
                    walkBinding expr2 workingRange


            | SynExpr.LetOrUse(_, _, bindings, bodyExpr, _) ->
                let potentialNestedRange =
                    bindings
                    |> List.tryFind (fun binding -> rangeContainsPos binding.RangeOfBindingAndRhs pos)
                    |> Option.bind tryGetIdentRangeFromBinding
                match potentialNestedRange with
                | Some range ->
                    walkBinding bodyExpr range
                | None ->
                    walkBinding bodyExpr workingRange

            
            | _ ->
                Some workingRange

        match scope.ParseTree with
        | Some input ->
            SyntaxTraversal.Traverse(pos, input, { new SyntaxVisitorBase<_>() with
                member _.VisitExpr(_, _, defaultTraverse, expr) =                        
                    defaultTraverse expr

                override _.VisitBinding(_path, defaultTraverse, binding) =
                    match binding with
                    | SynBinding.Binding (_, _, _, _, _, _, _, _, _, expr, _range, _) as b when rangeContainsPos b.RangeOfBindingAndRhs pos ->
                        match tryGetIdentRangeFromBinding b with
                        | Some range -> walkBinding expr range
                        | None -> None
                    | _ -> defaultTraverse binding })
        | None -> None
    
    member scope.TryIdentOfPipelineContainingPosAndNumArgsApplied pos =
        match scope.ParseTree with
        | Some input ->
            SyntaxTraversal.Traverse(pos, input, { new SyntaxVisitorBase<_>() with
                member _.VisitExpr(_, _, defaultTraverse, expr) =
                    match expr with
                    | SynExpr.App (_, _, SynExpr.App(_, true, SynExpr.Ident ident, _, _), argExpr, _) when rangeContainsPos argExpr.Range pos ->
                        match argExpr with
                        | SynExpr.App(_, _, _, SynExpr.Paren(expr, _, _, _), _) when rangeContainsPos expr.Range pos ->
                            None
                        | _ ->
                            if ident.idText = "op_PipeRight" then
                                Some (ident, 1)
                            elif ident.idText = "op_PipeRight2" then
                                Some (ident, 2)
                            elif ident.idText = "op_PipeRight3" then
                                Some (ident, 3)
                            else
                                None
                    | _ -> defaultTraverse expr
            })
        | None -> None
    
    member scope.IsPosContainedInApplication pos =
        match scope.ParseTree with
        | Some input ->
            let result =
                SyntaxTraversal.Traverse(pos, input, { new SyntaxVisitorBase<_>() with
                    member _.VisitExpr(_, traverseSynExpr, defaultTraverse, expr) =
                        match expr with
                        | SynExpr.App(_, _, _, SynExpr.CompExpr (_, _, expr, _), range) when rangeContainsPos range pos ->
                            traverseSynExpr expr
                        | SynExpr.App (_, _, _, _, range) when rangeContainsPos range pos ->
                            Some range
                        | _ -> defaultTraverse expr
                })
            result.IsSome
        | None -> false

    member scope.TryRangeOfFunctionOrMethodBeingApplied pos =
        let rec getIdentRangeForFuncExprInApp traverseSynExpr expr pos =
            match expr with
            | SynExpr.Ident ident -> Some ident.idRange
            
            | SynExpr.LongIdent(_, _, _, range) -> Some range

            | SynExpr.Paren(expr, _, _, range) when rangeContainsPos range pos ->
                getIdentRangeForFuncExprInApp traverseSynExpr expr pos

            // This matches computation expressions like 'async { ... }'
            | SynExpr.App(_, _, _, SynExpr.CompExpr (_, _, expr, range), _) when rangeContainsPos range pos ->
                getIdentRangeForFuncExprInApp traverseSynExpr expr pos

            | SynExpr.App(_, _, funcExpr, argExpr, _) ->
                match argExpr with
                | SynExpr.App (_, _, _, _, range) when rangeContainsPos range pos ->
                    getIdentRangeForFuncExprInApp traverseSynExpr argExpr pos

                | SynExpr.Paren(SynExpr.Lambda(_, _, _args, body, _, _), _, _, _) when rangeContainsPos body.Range pos -> 
                    getIdentRangeForFuncExprInApp traverseSynExpr body pos

                | _ ->
                    match funcExpr with
                    | SynExpr.App (_, true, _, _, _) when rangeContainsPos argExpr.Range pos ->
                        // x |> List.map 
                        // Don't dive into the funcExpr (the operator expr)
                        // because we dont want to offer sig help for that!
                        getIdentRangeForFuncExprInApp traverseSynExpr argExpr pos
                    | _ ->
                        // Generally, we want to dive into the func expr to get the range
                        // of the identifier of the function we're after
                        getIdentRangeForFuncExprInApp traverseSynExpr funcExpr pos

            | SynExpr.LetOrUse(_, _, bindings, body, range) when rangeContainsPos range pos  ->
                let binding =
                    bindings
                    |> List.tryFind (fun x -> rangeContainsPos x.RangeOfBindingAndRhs pos)
                match binding with
                | Some(SynBinding.Binding(_, _, _, _, _, _, _, _, _, expr, _, _)) ->
                    getIdentRangeForFuncExprInApp traverseSynExpr expr pos
                | None ->
                    getIdentRangeForFuncExprInApp traverseSynExpr body pos

            | expr ->
                traverseSynExpr expr
                |> Option.map (fun expr -> expr)

        match scope.ParseTree with
        | Some input ->
            SyntaxTraversal.Traverse(pos, input, { new SyntaxVisitorBase<_>() with
                member _.VisitExpr(_, traverseSynExpr, defaultTraverse, expr) =
                    match expr with
                    | SynExpr.App (_, _, _funcExpr, _, range) as app when rangeContainsPos range pos ->
                        getIdentRangeForFuncExprInApp traverseSynExpr app pos
                    | _ -> defaultTraverse expr
            })
        | None -> None

    member scope.GetAllArgumentsForFunctionApplicationAtPostion pos =
        match input with
        | Some input -> SynExprAppLocationsImpl.getAllCurriedArgsAtPosition pos input
        | None -> None

    member scope.TryRangeOfParenEnclosingOpEqualsGreaterUsage opGreaterEqualPos =
        let (|Ident|_|) ofName =
            function | SynExpr.Ident ident when ident.idText = ofName -> Some ()
                     | _ -> None
        let (|InfixAppOfOpEqualsGreater|_|) =
          function | SynExpr.App(ExprAtomicFlag.NonAtomic, false, SynExpr.App(ExprAtomicFlag.NonAtomic, true, Ident "op_EqualsGreater", actualParamListExpr, _), actualLambdaBodyExpr, _) ->
                        Some (actualParamListExpr, actualLambdaBodyExpr)
                   | _ -> None

        match scope.ParseTree with
        | Some parseTree ->
            SyntaxTraversal.Traverse(opGreaterEqualPos, parseTree, { new SyntaxVisitorBase<_>() with
                member _.VisitExpr(_, _, defaultTraverse, expr) =
                    match expr with
                    | SynExpr.Paren((InfixAppOfOpEqualsGreater(lambdaArgs, lambdaBody) as app), _, _, _) ->
                        Some (app.Range, lambdaArgs.Range, lambdaBody.Range)
                    | _ -> defaultTraverse expr

                member _.VisitBinding(_path, defaultTraverse, binding) =
                    match binding with
                    | SynBinding.Binding (_, SynBindingKind.NormalBinding, _, _, _, _, _, _, _, (InfixAppOfOpEqualsGreater(lambdaArgs, lambdaBody) as app), _, _) ->
                        Some(app.Range, lambdaArgs.Range, lambdaBody.Range)
                    | _ -> defaultTraverse binding })
        | None -> None
    
    member scope.TryRangeOfExprInYieldOrReturn pos =
        match scope.ParseTree with
        | Some parseTree ->
            SyntaxTraversal.Traverse(pos, parseTree, { new SyntaxVisitorBase<_>() with 
                member _.VisitExpr(_path, _, defaultTraverse, expr) =
                    match expr with
                    | SynExpr.YieldOrReturn(_, expr, range)
                    | SynExpr.YieldOrReturnFrom(_, expr, range) when rangeContainsPos range pos ->
                        Some expr.Range
                    | _ -> defaultTraverse expr })
        | None -> None

    member scope.TryRangeOfRecordExpressionContainingPos pos =
        match input with
        | Some input ->
            SyntaxTraversal.Traverse(pos, input, { new SyntaxVisitorBase<_>() with 
                member _.VisitExpr(_, _, defaultTraverse, expr) =
                    match expr with
                    | SynExpr.Record(_, _, _, range) when rangeContainsPos range pos ->
                        Some range
                    | _ -> defaultTraverse expr })
        | None ->
            None

    member _.TryRangeOfRefCellDereferenceContainingPos expressionPos =
        match input with
        | Some input ->
            SyntaxTraversal.Traverse(expressionPos, input, { new SyntaxVisitorBase<_>() with 
                member _.VisitExpr(_, _, defaultTraverse, expr) =
                    match expr with
                    | SynExpr.App(_, false, SynExpr.Ident funcIdent, expr, _) ->
                        if funcIdent.idText = "op_Dereference" && rangeContainsPos expr.Range expressionPos then
                            Some funcIdent.idRange
                        else
                            None
                    | _ -> defaultTraverse expr })
        | None ->
            None

    member _.FindNoteworthyParamInfoLocations pos = 
        match input with
        | Some input -> FSharpNoteworthyParamInfoLocations.Find(pos, input)
        | _ -> None

    member _.IsPositionContainedInACurriedParameter pos =
        match input with
        | Some input ->
            let result =
                SyntaxTraversal.Traverse(pos, input, { new SyntaxVisitorBase<_>() with 
                    member _.VisitExpr(_path, traverseSynExpr, defaultTraverse, expr) =
                        defaultTraverse(expr)

                    override _.VisitBinding (_path, _, binding) =
                        match binding with
                        | Binding(_, _, _, _, _, _, valData, _, _, _, range, _) when rangeContainsPos range pos ->
                            let info = valData.SynValInfo.CurriedArgInfos
                            let mutable found = false
                            for group in info do
                                for arg in group do
                                    match arg.Ident with
                                    | Some ident when rangeContainsPos ident.idRange pos ->
                                        found <- true
                                    | _ -> ()
                            if found then Some range else None
                        | _ ->
                            None
                })
            result.IsSome
        | _ -> false
    
    /// Get declared items and the selected item at the specified location
    member _.GetNavigationItemsImpl() =
       ErrorScope.Protect range0 
            (fun () -> 
                match input with
                | Some (ParsedInput.ImplFile _ as p) ->
                    FSharpNavigation.getNavigation p
                | Some (ParsedInput.SigFile _) ->
                    FSharpNavigation.empty
                | _ -> 
                    FSharpNavigation.empty)
            (fun err -> 
                Trace.TraceInformation(sprintf "FCS: recovering from error in GetNavigationItemsImpl: '%s'" err)
                FSharpNavigation.empty)
            
    member _.ValidateBreakpointLocationImpl pos =
        let isMatchRange m = rangeContainsPos m pos || m.StartLine = pos.Line

        // Process let-binding
        let findBreakPoints () = 
            let checkRange m = [ if isMatchRange m then yield m ]
            let walkBindSeqPt sp = [ match sp with DebugPointAtBinding m -> yield! checkRange m | _ -> () ]
            let walkForSeqPt sp = [ match sp with DebugPointAtFor.Yes m -> yield! checkRange m | _ -> () ]
            let walkWhileSeqPt sp = [ match sp with DebugPointAtWhile.Yes m -> yield! checkRange m | _ -> () ]
            let walkTrySeqPt sp = [ match sp with DebugPointAtTry.Yes m -> yield! checkRange m | _ -> () ]
            let walkWithSeqPt sp = [ match sp with DebugPointAtWith.Yes m -> yield! checkRange m | _ -> () ]
            let walkFinallySeqPt sp = [ match sp with DebugPointAtFinally.Yes m -> yield! checkRange m | _ -> () ]

            let rec walkBind (Binding(_, _, _, _, _, _, SynValData(memFlagsOpt, _, _), synPat, _, synExpr, _, spInfo)) =
                [ // Don't yield the binding sequence point if there are any arguments, i.e. we're defining a function or a method
                  let isFunction = 
                      Option.isSome memFlagsOpt ||
                      match synPat with 
                      | SynPat.LongIdent (_, _, _, SynArgPats.Pats args, _, _) when not (List.isEmpty args) -> true
                      | _ -> false
                  if not isFunction then 
                      yield! walkBindSeqPt spInfo

                  yield! walkExpr (isFunction || (match spInfo with DebugPointAtBinding _ -> false | _-> true)) synExpr ]

            and walkExprs es = List.collect (walkExpr false) es
            and walkBinds es = List.collect walkBind es
            and walkMatchClauses cl = 
                [ for (Clause(_, whenExpr, e, _, _)) in cl do 
                    match whenExpr with 
                    | Some e -> yield! walkExpr false e 
                    | _ -> ()
                    yield! walkExpr true e ]

            and walkExprOpt (spAlways: bool) eOpt = [ match eOpt with Some e -> yield! walkExpr spAlways e | _ -> () ]
            
            and IsBreakableExpression e =
                match e with
                | SynExpr.Match _
                | SynExpr.IfThenElse _
                | SynExpr.For _
                | SynExpr.ForEach _
                | SynExpr.While _ -> true
                | _ -> not (IsControlFlowExpression e)

            // Determine the breakpoint locations for an expression. spAlways indicates we always
            // emit a breakpoint location for the expression unless it is a syntactic control flow construct
            and walkExpr (spAlways: bool)  e =
                let m = e.Range
                if not (isMatchRange m) then [] else
                [ if spAlways && IsBreakableExpression e then 
                      yield! checkRange m

                  match e with
                  | SynExpr.ArbitraryAfterError _ 
                  | SynExpr.LongIdent _
                  | SynExpr.LibraryOnlyILAssembly _
                  | SynExpr.LibraryOnlyStaticOptimization _
                  | SynExpr.Null _
                  | SynExpr.Ident _
                  | SynExpr.ImplicitZero _
                  | SynExpr.Const _ -> 
                     ()

                  | SynExpr.Quote (_, _, e, _, _)
                  | SynExpr.TypeTest (e, _, _)
                  | SynExpr.Upcast (e, _, _)
                  | SynExpr.AddressOf (_, e, _, _)
                  | SynExpr.CompExpr (_, _, e, _) 
                  | SynExpr.ArrayOrListOfSeqExpr (_, e, _)
                  | SynExpr.Typed (e, _, _)
                  | SynExpr.FromParseError (e, _) 
                  | SynExpr.DiscardAfterMissingQualificationAfterDot (e, _) 
                  | SynExpr.Do (e, _)
                  | SynExpr.Assert (e, _)
                  | SynExpr.Fixed (e, _)
                  | SynExpr.DotGet (e, _, _, _) 
                  | SynExpr.LongIdentSet (_, e, _)
                  | SynExpr.New (_, _, e, _) 
                  | SynExpr.TypeApp (e, _, _, _, _, _, _) 
                  | SynExpr.LibraryOnlyUnionCaseFieldGet (e, _, _, _) 
                  | SynExpr.Downcast (e, _, _)
                  | SynExpr.InferredUpcast (e, _)
                  | SynExpr.InferredDowncast (e, _)
                  | SynExpr.Lazy (e, _)
                  | SynExpr.TraitCall (_, _, e, _)
                  | SynExpr.Paren (e, _, _, _) -> 
                      yield! walkExpr false e

                  | SynExpr.InterpolatedString (parts, _) -> 
                      yield! walkExprs [ for part in parts do 
                                            match part with 
                                            | SynInterpolatedStringPart.String _ -> ()
                                            | SynInterpolatedStringPart.FillExpr (fillExpr, _) -> yield fillExpr ]

                  | SynExpr.YieldOrReturn (_, e, _)
                  | SynExpr.YieldOrReturnFrom (_, e, _)
                  | SynExpr.DoBang  (e, _) ->
                      yield! checkRange e.Range
                      yield! walkExpr false e

                  | SynExpr.NamedIndexedPropertySet (_, e1, e2, _)
                  | SynExpr.DotSet (e1, _, e2, _)
                  | SynExpr.Set (e1, e2, _)
                  | SynExpr.LibraryOnlyUnionCaseFieldSet (e1, _, _, e2, _)
                  | SynExpr.App (_, _, e1, e2, _) -> 
                      yield! walkExpr false e1 
                      yield! walkExpr false e2

                  | SynExpr.ArrayOrList (_, es, _)
                  | SynExpr.Tuple (_, es, _, _) -> 
                      yield! walkExprs es

                  | SynExpr.Record (_, copyExprOpt, fs, _) ->
                      match copyExprOpt with
                      | Some (e, _) -> yield! walkExpr true e
                      | None -> ()
                      yield! walkExprs (fs |> List.choose p23)

                  | SynExpr.AnonRecd (_isStruct, copyExprOpt, fs, _) ->
                      match copyExprOpt with
                      | Some (e, _) -> yield! walkExpr true e
                      | None -> ()
                      yield! walkExprs (fs |> List.map snd)

                  | SynExpr.ObjExpr (_, args, bs, is, _, _) -> 
                      match args with
                      | None -> ()
                      | Some (arg, _) -> yield! walkExpr false arg
                      yield! walkBinds bs  
                      for (InterfaceImpl(_, bs, _)) in is do yield! walkBinds bs

                  | SynExpr.While (spWhile, e1, e2, _) -> 
                      yield! walkWhileSeqPt spWhile
                      yield! walkExpr false e1 
                      yield! walkExpr true e2

                  | SynExpr.JoinIn (e1, _range, e2, _range2) -> 
                      yield! walkExpr false e1 
                      yield! walkExpr false e2

                  | SynExpr.For (spFor, _, e1, _, e2, e3, _) -> 
                      yield! walkForSeqPt spFor
                      yield! walkExpr false e1 
                      yield! walkExpr true e2 
                      yield! walkExpr true e3

                  | SynExpr.ForEach (spFor, _, _, _, e1, e2, _) ->
                      yield! walkForSeqPt spFor
                      yield! walkExpr false e1 
                      yield! walkExpr true e2 

                  | SynExpr.MatchLambda (_isExnMatch, _argm, cl, spBind, _wholem) -> 
                      yield! walkBindSeqPt spBind
                      for (Clause(_, whenExpr, e, _, _)) in cl do 
                          yield! walkExprOpt false whenExpr
                          yield! walkExpr true e 

                  | SynExpr.Lambda (_, _, _, e, _, _) -> 
                      yield! walkExpr true e 

                  | SynExpr.Match (spBind, e, cl, _) ->
                      yield! walkBindSeqPt spBind
                      yield! walkExpr false e 
                      for (Clause(_, whenExpr, e, _, _)) in cl do 
                          yield! walkExprOpt false whenExpr
                          yield! walkExpr true e 

                  | SynExpr.LetOrUse (_, _, bs, e, _) -> 
                      yield! walkBinds bs  
                      yield! walkExpr true e

                  | SynExpr.TryWith (e, _, cl, _, _, spTry, spWith) -> 
                      yield! walkTrySeqPt spTry
                      yield! walkWithSeqPt spWith
                      yield! walkExpr true e 
                      yield! walkMatchClauses cl
                  
                  | SynExpr.TryFinally (e1, e2, _, spTry, spFinally) ->
                      yield! walkExpr true e1
                      yield! walkExpr true e2
                      yield! walkTrySeqPt spTry
                      yield! walkFinallySeqPt spFinally

                  | SynExpr.SequentialOrImplicitYield (spSeq, e1, e2, _, _)
                  | SynExpr.Sequential (spSeq, _, e1, e2, _) -> 
                      yield! walkExpr (match spSeq with DebugPointAtSequential.ExprOnly -> false | _ -> true) e1
                      yield! walkExpr (match spSeq with DebugPointAtSequential.StmtOnly -> false | _ -> true) e2

                  | SynExpr.IfThenElse (e1, e2, e3opt, spBind, _, _, _) ->
                      yield! walkBindSeqPt spBind
                      yield! walkExpr false e1
                      yield! walkExpr true e2
                      yield! walkExprOpt true e3opt

                  | SynExpr.DotIndexedGet (e1, es, _, _) -> 
                      yield! walkExpr false e1 
                      yield! walkExprs [ for e in es do yield! e.Exprs ]

                  | SynExpr.DotIndexedSet (e1, es, e2, _, _, _) ->
                      yield! walkExpr false e1 
                      yield! walkExprs [ for e in es do yield! e.Exprs ]
                      yield! walkExpr false e2 

                  | SynExpr.DotNamedIndexedPropertySet (e1, _, e2, e3, _) ->
                      yield! walkExpr false e1 
                      yield! walkExpr false e2 
                      yield! walkExpr false e3 

                  | SynExpr.LetOrUseBang (spBind, _, _, _, e1, es, e2, _) -> 
                      yield! walkBindSeqPt spBind
                      yield! walkExpr true e1
                      for (andBangSpBind,_,_,_,eAndBang,_) in es do
                          yield! walkBindSeqPt andBangSpBind
                          yield! walkExpr true eAndBang
                      yield! walkExpr true e2

                  | SynExpr.MatchBang (spBind, e, cl, _) ->
                      yield! walkBindSeqPt spBind
                      yield! walkExpr false e 
                      for (Clause(_, whenExpr, e, _, _)) in cl do 
                          yield! walkExprOpt false whenExpr
                          yield! walkExpr true e ]
            
            // Process a class declaration or F# type declaration
            let rec walkTycon (TypeDefn(ComponentInfo(_, _, _, _, _, _, _, _), repr, membDefns, implicitCtor, m)) =
                if not (isMatchRange m) then [] else
                [ for memb in membDefns do yield! walkMember memb
                  match repr with
                  | SynTypeDefnRepr.ObjectModel(_, membDefns, _) -> 
                      for memb in membDefns do yield! walkMember memb
                  | _ -> () 
                  for memb in membDefns do yield! walkMember memb
                  for memb in Option.toList implicitCtor do yield! walkMember memb]
                      
            // Returns class-members for the right dropdown                  
            and walkMember memb =
                if not (rangeContainsPos memb.Range pos) then [] else
                [ match memb with
                  | SynMemberDefn.LetBindings(binds, _, _, _) -> yield! walkBinds binds
                  | SynMemberDefn.AutoProperty(_attribs, _isStatic, _id, _tyOpt, _propKind, _, _xmlDoc, _access, synExpr, _, _) -> yield! walkExpr true synExpr
                  | SynMemberDefn.ImplicitCtor(_, _, _, _, _, m) -> yield! checkRange m
                  | SynMemberDefn.Member(bind, _) -> yield! walkBind bind
                  | SynMemberDefn.Interface(_, Some membs, _) -> for m in membs do yield! walkMember m
                  | SynMemberDefn.Inherit(_, _, m) -> 
                      // can break on the "inherit" clause
                      yield! checkRange m
                  | SynMemberDefn.ImplicitInherit(_, arg, _, m) -> 
                      // can break on the "inherit" clause
                      yield! checkRange m
                      yield! walkExpr true arg
                  | _ -> ()  ]

            // Process declarations nested in a module that should be displayed in the left dropdown
            // (such as type declarations, nested modules etc.)                            
            let rec walkDecl decl = 
                [ match decl with 
                  | SynModuleDecl.Let(_, binds, m) when isMatchRange m -> 
                      yield! walkBinds binds
                  | SynModuleDecl.DoExpr(spExpr, expr, m) when isMatchRange m ->  
                      yield! walkBindSeqPt spExpr
                      yield! walkExpr false expr
                  | SynModuleDecl.ModuleAbbrev _ -> ()
                  | SynModuleDecl.NestedModule(_, _isRec, decls, _, m) when isMatchRange m ->
                      for d in decls do yield! walkDecl d
                  | SynModuleDecl.Types(tydefs, m) when isMatchRange m -> 
                      for d in tydefs do yield! walkTycon d
                  | SynModuleDecl.Exception(SynExceptionDefn(SynExceptionDefnRepr(_, _, _, _, _, _), membDefns, _), m) 
                        when isMatchRange m ->
                      for m in membDefns do yield! walkMember m
                  | _ -> () ] 
                      
            // Collect all the items in a module  
            let walkModule (SynModuleOrNamespace(_, _, _, decls, _, _, _, m)) =
                if isMatchRange m then
                    List.collect walkDecl decls
                else
                    []
                      
           /// Get information for implementation file        
            let walkImplFile (modules: SynModuleOrNamespace list) = List.collect walkModule modules
                     
            match input with
            | Some (ParsedInput.ImplFile (ParsedImplFileInput (modules = modules))) -> walkImplFile modules 
            | _ -> []
 
        ErrorScope.Protect range0 
            (fun () -> 
                let locations = findBreakPoints()
                
                if pos.Column = 0 then
                    // we have a breakpoint that was set with mouse at line start
                    match locations |> List.filter (fun m -> m.StartLine = m.EndLine && pos.Line = m.StartLine) with
                    | [] ->
                        match locations |> List.filter (fun m -> rangeContainsPos m pos) with
                        | [] ->
                            match locations |> List.filter (fun m -> rangeBeforePos m pos |> not) with
                            | [] -> Seq.tryHead locations
                            | locationsAfterPos -> Seq.tryHead locationsAfterPos
                        | coveringLocations -> Seq.tryLast coveringLocations
                    | locationsOnSameLine -> Seq.tryHead locationsOnSameLine
                else
                    match locations |> List.filter (fun m -> rangeContainsPos m pos) with
                    | [] ->
                        match locations |> List.filter (fun m -> rangeBeforePos m pos |> not) with
                        | [] -> Seq.tryHead locations
                        | locationsAfterPos -> Seq.tryHead locationsAfterPos
                    | coveringLocations -> Seq.tryLast coveringLocations)
            (fun msg -> 
                Trace.TraceInformation(sprintf "FCS: recovering from error in ValidateBreakpointLocationImpl: '%s'" msg)
                None)
            
    /// When these files appear or disappear the configuration for the current project is invalidated.
    member _.DependencyFiles = dependencyFiles

    member _.FileName =
      match input with
      | Some (ParsedInput.ImplFile (ParsedImplFileInput (fileName = modname))) 
      | Some (ParsedInput.SigFile (ParsedSigFileInput (fileName = modname))) -> modname
      | _ -> ""
    
    // Get items for the navigation drop down bar       
    member scope.GetNavigationItems() =
        // This does not need to be run on the background thread
        scope.GetNavigationItemsImpl()

    member scope.ValidateBreakpointLocation pos =
        // This does not need to be run on the background thread
        scope.ValidateBreakpointLocationImpl pos

