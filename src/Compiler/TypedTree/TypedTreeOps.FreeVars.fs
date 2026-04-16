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
module internal FreeTypeVars =

    //---------------------------------------------------------------------------
    // Find all type variables in a type, apart from those that have had
    // an equation assigned by type inference.
    //---------------------------------------------------------------------------

    let emptyFreeLocals = Zset.empty valOrder

    let unionFreeLocals s1 s2 =
        if s1 === emptyFreeLocals then s2
        elif s2 === emptyFreeLocals then s1
        else Zset.union s1 s2

    let emptyFreeRecdFields = Zset.empty recdFieldRefOrder

    let unionFreeRecdFields s1 s2 =
        if s1 === emptyFreeRecdFields then s2
        elif s2 === emptyFreeRecdFields then s1
        else Zset.union s1 s2

    let emptyFreeUnionCases = Zset.empty unionCaseRefOrder

    let unionFreeUnionCases s1 s2 =
        if s1 === emptyFreeUnionCases then s2
        elif s2 === emptyFreeUnionCases then s1
        else Zset.union s1 s2

    let emptyFreeTycons = Zset.empty tyconOrder

    let unionFreeTycons s1 s2 =
        if s1 === emptyFreeTycons then s2
        elif s2 === emptyFreeTycons then s1
        else Zset.union s1 s2

    let typarOrder =
        { new IComparer<Typar> with
            member x.Compare(v1: Typar, v2: Typar) = compareBy v1 v2 _.Stamp
        }

    let emptyFreeTypars = Zset.empty typarOrder

    let unionFreeTypars s1 s2 =
        if s1 === emptyFreeTypars then s2
        elif s2 === emptyFreeTypars then s1
        else Zset.union s1 s2

    let emptyFreeTyvars =
        {
            FreeTycons = emptyFreeTycons
            // The summary of values used as trait solutions
            FreeTraitSolutions = emptyFreeLocals
            FreeTypars = emptyFreeTypars
        }

    let isEmptyFreeTyvars ftyvs =
        Zset.isEmpty ftyvs.FreeTypars && Zset.isEmpty ftyvs.FreeTycons

    let unionFreeTyvars fvs1 fvs2 =
        if fvs1 === emptyFreeTyvars then
            fvs2
        else if fvs2 === emptyFreeTyvars then
            fvs1
        else
            {
                FreeTycons = unionFreeTycons fvs1.FreeTycons fvs2.FreeTycons
                FreeTraitSolutions = unionFreeLocals fvs1.FreeTraitSolutions fvs2.FreeTraitSolutions
                FreeTypars = unionFreeTypars fvs1.FreeTypars fvs2.FreeTypars
            }

    type FreeVarOptions =
        {
            canCache: bool
            collectInTypes: bool
            includeLocalTycons: bool
            includeTypars: bool
            includeLocalTyconReprs: bool
            includeRecdFields: bool
            includeUnionCases: bool
            includeLocals: bool
            templateReplacement: ((TyconRef -> bool) * Typars) option
            stackGuard: StackGuard option
        }

        member this.WithTemplateReplacement(f, typars) =
            { this with
                templateReplacement = Some(f, typars)
            }

    let CollectAllNoCaching =
        {
            canCache = false
            collectInTypes = true
            includeLocalTycons = true
            includeLocalTyconReprs = true
            includeRecdFields = true
            includeUnionCases = true
            includeTypars = true
            includeLocals = true
            templateReplacement = None
            stackGuard = None
        }

    let CollectTyparsNoCaching =
        {
            canCache = false
            collectInTypes = true
            includeLocalTycons = false
            includeTypars = true
            includeLocalTyconReprs = false
            includeRecdFields = false
            includeUnionCases = false
            includeLocals = false
            templateReplacement = None
            stackGuard = None
        }

    let CollectLocalsNoCaching =
        {
            canCache = false
            collectInTypes = false
            includeLocalTycons = false
            includeTypars = false
            includeLocalTyconReprs = false
            includeRecdFields = false
            includeUnionCases = false
            includeLocals = true
            templateReplacement = None
            stackGuard = None
        }

    let CollectTyparsAndLocalsNoCaching =
        {
            canCache = false
            collectInTypes = true
            includeLocalTycons = false
            includeLocalTyconReprs = false
            includeRecdFields = false
            includeUnionCases = false
            includeTypars = true
            includeLocals = true
            templateReplacement = None
            stackGuard = None
        }

    let CollectAll =
        {
            canCache = false
            collectInTypes = true
            includeLocalTycons = true
            includeLocalTyconReprs = true
            includeRecdFields = true
            includeUnionCases = true
            includeTypars = true
            includeLocals = true
            templateReplacement = None
            stackGuard = None
        }

    let CollectTyparsAndLocalsImpl stackGuardOpt = // CollectAll
        {
            canCache = true // only cache for this one
            collectInTypes = true
            includeTypars = true
            includeLocals = true
            includeLocalTycons = false
            includeLocalTyconReprs = false
            includeRecdFields = false
            includeUnionCases = false
            templateReplacement = None
            stackGuard = stackGuardOpt
        }

    let CollectTyparsAndLocals = CollectTyparsAndLocalsImpl None

    let CollectTypars = CollectTyparsAndLocals

    let CollectLocals = CollectTyparsAndLocals

    let CollectTyparsAndLocalsWithStackGuard () =
        let stackGuard = StackGuard("AccFreeVarsStackGuardDepth")
        CollectTyparsAndLocalsImpl(Some stackGuard)

    let CollectLocalsWithStackGuard () = CollectTyparsAndLocalsWithStackGuard()

    let accFreeLocalTycon opts x acc =
        if not opts.includeLocalTycons then
            acc
        else if Zset.contains x acc.FreeTycons then
            acc
        else
            { acc with
                FreeTycons = Zset.add x acc.FreeTycons
            }

    let rec accFreeTycon opts (tcref: TyconRef) acc =
        let acc =
            match opts.templateReplacement with
            | Some(isTemplateTyconRef, cloFreeTyvars) when isTemplateTyconRef tcref ->
                let cloInst = List.map mkTyparTy cloFreeTyvars
                accFreeInTypes opts cloInst acc
            | _ -> acc

        if not opts.includeLocalTycons then
            acc
        elif tcref.IsLocalRef then
            accFreeLocalTycon opts tcref.ResolvedTarget acc
        else
            acc

    and boundTypars opts tps acc =
        // Bound type vars form a recursively-referential set due to constraints, e.g. A: I<B>, B: I<A>
        // So collect up free vars in all constraints first, then bind all variables
        let acc =
            List.foldBack (fun (tp: Typar) acc -> accFreeInTyparConstraints opts tp.Constraints acc) tps acc

        List.foldBack
            (fun tp acc ->
                { acc with
                    FreeTypars = Zset.remove tp acc.FreeTypars
                })
            tps
            acc

    and accFreeInTyparConstraints opts cxs acc =
        List.foldBack (accFreeInTyparConstraint opts) cxs acc

    and accFreeInTyparConstraint opts tpc acc =
        match tpc with
        | TyparConstraint.CoercesTo(ty, _) -> accFreeInType opts ty acc
        | TyparConstraint.MayResolveMember(traitInfo, _) -> accFreeInTrait opts traitInfo acc
        | TyparConstraint.DefaultsTo(_, defaultTy, _) -> accFreeInType opts defaultTy acc
        | TyparConstraint.SimpleChoice(tys, _) -> accFreeInTypes opts tys acc
        | TyparConstraint.IsEnum(underlyingTy, _) -> accFreeInType opts underlyingTy acc
        | TyparConstraint.IsDelegate(argTys, retTy, _) -> accFreeInType opts argTys (accFreeInType opts retTy acc)
        | TyparConstraint.SupportsComparison _
        | TyparConstraint.SupportsEquality _
        | TyparConstraint.SupportsNull _
        | TyparConstraint.NotSupportsNull _
        | TyparConstraint.IsNonNullableStruct _
        | TyparConstraint.IsReferenceType _
        | TyparConstraint.IsUnmanaged _
        | TyparConstraint.AllowsRefStruct _
        | TyparConstraint.RequiresDefaultConstructor _ -> acc

    and accFreeInTrait opts (TTrait(tys, _, _, argTys, retTy, _, sln)) acc =
        Option.foldBack
            (accFreeInTraitSln opts)
            sln.Value
            (accFreeInTypes opts tys (accFreeInTypes opts argTys (Option.foldBack (accFreeInType opts) retTy acc)))

    and accFreeInTraitSln opts sln acc =
        match sln with
        | ILMethSln(ty, _, _, minst, staticTyOpt) ->
            Option.foldBack (accFreeInType opts) staticTyOpt (accFreeInType opts ty (accFreeInTypes opts minst acc))
        | FSMethSln(ty, vref, minst, staticTyOpt) ->
            Option.foldBack
                (accFreeInType opts)
                staticTyOpt
                (accFreeInType opts ty (accFreeValRefInTraitSln opts vref (accFreeInTypes opts minst acc)))
        | FSAnonRecdFieldSln(_anonInfo, tinst, _n) -> accFreeInTypes opts tinst acc
        | FSRecdFieldSln(tinst, _rfref, _isSet) -> accFreeInTypes opts tinst acc
        | BuiltInSln -> acc
        | ClosedExprSln _ -> acc // nothing to accumulate because it's a closed expression referring only to erasure of provided method calls

    and accFreeLocalValInTraitSln _opts v fvs =
        if Zset.contains v fvs.FreeTraitSolutions then
            fvs
        else
            { fvs with
                FreeTraitSolutions = Zset.add v fvs.FreeTraitSolutions
            }

    and accFreeValRefInTraitSln opts (vref: ValRef) fvs =
        if vref.IsLocalRef then
            accFreeLocalValInTraitSln opts vref.ResolvedTarget fvs
        else
            // non-local values do not contain free variables
            fvs

    and accFreeTyparRef opts (tp: Typar) acc =
        if not opts.includeTypars then
            acc
        else if Zset.contains tp acc.FreeTypars then
            acc
        else
            accFreeInTyparConstraints
                opts
                tp.Constraints
                { acc with
                    FreeTypars = Zset.add tp acc.FreeTypars
                }

    and accFreeInType opts ty acc =
        match stripTyparEqns ty with
        | TType_tuple(tupInfo, l) -> accFreeInTypes opts l (accFreeInTupInfo opts tupInfo acc)

        | TType_anon(anonInfo, l) -> accFreeInTypes opts l (accFreeInTupInfo opts anonInfo.TupInfo acc)

        | TType_app(tcref, tinst, _) ->
            let acc = accFreeTycon opts tcref acc

            match tinst with
            | [] -> acc // optimization to avoid unneeded call
            | [ h ] -> accFreeInType opts h acc // optimization to avoid unneeded call
            | _ -> accFreeInTypes opts tinst acc

        | TType_ucase(UnionCaseRef(tcref, _), tinst) -> accFreeInTypes opts tinst (accFreeTycon opts tcref acc)

        | TType_fun(domainTy, rangeTy, _) -> accFreeInType opts domainTy (accFreeInType opts rangeTy acc)

        | TType_var(r, _) -> accFreeTyparRef opts r acc

        | TType_forall(tps, r) -> unionFreeTyvars (boundTypars opts tps (freeInType opts r)) acc

        | TType_measure unt -> accFreeInMeasure opts unt acc

    and accFreeInTupInfo _opts unt acc =
        match unt with
        | TupInfo.Const _ -> acc

    and accFreeInMeasure opts unt acc =
        List.foldBack (fun (tp, _) acc -> accFreeTyparRef opts tp acc) (ListMeasureVarOccsWithNonZeroExponents unt) acc

    and accFreeInTypes opts tys acc =
        match tys with
        | [] -> acc
        | h :: t -> accFreeInTypes opts t (accFreeInType opts h acc)

    and freeInType opts ty = accFreeInType opts ty emptyFreeTyvars

    and accFreeInVal opts (v: Val) acc = accFreeInType opts v.val_type acc

    let freeInTypes opts tys = accFreeInTypes opts tys emptyFreeTyvars
    let freeInVal opts v = accFreeInVal opts v emptyFreeTyvars

    let freeInTyparConstraints opts v =
        accFreeInTyparConstraints opts v emptyFreeTyvars

    let accFreeInTypars opts tps acc =
        List.foldBack (accFreeTyparRef opts) tps acc

    let rec addFreeInModuleTy (mtyp: ModuleOrNamespaceType) acc =
        QueueList.foldBack
            (typeOfVal >> accFreeInType CollectAllNoCaching)
            mtyp.AllValsAndMembers
            (QueueList.foldBack
                (fun (mspec: ModuleOrNamespace) acc -> addFreeInModuleTy mspec.ModuleOrNamespaceType acc)
                mtyp.AllEntities
                acc)

    let freeInModuleTy mtyp = addFreeInModuleTy mtyp emptyFreeTyvars

    //--------------------------------------------------------------------------
    // Free in type, left-to-right order preserved. This is used to determine the
    // order of type variables for top-level definitions based on their signature,
    // so be careful not to change the order. We accumulate in reverse
    // order.
    //--------------------------------------------------------------------------

    let emptyFreeTyparsLeftToRight = []

    let unionFreeTyparsLeftToRight fvs1 fvs2 =
        ListSet.unionFavourRight typarEq fvs1 fvs2

    let rec boundTyparsLeftToRight g cxFlag thruFlag acc tps =
        // Bound type vars form a recursively-referential set due to constraints, e.g. A: I<B>, B: I<A>
        // So collect up free vars in all constraints first, then bind all variables
        List.fold (fun acc (tp: Typar) -> accFreeInTyparConstraintsLeftToRight g cxFlag thruFlag acc tp.Constraints) tps acc

    and accFreeInTyparConstraintsLeftToRight g cxFlag thruFlag acc cxs =
        List.fold (accFreeInTyparConstraintLeftToRight g cxFlag thruFlag) acc cxs

    and accFreeInTyparConstraintLeftToRight g cxFlag thruFlag acc tpc =
        match tpc with
        | TyparConstraint.CoercesTo(ty, _) -> accFreeInTypeLeftToRight g cxFlag thruFlag acc ty
        | TyparConstraint.MayResolveMember(traitInfo, _) -> accFreeInTraitLeftToRight g cxFlag thruFlag acc traitInfo
        | TyparConstraint.DefaultsTo(_, defaultTy, _) -> accFreeInTypeLeftToRight g cxFlag thruFlag acc defaultTy
        | TyparConstraint.SimpleChoice(tys, _) -> accFreeInTypesLeftToRight g cxFlag thruFlag acc tys
        | TyparConstraint.IsEnum(underlyingTy, _) -> accFreeInTypeLeftToRight g cxFlag thruFlag acc underlyingTy
        | TyparConstraint.IsDelegate(argTys, retTy, _) ->
            accFreeInTypeLeftToRight g cxFlag thruFlag (accFreeInTypeLeftToRight g cxFlag thruFlag acc argTys) retTy
        | TyparConstraint.SupportsComparison _
        | TyparConstraint.SupportsEquality _
        | TyparConstraint.SupportsNull _
        | TyparConstraint.NotSupportsNull _
        | TyparConstraint.IsNonNullableStruct _
        | TyparConstraint.IsUnmanaged _
        | TyparConstraint.AllowsRefStruct _
        | TyparConstraint.IsReferenceType _
        | TyparConstraint.RequiresDefaultConstructor _ -> acc

    and accFreeInTraitLeftToRight g cxFlag thruFlag acc (TTrait(tys, _, _, argTys, retTy, _, _)) =
        let acc = accFreeInTypesLeftToRight g cxFlag thruFlag acc tys
        let acc = accFreeInTypesLeftToRight g cxFlag thruFlag acc argTys
        let acc = Option.fold (accFreeInTypeLeftToRight g cxFlag thruFlag) acc retTy
        acc

    and accFreeTyparRefLeftToRight g cxFlag thruFlag acc (tp: Typar) =
        if ListSet.contains typarEq tp acc then
            acc
        else
            let acc = ListSet.insert typarEq tp acc

            if cxFlag then
                accFreeInTyparConstraintsLeftToRight g cxFlag thruFlag acc tp.Constraints
            else
                acc

    and accFreeInTypeLeftToRight g cxFlag thruFlag acc ty =
        match (if thruFlag then stripTyEqns g ty else stripTyparEqns ty) with
        | TType_anon(anonInfo, anonTys) ->
            let acc = accFreeInTupInfoLeftToRight g cxFlag thruFlag acc anonInfo.TupInfo
            accFreeInTypesLeftToRight g cxFlag thruFlag acc anonTys

        | TType_tuple(tupInfo, tupTys) ->
            let acc = accFreeInTupInfoLeftToRight g cxFlag thruFlag acc tupInfo
            accFreeInTypesLeftToRight g cxFlag thruFlag acc tupTys

        | TType_app(_, tinst, _) -> accFreeInTypesLeftToRight g cxFlag thruFlag acc tinst

        | TType_ucase(_, tinst) -> accFreeInTypesLeftToRight g cxFlag thruFlag acc tinst

        | TType_fun(domainTy, rangeTy, _) ->
            let dacc = accFreeInTypeLeftToRight g cxFlag thruFlag acc domainTy
            accFreeInTypeLeftToRight g cxFlag thruFlag dacc rangeTy

        | TType_var(r, _) -> accFreeTyparRefLeftToRight g cxFlag thruFlag acc r

        | TType_forall(tps, r) ->
            let racc = accFreeInTypeLeftToRight g cxFlag thruFlag emptyFreeTyparsLeftToRight r
            unionFreeTyparsLeftToRight (boundTyparsLeftToRight g cxFlag thruFlag tps racc) acc

        | TType_measure unt ->
            let mvars = ListMeasureVarOccsWithNonZeroExponents unt
            List.foldBack (fun (tp, _) acc -> accFreeTyparRefLeftToRight g cxFlag thruFlag acc tp) mvars acc

    and accFreeInTupInfoLeftToRight _g _cxFlag _thruFlag acc unt =
        match unt with
        | TupInfo.Const _ -> acc

    and accFreeInTypesLeftToRight g cxFlag thruFlag acc tys =
        match tys with
        | [] -> acc
        | h :: t -> accFreeInTypesLeftToRight g cxFlag thruFlag (accFreeInTypeLeftToRight g cxFlag thruFlag acc h) t

    let freeInTypeLeftToRight g thruFlag ty =
        accFreeInTypeLeftToRight g true thruFlag emptyFreeTyparsLeftToRight ty
        |> List.rev

    let freeInTypesLeftToRight g thruFlag ty =
        accFreeInTypesLeftToRight g true thruFlag emptyFreeTyparsLeftToRight ty
        |> List.rev

    let freeInTypesLeftToRightSkippingConstraints g ty =
        accFreeInTypesLeftToRight g false true emptyFreeTyparsLeftToRight ty |> List.rev

