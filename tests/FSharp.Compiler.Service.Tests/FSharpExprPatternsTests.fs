module FSharp.Compiler.Service.Tests.FSharpExprPatternsTests

open FSharp.Test

#nowarn "57"

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Text
open FSharp.Compiler.Symbols
open Xunit

module TASTCollecting =

    open FSharp.Compiler.Symbols.FSharpExprPatterns

    type Handler = NewUnionCaseHandler of (string -> unit)

    let rec visitExpr (handler: Handler) (e: FSharpExpr) =
        match e with
        | AddressOf lvalueExpr -> visitExpr handler lvalueExpr
        | AddressSet (lvalueExpr, rvalueExpr) ->
            visitExpr handler lvalueExpr
            visitExpr handler rvalueExpr
        | Application (funcExpr, _typeArgs, argExprs) ->
            visitExpr handler funcExpr
            visitExprs handler argExprs
        | Call (objExprOpt, memberOrFunc, _typeArgs1, _typeArgs2, argExprs) ->
            visitObjArg handler objExprOpt
            visitExprs handler argExprs
        | Coerce (_targetType, inpExpr) -> visitExpr handler inpExpr
        | FastIntegerForLoop (startExpr, limitExpr, consumeExpr, _isUp, _debugPointAtFor, _debugPointAtInOrTo) ->
            visitExpr handler startExpr
            visitExpr handler limitExpr
            visitExpr handler consumeExpr
        | ILAsm (_asmCode, _typeArgs, argExprs) -> visitExprs handler argExprs
        | ILFieldGet (objExprOpt, _fieldType, _fieldName) -> visitObjArg handler objExprOpt
        | ILFieldSet (objExprOpt, _fieldType, _fieldName, _valueExpr) -> visitObjArg handler objExprOpt
        | IfThenElse (guardExpr, thenExpr, elseExpr) ->
            visitExpr handler guardExpr
            visitExpr handler thenExpr
            visitExpr handler elseExpr
        | Lambda (_lambdaVar, bodyExpr) -> visitExpr handler bodyExpr
        | Let ((_bindingVar, bindingExpr, _debugPointAtBinding), bodyExpr) ->
            visitExpr handler bindingExpr
            visitExpr handler bodyExpr
        | LetRec (recursiveBindings, bodyExpr) ->
            let recursiveBindings' =
                recursiveBindings |> List.map (fun (mfv, expr, _dp) -> (mfv, expr))

            List.iter (snd >> visitExpr handler) recursiveBindings'
            visitExpr handler bodyExpr
        | NewArray (_arrayType, argExprs) -> visitExprs handler argExprs
        | NewDelegate (_delegateType, delegateBodyExpr) -> visitExpr handler delegateBodyExpr
        | NewObject (_objType, _typeArgs, argExprs) -> visitExprs handler argExprs
        | NewRecord (_recordType, argExprs) -> visitExprs handler argExprs
        | NewTuple (_tupleType, argExprs) -> visitExprs handler argExprs
        | NewUnionCase (_unionType, unionCase, argExprs) ->
            match handler with
            | NewUnionCaseHandler h -> h unionCase.Name

            visitExprs handler argExprs
        | Quote quotedExpr -> visitExpr handler quotedExpr
        | FSharpFieldGet (objExprOpt, _recordOrClassType, _fieldInfo) -> visitObjArg handler objExprOpt
        | FSharpFieldSet (objExprOpt, _recordOrClassType, _fieldInfo, argExpr) ->
            visitObjArg handler objExprOpt
            visitExpr handler argExpr
        | Sequential (firstExpr, secondExpr) ->
            visitExpr handler firstExpr
            visitExpr handler secondExpr
        | TryFinally (bodyExpr, finalizeExpr, _debugPointAtTry, _debugPointAtFinally) ->
            visitExpr handler bodyExpr
            visitExpr handler finalizeExpr
        | TryWith (bodyExpr, _, _, _catchVar, catchExpr, _debugPointAtTry, _debugPointAtWith) ->
            visitExpr handler bodyExpr
            visitExpr handler catchExpr
        | TupleGet (_tupleType, _tupleElemIndex, tupleExpr) -> visitExpr handler tupleExpr
        | DecisionTree (decisionExpr, decisionTargets) ->
            visitExpr handler decisionExpr
            List.iter (snd >> visitExpr handler) decisionTargets
        | DecisionTreeSuccess (_decisionTargetIdx, decisionTargetExprs) -> visitExprs handler decisionTargetExprs
        | TypeLambda (_genericParam, bodyExpr) -> visitExpr handler bodyExpr
        | TypeTest (_ty, inpExpr) -> visitExpr handler inpExpr
        | UnionCaseSet (unionExpr, _unionType, _unionCase, _unionCaseField, valueExpr) ->
            visitExpr handler unionExpr
            visitExpr handler valueExpr
        | UnionCaseGet (unionExpr, _unionType, _unionCase, _unionCaseField) -> visitExpr handler unionExpr
        | UnionCaseTest (unionExpr, _unionType, _unionCase) -> visitExpr handler unionExpr
        | UnionCaseTag (unionExpr, _unionType) -> visitExpr handler unionExpr
        | ObjectExpr (_objType, baseCallExpr, overrides, interfaceImplementations) ->
            visitExpr handler baseCallExpr
            List.iter (visitObjMember handler) overrides
            List.iter (snd >> List.iter (visitObjMember handler)) interfaceImplementations
        | TraitCall (_sourceTypes, _traitName, _typeArgs, _typeInstantiation, _argTypes, argExprs) -> visitExprs handler argExprs
        | ValueSet (_valToSet, valueExpr) -> visitExpr handler valueExpr
        | WhileLoop (guardExpr, bodyExpr, _debugPointAtWhile) ->
            visitExpr handler guardExpr
            visitExpr handler bodyExpr
        | BaseValue _baseType -> ()
        | DefaultValue _defaultType -> ()
        | ThisValue _thisType -> ()
        | Const (_constValueObj, _constType) -> ()
        | Value _valueToGet -> ()
        | CallWithWitnesses (expr, _mfv, _typeLst, _typeLst2, exprs1, exprs2) ->
            expr |> Option.iter (visitExpr handler)
            exprs1 |> List.iter (visitExpr handler)
            exprs2 |> List.iter (visitExpr handler)
        | NewAnonRecord (_, exprLst) -> exprLst |> List.iter (visitExpr handler)
        | AnonRecordGet (expr, _, _) -> visitExpr handler expr
        | DebugPoint (_d, expr) -> visitExpr handler expr
        | WitnessArg _ -> ()
        | _ -> ()

    and visitExprs f exprs = List.iter (visitExpr f) exprs

    and visitObjArg f objOpt = Option.iter (visitExpr f) objOpt

    and visitObjMember f memb = visitExpr f memb.Body

    let rec visitDeclaration f d =
        match d with
        | FSharpImplementationFileDeclaration.Entity (_e, subDecls) ->
            for subDecl in subDecls do
                visitDeclaration f subDecl
        | FSharpImplementationFileDeclaration.MemberOrFunctionOrValue (_v, _vs, e) -> visitExpr f e
        | FSharpImplementationFileDeclaration.InitAction e -> visitExpr f e

