// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.TypeHierarchy

open System
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Import
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TypedTreeOps.DebugPrint
open FSharp.Compiler.Xml

#if !NO_TYPEPROVIDERS
open FSharp.Compiler.TypeProviders
#endif

//-------------------------------------------------------------------------
// Fold the hierarchy.
//  REVIEW: this code generalizes the iteration used below for member lookup.
//-------------------------------------------------------------------------

/// Get the base type of a type, taking into account type instantiations. Return None if the
/// type has no base type.
let GetSuperTypeOfType g amap m ty =
#if !NO_TYPEPROVIDERS
    let ty =
        match tryTcrefOfAppTy g ty with
        | ValueSome tcref when tcref.IsProvided -> stripTyEqns g ty 
        | _ -> stripTyEqnsAndMeasureEqns g ty
#else
    let ty = stripTyEqnsAndMeasureEqns g ty
#endif

    match metadataOfTy g ty with
#if !NO_TYPEPROVIDERS
    | ProvidedTypeMetadata info ->
        let st = info.ProvidedType
        let superOpt = st.PApplyOption((fun st -> match st.BaseType with null -> None | t -> Some t), m)
        match superOpt with
        | None -> None
        | Some super -> Some(ImportProvidedType amap m super)
#endif
    | ILTypeMetadata (TILObjectReprData(scoref, _, tdef)) ->
        let tinst = argsOfAppTy g ty
        match tdef.Extends with
        | None -> None
        | Some ilTy -> Some (RescopeAndImportILType scoref amap m tinst ilTy)

    | FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata ->
        if isFSharpObjModelTy g ty || isFSharpExceptionTy g ty then
            let tcref = tcrefOfAppTy g ty
            Some (instType (mkInstForAppTy g ty) (superOfTycon g tcref.Deref))
        elif isArrayTy g ty then
            Some g.system_Array_ty
        elif isRefTy g ty && not (isObjTy g ty) then
            Some g.obj_ty
        elif isStructTupleTy g ty then
            Some g.system_Value_ty
        elif isFSharpStructOrEnumTy g ty then
            if isFSharpEnumTy g ty then
                Some g.system_Enum_ty
            else
                Some g.system_Value_ty
        elif isStructAnonRecdTy g ty then
            Some g.system_Value_ty
        elif isAnonRecdTy g ty then
            Some g.obj_ty
        elif isRecdTy g ty || isUnionTy g ty then
            Some g.obj_ty
        else
            None

/// Make a type for System.Collections.Generic.IList<ty>
let mkSystemCollectionsGenericIListTy (g: TcGlobals) ty =
    TType_app(g.tcref_System_Collections_Generic_IList, [ty], g.knownWithoutNull)

/// Indicates whether we can skip interface types that lie outside the reference set
[<RequireQualifiedAccess>]
type SkipUnrefInterfaces = Yes | No

let GetImmediateInterfacesOfMetadataType g amap m skipUnref ty (tcref: TyconRef) tinst =
    [
        match metadataOfTy g ty with
#if !NO_TYPEPROVIDERS
        | ProvidedTypeMetadata info ->
            for ity in info.ProvidedType.PApplyArray((fun st -> st.GetInterfaces()), "GetInterfaces", m) do
                ImportProvidedType amap m ity
#endif
        | ILTypeMetadata (TILObjectReprData(scoref, _, tdef)) ->
            // ImportILType may fail for an interface if the assembly load set is incomplete and the interface
            // comes from another assembly. In this case we simply skip the interface:
            // if we don't skip it, then compilation will just fail here, and if type checking
            // succeeds with fewer non-dereferencable interfaces reported then it would have
            // succeeded with more reported. There are pathological corner cases where this
            // doesn't apply: e.g. for mscorlib interfaces like IComparable, but we can always
            // assume those are present.
            for ity in tdef.Implements do
                if skipUnref = SkipUnrefInterfaces.No || CanRescopeAndImportILType scoref amap m ity then
                    RescopeAndImportILType scoref amap m tinst ity
        | FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata ->
            for ity in tcref.ImmediateInterfaceTypesOfFSharpTycon do
               instType (mkInstForAppTy g ty) ity ]

