// Copyright (c) Microsoft Corporation. All Rights Reserved.

module internal FSharp.Compiler.TypedTreeDiff

open System
open System.Collections.Generic
open System.Text

open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps

[<RequireQualifiedAccess>]
type SymbolKind =
    | Value
    | Entity

[<RequireQualifiedAccess>]
type SymbolMemberKind =
    | Method
    | PropertyGet of propertyName: string
    | PropertySet of propertyName: string
    | EventAdd of eventName: string
    | EventRemove of eventName: string
    | EventInvoke of eventName: string

/// Joins a snapshot path and logical name into a dotted qualified name, dropping a redundant dotted head.
let internal canonicalQualifiedName (path: string list) (logicalName: string) =
    let canonical =
        match path with
        | head :: rest when head.Contains(".") ->
            let segments = head.Split('.') |> Array.toList

            if
                List.length rest >= List.length segments
                && List.truncate (List.length segments) rest = segments
            then
                rest
            else
                path
        | _ -> path

    match canonical with
    | [] -> logicalName
    | _ -> String.concat "." (canonical @ [ logicalName ])

type SymbolId =
    {
        Path: string list
        LogicalName: string
        Stamp: Stamp
        Kind: SymbolKind
        MemberKind: SymbolMemberKind option
        IsSynthesized: bool
        CompiledName: string option
        TotalArgCount: int option
        GenericArity: int option
    }

    member x.QualifiedName = canonicalQualifiedName x.Path x.LogicalName

[<RequireQualifiedAccess>]
type SemanticEditKind =
    | MethodBody
    | Insert
    | Delete
    | TypeDefinition

[<RequireQualifiedAccess>]
type RudeEditKind =
    | SignatureChange
    | InlineChange
    | TypeLayoutChange
    | DeclarationAdded
    | DeclarationRemoved
    | Unsupported

type SemanticEdit =
    {
        Symbol: SymbolId
        Kind: SemanticEditKind
        BaselineHash: int option
        UpdatedHash: int option
        IsSynthesized: bool
        ContainingEntity: string option
    }

type RudeEdit =
    {
        Symbol: SymbolId option
        Kind: RudeEditKind
        Message: string
    }

type TypedTreeDiffResult =
    {
        SemanticEdits: SemanticEdit list
        RudeEdits: RudeEdit list
    }

let private emptyDiff = { SemanticEdits = []; RudeEdits = [] }

let private stableHash (text: string) =
    if String.IsNullOrEmpty text then
        0
    else
        let mutable hash = 2166136261u

        for ch in text do
            hash <- (hash ^^^ uint32 ch) * 16777619u

        int hash

let private propertyDisplayName (vref: ValRef) =
    let name = vref.PropertyName

    if String.IsNullOrWhiteSpace name then
        vref.DisplayName
    else
        name

let private tryEventMemberKind (compiledName: string) =
    if String.IsNullOrEmpty compiledName then
        None
    elif compiledName.StartsWith("add_", StringComparison.Ordinal) then
        Some(SymbolMemberKind.EventAdd(compiledName.Substring(4)))
    elif compiledName.StartsWith("remove_", StringComparison.Ordinal) then
        Some(SymbolMemberKind.EventRemove(compiledName.Substring(7)))
    elif compiledName.StartsWith("raise_", StringComparison.Ordinal) then
        Some(SymbolMemberKind.EventInvoke(compiledName.Substring(6)))
    else
        None

let private memberKindOfVal (var: Val) =
    let vref = mkLocalValRef var

    if vref.IsPropertyGetterMethod then
        Some(SymbolMemberKind.PropertyGet(propertyDisplayName vref))
    elif vref.IsPropertySetterMethod then
        Some(SymbolMemberKind.PropertySet(propertyDisplayName vref))
    else
        let compiledName = vref.CompiledName None

        match tryEventMemberKind compiledName with
        | Some accessor -> Some accessor
        | None when vref.MemberInfo.IsSome -> Some SymbolMemberKind.Method
        | _ -> None

let private tryGetDeclaringEntityCompiledName (vref: ValRef) =
    match vref.TryDeclaringEntity with
    | Parent parent ->
        try
            Some parent.CompiledRepresentationForNamedType.FullName
        with _ ->
            try
                Some parent.CompiledName
            with _ ->
                None
    | ParentNone -> None

let private tryStableValReferenceIdentity (vref: ValRef) =
    let compiledName =
        try
            vref.CompiledName None
        with _ ->
            vref.LogicalName

    let totalArgCount =
        vref.ValReprInfo
        |> Option.map (fun info -> info.TotalArgCount)
        |> Option.defaultValue 0

    let genericArity =
        vref.ValReprInfo
        |> Option.map (fun info -> info.NumTypars)
        |> Option.defaultValue 0

    let baseIdentity = $"{compiledName}|args={totalArgCount}|gen={genericArity}"

    match tryGetDeclaringEntityCompiledName vref with
    | Some declaringType -> Some($"{declaringType}::{baseIdentity}")
    | None when vref.IsCompiledAsTopLevel -> Some baseIdentity
    | _ -> None

let private normalizeTypeString (text: string) =
    let sb = StringBuilder(text.Length)
    let mutable i = 0
    let mutable skipParen = 0
    let solvedMarker = " (solved: "

    while i < text.Length do
        let ch = text[i]

        if ch = '?' then
            let mutable j = i + 1

            while j < text.Length && Char.IsDigit text[j] do
                j <- j + 1

            if j < text.Length && text.AsSpan(j).StartsWith(solvedMarker.AsSpan()) then
                i <- j + solvedMarker.Length
                skipParen <- skipParen + 1
            else
                sb.Append ch |> ignore
                i <- i + 1
        elif ch = ')' && skipParen > 0 then
            skipParen <- skipParen - 1
            i <- i + 1
        else
            sb.Append ch |> ignore
            i <- i + 1

    sb.ToString().Replace("  ", " ").Trim()

