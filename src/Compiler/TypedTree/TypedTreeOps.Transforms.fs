// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Defines derived expression manipulation and construction functions.
namespace FSharp.Compiler.TypedTreeOps

open System
open System.CodeDom.Compiler
open System.Collections.Generic
open System.Collections.Immutable
open Internal.Utilities
open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open Internal.Utilities.Rational

open FSharp.Compiler
open FSharp.Compiler.IO
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open FSharp.Compiler.Syntax
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Text.Layout
open FSharp.Compiler.Text.LayoutRender
open FSharp.Compiler.Text.TaggedText
open FSharp.Compiler.Xml
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
#if !NO_TYPEPROVIDERS
open FSharp.Compiler.TypeProviders
#endif

[<AutoOpen>]
module internal XmlDocSignatures =

    let commaEncs strs = String.concat "," strs
    let angleEnc str = "{" + str + "}"

    let ticksAndArgCountTextOfTyconRef (tcref: TyconRef) =
        let path = Array.toList (fullMangledPathToTyconRef tcref) @ [ tcref.CompiledName ]
        textOfPath path

    let typarEnc (_g: TcGlobals) (gtpsType, gtpsMethod) typar =
        match List.tryFindIndex (typarEq typar) gtpsType with
        | Some idx -> "`" + string idx
        | None ->
            match List.tryFindIndex (typarEq typar) gtpsMethod with
            | Some idx -> "``" + string idx
            | None ->
                warning (InternalError("Typar not found during XmlDoc generation", typar.Range))
                "``0"

    let rec typeEnc g (gtpsType, gtpsMethod) ty =
        let stripped = stripTyEqnsAndMeasureEqns g ty

        match stripped with
        | TType_forall _ -> "Microsoft.FSharp.Core.FSharpTypeFunc"

        | _ when isByrefTy g ty ->
            let ety = destByrefTy g ty
            typeEnc g (gtpsType, gtpsMethod) ety + "@"

        | _ when isNativePtrTy g ty ->
            let ety = destNativePtrTy g ty
            typeEnc g (gtpsType, gtpsMethod) ety + "*"

        | TType_app(_, _, _nullness) when isArrayTy g ty ->
            let tcref, tinst = destAppTy g ty
            let rank = rankOfArrayTyconRef g tcref
            let arraySuffix = "[" + String.concat ", " (List.replicate (rank - 1) "0:") + "]"
            typeEnc g (gtpsType, gtpsMethod) (List.head tinst) + arraySuffix

        | TType_ucase(_, tinst)
        | TType_app(_, tinst, _) ->
            let tyName =
                let ty = stripTyEqnsAndMeasureEqns g ty

                match ty with
                | TType_app(tcref, _tinst, _nullness) ->
                    // Generic type names are (name + "`" + digits) where name does not contain "`".
                    // In XML doc, when used in type instances, these do not use the ticks.
                    let path = Array.toList (fullMangledPathToTyconRef tcref) @ [ tcref.CompiledName ]
                    textOfPath (List.map DemangleGenericTypeName path)
                | _ ->
                    assert false
                    failwith "impossible"

            tyName + tyargsEnc g (gtpsType, gtpsMethod) tinst

        | TType_anon(anonInfo, tinst) -> sprintf "%s%s" anonInfo.ILTypeRef.FullName (tyargsEnc g (gtpsType, gtpsMethod) tinst)

        | TType_tuple(tupInfo, tys) ->
            if evalTupInfoIsStruct tupInfo then
                sprintf "System.ValueTuple%s" (tyargsEnc g (gtpsType, gtpsMethod) tys)
            else
                sprintf "System.Tuple%s" (tyargsEnc g (gtpsType, gtpsMethod) tys)

        | TType_fun(domainTy, rangeTy, _nullness) ->
            "Microsoft.FSharp.Core.FSharpFunc"
            + tyargsEnc g (gtpsType, gtpsMethod) [ domainTy; rangeTy ]

        | TType_var(typar, _nullness) -> typarEnc g (gtpsType, gtpsMethod) typar

        | TType_measure _ -> "?"

    and tyargsEnc g (gtpsType, gtpsMethod) args =
        match args with
        | [] -> ""
        | [ a ] when
            (match (stripTyEqns g a) with
             | TType_measure _ -> true
             | _ -> false)
            ->
            "" // float<m> should appear as just "float" in the generated .XML xmldoc file
        | _ -> angleEnc (commaEncs (List.map (typeEnc g (gtpsType, gtpsMethod)) args))

    let XmlDocArgsEnc g (gtpsType, gtpsMethod) argTys =
        if isNil argTys then
            ""
        else
            "("
            + String.concat "," (List.map (typeEnc g (gtpsType, gtpsMethod)) argTys)
            + ")"

    let buildAccessPath (cp: CompilationPath option) =
        match cp with
        | Some cp ->
            let ap = cp.AccessPath |> List.map fst |> List.toArray
            String.Join(".", ap)
        | None -> "Extension Type"

    let prependPath path name =
        if String.IsNullOrEmpty(path) then
            name
        else
            !!path + "." + name

    let XmlDocSigOfVal g full path (v: Val) =
        let parentTypars, methTypars, cxs, argInfos, retTy, prefix, path, name =

            // CLEANUP: this is one of several code paths that treat module values and members
            // separately when really it would be cleaner to make sure GetValReprTypeInFSharpForm, GetMemberTypeInFSharpForm etc.
            // were lined up so code paths like this could be uniform

            match v.MemberInfo with
            | Some membInfo when not v.IsExtensionMember ->

                // Methods, Properties etc.
                let numEnclosingTypars = CountEnclosingTyparsOfActualParentOfVal v

                let tps, witnessInfos, argInfos, retTy, _ =
                    GetMemberTypeInMemberForm g membInfo.MemberFlags (Option.get v.ValReprInfo) numEnclosingTypars v.Type v.Range

                let prefix, name =
                    match membInfo.MemberFlags.MemberKind with
                    | SynMemberKind.ClassConstructor
                    | SynMemberKind.Constructor -> "M:", "#ctor"
                    | SynMemberKind.Member -> "M:", v.CompiledName g.CompilerGlobalState
                    | SynMemberKind.PropertyGetSet
                    | SynMemberKind.PropertySet
                    | SynMemberKind.PropertyGet ->
                        let prefix =
                            if attribsHaveValFlag g WellKnownValAttributes.CLIEventAttribute v.Attribs then
                                "E:"
                            else
                                "P:"

                        prefix, v.PropertyName

                let path =
                    if v.HasDeclaringEntity then
                        prependPath path v.DeclaringEntity.CompiledName
                    else
                        path

                let parentTypars, methTypars =
                    match PartitionValTypars g v with
                    | Some(_, memberParentTypars, memberMethodTypars, _, _) -> memberParentTypars, memberMethodTypars
                    | None -> [], tps

                parentTypars, methTypars, witnessInfos, argInfos, retTy, prefix, path, name

            | _ ->
                // Regular F# values and extension members
                let w = arityOfVal v
                let numEnclosingTypars = CountEnclosingTyparsOfActualParentOfVal v

                let tps, witnessInfos, argInfos, retTy, _ =
                    GetValReprTypeInCompiledForm g w numEnclosingTypars v.Type v.Range

                let name = v.CompiledName g.CompilerGlobalState
                let prefix = if w.NumCurriedArgs = 0 && isNil tps then "P:" else "M:"
                [], tps, witnessInfos, argInfos, retTy, prefix, path, name

        let witnessArgTys = GenWitnessTys g cxs
        let argTys = argInfos |> List.concat |> List.map fst

        let argTys =
            witnessArgTys
            @ argTys
            @ (match retTy with
               | Some t when full -> [ t ]
               | _ -> [])

        let args = XmlDocArgsEnc g (parentTypars, methTypars) argTys
        let arity = List.length methTypars
        let genArity = if arity = 0 then "" else sprintf "``%d" arity
        prefix + prependPath path name + genArity + args

    let BuildXmlDocSig prefix path = prefix + List.fold prependPath "" path

    // Would like to use "U:", but ParseMemberSignature only accepts C# signatures
    let XmlDocSigOfUnionCase path = BuildXmlDocSig "T:" path

    let XmlDocSigOfField path = BuildXmlDocSig "F:" path

    let XmlDocSigOfProperty path = BuildXmlDocSig "P:" path

    let XmlDocSigOfTycon path = BuildXmlDocSig "T:" path

    let XmlDocSigOfSubModul path = BuildXmlDocSig "T:" path

    let XmlDocSigOfEntity (eref: EntityRef) =
        XmlDocSigOfTycon [ (buildAccessPath eref.CompilationPathOpt); eref.Deref.CompiledName ]

    //---------------------------------------------------------------------------
    // Active pattern name helpers
    //---------------------------------------------------------------------------

    let TryGetActivePatternInfo (vref: ValRef) =
        // First is an optimization to prevent calls to string routines
        let logicalName = vref.LogicalName

        if logicalName.Length = 0 || logicalName[0] <> '|' then
            None
        else
            ActivePatternInfoOfValName vref.DisplayNameCoreMangled vref.Range

    type ActivePatternElemRef with
        member x.LogicalName =
            let (APElemRef(_, vref, n, _)) = x

            match TryGetActivePatternInfo vref with
            | None -> error (InternalError("not an active pattern name", vref.Range))
            | Some apinfo ->
                let nms = apinfo.ActiveTags

                if n < 0 || n >= List.length nms then
                    error (InternalError("name_of_apref: index out of range for active pattern reference", vref.Range))

                List.item n nms

        member x.DisplayNameCore = x.LogicalName

        member x.DisplayName = x.LogicalName |> ConvertLogicalNameToDisplayName

    let mkChoiceTyconRef (g: TcGlobals) m n =
        match n with
        | 0
        | 1 -> error (InternalError("mkChoiceTyconRef", m))
        | 2 -> g.choice2_tcr
        | 3 -> g.choice3_tcr
        | 4 -> g.choice4_tcr
        | 5 -> g.choice5_tcr
        | 6 -> g.choice6_tcr
        | 7 -> g.choice7_tcr
        | _ -> error (Error(FSComp.SR.tastActivePatternsLimitedToSeven (), m))

    let mkChoiceTy (g: TcGlobals) m tinst =
        match List.length tinst with
        | 0 -> g.unit_ty
        | 1 -> List.head tinst
        | length -> mkWoNullAppTy (mkChoiceTyconRef g m length) tinst

    let mkChoiceCaseRef g m n i =
        mkUnionCaseRef (mkChoiceTyconRef g m n) ("Choice" + string (i + 1) + "Of" + string n)

    type ActivePatternInfo with

        member x.DisplayNameCoreByIdx idx = x.ActiveTags[idx]

        member x.DisplayNameByIdx idx =
            x.ActiveTags[idx] |> ConvertLogicalNameToDisplayName

        member apinfo.ResultType g m retTys retKind =
            let choicety = mkChoiceTy g m retTys

            if apinfo.IsTotal then
                choicety
            else
                match retKind with
                | ActivePatternReturnKind.RefTypeWrapper -> mkOptionTy g choicety
                | ActivePatternReturnKind.StructTypeWrapper -> mkValueOptionTy g choicety
                | ActivePatternReturnKind.Boolean -> g.bool_ty

        member apinfo.OverallType g m argTy retTys retKind =
            mkFunTy g argTy (apinfo.ResultType g m retTys retKind)

    //---------------------------------------------------------------------------
    // Active pattern validation
    //---------------------------------------------------------------------------

    // check if an active pattern takes type parameters only bound by the return types,
    // not by their argument types.
    let doesActivePatternHaveFreeTypars g (v: ValRef) =
        let vty = v.TauType
        let vtps = v.Typars |> Zset.ofList typarOrder

        if not (isFunTy g v.TauType) then
            errorR (Error(FSComp.SR.activePatternIdentIsNotFunctionTyped (v.LogicalName), v.Range))

        let argTys, resty = stripFunTy g vty

        let argtps, restps =
            (freeInTypes CollectTypars argTys).FreeTypars, (freeInType CollectTypars resty).FreeTypars
        // Error if an active pattern is generic in type variables that only occur in the result Choice<_, ...>.
        // Note: The test restricts to v.Typars since typars from the closure are considered fixed.
        not (Zset.isEmpty (Zset.inter (Zset.diff restps argtps) vtps))

