// Copyright (c) Microsoft Corporation. All Rights Reserved.

module internal FSharp.Compiler.TypedTreeDiff

open System
open System.Collections.Generic
open System.Text

open Internal.Utilities.Collections
open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.EditAndContinue
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps

/// Describes the high-level category for a symbol participating in a hot reload edit.
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

[<RequireQualifiedAccess>]
[<StructuralEquality; StructuralComparison>]
/// Typed runtime method-signature identity transported from typed-tree diff into delta mapping.
/// This avoids string-only parameter/return matching in DeltaBuilder.
type RuntimeTypeIdentity =
    | NamedType of fullName: string * genericArguments: RuntimeTypeIdentity list
    | ArrayType of rank: int * elementType: RuntimeTypeIdentity
    | ByRefType of elementType: RuntimeTypeIdentity
    | PointerType of elementType: RuntimeTypeIdentity
    | FunctionPointerType of returnType: RuntimeTypeIdentity * argumentTypes: RuntimeTypeIdentity list
    | TypeVariable of ordinal: int
    | VoidType

/// Stable identity for values and entities tracked across baseline/hot reload sessions.
type SymbolId =
    { Path: string list
      LogicalName: string
      Stamp: Stamp
      Kind: SymbolKind
      MemberKind: SymbolMemberKind option
      IsSynthesized: bool
      CompiledName: string option
      TotalArgCount: int option
      GenericArity: int option
      ParameterTypeIdentities: RuntimeTypeIdentity list option
      ReturnTypeIdentity: RuntimeTypeIdentity option }

    member x.QualifiedName =
        match x.Path with
        | [] -> x.LogicalName
        | path -> String.concat "." (path @ [ x.LogicalName ])

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
    | LambdaShapeChange
    | StateMachineShapeChange
    | QueryExpressionShapeChange
    | SynthesizedDeclarationChange
    | Unsupported
    // Method addition restrictions (following Roslyn patterns)
    | InsertVirtual           // Virtual/abstract/override methods cannot be added
    | InsertConstructor       // Constructors cannot be added to existing types
    | InsertOperator          // User-defined operators cannot be added
    | InsertExplicitInterface // Explicit interface implementations cannot be added
    | InsertIntoInterface     // Members cannot be added to interfaces
    | FieldAdded              // Fields cannot be added (type layout change)
    // The edit itself is valid for hot reload, but the runtime did not advertise the
    // capability required to apply it (mirrors Roslyn's RudeEditKind.NotSupportedByRuntime).
    | NotSupportedByRuntime

/// The category of declaration being added to an existing type or module, used to
/// determine which runtime capability gates emission of the addition.
[<RequireQualifiedAccess>]
type AdditionKind =
    | Method
    | InstanceField
    | StaticField

/// Single seam consulted by edit classification: the runtime capability required to apply an
/// addition of the given kind. Phase B flips further addition kinds (fields, new types) from
/// always-rude to capability-gated by implementing their emission and routing them through here.
let capabilityForAddition (kind: AdditionKind) : EditAndContinueCapability =
    match kind with
    | AdditionKind.Method -> EditAndContinueCapability.AddMethodToExistingType
    | AdditionKind.InstanceField -> EditAndContinueCapability.AddInstanceFieldToExistingType
    | AdditionKind.StaticField -> EditAndContinueCapability.AddStaticFieldToExistingType

type SemanticEdit =
    { Symbol: SymbolId
      Kind: SemanticEditKind
      BaselineHash: int option
      UpdatedHash: int option
      IsSynthesized: bool
      ContainingEntity: string option }

type RudeEdit =
    { Symbol: SymbolId option
      Kind: RudeEditKind
      Message: string }

// ---------------------------------------------------------------------------
// Lambda occurrence model (Phase C1)
//
// Each member body is summarized as an ordered sequence of lambda OCCURRENCES.
// Identity is structural and positional (traversal ordinal + enclosing-lambda
// chain + structural digest); source ranges are recorded for diagnostics only
// and never participate in identity, so unrelated line edits cannot perturb
// occurrence matching (Roslyn equivalent: syntax-offset matching through the
// SyntaxMap rather than absolute source positions).
// ---------------------------------------------------------------------------

/// Identity of a single lambda occurrence within a member body.
type LambdaOccurrenceId =
    { /// The member whose body contains this lambda occurrence.
      MemberSymbol: SymbolId
      /// Pre-order traversal index of this occurrence among all lambda occurrences
      /// extracted from the member body (consecutive curried lambdas form ONE occurrence).
      Ordinal: int
      /// Ordinals of the enclosing lambda occurrences, nearest enclosing first.
      /// Empty for occurrences directly inside the member body.
      ParentChain: int list }

/// Identity of a value captured by a lambda occurrence from an enclosing scope:
/// logical (source) name plus runtime type identity. Mirrors Roslyn's display-class
/// field matching, which keys captured variables by name and type. Self captures are
/// normalized to 'this'/'base' so renaming the F# self identifier is not a capture rename.
type CaptureIdentity =
    { /// Source-level logical name of the captured value.
      LogicalName: string
      /// Runtime type identity of the captured value.
      Type: RuntimeTypeIdentity }

/// A lambda occurrence extracted from a member body. Consecutive curried lambdas
/// (fun x -> fun y -> ...) are merged into one occurrence, matching how IlxGen forms
/// a single closure class for a curried lambda chain.
type LambdaOccurrence =
    { /// Structural/positional identity of this occurrence.
      Id: LambdaOccurrenceId
      /// Number of consecutive curried lambda groups merged into this occurrence.
      CurriedArity: int
      /// Runtime type identities of the parameters in each curried group. Part of the
      /// structural digest so parameter-shape changes never silently align (the legacy
      /// digest treated them as shape changes, and so do we).
      ParameterTypes: RuntimeTypeIdentity list list
      /// Values captured from enclosing scopes (free locals of the occurrence expression
      /// that are not module/member-level), ordered deterministically by identity.
      Captures: CaptureIdentity list
      /// Runtime type identity of the value produced after all curried groups are applied.
      ReturnTypeIdentity: RuntimeTypeIdentity
      /// Digest of the occurrence expression, used to detect pure body edits between two
      /// occurrences that share the same structural digest. Never part of identity.
      BodyHash: int
      /// Source range of the occurrence. Diagnostics only — never identity.
      Range: range }

/// A change to the captured-value set of a matched lambda occurrence pair, classified
/// with C#-parity kinds (RenamingCapturedVariable / ChangingCapturedVariableType /
/// ChangingCapturedVariableScope). All of these are permanently rude edits; capture
/// additions are also rude today but may become applicable in Phase C4 via the
/// AddInstanceFieldToExistingType runtime capability.
[<RequireQualifiedAccess>]
type CaptureSetChange =
    /// A captured value kept its type but changed its logical name
    /// (C# parity: RenamingCapturedVariable).
    | Renamed of oldName: string * newName: string * captureType: RuntimeTypeIdentity
    /// A captured value kept its logical name but changed its type
    /// (C# parity: ChangingCapturedVariableType).
    | TypeChanged of name: string * oldType: RuntimeTypeIdentity * newType: RuntimeTypeIdentity
    /// A captured value moved between lambda occurrences: it stopped being captured by
    /// one occurrence and started being captured by another in the same member
    /// (C# parity: ChangingCapturedVariableScope).
    | ScopeChanged of name: string * captureType: RuntimeTypeIdentity
    /// The occurrence captures an additional value it did not capture before.
    | CaptureAdded of capture: CaptureIdentity
    /// The occurrence no longer captures a value it captured before.
    | CaptureRemoved of capture: CaptureIdentity

/// A classified edit for a single lambda occurrence, produced by aligning the baseline
/// and updated occurrence sequences of a matched member. Matched pairs always carry both
/// occurrences so downstream consumers (the Phase C4 emitter and its verifier) never have
/// to re-derive the pairing.
[<RequireQualifiedAccess>]
type LambdaEdit =
    /// The occurrence matched structurally and only its body changed. This is the
    /// hot-reload-compatible case: the member body update covers it.
    | BodyEdited of baseline: LambdaOccurrence * updated: LambdaOccurrence
    /// The occurrence exists only in the updated member body.
    | Added of updated: LambdaOccurrence
    /// The occurrence exists only in the baseline member body.
    | Removed of baseline: LambdaOccurrence
    /// The occurrence matched on shape (arity, parameter and return identity, position)
    /// but its captured-value set is incompatible.
    | CaptureSetChanged of baseline: LambdaOccurrence * updated: LambdaOccurrence * changes: CaptureSetChange list

/// Lambda edits for a single member, keyed by the member symbol. Carried alongside the
/// semantic/rude edits so Phase C4 emission can consume the structured data without
/// re-running occurrence extraction.
type MemberLambdaEdits =
    { /// The member whose body produced these lambda edits (baseline-side symbol).
      MemberSymbol: SymbolId
      /// Aligned per-occurrence edits, ordered by occurrence ordinal.
      Edits: LambdaEdit list }

type TypedTreeDiffResult =
    { SemanticEdits: SemanticEdit list
      RudeEdits: RudeEdit list
      /// Structured lambda occurrence edits for members whose lambda sets were analyzed,
      /// including members that produced rude edits (the payload names exactly which
      /// occurrences were added/removed/capture-incompatible).
      LambdaEdits: MemberLambdaEdits list }

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

let private stableHash (text: string) =
    if String.IsNullOrEmpty text then
        0
    else
        // FNV-1a hash for better collision resistance
        let mutable hash = 2166136261u // FNV offset basis

        for ch in text do
            hash <- (hash ^^^ uint32 ch) * 16777619u // FNV prime

        int hash

let private hashCombine (seed: int) (value: int) = (seed * 16777619) ^^^ value

let private hashList (items: seq<int>) =
    let mutable acc = 1

    for item in items do
        acc <- hashCombine acc item

    acc

let private traceHotReloadMethodDiff =
    match Environment.GetEnvironmentVariable("FSHARP_HOTRELOAD_TRACE_METHODS") with
    | null -> false
    | value when String.Equals(value, "1", StringComparison.OrdinalIgnoreCase) -> true
    | value when String.Equals(value, "true", StringComparison.OrdinalIgnoreCase) -> true
    | _ -> false

