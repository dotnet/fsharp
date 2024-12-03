module internal rec FSharp.Compiler.GraphChecking.FileContentMapping

open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTreeOps

type Continuations = ((FileContentEntry list -> FileContentEntry list) -> FileContentEntry list) list

/// Collect a list of 'U from option 'T via a mapping function.
let collectFromOption (mapping: 'T -> 'U list) (t: 'T option) : 'U list = List.collect mapping (Option.toList t)

let longIdentToPath (skipLast: bool) (longId: LongIdent) : LongIdentifier =

    // We always skip the "special" `global` identifier.
    let longId =
        match longId with
        | h :: t when h.idText = "`global`" -> t
        | _ -> longId

    match skipLast, longId with
    | true, _ :: _ -> List.take (longId.Length - 1) longId
    | _ -> longId
    |> List.map (fun ident -> ident.idText)

let synLongIdentToPath (skipLast: bool) (synLongIdent: SynLongIdent) =
    longIdentToPath skipLast synLongIdent.LongIdent

/// In some rare cases we are interested in the name of a single Ident.
/// For example `nameof ModuleName` in expressions or patterns.
let visitIdentAsPotentialModuleName (moduleNameIdent: Ident) =
    FileContentEntry.ModuleName moduleNameIdent.idText

let visitSynLongIdent (lid: SynLongIdent) : FileContentEntry list = visitLongIdent lid.LongIdent

let visitLongIdent (lid: LongIdent) =
    match lid with
    | []
    | [ _ ] -> []
    | lid -> [ FileContentEntry.PrefixedIdentifier(longIdentToPath true lid) ]

let visitLongIdentForModuleAbbrev (lid: LongIdent) =
    match lid with
    | [] -> []
    | lid -> [ FileContentEntry.PrefixedIdentifier(longIdentToPath false lid) ]

let visitSynAttribute (a: SynAttribute) : FileContentEntry list =
    [ yield! visitSynLongIdent a.TypeName; yield! visitSynExpr a.ArgExpr ]

let visitSynAttributeList (attributes: SynAttributeList) : FileContentEntry list =
    List.collect visitSynAttribute attributes.Attributes

let visitSynAttributes (attributes: SynAttributes) : FileContentEntry list =
    List.collect visitSynAttributeList attributes

let visitSynModuleDecl (decl: SynModuleDecl) : FileContentEntry list =
    [
        match decl with
        | SynModuleDecl.Open(target = SynOpenDeclTarget.ModuleOrNamespace(longId, _)) ->
            yield FileContentEntry.OpenStatement(synLongIdentToPath false longId)
        | SynModuleDecl.Open(target = SynOpenDeclTarget.Type(typeName, _)) -> yield! visitSynType typeName
        | SynModuleDecl.Attributes(attributes, _) -> yield! List.collect visitSynAttributeList attributes
        | SynModuleDecl.Expr(expr, _) -> yield! visitSynExpr expr
        | SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo(longId = [ ident ]; attributes = attributes); decls = decls) ->
            yield! visitSynAttributes attributes
            yield FileContentEntry.NestedModule(ident.idText, List.collect visitSynModuleDecl decls)
        | SynModuleDecl.NestedModule _ -> () // A nested module cannot have multiple identifiers. This will already be a parse error, but we could be working with recovered syntax tree
        | SynModuleDecl.Let(bindings = bindings) -> yield! List.collect visitBinding bindings
        | SynModuleDecl.Types(typeDefns = typeDefns) -> yield! List.collect visitSynTypeDefn typeDefns
        | SynModuleDecl.HashDirective _ -> ()
        | SynModuleDecl.ModuleAbbrev(longId = longId) -> yield! visitLongIdentForModuleAbbrev longId
        | SynModuleDecl.NamespaceFragment _ -> ()
        | SynModuleDecl.Exception(
            exnDefn = SynExceptionDefn(
                exnRepr = SynExceptionDefnRepr(attributes = attributes; caseName = caseName; longId = longId); members = members)) ->
            yield! visitSynAttributes attributes
            yield! visitSynUnionCase caseName
            yield! collectFromOption visitLongIdent longId
            yield! List.collect visitSynMemberDefn members
    ]

let visitSynModuleSigDecl (md: SynModuleSigDecl) =
    [
        match md with
        | SynModuleSigDecl.Open(target = SynOpenDeclTarget.ModuleOrNamespace(longId, _)) ->
            yield FileContentEntry.OpenStatement(synLongIdentToPath false longId)
        | SynModuleSigDecl.Open(target = SynOpenDeclTarget.Type(typeName, _)) -> yield! visitSynType typeName
        | SynModuleSigDecl.NestedModule(moduleInfo = SynComponentInfo(longId = [ ident ]; attributes = attributes); moduleDecls = decls) ->
            yield! visitSynAttributes attributes
            yield FileContentEntry.NestedModule(ident.idText, List.collect visitSynModuleSigDecl decls)
        | SynModuleSigDecl.NestedModule _ -> () // A nested module cannot have multiple identifiers. This will already be a parse error, but we could be working with recovered syntax tree
        | SynModuleSigDecl.ModuleAbbrev(longId = longId) -> yield! visitLongIdentForModuleAbbrev longId
        | SynModuleSigDecl.Val(valSig, _) -> yield! visitSynValSig valSig
        | SynModuleSigDecl.Types(types = types) -> yield! List.collect visitSynTypeDefnSig types
        | SynModuleSigDecl.Exception(
            exnSig = SynExceptionSig(
                exnRepr = SynExceptionDefnRepr(attributes = attributes; caseName = caseName; longId = longId); members = members)) ->
            yield! visitSynAttributes attributes
            yield! visitSynUnionCase caseName
            yield! collectFromOption visitLongIdent longId
            yield! List.collect visitSynMemberSig members
        | SynModuleSigDecl.HashDirective _
        | SynModuleSigDecl.NamespaceFragment _ -> ()
    ]