[<AutoOpen>]
module internal NullnessAnalysis =

    let inline HasConstraint ([<InlineIfLambda>] predicate) (tp: Typar) = tp.Constraints |> List.exists predicate

    let inline tryGetTyparTyWithConstraint g ([<InlineIfLambda>] predicate) ty =
        match tryDestTyparTy g ty with
        | ValueSome tp as x when HasConstraint predicate tp -> x
        | _ -> ValueNone

    let inline IsTyparTyWithConstraint g ([<InlineIfLambda>] predicate) ty =
        match tryDestTyparTy g ty with
        | ValueSome tp -> HasConstraint predicate tp
        | ValueNone -> false

    // Note, isStructTy does not include type parameters with the ': struct' constraint
    // This predicate is used to detect those type parameters.
    let IsNonNullableStructTyparTy g ty =
        ty |> IsTyparTyWithConstraint g _.IsIsNonNullableStruct

    // Note, isRefTy does not include type parameters with the ': not struct' or ': null' constraints
    // This predicate is used to detect those type parameters.
    let IsReferenceTyparTy g ty =
        ty
        |> IsTyparTyWithConstraint g (fun tc -> tc.IsIsReferenceType || tc.IsSupportsNull)

    let GetTyparTyIfSupportsNull g ty =
        ty |> tryGetTyparTyWithConstraint g _.IsSupportsNull

    let TypeNullNever g ty =
        let underlyingTy = stripTyEqnsAndMeasureEqns g ty

        isStructTy g underlyingTy
        || isByrefTy g underlyingTy
        || IsNonNullableStructTyparTy g ty

    /// The pre-nullness logic about whether a type admits the use of 'null' as a value.
    let TypeNullIsExtraValue g (_m: range) ty =
        if isILReferenceTy g ty || isDelegateTy g ty then
            match tryTcrefOfAppTy g ty with
            | ValueSome tcref ->
                // Putting AllowNullLiteralAttribute(false) on an IL or provided
                // type means 'null' can't be used with that type, otherwise it can
                TyconRefAllowsNull g tcref <> Some false
            | _ ->
                // In pre-nullness, other IL reference types (e.g. arrays) always support null
                true
        elif TypeNullNever g ty then
            false
        else
            // In F# 4.x, putting AllowNullLiteralAttribute(true) on an F# type means 'null' can be used with that type
            match tryTcrefOfAppTy g ty with
            | ValueSome tcref -> TyconRefAllowsNull g tcref = Some true
            | ValueNone ->

                // Consider type parameters
                (GetTyparTyIfSupportsNull g ty).IsSome

    // Any mention of a type with AllowNullLiteral(true) is considered to be with-null
    let intrinsicNullnessOfTyconRef g (tcref: TyconRef) =
        match TyconRefAllowsNull g tcref with
        | Some true -> g.knownWithNull
        | _ -> g.knownWithoutNull

    let nullnessOfTy g ty =
        ty
        |> stripTyEqns g
        |> function
            | TType_app(tcref, _, nullness) ->
                let nullness2 = intrinsicNullnessOfTyconRef g tcref

                if nullness2 === g.knownWithoutNull then
                    nullness
                else
                    combineNullness nullness nullness2
            | TType_fun(_, _, nullness)
            | TType_var(_, nullness) -> nullness
            | _ -> g.knownWithoutNull

    let changeWithNullReqTyToVariable g reqTy =
        let sty = stripTyEqns g reqTy

        match isTyparTy g sty with
        | false ->
            match nullnessOfTy g sty with
            | Nullness.Known NullnessInfo.AmbivalentToNull
            | Nullness.Known NullnessInfo.WithNull when g.checkNullness -> reqTy |> replaceNullnessOfTy (NewNullnessVar())
            | _ -> reqTy
        | true -> reqTy

    /// When calling a null-allowing API, we prefer to infer a without null argument for idiomatic F# code.
    /// That is, unless caller explicitly marks a value (e.g. coming from a function parameter) as WithNull, it should not be inferred as such.
    let reqTyForArgumentNullnessInference g actualTy reqTy =
        // Only change reqd nullness if actualTy is an inference variable
        match tryDestTyparTy g actualTy with
        | ValueSome t when t.IsCompilerGenerated && not (t |> HasConstraint _.IsSupportsNull) -> changeWithNullReqTyToVariable g reqTy
        | _ -> reqTy

    let GetDisallowedNullness (g: TcGlobals) (ty: TType) =
        if g.checkNullness then
            let rec hasWithNullAnyWhere ty alreadyWrappedInOuterWithNull =
                match ty with
                | TType_var(tp, n) ->
                    let withNull =
                        alreadyWrappedInOuterWithNull
                        || n.TryEvaluate() = (ValueSome NullnessInfo.WithNull)

                    match tp.Solution with
                    | None -> []
                    | Some t -> hasWithNullAnyWhere t withNull

                | TType_app(tcr, tinst, _) ->
                    let tyArgs = tinst |> List.collect (fun t -> hasWithNullAnyWhere t false)

                    match alreadyWrappedInOuterWithNull, tcr.TypeAbbrev with
                    | true, _ when isStructTyconRef tcr -> ty :: tyArgs
                    | true, _ when tcr.IsMeasureableReprTycon ->
                        match tcr.TypeReprInfo with
                        | TMeasureableRepr realType ->
                            if hasWithNullAnyWhere realType true |> List.isEmpty then
                                []
                            else
                                [ ty ]
                        | _ -> []
                    | true, Some tAbbrev -> (hasWithNullAnyWhere tAbbrev true) @ tyArgs
                    | _ -> tyArgs

                | TType_tuple(_, tupTypes) ->
                    let inner = tupTypes |> List.collect (fun t -> hasWithNullAnyWhere t false)
                    if alreadyWrappedInOuterWithNull then ty :: inner else inner

                | TType_anon(tys = tys) ->
                    let inner = tys |> List.collect (fun t -> hasWithNullAnyWhere t false)
                    if alreadyWrappedInOuterWithNull then ty :: inner else inner
                | TType_fun(d, r, _) -> (hasWithNullAnyWhere d false) @ (hasWithNullAnyWhere r false)

                | TType_forall _ -> []
                | TType_ucase _ -> []
                | TType_measure m ->
                    if alreadyWrappedInOuterWithNull then
                        let measuresInside =
                            ListMeasureVarOccs m
                            |> List.choose (fun x -> x.Solution)
                            |> List.collect (fun x -> hasWithNullAnyWhere x true)

                        ty :: measuresInside
                    else
                        []

            hasWithNullAnyWhere ty false
        else
            []

    let TypeHasAllowNull (tcref: TyconRef) g m =
        not tcref.IsStructOrEnumTycon
        && not (isByrefLikeTyconRef g m tcref)
        && (TyconRefAllowsNull g tcref = Some true)

    /// The new logic about whether a type admits the use of 'null' as a value.
    let TypeNullIsExtraValueNew g m ty =
        let sty = stripTyparEqns ty

        (match tryTcrefOfAppTy g sty with
         | ValueSome tcref -> TypeHasAllowNull tcref g m
         | _ -> false)
        || (match (nullnessOfTy g sty).Evaluate() with
            | NullnessInfo.AmbivalentToNull -> false
            | NullnessInfo.WithoutNull -> false
            | NullnessInfo.WithNull -> true)
        || (GetTyparTyIfSupportsNull g ty).IsSome

    /// The pre-nullness logic about whether a type uses 'null' as a true representation value
    let TypeNullIsTrueValue g ty =
        (match tryTcrefOfAppTy g ty with
         | ValueSome tcref -> IsUnionTypeWithNullAsTrueValue g tcref.Deref
         | _ -> false)
        || isUnitTy g ty

    /// Indicates if unbox<T>(null) is actively rejected at runtime.   See nullability RFC.  This applies to types that don't have null
    /// as a valid runtime representation under old compatibility rules.
    let TypeNullNotLiked g m ty =
        not (TypeNullIsExtraValue g m ty)
        && not (TypeNullIsTrueValue g ty)
        && not (TypeNullNever g ty)

    let rec TypeHasDefaultValueAux isNew g m ty =
        let ty = stripTyEqnsAndMeasureEqns g ty

        (if isNew then
             TypeNullIsExtraValueNew g m ty
         else
             TypeNullIsExtraValue g m ty)
        || (isStructTy g ty
            &&
            // Is it an F# struct type?
            (if isFSharpStructTy g ty then
                 let tcref, tinst = destAppTy g ty

                 let flds =
                     // Note this includes fields implied by the use of the implicit class construction syntax
                     tcref.AllInstanceFieldsAsList
                     // We can ignore fields with the DefaultValue(false) attribute
                     |> List.filter (fun fld ->
                         not (attribsHaveValFlag g WellKnownValAttributes.DefaultValueAttribute_False fld.FieldAttribs))

                 flds
                 |> List.forall (
                     actualTyOfRecdField (mkTyconRefInst tcref tinst)
                     >> TypeHasDefaultValueAux isNew g m
                 )

             // Struct tuple types have a DefaultValue if all their element types have a default value
             elif isStructTupleTy g ty then
                 destStructTupleTy g ty |> List.forall (TypeHasDefaultValueAux isNew g m)

             // Struct anonymous record types have a DefaultValue if all their element types have a default value
             elif isStructAnonRecdTy g ty then
                 match tryDestAnonRecdTy g ty with
                 | ValueNone -> true
                 | ValueSome(_, ptys) -> ptys |> List.forall (TypeHasDefaultValueAux isNew g m)
             else
                 // All nominal struct types defined in other .NET languages have a DefaultValue regardless of their instantiation
                 true))
        ||
        // Check for type variables with the ":struct" and "(new : unit -> 'T)" constraints
        (match ty |> tryGetTyparTyWithConstraint g _.IsIsNonNullableStruct with
         | ValueSome tp -> tp |> HasConstraint _.IsRequiresDefaultConstructor
         | ValueNone -> false)

    let TypeHasDefaultValue (g: TcGlobals) m ty = TypeHasDefaultValueAux false g m ty

    let TypeHasDefaultValueNew g m ty = TypeHasDefaultValueAux true g m ty

    let (|TyparTy|NullableTypar|StructTy|NullTrueValue|NullableRefType|WithoutNullRefType|UnresolvedRefType|) (ty, g) =
        let sty = ty |> stripTyEqns g

        if isTyparTy g sty then
            if (nullnessOfTy g sty).TryEvaluate() = ValueSome NullnessInfo.WithNull then
                NullableTypar
            else
                TyparTy
        elif isStructTy g sty then
            StructTy
        elif TypeNullIsTrueValue g sty then
            NullTrueValue
        else
            match (nullnessOfTy g sty).TryEvaluate() with
            | ValueSome NullnessInfo.WithNull -> NullableRefType
            | ValueSome NullnessInfo.WithoutNull -> WithoutNullRefType
            | _ -> UnresolvedRefType

[<AutoOpen>]
module internal TypeTestsAndPatterns =

    /// Determines types that are potentially known to satisfy the 'comparable' constraint and returns
    /// a set of residual types that must also satisfy the constraint
    [<return: Struct>]
    let (|SpecialComparableHeadType|_|) g ty =
        if isAnyTupleTy g ty then
            let _tupInfo, elemTys = destAnyTupleTy g ty
            ValueSome elemTys
        elif isAnonRecdTy g ty then
            match tryDestAnonRecdTy g ty with
            | ValueNone -> ValueSome []
            | ValueSome(_anonInfo, elemTys) -> ValueSome elemTys
        else
            match tryAppTy g ty with
            | ValueSome(tcref, tinst) ->
                if
                    isArrayTyconRef g tcref
                    || tyconRefEq g tcref g.system_UIntPtr_tcref
                    || tyconRefEq g tcref g.system_IntPtr_tcref
                then
                    ValueSome tinst
                else
                    ValueNone
            | _ -> ValueNone

    [<return: Struct>]
    let (|SpecialEquatableHeadType|_|) g ty = (|SpecialComparableHeadType|_|) g ty

    [<return: Struct>]
    let (|SpecialNotEquatableHeadType|_|) g ty =
        if isFunTy g ty then ValueSome() else ValueNone

    // Can we use the fast helper for the 'LanguagePrimitives.IntrinsicFunctions.TypeTestGeneric'?
    let canUseTypeTestFast g ty =
        not (isTyparTy g ty) && not (TypeNullIsTrueValue g ty)

    // Can we use the fast helper for the 'LanguagePrimitives.IntrinsicFunctions.UnboxGeneric'?
    let canUseUnboxFast (g: TcGlobals) m ty =
        if g.checkNullness then
            match (ty, g) with
            | TyparTy
            | WithoutNullRefType
            | UnresolvedRefType -> false
            | StructTy
            | NullTrueValue
            | NullableRefType
            | NullableTypar -> true
        else
            not (isTyparTy g ty) && not (TypeNullNotLiked g m ty)

    //--------------------------------------------------------------------------
    // Nullness tests and pokes
    //--------------------------------------------------------------------------

    // Generates the logical equivalent of
    // match inp with :? ty as v -> e2[v] | _ -> e3
    //
    // No sequence point is generated for this expression form as this function is only
    // used for compiler-generated code.
    let mkIsInstConditional g m tgtTy vinputExpr v e2 e3 =

        if canUseTypeTestFast g tgtTy && isRefTy g tgtTy then

            let mbuilder = MatchBuilder(DebugPointAtBinding.NoneAtInvisible, m)
            let tg2 = mbuilder.AddResultTarget(e2)
            let tg3 = mbuilder.AddResultTarget(e3)

            let dtree =
                TDSwitch(exprForVal m v, [ TCase(DecisionTreeTest.IsNull, tg3) ], Some tg2, m)

            let expr = mbuilder.Close(dtree, m, tyOfExpr g e2)
            mkCompGenLet m v (mkIsInst tgtTy vinputExpr m) expr

        else
            let mbuilder = MatchBuilder(DebugPointAtBinding.NoneAtInvisible, m)

            let tg2 =
                TDSuccess([ mkCallUnbox g m tgtTy vinputExpr ], mbuilder.AddTarget(TTarget([ v ], e2, None)))

            let tg3 = mbuilder.AddResultTarget(e3)

            let dtree =
                TDSwitch(vinputExpr, [ TCase(DecisionTreeTest.IsInst(tyOfExpr g vinputExpr, tgtTy), tg2) ], Some tg3, m)

            let expr = mbuilder.Close(dtree, m, tyOfExpr g e2)
            expr

    let isComInteropTy g ty =
        let tcref = tcrefOfAppTy g ty
        EntityHasWellKnownAttribute g WellKnownEntityAttributes.ComImportAttribute_True tcref.Deref

    //---------------------------------------------------------------------------
    // Crack information about an F# object model call
    //---------------------------------------------------------------------------

    let GetMemberCallInfo g (vref: ValRef, vFlags) =
        match vref.MemberInfo with
        | Some membInfo when not vref.IsExtensionMember ->
            let numEnclTypeArgs = vref.MemberApparentEntity.TyparsNoRange.Length

            let virtualCall =
                (membInfo.MemberFlags.IsOverrideOrExplicitImpl
                 || membInfo.MemberFlags.IsDispatchSlot)
                && not membInfo.MemberFlags.IsFinal
                && (match vFlags with
                    | VSlotDirectCall -> false
                    | _ -> true)

            let isNewObj =
                (membInfo.MemberFlags.MemberKind = SynMemberKind.Constructor)
                && (match vFlags with
                    | NormalValUse -> true
                    | _ -> false)

            let isSuperInit =
                (membInfo.MemberFlags.MemberKind = SynMemberKind.Constructor)
                && (match vFlags with
                    | CtorValUsedAsSuperInit -> true
                    | _ -> false)

            let isSelfInit =
                (membInfo.MemberFlags.MemberKind = SynMemberKind.Constructor)
                && (match vFlags with
                    | CtorValUsedAsSelfInit -> true
                    | _ -> false)

            let isCompiledAsInstance = ValRefIsCompiledAsInstanceMember g vref
            let takesInstanceArg = isCompiledAsInstance && not isNewObj

            let isPropGet =
                (membInfo.MemberFlags.MemberKind = SynMemberKind.PropertyGet)
                && (membInfo.MemberFlags.IsInstance = isCompiledAsInstance)

            let isPropSet =
                (membInfo.MemberFlags.MemberKind = SynMemberKind.PropertySet)
                && (membInfo.MemberFlags.IsInstance = isCompiledAsInstance)

            numEnclTypeArgs, virtualCall, isNewObj, isSuperInit, isSelfInit, takesInstanceArg, isPropGet, isPropSet
        | _ -> 0, false, false, false, false, false, false, false