let private propertyDisplayName (vref: ValRef) =
    let name = vref.PropertyName
    if String.IsNullOrWhiteSpace name then vref.DisplayName else name

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
            Some(parent.CompiledRepresentationForNamedType.FullName)
        with _ ->
            try
                Some(parent.CompiledName)
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
    | None when vref.IsCompiledAsTopLevel -> Some(baseIdentity)
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

let private tyToString (_: DisplayEnv) (ty: TType) =
    normalizeTypeString (ty.ToString())

let private runtimeNamedTypeIdentity (typeName: string) (args: RuntimeTypeIdentity list) =
    RuntimeTypeIdentity.NamedType(typeName, args)

/// Encodes typed-tree parameter types into a typed runtime identity model that mirrors
/// IL signature structure closely enough for structural token matching in DeltaBuilder.
let rec private tryTypeIdentityFromTType
    (g: TcGlobals)
    (typarOrdinals: Map<Stamp, int>)
    (ty: TType)
    : RuntimeTypeIdentity option =
    let ty = stripTyEqnsAndMeasureEqns g ty

    let tryEncodeGenericArgs (args: TType list) =
        let encoded = args |> List.map (tryTypeIdentityFromTType g typarOrdinals)

        if encoded |> List.exists Option.isNone then
            None
        else
            Some(encoded |> List.choose id)

    match ty with
    | TType_forall(_, bodyTy) -> tryTypeIdentityFromTType g typarOrdinals bodyTy
    | _ when isVoidTy g ty -> Some RuntimeTypeIdentity.VoidType
    | _ when isArrayTy g ty ->
        let rank = rankOfArrayTy g ty
        let elementType = destArrayTy g ty
        tryTypeIdentityFromTType g typarOrdinals elementType
        |> Option.map (fun elementIdentity -> RuntimeTypeIdentity.ArrayType(rank, elementIdentity))
    | _ when isByrefTy g ty ->
        let elementType = destByrefTy g ty
        tryTypeIdentityFromTType g typarOrdinals elementType
        |> Option.map RuntimeTypeIdentity.ByRefType
    | _ when isNativePtrTy g ty ->
        let elementType = destNativePtrTy g ty
        tryTypeIdentityFromTType g typarOrdinals elementType
        |> Option.map RuntimeTypeIdentity.PointerType
    | TType_app(tcref, tinst, _) ->
        let fullName =
            try
                tcref.CompiledRepresentationForNamedType.FullName
            with _ ->
                tcref.CompiledName

        tryEncodeGenericArgs tinst
        |> Option.map (runtimeNamedTypeIdentity fullName)
    | TType_anon(anonInfo, tys) ->
        tryEncodeGenericArgs tys
        |> Option.map (runtimeNamedTypeIdentity anonInfo.ILTypeRef.FullName)
    | TType_tuple(tupInfo, tys) ->
        let tupleName =
            if evalTupInfoIsStruct tupInfo then
                $"System.ValueTuple`{List.length tys}"
            else
                $"System.Tuple`{List.length tys}"

        tryEncodeGenericArgs tys
        |> Option.map (runtimeNamedTypeIdentity tupleName)
    | TType_fun(domainTy, rangeTy, _) ->
        match tryTypeIdentityFromTType g typarOrdinals domainTy, tryTypeIdentityFromTType g typarOrdinals rangeTy with
        | Some domainIdentity, Some rangeIdentity ->
            Some(runtimeNamedTypeIdentity "Microsoft.FSharp.Core.FSharpFunc`2" [ domainIdentity; rangeIdentity ])
        | _ -> None
    | TType_ucase(ucref, tinst) ->
        let fullName =
            try
                ucref.TyconRef.CompiledRepresentationForNamedType.FullName
            with _ ->
                ucref.TyconRef.CompiledName

        tryEncodeGenericArgs tinst
        |> Option.map (runtimeNamedTypeIdentity fullName)
    | TType_var (typar, _) ->
        Map.tryFind typar.Stamp typarOrdinals
        |> Option.map RuntimeTypeIdentity.TypeVariable
    | TType_measure _ -> None

let private tryGetMethodTyparOrdinalsAndGenericArity (g: TcGlobals) (var: Val) =
    match var.ValReprInfo with
    | None -> None
    | Some valReprInfo ->
        let numEnclosingTypars = CountEnclosingTyparsOfActualParentOfVal var
        let tps, _, _, _, _ = GetValReprTypeInCompiledForm g valReprInfo numEnclosingTypars var.Type var.Range
        let nonErasedTypars = tps |> List.filter (fun typar -> not typar.IsErased)

        // Keep typar ordinals aligned with IL generation (TypeReprEnv.Add drops erased typars).
        let typarOrdinals =
            nonErasedTypars
            |> List.mapi (fun ordinal typar -> typar.Stamp, ordinal)
            |> Map.ofList

        // Split method typars using the compiled-form enclosing count (same partition used by IlxGen).
        let methodTypars =
            if numEnclosingTypars <= tps.Length then
                tps |> List.skip numEnclosingTypars
            else
                []

        let methodGenericArity = methodTypars |> List.filter (fun typar -> not typar.IsErased) |> List.length
        Some(typarOrdinals, methodGenericArity)

let private tryGetParameterTypeIdentities (g: TcGlobals) (typarOrdinals: Map<Stamp, int>) (var: Val) =
    let parameterTypes =
        match var.MemberInfo, var.ValReprInfo with
        | Some _, _ ->
            ArgInfosOfMember g (mkLocalValRef var)
            |> List.concat
            |> List.map fst
        | None, Some valReprInfo ->
            let _, argInfos, _, _ = GetValReprTypeInFSharpForm g valReprInfo var.Type var.Range
            argInfos
            |> List.concat
            |> List.map fst
        | None, None -> []

    let encoded = parameterTypes |> List.map (tryTypeIdentityFromTType g typarOrdinals)

    if encoded |> List.forall Option.isSome then
        Some(encoded |> List.choose id)
    else
        None

let private tryGetReturnTypeIdentity (g: TcGlobals) (typarOrdinals: Map<Stamp, int>) (var: Val) =
    match var.ValReprInfo with
    | Some valReprInfo ->
        let numEnclosingTypars = CountEnclosingTyparsOfActualParentOfVal var
        let _, _, _, returnTy, _ = GetValReprTypeInCompiledForm g valReprInfo numEnclosingTypars var.Type var.Range

        match returnTy with
        | None -> Some RuntimeTypeIdentity.VoidType
        | Some ty -> tryTypeIdentityFromTType g typarOrdinals ty
    | None -> None

/// Generates a stable digest of type parameter constraints for change detection.
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

/// Generates a stable digest of all type parameter constraints for a value.
let private typarConstraintsDigest (denv: DisplayEnv) (typars: Typar list) =
    if List.isEmpty typars then
        ""
    else
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

/// Generates a stable digest for TOp operations, handling F#-specific constructs
/// that have non-informative ToString() output.
let private opDigest (denv: DisplayEnv) (op: TOp) =
    match op with
    | TOp.UnionCase ucref -> "UnionCase:" + ucref.CaseName
    | TOp.ExnConstr ecref -> "ExnConstr:" + ecref.LogicalName
    | TOp.Tuple (TupInfo.Const isStruct) ->
        let kind = if isStruct then "struct" else "ref"
        "Tuple:" + kind
    | TOp.AnonRecd anonInfo ->
        // Include anonymous record field names for stability
        let fields = anonInfo.SortedNames |> String.concat ","
        "AnonRecd:" + fields
    | TOp.AnonRecdGet (anonInfo, idx) ->
        let fields = anonInfo.SortedNames |> String.concat ","
        "AnonRecdGet:" + fields + ":" + string idx
    | TOp.Array -> "Array"
    | TOp.Bytes bytes ->
        // Hash the actual byte content
        let bytesHash = bytes |> Array.fold (fun acc b -> hashCombine acc (int b)) 17
        "Bytes:" + string bytesHash
    | TOp.UInt16s arr ->
        // Hash the actual uint16 content
        let arrHash = arr |> Array.fold (fun acc v -> hashCombine acc (int v)) 17
        "UInt16s:" + string arrHash
    | TOp.While (_, marker) -> "While:" + string marker
    | TOp.IntegerForLoop (_, _, style) -> "IntegerForLoop:" + string style
    | TOp.TryWith _ -> "TryWith"
    | TOp.TryFinally _ -> "TryFinally"
    | TOp.Recd (info, tcref) -> "Recd:" + string info + ":" + tcref.LogicalName
    | TOp.ValFieldSet rfref -> "ValFieldSet:" + rfref.FieldName
    | TOp.ValFieldGet rfref -> "ValFieldGet:" + rfref.FieldName
    | TOp.ValFieldGetAddr (rfref, readonly) -> "ValFieldGetAddr:" + rfref.FieldName + ":" + string readonly
    | TOp.UnionCaseTagGet tcref -> "UnionCaseTagGet:" + tcref.LogicalName
    | TOp.UnionCaseProof ucref -> "UnionCaseProof:" + ucref.CaseName
    | TOp.UnionCaseFieldGet (ucref, idx) -> "UnionCaseFieldGet:" + ucref.CaseName + ":" + string idx
    | TOp.UnionCaseFieldGetAddr (ucref, idx, readonly) ->
        "UnionCaseFieldGetAddr:" + ucref.CaseName + ":" + string idx + ":" + string readonly
    | TOp.UnionCaseFieldSet (ucref, idx) -> "UnionCaseFieldSet:" + ucref.CaseName + ":" + string idx
    | TOp.ExnFieldGet (tcref, idx) -> "ExnFieldGet:" + tcref.LogicalName + ":" + string idx
    | TOp.ExnFieldSet (tcref, idx) -> "ExnFieldSet:" + tcref.LogicalName + ":" + string idx
    | TOp.TupleFieldGet (TupInfo.Const isStruct, idx) ->
        let kind = if isStruct then "struct" else "ref"
        "TupleFieldGet:" + kind + ":" + string idx
    | TOp.ILAsm (instrs, retTypes) ->
        let instrsStr = instrs |> List.map (fun i -> i.ToString()) |> String.concat ";"
        let retStr = retTypes |> List.map (tyToString denv) |> String.concat ","
        "ILAsm:" + instrsStr + ":" + retStr
    | TOp.RefAddrGet readonly -> "RefAddrGet:" + string readonly
    | TOp.Coerce -> "Coerce"
    | TOp.Reraise -> "Reraise"
    | TOp.Return -> "Return"
    | TOp.Goto label -> "Goto:" + string label
    | TOp.Label label -> "Label:" + string label
    | TOp.TraitCall traitInfo -> "TraitCall:" + traitInfo.MemberLogicalName
    | TOp.LValueOp (lvOp, vref) -> "LValueOp:" + string lvOp + ":" + vref.LogicalName
    | TOp.ILCall (isVirtual, isProtected, isStruct, isCtor, valUseFlag, isProperty, noTailCall, ilMethRef, _, _, _) ->
        "ILCall:" + string isVirtual + ":" + string isProtected + ":" + string isStruct + ":" +
        string isCtor + ":" + string valUseFlag + ":" + string isProperty + ":" + string noTailCall +
        ":" + ilMethRef.DeclaringTypeRef.FullName + "." + ilMethRef.Name

