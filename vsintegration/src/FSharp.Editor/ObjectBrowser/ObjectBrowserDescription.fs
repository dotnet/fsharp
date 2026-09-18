// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.Text

open Microsoft.VisualStudio.Shell.Interop

open FSharp.Compiler.Symbols
open FSharp.Compiler.Text

/// Fills the Object Browser description pane for one node.
[<RequireQualifiedAccess>]
module internal ObjectBrowserDescription =

    let private blankLine = Environment.NewLine + Environment.NewLine

    let private add (description: IVsObjectBrowserDescription3) section text =
        description.AddDescriptionText3(text, section, null) |> ignore

    let private addLink (description: IVsObjectBrowserDescription3) text navInfo =
        description.AddDescriptionText3(text, VSOBDESCRIPTIONSECTION.OBDS_TYPE, navInfo)
        |> ignore

    let private accessPrefix (access: FSharpAccessibility voption) =
        match access with
        | ValueSome(Tokenizer.Private) -> "private "
        | ValueSome(Tokenizer.Internal) -> "internal "
        | _ -> ""

    let private xmlDocOf (symbol: FSharpSymbol) =
        match symbol with
        | :? FSharpEntity as entity -> entity.XmlDoc
        | :? FSharpMemberOrFunctionOrValue as value -> value.XmlDoc
        | :? FSharpField as field -> field.XmlDoc
        | :? FSharpUnionCase as case -> case.XmlDoc
        | _ -> FSharpXmlDoc.None

    /// `AppendXmlComment` is what QuickInfo uses: it normalises plain-text doc comments into
    /// `<summary>`, resolves `<see cref=.../>` and friends, and — unlike reading `XmlDoc` directly —
    /// reaches the companion XML file for symbols that came from metadata.
    let private summaryOf (documentation: IDocumentationBuilder) (symbol: FSharpSymbol) =
        let text = StringBuilder()

        let collector =
            TextSanitizingCollector(fun (taggedText: TaggedText) -> text.Append taggedText.Text |> ignore) :> ITaggedTextCollector

        XmlDocumentation.AppendXmlComment(
            documentation,
            collector,
            collector,
            xmlDocOf symbol,
            false, // showExceptions
            false, // showParameters
            false, // showRemarks
            None
        )

        text.ToString().Trim()

    let private addContainerLink description (navInfoFactory: FSharpNavInfoFactory) (navPath: ObjectNavPath) =
        let containerName, containerNavInfo =
            match navPath.Member, navPath.Class, navPath.Namespace with
            | ValueSome _, ValueSome className, ns -> ValueSome className, navInfoFactory.Create(navPath.Library, ns, ValueSome className)
            | ValueNone, ValueSome _, ValueSome ns -> ValueSome ns, navInfoFactory.Create(navPath.Library, ValueSome ns)
            | _ -> ValueNone, navInfoFactory.Create(navPath.Library)

        match containerName with
        | ValueNone -> ()
        | ValueSome containerName ->
            add description VSOBDESCRIPTIONSECTION.OBDS_MISC Environment.NewLine

            match SR.ObjectBrowserMemberOf().Split([| "{0}" |], StringSplitOptions.None) with
            | [| before; after |] ->
                add description VSOBDESCRIPTIONSECTION.OBDS_MISC before
                addLink description containerName containerNavInfo
                add description VSOBDESCRIPTIONSECTION.OBDS_MISC after
            | _ -> addLink description containerName containerNavInfo

    let private addSummary description documentation symbol =
        match summaryOf documentation symbol with
        | "" -> ()
        | summary ->
            add description VSOBDESCRIPTIONSECTION.OBDS_MISC blankLine
            add description VSOBDESCRIPTIONSECTION.OBDS_MISC summary

    let private addContainerOf description navInfoFactory (item: ObjectListItem) =
        match item.NavPath with
        | ValueSome navPath -> addContainerLink description navInfoFactory navPath
        | ValueNone -> ()

    let private typeDeclaration (item: ObjectListItem) (entity: FSharpEntity) kind =
        let keyword =
            match kind with
            | ObjectTypeKind.Module -> "module"
            | ObjectTypeKind.Exception -> "exception"
            | _ -> "type"

        let genericParameters: IList<_> =
            match kind with
            | ObjectTypeKind.Module -> ResizeArray()
            | _ ->
                try
                    entity.GenericParameters
                with _ ->
                    ResizeArray()

        let declaration =
            StringBuilder().Append(accessPrefix item.Accessibility).Append(keyword).Append(' ').Append(item.DisplayText)

        genericParameters
        |> Seq.iteri (fun index parameter ->
            declaration.Append(if index = 0 then "<'" else ", '").Append(parameter.DisplayName)
            |> ignore)

        if not (Seq.isEmpty genericParameters) then
            declaration.Append('>') |> ignore

        declaration.ToString()

    let fill
        (navInfoFactory: FSharpNavInfoFactory)
        (documentation: IDocumentationBuilder)
        (item: ObjectListItem)
        (description: IVsObjectBrowserDescription3)
        =
        match item.Data with
        | ObjectItemData.Pending
        | ObjectItemData.Folder _ -> add description VSOBDESCRIPTIONSECTION.OBDS_NAME item.DisplayText

        | ObjectItemData.Project ->
            add description VSOBDESCRIPTIONSECTION.OBDS_MISC (SR.ObjectBrowserProject() + " ")
            add description VSOBDESCRIPTIONSECTION.OBDS_NAME item.DisplayText

        | ObjectItemData.Reference(_, path) ->
            add description VSOBDESCRIPTIONSECTION.OBDS_MISC (SR.ObjectBrowserAssembly() + " ")
            add description VSOBDESCRIPTIONSECTION.OBDS_NAME item.DisplayText

            match path with
            | ValueSome path ->
                add description VSOBDESCRIPTIONSECTION.OBDS_ENDDECL Environment.NewLine
                add description VSOBDESCRIPTIONSECTION.OBDS_MISC path
            | ValueNone -> ()

        | ObjectItemData.Namespace _ ->
            add description VSOBDESCRIPTIONSECTION.OBDS_MISC "namespace "
            add description VSOBDESCRIPTIONSECTION.OBDS_NAME item.DisplayText

        | ObjectItemData.Type(entity, kind) ->
            add description VSOBDESCRIPTIONSECTION.OBDS_NAME (typeDeclaration item entity kind)
            add description VSOBDESCRIPTIONSECTION.OBDS_ENDDECL Environment.NewLine
            addContainerOf description navInfoFactory item
            addSummary description documentation entity

        | ObjectItemData.Member(symbol, _, _) ->
            add description VSOBDESCRIPTIONSECTION.OBDS_NAME item.DisplayText
            add description VSOBDESCRIPTIONSECTION.OBDS_ENDDECL Environment.NewLine
            addContainerOf description navInfoFactory item
            addSummary description documentation symbol
