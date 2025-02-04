// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Functions to import .NET binary metadata as TAST objects
module internal FSharp.Compiler.Import

open System.Collections.Concurrent
open System.Collections.Generic
open System.Collections.Immutable
open FSharp.Compiler.Text.Range
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open Internal.Utilities.TypeHashing
open Internal.Utilities.TypeHashing.HashTypes
open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.Text
open FSharp.Compiler.Xml
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TcGlobals

#if !NO_TYPEPROVIDERS
open FSharp.Compiler.TypeProviders
#endif

/// Represents an interface to some of the functionality of TcImports, for loading assemblies
/// and accessing information about generated provided assemblies.
type AssemblyLoader =

    /// Resolve an Abstract IL assembly reference to a Ccu
    abstract FindCcuFromAssemblyRef : CompilationThreadToken * range * ILAssemblyRef -> CcuResolutionResult

    abstract TryFindXmlDocumentationInfo : assemblyName: string -> XmlDocumentationInfo option

#if !NO_TYPEPROVIDERS

    /// Get a flag indicating if an assembly is a provided assembly, plus the
    /// table of information recording remappings from type names in the provided assembly to type
    /// names in the statically linked, embedded assembly.
    abstract GetProvidedAssemblyInfo : CompilationThreadToken * range * Tainted<ProvidedAssembly MaybeNull> -> bool * ProvidedAssemblyStaticLinkingMap option

    /// Record a root for a [<Generate>] type to help guide static linking & type relocation
    abstract RecordGeneratedTypeRoot : ProviderGeneratedType -> unit
#endif

[<Struct; NoComparison>]
type CanCoerce =
    | CanCoerce
    | NoCoerce

type [<Struct; NoComparison; CustomEquality>] TTypeCacheKey =

    val ty1: TType
    val ty2: TType
    val canCoerce: CanCoerce
    val tcGlobals: TcGlobals

    private new (ty1, ty2, canCoerce, tcGlobals) =
        { ty1 = ty1; ty2 = ty2; canCoerce = canCoerce; tcGlobals = tcGlobals }

    static member FromStrippedTypes (ty1, ty2, canCoerce, tcGlobals) =
        TTypeCacheKey(ty1, ty2, canCoerce, tcGlobals)

    interface System.IEquatable<TTypeCacheKey> with
        member this.Equals other =
            if this.canCoerce <> other.canCoerce then
                false
            elif this.ty1 === other.ty1 && this.ty2 === other.ty2 then
                true
            else
                stampEquals this.tcGlobals this.ty1 other.ty1
                && stampEquals this.tcGlobals this.ty2 other.ty2

    override this.Equals(other:objnull) =
        match other with
        | :? TTypeCacheKey as p -> (this :> System.IEquatable<TTypeCacheKey>).Equals p
        | _ -> false

    override this.GetHashCode() : int =
        let g = this.tcGlobals

        let ty1Hash = combineHash (hashStamp g this.ty1) (hashTType g this.ty1)
        let ty2Hash = combineHash (hashStamp g this.ty2) (hashTType g this.ty2)

        let combined = combineHash (combineHash ty1Hash ty2Hash) (hash this.canCoerce)

        combined

//-------------------------------------------------------------------------
// Import an IL types as F# types.
//-------------------------------------------------------------------------

/// Represents a context used by the import routines that convert AbstractIL types and provided
/// types to F# internal compiler data structures.
///
/// Also caches the conversion of AbstractIL ILTypeRef nodes, based on hashes of these.
///
/// There is normally only one ImportMap for any assembly compilation, though additional instances can be created
/// using tcImports.GetImportMap() if needed, and it is not harmful if multiple instances are used. The object
/// serves as an interface through to the tables stored in the primary TcImports structures defined in CompileOps.fs.
[<Sealed>]
type ImportMap(g: TcGlobals, assemblyLoader: AssemblyLoader) =
    let typeRefToTyconRefCache = ConcurrentDictionary<ILTypeRef, TyconRef>()

    let typeSubsumptionCache = ConcurrentDictionary<TTypeCacheKey, bool>(System.Environment.ProcessorCount, 1024)

    member _.g = g

    member _.assemblyLoader = assemblyLoader

    member _.ILTypeRefToTyconRefCache = typeRefToTyconRefCache

    member _.TypeSubsumptionCache = typeSubsumptionCache

let CanImportILScopeRef (env: ImportMap) m scoref =

    let isResolved assemblyRef =
        // Explanation: This represents an unchecked invariant in the hosted compiler: that any operations
        // which import types (and resolve assemblies from the tcImports tables) happen on the compilation thread.
        let ctok = AssumeCompilationThreadWithoutEvidence()

        match env.assemblyLoader.FindCcuFromAssemblyRef (ctok, m, assemblyRef) with
        | UnresolvedCcu _ -> false
        | ResolvedCcu _ -> true

    match scoref with
    | ILScopeRef.Local
    | ILScopeRef.Module _ -> true
    | ILScopeRef.Assembly assemblyRef -> isResolved assemblyRef
    | ILScopeRef.PrimaryAssembly -> isResolved env.g.ilg.primaryAssemblyRef

/// Import a reference to a type definition, given the AbstractIL data for the type reference
let ImportTypeRefData (env: ImportMap) m (scoref, path, typeName) =

    let findCcu assemblyRef =
        // Explanation: This represents an unchecked invariant in the hosted compiler: that any operations
        // which import types (and resolve assemblies from the tcImports tables) happen on the compilation thread.
        let ctok = AssumeCompilationThreadWithoutEvidence()

        env.assemblyLoader.FindCcuFromAssemblyRef (ctok, m, assemblyRef)

    let ccu =
        match scoref with
        | ILScopeRef.Local    -> error(InternalError("ImportILTypeRef: unexpected local scope", m))
        | ILScopeRef.Module _ -> error(InternalError("ImportILTypeRef: reference found to a type in an auxiliary module", m))
        | ILScopeRef.Assembly assemblyRef -> findCcu assemblyRef
        | ILScopeRef.PrimaryAssembly -> findCcu env.g.ilg.primaryAssemblyRef

    // Do a dereference of a fake tcref for the type just to check it exists in the target assembly and to find
    // the corresponding Tycon.
    let ccu =
        match ccu with
        | ResolvedCcu ccu->ccu
        | UnresolvedCcu ccuName ->
            error (Error(FSComp.SR.impTypeRequiredUnavailable(typeName, ccuName), m))
    let fakeTyconRef = mkNonLocalTyconRef (mkNonLocalEntityRef ccu path) typeName
    let tycon =
        try
            fakeTyconRef.Deref
        with _ ->
            error (Error(FSComp.SR.impReferencedTypeCouldNotBeFoundInAssembly(String.concat "." (Array.append path  [| typeName |]), ccu.AssemblyName), m))
