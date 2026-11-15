// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.SignatureFingerprint

open System
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.CheckDeclarations
open FSharp.Compiler.Syntax
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps

exception private NotDescribable

/// What sprintf "%A" gave for each key, kept: the few flag and kind values a signature has are formatted over and
/// over, and formatting them was most of the cost of describing it. Only types with few values are kept, so the
/// tables stay small for the life of the process. A struct tuple holds the key, as some values, such as a union
/// case represented by null, are null.
type private Shown<'K when 'K: equality>() =
    static member val Texts = Collections.Concurrent.ConcurrentDictionary<struct ('K * unit), string>(HashIdentity.Structural)

/// sprintf "%A" of the value, kept by a key that is equal only for values that print the same: the value itself
/// where its type's equality is structural, or else every part of it that printing shows (ModuleOrNamespaceKind
/// and SynMemberFlags have custom equality that ignores some of them).
let private shownBy (key: 'K) (value: 'T) =
    Shown<'K>.Texts.GetOrAdd(struct (key, ()), fun _ -> sprintf "%A" value)

let private shown (value: 'T) = shownBy value value

/// sprintf "%A" of a module or namespace kind.
let private kindText (kind: ModuleOrNamespaceKind) =
    match kind with
    | ModuleOrNamespaceKind.FSharpModuleWithSuffix -> "FSharpModuleWithSuffix"
    | ModuleOrNamespaceKind.ModuleOrType -> "ModuleOrType"
    | ModuleOrNamespaceKind.Namespace isExplicit -> if isExplicit then "Namespace true" else "Namespace false"

let private memberFlagsKey (flags: SynMemberFlags) =
    flags.IsInstance, flags.IsDispatchSlot, flags.IsOverrideOrExplicitImpl, flags.IsFinal,
    flags.GetterOrSetterIsCompilerGenerated, flags.MemberKind

/// sprintf "%b" of the value.
let private flag (value: bool) = if value then "true" else "false"

let inline private singleToInt32Bits (f: float32) : int32 =
#if NETSTANDARD2_0
    BitConverter.ToInt32(BitConverter.GetBytes(f), 0)
#else
    BitConverter.SingleToInt32Bits f
#endif

/// sprintf "%A" of the long access paths, kept by the CompilationPath, so only as long as the typed tree holding it
let private longAccessPaths = System.Runtime.CompilerServices.ConditionalWeakTable<CompilationPath, string>()

/// sprintf "%A" of an access path, written out rather than kept by value: a project has a path for every module,
/// and a table of them would grow with every name ever typed. A path %A could break over lines, or whose names
/// it could escape, goes to sprintf, kept for the path object.
let accessPathText (compPath: CompilationPath) =
    let path = compPath.AccessPath

    let plain (name: string) =
        name |> Seq.forall (fun c -> Char.IsLetterOrDigit c || c = '_' || c = '.' || c = '\'' || c = '`')

    if path |> List.forall (fst >> plain) then
        let text =
            "["
            + (path |> List.map (fun (name, kind) -> "(\"" + name + "\", " + kindText kind + ")") |> String.concat "; ")
            + "]"

        // Well inside the width at which %A starts breaking a list
        if text.Length <= 60 then text else longAccessPaths.GetValue(compPath, fun compPath -> sprintf "%A" compPath.AccessPath)
    else
        longAccessPaths.GetValue(compPath, fun compPath -> sprintf "%A" compPath.AccessPath)

/// A compiler-generated name (basic@line, then -n for the n-th earlier name of that basic name) without its count:
/// a check numbers the names over what it checks, so a file checked whole and the same file checked one
/// declaration at a time number them differently. Nothing outside the file can name such a value or field.
let private generatedName (name: string) =
    let at = name.LastIndexOf '@'
    let dash = if at < 0 then -1 else name.IndexOf('-', at)

    let digits (first: int) (last: int) =
        first <= last && Seq.forall Char.IsDigit (name.Substring(first, last - first + 1))

    if dash > at && digits (at + 1) (dash - 1) && digits (dash + 1) (name.Length - 1) then
        name.Substring(0, dash)
    else
        name

/// Describes a module type, then the given inline values' bodies, with type parameters and local values numbered
/// from 0 in the order they first appear. A member's implemented slots and whether it is implemented come from
/// `slotsOf`; None makes the module type not describable.
let private describe
    (mty: ModuleOrNamespaceType)
    (topAttrs: TopAttribs)
    (inlineBodies: (Val * Expr) list)
    (slotsOf: Val -> ValMemberInfo -> (SlotSig list * bool) option)
    : string option =
    let sb = Text.StringBuilder()
    let add (s: string) = sb.Append(s).Append('|') |> ignore
    let typarNumbers = Collections.Generic.Dictionary<Typar, int>(HashIdentity.Reference)
    let localNumbers = Collections.Generic.Dictionary<Val, int>(HashIdentity.Reference)

    let rec ty (t: TType) =
        match stripTyparEqns t with
        | TType_app (tcref, tinst, nullness) ->
            add "app"
            add (fullDisplayTextOfTyconRef tcref)
            nullnessOf nullness
            tys tinst
        | TType_fun (domainTy, rangeTy, nullness) ->
            add "fun"
            ty domainTy
            ty rangeTy
            nullnessOf nullness
        | TType_tuple (tupInfo, elementTys) ->
            add (if evalTupInfoIsStruct tupInfo then "struct tuple" else "tuple")
            tys elementTys
        | TType_anon (info, fieldTys) ->
            add "anon"
            anonInfo info
            tys fieldTys
        | TType_var (tp, nullness) ->
            typar tp
            nullnessOf nullness
        | TType_forall (tps, bodyTy) ->
            add "forall"
            typars tps
            ty bodyTy
        | TType_measure m ->
            add "measure"
            measure m
        | TType_ucase (ucref, tinst) ->
            add "ucase"
            add (fullDisplayTextOfTyconRef ucref.TyconRef)
            add ucref.CaseName
            tys tinst

    and tys (l: TType list) =
        add (string l.Length)
        List.iter ty l

    and anonInfo (info: AnonRecdTypeInfo) =
        add info.Assembly.AssemblyName
        add (string (evalAnonInfoIsStruct info))
        for name in info.SortedNames do add name

    and nullnessOf (nullness: Nullness) = add (shown (nullness.Evaluate()))

    and measure (m: Measure) =
        match stripUnitEqns m with
        | Measure.Var tp -> typar tp
        | Measure.Const (tcref, _) -> add (fullDisplayTextOfTyconRef tcref)
        | Measure.Prod (m1, m2, _) ->
            add "*"
            measure m1
            measure m2
        | Measure.Inv m1 ->
            add "/"
            measure m1
        | Measure.One _ -> add "1"
        | Measure.RationalPower (m1, power) ->
            add "^"
            measure m1
            add (sprintf "%A" power)

    // A type parameter is described once, where it first appears, and by its number after that.
    and typar (tp: Typar) =
        match typarNumbers.TryGetValue tp with
        | true, n -> add ("'" + string n)
        | _ ->
            let n = typarNumbers.Count
            typarNumbers[tp] <- n
            add ("'" + string n + " new")
            // A name inference gave is not the user's, and CheckFile and FinishFile need not agree on it
            add (if tp.IsCompilerGenerated then "-" else tp.Name)
            add (shown tp.StaticReq + " " + shown tp.Kind + " " + shown tp.Rigidity + " " + shown tp.DynamicReq)

            add (
                flag tp.EqualityConditionalOn
                + " "
                + flag tp.ComparisonConditionalOn
                + " "
                + flag tp.IsCompatFlex
                + " "
                + flag tp.typar_flags.IsSupportsNullFlex
                + " "
                + flag tp.IsCompilerGenerated
                + " "
                + flag tp.IsFromError
            )

            attribs tp.Attribs
            add (string tp.Constraints.Length)
            List.iter constr tp.Constraints

    and typars (tps: Typars) =
        add (string tps.Length)
        List.iter typar tps

    and constr (c: TyparConstraint) =
        match c with
        | TyparConstraint.CoercesTo (t, _) ->
            add ":>"
            ty t
        | TyparConstraint.DefaultsTo (priority, t, _) ->
            add "default"
            add (string priority)
            ty t
        | TyparConstraint.MayResolveMember (TTrait (supportTys, memberName, memberFlags, argTys, returnTy, _, _, _), _) ->
            add "member"
            tys supportTys
            add memberName
            add (sprintf "%A" memberFlags)
            tys argTys
            optTy returnTy
        | TyparConstraint.SimpleChoice (choiceTys, _) ->
            add "choice"
            tys choiceTys
        | TyparConstraint.IsEnum (t, _) ->
            add "enum"
            ty t
        | TyparConstraint.IsDelegate (argsTy, retTy, _) ->
            add "delegate"
            ty argsTy
            ty retTy
        | TyparConstraint.SupportsNull _ -> add "null"
        | TyparConstraint.NotSupportsNull _ -> add "not null"
        | TyparConstraint.IsNonNullableStruct _ -> add "struct"
        | TyparConstraint.IsReferenceType _ -> add "not struct"
        | TyparConstraint.RequiresDefaultConstructor _ -> add "new"
        | TyparConstraint.SupportsComparison _ -> add "comparison"
        | TyparConstraint.SupportsEquality _ -> add "equality"
        | TyparConstraint.IsUnmanaged _ -> add "unmanaged"
        | TyparConstraint.AllowsRefStruct _ -> add "allows ref struct"

    and optTy (t: TType option) =
        match t with
        | Some t -> ty t
        | None -> add "-"

    and access (TAccess paths) =
        add (string paths.Length)
        for path in paths do add (accessPathText path)

    and attribs (l: Attribs) =
        add (string l.Length)
        for Attrib (tcref, _, args, namedArgs, appliedToAccessor, targets, _) in l do
            add (fullDisplayTextOfTyconRef tcref)
            add (flag appliedToAccessor + " " + shown targets)
            add (string args.Length)
            for AttribExpr (_, e) in args do attribExpr e
            add (string namedArgs.Length)
            for AttribNamedArg (name, t, isField, AttribExpr (_, e)) in namedArgs do
                add name
                ty t
                add (string isField)
                attribExpr e

    and attribExpr (e: Expr) =
        match e with
        | Expr.Const (c, _, t) ->
            constant c
            ty t
        | Expr.Op (TOp.Coerce, tinst, [ e ], _) ->
            add "coerce"
            tys tinst
            attribExpr e
        | Expr.Op (TOp.Array, tinst, elements, _) ->
            add "array"
            tys tinst
            add (string elements.Length)
            List.iter attribExpr elements
        | Expr.App (Expr.Val (vref, _, _), _, tinst, args, _) ->
            add (vref.LogicalName)
            tys tinst
            add (string args.Length)
            List.iter attribExpr args
        | _ -> raise NotDescribable

    // Exact: %A rounds floating-point values, so two different constants could print the same.
    and constant (c: Const) =
        match c with
        | Const.Bool b -> add ("bool " + string b)
        | Const.SByte n -> add ("sbyte " + string n)
        | Const.Byte n -> add ("byte " + string n)
        | Const.Int16 n -> add ("int16 " + string n)
        | Const.UInt16 n -> add ("uint16 " + string n)
        | Const.Int32 n -> add ("int32 " + string n)
        | Const.UInt32 n -> add ("uint32 " + string n)
        | Const.Int64 n -> add ("int64 " + string n)
        | Const.UInt64 n -> add ("uint64 " + string n)
        | Const.IntPtr n -> add ("nativeint " + string n)
        | Const.UIntPtr n -> add ("unativeint " + string n)
        | Const.Single f -> add ("single " + string (singleToInt32Bits f))
        | Const.Double d -> add ("double " + string (BitConverter.DoubleToInt64Bits d))
        | Const.Char ch -> add ("char " + string (int ch))
        // The length keeps a separator inside the string from reading as the end of it
        | Const.String s -> add ("string " + string s.Length + " " + s)
        | Const.Decimal d -> add ("decimal " + String.Join(",", Decimal.GetBits d))
        | Const.Unit -> add "unit"
        | Const.Zero -> add "zero"

    and optConstant (c: Const option) =
        match c with
        | Some c -> constant c
        | None -> add "-"

    and argInfo (info: ArgReprInfo) =
        add (match info.Name with Some id -> id.idText | None -> "-")
        attribs (info.Attribs.AsList())

    and slotSig (TSlotSig (name, declaringTy, classTps, methodTps, paramGroups, returnTy)) =
        add name
        ty declaringTy
        typars classTps
        typars methodTps
        add (string paramGroups.Length)
        for group in paramGroups do
            add (string group.Length)
            for TSlotParam (paramName, paramTy, isIn, isOut, isOptional, paramAttribs) in group do
                add (defaultArg paramName "-")
                ty paramTy
                add (sprintf "%b %b %b" isIn isOut isOptional)
                attribs paramAttribs
        optTy returnTy

    and value (v: Val) =
        add "val"
        add (if v.IsCompilerGenerated then generatedName v.LogicalName else v.LogicalName)
        add (defaultArg v.ValCompiledName "-")
        access v.Accessibility
        ty v.Type
        attribs v.Attribs
        optConstant v.LiteralValue
        add (shown v.InlineInfo + " " + flag v.IsMutable + " " + flag v.IsCompilerGenerated + " " + flag v.IsExtensionMember)
        add (shown v.val_flags.BaseOrThisInfo + " " + flag v.val_flags.IsMemberOrModuleBinding + " " + flag v.val_flags.IsIncrClassSpecialMember)
        add (flag v.val_flags.IsTypeFunction + " " + flag v.val_flags.PermitsExplicitTypeInstantiation + " " + flag v.val_flags.IsGeneratedEventVal + " " + flag v.val_flags.IsFixed)

        match v.ValReprInfo with
        | Some (ValReprInfo (typarInfos, argGroups, result)) ->
            add (string typarInfos.Length)
            for TyparReprInfo (_, kind) in typarInfos do add (shown kind)
            add (string argGroups.Length)
            for group in argGroups do
                add (string group.Length)
                List.iter argInfo group
            argInfo result
        | None -> add "-"

        match v.MemberInfo with
        | Some memberInfo ->
            let slotSigs, isImplemented =
                match slotsOf v memberInfo with
                | Some slots -> slots
                | None -> raise NotDescribable
            add (fullDisplayTextOfTyconRef memberInfo.ApparentEnclosingEntity)
            add (shownBy (memberFlagsKey memberInfo.MemberFlags) memberInfo.MemberFlags + " " + flag isImplemented)
            add (string slotSigs.Length)
            List.iter slotSig slotSigs
        | None -> add "-"

    and field (f: RecdField) =
        add (if f.IsCompilerGenerated then generatedName f.LogicalName else f.LogicalName)
        ty f.FormalType
        add (flag f.IsMutable + " " + flag f.IsStatic + " " + flag f.IsVolatile + " " + flag f.IsCompilerGenerated)
        optConstant f.LiteralValue
        access f.Accessibility
        attribs f.PropertyAttribs
        attribs f.FieldAttribs

    and entity (e: Entity) =
        add "entity"
        add e.LogicalName
        add e.CompiledName
        access e.Accessibility
        access e.TypeReprAccessibility
        attribs e.Attribs
        add (flag e.IsPrefixDisplay + " " + flag e.entity_flags.HasSelfReferentialConstructor + " " + flag e.entity_flags.IsStructRecordOrUnionType + " " + shown e.TypeOrMeasureKind)
        typars e.Typars

        if e.IsModuleOrNamespace then
            add (kindText e.ModuleOrNamespaceType.ModuleOrNamespaceKind)
            moduleOrNamespace e.ModuleOrNamespaceType
        else
            optTy e.TypeAbbrev

            match e.TypeReprInfo with
            | TFSharpTyconRepr data ->
                match data.fsobjmodel_kind with
                | TFSharpRecord -> add "record"
                | TFSharpUnion -> add "union"
                | TFSharpClass -> add "class"
                | TFSharpInterface -> add "interface"
                | TFSharpStruct -> add "struct"
                | TFSharpEnum -> add "enum"
                | TFSharpDelegate ss ->
                    add "delegate"
                    slotSig ss
            | TMeasureableRepr t ->
                add "measureable"
                ty t
            | TNoRepr -> add "no repr"
            | TILObjectRepr _
            | TAsmRepr _
            | TProvidedTypeRepr _
            | TProvidedNamespaceRepr _ -> raise NotDescribable

            let contents = e.TypeContents
            optTy contents.tcaug_super
            add (string contents.tcaug_interfaces.Length)
            for interfaceTy, compilerGenerated, _ in contents.tcaug_interfaces do
                ty interfaceTy
                add (string compilerGenerated)
            add (sprintf "%b %b %b %b %b" contents.tcaug_compare.IsSome contents.tcaug_compare_withc.IsSome
                    contents.tcaug_equals.IsSome contents.tcaug_hash_and_equals_withc.IsSome contents.tcaug_abstract)

            add (string e.AllFieldsArray.Length)
            Array.iter field e.AllFieldsArray
            add (string e.UnionCasesArray.Length)
            for uc in e.UnionCasesArray do
                add uc.LogicalName
                add uc.CompiledName
                access uc.Accessibility
                attribs uc.Attribs
                ty uc.ReturnType
                add (string uc.RecdFieldsArray.Length)
                Array.iter field uc.RecdFieldsArray

            match e.ExceptionInfo with
            | TExnAbbrevRepr tcref -> add (fullDisplayTextOfTyconRef tcref)
            | TExnFresh fields ->
                add "exception"
                add (string fields.FieldsByIndex.Length)
                Array.iter field fields.FieldsByIndex
            | TExnNone -> add "-"
            | TExnAsmRepr _ -> raise NotDescribable

    and moduleOrNamespace (m: ModuleOrNamespaceType) =
        let entities = m.AllEntities |> Seq.toList
        add (string entities.Length)
        List.iter entity entities
        let vals = m.AllValsAndMembers |> Seq.toList
        add (string vals.Length)
        List.iter value vals

    // Inline bodies, whose code the users of a value get a copy of. Values bound inside the body are numbered, and
    // others named, so neither stamps nor ranges show.
    and bindVal (v: Val) =
        let n = localNumbers.Count
        localNumbers[v] <- n
        add ("local " + string n)
        ty v.Type

    and bindVals (vs: Val list) =
        add (string vs.Length)
        List.iter bindVal vs

    and optBindVal (v: Val option) =
        match v with
        | Some v -> bindVal v
        | None -> add "-"

    and valRef (vref: ValRef) =
        let local = if vref.IsLocalRef then localNumbers.TryGetValue vref.Deref else (false, 0)

        match local with
        | true, n -> add ("local " + string n)
        | _ ->
            add (fullDisplayTextOfValRef vref)
            ty vref.Type

    and useFlag (flag: ValUseFlag) =
        match flag with
        | PossibleConstrainedCall t ->
            add "constrained"
            ty t
        | NormalValUse -> add "normal"
        | CtorValUsedAsSuperInit -> add "super init"
        | CtorValUsedAsSelfInit -> add "self init"
        | VSlotDirectCall -> add "slot direct"

    and tyconRef (tcref: TyconRef) = add (fullDisplayTextOfTyconRef tcref)

    and unionCaseRef (ucref: UnionCaseRef) =
        tyconRef ucref.TyconRef
        add ucref.CaseName

    and recdFieldRef (rfref: RecdFieldRef) =
        tyconRef rfref.TyconRef
        add rfref.FieldName

    and ilMethodRef (mref: ILMethodRef) =
        add mref.DeclaringTypeRef.QualifiedName
        add mref.Name
        add (string mref.GenericArity)
        ilCallingConv mref.CallingConv
        add (string mref.ArgTypes.Length)
        List.iter ilType mref.ArgTypes
        ilType mref.ReturnType

    and ilCallingConv (cc: ILCallingConv) = add (sprintf "%A %A" cc.ThisConv cc.BasicConv)

    // Structural: ILType.QualifiedName fails on void, byref, pointer and function pointer types.
    and ilType (t: ILType) =
        match t with
        | ILType.Void -> add "void"
        | ILType.Array (shape, elementTy) ->
            add (sprintf "array %A" shape)
            ilType elementTy
        | ILType.Value spec ->
            add "value"
            ilTypeSpec spec
        | ILType.Boxed spec ->
            add "boxed"
            ilTypeSpec spec
        | ILType.Ptr t ->
            add "ptr"
            ilType t
        | ILType.Byref t ->
            add "byref"
            ilType t
        | ILType.FunctionPointer signature ->
            add "function pointer"
            ilCallingConv signature.CallingConv
            add (string signature.ArgTypes.Length)
            List.iter ilType signature.ArgTypes
            ilType signature.ReturnType
        | ILType.TypeVar n -> add ("!" + string n)
        | ILType.Modified (required, modifier, t) ->
            add (sprintf "modified %b" required)
            add modifier.QualifiedName
            ilType t

    and ilTypeSpec (spec: ILTypeSpec) =
        add spec.TypeRef.QualifiedName
        add (string spec.GenericArgs.Length)
        List.iter ilType spec.GenericArgs

    // Instructions carrying a type, method, field, token or label give NotDescribable rather than a lossy text.
    and ilInstr (instr: ILInstr) =
        match instr with
        | AI_ldc (basicTy, c) ->
            add (sprintf "ldc %A" basicTy)
            match c with
            | ILConst.I4 n -> add ("i4 " + string n)
            | ILConst.I8 n -> add ("i8 " + string n)
            | ILConst.R4 f -> add ("r4 " + string (singleToInt32Bits f))
            | ILConst.R8 d -> add ("r8 " + string (BitConverter.DoubleToInt64Bits d))
        | I_ldstr s -> add ("ldstr " + string s.Length + " " + s)
        | AI_conv _ | AI_conv_ovf _ | AI_conv_ovf_un _
        | I_ldarg _ | I_ldarga _ | I_ldind _ | I_ldloc _ | I_ldloca _ | I_starg _ | I_stind _ | I_stloc _
        | I_ldelem _ | I_stelem _ -> add (sprintf "%A" instr)
        | _ ->
            let text = sprintf "%A" instr
            // A case without a payload prints as its bare name
            if text |> Seq.exists Char.IsWhiteSpace then raise NotDescribable
            add text

    and traitInfo (TTrait (supportTys, memberName, memberFlags, argTys, returnTy, _, solution, _)) =
        tys supportTys
        add memberName
        add (sprintf "%A" memberFlags)
        tys argTys
        optTy returnTy

        match solution.Value with
        | None -> add "-"
        | Some (FSMethSln (t, vref, minst, staticTyOpt)) ->
            add "fs method"
            ty t
            valRef vref
            tys minst
            optTy staticTyOpt
        | Some (FSRecdFieldSln (tinst, rfref, isSetProp)) ->
            add "field"
            tys tinst
            recdFieldRef rfref
            add (string isSetProp)
        | Some (FSAnonRecdFieldSln (info, tinst, index)) ->
            add "anon field"
            anonInfo info
            tys tinst
            add (string index)
        | Some (ILMethSln (t, extOpt, mref, minst, staticTyOpt)) ->
            add "il method"
            ty t
            add (match extOpt with Some tref -> tref.QualifiedName | None -> "-")
            ilMethodRef mref
            tys minst
            optTy staticTyOpt
        | Some (ClosedExprSln e) ->
            add "closed"
            expr e
        | Some BuiltInSln -> add "built in"

    and exprs (es: Expr list) =
        add (string es.Length)
        List.iter expr es

    and expr (e: Expr) =
        match e with
        | Expr.Const (c, _, t) ->
            add "const"
            constant c
            ty t
        | Expr.Val (vref, flag, _) ->
            add "val"
            valRef vref
            useFlag flag
        | Expr.Sequential (e1, e2, kind, _) ->
            add (sprintf "seq %A" kind)
            expr e1
            expr e2
        | Expr.Lambda (_, ctorThisValOpt, baseValOpt, valParams, body, _, overallTy) ->
            add "lambda"
            optBindVal ctorThisValOpt
            optBindVal baseValOpt
            bindVals valParams
            expr body
            ty overallTy
        | Expr.TyLambda (_, tps, body, _, overallTy) ->
            add "type lambda"
            typars tps
            expr body
            ty overallTy
        | Expr.App (f, fTy, tinst, args, _) ->
            add "app"
            expr f
            ty fTy
            tys tinst
            exprs args
        | Expr.LetRec (bindings, body, _, _) ->
            add "let rec"
            bindVals [ for TBind (v, _, _) in bindings -> v ]
            for TBind (_, rhs, _) in bindings do expr rhs
            expr body
        | Expr.Let (TBind (v, rhs, _), body, _, _) ->
            add "let"
            expr rhs
            bindVal v
            expr body
        | Expr.Obj (_, objTy, baseVal, ctorCall, overrides, interfaceImpls, _) ->
            add "object"
            ty objTy
            optBindVal baseVal
            expr ctorCall
            objMethods overrides
            add (string interfaceImpls.Length)
            for interfaceTy, methods in interfaceImpls do
                ty interfaceTy
                objMethods methods
        | Expr.Match (_, _, tree, targets, _, t) ->
            add "match"
            decisionTree tree
            add (string targets.Length)
            for TTarget (vs, target, flags) in targets do
                bindVals vs
                expr target
                add (sprintf "%A" flags)
            ty t
        | Expr.StaticOptimization (conditions, e1, e2, _) ->
            add "static optimization"
            add (string conditions.Length)
            for condition in conditions do
                match condition with
                | TTyconEqualsTycon (t1, t2) ->
                    add "equals"
                    ty t1
                    ty t2
                | TTyconIsStruct t ->
                    add "struct"
                    ty t
            expr e1
            expr e2
        | Expr.Op (op, tinst, args, _) ->
            add "op"
            operation op
            tys tinst
            exprs args
        | Expr.Quote (quoted, _, isFromQueryExpression, _, t) ->
            add (sprintf "quote %b" isFromQueryExpression)
            expr quoted
            ty t
        | Expr.WitnessArg (info, _) ->
            add "witness"
            traitInfo info
        | Expr.TyChoose (tps, body, _) ->
            add "type choose"
            typars tps
            expr body
        | Expr.Link link -> expr link.Value
        | Expr.DebugPoint (_, e) -> expr e

    and objMethods (methods: ObjExprMethod list) =
        add (string methods.Length)
        for TObjExprMethod (ss, methodAttribs, tps, paramGroups, body, _) in methods do
            slotSig ss
            attribs methodAttribs
            typars tps
            add (string paramGroups.Length)
            List.iter bindVals paramGroups
            expr body

    and decisionTree (tree: DecisionTree) =
        match tree with
        | TDSwitch (input, cases, defaultOpt, _) ->
            add "switch"
            expr input
            add (string cases.Length)
            for TCase (test, caseTree) in cases do
                decisionTest test
                decisionTree caseTree
            match defaultOpt with
            | Some d ->
                add "default"
                decisionTree d
            | None -> add "-"
        | TDSuccess (results, targetNum) ->
            add ("success " + string targetNum)
            exprs results
        | TDBind (TBind (v, rhs, _), body) ->
            add "bind"
            expr rhs
            bindVal v
            decisionTree body

    and decisionTest (test: DecisionTreeTest) =
        match test with
        | DecisionTreeTest.UnionCase (ucref, tinst) ->
            add "case"
            unionCaseRef ucref
            tys tinst
        | DecisionTreeTest.ArrayLength (length, t) ->
            add ("array length " + string length)
            ty t
        | DecisionTreeTest.Const c ->
            add "const"
            constant c
        | DecisionTreeTest.IsNull -> add "null"
        | DecisionTreeTest.IsInst (source, target) ->
            add "is"
            ty source
            ty target
        | DecisionTreeTest.ActivePatternCase (activePatExpr, resTys, retKind, identity, idx, _) ->
            add (sprintf "active pattern %A %d" retKind idx)
            expr activePatExpr
            tys resTys
            match identity with
            | Some (vref, tinst) ->
                valRef vref
                tys tinst
            | None -> add "-"
        | DecisionTreeTest.Error _ -> add "error"

    and operation (op: TOp) =
        match op with
        | TOp.UnionCase ucref ->
            add "union case"
            unionCaseRef ucref
        | TOp.ExnConstr tcref ->
            add "exception"
            tyconRef tcref
        | TOp.Tuple info -> add (if evalTupInfoIsStruct info then "struct tuple" else "tuple")
        | TOp.AnonRecd info ->
            add "anon"
            anonInfo info
        | TOp.AnonRecdGet (info, i) ->
            add ("anon get " + string i)
            anonInfo info
        | TOp.Array -> add "array"
        | TOp.Bytes bytes -> add ("bytes " + Convert.ToBase64String bytes)
        | TOp.UInt16s values -> add ("uint16s " + String.Join(",", values))
        | TOp.While (_, marker) -> add (sprintf "while %A" marker)
        | TOp.IntegerForLoop (_, _, style) -> add (sprintf "for %A" style)
        | TOp.TryWith _ -> add "try with"
        | TOp.TryFinally _ -> add "try finally"
        | TOp.Recd (info, tcref) ->
            add (sprintf "record %A" info)
            tyconRef tcref
        | TOp.ValFieldSet rfref ->
            add "field set"
            recdFieldRef rfref
        | TOp.ValFieldGet rfref ->
            add "field get"
            recdFieldRef rfref
        | TOp.ValFieldGetAddr (rfref, readonly) ->
            add (sprintf "field address %b" readonly)
            recdFieldRef rfref
        | TOp.UnionCaseTagGet tcref ->
            add "tag"
            tyconRef tcref
        | TOp.UnionCaseProof ucref ->
            add "proof"
            unionCaseRef ucref
        | TOp.UnionCaseFieldGet (ucref, i) ->
            add (sprintf "case field get %d" i)
            unionCaseRef ucref
        | TOp.UnionCaseFieldGetAddr (ucref, i, readonly) ->
            add (sprintf "case field address %d %b" i readonly)
            unionCaseRef ucref
        | TOp.UnionCaseFieldSet (ucref, i) ->
            add (sprintf "case field set %d" i)
            unionCaseRef ucref
        | TOp.ExnFieldGet (tcref, i) ->
            add (sprintf "exception field get %d" i)
            tyconRef tcref
        | TOp.ExnFieldSet (tcref, i) ->
            add (sprintf "exception field set %d" i)
            tyconRef tcref
        | TOp.TupleFieldGet (info, i) -> add (sprintf "tuple get %b %d" (evalTupInfoIsStruct info) i)
        | TOp.ILAsm (instrs, retTys) ->
            add ("il " + string instrs.Length)
            List.iter ilInstr instrs
            tys retTys
        | TOp.RefAddrGet readonly -> add (sprintf "ref address %b" readonly)
        | TOp.Coerce -> add "coerce"
        | TOp.Reraise -> add "reraise"
        | TOp.Return -> add "return"
        // Labels come from a generator, so their numbers are not positional
        | TOp.Goto _
        | TOp.Label _ -> raise NotDescribable
        | TOp.TraitCall info ->
            add "trait call"
            traitInfo info
        | TOp.LValueOp (lop, vref) ->
            add (sprintf "lvalue %A" lop)
            valRef vref
        | TOp.DebugLocalScope (vref, _) ->
            add "scope"
            valRef vref
        | TOp.ILCall (isVirtual, isProtected, isStruct, isCtor, flag, isProperty, noTailCall, mref, enclTypeInst, methInst, retTys) ->
            add (sprintf "il call %b %b %b %b %b %b" isVirtual isProtected isStruct isCtor isProperty noTailCall)
            useFlag flag
            ilMethodRef mref
            tys enclTypeInst
            tys methInst
            tys retTys

    try
        moduleOrNamespace mty
        attribs topAttrs.mainMethodAttrs
        attribs topAttrs.netModuleAttrs
        attribs topAttrs.assemblyAttrs

        for v, body in inlineBodies do
            add "inline"
            add v.LogicalName
            ty v.Type
            expr body

        Some (sb.ToString())
    with NotDescribable ->
        None

let private currentSlots (_: Val) (memberInfo: ValMemberInfo) = Some(memberInfo.ImplementedSlotSigs, memberInfo.IsImplemented)

let compute (mty: ModuleOrNamespaceType) (topAttrs: TopAttribs) : string option = describe mty topAttrs [] currentSlots

/// What the code after a declaration sees of it: its contribution to the module or namespace it is in, and the
/// bodies of its inline values. Numbering starts again for each declaration, so a fingerprint depends neither on
/// other declarations nor on the order they were checked in. A member's implemented slots come from `slotsOf`:
/// closing the inference scope completes them (see DeclarationCheck.slotsAtCheck).
let computeDeclaration
    (contribution: ModuleOrNamespaceType)
    (inlineBodies: (Val * Expr) list)
    (slotsOf: Val -> ValMemberInfo -> (SlotSig list * bool) option)
    : string option =
    describe contribution EmptyTopAttrs inlineBodies slotsOf