[<AutoOpen>]
module internal MemberRepresentation =

    //--------------------------------------------------------------------------
    // Values representing member functions on F# types
    //--------------------------------------------------------------------------

    // Pull apart the type for an F# value that represents an object model method. Do not strip off a 'unit' argument.
    // Review: Should GetMemberTypeInFSharpForm have any other direct callers?
    let GetMemberTypeInFSharpForm g (memberFlags: SynMemberFlags) arities ty m =
        let tps, argInfos, retTy, retInfo = GetValReprTypeInFSharpForm g arities ty m

        let argInfos =
            if memberFlags.IsInstance then
                match argInfos with
                | [] ->
                    errorR (InternalError("value does not have a valid member type", m))
                    argInfos
                | _ :: t -> t
            else
                argInfos

        tps, argInfos, retTy, retInfo

    // Check that an F# value represents an object model method.
    // It will also always have an arity (inferred from syntax).
    let checkMemberVal membInfo arity m =
        match membInfo, arity with
        | None, _ -> error (InternalError("checkMemberVal - no membInfo", m))
        | _, None -> error (InternalError("checkMemberVal - no arity", m))
        | Some membInfo, Some arity -> (membInfo, arity)

    let checkMemberValRef (vref: ValRef) =
        checkMemberVal vref.MemberInfo vref.ValReprInfo vref.Range

    let GetFSharpViewOfReturnType (g: TcGlobals) retTy =
        match retTy with
        | None -> g.unit_ty
        | Some retTy -> retTy

    type TraitConstraintInfo with
        member traitInfo.GetReturnType(g: TcGlobals) =
            GetFSharpViewOfReturnType g traitInfo.CompiledReturnType

        member traitInfo.GetObjectType() =
            match traitInfo.MemberFlags.IsInstance, traitInfo.CompiledObjectAndArgumentTypes with
            | true, objTy :: _ -> Some objTy
            | _ -> None

        // For static property traits:
        //      ^T: (static member Zero: ^T)
        // The inner representation is
        //      TraitConstraintInfo([^T], get_Zero, Property, Static, [], ^T)
        // and this returns
        //      []
        //
        // For the logically equivalent static get_property traits (i.e. the property as a get_ method)
        //      ^T: (static member get_Zero: unit -> ^T)
        // The inner representation is
        //      TraitConstraintInfo([^T], get_Zero, Member, Static, [], ^T)
        // and this returns
        //      []
        //
        // For instance property traits
        //      ^T: (member Length: int)
        // The inner TraitConstraintInfo representation is
        //      TraitConstraintInfo([^T], get_Length, Property, Instance, [], int)
        // and this returns
        //      []
        //
        // For the logically equivalent instance get_property traits (i.e. the property as a get_ method)
        //      ^T: (member get_Length: unit -> int)
        // The inner TraitConstraintInfo representation is
        //      TraitConstraintInfo([^T], get_Length, Method, Instance, [^T], int)
        // and this returns
        //      []
        //
        // For index property traits
        //      ^T: (member Item: int -> int with get)
        // The inner TraitConstraintInfo representation is
        //      TraitConstraintInfo([^T], get_Item, Property, Instance, [^T; int], int)
        // and this returns
        //      [int]
        member traitInfo.GetCompiledArgumentTypes() =
            match traitInfo.MemberFlags.IsInstance, traitInfo.CompiledObjectAndArgumentTypes with
            | true, _ :: argTys -> argTys
            | _, argTys -> argTys

        // For static property traits:
        //      ^T: (static member Zero: ^T)
        // The inner representation is
        //      TraitConstraintInfo([^T], get_Zero, PropertyGet, Static, [], ^T)
        // and this returns
        //      []
        //
        // For the logically equivalent static get_property traits (i.e. the property as a get_ method)
        //      ^T: (static member get_Zero: unit -> ^T)
        // The inner representation is
        //      TraitConstraintInfo([^T], get_Zero, Member, Static, [], ^T)
        // and this returns
        //      [unit]
        //
        // For instance property traits
        //      ^T: (member Length: int)
        // The inner TraitConstraintInfo representation is
        //      TraitConstraintInfo([^T], get_Length, PropertyGet, Instance, [^T], int)
        // and this views the constraint as if it were
        //      []
        //
        // For the logically equivalent instance get_property traits (i.e. the property as a get_ method)
        //      ^T: (member get_Length: unit -> int)
        // The inner TraitConstraintInfo representation is
        //      TraitConstraintInfo([^T], get_Length, Member, Instance, [^T], int)
        // and this returns
        //      [unit]
        //
        // For index property traits
        //      (member Item: int -> int with get)
        // The inner TraitConstraintInfo representation is
        //      TraitConstraintInfo([^T], get_Item, PropertyGet, [^T; int], int)
        // and this returns
        //      [int]
        member traitInfo.GetLogicalArgumentTypes(g: TcGlobals) =
            match traitInfo.GetCompiledArgumentTypes(), traitInfo.MemberFlags.MemberKind with
            | [], SynMemberKind.Member -> [ g.unit_ty ]
            | argTys, _ -> argTys

        member traitInfo.MemberDisplayNameCore =
            let traitName0 = traitInfo.MemberLogicalName

            match traitInfo.MemberFlags.MemberKind with
            | SynMemberKind.PropertyGet
            | SynMemberKind.PropertySet ->
                match TryChopPropertyName traitName0 with
                | Some nm -> nm
                | None -> traitName0
            | _ -> traitName0

        /// Get the key associated with the member constraint.
        member traitInfo.GetWitnessInfo() =
            let (TTrait(tys, nm, memFlags, objAndArgTys, rty, _, _)) = traitInfo
            TraitWitnessInfo(tys, nm, memFlags, objAndArgTys, rty)

    /// Get information about the trait constraints for a set of typars.
    /// Put these in canonical order.
    let GetTraitConstraintInfosOfTypars g (tps: Typars) =
        [
            for tp in tps do
                for cx in tp.Constraints do
                    match cx with
                    | TyparConstraint.MayResolveMember(traitInfo, _) -> traitInfo
                    | _ -> ()
        ]
        |> ListSet.setify (traitsAEquiv g TypeEquivEnv.EmptyIgnoreNulls)
        |> List.sortBy (fun traitInfo -> traitInfo.MemberLogicalName, traitInfo.GetCompiledArgumentTypes().Length)

    /// Get information about the runtime witnesses needed for a set of generalized typars
    let GetTraitWitnessInfosOfTypars g numParentTypars typars =
        let typs = typars |> List.skip numParentTypars
        let cxs = GetTraitConstraintInfosOfTypars g typs
        cxs |> List.map (fun cx -> cx.GetWitnessInfo())

    /// Count the number of type parameters on the enclosing type
    let CountEnclosingTyparsOfActualParentOfVal (v: Val) =
        match v.ValReprInfo with
        | None -> 0
        | Some _ ->
            if v.IsExtensionMember then 0
            elif not v.IsMember then 0
            else v.MemberApparentEntity.TyparsNoRange.Length

    let GetValReprTypeInCompiledForm g valReprInfo numEnclosingTypars ty m =
        let tps, paramArgInfos, retTy, retInfo =
            GetValReprTypeInFSharpForm g valReprInfo ty m

        let witnessInfos = GetTraitWitnessInfosOfTypars g numEnclosingTypars tps
        // Eliminate lone single unit arguments
        let paramArgInfos =
            match paramArgInfos, valReprInfo.ArgInfos with
            // static member and module value unit argument elimination
            | [ [ (_argType, _) ] ], [ [] ] ->
                //assert isUnitTy g argType
                [ [] ]
            // instance member unit argument elimination
            | [ objInfo; [ (_argType, _) ] ], [ [ _objArg ]; [] ] ->
                //assert isUnitTy g argType
                [ objInfo; [] ]
            | _ -> paramArgInfos

        let retTy = if isUnitTy g retTy then None else Some retTy
        (tps, witnessInfos, paramArgInfos, retTy, retInfo)

    // Pull apart the type for an F# value that represents an object model method
    // and see the "member" form for the type, i.e.
    // detect methods with no arguments by (effectively) looking for single argument type of 'unit'.
    // The analysis is driven of the inferred arity information for the value.
    //
    // This is used not only for the compiled form - it's also used for all type checking and object model
    // logic such as determining if abstract methods have been implemented or not, and how
    // many arguments the method takes etc.
    let GetMemberTypeInMemberForm g memberFlags valReprInfo numEnclosingTypars ty m =
        let tps, paramArgInfos, retTy, retInfo =
            GetMemberTypeInFSharpForm g memberFlags valReprInfo ty m

        let witnessInfos = GetTraitWitnessInfosOfTypars g numEnclosingTypars tps
        // Eliminate lone single unit arguments
        let paramArgInfos =
            match paramArgInfos, valReprInfo.ArgInfos with
            // static member and module value unit argument elimination
            | [ [ (argTy, _) ] ], [ [] ] ->
                assert isUnitTy g argTy
                [ [] ]
            // instance member unit argument elimination
            | [ [ (argTy, _) ] ], [ [ _objArg ]; [] ] ->
                assert isUnitTy g argTy
                [ [] ]
            | _ -> paramArgInfos

        let retTy = if isUnitTy g retTy then None else Some retTy
        (tps, witnessInfos, paramArgInfos, retTy, retInfo)

    let GetTypeOfMemberInMemberForm g (vref: ValRef) =
        //assert (not vref.IsExtensionMember)
        let membInfo, valReprInfo = checkMemberValRef vref
        let numEnclosingTypars = CountEnclosingTyparsOfActualParentOfVal vref.Deref
        GetMemberTypeInMemberForm g membInfo.MemberFlags valReprInfo numEnclosingTypars vref.Type vref.Range

    let GetTypeOfMemberInFSharpForm g (vref: ValRef) =
        let membInfo, valReprInfo = checkMemberValRef vref
        GetMemberTypeInFSharpForm g membInfo.MemberFlags valReprInfo vref.Type vref.Range

    let PartitionValTyparsForApparentEnclosingType g (v: Val) =
        match v.ValReprInfo with
        | None -> error (InternalError("PartitionValTypars: not a top value", v.Range))
        | Some arities ->
            let fullTypars, _ = destTopForallTy g arities v.Type
            let parent = v.MemberApparentEntity
            let parentTypars = parent.TyparsNoRange
            let nparentTypars = parentTypars.Length

            if nparentTypars <= fullTypars.Length then
                let memberParentTypars, memberMethodTypars = List.splitAt nparentTypars fullTypars

                let memberToParentInst, tinst =
                    mkTyparToTyparRenaming memberParentTypars parentTypars

                Some(parentTypars, memberParentTypars, memberMethodTypars, memberToParentInst, tinst)
            else
                None

    /// Match up the type variables on an member value with the type
    /// variables on the apparent enclosing type
    let PartitionValTypars g (v: Val) =
        match v.ValReprInfo with
        | None -> error (InternalError("PartitionValTypars: not a top value", v.Range))
        | Some arities ->
            if v.IsExtensionMember then
                let fullTypars, _ = destTopForallTy g arities v.Type
                Some([], [], fullTypars, emptyTyparInst, [])
            else
                PartitionValTyparsForApparentEnclosingType g v

    let PartitionValRefTypars g (vref: ValRef) = PartitionValTypars g vref.Deref

    /// Get the arguments for an F# value that represents an object model method
    let ArgInfosOfMemberVal g (v: Val) =
        let membInfo, valReprInfo = checkMemberVal v.MemberInfo v.ValReprInfo v.Range
        let numEnclosingTypars = CountEnclosingTyparsOfActualParentOfVal v

        let _, _, arginfos, _, _ =
            GetMemberTypeInMemberForm g membInfo.MemberFlags valReprInfo numEnclosingTypars v.Type v.Range

        arginfos

    let ArgInfosOfMember g (vref: ValRef) = ArgInfosOfMemberVal g vref.Deref

    /// Get the property "type" (getter return type) for an F# value that represents a getter or setter
    /// of an object model property.
    let ReturnTypeOfPropertyVal g (v: Val) =
        let membInfo, valReprInfo = checkMemberVal v.MemberInfo v.ValReprInfo v.Range

        match membInfo.MemberFlags.MemberKind with
        | SynMemberKind.PropertySet ->
            let numEnclosingTypars = CountEnclosingTyparsOfActualParentOfVal v

            let _, _, arginfos, _, _ =
                GetMemberTypeInMemberForm g membInfo.MemberFlags valReprInfo numEnclosingTypars v.Type v.Range

            if not arginfos.IsEmpty && not arginfos.Head.IsEmpty then
                arginfos.Head |> List.last |> fst
            else
                error (Error(FSComp.SR.tastValueDoesNotHaveSetterType (), v.Range))
        | SynMemberKind.PropertyGet ->
            let numEnclosingTypars = CountEnclosingTyparsOfActualParentOfVal v

            let _, _, _, retTy, _ =
                GetMemberTypeInMemberForm g membInfo.MemberFlags valReprInfo numEnclosingTypars v.Type v.Range

            GetFSharpViewOfReturnType g retTy
        | _ -> error (InternalError("ReturnTypeOfPropertyVal", v.Range))

    /// Get the property arguments for an F# value that represents a getter or setter
    /// of an object model property.
    let ArgInfosOfPropertyVal g (v: Val) =
        let membInfo, valReprInfo = checkMemberVal v.MemberInfo v.ValReprInfo v.Range

        match membInfo.MemberFlags.MemberKind with
        | SynMemberKind.PropertyGet -> ArgInfosOfMemberVal g v |> List.concat
        | SynMemberKind.PropertySet ->
            let numEnclosingTypars = CountEnclosingTyparsOfActualParentOfVal v

            let _, _, arginfos, _, _ =
                GetMemberTypeInMemberForm g membInfo.MemberFlags valReprInfo numEnclosingTypars v.Type v.Range

            if not arginfos.IsEmpty && not arginfos.Head.IsEmpty then
                arginfos.Head |> List.frontAndBack |> fst
            else
                error (Error(FSComp.SR.tastValueDoesNotHaveSetterType (), v.Range))
        | _ -> error (InternalError("ArgInfosOfPropertyVal", v.Range))

    //---------------------------------------------------------------------------
    // Generalize type constructors to types
    //---------------------------------------------------------------------------

    let generalTyconRefInst (tcref: TyconRef) = generalizeTypars tcref.TyparsNoRange

    let generalizeTyconRef (g: TcGlobals) tcref =
        let tinst = generalTyconRefInst tcref
        tinst, TType_app(tcref, tinst, g.knownWithoutNull)

    let generalizedTyconRef (g: TcGlobals) tcref =
        let tinst = generalTyconRefInst tcref
        TType_app(tcref, tinst, g.knownWithoutNull)

    let isTTyparCoercesToType tpc =
        match tpc with
        | TyparConstraint.CoercesTo _ -> true
        | _ -> false

    //--------------------------------------------------------------------------
    // Print Signatures/Types - prelude
    //--------------------------------------------------------------------------

    let prefixOfStaticReq s =
        match s with
        | TyparStaticReq.None -> "'"
        | TyparStaticReq.HeadType -> "^"

    let prefixOfInferenceTypar (typar: Typar) =
        if typar.Rigidity <> TyparRigidity.Rigid then "_" else ""

    let isTyparOrderMismatch (tps: Typars) (argInfos: CurriedArgInfos) =
        let rec getTyparName (ty: TType) : string list =
            match ty with
            | TType_var(typar = tp) ->
                if tp.Id.idText <> unassignedTyparName then
                    [ tp.Id.idText ]
                else
                    match tp.Solution with
                    | None -> []
                    | Some solutionType -> getTyparName solutionType
            | TType_fun(domainType, rangeType, _) -> [ yield! getTyparName domainType; yield! getTyparName rangeType ]
            | TType_anon(tys = ti)
            | TType_app(typeInstantiation = ti)
            | TType_tuple(elementTypes = ti) -> List.collect getTyparName ti
            | _ -> []

        let typarNamesInArguments =
            argInfos
            |> List.collect (fun argInfos -> argInfos |> List.collect (fun (ty, _) -> getTyparName ty))
            |> List.distinct

        let typarNamesInDefinition =
            tps |> List.map (fun (tp: Typar) -> tp.Id.idText) |> List.distinct

        typarNamesInArguments.Length = typarNamesInDefinition.Length
        && typarNamesInArguments <> typarNamesInDefinition

    //---------------------------------------------------------------------------
    // Prettify: PrettyTyparNames/PrettifyTypes - make typar names human friendly
    //---------------------------------------------------------------------------

    type TyparConstraintsWithTypars = (Typar * TyparConstraint) list

    module PrettyTypes =
        let newPrettyTypar (tp: Typar) nm =
            Construct.NewTypar(
                tp.Kind,
                tp.Rigidity,
                SynTypar(ident (nm, tp.Range), tp.StaticReq, false),
                false,
                TyparDynamicReq.Yes,
                [],
                false,
                false
            )

        let NewPrettyTypars renaming tps names =
            let niceTypars = List.map2 newPrettyTypar tps names
            let tl, _tt = mkTyparToTyparRenaming tps niceTypars in
            let renaming = renaming @ tl

            (tps, niceTypars)
            ||> List.iter2 (fun tp tpnice -> tpnice.SetConstraints(instTyparConstraints renaming tp.Constraints))

            niceTypars, renaming

        // We choose names for type parameters from 'a'..'t'
        // We choose names for unit-of-measure from 'u'..'z'
        // If we run off the end of these ranges, we use 'aX' for positive integer X or 'uX' for positive integer X
        // Finally, we skip any names already in use
        let NeedsPrettyTyparName (tp: Typar) =
            tp.IsCompilerGenerated
            && tp.ILName.IsNone
            && (tp.typar_id.idText = unassignedTyparName)

        let PrettyTyparNames pred alreadyInUse tps =
            let rec choose (tps: Typar list) (typeIndex, measureIndex) acc =
                match tps with
                | [] -> List.rev acc
                | tp :: tps ->

                    // Use a particular name, possibly after incrementing indexes
                    let useThisName (nm, typeIndex, measureIndex) =
                        choose tps (typeIndex, measureIndex) (nm :: acc)

                    // Give up, try again with incremented indexes
                    let tryAgain (typeIndex, measureIndex) =
                        choose (tp :: tps) (typeIndex, measureIndex) acc

                    let tryName (nm, typeIndex, measureIndex) f =
                        if List.contains nm alreadyInUse then
                            f ()
                        else
                            useThisName (nm, typeIndex, measureIndex)

                    if pred tp then
                        if NeedsPrettyTyparName tp then
                            let typeIndex, measureIndex, baseName, letters, i =
                                match tp.Kind with
                                | TyparKind.Type -> (typeIndex + 1, measureIndex, 'a', 20, typeIndex)
                                | TyparKind.Measure -> (typeIndex, measureIndex + 1, 'u', 6, measureIndex)

                            let nm =
                                if i < letters then
                                    String.make 1 (char (int baseName + i))
                                else
                                    String.make 1 baseName + string (i - letters + 1)

                            tryName (nm, typeIndex, measureIndex) (fun () -> tryAgain (typeIndex, measureIndex))

                        else
                            tryName (tp.Name, typeIndex, measureIndex) (fun () ->
                                // Use the next index and append it to the natural name
                                let typeIndex, measureIndex, nm =
                                    match tp.Kind with
                                    | TyparKind.Type -> (typeIndex + 1, measureIndex, tp.Name + string typeIndex)
                                    | TyparKind.Measure -> (typeIndex, measureIndex + 1, tp.Name + string measureIndex)

                                tryName (nm, typeIndex, measureIndex) (fun () -> tryAgain (typeIndex, measureIndex)))
                    else
                        useThisName (tp.Name, typeIndex, measureIndex)

            choose tps (0, 0) []

        let AssignPrettyTyparNames typars prettyNames =
            (typars, prettyNames)
            ||> List.iter2 (fun tp nm ->
                if NeedsPrettyTyparName tp then
                    tp.typar_id <- ident (nm, tp.Range))

        let PrettifyThings g foldTys mapTys things =
            let ftps =
                foldTys (accFreeInTypeLeftToRight g true false) emptyFreeTyparsLeftToRight things

            let ftps = List.rev ftps

            let rec computeKeep (keep: Typars) change (tps: Typars) =
                match tps with
                | [] -> List.rev keep, List.rev change
                | tp :: rest ->
                    if
                        not (NeedsPrettyTyparName tp)
                        && (not (keep |> List.exists (fun tp2 -> tp.Name = tp2.Name)))
                    then
                        computeKeep (tp :: keep) change rest
                    else
                        computeKeep keep (tp :: change) rest

            let keep, change = computeKeep [] [] ftps

            let alreadyInUse = keep |> List.map (fun x -> x.Name)
            let names = PrettyTyparNames (fun x -> List.memq x change) alreadyInUse ftps

            let niceTypars, renaming = NewPrettyTypars [] ftps names

            // strip universal types for printing
            let getTauStayTau ty =
                match ty with
                | TType_forall(_, tau) -> tau
                | _ -> ty

            let tauThings = mapTys getTauStayTau things

            let prettyThings = mapTys (instType renaming) tauThings

            let tpconstraints =
                niceTypars
                |> List.collect (fun tpnice -> List.map (fun tpc -> tpnice, tpc) tpnice.Constraints)

            prettyThings, tpconstraints

        let PrettifyType g x = PrettifyThings g id id x

        let PrettifyTypePair g x =
            PrettifyThings g (fun f -> foldPair (f, f)) (fun f -> mapPair (f, f)) x

        let PrettifyTypes g x = PrettifyThings g List.fold List.map x

        let PrettifyDiscriminantAndTypePairs g x =
            let tys, cxs = (PrettifyThings g List.fold List.map (x |> List.map snd))
            List.zip (List.map fst x) tys, cxs

        let PrettifyCurriedTypes g x =
            PrettifyThings g (List.fold >> List.fold) List.mapSquared x

        let PrettifyCurriedSigTypes g x =
            PrettifyThings g (fun f -> foldPair (List.fold (List.fold f), f)) (fun f -> mapPair (List.mapSquared f, f)) x

        // Badly formed code may instantiate rigid declared typars to types.
        // Hence we double check here that the thing is really a type variable
        let safeDestAnyParTy orig g ty =
            match tryAnyParTy g ty with
            | ValueNone -> orig
            | ValueSome x -> x

        let foldUncurriedArgInfos f z (x: UncurriedArgInfos) = List.fold (fold1Of2 f) z x
        let foldTypar f z (x: Typar) = foldOn mkTyparTy f z x

        let mapTypar g f (x: Typar) : Typar =
            (mkTyparTy >> f >> safeDestAnyParTy x g) x

        let foldTypars f z (x: Typars) = List.fold (foldTypar f) z x
        let mapTypars g f (x: Typars) : Typars = List.map (mapTypar g f) x

        let foldTyparInst f z (x: TyparInstantiation) =
            List.fold (foldPair (foldTypar f, f)) z x

        let mapTyparInst g f (x: TyparInstantiation) : TyparInstantiation = List.map (mapPair (mapTypar g f, f)) x

        let PrettifyInstAndTyparsAndType g x =
            PrettifyThings
                g
                (fun f -> foldTriple (foldTyparInst f, foldTypars f, f))
                (fun f -> mapTriple (mapTyparInst g f, mapTypars g f, f))
                x

        let PrettifyInstAndUncurriedSig g (x: TyparInstantiation * UncurriedArgInfos * TType) =
            PrettifyThings
                g
                (fun f -> foldTriple (foldTyparInst f, foldUncurriedArgInfos f, f))
                (fun f -> mapTriple (mapTyparInst g f, List.map (map1Of2 f), f))
                x

        let PrettifyInstAndCurriedSig g (x: TyparInstantiation * TTypes * CurriedArgInfos * TType) =
            PrettifyThings
                g
                (fun f -> foldQuadruple (foldTyparInst f, List.fold f, List.fold (List.fold (fold1Of2 f)), f))
                (fun f -> mapQuadruple (mapTyparInst g f, List.map f, List.mapSquared (map1Of2 f), f))
                x

        let PrettifyInstAndSig g x =
            PrettifyThings
                g
                (fun f -> foldTriple (foldTyparInst f, List.fold f, f))
                (fun f -> mapTriple (mapTyparInst g f, List.map f, f))
                x

        let PrettifyInstAndTypes g x =
            PrettifyThings g (fun f -> foldPair (foldTyparInst f, List.fold f)) (fun f -> mapPair (mapTyparInst g f, List.map f)) x

        let PrettifyInstAndType g x =
            PrettifyThings g (fun f -> foldPair (foldTyparInst f, f)) (fun f -> mapPair (mapTyparInst g f, f)) x

        let PrettifyInst g x =
            PrettifyThings g foldTyparInst (fun f -> mapTyparInst g f) x

    module SimplifyTypes =

        // CAREFUL! This function does NOT walk constraints
        let rec foldTypeButNotConstraints f z ty =
            let ty = stripTyparEqns ty
            let z = f z ty

            match ty with
            | TType_forall(_, bodyTy) -> foldTypeButNotConstraints f z bodyTy

            | TType_app(_, tys, _)
            | TType_ucase(_, tys)
            | TType_anon(_, tys)
            | TType_tuple(_, tys) -> List.fold (foldTypeButNotConstraints f) z tys

            | TType_fun(domainTy, rangeTy, _) -> foldTypeButNotConstraints f (foldTypeButNotConstraints f z domainTy) rangeTy

            | TType_var _ -> z

            | TType_measure _ -> z

        let incM x m =
            if Zmap.mem x m then
                Zmap.add x (1 + Zmap.find x m) m
            else
                Zmap.add x 1 m

        let accTyparCounts z ty =
            // Walk type to determine typars and their counts (for pprinting decisions)
            (z, ty)
            ||> foldTypeButNotConstraints (fun z ty ->
                match ty with
                | TType_var(tp, _) when tp.Rigidity = TyparRigidity.Rigid -> incM tp z
                | _ -> z)

        let emptyTyparCounts = Zmap.empty typarOrder

        // print multiple fragments of the same type using consistent naming and formatting
        let accTyparCountsMulti acc l = List.fold accTyparCounts acc l

        type TypeSimplificationInfo =
            {
                singletons: Typar Zset
                inplaceConstraints: Zmap<Typar, TType>
                postfixConstraints: (Typar * TyparConstraint) list
            }

        let typeSimplificationInfo0 =
            {
                singletons = Zset.empty typarOrder
                inplaceConstraints = Zmap.empty typarOrder
                postfixConstraints = []
            }

        let categorizeConstraints simplify m cxs =
            let singletons =
                if simplify then
                    Zmap.chooseL (fun tp n -> if n = 1 then Some tp else None) m
                else
                    []

            let singletons = Zset.addList singletons (Zset.empty typarOrder)
            // Here, singletons are typars that occur once in the type.
            // However, they may also occur in a type constraint.
            // If they do, they are really multiple occurrence - so we should remove them.
            let constraintTypars =
                (freeInTyparConstraints CollectTyparsNoCaching (List.map snd cxs)).FreeTypars

            let usedInTypeConstraint typar = Zset.contains typar constraintTypars
            let singletons = singletons |> Zset.filter (usedInTypeConstraint >> not)
            // Here, singletons should really be used once
            let inplace, postfix =
                cxs
                |> List.partition (fun (tp, tpc) ->
                    simplify
                    && isTTyparCoercesToType tpc
                    && Zset.contains tp singletons
                    && List.isSingleton tp.Constraints)

            let inplace =
                inplace
                |> List.map (function
                    | tp, TyparConstraint.CoercesTo(ty, _) -> tp, ty
                    | _ -> failwith "not isTTyparCoercesToType")

            {
                singletons = singletons
                inplaceConstraints = Zmap.ofList typarOrder inplace
                postfixConstraints = postfix
            }

        let CollectInfo simplify tys cxs =
            categorizeConstraints simplify (accTyparCountsMulti emptyTyparCounts tys) cxs

    //--------------------------------------------------------------------------
    // Print Signatures/Types
    //--------------------------------------------------------------------------

    type GenericParameterStyle =
        | Implicit
        | Prefix
        | Suffix
        | TopLevelPrefix of nested: GenericParameterStyle

    [<NoEquality; NoComparison>]
    type DisplayEnv =
        {
            includeStaticParametersInTypeNames: bool
            openTopPathsSorted: InterruptibleLazy<string list list>
            openTopPathsRaw: string list list
            shortTypeNames: bool
            suppressNestedTypes: bool
            maxMembers: int option
            showObsoleteMembers: bool
            showHiddenMembers: bool
            showTyparBinding: bool
            showInferenceTyparAnnotations: bool
            suppressInlineKeyword: bool
            suppressMutableKeyword: bool
            showMemberContainers: bool
            shortConstraints: bool
            useColonForReturnType: bool
            showAttributes: bool
            showCsharpCodeAnalysisAttributes: bool
            showOverrides: bool
            showStaticallyResolvedTyparAnnotations: bool
            showNullnessAnnotations: bool option
            abbreviateAdditionalConstraints: bool
            showTyparDefaultConstraints: bool
            showDocumentation: bool
            shrinkOverloads: bool
            printVerboseSignatures: bool
            escapeKeywordNames: bool
            g: TcGlobals
            contextAccessibility: Accessibility
            generatedValueLayout: Val -> Layout option
            genericParameterStyle: GenericParameterStyle
        }

        member x.SetOpenPaths paths =
            { x with
                openTopPathsSorted = InterruptibleLazy(fun _ -> paths |> List.sortWith (fun p1 p2 -> -(compare p1 p2)))
                openTopPathsRaw = paths
            }

        static member Empty tcGlobals =
            {
                includeStaticParametersInTypeNames = false
                openTopPathsRaw = []
                openTopPathsSorted = notlazy []
                shortTypeNames = false
                suppressNestedTypes = false
                maxMembers = None
                showObsoleteMembers = false
                showHiddenMembers = false
                showTyparBinding = false
                showInferenceTyparAnnotations = false
                suppressInlineKeyword = true
                suppressMutableKeyword = false
                showMemberContainers = false
                showAttributes = false
                showCsharpCodeAnalysisAttributes = false
                showOverrides = true
                showStaticallyResolvedTyparAnnotations = true
                showNullnessAnnotations = None
                showDocumentation = false
                abbreviateAdditionalConstraints = false
                showTyparDefaultConstraints = false
                shortConstraints = false
                useColonForReturnType = false
                shrinkOverloads = true
                printVerboseSignatures = false
                escapeKeywordNames = false
                g = tcGlobals
                contextAccessibility = taccessPublic
                generatedValueLayout = (fun _ -> None)
                genericParameterStyle = GenericParameterStyle.Implicit
            }

        member denv.AddOpenPath path =
            denv.SetOpenPaths(path :: denv.openTopPathsRaw)

        member denv.AddOpenModuleOrNamespace(modref: ModuleOrNamespaceRef) =
            denv.AddOpenPath (fullCompPathOfModuleOrNamespace modref.Deref).DemangledPath

        member denv.AddAccessibility access =
            { denv with
                contextAccessibility = combineAccess denv.contextAccessibility access
            }

        member denv.UseGenericParameterStyle style =
            { denv with
                genericParameterStyle = style
            }

        member denv.UseTopLevelPrefixGenericParameterStyle() =
            let nestedStyle =
                match denv.genericParameterStyle with
                | TopLevelPrefix(nested) -> nested
                | style -> style

            { denv with
                genericParameterStyle = TopLevelPrefix(nestedStyle)
            }

        static member InitialForSigFileGeneration g =
            let denv =
                { DisplayEnv.Empty g with
                    showInferenceTyparAnnotations = true
                    showHiddenMembers = true
                    showObsoleteMembers = true
                    showAttributes = true
                    suppressInlineKeyword = false
                    showDocumentation = true
                    shrinkOverloads = false
                    escapeKeywordNames = true
                    includeStaticParametersInTypeNames = true
                }

            denv.SetOpenPaths
                [
                    RootPath
                    CorePath
                    CollectionsPath
                    ControlPath
                    (splitNamespace ExtraTopLevelOperatorsName)
                ]

    let (+.+) s1 s2 =
        if String.IsNullOrEmpty(s1) then s2 else !!s1 + "." + s2

    let layoutOfPath p =
        sepListL SepL.dot (List.map (tagNamespace >> wordL) p)

    let fullNameOfParentOfPubPath pp =
        match pp with
        | PubPath([| _ |]) -> ValueNone
        | pp -> ValueSome(textOfPath pp.EnclosingPath)

    let fullNameOfParentOfPubPathAsLayout pp =
        match pp with
        | PubPath([| _ |]) -> ValueNone
        | pp -> ValueSome(layoutOfPath (Array.toList pp.EnclosingPath))

    let fullNameOfPubPath (PubPath p) = textOfPath p
    let fullNameOfPubPathAsLayout (PubPath p) = layoutOfPath (Array.toList p)

    let fullNameOfParentOfNonLocalEntityRef (nlr: NonLocalEntityRef) =
        if nlr.Path.Length < 2 then
            ValueNone
        else
            ValueSome(textOfPath nlr.EnclosingMangledPath)

    let fullNameOfParentOfNonLocalEntityRefAsLayout (nlr: NonLocalEntityRef) =
        if nlr.Path.Length < 2 then
            ValueNone
        else
            ValueSome(layoutOfPath (List.ofArray nlr.EnclosingMangledPath))

    let fullNameOfParentOfEntityRef eref =
        match eref with
        | ERefLocal x ->
            match x.PublicPath with
            | None -> ValueNone
            | Some ppath -> fullNameOfParentOfPubPath ppath
        | ERefNonLocal nlr -> fullNameOfParentOfNonLocalEntityRef nlr

    let fullNameOfParentOfEntityRefAsLayout eref =
        match eref with
        | ERefLocal x ->
            match x.PublicPath with
            | None -> ValueNone
            | Some ppath -> fullNameOfParentOfPubPathAsLayout ppath
        | ERefNonLocal nlr -> fullNameOfParentOfNonLocalEntityRefAsLayout nlr

    let fullNameOfEntityRef nmF xref =
        match fullNameOfParentOfEntityRef xref with
        | ValueNone -> nmF xref
        | ValueSome pathText -> pathText +.+ nmF xref

    let tagEntityRefName (xref: EntityRef) name =
        if xref.IsNamespace then
            tagNamespace name
        elif xref.IsModule then
            tagModule name
        elif xref.IsTypeAbbrev then
            tagAlias name
        elif xref.IsFSharpDelegateTycon then
            tagDelegate name
        elif xref.IsILEnumTycon || xref.IsFSharpEnumTycon then
            tagEnum name
        elif xref.IsStructOrEnumTycon then
            tagStruct name
        elif isInterfaceTyconRef xref then
            tagInterface name
        elif xref.IsUnionTycon then
            tagUnion name
        elif xref.IsRecordTycon then
            tagRecord name
        else
            tagClass name

    let fullDisplayTextOfTyconRef (tcref: TyconRef) =
        fullNameOfEntityRef (fun tcref -> tcref.DisplayNameWithStaticParametersAndUnderscoreTypars) tcref

    let fullNameOfEntityRefAsLayout nmF (xref: EntityRef) =
        let navigableText =
            tagEntityRefName xref (nmF xref) |> mkNav xref.DefinitionRange |> wordL

        match fullNameOfParentOfEntityRefAsLayout xref with
        | ValueNone -> navigableText
        | ValueSome pathText -> pathText ^^ SepL.dot ^^ navigableText

    let fullNameOfParentOfValRef vref =
        match vref with
        | VRefLocal x ->
            match x.PublicPath with
            | None -> ValueNone
            | Some(ValPubPath(pp, _)) -> ValueSome(fullNameOfPubPath pp)
        | VRefNonLocal nlr -> ValueSome(fullNameOfEntityRef (fun (x: EntityRef) -> x.DemangledModuleOrNamespaceName) nlr.EnclosingEntity)

    let fullNameOfParentOfValRefAsLayout vref =
        match vref with
        | VRefLocal x ->
            match x.PublicPath with
            | None -> ValueNone
            | Some(ValPubPath(pp, _)) -> ValueSome(fullNameOfPubPathAsLayout pp)
        | VRefNonLocal nlr ->
            ValueSome(fullNameOfEntityRefAsLayout (fun (x: EntityRef) -> x.DemangledModuleOrNamespaceName) nlr.EnclosingEntity)

    let fullDisplayTextOfParentOfModRef eref = fullNameOfParentOfEntityRef eref

    let fullDisplayTextOfModRef r =
        fullNameOfEntityRef (fun eref -> eref.DemangledModuleOrNamespaceName) r

    let fullDisplayTextOfTyconRefAsLayout tcref =
        fullNameOfEntityRefAsLayout (fun tcref -> tcref.DisplayNameWithStaticParametersAndUnderscoreTypars) tcref

    let fullDisplayTextOfExnRef tcref =
        fullNameOfEntityRef (fun tcref -> tcref.DisplayNameWithStaticParametersAndUnderscoreTypars) tcref

    let fullDisplayTextOfExnRefAsLayout tcref =
        fullNameOfEntityRefAsLayout (fun tcref -> tcref.DisplayNameWithStaticParametersAndUnderscoreTypars) tcref

    let fullDisplayTextOfUnionCaseRef (ucref: UnionCaseRef) =
        fullDisplayTextOfTyconRef ucref.TyconRef +.+ ucref.CaseName

    let fullDisplayTextOfRecdFieldRef (rfref: RecdFieldRef) =
        fullDisplayTextOfTyconRef rfref.TyconRef +.+ rfref.FieldName

    let fullDisplayTextOfValRef (vref: ValRef) =
        match fullNameOfParentOfValRef vref with
        | ValueNone -> vref.DisplayName
        | ValueSome pathText -> pathText +.+ vref.DisplayName

    let fullDisplayTextOfValRefAsLayout (vref: ValRef) =
        let n =
            match vref.MemberInfo with
            | None ->
                if vref.IsModuleBinding then
                    tagModuleBinding vref.DisplayName
                else
                    tagUnknownEntity vref.DisplayName
            | Some memberInfo ->
                match memberInfo.MemberFlags.MemberKind with
                | SynMemberKind.PropertyGet
                | SynMemberKind.PropertySet
                | SynMemberKind.PropertyGetSet -> tagProperty vref.DisplayName
                | SynMemberKind.ClassConstructor
                | SynMemberKind.Constructor -> tagMethod vref.DisplayName
                | SynMemberKind.Member -> tagMember vref.DisplayName

        match fullNameOfParentOfValRefAsLayout vref with
        | ValueNone -> wordL n
        | ValueSome pathText -> pathText ^^ SepL.dot ^^ wordL n
    //pathText +.+ vref.DisplayName

    let fullMangledPathToTyconRef (tcref: TyconRef) =
        match tcref with
        | ERefLocal _ ->
            (match tcref.PublicPath with
             | None -> [||]
             | Some pp -> pp.EnclosingPath)
        | ERefNonLocal nlr -> nlr.EnclosingMangledPath

    /// generates a name like 'System.IComparable<System.Int32>.Get'
    let tyconRefToFullName (tcref: TyconRef) =
        let namespaceParts =
            // we need to ensure there are no collisions between (for example)
            // - ``IB<GlobalType>`` (non-generic)
            // - IB<'T> instantiated with 'T = GlobalType
            // This is only an issue for types inside the global namespace, because '.' is invalid even in a quoted identifier.
            // So if the type is in the global namespace, prepend 'global`', because '`' is also illegal -> there can be no quoted identifer with that name.
            match fullMangledPathToTyconRef tcref with
            | [||] -> [| "global`" |]
            | ns -> ns

        seq {
            yield! namespaceParts
            yield tcref.DisplayName
        }
        |> String.concat "."

    let rec qualifiedInterfaceImplementationNameAux g (x: TType) : string =
        match stripMeasuresFromTy g (stripTyEqnsAndErase true g x) with
        | TType_app(a, [], _) -> tyconRefToFullName a

        | TType_anon(a, b) ->
            let genericParameters =
                b |> Seq.map (qualifiedInterfaceImplementationNameAux g) |> String.concat ", "

            sprintf "%s<%s>" a.ILTypeRef.FullName genericParameters

        | TType_app(a, b, _) ->
            let genericParameters =
                b |> Seq.map (qualifiedInterfaceImplementationNameAux g) |> String.concat ", "

            sprintf "%s<%s>" (tyconRefToFullName a) genericParameters

        | TType_var(v, _) -> "'" + v.Name

        | _ -> failwithf "unexpected: expected TType_app but got %O" (x.GetType())

    /// for types in the global namespace, `global is prepended (note the backtick)
    let qualifiedInterfaceImplementationName g (ty: TType) memberName =
        let interfaceName = ty |> qualifiedInterfaceImplementationNameAux g
        sprintf "%s.%s" interfaceName memberName

    let qualifiedMangledNameOfTyconRef tcref nm =
        String.concat
            "-"
            (Array.toList (fullMangledPathToTyconRef tcref)
             @ [ tcref.LogicalName + "-" + nm ])

    let rec firstEq p1 p2 =
        match p1 with
        | [] -> true
        | h1 :: t1 ->
            match p2 with
            | h2 :: t2 -> h1 = h2 && firstEq t1 t2
            | _ -> false

    let rec firstRem p1 p2 =
        match p1 with
        | [] -> p2
        | _ :: t1 -> firstRem t1 (List.tail p2)

    let trimPathByDisplayEnv denv path =
        let findOpenedNamespace openedPath =
            if firstEq openedPath path then
                let t2 = firstRem openedPath path
                if t2 <> [] then Some(textOfPath t2 + ".") else Some("")
            else
                None

        match List.tryPick findOpenedNamespace (denv.openTopPathsSorted.Force()) with
        | Some s -> s
        | None -> if isNil path then "" else textOfPath path + "."

    let superOfTycon (g: TcGlobals) (tycon: Tycon) =
        match tycon.TypeContents.tcaug_super with
        | None -> g.obj_ty_noNulls
        | Some ty -> ty
