// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// The ILX generator.
module internal FSharp.Compiler.IlxGen

open System.IO
open System.Reflection
open System.Collections.Generic

open FSharp.Compiler.IO
open Internal.Utilities
open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras

open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.BinaryConstants
open FSharp.Compiler.AbstractIL.ILX
open FSharp.Compiler.AbstractIL.ILX.Types
open FSharp.Compiler.AttributeChecking
open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open FSharp.Compiler.Infos
open FSharp.Compiler.Import
open FSharp.Compiler.LowerStateMachines
open FSharp.Compiler.Syntax
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Text
open FSharp.Compiler.Text.LayoutRender
open FSharp.Compiler.Xml
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TypedTreeOps.DebugPrint
open FSharp.Compiler.TypeHierarchy
open FSharp.Compiler.TypeRelations

let IlxGenStackGuardDepth = StackGuard.GetDepthOption "IlxGen"

let IsNonErasedTypar (tp: Typar) = not tp.IsErased

let DropErasedTypars (tps: Typar list) = tps |> List.filter IsNonErasedTypar

let DropErasedTyargs tys =
    tys
    |> List.filter (fun ty ->
        match ty with
        | TType_measure _ -> false
        | _ -> true)

let AddNonUserCompilerGeneratedAttribs (g: TcGlobals) (mdef: ILMethodDef) = g.AddMethodGeneratedAttributes mdef

let debugDisplayMethodName = "__DebugDisplay"

let useHiddenInitCode = true

let iLdcZero = AI_ldc(DT_I4, ILConst.I4 0)

let iLdcInt64 i = AI_ldc(DT_I8, ILConst.I8 i)

let iLdcDouble i = AI_ldc(DT_R8, ILConst.R8 i)

let iLdcSingle i = AI_ldc(DT_R4, ILConst.R4 i)

/// Make a method that simply loads a field
let mkLdfldMethodDef (ilMethName, reprAccess, isStatic, ilTy, ilFieldName, ilPropType, customAttrs) =
    let ilFieldSpec = mkILFieldSpecInTy (ilTy, ilFieldName, ilPropType)
    let ilReturn = mkILReturn ilPropType

    let ilMethodDef =
        if isStatic then
            let body =
                mkMethodBody (true, [], 2, nonBranchingInstrsToCode [ mkNormalLdsfld ilFieldSpec ], None, None)

            mkILNonGenericStaticMethod (ilMethName, reprAccess, [], ilReturn, body)
        else
            let body =
                mkMethodBody (true, [], 2, nonBranchingInstrsToCode [ mkLdarg0; mkNormalLdfld ilFieldSpec ], None, None)

            mkILNonGenericInstanceMethod (ilMethName, reprAccess, [], ilReturn, body)

    ilMethodDef.With(customAttrs = mkILCustomAttrs customAttrs).WithSpecialName

/// Choose the constructor parameter names for fields
let ChooseParamNames fieldNamesAndTypes =
    let takenFieldNames = fieldNamesAndTypes |> List.map p23 |> Set.ofList

    fieldNamesAndTypes
    |> List.map (fun (ilPropName, ilFieldName, ilPropType) ->
        let lowerPropName = String.uncapitalize ilPropName

        let ilParamName =
            if takenFieldNames.Contains lowerPropName then
                ilPropName
            else
                lowerPropName

        ilParamName, ilFieldName, ilPropType)

/// Approximation for purposes of optimization and giving a warning when compiling definition-only files as EXEs
let rec CheckCodeDoesSomething (code: ILCode) =
    code.Instrs
    |> Array.exists (function
        | AI_ldnull
        | AI_nop
        | AI_pop
        | I_ret
        | I_seqpoint _ -> false
        | _ -> true)

/// Choose the field names for variables captured by closures
let ChooseFreeVarNames takenNames ts =
    let tns = List.map (fun t -> (t, None)) ts

    let rec chooseName names (t, nOpt) =
        let tn =
            match nOpt with
            | None -> t
            | Some n -> t + string n

        if Zset.contains tn names then
            chooseName
                names
                (t,
                 Some(
                     match nOpt with
                     | None -> 0
                     | Some n -> (n + 1)
                 ))
        else
            let names = Zset.add tn names
            tn, names

    let names = Zset.empty String.order |> Zset.addList takenNames
    let ts, _names = List.mapFold chooseName names tns
    ts

/// We can't tailcall to methods taking byrefs. This helper helps search for them
let rec IsILTypeByref inp =
    match inp with
    | ILType.Byref _ -> true
    | ILType.Modified (_, _, nestedTy) -> IsILTypeByref nestedTy
    | _ -> false

let mainMethName = CompilerGeneratedName "main"

/// Used to query custom attributes when emitting COM interop code.
type AttributeDecoder(namedArgs) =

    let nameMap =
        namedArgs
        |> List.map (fun (AttribNamedArg (s, _, _, c)) -> s, c)
        |> NameMap.ofList

    let findConst x =
        match NameMap.tryFind x nameMap with
        | Some (AttribExpr (_, Expr.Const (c, _, _))) -> Some c
        | _ -> None

    let findTyconRef x =
        match NameMap.tryFind x nameMap with
        | Some (AttribExpr (_, Expr.App (_, _, [ TType_app (tcref, _, _) ], _, _))) -> Some tcref
        | _ -> None

    member _.FindInt16 x dflt =
        match findConst x with
        | Some (Const.Int16 x) -> x
        | _ -> dflt

    member _.FindInt32 x dflt =
        match findConst x with
        | Some (Const.Int32 x) -> x
        | _ -> dflt

    member _.FindBool x dflt =
        match findConst x with
        | Some (Const.Bool x) -> x
        | _ -> dflt

    member _.FindString x dflt =
        match findConst x with
        | Some (Const.String x) -> x
        | _ -> dflt

    member _.FindTypeName x dflt =
        match findTyconRef x with
        | Some tr -> tr.DisplayName
        | _ -> dflt

//--------------------------------------------------------------------------
// Statistics
//--------------------------------------------------------------------------

let mutable reports = (fun _ -> ())

let AddReport f =
    let old = reports

    reports <-
        (fun oc ->
            old oc
            f oc)

let ReportStatistics (oc: TextWriter) = reports oc

let NewCounter nm =
    let mutable count = 0

    AddReport(fun oc ->
        if count <> 0 then
            oc.WriteLine(string count + " " + nm))

    (fun () -> count <- count + 1)

let CountClosure = NewCounter "closures"

let CountMethodDef = NewCounter "IL method definitions corresponding to values"

let CountStaticFieldDef = NewCounter "IL field definitions corresponding to values"

let CountCallFuncInstructions = NewCounter "callfunc instructions (indirect calls)"

/// Non-local information related to internals of code generation within an assembly
type IlxGenIntraAssemblyInfo =
    {
        /// A table recording the generated name of the static backing fields for each mutable top level value where
        /// we may need to take the address of that value, e.g. static mutable module-bound values which are structs. These are
        /// only accessible intra-assembly. Across assemblies, taking the address of static mutable module-bound values is not permitted.
        /// The key to the table is the method ref for the property getter for the value, which is a stable name for the Val's
        /// that come from both the signature and the implementation.
        StaticFieldInfo: Dictionary<ILMethodRef, ILFieldSpec>
    }

/// Helper to make sure we take tailcalls in some situations
type FakeUnit = | Fake

/// Indicates how the generated IL code is ultimately emitted
type IlxGenBackend =
    /// Indicates we are emitting code for ilwrite
    | IlWriteBackend

    /// Indicates we are emitting code for Reflection.Emit in F# Interactive.
    | IlReflectBackend

[<NoEquality; NoComparison>]
type IlxGenOptions =
    {
        /// Indicates the "fragment name" for the part of the assembly we are emitting, particularly for incremental
        /// emit using Reflection.Emit in F# Interactive.
        fragName: string

        /// Indicates if we are generating filter blocks
        generateFilterBlocks: bool

        /// Indicates if we are working around historical Reflection.Emit bugs
        workAroundReflectionEmitBugs: bool

        /// Indicates if we should/shouldn't emit constant arrays as static data blobs
        emitConstantArraysUsingStaticDataBlobs: bool

        /// If this is set, then the last module becomes the "main" module and its toplevel bindings are executed at startup
        mainMethodInfo: Attribs option

        /// Indicates if local optimizations are on
        localOptimizationsEnabled: bool

        /// Indicates if we are generating debug symbols
        generateDebugSymbols: bool

        /// Indicates that FeeFee debug values should be emitted as value 100001 for
        /// easier detection in debug output
        testFlagEmitFeeFeeAs100001: bool

        ilxBackend: IlxGenBackend

        fsiMultiAssemblyEmit: bool

        /// Indicates the code is being generated in FSI.EXE and is executed immediately after code generation
        /// This includes all interactively compiled code, including #load, definitions, and expressions
        isInteractive: bool

        /// Indicates the code generated is an interactive 'it' expression. We generate a setter to allow clearing of the underlying
        /// storage, even though 'it' is not logically mutable
        isInteractiveItExpr: bool

        /// Suppress ToString emit
        useReflectionFreeCodeGen: bool

        /// Whenever possible, use callvirt instead of call
        alwaysCallVirt: bool
    }

/// Compilation environment for compiling a fragment of an assembly
[<NoEquality; NoComparison>]
type cenv =
    {
        /// The TcGlobals for the compilation
        g: TcGlobals

        /// The ImportMap for reading IL
        amap: ImportMap

        /// Environment for EraseClosures functionality
        ilxPubCloEnv: EraseClosures.cenv

        /// A callback for TcVal in the typechecker.  Used to generalize values when finding witnesses.
        /// It is unfortunate this is needed but it is until we supply witnesses through the compilation.
        tcVal: ConstraintSolver.TcValF

        /// The TAST for the assembly being emitted
        viewCcu: CcuThunk

        /// Collection of all debug points available for inlined code
        namedDebugPointsForInlinedCode: Map<NamedDebugPointKey, range>

        /// The options for ILX code generation. Only available when generating in implementation code.
        optionsOpt: IlxGenOptions option

        /// Cache the generation of the "unit" type
        mutable ilUnitTy: ILType option

        /// Other information from the emit of this assembly
        intraAssemblyInfo: IlxGenIntraAssemblyInfo

        /// Cache methods with SecurityAttribute applied to them, to prevent unnecessary calls to ExistsInEntireHierarchyOfType
        casApplied: Dictionary<Stamp, bool>

        /// Used to apply forced inlining optimizations to witnesses generated late during codegen
        mutable optimizeDuringCodeGen: bool -> Expr -> Expr

        /// Guard the stack and move to a new one if necessary
        mutable stackGuard: StackGuard

    }

    member cenv.options =
        match cenv.optionsOpt with
        | None -> failwith "per-module code generation options not available for this operation"
        | Some options -> options

    override _.ToString() = "<cenv>"

let mkTypeOfExpr cenv m ilTy =
    let g = cenv.g

    mkAsmExpr (
        [ mkNormalCall (mspec_Type_GetTypeFromHandle g) ],
        [],
        [
            mkAsmExpr ([ I_ldtoken(ILToken.ILType ilTy) ], [], [], [ g.system_RuntimeTypeHandle_ty ], m)
        ],
        [ g.system_Type_ty ],
        m
    )

let mkGetNameExpr cenv (ilt: ILType) m =
    mkAsmExpr ([ I_ldstr ilt.BasicQualifiedName ], [], [], [ cenv.g.string_ty ], m)

let useCallVirt (cenv: cenv) boxity (mspec: ILMethodSpec) isBaseCall =
    cenv.options.alwaysCallVirt
    && (boxity = AsObject)
    && not mspec.CallingConv.IsStatic
    && not isBaseCall

/// Describes where items are to be placed within the generated IL namespace/typespace.
/// This should be cleaned up.
type CompileLocation =
    {
        Scope: ILScopeRef

        TopImplQualifiedName: string

        Namespace: string option

        Enclosing: string list

        QualifiedNameOfFile: string
    }

//--------------------------------------------------------------------------
// Access this and other assemblies
//--------------------------------------------------------------------------

let mkTopName ns n =
    String.concat
        "."
        (match ns with
         | Some x -> [ x; n ]
         | None -> [ n ])

let CompLocForFragment fragName (ccu: CcuThunk) =
    {
        QualifiedNameOfFile = fragName
        TopImplQualifiedName = fragName
        Scope = ccu.ILScopeRef
        Namespace = None
        Enclosing = []
    }

let CompLocForCcu (ccu: CcuThunk) = CompLocForFragment ccu.AssemblyName ccu

let CompLocForSubModuleOrNamespace cloc (submod: ModuleOrNamespace) =
    let n = submod.CompiledName

    match submod.ModuleOrNamespaceType.ModuleOrNamespaceKind with
    | FSharpModuleWithSuffix
    | ModuleOrType ->
        { cloc with
            Enclosing = cloc.Enclosing @ [ n ]
        }
    | Namespace _ ->
        { cloc with
            Namespace = Some(mkTopName cloc.Namespace n)
        }

let CompLocForFixedPath fragName qname (CompPath (sref, cpath)) =
    let ns, t =
        cpath
        |> List.takeUntil (fun (_, mkind) ->
            match mkind with
            | Namespace _ -> false
            | _ -> true)

    let ns = List.map fst ns
    let ns = textOfPath ns
    let encl = t |> List.map (fun (s, _) -> s)
    let ns = if ns = "" then None else Some ns

    {
        QualifiedNameOfFile = fragName
        TopImplQualifiedName = qname
        Scope = sref
        Namespace = ns
        Enclosing = encl
    }

let CompLocForFixedModule fragName qname (mspec: ModuleOrNamespace) =
    let cloc = CompLocForFixedPath fragName qname mspec.CompilationPath
    let cloc = CompLocForSubModuleOrNamespace cloc mspec
    cloc

let NestedTypeRefForCompLoc cloc n =
    match cloc.Enclosing with
    | [] ->
        let tyname = mkTopName cloc.Namespace n
        mkILTyRef (cloc.Scope, tyname)
    | h :: t -> mkILNestedTyRef (cloc.Scope, mkTopName cloc.Namespace h :: t, n)

let CleanUpGeneratedTypeName (nm: string) =
    if nm.IndexOfAny IllegalCharactersInTypeAndNamespaceNames = -1 then
        nm
    else
        (nm, IllegalCharactersInTypeAndNamespaceNames)
        ||> Array.fold (fun nm c -> nm.Replace(string c, "-"))

let TypeNameForInitClass cloc =
    "<StartupCode$"
    + (CleanUpGeneratedTypeName cloc.QualifiedNameOfFile)
    + ">.$"
    + cloc.TopImplQualifiedName

let TypeNameForImplicitMainMethod cloc = TypeNameForInitClass cloc + "$Main"

let TypeNameForPrivateImplementationDetails cloc =
    "<PrivateImplementationDetails$"
    + (CleanUpGeneratedTypeName cloc.QualifiedNameOfFile)
    + ">"

let CompLocForInitClass cloc =
    { cloc with
        Enclosing = [ TypeNameForInitClass cloc ]
        Namespace = None
    }

let CompLocForImplicitMainMethod cloc =
    { cloc with
        Enclosing = [ TypeNameForImplicitMainMethod cloc ]
        Namespace = None
    }

let CompLocForPrivateImplementationDetails cloc =
    { cloc with
        Enclosing = [ TypeNameForPrivateImplementationDetails cloc ]
        Namespace = None
    }

/// Compute an ILTypeRef for a CompilationLocation
let rec TypeRefForCompLoc cloc =
    match cloc.Enclosing with
    | [] -> mkILTyRef (cloc.Scope, TypeNameForPrivateImplementationDetails cloc)
    | [ h ] ->
        let tyname = mkTopName cloc.Namespace h
        mkILTyRef (cloc.Scope, tyname)
    | _ ->
        let encl, n = List.frontAndBack cloc.Enclosing
        NestedTypeRefForCompLoc { cloc with Enclosing = encl } n

/// Compute an ILType for a CompilationLocation for a non-generic type
let mkILTyForCompLoc cloc =
    mkILNonGenericBoxedTy (TypeRefForCompLoc cloc)

let ComputeMemberAccess hidden =
    if hidden then
        ILMemberAccess.Assembly
    else
        ILMemberAccess.Public

// Under --publicasinternal change types from Public to Private (internal for types)
let ComputePublicTypeAccess () = ILTypeDefAccess.Public

let ComputeTypeAccess (tref: ILTypeRef) hidden =
    match tref.Enclosing with
    | [] ->
        if hidden then
            ILTypeDefAccess.Private
        else
            ComputePublicTypeAccess()
    | _ -> ILTypeDefAccess.Nested(ComputeMemberAccess hidden)

//--------------------------------------------------------------------------
// TypeReprEnv
//--------------------------------------------------------------------------

/// Indicates how type parameters are mapped to IL type variables
[<NoEquality; NoComparison>]
type TypeReprEnv(reprs: Map<Stamp, uint16>, count: int, templateReplacement: (TyconRef * ILTypeRef * Typars * TyparInstantiation) option) =

    static let empty = TypeReprEnv(count = 0, reprs = Map.empty, templateReplacement = None)

    /// Get the template replacement information used when using struct types for state machines based on a "template" struct
    member _.TemplateReplacement = templateReplacement

    member _.WithTemplateReplacement(tcref, ilCloTyRef, cloFreeTyvars, templateTypeInst) =
        TypeReprEnv(reprs, count, Some(tcref, ilCloTyRef, cloFreeTyvars, templateTypeInst))

    member _.WithoutTemplateReplacement() = TypeReprEnv(reprs, count, None)

    /// Lookup a type parameter
    member _.Item(tp: Typar, m: range) =
        try
            reprs[tp.Stamp]
        with :? KeyNotFoundException ->
            errorR (InternalError("Undefined or unsolved type variable: " + showL (typarL tp), m))
            // Random value for post-hoc diagnostic analysis on generated tree *
            uint16 666

    /// Add an additional type parameter to the environment. If the parameter is a units-of-measure parameter
    /// then it is ignored, since it doesn't correspond to a .NET type parameter.
    member tyenv.AddOne(tp: Typar) =
        if IsNonErasedTypar tp then
            TypeReprEnv(reprs.Add(tp.Stamp, uint16 count), count + 1, templateReplacement)
        else
            tyenv

    /// Add multiple additional type parameters to the environment.
    member tyenv.Add tps =
        (tyenv, tps) ||> List.fold (fun tyenv tp -> tyenv.AddOne tp)

    /// Get the count of the non-erased type parameters in scope.
    member _.Count = count

    /// Get the empty environment, where no type parameters are in scope.
    static member Empty = empty

    /// Reset to the empty environment, where no type parameters are in scope.
    member eenv.ResetTypars() =
        TypeReprEnv(count = 0, reprs = Map.empty, templateReplacement = eenv.TemplateReplacement)

    /// Get the environment for a fixed set of type parameters
    member eenv.ForTypars tps = eenv.ResetTypars().Add tps

    /// Get the environment for within a type definition
    member eenv.ForTycon(tycon: Tycon) = eenv.ForTypars tycon.TyparsNoRange

    /// Get the environment for generating a reference to items within a type definition
    member eenv.ForTyconRef(tcref: TyconRef) = eenv.ForTycon tcref.Deref

//--------------------------------------------------------------------------
// Generate type references
//--------------------------------------------------------------------------

/// Get the ILTypeRef or other representation information for a type
let GenTyconRef (tcref: TyconRef) =
    assert (not tcref.IsTypeAbbrev)
    tcref.CompiledRepresentation

type VoidNotOK =
    | VoidNotOK
    | VoidOK

#if DEBUG
let voidCheck m g permits ty =
    if permits = VoidNotOK && isVoidTy g ty then
        error (InternalError("System.Void unexpectedly detected in IL code generation. This should not occur.", m))
#endif

/// When generating parameter and return types generate precise .NET IL pointer types.
/// These can't be generated for generic instantiations, since .NET generics doesn't
/// permit this. But for 'naked' values (locals, parameters, return values etc.) machine
/// integer values and native pointer values are compatible (though the code is unverifiable).
type PtrsOK =
    | PtrTypesOK
    | PtrTypesNotOK

let GenReadOnlyAttribute (g: TcGlobals) =
    mkILCustomAttribute (g.attrib_IsReadOnlyAttribute.TypeRef, [], [], [])

let GenReadOnlyAttributeIfNecessary (g: TcGlobals) ty =
    let add = isInByrefTy g ty && g.attrib_IsReadOnlyAttribute.TyconRef.CanDeref

    if add then
        let attr = GenReadOnlyAttribute g
        Some attr
    else
        None

/// Generate "modreq([mscorlib]System.Runtime.InteropServices.InAttribute)" on inref types.
let GenReadOnlyModReqIfNecessary (g: TcGlobals) ty ilTy =
    let add = isInByrefTy g ty && g.attrib_InAttribute.TyconRef.CanDeref

    if add then
        ILType.Modified(true, g.attrib_InAttribute.TypeRef, ilTy)
    else
        ilTy

let rec GenTypeArgAux cenv m tyenv tyarg =
    GenTypeAux cenv m tyenv VoidNotOK PtrTypesNotOK tyarg

and GenTypeArgsAux cenv m tyenv tyargs =
    List.map (GenTypeArgAux cenv m tyenv) (DropErasedTyargs tyargs)

and GenTyAppAux cenv m tyenv repr tinst =
    match repr with
    | CompiledTypeRepr.ILAsmOpen ty ->
        let ilTypeInst = GenTypeArgsAux cenv m tyenv tinst
        let ty = instILType ilTypeInst ty
        ty
    | CompiledTypeRepr.ILAsmNamed (tref, boxity, ilTypeOpt) -> GenILTyAppAux cenv m tyenv (tref, boxity, ilTypeOpt) tinst

and GenILTyAppAux cenv m tyenv (tref, boxity, ilTypeOpt) tinst =
    match ilTypeOpt with
    | None ->
        let ilTypeInst = GenTypeArgsAux cenv m tyenv tinst
        mkILTy boxity (mkILTySpec (tref, ilTypeInst))
    | Some ilType -> ilType // monomorphic types include a cached ilType to avoid reallocation of an ILType node

and GenNamedTyAppAux (cenv: cenv) m (tyenv: TypeReprEnv) ptrsOK tcref tinst =
    let g = cenv.g

    match tyenv.TemplateReplacement with
    | Some (tcref2, ilCloTyRef, cloFreeTyvars, _) when tyconRefEq g tcref tcref2 ->
        let cloInst = List.map mkTyparTy cloFreeTyvars
        let ilTypeInst = GenTypeArgsAux cenv m tyenv cloInst
        mkILValueTy ilCloTyRef ilTypeInst
    | _ ->
        let tinst = DropErasedTyargs tinst
        // See above note on ptrsOK
        if
            ptrsOK = PtrTypesOK
            && tyconRefEq g tcref g.nativeptr_tcr
            && (freeInTypes CollectTypars tinst).FreeTypars.IsEmpty
        then
            GenNamedTyAppAux cenv m tyenv ptrsOK g.ilsigptr_tcr tinst
        else
#if !NO_TYPEPROVIDERS
            match tcref.TypeReprInfo with
            // Generate the base type, because that is always the representation of the erased type, unless the assembly is being injected
            | TProvidedTypeRepr info when info.IsErased -> GenTypeAux cenv m tyenv VoidNotOK ptrsOK (info.BaseTypeForErased(m, g.obj_ty))
            | _ ->
#endif
            GenTyAppAux cenv m tyenv (GenTyconRef tcref) tinst

and GenTypeAux cenv m (tyenv: TypeReprEnv) voidOK ptrsOK ty =
    let g = cenv.g
#if DEBUG
    voidCheck m g voidOK ty
#else
    ignore voidOK
#endif
    match stripTyEqnsAndMeasureEqns g ty with
    | TType_app (tcref, tinst, _) -> GenNamedTyAppAux cenv m tyenv ptrsOK tcref tinst

    | TType_tuple (tupInfo, args) -> GenTypeAux cenv m tyenv VoidNotOK ptrsOK (mkCompiledTupleTy g (evalTupInfoIsStruct tupInfo) args)

    | TType_fun (dty, returnTy, _) ->
        EraseClosures.mkILFuncTy cenv.ilxPubCloEnv (GenTypeArgAux cenv m tyenv dty) (GenTypeArgAux cenv m tyenv returnTy)

    | TType_anon (anonInfo, tinst) ->
        let tref = anonInfo.ILTypeRef

        let boxity =
            if evalAnonInfoIsStruct anonInfo then
                ILBoxity.AsValue
            else
                ILBoxity.AsObject

        GenILTyAppAux cenv m tyenv (tref, boxity, None) tinst

    | TType_ucase (ucref, args) ->
        let cuspec, idx = GenUnionCaseSpec cenv m tyenv ucref args
        EraseUnions.GetILTypeForAlternative cuspec idx

    | TType_forall (tps, tau) ->
        let tps = DropErasedTypars tps

        if tps.IsEmpty then
            GenTypeAux cenv m tyenv VoidNotOK ptrsOK tau
        else
            EraseClosures.mkILTyFuncTy cenv.ilxPubCloEnv

    | TType_var (tp, _) -> mkILTyvarTy tyenv[tp, m]

    | TType_measure _ -> g.ilg.typ_Int32

//--------------------------------------------------------------------------
// Generate ILX references to closures, classunions etc. given a tyenv
//--------------------------------------------------------------------------

and GenUnionCaseRef (cenv: cenv) m tyenv i (fspecs: RecdField[]) =
    let g = cenv.g

    fspecs
    |> Array.mapi (fun j fspec ->
        let ilFieldDef =
            mkILInstanceField (fspec.LogicalName, GenType cenv m tyenv fspec.FormalType, None, ILMemberAccess.Public)
        // These properties on the "field" of an alternative end up going on a property generated by cu_erase.fs
        IlxUnionCaseField(
            ilFieldDef.With(
                customAttrs =
                    mkILCustomAttrs
                        [
                            (mkCompilationMappingAttrWithVariantNumAndSeqNum g (int SourceConstructFlags.Field) i j)
                        ]
            )
        ))

and GenUnionRef (cenv: cenv) m (tcref: TyconRef) =
    let g = cenv.g
    let tycon = tcref.Deref
    assert (not tycon.IsTypeAbbrev)

    match tycon.UnionTypeInfo with
    | ValueNone -> failwith "GenUnionRef m"
    | ValueSome funion ->
        cached funion.CompiledRepresentation (fun () ->
            let tyenvinner = TypeReprEnv.Empty.ForTycon tycon

            match tcref.CompiledRepresentation with
            | CompiledTypeRepr.ILAsmOpen _ -> failwith "GenUnionRef m: unexpected ASM tyrep"
            | CompiledTypeRepr.ILAsmNamed (tref, _, _) ->
                let alternatives =
                    tycon.UnionCasesArray
                    |> Array.mapi (fun i cspec ->
                        {
                            altName = cspec.CompiledName
                            altCustomAttrs = emptyILCustomAttrs
                            altFields = GenUnionCaseRef cenv m tyenvinner i cspec.RecdFieldsArray
                        })

                let nullPermitted = IsUnionTypeWithNullAsTrueValue g tycon
                let hasHelpers = ComputeUnionHasHelpers g tcref

                let boxity =
                    (if tcref.IsStructOrEnumTycon then
                         ILBoxity.AsValue
                     else
                         ILBoxity.AsObject)

                IlxUnionRef(boxity, tref, alternatives, nullPermitted, hasHelpers))

and ComputeUnionHasHelpers g (tcref: TyconRef) =
    if tyconRefEq g tcref g.unit_tcr_canon then
        NoHelpers
    elif tyconRefEq g tcref g.list_tcr_canon then
        SpecialFSharpListHelpers
    elif tyconRefEq g tcref g.option_tcr_canon then
        SpecialFSharpOptionHelpers
    else
        match TryFindFSharpAttribute g g.attrib_DefaultAugmentationAttribute tcref.Attribs with
        | Some (Attrib (_, _, [ AttribBoolArg b ], _, _, _, _)) -> if b then AllHelpers else NoHelpers
        | Some (Attrib (_, _, _, _, _, _, m)) ->
            errorR (Error(FSComp.SR.ilDefaultAugmentationAttributeCouldNotBeDecoded (), m))
            AllHelpers
        | _ -> AllHelpers (* not hiddenRepr *)

and GenUnionSpec (cenv: cenv) m tyenv tcref tyargs =
    let curef = GenUnionRef cenv m tcref
    let tinst = GenTypeArgs cenv m tyenv tyargs
    IlxUnionSpec(curef, tinst)

and GenUnionCaseSpec cenv m tyenv (ucref: UnionCaseRef) tyargs =
    let cuspec = GenUnionSpec cenv m tyenv ucref.TyconRef tyargs
    cuspec, ucref.Index

and GenType cenv m tyenv ty =
    GenTypeAux cenv m tyenv VoidNotOK PtrTypesNotOK ty

and GenTypes cenv m tyenv tys = List.map (GenType cenv m tyenv) tys

and GenTypePermitVoid cenv m tyenv ty =
    (GenTypeAux cenv m tyenv VoidOK PtrTypesNotOK ty)

and GenTypesPermitVoid cenv m tyenv tys =
    List.map (GenTypePermitVoid cenv m tyenv) tys

and GenTyApp cenv m tyenv repr tyargs = GenTyAppAux cenv m tyenv repr tyargs

and GenNamedTyApp cenv m tyenv tcref tinst =
    GenNamedTyAppAux cenv m tyenv PtrTypesNotOK tcref tinst

/// IL void types are only generated for return types
and GenReturnType cenv m tyenv returnTyOpt =
    match returnTyOpt with
    | None -> ILType.Void
    | Some returnTy ->
        let ilTy =
            GenTypeAux cenv m tyenv VoidNotOK (*1*) PtrTypesOK returnTy (*1: generate void from unit, but not accept void *)

        GenReadOnlyModReqIfNecessary cenv.g returnTy ilTy

and GenParamType cenv m tyenv isSlotSig ty =
    let ilTy = GenTypeAux cenv m tyenv VoidNotOK PtrTypesOK ty

    if isSlotSig then
        GenReadOnlyModReqIfNecessary cenv.g ty ilTy
    else
        ilTy

and GenParamTypes cenv m tyenv isSlotSig tys =
    tys |> List.map (GenParamType cenv m tyenv isSlotSig)

and GenTypeArgs cenv m tyenv tyargs = GenTypeArgsAux cenv m tyenv tyargs

and GenTypePermitVoidAux cenv m tyenv ty =
    GenTypeAux cenv m tyenv VoidOK PtrTypesNotOK ty

// Static fields generally go in a private InitializationCodeAndBackingFields section. This is to ensure all static
// fields are initialized only in their class constructors (we generate one primary
// cctor for each file to ensure initialization coherence across the file, regardless
// of how many modules are in the file). This means F# passes an extra check applied by SQL Server when it
// verifies stored procedures: SQL Server checks that all 'initionly' static fields are only initialized from
// their own class constructor.
//
// However, mutable static fields must be accessible across compilation units. This means we place them in their "natural" location
// which may be in a nested module etc. This means mutable static fields can't be used in code to be loaded by SQL Server.
//
// Computes the location where the static field for a value lives.
//     - Literals go in their type/module.
//     - For interactive code, we always place fields in their type/module with an accurate name
let GenFieldSpecForStaticField (isInteractive, g, ilContainerTy, vspec: Val, nm, m, cloc, ilTy) =
    if isInteractive || HasFSharpAttribute g g.attrib_LiteralAttribute vspec.Attribs then
        let fieldName = vspec.CompiledName g.CompilerGlobalState

        let fieldName =
            if isInteractive then
                CompilerGeneratedName fieldName
            else
                fieldName

        mkILFieldSpecInTy (ilContainerTy, fieldName, ilTy)
    else
        let fieldName =
            // Ensure that we have an g.CompilerGlobalState
            assert (g.CompilerGlobalState |> Option.isSome)
            g.CompilerGlobalState.Value.IlxGenNiceNameGenerator.FreshCompilerGeneratedName(nm, m)

        let ilFieldContainerTy = mkILTyForCompLoc (CompLocForInitClass cloc)
        mkILFieldSpecInTy (ilFieldContainerTy, fieldName, ilTy)

let GenRecdFieldRef m cenv (tyenv: TypeReprEnv) (rfref: RecdFieldRef) tyargs =
    match tyenv.TemplateReplacement with
    | Some (tcref2, ilCloTyRef, cloFreeTyvars, templateTypeInst) when tyconRefEq cenv.g rfref.TyconRef tcref2 ->
        // Fixup references to the fields of a struct machine template
        //     templateStructTy = ResumableStateMachine<TaskStateMachineData<SomeType['FreeTyVars]>
        //     templateTyconRef = ResumableStateMachine<'Data>
        //     templateTypeArgs = <TaskStateMachineData<SomeType['FreeTyVars]>
        //     templateTypeInst = 'Data -> TaskStateMachineData<SomeType['FreeTyVars]>
        //     cloFreeTyvars = <'FreeTyVars>
        //     ilCloTy = clo<'FreeTyVars> w.r.t envinner
        //     rfref = ResumableStateMachine<'Data>::Result
        //     rfref.RecdField.FormalType = 'Data
        let ilCloTy =
            let cloInst = List.map mkTyparTy cloFreeTyvars
            let ilTypeInst = GenTypeArgsAux cenv m tyenv cloInst
            mkILValueTy ilCloTyRef ilTypeInst

        let tyenvinner = TypeReprEnv.Empty.ForTypars cloFreeTyvars

        mkILFieldSpecInTy (
            ilCloTy,
            ComputeFieldName rfref.Tycon rfref.RecdField,
            GenType cenv m tyenvinner (instType templateTypeInst rfref.RecdField.FormalType)
        )
    | _ ->
        let tyenvinner = TypeReprEnv.Empty.ForTycon rfref.Tycon
        let ilTy = GenTyApp cenv m tyenv rfref.TyconRef.CompiledRepresentation tyargs
        mkILFieldSpecInTy (ilTy, ComputeFieldName rfref.Tycon rfref.RecdField, GenType cenv m tyenvinner rfref.RecdField.FormalType)

let GenExnType amap m tyenv (ecref: TyconRef) =
    GenTyApp amap m tyenv ecref.CompiledRepresentation []

type ArityInfo = int list

//--------------------------------------------------------------------------
// Closure summaries
//
// Function, Object, Delegate and State Machine Closures
// =====================================================
//
// For a normal expression closure, we generate:
//
//    class Implementation<cloFreeTyvars> : FSharpFunc<...> {
//        override Invoke(..) { expr }
//    }
//
// Local Type Functions
// ====================
//
// The input expression is:
//   let input-val : FORALL<directTypars>. body-type = LAM <directTypars>. body-expr : body-type
//   ...
//
// This is called at some point:
//
//   input-val<directTyargs>
//
// Note 'input-val' is never used without applying it to some type arguments.
//
// Basic examples - first define some functions that extract information from generic parameters, and which are constrained:
//
//    type TypeInfo<'T> = TypeInfo of System.Type
//    type TypeName = TypeName of string
//
//    let typeinfo<'T when 'T :> System.IComparable) = TypeInfo (typeof<'T>)
//    let typename<'T when 'T :> System.IComparable) = TypeName (typeof<'T>.Name)
//
// Then here are examples:
//
//    LAM <'T>{addWitness}.  (typeinfo<'T>, typeinfo<'T[]>, (incr{ : 'T -> 'T)) :  TypeInfo<'T> * TypeInfo<'T[]> * ('T -> 'T)
//    directTypars = 'T
//    cloFreeTyvars = empty
//
// or
//    LAM <'T>.  (typeinfo<'T>, typeinfo<'U>) :  TypeInfo<'T> * TypeInfo<'U>
//    directTypars = 'T
//    cloFreeTyvars = 'U
//
// or
//    LAM <'T>.  (typeinfo<'T>, typeinfo<'U>, typename<'V>) :  TypeInfo<'T> * TypeInfo<'U> * TypeName
//    directTypars = 'T
//    cloFreeTyvars = 'U,'V
//
// or, for witnesses:
//
//    let inline incr{addWitnessForT} (x: 'T) = x + GenericZero<'T> // has witness argment for '+'
//
//    LAM <'T when 'T :... op_Addition ...>{addWitnessForT}.  (incr<'T>{addWitnessForT}, incr<'U>{addWitnessForU}, incr<'V>{addWitnessForV}) :  ('T -> 'T) * ('U -> 'U) * ('V -> 'V)
//    directTypars = 'T
//    cloFreeTyvars = 'U,'V
//    cloFreeTyvarsWitnesses = witnesses implied by cloFreeTyvars = {addWitnessForU, addWitnessForV}
//    directTyparsWitnesses = witnesses implied by directTypars = {addWitnessForT}
//
// Define the free variable sets:
//
//    cloFreeTyvars = free-tyvars-of(input-expr)
//
// where IsNamedLocalTypeFuncVal is true.
//
// The directTypars may have constraints that require some witnesses.  Making those explicit with "{ ... }" syntax for witnesses:
//    input-expr = {LAM <directTypars>{directWitnessInfoArgs}. body-expr : body-type }
//
//    let x : FORALL<'T ... constrained ...> ... = clo<directTyargs>{directWitnessInfos}
//
// Given this, we generate this shape of code:
//
//    type Implementation<cloFreeTyvars>(cloFreeTyvarsWitnesses) =
//        member DirectInvoke<directTypars>(directTyparsWitnesses) : body-type =
//             body-expr
//
//    local x : obj = new Implementation<cloFreeTyvars>(cloFreeTyvarsWitnesses)
//    ....
//    ldloc x
//    unbox Implementation<cloFreeTyvars>
//    call Implementation<cloFreeTyvars>::DirectInvoke<directTypars>(directTyparsWitnesses)
//
// First-class Type Functions
// ==========================
//
// If IsNamedLocalTypeFuncVal is false, we have a "non-local" or "first-class" type function closure
// that implements FSharpTypeFunc, and we generate:
//
//    class Implementation<cloFreeTyvars> : FSharpTypeFunc {
//        override Specialize<directTypars> : overall-type { expr }
//    }
//

[<NoEquality; NoComparison>]
type IlxClosureInfo =
    {
        /// The whole expression for the closure
        cloExpr: Expr

        /// The name of the generated closure class
        cloName: string

        /// The counts of curried arguments for the closure
        cloArityInfo: ArityInfo

        /// The formal return type
        ilCloFormalReturnTy: ILType

        /// An immutable array of free variable descriptions for the closure
        ilCloAllFreeVars: IlxClosureFreeVar[]

        /// The ILX specification for the closure
        cloSpec: IlxClosureSpec

        /// The generic parameters for the closure, i.e. the type variables it captures
        cloILGenericParams: ILGenericParameterDefs

        /// The captured variables for the closure
        cloFreeVars: Val list

        cloFreeTyvars: Typars

        cloWitnessInfos: TraitWitnessInfos

        /// ILX view of the lambdas for the closures
        ilCloLambdas: IlxClosureLambdas

    }

//--------------------------------------------------------------------------
// ValStorage
//--------------------------------------------------------------------------

/// Describes the storage for a value
[<NoEquality; NoComparison>]
type ValStorage =
    /// Indicates the value is always null
    | Null

    /// Indicates the value is stored in a static field.
    | StaticPropertyWithField of
        ilFieldSpec: ILFieldSpec *
        valRef: ValRef *
        hasLiteralAttr: bool *
        ilTyForProperty: ILType *
        name: string *
        ilTy: ILType *
        ilGetterMethRef: ILMethodRef *
        ilSetterMethRef: ILMethodRef *
        optShadowLocal: OptionalShadowLocal

    /// Indicates the value is represented as a property that recomputes it each time it is referenced. Used for simple constants that do not cause initialization triggers
    | StaticProperty of ilGetterMethSpec: ILMethodSpec * optShadowLocal: OptionalShadowLocal

    /// Indicates the value is represented as an IL method (in a "main" class for a F#
    /// compilation unit, or as a member) according to its inferred or specified arity.
    | Method of
        valReprInfo: ValReprInfo *
        valRef: ValRef *
        ilMethSpec: ILMethodSpec *
        ilMethSpecWithWitnesses: ILMethodSpec *
        m: range *
        classTypars: Typars *
        methTypars: Typars *
        curriedArgInfos: CurriedArgInfos *
        paramInfos: ArgReprInfo list *
        witnessInfos: TraitWitnessInfos *
        methodArgTys: TType list *
        retInfo: ArgReprInfo

    /// Indicates the value is stored at the given position in the closure environment accessed via "ldarg 0"
    | Env of ilCloTyInner: ILType * ilField: ILFieldSpec * localCloInfo: (FreeTyvars * NamedLocalIlxClosureInfo ref) option

    /// Indicates that the value is an argument of a method being generated
    | Arg of index: int

    /// Indicates that the value is stored in local of the method being generated. NamedLocalIlxClosureInfo is normally empty.
    /// It is non-empty for 'local type functions', see comments on definition of NamedLocalIlxClosureInfo.
    | Local of index: int * realloc: bool * localCloInfo: (FreeTyvars * NamedLocalIlxClosureInfo ref) option

/// Indicates if there is a shadow local storage for a local, to make sure it gets a good name in debugging
and OptionalShadowLocal =
    | NoShadowLocal
    | ShadowLocal of startMark: Mark * storage: ValStorage

/// The representation of a NamedLocalClosure is based on a cloinfo. However we can't generate a cloinfo until we've
/// decided the representations of other items in the recursive set. Hence we use two phases to decide representations in
/// a recursive set. Yuck.
and NamedLocalIlxClosureInfo =
    | NamedLocalIlxClosureInfoGenerator of (IlxGenEnv -> IlxClosureInfo)
    | NamedLocalIlxClosureInfoGenerated of IlxClosureInfo

    override _.ToString() = "<NamedLocalIlxClosureInfo>"

/// Indicates the overall representation decisions for all the elements of a namespace of module
and ModuleStorage =
    {
        Vals: Lazy<NameMap<ValStorage>>

        SubModules: Lazy<NameMap<ModuleStorage>>
    }

    override _.ToString() = "<ModuleStorage>"

/// Indicate whether a call to the value can be implemented as
/// a branch. At the moment these are only used for generating branch calls back to
/// the entry label of the method currently being generated when a direct tailcall is
/// made in the method itself.
and BranchCallItem =

    | BranchCallClosure of ArityInfo

    | BranchCallMethod of
        // Argument counts for compiled form of F# method or value
        ArityInfo *
        // Arg infos for compiled form of F# method or value
        (TType * ArgReprInfo) list list *
        // Typars for F# method or value
        Typars *
        // num obj args in IL
        int *
        // num witness args in IL
        int *
        // num actual args in IL
        int

    override _.ToString() = "<BranchCallItem>"

/// Represents a place we can branch to
and Mark =
    | Mark of ILCodeLabel

    member x.CodeLabel = (let (Mark lab) = x in lab)

/// Represents "what to do next after we generate this expression"
and sequel =
    | EndFilter

    /// Exit a 'handler' block. The integer says which local to save result in
    | LeaveHandler of isFinally: bool * whereToSaveOpt: (int * ILType) option * afterHandler: Mark * hasResult: bool

    /// Branch to the given mark
    | Br of Mark

    /// Execute the given comparison-then-branch instructions on the result of the expression
    /// If the branch isn't taken then drop through.
    | CmpThenBrOrContinue of Pops * ILInstr list

    /// Continue and leave the value on the IL computation stack
    | Continue

    /// The value then do something else
    | DiscardThen of sequel

    /// Return from the method
    | Return

    /// End a scope of local variables. Used at end of 'let' and 'let rec' blocks to get tail recursive setting
    /// of end-of-scope marks
    | EndLocalScope of sequel * Mark

    /// Return from a method whose return type is void
    | ReturnVoid

and Pushes = ILType list
and Pops = int

/// The overall environment at a particular point in the declaration/expression tree.
and IlxGenEnv =
    {
        /// The representation decisions for the (non-erased) type parameters that are in scope
        tyenv: TypeReprEnv

        /// An ILType for some random type in this assembly
        someTypeInThisAssembly: ILType

        /// Indicates if we are generating code for the last file in a .EXE
        isFinalFile: bool

        /// Indicates the default "place" for stuff we're currently generating
        cloc: CompileLocation

        /// The sequel to use for an "early exit" in a state machine, e.g. a return from the middle of an
        /// async block
        exitSequel: sequel

        /// Hiding information down the signature chain, used to compute what's public to the assembly
        sigToImplRemapInfo: (Remap * SignatureHidingInfo) list

        /// The open/open-type declarations in scope
        imports: ILDebugImports option

        /// All values in scope
        valsInScope: ValMap<Lazy<ValStorage>>

        /// All witnesses in scope and their mapping to storage for the witness value.
        witnessesInScope: TraitWitnessInfoHashMap<ValStorage>

        /// Suppress witnesses when not generating witness-passing code
        suppressWitnesses: bool

        /// For optimizing direct tail recursion to a loop - mark says where to branch to.  Length is 0 or 1.
        /// REVIEW: generalize to arbitrary nested local loops??
        innerVals: (ValRef * (BranchCallItem * Mark)) list

        /// Full list of enclosing bound values. First non-compiler-generated element is used to help give nice names for closures and other expressions.
        letBoundVars: ValRef list

        /// The set of IL local variable indexes currently in use by lexically scoped variables, to allow reuse on different branches.
        /// Really an integer set.
        liveLocals: IntMap<unit>

        /// Are we under the scope of a try, catch or finally? If so we can't tailcall. SEH = structured exception handling
        withinSEH: bool

        /// Are we inside of a recursive let binding, while loop, or a for loop?
        isInLoop: bool

        /// Indicates that the .locals init flag should be set on a method and all its nested methods and lambdas
        initLocals: bool
    }

    override _.ToString() = "<IlxGenEnv>"

let discard = DiscardThen Continue
let discardAndReturnVoid = DiscardThen ReturnVoid

let SetIsInLoop isInLoop eenv =
    if eenv.isInLoop = isInLoop then
        eenv
    else
        { eenv with isInLoop = isInLoop }

let EnvForTypars tps eenv =
    { eenv with
        tyenv = eenv.tyenv.ForTypars tps
    }

let EnvForTycon tps eenv =
    { eenv with
        tyenv = eenv.tyenv.ForTycon tps
    }

let AddTyparsToEnv typars (eenv: IlxGenEnv) =
    { eenv with
        tyenv = eenv.tyenv.Add typars
    }

let AddSignatureRemapInfo _msg (rpi, mhi) eenv =
    { eenv with
        sigToImplRemapInfo = (mkRepackageRemapping rpi, mhi) :: eenv.sigToImplRemapInfo
    }

let OutputStorage (pps: TextWriter) s =
    match s with
    | StaticPropertyWithField _ -> pps.Write "(top)"
    | StaticProperty _ -> pps.Write "(top)"
    | Method _ -> pps.Write "(top)"
    | Local _ -> pps.Write "(local)"
    | Arg _ -> pps.Write "(arg)"
    | Env _ -> pps.Write "(env)"
    | Null -> pps.Write "(null)"

//--------------------------------------------------------------------------
// Augment eenv with values
//--------------------------------------------------------------------------

let AddStorageForVal (g: TcGlobals) (v, s) eenv =
    let eenv =
        { eenv with
            valsInScope = eenv.valsInScope.Add v s
        }
    // If we're compiling fslib then also bind the value as a non-local path to
    // allow us to resolve the compiler-non-local-references that arise from env.fs
    //
    // Do this by generating a fake "looking from the outside in" non-local value reference for
    // v, dereferencing it to find the corresponding signature Val, and adding an entry for the signature val.
    //
    // A similar code path exists in ilxgen.fs for the tables of "optimization data" for values
    if g.compilingFSharpCore then
        // Passing an empty remap is sufficient for FSharp.Core.dll because it turns out the remapped type signature can
        // still be resolved.
        match tryRescopeVal g.fslibCcu Remap.Empty v with
        | ValueNone -> eenv
        | ValueSome vref ->
            match vref.TryDeref with
            | ValueNone ->
                //let msg = sprintf "could not dereference external value reference to something in FSharp.Core.dll during code generation, v.MangledName = '%s', v.Range = %s" v.MangledName (stringOfRange v.Range)
                //System.Diagnostics.Debug.Assert(false, msg)
                eenv
            | ValueSome gv ->
                { eenv with
                    valsInScope = eenv.valsInScope.Add gv s
                }
    else
        eenv

let AddStorageForLocalVals g vals eenv =
    List.foldBack (fun (v, s) acc -> AddStorageForVal g (v, notlazy s) acc) vals eenv

let RemoveTemplateReplacement eenv =
    { eenv with
        tyenv = eenv.tyenv.WithoutTemplateReplacement()
    }

let AddTemplateReplacement eenv (tcref, ftyvs, ilTy, inst) =
    { eenv with
        tyenv = eenv.tyenv.WithTemplateReplacement(tcref, ftyvs, ilTy, inst)
    }

let AddStorageForLocalWitness eenv (w, s) =
    { eenv with
        witnessesInScope = eenv.witnessesInScope.SetItem(w, s)
    }

let AddStorageForLocalWitnesses witnesses eenv =
    (eenv, witnesses) ||> List.fold AddStorageForLocalWitness

//--------------------------------------------------------------------------
// Lookup eenv
//--------------------------------------------------------------------------

let StorageForVal m v eenv =
    let v =
        try
            eenv.valsInScope[v]
        with :? KeyNotFoundException ->
            assert false
            errorR (Error(FSComp.SR.ilUndefinedValue (showL (valAtBindL v)), m))
            notlazy (Arg 668 (* random value for post-hoc diagnostic analysis on generated tree *) )

    v.Force()

let StorageForValRef m (v: ValRef) eenv = StorageForVal m v.Deref eenv

let ComputeGenerateWitnesses (g: TcGlobals) eenv =
    g.generateWitnesses
    && not eenv.witnessesInScope.IsEmpty
    && not eenv.suppressWitnesses

let TryStorageForWitness (_g: TcGlobals) eenv (w: TraitWitnessInfo) =
    match eenv.witnessesInScope.TryGetValue w with
    | true, storage -> Some storage
    | _ -> None

let IsValRefIsDllImport g (vref: ValRef) =
    vref.Attribs |> HasFSharpAttributeOpt g g.attrib_DllImportAttribute

/// Determine how a top level value is represented, when it is being represented
/// as a method.
let GetMethodSpecForMemberVal cenv (memberInfo: ValMemberInfo) (vref: ValRef) =
    let g = cenv.g
    let m = vref.Range
    let numEnclosingTypars = CountEnclosingTyparsOfActualParentOfVal vref.Deref

    let tps, witnessInfos, curriedArgInfos, returnTy, retInfo =
        assert vref.ValReprInfo.IsSome
        GetValReprTypeInCompiledForm g vref.ValReprInfo.Value numEnclosingTypars vref.Type m

    let tyenvUnderTypars = TypeReprEnv.Empty.ForTypars tps
    let flatArgInfos = List.concat curriedArgInfos
    let isCtor = (memberInfo.MemberFlags.MemberKind = SynMemberKind.Constructor)
    let cctor = (memberInfo.MemberFlags.MemberKind = SynMemberKind.ClassConstructor)
    let parentTcref = vref.DeclaringEntity
    let parentTypars = parentTcref.TyparsNoRange
    let numParentTypars = parentTypars.Length

    if tps.Length < numParentTypars then
        error (InternalError("CodeGen check: type checking did not ensure that this method is sufficiently generic", m))

    let ctps, mtps = List.splitAt numParentTypars tps
    let isCompiledAsInstance = ValRefIsCompiledAsInstanceMember g vref

    let ilActualRetTy =
        let ilRetTy = GenReturnType cenv m tyenvUnderTypars returnTy
        if isCtor || cctor then ILType.Void else ilRetTy

    let ilTy =
        GenType cenv m tyenvUnderTypars (mkAppTy parentTcref (List.map mkTyparTy ctps))

    let nm = vref.CompiledName g.CompilerGlobalState

    if isCompiledAsInstance || isCtor then
        // Find the 'this' argument type if any
        let thisTy, flatArgInfos =
            if isCtor then
                (GetFSharpViewOfReturnType g returnTy), flatArgInfos
            else
                match flatArgInfos with
                | [] -> error (InternalError("This instance method '" + vref.LogicalName + "' has no arguments", m))
                | (h, _) :: t -> h, t

        let thisTy = if isByrefTy g thisTy then destByrefTy g thisTy else thisTy
        let thisArgTys = argsOfAppTy g thisTy

        if numParentTypars <> thisArgTys.Length then
            let msg =
                sprintf
                    "CodeGen check: type checking did not quantify the correct number of type variables for this method, #parentTypars = %d, #mtps = %d, #thisArgTys = %d"
                    numParentTypars
                    mtps.Length
                    thisArgTys.Length

            warning (InternalError(msg, m))
        else
            List.iter2
                (fun gtp ty2 ->
                    if not (typeEquiv g (mkTyparTy gtp) ty2) then
                        warning (
                            InternalError(
                                "CodeGen check: type checking did not quantify the correct type variables for this method: generalization list contained "
                                + gtp.Name
                                + "#"
                                + string gtp.Stamp
                                + " and list from 'this' pointer contained "
                                + (showL (typeL ty2)),
                                m
                            )
                        ))
                ctps
                thisArgTys

        let methodArgTys, paramInfos = List.unzip flatArgInfos

        let isSlotSig =
            memberInfo.MemberFlags.IsDispatchSlot
            || memberInfo.MemberFlags.IsOverrideOrExplicitImpl

        let ilMethodArgTys = GenParamTypes cenv m tyenvUnderTypars isSlotSig methodArgTys
        let ilMethodInst = GenTypeArgs cenv m tyenvUnderTypars (List.map mkTyparTy mtps)

        let mspec =
            mkILInstanceMethSpecInTy (ilTy, nm, ilMethodArgTys, ilActualRetTy, ilMethodInst)

        let mspecW =
            if not g.generateWitnesses || witnessInfos.IsEmpty then
                mspec
            else
                let ilWitnessArgTys =
                    GenTypes cenv m tyenvUnderTypars (GenWitnessTys g witnessInfos)

                let nmW = ExtraWitnessMethodName nm
                mkILInstanceMethSpecInTy (ilTy, nmW, ilWitnessArgTys @ ilMethodArgTys, ilActualRetTy, ilMethodInst)

        mspec, mspecW, ctps, mtps, curriedArgInfos, paramInfos, retInfo, witnessInfos, methodArgTys, returnTy
    else
        let methodArgTys, paramInfos = List.unzip flatArgInfos
        let ilMethodArgTys = GenParamTypes cenv m tyenvUnderTypars false methodArgTys
        let ilMethodInst = GenTypeArgs cenv m tyenvUnderTypars (List.map mkTyparTy mtps)

        let mspec =
            mkILStaticMethSpecInTy (ilTy, nm, ilMethodArgTys, ilActualRetTy, ilMethodInst)

        let mspecW =
            if not g.generateWitnesses || witnessInfos.IsEmpty then
                mspec
            else
                let ilWitnessArgTys =
                    GenTypes cenv m tyenvUnderTypars (GenWitnessTys g witnessInfos)

                let nmW = ExtraWitnessMethodName nm
                mkILStaticMethSpecInTy (ilTy, nmW, ilWitnessArgTys @ ilMethodArgTys, ilActualRetTy, ilMethodInst)

        mspec, mspecW, ctps, mtps, curriedArgInfos, paramInfos, retInfo, witnessInfos, methodArgTys, returnTy

/// Determine how a top-level value is represented, when representing as a field, by computing an ILFieldSpec
let ComputeFieldSpecForVal
    (
        optIntraAssemblyInfo: IlxGenIntraAssemblyInfo option,
        isInteractive,
        g,
        ilTyForProperty,
        vspec: Val,
        nm,
        m,
        cloc,
        ilTy,
        ilGetterMethRef
    ) =
    assert vspec.IsCompiledAsTopLevel

    let generate () =
        GenFieldSpecForStaticField(isInteractive, g, ilTyForProperty, vspec, nm, m, cloc, ilTy)

    match optIntraAssemblyInfo with
    | None -> generate ()
    | Some intraAssemblyInfo ->
        match intraAssemblyInfo.StaticFieldInfo.TryGetValue ilGetterMethRef with
        | true, res -> res
        | _ ->
            let res = generate ()
            intraAssemblyInfo.StaticFieldInfo[ ilGetterMethRef ] <- res
            res

/// Compute the representation information for an F#-declared value (not a member nor a function).
/// Mutable and literal static fields must have stable names and live in the "public" location
let ComputeStorageForFSharpValue amap (g: TcGlobals) cloc optIntraAssemblyInfo optShadowLocal isInteractive returnTy (vref: ValRef) m =
    let nm = vref.CompiledName g.CompilerGlobalState
    let vspec = vref.Deref

    let ilTy =
        GenType amap m TypeReprEnv.Empty returnTy (* TypeReprEnv.Empty ok: not a field in a generic class *)

    let ilTyForProperty = mkILTyForCompLoc cloc
    let attribs = vspec.Attribs
    let hasLiteralAttr = HasFSharpAttribute g g.attrib_LiteralAttribute attribs
    let ilTypeRefForProperty = ilTyForProperty.TypeRef

    let ilGetterMethRef =
        mkILMethRef (ilTypeRefForProperty, ILCallingConv.Static, "get_" + nm, 0, [], ilTy)

    let ilSetterMethRef =
        mkILMethRef (ilTypeRefForProperty, ILCallingConv.Static, "set_" + nm, 0, [ ilTy ], ILType.Void)

    let ilFieldSpec =
        ComputeFieldSpecForVal(optIntraAssemblyInfo, isInteractive, g, ilTyForProperty, vspec, nm, m, cloc, ilTy, ilGetterMethRef)

    StaticPropertyWithField(ilFieldSpec, vref, hasLiteralAttr, ilTyForProperty, nm, ilTy, ilGetterMethRef, ilSetterMethRef, optShadowLocal)

/// Compute the representation information for an F#-declared member
let ComputeStorageForFSharpMember cenv valReprInfo memberInfo (vref: ValRef) m =
    let mspec, mspecW, ctps, mtps, curriedArgInfos, paramInfos, retInfo, witnessInfos, methodArgTys, _ =
        GetMethodSpecForMemberVal cenv memberInfo vref

    Method(valReprInfo, vref, mspec, mspecW, m, ctps, mtps, curriedArgInfos, paramInfos, witnessInfos, methodArgTys, retInfo)

/// Compute the representation information for an F#-declared function in a module or an F#-declared extension member.
/// Note, there is considerable overlap with ComputeStorageForFSharpMember/GetMethodSpecForMemberVal and these could be
/// rationalized.
let ComputeStorageForFSharpFunctionOrFSharpExtensionMember (cenv: cenv) cloc valReprInfo (vref: ValRef) m =
    let g = cenv.g
    let nm = vref.CompiledName g.CompilerGlobalState
    let numEnclosingTypars = CountEnclosingTyparsOfActualParentOfVal vref.Deref

    let tps, witnessInfos, curriedArgInfos, returnTy, retInfo =
        GetValReprTypeInCompiledForm g valReprInfo numEnclosingTypars vref.Type m

    let tyenvUnderTypars = TypeReprEnv.Empty.ForTypars tps
    let methodArgTys, paramInfos = curriedArgInfos |> List.concat |> List.unzip
    let ilMethodArgTys = GenParamTypes cenv m tyenvUnderTypars false methodArgTys
    let ilRetTy = GenReturnType cenv m tyenvUnderTypars returnTy
    let ilLocTy = mkILTyForCompLoc cloc
    let ilMethodInst = GenTypeArgs cenv m tyenvUnderTypars (List.map mkTyparTy tps)

    let mspec =
        mkILStaticMethSpecInTy (ilLocTy, nm, ilMethodArgTys, ilRetTy, ilMethodInst)

    let mspecW =
        if not g.generateWitnesses || witnessInfos.IsEmpty then
            mspec
        else
            let ilWitnessArgTys =
                GenTypes cenv m tyenvUnderTypars (GenWitnessTys g witnessInfos)

            mkILStaticMethSpecInTy (ilLocTy, ExtraWitnessMethodName nm, (ilWitnessArgTys @ ilMethodArgTys), ilRetTy, ilMethodInst)

    Method(valReprInfo, vref, mspec, mspecW, m, [], tps, curriedArgInfos, paramInfos, witnessInfos, methodArgTys, retInfo)

/// Determine if an F#-declared value, method or function is compiled as a method.
let IsFSharpValCompiledAsMethod g (v: Val) =
    match v.ValReprInfo with
    | None -> false
    | Some valReprInfo ->
        not (isUnitTy g v.Type && not v.IsMemberOrModuleBinding && not v.IsMutable)
        && not v.IsCompiledAsStaticPropertyWithoutField
        && match GetValReprTypeInFSharpForm g valReprInfo v.Type v.Range with
           | [], [], _, _ when not v.IsMember -> false
           | _ -> true

/// Determine how a top level value is represented, when it is being represented
/// as a method. This depends on its type and other representation information.
/// If it's a function or is polymorphic, then it gets represented as a
/// method (possibly and instance method). Otherwise it gets represented as a
/// static field and property.
let ComputeStorageForValWithValReprInfo
    (
        cenv,
        g,
        optIntraAssemblyInfo: IlxGenIntraAssemblyInfo option,
        isInteractive,
        optShadowLocal,
        vref: ValRef,
        cloc
    ) =

    if isUnitTy g vref.Type && not vref.IsMemberOrModuleBinding && not vref.IsMutable then
        Null
    else
        let valReprInfo =
            match vref.ValReprInfo with
            | None ->
                error (
                    InternalError(
                        "ComputeStorageForValWithValReprInfo: no ValReprInfo found for "
                        + showL (valRefL vref),
                        vref.Range
                    )
                )
            | Some a -> a

        let m = vref.Range
        let nm = vref.CompiledName g.CompilerGlobalState

        if vref.Deref.IsCompiledAsStaticPropertyWithoutField then
            let nm = "get_" + nm
            let tyenvUnderTypars = TypeReprEnv.Empty.ForTypars []
            let ilRetTy = GenType cenv m tyenvUnderTypars vref.Type
            let ty = mkILTyForCompLoc cloc
            let mspec = mkILStaticMethSpecInTy (ty, nm, [], ilRetTy, [])

            StaticProperty(mspec, optShadowLocal)
        else

            // Determine when a static field is required.
            //
            // REVIEW: This call to GetValReprTypeInFSharpForm is only needed to determine if this is a (type) function or a value
            // We should just look at the arity
            match GetValReprTypeInFSharpForm g valReprInfo vref.Type vref.Range with
            | [], [], returnTy, _ when not vref.IsMember ->
                ComputeStorageForFSharpValue cenv g cloc optIntraAssemblyInfo optShadowLocal isInteractive returnTy vref m
            | _ ->
                match vref.MemberInfo with
                | Some memberInfo when not vref.IsExtensionMember -> ComputeStorageForFSharpMember cenv valReprInfo memberInfo vref m
                | _ -> ComputeStorageForFSharpFunctionOrFSharpExtensionMember cenv cloc valReprInfo vref m

/// Determine how an F#-declared value, function or member is represented, if it is in the assembly being compiled.
let ComputeAndAddStorageForLocalValWithValReprInfo (cenv, g, intraAssemblyFieldTable, isInteractive, optShadowLocal) cloc (v: Val) eenv =
    let storage =
        ComputeStorageForValWithValReprInfo(cenv, g, Some intraAssemblyFieldTable, isInteractive, optShadowLocal, mkLocalValRef v, cloc)

    AddStorageForVal g (v, notlazy storage) eenv

/// Determine how an F#-declared value, function or member is represented, if it is an external assembly.
let ComputeStorageForNonLocalVal cenv g cloc modref (v: Val) =
    match v.ValReprInfo with
    | None -> error (InternalError("ComputeStorageForNonLocalVal, expected an ValReprInfo for " + v.LogicalName, v.Range))
    | Some _ -> ComputeStorageForValWithValReprInfo(cenv, g, None, false, NoShadowLocal, mkNestedValRef modref v, cloc)

/// Determine how all the F#-declared top level values, functions and members are represented, for an external module or namespace.
let rec AddStorageForNonLocalModuleOrNamespaceRef cenv g cloc acc (modref: ModuleOrNamespaceRef) (modul: ModuleOrNamespace) =
    let acc =
        (acc, modul.ModuleOrNamespaceType.ModuleAndNamespaceDefinitions)
        ||> List.fold (fun acc smodul ->
            AddStorageForNonLocalModuleOrNamespaceRef
                cenv
                g
                (CompLocForSubModuleOrNamespace cloc smodul)
                acc
                (modref.NestedTyconRef smodul)
                smodul)

    let acc =
        (acc, modul.ModuleOrNamespaceType.AllValsAndMembers)
        ||> Seq.fold (fun acc v -> AddStorageForVal g (v, lazy (ComputeStorageForNonLocalVal cenv g cloc modref v)) acc)

    acc

/// Determine how all the F#-declared top level values, functions and members are represented, for an external assembly.
let AddStorageForExternalCcu cenv g eenv (ccu: CcuThunk) =
    if not ccu.IsFSharp then
        eenv
    else
        let cloc = CompLocForCcu ccu

        let eenv =
            List.foldBack
                (fun smodul acc ->
                    let cloc = CompLocForSubModuleOrNamespace cloc smodul
                    let modref = mkNonLocalCcuRootEntityRef ccu smodul
                    AddStorageForNonLocalModuleOrNamespaceRef cenv g cloc acc modref smodul)
                ccu.RootModulesAndNamespaces
                eenv

        let eenv =
            let eref = ERefNonLocalPreResolved ccu.Contents (mkNonLocalEntityRef ccu [||])

            (eenv, ccu.Contents.ModuleOrNamespaceType.AllValsAndMembers)
            ||> Seq.fold (fun acc v -> AddStorageForVal g (v, lazy (ComputeStorageForNonLocalVal cenv g cloc eref v)) acc)

        eenv

/// Record how all the top level F#-declared values, functions and members are represented, for a local module or namespace.
let rec AddBindingsForLocalModuleOrNamespaceType allocVal cloc eenv (mty: ModuleOrNamespaceType) =
    let eenv =
        List.fold
            (fun eenv submodul ->
                AddBindingsForLocalModuleOrNamespaceType
                    allocVal
                    (CompLocForSubModuleOrNamespace cloc submodul)
                    eenv
                    submodul.ModuleOrNamespaceType)
            eenv
            mty.ModuleAndNamespaceDefinitions

    let eenv = Seq.fold (fun eenv v -> allocVal cloc v eenv) eenv mty.AllValsAndMembers
    eenv

/// Record how all the top level F#-declared values, functions and members are represented, for a set of referenced assemblies.
let AddExternalCcusToIlxGenEnv cenv g eenv ccus =
    List.fold (AddStorageForExternalCcu cenv g) eenv ccus

/// Record how all the unrealized abstract slots are represented, for a type definition.
let AddBindingsForTycon allocVal (cloc: CompileLocation) (tycon: Tycon) eenv =
    let unrealizedSlots =
        if tycon.IsFSharpObjectModelTycon then
            tycon.FSharpObjectModelTypeInfo.fsobjmodel_vslots
        else
            []

    (eenv, unrealizedSlots)
    ||> List.fold (fun eenv vref -> allocVal cloc vref.Deref eenv)

/// Record how constructs are represented, for a sequence of definitions in a module or namespace fragment.
let AddDebugImportsToEnv (cenv: cenv) eenv (openDecls: OpenDeclaration list) =
    let ilImports =
        [|
            for openDecl in openDecls do
                for modul in openDecl.Modules do
                    if modul.IsNamespace then
                        ILDebugImport.ImportNamespace(fullDisplayTextOfModRef modul)
                    else
                        ILDebugImport.ImportType(mkILNonGenericBoxedTy modul.CompiledRepresentationForNamedType)

                for t in openDecl.Types do
                    let m = defaultArg openDecl.Range Range.range0
                    ILDebugImport.ImportType(GenType cenv m TypeReprEnv.Empty t)
        |]

    if ilImports.Length = 0 then
        eenv
    else
        // We flatten _all_ the import scopes, creating repetition, because C# debug engine doesn't seem to handle
        // nesting of import scopes at all. This means every new "open" in, say, a nested module in F# causes
        // duplication of all the implicit/enclosing "open" in within the debug information.
        // However overall there are not very many "open" declarations and debug information can be large
        // so this is not considered a problem.
        let imports =
            [|
                match eenv.imports with
                | None -> ()
                | Some parent -> yield! parent.Imports
                yield! ilImports
            |]
            |> Array.filter (function
                | ILDebugImport.ImportNamespace _ -> true
                | ILDebugImport.ImportType t ->
                    t.IsNominal
                    &&
                    // We filter out FSI_NNNN types (dynamic modules), since we don't really need them in the import tables.
                    not (
                        t.QualifiedName.StartsWithOrdinal FsiDynamicModulePrefix
                        && t.TypeRef.Scope = ILScopeRef.Local
                    ))
            |> Array.distinctBy (function
                | ILDebugImport.ImportNamespace nsp -> nsp
                | ILDebugImport.ImportType t -> t.QualifiedName)

        { eenv with
            imports = Some { Parent = None; Imports = imports }
        }

let rec AddBindingsForModuleOrNamespaceContents allocVal cloc eenv x =
    match x with
    | TMDefRec (_isRec, _opens, tycons, mbinds, _) ->
        // Virtual don't have 'let' bindings and must be added to the environment
        let eenv = List.foldBack (AddBindingsForTycon allocVal cloc) tycons eenv

        let eenv =
            List.foldBack (AddBindingsForModuleOrNamespaceBinding allocVal cloc) mbinds eenv

        eenv
    | TMDefLet (bind, _) -> allocVal cloc bind.Var eenv
    | TMDefDo _ -> eenv
    | TMDefOpens _ -> eenv
    | TMDefs mdefs ->
        (eenv, mdefs)
        ||> List.fold (AddBindingsForModuleOrNamespaceContents allocVal cloc)

/// Record how constructs are represented, for a module or namespace.
and AddBindingsForModuleOrNamespaceBinding allocVal cloc x eenv =
    match x with
    | ModuleOrNamespaceBinding.Binding bind -> allocVal cloc bind.Var eenv
    | ModuleOrNamespaceBinding.Module (mspec, mdef) ->
        let cloc =
            if mspec.IsNamespace then
                cloc
            else
                CompLocForFixedModule cloc.QualifiedNameOfFile cloc.TopImplQualifiedName mspec

        AddBindingsForModuleOrNamespaceContents allocVal cloc eenv mdef

/// Put the partial results for a generated fragment (i.e. a part of a CCU generated by FSI)
/// into the stored results for the whole CCU.
/// isIncrementalFragment = true --> "typed input"
/// isIncrementalFragment = false --> "#load"
let AddIncrementalLocalAssemblyFragmentToIlxGenEnv
    (
        cenv: cenv,
        isIncrementalFragment,
        g,
        ccu,
        fragName,
        intraAssemblyInfo,
        eenv,
        implFiles
    ) =
    let cloc = CompLocForFragment fragName ccu

    let allocVal =
        ComputeAndAddStorageForLocalValWithValReprInfo(cenv, g, intraAssemblyInfo, true, NoShadowLocal)

    (eenv, implFiles)
    ||> List.fold (fun eenv implFile ->
        let (CheckedImplFile (qualifiedNameOfFile = qname; signature = signature; contents = contents)) =
            implFile

        let cloc =
            { cloc with
                TopImplQualifiedName = qname.Text
            }

        if isIncrementalFragment then
            AddBindingsForModuleOrNamespaceContents allocVal cloc eenv contents
        else
            AddBindingsForLocalModuleOrNamespaceType allocVal cloc eenv signature)

//--------------------------------------------------------------------------
// Generate debugging marks
//--------------------------------------------------------------------------

/// Generate IL debugging information.
let GenILSourceMarker (g: TcGlobals) (m: range) =
    ILDebugPoint.Create(
        document = g.memoize_file m.FileIndex,
        line = m.StartLine,
        // NOTE: .NET && VS measure first column as column 1
        column = m.StartColumn + 1,
        endLine = m.EndLine,
        endColumn = m.EndColumn + 1
    )

/// Optionally generate DebugRange for methods.  This gets attached to the whole method.
let GenPossibleILDebugRange (cenv: cenv) m =
    if cenv.options.generateDebugSymbols then
        Some(GenILSourceMarker cenv.g m)
    else
        None

//--------------------------------------------------------------------------
// Helpers for merging property definitions
//--------------------------------------------------------------------------

let HashRangeSorted (ht: IDictionary<_, int * _>) =
    [ for KeyValue (_k, v) in ht -> v ] |> List.sortBy fst |> List.map snd

let MergeOptions m o1 o2 =
    match o1, o2 with
    | Some x, None
    | None, Some x -> Some x
    | None, None -> None
    | Some x, Some _ ->
#if DEBUG
        // This warning fires on some code that also triggers this warning:
        //    The implementation of a specified generic interface
        //    required a method implementation not fully supported by F# Interactive. In
        //    the unlikely event that the resulting class fails to load then compile
        //    the interface type into a statically-compiled DLL and reference it using '#r'
        // The code is OK so we don't print this.
        errorR (InternalError("MergeOptions: two values given", m))
#else
        ignore m
#endif
        Some x

let MergePropertyPair m (pd: ILPropertyDef) (pdef: ILPropertyDef) =
    pd.With(getMethod = MergeOptions m pd.GetMethod pdef.GetMethod, setMethod = MergeOptions m pd.SetMethod pdef.SetMethod)

type PropKey = PropKey of string * ILTypes * ILThisConvention

let AddPropertyDefToHash (m: range) (ht: Dictionary<PropKey, int * ILPropertyDef>) (pdef: ILPropertyDef) =
    let nm = PropKey(pdef.Name, pdef.Args, pdef.CallingConv)

    match ht.TryGetValue nm with
    | true, (idx, pd) -> ht[nm] <- (idx, MergePropertyPair m pd pdef)
    | _ -> ht[nm] <- (ht.Count, pdef)

/// Merge a whole group of properties all at once
let MergePropertyDefs m ilPropertyDefs =
    let ht = Dictionary<_, _>(3, HashIdentity.Structural)
    ilPropertyDefs |> List.iter (AddPropertyDefToHash m ht)
    HashRangeSorted ht

//--------------------------------------------------------------------------
// Buffers for compiling modules. The entire assembly gets compiled via an AssemblyBuilder
//--------------------------------------------------------------------------

/// Information collected imperatively for each type definition
type TypeDefBuilder(tdef: ILTypeDef, tdefDiscards) =
    let gmethods = ResizeArray<ILMethodDef>(0)
    let gfields = ResizeArray<ILFieldDef>(0)

    let gproperties: Dictionary<PropKey, int * ILPropertyDef> =
        Dictionary<_, _>(3, HashIdentity.Structural)

    let gevents = ResizeArray<ILEventDef>(0)
    let gnested = TypeDefsBuilder()

    member b.Close() =
        tdef.With(
            methods = mkILMethods (tdef.Methods.AsList() @ ResizeArray.toList gmethods),
            fields = mkILFields (tdef.Fields.AsList() @ ResizeArray.toList gfields),
            properties = mkILProperties (tdef.Properties.AsList() @ HashRangeSorted gproperties),
            events = mkILEvents (tdef.Events.AsList() @ ResizeArray.toList gevents),
            nestedTypes = mkILTypeDefs (tdef.NestedTypes.AsList() @ gnested.Close())
        )

    member b.AddEventDef edef = gevents.Add edef

    member b.AddFieldDef ilFieldDef = gfields.Add ilFieldDef

    member b.AddMethodDef ilMethodDef =
        let discard =
            match tdefDiscards with
            | Some (mdefDiscard, _) -> mdefDiscard ilMethodDef
            | None -> false

        if not discard then
            gmethods.Add ilMethodDef

    member _.NestedTypeDefs = gnested

    member _.GetCurrentFields() = gfields |> Seq.readonly

    /// Merge Get and Set property nodes, which we generate independently for F# code
    /// when we come across their corresponding methods.
    member _.AddOrMergePropertyDef(pdef, m) =
        let discard =
            match tdefDiscards with
            | Some (_, pdefDiscard) -> pdefDiscard pdef
            | None -> false

        if not discard then
            AddPropertyDefToHash m gproperties pdef

    member _.PrependInstructionsToSpecificMethodDef(cond, instrs, tag, imports) =
        match ResizeArray.tryFindIndex cond gmethods with
        | Some idx -> gmethods[idx] <- prependInstrsToMethod instrs gmethods[idx]
        | None ->
            let body =
                mkMethodBody (false, [], 1, nonBranchingInstrsToCode instrs, tag, imports)

            gmethods.Add(mkILClassCtor body)

and TypeDefsBuilder() =
    let tdefs: HashMultiMap<string, int * (TypeDefBuilder * bool)> =
        HashMultiMap(0, HashIdentity.Structural)

    let mutable countDown = System.Int32.MaxValue

    member b.Close() =
        //The order we emit type definitions is not deterministic since it is using the reverse of a range from a hash table. We should use an approximation of source order.
        // Ideally it shouldn't matter which order we use.
        // However, for some tests FSI generated code appears sensitive to the order, especially for nested types.

        [
            for b, eliminateIfEmpty in HashRangeSorted tdefs do
                let tdef = b.Close()
                // Skip the <PrivateImplementationDetails$> type if it is empty
                if
                    not eliminateIfEmpty
                    || not (tdef.NestedTypes.AsList()).IsEmpty
                    || not (tdef.Fields.AsList()).IsEmpty
                    || not (tdef.Events.AsList()).IsEmpty
                    || not (tdef.Properties.AsList()).IsEmpty
                    || not (Array.isEmpty (tdef.Methods.AsArray()))
                then
                    yield tdef
        ]

    member b.FindTypeDefBuilder nm =
        try
            tdefs[nm] |> snd |> fst
        with :? KeyNotFoundException ->
            failwith ("FindTypeDefBuilder: " + nm + " not found")

    member b.FindNestedTypeDefsBuilder path =
        List.fold (fun (acc: TypeDefsBuilder) x -> acc.FindTypeDefBuilder(x).NestedTypeDefs) b path

    member b.FindNestedTypeDefBuilder(tref: ILTypeRef) =
        b.FindNestedTypeDefsBuilder(tref.Enclosing).FindTypeDefBuilder(tref.Name)

    member b.AddTypeDef(tdef: ILTypeDef, eliminateIfEmpty, addAtEnd, tdefDiscards) =
        let idx =
            if addAtEnd then
                (countDown <- countDown - 1
                 countDown)
            else
                tdefs.Count

        tdefs.Add(tdef.Name, (idx, (TypeDefBuilder(tdef, tdefDiscards), eliminateIfEmpty)))

type AnonTypeGenerationTable() =
    // Dictionary is safe here as it will only be used during the codegen stage - will happen on a single thread.
    let dict =
        Dictionary<Stamp, ILMethodRef * ILMethodRef[] * ILType>(HashIdentity.Structural)

    member _.Table = dict

/// Assembly generation buffers
type AssemblyBuilder(cenv: cenv, anonTypeTable: AnonTypeGenerationTable) as mgbuf =
    let g = cenv.g
    // The Abstract IL table of types
    let gtdefs = TypeDefsBuilder()

    // The definitions of top level values, as quotations.
    // Dictionary is safe here as it will only be used during the codegen stage - will happen on a single thread.
    let mutable reflectedDefinitions: Dictionary<Val, string * int * Expr> =
        Dictionary(HashIdentity.Reference)

    let mutable extraBindingsToGenerate = []

    // A memoization table for generating value types for big constant arrays
    let rawDataValueTypeGenerator =
        MemoizationTable<CompileLocation * int, ILTypeSpec>(
            (fun (cloc, size) ->
                let name =
                    CompilerGeneratedName("T" + string (newUnique ()) + "_" + string size + "Bytes") // Type names ending ...$T<unique>_37Bytes

                let vtdef = mkRawDataValueTypeDef g.iltyp_ValueType (name, size, 0us)
                let vtref = NestedTypeRefForCompLoc cloc vtdef.Name
                let vtspec = mkILTySpec (vtref, [])
                let vtdef = vtdef.WithAccess(ComputeTypeAccess vtref true)
                mgbuf.AddTypeDef(vtref, vtdef, false, true, None)
                vtspec),
            keyComparer = HashIdentity.Structural
        )

    let generateAnonType genToStringMethod (isStruct, ilTypeRef, nms) =

        let propTys = [ for i, nm in Array.indexed nms -> nm, ILType.TypeVar(uint16 i) ]

        // Note that this alternative below would give the same names as C#, but the generated
        // comparison/equality doesn't know about these names.
        //let flds = [ for (i, nm) in Array.indexed nms -> (nm, "<" + nm + ">" + "i__Field", ILType.TypeVar (uint16 i)) ]
        let ilCtorRef =
            mkILMethRef (ilTypeRef, ILCallingConv.Instance, ".ctor", 0, List.map snd propTys, ILType.Void)

        let ilMethodRefs =
            [|
                for propName, propTy in propTys -> mkILMethRef (ilTypeRef, ILCallingConv.Instance, "get_" + propName, 0, [], propTy)
            |]

        let ilTy =
            mkILNamedTy (if isStruct then ILBoxity.AsValue else ILBoxity.AsObject) ilTypeRef (List.map snd propTys)

        if ilTypeRef.Scope.IsLocalRef then

            let flds =
                [ for i, nm in Array.indexed nms -> (nm, nm + "@", ILType.TypeVar(uint16 i)) ]

            let ilGenericParams =
                [
                    for nm in nms ->
                        {
                            Name = sprintf "<%s>j__TPar" nm
                            Constraints = []
                            Variance = NonVariant
                            CustomAttrsStored = storeILCustomAttrs emptyILCustomAttrs
                            HasReferenceTypeConstraint = false
                            HasNotNullableValueTypeConstraint = false
                            HasDefaultConstructorConstraint = false
                            MetadataIndex = NoMetadataIdx
                        }
                ]

            let ilTy =
                mkILFormalNamedTy (if isStruct then ILBoxity.AsValue else ILBoxity.AsObject) ilTypeRef ilGenericParams

            // Generate the IL fields
            let ilFieldDefs =
                mkILFields
                    [
                        for _, fldName, fldTy in flds ->

                            let access =
                                if cenv.options.isInteractive && cenv.options.fsiMultiAssemblyEmit then
                                    ILMemberAccess.Public
                                else
                                    ILMemberAccess.Private

                            let fdef = mkILInstanceField (fldName, fldTy, None, access)
                            let attrs = [ g.CompilerGeneratedAttribute; g.DebuggerBrowsableNeverAttribute ]
                            fdef.With(customAttrs = mkILCustomAttrs attrs)
                    ]

            // Generate property definitions for the fields compiled as properties
            let ilProperties =
                mkILProperties
                    [
                        for i, (propName, _fldName, fldTy) in List.indexed flds ->
                            ILPropertyDef(
                                name = propName,
                                attributes = PropertyAttributes.None,
                                setMethod = None,
                                getMethod = Some(mkILMethRef (ilTypeRef, ILCallingConv.Instance, "get_" + propName, 0, [], fldTy)),
                                callingConv = ILCallingConv.Instance.ThisConv,
                                propertyType = fldTy,
                                init = None,
                                args = [],
                                customAttrs = mkILCustomAttrs [ mkCompilationMappingAttrWithSeqNum g (int SourceConstructFlags.Field) i ]
                            )
                    ]

            let ilMethods =
                [
                    for propName, fldName, fldTy in flds ->
                        let attrs = if isStruct then [ GenReadOnlyAttribute g ] else []

                        mkLdfldMethodDef ("get_" + propName, ILMemberAccess.Public, false, ilTy, fldName, fldTy, attrs)
                        |> g.AddMethodGeneratedAttributes
                    yield! genToStringMethod ilTy
                ]

            let ilBaseTy = (if isStruct then g.iltyp_ValueType else g.ilg.typ_Object)

            let ilBaseTySpec = (if isStruct then None else Some ilBaseTy.TypeSpec)

            let ilCtorDef =
                mkILSimpleStorageCtorWithParamNames (ilBaseTySpec, ilTy, [], flds, ILMemberAccess.Public, None, None)

            // Create a tycon that looks exactly like a record definition, to help drive the generation of equality/comparison code
            let m = range0

            let tps =
                [
                    for nm in nms ->
                        let stp = SynTypar(mkSynId m ("T" + nm), TyparStaticReq.None, true)
                        Construct.NewTypar(TyparKind.Type, TyparRigidity.WarnIfNotRigid, stp, false, TyparDynamicReq.Yes, [], true, true)
                ]

            let tycon =
                let lmtyp = MaybeLazy.Strict(Construct.NewEmptyModuleOrNamespaceType ModuleOrType)
                let cpath = CompPath(ilTypeRef.Scope, [])

                Construct.NewTycon(
                    Some cpath,
                    ilTypeRef.Name,
                    m,
                    taccessPublic,
                    taccessPublic,
                    TyparKind.Type,
                    LazyWithContext.NotLazy tps,
                    XmlDoc.Empty,
                    false,
                    false,
                    false,
                    lmtyp
                )

            if isStruct then
                tycon.SetIsStructRecordOrUnion true

            tycon.entity_tycon_repr <-
                TFSharpRecdRepr(
                    Construct.MakeRecdFieldsTable(
                        (tps, flds)
                        ||> List.map2 (fun tp (propName, _fldName, _fldTy) ->
                            Construct.NewRecdField
                                false
                                None
                                (mkSynId m propName)
                                false
                                (mkTyparTy tp)
                                true
                                false
                                []
                                []
                                XmlDoc.Empty
                                taccessPublic
                                false)
                    )
                )

            let tcref = mkLocalTyconRef tycon
            let ty = generalizedTyconRef g tcref
            let tcaug = tcref.TypeContents

            tcaug.tcaug_interfaces <-
                [
                    (g.mk_IStructuralComparable_ty, true, m)
                    (g.mk_IComparable_ty, true, m)
                    (mkAppTy g.system_GenericIComparable_tcref [ ty ], true, m)
                    (g.mk_IStructuralEquatable_ty, true, m)
                    (mkAppTy g.system_GenericIEquatable_tcref [ ty ], true, m)
                ]

            let vspec1, vspec2 = AugmentWithHashCompare.MakeValsForEqualsAugmentation g tcref

            let evspec1, evspec2, evspec3 =
                AugmentWithHashCompare.MakeValsForEqualityWithComparerAugmentation g tcref

            let cvspec1, cvspec2 = AugmentWithHashCompare.MakeValsForCompareAugmentation g tcref

            let cvspec3 =
                AugmentWithHashCompare.MakeValsForCompareWithComparerAugmentation g tcref

            tcaug.SetCompare(mkLocalValRef cvspec1, mkLocalValRef cvspec2)
            tcaug.SetCompareWith(mkLocalValRef cvspec3)
            tcaug.SetEquals(mkLocalValRef vspec1, mkLocalValRef vspec2)
            tcaug.SetHashAndEqualsWith(mkLocalValRef evspec1, mkLocalValRef evspec2, mkLocalValRef evspec3)

            // Build the ILTypeDef. We don't rely on the normal record generation process because we want very specific field names

            let ilTypeDefAttribs =
                mkILCustomAttrs
                    [
                        g.CompilerGeneratedAttribute
                        mkCompilationMappingAttr g (int SourceConstructFlags.RecordType)
                    ]

            let ilInterfaceTys =
                [
                    for intfTy, _, _ in tcaug.tcaug_interfaces -> GenType cenv m (TypeReprEnv.Empty.ForTypars tps) intfTy
                ]

            let ilTypeDef =
                mkILGenericClass (
                    ilTypeRef.Name,
                    ILTypeDefAccess.Public,
                    ilGenericParams,
                    ilBaseTy,
                    ilInterfaceTys,
                    mkILMethods (ilCtorDef :: ilMethods),
                    ilFieldDefs,
                    emptyILTypeDefs,
                    ilProperties,
                    mkILEvents [],
                    ilTypeDefAttribs,
                    ILTypeInit.BeforeField
                )

            let ilTypeDef = ilTypeDef.WithSealed(true).WithSerializable(true)

            mgbuf.AddTypeDef(ilTypeRef, ilTypeDef, false, true, None)

            let extraBindings =
                [
                    yield! AugmentWithHashCompare.MakeBindingsForCompareAugmentation g tycon
                    yield! AugmentWithHashCompare.MakeBindingsForCompareWithComparerAugmentation g tycon
                    yield! AugmentWithHashCompare.MakeBindingsForEqualityWithComparerAugmentation g tycon
                    yield! AugmentWithHashCompare.MakeBindingsForEqualsAugmentation g tycon
                ]

            let optimizedExtraBindings =
                extraBindings
                |> List.map (fun (TBind (a, b, c)) ->
                    // Disable method splitting for bindings related to anonymous records
                    TBind(a, cenv.optimizeDuringCodeGen true b, c))

            extraBindingsToGenerate <- optimizedExtraBindings @ extraBindingsToGenerate

        (ilCtorRef, ilMethodRefs, ilTy)

    let mutable explicitEntryPointInfo: ILTypeRef option = None

    /// static init fields on script modules.
    let mutable scriptInitFspecs: (ILFieldSpec * range) list = []

    member _.AddScriptInitFieldSpec(fieldSpec, range) =
        scriptInitFspecs <- (fieldSpec, range) :: scriptInitFspecs

    /// This initializes the script in #load and fsc command-line order causing their
    /// side effects to be executed.
    member mgbuf.AddInitializeScriptsInOrderToEntryPoint(imports) =
        // Get the entry point and initialized any scripts in order.
        match explicitEntryPointInfo with
        | Some tref ->
            let InitializeCompiledScript (fspec, m) =
                let ilDebugRange = GenPossibleILDebugRange cenv m

                mgbuf.AddExplicitInitToSpecificMethodDef(
                    (fun (md: ILMethodDef) -> md.IsEntryPoint),
                    tref,
                    fspec,
                    ilDebugRange,
                    imports,
                    [],
                    []
                )

            scriptInitFspecs |> List.iter InitializeCompiledScript
        | None -> ()

    member _.GenerateRawDataValueType(cloc, size) =
        // Byte array literals require a ValueType of size the required number of bytes.
        // With fsi.exe, S.R.Emit TypeBuilder CreateType has restrictions when a ValueType VT is nested inside a type T, and T has a field of type VT.
        // To avoid this situation, these ValueTypes are generated under the private implementation rather than in the current cloc. [was bug 1532].
        let cloc = CompLocForPrivateImplementationDetails cloc
        rawDataValueTypeGenerator.Apply((cloc, size))

    member _.GenerateAnonType(genToStringMethod, anonInfo: AnonRecdTypeInfo) =
        let isStruct = evalAnonInfoIsStruct anonInfo
        let key = anonInfo.Stamp

        if not (anonTypeTable.Table.ContainsKey key) then
            let info =
                generateAnonType genToStringMethod (isStruct, anonInfo.ILTypeRef, anonInfo.SortedNames)

            anonTypeTable.Table[ key ] <- info

    member this.LookupAnonType(genToStringMethod, anonInfo: AnonRecdTypeInfo) =
        match anonTypeTable.Table.TryGetValue anonInfo.Stamp with
        | true, res -> res
        | _ ->
            if anonInfo.ILTypeRef.Scope.IsLocalRef then
                failwithf "the anonymous record %A has not been generated in the pre-phase of generating this module" anonInfo.ILTypeRef

            this.GenerateAnonType(genToStringMethod, anonInfo)
            anonTypeTable.Table[anonInfo.Stamp]

    member _.GrabExtraBindingsToGenerate() =
        let result = extraBindingsToGenerate
        extraBindingsToGenerate <- []
        result

    member _.AddTypeDef(tref: ILTypeRef, tdef, eliminateIfEmpty, addAtEnd, tdefDiscards) =
        gtdefs
            .FindNestedTypeDefsBuilder(tref.Enclosing)
            .AddTypeDef(tdef, eliminateIfEmpty, addAtEnd, tdefDiscards)

    member _.GetCurrentFields(tref: ILTypeRef) =
        gtdefs.FindNestedTypeDefBuilder(tref).GetCurrentFields()

    member _.AddReflectedDefinition(vspec: Val, expr) =
        // preserve order by storing index of item
        let n = reflectedDefinitions.Count
        reflectedDefinitions.Add(vspec, (vspec.CompiledName cenv.g.CompilerGlobalState, n, expr))

    member _.ReplaceNameOfReflectedDefinition(vspec, newName) =
        match reflectedDefinitions.TryGetValue vspec with
        | true, (name, n, expr) when name <> newName -> reflectedDefinitions[vspec] <- (newName, n, expr)
        | _ -> ()

    member _.AddMethodDef(tref: ILTypeRef, ilMethodDef) =
        gtdefs.FindNestedTypeDefBuilder(tref).AddMethodDef(ilMethodDef)

        if ilMethodDef.IsEntryPoint then
            explicitEntryPointInfo <- Some tref

    member _.AddExplicitInitToSpecificMethodDef(cond, tref, fspec, sourceOpt, imports, feefee, seqpt) =
        // Authoring a .cctor with effects forces the cctor for the 'initialization' module by doing a dummy store & load of a field
        // Doing both a store and load keeps FxCop happier because it thinks the field is useful
        let instrs =
            [
                yield!
                    (if condition "NO_ADD_FEEFEE_TO_CCTORS" then []
                     elif condition "ADD_SEQPT_TO_CCTORS" then seqpt
                     else feefee) // mark start of hidden code
                yield mkLdcInt32 0
                yield mkNormalStsfld fspec
                yield mkNormalLdsfld fspec
                yield AI_pop
            ]

        gtdefs
            .FindNestedTypeDefBuilder(tref)
            .PrependInstructionsToSpecificMethodDef(cond, instrs, sourceOpt, imports)

    member _.AddEventDef(tref, edef) =
        gtdefs.FindNestedTypeDefBuilder(tref).AddEventDef(edef)

    member _.AddFieldDef(tref, ilFieldDef) =
        gtdefs.FindNestedTypeDefBuilder(tref).AddFieldDef(ilFieldDef)

    member _.AddOrMergePropertyDef(tref, pdef, m) =
        gtdefs.FindNestedTypeDefBuilder(tref).AddOrMergePropertyDef(pdef, m)

    member _.Close() =
        // old implementation adds new element to the head of list so result was accumulated in reversed order
        let orderedReflectedDefinitions =
            [
                for KeyValue (vspec, (name, n, expr)) in reflectedDefinitions -> n, ((name, vspec), expr)
            ]
            |> List.sortBy (fst >> (~-)) // invert the result to get 'order-by-descending' behavior (items in list are 0..* so we don't need to worry about int.MinValue)
            |> List.map snd

        gtdefs.Close(), orderedReflectedDefinitions

    member _.cenv = cenv

    member _.GetExplicitEntryPointInfo() = explicitEntryPointInfo

/// Record the types of the things on the evaluation stack.
/// Used for the few times we have to flush the IL evaluation stack and to compute maxStack.
let pop (i: int) : Pops = i

let Push tys : Pushes = tys
let Push0 = Push []

let FeeFee (cenv: cenv) =
    (if cenv.options.testFlagEmitFeeFeeAs100001 then
         100001
     else
         0x00feefee)

let FeeFeeInstr (cenv: cenv) doc =
    I_seqpoint(ILDebugPoint.Create(document = doc, line = FeeFee cenv, column = 0, endLine = FeeFee cenv, endColumn = 0))

/// Buffers for IL code generation
type CodeGenBuffer(m: range, mgbuf: AssemblyBuilder, methodName, alreadyUsedArgs: int) =

    let g = mgbuf.cenv.g
    let locals = ResizeArray<(string * (Mark * Mark)) list * ILType * bool>(10)
    let codebuf = ResizeArray<ILInstr>(200)
    let exnSpecs = ResizeArray<ILExceptionSpec>(10)

    // Keep track of the current stack so we can spill stuff when we hit a "try" when some stuff
    // is on the stack.
    let mutable stack: ILType list = []
    let mutable nstack = 0
    let mutable maxStack = 0
    let mutable hasDebugPoints = false
    let mutable anyDocument = None // we collect an arbitrary document in order to emit the header FeeFee if needed

    let codeLabelToPC: Dictionary<ILCodeLabel, int> = Dictionary<_, _>(10)

    let codeLabelToCodeLabel: Dictionary<ILCodeLabel, ILCodeLabel> =
        Dictionary<_, _>(10)

    let rec lab2pc n lbl =
        if n = System.Int32.MaxValue then
            error (InternalError("recursive label graph", m))

        match codeLabelToCodeLabel.TryGetValue lbl with
        | true, l -> lab2pc (n + 1) l
        | _ -> codeLabelToPC[lbl]

    // Add a nop to make way for the first debug point.
    do
        if mgbuf.cenv.options.generateDebugSymbols then
            let doc = g.memoize_file m.FileIndex
            let i = FeeFeeInstr mgbuf.cenv doc
            codebuf.Add i // for the FeeFee or a better debug point

    member _.DoPushes(pushes: Pushes) =
        for ty in pushes do
            stack <- ty :: stack
            nstack <- nstack + 1
            maxStack <- Operators.max maxStack nstack

    member _.DoPops(n: Pops) =
        for i = 0 to n - 1 do
            match stack with
            | [] ->
                let msg =
                    sprintf "pop on empty stack during code generation, methodName = %s, m = %s" methodName (stringOfRange m)

                System.Diagnostics.Debug.Assert(false, msg)
                warning (InternalError(msg, m))
            | _ :: t ->
                stack <- t
                nstack <- nstack - 1

    member _.GetCurrentStack() = stack

    member _.AssertEmptyStack() =
        if not (isNil stack) then
            let msg =
                sprintf
                    "stack flush didn't work, or extraneous expressions left on stack before stack restore, methodName = %s, stack = %+A, m = %s"
                    methodName
                    stack
                    (stringOfRange m)

            System.Diagnostics.Debug.Assert(false, msg)
            warning (InternalError(msg, m))

        ()

    member cgbuf.EmitInstr(pops, pushes, i) =
        cgbuf.DoPops pops
        cgbuf.DoPushes pushes
        codebuf.Add i

    member cgbuf.EmitInstrs(pops, pushes, is) =
        cgbuf.DoPops pops
        cgbuf.DoPushes pushes
        is |> List.iter codebuf.Add

    member private _.EnsureNopBetweenDebugPoints() =
        // Always add a nop between debug points to help .NET get the stepping right
        // Don't do this after a FeeFee marker for hidden code
        if
            (codebuf.Count > 0
             && (match codebuf[codebuf.Count - 1] with
                 | I_seqpoint sm when sm.Line <> FeeFee mgbuf.cenv -> true
                 | _ -> false))
        then

            codebuf.Add(AI_nop)

    member cgbuf.EmitDebugPoint(m: range) =
        if mgbuf.cenv.options.generateDebugSymbols then

            let attr = GenILSourceMarker g m
            let i = I_seqpoint attr
            hasDebugPoints <- true

            // Replace a FeeFee seqpoint with a better debug point
            let n = codebuf.Count

            let isSingleFeeFee =
                match codebuf[n - 1] with
                | I_seqpoint sm -> (sm.Line = FeeFee mgbuf.cenv)
                | _ -> false

            if isSingleFeeFee then
                codebuf[n - 1] <- i
            else
                cgbuf.EnsureNopBetweenDebugPoints()
                codebuf.Add i

            anyDocument <- Some attr.Document

    // Emit FeeFee breakpoints for hidden code, see https://blogs.msdn.microsoft.com/jmstall/2005/06/19/line-hidden-and-0xfeefee-sequence-points/
    member cgbuf.EmitStartOfHiddenCode() =
        if mgbuf.cenv.options.generateDebugSymbols then
            let doc = g.memoize_file m.FileIndex
            let i = FeeFeeInstr mgbuf.cenv doc
            hasDebugPoints <- true

            // don't emit just after another FeeFee
            let n = codebuf.Count

            let isSingleFeeFee =
                match codebuf[n - 1] with
                | I_seqpoint sm -> (sm.Line = FeeFee mgbuf.cenv)
                | _ -> false

            if not isSingleFeeFee then
                cgbuf.EnsureNopBetweenDebugPoints()
                codebuf.Add i

    member _.EmitExceptionClause clause = exnSpecs.Add clause

    member _.GenerateDelayMark(_nm) =
        let lab = generateCodeLabel ()
        Mark lab

    member _.SetCodeLabelToCodeLabel(lab1, lab2) =
#if DEBUG
        if codeLabelToCodeLabel.ContainsKey lab1 then
            let msg =
                sprintf "two values given for label %s, methodName = %s, m = %s" (formatCodeLabel lab1) methodName (stringOfRange m)

            System.Diagnostics.Debug.Assert(false, msg)
            warning (InternalError(msg, m))
#endif
        codeLabelToCodeLabel[lab1] <- lab2

    member _.SetCodeLabelToPC(lab, pc) =
#if DEBUG
        if codeLabelToPC.ContainsKey lab then
            let msg =
                sprintf "two values given for label %s, methodName = %s, m = %s" (formatCodeLabel lab) methodName (stringOfRange m)

            System.Diagnostics.Debug.Assert(false, msg)
            warning (InternalError(msg, m))
#endif
        codeLabelToPC[lab] <- pc

    member cgbuf.SetMark(mark1: Mark, mark2: Mark) =
        cgbuf.SetCodeLabelToCodeLabel(mark1.CodeLabel, mark2.CodeLabel)

    member cgbuf.SetMarkToHere(Mark lab) =
        cgbuf.SetCodeLabelToPC(lab, codebuf.Count)

    member cgbuf.SetMarkToHereIfNecessary(inplabOpt: Mark option) =
        match inplabOpt with
        | None -> ()
        | Some inplab -> cgbuf.SetMarkToHere inplab

    member cgbuf.SetMarkOrEmitBranchIfNecessary(inplabOpt: Mark option, target: Mark) =
        match inplabOpt with
        | None -> cgbuf.EmitInstr(pop 0, Push0, I_br target.CodeLabel)
        | Some inplab -> cgbuf.SetMark(inplab, target)

    member cgbuf.SetStack s =
        stack <- s
        nstack <- s.Length

    member cgbuf.Mark s =
        let res = cgbuf.GenerateDelayMark s
        cgbuf.SetMarkToHere res
        res

    member _.mgbuf = mgbuf

    member _.MethodName = methodName

    member _.PreallocatedArgCount = alreadyUsedArgs

    member _.AllocLocal(ranges, ty, isFixed) =
        let j = locals.Count
        locals.Add((ranges, ty, isFixed))
        j

    member cgbuf.ReallocLocal(cond, ranges, ty, isFixed) =
        match ResizeArray.tryFindIndexi cond locals with
        | Some j ->
            let prevRanges, _, isFixed = locals[j]
            locals[j] <- ((ranges @ prevRanges), ty, isFixed)
            j, true
        | None -> cgbuf.AllocLocal(ranges, ty, isFixed), false

    member _.Close() =

        let instrs = codebuf.ToArray()

        // Fixup the first instruction to be a FeeFee debug point if needed
        let instrs =
            instrs
            |> Array.mapi (fun idx i2 ->
                if
                    idx = 0
                    && (match i2 with
                        | AI_nop -> true
                        | _ -> false)
                    && anyDocument.IsSome
                then
                    // This special dummy debug point says skip the start of the method
                    hasDebugPoints <- true
                    FeeFeeInstr mgbuf.cenv anyDocument.Value
                else
                    i2)

        let codeLabels =
            let dict = Dictionary.newWithSize (codeLabelToPC.Count + codeLabelToCodeLabel.Count)

            for kvp in codeLabelToPC do
                dict.Add(kvp.Key, lab2pc 0 kvp.Key)

            for kvp in codeLabelToCodeLabel do
                dict.Add(kvp.Key, lab2pc 0 kvp.Key)

            dict

        (ResizeArray.toList locals, maxStack, codeLabels, instrs, ResizeArray.toList exnSpecs, hasDebugPoints)

module CG =
    let EmitInstr (cgbuf: CodeGenBuffer) pops pushes i = cgbuf.EmitInstr(pops, pushes, i)
    let EmitInstrs (cgbuf: CodeGenBuffer) pops pushes is = cgbuf.EmitInstrs(pops, pushes, is)
    let EmitDebugPoint (cgbuf: CodeGenBuffer) m = cgbuf.EmitDebugPoint m
    let GenerateDelayMark (cgbuf: CodeGenBuffer) nm = cgbuf.GenerateDelayMark nm
    let SetMark (cgbuf: CodeGenBuffer) m1 m2 = cgbuf.SetMark(m1, m2)
    let SetMarkToHere (cgbuf: CodeGenBuffer) m1 = cgbuf.SetMarkToHere m1
    let SetStack (cgbuf: CodeGenBuffer) s = cgbuf.SetStack s
    let GenerateMark (cgbuf: CodeGenBuffer) s = cgbuf.Mark s

//--------------------------------------------------------------------------
// Compile constants
//--------------------------------------------------------------------------

let GenString cenv cgbuf s =
    CG.EmitInstr cgbuf (pop 0) (Push [ cenv.g.ilg.typ_String ]) (I_ldstr s)

let GenConstArray cenv (cgbuf: CodeGenBuffer) eenv ilElementType (data: 'a[]) (write: ByteBuffer -> 'a -> unit) =
    let g = cenv.g
    use buf = ByteBuffer.Create data.Length
    data |> Array.iter (write buf)
    let bytes = buf.AsMemory().ToArray()
    let ilArrayType = mkILArr1DTy ilElementType

    if data.Length = 0 then
        CG.EmitInstrs cgbuf (pop 0) (Push [ ilArrayType ]) [ mkLdcInt32 0; I_newarr(ILArrayShape.SingleDimensional, ilElementType) ]
    else
        let vtspec = cgbuf.mgbuf.GenerateRawDataValueType(eenv.cloc, bytes.Length)
        let ilFieldName = CompilerGeneratedName("field" + string (newUnique ()))
        let fty = ILType.Value vtspec

        let ilFieldDef =
            mkILStaticField (ilFieldName, fty, None, Some bytes, ILMemberAccess.Assembly)

        let ilFieldDef =
            ilFieldDef.With(customAttrs = mkILCustomAttrs [ g.DebuggerBrowsableNeverAttribute ])

        let fspec = mkILFieldSpecInTy (mkILTyForCompLoc eenv.cloc, ilFieldName, fty)
        CountStaticFieldDef()
        cgbuf.mgbuf.AddFieldDef(fspec.DeclaringTypeRef, ilFieldDef)

        CG.EmitInstrs
            cgbuf
            (pop 0)
            (Push [ ilArrayType; ilArrayType; g.iltyp_RuntimeFieldHandle ])
            [
                mkLdcInt32 data.Length
                I_newarr(ILArrayShape.SingleDimensional, ilElementType)
                AI_dup
                I_ldtoken(ILToken.ILField fspec)
            ]

        CG.EmitInstr cgbuf (pop 2) Push0 (mkNormalCall (mkInitializeArrayMethSpec g))

//-------------------------------------------------------------------------
// This is the main code generation routine. It is used to generate
// the bodies of methods in a couple of places
//-------------------------------------------------------------------------

let CodeGenThen (cenv: cenv) mgbuf (entryPointInfo, methodName, eenv, alreadyUsedArgs, selfArgOpt: Val option, codeGenFunction, m) =
    let cgbuf = CodeGenBuffer(m, mgbuf, methodName, alreadyUsedArgs)
    let start = CG.GenerateMark cgbuf "mstart"
    let finish = CG.GenerateDelayMark cgbuf "mfinish"
    let innerVals = entryPointInfo |> List.map (fun (v, kind) -> (v, (kind, start)))

    // When debugging, put the "this" parameter in a local that has the right name
    match selfArgOpt with
    | Some selfArg when
        selfArg.LogicalName <> "this"
        && not (selfArg.LogicalName.StartsWith("_"))
        && not cenv.options.localOptimizationsEnabled
        ->
        let ilTy = selfArg.Type |> GenType cenv m eenv.tyenv
        let idx = cgbuf.AllocLocal([ (selfArg.LogicalName, (start, finish)) ], ilTy, false)
        cgbuf.EmitStartOfHiddenCode()
        CG.EmitInstrs cgbuf (pop 0) Push0 [ mkLdarg0; I_stloc(uint16 idx) ]
    | _ -> ()

    // Call the given code generator
    codeGenFunction
        cgbuf
        { eenv with
            withinSEH = false
            liveLocals = IntMap.empty ()
            innerVals = innerVals
        }

    cgbuf.SetMarkToHere finish

    let locals, maxStack, lab2pc, code, exnSpecs, hasDebugPoints = cgbuf.Close()

    let localDebugSpecs: ILLocalDebugInfo list =
        locals
        |> List.mapi (fun i (nms, _, _isFixed) -> List.map (fun nm -> (i, nm)) nms)
        |> List.concat
        |> List.map (fun (i, (nm, (start, finish))) ->
            {
                Range = (start.CodeLabel, finish.CodeLabel)
                DebugMappings = [ { LocalIndex = i; LocalName = nm } ]
            })

    let ilLocals =
        locals
        |> List.map (fun (infos, ty, isFixed) ->
            let loc =
                // in interactive environment, attach name and range info to locals to improve debug experience
                if cenv.options.isInteractive && cenv.options.generateDebugSymbols then
                    match infos with
                    | [ (nm, (start, finish)) ] -> mkILLocal ty (Some(nm, start.CodeLabel, finish.CodeLabel))
                    // REVIEW: what do these cases represent?
                    | _ :: _
                    | [] -> mkILLocal ty None
                // if not interactive, don't bother adding this info
                else
                    mkILLocal ty None

            if isFixed then { loc with IsPinned = true } else loc)

    (ilLocals, maxStack, lab2pc, code, exnSpecs, localDebugSpecs, hasDebugPoints)

let CodeGenMethod cenv mgbuf (entryPointInfo, methodName, eenv, alreadyUsedArgs, selfArgOpt, codeGenFunction, m) =

    let locals, maxStack, lab2pc, instrs, exns, localDebugSpecs, hasDebugPoints =
        CodeGenThen cenv mgbuf (entryPointInfo, methodName, eenv, alreadyUsedArgs, selfArgOpt, codeGenFunction, m)

    let code = buildILCode methodName lab2pc instrs exns localDebugSpecs

    // Attach a source range to the method. Only do this if it has some debug points.
    let ilDebugRange =
        if hasDebugPoints then
            GenPossibleILDebugRange cenv m
        else
            None

    let ilImports = eenv.imports

    // The old union erasure phase increased maxstack by 2 since the code pushes some items, we do the same here
    let maxStack = maxStack + 2

    // Build an Abstract IL method
    let body =
        mkILMethodBody (eenv.initLocals, locals, maxStack, code, ilDebugRange, ilImports)

    instrs, body

let StartDelayedLocalScope nm cgbuf =
    let startMark = CG.GenerateDelayMark cgbuf ("start_" + nm)
    let endMark = CG.GenerateDelayMark cgbuf ("end_" + nm)
    startMark, endMark

let StartLocalScope nm cgbuf =
    let startMark = CG.GenerateMark cgbuf ("start_" + nm)
    let endMark = CG.GenerateDelayMark cgbuf ("end_" + nm)
    startMark, endMark

let LocalScope nm cgbuf (f: Mark * Mark -> 'a) : 'a =
    let _, endMark as scopeMarks = StartLocalScope nm cgbuf
    let res = f scopeMarks
    CG.SetMarkToHere cgbuf endMark
    res

let compileSequenceExpressions = true // try (System.Environment.GetEnvironmentVariable("FSHARP_COMPILED_SEQ") <> null) with _ -> false
let compileStateMachineExpressions = true // try (System.Environment.GetEnvironmentVariable("FSHARP_COMPILED_STATEMACHINES") <> null) with _ -> false

//-------------------------------------------------------------------------
// Sequence Point Logic
//-------------------------------------------------------------------------

/// Determines if any code at all will be emitted for a binding
let BindingEmitsNoCode g (b: Binding) = IsFSharpValCompiledAsMethod g b.Var

/// Determines what debug point should be emitted when generating the r.h.s of a binding.
/// For example, if the r.h.s is a lambda then no debug point is emitted.
///
/// Returns (useWholeExprRange, sequencePointForBind, sequencePointGenerationFlagForRhsOfBind)
let ComputeDebugPointForBinding g bind =
    let (TBind (_, e, spBind)) = bind

    if BindingEmitsNoCode g bind then
        false, None
    else
        match spBind, stripExpr e with
        | DebugPointAtBinding.NoneAtInvisible, _ -> false, None
        | DebugPointAtBinding.NoneAtSticky, _ -> true, None
        | DebugPointAtBinding.NoneAtDo, _ -> false, None
        | DebugPointAtBinding.NoneAtLet, _ -> false, None
        // Don't emit debug points for lambdas.
        | _,
          (Expr.Lambda _
          | Expr.TyLambda _) -> false, None
        | DebugPointAtBinding.Yes m, _ -> false, Some m

//-------------------------------------------------------------------------
// Generate expressions
//-------------------------------------------------------------------------

let rec GenExpr cenv cgbuf eenv (expr: Expr) sequel =
    cenv.stackGuard.Guard
    <| fun () ->

        GenExprAux cenv cgbuf eenv expr sequel

/// Process the debug point and check for alternative ways to generate this expression.
/// Returns 'true' if the expression was processed by alternative means.
and GenExprPreSteps (cenv: cenv) (cgbuf: CodeGenBuffer) eenv expr sequel =
    let g = cenv.g

    // Check for the '__debugPoint" construct for inlined code
    match expr with
    | Expr.Sequential ((DebugPointExpr g debugPointName) as dpExpr, codeExpr, NormalSeq, m) ->
        match cenv.namedDebugPointsForInlinedCode.TryGetValue({ Range = m; Name = debugPointName }) with
        | false, _ when debugPointName = "" -> CG.EmitDebugPoint cgbuf m
        | false, _ ->
            // printfn $"---- Unfound debug point {debugPointName} at {m}"
            // for KeyValue(k,v) in cenv.namedDebugPointsForInlinedCode do
            //     printfn $"{k.Range} , {k.Name} -> {v}"
            let others =
                [
                    for k in cenv.namedDebugPointsForInlinedCode.Keys do
                        if Range.equals m k.Range then
                            yield k.Name
                ]
                |> String.concat ","

            informationalWarning (Error(FSComp.SR.ilxGenUnknownDebugPoint (debugPointName, others), dpExpr.Range))
            CG.EmitDebugPoint cgbuf m
        | true, dp ->
            // printfn $"---- Found debug point {debugPointName} at {m} --> {dp}"
            CG.EmitDebugPoint cgbuf dp

        GenExpr cenv cgbuf eenv codeExpr sequel
        true

    | _ ->

        //ProcessDebugPointForExpr cenv cgbuf expr

        let lowering =
            if compileSequenceExpressions then
                LowerComputedCollectionExpressions.LowerComputedListOrArrayExpr cenv.tcVal g cenv.amap expr
            else
                None

        match lowering with
        | Some altExpr ->
            GenExpr cenv cgbuf eenv altExpr sequel
            true
        | None ->

            let lowering =
                if compileSequenceExpressions then
                    LowerSequenceExpressions.ConvertSequenceExprToObject g cenv.amap expr
                else
                    None

            match lowering with
            | Some info ->
                GenSequenceExpr cenv cgbuf eenv info sequel
                true
            | None ->

                match LowerStateMachineExpr cenv.g expr with
                | LoweredStateMachineResult.Lowered res ->
                    let eenv = RemoveTemplateReplacement eenv
                    checkLanguageFeatureError cenv.g.langVersion LanguageFeature.ResumableStateMachines expr.Range
                    GenStructStateMachine cenv cgbuf eenv res sequel
                    true
                | LoweredStateMachineResult.UseAlternative (msg, altExpr) ->
                    // When prepping to generate a state machine, we can remove any trace of the template struct
                    // type for the internal state of any enclosing state machine, as they do not interact. This
                    // is important if the nested state machine generates dynamic code (LoweredStateMachineResult.UseAlternative).
                    let eenv = RemoveTemplateReplacement eenv
                    checkLanguageFeatureError cenv.g.langVersion LanguageFeature.ResumableStateMachines expr.Range
                    warning (Error(FSComp.SR.reprStateMachineNotCompilable (msg), expr.Range))
                    GenExpr cenv cgbuf eenv altExpr sequel
                    true
                | LoweredStateMachineResult.NoAlternative msg ->
                    let eenv = RemoveTemplateReplacement eenv
                    checkLanguageFeatureError cenv.g.langVersion LanguageFeature.ResumableStateMachines expr.Range
                    errorR (Error(FSComp.SR.reprStateMachineNotCompilableNoAlternative (msg), expr.Range))
                    GenDefaultValue cenv cgbuf eenv (tyOfExpr cenv.g expr, expr.Range)
                    true
                | LoweredStateMachineResult.NotAStateMachine ->
                    match expr with
                    | IfUseResumableStateMachinesExpr g (_thenExpr, elseExpr) ->
                        GenExpr cenv cgbuf eenv elseExpr sequel
                        true
                    | _ -> false

and GenExprAux (cenv: cenv) (cgbuf: CodeGenBuffer) eenv expr (sequel: sequel) =
    let g = cenv.g
    let expr = stripExpr expr

    // Process the debug point and see if there's a replacement technique to process this expression
    if GenExprPreSteps cenv cgbuf eenv expr sequel then
        ()
    else

        match expr with
        // Most generation of linear expressions is implemented routinely using tailcalls and the correct sequels.
        // This is because the element of expansion happens to be the final thing generated in most cases. However
        // for large lists we have to process the linearity separately
        | Expr.Sequential _
        | Expr.Let _
        | LinearOpExpr _
        | Expr.Match _ -> GenLinearExpr cenv cgbuf eenv expr sequel false id |> ignore<FakeUnit>

        | Expr.DebugPoint (DebugPointAtLeafExpr.Yes m, innerExpr) ->
            CG.EmitDebugPoint cgbuf m
            GenExpr cenv cgbuf eenv innerExpr sequel

        | Expr.Const (c, m, ty) -> GenConstant cenv cgbuf eenv (c, m, ty) sequel

        | Expr.LetRec (binds, body, m, _) -> GenLetRec cenv cgbuf eenv (binds, body, m) sequel

        | Expr.Lambda _
        | Expr.TyLambda _ -> GenLambda cenv cgbuf eenv false [] expr sequel

        | Expr.App (Expr.Val (vref, _, m) as v, _, tyargs, [], _) when
            List.forall (isMeasureTy g) tyargs
            && (
                // inline only values that are stored in local variables
                match StorageForValRef m vref eenv with
                | ValStorage.Local _ -> true
                | _ -> false)
            ->
            // application of local type functions with type parameters = measure types and body = local value - inline the body
            GenExpr cenv cgbuf eenv v sequel

        | Expr.App (f, fty, tyargs, curriedArgs, m) -> GenApp cenv cgbuf eenv (f, fty, tyargs, curriedArgs, m) sequel

        | Expr.Val (v, _, m) -> GenGetVal cenv cgbuf eenv (v, m) sequel

        | Expr.Op (op, tyargs, args, m) ->
            match op, args, tyargs with
            | TOp.ExnConstr c, _, _ -> GenAllocExn cenv cgbuf eenv (c, args, m) sequel
            | TOp.UnionCase c, _, _ -> GenAllocUnionCase cenv cgbuf eenv (c, tyargs, args, m) sequel
            | TOp.Recd (isCtor, tcref), _, _ -> GenAllocRecd cenv cgbuf eenv isCtor (tcref, tyargs, args, m) sequel
            | TOp.AnonRecd anonInfo, _, _ -> GenAllocAnonRecd cenv cgbuf eenv (anonInfo, tyargs, args, m) sequel
            | TOp.AnonRecdGet (anonInfo, n), [ e ], _ -> GenGetAnonRecdField cenv cgbuf eenv (anonInfo, e, tyargs, n, m) sequel
            | TOp.TupleFieldGet (tupInfo, n), [ e ], _ -> GenGetTupleField cenv cgbuf eenv (tupInfo, e, tyargs, n, m) sequel
            | TOp.ExnFieldGet (ecref, n), [ e ], _ -> GenGetExnField cenv cgbuf eenv (e, ecref, n, m) sequel
            | TOp.UnionCaseFieldGet (ucref, n), [ e ], _ -> GenGetUnionCaseField cenv cgbuf eenv (e, ucref, tyargs, n, m) sequel
            | TOp.UnionCaseFieldGetAddr (ucref, n, _readonly), [ e ], _ ->
                GenGetUnionCaseFieldAddr cenv cgbuf eenv (e, ucref, tyargs, n, m) sequel
            | TOp.UnionCaseTagGet ucref, [ e ], _ -> GenGetUnionCaseTag cenv cgbuf eenv (e, ucref, tyargs, m) sequel
            | TOp.UnionCaseProof ucref, [ e ], _ -> GenUnionCaseProof cenv cgbuf eenv (e, ucref, tyargs, m) sequel
            | TOp.ExnFieldSet (ecref, n), [ e; e2 ], _ -> GenSetExnField cenv cgbuf eenv (e, ecref, n, e2, m) sequel
            | TOp.UnionCaseFieldSet (ucref, n), [ e; e2 ], _ -> GenSetUnionCaseField cenv cgbuf eenv (e, ucref, tyargs, n, e2, m) sequel
            | TOp.ValFieldGet f, [ e ], _ -> GenGetRecdField cenv cgbuf eenv (e, f, tyargs, m) sequel
            | TOp.ValFieldGet f, [], _ -> GenGetStaticField cenv cgbuf eenv (f, tyargs, m) sequel
            | TOp.ValFieldGetAddr (f, _readonly), [ e ], _ -> GenGetRecdFieldAddr cenv cgbuf eenv (e, f, tyargs, m) sequel
            | TOp.ValFieldGetAddr (f, _readonly), [], _ -> GenGetStaticFieldAddr cenv cgbuf eenv (f, tyargs, m) sequel
            | TOp.ValFieldSet f, [ e1; e2 ], _ -> GenSetRecdField cenv cgbuf eenv (e1, f, tyargs, e2, m) sequel
            | TOp.ValFieldSet f, [ e2 ], _ -> GenSetStaticField cenv cgbuf eenv (f, tyargs, e2, m) sequel
            | TOp.Tuple tupInfo, _, _ -> GenAllocTuple cenv cgbuf eenv (tupInfo, args, tyargs, m) sequel
            | TOp.ILAsm (instrs, retTypes), _, _ -> GenAsmCode cenv cgbuf eenv (instrs, tyargs, args, retTypes, m) sequel
            | TOp.While (sp, _), [ Expr.Lambda (_, _, _, [ _ ], e1, _, _); Expr.Lambda (_, _, _, [ _ ], e2, _, _) ], [] ->
                GenWhileLoop cenv cgbuf eenv (sp, e1, e2, m) sequel
            | TOp.IntegerForLoop (spFor, spTo, dir),
              [ Expr.Lambda (_, _, _, [ _ ], e1, _, _); Expr.Lambda (_, _, _, [ _ ], e2, _, _); Expr.Lambda (_, _, _, [ v ], e3, _, _) ],
              [] -> GenIntegerForLoop cenv cgbuf eenv (spFor, spTo, v, e1, dir, e2, e3, m) sequel
            | TOp.TryFinally (spTry, spFinally),
              [ Expr.Lambda (_, _, _, [ _ ], e1, _, _); Expr.Lambda (_, _, _, [ _ ], e2, _, _) ],
              [ resTy ] -> GenTryFinally cenv cgbuf eenv (e1, e2, m, resTy, spTry, spFinally) sequel
            | TOp.TryWith (spTry, spWith),
              [ Expr.Lambda (_, _, _, [ _ ], e1, _, _); Expr.Lambda (_, _, _, [ vf ], ef, _, _); Expr.Lambda (_, _, _, [ vh ], eh, _, _) ],
              [ resTy ] -> GenTryWith cenv cgbuf eenv (e1, vf, ef, vh, eh, m, resTy, spTry, spWith) sequel
            | TOp.ILCall (isVirtual, _, isStruct, isCtor, valUseFlag, _, noTailCall, ilMethRef, enclTypeInst, methInst, returnTypes),
              args,
              [] ->
                GenILCall
                    cenv
                    cgbuf
                    eenv
                    (isVirtual, isStruct, isCtor, valUseFlag, noTailCall, ilMethRef, enclTypeInst, methInst, args, returnTypes, m)
                    sequel
            | TOp.RefAddrGet _readonly, [ e ], [ ty ] -> GenGetAddrOfRefCellField cenv cgbuf eenv (e, ty, m) sequel
            | TOp.Coerce, [ e ], [ tgtTy; srcTy ] -> GenCoerce cenv cgbuf eenv (e, tgtTy, m, srcTy) sequel
            | TOp.Reraise, [], [ retTy ] -> GenReraise cenv cgbuf eenv (retTy, m) sequel
            | TOp.TraitCall traitInfo, args, [] -> GenTraitCall cenv cgbuf eenv (traitInfo, args, m) expr sequel
            | TOp.LValueOp (LSet, v), [ e ], [] -> GenSetVal cenv cgbuf eenv (v, e, m) sequel
            | TOp.LValueOp (LByrefGet, v), [], [] -> GenGetByref cenv cgbuf eenv (v, m) sequel
            | TOp.LValueOp (LByrefSet, v), [ e ], [] -> GenSetByref cenv cgbuf eenv (v, e, m) sequel
            | TOp.LValueOp (LAddrOf _, v), [], [] -> GenGetValAddr cenv cgbuf eenv (v, m) sequel
            | TOp.Array, elems, [ elemTy ] -> GenNewArray cenv cgbuf eenv (elems, elemTy, m) sequel
            | TOp.Bytes bytes, [], [] ->
                if cenv.options.emitConstantArraysUsingStaticDataBlobs then
                    GenConstArray cenv cgbuf eenv g.ilg.typ_Byte bytes (fun buf b -> buf.EmitByte b)
                    GenSequel cenv eenv.cloc cgbuf sequel
                else
                    GenNewArraySimple cenv cgbuf eenv (List.ofArray (Array.map (mkByte g m) bytes), g.byte_ty, m) sequel
            | TOp.UInt16s arr, [], [] ->
                if cenv.options.emitConstantArraysUsingStaticDataBlobs then
                    GenConstArray cenv cgbuf eenv g.ilg.typ_UInt16 arr (fun buf b -> buf.EmitUInt16 b)
                    GenSequel cenv eenv.cloc cgbuf sequel
                else
                    GenNewArraySimple cenv cgbuf eenv (List.ofArray (Array.map (mkUInt16 g m) arr), g.uint16_ty, m) sequel
            | TOp.Goto label, _, _ ->
                if cgbuf.mgbuf.cenv.options.generateDebugSymbols then
                    cgbuf.EmitStartOfHiddenCode()
                    CG.EmitInstr cgbuf (pop 0) Push0 AI_nop

                CG.EmitInstr cgbuf (pop 0) Push0 (I_br label)
            // NOTE: discard sequel
            | TOp.Return, [ e ], _ -> GenExpr cenv cgbuf eenv e eenv.exitSequel
            // NOTE: discard sequel
            | TOp.Return, [], _ -> GenSequel cenv eenv.cloc cgbuf ReturnVoid
            // NOTE: discard sequel
            | TOp.Label label, _, _ ->
                cgbuf.SetMarkToHere(Mark label)
                GenUnitThenSequel cenv eenv m eenv.cloc cgbuf sequel
            | _ -> error (InternalError("Unexpected operator node expression", expr.Range))

        | Expr.StaticOptimization (constraints, e2, e3, m) -> GenStaticOptimization cenv cgbuf eenv (constraints, e2, e3, m) sequel

        | Expr.Obj (_, ty, _, _, [ meth ], [], m) when isDelegateTy g ty -> GenDelegateExpr cenv cgbuf eenv expr (meth, m) sequel

        | Expr.Obj (_, ty, basev, basecall, overrides, interfaceImpls, m) ->
            GenObjectExpr cenv cgbuf eenv expr (ty, basev, basecall, overrides, interfaceImpls, m) sequel

        | Expr.Quote (ast, conv, _, m, ty) -> GenQuotation cenv cgbuf eenv (ast, conv, m, ty) sequel

        | Expr.WitnessArg (traitInfo, m) ->
            GenWitnessArgFromTraitInfo cenv cgbuf eenv m traitInfo
            GenSequel cenv eenv.cloc cgbuf sequel

        | Expr.Link _ -> failwith "Unexpected reclink"

        | Expr.TyChoose (_, _, m) -> error (InternalError("Unexpected Expr.TyChoose", m))

and GenExprs cenv cgbuf eenv es =
    List.iter (fun e -> GenExpr cenv cgbuf eenv e Continue) es

and CodeGenMethodForExpr cenv mgbuf (entryPointInfo, methodName, eenv, alreadyUsedArgs, selfArgOpt, expr0, sequel0) =
    let eenv = { eenv with exitSequel = sequel0 }

    let _, code =
        CodeGenMethod
            cenv
            mgbuf
            (entryPointInfo,
             methodName,
             eenv,
             alreadyUsedArgs,
             selfArgOpt,
             (fun cgbuf eenv -> GenExpr cenv cgbuf eenv expr0 sequel0),
             expr0.Range)

    code

//--------------------------------------------------------------------------
// Generate sequels
//--------------------------------------------------------------------------

/// Adjust the sequel for an implicit discard (e.g. a discard that occurs by
/// not generating a 'unit' expression at all)
and sequelAfterDiscard sequel =
    match sequel with
    | LeaveHandler (isFinally, whereToSaveResultOpt, afterHandler, true) ->
        // If we're not saving the result as we leave a handler and we're doing a discard
        // then we can just adjust the sequel to record the fact we've implicitly done a discard
        if isFinally || whereToSaveResultOpt.IsNone then
            Some(LeaveHandler(isFinally, whereToSaveResultOpt, afterHandler, false))
        else
            None
    | DiscardThen sequel -> Some sequel
    | EndLocalScope (sq, mark) -> sequelAfterDiscard sq |> Option.map (fun sq -> EndLocalScope(sq, mark))
    | _ -> None

and sequelIgnoringEndScopesAndDiscard sequel =
    let sequel = sequelIgnoreEndScopes sequel

    match sequelAfterDiscard sequel with
    | Some sq -> sq
    | None -> sequel

and sequelIgnoreEndScopes sequel =
    match sequel with
    | EndLocalScope (sq, _) -> sequelIgnoreEndScopes sq
    | sq -> sq

(* commit any 'EndLocalScope' nodes in the sequel and return the residue *)
and GenSequelEndScopes cgbuf sequel =
    match sequel with
    | EndLocalScope (sq, m) ->
        CG.SetMarkToHere cgbuf m
        GenSequelEndScopes cgbuf sq
    | _ -> ()

and StringOfSequel sequel =
    match sequel with
    | Continue -> "continue"
    | DiscardThen sequel -> "discard; " + StringOfSequel sequel
    | ReturnVoid -> "ReturnVoid"
    | CmpThenBrOrContinue _ -> "CmpThenBrOrContinue"
    | Return -> "Return"
    | EndLocalScope (sq, Mark k) -> "EndLocalScope(" + StringOfSequel sq + "," + formatCodeLabel k + ")"
    | Br (Mark x) -> sprintf "Br L%s" (formatCodeLabel x)
    | LeaveHandler _ -> "LeaveHandler"
    | EndFilter -> "EndFilter"

and GenSequel cenv cloc cgbuf sequel =
    let sq = sequelIgnoreEndScopes sequel

    (match sq with
     | Continue -> ()
     | DiscardThen sq ->
         CG.EmitInstr cgbuf (pop 1) Push0 AI_pop
         GenSequel cenv cloc cgbuf sq
     | ReturnVoid -> CG.EmitInstr cgbuf (pop 0) Push0 I_ret
     | CmpThenBrOrContinue (pops, bri) -> CG.EmitInstrs cgbuf pops Push0 bri
     | Return -> CG.EmitInstr cgbuf (pop 1) Push0 I_ret
     | EndLocalScope _ -> failwith "EndLocalScope unexpected"
     | Br x ->
         // Emit a NOP in debug code in case the branch instruction gets eliminated
         // because it is a "branch to next instruction". This prevents two unrelated debug points
         // (the one before the branch and the one after) being coalesced together
         if cgbuf.mgbuf.cenv.options.generateDebugSymbols then
             cgbuf.EmitStartOfHiddenCode()
             CG.EmitInstr cgbuf (pop 0) Push0 AI_nop

         CG.EmitInstr cgbuf (pop 0) Push0 (I_br x.CodeLabel)

     | LeaveHandler (isFinally, whereToSaveResultOpt, afterHandler, hasResult) ->
         if hasResult then
             if isFinally then
                 CG.EmitInstr cgbuf (pop 1) Push0 AI_pop
             else
                 match whereToSaveResultOpt with
                 | None -> CG.EmitInstr cgbuf (pop 1) Push0 AI_pop
                 | Some (whereToSaveResult, _) -> EmitSetLocal cgbuf whereToSaveResult

         CG.EmitInstr
             cgbuf
             (pop 0)
             Push0
             (if isFinally then
                  I_endfinally
              else
                  I_leave(afterHandler.CodeLabel))

     | EndFilter -> CG.EmitInstr cgbuf (pop 1) Push0 I_endfilter)

    GenSequelEndScopes cgbuf sequel

//--------------------------------------------------------------------------
// Generate constants
//--------------------------------------------------------------------------

and GenConstant cenv cgbuf eenv (c, m, ty) sequel =
    let g = cenv.g
    let ilTy = GenType cenv m eenv.tyenv ty
    // Check if we need to generate the value at all
    match sequelAfterDiscard sequel with
    | None ->
        match TryEliminateDesugaredConstants g m c with
        | Some e -> GenExpr cenv cgbuf eenv e Continue
        | None ->
            let emitInt64Constant i =
                // see https://github.com/dotnet/fsharp/pull/3620
                // and https://github.com/dotnet/fsharp/issue/8683
                // and https://github.com/dotnet/roslyn/blob/98f12bb/src/Compilers/Core/Portable/CodeGen/ILBuilderEmit.cs#L679
                if i >= int64 System.Int32.MinValue && i <= int64 System.Int32.MaxValue then
                    CG.EmitInstrs cgbuf (pop 0) (Push [ ilTy ]) [ mkLdcInt32 (int32 i); AI_conv DT_I8 ]
                elif i >= int64 System.UInt32.MinValue && i <= int64 System.UInt32.MaxValue then
                    CG.EmitInstrs cgbuf (pop 0) (Push [ ilTy ]) [ mkLdcInt32 (int32 i); AI_conv DT_U8 ]
                else
                    CG.EmitInstr cgbuf (pop 0) (Push [ ilTy ]) (iLdcInt64 i)

            match c with
            | Const.Bool b -> CG.EmitInstr cgbuf (pop 0) (Push [ g.ilg.typ_Bool ]) (mkLdcInt32 (if b then 1 else 0))
            | Const.SByte i -> CG.EmitInstr cgbuf (pop 0) (Push [ ilTy ]) (mkLdcInt32 (int32 i))
            | Const.Int16 i -> CG.EmitInstr cgbuf (pop 0) (Push [ ilTy ]) (mkLdcInt32 (int32 i))
            | Const.Int32 i -> CG.EmitInstr cgbuf (pop 0) (Push [ ilTy ]) (mkLdcInt32 i)
            | Const.Int64 i -> emitInt64Constant i
            | Const.IntPtr i -> CG.EmitInstrs cgbuf (pop 0) (Push [ ilTy ]) [ iLdcInt64 i; AI_conv DT_I ]
            | Const.Byte i -> CG.EmitInstr cgbuf (pop 0) (Push [ ilTy ]) (mkLdcInt32 (int32 i))
            | Const.UInt16 i -> CG.EmitInstr cgbuf (pop 0) (Push [ ilTy ]) (mkLdcInt32 (int32 i))
            | Const.UInt32 i -> CG.EmitInstr cgbuf (pop 0) (Push [ ilTy ]) (mkLdcInt32 (int32 i))
            | Const.UInt64 i -> emitInt64Constant (int64 i)
            | Const.UIntPtr i -> CG.EmitInstrs cgbuf (pop 0) (Push [ ilTy ]) [ iLdcInt64 (int64 i); AI_conv DT_U ]
            | Const.Double f -> CG.EmitInstr cgbuf (pop 0) (Push [ ilTy ]) (AI_ldc(DT_R8, ILConst.R8 f))
            | Const.Single f -> CG.EmitInstr cgbuf (pop 0) (Push [ ilTy ]) (AI_ldc(DT_R4, ILConst.R4 f))
            | Const.Char c -> CG.EmitInstr cgbuf (pop 0) (Push [ ilTy ]) (mkLdcInt32 (int c))
            | Const.String s -> GenString cenv cgbuf s
            | Const.Unit -> GenUnit cenv eenv m cgbuf
            | Const.Zero -> GenDefaultValue cenv cgbuf eenv (ty, m)
            | Const.Decimal _ -> failwith "unreachable"

        GenSequel cenv eenv.cloc cgbuf sequel
    | Some sq ->
        // Even if we didn't need to generate the value then maybe we still have to branch or return
        GenSequel cenv eenv.cloc cgbuf sq

and GenUnitTy cenv eenv m =
    match cenv.ilUnitTy with
    | None ->
        let res = GenType cenv m eenv.tyenv cenv.g.unit_ty
        cenv.ilUnitTy <- Some res
        res
    | Some res -> res

and GenUnit cenv eenv m cgbuf =
    CG.EmitInstr cgbuf (pop 0) (Push [ GenUnitTy cenv eenv m ]) AI_ldnull

and GenUnitThenSequel cenv eenv m cloc cgbuf sequel =
    match sequelAfterDiscard sequel with
    | Some sq -> GenSequel cenv cloc cgbuf sq
    | None ->
        GenUnit cenv eenv m cgbuf
        GenSequel cenv cloc cgbuf sequel

//--------------------------------------------------------------------------
// Generate simple data-related constructs
//--------------------------------------------------------------------------

and GenAllocTuple cenv cgbuf eenv (tupInfo, args, argTys, m) sequel =

    let tupInfo = evalTupInfoIsStruct tupInfo
    let tcref, tys, args, newm = mkCompiledTuple cenv.g tupInfo (argTys, args, m)
    let ty = GenNamedTyApp cenv newm eenv.tyenv tcref tys

    let ntyvars =
        if (tys.Length - 1) < goodTupleFields then
            (tys.Length - 1)
        else
            goodTupleFields

    let formalTyvars =
        [
            for n in 0..ntyvars do
                yield mkILTyvarTy (uint16 n)
        ]

    GenExprs cenv cgbuf eenv args
    // Generate a reference to the constructor
    CG.EmitInstr cgbuf (pop args.Length) (Push [ ty ]) (mkNormalNewobj (mkILCtorMethSpecForTy (ty, formalTyvars)))
    GenSequel cenv eenv.cloc cgbuf sequel

and GenGetTupleField cenv cgbuf eenv (tupInfo, e, tys, n, m) sequel =
    let tupInfo = evalTupInfoIsStruct tupInfo

    let rec getCompiledTupleItem g (e, tys: TTypes, n, m) =
        let ar = tys.Length

        if ar <= 0 then
            failwith "getCompiledTupleItem"
        elif ar < maxTuple then
            let tcref = mkCompiledTupleTyconRef g tupInfo ar
            let ty = GenNamedTyApp cenv m eenv.tyenv tcref tys
            mkGetTupleItemN g m n ty tupInfo e tys[n]
        else
            let tysA, tysB = List.splitAfter goodTupleFields tys
            let tyB = mkCompiledTupleTy g tupInfo tysB
            let tysC = tysA @ [ tyB ]
            let tcref = mkCompiledTupleTyconRef g tupInfo (List.length tysC)
            let tyR = GenNamedTyApp cenv m eenv.tyenv tcref tysC
            let nR = min n goodTupleFields
            let elast = mkGetTupleItemN g m nR tyR tupInfo e tysC[nR]

            if n < goodTupleFields then
                elast
            else
                getCompiledTupleItem g (elast, tysB, n - goodTupleFields, m)

    GenExpr cenv cgbuf eenv (getCompiledTupleItem cenv.g (e, tys, n, m)) sequel

and GenAllocExn cenv cgbuf eenv (c, args, m) sequel =
    GenExprs cenv cgbuf eenv args
    let ty = GenExnType cenv m eenv.tyenv c
    let flds = recdFieldsOfExnDefRef c

    let argTys =
        flds |> List.map (fun rfld -> GenType cenv m eenv.tyenv rfld.FormalType)

    let mspec = mkILCtorMethSpecForTy (ty, argTys)
    CG.EmitInstr cgbuf (pop args.Length) (Push [ ty ]) (mkNormalNewobj mspec)
    GenSequel cenv eenv.cloc cgbuf sequel

and GenAllocUnionCaseCore cenv cgbuf eenv (c, tyargs, n, m) =
    let cuspec, idx = GenUnionCaseSpec cenv m eenv.tyenv c tyargs
    CG.EmitInstrs cgbuf (pop n) (Push [ cuspec.DeclaringType ]) (EraseUnions.mkNewData cenv.g.ilg (cuspec, idx))

and GenAllocUnionCase cenv cgbuf eenv (c, tyargs, args, m) sequel =
    GenExprs cenv cgbuf eenv args
    GenAllocUnionCaseCore cenv cgbuf eenv (c, tyargs, args.Length, m)
    GenSequel cenv eenv.cloc cgbuf sequel

and GenLinearExpr cenv cgbuf eenv expr sequel preSteps (contf: FakeUnit -> FakeUnit) =
    let expr = stripExpr expr

    match expr with
    | Expr.Sequential (e1, e2, specialSeqFlag, _) ->
        // Process the debug point and see if there's a replacement technique to process this expression
        if preSteps && GenExprPreSteps cenv cgbuf eenv expr sequel then
            contf Fake
        else

            match specialSeqFlag with
            | NormalSeq ->
                GenExpr cenv cgbuf eenv e1 discard
                GenLinearExpr cenv cgbuf eenv e2 sequel true contf
            | ThenDoSeq ->
                // "e then ()" with DebugPointAtSequential.SuppressStmt is used
                // in mkDebugPoint to emit a debug point on "e".  However we don't want this to interfere
                // with tailcalls, so detect this case and throw the "then ()" away, having already
                // worked out "spExpr" up above.
                match e2 with
                | Expr.Const (Const.Unit, _, _) -> GenExpr cenv cgbuf eenv e1 sequel
                | _ ->
                    let g = cenv.g
                    let isUnit = isUnitTy g (tyOfExpr g e1)

                    if isUnit then
                        GenExpr cenv cgbuf eenv e1 discard
                        GenExpr cenv cgbuf eenv e2 discard
                        GenUnitThenSequel cenv eenv e2.Range eenv.cloc cgbuf sequel
                    else
                        GenExpr cenv cgbuf eenv e1 Continue
                        GenExpr cenv cgbuf eenv e2 discard
                        GenSequel cenv eenv.cloc cgbuf sequel

                contf Fake

    | Expr.Let (bind, body, _, _) ->
        // Process the debug point and see if there's a replacement technique to process this expression
        if preSteps && GenExprPreSteps cenv cgbuf eenv expr sequel then
            contf Fake
        else

            // This case implemented here to get a guaranteed tailcall
            // Make sure we generate the debug point outside the scope of the variable
            let startMark, endMark as scopeMarks = StartDelayedLocalScope "let" cgbuf
            let eenv = AllocStorageForBind cenv cgbuf scopeMarks eenv bind
            GenDebugPointForBind cenv cgbuf bind
            GenBindingAfterDebugPoint cenv cgbuf eenv bind false (Some startMark)

            // Generate the body
            GenLinearExpr cenv cgbuf eenv body (EndLocalScope(sequel, endMark)) true contf

    | Expr.Match (spBind, _exprm, tree, targets, m, ty) ->
        // Process the debug point and see if there's a replacement technique to process this expression
        if preSteps && GenExprPreSteps cenv cgbuf eenv expr sequel then
            contf Fake
        else

            match spBind with
            | DebugPointAtBinding.Yes m -> CG.EmitDebugPoint cgbuf m
            | DebugPointAtBinding.NoneAtDo
            | DebugPointAtBinding.NoneAtLet
            | DebugPointAtBinding.NoneAtInvisible
            | DebugPointAtBinding.NoneAtSticky -> ()

            // First try the common cases where we don't need a join point.
            match tree with
            | TDSuccess _ ->
                failwith "internal error: matches that immediately succeed should have been normalized using mkAndSimplifyMatch"

            | _ ->
                // Create a join point
                let stackAtTargets = cgbuf.GetCurrentStack() // the stack at the target of each clause

                let sequelOnBranches, afterJoin, stackAfterJoin, sequelAfterJoin =
                    GenJoinPoint cenv cgbuf "match" eenv ty m sequel

                // Stack: "stackAtTargets" is "stack prior to any match-testing" and also "stack at the start of each branch-RHS".
                //        match-testing (dtrees) should not contribute to the stack.
                //        Each branch-RHS (targets) may contribute to the stack, leaving it in the "stackAfterJoin" state, for the join point.
                //        Since code is branching and joining, the cgbuf stack is maintained manually.
                GenDecisionTreeAndTargets
                    cenv
                    cgbuf
                    stackAtTargets
                    eenv
                    tree
                    targets
                    sequelOnBranches
                    (contf
                     << (fun Fake ->
                         CG.SetMarkToHere cgbuf afterJoin

                         //assert(cgbuf.GetCurrentStack() = stackAfterJoin)  // REVIEW: Since gen_dtree* now sets stack, stack should be stackAfterJoin at this point...
                         CG.SetStack cgbuf stackAfterJoin
                         // If any values are left on the stack after the join then we're certainly going to do something with them
                         // For example, we may be about to execute a 'stloc' for
                         //
                         //   let y2 = if System.DateTime.Now.Year < 2000 then 1 else 2
                         //
                         // or a 'stelem' for
                         //
                         //   arr.[0] <- if System.DateTime.Now.Year > 2000 then 1 else 2
                         //
                         // In both cases, any instructions that come after this point will be falsely associated with the last branch of the control
                         // prior to the join point. This is base, e.g. see FSharp 1.0 bug 5155
                         if not (isNil stackAfterJoin) then
                             cgbuf.EmitStartOfHiddenCode()

                         GenSequel cenv eenv.cloc cgbuf sequelAfterJoin
                         Fake))

    | Expr.DebugPoint (DebugPointAtLeafExpr.Yes m, innerExpr) ->
        CG.EmitDebugPoint cgbuf m
        GenLinearExpr cenv cgbuf eenv innerExpr sequel true contf

    | LinearOpExpr (TOp.UnionCase c, tyargs, argsFront, argLast, m) ->
        // Process the debug point and see if there's a replacement technique to process this expression
        if preSteps && GenExprPreSteps cenv cgbuf eenv expr sequel then
            contf Fake
        else

            GenExprs cenv cgbuf eenv argsFront

            GenLinearExpr
                cenv
                cgbuf
                eenv
                argLast
                Continue
                true
                (contf
                 << (fun Fake ->
                     GenAllocUnionCaseCore cenv cgbuf eenv (c, tyargs, argsFront.Length + 1, m)
                     GenSequel cenv eenv.cloc cgbuf sequel
                     Fake))

    | _ ->
        GenExpr cenv cgbuf eenv expr sequel
        contf Fake

and GenAllocRecd cenv cgbuf eenv ctorInfo (tcref, argTys, args, m) sequel =
    let ty = GenNamedTyApp cenv m eenv.tyenv tcref argTys

    // Filter out fields with default initialization
    let relevantFields =
        tcref.AllInstanceFieldsAsList
        |> List.filter (fun f -> not f.IsZeroInit)
        |> List.filter (fun f -> not f.IsCompilerGenerated)

    match ctorInfo with
    | RecdExprIsObjInit ->
        (args, relevantFields)
        ||> List.iter2 (fun e f ->
            CG.EmitInstr
                cgbuf
                (pop 0)
                (Push(
                    if tcref.IsStructOrEnumTycon then
                        [ ILType.Byref ty ]
                    else
                        [ ty ]
                ))
                mkLdarg0

            GenExpr cenv cgbuf eenv e Continue
            GenFieldStore false cenv cgbuf eenv (tcref.MakeNestedRecdFieldRef f, argTys, m) discard)
        // Object construction doesn't generate a true value.
        // Object constructions will always just get thrown away so this is safe
        GenSequel cenv eenv.cloc cgbuf sequel
    | RecdExpr ->
        GenExprs cenv cgbuf eenv args
        // generate a reference to the record constructor
        let tyenvinner = eenv.tyenv.ForTyconRef tcref

        CG.EmitInstr
            cgbuf
            (pop args.Length)
            (Push [ ty ])
            (mkNormalNewobj (mkILCtorMethSpecForTy (ty, relevantFields |> List.map (fun f -> GenType cenv m tyenvinner f.FormalType))))

        GenSequel cenv eenv.cloc cgbuf sequel

and GenAllocAnonRecd cenv cgbuf eenv (anonInfo: AnonRecdTypeInfo, tyargs, args, m) sequel =
    let anonCtor, _anonMethods, anonType =
        cgbuf.mgbuf.LookupAnonType((fun ilThisTy -> GenToStringMethod cenv eenv ilThisTy m), anonInfo)

    let boxity = anonType.Boxity
    GenExprs cenv cgbuf eenv args
    let ilTypeArgs = GenTypeArgs cenv m eenv.tyenv tyargs

    let anonTypeWithInst =
        mkILTy boxity (mkILTySpec (anonType.TypeSpec.TypeRef, ilTypeArgs))

    CG.EmitInstr cgbuf (pop args.Length) (Push [ anonTypeWithInst ]) (mkNormalNewobj (mkILMethSpec (anonCtor, boxity, ilTypeArgs, [])))
    GenSequel cenv eenv.cloc cgbuf sequel

and GenGetAnonRecdField cenv cgbuf eenv (anonInfo: AnonRecdTypeInfo, e, tyargs, n, m) sequel =
    let _anonCtor, anonMethods, anonType =
        cgbuf.mgbuf.LookupAnonType((fun ilThisTy -> GenToStringMethod cenv eenv ilThisTy m), anonInfo)

    let boxity = anonType.Boxity
    let ilTypeArgs = GenTypeArgs cenv m eenv.tyenv tyargs
    let anonMethod = anonMethods[n]
    let anonFieldType = ilTypeArgs[n]
    GenExpr cenv cgbuf eenv e Continue
    CG.EmitInstr cgbuf (pop 1) (Push [ anonFieldType ]) (mkNormalCall (mkILMethSpec (anonMethod, boxity, ilTypeArgs, [])))
    GenSequel cenv eenv.cloc cgbuf sequel

and GenNewArraySimple cenv cgbuf eenv (elems, elemTy, m) sequel =
    let ilElemTy = GenType cenv m eenv.tyenv elemTy
    let ilArrTy = mkILArr1DTy ilElemTy

    if List.isEmpty elems && cenv.g.isArrayEmptyAvailable then
        mkNormalCall (
            mkILMethSpecInTy (cenv.g.ilg.typ_Array, ILCallingConv.Static, "Empty", [], mkILArr1DTy (mkILTyvarTy 0us), [ ilElemTy ])
        )
        |> CG.EmitInstr cgbuf (pop 0) (Push [ ilArrTy ])
    else
        CG.EmitInstrs
            cgbuf
            (pop 0)
            (Push [ ilArrTy ])
            [
                (AI_ldc(DT_I4, ILConst.I4 elems.Length))
                I_newarr(ILArrayShape.SingleDimensional, ilElemTy)
            ]

        elems
        |> List.iteri (fun i e ->
            CG.EmitInstrs cgbuf (pop 0) (Push [ ilArrTy; cenv.g.ilg.typ_Int32 ]) [ AI_dup; (AI_ldc(DT_I4, ILConst.I4 i)) ]
            GenExpr cenv cgbuf eenv e Continue
            CG.EmitInstr cgbuf (pop 3) Push0 (I_stelem_any(ILArrayShape.SingleDimensional, ilElemTy)))

    GenSequel cenv eenv.cloc cgbuf sequel

and GenNewArray cenv cgbuf eenv (elems: Expr list, elemTy, m) sequel =
    // REVIEW: The restriction against enum types here has to do with Dev10/Dev11 bug 872799
    // GenConstArray generates a call to RuntimeHelpers.InitializeArray. On CLR 2.0/x64 and CLR 4.0/x64/x86,
    // InitializeArray is a JIT intrinsic that will result in invalid runtime CodeGen when initializing an array
    // of enum types. Until bug 872799 is fixed, we'll need to generate arrays the "simple" way for enum types
    // Also note - C# never uses InitializeArray for enum types, so this change puts us on equal footing with them.
    if
        elems.Length <= 5
        || not cenv.options.emitConstantArraysUsingStaticDataBlobs
        || (isEnumTy cenv.g elemTy)
    then
        GenNewArraySimple cenv cgbuf eenv (elems, elemTy, m) sequel
    else
        // Try to emit a constant byte-blob array
        let elemsArray = Array.ofList elems

        let test, write =
            match stripDebugPoints elemsArray[0] with
            | Expr.Const (Const.Bool _, _, _) ->
                (function
                | Const.Bool _ -> true
                | _ -> false),
                (fun (buf: ByteBuffer) ->
                    function
                    | Const.Bool b -> buf.EmitBoolAsByte b
                    | _ -> failwith "unreachable")
            | Expr.Const (Const.Char _, _, _) ->
                (function
                | Const.Char _ -> true
                | _ -> false),
                (fun buf ->
                    function
                    | Const.Char b -> buf.EmitInt32AsUInt16(int b)
                    | _ -> failwith "unreachable")
            | Expr.Const (Const.Byte _, _, _) ->
                (function
                | Const.Byte _ -> true
                | _ -> false),
                (fun buf ->
                    function
                    | Const.Byte b -> buf.EmitByte b
                    | _ -> failwith "unreachable")
            | Expr.Const (Const.UInt16 _, _, _) ->
                (function
                | Const.UInt16 _ -> true
                | _ -> false),
                (fun buf ->
                    function
                    | Const.UInt16 b -> buf.EmitUInt16 b
                    | _ -> failwith "unreachable")
            | Expr.Const (Const.UInt32 _, _, _) ->
                (function
                | Const.UInt32 _ -> true
                | _ -> false),
                (fun buf ->
                    function
                    | Const.UInt32 b -> buf.EmitInt32(int32 b)
                    | _ -> failwith "unreachable")
            | Expr.Const (Const.UInt64 _, _, _) ->
                (function
                | Const.UInt64 _ -> true
                | _ -> false),
                (fun buf ->
                    function
                    | Const.UInt64 b -> buf.EmitInt64(int64 b)
                    | _ -> failwith "unreachable")
            | Expr.Const (Const.SByte _, _, _) ->
                (function
                | Const.SByte _ -> true
                | _ -> false),
                (fun buf ->
                    function
                    | Const.SByte b -> buf.EmitByte(byte b)
                    | _ -> failwith "unreachable")
            | Expr.Const (Const.Int16 _, _, _) ->
                (function
                | Const.Int16 _ -> true
                | _ -> false),
                (fun buf ->
                    function
                    | Const.Int16 b -> buf.EmitUInt16(uint16 b)
                    | _ -> failwith "unreachable")
            | Expr.Const (Const.Int32 _, _, _) ->
                (function
                | Const.Int32 _ -> true
                | _ -> false),
                (fun buf ->
                    function
                    | Const.Int32 b -> buf.EmitInt32 b
                    | _ -> failwith "unreachable")
            | Expr.Const (Const.Int64 _, _, _) ->
                (function
                | Const.Int64 _ -> true
                | _ -> false),
                (fun buf ->
                    function
                    | Const.Int64 b -> buf.EmitInt64 b
                    | _ -> failwith "unreachable")
            | _ ->
                (function
                | _ -> false),
                (fun _ _ -> failwith "unreachable")

        if
            elemsArray
            |> Array.forall (function
                | Expr.Const (c, _, _) -> test c
                | _ -> false)
        then
            let ilElemTy = GenType cenv m eenv.tyenv elemTy

            GenConstArray cenv cgbuf eenv ilElemTy elemsArray (fun buf ->
                function
                | Expr.Const (c, _, _) -> write buf c
                | _ -> failwith "unreachable")

            GenSequel cenv eenv.cloc cgbuf sequel

        else
            GenNewArraySimple cenv cgbuf eenv (elems, elemTy, m) sequel

and GenCoerce cenv cgbuf eenv (e, tgtTy, m, srcTy) sequel =
    let g = cenv.g
    // Is this an upcast?
    if
        TypeDefinitelySubsumesTypeNoCoercion 0 g cenv.amap m tgtTy srcTy
        &&
        // Do an extra check - should not be needed
        TypeFeasiblySubsumesType 0 g cenv.amap m tgtTy NoCoerce srcTy
    then
        if isInterfaceTy g tgtTy then
            GenExpr cenv cgbuf eenv e Continue
            let ilToTy = GenType cenv m eenv.tyenv tgtTy
            // Section "III.1.8.1.3 Merging stack states" of ECMA-335 implies that no unboxing
            // is required, but we still push the coerced type on to the code gen buffer.
            CG.EmitInstrs cgbuf (pop 1) (Push [ ilToTy ]) []
            GenSequel cenv eenv.cloc cgbuf sequel
        else
            GenExpr cenv cgbuf eenv e sequel
    else
        GenExpr cenv cgbuf eenv e Continue

        if not (isObjTy g srcTy) then
            let ilFromTy = GenType cenv m eenv.tyenv srcTy
            CG.EmitInstr cgbuf (pop 1) (Push [ g.ilg.typ_Object ]) (I_box ilFromTy)

        if not (isObjTy g tgtTy) then
            let ilToTy = GenType cenv m eenv.tyenv tgtTy
            CG.EmitInstr cgbuf (pop 1) (Push [ ilToTy ]) (I_unbox_any ilToTy)

        GenSequel cenv eenv.cloc cgbuf sequel

and GenReraise cenv cgbuf eenv (retTy, m) sequel =
    let ilReturnTy = GenType cenv m eenv.tyenv retTy
    CG.EmitInstr cgbuf (pop 0) Push0 I_rethrow
    // [See comment related to I_throw].
    // Rethrow does not return. Required to push dummy value on the stack.
    // This follows prior behaviour by prim-types reraise<_>.
    CG.EmitInstrs cgbuf (pop 0) (Push [ ilReturnTy ]) [ AI_ldnull; I_unbox_any ilReturnTy ]
    GenSequel cenv eenv.cloc cgbuf sequel

and GenGetExnField cenv cgbuf eenv (e, ecref, fieldNum, m) sequel =
    GenExpr cenv cgbuf eenv e Continue
    let exnc = stripExnEqns ecref
    let ty = GenExnType cenv m eenv.tyenv ecref
    CG.EmitInstr cgbuf (pop 0) Push0 (I_castclass ty)

    let fld = List.item fieldNum exnc.TrueInstanceFieldsAsList
    let ftyp = GenType cenv m eenv.tyenv fld.FormalType

    let mspec =
        mkILNonGenericInstanceMethSpecInTy (ty, "get_" + fld.LogicalName, [], ftyp)

    CG.EmitInstr cgbuf (pop 1) (Push [ ftyp ]) (mkNormalCall mspec)

    GenSequel cenv eenv.cloc cgbuf sequel

and GenSetExnField cenv cgbuf eenv (e, ecref, fieldNum, e2, m) sequel =
    GenExpr cenv cgbuf eenv e Continue
    let exnc = stripExnEqns ecref
    let ty = GenExnType cenv m eenv.tyenv ecref
    CG.EmitInstr cgbuf (pop 0) Push0 (I_castclass ty)
    let fld = List.item fieldNum exnc.TrueInstanceFieldsAsList
    let ftyp = GenType cenv m eenv.tyenv fld.FormalType
    let ilFieldName = ComputeFieldName exnc fld
    GenExpr cenv cgbuf eenv e2 Continue
    CG.EmitInstr cgbuf (pop 2) Push0 (mkNormalStfld (mkILFieldSpecInTy (ty, ilFieldName, ftyp)))
    GenUnitThenSequel cenv eenv m eenv.cloc cgbuf sequel

and UnionCodeGen (cgbuf: CodeGenBuffer) =
    { new EraseUnions.ICodeGen<Mark> with
        member _.CodeLabel m = m.CodeLabel

        member _.GenerateDelayMark() =
            CG.GenerateDelayMark cgbuf "unionCodeGenMark"

        member _.GenLocal ilTy =
            cgbuf.AllocLocal([], ilTy, false) |> uint16

        member _.SetMarkToHere m = CG.SetMarkToHere cgbuf m

        member _.MkInvalidCastExnNewobj() =
            mkInvalidCastExnNewobj cgbuf.mgbuf.cenv.g

        member _.EmitInstr x = CG.EmitInstr cgbuf (pop 0) (Push []) x

        member _.EmitInstrs xs =
            CG.EmitInstrs cgbuf (pop 0) (Push []) xs
    }

and GenUnionCaseProof cenv cgbuf eenv (e, ucref, tyargs, m) sequel =
    let g = cenv.g
    GenExpr cenv cgbuf eenv e Continue
    let cuspec, idx = GenUnionCaseSpec cenv m eenv.tyenv ucref tyargs
    let fty = EraseUnions.GetILTypeForAlternative cuspec idx
    let avoidHelpers = entityRefInThisAssembly g.compilingFSharpCore ucref.TyconRef
    EraseUnions.emitCastData g.ilg (UnionCodeGen cgbuf) (false, avoidHelpers, cuspec, idx)
    CG.EmitInstrs cgbuf (pop 1) (Push [ fty ]) [] // push/pop to match the line above
    GenSequel cenv eenv.cloc cgbuf sequel

and GenGetUnionCaseField cenv cgbuf eenv (e, ucref, tyargs, n, m) sequel =
    let g = cenv.g
    assert (ucref.Tycon.IsStructOrEnumTycon || isProvenUnionCaseTy (tyOfExpr g e))

    GenExpr cenv cgbuf eenv e Continue
    let cuspec, idx = GenUnionCaseSpec cenv m eenv.tyenv ucref tyargs
    let fty = actualTypOfIlxUnionField cuspec idx n
    let avoidHelpers = entityRefInThisAssembly g.compilingFSharpCore ucref.TyconRef
    CG.EmitInstr cgbuf (pop 1) (Push [ fty ]) (EraseUnions.mkLdData (avoidHelpers, cuspec, idx, n))
    GenSequel cenv eenv.cloc cgbuf sequel

and GenGetUnionCaseFieldAddr cenv cgbuf eenv (e, ucref, tyargs, n, m) sequel =
    let g = cenv.g
    assert (ucref.Tycon.IsStructOrEnumTycon || isProvenUnionCaseTy (tyOfExpr g e))

    GenExpr cenv cgbuf eenv e Continue
    let cuspec, idx = GenUnionCaseSpec cenv m eenv.tyenv ucref tyargs
    let fty = actualTypOfIlxUnionField cuspec idx n
    let avoidHelpers = entityRefInThisAssembly g.compilingFSharpCore ucref.TyconRef
    CG.EmitInstr cgbuf (pop 1) (Push [ ILType.Byref fty ]) (EraseUnions.mkLdDataAddr (avoidHelpers, cuspec, idx, n))
    GenSequel cenv eenv.cloc cgbuf sequel

and GenGetUnionCaseTag cenv cgbuf eenv (e, tcref, tyargs, m) sequel =
    let g = cenv.g
    GenExpr cenv cgbuf eenv e Continue
    let cuspec = GenUnionSpec cenv m eenv.tyenv tcref tyargs
    let avoidHelpers = entityRefInThisAssembly g.compilingFSharpCore tcref
    EraseUnions.emitLdDataTag g.ilg (UnionCodeGen cgbuf) (avoidHelpers, cuspec)
    CG.EmitInstrs cgbuf (pop 1) (Push [ g.ilg.typ_Int32 ]) [] // push/pop to match the line above
    GenSequel cenv eenv.cloc cgbuf sequel

and GenSetUnionCaseField cenv cgbuf eenv (e, ucref, tyargs, n, e2, m) sequel =
    let g = cenv.g
    GenExpr cenv cgbuf eenv e Continue
    let cuspec, idx = GenUnionCaseSpec cenv m eenv.tyenv ucref tyargs
    let avoidHelpers = entityRefInThisAssembly g.compilingFSharpCore ucref.TyconRef
    EraseUnions.emitCastData g.ilg (UnionCodeGen cgbuf) (false, avoidHelpers, cuspec, idx)
    CG.EmitInstrs cgbuf (pop 1) (Push [ cuspec.DeclaringType ]) [] // push/pop to match the line above
    GenExpr cenv cgbuf eenv e2 Continue
    CG.EmitInstr cgbuf (pop 2) Push0 (EraseUnions.mkStData (cuspec, idx, n))
    GenUnitThenSequel cenv eenv m eenv.cloc cgbuf sequel

and GenGetRecdFieldAddr cenv cgbuf eenv (e, f, tyargs, m) sequel =
    GenExpr cenv cgbuf eenv e Continue
    let fref = GenRecdFieldRef m cenv eenv.tyenv f tyargs
    CG.EmitInstr cgbuf (pop 1) (Push [ ILType.Byref fref.ActualType ]) (I_ldflda fref)
    GenSequel cenv eenv.cloc cgbuf sequel

and GenGetStaticFieldAddr cenv cgbuf eenv (f, tyargs, m) sequel =
    let fspec = GenRecdFieldRef m cenv eenv.tyenv f tyargs
    CG.EmitInstr cgbuf (pop 0) (Push [ ILType.Byref fspec.ActualType ]) (I_ldsflda fspec)
    GenSequel cenv eenv.cloc cgbuf sequel

and GenGetRecdField cenv cgbuf eenv (e, f, tyargs, m) sequel =
    GenExpr cenv cgbuf eenv e Continue
    GenFieldGet false cenv cgbuf eenv (f, tyargs, m)
    GenSequel cenv eenv.cloc cgbuf sequel

and GenSetRecdField cenv cgbuf eenv (e1, f, tyargs, e2, m) sequel =
    GenExpr cenv cgbuf eenv e1 Continue
    GenExpr cenv cgbuf eenv e2 Continue
    GenFieldStore false cenv cgbuf eenv (f, tyargs, m) sequel

and GenGetStaticField cenv cgbuf eenv (f, tyargs, m) sequel =
    GenFieldGet true cenv cgbuf eenv (f, tyargs, m)
    GenSequel cenv eenv.cloc cgbuf sequel

and GenSetStaticField cenv cgbuf eenv (f, tyargs, e2, m) sequel =
    GenExpr cenv cgbuf eenv e2 Continue
    GenFieldStore true cenv cgbuf eenv (f, tyargs, m) sequel

and mk_field_pops isStatic n = if isStatic then pop n else pop (n + 1)

and GenFieldGet isStatic cenv cgbuf eenv (rfref: RecdFieldRef, tyargs, m) =
    let fspec = GenRecdFieldRef m cenv eenv.tyenv rfref tyargs
    let vol = if rfref.RecdField.IsVolatile then Volatile else Nonvolatile

    if
        useGenuineField rfref.Tycon rfref.RecdField
        || entityRefInThisAssembly cenv.g.compilingFSharpCore rfref.TyconRef
    then
        let instr =
            if isStatic then
                I_ldsfld(vol, fspec)
            else
                I_ldfld(ILAlignment.Aligned, vol, fspec)

        CG.EmitInstr cgbuf (mk_field_pops isStatic 0) (Push [ fspec.ActualType ]) instr
    else
        let cconv =
            if isStatic then
                ILCallingConv.Static
            else
                ILCallingConv.Instance

        let mspec =
            mkILMethSpecInTy (fspec.DeclaringType, cconv, "get_" + rfref.RecdField.rfield_id.idText, [], fspec.FormalType, [])

        CG.EmitInstr cgbuf (mk_field_pops isStatic 0) (Push [ fspec.ActualType ]) (mkNormalCall mspec)

and GenFieldStore isStatic cenv cgbuf eenv (rfref: RecdFieldRef, tyargs, m) sequel =
    let fspec = GenRecdFieldRef m cenv eenv.tyenv rfref tyargs
    let fld = rfref.RecdField

    if fld.IsMutable && not (useGenuineField rfref.Tycon fld) then
        let cconv =
            if isStatic then
                ILCallingConv.Static
            else
                ILCallingConv.Instance

        let mspec =
            mkILMethSpecInTy (fspec.DeclaringType, cconv, "set_" + fld.rfield_id.idText, [ fspec.FormalType ], ILType.Void, [])

        CG.EmitInstr cgbuf (mk_field_pops isStatic 1) Push0 (mkNormalCall mspec)
    else
        let vol = if rfref.RecdField.IsVolatile then Volatile else Nonvolatile

        let instr =
            if isStatic then
                I_stsfld(vol, fspec)
            else
                I_stfld(ILAlignment.Aligned, vol, fspec)

        CG.EmitInstr cgbuf (mk_field_pops isStatic 1) Push0 instr

    GenUnitThenSequel cenv eenv m eenv.cloc cgbuf sequel

//--------------------------------------------------------------------------
// Generate arguments to calls
//--------------------------------------------------------------------------

/// Generate arguments to a call, unless the argument is the single lone "unit" value
/// to a method or value compiled as a method taking no arguments
and GenUntupledArgsDiscardingLoneUnit cenv cgbuf eenv m numObjArgs curriedArgInfos args =
    let g = cenv.g

    match curriedArgInfos, args with
    // Type.M()
    // new C()
    | [ [] ], [ arg ] when numObjArgs = 0 ->
        assert isUnitTy g (tyOfExpr g arg)
        GenExpr cenv cgbuf eenv arg discard
    // obj.M()
    | [ [ _ ]; [] ], [ arg1; arg2 ] when numObjArgs = 1 ->
        assert isUnitTy g (tyOfExpr g arg2)
        GenExpr cenv cgbuf eenv arg1 Continue
        GenExpr cenv cgbuf eenv arg2 discard
    | _ ->
        (curriedArgInfos, args)
        ||> List.iter2 (fun argInfos x -> GenUntupledArgExpr cenv cgbuf eenv m argInfos x)

/// Codegen arguments
and GenUntupledArgExpr cenv cgbuf eenv m argInfos expr =
    let g = cenv.g
    let numRequiredExprs = List.length argInfos

    if numRequiredExprs = 0 then
        ()
    elif numRequiredExprs = 1 then
        GenExpr cenv cgbuf eenv expr Continue
    elif isRefTupleExpr expr then
        let es = tryDestRefTupleExpr expr

        if es.Length <> numRequiredExprs then
            error (InternalError("GenUntupledArgExpr (2)", m))

        es |> List.iter (fun x -> GenExpr cenv cgbuf eenv x Continue)
    else
        let ty = tyOfExpr g expr
        let locv, loce = mkCompGenLocal m "arg" ty
        let bind = mkCompGenBind locv expr

        LocalScope "untuple" cgbuf (fun scopeMarks ->
            let eenvinner = AllocStorageForBind cenv cgbuf scopeMarks eenv bind
            GenBinding cenv cgbuf eenvinner bind false
            let tys = destRefTupleTy g ty
            assert (tys.Length = numRequiredExprs)

            argInfos
            |> List.iteri (fun i _ -> GenGetTupleField cenv cgbuf eenvinner (tupInfoRef, loce, tys, i, m) Continue))

//--------------------------------------------------------------------------
// Generate calls (try to detect direct calls)
//--------------------------------------------------------------------------

and GenWitnessArgFromTraitInfo cenv cgbuf eenv m traitInfo =
    let g = cenv.g
    let storage = TryStorageForWitness g eenv traitInfo.TraitKey

    match storage with
    | None ->
        let witnessExpr =
            ConstraintSolver.CodegenWitnessArgForTraitConstraint cenv.tcVal g cenv.amap m traitInfo
            |> CommitOperationResult

        match witnessExpr with
        | Choice1Of2 _traitInfo ->
            System.Diagnostics.Debug.Assert(false, "expected storage for witness")
            failwith "unexpected non-generation of witness "
        | Choice2Of2 arg ->
            let eenv = { eenv with suppressWitnesses = true }
            GenExpr cenv cgbuf eenv arg Continue
    | Some storage ->
        let ty = GenWitnessTy g traitInfo.TraitKey
        GenGetStorageAndSequel cenv cgbuf eenv m (ty, GenType cenv m eenv.tyenv ty) storage None

and GenWitnessArgFromWitnessInfo cenv cgbuf eenv m witnessInfo =
    let g = cenv.g
    let storage = TryStorageForWitness g eenv witnessInfo

    match storage with
    | None ->
        System.Diagnostics.Debug.Assert(false, "expected storage for witness")
        failwith "unexpected non-generation of witness "
    | Some storage ->
        let ty = GenWitnessTy g witnessInfo
        GenGetStorageAndSequel cenv cgbuf eenv m (ty, GenType cenv m eenv.tyenv ty) storage None

and GenWitnessArgsFromWitnessInfos cenv cgbuf eenv m witnessInfos =
    let g = cenv.g
    let generateWitnesses = ComputeGenerateWitnesses g eenv
    // Witness arguments are only generated in emitted 'inline' code where witness parameters are available.
    if generateWitnesses then
        for witnessInfo in witnessInfos do
            GenWitnessArgFromWitnessInfo cenv cgbuf eenv m witnessInfo

and GenWitnessArgs cenv cgbuf eenv m tps tyargs =
    let g = cenv.g
    let generateWitnesses = ComputeGenerateWitnesses g eenv
    // Witness arguments are only generated in emitted 'inline' code where witness parameters are available.
    if generateWitnesses then
        let mwitnesses =
            ConstraintSolver.CodegenWitnessesForTyparInst cenv.tcVal g cenv.amap m tps tyargs
            |> CommitOperationResult

        for witnessArg in mwitnesses do
            match witnessArg with
            | Choice1Of2 traitInfo -> GenWitnessArgFromTraitInfo cenv cgbuf eenv m traitInfo
            | Choice2Of2 arg -> GenExpr cenv cgbuf eenv arg Continue

and IsBranchTailcall (cenv: cenv) eenv (v: ValRef, tyargs, curriedArgs: _ list) sequel =
    let g = cenv.g

    match ListAssoc.tryFind g.valRefEq v eenv.innerVals with
    | Some (kind, _) ->
        not v.IsConstructor
        &&
        // when branch-calling methods we must have the right type parameters
        (match kind with
         | BranchCallClosure _ -> true
         | BranchCallMethod (_, _, tps, _, _, _) -> (List.lengthsEqAndForall2 (fun ty tp -> typeEquiv g ty (mkTyparTy tp)) tyargs tps))
        &&
        // must be exact #args, ignoring tupling - we untuple if needed below
        (let arityInfo =
            match kind with
            | BranchCallClosure arityInfo
            | BranchCallMethod (arityInfo, _, _, _, _, _) -> arityInfo

         arityInfo.Length = curriedArgs.Length)
        &&
        // no tailcall out of exception handler, etc.
        (match sequelIgnoringEndScopesAndDiscard sequel with
         | Return
         | ReturnVoid -> true
         | _ -> false)
    | None -> false

and GenApp (cenv: cenv) cgbuf eenv (f, fty, tyargs, curriedArgs, m) sequel =
    let g = cenv.g

    match (stripDebugPoints f, tyargs, curriedArgs) with
    // Look for tailcall to turn into branch
    | Expr.Val (v, _, _), _, _ when IsBranchTailcall cenv eenv (v, tyargs, curriedArgs) sequel ->
        let kind, mark = ListAssoc.find g.valRefEq v eenv.innerVals // already checked above in when guard

        // Generate the arguments for the direct tail call.
        // We push all the arguments on the IL stack then write them back to the argument slots using
        // I_starg.  This seems a little sloppy, we could generate-then-write for each of the arguments.
        //
        // The arguments pushed don't include the 'this' argument for a recursive closure call (in PreallocatedArgCount)
        // The arguments _do_ include the 'this' argument for instance method calls.  The arguments do _not_ include witness arguments.
        match kind with
        | BranchCallClosure arityInfo ->
            GenExprs cenv cgbuf eenv curriedArgs

            let numArgs = List.sum arityInfo

            for i = numArgs - 1 downto 0 do
                CG.EmitInstr cgbuf (pop 1) Push0 (I_starg(uint16 (cgbuf.PreallocatedArgCount + i)))

        | BranchCallMethod (arityInfo, curriedArgInfos, _, numObjArgs, numWitnessArgs, numMethodArgs) ->
            assert (curriedArgInfos.Length = arityInfo.Length)
            assert (curriedArgInfos.Length = curriedArgs.Length)

            //assert (curriedArgInfos.Length = numArgs )
            // NOTE: we are not generating the witness arguments here
            GenUntupledArgsDiscardingLoneUnit cenv cgbuf eenv m numObjArgs curriedArgInfos curriedArgs

            // Extension methods with empty arguments are evidently not quite in sufficiently normalized form,
            // so apply a fixup here. This feels like a mistake associated with BindUnitVars, where that is not triggering
            // in this case.
            let numArgs =
                if v.IsExtensionMember then
                    match curriedArgInfos, curriedArgs with
                    // static extension method with empty arguments.
                    | [ [] ], [ _ ] when numObjArgs = 0 -> 0
                    // instance extension method with empty arguments.
                    | [ [ _ ]; [] ], [ _; _ ] when numObjArgs = 0 -> 1
                    | _ -> numMethodArgs
                else
                    numMethodArgs

            for i = numArgs - 1 downto 0 do
                CG.EmitInstr cgbuf (pop 1) Push0 (I_starg(uint16 (cgbuf.PreallocatedArgCount + numObjArgs + numWitnessArgs + i)))

            // Note, we don't reassign the witness arguments as these wont' have changed, because the type parameters are identical

            for i = numObjArgs - 1 downto 0 do
                CG.EmitInstr cgbuf (pop 1) Push0 (I_starg(uint16 (cgbuf.PreallocatedArgCount + i)))

        CG.EmitInstr cgbuf (pop 0) Push0 (I_br mark.CodeLabel)

        GenSequelEndScopes cgbuf sequel

    // PhysicalEquality becomes cheap reference equality once
    // a nominal type is known. We can't replace it for variable types since
    // a "ceq" instruction can't be applied to variable type values.
    | Expr.Val (v, _, _), [ ty ], [ arg1; arg2 ] when (valRefEq g v g.reference_equality_inner_vref) && isAppTy g ty ->

        GenExpr cenv cgbuf eenv arg1 Continue
        GenExpr cenv cgbuf eenv arg2 Continue
        CG.EmitInstr cgbuf (pop 2) (Push [ g.ilg.typ_Bool ]) AI_ceq
        GenSequel cenv eenv.cloc cgbuf sequel

    | Expr.Val (v, _, m), _, _ when
        valRefEq g v g.cgh__resumeAt_vref
        || valRefEq g v g.cgh__resumableEntry_vref
        || valRefEq g v g.cgh__stateMachine_vref
        ->
        errorR (Error(FSComp.SR.ilxgenInvalidConstructInStateMachineDuringCodegen (v.DisplayName), m))
        CG.EmitInstr cgbuf (pop 0) (Push [ g.ilg.typ_Object ]) AI_ldnull
        GenSequel cenv eenv.cloc cgbuf sequel

    // Emit "methodhandleof" calls as ldtoken instructions
    //
    // The token for the "GenericMethodDefinition" is loaded
    | Expr.Val (v, _, m), _, [ arg ] when valRefEq g v g.methodhandleof_vref ->
        let (|OptionalCoerce|) x =
            match stripDebugPoints x with
            | Expr.Op (TOp.Coerce _, _, [ arg ], _) -> arg
            | x -> x

        let (|OptionalTyapp|) x =
            match stripDebugPoints x with
            | Expr.App (f, _, [ _ ], [], _) -> f
            | x -> x

        match stripDebugPoints arg with
        // Generate ldtoken instruction for "methodhandleof(fun (a, b, c) -> f(a, b, c))"
        // where f is an F# function value or F# method
        | Expr.Lambda (_, _, _, _, DebugPoints (Expr.App (OptionalCoerce (OptionalTyapp (Expr.Val (vref, _, _))), _, _, _, _), _), _, _) ->

            let storage = StorageForValRef m vref eenv

            match storage with
            | Method (_, _, mspec, _, _, _, _, _, _, _, _, _) ->
                CG.EmitInstr cgbuf (pop 0) (Push [ g.iltyp_RuntimeMethodHandle ]) (I_ldtoken(ILToken.ILMethod mspec))
            | _ -> errorR (Error(FSComp.SR.ilxgenUnexpectedArgumentToMethodHandleOfDuringCodegen (), m))

        // Generate ldtoken instruction for "methodhandleof(fun (a, b, c) -> obj.M(a, b, c))"
        // where M is an IL method.
        | Expr.Lambda (_,
                       _,
                       _,
                       _,
                       DebugPoints (Expr.Op (TOp.ILCall (_, _, isStruct, _, _, _, _, ilMethRef, enclTypeInst, methInst, _), _, _, _), _),
                       _,
                       _) ->

            let boxity = (if isStruct then AsValue else AsObject)

            let mkFormalParams gparams =
                gparams |> DropErasedTyargs |> List.mapi (fun n _gf -> mkILTyvarTy (uint16 n))

            let ilGenericMethodSpec =
                mkILMethSpec (ilMethRef, boxity, mkFormalParams enclTypeInst, mkFormalParams methInst)

            let i = I_ldtoken(ILToken.ILMethod ilGenericMethodSpec)
            CG.EmitInstr cgbuf (pop 0) (Push [ g.iltyp_RuntimeMethodHandle ]) i

        | _ ->
            System.Diagnostics.Debug.Assert(false, sprintf "Break for invalid methodhandleof argument expression")
            //System.Diagnostics.Debugger.Break()
            errorR (Error(FSComp.SR.ilxgenUnexpectedArgumentToMethodHandleOfDuringCodegen (), m))

        GenSequel cenv eenv.cloc cgbuf sequel

    // Optimize calls to top methods when given "enough" arguments.
    | Expr.Val (vref, valUseFlags, _), _, _ when
        (let storage = StorageForValRef m vref eenv

         match storage with
         | Method (valReprInfo, vref, _, _, _, _, _, _, _, _, _, _) ->
             (let tps, argTys, _, _ = GetValReprTypeInFSharpForm g valReprInfo vref.Type m
              tps.Length = tyargs.Length && argTys.Length <= curriedArgs.Length)
         | _ -> false)
        ->

        let storage = StorageForValRef m vref eenv

        match storage with
        | Method (valReprInfo, vref, mspec, mspecW, _, ctps, mtps, curriedArgInfos, _, _, _, _) ->

            let nowArgs, laterArgs = List.splitAt curriedArgInfos.Length curriedArgs

            let actualRetTy = applyTys cenv.g vref.Type (tyargs, nowArgs)

            let _, witnessInfos, curriedArgInfos, returnTy, _ =
                GetValReprTypeInCompiledForm cenv.g valReprInfo ctps.Length vref.Type m

            let mspec =
                let generateWitnesses = ComputeGenerateWitnesses g eenv

                if not generateWitnesses || witnessInfos.IsEmpty then
                    mspec
                else
                    mspecW

            let ilTyArgs = GenTypeArgs cenv m eenv.tyenv tyargs

            // For instance method calls chop off some type arguments, which are already
            // carried by the class.  Also work out if it's a virtual call.
            let _, virtualCall, newobj, isSuperInit, isSelfInit, takesInstanceArg, _, _ =
                GetMemberCallInfo g (vref, valUseFlags)

            // numEnclILTypeArgs will include unit-of-measure args, unfortunately. For now, just cut-and-paste code from GetMemberCallInfo
            // @REVIEW: refactor this
            let numEnclILTypeArgs =
                match vref.MemberInfo with
                | Some _ when not vref.IsExtensionMember -> List.length (vref.MemberApparentEntity.TyparsNoRange |> DropErasedTypars)
                | _ -> 0

            let ilEnclArgTys, ilMethArgTys =
                if ilTyArgs.Length < numEnclILTypeArgs then
                    error (InternalError("length mismatch", m))

                List.splitAt numEnclILTypeArgs ilTyArgs

            let boxity = mspec.DeclaringType.Boxity
            let mspec = mkILMethSpec (mspec.MethodRef, boxity, ilEnclArgTys, ilMethArgTys)

            // "Unit" return types on static methods become "void"
            let mustGenerateUnitAfterCall = Option.isNone returnTy

            let ccallInfo =
                match valUseFlags with
                | PossibleConstrainedCall ty -> Some ty
                | _ -> None

            let isBaseCall =
                match valUseFlags with
                | VSlotDirectCall -> true
                | _ -> false

            let isTailCall =
                if isNil laterArgs && not isSelfInit then
                    let isDllImport = IsValRefIsDllImport g vref
                    let hasByrefArg = mspec.FormalArgTypes |> List.exists IsILTypeByref
                    let makesNoCriticalTailcalls = vref.MakesNoCriticalTailcalls
                    let hasStructObjArg = (boxity = AsValue) && takesInstanceArg

                    CanTailcall(
                        hasStructObjArg,
                        ccallInfo,
                        eenv.withinSEH,
                        hasByrefArg,
                        mustGenerateUnitAfterCall,
                        isDllImport,
                        isSelfInit,
                        makesNoCriticalTailcalls,
                        sequel
                    )
                else
                    Normalcall

            let useICallVirt = virtualCall || useCallVirt cenv boxity mspec isBaseCall

            let callInstr =
                match valUseFlags with
                | PossibleConstrainedCall ty ->
                    let ilThisTy = GenType cenv m eenv.tyenv ty
                    I_callconstraint(isTailCall, ilThisTy, mspec, None)
                | _ ->
                    if newobj then I_newobj(mspec, None)
                    elif useICallVirt then I_callvirt(isTailCall, mspec, None)
                    else I_call(isTailCall, mspec, None)

            // ok, now we're ready to generate
            if isSuperInit || isSelfInit then
                CG.EmitInstr cgbuf (pop 0) (Push [ mspec.DeclaringType ]) mkLdarg0

            if not cenv.g.generateWitnesses || witnessInfos.IsEmpty then
                () // no witness args
            else
                let _ctyargs, mtyargs = List.splitAt ctps.Length tyargs
                GenWitnessArgs cenv cgbuf eenv m mtps mtyargs

            GenUntupledArgsDiscardingLoneUnit cenv cgbuf eenv m vref.NumObjArgs curriedArgInfos nowArgs

            // Generate laterArgs (for effects) and save
            LocalScope "callstack" cgbuf (fun scopeMarks ->
                let whereSaved, eenv =
                    (eenv, laterArgs)
                    ||> List.mapFold (fun eenv laterArg ->
                        // Only save arguments that have effects
                        if Optimizer.ExprHasEffect g laterArg then
                            let ilTy = laterArg |> tyOfExpr g |> GenType cenv m eenv.tyenv

                            let locName =
                                // Ensure that we have an g.CompilerGlobalState
                                assert (g.CompilerGlobalState |> Option.isSome)
                                g.CompilerGlobalState.Value.IlxGenNiceNameGenerator.FreshCompilerGeneratedName("arg", m), ilTy, false

                            let loc, _realloc, eenv = AllocLocal cenv cgbuf eenv true locName scopeMarks
                            GenExpr cenv cgbuf eenv laterArg Continue
                            EmitSetLocal cgbuf loc
                            Choice1Of2(ilTy, loc), eenv
                        else
                            Choice2Of2 laterArg, eenv)

                let nargs = mspec.FormalArgTypes.Length

                let pushes =
                    if mustGenerateUnitAfterCall || isSuperInit || isSelfInit then
                        Push0
                    else
                        (Push [ (GenType cenv m eenv.tyenv actualRetTy) ])

                CG.EmitInstr cgbuf (pop (nargs + (if mspec.CallingConv.IsStatic || newobj then 0 else 1))) pushes callInstr

                // For isSuperInit, load the 'this' pointer as the pretend 'result' of the operation. It will be popped again in most cases
                if isSuperInit then
                    CG.EmitInstr cgbuf (pop 0) (Push [ mspec.DeclaringType ]) mkLdarg0

                // When generating debug code, generate a 'nop' after a 'call' that returns 'void'
                // This is what C# does, as it allows the call location to be maintained correctly in the stack frame
                if
                    cenv.options.generateDebugSymbols
                    && mustGenerateUnitAfterCall
                    && (isTailCall = Normalcall)
                then
                    CG.EmitInstr cgbuf (pop 0) Push0 AI_nop

                if isNil laterArgs then
                    assert isNil whereSaved
                    // Generate the "unit" value if necessary
                    CommitCallSequel cenv eenv m eenv.cloc cgbuf mustGenerateUnitAfterCall sequel
                else
                    //printfn "%d EXTRA ARGS IN TOP APP at %s" laterArgs.Length (stringOfRange m)
                    whereSaved
                    |> List.iter (function
                        | Choice1Of2 (ilTy, loc) -> EmitGetLocal cgbuf ilTy loc
                        | Choice2Of2 expr -> GenExpr cenv cgbuf eenv expr Continue)

                    GenIndirectCall cenv cgbuf eenv (actualRetTy, [], laterArgs, m) sequel)

        | _ -> failwith "??"

    // This case is for getting/calling a value, when we can't call it directly.
    // However, we know the type instantiation for the value.
    // In this case we can often generate a type-specific local expression for the value.
    // This reduces the number of dynamic type applications.
    | Expr.Val (vref, _, _), _, _ -> GenGetValRefAndSequel cenv cgbuf eenv m vref (Some(tyargs, curriedArgs, m, sequel))

    | _ ->
        (* worst case: generate a first-class function value and call *)
        GenExpr cenv cgbuf eenv f Continue
        GenCurriedArgsAndIndirectCall cenv cgbuf eenv (fty, tyargs, curriedArgs, m) sequel

and CanTailcall
    (
        hasStructObjArg,
        ccallInfo,
        withinSEH,
        hasByrefArg,
        mustGenerateUnitAfterCall,
        isDllImport,
        isSelfInit,
        makesNoCriticalTailcalls,
        sequel
    ) =

    // Can't tailcall with a struct object arg since it involves a byref
    // Can't tailcall with a .NET 2.0 generic constrained call since it involves a byref
    if
        not hasStructObjArg
        && Option.isNone ccallInfo
        && not withinSEH
        && not hasByrefArg
        && not isDllImport
        && not isSelfInit
        && not makesNoCriticalTailcalls
        &&

        // We can tailcall even if we need to generate "unit", as long as we're about to throw the value away anyway as par of the return.
        // We can tailcall if we don't need to generate "unit", as long as we're about to return.
        (match sequelIgnoreEndScopes sequel with
         | ReturnVoid
         | Return -> not mustGenerateUnitAfterCall
         | DiscardThen ReturnVoid -> mustGenerateUnitAfterCall
         | _ -> false)
    then
        Tailcall
    else
        Normalcall

/// Choose the names for TraitWitnessInfo representations in arguments and free variables
and ChooseWitnessInfoNames takenNames (witnessInfos: TraitWitnessInfo list) =
    witnessInfos
    |> List.map (fun w -> String.uncapitalize w.MemberName)
    |> ChooseFreeVarNames takenNames

/// Represent the TraitWitnessInfos as arguments, e.g. in local type functions
and ArgStorageForWitnessInfos (cenv: cenv) (eenv: IlxGenEnv) takenNames pretakenArgs m (witnessInfos: TraitWitnessInfo list) =
    let names = ChooseWitnessInfoNames takenNames witnessInfos

    (witnessInfos, List.indexed names)
    ||> List.map2 (fun w (i, nm) ->
        let ty = GenWitnessTy cenv.g w
        let ilTy = GenType cenv m eenv.tyenv ty
        let ilParam = mkILParam (Some nm, ilTy)
        let storage = Arg(i + pretakenArgs)
        ilParam, (w, storage))
    |> List.unzip

/// Represent the TraitWitnessInfos as free variables, e.g. in closures
and FreeVarStorageForWitnessInfos (cenv: cenv) (eenv: IlxGenEnv) takenNames ilCloTyInner m (witnessInfos: TraitWitnessInfo list) =
    let names = ChooseWitnessInfoNames takenNames witnessInfos

    (witnessInfos, names)
    ||> List.map2 (fun w nm ->
        let ty = GenWitnessTy cenv.g w
        let ilTy = GenType cenv m eenv.tyenv ty
        let ilFv = mkILFreeVar (nm, true, ilTy)

        let storage =
            let ilField = mkILFieldSpecInTy (ilCloTyInner, ilFv.fvName, ilFv.fvType)
            Env(ilCloTyInner, ilField, None)

        ilFv, (w, storage))
    |> List.unzip

//--------------------------------------------------------------------------
// Locally erased type functions
//--------------------------------------------------------------------------

/// Check for type lambda with entirely erased type arguments that is stored as
/// local variable (not method or property). For example
//      let foo() =
//          let a = 0<_>
//          ()
//  in debug code , here `a` will be a TyLamba.  However the compiled representation of
// `a` is an integer.
and IsLocalErasedTyLambda eenv (v: Val) e =
    match e with
    | Expr.TyLambda (_, tyargs, body, _, _) when
        tyargs |> List.forall (fun tp -> tp.IsErased)
        && (match StorageForVal v.Range v eenv with
            | Local _ -> true
            | _ -> false)
        ->
        match stripExpr body with
        | Expr.Lambda _ -> None
        | _ -> Some body
    | _ -> None

//--------------------------------------------------------------------------
// Named local type functions
//--------------------------------------------------------------------------

and IsNamedLocalTypeFuncVal g (v: Val) expr =
    not v.IsCompiledAsTopLevel
    && IsGenericValWithGenericConstraints g v
    && (match stripExpr expr with
        | Expr.TyLambda _ -> true
        | _ -> false)

and AddDirectTyparWitnessParams cenv eenv cloinfo m =
    let directTypars =
        match cloinfo.cloExpr with
        | Expr.TyLambda (_, tvs, _, _, _) -> tvs
        | _ -> []

    let directWitnessInfos =
        let generateWitnesses = ComputeGenerateWitnesses cenv.g eenv

        if generateWitnesses then
            // The 0 here represents that a closure doesn't reside within a generic class - there are no "enclosing class type parameters" to lop off.
            GetTraitWitnessInfosOfTypars cenv.g 0 directTypars
        else
            []

    // Direct witnesses get passed as arguments to DirectInvoke
    let ilDirectWitnessParams, ilDirectWitnessParamsStorage =
        let pretakenArgs = 1
        ArgStorageForWitnessInfos cenv eenv [] pretakenArgs m directWitnessInfos

    let eenv = eenv |> AddStorageForLocalWitnesses ilDirectWitnessParamsStorage

    directTypars, ilDirectWitnessParams, directWitnessInfos, eenv

and GenNamedLocalTyFuncCall cenv (cgbuf: CodeGenBuffer) eenv ty cloinfo tyargs m =
    let g = cenv.g

    let ilTyArgs = tyargs |> GenTypeArgs cenv m eenv.tyenv

    let ilCloTy = cloinfo.cloSpec.ILType

    let ilDirectGenericParams, ilDirectWitnessParams, directWitnessInfos =
        let eenvinner = EnvForTypars cloinfo.cloFreeTyvars eenv

        let directTypars =
            match cloinfo.cloExpr with
            | Expr.TyLambda (_, tvs, _, _, _) -> tvs
            | _ -> []

        let eenvinner = AddTyparsToEnv directTypars eenvinner

        let ilDirectGenericParams = GenGenericParams cenv eenvinner directTypars

        let _directTypars, ilDirectWitnessParams, directWitnessInfos, _eenv =
            AddDirectTyparWitnessParams cenv eenvinner cloinfo m

        ilDirectGenericParams, ilDirectWitnessParams, directWitnessInfos

    if not (List.length ilDirectGenericParams = ilTyArgs.Length) then
        errorR (Error(FSComp.SR.ilIncorrectNumberOfTypeArguments (), m))

    // Recover result (value or reference types) via unbox_any.
    CG.EmitInstr cgbuf (pop 1) (Push [ ilCloTy ]) (I_unbox_any ilCloTy)

    let actualRetTy = applyTys g ty (tyargs, [])

    let ilDirectWitnessParamsTys = ilDirectWitnessParams |> List.map (fun p -> p.Type)

    let ilDirectInvokeMethSpec =
        mkILInstanceMethSpecInTy (ilCloTy, "DirectInvoke", ilDirectWitnessParamsTys, cloinfo.ilCloFormalReturnTy, ilTyArgs)

    GenWitnessArgsFromWitnessInfos cenv cgbuf eenv m directWitnessInfos

    let ilActualRetTy = GenType cenv m eenv.tyenv actualRetTy
    CountCallFuncInstructions()
    CG.EmitInstr cgbuf (pop (1 + ilDirectWitnessParamsTys.Length)) (Push [ ilActualRetTy ]) (mkNormalCall ilDirectInvokeMethSpec)
    actualRetTy

/// Generate an indirect call, converting to an ILX callfunc instruction
and GenCurriedArgsAndIndirectCall cenv cgbuf eenv (funcTy, tyargs, curriedArgs, m) sequel =

    // Generate the curried arguments to the indirect call
    GenExprs cenv cgbuf eenv curriedArgs
    GenIndirectCall cenv cgbuf eenv (funcTy, tyargs, curriedArgs, m) sequel

/// Generate an indirect call, converting to an ILX callfunc instruction
and GenIndirectCall cenv cgbuf eenv (funcTy, tyargs, curriedArgs, m) sequel =
    let g = cenv.g

    // Fold in the new types into the environment as we generate the formal types.
    let ilxClosureApps =
        // keep only non-erased type arguments when computing indirect call
        let tyargs = DropErasedTyargs tyargs

        let typars, formalFuncTy = tryDestForallTy g funcTy

        let feenv = eenv.tyenv.Add typars

        // This does two phases: REVIEW: the code is too complex for what it's achieving and should be rewritten
        let formalRetTy, appBuilder =
            ((formalFuncTy, id), curriedArgs)
            ||> List.fold (fun (formalFuncTy, appBuilder) _ ->
                let dty, rty = destFunTy cenv.g formalFuncTy
                (rty, (fun acc -> appBuilder (Apps_app(GenType cenv m feenv dty, acc)))))

        let ilxRetApps = Apps_done(GenType cenv m feenv formalRetTy)

        List.foldBack (fun tyarg acc -> Apps_tyapp(GenType cenv m eenv.tyenv tyarg, acc)) tyargs (appBuilder ilxRetApps)

    let actualRetTy = applyTys g funcTy (tyargs, curriedArgs)
    let ilActualRetTy = GenType cenv m eenv.tyenv actualRetTy

    // Check if any byrefs are involved to make sure we don't tailcall
    let hasByrefArg =
        let rec check x =
            match x with
            | Apps_tyapp (_, apps) -> check apps
            | Apps_app (arg, apps) -> IsILTypeByref arg || check apps
            | _ -> false

        check ilxClosureApps

    let isTailCall =
        CanTailcall(false, None, eenv.withinSEH, hasByrefArg, false, false, false, false, sequel)

    CountCallFuncInstructions()

    // Generate the code code an ILX callfunc operation
    let instrs =
        EraseClosures.mkCallFunc
            cenv.ilxPubCloEnv
            (fun ty -> cgbuf.AllocLocal([], ty, false) |> uint16)
            eenv.tyenv.Count
            isTailCall
            ilxClosureApps

    CG.EmitInstrs cgbuf (pop (1 + curriedArgs.Length)) (Push [ ilActualRetTy ]) instrs

    // Done compiling indirect call...
    GenSequel cenv eenv.cloc cgbuf sequel

//--------------------------------------------------------------------------
// Generate try expressions
//--------------------------------------------------------------------------

and GenTry cenv cgbuf eenv scopeMarks (e1, m, resultTy, spTry) =
    let g = cenv.g

    match spTry with
    | DebugPointAtTry.Yes m -> CG.EmitDebugPoint cgbuf m
    | DebugPointAtTry.No -> ()

    let stack, eenvinner = EmitSaveStack cenv cgbuf eenv m scopeMarks
    let startTryMark = CG.GenerateMark cgbuf "startTryMark"
    let endTryMark = CG.GenerateDelayMark cgbuf "endTryMark"
    let afterHandler = CG.GenerateDelayMark cgbuf "afterHandler"

    let ilResultTyOpt =
        if isUnitTy g resultTy then
            None
        else
            Some(GenType cenv m eenvinner.tyenv resultTy)

    let whereToSaveOpt, eenvinner =
        match ilResultTyOpt with
        | None -> None, eenvinner
        | Some ilResultTy ->
            // Ensure that we have an g.CompilerGlobalState
            assert (cenv.g.CompilerGlobalState |> Option.isSome)

            let whereToSave, _realloc, eenvinner =
                AllocLocal
                    cenv
                    cgbuf
                    eenvinner
                    true
                    (cenv.g.CompilerGlobalState.Value.IlxGenNiceNameGenerator.FreshCompilerGeneratedName("tryres", m), ilResultTy, false)
                    (startTryMark, endTryMark)

            Some(whereToSave, ilResultTy), eenvinner

    let exitSequel = LeaveHandler(false, whereToSaveOpt, afterHandler, true)

    let eenvinner =
        { eenvinner with
            withinSEH = true
            exitSequel = exitSequel
        }

    // Generate the body of the try.
    GenExpr cenv cgbuf eenvinner e1 exitSequel
    CG.SetMarkToHere cgbuf endTryMark
    let tryMarks = (startTryMark.CodeLabel, endTryMark.CodeLabel)
    whereToSaveOpt, eenvinner, stack, tryMarks, afterHandler

/// Determine if a filter block is side-effect free, meaning it can be run on the first pass and
/// the pattern match logic repeated on the second pass.
///
/// Filter blocks are only ever generated by pattern match compilation so we can safely look for particular
/// constructs.
and eligibleForFilter (cenv: cenv) expr =
    let rec check expr =
        match expr with
        | Expr.Let (TBind (_, be, _), body, _, _) -> check be && check body
        | Expr.DebugPoint (_, expr) -> check expr
        | Expr.Match (_spBind, _exprm, dtree, targets, _, _) ->
            checkDecisionTree dtree
            && targets |> Array.forall (fun (TTarget (_, e, _)) -> check e)
        | Expr.Const _ -> true
        | Expr.Op (TOp.ILAsm ([ I_isinst _ ], _), _, _, _) -> true
        | Expr.Op (TOp.UnionCaseTagGet _, _, _, _) -> true
        | Expr.Op (TOp.ExnFieldGet _, _, _, _) -> true
        | Expr.Op (TOp.UnionCaseFieldGet _, _, _, _) -> true
        | Expr.Op (TOp.ValFieldGet _, _, _, _) -> true
        | Expr.Op (TOp.TupleFieldGet _, _, _, _) -> true
        | Expr.Op (TOp.Coerce _, _, _, _) -> true
        | Expr.Val _ -> true
        | _ -> false

    and checkDecisionTree dtree =
        match dtree with
        | TDSwitch (ve, cases, dflt, _) ->
            check ve
            && cases |> List.forall checkDecisionTreeCase
            && dflt |> Option.forall checkDecisionTree
        | TDSuccess (es, _) -> es |> List.forall check
        | TDBind (bind, rest) -> check bind.Expr && checkDecisionTree rest

    and checkDecisionTreeCase dcase =
        let (TCase (test, tree)) = dcase

        checkDecisionTree tree
        && match test with
           | DecisionTreeTest.UnionCase _ -> true
           | DecisionTreeTest.ArrayLength _ -> true
           | DecisionTreeTest.Const _ -> true
           | DecisionTreeTest.IsNull -> true
           | DecisionTreeTest.IsInst _ -> true
           | DecisionTreeTest.ActivePatternCase _ -> false // must only be run once
           | DecisionTreeTest.Error _ -> false

    let isTrivial =
        match expr with
        | DebugPoints (Expr.Const _, _) -> true
        | _ -> false

    // Filters seem to generate invalid code for the ilreflect.fs backend
    (cenv.options.ilxBackend = IlxGenBackend.IlWriteBackend)
    && not isTrivial
    && check expr

and GenTryWith cenv cgbuf eenv (e1, valForFilter: Val, filterExpr, valForHandler: Val, handlerExpr, m, resTy, spTry, spWith) sequel =
    let g = cenv.g

    // Save the stack - gross because IL flushes the stack at the exn. handler
    // note: eenvinner notes spill vars are live
    LocalScope "trystack" cgbuf (fun scopeMarks ->
        let whereToSaveOpt, eenvinner, stack, tryMarks, afterHandler =
            GenTry cenv cgbuf eenv scopeMarks (e1, m, resTy, spTry)

        let seh =
            if cenv.options.generateFilterBlocks || eligibleForFilter cenv filterExpr then
                let startOfFilter = CG.GenerateMark cgbuf "startOfFilter"
                let afterFilter = CG.GenerateDelayMark cgbuf "afterFilter"

                let sequelOnBranches, afterJoin, stackAfterJoin, sequelAfterJoin =
                    GenJoinPoint cenv cgbuf "filter" eenv g.int_ty m EndFilter

                let eenvinner =
                    { eenvinner with
                        exitSequel = sequelOnBranches
                    }
                // We emit the debug point for the 'with' keyword span on the start of the filter
                // block. However the targets of the filter block pattern matching should not get any
                // debug points (they will be 'true'/'false' values indicating if the exception has been
                // caught or not).
                //
                // The targets of the handler block DO get debug points. Thus the expected behaviour
                // for a try/with with a complex pattern is that we hit the "with" before the filter is run
                // and then jump to the handler for the successful catch (or continue with exception handling
                // if the filter fails)
                match spWith with
                | DebugPointAtWith.Yes m -> CG.EmitDebugPoint cgbuf m
                | DebugPointAtWith.No -> ()

                CG.SetStack cgbuf [ g.ilg.typ_Object ]

                let _, eenvinner =
                    AllocLocalVal cenv cgbuf valForFilter eenvinner None (startOfFilter, afterFilter)

                CG.EmitInstr cgbuf (pop 1) (Push [ g.iltyp_Exception ]) (I_castclass g.iltyp_Exception)

                GenStoreVal cgbuf eenvinner valForFilter.Range valForFilter

                // Why SPSuppress? Because we do not emit a debug point at the start of the List.filter - we've already put one on
                // the 'with' keyword above
                GenExpr cenv cgbuf eenvinner filterExpr sequelOnBranches
                CG.SetMarkToHere cgbuf afterJoin
                CG.SetStack cgbuf stackAfterJoin
                GenSequel cenv eenv.cloc cgbuf sequelAfterJoin
                let endOfFilter = CG.GenerateMark cgbuf "endOfFilter"
                let filterMarks = (startOfFilter.CodeLabel, endOfFilter.CodeLabel)
                CG.SetMarkToHere cgbuf afterFilter

                let startOfHandler = CG.GenerateMark cgbuf "startOfHandler"

                CG.SetStack cgbuf [ g.ilg.typ_Object ]

                let _, eenvinner =
                    AllocLocalVal cenv cgbuf valForHandler eenvinner None (startOfHandler, afterHandler)

                CG.EmitInstr cgbuf (pop 1) (Push [ g.iltyp_Exception ]) (I_castclass g.iltyp_Exception)
                GenStoreVal cgbuf eenvinner valForHandler.Range valForHandler

                let exitSequel = LeaveHandler(false, whereToSaveOpt, afterHandler, true)
                GenExpr cenv cgbuf eenvinner handlerExpr exitSequel

                let endOfHandler = CG.GenerateMark cgbuf "endOfHandler"
                let handlerMarks = (startOfHandler.CodeLabel, endOfHandler.CodeLabel)
                ILExceptionClause.FilterCatch(filterMarks, handlerMarks)
            else
                let startOfHandler = CG.GenerateMark cgbuf "startOfHandler"

                match spWith with
                | DebugPointAtWith.Yes m -> CG.EmitDebugPoint cgbuf m
                | DebugPointAtWith.No -> ()

                CG.SetStack cgbuf [ g.ilg.typ_Object ]

                let _, eenvinner =
                    AllocLocalVal cenv cgbuf valForHandler eenvinner None (startOfHandler, afterHandler)

                CG.EmitInstr cgbuf (pop 1) (Push [ g.iltyp_Exception ]) (I_castclass g.iltyp_Exception)

                GenStoreVal cgbuf eenvinner m valForHandler

                let exitSequel = LeaveHandler(false, whereToSaveOpt, afterHandler, true)

                let eenvinner =
                    { eenvinner with
                        exitSequel = exitSequel
                    }

                GenExpr cenv cgbuf eenvinner handlerExpr exitSequel

                let endOfHandler = CG.GenerateMark cgbuf "endOfHandler"
                let handlerMarks = (startOfHandler.CodeLabel, endOfHandler.CodeLabel)
                ILExceptionClause.TypeCatch(g.ilg.typ_Object, handlerMarks)

        cgbuf.EmitExceptionClause { Clause = seh; Range = tryMarks }

        CG.SetMarkToHere cgbuf afterHandler
        CG.SetStack cgbuf []

        cgbuf.EmitStartOfHiddenCode()

        // Restore the stack and load the result
        EmitRestoreStack cgbuf stack

        match whereToSaveOpt with
        | Some (whereToSave, ilResultTy) ->
            EmitGetLocal cgbuf ilResultTy whereToSave
            GenSequel cenv eenv.cloc cgbuf sequel
        | None -> GenUnitThenSequel cenv eenv m eenv.cloc cgbuf sequel)

and GenTryFinally cenv cgbuf eenv (bodyExpr, handlerExpr, m, resTy, spTry, spFinally) sequel =
    // Save the stack - needed because IL flushes the stack at the exn. handler
    // note: eenvinner notes spill vars are live
    LocalScope "trystack" cgbuf (fun scopeMarks ->

        let whereToSaveOpt, eenvinner, stack, tryMarks, afterHandler =
            GenTry cenv cgbuf eenv scopeMarks (bodyExpr, m, resTy, spTry)

        // Now the catch/finally block
        let startOfHandler = CG.GenerateMark cgbuf "startOfHandler"
        CG.SetStack cgbuf []

        match spFinally with
        | DebugPointAtFinally.Yes m -> CG.EmitDebugPoint cgbuf m
        | DebugPointAtFinally.No -> ()

        let exitSequel = LeaveHandler(true, whereToSaveOpt, afterHandler, true)
        GenExpr cenv cgbuf eenvinner handlerExpr exitSequel
        let endOfHandler = CG.GenerateMark cgbuf "endOfHandler"
        let handlerMarks = (startOfHandler.CodeLabel, endOfHandler.CodeLabel)

        cgbuf.EmitExceptionClause
            {
                Clause = ILExceptionClause.Finally handlerMarks
                Range = tryMarks
            }

        CG.SetMarkToHere cgbuf afterHandler
        CG.SetStack cgbuf []

        // Restore the stack and load the result
        cgbuf.EmitStartOfHiddenCode()
        EmitRestoreStack cgbuf stack

        match whereToSaveOpt with
        | Some (whereToSave, ilResultTy) ->
            EmitGetLocal cgbuf ilResultTy whereToSave
            GenSequel cenv eenv.cloc cgbuf sequel
        | None -> GenUnitThenSequel cenv eenv m eenv.cloc cgbuf sequel)

//--------------------------------------------------------------------------
// Generate for-loop
//--------------------------------------------------------------------------

and GenIntegerForLoop cenv cgbuf eenv (spFor, spTo, v, e1, dir, e2, loopBody, m) sequel =
    let eenv = SetIsInLoop true eenv
    let g = cenv.g

    // The JIT/NGen eliminate array-bounds checks for C# loops of form:
    //   for(int i=0; i < (#ldlen arr#); i++) { ... arr[i] ... }
    // Here
    //     dir = BI_blt indicates an optimized for loop that fits C# form that evaluates its 'end' argument each time around
    //     dir = BI_ble indicates a normal F# for loop that evaluates its argument only once
    //
    // It is also important that we follow C# IL-layout exactly "prefix, jmp test, body, test, finish" for JIT/NGEN.
    let start = CG.GenerateMark cgbuf "for_start"
    let finish = CG.GenerateDelayMark cgbuf "for_finish"
    let inner = CG.GenerateDelayMark cgbuf "for_inner"
    let test = CG.GenerateDelayMark cgbuf "for_test"
    let stack, eenvinner = EmitSaveStack cenv cgbuf eenv m (start, finish)

    let isUp =
        (match dir with
         | FSharpForLoopUp
         | CSharpForLoopUp -> true
         | FSharpForLoopDown -> false)

    let isFSharpStyle =
        (match dir with
         | FSharpForLoopUp
         | FSharpForLoopDown -> true
         | CSharpForLoopUp -> false)

    let finishIdx, eenvinner =
        if isFSharpStyle then
            // Ensure that we have an g.CompilerGlobalState
            assert (g.CompilerGlobalState |> Option.isSome)

            let vName =
                g.CompilerGlobalState.Value.IlxGenNiceNameGenerator.FreshCompilerGeneratedName("endLoop", m)

            let v, _realloc, eenvinner =
                AllocLocal cenv cgbuf eenvinner true (vName, g.ilg.typ_Int32, false) (start, finish)

            v, eenvinner
        else
            -1, eenvinner

    let _, eenvinner = AllocLocalVal cenv cgbuf v eenvinner None (start, finish)

    match spFor with
    | DebugPointAtFor.Yes spStart -> CG.EmitDebugPoint cgbuf spStart
    | DebugPointAtFor.No -> ()

    GenExpr cenv cgbuf eenv e1 Continue
    GenStoreVal cgbuf eenvinner m v

    if isFSharpStyle then
        GenExpr cenv cgbuf eenvinner e2 Continue
        EmitSetLocal cgbuf finishIdx
        EmitGetLocal cgbuf g.ilg.typ_Int32 finishIdx
        GenGetLocalVal cenv cgbuf eenvinner e2.Range v None
        CG.EmitInstr cgbuf (pop 2) Push0 (I_brcmp((if isUp then BI_blt else BI_bgt), finish.CodeLabel))

    else
        CG.EmitInstr cgbuf (pop 0) Push0 (I_br test.CodeLabel)

    cgbuf.EmitStartOfHiddenCode()

    // .inner
    CG.SetMarkToHere cgbuf inner

    //    <loop body>
    GenExpr cenv cgbuf eenvinner loopBody discard

    //    v++ or v--
    GenGetLocalVal cenv cgbuf eenvinner e2.Range v None

    CG.EmitInstr cgbuf (pop 0) (Push [ g.ilg.typ_Int32 ]) (mkLdcInt32 1)
    CG.EmitInstr cgbuf (pop 1) Push0 (if isUp then AI_add else AI_sub)
    GenStoreVal cgbuf eenvinner m v

    // .text
    CG.SetMarkToHere cgbuf test

    // FSharpForLoopUp: if v <> e2 + 1 then goto .inner
    // FSharpForLoopDown: if v <> e2 - 1 then goto .inner
    // CSharpStyle: if v < e2 then goto .inner
    match spTo with
    | DebugPointAtInOrTo.Yes spStart -> CG.EmitDebugPoint cgbuf spStart
    | DebugPointAtInOrTo.No -> ()

    GenGetLocalVal cenv cgbuf eenvinner e2.Range v None

    let cmp =
        match dir with
        | FSharpForLoopUp
        | FSharpForLoopDown -> BI_bne_un
        | CSharpForLoopUp -> BI_blt

    let e2Sequel = (CmpThenBrOrContinue(pop 2, [ I_brcmp(cmp, inner.CodeLabel) ]))

    if isFSharpStyle then
        EmitGetLocal cgbuf g.ilg.typ_Int32 finishIdx
        CG.EmitInstr cgbuf (pop 0) (Push [ g.ilg.typ_Int32 ]) (mkLdcInt32 1)
        CG.EmitInstr cgbuf (pop 1) Push0 (if isUp then AI_add else AI_sub)
        GenSequel cenv eenv.cloc cgbuf e2Sequel
    else
        GenExpr cenv cgbuf eenv e2 e2Sequel

    // .finish - loop-exit here
    CG.SetMarkToHere cgbuf finish

    // Restore the stack and load the result
    EmitRestoreStack cgbuf stack
    GenUnitThenSequel cenv eenv m eenv.cloc cgbuf sequel

//--------------------------------------------------------------------------
// Generate while-loop
//--------------------------------------------------------------------------

and GenWhileLoop cenv cgbuf eenv (spWhile, condExpr, bodyExpr, m) sequel =
    let eenv = SetIsInLoop true eenv

    // jmp test; body; test; if testPassed then jmp body else finish
    //
    // This is a pattern recognized by the JIT and it results in the most efficient assembly.
    if cgbuf.GetCurrentStack().IsEmpty then
        let startTest = CG.GenerateDelayMark cgbuf "startTest"
        CG.EmitInstr cgbuf (pop 0) Push0 (I_br startTest.CodeLabel)

        let startBody = CG.GenerateMark cgbuf "startBody"
        GenExpr cenv cgbuf eenv bodyExpr discard

        match spWhile with
        | DebugPointAtWhile.Yes spStart -> CG.EmitDebugPoint cgbuf spStart
        | DebugPointAtWhile.No -> ()

        CG.SetMarkToHere cgbuf startTest
        GenExpr cenv cgbuf eenv condExpr (CmpThenBrOrContinue(pop 1, [ I_brcmp(BI_brtrue, startBody.CodeLabel) ]))

    // In the rare cases when there is something already on the stack, e.g.
    //
    // let f() =
    //     callSomething firstArgument ((while .... do ...); secondArgument)
    //
    // we emit
    //
    // test; if not testPassed jmp finish; body; jmp test; finish
    else
        let finish = CG.GenerateDelayMark cgbuf "while_finish"

        match spWhile with
        | DebugPointAtWhile.Yes spStart -> CG.EmitDebugPoint cgbuf spStart
        | DebugPointAtWhile.No -> ()

        let startTest = CG.GenerateMark cgbuf "startTest"

        GenExpr cenv cgbuf eenv condExpr (CmpThenBrOrContinue(pop 1, [ I_brcmp(BI_brfalse, finish.CodeLabel) ]))

        GenExpr cenv cgbuf eenv bodyExpr (DiscardThen(Br startTest))
        CG.SetMarkToHere cgbuf finish

    GenUnitThenSequel cenv eenv m eenv.cloc cgbuf sequel

//--------------------------------------------------------------------------
// Generate IL assembly code.
// Polymorphic IL/ILX instructions may be instantiated when polymorphic code is inlined.
// We must implement this for the few uses of polymorphic instructions
// in the standard libarary.
//--------------------------------------------------------------------------

and GenAsmCode cenv cgbuf eenv (il, tyargs, args, returnTys, m) sequel =
    let g = cenv.g
    let ilTyArgs = GenTypesPermitVoid cenv m eenv.tyenv tyargs
    let ilReturnTys = GenTypesPermitVoid cenv m eenv.tyenv returnTys

    let ilAfterInst =
        il
        |> List.filter (function
            | AI_nop -> false
            | _ -> true)
        |> List.map (fun i ->
            let err s =
                errorR (InternalError(sprintf "%s: bad instruction: %A" s i, m))

            let modFieldSpec fspec =
                if isNil ilTyArgs then
                    fspec
                else
                    { fspec with
                        DeclaringType =
                            let ty = fspec.DeclaringType
                            let tspec = ty.TypeSpec
                            mkILTy ty.Boxity (mkILTySpec (tspec.TypeRef, ilTyArgs))
                    }

            match i, ilTyArgs with
            | I_unbox_any (ILType.TypeVar _), [ tyarg ] -> I_unbox_any tyarg
            | I_box (ILType.TypeVar _), [ tyarg ] -> I_box tyarg
            | I_isinst (ILType.TypeVar _), [ tyarg ] -> I_isinst tyarg
            | I_castclass (ILType.TypeVar _), [ tyarg ] -> I_castclass tyarg
            | I_newarr (shape, ILType.TypeVar _), [ tyarg ] -> I_newarr(shape, tyarg)
            | I_ldelem_any (shape, ILType.TypeVar _), [ tyarg ] -> I_ldelem_any(shape, tyarg)
            | I_ldelema (ro, _, shape, ILType.TypeVar _), [ tyarg ] -> I_ldelema(ro, false, shape, tyarg)
            | I_stelem_any (shape, ILType.TypeVar _), [ tyarg ] -> I_stelem_any(shape, tyarg)
            | I_ldobj (a, b, ILType.TypeVar _), [ tyarg ] -> I_ldobj(a, b, tyarg)
            | I_stobj (a, b, ILType.TypeVar _), [ tyarg ] -> I_stobj(a, b, tyarg)
            | I_ldtoken (ILToken.ILType (ILType.TypeVar _)), [ tyarg ] -> I_ldtoken(ILToken.ILType tyarg)
            | I_sizeof (ILType.TypeVar _), [ tyarg ] -> I_sizeof tyarg
            | I_cpobj (ILType.TypeVar _), [ tyarg ] -> I_cpobj tyarg
            | I_initobj (ILType.TypeVar _), [ tyarg ] -> I_initobj tyarg
            | I_ldfld (al, vol, fspec), _ -> I_ldfld(al, vol, modFieldSpec fspec)
            | I_ldflda fspec, _ -> I_ldflda(modFieldSpec fspec)
            | I_stfld (al, vol, fspec), _ -> I_stfld(al, vol, modFieldSpec fspec)
            | I_stsfld (vol, fspec), _ -> I_stsfld(vol, modFieldSpec fspec)
            | I_ldsfld (vol, fspec), _ -> I_ldsfld(vol, modFieldSpec fspec)
            | I_ldsflda fspec, _ -> I_ldsflda(modFieldSpec fspec)
            | EI_ilzero (ILType.TypeVar _), [ tyarg ] -> EI_ilzero tyarg
            | AI_nop, _ -> i
            // These are embedded in the IL for a an initonly ldfld, i.e.
            // here's the relevant comment from tc.fs
            //     "Add an I_nop if this is an initonly field to make sure we never recognize it as an lvalue. See mkExprAddrOfExpr."

            | _ ->
                if not (isNil tyargs) then
                    err "Bad polymorphic IL instruction"

                i)

    match ilAfterInst, args, sequel, ilReturnTys with

    | [ EI_ilzero _ ], _, _, _ ->
        match tyargs with
        | [ ty ] ->
            GenDefaultValue cenv cgbuf eenv (ty, m)
            GenSequel cenv eenv.cloc cgbuf sequel
        | _ -> failwith "Bad polymorphic IL instruction"

    // ldnull; cgt.un then branch is used to test for null and can become a direct brtrue/brfalse
    | [ AI_ldnull; AI_cgt_un ], [ arg1 ], CmpThenBrOrContinue (1, [ I_brcmp (bi, label1) ]), _ ->

        GenExpr cenv cgbuf eenv arg1 (CmpThenBrOrContinue(pop 1, [ I_brcmp(bi, label1) ]))

    // Strip off any ("ceq" x false) when the sequel is a comparison branch and change the BI_brfalse to a BI_brtrue
    // This is the instruction sequence for "not"
    // For these we can just generate the argument and change the test (from a brfalse to a brtrue and vice versa)
    | ([ AI_ceq ],
       [ arg1
         Expr.Const ((Const.Bool false
                     | Const.SByte 0y
                     | Const.Int16 0s
                     | Const.Int32 0
                     | Const.Int64 0L
                     | Const.Byte 0uy
                     | Const.UInt16 0us
                     | Const.UInt32 0u
                     | Const.UInt64 0UL),
                     _,
                     _) ],
       CmpThenBrOrContinue (1,
                            [ I_brcmp ((BI_brfalse
                                       | BI_brtrue) as bi,
                                       label1) ]),
       _) ->

        let bi =
            match bi with
            | BI_brtrue -> BI_brfalse
            | _ -> BI_brtrue

        GenExpr cenv cgbuf eenv arg1 (CmpThenBrOrContinue(pop 1, [ I_brcmp(bi, label1) ]))

    // Query; when do we get a 'ret' in IL assembly code?
    | [ I_ret ], [ arg1 ], sequel, [ _ilRetTy ] ->

        GenExpr cenv cgbuf eenv arg1 Continue
        CG.EmitInstr cgbuf (pop 1) Push0 I_ret
        GenSequelEndScopes cgbuf sequel

    // Query; when do we get a 'ret' in IL assembly code?
    | [ I_ret ], [], sequel, [ _ilRetTy ] ->

        CG.EmitInstr cgbuf (pop 1) Push0 I_ret
        GenSequelEndScopes cgbuf sequel

    // 'throw' instructions are a bit of a problem - e.g. let x = (throw ...) in ... expects a value *)
    // to be left on the stack. But dead-code checking by some versions of the .NET verifier *)
    // mean that we can't just have fake code after the throw to generate the fake value *)
    // (nb. a fake value can always be generated by a "ldnull unbox.any ty" sequence *)
    // So in the worst case we generate a fake (never-taken) branch to a piece of code to generate *)
    // the fake value *)
    | [ I_throw ], [ arg1 ], sequel, [ ilRetTy ] ->
        match sequelIgnoreEndScopes sequel with
        | s when IsSequelImmediate s ->
            (* In most cases we can avoid doing this... *)
            GenExpr cenv cgbuf eenv arg1 Continue
            CG.EmitInstr cgbuf (pop 1) Push0 I_throw
            GenSequelEndScopes cgbuf sequel
        | _ ->
            let after1 = CG.GenerateDelayMark cgbuf "fake_join"
            let after2 = CG.GenerateDelayMark cgbuf "fake_join"
            let after3 = CG.GenerateDelayMark cgbuf "fake_join"
            CG.EmitInstrs cgbuf (pop 0) Push0 [ mkLdcInt32 0; I_brcmp(BI_brfalse, after2.CodeLabel) ]

            CG.SetMarkToHere cgbuf after1
            CG.EmitInstrs cgbuf (pop 0) (Push [ ilRetTy ]) [ AI_ldnull; I_unbox_any ilRetTy; I_br after3.CodeLabel ]

            CG.SetMarkToHere cgbuf after2
            GenExpr cenv cgbuf eenv arg1 Continue
            CG.EmitInstr cgbuf (pop 1) Push0 I_throw
            CG.SetMarkToHere cgbuf after3
            GenSequel cenv eenv.cloc cgbuf sequel
    | _ ->
        // float or float32 or float<_> or float32<_>
        let anyfpType ty =
            typeEquivAux EraseMeasures g g.float_ty ty
            || typeEquivAux EraseMeasures g g.float32_ty ty

        // Otherwise generate the arguments, and see if we can use a I_brcmp rather than a comparison followed by an I_brfalse/I_brtrue
        GenExprs cenv cgbuf eenv args

        match ilAfterInst, sequel with

        // NOTE: THESE ARE NOT VALID ON FLOATING POINT DUE TO NaN. Hence INLINE ASM ON FP. MUST BE CAREFULLY WRITTEN

        | [ AI_clt ], CmpThenBrOrContinue (1, [ I_brcmp (BI_brfalse, label1) ]) when not (anyfpType (tyOfExpr g args.Head)) ->
            CG.EmitInstr cgbuf (pop 2) Push0 (I_brcmp(BI_bge, label1))
        | [ AI_cgt ], CmpThenBrOrContinue (1, [ I_brcmp (BI_brfalse, label1) ]) when not (anyfpType (tyOfExpr g args.Head)) ->
            CG.EmitInstr cgbuf (pop 2) Push0 (I_brcmp(BI_ble, label1))
        | [ AI_clt_un ], CmpThenBrOrContinue (1, [ I_brcmp (BI_brfalse, label1) ]) when not (anyfpType (tyOfExpr g args.Head)) ->
            CG.EmitInstr cgbuf (pop 2) Push0 (I_brcmp(BI_bge_un, label1))
        | [ AI_cgt_un ], CmpThenBrOrContinue (1, [ I_brcmp (BI_brfalse, label1) ]) when not (anyfpType (tyOfExpr g args.Head)) ->
            CG.EmitInstr cgbuf (pop 2) Push0 (I_brcmp(BI_ble_un, label1))
        | [ AI_ceq ], CmpThenBrOrContinue (1, [ I_brcmp (BI_brfalse, label1) ]) when not (anyfpType (tyOfExpr g args.Head)) ->
            CG.EmitInstr cgbuf (pop 2) Push0 (I_brcmp(BI_bne_un, label1))

        // THESE ARE VALID ON FP w.r.t. NaN

        | [ AI_clt ], CmpThenBrOrContinue (1, [ I_brcmp (BI_brtrue, label1) ]) -> CG.EmitInstr cgbuf (pop 2) Push0 (I_brcmp(BI_blt, label1))
        | [ AI_cgt ], CmpThenBrOrContinue (1, [ I_brcmp (BI_brtrue, label1) ]) -> CG.EmitInstr cgbuf (pop 2) Push0 (I_brcmp(BI_bgt, label1))
        | [ AI_clt_un ], CmpThenBrOrContinue (1, [ I_brcmp (BI_brtrue, label1) ]) ->
            CG.EmitInstr cgbuf (pop 2) Push0 (I_brcmp(BI_blt_un, label1))
        | [ AI_cgt_un ], CmpThenBrOrContinue (1, [ I_brcmp (BI_brtrue, label1) ]) ->
            CG.EmitInstr cgbuf (pop 2) Push0 (I_brcmp(BI_bgt_un, label1))
        | [ AI_ceq ], CmpThenBrOrContinue (1, [ I_brcmp (BI_brtrue, label1) ]) -> CG.EmitInstr cgbuf (pop 2) Push0 (I_brcmp(BI_beq, label1))
        | _ ->
            // Failing that, generate the real IL leaving value(s) on the stack
            CG.EmitInstrs cgbuf (pop args.Length) (Push ilReturnTys) ilAfterInst

            // If no return values were specified generate a "unit"
            if isNil returnTys then
                GenUnitThenSequel cenv eenv m eenv.cloc cgbuf sequel
            else
                GenSequel cenv eenv.cloc cgbuf sequel

//--------------------------------------------------------------------------
// Generate expression quotations
//--------------------------------------------------------------------------

and GenQuotation cenv cgbuf eenv (ast, qdataCell, m, ety) sequel =
    let g = cenv.g
    let suppressWitnesses = eenv.suppressWitnesses

    let referencedTypeDefs, typeSplices, exprSplices, astSpec =
        match qdataCell.Value with
        | Some (data1, data2) -> if suppressWitnesses then data1 else data2

        | None ->
            try
                let qscope =
                    QuotationTranslator.QuotationGenerationScope.Create(
                        g,
                        cenv.amap,
                        cenv.viewCcu,
                        cenv.tcVal,
                        QuotationTranslator.IsReflectedDefinition.No
                    )

                let astSpec = QuotationTranslator.ConvExprPublic qscope suppressWitnesses ast
                let referencedTypeDefs, typeSplices, exprSplices = qscope.Close()
                referencedTypeDefs, List.map fst typeSplices, List.map fst exprSplices, astSpec
            with QuotationTranslator.InvalidQuotedTerm e ->
                error e

    let astSerializedBytes = QuotationPickler.pickle astSpec

    let someTypeInModuleExpr = mkTypeOfExpr cenv m eenv.someTypeInThisAssembly
    let rawTy = mkRawQuotedExprTy g

    let typeSpliceExprs =
        List.map (GenType cenv m eenv.tyenv >> (mkTypeOfExpr cenv m)) typeSplices

    let bytesExpr = Expr.Op(TOp.Bytes astSerializedBytes, [], [], m)

    let deserializeExpr =
        let qf = QuotationTranslator.QuotationGenerationScope.ComputeQuotationFormat g

        if qf.SupportsDeserializeEx then
            let referencedTypeDefExprs =
                List.map (mkILNonGenericBoxedTy >> mkTypeOfExpr cenv m) referencedTypeDefs

            let referencedTypeDefsExpr = mkArray (g.system_Type_ty, referencedTypeDefExprs, m)
            let typeSplicesExpr = mkArray (g.system_Type_ty, typeSpliceExprs, m)
            let spliceArgsExpr = mkArray (rawTy, exprSplices, m)
            mkCallDeserializeQuotationFSharp40Plus g m someTypeInModuleExpr referencedTypeDefsExpr typeSplicesExpr spliceArgsExpr bytesExpr
        else
            let mkList ty els =
                List.foldBack (mkCons g ty) els (mkNil g m ty)

            let typeSplicesExpr = mkList g.system_Type_ty typeSpliceExprs
            let spliceArgsExpr = mkList rawTy exprSplices
            mkCallDeserializeQuotationFSharp20Plus g m someTypeInModuleExpr typeSplicesExpr spliceArgsExpr bytesExpr

    let afterCastExpr =
        // Detect a typed quotation and insert the cast if needed. The cast should not fail but does
        // unfortunately involve a "typeOf" computation over a quotation tree.
        if tyconRefEq g (tcrefOfAppTy g ety) g.expr_tcr then
            mkCallCastQuotation g m (List.head (argsOfAppTy g ety)) deserializeExpr
        else
            deserializeExpr

    GenExpr cenv cgbuf eenv afterCastExpr sequel

//--------------------------------------------------------------------------
// Generate calls to IL methods
//--------------------------------------------------------------------------

and GenILCall
    cenv
    cgbuf
    eenv
    (virt, valu, newobj, valUseFlags, isDllImport, ilMethRef: ILMethodRef, enclArgTys, methArgTys, argExprs, returnTys, m)
    sequel
    =
    let hasByrefArg = ilMethRef.ArgTypes |> List.exists IsILTypeByref

    let isSuperInit =
        match valUseFlags with
        | CtorValUsedAsSuperInit -> true
        | _ -> false

    let isBaseCall =
        match valUseFlags with
        | VSlotDirectCall -> true
        | _ -> false

    let ccallInfo =
        match valUseFlags with
        | PossibleConstrainedCall ty -> Some ty
        | _ -> None

    let boxity = (if valu then AsValue else AsObject)
    let mustGenerateUnitAfterCall = isNil returnTys
    let makesNoCriticalTailcalls = (newobj || not virt) // Don't tailcall for 'newobj', or 'call' to IL code
    let hasStructObjArg = valu && ilMethRef.CallingConv.IsInstance

    let tail =
        CanTailcall(
            hasStructObjArg,
            ccallInfo,
            eenv.withinSEH,
            hasByrefArg,
            mustGenerateUnitAfterCall,
            isDllImport,
            false,
            makesNoCriticalTailcalls,
            sequel
        )

    let ilEnclArgTys = GenTypeArgs cenv m eenv.tyenv enclArgTys
    let ilMethArgTys = GenTypeArgs cenv m eenv.tyenv methArgTys
    let ilReturnTys = GenTypes cenv m eenv.tyenv returnTys
    let ilMethSpec = mkILMethSpec (ilMethRef, boxity, ilEnclArgTys, ilMethArgTys)
    let useICallVirt = virt || useCallVirt cenv boxity ilMethSpec isBaseCall

    // Load the 'this' pointer to pass to the superclass constructor. This argument is not
    // in the expression tree since it can't be treated like an ordinary value
    if isSuperInit then
        CG.EmitInstr cgbuf (pop 0) (Push [ ilMethSpec.DeclaringType ]) mkLdarg0

    GenExprs cenv cgbuf eenv argExprs

    let il =
        if newobj then
            I_newobj(ilMethSpec, None)
        else
            match ccallInfo with
            | Some objArgTy ->
                let ilObjArgTy = GenType cenv m eenv.tyenv objArgTy
                I_callconstraint(tail, ilObjArgTy, ilMethSpec, None)
            | None ->
                if useICallVirt then
                    I_callvirt(tail, ilMethSpec, None)
                else
                    I_call(tail, ilMethSpec, None)

    CG.EmitInstr cgbuf (pop (argExprs.Length + (if isSuperInit then 1 else 0))) (if isSuperInit then Push0 else Push ilReturnTys) il

    // Load the 'this' pointer as the pretend 'result' of the isSuperInit operation.
    // It will be immediately popped in most cases, but may also be used as the target of some "property set" operations.
    if isSuperInit then
        CG.EmitInstr cgbuf (pop 0) (Push [ ilMethSpec.DeclaringType ]) mkLdarg0

    CommitCallSequel cenv eenv m eenv.cloc cgbuf mustGenerateUnitAfterCall sequel

and CommitCallSequel cenv eenv m cloc cgbuf mustGenerateUnitAfterCall sequel =
    if mustGenerateUnitAfterCall then
        GenUnitThenSequel cenv eenv m cloc cgbuf sequel
    else
        GenSequel cenv cloc cgbuf sequel

and MakeNotSupportedExnExpr cenv eenv (argExpr, m) =
    let g = cenv.g
    let ety = mkAppTy (g.FindSysTyconRef [ "System" ] "NotSupportedException") []
    let ilTy = GenType cenv m eenv.tyenv ety
    let mref = mkILCtorMethSpecForTy(ilTy, [ g.ilg.typ_String ]).MethodRef
    Expr.Op(TOp.ILCall(false, false, false, true, NormalValUse, false, false, mref, [], [], [ ety ]), [], [ argExpr ], m)

and GenTraitCall (cenv: cenv) cgbuf eenv (traitInfo: TraitConstraintInfo, argExprs, m) expr sequel =
    let g = cenv.g
    let generateWitnesses = ComputeGenerateWitnesses g eenv

    let witness =
        if generateWitnesses then
            TryStorageForWitness g eenv traitInfo.TraitKey
        else
            None

    match witness with
    | Some storage ->

        let ty = GenWitnessTy g traitInfo.TraitKey
        let argExprs = if argExprs.Length = 0 then [ mkUnit g m ] else argExprs
        GenGetStorageAndSequel cenv cgbuf eenv m (ty, GenType cenv m eenv.tyenv ty) storage (Some([], argExprs, m, sequel))

    | None ->

        // If witnesses are available, we should now always find trait witnesses in scope
        assert not generateWitnesses

        let minfoOpt =
            CommitOperationResult(ConstraintSolver.CodegenWitnessExprForTraitConstraint cenv.tcVal g cenv.amap m traitInfo argExprs)

        match minfoOpt with
        | None ->
            let exnArg =
                mkString g m (FSComp.SR.ilDynamicInvocationNotSupported (traitInfo.MemberName))

            let exnExpr = MakeNotSupportedExnExpr cenv eenv (exnArg, m)
            let replacementExpr = mkThrow m (tyOfExpr g expr) exnExpr
            GenExpr cenv cgbuf eenv replacementExpr sequel
        | Some expr ->
            let expr = cenv.optimizeDuringCodeGen false expr
            GenExpr cenv cgbuf eenv expr sequel

//--------------------------------------------------------------------------
// Generate byref-related operations
//--------------------------------------------------------------------------

and GenGetAddrOfRefCellField cenv cgbuf eenv (e, ty, m) sequel =
    GenExpr cenv cgbuf eenv e Continue
    let fref = GenRecdFieldRef m cenv eenv.tyenv (mkRefCellContentsRef cenv.g) [ ty ]
    CG.EmitInstr cgbuf (pop 1) (Push [ ILType.Byref fref.ActualType ]) (I_ldflda fref)
    GenSequel cenv eenv.cloc cgbuf sequel

and GenGetValAddr cenv cgbuf eenv (v: ValRef, m) sequel =
    let vspec = v.Deref
    let ilTy = GenTypeOfVal cenv eenv vspec
    let storage = StorageForValRef m v eenv

    match storage with
    | Local (idx, _, None) -> CG.EmitInstr cgbuf (pop 0) (Push [ ILType.Byref ilTy ]) (I_ldloca(uint16 idx))

    | Arg idx -> CG.EmitInstr cgbuf (pop 0) (Push [ ILType.Byref ilTy ]) (I_ldarga(uint16 idx))

    | StaticPropertyWithField (fspec, _vref, hasLiteralAttr, _ilTyForProperty, _, ilTy, _, _, _) ->
        if hasLiteralAttr then
            errorR (Error(FSComp.SR.ilAddressOfLiteralFieldIsInvalid (), m))

        let ilTy =
            if ilTy.IsNominal && ilTy.Boxity = ILBoxity.AsValue then
                ILType.Byref ilTy
            else
                ilTy

        EmitGetStaticFieldAddr cgbuf ilTy fspec

    | Env (_, ilField, _) -> CG.EmitInstrs cgbuf (pop 0) (Push [ ILType.Byref ilTy ]) [ mkLdarg0; mkNormalLdflda ilField ]

    | Local (_, _, Some _)
    | StaticProperty _
    | Method _
    | Env _
    | Null ->
        errorR (Error(FSComp.SR.ilAddressOfValueHereIsInvalid (v.DisplayName), m))

        CG.EmitInstr
            cgbuf
            (pop 1)
            (Push [ ILType.Byref ilTy ])
            (I_ldarga(uint16 669 (* random value for post-hoc diagnostic analysis on generated tree *) ))

    GenSequel cenv eenv.cloc cgbuf sequel

and GenGetByref cenv cgbuf eenv (v: ValRef, m) sequel =
    GenGetLocalVRef cenv cgbuf eenv m v None
    let ilTy = GenType cenv m eenv.tyenv (destByrefTy cenv.g v.Type)
    CG.EmitInstr cgbuf (pop 1) (Push [ ilTy ]) (mkNormalLdobj ilTy)
    GenSequel cenv eenv.cloc cgbuf sequel

and GenSetByref cenv cgbuf eenv (v: ValRef, e, m) sequel =
    GenGetLocalVRef cenv cgbuf eenv m v None
    GenExpr cenv cgbuf eenv e Continue
    let ilTy = GenType cenv m eenv.tyenv (destByrefTy cenv.g v.Type)
    CG.EmitInstr cgbuf (pop 2) Push0 (mkNormalStobj ilTy)
    GenUnitThenSequel cenv eenv m eenv.cloc cgbuf sequel

and GenDefaultValue cenv cgbuf eenv (ty, m) =
    let g = cenv.g
    let ilTy = GenType cenv m eenv.tyenv ty

    if isRefTy g ty then
        CG.EmitInstr cgbuf (pop 0) (Push [ ilTy ]) AI_ldnull
    else
        match tryTcrefOfAppTy g ty with
        | ValueSome tcref when
            (tyconRefEq g g.system_SByte_tcref tcref
             || tyconRefEq g g.system_Int16_tcref tcref
             || tyconRefEq g g.system_Int32_tcref tcref
             || tyconRefEq g g.system_Bool_tcref tcref
             || tyconRefEq g g.system_Byte_tcref tcref
             || tyconRefEq g g.system_Char_tcref tcref
             || tyconRefEq g g.system_UInt16_tcref tcref
             || tyconRefEq g g.system_UInt32_tcref tcref)
            ->
            CG.EmitInstr cgbuf (pop 0) (Push [ ilTy ]) iLdcZero
        | ValueSome tcref when
            (tyconRefEq g g.system_Int64_tcref tcref
             || tyconRefEq g g.system_UInt64_tcref tcref)
            ->
            CG.EmitInstr cgbuf (pop 0) (Push [ ilTy ]) (iLdcInt64 0L)
        | ValueSome tcref when (tyconRefEq g g.system_Single_tcref tcref) -> CG.EmitInstr cgbuf (pop 0) (Push [ ilTy ]) (iLdcSingle 0.0f)
        | ValueSome tcref when (tyconRefEq g g.system_Double_tcref tcref) -> CG.EmitInstr cgbuf (pop 0) (Push [ ilTy ]) (iLdcDouble 0.0)
        | _ ->
            let ilTy = GenType cenv m eenv.tyenv ty

            LocalScope "ilzero" cgbuf (fun scopeMarks ->
                let locIdx, realloc, _ =
                    // Ensure that we have an g.CompilerGlobalState
                    assert (g.CompilerGlobalState |> Option.isSome)

                    AllocLocal
                        cenv
                        cgbuf
                        eenv
                        true
                        (g.CompilerGlobalState.Value.IlxGenNiceNameGenerator.FreshCompilerGeneratedName("default", m), ilTy, false)
                        scopeMarks
                // We can normally rely on .NET IL zero-initialization of the temporaries
                // we create to get zero values for struct types.
                //
                // However this doesn't work when
                //   - we're reusing a local (realloc)
                //   - SkipLocalsInit is active (not eenv.initLocals)
                //   - we're in a loop (when we may get a backward branch, and the local may have been realloc'd elsewhere)
                //
                // "initobj" (Generated by EmitInitLocal) doesn't work on byref types
                // But ilzero(&ty) only gets generated in the built-in get-address function so
                // we can just rely on zeroinit of all IL locals.
                if (realloc || not eenv.initLocals || eenv.isInLoop) && not (IsILTypeByref ilTy) then
                    EmitInitLocal cgbuf ilTy locIdx

                EmitGetLocal cgbuf ilTy locIdx)

//--------------------------------------------------------------------------
// Generate generic parameters
//--------------------------------------------------------------------------

and GenGenericParam cenv eenv (tp: Typar) =
    let g = cenv.g

    let subTypeConstraints =
        tp.Constraints
        |> List.choose (function
            | TyparConstraint.CoercesTo (ty, _) -> Some ty
            | _ -> None)
        |> List.map (GenTypeAux cenv tp.Range eenv.tyenv VoidNotOK PtrTypesNotOK)

    let refTypeConstraint =
        tp.Constraints
        |> List.exists (function
            | TyparConstraint.IsReferenceType _ -> true
            | TyparConstraint.SupportsNull _ -> true
            | _ -> false)

    let notNullableValueTypeConstraint =
        tp.Constraints
        |> List.exists (function
            | TyparConstraint.IsNonNullableStruct _ -> true
            | _ -> false)

    let defaultConstructorConstraint =
        tp.Constraints
        |> List.exists (function
            | TyparConstraint.RequiresDefaultConstructor _ -> true
            | _ -> false)

    let tpName =
        // use the CompiledName if given
        // Inference variables get given an IL name "TA, TB" etc.
        let nm =
            match tp.ILName with
            | None -> tp.Name
            | Some nm -> nm
        // Some special rules apply when compiling Fsharp.Core.dll to avoid a proliferation of [<CompiledName>] attributes on type parameters
        if g.compilingFSharpCore then
            match nm with
            | "U" -> "TResult"
            | "U1" -> "TResult1"
            | "U2" -> "TResult2"
            | _ ->
                if nm.TrimEnd([| '0' .. '9' |]).Length = 1 then
                    nm
                elif
                    nm.Length >= 1
                    && nm[0] = 'T'
                    && (nm.Length = 1 || not (System.Char.IsLower nm[1]))
                then
                    nm
                else
                    "T" + (String.capitalize nm)
        else
            nm

    let tpAttrs = mkILCustomAttrs (GenAttrs cenv eenv tp.Attribs)

    {
        Name = tpName
        Constraints = subTypeConstraints
        Variance = NonVariant
        CustomAttrsStored = storeILCustomAttrs tpAttrs
        MetadataIndex = NoMetadataIdx
        HasReferenceTypeConstraint = refTypeConstraint
        HasNotNullableValueTypeConstraint = notNullableValueTypeConstraint
        HasDefaultConstructorConstraint = defaultConstructorConstraint
    }

//--------------------------------------------------------------------------
// Generate object expressions as ILX "closures"
//--------------------------------------------------------------------------

/// Generates the data used for parameters at definitions of abstract method slots such as interface methods or override methods.
and GenSlotParam m cenv eenv slotParam : ILParameter =
    let (TSlotParam (nm, ty, inFlag, outFlag, optionalFlag, attribs)) = slotParam
    let ilTy = GenParamType cenv m eenv.tyenv true ty

    let inFlag2, outFlag2, optionalFlag2, defaultParamValue, paramMarshal2, attribs =
        GenParamAttribs cenv ty attribs

    let ilAttribs = GenAttrs cenv eenv attribs

    let ilAttribs =
        match GenReadOnlyAttributeIfNecessary cenv.g ty with
        | Some attr -> ilAttribs @ [ attr ]
        | None -> ilAttribs

    {
        Name = nm
        Type = ilTy
        Default = defaultParamValue
        Marshal = paramMarshal2
        IsIn = inFlag || inFlag2
        IsOut = outFlag || outFlag2
        IsOptional = optionalFlag || optionalFlag2
        CustomAttrsStored = storeILCustomAttrs (mkILCustomAttrs ilAttribs)
        MetadataIndex = NoMetadataIdx
    }

and GenFormalSlotsig m cenv eenv slotsig =
    let (TSlotSig (_, ty, ctps, mtps, paraml, returnTy)) = slotsig
    let paraml = List.concat paraml
    let ilTy = GenType cenv m eenv.tyenv ty
    let eenvForSlotSig = EnvForTypars (ctps @ mtps) eenv
    let ilParams = paraml |> List.map (GenSlotParam m cenv eenvForSlotSig)
    let ilRet = GenFormalReturnType m cenv eenvForSlotSig returnTy
    ilTy, ilParams, ilRet

and GenOverridesSpec cenv eenv slotsig m =
    let (TSlotSig (nameOfOverridenMethod, _, _, methodTypars, _, _)) = slotsig

    let ilOverrideTy, ilOverrideParams, ilOverrideRet =
        GenFormalSlotsig m cenv eenv slotsig

    let ilOverrideTyRef = ilOverrideTy.TypeRef

    let ilOverrideMethRef =
        mkILMethRef (
            ilOverrideTyRef,
            ILCallingConv.Instance,
            nameOfOverridenMethod,
            List.length (DropErasedTypars methodTypars),
            typesOfILParams ilOverrideParams,
            ilOverrideRet.Type
        )

    OverridesSpec(ilOverrideMethRef, ilOverrideTy)

and GenFormalReturnType m cenv eenvFormal returnTy : ILReturn =
    let ilRetTy = GenReturnType cenv m eenvFormal.tyenv returnTy
    let ilRet = mkILReturn ilRetTy

    match returnTy with
    | None -> ilRet
    | Some ty ->
        match GenReadOnlyAttributeIfNecessary cenv.g ty with
        | Some attr -> ilRet.WithCustomAttrs(mkILCustomAttrs (ilRet.CustomAttrs.AsList() @ [ attr ]))
        | None -> ilRet

and instSlotParam inst (TSlotParam (nm, ty, inFlag, fl2, fl3, attrs)) =
    TSlotParam(nm, instType inst ty, inFlag, fl2, fl3, attrs)

and GenActualSlotsig
    m
    cenv
    eenv
    (TSlotSig (_, ty, ctps, mtps, ilSlotParams, ilSlotRetTy))
    methTyparsOfOverridingMethod
    (methodParams: Val list)
    =
    let ilSlotParams = List.concat ilSlotParams

    let instForSlotSig =
        mkTyparInst (ctps @ mtps) (argsOfAppTy cenv.g ty @ generalizeTypars methTyparsOfOverridingMethod)

    let ilParams =
        ilSlotParams
        |> List.map (instSlotParam instForSlotSig >> GenSlotParam m cenv eenv)

    // Use the better names if available
    let ilParams =
        if ilParams.Length = methodParams.Length then
            (ilParams, methodParams)
            ||> List.map2 (fun p pv -> { p with Name = Some(nameOfVal pv) })
        else
            ilParams

    let ilRetTy =
        GenReturnType cenv m eenv.tyenv (Option.map (instType instForSlotSig) ilSlotRetTy)

    let iLRet = mkILReturn ilRetTy
    ilParams, iLRet

and GenNameOfOverridingMethod cenv (useMethodImpl, slotsig) =
    let (TSlotSig (nameOfOverridenMethod, enclTypOfOverridenMethod, _, _, _, _)) =
        slotsig

    if useMethodImpl then
        qualifiedInterfaceImplementationName cenv.g enclTypOfOverridenMethod nameOfOverridenMethod
    else
        nameOfOverridenMethod

and GenMethodImpl cenv eenv (useMethodImpl, slotsig) m =
    let ilOverridesSpec = GenOverridesSpec cenv eenv slotsig m

    let nameOfOverridingMethod = GenNameOfOverridingMethod cenv (useMethodImpl, slotsig)

    nameOfOverridingMethod,
    (fun (ilTyForOverriding, methTyparsOfOverridingMethod) ->
        let eenvForOverrideBy = AddTyparsToEnv methTyparsOfOverridingMethod eenv

        let ilParamsOfOverridingMethod, ilReturnOfOverridingMethod =
            GenActualSlotsig m cenv eenvForOverrideBy slotsig methTyparsOfOverridingMethod []

        let ilOverrideMethGenericParams =
            GenGenericParams cenv eenvForOverrideBy methTyparsOfOverridingMethod

        let ilOverrideMethGenericArgs = mkILFormalGenericArgs 0 ilOverrideMethGenericParams

        let ilOverrideBy =
            mkILInstanceMethSpecInTy (
                ilTyForOverriding,
                nameOfOverridingMethod,
                typesOfILParams ilParamsOfOverridingMethod,
                ilReturnOfOverridingMethod.Type,
                ilOverrideMethGenericArgs
            )

        {
            Overrides = ilOverridesSpec
            OverrideBy = ilOverrideBy
        })

and bindBaseOrThisVarOpt cenv eenv baseValOpt =
    match baseValOpt with
    | None -> eenv
    | Some basev -> AddStorageForVal cenv.g (basev, notlazy (Arg 0)) eenv

and fixupVirtualSlotFlags (mdef: ILMethodDef) = mdef.WithHideBySig()

and renameMethodDef nameOfOverridingMethod (mdef: ILMethodDef) =
    mdef.With(name = nameOfOverridingMethod)

and fixupMethodImplFlags (mdef: ILMethodDef) =
    mdef.WithAccess(ILMemberAccess.Private).WithHideBySig().WithFinal(
        true
    )
        .WithNewSlot

and GenObjectMethod cenv eenvinner (cgbuf: CodeGenBuffer) useMethodImpl tmethod =
    let g = cenv.g

    let (TObjExprMethod (slotsig, attribs, methTyparsOfOverridingMethod, methParams, methBodyExpr, m)) =
        tmethod

    let (TSlotSig (nameOfOverridenMethod, _, _, _, _, _)) = slotsig

    // Check if we're compiling the property as a .NET event
    if CompileAsEvent g attribs then
        []
    else
        let eenvUnderTypars = AddTyparsToEnv methTyparsOfOverridingMethod eenvinner
        let methParams = List.concat methParams

        // drop the 'this' arg when computing better argument names for IL parameters
        let selfArgOpt, methParamsNonSelf =
            match methParams with
            | [] -> None, []
            | h :: t -> Some h, t

        let ilParamsOfOverridingMethod, ilReturnOfOverridingMethod =
            GenActualSlotsig m cenv eenvUnderTypars slotsig methTyparsOfOverridingMethod methParamsNonSelf

        let ilAttribs = GenAttrs cenv eenvinner attribs

        // Args are stored starting at #0, the args include the self parameter
        let eenvForMeth =
            AddStorageForLocalVals g (methParams |> List.mapi (fun i v -> (v, Arg i))) eenvUnderTypars

        let sequel =
            (if slotSigHasVoidReturnTy slotsig then
                 discardAndReturnVoid
             else
                 Return)

        let ilMethodBody =
            CodeGenMethodForExpr cenv cgbuf.mgbuf ([], nameOfOverridenMethod, eenvForMeth, 0, selfArgOpt, methBodyExpr, sequel)

        let nameOfOverridingMethod, methodImplGenerator =
            GenMethodImpl cenv eenvinner (useMethodImpl, slotsig) methBodyExpr.Range

        let mdef =
            mkILGenericVirtualMethod (
                nameOfOverridingMethod,
                ILMemberAccess.Public,
                GenGenericParams cenv eenvUnderTypars methTyparsOfOverridingMethod,
                ilParamsOfOverridingMethod,
                ilReturnOfOverridingMethod,
                MethodBody.IL(lazy ilMethodBody)
            )
        // fixup attributes to generate a method impl
        let mdef = if useMethodImpl then fixupMethodImplFlags mdef else mdef
        let mdef = fixupVirtualSlotFlags mdef
        let mdef = mdef.With(customAttrs = mkILCustomAttrs ilAttribs)
        [ (useMethodImpl, methodImplGenerator, methTyparsOfOverridingMethod), mdef ]

and GenStructStateMachine cenv cgbuf eenvouter (res: LoweredStateMachine) sequel =

    let (LoweredStateMachine (templateStructTy,
                              dataTy,
                              stateVars,
                              thisVars,
                              (moveNextThisVar, moveNextBody),
                              (setStateMachineThisVar, setStateMachineStateVar, setStateMachineBody),
                              (afterCodeThisVar, afterCodeBody))) =
        res

    let m = moveNextBody.Range
    let g = cenv.g
    let amap = cenv.amap

    let stateVarsSet =
        stateVars |> List.map (fun vref -> vref.Deref) |> Zset.ofList valOrder

    // Find the free variables of the closure, to make them further fields of the object.
    let cloinfo, _, eenvinner =
        // State vars are only populated for state machine objects
        //
        // Like in GenSequenceExpression we pretend any stateVars and the stateMachineVar are bound in the outer environment. This prevents the being
        // considered true free variables that need to be passed to the constructor.
        //
        // Note, the 'let' bindings for the stateVars have already been transformed to 'set' expressions, and thus the stateVars are now
        // free variables of the expression.
        let eenvouter =
            eenvouter
            |> AddStorageForLocalVals g (stateVars |> List.map (fun v -> v.Deref, Local(0, false, None)))

        let eenvouter =
            eenvouter
            |> AddStorageForLocalVals g (thisVars |> List.map (fun v -> v.Deref, Local(0, false, None)))

        let eenvouter =
            eenvouter
            |> AddStorageForLocalVals g [ (moveNextThisVar, Local(0, false, None)) ]

        GetIlxClosureInfo cenv m ILBoxity.AsValue false false (mkLocalValRef moveNextThisVar :: thisVars) eenvouter moveNextBody

    let cloFreeVars = cloinfo.cloFreeVars

    let ilCloFreeVars = cloinfo.ilCloAllFreeVars
    let ilCloGenericFormals = cloinfo.cloILGenericParams
    let ilCloGenericActuals = cloinfo.cloSpec.GenericArgs
    let ilCloTypeRef = cloinfo.cloSpec.TypeRef
    let ilCloTy = mkILValueTy ilCloTypeRef ilCloGenericActuals

    // The closure implements what ever interfaces the template implements.
    let interfaceTys =
        GetImmediateInterfacesOfType SkipUnrefInterfaces.Yes g cenv.amap m templateStructTy

    let ilInterfaceTys = List.map (GenType cenv m eenvinner.tyenv) interfaceTys

    let super = g.iltyp_ValueType

    let templateTyconRef, templateTypeArgs = destAppTy g templateStructTy
    let templateTypeInst = mkTyconRefInst templateTyconRef templateTypeArgs

    let eenvinner =
        AddTemplateReplacement eenvinner (templateTyconRef, ilCloTypeRef, cloinfo.cloFreeTyvars, templateTypeInst)

    let infoReader = InfoReader.InfoReader(g, cenv.amap)

    // We codegen the IResumableStateMachine implementation for each generated struct type
    let getResumptionPointThisVar, getResumptionPointBody =
        let fieldName = "ResumptionPoint"
        let thisVar = moveNextThisVar // reusing the this var from the MoveNext implementation

        let finfo =
            match
                infoReader.GetRecordOrClassFieldsOfType(
                    Some fieldName,
                    AccessibilityLogic.AccessorDomain.AccessibleFromSomewhere,
                    m,
                    templateStructTy
                )
            with
            | [ finfo ] -> finfo
            | _ -> error (InternalError(sprintf "expected class field %s not found" fieldName, m))

        thisVar, mkRecdFieldGetViaExprAddr (exprForVal m thisVar, finfo.RecdFieldRef, finfo.TypeInst, m)

    let (getDataThisVar, getDataBody), (setDataThisVar, setDataValueVar, setDataBody) =
        let fieldName = "Data"
        let thisVar = moveNextThisVar // reusing the this var from the MoveNext implementation
        let setDataValueVar, setDataValueExpr = mkCompGenLocal m "value" dataTy

        let finfo =
            match
                infoReader.GetRecordOrClassFieldsOfType(
                    Some fieldName,
                    AccessibilityLogic.AccessorDomain.AccessibleFromSomewhere,
                    m,
                    templateStructTy
                )
            with
            | [ finfo ] -> finfo
            | _ -> error (InternalError(sprintf "expected class field %s not found" fieldName, m))

        (thisVar, mkRecdFieldGetViaExprAddr (exprForVal m thisVar, finfo.RecdFieldRef, finfo.TypeInst, m)),
        (thisVar, setDataValueVar, mkRecdFieldSetViaExprAddr (exprForVal m thisVar, finfo.RecdFieldRef, finfo.TypeInst, setDataValueExpr, m))

    let methods =
        [
            ((mkLocalValRef moveNextThisVar :: thisVars), [], g.mk_IAsyncStateMachine_ty, "MoveNext", moveNextBody)
            ([ mkLocalValRef setStateMachineThisVar ],
             [ setStateMachineStateVar ],
             g.mk_IAsyncStateMachine_ty,
             "SetStateMachine",
             setStateMachineBody)
            ([ mkLocalValRef getResumptionPointThisVar ],
             [],
             g.mk_IResumableStateMachine_ty dataTy,
             "get_ResumptionPoint",
             getResumptionPointBody)
            ([ mkLocalValRef getDataThisVar ], [], g.mk_IResumableStateMachine_ty dataTy, "get_Data", getDataBody)
            ([ mkLocalValRef setDataThisVar ], [ setDataValueVar ], g.mk_IResumableStateMachine_ty dataTy, "set_Data", setDataBody)
        ]

    let mdefs =
        [
            for thisVals, argVals, interfaceTy, imethName, bodyR in methods do
                let eenvinner = eenvinner |> AddStorageForLocalVals g [ (moveNextThisVar, Arg 0) ]
                let m = bodyR.Range

                let implementedMeth =
                    match
                        InfoReader.TryFindIntrinsicMethInfo
                            infoReader
                            m
                            AccessibilityLogic.AccessorDomain.AccessibleFromSomewhere
                            imethName
                            interfaceTy
                    with
                    | [ meth ] when meth.IsInstance -> meth
                    | _ -> error (InternalError(sprintf "expected method %s not found" imethName, m))

                let argTys = implementedMeth.GetParamTypes(cenv.amap, m, []) |> List.concat
                let retTy = implementedMeth.GetCompiledReturnType(cenv.amap, m, [])
                let ilRetTy = GenReturnType cenv m eenvinner.tyenv retTy
                let ilArgTys = argTys |> GenTypes cenv m eenvinner.tyenv

                if ilArgTys.Length <> argVals.Length then
                    error (
                        InternalError(
                            sprintf "expected method arg count of %d, got %d for method %s" argVals.Length ilArgTys.Length imethName,
                            m
                        )
                    )

                let eenvinner =
                    eenvinner
                    |> AddStorageForLocalVals g (thisVals |> List.map (fun v -> (v.Deref, Arg 0)))

                let eenvinner =
                    eenvinner
                    |> AddStorageForLocalVals g (argVals |> List.mapi (fun i v -> v, Arg(i + 1)))

                let sequel = if retTy.IsNone then discardAndReturnVoid else Return

                let ilCode =
                    CodeGenMethodForExpr cenv cgbuf.mgbuf ([], imethName, eenvinner, 1 + argVals.Length, None, bodyR, sequel)

                let ilParams =
                    (ilArgTys, argVals)
                    ||> List.map2 (fun ty v -> mkILParamNamed (v.LogicalName, ty))

                mkILNonGenericVirtualMethod (imethName, ILMemberAccess.Public, ilParams, mkILReturn ilRetTy, MethodBody.IL(notlazy ilCode))
        ]

    let mimpls =
        [
            for (_thisVals, _argVals, interfaceTy, imethName, bodyR), mdef in (List.zip methods mdefs) do
                let m = bodyR.Range

                let implementedMeth =
                    match
                        InfoReader.TryFindIntrinsicMethInfo
                            infoReader
                            m
                            AccessibilityLogic.AccessorDomain.AccessibleFromSomewhere
                            imethName
                            interfaceTy
                    with
                    | [ meth ] when meth.IsInstance -> meth
                    | _ -> error (InternalError(sprintf "expected method %s not found" imethName, m))

                let slotsig = implementedMeth.GetSlotSig(amap, m)
                let ilOverridesSpec = GenOverridesSpec cenv eenvinner slotsig m

                let ilOverrideBy =
                    mkILInstanceMethSpecInTy (ilCloTy, imethName, mdef.ParameterTypes, mdef.Return.Type, [])

                {
                    Overrides = ilOverridesSpec
                    OverrideBy = ilOverrideBy
                }
        ]

    let fdefs =
        [ // Fields copied from the template struct
            for templateFld in
                infoReader.GetRecordOrClassFieldsOfType(
                    None,
                    AccessibilityLogic.AccessorDomain.AccessibleFromSomewhere,
                    m,
                    templateStructTy
                ) do
                // Suppress the "ResumptionDynamicInfo" from generated state machines
                if templateFld.LogicalName <> "ResumptionDynamicInfo" then
                    let access = ComputeMemberAccess false
                    let fty = GenType cenv m eenvinner.tyenv templateFld.FieldType

                    let fdef =
                        ILFieldDef(
                            name = templateFld.LogicalName,
                            fieldType = fty,
                            attributes = enum 0,
                            data = None,
                            literalValue = None,
                            offset = None,
                            marshal = None,
                            customAttrs = mkILCustomAttrs []
                        )
                            .WithAccess(access)
                            .WithStatic(false)

                    yield fdef

            // Fields for captured variables
            for ilCloFreeVar in ilCloFreeVars do
                let access = ComputeMemberAccess false

                let fdef =
                    ILFieldDef(
                        name = ilCloFreeVar.fvName,
                        fieldType = ilCloFreeVar.fvType,
                        attributes = enum 0,
                        data = None,
                        literalValue = None,
                        offset = None,
                        marshal = None,
                        customAttrs = mkILCustomAttrs []
                    )
                        .WithAccess(access)
                        .WithStatic(false)

                yield fdef
        ]

    let cloTypeDef =
        ILTypeDef(
            name = ilCloTypeRef.Name,
            layout = ILTypeDefLayout.Auto,
            attributes = enum 0,
            genericParams = ilCloGenericFormals,
            customAttrs =
                mkILCustomAttrs (
                    [
                        g.CompilerGeneratedAttribute
                        mkCompilationMappingAttr g (int SourceConstructFlags.Closure)
                    ]
                ),
            fields = mkILFields fdefs,
            events = emptyILEvents,
            properties = emptyILProperties,
            methods = mkILMethods mdefs,
            methodImpls = mkILMethodImpls mimpls,
            nestedTypes = emptyILTypeDefs,
            implements = ilInterfaceTys,
            extends = Some super,
            isKnownToBeAttribute = false,
            securityDecls = emptyILSecurityDecls
        )
            .WithSealed(true)
            .WithSpecialName(true)
            .WithAccess(ComputeTypeAccess ilCloTypeRef true)
            .WithLayout(ILTypeDefLayout.Auto)
            .WithEncoding(ILDefaultPInvokeEncoding.Auto)
            .WithInitSemantics(ILTypeInit.BeforeField)

    cgbuf.mgbuf.AddTypeDef(ilCloTypeRef, cloTypeDef, false, false, None)

    CountClosure()

    LocalScope "machine" cgbuf (fun scopeMarks ->
        let eenvouter =
            AddTemplateReplacement eenvouter (templateTyconRef, ilCloTypeRef, cloinfo.cloFreeTyvars, templateTypeInst)

        let ilMachineAddrTy = ILType.Byref ilCloTy

        // The local for the state machine
        let locIdx, realloc, _ =
            AllocLocal
                cenv
                cgbuf
                eenvouter
                true
                (g.CompilerGlobalState.Value.IlxGenNiceNameGenerator.FreshCompilerGeneratedName("machine", m), ilCloTy, false)
                scopeMarks

        // The local for the state machine address
        let locIdx2, _realloc2, _ =
            AllocLocal
                cenv
                cgbuf
                eenvouter
                true
                (g.CompilerGlobalState.Value.IlxGenNiceNameGenerator.FreshCompilerGeneratedName(afterCodeThisVar.DisplayName, m),
                 ilMachineAddrTy,
                 false)
                scopeMarks

        let eenvouter =
            eenvouter
            |> AddStorageForLocalVals g [ (afterCodeThisVar, Local(locIdx2, realloc, None)) ]

        // Zero-initialize the machine
        EmitInitLocal cgbuf ilCloTy locIdx

        // Initialize the address-of-machine local
        CG.EmitInstr cgbuf (pop 0) (Push [ ilMachineAddrTy ]) (I_ldloca(uint16 locIdx))
        CG.EmitInstr cgbuf (pop 1) (Push []) (I_stloc(uint16 locIdx2))

        // Initialize the closure variables
        for fv, ilv in Seq.zip cloFreeVars cloinfo.ilCloAllFreeVars do
            if stateVarsSet.Contains fv then
                // zero-initialize the state var
                if realloc then
                    CG.EmitInstr cgbuf (pop 0) (Push [ ilMachineAddrTy ]) (I_ldloc(uint16 locIdx2))
                    GenDefaultValue cenv cgbuf eenvouter (fv.Type, m)
                    CG.EmitInstr cgbuf (pop 2) (Push []) (mkNormalStfld (mkILFieldSpecInTy (ilCloTy, ilv.fvName, ilv.fvType)))
            else
                // initialize the captured var
                CG.EmitInstr cgbuf (pop 0) (Push [ ilMachineAddrTy ]) (I_ldloc(uint16 locIdx2))
                GenGetLocalVal cenv cgbuf eenvouter m fv None
                CG.EmitInstr cgbuf (pop 2) (Push []) (mkNormalStfld (mkILFieldSpecInTy (ilCloTy, ilv.fvName, ilv.fvType)))

        // Generate the start expression
        GenExpr cenv cgbuf eenvouter afterCodeBody sequel

    )

and GenObjectExpr cenv cgbuf eenvouter objExpr (baseType, baseValOpt, basecall, overrides, interfaceImpls, m) sequel =
    let g = cenv.g

    // Find the free variables of the closure, to make them further fields of the object.
    //
    // Note, the 'let' bindings for the stateVars have already been transformed to 'set' expressions, and thus the stateVars are now
    // free variables of the expression.
    let cloinfo, _, eenvinner =
        GetIlxClosureInfo cenv m ILBoxity.AsObject false false [] eenvouter objExpr

    let ilCloLambdas = cloinfo.ilCloLambdas
    let cloName = cloinfo.cloName
    let cloSpec = cloinfo.cloSpec

    let ilCloAllFreeVars = cloinfo.ilCloAllFreeVars
    let ilCloGenericFormals = cloinfo.cloILGenericParams
    let ilCloGenericActuals = cloinfo.cloSpec.GenericArgs
    let ilCloRetTy = cloinfo.ilCloFormalReturnTy
    let ilCloTypeRef = cloSpec.TypeRef
    let ilTyForOverriding = mkILBoxedTy ilCloTypeRef ilCloGenericActuals

    let eenvinner = bindBaseOrThisVarOpt cenv eenvinner baseValOpt

    let ilCtorBody =
        CodeGenMethodForExpr cenv cgbuf.mgbuf ([], cloName, eenvinner, 1, None, basecall, discardAndReturnVoid)

    let genMethodAndOptionalMethodImpl tmethod useMethodImpl =
        [
            for (useMethodImpl, methodImplGeneratorFunction, methTyparsOfOverridingMethod), mdef in
                GenObjectMethod cenv eenvinner cgbuf useMethodImpl tmethod do
                let mimpl =
                    (if useMethodImpl then
                         Some(methodImplGeneratorFunction (ilTyForOverriding, methTyparsOfOverridingMethod))
                     else
                         None)

                yield (mimpl, mdef)
        ]

    let mimpls, mdefs =
        [
            for ov in overrides do
                yield! genMethodAndOptionalMethodImpl ov (isInterfaceTy g baseType)
            for _, tmethods in interfaceImpls do
                for tmethod in tmethods do
                    yield! genMethodAndOptionalMethodImpl tmethod true
        ]
        |> List.unzip

    let mimpls = mimpls |> List.choose id // choose the ones that actually have method impls

    let interfaceTys =
        interfaceImpls |> List.map (fst >> GenType cenv m eenvinner.tyenv)

    let super =
        (if isInterfaceTy g baseType then
             g.ilg.typ_Object
         else
             ilCloRetTy)

    let interfaceTys =
        interfaceTys @ (if isInterfaceTy g baseType then [ ilCloRetTy ] else [])

    let cloTypeDefs =
        GenClosureTypeDefs
            cenv
            (ilCloTypeRef,
             ilCloGenericFormals,
             [],
             ilCloAllFreeVars,
             ilCloLambdas,
             ilCtorBody,
             mdefs,
             mimpls,
             super,
             interfaceTys,
             Some cloinfo.cloSpec)

    for cloTypeDef in cloTypeDefs do
        cgbuf.mgbuf.AddTypeDef(ilCloTypeRef, cloTypeDef, false, false, None)

    CountClosure()
    GenWitnessArgsFromWitnessInfos cenv cgbuf eenvouter m cloinfo.cloWitnessInfos

    for fv in cloinfo.cloFreeVars do
        GenGetLocalVal cenv cgbuf eenvouter m fv None

    CG.EmitInstr
        cgbuf
        (pop ilCloAllFreeVars.Length)
        (Push [ EraseClosures.mkTyOfLambdas cenv.ilxPubCloEnv ilCloLambdas ])
        (I_newobj(cloSpec.Constructor, None))

    GenSequel cenv eenvouter.cloc cgbuf sequel

and GenSequenceExpr
    cenv
    (cgbuf: CodeGenBuffer)
    eenvouter
    (nextEnumeratorValRef: ValRef,
     pcvref: ValRef,
     currvref: ValRef,
     stateVars,
     generateNextExpr,
     closeExpr,
     checkCloseExpr: Expr,
     seqElemTy,
     m)
    sequel
    =

    let g = cenv.g
    let stateVars = [ pcvref; currvref ] @ stateVars

    let stateVarsSet =
        stateVars |> List.map (fun vref -> vref.Deref) |> Zset.ofList valOrder

    // pretend that the state variables are bound
    let eenvouter =
        eenvouter
        |> AddStorageForLocalVals g (stateVars |> List.map (fun v -> v.Deref, Local(0, false, None)))

    // Get the free variables. Make a lambda to pretend that the 'nextEnumeratorValRef' is bound (it is an argument to GenerateNext)
    let (cloFreeTyvars, cloWitnessInfos, cloFreeVars, ilCloTypeRef: ILTypeRef, ilCloAllFreeVars, eenvinner) =
        GetIlxClosureFreeVars
            cenv
            m
            []
            ILBoxity.AsObject
            eenvouter
            []
            (mkLambda m nextEnumeratorValRef.Deref (generateNextExpr, g.int32_ty))

    let ilCloSeqElemTy = GenType cenv m eenvinner.tyenv seqElemTy
    let cloRetTy = mkSeqTy g seqElemTy
    let ilCloRetTyInner = GenType cenv m eenvinner.tyenv cloRetTy
    let ilCloRetTyOuter = GenType cenv m eenvouter.tyenv cloRetTy
    let ilCloEnumeratorTy = GenType cenv m eenvinner.tyenv (mkIEnumeratorTy g seqElemTy)
    let ilCloEnumerableTy = GenType cenv m eenvinner.tyenv (mkSeqTy g seqElemTy)

    let ilCloBaseTy =
        GenType cenv m eenvinner.tyenv (g.mk_GeneratedSequenceBase_ty seqElemTy)

    let ilCloGenericParams = GenGenericParams cenv eenvinner cloFreeTyvars

    // Create a new closure class with a single "MoveNext" method that implements the iterator.
    let ilCloTyInner = mkILFormalBoxedTy ilCloTypeRef ilCloGenericParams
    let ilCloLambdas = Lambdas_return ilCloRetTyInner
    let cloref = IlxClosureRef(ilCloTypeRef, ilCloLambdas, ilCloAllFreeVars)

    let ilxCloSpec =
        IlxClosureSpec.Create(cloref, GenGenericArgs m eenvouter.tyenv cloFreeTyvars, false)

    let formalClospec =
        IlxClosureSpec.Create(cloref, mkILFormalGenericArgs 0 ilCloGenericParams, false)

    let getFreshMethod =
        let _, mbody =
            CodeGenMethod
                cenv
                cgbuf.mgbuf
                ([],
                 "GetFreshEnumerator",
                 eenvinner,
                 1,
                 None,
                 (fun cgbuf eenv ->
                     GenWitnessArgsFromWitnessInfos cenv cgbuf eenv m cloWitnessInfos

                     for fv in cloFreeVars do
                         // State variables always get zero-initialized
                         if stateVarsSet.Contains fv then
                             GenDefaultValue cenv cgbuf eenv (fv.Type, m)
                         else
                             GenGetLocalVal cenv cgbuf eenv m fv None

                     CG.EmitInstr cgbuf (pop ilCloAllFreeVars.Length) (Push [ ilCloRetTyInner ]) (I_newobj(formalClospec.Constructor, None))
                     GenSequel cenv eenv.cloc cgbuf Return),
                 m)

        mkILNonGenericVirtualMethod (
            "GetFreshEnumerator",
            ILMemberAccess.Public,
            [],
            mkILReturn ilCloEnumeratorTy,
            MethodBody.IL(lazy mbody)
        )
        |> AddNonUserCompilerGeneratedAttribs g

    let closeMethod =
        let ilCode =
            CodeGenMethodForExpr cenv cgbuf.mgbuf ([], "Close", eenvinner, 1, None, closeExpr, discardAndReturnVoid)

        mkILNonGenericVirtualMethod ("Close", ILMemberAccess.Public, [], mkILReturn ILType.Void, MethodBody.IL(lazy ilCode))

    let checkCloseMethod =
        let ilCode =
            CodeGenMethodForExpr cenv cgbuf.mgbuf ([], "get_CheckClose", eenvinner, 1, None, checkCloseExpr, Return)

        mkILNonGenericVirtualMethod ("get_CheckClose", ILMemberAccess.Public, [], mkILReturn g.ilg.typ_Bool, MethodBody.IL(lazy ilCode))

    let generateNextMethod =
        // the 'next enumerator' byref arg is at arg position 1
        let eenvinner =
            eenvinner |> AddStorageForLocalVals g [ (nextEnumeratorValRef.Deref, Arg 1) ]

        let ilParams = [ mkILParamNamed ("next", ILType.Byref ilCloEnumerableTy) ]
        let ilReturn = mkILReturn g.ilg.typ_Int32

        let ilCode =
            MethodBody.IL(lazy (CodeGenMethodForExpr cenv cgbuf.mgbuf ([], "GenerateNext", eenvinner, 2, None, generateNextExpr, Return)))

        mkILNonGenericVirtualMethod ("GenerateNext", ILMemberAccess.Public, ilParams, ilReturn, ilCode)

    let lastGeneratedMethod =
        let ilCode =
            CodeGenMethodForExpr cenv cgbuf.mgbuf ([], "get_LastGenerated", eenvinner, 1, None, exprForValRef m currvref, Return)

        mkILNonGenericVirtualMethod ("get_LastGenerated", ILMemberAccess.Public, [], mkILReturn ilCloSeqElemTy, MethodBody.IL(lazy ilCode))
        |> AddNonUserCompilerGeneratedAttribs g

    let ilCtorBody =
        mkILSimpleStorageCtor(
            Some ilCloBaseTy.TypeSpec,
            ilCloTyInner,
            [],
            [],
            ILMemberAccess.Assembly,
            None,
            eenvouter.imports
        )
            .MethodBody

    let cloMethods =
        [
            generateNextMethod
            closeMethod
            checkCloseMethod
            lastGeneratedMethod
            getFreshMethod
        ]

    let cloTypeDefs =
        GenClosureTypeDefs
            cenv
            (ilCloTypeRef,
             ilCloGenericParams,
             [],
             ilCloAllFreeVars,
             ilCloLambdas,
             ilCtorBody,
             cloMethods,
             [],
             ilCloBaseTy,
             [],
             Some ilxCloSpec)

    for cloTypeDef in cloTypeDefs do
        cgbuf.mgbuf.AddTypeDef(ilCloTypeRef, cloTypeDef, false, false, None)

    CountClosure()

    GenWitnessArgsFromWitnessInfos cenv cgbuf eenvouter m cloWitnessInfos

    for fv in cloFreeVars do
        // State variables always get zero-initialized
        if stateVarsSet.Contains fv then
            GenDefaultValue cenv cgbuf eenvouter (fv.Type, m)
        else
            GenGetLocalVal cenv cgbuf eenvouter m fv None

    CG.EmitInstr cgbuf (pop ilCloAllFreeVars.Length) (Push [ ilCloRetTyOuter ]) (I_newobj(ilxCloSpec.Constructor, None))
    GenSequel cenv eenvouter.cloc cgbuf sequel

/// Generate the class for a closure type definition
and GenClosureTypeDefs
    cenv
    (tref: ILTypeRef,
     ilGenParams,
     attrs,
     ilCloAllFreeVars,
     ilCloLambdas,
     ilCtorBody,
     mdefs,
     mimpls,
     ext,
     ilIntfTys,
     cloSpec: IlxClosureSpec option)
    =
    let g = cenv.g

    let cloInfo =
        {
            cloFreeVars = ilCloAllFreeVars
            cloStructure = ilCloLambdas
            cloCode = notlazy ilCtorBody
            cloUseStaticField =
                (match cloSpec with
                 | None -> false
                 | Some cloSpec -> cloSpec.UseStaticField)
        }

    let mdefs, fdefs =
        if cloInfo.cloUseStaticField then
            let cloSpec = cloSpec.Value
            let cloTy = mkILFormalBoxedTy cloSpec.TypeRef (mkILFormalTypars cloSpec.GenericArgs)
            let fspec = mkILFieldSpec (cloSpec.GetStaticFieldSpec().FieldRef, cloTy)
            let ctorSpec = mkILMethSpecForMethRefInTy (cloSpec.Constructor.MethodRef, cloTy, [])
            let ilInstrs = [ I_newobj(ctorSpec, None); mkNormalStsfld fspec ]

            let ilCode =
                mkILMethodBody (true, [], 8, nonBranchingInstrsToCode ilInstrs, None, None)

            let cctor = mkILClassCtor (MethodBody.IL(notlazy ilCode))

            let ilFieldDef =
                mkILStaticField(fspec.Name, fspec.FormalType, None, None, ILMemberAccess.Assembly)
                    .WithInitOnly(true)

            (cctor :: mdefs), [ ilFieldDef ]
        else
            mdefs, []

    let tdef =
        ILTypeDef(
            name = tref.Name,
            layout = ILTypeDefLayout.Auto,
            attributes = enum 0,
            genericParams = ilGenParams,
            customAttrs = mkILCustomAttrs (attrs @ [ mkCompilationMappingAttr g (int SourceConstructFlags.Closure) ]),
            fields = mkILFields fdefs,
            events = emptyILEvents,
            properties = emptyILProperties,
            methods = mkILMethods mdefs,
            methodImpls = mkILMethodImpls mimpls,
            nestedTypes = emptyILTypeDefs,
            implements = ilIntfTys,
            extends = Some ext,
            isKnownToBeAttribute = false,
            securityDecls = emptyILSecurityDecls
        )
            .WithSealed(true)
            .WithSerializable(true)
            .WithSpecialName(true)
            .WithAccess(ComputeTypeAccess tref true)
            .WithLayout(ILTypeDefLayout.Auto)
            .WithEncoding(ILDefaultPInvokeEncoding.Auto)
            .WithInitSemantics(ILTypeInit.BeforeField)

    let tdefs =
        EraseClosures.convIlxClosureDef cenv.ilxPubCloEnv tref.Enclosing tdef cloInfo

    tdefs

and GenStaticDelegateClosureTypeDefs
    cenv
    (tref: ILTypeRef, ilGenParams, attrs, ilCloAllFreeVars, ilCloLambdas, ilCtorBody, mdefs, mimpls, ext, ilIntfTys, staticCloInfo)
    =
    let tdefs =
        GenClosureTypeDefs
            cenv
            (tref, ilGenParams, attrs, ilCloAllFreeVars, ilCloLambdas, ilCtorBody, mdefs, mimpls, ext, ilIntfTys, staticCloInfo)

    // Apply the abstract attribute, turning the sealed class into abstract sealed (i.e. static class).
    // Remove the redundant constructor.
    tdefs
    |> List.map (fun td ->
        td
            .WithAbstract(true)
            .With(methods = mkILMethodsFromArray (td.Methods.AsArray() |> Array.filter (fun m -> not m.IsConstructor))))

and GenGenericParams cenv eenv tps =
    tps |> DropErasedTypars |> List.map (GenGenericParam cenv eenv)

and GenGenericArgs m (tyenv: TypeReprEnv) tps =
    tps |> DropErasedTypars |> List.map (fun c -> (mkILTyvarTy tyenv[c, m]))

/// Generate a local type function contract class and implementation
and GenClosureAsLocalTypeFunction cenv (cgbuf: CodeGenBuffer) eenv thisVars expr m =
    let g = cenv.g

    let cloinfo, body, eenvinner =
        GetIlxClosureInfo cenv m ILBoxity.AsObject true true thisVars eenv expr

    let ilCloTypeRef = cloinfo.cloSpec.TypeRef

    let entryPointInfo =
        thisVars |> List.map (fun v -> (v, BranchCallClosure cloinfo.cloArityInfo))
    // Now generate the actual closure implementation w.r.t. eenvinner
    let directTypars, ilDirectWitnessParams, _directWitnessInfos, eenvinner =
        AddDirectTyparWitnessParams cenv eenvinner cloinfo m

    let ilDirectGenericParams = GenGenericParams cenv eenvinner directTypars

    // The type-lambdas are dealt with by the local type function
    let ilCloFormalReturnTy, ilCloLambdas =
        let rec strip lambdas =
            match lambdas with
            | Lambdas_forall (_, r) -> strip r
            | Lambdas_return returnTy -> returnTy, lambdas
            | _ -> failwith "AdjustNamedLocalTypeFuncIlxClosureInfo: local functions can currently only be type functions"

        strip cloinfo.ilCloLambdas

    let ilCloBody =
        CodeGenMethodForExpr cenv cgbuf.mgbuf (entryPointInfo, cloinfo.cloName, eenvinner, 1, None, body, Return)

    let ilCtorBody =
        mkILMethodBody (true, [], 8, nonBranchingInstrsToCode (mkCallBaseConstructor (g.ilg.typ_Object, [])), None, eenv.imports)

    let cloMethods =
        [
            mkILGenericVirtualMethod (
                "DirectInvoke",
                ILMemberAccess.Assembly,
                ilDirectGenericParams,
                ilDirectWitnessParams,
                mkILReturn ilCloFormalReturnTy,
                MethodBody.IL(lazy ilCloBody)
            )
        ]

    let cloTypeDefs =
        GenClosureTypeDefs
            cenv
            (ilCloTypeRef,
             cloinfo.cloILGenericParams,
             [],
             cloinfo.ilCloAllFreeVars,
             ilCloLambdas,
             ilCtorBody,
             cloMethods,
             [],
             g.ilg.typ_Object,
             [],
             Some cloinfo.cloSpec)

    cloinfo, ilCloTypeRef, cloTypeDefs

and GenClosureAsFirstClassFunction cenv (cgbuf: CodeGenBuffer) eenv thisVars m expr =
    let g = cenv.g

    let cloinfo, body, eenvinner =
        GetIlxClosureInfo cenv m ILBoxity.AsObject false true thisVars eenv expr

    let entryPointInfo =
        thisVars |> List.map (fun v -> (v, BranchCallClosure(cloinfo.cloArityInfo)))

    let ilCloTypeRef = cloinfo.cloSpec.TypeRef

    let ilCloBody =
        CodeGenMethodForExpr cenv cgbuf.mgbuf (entryPointInfo, cloinfo.cloName, eenvinner, 1, None, body, Return)

    let cloTypeDefs =
        GenClosureTypeDefs
            cenv
            (ilCloTypeRef,
             cloinfo.cloILGenericParams,
             [],
             cloinfo.ilCloAllFreeVars,
             cloinfo.ilCloLambdas,
             ilCloBody,
             [],
             [],
             g.ilg.typ_Object,
             [],
             Some cloinfo.cloSpec)

    cloinfo, ilCloTypeRef, cloTypeDefs

/// Generate the closure class for a function
and GenLambdaClosure cenv (cgbuf: CodeGenBuffer) eenv isLocalTypeFunc thisVars expr =
    match expr with
    | Expr.Lambda (_, _, _, _, _, m, _)
    | Expr.TyLambda (_, _, _, m, _) ->

        let cloinfo, ilCloTypeRef, cloTypeDefs =
            if isLocalTypeFunc then
                GenClosureAsLocalTypeFunction cenv cgbuf eenv thisVars expr m
            else
                GenClosureAsFirstClassFunction cenv cgbuf eenv thisVars m expr

        CountClosure()

        for cloTypeDef in cloTypeDefs do
            cgbuf.mgbuf.AddTypeDef(ilCloTypeRef, cloTypeDef, false, false, None)

        cloinfo, m

    | _ -> failwith "GenLambda: not a lambda"

and GenClosureAlloc cenv (cgbuf: CodeGenBuffer) eenv (cloinfo, m) =
    CountClosure()

    if cloinfo.cloSpec.UseStaticField then
        let fspec = cloinfo.cloSpec.GetStaticFieldSpec()
        CG.EmitInstr cgbuf (pop 0) (Push [ EraseClosures.mkTyOfLambdas cenv.ilxPubCloEnv cloinfo.ilCloLambdas ]) (mkNormalLdsfld fspec)
    else
        GenWitnessArgsFromWitnessInfos cenv cgbuf eenv m cloinfo.cloWitnessInfos
        GenGetLocalVals cenv cgbuf eenv m cloinfo.cloFreeVars

        CG.EmitInstr
            cgbuf
            (pop cloinfo.ilCloAllFreeVars.Length)
            (Push [ EraseClosures.mkTyOfLambdas cenv.ilxPubCloEnv cloinfo.ilCloLambdas ])
            (I_newobj(cloinfo.cloSpec.Constructor, None))

and GenLambda cenv cgbuf eenv isLocalTypeFunc thisVars expr sequel =
    let cloinfo, m = GenLambdaClosure cenv cgbuf eenv isLocalTypeFunc thisVars expr
    GenClosureAlloc cenv cgbuf eenv (cloinfo, m)
    GenSequel cenv eenv.cloc cgbuf sequel

and GenTypeOfVal cenv eenv (v: Val) = GenType cenv v.Range eenv.tyenv v.Type

and GenFreevar cenv m eenvouter tyenvinner (fv: Val) =
    let g = cenv.g

    match StorageForVal m fv eenvouter with
    // Local type functions
    | Local (_, _, Some _)
    | Env (_, _, Some _) -> g.ilg.typ_Object
#if DEBUG
    // Check for things that should never make it into the free variable set. Only do this in debug for performance reasons
    | StaticPropertyWithField _
    | StaticProperty _
    | Method _
    | Null -> error (InternalError("GenFreevar: compiler error: unexpected unrealized value", fv.Range))
#endif
    | _ -> GenType cenv m tyenvinner fv.Type

and GetIlxClosureFreeVars cenv m (thisVars: ValRef list) boxity eenvouter takenNames expr =
    let g = cenv.g

    // Choose a base name for the closure
    let basename =
        let boundv =
            eenvouter.letBoundVars |> List.tryFind (fun v -> not v.IsCompilerGenerated)

        match boundv with
        | Some v -> v.CompiledName cenv.g.CompilerGlobalState
        | None -> "clo"

    // Get a unique stamp for the closure. This must be stable for things that can be part of a let rec.
    let uniq =
        match expr with
        | Expr.Obj (uniq, _, _, _, _, _, _)
        | Expr.Lambda (uniq, _, _, _, _, _, _)
        | Expr.TyLambda (uniq, _, _, _, _) -> uniq
        | _ -> newUnique ()

    // Choose a name for the closure
    let ilCloTypeRef =
        // FSharp 1.0 bug 3404: System.Reflection doesn't like '.' and '`' in type names
        let basenameSafeForUseAsTypename = CleanUpGeneratedTypeName basename
        let suffixmark = expr.Range

        let cloName =
            // Ensure that we have an g.CompilerGlobalState
            assert (g.CompilerGlobalState |> Option.isSome)
            g.CompilerGlobalState.Value.StableNameGenerator.GetUniqueCompilerGeneratedName(basenameSafeForUseAsTypename, suffixmark, uniq)

        NestedTypeRefForCompLoc eenvouter.cloc cloName

    // Collect the free variables of the closure
    let cloFreeVarResults = freeInExpr (CollectTyparsAndLocalsWithStackGuard()) expr

    // Partition the free variables when some can be accessed from places besides the immediate environment
    // Also filter out the current value being bound, if any, as it is available from the "this"
    // pointer which gives the current closure itself. This is in the case e.g. let rec f = ... f ...
    let freeLocals = cloFreeVarResults.FreeLocals |> Zset.elements

    let cloFreeVars =
        freeLocals
        |> List.filter (fun fv ->
            (thisVars |> List.forall (fun v -> not (valRefEq g (mkLocalValRef fv) v)))
            && (match StorageForVal m fv eenvouter with
                | StaticPropertyWithField _
                | StaticProperty _
                | Method _
                | Null -> false
                | _ -> true))

    // Any closure using values represented as local type functions also captures the type variables captured
    // by that local type function
    let cloFreeTyvars =
        (cloFreeVarResults.FreeTyvars, freeLocals)
        ||> List.fold (fun ftyvs fv ->
            match StorageForVal m fv eenvouter with
            | Env (_, _, Some (moreFtyvs, _))
            | Local (_, _, Some (moreFtyvs, _)) -> unionFreeTyvars ftyvs moreFtyvs
            | _ -> ftyvs)

    let cloFreeTyvars = cloFreeTyvars.FreeTypars |> Zset.elements

    let eenvinner = eenvouter |> EnvForTypars cloFreeTyvars

    let ilCloTyInner =
        let ilCloGenericParams = GenGenericParams cenv eenvinner cloFreeTyvars
        mkILFormalNamedTy boxity ilCloTypeRef ilCloGenericParams

    // If generating a named closure, add the closure itself as a var, available via "arg0" .
    // The latter doesn't apply for the delegate implementation of closures.
    // Build the environment that is active inside the closure itself
    let eenvinner =
        eenvinner
        |> AddStorageForLocalVals g (thisVars |> List.map (fun v -> (v.Deref, Arg 0)))

    // Work out if the closure captures any witnesses.
    let cloWitnessInfos =
        let generateWitnesses = ComputeGenerateWitnesses g eenvinner

        if generateWitnesses then
            // The 0 here represents that a closure doesn't reside within a generic class - there are no "enclosing class type parameters" to lop off.
            GetTraitWitnessInfosOfTypars g 0 cloFreeTyvars
        else
            []

    // Captured witnesses get captured in free variable fields
    let ilCloWitnessFreeVars, ilCloWitnessStorage =
        FreeVarStorageForWitnessInfos cenv eenvinner takenNames ilCloTyInner m cloWitnessInfos

    // Allocate storage in the environment for the witnesses
    let eenvinner = eenvinner |> AddStorageForLocalWitnesses ilCloWitnessStorage

    let ilCloFreeVars, ilCloFreeVarStorage =
        let names = cloFreeVars |> List.map nameOfVal |> ChooseFreeVarNames takenNames

        (cloFreeVars, names)
        ||> List.map2 (fun fv nm ->
            let localCloInfo =
                match StorageForVal m fv eenvouter with
                | Local (_, _, localCloInfo)
                | Env (_, _, localCloInfo) -> localCloInfo
                | _ -> None

            let ilFv =
                mkILFreeVar (nm, fv.IsCompilerGenerated, GenFreevar cenv m eenvouter eenvinner.tyenv fv)

            let storage =
                let ilField = mkILFieldSpecInTy (ilCloTyInner, ilFv.fvName, ilFv.fvType)
                Env(ilCloTyInner, ilField, localCloInfo)

            ilFv, (fv, storage))
        |> List.unzip

    let ilCloAllFreeVars = Array.ofList (ilCloWitnessFreeVars @ ilCloFreeVars)

    let eenvinner = eenvinner |> AddStorageForLocalVals g ilCloFreeVarStorage

    // Return a various results
    (cloFreeTyvars, cloWitnessInfos, cloFreeVars, ilCloTypeRef, ilCloAllFreeVars, eenvinner)

and GetIlxClosureInfo cenv m boxity isLocalTypeFunc canUseStaticField thisVars eenvouter expr =
    let g = cenv.g

    let returnTy =
        match expr with
        | Expr.Lambda (_, _, _, _, _, _, returnTy)
        | Expr.TyLambda (_, _, _, _, returnTy) -> returnTy
        | _ -> tyOfExpr g expr

    // Determine the structure of the closure. We do this before analyzing free variables to
    // determine the taken argument names.
    let tvsl, vs, body, returnTy =
        let rec getCallStructure tvacc vacc (e, ety) =
            match e with
            | Expr.TyLambda (_, tvs, body, _m, bty) -> getCallStructure ((DropErasedTypars tvs) :: tvacc) vacc (body, bty)
            | Expr.Lambda (_, _, _, vs, body, _, bty) when not isLocalTypeFunc ->
                // Transform a lambda taking untupled arguments into one
                // taking only a single tupled argument if necessary. REVIEW: do this earlier
                let tupledv, body = MultiLambdaToTupledLambda g vs body
                getCallStructure tvacc (tupledv :: vacc) (body, bty)
            | _ -> (List.rev tvacc, List.rev vacc, e, ety)

        getCallStructure [] [] (expr, returnTy)

    let takenNames = vs |> List.map (fun v -> v.CompiledName g.CompilerGlobalState)

    // Get the free variables and the information about the closure, add the free variables to the environment
    let cloFreeTyvars, cloWitnessInfos, cloFreeVars, ilCloTypeRef, ilCloAllFreeVars, eenvinner =
        GetIlxClosureFreeVars cenv m thisVars boxity eenvouter takenNames expr

    // Put the type and value arguments into the environment
    let rec getClosureArgs eenv numArgs tvsl (vs: Val list) =
        match tvsl, vs with
        | tvs :: rest, _ ->
            let eenv = AddTyparsToEnv tvs eenv
            let l, eenv = getClosureArgs eenv numArgs rest vs

            let lambdas =
                (tvs, l)
                ||> List.foldBack (fun tv sofar -> Lambdas_forall(GenGenericParam cenv eenv tv, sofar))

            lambdas, eenv
        | [], v :: rest ->
            let nm = v.CompiledName g.CompilerGlobalState

            let l, eenv =
                let eenv = AddStorageForVal g (v, notlazy (Arg numArgs)) eenv
                getClosureArgs eenv (numArgs + 1) [] rest

            let lambdas = Lambdas_lambda(mkILParamNamed (nm, GenTypeOfVal cenv eenv v), l)
            lambdas, eenv
        | _ ->
            let returnTy' = GenType cenv m eenv.tyenv returnTy
            Lambdas_return returnTy', eenv

    // start at arg number 1 as "this" pointer holds the current closure
    let ilCloLambdas, eenvinner = getClosureArgs eenvinner 1 tvsl vs

    // Arity info: one argument at each position
    let narginfo = vs |> List.map (fun _ -> 1)

    // Generate the ILX view of the lambdas
    let ilCloReturnTy = GenType cenv m eenvinner.tyenv returnTy

    /// Compute the contract if it is a local type function
    let ilCloGenericFormals = GenGenericParams cenv eenvinner cloFreeTyvars
    let ilCloGenericActuals = GenGenericArgs m eenvouter.tyenv cloFreeTyvars

    let useStaticField = canUseStaticField && (ilCloAllFreeVars.Length = 0)

    let ilxCloSpec =
        IlxClosureSpec.Create(IlxClosureRef(ilCloTypeRef, ilCloLambdas, ilCloAllFreeVars), ilCloGenericActuals, useStaticField)

    let cloinfo =
        {
            cloExpr = expr
            cloName = ilCloTypeRef.Name
            cloArityInfo = narginfo
            ilCloLambdas = ilCloLambdas
            ilCloAllFreeVars = ilCloAllFreeVars
            ilCloFormalReturnTy = ilCloReturnTy
            cloSpec = ilxCloSpec
            cloILGenericParams = ilCloGenericFormals
            cloFreeVars = cloFreeVars
            cloFreeTyvars = cloFreeTyvars
            cloWitnessInfos = cloWitnessInfos
        }

    cloinfo, body, eenvinner

/// Generate a new delegate construction including a closure class if necessary. This is a lot like generating function closures
/// and object expression closures, and most of the code is shared.
and GenDelegateExpr cenv cgbuf eenvouter expr (TObjExprMethod (slotsig, _attribs, methTyparsOfOverridingMethod, tmvs, body, _), m) sequel =
    let g = cenv.g
    let (TSlotSig (_, delegateTy, _, _, _, _)) = slotsig

    // Get the instantiation of the delegate type
    let ilCtxtDelTy = GenType cenv m eenvouter.tyenv delegateTy
    let tmvs = List.concat tmvs

    // Yuck. TLBIMP.EXE generated APIs use UIntPtr for the delegate ctor.
    let useUIntPtrForDelegateCtor =
        try
            if isILAppTy g delegateTy then
                let tcref = tcrefOfAppTy g delegateTy
                let tdef = tcref.ILTyconRawMetadata

                match tdef.Methods.FindByName ".ctor" with
                | [ ctorMDef ] ->
                    match ctorMDef.Parameters with
                    | [ _; p2 ] -> (p2.Type.TypeSpec.Name = "System.UIntPtr")
                    | _ -> false
                | _ -> false
            else
                false
        with _ ->
            false

    // Work out the free type variables for the morphing thunk
    let takenNames = List.map nameOfVal tmvs

    let cloFreeTyvars, cloWitnessInfos, cloFreeVars, ilDelegeeTypeRef, ilCloAllFreeVars, eenvinner =
        GetIlxClosureFreeVars cenv m [] ILBoxity.AsObject eenvouter takenNames expr

    let ilDelegeeGenericParams = GenGenericParams cenv eenvinner cloFreeTyvars
    let ilDelegeeGenericActualsInner = mkILFormalGenericArgs 0 ilDelegeeGenericParams

    // When creating a delegate that does not capture any variables, we can instead create a static closure and directly reference the method.
    let useStaticClosure = cloFreeVars.IsEmpty

    // Create a new closure class with a single "delegee" method that implements the delegate.
    let delegeeMethName = "Invoke"
    let ilDelegeeTyInner = mkILBoxedTy ilDelegeeTypeRef ilDelegeeGenericActualsInner

    let envForDelegeeUnderTypars = AddTyparsToEnv methTyparsOfOverridingMethod eenvinner

    let numthis = if useStaticClosure then 0 else 1

    let tmvs, body =
        BindUnitVars g (tmvs, List.replicate (List.concat slotsig.FormalParams).Length ValReprInfo.unnamedTopArg1, body)

    // The slot sig contains a formal instantiation. When creating delegates we're only
    // interested in the actual instantiation since we don't have to emit a method impl.
    let ilDelegeeParams, ilDelegeeRet =
        GenActualSlotsig m cenv envForDelegeeUnderTypars slotsig methTyparsOfOverridingMethod tmvs

    let envForDelegeeMeth =
        AddStorageForLocalVals g (List.mapi (fun i v -> (v, Arg(i + numthis))) tmvs) envForDelegeeUnderTypars

    let ilMethodBody =
        CodeGenMethodForExpr
            cenv
            cgbuf.mgbuf
            ([],
             delegeeMethName,
             envForDelegeeMeth,
             1,
             None,
             body,
             (if slotSigHasVoidReturnTy slotsig then
                  discardAndReturnVoid
              else
                  Return))

    let delegeeInvokeMeth =
        (if useStaticClosure then
             mkILNonGenericStaticMethod
         else
             mkILNonGenericInstanceMethod)
            (
                delegeeMethName,
                ILMemberAccess.Assembly,
                ilDelegeeParams,
                ilDelegeeRet,
                MethodBody.IL(lazy ilMethodBody)
            )

    let delegeeCtorMeth =
        mkILSimpleStorageCtor (Some g.ilg.typ_Object.TypeSpec, ilDelegeeTyInner, [], [], ILMemberAccess.Assembly, None, eenvouter.imports)

    let ilCtorBody = delegeeCtorMeth.MethodBody

    let ilCloLambdas = Lambdas_return ilCtxtDelTy

    let cloTypeDefs =
        (if useStaticClosure then
             GenStaticDelegateClosureTypeDefs
         else
             GenClosureTypeDefs)
            cenv
            (ilDelegeeTypeRef,
             ilDelegeeGenericParams,
             [],
             ilCloAllFreeVars,
             ilCloLambdas,
             ilCtorBody,
             [ delegeeInvokeMeth ],
             [],
             g.ilg.typ_Object,
             [],
             None)

    for cloTypeDef in cloTypeDefs do
        cgbuf.mgbuf.AddTypeDef(ilDelegeeTypeRef, cloTypeDef, false, false, None)

    CountClosure()

    // Push the constructor for the delegee
    let ctxtGenericArgsForDelegee = GenGenericArgs m eenvouter.tyenv cloFreeTyvars

    if useStaticClosure then
        GenUnit cenv eenvouter m cgbuf
    else
        let ilxCloSpec =
            IlxClosureSpec.Create(IlxClosureRef(ilDelegeeTypeRef, ilCloLambdas, ilCloAllFreeVars), ctxtGenericArgsForDelegee, false)

        GenWitnessArgsFromWitnessInfos cenv cgbuf eenvouter m cloWitnessInfos
        GenGetLocalVals cenv cgbuf eenvouter m cloFreeVars

        CG.EmitInstr
            cgbuf
            (pop ilCloAllFreeVars.Length)
            (Push [ EraseClosures.mkTyOfLambdas cenv.ilxPubCloEnv ilCloLambdas ])
            (I_newobj(ilxCloSpec.Constructor, None))

    // Push the function pointer to the Invoke method of the delegee
    let ilDelegeeTyOuter = mkILBoxedTy ilDelegeeTypeRef ctxtGenericArgsForDelegee

    let ilDelegeeInvokeMethOuter =
        (if useStaticClosure then
             mkILNonGenericStaticMethSpecInTy
         else
             mkILNonGenericInstanceMethSpecInTy)
            (
                ilDelegeeTyOuter,
                "Invoke",
                typesOfILParams ilDelegeeParams,
                ilDelegeeRet.Type
            )

    CG.EmitInstr cgbuf (pop 0) (Push [ g.ilg.typ_IntPtr ]) (I_ldftn ilDelegeeInvokeMethOuter)

    // Instantiate the delegate
    let ilDelegeeCtorMethOuter =
        mkCtorMethSpecForDelegate g.ilg (ilCtxtDelTy, useUIntPtrForDelegateCtor)

    CG.EmitInstr cgbuf (pop 2) (Push [ ilCtxtDelTy ]) (I_newobj(ilDelegeeCtorMethOuter, None))
    GenSequel cenv eenvouter.cloc cgbuf sequel

/// Generate statically-resolved conditionals used for type-directed optimizations.
and GenStaticOptimization cenv cgbuf eenv (constraints, e2, e3, _m) sequel =
    // Note: during IlxGen, even if answer is StaticOptimizationAnswer.Unknown we discard the static optimization
    // This means 'when ^T : ^T' is discarded if not resolved.
    //
    // This doesn't apply when witnesses are available. In that case, "when ^T : ^T" is resolved as 'Yes',
    // this is because all the uses of "when ^T : ^T" in FSharp.Core (e.g. for are for deciding between the
    // witness-based implementation and the legacy dynamic implementation, e.g.
    //
    //    let inline ( * ) (x: ^T) (y: ^U) : ^V =
    //         MultiplyDynamic<(^T),(^U),(^V)>  x y
    //         ...
    //         when ^T : ^T = ((^T or ^U): (static member (*) : ^T * ^U -> ^V) (x,y))
    //
    // When witnesses are not available we use the dynamic implementation.

    let e =
        let generateWitnesses = ComputeGenerateWitnesses cenv.g eenv

        if DecideStaticOptimizations cenv.g constraints generateWitnesses = StaticOptimizationAnswer.Yes then
            e2
        else
            e3

    GenExpr cenv cgbuf eenv e sequel

//-------------------------------------------------------------------------
// Generate discrimination trees
//-------------------------------------------------------------------------

and IsSequelImmediate sequel =
    match sequel with
    (* All of these can be done at the end of each branch - we don't need a real join point *)
    | Return
    | ReturnVoid
    | Br _
    | LeaveHandler _ -> true
    | DiscardThen sequel -> IsSequelImmediate sequel
    | _ -> false

/// Generate a point where several branches of control flow can merge back together, e.g. after a conditional
/// or 'match'.
and GenJoinPoint cenv cgbuf pos eenv ty m sequel =

    // What the join point does depends on the contents of the sequel. For example, if the sequal is "return" then
    // each branch can just return and no true join point is needed.
    match sequel with
    // All of these can be done at the end of each branch - we don't need a real join point
    | _ when IsSequelImmediate sequel ->
        let stackAfterJoin = cgbuf.GetCurrentStack()
        let afterJoin = CG.GenerateDelayMark cgbuf (pos + "_join")
        sequel, afterJoin, stackAfterJoin, Continue

    // We end scopes at the join point, if any
    | EndLocalScope (sq, mark) ->
        let sequelNow, afterJoin, stackAfterJoin, sequelAfterJoin =
            GenJoinPoint cenv cgbuf pos eenv ty m sq

        sequelNow, afterJoin, stackAfterJoin, EndLocalScope(sequelAfterJoin, mark)

    // If something non-trivial happens after a discard then generate a join point, but first discard the value (often this means we won't generate it at all)
    | DiscardThen sequel ->
        let stackAfterJoin = cgbuf.GetCurrentStack()
        let afterJoin = CG.GenerateDelayMark cgbuf (pos + "_join")
        DiscardThen(Br afterJoin), afterJoin, stackAfterJoin, sequel

    // The others (e.g. Continue, LeaveFilter and CmpThenBrOrContinue) can't be done at the end of each branch. We must create a join point.
    | _ ->
        let pushed = GenType cenv m eenv.tyenv ty
        let stackAfterJoin = (pushed :: (cgbuf.GetCurrentStack()))
        let afterJoin = CG.GenerateDelayMark cgbuf (pos + "_join")
        // go to the join point
        Br afterJoin, afterJoin, stackAfterJoin, sequel

// Accumulate the decision graph as we go
and GenDecisionTreeAndTargets cenv cgbuf stackAtTargets eenv tree targets sequel contf =
    let targetCounts =
        accTargetsOfDecisionTree tree [] |> List.countBy id |> Dictionary.ofList

    let targetNext = ref 0 // used to make sure we generate the targets in-order, postponing if necessary

    GenDecisionTreeAndTargetsInner
        cenv
        cgbuf
        None
        stackAtTargets
        eenv
        tree
        targets
        (targetNext, targetCounts)
        (IntMap.empty ())
        sequel
        (fun targetInfos ->
            let sortedTargetInfos =
                targetInfos
                |> Seq.sortBy (fun (KeyValue (targetIdx, _)) -> targetIdx)
                |> Seq.filter (fun (KeyValue (_, (_, isTargetPostponed))) -> isTargetPostponed)
                |> Seq.map (fun (KeyValue (_, (targetInfo, _))) -> targetInfo)
                |> List.ofSeq

            GenPostponedDecisionTreeTargets cenv cgbuf sortedTargetInfos stackAtTargets sequel contf)

and GenPostponedDecisionTreeTargets cenv cgbuf targetInfos stackAtTargets sequel contf =
    match targetInfos with
    | [] -> contf Fake
    | targetInfo :: rest ->
        let eenvAtTarget, exprAtTarget, sequelAtTarget =
            GenDecisionTreeTarget cenv cgbuf stackAtTargets targetInfo sequel

        GenLinearExpr cenv cgbuf eenvAtTarget exprAtTarget sequelAtTarget true (fun Fake ->
            GenPostponedDecisionTreeTargets cenv cgbuf rest stackAtTargets sequel contf)

/// When inplabOpt is None, we are assuming a branch or fallthrough to the current code location
///
/// When inplabOpt is "Some inplab", we are assuming an existing branch to "inplab" and can optionally
/// set inplab to point to another location if no codegen is required.
and GenDecisionTreeAndTargetsInner
    cenv
    cgbuf
    inplabOpt
    stackAtTargets
    eenv
    tree
    targets
    targetCounts
    targetInfos
    sequel
    (contf: Zmap<_, _> -> FakeUnit)
    =
    CG.SetStack cgbuf stackAtTargets // Set the expected initial stack.

    match tree with
    | TDBind (bind, rest) ->
        cgbuf.SetMarkToHereIfNecessary inplabOpt
        let startMark, endMark as scopeMarks = StartDelayedLocalScope "dtreeBind" cgbuf
        let eenv = AllocStorageForBind cenv cgbuf scopeMarks eenv bind
        GenDebugPointForBind cenv cgbuf bind
        GenBindingAfterDebugPoint cenv cgbuf eenv bind false (Some startMark)

        // We don't get the scope marks quite right for dtree-bound variables. This is because
        // we effectively lose an EndLocalScope for all dtrees that go to the same target
        // So we just pretend that the variable goes out of scope here.
        CG.SetMarkToHere cgbuf endMark
        GenDecisionTreeAndTargetsInner cenv cgbuf None stackAtTargets eenv rest targets targetCounts targetInfos sequel contf

    | TDSuccess (es, targetIdx) ->
        let targetInfos, genTargetInfoOpt =
            GenDecisionTreeSuccess cenv cgbuf inplabOpt stackAtTargets eenv es targetIdx targets targetCounts targetInfos sequel

        match genTargetInfoOpt with
        | Some (eenvAtTarget, exprAtTarget, sequelAtTarget) ->
            GenLinearExpr cenv cgbuf eenvAtTarget exprAtTarget sequelAtTarget true (fun Fake -> contf targetInfos)
        | _ -> contf targetInfos

    | TDSwitch (e, cases, dflt, m) ->
        GenDecisionTreeSwitch cenv cgbuf inplabOpt stackAtTargets eenv e cases dflt m targets targetCounts targetInfos sequel contf

and GetTarget (targets: _[]) n =
    if n >= targets.Length then
        failwith "GetTarget: target not found in decision tree"

    targets[n]

/// Generate a success node of a decision tree, binding the variables and going to the target
/// If inplabOpt is present, this label must get set to the first logical place to execute.
/// For example, if no variables get bound this can just be set to jump straight to the target.
and GenDecisionTreeSuccess
    cenv
    cgbuf
    inplabOpt
    stackAtTargets
    eenv
    es
    targetIdx
    targets
    (targetNext: int ref, targetCounts: Dictionary<int, int>)
    targetInfos
    sequel
    =
    let (TTarget (vs, successExpr, stateVarFlagsOpt)) = GetTarget targets targetIdx

    match IntMap.tryFind targetIdx targetInfos with
    | Some (targetInfo, isTargetPostponed) ->

        let (targetMarkBeforeBinds, targetMarkAfterBinds: Mark, eenvAtTarget, _, _, _, _, _, _) =
            targetInfo

        // We have encountered this target before. See if we should generate it now
        let targetCount = targetCounts[targetIdx]

        let generateTargetNow =
            isTargetPostponed
            && cenv.options.localOptimizationsEnabled
            && targetCount = 1
            && targetNext.Value = targetIdx

        targetCounts[targetIdx] <- targetCount - 1

        // If not binding anything we can go directly to the targetMarkBeforeBinds point
        // This is useful to avoid lots of branches e.g. in match A | B | C -> e
        // In this case each case will just go straight to "e"
        if isNil vs then
            cgbuf.SetMarkOrEmitBranchIfNecessary(inplabOpt, targetMarkBeforeBinds)
        else
            cgbuf.SetMarkToHereIfNecessary inplabOpt
            cgbuf.EmitStartOfHiddenCode()

            (vs, es)
            ||> List.iter2 (fun v e ->

                GetStoreValCtxt cgbuf eenvAtTarget v
                // Emit the expression
                GenBindingRhs cenv cgbuf eenv v e)

            vs
            |> List.rev
            |> List.iter (fun v ->
                // Store the results
                GenStoreVal cgbuf eenvAtTarget v.Range v)

            CG.EmitInstr cgbuf (pop 0) Push0 (I_br targetMarkAfterBinds.CodeLabel)

        let genTargetInfoOpt =
            if generateTargetNow then
                // Fenerate the targets in-order only
                targetNext.Value <- targetNext.Value + 1
                Some(GenDecisionTreeTarget cenv cgbuf stackAtTargets targetInfo sequel)
            else
                None

        // Update the targetInfos
        let isTargetStillPostponed = isTargetPostponed && not generateTargetNow

        let targetInfos =
            IntMap.add targetIdx (targetInfo, isTargetStillPostponed) targetInfos

        targetInfos, genTargetInfoOpt

    | None ->
        // We have not encountered this target before. Set up the generation of the target, even if we're
        // going to postpone it

        let targetMarkBeforeBinds = CG.GenerateDelayMark cgbuf "targetBeforeBinds"
        let targetMarkAfterBinds = CG.GenerateDelayMark cgbuf "targetAfterBinds"
        let startMark, endMark as scopeMarks = StartDelayedLocalScope "targetBinds" cgbuf

        // Allocate storage for variables (except those lifted to be state machine variables)
        let binds =
            match stateVarFlagsOpt with
            | None -> mkInvisibleBinds vs es
            | Some stateVarFlags ->
                (vs, es, stateVarFlags)
                |||> List.zip3
                |> List.choose (fun (v, e, isStateVar) -> if isStateVar then None else Some(mkInvisibleBind v e))

        let eenvAtTarget = AllocStorageForBinds cenv cgbuf scopeMarks eenv binds

        let targetInfo =
            (targetMarkBeforeBinds, targetMarkAfterBinds, eenvAtTarget, successExpr, vs, es, stateVarFlagsOpt, startMark, endMark)

        let targetCount = targetCounts[targetIdx]

        // In debug mode, postpone all decision tree targets to after the switching.
        // In release mode, if a target is the target of multiple incoming success nodes, postpone it to avoid
        // making any backward branches
        let generateTargetNow =
            cenv.options.localOptimizationsEnabled
            && targetCount = 1
            && targetNext.Value = targetIdx

        targetCounts[targetIdx] <- targetCount - 1

        let genTargetInfoOpt =
            if generateTargetNow then
                // Here we are generating the target immediately
                // Generate the targets in-order only
                targetNext.Value <- targetNext.Value + 1
                cgbuf.SetMarkToHereIfNecessary inplabOpt
                Some(GenDecisionTreeTarget cenv cgbuf stackAtTargets targetInfo sequel)
            else
                // Here we are postponing the generation of the target.
                cgbuf.SetMarkOrEmitBranchIfNecessary(inplabOpt, targetMarkBeforeBinds)
                None

        let isTargetPostponed = not generateTargetNow
        let targetInfos = IntMap.add targetIdx (targetInfo, isTargetPostponed) targetInfos
        targetInfos, genTargetInfoOpt

and GenDecisionTreeTarget cenv cgbuf stackAtTargets targetInfo sequel =
    let targetMarkBeforeBinds, targetMarkAfterBinds, eenvAtTarget, successExpr, vs, es, stateVarFlagsOpt, startMark, endMark =
        targetInfo

    CG.SetMarkToHere cgbuf targetMarkBeforeBinds

    cgbuf.EmitStartOfHiddenCode()

    CG.SetMarkToHere cgbuf startMark
    let binds = mkInvisibleBinds vs es
    GenBindings cenv cgbuf eenvAtTarget binds stateVarFlagsOpt
    CG.SetMarkToHere cgbuf targetMarkAfterBinds
    CG.SetStack cgbuf stackAtTargets
    (eenvAtTarget, successExpr, (EndLocalScope(sequel, endMark)))

and GenDecisionTreeSwitch
    cenv
    cgbuf
    inplabOpt
    stackAtTargets
    eenv
    e
    cases
    defaultTargetOpt
    switchm
    targets
    targetCounts
    targetInfos
    sequel
    contf
    =
    let g = cenv.g
    let m = e.Range
    cgbuf.SetMarkToHereIfNecessary inplabOpt

    cgbuf.EmitStartOfHiddenCode()

    match cases with
    // optimize a test against a boolean value, i.e. the all-important if-then-else
    | TCase (DecisionTreeTest.Const (Const.Bool b), successTree) :: _ ->
        let failureTree =
            (match defaultTargetOpt with
             | None -> cases.Tail.Head.CaseTree
             | Some d -> d)

        GenDecisionTreeTest
            cenv
            eenv.cloc
            cgbuf
            stackAtTargets
            e
            None
            false
            eenv
            (if b then successTree else failureTree)
            (if b then failureTree else successTree)
            targets
            targetCounts
            targetInfos
            sequel
            contf

    // Optimize a single test for a union case to an "isdata" test - much
    // more efficient code, and this case occurs in the generated equality testers where perf is important
    | TCase (DecisionTreeTest.UnionCase (c, tyargs), successTree) :: rest when
        rest.Length = (match defaultTargetOpt with
                       | None -> 1
                       | Some _ -> 0)
        ->
        let failureTree =
            match defaultTargetOpt with
            | None -> rest.Head.CaseTree
            | Some tg -> tg

        let cuspec = GenUnionSpec cenv m eenv.tyenv c.TyconRef tyargs
        let idx = c.Index
        let avoidHelpers = entityRefInThisAssembly g.compilingFSharpCore c.TyconRef

        let tester =
            (Some(pop 1, Push [ g.ilg.typ_Bool ], Choice1Of2(avoidHelpers, cuspec, idx)))

        GenDecisionTreeTest
            cenv
            eenv.cloc
            cgbuf
            stackAtTargets
            e
            tester
            false
            eenv
            successTree
            failureTree
            targets
            targetCounts
            targetInfos
            sequel
            contf

    // Use GenDecisionTreeTest to generate a single test for null (when no box required) where the success
    // is going to the immediate first node in the tree
    | TCase (DecisionTreeTest.IsNull _, (TDSuccess ([], 0) as successTree)) :: rest when
        rest.Length = (match defaultTargetOpt with
                       | None -> 1
                       | Some _ -> 0)
        && not (isTyparTy g (tyOfExpr g e))
        ->
        let failureTree =
            match defaultTargetOpt with
            | None -> rest.Head.CaseTree
            | Some tg -> tg

        GenDecisionTreeTest
            cenv
            eenv.cloc
            cgbuf
            stackAtTargets
            e
            None
            true
            eenv
            successTree
            failureTree
            targets
            targetCounts
            targetInfos
            sequel
            contf

    | _ ->
        let caseLabels = List.map (fun _ -> CG.GenerateDelayMark cgbuf "switch_case") cases
        let firstDiscrim = cases.Head.Discriminator

        match firstDiscrim with
        // Iterated tests, e.g. exception constructors, nulltests, typetests and active patterns.
        // These should always have one positive and one negative branch
        | DecisionTreeTest.ArrayLength _
        | DecisionTreeTest.IsInst _
        | DecisionTreeTest.IsNull
        | DecisionTreeTest.Const (Const.Zero) ->
            if not (isSingleton cases) || Option.isNone defaultTargetOpt then
                failwith "internal error: GenDecisionTreeSwitch: DecisionTreeTest.IsInst/isnull/query"

            let bi =
                match firstDiscrim with
                | DecisionTreeTest.Const (Const.Zero) ->
                    GenExpr cenv cgbuf eenv e Continue
                    BI_brfalse
                | DecisionTreeTest.IsNull ->
                    GenExpr cenv cgbuf eenv e Continue
                    let srcTy = tyOfExpr g e

                    if isTyparTy g srcTy then
                        let ilFromTy = GenType cenv m eenv.tyenv srcTy
                        CG.EmitInstr cgbuf (pop 1) (Push [ g.ilg.typ_Object ]) (I_box ilFromTy)

                    BI_brfalse
                | DecisionTreeTest.IsInst (_srcTy, tgtTy) ->
                    let e = mkCallTypeTest g m tgtTy e
                    GenExpr cenv cgbuf eenv e Continue
                    BI_brtrue
                | _ -> failwith "internal error: GenDecisionTreeSwitch"

            CG.EmitInstr cgbuf (pop 1) Push0 (I_brcmp(bi, (List.head caseLabels).CodeLabel))

            GenDecisionTreeCases
                cenv
                cgbuf
                stackAtTargets
                eenv
                defaultTargetOpt
                targets
                targetCounts
                targetInfos
                sequel
                caseLabels
                cases
                contf

        | DecisionTreeTest.ActivePatternCase _ ->
            error (InternalError("internal error in codegen: DecisionTreeTest.ActivePatternCase", switchm))

        | DecisionTreeTest.UnionCase (hdc, tyargs) ->
            GenExpr cenv cgbuf eenv e Continue
            let cuspec = GenUnionSpec cenv m eenv.tyenv hdc.TyconRef tyargs

            let dests =
                if cases.Length <> caseLabels.Length then
                    failwith "internal error: DecisionTreeTest.UnionCase"

                (cases, caseLabels)
                ||> List.map2 (fun case label ->
                    match case with
                    | TCase (DecisionTreeTest.UnionCase (c, _), _) -> (c.Index, label.CodeLabel)
                    | _ -> failwith "error: mixed constructor/const test?")

            let avoidHelpers = entityRefInThisAssembly g.compilingFSharpCore hdc.TyconRef
            EraseUnions.emitDataSwitch g.ilg (UnionCodeGen cgbuf) (avoidHelpers, cuspec, dests)
            CG.EmitInstrs cgbuf (pop 1) Push0 [] // push/pop to match the line above

            GenDecisionTreeCases
                cenv
                cgbuf
                stackAtTargets
                eenv
                defaultTargetOpt
                targets
                targetCounts
                targetInfos
                sequel
                caseLabels
                cases
                contf

        | DecisionTreeTest.Const c ->
            GenExpr cenv cgbuf eenv e Continue

            match c with
            | Const.Bool _ -> failwith "should have been done earlier"
            | Const.SByte _
            | Const.Int16 _
            | Const.Int32 _
            | Const.Byte _
            | Const.UInt16 _
            | Const.UInt32 _
            | Const.Char _ ->
                if List.length cases <> List.length caseLabels then
                    failwith "internal error: "

                let dests =
                    (cases, caseLabels)
                    ||> List.map2 (fun case label ->
                        let i =
                            match case.Discriminator with
                            | DecisionTreeTest.Const c2 ->
                                match c2 with
                                | Const.SByte i -> int32 i
                                | Const.Int16 i -> int32 i
                                | Const.Int32 i -> i
                                | Const.Byte i -> int32 i
                                | Const.UInt16 i -> int32 i
                                | Const.UInt32 i -> int32 i
                                | Const.Char c -> int32 c
                                | _ -> failwith "internal error: badly formed const test"

                            | _ -> failwith "internal error: badly formed const test"

                        (i, label.CodeLabel))

                let mn = List.foldBack (fst >> Operators.min) dests (fst (List.head dests))
                let mx = List.foldBack (fst >> Operators.max) dests (fst (List.head dests))
                // Check if it's worth using a switch
                // REVIEW: this is using switches even for single integer matches!
                if mx - mn = (List.length dests - 1) then
                    let destinationLabels = dests |> List.sortBy fst |> List.map snd

                    if mn <> 0 then
                        CG.EmitInstr cgbuf (pop 0) (Push [ g.ilg.typ_Int32 ]) (mkLdcInt32 mn)
                        CG.EmitInstr cgbuf (pop 1) Push0 AI_sub

                    CG.EmitInstr cgbuf (pop 1) Push0 (I_switch destinationLabels)
                else
                    error (
                        InternalError(
                            "non-dense integer matches not implemented in codegen - these should have been removed by the pattern match compiler",
                            switchm
                        )
                    )

                GenDecisionTreeCases
                    cenv
                    cgbuf
                    stackAtTargets
                    eenv
                    defaultTargetOpt
                    targets
                    targetCounts
                    targetInfos
                    sequel
                    caseLabels
                    cases
                    contf
            | _ -> error (InternalError("these matches should never be needed", switchm))

        | DecisionTreeTest.Error m -> error (InternalError("Trying to compile error recovery branch", m))

and GenDecisionTreeCases
    cenv
    cgbuf
    stackAtTargets
    eenv
    defaultTargetOpt
    targets
    targetCounts
    targetInfos
    sequel
    caseLabels
    cases
    (contf: Zmap<_, _> -> FakeUnit)
    =

    match defaultTargetOpt with
    | Some defaultTarget ->
        GenDecisionTreeAndTargetsInner
            cenv
            cgbuf
            None
            stackAtTargets
            eenv
            defaultTarget
            targets
            targetCounts
            targetInfos
            sequel
            (fun targetInfos ->
                GenDecisionTreeCases cenv cgbuf stackAtTargets eenv None targets targetCounts targetInfos sequel caseLabels cases contf)
    | None ->
        match caseLabels, cases with
        | caseLabel :: caseLabelsTail, TCase (_, caseTree) :: casesTail ->
            GenDecisionTreeAndTargetsInner
                cenv
                cgbuf
                (Some caseLabel)
                stackAtTargets
                eenv
                caseTree
                targets
                targetCounts
                targetInfos
                sequel
                (fun targetInfos ->
                    GenDecisionTreeCases
                        cenv
                        cgbuf
                        stackAtTargets
                        eenv
                        None
                        targets
                        targetCounts
                        targetInfos
                        sequel
                        caseLabelsTail
                        casesTail
                        contf)
        | _ -> contf targetInfos

// Used for the peephole optimization below
and (|BoolExpr|_|) =
    function
    | Expr.Const (Const.Bool b1, _, _) -> Some b1
    | _ -> None

and GenDecisionTreeTest
    cenv
    cloc
    cgbuf
    stackAtTargets
    e
    tester
    isNullTest
    eenv
    successTree
    failureTree
    targets
    targetCounts
    targetInfos
    sequel
    contf
    =
    let g = cenv.g

    match successTree, failureTree with

    // Peephole: if generating a boolean value or its negation then just leave it on the stack
    // This comes up in the generated equality functions. REVIEW: do this as a peephole optimization elsewhere
    | TDSuccess (es1, n1), TDSuccess (es2, n2) when
        not isNullTest
        && isNil es1
        && isNil es2
        && (match GetTarget targets n1, GetTarget targets n2 with
            | TTarget (_, BoolExpr b1, _), TTarget (_, BoolExpr b2, _) -> b1 = not b2
            | _ -> false)
        ->

        match GetTarget targets n1, GetTarget targets n2 with

        | TTarget (_, BoolExpr b1, _), _ ->
            GenExpr cenv cgbuf eenv e Continue

            match tester with
            | Some (pops, pushes, i) ->
                match i with
                | Choice1Of2 (avoidHelpers, cuspec, idx) ->
                    CG.EmitInstrs cgbuf pops pushes (EraseUnions.mkIsData g.ilg (avoidHelpers, cuspec, idx))
                | Choice2Of2 i -> CG.EmitInstr cgbuf pops pushes i
            | _ -> ()

            if not b1 then
                CG.EmitInstr cgbuf (pop 0) (Push [ g.ilg.typ_Bool ]) (mkLdcInt32 0)
                CG.EmitInstr cgbuf (pop 1) Push0 AI_ceq

            GenSequel cenv cloc cgbuf sequel
            contf targetInfos

        | _ -> failwith "internal error: GenDecisionTreeTest during bool elim"

    | _ ->
        match tester with
        | None ->

            // Check if there is more logic in the decision tree for the failure branch
            // (and no more logic for the success branch), for example
            // when emitting the first part of 'expr1 || expr2'.
            //
            // If so, emit the failure logic, then came back and do the success target, then
            // do any postponed failure target.
            match successTree, failureTree with
            | TDSuccess _,
              (TDBind _
              | TDSwitch _) ->

                // OK, there is more logic in the decision tree on the failure branch
                let success = CG.GenerateDelayMark cgbuf "testSuccess"
                let testForSuccess = if isNullTest then BI_brfalse else BI_brtrue
                GenExpr cenv cgbuf eenv e (CmpThenBrOrContinue(pop 1, [ I_brcmp(testForSuccess, success.CodeLabel) ]))

                GenDecisionTreeAndTargetsInner
                    cenv
                    cgbuf
                    None
                    stackAtTargets
                    eenv
                    failureTree
                    targets
                    targetCounts
                    targetInfos
                    sequel
                    (fun targetInfos ->
                        GenDecisionTreeAndTargetsInner
                            cenv
                            cgbuf
                            (Some success)
                            stackAtTargets
                            eenv
                            successTree
                            targets
                            targetCounts
                            targetInfos
                            sequel
                            contf)

            | _ ->

                // Either we're not yet done with the success branch, or there is no more logic
                // in the decision tree on the failure branch. Continue doing the success branch
                // logic first.
                let failure = CG.GenerateDelayMark cgbuf "testFailure"
                let testForFailure = if isNullTest then BI_brtrue else BI_brfalse
                GenExpr cenv cgbuf eenv e (CmpThenBrOrContinue(pop 1, [ I_brcmp(testForFailure, failure.CodeLabel) ]))

                GenDecisionTreeAndTargetsInner
                    cenv
                    cgbuf
                    None
                    stackAtTargets
                    eenv
                    successTree
                    targets
                    targetCounts
                    targetInfos
                    sequel
                    (fun targetInfos ->
                        GenDecisionTreeAndTargetsInner
                            cenv
                            cgbuf
                            (Some failure)
                            stackAtTargets
                            eenv
                            failureTree
                            targets
                            targetCounts
                            targetInfos
                            sequel
                            contf)

        // Turn 'isdata' tests that branch into EI_brisdata tests
        | Some (_, _, Choice1Of2 (avoidHelpers, cuspec, idx)) ->
            let failure = CG.GenerateDelayMark cgbuf "testFailure"

            GenExpr
                cenv
                cgbuf
                eenv
                e
                (CmpThenBrOrContinue(pop 1, EraseUnions.mkBrIsData g.ilg false (avoidHelpers, cuspec, idx, failure.CodeLabel)))

            GenDecisionTreeAndTargetsInner
                cenv
                cgbuf
                None
                stackAtTargets
                eenv
                successTree
                targets
                targetCounts
                targetInfos
                sequel
                (fun targetInfos ->
                    GenDecisionTreeAndTargetsInner
                        cenv
                        cgbuf
                        (Some failure)
                        stackAtTargets
                        eenv
                        failureTree
                        targets
                        targetCounts
                        targetInfos
                        sequel
                        contf)

        | Some (pops, pushes, i) ->
            let failure = CG.GenerateDelayMark cgbuf "testFailure"
            GenExpr cenv cgbuf eenv e Continue

            match i with
            | Choice1Of2 (avoidHelpers, cuspec, idx) ->
                CG.EmitInstrs cgbuf pops pushes (EraseUnions.mkIsData g.ilg (avoidHelpers, cuspec, idx))
            | Choice2Of2 i -> CG.EmitInstr cgbuf pops pushes i

            CG.EmitInstr cgbuf (pop 1) Push0 (I_brcmp(BI_brfalse, failure.CodeLabel))

            GenDecisionTreeAndTargetsInner
                cenv
                cgbuf
                None
                stackAtTargets
                eenv
                successTree
                targets
                targetCounts
                targetInfos
                sequel
                (fun targetInfos ->
                    GenDecisionTreeAndTargetsInner
                        cenv
                        cgbuf
                        (Some failure)
                        stackAtTargets
                        eenv
                        failureTree
                        targets
                        targetCounts
                        targetInfos
                        sequel
                        contf)

/// Generate fixups for letrec bindings
and GenLetRecFixup cenv cgbuf eenv (ilxCloSpec: IlxClosureSpec, e, ilField: ILFieldSpec, e2, _m) =
    GenExpr cenv cgbuf eenv e Continue
    CG.EmitInstr cgbuf (pop 0) Push0 (I_castclass ilxCloSpec.ILType)
    GenExpr cenv cgbuf eenv e2 Continue
    CG.EmitInstr cgbuf (pop 2) Push0 (mkNormalStfld (mkILFieldSpec (ilField.FieldRef, ilxCloSpec.ILType)))

/// Generate letrec bindings
and GenLetRecBindings cenv (cgbuf: CodeGenBuffer) eenv (allBinds: Bindings, m) =

    // 'let rec' bindings are always considered to be in loops, that is each may have backward branches for the
    // tailcalls back to the entry point. This means we don't rely on zero-init of mutable locals
    let eenv = SetIsInLoop true eenv

    // Fix up recursion for non-toplevel recursive bindings
    let bindsPossiblyRequiringFixup =
        allBinds
        |> List.filter (fun b ->
            match (StorageForVal m b.Var eenv) with
            | StaticProperty _
            | Method _
            // Note: Recursive data stored in static fields may require fixups e.g. let x = C(x)
            // | StaticPropertyWithField _
            | Null -> false
            | _ -> true)

    let computeFixupsForOneRecursiveVar boundv forwardReferenceSet (fixups: _ ref) thisVars access set e =
        match e with
        | Expr.Lambda _
        | Expr.TyLambda _
        | Expr.Obj _ ->
            let isLocalTypeFunc =
                Option.isSome thisVars
                && (IsNamedLocalTypeFuncVal cenv.g (Option.get thisVars) e)

            let thisVars =
                (match e with
                 | Expr.Obj _ -> []
                 | _ when isLocalTypeFunc -> []
                 | _ -> Option.map mkLocalValRef thisVars |> Option.toList)

            let canUseStaticField =
                (match e with
                 | Expr.Obj _ -> false
                 | _ -> true)

            let clo, _, eenvclo =
                GetIlxClosureInfo
                    cenv
                    m
                    ILBoxity.AsObject
                    isLocalTypeFunc
                    canUseStaticField
                    thisVars
                    { eenv with
                        letBoundVars = (mkLocalValRef boundv) :: eenv.letBoundVars
                    }
                    e

            for fv in clo.cloFreeVars do
                if Zset.contains fv forwardReferenceSet then
                    match StorageForVal m fv eenvclo with
                    | Env (_, ilField, _) ->
                        let fixup =
                            (boundv, fv, (fun () -> GenLetRecFixup cenv cgbuf eenv (clo.cloSpec, access, ilField, exprForVal m fv, m)))

                        fixups.Value <- fixup :: fixups.Value
                    | _ -> error (InternalError("GenLetRec: " + fv.LogicalName + " was not in the environment", m))

        | Expr.Val (vref, _, _) ->
            let fv = vref.Deref
            let needsFixup = Zset.contains fv forwardReferenceSet

            if needsFixup then
                let fixup = (boundv, fv, (fun () -> GenExpr cenv cgbuf eenv (set e) discard))
                fixups.Value <- fixup :: fixups.Value
        | _ -> failwith "compute real fixup vars"

    let fixups = ref []

    let recursiveVars =
        Zset.addList (bindsPossiblyRequiringFixup |> List.map (fun v -> v.Var)) (Zset.empty valOrder)

    let _ =
        (recursiveVars, bindsPossiblyRequiringFixup)
        ||> List.fold (fun forwardReferenceSet (bind: Binding) ->
            // Compute fixups
            bind.Expr
            |> IterateRecursiveFixups
                cenv.g
                (Some bind.Var)
                (computeFixupsForOneRecursiveVar bind.Var forwardReferenceSet fixups)
                (exprForVal m bind.Var,
                 (fun _ ->
                     failwith (
                         "internal error: should never need to set non-delayed recursive val: "
                         + bind.Var.LogicalName
                     )))
            // Record the variable as defined
            let forwardReferenceSet = Zset.remove bind.Var forwardReferenceSet
            forwardReferenceSet)

    // Generate the actual bindings
    let _ =
        (recursiveVars, allBinds)
        ||> List.fold (fun forwardReferenceSet (bind: Binding) ->
            GenBinding cenv cgbuf eenv bind false

            // Record the variable as defined
            let forwardReferenceSet = Zset.remove bind.Var forwardReferenceSet

            // Execute and discard any fixups that can now be committed
            let newFixups =
                fixups.Value
                |> List.filter (fun (boundv, fv, action) ->
                    if (Zset.contains boundv forwardReferenceSet || Zset.contains fv forwardReferenceSet) then
                        true
                    else
                        action ()
                        false)

            fixups.Value <- newFixups

            forwardReferenceSet)

    ()

and GenLetRec cenv cgbuf eenv (binds, body, m) sequel =
    let _, endMark as scopeMarks = StartLocalScope "letrec" cgbuf
    let eenv = AllocStorageForBinds cenv cgbuf scopeMarks eenv binds
    GenLetRecBindings cenv cgbuf eenv (binds, m)
    GenExpr cenv cgbuf eenv body (EndLocalScope(sequel, endMark))

//-------------------------------------------------------------------------
// Generate simple bindings
//-------------------------------------------------------------------------

and GenDebugPointForBind cenv cgbuf bind =
    let _, pt = ComputeDebugPointForBinding cenv.g bind
    pt |> Option.iter (CG.EmitDebugPoint cgbuf)

and GenBinding cenv cgbuf eenv (bind: Binding) (isStateVar: bool) =
    GenDebugPointForBind cenv cgbuf bind
    GenBindingAfterDebugPoint cenv cgbuf eenv bind isStateVar None

and ComputeMemberAccessRestrictedBySig eenv vspec =
    let isHidden =
        // Anything hidden by a signature gets assembly visibility
        IsHiddenVal eenv.sigToImplRemapInfo vspec
        ||
        // Anything that's not a module or member binding gets assembly visibility
        not vspec.IsMemberOrModuleBinding
        ||
        // Compiler generated members for class function 'let' bindings get assembly visibility
        vspec.IsIncrClassGeneratedMember

    ComputeMemberAccess isHidden

and ComputeMethodAccessRestrictedBySig eenv vspec =
    let isHidden =
        // Anything hidden by a signature gets assembly visibility
        IsHiddenVal eenv.sigToImplRemapInfo vspec
        ||
        // Anything that's not a module or member binding gets assembly visibility
        not vspec.IsMemberOrModuleBinding
        ||
        // Compiler generated members for class function 'let' bindings get assembly visibility
        vspec.IsIncrClassGeneratedMember

    ComputeMemberAccess isHidden

and GenBindingAfterDebugPoint cenv cgbuf eenv bind isStateVar startMarkOpt =
    let g = cenv.g
    let (TBind (vspec, rhsExpr, _)) = bind

    // Record the closed reflection definition if publishing
    // There is no real reason we're doing this so late in the day
    match vspec.PublicPath, vspec.ReflectedDefinition with
    | Some _, Some e when not isStateVar -> cgbuf.mgbuf.AddReflectedDefinition(vspec, e)
    | _ -> ()

    let eenv =
        if isStateVar then
            eenv
        else
            { eenv with
                letBoundVars = (mkLocalValRef vspec) :: eenv.letBoundVars
                initLocals =
                    eenv.initLocals
                    && (match vspec.ApparentEnclosingEntity with
                        | Parent ref -> not (HasFSharpAttribute g g.attrib_SkipLocalsInitAttribute ref.Attribs)
                        | _ -> true)
            }

    let access = ComputeMethodAccessRestrictedBySig eenv vspec

    // Workaround for .NET and Visual Studio restriction w.r.t debugger type proxys
    // Mark internal constructors in internal classes as public.
    let access =
        if
            access = ILMemberAccess.Assembly
            && vspec.IsConstructor
            && IsHiddenTycon eenv.sigToImplRemapInfo vspec.MemberApparentEntity.Deref
        then
            ILMemberAccess.Public
        else
            access

    let m = vspec.Range

    match StorageForVal m vspec eenv with

    | Null ->
        GenExpr cenv cgbuf eenv rhsExpr discard
        CommitStartScope cgbuf startMarkOpt

    // The initialization code for static 'let' and 'do' bindings gets compiled into the initialization .cctor for the whole file
    | _ when
        vspec.IsClassConstructor
        && isNil vspec.DeclaringEntity.TyparsNoRange
        && not isStateVar
        ->
        let tps, _, _, _, cctorBody, _ =
            IteratedAdjustLambdaToMatchValReprInfo g cenv.amap vspec.ValReprInfo.Value rhsExpr

        let eenv = EnvForTypars tps eenv
        CommitStartScope cgbuf startMarkOpt
        GenExpr cenv cgbuf eenv cctorBody discard

    | Method (valReprInfo, _, mspec, mspecW, _, ctps, mtps, curriedArgInfos, paramInfos, witnessInfos, argTys, retInfo) when not isStateVar ->

        let methLambdaTypars, methLambdaCtorThisValOpt, methLambdaBaseValOpt, methLambdaCurriedVars, methLambdaBody, methLambdaBodyTy =
            IteratedAdjustLambdaToMatchValReprInfo g cenv.amap valReprInfo rhsExpr

        let methLambdaVars = List.concat methLambdaCurriedVars

        CommitStartScope cgbuf startMarkOpt

        let hasWitnessEntry = cenv.g.generateWitnesses && not witnessInfos.IsEmpty

        GenMethodForBinding
            cenv
            cgbuf.mgbuf
            eenv
            (vspec,
             mspec,
             hasWitnessEntry,
             false,
             access,
             ctps,
             mtps,
             [],
             curriedArgInfos,
             paramInfos,
             argTys,
             retInfo,
             valReprInfo,
             methLambdaCtorThisValOpt,
             methLambdaBaseValOpt,
             methLambdaTypars,
             methLambdaVars,
             methLambdaBody,
             methLambdaBodyTy)

        // If generating witnesses, then generate the second entry point with additional arguments.
        // Take a copy of the expression to ensure generated names are unique.
        if hasWitnessEntry then
            let copyOfLambdaBody = copyExpr cenv.g CloneAll methLambdaBody

            GenMethodForBinding
                cenv
                cgbuf.mgbuf
                eenv
                (vspec,
                 mspecW,
                 hasWitnessEntry,
                 true,
                 access,
                 ctps,
                 mtps,
                 witnessInfos,
                 curriedArgInfos,
                 paramInfos,
                 argTys,
                 retInfo,
                 valReprInfo,
                 methLambdaCtorThisValOpt,
                 methLambdaBaseValOpt,
                 methLambdaTypars,
                 methLambdaVars,
                 copyOfLambdaBody,
                 methLambdaBodyTy)

    | StaticProperty (ilGetterMethSpec, optShadowLocal) when not isStateVar ->

        let ilAttribs = GenAttrs cenv eenv vspec.Attribs
        let ilTy = ilGetterMethSpec.FormalReturnType

        let ilPropDef =
            ILPropertyDef(
                name = ChopPropertyName ilGetterMethSpec.Name,
                attributes = PropertyAttributes.None,
                setMethod = None,
                getMethod = Some ilGetterMethSpec.MethodRef,
                callingConv = ILThisConvention.Static,
                propertyType = ilTy,
                init = None,
                args = [],
                customAttrs = mkILCustomAttrs ilAttribs
            )

        cgbuf.mgbuf.AddOrMergePropertyDef(ilGetterMethSpec.MethodRef.DeclaringTypeRef, ilPropDef, m)

        let ilMethodDef =
            let ilCode =
                CodeGenMethodForExpr cenv cgbuf.mgbuf ([], ilGetterMethSpec.Name, eenv, 0, None, rhsExpr, Return)

            let ilMethodBody = MethodBody.IL(lazy ilCode)

            (mkILStaticMethod ([], ilGetterMethSpec.Name, access, [], mkILReturn ilTy, ilMethodBody))
                .WithSpecialName
            |> AddNonUserCompilerGeneratedAttribs g

        CountMethodDef()
        cgbuf.mgbuf.AddMethodDef(ilGetterMethSpec.MethodRef.DeclaringTypeRef, ilMethodDef)

        CommitStartScope cgbuf startMarkOpt

        match optShadowLocal with
        | NoShadowLocal -> ()

        | ShadowLocal (startMark, storage) ->
            CG.EmitInstr cgbuf (pop 0) (Push [ ilTy ]) (I_call(Normalcall, ilGetterMethSpec, None))
            GenSetStorage m cgbuf storage
            cgbuf.SetMarkToHere startMark

    | StaticPropertyWithField (fspec,
                               vref,
                               hasLiteralAttr,
                               ilTyForProperty,
                               ilPropName,
                               fty,
                               ilGetterMethRef,
                               ilSetterMethRef,
                               optShadowLocal) ->
        let mut = vspec.IsMutable

        let canTarget (targets, goal: System.AttributeTargets) =
            match targets with
            | None -> true
            | Some tgts -> 0 <> int (tgts &&& goal)

        /// Generate a static field definition...
        let ilFieldDefs =
            let access =
                ComputeMemberAccess(not hasLiteralAttr || IsHiddenVal eenv.sigToImplRemapInfo vspec)

            let ilFieldDef = mkILStaticField (fspec.Name, fty, None, None, access)

            let ilFieldDef =
                match vref.LiteralValue with
                | Some konst -> ilFieldDef.WithLiteralDefaultValue(Some(GenFieldInit m konst))
                | None -> ilFieldDef

            let ilFieldDef =
                let isClassInitializer = (cgbuf.MethodName = ".cctor")

                ilFieldDef.WithInitOnly(
                    not (
                        mut
                        || cenv.options.isInteractiveItExpr
                        || not isClassInitializer
                        || hasLiteralAttr
                    )
                )

            let ilAttribs =
                if not hasLiteralAttr then
                    vspec.Attribs
                    |> List.filter (fun (Attrib (_, _, _, _, _, targets, _)) -> canTarget (targets, System.AttributeTargets.Field))
                    |> GenAttrs cenv eenv // backing field only gets attributes that target fields
                else
                    GenAttrs cenv eenv vspec.Attribs // literals have no property, so preserve all the attributes on the field itself

            let ilFieldDef =
                ilFieldDef.With(customAttrs = mkILCustomAttrs (ilAttribs @ [ g.DebuggerBrowsableNeverAttribute ]))

            [ (fspec.DeclaringTypeRef, ilFieldDef) ]

        let ilTypeRefForProperty = ilTyForProperty.TypeRef

        for tref, ilFieldDef in ilFieldDefs do
            cgbuf.mgbuf.AddFieldDef(tref, ilFieldDef)
            CountStaticFieldDef()

        // ... and the get/set properties to access it.
        if hasLiteralAttr then
            match optShadowLocal with
            | NoShadowLocal -> ()
            | ShadowLocal (startMark, _storage) -> cgbuf.SetMarkToHere startMark
        else
            let ilAttribs =
                vspec.Attribs
                |> List.filter (fun (Attrib (_, _, _, _, _, targets, _)) -> canTarget (targets, System.AttributeTargets.Property))
                |> GenAttrs cenv eenv // property only gets attributes that target properties

            let ilPropDef =
                ILPropertyDef(
                    name = ilPropName,
                    attributes = PropertyAttributes.None,
                    setMethod =
                        (if mut || cenv.options.isInteractiveItExpr then
                             Some ilSetterMethRef
                         else
                             None),
                    getMethod = Some ilGetterMethRef,
                    callingConv = ILThisConvention.Static,
                    propertyType = fty,
                    init = None,
                    args = [],
                    customAttrs = mkILCustomAttrs (ilAttribs @ [ mkCompilationMappingAttr g (int SourceConstructFlags.Value) ])
                )

            cgbuf.mgbuf.AddOrMergePropertyDef(ilTypeRefForProperty, ilPropDef, m)

            let getterMethod =
                let body =
                    mkMethodBody (true, [], 2, nonBranchingInstrsToCode [ mkNormalLdsfld fspec ], None, eenv.imports)

                mkILStaticMethod(
                    [],
                    ilGetterMethRef.Name,
                    access,
                    [],
                    mkILReturn fty,
                    body
                )
                    .WithSpecialName

            cgbuf.mgbuf.AddMethodDef(ilTypeRefForProperty, getterMethod)

            if mut || cenv.options.isInteractiveItExpr then
                let body =
                    mkMethodBody (true, [], 2, nonBranchingInstrsToCode [ mkLdarg0; mkNormalStsfld fspec ], None, eenv.imports)

                let setterMethod =
                    mkILStaticMethod(
                        [],
                        ilSetterMethRef.Name,
                        access,
                        [ mkILParamNamed ("value", fty) ],
                        mkILReturn ILType.Void,
                        body
                    )
                        .WithSpecialName

                cgbuf.mgbuf.AddMethodDef(ilTypeRefForProperty, setterMethod)

            GenBindingRhs cenv cgbuf eenv vspec rhsExpr
            CommitStartScope cgbuf startMarkOpt

            match optShadowLocal with
            | NoShadowLocal -> EmitSetStaticField cgbuf fspec

            | ShadowLocal (startMark, storage) ->
                CG.EmitInstr cgbuf (pop 0) (Push [ fty ]) AI_dup
                EmitSetStaticField cgbuf fspec
                GenSetStorage m cgbuf storage
                cgbuf.SetMarkToHere startMark

    | _ ->
        let storage = StorageForVal m vspec eenv

        match storage, rhsExpr with
        // locals are zero-init, no need to initialize them, except if you are in a loop and the local is mutable.
        | Local (_, realloc, _), Expr.Const (Const.Zero, _, _) when not realloc && not (eenv.isInLoop && vspec.IsMutable) ->
            CommitStartScope cgbuf startMarkOpt
        | _ ->
            GetStoreValCtxt cgbuf eenv vspec
            GenBindingRhs cenv cgbuf eenv vspec rhsExpr
            CommitStartScope cgbuf startMarkOpt
            GenStoreVal cgbuf eenv vspec.Range vspec

and GetStoreValCtxt cgbuf eenv (vspec: Val) =
    // Emit the ldarg0 if needed
    match StorageForVal vspec.Range vspec eenv with
    | Env (ilCloTy, _, _) ->
        let ilCloAddrTy =
            if ilCloTy.Boxity = ILBoxity.AsValue then
                ILType.Byref ilCloTy
            else
                ilCloTy

        CG.EmitInstr cgbuf (pop 0) (Push [ ilCloAddrTy ]) mkLdarg0
    | _ -> ()

//-------------------------------------------------------------------------
// Generate method bindings
//-------------------------------------------------------------------------

/// Generate encoding P/Invoke and COM marshalling information
and GenMarshal cenv attribs =
    let g = cenv.g

    let otherAttribs =
        // For IlReflect backend, we rely on Reflection.Emit API to emit the pseudo-custom attributes
        // correctly, so we do not filter them out.
        // For IlWriteBackend, we filter MarshalAs attributes
        match cenv.options.ilxBackend with
        | IlReflectBackend -> attribs
        | IlWriteBackend ->
            attribs
            |> List.filter (IsMatchingFSharpAttributeOpt g g.attrib_MarshalAsAttribute >> not)

    match TryFindFSharpAttributeOpt g g.attrib_MarshalAsAttribute attribs with
    | Some (Attrib (_, _, [ AttribInt32Arg unmanagedType ], namedArgs, _, _, m)) ->
        let decoder = AttributeDecoder namedArgs

        let rec decodeUnmanagedType unmanagedType =
            // enumeration values for System.Runtime.InteropServices.UnmanagedType taken from mscorlib.il
            match unmanagedType with
            | 0x0 -> ILNativeType.Empty
            | 0x01 -> ILNativeType.Void
            | 0x02 -> ILNativeType.Bool
            | 0x03 -> ILNativeType.Int8
            | 0x04 -> ILNativeType.Byte
            | 0x05 -> ILNativeType.Int16
            | 0x06 -> ILNativeType.UInt16
            | 0x07 -> ILNativeType.Int32
            | 0x08 -> ILNativeType.UInt32
            | 0x09 -> ILNativeType.Int64
            | 0x0A -> ILNativeType.UInt64
            | 0x0B -> ILNativeType.Single
            | 0x0C -> ILNativeType.Double
            | 0x0F -> ILNativeType.Currency
            | 0x13 -> ILNativeType.BSTR
            | 0x14 -> ILNativeType.LPSTR
            | 0x15 -> ILNativeType.LPWSTR
            | 0x16 -> ILNativeType.LPTSTR
            | 0x17 -> ILNativeType.FixedSysString(decoder.FindInt32 "SizeConst" 0x0)
            | 0x19 -> ILNativeType.IUnknown
            | 0x1A -> ILNativeType.IDispatch
            | 0x1B -> ILNativeType.Struct
            | 0x1C -> ILNativeType.Interface
            | 0x1D ->
                let safeArraySubType =
                    match decoder.FindInt32 "SafeArraySubType" 0x0 with
                    (* enumeration values for System.Runtime.InteropServices.VarType taken from mscorlib.il *)
                    | 0x0 -> ILNativeVariant.Empty
                    | 0x1 -> ILNativeVariant.Null
                    | 0x02 -> ILNativeVariant.Int16
                    | 0x03 -> ILNativeVariant.Int32
                    | 0x0C -> ILNativeVariant.Variant
                    | 0x04 -> ILNativeVariant.Single
                    | 0x05 -> ILNativeVariant.Double
                    | 0x06 -> ILNativeVariant.Currency
                    | 0x07 -> ILNativeVariant.Date
                    | 0x08 -> ILNativeVariant.BSTR
                    | 0x09 -> ILNativeVariant.IDispatch
                    | 0x0a -> ILNativeVariant.Error
                    | 0x0b -> ILNativeVariant.Bool
                    | 0x0d -> ILNativeVariant.IUnknown
                    | 0x0e -> ILNativeVariant.Decimal
                    | 0x10 -> ILNativeVariant.Int8
                    | 0x11 -> ILNativeVariant.UInt8
                    | 0x12 -> ILNativeVariant.UInt16
                    | 0x13 -> ILNativeVariant.UInt32
                    | 0x15 -> ILNativeVariant.UInt64
                    | 0x16 -> ILNativeVariant.Int
                    | 0x17 -> ILNativeVariant.UInt
                    | 0x18 -> ILNativeVariant.Void
                    | 0x19 -> ILNativeVariant.HRESULT
                    | 0x1a -> ILNativeVariant.PTR
                    | 0x1c -> ILNativeVariant.CArray
                    | 0x1d -> ILNativeVariant.UserDefined
                    | 0x1e -> ILNativeVariant.LPSTR
                    | 0x1B -> ILNativeVariant.SafeArray
                    | 0x1f -> ILNativeVariant.LPWSTR
                    | 0x24 -> ILNativeVariant.Record
                    | 0x40 -> ILNativeVariant.FileTime
                    | 0x41 -> ILNativeVariant.Blob
                    | 0x42 -> ILNativeVariant.Stream
                    | 0x43 -> ILNativeVariant.Storage
                    | 0x44 -> ILNativeVariant.StreamedObject
                    | 0x45 -> ILNativeVariant.StoredObject
                    | 0x46 -> ILNativeVariant.BlobObject
                    | 0x47 -> ILNativeVariant.CF
                    | 0x48 -> ILNativeVariant.CLSID
                    | 0x14 -> ILNativeVariant.Int64
                    | _ -> ILNativeVariant.Empty

                let safeArrayUserDefinedSubType =
                    // the argument is a System.Type obj, but it's written to MD as a UTF8 string
                    match decoder.FindTypeName "SafeArrayUserDefinedSubType" "" with
                    | "" -> None
                    | res ->
                        if
                            (safeArraySubType = ILNativeVariant.IDispatch)
                            || (safeArraySubType = ILNativeVariant.IUnknown)
                        then
                            Some res
                        else
                            None

                ILNativeType.SafeArray(safeArraySubType, safeArrayUserDefinedSubType)
            | 0x1E -> ILNativeType.FixedArray(decoder.FindInt32 "SizeConst" 0x0)
            | 0x1F -> ILNativeType.Int
            | 0x20 -> ILNativeType.UInt
            | 0x22 -> ILNativeType.ByValStr
            | 0x23 -> ILNativeType.ANSIBSTR
            | 0x24 -> ILNativeType.TBSTR
            | 0x25 -> ILNativeType.VariantBool
            | 0x26 -> ILNativeType.Method
            | 0x28 -> ILNativeType.AsAny
            | 0x2A ->
                let sizeParamIndex =
                    match decoder.FindInt16 "SizeParamIndex" -1s with
                    | -1s -> None
                    | res -> Some(int res, None)

                let arraySubType =
                    match decoder.FindInt32 "ArraySubType" -1 with
                    | -1 -> None
                    | res -> Some(decodeUnmanagedType res)

                ILNativeType.Array(arraySubType, sizeParamIndex)
            | 0x2B -> ILNativeType.LPSTRUCT
            | 0x2C -> error (Error(FSComp.SR.ilCustomMarshallersCannotBeUsedInFSharp (), m))
            (* ILNativeType.Custom of bytes * string * string * bytes (* GUID, nativeTypeName, custMarshallerName, cookieString *) *)
            //ILNativeType.Error
            | 0x2D -> ILNativeType.Error
            | 0x30 -> ILNativeType.LPUTF8STR
            | _ -> ILNativeType.Empty

        Some(decodeUnmanagedType unmanagedType), otherAttribs
    | Some (Attrib (_, _, _, _, _, _, m)) ->
        errorR (Error(FSComp.SR.ilMarshalAsAttributeCannotBeDecoded (), m))
        None, attribs
    | _ ->
        // No MarshalAs detected
        None, attribs

/// Generate special attributes on an IL parameter
and GenParamAttribs cenv paramTy attribs =
    let g = cenv.g

    let inFlag =
        HasFSharpAttribute g g.attrib_InAttribute attribs || isInByrefTy g paramTy

    let outFlag =
        HasFSharpAttribute g g.attrib_OutAttribute attribs || isOutByrefTy g paramTy

    let optionalFlag = HasFSharpAttributeOpt g g.attrib_OptionalAttribute attribs

    let defaultValue =
        TryFindFSharpAttributeOpt g g.attrib_DefaultParameterValueAttribute attribs
        |> Option.bind OptionalArgInfo.FieldInitForDefaultParameterValueAttrib
    // Return the filtered attributes. Do not generate In, Out, Optional or DefaultParameterValue attributes
    // as custom attributes in the code - they are implicit from the IL bits for these
    let attribs =
        attribs
        |> List.filter (IsMatchingFSharpAttribute g g.attrib_InAttribute >> not)
        |> List.filter (IsMatchingFSharpAttribute g g.attrib_OutAttribute >> not)
        |> List.filter (IsMatchingFSharpAttributeOpt g g.attrib_OptionalAttribute >> not)
        |> List.filter (IsMatchingFSharpAttributeOpt g g.attrib_DefaultParameterValueAttribute >> not)

    let Marshal, attribs = GenMarshal cenv attribs
    inFlag, outFlag, optionalFlag, defaultValue, Marshal, attribs

/// Generate IL parameters
and GenParams
    (cenv: cenv)
    eenv
    m
    (mspec: ILMethodSpec)
    witnessInfos
    (argInfos: ArgReprInfo list)
    methArgTys
    (implValsOpt: Val list option)
    =
    let g = cenv.g
    let ilWitnessParams = GenWitnessParams cenv eenv m witnessInfos
    let ilArgTys = mspec.FormalArgTypes |> List.skip witnessInfos.Length

    let ilArgTysAndInfos =
        if argInfos.Length = ilArgTys.Length then
            List.zip ilArgTys argInfos
        else
            assert false
            ilArgTys |> List.map (fun ilArgTy -> ilArgTy, ValReprInfo.unnamedTopArg1)

    let ilArgTysAndInfoAndVals =
        match implValsOpt with
        | Some implVals when (implVals.Length = ilArgTys.Length) -> List.map2 (fun x y -> x, Some y) ilArgTysAndInfos implVals
        | _ -> List.map (fun x -> x, None) ilArgTysAndInfos

    let ilParams, _ =
        (Set.empty, List.zip methArgTys ilArgTysAndInfoAndVals)
        ||> List.mapFold (fun takenNames (methodArgTy, ((ilArgTy, topArgInfo), implValOpt)) ->
            let inFlag, outFlag, optionalFlag, defaultParamValue, Marshal, attribs =
                GenParamAttribs cenv methodArgTy topArgInfo.Attribs

            let idOpt =
                match topArgInfo.Name with
                | Some v -> Some v
                | None ->
                    match implValOpt with
                    | Some v -> Some v.Id
                    | None -> None

            let nmOpt, takenNames =
                match idOpt with
                | Some id ->
                    let nm =
                        if takenNames.Contains(id.idText) then
                            // Ensure that we have an g.CompilerGlobalState
                            assert (g.CompilerGlobalState |> Option.isSome)
                            g.CompilerGlobalState.Value.NiceNameGenerator.FreshCompilerGeneratedName(id.idText, id.idRange)
                        else
                            id.idText

                    Some nm, takenNames.Add(nm)
                | None -> None, takenNames

            let ilAttribs = GenAttrs cenv eenv attribs

            let ilAttribs =
                match GenReadOnlyAttributeIfNecessary g methodArgTy with
                | Some attr -> ilAttribs @ [ attr ]
                | None -> ilAttribs

            let param: ILParameter =
                {
                    Name = nmOpt
                    Type = ilArgTy
                    Default = defaultParamValue
                    Marshal = Marshal
                    IsIn = inFlag
                    IsOut = outFlag
                    IsOptional = optionalFlag
                    CustomAttrsStored = storeILCustomAttrs (mkILCustomAttrs ilAttribs)
                    MetadataIndex = NoMetadataIdx
                }

            param, takenNames)

    ilWitnessParams @ ilParams

/// Generate IL method return information
and GenReturnInfo cenv eenv returnTy ilRetTy (retInfo: ArgReprInfo) : ILReturn =
    let marshal, attribs = GenMarshal cenv retInfo.Attribs
    let ilAttribs = GenAttrs cenv eenv attribs

    let ilAttribs =
        match returnTy with
        | Some retTy ->
            match GenReadOnlyAttributeIfNecessary cenv.g retTy with
            | Some attr -> ilAttribs @ [ attr ]
            | None -> ilAttribs
        | _ -> ilAttribs

    let ilAttrs = mkILCustomAttrs ilAttribs

    {
        Type = ilRetTy
        Marshal = marshal
        CustomAttrsStored = storeILCustomAttrs ilAttrs
        MetadataIndex = NoMetadataIdx
    }

/// Generate an IL property for a member
and GenPropertyForMethodDef compileAsInstance tref mdef (v: Val) (memberInfo: ValMemberInfo) ilArgTys ilPropTy ilAttrs compiledName =
    let name =
        match compiledName with
        | Some n -> n
        | _ -> v.PropertyName in (* chop "get_" *)

    ILPropertyDef(
        name = name,
        attributes = PropertyAttributes.None,
        setMethod =
            (if memberInfo.MemberFlags.MemberKind = SynMemberKind.PropertySet then
                 Some(mkRefToILMethod (tref, mdef))
             else
                 None),
        getMethod =
            (if memberInfo.MemberFlags.MemberKind = SynMemberKind.PropertyGet then
                 Some(mkRefToILMethod (tref, mdef))
             else
                 None),
        callingConv =
            (if compileAsInstance then
                 ILThisConvention.Instance
             else
                 ILThisConvention.Static),
        propertyType = ilPropTy,
        init = None,
        args = ilArgTys,
        customAttrs = ilAttrs
    )

/// Generate an ILEventDef for a [<CLIEvent>] member
and GenEventForProperty cenv eenvForMeth (mspec: ILMethodSpec) (v: Val) ilAttrsThatGoOnPrimaryItem m returnTy =
    let evname = v.PropertyName
    let delegateTy = FindDelegateTypeOfPropertyEvent cenv.g cenv.amap evname m returnTy
    let ilDelegateTy = GenType cenv m eenvForMeth.tyenv delegateTy
    let ilThisTy = mspec.DeclaringType

    let addMethRef =
        mkILMethRef (ilThisTy.TypeRef, mspec.CallingConv, "add_" + evname, 0, [ ilDelegateTy ], ILType.Void)

    let removeMethRef =
        mkILMethRef (ilThisTy.TypeRef, mspec.CallingConv, "remove_" + evname, 0, [ ilDelegateTy ], ILType.Void)

    ILEventDef(
        eventType = Some ilDelegateTy,
        name = evname,
        attributes = EventAttributes.None,
        addMethod = addMethRef,
        removeMethod = removeMethRef,
        fireMethod = None,
        otherMethods = [],
        customAttrs = mkILCustomAttrs ilAttrsThatGoOnPrimaryItem
    )

and ComputeMethodImplNameFixupForMemberBinding cenv (v: Val) =
    if isNil v.ImplementedSlotSigs then
        None
    else
        let slotsig = v.ImplementedSlotSigs |> List.last
        let useMethodImpl = ComputeUseMethodImpl cenv.g v
        let nameOfOverridingMethod = GenNameOfOverridingMethod cenv (useMethodImpl, slotsig)
        Some nameOfOverridingMethod

and ComputeFlagFixupsForMemberBinding cenv (v: Val) =
    [
        let useMethodImpl = ComputeUseMethodImpl cenv.g v

        if useMethodImpl then
            fixupMethodImplFlags
        else
            fixupVirtualSlotFlags

        match ComputeMethodImplNameFixupForMemberBinding cenv v with
        | Some nm -> renameMethodDef nm
        | None -> ()
    ]

and ComputeMethodImplAttribs cenv (_v: Val) attrs =
    let g = cenv.g

    let implflags =
        match TryFindFSharpAttribute g g.attrib_MethodImplAttribute attrs with
        | Some (Attrib (_, _, [ AttribInt32Arg flags ], _, _, _, _)) -> flags
        | _ -> 0x0

    let hasPreserveSigAttr =
        match TryFindFSharpAttributeOpt g g.attrib_PreserveSigAttribute attrs with
        | Some _ -> true
        | _ -> false

    // strip the MethodImpl pseudo-custom attribute
    // The following method implementation flags are used here
    // 0x80 - hasPreserveSigImplFlag
    // 0x20 - synchronize
    // (See ECMA 335, Partition II, section 23.1.11 - Flags for methods [MethodImplAttributes])
    let attrs =
        attrs
        |> List.filter (IsMatchingFSharpAttribute g g.attrib_MethodImplAttribute >> not)
        |> List.filter (IsMatchingFSharpAttributeOpt g g.attrib_PreserveSigAttribute >> not)

    let hasPreserveSigImplFlag = ((implflags &&& 0x80) <> 0x0) || hasPreserveSigAttr
    let hasSynchronizedImplFlag = (implflags &&& 0x20) <> 0x0
    let hasNoInliningImplFlag = (implflags &&& 0x08) <> 0x0
    let hasAggressiveInliningImplFlag = (implflags &&& 0x0100) <> 0x0
    hasPreserveSigImplFlag, hasSynchronizedImplFlag, hasNoInliningImplFlag, hasAggressiveInliningImplFlag, attrs

and GenMethodForBinding
    cenv
    mgbuf
    eenv
    (v: Val,
     mspec,
     hasWitnessEntry,
     generateWitnessArgs,
     access,
     ctps,
     mtps,
     witnessInfos,
     curriedArgInfos,
     paramInfos,
     argTys,
     retInfo,
     valReprInfo,
     ctorThisValOpt,
     baseValOpt,
     methLambdaTypars,
     methLambdaVars,
     methLambdaBody,
     returnTy)
    =
    let g = cenv.g
    let m = v.Range

    // If a method has a witness-passing version of the code, then suppress
    // the generation of any witness in the non-witness passing version of the code
    let eenv =
        { eenv with
            suppressWitnesses = hasWitnessEntry && not generateWitnessArgs
        }

    let selfMethodVars, nonSelfMethodVars, compileAsInstance =
        match v.MemberInfo with
        | Some _ when ValSpecIsCompiledAsInstance g v ->
            match methLambdaVars with
            | [] -> error (InternalError("Internal error: empty argument list for instance method", v.Range))
            | h :: t -> [ h ], t, true
        | _ -> [], methLambdaVars, false

    let nonUnitNonSelfMethodVars, body =
        BindUnitVars cenv.g (nonSelfMethodVars, paramInfos, methLambdaBody)

    let eenv = bindBaseOrThisVarOpt cenv eenv ctorThisValOpt
    let eenv = bindBaseOrThisVarOpt cenv eenv baseValOpt

    // The type parameters of the method's type are different to the type parameters
    // for the big lambda ("tlambda") of the implementation of the method.
    let eenvUnderMethLambdaTypars = EnvForTypars methLambdaTypars eenv
    let eenvUnderMethTypeClassTypars = EnvForTypars ctps eenv
    let eenvUnderMethTypeTypars = AddTyparsToEnv mtps eenvUnderMethTypeClassTypars

    // Add the arguments to the environment. We add an implicit 'this' argument to constructors
    let isCtor = v.IsConstructor

    let methLambdaWitnessInfos =
        if generateWitnessArgs then
            GetTraitWitnessInfosOfTypars cenv.g ctps.Length methLambdaTypars
        else
            []

    // If this assert fails then there is a mismatch in the number of trait constraints on the method type and the number
    // on the method implementation.
    assert (methLambdaWitnessInfos.Length = witnessInfos.Length)

    let eenvForMeth =
        let eenvForMeth = eenvUnderMethLambdaTypars
        let numArgsUsed = 0
        let numArgsUsed = numArgsUsed + (if isCtor then 1 else 0)

        let eenvForMeth =
            eenvForMeth
            |> AddStorageForLocalVals cenv.g (selfMethodVars |> List.mapi (fun i v -> (v, Arg(numArgsUsed + i))))

        let numArgsUsed = numArgsUsed + selfMethodVars.Length

        let eenvForMeth =
            eenvForMeth
            |> AddStorageForLocalWitnesses(methLambdaWitnessInfos |> List.mapi (fun i w -> (w, Arg(numArgsUsed + i))))

        let numArgsUsed = numArgsUsed + methLambdaWitnessInfos.Length

        let eenvForMeth =
            eenvForMeth
            |> AddStorageForLocalVals cenv.g (List.mapi (fun i v -> (v, Arg(numArgsUsed + i))) nonUnitNonSelfMethodVars)

        let eenvForMeth =
            if
                eenvForMeth.initLocals
                && HasFSharpAttribute g g.attrib_SkipLocalsInitAttribute v.Attribs
            then
                { eenvForMeth with initLocals = false }
            else
                eenvForMeth

        eenvForMeth

    let tailCallInfo =
        [
            (mkLocalValRef v,
             BranchCallMethod(
                 valReprInfo.AritiesOfArgs,
                 curriedArgInfos,
                 methLambdaTypars,
                 selfMethodVars.Length,
                 methLambdaWitnessInfos.Length,
                 nonUnitNonSelfMethodVars.Length
             ))
        ]

    // Discard the result on a 'void' return type. For a constructor just return 'void'
    let sequel =
        if isUnitTy g returnTy then discardAndReturnVoid
        elif isCtor then ReturnVoid
        else Return

    // Now generate the code.
    let hasPreserveSigNamedArg, ilMethodBody, hasDllImport =
        match TryFindFSharpAttributeOpt g g.attrib_DllImportAttribute v.Attribs with
        | Some (Attrib (_, _, [ AttribStringArg dll ], namedArgs, _, _, m)) ->
            if not (isNil methLambdaTypars) then
                error (Error(FSComp.SR.ilSignatureForExternalFunctionContainsTypeParameters (), m))

            let hasPreserveSigNamedArg, mbody =
                GenPInvokeMethod(v.CompiledName g.CompilerGlobalState, dll, namedArgs)

            hasPreserveSigNamedArg, mbody, true

        | Some (Attrib (_, _, _, _, _, _, m)) -> error (Error(FSComp.SR.ilDllImportAttributeCouldNotBeDecoded (), m))

        | _ ->
            // Replace the body of ValInline.PseudoVal "must inline" methods with a 'throw'
            // For witness-passing methods, don't do this if `isLegacy` flag specified
            // on the attribute. Older compilers
            let bodyExpr =
                let attr =
                    TryFindFSharpBoolAttributeAssumeFalse cenv.g cenv.g.attrib_NoDynamicInvocationAttribute v.Attribs

                if
                    (not generateWitnessArgs && attr.IsSome)
                    || (generateWitnessArgs && attr = Some false)
                then
                    let exnArg =
                        mkString cenv.g m (FSComp.SR.ilDynamicInvocationNotSupported (v.CompiledName g.CompilerGlobalState))

                    let exnExpr = MakeNotSupportedExnExpr cenv eenv (exnArg, m)
                    mkThrow m returnTy exnExpr
                else
                    body

            let selfValOpt =
                match selfMethodVars with
                | [ h ] -> Some h
                | _ -> None

            let ilCodeLazy =
                CodeGenMethodForExpr cenv mgbuf (tailCallInfo, mspec.Name, eenvForMeth, 0, selfValOpt, bodyExpr, sequel)

            // This is the main code generation for most methods
            false, MethodBody.IL(notlazy ilCodeLazy), false

    // Do not generate DllImport attributes into the code - they are implicit from the P/Invoke
    let attrs =
        v.Attribs
        |> List.filter (IsMatchingFSharpAttributeOpt g g.attrib_DllImportAttribute >> not)
        |> List.filter (IsMatchingFSharpAttribute g g.attrib_CompiledNameAttribute >> not)

    let attrsAppliedToGetterOrSetter, attrs =
        List.partition (fun (Attrib (_, _, _, _, isAppliedToGetterOrSetter, _, _)) -> isAppliedToGetterOrSetter) attrs

    let sourceNameAttribs, compiledName =
        match
            v.Attribs
            |> List.tryFind (IsMatchingFSharpAttribute g g.attrib_CompiledNameAttribute)
        with
        | Some (Attrib (_, _, [ AttribStringArg b ], _, _, _, _)) -> [ mkCompilationSourceNameAttr g v.LogicalName ], Some b
        | _ -> [], None

    // check if the hasPreserveSigNamedArg and hasSynchronizedImplFlag implementation flags have been specified
    let hasPreserveSigImplFlag, hasSynchronizedImplFlag, hasNoInliningFlag, hasAggressiveInliningImplFlag, attrs =
        ComputeMethodImplAttribs cenv v attrs

    let securityAttributes, attrs =
        attrs
        |> List.partition (fun a -> IsSecurityAttribute g cenv.amap cenv.casApplied a m)

    let permissionSets = CreatePermissionSets cenv eenv securityAttributes

    let secDecls =
        if List.isEmpty securityAttributes then
            emptyILSecurityDecls
        else
            mkILSecurityDecls permissionSets

    // Do not push the attributes to the method for events and properties
    let ilAttrsCompilerGenerated =
        if v.IsCompilerGenerated || v.GetterOrSetterIsCompilerGenerated then
            [ g.CompilerGeneratedAttribute; g.DebuggerNonUserCodeAttribute ]
        else
            []

    let ilAttrsThatGoOnPrimaryItem =
        [
            yield! GenAttrs cenv eenv attrs
            yield! GenCompilationArgumentCountsAttr cenv v

            match v.MemberInfo with
            | Some memberInfo when
                memberInfo.MemberFlags.MemberKind = SynMemberKind.PropertyGet
                || memberInfo.MemberFlags.MemberKind = SynMemberKind.PropertySet
                || memberInfo.MemberFlags.MemberKind = SynMemberKind.PropertyGetSet
                ->
                match GenReadOnlyAttributeIfNecessary g returnTy with
                | Some ilAttr -> ilAttr
                | _ -> ()
            | _ -> ()
        ]

    let ilTypars = GenGenericParams cenv eenvUnderMethLambdaTypars methLambdaTypars

    let ilParams =
        GenParams cenv eenvUnderMethTypeTypars m mspec witnessInfos paramInfos argTys (Some nonUnitNonSelfMethodVars)

    let ilReturn =
        GenReturnInfo cenv eenvUnderMethTypeTypars (Some returnTy) mspec.FormalReturnType retInfo

    let methName = mspec.Name
    let tref = mspec.MethodRef.DeclaringTypeRef

    match v.MemberInfo with
    // don't generate unimplemented abstracts
    | Some memberInfo when memberInfo.MemberFlags.IsDispatchSlot && not memberInfo.IsImplemented ->
        // skipping unimplemented abstract method
        ()

    // compiling CLIEvent properties
    | Some memberInfo when
        not v.IsExtensionMember
        && (match memberInfo.MemberFlags.MemberKind with
            | SynMemberKind.PropertySet
            | SynMemberKind.PropertyGet -> CompileAsEvent cenv.g v.Attribs
            | _ -> false)
        ->

        let useMethodImpl =
            if
                compileAsInstance
                && ((memberInfo.MemberFlags.IsDispatchSlot && memberInfo.IsImplemented)
                    || memberInfo.MemberFlags.IsOverrideOrExplicitImpl)
            then

                let useMethodImpl = ComputeUseMethodImpl cenv.g v

                let nameOfOverridingMethod =
                    match ComputeMethodImplNameFixupForMemberBinding cenv v with
                    | None -> mspec.Name
                    | Some nm -> nm

                // Fixup can potentially change name of reflected definition that was already recorded - patch it if necessary
                mgbuf.ReplaceNameOfReflectedDefinition(v, nameOfOverridingMethod)
                useMethodImpl
            else
                false

        // skip method generation for compiling the property as a .NET event
        // Instead emit the pseudo-property as an event.
        // on't do this if it's a private method impl.
        if not useMethodImpl then
            let edef =
                GenEventForProperty cenv eenvForMeth mspec v ilAttrsThatGoOnPrimaryItem m returnTy

            mgbuf.AddEventDef(tref, edef)

    | _ ->

        let mdef =
            match v.MemberInfo with
            | Some memberInfo when not v.IsExtensionMember ->

                let ilMethTypars = ilTypars |> List.skip mspec.DeclaringType.GenericArgs.Length

                if memberInfo.MemberFlags.MemberKind = SynMemberKind.Constructor then
                    assert (isNil ilMethTypars)
                    let mdef = mkILCtor (access, ilParams, ilMethodBody)

                    let mdef =
                        mdef.With(customAttrs = mkILCustomAttrs (ilAttrsThatGoOnPrimaryItem @ sourceNameAttribs @ ilAttrsCompilerGenerated))

                    mdef

                elif memberInfo.MemberFlags.MemberKind = SynMemberKind.ClassConstructor then
                    assert (isNil ilMethTypars)
                    let mdef = mkILClassCtor ilMethodBody

                    let mdef =
                        mdef.With(customAttrs = mkILCustomAttrs (ilAttrsThatGoOnPrimaryItem @ sourceNameAttribs @ ilAttrsCompilerGenerated))

                    mdef

                // Generate virtual/override methods + method-impl information if needed
                else
                    let mdef =
                        if not compileAsInstance then
                            mkILStaticMethod (ilMethTypars, mspec.Name, access, ilParams, ilReturn, ilMethodBody)

                        elif
                            (memberInfo.MemberFlags.IsDispatchSlot && memberInfo.IsImplemented)
                            || memberInfo.MemberFlags.IsOverrideOrExplicitImpl
                        then

                            let flagFixups = ComputeFlagFixupsForMemberBinding cenv v

                            let mdef =
                                mkILGenericVirtualMethod (mspec.Name, ILMemberAccess.Public, ilMethTypars, ilParams, ilReturn, ilMethodBody)

                            let mdef = List.fold (fun mdef f -> f mdef) mdef flagFixups

                            // fixup can potentially change name of reflected definition that was already recorded - patch it if necessary
                            mgbuf.ReplaceNameOfReflectedDefinition(v, mdef.Name)
                            mdef
                        else
                            mkILGenericNonVirtualMethod (mspec.Name, access, ilMethTypars, ilParams, ilReturn, ilMethodBody)

                    let isAbstract =
                        memberInfo.MemberFlags.IsDispatchSlot
                        && let tcref = v.MemberApparentEntity in
                           not tcref.Deref.IsFSharpDelegateTycon

                    let mdef =
                        if mdef.IsVirtual then
                            mdef.WithFinal(memberInfo.MemberFlags.IsFinal).WithAbstract(isAbstract)
                        else
                            mdef

                    match memberInfo.MemberFlags.MemberKind with

                    | SynMemberKind.PropertySet
                    | SynMemberKind.PropertyGet ->
                        if not (isNil ilMethTypars) then
                            error (
                                InternalError(
                                    "A property may not be more generic than the enclosing type - constrain the polymorphism in the expression",
                                    v.Range
                                )
                            )

                        // Check if we're compiling the property as a .NET event
                        assert not (CompileAsEvent cenv.g v.Attribs)

                        // Emit the property, but not if it's a private method impl
                        if mdef.Access <> ILMemberAccess.Private then
                            let vtyp = ReturnTypeOfPropertyVal g v
                            let ilPropTy = GenType cenv m eenvUnderMethTypeTypars.tyenv vtyp
                            let ilPropTy = GenReadOnlyModReqIfNecessary g vtyp ilPropTy

                            let ilArgTys =
                                v
                                |> ArgInfosOfPropertyVal g
                                |> List.map fst
                                |> GenTypes cenv m eenvUnderMethTypeTypars.tyenv

                            let ilPropDef =
                                GenPropertyForMethodDef
                                    compileAsInstance
                                    tref
                                    mdef
                                    v
                                    memberInfo
                                    ilArgTys
                                    ilPropTy
                                    (mkILCustomAttrs ilAttrsThatGoOnPrimaryItem)
                                    compiledName

                            mgbuf.AddOrMergePropertyDef(tref, ilPropDef, m)

                        // Add the special name flag for all properties
                        let mdef =
                            mdef.WithSpecialName.With(
                                customAttrs =
                                    mkILCustomAttrs (
                                        (GenAttrs cenv eenv attrsAppliedToGetterOrSetter)
                                        @ sourceNameAttribs @ ilAttrsCompilerGenerated
                                    )
                            )

                        mdef

                    | _ ->
                        let mdef =
                            mdef.With(
                                customAttrs = mkILCustomAttrs (ilAttrsThatGoOnPrimaryItem @ sourceNameAttribs @ ilAttrsCompilerGenerated)
                            )

                        mdef

            | _ ->
                let mdef =
                    mkILStaticMethod (ilTypars, methName, access, ilParams, ilReturn, ilMethodBody)

                // For extension properties, also emit attrsAppliedToGetterOrSetter on the getter or setter method
                let ilAttrs =
                    match v.MemberInfo with
                    | Some memberInfo when v.IsExtensionMember ->
                        match memberInfo.MemberFlags.MemberKind with
                        | SynMemberKind.PropertySet
                        | SynMemberKind.PropertyGet -> ilAttrsThatGoOnPrimaryItem @ GenAttrs cenv eenv attrsAppliedToGetterOrSetter
                        | _ -> ilAttrsThatGoOnPrimaryItem
                    | _ -> ilAttrsThatGoOnPrimaryItem

                let ilCustomAttrs =
                    mkILCustomAttrs (ilAttrs @ sourceNameAttribs @ ilAttrsCompilerGenerated)

                let mdef = mdef.With(customAttrs = ilCustomAttrs)
                mdef

        // Does the function have an explicit [<EntryPoint>] attribute?
        let isExplicitEntryPoint = HasFSharpAttribute g g.attrib_EntryPointAttribute attrs

        let mdef =
            mdef
                .WithSecurity(not (List.isEmpty securityAttributes))
                .WithPInvoke(hasDllImport)
                .WithPreserveSig(hasPreserveSigImplFlag || hasPreserveSigNamedArg)
                .WithSynchronized(hasSynchronizedImplFlag)
                .WithNoInlining(hasNoInliningFlag)
                .WithAggressiveInlining(hasAggressiveInliningImplFlag)
                .With(isEntryPoint = isExplicitEntryPoint, securityDecls = secDecls)

        let mdef =
            if // operator names
                mdef.Name.StartsWithOrdinal("op_")
                ||
                // active pattern names
                mdef.Name.StartsWithOrdinal("|")
                ||
                // event add/remove method
                v.val_flags.IsGeneratedEventVal
            then
                mdef.WithSpecialName
            else
                mdef

        CountMethodDef()
        mgbuf.AddMethodDef(tref, mdef)

and GenPInvokeMethod (nm, dll, namedArgs) =
    let decoder = AttributeDecoder namedArgs

    let hasPreserveSigNamedArg = decoder.FindBool "PreserveSig" true

    hasPreserveSigNamedArg,
    let pinvoke: PInvokeMethod =
        {
            Where = mkSimpleModRef dll
            Name = decoder.FindString "EntryPoint" nm
            CallingConv =
                match decoder.FindInt32 "CallingConvention" 0 with
                | 1 -> PInvokeCallingConvention.WinApi
                | 2 -> PInvokeCallingConvention.Cdecl
                | 3 -> PInvokeCallingConvention.Stdcall
                | 4 -> PInvokeCallingConvention.Thiscall
                | 5 -> PInvokeCallingConvention.Fastcall
                | _ -> PInvokeCallingConvention.WinApi
            CharEncoding =
                match decoder.FindInt32 "CharSet" 0 with
                | 1 -> PInvokeCharEncoding.None
                | 2 -> PInvokeCharEncoding.Ansi
                | 3 -> PInvokeCharEncoding.Unicode
                | 4 -> PInvokeCharEncoding.Auto
                | _ -> PInvokeCharEncoding.None
            NoMangle = decoder.FindBool "ExactSpelling" false
            LastError = decoder.FindBool "SetLastError" false
            ThrowOnUnmappableChar =
                if (decoder.FindBool "ThrowOnUnmappableChar" false) then
                    PInvokeThrowOnUnmappableChar.Enabled
                else
                    PInvokeThrowOnUnmappableChar.UseAssembly
            CharBestFit =
                if (decoder.FindBool "BestFitMapping" false) then
                    PInvokeCharBestFit.Enabled
                else
                    PInvokeCharBestFit.UseAssembly
        }

    MethodBody.PInvoke(lazy pinvoke)

and GenBindings cenv cgbuf eenv binds stateVarFlagsOpt =
    match stateVarFlagsOpt with
    | None -> binds |> List.iter (fun bind -> GenBinding cenv cgbuf eenv bind false)
    | Some stateVarFlags ->
        (binds, stateVarFlags)
        ||> List.iter2 (fun bind isStateVar -> GenBinding cenv cgbuf eenv bind isStateVar)

//-------------------------------------------------------------------------
// Generate locals and other storage of values
//-------------------------------------------------------------------------

and GenSetVal cenv cgbuf eenv (vref, e, m) sequel =
    let storage = StorageForValRef m vref eenv
    GetStoreValCtxt cgbuf eenv vref.Deref
    GenExpr cenv cgbuf eenv e Continue
    GenSetStorage vref.Range cgbuf storage
    GenUnitThenSequel cenv eenv m eenv.cloc cgbuf sequel

and GenGetValRefAndSequel cenv cgbuf eenv m (v: ValRef) storeSequel =
    let ty = v.Type
    GenGetStorageAndSequel cenv cgbuf eenv m (ty, GenType cenv m eenv.tyenv ty) (StorageForValRef m v eenv) storeSequel

and GenGetVal cenv cgbuf eenv (v: ValRef, m) sequel =
    GenGetValRefAndSequel cenv cgbuf eenv m v None
    GenSequel cenv eenv.cloc cgbuf sequel

and GenBindingRhs cenv cgbuf eenv (vspec: Val) expr =
    let g = cenv.g

    match expr with
    | Expr.TyLambda _
    | Expr.Lambda _ ->

        match IsLocalErasedTyLambda eenv vspec expr with
        | Some body -> GenExpr cenv cgbuf eenv body Continue
        | None ->
            let isLocalTypeFunc = IsNamedLocalTypeFuncVal g vspec expr
            let thisVars = if isLocalTypeFunc then [] else [ mkLocalValRef vspec ]
            GenLambda cenv cgbuf eenv isLocalTypeFunc thisVars expr Continue
    | _ -> GenExpr cenv cgbuf eenv expr Continue

and CommitStartScope cgbuf startMarkOpt =
    match startMarkOpt with
    | None -> ()
    | Some startMark -> cgbuf.SetMarkToHere startMark

and EmitInitLocal cgbuf ty idx =
    CG.EmitInstrs cgbuf (pop 0) Push0 [ I_ldloca(uint16 idx); (I_initobj ty) ]

and EmitSetLocal cgbuf idx =
    CG.EmitInstr cgbuf (pop 1) Push0 (mkStloc (uint16 idx))

and EmitGetLocal cgbuf ty idx =
    CG.EmitInstr cgbuf (pop 0) (Push [ ty ]) (mkLdloc (uint16 idx))

and EmitSetStaticField cgbuf fspec =
    CG.EmitInstr cgbuf (pop 1) Push0 (mkNormalStsfld fspec)

and EmitGetStaticFieldAddr cgbuf ty fspec =
    CG.EmitInstr cgbuf (pop 0) (Push [ ty ]) (I_ldsflda fspec)

and EmitGetStaticField cgbuf ty fspec =
    CG.EmitInstr cgbuf (pop 0) (Push [ ty ]) (mkNormalLdsfld fspec)

and GenSetStorage m cgbuf storage =
    match storage with
    | Local (idx, _, _) -> EmitSetLocal cgbuf idx

    | StaticPropertyWithField (_, _, hasLiteralAttr, ilContainerTy, _, _, _, ilSetterMethRef, _) ->
        if hasLiteralAttr then
            errorR (Error(FSComp.SR.ilLiteralFieldsCannotBeSet (), m))

        CG.EmitInstr cgbuf (pop 1) Push0 (I_call(Normalcall, mkILMethSpecForMethRefInTy (ilSetterMethRef, ilContainerTy, []), None))

    | StaticProperty (ilGetterMethSpec, _) -> error (Error(FSComp.SR.ilStaticMethodIsNotLambda (ilGetterMethSpec.Name), m))

    | Method (_, _, mspec, _, m, _, _, _, _, _, _, _) -> error (Error(FSComp.SR.ilStaticMethodIsNotLambda (mspec.Name), m))

    | Null -> CG.EmitInstr cgbuf (pop 1) Push0 AI_pop

    | Arg _ -> error (Error(FSComp.SR.ilMutableVariablesCannotEscapeMethod (), m))

    | Env (_, ilField, _) ->
        // Note: ldarg0 has already been emitted in GenSetVal
        CG.EmitInstr cgbuf (pop 2) Push0 (mkNormalStfld ilField)

and CommitGetStorageSequel cenv cgbuf eenv m ty localCloInfo storeSequel =
    match localCloInfo, storeSequel with
    | Some (_,
            {
                contents = NamedLocalIlxClosureInfoGenerator _cloinfo
            }),
      _ -> error (InternalError("Unexpected generator", m))

    | Some (_,
            {
                contents = NamedLocalIlxClosureInfoGenerated cloinfo
            }),
      Some (tyargs, args, m, sequel) when not (isNil tyargs) ->
        let actualRetTy = GenNamedLocalTyFuncCall cenv cgbuf eenv ty cloinfo tyargs m
        CommitGetStorageSequel cenv cgbuf eenv m actualRetTy None (Some([], args, m, sequel))

    | _, None -> ()

    | _, Some ([], [], _, sequel) -> GenSequel cenv eenv.cloc cgbuf sequel

    | _, Some (tyargs, args, m, sequel) -> GenCurriedArgsAndIndirectCall cenv cgbuf eenv (ty, tyargs, args, m) sequel

and GenGetStorageAndSequel (cenv: cenv) cgbuf eenv m (ty, ilTy) storage storeSequel =
    let g = cenv.g

    match storage with
    | Local (idx, _, localCloInfo) ->
        EmitGetLocal cgbuf ilTy idx
        CommitGetStorageSequel cenv cgbuf eenv m ty localCloInfo storeSequel

    | StaticPropertyWithField (fspec, _, hasLiteralAttr, ilContainerTy, _, _, ilGetterMethRef, _, _) ->
        // References to literals go directly to the field - no property is used
        if hasLiteralAttr then
            EmitGetStaticField cgbuf ilTy fspec
        else
            CG.EmitInstr
                cgbuf
                (pop 0)
                (Push [ ilTy ])
                (I_call(Normalcall, mkILMethSpecForMethRefInTy (ilGetterMethRef, ilContainerTy, []), None))

        CommitGetStorageSequel cenv cgbuf eenv m ty None storeSequel

    | StaticProperty (ilGetterMethSpec, _) ->
        CG.EmitInstr cgbuf (pop 0) (Push [ ilTy ]) (I_call(Normalcall, ilGetterMethSpec, None))
        CommitGetStorageSequel cenv cgbuf eenv m ty None storeSequel

    | Method (valReprInfo, vref, _, _, _, _, _, _, _, _, _, _) ->
        // Get a toplevel value as a first-class value.
        // We generate a lambda expression and that simply calls
        // the toplevel method. However we optimize the case where we are
        // immediately applying the value anyway (to insufficient arguments).

        // First build a lambda expression for the saturated use of the toplevel value...
        // REVIEW: we should NOT be doing this in the backend...
        let expr, exprTy = AdjustValForExpectedValReprInfo g m vref NormalValUse valReprInfo

        // Then reduce out any arguments (i.e. apply the sequel immediately if we can...)
        match storeSequel with
        | None -> GenLambda cenv cgbuf eenv false [] expr Continue
        | Some (tyargs', args, m, sequel) ->
            let specializedExpr =
                if isNil args && isNil tyargs' then
                    failwith ("non-lambda at use of method " + vref.LogicalName)

                MakeApplicationAndBetaReduce cenv.g (expr, exprTy, [ tyargs' ], args, m)

            GenExpr cenv cgbuf eenv specializedExpr sequel

    | Null ->
        CG.EmitInstr cgbuf (pop 0) (Push [ ilTy ]) AI_ldnull
        CommitGetStorageSequel cenv cgbuf eenv m ty None storeSequel

    | Arg i ->
        CG.EmitInstr cgbuf (pop 0) (Push [ ilTy ]) (mkLdarg (uint16 i))
        CommitGetStorageSequel cenv cgbuf eenv m ty None storeSequel

    | Env (_, ilField, localCloInfo) ->
        CG.EmitInstrs cgbuf (pop 0) (Push [ ilTy ]) [ mkLdarg0; mkNormalLdfld ilField ]
        CommitGetStorageSequel cenv cgbuf eenv m ty localCloInfo storeSequel

and GenGetLocalVals cenv cgbuf eenvouter m fvs =
    List.iter (fun v -> GenGetLocalVal cenv cgbuf eenvouter m v None) fvs

and GenGetLocalVal cenv cgbuf eenv m (vspec: Val) storeSequel =
    GenGetStorageAndSequel cenv cgbuf eenv m (vspec.Type, GenTypeOfVal cenv eenv vspec) (StorageForVal m vspec eenv) storeSequel

and GenGetLocalVRef cenv cgbuf eenv m (vref: ValRef) storeSequel =
    GenGetStorageAndSequel cenv cgbuf eenv m (vref.Type, GenTypeOfVal cenv eenv vref.Deref) (StorageForValRef m vref eenv) storeSequel

and GenStoreVal cgbuf eenv m (vspec: Val) =
    GenSetStorage vspec.Range cgbuf (StorageForVal m vspec eenv)

/// Allocate IL locals
and AllocLocal cenv cgbuf eenv compgen (v, ty, isFixed) (scopeMarks: Mark * Mark) : int * _ * _ =
    // The debug range for the local
    let ranges = if compgen then [] else [ (v, scopeMarks) ]
    // Get an index for the local
    let j, realloc =
        if cenv.options.localOptimizationsEnabled then
            cgbuf.ReallocLocal(
                (fun i (_, ty2, isFixed2) -> not isFixed2 && not isFixed && not (IntMap.mem i eenv.liveLocals) && (ty = ty2)),
                ranges,
                ty,
                isFixed
            )
        else
            cgbuf.AllocLocal(ranges, ty, isFixed), false

    j,
    realloc,
    { eenv with
        liveLocals = IntMap.add j () eenv.liveLocals
    }

/// Decide storage for local value and if necessary allocate an ILLocal for it
and AllocLocalVal cenv cgbuf v eenv repr scopeMarks =
    let g = cenv.g

    let repr, eenv =
        let ty = v.Type

        if isUnitTy g ty && not v.IsMutable then
            Null, eenv
        else
            match repr with
            | Some repr when IsNamedLocalTypeFuncVal g v repr ->
                let ftyvs = (freeInExpr CollectTypars repr).FreeTyvars
                // known, named, non-escaping type functions
                let cloinfoGenerate eenv =
                    let eenvinner =
                        { eenv with
                            letBoundVars = (mkLocalValRef v) :: eenv.letBoundVars
                        }

                    let cloinfo, _, _ =
                        GetIlxClosureInfo cenv v.Range ILBoxity.AsObject true true [] eenvinner repr

                    cloinfo

                let idx, realloc, eenv =
                    AllocLocal
                        cenv
                        cgbuf
                        eenv
                        v.IsCompilerGenerated
                        (v.CompiledName g.CompilerGlobalState, g.ilg.typ_Object, false)
                        scopeMarks

                Local(idx, realloc, Some(ftyvs, ref (NamedLocalIlxClosureInfoGenerator cloinfoGenerate))), eenv
            | _ ->
                // normal local
                let idx, realloc, eenv =
                    AllocLocal
                        cenv
                        cgbuf
                        eenv
                        v.IsCompilerGenerated
                        (v.CompiledName g.CompilerGlobalState, GenTypeOfVal cenv eenv v, v.IsFixed)
                        scopeMarks

                Local(idx, realloc, None), eenv

    let eenv = AddStorageForVal g (v, notlazy repr) eenv
    repr, eenv

and AllocStorageForBind cenv cgbuf scopeMarks eenv bind =
    AllocStorageForBinds cenv cgbuf scopeMarks eenv [ bind ]

and AllocStorageForBinds cenv cgbuf scopeMarks eenv binds =
    // phase 1 - decide representations - most are very simple.
    let reps, eenv = List.mapFold (AllocValForBind cenv cgbuf scopeMarks) eenv binds

    // Phase 2 - run the cloinfo generators for NamedLocalClosure values against the environment recording the
    // representation choices.
    reps
    |> List.iter (fun reprOpt ->
        match reprOpt with
        | Some repr ->
            match repr with
            | Local (_, _, Some (_, g))
            | Env (_, _, Some (_, g)) ->
                match g.Value with
                | NamedLocalIlxClosureInfoGenerator f -> g.Value <- NamedLocalIlxClosureInfoGenerated(f eenv)
                | NamedLocalIlxClosureInfoGenerated _ -> ()
            | _ -> ()
        | _ -> ())

    eenv

and AllocValForBind cenv cgbuf (scopeMarks: Mark * Mark) eenv (TBind (v, repr, _)) =
    match v.ValReprInfo with
    | None ->
        let repr, eenv = AllocLocalVal cenv cgbuf v eenv (Some repr) scopeMarks
        Some repr, eenv
    | Some _ -> None, AllocValReprWithinExpr cenv cgbuf (snd scopeMarks) eenv.cloc v eenv

and AllocValReprWithinExpr cenv cgbuf endMark cloc v eenv =
    let g = cenv.g

    // decide whether to use a shadow local or not
    let useShadowLocal =
        cenv.options.generateDebugSymbols
        && not cenv.options.localOptimizationsEnabled
        && not v.IsCompilerGenerated
        && not v.IsMutable
        &&
        // Don't use shadow locals for things like functions which are not compiled as static values/properties
        IsCompiledAsStaticProperty g v

    let optShadowLocal, eenv =
        if useShadowLocal then
            let startMark = CG.GenerateDelayMark cgbuf ("start_" + v.LogicalName)
            let storage, eenv = AllocLocalVal cenv cgbuf v eenv None (startMark, endMark)
            ShadowLocal(startMark, storage), eenv
        else
            NoShadowLocal, eenv

    ComputeAndAddStorageForLocalValWithValReprInfo (cenv, g, cenv.intraAssemblyInfo, cenv.options.isInteractive, optShadowLocal) cloc v eenv

//--------------------------------------------------------------------------
// Generate stack save/restore and assertions - pulled into letrec by alloc*
//--------------------------------------------------------------------------

/// Save the stack
/// - [gross] because IL flushes the stack at the exn. handler
/// - and because IL requires empty stack following a forward br (jump).
and EmitSaveStack cenv cgbuf eenv m scopeMarks =
    let savedStack = (cgbuf.GetCurrentStack())

    let savedStackLocals, eenvinner =
        (eenv, savedStack)
        ||> List.mapFold (fun eenv ty ->
            let idx, _realloc, eenv =
                // Ensure that we have an g.CompilerGlobalState
                assert (cenv.g.CompilerGlobalState |> Option.isSome)

                AllocLocal
                    cenv
                    cgbuf
                    eenv
                    true
                    (cenv.g.CompilerGlobalState.Value.IlxGenNiceNameGenerator.FreshCompilerGeneratedName("spill", m), ty, false)
                    scopeMarks

            idx, eenv)

    List.iter (EmitSetLocal cgbuf) savedStackLocals
    cgbuf.AssertEmptyStack()
    (savedStack, savedStackLocals), eenvinner (* need to return, it marks locals "live" *)

/// Restore the stack and load the result
and EmitRestoreStack cgbuf (savedStack, savedStackLocals) =
    cgbuf.AssertEmptyStack()
    List.iter2 (EmitGetLocal cgbuf) (List.rev savedStack) (List.rev savedStackLocals)

//-------------------------------------------------------------------------
//GenAttr: custom attribute generation
//-------------------------------------------------------------------------

and GenAttribArg amap g eenv x (ilArgTy: ILType) =

    match stripDebugPoints x, ilArgTy with
    // Detect 'null' used for an array argument
    | Expr.Const (Const.Zero, _, _), ILType.Array _ -> ILAttribElem.Null

    // Detect standard constants
    | Expr.Const (c, m, _), _ ->
        let tynm = ilArgTy.TypeSpec.Name
        let isobj = (tynm = "System.Object")

        match c with
        | Const.Bool b -> ILAttribElem.Bool b
        | Const.Int32 i when isobj || tynm = "System.Int32" -> ILAttribElem.Int32 i
        | Const.Int32 i when tynm = "System.SByte" -> ILAttribElem.SByte(sbyte i)
        | Const.Int32 i when tynm = "System.Int16" -> ILAttribElem.Int16(int16 i)
        | Const.Int32 i when tynm = "System.Byte" -> ILAttribElem.Byte(byte i)
        | Const.Int32 i when tynm = "System.UInt16" -> ILAttribElem.UInt16(uint16 i)
        | Const.Int32 i when tynm = "System.UInt32" -> ILAttribElem.UInt32(uint32 i)
        | Const.Int32 i when tynm = "System.UInt64" -> ILAttribElem.UInt64(uint64 (int64 i))
        | Const.SByte i -> ILAttribElem.SByte i
        | Const.Int16 i -> ILAttribElem.Int16 i
        | Const.Int32 i -> ILAttribElem.Int32 i
        | Const.Int64 i -> ILAttribElem.Int64 i
        | Const.Byte i -> ILAttribElem.Byte i
        | Const.UInt16 i -> ILAttribElem.UInt16 i
        | Const.UInt32 i -> ILAttribElem.UInt32 i
        | Const.UInt64 i -> ILAttribElem.UInt64 i
        | Const.Double i -> ILAttribElem.Double i
        | Const.Single i -> ILAttribElem.Single i
        | Const.Char i -> ILAttribElem.Char i
        | Const.Zero when isobj -> ILAttribElem.Null
        | Const.Zero when tynm = "System.String" -> ILAttribElem.String None
        | Const.Zero when tynm = "System.Type" -> ILAttribElem.Type None
        | Const.String i when isobj || tynm = "System.String" -> ILAttribElem.String(Some i)
        | _ -> error (InternalError("The type '" + tynm + "' may not be used as a custom attribute value", m))

    // Detect '[| ... |]' nodes
    | Expr.Op (TOp.Array, [ elemTy ], args, m), _ ->
        let ilElemTy = GenType amap m eenv.tyenv elemTy
        ILAttribElem.Array(ilElemTy, List.map (fun arg -> GenAttribArg amap g eenv arg ilElemTy) args)

    // Detect 'typeof<ty>' calls
    | TypeOfExpr g ty, _ -> ILAttribElem.Type(Some(GenType amap x.Range eenv.tyenv ty))

    // Detect 'typedefof<ty>' calls
    | TypeDefOfExpr g ty, _ -> ILAttribElem.TypeRef(Some (GenType amap x.Range eenv.tyenv ty).TypeRef)

    // Ignore upcasts
    | Expr.Op (TOp.Coerce, _, [ arg2 ], _), _ -> GenAttribArg amap g eenv arg2 ilArgTy

    // Detect explicit enum values
    | EnumExpr g arg1, _ -> GenAttribArg amap g eenv arg1 ilArgTy

    // Detect bitwise or of attribute flags: one case of constant folding (a more general treatment is needed)

    | AttribBitwiseOrExpr g (arg1, arg2), _ ->
        let v1 = GenAttribArg amap g eenv arg1 ilArgTy
        let v2 = GenAttribArg amap g eenv arg2 ilArgTy

        match v1, v2 with
        | ILAttribElem.SByte i1, ILAttribElem.SByte i2 -> ILAttribElem.SByte(i1 ||| i2)
        | ILAttribElem.Int16 i1, ILAttribElem.Int16 i2 -> ILAttribElem.Int16(i1 ||| i2)
        | ILAttribElem.Int32 i1, ILAttribElem.Int32 i2 -> ILAttribElem.Int32(i1 ||| i2)
        | ILAttribElem.Int64 i1, ILAttribElem.Int64 i2 -> ILAttribElem.Int64(i1 ||| i2)
        | ILAttribElem.Byte i1, ILAttribElem.Byte i2 -> ILAttribElem.Byte(i1 ||| i2)
        | ILAttribElem.UInt16 i1, ILAttribElem.UInt16 i2 -> ILAttribElem.UInt16(i1 ||| i2)
        | ILAttribElem.UInt32 i1, ILAttribElem.UInt32 i2 -> ILAttribElem.UInt32(i1 ||| i2)
        | ILAttribElem.UInt64 i1, ILAttribElem.UInt64 i2 -> ILAttribElem.UInt64(i1 ||| i2)
        | _ -> error (InternalError("invalid custom attribute value (not a valid constant): " + showL (exprL x), x.Range))

    // Other expressions are not valid custom attribute values
    | _ -> error (InternalError("invalid custom attribute value (not a constant): " + showL (exprL x), x.Range))

and GenAttr cenv g eenv (Attrib (_, k, args, props, _, _, _)) =
    let props =
        props
        |> List.map (fun (AttribNamedArg (s, ty, fld, AttribExpr (_, expr))) ->
            let m = expr.Range
            let ilTy = GenType cenv m eenv.tyenv ty
            let cval = GenAttribArg cenv g eenv expr ilTy
            (s, ilTy, fld, cval))

    let mspec =
        match k with
        | ILAttrib mref -> mkILMethSpec (mref, AsObject, [], [])
        | FSAttrib vref ->
            assert vref.IsMember

            let mspec, _, _, _, _, _, _, _, _, _ =
                GetMethodSpecForMemberVal cenv (Option.get vref.MemberInfo) vref

            mspec

    let ilArgs =
        List.map2 (fun (AttribExpr (_, vexpr)) ty -> GenAttribArg cenv g eenv vexpr ty) args mspec.FormalArgTypes

    mkILCustomAttribMethRef (mspec, ilArgs, props)

and GenAttrs cenv eenv attrs =
    List.map (GenAttr cenv cenv.g eenv) attrs

and GenCompilationArgumentCountsAttr cenv (v: Val) =
    let g = cenv.g

    [
        match v.ValReprInfo with
        | Some tvi when v.IsMemberOrModuleBinding ->
            let arities =
                if ValSpecIsCompiledAsInstance g v then
                    List.tail tvi.AritiesOfArgs
                else
                    tvi.AritiesOfArgs

            if arities.Length > 1 then
                yield mkCompilationArgumentCountsAttr g arities
        | _ -> ()
    ]

// Create a permission set for a list of security attributes
and CreatePermissionSets cenv eenv (securityAttributes: Attrib list) =
    let g = cenv.g

    [
        for Attrib (tcref, _, actions, _, _, _, _) as attr in securityAttributes do
            let action =
                match actions with
                | [ AttribInt32Arg act ] -> act
                | _ -> failwith "internal error: unrecognized security action"

            let secaction = (List.assoc action (Lazy.force ILSecurityActionRevMap))
            let tref = tcref.CompiledRepresentationForNamedType
            let ilattr = GenAttr cenv g eenv attr

            let _, ilNamedArgs =
                match TryDecodeILAttribute tref (mkILCustomAttrs [ ilattr ]) with
                | Some (ae, na) -> ae, na
                | _ -> [], []

            let setArgs = ilNamedArgs |> List.map (fun (n, ilt, _, ilae) -> (n, ilt, ilae))
            yield mkPermissionSet (secaction, [ (tref, setArgs) ])
    ]

//--------------------------------------------------------------------------
// Generate the set of modules for an assembly, and the declarations in each module
//--------------------------------------------------------------------------

/// Generate a static class at the given cloc
and GenTypeDefForCompLoc (cenv, eenv, mgbuf: AssemblyBuilder, cloc, hidden, attribs, initTrigger, eliminateIfEmpty, addAtEnd) =
    let g = cenv.g
    let tref = TypeRefForCompLoc cloc

    let tdef =
        mkILSimpleClass
            g.ilg
            (tref.Name,
             ComputeTypeAccess tref hidden,
             emptyILMethods,
             emptyILFields,
             emptyILTypeDefs,
             emptyILProperties,
             emptyILEvents,
             mkILCustomAttrs (
                 GenAttrs cenv eenv attribs
                 @ (if
                        List.contains
                            tref.Name
                            [
                                TypeNameForImplicitMainMethod cloc
                                TypeNameForInitClass cloc
                                TypeNameForPrivateImplementationDetails cloc
                            ]
                    then
                        []
                    else
                        [ mkCompilationMappingAttr g (int SourceConstructFlags.Module) ])
             ),
             initTrigger)

    let tdef = tdef.WithSealed(true).WithAbstract(true)
    mgbuf.AddTypeDef(tref, tdef, eliminateIfEmpty, addAtEnd, None)

and GenImplFileContents cenv cgbuf qname lazyInitInfo eenv mty def =
    // REVIEW: the scopeMarks are used for any shadow locals we create for the module bindings
    // We use one scope for all the bindings in the module, which makes them all appear with their "default" values
    // rather than incrementally as we step through the initializations in the module. This is a little unfortunate
    // but stems from the way we add module values all at once before we generate the module itself.
    LocalScope "module" cgbuf (fun (_, endMark) ->
        let sigToImplRemapInfo =
            ComputeRemappingFromImplementationToSignature cenv.g def mty

        let eenv = AddSignatureRemapInfo "defs" sigToImplRemapInfo eenv

        // Allocate all the values, including any shadow locals for static fields
        let eenv =
            AddBindingsForModuleOrNamespaceContents (AllocValReprWithinExpr cenv cgbuf endMark) eenv.cloc eenv def

        let _eenvEnd = GenModuleOrNamespaceContents cenv cgbuf qname lazyInitInfo eenv def
        ())

and GenModuleOrNamespaceContents cenv (cgbuf: CodeGenBuffer) qname lazyInitInfo eenv x =
    match x with
    | TMDefRec (_isRec, opens, tycons, mbinds, m) ->
        let eenvinner = AddDebugImportsToEnv cenv eenv opens

        for tc in tycons do
            if tc.IsFSharpException then
                GenExnDef cenv cgbuf.mgbuf eenvinner m tc
            else
                GenTypeDef cenv cgbuf.mgbuf lazyInitInfo eenvinner m tc

        // Generate chunks of non-nested bindings together to allow recursive fixups.
        let mutable bindsRemaining = mbinds

        while not bindsRemaining.IsEmpty do
            match bindsRemaining with
            | ModuleOrNamespaceBinding.Binding _ :: _ ->
                let recBinds =
                    bindsRemaining
                    |> List.takeWhile (function
                        | ModuleOrNamespaceBinding.Binding _ -> true
                        | _ -> false)
                    |> List.map (function
                        | ModuleOrNamespaceBinding.Binding recBind -> recBind
                        | _ -> failwith "GenModuleOrNamespaceContents - unexpected")

                let otherBinds =
                    bindsRemaining
                    |> List.skipWhile (function
                        | ModuleOrNamespaceBinding.Binding _ -> true
                        | _ -> false)

                GenLetRecBindings cenv cgbuf eenv (recBinds, m)
                bindsRemaining <- otherBinds
            | (ModuleOrNamespaceBinding.Module _ as mbind) :: rest ->
                GenModuleBinding cenv cgbuf qname lazyInitInfo eenvinner m mbind
                bindsRemaining <- rest
            | [] -> failwith "unreachable"

        eenvinner

    | TMDefLet (bind, _) ->
        GenBindings cenv cgbuf eenv [ bind ] None
        eenv

    | TMDefOpens openDecls ->
        let eenvinner = AddDebugImportsToEnv cenv eenv openDecls
        eenvinner

    | TMDefDo (e, _) ->
        GenExpr cenv cgbuf eenv e discard
        eenv

    | TMDefs mdefs ->
        (eenv, mdefs)
        ||> List.fold (GenModuleOrNamespaceContents cenv cgbuf qname lazyInitInfo)

// Generate a module binding
and GenModuleBinding cenv (cgbuf: CodeGenBuffer) (qname: QualifiedNameOfFile) lazyInitInfo eenv m x =
    match x with
    | ModuleOrNamespaceBinding.Binding bind -> GenLetRecBindings cenv cgbuf eenv ([ bind ], m)

    | ModuleOrNamespaceBinding.Module (mspec, mdef) ->
        let hidden = IsHiddenTycon eenv.sigToImplRemapInfo mspec

        let eenvinner =
            if mspec.IsNamespace then
                eenv
            else
                { eenv with
                    cloc = CompLocForFixedModule cenv.options.fragName qname.Text mspec
                    initLocals =
                        eenv.initLocals
                        && not (HasFSharpAttribute cenv.g cenv.g.attrib_SkipLocalsInitAttribute mspec.Attribs)
                }

        // Create the class to hold the contents of this module. No class needed if
        // we're compiling it as a namespace.
        //
        // Most module static fields go into the "InitClass" static class.
        // However mutable static fields go into the class for the module itself.
        // So this static class ends up with a .cctor if it has mutable fields.
        //
        if not mspec.IsNamespace then
            // The use of ILTypeInit.OnAny prevents the execution of the cctor before the
            // "main" method in the case where the "main" method is implicit.
            let staticClassTrigger = (* if eenv.isFinalFile then *)
                ILTypeInit.OnAny (* else ILTypeInit.BeforeField *)

            GenTypeDefForCompLoc(
                cenv,
                eenvinner,
                cgbuf.mgbuf,
                eenvinner.cloc,
                hidden,
                mspec.Attribs,
                staticClassTrigger,
                false (* atEnd= *) ,
                true
            )

        // Generate the declarations in the module and its initialization code
        let _envAtEnd =
            GenModuleOrNamespaceContents cenv cgbuf qname lazyInitInfo eenvinner mdef

        // If the module has a .cctor for some mutable fields, we need to ensure that when
        // those fields are "touched" the InitClass .cctor is forced. The InitClass .cctor will
        // then fill in the value of the mutable fields.
        if
            not mspec.IsNamespace
            && (cgbuf.mgbuf.GetCurrentFields(TypeRefForCompLoc eenvinner.cloc)
                |> Seq.isEmpty
                |> not)
        then
            GenForceWholeFileInitializationAsPartOfCCtor
                cenv
                cgbuf.mgbuf
                lazyInitInfo
                (TypeRefForCompLoc eenvinner.cloc)
                eenv.imports
                mspec.Range

/// Generate the namespace fragments in a single file
and GenImplFile cenv (mgbuf: AssemblyBuilder) mainInfoOpt eenv (implFile: CheckedImplFileAfterOptimization) =
    let (CheckedImplFile (qname, _, signature, contents, hasExplicitEntryPoint, isScript, anonRecdTypes, _)) =
        implFile.ImplFile

    let optimizeDuringCodeGen = implFile.OptimizeDuringCodeGen
    let g = cenv.g
    let m = qname.Range

    // Generate all the anonymous record types mentioned anywhere in this module
    for anonInfo in anonRecdTypes.Values do
        mgbuf.GenerateAnonType((fun ilThisTy -> GenToStringMethod cenv eenv ilThisTy m), anonInfo)

    let eenv =
        { eenv with
            cloc =
                { eenv.cloc with
                    TopImplQualifiedName = qname.Text
                }
        }

    cenv.optimizeDuringCodeGen <- optimizeDuringCodeGen

    // This is used to point the inner classes back to the startup module for initialization purposes
    let isFinalFile = Option.isSome mainInfoOpt

    let initClassCompLoc = CompLocForInitClass eenv.cloc
    let initClassTy = mkILTyForCompLoc initClassCompLoc

    let initClassTrigger = (* if isFinalFile then *)
        ILTypeInit.OnAny (* else ILTypeInit.BeforeField *)

    let eenv =
        { eenv with
            cloc = initClassCompLoc
            isFinalFile = isFinalFile
            someTypeInThisAssembly = initClassTy
        }

    // Create the class to hold the initialization code and static fields for this file.
    //     internal static class $<StartupCode...> {}
    // Put it at the end since that gives an approximation of dependency order (to aid FSI.EXE's code generator - see FSharp 1.0 5548)
    GenTypeDefForCompLoc(cenv, eenv, mgbuf, initClassCompLoc, useHiddenInitCode, [], initClassTrigger, false, true)

    // lazyInitInfo is an accumulator of functions which add the forced initialization of the storage module to
    //    - mutable fields in public modules
    //    - static "let" bindings in types
    // These functions only get executed/committed if we actually end up producing some code for the .cctor for the storage module.
    // The existence of .cctors adds costs to execution, so this is a half-sensible attempt to avoid adding them when possible.
    let lazyInitInfo =
        ResizeArray<ILFieldSpec -> ILInstr list -> ILInstr list -> unit>()

    // codegen .cctor/main for outer module
    let clocCcu = CompLocForCcu cenv.viewCcu

    // This method name is only used internally in ilxgen.fs to aid debugging
    let methodName =
        match mainInfoOpt with
        //   Library file
        | None -> ".cctor"
        //   Final file, explicit entry point
        | Some _ when hasExplicitEntryPoint -> ".cctor"
        //   Final file, implicit entry point
        | Some _ -> mainMethName

    // topInstrs is ILInstr[] and contains the abstract IL for this file's top-level actions. topCode is the ILMethodBody for that same code.
    let topInstrs, topCode =
        CodeGenMethod
            cenv
            mgbuf
            ([],
             methodName,
             eenv,
             0,
             None,
             (fun cgbuf eenv ->
                 GenImplFileContents cenv cgbuf qname lazyInitInfo eenv signature contents
                 CG.EmitInstr cgbuf (pop 0) Push0 I_ret),
             m)

    // The code generation for the initialization is now complete and the IL code is in topCode.
    // Make a .cctor and/or main method to contain the code. This initializes all modules.
    //   Library file (mainInfoOpt = None) : optional .cctor if topCode has initialization effect
    //   Final file, explicit entry point (mainInfoOpt = Some _, GetExplicitEntryPointInfo() = Some) : main + optional .cctor if topCode has initialization effect
    //   Final file, implicit entry point (mainInfoOpt = Some _, GetExplicitEntryPointInfo() = None) : main + initialize + optional .cctor calling initialize
    let doesSomething = CheckCodeDoesSomething topCode.Code

    // Make a FEEFEE instruction to mark hidden code regions
    // We expect the first instruction to be a debug point when generating debug symbols
    let feefee, seqpt =
        if topInstrs.Length > 1 then
            match topInstrs[0] with
            | I_seqpoint sp as i -> [ FeeFeeInstr cenv sp.Document ], [ i ]
            | _ -> [], []
        else
            [], []

    match mainInfoOpt with
    // Final file in .EXE
    | Some mainInfo ->
        // Generate an explicit main method. If necessary, make a class constructor as
        // well for the bindings earlier in the file containing the entry point.
        match mgbuf.GetExplicitEntryPointInfo() with
        // Final file, explicit entry point: place the code in a .cctor, and add code to main that forces the .cctor (if topCode has initialization effect).
        | Some tref ->
            if doesSomething then
                lazyInitInfo.Add(fun fspec feefee seqpt ->
                    // This adds the explicit init of the .cctor to the explicit entry point main method
                    let ilDebugRange = GenPossibleILDebugRange cenv m

                    mgbuf.AddExplicitInitToSpecificMethodDef(
                        (fun md -> md.IsEntryPoint),
                        tref,
                        fspec,
                        ilDebugRange,
                        eenv.imports,
                        feefee,
                        seqpt
                    ))

                let cctorMethDef = mkILClassCtor (MethodBody.IL(lazy topCode))
                mgbuf.AddMethodDef(initClassTy.TypeRef, cctorMethDef)

        // Final file, implicit entry point. We generate no .cctor.
        //       void main@() {
        //             <topCode>
        //    }
        | None ->
            let ilAttrs = mkILCustomAttrs (GenAttrs cenv eenv mainInfo)

            if not cenv.options.isInteractive && not doesSomething then
                let errorM = m.EndRange
                warning (Error(FSComp.SR.ilMainModuleEmpty (), errorM))

            // generate main@
            let ilMainMethodDef =
                let mdef =
                    mkILNonGenericStaticMethod (
                        mainMethName,
                        ILMemberAccess.Public,
                        [],
                        mkILReturn ILType.Void,
                        MethodBody.IL(lazy topCode)
                    )

                mdef.With(isEntryPoint = true, customAttrs = ilAttrs)

            mgbuf.AddMethodDef(initClassTy.TypeRef, ilMainMethodDef)

    //   Library file: generate an optional .cctor if topCode has initialization effect
    | None ->
        if doesSomething then
            // Add the cctor
            let cctorMethDef = mkILClassCtor (MethodBody.IL(lazy topCode))
            mgbuf.AddMethodDef(initClassTy.TypeRef, cctorMethDef)

    // Commit the directed initializations
    if doesSomething then
        // Create the field to act as the target for the forced initialization.
        // Why do this for the final file?
        // There is no need to do this for a final file with an implicit entry point. For an explicit entry point in lazyInitInfo.
        let initFieldName = CompilerGeneratedName "init"

        let ilFieldDef =
            mkILStaticField (initFieldName, g.ilg.typ_Int32, None, None, ComputeMemberAccess true)
            |> g.AddFieldNeverAttributes
            |> g.AddFieldGeneratedAttributes

        let fspec = mkILFieldSpecInTy (initClassTy, initFieldName, cenv.g.ilg.typ_Int32)
        CountStaticFieldDef()
        mgbuf.AddFieldDef(initClassTy.TypeRef, ilFieldDef)

        // Run the imperative (yuck!) actions that force the generation
        // of references to the cctor for nested modules etc.
        lazyInitInfo |> Seq.iter (fun f -> f fspec feefee seqpt)

        if isScript && not isFinalFile then
            mgbuf.AddScriptInitFieldSpec(fspec, m)

    // Compute the ilxgenEnv after the generation of the module, i.e. the residue need to generate anything that
    // uses the constructs exported from this module.
    // We add the module type all over again. Note no shadow locals for static fields needed here since they are only relevant to the main/.cctor
    let eenvafter =
        let allocVal =
            ComputeAndAddStorageForLocalValWithValReprInfo(cenv, g, cenv.intraAssemblyInfo, cenv.options.isInteractive, NoShadowLocal)

        AddBindingsForLocalModuleOrNamespaceType allocVal clocCcu eenv signature

    eenvafter

and GenForceWholeFileInitializationAsPartOfCCtor cenv (mgbuf: AssemblyBuilder) (lazyInitInfo: ResizeArray<_>) tref imports m =
    // Authoring a .cctor with effects forces the cctor for the 'initialization' module by doing a dummy store & load of a field
    // Doing both a store and load keeps FxCop happier because it thinks the field is useful
    lazyInitInfo.Add(fun fspec feefee seqpt ->
        let ilDebugRange = GenPossibleILDebugRange cenv m
        mgbuf.AddExplicitInitToSpecificMethodDef((fun md -> md.Name = ".cctor"), tref, fspec, ilDebugRange, imports, feefee, seqpt))

/// Generate an Equals method.
and GenEqualsOverrideCallingIComparable cenv (tcref: TyconRef, ilThisTy, _ilThatTy) =
    let g = cenv.g

    let mspec =
        mkILNonGenericInstanceMethSpecInTy (g.iltyp_IComparable, "CompareTo", [ g.ilg.typ_Object ], g.ilg.typ_Int32)

    let ilInstrs =
        [
            mkLdarg0
            mkLdarg 1us
            if tcref.IsStructOrEnumTycon then
                I_callconstraint(Normalcall, ilThisTy, mspec, None)
            else
                I_callvirt(Normalcall, mspec, None)
            mkLdcInt32 0
            AI_ceq
        ]

    let ilMethodBody =
        mkMethodBody (true, [], 2, nonBranchingInstrsToCode ilInstrs, None, None)

    mkILNonGenericVirtualMethod (
        "Equals",
        ILMemberAccess.Public,
        [ mkILParamNamed ("obj", g.ilg.typ_Object) ],
        mkILReturn g.ilg.typ_Bool,
        ilMethodBody
    )
    |> AddNonUserCompilerGeneratedAttribs g

and GenFieldInit m c =
    match c with
    | ConstToILFieldInit fieldInit -> fieldInit
    | _ -> error (Error(FSComp.SR.ilTypeCannotBeUsedForLiteralField (), m))

and GenWitnessParams cenv eenv m (witnessInfos: TraitWitnessInfos) =
    ((Set.empty, 0), witnessInfos)
    ||> List.mapFold (fun (used, i) witnessInfo ->
        let ty = GenWitnessTy cenv.g witnessInfo
        let nm = String.uncapitalize witnessInfo.MemberName
        let nm = if used.Contains nm then nm + string i else nm

        let ilParam: ILParameter =
            {
                Name = Some nm
                Type = GenType cenv m eenv.tyenv ty
                Default = None
                Marshal = None
                IsIn = false
                IsOut = false
                IsOptional = false
                CustomAttrsStored = storeILCustomAttrs (mkILCustomAttrs [])
                MetadataIndex = NoMetadataIdx
            }

        ilParam, (used.Add nm, i + 1))
    |> fst

and GenAbstractBinding cenv eenv tref (vref: ValRef) =
    assert vref.IsMember
    let g = cenv.g
    let m = vref.Range
    let memberInfo = Option.get vref.MemberInfo
    let attribs = vref.Attribs

    let hasPreserveSigImplFlag, hasSynchronizedImplFlag, hasNoInliningFlag, hasAggressiveInliningImplFlag, attribs =
        ComputeMethodImplAttribs cenv vref.Deref attribs

    if memberInfo.MemberFlags.IsDispatchSlot && not memberInfo.IsImplemented then
        let mspec, _mspecW, ctps, mtps, _curriedArgInfos, argInfos, retInfo, witnessInfos, methArgTys, returnTy =
            GetMethodSpecForMemberVal cenv memberInfo vref

        let ilAttrs =
            [
                yield! GenAttrs cenv eenv attribs
                yield! GenCompilationArgumentCountsAttr cenv vref.Deref

                match vref.MemberInfo, returnTy with
                | Some memberInfo, Some returnTy when
                    memberInfo.MemberFlags.MemberKind = SynMemberKind.PropertyGet
                    || memberInfo.MemberFlags.MemberKind = SynMemberKind.PropertySet
                    || memberInfo.MemberFlags.MemberKind = SynMemberKind.PropertyGetSet
                    ->
                    match GenReadOnlyAttributeIfNecessary g returnTy with
                    | Some ilAttr -> ilAttr
                    | _ -> ()
                | _ -> ()
            ]

        assert witnessInfos.IsEmpty

        let eenvForMeth = EnvForTypars (ctps @ mtps) eenv
        let ilMethTypars = GenGenericParams cenv eenvForMeth mtps

        let ilReturn =
            GenReturnInfo cenv eenvForMeth returnTy mspec.FormalReturnType retInfo

        let ilParams = GenParams cenv eenvForMeth m mspec [] argInfos methArgTys None

        let compileAsInstance = ValRefIsCompiledAsInstanceMember g vref

        let mdef =
            mkILGenericVirtualMethod (
                vref.CompiledName g.CompilerGlobalState,
                ILMemberAccess.Public,
                ilMethTypars,
                ilParams,
                ilReturn,
                MethodBody.Abstract
            )

        let mdef = fixupVirtualSlotFlags mdef

        let mdef =
            if mdef.IsVirtual then
                mdef
                    .WithFinal(memberInfo.MemberFlags.IsFinal)
                    .WithAbstract(memberInfo.MemberFlags.IsDispatchSlot)
            else
                mdef

        let mdef =
            mdef
                .WithPreserveSig(hasPreserveSigImplFlag)
                .WithSynchronized(hasSynchronizedImplFlag)
                .WithNoInlining(hasNoInliningFlag)
                .WithAggressiveInlining(hasAggressiveInliningImplFlag)

        match memberInfo.MemberFlags.MemberKind with
        | SynMemberKind.ClassConstructor
        | SynMemberKind.Constructor
        | SynMemberKind.Member ->
            let mdef = mdef.With(customAttrs = mkILCustomAttrs ilAttrs)
            [ mdef ], [], []
        | SynMemberKind.PropertyGetSet -> error (Error(FSComp.SR.ilUnexpectedGetSetAnnotation (), m))
        | SynMemberKind.PropertySet
        | SynMemberKind.PropertyGet ->
            let v = vref.Deref
            let vtyp = ReturnTypeOfPropertyVal g v

            if CompileAsEvent g attribs then

                let edef = GenEventForProperty cenv eenvForMeth mspec v ilAttrs m vtyp
                [], [], [ edef ]
            else
                let ilPropDef =
                    let ilPropTy = GenType cenv m eenvForMeth.tyenv vtyp
                    let ilPropTy = GenReadOnlyModReqIfNecessary g vtyp ilPropTy

                    let ilArgTys =
                        v
                        |> ArgInfosOfPropertyVal g
                        |> List.map fst
                        |> GenTypes cenv m eenvForMeth.tyenv

                    GenPropertyForMethodDef compileAsInstance tref mdef v memberInfo ilArgTys ilPropTy (mkILCustomAttrs ilAttrs) None

                let mdef = mdef.WithSpecialName
                [ mdef ], [ ilPropDef ], []

    else
        [], [], []

and GenToStringMethod cenv eenv ilThisTy m =
    GenPrintingMethod cenv eenv "ToString" ilThisTy m

/// Generate a ToString/get_Message method that calls 'sprintf "%A"'
and GenPrintingMethod cenv eenv methName ilThisTy m =
    let g = cenv.g

    [
        if not g.useReflectionFreeCodeGen then
            match (eenv.valsInScope.TryFind g.sprintf_vref.Deref, eenv.valsInScope.TryFind g.new_format_vref.Deref) with
            | Some (Lazy (Method (_, _, sprintfMethSpec, _, _, _, _, _, _, _, _, _))),
              Some (Lazy (Method (_, _, newFormatMethSpec, _, _, _, _, _, _, _, _, _))) ->
                // The type returned by the 'sprintf' call
                let funcTy = EraseClosures.mkILFuncTy cenv.ilxPubCloEnv ilThisTy g.ilg.typ_String

                // Give the instantiation of the printf format object, i.e. a Format`5 object compatible with StringFormat<ilThisTy>
                let newFormatMethSpec =
                    mkILMethSpec (
                        newFormatMethSpec.MethodRef,
                        AsObject,
                        [ // 'T -> string'
                            funcTy
                            // rest follow from 'StringFormat<T>'
                            GenUnitTy cenv eenv m
                            g.ilg.typ_String
                            g.ilg.typ_String
                            ilThisTy
                        ],
                        []
                    )

                // Instantiate with our own type
                let sprintfMethSpec =
                    mkILMethSpec (sprintfMethSpec.MethodRef, AsObject, [], [ funcTy ])

                // Here's the body of the method. Call printf, then invoke the function it returns
                let callInstrs =
                    EraseClosures.mkCallFunc
                        cenv.ilxPubCloEnv
                        (fun _ -> 0us)
                        eenv.tyenv.Count
                        Normalcall
                        (Apps_app(ilThisTy, Apps_done g.ilg.typ_String))

                let ilInstrs =
                    [ // load the hardwired format string
                        I_ldstr "%+A"
                        // make the printf format object
                        mkNormalNewobj newFormatMethSpec
                        // call sprintf
                        mkNormalCall sprintfMethSpec
                        // call the function returned by sprintf
                        mkLdarg0
                        if ilThisTy.Boxity = ILBoxity.AsValue then
                            mkNormalLdobj ilThisTy
                        yield! callInstrs
                    ]

                let ilMethodBody =
                    mkMethodBody (true, [], 2, nonBranchingInstrsToCode ilInstrs, None, eenv.imports)

                let mdef =
                    mkILNonGenericVirtualMethod (methName, ILMemberAccess.Public, [], mkILReturn g.ilg.typ_String, ilMethodBody)

                let mdef = mdef.With(customAttrs = mkILCustomAttrs [ g.CompilerGeneratedAttribute ])
                yield mdef
            | _ -> ()
    ]

and GenTypeDef cenv mgbuf lazyInitInfo eenv m (tycon: Tycon) =
    let g = cenv.g
    let tcref = mkLocalTyconRef tycon

    if tycon.IsTypeAbbrev then
        ()
    else
        match tycon.TypeReprInfo with
#if !NO_TYPEPROVIDERS
        | TProvidedNamespaceRepr _
        | TProvidedTypeRepr _
#endif
        | TNoRepr
        | TAsmRepr _
        | TILObjectRepr _
        | TMeasureableRepr _ -> ()
        | TFSharpObjectRepr _
        | TFSharpRecdRepr _
        | TFSharpUnionRepr _ ->
            let eenvinner = EnvForTycon tycon eenv
            let thisTy = generalizedTyconRef g tcref

            let ilThisTy = GenType cenv m eenvinner.tyenv thisTy
            let tref = ilThisTy.TypeRef
            let ilGenParams = GenGenericParams cenv eenvinner tycon.TyparsNoRange

            let ilIntfTys =
                tycon.ImmediateInterfaceTypesOfFSharpTycon
                |> List.map (GenType cenv m eenvinner.tyenv)

            let ilTypeName = tref.Name

            let hidden = IsHiddenTycon eenv.sigToImplRemapInfo tycon
            let hiddenRepr = hidden || IsHiddenTyconRepr eenv.sigToImplRemapInfo tycon
            let access = ComputeTypeAccess tref hidden

            // The implicit augmentation doesn't actually create CompareTo(object) or Object.Equals
            // So we do it here.
            //
            // Note you only have to implement 'System.IComparable' to customize structural comparison AND equality on F# types
            // See also FinalTypeDefinitionChecksAtEndOfInferenceScope in tc.fs
            //
            // Generate an Equals method implemented via IComparable if the type EXPLICITLY implements IComparable.
            // HOWEVER, if the type doesn't override Object.Equals already.
            let augmentOverrideMethodDefs =

                (if
                     Option.isNone tycon.GeneratedCompareToValues
                     && Option.isNone tycon.GeneratedHashAndEqualsValues
                     && tycon.HasInterface g g.mk_IComparable_ty
                     && not (tycon.HasOverride g "Equals" [ g.obj_ty ])
                     && not tycon.IsFSharpInterfaceTycon
                 then
                     [ GenEqualsOverrideCallingIComparable cenv (tcref, ilThisTy, ilThisTy) ]
                 else
                     [])

            // Generate the interface slots and abstract slots.
            let abstractMethodDefs, abstractPropDefs, abstractEventDefs =
                if tycon.IsFSharpDelegateTycon then
                    [], [], []
                else
                    // sort by order of declaration
                    // REVIEW: this should be based off tcaug_adhoc_list, which is in declaration order
                    tycon.MembersOfFSharpTyconSorted
                    |> List.sortWith (fun v1 v2 -> rangeOrder.Compare(v1.DefinitionRange, v2.DefinitionRange))
                    |> List.map (GenAbstractBinding cenv eenv tref)
                    |> List.unzip3
                    |> mapTriple (List.concat, List.concat, List.concat)

            let abstractPropDefs = abstractPropDefs |> MergePropertyDefs m
            let isAbstract = isAbstractTycon tycon

            // Generate all the method impls showing how various abstract slots and interface slots get implemented
            // REVIEW: no method impl generated for IStructuralHash or ICompare
            let methodImpls =
                [
                    for vref in tycon.MembersOfFSharpTyconByName |> NameMultiMap.range do
                        assert vref.IsMember
                        let memberInfo = vref.MemberInfo.Value

                        if
                            memberInfo.MemberFlags.IsOverrideOrExplicitImpl
                            && not (CompileAsEvent g vref.Attribs)
                        then

                            for slotsig in memberInfo.ImplementedSlotSigs do

                                if isInterfaceTy g slotsig.DeclaringType then

                                    match vref.ValReprInfo with
                                    | Some _ ->

                                        let memberParentTypars, memberMethodTypars =
                                            match PartitionValRefTypars g vref with
                                            | Some (_, memberParentTypars, memberMethodTypars, _, _) ->
                                                memberParentTypars, memberMethodTypars
                                            | None -> [], []

                                        let useMethodImpl = true
                                        let eenvUnderTypars = EnvForTypars memberParentTypars eenv

                                        let _, methodImplGenerator =
                                            GenMethodImpl cenv eenvUnderTypars (useMethodImpl, slotsig) m

                                        if useMethodImpl then
                                            yield methodImplGenerator (ilThisTy, memberMethodTypars)

                                    | _ -> ()
                ]

            // Try to add a DefaultMemberAttribute for the 'Item' property
            let defaultMemberAttrs =
                // REVIEW: this should be based off tcaug_adhoc_list, which is in declaration order
                tycon.MembersOfFSharpTyconSorted
                |> List.tryPick (fun vref ->
                    let name = vref.DisplayName

                    match vref.MemberInfo with
                    | None -> None
                    | Some memberInfo ->
                        match name, memberInfo.MemberFlags.MemberKind with
                        | ("Item"
                          | "op_IndexedLookup"),
                          (SynMemberKind.PropertyGet
                          | SynMemberKind.PropertySet) when not (isNil (ArgInfosOfPropertyVal g vref.Deref)) ->
                            Some(
                                mkILCustomAttribute (
                                    g.FindSysILTypeRef "System.Reflection.DefaultMemberAttribute",
                                    [ g.ilg.typ_String ],
                                    [ ILAttribElem.String(Some name) ],
                                    []
                                )
                            )
                        | _ -> None)
                |> Option.toList

            let tyconRepr = tycon.TypeReprInfo

            let reprAccess = ComputeMemberAccess hiddenRepr

            // DebugDisplayAttribute gets copied to the subtypes generated as part of DU compilation
            let debugDisplayAttrs, normalAttrs =
                tycon.Attribs
                |> List.partition (IsMatchingFSharpAttribute g g.attrib_DebuggerDisplayAttribute)

            let securityAttrs, normalAttrs =
                normalAttrs
                |> List.partition (fun a -> IsSecurityAttribute g cenv.amap cenv.casApplied a m)

            let generateDebugDisplayAttribute =
                not g.useReflectionFreeCodeGen
                && not g.compilingFSharpCore
                && tycon.IsUnionTycon
                && isNil debugDisplayAttrs

            let generateDebugProxies =
                not (tyconRefEq g tcref g.unit_tcr_canon)
                && not (HasFSharpAttribute g g.attrib_DebuggerTypeProxyAttribute tycon.Attribs)

            let permissionSets = CreatePermissionSets cenv eenv securityAttrs

            let secDecls =
                if List.isEmpty securityAttrs then
                    emptyILSecurityDecls
                else
                    mkILSecurityDecls permissionSets

            let ilDebugDisplayAttributes =
                [
                    yield! GenAttrs cenv eenv debugDisplayAttrs
                    if generateDebugDisplayAttribute then
                        yield g.mkDebuggerDisplayAttribute ("{" + debugDisplayMethodName + "(),nq}")
                ]

            let ilCustomAttrs =
                [
                    yield! defaultMemberAttrs
                    yield!
                        normalAttrs
                        |> List.filter (IsMatchingFSharpAttribute g g.attrib_StructLayoutAttribute >> not)
                        |> GenAttrs cenv eenv
                    yield! ilDebugDisplayAttributes
                ]

            let ilTypeDefKind =
                match tyconRepr with
                | TFSharpObjectRepr o ->
                    match o.fsobjmodel_kind with
                    | TFSharpClass -> ILTypeDefKind.Class
                    | TFSharpStruct -> ILTypeDefKind.ValueType
                    | TFSharpInterface -> ILTypeDefKind.Interface
                    | TFSharpEnum -> ILTypeDefKind.Enum
                    | TFSharpDelegate _ -> ILTypeDefKind.Delegate
                | TFSharpRecdRepr _
                | TFSharpUnionRepr _ when tycon.IsStructOrEnumTycon -> ILTypeDefKind.ValueType
                | _ -> ILTypeDefKind.Class

            let requiresExtraField =
                let isEmptyStruct =
                    (match ilTypeDefKind with
                     | ILTypeDefKind.ValueType -> true
                     | _ -> false)
                    &&
                    // All structs are sequential by default
                    // Structs with no instance fields get size 1, pack 0
                    tycon.AllFieldsArray |> Array.forall (fun f -> f.IsStatic)

                isEmptyStruct
                && cenv.options.workAroundReflectionEmitBugs
                && not tycon.TyparsNoRange.IsEmpty

            // Compute a bunch of useful things for each field
            let isCLIMutable =
                (TryFindFSharpBoolAttribute g g.attrib_CLIMutableAttribute tycon.Attribs = Some true)

            let fieldSummaries =

                [
                    for fspec in tycon.AllFieldsArray do

                        let useGenuineField = useGenuineField tycon fspec

                        // The property (or genuine IL field) is hidden in these circumstances:
                        //     - secret fields apart from "__value" fields for enums
                        //     - the representation of the type is hidden
                        //     - the F# field is hidden by a signature or private declaration
                        let isPropHidden =
                            // Enums always have public cases irrespective of Enum Visibility
                            if tycon.IsEnumTycon then
                                false
                            else
                                (fspec.IsCompilerGenerated
                                 || hiddenRepr
                                 || IsHiddenRecdField eenv.sigToImplRemapInfo (tcref.MakeNestedRecdFieldRef fspec))

                        let ilType = GenType cenv m eenvinner.tyenv fspec.FormalType
                        let ilFieldName = ComputeFieldName tycon fspec

                        yield
                            (useGenuineField,
                             ilFieldName,
                             fspec.IsMutable,
                             fspec.IsStatic,
                             fspec.PropertyAttribs,
                             ilType,
                             isPropHidden,
                             fspec)
                ]

            // Generate the IL fields
            let ilFieldDefs =
                [
                    for useGenuineField, ilFieldName, isFSharpMutable, isStatic, _, ilPropType, isPropHidden, fspec in fieldSummaries do

                        let ilFieldOffset =
                            match TryFindFSharpAttribute g g.attrib_FieldOffsetAttribute fspec.FieldAttribs with
                            | Some (Attrib (_, _, [ AttribInt32Arg fieldOffset ], _, _, _, _)) -> Some fieldOffset
                            | Some attrib ->
                                errorR (Error(FSComp.SR.ilFieldOffsetAttributeCouldNotBeDecoded (), attrib.Range))
                                None
                            | _ -> None

                        let attribs =
                            [ // If using a field then all the attributes go on the field
                                // See also FSharp 1.0 Bug 4727: once we start compiling them as real mutable fields, you should not be able to target both "property" for "val mutable" fields in classes

                                if useGenuineField then
                                    yield! fspec.PropertyAttribs
                                yield! fspec.FieldAttribs
                            ]

                        let ilNotSerialized =
                            HasFSharpAttributeOpt g g.attrib_NonSerializedAttribute attribs

                        let fattribs =
                            attribs
                            // Do not generate FieldOffset as a true CLI custom attribute, since it is implied by other corresponding CLI metadata
                            |> List.filter (IsMatchingFSharpAttribute g g.attrib_FieldOffsetAttribute >> not)
                            // Do not generate NonSerialized as a true CLI custom attribute, since it is implied by other corresponding CLI metadata
                            |> List.filter (IsMatchingFSharpAttributeOpt g g.attrib_NonSerializedAttribute >> not)

                        let ilFieldMarshal, fattribs = GenMarshal cenv fattribs

                        // The IL field is hidden if the property/field is hidden OR we're using a property
                        // AND the field is not mutable (because we can take the address of a mutable field).
                        // Otherwise fields are always accessed via their property getters/setters
                        //
                        // Additionally, don't hide fields for multiemit in F# Interactive
                        let isFieldHidden =
                            isPropHidden
                            || (not useGenuineField
                                && not isFSharpMutable
                                && not (cenv.options.isInteractive && cenv.options.fsiMultiAssemblyEmit))

                        let extraAttribs =
                            match tyconRepr with
                            | TFSharpRecdRepr _ when not useGenuineField ->
                                [ g.CompilerGeneratedAttribute; g.DebuggerBrowsableNeverAttribute ]
                            | _ -> [] // don't hide fields in classes in debug display

                        let access = ComputeMemberAccess isFieldHidden

                        let literalValue = Option.map (GenFieldInit m) fspec.LiteralValue

                        let fdef =
                            ILFieldDef(
                                name = ilFieldName,
                                fieldType = ilPropType,
                                attributes = enum 0,
                                data = None,
                                literalValue = None,
                                offset = ilFieldOffset,
                                marshal = None,
                                customAttrs = mkILCustomAttrs (GenAttrs cenv eenv fattribs @ extraAttribs)
                            )
                                .WithAccess(access)
                                .WithStatic(isStatic)
                                .WithSpecialName(ilFieldName = "value__" && tycon.IsEnumTycon)
                                .WithNotSerialized(ilNotSerialized)
                                .WithLiteralDefaultValue(literalValue)
                                .WithFieldMarshal(ilFieldMarshal)

                        yield fdef

                    if requiresExtraField then
                        yield mkILInstanceField ("__dummy", g.ilg.typ_Int32, None, ILMemberAccess.Assembly)
                ]

            // Generate property definitions for the fields compiled as properties
            let ilPropertyDefsForFields =
                [
                    for i, (useGenuineField, _, isFSharpMutable, isStatic, propAttribs, ilPropType, _, fspec) in Seq.indexed fieldSummaries do
                        if not useGenuineField then
                            let ilCallingConv =
                                if isStatic then
                                    ILCallingConv.Static
                                else
                                    ILCallingConv.Instance

                            let ilPropName = fspec.LogicalName
                            let ilHasSetter = isCLIMutable || isFSharpMutable

                            let ilFieldAttrs =
                                GenAttrs cenv eenv propAttribs
                                @ [ mkCompilationMappingAttrWithSeqNum g (int SourceConstructFlags.Field) i ]

                            yield
                                ILPropertyDef(
                                    name = ilPropName,
                                    attributes = PropertyAttributes.None,
                                    setMethod =
                                        (if ilHasSetter then
                                             Some(mkILMethRef (tref, ilCallingConv, "set_" + ilPropName, 0, [ ilPropType ], ILType.Void))
                                         else
                                             None),
                                    getMethod = Some(mkILMethRef (tref, ilCallingConv, "get_" + ilPropName, 0, [], ilPropType)),
                                    callingConv = ilCallingConv.ThisConv,
                                    propertyType = ilPropType,
                                    init = None,
                                    args = [],
                                    customAttrs = mkILCustomAttrs ilFieldAttrs
                                )
                ]

            let methodDefs =
                [ // Generate property getter methods for those fields that have properties
                    for useGenuineField, ilFieldName, _, isStatic, _, ilPropType, isPropHidden, fspec in fieldSummaries do
                        if not useGenuineField then
                            let ilPropName = fspec.LogicalName
                            let ilMethName = "get_" + ilPropName
                            let access = ComputeMemberAccess isPropHidden
                            let isStruct = isStructTyconRef tcref

                            let attrs =
                                if isStruct && not isStatic then
                                    [ GenReadOnlyAttribute g ]
                                else
                                    []

                            yield
                                mkLdfldMethodDef (ilMethName, access, isStatic, ilThisTy, ilFieldName, ilPropType, attrs)
                                |> g.AddMethodGeneratedAttributes

                    // Generate property setter methods for the mutable fields
                    for useGenuineField, ilFieldName, isFSharpMutable, isStatic, _, ilPropType, isPropHidden, fspec in fieldSummaries do
                        let ilHasSetter = (isCLIMutable || isFSharpMutable) && not useGenuineField

                        if ilHasSetter then
                            let ilPropName = fspec.LogicalName
                            let ilFieldSpec = mkILFieldSpecInTy (ilThisTy, ilFieldName, ilPropType)
                            let ilMethName = "set_" + ilPropName
                            let ilParams = [ mkILParamNamed ("value", ilPropType) ]
                            let ilReturn = mkILReturn ILType.Void
                            let iLAccess = ComputeMemberAccess isPropHidden

                            let ilMethodDef =
                                if isStatic then
                                    let ilMethodBody =
                                        mkMethodBody (
                                            true,
                                            [],
                                            2,
                                            nonBranchingInstrsToCode [ mkLdarg0; mkNormalStsfld ilFieldSpec ],
                                            None,
                                            eenv.imports
                                        )

                                    mkILNonGenericStaticMethod (ilMethName, iLAccess, ilParams, ilReturn, ilMethodBody)
                                else
                                    let ilMethodBody =
                                        mkMethodBody (
                                            true,
                                            [],
                                            2,
                                            nonBranchingInstrsToCode [ mkLdarg0; mkLdarg 1us; mkNormalStfld ilFieldSpec ],
                                            None,
                                            eenv.imports
                                        )

                                    mkILNonGenericInstanceMethod (ilMethName, iLAccess, ilParams, ilReturn, ilMethodBody)
                                    |> g.AddMethodGeneratedAttributes

                            yield ilMethodDef.WithSpecialName

                    if generateDebugDisplayAttribute then
                        let (|Lazy|) (x: Lazy<_>) = x.Force()

                        match (eenv.valsInScope.TryFind g.sprintf_vref.Deref, eenv.valsInScope.TryFind g.new_format_vref.Deref) with
                        | Some (Lazy (Method (_, _, sprintfMethSpec, _, _, _, _, _, _, _, _, _))),
                          Some (Lazy (Method (_, _, newFormatMethSpec, _, _, _, _, _, _, _, _, _))) ->
                            // The type returned by the 'sprintf' call
                            let funcTy = EraseClosures.mkILFuncTy cenv.ilxPubCloEnv ilThisTy g.ilg.typ_String
                            // Give the instantiation of the printf format object, i.e. a Format`5 object compatible with StringFormat<ilThisTy>
                            let newFormatMethSpec =
                                mkILMethSpec (
                                    newFormatMethSpec.MethodRef,
                                    AsObject,
                                    [ // 'T -> string'
                                        funcTy
                                        // rest follow from 'StringFormat<T>'
                                        GenUnitTy cenv eenv m
                                        g.ilg.typ_String
                                        g.ilg.typ_String
                                        g.ilg.typ_String
                                    ],
                                    []
                                )
                            // Instantiate with our own type
                            let sprintfMethSpec =
                                mkILMethSpec (sprintfMethSpec.MethodRef, AsObject, [], [ funcTy ])

                            // Here's the body of the method. Call printf, then invoke the function it returns
                            let callInstrs =
                                EraseClosures.mkCallFunc
                                    cenv.ilxPubCloEnv
                                    (fun _ -> 0us)
                                    eenv.tyenv.Count
                                    Normalcall
                                    (Apps_app(ilThisTy, Apps_done g.ilg.typ_String))

                            let ilInstrs =
                                [ // load the hardwired format string
                                    I_ldstr "%+0.8A"
                                    // make the printf format object
                                    mkNormalNewobj newFormatMethSpec
                                    // call sprintf
                                    mkNormalCall sprintfMethSpec
                                    // call the function returned by sprintf
                                    mkLdarg0
                                    if ilThisTy.Boxity = ILBoxity.AsValue then
                                        mkNormalLdobj ilThisTy
                                    yield! callInstrs
                                ]

                            let ilMethodBody =
                                mkMethodBody (true, [], 2, nonBranchingInstrsToCode ilInstrs, None, eenv.imports)

                            let ilMethodDef =
                                mkILNonGenericInstanceMethod (
                                    debugDisplayMethodName,
                                    ILMemberAccess.Assembly,
                                    [],
                                    mkILReturn g.ilg.typ_Object,
                                    ilMethodBody
                                )

                            yield ilMethodDef.WithSpecialName |> AddNonUserCompilerGeneratedAttribs g
                        | None, _ ->
                            //printfn "sprintf not found"
                            ()
                        | _, None ->
                            //printfn "new format not found"
                            ()
                        | _ ->
                            //printfn "neither found, or non-method"
                            ()

                    // Build record constructors and the funky methods that go with records and delegate types.
                    // Constructors and delegate methods have the same access as the representation
                    match tyconRepr with
                    | TFSharpRecdRepr _ when not tycon.IsEnumTycon ->
                        // No constructor for enum types
                        // Otherwise find all the non-static, non zero-init fields and build a constructor
                        let relevantFields =
                            fieldSummaries
                            |> List.filter (fun (_, _, _, isStatic, _, _, _, fspec) -> not isStatic && not fspec.IsZeroInit)

                        let fieldNamesAndTypes =
                            relevantFields
                            |> List.map (fun (_, ilFieldName, _, _, _, ilPropType, _, fspec) -> (fspec.LogicalName, ilFieldName, ilPropType))

                        let isStructRecord = tycon.IsStructRecordOrUnionTycon

                        // No type spec if the record is a value type
                        let spec =
                            if isStructRecord then
                                None
                            else
                                Some(g.ilg.typ_Object.TypeSpec)

                        let ilMethodDef =
                            mkILSimpleStorageCtorWithParamNames (
                                spec,
                                ilThisTy,
                                [],
                                ChooseParamNames fieldNamesAndTypes,
                                reprAccess,
                                None,
                                eenv.imports
                            )

                        yield ilMethodDef
                        // FSharp 1.0 bug 1988: Explicitly setting the ComVisible(true) attribute on an F# type causes an F# record to be emitted in a way that enables mutation for COM interop scenarios
                        // FSharp 3.0 feature: adding CLIMutable to a record type causes emit of default constructor, and all fields get property setters
                        // Records that are value types do not create a default constructor with CLIMutable or ComVisible
                        if
                            not isStructRecord
                            && (isCLIMutable
                                || (TryFindFSharpBoolAttribute g g.attrib_ComVisibleAttribute tycon.Attribs = Some true))
                        then
                            yield mkILSimpleStorageCtor (Some g.ilg.typ_Object.TypeSpec, ilThisTy, [], [], reprAccess, None, eenv.imports)

                        if not (tycon.HasMember g "ToString" []) then
                            yield! GenToStringMethod cenv eenv ilThisTy m

                    | TFSharpObjectRepr r when tycon.IsFSharpDelegateTycon ->

                        // Build all the methods that go with a delegate type
                        match r.fsobjmodel_kind with
                        | TFSharpDelegate slotSig ->

                            let parameters, ret =
                                // When "type delegateTy = delegate of unit -> returnTy",
                                // suppress the unit arg from delegate .Invoke vslot.
                                let (TSlotSig (nm, ty, ctps, mtps, paraml, returnTy)) = slotSig

                                let paraml =
                                    match paraml with
                                    | [ [ tsp ] ] when isUnitTy g tsp.Type -> [] (* suppress unit arg *)
                                    | paraml -> paraml

                                GenActualSlotsig m cenv eenvinner (TSlotSig(nm, ty, ctps, mtps, paraml, returnTy)) [] []

                            yield! mkILDelegateMethods reprAccess g.ilg (g.iltyp_AsyncCallback, g.iltyp_IAsyncResult) (parameters, ret)
                        | _ -> ()

                    | TFSharpUnionRepr _ when not (tycon.HasMember g "ToString" []) -> yield! GenToStringMethod cenv eenv ilThisTy m
                    | _ -> ()
                ]

            let ilMethods = methodDefs @ augmentOverrideMethodDefs @ abstractMethodDefs
            let ilProperties = mkILProperties (ilPropertyDefsForFields @ abstractPropDefs)
            let ilEvents = mkILEvents abstractEventDefs
            let ilFields = mkILFields ilFieldDefs

            let tdef, tdefDiscards =
                let isSerializable =
                    (TryFindFSharpBoolAttribute g g.attrib_AutoSerializableAttribute tycon.Attribs
                     <> Some false)

                match tycon.TypeReprInfo with
                | TILObjectRepr _ ->
                    let tdef = tycon.ILTyconRawMetadata.WithAccess access

                    let tdef =
                        tdef.With(customAttrs = mkILCustomAttrs ilCustomAttrs, genericParams = ilGenParams)

                    tdef, None

                | TFSharpRecdRepr _
                | TFSharpObjectRepr _ as tyconRepr ->
                    let super = superOfTycon g tycon
                    let ilBaseTy = GenType cenv m eenvinner.tyenv super

                    // Build a basic type definition
                    let isObjectType =
                        (match tyconRepr with
                         | TFSharpObjectRepr _ -> true
                         | _ -> false)

                    let ilAttrs =
                        ilCustomAttrs
                        @ [
                            mkCompilationMappingAttr
                                g
                                (int (
                                    if isObjectType then
                                        SourceConstructFlags.ObjectType
                                    elif hiddenRepr then
                                        SourceConstructFlags.RecordType ||| SourceConstructFlags.NonPublicRepresentation
                                    else
                                        SourceConstructFlags.RecordType
                                ))
                        ]

                    // For now, generic types always use ILTypeInit.BeforeField. This is because
                    // there appear to be some cases where ILTypeInit.OnAny causes problems for
                    // the .NET CLR when used in conjunction with generic classes in cross-DLL
                    // and NGEN scenarios.
                    //
                    // We don't apply this rule to the final file. This is because ALL classes with .cctors in
                    // the final file (which may in turn trigger the .cctor for the .EXE itself, which
                    // in turn calls the main() method) must have deterministic initialization
                    // that is not triggered prior to execution of the main() method.
                    // If this property doesn't hold then the .cctor can end up running
                    // before the main method even starts.
                    let typeDefTrigger =
                        if eenv.isFinalFile || tycon.TyparsNoRange.IsEmpty then
                            ILTypeInit.OnAny
                        else
                            ILTypeInit.BeforeField

                    let isKnownToBeAttribute =
                        ExistsSameHeadTypeInHierarchy g cenv.amap m super g.mk_Attribute_ty

                    let tdef =
                        mkILGenericClass (
                            ilTypeName,
                            access,
                            ilGenParams,
                            ilBaseTy,
                            ilIntfTys,
                            mkILMethods ilMethods,
                            ilFields,
                            emptyILTypeDefs,
                            ilProperties,
                            ilEvents,
                            mkILCustomAttrs ilAttrs,
                            typeDefTrigger
                        )

                    // Set some the extra entries in the definition
                    let isTheSealedAttribute = tyconRefEq g tcref g.attrib_SealedAttribute.TyconRef

                    let tdef =
                        tdef
                            .WithSealed(isSealedTy g thisTy || isTheSealedAttribute)
                            .WithSerializable(isSerializable)
                            .WithAbstract(isAbstract)
                            .WithImport(isComInteropTy g thisTy)
                            .With(methodImpls = mkILMethodImpls methodImpls, isKnownToBeAttribute = isKnownToBeAttribute)

                    let tdLayout, tdEncoding =
                        match TryFindFSharpAttribute g g.attrib_StructLayoutAttribute tycon.Attribs with
                        | Some (Attrib (_, _, [ AttribInt32Arg layoutKind ], namedArgs, _, _, _)) ->
                            let decoder = AttributeDecoder namedArgs
                            let ilPack = decoder.FindInt32 "Pack" 0x0
                            let ilSize = decoder.FindInt32 "Size" 0x0

                            let tdEncoding =
                                match (decoder.FindInt32 "CharSet" 0x0) with
                                (* enumeration values for System.Runtime.InteropServices.CharSet taken from mscorlib.il *)
                                | 0x03 -> ILDefaultPInvokeEncoding.Unicode
                                | 0x04 -> ILDefaultPInvokeEncoding.Auto
                                | _ -> ILDefaultPInvokeEncoding.Ansi

                            let layoutInfo =
                                if ilPack = 0x0 && ilSize = 0x0 then
                                    { Size = None; Pack = None }
                                else
                                    {
                                        Size = Some ilSize
                                        Pack = Some(uint16 ilPack)
                                    }

                            let tdLayout =
                                match layoutKind with
                                (* enumeration values for System.Runtime.InteropServices.LayoutKind taken from mscorlib.il *)
                                | 0x0 -> ILTypeDefLayout.Sequential layoutInfo
                                | 0x2 -> ILTypeDefLayout.Explicit layoutInfo
                                | _ -> ILTypeDefLayout.Auto

                            tdLayout, tdEncoding
                        | Some (Attrib (_, _, _, _, _, _, m)) ->
                            errorR (Error(FSComp.SR.ilStructLayoutAttributeCouldNotBeDecoded (), m))
                            ILTypeDefLayout.Auto, ILDefaultPInvokeEncoding.Ansi

                        | _ when
                            (match ilTypeDefKind with
                             | ILTypeDefKind.ValueType -> true
                             | _ -> false)
                            ->

                            // All structs are sequential by default
                            // Structs with no instance fields get size 1, pack 0
                            if
                                tycon.AllFieldsArray |> Array.exists (fun f -> not f.IsStatic)
                                ||
                                // Reflection emit doesn't let us emit 'pack' and 'size' for generic structs.
                                // In that case we generate a dummy field instead
                                (cenv.options.workAroundReflectionEmitBugs && not tycon.TyparsNoRange.IsEmpty)
                            then
                                ILTypeDefLayout.Sequential { Size = None; Pack = None }, ILDefaultPInvokeEncoding.Ansi
                            else
                                ILTypeDefLayout.Sequential { Size = Some 1; Pack = Some 0us }, ILDefaultPInvokeEncoding.Ansi

                        | _ -> ILTypeDefLayout.Auto, ILDefaultPInvokeEncoding.Ansi

                    // if the type's layout is Explicit, ensure that each field has a valid offset
                    let validateExplicit (fdef: ILFieldDef) =
                        match fdef.Offset with
                        // Remove field suffix "@" for pretty printing
                        | None ->
                            errorR (
                                Error(
                                    FSComp.SR.ilFieldDoesNotHaveValidOffsetForStructureLayout (tdef.Name, fdef.Name.Replace("@", "")),
                                    (trimRangeToLine m)
                                )
                            )
                        | _ -> ()

                    // if the type's layout is Sequential, no offsets should be applied
                    let validateSequential (fdef: ILFieldDef) =
                        match fdef.Offset with
                        | Some _ -> errorR (Error(FSComp.SR.ilFieldHasOffsetForSequentialLayout (), (trimRangeToLine m)))
                        | _ -> ()

                    match tdLayout with
                    | ILTypeDefLayout.Explicit _ -> List.iter validateExplicit ilFieldDefs
                    | ILTypeDefLayout.Sequential _ -> List.iter validateSequential ilFieldDefs
                    | _ -> ()

                    let tdef =
                        tdef.WithKind(ilTypeDefKind).WithLayout(tdLayout).WithEncoding(tdEncoding)

                    tdef, None

                | TFSharpUnionRepr _ ->
                    let alternatives =
                        tycon.UnionCasesArray
                        |> Array.mapi (fun i ucspec ->
                            {
                                altName = ucspec.CompiledName
                                altFields = GenUnionCaseRef cenv m eenvinner.tyenv i ucspec.RecdFieldsArray
                                altCustomAttrs =
                                    mkILCustomAttrs (
                                        GenAttrs cenv eenv ucspec.Attribs
                                        @ [ mkCompilationMappingAttrWithSeqNum g (int SourceConstructFlags.UnionCase) i ]
                                    )
                            })

                    let cuinfo =
                        {
                            UnionCasesAccessibility = reprAccess
                            IsNullPermitted = IsUnionTypeWithNullAsTrueValue g tycon
                            HelpersAccessibility = reprAccess
                            HasHelpers = ComputeUnionHasHelpers g tcref
                            GenerateDebugProxies = generateDebugProxies
                            DebugDisplayAttributes = ilDebugDisplayAttributes
                            UnionCases = alternatives
                            DebugPoint = None
                            DebugImports = eenv.imports
                        }

                    let layout =
                        if isStructTy g thisTy then
                            if
                                (match ilTypeDefKind with
                                 | ILTypeDefKind.ValueType -> true
                                 | _ -> false)
                            then
                                // Structs with no instance fields get size 1, pack 0
                                ILTypeDefLayout.Sequential { Size = Some 1; Pack = Some 0us }
                            else
                                ILTypeDefLayout.Sequential { Size = None; Pack = None }
                        else
                            ILTypeDefLayout.Auto

                    let cattrs =
                        mkILCustomAttrs (
                            ilCustomAttrs
                            @ [
                                mkCompilationMappingAttr
                                    g
                                    (int (
                                        if hiddenRepr then
                                            SourceConstructFlags.SumType ||| SourceConstructFlags.NonPublicRepresentation
                                        else
                                            SourceConstructFlags.SumType
                                    ))
                            ]
                        )

                    let tdef =
                        ILTypeDef(
                            name = ilTypeName,
                            layout = layout,
                            attributes = enum 0,
                            genericParams = ilGenParams,
                            customAttrs = cattrs,
                            fields = ilFields,
                            events = ilEvents,
                            properties = ilProperties,
                            methods = mkILMethods ilMethods,
                            methodImpls = mkILMethodImpls methodImpls,
                            nestedTypes = emptyILTypeDefs,
                            implements = ilIntfTys,
                            extends =
                                Some(
                                    if tycon.IsStructOrEnumTycon then
                                        g.iltyp_ValueType
                                    else
                                        g.ilg.typ_Object
                                ),
                            isKnownToBeAttribute = false,
                            securityDecls = emptyILSecurityDecls
                        )
                            .WithLayout(layout)
                            .WithSerializable(isSerializable)
                            .WithSealed(true)
                            .WithEncoding(ILDefaultPInvokeEncoding.Auto)
                            .WithAccess(access)
                            .WithInitSemantics(ILTypeInit.BeforeField)

                    let tdef2 =
                        EraseUnions.mkClassUnionDef
                            (g.AddMethodGeneratedAttributes,
                             g.AddPropertyGeneratedAttributes,
                             g.AddPropertyNeverAttributes,
                             g.AddFieldGeneratedAttributes,
                             g.AddFieldNeverAttributes,
                             g.MkDebuggerTypeProxyAttribute)
                            g.ilg
                            tref
                            tdef
                            cuinfo

                    // Discard the user-supplied (i.e. prim-type.fs) implementations of the get_Empty, get_IsEmpty, get_Value and get_None and Some methods.
                    // This is because we will replace their implementations by ones that load the unique
                    // private static field for lists etc.
                    //
                    // Also discard the F#-compiler supplied implementation of the Empty, IsEmpty, Value and None properties.
                    let tdefDiscards =
                        Some(
                            (fun (md: ILMethodDef) ->
                                (cuinfo.HasHelpers = SpecialFSharpListHelpers
                                 && (md.Name = "get_Empty" || md.Name = "Cons" || md.Name = "get_IsEmpty"))
                                || (cuinfo.HasHelpers = SpecialFSharpOptionHelpers
                                    && (md.Name = "get_Value" || md.Name = "get_None" || md.Name = "Some"))),

                            (fun (pd: ILPropertyDef) ->
                                (cuinfo.HasHelpers = SpecialFSharpListHelpers
                                 && (pd.Name = "Empty" || pd.Name = "IsEmpty"))
                                || (cuinfo.HasHelpers = SpecialFSharpOptionHelpers
                                    && (pd.Name = "Value" || pd.Name = "None")))
                        )

                    tdef2, tdefDiscards

                | _ -> failwith "??"

            let tdef = tdef.WithHasSecurity(not (List.isEmpty securityAttrs))
            let tdef = tdef.With(securityDecls = secDecls)
            mgbuf.AddTypeDef(tref, tdef, false, false, tdefDiscards)

            // If a non-generic type is written with "static let" and "static do" (i.e. it has a ".cctor")
            // then the code for the .cctor is placed into .cctor for the backing static class for the file.
            // It is not placed in its own .cctor as there is no feasible way for this to be given a coherent
            // order in the sequential initialization of the file.
            //
            // In this case, the .cctor for this type must force the .cctor of the backing static class for the file.
            if
                tycon.TyparsNoRange.IsEmpty
                && tycon.MembersOfFSharpTyconSorted
                   |> List.exists (fun vref -> vref.Deref.IsClassConstructor)
            then
                GenForceWholeFileInitializationAsPartOfCCtor cenv mgbuf lazyInitInfo tref eenv.imports m

/// Generate the type for an F# exception declaration.
and GenExnDef cenv mgbuf eenv m (exnc: Tycon) =
    let g = cenv.g
    let exncref = mkLocalEntityRef exnc

    match exnc.ExceptionInfo with
    | TExnAbbrevRepr _
    | TExnAsmRepr _
    | TExnNone -> ()
    | TExnFresh _ ->
        let ilThisTy = GenExnType cenv m eenv.tyenv exncref
        let tref = ilThisTy.TypeRef
        let isHidden = IsHiddenTycon eenv.sigToImplRemapInfo exnc
        let access = ComputeTypeAccess tref isHidden
        let reprAccess = ComputeMemberAccess isHidden
        let fspecs = exnc.TrueInstanceFieldsAsList

        let ilMethodDefsForProperties, ilFieldDefs, ilPropertyDefs, fieldNamesAndTypes =
            [
                for i, fld in Seq.indexed fspecs do
                    let ilPropName = fld.LogicalName
                    let ilPropType = GenType cenv m eenv.tyenv fld.FormalType
                    let ilMethName = "get_" + fld.LogicalName
                    let ilFieldName = ComputeFieldName exnc fld

                    let ilMethodDef =
                        mkLdfldMethodDef (ilMethName, reprAccess, false, ilThisTy, ilFieldName, ilPropType, [])

                    let ilFieldDef =
                        mkILInstanceField (ilFieldName, ilPropType, None, ILMemberAccess.Assembly)

                    let ilPropDef =
                        ILPropertyDef(
                            name = ilPropName,
                            attributes = PropertyAttributes.None,
                            setMethod = None,
                            getMethod = Some(mkILMethRef (tref, ILCallingConv.Instance, ilMethName, 0, [], ilPropType)),
                            callingConv = ILThisConvention.Instance,
                            propertyType = ilPropType,
                            init = None,
                            args = [],
                            customAttrs =
                                mkILCustomAttrs (
                                    GenAttrs cenv eenv fld.PropertyAttribs
                                    @ [ mkCompilationMappingAttrWithSeqNum g (int SourceConstructFlags.Field) i ]
                                )
                        )

                    yield (ilMethodDef, ilFieldDef, ilPropDef, (ilPropName, ilFieldName, ilPropType))
            ]
            |> List.unzip4

        let ilCtorDef =
            mkILSimpleStorageCtorWithParamNames (
                Some g.iltyp_Exception.TypeSpec,
                ilThisTy,
                [],
                ChooseParamNames fieldNamesAndTypes,
                reprAccess,
                None,
                eenv.imports
            )

        // In compiled code, all exception types get a parameterless constructor for use with XML serialization
        // This does default-initialization of all fields
        let ilCtorDefNoArgs =
            if not (isNil fieldNamesAndTypes) then
                [
                    mkILSimpleStorageCtor (Some g.iltyp_Exception.TypeSpec, ilThisTy, [], [], reprAccess, None, eenv.imports)
                ]
            else
                []

        let serializationRelatedMembers =
            // do not emit serialization related members if target framework lacks SerializationInfo or StreamingContext
            match g.iltyp_SerializationInfo, g.iltyp_StreamingContext with
            | Some serializationInfoType, Some streamingContextType ->

                let ilInstrsForSerialization =
                    [
                        mkLdarg0
                        mkLdarg 1us
                        mkLdarg 2us
                        mkNormalCall (mkILCtorMethSpecForTy (g.iltyp_Exception, [ serializationInfoType; streamingContextType ]))
                    ]
                    |> nonBranchingInstrsToCode

                let ilCtorDefForSerialization =
                    mkILCtor (
                        ILMemberAccess.Family,
                        [
                            mkILParamNamed ("info", serializationInfoType)
                            mkILParamNamed ("context", streamingContextType)
                        ],
                        mkMethodBody (false, [], 8, ilInstrsForSerialization, None, eenv.imports)
                    )

                [ ilCtorDefForSerialization ]
            | _ -> []

        let ilTypeName = tref.Name

        let ilMethodDefs =
            [
                ilCtorDef
                yield! ilCtorDefNoArgs
                yield! serializationRelatedMembers
                yield! ilMethodDefsForProperties

                if
                    cenv.g.langVersion.SupportsFeature(LanguageFeature.BetterExceptionPrinting)
                    && not (exnc.HasMember g "get_Message" [])
                    && not (exnc.HasMember g "Message" [])
                then
                    yield! GenPrintingMethod cenv eenv "get_Message" ilThisTy m
            ]

        let interfaces =
            exnc.ImmediateInterfaceTypesOfFSharpTycon
            |> List.map (GenType cenv m eenv.tyenv)

        let tdef =
            mkILGenericClass (
                ilTypeName,
                access,
                [],
                g.iltyp_Exception,
                interfaces,
                mkILMethods ilMethodDefs,
                mkILFields ilFieldDefs,
                emptyILTypeDefs,
                mkILProperties ilPropertyDefs,
                emptyILEvents,
                mkILCustomAttrs [ mkCompilationMappingAttr g (int SourceConstructFlags.Exception) ],
                ILTypeInit.BeforeField
            )

        let tdef = tdef.WithSerializable(true)
        mgbuf.AddTypeDef(tref, tdef, false, false, None)

let CodegenAssembly cenv eenv mgbuf implFiles =
    match List.tryFrontAndBack implFiles with
    | None -> ()
    | Some (firstImplFiles, lastImplFile) ->
        let eenv = List.fold (GenImplFile cenv mgbuf None) eenv firstImplFiles
        let eenv = GenImplFile cenv mgbuf cenv.options.mainMethodInfo eenv lastImplFile

        // Some constructs generate residue types and bindings. Generate these now. They don't result in any
        // top-level initialization code.
        let extraBindings = mgbuf.GrabExtraBindingsToGenerate()
        //printfn "#extraBindings = %d" extraBindings.Length
        if not (isNil extraBindings) then
            let mexpr = TMDefs [ for b in extraBindings -> TMDefLet(b, range0) ]

            let _emptyTopInstrs, _emptyTopCode =
                CodeGenMethod
                    cenv
                    mgbuf
                    ([],
                     "unused",
                     eenv,
                     0,
                     None,
                     (fun cgbuf eenv ->
                         let lazyInitInfo = ResizeArray()
                         let qname = QualifiedNameOfFile(mkSynId range0 "unused")

                         LocalScope "module" cgbuf (fun (_, endMark) ->
                             let eenv =
                                 AddBindingsForModuleOrNamespaceContents (AllocValReprWithinExpr cenv cgbuf endMark) eenv.cloc eenv mexpr

                             let _eenvEnv = GenModuleOrNamespaceContents cenv cgbuf qname lazyInitInfo eenv mexpr
                             ())),
                     range0)
            //printfn "#_emptyTopInstrs = %d" _emptyTopInstrs.Length
            ()

        mgbuf.AddInitializeScriptsInOrderToEntryPoint(eenv.imports)

//-------------------------------------------------------------------------
// When generating a module we just write into mutable
// structures representing the contents of the module.
//-------------------------------------------------------------------------

let GetEmptyIlxGenEnv (g: TcGlobals) ccu =
    let thisCompLoc = CompLocForCcu ccu

    {
        tyenv = TypeReprEnv.Empty
        cloc = thisCompLoc
        exitSequel = Return
        valsInScope = ValMap<_>.Empty
        witnessesInScope = EmptyTraitWitnessInfoHashMap g
        suppressWitnesses = false
        someTypeInThisAssembly = g.ilg.typ_Object // dummy value
        isFinalFile = false
        letBoundVars = []
        liveLocals = IntMap.empty ()
        innerVals = []
        sigToImplRemapInfo = [] (* "module remap info" *)
        withinSEH = false
        isInLoop = false
        initLocals = true
        imports = None
    }

type IlxGenResults =
    {
        ilTypeDefs: ILTypeDef list
        ilAssemAttrs: ILAttribute list
        ilNetModuleAttrs: ILAttribute list
        topAssemblyAttrs: Attribs
        permissionSets: ILSecurityDecl list
        quotationResourceInfo: (ILTypeRef list * byte[]) list
    }

let GenerateCode (cenv, anonTypeTable, eenv, CheckedAssemblyAfterOptimization implFiles, assemAttribs, moduleAttribs) =

    use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.IlxGen
    let g = cenv.g

    // Generate the implementations into the mgbuf
    let mgbuf = AssemblyBuilder(cenv, anonTypeTable)

    let eenv =
        { eenv with
            cloc = CompLocForFragment cenv.options.fragName cenv.viewCcu
        }

    // Generate the PrivateImplementationDetails type
    GenTypeDefForCompLoc(
        cenv,
        eenv,
        mgbuf,
        CompLocForPrivateImplementationDetails eenv.cloc,
        useHiddenInitCode,
        [],
        ILTypeInit.BeforeField,
        true (* atEnd= *) ,
        true
    )

    // Generate the whole assembly
    CodegenAssembly cenv eenv mgbuf implFiles

    let ilAssemAttrs =
        GenAttrs cenv eenv (assemAttribs |> List.filter (fun a -> not (IsAssemblyVersionAttribute g a)))

    let tdefs, reflectedDefinitions = mgbuf.Close()

    // Generate the quotations
    let quotationResourceInfo =
        match reflectedDefinitions with
        | [] -> []
        | _ ->
            let qscope =
                QuotationTranslator.QuotationGenerationScope.Create(
                    g,
                    cenv.amap,
                    cenv.viewCcu,
                    cenv.tcVal,
                    QuotationTranslator.IsReflectedDefinition.Yes
                )

            let defns =
                reflectedDefinitions
                |> List.choose (fun ((methName, v), e) ->
                    try
                        let mbaseR, astExpr =
                            QuotationTranslator.ConvReflectedDefinition qscope methName v e

                        Some(mbaseR, astExpr)
                    with QuotationTranslator.InvalidQuotedTerm e ->
                        warning e
                        None)

            let referencedTypeDefs, typeSplices, exprSplices = qscope.Close()

            for _typeSplice, m in typeSplices do
                error (InternalError("A free type variable was detected in a reflected definition", m))

            for _exprSplice, m in exprSplices do
                error (Error(FSComp.SR.ilReflectedDefinitionsCannotUseSliceOperator (), m))

            let defnsResourceBytes = defns |> QuotationPickler.PickleDefns

            [ (referencedTypeDefs, defnsResourceBytes) ]

    let ilNetModuleAttrs = GenAttrs cenv eenv moduleAttribs

    let casApplied = Dictionary<Stamp, bool>()

    let securityAttrs, topAssemblyAttrs =
        assemAttribs
        |> List.partition (fun a -> IsSecurityAttribute g cenv.amap casApplied a rangeStartup)
    // remove any security attributes from the top-level assembly attribute list
    let permissionSets = CreatePermissionSets cenv eenv securityAttrs

    {
        ilTypeDefs = tdefs
        ilAssemAttrs = ilAssemAttrs
        ilNetModuleAttrs = ilNetModuleAttrs
        topAssemblyAttrs = topAssemblyAttrs
        permissionSets = permissionSets
        quotationResourceInfo = quotationResourceInfo
    }

//-------------------------------------------------------------------------
// For printing values in fsi we want to lookup the value of given vrefs.
// The storage in the eenv says if the vref is stored in a static field.
// If we know how/where the field was generated, then we can lookup via reflection.
//-------------------------------------------------------------------------

open System

/// The lookup* functions are the conversions available from ilreflect.
type ExecutionContext =
    {
        LookupTypeRef: ILTypeRef -> Type
        LookupType: ILType -> Type
    }

// A helper to generate a default value for any System.Type. I couldn't find a System.Reflection
// method to do this.
let defaultOf =
    let gminfo =
        lazy
            (match <@@ Unchecked.defaultof<int> @@> with
             | Quotations.Patterns.Call (_, minfo, _) -> minfo.GetGenericMethodDefinition()
             | _ -> failwith "unexpected failure decoding quotation at ilxgen startup")

    fun ty -> gminfo.Value.MakeGenericMethod([| ty |]).Invoke(null, [||])

/// Top-level val bindings are stored (for example) in static fields.
/// In the FSI case, these fields are be created and initialised, so we can recover the object.
/// IlxGen knows how v was stored, and then ilreflect knows how this storage was generated.
/// IlxGen converts (v: Tast.Val) to AbsIL data structures.
/// Ilreflect converts from AbsIL data structures to emitted Type, FieldInfo, MethodInfo etc.
let LookupGeneratedValue (cenv: cenv) (ctxt: ExecutionContext) eenv (v: Val) =
    try
        // Convert the v.Type into a System.Type according to ilxgen and ilreflect.
        let objTyp () =
            let ilTy = GenType cenv v.Range TypeReprEnv.Empty v.Type
            ctxt.LookupType ilTy
        // Lookup the compiled v value (as an object).
        match StorageForVal v.Range v eenv with
        | StaticPropertyWithField (fspec, _, hasLiteralAttr, ilContainerTy, _, _, ilGetterMethRef, _, _) ->
            let obj =
                if hasLiteralAttr then
                    let staticTy = ctxt.LookupTypeRef fspec.DeclaringTypeRef
                    // Checked: This FieldInfo (FieldBuilder) supports GetValue().
                    staticTy.GetField(fspec.Name).GetValue(null: obj)
                else
                    let staticTy = ctxt.LookupTypeRef ilContainerTy.TypeRef
                    // We can't call .Invoke on the ILMethodRef's MethodInfo,
                    // because it is the MethodBuilder and that does not support Invoke.
                    // Rather, we look for the getter MethodInfo from the built type and .Invoke on that.
                    let methInfo =
                        staticTy.GetMethod(ilGetterMethRef.Name, BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)

                    methInfo.Invoke(null, null)

            Some(obj, objTyp ())

        | StaticProperty (ilGetterMethSpec, _) ->
            let obj =
                let staticTy = ctxt.LookupTypeRef ilGetterMethSpec.MethodRef.DeclaringTypeRef
                // We can't call .Invoke on the ILMethodRef's MethodInfo,
                // because it is the MethodBuilder and that does not support Invoke.
                // Rather, we look for the getter MethodInfo from the built type and .Invoke on that.
                let methInfo =
                    staticTy.GetMethod(ilGetterMethSpec.Name, BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)

                methInfo.Invoke(null, null)

            Some(obj, objTyp ())

        | Null -> Some(null, objTyp ())
        | Local _ -> None
        | Method _ -> None
        | Arg _ -> None
        | Env _ -> None
    with e ->
#if DEBUG
        printf "ilxGen.LookupGeneratedValue for v=%s caught exception:\n%A\n\n" v.LogicalName e
#endif
        None

// Invoke the set_Foo method for a declaration with a value. Used to create variables with values programatically in fsi.exe.
let SetGeneratedValue (ctxt: ExecutionContext) eenv isForced (v: Val) (value: obj) =
    try
        match StorageForVal v.Range v eenv with
        | StaticPropertyWithField (fspec, _, hasLiteralAttr, _, _, _, _f, ilSetterMethRef, _) ->
            if not hasLiteralAttr && (v.IsMutable || isForced) then
                if isForced then
                    let staticTy = ctxt.LookupTypeRef fspec.DeclaringTypeRef

                    let fieldInfo =
                        staticTy.GetField(fspec.Name, BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)

                    fieldInfo.SetValue(null, value)
                else
                    let staticTy = ctxt.LookupTypeRef ilSetterMethRef.DeclaringTypeRef

                    let methInfo =
                        staticTy.GetMethod(ilSetterMethRef.Name, BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)

                    methInfo.Invoke(null, [| value |]) |> ignore
        | _ -> ()
    with e ->
#if DEBUG
        printf "ilxGen.SetGeneratedValue for v=%s caught exception:\n%A\n\n" v.LogicalName e
#endif
        ()

// Invoke the set_Foo method for a declaration with a default/null value. Used to release storage in fsi.exe
let ClearGeneratedValue (ctxt: ExecutionContext) eenv (v: Val) =
    try
        match StorageForVal v.Range v eenv with
        | StaticPropertyWithField (fspec, _, hasLiteralAttr, _, _, _, _ilGetterMethRef, _ilSetterMethRef, _) ->
            if not hasLiteralAttr && v.IsMutable then
                let ty = ctxt.LookupType fspec.ActualType
                SetGeneratedValue ctxt eenv false v (defaultOf ty)
        | _ -> ()
    with e ->
#if DEBUG
        printf "ilxGen.ClearGeneratedValue for v=%s caught exception:\n%A\n\n" v.LogicalName e
#endif
        ()

/// The published API from the ILX code generator
type IlxAssemblyGenerator(amap: ImportMap, tcGlobals: TcGlobals, tcVal: ConstraintSolver.TcValF, ccu: CcuThunk) =

    // The incremental state held by the ILX code generator
    let mutable ilxGenEnv = GetEmptyIlxGenEnv tcGlobals ccu
    let anonTypeTable = AnonTypeGenerationTable()
    // Dictionaries are safe here as they will only be used during the codegen stage - will happen on a single thread.
    let intraAssemblyInfo =
        {
            StaticFieldInfo = Dictionary<_, _>(HashIdentity.Structural)
        }

    let casApplied = Dictionary<Stamp, bool>()

    let cenv =
        {
            g = tcGlobals
            ilxPubCloEnv =
                EraseClosures.newIlxPubCloEnv (
                    tcGlobals.ilg,
                    tcGlobals.AddMethodGeneratedAttributes,
                    tcGlobals.AddFieldGeneratedAttributes,
                    tcGlobals.AddFieldNeverAttributes
                )
            tcVal = tcVal
            viewCcu = ccu
            ilUnitTy = None
            namedDebugPointsForInlinedCode = Map.empty
            amap = amap
            casApplied = casApplied
            intraAssemblyInfo = intraAssemblyInfo
            optionsOpt = None
            optimizeDuringCodeGen = (fun _flag expr -> expr)
            stackGuard = StackGuard(IlxGenStackGuardDepth)
        }

    /// Register a set of referenced assemblies with the ILX code generator
    member _.AddExternalCcus ccus =
        ilxGenEnv <- AddExternalCcusToIlxGenEnv cenv tcGlobals ilxGenEnv ccus

    /// Register a fragment of the current assembly with the ILX code generator. If 'isIncrementalFragment' is true then the input
    /// is assumed to be a fragment 'typed' into FSI.EXE, otherwise the input is assumed to be the result of a '#load'
    member _.AddIncrementalLocalAssemblyFragment(isIncrementalFragment, fragName, typedImplFiles) =
        ilxGenEnv <-
            AddIncrementalLocalAssemblyFragmentToIlxGenEnv(
                cenv,
                isIncrementalFragment,
                tcGlobals,
                ccu,
                fragName,
                intraAssemblyInfo,
                ilxGenEnv,
                typedImplFiles
            )

    /// Generate ILX code for an assembly fragment
    member _.GenerateCode(codeGenOpts, typedAssembly: CheckedAssemblyAfterOptimization, assemAttribs, moduleAttribs) =
        let namedDebugPointsForInlinedCode =
            let (CheckedAssemblyAfterOptimization impls) = typedAssembly

            [|
                for impl in impls do
                    let (CheckedImplFile (namedDebugPointsForInlinedCode = dps)) = impl.ImplFile

                    for KeyValue (k, v) in dps do
                        yield (k, v)
            |]

        let cenv =
            { cenv with
                optionsOpt = Some codeGenOpts
                namedDebugPointsForInlinedCode = Map.ofArray namedDebugPointsForInlinedCode
            }

        GenerateCode(cenv, anonTypeTable, ilxGenEnv, typedAssembly, assemAttribs, moduleAttribs)

    /// Invert the compilation of the given value and clear the storage of the value
    member _.ClearGeneratedValue(ctxt, v) = ClearGeneratedValue ctxt ilxGenEnv v

    /// Invert the compilation of the given value and set the storage of the value, even if it is immutable
    member _.ForceSetGeneratedValue(ctxt, v, value: obj) =
        SetGeneratedValue ctxt ilxGenEnv true v value

    /// Invert the compilation of the given value and return its current dynamic value and its compiled System.Type
    member _.LookupGeneratedValue(ctxt, v) =
        LookupGeneratedValue cenv ctxt ilxGenEnv v
