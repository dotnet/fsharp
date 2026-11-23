module internal Internal.Utilities.TypeHashing

open Internal.Utilities.Rational
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.Syntax
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps

type ObserverVisibility =
    | PublicOnly
    | PublicAndInternal

[<AutoOpen>]
module internal HashingPrimitives =

    type Hash = int64

    /// FNV-1a 64-bit over UTF-16 code units – deterministic across processes
    /// (unlike String.GetHashCode which is randomized in .NET 6+).
    let hashStableString (s: string) : Hash =
        let mutable h = 0xCBF29CE484222325UL

        for c in s do
            h <- (h ^^^ uint64 c) * 0x100000001B3UL

        int64 h

    let hashText (s: string) : Hash = hashStableString s

    let inline combineHash (acc: Hash) (y: Hash) : Hash = (acc ^^^ y) * 1099511628211L
    let inline pipeToHash (value: Hash) (acc: Hash) = combineHash acc value
    let inline addFullStructuralHash value (acc: Hash) = combineHash acc (int64 (hash value))

    let inline hashListOrderMatters ([<InlineIfLambda>] func) (items: #seq<'T>) : Hash =
        let mutable acc = 0L

        for i in items do
            let valHash = func i
            // We are calling hashListOrderMatters for things like list of types, list of properties, list of fields etc. The ones which are visibility-hidden will return 0, and are omitted.
            if valHash <> 0L then
                acc <- combineHash acc valHash

        acc

    let inline hashListOrderIndependent ([<InlineIfLambda>] func) (items: #seq<'T>) : Hash =
        let mutable acc = 0L

        for i in items do
            let valHash = func i
            acc <- acc ^^^ valHash

        acc

    let (@@) (h1: Hash) (h2: Hash) = combineHash h1 h2

    /// Maximum number of tokens emitted when generating type structure fingerprints.
    /// Limits memory usage and prevents infinite type loops.
    [<Literal>]
    let MaxTokenCount = 256

    [<Literal>]
    let MaxExactStructureTokenCount = 1024

[<AutoOpen>]
module internal HashUtilities =

    let private hashEntityRefName (xref: EntityRef) name =
        let tag =
            if xref.IsNamespace then
                TextTag.Namespace
            elif xref.IsModule then
                TextTag.Module
            elif xref.IsTypeAbbrev then
                TextTag.Alias
            elif xref.IsFSharpDelegateTycon then
                TextTag.Delegate
            elif xref.IsILEnumTycon || xref.IsFSharpEnumTycon then
                TextTag.Enum
            elif xref.IsStructOrEnumTycon then
                TextTag.Struct
            elif isInterfaceTyconRef xref then
                TextTag.Interface
            elif xref.IsUnionTycon then
                TextTag.Union
            elif xref.IsRecordTycon then
                TextTag.Record
            else
                TextTag.Class

        (int64 (hash tag)) @@ (hashText name)

    let hashTyconRefImpl (tcref: TyconRef) =
        let demangled = tcref.DisplayNameWithStaticParameters
        let tyconHash = hashEntityRefName tcref demangled

        tcref.CompilationPath.AccessPath
        |> hashListOrderMatters (fst >> hashText)
        |> pipeToHash tyconHash

module HashIL =

    let hashILTypeRef (tref: ILTypeRef) =
        tref.Enclosing
        |> hashListOrderMatters hashText
        |> pipeToHash (hashText tref.Name)

    let private hashILArrayShape (sh: ILArrayShape) : Hash = int64 sh.Rank

    let rec hashILType (ty: ILType) : Hash =
        match ty with
        | ILType.Void -> int64 (hash ILType.Void)
        | ILType.Array(sh, t) -> hashILType t @@ hashILArrayShape sh
        | ILType.Value t
        | ILType.Boxed t -> hashILTypeRef t.TypeRef @@ (t.GenericArgs |> hashListOrderMatters hashILType)
        | ILType.Ptr t
        | ILType.Byref t -> hashILType t
        | ILType.FunctionPointer t -> hashILCallingSignature t
        | ILType.TypeVar n -> int64 (hash n)
        | ILType.Modified(_, _, t) -> hashILType t

    and hashILCallingSignature (signature: ILCallingSignature) =
        let res = signature.ReturnType |> hashILType
        signature.ArgTypes |> hashListOrderMatters hashILType |> pipeToHash res

module HashAccessibility =

    let isHiddenToObserver (TAccess access) (observer: ObserverVisibility) =
        let isInternalCompPath x =
            match x with
            | CompPath(ILScopeRef.Local, _, []) -> true
            | _ -> false

        match access with
        | [] -> false
        | _ when List.forall isInternalCompPath access ->
            match observer with
            // The 'access' means internal, but our observer can see it (e.g. because of IVT attribute)
            | PublicAndInternal -> false
            | PublicOnly -> true
        | _ -> true

module rec HashTypes =

    /// Hash a reference to a type
    let hashTyconRef tcref = hashTyconRefImpl tcref

    /// Hash the flags of a member
    let hashMemberFlags (memFlags: SynMemberFlags) = int64 (hash memFlags)

    /// Hash an attribute 'Type(arg1, ..., argN)'
    let private hashAttrib (Attrib(tyconRef = tcref)) = hashTyconRefImpl tcref

    let hashAttributeList attrs =
        attrs |> hashListOrderIndependent hashAttrib

    let private hashTyparRef (typar: Typar) =
        hashText typar.DisplayName
        |> addFullStructuralHash typar.Rigidity
        |> addFullStructuralHash typar.StaticReq

    let private hashTyparRefWithInfo (typar: Typar) =
        hashTyparRef typar @@ hashAttributeList typar.Attribs

    let private hashConstraint (g: TcGlobals) struct (tp, tpc) =
        let tpHash = hashTyparRefWithInfo tp

        match tpc with
        | TyparConstraint.CoercesTo(tgtTy, _) -> tpHash @@ 1 @@ hashTType g tgtTy
        | TyparConstraint.MayResolveMember(traitInfo, _) -> tpHash @@ 2 @@ hashTraitWithInfo (* denv *) g traitInfo
        | TyparConstraint.DefaultsTo(_, ty, _) -> tpHash @@ 3 @@ hashTType g ty
        | TyparConstraint.IsEnum(ty, _) -> tpHash @@ 4 @@ hashTType g ty
        | TyparConstraint.SupportsComparison _ -> tpHash @@ 5
        | TyparConstraint.SupportsEquality _ -> tpHash @@ 6
        | TyparConstraint.IsDelegate(aty, bty, _) -> tpHash @@ 7 @@ hashTType g aty @@ hashTType g bty
        | TyparConstraint.SupportsNull _ -> tpHash @@ 8
        | TyparConstraint.IsNonNullableStruct _ -> tpHash @@ 9
        | TyparConstraint.IsUnmanaged _ -> tpHash @@ 10
        | TyparConstraint.IsReferenceType _ -> tpHash @@ 11
        | TyparConstraint.SimpleChoice(tys, _) -> tpHash @@ 12 @@ (tys |> hashListOrderIndependent (hashTType g))
        | TyparConstraint.RequiresDefaultConstructor _ -> tpHash @@ 13
        | TyparConstraint.NotSupportsNull _ -> tpHash @@ 14
        | TyparConstraint.AllowsRefStruct _ -> tpHash @@ 15

    /// Hash type parameter constraints
    let private hashConstraints (g: TcGlobals) cxs =
        cxs |> hashListOrderIndependent (hashConstraint g)

    let private hashTraitWithInfo (g: TcGlobals) traitInfo =
        let nameHash = hashText traitInfo.MemberLogicalName
        let memberHash = hashMemberFlags traitInfo.MemberFlags

        let returnTypeHash =
            match traitInfo.CompiledReturnType with
            | Some t -> hashTType g t
            | _ -> -1

        traitInfo.CompiledObjectAndArgumentTypes
        |> hashListOrderIndependent (hashTType g)
        |> pipeToHash nameHash
        |> pipeToHash returnTypeHash
        |> pipeToHash memberHash

    /// Hash a unit of measure expression
    let private hashMeasure g unt =
        let measureVarsWithExponents =
            ListMeasureVarOccsWithNonZeroExponents unt
            |> List.sortBy (fun (tp: Typar, _) -> tp.DisplayName)

        let measureConsWithExponents = ListMeasureConOccsWithNonZeroExponents g false unt

        let varHash =
            measureVarsWithExponents
            |> hashListOrderIndependent (fun (typar, exp: Rational) -> hashTyparRef typar @@ int64 (hash exp))

        let conHash =
            measureConsWithExponents
            |> hashListOrderIndependent (fun (tcref, exp: Rational) -> hashTyconRef tcref @@ int64 (hash exp))

        varHash @@ conHash

    /// Hash a type, taking precedence into account to insert brackets where needed
    let hashTType (g: TcGlobals) ty =

        match stripTyparEqns ty |> (stripTyEqns g) with
        | TType_ucase(UnionCaseRef(tc, _), args)
        | TType_app(tc, args, _) -> args |> hashListOrderMatters (hashTType g) |> pipeToHash (hashTyconRef tc)
        | TType_anon(anonInfo, tys) ->
            tys
            |> hashListOrderMatters (hashTType g)
            |> pipeToHash (anonInfo.SortedNames |> hashListOrderMatters hashText)
            |> addFullStructuralHash (evalAnonInfoIsStruct anonInfo)
        | TType_tuple(tupInfo, t) ->
            t
            |> hashListOrderMatters (hashTType g)
            |> addFullStructuralHash (evalTupInfoIsStruct tupInfo)
        // Hash a first-class generic type.
        | TType_forall(tps, tau) -> tps |> hashListOrderMatters hashTyparRef |> pipeToHash (hashTType g tau)
        | TType_fun _ ->
            let argTys, retTy = stripFunTy g ty
            argTys |> hashListOrderMatters (hashTType g) |> pipeToHash (hashTType g retTy)
        | TType_var(r, _) -> hashTyparRefWithInfo r
        | TType_measure unt -> hashMeasure g unt

    // Hash a single argument, including its name and type
    let private hashArgInfo (g: TcGlobals) (ty, argInfo: ArgReprInfo) =

        let attributesHash = hashAttributeList (argInfo.Attribs.AsList())

        let nameHash =
            match argInfo.Name with
            | Some i -> hashText i.idText
            | _ -> -1L

        let typeHash = hashTType g ty

        typeHash @@ nameHash @@ attributesHash

    let private hashCurriedArgInfos (g: TcGlobals) argInfos =
        argInfos
        |> hashListOrderMatters (fun l -> l |> hashListOrderMatters (hashArgInfo g))

    /// Hash a single type used as the type of a member or value
    let hashTopType (g: TcGlobals) argInfos retTy cxs =
        let retTypeHash = hashTType g retTy
        let cxsHash = hashConstraints g cxs
        let argHash = hashCurriedArgInfos g argInfos

        retTypeHash @@ cxsHash @@ argHash

    let private hashTyparInclConstraints (g: TcGlobals) (typar: Typar) =
        typar.Constraints
        |> hashListOrderIndependent (fun tpc -> hashConstraint g (typar, tpc))
        |> pipeToHash (hashTyparRef typar)

    /// Hash type parameters
    let hashTyparDecls (g: TcGlobals) (typars: Typars) =
        typars |> hashListOrderMatters (hashTyparInclConstraints g)

    let private hashUncurriedSig (g: TcGlobals) typarInst argInfos retTy =
        typarInst
        |> hashListOrderMatters (fun (typar, ttype) -> hashTyparInclConstraints g typar @@ hashTType g ttype)
        |> pipeToHash (hashTopType g argInfos retTy [])

    let private hashMemberSigCore (g: TcGlobals) memberToParentInst (typarInst, methTypars: Typars, argInfos, retTy) =
        typarInst
        |> hashListOrderMatters (fun (typar, ttype) -> hashTyparInclConstraints g typar @@ hashTType g ttype)
        |> pipeToHash (hashTopType g argInfos retTy [])
        |> pipeToHash (
            memberToParentInst
            |> hashListOrderMatters (fun (typar, ty) -> hashTyparRef typar @@ hashTType g ty)
        )
        |> pipeToHash (hashTyparDecls g methTypars)

    let hashMemberType (g: TcGlobals) vref typarInst argInfos retTy =
        match PartitionValRefTypars g vref with
        | Some(_, _, memberMethodTypars, memberToParentInst, _) ->
            hashMemberSigCore g memberToParentInst (typarInst, memberMethodTypars, argInfos, retTy)
        | None -> hashUncurriedSig g typarInst argInfos retTy

module HashTastMemberOrVals =
    open HashTypes

    let private hashMember (g: TcGlobals, observer) typarInst (v: Val) =
        let vref = mkLocalValRef v

        if HashAccessibility.isHiddenToObserver vref.Accessibility observer then
            0L
        else
            let membInfo = Option.get vref.MemberInfo
            let _tps, argInfos, retTy, _ = GetTypeOfMemberInFSharpForm g vref

            let memberFlagsHash = hashMemberFlags membInfo.MemberFlags
            let parentTypeHash = hashTyconRef membInfo.ApparentEnclosingEntity
            let memberTypeHash = hashMemberType g vref typarInst argInfos retTy
            let flagsHash = int64 (hash v.val_flags.PickledBits)
            let nameHash = hashText v.DisplayNameCoreMangled
            let attribsHash = hashAttributeList v.Attribs

            let combinedHash =
                memberFlagsHash
                @@ parentTypeHash
                @@ memberTypeHash
                @@ flagsHash
                @@ nameHash
                @@ attribsHash

            combinedHash

    /// Hash a constant value with exhaustive pattern matching over all Const cases
    let private hashConst (constVal: Const) : Hash =
        match constVal with
        | Const.Bool b -> int64 (hash b)
        | Const.SByte x -> int64 (hash x)
        | Const.Byte x -> int64 (hash x)
        | Const.Int16 x -> int64 (hash x)
        | Const.UInt16 x -> int64 (hash x)
        | Const.Int32 x -> int64 (hash x)
        | Const.UInt32 x -> int64 (hash x)
        | Const.Int64 x -> int64 (hash x)
        | Const.UInt64 x -> int64 (hash x)
        | Const.IntPtr x -> int64 (hash x)
        | Const.UIntPtr x -> int64 (hash x)
        | Const.Single x -> int64 (hash x)
        | Const.Double x -> int64 (hash x)
        | Const.Char x -> int64 (hash x)
        | Const.String x -> hashText x
        | Const.Decimal x -> int64 (hash x)
        | Const.Unit -> 0L
        | Const.Zero -> 0L

    let private hashNonMemberVal (g: TcGlobals, observer) (tps, v: Val, tau, cxs) =
        if HashAccessibility.isHiddenToObserver v.Accessibility observer then
            0L
        else
            let valReprInfo = arityOfValForDisplay v
            let nameHash = hashText v.DisplayNameCoreMangled
            let typarHash = hashTyparDecls g tps
            let argInfos, retTy = GetTopTauTypeInFSharpForm g valReprInfo.ArgInfos tau v.Range
            let typeHash = hashTopType g argInfos retTy cxs
            let flagsHash = int64 (hash v.val_flags.PickledBits)
            let attribsHash = hashAttributeList v.Attribs

            let combinedHash = nameHash @@ typarHash @@ typeHash @@ flagsHash @@ attribsHash

            // Include literal constant value in hash for deterministic builds
            match v.LiteralValue with
            | Some constVal ->
                let constHash = hashConst constVal
                combinedHash @@ constHash
            | None -> combinedHash

    let hashValOrMemberNoInst (g, obs) (vref: ValRef) =
        match vref.MemberInfo with
        | None ->
            let tps, tau = vref.GeneralizedType

            let cxs =
                tps
                |> Seq.collect (fun tp -> tp.Constraints |> Seq.map (fun cx -> struct (tp, cx)))

            hashNonMemberVal (g, obs) (tps, vref.Deref, tau, cxs)
        | Some _ -> hashMember (g, obs) emptyTyparInst vref.Deref

/// <summary>
/// StructuralUtilities: structural fingerprints of <c>TType</c>, used as cache keys.
///
/// A fingerprint is a sequence of tokens in which every node of a type writes a token fixing its number of
/// children before the children themselves, so that two different types never write the same sequence.
/// The same type can write different sequences (an abbreviation is not expanded below the root, a measure is
/// not normalised); that only costs a cache hit.
///
/// Entities, anonymous records and measure constants are identified by stamp. Type parameters are written in
/// one of two ways:
///   * by the position of their first appearance, for the typeSubsumptionCache: feasible subsumption treats any
///     type parameter as a wildcard, so 'a list and 'b list may share an entry.
///   * exactly, for keys that must tell apart everything a type means: a rigid type parameter by stamp, and
///     nothing at all for a type with an unsolved inference variable, whose meaning can still change.
///
/// Type constraints are never written. Nullness is written only on request.
/// </summary>
module StructuralUtilities =
    open Internal.Utilities.Library.Extras

    [<Struct; NoComparison; RequireQualifiedAccess>]
    type TypeToken =
        | Stamp of int
        | StampHigh of int
        | UCase of int
        | Nullness of int
        | StructTuple of int
        | RefTuple of int
        | Fun
        | Forall of int
        | MeasureOne
        | MeasureProduct
        | MeasureInverse
        | MeasureNumerator of int
        | MeasureDenominator of int
        | Unsolved of int
        | Rigid of int
        | Number of int

    let private hashTokenArray (tokens: TypeToken[]) : int =
        let mutable acc = 0L

        for t in tokens do
            acc <- combineHash acc (int64 (hash t))

        int acc

    [<CustomEquality; NoComparison>]
    type TypeStructure =
        | Stable of hash: int * tokens: TypeToken[]
        // Unstable means that the type structure of a given TType may change because of constraint solving or Trace.Undo.
        | Unstable of hash: int * tokens: TypeToken[]
        | PossiblyInfinite

        override this.GetHashCode() =
            match this with
            | Stable(h, _) -> h
            | Unstable(h, _) -> h ||| 0x40000000
            | PossiblyInfinite -> 0

        override this.Equals(obj) =
            match obj with
            | :? TypeStructure as other ->
                match this, other with
                | Stable(h1, a), Stable(h2, b) -> h1 = h2 && a = b
                | Unstable(h1, a), Unstable(h2, b) -> h1 = h2 && a = b
                | PossiblyInfinite, PossiblyInfinite -> true
                | _ -> false
            | _ -> false

    type private GenerationContext(maxTokenCount: int) =
        member val TyparMap = System.Collections.Generic.Dictionary<Stamp, int>(4)
        member val Tokens = ResizeArray<TypeToken>(maxTokenCount)
        member val Stable = true with get, set
        member val IsExact = false with get, set
        member val EmitNullness = false with get, set

        /// Set in exact mode once something was met that tokens cannot identify.
        member val IsRejected = false with get, set

        member val DependsOnFileBeingChecked = false with get, set

        member this.IsFull = this.Tokens.Count >= maxTokenCount

        member this.Reset() =
            this.TyparMap.Clear()
            this.Tokens.Clear()
            this.Stable <- true
            this.IsExact <- false
            this.EmitNullness <- false
            this.IsRejected <- false
            this.DependsOnFileBeingChecked <- false

        member this.Add(token: TypeToken) =
            if not this.IsFull then
                this.Tokens.Add token

        member this.GetOrAddTyparPosition(stamp: Stamp) =
            match this.TyparMap.TryGetValue stamp with
            | true, position -> position
            | _ ->
                let position = this.TyparMap.Count
                this.TyparMap[stamp] <- position
                position

    [<RequireQualifiedAccess>]
    module private NullnessTag =
        [<Literal>]
        let Ambivalent = 0

        [<Literal>]
        let WithNull = 1

        [<Literal>]
        let WithoutNull = 2

        // Checked like WithoutNull, except that it is never warned about.
        [<Literal>]
        let FromConstructor = 3

    let rec private emitNullness (ctx: GenerationContext) (n: Nullness) =
        if ctx.EmitNullness then
            match n with
            | Nullness.Known NullnessInfo.AmbivalentToNull -> ctx.Add(TypeToken.Nullness NullnessTag.Ambivalent)
            | Nullness.Known NullnessInfo.WithNull -> ctx.Add(TypeToken.Nullness NullnessTag.WithNull)
            | Nullness.Known NullnessInfo.WithoutNull -> ctx.Add(TypeToken.Nullness NullnessTag.WithoutNull)
            | Nullness.KnownFromConstructor -> ctx.Add(TypeToken.Nullness NullnessTag.FromConstructor)
            | Nullness.Variable v when v.IsSolved -> emitNullness ctx v.Solution
            | Nullness.Variable _ -> ctx.IsRejected <- true

    let private emitStamp (ctx: GenerationContext) (stamp: Stamp) =
        ctx.Add(TypeToken.Stamp(int (stamp &&& 0xFFFFFFFFL)))

        let high = stamp >>> 32

        if high <> 0L then
            ctx.Add(TypeToken.StampHigh(int high))

    let rec private emitMeasure (ctx: GenerationContext) (m: Measure) =
        if not ctx.IsFull then
            match m with
            | Measure.Var typar ->
                ctx.Stable <- false

                match typar.Solution with
                | Some ty -> emitTType ctx ty
                | None when ctx.IsExact && typar.Rigidity <> TyparRigidity.Rigid -> ctx.IsRejected <- true
                | None ->
                    if ctx.IsExact then
                        ctx.DependsOnFileBeingChecked <- true

                    emitStamp ctx typar.Stamp
            | Measure.Const(tcref, _) ->
                emitStamp ctx tcref.Stamp

                if ctx.IsExact && not tcref.IsAugmentationClosed then
                    ctx.DependsOnFileBeingChecked <- true
            | Measure.Prod(m1, m2, _) ->
                ctx.Add TypeToken.MeasureProduct
                emitMeasure ctx m1
                emitMeasure ctx m2
            | Measure.Inv m1 ->
                ctx.Add TypeToken.MeasureInverse
                emitMeasure ctx m1
            | Measure.One _ -> ctx.Add TypeToken.MeasureOne
            | Measure.RationalPower(m1, r) ->
                ctx.Add(TypeToken.MeasureNumerator(GetNumerator r))
                ctx.Add(TypeToken.MeasureDenominator(GetDenominator r))
                emitMeasure ctx m1

    and private emitTypar (ctx: GenerationContext) (typar: Typar) =
        // Solved may become unsolved, in case of Trace.Undo.
        if not typar.IsFromError then
            ctx.Stable <- false

        match typar.Solution with
        | Some ty -> emitTType ctx ty
        | None ->
            let isRigid = typar.Rigidity = TyparRigidity.Rigid

            if ctx.IsExact then
                if isRigid && not typar.IsFromError then
                    ctx.DependsOnFileBeingChecked <- true
                    emitStamp ctx typar.Stamp
                else
                    ctx.IsRejected <- true
            elif isRigid then
                ctx.Add(TypeToken.Rigid(ctx.GetOrAddTyparPosition typar.Stamp))
            else
                ctx.Add(TypeToken.Unsolved(ctx.GetOrAddTyparPosition typar.Stamp))

    and private emitTType (ctx: GenerationContext) (ty: TType) =
        if not ctx.IsFull then
            match ty with
            | TType_app(tcref, tinst, n) ->
                emitStamp ctx tcref.Stamp

                if ctx.IsExact && not tcref.IsAugmentationClosed then
                    ctx.DependsOnFileBeingChecked <- true

                emitNullness ctx n

                for arg in tinst do
                    emitTType ctx arg

            | TType_anon(info, tys) ->
                emitStamp ctx info.Stamp

                for arg in tys do
                    emitTType ctx arg

            | TType_tuple(tupInfo, tys) ->
                if evalTupInfoIsStruct tupInfo then
                    ctx.Add(TypeToken.StructTuple tys.Length)
                else
                    ctx.Add(TypeToken.RefTuple tys.Length)

                for arg in tys do
                    emitTType ctx arg

            | TType_fun(d, r, n) ->
                ctx.Add TypeToken.Fun
                emitNullness ctx n
                emitTType ctx d
                emitTType ctx r

            | TType_var(typar, n) ->
                emitNullness ctx n
                emitTypar ctx typar

            | TType_measure m -> emitMeasure ctx m

            // The two below are written by a hash of a name and by position: neither identifies exactly.
            | TType_ucase _
            | TType_forall _ when ctx.IsExact -> ctx.IsRejected <- true

            | TType_ucase(u, tinst) ->
                emitStamp ctx u.TyconRef.Stamp
                ctx.Add(TypeToken.UCase(int (hashText u.CaseName)))

                for arg in tinst do
                    emitTType ctx arg

            | TType_forall(tps, tau) ->
                for tp in tps do
                    ctx.TyparMap[tp.Stamp] <- ctx.TyparMap.Count

                ctx.Add(TypeToken.Forall tps.Length)
                emitTType ctx tau

    let private context =
        new System.Threading.ThreadLocal<GenerationContext>(fun () -> GenerationContext MaxTokenCount)

    let private getTypeStructureOfStrippedTypeUncached (ty: TType) =
        let ctx = context.Value
        ctx.Reset()
        emitTType ctx ty

        // If the sequence got too long, just drop it, we could be dealing with an infinite type.
        if ctx.IsFull then
            PossiblyInfinite
        else
            let tokens = ctx.Tokens.ToArray()
            let h = hashTokenArray tokens

            if not ctx.Stable then
                Unstable(h, tokens)
            else
                Stable(h, tokens)

    // Speed up repeated calls by memoizing results for types that yield a stable structure.
    let private getTypeStructureOfStrippedType =
        WeakMap.cacheConditionally
            (function
            | Stable _ -> true
            | _ -> false)
            getTypeStructureOfStrippedTypeUncached

    let tryGetTypeStructureOfStrippedType ty =
        match getTypeStructureOfStrippedType ty with
        | PossiblyInfinite -> ValueNone
        | ts -> ValueSome ts

    /// Writes several types, and the stamps and numbers that go with them, as one exact structure.
    /// Not thread-safe: use the builder of the current thread, and finish with it before asking for another.
    [<Sealed>]
    type ExactStructureBuilder private (ctx: GenerationContext) =

        static let builderOfThread =
            new System.Threading.ThreadLocal<ExactStructureBuilder>(fun () ->
                ExactStructureBuilder(GenerationContext MaxExactStructureTokenCount))

        /// True once something was added that tokens cannot identify, or once the structure got too long.
        member _.IsRejected = ctx.IsRejected || ctx.IsFull

        /// Whether something added so far is declared in the file being checked: a type, or a type parameter in scope
        member _.DependsOnFileBeingChecked = ctx.DependsOnFileBeingChecked

        member _.AddType(ty: TType) =
            if not ctx.IsRejected then
                emitTType ctx ty

        member _.AddStamp(stamp: Stamp) = emitStamp ctx stamp

        member _.AddNumber(number: int) = ctx.Add(TypeToken.Number number)

        member _.Reject() = ctx.IsRejected <- true

        member this.TryGetStructure() =
            if this.IsRejected then
                ValueNone
            else
                let tokens = ctx.Tokens.ToArray()
                ValueSome(Stable(hashTokenArray tokens, tokens))

        static member Start(emitNullness: bool) =
            let builder = builderOfThread.Value
            builder.Restart emitNullness
            builder

        member private _.Restart(emitNullness: bool) =
            ctx.Reset()
            ctx.IsExact <- true
            ctx.EmitNullness <- emitNullness
