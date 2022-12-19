module rec FSharp.Compiler.Service.Service.FindAllIdentifiers

open System.Collections.Generic
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open FSharp.Compiler.SyntaxTreeOps
//
// module Continuation =
//     let rec sequence<'a, 'ret> (recursions: (('a -> 'ret) -> 'ret) list) (finalContinuation: 'a list -> 'ret) : 'ret =
//         match recursions with
//         | [] -> [] |> finalContinuation
//         | recurse :: recurses -> recurse (fun ret -> sequence recurses (fun rets -> ret :: rets |> finalContinuation))

type AddItem = string -> unit

let visitAll f g items = Seq.iter (f g) items
let visitOpt f g item = Option.iter (f g) item

let longIdentToPath (skipLast: bool) (longId: LongIdent) =
    if skipLast then
        List.take (longId.Length - 1) longId
    else
        longId
    |> List.map (fun ident -> ident.idText)

let synLongIdentToPath (skipLast: bool) (synLongIdent: SynLongIdent) =
    longIdentToPath skipLast synLongIdent.LongIdent

let visitIdent (addItem: AddItem) (ident: Ident) = addItem ident.idText

let visitIdentTrivia (addItem: AddItem) =
    function
    | IdentTrivia.OriginalNotation (text = text)
    | IdentTrivia.OriginalNotationWithParen (text = text) -> addItem text
    | IdentTrivia.HasParenthesis _ -> ()

let visitSynIdent (addItem: AddItem) (SynIdent (ident, trivia)) =
    match trivia with
    | None -> visitIdent addItem ident
    | Some identTrivia -> visitIdentTrivia addItem identTrivia

let visitSynLongIdent (addItem: AddItem) (lid: SynLongIdent) =
    List.iter (visitSynIdent addItem) lid.IdentsWithTrivia

let visitLongIdent (addItem: AddItem) (lid: LongIdent) = List.iter (visitIdent addItem) lid

let visitSynAttribute (addItem: AddItem) (a: SynAttribute) =
    visitSynLongIdent addItem a.TypeName
    visitSynExpr a.ArgExpr addItem

let visitSynAttributeList (addItem: AddItem) (attributes: SynAttributeList) =
    for a in attributes.Attributes do
        visitSynAttribute addItem a

let visitSynAttributes (addItem: AddItem) (attributes: SynAttributes) =
    for a in attributes do
        visitSynAttributeList addItem a

let visitSynComponentInfo (addItem: AddItem) (SynComponentInfo (attributes, typeParams, constraints, longId, _, _, _, _)) =
    visitSynAttributes addItem attributes
    visitOpt visitSynTyparDecls addItem typeParams
    visitAll visitSynTypeConstraint addItem constraints
    visitLongIdent addItem longId

let visitParsedHashDirectiveArgument (addItem: AddItem) =
    function
    | ParsedHashDirectiveArgument.String (value = v) -> addItem v
    | ParsedHashDirectiveArgument.SourceIdentifier _ -> () // I guess

let visitParsedHashDirective (addItem: AddItem) (ParsedHashDirective (ident, args, _)) =
    addItem ident
    visitAll visitParsedHashDirectiveArgument addItem args

let visitSynModuleDecl (addItem: AddItem) (decl: SynModuleDecl) =
    match decl with
    | SynModuleDecl.Open(target = SynOpenDeclTarget.ModuleOrNamespace (longId, _)) -> visitSynLongIdent addItem longId
    | SynModuleDecl.Open(target = SynOpenDeclTarget.Type (typeName, _)) -> visitSynType typeName addItem
    | SynModuleDecl.Attributes (attributes, _) -> visitAll visitSynAttributeList addItem attributes
    | SynModuleDecl.Expr (expr, _) -> visitSynExpr expr addItem
    | SynModuleDecl.NestedModule (moduleInfo = moduleInfo; decls = decls) ->
        visitSynComponentInfo addItem moduleInfo
        visitAll visitSynModuleDecl addItem decls
    | SynModuleDecl.Let (bindings = bindings) -> visitAll visitBinding addItem bindings
    | SynModuleDecl.Types (typeDefns = typeDefns) -> visitAll visitSynTypeDefn addItem typeDefns
    | SynModuleDecl.HashDirective (hashDirective = hashDirective) -> visitParsedHashDirective addItem hashDirective
    | SynModuleDecl.ModuleAbbrev (ident = ident; longId = longId) ->
        visitIdent addItem ident
        visitLongIdent addItem longId
    | SynModuleDecl.NamespaceFragment _ -> ()
    | SynModuleDecl.Exception(exnDefn = SynExceptionDefn (exnRepr = SynExceptionDefnRepr (attributes = attributes
                                                                                          caseName = caseName
                                                                                          longId = longId)
                                                          members = members)) ->
        visitSynAttributes addItem attributes
        visitSynUnionCase addItem caseName
        visitOpt visitLongIdent addItem longId
        visitAll visitSynMemberDefn addItem members

