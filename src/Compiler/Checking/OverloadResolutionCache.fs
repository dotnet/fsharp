// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.OverloadResolutionCache

open System.Runtime.CompilerServices
open System.Threading

open Internal.Utilities.Library
open Internal.Utilities.TypeHashing.StructuralUtilities

open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.Caches
open FSharp.Compiler.InfoReader
open FSharp.Compiler.Infos
open FSharp.Compiler.MethodCalls
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

[<Struct; RequireQualifiedAccess>]
type ExpectedReturnType =
    | NotConsidered
    | Unknown
    | MustEqual of equalTy: TType
    | MustConvertTo of isMethodArg: bool * convertibleTy: TType

[<Struct>]
type OverloadResolutionCacheKey = OverloadResolutionCacheKey of callStructure: TypeStructure

type OverloadResolutionCacheResult =
    {
        WinnerIndex: int
        IsExactMatch: bool
        LessConcreteLoserIndexes: int list
    }

let getOverloadResolutionCache =
    let factory (g: TcGlobals) =
        let options =
            match g.compilationMode with
            | CompilationMode.OneOff -> CacheOptions.getDefault HashIdentity.Structural |> CacheOptions.withNoEviction
            | CompilationMode.Unset
            | CompilationMode.Service
            | CompilationMode.Interactive ->
                { CacheOptions.getDefault HashIdentity.Structural with
                    TotalCapacity = 4096
                    HeadroomPercentage = 50
                }

        new Cache<OverloadResolutionCacheKey, OverloadResolutionCacheResult>(options, "overloadResolutionCache")

    Extras.WeakMap.getOrCreate factory

let private methodDefIds = ConditionalWeakTable<ILMethodDef, StrongBox<int64>>()

let mutable private lastMethodDefId = 0L

let private createMethodDefId =
    ConditionalWeakTable<ILMethodDef, StrongBox<int64>>.CreateValueCallback(fun _ -> StrongBox(Interlocked.Increment &lastMethodDefId))

// IL method definitions have no stamp: two are the same method only when they are the same object.
let private getMethodDefId (methodDef: ILMethodDef) =
    methodDefIds.GetValue(methodDef, createMethodDefId).Value

[<RequireQualifiedAccess>]
module private ExpectedReturnTag =
    [<Literal>]
    let NotConsidered = 0

    [<Literal>]
    let Unknown = 1

    [<Literal>]
    let MustEqual = 2

    [<Literal>]
    let MethodArgMustConvertTo = 3

    [<Literal>]
    let MustConvertTo = 4

[<RequireQualifiedAccess>]
module private TypeArgTag =
    [<Literal>]
    let Unsolved = 0

    [<Literal>]
    let Solved = 1

[<RequireQualifiedAccess>]
module private MethodTag =
    [<Literal>]
    let FSharpMethod = 0

    [<Literal>]
    let ILMethod = 1

    [<Literal>]
    let DefaultStructCtor = 2

    [<Literal>]
    let RecdCtor = 3

    [<Literal>]
    let ModifiedReturnType = 4

[<RequireQualifiedAccess>]
module private CandidateFlag =
    [<Literal>]
    let UsesParamArrayConversion = 1

    [<Literal>]
    let HasAssociatedProperty = 2

    [<Literal>]
    let IsExtensionMember = 4

    [<Literal>]
    let DeclaringTypeChanged = 8

    [<Literal>]
    let ExtensionRankShift = 4

let inline private flagIf (isSet: bool) (value: int) = if isSet then value else 0

let private addType (g: TcGlobals) (builder: ExactStructureBuilder) (ty: TType) = builder.AddType(stripTyEqns g ty)

let private addTypes (g: TcGlobals) (builder: ExactStructureBuilder) (tys: TType list) =
    builder.AddNumber tys.Length

    for ty in tys do
        addType g builder ty