type private LoweredShapeCollector =
    { LambdaArities: ResizeArray<int>
      StateMachineStructuralOperations: ResizeArray<string>
      QueryStructuralOperations: ResizeArray<string> }

let private traitConstraintShapeDigest (denv: DisplayEnv) (traitInfo: TraitConstraintInfo) =
    // Capture a structural trait-call fingerprint for lowered-shape classification.
    // This tracks new builder operations without depending solely on member-name
    // heuristic lists that are brittle across compiler/runtime changes.
    let supportTypes =
        traitInfo.SupportTypes
        |> List.map (tyToString denv)
        |> String.concat ","

    let argumentTypes =
        traitInfo.GetCompiledArgumentTypes()
        |> List.map (tyToString denv)
        |> String.concat ","

    let returnType =
        traitInfo.CompiledReturnType
        |> Option.map (tyToString denv)
        |> Option.defaultValue "System.Void"

    $"member={traitInfo.MemberLogicalName}|kind={traitInfo.MemberFlags.MemberKind}|instance={traitInfo.MemberFlags.IsInstance}|support=[{supportTypes}]|args=[{argumentTypes}]|ret={returnType}"

let private addDistinct (items: ResizeArray<string>) (value: string) =
    if not (String.IsNullOrEmpty value) && not (items.Contains value) then
        items.Add value

let private formatLoweredShapeDigest (structural: ResizeArray<string>) =
    let structuralDigest =
        structural
        |> Seq.sort
        |> String.concat ","

    $"struct=[{structuralDigest}]"

let private collectLoweredShapeInfo (denv: DisplayEnv) (expr: Expr) =
    let collector =
        { LambdaArities = ResizeArray()
          StateMachineStructuralOperations = ResizeArray()
          QueryStructuralOperations = ResizeArray() }

    let rec walk (expr: Expr) =
        match expr with
        | Expr.Const _ -> ()
        | Expr.Val (vref, _, _) ->
            // Keep state-machine classification tied to structural evidence from
            // compiler-generated MoveNext references instead of fragile name lists.
            if vref.LogicalName.Equals("MoveNext", StringComparison.Ordinal) then
                addDistinct collector.StateMachineStructuralOperations vref.LogicalName
        | Expr.App (funcExpr, _, _, args, _) ->
            walk funcExpr
            args |> List.iter walk
        | Expr.Sequential (expr1, expr2, _, _) ->
            walk expr1
            walk expr2
        | Expr.Lambda (_, _, _, valParams, bodyExpr, _, _) ->
            collector.LambdaArities.Add(valParams.Length)
            walk bodyExpr
        | Expr.TyLambda (_, _, bodyExpr, _, _) ->
            walk bodyExpr
        | Expr.Let (binding, bodyExpr, _, _) ->
            let (TBind (_, bindingExpr, _)) = binding
            walk bindingExpr
            walk bodyExpr
        | Expr.LetRec (bindings, bodyExpr, _, _) ->
            bindings
            |> List.iter (fun (TBind (_, bindingExpr, _)) -> walk bindingExpr)
            walk bodyExpr
        | Expr.Match (_, _, _, targets, _, _) ->
            targets
            |> Array.iter (fun (TTarget(_, targetExpr, _)) -> walk targetExpr)
        | Expr.Op (op, _, args, _) ->
            match op with
            | TOp.TryWith _ -> addDistinct collector.StateMachineStructuralOperations "TryWith"
            | TOp.TryFinally _ -> addDistinct collector.StateMachineStructuralOperations "TryFinally"
            | TOp.While _ -> addDistinct collector.StateMachineStructuralOperations "While"
            | TOp.IntegerForLoop _ -> addDistinct collector.StateMachineStructuralOperations "ForLoop"
            | TOp.TraitCall traitInfo ->
                let traitDigest = traitConstraintShapeDigest denv traitInfo
                addDistinct collector.QueryStructuralOperations traitDigest
            | _ -> ()

            args |> List.iter walk
        | Expr.Obj (_, _, _, ctorCall, overrides, interfaceImpls, _) ->
            walk ctorCall

            overrides
            |> List.iter (fun (TObjExprMethod(_, _, _, _, body, _)) -> walk body)

            interfaceImpls
            |> List.iter (fun (_, methods) ->
                methods
                |> List.iter (fun (TObjExprMethod(_, _, _, _, body, _)) -> walk body))
        | Expr.Quote (quotedExpr, _, _, _, _) ->
            walk quotedExpr
        | Expr.DebugPoint (_, body) ->
            walk body
        | Expr.Link eref ->
            walk eref.Value
        | Expr.TyChoose (_, bodyExpr, _) ->
            walk bodyExpr
        | Expr.WitnessArg (traitInfo, _) ->
            let traitDigest = traitConstraintShapeDigest denv traitInfo
            addDistinct collector.QueryStructuralOperations traitDigest
        | Expr.StaticOptimization (_, onExpr, elseExpr, _) ->
            walk onExpr
            walk elseExpr

    walk expr

    let lambdaDigest =
        collector.LambdaArities
        |> Seq.map string
        |> String.concat ","

    let stateMachineDigest =
        formatLoweredShapeDigest collector.StateMachineStructuralOperations

    let queryDigest =
        formatLoweredShapeDigest collector.QueryStructuralOperations

    lambdaDigest, stateMachineDigest, queryDigest

let rec private exprDigest (denv: DisplayEnv) (expr: Expr) =
    let recurse = exprDigest denv

    match expr with
    | Expr.Const (c, _, ty) ->
        [ 1
          stableHash (constDigest c)
          stableHash (tyToString denv ty) ]
        |> hashList
    | Expr.Val (vref, _, _) ->
        // References to top-level values/members hash by compiled identity rather than stamps.
        // This keeps caller hashes stable when callees are recompiled with new stamps.
        let referenceHash =
            match tryStableValReferenceIdentity vref with
            | Some identity -> stableHash identity
            | None ->
                // Local/parameter stamps are reallocated each compilation.
                // Hash by logical identity so unchanged method bodies stay stable across generations.
                stableHash $"local:{vref.LogicalName}|ty={tyToString denv vref.Type}"

        hashCombine 2 referenceHash
    | Expr.App (funcExpr, _, _, args, _) ->
        let funcHash = recurse funcExpr
        let argHash = args |> Seq.map recurse |> hashList
        hashCombine (hashCombine 3 funcHash) argHash
    | Expr.Sequential (expr1, expr2, _, _) ->
        hashCombine (hashCombine 4 (recurse expr1)) (recurse expr2)
    | Expr.Lambda (_, _, _, valParams, bodyExpr, _, _) ->
        let paramsHash =
            valParams
            |> Seq.map (fun v -> stableHash v.LogicalName)
            |> hashList

        hashCombine (hashCombine 5 paramsHash) (recurse bodyExpr)
    | Expr.TyLambda (_, typars, bodyExpr, _, _) ->
        let typarHash =
            typars
            |> Seq.map (fun tp -> stableHash tp.DisplayName)
            |> hashList

        hashCombine (hashCombine 6 typarHash) (recurse bodyExpr)
    | Expr.Let (binding, bodyExpr, _, _) ->
        let bindHash = bindingDigest denv binding
        hashCombine (hashCombine 7 bindHash) (recurse bodyExpr)
    | Expr.LetRec (bindings, bodyExpr, _, _) ->
        let bindsHash =
            bindings
            |> Seq.map (bindingDigest denv)
            |> hashList

        hashCombine (hashCombine 8 bindsHash) (recurse bodyExpr)
    | Expr.Match (_, _, _, targets, _, _) ->
        let targetsHash =
            targets
            |> Array.map (fun tgt ->
                match tgt with
                | TTarget(boundVals, targetExpr, _) ->
                    let valsHash =
                        boundVals
                        |> Seq.map (fun v -> stableHash v.LogicalName)
                        |> hashList

                    hashCombine valsHash (recurse targetExpr))
            |> hashList

        hashCombine 9 targetsHash
    | Expr.Op (op, typeArgs, args, _) ->
        let opHash = stableHash (opDigest denv op)
        let argsHash = args |> Seq.map recurse |> hashList
        let tyHash =
            typeArgs
            |> Seq.map (tyToString denv >> stableHash)
            |> hashList

        [ 10; opHash; argsHash; tyHash ] |> hashList
    | Expr.Obj (_, objTy, _, ctorCall, overrides, interfaceImpls, _) ->
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

        [ 11
          stableHash (tyToString denv objTy)
          recurse ctorCall
          overridesHash
          interfaceHash ]
        |> hashList
    | Expr.Quote (quotedExpr, _, _, _, _) ->
        hashCombine 12 (recurse quotedExpr)
    | Expr.DebugPoint (_, body) ->
        recurse body
    | Expr.Link eref ->
        recurse eref.Value
    | Expr.TyChoose (typars, bodyExpr, _) ->
        let typarHash =
            typars
            |> Seq.map (fun tp -> stableHash tp.DisplayName)
            |> hashList

        hashCombine (hashCombine 13 typarHash) (recurse bodyExpr)
    | Expr.WitnessArg (traitInfo, _) ->
        hashCombine 14 (stableHash traitInfo.MemberLogicalName)
    | Expr.StaticOptimization (_, onExpr, elseExpr, _) ->
        hashCombine (hashCombine 15 (recurse onExpr)) (recurse elseExpr)