let testPatterns handler source =
    let files = Map.ofArray [| "A.fs", SourceText.ofString source |]

    let documentSource fileName =
        Map.tryFind fileName files |> async.Return

    let projectOptions =
        let _, projectOptions = mkTestFileAndOptions "" Array.empty

        { projectOptions with
            SourceFiles = [| "A.fs" |]
        }

    let checker =
        FSharpChecker.Create(documentSource = DocumentSource.Custom documentSource, keepAssemblyContents = true, useTransparentCompiler = CompilerAssertHelpers.UseTransparentCompiler)

    let checkResult =
        checker.ParseAndCheckFileInProject("A.fs", 0, Map.find "A.fs" files, projectOptions)
        |> Async.RunImmediate

    match checkResult with
    | _, FSharpCheckFileAnswer.Succeeded (checkResults) ->

        match checkResults.ImplementationFile with
        | Some implFile ->
            for decl in implFile.Declarations do
                TASTCollecting.visitDeclaration handler decl
        | _ -> ()
    | _, _ -> ()

[<Fact>]
let ``union case with type`` () =
    let implSource =
        """
module M

type T = Case1 of string

let x = Case1 "bla" 
"""

    let lst = ResizeArray<string>()

    let handler: TASTCollecting.Handler =
        TASTCollecting.Handler.NewUnionCaseHandler lst.Add

    testPatterns handler implSource // check this doesn't throw
    Assert.Contains("Case1", lst)