#if !NO_TYPEPROVIDERS
    // Validate (once because of caching)
    match tycon.TypeReprInfo with
    | TProvidedTypeRepr info ->
            //printfn "ImportTypeRefData: validating type: typeLogicalName = %A" typeName
            ValidateProvidedTypeAfterStaticInstantiation(m, info.ProvidedType, path, typeName)
    | _ ->
            ()
#endif
    match tryRescopeEntity ccu tycon with
    | ValueNone -> error (Error(FSComp.SR.impImportedAssemblyUsesNotPublicType(String.concat "." (Array.toList path@[typeName])), m))
    | ValueSome tcref -> tcref


/// Import a reference to a type definition, given an AbstractIL ILTypeRef, without caching
//
// Note, the type names that flow to the point include the "mangled" type names used for static parameters for provided types.
// For example,
//       Foo.Bar,"1.0"
// This is because ImportProvidedType goes via Abstract IL type references.
let ImportILTypeRefUncached (env: ImportMap) m (tref: ILTypeRef) =
    let path, typeName =
        match tref.Enclosing with
        | [] ->
            splitILTypeNameWithPossibleStaticArguments tref.Name
        | h :: t ->
            let nsp, tname = splitILTypeNameWithPossibleStaticArguments h
            // Note, subsequent type names do not need to be split, only the first
            [| yield! nsp; yield tname; yield! t |], tref.Name

    ImportTypeRefData (env: ImportMap) m (tref.Scope, path, typeName)


/// Import a reference to a type definition, given an AbstractIL ILTypeRef, with caching
let ImportILTypeRef (env: ImportMap) m (tref: ILTypeRef) =
    match env.ILTypeRefToTyconRefCache.TryGetValue tref with
    | true, tcref -> tcref
    | _ ->
        let tcref = ImportILTypeRefUncached  env m tref
        env.ILTypeRefToTyconRefCache[tref] <- tcref
        tcref

/// Import a reference to a type definition, given an AbstractIL ILTypeRef, with caching
let CanImportILTypeRef (env: ImportMap) m (tref: ILTypeRef) =
    env.ILTypeRefToTyconRefCache.ContainsKey(tref) || CanImportILScopeRef env m tref.Scope

/// Import a type, given an AbstractIL ILTypeRef and an F# type instantiation.
///
/// Prefer the F# abbreviation for some built-in types, e.g. 'string' rather than
/// 'System.String', since we prefer the F# abbreviation to the .NET equivalents.
let ImportTyconRefApp (env: ImportMap) tcref tyargs nullness =
    env.g.improveType tcref tyargs nullness


module Nullness =

    open FSharp.Compiler.AbstractIL.Diagnostics

    let arrayWithByte0 = [|0uy|]
    let arrayWithByte1 = [|1uy|]
    let arrayWithByte2 = [|2uy|]

    let knownAmbivalent = NullnessInfo.AmbivalentToNull |> Nullness.Known
    let knownWithoutNull = NullnessInfo.WithoutNull |> Nullness.Known
    let knownNullable = NullnessInfo.WithNull |> Nullness.Known

    let mapping byteValue =
        match byteValue with
        | 0uy -> knownAmbivalent
        | 1uy -> knownWithoutNull
        | 2uy -> knownNullable
        | _ ->
            dprintfn "%i was passed to Nullness mapping, this is not a valid value" byteValue
            knownAmbivalent

    let isByte (g:TcGlobals) (ilgType:ILType) =
        g.ilg.typ_Byte.BasicQualifiedName = ilgType.BasicQualifiedName

    let tryParseAttributeDataToNullableByteFlags (g:TcGlobals) attrData =
        match attrData with
        | None -> ValueNone
        | Some ([ILAttribElem.Byte 0uy],_) -> ValueSome arrayWithByte0
        | Some ([ILAttribElem.Byte 1uy],_) -> ValueSome arrayWithByte1
        | Some ([ILAttribElem.Byte 2uy],_) -> ValueSome arrayWithByte2
        | Some ([ILAttribElem.Array(byteType,listOfBytes)],_) when isByte g byteType ->
            listOfBytes
            |> Array.ofList
            |> Array.choose(function | ILAttribElem.Byte b -> Some b | _ -> None)
            |> ValueSome

        | _ -> ValueNone

    [<Struct;NoEquality;NoComparison>]
    type AttributesFromIL = AttributesFromIL of metadataIndex:int * attrs:ILAttributesStored
        with
            member this.Read() =  match this with| AttributesFromIL(idx,attrs) -> attrs.GetCustomAttrs(idx)
            member this.GetNullable(g:TcGlobals) =
                match g.attrib_NullableAttribute_opt with
                | None -> ValueNone
                | Some n ->
                    TryDecodeILAttribute n.TypeRef (this.Read())
                    |> tryParseAttributeDataToNullableByteFlags g

            member this.GetNullableContext(g:TcGlobals) =
                match g.attrib_NullableContextAttribute_opt with
                | None -> ValueNone
                | Some n ->
                    TryDecodeILAttribute n.TypeRef (this.Read())
                    |> tryParseAttributeDataToNullableByteFlags g

    [<Struct;NoEquality;NoComparison>]
    type NullableContextSource =
        | FromClass of AttributesFromIL
        | FromMethodAndClass of methodAttrs:AttributesFromIL * classAttrs:AttributesFromIL

    [<Struct;NoEquality;NoComparison>]
    type NullableAttributesSource =
        { DirectAttributes: AttributesFromIL
          Fallback : NullableContextSource}
          with
            member this.GetFlags(g:TcGlobals) =
                let fallback = this.Fallback
                this.DirectAttributes.GetNullable(g)
                |> ValueOption.orElseWith(fun () ->
                    match fallback with
                    | FromClass attrs -> attrs.GetNullableContext(g)
                    | FromMethodAndClass(methodCtx,classCtx) ->
                        methodCtx.GetNullableContext(g)
                        |> ValueOption.orElseWith (fun () -> classCtx.GetNullableContext(g)))
                |> ValueOption.defaultValue arrayWithByte0
            static member Empty =
                let emptyFromIL = AttributesFromIL(0,Given(ILAttributes.Empty))
                {DirectAttributes = emptyFromIL; Fallback = FromClass(emptyFromIL)}

    [<Struct;NoEquality;NoComparison>]
    type NullableFlags = {Data : byte[]; Idx : int }