let visitSynUnionCase (SynUnionCase(attributes = attributes; caseType = caseType)) =
    [
        yield! visitSynAttributes attributes
        match caseType with
        | SynUnionCaseKind.Fields cases -> yield! List.collect visitSynField cases
        | SynUnionCaseKind.FullType(fullType = fullType) -> yield! visitSynType fullType
    ]

let visitSynEnumCase (SynEnumCase(attributes = attributes)) = visitSynAttributes attributes

let visitSynTypeDefn
    (SynTypeDefn(
        typeInfo = SynComponentInfo(attributes = attributes; longId = longId; typeParams = typeParams; constraints = constraints)
        typeRepr = typeRepr
        members = members))
    : FileContentEntry list =
    [
        yield! visitSynAttributes attributes
        yield! collectFromOption visitSynTyparDecls typeParams
        yield! List.collect visitSynTypeConstraint constraints
        match typeRepr with
        | SynTypeDefnRepr.Simple(simpleRepr, _) ->
            match simpleRepr with
            | SynTypeDefnSimpleRepr.Union(unionCases = unionCases) -> yield! List.collect visitSynUnionCase unionCases
            | SynTypeDefnSimpleRepr.Enum(cases = cases) -> yield! List.collect visitSynEnumCase cases
            | SynTypeDefnSimpleRepr.Record(recordFields = recordFields) -> yield! List.collect visitSynField recordFields
            // This is only used in the typed tree
            // The parser doesn't construct this
            | SynTypeDefnSimpleRepr.General _
            | SynTypeDefnSimpleRepr.LibraryOnlyILAssembly _ -> ()
            | SynTypeDefnSimpleRepr.TypeAbbrev(rhsType = rhsType) -> yield! visitSynType rhsType
            | SynTypeDefnSimpleRepr.None _
            // This is only used in the typed tree
            // The parser doesn't construct this
            | SynTypeDefnSimpleRepr.Exception _ -> ()
        | SynTypeDefnRepr.ObjectModel(kind, members, _) ->
            match kind with
            | SynTypeDefnKind.Delegate(signature, _) ->
                yield! visitSynType signature
                yield! List.collect visitSynMemberDefn members
            | SynTypeDefnKind.Augmentation _ ->
                yield! visitLongIdent longId
                yield! List.collect visitSynMemberDefn members
            | _ -> yield! List.collect visitSynMemberDefn members
        | SynTypeDefnRepr.Exception _ ->
            // This is only used in the typed tree
            // The parser doesn't construct this
            ()
        yield! List.collect visitSynMemberDefn members
    ]

let visitSynTypeDefnSig
    (SynTypeDefnSig(
        typeInfo = SynComponentInfo(attributes = attributes; longId = longId; typeParams = typeParams; constraints = constraints)
        typeRepr = typeRepr
        members = members))
    =
    [
        yield! visitSynAttributes attributes
        yield! collectFromOption visitSynTyparDecls typeParams
        yield! List.collect visitSynTypeConstraint constraints
        match typeRepr with
        | SynTypeDefnSigRepr.Simple(simpleRepr, _) ->
            match simpleRepr with
            | SynTypeDefnSimpleRepr.Union(unionCases = unionCases) -> yield! List.collect visitSynUnionCase unionCases
            | SynTypeDefnSimpleRepr.Enum(cases = cases) -> yield! List.collect visitSynEnumCase cases
            | SynTypeDefnSimpleRepr.Record(recordFields = recordFields) -> yield! List.collect visitSynField recordFields
            // This is only used in the typed tree
            // The parser doesn't construct this
            | SynTypeDefnSimpleRepr.General _
            | SynTypeDefnSimpleRepr.LibraryOnlyILAssembly _ -> ()
            | SynTypeDefnSimpleRepr.TypeAbbrev(rhsType = rhsType) -> yield! visitSynType rhsType
            // This is a type augmentation in a signature file
            | SynTypeDefnSimpleRepr.None _ ->
                yield! visitLongIdent longId
                yield! List.collect visitSynMemberSig members
            // This is only used in the typed tree
            // The parser doesn't construct this
            | SynTypeDefnSimpleRepr.Exception _ -> ()
        | SynTypeDefnSigRepr.ObjectModel(kind, members, _) ->
            match kind with
            | SynTypeDefnKind.Delegate(signature, _) ->
                yield! visitSynType signature
                yield! List.collect visitSynMemberSig members
            | _ -> yield! List.collect visitSynMemberSig members
        | SynTypeDefnSigRepr.Exception _ ->
            // This is only used in the typed tree
            // The parser doesn't construct this
            ()
        yield! List.collect visitSynMemberSig members
    ]

