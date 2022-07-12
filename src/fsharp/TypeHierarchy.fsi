// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.TypeHierarchy

open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.Syntax
open FSharp.Compiler.Import
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.Xml
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

#if !NO_TYPEPROVIDERS
open FSharp.Compiler.TypeProviders
#endif

/// Get the base type of a type, taking into account type instantiations. Return None if the
/// type has no base type.
val GetSuperTypeOfType: g: TcGlobals -> amap: ImportMap -> m: range -> ty: TType -> TType option

/// Indicates whether we can skip interface types that lie outside the reference set
[<RequireQualifiedAccess>]
type SkipUnrefInterfaces =
    | Yes
    | No

/// Collect the set of immediate declared interface types for an F# type, but do not
/// traverse the type hierarchy to collect further interfaces.
val GetImmediateInterfacesOfType:
    skipUnref: SkipUnrefInterfaces -> g: TcGlobals -> amap: ImportMap -> m: range -> ty: TType -> TType list

/// Indicates whether we should visit multiple instantiations of the same generic interface or not
[<RequireQualifiedAccess>]
type AllowMultiIntfInstantiations =
    | Yes
    | No

/// Fold, do not follow interfaces (unless the type is itself an interface)
val FoldPrimaryHierarchyOfType:
    f: (TType -> 'a -> 'a) ->
    g: TcGlobals ->
    amap: ImportMap ->
    m: range ->
    allowMultiIntfInst: AllowMultiIntfInstantiations ->
    ty: TType ->
    acc: 'a ->
        'a

/// Fold, following interfaces. Skipping interfaces that lie outside the referenced assembly set is allowed.
val FoldEntireHierarchyOfType:
    f: (TType -> 'a -> 'a) ->
    g: TcGlobals ->
    amap: ImportMap ->
    m: range ->
    allowMultiIntfInst: AllowMultiIntfInstantiations ->
    ty: TType ->
    acc: 'a ->
        'a

/// Iterate, following interfaces. Skipping interfaces that lie outside the referenced assembly set is allowed.
val IterateEntireHierarchyOfType:
    f: (TType -> unit) ->
    g: TcGlobals ->
    amap: ImportMap ->
    m: range ->
    allowMultiIntfInst: AllowMultiIntfInstantiations ->
    ty: TType ->
        unit

/// Search for one element satisfying a predicate, following interfaces
val ExistsInEntireHierarchyOfType:
    f: (TType -> bool) ->
    g: TcGlobals ->
    amap: ImportMap ->
    m: range ->
    allowMultiIntfInst: AllowMultiIntfInstantiations ->
    ty: TType ->
        bool

/// Search for one element where a function returns a 'Some' result, following interfaces
val SearchEntireHierarchyOfType:
    f: (TType -> bool) -> g: TcGlobals -> amap: ImportMap -> m: range -> ty: TType -> TType option

/// Get all super types of the type, including the type itself
val AllSuperTypesOfType:
    g: TcGlobals ->
    amap: ImportMap ->
    m: range ->
    allowMultiIntfInst: AllowMultiIntfInstantiations ->
    ty: TType ->
        TType list

/// Get all interfaces of a type, including the type itself if it is an interface
val AllInterfacesOfType:
    g: TcGlobals ->
    amap: ImportMap ->
    m: range ->
    allowMultiIntfInst: AllowMultiIntfInstantiations ->
    ty: TType ->
        TType list

/// Check if two types have the same nominal head type
val HaveSameHeadType: g: TcGlobals -> ty1: TType -> ty2: TType -> bool

/// Check if a type has a particular head type
val HasHeadType: g: TcGlobals -> tcref: TyconRef -> ty2: TType -> bool

/// Check if a type exists somewhere in the hierarchy which has the same head type as the given type (note, the given type need not have a head type at all)
val ExistsSameHeadTypeInHierarchy:
    g: TcGlobals -> amap: ImportMap -> m: range -> typeToSearchFrom: TType -> typeToLookFor: TType -> bool

/// Check if a type exists somewhere in the hierarchy which has the given head type.
val ExistsHeadTypeInEntireHierarchy:
    g: TcGlobals -> amap: ImportMap -> m: range -> typeToSearchFrom: TType -> tcrefToLookFor: TyconRef -> bool

/// Read an Abstract IL type from metadata and convert to an F# type.
val ImportILTypeFromMetadata:
    amap: ImportMap -> m: range -> scoref: ILScopeRef -> tinst: TType list -> minst: TType list -> ilty: ILType -> TType

/// Read an Abstract IL type from metadata, including any attributes that may affect the type itself, and convert to an F# type.
val ImportILTypeFromMetadataWithAttributes:
    amap: ImportMap ->
    m: range ->
    scoref: ILScopeRef ->
    tinst: TType list ->
    minst: TType list ->
    ilty: ILType ->
    getCattrs: (unit -> ILAttributes) ->
        TType

/// Get the parameter type of an IL method.
val ImportParameterTypeFromMetadata:
    amap: ImportMap ->
    m: range ->
    ilty: ILType ->
    getCattrs: (unit -> ILAttributes) ->
    scoref: ILScopeRef ->
    tinst: TType list ->
    mist: TType list ->
        TType

/// Get the return type of an IL method, taking into account instantiations for type, return attributes and method generic parameters, and
/// translating 'void' to 'None'.
val ImportReturnTypeFromMetadata:
    amap: ImportMap ->
    m: range ->
    ilty: ILType ->
    getCattrs: (unit -> ILAttributes) ->
    scoref: ILScopeRef ->
    tinst: TType list ->
    minst: TType list ->
        TType option

/// Copy constraints.  If the constraint comes from a type parameter associated
/// with a type constructor then we are simply renaming type variables.  If it comes
/// from a generic method in a generic class (e.g. ty.M<_>) then we may be both substituting the
/// instantiation associated with 'ty' as well as copying the type parameters associated with
/// M and instantiating their constraints
///
/// Note: this now looks identical to constraint instantiation.

val CopyTyparConstraints: traitCtxt: ITraitContext option -> m: range -> tprefInst: TyparInst -> tporig: Typar -> TyparConstraint list

/// The constraints for each typar copied from another typar can only be fixed up once
/// we have generated all the new constraints, e.g. f<A :> List<B>, B :> List<A>> ...
val FixupNewTypars:
    traitCtxt: ITraitContext option ->
    m: range ->
    formalEnclosingTypars: Typars ->
    tinst: TType list ->
    tpsorig: Typars ->
    tps: Typars ->
        TyparInst * TTypes