and private bindingDigest denv (TBind (var, body, _)) =
    let sigHash = tyToString denv var.Type |> stableHash
    hashCombine sigHash (exprDigest denv body)

// ---------------------------------------------------------------------------
// Lambda occurrence extraction
// ---------------------------------------------------------------------------

/// Result of extracting lambda occurrences from a member body. Constructs we cannot yet
/// model faithfully (quotations, object expressions, local type functions, types without
/// a computable runtime identity) keep the member on the legacy whole-body digest path,
/// which classifies any lowered-shape change as rude exactly as before this model existed.
[<RequireQualifiedAccess>]
type private LambdaOccurrenceExtraction =
    | Extracted of LambdaOccurrence list
    | Unsupported of reason: string

/// Looks through wrappers that carry no structure of their own.
let rec private stripDebugPointsAndLinks (expr: Expr) =
    match expr with
    | Expr.DebugPoint (_, inner) -> stripDebugPointsAndLinks inner
    | Expr.Link eref -> stripDebugPointsAndLinks eref.Value
    | _ -> expr

/// Strips the member's own top-level type lambdas and (up to) its declared curried
/// argument groups, leaving the body the diff should treat as "inside" the member.
/// The member's own parameter lambdas are not closures; IlxGen strips them the same way
/// via the binding's ValReprInfo before forming closure classes.
let private stripMemberTopLambdas (valReprInfo: ValReprInfo option) (expr: Expr) =
    let rec stripTyLambdas expr =
        match stripDebugPointsAndLinks expr with
        | Expr.TyLambda (_, _, body, _, _) -> stripTyLambdas body
        | stripped -> stripped

    let rec stripLambdas remaining expr =
        if remaining = 0 then
            expr
        else
            match stripDebugPointsAndLinks expr with
            | Expr.Lambda (_, _, _, _, body, _, _) -> stripLambdas (remaining - 1) body
            | stripped -> stripped

    match valReprInfo with
    | Some info -> stripLambdas info.NumCurriedArgs (stripTyLambdas expr)
    | None -> expr

/// Extracts the ordered lambda occurrence sequence from a member body, or reports the
/// member as unsupported for occurrence modelling (in which case the caller falls back
/// to the legacy whole-body lambda digest).
let private extractLambdaOccurrences
    (g: TcGlobals)
    (denv: DisplayEnv)
    (typarOrdinals: Map<Stamp, int>)
    (memberSymbol: SymbolId)
    (valReprInfo: ValReprInfo option)
    (expr: Expr)
    : LambdaOccurrenceExtraction =

    let occurrences = ResizeArray<LambdaOccurrence>()
    let mutable unsupported = None
    let mutable nextOrdinal = 0

    let markUnsupported (reason: string) =
        if unsupported.IsNone then
            unsupported <- Some reason

    let tryTypeIdentity (ty: TType) =
        match tryTypeIdentityFromTType g typarOrdinals ty with
        | Some identity -> Some identity
        | None ->
            markUnsupported "a type without a computable runtime identity"
            None

    // Captures = free locals of the occurrence expression that come from enclosing scopes.
    // Module/member-level values are accessed via static call/field paths in IL, never
    // hoisted into closure fields, so they are not captures.
    let captureIdentities (lambdaExpr: Expr) =
        let freeVars = freeInExpr CollectTyparsAndLocals lambdaExpr

        let captures =
            freeVars.FreeLocals
            |> Zset.elements
            |> List.choose (fun (v: Val) ->
                if v.IsCompiledAsTopLevel || v.IsMemberOrModuleBinding then
                    None
                else
                    // Normalize self captures: the user-chosen self identifier is not part
                    // of the runtime capture identity (it always lowers to the 'this' slot).
                    let logicalName =
                        if v.IsMemberThisVal || v.IsCtorThisVal then "this"
                        elif v.IsBaseVal then "base"
                        else v.LogicalName

                    tryTypeIdentity v.Type
                    |> Option.map (fun tyIdentity ->
                        { LogicalName = logicalName
                          Type = tyIdentity }))
            |> List.distinct
            |> List.sortBy (fun capture -> capture.LogicalName, capture.Type)

        captures

    // Gathers the consecutive curried lambda chain starting at a lambda node into one
    // occurrence: parameter groups plus the body/return type after the last group.
    let rec gatherCurriedGroups acc expr =
        match stripDebugPointsAndLinks expr with
        | Expr.Lambda (_, _, _, valParams, bodyExpr, _, bodyTy) ->
            gatherCurriedGroups ((valParams, bodyTy) :: acc) bodyExpr
        | _ -> List.rev acc, expr

    let rec walk (parentChain: int list) (expr: Expr) =
        match expr with
        | Expr.Lambda _ -> visitLambda parentChain expr
        | Expr.TyLambda _ ->
            // Local type functions lower through a different IlxGen path (erased or TyFunc
            // closures); keep members containing them on the legacy digest path for now.
            markUnsupported "a local type function"
        | Expr.Const _ -> ()
        | Expr.Val _ -> ()
        | Expr.App (funcExpr, _, _, args, _) ->
            walk parentChain funcExpr
            args |> List.iter (walk parentChain)
        | Expr.Sequential (expr1, expr2, _, _) ->
            walk parentChain expr1
            walk parentChain expr2
        | Expr.Let (TBind (_, bindingExpr, _), bodyExpr, _, _) ->
            walk parentChain bindingExpr
            walk parentChain bodyExpr
        | Expr.LetRec (bindings, bodyExpr, _, _) ->
            bindings |> List.iter (fun (TBind (_, bindingExpr, _)) -> walk parentChain bindingExpr)
            walk parentChain bodyExpr
        | Expr.Match (_, _, _, targets, _, _) ->
            // Mirrors the legacy digest traversal: classification is driven by the
            // expressions in the match targets.
            targets |> Array.iter (fun (TTarget (_, targetExpr, _)) -> walk parentChain targetExpr)
        | Expr.Op (op, _, args, _) ->
            match op with
            | TOp.While _
            | TOp.IntegerForLoop _
            | TOp.TryWith _
            | TOp.TryFinally _ ->
                // Loop/try operands are wrapped in compiler-generated delay lambdas
                // (mkDummyLambda) that IlxGen always eliminates; they never become
                // closures, so walk through them without creating occurrences.
                args |> List.iter (walkThroughOperandLambda parentChain)
            | _ -> args |> List.iter (walk parentChain)
        | Expr.Obj _ ->
            // Object expressions form closure classes through a separate IlxGen path.
            markUnsupported "an object expression"
        | Expr.Quote _ ->
            // Quotation-bearing members stay entirely on the query/legacy digest paths.
            markUnsupported "a quotation"
        | Expr.DebugPoint (_, body) -> walk parentChain body
        | Expr.Link eref -> walk parentChain eref.Value
        | Expr.TyChoose (_, bodyExpr, _) -> walk parentChain bodyExpr
        | Expr.WitnessArg _ -> ()
        | Expr.StaticOptimization (_, onExpr, elseExpr, _) ->
            walk parentChain onExpr
            walk parentChain elseExpr

    and walkThroughOperandLambda parentChain expr =
        match stripDebugPointsAndLinks expr with
        | Expr.Lambda (_, _, _, _, bodyExpr, _, _) -> walk parentChain bodyExpr
        | other -> walk parentChain other

    and visitLambda parentChain lambdaExpr =
        let ordinal = nextOrdinal
        nextOrdinal <- nextOrdinal + 1

        let groups, innerBody = gatherCurriedGroups [] lambdaExpr

        match groups with
        | [] -> walk parentChain innerBody
        | _ ->
            let parameterTypes =
                groups
                |> List.map (fun (valParams, _) ->
                    valParams |> List.choose (fun (v: Val) -> tryTypeIdentity v.Type))

            let returnTypeIdentity =
                let _, lastBodyTy = List.last groups
                tryTypeIdentity lastBodyTy

            if unsupported.IsNone then
                match returnTypeIdentity with
                | Some returnIdentity ->
                    occurrences.Add
                        { Id =
                            { MemberSymbol = memberSymbol
                              Ordinal = ordinal
                              ParentChain = parentChain }
                          CurriedArity = groups.Length
                          ParameterTypes = parameterTypes
                          Captures = captureIdentities lambdaExpr
                          ReturnTypeIdentity = returnIdentity
                          BodyHash = exprDigest denv lambdaExpr
                          Range = lambdaExpr.Range }
                | None -> ()

            walk (ordinal :: parentChain) innerBody

    walk [] (stripMemberTopLambdas valReprInfo expr)

    match unsupported with
    | Some reason -> LambdaOccurrenceExtraction.Unsupported reason
    | None -> LambdaOccurrenceExtraction.Extracted(List.ofSeq occurrences)

// ---------------------------------------------------------------------------
// Lambda occurrence alignment
// ---------------------------------------------------------------------------

/// Longest-common-subsequence pairing of two sequences under the given equality,
/// returning matched index pairs in order.
let private lcsMatchIndexes (isEqual: int -> int -> bool) (oldCount: int) (newCount: int) =
    let table = Array2D.zeroCreate (oldCount + 1) (newCount + 1)

    for i in oldCount - 1 .. -1 .. 0 do
        for j in newCount - 1 .. -1 .. 0 do
            table[i, j] <-
                if isEqual i j then
                    table[i + 1, j + 1] + 1
                else
                    max table[i + 1, j] table[i, j + 1]

    let pairs = ResizeArray()
    let mutable i = 0
    let mutable j = 0

    while i < oldCount && j < newCount do
        if isEqual i j then
            pairs.Add(i, j)
            i <- i + 1
            j <- j + 1
        elif table[i + 1, j] >= table[i, j + 1] then
            i <- i + 1
        else
            j <- j + 1

    List.ofSeq pairs