[<AutoOpen>]
module internal Rewriting =

    //---------------------------------------------------------------------------
    // RewriteExpr: rewrite bottom up with interceptors
    //---------------------------------------------------------------------------

    [<NoEquality; NoComparison>]
    type ExprRewritingEnv =
        {
            PreIntercept: ((Expr -> Expr) -> Expr -> Expr option) option
            PostTransform: Expr -> Expr option
            PreInterceptBinding: ((Expr -> Expr) -> Binding -> Binding option) option
            RewriteQuotations: bool
            StackGuard: StackGuard
        }

    let rec rewriteBind env bind =
        match env.PreInterceptBinding with
        | Some f ->
            match f (RewriteExpr env) bind with
            | Some res -> res
            | None -> rewriteBindStructure env bind
        | None -> rewriteBindStructure env bind

    and rewriteBindStructure env (TBind(v, e, letSeqPtOpt)) =
        TBind(v, RewriteExpr env e, letSeqPtOpt)

    and rewriteBinds env binds = List.map (rewriteBind env) binds

    and RewriteExpr env expr =
        env.StackGuard.Guard
        <| fun () ->
            match expr with
            | LinearOpExpr _
            | LinearMatchExpr _
            | Expr.Let _
            | Expr.Sequential _
            | Expr.DebugPoint _ -> rewriteLinearExpr env expr id
            | _ ->
                let expr =
                    match preRewriteExpr env expr with
                    | Some expr -> expr
                    | None -> rewriteExprStructure env expr

                postRewriteExpr env expr

    and preRewriteExpr env expr =
        match env.PreIntercept with
        | Some f -> f (RewriteExpr env) expr
        | None -> None

    and postRewriteExpr env expr =
        match env.PostTransform expr with
        | None -> expr
        | Some expr2 -> expr2

    and rewriteExprStructure env expr =
        match expr with
        | Expr.Const _
        | Expr.Val _ -> expr

        | Expr.App(f0, f0ty, tyargs, args, m) ->
            let f0R = RewriteExpr env f0
            let argsR = rewriteExprs env args

            if f0 === f0R && args === argsR then
                expr
            else
                Expr.App(f0R, f0ty, tyargs, argsR, m)

        | Expr.Quote(ast, dataCell, isFromQueryExpression, m, ty) ->
            let data =
                match dataCell.Value with
                | None -> None
                | Some(data1, data2) -> Some(map3Of4 (rewriteExprs env) data1, map3Of4 (rewriteExprs env) data2)

            Expr.Quote((if env.RewriteQuotations then RewriteExpr env ast else ast), ref data, isFromQueryExpression, m, ty)

        | Expr.Obj(_, ty, basev, basecall, overrides, iimpls, m) ->
            let overridesR = List.map (rewriteObjExprOverride env) overrides
            let basecallR = RewriteExpr env basecall
            let iimplsR = List.map (rewriteObjExprInterfaceImpl env) iimpls
            mkObjExpr (ty, basev, basecallR, overridesR, iimplsR, m)

        | Expr.Link eref -> RewriteExpr env eref.Value

        | Expr.DebugPoint _ -> failwith "unreachable - linear debug point"

        | Expr.Op(c, tyargs, args, m) ->
            let argsR = rewriteExprs env args

            if args === argsR then
                expr
            else
                Expr.Op(c, tyargs, argsR, m)

        | Expr.Lambda(_lambdaId, ctorThisValOpt, baseValOpt, argvs, body, m, bodyTy) ->
            let bodyR = RewriteExpr env body
            rebuildLambda m ctorThisValOpt baseValOpt argvs (bodyR, bodyTy)

        | Expr.TyLambda(_lambdaId, tps, body, m, bodyTy) ->
            let bodyR = RewriteExpr env body
            mkTypeLambda m tps (bodyR, bodyTy)

        | Expr.Match(spBind, mExpr, dtree, targets, m, ty) ->
            let dtreeR = RewriteDecisionTree env dtree
            let targetsR = rewriteTargets env targets
            mkAndSimplifyMatch spBind mExpr m ty dtreeR targetsR

        | Expr.LetRec(binds, e, m, _) ->
            let bindsR = rewriteBinds env binds
            let eR = RewriteExpr env e
            Expr.LetRec(bindsR, eR, m, Construct.NewFreeVarsCache())

        | Expr.Let _ -> failwith "unreachable - linear let"

        | Expr.Sequential _ -> failwith "unreachable - linear seq"

        | Expr.StaticOptimization(constraints, e2, e3, m) ->
            let e2R = RewriteExpr env e2
            let e3R = RewriteExpr env e3
            Expr.StaticOptimization(constraints, e2R, e3R, m)

        | Expr.TyChoose(a, b, m) -> Expr.TyChoose(a, RewriteExpr env b, m)

        | Expr.WitnessArg(witnessInfo, m) -> Expr.WitnessArg(witnessInfo, m)

    and rewriteLinearExpr env expr contf =
        // schedule a rewrite on the way back up by adding to the continuation
        let contf = contf << postRewriteExpr env

        match preRewriteExpr env expr with
        | Some expr -> contf expr
        | None ->
            match expr with
            | Expr.Let(bind, bodyExpr, m, _) ->
                let bind = rewriteBind env bind
                // tailcall
                rewriteLinearExpr env bodyExpr (contf << (fun bodyExprR -> mkLetBind m bind bodyExprR))

            | Expr.Sequential(expr1, expr2, dir, m) ->
                let expr1R = RewriteExpr env expr1
                // tailcall
                rewriteLinearExpr
                    env
                    expr2
                    (contf
                     << (fun expr2R ->
                         if expr1 === expr1R && expr2 === expr2R then
                             expr
                         else
                             Expr.Sequential(expr1R, expr2R, dir, m)))

            | LinearOpExpr(op, tyargs, argsFront, argLast, m) ->
                let argsFrontR = rewriteExprs env argsFront
                // tailcall
                rewriteLinearExpr
                    env
                    argLast
                    (contf
                     << (fun argLastR ->
                         if argsFront === argsFrontR && argLast === argLastR then
                             expr
                         else
                             rebuildLinearOpExpr (op, tyargs, argsFrontR, argLastR, m)))

            | LinearMatchExpr(spBind, mExpr, dtree, tg1, expr2, m2, ty) ->
                let dtree = RewriteDecisionTree env dtree
                let tg1R = rewriteTarget env tg1
                // tailcall
                rewriteLinearExpr
                    env
                    expr2
                    (contf
                     << (fun expr2R -> rebuildLinearMatchExpr (spBind, mExpr, dtree, tg1R, expr2R, m2, ty)))

            | Expr.DebugPoint(dpm, innerExpr) ->
                rewriteLinearExpr env innerExpr (contf << (fun innerExprR -> Expr.DebugPoint(dpm, innerExprR)))

            | _ ->
                // no longer linear, no tailcall
                contf (RewriteExpr env expr)

    and rewriteExprs env exprs = List.mapq (RewriteExpr env) exprs

    and rewriteFlatExprs env exprs = List.mapq (RewriteExpr env) exprs

    and RewriteDecisionTree env x =
        match x with
        | TDSuccess(es, n) ->
            let esR = rewriteFlatExprs env es

            if LanguagePrimitives.PhysicalEquality es esR then
                x
            else
                TDSuccess(esR, n)

        | TDSwitch(e, cases, dflt, m) ->
            let eR = RewriteExpr env e

            let casesR =
                List.map (fun (TCase(discrim, e)) -> TCase(discrim, RewriteDecisionTree env e)) cases

            let dfltR = Option.map (RewriteDecisionTree env) dflt
            TDSwitch(eR, casesR, dfltR, m)

        | TDBind(bind, body) ->
            let bindR = rewriteBind env bind
            let bodyR = RewriteDecisionTree env body
            TDBind(bindR, bodyR)

    and rewriteTarget env (TTarget(vs, e, flags)) =
        let eR = RewriteExpr env e
        TTarget(vs, eR, flags)

    and rewriteTargets env targets =
        List.map (rewriteTarget env) (Array.toList targets)

    and rewriteObjExprOverride env (TObjExprMethod(slotsig, attribs, tps, vs, e, m)) =
        TObjExprMethod(slotsig, attribs, tps, vs, RewriteExpr env e, m)

    and rewriteObjExprInterfaceImpl env (ty, overrides) =
        (ty, List.map (rewriteObjExprOverride env) overrides)

    and rewriteModuleOrNamespaceContents env x =
        match x with
        | TMDefRec(isRec, opens, tycons, mbinds, m) -> TMDefRec(isRec, opens, tycons, rewriteModuleOrNamespaceBindings env mbinds, m)
        | TMDefLet(bind, m) -> TMDefLet(rewriteBind env bind, m)
        | TMDefDo(e, m) -> TMDefDo(RewriteExpr env e, m)
        | TMDefOpens _ -> x
        | TMDefs defs -> TMDefs(List.map (rewriteModuleOrNamespaceContents env) defs)

    and rewriteModuleOrNamespaceBinding env x =
        match x with
        | ModuleOrNamespaceBinding.Binding bind -> ModuleOrNamespaceBinding.Binding(rewriteBind env bind)
        | ModuleOrNamespaceBinding.Module(nm, rhs) -> ModuleOrNamespaceBinding.Module(nm, rewriteModuleOrNamespaceContents env rhs)

    and rewriteModuleOrNamespaceBindings env mbinds =
        List.map (rewriteModuleOrNamespaceBinding env) mbinds

    and RewriteImplFile env implFile =
        let (CheckedImplFile(fragName, signature, contents, hasExplicitEntryPoint, isScript, anonRecdTypes, namedDebugPointsForInlinedCode)) =
            implFile

        let contentsR = rewriteModuleOrNamespaceContents env contents

        let implFileR =
            CheckedImplFile(fragName, signature, contentsR, hasExplicitEntryPoint, isScript, anonRecdTypes, namedDebugPointsForInlinedCode)

        implFileR

    //--------------------------------------------------------------------------
    // Apply a "local to nonlocal" renaming to a module type. This can't use
    // remap_mspec since the remapping we want isn't to newly created nodes
    // but rather to remap to the nonlocal references. This is deliberately
    // "breaking" the binding structure implicit in the module type, which is
    // the whole point - one things are rewritten to use non local references then
    // the elements can be copied at will, e.g. when inlining during optimization.
    //------------------------------------------------------------------------

    let rec remapEntityDataToNonLocal ctxt tmenv (d: Entity) =
        let tpsR, tmenvinner =
            tmenvCopyRemapAndBindTypars (remapAttribs ctxt tmenv) tmenv (d.entity_typars.Force(d.entity_range))

        let typarsR = LazyWithContext.NotLazy tpsR
        let attribsR = d.entity_attribs.AsList() |> remapAttribs ctxt tmenvinner
        let tyconReprR = d.entity_tycon_repr |> remapTyconRepr ctxt tmenvinner
        let tyconAbbrevR = d.TypeAbbrev |> Option.map (remapType tmenvinner)
        let tyconTcaugR = d.entity_tycon_tcaug |> remapTyconAug tmenvinner

        let modulContentsR =
            MaybeLazy.Strict(
                d.entity_modul_type.Value
                |> mapImmediateValsAndTycons (remapTyconToNonLocal ctxt tmenv) (remapValToNonLocal ctxt tmenv)
            )

        let exnInfoR = d.ExceptionInfo |> remapTyconExnInfo ctxt tmenvinner

        { d with
            entity_typars = typarsR
            entity_attribs = WellKnownEntityAttribs.Create(attribsR)
            entity_tycon_repr = tyconReprR
            entity_tycon_tcaug = tyconTcaugR
            entity_modul_type = modulContentsR
            entity_opt_data =
                match d.entity_opt_data with
                | Some dd ->
                    Some
                        { dd with
                            entity_tycon_abbrev = tyconAbbrevR
                            entity_exn_info = exnInfoR
                        }
                | _ -> None
        }

    and remapTyconToNonLocal ctxt tmenv x =
        x |> Construct.NewModifiedTycon(remapEntityDataToNonLocal ctxt tmenv)

    and remapValToNonLocal ctxt tmenv inp =
        // creates a new stamp
        inp |> Construct.NewModifiedVal(remapValData ctxt tmenv)

    let ApplyExportRemappingToEntity g tmenv x =
        let ctxt = mkRemapContext g (StackGuard("RemapExprStackGuardDepth"))
        remapTyconToNonLocal ctxt tmenv x

    (* Which constraints actually get compiled to .NET constraints? *)
    let isCompiledOrWitnessPassingConstraint (g: TcGlobals) cx =
        match cx with
        | TyparConstraint.SupportsNull _ // this implies the 'class' constraint
        | TyparConstraint.IsReferenceType _ // this is the 'class' constraint
        | TyparConstraint.IsNonNullableStruct _
        | TyparConstraint.IsReferenceType _
        | TyparConstraint.RequiresDefaultConstructor _
        | TyparConstraint.IsUnmanaged _ //  implies "struct" and also causes a modreq
        | TyparConstraint.CoercesTo _ -> true
        | TyparConstraint.MayResolveMember _ when g.langVersion.SupportsFeature LanguageFeature.WitnessPassing -> true
        | _ -> false

    // Is a value a first-class polymorphic value with .NET constraints, or witness-passing constraints?
    // Used to turn off TLR and method splitting and do not compile to
    // FSharpTypeFunc, but rather bake a "local type function" for each TyLambda abstraction.
    let IsGenericValWithGenericConstraints g (v: Val) =
        isForallTy g v.Type
        && v.Type
           |> destForallTy g
           |> fst
           |> List.exists (fun tp -> HasConstraint (isCompiledOrWitnessPassingConstraint g) tp)

    // Does a type support a given interface?
    type Entity with
        member tycon.HasInterface g ty =
            tycon.TypeContents.tcaug_interfaces
            |> List.exists (fun (x, _, _) -> typeEquiv g ty x)

        // Does a type have an override matching the given name and argument types?
        // Used to detect the presence of 'Equals' and 'GetHashCode' in type checking
        member tycon.HasOverride g nm argTys =
            tycon.TypeContents.tcaug_adhoc
            |> NameMultiMap.find nm
            |> List.exists (fun vref ->
                match vref.MemberInfo with
                | None -> false
                | Some membInfo ->

                    let argInfos = ArgInfosOfMember g vref

                    match argInfos with
                    | [ argInfos ] ->
                        List.lengthsEqAndForall2 (typeEquiv g) (List.map fst argInfos) argTys
                        && membInfo.MemberFlags.IsOverrideOrExplicitImpl
                    | _ -> false)

        member tycon.TryGetMember g nm argTys =
            tycon.TypeContents.tcaug_adhoc
            |> NameMultiMap.find nm
            |> List.tryFind (fun vref ->
                match vref.MemberInfo with
                | None -> false
                | _ ->

                    let argInfos = ArgInfosOfMember g vref

                    match argInfos with
                    | [ argInfos ] -> List.lengthsEqAndForall2 (typeEquiv g) (List.map fst argInfos) argTys
                    | _ -> false)

        member tycon.HasMember g nm argTys = (tycon.TryGetMember g nm argTys).IsSome

    type EntityRef with
        member tcref.HasInterface g ty = tcref.Deref.HasInterface g ty
        member tcref.HasOverride g nm argTys = tcref.Deref.HasOverride g nm argTys
        member tcref.HasMember g nm argTys = tcref.Deref.HasMember g nm argTys