let private compiledTyconName (tcref: TyconRef) =
    try
        tcref.CompiledRepresentationForNamedType.FullName
    with _ ->
        tcref.CompiledName

/// Renders types with their compiled identities so equal display names from different
/// namespaces or enclosing types cannot collapse to the same hot reload signature.
let private tyToString (_: DisplayEnv) (ty: TType) =
    let rec render ty =
        let renderArgs tys =
            tys |> List.map render |> String.concat ","

        match ty with
        | TType_forall(typars, bodyTy) ->
            let names = typars |> List.map (fun typar -> typar.DisplayName) |> String.concat ","
            $"forall<{names}>.{render bodyTy}"
        | TType_app(tcref, typeArgs, _) ->
            let name = compiledTyconName tcref

            if List.isEmpty typeArgs then
                name
            else
                $"{name}<{renderArgs typeArgs}>"
        | TType_anon(anonInfo, typeArgs) ->
            let name = anonInfo.ILTypeRef.FullName

            if List.isEmpty typeArgs then
                name
            else
                $"{name}<{renderArgs typeArgs}>"
        | TType_tuple(tupleInfo, typeArgs) ->
            let kind = if evalTupInfoIsStruct tupleInfo then "struct" else "ref"
            $"tuple:{kind}<{renderArgs typeArgs}>"
        | TType_fun(domainTy, rangeTy, _) -> $"func<{render domainTy},{render rangeTy}>"
        | TType_ucase(caseRef, typeArgs) -> $"ucase:{compiledTyconName caseRef.TyconRef}.{caseRef.CaseName}<{renderArgs typeArgs}>"
        | TType_var(typar, _) ->
            match typar.Solution with
            | Some solution -> render solution
            | None when typar.IsCompilerGenerated -> "inference-variable"
            | None -> $"typar:{typar.DisplayName}"
        | TType_measure _ -> normalizeTypeString (ty.ToString())

    render ty

let private constraintDigest (denv: DisplayEnv) (constraint_: TyparConstraint) =
    match constraint_ with
    | TyparConstraint.CoercesTo(ty, _) -> "coerces:" + tyToString denv ty
    | TyparConstraint.DefaultsTo(priority, ty, _) -> $"defaults:{priority}:{tyToString denv ty}"
    | TyparConstraint.SupportsNull _ -> "null"
    | TyparConstraint.NotSupportsNull _ -> "notnull"
    | TyparConstraint.MayResolveMember(traitInfo, _) -> "member:" + traitInfo.MemberLogicalName
    | TyparConstraint.IsNonNullableStruct _ -> "struct"
    | TyparConstraint.IsReferenceType _ -> "class"
    | TyparConstraint.SimpleChoice(tys, _) -> "choice:" + (tys |> Seq.map (tyToString denv) |> String.concat ",")
    | TyparConstraint.RequiresDefaultConstructor _ -> "new"
    | TyparConstraint.IsEnum(ty, _) -> "enum:" + tyToString denv ty
    | TyparConstraint.IsDelegate(ty1, ty2, _) -> "delegate:" + tyToString denv ty1 + "," + tyToString denv ty2
    | TyparConstraint.SupportsComparison _ -> "comparison"
    | TyparConstraint.SupportsEquality _ -> "equality"
    | TyparConstraint.IsUnmanaged _ -> "unmanaged"
    | TyparConstraint.AllowsRefStruct _ -> "allowsrefstruct"

let private typarConstraintsDigest (denv: DisplayEnv) (typars: Typar list) =
    typars
    |> Seq.collect (fun tp ->
        tp.Constraints
        |> Seq.map (fun c -> $"{tp.DisplayName}:{constraintDigest denv c}"))
    |> Seq.sort
    |> String.concat ";"

let private constDigest (c: Const) =
    match c with
    | Const.Bool v -> if v then "true" else "false"
    | Const.SByte v -> v.ToString("g", Globalization.CultureInfo.InvariantCulture)
    | Const.Int16 v -> v.ToString("g", Globalization.CultureInfo.InvariantCulture)
    | Const.Int32 v -> v.ToString("g", Globalization.CultureInfo.InvariantCulture)
    | Const.Int64 v -> v.ToString("g", Globalization.CultureInfo.InvariantCulture)
    | Const.Byte v -> v.ToString("g", Globalization.CultureInfo.InvariantCulture)
    | Const.UInt16 v -> v.ToString("g", Globalization.CultureInfo.InvariantCulture)
    | Const.UInt32 v -> v.ToString("g", Globalization.CultureInfo.InvariantCulture)
    | Const.UInt64 v -> v.ToString("g", Globalization.CultureInfo.InvariantCulture)
    | Const.IntPtr v -> v.ToString("g", Globalization.CultureInfo.InvariantCulture)
    | Const.UIntPtr v -> v.ToString("g", Globalization.CultureInfo.InvariantCulture)
    | Const.Single v -> v.ToString("r", Globalization.CultureInfo.InvariantCulture)
    | Const.Double v -> v.ToString("r", Globalization.CultureInfo.InvariantCulture)
    | Const.String v -> $"\"{v}\""
    | Const.Char v -> $"'{string v}'"
    | Const.Decimal v -> v.ToString("g", Globalization.CultureInfo.InvariantCulture)
    | Const.Unit -> "()"
    | Const.Zero -> "zero"

