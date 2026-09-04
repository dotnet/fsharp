// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.ComponentModel
open System.IO

open Microsoft.CodeAnalysis

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols

/// Turns the F# symbol graph into the immutable item arrays an object list hands to the shell.
/// Every symbol access is guarded: an entity imported from a broken reference throws rather than
/// reporting itself unresolved, and one bad entity must not take the whole tree down.
[<RequireQualifiedAccess>]
module internal ObjectBrowserItems =

    /// FCS answers in 'T option; the tree speaks voption, so the conversion lives here at the boundary.
    let private tryGet (f: unit -> 'T option) =
        try
            match f () with
            | Some value -> ValueSome value
            | None -> ValueNone
        with _ ->
            ValueNone

    let private tryValue (f: unit -> 'T) =
        try
            ValueSome(f ())
        with _ ->
            ValueNone

    let private items (f: unit -> #seq<'T>) : seq<'T> =
        try
            f () :> seq<'T>
        with _ ->
            Seq.empty

    let private namespaceOf (entity: FSharpEntity) = tryGet (fun () -> entity.Namespace)

    let private accessibilityOf (symbol: FSharpSymbol) =
        tryValue (fun () -> symbol.Accessibility)

    let private isEditorHidden (attributes: seq<FSharpAttribute>) =
        try
            attributes
            |> Seq.exists (fun attr ->
                match attr.AttributeType.TryFullName with
                | Some "System.ComponentModel.EditorBrowsableAttribute" ->
                    attr.ConstructorArguments
                    |> Seq.exists (fun (_, value) ->
                        match value with
                        | :? int as state -> state = int EditorBrowsableState.Never
                        | _ -> false)
                | _ -> false)
        with _ ->
            false

    let private tryTypeKind (entity: FSharpEntity) =
        try
            if
                entity.IsUnresolved
                || entity.IsNamespace
                || entity.IsArrayType
                || entity.IsByRef
                || entity.IsMeasure
            then
                ValueNone
            elif entity.IsInterface then
                ValueSome ObjectTypeKind.Interface
            elif entity.IsEnum then
                ValueSome ObjectTypeKind.Enum
            elif entity.IsDelegate then
                ValueSome ObjectTypeKind.Delegate
            elif entity.IsFSharpExceptionDeclaration then
                ValueSome ObjectTypeKind.Exception
            elif entity.IsFSharpModule then
                ValueSome ObjectTypeKind.Module
            elif entity.IsValueType then
                ValueSome ObjectTypeKind.Struct
            else
                ValueSome ObjectTypeKind.Class
        with _ ->
            ValueNone

    /// Object Browser lists nested types alongside their containers, qualified by them: `Outer.Inner`.
    let private classNameOf (entity: FSharpEntity) =
        match tryGet (fun () -> entity.TryFullName) with
        | ValueSome fullName ->
            match namespaceOf entity with
            // Checking the separator by index keeps `ns + "."` from being built for every entity.
            | ValueSome ns when
                fullName.Length > ns.Length
                && fullName[ns.Length] = '.'
                && fullName.StartsWith(ns, StringComparison.Ordinal)
                ->
                fullName.Substring(ns.Length + 1)
            | _ -> fullName
        | ValueNone -> entity.DisplayName

    let private qualify ns name =
        match ns with
        | ValueSome ns -> $"%s{ns}.%s{name}"
        | ValueNone -> name

    let private formatType (fsharpType: FSharpType) =
        try
            fsharpType.Format FSharpDisplayContext.Empty
        with _ ->
            ""

    let private withSignature name signature =
        if String.IsNullOrEmpty signature then
            name
        else
            $"%s{name}: %s{signature}"

    // Entities

    let private isNamespace (entity: FSharpEntity) =
        try
            entity.IsNamespace
        with _ ->
            true

    let private flatten (entity: FSharpEntity) =
        let rec collect (entity: FSharpEntity) =
            seq {
                if not (isNamespace entity) then
                    entity

                for nested in items (fun () -> entity.NestedEntities) do
                    yield! collect nested
            }

        collect entity

    let typesOfSignature (signature: FSharpAssemblySignature) =
        items (fun () -> signature.Entities)
        |> Seq.collect flatten
        |> Seq.filter (fun entity -> (tryTypeKind entity).IsSome)
        |> Seq.toArray

    let typesOfAssembly (assembly: FSharpAssembly) =
        match tryValue (fun () -> assembly.Contents) with
        | ValueSome contents -> typesOfSignature contents
        | ValueNone -> Array.empty

    // Items

    let private typeItem projectId library (entity: FSharpEntity) kind =
        let ns = namespaceOf entity
        let className = classNameOf entity
        let access = accessibilityOf entity

        {
            Data = ObjectItemData.Type(entity, kind)
            DisplayText = className
            FullName = qualify ns className
            GlyphIndex = ObjectBrowserGlyph.forType kind access
            IsHidden = isEditorHidden (items (fun () -> entity.Attributes))
            Accessibility = access
            NavPath =
                ValueSome
                    {
                        Library = library
                        Namespace = ns
                        Class = ValueSome className
                        Member = ValueNone
                    }
            ProjectId = projectId
        }

    /// The container facts (namespace, class name, qualified container) are entity-level and are
    /// computed once per entity, not once per member.
    let private memberItem projectId library ns className container (symbol: FSharpSymbol) kind isInherited displayText =
        let access = accessibilityOf symbol

        {
            Data = ObjectItemData.Member(symbol, kind, isInherited)
            DisplayText = displayText
            FullName = $"%s{container}.%s{displayText}"
            GlyphIndex = ObjectBrowserGlyph.forMember kind access
            IsHidden = false
            Accessibility = access
            NavPath =
                ValueSome
                    {
                        Library = library
                        Namespace = ns
                        Class = ValueSome className
                        Member = ValueSome displayText
                    }
            ProjectId = projectId
        }

    let private namespaceItem projectId library name (types: FSharpEntity[]) =
        {
            Data = ObjectItemData.Namespace types
            DisplayText = name
            FullName = name
            GlyphIndex = ObjectBrowserGlyph.forNamespace
            IsHidden = false
            Accessibility = ValueNone
            NavPath =
                ValueSome
                    {
                        Library = library
                        Namespace = ValueSome name
                        Class = ValueNone
                        Member = ValueNone
                    }
            ProjectId = projectId
        }

    let projectItem projectId name =
        {
            Data = ObjectItemData.Project
            DisplayText = name
            FullName = name
            GlyphIndex = ObjectBrowserGlyph.forProject
            IsHidden = false
            Accessibility = ValueNone
            NavPath =
                ValueSome
                    {
                        Library = name
                        Namespace = ValueNone
                        Class = ValueNone
                        Member = ValueNone
                    }
            ProjectId = projectId
        }

    let referenceItem projectId name (path: string voption) =
        let library =
            match path with
            | ValueSome path -> path
            | ValueNone -> name

        {
            Data = ObjectItemData.Reference(name, path)
            DisplayText = name
            FullName = name
            GlyphIndex = ObjectBrowserGlyph.forAssembly
            IsHidden = false
            Accessibility = ValueNone
            NavPath =
                ValueSome
                    {
                        Library = library
                        Namespace = ValueNone
                        Class = ValueNone
                        Member = ValueNone
                    }
            ProjectId = projectId
        }

    /// C#/VB read reference rows from the Roslyn project's `MetadataReferences`, which costs nothing.
    /// F# only populates those for the legacy project system — for SDK projects the reference set
    /// lives in the `-r:` flags of the project options, which are just as cheap to read and, unlike
    /// `ParseAndCheckProject`, already cached by the editor.
    let referenceItemsOfOptions projectId (options: FSharpProjectOptions) =
        seq {
            for option in options.OtherOptions do
                if option.StartsWith("-r:", StringComparison.Ordinal) then
                    let path = option.Substring 3

                    if not (String.IsNullOrWhiteSpace path) then
                        referenceItem projectId (Path.GetFileNameWithoutExtension path) (ValueSome path)
        }
        |> Seq.sortBy _.DisplayText
        |> Seq.toArray

    let tryFindReferencedAssembly (symbols: ProjectSymbols) (name: string) =
        let matches (assembly: FSharpAssembly) =
            match tryValue (fun () -> assembly.SimpleName) with
            | ValueSome simpleName -> String.Equals(simpleName, name, StringComparison.OrdinalIgnoreCase)
            | ValueNone -> false

        match symbols.ReferencedAssemblies |> Array.tryFind matches with
        | Some assembly -> ValueSome assembly
        | None -> ValueNone

    let folderItem projectId name childKind =
        {
            Data = ObjectItemData.Folder childKind
            DisplayText = name
            FullName = name
            GlyphIndex = ObjectBrowserGlyph.forFolder
            IsHidden = false
            Accessibility = ValueNone
            NavPath = ValueNone
            ProjectId = projectId
        }

    let pendingItem projectId =
        let text = SR.ObjectBrowserPending()

        {
            Data = ObjectItemData.Pending
            DisplayText = text
            FullName = text
            GlyphIndex = ObjectBrowserGlyph.forPending
            IsHidden = false
            Accessibility = ValueNone
            NavPath = ValueNone
            ProjectId = projectId
        }

    // Lists

    let typeItems projectId library (types: FSharpEntity seq) =
        seq {
            for entity in types do
                match tryTypeKind entity with
                | ValueSome kind -> typeItem projectId library entity kind
                | ValueNone -> ()
        }
        |> Seq.sortBy _.DisplayText
        |> Seq.toArray

    let namespaceItems projectId library (types: FSharpEntity[]) =
        seq {
            for entity in types do
                match namespaceOf entity with
                | ValueSome ns -> ns, entity
                | ValueNone -> ()
        }
        |> Seq.groupBy fst
        |> Seq.map (fun (ns, group) -> namespaceItem projectId library ns (group |> Seq.map snd |> Seq.toArray))
        |> Seq.sortBy _.DisplayText
        |> Seq.toArray

    /// Types that sit directly under the project or assembly, outside any namespace.
    let globalTypeItems projectId library (types: FSharpEntity[]) =
        types
        |> Seq.filter (fun entity -> (namespaceOf entity).IsNone)
        |> typeItems projectId library

    let private valueKind (value: FSharpMemberOrFunctionOrValue) =
        if value.IsProperty then
            ObjectMemberKind.Property
        elif value.IsEvent then
            ObjectMemberKind.Event
        elif value.LiteralValue.IsSome then
            ObjectMemberKind.Constant
        elif value.CompiledName.StartsWith("op_", StringComparison.Ordinal) then
            ObjectMemberKind.Operator
        elif not value.IsMember && not value.FullType.IsFunctionType then
            ObjectMemberKind.Field
        else
            ObjectMemberKind.Method

    let private isBrowsableValue (value: FSharpMemberOrFunctionOrValue) =
        not value.IsCompilerGenerated
        && not value.IsPropertyGetterMethod
        && not value.IsPropertySetterMethod
        && not value.IsEventAddMethod
        && not value.IsEventRemoveMethod

    /// One guard per produced item: a member that throws on any property is skipped, and cannot
    /// take its siblings down with it.
    let private declaredMemberItems projectId library (entity: FSharpEntity) isInherited (includeName: string -> bool) =
        let ns = namespaceOf entity
        let className = classNameOf entity
        let container = qualify ns className

        let make (symbol: #FSharpSymbol) =
            memberItem projectId library ns className container symbol

        [|
            for value in items (fun () -> entity.MembersFunctionsAndValues) do
                match
                    tryValue (fun () ->
                        if isBrowsableValue value && includeName value.DisplayName then
                            let displayText = withSignature value.DisplayName (formatType value.FullType)
                            ValueSome(make value (valueKind value) isInherited displayText)
                        else
                            ValueNone)
                with
                | ValueSome(ValueSome item) -> item
                | _ -> ()

            for field in items (fun () -> entity.FSharpFields) do
                match
                    tryValue (fun () ->
                        if not field.IsCompilerGenerated && includeName field.Name then
                            let kind =
                                if field.IsLiteral then
                                    ObjectMemberKind.Constant
                                else
                                    ObjectMemberKind.Field

                            let displayText = withSignature field.Name (formatType field.FieldType)
                            ValueSome(make field kind isInherited displayText)
                        else
                            ValueNone)
                with
                | ValueSome(ValueSome item) -> item
                | _ -> ()

            for case in items (fun () -> entity.UnionCases) do
                match
                    tryValue (fun () ->
                        if includeName case.Name then
                            ValueSome(make case ObjectMemberKind.EnumMember isInherited case.Name)
                        else
                            ValueNone)
                with
                | ValueSome(ValueSome item) -> item
                | _ -> ()
        |]

    let private includeAllNames (_: string) = true

    let private tryTypeDefinition (fsharpType: FSharpType) =
        match tryValue (fun () -> fsharpType.HasTypeDefinition) with
        | ValueSome true -> ValueSome fsharpType.TypeDefinition
        | _ -> ValueNone

    let private tryBaseEntity (entity: FSharpEntity) =
        tryGet (fun () -> entity.BaseType) |> ValueOption.bind tryTypeDefinition

    let memberItems projectId library (entity: FSharpEntity) =
        let declared = declaredMemberItems projectId library entity false includeAllNames

        let seen =
            HashSet<string>(declared |> Seq.map _.DisplayText, StringComparer.Ordinal)

        let rec inherited (entity: FSharpEntity) depth =
            [|
                if depth < 16 then
                    match tryBaseEntity entity with
                    | ValueSome baseEntity ->
                        yield!
                            declaredMemberItems projectId library baseEntity true includeAllNames
                            |> Array.filter (fun item -> seen.Add item.DisplayText)

                        yield! inherited baseEntity (depth + 1)
                    | ValueNone -> ()
            |]

        [| yield! declared |> Array.sortBy _.DisplayText; yield! inherited entity 0 |]

    let baseTypeItems projectId library (entity: FSharpEntity) =
        let baseTypes =
            seq {
                match tryBaseEntity entity with
                | ValueSome baseEntity -> baseEntity
                | ValueNone -> ()

                for interfaceType in items (fun () -> entity.DeclaredInterfaces) do
                    match tryTypeDefinition interfaceType with
                    | ValueSome definition -> definition
                    | ValueNone -> ()
            }

        typeItems projectId library baseTypes

    let matchesSearch (searchText: string) (item: ObjectListItem) =
        item.FullName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0

    /// Search materializes only members that can still match after filtering: declared members whose
    /// name — or whose container's qualified name — contains the text. Inherited members are skipped;
    /// a hit on one is reported under its declaring type anyway.
    let searchMemberItems projectId library (searchText: string) (entity: FSharpEntity) =
        let containerMatches =
            (qualify (namespaceOf entity) (classNameOf entity)).IndexOf(searchText, StringComparison.OrdinalIgnoreCase)
            >= 0

        let includeName (name: string) =
            containerMatches
            || name.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0

        declaredMemberItems projectId library entity false includeName

    let trySymbol (item: ObjectListItem) =
        match item.Data with
        | ObjectItemData.Type(entity, _) -> ValueSome(entity :> FSharpSymbol)
        | ObjectItemData.Member(symbol, _, _) -> ValueSome symbol
        | _ -> ValueNone

    let tryDeclarationRange (item: ObjectListItem) =
        trySymbol item
        |> ValueOption.bind (fun symbol -> tryGet (fun () -> symbol.DeclarationLocation))