/// Full structural digest of an occurrence (sans body): position, shape, and captures.
/// Occurrences equal under this key are perfectly matched.
let private occurrenceStructuralKey (occ: LambdaOccurrence) =
    occ.Id.ParentChain, occ.CurriedArity, occ.ParameterTypes, occ.ReturnTypeIdentity, occ.Captures

/// Shape-only digest of an occurrence: the structural digest without the capture set.
/// Occurrences equal under this key but not under the structural key form a matched
/// pair whose capture sets are incompatible.
let private occurrenceShapeKey (occ: LambdaOccurrence) =
    occ.Id.ParentChain, occ.CurriedArity, occ.ParameterTypes, occ.ReturnTypeIdentity

/// Classifies the difference between the capture sets of a matched occurrence pair.
/// Pairing strategy: same-name/different-type pairs become TypeChanged, then
/// same-type/different-name pairs become Renamed, and the leftovers are reported as
/// plain additions/removals (the cross-occurrence scope post-pass may upgrade those).
let private diffCaptureSets (baseline: CaptureIdentity list) (updated: CaptureIdentity list) =
    let baselineSet = Set.ofList baseline
    let updatedSet = Set.ofList updated
    let removed = Set.difference baselineSet updatedSet |> Set.toList
    let added = Set.difference updatedSet baselineSet |> Set.toList

    if List.isEmpty removed && List.isEmpty added then
        []
    else
        let changes = ResizeArray()
        let mutable remainingAdded = added
        let mutable unpaired = []

        let takeAdded predicate =
            match remainingAdded |> List.tryFind predicate with
            | Some candidate ->
                remainingAdded <- remainingAdded |> List.filter (fun a -> a <> candidate)
                Some candidate
            | None -> None

        for removedCapture in removed do
            match takeAdded (fun a -> a.LogicalName = removedCapture.LogicalName) with
            | Some addedCapture ->
                changes.Add(
                    CaptureSetChange.TypeChanged(removedCapture.LogicalName, removedCapture.Type, addedCapture.Type)
                )
            | None -> unpaired <- removedCapture :: unpaired

        for removedCapture in List.rev unpaired do
            match takeAdded (fun a -> a.Type = removedCapture.Type) with
            | Some addedCapture ->
                changes.Add(
                    CaptureSetChange.Renamed(removedCapture.LogicalName, addedCapture.LogicalName, removedCapture.Type)
                )
            | None -> changes.Add(CaptureSetChange.CaptureRemoved removedCapture)

        for addedCapture in remainingAdded do
            changes.Add(CaptureSetChange.CaptureAdded addedCapture)

        List.ofSeq changes

/// Upgrades matching CaptureAdded/CaptureRemoved entries observed on DIFFERENT occurrence
/// pairs of the same member to ScopeChanged: the value did not stop being captured, it
/// moved between lambda scopes (C# parity: ChangingCapturedVariableScope).
let private classifyCaptureScopeMoves (edits: LambdaEdit list) =
    let capturesOf selector =
        edits
        |> List.collect (function
            | LambdaEdit.CaptureSetChanged (_, _, changes) -> changes |> List.choose selector
            | _ -> [])
        |> Set.ofList

    let addedIdentities =
        capturesOf (function
            | CaptureSetChange.CaptureAdded c -> Some c
            | _ -> None)

    let removedIdentities =
        capturesOf (function
            | CaptureSetChange.CaptureRemoved c -> Some c
            | _ -> None)

    let moved = Set.intersect addedIdentities removedIdentities

    if Set.isEmpty moved then
        edits
    else
        edits
        |> List.map (function
            | LambdaEdit.CaptureSetChanged (baseline, updated, changes) ->
                let upgraded =
                    changes
                    |> List.map (function
                        | CaptureSetChange.CaptureAdded c when Set.contains c moved ->
                            CaptureSetChange.ScopeChanged(c.LogicalName, c.Type)
                        | CaptureSetChange.CaptureRemoved c when Set.contains c moved ->
                            CaptureSetChange.ScopeChanged(c.LogicalName, c.Type)
                        | other -> other)

                LambdaEdit.CaptureSetChanged(baseline, updated, upgraded)
            | other -> other)

/// Aligns the baseline and updated occurrence sequences of a matched member.
///
/// Pass 1 runs an LCS over the full structural digests (position, shape, captures):
/// pairs matched here are compatible and become BodyEdited when their bodies differ.
/// Pass 2 re-aligns the leftovers on the shape-only digest: pairs matched here either
/// survived a reordering (capture sets still equal → BodyEdited) or have incompatible
/// capture sets (→ CaptureSetChanged). Anything still unmatched is Added/Removed.
///
/// Two identical occurrences that are reordered are indistinguishable by construction
/// and therefore align positionally — this is intentional (their closures are
/// interchangeable, so positional identity is correct).
let private alignLambdaOccurrences (baseline: LambdaOccurrence list) (updated: LambdaOccurrence list) =
    let olds = Array.ofList baseline
    let news = Array.ofList updated

    let pass1 =
        lcsMatchIndexes
            (fun i j -> occurrenceStructuralKey olds[i] = occurrenceStructuralKey news[j])
            olds.Length
            news.Length

    let matchedOld = HashSet(pass1 |> List.map fst)
    let matchedNew = HashSet(pass1 |> List.map snd)

    let leftoverOld =
        [| for i in 0 .. olds.Length - 1 do
               if not (matchedOld.Contains i) then yield i |]

    let leftoverNew =
        [| for j in 0 .. news.Length - 1 do
               if not (matchedNew.Contains j) then yield j |]

    let pass2 =
        lcsMatchIndexes
            (fun i j -> occurrenceShapeKey olds[leftoverOld[i]] = occurrenceShapeKey news[leftoverNew[j]])
            leftoverOld.Length
            leftoverNew.Length
        |> List.map (fun (i, j) -> leftoverOld[i], leftoverNew[j])

    let allPairs = pass1 @ pass2
    let pairedOld = HashSet(allPairs |> List.map fst)
    let pairedNew = HashSet(allPairs |> List.map snd)

    let pairEdits =
        allPairs
        |> List.choose (fun (i, j) ->
            let baselineOcc = olds[i]
            let updatedOcc = news[j]

            match diffCaptureSets baselineOcc.Captures updatedOcc.Captures with
            | [] when baselineOcc.BodyHash = updatedOcc.BodyHash -> None
            | [] -> Some(LambdaEdit.BodyEdited(baselineOcc, updatedOcc))
            | changes -> Some(LambdaEdit.CaptureSetChanged(baselineOcc, updatedOcc, changes)))

    let removedEdits =
        [ for i in 0 .. olds.Length - 1 do
              if not (pairedOld.Contains i) then
                  yield LambdaEdit.Removed olds[i] ]

    let addedEdits =
        [ for j in 0 .. news.Length - 1 do
              if not (pairedNew.Contains j) then
                  yield LambdaEdit.Added news[j] ]

    let ordinalOf =
        function
        | LambdaEdit.BodyEdited (_, updatedOcc)
        | LambdaEdit.CaptureSetChanged (_, updatedOcc, _)
        | LambdaEdit.Added updatedOcc -> updatedOcc.Id.Ordinal
        | LambdaEdit.Removed baselineOcc -> baselineOcc.Id.Ordinal

    pairEdits @ removedEdits @ addedEdits
    |> classifyCaptureScopeMoves
    |> List.sortBy ordinalOf

/// Properties needed to check if a method addition is allowed.
/// Following Roslyn patterns for Edit and Continue restrictions.
type private MethodAdditionInfo =
    { IsMethod: bool                    // True if this is a method (vs module value/field)
      IsDispatchSlot: bool              // Virtual or abstract
      IsOverrideOrExplicitImpl: bool    // Override or explicit interface impl
      IsExplicitInterfaceImplementation: bool // Explicit interface implementation
      IsConstructor: bool               // .ctor or .cctor
      IsOperator: bool                  // User-defined operator
      IsInInterface: bool               // Member of an interface type
      IsField: bool                     // Field (not a method)
      IsModuleBinding: bool }           // Bound at module scope (lowers to static members of the module type)

    static member Default =
        { IsMethod = false
          IsDispatchSlot = false
          IsOverrideOrExplicitImpl = false
          IsExplicitInterfaceImplementation = false
          IsConstructor = false
          IsOperator = false
          IsInInterface = false
          IsField = false
          IsModuleBinding = false }

type private BindingSnapshot =
    { Symbol: SymbolId
      InlineInfo: ValInline
      SignatureText: string
      ConstraintsText: string
      BodyHash: int
      LambdaShapeDigest: string
      StateMachineShapeDigest: string
      QueryShapeDigest: string
      // Structured lambda occurrence sequence (or the reason extraction was not possible,
      // in which case lambda classification falls back to LambdaShapeDigest).
      LambdaOccurrenceData: LambdaOccurrenceExtraction
      IsSynthesized: bool
      ContainingEntity: string option
      AdditionInfo: MethodAdditionInfo }

type private EntitySnapshot =
    { Symbol: SymbolId
      RepresentationHash: int
      RepresentationText: string
      IsSynthesized: bool }

let private hasLoweredShapeDigestSegmentValues (segmentName: string) (digest: string) =
    let marker = segmentName + "=["
    let startIndex = digest.IndexOf(marker, StringComparison.Ordinal)

    if startIndex < 0 then
        false
    else
        let valueStart = startIndex + marker.Length
        let valueEnd = digest.IndexOf("]", valueStart, StringComparison.Ordinal)

        if valueEnd < valueStart then
            false
        else
            valueEnd > valueStart