let private identityNode (tag: string) (fields: string seq) =
    let builder = StringBuilder(tag)

    for field in fields do
        builder.Append('|').Append(field.Length).Append(':').Append(field) |> ignore

    builder.ToString()

let private valIdentity denv (vref: ValRef) =
    let stableName =
        tryStableValReferenceIdentity vref
        |> Option.defaultValue $"local:{vref.LogicalName}"

    identityNode "val" [ stableName; tyToString denv vref.Type ]

let private traitIdentity denv (traitInfo: TraitConstraintInfo) =
    identityNode
        "trait"
        [
            traitInfo.MemberLogicalName
            string traitInfo.MemberFlags.IsInstance
            string traitInfo.MemberFlags.IsDispatchSlot
            string traitInfo.MemberFlags.IsOverrideOrExplicitImpl
            string traitInfo.MemberFlags.IsFinal
            string traitInfo.MemberFlags.MemberKind
            traitInfo.SupportTypes |> List.map (tyToString denv) |> String.concat ","
            traitInfo.CompiledObjectAndArgumentTypes
            |> List.map (tyToString denv)
            |> String.concat ","
            traitInfo.CompiledReturnType
            |> Option.map (tyToString denv)
            |> Option.defaultValue "void"
        ]

/// Produces an exhaustive, payload-sensitive identity for every TOp case. Debug-point
/// payloads are intentionally excluded because they do not alter emitted instructions.
let private opIdentity denv (op: TOp) =
    let tyconName = compiledTyconName

    let caseName (caseRef: UnionCaseRef) =
        $"{tyconName caseRef.TyconRef}.{caseRef.CaseName}"

    let types tys =
        tys |> List.map (tyToString denv) |> String.concat ","

    match op with
    | TOp.UnionCase caseRef -> identityNode "union-case" [ caseName caseRef ]
    | TOp.ExnConstr typeRef -> identityNode "exn" [ tyconName typeRef ]
    | TOp.Tuple tupleInfo -> identityNode "tuple" [ string (evalTupInfoIsStruct tupleInfo) ]
    | TOp.AnonRecd info -> identityNode "anon-record" [ info.ILTypeRef.FullName; String.concat "," info.SortedNames ]
    | TOp.AnonRecdGet(info, index) ->
        identityNode "anon-record-get" [ info.ILTypeRef.FullName; String.concat "," info.SortedNames; string index ]
    | TOp.Array -> "array"
    | TOp.Bytes bytes -> identityNode "bytes" [ BitConverter.ToString bytes ]
    | TOp.UInt16s values -> identityNode "uint16s" [ values |> Array.map string |> String.concat "," ]
    | TOp.While(_, marker) -> identityNode "while" [ string marker ]
    | TOp.IntegerForLoop(_, _, style) -> identityNode "for" [ string style ]
    | TOp.TryWith _ -> "try-with"
    | TOp.TryFinally _ -> "try-finally"
    | TOp.Recd(info, typeRef) -> identityNode "record" [ string info; tyconName typeRef ]
    | TOp.ValFieldSet fieldRef -> identityNode "field-set" [ tyconName fieldRef.TyconRef; fieldRef.FieldName ]
    | TOp.ValFieldGet fieldRef -> identityNode "field-get" [ tyconName fieldRef.TyconRef; fieldRef.FieldName ]
    | TOp.ValFieldGetAddr(fieldRef, isReadonly) ->
        identityNode "field-address" [ tyconName fieldRef.TyconRef; fieldRef.FieldName; string isReadonly ]
    | TOp.UnionCaseTagGet typeRef -> identityNode "union-tag" [ tyconName typeRef ]
    | TOp.UnionCaseProof caseRef -> identityNode "union-proof" [ caseName caseRef ]
    | TOp.UnionCaseFieldGet(caseRef, index) -> identityNode "union-field-get" [ caseName caseRef; string index ]
    | TOp.UnionCaseFieldGetAddr(caseRef, index, isReadonly) ->
        identityNode "union-field-address" [ caseName caseRef; string index; string isReadonly ]
    | TOp.UnionCaseFieldSet(caseRef, index) -> identityNode "union-field-set" [ caseName caseRef; string index ]
    | TOp.ExnFieldGet(typeRef, index) -> identityNode "exn-field-get" [ tyconName typeRef; string index ]
    | TOp.ExnFieldSet(typeRef, index) -> identityNode "exn-field-set" [ tyconName typeRef; string index ]
    | TOp.TupleFieldGet(tupleInfo, index) -> identityNode "tuple-field-get" [ string (evalTupInfoIsStruct tupleInfo); string index ]
    | TOp.ILAsm(instructions, returnTypes) -> identityNode "il" [ instructions |> List.map string |> String.concat ";"; types returnTypes ]
    | TOp.RefAddrGet isReadonly -> identityNode "ref-address" [ string isReadonly ]
    | TOp.Coerce -> "coerce"
    | TOp.Reraise -> "reraise"
    | TOp.Return -> "return"
    | TOp.Goto label -> identityNode "goto" [ string label ]
    | TOp.Label label -> identityNode "label" [ string label ]
    | TOp.TraitCall traitInfo -> traitIdentity denv traitInfo
    | TOp.LValueOp(operation, vref) -> identityNode "lvalue" [ string operation; valIdentity denv vref ]
    | TOp.ILCall(isVirtual,
                 isProtected,
                 isStruct,
                 isCtor,
                 valUseFlag,
                 isProperty,
                 noTailCall,
                 methodRef,
                 enclosingTypeArgs,
                 methodTypeArgs,
                 returnTypes) ->
        identityNode
            "il-call"
            [
                string isVirtual
                string isProtected
                string isStruct
                string isCtor
                string valUseFlag
                string isProperty
                string noTailCall
                sprintf "%A" methodRef
                types enclosingTypeArgs
                types methodTypeArgs
                types returnTypes
            ]

