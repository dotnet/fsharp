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

    type Hash = int

    let inline hashText (s: string) : Hash = hash s
    let inline combineHash acc y : Hash = (acc <<< 1) + y + 631
    let inline pipeToHash (value: Hash) (acc: Hash) = combineHash acc value
    let inline addFullStructuralHash (value) (acc: Hash) = combineHash (acc) (hash value)

    let inline hashListOrderMatters ([<InlineIfLambda>] func) (items: #seq<'T>) : Hash =
        let mutable acc = 0

        for i in items do
            let valHash = func i
            // We are calling hashListOrderMatters for things like list of types, list of properties, list of fields etc. The ones which are visibility-hidden will return 0, and are omitted.
            if valHash <> 0 then
                acc <- combineHash acc valHash

        acc

    let inline hashListOrderIndependent ([<InlineIfLambda>] func) (items: #seq<'T>) : Hash =
        let mutable acc = 0

        for i in items do
            let valHash = func i
            acc <- acc ^^^ valHash

        acc

    let (@@) (h1: Hash) (h2: Hash) = combineHash h1 h2

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

        (hash tag) @@ (hashText name)

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
        |> addFullStructuralHash tref.Name

    let private hashILArrayShape (sh: ILArrayShape) = sh.Rank

    let rec hashILType (ty: ILType) : Hash =
        match ty with
        | ILType.Void -> hash ILType.Void
        | ILType.Array(sh, t) -> hashILType t @@ hashILArrayShape sh
        | ILType.Value t
        | ILType.Boxed t -> hashILTypeRef t.TypeRef @@ (t.GenericArgs |> hashListOrderMatters (hashILType))
        | ILType.Ptr t
        | ILType.Byref t -> hashILType t
        | ILType.FunctionPointer t -> hashILCallingSignature t
        | ILType.TypeVar n -> hash n
        | ILType.Modified(_, _, t) -> hashILType t

    and hashILCallingSignature (signature: ILCallingSignature) =
        let res = signature.ReturnType |> hashILType
        signature.ArgTypes |> hashListOrderMatters (hashILType) |> pipeToHash res

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
    open Microsoft.FSharp.Core.LanguagePrimitives

    let stampEquals g ty1 ty2 =
        match (stripTyEqns g ty1), (stripTyEqns g ty2) with
        | TType_app(tcref1, _, _), TType_app(tcref2, _, _) -> tcref1.Stamp.Equals(tcref2.Stamp)
        | TType_var(r1, _), TType_var(r2, _) -> r1.Stamp.Equals(r2.Stamp)
        | _ -> false

    /// Get has for Stamp for TType_app tyconref and TType_var typar
    let hashStamp g ty =
        let v: Stamp =
            match (stripTyEqns g ty) with
            | TType_app(tcref, _, _) -> tcref.Stamp
            | TType_var(r, _) -> r.Stamp
            | _ -> GenericZero

        hash v

    /// Hash a reference to a type
    let hashTyconRef tcref = hashTyconRefImpl tcref

    /// Hash the flags of a member
    let hashMemberFlags (memFlags: SynMemberFlags) = hash memFlags

    /// Hash an attribute 'Type(arg1, ..., argN)'
    let private hashAttrib (Attrib(tyconRef = tcref)) = hashTyconRefImpl tcref

    let hashAttributeList attrs =
        attrs |> hashListOrderIndependent hashAttrib

    let private hashTyparRef (typar: Typar) =
        hashText typar.DisplayName
        |> addFullStructuralHash (typar.Rigidity)
        |> addFullStructuralHash (typar.StaticReq)

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
        | TyparConstraint.NotSupportsNull(_) -> tpHash @@ 14
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
        |> pipeToHash (nameHash)
        |> pipeToHash (returnTypeHash)
        |> pipeToHash memberHash

    /// Hash a unit of measure expression
    let private hashMeasure unt =
        let measuresWithExponents =
            ListMeasureVarOccsWithNonZeroExponents unt
            |> List.sortBy (fun (tp: Typar, _) -> tp.DisplayName)

        measuresWithExponents
        |> hashListOrderIndependent (fun (typar, exp: Rational) -> hashTyparRef typar @@ hash exp)

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
        | TType_forall(tps, tau) -> tps |> hashListOrderMatters (hashTyparRef) |> pipeToHash (hashTType g tau)
        | TType_fun _ ->
            let argTys, retTy = stripFunTy g ty
            argTys |> hashListOrderMatters (hashTType g) |> pipeToHash (hashTType g retTy)
        | TType_var(r, _) -> hashTyparRefWithInfo r
        | TType_measure unt -> hashMeasure unt

    // Hash a single argument, including its name and type
    let private hashArgInfo (g: TcGlobals) (ty, argInfo: ArgReprInfo) =

        let attributesHash = hashAttributeList argInfo.Attribs

        let nameHash =
            match argInfo.Name with
            | Some i -> hashText i.idText
            | _ -> -1

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
            0
        else
            let membInfo = Option.get vref.MemberInfo
            let _tps, argInfos, retTy, _ = GetTypeOfMemberInFSharpForm g vref

            let memberFlagsHash = hashMemberFlags membInfo.MemberFlags
            let parentTypeHash = hashTyconRef membInfo.ApparentEnclosingEntity
            let memberTypeHash = hashMemberType g vref typarInst argInfos retTy
            let flagsHash = hash v.val_flags.PickledBits
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

    let private hashNonMemberVal (g: TcGlobals, observer) (tps, v: Val, tau, cxs) =
        if HashAccessibility.isHiddenToObserver v.Accessibility observer then
            0
        else
            let valReprInfo = arityOfValForDisplay v
            let nameHash = hashText v.DisplayNameCoreMangled
            let typarHash = hashTyparDecls g tps
            let argInfos, retTy = GetTopTauTypeInFSharpForm g valReprInfo.ArgInfos tau v.Range
            let typeHash = hashTopType g argInfos retTy cxs
            let flagsHash = hash v.val_flags.PickledBits
            let attribsHash = hashAttributeList v.Attribs

            let combinedHash = nameHash @@ typarHash @@ typeHash @@ flagsHash @@ attribsHash
            combinedHash

    let hashValOrMemberNoInst (g, obs) (vref: ValRef) =
        match vref.MemberInfo with
        | None ->
            let tps, tau = vref.GeneralizedType

            let cxs =
                tps
                |> Seq.collect (fun tp -> tp.Constraints |> Seq.map (fun cx -> struct (tp, cx)))

            hashNonMemberVal (g, obs) (tps, vref.Deref, tau, cxs)
        | Some _ -> hashMember (g, obs) emptyTyparInst vref.Deref