/// Collect the set of immediate declared interface types for an F# type, but do not
/// traverse the type hierarchy to collect further interfaces.
//
// NOTE: Anonymous record types are not directly considered to implement IComparable,
// IComparable<T> or IEquatable<T>. This is because whether they support these interfaces depend on their
// consitutent types, which may not yet be known in type inference.
let rec GetImmediateInterfacesOfType skipUnref g amap m ty =
    [
        match tryAppTy g ty with
        | ValueSome(tcref, tinst) ->
            // Check if this is a measure-annotated type
            match tcref.TypeReprInfo with
            | TMeasureableRepr reprTy ->
                yield! GetImmediateInterfacesOfMeasureAnnotatedType skipUnref g amap m ty reprTy
            | _ ->
                yield! GetImmediateInterfacesOfMetadataType g amap m skipUnref ty tcref tinst

        | ValueNone ->
            // For tuple types, func types, check if we can eliminate to a type with metadata.
            let tyWithMetadata = convertToTypeWithMetadataIfPossible g ty
            match tryAppTy g tyWithMetadata with
            | ValueSome (tcref, tinst) ->
                if isAnyTupleTy g ty then
                    yield! GetImmediateInterfacesOfMetadataType g amap m skipUnref tyWithMetadata tcref tinst
            | _ -> ()

        // .NET array types are considered to implement IList<T>
        if isArray1DTy g ty then
            mkSystemCollectionsGenericIListTy g (destArrayTy g ty)
    ]

// Report the interfaces supported by a measure-annotated type.
//
// For example, consider:
//
//     [<MeasureAnnotatedAbbreviation>]
//     type A<[<Measure>] 'm> = A
//
// This measure-annotated type is considered to support the interfaces on its representation type A,
// with the exception that
//
//   1. we rewrite the IComparable and IEquatable interfaces, so that
//    IComparable<A> --> IComparable<A<'m>>
//    IEquatable<A> --> IEquatable<A<'m>>
//
//   2. we emit any other interfaces that derive from IComparable and IEquatable interfaces
//
// This rule is conservative and only applies to IComparable and IEquatable interfaces.
//
// This rule may in future be extended to rewrite the "trait" interfaces associated with .NET 7.
and GetImmediateInterfacesOfMeasureAnnotatedType skipUnref g amap m ty reprTy =
    [
        // Report any interfaces that don't derive from IComparable<_> or IEquatable<_>
        for ity in GetImmediateInterfacesOfType skipUnref g amap m reprTy do
            if not (ExistsHeadTypeInInterfaceHierarchy g.system_GenericIComparable_tcref skipUnref g amap m ity) &&
               not (ExistsHeadTypeInInterfaceHierarchy g.system_GenericIEquatable_tcref skipUnref g amap m ity) then
                ity

        // NOTE: we should really only report the IComparable<A<'m>> interface for measure-annotated types
        // if the original type supports IComparable<A> somewhere in the hierarchy, likeiwse IEquatable<A<'m>>.
        //
        // However since F# 2.0 we have always reported these interfaces for all measure-annotated types.

        //if ExistsInInterfaceHierarchy (typeEquiv g (mkAppTy g.system_GenericIComparable_tcref [reprTy])) skipUnref g amap m ty then
        mkAppTy g.system_GenericIComparable_tcref [ty]

        //if ExistsInInterfaceHierarchy (typeEquiv g (mkAppTy g.system_GenericIEquatable_tcref [reprTy])) skipUnref g amap m ty then
        mkAppTy g.system_GenericIEquatable_tcref [ty]
    ]

// Check for IComparable<A>, IEquatable<A> and interfaces that derive from these
and ExistsHeadTypeInInterfaceHierarchy target skipUnref g amap m ity =
    ExistsInInterfaceHierarchy (function AppTy g (tcref,_) -> tyconRefEq g tcref target | _ -> false) skipUnref g amap m ity

// Check for IComparable<A>, IEquatable<A> and interfaces that derive from these
and ExistsInInterfaceHierarchy p skipUnref g amap m ity =
    match ity with
    | AppTy g (tcref, tinst) ->
        p ity ||
        (GetImmediateInterfacesOfMetadataType g amap m skipUnref ity tcref tinst 
         |> List.exists (ExistsInInterfaceHierarchy p skipUnref g amap m))
    | _ -> false

