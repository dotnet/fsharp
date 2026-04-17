// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// TypedTreeOps.Remapping: signature operations, expression free variables, expression remapping, and expression shape queries.
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
module internal SignatureOps =

    //--------------------------------------------------------------------------
    // Helpers related to type checking modules & namespaces
    //--------------------------------------------------------------------------

    let wrapModuleOrNamespaceType id cpath mtyp =
        Construct.NewModuleOrNamespace (Some cpath) taccessPublic id XmlDoc.Empty [] (MaybeLazy.Strict mtyp)

    let wrapModuleOrNamespaceTypeInNamespace id cpath mtyp =
        let mspec = wrapModuleOrNamespaceType id cpath mtyp
        Construct.NewModuleOrNamespaceType (Namespace false) [ mspec ] [], mspec

    let wrapModuleOrNamespaceContentsInNamespace isModule (id: Ident) (cpath: CompilationPath) mexpr =
        let mspec =
            wrapModuleOrNamespaceType id cpath (Construct.NewEmptyModuleOrNamespaceType(Namespace(not isModule)))

        TMDefRec(false, [], [], [ ModuleOrNamespaceBinding.Module(mspec, mexpr) ], id.idRange)

    //--------------------------------------------------------------------------
    // Data structures representing what gets hidden and what gets remapped
    // when a module signature is applied to a module.
    //--------------------------------------------------------------------------

    type SignatureRepackageInfo =
        {
            RepackagedVals: (ValRef * ValRef) list
            RepackagedEntities: (TyconRef * TyconRef) list
        }

        member remapInfo.ImplToSigMapping g =
            { TypeEquivEnv.EmptyWithNullChecks g with
                EquivTycons = TyconRefMap.OfList remapInfo.RepackagedEntities
            }

        static member Empty =
            {
                RepackagedVals = []
                RepackagedEntities = []
            }

    type SignatureHidingInfo =
        {
            HiddenTycons: Zset<Tycon>
            HiddenTyconReprs: Zset<Tycon>
            HiddenVals: Zset<Val>
            HiddenRecdFields: Zset<RecdFieldRef>
            HiddenUnionCases: Zset<UnionCaseRef>
        }

        static member Empty =
            {
                HiddenTycons = Zset.empty tyconOrder
                HiddenTyconReprs = Zset.empty tyconOrder
                HiddenVals = Zset.empty valOrder
                HiddenRecdFields = Zset.empty recdFieldRefOrder
                HiddenUnionCases = Zset.empty unionCaseRefOrder
            }

    let addValRemap v vNew tmenv =
        { tmenv with
            valRemap = tmenv.valRemap.Add v (mkLocalValRef vNew)
        }

    let mkRepackageRemapping mrpi =
        {
            valRemap = ValMap.OfList(mrpi.RepackagedVals |> List.map (fun (vref, x) -> vref.Deref, x))
            tpinst = emptyTyparInst
            tyconRefRemap = TyconRefMap.OfList mrpi.RepackagedEntities
            removeTraitSolutions = false
        }

    //--------------------------------------------------------------------------
    // Compute instances of the above for mty -> mty
    //--------------------------------------------------------------------------

    let accEntityRemap (msigty: ModuleOrNamespaceType) (entity: Entity) (mrpi, mhi) =
        let sigtyconOpt =
            (NameMap.tryFind entity.LogicalName msigty.AllEntitiesByCompiledAndLogicalMangledNames)

        match sigtyconOpt with
        | None ->
            // The type constructor is not present in the signature. Hence it is hidden.
            let mhi =
                { mhi with
                    HiddenTycons = Zset.add entity mhi.HiddenTycons
                }

            (mrpi, mhi)
        | Some sigtycon ->
            // The type constructor is in the signature. Hence record the repackage entry
            let sigtcref = mkLocalTyconRef sigtycon
            let tcref = mkLocalTyconRef entity

            let mrpi =
                { mrpi with
                    RepackagedEntities = ((tcref, sigtcref) :: mrpi.RepackagedEntities)
                }
            // OK, now look for hidden things
            let mhi =
                if
                    (match entity.TypeReprInfo with
                     | TNoRepr -> false
                     | _ -> true)
                    && (match sigtycon.TypeReprInfo with
                        | TNoRepr -> true
                        | _ -> false)
                then
                    // The type representation is absent in the signature, hence it is hidden
                    { mhi with
                        HiddenTyconReprs = Zset.add entity mhi.HiddenTyconReprs
                    }
                else
                    // The type representation is present in the signature.
                    // Find the fields that have been hidden or which were non-public anyway.
                    let mhi =
                        (entity.AllFieldsArray, mhi)
                        ||> Array.foldBack (fun rfield mhi ->
                            match sigtycon.GetFieldByName(rfield.LogicalName) with
                            | Some _ ->
                                // The field is in the signature. Hence it is not hidden.
                                mhi
                            | _ ->
                                // The field is not in the signature. Hence it is regarded as hidden.
                                let rfref = tcref.MakeNestedRecdFieldRef rfield

                                { mhi with
                                    HiddenRecdFields = Zset.add rfref mhi.HiddenRecdFields
                                })

                    let mhi =
                        (entity.UnionCasesAsList, mhi)
                        ||> List.foldBack (fun ucase mhi ->
                            match sigtycon.GetUnionCaseByName ucase.LogicalName with
                            | Some _ ->
                                // The constructor is in the signature. Hence it is not hidden.
                                mhi
                            | _ ->
                                // The constructor is not in the signature. Hence it is regarded as hidden.
                                let ucref = tcref.MakeNestedUnionCaseRef ucase

                                { mhi with
                                    HiddenUnionCases = Zset.add ucref mhi.HiddenUnionCases
                                })

                    mhi

            (mrpi, mhi)

    let accSubEntityRemap (msigty: ModuleOrNamespaceType) (entity: Entity) (mrpi, mhi) =
        let sigtyconOpt =
            (NameMap.tryFind entity.LogicalName msigty.AllEntitiesByCompiledAndLogicalMangledNames)

        match sigtyconOpt with
        | None ->
            // The type constructor is not present in the signature. Hence it is hidden.
            let mhi =
                { mhi with
                    HiddenTycons = Zset.add entity mhi.HiddenTycons
                }

            (mrpi, mhi)
        | Some sigtycon ->
            // The type constructor is in the signature. Hence record the repackage entry
            let sigtcref = mkLocalTyconRef sigtycon
            let tcref = mkLocalTyconRef entity

            let mrpi =
                { mrpi with
                    RepackagedEntities = ((tcref, sigtcref) :: mrpi.RepackagedEntities)
                }

            (mrpi, mhi)

    let valLinkageAEquiv g aenv (v1: Val) (v2: Val) =
        (v1.GetLinkagePartialKey() = v2.GetLinkagePartialKey())
        && (if v1.IsMember && v2.IsMember then
                typeAEquivAux EraseAll g aenv v1.Type v2.Type
            else
                true)

    let accValRemap g aenv (msigty: ModuleOrNamespaceType) (implVal: Val) (mrpi, mhi) =
        let implValKey = implVal.GetLinkagePartialKey()

        let sigValOpt =
            msigty.AllValsAndMembersByPartialLinkageKey
            |> MultiMap.find implValKey
            |> List.tryFind (fun sigVal -> valLinkageAEquiv g aenv implVal sigVal)

        let vref = mkLocalValRef implVal

        match sigValOpt with
        | None ->
            let mhi =
                { mhi with
                    HiddenVals = Zset.add implVal mhi.HiddenVals
                }

            (mrpi, mhi)
        | Some(sigVal: Val) ->
            // The value is in the signature. Add the repackage entry.
            let mrpi =
                { mrpi with
                    RepackagedVals = (vref, mkLocalValRef sigVal) :: mrpi.RepackagedVals
                }

            (mrpi, mhi)

    let getCorrespondingSigTy nm (msigty: ModuleOrNamespaceType) =
        match NameMap.tryFind nm msigty.AllEntitiesByCompiledAndLogicalMangledNames with
        | None -> Construct.NewEmptyModuleOrNamespaceType ModuleOrType
        | Some sigsubmodul -> sigsubmodul.ModuleOrNamespaceType

    let rec accEntityRemapFromModuleOrNamespaceType (mty: ModuleOrNamespaceType) (msigty: ModuleOrNamespaceType) acc =
        let acc =
            (mty.AllEntities, acc)
            ||> QueueList.foldBack (fun e acc ->
                accEntityRemapFromModuleOrNamespaceType e.ModuleOrNamespaceType (getCorrespondingSigTy e.LogicalName msigty) acc)

        let acc = (mty.AllEntities, acc) ||> QueueList.foldBack (accEntityRemap msigty)
        acc

    let rec accValRemapFromModuleOrNamespaceType g aenv (mty: ModuleOrNamespaceType) msigty acc =
        let acc =
            (mty.AllEntities, acc)
            ||> QueueList.foldBack (fun e acc ->
                accValRemapFromModuleOrNamespaceType g aenv e.ModuleOrNamespaceType (getCorrespondingSigTy e.LogicalName msigty) acc)

        let acc =
            (mty.AllValsAndMembers, acc) ||> QueueList.foldBack (accValRemap g aenv msigty)

        acc

    let ComputeRemappingFromInferredSignatureToExplicitSignature g mty msigty =
        let mrpi, _ as entityRemap =
            accEntityRemapFromModuleOrNamespaceType mty msigty (SignatureRepackageInfo.Empty, SignatureHidingInfo.Empty)

        let aenv = mrpi.ImplToSigMapping g

        let valAndEntityRemap =
            accValRemapFromModuleOrNamespaceType g aenv mty msigty entityRemap

        valAndEntityRemap

    //--------------------------------------------------------------------------
    // Compute instances of the above for mexpr -> mty
    //--------------------------------------------------------------------------

    /// At TMDefRec nodes abstract (virtual) vslots are effectively binders, even
    /// though they are tucked away inside the tycon. This helper function extracts the
    /// virtual slots to aid with finding this babies.
    let abstractSlotValRefsOfTycons (tycons: Tycon list) =
        tycons
        |> List.collect (fun tycon ->
            if tycon.IsFSharpObjectModelTycon then
                tycon.FSharpTyconRepresentationData.fsobjmodel_vslots
            else
                [])

    let abstractSlotValsOfTycons (tycons: Tycon list) =
        abstractSlotValRefsOfTycons tycons |> List.map (fun v -> v.Deref)

    let rec accEntityRemapFromModuleOrNamespace msigty x acc =
        match x with
        | TMDefRec(_, _, tycons, mbinds, _) ->
            let acc =
                (mbinds, acc) ||> List.foldBack (accEntityRemapFromModuleOrNamespaceBind msigty)

            let acc = (tycons, acc) ||> List.foldBack (accEntityRemap msigty)

            let acc =
                (tycons, acc)
                ||> List.foldBack (fun e acc ->
                    accEntityRemapFromModuleOrNamespaceType e.ModuleOrNamespaceType (getCorrespondingSigTy e.LogicalName msigty) acc)

            acc
        | TMDefLet _ -> acc
        | TMDefOpens _ -> acc
        | TMDefDo _ -> acc
        | TMDefs defs -> accEntityRemapFromModuleOrNamespaceDefs msigty defs acc

    and accEntityRemapFromModuleOrNamespaceDefs msigty mdefs acc =
        List.foldBack (accEntityRemapFromModuleOrNamespace msigty) mdefs acc

    and accEntityRemapFromModuleOrNamespaceBind msigty x acc =
        match x with
        | ModuleOrNamespaceBinding.Binding _ -> acc
        | ModuleOrNamespaceBinding.Module(mspec, def) ->
            accSubEntityRemap msigty mspec (accEntityRemapFromModuleOrNamespace (getCorrespondingSigTy mspec.LogicalName msigty) def acc)

    let rec accValRemapFromModuleOrNamespace g aenv msigty x acc =
        match x with
        | TMDefRec(_, _, tycons, mbinds, _) ->
            let acc =
                (mbinds, acc)
                ||> List.foldBack (accValRemapFromModuleOrNamespaceBind g aenv msigty)
            //  Abstract (virtual) vslots in the tycons at TMDefRec nodes are binders. They also need to be added to the remapping.
            let vslotvs = abstractSlotValsOfTycons tycons
            let acc = (vslotvs, acc) ||> List.foldBack (accValRemap g aenv msigty)
            acc
        | TMDefLet(bind, _) -> accValRemap g aenv msigty bind.Var acc
        | TMDefOpens _ -> acc
        | TMDefDo _ -> acc
        | TMDefs defs -> accValRemapFromModuleOrNamespaceDefs g aenv msigty defs acc

    and accValRemapFromModuleOrNamespaceBind g aenv msigty x acc =
        match x with
        | ModuleOrNamespaceBinding.Binding bind -> accValRemap g aenv msigty bind.Var acc
        | ModuleOrNamespaceBinding.Module(mspec, def) ->
            accSubEntityRemap
                msigty
                mspec
                (accValRemapFromModuleOrNamespace g aenv (getCorrespondingSigTy mspec.LogicalName msigty) def acc)

    and accValRemapFromModuleOrNamespaceDefs g aenv msigty mdefs acc =
        List.foldBack (accValRemapFromModuleOrNamespace g aenv msigty) mdefs acc

    let ComputeRemappingFromImplementationToSignature g mdef msigty =
        let mrpi, _ as entityRemap =
            accEntityRemapFromModuleOrNamespace msigty mdef (SignatureRepackageInfo.Empty, SignatureHidingInfo.Empty)

        let aenv = mrpi.ImplToSigMapping g

        let valAndEntityRemap =
            accValRemapFromModuleOrNamespace g aenv msigty mdef entityRemap

        valAndEntityRemap

    //--------------------------------------------------------------------------
    // Compute instances of the above for the assembly boundary
    //--------------------------------------------------------------------------

    let accTyconHidingInfoAtAssemblyBoundary (tycon: Tycon) mhi =
        if not (canAccessFromEverywhere tycon.Accessibility) then
            // The type constructor is not public, hence hidden at the assembly boundary.
            { mhi with
                HiddenTycons = Zset.add tycon mhi.HiddenTycons
            }
        elif not (canAccessFromEverywhere tycon.TypeReprAccessibility) then
            { mhi with
                HiddenTyconReprs = Zset.add tycon mhi.HiddenTyconReprs
            }
        else
            let mhi =
                (tycon.AllFieldsArray, mhi)
                ||> Array.foldBack (fun rfield mhi ->
                    if not (canAccessFromEverywhere rfield.Accessibility) then
                        let tcref = mkLocalTyconRef tycon
                        let rfref = tcref.MakeNestedRecdFieldRef rfield

                        { mhi with
                            HiddenRecdFields = Zset.add rfref mhi.HiddenRecdFields
                        }
                    else
                        mhi)

            let mhi =
                (tycon.UnionCasesAsList, mhi)
                ||> List.foldBack (fun ucase mhi ->
                    if not (canAccessFromEverywhere ucase.Accessibility) then
                        let tcref = mkLocalTyconRef tycon
                        let ucref = tcref.MakeNestedUnionCaseRef ucase

                        { mhi with
                            HiddenUnionCases = Zset.add ucref mhi.HiddenUnionCases
                        }
                    else
                        mhi)

            mhi

    // Collect up the values hidden at the assembly boundary. This is used by IsHiddenVal to
    // determine if something is considered hidden. This is used in turn to eliminate optimization
    // information at the assembly boundary and to decide to label things as "internal".
    let accValHidingInfoAtAssemblyBoundary (vspec: Val) mhi =
        if // anything labelled "internal" or more restrictive is considered to be hidden at the assembly boundary
            not (canAccessFromEverywhere vspec.Accessibility)
            ||
            // compiler generated members for class function 'let' bindings are considered to be hidden at the assembly boundary
            vspec.IsIncrClassGeneratedMember
            ||
            // anything that's not a module or member binding gets assembly visibility
            not vspec.IsMemberOrModuleBinding
        then
            // The value is not public, hence hidden at the assembly boundary.
            { mhi with
                HiddenVals = Zset.add vspec mhi.HiddenVals
            }
        else
            mhi

    let rec accModuleOrNamespaceHidingInfoAtAssemblyBoundary mty acc =
        let acc =
            QueueList.foldBack
                (fun (e: Entity) acc -> accModuleOrNamespaceHidingInfoAtAssemblyBoundary e.ModuleOrNamespaceType acc)
                mty.AllEntities
                acc

        let acc =
            QueueList.foldBack accTyconHidingInfoAtAssemblyBoundary mty.AllEntities acc

        let acc =
            QueueList.foldBack accValHidingInfoAtAssemblyBoundary mty.AllValsAndMembers acc

        acc

    let ComputeSignatureHidingInfoAtAssemblyBoundary mty acc =
        accModuleOrNamespaceHidingInfoAtAssemblyBoundary mty acc

    let rec accImplHidingInfoAtAssemblyBoundary mdef acc =
        match mdef with
        | TMDefRec(_isRec, _opens, tycons, mbinds, _m) ->
            let acc = List.foldBack accTyconHidingInfoAtAssemblyBoundary tycons acc

            let acc =
                (mbinds, acc)
                ||> List.foldBack (fun mbind acc ->
                    match mbind with
                    | ModuleOrNamespaceBinding.Binding bind -> accValHidingInfoAtAssemblyBoundary bind.Var acc
                    | ModuleOrNamespaceBinding.Module(_mspec, def) -> accImplHidingInfoAtAssemblyBoundary def acc)

            acc

        | TMDefOpens _openDecls -> acc

        | TMDefLet(bind, _m) -> accValHidingInfoAtAssemblyBoundary bind.Var acc

        | TMDefDo _ -> acc

        | TMDefs defs -> List.foldBack accImplHidingInfoAtAssemblyBoundary defs acc

    let ComputeImplementationHidingInfoAtAssemblyBoundary mty acc =
        accImplHidingInfoAtAssemblyBoundary mty acc

    let DoRemap setF remapF =
        let rec remap mrmi x =

            match mrmi with
            | [] -> x
            | (rpi, mhi) :: rest ->
                // Explicitly hidden?
                if Zset.contains x (setF mhi) then
                    x
                else
                    remap rest (remapF rpi x)

        fun mrmi x -> remap mrmi x

    let DoRemapTycon mrmi x =
        DoRemap (fun mhi -> mhi.HiddenTycons) (fun rpi x -> (remapTyconRef rpi.tyconRefRemap (mkLocalTyconRef x)).Deref) mrmi x

    let DoRemapVal mrmi x =
        DoRemap (fun mhi -> mhi.HiddenVals) (fun rpi x -> (remapValRef rpi (mkLocalValRef x)).Deref) mrmi x

    //--------------------------------------------------------------------------
    // Compute instances of the above for mexpr -> mty
    //--------------------------------------------------------------------------
    let IsHidden setF accessF remapF =
        let rec check mrmi x =
            // Internal/private?
            not (canAccessFromEverywhere (accessF x))
            || (match mrmi with
                | [] -> false // Ah! we escaped to freedom!
                | (rpi, mhi) :: rest ->
                    // Explicitly hidden?
                    Zset.contains x (setF mhi)
                    ||
                    // Recurse...
                    check rest (remapF rpi x))

        check

    let IsHiddenTycon mrmi x =
        IsHidden
            (fun mhi -> mhi.HiddenTycons)
            (fun tc -> tc.Accessibility)
            (fun rpi x -> (remapTyconRef rpi.tyconRefRemap (mkLocalTyconRef x)).Deref)
            mrmi
            x

    let IsHiddenTyconRepr mrmi x =
        IsHidden
            (fun mhi -> mhi.HiddenTyconReprs)
            (fun v -> v.TypeReprAccessibility)
            (fun rpi x -> (remapTyconRef rpi.tyconRefRemap (mkLocalTyconRef x)).Deref)
            mrmi
            x

    let IsHiddenVal mrmi x =
        IsHidden (fun mhi -> mhi.HiddenVals) (fun v -> v.Accessibility) (fun rpi x -> (remapValRef rpi (mkLocalValRef x)).Deref) mrmi x

    let IsHiddenRecdField mrmi x =
        IsHidden
            (fun mhi -> mhi.HiddenRecdFields)
            (fun rfref -> rfref.RecdField.Accessibility)
            (fun rpi x -> remapRecdFieldRef rpi.tyconRefRemap x)
            mrmi
            x

    //--------------------------------------------------------------------------
    // Generic operations on module types
    //--------------------------------------------------------------------------

    let foldModuleOrNamespaceTy ft fv mty acc =
        let rec go mty acc =
            let acc =
                QueueList.foldBack (fun (e: Entity) acc -> go e.ModuleOrNamespaceType acc) mty.AllEntities acc

            let acc = QueueList.foldBack ft mty.AllEntities acc
            let acc = QueueList.foldBack fv mty.AllValsAndMembers acc
            acc

        go mty acc

    let allValsOfModuleOrNamespaceTy m =
        foldModuleOrNamespaceTy (fun _ acc -> acc) (fun v acc -> v :: acc) m []

    let allEntitiesOfModuleOrNamespaceTy m =
        foldModuleOrNamespaceTy (fun ft acc -> ft :: acc) (fun _ acc -> acc) m []

    //---------------------------------------------------------------------------
    // Free variables in terms. Are all constructs public accessible?
    //---------------------------------------------------------------------------

    let isPublicVal (lv: Val) = (lv.Accessibility = taccessPublic)

    let isPublicUnionCase (ucr: UnionCaseRef) =
        (ucr.UnionCase.Accessibility = taccessPublic)

    let isPublicRecdField (rfr: RecdFieldRef) =
        (rfr.RecdField.Accessibility = taccessPublic)

    let isPublicTycon (tcref: Tycon) = (tcref.Accessibility = taccessPublic)

    let freeVarsAllPublic fvs =
        // Are any non-public items used in the expr (which corresponded to the fvs)?
        // Recall, taccess occurs in:
        //      EntityData has ReprAccessibility and Accessibility
        //      UnionCase has Accessibility
        //      RecdField has Accessibility
        //      ValData has Accessibility
        // The freevars and FreeTyvars collect local constructs.
        // Here, we test that all those constructs are public.
        //
        // CODE REVIEW:
        // What about non-local vals. This fix assumes non-local vals must be public. OK?
        Zset.forall isPublicVal fvs.FreeLocals
        && Zset.forall isPublicUnionCase fvs.FreeUnionCases
        && Zset.forall isPublicRecdField fvs.FreeRecdFields
        && Zset.forall isPublicTycon fvs.FreeTyvars.FreeTycons

    let freeTyvarsAllPublic tyvars =
        Zset.forall isPublicTycon tyvars.FreeTycons

    /// Combine a list of ModuleOrNamespaceType's making up the description of a CCU. checking there are now
    /// duplicate modules etc.
    let CombineCcuContentFragments l =

        /// Combine module types when multiple namespace fragments contribute to the
        /// same namespace, making new module specs as we go.
        let rec CombineModuleOrNamespaceTypes path (mty1: ModuleOrNamespaceType) (mty2: ModuleOrNamespaceType) =
            let kind = mty1.ModuleOrNamespaceKind
            let tab1 = mty1.AllEntitiesByLogicalMangledName
            let tab2 = mty2.AllEntitiesByLogicalMangledName

            let entities =
                [
                    for e1 in mty1.AllEntities do
                        match tab2.TryGetValue e1.LogicalName with
                        | true, e2 -> yield CombineEntities path e1 e2
                        | _ -> yield e1

                    for e2 in mty2.AllEntities do
                        match tab1.TryGetValue e2.LogicalName with
                        | true, _ -> ()
                        | _ -> yield e2
                ]

            let vals = QueueList.append mty1.AllValsAndMembers mty2.AllValsAndMembers

            ModuleOrNamespaceType(kind, vals, QueueList.ofList entities)

        and CombineEntities path (entity1: Entity) (entity2: Entity) =

            let path2 = path @ [ entity2.DemangledModuleOrNamespaceName ]

            match entity1.IsNamespace, entity2.IsNamespace, entity1.IsModule, entity2.IsModule with
            | true, true, _, _ -> ()
            | true, _, _, _
            | _, true, _, _ -> errorR (Error(FSComp.SR.tastNamespaceAndModuleWithSameNameInAssembly (textOfPath path2), entity2.Range))
            | false, false, false, false ->
                errorR (Error(FSComp.SR.tastDuplicateTypeDefinitionInAssembly (entity2.LogicalName, textOfPath path), entity2.Range))
            | false, false, true, true -> errorR (Error(FSComp.SR.tastTwoModulesWithSameNameInAssembly (textOfPath path2), entity2.Range))
            | _ ->
                errorR (
                    Error(FSComp.SR.tastConflictingModuleAndTypeDefinitionInAssembly (entity2.LogicalName, textOfPath path), entity2.Range)
                )

            entity1
            |> Construct.NewModifiedTycon(fun data1 ->
                let xml = XmlDoc.Merge entity1.XmlDoc entity2.XmlDoc

                { data1 with
                    entity_attribs =
                        if entity2.Attribs.IsEmpty then
                            entity1.EntityAttribs
                        elif entity1.Attribs.IsEmpty then
                            entity2.EntityAttribs
                        else
                            WellKnownEntityAttribs.Create(entity1.Attribs @ entity2.Attribs)
                    entity_modul_type =
                        MaybeLazy.Lazy(
                            InterruptibleLazy(fun _ ->
                                CombineModuleOrNamespaceTypes path2 entity1.ModuleOrNamespaceType entity2.ModuleOrNamespaceType)
                        )
                    entity_opt_data =
                        match data1.entity_opt_data with
                        | Some optData -> Some { optData with entity_xmldoc = xml }
                        | _ ->
                            Some
                                { Entity.NewEmptyEntityOptData() with
                                    entity_xmldoc = xml
                                }
                })

        and CombineModuleOrNamespaceTypeList path l =
            match l with
            | h :: t -> List.fold (CombineModuleOrNamespaceTypes path) h t
            | _ -> failwith "CombineModuleOrNamespaceTypeList"

        CombineModuleOrNamespaceTypeList [] l

    //--------------------------------------------------------------------------
    // Build a Remap that converts all "local" references to "public" things
    // accessed via non local references.
    //--------------------------------------------------------------------------

    let MakeExportRemapping viewedCcu (mspec: ModuleOrNamespace) =

        let accEntityRemap (entity: Entity) acc =
            match tryRescopeEntity viewedCcu entity with
            | ValueSome eref -> addTyconRefRemap (mkLocalTyconRef entity) eref acc
            | _ ->
                if entity.IsNamespace then
                    acc
                else
                    error (InternalError("Unexpected entity without a pubpath when remapping assembly data", entity.Range))

        let accValRemap (vspec: Val) acc =
            // The acc contains the entity remappings
            match tryRescopeVal viewedCcu acc vspec with
            | ValueSome vref ->
                { acc with
                    valRemap = acc.valRemap.Add vspec vref
                }
            | _ -> error (InternalError("Unexpected value without a pubpath when remapping assembly data", vspec.Range))

        let mty = mspec.ModuleOrNamespaceType
        let entities = allEntitiesOfModuleOrNamespaceTy mty
        let vs = allValsOfModuleOrNamespaceTy mty
        // Remap the entities first so we can correctly remap the types in the signatures of the ValLinkageFullKey's in the value references
        let acc = List.foldBack accEntityRemap entities Remap.Empty
        let allRemap = List.foldBack accValRemap vs acc
        allRemap

    let updateSeqTypeIsPrefix (fsharpCoreMSpec: ModuleOrNamespace) =
        let findModuleOrNamespace (name: string) (entity: Entity) =
            if not entity.IsModuleOrNamespace then
                None
            else
                entity.ModuleOrNamespaceType.ModulesAndNamespacesByDemangledName
                |> Map.tryFind name

        findModuleOrNamespace "Microsoft" fsharpCoreMSpec
        |> Option.bind (findModuleOrNamespace "FSharp")
        |> Option.bind (findModuleOrNamespace "Collections")
        |> Option.iter (fun collectionsEntity ->
            collectionsEntity.ModuleOrNamespaceType.AllEntitiesByLogicalMangledName
            |> Map.tryFind "seq`1"
            |> Option.iter (fun seqEntity ->
                seqEntity.entity_flags <-
                    EntityFlags(
                        false,
                        seqEntity.entity_flags.IsModuleOrNamespace,
                        seqEntity.entity_flags.PreEstablishedHasDefaultConstructor,
                        seqEntity.entity_flags.HasSelfReferentialConstructor,
                        seqEntity.entity_flags.IsStructRecordOrUnionType
                    )))

    /// Matches a ModuleOrNamespaceContents that is empty from a signature printing point of view.
    /// Signatures printed via the typed tree in NicePrint don't print TMDefOpens or TMDefDo.
    /// This will match anything that does not have any types or bindings.
    [<return: Struct>]
    let (|EmptyModuleOrNamespaces|_|) (moduleOrNamespaceContents: ModuleOrNamespaceContents) =
        match moduleOrNamespaceContents with
        | TMDefs(defs = defs) ->
            let mdDefsLength =
                defs
                |> List.count (function
                    | ModuleOrNamespaceContents.TMDefRec _
                    | ModuleOrNamespaceContents.TMDefs _ -> true
                    | _ -> false)

            let emptyModuleOrNamespaces =
                defs
                |> List.choose (function
                    | ModuleOrNamespaceContents.TMDefRec _ as defRec
                    | ModuleOrNamespaceContents.TMDefs(defs = [ ModuleOrNamespaceContents.TMDefRec _ as defRec ]) ->
                        match defRec with
                        | TMDefRec(bindings = [ ModuleOrNamespaceBinding.Module(mspec, ModuleOrNamespaceContents.TMDefs(defs = defs)) ]) ->
                            defs
                            |> List.forall (function
                                | ModuleOrNamespaceContents.TMDefOpens _
                                | ModuleOrNamespaceContents.TMDefDo _
                                | ModuleOrNamespaceContents.TMDefRec(isRec = true; tycons = []; bindings = []) -> true
                                | _ -> false)
                            |> fun isEmpty -> if isEmpty then Some mspec else None
                        | _ -> None
                    | _ -> None)

            if mdDefsLength = emptyModuleOrNamespaces.Length then
                ValueSome emptyModuleOrNamespaces
            else
                ValueNone
        | _ -> ValueNone

