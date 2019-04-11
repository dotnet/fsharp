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
        | Enum -> ClassificationTypeNames.EnumName // Roslyn-style classification name
        | Alias
        | Class
        | Module
        | Record
        | Struct
        | TypeParameter
        | Union
        | UnknownType -> PredefinedClassificationTypeNames.Type
        | Interface -> ClassificationTypeNames.InterfaceName // Roslyn-style classification name
        | Keyword -> PredefinedClassificationTypeNames.Keyword
        | Delegate
        | Event
        | Field
        | Local
        | Member
        | Method
        | ModuleBinding
        | Namespace
        | Parameter
        | Property
        | RecordField -> PredefinedClassificationTypeNames.Identifier
        | LineBreak
        | Space -> PredefinedClassificationTypeNames.WhiteSpace
        | NumericLiteral -> PredefinedClassificationTypeNames.Number
        | Operator -> PredefinedClassificationTypeNames.Operator
        | StringLiteral -> PredefinedClassificationTypeNames.String
        | Punctuation
        | Text
        | UnknownEntity -> PredefinedClassificationTypeNames.Other

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