let private tryClassifySynthesizedLoweredShapeChurn (snapshot: BindingSnapshot) =
    if not snapshot.IsSynthesized then
        None
    else
        let logicalName = snapshot.Symbol.LogicalName

        let hasQueryStructuralEvidence =
            hasLoweredShapeDigestSegmentValues "struct" snapshot.QueryShapeDigest

        let hasQueryEvidence =
            hasQueryStructuralEvidence

        let hasStateMachineStructuralEvidence =
            hasLoweredShapeDigestSegmentValues "struct" snapshot.StateMachineShapeDigest

        let hasStateMachineEvidence =
            hasStateMachineStructuralEvidence
            || logicalName.Equals("MoveNext", StringComparison.Ordinal)

        let hasLambdaEvidence =
            not (String.IsNullOrEmpty snapshot.LambdaShapeDigest)

        if hasQueryEvidence then
            Some RudeEditKind.QueryExpressionShapeChange
        elif hasStateMachineEvidence then
            Some RudeEditKind.StateMachineShapeChange
        elif hasLambdaEvidence then
            Some RudeEditKind.LambdaShapeChange
        else
            // Fail closed when a synthesized declaration changes and we cannot confidently
            // classify it into a known lowered-shape bucket.
            Some RudeEditKind.SynthesizedDeclarationChange

let private symbolId
    path
    logicalName
    stamp
    kind
    memberKind
    isSynthesized
    compiledName
    totalArgCount
    genericArity
    parameterTypeIdentities
    returnTypeIdentity
    =
    { Path = path
      LogicalName = logicalName
      Stamp = stamp
      Kind = kind
      MemberKind = memberKind
      IsSynthesized = isSynthesized
      CompiledName = compiledName
      TotalArgCount = totalArgCount
      GenericArity = genericArity
      ParameterTypeIdentities = parameterTypeIdentities
      ReturnTypeIdentity = returnTypeIdentity }

let private bindingKey (snapshot: BindingSnapshot) =
    let entityKey = snapshot.ContainingEntity |> Option.defaultValue ""
    $"{snapshot.Symbol.QualifiedName}|{snapshot.SignatureText}|{entityKey}"

let private entityKey (snapshot: EntitySnapshot) = snapshot.Symbol.QualifiedName

let rec private snapshotModuleBinding g denv (path: string list) (map, entities) binding =
    match binding with
    | ModuleOrNamespaceBinding.Binding b ->
        let snapshot = snapshotBinding g denv path b
        (Map.add (bindingKey snapshot) snapshot map, entities)
    | ModuleOrNamespaceBinding.Module (moduleEntity, contents) ->
        snapshotModuleContents g denv (path @ [ moduleEntity.LogicalName ]) (map, entities) contents

and private snapshotModuleContents g denv path (map, entities) contents =
    match contents with
    | ModuleOrNamespaceContents.TMDefs defs ->
        ((map, entities), defs)
        ||> List.fold (snapshotModuleContents g denv path)
    | ModuleOrNamespaceContents.TMDefLet (binding, _) ->
        let snapshot = snapshotBinding g denv path binding
        (Map.add (bindingKey snapshot) snapshot map, entities)
    | ModuleOrNamespaceContents.TMDefRec (_, _, tycons, bindings, _) ->
        let entitiesWithTypes =
            (entities, tycons)
            ||> List.fold (fun acc tycon ->
                let snapshot = snapshotTycon denv path tycon
                Map.add (entityKey snapshot) snapshot acc)

        List.fold (snapshotModuleBinding g denv path) (map, entitiesWithTypes) bindings
    | ModuleOrNamespaceContents.TMDefDo _ -> (map, entities)
    | ModuleOrNamespaceContents.TMDefOpens _ -> (map, entities)

and private tryGetContainingEntityFullName (var: Val) =
    match var.MemberInfo with
    | Some memberInfo ->
        try
            let tyconRef = memberInfo.ApparentEnclosingEntity
            let ilTypeRef = tyconRef.CompiledRepresentationForNamedType
            Some(ilTypeRef.FullName)
        with _ -> None
    | None -> None

and private snapshotBinding g denv path (TBind (var, expr, _)) =
    let signature = tyToString denv var.Type
    let constraints = typarConstraintsDigest denv var.Typars
    let bodyHash = exprDigest denv expr
    let lambdaShapeDigest, stateMachineShapeDigest, queryShapeDigest = collectLoweredShapeInfo denv expr
    let containingEntity = tryGetContainingEntityFullName var
    let memberKind = memberKindOfVal var
    let vref = mkLocalValRef var
    let compiledName =
        try
            Some(vref.CompiledName None)
        with _ ->
            None
    let totalArgCount =
        var.ValReprInfo
        |> Option.map (fun info ->
            let isInstanceMember =
                match var.MemberInfo with
                | Some memberInfo -> memberInfo.MemberFlags.IsInstance
                | None -> false

            // ValReprInfo.TotalArgCount includes the implicit 'this' argument for instance members.
            // MethodDefinitionKey.ParameterTypes only includes emitted IL parameters, so subtract it.
            if isInstanceMember then
                max 0 (info.TotalArgCount - 1)
            else
                info.TotalArgCount)

    let methodTypeInfo = tryGetMethodTyparOrdinalsAndGenericArity g var
    let typarOrdinals = methodTypeInfo |> Option.map fst |> Option.defaultValue Map.empty
    let genericArity = methodTypeInfo |> Option.map snd
    let parameterTypeIdentities = tryGetParameterTypeIdentities g typarOrdinals var
    let returnTypeIdentity = tryGetReturnTypeIdentity g typarOrdinals var

    let symbol =
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
            parameterTypeIdentities
            returnTypeIdentity

    // Structured lambda occurrence sequence for the member body (Phase C1). Members with
    // constructs the model does not cover stay on the legacy whole-body digest path.
    let lambdaOccurrenceData =
        extractLambdaOccurrences g denv typarOrdinals symbol var.ValReprInfo expr

    // Determine addition info for hot reload restrictions
    let additionInfo =
        let isMethod = memberKind.IsSome
        let isDispatchSlot =
            match var.MemberInfo with
            | Some memberInfo -> memberInfo.MemberFlags.IsDispatchSlot
            | None -> false
        let isOverrideOrExplicitImpl =
            match var.MemberInfo with
            | Some memberInfo -> memberInfo.MemberFlags.IsOverrideOrExplicitImpl
            | None -> false
        let isExplicitInterfaceImplementation =
            try
                ValRefIsExplicitImpl g vref
            with _ ->
                false
        let isConstructor = var.IsConstructor || var.IsClassConstructor
        // Operators have logical names starting with "op_"
        let isOperator = var.LogicalName.StartsWith("op_", StringComparison.Ordinal)
        let isInInterface =
            match var.MemberInfo with
            | Some memberInfo ->
                try memberInfo.ApparentEnclosingEntity.IsFSharpInterfaceTycon
                with _ -> false
            | None -> false
        // A field is a module-level mutable value or a non-method member
        let isField = not isMethod && var.IsMutable
        { IsMethod = isMethod
          IsDispatchSlot = isDispatchSlot
          IsOverrideOrExplicitImpl = isOverrideOrExplicitImpl
          IsExplicitInterfaceImplementation = isExplicitInterfaceImplementation
          IsConstructor = isConstructor
          IsOperator = isOperator
          IsInInterface = isInInterface
          IsField = isField
          IsModuleBinding = var.IsModuleBinding }

    { Symbol = symbol
      InlineInfo = var.InlineInfo
      SignatureText = signature
      ConstraintsText = constraints
      BodyHash = bodyHash
      LambdaShapeDigest = lambdaShapeDigest
      StateMachineShapeDigest = stateMachineShapeDigest
      QueryShapeDigest = queryShapeDigest
      LambdaOccurrenceData = lambdaOccurrenceData
      IsSynthesized = var.IsCompilerGenerated
      ContainingEntity = containingEntity
      AdditionInfo = additionInfo }: BindingSnapshot

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
                    sb.Append("|case:") |> ignore
                    sb.Append(case.LogicalName) |> ignore
                    case.FieldTable.FieldsByIndex
                    |> Array.iter (fun field ->
                        sb.Append(":") |> ignore
                        sb.Append(field.LogicalName) |> ignore
                        sb.Append("=") |> ignore
                        sb.Append(tyToString denv field.FormalType) |> ignore))
            | FSharpTyconKind.TFSharpRecord
            | FSharpTyconKind.TFSharpStruct
            | FSharpTyconKind.TFSharpClass
            | FSharpTyconKind.TFSharpInterface
            | FSharpTyconKind.TFSharpEnum ->
                data.fsobjmodel_rfields.FieldsByIndex
                |> Array.iter (fun field ->
                    sb.Append("|field:") |> ignore
                    sb.Append(field.LogicalName) |> ignore
                    if field.IsMutable then sb.Append("[mutable]") |> ignore
                    sb.Append("=") |> ignore
                    sb.Append(tyToString denv field.FormalType) |> ignore)
            | FSharpTyconKind.TFSharpDelegate slotSig ->
                sb.Append("|delegate:") |> ignore
                sb.Append(slotSig.Name) |> ignore
        | TILObjectRepr (TILObjectReprData(_, _, definition)) ->
            sb.Append("|til:") |> ignore
            sb.Append(definition.Name) |> ignore
        | TAsmRepr ilTy ->
            sb.Append("|asm:") |> ignore
            sb.Append(ilTy.ToString()) |> ignore
        | TMeasureableRepr ty ->
            sb.Append("|measure:") |> ignore
            sb.Append(tyToString denv ty) |> ignore
#if !NO_TYPEPROVIDERS
        | TProvidedTypeRepr info ->
            sb.Append("|provided:") |> ignore
            sb.Append(string info.IsErased) |> ignore
        | TProvidedNamespaceRepr _ ->
            sb.Append("|provided-namespace") |> ignore
#endif
        | TNoRepr ->
            sb.Append("|norepr") |> ignore

        sb.ToString()

    { Symbol = symbolId path tycon.LogicalName tycon.Stamp SymbolKind.Entity None false None None None None None
      RepresentationHash = stableHash reprText
      RepresentationText = reprText
      IsSynthesized = false }: EntitySnapshot

let private collectSnapshots g denv (CheckedImplFile (qualifiedNameOfFile = qual; contents = contents)) =
    let initialPath = [ qual.Text ]
    let initialBindings: Map<string, BindingSnapshot> = Map.empty
    let initialEntities: Map<string, EntitySnapshot> = Map.empty
    snapshotModuleContents g denv initialPath (initialBindings, initialEntities) contents

