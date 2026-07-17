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

let private hashCombine (seed: int) (value: int) = (seed * 16777619) ^^^ value

let private hashList (items: seq<int>) =
    let mutable acc = 1

    for item in items do
        acc <- hashCombine acc item

    acc

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

let private tyToString (_: DisplayEnv) (ty: TType) = normalizeTypeString (ty.ToString())

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

let private opDigest (op: TOp) = op.ToString()

let rec private exprDigest (denv: DisplayEnv) (expr: Expr) =
    let recurse = exprDigest denv

    match expr with
    | Expr.Const(c, _, ty) -> [ 1; stableHash (constDigest c); stableHash (tyToString denv ty) ] |> hashList
    | Expr.Val(vref, _, _) ->
        let referenceHash =
            match tryStableValReferenceIdentity vref with
            | Some identity -> stableHash identity
            | None -> stableHash $"local:{vref.LogicalName}|ty={tyToString denv vref.Type}"

        hashCombine 2 referenceHash
    | Expr.App(funcExpr, _, typeArgs, args, _) ->
        let funcHash = recurse funcExpr
        let argHash = args |> Seq.map recurse |> hashList
        let typeHash = typeArgs |> Seq.map (tyToString denv >> stableHash) |> hashList
        [ 3; funcHash; argHash; typeHash ] |> hashList
    | Expr.Sequential(expr1, expr2, _, _) -> hashCombine (hashCombine 4 (recurse expr1)) (recurse expr2)
    | Expr.Lambda(_, _, _, valParams, bodyExpr, _, _) ->
        let paramsHash =
            valParams
            |> Seq.map (fun v -> stableHash $"{v.LogicalName}:{tyToString denv v.Type}")
            |> hashList

        hashCombine (hashCombine 5 paramsHash) (recurse bodyExpr)
    | Expr.TyLambda(_, typars, bodyExpr, _, _) ->
        let typarHash = typars |> Seq.map (fun tp -> stableHash tp.DisplayName) |> hashList
        hashCombine (hashCombine 6 typarHash) (recurse bodyExpr)
    | Expr.Let(binding, bodyExpr, _, _) -> hashCombine (hashCombine 7 (bindingDigest denv binding)) (recurse bodyExpr)
    | Expr.LetRec(bindings, bodyExpr, _, _) ->
        let bindsHash = bindings |> Seq.map (bindingDigest denv) |> hashList
        hashCombine (hashCombine 8 bindsHash) (recurse bodyExpr)
    | Expr.Match(_, _, _, targets, _, _) ->
        let targetHash =
            targets
            |> Array.map (fun (TTarget(boundVals, targetExpr, _)) ->
                let valsHash = boundVals |> Seq.map (fun v -> stableHash v.LogicalName) |> hashList
                hashCombine valsHash (recurse targetExpr))
            |> hashList

        hashCombine 9 targetHash
    | Expr.Op(op, typeArgs, args, _) ->
        [
            10
            stableHash (opDigest op)
            args |> Seq.map recurse |> hashList
            typeArgs |> Seq.map (tyToString denv >> stableHash) |> hashList
        ]
        |> hashList
    | Expr.Obj(_, objTy, _, ctorCall, overrides, interfaceImpls, _) ->
        let overridesHash =
            overrides
            |> Seq.map (fun (TObjExprMethod(_, _, _, _, body, _)) -> recurse body)
            |> hashList

        let interfaceHash =
            interfaceImpls
            |> Seq.map (fun (_, methods) ->
                methods
                |> Seq.map (fun (TObjExprMethod(_, _, _, _, body, _)) -> recurse body)
                |> hashList)
            |> hashList

        [
            11
            stableHash (tyToString denv objTy)
            recurse ctorCall
            overridesHash
            interfaceHash
        ]
        |> hashList
    | Expr.Quote(quotedExpr, _, _, _, _) -> hashCombine 12 (recurse quotedExpr)
    | Expr.DebugPoint(_, body) -> recurse body
    | Expr.Link eref -> recurse eref.Value
    | Expr.TyChoose(typars, bodyExpr, _) ->
        let typarHash = typars |> Seq.map (fun tp -> stableHash tp.DisplayName) |> hashList
        hashCombine (hashCombine 13 typarHash) (recurse bodyExpr)
    | Expr.WitnessArg(traitInfo, _) -> hashCombine 14 (stableHash traitInfo.MemberLogicalName)
    | Expr.StaticOptimization(_, onExpr, elseExpr, _) -> hashCombine (hashCombine 15 (recurse onExpr)) (recurse elseExpr)