[<AutoOpen>]
module internal ExprFreeVars =

    /// Detect the subset of match expressions we process in a linear way (i.e. using tailcalls, rather than
    /// unbounded stack)
    ///   -- if then else
    ///   -- match e with pat[vs] -> e1[vs] | _ -> e2

    [<return: Struct>]
    let (|LinearMatchExpr|_|) expr =
        match expr with
        | Expr.Match(sp, m, dtree, [| tg1; (TTarget([], e2, _)) |], m2, ty) -> ValueSome(sp, m, dtree, tg1, e2, m2, ty)
        | _ -> ValueNone

    let rebuildLinearMatchExpr (sp, m, dtree, tg1, e2, m2, ty) =
        primMkMatch (sp, m, dtree, [| tg1; TTarget([], e2, None) |], m2, ty)

    /// Detect a subset of 'Expr.Op' expressions we process in a linear way (i.e. using tailcalls, rather than
    /// unbounded stack). Only covers Cons(args,Cons(args,Cons(args,Cons(args,...._)))).
    [<return: Struct>]
    let (|LinearOpExpr|_|) expr =
        match expr with
        | Expr.Op(TOp.UnionCase _ as op, tinst, args, m) when not args.IsEmpty ->
            let argsFront, argLast = List.frontAndBack args
            ValueSome(op, tinst, argsFront, argLast, m)
        | _ -> ValueNone

    let rebuildLinearOpExpr (op, tinst, argsFront, argLast, m) =
        Expr.Op(op, tinst, argsFront @ [ argLast ], m)

    //---------------------------------------------------------------------------
    // Free variables in terms. All binders are distinct.
    //---------------------------------------------------------------------------

    let emptyFreeVars =
        {
            UsesMethodLocalConstructs = false
            UsesUnboundRethrow = false
            FreeLocalTyconReprs = emptyFreeTycons
            FreeLocals = emptyFreeLocals
            FreeTyvars = emptyFreeTyvars
            FreeRecdFields = emptyFreeRecdFields
            FreeUnionCases = emptyFreeUnionCases
        }

    let unionFreeVars fvs1 fvs2 =
        if fvs1 === emptyFreeVars then
            fvs2
        else if fvs2 === emptyFreeVars then
            fvs1
        else
            {
                FreeLocals = unionFreeLocals fvs1.FreeLocals fvs2.FreeLocals
                FreeTyvars = unionFreeTyvars fvs1.FreeTyvars fvs2.FreeTyvars
                UsesMethodLocalConstructs = fvs1.UsesMethodLocalConstructs || fvs2.UsesMethodLocalConstructs
                UsesUnboundRethrow = fvs1.UsesUnboundRethrow || fvs2.UsesUnboundRethrow
                FreeLocalTyconReprs = unionFreeTycons fvs1.FreeLocalTyconReprs fvs2.FreeLocalTyconReprs
                FreeRecdFields = unionFreeRecdFields fvs1.FreeRecdFields fvs2.FreeRecdFields
                FreeUnionCases = unionFreeUnionCases fvs1.FreeUnionCases fvs2.FreeUnionCases
            }

    let inline accFreeTyvars (opts: FreeVarOptions) f v acc =
        if not opts.collectInTypes then
            acc
        else
            let ftyvs = acc.FreeTyvars
            let ftyvs' = f opts v ftyvs

            if ftyvs === ftyvs' then
                acc
            else
                { acc with FreeTyvars = ftyvs' }

    let accFreeVarsInTy opts ty acc = accFreeTyvars opts accFreeInType ty acc

    let accFreeVarsInTys opts tys acc =
        if isNil tys then
            acc
        else
            accFreeTyvars opts accFreeInTypes tys acc

    let accFreevarsInTycon opts tcref acc =
        accFreeTyvars opts accFreeTycon tcref acc

    let accFreevarsInVal opts v acc = accFreeTyvars opts accFreeInVal v acc

    let accFreeVarsInTraitSln opts tys acc =
        accFreeTyvars opts accFreeInTraitSln tys acc

    let accFreeVarsInTraitInfo opts tys acc =
        accFreeTyvars opts accFreeInTrait tys acc

    let boundLocalVal opts v fvs =
        if not opts.includeLocals then
            fvs
        else
            let fvs = accFreevarsInVal opts v fvs

            if not (Zset.contains v fvs.FreeLocals) then
                fvs
            else
                { fvs with
                    FreeLocals = Zset.remove v fvs.FreeLocals
                }

    let boundProtect fvs =
        if fvs.UsesMethodLocalConstructs then
            { fvs with
                UsesMethodLocalConstructs = false
            }
        else
            fvs

    let accUsesFunctionLocalConstructs flg fvs =
        if flg && not fvs.UsesMethodLocalConstructs then
            { fvs with
                UsesMethodLocalConstructs = true
            }
        else
            fvs

    let bound_rethrow fvs =
        if fvs.UsesUnboundRethrow then
            { fvs with UsesUnboundRethrow = false }
        else
            fvs

    let accUsesRethrow flg fvs =
        if flg && not fvs.UsesUnboundRethrow then
            { fvs with UsesUnboundRethrow = true }
        else
            fvs

    let boundLocalVals opts vs fvs =
        List.foldBack (boundLocalVal opts) vs fvs

    let bindLhs opts (bind: Binding) fvs = boundLocalVal opts bind.Var fvs

    let freeVarsCacheCompute opts cache f =
        if opts.canCache then cached cache f else f ()

    let tryGetFreeVarsCacheValue opts cache =
        if opts.canCache then tryGetCacheValue cache else ValueNone

    let accFreeLocalVal opts v fvs =
        if not opts.includeLocals then
            fvs
        else if Zset.contains v fvs.FreeLocals then
            fvs
        else
            let fvs = accFreevarsInVal opts v fvs

            { fvs with
                FreeLocals = Zset.add v fvs.FreeLocals
            }

    let accFreeInValFlags opts flag acc =
        let isMethLocal =
            match flag with
            | VSlotDirectCall
            | CtorValUsedAsSelfInit
            | CtorValUsedAsSuperInit -> true
            | PossibleConstrainedCall _
            | NormalValUse -> false

        let acc = accUsesFunctionLocalConstructs isMethLocal acc

        match flag with
        | PossibleConstrainedCall ty -> accFreeTyvars opts accFreeInType ty acc
        | _ -> acc

    let accLocalTyconRepr opts b fvs =
        if not opts.includeLocalTyconReprs then
            fvs
        else if Zset.contains b fvs.FreeLocalTyconReprs then
            fvs
        else
            { fvs with
                FreeLocalTyconReprs = Zset.add b fvs.FreeLocalTyconReprs
            }

    let inline accFreeExnRef _exnc fvs = fvs // Note: this exnc (TyconRef) should be collected the surround types, e.g. tinst of Expr.Op

    let rec accBindRhs opts (TBind(_, repr, _)) acc = accFreeInExpr opts repr acc

    and accFreeInSwitchCases opts csl dflt (acc: FreeVars) =
        Option.foldBack (accFreeInDecisionTree opts) dflt (List.foldBack (accFreeInSwitchCase opts) csl acc)

    and accFreeInSwitchCase opts (TCase(discrim, dtree)) acc =
        accFreeInDecisionTree opts dtree (accFreeInTest opts discrim acc)

    and accFreeInTest (opts: FreeVarOptions) discrim acc =
        match discrim with
        | DecisionTreeTest.UnionCase(ucref, tinst) -> accFreeUnionCaseRef opts ucref (accFreeVarsInTys opts tinst acc)
        | DecisionTreeTest.ArrayLength(_, ty) -> accFreeVarsInTy opts ty acc
        | DecisionTreeTest.Const _
        | DecisionTreeTest.IsNull -> acc
        | DecisionTreeTest.IsInst(srcTy, tgtTy) -> accFreeVarsInTy opts srcTy (accFreeVarsInTy opts tgtTy acc)
        | DecisionTreeTest.ActivePatternCase(exp, tys, _, activePatIdentity, _, _) ->
            accFreeInExpr
                opts
                exp
                (accFreeVarsInTys
                    opts
                    tys
                    (Option.foldBack
                        (fun (vref, tinst) acc -> accFreeValRef opts vref (accFreeVarsInTys opts tinst acc))
                        activePatIdentity
                        acc))
        | DecisionTreeTest.Error _ -> acc

    and accFreeInDecisionTree opts x (acc: FreeVars) =
        match x with
        | TDSwitch(e1, csl, dflt, _) -> accFreeInExpr opts e1 (accFreeInSwitchCases opts csl dflt acc)
        | TDSuccess(es, _) -> accFreeInFlatExprs opts es acc
        | TDBind(bind, body) -> unionFreeVars (bindLhs opts bind (accBindRhs opts bind (freeInDecisionTree opts body))) acc

    and accUsedRecdOrUnionTyconRepr opts (tc: Tycon) fvs =
        if
            (match tc.TypeReprInfo with
             | TFSharpTyconRepr _ -> true
             | _ -> false)
        then
            accLocalTyconRepr opts tc fvs
        else
            fvs

    and accFreeUnionCaseRef opts ucref fvs =
        if not opts.includeUnionCases then
            fvs
        else if Zset.contains ucref fvs.FreeUnionCases then
            fvs
        else
            let fvs = fvs |> accUsedRecdOrUnionTyconRepr opts ucref.Tycon
            let fvs = fvs |> accFreevarsInTycon opts ucref.TyconRef

            { fvs with
                FreeUnionCases = Zset.add ucref fvs.FreeUnionCases
            }

    and accFreeRecdFieldRef opts rfref fvs =
        if not opts.includeRecdFields then
            fvs
        else if Zset.contains rfref fvs.FreeRecdFields then
            fvs
        else
            let fvs = fvs |> accUsedRecdOrUnionTyconRepr opts rfref.Tycon
            let fvs = fvs |> accFreevarsInTycon opts rfref.TyconRef

            { fvs with
                FreeRecdFields = Zset.add rfref fvs.FreeRecdFields
            }

    and accFreeValRef opts (vref: ValRef) fvs =
        match vref.IsLocalRef with
        | true -> accFreeLocalVal opts vref.ResolvedTarget fvs
        // non-local values do not contain free variables
        | _ -> fvs

    and accFreeInMethod opts (TObjExprMethod(slotsig, _attribs, tps, tmvs, e, _)) acc =
        accFreeInSlotSig
            opts
            slotsig
            (unionFreeVars (accFreeTyvars opts boundTypars tps (List.foldBack (boundLocalVals opts) tmvs (freeInExpr opts e))) acc)

    and accFreeInMethods opts methods acc =
        List.foldBack (accFreeInMethod opts) methods acc

    and accFreeInInterfaceImpl opts (ty, overrides) acc =
        accFreeVarsInTy opts ty (accFreeInMethods opts overrides acc)

    and accFreeInExpr (opts: FreeVarOptions) x acc =
        match x with
        | Expr.Let _ -> accFreeInExprLinear opts x acc id
        | _ -> accFreeInExprNonLinear opts x acc

    and accFreeInExprLinear (opts: FreeVarOptions) x acc contf =
        // for nested let-bindings, we need to continue after the whole let-binding is processed
        match x with
        | Expr.Let(bind, e, _, cache) ->
            match tryGetFreeVarsCacheValue opts cache with
            | ValueSome free -> contf (unionFreeVars free acc)
            | _ ->
                accFreeInExprLinear
                    opts
                    e
                    emptyFreeVars
                    (contf
                     << (fun free ->
                         unionFreeVars (freeVarsCacheCompute opts cache (fun () -> bindLhs opts bind (accBindRhs opts bind free))) acc))
        | _ ->
            // No longer linear expr
            contf (accFreeInExpr opts x acc)

    and accFreeInExprNonLinear opts x acc =

        match opts.stackGuard with
        | None -> accFreeInExprNonLinearImpl opts x acc
        | Some stackGuard -> stackGuard.Guard(fun () -> accFreeInExprNonLinearImpl opts x acc)

    and accFreeInExprNonLinearImpl opts x acc =

        match x with
        // BINDING CONSTRUCTS
        | Expr.Lambda(_, ctorThisValOpt, baseValOpt, vs, bodyExpr, _, bodyTy) ->
            unionFreeVars
                (Option.foldBack
                    (boundLocalVal opts)
                    ctorThisValOpt
                    (Option.foldBack
                        (boundLocalVal opts)
                        baseValOpt
                        (boundLocalVals opts vs (accFreeVarsInTy opts bodyTy (freeInExpr opts bodyExpr)))))
                acc

        | Expr.TyLambda(_, vs, bodyExpr, _, bodyTy) ->
            unionFreeVars (accFreeTyvars opts boundTypars vs (accFreeVarsInTy opts bodyTy (freeInExpr opts bodyExpr))) acc

        | Expr.TyChoose(vs, bodyExpr, _) -> unionFreeVars (accFreeTyvars opts boundTypars vs (freeInExpr opts bodyExpr)) acc

        | Expr.LetRec(binds, bodyExpr, _, cache) ->
            unionFreeVars
                (freeVarsCacheCompute opts cache (fun () ->
                    List.foldBack (bindLhs opts) binds (List.foldBack (accBindRhs opts) binds (freeInExpr opts bodyExpr))))
                acc

        | Expr.Let _ -> failwith "unreachable - linear expr"

        | Expr.Obj(_, ty, basev, basecall, overrides, iimpls, _) ->
            unionFreeVars
                (boundProtect (
                    Option.foldBack
                        (boundLocalVal opts)
                        basev
                        (accFreeVarsInTy
                            opts
                            ty
                            (accFreeInExpr
                                opts
                                basecall
                                (accFreeInMethods opts overrides (List.foldBack (accFreeInInterfaceImpl opts) iimpls emptyFreeVars))))
                ))
                acc

        // NON-BINDING CONSTRUCTS
        | Expr.Const _ -> acc

        | Expr.Val(lvr, flags, _) -> accFreeInValFlags opts flags (accFreeValRef opts lvr acc)

        | Expr.Quote(ast, dataCell, _, _, ty) ->
            match dataCell.Value with
            | Some(_, (_, argTypes, argExprs, _data)) ->
                accFreeInExpr opts ast (accFreeInExprs opts argExprs (accFreeVarsInTys opts argTypes (accFreeVarsInTy opts ty acc)))

            | None -> accFreeInExpr opts ast (accFreeVarsInTy opts ty acc)

        | Expr.App(f0, f0ty, tyargs, args, _) ->
            accFreeVarsInTy opts f0ty (accFreeInExpr opts f0 (accFreeVarsInTys opts tyargs (accFreeInExprs opts args acc)))

        | Expr.Link eref -> accFreeInExpr opts eref.Value acc

        | Expr.Sequential(expr1, expr2, _, _) ->
            let acc = accFreeInExpr opts expr1 acc
            // tail-call - linear expression
            accFreeInExpr opts expr2 acc

        | Expr.StaticOptimization(_, expr2, expr3, _) -> accFreeInExpr opts expr2 (accFreeInExpr opts expr3 acc)

        | Expr.Match(_, _, dtree, targets, _, _) ->
            match x with
            // Handle if-then-else
            | LinearMatchExpr(_, _, dtree, target, bodyExpr, _, _) ->
                let acc = accFreeInDecisionTree opts dtree acc
                let acc = accFreeInTarget opts target acc
                accFreeInExpr opts bodyExpr acc // tailcall

            | _ ->
                let acc = accFreeInDecisionTree opts dtree acc
                accFreeInTargets opts targets acc

        | Expr.Op(TOp.TryWith _, tinst, [ expr1; expr2; expr3 ], _) ->
            unionFreeVars
                (accFreeVarsInTys opts tinst (accFreeInExprs opts [ expr1; expr2 ] acc))
                (bound_rethrow (accFreeInExpr opts expr3 emptyFreeVars))

        | Expr.Op(op, tinst, args, _) ->
            let acc = accFreeInOp opts op acc
            let acc = accFreeVarsInTys opts tinst acc
            accFreeInExprs opts args acc

        | Expr.WitnessArg(traitInfo, _) -> accFreeVarsInTraitInfo opts traitInfo acc

        | Expr.DebugPoint(_, innerExpr) -> accFreeInExpr opts innerExpr acc

    and accFreeInOp opts op acc =
        match op with

        // Things containing no references
        | TOp.Bytes _
        | TOp.UInt16s _
        | TOp.TryWith _
        | TOp.TryFinally _
        | TOp.IntegerForLoop _
        | TOp.Coerce
        | TOp.RefAddrGet _
        | TOp.Array
        | TOp.While _
        | TOp.Goto _
        | TOp.Label _
        | TOp.Return
        | TOp.TupleFieldGet _ -> acc

        | TOp.Tuple tupInfo -> accFreeTyvars opts accFreeInTupInfo tupInfo acc

        | TOp.AnonRecd anonInfo
        | TOp.AnonRecdGet(anonInfo, _) -> accFreeTyvars opts accFreeInTupInfo anonInfo.TupInfo acc

        | TOp.UnionCaseTagGet tcref -> accUsedRecdOrUnionTyconRepr opts tcref.Deref acc

        // Things containing just a union case reference
        | TOp.UnionCaseProof ucref
        | TOp.UnionCase ucref
        | TOp.UnionCaseFieldGetAddr(ucref, _, _)
        | TOp.UnionCaseFieldGet(ucref, _)
        | TOp.UnionCaseFieldSet(ucref, _) -> accFreeUnionCaseRef opts ucref acc

        // Things containing just an exception reference
        | TOp.ExnConstr ecref
        | TOp.ExnFieldGet(ecref, _)
        | TOp.ExnFieldSet(ecref, _) -> accFreeExnRef ecref acc

        | TOp.ValFieldGet fref
        | TOp.ValFieldGetAddr(fref, _)
        | TOp.ValFieldSet fref -> accFreeRecdFieldRef opts fref acc

        | TOp.Recd(kind, tcref) ->
            let acc = accUsesFunctionLocalConstructs (kind = RecdExprIsObjInit) acc
            (accUsedRecdOrUnionTyconRepr opts tcref.Deref (accFreeTyvars opts accFreeTycon tcref acc))

        | TOp.ILAsm(_, retTypes) -> accFreeVarsInTys opts retTypes acc

        | TOp.Reraise -> accUsesRethrow true acc

        | TOp.TraitCall(TTrait(tys, _, _, argTys, retTy, _, sln)) ->
            Option.foldBack
                (accFreeVarsInTraitSln opts)
                sln.Value
                (accFreeVarsInTys opts tys (accFreeVarsInTys opts argTys (Option.foldBack (accFreeVarsInTy opts) retTy acc)))

        | TOp.LValueOp(_, vref) -> accFreeValRef opts vref acc

        | TOp.ILCall(_, isProtected, _, _, valUseFlag, _, _, _, enclTypeInst, methInst, retTypes) ->
            accFreeVarsInTys
                opts
                enclTypeInst
                (accFreeVarsInTys
                    opts
                    methInst
                    (accFreeInValFlags opts valUseFlag (accFreeVarsInTys opts retTypes (accUsesFunctionLocalConstructs isProtected acc))))

    and accFreeInTargets opts targets acc =
        Array.foldBack (accFreeInTarget opts) targets acc

    and accFreeInTarget opts (TTarget(vs, expr, flags)) acc =
        match flags with
        | None -> List.foldBack (boundLocalVal opts) vs (accFreeInExpr opts expr acc)
        | Some xs ->
            List.foldBack2
                (fun v isStateVar acc -> if isStateVar then acc else boundLocalVal opts v acc)
                vs
                xs
                (accFreeInExpr opts expr acc)

    and accFreeInFlatExprs opts (exprs: Exprs) acc =
        List.foldBack (accFreeInExpr opts) exprs acc

    and accFreeInExprs opts (exprs: Exprs) acc =
        match exprs with
        | [] -> acc
        | [ h ] ->
            // tailcall - e.g. Cons(x, Cons(x2, .......Cons(x1000000, Nil))) and [| x1; .... ; x1000000 |]
            accFreeInExpr opts h acc
        | h :: t ->
            let acc = accFreeInExpr opts h acc
            accFreeInExprs opts t acc

    and accFreeInSlotSig opts (TSlotSig(_, ty, _, _, _, _)) acc = accFreeVarsInTy opts ty acc

    and freeInDecisionTree opts dtree =
        accFreeInDecisionTree opts dtree emptyFreeVars

    and freeInExpr opts expr = accFreeInExpr opts expr emptyFreeVars

    // Note: these are only an approximation - they are currently used only by the optimizer
    let rec accFreeInModuleOrNamespace opts mexpr acc =
        match mexpr with
        | TMDefRec(_, _, _, mbinds, _) -> List.foldBack (accFreeInModuleOrNamespaceBind opts) mbinds acc
        | TMDefLet(bind, _) -> accBindRhs opts bind acc
        | TMDefDo(e, _) -> accFreeInExpr opts e acc
        | TMDefOpens _ -> acc
        | TMDefs defs -> accFreeInModuleOrNamespaces opts defs acc

    and accFreeInModuleOrNamespaceBind opts mbind acc =
        match mbind with
        | ModuleOrNamespaceBinding.Binding bind -> accBindRhs opts bind acc
        | ModuleOrNamespaceBinding.Module(_, def) -> accFreeInModuleOrNamespace opts def acc

    and accFreeInModuleOrNamespaces opts mexprs acc =
        List.foldBack (accFreeInModuleOrNamespace opts) mexprs acc

    let freeInBindingRhs opts bind = accBindRhs opts bind emptyFreeVars

    let freeInModuleOrNamespace opts mdef =
        accFreeInModuleOrNamespace opts mdef emptyFreeVars

[<AutoOpen>]
module internal ExprRemapping =

    //---------------------------------------------------------------------------
    // Destruct - rarely needed
    //---------------------------------------------------------------------------

    let rec stripLambda (expr, ty) =
        match expr with
        | Expr.Lambda(_, ctorThisValOpt, baseValOpt, v, bodyExpr, _, bodyTy) ->
            if Option.isSome ctorThisValOpt then
                errorR (InternalError("skipping ctorThisValOpt", expr.Range))

            if Option.isSome baseValOpt then
                errorR (InternalError("skipping baseValOpt", expr.Range))

            let vs', bodyExpr', bodyTy' = stripLambda (bodyExpr, bodyTy)
            (v :: vs', bodyExpr', bodyTy')
        | _ -> ([], expr, ty)

    let rec stripLambdaN n expr =
        assert (n >= 0)

        match expr with
        | Expr.Lambda(_, ctorThisValOpt, baseValOpt, v, bodyExpr, _, _) when n > 0 ->
            if Option.isSome ctorThisValOpt then
                errorR (InternalError("skipping ctorThisValOpt", expr.Range))

            if Option.isSome baseValOpt then
                errorR (InternalError("skipping baseValOpt", expr.Range))

            let vs, bodyExpr', remaining = stripLambdaN (n - 1) bodyExpr
            (v :: vs, bodyExpr', remaining)
        | _ -> ([], expr, n)

    let tryStripLambdaN n expr =
        match expr with
        | Expr.Lambda(_, None, None, _, _, _, _) ->
            let argvsl, bodyExpr, remaining = stripLambdaN n expr
            if remaining = 0 then Some(argvsl, bodyExpr) else None
        | _ -> None

    let stripTopLambda (expr, exprTy) =
        let tps, taue, tauty =
            match expr with
            | Expr.TyLambda(_, tps, body, _, bodyTy) -> tps, body, bodyTy
            | _ -> [], expr, exprTy

        let vs, body, bodyTy = stripLambda (taue, tauty)
        tps, vs, body, bodyTy

    [<RequireQualifiedAccess>]
    type AllowTypeDirectedDetupling =
        | Yes
        | No

    // This is used to infer arities of expressions
    // i.e. base the chosen arity on the syntactic expression shape and type of arguments
    let InferValReprInfoOfExpr g allowTypeDirectedDetupling ty partialArgAttribsL retAttribs expr =
        let rec stripLambda_notypes e =
            match stripDebugPoints e with
            | Expr.Lambda(_, _, _, vs, b, _, _) ->
                let vs', b' = stripLambda_notypes b
                (vs :: vs', b')
            | Expr.TyChoose(_, b, _) -> stripLambda_notypes b
            | _ -> ([], e)

        let stripTopLambdaNoTypes e =
            let tps, taue =
                match stripDebugPoints e with
                | Expr.TyLambda(_, tps, b, _, _) -> tps, b
                | _ -> [], e

            let vs, body = stripLambda_notypes taue
            tps, vs, body

        let tps, vsl, _ = stripTopLambdaNoTypes expr
        let fun_arity = vsl.Length
        let dtys, _ = stripFunTyN g fun_arity (snd (tryDestForallTy g ty))
        let partialArgAttribsL = Array.ofList partialArgAttribsL
        assert (List.length vsl = List.length dtys)

        let curriedArgInfos =
            (vsl, dtys)
            ||> List.mapi2 (fun i vs ty ->
                let partialAttribs =
                    if i < partialArgAttribsL.Length then
                        partialArgAttribsL[i]
                    else
                        []

                let tys =
                    match allowTypeDirectedDetupling with
                    | AllowTypeDirectedDetupling.No -> [ ty ]
                    | AllowTypeDirectedDetupling.Yes ->
                        if (i = 0 && isUnitTy g ty) then
                            []
                        else
                            tryDestRefTupleTy g ty

                let ids =
                    if vs.Length = tys.Length then
                        vs |> List.map (fun v -> Some v.Id)
                    else
                        tys |> List.map (fun _ -> None)

                let attribs =
                    if partialAttribs.Length = tys.Length then
                        partialAttribs
                    else
                        tys |> List.map (fun _ -> [])

                (ids, attribs)
                ||> List.map2 (fun id attribs ->
                    {
                        Name = id
                        Attribs = WellKnownValAttribs.Create(attribs)
                        OtherRange = None
                    }
                    : ArgReprInfo))

        let retInfo: ArgReprInfo =
            {
                Attribs = WellKnownValAttribs.Create(retAttribs)
                Name = None
                OtherRange = None
            }

        let info = ValReprInfo(ValReprInfo.InferTyparInfo tps, curriedArgInfos, retInfo)

        if ValReprInfo.IsEmpty info then
            ValReprInfo.emptyValData
        else
            info

    let InferValReprInfoOfBinding g allowTypeDirectedDetupling (v: Val) expr =
        match v.ValReprInfo with
        | Some info -> info
        | None -> InferValReprInfoOfExpr g allowTypeDirectedDetupling v.Type [] [] expr

    //-------------------------------------------------------------------------
    // Check if constraints are satisfied that allow us to use more optimized
    // implementations
    //-------------------------------------------------------------------------

    //--------------------------------------------------------------------------
    // Resolve static optimization constraints
    //--------------------------------------------------------------------------

    type StaticOptimizationAnswer =
        | Yes = 1y
        | No = -1y
        | Unknown = 0y

    // Most static optimization conditionals in FSharp.Core are
    //   ^T : tycon
    //
    // These decide positively if ^T is nominal and identical to tycon.
    // These decide negatively if ^T is nominal and different to tycon.
    //
    // The "special" static optimization conditionals
    //    ^T : ^T
    //    'T : 'T
    // are used as hacks in FSharp.Core as follows:
    //    ^T : ^T  --> used in (+), (-) etc. to guard witness-invoking implementations added in F# 5
    //    'T : 'T  --> used in FastGenericEqualityComparer, FastGenericComparer to guard struct/tuple implementations
    //
    // For performance and compatibility reasons, 'T when 'T is an enum is handled with its own special hack.
    // Unlike for other 'T : tycon constraints, 'T can be any enum; it need not (and indeed must not) be identical to System.Enum itself.
    //    'T : Enum
    //
    // In order to add this hack in a backwards-compatible way, we must hide this capability behind a marker type
    // which we use solely as an indicator of whether the compiler understands `when 'T : Enum`.
    //    'T : SupportsWhenTEnum
    //
    // canDecideTyparEqn is set to true in IlxGen when the witness-invoking implementation can be used.
    let decideStaticOptimizationConstraint g c canDecideTyparEqn =
        match c with
        | TTyconEqualsTycon(a, b) when canDecideTyparEqn && typeEquiv g a b && isTyparTy g a -> StaticOptimizationAnswer.Yes
        | TTyconEqualsTycon(_, b) when tryTcrefOfAppTy g b |> ValueOption.exists (tyconRefEq g g.SupportsWhenTEnum_tcr) ->
            StaticOptimizationAnswer.Yes
        | TTyconEqualsTycon(a, b) when
            isEnumTy g a
            && not (typeEquiv g a g.system_Enum_ty)
            && typeEquiv g b g.system_Enum_ty
            ->
            StaticOptimizationAnswer.Yes
        | TTyconEqualsTycon(a, b) ->
            // Both types must be nominal for a definite result
            let rec checkTypes a b =
                let a = normalizeEnumTy g (stripTyEqnsAndMeasureEqns g a)

                match a with
                | AppTy g (tcref1, _) ->
                    let b = normalizeEnumTy g (stripTyEqnsAndMeasureEqns g b)

                    match b with
                    | AppTy g (tcref2, _) ->
                        if tyconRefEq g tcref1 tcref2 && not (typeEquiv g a g.system_Enum_ty) then
                            StaticOptimizationAnswer.Yes
                        else
                            StaticOptimizationAnswer.No
                    | RefTupleTy g _
                    | FunTy g _ -> StaticOptimizationAnswer.No
                    | _ -> StaticOptimizationAnswer.Unknown

                | FunTy g _ ->
                    let b = normalizeEnumTy g (stripTyEqnsAndMeasureEqns g b)

                    match b with
                    | FunTy g _ -> StaticOptimizationAnswer.Yes
                    | AppTy g _
                    | RefTupleTy g _ -> StaticOptimizationAnswer.No
                    | _ -> StaticOptimizationAnswer.Unknown
                | RefTupleTy g ts1 ->
                    let b = normalizeEnumTy g (stripTyEqnsAndMeasureEqns g b)

                    match b with
                    | RefTupleTy g ts2 ->
                        if ts1.Length = ts2.Length then
                            StaticOptimizationAnswer.Yes
                        else
                            StaticOptimizationAnswer.No
                    | AppTy g _
                    | FunTy g _ -> StaticOptimizationAnswer.No
                    | _ -> StaticOptimizationAnswer.Unknown
                | _ -> StaticOptimizationAnswer.Unknown

            checkTypes a b
        | TTyconIsStruct a ->
            let a = normalizeEnumTy g (stripTyEqnsAndMeasureEqns g a)

            match tryTcrefOfAppTy g a with
            | ValueSome tcref1 ->
                if tcref1.IsStructOrEnumTycon then
                    StaticOptimizationAnswer.Yes
                else
                    StaticOptimizationAnswer.No
            | ValueNone -> StaticOptimizationAnswer.Unknown

    let rec DecideStaticOptimizations g cs canDecideTyparEqn =
        match cs with
        | [] -> StaticOptimizationAnswer.Yes
        | h :: t ->
            let d = decideStaticOptimizationConstraint g h canDecideTyparEqn

            if d = StaticOptimizationAnswer.No then
                StaticOptimizationAnswer.No
            elif d = StaticOptimizationAnswer.Yes then
                DecideStaticOptimizations g t canDecideTyparEqn
            else
                StaticOptimizationAnswer.Unknown

    let mkStaticOptimizationExpr g (cs, e1, e2, m) =
        let d = DecideStaticOptimizations g cs false

        if d = StaticOptimizationAnswer.No then e2
        elif d = StaticOptimizationAnswer.Yes then e1
        else Expr.StaticOptimization(cs, e1, e2, m)

    //--------------------------------------------------------------------------
    // Copy expressions, including new names for locally bound values.
    // Used to inline expressions.
    //--------------------------------------------------------------------------

    type ValCopyFlag =
        | CloneAll
        | CloneAllAndMarkExprValsAsCompilerGenerated
        | OnlyCloneExprVals

    // for quotations we do no want to avoid marking values as compiler generated since this may affect the shape of quotation (compiler generated values can be inlined)
    let fixValCopyFlagForQuotations =
        function
        | CloneAllAndMarkExprValsAsCompilerGenerated -> CloneAll
        | x -> x

    let markAsCompGen compgen d =
        let compgen =
            match compgen with
            | CloneAllAndMarkExprValsAsCompilerGenerated -> true
            | _ -> false

        { d with
            val_flags = d.val_flags.WithIsCompilerGenerated(d.val_flags.IsCompilerGenerated || compgen)
        }

    let bindLocalVal (v: Val) (v': Val) tmenv =
        { tmenv with
            valRemap = tmenv.valRemap.Add v (mkLocalValRef v')
        }

    let bindLocalVals vs vs' tmenv =
        { tmenv with
            valRemap =
                (vs, vs', tmenv.valRemap)
                |||> List.foldBack2 (fun v v' acc -> acc.Add v (mkLocalValRef v'))
        }

    let bindTycons tcs tcs' tyenv =
        { tyenv with
            tyconRefRemap =
                (tcs, tcs', tyenv.tyconRefRemap)
                |||> List.foldBack2 (fun tc tc' acc -> acc.Add (mkLocalTyconRef tc) (mkLocalTyconRef tc'))
        }

    let remapAttribKind tmenv k =
        match k with
        | ILAttrib _ as x -> x
        | FSAttrib vref -> FSAttrib(remapValRef tmenv vref)

    let tmenvCopyRemapAndBindTypars remapAttrib tmenv tps =
        let tps', tyenvinner = copyAndRemapAndBindTyparsFull remapAttrib tmenv tps
        let tmenvinner = tyenvinner
        tps', tmenvinner

    type RemapContext =
        { g: TcGlobals; stackGuard: StackGuard }

    let mkRemapContext g stackGuard = { g = g; stackGuard = stackGuard }

    let rec remapAttribImpl ctxt tmenv (Attrib(tcref, kind, args, props, isGetOrSetAttr, targets, m)) =
        Attrib(
            remapTyconRef tmenv.tyconRefRemap tcref,
            remapAttribKind tmenv kind,
            args |> List.map (remapAttribExpr ctxt tmenv),
            props
            |> List.map (fun (AttribNamedArg(nm, ty, flg, expr)) ->
                AttribNamedArg(nm, remapType tmenv ty, flg, remapAttribExpr ctxt tmenv expr)),
            isGetOrSetAttr,
            targets,
            m
        )

    and remapAttribExpr ctxt tmenv (AttribExpr(e1, e2)) =
        AttribExpr(remapExprImpl ctxt CloneAll tmenv e1, remapExprImpl ctxt CloneAll tmenv e2)

    and remapAttribs ctxt tmenv xs =
        List.map (remapAttribImpl ctxt tmenv) xs

    and remapPossibleForallTyImpl ctxt tmenv ty =
        remapTypeFull (remapAttribs ctxt tmenv) tmenv ty

    and remapArgData ctxt tmenv (argInfo: ArgReprInfo) : ArgReprInfo =
        {
            Attribs = WellKnownValAttribs.Create(remapAttribs ctxt tmenv (argInfo.Attribs.AsList()))
            Name = argInfo.Name
            OtherRange = argInfo.OtherRange
        }

    and remapValReprInfo ctxt tmenv (ValReprInfo(tpNames, arginfosl, retInfo)) =
        ValReprInfo(tpNames, List.mapSquared (remapArgData ctxt tmenv) arginfosl, remapArgData ctxt tmenv retInfo)

    and remapValData ctxt tmenv (d: ValData) =
        let ty = d.val_type
        let valReprInfo = d.ValReprInfo
        let tyR = ty |> remapPossibleForallTyImpl ctxt tmenv
        let declaringEntityR = d.TryDeclaringEntity |> remapParentRef tmenv
        let reprInfoR = d.ValReprInfo |> Option.map (remapValReprInfo ctxt tmenv)

        let memberInfoR =
            d.MemberInfo
            |> Option.map (remapMemberInfo ctxt d.val_range valReprInfo ty tyR tmenv)

        let attribsR = d.Attribs |> remapAttribs ctxt tmenv

        { d with
            val_type = tyR
            val_opt_data =
                match d.val_opt_data with
                | Some dd ->
                    Some
                        { dd with
                            val_declaring_entity = declaringEntityR
                            val_repr_info = reprInfoR
                            val_member_info = memberInfoR
                            val_attribs = WellKnownValAttribs.Create(attribsR)
                        }
                | None -> None
        }

    and remapParentRef tyenv p =
        match p with
        | ParentNone -> ParentNone
        | Parent x -> Parent(x |> remapTyconRef tyenv.tyconRefRemap)

    and mapImmediateValsAndTycons ft fv (x: ModuleOrNamespaceType) =
        let vals = x.AllValsAndMembers |> QueueList.map fv
        let tycons = x.AllEntities |> QueueList.map ft
        ModuleOrNamespaceType(x.ModuleOrNamespaceKind, vals, tycons)

    and copyVal compgen (v: Val) =
        match compgen with
        | OnlyCloneExprVals when v.IsMemberOrModuleBinding -> v
        | _ -> v |> Construct.NewModifiedVal id

    and fixupValData ctxt compgen tmenv (v2: Val) =
        // only fixup if we copy the value
        match compgen with
        | OnlyCloneExprVals when v2.IsMemberOrModuleBinding -> ()
        | _ ->
            let newData = remapValData ctxt tmenv v2 |> markAsCompGen compgen
            // uses the same stamp
            v2.SetData newData

    and copyAndRemapAndBindVals ctxt compgen tmenv vs =
        let vs2 = vs |> List.map (copyVal compgen)
        let tmenvinner = bindLocalVals vs vs2 tmenv
        vs2 |> List.iter (fixupValData ctxt compgen tmenvinner)
        vs2, tmenvinner

    and copyAndRemapAndBindVal ctxt compgen tmenv v =
        let v2 = v |> copyVal compgen
        let tmenvinner = bindLocalVal v v2 tmenv
        fixupValData ctxt compgen tmenvinner v2
        v2, tmenvinner

    and remapExprImpl (ctxt: RemapContext) (compgen: ValCopyFlag) (tmenv: Remap) expr =

        // Guard against stack overflow, moving to a whole new stack if necessary
        ctxt.stackGuard.Guard
        <| fun () ->

            match expr with

            // Handle the linear cases for arbitrary-sized inputs
            | LinearOpExpr _
            | LinearMatchExpr _
            | Expr.Sequential _
            | Expr.Let _
            | Expr.DebugPoint _ -> remapLinearExpr ctxt compgen tmenv expr id

            // Binding constructs - see also dtrees below
            | Expr.Lambda(_, ctorThisValOpt, baseValOpt, vs, b, m, bodyTy) ->
                remapLambaExpr ctxt compgen tmenv (ctorThisValOpt, baseValOpt, vs, b, m, bodyTy)

            | Expr.TyLambda(_, tps, b, m, bodyTy) ->
                let tps', tmenvinner =
                    tmenvCopyRemapAndBindTypars (remapAttribs ctxt tmenv) tmenv tps

                mkTypeLambda m tps' (remapExprImpl ctxt compgen tmenvinner b, remapType tmenvinner bodyTy)

            | Expr.TyChoose(tps, b, m) ->
                let tps', tmenvinner =
                    tmenvCopyRemapAndBindTypars (remapAttribs ctxt tmenv) tmenv tps

                Expr.TyChoose(tps', remapExprImpl ctxt compgen tmenvinner b, m)

            | Expr.LetRec(binds, e, m, _) ->
                let binds', tmenvinner = copyAndRemapAndBindBindings ctxt compgen tmenv binds
                Expr.LetRec(binds', remapExprImpl ctxt compgen tmenvinner e, m, Construct.NewFreeVarsCache())

            | Expr.Match(spBind, mExpr, pt, targets, m, ty) ->
                primMkMatch (
                    spBind,
                    mExpr,
                    remapDecisionTree ctxt compgen tmenv pt,
                    targets |> Array.map (remapTarget ctxt compgen tmenv),
                    m,
                    remapType tmenv ty
                )

            | Expr.Val(vr, vf, m) ->
                let vr' = remapValRef tmenv vr
                let vf' = remapValFlags tmenv vf

                if vr === vr' && vf === vf' then
                    expr
                else
                    Expr.Val(vr', vf', m)

            | Expr.Quote(a, dataCell, isFromQueryExpression, m, ty) ->
                remapQuoteExpr ctxt compgen tmenv (a, dataCell, isFromQueryExpression, m, ty)

            | Expr.Obj(_, ty, basev, basecall, overrides, iimpls, m) ->
                let basev', tmenvinner =
                    Option.mapFold (copyAndRemapAndBindVal ctxt compgen) tmenv basev

                mkObjExpr (
                    remapType tmenv ty,
                    basev',
                    remapExprImpl ctxt compgen tmenv basecall,
                    List.map (remapMethod ctxt compgen tmenvinner) overrides,
                    List.map (remapInterfaceImpl ctxt compgen tmenvinner) iimpls,
                    m
                )

            // Addresses of immutable field may "leak" across assembly boundaries - see CanTakeAddressOfRecdFieldRef below.
            // This is "ok", in the sense that it is always valid to fix these up to be uses
            // of a temporary local, e.g.
            //       &(E.RF) --> let mutable v = E.RF in &v

            | Expr.Op(TOp.ValFieldGetAddr(rfref, readonly), tinst, [ arg ], m) when
                not rfref.RecdField.IsMutable
                && not (entityRefInThisAssembly ctxt.g.compilingFSharpCore rfref.TyconRef)
                ->

                let tinst = remapTypes tmenv tinst
                let arg = remapExprImpl ctxt compgen tmenv arg

                let tmp, _ =
                    mkMutableCompGenLocal m WellKnownNames.CopyOfStruct (actualTyOfRecdFieldRef rfref tinst)

                mkCompGenLet m tmp (mkRecdFieldGetViaExprAddr (arg, rfref, tinst, m)) (mkValAddr m readonly (mkLocalValRef tmp))

            | Expr.Op(TOp.UnionCaseFieldGetAddr(uref, cidx, readonly), tinst, [ arg ], m) when
                not (uref.FieldByIndex(cidx).IsMutable)
                && not (entityRefInThisAssembly ctxt.g.compilingFSharpCore uref.TyconRef)
                ->

                let tinst = remapTypes tmenv tinst
                let arg = remapExprImpl ctxt compgen tmenv arg

                let tmp, _ =
                    mkMutableCompGenLocal m WellKnownNames.CopyOfStruct (actualTyOfUnionFieldRef uref cidx tinst)

                mkCompGenLet
                    m
                    tmp
                    (mkUnionCaseFieldGetProvenViaExprAddr (arg, uref, tinst, cidx, m))
                    (mkValAddr m readonly (mkLocalValRef tmp))

            | Expr.Op(op, tinst, args, m) -> remapOpExpr ctxt compgen tmenv (op, tinst, args, m) expr

            | Expr.App(e1, e1ty, tyargs, args, m) -> remapAppExpr ctxt compgen tmenv (e1, e1ty, tyargs, args, m) expr

            | Expr.Link eref -> remapExprImpl ctxt compgen tmenv eref.Value

            | Expr.StaticOptimization(cs, e2, e3, m) ->
                // note that type instantiation typically resolve the static constraints here
                mkStaticOptimizationExpr
                    ctxt.g
                    (List.map (remapConstraint tmenv) cs, remapExprImpl ctxt compgen tmenv e2, remapExprImpl ctxt compgen tmenv e3, m)

            | Expr.Const(c, m, ty) ->
                let ty' = remapType tmenv ty
                if ty === ty' then expr else Expr.Const(c, m, ty')

            | Expr.WitnessArg(traitInfo, m) ->
                let traitInfoR = remapTraitInfo tmenv traitInfo
                Expr.WitnessArg(traitInfoR, m)

    and remapLambaExpr (ctxt: RemapContext) (compgen: ValCopyFlag) (tmenv: Remap) (ctorThisValOpt, baseValOpt, vs, body, m, bodyTy) =
        let ctorThisValOptR, tmenv =
            Option.mapFold (copyAndRemapAndBindVal ctxt compgen) tmenv ctorThisValOpt

        let baseValOptR, tmenv =
            Option.mapFold (copyAndRemapAndBindVal ctxt compgen) tmenv baseValOpt

        let vsR, tmenv = copyAndRemapAndBindVals ctxt compgen tmenv vs
        let bodyR = remapExprImpl ctxt compgen tmenv body
        let bodyTyR = remapType tmenv bodyTy
        Expr.Lambda(newUnique (), ctorThisValOptR, baseValOptR, vsR, bodyR, m, bodyTyR)

    and remapQuoteExpr (ctxt: RemapContext) (compgen: ValCopyFlag) (tmenv: Remap) (a, dataCell, isFromQueryExpression, m, ty) =
        let doData (typeDefs, argTypes, argExprs, res) =
            (typeDefs, remapTypesAux tmenv argTypes, remapExprs ctxt compgen tmenv argExprs, res)

        let data' =
            match dataCell.Value with
            | None -> None
            | Some(data1, data2) -> Some(doData data1, doData data2)
        // fix value of compgen for both original expression and pickled AST
        let compgen = fixValCopyFlagForQuotations compgen
        Expr.Quote(remapExprImpl ctxt compgen tmenv a, ref data', isFromQueryExpression, m, remapType tmenv ty)

    and remapOpExpr (ctxt: RemapContext) (compgen: ValCopyFlag) (tmenv: Remap) (op, tinst, args, m) origExpr =
        let opR = remapOp tmenv op
        let tinstR = remapTypes tmenv tinst
        let argsR = remapExprs ctxt compgen tmenv args

        if op === opR && tinst === tinstR && args === argsR then
            origExpr
        else
            Expr.Op(opR, tinstR, argsR, m)

    and remapAppExpr (ctxt: RemapContext) (compgen: ValCopyFlag) (tmenv: Remap) (e1, e1ty, tyargs, args, m) origExpr =
        let e1R = remapExprImpl ctxt compgen tmenv e1
        let e1tyR = remapPossibleForallTyImpl ctxt tmenv e1ty
        let tyargsR = remapTypes tmenv tyargs
        let argsR = remapExprs ctxt compgen tmenv args

        if e1 === e1R && e1ty === e1tyR && tyargs === tyargsR && args === argsR then
            origExpr
        else
            Expr.App(e1R, e1tyR, tyargsR, argsR, m)

    and remapTarget ctxt compgen tmenv (TTarget(vs, e, flags)) =
        let vsR, tmenvinner = copyAndRemapAndBindVals ctxt compgen tmenv vs
        TTarget(vsR, remapExprImpl ctxt compgen tmenvinner e, flags)

    and remapLinearExpr ctxt compgen tmenv expr contf =

        match expr with

        | Expr.Let(bind, bodyExpr, m, _) ->
            let bindR, tmenvinner = copyAndRemapAndBindBinding ctxt compgen tmenv bind
            // tailcall for the linear position
            remapLinearExpr ctxt compgen tmenvinner bodyExpr (contf << mkLetBind m bindR)

        | Expr.Sequential(expr1, expr2, dir, m) ->
            let expr1R = remapExprImpl ctxt compgen tmenv expr1
            // tailcall for the linear position
            remapLinearExpr
                ctxt
                compgen
                tmenv
                expr2
                (contf
                 << (fun expr2R ->
                     if expr1 === expr1R && expr2 === expr2R then
                         expr
                     else
                         Expr.Sequential(expr1R, expr2R, dir, m)))

        | LinearMatchExpr(spBind, mExpr, dtree, tg1, expr2, m2, ty) ->
            let dtreeR = remapDecisionTree ctxt compgen tmenv dtree
            let tg1R = remapTarget ctxt compgen tmenv tg1
            let tyR = remapType tmenv ty
            // tailcall for the linear position
            remapLinearExpr
                ctxt
                compgen
                tmenv
                expr2
                (contf
                 << (fun expr2R -> rebuildLinearMatchExpr (spBind, mExpr, dtreeR, tg1R, expr2R, m2, tyR)))

        | LinearOpExpr(op, tyargs, argsFront, argLast, m) ->
            let opR = remapOp tmenv op
            let tinstR = remapTypes tmenv tyargs
            let argsFrontR = remapExprs ctxt compgen tmenv argsFront
            // tailcall for the linear position
            remapLinearExpr
                ctxt
                compgen
                tmenv
                argLast
                (contf
                 << (fun argLastR ->
                     if
                         op === opR
                         && tyargs === tinstR
                         && argsFront === argsFrontR
                         && argLast === argLastR
                     then
                         expr
                     else
                         rebuildLinearOpExpr (opR, tinstR, argsFrontR, argLastR, m)))

        | Expr.DebugPoint(dpm, innerExpr) ->
            remapLinearExpr ctxt compgen tmenv innerExpr (contf << (fun innerExprR -> Expr.DebugPoint(dpm, innerExprR)))

        | _ -> contf (remapExprImpl ctxt compgen tmenv expr)

    and remapConstraint tyenv c =
        match c with
        | TTyconEqualsTycon(ty1, ty2) -> TTyconEqualsTycon(remapType tyenv ty1, remapType tyenv ty2)
        | TTyconIsStruct ty1 -> TTyconIsStruct(remapType tyenv ty1)

    and remapOp tmenv op =
        match op with
        | TOp.Recd(ctor, tcref) -> TOp.Recd(ctor, remapTyconRef tmenv.tyconRefRemap tcref)
        | TOp.UnionCaseTagGet tcref -> TOp.UnionCaseTagGet(remapTyconRef tmenv.tyconRefRemap tcref)
        | TOp.UnionCase ucref -> TOp.UnionCase(remapUnionCaseRef tmenv.tyconRefRemap ucref)
        | TOp.UnionCaseProof ucref -> TOp.UnionCaseProof(remapUnionCaseRef tmenv.tyconRefRemap ucref)
        | TOp.ExnConstr ec -> TOp.ExnConstr(remapTyconRef tmenv.tyconRefRemap ec)
        | TOp.ExnFieldGet(ec, n) -> TOp.ExnFieldGet(remapTyconRef tmenv.tyconRefRemap ec, n)
        | TOp.ExnFieldSet(ec, n) -> TOp.ExnFieldSet(remapTyconRef tmenv.tyconRefRemap ec, n)
        | TOp.ValFieldSet rfref -> TOp.ValFieldSet(remapRecdFieldRef tmenv.tyconRefRemap rfref)
        | TOp.ValFieldGet rfref -> TOp.ValFieldGet(remapRecdFieldRef tmenv.tyconRefRemap rfref)
        | TOp.ValFieldGetAddr(rfref, readonly) -> TOp.ValFieldGetAddr(remapRecdFieldRef tmenv.tyconRefRemap rfref, readonly)
        | TOp.UnionCaseFieldGet(ucref, n) -> TOp.UnionCaseFieldGet(remapUnionCaseRef tmenv.tyconRefRemap ucref, n)
        | TOp.UnionCaseFieldGetAddr(ucref, n, readonly) ->
            TOp.UnionCaseFieldGetAddr(remapUnionCaseRef tmenv.tyconRefRemap ucref, n, readonly)
        | TOp.UnionCaseFieldSet(ucref, n) -> TOp.UnionCaseFieldSet(remapUnionCaseRef tmenv.tyconRefRemap ucref, n)
        | TOp.ILAsm(instrs, retTypes) ->
            let retTypes2 = remapTypes tmenv retTypes

            if retTypes === retTypes2 then
                op
            else
                TOp.ILAsm(instrs, retTypes2)
        | TOp.TraitCall traitInfo -> TOp.TraitCall(remapTraitInfo tmenv traitInfo)
        | TOp.LValueOp(kind, lvr) -> TOp.LValueOp(kind, remapValRef tmenv lvr)
        | TOp.ILCall(isVirtual,
                     isProtected,
                     isStruct,
                     isCtor,
                     valUseFlag,
                     isProperty,
                     noTailCall,
                     ilMethRef,
                     enclTypeInst,
                     methInst,
                     retTypes) ->
            TOp.ILCall(
                isVirtual,
                isProtected,
                isStruct,
                isCtor,
                remapValFlags tmenv valUseFlag,
                isProperty,
                noTailCall,
                ilMethRef,
                remapTypes tmenv enclTypeInst,
                remapTypes tmenv methInst,
                remapTypes tmenv retTypes
            )
        | _ -> op

    and remapValFlags tmenv x =
        match x with
        | PossibleConstrainedCall ty -> PossibleConstrainedCall(remapType tmenv ty)
        | _ -> x

    and remapExprs ctxt compgen tmenv es =
        List.mapq (remapExprImpl ctxt compgen tmenv) es

    and remapFlatExprs ctxt compgen tmenv es =
        List.mapq (remapExprImpl ctxt compgen tmenv) es

    and remapDecisionTree ctxt compgen tmenv x =
        match x with
        | TDSwitch(e1, cases, dflt, m) ->
            let e1R = remapExprImpl ctxt compgen tmenv e1

            let casesR =
                cases
                |> List.map (fun (TCase(test, subTree)) ->
                    let testR =
                        match test with
                        | DecisionTreeTest.UnionCase(uc, tinst) ->
                            DecisionTreeTest.UnionCase(remapUnionCaseRef tmenv.tyconRefRemap uc, remapTypes tmenv tinst)
                        | DecisionTreeTest.ArrayLength(n, ty) -> DecisionTreeTest.ArrayLength(n, remapType tmenv ty)
                        | DecisionTreeTest.Const _ -> test
                        | DecisionTreeTest.IsInst(srcTy, tgtTy) -> DecisionTreeTest.IsInst(remapType tmenv srcTy, remapType tmenv tgtTy)
                        | DecisionTreeTest.IsNull -> DecisionTreeTest.IsNull
                        | DecisionTreeTest.ActivePatternCase _ ->
                            failwith "DecisionTreeTest.ActivePatternCase should only be used during pattern match compilation"
                        | DecisionTreeTest.Error(m) -> DecisionTreeTest.Error(m)

                    let subTreeR = remapDecisionTree ctxt compgen tmenv subTree
                    TCase(testR, subTreeR))

            let dfltR = Option.map (remapDecisionTree ctxt compgen tmenv) dflt
            TDSwitch(e1R, casesR, dfltR, m)

        | TDSuccess(es, n) -> TDSuccess(remapFlatExprs ctxt compgen tmenv es, n)

        | TDBind(bind, rest) ->
            let bindR, tmenvinner = copyAndRemapAndBindBinding ctxt compgen tmenv bind
            TDBind(bindR, remapDecisionTree ctxt compgen tmenvinner rest)

    and copyAndRemapAndBindBinding ctxt compgen tmenv (bind: Binding) =
        let v = bind.Var
        let vR, tmenv = copyAndRemapAndBindVal ctxt compgen tmenv v
        remapAndRenameBind ctxt compgen tmenv bind vR, tmenv

    and copyAndRemapAndBindBindings ctxt compgen tmenv binds =
        let vsR, tmenvinner = copyAndRemapAndBindVals ctxt compgen tmenv (valsOfBinds binds)
        remapAndRenameBinds ctxt compgen tmenvinner binds vsR, tmenvinner

    and remapAndRenameBinds ctxt compgen tmenvinner binds vsR =
        List.map2 (remapAndRenameBind ctxt compgen tmenvinner) binds vsR

    and remapAndRenameBind ctxt compgen tmenvinner (TBind(_, repr, letSeqPtOpt)) vR =
        TBind(vR, remapExprImpl ctxt compgen tmenvinner repr, letSeqPtOpt)

    and remapMethod ctxt compgen tmenv (TObjExprMethod(slotsig, attribs, tps, vs, e, m)) =
        let attribs2 = attribs |> remapAttribs ctxt tmenv
        let slotsig2 = remapSlotSig (remapAttribs ctxt tmenv) tmenv slotsig

        let tps2, tmenvinner =
            tmenvCopyRemapAndBindTypars (remapAttribs ctxt tmenv) tmenv tps

        let vs2, tmenvinner2 =
            List.mapFold (copyAndRemapAndBindVals ctxt compgen) tmenvinner vs

        let e2 = remapExprImpl ctxt compgen tmenvinner2 e
        TObjExprMethod(slotsig2, attribs2, tps2, vs2, e2, m)

    and remapInterfaceImpl ctxt compgen tmenv (ty, overrides) =
        (remapType tmenv ty, List.map (remapMethod ctxt compgen tmenv) overrides)

    and remapRecdField ctxt tmenv x =
        { x with
            rfield_type = x.rfield_type |> remapPossibleForallTyImpl ctxt tmenv
            rfield_pattribs = x.rfield_pattribs |> remapAttribs ctxt tmenv
            rfield_fattribs = x.rfield_fattribs |> remapAttribs ctxt tmenv
        }

    and remapRecdFields ctxt tmenv (x: TyconRecdFields) =
        x.AllFieldsAsList
        |> List.map (remapRecdField ctxt tmenv)
        |> Construct.MakeRecdFieldsTable

    and remapUnionCase ctxt tmenv (x: UnionCase) =
        { x with
            FieldTable = x.FieldTable |> remapRecdFields ctxt tmenv
            ReturnType = x.ReturnType |> remapType tmenv
            Attribs = x.Attribs |> remapAttribs ctxt tmenv
        }

    and remapUnionCases ctxt tmenv (x: TyconUnionData) =
        x.UnionCasesAsList
        |> List.map (remapUnionCase ctxt tmenv)
        |> Construct.MakeUnionCases

    and remapFsObjData ctxt tmenv x =
        {
            fsobjmodel_cases = remapUnionCases ctxt tmenv x.fsobjmodel_cases
            fsobjmodel_kind =
                (match x.fsobjmodel_kind with
                 | TFSharpDelegate slotsig -> TFSharpDelegate(remapSlotSig (remapAttribs ctxt tmenv) tmenv slotsig)
                 | _ -> x.fsobjmodel_kind)
            fsobjmodel_vslots = x.fsobjmodel_vslots |> List.map (remapValRef tmenv)
            fsobjmodel_rfields = x.fsobjmodel_rfields |> remapRecdFields ctxt tmenv
        }

    and remapTyconRepr ctxt tmenv repr =
        match repr with
        | TFSharpTyconRepr x -> TFSharpTyconRepr(remapFsObjData ctxt tmenv x)
        | TILObjectRepr _ -> failwith "cannot remap IL type definitions"
#if !NO_TYPEPROVIDERS
        | TProvidedNamespaceRepr _ -> repr
        | TProvidedTypeRepr info ->
            TProvidedTypeRepr
                { info with
                    LazyBaseType =
                        info.LazyBaseType.Force(range0, ctxt.g.obj_ty_withNulls)
                        |> remapType tmenv
                        |> LazyWithContext.NotLazy
                    // The load context for the provided type contains TyconRef objects. We must remap these.
                    // This is actually done on-demand (see the implementation of ProvidedTypeContext)
                    ProvidedType =
                        info.ProvidedType.PApplyNoFailure(fun st ->
                            let ctxt =
                                st.Context.RemapTyconRefs(unbox >> remapTyconRef tmenv.tyconRefRemap >> box >> (!!))

                            ProvidedType.ApplyContext(st, ctxt))
                }
#endif
        | TNoRepr -> repr
        | TAsmRepr _ -> repr
        | TMeasureableRepr x -> TMeasureableRepr(remapType tmenv x)

    and remapTyconAug tmenv (x: TyconAugmentation) =
        { x with
            tcaug_equals = x.tcaug_equals |> Option.map (mapPair (remapValRef tmenv, remapValRef tmenv))
            tcaug_compare = x.tcaug_compare |> Option.map (mapPair (remapValRef tmenv, remapValRef tmenv))
            tcaug_compare_withc = x.tcaug_compare_withc |> Option.map (remapValRef tmenv)
            tcaug_hash_and_equals_withc =
                x.tcaug_hash_and_equals_withc
                |> Option.map (mapQuadruple (remapValRef tmenv, remapValRef tmenv, remapValRef tmenv, Option.map (remapValRef tmenv)))
            tcaug_adhoc = x.tcaug_adhoc |> NameMap.map (List.map (remapValRef tmenv))
            tcaug_adhoc_list =
                x.tcaug_adhoc_list
                |> ResizeArray.map (fun (flag, vref) -> (flag, remapValRef tmenv vref))
            tcaug_super = x.tcaug_super |> Option.map (remapType tmenv)
            tcaug_interfaces = x.tcaug_interfaces |> List.map (map1Of3 (remapType tmenv))
        }

    and remapTyconExnInfo ctxt tmenv inp =
        match inp with
        | TExnAbbrevRepr x -> TExnAbbrevRepr(remapTyconRef tmenv.tyconRefRemap x)
        | TExnFresh x -> TExnFresh(remapRecdFields ctxt tmenv x)
        | TExnAsmRepr _
        | TExnNone -> inp

    and remapMemberInfo ctxt m valReprInfo ty tyR tmenv x =
        // The slotsig in the ImplementedSlotSigs is w.r.t. the type variables in the value's type.
        // REVIEW: this is a bit gross. It would be nice if the slotsig was standalone
        assert (Option.isSome valReprInfo)

        let tpsorig, _, _, _ =
            GetMemberTypeInFSharpForm ctxt.g x.MemberFlags (Option.get valReprInfo) ty m

        let tps, _, _, _ =
            GetMemberTypeInFSharpForm ctxt.g x.MemberFlags (Option.get valReprInfo) tyR m

        let renaming, _ = mkTyparToTyparRenaming tpsorig tps

        let tmenv =
            { tmenv with
                tpinst = tmenv.tpinst @ renaming
            }

        { x with
            ApparentEnclosingEntity = x.ApparentEnclosingEntity |> remapTyconRef tmenv.tyconRefRemap
            ImplementedSlotSigs = x.ImplementedSlotSigs |> List.map (remapSlotSig (remapAttribs ctxt tmenv) tmenv)
        }

    and copyAndRemapAndBindModTy ctxt compgen tmenv mty =
        let tycons = allEntitiesOfModuleOrNamespaceTy mty
        let vs = allValsOfModuleOrNamespaceTy mty
        let _, _, tmenvinner = copyAndRemapAndBindTyconsAndVals ctxt compgen tmenv tycons vs
        (mapImmediateValsAndTycons (renameTycon tmenvinner) (renameVal tmenvinner) mty), tmenvinner

    and renameTycon tyenv x =
        let tcref =
            try
                let res = tyenv.tyconRefRemap[mkLocalTyconRef x]
                res
            with :? KeyNotFoundException ->
                errorR (InternalError("couldn't remap internal tycon " + showL (DebugPrint.tyconL x), x.Range))
                mkLocalTyconRef x

        tcref.Deref

    and renameVal tmenv x =
        match tmenv.valRemap.TryFind x with
        | Some v -> v.Deref
        | None -> x

    and copyTycon compgen (tycon: Tycon) =
        match compgen with
        | OnlyCloneExprVals -> tycon
        | _ -> Construct.NewClonedTycon tycon

    /// This operates over a whole nested collection of tycons and vals simultaneously *)
    and copyAndRemapAndBindTyconsAndVals ctxt compgen tmenv tycons vs =
        let tyconsR = tycons |> List.map (copyTycon compgen)

        let tmenvinner = bindTycons tycons tyconsR tmenv

        // Values need to be copied and renamed.
        let vsR, tmenvinner = copyAndRemapAndBindVals ctxt compgen tmenvinner vs

        // "if a type constructor is hidden then all its inner values and inner type constructors must also be hidden"
        // Hence we can just lookup the inner tycon/value mappings in the tables.

        let lookupVal (v: Val) =
            let vref =
                try
                    let res = tmenvinner.valRemap[v]
                    res
                with :? KeyNotFoundException ->
                    errorR (InternalError(sprintf "couldn't remap internal value '%s'" v.LogicalName, v.Range))
                    mkLocalValRef v

            vref.Deref

        let lookupTycon tycon =
            let tcref =
                try
                    let res = tmenvinner.tyconRefRemap[mkLocalTyconRef tycon]
                    res
                with :? KeyNotFoundException ->
                    errorR (InternalError("couldn't remap internal tycon " + showL (DebugPrint.tyconL tycon), tycon.Range))
                    mkLocalTyconRef tycon

            tcref.Deref

        (tycons, tyconsR)
        ||> List.iter2 (fun tcd tcdR ->
            let lookupTycon tycon = lookupTycon tycon

            let tpsR, tmenvinner2 =
                tmenvCopyRemapAndBindTypars (remapAttribs ctxt tmenvinner) tmenvinner (tcd.entity_typars.Force(tcd.entity_range))

            tcdR.entity_typars <- LazyWithContext.NotLazy tpsR
            tcdR.entity_attribs <- WellKnownEntityAttribs.Create(tcd.entity_attribs.AsList() |> remapAttribs ctxt tmenvinner2)
            tcdR.entity_tycon_repr <- tcd.entity_tycon_repr |> remapTyconRepr ctxt tmenvinner2
            let typeAbbrevR = tcd.TypeAbbrev |> Option.map (remapType tmenvinner2)
            tcdR.entity_tycon_tcaug <- tcd.entity_tycon_tcaug |> remapTyconAug tmenvinner2
            tcdR.entity_modul_type <- MaybeLazy.Strict(tcd.entity_modul_type.Value |> mapImmediateValsAndTycons lookupTycon lookupVal)
            let exnInfoR = tcd.ExceptionInfo |> remapTyconExnInfo ctxt tmenvinner2

            match tcdR.entity_opt_data with
            | Some optData ->
                tcdR.entity_opt_data <-
                    Some
                        { optData with
                            entity_tycon_abbrev = typeAbbrevR
                            entity_exn_info = exnInfoR
                        }
            | _ ->
                tcdR.SetTypeAbbrev typeAbbrevR
                tcdR.SetExceptionInfo exnInfoR)

        tyconsR, vsR, tmenvinner

    and allTyconsOfTycon (tycon: Tycon) =
        seq {
            yield tycon

            for nestedTycon in tycon.ModuleOrNamespaceType.AllEntities do
                yield! allTyconsOfTycon nestedTycon
        }

    and allEntitiesOfModDef mdef =
        seq {
            match mdef with
            | TMDefRec(_, _, tycons, mbinds, _) ->
                for tycon in tycons do
                    yield! allTyconsOfTycon tycon

                for mbind in mbinds do
                    match mbind with
                    | ModuleOrNamespaceBinding.Binding _ -> ()
                    | ModuleOrNamespaceBinding.Module(mspec, def) ->
                        yield mspec
                        yield! allEntitiesOfModDef def
            | TMDefLet _ -> ()
            | TMDefDo _ -> ()
            | TMDefOpens _ -> ()
            | TMDefs defs ->
                for def in defs do
                    yield! allEntitiesOfModDef def
        }

    and allValsOfModDefWithOption processNested mdef =
        seq {
            match mdef with
            | TMDefRec(_, _, tycons, mbinds, _) ->
                yield! abstractSlotValsOfTycons tycons

                for mbind in mbinds do
                    match mbind with
                    | ModuleOrNamespaceBinding.Binding bind -> yield bind.Var
                    | ModuleOrNamespaceBinding.Module(_, def) ->
                        if processNested then
                            yield! allValsOfModDefWithOption processNested def
            | TMDefLet(bind, _) -> yield bind.Var
            | TMDefDo _ -> ()
            | TMDefOpens _ -> ()
            | TMDefs defs ->
                for def in defs do
                    yield! allValsOfModDefWithOption processNested def
        }

    and allValsOfModDef mdef = allValsOfModDefWithOption true mdef

    and allTopLevelValsOfModDef mdef = allValsOfModDefWithOption false mdef

    and copyAndRemapModDef ctxt compgen tmenv mdef =
        let tycons = allEntitiesOfModDef mdef |> List.ofSeq
        let vs = allValsOfModDef mdef |> List.ofSeq
        let _, _, tmenvinner = copyAndRemapAndBindTyconsAndVals ctxt compgen tmenv tycons vs
        remapAndRenameModDef ctxt compgen tmenvinner mdef

    and remapAndRenameModDefs ctxt compgen tmenv x =
        List.map (remapAndRenameModDef ctxt compgen tmenv) x

    and remapOpenDeclarations tmenv opens =
        opens
        |> List.map (fun od ->
            { od with
                Modules = od.Modules |> List.map (remapTyconRef tmenv.tyconRefRemap)
                Types = od.Types |> List.map (remapType tmenv)
            })

    and remapAndRenameModDef ctxt compgen tmenv mdef =
        match mdef with
        | TMDefRec(isRec, opens, tycons, mbinds, m) ->
            // Abstract (virtual) vslots in the tycons at TMDefRec nodes are binders. They also need to be copied and renamed.
            let opensR = remapOpenDeclarations tmenv opens
            let tyconsR = tycons |> List.map (renameTycon tmenv)
            let mbindsR = mbinds |> List.map (remapAndRenameModBind ctxt compgen tmenv)
            TMDefRec(isRec, opensR, tyconsR, mbindsR, m)
        | TMDefLet(bind, m) ->
            let v = bind.Var
            let bind = remapAndRenameBind ctxt compgen tmenv bind (renameVal tmenv v)
            TMDefLet(bind, m)
        | TMDefDo(e, m) ->
            let e = remapExprImpl ctxt compgen tmenv e
            TMDefDo(e, m)
        | TMDefOpens opens ->
            let opens = remapOpenDeclarations tmenv opens
            TMDefOpens opens
        | TMDefs defs ->
            let defs = remapAndRenameModDefs ctxt compgen tmenv defs
            TMDefs defs

    and remapAndRenameModBind ctxt compgen tmenv x =
        match x with
        | ModuleOrNamespaceBinding.Binding bind ->
            let v2 = bind |> valOfBind |> renameVal tmenv
            let bind2 = remapAndRenameBind ctxt compgen tmenv bind v2
            ModuleOrNamespaceBinding.Binding bind2
        | ModuleOrNamespaceBinding.Module(mspec, def) ->
            let mspec = renameTycon tmenv mspec
            let def = remapAndRenameModDef ctxt compgen tmenv def
            ModuleOrNamespaceBinding.Module(mspec, def)

    and remapImplFile ctxt compgen tmenv implFile =
        let (CheckedImplFile(fragName, signature, contents, hasExplicitEntryPoint, isScript, anonRecdTypes, namedDebugPointsForInlinedCode)) =
            implFile

        let contentsR = copyAndRemapModDef ctxt compgen tmenv contents
        let signatureR, tmenv = copyAndRemapAndBindModTy ctxt compgen tmenv signature

        let implFileR =
            CheckedImplFile(fragName, signatureR, contentsR, hasExplicitEntryPoint, isScript, anonRecdTypes, namedDebugPointsForInlinedCode)

        implFileR, tmenv

    // Entry points

    let remapAttrib g tmenv attrib =
        let ctxt =
            {
                g = g
                stackGuard = StackGuard("RemapExprStackGuardDepth")
            }

        remapAttribImpl ctxt tmenv attrib

    let remapExpr g (compgen: ValCopyFlag) (tmenv: Remap) expr =
        let ctxt =
            {
                g = g
                stackGuard = StackGuard("RemapExprStackGuardDepth")
            }

        remapExprImpl ctxt compgen tmenv expr

    let remapPossibleForallTy g tmenv ty =
        let ctxt =
            {
                g = g
                stackGuard = StackGuard("RemapExprStackGuardDepth")
            }

        remapPossibleForallTyImpl ctxt tmenv ty

    let copyModuleOrNamespaceType g compgen mtyp =
        let ctxt =
            {
                g = g
                stackGuard = StackGuard("RemapExprStackGuardDepth")
            }

        copyAndRemapAndBindModTy ctxt compgen Remap.Empty mtyp |> fst

    let copyExpr g compgen e =
        let ctxt =
            {
                g = g
                stackGuard = StackGuard("RemapExprStackGuardDepth")
            }

        remapExprImpl ctxt compgen Remap.Empty e

    let copyImplFile g compgen e =
        let ctxt =
            {
                g = g
                stackGuard = StackGuard("RemapExprStackGuardDepth")
            }

        remapImplFile ctxt compgen Remap.Empty e |> fst

    let instExpr g tpinst e =
        let ctxt =
            {
                g = g
                stackGuard = StackGuard("RemapExprStackGuardDepth")
            }

        remapExprImpl ctxt CloneAll (mkInstRemap tpinst) e

[<AutoOpen>]
module internal ExprAnalysis =

    //--------------------------------------------------------------------------
    // Replace Marks - adjust debugging marks when a lambda gets
    // eliminated (i.e. an expression gets inlined)
    //--------------------------------------------------------------------------

    let rec remarkExpr (m: range) x =
        match x with
        | Expr.Lambda(uniq, ctorThisValOpt, baseValOpt, vs, b, _, bodyTy) ->
            Expr.Lambda(uniq, ctorThisValOpt, baseValOpt, vs, remarkExpr m b, m, bodyTy)

        | Expr.TyLambda(uniq, tps, b, _, bodyTy) -> Expr.TyLambda(uniq, tps, remarkExpr m b, m, bodyTy)

        | Expr.TyChoose(tps, b, _) -> Expr.TyChoose(tps, remarkExpr m b, m)

        | Expr.LetRec(binds, e, _, fvs) -> Expr.LetRec(remarkBinds m binds, remarkExpr m e, m, fvs)

        | Expr.Let(bind, e, _, fvs) -> Expr.Let(remarkBind m bind, remarkExpr m e, m, fvs)

        | Expr.Match(_, _, pt, targets, _, ty) ->
            let targetsR =
                targets
                |> Array.map (fun (TTarget(vs, e, flags)) -> TTarget(vs, remarkExpr m e, flags))

            primMkMatch (DebugPointAtBinding.NoneAtInvisible, m, remarkDecisionTree m pt, targetsR, m, ty)

        | Expr.Val(x, valUseFlags, _) -> Expr.Val(x, valUseFlags, m)

        | Expr.Quote(a, conv, isFromQueryExpression, _, ty) -> Expr.Quote(remarkExpr m a, conv, isFromQueryExpression, m, ty)

        | Expr.Obj(n, ty, basev, basecall, overrides, iimpls, _) ->
            Expr.Obj(
                n,
                ty,
                basev,
                remarkExpr m basecall,
                List.map (remarkObjExprMethod m) overrides,
                List.map (remarkInterfaceImpl m) iimpls,
                m
            )

        | Expr.Op(op, tinst, args, _) ->

            // This code allows a feature where if a 'while'/'for' etc in a computation expression is
            // implemented using code inlining and is ultimately implemented by a corresponding construct somewhere
            // in the remark'd code then at least one debug point is recovered, based on the noted debug point for the original construct.
            //
            // However it is imperfect, since only one debug point is recovered
            let op =
                match op with
                | TOp.IntegerForLoop(_, _, style) -> TOp.IntegerForLoop(DebugPointAtFor.No, DebugPointAtInOrTo.No, style)
                | TOp.While(_, marker) -> TOp.While(DebugPointAtWhile.No, marker)
                | TOp.TryFinally _ -> TOp.TryFinally(DebugPointAtTry.No, DebugPointAtFinally.No)
                | TOp.TryWith _ -> TOp.TryWith(DebugPointAtTry.No, DebugPointAtWith.No)
                | _ -> op

            Expr.Op(op, tinst, remarkExprs m args, m)

        | Expr.Link eref ->
            // Preserve identity of fixup nodes during remarkExpr
            eref.Value <- remarkExpr m eref.Value
            x

        | Expr.App(e1, e1ty, tyargs, args, _) -> Expr.App(remarkExpr m e1, e1ty, tyargs, remarkExprs m args, m)

        | Expr.Sequential(e1, e2, dir, _) ->
            let e1R = remarkExpr m e1
            let e2R = remarkExpr m e2
            Expr.Sequential(e1R, e2R, dir, m)

        | Expr.StaticOptimization(eqns, e2, e3, _) -> Expr.StaticOptimization(eqns, remarkExpr m e2, remarkExpr m e3, m)

        | Expr.Const(c, _, ty) -> Expr.Const(c, m, ty)

        | Expr.WitnessArg(witnessInfo, _) -> Expr.WitnessArg(witnessInfo, m)

        | Expr.DebugPoint(_, innerExpr) -> remarkExpr m innerExpr

    and remarkObjExprMethod m (TObjExprMethod(slotsig, attribs, tps, vs, e, _)) =
        TObjExprMethod(slotsig, attribs, tps, vs, remarkExpr m e, m)

    and remarkInterfaceImpl m (ty, overrides) =
        (ty, List.map (remarkObjExprMethod m) overrides)

    and remarkExprs m es = es |> List.map (remarkExpr m)

    and remarkDecisionTree m x =
        match x with
        | TDSwitch(e1, cases, dflt, _) ->
            let e1R = remarkExpr m e1

            let casesR =
                cases |> List.map (fun (TCase(test, y)) -> TCase(test, remarkDecisionTree m y))

            let dfltR = Option.map (remarkDecisionTree m) dflt
            TDSwitch(e1R, casesR, dfltR, m)
        | TDSuccess(es, n) -> TDSuccess(remarkExprs m es, n)
        | TDBind(bind, rest) -> TDBind(remarkBind m bind, remarkDecisionTree m rest)

    and remarkBinds m binds = List.map (remarkBind m) binds

    // This very deliberately drops the sequence points since this is used when adjusting the marks for inlined expressions
    and remarkBind m (TBind(v, repr, _)) =
        TBind(v, remarkExpr m repr, DebugPointAtBinding.NoneAtSticky)

    //--------------------------------------------------------------------------
    // Mutability analysis
    //--------------------------------------------------------------------------

    let isRecdOrStructFieldDefinitelyMutable (f: RecdField) = not f.IsStatic && f.IsMutable

    let isUnionCaseDefinitelyMutable (uc: UnionCase) =
        uc.FieldTable.FieldsByIndex |> Array.exists isRecdOrStructFieldDefinitelyMutable

    let isUnionCaseRefDefinitelyMutable (uc: UnionCaseRef) =
        uc.UnionCase |> isUnionCaseDefinitelyMutable

    /// This is an incomplete check for .NET struct types. Returning 'false' doesn't mean the thing is immutable.
    let isRecdOrUnionOrStructTyconRefDefinitelyMutable (tcref: TyconRef) =
        let tycon = tcref.Deref

        if tycon.IsUnionTycon then
            tycon.UnionCasesArray |> Array.exists isUnionCaseDefinitelyMutable
        elif tycon.IsRecordTycon || tycon.IsStructOrEnumTycon then
            // Note: This only looks at the F# fields, causing oddities.
            // See https://github.com/dotnet/fsharp/pull/4576
            tycon.AllFieldsArray |> Array.exists isRecdOrStructFieldDefinitelyMutable
        else
            false

    // Although from the pure F# perspective exception values cannot be changed, the .NET
    // implementation of exception objects attaches a whole bunch of stack information to
    // each raised object. Hence we treat exception objects as if they have identity
    let isExnDefinitelyMutable (_ecref: TyconRef) = true

    // Some of the implementations of library functions on lists use mutation on the tail
    // of the cons cell. These cells are always private, i.e. not accessible by any other
    // code until the construction of the entire return list has been completed.
    // However, within the implementation code reads of the tail cell must in theory be treated
    // with caution. Hence we are conservative and within FSharp.Core we don't treat list
    // reads as if they were pure.
    let isUnionCaseFieldMutable (g: TcGlobals) (ucref: UnionCaseRef) n =
        (g.compilingFSharpCore && tyconRefEq g ucref.TyconRef g.list_tcr_canon && n = 1)
        || (ucref.FieldByIndex n).IsMutable

    let isExnFieldMutable ecref n =
        if n < 0 || n >= List.length (recdFieldsOfExnDefRef ecref) then
            errorR (InternalError(sprintf "isExnFieldMutable, exnc = %s, n = %d" ecref.LogicalName n, ecref.Range))

        (recdFieldOfExnDefRefByIdx ecref n).IsMutable

    //---------------------------------------------------------------------------
    // Witnesses
    //---------------------------------------------------------------------------

    let GenWitnessArgTys (g: TcGlobals) (traitInfo: TraitWitnessInfo) =
        let (TraitWitnessInfo(_tys, _nm, _memFlags, argTys, _rty)) = traitInfo
        let argTys = if argTys.IsEmpty then [ g.unit_ty ] else argTys
        let argTysl = List.map List.singleton argTys
        argTysl

    let GenWitnessTy (g: TcGlobals) (traitInfo: TraitWitnessInfo) =
        let retTy =
            match traitInfo.ReturnType with
            | None -> g.unit_ty
            | Some ty -> ty

        let argTysl = GenWitnessArgTys g traitInfo
        mkMethodTy g argTysl retTy

    let GenWitnessTys (g: TcGlobals) (cxs: TraitWitnessInfos) =
        if g.generateWitnesses then
            cxs |> List.map (GenWitnessTy g)
        else
            []

    //--------------------------------------------------------------------------
    // tyOfExpr
    //--------------------------------------------------------------------------

    let rec tyOfExpr g expr =
        match expr with
        | Expr.App(_, fty, tyargs, args, _) -> applyTys g fty (tyargs, args)
        | Expr.Obj(_, ty, _, _, _, _, _)
        | Expr.Match(_, _, _, _, _, ty)
        | Expr.Quote(_, _, _, _, ty)
        | Expr.Const(_, _, ty) -> ty
        | Expr.Val(vref, _, _) -> vref.Type
        | Expr.Sequential(a, b, k, _) ->
            tyOfExpr
                g
                (match k with
                 | NormalSeq -> b
                 | ThenDoSeq -> a)
        | Expr.Lambda(_, _, _, vs, _, _, bodyTy) -> mkFunTy g (mkRefTupledVarsTy g vs) bodyTy
        | Expr.TyLambda(_, tyvs, _, _, bodyTy) -> (tyvs +-> bodyTy)
        | Expr.Let(_, e, _, _)
        | Expr.TyChoose(_, e, _)
        | Expr.Link { contents = e }
        | Expr.DebugPoint(_, e)
        | Expr.StaticOptimization(_, _, e, _)
        | Expr.LetRec(_, e, _, _) -> tyOfExpr g e
        | Expr.Op(op, tinst, _, _) ->
            match op with
            | TOp.Coerce ->
                (match tinst with
                 | [ toTy; _fromTy ] -> toTy
                 | _ -> failwith "bad TOp.Coerce node")
            | TOp.ILCall(_, _, _, _, _, _, _, _, _, _, retTypes)
            | TOp.ILAsm(_, retTypes) ->
                (match retTypes with
                 | [ h ] -> h
                 | _ -> g.unit_ty)
            | TOp.UnionCase uc -> actualResultTyOfUnionCase tinst uc
            | TOp.UnionCaseProof uc -> mkProvenUnionCaseTy uc tinst
            | TOp.Recd(_, tcref) -> mkWoNullAppTy tcref tinst
            | TOp.ExnConstr _ -> g.exn_ty
            | TOp.Bytes _ -> mkByteArrayTy g
            | TOp.UInt16s _ -> mkArrayType g g.uint16_ty
            | TOp.AnonRecdGet(_, i) -> List.item i tinst
            | TOp.TupleFieldGet(_, i) -> List.item i tinst
            | TOp.Tuple tupInfo -> mkAnyTupledTy g tupInfo tinst
            | TOp.AnonRecd anonInfo -> mkAnyAnonRecdTy g anonInfo tinst
            | TOp.IntegerForLoop _
            | TOp.While _ -> g.unit_ty
            | TOp.Array ->
                (match tinst with
                 | [ ty ] -> mkArrayType g ty
                 | _ -> failwith "bad TOp.Array node")
            | TOp.TryWith _
            | TOp.TryFinally _ ->
                (match tinst with
                 | [ ty ] -> ty
                 | _ -> failwith "bad TOp_try node")
            | TOp.ValFieldGetAddr(fref, readonly) -> mkByrefTyWithFlag g readonly (actualTyOfRecdFieldRef fref tinst)
            | TOp.ValFieldGet fref -> actualTyOfRecdFieldRef fref tinst
            | TOp.ValFieldSet _
            | TOp.UnionCaseFieldSet _
            | TOp.ExnFieldSet _
            | TOp.LValueOp((LSet | LByrefSet), _) -> g.unit_ty
            | TOp.UnionCaseTagGet _ -> g.int_ty
            | TOp.UnionCaseFieldGetAddr(cref, j, readonly) ->
                mkByrefTyWithFlag g readonly (actualTyOfRecdField (mkTyconRefInst cref.TyconRef tinst) (cref.FieldByIndex j))
            | TOp.UnionCaseFieldGet(cref, j) -> actualTyOfRecdField (mkTyconRefInst cref.TyconRef tinst) (cref.FieldByIndex j)
            | TOp.ExnFieldGet(ecref, j) -> recdFieldTyOfExnDefRefByIdx ecref j
            | TOp.LValueOp(LByrefGet, v) -> destByrefTy g v.Type
            | TOp.LValueOp(LAddrOf readonly, v) -> mkByrefTyWithFlag g readonly v.Type
            | TOp.RefAddrGet readonly ->
                (match tinst with
                 | [ ty ] -> mkByrefTyWithFlag g readonly ty
                 | _ -> failwith "bad TOp.RefAddrGet node")
            | TOp.TraitCall traitInfo -> traitInfo.GetReturnType(g)
            | TOp.Reraise ->
                (match tinst with
                 | [ rtn_ty ] -> rtn_ty
                 | _ -> failwith "bad TOp.Reraise node")
            | TOp.Goto _
            | TOp.Label _
            | TOp.Return ->
                //assert false
                //errorR(InternalError("unexpected goto/label/return in tyOfExpr", m))
                // It doesn't matter what type we return here. This is only used in free variable analysis in the code generator
                g.unit_ty
        | Expr.WitnessArg(traitInfo, _m) ->
            let witnessInfo = traitInfo.GetWitnessInfo()
            GenWitnessTy g witnessInfo

    //--------------------------------------------------------------------------
    // Decision tree reduction
    //--------------------------------------------------------------------------

    let rec accTargetsOfDecisionTree tree acc =
        match tree with
        | TDSwitch(_, cases, dflt, _) ->
            List.foldBack
                (fun (c: DecisionTreeCase) -> accTargetsOfDecisionTree c.CaseTree)
                cases
                (Option.foldBack accTargetsOfDecisionTree dflt acc)
        | TDSuccess(_, i) -> i :: acc
        | TDBind(_, rest) -> accTargetsOfDecisionTree rest acc

    let rec mapTargetsOfDecisionTree f tree =
        match tree with
        | TDSwitch(e, cases, dflt, m) ->
            let casesR = cases |> List.map (mapTargetsOfDecisionTreeCase f)
            let dfltR = Option.map (mapTargetsOfDecisionTree f) dflt
            TDSwitch(e, casesR, dfltR, m)
        | TDSuccess(es, i) -> TDSuccess(es, f i)
        | TDBind(bind, rest) -> TDBind(bind, mapTargetsOfDecisionTree f rest)

    and mapTargetsOfDecisionTreeCase f (TCase(x, t)) = TCase(x, mapTargetsOfDecisionTree f t)

    // Dead target elimination
    let eliminateDeadTargetsFromMatch tree (targets: _[]) =
        let used = accTargetsOfDecisionTree tree [] |> ListSet.setify (=) |> Array.ofList

        if used.Length < targets.Length then
            Array.sortInPlace used
            let ntargets = targets.Length

            let treeR =
                let remap = Array.create ntargets -1
                Array.iteri (fun i tgn -> remap[tgn] <- i) used

                tree
                |> mapTargetsOfDecisionTree (fun tgn ->
                    if remap[tgn] = -1 then
                        failwith "eliminateDeadTargetsFromMatch: failure while eliminating unused targets"

                    remap[tgn])

            let targetsR = Array.map (Array.get targets) used
            treeR, targetsR
        else
            tree, targets

    let rec targetOfSuccessDecisionTree tree =
        match tree with
        | TDSwitch _ -> None
        | TDSuccess(_, i) -> Some i
        | TDBind(_, t) -> targetOfSuccessDecisionTree t

    /// Check a decision tree only has bindings that immediately cover a 'Success'
    let rec decisionTreeHasNonTrivialBindings tree =
        match tree with
        | TDSwitch(_, cases, dflt, _) ->
            cases |> List.exists (fun c -> decisionTreeHasNonTrivialBindings c.CaseTree)
            || dflt |> Option.exists decisionTreeHasNonTrivialBindings
        | TDSuccess _ -> false
        | TDBind(_, t) -> Option.isNone (targetOfSuccessDecisionTree t)

    // If a target has assignments and can only be reached through one
    // branch (i.e. is "linear"), then transfer the assignments to the r.h.s. to be a "let".
    let foldLinearBindingTargetsOfMatch tree (targets: _[]) =

        // Don't do this when there are any bindings in the tree except where those bindings immediately cover a success node
        // since the variables would be extruded from their scope.
        if decisionTreeHasNonTrivialBindings tree then
            tree, targets

        else
            let branchesToTargets = Array.create targets.Length []
            // Build a map showing how each target might be reached
            let rec accumulateTipsOfDecisionTree accBinds tree =
                match tree with
                | TDSwitch(_, cases, dflt, _) ->
                    assert (isNil accBinds) // No switches under bindings

                    for edge in cases do
                        accumulateTipsOfDecisionTree accBinds edge.CaseTree

                    match dflt with
                    | None -> ()
                    | Some tree -> accumulateTipsOfDecisionTree accBinds tree
                | TDSuccess(es, i) -> branchesToTargets[i] <- (List.rev accBinds, es) :: branchesToTargets[i]
                | TDBind(bind, rest) -> accumulateTipsOfDecisionTree (bind :: accBinds) rest

            // Compute the targets that can only be reached one way
            accumulateTipsOfDecisionTree [] tree

            let isLinearTarget bs =
                match bs with
                | [ _ ] -> true
                | _ -> false

            let isLinearTgtIdx i = isLinearTarget branchesToTargets[i]
            let getLinearTgtIdx i = branchesToTargets[i].Head
            let hasLinearTgtIdx = branchesToTargets |> Array.exists isLinearTarget

            if not hasLinearTgtIdx then

                tree, targets

            else

                /// rebuild the decision tree, replacing 'bind-then-success' decision trees by TDSuccess nodes that just go to the target
                let rec rebuildDecisionTree tree =

                    // Check if this is a bind-then-success tree
                    match targetOfSuccessDecisionTree tree with
                    | Some i when isLinearTgtIdx i -> TDSuccess([], i)
                    | _ ->
                        match tree with
                        | TDSwitch(e, cases, dflt, m) ->
                            let casesR = List.map rebuildDecisionTreeEdge cases
                            let dfltR = Option.map rebuildDecisionTree dflt
                            TDSwitch(e, casesR, dfltR, m)
                        | TDSuccess _ -> tree
                        | TDBind _ -> tree

                and rebuildDecisionTreeEdge (TCase(x, t)) = TCase(x, rebuildDecisionTree t)

                let treeR = rebuildDecisionTree tree

                /// rebuild the targets, replacing linear targets by ones that include all the 'let' bindings from the source
                let targetsR =
                    targets
                    |> Array.mapi (fun i (TTarget(vs, exprTarget, _) as tg) ->
                        if isLinearTgtIdx i then
                            let binds, es = getLinearTgtIdx i
                            // The value bindings are moved to become part of the target.
                            // Hence the expressions in the value bindings can be remarked with the range of the target.
                            let mTarget = exprTarget.Range
                            let es = es |> List.map (remarkExpr mTarget)
                            // These are non-sticky - any sequence point for 'exprTarget' goes on 'exprTarget' _after_ the bindings have been evaluated
                            TTarget(List.empty, mkLetsBind mTarget binds (mkInvisibleLetsFromBindings mTarget vs es exprTarget), None)
                        else
                            tg)

                treeR, targetsR

    // Simplify a little as we go, including dead target elimination
    let simplifyTrivialMatch spBind mExpr mMatch ty tree (targets: _[]) =
        match tree with
        | TDSuccess(es, n) ->
            if n >= targets.Length then
                failwith "simplifyTrivialMatch: target out of range"

            let (TTarget(vs, rhs, _)) = targets[n]

            if vs.Length <> es.Length then
                failwith (
                    "simplifyTrivialMatch: invalid argument, n = "
                    + string n
                    + ", #targets = "
                    + string targets.Length
                )

            // These are non-sticky - any sequence point for 'rhs' goes on 'rhs' _after_ the bindings have been made
            let res = mkInvisibleLetsFromBindings rhs.Range vs es rhs

            // Incorporate spBind as a note if present
            let res =
                match spBind with
                | DebugPointAtBinding.Yes dp -> Expr.DebugPoint(DebugPointAtLeafExpr.Yes dp, res)
                | _ -> res

            res
        | _ -> primMkMatch (spBind, mExpr, tree, targets, mMatch, ty)

    // Simplify a little as we go, including dead target elimination
    let mkAndSimplifyMatch spBind mExpr mMatch ty tree targets =
        let targets = Array.ofList targets

        match tree with
        | TDSuccess _ -> simplifyTrivialMatch spBind mExpr mMatch ty tree targets
        | _ ->
            let tree, targets = eliminateDeadTargetsFromMatch tree targets
            let tree, targets = foldLinearBindingTargetsOfMatch tree targets
            simplifyTrivialMatch spBind mExpr mMatch ty tree targets

    [<return: Struct>]
    let (|WhileExpr|_|) expr =
        match expr with
        | Expr.Op(TOp.While(sp1, sp2),
                  _,
                  [ Expr.Lambda(_, _, _, [ _gv ], guardExpr, _, _); Expr.Lambda(_, _, _, [ _bv ], bodyExpr, _, _) ],
                  m) -> ValueSome(sp1, sp2, guardExpr, bodyExpr, m)
        | _ -> ValueNone

    [<return: Struct>]
    let (|TryFinallyExpr|_|) expr =
        match expr with
        | Expr.Op(TOp.TryFinally(sp1, sp2), [ ty ], [ Expr.Lambda(_, _, _, [ _ ], e1, _, _); Expr.Lambda(_, _, _, [ _ ], e2, _, _) ], m) ->
            ValueSome(sp1, sp2, ty, e1, e2, m)
        | _ -> ValueNone

    [<return: Struct>]
    let (|IntegerForLoopExpr|_|) expr =
        match expr with
        | Expr.Op(TOp.IntegerForLoop(sp1, sp2, style),
                  _,
                  [ Expr.Lambda(_, _, _, [ _ ], e1, _, _); Expr.Lambda(_, _, _, [ _ ], e2, _, _); Expr.Lambda(_, _, _, [ v ], e3, _, _) ],
                  m) -> ValueSome(sp1, sp2, style, e1, e2, v, e3, m)
        | _ -> ValueNone

    [<return: Struct>]
    let (|TryWithExpr|_|) expr =
        match expr with
        | Expr.Op(TOp.TryWith(spTry, spWith),
                  [ resTy ],
                  [ Expr.Lambda(_, _, _, [ _ ], bodyExpr, _, _)
                    Expr.Lambda(_, _, _, [ filterVar ], filterExpr, _, _)
                    Expr.Lambda(_, _, _, [ handlerVar ], handlerExpr, _, _) ],
                  m) -> ValueSome(spTry, spWith, resTy, bodyExpr, filterVar, filterExpr, handlerVar, handlerExpr, m)
        | _ -> ValueNone