/// Formats a structured message for a lambda-set change, naming the counts and ordinals
/// of the added/removed occurrences (e.g. "1 lambda added at ordinal 2").
let private formatLambdaSetChangeMessage
    (symbol: SymbolId)
    (added: LambdaOccurrence list)
    (removed: LambdaOccurrence list)
    =
    let describe verb (occs: LambdaOccurrence list) =
        let ordinals = occs |> List.map (fun occ -> string occ.Id.Ordinal) |> String.concat ", "

        if occs.Length = 1 then
            $"1 lambda {verb} at ordinal {ordinals}"
        else
            $"{occs.Length} lambdas {verb} at ordinals {ordinals}"

    let parts =
        [ if not removed.IsEmpty then describe "removed" removed
          if not added.IsEmpty then describe "added" added ]

    $"""Lambda set changed for '{symbol.QualifiedName}': {String.concat "; " parts}."""

/// Formats a structured message for capture-set incompatibilities of matched lambda
/// occurrence pairs, using the C#-parity rude edit kind names.
let private formatCaptureSetChangeMessage (symbol: SymbolId) (captureChanged: LambdaEdit list) =
    let describeChange ordinal change =
        match change with
        | CaptureSetChange.Renamed (oldName, newName, _) ->
            $"RenamingCapturedVariable: captured value '{oldName}' renamed to '{newName}' (lambda ordinal {ordinal})"
        | CaptureSetChange.TypeChanged (name, _, _) ->
            $"ChangingCapturedVariableType: captured value '{name}' changed type (lambda ordinal {ordinal})"
        | CaptureSetChange.ScopeChanged (name, _) ->
            $"ChangingCapturedVariableScope: captured value '{name}' moved to a different lambda scope (lambda ordinal {ordinal})"
        | CaptureSetChange.CaptureAdded capture ->
            $"lambda at ordinal {ordinal} captures additional value '{capture.LogicalName}' (may become a supported edit via AddInstanceFieldToExistingType)"
        | CaptureSetChange.CaptureRemoved capture ->
            $"lambda at ordinal {ordinal} no longer captures value '{capture.LogicalName}'"

    let parts =
        captureChanged
        |> List.collect (function
            | LambdaEdit.CaptureSetChanged (_, updatedOcc, changes) ->
                changes |> List.map (describeChange updatedOcc.Id.Ordinal)
            | _ -> [])

    $"""Lambda capture set changed for '{symbol.QualifiedName}': {String.concat "; " parts}."""

