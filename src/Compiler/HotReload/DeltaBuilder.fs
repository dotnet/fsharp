module internal FSharp.Compiler.HotReload.DeltaBuilder

open System
open FSharp.Compiler.EnvironmentHelpers
open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.EditAndContinue
open FSharp.Compiler.HotReload
open FSharp.Compiler.HotReload.DefinitionMap
open FSharp.Compiler.HotReload.SymbolChanges
open FSharp.Compiler.HotReloadBaseline
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeDiff

let private traceMethodResolution = isEnvVarTruthy "FSHARP_HOTRELOAD_TRACE_METHODS"

let private checkedFiles (CheckedAssemblyAfterOptimization impls) =
    impls |> Seq.map (fun afterOpt -> afterOpt.ImplFile)

let private fileKey (CheckedImplFile(qualifiedNameOfFile = qual)) = qual.Text

let private buildLookup (files: seq<CheckedImplFile>) =
    files |> Seq.map (fun file -> fileKey file, file) |> Map.ofSeq

let private emptyDefinitionMap: FSharpDefinitionMap =
    {
        Changes = []
        RudeEdits = []
        RequiredCapabilities = []
    }

let private mergeDefinitionMaps (left: FSharpDefinitionMap) (right: FSharpDefinitionMap) : FSharpDefinitionMap =
    {
        Changes = left.Changes @ right.Changes
        RudeEdits = left.RudeEdits @ right.RudeEdits
        RequiredCapabilities =
            left.RequiredCapabilities @ right.RequiredCapabilities
            |> Set.ofList
            |> Set.toList
    }

let computeSymbolChanges
    (tcGlobals: TcGlobals)
    (capabilities: EditAndContinueCapabilities)
    (baseline: CheckedAssemblyAfterOptimization)
    (updated: CheckedAssemblyAfterOptimization)
    : FSharpSymbolChanges =
    let baselineFiles = checkedFiles baseline |> Seq.toList
    let updatedFiles = checkedFiles updated |> Seq.toList

    let baselineLookup = buildLookup baselineFiles
    let updatedLookup = buildLookup updatedFiles

    let definitionMap, skippedFiles, diffedFiles =
        ((emptyDefinitionMap, 0, 0), updatedFiles)
        ||> Seq.fold (fun (acc, skippedFiles, diffedFiles) updatedFile ->
            match Map.tryFind (fileKey updatedFile) baselineLookup with
            | Some baselineFile when obj.ReferenceEquals(baselineFile, updatedFile) -> acc, skippedFiles + 1, diffedFiles
            | Some baselineFile ->
                let diff = diffImplementationFile tcGlobals capabilities baselineFile updatedFile
                let map = FSharpDefinitionMap.ofTypedTreeDiff diff
                mergeDefinitionMaps acc map, skippedFiles, diffedFiles + 1
            | None ->
                // No matching baseline file: surface a rude edit rather than silently dropping the change.
                let rudeEdit =
                    {
                        Symbol = None
                        Kind = RudeEditKind.Unsupported
                        Message = $"File '{fileKey updatedFile}' is new or renamed; full rebuild required."
                    }

                mergeDefinitionMaps
                    acc
                    { emptyDefinitionMap with
                        RudeEdits = [ rudeEdit ]
                    },
                skippedFiles,
                diffedFiles)

    if traceMethodResolution then
        printfn "[fsharp-hotreload][diff] checked implementation files: skipped=%d diffed=%d" skippedFiles diffedFiles

    let definitionMap =
        ((definitionMap, baselineLookup)
         ||> Map.fold (fun acc key _ ->
             if Map.containsKey key updatedLookup then
                 acc
             else
                 let rudeEdit =
                     {
                         Symbol = None
                         Kind = RudeEditKind.Unsupported
                         Message = $"File '{key}' was removed or renamed; full rebuild required."
                     }

                 mergeDefinitionMaps
                     acc
                     { emptyDefinitionMap with
                         RudeEdits = [ rudeEdit ]
                     }))

    FSharpSymbolChanges.ofDefinitionMap definitionMap

