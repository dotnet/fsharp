// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Collections.Generic
open Internal.Utilities.StructuredFormat
open Microsoft.CodeAnalysis.Classification
open FSharp.Compiler
open Microsoft.VisualStudio.Core.Imaging
open Microsoft.VisualStudio.Language.StandardClassification
open Microsoft.VisualStudio.Text.Adornments

module internal QuickInfoViewProvider =

    let layoutTagToClassificationTag (layoutTag:LayoutTag) =
        match layoutTag with
        | ActivePatternCase
        | ActivePatternResult
        | UnionCase
        | Enum -> ClassificationTypeNames.EnumName
        | Struct -> ClassificationTypeNames.StructName
        | TypeParameter -> ClassificationTypeNames.TypeParameterName
        | Alias
        | Class
        | Record
        | Union
        | UnknownType // Default to class until/unless we use classification data
        | Module -> ClassificationTypeNames.ClassName
        | Interface -> ClassificationTypeNames.InterfaceName
        | Keyword -> ClassificationTypeNames.Keyword
        | Member
        | Method -> ClassificationTypeNames.MethodName
        | Property
        | RecordField -> ClassificationTypeNames.PropertyName
        | Parameter
        | Local -> ClassificationTypeNames.LocalName
        | Namespace -> ClassificationTypeNames.NamespaceName
        | Delegate -> ClassificationTypeNames.DelegateName
        | Event -> ClassificationTypeNames.EventName
        | Field -> ClassificationTypeNames.FieldName
        | ModuleBinding -> ClassificationTypeNames.Identifier
        | LineBreak
        | Space -> ClassificationTypeNames.WhiteSpace
        | NumericLiteral -> ClassificationTypeNames.NumericLiteral
        | Operator -> ClassificationTypeNames.Operator
        | StringLiteral -> ClassificationTypeNames.StringLiteral
        | Punctuation -> ClassificationTypeNames.Punctuation
        | UnknownEntity
        | Text -> ClassificationTypeNames.Text

    let provideContent
        (
            imageId:ImageId,
            description:#seq<Layout.TaggedText>,
            documentation:#seq<Layout.TaggedText>,
            navigation:QuickInfoNavigation
        ) =

        let buildContainerElement (itemGroup:#seq<Layout.TaggedText>) =
            let finalCollection = List<ContainerElement>()
            let currentContainerItems = List<obj>()
            let runsCollection = List<ClassifiedTextRun>()
            let flushRuns() =
                if runsCollection.Count > 0 then
                    let element = ClassifiedTextElement(runsCollection)
                    currentContainerItems.Add(element :> obj)
                    runsCollection.Clear()
            let flushContainer() =
                if currentContainerItems.Count > 0 then
                    let element = ContainerElement(ContainerElementStyle.Wrapped, currentContainerItems)
                    finalCollection.Add(element)
                    currentContainerItems.Clear()
            for item in itemGroup do
                let classificationTag = layoutTagToClassificationTag item.Tag
                match item with
                | :? Layout.NavigableTaggedText as nav when navigation.IsTargetValid nav.Range ->
                    flushRuns()
                    let navigableTextRun = NavigableTextRun(classificationTag, item.Text, fun () -> navigation.NavigateTo nav.Range)
                    currentContainerItems.Add(navigableTextRun :> obj)
                | _ when item.Tag = LineBreak ->
                    flushRuns()
                    // preserve succesive linebreaks
                    if currentContainerItems.Count = 0 then
                        runsCollection.Add(ClassifiedTextRun(PredefinedClassificationTypeNames.Other, System.String.Empty))
                        flushRuns()
                    flushContainer()
                | _ -> 
                    let newRun = ClassifiedTextRun(classificationTag, item.Text)
                    runsCollection.Add(newRun)   
            flushRuns()
            flushContainer()
            ContainerElement(ContainerElementStyle.Stacked, finalCollection |> Seq.map box)

        ContainerElement(ContainerElementStyle.Stacked,
            ContainerElement(ContainerElementStyle.Wrapped, 
                ImageElement(imageId), 
                buildContainerElement description),
            buildContainerElement documentation
        )