let private compareBindings
    (capabilities: EditAndContinueCapabilities)
    (baseline: Map<string, BindingSnapshot>)
    (updated: Map<string, BindingSnapshot>)
    =
    let edits = ResizeArray()
    let rude = ResizeArray()
    let memberLambdaEdits = ResizeArray()
    let matchedUpdatedKeys = HashSet()

    let handleEdit (snapshot: BindingSnapshot) kind baselineHash updatedHash =
        let symbol = snapshot.Symbol
        edits.Add(
            { Symbol = symbol
              Kind = kind
              BaselineHash = baselineHash
              UpdatedHash = updatedHash
              IsSynthesized = snapshot.IsSynthesized
              ContainingEntity = snapshot.ContainingEntity }
        )

    let compareMatchedBindings (baselineBinding: BindingSnapshot) (updatedBinding: BindingSnapshot) =
        let runtimeSignatureIdentityKnown =
            baselineBinding.Symbol.CompiledName.IsSome
            && updatedBinding.Symbol.CompiledName.IsSome
            && baselineBinding.Symbol.TotalArgCount.IsSome
            && updatedBinding.Symbol.TotalArgCount.IsSome
            && baselineBinding.Symbol.GenericArity.IsSome
            && updatedBinding.Symbol.GenericArity.IsSome
            && baselineBinding.Symbol.ParameterTypeIdentities.IsSome
            && updatedBinding.Symbol.ParameterTypeIdentities.IsSome
            && baselineBinding.Symbol.ReturnTypeIdentity.IsSome
            && updatedBinding.Symbol.ReturnTypeIdentity.IsSome

        let hasEquivalentRuntimeSignature =
            runtimeSignatureIdentityKnown
            && baselineBinding.Symbol.CompiledName = updatedBinding.Symbol.CompiledName
            && baselineBinding.Symbol.TotalArgCount = updatedBinding.Symbol.TotalArgCount
            && baselineBinding.Symbol.GenericArity = updatedBinding.Symbol.GenericArity
            && baselineBinding.Symbol.ParameterTypeIdentities = updatedBinding.Symbol.ParameterTypeIdentities
            && baselineBinding.Symbol.ReturnTypeIdentity = updatedBinding.Symbol.ReturnTypeIdentity

        if baselineBinding.SignatureText <> updatedBinding.SignatureText && not hasEquivalentRuntimeSignature then
            rude.Add(
                { Symbol = Some baselineBinding.Symbol
                  Kind = RudeEditKind.SignatureChange
                  Message =
                    $"Signature changed from '{baselineBinding.SignatureText}' to '{updatedBinding.SignatureText}'." }
            )
        elif baselineBinding.ConstraintsText <> updatedBinding.ConstraintsText then
            rude.Add(
                { Symbol = Some baselineBinding.Symbol
                  Kind = RudeEditKind.SignatureChange
                  Message =
                    $"Type parameter constraints changed from '{baselineBinding.ConstraintsText}' to '{updatedBinding.ConstraintsText}'." }
            )
        elif baselineBinding.InlineInfo <> updatedBinding.InlineInfo then
            rude.Add(
                { Symbol = Some baselineBinding.Symbol
                  Kind = RudeEditKind.InlineChange
                  Message = "Inline annotation changed." }
            )
        elif baselineBinding.QueryShapeDigest <> updatedBinding.QueryShapeDigest then
            rude.Add(
                { Symbol = Some baselineBinding.Symbol
                  Kind = RudeEditKind.QueryExpressionShapeChange
                  Message =
                    $"Query-expression lowering shape changed from '{baselineBinding.QueryShapeDigest}' to '{updatedBinding.QueryShapeDigest}'." }
            )
        elif baselineBinding.StateMachineShapeDigest <> updatedBinding.StateMachineShapeDigest then
            rude.Add(
                { Symbol = Some baselineBinding.Symbol
                  Kind = RudeEditKind.StateMachineShapeChange
                  Message =
                    $"State-machine lowering shape changed from '{baselineBinding.StateMachineShapeDigest}' to '{updatedBinding.StateMachineShapeDigest}'." }
            )
        else
            // Lambda classification (Phase C1): when occurrence extraction succeeded on both
            // sides, align the occurrence sequences and classify per occurrence; otherwise
            // fall back to the legacy whole-body lambda digest comparison.
            let lambdaRudeEdit =
                match baselineBinding.LambdaOccurrenceData, updatedBinding.LambdaOccurrenceData with
                | LambdaOccurrenceExtraction.Extracted baselineOccs, LambdaOccurrenceExtraction.Extracted updatedOccs ->
                    let lambdaEditList = alignLambdaOccurrences baselineOccs updatedOccs

                    if not lambdaEditList.IsEmpty then
                        memberLambdaEdits.Add
                            { MemberSymbol = baselineBinding.Symbol
                              Edits = lambdaEditList }

                    let added =
                        lambdaEditList
                        |> List.choose (function
                            | LambdaEdit.Added occ -> Some occ
                            | _ -> None)

                    let removed =
                        lambdaEditList
                        |> List.choose (function
                            | LambdaEdit.Removed occ -> Some occ
                            | _ -> None)

                    let captureChanged =
                        lambdaEditList
                        |> List.filter (function
                            | LambdaEdit.CaptureSetChanged _ -> true
                            | _ -> false)

                    if not added.IsEmpty || not removed.IsEmpty then
                        Some(formatLambdaSetChangeMessage baselineBinding.Symbol added removed)
                    elif not captureChanged.IsEmpty then
                        Some(formatCaptureSetChangeMessage baselineBinding.Symbol captureChanged)
                    else
                        // Only BodyEdited occurrences (or no lambda changes at all): the
                        // member body update covers the edit; fall through to body hashing.
                        None
                | _ ->
                    // Legacy digest path: extraction was unsupported on at least one side.
                    if baselineBinding.LambdaShapeDigest <> updatedBinding.LambdaShapeDigest then
                        Some
                            $"Lambda lowering shape changed from '{baselineBinding.LambdaShapeDigest}' to '{updatedBinding.LambdaShapeDigest}'."
                    else
                        None

            match lambdaRudeEdit with
            | Some message ->
                rude.Add(
                    { Symbol = Some baselineBinding.Symbol
                      Kind = RudeEditKind.LambdaShapeChange
                      Message = message }
                )
            | None ->
                if baselineBinding.BodyHash <> updatedBinding.BodyHash then
                    if traceHotReloadMethodDiff then
                        printfn
                            "[fsharp-hotreload][typed-diff] body change symbol=%s synthesized=%b baselineHash=%d updatedHash=%d"
                            baselineBinding.Symbol.LogicalName
                            baselineBinding.IsSynthesized
                            baselineBinding.BodyHash
                            updatedBinding.BodyHash

                    handleEdit
                        baselineBinding
                        SemanticEditKind.MethodBody
                        (Some baselineBinding.BodyHash)
                        (Some updatedBinding.BodyHash)

    let addRemovedDeclarationRudeEdit (baselineBinding: BindingSnapshot) =
        match tryClassifySynthesizedLoweredShapeChurn baselineBinding with
        | Some loweredKind ->
            let message =
                if loweredKind = RudeEditKind.SynthesizedDeclarationChange then
                    $"Synthesized declaration removed for '{baselineBinding.Symbol.QualifiedName}', but no known lowered-shape classifier matched."
                else
                    $"Synthesized declaration removed while lowered shape changed for '{baselineBinding.Symbol.QualifiedName}'."
            rude.Add(
                { Symbol = Some baselineBinding.Symbol
                  Kind = loweredKind
                  Message = message }
            )
        | None ->
            rude.Add(
                { Symbol = Some baselineBinding.Symbol
                  Kind = RudeEditKind.DeclarationRemoved
                  Message = "Declaration removed." }
            )

    let addAddedDeclarationOrInsertEdit (updatedBinding: BindingSnapshot) =
        match tryClassifySynthesizedLoweredShapeChurn updatedBinding with
        | Some loweredKind ->
            let message =
                if loweredKind = RudeEditKind.SynthesizedDeclarationChange then
                    $"Synthesized declaration added for '{updatedBinding.Symbol.QualifiedName}', but no known lowered-shape classifier matched."
                else
                    $"Synthesized declaration added while lowered shape changed for '{updatedBinding.Symbol.QualifiedName}'."
            rude.Add(
                { Symbol = Some updatedBinding.Symbol
                  Kind = loweredKind
                  Message = message }
            )
        | None ->
            let info = updatedBinding.AdditionInfo

            // Module-level values lower to a static field on the module type plus accessor
            // methods (get_/set_), with initialization appended to the startup-code class
            // constructor. Applying one therefore needs BOTH the static-field and the method
            // runtime capabilities (Roslyn parity: a C# static field with initializer needs
            // AddStaticFieldToExistingType, and our accessors additionally need
            // AddMethodToExistingType).
            let addModuleValueInsertOrRude () =
                let requiredCapabilities =
                    [ capabilityForAddition AdditionKind.StaticField
                      capabilityForAddition AdditionKind.Method ]

                match requiredCapabilities |> List.tryFind (capabilities.Supports >> not) with
                | None -> handleEdit updatedBinding SemanticEditKind.Insert None (Some updatedBinding.BodyHash)
                | Some missing ->
                    rude.Add(
                        { Symbol = Some updatedBinding.Symbol
                          Kind = RudeEditKind.NotSupportedByRuntime
                          Message =
                            FSComp.SR.hotReloadAdditionNotSupportedByRuntime (
                                updatedBinding.Symbol.QualifiedName,
                                missing.Name
                            ) }
                    )

            // Check restrictions following Roslyn patterns
            if info.IsField then
                if info.IsModuleBinding then
                    // Mutable module-level value: static field + get_/set_ accessors.
                    addModuleValueInsertOrRude ()
                else
                    // Instance fields (class let-bounds / val mutable) are Phase B2: emission is
                    // not implemented yet, so they stay rude regardless of runtime capabilities.
                    rude.Add(
                        { Symbol = Some updatedBinding.Symbol
                          Kind = RudeEditKind.FieldAdded
                          Message = "Adding fields is not supported. Fields change type layout." }
                    )
            elif info.IsExplicitInterfaceImplementation then
                rude.Add(
                    { Symbol = Some updatedBinding.Symbol
                      Kind = RudeEditKind.InsertExplicitInterface
                      Message = "Adding explicit interface implementations is not supported." }
                )
            elif info.IsDispatchSlot || info.IsOverrideOrExplicitImpl then
                // Virtual, abstract, or override methods cannot be added
                rude.Add(
                    { Symbol = Some updatedBinding.Symbol
                      Kind = RudeEditKind.InsertVirtual
                      Message = "Adding virtual, abstract, or override methods is not supported." }
                )
            elif info.IsConstructor then
                // Constructors cannot be added to existing types
                rude.Add(
                    { Symbol = Some updatedBinding.Symbol
                      Kind = RudeEditKind.InsertConstructor
                      Message = "Adding constructors is not supported." }
                )
            elif info.IsOperator then
                // User-defined operators cannot be added
                rude.Add(
                    { Symbol = Some updatedBinding.Symbol
                      Kind = RudeEditKind.InsertOperator
                      Message = "Adding user-defined operators is not supported." }
                )
            elif info.IsInInterface then
                // Members cannot be added to interfaces
                rude.Add(
                    { Symbol = Some updatedBinding.Symbol
                      Kind = RudeEditKind.InsertIntoInterface
                      Message = "Adding members to interfaces is not supported." }
                )
            elif info.IsMethod then
                // The edit kind itself is supported; whether it can be applied depends on the
                // capabilities the runtime negotiated at session start (Roslyn parity:
                // AbstractEditAndContinueAnalyzer reports RudeEditKind.NotSupportedByRuntime
                // when an otherwise-valid edit exceeds the runtime's capabilities).
                let requiredCapability = capabilityForAddition AdditionKind.Method

                if capabilities.Supports requiredCapability then
                    // Method can be added - emit as Insert edit
                    handleEdit updatedBinding SemanticEditKind.Insert None (Some updatedBinding.BodyHash)
                else
                    rude.Add(
                        { Symbol = Some updatedBinding.Symbol
                          Kind = RudeEditKind.NotSupportedByRuntime
                          Message =
                            FSComp.SR.hotReloadAdditionNotSupportedByRuntime (
                                updatedBinding.Symbol.QualifiedName,
                                requiredCapability.Name
                            ) }
                    )
            elif info.IsModuleBinding then
                match updatedBinding.Symbol.TotalArgCount with
                | Some argCount when argCount > 0 ->
                    // Module-level functions lower to plain static methods on the module type —
                    // the same edit shape as adding a type member, gated the same way.
                    let requiredCapability = capabilityForAddition AdditionKind.Method

                    if capabilities.Supports requiredCapability then
                        handleEdit updatedBinding SemanticEditKind.Insert None (Some updatedBinding.BodyHash)
                    else
                        rude.Add(
                            { Symbol = Some updatedBinding.Symbol
                              Kind = RudeEditKind.NotSupportedByRuntime
                              Message =
                                FSComp.SR.hotReloadAdditionNotSupportedByRuntime (
                                    updatedBinding.Symbol.QualifiedName,
                                    requiredCapability.Name
                                ) }
                        )
                | _ ->
                    // Immutable module-level value: static field (+ getter) initialized from the
                    // startup-code class constructor — same capability pair as the mutable case.
                    addModuleValueInsertOrRude ()
            else
                // Additions that are neither members, fields, nor module bindings (e.g. local
                // bindings surfaced at this level) remain unsupported.
                rude.Add(
                    { Symbol = Some updatedBinding.Symbol
                      Kind = RudeEditKind.DeclarationAdded
                      Message = "Adding this kind of declaration is not supported." }
                )

    let hasSameBindingIdentity (baselineBinding: BindingSnapshot) (updatedBinding: BindingSnapshot) =
        baselineBinding.Symbol.QualifiedName = updatedBinding.Symbol.QualifiedName
        && baselineBinding.ContainingEntity = updatedBinding.ContainingEntity
        && baselineBinding.Symbol.MemberKind = updatedBinding.Symbol.MemberKind
        && baselineBinding.Symbol.CompiledName = updatedBinding.Symbol.CompiledName
        && baselineBinding.Symbol.TotalArgCount = updatedBinding.Symbol.TotalArgCount
        && baselineBinding.Symbol.GenericArity = updatedBinding.Symbol.GenericArity
        && baselineBinding.Symbol.ParameterTypeIdentities = updatedBinding.Symbol.ParameterTypeIdentities
        && baselineBinding.Symbol.ReturnTypeIdentity = updatedBinding.Symbol.ReturnTypeIdentity

    let tryFindFallbackUpdatedBinding (baselineBinding: BindingSnapshot) =
        updated
        |> Seq.tryPick (fun (KeyValue(updatedKey, updatedBinding)) ->
            if matchedUpdatedKeys.Contains updatedKey then
                None
            elif hasSameBindingIdentity baselineBinding updatedBinding then
                Some(updatedKey, updatedBinding)
            else
                None)

    for KeyValue(key, baselineBinding) in baseline do
        match Map.tryFind key updated with
        | Some updatedBinding ->
            matchedUpdatedKeys.Add key |> ignore
            compareMatchedBindings baselineBinding updatedBinding
        | None ->
            match tryFindFallbackUpdatedBinding baselineBinding with
            | Some(updatedKey, updatedBinding) ->
                matchedUpdatedKeys.Add updatedKey |> ignore
                compareMatchedBindings baselineBinding updatedBinding
            | None ->
                addRemovedDeclarationRudeEdit baselineBinding

    for KeyValue(key, updatedBinding) in updated do
        if not (matchedUpdatedKeys.Contains key) && not (Map.containsKey key baseline) then
            addAddedDeclarationOrInsertEdit updatedBinding

    edits |> Seq.toList, rude |> Seq.toList, memberLambdaEdits |> Seq.toList

let private compareEntities (baseline: Map<string, EntitySnapshot>) (updated: Map<string, EntitySnapshot>) =
    let rude = ResizeArray()

    for KeyValue(key, baselineEntity) in baseline do
        match Map.tryFind key updated with
        | Some updatedEntity ->
            if baselineEntity.RepresentationHash <> updatedEntity.RepresentationHash then
                rude.Add(
                    { Symbol = Some baselineEntity.Symbol
                      Kind = RudeEditKind.TypeLayoutChange
                      Message =
                        $"Type representation changed from '{baselineEntity.RepresentationText}' to '{updatedEntity.RepresentationText}'." }
                )
        | None ->
            rude.Add(
                { Symbol = Some baselineEntity.Symbol
                  Kind = RudeEditKind.DeclarationRemoved
                  Message = "Type declaration removed." }
            )

    for KeyValue(key, updatedEntity) in updated do
        if not (Map.containsKey key baseline) then
            rude.Add(
                { Symbol = Some updatedEntity.Symbol
                  Kind = RudeEditKind.DeclarationAdded
                  Message = "Type declaration added." }
            )

    rude |> Seq.toList

/// Computes semantic edits between two checked implementation files, classifying additions
/// against the runtime capabilities negotiated for the active hot reload session.
let diffImplementationFile (g: TcGlobals) (capabilities: EditAndContinueCapabilities) baseline updated =
    let denv = DisplayEnv.Empty g
    let baselineBindings, baselineEntities = collectSnapshots g denv baseline
    let updatedBindings, updatedEntities = collectSnapshots g denv updated

    let semanticEdits, bindingRudeEdits, lambdaEdits =
        compareBindings capabilities baselineBindings updatedBindings

    let entityRudeEdits = compareEntities baselineEntities updatedEntities

    { SemanticEdits = semanticEdits
      RudeEdits = bindingRudeEdits @ entityRudeEdits
      LambdaEdits = lambdaEdits }