/// Indicates whether we should visit multiple instantiations of the same generic interface or not
[<RequireQualifiedAccess>]
type AllowMultiIntfInstantiations = Yes | No

/// Traverse the type hierarchy, e.g. f D (f C (f System.Object acc)).
/// Visit base types and interfaces first.
let private FoldHierarchyOfTypeAux followInterfaces allowMultiIntfInst skipUnref visitor g amap m ty acc =
    let rec loop ndeep ty (visitedTycon, visited: TyconRefMultiMap<_>, acc as state) =

        let seenThisTycon = 
            match tryTcrefOfAppTy g ty with
            | ValueSome tcref -> Set.contains tcref.Stamp visitedTycon
            | _ -> false

        // Do not visit the same type twice. Could only be doing this if we've seen this tycon
        if seenThisTycon && List.exists (typeEquiv g ty) (visited.Find (tcrefOfAppTy g ty)) then state else

        // Do not visit the same tycon twice, e.g. I<int> and I<string>, collect I<int> only, unless directed to allow this
        if seenThisTycon && allowMultiIntfInst = AllowMultiIntfInstantiations.No then state else

        let state =
            match tryTcrefOfAppTy g ty with
            | ValueSome tcref ->
                let visitedTycon = Set.add tcref.Stamp visitedTycon
                visitedTycon, visited.Add (tcref, ty), acc
            | _ ->
                state

        if ndeep > 100 then (errorR(Error((FSComp.SR.recursiveClassHierarchy (showType ty)), m)); (visitedTycon, visited, acc)) else
        let visitedTycon, visited, acc =
            if isInterfaceTy g ty then
                List.foldBack
                   (loop (ndeep+1))
                   (GetImmediateInterfacesOfType skipUnref g amap m ty)
                      (loop ndeep g.obj_ty state)
            else
                match tryDestTyparTy g ty with
                | ValueSome tp ->
                    let state = loop (ndeep+1) g.obj_ty state
                    List.foldBack
                        (fun x vacc ->
                          match x with
                          | TyparConstraint.MayResolveMember _
                          | TyparConstraint.DefaultsTo _
                          | TyparConstraint.SupportsComparison _
                          | TyparConstraint.SupportsEquality _
                          | TyparConstraint.IsEnum _
                          | TyparConstraint.IsDelegate _
                          | TyparConstraint.SupportsNull _
                          | TyparConstraint.IsNonNullableStruct _
                          | TyparConstraint.IsUnmanaged _
                          | TyparConstraint.IsReferenceType _
                          | TyparConstraint.SimpleChoice _
                          | TyparConstraint.RequiresDefaultConstructor _ -> vacc
                          | TyparConstraint.CoercesTo(cty, _) ->
                                  loop (ndeep + 1)  cty vacc)
                        tp.Constraints
                        state
                | _ ->
                    let state =
                        if followInterfaces then
                            List.foldBack
                              (loop (ndeep+1))
                              (GetImmediateInterfacesOfType skipUnref g amap m ty)
                              state
                        else
                            state
                    let state =
                        Option.foldBack
                          (loop (ndeep+1))
                          (GetSuperTypeOfType g amap m ty)
                          state
                    state
        let acc = visitor ty acc
        (visitedTycon, visited, acc)
    loop 0 ty (Set.empty, TyconRefMultiMap<_>.Empty, acc)  |> p33

/// Fold, do not follow interfaces (unless the type is itself an interface)
let FoldPrimaryHierarchyOfType f g amap m allowMultiIntfInst ty acc =
    FoldHierarchyOfTypeAux false allowMultiIntfInst SkipUnrefInterfaces.No f g amap m ty acc

/// Fold, following interfaces. Skipping interfaces that lie outside the referenced assembly set is allowed.
let FoldEntireHierarchyOfType f g amap m allowMultiIntfInst ty acc =
    FoldHierarchyOfTypeAux true allowMultiIntfInst SkipUnrefInterfaces.Yes f g amap m ty acc