(* Nullness logic for generic arguments:
The data which comes from NullableAttribute back might be a flat array, or a scalar (which represents a virtual array of unknown size)
The array is passed trough all generic typars depth first , e.g.  List<Tuple<Map<int,string>,Uri>>
        -- see here how the array indexes map to types above:   [| 0    1     2   3    4      5 |]
For value types, a value is passed even though it is always 0
*)
        with
            member this.GetNullness() =
                match this.Data.Length with
                // No nullable data nor parent context -> we cannot tell
                | 0 -> knownAmbivalent
                // A scalar value from attributes, cover type and all it's potential typars
                | 1 -> this.Data[0] |> mapping
                // We have a bigger array, indexes map to typars in a depth-first fashion
                | n when n > this.Idx -> this.Data[this.Idx] |> mapping
                // This is an erroneous case, we need more nullnessinfo then the metadata contains
                | _ ->
                    // TODO nullness - once being confident that our bugs are solved and what remains are incoming metadata bugs, remove failwith and replace with dprintfn
                    // Testing with .NET compilers other then Roslyn producing nullness metadata?
                    failwithf "Length of Nullable metadata and needs of its processing do not match:  %A" this
                    knownAmbivalent

            member this.Advance() = {Data = this.Data; Idx = this.Idx + 1}

    let inline isSystemNullable (tspec:ILTypeSpec) =
        match tspec.Name,tspec.Enclosing with
        | "Nullable`1",["System"] -> true
        | "System.Nullable`1",[] -> true
        | _ -> false

    let inline evaluateFirstOrderNullnessAndAdvance (ilt:ILType) (flags:NullableFlags) =
        match ilt with
        | ILType.Value tspec when tspec.GenericArgs.IsEmpty -> KnownWithoutNull, flags
        // System.Nullable is special-cased in C# spec for nullness metadata.
        // You CAN assign 'null' to it, and when boxed, it CAN be boxed to 'null'.
        | ILType.Value tspec when isSystemNullable tspec -> KnownWithoutNull, flags
        | ILType.Value _  -> KnownWithoutNull, flags.Advance()
        | _ -> flags.GetNullness(), flags.Advance()

/// Import an IL type as an F# type.
let rec ImportILType (env: ImportMap) m tinst ty =
    match ty with
    | ILType.Void ->
        env.g.unit_ty

    | ILType.Array(bounds, ty) ->
        let n = bounds.Rank
        let elemTy = ImportILType env m tinst ty
        mkArrayTy env.g n Nullness.knownAmbivalent elemTy m

    | ILType.Boxed  tspec | ILType.Value tspec ->
        let tcref = ImportILTypeRef env m tspec.TypeRef
        let inst = tspec.GenericArgs |> List.map (ImportILType env m tinst)
        ImportTyconRefApp env tcref inst Nullness.knownAmbivalent

    | ILType.Byref ty -> mkByrefTy env.g (ImportILType env m tinst ty)
    | ILType.Ptr ILType.Void  when env.g.voidptr_tcr.CanDeref -> mkVoidPtrTy env.g
    | ILType.Ptr ty  -> mkNativePtrTy env.g (ImportILType env m tinst ty)
    | ILType.FunctionPointer _ -> env.g.nativeint_ty (* failwith "cannot import this kind of type (ptr, fptr)" *)
    | ILType.Modified(_, _, ty) ->
         // All custom modifiers are ignored
         ImportILType env m tinst ty

    | ILType.TypeVar u16 ->
        let ty =
            try
                List.item (int u16) tinst
            with _ ->
                error(Error(FSComp.SR.impNotEnoughTypeParamsInScopeWhileImporting(), m))

        let tyWithNullness = addNullnessToTy Nullness.knownAmbivalent ty
        tyWithNullness

/// Import an IL type as an F# type.
let rec ImportILTypeWithNullness (env: ImportMap) m tinst (nf:Nullness.NullableFlags) ty : struct(TType*Nullness.NullableFlags) =
    match ty with
    | ILType.Void ->
        env.g.unit_ty,nf

    | ILType.Array(bounds, innerTy) ->
        let n = bounds.Rank
        let (arrayNullness,nf) = Nullness.evaluateFirstOrderNullnessAndAdvance ty nf
        let struct(elemTy,nf) = ImportILTypeWithNullness env m tinst nf innerTy
        mkArrayTy env.g n arrayNullness elemTy m, nf

    | ILType.Boxed  tspec | ILType.Value tspec ->
        let tcref = ImportILTypeRef env m tspec.TypeRef
        let (typeRefNullness,nf) = Nullness.evaluateFirstOrderNullnessAndAdvance ty nf
        let struct(inst,nullableFlagsLeft) = (nf,tspec.GenericArgs) ||> List.vMapFold (fun nf current -> ImportILTypeWithNullness env m tinst nf current )

        ImportTyconRefApp env tcref inst typeRefNullness, nullableFlagsLeft

    | ILType.Byref ty ->
        let struct(ttype,nf) = ImportILTypeWithNullness env m tinst nf ty
        mkByrefTy env.g ttype, nf

    | ILType.Ptr ILType.Void  when env.g.voidptr_tcr.CanDeref -> mkVoidPtrTy env.g, nf

    | ILType.Ptr ty  ->
        let struct(ttype,nf) = ImportILTypeWithNullness env m tinst nf ty
        mkNativePtrTy env.g ttype, nf

    | ILType.FunctionPointer _ -> env.g.nativeint_ty, nf (* failwith "cannot import this kind of type (ptr, fptr)" *)

    | ILType.Modified(_, _, ty) ->
         // All custom modifiers are ignored
         ImportILTypeWithNullness env m tinst nf ty

    | ILType.TypeVar u16 ->
        let ttype =
            try
                List.item (int u16) tinst
            with _ ->
                error(Error(FSComp.SR.impNotEnoughTypeParamsInScopeWhileImporting(), m))

        let (typeVarNullness,nf) = Nullness.evaluateFirstOrderNullnessAndAdvance ty nf
        addNullnessToTy typeVarNullness ttype, nf