let private joinPath (segments: string list) = String.concat "." segments

// Normalize nested type separators ("+" vs ".") so symbol/baseline matching is resilient
// to representation differences while still using canonical baseline names in emitted deltas.
let private splitTypePath (typeName: string) =
    typeName.Split([| '.'; '+' |], StringSplitOptions.RemoveEmptyEntries)
    |> Array.toList

let private buildTypePathLookup (typeTokens: Map<string, int>) =
    typeTokens
    |> Map.toSeq
    |> Seq.fold
        (fun acc (name, _) ->
            let key = splitTypePath name
            let existing = acc |> Map.tryFind key |> Option.defaultValue []
            acc |> Map.add key (name :: existing))
        Map.empty

type private TypeNameResolution =
    | TypeNameResolved of string
    | TypeNameMissing
    | TypeNameAmbiguous of string list

let private resolveTypeNameByPath (typeTokens: Map<string, int>) (typePathLookup: Map<string list, string list>) (names: string list) =
    let candidates =
        names
        |> List.collect (fun name -> typePathLookup |> Map.tryFind (splitTypePath name) |> Option.defaultValue [])
        |> List.distinct
        |> List.filter (fun candidate -> typeTokens |> Map.containsKey candidate)

    match candidates with
    | [] -> TypeNameMissing
    | [ resolved ] -> TypeNameResolved resolved
    | ambiguous -> TypeNameAmbiguous ambiguous

let private typeNamesEquivalent (left: string) (right: string) =
    String.Equals(left, right, StringComparison.Ordinal)
    || splitTypePath left = splitTypePath right

let private deduplicate list =
    list
    |> List.fold (fun acc item -> if List.contains item acc then acc else item :: acc) []
    |> List.rev

let private deduplicateSymbols symbols =
    symbols
    |> List.fold
        (fun acc symbol ->
            if acc |> List.exists (fun existing -> existing.Stamp = symbol.Stamp) then
                acc
            else
                symbol :: acc)
        []
    |> List.rev

let private methodNameOfSymbol (symbol: SymbolId) =
    symbol.CompiledName |> Option.defaultValue symbol.LogicalName

let rec private ilTypeIdentity (ilType: ILType) : RuntimeTypeIdentity =
    match ilType with
    | ILType.Void -> RuntimeTypeIdentity.VoidType
    | ILType.Array(ILArrayShape shape, elementType) -> RuntimeTypeIdentity.ArrayType(shape.Length, ilTypeIdentity elementType)
    | ILType.Value typeSpec
    | ILType.Boxed typeSpec -> ilTypeSpecIdentity typeSpec
    | ILType.Ptr elementType -> RuntimeTypeIdentity.PointerType(ilTypeIdentity elementType)
    | ILType.Byref elementType -> RuntimeTypeIdentity.ByRefType(ilTypeIdentity elementType)
    | ILType.FunctionPointer signature ->
        RuntimeTypeIdentity.FunctionPointerType(ilTypeIdentity signature.ReturnType, signature.ArgTypes |> List.map ilTypeIdentity)
    | ILType.TypeVar index -> RuntimeTypeIdentity.TypeVariable(int index)
    | ILType.Modified(_, _, innerType) -> ilTypeIdentity innerType

and private ilTypeSpecIdentity (typeSpec: ILTypeSpec) : RuntimeTypeIdentity =
    RuntimeTypeIdentity.NamedType(typeSpec.TypeRef.FullName, typeSpec.GenericArgs |> List.map ilTypeIdentity)

let private methodKeyMatchesSymbol (symbol: SymbolId) (key: MethodDefinitionKey) =
    let nameMatches =
        String.Equals(key.Name, methodNameOfSymbol symbol, StringComparison.Ordinal)

    let genericArityMatches =
        match symbol.GenericArity with
        | Some arity -> key.GenericArity = arity
        | None -> true

    nameMatches && genericArityMatches

let private fsharpUnitRuntimeTypeIdentity =
    RuntimeTypeIdentity.NamedType("Microsoft.FSharp.Core.Unit", [])

