// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SourceCodeServices

open System.Diagnostics
open System.Collections.Generic
open System.Collections.Immutable

open FSharp.Compiler
open FSharp.Compiler.AbstractIL.Internal.Library  
open FSharp.Compiler.Infos
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.NameResolution
open FSharp.Compiler.PrettyNaming
open FSharp.Compiler.Range
open FSharp.Compiler.TcGlobals 
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.SourceCodeServices.SymbolHelpers 

[<RequireQualifiedAccess>]
type SemanticClassificationType =
    | ReferenceType
    | ValueType
    | UnionCase
    | UnionCaseField
    | Function
    | Property
    | MutableVar
    | Module
    | NameSpace
    | Printf
    | ComputationExpression
    | IntrinsicFunction
    | Enumeration
    | Interface
    | TypeArgument
    | Operator
    | DisposableType
    | DisposableValue
    | Method
    | ExtensionMethod
    | ConstructorForReferenceType
    | ConstructorForValueType
    | Literal
    | RecordField
    | MutableRecordField
    | RecordFieldAsFunction
    | Exception
    | Field
    | Event
    | Delegate
    | NamedArgument
    | Value
    | LocalValue
    | Type
    | TypeDef

[<AutoOpen>]
module TcResolutionsExtensions =
    let (|CNR|) (cnr:CapturedNameResolution) =
        (cnr.Item, cnr.ItemOccurence, cnr.DisplayEnv, cnr.NameResolutionEnv, cnr.AccessorDomain, cnr.Range)

    type TcResolutions with
        member sResolutions.GetSemanticClassification(g: TcGlobals, amap: Import.ImportMap, formatSpecifierLocations: (range * int) [], range: range option) : struct(range * SemanticClassificationType) [] =
            ErrorScope.Protect Range.range0 (fun () ->
                let (|LegitTypeOccurence|_|) = function
                    | ItemOccurence.UseInType
                    | ItemOccurence.UseInAttribute
                    | ItemOccurence.Use _
                    | ItemOccurence.Binding _
                    | ItemOccurence.Pattern _ -> Some()
                    | _ -> None

                let (|KeywordIntrinsicValue|_|) (vref: ValRef) =
                    if valRefEq g g.raise_vref vref ||
                        valRefEq g g.reraise_vref vref ||
                        valRefEq g g.typeof_vref vref ||
                        valRefEq g g.typedefof_vref vref ||
                        valRefEq g g.sizeof_vref vref ||
                        valRefEq g g.nameof_vref vref then Some()
                    else None
            
                let (|EnumCaseFieldInfo|_|) (rfinfo : RecdFieldInfo) =
                    match rfinfo.TyconRef.TypeReprInfo with
                    | TFSharpObjectRepr x ->
                        match x.fsobjmodel_kind with
                        | TTyconEnum -> Some ()
                        | _ -> None
                    | _ -> None

                let resolutions =
                    match range with
                    | Some range ->
                        sResolutions.CapturedNameResolutions
                        |> Seq.filter (fun cnr -> rangeContainsPos range cnr.Range.Start || rangeContainsPos range cnr.Range.End)
                    | None -> 
                        sResolutions.CapturedNameResolutions :> seq<_>

                let isDisposableTy (ty: TType) =
                    not (typeEquiv g ty g.system_IDisposable_ty) &&
                    protectAssemblyExplorationNoReraise false false (fun () -> Infos.ExistsHeadTypeInEntireHierarchy g amap range0 ty g.tcref_System_IDisposable)
                    
                let isDiscard (str: string) = str.StartsWith("_")

                let isValRefDisposable (vref: ValRef) =
                    not (isDiscard vref.DisplayName) &&
                    // For values, we actually do want to color things if they literally are IDisposables 
                    protectAssemblyExplorationNoReraise false false (fun () -> Infos.ExistsHeadTypeInEntireHierarchy g amap range0 vref.Type g.tcref_System_IDisposable)

                let isStructTyconRef (tyconRef: TyconRef) = 
                    let ty = generalizedTyconRef tyconRef
                    let underlyingTy = stripTyEqnsAndMeasureEqns g ty
                    isStructTy g underlyingTy

                let isValRefMutable (vref: ValRef) =
                    // Mutable values, ref cells, and non-inref byrefs are mutable.
                    vref.IsMutable
                    || isRefCellTy g vref.Type
                    || (isByrefTy g vref.Type && not (isInByrefTy g vref.Type))

                let isRecdFieldMutable (rfinfo: RecdFieldInfo) =
                    (rfinfo.RecdField.IsMutable && rfinfo.LiteralValue.IsNone)
                    || isRefCellTy g rfinfo.RecdField.FormalType

                let duplicates = HashSet<range>(Range.comparer)

                let results = ImmutableArray.CreateBuilder()
                let inline add m typ =
                    if duplicates.Add m then
                        results.Add struct(m, typ)
                resolutions
                |> Seq.iter (fun cnr ->
                    match cnr.Item, cnr.ItemOccurence, cnr.DisplayEnv, cnr.NameResolutionEnv, cnr.AccessorDomain, cnr.Range with
                    // 'seq' in 'seq { ... }' gets colored as keywords
                    | (Item.Value vref), ItemOccurence.Use, _, _, _, m when valRefEq g g.seq_vref vref ->
                        add m SemanticClassificationType.ComputationExpression

                    | (Item.Value vref), _, _, _, _, m when isValRefMutable vref ->
                        add m SemanticClassificationType.MutableVar

                    | Item.Value KeywordIntrinsicValue, ItemOccurence.Use, _, _, _, m ->
                        add m SemanticClassificationType.IntrinsicFunction

                    | (Item.Value vref), _, _, _, _, m when isFunction g vref.Type ->
                        if valRefEq g g.range_op_vref vref || valRefEq g g.range_step_op_vref vref then 
                            ()
                        elif vref.IsPropertyGetterMethod || vref.IsPropertySetterMethod then
                            add m SemanticClassificationType.Property
                        elif vref.IsMember then
                            add m SemanticClassificationType.Method
                        elif IsOperatorName vref.DisplayName then
                            add m SemanticClassificationType.Operator
                        else
                            add m SemanticClassificationType.Function

                    | (Item.Value vref), _, _, _, _, m ->
                        if isValRefDisposable vref then
                            add m SemanticClassificationType.DisposableValue
                        elif Option.isSome vref.LiteralValue then
                            add m SemanticClassificationType.Literal
                        elif not vref.IsCompiledAsTopLevel && not(isDiscard vref.DisplayName) then
                            add m SemanticClassificationType.LocalValue
                        else
                            add m SemanticClassificationType.Value

                    | Item.RecdField rfinfo, _, _, _, _, m ->
                        match rfinfo with
                        | EnumCaseFieldInfo ->
                            add m SemanticClassificationType.Enumeration
                        | _ ->
                            if isRecdFieldMutable rfinfo then
                                add m SemanticClassificationType.MutableRecordField
                            elif isFunTy g rfinfo.FieldType then
                                add m SemanticClassificationType.RecordFieldAsFunction
                            else
                                add m SemanticClassificationType.RecordField

                    | Item.AnonRecdField(_, tys, idx, m), _, _, _, _, _ ->
                        let ty = tys.[idx]

                        // It's not currently possible for anon record fields to be mutable, but they can be ref cells
                        if isRefCellTy g ty then
                            add m SemanticClassificationType.MutableRecordField
                        elif isFunTy g ty then
                            add m SemanticClassificationType.RecordFieldAsFunction
                        else
                            add m SemanticClassificationType.RecordField

                    | Item.Property (_, pinfo :: _), _, _, _, _, m ->
                        if not pinfo.IsIndexer then
                            add m SemanticClassificationType.Property

                    | Item.CtorGroup (_, minfos), _, _, _, _, m ->
                        if minfos |> List.forall (fun minfo -> isDisposableTy minfo.ApparentEnclosingType) then
                            add m SemanticClassificationType.DisposableType
                        elif minfos |> List.forall (fun minfo -> isStructTy g minfo.ApparentEnclosingType) then
                            add m SemanticClassificationType.ConstructorForValueType
                        else
                            add m SemanticClassificationType.ConstructorForReferenceType

                    | (Item.DelegateCtor _ | Item.FakeInterfaceCtor _), _, _, _, _, m ->
                        add m SemanticClassificationType.ConstructorForReferenceType

                    | Item.MethodGroup (_, minfos, _), _, _, _, _, m ->
                        if minfos |> List.forall (fun minfo -> minfo.IsExtensionMember || minfo.IsCSharpStyleExtensionMember) then
                            add m SemanticClassificationType.ExtensionMethod
                        else
                            add m SemanticClassificationType.Method

                    | (Item.CustomBuilder _ | Item.CustomOperation _), ItemOccurence.Use, _, _, _, m ->
                        add m SemanticClassificationType.ComputationExpression

                    // Special case measures for struct types
                    | Item.Types(_, TType_app(tyconRef, TType_measure _ :: _) :: _), LegitTypeOccurence, _, _, _, m when isStructTyconRef tyconRef ->
                        add m SemanticClassificationType.ValueType

                    | Item.Types (_, ty :: _), LegitTypeOccurence, _, _, _, m ->
                        let reprToClassificationType repr tcref = 
                            match repr with
                            | TFSharpObjectRepr om -> 
                                match om.fsobjmodel_kind with 
                                | TTyconClass -> SemanticClassificationType.ReferenceType
                                | TTyconInterface -> SemanticClassificationType.Interface
                                | TTyconStruct -> SemanticClassificationType.ValueType
                                | TTyconDelegate _ -> SemanticClassificationType.Delegate
                                | TTyconEnum _ -> SemanticClassificationType.Enumeration
                            | TRecdRepr _
                            | TUnionRepr _ -> 
                                if isStructTyconRef tcref then
                                    SemanticClassificationType.ValueType
                                else
                                    SemanticClassificationType.Type
                            | TILObjectRepr (TILObjectReprData (_, _, td)) -> 
                                if td.IsClass then
                                    SemanticClassificationType.ReferenceType
                                elif td.IsStruct then
                                    SemanticClassificationType.ValueType
                                elif td.IsInterface then
                                    SemanticClassificationType.Interface
                                elif td.IsEnum then
                                    SemanticClassificationType.Enumeration
                                else
                                    SemanticClassificationType.Delegate
                            | TAsmRepr _ -> SemanticClassificationType.TypeDef
                            | TMeasureableRepr _-> SemanticClassificationType.TypeDef 
#if !NO_EXTENSIONTYPING
                            | TProvidedTypeExtensionPoint _-> SemanticClassificationType.TypeDef 
                            | TProvidedNamespaceExtensionPoint  _-> SemanticClassificationType.TypeDef  
#endif
                            | TNoRepr -> SemanticClassificationType.ReferenceType

                        let ty = stripTyEqns g ty
                        if isDisposableTy ty then
                            add m SemanticClassificationType.DisposableType
                        else
                            match tryTcrefOfAppTy g ty with
                            | ValueSome tcref ->
                                add m (reprToClassificationType tcref.TypeReprInfo tcref)
                            | ValueNone ->
                                if isStructTupleTy g ty then
                                    add m SemanticClassificationType.ValueType
                                elif isRefTupleTy g ty then
                                    add m SemanticClassificationType.ReferenceType
                                elif isFunction g ty then
                                    add m SemanticClassificationType.Function
                                elif isTyparTy g ty then
                                    add m SemanticClassificationType.ValueType
                                else
                                    add m SemanticClassificationType.TypeDef                            

                    | (Item.TypeVar _ ), LegitTypeOccurence, _, _, _, m ->
                        add m SemanticClassificationType.TypeArgument

                    | Item.ExnCase _, LegitTypeOccurence, _, _, _, m ->
                        add m SemanticClassificationType.Exception

                    | Item.ModuleOrNamespaces (modref :: _), LegitTypeOccurence, _, _, _, m ->
                        if modref.IsNamespace then
                            add m SemanticClassificationType.NameSpace
                        else
                            add m SemanticClassificationType.Module

                    | (Item.ActivePatternCase _ | Item.UnionCase _ | Item.ActivePatternResult _), _, _, _, _, m ->
                        add m SemanticClassificationType.UnionCase

                    | Item.UnionCaseField _, _, _, _, _, m ->
                        add m SemanticClassificationType.UnionCaseField

                    | Item.ILField _, _, _, _, _, m ->
                        add m SemanticClassificationType.Field

                    | Item.Event _, _, _, _, _, m ->
                        add m SemanticClassificationType.Event

                    | (Item.ArgName _ | Item.SetterArg _), _, _, _, _, m ->
                        add m SemanticClassificationType.NamedArgument

                    | Item.SetterArg _, _, _, _, _, m ->
                        add m SemanticClassificationType.Property

                    | Item.UnqualifiedType (tcref :: _), LegitTypeOccurence, _, _, _, m ->
                        if tcref.IsEnumTycon || tcref.IsILEnumTycon then
                            add m SemanticClassificationType.Enumeration
                        elif tcref.IsExceptionDecl then
                            add m SemanticClassificationType.Exception
                        elif tcref.IsFSharpDelegateTycon then
                            add m SemanticClassificationType.Delegate
                        elif tcref.IsFSharpInterfaceTycon then
                            add m SemanticClassificationType.Interface
                        elif tcref.IsFSharpStructOrEnumTycon then
                            add m SemanticClassificationType.ValueType
                        elif tcref.IsModule then
                            add m SemanticClassificationType.Module
                        elif tcref.IsNamespace then
                            add m SemanticClassificationType.NameSpace
                        elif tcref.IsUnionTycon || tcref.IsRecordTycon then
                            if isStructTyconRef tcref then
                                add m SemanticClassificationType.ValueType
                            else
                                add m SemanticClassificationType.UnionCase
                        elif tcref.IsILTycon then
                            let (TILObjectReprData (_, _, tydef)) = tcref.ILTyconInfo

                            if tydef.IsInterface then
                                add m SemanticClassificationType.Interface
                            elif tydef.IsDelegate then
                                add m SemanticClassificationType.Delegate
                            elif tydef.IsEnum then
                                add m SemanticClassificationType.Enumeration
                            elif tydef.IsStruct then
                                add m SemanticClassificationType.ValueType
                            else
                                add m SemanticClassificationType.ReferenceType

                    | _ ->
                        ())
                results.AddRange(formatSpecifierLocations |> Array.map (fun (m, _) -> struct(m, SemanticClassificationType.Printf)))
                results.ToArray()
               ) 
               (fun msg -> 
                   Trace.TraceInformation(sprintf "FCS: recovering from error in GetSemanticClassification: '%s'" msg)
                   Array.empty)