module rec ParallelTypeCheckingTests.Code.TrieApproach.FileContentMapping

open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTreeOps

[<RequireQualifiedAccess>]
module Continuation =
    let rec sequence<'a, 'ret> (recursions: (('a -> 'ret) -> 'ret) list) (finalContinuation: 'a list -> 'ret) : 'ret =
        match recursions with
        | [] -> [] |> finalContinuation
        | recurse :: recurses -> recurse (fun ret -> sequence recurses (fun rets -> ret :: rets |> finalContinuation))

type Continuations = ((FileContentEntry list -> FileContentEntry list) -> FileContentEntry list) list

/// Option.toList >> (List.collect f)
let cfo f a = lc f (Option.toList a)
/// List.collect
let lc = List.collect

let longIdentToPath (skipLast: bool) (longId: LongIdent) : ModuleSegment list =
    if skipLast then
        List.take (longId.Length - 1) longId
    else
        longId
    |> List.map (fun ident -> ident.idText)

let synLongIdentToPath (skipLast: bool) (synLongIdent: SynLongIdent) =
    longIdentToPath skipLast synLongIdent.LongIdent

let visitSynLongIdent (lid: SynLongIdent) : FileContentEntry list = visitLongIdent lid.LongIdent

let visitLongIdent (lid: LongIdent) =
    match lid with
    | []
    | [ _ ] -> []
    | lid -> [ FileContentEntry.PrefixedIdentifier(longIdentToPath true lid) ]

let visitSynAttribute (a: SynAttribute) : FileContentEntry list =
    [ yield! visitSynLongIdent a.TypeName; yield! visitSynExpr a.ArgExpr ]

let visitSynAttributeList (attributes: SynAttributeList) : FileContentEntry list =
    lc visitSynAttribute attributes.Attributes

let visitSynAttributes (attributes: SynAttributes) : FileContentEntry list = lc visitSynAttributeList attributes

let visitSynModuleDecl (decl: SynModuleDecl) : FileContentEntry list =
    match decl with
    | SynModuleDecl.Open(target = SynOpenDeclTarget.ModuleOrNamespace (longId, _)) ->
        [ FileContentEntry.OpenStatement(synLongIdentToPath false longId) ]
    | SynModuleDecl.Open(target = SynOpenDeclTarget.Type (typeName, _)) -> visitSynType typeName
    | SynModuleDecl.Attributes (attributes, _) -> lc visitSynAttributeList attributes
    | SynModuleDecl.Expr (expr, _) -> visitSynExpr expr
    | SynModuleDecl.NestedModule (moduleInfo = SynComponentInfo (longId = [ ident ]; attributes = attributes); decls = decls) ->
        [
            yield! visitSynAttributes attributes
            yield FileContentEntry.NestedModule(ident.idText, lc visitSynModuleDecl decls)
        ]
    | SynModuleDecl.NestedModule _ -> failwith "A nested module cannot have multiple identifiers"
    | SynModuleDecl.Let (bindings = bindings) -> lc visitBinding bindings
    | SynModuleDecl.Types (typeDefns = typeDefns) -> lc visitSynTypeDefn typeDefns
    | SynModuleDecl.HashDirective _ -> []
    | SynModuleDecl.ModuleAbbrev (longId = longId) ->
        // I believe this is enough
        // A module abbreviation doesn't appear to be exposed as part of the current module/namespace
        visitLongIdent longId
    | SynModuleDecl.NamespaceFragment _ -> []
    | SynModuleDecl.Exception(exnDefn = SynExceptionDefn (exnRepr = SynExceptionDefnRepr (attributes = attributes
                                                                                          caseName = caseName
                                                                                          longId = longId)
                                                          members = members)) ->
        [
            yield! visitSynAttributes attributes
            yield! visitSynUnionCase caseName
            yield! cfo visitLongIdent longId
            yield! lc visitSynMemberDefn members
        ]

let visitSynModuleSigDecl (md: SynModuleSigDecl) =
    match md with
    | SynModuleSigDecl.Open(target = SynOpenDeclTarget.ModuleOrNamespace (longId, _)) ->
        [ FileContentEntry.OpenStatement(synLongIdentToPath false longId) ]
    | SynModuleSigDecl.Open(target = SynOpenDeclTarget.Type (typeName, _)) -> visitSynType typeName
    | SynModuleSigDecl.NestedModule (moduleInfo = SynComponentInfo (longId = [ ident ]; attributes = attributes); moduleDecls = decls) ->
        [
            yield! visitSynAttributes attributes
            yield FileContentEntry.NestedModule(ident.idText, lc visitSynModuleSigDecl decls)
        ]
    | SynModuleSigDecl.NestedModule _ -> failwith "A nested module cannot have multiple identifiers"
    | SynModuleSigDecl.ModuleAbbrev _ -> failwith "no support for module abbreviations"
    | SynModuleSigDecl.Val (valSig, _) -> visitSynValSig valSig
    | SynModuleSigDecl.Types (types = types) -> lc visitSynTypeDefnSig types
    | SynModuleSigDecl.Exception(exnSig = SynExceptionSig (exnRepr = SynExceptionDefnRepr (attributes = attributes
                                                                                           caseName = caseName
                                                                                           longId = longId)
                                                           members = members)) ->
        [
            yield! visitSynAttributes attributes
            yield! visitSynUnionCase caseName
            yield! cfo visitLongIdent longId
            yield! lc visitSynMemberSig members
        ]
    | SynModuleSigDecl.HashDirective _ -> []
    | SynModuleSigDecl.NamespaceFragment _ -> []

let visitSynUnionCase (SynUnionCase (attributes = attributes; caseType = caseType)) =
    let caseEntries =
        match caseType with
        | SynUnionCaseKind.Fields cases -> lc visitSynField cases
        | SynUnionCaseKind.FullType (fullType = fullType) -> visitSynType fullType

    [ yield! visitSynAttributes attributes; yield! caseEntries ]

let visitSynEnumCase (SynEnumCase (attributes = attributes)) = visitSynAttributes attributes

let visitSynTypeDefn
    (SynTypeDefn (typeInfo = SynComponentInfo (attributes = attributes; typeParams = typeParams; constraints = constraints)
                  typeRepr = typeRepr
                  members = members))
    : FileContentEntry list =
    let reprEntries =
        match typeRepr with
        | SynTypeDefnRepr.Simple (simpleRepr, _) ->
            match simpleRepr with
            | SynTypeDefnSimpleRepr.Union (unionCases = unionCases) -> lc visitSynUnionCase unionCases
            | SynTypeDefnSimpleRepr.Enum (cases = cases) -> lc visitSynEnumCase cases
            | SynTypeDefnSimpleRepr.Record (recordFields = recordFields) -> lc visitSynField recordFields
            // This is only used in the typed tree
            // The parser doesn't construct this
            | SynTypeDefnSimpleRepr.General _ -> []
            | SynTypeDefnSimpleRepr.LibraryOnlyILAssembly _ -> []
            | SynTypeDefnSimpleRepr.TypeAbbrev (rhsType = rhsType) -> visitSynType rhsType
            | SynTypeDefnSimpleRepr.None _ -> []
            // This is only used in the typed tree
            // The parser doesn't construct this
            | SynTypeDefnSimpleRepr.Exception _ -> []
        | SynTypeDefnRepr.ObjectModel (kind, members, _) ->
            match kind with
            | SynTypeDefnKind.Delegate (signature, _) -> [ yield! visitSynType signature; yield! lc visitSynMemberDefn members ]
            | _ -> lc visitSynMemberDefn members
        | SynTypeDefnRepr.Exception _ ->
            // This is only used in the typed tree
            // The parser doesn't construct this
            []

    [
        yield! visitSynAttributes attributes
        yield! cfo visitSynTyparDecls typeParams
        yield! lc visitSynTypeConstraint constraints
        yield! reprEntries
        yield! lc visitSynMemberDefn members
    ]

let visitSynTypeDefnSig
    (SynTypeDefnSig (typeInfo = SynComponentInfo (attributes = attributes; typeParams = typeParams; constraints = constraints)
                     typeRepr = typeRepr
                     members = members))
    =
    let reprEntries =
        match typeRepr with
        | SynTypeDefnSigRepr.Simple (simpleRepr, _) ->
            match simpleRepr with
            | SynTypeDefnSimpleRepr.Union (unionCases = unionCases) -> lc visitSynUnionCase unionCases
            | SynTypeDefnSimpleRepr.Enum (cases = cases) -> lc visitSynEnumCase cases
            | SynTypeDefnSimpleRepr.Record (recordFields = recordFields) -> lc visitSynField recordFields
            // This is only used in the typed tree
            // The parser doesn't construct this
            | SynTypeDefnSimpleRepr.General _ -> []
            | SynTypeDefnSimpleRepr.LibraryOnlyILAssembly _ -> []
            | SynTypeDefnSimpleRepr.TypeAbbrev (rhsType = rhsType) -> visitSynType rhsType
            | SynTypeDefnSimpleRepr.None _ -> []
            // This is only used in the typed tree
            // The parser doesn't construct this
            | SynTypeDefnSimpleRepr.Exception _ -> []
        | SynTypeDefnSigRepr.ObjectModel (kind, members, _) ->
            match kind with
            | SynTypeDefnKind.Delegate (signature, _) -> [ yield! visitSynType signature; yield! lc visitSynMemberSig members ]
            | _ -> lc visitSynMemberSig members
        | SynTypeDefnSigRepr.Exception _ ->
            // This is only used in the typed tree
            // The parser doesn't construct this
            []

    [
        yield! visitSynAttributes attributes
        yield! cfo visitSynTyparDecls typeParams
        yield! lc visitSynTypeConstraint constraints
        yield! reprEntries
        yield! lc visitSynMemberSig members
    ]

let visitSynValSig (SynValSig (attributes = attributes; synType = synType; synExpr = synExpr)) =
    [
        yield! visitSynAttributes attributes
        yield! visitSynType synType
        yield! cfo visitSynExpr synExpr
    ]

let visitSynField (SynField (attributes = attributes; fieldType = fieldType)) =
    [ yield! visitSynAttributes attributes; yield! visitSynType fieldType ]

let visitSynMemberDefn (md: SynMemberDefn) : FileContentEntry list =
    match md with
    | SynMemberDefn.Member (memberDefn = binding) -> visitBinding binding
    | SynMemberDefn.Open _ -> []
    | SynMemberDefn.GetSetMember (memberDefnForGet, memberDefnForSet, _, _) ->
        [
            yield! cfo visitBinding memberDefnForGet
            yield! cfo visitBinding memberDefnForSet
        ]
    | SynMemberDefn.ImplicitCtor (ctorArgs = ctorArgs) -> visitSynSimplePats ctorArgs
    | SynMemberDefn.ImplicitInherit (inheritType, inheritArgs, _, _) -> [ yield! visitSynType inheritType; yield! visitSynExpr inheritArgs ]
    | SynMemberDefn.LetBindings (bindings = bindings) -> lc visitBinding bindings
    | SynMemberDefn.AbstractSlot (slotSig = slotSig) -> visitSynValSig slotSig
    | SynMemberDefn.Interface (interfaceType, _, members, _) ->
        [
            yield! visitSynType interfaceType
            yield! cfo (lc visitSynMemberDefn) members
        ]
    | SynMemberDefn.Inherit (baseType, _, _) -> visitSynType baseType
    | SynMemberDefn.ValField (fieldInfo, _) -> visitSynField fieldInfo
    | SynMemberDefn.NestedType _ -> []
    | SynMemberDefn.AutoProperty (attributes = attributes; typeOpt = typeOpt; synExpr = synExpr) ->
        [
            yield! visitSynAttributes attributes
            yield! cfo visitSynType typeOpt
            yield! visitSynExpr synExpr
        ]

let visitSynInterfaceImpl (SynInterfaceImpl (interfaceTy = t; bindings = bindings; members = members)) =
    [
        yield! visitSynType t
        yield! lc visitBinding bindings
        yield! lc visitSynMemberDefn members
    ]

let visitSynType (t: SynType) : FileContentEntry list =
    let rec visit (t: SynType) (continuation: FileContentEntry list -> FileContentEntry list) =
        match t with
        | SynType.LongIdent lid -> continuation (visitSynLongIdent lid)
        | SynType.App (typeName = typeName; typeArgs = typeArgs) ->
            let continuations = List.map visit (typeName :: typeArgs)
            let finalContinuation = lc id >> continuation
            Continuation.sequence continuations finalContinuation
        | SynType.LongIdentApp (typeName = typeName; longDotId = longDotId; typeArgs = typeArgs) ->
            let continuations = List.map visit (typeName :: typeArgs)

            let finalContinuation nodes =
                visitSynLongIdent longDotId @ lc id nodes |> continuation

            Continuation.sequence continuations finalContinuation
        | SynType.Tuple (path = path) ->
            let continuations = List.map visit (getTypeFromTuplePath path)
            let finalContinuation = lc id >> continuation
            Continuation.sequence continuations finalContinuation
        | SynType.AnonRecd (fields = fields) ->
            let continuations = List.map (snd >> visit) fields
            let finalContinuation = lc id >> continuation
            Continuation.sequence continuations finalContinuation
        | SynType.Array (elementType = elementType) -> visit elementType continuation
        | SynType.Fun (argType, returnType, _, _) ->
            let continuations = List.map visit [ argType; returnType ]
            let finalContinuation = lc id >> continuation
            Continuation.sequence continuations finalContinuation
        | SynType.Var _ -> continuation []
        | SynType.Anon _ -> continuation []
        | SynType.WithGlobalConstraints (typeName, constraints, _) ->
            visit typeName (fun nodes -> nodes @ lc visitSynTypeConstraint constraints |> continuation)
        | SynType.HashConstraint (innerType, _) -> visit innerType continuation
        | SynType.MeasureDivide (dividend, divisor, _) ->
            let continuations = List.map visit [ dividend; divisor ]
            let finalContinuation = lc id >> continuation
            Continuation.sequence continuations finalContinuation
        | SynType.MeasurePower (baseMeasure = baseMeasure) -> visit baseMeasure continuation
        | SynType.StaticConstant _ -> continuation []
        | SynType.StaticConstantExpr (expr, _) -> continuation (visitSynExpr expr)
        | SynType.StaticConstantNamed (ident, value, _) ->
            let continuations = List.map visit [ ident; value ]
            let finalContinuation = lc id >> continuation
            Continuation.sequence continuations finalContinuation
        | SynType.Paren (innerType, _) -> visit innerType continuation
        | SynType.SignatureParameter (attributes = attributes; usedType = usedType) ->
            visit usedType (fun nodes -> [ yield! visitSynAttributes attributes; yield! nodes ] |> continuation)
        | SynType.Or (lhsType, rhsType, _, _) ->
            let continuations = List.map visit [ lhsType; rhsType ]
            let finalContinuation = lc id >> continuation
            Continuation.sequence continuations finalContinuation

    visit t id

let visitSynValTyparDecls (SynValTyparDecls (typars = typars)) = cfo visitSynTyparDecls typars

let visitSynTyparDecls (td: SynTyparDecls) : FileContentEntry list =
    match td with
    | SynTyparDecls.PostfixList (decls, constraints, _) ->
        [
            yield! lc visitSynTyparDecl decls
            yield! lc visitSynTypeConstraint constraints
        ]
    | SynTyparDecls.PrefixList (decls = decls) -> lc visitSynTyparDecl decls
    | SynTyparDecls.SinglePrefix (decl = decl) -> visitSynTyparDecl decl

let visitSynTyparDecl (SynTyparDecl (attributes = attributes)) = visitSynAttributes attributes

let visitSynTypeConstraint (tc: SynTypeConstraint) : FileContentEntry list =
    match tc with
    | SynTypeConstraint.WhereSelfConstrained _ -> []
    | SynTypeConstraint.WhereTyparIsValueType _ -> []
    | SynTypeConstraint.WhereTyparIsReferenceType _ -> []
    | SynTypeConstraint.WhereTyparIsUnmanaged _ -> []
    | SynTypeConstraint.WhereTyparSupportsNull _ -> []
    | SynTypeConstraint.WhereTyparIsComparable _ -> []
    | SynTypeConstraint.WhereTyparIsEquatable _ -> []
    | SynTypeConstraint.WhereTyparDefaultsToType (typeName = typeName) -> visitSynType typeName
    | SynTypeConstraint.WhereTyparSubtypeOfType (typeName = typeName) -> visitSynType typeName
    | SynTypeConstraint.WhereTyparSupportsMember (typars, memberSig, _) ->
        [ yield! visitSynType typars; yield! visitSynMemberSig memberSig ]
    | SynTypeConstraint.WhereTyparIsEnum (typeArgs = typeArgs) -> lc visitSynType typeArgs
    | SynTypeConstraint.WhereTyparIsDelegate (typeArgs = typeArgs) -> lc visitSynType typeArgs

let visitSynExpr (e: SynExpr) : FileContentEntry list =
    let rec visit (e: SynExpr) (continuation: FileContentEntry list -> FileContentEntry list) : FileContentEntry list =
        match e with
        | SynExpr.Const _ -> continuation []
        | SynExpr.Paren (expr = expr) -> visit expr continuation
        | SynExpr.Quote (operator = operator; quotedExpr = quotedExpr) ->
            visit operator (fun operatorNodes -> visit quotedExpr (fun quotedNodes -> operatorNodes @ quotedNodes |> continuation))
        | SynExpr.Typed (expr, targetType, _) -> visit expr (fun nodes -> nodes @ visitSynType targetType |> continuation)
        | SynExpr.Tuple (exprs = exprs) ->
            let continuations = List.map visit exprs
            let finalContinuation = lc id >> continuation
            Continuation.sequence continuations finalContinuation
        | SynExpr.AnonRecd (copyInfo = copyInfo; recordFields = recordFields) ->
            let continuations =
                match copyInfo with
                | None -> List.map (fun (_, _, e) -> visit e) recordFields
                | Some (cp, _) -> visit cp :: List.map (fun (_, _, e) -> visit e) recordFields

            let finalContinuation = lc id >> continuation
            Continuation.sequence continuations finalContinuation
        | SynExpr.ArrayOrList (exprs = exprs) ->
            let continuations = List.map visit exprs
            let finalContinuation = lc id >> continuation
            Continuation.sequence continuations finalContinuation
        | SynExpr.Record (baseInfo = baseInfo; copyInfo = copyInfo; recordFields = recordFields) ->
            let fieldNodes =
                recordFields
                |> lc (fun (SynExprRecordField (fieldName = (si, _); expr = expr)) ->
                    [ yield! visitSynLongIdent si; yield! cfo visitSynExpr expr ])

            match baseInfo, copyInfo with
            | Some (t, e, _, _, _), None ->
                visit e (fun nodes -> [ yield! visitSynType t; yield! nodes; yield! fieldNodes ] |> continuation)
            | None, Some (e, _) -> visit e (fun nodes -> nodes @ fieldNodes |> continuation)
            | _ -> fieldNodes
        | SynExpr.New (targetType = targetType; expr = expr) -> visit expr (fun nodes -> visitSynType targetType @ nodes |> continuation)
        | SynExpr.ObjExpr (objType, argOptions, _, bindings, members, extraImpls, _, _) ->
            [
                yield! visitSynType objType
                yield! cfo (fst >> visitSynExpr) argOptions
                yield! lc visitBinding bindings
                yield! lc visitSynMemberDefn members
                yield! lc visitSynInterfaceImpl extraImpls
            ]
            |> continuation
        | SynExpr.While (whileExpr = whileExpr; doExpr = doExpr) ->
            visit whileExpr (fun whileNodes -> visit doExpr (fun doNodes -> whileNodes @ doNodes |> continuation))
        | SynExpr.For (identBody = identBody; toBody = toBody; doBody = doBody) ->
            let continuations = List.map visit [ identBody; toBody; doBody ]
            let finalContinuation = lc id >> continuation
            Continuation.sequence continuations finalContinuation
        | SynExpr.ForEach (pat = pat; enumExpr = enumExpr; bodyExpr = bodyExpr) ->
            visit enumExpr (fun enumNodes ->
                visit bodyExpr (fun bodyNodes -> [ yield! visitPat pat; yield! enumNodes; yield! bodyNodes ] |> continuation))
        | SynExpr.ArrayOrListComputed (expr = expr) -> visit expr continuation
        | SynExpr.IndexRange (expr1 = expr1; expr2 = expr2) ->
            match expr1, expr2 with
            | None, None -> continuation []
            | Some e, None
            | None, Some e -> visit e continuation
            | Some e1, Some e2 -> visit e1 (fun e1Nodes -> visit e2 (fun e2Nodes -> e1Nodes @ e2Nodes |> continuation))
        | SynExpr.IndexFromEnd (expr, _) -> visit expr continuation
        | SynExpr.ComputationExpr (expr = expr) -> visit expr continuation
        | SynExpr.Lambda (args = args; body = body) -> visit body (fun bodyNodes -> visitSynSimplePats args @ bodyNodes |> continuation)
        | SynExpr.MatchLambda (matchClauses = clauses) -> lc visitSynMatchClause clauses |> continuation
        | SynExpr.Match (expr = expr; clauses = clauses) ->
            visit expr (fun exprNodes -> [ yield! exprNodes; yield! lc visitSynMatchClause clauses ] |> continuation)
        | SynExpr.Do (expr, _) -> visit expr continuation
        | SynExpr.Assert (expr, _) -> visit expr continuation
        | SynExpr.App (funcExpr = funcExpr; argExpr = argExpr) ->
            visit funcExpr (fun funcNodes -> visit argExpr (fun argNodes -> funcNodes @ argNodes |> continuation))
        | SynExpr.TypeApp (expr = expr; typeArgs = typeArgs) ->
            visit expr (fun exprNodes -> exprNodes @ lc visitSynType typeArgs |> continuation)
        | SynExpr.LetOrUse (bindings = bindings; body = body) -> visit body (fun nodes -> lc visitBinding bindings @ nodes |> continuation)
        | SynExpr.TryWith (tryExpr = tryExpr; withCases = withCases) ->
            visit tryExpr (fun nodes -> nodes @ lc visitSynMatchClause withCases |> continuation)
        | SynExpr.TryFinally (tryExpr = tryExpr; finallyExpr = finallyExpr) ->
            visit tryExpr (fun tNodes -> visit finallyExpr (fun fNodes -> tNodes @ fNodes |> continuation))
        | SynExpr.Lazy (expr, _) -> visit expr continuation
        | SynExpr.Sequential (expr1 = expr1; expr2 = expr2) ->
            visit expr1 (fun nodes1 -> visit expr2 (fun nodes2 -> nodes1 @ nodes2 |> continuation))
        | SynExpr.IfThenElse (ifExpr = ifExpr; thenExpr = thenExpr; elseExpr = elseExpr) ->
            let continuations = List.map visit (ifExpr :: thenExpr :: Option.toList elseExpr)
            let finalContinuation = lc id >> continuation
            Continuation.sequence continuations finalContinuation
        | SynExpr.Typar _ -> continuation []
        | SynExpr.Ident _ -> continuation []
        | SynExpr.LongIdent (longDotId = longDotId) -> continuation (visitSynLongIdent longDotId)
        | SynExpr.LongIdentSet (longDotId, expr, _) -> visit expr (fun nodes -> visitSynLongIdent longDotId @ nodes |> continuation)
        | SynExpr.DotGet (expr = expr; longDotId = longDotId) ->
            visit expr (fun nodes -> visitSynLongIdent longDotId @ nodes |> continuation)
        | SynExpr.DotSet (targetExpr, longDotId, rhsExpr, _) ->
            visit targetExpr (fun tNodes ->
                visit rhsExpr (fun rNodes ->
                    [ yield! tNodes; yield! visitSynLongIdent longDotId; yield! rNodes ]
                    |> continuation))
        | SynExpr.Set (targetExpr, rhsExpr, _) ->
            let continuations = List.map visit [ targetExpr; rhsExpr ]
            let finalContinuation = lc id >> continuation
            Continuation.sequence continuations finalContinuation
        | SynExpr.DotIndexedGet (objectExpr, indexArgs, _, _) ->
            let continuations = List.map visit [ objectExpr; indexArgs ]
            let finalContinuation = lc id >> continuation
            Continuation.sequence continuations finalContinuation
        | SynExpr.DotIndexedSet (objectExpr, indexArgs, valueExpr, _, _, _) ->
            let continuations = List.map visit [ objectExpr; indexArgs; valueExpr ]
            let finalContinuation = lc id >> continuation
            Continuation.sequence continuations finalContinuation
        | SynExpr.NamedIndexedPropertySet (longDotId, expr1, expr2, _) ->
            visit expr1 (fun nodes1 ->
                visit expr2 (fun nodes2 ->
                    [ yield! visitSynLongIdent longDotId; yield! nodes1; yield! nodes2 ]
                    |> continuation))
        | SynExpr.DotNamedIndexedPropertySet (targetExpr, longDotId, argExpr, rhsExpr, _) ->
            let continuations = List.map visit [ targetExpr; argExpr; rhsExpr ]

            let finalContinuation nodes =
                visitSynLongIdent longDotId @ lc id nodes |> continuation

            Continuation.sequence continuations finalContinuation
        | SynExpr.TypeTest (expr, targetType, _) -> visit expr (fun nodes -> nodes @ visitSynType targetType |> continuation)
        | SynExpr.Upcast (expr, targetType, _) -> visit expr (fun nodes -> nodes @ visitSynType targetType |> continuation)
        | SynExpr.Downcast (expr, targetType, _) -> visit expr (fun nodes -> nodes @ visitSynType targetType |> continuation)
        | SynExpr.InferredUpcast (expr, _) -> visit expr continuation
        | SynExpr.InferredDowncast (expr, _) -> visit expr continuation
        | SynExpr.Null _ -> continuation []
        | SynExpr.AddressOf (expr = expr) -> visit expr continuation
        | SynExpr.TraitCall (supportTys, traitSig, argExpr, _) ->
            visit argExpr (fun nodes ->
                [
                    yield! visitSynType supportTys
                    yield! visitSynMemberSig traitSig
                    yield! nodes
                ]
                |> continuation)
        | SynExpr.JoinIn (lhsExpr, _, rhsExpr, _) ->
            let continuations = List.map visit [ lhsExpr; rhsExpr ]
            let finalContinuation = lc id >> continuation
            Continuation.sequence continuations finalContinuation
        | SynExpr.ImplicitZero _ -> continuation []
        | SynExpr.SequentialOrImplicitYield (_, expr1, expr2, _, _) ->
            let continuations = List.map visit [ expr1; expr2 ]
            let finalContinuation = lc id >> continuation
            Continuation.sequence continuations finalContinuation
        | SynExpr.YieldOrReturn (expr = expr) -> visit expr continuation
        | SynExpr.YieldOrReturnFrom (expr = expr) -> visit expr continuation
        | SynExpr.LetOrUseBang (pat = pat; rhs = rhs; andBangs = andBangs; body = body) ->
            let continuations =
                let andBangExprs = List.map (fun (SynExprAndBang (body = body)) -> body) andBangs
                List.map visit (body :: rhs :: andBangExprs)

            let finalContinuation nodes =
                [
                    yield! lc id nodes
                    yield! visitPat pat
                    yield! lc (fun (SynExprAndBang (pat = pat)) -> visitPat pat) andBangs
                ]
                |> continuation

            Continuation.sequence continuations finalContinuation
        | SynExpr.MatchBang (expr = expr; clauses = clauses) ->
            visit expr (fun exprNodes -> [ yield! exprNodes; yield! lc visitSynMatchClause clauses ] |> continuation)
        | SynExpr.DoBang (expr, _) -> visit expr continuation
        | SynExpr.LibraryOnlyILAssembly (typeArgs = typeArgs; args = args; retTy = retTy) ->
            let typeNodes = lc visitSynType (typeArgs @ retTy)
            let continuations = List.map visit args
            let finalContinuation nodes = lc id nodes @ typeNodes |> continuation
            Continuation.sequence continuations finalContinuation
        | SynExpr.LibraryOnlyStaticOptimization (constraints, expr, optimizedExpr, _) ->
            let constraintTypes =
                constraints
                |> List.choose (function
                    | SynStaticOptimizationConstraint.WhenTyparTyconEqualsTycon (rhsType = t) -> Some t
                    | SynStaticOptimizationConstraint.WhenTyparIsStruct _ -> None)

            visit expr (fun eNodes ->
                visit optimizedExpr (fun oNodes ->
                    [ yield! lc visitSynType constraintTypes; yield! eNodes; yield! oNodes ]
                    |> continuation))
        | SynExpr.LibraryOnlyUnionCaseFieldGet (expr, longId, _, _) ->
            visit expr (fun eNodes -> visitLongIdent longId @ eNodes |> continuation)
        | SynExpr.LibraryOnlyUnionCaseFieldSet (expr, longId, _, rhsExpr, _) ->
            visit expr (fun eNodes ->
                visit rhsExpr (fun rhsNodes -> [ yield! visitLongIdent longId; yield! eNodes; yield! rhsNodes ] |> continuation))
        | SynExpr.ArbitraryAfterError _ -> continuation []
        | SynExpr.FromParseError _ -> continuation []
        | SynExpr.DiscardAfterMissingQualificationAfterDot _ -> continuation []
        | SynExpr.Fixed (expr, _) -> visit expr continuation
        | SynExpr.InterpolatedString (contents = contents) ->
            let continuations =
                List.map
                    visit
                    (List.choose
                        (function
                        | SynInterpolatedStringPart.FillExpr (fillExpr = e) -> Some e
                        | SynInterpolatedStringPart.String _ -> None)
                        contents)

            let finalContinuation = lc id >> continuation
            Continuation.sequence continuations finalContinuation
        | SynExpr.DebugPoint _ -> continuation []
        | SynExpr.Dynamic (funcExpr, _, argExpr, _) ->
            let continuations = List.map visit [ funcExpr; argExpr ]
            let finalContinuation = lc id >> continuation
            Continuation.sequence continuations finalContinuation

    visit e id

let visitPat (p: SynPat) : FileContentEntry list =
    let rec visit (p: SynPat) (continuation: FileContentEntry list -> FileContentEntry list) : FileContentEntry list =
        match p with
        | SynPat.Paren (pat = pat) -> visit pat continuation
        | SynPat.Typed (pat = pat; targetType = t) -> visit pat (fun nodes -> nodes @ visitSynType t)
        | SynPat.Const _ -> continuation []
        | SynPat.Wild _ -> continuation []
        | SynPat.Named _ -> continuation []
        | SynPat.Attrib (pat, attributes, _) -> visit pat (fun nodes -> visitSynAttributes attributes @ nodes |> continuation)
        | SynPat.Or (lhsPat, rhsPat, _, _) ->
            let continuations = List.map visit [ lhsPat; rhsPat ]
            let finalContinuation = lc id >> continuation
            Continuation.sequence continuations finalContinuation
        | SynPat.ListCons (lhsPat, rhsPat, _, _) ->
            let continuations = List.map visit [ lhsPat; rhsPat ]
            let finalContinuation = lc id >> continuation
            Continuation.sequence continuations finalContinuation
        | SynPat.Ands (pats, _) ->
            let continuations = List.map visit pats
            let finalContinuation = lc id >> continuation
            Continuation.sequence continuations finalContinuation
        | SynPat.As (lhsPat, rhsPat, _) ->
            let continuations = List.map visit [ lhsPat; rhsPat ]
            let finalContinuation = lc id >> continuation
            Continuation.sequence continuations finalContinuation
        | SynPat.LongIdent (longDotId = longDotId; typarDecls = typarDecls; argPats = argPats) ->
            continuation
                [
                    yield! visitSynLongIdent longDotId
                    yield! cfo visitSynValTyparDecls typarDecls
                    yield! visitSynArgPats argPats
                ]
        | SynPat.Tuple (_, elementPats, _) ->
            let continuations = List.map visit elementPats
            let finalContinuation = lc id >> continuation
            Continuation.sequence continuations finalContinuation
        | SynPat.ArrayOrList (_, elementPats, _) ->
            let continuations = List.map visit elementPats
            let finalContinuation = lc id >> continuation
            Continuation.sequence continuations finalContinuation
        | SynPat.Record (fieldPats, _) ->
            let pats = List.map (fun (_, _, p) -> p) fieldPats
            let lids = lc (fun ((l, _), _, _) -> visitLongIdent l) fieldPats
            let continuations = List.map visit pats

            let finalContinuation nodes =
                [ yield! lc id nodes; yield! lids ] |> continuation

            Continuation.sequence continuations finalContinuation
        | SynPat.Null _ -> continuation []
        | SynPat.OptionalVal _ -> continuation []
        | SynPat.IsInst (t, _) -> continuation (visitSynType t)
        | SynPat.QuoteExpr (expr, _) -> continuation (visitSynExpr expr)
        | SynPat.DeprecatedCharRange _ -> continuation []
        | SynPat.InstanceMember _ -> continuation []
        | SynPat.FromParseError _ -> continuation []

    visit p id

let visitSynArgPats (argPat: SynArgPats) =
    match argPat with
    | SynArgPats.Pats args -> lc visitPat args
    | SynArgPats.NamePatPairs (pats = pats) -> lc (fun (_, _, p) -> visitPat p) pats

let visitSynSimplePat (pat: SynSimplePat) =
    match pat with
    | SynSimplePat.Id _ -> []
    | SynSimplePat.Attrib (pat, attributes, _) -> [ yield! visitSynSimplePat pat; yield! visitSynAttributes attributes ]
    | SynSimplePat.Typed (pat, t, _) -> [ yield! visitSynSimplePat pat; yield! visitSynType t ]

let visitSynSimplePats (pats: SynSimplePats) =
    match pats with
    | SynSimplePats.SimplePats (pats = pats) -> lc visitSynSimplePat pats
    | SynSimplePats.Typed (pats, t, _) -> [ yield! visitSynSimplePats pats; yield! visitSynType t ]

let visitSynMatchClause (SynMatchClause (pat = pat; whenExpr = whenExpr; resultExpr = resultExpr)) =
    [
        yield! visitPat pat
        yield! cfo visitSynExpr whenExpr
        yield! visitSynExpr resultExpr
    ]

let visitBinding (SynBinding (attributes = attributes; headPat = headPat; returnInfo = returnInfo; expr = expr)) : FileContentEntry list =
    let pattern =
        match headPat with
        | SynPat.LongIdent(argPats = SynArgPats.Pats pats) -> lc visitPat pats
        | _ -> visitPat headPat

    [
        yield! visitSynAttributes attributes
        yield! pattern
        yield! cfo visitSynBindingReturnInfo returnInfo
        yield! visitSynExpr expr
    ]

let visitSynBindingReturnInfo (SynBindingReturnInfo (typeName = typeName; attributes = attributes)) =
    [ yield! visitSynAttributes attributes; yield! visitSynType typeName ]

let visitSynMemberSig (ms: SynMemberSig) : FileContentEntry list =
    match ms with
    | SynMemberSig.Member (memberSig = memberSig) -> visitSynValSig memberSig
    | SynMemberSig.Interface (interfaceType, _) -> visitSynType interfaceType
    | SynMemberSig.Inherit (inheritedType, _) -> visitSynType inheritedType
    | SynMemberSig.ValField (field, _) -> visitSynField field
    | SynMemberSig.NestedType _ -> []

let mkFileContent (f: FileWithAST) : FileContentEntry list =
    match f.AST with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = contents)) ->
        lc
            (fun (SynModuleOrNamespaceSig (longId = longId; kind = kind; decls = decls; attribs = attribs)) ->
                let attributes = lc visitSynAttributeList attribs

                let contentEntries =
                    match kind with
                    | SynModuleOrNamespaceKind.GlobalNamespace
                    | SynModuleOrNamespaceKind.AnonModule -> lc visitSynModuleSigDecl decls
                    | SynModuleOrNamespaceKind.DeclaredNamespace ->
                        let path = longIdentToPath false longId

                        [ FileContentEntry.TopLevelNamespace(path, lc visitSynModuleSigDecl decls) ]
                    | SynModuleOrNamespaceKind.NamedModule ->
                        let path = longIdentToPath true longId

                        [ FileContentEntry.TopLevelNamespace(path, lc visitSynModuleSigDecl decls) ]

                [ yield! attributes; yield! contentEntries ])
            contents

    | ParsedInput.ImplFile (ParsedImplFileInput (contents = contents)) ->
        lc
            (fun (SynModuleOrNamespace (longId = longId; attribs = attribs; kind = kind; decls = decls)) ->
                let attributes = lc visitSynAttributeList attribs

                let contentEntries =
                    match kind with
                    | SynModuleOrNamespaceKind.GlobalNamespace
                    | SynModuleOrNamespaceKind.AnonModule -> lc visitSynModuleDecl decls
                    | SynModuleOrNamespaceKind.DeclaredNamespace ->
                        let path = longIdentToPath false longId

                        [ FileContentEntry.TopLevelNamespace(path, lc visitSynModuleDecl decls) ]
                    | SynModuleOrNamespaceKind.NamedModule ->
                        let path = longIdentToPath true longId

                        [ FileContentEntry.TopLevelNamespace(path, lc visitSynModuleDecl decls) ]

                [ yield! attributes; yield! contentEntries ])
            contents

// ================================================================================================================================
// ================================================================================================================================
module Tests =
    open NUnit.Framework
    open FSharp.Compiler.Service.Tests.Common

    [<Test>]
    let ``Test a single file`` () =
        let fileName =
            @"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\CodeFormatter.fsi"

        let ast = parseSourceCode (fileName, System.IO.File.ReadAllText(fileName))
        let contents = mkFileContent { Idx = 0; File = fileName; AST = ast }
        ignore contents
