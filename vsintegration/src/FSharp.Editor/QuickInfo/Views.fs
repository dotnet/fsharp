// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Collections.Generic
open FSharp.Compiler.TextLayout
open Microsoft.CodeAnalysis.Classification
open Microsoft.VisualStudio.Core.Imaging
open Microsoft.VisualStudio.Language.StandardClassification
open Microsoft.VisualStudio.Text.Adornments

module internal QuickInfoViewProvider =

    let layoutTagToClassificationTag (layoutTag:LayoutTag) =
        match layoutTag with
        | LayoutTag.ActivePatternCase
        | LayoutTag.ActivePatternResult
        | LayoutTag.UnionCase
        | LayoutTag.Enum -> ClassificationTypeNames.EnumName
        | LayoutTag.Struct -> ClassificationTypeNames.StructName
        | LayoutTag.TypeParameter -> ClassificationTypeNames.TypeParameterName
        | LayoutTag.Alias
        | LayoutTag.Class
        | LayoutTag.Record
        | LayoutTag.Union
        | LayoutTag.UnknownType // Default to class until/unless we use classification data
        | LayoutTag.Module -> ClassificationTypeNames.ClassName
        | LayoutTag.Interface -> ClassificationTypeNames.InterfaceName
        | LayoutTag.Keyword -> ClassificationTypeNames.Keyword
        | LayoutTag.Member
        | LayoutTag.Function
        | LayoutTag.Method -> ClassificationTypeNames.MethodName
        | LayoutTag.Property
        | LayoutTag.RecordField -> ClassificationTypeNames.PropertyName
        | LayoutTag.Parameter
        | LayoutTag.Local -> ClassificationTypeNames.LocalName
        | LayoutTag.ModuleBinding -> ClassificationTypeNames.Identifier
        | LayoutTag.Namespace -> ClassificationTypeNames.NamespaceName
        | LayoutTag.Delegate -> ClassificationTypeNames.DelegateName
        | LayoutTag.Event -> ClassificationTypeNames.EventName
        | LayoutTag.Field -> ClassificationTypeNames.FieldName
        | LayoutTag.LineBreak
        | LayoutTag.Space -> ClassificationTypeNames.WhiteSpace
        | LayoutTag.NumericLiteral -> ClassificationTypeNames.NumericLiteral
        | LayoutTag.Operator -> ClassificationTypeNames.Operator
        | LayoutTag.StringLiteral -> ClassificationTypeNames.StringLiteral
        | LayoutTag.Punctuation -> ClassificationTypeNames.Punctuation
        | LayoutTag.UnknownEntity
        | LayoutTag.Text -> ClassificationTypeNames.Text

    let provideContent
        (
            imageId:ImageId option,
            description: seq<TaggedText>,
            documentation: seq<TaggedText>,
            navigation:QuickInfoNavigation
        ) =

        let buildContainerElement (itemGroup: seq<TaggedText>) =
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
                | :? NavigableTaggedText as nav when navigation.IsTargetValid nav.Range ->
                    flushRuns()
                    let navigableTextRun = NavigableTextRun(classificationTag, item.Text, fun () -> navigation.NavigateTo nav.Range)
                    currentContainerItems.Add(navigableTextRun :> obj)
                | _ when item.Tag = LayoutTag.LineBreak ->
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

        let innerElement =
            match imageId with
            | Some imageId ->
                ContainerElement(ContainerElementStyle.Wrapped, ImageElement(imageId), buildContainerElement description)
            | None ->
                ContainerElement(ContainerElementStyle.Wrapped, buildContainerElement description)

        ContainerElement(ContainerElementStyle.Stacked, innerElement, buildContainerElement documentation)