let rec private exprIdentity (denv: DisplayEnv) (expr: Expr) =
    let recurse = exprIdentity denv

    let expressions values =
        values |> Seq.map recurse |> identityNode "exprs"

    let types values =
        values |> Seq.map (tyToString denv) |> identityNode "types"

    match expr with
    | Expr.Const(value, _, ty) -> identityNode "const" [ constDigest value; tyToString denv ty ]
    | Expr.Val(vref, _, _) -> valIdentity denv vref
    | Expr.App(functionExpr, _, typeArgs, args, _) -> identityNode "app" [ recurse functionExpr; types typeArgs; expressions args ]
    | Expr.Sequential(first, second, kind, _) -> identityNode "sequential" [ string kind; recurse first; recurse second ]
    | Expr.Lambda(_, _, _, parameters, body, _, _) ->
        let parameterIdentity =
            parameters
            |> List.map (fun parameter -> identityNode "parameter" [ parameter.LogicalName; tyToString denv parameter.Type ])
            |> identityNode "parameters"

        identityNode "lambda" [ parameterIdentity; recurse body ]
    | Expr.TyLambda(_, typeParameters, body, _, _) ->
        identityNode
            "type-lambda"
            [
                typeParameters
                |> List.map (fun parameter -> parameter.DisplayName)
                |> identityNode "type-parameters"
                recurse body
            ]
    | Expr.Let(binding, body, _, _) -> identityNode "let" [ bindingIdentity denv binding; recurse body ]
    | Expr.LetRec(bindings, body, _, _) ->
        identityNode
            "let-rec"
            [
                bindings |> List.map (bindingIdentity denv) |> identityNode "bindings"
                recurse body
            ]
    | Expr.Match(_, _, decision, targets, _, exprType) ->
        let targetIdentity =
            targets
            |> Array.map (fun (TTarget(boundValues, targetExpr, stateFlags)) ->
                identityNode
                    "target"
                    [
                        boundValues
                        |> List.map (fun value -> identityNode "bound" [ value.LogicalName; tyToString denv value.Type ])
                        |> identityNode "values"
                        stateFlags
                        |> Option.map (List.map string >> identityNode "state-flags")
                        |> Option.defaultValue "none"
                        recurse targetExpr
                    ])
            |> identityNode "targets"

        identityNode "match" [ decisionTreeIdentity denv decision; targetIdentity; tyToString denv exprType ]
    | Expr.Op(op, typeArgs, args, _) -> identityNode "op" [ opIdentity denv op; types typeArgs; expressions args ]
    | Expr.Obj(_, objectType, baseValue, ctorCall, overrides, interfaceImpls, _) ->
        let methodIdentity (TObjExprMethod(slotSignature, attributes, typeParameters, parameters, body, _)) =
            identityNode
                "object-method"
                [
                    slotSignatureIdentity denv slotSignature
                    attributes |> List.map (attribIdentity denv) |> identityNode "attributes"
                    typeParameters
                    |> List.map (fun parameter -> parameter.DisplayName)
                    |> identityNode "type-parameters"
                    parameters
                    |> List.concat
                    |> List.map (fun parameter -> identityNode "parameter" [ parameter.LogicalName; tyToString denv parameter.Type ])
                    |> identityNode "parameters"
                    recurse body
                ]

        identityNode
            "object"
            [
                tyToString denv objectType
                baseValue
                |> Option.map (mkLocalValRef >> valIdentity denv)
                |> Option.defaultValue "none"
                recurse ctorCall
                overrides |> List.map methodIdentity |> identityNode "overrides"
                interfaceImpls
                |> List.map (fun (interfaceType, methods) ->
                    identityNode
                        "interface"
                        [
                            tyToString denv interfaceType
                            methods |> List.map methodIdentity |> identityNode "methods"
                        ])
                |> identityNode "interfaces"
            ]
    | Expr.Quote(quotedExpr, _, isFromQueryExpression, _, quotedType) ->
        identityNode "quote" [ string isFromQueryExpression; recurse quotedExpr; tyToString denv quotedType ]
    | Expr.DebugPoint(_, body) -> recurse body
    | Expr.Link expressionRef -> recurse expressionRef.Value
    | Expr.TyChoose(typeParameters, body, _) ->
        identityNode
            "type-choose"
            [
                typeParameters
                |> List.map (fun parameter -> parameter.DisplayName)
                |> identityNode "type-parameters"
                recurse body
            ]
    | Expr.WitnessArg(traitInfo, _) -> traitIdentity denv traitInfo
    | Expr.StaticOptimization(conditions, whenTrue, whenFalse, _) ->
        let conditionIdentity =
            conditions
            |> List.map (function
                | TTyconEqualsTycon(first, second) -> identityNode "type-equals" [ tyToString denv first; tyToString denv second ]
                | TTyconIsStruct ty -> identityNode "type-is-struct" [ tyToString denv ty ])
            |> identityNode "conditions"

        identityNode "static-optimization" [ conditionIdentity; recurse whenTrue; recurse whenFalse ]

and private bindingIdentity denv (TBind(var, body, _)) =
    identityNode "binding" [ var.LogicalName; tyToString denv var.Type; exprIdentity denv body ]