let visitSynModuleSigDecl (addItem: AddItem) (md: SynModuleSigDecl) =
    match md with
    | SynModuleSigDecl.Open(target = SynOpenDeclTarget.ModuleOrNamespace (longId, _)) -> visitSynLongIdent addItem longId
    | SynModuleSigDecl.Open(target = SynOpenDeclTarget.Type (typeName, _)) -> visitSynType typeName addItem
    | SynModuleSigDecl.NestedModule (moduleInfo = componentInfo; moduleDecls = decls) ->
        visitSynComponentInfo addItem componentInfo
        visitAll visitSynModuleSigDecl addItem decls
    | SynModuleSigDecl.ModuleAbbrev (ident = ident; longId = longId) ->
        visitIdent addItem ident
        visitLongIdent addItem longId
    | SynModuleSigDecl.Val (valSig, _) -> visitSynValSig addItem valSig
    | SynModuleSigDecl.Types (types = types) -> visitAll visitSynTypeDefnSig addItem types
    | SynModuleSigDecl.Exception(exnSig = SynExceptionSig (exnRepr = SynExceptionDefnRepr (attributes = attributes
                                                                                           caseName = caseName
                                                                                           longId = longId)
                                                           members = members)) ->
        visitSynAttributes addItem attributes
        visitSynUnionCase addItem caseName
        visitOpt visitLongIdent addItem longId
        visitAll visitSynMemberSig addItem members
    | SynModuleSigDecl.HashDirective (hashDirective, _) -> visitParsedHashDirective addItem hashDirective
    | SynModuleSigDecl.NamespaceFragment _ -> ()

let visitSynUnionCase (addItem: AddItem) (SynUnionCase (attributes = attributes; ident = ident; caseType = caseType)) =
    match caseType with
    | SynUnionCaseKind.Fields fields -> visitAll visitSynField addItem fields
    | SynUnionCaseKind.FullType (fullType = fullType) -> visitSynType fullType addItem

    visitSynAttributes addItem attributes
    visitSynIdent addItem ident

let visitSynEnumCase (addItem: AddItem) (SynEnumCase (ident = ident; attributes = attributes)) =
    visitSynAttributes addItem attributes
    visitSynIdent addItem ident

let visitSynTypeDefn (addItem: AddItem) (SynTypeDefn (typeInfo = componentInfo; typeRepr = typeRepr; members = members)) =
    visitSynComponentInfo addItem componentInfo
    visitAll visitSynMemberDefn addItem members

    match typeRepr with
    | SynTypeDefnRepr.Simple (simpleRepr, _) ->
        match simpleRepr with
        | SynTypeDefnSimpleRepr.Union (unionCases = unionCases) -> visitAll visitSynUnionCase addItem unionCases
        | SynTypeDefnSimpleRepr.Enum (cases = cases) -> visitAll visitSynEnumCase addItem cases
        | SynTypeDefnSimpleRepr.Record (recordFields = recordFields) -> visitAll visitSynField addItem recordFields
        // This is only used in the typed tree
        // The parser doesn't construct this
        | SynTypeDefnSimpleRepr.General _ -> ()
        | SynTypeDefnSimpleRepr.LibraryOnlyILAssembly _ -> ()
        | SynTypeDefnSimpleRepr.TypeAbbrev (rhsType = rhsType) -> visitSynType rhsType addItem
        | SynTypeDefnSimpleRepr.None _ -> ()
        // This is only used in the typed tree
        // The parser doesn't construct this
        | SynTypeDefnSimpleRepr.Exception _ -> ()
    | SynTypeDefnRepr.ObjectModel (kind, members, _) ->
        match kind with
        | SynTypeDefnKind.Delegate (signature, _) -> visitSynType signature addItem
        | _ -> ()

        visitAll visitSynMemberDefn addItem members
    | SynTypeDefnRepr.Exception _ ->
        // This is only used in the typed tree
        // The parser doesn't construct this
        ()

let visitSynTypeDefnSig (addItem: AddItem) (SynTypeDefnSig (typeInfo = componentInfo; typeRepr = typeRepr; members = members)) =
    visitSynComponentInfo addItem componentInfo
    visitAll visitSynMemberSig addItem members

    match typeRepr with
    | SynTypeDefnSigRepr.Simple (simpleRepr, _) ->
        match simpleRepr with
        | SynTypeDefnSimpleRepr.Union (unionCases = unionCases) -> visitAll visitSynUnionCase addItem unionCases
        | SynTypeDefnSimpleRepr.Enum (cases = cases) -> visitAll visitSynEnumCase addItem cases
        | SynTypeDefnSimpleRepr.Record (recordFields = recordFields) -> visitAll visitSynField addItem recordFields
        // This is only used in the typed tree
        // The parser doesn't construct this
        | SynTypeDefnSimpleRepr.General _ -> ()
        | SynTypeDefnSimpleRepr.LibraryOnlyILAssembly _ -> ()
        | SynTypeDefnSimpleRepr.TypeAbbrev (rhsType = rhsType) -> visitSynType rhsType addItem
        | SynTypeDefnSimpleRepr.None _ -> ()
        // This is only used in the typed tree
        // The parser doesn't construct this
        | SynTypeDefnSimpleRepr.Exception _ -> ()
    | SynTypeDefnSigRepr.ObjectModel (kind, members, _) ->
        match kind with
        | SynTypeDefnKind.Delegate (signature, _) -> visitSynType signature addItem
        | _ -> ()

        visitAll visitSynMemberSig addItem members
    | SynTypeDefnSigRepr.Exception _ ->
        // This is only used in the typed tree
        // The parser doesn't construct this
        ()