let visitSynValSig (SynValSig(attributes = attributes; synType = synType; synExpr = synExpr)) =
    [
        yield! visitSynAttributes attributes
        yield! visitSynType synType
        yield! collectFromOption visitSynExpr synExpr
    ]

let visitSynField (SynField(attributes = attributes; fieldType = fieldType)) =
    visitSynAttributes attributes @ visitSynType fieldType

let visitSynMemberDefn (md: SynMemberDefn) : FileContentEntry list =
    [
        match md with
        | SynMemberDefn.Member(memberDefn = binding) -> yield! visitBinding binding
        | SynMemberDefn.Open _ -> ()
        | SynMemberDefn.GetSetMember(memberDefnForGet, memberDefnForSet, _, _) ->
            yield! collectFromOption visitBinding memberDefnForGet
            yield! collectFromOption visitBinding memberDefnForSet
        | SynMemberDefn.ImplicitCtor(ctorArgs = pat) -> yield! visitPat pat
        | SynMemberDefn.ImplicitInherit(inheritType, inheritArgs, _, _, _) ->
            yield! visitSynType inheritType
            yield! visitSynExpr inheritArgs
        | SynMemberDefn.LetBindings(bindings = bindings) -> yield! List.collect visitBinding bindings
        | SynMemberDefn.AbstractSlot(slotSig = slotSig) -> yield! visitSynValSig slotSig
        | SynMemberDefn.Interface(interfaceType, _, members, _) ->
            yield! visitSynType interfaceType
            yield! collectFromOption (List.collect visitSynMemberDefn) members
        | SynMemberDefn.Inherit(baseType = Some baseType) -> yield! visitSynType baseType
        | SynMemberDefn.Inherit(baseType = None) -> ()
        | SynMemberDefn.ValField(fieldInfo, _) -> yield! visitSynField fieldInfo
        | SynMemberDefn.NestedType _ -> ()
        | SynMemberDefn.AutoProperty(attributes = attributes; typeOpt = typeOpt; synExpr = synExpr) ->
            yield! visitSynAttributes attributes
            yield! collectFromOption visitSynType typeOpt
            yield! visitSynExpr synExpr
    ]

let visitSynInterfaceImpl (SynInterfaceImpl(interfaceTy = t; bindings = bindings; members = members)) =
    [
        yield! visitSynType t
        yield! List.collect visitBinding bindings
        yield! List.collect visitSynMemberDefn members
    ]

let visitSynType (t: SynType) : FileContentEntry list =
    let rec visit (t: SynType) (continuation: FileContentEntry list -> FileContentEntry list) =
        match t with
        | SynType.LongIdent lid -> continuation (visitSynLongIdent lid)
        | SynType.App(typeName = typeName; typeArgs = typeArgs) ->
            let continuations = List.map visit (typeName :: typeArgs)
            Continuation.concatenate continuations continuation
        | SynType.LongIdentApp(typeName = typeName; longDotId = longDotId; typeArgs = typeArgs) ->
            let continuations = List.map visit (typeName :: typeArgs)

            let finalContinuation nodes =
                visitSynLongIdent longDotId @ List.concat nodes |> continuation

            Continuation.sequence continuations finalContinuation
        | SynType.Tuple(path = path) ->
            let continuations = List.map visit (getTypeFromTuplePath path)
            Continuation.concatenate continuations continuation
        | SynType.AnonRecd(fields = fields) ->
            let continuations = List.map (snd >> visit) fields
            Continuation.concatenate continuations continuation
        | SynType.Array(elementType = elementType) -> visit elementType continuation
        | SynType.WithNull(innerType = innerType) -> visit innerType continuation
        | SynType.Fun(argType, returnType, _, _) ->
            let continuations = List.map visit [ argType; returnType ]
            Continuation.concatenate continuations continuation
        | SynType.Var _ -> continuation []
        | SynType.Anon _ -> continuation []
        | SynType.WithGlobalConstraints(typeName, constraints, _) ->
            visit typeName (fun nodes -> nodes @ List.collect visitSynTypeConstraint constraints |> continuation)
        | SynType.HashConstraint(innerType, _) -> visit innerType continuation
        | SynType.MeasurePower(baseMeasure = baseMeasure) -> visit baseMeasure continuation
        | SynType.StaticConstant _ -> continuation []
        | SynType.StaticConstantNull _ -> continuation []
        | SynType.StaticConstantExpr(expr, _) -> continuation (visitSynExpr expr)
        | SynType.StaticConstantNamed(ident, value, _) ->
            let continuations = List.map visit [ ident; value ]
            Continuation.concatenate continuations continuation
        | SynType.Paren(innerType, _) -> visit innerType continuation
        | SynType.SignatureParameter(attributes = attributes; usedType = usedType) ->
            visit usedType (fun nodes -> [ yield! visitSynAttributes attributes; yield! nodes ] |> continuation)
        | SynType.Or(lhsType, rhsType, _, _) ->
            let continuations = List.map visit [ lhsType; rhsType ]
            Continuation.concatenate continuations continuation
        | SynType.FromParseError _ -> continuation []
        | SynType.Intersection(types = types) ->
            let continuations = List.map visit types
            Continuation.concatenate continuations continuation

    visit t id