[<AutoOpen>]
module internal TupleCompilation =

    let mkFastForLoop g (spFor, spTo, m, idv: Val, start, dir, finish, body) =
        let dir = if dir then FSharpForLoopUp else FSharpForLoopDown
        mkIntegerForLoop g (spFor, spTo, idv, start, dir, finish, body, m)

    // Take into account the fact that some "instance" members are compiled as static
    // members when using CompilationRepresentation.Static, or any non-virtual instance members
    // in a type that supports "null" as a true value. This is all members
    // where ValRefIsCompiledAsInstanceMember is false but membInfo.MemberFlags.IsInstance
    // is true.
    //
    // This is the right abstraction for viewing member types, but the implementation
    // below is a little ugly.
    let GetTypeOfIntrinsicMemberInCompiledForm g (vref: ValRef) =
        assert (not vref.IsExtensionMember)
        let membInfo, valReprInfo = checkMemberValRef vref
        let tps, cxs, argInfos, retTy, retInfo = GetTypeOfMemberInMemberForm g vref

        let argInfos =
            // Check if the thing is really an instance member compiled as a static member
            // If so, the object argument counts as a normal argument in the compiled form
            if membInfo.MemberFlags.IsInstance && not (ValRefIsCompiledAsInstanceMember g vref) then
                let _, origArgInfos, _, _ =
                    GetValReprTypeInFSharpForm g valReprInfo vref.Type vref.Range

                match origArgInfos with
                | [] ->
                    errorR (InternalError("value does not have a valid member type", vref.Range))
                    argInfos
                | h :: _ -> h :: argInfos
            else
                argInfos

        tps, cxs, argInfos, retTy, retInfo

    //--------------------------------------------------------------------------
    // Tuple compilation (expressions)
    //------------------------------------------------------------------------

    let rec mkCompiledTuple g isStruct (argTys, args, m) =
        let n = List.length argTys

        if n <= 0 then
            failwith "mkCompiledTuple"
        elif n < maxTuple then
            (mkCompiledTupleTyconRef g isStruct n, argTys, args, m)
        else
            let argTysA, argTysB = List.splitAfter goodTupleFields argTys
            let argsA, argsB = List.splitAfter goodTupleFields args

            let ty8, v8 =
                match argTysB, argsB with
                | [ ty8 ], [ arg8 ] ->
                    match ty8 with
                    // if it's already been nested or ended, pass it through
                    | TType_app(tn, _, _) when (isCompiledTupleTyconRef g tn) -> ty8, arg8
                    | _ ->
                        let ty8enc =
                            TType_app((if isStruct then g.struct_tuple1_tcr else g.ref_tuple1_tcr), [ ty8 ], g.knownWithoutNull)

                        let v8enc = Expr.Op(TOp.Tuple(mkTupInfo isStruct), [ ty8 ], [ arg8 ], m)
                        ty8enc, v8enc
                | _ ->
                    let a, b, c, d = mkCompiledTuple g isStruct (argTysB, argsB, m)
                    let ty8plus = TType_app(a, b, g.knownWithoutNull)
                    let v8plus = Expr.Op(TOp.Tuple(mkTupInfo isStruct), b, c, d)
                    ty8plus, v8plus

            let argTysAB = argTysA @ [ ty8 ]
            (mkCompiledTupleTyconRef g isStruct (List.length argTysAB), argTysAB, argsA @ [ v8 ], m)

    let mkILMethodSpecForTupleItem (_g: TcGlobals) (ty: ILType) n =
        mkILNonGenericInstanceMethSpecInTy (
            ty,
            (if n < goodTupleFields then
                 "get_Item" + (n + 1).ToString()
             else
                 "get_Rest"),
            [],
            mkILTyvarTy (uint16 n)
        )

    let mkILFieldSpecForTupleItem (ty: ILType) n =
        mkILFieldSpecInTy (
            ty,
            (if n < goodTupleFields then
                 "Item" + (n + 1).ToString()
             else
                 "Rest"),
            mkILTyvarTy (uint16 n)
        )

    let mkGetTupleItemN g m n (ty: ILType) isStruct expr retTy =
        if isStruct then
            mkAsmExpr ([ mkNormalLdfld (mkILFieldSpecForTupleItem ty n) ], [], [ expr ], [ retTy ], m)
        else
            mkAsmExpr ([ mkNormalCall (mkILMethodSpecForTupleItem g ty n) ], [], [ expr ], [ retTy ], m)

    /// Match a try-finally expression
    [<return: Struct>]
    let (|TryFinally|_|) expr =
        match expr with
        | Expr.Op(TOp.TryFinally _, [ _resTy ], [ Expr.Lambda(_, _, _, [ _ ], e1, _, _); Expr.Lambda(_, _, _, [ _ ], e2, _, _) ], _) ->
            ValueSome(e1, e2)
        | _ -> ValueNone

    // detect ONLY the while loops that result from compiling 'for ... in ... do ...'
    [<return: Struct>]
    let (|WhileLoopForCompiledForEachExpr|_|) expr =
        match expr with
        | Expr.Op(TOp.While(spInWhile, WhileLoopForCompiledForEachExprMarker),
                  _,
                  [ Expr.Lambda(_, _, _, [ _ ], e1, _, _); Expr.Lambda(_, _, _, [ _ ], e2, _, _) ],
                  m) -> ValueSome(spInWhile, e1, e2, m)
        | _ -> ValueNone

    [<return: Struct>]
    let (|Let|_|) expr =
        match expr with
        | Expr.Let(TBind(v, e1, sp), e2, _, _) -> ValueSome(v, e1, sp, e2)
        | _ -> ValueNone

    [<return: Struct>]
    let (|RangeInt32Step|_|) g expr =
        match expr with
        // detect 'n .. m'
        | Expr.App(Expr.Val(vf, _, _), _, [ tyarg ], [ startExpr; finishExpr ], _) when
            valRefEq g vf g.range_op_vref && typeEquiv g tyarg g.int_ty
            ->
            ValueSome(startExpr, 1, finishExpr)

        // detect (RangeInt32 startExpr N finishExpr), the inlined/compiled form of 'n .. m' and 'n .. N .. m'
        | Expr.App(Expr.Val(vf, _, _), _, [], [ startExpr; Expr.Const(Const.Int32 n, _, _); finishExpr ], _) when
            valRefEq g vf g.range_int32_op_vref
            ->
            ValueSome(startExpr, n, finishExpr)

        | _ -> ValueNone

    [<return: Struct>]
    let (|GetEnumeratorCall|_|) expr =
        match expr with
        | Expr.Op(TOp.ILCall(_, _, _, _, _, _, _, ilMethodRef, _, _, _),
                  _,
                  [ Expr.Val(vref, _, _) | Expr.Op(_, _, [ Expr.Val(vref, ValUseFlag.NormalValUse, _) ], _) ],
                  _) ->
            if ilMethodRef.Name = "GetEnumerator" then
                ValueSome vref
            else
                ValueNone
        | _ -> ValueNone

    // This code matches exactly the output of TcForEachExpr
    [<return: Struct>]
    let (|CompiledForEachExpr|_|) g expr =
        match expr with
        | Let(enumerableVar,
              enumerableExpr,
              spFor,
              Let(enumeratorVar,
                  GetEnumeratorCall enumerableVar2,
                  _enumeratorBind,
                  TryFinally(WhileLoopForCompiledForEachExpr(spInWhile, _, (Let(elemVar, _, _, bodyExpr) as elemLet), _), _))) when
            // Apply correctness conditions to ensure this really is a compiled for-each expression.
            valRefEq g (mkLocalValRef enumerableVar) enumerableVar2
            && enumerableVar.IsCompilerGenerated
            && enumeratorVar.IsCompilerGenerated
            && (let fvs = (freeInExpr CollectLocals bodyExpr)

                not (Zset.contains enumerableVar fvs.FreeLocals)
                && not (Zset.contains enumeratorVar fvs.FreeLocals))
            ->

            // Extract useful ranges
            let mBody = bodyExpr.Range
            let mWholeExpr = expr.Range
            let mIn = elemLet.Range

            let mFor =
                match spFor with
                | DebugPointAtBinding.Yes mFor -> mFor
                | _ -> enumerableExpr.Range

            let spIn, mIn =
                match spInWhile with
                | DebugPointAtWhile.Yes mIn -> DebugPointAtInOrTo.Yes mIn, mIn
                | _ -> DebugPointAtInOrTo.No, mIn

            let spInWhile =
                match spIn with
                | DebugPointAtInOrTo.Yes m -> DebugPointAtWhile.Yes m
                | DebugPointAtInOrTo.No -> DebugPointAtWhile.No

            let enumerableTy = tyOfExpr g enumerableExpr

            ValueSome(enumerableTy, enumerableExpr, elemVar, bodyExpr, (mBody, spFor, spIn, mFor, mIn, spInWhile, mWholeExpr))
        | _ -> ValueNone

    [<return: Struct>]
    let (|CompiledInt32RangeForEachExpr|_|) g expr =
        match expr with
        | CompiledForEachExpr g (_, RangeInt32Step g (startExpr, step, finishExpr), elemVar, bodyExpr, ranges) ->
            ValueSome(startExpr, step, finishExpr, elemVar, bodyExpr, ranges)
        | _ -> ValueNone

    [<return: Struct>]
    let (|ValApp|_|) g vref expr =
        match expr with
        | Expr.App(Expr.Val(vref2, _, _), _f0ty, tyargs, args, m) when valRefEq g vref vref2 -> ValueSome(tyargs, args, m)
        | _ -> ValueNone

    [<RequireQualifiedAccess>]
    module IntegralConst =
        /// Constant 0.
        [<return: Struct>]
        let (|Zero|_|) c =
            match c with
            | Const.Zero
            | Const.Int32 0
            | Const.Int64 0L
            | Const.UInt64 0UL
            | Const.UInt32 0u
            | Const.IntPtr 0L
            | Const.UIntPtr 0UL
            | Const.Int16 0s
            | Const.UInt16 0us
            | Const.SByte 0y
            | Const.Byte 0uy
            | Const.Char '\000' -> ValueSome Zero
            | _ -> ValueNone

        /// Constant 1.
        [<return: Struct>]
        let (|One|_|) expr =
            match expr with
            | Const.Int32 1
            | Const.Int64 1L
            | Const.UInt64 1UL
            | Const.UInt32 1u
            | Const.IntPtr 1L
            | Const.UIntPtr 1UL
            | Const.Int16 1s
            | Const.UInt16 1us
            | Const.SByte 1y
            | Const.Byte 1uy
            | Const.Char '\001' -> ValueSome One
            | _ -> ValueNone

        /// Constant -1.
        [<return: Struct>]
        let (|MinusOne|_|) c =
            match c with
            | Const.Int32 -1
            | Const.Int64 -1L
            | Const.IntPtr -1L
            | Const.Int16 -1s
            | Const.SByte -1y -> ValueSome MinusOne
            | _ -> ValueNone

        /// Positive constant.
        [<return: Struct>]
        let (|Positive|_|) c =
            match c with
            | Const.Int32 v when v > 0 -> ValueSome Positive
            | Const.Int64 v when v > 0L -> ValueSome Positive
            // sizeof<nativeint> is not constant, so |𝑐| ≥ 0x80000000n cannot be treated as a constant.
            | Const.IntPtr v when v > 0L && uint64 v < 0x80000000UL -> ValueSome Positive
            | Const.Int16 v when v > 0s -> ValueSome Positive
            | Const.SByte v when v > 0y -> ValueSome Positive
            | Const.UInt64 v when v > 0UL -> ValueSome Positive
            | Const.UInt32 v when v > 0u -> ValueSome Positive
            // sizeof<unativeint> is not constant, so |𝑐| > 0xffffffffun cannot be treated as a constant.
            | Const.UIntPtr v when v > 0UL && v <= 0xffffffffUL -> ValueSome Positive
            | Const.UInt16 v when v > 0us -> ValueSome Positive
            | Const.Byte v when v > 0uy -> ValueSome Positive
            | Const.Char v when v > '\000' -> ValueSome Positive
            | _ -> ValueNone

        /// Negative constant.
        [<return: Struct>]
        let (|Negative|_|) c =
            match c with
            | Const.Int32 v when v < 0 -> ValueSome Negative
            | Const.Int64 v when v < 0L -> ValueSome Negative
            // sizeof<nativeint> is not constant, so |𝑐| ≥ 0x80000000n cannot be treated as a constant.
            | Const.IntPtr v when v < 0L && uint64 v < 0x80000000UL -> ValueSome Negative
            | Const.Int16 v when v < 0s -> ValueSome Negative
            | Const.SByte v when v < 0y -> ValueSome Negative
            | _ -> ValueNone

        /// Returns the absolute value of the given integral constant.
        let abs c =
            match c with
            | Const.Int32 Int32.MinValue -> Const.UInt32(uint Int32.MaxValue + 1u)
            | Const.Int64 Int64.MinValue -> Const.UInt64(uint64 Int64.MaxValue + 1UL)
            | Const.IntPtr Int64.MinValue -> Const.UIntPtr(uint64 Int64.MaxValue + 1UL)
            | Const.Int16 Int16.MinValue -> Const.UInt16(uint16 Int16.MaxValue + 1us)
            | Const.SByte SByte.MinValue -> Const.Byte(byte SByte.MaxValue + 1uy)
            | Const.Int32 v -> Const.Int32(abs v)
            | Const.Int64 v -> Const.Int64(abs v)
            | Const.IntPtr v -> Const.IntPtr(abs v)
            | Const.Int16 v -> Const.Int16(abs v)
            | Const.SByte v -> Const.SByte(abs v)
            | _ -> c

    let tryMatchIntegralRange g expr =
        match expr with
        | ValApp g g.range_int32_op_vref ([], [ start; step; finish ], _) -> ValueSome(g.int32_ty, (start, step, finish))
        | ValApp g g.range_int64_op_vref ([], [ start; step; finish ], _) -> ValueSome(g.int64_ty, (start, step, finish))
        | ValApp g g.range_uint64_op_vref ([], [ start; step; finish ], _) -> ValueSome(g.uint64_ty, (start, step, finish))
        | ValApp g g.range_uint32_op_vref ([], [ start; step; finish ], _) -> ValueSome(g.uint32_ty, (start, step, finish))
        | ValApp g g.range_nativeint_op_vref ([], [ start; step; finish ], _) -> ValueSome(g.nativeint_ty, (start, step, finish))
        | ValApp g g.range_unativeint_op_vref ([], [ start; step; finish ], _) -> ValueSome(g.unativeint_ty, (start, step, finish))
        | ValApp g g.range_int16_op_vref ([], [ start; step; finish ], _) -> ValueSome(g.int16_ty, (start, step, finish))
        | ValApp g g.range_uint16_op_vref ([], [ start; step; finish ], _) -> ValueSome(g.uint16_ty, (start, step, finish))
        | ValApp g g.range_sbyte_op_vref ([], [ start; step; finish ], _) -> ValueSome(g.sbyte_ty, (start, step, finish))
        | ValApp g g.range_byte_op_vref ([], [ start; step; finish ], _) -> ValueSome(g.byte_ty, (start, step, finish))
        | ValApp g g.range_char_op_vref ([], [ start; finish ], _) ->
            ValueSome(g.char_ty, (start, Expr.Const(Const.Char '\001', range0, g.char_ty), finish))
        | ValApp g g.range_op_vref (ty :: _, [ start; finish ], _) when isIntegerTy g ty || typeEquivAux EraseMeasures g ty g.char_ty ->
            ValueSome(ty, (start, mkTypedOne g range0 ty, finish))
        | ValApp g g.range_step_op_vref ([ ty; ty2 ], [ start; step; finish ], _) when
            typeEquiv g ty ty2
            && (isIntegerTy g ty || typeEquivAux EraseMeasures g ty g.char_ty)
            ->
            ValueSome(ty, (start, step, finish))
        | ValApp g g.range_generic_op_vref ([ ty; ty2 ], [ _one; _add; start; finish ], _) when
            typeEquiv g ty ty2
            && (isIntegerTy g ty || typeEquivAux EraseMeasures g ty g.char_ty)
            ->
            ValueSome(ty, (start, mkTypedOne g range0 ty, finish))
        | ValApp g g.range_step_generic_op_vref ([ ty; ty2 ], [ _zero; _add; start; step; finish ], _) when
            typeEquiv g ty ty2
            && (isIntegerTy g ty || typeEquivAux EraseMeasures g ty g.char_ty)
            ->
            ValueSome(ty, (start, step, finish))
        | _ -> ValueNone

    /// 5..1
    /// 1..-5
    /// 1..-1..5
    /// -5..-1..-1
    /// 5..2..1
    [<return: Struct>]
    let (|EmptyRange|_|) (start, step, finish) =
        match start, step, finish with
        | Expr.Const(value = Const.Int32 start), Expr.Const(value = Const.Int32 step), Expr.Const(value = Const.Int32 finish) when
            finish < start && step > 0 || finish > start && step < 0
            ->
            ValueSome EmptyRange
        | Expr.Const(value = Const.Int64 start), Expr.Const(value = Const.Int64 step), Expr.Const(value = Const.Int64 finish) when
            finish < start && step > 0L || finish > start && step < 0L
            ->
            ValueSome EmptyRange
        | Expr.Const(value = Const.UInt64 start), Expr.Const(value = Const.UInt64 _), Expr.Const(value = Const.UInt64 finish) when
            finish < start
            ->
            ValueSome EmptyRange
        | Expr.Const(value = Const.UInt32 start), Expr.Const(value = Const.UInt32 _), Expr.Const(value = Const.UInt32 finish) when
            finish < start
            ->
            ValueSome EmptyRange

        // sizeof<nativeint> is not constant, so |𝑐| ≥ 0x80000000n cannot be treated as a constant.
        | Expr.Const(value = Const.IntPtr start), Expr.Const(value = Const.IntPtr step), Expr.Const(value = Const.IntPtr finish) when
            uint64 start < 0x80000000UL
            && uint64 step < 0x80000000UL
            && uint64 finish < 0x80000000UL
            && (finish < start && step > 0L || finish > start && step < 0L)
            ->
            ValueSome EmptyRange

        // sizeof<unativeint> is not constant, so |𝑐| > 0xffffffffun cannot be treated as a constant.
        | Expr.Const(value = Const.UIntPtr start), Expr.Const(value = Const.UIntPtr step), Expr.Const(value = Const.UIntPtr finish) when
            start <= 0xffffffffUL
            && step <= 0xffffffffUL
            && finish <= 0xffffffffUL
            && finish <= start
            ->
            ValueSome EmptyRange

        | Expr.Const(value = Const.Int16 start), Expr.Const(value = Const.Int16 step), Expr.Const(value = Const.Int16 finish) when
            finish < start && step > 0s || finish > start && step < 0s
            ->
            ValueSome EmptyRange
        | Expr.Const(value = Const.UInt16 start), Expr.Const(value = Const.UInt16 _), Expr.Const(value = Const.UInt16 finish) when
            finish < start
            ->
            ValueSome EmptyRange
        | Expr.Const(value = Const.SByte start), Expr.Const(value = Const.SByte step), Expr.Const(value = Const.SByte finish) when
            finish < start && step > 0y || finish > start && step < 0y
            ->
            ValueSome EmptyRange
        | Expr.Const(value = Const.Byte start), Expr.Const(value = Const.Byte _), Expr.Const(value = Const.Byte finish) when finish < start ->
            ValueSome EmptyRange
        | Expr.Const(value = Const.Char start), Expr.Const(value = Const.Char _), Expr.Const(value = Const.Char finish) when finish < start ->
            ValueSome EmptyRange
        | _ -> ValueNone

    /// Note: this assumes that an empty range has already been checked for
    /// (otherwise the conversion operations here might overflow).
    [<return: Struct>]
    let (|ConstCount|_|) (start, step, finish) =
        match start, step, finish with
        // The count for these ranges is 2⁶⁴ + 1. We must handle such ranges at runtime.
        | Expr.Const(value = Const.Int64 Int64.MinValue), Expr.Const(value = Const.Int64 1L), Expr.Const(value = Const.Int64 Int64.MaxValue)
        | Expr.Const(value = Const.Int64 Int64.MaxValue),
          Expr.Const(value = Const.Int64 -1L),
          Expr.Const(value = Const.Int64 Int64.MinValue)
        | Expr.Const(value = Const.UInt64 UInt64.MinValue),
          Expr.Const(value = Const.UInt64 1UL),
          Expr.Const(value = Const.UInt64 UInt64.MaxValue)
        | Expr.Const(value = Const.IntPtr Int64.MinValue),
          Expr.Const(value = Const.IntPtr 1L),
          Expr.Const(value = Const.IntPtr Int64.MaxValue)
        | Expr.Const(value = Const.IntPtr Int64.MaxValue),
          Expr.Const(value = Const.IntPtr -1L),
          Expr.Const(value = Const.IntPtr Int64.MinValue)
        | Expr.Const(value = Const.UIntPtr UInt64.MinValue),
          Expr.Const(value = Const.UIntPtr 1UL),
          Expr.Const(value = Const.UIntPtr UInt64.MaxValue) -> ValueNone

        // We must special-case a step of Int64.MinValue, since we cannot call abs on it.
        | Expr.Const(value = Const.Int64 start), Expr.Const(value = Const.Int64 Int64.MinValue), Expr.Const(value = Const.Int64 finish) when
            start <= finish
            ->
            ValueSome(Const.UInt64((uint64 finish - uint64 start) / (uint64 Int64.MaxValue + 1UL) + 1UL))
        | Expr.Const(value = Const.Int64 start), Expr.Const(value = Const.Int64 Int64.MinValue), Expr.Const(value = Const.Int64 finish) ->
            ValueSome(Const.UInt64((uint64 start - uint64 finish) / (uint64 Int64.MaxValue + 1UL) + 1UL))
        | Expr.Const(value = Const.IntPtr start), Expr.Const(value = Const.IntPtr Int64.MinValue), Expr.Const(value = Const.IntPtr finish) when
            start <= finish
            ->
            ValueSome(Const.UIntPtr((uint64 start - uint64 finish) / (uint64 Int64.MaxValue + 1UL) + 1UL))
        | Expr.Const(value = Const.IntPtr start), Expr.Const(value = Const.IntPtr Int64.MinValue), Expr.Const(value = Const.IntPtr finish) ->
            ValueSome(Const.UIntPtr((uint64 start - uint64 finish) / (uint64 Int64.MaxValue + 1UL) + 1UL))

        | Expr.Const(value = Const.Int64 start), Expr.Const(value = Const.Int64 step), Expr.Const(value = Const.Int64 finish) when
            start <= finish
            ->
            ValueSome(Const.UInt64((uint64 finish - uint64 start) / uint64 (abs step) + 1UL))
        | Expr.Const(value = Const.Int64 start), Expr.Const(value = Const.Int64 step), Expr.Const(value = Const.Int64 finish) ->
            ValueSome(Const.UInt64((uint64 start - uint64 finish) / uint64 (abs step) + 1UL))

        // sizeof<nativeint> is not constant, so |𝑐| ≥ 0x80000000n cannot be treated as a constant.
        | Expr.Const(value = Const.IntPtr start), Expr.Const(value = Const.IntPtr step), Expr.Const(value = Const.IntPtr finish) when
            uint64 start < 0x80000000UL
            && uint64 step < 0x80000000UL
            && uint64 finish < 0x80000000UL
            && start <= finish
            ->
            ValueSome(Const.UIntPtr((uint64 finish - uint64 start) / uint64 (abs step) + 1UL))

        | Expr.Const(value = Const.IntPtr start), Expr.Const(value = Const.IntPtr step), Expr.Const(value = Const.IntPtr finish) when
            uint64 start < 0x80000000UL
            && uint64 step < 0x80000000UL
            && uint64 finish < 0x80000000UL
            ->
            ValueSome(Const.UIntPtr((uint64 start - uint64 finish) / uint64 (abs step) + 1UL))

        | Expr.Const(value = Const.Int32 start), Expr.Const(value = Const.Int32 step), Expr.Const(value = Const.Int32 finish) when
            start <= finish
            ->
            ValueSome(Const.UInt64((uint64 finish - uint64 start) / uint64 (abs (int64 step)) + 1UL))
        | Expr.Const(value = Const.Int32 start), Expr.Const(value = Const.Int32 step), Expr.Const(value = Const.Int32 finish) ->
            ValueSome(Const.UInt64((uint64 start - uint64 finish) / uint64 (abs (int64 step)) + 1UL))

        | Expr.Const(value = Const.Int16 start), Expr.Const(value = Const.Int16 step), Expr.Const(value = Const.Int16 finish) when
            start <= finish
            ->
            ValueSome(Const.UInt32((uint finish - uint start) / uint (abs (int step)) + 1u))
        | Expr.Const(value = Const.Int16 start), Expr.Const(value = Const.Int16 step), Expr.Const(value = Const.Int16 finish) ->
            ValueSome(Const.UInt32((uint start - uint finish) / uint (abs (int step)) + 1u))

        | Expr.Const(value = Const.SByte start), Expr.Const(value = Const.SByte step), Expr.Const(value = Const.SByte finish) when
            start <= finish
            ->
            ValueSome(Const.UInt16((uint16 finish - uint16 start) / uint16 (abs (int16 step)) + 1us))
        | Expr.Const(value = Const.SByte start), Expr.Const(value = Const.SByte step), Expr.Const(value = Const.SByte finish) ->
            ValueSome(Const.UInt16((uint16 start - uint16 finish) / uint16 (abs (int16 step)) + 1us))

        // sizeof<unativeint> is not constant, so |𝑐| > 0xffffffffun cannot be treated as a constant.
        | Expr.Const(value = Const.UIntPtr start), Expr.Const(value = Const.UIntPtr step), Expr.Const(value = Const.UIntPtr finish) when
            start <= 0xffffffffUL && step <= 0xffffffffUL && finish <= 0xffffffffUL
            ->
            ValueSome(Const.UIntPtr((finish - start) / step + 1UL))

        | Expr.Const(value = Const.UInt64 start), Expr.Const(value = Const.UInt64 step), Expr.Const(value = Const.UInt64 finish) when
            start <= finish
            ->
            ValueSome(Const.UInt64((finish - start) / step + 1UL))
        | Expr.Const(value = Const.UInt64 start), Expr.Const(value = Const.UInt64 step), Expr.Const(value = Const.UInt64 finish) ->
            ValueSome(Const.UInt64((start - finish) / step + 1UL))
        | Expr.Const(value = Const.UInt32 start), Expr.Const(value = Const.UInt32 step), Expr.Const(value = Const.UInt32 finish) when
            start <= finish
            ->
            ValueSome(Const.UInt64(uint64 (finish - start) / uint64 step + 1UL))
        | Expr.Const(value = Const.UInt32 start), Expr.Const(value = Const.UInt32 step), Expr.Const(value = Const.UInt32 finish) ->
            ValueSome(Const.UInt64(uint64 (start - finish) / uint64 step + 1UL))
        | Expr.Const(value = Const.UInt16 start), Expr.Const(value = Const.UInt16 step), Expr.Const(value = Const.UInt16 finish) when
            start <= finish
            ->
            ValueSome(Const.UInt32(uint (finish - start) / uint step + 1u))
        | Expr.Const(value = Const.UInt16 start), Expr.Const(value = Const.UInt16 step), Expr.Const(value = Const.UInt16 finish) ->
            ValueSome(Const.UInt32(uint (start - finish) / uint step + 1u))
        | Expr.Const(value = Const.Byte start), Expr.Const(value = Const.Byte step), Expr.Const(value = Const.Byte finish) when
            start <= finish
            ->
            ValueSome(Const.UInt16(uint16 (finish - start) / uint16 step + 1us))
        | Expr.Const(value = Const.Byte start), Expr.Const(value = Const.Byte step), Expr.Const(value = Const.Byte finish) ->
            ValueSome(Const.UInt16(uint16 (start - finish) / uint16 step + 1us))
        | Expr.Const(value = Const.Char start), Expr.Const(value = Const.Char step), Expr.Const(value = Const.Char finish) when
            start <= finish
            ->
            ValueSome(Const.UInt32(uint (finish - start) / uint step + 1u))
        | Expr.Const(value = Const.Char start), Expr.Const(value = Const.Char step), Expr.Const(value = Const.Char finish) ->
            ValueSome(Const.UInt32(uint (start - finish) / uint step + 1u))

        | _ -> ValueNone

    type Count = Expr
    type Idx = Expr
    type Elem = Expr
    type Body = Expr
    type Loop = Expr
    type WouldOvf = Expr

    [<RequireQualifiedAccess; NoEquality; NoComparison>]
    type RangeCount =
        /// An expression representing a count known at compile time.
        | Constant of Count

        /// An expression representing a "count" whose step is known to be zero at compile time.
        /// Evaluating this expression at runtime will raise an exception.
        | ConstantZeroStep of Expr

        /// An expression to compute a count at runtime that will definitely fit in 64 bits without overflow.
        | Safe of Count

        /// A function for building a loop given an expression that may produce a count that
        /// would not fit in 64 bits without overflow, and an expression indicating whether
        /// evaluating the first expression directly would in fact overflow.
        | PossiblyOversize of ((Count -> WouldOvf -> Expr) -> Expr)

    /// Makes an expression to compute the iteration count for the given integral range.
    let mkRangeCount g m rangeTy rangeExpr start step finish =
        /// This will raise an exception at runtime if step is zero.
        let mkCallAndIgnoreRangeExpr start step finish =
            // Use the potentially-evaluated-and-bound start, step, and finish.
            let rangeExpr =
                match rangeExpr with
                // Type-specific range op (RangeInt32, etc.).
                | Expr.App(funcExpr, formalType, tyargs, [ _start; _step; _finish ], m) ->
                    Expr.App(funcExpr, formalType, tyargs, [ start; step; finish ], m)
                // Generic range–step op (RangeStepGeneric).
                | Expr.App(funcExpr, formalType, tyargs, [ zero; add; _start; _step; _finish ], m) ->
                    Expr.App(funcExpr, formalType, tyargs, [ zero; add; start; step; finish ], m)
                | _ -> error (InternalError($"Unrecognized range function application '{rangeExpr}'.", m))

            mkSequential m rangeExpr (mkUnit g m)

        let mkSignednessAppropriateClt ty e1 e2 =
            if isSignedIntegerTy g ty then
                mkILAsmClt g m e1 e2
            else
                mkAsmExpr ([ AI_clt_un ], [], [ e1; e2 ], [ g.bool_ty ], m)

        let unsignedEquivalent ty =
            if typeEquivAux EraseMeasures g ty g.int64_ty then
                g.uint64_ty
            elif typeEquivAux EraseMeasures g ty g.int32_ty then
                g.uint32_ty
            elif typeEquivAux EraseMeasures g ty g.int16_ty then
                g.uint16_ty
            elif typeEquivAux EraseMeasures g ty g.sbyte_ty then
                g.byte_ty
            else
                ty

        /// Find the unsigned type with twice the width of the given type, if available.
        let nextWidestUnsignedTy ty =
            if
                typeEquivAux EraseMeasures g ty g.int64_ty
                || typeEquivAux EraseMeasures g ty g.int32_ty
                || typeEquivAux EraseMeasures g ty g.uint32_ty
            then
                g.uint64_ty
            elif
                typeEquivAux EraseMeasures g ty g.int16_ty
                || typeEquivAux EraseMeasures g ty g.uint16_ty
                || typeEquivAux EraseMeasures g ty g.char_ty
            then
                g.uint32_ty
            elif
                typeEquivAux EraseMeasures g ty g.sbyte_ty
                || typeEquivAux EraseMeasures g ty g.byte_ty
            then
                g.uint16_ty
            else
                ty

        /// Convert the value to the next-widest unsigned type.
        /// We do this so that adding one won't result in overflow.
        let mkWiden e =
            if typeEquivAux EraseMeasures g rangeTy g.int32_ty then
                mkAsmExpr ([ AI_conv DT_I8 ], [], [ e ], [ g.uint64_ty ], m)
            elif typeEquivAux EraseMeasures g rangeTy g.uint32_ty then
                mkAsmExpr ([ AI_conv DT_U8 ], [], [ e ], [ g.uint64_ty ], m)
            elif typeEquivAux EraseMeasures g rangeTy g.int16_ty then
                mkAsmExpr ([ AI_conv DT_I4 ], [], [ e ], [ g.uint32_ty ], m)
            elif
                typeEquivAux EraseMeasures g rangeTy g.uint16_ty
                || typeEquivAux EraseMeasures g rangeTy g.char_ty
            then
                mkAsmExpr ([ AI_conv DT_U4 ], [], [ e ], [ g.uint32_ty ], m)
            elif typeEquivAux EraseMeasures g rangeTy g.sbyte_ty then
                mkAsmExpr ([ AI_conv DT_I2 ], [], [ e ], [ g.uint16_ty ], m)
            elif typeEquivAux EraseMeasures g rangeTy g.byte_ty then
                mkAsmExpr ([ AI_conv DT_U2 ], [], [ e ], [ g.uint16_ty ], m)
            else
                e

        /// Expects that |e1| ≥ |e2|.
        let mkDiff e1 e2 =
            mkAsmExpr ([ AI_sub ], [], [ e1; e2 ], [ unsignedEquivalent (tyOfExpr g e1) ], m)

        /// diff / step
        let mkQuotient diff step =
            mkAsmExpr ([ AI_div_un ], [], [ diff; step ], [ tyOfExpr g diff ], m)

        /// Whether the total count might not fit in 64 bits.
        let couldBeTooBig ty =
            typeEquivAux EraseMeasures g ty g.int64_ty
            || typeEquivAux EraseMeasures g ty g.uint64_ty
            || typeEquivAux EraseMeasures g ty g.nativeint_ty
            || typeEquivAux EraseMeasures g ty g.unativeint_ty

        /// pseudoCount + 1
        let mkAddOne pseudoCount =
            let pseudoCount = mkWiden pseudoCount
            let ty = tyOfExpr g pseudoCount

            if couldBeTooBig rangeTy then
                mkAsmExpr ([ AI_add_ovf_un ], [], [ pseudoCount; mkTypedOne g m ty ], [ ty ], m)
            else
                mkAsmExpr ([ AI_add ], [], [ pseudoCount; mkTypedOne g m ty ], [ ty ], m)

        let mkRuntimeCalc mkThrowIfStepIsZero pseudoCount count =
            if
                typeEquivAux EraseMeasures g rangeTy g.int64_ty
                || typeEquivAux EraseMeasures g rangeTy g.uint64_ty
            then
                RangeCount.PossiblyOversize(fun mkLoopExpr ->
                    mkThrowIfStepIsZero (
                        mkCompGenLetIn m (nameof pseudoCount) (tyOfExpr g pseudoCount) pseudoCount (fun (_, pseudoCount) ->
                            let wouldOvf =
                                mkILAsmCeq g m pseudoCount (Expr.Const(Const.UInt64 UInt64.MaxValue, m, g.uint64_ty))

                            mkCompGenLetIn m (nameof wouldOvf) g.bool_ty wouldOvf (fun (_, wouldOvf) -> mkLoopExpr count wouldOvf))
                    ))
            elif
                typeEquivAux EraseMeasures g rangeTy g.nativeint_ty
                || typeEquivAux EraseMeasures g rangeTy g.unativeint_ty
            then // We have a nativeint ty whose size we won't know till runtime.
                RangeCount.PossiblyOversize(fun mkLoopExpr ->
                    mkThrowIfStepIsZero (
                        mkCompGenLetIn m (nameof pseudoCount) (tyOfExpr g pseudoCount) pseudoCount (fun (_, pseudoCount) ->
                            let wouldOvf =
                                mkCond
                                    DebugPointAtBinding.NoneAtInvisible
                                    m
                                    g.bool_ty
                                    (mkILAsmCeq
                                        g
                                        m
                                        (mkAsmExpr ([ I_sizeof g.ilg.typ_IntPtr ], [], [], [ g.uint32_ty ], m))
                                        (Expr.Const(Const.UInt32 4u, m, g.uint32_ty)))
                                    (mkILAsmCeq g m pseudoCount (Expr.Const(Const.UIntPtr(uint64 UInt32.MaxValue), m, g.unativeint_ty)))
                                    (mkILAsmCeq g m pseudoCount (Expr.Const(Const.UIntPtr UInt64.MaxValue, m, g.unativeint_ty)))

                            mkCompGenLetIn m (nameof wouldOvf) g.bool_ty wouldOvf (fun (_, wouldOvf) -> mkLoopExpr count wouldOvf))
                    ))
            else
                RangeCount.Safe(mkThrowIfStepIsZero count)

        match start, step, finish with
        // start..0..finish
        | _, Expr.Const(value = IntegralConst.Zero), _ ->
            RangeCount.ConstantZeroStep(mkSequential m (mkCallAndIgnoreRangeExpr start step finish) (mkTypedZero g m rangeTy))

        // 5..1
        // 1..-1..5
        | EmptyRange -> RangeCount.Constant(mkTypedZero g m rangeTy)

        // 1..5
        // 1..2..5
        // 5..-1..1
        | ConstCount count -> RangeCount.Constant(Expr.Const(count, m, nextWidestUnsignedTy rangeTy))

        // start..finish
        // start..1..finish
        //
        //     if finish < start then 0 else finish - start + 1
        | _, Expr.Const(value = IntegralConst.One), _ ->
            let mkCount mkAddOne =
                let count = mkAddOne (mkDiff finish start)
                let countTy = tyOfExpr g count

                mkCond
                    DebugPointAtBinding.NoneAtInvisible
                    m
                    countTy
                    (mkSignednessAppropriateClt rangeTy finish start)
                    (mkTypedZero g m countTy)
                    count

            match start, finish with
            // The total count could exceed 2⁶⁴.
            | Expr.Const(value = Const.Int64 Int64.MinValue), _
            | _, Expr.Const(value = Const.Int64 Int64.MaxValue)
            | Expr.Const(value = Const.UInt64 UInt64.MinValue), _
            | _, Expr.Const(value = Const.UInt64 UInt64.MaxValue) -> mkRuntimeCalc id (mkCount id) (mkCount mkAddOne)

            // The total count could not exceed 2⁶⁴.
            | Expr.Const(value = Const.Int64 _), _
            | _, Expr.Const(value = Const.Int64 _)
            | Expr.Const(value = Const.UInt64 _), _
            | _, Expr.Const(value = Const.UInt64 _) -> RangeCount.Safe(mkCount mkAddOne)

            | _ -> mkRuntimeCalc id (mkCount id) (mkCount mkAddOne)

        // (Only possible for signed types.)
        //
        // start..-1..finish
        //
        //     if start < finish then 0 else start - finish + 1
        | _, Expr.Const(value = IntegralConst.MinusOne), _ ->
            let mkCount mkAddOne =
                let count = mkAddOne (mkDiff start finish)
                let countTy = tyOfExpr g count

                mkCond
                    DebugPointAtBinding.NoneAtInvisible
                    m
                    countTy
                    (mkSignednessAppropriateClt rangeTy start finish)
                    (mkTypedZero g m countTy)
                    count

            match start, finish with
            // The total count could exceed 2⁶⁴.
            | Expr.Const(value = Const.Int64 Int64.MaxValue), _
            | _, Expr.Const(value = Const.Int64 Int64.MinValue) -> mkRuntimeCalc id (mkCount id) (mkCount mkAddOne)

            // The total count could not exceed 2⁶⁴.
            | Expr.Const(value = Const.Int64 _), _
            | _, Expr.Const(value = Const.Int64 _) -> RangeCount.Safe(mkCount mkAddOne)

            | _ -> mkRuntimeCalc id (mkCount id) (mkCount mkAddOne)

        // start..2..finish
        //
        //     if finish < start then 0 else (finish - start) / step + 1
        | _, Expr.Const(value = IntegralConst.Positive), _ ->
            let count =
                let count = mkAddOne (mkQuotient (mkDiff finish start) step)
                let countTy = tyOfExpr g count

                mkCond
                    DebugPointAtBinding.NoneAtInvisible
                    m
                    countTy
                    (mkSignednessAppropriateClt rangeTy finish start)
                    (mkTypedZero g m countTy)
                    count

            // We know that the magnitude of step is greater than one,
            // so we know that the total count won't overflow.
            RangeCount.Safe count

        // (Only possible for signed types.)
        //
        // start..-2..finish
        //
        //     if start < finish then 0 else (start - finish) / abs step + 1
        | _, Expr.Const(value = IntegralConst.Negative as negativeStep), _ ->
            let count =
                let count =
                    mkAddOne (mkQuotient (mkDiff start finish) (Expr.Const(IntegralConst.abs negativeStep, m, unsignedEquivalent rangeTy)))

                let countTy = tyOfExpr g count

                mkCond
                    DebugPointAtBinding.NoneAtInvisible
                    m
                    countTy
                    (mkSignednessAppropriateClt rangeTy start finish)
                    (mkTypedZero g m countTy)
                    count

            // We know that the magnitude of step is greater than one,
            // so we know that the total count won't overflow.
            RangeCount.Safe count

        // start..step..finish
        //
        //     if step = 0 then
        //         ignore ((.. ..) start step finish) // Throws.
        //     if 0 < step then
        //         if finish < start then 0 else unsigned (finish - start) / unsigned step + 1
        //     else // step < 0
        //         if start < finish then 0 else unsigned (start - finish) / unsigned (abs step) + 1
        | _, _, _ ->
            // Let the range call throw the appropriate localized
            // exception at runtime if step is zero:
            //
            //     if step = 0 then ignore ((.. ..) start step finish)
            let mkThrowIfStepIsZero count =
                let throwIfStepIsZero =
                    mkCond
                        DebugPointAtBinding.NoneAtInvisible
                        m
                        g.unit_ty
                        (mkILAsmCeq g m step (mkTypedZero g m rangeTy))
                        (mkCallAndIgnoreRangeExpr start step finish)
                        (mkUnit g m)

                mkSequential m throwIfStepIsZero count

            let mkCount mkAddOne =
                if isSignedIntegerTy g rangeTy then
                    let positiveStep =
                        let count = mkAddOne (mkQuotient (mkDiff finish start) step)
                        let countTy = tyOfExpr g count

                        mkCond
                            DebugPointAtBinding.NoneAtInvisible
                            m
                            countTy
                            (mkSignednessAppropriateClt rangeTy finish start)
                            (mkTypedZero g m countTy)
                            count

                    let negativeStep =
                        let absStep =
                            mkAsmExpr (
                                [ AI_add ],
                                [],
                                [ mkAsmExpr ([ AI_not ], [], [ step ], [ rangeTy ], m); mkTypedOne g m rangeTy ],
                                [ rangeTy ],
                                m
                            )

                        let count = mkAddOne (mkQuotient (mkDiff start finish) absStep)
                        let countTy = tyOfExpr g count

                        mkCond
                            DebugPointAtBinding.NoneAtInvisible
                            m
                            countTy
                            (mkSignednessAppropriateClt rangeTy start finish)
                            (mkTypedZero g m countTy)
                            count

                    mkCond
                        DebugPointAtBinding.NoneAtInvisible
                        m
                        (tyOfExpr g positiveStep)
                        (mkSignednessAppropriateClt rangeTy (mkTypedZero g m rangeTy) step)
                        positiveStep
                        negativeStep
                else // Unsigned.
                    let count = mkAddOne (mkQuotient (mkDiff finish start) step)
                    let countTy = tyOfExpr g count

                    mkCond
                        DebugPointAtBinding.NoneAtInvisible
                        m
                        countTy
                        (mkSignednessAppropriateClt rangeTy finish start)
                        (mkTypedZero g m countTy)
                        count

            match start, finish with
            // The total count could exceed 2⁶⁴.
            | Expr.Const(value = Const.Int64 Int64.MinValue), _
            | _, Expr.Const(value = Const.Int64 Int64.MaxValue)
            | Expr.Const(value = Const.Int64 Int64.MaxValue), _
            | _, Expr.Const(value = Const.Int64 Int64.MinValue)
            | Expr.Const(value = Const.UInt64 UInt64.MinValue), _
            | _, Expr.Const(value = Const.UInt64 UInt64.MaxValue) -> mkRuntimeCalc mkThrowIfStepIsZero (mkCount id) (mkCount mkAddOne)

            // The total count could not exceed 2⁶⁴.
            | Expr.Const(value = Const.Int64 _), _
            | _, Expr.Const(value = Const.Int64 _)
            | Expr.Const(value = Const.UInt64 _), _
            | _, Expr.Const(value = Const.UInt64 _) -> RangeCount.Safe(mkThrowIfStepIsZero (mkCount mkAddOne))

            | _ -> mkRuntimeCalc mkThrowIfStepIsZero (mkCount id) (mkCount mkAddOne)

    let mkOptimizedRangeLoop
        (g: TcGlobals)
        (mBody, mFor, mIn, spInWhile)
        (rangeTy, rangeExpr)
        (start, step, finish)
        (buildLoop: Count -> ((Idx -> Elem -> Body) -> Loop) -> Expr)
        =
        let inline mkLetBindingsIfNeeded f =
            match start, step, finish with
            | (Expr.Const _ | Expr.Val _), (Expr.Const _ | Expr.Val _), (Expr.Const _ | Expr.Val _) -> f start step finish

            | (Expr.Const _ | Expr.Val _), (Expr.Const _ | Expr.Val _), _ ->
                mkCompGenLetIn finish.Range (nameof finish) rangeTy finish (fun (_, finish) -> f start step finish)

            | _, (Expr.Const _ | Expr.Val _), (Expr.Const _ | Expr.Val _) ->
                mkCompGenLetIn start.Range (nameof start) rangeTy start (fun (_, start) -> f start step finish)

            | (Expr.Const _ | Expr.Val _), _, (Expr.Const _ | Expr.Val _) ->
                mkCompGenLetIn step.Range (nameof step) rangeTy step (fun (_, step) -> f start step finish)

            | _, (Expr.Const _ | Expr.Val _), _ ->
                mkCompGenLetIn start.Range (nameof start) rangeTy start (fun (_, start) ->
                    mkCompGenLetIn finish.Range (nameof finish) rangeTy finish (fun (_, finish) -> f start step finish))

            | (Expr.Const _ | Expr.Val _), _, _ ->
                mkCompGenLetIn step.Range (nameof step) rangeTy step (fun (_, step) ->
                    mkCompGenLetIn finish.Range (nameof finish) rangeTy finish (fun (_, finish) -> f start step finish))

            | _, _, (Expr.Const _ | Expr.Val _) ->
                mkCompGenLetIn start.Range (nameof start) rangeTy start (fun (_, start) ->
                    mkCompGenLetIn step.Range (nameof step) rangeTy step (fun (_, step) -> f start step finish))

            | _, _, _ ->
                mkCompGenLetIn start.Range (nameof start) rangeTy start (fun (_, start) ->
                    mkCompGenLetIn step.Range (nameof step) rangeTy step (fun (_, step) ->
                        mkCompGenLetIn finish.Range (nameof finish) rangeTy finish (fun (_, finish) -> f start step finish)))

        mkLetBindingsIfNeeded (fun start step finish ->
            /// Start at 0 and count up through count - 1.
            ///
            ///     while i < count do
            ///         <body>
            ///         i <- i + 1
            let mkCountUpExclusive mkBody count =
                let countTy = tyOfExpr g count

                mkCompGenLetMutableIn mIn "i" countTy (mkTypedZero g mIn countTy) (fun (idxVal, idxVar) ->
                    mkCompGenLetMutableIn mIn "loopVar" rangeTy start (fun (loopVal, loopVar) ->
                        // loopVar <- loopVar + step
                        let incrV =
                            mkValSet mIn (mkLocalValRef loopVal) (mkAsmExpr ([ AI_add ], [], [ loopVar; step ], [ rangeTy ], mIn))

                        // i <- i + 1
                        let incrI =
                            mkValSet
                                mIn
                                (mkLocalValRef idxVal)
                                (mkAsmExpr ([ AI_add ], [], [ idxVar; mkTypedOne g mIn countTy ], [ rangeTy ], mIn))

                        // <body>
                        // loopVar <- loopVar + step
                        // i <- i + 1
                        let body = mkSequentials g mBody [ mkBody idxVar loopVar; incrV; incrI ]

                        // i < count
                        let guard = mkAsmExpr ([ AI_clt_un ], [], [ idxVar; count ], [ g.bool_ty ], mFor)

                        // while i < count do
                        //     <body>
                        //     loopVar <- loopVar + step
                        //     i <- i + 1
                        mkWhile g (spInWhile, WhileLoopForCompiledForEachExprMarker, guard, body, mBody)))

            /// Start at 0 and count up till we have wrapped around.
            /// We only emit this if the type is or may be 64-bit and step is not constant,
            /// and we only execute it if step = 1 and |finish - step| = 2⁶⁴ + 1.
            ///
            /// Logically equivalent to (pseudo-code):
            ///
            ///     while true do
            ///         <body>
            ///         loopVar <- loopVar + step
            ///         i <- i + 1
            ///         if i = 0 then break
            let mkCountUpInclusive mkBody countTy =
                mkCompGenLetMutableIn mFor "guard" g.bool_ty (mkTrue g mFor) (fun (guardVal, guardVar) ->
                    mkCompGenLetMutableIn mIn "i" countTy (mkTypedZero g mIn countTy) (fun (idxVal, idxVar) ->
                        mkCompGenLetMutableIn mIn "loopVar" rangeTy start (fun (loopVal, loopVar) ->
                            // loopVar <- loopVar + step
                            let incrV =
                                mkValSet mIn (mkLocalValRef loopVal) (mkAsmExpr ([ AI_add ], [], [ loopVar; step ], [ rangeTy ], mIn))

                            // i <- i + 1
                            let incrI =
                                mkValSet
                                    mIn
                                    (mkLocalValRef idxVal)
                                    (mkAsmExpr ([ AI_add ], [], [ idxVar; mkTypedOne g mIn countTy ], [ rangeTy ], mIn))

                            // guard <- i <> 0
                            let breakIfZero =
                                mkValSet
                                    mFor
                                    (mkLocalValRef guardVal)
                                    (mkAsmExpr ([ ILInstr.AI_cgt_un ], [], [ idxVar; mkTypedZero g mFor countTy ], [ g.bool_ty ], mFor))

                            // <body>
                            // loopVar <- loopVar + step
                            // i <- i + 1
                            // guard <- i <> 0
                            let body =
                                mkSequentials g mBody [ mkBody idxVar loopVar; incrV; incrI; breakIfZero ]

                            // while guard do
                            //     <body>
                            //     loopVar <- loopVar + step
                            //     i <- i + 1
                            //     guard <- i <> 0
                            mkWhile g (spInWhile, WhileLoopForCompiledForEachExprMarker, guardVar, body, mBody))))

            match mkRangeCount g mIn rangeTy rangeExpr start step finish with
            | RangeCount.Constant count -> buildLoop count (fun mkBody -> mkCountUpExclusive mkBody count)

            | RangeCount.ConstantZeroStep count ->
                mkCompGenLetIn mIn (nameof count) (tyOfExpr g count) count (fun (_, count) ->
                    buildLoop count (fun mkBody -> mkCountUpExclusive mkBody count))

            | RangeCount.Safe count ->
                mkCompGenLetIn mIn (nameof count) (tyOfExpr g count) count (fun (_, count) ->
                    buildLoop count (fun mkBody -> mkCountUpExclusive mkBody count))

            | RangeCount.PossiblyOversize calc ->
                calc (fun count wouldOvf ->
                    buildLoop count (fun mkBody ->
                        // mkBody creates expressions that may contain lambdas with unique stamps.
                        // We need to copy the expression for the second branch to avoid duplicate type names.
                        let mkBodyCopied idxVar loopVar =
                            copyExpr g CloneAll (mkBody idxVar loopVar)

                        mkCond
                            DebugPointAtBinding.NoneAtInvisible
                            mIn
                            g.unit_ty
                            wouldOvf
                            (mkCountUpInclusive mkBody (tyOfExpr g count))
                            (mkCompGenLetIn mIn (nameof count) (tyOfExpr g count) count (fun (_, count) ->
                                mkCountUpExclusive mkBodyCopied count)))))

    type OptimizeForExpressionOptions =
        | OptimizeIntRangesOnly
        | OptimizeAllForExpressions

    let DetectAndOptimizeForEachExpression g option expr =
        match option, expr with
        | _, CompiledInt32RangeForEachExpr g (startExpr, (1 | -1 as step), finishExpr, elemVar, bodyExpr, ranges) ->

            let _mBody, spFor, spIn, _mFor, _mIn, _spInWhile, mWholeExpr = ranges

            let spFor =
                match spFor with
                | DebugPointAtBinding.Yes mFor -> DebugPointAtFor.Yes mFor
                | _ -> DebugPointAtFor.No

            mkFastForLoop g (spFor, spIn, mWholeExpr, elemVar, startExpr, (step = 1), finishExpr, bodyExpr)

        | OptimizeAllForExpressions, CompiledForEachExpr g (enumerableTy, enumerableExpr, elemVar, bodyExpr, ranges) ->
            match
                (if g.langVersion.SupportsFeature LanguageFeature.LowerIntegralRangesToFastLoops then
                     tryMatchIntegralRange g enumerableExpr
                 else
                     ValueNone)
            with
            | ValueSome(rangeTy, (start, step, finish)) ->
                let mBody, _spFor, _spIn, mFor, mIn, spInWhile, _mWhole = ranges

                mkOptimizedRangeLoop g (mBody, mFor, mIn, spInWhile) (rangeTy, enumerableExpr) (start, step, finish) (fun _count mkLoop ->
                    mkLoop (fun _idxVar loopVar -> mkInvisibleLet elemVar.Range elemVar loopVar bodyExpr))
            | ValueNone ->

                let mBody, spFor, spIn, mFor, mIn, spInWhile, mWholeExpr = ranges

                if isStringTy g enumerableTy then
                    // type is string, optimize for expression as:
                    //  let $str = enumerable
                    //  for $idx = 0 to str.Length - 1 do
                    //      let elem = str.[idx]
                    //      body elem

                    let strVar, strExpr = mkCompGenLocal mFor "str" enumerableTy
                    let idxVar, idxExpr = mkCompGenLocal elemVar.Range "idx" g.int32_ty

                    let lengthExpr = mkGetStringLength g mFor strExpr
                    let charExpr = mkGetStringChar g mFor strExpr idxExpr

                    let startExpr = mkZero g mFor
                    let finishExpr = mkDecr g mFor lengthExpr
                    // for compat reasons, loop item over string is sometimes object, not char
                    let loopItemExpr = mkCoerceIfNeeded g elemVar.Type g.char_ty charExpr
                    let bodyExpr = mkInvisibleLet mIn elemVar loopItemExpr bodyExpr

                    let forExpr =
                        mkFastForLoop g (DebugPointAtFor.No, spIn, mWholeExpr, idxVar, startExpr, true, finishExpr, bodyExpr)

                    let expr = mkLet spFor mFor strVar enumerableExpr forExpr

                    expr

                elif isListTy g enumerableTy then
                    // type is list, optimize for expression as:
                    //  let mutable $currentVar = listExpr
                    //  let mutable $nextVar = $tailOrNull
                    //  while $guardExpr do
                    //    let i = $headExpr
                    //    bodyExpr ()
                    //    $current <- $next
                    //    $next <- $tailOrNull

                    let IndexHead = 0
                    let IndexTail = 1

                    let currentVar, currentExpr = mkMutableCompGenLocal mIn "current" enumerableTy
                    let nextVar, nextExpr = mkMutableCompGenLocal mIn "next" enumerableTy
                    let elemTy = destListTy g enumerableTy

                    let guardExpr = mkNonNullTest g mFor nextExpr

                    let headOrDefaultExpr =
                        mkUnionCaseFieldGetUnprovenViaExprAddr (currentExpr, g.cons_ucref, [ elemTy ], IndexHead, mIn)

                    let tailOrNullExpr =
                        mkUnionCaseFieldGetUnprovenViaExprAddr (currentExpr, g.cons_ucref, [ elemTy ], IndexTail, mIn)

                    let bodyExpr =
                        mkInvisibleLet
                            mIn
                            elemVar
                            headOrDefaultExpr
                            (mkSequential
                                mIn
                                bodyExpr
                                (mkSequential
                                    mIn
                                    (mkValSet mIn (mkLocalValRef currentVar) nextExpr)
                                    (mkValSet mIn (mkLocalValRef nextVar) tailOrNullExpr)))

                    let expr =
                        // let mutable current = enumerableExpr
                        mkLet
                            spFor
                            mIn
                            currentVar
                            enumerableExpr
                            // let mutable next = current.TailOrNull
                            (mkInvisibleLet
                                mFor
                                nextVar
                                tailOrNullExpr
                                // while nonNull next do
                                (mkWhile g (spInWhile, WhileLoopForCompiledForEachExprMarker, guardExpr, bodyExpr, mBody)))

                    expr

                else
                    expr

        | _ -> expr

    /// One of the transformations performed by the compiler
    /// is to eliminate variables of static type "unit". These is a
    /// utility function related to this.

    let BindUnitVars g (mvs: Val list, paramInfos: ArgReprInfo list, body) =
        match mvs, paramInfos with
        | [ v ], [] ->
            assert isUnitTy g v.Type
            [], mkLet DebugPointAtBinding.NoneAtInvisible v.Range v (mkUnit g v.Range) body
        | _ -> mvs, body

    let mkUnitDelayLambda (g: TcGlobals) m e =
        let uv, _ = mkCompGenLocal m "unitVar" g.unit_ty
        mkLambda m uv (e, tyOfExpr g e)

    [<return: Struct>]
    let (|UseResumableStateMachinesExpr|_|) g expr =
        match expr with
        | ValApp g g.cgh__useResumableCode_vref (_, _, _m) -> ValueSome()
        | _ -> ValueNone

    /// Match an if...then...else expression or the result of "a && b" or "a || b"
    [<return: Struct>]
    let (|IfThenElseExpr|_|) expr =
        match expr with
        | Expr.Match(_spBind,
                     _exprm,
                     TDSwitch(cond, [ TCase(DecisionTreeTest.Const(Const.Bool true), TDSuccess([], 0)) ], Some(TDSuccess([], 1)), _),
                     [| TTarget([], thenExpr, _); TTarget([], elseExpr, _) |],
                     _m,
                     _ty) -> ValueSome(cond, thenExpr, elseExpr)
        | _ -> ValueNone

    /// if __useResumableCode then ... else ...
    [<return: Struct>]
    let (|IfUseResumableStateMachinesExpr|_|) g expr =
        match expr with
        | IfThenElseExpr(UseResumableStateMachinesExpr g (), thenExpr, elseExpr) -> ValueSome(thenExpr, elseExpr)
        | _ -> ValueNone