let visitSynValSig (addItem: AddItem) (SynValSig (attributes = attributes; ident = ident; synType = synType; synExpr = synExpr)) =
    visitSynIdent addItem ident
    visitSynAttributes addItem attributes
    visitSynType synType addItem
    Option.iter (fun e -> visitSynExpr e addItem) synExpr

let visitSynField (addItem: AddItem) (SynField (attributes = attributes; idOpt = idOpt; fieldType = fieldType)) =
    visitSynAttributes addItem attributes
    visitOpt visitIdent addItem idOpt
    visitSynType fieldType addItem

let visitSynMemberDefn (addItem: AddItem) (md: SynMemberDefn) =
    match md with
    | SynMemberDefn.Member (memberDefn = binding) -> visitBinding addItem binding
    | SynMemberDefn.Open _ -> ()
    | SynMemberDefn.GetSetMember (memberDefnForGet, memberDefnForSet, _, _) ->
        visitOpt visitBinding addItem memberDefnForGet
        visitOpt visitBinding addItem memberDefnForSet
    | SynMemberDefn.ImplicitCtor (attributes = attributes; ctorArgs = ctorArgs; selfIdentifier = selfIdentifier) ->
        visitSynSimplePats ctorArgs addItem
        visitSynAttributes addItem attributes
        visitOpt visitIdent addItem selfIdentifier
    | SynMemberDefn.ImplicitInherit (inheritType, inheritArgs, inheritAlias, _) ->
        visitSynType inheritType addItem
        visitSynExpr inheritArgs addItem
        visitOpt visitIdent addItem inheritAlias
    | SynMemberDefn.LetBindings (bindings = bindings) -> visitAll visitBinding addItem bindings
    | SynMemberDefn.AbstractSlot (slotSig = slotSig) -> visitSynValSig addItem slotSig
    | SynMemberDefn.Interface (interfaceType, _, members, _) ->
        visitSynType interfaceType addItem

        match members with
        | None -> ()
        | Some members -> visitAll visitSynMemberDefn addItem members
    | SynMemberDefn.Inherit (baseType, asIdent, _) ->
        visitSynType baseType addItem
        visitOpt visitIdent addItem asIdent
    | SynMemberDefn.ValField (fieldInfo, _) -> visitSynField addItem fieldInfo
    | SynMemberDefn.NestedType _ -> ()
    | SynMemberDefn.AutoProperty (attributes = attributes; ident = ident; typeOpt = typeOpt; synExpr = synExpr) ->
        visitIdent addItem ident
        visitSynAttributes addItem attributes
        Option.iter (fun t -> visitSynType t addItem) typeOpt
        visitSynExpr synExpr addItem

let visitSynInterfaceImpl (addItem: AddItem) (SynInterfaceImpl (interfaceTy = t; bindings = bindings; members = members)) =
    visitSynType t addItem
    visitAll visitBinding addItem bindings
    visitAll visitSynMemberDefn addItem members

let visitSynTypes (ts: SynType list) (addItem: AddItem) =
    for t in ts do
        visitSynType t addItem

let rec visitSynType (t: SynType) (addItem: AddItem) =
    match t with
    | SynType.LongIdent lid -> visitSynLongIdent addItem lid
    | SynType.App (typeName = typeName; typeArgs = typeArgs) ->
        visitSynTypes typeArgs addItem
        visitSynType typeName addItem
    // let continuations = List.iter visitSynType (typeName :: typeArgs)
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynType.LongIdentApp (typeName = typeName; longDotId = longDotId; typeArgs = typeArgs) ->
        visitSynLongIdent addItem longDotId
        visitSynTypes typeArgs addItem
        visitSynType typeName addItem
    // let continuations = List.map visitSynType (typeName :: typeArgs)
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynType.Tuple (path = path) -> visitSynTypes (getTypeFromTuplePath path) addItem
    // let continuations = List.map visitSynType (getTypeFromTuplePath path)
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynType.AnonRecd (fields = fields) ->
        for _, field in fields do
            visitSynType field addItem
    // let continuations = List.map (snd >> visitSynType) fields
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynType.Array (elementType = elementType) -> visitSynType elementType addItem
    | SynType.Fun (argType, returnType, _, _) ->
        visitSynType argType addItem
        visitSynType returnType addItem
    // let continuations = List.map visitSynType [ argType; returnType ]
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynType.Var (typar = typar) -> visitSynTypar addItem typar
    | SynType.Anon _ -> ()
    | SynType.WithGlobalConstraints (typeName, constraints, _) ->
        visitAll visitSynTypeConstraint addItem constraints
        visitSynType typeName addItem
    | SynType.HashConstraint (innerType, _) -> visitSynType innerType addItem
    | SynType.MeasurePower (baseMeasure = baseMeasure) -> visitSynType baseMeasure addItem
    | SynType.StaticConstant _ -> ()
    | SynType.StaticConstantExpr (expr, _) -> visitSynExpr expr addItem
    | SynType.StaticConstantNamed (ident, value, _) ->
        visitSynType ident addItem
        visitSynType value addItem
    // let continuations = List.map visitSynType [ ident; value ]
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynType.Paren (innerType, _) -> visitSynType innerType addItem
    | SynType.SignatureParameter (attributes = attributes; usedType = usedType; id = ident) ->
        Option.iter (visitIdent addItem) ident
        visitSynAttributes addItem attributes
        visitSynType usedType addItem
    | SynType.Or (lhsType, rhsType, _, _) ->
        visitSynType lhsType addItem
        visitSynType rhsType addItem