let visitSynValTyparDecls (SynValTyparDecls(typars = typars)) =
    collectFromOption visitSynTyparDecls typars

let visitSynTyparDecls (td: SynTyparDecls) : FileContentEntry list =
    match td with
    | SynTyparDecls.PostfixList(decls, constraints, _) ->
        List.collect visitSynTyparDecl decls
        @ List.collect visitSynTypeConstraint constraints
    | SynTyparDecls.PrefixList(decls = decls) -> List.collect visitSynTyparDecl decls
    | SynTyparDecls.SinglePrefix(decl = decl) -> visitSynTyparDecl decl

let visitSynTyparDecl (SynTyparDecl(attributes = attributes; intersectionConstraints = constraints)) =
    visitSynAttributes attributes @ List.collect visitSynType constraints

let visitSynTypeConstraint (tc: SynTypeConstraint) : FileContentEntry list =
    match tc with
    | SynTypeConstraint.WhereSelfConstrained _
    | SynTypeConstraint.WhereTyparIsValueType _
    | SynTypeConstraint.WhereTyparIsReferenceType _
    | SynTypeConstraint.WhereTyparIsUnmanaged _
    | SynTypeConstraint.WhereTyparSupportsNull _
    | SynTypeConstraint.WhereTyparNotSupportsNull _
    | SynTypeConstraint.WhereTyparIsComparable _
    | SynTypeConstraint.WhereTyparIsEquatable _ -> []
    | SynTypeConstraint.WhereTyparDefaultsToType(typeName = typeName) -> visitSynType typeName
    | SynTypeConstraint.WhereTyparSubtypeOfType(typeName = typeName) -> visitSynType typeName
    | SynTypeConstraint.WhereTyparSupportsMember(typars, memberSig, _) -> visitSynType typars @ visitSynMemberSig memberSig
    | SynTypeConstraint.WhereTyparIsEnum(typeArgs = typeArgs) -> List.collect visitSynType typeArgs
    | SynTypeConstraint.WhereTyparIsDelegate(typeArgs = typeArgs) -> List.collect visitSynType typeArgs

[<return: Struct>]
let inline (|NameofIdent|_|) (ident: Ident) =
    if ident.idText = "nameof" then ValueSome() else ValueNone

/// nameof X.Y.Z can be used in expressions and patterns
[<RequireQualifiedAccess; NoComparison>]
type NameofResult =
    /// Example: nameof X
    /// Where X is a module name
    | SingleIdent of potentialModuleName: Ident
    /// Example: nameof X.Y.Z
    /// Where Z is either a module name or something from inside module or namespace Y.
    /// Both options need to be explored.
    | LongIdent of longIdent: LongIdent

let visitNameofResult (nameofResult: NameofResult) : FileContentEntry =
    match nameofResult with
    | NameofResult.SingleIdent moduleName -> visitIdentAsPotentialModuleName moduleName
    | NameofResult.LongIdent longIdent ->
        // In this case the last part of the LongIdent could be a module name.
        // So we should not cut off the last part.
        FileContentEntry.PrefixedIdentifier(longIdentToPath false longIdent)

/// Special case of `nameof Module` type of expression
[<return: Struct>]
let (|NameofExpr|_|) (e: SynExpr) : NameofResult voption =
    let rec stripParen (e: SynExpr) =
        match e with
        | SynExpr.Paren(expr = expr) -> stripParen expr
        | _ -> e

    match e with
    | SynExpr.App(flag = ExprAtomicFlag.NonAtomic; isInfix = false; funcExpr = SynExpr.Ident NameofIdent; argExpr = moduleNameExpr) ->
        match stripParen moduleNameExpr with
        | SynExpr.Ident moduleNameIdent -> ValueSome(NameofResult.SingleIdent moduleNameIdent)
        | SynExpr.LongIdent(longDotId = longIdent) ->
            match longIdent.LongIdent with
            | [] -> ValueNone
            // This is highly unlikely to be produced by the parser
            | [ moduleNameIdent ] -> ValueSome(NameofResult.SingleIdent moduleNameIdent)
            | lid -> ValueSome(NameofResult.LongIdent(lid))
        | _ -> ValueNone
    | _ -> ValueNone