let private addCallerArgs (g: TcGlobals) (builder: ExactStructureBuilder) (callerArgs: CallerArgs<'T>) =
    // The result of a call with named arguments also depends on the properties and fields they can set.
    if not (callerArgs.Named |> List.forall List.isEmpty) then
        builder.Reject()

    builder.AddNumber callerArgs.Unnamed.Length

    for argGroup in callerArgs.Unnamed do
        builder.AddNumber argGroup.Length

        for callerArg in argGroup do
            if callerArg.IsExplicitOptional then
                builder.Reject()

            addType g builder callerArg.CallerArgumentType

let private addExpectedReturnType (g: TcGlobals) (builder: ExactStructureBuilder) (expectedReturnType: ExpectedReturnType) =
    match expectedReturnType with
    | ExpectedReturnType.NotConsidered -> builder.AddNumber ExpectedReturnTag.NotConsidered
    | ExpectedReturnType.Unknown -> builder.AddNumber ExpectedReturnTag.Unknown
    | ExpectedReturnType.MustEqual ty ->
        builder.AddNumber ExpectedReturnTag.MustEqual
        addType g builder ty
    | ExpectedReturnType.MustConvertTo(isMethodArg, ty) ->
        builder.AddNumber(
            if isMethodArg then
                ExpectedReturnTag.MethodArgMustConvertTo
            else
                ExpectedReturnTag.MustConvertTo
        )

        addType g builder ty

let private isSameTypar (g: TcGlobals) (typar: Typar) (ty: TType) =
    match tryDestTyparTy g ty with
    | ValueSome other -> other === typar
    | ValueNone -> false

// Checking the arguments of a call can solve or constrain the type arguments of a method before it is resolved.
let private addCalledTyArgs (g: TcGlobals) (builder: ExactStructureBuilder) (candidate: CalledMeth<'T>) =
    let tyArgs = candidate.CalledTyArgs
    builder.AddNumber tyArgs.Length

    if not tyArgs.IsEmpty then
        let formalTypars = candidate.Method.FormalMethodTypars

        if formalTypars.Length <> tyArgs.Length then
            builder.Reject()
        else
            (formalTypars, tyArgs)
            ||> List.iter2 (fun formalTypar tyArg ->
                match tryDestTyparTy g tyArg with
                | ValueSome typar when typar.Rigidity <> TyparRigidity.Rigid ->
                    let isConstrainedByCall = typar.Constraints.Length <> formalTypar.Constraints.Length

                    // The solution of a member constraint can come from the extension members in scope at the call.
                    if isConstrainedByCall || typar.Constraints |> List.exists _.IsMayResolveMember then
                        builder.Reject()

                    builder.AddNumber TypeArgTag.Unsolved
                    builder.AddNumber(tyArgs |> List.findIndex (isSameTypar g typar))
                | _ ->
                    builder.AddNumber TypeArgTag.Solved
                    addType g builder tyArg)

let rec private addMethod (g: TcGlobals) (builder: ExactStructureBuilder) (returnTypeIsConsidered: bool) (minfo: MethInfo) =
    match minfo with
    | FSMeth(_, _, vref, _) ->
        builder.AddNumber MethodTag.FSharpMethod
        builder.AddStamp vref.Stamp

        // The signature of a method is only final once the file declaring it has been checked:
        // until then the signature it has at the call is part of the call.
        match vref.TryDeclaringEntity with
        | Parent declaringEntity when declaringEntity.IsAugmentationClosed -> ()
        | Parent _
        | ParentNone ->
            let _, signatureTy = tryDestForallTy g vref.Type
            addType g builder signatureTy
    | ILMeth(_, ilMethInfo, _) ->
        builder.AddNumber MethodTag.ILMethod
        // The same definition can be imported more than once, with its types standing for different assemblies.
        builder.AddStamp ilMethInfo.DeclaringTyconRef.Stamp
        builder.AddStamp(getMethodDefId ilMethInfo.RawMetadata)
    | DefaultStructCtor _ -> builder.AddNumber MethodTag.DefaultStructCtor
    | RecdCtor _ -> builder.AddNumber MethodTag.RecdCtor
    | MethInfoWithModifiedReturnType(original, returnTy) ->
        builder.AddNumber MethodTag.ModifiedReturnType

        if returnTypeIsConsidered then
            addType g builder returnTy

        addMethod g builder returnTypeIsConsidered original
#if !NO_TYPEPROVIDERS
    | ProvidedMeth _ -> builder.Reject()
#endif

// Extension members are ranked against each other and against other members by their priority alone.
let private getExtensionRank (candidates: CalledMeth<'T> list) (priority: ExtensionMethodPriority) =
    let mutable rank = 0

    for other in candidates do
        if other.Method.ExtensionMemberPriority < priority then
            rank <- rank + 1

    rank

let private addCandidates (g: TcGlobals) (builder: ExactStructureBuilder) (returnTypeIsConsidered: bool) (candidates: CalledMeth<'T> list) =
    builder.AddNumber candidates.Length

    let haveSameExtensionPriority =
        match candidates with
        | [] -> true
        | first :: rest ->
            let priority = first.Method.ExtensionMemberPriority

            rest
            |> List.forall (fun candidate -> candidate.Method.ExtensionMemberPriority = priority)

    let mutable previousDeclaringTy = ValueNone

    for candidate in candidates do
        if not builder.IsRejected then
            let minfo = candidate.Method
            let declaringTy = minfo.ApparentEnclosingType

            let declaringTyChanged =
                match previousDeclaringTy with
                | ValueSome previous -> not (previous === declaringTy)
                | ValueNone -> true

            previousDeclaringTy <- ValueSome declaringTy

            let extensionRank =
                if haveSameExtensionPriority then
                    0
                else
                    getExtensionRank candidates minfo.ExtensionMemberPriority

            builder.AddNumber(
                flagIf candidate.UsesParamArrayConversion CandidateFlag.UsesParamArrayConversion
                ||| flagIf candidate.AssociatedPropertyInfo.IsSome CandidateFlag.HasAssociatedProperty
                ||| flagIf minfo.IsExtensionMember CandidateFlag.IsExtensionMember
                ||| flagIf declaringTyChanged CandidateFlag.DeclaringTypeChanged
                ||| (extensionRank <<< CandidateFlag.ExtensionRankShift)
            )

            if declaringTyChanged then
                addType g builder declaringTy

            // An override of a property is preferred by the type that declares the property.
            match candidate.AssociatedPropertyInfo with
            | Some pinfo -> addType g builder pinfo.ApparentEnclosingType
            | None -> ()

            addCalledTyArgs g builder candidate
            addMethod g builder returnTypeIsConsidered minfo

let tryComputeOverloadCacheKey
    (g: TcGlobals)
    (infoReader: InfoReader)
    (candidates: CalledMeth<'T> list)
    (explicitTyArgs: TType list)
    (callerArgs: CallerArgs<'T>)
    (expectedReturnType: ExpectedReturnType)
    : OverloadResolutionCacheKey voption =

    let builder = ExactStructureBuilder.Start g.checkNullness

    addCallerArgs g builder callerArgs

    if not builder.IsRejected then
        match candidates with
        | [] -> ()
        | first :: _ ->
            addTypes g builder first.CallerObjArgTys
            addTypes g builder (Option.toList first.OptionalStaticType)

        addTypes g builder explicitTyArgs
        addExpectedReturnType g builder expectedReturnType
        addCandidates g builder (not expectedReturnType.IsNotConsidered) candidates

    if builder.DependsOnFileBeingChecked then
        // Until established, a type can still change its kind, base type, abbreviation and constraints.
        if infoReader.IsEstablishingTypeDefinitions then
            builder.Reject()

        // Until its file has been checked a type can gain members and interfaces, and so can its base types
        // and the types it is constrained to, whether or not they are part of the call.
        builder.AddNumber infoReader.PublishedDeclarationCount

    builder.TryGetStructure() |> ValueOption.map OverloadResolutionCacheKey