and private bindingDigest denv (TBind(var, body, _)) =
    let sigHash = tyToString denv var.Type |> stableHash
    hashCombine sigHash (exprDigest denv body)

type private BindingSnapshot =
    {
        Symbol: SymbolId
        InlineInfo: ValInline
        SignatureText: string
        ConstraintsText: string
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

    let totalArgCount =
        snapshot.Symbol.TotalArgCount |> Option.map string |> Option.defaultValue ""

    let genericArity =
        snapshot.Symbol.GenericArity |> Option.map string |> Option.defaultValue ""

    $"{snapshot.Symbol.QualifiedName}|entity={entityKey}|kind={memberKind}|compiled={compiledName}|args={totalArgCount}|gen={genericArity}"

let private entityKey (snapshot: EntitySnapshot) = snapshot.Symbol.QualifiedName

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

    let genericArity = var.ValReprInfo |> Option.map (fun info -> info.NumTypars)

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
        BodyHash = exprDigest denv expr
        IsSynthesized = var.IsCompilerGenerated
        ContainingEntity = containingEntity
    }

and private snapshotTycon denv path (tycon: Tycon) =
    let reprText =
        let sb = StringBuilder()
        sb.Append("kind:").Append(tycon.TypeOrMeasureKind.ToString()) |> ignore

        match tycon.TypeReprInfo with
        | TFSharpTyconRepr data ->
            sb.Append("|fs-kind:").Append(data.fsobjmodel_kind.ToString()) |> ignore

            match data.fsobjmodel_kind with
            | FSharpTyconKind.TFSharpUnion ->
                data.fsobjmodel_cases.UnionCasesAsList
                |> List.iter (fun case ->
                    sb.Append("|case:").Append(case.LogicalName) |> ignore

                    case.FieldTable.FieldsByIndex
                    |> Array.iter (fun field ->
                        sb.Append(":").Append(field.LogicalName).Append("=").Append(tyToString denv field.FormalType)
                        |> ignore))
            | FSharpTyconKind.TFSharpRecord
            | FSharpTyconKind.TFSharpStruct
            | FSharpTyconKind.TFSharpClass
            | FSharpTyconKind.TFSharpInterface
            | FSharpTyconKind.TFSharpEnum ->
                data.fsobjmodel_rfields.FieldsByIndex
                |> Array.iter (fun field ->
                    sb.Append("|field:").Append(field.LogicalName) |> ignore

                    if field.IsMutable then
                        sb.Append("[mutable]") |> ignore

                    sb.Append("=").Append(tyToString denv field.FormalType) |> ignore

                    field.LiteralValue
                    |> Option.iter (fun value -> sb.Append("[literal:").Append(constDigest value).Append("]") |> ignore))
            | FSharpTyconKind.TFSharpDelegate slotSig -> sb.Append("|delegate:").Append(slotSig.Name) |> ignore
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
    elif baseline.InlineInfo <> updated.InlineInfo then
        Choice2Of2
            {
                Symbol = Some baseline.Symbol
                Kind = RudeEditKind.InlineChange
                Message = "Inline annotation changed."
            }
    elif baseline.BodyHash <> updated.BodyHash then
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
    |> List.iter (fun added -> edits.Add(semanticEdit added SemanticEditKind.Insert None (Some added.BodyHash)))

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
            |> List.iter (fun added -> edits.Add(semanticEdit added SemanticEditKind.Insert None (Some added.BodyHash)))

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
            edits.Add
                {
                    Symbol = updatedEntity.Symbol
                    Kind = SemanticEditKind.Insert
                    BaselineHash = None
                    UpdatedHash = Some updatedEntity.RepresentationHash
                    IsSynthesized = updatedEntity.Symbol.IsSynthesized
                    ContainingEntity = updatedEntity.CompiledFullName
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