let visitSynExpr (e: SynExpr) : FileContentEntry list =
    let rec visit (e: SynExpr) (continuation: FileContentEntry list -> FileContentEntry list) : FileContentEntry list =
        match e with
        | NameofExpr nameofResult -> continuation [ visitNameofResult nameofResult ]
        | SynExpr.Const _ -> continuation []
        | SynExpr.Paren(expr = expr) -> visit expr continuation
        | SynExpr.Quote(operator = operator; quotedExpr = quotedExpr) ->
            visit operator (fun operatorNodes -> visit quotedExpr (fun quotedNodes -> operatorNodes @ quotedNodes |> continuation))
        | SynExpr.Typed(expr, targetType, _) -> visit expr (fun nodes -> nodes @ visitSynType targetType |> continuation)
        | SynExpr.Tuple(exprs = exprs) ->
            let continuations: ((FileContentEntry list -> FileContentEntry list) -> FileContentEntry list) list =
                List.map visit exprs

            Continuation.concatenate continuations continuation
        | SynExpr.AnonRecd(copyInfo = copyInfo; recordFields = recordFields) ->
            let continuations =
                match copyInfo with
                | None -> List.map (fun (_, _, e) -> visit e) recordFields
                | Some(cp, _) -> visit cp :: List.map (fun (_, _, e) -> visit e) recordFields

            Continuation.concatenate continuations continuation
        | SynExpr.ArrayOrList(exprs = exprs) ->
            let continuations = List.map visit exprs
            Continuation.concatenate continuations continuation
        | SynExpr.Record(baseInfo = baseInfo; copyInfo = copyInfo; recordFields = recordFields) ->
            let fieldNodes =
                [
                    for SynExprRecordField(fieldName = (si, _); expr = expr) in recordFields do
                        yield! visitSynLongIdent si
                        yield! collectFromOption visitSynExpr expr
                ]

            match baseInfo, copyInfo with
            | Some(t, e, _, _, _), None -> visit e (fun nodes -> [ yield! visitSynType t; yield! nodes; yield! fieldNodes ] |> continuation)
            | None, Some(e, _) -> visit e (fun nodes -> nodes @ fieldNodes |> continuation)
            | _ -> continuation fieldNodes
        | SynExpr.New(targetType = targetType; expr = expr) -> visit expr (fun nodes -> visitSynType targetType @ nodes |> continuation)
        | SynExpr.ObjExpr(objType, argOptions, _, bindings, members, extraImpls, _, _) ->
            [
                yield! visitSynType objType
                yield! collectFromOption (fst >> visitSynExpr) argOptions
                yield! List.collect visitBinding bindings
                yield! List.collect visitSynMemberDefn members
                yield! List.collect visitSynInterfaceImpl extraImpls
            ]
            |> continuation
        | SynExpr.While(whileExpr = whileExpr; doExpr = doExpr) ->
            visit whileExpr (fun whileNodes -> visit doExpr (fun doNodes -> whileNodes @ doNodes |> continuation))
        | SynExpr.For(identBody = identBody; toBody = toBody; doBody = doBody) ->
            let continuations = List.map visit [ identBody; toBody; doBody ]
            Continuation.concatenate continuations continuation
        | SynExpr.ForEach(pat = pat; enumExpr = enumExpr; bodyExpr = bodyExpr) ->
            visit enumExpr (fun enumNodes ->
                visit bodyExpr (fun bodyNodes -> [ yield! visitPat pat; yield! enumNodes; yield! bodyNodes ] |> continuation))
        | SynExpr.ArrayOrListComputed(expr = expr) -> visit expr continuation
        | SynExpr.IndexRange(expr1 = expr1; expr2 = expr2) ->
            match expr1, expr2 with
            | None, None -> continuation []
            | Some e, None
            | None, Some e -> visit e continuation
            | Some e1, Some e2 -> visit e1 (fun e1Nodes -> visit e2 (fun e2Nodes -> e1Nodes @ e2Nodes |> continuation))
        | SynExpr.IndexFromEnd(expr, _) -> visit expr continuation
        | SynExpr.ComputationExpr(expr = expr) -> visit expr continuation
        | SynExpr.Lambda(args = args; body = body) -> visit body (fun bodyNodes -> visitSynSimplePats args @ bodyNodes |> continuation)
        | SynExpr.DotLambda(expr = expr) -> visit expr continuation
        | SynExpr.MatchLambda(matchClauses = clauses) -> List.collect visitSynMatchClause clauses |> continuation
        | SynExpr.Match(expr = expr; clauses = clauses) ->
            visit expr (fun exprNodes ->
                [ yield! exprNodes; yield! List.collect visitSynMatchClause clauses ]
                |> continuation)
        | SynExpr.Do(expr, _) -> visit expr continuation
        | SynExpr.Assert(expr, _) -> visit expr continuation
        | SynExpr.App(funcExpr = funcExpr; argExpr = argExpr) ->
            visit funcExpr (fun funcNodes -> visit argExpr (fun argNodes -> funcNodes @ argNodes |> continuation))
        | SynExpr.TypeApp(expr = expr; typeArgs = typeArgs) ->
            visit expr (fun exprNodes -> exprNodes @ List.collect visitSynType typeArgs |> continuation)
        | SynExpr.LetOrUse(bindings = bindings; body = body) ->
            visit body (fun nodes -> List.collect visitBinding bindings @ nodes |> continuation)
        | SynExpr.TryWith(tryExpr = tryExpr; withCases = withCases) ->
            visit tryExpr (fun nodes -> nodes @ List.collect visitSynMatchClause withCases |> continuation)
        | SynExpr.TryFinally(tryExpr = tryExpr; finallyExpr = finallyExpr) ->
            visit tryExpr (fun tNodes -> visit finallyExpr (fun fNodes -> tNodes @ fNodes |> continuation))
        | SynExpr.Lazy(expr, _) -> visit expr continuation
        | SynExpr.Sequential(expr1 = expr1; expr2 = expr2) ->
            visit expr1 (fun nodes1 -> visit expr2 (fun nodes2 -> nodes1 @ nodes2 |> continuation))
        | SynExpr.IfThenElse(ifExpr = ifExpr; thenExpr = thenExpr; elseExpr = elseExpr) ->
            let continuations = List.map visit (ifExpr :: thenExpr :: Option.toList elseExpr)
            Continuation.concatenate continuations continuation
        | SynExpr.Typar _
        | SynExpr.Ident _ -> continuation []
        | SynExpr.LongIdent(longDotId = longDotId) -> continuation (visitSynLongIdent longDotId)
        | SynExpr.LongIdentSet(longDotId, expr, _) -> visit expr (fun nodes -> visitSynLongIdent longDotId @ nodes |> continuation)
        | SynExpr.DotGet(expr = expr; longDotId = longDotId) ->
            visit expr (fun nodes -> visitSynLongIdent longDotId @ nodes |> continuation)
        | SynExpr.DotSet(targetExpr, longDotId, rhsExpr, _) ->
            visit targetExpr (fun tNodes ->
                visit rhsExpr (fun rNodes ->
                    [ yield! tNodes; yield! visitSynLongIdent longDotId; yield! rNodes ]
                    |> continuation))
        | SynExpr.Set(targetExpr, rhsExpr, _) ->
            let continuations = List.map visit [ targetExpr; rhsExpr ]
            Continuation.concatenate continuations continuation
        | SynExpr.DotIndexedGet(objectExpr, indexArgs, _, _) ->
            let continuations = List.map visit [ objectExpr; indexArgs ]
            Continuation.concatenate continuations continuation
        | SynExpr.DotIndexedSet(objectExpr, indexArgs, valueExpr, _, _, _) ->
            let continuations = List.map visit [ objectExpr; indexArgs; valueExpr ]
            Continuation.concatenate continuations continuation
        | SynExpr.NamedIndexedPropertySet(longDotId, expr1, expr2, _) ->
            visit expr1 (fun nodes1 ->
                visit expr2 (fun nodes2 ->
                    [ yield! visitSynLongIdent longDotId; yield! nodes1; yield! nodes2 ]
                    |> continuation))
        | SynExpr.DotNamedIndexedPropertySet(targetExpr, longDotId, argExpr, rhsExpr, _) ->
            let continuations = List.map visit [ targetExpr; argExpr; rhsExpr ]

            let finalContinuation nodes =
                visitSynLongIdent longDotId @ List.concat nodes |> continuation

            Continuation.sequence continuations finalContinuation
        | SynExpr.TypeTest(expr, targetType, _) -> visit expr (fun nodes -> nodes @ visitSynType targetType |> continuation)
        | SynExpr.Upcast(expr, targetType, _) -> visit expr (fun nodes -> nodes @ visitSynType targetType |> continuation)
        | SynExpr.Downcast(expr, targetType, _) -> visit expr (fun nodes -> nodes @ visitSynType targetType |> continuation)
        | SynExpr.InferredUpcast(expr, _) -> visit expr continuation
        | SynExpr.InferredDowncast(expr, _) -> visit expr continuation
        | SynExpr.Null _ -> continuation []
        | SynExpr.AddressOf(expr = expr) -> visit expr continuation
        | SynExpr.TraitCall(supportTys, traitSig, argExpr, _) ->
            visit argExpr (fun nodes ->
                [
                    yield! visitSynType supportTys
                    yield! visitSynMemberSig traitSig
                    yield! nodes
                ]
                |> continuation)
        | SynExpr.JoinIn(lhsExpr, _, rhsExpr, _) ->
            let continuations = List.map visit [ lhsExpr; rhsExpr ]
            Continuation.concatenate continuations continuation
        | SynExpr.ImplicitZero _ -> continuation []
        | SynExpr.SequentialOrImplicitYield(_, expr1, expr2, _, _) ->
            let continuations = List.map visit [ expr1; expr2 ]
            Continuation.concatenate continuations continuation
        | SynExpr.YieldOrReturn(expr = expr) -> visit expr continuation
        | SynExpr.YieldOrReturnFrom(expr = expr) -> visit expr continuation
        | SynExpr.LetOrUseBang(pat = pat; rhs = rhs; andBangs = andBangs; body = body) ->
            let continuations =
                let andBangExprs = List.map (fun (SynExprAndBang(body = body)) -> body) andBangs
                List.map visit (body :: rhs :: andBangExprs)

            let finalContinuation nodes =
                [
                    yield! List.concat nodes
                    yield! visitPat pat
                    for SynExprAndBang(pat = pat) in andBangs do
                        yield! visitPat pat
                ]
                |> continuation

            Continuation.sequence continuations finalContinuation
        | SynExpr.MatchBang(expr = expr; clauses = clauses) ->
            visit expr (fun exprNodes ->
                [ yield! exprNodes; yield! List.collect visitSynMatchClause clauses ]
                |> continuation)
        | SynExpr.DoBang(expr = expr) -> visit expr continuation
        | SynExpr.WhileBang(whileExpr = whileExpr; doExpr = doExpr) ->
            visit whileExpr (fun whileNodes -> visit doExpr (fun doNodes -> whileNodes @ doNodes |> continuation))
        | SynExpr.LibraryOnlyILAssembly(typeArgs = typeArgs; args = args; retTy = retTy) ->
            let typeNodes = List.collect visitSynType (typeArgs @ retTy)
            let continuations = List.map visit args

            let finalContinuation nodes =
                List.concat nodes @ typeNodes |> continuation

            Continuation.sequence continuations finalContinuation
        | SynExpr.LibraryOnlyStaticOptimization(constraints, expr, optimizedExpr, _) ->
            let constraintTypes =
                constraints
                |> List.choose (function
                    | SynStaticOptimizationConstraint.WhenTyparTyconEqualsTycon(rhsType = t) -> Some t
                    | SynStaticOptimizationConstraint.WhenTyparIsStruct _ -> None)

            visit expr (fun eNodes ->
                visit optimizedExpr (fun oNodes ->
                    [
                        yield! List.collect visitSynType constraintTypes
                        yield! eNodes
                        yield! oNodes
                    ]
                    |> continuation))
        | SynExpr.LibraryOnlyUnionCaseFieldGet(expr, longId, _, _) ->
            visit expr (fun eNodes -> visitLongIdent longId @ eNodes |> continuation)
        | SynExpr.LibraryOnlyUnionCaseFieldSet(expr, longId, _, rhsExpr, _) ->
            visit expr (fun eNodes ->
                visit rhsExpr (fun rhsNodes -> [ yield! visitLongIdent longId; yield! eNodes; yield! rhsNodes ] |> continuation))
        | SynExpr.ArbitraryAfterError _ -> continuation []
        | SynExpr.FromParseError _ -> continuation []
        | SynExpr.DiscardAfterMissingQualificationAfterDot _ -> continuation []
        | SynExpr.Fixed(expr, _) -> visit expr continuation
        | SynExpr.InterpolatedString(contents = contents) ->
            let continuations =
                List.map
                    visit
                    (List.choose
                        (function
                        | SynInterpolatedStringPart.FillExpr(fillExpr = e) -> Some e
                        | SynInterpolatedStringPart.String _ -> None)
                        contents)

            Continuation.concatenate continuations continuation
        | SynExpr.DebugPoint _ -> continuation []
        | SynExpr.Dynamic(funcExpr, _, argExpr, _) ->
            let continuations = List.map visit [ funcExpr; argExpr ]
            Continuation.concatenate continuations continuation

    visit e id