// let continuations = List.map visitSynType [ lhsType; rhsType ]
// let finalContinuation = List.iter addItem
// Continuation.sequence continuations finalContinuation

let visitSynTypar (addItem: AddItem) (SynTypar (ident = ident)) = visitIdent addItem ident

let visitSynValTyparDecls (addItem: AddItem) (SynValTyparDecls (typars = typars)) =
    visitOpt visitSynTyparDecls addItem typars

let visitSynTyparDecls (addItem: AddItem) (td: SynTyparDecls) =
    match td with
    | SynTyparDecls.PostfixList (decls, constraints, _) ->
        visitAll visitSynTyparDecl addItem decls
        visitAll visitSynTypeConstraint addItem constraints
    | SynTyparDecls.PrefixList (decls = decls) -> visitAll visitSynTyparDecl addItem decls
    | SynTyparDecls.SinglePrefix (decl = decl) -> visitSynTyparDecl addItem decl

let visitSynTyparDecl (addItem: AddItem) (SynTyparDecl (attributes, typar)) =
    visitSynAttributes addItem attributes
    visitSynTypar addItem typar

let visitSynTypeConstraint (addItem: AddItem) (tc: SynTypeConstraint) =
    match tc with
    | SynTypeConstraint.WhereSelfConstrained (selfConstraint = selfConstraint) -> visitSynType selfConstraint addItem
    | SynTypeConstraint.WhereTyparIsValueType (typar = typar)
    | SynTypeConstraint.WhereTyparIsReferenceType (typar = typar)
    | SynTypeConstraint.WhereTyparIsUnmanaged (typar = typar)
    | SynTypeConstraint.WhereTyparSupportsNull (typar = typar)
    | SynTypeConstraint.WhereTyparIsComparable (typar = typar)
    | SynTypeConstraint.WhereTyparIsEquatable (typar = typar) -> visitSynTypar addItem typar
    | SynTypeConstraint.WhereTyparDefaultsToType (typar = typar; typeName = typeName)
    | SynTypeConstraint.WhereTyparSubtypeOfType (typar = typar; typeName = typeName) ->
        visitSynType typeName addItem
        visitSynTypar addItem typar
    | SynTypeConstraint.WhereTyparSupportsMember (typars, memberSig, _) ->
        visitSynType typars addItem
        visitSynMemberSig addItem memberSig
    | SynTypeConstraint.WhereTyparIsEnum (typar = typar; typeArgs = typeArgs)
    | SynTypeConstraint.WhereTyparIsDelegate (typar = typar; typeArgs = typeArgs) ->
        List.iter (fun t -> visitSynType t addItem) typeArgs
        visitSynTypar addItem typar

let visitSynExprs (es: SynExpr list) (addItem: AddItem) =
    for e in es do
        visitSynExpr e addItem