/// Determines if an IL type can be imported as an F# type
let rec CanImportILType (env: ImportMap) m ty =
    match ty with
    | ILType.Void -> true
    | ILType.Array(_bounds, ety) -> CanImportILType env m ety
    | ILType.Boxed  tspec
    | ILType.Value tspec ->
        CanImportILTypeRef env m tspec.TypeRef
        && tspec.GenericArgs |> List.forall (CanImportILType env m)

    | ILType.Byref ety -> CanImportILType env m ety
    | ILType.Ptr ety  -> CanImportILType env m ety
    | ILType.FunctionPointer _ -> true
    | ILType.Modified(_, _, ety) -> CanImportILType env m ety
    | ILType.TypeVar _u16 -> true

#if !NO_TYPEPROVIDERS

/// Import a provided type reference as an F# type TyconRef
let ImportProvidedNamedType (env: ImportMap) (m: range) (st: Tainted<ProvidedType>) =
    // See if a reverse-mapping exists for a generated/relocated System.Type
    match st.PUntaint((fun st -> st.TryGetTyconRef()), m) with
    | Some x -> (x :?> TyconRef)
    | None ->
        let tref = GetILTypeRefOfProvidedType (st, m)
        ImportILTypeRef env m tref

/// Import a provided type as an AbstractIL type
let rec ImportProvidedTypeAsILType (env: ImportMap) (m: range) (st: Tainted<ProvidedType>) =
    if st.PUntaint ((fun x -> x.IsVoid), m) then ILType.Void
    elif st.PUntaint((fun st -> st.IsGenericParameter), m) then
        mkILTyvarTy (uint16 (st.PUntaint((fun st -> st.GenericParameterPosition), m)))
    elif st.PUntaint((fun st -> st.IsArray), m) then
        let et = ImportProvidedTypeAsILType env m (st.PApply((fun st -> !! st.GetElementType()), m))
        ILType.Array(ILArrayShape.FromRank (st.PUntaint((fun st -> st.GetArrayRank()), m)), et)
    elif st.PUntaint((fun st -> st.IsByRef), m) then
        let et = ImportProvidedTypeAsILType env m (st.PApply((fun st -> !! st.GetElementType()), m))
        ILType.Byref et
    elif st.PUntaint((fun st -> st.IsPointer), m) then
        let et = ImportProvidedTypeAsILType env m (st.PApply((fun st -> !! st.GetElementType()), m))
        ILType.Ptr et
    else
        let gst, genericArgs =
            if st.PUntaint((fun st -> st.IsGenericType), m) then
                let args = st.PApplyArray((fun st -> st.GetGenericArguments()), "GetGenericArguments", m) |> Array.map (ImportProvidedTypeAsILType env m) |> List.ofArray
                let gst = st.PApply((fun st -> st.GetGenericTypeDefinition()), m)
                gst, args
            else
                st, []
        let tref = GetILTypeRefOfProvidedType (gst, m)
        let tcref = ImportProvidedNamedType env m gst
        let tps = tcref.Typars m
        if tps.Length <> genericArgs.Length then
           error(Error(FSComp.SR.impInvalidNumberOfGenericArguments(tcref.CompiledName, tps.Length, genericArgs.Length), m))
        // We're converting to an IL type, where generic arguments are erased
        let genericArgs = List.zip tps genericArgs |> List.filter (fun (tp, _) -> not tp.IsErased) |> List.map snd

        let tspec = mkILTySpec(tref, genericArgs)
        if st.PUntaint((fun st -> st.IsValueType), m) then
            ILType.Value tspec
        else
            mkILBoxedType tspec