/// Iterate, following interfaces. Skipping interfaces that lie outside the referenced assembly set is allowed.
let IterateEntireHierarchyOfType f g amap m allowMultiIntfInst ty =
    FoldHierarchyOfTypeAux true allowMultiIntfInst SkipUnrefInterfaces.Yes (fun ty () -> f ty) g amap m ty ()

/// Search for one element satisfying a predicate, following interfaces
let ExistsInEntireHierarchyOfType f g amap m allowMultiIntfInst ty =
    FoldHierarchyOfTypeAux true allowMultiIntfInst SkipUnrefInterfaces.Yes (fun ty acc -> acc || f ty ) g amap m ty false

/// Search for one element where a function returns a 'Some' result, following interfaces
let SearchEntireHierarchyOfType f g amap m ty =
    FoldHierarchyOfTypeAux true AllowMultiIntfInstantiations.Yes SkipUnrefInterfaces.Yes
        (fun ty acc ->
            match acc with
            | None -> if f ty then Some ty else None
            | Some _ -> acc)
        g amap m ty None

/// Get all super types of the type, including the type itself
let AllSuperTypesOfType g amap m allowMultiIntfInst ty =
    FoldHierarchyOfTypeAux true allowMultiIntfInst SkipUnrefInterfaces.No (ListSet.insert (typeEquiv g)) g amap m ty []

/// Get all interfaces of a type, including the type itself if it is an interface
let AllInterfacesOfType g amap m allowMultiIntfInst ty =
    AllSuperTypesOfType g amap m allowMultiIntfInst ty |> List.filter (isInterfaceTy g)

/// Check if two types have the same nominal head type
let HaveSameHeadType g ty1 ty2 =
    match tryTcrefOfAppTy g ty1 with
    | ValueSome tcref1 ->
        match tryTcrefOfAppTy g ty2 with
        | ValueSome tcref2 -> tyconRefEq g tcref1 tcref2
        | _ -> false
    | _ -> false

/// Check if a type has a particular head type
let HasHeadType g tcref ty2 =
    match tryTcrefOfAppTy g ty2 with
    | ValueSome tcref2 -> tyconRefEq g tcref tcref2
    | ValueNone -> false

/// Check if a type exists somewhere in the hierarchy which has the same head type as the given type (note, the given type need not have a head type at all)
let ExistsSameHeadTypeInHierarchy g amap m typeToSearchFrom typeToLookFor =
    ExistsInEntireHierarchyOfType (HaveSameHeadType g typeToLookFor)  g amap m AllowMultiIntfInstantiations.Yes typeToSearchFrom

/// Check if a type exists somewhere in the hierarchy which has the given head type.
let ExistsHeadTypeInEntireHierarchy g amap m typeToSearchFrom tcrefToLookFor =
    ExistsInEntireHierarchyOfType (HasHeadType g tcrefToLookFor) g amap m AllowMultiIntfInstantiations.Yes typeToSearchFrom

/// Read an Abstract IL type from metadata and convert to an F# type.
let ImportILTypeFromMetadata amap m scoref tinst minst ilTy =
    RescopeAndImportILType scoref amap m (tinst@minst) ilTy

/// Read an Abstract IL type from metadata, including any attributes that may affect the type itself, and convert to an F# type.
let ImportILTypeFromMetadataWithAttributes amap m scoref tinst minst ilTy getCattrs =
    let ty = RescopeAndImportILType scoref amap m (tinst@minst) ilTy
    // If the type is a byref and one of attributes from a return or parameter has IsReadOnly, then it's a inref.
    if isByrefTy amap.g ty && TryFindILAttribute amap.g.attrib_IsReadOnlyAttribute (getCattrs ()) then
        mkInByrefTy amap.g (destByrefTy amap.g ty)
    else
        ty

/// Get the parameter type of an IL method.
let ImportParameterTypeFromMetadata amap m ilTy getCattrs scoref tinst mist =
    ImportILTypeFromMetadataWithAttributes amap m scoref tinst mist ilTy getCattrs

