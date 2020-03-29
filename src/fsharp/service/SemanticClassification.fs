// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SourceCodeServices

open System.Diagnostics
open System.Collections.Generic
open System.Collections.Immutable

open FSharp.Compiler
open FSharp.Compiler.AbstractIL.Internal.Library  
open FSharp.Compiler.Infos
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.Lib
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
    | Function
    | Property
    | MutableVar
    | Module
    | Printf
    | ComputationExpression
    | IntrinsicFunction
    | Enumeration
    | Interface
    | TypeArgument
    | Operator
    | Disposable

[<AutoOpen>]
module TcResolutionsExtensions =

    let (|CNR|) (cnr:CapturedNameResolution) =
        (cnr.Item, cnr.ItemOccurence, cnr.DisplayEnv, cnr.NameResolutionEnv, cnr.AccessorDomain, cnr.Range)

    type TcResolutions with

        member sResolutions.GetSemanticClassification(g: TcGlobals, amap: Import.ImportMap, formatSpecifierLocations: (range * int) [], range: range option) : struct(range * SemanticClassificationType) [] =
              ErrorScope.Protect Range.range0 
               (fun () -> 
                let (|LegitTypeOccurence|_|) = function
                    | ItemOccurence.UseInType
                    | ItemOccurence.UseInAttribute
                    | ItemOccurence.Use _
                    | ItemOccurence.Binding _
                    | ItemOccurence.Pattern _ -> Some()
                    | _ -> None

                let (|OptionalArgumentAttribute|_|) ttype =
                    match ttype with
                    | TType.TType_app(tref, _) when tref.Stamp = g.attrib_OptionalArgumentAttribute.TyconRef.Stamp -> Some()
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
                    protectAssemblyExplorationNoReraise false false (fun () -> Infos.ExistsHeadTypeInEntireHierarchy g amap range0 ty g.tcref_System_IDisposable)

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
                        elif IsOperatorName vref.DisplayName then
                            add m SemanticClassificationType.Operator
                        else
                            add m SemanticClassificationType.Function
                    | Item.RecdField rfinfo, _, _, _, _, m when isRecdFieldMutable rfinfo ->
                        add m SemanticClassificationType.MutableVar
                    | Item.RecdField rfinfo, _, _, _, _, m when isFunction g rfinfo.FieldType ->
                        add m SemanticClassificationType.Function
                    | Item.RecdField EnumCaseFieldInfo, _, _, _, _, m ->
                        add m SemanticClassificationType.Enumeration
                    | Item.MethodGroup _, _, _, _, _, m ->
                        add m SemanticClassificationType.Function
                    // custom builders, custom operations get colored as keywords
                    | (Item.CustomBuilder _ | Item.CustomOperation _), ItemOccurence.Use, _, _, _, m ->
                        add m SemanticClassificationType.ComputationExpression
                    // types get colored as types when they occur in syntactic types or custom attributes
                    // type variables get colored as types when they occur in syntactic types custom builders, custom operations get colored as keywords
                    | Item.Types (_, [OptionalArgumentAttribute]), LegitTypeOccurence, _, _, _, _ -> ()
                    | Item.CtorGroup(_, [MethInfo.FSMeth(_, OptionalArgumentAttribute, _, _)]), LegitTypeOccurence, _, _, _, _ -> ()
                    | Item.Types(_, types), LegitTypeOccurence, _, _, _, m when types |> List.exists (isInterfaceTy g) -> 
                        add m SemanticClassificationType.Interface
                    | Item.Types(_, types), LegitTypeOccurence, _, _, _, m when types |> List.exists (isStructTy g) -> 
                        add m SemanticClassificationType.ValueType
                    | Item.Types(_, TType_app(tyconRef, TType_measure _ :: _) :: _), LegitTypeOccurence, _, _, _, m when isStructTyconRef tyconRef ->
                        add m SemanticClassificationType.ValueType
                    | Item.Types(_, types), LegitTypeOccurence, _, _, _, m when types |> List.exists isDisposableTy ->
                        add m SemanticClassificationType.Disposable
                    | Item.Types _, LegitTypeOccurence, _, _, _, m -> 
                        add m SemanticClassificationType.ReferenceType
                    | (Item.TypeVar _ ), LegitTypeOccurence, _, _, _, m ->
                        add m SemanticClassificationType.TypeArgument
                    | Item.UnqualifiedType tyconRefs, LegitTypeOccurence, _, _, _, m ->
                        if tyconRefs |> List.exists (fun tyconRef -> tyconRef.Deref.IsStructOrEnumTycon) then
                            add m SemanticClassificationType.ValueType
                        else add m SemanticClassificationType.ReferenceType
                    | Item.CtorGroup(_, minfos), LegitTypeOccurence, _, _, _, m ->
                        if minfos |> List.exists (fun minfo -> isStructTy g minfo.ApparentEnclosingType) then
                            add m SemanticClassificationType.ValueType
                        else add m SemanticClassificationType.ReferenceType
                    | Item.ExnCase _, LegitTypeOccurence, _, _, _, m ->
                        add m SemanticClassificationType.ReferenceType
                    | Item.ModuleOrNamespaces refs, LegitTypeOccurence, _, _, _, m when refs |> List.exists (fun x -> x.IsModule) ->
                        add m SemanticClassificationType.Module
                    | (Item.ActivePatternCase _ | Item.UnionCase _ | Item.ActivePatternResult _), _, _, _, _, m ->
                        add m SemanticClassificationType.UnionCase
                    | _ -> ())
                results.AddRange(formatSpecifierLocations |> Array.map (fun (m, _) -> struct(m, SemanticClassificationType.Printf)))
                results.ToArray()
               ) 
               (fun msg -> 
                   Trace.TraceInformation(sprintf "FCS: recovering from error in GetSemanticClassification: '%s'" msg)
                   Array.empty)