/// Import a provided type as an F# type.
let rec ImportProvidedType (env: ImportMap) (m: range) (* (tinst: TypeInst) *) (st: Tainted<ProvidedType>) =

    // Explanation: The two calls below represent am unchecked invariant of the hosted compiler:
    // that type providers are only activated on the CompilationThread. This invariant is not currently checked
    // via CompilationThreadToken passing. We leave the two calls below as a reminder of this.
    //
    // This function is one major source of type provider activations, but not the only one: almost
    // any call in the 'TypeProviders' module is a potential type provider activation.
    let ctok = AssumeCompilationThreadWithoutEvidence ()
    RequireCompilationThread ctok

    let g = env.g
    if st.PUntaint((fun st -> st.IsArray), m) then
        let elemTy = ImportProvidedType env m (* tinst *) (st.PApply((fun st -> !! st.GetElementType()), m))
        // TODO Nullness - integration into type providers as a separate feature for later.
        let nullness = Nullness.knownAmbivalent
        mkArrayTy g (st.PUntaint((fun st -> st.GetArrayRank()), m)) nullness elemTy m
    elif st.PUntaint((fun st -> st.IsByRef), m) then
        let elemTy = ImportProvidedType env m (* tinst *) (st.PApply((fun st -> !! st.GetElementType()), m))
        mkByrefTy g elemTy
    elif st.PUntaint((fun st -> st.IsPointer), m) then
        let elemTy = ImportProvidedType env m (* tinst *) (st.PApply((fun st -> !! st.GetElementType()), m))
        if isUnitTy g elemTy || isVoidTy g elemTy && g.voidptr_tcr.CanDeref then
            mkVoidPtrTy g
        else
            mkNativePtrTy g elemTy
    else

        // REVIEW: Extension type could try to be its own generic arg (or there could be a type loop)
        let tcref, genericArgs =
            if st.PUntaint((fun st -> st.IsGenericType), m) then
                let tcref = ImportProvidedNamedType env m (st.PApply((fun st -> st.GetGenericTypeDefinition()), m))
                let args = st.PApplyArray((fun st -> st.GetGenericArguments()), "GetGenericArguments", m) |> Array.map (ImportProvidedType env m (* tinst *) ) |> List.ofArray
                tcref, args
            else
                let tcref = ImportProvidedNamedType env m st
                tcref, []

        let genericArgsLength = genericArgs.Length
        /// Adjust for the known primitive numeric types that accept units of measure.
        let tcref =
            if genericArgsLength = 1 then
                // real
                if tyconRefEq g tcref g.system_Double_tcref then g.pfloat_tcr
                elif tyconRefEq g tcref g.system_Single_tcref then g.pfloat32_tcr
                elif tyconRefEq g tcref g.system_Decimal_tcref then g.pdecimal_tcr
                // signed
                elif tyconRefEq g tcref g.system_Int16_tcref then g.pint16_tcr
                elif tyconRefEq g tcref g.system_Int32_tcref then g.pint_tcr
                elif tyconRefEq g tcref g.system_Int64_tcref then g.pint64_tcr
                elif tyconRefEq g tcref g.system_SByte_tcref then g.pint8_tcr
                // unsigned
                elif tyconRefEq g tcref g.system_UInt16_tcref then g.puint16_tcr
                elif tyconRefEq g tcref g.system_UInt32_tcref then g.puint_tcr
                elif tyconRefEq g tcref g.system_UInt64_tcref then g.puint64_tcr
                elif tyconRefEq g tcref g.system_Byte_tcref then g.puint8_tcr
                //native
                elif tyconRefEq g tcref g.system_IntPtr_tcref then g.pnativeint_tcr
                elif tyconRefEq g tcref g.system_UIntPtr_tcref then g.punativeint_tcr
                // other
                else tcref
            else
                tcref

        let tps = tcref.Typars m
        if tps.Length <> genericArgsLength then
           error(Error(FSComp.SR.impInvalidNumberOfGenericArguments(tcref.CompiledName, tps.Length, genericArgsLength), m))

        let genericArgs =
            (tps, genericArgs) ||> List.map2 (fun tp genericArg ->
                if tp.Kind = TyparKind.Measure then
                    let rec conv ty =
                        match ty with
                        | TType_app (tcref, [ty1;ty2], _) when tyconRefEq g tcref g.measureproduct_tcr ->
                            let ms1: Measure = conv ty1
                            let ms2: Measure = conv ty2
                            Measure.Prod(ms1, ms2, unionRanges ms1.Range ms2.Range)
                        | TType_app (tcref, [ty1], _) when tyconRefEq g tcref g.measureinverse_tcr -> Measure.Inv (conv ty1)
                        | TType_app (tcref, [], _) when tyconRefEq g tcref g.measureone_tcr -> Measure.One(tcref.Range)
                        | TType_app (tcref, [], _) when tcref.TypeOrMeasureKind = TyparKind.Measure -> Measure.Const(tcref, tcref.Range)
                        | TType_app (tcref, _, _) ->
                            errorR(Error(FSComp.SR.impInvalidMeasureArgument1(tcref.CompiledName, tp.Name), m))
                            Measure.One(tcref.Range)
                        | _ ->
                            errorR(Error(FSComp.SR.impInvalidMeasureArgument2(tp.Name), m))
                            Measure.One(Range.Zero)

                    TType_measure (conv genericArg)
                else
                    genericArg)

        // TODO Nullness - integration into type providers as a separate feature for later.
        let nullness = Nullness.knownAmbivalent

        ImportTyconRefApp env tcref genericArgs nullness