and private decisionTreeIdentity denv decision =
    match decision with
    | TDSwitch(input, cases, defaultCase, _) ->
        identityNode
            "switch"
            [
                exprIdentity denv input
                cases
                |> List.map (fun (TCase(test, caseTree)) ->
                    identityNode "case" [ decisionTestIdentity denv test; decisionTreeIdentity denv caseTree ])
                |> identityNode "cases"
                defaultCase
                |> Option.map (decisionTreeIdentity denv)
                |> Option.defaultValue "none"
            ]
    | TDSuccess(results, targetNumber) ->
        identityNode
            "success"
            [
                string targetNumber
                results |> List.map (exprIdentity denv) |> identityNode "results"
            ]
    | TDBind(binding, body) -> identityNode "decision-bind" [ bindingIdentity denv binding; decisionTreeIdentity denv body ]

and private decisionTestIdentity denv test =
    match test with
    | DecisionTreeTest.UnionCase(caseRef, typeArgs) ->
        identityNode
            "test-union"
            [
                $"{compiledTyconName caseRef.TyconRef}.{caseRef.CaseName}"
                typeArgs |> List.map (tyToString denv) |> identityNode "types"
            ]
    | DecisionTreeTest.ArrayLength(length, ty) -> identityNode "test-array-length" [ string length; tyToString denv ty ]
    | DecisionTreeTest.Const value -> identityNode "test-const" [ constDigest value ]
    | DecisionTreeTest.IsNull -> "test-null"
    | DecisionTreeTest.IsInst(sourceType, targetType) -> identityNode "test-type" [ tyToString denv sourceType; tyToString denv targetType ]
    | DecisionTreeTest.ActivePatternCase(activePatternExpr, resultTypes, returnKind, activePatternIdentity, index, info) ->
        identityNode
            "test-active-pattern"
            [
                exprIdentity denv activePatternExpr
                resultTypes |> List.map (tyToString denv) |> identityNode "result-types"
                string returnKind
                activePatternIdentity
                |> Option.map (fun (vref, typeArgs) ->
                    identityNode
                        "active-pattern-value"
                        [
                            valIdentity denv vref
                            typeArgs |> List.map (tyToString denv) |> identityNode "types"
                        ])
                |> Option.defaultValue "none"
                string index
                info.LogicalName
            ]
    | DecisionTreeTest.Error _ -> "test-error"

and private slotSignatureIdentity
    denv
    (TSlotSig(name, declaringType, classTypeParameters, methodTypeParameters, parameterGroups, returnType))
    =
    let typeParameters (values: Typar list) =
        values
        |> List.map (fun parameter -> identityNode "type-parameter" [ parameter.DisplayName; typarConstraintsDigest denv [ parameter ] ])
        |> identityNode "type-parameters"

    let parameters =
        parameterGroups
        |> List.map (fun group ->
            group
            |> List.map (fun (TSlotParam(name, ty, isIn, isOut, isOptional, attributes)) ->
                identityNode
                    "slot-parameter"
                    [
                        name |> Option.defaultValue ""
                        tyToString denv ty
                        string isIn
                        string isOut
                        string isOptional
                        attributes |> List.map (attribIdentity denv) |> identityNode "attributes"
                    ])
            |> identityNode "parameter-group")
        |> identityNode "parameters"

    identityNode
        "slot"
        [
            name
            tyToString denv declaringType
            typeParameters classTypeParameters
            typeParameters methodTypeParameters
            parameters
            returnType |> Option.map (tyToString denv) |> Option.defaultValue "void"
        ]

and private attribIdentity denv (Attrib(typeRef, kind, unnamedArgs, namedArgs, appliedToAccessor, targets, _)) =
    let expressionIdentity (AttribExpr(_, evaluated)) = exprIdentity denv evaluated

    let kindIdentity =
        match kind with
        | AttribKind.ILAttrib methodRef -> identityNode "il-attribute" [ sprintf "%A" methodRef ]
        | AttribKind.FSAttrib valueRef -> identityNode "fs-attribute" [ valIdentity denv valueRef ]

    let namedArgIdentity (AttribNamedArg(name, ty, isField, value)) =
        identityNode "named-argument" [ name; tyToString denv ty; string isField; expressionIdentity value ]

    identityNode
        "attribute"
        [
            compiledTyconName typeRef
            kindIdentity
            unnamedArgs |> List.map expressionIdentity |> identityNode "arguments"
            namedArgs |> List.map namedArgIdentity |> identityNode "named-arguments"
            string appliedToAccessor
            targets |> Option.map string |> Option.defaultValue "none"
        ]

type private BindingSnapshot =
    {
        Symbol: SymbolId
        InlineInfo: ValInline
        SignatureText: string
        ConstraintsText: string
        MetadataText: string
        BodyIdentity: string
        BodyHash: int
        IsSynthesized: bool
        ContainingEntity: string option
    }

type private EntitySnapshot =
    {
        Symbol: SymbolId
        RepresentationHash: int
        RepresentationText: string
        CompiledFullName: string option
    }

let private symbolId path logicalName stamp kind memberKind isSynthesized compiledName totalArgCount genericArity =
    {
        Path = path
        LogicalName = logicalName
        Stamp = stamp
        Kind = kind
        MemberKind = memberKind
        IsSynthesized = isSynthesized
        CompiledName = compiledName
        TotalArgCount = totalArgCount
        GenericArity = genericArity
    }

let private bindingKey (snapshot: BindingSnapshot) =
    let entityKey = snapshot.ContainingEntity |> Option.defaultValue ""

    let memberKind =
        snapshot.Symbol.MemberKind |> Option.map string |> Option.defaultValue ""

    let compiledName = snapshot.Symbol.CompiledName |> Option.defaultValue ""

    // Signature and generic arity participate in deterministic list pairing below. Keeping
    // them out of the primary key means a changed signature is classified as rude instead
    // of looking like an unrelated remove/add pair.
    $"{snapshot.Symbol.QualifiedName}|entity={entityKey}|kind={memberKind}|compiled={compiledName}"