let rec visitSynExpr (e: SynExpr) (addItem: AddItem) =
    match e with
    | SynExpr.Const _ -> ()
    | SynExpr.Paren (expr = expr) -> visitSynExpr expr addItem
    | SynExpr.Quote (operator = operator; quotedExpr = quotedExpr) ->
        visitSynExpr operator (fun operator ->
            addItem operator
            visitSynExpr quotedExpr addItem)
    | SynExpr.Typed (expr, targetType, _) ->
        visitSynType targetType addItem
        visitSynExpr expr addItem
    | SynExpr.Tuple (exprs = exprs) -> visitSynExprs exprs addItem
    // let continuations = List.map visitSynExpr exprs
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynExpr.AnonRecd (copyInfo = copyInfo; recordFields = recordFields) ->
        let exprs =
            match copyInfo with
            | None -> List.map (fun (_, _, e) -> e) recordFields
            | Some (cp, _) -> cp :: List.map (fun (_, _, e) -> e) recordFields

        visitSynExprs exprs addItem
    // let continuations =
    //     match copyInfo with
    //     | None -> List.map (fun (_, _, e) -> visitSynExpr e) recordFields
    //     | Some (cp, _) -> visitSynExpr cp :: List.map (fun (_, _, e) -> visitSynExpr e) recordFields
    //
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynExpr.ArrayOrList _ -> ()
    | SynExpr.Record (baseInfo = baseInfo; copyInfo = copyInfo; recordFields = recordFields) ->
        let es =
            [
                yield!
                    List.choose
                        (fun (SynExprRecordField (fieldName = (si, _); expr = expr)) ->
                            visitSynLongIdent addItem si
                            expr)
                        recordFields
                match baseInfo with
                | None -> ()
                | Some (t, e, _, _, _) ->
                    visitSynType t addItem
                    yield e
                match copyInfo with
                | None -> ()
                | Some (e, _) -> yield e
            ]

        visitSynExprs es addItem
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation

    // let continuations =
    //     [
    //         yield!
    //             List.choose
    //                 (fun (SynExprRecordField (fieldName = (si, _); expr = expr)) ->
    //                     visitSynLongIdent addItem si
    //                     Option.map visitSynExpr expr)
    //                 recordFields
    //         match baseInfo with
    //         | None -> ()
    //         | Some (t, e, _, _, _) ->
    //             visitSynType t addItem
    //             yield visitSynExpr e
    //         match copyInfo with
    //         | None -> ()
    //         | Some (e, _) -> yield visitSynExpr e
    //     ]
    //
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynExpr.New (targetType = targetType; expr = expr) ->
        visitSynType targetType addItem
        visitSynExpr expr addItem
    | SynExpr.ObjExpr (objType, argOptions, _, bindings, members, extraImpls, _, _) ->
        visitSynType objType addItem
        visitAll visitBinding addItem bindings
        visitAll visitSynMemberDefn addItem members
        visitAll visitSynInterfaceImpl addItem extraImpls

        match argOptions with
        | None -> ()
        | Some (e, i) ->
            visitOpt visitIdent addItem i
            visitSynExpr e addItem
    | SynExpr.While (whileExpr = whileExpr; doExpr = doExpr) ->
        visitSynExpr whileExpr addItem
        visitSynExpr doExpr addItem
    // let continuations = [ visitSynExpr whileExpr; visitSynExpr doExpr ]
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynExpr.For (ident = ident; identBody = identBody; toBody = toBody; doBody = doBody) ->
        visitIdent addItem ident
        visitSynExpr identBody addItem
        visitSynExpr toBody addItem
        visitSynExpr doBody addItem
    // let continuations = List.map visitSynExpr [ identBody; toBody; doBody ]
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynExpr.ForEach (pat = pat; enumExpr = enumExpr; bodyExpr = bodyExpr) ->
        visitPat pat addItem
        visitSynExpr enumExpr addItem
        visitSynExpr bodyExpr addItem
    // let continuations = [ visitSynExpr enumExpr; visitSynExpr bodyExpr ]
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynExpr.ArrayOrListComputed (expr = expr) -> visitSynExpr expr addItem
    | SynExpr.IndexRange (expr1 = expr1; expr2 = expr2) ->
        Option.iter (fun e -> visitSynExpr e addItem) expr1
        Option.iter (fun e -> visitSynExpr e addItem) expr2

    // let continuations =
    //     [
    //         match expr1 with
    //         | None -> ()
    //         | Some e -> yield visitSynExpr e
    //         match expr2 with
    //         | None -> ()
    //         | Some e -> yield visitSynExpr e
    //     ]
    //
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynExpr.IndexFromEnd (expr, _) -> visitSynExpr expr addItem
    | SynExpr.ComputationExpr (expr = expr) -> visitSynExpr expr addItem
    | SynExpr.Lambda (args = args; body = body) ->
        visitSynSimplePats args addItem
        visitSynExpr body addItem
    | SynExpr.MatchLambda (matchClauses = clauses) -> visitAll visitSynMatchClause addItem clauses
    | SynExpr.Match (expr = expr; clauses = clauses)
    | SynExpr.MatchBang (expr = expr; clauses = clauses) ->
        visitAll visitSynMatchClause addItem clauses
        visitSynExpr expr addItem
    | SynExpr.Do (expr, _) -> visitSynExpr expr addItem
    | SynExpr.Assert (expr, _) -> visitSynExpr expr addItem
    | SynExpr.App (funcExpr = funcExpr; argExpr = argExpr) -> visitSynExprs [ funcExpr; argExpr ] addItem
    // let continuations = [ visitSynExpr funcExpr; visitSynExpr argExpr ]
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynExpr.TypeApp (expr = expr; typeArgs = typeArgs) ->
        List.iter (fun t -> visitSynType t addItem) typeArgs
        visitSynExpr expr addItem
    | SynExpr.LetOrUse (bindings = bindings; body = body) ->
        visitAll visitBinding addItem bindings
        visitSynExpr body addItem
    | SynExpr.TryWith (tryExpr = tryExpr; withCases = withCases) ->
        visitAll visitSynMatchClause addItem withCases
        visitSynExpr tryExpr addItem
    | SynExpr.TryFinally (tryExpr = tryExpr; finallyExpr = finallyExpr) -> visitSynExprs [ tryExpr; finallyExpr ] addItem
    // let continuations = [ visitSynExpr tryExpr; visitSynExpr finallyExpr ]
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynExpr.Lazy (expr, _) -> visitSynExpr expr addItem
    | SynExpr.Sequential (expr1 = expr1; expr2 = expr2) -> visitSynExprs [ expr1; expr2 ] addItem
    // let continuations = [ visitSynExpr expr1; visitSynExpr expr2 ]
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynExpr.IfThenElse (ifExpr = ifExpr; thenExpr = thenExpr; elseExpr = elseExpr) ->
        visitSynExprs (ifExpr :: thenExpr :: Option.toList elseExpr) addItem
    // let continuations =
    //     List.map visitSynExpr (ifExpr :: thenExpr :: Option.toList elseExpr)
    //
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynExpr.Typar (typar, _) -> visitSynTypar addItem typar
    | SynExpr.Ident ident -> visitIdent addItem ident
    | SynExpr.LongIdent (longDotId = longDotId) -> visitSynLongIdent addItem longDotId
    | SynExpr.LongIdentSet (longDotId, expr, _) ->
        visitSynLongIdent addItem longDotId
        visitSynExpr expr addItem
    | SynExpr.DotGet (expr = expr; longDotId = longDotId) ->
        visitSynLongIdent addItem longDotId
        visitSynExpr expr addItem
    | SynExpr.DotSet (targetExpr, longDotId, rhsExpr, _) ->
        visitSynLongIdent addItem longDotId
        visitSynExprs [ targetExpr; rhsExpr ] addItem
    // let continuations = [ visitSynExpr targetExpr; visitSynExpr rhsExpr ]
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynExpr.Set (targetExpr, rhsExpr, _) -> visitSynExprs [ targetExpr; rhsExpr ] addItem
    // let continuations = List.map visitSynExpr [ targetExpr; rhsExpr ]
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynExpr.DotIndexedGet (objectExpr, indexArgs, _, _) -> visitSynExprs [ objectExpr; indexArgs ] addItem
    // let continuations = List.map visitSynExpr [ objectExpr; indexArgs ]
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynExpr.DotIndexedSet (objectExpr, indexArgs, valueExpr, _, _, _) -> visitSynExprs [ objectExpr; indexArgs; valueExpr ] addItem
    // let continuations = List.map visitSynExpr [ objectExpr; indexArgs; valueExpr ]
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynExpr.NamedIndexedPropertySet (longDotId, expr1, expr2, _) ->
        visitSynLongIdent addItem longDotId
        visitSynExprs [ expr1; expr2 ] addItem
    // let continuations = [ visitSynExpr expr1; visitSynExpr expr2 ]
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynExpr.DotNamedIndexedPropertySet (targetExpr, longDotId, argExpr, rhsExpr, _) ->
        visitSynLongIdent addItem longDotId
        visitSynExprs [ targetExpr; argExpr; rhsExpr ] addItem
    // let continuations = List.map visitSynExpr [ targetExpr; argExpr; rhsExpr ]
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynExpr.TypeTest (expr, targetType, _)
    | SynExpr.Upcast (expr, targetType, _)
    | SynExpr.Downcast (expr, targetType, _) ->
        visitSynType targetType addItem
        visitSynExpr expr addItem
    | SynExpr.InferredUpcast (expr, _)
    | SynExpr.InferredDowncast (expr, _) -> visitSynExpr expr addItem
    | SynExpr.Null _ -> ()
    | SynExpr.AddressOf (expr = expr) -> visitSynExpr expr addItem
    | SynExpr.TraitCall (supportTys, traitSig, argExpr, _) ->
        visitSynType supportTys addItem
        visitSynMemberSig addItem traitSig
        visitSynExpr argExpr addItem
    | SynExpr.JoinIn (lhsExpr, _, rhsExpr, _) -> visitSynExprs [ lhsExpr; rhsExpr ] addItem
    // let continuations = List.map visitSynExpr [ lhsExpr; rhsExpr ]
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynExpr.ImplicitZero _ -> ()
    | SynExpr.SequentialOrImplicitYield (_, expr1, expr2, _, _) -> visitSynExprs [ expr1; expr2 ] addItem
    // let continuations = List.map visitSynExpr [ expr1; expr2 ]
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynExpr.YieldOrReturn (expr = expr)
    | SynExpr.YieldOrReturnFrom (expr = expr) -> visitSynExpr expr addItem
    | SynExpr.LetOrUseBang (pat = pat; rhs = rhs; andBangs = andBangs; body = body) ->
        visitPat pat addItem

        let es =
            let andBangExprs =
                List.map
                    (fun (SynExprAndBang (body = body)) ->
                        visitPat pat addItem
                        body)
                    andBangs

            body :: rhs :: andBangExprs

        visitSynExprs es addItem
    // let continuations =
    //     let andBangExprs =
    //         List.map
    //             (fun (SynExprAndBang (body = body)) ->
    //                 visitPat pat addItem
    //                 body)
    //             andBangs
    //
    //     List.map visitSynExpr (body :: rhs :: andBangExprs)
    //
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynExpr.DoBang (expr, _) -> visitSynExpr expr addItem
    | SynExpr.LibraryOnlyILAssembly (typeArgs = typeArgs; args = args; retTy = retTy) ->
        List.iter (fun t -> visitSynType t addItem) (typeArgs @ retTy)
        visitSynExprs args addItem
    // let continuations = List.map visitSynExpr args
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynExpr.LibraryOnlyStaticOptimization (constraints, expr, optimizedExpr, _) ->
        constraints
        |> List.iter (function
            | SynStaticOptimizationConstraint.WhenTyparTyconEqualsTycon (typar = typar; rhsType = t) ->
                visitSynTypar addItem typar
                visitSynType t addItem
            | SynStaticOptimizationConstraint.WhenTyparIsStruct (typar = typar) -> visitSynTypar addItem typar)

        visitSynExprs [ expr; optimizedExpr ] addItem
    // let continuations = List.map visitSynExpr [ expr; optimizedExpr ]
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynExpr.LibraryOnlyUnionCaseFieldGet (expr, longId, _, _) ->
        visitLongIdent addItem longId
        visitSynExpr expr addItem
    | SynExpr.LibraryOnlyUnionCaseFieldSet (expr, longId, _, rhsExpr, _) ->
        visitLongIdent addItem longId
        visitSynExprs [ expr; rhsExpr ] addItem
    // let continuations = List.map visitSynExpr [ expr; rhsExpr ]
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynExpr.ArbitraryAfterError _ -> ()
    | SynExpr.FromParseError _ -> ()
    | SynExpr.DiscardAfterMissingQualificationAfterDot _ -> ()
    | SynExpr.Fixed (expr, _) -> visitSynExpr expr addItem
    | SynExpr.InterpolatedString (contents = contents) ->
        let es =
            List.choose
                (function
                | SynInterpolatedStringPart.FillExpr (fillExpr = e) -> Some e
                | SynInterpolatedStringPart.String _ -> None)
                contents

        visitSynExprs es addItem
    // let continuations =
    //     List.map
    //         visitSynExpr
    //         (List.choose
    //             (function
    //             | SynInterpolatedStringPart.FillExpr (fillExpr = e) -> Some e
    //             | SynInterpolatedStringPart.String _ -> None)
    //             contents)
    //
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynExpr.DebugPoint _ -> ()
    | SynExpr.Dynamic (funcExpr, _, argExpr, _) -> visitSynExprs [ funcExpr; argExpr ] addItem