let private normalizeSymbolParameterTypeIdentities (symbol: SymbolId) (parameterTypeIdentities: RuntimeTypeIdentity list) =
    match symbol.TotalArgCount, parameterTypeIdentities with
    | Some 0, [ unitType ] when unitType = fsharpUnitRuntimeTypeIdentity -> []
    | _ -> parameterTypeIdentities

let private methodParameterTypesMatchSymbol (symbol: SymbolId) (key: MethodDefinitionKey) =
    match symbol.ParameterTypeIdentities with
    | Some parameterTypeIdentities ->
        let methodParameterTypes = key.ParameterTypes |> List.map ilTypeIdentity

        let normalizedParameterTypeIdentities =
            normalizeSymbolParameterTypeIdentities symbol parameterTypeIdentities

        methodParameterTypes = normalizedParameterTypeIdentities
    | None -> false

let private methodReturnTypeMatchesSymbol (symbol: SymbolId) (key: MethodDefinitionKey) =
    match symbol.ReturnTypeIdentity with
    | Some returnTypeIdentity -> ilTypeIdentity key.ReturnType = returnTypeIdentity
    | None -> false

let private formatSymbolIdentity (symbol: SymbolId) =
    let path =
        match symbol.Path with
        | [] -> "<global>"
        | _ -> joinPath symbol.Path

    let memberName = methodNameOfSymbol symbol
    $"{path}::{memberName}"

type private MethodResolutionResult =
    | MethodResolved of MethodDefinitionKey
    | MethodIdentityMissing of string list
    | MethodMissing
    | MethodAmbiguous of MethodDefinitionKey list

type private MethodIdentityKey =
    {
        DeclaringTypeToken: int
        Name: string
        GenericArity: int
        ParameterTypes: RuntimeTypeIdentity list
        ReturnType: RuntimeTypeIdentity
    }

let private methodIdentityKey (declaringTypeToken: int) (methodKey: MethodDefinitionKey) : MethodIdentityKey =
    {
        DeclaringTypeToken = declaringTypeToken
        Name = methodKey.Name
        GenericArity = methodKey.GenericArity
        ParameterTypes = methodKey.ParameterTypes |> List.map ilTypeIdentity
        ReturnType = ilTypeIdentity methodKey.ReturnType
    }

let private tryMethodIdentityKeyFromSymbol (declaringTypeToken: int) (symbol: SymbolId) : MethodIdentityKey option =
    match symbol.GenericArity, symbol.ParameterTypeIdentities, symbol.ReturnTypeIdentity with
    | Some genericArity, Some parameterTypes, Some returnType ->
        let normalizedParameterTypes =
            normalizeSymbolParameterTypeIdentities symbol parameterTypes

        Some
            {
                DeclaringTypeToken = declaringTypeToken
                Name = methodNameOfSymbol symbol
                GenericArity = genericArity
                ParameterTypes = normalizedParameterTypes
                ReturnType = returnType
            }
    | _ -> None

let private describeMethodKey (key: MethodDefinitionKey) =
    let parameterCount = key.ParameterTypes.Length
    $"{key.DeclaringType}::{key.Name}/{parameterCount}`{key.GenericArity}"

let private missingRuntimeSignatureIdentityParts (symbol: SymbolId) =
    [
        if symbol.CompiledName.IsNone then
            yield "compiled name"
        if symbol.TotalArgCount.IsNone then
            yield "argument count"
        if symbol.GenericArity.IsNone then
            yield "generic arity"
        if symbol.ParameterTypeIdentities.IsNone then
            yield "parameter type identities"
        if symbol.ReturnTypeIdentity.IsNone then
            yield "return type identity"
    ]