/// Import a provided method reference as an Abstract IL method reference
let ImportProvidedMethodBaseAsILMethodRef (env: ImportMap) (m: range) (mbase: Tainted<ProvidedMethodBase>) =
     let tref = GetILTypeRefOfProvidedType (mbase.PApply((fun mbase -> nonNull<ProvidedType> mbase.DeclaringType), m), m)

     let mbase =
         // Find the formal member corresponding to the called member
         match mbase.OfType<ProvidedMethodInfo>() with
         | Some minfo when
                    minfo.PUntaint((fun minfo -> minfo.IsGenericMethod|| (nonNull<ProvidedType> minfo.DeclaringType).IsGenericType), m) ->

                let declaringType = minfo.PApply((fun minfo -> nonNull<ProvidedType> minfo.DeclaringType), m)

                let declaringGenericTypeDefn =
                    if declaringType.PUntaint((fun t -> t.IsGenericType), m) then
                        declaringType.PApply((fun declaringType -> declaringType.GetGenericTypeDefinition()), m)
                    else
                        declaringType

                let methods = declaringGenericTypeDefn.PApplyArray((fun x -> x.GetMethods()), "GetMethods", m)
                let metadataToken = minfo.PUntaint((fun minfo -> minfo.MetadataToken), m)
                let found = methods |> Array.tryFind (fun x -> x.PUntaint((fun x -> x.MetadataToken), m) = metadataToken)
                match found with
                | Some found -> found.Coerce(m)
                | None ->
                    let methodName = minfo.PUntaint((fun minfo -> minfo.Name), m)
                    let typeName = declaringGenericTypeDefn.PUntaint((fun declaringGenericTypeDefn -> string declaringGenericTypeDefn.FullName), m)
                    error(Error(FSComp.SR.etIncorrectProvidedMethod(DisplayNameOfTypeProvider(minfo.TypeProvider, m), methodName, metadataToken, typeName), m))
         | _ ->
         match mbase.OfType<ProvidedConstructorInfo>() with
         | Some cinfo when cinfo.PUntaint((fun x -> (nonNull<ProvidedType> x.DeclaringType).IsGenericType), m) ->
                let declaringType = cinfo.PApply((fun x -> nonNull<ProvidedType> x.DeclaringType), m)
                let declaringGenericTypeDefn =  declaringType.PApply((fun x -> x.GetGenericTypeDefinition()), m)

                // We have to find the uninstantiated formal signature corresponding to this instantiated constructor.
                // Annoyingly System.Reflection doesn't give us a MetadataToken to compare on, so we have to look by doing
                // the instantiation and comparing..
                let found =
                    let ctors = declaringGenericTypeDefn.PApplyArray((fun x -> x.GetConstructors()), "GetConstructors", m)

                    let actualParamTys =
                        [ for p in cinfo.PApplyArray((fun x -> x.GetParameters()), "GetParameters", m) do
                            ImportProvidedType env m (p.PApply((fun p -> p.ParameterType), m)) ]

                    let actualGenericArgs = argsOfAppTy env.g (ImportProvidedType env m declaringType)

                    ctors |> Array.tryFind (fun ctor ->
                       let formalParamTysAfterInst =
                           [ for p in ctor.PApplyArray((fun x -> x.GetParameters()), "GetParameters", m) do
                                let ilFormalTy = ImportProvidedTypeAsILType env m (p.PApply((fun p -> p.ParameterType), m))
                                // TODO Nullness - integration into type providers as a separate feature for later.
                                yield ImportILType env m actualGenericArgs ilFormalTy ]

                       (formalParamTysAfterInst, actualParamTys) ||>  List.lengthsEqAndForall2 (typeEquiv env.g))

                match found with
                | Some found -> found.Coerce(m)
                | None ->
                    let typeName = declaringGenericTypeDefn.PUntaint((fun x -> string x.FullName), m)
                    error(Error(FSComp.SR.etIncorrectProvidedConstructor(DisplayNameOfTypeProvider(cinfo.TypeProvider, m), typeName), m))
         | _ -> mbase

     let retTy =
         match mbase.OfType<ProvidedMethodInfo>() with
         |  Some minfo -> minfo.PApply((fun minfo -> minfo.ReturnType), m)
         |  None ->
            match mbase.OfType<ProvidedConstructorInfo>() with
            | Some _  -> mbase.PApply((fun _ -> ProvidedType.Void), m)
            | _ -> failwith "ImportProvidedMethodBaseAsILMethodRef - unexpected"

     let genericArity =
        if mbase.PUntaint((fun x -> x.IsGenericMethod), m) then
            mbase.PApplyArray((fun x -> x.GetGenericArguments()),"GetGenericArguments", m).Length
        else 0

     let callingConv = (if mbase.PUntaint((fun x -> x.IsStatic), m) then ILCallingConv.Static else ILCallingConv.Instance)

     let ilParamTys =
         [ for p in mbase.PApplyArray((fun x -> x.GetParameters()), "GetParameters", m) do
              yield ImportProvidedTypeAsILType env m (p.PApply((fun p -> p.ParameterType), m)) ]

     let ilRetTy = ImportProvidedTypeAsILType env m retTy

     mkILMethRef (tref, callingConv, mbase.PUntaint((fun x -> x.Name), m), genericArity, ilParamTys, ilRetTy)
#endif

//-------------------------------------------------------------------------
// Load an IL assembly into the compiler's internal data structures
// Careful use is made of laziness here to ensure we don't read the entire IL
// assembly on startup.
//--------------------------------------------------------------------------

/// Import a set of Abstract IL generic parameter specifications as a list of new
/// F# generic parameters.
///
/// Fixup the constraints so that any references to the generic parameters
/// in the constraints now refer to the new generic parameters.
let ImportILGenericParameters amap m scoref tinst (nullableFallback:Nullness.NullableContextSource) (gps: ILGenericParameterDefs) =
    match gps with
    | [] -> []
    | _ ->
        let amap : ImportMap = amap()
        let tps = gps |> List.map (fun gp -> Construct.NewRigidTypar gp.Name m)

        let tptys = tps |> List.map mkTyparTy
        let importInst = tinst@tptys
        (tps, gps) ||> List.iter2 (fun tp gp ->
            if gp.Variance = ILGenericVariance.ContraVariant then
                tp.MarkAsContravariant()
            let constraints =
                [
                  if amap.g.langFeatureNullness && amap.g.checkNullness then
                    let nullness =
                        {   Nullness.DirectAttributes = Nullness.AttributesFromIL(gp.MetadataIndex,gp.CustomAttrsStored)
                            Nullness.Fallback = nullableFallback }

                    match nullness.GetFlags(amap.g) with
                    |  [|1uy|] -> TyparConstraint.NotSupportsNull(m)
                    // In F#, 'SupportsNull' has the meaning of "must support null as a value". In C#, Nullable(2) is an allowance, not a requirement.
                    //|  [|2uy|] -> TyparConstraint.SupportsNull(m)
                    | _ -> ()

                  if gp.CustomAttrs |> TryFindILAttribute amap.g.attrib_IsUnmanagedAttribute then
                    TyparConstraint.IsUnmanaged(m)
                  if gp.HasDefaultConstructorConstraint then
                    TyparConstraint.RequiresDefaultConstructor(m)
                  if gp.HasNotNullableValueTypeConstraint then
                    TyparConstraint.IsNonNullableStruct(m)
                  if gp.HasReferenceTypeConstraint then
                    TyparConstraint.IsReferenceType(m)
                  if gp.HasAllowsRefStruct then
                    TyparConstraint.AllowsRefStruct(m)
                  for ilTy in gp.Constraints do
                    TyparConstraint.CoercesTo(ImportILType amap m importInst (rescopeILType scoref ilTy), m) ]

            tp.SetConstraints constraints)
        tps