// let continuations = List.map visitSynExpr [ funcExpr; argExpr ]
// let finalContinuation = List.iter addItem
// Continuation.sequence continuations finalContinuation

let visitPats (ps: SynPat list) (addItem: AddItem) =
    for p in ps do
        visitPat p addItem

let visitPat (p: SynPat) (addItem: AddItem) =
    match p with
    | SynPat.Paren (pat = pat) -> visitPat pat addItem
    | SynPat.Typed (pat = pat; targetType = t) ->
        visitSynType t addItem
        visitPat pat addItem
    | SynPat.Const _ -> ()
    | SynPat.Wild _ -> ()
    | SynPat.Named (ident = ident) -> visitSynIdent addItem ident
    | SynPat.Attrib (pat, attributes, _) ->
        visitSynAttributes addItem attributes
        visitPat pat addItem
    | SynPat.Or (lhsPat, rhsPat, _, _) -> visitPats [ lhsPat; rhsPat ] addItem
    // let continuations = List.map visitPat [ lhsPat; rhsPat ]
    // let finalContinuation = List.iter addItem
    // Continuation.sequence continuations finalContinuation
    | SynPat.ListCons (lhsPat, rhsPat, _, _) ->
        visitPats [ lhsPat; rhsPat ] addItem
        // let continuations = List.map visitPat [ lhsPat; rhsPat ]
        // let finalContinuation = List.iter addItem
        // Continuation.sequence continuations finalContinuation
    | SynPat.Ands (pats, _) ->
        visitPats pats addItem
        // let continuations = List.map visitPat pats
        // let finalContinuation = List.iter addItem
        // Continuation.sequence continuations finalContinuation
    | SynPat.As (lhsPat, rhsPat, _) ->
        visitPats  [ lhsPat; rhsPat ] addItem
        // let continuations = List.map visitPat [ lhsPat; rhsPat ]
        // let finalContinuation = List.iter addItem
        // Continuation.sequence continuations finalContinuation
    | SynPat.LongIdent (longDotId = longDotId; typarDecls = typarDecls; argPats = argPats) ->
        visitSynLongIdent addItem longDotId
        visitOpt visitSynValTyparDecls addItem typarDecls
        visitSynArgPats addItem argPats
    | SynPat.Tuple (_, elementPats, _) ->
        visitPats elementPats addItem
        // let continuations = List.map visitPat elementPats
        // let finalContinuation = List.iter addItem
        // Continuation.sequence continuations finalContinuation
    | SynPat.ArrayOrList (_, elementPats, _) ->
        visitPats elementPats addItem
        // let continuations = List.map visitPat elementPats
        // let finalContinuation = List.iter addItem
        // Continuation.sequence continuations finalContinuation
    | SynPat.Record (fieldPats, _) ->
        let pats =
            List.map
                (fun ((l, _), _, p) ->
                    visitLongIdent addItem l
                    p)
                fieldPats

        visitPats pats addItem
        // let continuations = List.map visitPat pats
        // let finalContinuation = List.iter addItem
        // Continuation.sequence continuations finalContinuation
    | SynPat.Null _ -> ()
    | SynPat.OptionalVal _ -> ()
    | SynPat.IsInst (t, _) -> visitSynType t addItem
    | SynPat.QuoteExpr (expr, _) -> visitSynExpr expr addItem
    | SynPat.DeprecatedCharRange _ -> ()
    | SynPat.InstanceMember _ -> ()
    | SynPat.FromParseError _ -> ()