/// Special case of `| nameof Module ->` type of pattern
[<return: Struct>]
let (|NameofPat|_|) (pat: SynPat) =
    let rec stripPats p =
        match p with
        | SynPat.Paren(pat = pat) -> stripPats pat
        | _ -> p

    match pat with
    | SynPat.LongIdent(longDotId = SynLongIdent(id = [ NameofIdent ]); typarDecls = None; argPats = SynArgPats.Pats [ moduleNamePat ]) ->
        match stripPats moduleNamePat with
        | SynPat.LongIdent(
            longDotId = SynLongIdent.SynLongIdent(id = longIdent)
            extraId = None
            typarDecls = None
            argPats = SynArgPats.Pats []
            accessibility = None) ->
            match longIdent with
            | [] -> ValueNone
            | [ moduleNameIdent ] -> ValueSome(NameofResult.SingleIdent moduleNameIdent)
            | lid -> ValueSome(NameofResult.LongIdent lid)
        | _ -> ValueNone
    | _ -> ValueNone

let visitPat (p: SynPat) : FileContentEntry list =
    let rec visit (p: SynPat) (continuation: FileContentEntry list -> FileContentEntry list) : FileContentEntry list =
        match p with
        | NameofPat moduleNameIdent -> continuation [ visitNameofResult moduleNameIdent ]
        | SynPat.Paren(pat = pat) -> visit pat continuation
        | SynPat.Typed(pat = pat; targetType = t) -> visit pat (fun nodes -> nodes @ visitSynType t |> continuation)
        | SynPat.Const _ -> continuation []
        | SynPat.Wild _ -> continuation []
        | SynPat.Named _ -> continuation []
        | SynPat.Attrib(pat, attributes, _) -> visit pat (fun nodes -> visitSynAttributes attributes @ nodes |> continuation)
        | SynPat.Or(lhsPat, rhsPat, _, _) ->
            let continuations = List.map visit [ lhsPat; rhsPat ]
            Continuation.concatenate continuations continuation
        | SynPat.ListCons(lhsPat, rhsPat, _, _) ->
            let continuations = List.map visit [ lhsPat; rhsPat ]
            Continuation.concatenate continuations continuation
        | SynPat.Ands(pats, _) ->
            let continuations = List.map visit pats
            Continuation.concatenate continuations continuation
        | SynPat.As(lhsPat, rhsPat, _) ->
            let continuations = List.map visit [ lhsPat; rhsPat ]
            Continuation.concatenate continuations continuation
        | SynPat.LongIdent(longDotId = longDotId; typarDecls = typarDecls; argPats = argPats) ->
            continuation
                [
                    yield! visitSynLongIdent longDotId
                    yield! collectFromOption visitSynValTyparDecls typarDecls
                    yield! visitSynArgPats argPats
                ]
        | SynPat.Tuple(elementPats = elementPats) ->
            let continuations = List.map visit elementPats
            Continuation.concatenate continuations continuation
        | SynPat.ArrayOrList(_, elementPats, _) ->
            let continuations = List.map visit elementPats
            Continuation.concatenate continuations continuation
        | SynPat.Record(fieldPats, _) ->
            let pats = List.map (fun (_, _, p) -> p) fieldPats

            let lids =
                [
                    for (l, _), _, _ in fieldPats do
                        yield! visitLongIdent l
                ]

            let continuations = List.map visit pats

            let finalContinuation nodes =
                [ yield! List.concat nodes; yield! lids ] |> continuation

            Continuation.sequence continuations finalContinuation
        | SynPat.Null _ -> continuation []
        | SynPat.OptionalVal _ -> continuation []
        | SynPat.IsInst(t, _) -> continuation (visitSynType t)
        | SynPat.QuoteExpr(expr, _) -> continuation (visitSynExpr expr)
        | SynPat.InstanceMember _ -> continuation []
        | SynPat.FromParseError _ -> continuation []

    visit p id