/// Given a list of items each keyed by an ordered list of keys, apply 'nodef' to the each group
/// with the same leading key. Apply 'tipf' to the elements where the keylist is empty, and return
/// the overall results.  Used to bucket types, so System.Char and System.Collections.Generic.List
/// both get initially bucketed under 'System'.
let multisetDiscriminateAndMap nodef tipf (items: ('Key list * 'Value) list) =
    // Find all the items with an empty key list and call 'tipf'
    let tips =
        [ for keylist, v in items do
             match keylist with
             | [] -> yield tipf v
             | _ -> () ]

    // Find all the items with a non-empty key list. Bucket them together by
    // the first key. For each bucket, call 'nodef' on that head key and the bucket.
    let nodes =
        let buckets = Dictionary<_, _>(10)
        for keylist, v in items do
            match keylist with
            | [] -> ()
            | key :: rest ->
                buckets[key] <-
                    match buckets.TryGetValue key with
                    | true, b -> (rest, v) :: b
                    | _ -> [rest, v]

        [ for KeyValue(key, items) in buckets -> nodef key items ]

    tips @ nodes

/// Import an IL type definition as a new F# TAST Entity node.
let rec ImportILTypeDef amap m scoref (cpath: CompilationPath) enc nm (tdef: ILTypeDef)  =
    let lazyModuleOrNamespaceTypeForNestedTypes =
        InterruptibleLazy(fun _ ->
            let cpath = cpath.NestedCompPath nm ModuleOrType
            ImportILTypeDefs amap m scoref cpath (enc@[tdef]) tdef.NestedTypes
        )

    let nullableFallback = Nullness.FromClass(Nullness.AttributesFromIL(tdef.MetadataIndex,tdef.CustomAttrsStored))

    // Add the type itself.
    Construct.NewILTycon
        (Some cpath)
        (nm, m)
        // The read of the type parameters may fail to resolve types. We pick up a new range from the point where that read is forced
        // Make sure we reraise the original exception one occurs - see findOriginalException.
        (LazyWithContext.Create((fun m -> ImportILGenericParameters amap m scoref [] nullableFallback tdef.GenericParams), findOriginalException))
        (scoref, enc, tdef)
        (MaybeLazy.Lazy lazyModuleOrNamespaceTypeForNestedTypes)


/// Import a list of (possibly nested) IL types as a new ModuleOrNamespaceType node
/// containing new entities, bucketing by namespace along the way.
and ImportILTypeDefList amap m (cpath: CompilationPath) enc items =
    // Split into the ones with namespaces and without. Add the ones with namespaces in buckets.
    // That is, discriminate based in the first element of the namespace list (e.g. "System")
    // and, for each bag, fold-in a lazy computation to add the types under that bag .
    //
    // nodef - called for each bucket, where 'n' is the head element of the namespace used
    // as a key in the discrimination, tgs is the remaining descriptors.  We create an entity for 'n'.
    //
    // tipf - called if there are no namespace items left to discriminate on.
    let entities =
        items
        |> multisetDiscriminateAndMap
            (fun n tgs ->
                let modty = InterruptibleLazy(fun _ -> ImportILTypeDefList amap m (cpath.NestedCompPath n (Namespace true)) enc tgs)
                Construct.NewModuleOrNamespace (Some cpath) taccessPublic (mkSynId m n) XmlDoc.Empty [] (MaybeLazy.Lazy modty))
            (fun (n, info: InterruptibleLazy<_>) ->
                let (scoref2, lazyTypeDef: ILPreTypeDef) = info.Force()
                ImportILTypeDef amap m scoref2 cpath enc n (lazyTypeDef.GetTypeDef()))

    let kind = match enc with [] -> Namespace true | _ -> ModuleOrType
    Construct.NewModuleOrNamespaceType kind entities []

/// Import a table of IL types as a ModuleOrNamespaceType.
///
and ImportILTypeDefs amap m scoref cpath enc (tdefs: ILTypeDefs) =
    // We be very careful not to force a read of the type defs here
    tdefs.AsArrayOfPreTypeDefs()
    |> Array.map (fun pre -> (pre.Namespace, (pre.Name, notlazy(scoref, pre))))
    |> Array.toList
    |> ImportILTypeDefList amap m cpath enc

/// Import the main type definitions in an IL assembly.
///
/// Example: for a collection of types "System.Char", "System.Int32" and "Library.C"
/// the return ModuleOrNamespaceType will contain namespace entities for "System" and "Library", which in turn contain
/// type definition entities for ["Char"; "Int32"]  and ["C"] respectively.
let ImportILAssemblyMainTypeDefs amap m scoref modul =
    modul.TypeDefs |> ImportILTypeDefs amap m scoref (CompPath(scoref, SyntaxAccess.Unknown, [])) []

/// Import the "exported types" table for multi-module assemblies.
let ImportILAssemblyExportedType amap m auxModLoader (scoref: ILScopeRef) (exportedType: ILExportedTypeOrForwarder) =
    // Forwarders are dealt with separately in the ref->def dereferencing logic in tast.fs as they effectively give rise to type equivalences
    if exportedType.IsForwarder then
        []
    else
        let ns, n = splitILTypeName exportedType.Name
        let info =
            InterruptibleLazy (fun _ ->
                match
                    (try
                        let modul = auxModLoader exportedType.ScopeRef
                        let ptd = mkILPreTypeDefComputed (ns, n, (fun () -> modul.TypeDefs.FindByName exportedType.Name))
                        Some ptd
                     with :? KeyNotFoundException -> None)
                with
                | None ->
                    error(Error(FSComp.SR.impReferenceToDllRequiredByAssembly(exportedType.ScopeRef.QualifiedName, scoref.QualifiedName, exportedType.Name), m))
                | Some preTypeDef ->
                    scoref, preTypeDef
            )

        [ ImportILTypeDefList amap m (CompPath(scoref, SyntaxAccess.Unknown, [])) [] [(ns, (n, info))]  ]

/// Import the "exported types" table for multi-module assemblies.
let ImportILAssemblyExportedTypes amap m auxModLoader scoref (exportedTypes: ILExportedTypesAndForwarders) =
    [ for exportedType in exportedTypes.AsList() do
         yield! ImportILAssemblyExportedType amap m auxModLoader scoref exportedType ]