let private entityKey (snapshot: EntitySnapshot) = snapshot.Symbol.QualifiedName

let private tryGetMethodGenericArity (var: Val) =
    match var.ValReprInfo with
    | None -> None
    | Some valReprInfo ->
        let numEnclosingTypars = CountEnclosingTyparsOfActualParentOfVal var

        if
            var.Typars.Length = valReprInfo.NumTypars
            && numEnclosingTypars <= var.Typars.Length
        then
            var.Typars
            |> List.skip numEnclosingTypars
            |> List.filter (fun typeParameter -> not typeParameter.IsErased)
            |> List.length
            |> Some
        else
            Some(max 0 (valReprInfo.NumTypars - numEnclosingTypars))

let private bindingMetadataIdentity denv (var: Val) =
    let memberFlags =
        match var.MemberInfo with
        | None -> "none"
        | Some memberInfo ->
            let flags = memberInfo.MemberFlags

            identityNode
                "member"
                [
                    string flags.IsInstance
                    string flags.IsDispatchSlot
                    string flags.IsOverrideOrExplicitImpl
                    string flags.IsFinal
                    string flags.GetterOrSetterIsCompilerGenerated
                    string flags.MemberKind
                    string memberInfo.IsImplemented
                ]

    identityNode
        "metadata"
        [
            string (var.Accessibility.AsILMemberAccess())
            memberFlags
            string var.IsMutable
            string var.IsExtensionMember
            string var.IsCompiledAsTopLevel
            var.LiteralValue |> Option.map constDigest |> Option.defaultValue "none"
            var.Attribs |> List.map (attribIdentity denv) |> identityNode "attributes"
        ]

let rec private snapshotModuleBinding g denv (path: string list) (bindings, entities) binding =
    match binding with
    | ModuleOrNamespaceBinding.Binding b ->
        let snapshot = snapshotBinding g denv path b
        (addBinding snapshot bindings, entities)
    | ModuleOrNamespaceBinding.Module(moduleEntity, contents) ->
        snapshotModuleContents g denv (path @ [ moduleEntity.LogicalName ]) (bindings, entities) contents

and private snapshotModuleContents g denv path (bindings, entities) contents =
    match contents with
    | ModuleOrNamespaceContents.TMDefs defs -> ((bindings, entities), defs) ||> List.fold (snapshotModuleContents g denv path)
    | ModuleOrNamespaceContents.TMDefLet(binding, _) ->
        let snapshot = snapshotBinding g denv path binding
        (addBinding snapshot bindings, entities)
    | ModuleOrNamespaceContents.TMDefRec(_, _, tycons, moduleBindings, _) ->
        let entitiesWithTypes =
            (entities, tycons)
            ||> List.fold (fun acc tycon ->
                let snapshot = snapshotTycon denv path tycon
                Map.add (entityKey snapshot) snapshot acc)

        List.fold (snapshotModuleBinding g denv path) (bindings, entitiesWithTypes) moduleBindings
    | ModuleOrNamespaceContents.TMDefDo _
    | ModuleOrNamespaceContents.TMDefOpens _ -> (bindings, entities)

and private tryGetContainingEntityFullName (var: Val) =
    match var.MemberInfo with
    | Some memberInfo ->
        try
            Some memberInfo.ApparentEnclosingEntity.CompiledRepresentationForNamedType.FullName
        with _ ->
            None
    | None -> None

and private snapshotBinding _g denv path (TBind(var, expr, _)) =
    let signature = tyToString denv var.Type
    let constraints = typarConstraintsDigest denv var.Typars
    let containingEntity = tryGetContainingEntityFullName var
    let memberKind = memberKindOfVal var
    let vref = mkLocalValRef var

    let compiledName =
        try
            Some(vref.CompiledName None)
        with _ ->
            None

    let isInstanceMember =
        match var.MemberInfo with
        | Some memberInfo -> memberInfo.MemberFlags.IsInstance
        | None -> false

    let totalArgCount =
        var.ValReprInfo
        |> Option.map (fun info ->
            if isInstanceMember then
                max 0 (info.TotalArgCount - 1)
            else
                info.TotalArgCount)

    let genericArity = tryGetMethodGenericArity var
    let bodyIdentity = exprIdentity denv expr

    {
        Symbol =
            symbolId
                path
                var.LogicalName
                var.Stamp
                SymbolKind.Value
                memberKind
                var.IsCompilerGenerated
                compiledName
                totalArgCount
                genericArity
        InlineInfo = var.InlineInfo
        SignatureText = signature
        ConstraintsText = constraints
        MetadataText = bindingMetadataIdentity denv var
        BodyIdentity = bodyIdentity
        BodyHash = stableHash bodyIdentity
        IsSynthesized = var.IsCompilerGenerated
        ContainingEntity = containingEntity
    }