/// Get the return type of an IL method, taking into account instantiations for type, return attributes and method generic parameters, and
/// translating 'void' to 'None'.
let ImportReturnTypeFromMetadata amap m ilTy getCattrs scoref tinst minst =
    match ilTy with
    | ILType.Void -> None
    | retTy -> Some(ImportILTypeFromMetadataWithAttributes amap m scoref tinst minst retTy getCattrs)


/// If you use a generic thing, then the extension members in scope at the point of _use_
/// are the ones available to solve the constraint
let FreshenTrait (traitCtxt: ITraitContext option) traitInfo =
    let (TTrait(typs, nm, mf, argtys, rty, slnCell, traitCtxtOld)) = traitInfo
    let traitCtxtNew = match traitCtxt with None -> traitCtxtOld | Some _ -> traitCtxt

    TTrait(typs, nm, mf, argtys, rty, slnCell, traitCtxtNew)

/// Copy constraints.  If the constraint comes from a type parameter associated
/// with a type constructor then we are simply renaming type variables.  If it comes
/// from a generic method in a generic class (e.g. ty.M<_>) then we may be both substituting the
/// instantiation associated with 'ty' as well as copying the type parameters associated with
/// M and instantiating their constraints
///
/// Note: this now looks identical to constraint instantiation.

let CopyTyparConstraints traitCtxt m tprefInst (tporig: Typar) =
    tporig.Constraints
    |>  List.map (fun tpc ->
           match tpc with
           | TyparConstraint.CoercesTo(ty, _) ->
               TyparConstraint.CoercesTo (instType tprefInst ty, m)
           | TyparConstraint.DefaultsTo(priority, ty, _) ->
               TyparConstraint.DefaultsTo (priority, instType tprefInst ty, m)
           | TyparConstraint.SupportsNull _ ->
               TyparConstraint.SupportsNull m
           | TyparConstraint.IsEnum (uty, _) ->
               TyparConstraint.IsEnum (instType tprefInst uty, m)
           | TyparConstraint.SupportsComparison _ ->
               TyparConstraint.SupportsComparison m
           | TyparConstraint.SupportsEquality _ ->
               TyparConstraint.SupportsEquality m
           | TyparConstraint.IsDelegate(aty, bty, _) ->
               TyparConstraint.IsDelegate (instType tprefInst aty, instType tprefInst bty, m)
           | TyparConstraint.IsNonNullableStruct _ ->
               TyparConstraint.IsNonNullableStruct m
           | TyparConstraint.IsUnmanaged _ ->
               TyparConstraint.IsUnmanaged m
           | TyparConstraint.IsReferenceType _ ->
               TyparConstraint.IsReferenceType m
           | TyparConstraint.SimpleChoice (tys, _) ->
               TyparConstraint.SimpleChoice (List.map (instType tprefInst) tys, m)
           | TyparConstraint.RequiresDefaultConstructor _ ->
               TyparConstraint.RequiresDefaultConstructor m
           | TyparConstraint.MayResolveMember(traitInfo, _) ->
               let traitInfo2 = FreshenTrait traitCtxt traitInfo 
               TyparConstraint.MayResolveMember (instTrait tprefInst traitInfo2, m))

/// The constraints for each typar copied from another typar can only be fixed up once
/// we have generated all the new constraints, e.g. f<A :> List<B>, B :> List<A>> ...
let FixupNewTypars traitCtxt m (formalEnclosingTypars: Typars) (tinst: TType list) (tpsorig: Typars) (tps: Typars) =
    // Checks.. These are defensive programming against early reported errors.
    let n0 = formalEnclosingTypars.Length
    let n1 = tinst.Length
    let n2 = tpsorig.Length
    let n3 = tps.Length
    if n0 <> n1 then error(Error((FSComp.SR.tcInvalidTypeArgumentCount(n0, n1)), m))
    if n2 <> n3 then error(Error((FSComp.SR.tcInvalidTypeArgumentCount(n2, n3)), m))

    // The real code..
    let renaming, tptys = mkTyparToTyparRenaming tpsorig tps
    let tprefInst = mkTyparInst formalEnclosingTypars tinst @ renaming
    (tpsorig, tps) ||> List.iter2 (fun tporig tp -> tp.SetConstraints (CopyTyparConstraints traitCtxt m tprefInst tporig))
    renaming, tptys