[<AutoOpen>]
module internal ConstantEvaluation =

    /// Accessing a binding of the form "let x = 1" or "let x = e" for any "e" satisfying the predicate
    /// below does not cause an initialization trigger, i.e. does not get compiled as a static field.
    let IsSimpleSyntacticConstantExpr g inputExpr =
        let rec checkExpr (vrefs: Set<Stamp>) x =
            match stripExpr x with
            | Expr.Op(TOp.Coerce, _, [ arg ], _) -> checkExpr vrefs arg
            | UnopExpr g (vref, arg) when
                (valRefEq g vref g.unchecked_unary_minus_vref
                 || valRefEq g vref g.unchecked_unary_plus_vref
                 || valRefEq g vref g.unchecked_unary_not_vref
                 || valRefEq g vref g.bitwise_unary_not_vref
                 || valRefEq g vref g.enum_vref)
                ->
                checkExpr vrefs arg
            // compare, =, <>, +, -, <, >, <=, >=, <<<, >>>, &&&, |||, ^^^
            | BinopExpr g (vref, arg1, arg2) when
                (valRefEq g vref g.equals_operator_vref
                 || valRefEq g vref g.compare_operator_vref
                 || valRefEq g vref g.unchecked_addition_vref
                 || valRefEq g vref g.less_than_operator_vref
                 || valRefEq g vref g.less_than_or_equals_operator_vref
                 || valRefEq g vref g.greater_than_operator_vref
                 || valRefEq g vref g.greater_than_or_equals_operator_vref
                 || valRefEq g vref g.not_equals_operator_vref
                 || valRefEq g vref g.unchecked_addition_vref
                 || valRefEq g vref g.unchecked_multiply_vref
                 || valRefEq g vref g.unchecked_subtraction_vref
                 ||
                 // Note: division and modulus can raise exceptions, so are not included
                 valRefEq g vref g.bitwise_shift_left_vref
                 || valRefEq g vref g.bitwise_shift_right_vref
                 || valRefEq g vref g.bitwise_xor_vref
                 || valRefEq g vref g.bitwise_and_vref
                 || valRefEq g vref g.bitwise_or_vref
                 || valRefEq g vref g.exponentiation_vref)
                && (not (typeEquiv g (tyOfExpr g arg1) g.string_ty)
                    && not (typeEquiv g (tyOfExpr g arg1) g.decimal_ty))
                ->
                checkExpr vrefs arg1 && checkExpr vrefs arg2
            | Expr.Val(vref, _, _) -> vref.Deref.IsCompiledAsStaticPropertyWithoutField || vrefs.Contains vref.Stamp
            | Expr.Match(_, _, dtree, targets, _, _) ->
                checkDecisionTree vrefs dtree
                && targets |> Array.forall (checkDecisionTreeTarget vrefs)
            | Expr.Let(b, e, _, _) -> checkExpr vrefs b.Expr && checkExpr (vrefs.Add b.Var.Stamp) e
            | Expr.DebugPoint(_, b) -> checkExpr vrefs b
            | Expr.TyChoose(_, b, _) -> checkExpr vrefs b
            // Detect standard constants
            | Expr.Const _
            | Expr.Op(TOp.UnionCase _, _, [], _) // Nullary union cases
            | UncheckedDefaultOfExpr g _
            | SizeOfExpr g _
            | TypeOfExpr g _ -> true
            | NameOfExpr g _ when g.langVersion.SupportsFeature LanguageFeature.NameOf -> true
            // All others are not simple constant expressions
            | _ -> false

        and checkDecisionTree vrefs x =
            match x with
            | TDSuccess(es, _n) -> es |> List.forall (checkExpr vrefs)
            | TDSwitch(e, cases, dflt, _m) ->
                checkExpr vrefs e
                && cases |> List.forall (checkDecisionTreeCase vrefs)
                && dflt |> Option.forall (checkDecisionTree vrefs)
            | TDBind(bind, body) -> checkExpr vrefs bind.Expr && checkDecisionTree (vrefs.Add bind.Var.Stamp) body

        and checkDecisionTreeCase vrefs (TCase(discrim, dtree)) =
            (match discrim with
             | DecisionTreeTest.Const _c -> true
             | _ -> false)
            && checkDecisionTree vrefs dtree

        and checkDecisionTreeTarget vrefs (TTarget(vs, e, _)) =
            let vrefs = ((vrefs, vs) ||> List.fold (fun s v -> s.Add v.Stamp))
            checkExpr vrefs e

        checkExpr Set.empty inputExpr

    let EvalArithShiftOp (opInt8, opInt16, opInt32, opInt64, opUInt8, opUInt16, opUInt32, opUInt64) (arg1: Expr) (arg2: Expr) =
        // At compile-time we check arithmetic
        let m = unionRanges arg1.Range arg2.Range

        try
            match arg1, arg2 with
            | Expr.Const(Const.Int32 x1, _, ty), Expr.Const(Const.Int32 shift, _, _) -> Expr.Const(Const.Int32(opInt32 x1 shift), m, ty)
            | Expr.Const(Const.SByte x1, _, ty), Expr.Const(Const.Int32 shift, _, _) -> Expr.Const(Const.SByte(opInt8 x1 shift), m, ty)
            | Expr.Const(Const.Int16 x1, _, ty), Expr.Const(Const.Int32 shift, _, _) -> Expr.Const(Const.Int16(opInt16 x1 shift), m, ty)
            | Expr.Const(Const.Int64 x1, _, ty), Expr.Const(Const.Int32 shift, _, _) -> Expr.Const(Const.Int64(opInt64 x1 shift), m, ty)
            | Expr.Const(Const.Byte x1, _, ty), Expr.Const(Const.Int32 shift, _, _) -> Expr.Const(Const.Byte(opUInt8 x1 shift), m, ty)
            | Expr.Const(Const.UInt16 x1, _, ty), Expr.Const(Const.Int32 shift, _, _) -> Expr.Const(Const.UInt16(opUInt16 x1 shift), m, ty)
            | Expr.Const(Const.UInt32 x1, _, ty), Expr.Const(Const.Int32 shift, _, _) -> Expr.Const(Const.UInt32(opUInt32 x1 shift), m, ty)
            | Expr.Const(Const.UInt64 x1, _, ty), Expr.Const(Const.Int32 shift, _, _) -> Expr.Const(Const.UInt64(opUInt64 x1 shift), m, ty)
            | _ -> error (Error(FSComp.SR.tastNotAConstantExpression (), m))
        with :? OverflowException ->
            error (Error(FSComp.SR.tastConstantExpressionOverflow (), m))

    let EvalArithUnOp (opInt8, opInt16, opInt32, opInt64, opUInt8, opUInt16, opUInt32, opUInt64, opSingle, opDouble) (arg1: Expr) =
        // At compile-time we check arithmetic
        let m = arg1.Range

        try
            match arg1 with
            | Expr.Const(Const.Int32 x1, _, ty) -> Expr.Const(Const.Int32(opInt32 x1), m, ty)
            | Expr.Const(Const.SByte x1, _, ty) -> Expr.Const(Const.SByte(opInt8 x1), m, ty)
            | Expr.Const(Const.Int16 x1, _, ty) -> Expr.Const(Const.Int16(opInt16 x1), m, ty)
            | Expr.Const(Const.Int64 x1, _, ty) -> Expr.Const(Const.Int64(opInt64 x1), m, ty)
            | Expr.Const(Const.Byte x1, _, ty) -> Expr.Const(Const.Byte(opUInt8 x1), m, ty)
            | Expr.Const(Const.UInt16 x1, _, ty) -> Expr.Const(Const.UInt16(opUInt16 x1), m, ty)
            | Expr.Const(Const.UInt32 x1, _, ty) -> Expr.Const(Const.UInt32(opUInt32 x1), m, ty)
            | Expr.Const(Const.UInt64 x1, _, ty) -> Expr.Const(Const.UInt64(opUInt64 x1), m, ty)
            | Expr.Const(Const.Single x1, _, ty) -> Expr.Const(Const.Single(opSingle x1), m, ty)
            | Expr.Const(Const.Double x1, _, ty) -> Expr.Const(Const.Double(opDouble x1), m, ty)
            | _ -> error (Error(FSComp.SR.tastNotAConstantExpression (), m))
        with :? OverflowException ->
            error (Error(FSComp.SR.tastConstantExpressionOverflow (), m))

    let EvalArithBinOp
        (opInt8, opInt16, opInt32, opInt64, opUInt8, opUInt16, opUInt32, opUInt64, opSingle, opDouble, opDecimal)
        (arg1: Expr)
        (arg2: Expr)
        =
        // At compile-time we check arithmetic
        let m = unionRanges arg1.Range arg2.Range

        try
            match arg1, arg2 with
            | Expr.Const(Const.Int32 x1, _, ty), Expr.Const(Const.Int32 x2, _, _) -> Expr.Const(Const.Int32(opInt32 x1 x2), m, ty)
            | Expr.Const(Const.SByte x1, _, ty), Expr.Const(Const.SByte x2, _, _) -> Expr.Const(Const.SByte(opInt8 x1 x2), m, ty)
            | Expr.Const(Const.Int16 x1, _, ty), Expr.Const(Const.Int16 x2, _, _) -> Expr.Const(Const.Int16(opInt16 x1 x2), m, ty)
            | Expr.Const(Const.Int64 x1, _, ty), Expr.Const(Const.Int64 x2, _, _) -> Expr.Const(Const.Int64(opInt64 x1 x2), m, ty)
            | Expr.Const(Const.Byte x1, _, ty), Expr.Const(Const.Byte x2, _, _) -> Expr.Const(Const.Byte(opUInt8 x1 x2), m, ty)
            | Expr.Const(Const.UInt16 x1, _, ty), Expr.Const(Const.UInt16 x2, _, _) -> Expr.Const(Const.UInt16(opUInt16 x1 x2), m, ty)
            | Expr.Const(Const.UInt32 x1, _, ty), Expr.Const(Const.UInt32 x2, _, _) -> Expr.Const(Const.UInt32(opUInt32 x1 x2), m, ty)
            | Expr.Const(Const.UInt64 x1, _, ty), Expr.Const(Const.UInt64 x2, _, _) -> Expr.Const(Const.UInt64(opUInt64 x1 x2), m, ty)
            | Expr.Const(Const.Single x1, _, ty), Expr.Const(Const.Single x2, _, _) -> Expr.Const(Const.Single(opSingle x1 x2), m, ty)
            | Expr.Const(Const.Double x1, _, ty), Expr.Const(Const.Double x2, _, _) -> Expr.Const(Const.Double(opDouble x1 x2), m, ty)
            | Expr.Const(Const.Decimal x1, _, ty), Expr.Const(Const.Decimal x2, _, _) -> Expr.Const(Const.Decimal(opDecimal x1 x2), m, ty)
            | _ -> error (Error(FSComp.SR.tastNotAConstantExpression (), m))
        with :? OverflowException ->
            error (Error(FSComp.SR.tastConstantExpressionOverflow (), m))

    // See also PostTypeCheckSemanticChecks.CheckAttribArgExpr, which must match this precisely
    let rec EvalAttribArgExpr suppressLangFeatureCheck (g: TcGlobals) (x: Expr) =
        let ignore (_x: 'a) = Unchecked.defaultof<'a>
        let ignore2 (_x: 'a) (_y: 'a) = Unchecked.defaultof<'a>

        let inline checkFeature () =
            if suppressLangFeatureCheck = SuppressLanguageFeatureCheck.No then
                checkLanguageFeatureAndRecover g.langVersion LanguageFeature.ArithmeticInLiterals x.Range

        match x with

        // Detect standard constants
        | Expr.Const(c, m, _) ->
            match c with
            | Const.Bool _
            | Const.Int32 _
            | Const.SByte _
            | Const.Int16 _
            | Const.Int32 _
            | Const.Int64 _
            | Const.Byte _
            | Const.UInt16 _
            | Const.UInt32 _
            | Const.UInt64 _
            | Const.Double _
            | Const.Single _
            | Const.Char _
            | Const.Zero
            | Const.String _
            | Const.Decimal _ -> x
            | Const.IntPtr _
            | Const.UIntPtr _
            | Const.Unit ->
                errorR (Error(FSComp.SR.tastNotAConstantExpression (), m))
                x

        | TypeOfExpr g _ -> x
        | TypeDefOfExpr g _ -> x
        | Expr.Op(TOp.Coerce, _, [ arg ], _) -> EvalAttribArgExpr suppressLangFeatureCheck g arg
        | EnumExpr g arg1 -> EvalAttribArgExpr suppressLangFeatureCheck g arg1
        // Detect bitwise or of attribute flags
        | AttribBitwiseOrExpr g (arg1, arg2) ->
            let v1 = EvalAttribArgExpr suppressLangFeatureCheck g arg1

            match v1 with
            | IntegerConstExpr ->
                EvalArithBinOp
                    ((|||), (|||), (|||), (|||), (|||), (|||), (|||), (|||), ignore2, ignore2, ignore2)
                    v1
                    (EvalAttribArgExpr suppressLangFeatureCheck g arg2)
            | _ ->
                errorR (Error(FSComp.SR.tastNotAConstantExpression (), x.Range))
                x
        | SpecificBinopExpr g g.unchecked_addition_vref (arg1, arg2) ->
            let v1, v2 =
                EvalAttribArgExpr suppressLangFeatureCheck g arg1, EvalAttribArgExpr suppressLangFeatureCheck g arg2

            match v1, v2 with
            | Expr.Const(Const.String x1, m, ty), Expr.Const(Const.String x2, _, _) -> Expr.Const(Const.String(x1 + x2), m, ty)
            | Expr.Const(Const.Char x1, m, ty), Expr.Const(Const.Char x2, _, _) ->
                checkFeature ()
                Expr.Const(Const.Char(x1 + x2), m, ty)
            | _ ->
                checkFeature ()

                EvalArithBinOp
                    (Checked.(+),
                     Checked.(+),
                     Checked.(+),
                     Checked.(+),
                     Checked.(+),
                     Checked.(+),
                     Checked.(+),
                     Checked.(+),
                     Checked.(+),
                     Checked.(+),
                     Checked.(+))
                    v1
                    v2
        | SpecificBinopExpr g g.unchecked_subtraction_vref (arg1, arg2) ->
            checkFeature ()

            let v1, v2 =
                EvalAttribArgExpr SuppressLanguageFeatureCheck.Yes g arg1, EvalAttribArgExpr SuppressLanguageFeatureCheck.Yes g arg2

            match v1, v2 with
            | Expr.Const(Const.Char x1, m, ty), Expr.Const(Const.Char x2, _, _) -> Expr.Const(Const.Char(x1 - x2), m, ty)
            | _ ->
                EvalArithBinOp
                    (Checked.(-),
                     Checked.(-),
                     Checked.(-),
                     Checked.(-),
                     Checked.(-),
                     Checked.(-),
                     Checked.(-),
                     Checked.(-),
                     Checked.(-),
                     Checked.(-),
                     Checked.(-))
                    v1
                    v2
        | SpecificBinopExpr g g.unchecked_multiply_vref (arg1, arg2) ->
            checkFeature ()

            EvalArithBinOp
                (Checked.(*),
                 Checked.(*),
                 Checked.(*),
                 Checked.(*),
                 Checked.(*),
                 Checked.(*),
                 Checked.(*),
                 Checked.(*),
                 Checked.(*),
                 Checked.(*),
                 Checked.(*))
                (EvalAttribArgExpr SuppressLanguageFeatureCheck.Yes g arg1)
                (EvalAttribArgExpr SuppressLanguageFeatureCheck.Yes g arg2)
        | SpecificBinopExpr g g.unchecked_division_vref (arg1, arg2) ->
            checkFeature ()

            EvalArithBinOp
                ((/), (/), (/), (/), (/), (/), (/), (/), (/), (/), (/))
                (EvalAttribArgExpr SuppressLanguageFeatureCheck.Yes g arg1)
                (EvalAttribArgExpr SuppressLanguageFeatureCheck.Yes g arg2)
        | SpecificBinopExpr g g.unchecked_modulus_vref (arg1, arg2) ->
            checkFeature ()

            EvalArithBinOp
                ((%), (%), (%), (%), (%), (%), (%), (%), (%), (%), (%))
                (EvalAttribArgExpr SuppressLanguageFeatureCheck.Yes g arg1)
                (EvalAttribArgExpr SuppressLanguageFeatureCheck.Yes g arg2)
        | SpecificBinopExpr g g.bitwise_shift_left_vref (arg1, arg2) ->
            checkFeature ()

            EvalArithShiftOp
                ((<<<), (<<<), (<<<), (<<<), (<<<), (<<<), (<<<), (<<<))
                (EvalAttribArgExpr SuppressLanguageFeatureCheck.Yes g arg1)
                (EvalAttribArgExpr SuppressLanguageFeatureCheck.Yes g arg2)
        | SpecificBinopExpr g g.bitwise_shift_right_vref (arg1, arg2) ->
            checkFeature ()

            EvalArithShiftOp
                ((>>>), (>>>), (>>>), (>>>), (>>>), (>>>), (>>>), (>>>))
                (EvalAttribArgExpr SuppressLanguageFeatureCheck.Yes g arg1)
                (EvalAttribArgExpr SuppressLanguageFeatureCheck.Yes g arg2)
        | SpecificBinopExpr g g.bitwise_and_vref (arg1, arg2) ->
            checkFeature ()
            let v1 = EvalAttribArgExpr SuppressLanguageFeatureCheck.Yes g arg1

            match v1 with
            | IntegerConstExpr ->
                EvalArithBinOp
                    ((&&&), (&&&), (&&&), (&&&), (&&&), (&&&), (&&&), (&&&), ignore2, ignore2, ignore2)
                    v1
                    (EvalAttribArgExpr SuppressLanguageFeatureCheck.Yes g arg2)
            | _ ->
                errorR (Error(FSComp.SR.tastNotAConstantExpression (), x.Range))
                x
        | SpecificBinopExpr g g.bitwise_xor_vref (arg1, arg2) ->
            checkFeature ()
            let v1 = EvalAttribArgExpr SuppressLanguageFeatureCheck.Yes g arg1

            match v1 with
            | IntegerConstExpr ->
                EvalArithBinOp
                    ((^^^), (^^^), (^^^), (^^^), (^^^), (^^^), (^^^), (^^^), ignore2, ignore2, ignore2)
                    v1
                    (EvalAttribArgExpr SuppressLanguageFeatureCheck.Yes g arg2)
            | _ ->
                errorR (Error(FSComp.SR.tastNotAConstantExpression (), x.Range))
                x
        | SpecificBinopExpr g g.exponentiation_vref (arg1, arg2) ->
            checkFeature ()
            let v1 = EvalAttribArgExpr SuppressLanguageFeatureCheck.Yes g arg1

            match v1 with
            | FloatConstExpr ->
                EvalArithBinOp
                    (ignore2, ignore2, ignore2, ignore2, ignore2, ignore2, ignore2, ignore2, ( ** ), ( ** ), ignore2)
                    v1
                    (EvalAttribArgExpr SuppressLanguageFeatureCheck.Yes g arg2)
            | _ ->
                errorR (Error(FSComp.SR.tastNotAConstantExpression (), x.Range))
                x
        | SpecificUnopExpr g g.bitwise_unary_not_vref arg1 ->
            checkFeature ()
            let v1 = EvalAttribArgExpr SuppressLanguageFeatureCheck.Yes g arg1

            match v1 with
            | IntegerConstExpr ->
                EvalArithUnOp
                    ((~~~), (~~~), (~~~), (~~~), (~~~), (~~~), (~~~), (~~~), ignore, ignore)
                    (EvalAttribArgExpr SuppressLanguageFeatureCheck.Yes g arg1)
            | _ ->
                errorR (Error(FSComp.SR.tastNotAConstantExpression (), x.Range))
                x
        | SpecificUnopExpr g g.unchecked_unary_minus_vref arg1 ->
            checkFeature ()
            let v1 = EvalAttribArgExpr SuppressLanguageFeatureCheck.Yes g arg1

            match v1 with
            | SignedConstExpr ->
                EvalArithUnOp
                    (Checked.(~-), Checked.(~-), Checked.(~-), Checked.(~-), ignore, ignore, ignore, ignore, Checked.(~-), Checked.(~-))
                    v1
            | _ ->
                errorR (Error(FSComp.SR.tastNotAConstantExpression (), v1.Range))
                x
        | SpecificUnopExpr g g.unchecked_unary_plus_vref arg1 ->
            checkFeature ()

            EvalArithUnOp
                ((~+), (~+), (~+), (~+), (~+), (~+), (~+), (~+), (~+), (~+))
                (EvalAttribArgExpr SuppressLanguageFeatureCheck.Yes g arg1)
        | SpecificUnopExpr g g.unchecked_unary_not_vref arg1 ->
            checkFeature ()

            match EvalAttribArgExpr SuppressLanguageFeatureCheck.Yes g arg1 with
            | Expr.Const(Const.Bool value, m, ty) -> Expr.Const(Const.Bool(not value), m, ty)
            | expr ->
                errorR (Error(FSComp.SR.tastNotAConstantExpression (), expr.Range))
                x
        // Detect logical operations on booleans, which are represented as a match expression
        | Expr.Match(
            decision = TDSwitch(input = input; cases = [ TCase(DecisionTreeTest.Const(Const.Bool test), TDSuccess([], targetNum)) ])
            targets = [| TTarget(_, t0, _); TTarget(_, t1, _) |]) ->
            checkFeature ()

            match EvalAttribArgExpr SuppressLanguageFeatureCheck.Yes g (stripDebugPoints input) with
            | Expr.Const(Const.Bool value, _, _) ->
                let pass, fail = if targetNum = 0 then t0, t1 else t1, t0

                if value = test then
                    EvalAttribArgExpr SuppressLanguageFeatureCheck.Yes g (stripDebugPoints pass)
                else
                    EvalAttribArgExpr SuppressLanguageFeatureCheck.Yes g (stripDebugPoints fail)
            | _ ->
                errorR (Error(FSComp.SR.tastNotAConstantExpression (), x.Range))
                x
        | _ ->
            errorR (Error(FSComp.SR.tastNotAConstantExpression (), x.Range))
            x

    and EvaledAttribExprEquality g e1 e2 =
        match e1, e2 with
        | Expr.Const(c1, _, _), Expr.Const(c2, _, _) -> c1 = c2
        | TypeOfExpr g ty1, TypeOfExpr g ty2 -> typeEquiv g ty1 ty2
        | TypeDefOfExpr g ty1, TypeDefOfExpr g ty2 -> typeEquiv g ty1 ty2
        | _ -> false

    [<return: Struct>]
    let (|ConstToILFieldInit|_|) c =
        match c with
        | Const.SByte n -> ValueSome(ILFieldInit.Int8 n)
        | Const.Int16 n -> ValueSome(ILFieldInit.Int16 n)
        | Const.Int32 n -> ValueSome(ILFieldInit.Int32 n)
        | Const.Int64 n -> ValueSome(ILFieldInit.Int64 n)
        | Const.Byte n -> ValueSome(ILFieldInit.UInt8 n)
        | Const.UInt16 n -> ValueSome(ILFieldInit.UInt16 n)
        | Const.UInt32 n -> ValueSome(ILFieldInit.UInt32 n)
        | Const.UInt64 n -> ValueSome(ILFieldInit.UInt64 n)
        | Const.Bool n -> ValueSome(ILFieldInit.Bool n)
        | Const.Char n -> ValueSome(ILFieldInit.Char(uint16 n))
        | Const.Single n -> ValueSome(ILFieldInit.Single n)
        | Const.Double n -> ValueSome(ILFieldInit.Double n)
        | Const.String s -> ValueSome(ILFieldInit.String s)
        | Const.Zero -> ValueSome ILFieldInit.Null
        | _ -> ValueNone

    let EvalLiteralExprOrAttribArg g x =
        match x with
        | Expr.Op(TOp.Coerce, _, [ Expr.Op(TOp.Array, [ elemTy ], args, m) ], _)
        | Expr.Op(TOp.Array, [ elemTy ], args, m) ->
            let args = args |> List.map (EvalAttribArgExpr SuppressLanguageFeatureCheck.No g)
            Expr.Op(TOp.Array, [ elemTy ], args, m)
        | _ -> EvalAttribArgExpr SuppressLanguageFeatureCheck.No g x

    /// Match an Int32 constant expression
    [<return: Struct>]
    let (|Int32Expr|_|) expr =
        match expr with
        | Expr.Const(Const.Int32 n, _, _) -> ValueSome n
        | _ -> ValueNone

    /// start..finish
    /// start..step..finish
    [<return: Struct>]
    let (|IntegralRange|_|) g expr = tryMatchIntegralRange g expr

[<AutoOpen>]
module internal ResumableCodePatterns =

    [<return: Struct>]
    let (|MatchTwoCasesExpr|_|) expr =
        match expr with
        | Expr.Match(spBind,
                     mExpr,
                     TDSwitch(cond, [ TCase(DecisionTreeTest.UnionCase(ucref, a), TDSuccess([], tg1)) ], Some(TDSuccess([], tg2)), b),
                     tgs,
                     m,
                     ty) ->

            // How to rebuild this construct
            let rebuild (cond, ucref, tg1, tg2, tgs) =
                Expr.Match(
                    spBind,
                    mExpr,
                    TDSwitch(cond, [ TCase(DecisionTreeTest.UnionCase(ucref, a), TDSuccess([], tg1)) ], Some(TDSuccess([], tg2)), b),
                    tgs,
                    m,
                    ty
                )

            ValueSome(cond, ucref, tg1, tg2, tgs, rebuild)

        | _ -> ValueNone

    /// match e with None -> ... | Some v -> ... or other variations of the same
    [<return: Struct>]
    let (|MatchOptionExpr|_|) expr =
        match expr with
        | MatchTwoCasesExpr(cond, ucref, tg1, tg2, tgs, rebuildTwoCases) ->
            let tgNone, tgSome = if ucref.CaseName = "None" then tg1, tg2 else tg2, tg1

            match tgs[tgNone], tgs[tgSome] with
            | TTarget([], noneBranchExpr, b2),
              TTarget([],
                      Expr.Let(TBind(unionCaseVar, Expr.Op(TOp.UnionCaseProof a1, a2, a3, a4), a5),
                               Expr.Let(TBind(someVar, Expr.Op(TOp.UnionCaseFieldGet(a6a, a6b), a7, a8, a9), a10), someBranchExpr, a11, a12),
                               a13,
                               a14),
                      a16) when unionCaseVar.LogicalName = "unionCase" ->

                // How to rebuild this construct
                let rebuild (cond, noneBranchExpr, someVar, someBranchExpr) =
                    let tgs = Array.zeroCreate 2
                    tgs[tgNone] <- TTarget([], noneBranchExpr, b2)

                    tgs[tgSome] <-
                        TTarget(
                            [],
                            Expr.Let(
                                TBind(unionCaseVar, Expr.Op(TOp.UnionCaseProof a1, a2, a3, a4), a5),
                                Expr.Let(
                                    TBind(someVar, Expr.Op(TOp.UnionCaseFieldGet(a6a, a6b), a7, a8, a9), a10),
                                    someBranchExpr,
                                    a11,
                                    a12
                                ),
                                a13,
                                a14
                            ),
                            a16
                        )

                    rebuildTwoCases (cond, ucref, tg1, tg2, tgs)

                ValueSome(cond, noneBranchExpr, someVar, someBranchExpr, rebuild)
            | _ -> ValueNone
        | _ -> ValueNone

    [<return: Struct>]
    let (|ResumableEntryAppExpr|_|) g expr =
        match expr with
        | ValApp g g.cgh__resumableEntry_vref (_, _, _m) -> ValueSome()
        | _ -> ValueNone

    /// Match an (unoptimized) __resumableEntry expression
    [<return: Struct>]
    let (|ResumableEntryMatchExpr|_|) g expr =
        match expr with
        | Expr.Let(TBind(matchVar, matchExpr, sp1),
                   MatchOptionExpr(Expr.Val(matchVar2, b, c), noneBranchExpr, someVar, someBranchExpr, rebuildMatch),
                   d,
                   e) ->
            match matchExpr with
            | ResumableEntryAppExpr g () ->
                if valRefEq g (mkLocalValRef matchVar) matchVar2 then

                    // How to rebuild this construct
                    let rebuild (noneBranchExpr, someBranchExpr) =
                        Expr.Let(
                            TBind(matchVar, matchExpr, sp1),
                            rebuildMatch (Expr.Val(matchVar2, b, c), noneBranchExpr, someVar, someBranchExpr),
                            d,
                            e
                        )

                    ValueSome(noneBranchExpr, someVar, someBranchExpr, rebuild)

                else
                    ValueNone

            | _ -> ValueNone
        | _ -> ValueNone

    [<return: Struct>]
    let (|StructStateMachineExpr|_|) g expr =
        match expr with
        | ValApp g g.cgh__stateMachine_vref ([ dataTy; _resultTy ], [ moveNext; setStateMachine; afterCode ], _m) ->
            match moveNext, setStateMachine, afterCode with
            | NewDelegateExpr g (_, [ moveNextThisVar ], moveNextBody, _, _),
              NewDelegateExpr g (_, [ setStateMachineThisVar; setStateMachineStateVar ], setStateMachineBody, _, _),
              NewDelegateExpr g (_, [ afterCodeThisVar ], afterCodeBody, _, _) ->
                ValueSome(
                    dataTy,
                    (moveNextThisVar, moveNextBody),
                    (setStateMachineThisVar, setStateMachineStateVar, setStateMachineBody),
                    (afterCodeThisVar, afterCodeBody)
                )
            | _ -> ValueNone
        | _ -> ValueNone

    [<return: Struct>]
    let (|ResumeAtExpr|_|) g expr =
        match expr with
        | ValApp g g.cgh__resumeAt_vref (_, [ pcExpr ], _m) -> ValueSome pcExpr
        | _ -> ValueNone

    // Detect __debugPoint calls
    [<return: Struct>]
    let (|DebugPointExpr|_|) g expr =
        match expr with
        | ValApp g g.cgh__debugPoint_vref (_, [ StringExpr debugPointName ], _m) -> ValueSome debugPointName
        | _ -> ValueNone

    // Detect sequencing constructs in state machine code
    [<return: Struct>]
    let (|SequentialResumableCode|_|) (g: TcGlobals) expr =
        match expr with

        // e1; e2
        | Expr.Sequential(e1, e2, NormalSeq, m) -> ValueSome(e1, e2, m, (fun e1 e2 -> Expr.Sequential(e1, e2, NormalSeq, m)))

        // let __stack_step = e1 in e2
        | Expr.Let(bind, e2, m, _) when bind.Var.CompiledName(g.CompilerGlobalState).StartsWithOrdinal(stackVarPrefix) ->
            ValueSome(bind.Expr, e2, m, (fun e1 e2 -> mkLet bind.DebugPoint m bind.Var e1 e2))

        | _ -> ValueNone

    [<return: Struct>]
    let (|ResumableCodeInvoke|_|) g expr =
        match expr with
        // defn.Invoke x --> let arg = x in [defn][arg/x]
        | Expr.App(Expr.Val(invokeRef, _, _) as iref, a, b, f :: args, m) when
            invokeRef.LogicalName = "Invoke" && isReturnsResumableCodeTy g (tyOfExpr g f)
            ->
            ValueSome(iref, f, args, m, (fun (f2, args2) -> Expr.App((iref, a, b, (f2 :: args2), m))))
        | _ -> ValueNone

[<AutoOpen>]
module internal SeqExprPatterns =

    [<return: Struct>]
    let (|Seq|_|) g expr =
        match expr with
        // use 'seq { ... }' as an indicator
        | ValApp g g.seq_vref ([ elemTy ], [ e ], _m) -> ValueSome(e, elemTy)
        | _ -> ValueNone

    /// Detect a 'yield x' within a 'seq { ... }'
    [<return: Struct>]
    let (|SeqYield|_|) g expr =
        match expr with
        | ValApp g g.seq_singleton_vref (_, [ arg ], m) -> ValueSome(arg, m)
        | _ -> ValueNone

    /// Detect a 'expr; expr' within a 'seq { ... }'
    [<return: Struct>]
    let (|SeqAppend|_|) g expr =
        match expr with
        | ValApp g g.seq_append_vref (_, [ arg1; arg2 ], m) -> ValueSome(arg1, arg2, m)
        | _ -> ValueNone

    let isVarFreeInExpr v e =
        Zset.contains v (freeInExpr CollectTyparsAndLocals e).FreeLocals

    /// Detect a 'while gd do expr' within a 'seq { ... }'
    [<return: Struct>]
    let (|SeqWhile|_|) g expr =
        match expr with
        | ValApp g g.seq_generated_vref (_, [ Expr.Lambda(_, _, _, [ dummyv ], guardExpr, _, _); innerExpr ], m) when
            not (isVarFreeInExpr dummyv guardExpr)
            ->

            // The debug point for 'while' is attached to the innerExpr, see TcSequenceExpression
            let mWhile = innerExpr.Range

            let spWhile =
                match mWhile.NotedSourceConstruct with
                | NotedSourceConstruct.While -> DebugPointAtWhile.Yes mWhile
                | _ -> DebugPointAtWhile.No

            ValueSome(guardExpr, innerExpr, spWhile, m)

        | _ -> ValueNone

    [<return: Struct>]
    let (|SeqTryFinally|_|) g expr =
        match expr with
        | ValApp g g.seq_finally_vref (_, [ arg1; Expr.Lambda(_, _, _, [ dummyv ], compensation, _, _) as arg2 ], m) when
            not (isVarFreeInExpr dummyv compensation)
            ->

            // The debug point for 'try' and 'finally' are attached to the first and second arguments
            // respectively, see TcSequenceExpression
            let mTry = arg1.Range
            let mFinally = arg2.Range

            let spTry =
                match mTry.NotedSourceConstruct with
                | NotedSourceConstruct.Try -> DebugPointAtTry.Yes mTry
                | _ -> DebugPointAtTry.No

            let spFinally =
                match mFinally.NotedSourceConstruct with
                | NotedSourceConstruct.Finally -> DebugPointAtFinally.Yes mFinally
                | _ -> DebugPointAtFinally.No

            ValueSome(arg1, compensation, spTry, spFinally, m)

        | _ -> ValueNone

    [<return: Struct>]
    let (|SeqUsing|_|) g expr =
        match expr with
        | ValApp g g.seq_using_vref ([ _; _; elemTy ], [ resource; Expr.Lambda(_, _, _, [ v ], body, mBind, _) ], m) ->
            // The debug point mFor at the 'use x = ... ' gets attached to the lambda
            let spBind =
                match mBind.NotedSourceConstruct with
                | NotedSourceConstruct.Binding -> DebugPointAtBinding.Yes mBind
                | _ -> DebugPointAtBinding.NoneAtInvisible

            ValueSome(resource, v, body, elemTy, spBind, m)
        | _ -> ValueNone

    [<return: Struct>]
    let (|SeqForEach|_|) g expr =
        match expr with
        // Nested for loops are represented by calls to Seq.collect
        | ValApp g g.seq_collect_vref ([ _inpElemTy; _enumty2; genElemTy ], [ Expr.Lambda(_, _, _, [ v ], body, mIn, _); inp ], mFor) ->
            // The debug point mIn at the 'in' gets attached to the first argument, see TcSequenceExpression
            let spIn =
                match mIn.NotedSourceConstruct with
                | NotedSourceConstruct.InOrTo -> DebugPointAtInOrTo.Yes mIn
                | _ -> DebugPointAtInOrTo.No

            ValueSome(inp, v, body, genElemTy, mFor, mIn, spIn)

        // "for x in e -> e2" is converted to a call to Seq.map by the F# type checker. This could be removed, except it is also visible in F# quotations.
        | ValApp g g.seq_map_vref ([ _inpElemTy; genElemTy ], [ Expr.Lambda(_, _, _, [ v ], body, mIn, _); inp ], mFor) ->
            let spIn =
                match mIn.NotedSourceConstruct with
                | NotedSourceConstruct.InOrTo -> DebugPointAtInOrTo.Yes mIn
                | _ -> DebugPointAtInOrTo.No
            // The debug point mFor at the 'for' gets attached to the first argument, see TcSequenceExpression
            ValueSome(inp, v, mkCallSeqSingleton g body.Range genElemTy body, genElemTy, mFor, mIn, spIn)

        | _ -> ValueNone

    [<return: Struct>]
    let (|SeqDelay|_|) g expr =
        match expr with
        | ValApp g g.seq_delay_vref ([ elemTy ], [ Expr.Lambda(_, _, _, [ v ], e, _, _) ], _m) when not (isVarFreeInExpr v e) ->
            ValueSome(e, elemTy)
        | _ -> ValueNone

    [<return: Struct>]
    let (|SeqEmpty|_|) g expr =
        match expr with
        | ValApp g g.seq_empty_vref (_, [], m) -> ValueSome m
        | _ -> ValueNone
