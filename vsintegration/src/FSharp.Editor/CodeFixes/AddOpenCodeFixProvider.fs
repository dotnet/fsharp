// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Generic
open System.Collections.Immutable
open System.Runtime.CompilerServices
open System.Threading

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

open CancellableTasks

[<RequireQualifiedAccess>]
type internal MemberReceiver =
    | Value of FSharpType voption
    | Type of FSharpEntity

type internal ReceiverTypes = ResizeArray<struct (string * FSharpType)>

type internal EntityFinder =
    (ShortIdents option * ShortIdents option * ShortIdents option * ShortIdents) -> (InsertionContextEntity * InsertionContext)[]

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.AddOpen); Shared>]
type internal AddOpenCodeFixProvider [<ImportingConstructor>] (assemblyContentProvider: AssemblyContentProvider) =
    inherit CodeFixProvider()

    let maxOpenSuggestions = 5
    let maxQualifySuggestions = 3

    let systemFirst (name: ReadOnlySpan<char>) (sortText: string) =
        let isInSystem =
            name.Equals("System".AsSpan(), StringComparison.Ordinal)
            || name.StartsWith("System.".AsSpan(), StringComparison.Ordinal)

        (if isInSystem then 0 else 1), sortText

    let openOrder (declaration: string) =
        systemFirst (declaration.AsSpan(declaration.LastIndexOf ' ' + 1)) declaration

    let qualificationOrder (fullName: string) =
        systemFirst (fullName.AsSpan()) fullName

    let fixUnderscoresInMenuText (text: string) = text.Replace("_", "__")

    let qualifySymbolFix (firstIdentSpan: TextSpan) (fullName, qualifier) =
        {
            Name = CodeFix.AddOpen
            Message = fixUnderscoresInMenuText fullName
            Changes = [ TextChange(firstIdentSpan, qualifier) ]
        }

    let openNamespaceFix ctx name declaration multipleNames sourceText =
        let displayText = declaration + (if multipleNames then " (" + name + ")" else "")

        let change =
            OpenDeclarationHelper.getOpenDeclarationChange sourceText ctx declaration

        {
            Name = CodeFix.AddOpen
            Message = fixUnderscoresInMenuText displayText
            Changes = [ change ]
        }

    let declaringEntity (symbol: FSharpSymbol) =
        match symbol with
        | :? FSharpEntity as entity -> entity.DeclaringEntity
        | :? FSharpMemberOrFunctionOrValue as mfv -> mfv.DeclaringEntity
        | _ -> None

    let rec tryFindModuleOrTypeNamedByLeading (leadingIdentCount: int) (nameIdentCount: int) (symbol: FSharpSymbol) =
        match declaringEntity symbol with
        | Some parent when not parent.IsNamespace ->
            if nameIdentCount - 1 = leadingIdentCount then
                ValueSome parent
            else
                tryFindModuleOrTypeNamedByLeading leadingIdentCount (nameIdentCount - 1) parent
        | _ -> ValueNone

    let isCSharpStyleExtension (method: FSharpMemberOrFunctionOrValue) =
        not method.IsExtensionMember && method.HasAttribute<ExtensionAttribute>()

    let tryGetOpenDeclaration (entity: InsertionContextEntity) (symbol: AssemblySymbol) ns =
        match symbol.Symbol, tryFindModuleOrTypeNamedByLeading entity.NamespaceIdentCount symbol.CleanedIdents.Length symbol.Symbol with
        | (:? FSharpMemberOrFunctionOrValue as method), _ when isCSharpStyleExtension method -> ValueNone
        | _, ValueSome opened when opened.IsFSharpModule -> ValueSome $"open {ns}"
        | _, ValueSome opened when opened.GenericParameters.Count > 0 -> ValueNone
        | _, ValueSome _ -> ValueSome $"open type {ns}"
        | _, ValueNone -> ValueSome $"open {ns}"

    let isReachedByName (symbol: AssemblySymbol) =
        match symbol.Symbol with
        | :? FSharpMemberOrFunctionOrValue as mfv -> not mfv.IsExtensionMember
        | _ -> true

    let isQualifiable (entity: InsertionContextEntity) = not (entity.LastIdent.StartsWith "op_")

    let memberReceiverOf (symbol: FSharpSymbol option) =
        match symbol with
        | Some(:? FSharpEntity as entity) when entity.IsNamespace || entity.IsFSharpModule -> ValueNone
        | Some(:? FSharpEntity as entity) -> ValueSome(MemberReceiver.Type entity)
        | Some(:? FSharpMemberOrFunctionOrValue as value) -> ValueSome(MemberReceiver.Value(ValueSome value.FullType))
        | Some(:? FSharpField as field) -> ValueSome(MemberReceiver.Value(ValueSome field.FieldType))
        | Some(:? FSharpParameter as parameter) -> ValueSome(MemberReceiver.Value(ValueSome parameter.Type))
        | Some _ -> ValueSome(MemberReceiver.Value ValueNone)
        | None -> ValueNone

    let tryGetMemberReceiver
        (parseResults: FSharpParseFileResults)
        (checkResults: FSharpCheckFileResults)
        (sourceText: SourceText)
        (longIdent: LongIdent)
        (unresolvedIdentRange: range)
        =
        match ParsedInput.GetRangeOfExprLeftOfDot(unresolvedIdentRange.Start, parseResults.ParseTree) with
        | Some receiverRange when not (Position.posGt receiverRange.End unresolvedIdentRange.Start) ->
            match longIdent |> List.takeWhile (fun ident -> ident.idRange <> unresolvedIdentRange) with
            | [] -> ValueSome(MemberReceiver.Value ValueNone)
            | receiverIdents ->
                let lastIdent = List.last receiverIdents
                let lineText = sourceText.Lines[Line.toZ lastIdent.idRange.EndLine].ToString()

                checkResults.GetSymbolUseAtLocation(
                    lastIdent.idRange.EndLine,
                    lastIdent.idRange.EndColumn,
                    lineText,
                    receiverIdents |> List.map _.idText
                )
                |> Option.map _.Symbol
                |> memberReceiverOf
        | _ -> ValueNone

    let tryGetDefinitionName (ty: FSharpType) =
        let ty = ty.ErasedType

        if ty.HasTypeDefinition then
            ty.TypeDefinition.TryFullName
        else
            None

    let getReceiverTypes (ty: FSharpType) =
        let types = ReceiverTypes()
        let baseTypeNames = HashSet<string>(StringComparer.Ordinal)

        let rec addWithBaseTypes (ty: FSharpType) =
            match tryGetDefinitionName ty with
            | Some name when baseTypeNames.Add name ->
                types.Add(struct (name, ty))
                ty.BaseType |> Option.iter addWithBaseTypes
            | _ -> ()

        addWithBaseTypes ty

        for implemented in ty.AllInterfaces do
            match tryGetDefinitionName implemented with
            | Some name -> types.Add(struct (name, implemented))
            | None -> ()

        types

    let rec typeArgumentMatches (extended: FSharpType) (actual: FSharpType) =
        extended.IsGenericParameter
        || actual.IsGenericParameter
        || match tryGetDefinitionName extended, tryGetDefinitionName actual with
           | Some extendedName, Some actualName -> extendedName = actualName && typeArgumentsMatch extended actual
           | None, None -> true
           | _ -> false

    and typeArgumentsMatch (extended: FSharpType) (actual: FSharpType) =
        let extendedArguments = extended.ErasedType.GenericArguments
        let actualArguments = actual.ErasedType.GenericArguments

        extendedArguments.Count = actualArguments.Count
        && Seq.forall2 typeArgumentMatches extendedArguments actualArguments

    let receiverTypesInclude (receiverTypes: ReceiverTypes) (extended: FSharpType) =
        match tryGetDefinitionName extended with
        | Some extendedName -> receiverTypes.Exists(fun struct (name, ty) -> name = extendedName && typeArgumentsMatch extended ty)
        | None -> true

    let fsharpExtensionExtendsOneOf (receiverTypes: ReceiverTypes) (extension: FSharpMemberOrFunctionOrValue) =
        match extension.ApparentEnclosingEntity |> Option.bind _.TryFullName with
        | Some extendedName -> receiverTypes.Exists(fun struct (name, _) -> name = extendedName)
        | None -> true

    let csharpExtensionExtendsOneOf (receiverTypes: ReceiverTypes) (extension: FSharpMemberOrFunctionOrValue) =
        match extension.CurriedParameterGroups |> Seq.tryHead |> Option.bind Seq.tryHead with
        | Some thisParameter when thisParameter.Type.IsGenericParameter ->
            thisParameter.Type.GenericParameter.Constraints
            |> Seq.forall (fun typeConstraint ->
                not typeConstraint.IsCoercesToConstraint
                || typeConstraint.CoercesToTarget.IsGenericParameter
                || receiverTypesInclude receiverTypes typeConstraint.CoercesToTarget)
        | Some thisParameter -> receiverTypesInclude receiverTypes thisParameter.Type
        | None -> true

    let extendsOneOf (receiverTypes: ReceiverTypes voption) (extension: FSharpMemberOrFunctionOrValue) =
        match receiverTypes with
        | ValueNone -> true
        | ValueSome types when extension.IsExtensionMember -> fsharpExtensionExtendsOneOf types extension
        | ValueSome types -> csharpExtensionExtendsOneOf types extension

    let moduleExtensionMembers =
        ConditionalWeakTable<FSharpEntity, FSharpMemberOrFunctionOrValue[]>()

    // Asked of every module in reach on each failed member access, so cached per module.
    let getExtensionMembers (cancellationToken: CancellationToken) (moduleEntity: FSharpEntity) =
        moduleExtensionMembers.GetValue(
            moduleEntity,
            fun moduleEntity ->
                moduleEntity.TryGetMembersFunctionsAndValues()
                |> Seq.filter (fun mfv ->
                    cancellationToken.ThrowIfCancellationRequested()

                    mfv.IsExtensionMember
                    && not mfv.IsPropertyGetterMethod
                    && not mfv.IsPropertySetterMethod)
                |> Seq.toArray
        )

    let getModuleOpenTargets (moduleSymbol: AssemblySymbol) (moduleEntity: FSharpEntity) =
        if moduleEntity.HasAttribute<RequireQualifiedAccessAttribute>() then
            []
        else
            [
                if moduleEntity.HasAttribute<AutoOpenAttribute>() then
                    let outermostAutoOpen =
                        moduleSymbol.AutoOpenParent |> Option.defaultValue moduleSymbol.CleanedIdents

                    if outermostAutoOpen.Length > 1 then
                        outermostAutoOpen[.. outermostAutoOpen.Length - 2]

                moduleSymbol.CleanedIdents
            ]

    let getExtensionOpenTargets
        (checkResults: FSharpCheckFileResults)
        (receiver: MemberReceiver)
        (name: string)
        (symbols: AssemblySymbol[])
        (cancellationToken: CancellationToken)
        =
        let receiverTypes =
            match receiver with
            | MemberReceiver.Value(ValueSome ty) -> ValueSome(getReceiverTypes ty)
            | MemberReceiver.Value ValueNone -> ValueNone
            | MemberReceiver.Type entity -> ValueSome(getReceiverTypes (entity.AsType()))

        let isNamed (symbol: FSharpSymbol) = symbol.DisplayNameCore = name

        let accessibilityRights = checkResults.ProjectContext.AccessibilityRights

        let appliesToReceiver (extension: FSharpMemberOrFunctionOrValue) =
            extension.IsInstanceMember = receiver.IsValue
            && extension.IsAccessible(accessibilityRights)
            && extendsOneOf receiverTypes extension

        let anyOverloadExtends (method: FSharpMemberOrFunctionOrValue) =
            match receiverTypes, method.DeclaringEntity with
            | ValueSome _, Some staticClass ->
                staticClass.TryGetMembersFunctionsAndValues()
                |> Seq.exists (fun overload ->
                    cancellationToken.ThrowIfCancellationRequested()

                    isNamed overload
                    && isCSharpStyleExtension overload
                    && extendsOneOf receiverTypes overload)
            | _ -> extendsOneOf receiverTypes method

        [|
            for symbol in symbols do
                cancellationToken.ThrowIfCancellationRequested()

                match symbol.Symbol with
                | :? FSharpMemberOrFunctionOrValue as method when
                    receiver.IsValue
                    && Array.last symbol.CleanedIdents = name
                    && isCSharpStyleExtension method
                    && anyOverloadExtends method
                    ->
                    match symbol.Namespace with
                    | Some ns -> struct (symbol, ns)
                    | None -> ()
                | :? FSharpEntity as moduleEntity when
                    moduleEntity.IsFSharpModule
                    && getExtensionMembers cancellationToken moduleEntity
                       |> Array.exists (fun extension -> isNamed extension && appliesToReceiver extension)
                    ->
                    for target in getModuleOpenTargets symbol moduleEntity do
                        struct (symbol, target)
                | _ -> ()
        |]

    let getExtensionOpens
        (checkResults: FSharpCheckFileResults)
        (receiver: MemberReceiver)
        (name: string)
        (symbols: AssemblySymbol[])
        (cancellationToken: CancellationToken)
        (findEntities: EntityFinder)
        =
        [
            for struct (symbol, target) in getExtensionOpenTargets checkResults receiver name symbols cancellationToken do
                for entity, ctx in findEntities (None, None, symbol.Namespace, Array.append target [| name |]) do
                    match entity.Namespace with
                    | Some ns -> $"open {ns}", entity.FullDisplayName, ctx
                    | None -> ()
        ]

    let nameIdentsOf (symbol: AssemblySymbol) isAttribute =
        seq {
            yield symbol.CleanedIdents

            if isAttribute then
                let lastIdent = Array.last symbol.CleanedIdents

                if
                    lastIdent.EndsWith "Attribute"
                    && symbol.Kind LookupType.Precise = EntityKind.Attribute
                then
                    yield
                        symbol.CleanedIdents
                        |> Array.replace (symbol.CleanedIdents.Length - 1) (lastIdent.Substring(0, lastIdent.Length - 9))
        }

    let getNameCandidates (symbols: AssemblySymbol[]) isAttribute (findEntities: EntityFinder) =
        symbols
        |> Seq.filter isReachedByName
        |> Seq.collect (fun symbol ->
            nameIdentsOf symbol isAttribute
            |> Seq.collect (fun idents ->
                findEntities (symbol.TopRequireQualifiedAccessParent, symbol.AutoOpenParent, symbol.Namespace, idents)
                |> Seq.map (fun (entity, ctx) -> entity, ctx, symbol)))
        |> Seq.toList

    let toMaybeUnresolvedIdents (unresolvedIdentRange: range) (longIdent: LongIdent) =
        longIdent
        |> List.map (fun ident ->
            {
                Ident = ident.idText
                Resolved = not (ident.idRange = unresolvedIdentRange)
            })
        |> List.toArray

    let getSuggestionsAsCodeFixes
        (firstIdentSpan: TextSpan)
        (sourceText: SourceText)
        (candidates: (InsertionContextEntity * InsertionContext * AssemblySymbol) list)
        (extensionOpens: (string * string * InsertionContext) list)
        =
        let nameOpens =
            candidates
            |> Seq.chooseV (fun (entity, ctx, symbol) ->
                match entity.Namespace with
                | Some ns ->
                    tryGetOpenDeclaration entity symbol ns
                    |> ValueOption.map (fun declaration -> declaration, entity.FullDisplayName, ctx)
                | None -> ValueNone)

        seq {
            Seq.append nameOpens extensionOpens
            |> Seq.groupBy (fun (declaration, _, _) -> declaration)
            |> Seq.map (fun (declaration, xs) ->
                declaration,
                xs
                |> Seq.map (fun (_, name, ctx) -> name, ctx)
                |> Seq.distinctBy (fun (name, _) -> name)
                |> Seq.sortBy fst
                |> Seq.toArray)
            |> Seq.sortBy (fst >> openOrder)
            |> Seq.map (fun (declaration, names) ->
                let multipleNames = names |> Array.length > 1
                names |> Seq.map (fun (name, ctx) -> declaration, name, ctx, multipleNames))
            |> Seq.concat
            |> Seq.truncate maxOpenSuggestions
            |> Seq.map (fun (declaration, name, ctx, multipleNames) -> openNamespaceFix ctx name declaration multipleNames sourceText)

            candidates
            |> Seq.filter (fun (entity, _, _) -> isQualifiable entity)
            |> Seq.map (fun (entity, _, _) -> entity.FullRelativeName, entity.Qualifier)
            |> Seq.distinct
            |> Seq.sortBy (fst >> qualificationOrder)
            |> Seq.truncate maxQualifySuggestions
            |> Seq.map (qualifySymbolFix firstIdentSpan)

        }
        |> Seq.concat

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0039", "FS0043")

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFixes this

    interface IFSharpMultiCodeFixProvider with
        member _.GetCodeFixesAsync context =
            cancellableTask {
                let document = context.Document

                let! sourceText = context.GetSourceTextAsync()

                let! parseResults, checkResults = document.GetFSharpParseAndCheckResultsAsync(nameof AddOpenCodeFixProvider)

                let line = sourceText.Lines.GetLineFromPosition(context.Span.End)
                let linePos = sourceText.Lines.GetLinePosition(context.Span.End)

                let! defines, langVersion = document.GetFsharpParsingOptionsAsync(nameof AddOpenCodeFixProvider)

                return
                    Tokenizer.getSymbolAtPosition (
                        document.Id,
                        sourceText,
                        context.Span.End,
                        document.FilePath,
                        defines,
                        SymbolLookupKind.Greedy,
                        false,
                        false,
                        Some langVersion,
                        context.CancellationToken
                    )
                    |> Option.filter (fun lexerSymbol ->
                        let symbolOpt =
                            checkResults.GetSymbolUseAtLocation(
                                Line.fromZ linePos.Line,
                                lexerSymbol.Ident.idRange.EndColumn,
                                line.ToString(),
                                lexerSymbol.FullIsland
                            )

                        match symbolOpt with
                        | None -> true
                        // this is for operators for FS0043
                        | Some symbol when PrettyNaming.IsLogicalOpName symbol.Symbol.DisplayName -> true
                        | _ -> false)
                    |> Option.bind (fun _ ->
                        let unresolvedIdentRange =
                            let startLinePos = sourceText.Lines.GetLinePosition context.Span.Start
                            let startPos = Position.fromZ startLinePos.Line startLinePos.Character
                            let endLinePos = sourceText.Lines.GetLinePosition context.Span.End
                            let endPos = Position.fromZ endLinePos.Line endLinePos.Character
                            Range.mkRange context.Document.FilePath startPos endPos

                        let isAttribute =
                            ParsedInput.GetEntityKind(unresolvedIdentRange.Start, parseResults.ParseTree) = Some EntityKind.Attribute

                        let symbols =
                            assemblyContentProvider.GetAllEntitiesInProjectAndReferencedAssemblies checkResults

                        ParsedInput.GetLongIdentAt parseResults.ParseTree unresolvedIdentRange.End
                        |> Option.map (fun longIdent ->
                            let insertionPoint =
                                if document.Project.IsFSharpCodeFixesAlwaysPlaceOpensAtTopLevelEnabled then
                                    OpenStatementInsertionPoint.TopLevel
                                else
                                    OpenStatementInsertionPoint.Nearest

                            let createEntity (maybeUnresolvedIdents: MaybeUnresolvedIdent[]) : EntityFinder =
                                ParsedInput.TryFindInsertionContext
                                    unresolvedIdentRange.StartLine
                                    parseResults.ParseTree
                                    maybeUnresolvedIdents
                                    insertionPoint

                            let receiver =
                                tryGetMemberReceiver parseResults checkResults sourceText longIdent unresolvedIdentRange

                            let candidates =
                                match receiver with
                                | ValueSome(MemberReceiver.Value _) -> []
                                | _ ->
                                    createEntity (toMaybeUnresolvedIdents unresolvedIdentRange longIdent)
                                    |> getNameCandidates symbols isAttribute

                            let extensionOpens =
                                match receiver, longIdent |> List.tryFind (fun ident -> ident.idRange = unresolvedIdentRange) with
                                | ValueSome receiver, Some unresolvedIdent ->
                                    let name = unresolvedIdent.idText

                                    createEntity [| { Ident = name; Resolved = false } |]
                                    |> getExtensionOpens checkResults receiver name symbols context.CancellationToken
                                | _ -> []

                            getSuggestionsAsCodeFixes
                                (RoslynHelpers.FSharpRangeToTextSpan(sourceText, longIdent.Head.idRange))
                                sourceText
                                candidates
                                extensionOpens))
                    |> Option.defaultValue Seq.empty
            }