/// Import both the main type definitions and the "exported types" table, i.e. all the
/// types defined in an IL assembly.
let ImportILAssemblyTypeDefs (amap, m, auxModLoader, aref, mainmod: ILModuleDef) =
    let scoref = ILScopeRef.Assembly aref
    let mtypsForExportedTypes = ImportILAssemblyExportedTypes amap m auxModLoader scoref mainmod.ManifestOfAssembly.ExportedTypes
    let mainmod = ImportILAssemblyMainTypeDefs amap m scoref mainmod
    CombineCcuContentFragments (mainmod :: mtypsForExportedTypes)

/// Import the type forwarder table for an IL assembly
let ImportILAssemblyTypeForwarders (amap, m, exportedTypes: ILExportedTypesAndForwarders): CcuTypeForwarderTable =
    let rec addToTree tree path item value =
        match path with
        | [] ->
            { tree with
                Children =
                    tree.Children.Add(
                        item,
                        { Value = Some value
                          Children = ImmutableDictionary.Empty }
                    ) }
        | nodeKey :: rest ->
            match tree.Children.TryGetValue(nodeKey) with
            | true, subTree -> { tree with Children = tree.Children.SetItem(nodeKey, addToTree subTree rest item value) }
            | false, _ -> { tree with Children = tree.Children.Add(nodeKey, mkTreeWith rest item value) }

    and mkTreeWith path item value =
        match path with
        | [] ->
            { Value = None
              Children =
                ImmutableDictionary.Empty.Add(
                    item,
                    { Value = Some value
                      Children = ImmutableDictionary.Empty }
                ) }
        | nodeKey :: rest ->
            { Value = None
              Children = ImmutableDictionary.Empty.Add(nodeKey, mkTreeWith rest item value) }

    let rec addNested
        (exportedType: ILExportedTypeOrForwarder)
        (nets: ILNestedExportedTypes)
        (enc: string list)
        (tree: CcuTypeForwarderTree)
        : CcuTypeForwarderTree =
        (tree, nets.AsList())
        ||> List.fold(fun tree net ->
            let tcref = lazy ImportILTypeRefUncached (amap ()) m (ILTypeRef.Create(exportedType.ScopeRef, enc, net.Name))
            addToTree tree enc exportedType.Name tcref
            |> addNested exportedType net.Nested [yield! enc; yield net.Name])

    match exportedTypes.AsList() with
    | [] -> CcuTypeForwarderTable.Empty
    | rootTypes ->
        ({ Value = None; Children = ImmutableDictionary.Empty } , rootTypes)
        ||> List.fold(fun tree exportedType ->
            let ns, n = splitILTypeName exportedType.Name
            let tcref = lazy ImportILTypeRefUncached (amap ()) m (ILTypeRef.Create(exportedType.ScopeRef, [], exportedType.Name))
            addToTree tree ns n tcref
            |> addNested exportedType exportedType.Nested [yield! ns; yield n]
        )
        |> fun root -> { Root = root }

/// Import an IL assembly as a new TAST CCU
let ImportILAssembly(amap: unit -> ImportMap, m, auxModuleLoader, xmlDocInfoLoader: IXmlDocumentationInfoLoader option, ilScopeRef, sourceDir, fileName, ilModule: ILModuleDef, invalidateCcu: IEvent<string>) =
    invalidateCcu |> ignore
    let aref =
        match ilScopeRef with
        | ILScopeRef.Assembly aref -> aref
        | _ -> error(InternalError("ImportILAssembly: cannot reference .NET netmodules directly, reference the containing assembly instead", m))
    let nm = aref.Name
    let mty = ImportILAssemblyTypeDefs(amap, m, auxModuleLoader, aref, ilModule)
    let forwarders =
        match ilModule.Manifest with
        | None -> CcuTypeForwarderTable.Empty
        | Some manifest -> ImportILAssemblyTypeForwarders(amap, m, manifest.ExportedTypes)

    let ccuData: CcuData =
        { IsFSharp=false
          UsesFSharp20PlusQuotations=false
#if !NO_TYPEPROVIDERS
          InvalidateEvent=invalidateCcu
          IsProviderGenerated = false
          ImportProvidedType = (fun ty -> ImportProvidedType (amap()) m ty)
#endif
          QualifiedName= Some ilScopeRef.QualifiedName
          Contents = Construct.NewCcuContents ilScopeRef m nm mty
          ILScopeRef = ilScopeRef
          Stamp = newStamp()
          SourceCodeDirectory = sourceDir  // note: not an accurate value, but IL assemblies don't give us this information in any attributes.
          FileName = fileName
          MemberSignatureEquality= (fun ty1 ty2 -> typeEquivAux EraseAll (amap()).g ty1 ty2)
          TryGetILModuleDef = (fun () -> Some ilModule)
          TypeForwarders = forwarders
          XmlDocumentationInfo =
              match xmlDocInfoLoader, fileName with
              | Some xmlDocInfoLoader, Some fileName -> xmlDocInfoLoader.TryLoad(fileName)
              | _ -> None
        }

    CcuThunk.Create(nm, ccuData)

//-------------------------------------------------------------------------
// From IL types to F# types
//-------------------------------------------------------------------------

/// Import an IL type as an F# type. importInst gives the context for interpreting type variables.
let RescopeAndImportILTypeSkipNullness scoref amap m importInst ilTy =
    ilTy |> rescopeILType scoref |> ImportILType amap m importInst

let RescopeAndImportILType scoref (amap:ImportMap) m importInst (nullnessSource:Nullness.NullableAttributesSource) ilTy =
    let g = amap.g
    if g.langFeatureNullness && g.checkNullness then
        let flags = nullnessSource.GetFlags(g)
        let flags = {Nullness.NullableFlags.Data = flags; Nullness.NullableFlags.Idx = 0}
        let struct(ty,_) = ilTy |> rescopeILType scoref |>  ImportILTypeWithNullness amap m importInst flags
        ty
    else
        RescopeAndImportILTypeSkipNullness scoref amap m importInst ilTy



let CanRescopeAndImportILType scoref amap m ilTy =
    ilTy |> rescopeILType scoref |>  CanImportILType amap m