let visitSynArgPats (addItem: AddItem) (argPat: SynArgPats) =
    match argPat with
    | SynArgPats.Pats args -> List.iter (fun p -> visitPat p addItem) args
    | SynArgPats.NamePatPairs (pats = pats) -> List.iter (fun (_, _, p) -> visitPat p addItem) pats

let visitSynSimplePat (pat: SynSimplePat) (addItem: AddItem) =
    match pat with
    | SynSimplePat.Id (ident = ident) -> visitIdent addItem ident
    | SynSimplePat.Attrib (pat, attributes, _) ->
        visitSynAttributes addItem attributes
        visitSynSimplePat pat addItem
    | SynSimplePat.Typed (pat, t, _) ->
        visitSynType t addItem
        visitSynSimplePat pat addItem

let visitSynSimplePats (pats: SynSimplePats) (addItem: AddItem) =
    match pats with
    | SynSimplePats.SimplePats (pats = pats) -> List.iter (fun p -> visitSynSimplePat p addItem) pats
    | SynSimplePats.Typed (pats, t, _) ->
        visitSynType t addItem
        visitSynSimplePats pats addItem

let visitSynMatchClause (addItem: AddItem) (SynMatchClause (pat = pat; whenExpr = whenExpr; resultExpr = resultExpr)) =
    visitPat pat addItem
    Option.iter (fun e -> visitSynExpr e addItem) whenExpr
    visitSynExpr resultExpr addItem

