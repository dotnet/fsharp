module internal Fsharp.Compiler.SignatureHash

open Internal.Utilities.Library
open Internal.Utilities.Rational
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.Syntax
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.CheckDeclarations

type ObserverVisibility =
    | PublicOnly
    | PublicAndInternal

[<AutoOpen>]
module internal HashingPrimitives =

    type Hash = int

    let inline hashText (s: string) : Hash = hash s
    let inline private combineHash acc y : Hash = (acc <<< 1) + y + 631
    let inline pipeToHash (value: Hash) (acc: Hash) = combineHash acc value
    let inline addFullStructuralHash (value) (acc: Hash) = combineHash (acc) (hash value)

    let inline hashListOrderMatters ([<InlineIfLambda>] func) (items: #seq<'T>) : Hash =
        let mutable acc = 0

        for i in items do
            let valHash = func i
            // We are calling hashListOrderMatters for things like list of types, list of properties, list of fields etc. The ones which are visibility-hidden will return 0, and are ommited.
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
        | ILType.Array (sh, t) -> hashILType t @@ hashILArrayShape sh
        | ILType.Value t
        | ILType.Boxed t -> hashILTypeRef t.TypeRef @@ (t.GenericArgs |> hashListOrderMatters (hashILType))
        | ILType.Ptr t
        | ILType.Byref t -> hashILType t
        | ILType.FunctionPointer t -> hashILCallingSignature t
        | ILType.TypeVar n -> hash n
        | ILType.Modified (_, _, t) -> hashILType t

    and hashILCallingSignature (signature: ILCallingSignature) =
        let res = signature.ReturnType |> hashILType
        signature.ArgTypes |> hashListOrderMatters (hashILType) |> pipeToHash res

module HashAccessibility =

    let isHiddenToObserver (TAccess access) (observer: ObserverVisibility) =
        let isInternalCompPath x =
            match x with
            | CompPath (ILScopeRef.Local, []) -> true
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
    let hashMemberFlags (memFlags: SynMemberFlags) = hash memFlags

    /// Hash an attribute 'Type(arg1, ..., argN)'
    let private hashAttrib (Attrib (tyconRef = tcref)) = hashTyconRefImpl tcref

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
        | TyparConstraint.CoercesTo (tgtTy, _) -> tpHash @@ 1 @@ hashTType g tgtTy
        | TyparConstraint.MayResolveMember (traitInfo, _) -> tpHash @@ 2 @@ hashTraitWithInfo (* denv *) g traitInfo
        | TyparConstraint.DefaultsTo (_, ty, _) -> tpHash @@ 3 @@ hashTType g ty
        | TyparConstraint.IsEnum (ty, _) -> tpHash @@ 4 @@ hashTType g ty
        | TyparConstraint.SupportsComparison _ -> tpHash @@ 5
        | TyparConstraint.SupportsEquality _ -> tpHash @@ 6
        | TyparConstraint.IsDelegate (aty, bty, _) -> tpHash @@ 7 @@ hashTType g aty @@ hashTType g bty
        | TyparConstraint.SupportsNull _ -> tpHash @@ 8
        | TyparConstraint.IsNonNullableStruct _ -> tpHash @@ 9
        | TyparConstraint.IsUnmanaged _ -> tpHash @@ 10
        | TyparConstraint.IsReferenceType _ -> tpHash @@ 11
        | TyparConstraint.SimpleChoice (tys, _) -> tpHash @@ 12 @@ (tys |> hashListOrderIndependent (hashTType g))
        | TyparConstraint.RequiresDefaultConstructor _ -> tpHash @@ 13

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
        | TType_ucase (UnionCaseRef (tc, _), args)
        | TType_app (tc, args, _) -> args |> hashListOrderMatters (hashTType g) |> pipeToHash (hashTyconRef tc)
        | TType_anon (anonInfo, tys) ->
            tys
            |> hashListOrderMatters (hashTType g)
            |> pipeToHash (anonInfo.SortedNames |> hashListOrderMatters hashText)
            |> addFullStructuralHash (evalAnonInfoIsStruct anonInfo)
        | TType_tuple (tupInfo, t) ->
            t
            |> hashListOrderMatters (hashTType g)
            |> addFullStructuralHash (evalTupInfoIsStruct tupInfo)
        // Hash a first-class generic type.
        | TType_forall (tps, tau) -> tps |> hashListOrderMatters (hashTyparRef) |> pipeToHash (hashTType g tau)
        | TType_fun _ ->
            let argTys, retTy = stripFunTy g ty
            argTys |> hashListOrderMatters (hashTType g) |> pipeToHash (hashTType g retTy)
        | TType_var (r, _) -> hashTyparRefWithInfo r
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
        | Some (_, _, memberMethodTypars, memberToParentInst, _) ->
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
                @@ parentTypeHash @@ memberTypeHash @@ flagsHash @@ nameHash @@ attribsHash

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

//-------------------------------------------------------------------------

/// Printing TAST objects
module TyconDefinitionHash =
    open HashTypes

    let private hashRecdField (g: TcGlobals, observer) (fld: RecdField) =
        if HashAccessibility.isHiddenToObserver fld.Accessibility observer then
            0
        else
            let nameHash = hashText fld.DisplayNameCore

            let attribHash =
                hashAttributeList fld.FieldAttribs @@ hashAttributeList fld.PropertyAttribs

            let typeHash = hashTType g fld.FormalType

            let combined =
                nameHash
                @@ attribHash
                   @@ typeHash @@ (hash fld.IsStatic) @@ (hash fld.IsVolatile) @@ (hash fld.IsMutable)

            combined

    let private hashUnionCase (g: TcGlobals, observer) (ucase: UnionCase) =
        if HashAccessibility.isHiddenToObserver ucase.Accessibility observer then
            0
        else
            let nameHash = hashText ucase.Id.idText
            let attribHash = hashAttributeList ucase.Attribs

            ucase.RecdFieldsArray
            |> hashListOrderMatters (fun rf -> hashRecdField (g, observer) rf)
            |> pipeToHash nameHash
            |> pipeToHash attribHash

    let private hashUnionCases (g, obs) ucases =
        ucases
        // Why order matters here?
        // Union cases come with generated Tag members, on which code in higher-level project can depend -> if order of union cases changes, higher-level project has to be also recompiled.
        // Correct me if I am wrong here pls.
        |> hashListOrderMatters (hashUnionCase (g, obs))

    let private hashFsharpDelegate g slotSig =
        let (TSlotSig (_, _, _, _, paraml, retTy)) = slotSig

        (paraml
         |> hashListOrderMatters (fun pl -> pl |> hashListOrderMatters (fun sp -> hashTType g sp.Type)))
        |> pipeToHash (hashTType g (GetFSharpViewOfReturnType g retTy))

    let private hashFsharpEnum (tycon: Tycon) =
        tycon.AllFieldsArray
        |> hashListOrderIndependent (fun f -> hashText f.DisplayNameCore)

    let private hashTyconDefn (g, observer) (tcref: TyconRef) =
        let tycon = tcref.Deref

        if HashAccessibility.isHiddenToObserver tycon.Accessibility observer then
            0
        else

            let repr = tycon.TypeReprInfo

            let tyconHash = HashTypes.hashTyconRef tcref
            let attribHash = hashAttributeList tcref.Attribs
            let typarsHash = hashTyparDecls g tycon.TyparsNoRange
            let topLevelDeclarationHash = tyconHash @@ attribHash @@ typarsHash

            // Interface implementation
            let iimplsHash () =
                tycon.ImmediateInterfacesOfFSharpTycon
                |> hashListOrderIndependent (fun (ttype, _, _) -> hashTType g ttype)

            // Fields, static fields, val declarations
            let fieldsHash () =
                tycon.AllFieldsArray |> hashListOrderIndependent (hashRecdField (g, observer))

            /// Properties, methods, constructors
            let membersHash () =
                tycon.MembersOfFSharpTyconByName
                |> hashListOrderIndependent (fun kvp ->
                    kvp.Value
                    |> hashListOrderIndependent (HashTastMemberOrVals.hashValOrMemberNoInst (g, observer)))

            /// Super type or obj
            let inheritsHash () = superOfTycon g tycon |> hashTType g

            let specializedHash =
                match repr with
                | TFSharpRecdRepr _ -> fieldsHash ()
                | TFSharpUnionRepr _ -> hashUnionCases (g, observer) tycon.UnionCasesArray
                | TFSharpObjectRepr {
                                        fsobjmodel_kind = TFSharpDelegate slotSig
                                    } -> hashFsharpDelegate g slotSig
                | TFSharpObjectRepr { fsobjmodel_kind = TFSharpEnum } -> hashFsharpEnum tycon
                | TFSharpObjectRepr {
                                        fsobjmodel_kind = TFSharpClass | TFSharpInterface | TFSharpStruct as tfor
                                    } ->
                    iimplsHash () @@ fieldsHash () @@ membersHash () @@ inheritsHash ()
                    |> pipeToHash (
                        match tfor with
                        | TFSharpClass -> 1
                        | TFSharpInterface -> 2
                        | TFSharpStruct -> 3
                        | _ -> 4
                    )
                | TAsmRepr ilType -> HashIL.hashILType ilType
                | TMeasureableRepr ty -> hashTType g ty
                | TILObjectRepr _ -> iimplsHash () @@ fieldsHash () @@ membersHash () @@ inheritsHash ()
                | TNoRepr when tycon.TypeAbbrev.IsSome ->
                    let abbreviatedTy = tycon.TypeAbbrev.Value
                    hashTType g abbreviatedTy
                | TNoRepr when tycon.IsFSharpException ->
                    match tycon.ExceptionInfo with
                    | TExnAbbrevRepr exnTcRef -> hashTyconRef exnTcRef
                    | TExnAsmRepr iLTypeRef -> HashIL.hashILTypeRef iLTypeRef
                    | TExnNone -> 0
                    | TExnFresh _ -> fieldsHash ()

#if !NO_TYPEPROVIDERS
                | TProvidedNamespaceRepr _
                | TProvidedTypeRepr _
#endif
                | TNoRepr -> iimplsHash () @@ fieldsHash () @@ membersHash () @@ inheritsHash ()

            specializedHash |> pipeToHash topLevelDeclarationHash

    // Hash: module spec

    let hashTyconDefns (g, obs) (tycons: Tycon list) =
        tycons
        |> hashListOrderIndependent (mkLocalEntityRef >> (hashTyconDefn (g, obs)))

    let rec fullPath (mspec: ModuleOrNamespace) acc =
        if mspec.IsNamespace then
            match mspec.ModuleOrNamespaceType.ModuleAndNamespaceDefinitions |> List.tryHead with
            | Some next when next.IsNamespace -> fullPath next (acc @ [ next.DisplayNameCore ])
            | _ -> acc, mspec
        else
            acc, mspec

let calculateHashOfImpliedSignature g observer (expr: ModuleOrNamespaceContents) =

    let rec hashModuleOrNameSpaceBinding (monb: ModuleOrNamespaceBinding) =
        match monb with
        | ModuleOrNamespaceBinding.Binding b -> HashTastMemberOrVals.hashValOrMemberNoInst (g, observer) (mkLocalValRef b.Var)
        | ModuleOrNamespaceBinding.Module (moduleInfo, contents) -> hashSingleModuleOrNameSpaceIncludingName (moduleInfo, contents)

    and hashSingleModuleOrNamespaceContents x =
        match x with
        | TMDefRec (_, _opens, tycons, mbinds, _) ->
            let mbindsHash = 
                mbinds
                |> hashListOrderIndependent (hashModuleOrNameSpaceBinding)

            let tyconsHash = TyconDefinitionHash.hashTyconDefns (g, observer) tycons

            if mbindsHash <> 0 || tyconsHash <> 0 then
                mbindsHash @@ tyconsHash
            else
                0

        | TMDefLet (bind, _) -> HashTastMemberOrVals.hashValOrMemberNoInst (g, observer) (mkLocalValRef bind.Var)
        | TMDefOpens _ -> 0 (* empty hash *)
        | TMDefs defs -> defs |> hashListOrderIndependent hashSingleModuleOrNamespaceContents
        | TMDefDo _ -> 0 (* empty hash *)

    and hashSingleModuleOrNameSpaceIncludingName (mspec, def) =
        if HashAccessibility.isHiddenToObserver mspec.Accessibility observer then
            0
        else
            let outerPathHash =
                mspec.CompilationPath.MangledPath |> hashListOrderMatters hashText

            let thisNameHash = hashText mspec.entity_logical_name

            let fullNameHash = outerPathHash @@ thisNameHash @@ (hash mspec.IsModule)
            let contentHash = hashSingleModuleOrNamespaceContents def

            if contentHash = 0 then 0 else fullNameHash @@ contentHash

    hashSingleModuleOrNamespaceContents expr

let calculateSignatureHashOfFiles (files: CheckedImplFile list) g observer =
    use _ =
        FSharp.Compiler.Diagnostics.Activity.startNoTags "calculateSignatureHashOfFiles"

    files
    |> hashListOrderMatters (fun f -> calculateHashOfImpliedSignature g observer f.Contents)

let calculateHashOfAssemblyTopAttributes (attrs: TopAttribs) (platform: ILPlatform option) =
    let platformHash =
        match platform with
        | None -> 0
        | Some AMD64 -> 1
        | Some IA64 -> 2
        | Some ARM -> 3
        | Some ARM64 -> 4
        | Some X86 -> 5

    HashTypes.hashAttributeList attrs.assemblyAttrs
    @@ HashTypes.hashAttributeList attrs.mainMethodAttrs
       @@ HashTypes.hashAttributeList attrs.netModuleAttrs @@ platformHash