let visitSynArgPats (argPat: SynArgPats) =
    match argPat with
    | SynArgPats.Pats args -> List.collect visitPat args
    | SynArgPats.NamePatPairs(pats = pats) ->
        [
            for _, _, p in pats do
                yield! visitPat p
        ]

let visitSynSimplePat (pat: SynSimplePat) =
    match pat with
    | SynSimplePat.Id _ -> []
    | SynSimplePat.Attrib(pat, attributes, _) -> visitSynSimplePat pat @ visitSynAttributes attributes
    | SynSimplePat.Typed(pat, t, _) -> visitSynSimplePat pat @ visitSynType t

let visitSynSimplePats (pats: SynSimplePats) =
    match pats with
    | SynSimplePats.SimplePats(pats = pats) -> List.collect visitSynSimplePat pats

let visitSynMatchClause (SynMatchClause(pat = pat; whenExpr = whenExpr; resultExpr = resultExpr)) =
    [
        yield! visitPat pat
        yield! collectFromOption visitSynExpr whenExpr
        yield! visitSynExpr resultExpr
    ]

let visitBinding (SynBinding(attributes = attributes; headPat = headPat; returnInfo = returnInfo; expr = expr)) : FileContentEntry list =
    [
        yield! visitSynAttributes attributes
        match headPat with
        | SynPat.LongIdent(argPats = SynArgPats.Pats pats) -> yield! List.collect visitPat pats
        | _ -> yield! visitPat headPat
        yield! collectFromOption visitSynBindingReturnInfo returnInfo
        yield! visitSynExpr expr
    ]