let visitBinding (addItem: AddItem) (SynBinding (attributes = attributes; headPat = headPat; returnInfo = returnInfo; expr = expr)) =
    match headPat with
    | SynPat.LongIdent(argPats = SynArgPats.Pats pats) -> List.iter (fun p -> visitPat p addItem) pats
    | _ -> visitPat headPat addItem

    visitSynAttributes addItem attributes
    visitOpt visitSynBindingReturnInfo addItem returnInfo
    visitSynExpr expr addItem

let visitSynBindingReturnInfo (addItem: AddItem) (SynBindingReturnInfo (typeName = typeName; attributes = attributes)) =
    visitSynAttributes addItem attributes
    visitSynType typeName addItem

let visitSynMemberSig (addItem: AddItem) (ms: SynMemberSig) : unit =
    match ms with
    | SynMemberSig.Member (memberSig = memberSig) -> visitSynValSig addItem memberSig
    | SynMemberSig.Interface (interfaceType, _) -> visitSynType interfaceType addItem
    | SynMemberSig.Inherit (inheritedType, _) -> visitSynType inheritedType addItem
    | SynMemberSig.ValField (field, _) -> visitSynField addItem field
    | SynMemberSig.NestedType _ -> ()

let visitFile (f: ParsedInput) : Set<string> =
    let hs = HashSet<string>()
    let add item = hs.Add(item) |> ignore

    match f with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = contents)) ->
        for SynModuleOrNamespaceSig (longId = longId; decls = decls; attribs = attribs) in contents do
            visitLongIdent add longId
            visitSynAttributes add attribs

            for decl in decls do
                visitSynModuleSigDecl add decl

    | ParsedInput.ImplFile (ParsedImplFileInput (contents = contents)) ->
        for SynModuleOrNamespace (longId = longId; decls = decls; attribs = attribs) in contents do
            visitLongIdent add longId
            visitSynAttributes add attribs

            for decl in decls do
                visitSynModuleDecl add decl

    Set(hs)