// Maps typed-tree symbol changes to baseline tokens using fail-closed matching:
// unresolved or ambiguous bindings return errors instead of silently dropping edits.
let mapSymbolChangesToDelta
    (baseline: FSharpEmitBaseline)
    (changes: FSharpSymbolChanges)
    : Result<string list * MethodDefinitionKey list * AccessorUpdate list, string list> =

    if traceMethodResolution then
        let formatSymbol (symbol: SymbolId) =
            sprintf
                "name=%s path=%A kind=%A memberKind=%A synthesized=%b"
                symbol.LogicalName
                symbol.Path
                symbol.Kind
                symbol.MemberKind
                symbol.IsSynthesized

        let formatUpdated (change: UpdatedSymbolChange) =
            sprintf "%s semanticEdit=%A containingEntity=%A" (formatSymbol change.Symbol) change.Kind change.ContainingEntity

        let addedText = changes.Added |> List.map formatSymbol |> String.concat " | "

        let deletedText = changes.Deleted |> List.map formatSymbol |> String.concat " | "

        let updatedText = changes.Updated |> List.map formatUpdated |> String.concat " | "

        printfn "[fsharp-hotreload][delta-builder] changes summary: added=[%s] deleted=[%s] updated=[%s]" addedText deletedText updatedText

    let singlePathCandidate (segments: string list) =
        match segments with
        | [] -> []
        | _ -> [ joinPath segments ]

    let candidateEntityNames (symbol: SymbolId) =
        singlePathCandidate (symbol.Path @ [ symbol.LogicalName ])

    let suffixPathCandidates (segments: string list) =
        let rec tails acc remaining =
            match remaining with
            | [] -> List.rev acc
            | _ :: tail as segs -> tails (joinPath segs :: acc) tail

        tails [] segments

    let candidateContainingTypeNames (symbol: SymbolId) = suffixPathCandidates symbol.Path

    let typePathLookup = buildTypePathLookup baseline.TypeTokens

    let resolveTypeName (names: string list) =
        let exactMatches =
            names
            |> List.filter (fun name -> Map.containsKey name baseline.TypeTokens)
            |> deduplicate

        match exactMatches with
        | [ resolved ] -> TypeNameResolved resolved
        | _ :: _ as ambiguous -> TypeNameAmbiguous ambiguous
        | [] -> resolveTypeNameByPath baseline.TypeTokens typePathLookup names

    let methodIdentityIndex =
        let index =
            System.Collections.Generic.Dictionary<MethodIdentityKey, ResizeArray<MethodDefinitionKey>>(HashIdentity.Structural)

        for KeyValue(methodKey, _) in baseline.MethodTokens do
            match baseline.TypeTokens |> Map.tryFind methodKey.DeclaringType with
            | Some declaringTypeToken ->
                let identity = methodIdentityKey declaringTypeToken methodKey

                let bucket =
                    match index.TryGetValue identity with
                    | true, existing -> existing
                    | _ ->
                        let created = ResizeArray<MethodDefinitionKey>()
                        index[identity] <- created
                        created

                bucket.Add methodKey
            | None -> ()

        index

    let lookupMethodsByIdentity (symbol: SymbolId) (resolvedTypeNames: string list) =
        let typeTokens =
            resolvedTypeNames
            |> List.choose (fun name -> baseline.TypeTokens |> Map.tryFind name)
            |> Seq.toList
            |> deduplicate

        let matchedMethods =
            typeTokens
            |> List.collect (fun typeToken ->
                match tryMethodIdentityKeyFromSymbol typeToken symbol with
                | Some identity ->
                    match methodIdentityIndex.TryGetValue identity with
                    | true, methods -> methods |> Seq.toList
                    | _ -> []
                | None -> [])
            |> deduplicate

        typeTokens, matchedMethods

    // ADDED entity symbols (new type definitions) have no baseline type token
    // by definition: the emitter discovers them by walking the fresh module against the
    // added-entity symbol names. Only updated/deleted/synthesized entities resolve here.
    let addedEntityStamps =
        changes
        |> FSharpSymbolChanges.addedEntitySymbols
        |> List.map (fun symbol -> symbol.Stamp)
        |> Set.ofList

    let updatedTypes, typeResolutionErrors =
        changes
        |> FSharpSymbolChanges.entitySymbolsWithChanges
        |> List.filter (fun symbol -> not (Set.contains symbol.Stamp addedEntityStamps))
        |> List.fold
            (fun (resolvedTypes, errors) symbol ->
                let candidates = symbol |> candidateEntityNames

                match resolveTypeName candidates with
                | TypeNameResolved resolvedTypeName -> resolvedTypeName :: resolvedTypes, errors
                | TypeNameAmbiguous ambiguousMatches ->
                    let errorMessage =
                        $"Ambiguous changed type symbol '{formatSymbolIdentity symbol}' to baseline type token mapping (candidates={candidates}, matches={ambiguousMatches}); full rebuild required."

                    resolvedTypes, errorMessage :: errors
                | TypeNameMissing ->
                    let errorMessage =
                        $"Unable to resolve changed type symbol '{formatSymbolIdentity symbol}' to a baseline type token (candidates={candidates}); full rebuild required."

                    resolvedTypes, errorMessage :: errors)
            ([], [])

    let updatedTypes = updatedTypes |> List.rev |> deduplicate
    let typeResolutionErrors = typeResolutionErrors |> List.rev

    let resolveContainingTypeCandidates (change: UpdatedSymbolChange) =
        let explicitEntity =
            match change.ContainingEntity with
            | Some name -> [ name ]
            | None -> []

        let pathCandidates = change.Symbol |> candidateContainingTypeNames
        let rawCandidates = deduplicate (explicitEntity @ pathCandidates)

        // Keep explicit ambiguity details instead of collapsing into a generic "missing" state.
        // This keeps failure diagnostics deterministic when type-path normalization yields multiple matches.
        let normalizedCandidates, ambiguousCandidates =
            rawCandidates
            |> List.fold
                (fun (resolved, ambiguous) candidate ->
                    match resolveTypeName [ candidate ] with
                    | TypeNameResolved resolvedName -> resolvedName :: resolved, ambiguous
                    | TypeNameAmbiguous matches -> resolved, (candidate, matches) :: ambiguous
                    | TypeNameMissing -> resolved, ambiguous)
                ([], [])

        let normalizedCandidates = normalizedCandidates |> List.rev |> deduplicate
        let ambiguousCandidates = ambiguousCandidates |> List.rev

        match change.ContainingEntity with
        | Some explicitEntity ->
            match resolveTypeName [ explicitEntity ] with
            | TypeNameResolved resolvedExplicit -> Ok [ resolvedExplicit ]
            | TypeNameAmbiguous ambiguousMatches ->
                Error(
                    $"Ambiguous explicit containing entity '{explicitEntity}' for symbol '{formatSymbolIdentity change.Symbol}' to baseline type token mapping (matches={ambiguousMatches}); full rebuild required."
                )
            | TypeNameMissing ->
                Error(
                    $"Unable to resolve explicit containing entity '{explicitEntity}' for symbol '{formatSymbolIdentity change.Symbol}' to a baseline type token; full rebuild required."
                )
        | None ->
            if not (List.isEmpty ambiguousCandidates) then
                let ambiguityDetails =
                    ambiguousCandidates
                    |> List.map (fun (candidate, matches) -> $"{candidate} -> {matches}")
                    |> String.concat "; "

                Error(
                    $"Ambiguous containing type mapping for symbol '{formatSymbolIdentity change.Symbol}' to baseline type tokens (candidates={rawCandidates}, ambiguous={ambiguityDetails}); full rebuild required."
                )
            elif List.isEmpty normalizedCandidates then
                // Compatibility fallback: some baseline method keys may reference declaring
                // types missing from baseline.TypeTokens. Preserve name-based fallback in that case.
                Ok rawCandidates
            elif normalizedCandidates.Length > 1 then
                Error(
                    $"Ambiguous containing type mapping for symbol '{formatSymbolIdentity change.Symbol}' to baseline type tokens (candidates={rawCandidates}, matches={normalizedCandidates}); full rebuild required."
                )
            else
                Ok normalizedCandidates

    let resolveMethodKey (symbol: SymbolId) (resolvedTypeNames: string list) =
        let missingIdentityParts = missingRuntimeSignatureIdentityParts symbol

        if not (List.isEmpty missingIdentityParts) then
            // Fail closed: if we cannot describe the runtime method signature precisely,
            // avoid best-effort token matching that could map edits to the wrong method.
            MethodIdentityMissing missingIdentityParts
        else
            let resolvedTypeTokens =
                resolvedTypeNames
                |> List.choose (fun name -> baseline.TypeTokens |> Map.tryFind name)
                |> deduplicate

            let _, identityMatchedCandidates = lookupMethodsByIdentity symbol resolvedTypeNames

            match identityMatchedCandidates with
            | [ candidate ] -> MethodResolved candidate
            | _ :: _ as ambiguous -> MethodAmbiguous ambiguous
            | [] ->
                let candidates =
                    baseline.MethodTokens
                    |> Map.toSeq
                    |> Seq.choose (fun (key, _) ->
                        let containingTypeMatches =
                            if List.isEmpty resolvedTypeTokens then
                                resolvedTypeNames |> List.exists (typeNamesEquivalent key.DeclaringType)
                            else
                                match baseline.TypeTokens |> Map.tryFind key.DeclaringType with
                                | Some declaringTypeToken -> resolvedTypeTokens |> List.contains declaringTypeToken
                                | None -> false

                        if containingTypeMatches && methodKeyMatchesSymbol symbol key then
                            Some key
                        else
                            None)
                    |> Seq.distinct
                    |> Seq.toList

                match candidates with
                | [] -> MethodMissing
                | _ ->
                    let parameterMatchedCandidates =
                        candidates |> List.filter (methodParameterTypesMatchSymbol symbol)

                    match parameterMatchedCandidates with
                    | [] -> MethodMissing
                    | _ ->
                        // Return type disambiguation mirrors Roslyn's signature equality only after parameter matching.
                        let returnMatchedCandidates =
                            parameterMatchedCandidates |> List.filter (methodReturnTypeMatchesSymbol symbol)

                        match returnMatchedCandidates with
                        | [] -> MethodMissing
                        | [ candidate ] -> MethodResolved candidate
                        | ambiguous -> MethodAmbiguous ambiguous

    let updatedMethods, methodResolutionErrors =
        changes.Updated
        |> List.fold
            (fun (resolvedMethods, errors) change ->
                match change.Kind with
                | SemanticEditKind.MethodBody when change.Symbol.Kind = SymbolKind.Value ->
                    match resolveContainingTypeCandidates change with
                    | Error errorMessage -> resolvedMethods, errorMessage :: errors
                    | Ok candidates ->
                        let resolution = resolveMethodKey change.Symbol candidates

                        if traceMethodResolution then
                            printfn
                                "[fsharp-hotreload][delta-builder] symbol=%s compiledName=%A args=%A genericArity=%A parameterTypes=%A returnType=%A path=%A containingEntity=%A candidates=%A resolution=%A"
                                change.Symbol.LogicalName
                                change.Symbol.CompiledName
                                change.Symbol.TotalArgCount
                                change.Symbol.GenericArity
                                change.Symbol.ParameterTypeIdentities
                                change.Symbol.ReturnTypeIdentity
                                change.Symbol.Path
                                change.ContainingEntity
                                candidates
                                resolution

                        match resolution with
                        | MethodResolved methodKey -> methodKey :: resolvedMethods, errors
                        | MethodIdentityMissing missingParts ->
                            let missingText = String.concat ", " missingParts

                            let errorMessage =
                                $"Unable to resolve changed method symbol '{formatSymbolIdentity change.Symbol}' because runtime signature identity is incomplete (missing: {missingText}); full rebuild required."

                            resolvedMethods, errorMessage :: errors
                        | MethodMissing ->
                            let errorMessage =
                                $"Unable to resolve changed method symbol '{formatSymbolIdentity change.Symbol}' to a unique baseline method token (containingTypeCandidates={candidates}); full rebuild required."

                            resolvedMethods, errorMessage :: errors
                        | MethodAmbiguous ambiguous ->
                            let ambiguousText = ambiguous |> List.map describeMethodKey |> String.concat "; "

                            let errorMessage =
                                $"Ambiguous baseline method mapping for '{formatSymbolIdentity change.Symbol}' (containingTypeCandidates={candidates}, matches=[{ambiguousText}]); full rebuild required."

                            resolvedMethods, errorMessage :: errors
                | _ -> resolvedMethods, errors)
            ([], [])

    let updatedMethods = updatedMethods |> List.rev |> deduplicate
    let methodResolutionErrors = methodResolutionErrors |> List.rev

    // Added module-level values lower to a static backing field plus accessor methods, with
    // initialization appended to the startup-code class constructor
    // (`<StartupCode$asm>.$Path.Module..cctor`). The startup constructor is not a typed-tree
    // binding, so the diff cannot pair its body change when the baseline already contains
    // it, resolve it here so the emitter re-emits the body that now initializes the new
    // value. When the baseline has no startup constructor yet (first value added to the
    // module), the fresh compile introduces it and the emitter discovers it as an added
    // method, so nothing needs to resolve here.
    let startupInitMethods =
        changes.Added
        |> List.filter (fun symbol ->
            symbol.Kind = SymbolKind.Value
            && symbol.MemberKind.IsNone
            && (match symbol.TotalArgCount with
                | Some argCount -> argCount = 0
                | None -> true))
        |> List.collect (fun symbol ->
            // The startup class is per implementation FILE (`$<file qualified name>`),
            // while the added value's symbol path appends every enclosing module
            // segment. Matching every path PREFIX covers both shapes: a value added
            // directly to the file's top-level module (full path = file name, the
            // historical match) and a value inside an ADDED or nested module (the
            // file-name prefix matches the startup class; the deeper segments do not).
            let pathPrefixes =
                [
                    for prefixLength in 1 .. symbol.Path.Length -> joinPath (symbol.Path |> List.truncate prefixLength)
                ]

            baseline.MethodTokens
            |> Map.toSeq
            |> Seq.map fst
            |> Seq.filter (fun key ->
                key.Name = ".cctor"
                && key.DeclaringType.StartsWith("<StartupCode$", StringComparison.Ordinal)
                && pathPrefixes
                   |> List.exists (fun prefix -> key.DeclaringType.EndsWith("$" + prefix, StringComparison.Ordinal)))
            |> Seq.toList)
        |> deduplicate

    let updatedMethods = deduplicate (updatedMethods @ startupInitMethods)

    // ADDED accessors are intentionally absent: the emitter discovers added accessor
    // methods by walking the fresh compile and derives the MethodSemantics rows binding
    // them to their added Property/Event rows from the fresh metadata's accessor
    // relationships, so no baseline method resolution exists (or is needed) for them.
    let accessorCandidates =
        [
            yield!
                FSharpSymbolChanges.propertyAccessorsUpdated changes
                |> List.map (fun change -> change.Symbol, change.ContainingEntity)
            yield!
                FSharpSymbolChanges.propertyAccessorsDeleted changes
                |> List.map (fun symbol -> symbol, None)
            yield!
                FSharpSymbolChanges.eventAccessorsUpdated changes
                |> List.map (fun change -> change.Symbol, change.ContainingEntity)
            yield!
                FSharpSymbolChanges.eventAccessorsDeleted changes
                |> List.map (fun symbol -> symbol, None)
        ]
        |> List.filter (fun (symbol, _) ->
            match symbol.MemberKind with
            | Some SymbolMemberKind.Method -> false
            | Some _ -> true
            | None -> false)
        |> List.fold
            (fun (seen, acc) ((symbol, _) as candidate) ->
                if seen |> Set.contains symbol.Stamp then
                    seen, acc
                else
                    Set.add symbol.Stamp seen, candidate :: acc)
            (Set.empty, [])
        |> snd
        |> List.rev

    let resolveAccessorContainingTypeCandidates (symbol: SymbolId) (explicitContainingEntity: string option) =
        let explicitCandidates = explicitContainingEntity |> Option.toList
        let pathCandidates = symbol |> candidateContainingTypeNames
        let rawCandidates = deduplicate (explicitCandidates @ pathCandidates)

        let normalizedCandidates =
            rawCandidates
            |> List.choose (fun candidate ->
                match resolveTypeName [ candidate ] with
                | TypeNameResolved resolved -> Some resolved
                | _ -> None)
            |> deduplicate

        match explicitContainingEntity with
        | Some explicitEntity ->
            match resolveTypeName [ explicitEntity ] with
            | TypeNameResolved resolved -> Ok [ resolved ]
            | TypeNameAmbiguous ambiguousMatches ->
                Error(
                    $"Ambiguous explicit accessor containing entity '{explicitEntity}' for symbol '{formatSymbolIdentity symbol}' to baseline type token mapping (matches={ambiguousMatches}); full rebuild required."
                )
            | TypeNameMissing ->
                Error(
                    $"Unable to resolve explicit accessor containing entity '{explicitEntity}' for symbol '{formatSymbolIdentity symbol}' to a baseline type token; full rebuild required."
                )
        | None ->
            if List.isEmpty normalizedCandidates then
                Error(
                    $"Unable to resolve accessor containing type for symbol '{formatSymbolIdentity symbol}' (candidates={rawCandidates}); full rebuild required."
                )
            elif normalizedCandidates.Length > 1 then
                Error(
                    $"Ambiguous accessor containing type mapping for symbol '{formatSymbolIdentity symbol}' (candidates={rawCandidates}, matches={normalizedCandidates}); full rebuild required."
                )
            else
                Ok normalizedCandidates

    let accessorUpdates, accessorResolutionErrors =
        accessorCandidates
        |> List.fold
            (fun (resolvedAccessors, errors) (symbol, explicitContainingEntity) ->
                match resolveAccessorContainingTypeCandidates symbol explicitContainingEntity with
                | Error errorMessage -> resolvedAccessors, errorMessage :: errors
                | Ok [] -> resolvedAccessors, errors
                | Ok [ typeName ] ->
                    let method, updatedErrors =
                        match resolveMethodKey symbol [ typeName ] with
                        | MethodResolved methodKey -> Some methodKey, errors
                        | MethodIdentityMissing missingParts ->
                            let missingText = String.concat ", " missingParts

                            let errorMessage =
                                $"Unable to resolve accessor symbol '{formatSymbolIdentity symbol}' because runtime signature identity is incomplete (missing: {missingText}); full rebuild required."

                            None, errorMessage :: errors
                        | MethodMissing ->
                            let errorMessage =
                                $"Unable to resolve accessor symbol '{formatSymbolIdentity symbol}' to a unique baseline method token (type={typeName}); full rebuild required."

                            None, errorMessage :: errors
                        | MethodAmbiguous ambiguous ->
                            let ambiguousText = ambiguous |> List.map describeMethodKey |> String.concat "; "

                            let errorMessage =
                                $"Ambiguous accessor method mapping for '{formatSymbolIdentity symbol}' (type={typeName}, matches=[{ambiguousText}]); full rebuild required."

                            None, errorMessage :: errors

                    {
                        AccessorUpdate.Symbol = symbol
                        ContainingType = typeName
                        MemberKind = symbol.MemberKind.Value
                        Method = method
                    }
                    :: resolvedAccessors,
                    updatedErrors
                | Ok typeNames ->
                    let errorMessage =
                        $"Ambiguous accessor containing type mapping for symbol '{formatSymbolIdentity symbol}' (matches={typeNames}); full rebuild required."

                    resolvedAccessors, errorMessage :: errors)
            ([], [])

    let accessorUpdates = accessorUpdates |> List.rev
    let accessorResolutionErrors = accessorResolutionErrors |> List.rev

    let resolutionErrors =
        typeResolutionErrors @ methodResolutionErrors @ accessorResolutionErrors
        |> deduplicate

    if List.isEmpty resolutionErrors then
        Ok(updatedTypes, updatedMethods, accessorUpdates)
    else
        Error resolutionErrors