let visitSynBindingReturnInfo (SynBindingReturnInfo(typeName = typeName; attributes = attributes)) =
    visitSynAttributes attributes @ visitSynType typeName

let visitSynMemberSig (ms: SynMemberSig) : FileContentEntry list =
    match ms with
    | SynMemberSig.Member(memberSig = memberSig) -> visitSynValSig memberSig
    | SynMemberSig.Interface(interfaceType, _) -> visitSynType interfaceType
    | SynMemberSig.Inherit(inheritedType, _) -> visitSynType inheritedType
    | SynMemberSig.ValField(field, _) -> visitSynField field
    | SynMemberSig.NestedType _ -> []

let mkFileContent (f: FileInProject) : FileContentEntry list =
    [
        match f.ParsedInput with
        | ParsedInput.SigFile(ParsedSigFileInput(contents = contents)) ->
            for SynModuleOrNamespaceSig(longId = longId; kind = kind; decls = decls; attribs = attribs) in contents do
                yield! List.collect visitSynAttributeList attribs

                match kind with
                | SynModuleOrNamespaceKind.GlobalNamespace
                | SynModuleOrNamespaceKind.AnonModule -> yield! List.collect visitSynModuleSigDecl decls
                | SynModuleOrNamespaceKind.DeclaredNamespace ->
                    let path = longIdentToPath false longId
                    yield FileContentEntry.TopLevelNamespace(path, List.collect visitSynModuleSigDecl decls)
                | SynModuleOrNamespaceKind.NamedModule ->
                    let path = longIdentToPath true longId
                    yield FileContentEntry.TopLevelNamespace(path, List.collect visitSynModuleSigDecl decls)
        | ParsedInput.ImplFile(ParsedImplFileInput(contents = contents)) ->
            for SynModuleOrNamespace(longId = longId; attribs = attribs; kind = kind; decls = decls) in contents do
                yield! List.collect visitSynAttributeList attribs

                match kind with
                | SynModuleOrNamespaceKind.GlobalNamespace
                | SynModuleOrNamespaceKind.AnonModule -> yield! List.collect visitSynModuleDecl decls
                | SynModuleOrNamespaceKind.DeclaredNamespace ->
                    let path = longIdentToPath false longId
                    yield FileContentEntry.TopLevelNamespace(path, List.collect visitSynModuleDecl decls)
                | SynModuleOrNamespaceKind.NamedModule ->
                    let path = longIdentToPath true longId
                    yield FileContentEntry.TopLevelNamespace(path, List.collect visitSynModuleDecl decls)
    ]