and private snapshotTycon denv path (tycon: Tycon) =
    let reprText =
        let sb = StringBuilder()
        sb.Append("kind:").Append(tycon.TypeOrMeasureKind.ToString()) |> ignore

        sb.Append("|access:").Append(tycon.Accessibility.AsILTypeDefAccess().ToString())
        |> ignore

        sb.Append("|repr-access:").Append(tycon.TypeReprAccessibility.AsILTypeDefAccess().ToString())
        |> ignore

        sb.Append("|constraints:").Append(typarConstraintsDigest denv tycon.Typars)
        |> ignore

        sb.Append("|attributes:").Append(tycon.Attribs |> List.map (attribIdentity denv) |> identityNode "attributes")
        |> ignore

        match tycon.TypeReprInfo with
        | TFSharpTyconRepr data ->
            sb.Append("|fs-kind:").Append(data.fsobjmodel_kind.ToString()) |> ignore

            match data.fsobjmodel_kind with
            | FSharpTyconKind.TFSharpUnion ->
                data.fsobjmodel_cases.UnionCasesAsList
                |> List.iter (fun case ->
                    sb
                        .Append("|case:")
                        .Append(case.LogicalName)
                        .Append("[")
                        .Append(case.Accessibility.AsILMemberAccess().ToString())
                        .Append("]")
                    |> ignore

                    case.FieldTable.FieldsByIndex
                    |> Array.iter (fun field ->
                        sb
                            .Append(":")
                            .Append(field.LogicalName)
                            .Append("[")
                            .Append(field.Accessibility.AsILMemberAccess().ToString())
                            .Append(",static=")
                            .Append(field.IsStatic)
                            .Append(",mutable=")
                            .Append(field.IsMutable)
                            .Append(",volatile=")
                            .Append(field.IsVolatile)
                            .Append("]=")
                            .Append(tyToString denv field.FormalType)
                        |> ignore))
            | FSharpTyconKind.TFSharpRecord
            | FSharpTyconKind.TFSharpStruct
            | FSharpTyconKind.TFSharpClass
            | FSharpTyconKind.TFSharpInterface
            | FSharpTyconKind.TFSharpEnum ->
                data.fsobjmodel_rfields.FieldsByIndex
                |> Array.iter (fun field ->
                    sb
                        .Append("|field:")
                        .Append(field.LogicalName)
                        .Append("[")
                        .Append(field.Accessibility.AsILMemberAccess().ToString())
                        .Append(",static=")
                        .Append(field.IsStatic)
                        .Append(",mutable=")
                        .Append(field.IsMutable)
                        .Append(",volatile=")
                        .Append(field.IsVolatile)
                        .Append("]")
                    |> ignore

                    sb.Append("=").Append(tyToString denv field.FormalType) |> ignore

                    field.LiteralValue
                    |> Option.iter (fun value -> sb.Append("[literal:").Append(constDigest value).Append("]") |> ignore))
            | FSharpTyconKind.TFSharpDelegate slotSig -> sb.Append("|delegate:").Append(slotSignatureIdentity denv slotSig) |> ignore
        | TILObjectRepr(TILObjectReprData(_, _, definition)) -> sb.Append("|til:").Append(definition.Name) |> ignore
        | TAsmRepr ilTy -> sb.Append("|asm:").Append(ilTy.ToString()) |> ignore
        | TMeasureableRepr ty -> sb.Append("|measure:").Append(tyToString denv ty) |> ignore
#if !NO_TYPEPROVIDERS
        | TProvidedTypeRepr info -> sb.Append("|provided:").Append(string info.IsErased) |> ignore
        | TProvidedNamespaceRepr _ -> sb.Append("|provided-namespace") |> ignore
#endif
        | TNoRepr -> sb.Append("|norepr") |> ignore

        let superText =
            match tycon.TypeContents.tcaug_super with
            | Some ty -> tyToString denv ty
            | None -> ""

        sb.Append("|super:").Append(superText) |> ignore

        tycon.ImmediateInterfaceTypesOfFSharpTycon
        |> List.map (tyToString denv)
        |> List.sort
        |> List.iter (fun interfaceText -> sb.Append("|intf:").Append(interfaceText) |> ignore)

        sb.ToString()

    let compiledFullName =
        try
            Some tycon.CompiledRepresentationForNamedType.FullName
        with _ ->
            None

    {
        Symbol = symbolId path tycon.LogicalName tycon.Stamp SymbolKind.Entity None false None None None
        RepresentationHash = stableHash reprText
        RepresentationText = reprText
        CompiledFullName = compiledFullName
    }

and private addBinding (snapshot: BindingSnapshot) (bindings: Map<string, BindingSnapshot list>) =
    let key = bindingKey snapshot
    let existing = bindings |> Map.tryFind key |> Option.defaultValue []
    bindings |> Map.add key (snapshot :: existing)

let private collectSnapshots g denv (CheckedImplFile(qualifiedNameOfFile = qual; contents = contents)) =
    let initialPath = [ qual.Text ]
    let initialBindings: Map<string, BindingSnapshot list> = Map.empty
    let initialEntities: Map<string, EntitySnapshot> = Map.empty
    snapshotModuleContents g denv initialPath (initialBindings, initialEntities) contents

let private semanticEdit (snapshot: BindingSnapshot) kind baselineHash updatedHash : SemanticEdit =
    {
        Symbol = snapshot.Symbol
        Kind = kind
        BaselineHash = baselineHash
        UpdatedHash = updatedHash
        IsSynthesized = snapshot.IsSynthesized
        ContainingEntity = snapshot.ContainingEntity
    }

