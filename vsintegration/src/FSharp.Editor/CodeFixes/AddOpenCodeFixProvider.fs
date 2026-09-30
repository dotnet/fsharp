// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Generic
open System.Collections.Immutable
open System.Runtime.CompilerServices

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

open CancellableTasks

/// What the unresolved name is written as a member of. Only an extension member can supply it then.
[<RequireQualifiedAccess>]
type internal MemberReceiver =
    /// `x.Name` or `f().Name`: an instance member of a value, of this type when it is known.
    | Value of FSharpType voption
    /// `T.Name`: a static member of a type.
    | Type of FSharpEntity

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.AddOpen); Shared>]
type internal AddOpenCodeFixProvider [<ImportingConstructor>] (assemblyContentProvider: AssemblyContentProvider) =
    inherit CodeFixProvider()

    // A name can be reachable from a great many places, and the lightbulb is a menu a person reads.
    // The same reason Roslyn's add-import fix stops at five suggestions and its fully-qualify at three.
    let maxOpenSuggestions = 5
    let maxQualifySuggestions = 3

    // Which assembly the entity crawler reached first is no order to offer suggestions in. Sort them
    // the way Roslyn's add-import fix does: what `System` holds first, the rest alphabetically after.
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

    // The qualifier stands in for the first identifier written, whichever one the diagnostic is on:
    // choosing `N.M.T` for `M.T` rewrites `M`, not `T`.
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

    // Below its namespace, each ident of a symbol's name names the module or type the next one is
    // declared in. Finds what the first `count` of `identCount` idents name, unless that is a namespace.
    let rec tryFindEntityNamedBy count identCount (symbol: FSharpSymbol) =
        match declaringEntity symbol with
        | Some parent when not parent.IsNamespace ->
            if identCount - 1 = count then
                ValueSome parent
            else
                tryFindEntityNamedBy count (identCount - 1) parent
        | _ -> ValueNone

    let isCSharpStyleExtension (method: FSharpMemberOrFunctionOrValue) =
        not method.IsExtensionMember && method.HasAttribute<ExtensionAttribute>()

    // A plain `open` reaches namespaces and F# modules. When the name is opened deeper than that - a
    // type nested in a type, or a static member of one - the type itself has to be opened, and a generic
    // one cannot be: the type arguments `open type` needs are not part of the name.
    let tryGetOpenDeclaration (entity: InsertionContextEntity) (symbol: AssemblySymbol) ns =
        match symbol.Symbol with
        // Opening its class does not make a C#-style extension method a name of its own: F# calls one
        // on the value it extends, or qualified by the class.
        | :? FSharpMemberOrFunctionOrValue as method when isCSharpStyleExtension method -> ValueNone
        | _ ->
            match tryFindEntityNamedBy entity.NamespaceIdentCount symbol.CleanedIdents.Length symbol.Symbol with
            | ValueSome opened when not opened.IsFSharpModule ->
                if opened.GenericParameters.Count > 0 then
                    ValueNone
                else
                    ValueSome $"open type {ns}"
            | _ -> ValueSome $"open {ns}"

    // An F# extension member is reached through the value or type it extends, never by a name of its own.
    let isReachedByName (symbol: AssemblySymbol) =
        match symbol.Symbol with
        | :? FSharpMemberOrFunctionOrValue as mfv -> not mfv.IsExtensionMember
        | _ -> true

    // Whether the unresolved name is written as a member of something, and of what. A bare name, or one
    // qualified by a namespace or module, is not.
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

                let receiver =
                    checkResults.GetSymbolUseAtLocation(
                        lastIdent.idRange.EndLine,
                        lastIdent.idRange.EndColumn,
                        lineText,
                        receiverIdents |> List.map _.idText
                    )

                match receiver |> Option.map _.Symbol with
                | Some(:? FSharpEntity as entity) when entity.IsNamespace || entity.IsFSharpModule -> ValueNone
                | Some(:? FSharpEntity as entity) -> ValueSome(MemberReceiver.Type entity)
                | Some(:? FSharpMemberOrFunctionOrValue as value) -> ValueSome(MemberReceiver.Value(ValueSome value.FullType))
                | Some(:? FSharpField as field) -> ValueSome(MemberReceiver.Value(ValueSome field.FieldType))
                | Some(:? FSharpParameter as parameter) -> ValueSome(MemberReceiver.Value(ValueSome parameter.Type))
                | Some _ -> ValueSome(MemberReceiver.Value ValueNone)
                | None -> ValueNone
        | _ -> ValueNone

    let tryGetDefinitionName (ty: FSharpType) =
        let ty = ty.ErasedType

        if ty.HasTypeDefinition then
            ty.TypeDefinition.TryFullName
        else
            None

    // What an extension member has to extend to apply to a value of this type: the type itself, one of
    // its base types or one of its interfaces.
    let getExtensibleDefinitionNames (ty: FSharpType) =
        let names = HashSet<string>(StringComparer.Ordinal)

        let rec addWithBaseTypes (ty: FSharpType) =
            match tryGetDefinitionName ty with
            | Some name when names.Add name -> ty.BaseType |> Option.iter addWithBaseTypes
            | _ -> ()

        addWithBaseTypes ty

        for implemented in ty.AllInterfaces do
            tryGetDefinitionName implemented
            |> Option.iter (fun name -> names.Add name |> ignore)

        names

    // The type an extension member extends: the one an F# member is declared on, or the first parameter
    // of a C#-style method. None when that parameter is a type parameter, which every type satisfies.
    let tryGetExtendedDefinitionName (extension: FSharpMemberOrFunctionOrValue) =
        if extension.IsExtensionMember then
            extension.ApparentEnclosingEntity |> Option.bind _.TryFullName
        else
            match extension.CurriedParameterGroups |> Seq.tryHead |> Option.bind Seq.tryHead with
            | Some thisParameter when not thisParameter.Type.IsGenericParameter -> tryGetDefinitionName thisParameter.Type
            | _ -> None

    let extendsAny (extensibleNames: HashSet<string> voption) (extension: FSharpMemberOrFunctionOrValue) =
        match extensibleNames, tryGetExtendedDefinitionName extension with
        | ValueSome names, Some extended -> names.Contains extended
        | _ -> true

    let moduleExtensionMembers =
        ConditionalWeakTable<FSharpEntity, FSharpMemberOrFunctionOrValue[]>()

    // Asked of every module in reach whenever a member access fails, so kept per module: the modules of
    // referenced assemblies are cached along with them, for as long as the assembly does not change.
    let getExtensionMembers (moduleEntity: FSharpEntity) =
        moduleExtensionMembers.GetValue(
            moduleEntity,
            fun moduleEntity ->
                moduleEntity.TryGetMembersFunctionsAndValues()
                |> Seq.filter (fun mfv ->
                    mfv.IsExtensionMember
                    && not mfv.IsPropertyGetterMethod
                    && not mfv.IsPropertySetterMethod)
                |> Seq.toArray
        )

    // What to open to bring the extension members a module declares into scope: the module itself and,
    // when it is [<AutoOpen>], also what opens it. One that requires qualified access cannot be opened.
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

    // The namespaces and modules that hold an extension member of this name for the receiver: that of a
    // C#-style method's static class, or the module an F# one is declared in and what opens that module.
    let getExtensionOpenTargets
        (checkResults: FSharpCheckFileResults)
        (receiver: MemberReceiver)
        (name: string)
        (symbols: AssemblySymbol[])
        =
        let extensibleNames =
            match receiver with
            | MemberReceiver.Value(ValueSome ty) -> ValueSome(getExtensibleDefinitionNames ty)
            | MemberReceiver.Value ValueNone -> ValueNone
            | MemberReceiver.Type entity -> ValueSome(getExtensibleDefinitionNames (entity.AsType()))

        let isNamed (symbol: FSharpSymbol) =
            String.Equals(symbol.DisplayNameCore, name, StringComparison.Ordinal)

        let accessibilityRights = checkResults.ProjectContext.AccessibilityRights

        let appliesToReceiver (extension: FSharpMemberOrFunctionOrValue) =
            extension.IsInstanceMember = receiver.IsValue
            && extension.IsAccessible(accessibilityRights)
            && extendsAny extensibleNames extension

        // The assembly content lists one of a class's overloads, and they can extend different types.
        let anyOverloadExtends (method: FSharpMemberOrFunctionOrValue) =
            match extensibleNames, method.DeclaringEntity with
            | ValueSome _, Some staticClass ->
                staticClass.TryGetMembersFunctionsAndValues()
                |> Seq.exists (fun overload ->
                    isNamed overload
                    && isCSharpStyleExtension overload
                    && extendsAny extensibleNames overload)
            | _ -> extendsAny extensibleNames method

        [|
            for symbol in symbols do
                match symbol.Symbol with
                | :? FSharpMemberOrFunctionOrValue as method when
                    receiver.IsValue
                    && String.Equals(Array.last symbol.CleanedIdents, name, StringComparison.Ordinal)
                    && isCSharpStyleExtension method
                    && anyOverloadExtends method
                    ->
                    match symbol.Namespace with
                    | Some ns -> struct (symbol, ns)
                    | None -> ()
                | :? FSharpEntity as moduleEntity when
                    moduleEntity.IsFSharpModule
                    && getExtensionMembers moduleEntity
                       |> Array.exists (fun extension -> isNamed extension && appliesToReceiver extension)
                    ->
                    for target in getModuleOpenTargets symbol moduleEntity do
                        struct (symbol, target)
                | _ -> ()
        |]

    let getSuggestionsAsCodeFixes
        (firstIdentSpan: TextSpan)
        (sourceText: SourceText)
        (opens: (string * string * InsertionContext) seq)
        (qualifications: (string * string) seq)
        =
        seq {
            opens
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

            qualifications
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

                            let receiver =
                                tryGetMemberReceiver parseResults checkResults sourceText longIdent unresolvedIdentRange

                            // A name written as a member of a value can only be an extension member.
                            let candidates =
                                match receiver with
                                | ValueSome(MemberReceiver.Value _) -> []
                                | _ ->
                                    let maybeUnresolvedIdents =
                                        longIdent
                                        |> List.map (fun ident ->
                                            {
                                                Ident = ident.idText
                                                Resolved = not (ident.idRange = unresolvedIdentRange)
                                            })
                                        |> List.toArray

                                    let createEntity =
                                        ParsedInput.TryFindInsertionContext
                                            unresolvedIdentRange.StartLine
                                            parseResults.ParseTree
                                            maybeUnresolvedIdents
                                            insertionPoint

                                    [
                                        for s in symbols do
                                            if isReachedByName s then
                                                let candidate =
                                                    s.TopRequireQualifiedAccessParent, s.AutoOpenParent, s.Namespace, s.CleanedIdents

                                                for entity, ctx in createEntity candidate do
                                                    entity, ctx, s

                                                if isAttribute then
                                                    let lastIdent = s.CleanedIdents.[s.CleanedIdents.Length - 1]

                                                    if
                                                        lastIdent.EndsWith "Attribute"
                                                        && s.Kind LookupType.Precise = EntityKind.Attribute
                                                    then
                                                        let attributeCandidate =
                                                            s.TopRequireQualifiedAccessParent,
                                                            s.AutoOpenParent,
                                                            s.Namespace,
                                                            s.CleanedIdents
                                                            |> Array.replace
                                                                (s.CleanedIdents.Length - 1)
                                                                (lastIdent.Substring(0, lastIdent.Length - 9))

                                                        for entity, ctx in createEntity attributeCandidate do
                                                            entity, ctx, s
                                    ]

                            let extensionOpens =
                                match receiver, longIdent |> List.tryFind (fun ident -> ident.idRange = unresolvedIdentRange) with
                                | ValueSome receiver, Some unresolvedIdent ->
                                    let name = unresolvedIdent.idText

                                    let createEntity =
                                        ParsedInput.TryFindInsertionContext
                                            unresolvedIdentRange.StartLine
                                            parseResults.ParseTree
                                            [| { Ident = name; Resolved = false } |]
                                            insertionPoint

                                    [
                                        for struct (symbol, target) in getExtensionOpenTargets checkResults receiver name symbols do
                                            for entity, ctx in createEntity (None, None, symbol.Namespace, Array.append target [| name |]) do
                                                match entity.Namespace with
                                                | Some ns -> $"open {ns}", entity.FullDisplayName, ctx
                                                | None -> ()
                                    ]
                                | _ -> []

                            let opens =
                                seq {
                                    for entity, ctx, symbol in candidates do
                                        match entity.Namespace with
                                        | Some ns ->
                                            match tryGetOpenDeclaration entity symbol ns with
                                            | ValueSome declaration -> declaration, entity.FullDisplayName, ctx
                                            | ValueNone -> ()
                                        | None -> ()

                                    yield! extensionOpens
                                }

                            let qualifications =
                                candidates
                                |> Seq.filter (fun (entity, _, _) -> not (entity.LastIdent.StartsWith "op_")) // Don't include qualified operator names. The resultant codefix won't compile because it won't be an infix operator anymore.
                                |> Seq.map (fun (entity, _, _) -> entity.FullRelativeName, entity.Qualifier)

                            getSuggestionsAsCodeFixes
                                (RoslynHelpers.FSharpRangeToTextSpan(sourceText, longIdent.Head.idRange))
                                sourceText
                                opens
                                qualifications))

                    |> Option.defaultValue Seq.empty
            }