let private compareMatchedBinding (baseline: BindingSnapshot) (updated: BindingSnapshot) : Choice<SemanticEdit option, RudeEdit> =
    if baseline.SignatureText <> updated.SignatureText then
        Choice2Of2
            {
                Symbol = Some baseline.Symbol
                Kind = RudeEditKind.SignatureChange
                Message = $"Signature changed from '{baseline.SignatureText}' to '{updated.SignatureText}'."
            }
    elif baseline.ConstraintsText <> updated.ConstraintsText then
        Choice2Of2
            {
                Symbol = Some baseline.Symbol
                Kind = RudeEditKind.SignatureChange
                Message = $"Type parameter constraints changed from '{baseline.ConstraintsText}' to '{updated.ConstraintsText}'."
            }
    elif baseline.MetadataText <> updated.MetadataText then
        Choice2Of2
            {
                Symbol = Some baseline.Symbol
                Kind = RudeEditKind.SignatureChange
                Message = "Metadata-affecting declaration flags changed."
            }
    elif baseline.InlineInfo <> updated.InlineInfo then
        Choice2Of2
            {
                Symbol = Some baseline.Symbol
                Kind = RudeEditKind.InlineChange
                Message = "Inline annotation changed."
            }
    elif baseline.BodyIdentity <> updated.BodyIdentity then
        Choice1Of2(Some(semanticEdit baseline SemanticEditKind.MethodBody (Some baseline.BodyHash) (Some updated.BodyHash)))
    else
        Choice1Of2 None

let private compareBindingLists (baseline: BindingSnapshot list) (updated: BindingSnapshot list) =
    let baseline =
        baseline
        |> List.sortBy (fun snapshot -> snapshot.SignatureText, snapshot.Symbol.LogicalName)

    let updated =
        updated
        |> List.sortBy (fun snapshot -> snapshot.SignatureText, snapshot.Symbol.LogicalName)

    let edits = ResizeArray<SemanticEdit>()
    let rude = ResizeArray<RudeEdit>()
    let pairedCount = min baseline.Length updated.Length

    for index in 0 .. pairedCount - 1 do
        match compareMatchedBinding baseline[index] updated[index] with
        | Choice1Of2(Some edit) -> edits.Add edit
        | Choice1Of2 None -> ()
        | Choice2Of2 rudeEdit -> rude.Add rudeEdit

    baseline
    |> List.skip pairedCount
    |> List.iter (fun removed ->
        rude.Add
            {
                Symbol = Some removed.Symbol
                Kind = RudeEditKind.DeclarationRemoved
                Message = "Declaration removed."
            })

    updated
    |> List.skip pairedCount
    |> List.iter (fun added ->
        rude.Add
            {
                Symbol = Some added.Symbol
                Kind = RudeEditKind.DeclarationAdded
                Message = "Declaration addition is not classified by this foundational diff slice."
            })

    List.ofSeq edits, List.ofSeq rude

let private compareBindings (baseline: Map<string, BindingSnapshot list>) (updated: Map<string, BindingSnapshot list>) =
    let edits = ResizeArray<SemanticEdit>()
    let rude = ResizeArray<RudeEdit>()
    let matchedUpdatedKeys = HashSet<string>()

    for KeyValue(key, baselineList) in baseline do
        match Map.tryFind key updated with
        | Some updatedList ->
            matchedUpdatedKeys.Add key |> ignore
            let semanticEdits, rudeEdits = compareBindingLists baselineList updatedList
            edits.AddRange semanticEdits
            rude.AddRange rudeEdits
        | None ->
            baselineList
            |> List.iter (fun removed ->
                rude.Add
                    {
                        Symbol = Some removed.Symbol
                        Kind = RudeEditKind.DeclarationRemoved
                        Message = "Declaration removed."
                    })

    for KeyValue(key, updatedList) in updated do
        if not (matchedUpdatedKeys.Contains key) then
            updatedList
            |> List.iter (fun added ->
                rude.Add
                    {
                        Symbol = Some added.Symbol
                        Kind = RudeEditKind.DeclarationAdded
                        Message = "Declaration addition is not classified by this foundational diff slice."
                    })

    List.ofSeq edits, List.ofSeq rude

let private compareEntities (baseline: Map<string, EntitySnapshot>) (updated: Map<string, EntitySnapshot>) =
    let edits = ResizeArray<SemanticEdit>()
    let rude = ResizeArray<RudeEdit>()

    for KeyValue(key, baselineEntity) in baseline do
        match Map.tryFind key updated with
        | Some updatedEntity ->
            if baselineEntity.RepresentationText <> updatedEntity.RepresentationText then
                rude.Add
                    {
                        Symbol = Some baselineEntity.Symbol
                        Kind = RudeEditKind.TypeLayoutChange
                        Message =
                            $"Type representation changed from '{baselineEntity.RepresentationText}' to '{updatedEntity.RepresentationText}'."
                    }
        | None ->
            rude.Add
                {
                    Symbol = Some baselineEntity.Symbol
                    Kind = RudeEditKind.DeclarationRemoved
                    Message = "Type declaration removed."
                }

    for KeyValue(key, updatedEntity) in updated do
        if not (Map.containsKey key baseline) then
            rude.Add
                {
                    Symbol = Some updatedEntity.Symbol
                    Kind = RudeEditKind.DeclarationAdded
                    Message = "Type addition is not classified by this foundational diff slice."
                }

    List.ofSeq edits, List.ofSeq rude

let diffImplementationFile (g: TcGlobals) (baseline: CheckedImplFile) (updated: CheckedImplFile) =
    if obj.ReferenceEquals(baseline, updated) then
        emptyDiff
    else
        let denv = DisplayEnv.Empty g
        let baselineBindings, baselineEntities = collectSnapshots g denv baseline
        let updatedBindings, updatedEntities = collectSnapshots g denv updated

        let semanticEdits, bindingRudeEdits =
            compareBindings baselineBindings updatedBindings

        let entityEdits, entityRudeEdits = compareEntities baselineEntities updatedEntities

        {
            SemanticEdits = semanticEdits @ entityEdits
            RudeEdits = bindingRudeEdits @ entityRudeEdits
        }
