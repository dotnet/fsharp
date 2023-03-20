﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.QuickInfo

open System.Threading
open FSharp.Compiler.Text
open Microsoft.CodeAnalysis.Classification
open Microsoft.VisualStudio.Core.Imaging
open Microsoft.VisualStudio.Text.Adornments

open Microsoft.VisualStudio.FSharp.Editor

module internal QuickInfoViewProvider =

    let layoutTagToClassificationTag (layoutTag: TextTag) =
        match layoutTag with
        | TextTag.ActivePatternCase
        | TextTag.ActivePatternResult
        | TextTag.UnionCase
        | TextTag.Enum -> ClassificationTypeNames.EnumName
        | TextTag.Struct -> ClassificationTypeNames.StructName
        | TextTag.TypeParameter -> ClassificationTypeNames.TypeParameterName
        | TextTag.Alias
        | TextTag.Class
        | TextTag.Record
        | TextTag.Union
        | TextTag.UnknownType // Default to class until/unless we use classification data
        | TextTag.Module -> ClassificationTypeNames.ClassName
        | TextTag.Interface -> ClassificationTypeNames.InterfaceName
        | TextTag.Keyword -> ClassificationTypeNames.Keyword
        | TextTag.Member
        | TextTag.Function
        | TextTag.Method -> ClassificationTypeNames.MethodName
        | TextTag.Property
        | TextTag.RecordField -> ClassificationTypeNames.PropertyName
        | TextTag.Parameter
        | TextTag.Local -> ClassificationTypeNames.LocalName
        | TextTag.ModuleBinding -> ClassificationTypeNames.Identifier
        | TextTag.Namespace -> ClassificationTypeNames.NamespaceName
        | TextTag.Delegate -> ClassificationTypeNames.DelegateName
        | TextTag.Event -> ClassificationTypeNames.EventName
        | TextTag.Field -> ClassificationTypeNames.FieldName
        | TextTag.LineBreak
        | TextTag.Space -> ClassificationTypeNames.WhiteSpace
        | TextTag.NumericLiteral -> ClassificationTypeNames.NumericLiteral
        | TextTag.Operator -> ClassificationTypeNames.Operator
        | TextTag.StringLiteral -> ClassificationTypeNames.StringLiteral
        | TextTag.Punctuation -> ClassificationTypeNames.Punctuation
        | TextTag.UnknownEntity
        | TextTag.Text -> ClassificationTypeNames.Text

    let (|TaggedText|) (tt: TaggedText) = tt.Tag, tt.Text

    let (|LineBreak|_|) =
        function TaggedText (TextTag.LineBreak, _) -> Some () | _ -> None

    let (|DocSeparator|_|) =
        function LineBreak :: TaggedText (TextTag.Text, "-------------") :: LineBreak :: rest -> Some rest | _ -> None

    let wrapContent (elements: obj list) =
        ContainerElement(ContainerElementStyle.Wrapped, elements |> Seq.map box)

    let stackContent (elements: obj list) =
        ContainerElement(ContainerElementStyle.Stacked, elements |> Seq.map box)

    let encloseRuns runs = wrapContent (runs |> List.rev) |> box

    let emptyLine = wrapContent [ ClassifiedTextRun(ClassificationTypeNames.WhiteSpace, "") |> box ]

    let provideContent
        (
            imageId: ImageId option,
            description: TaggedText list,
            documentation: TaggedText list list,
            navigation: FSharpNavigation,
            getTooltip
        ) =

        let encloseText text =
            let rec loop text runs stack =
                match (text: TaggedText list) with
                | [] -> stackContent (encloseRuns runs :: stack |> List.rev)
                | DocSeparator (LineBreak :: rest)
                | DocSeparator rest -> loop rest [] (box Separator :: encloseRuns runs :: stack)
                | LineBreak :: rest when runs |> List.isEmpty -> loop rest [] (emptyLine :: stack)
                | LineBreak :: rest -> loop rest [] (encloseRuns runs :: stack)
                | :? NavigableTaggedText as item :: rest when navigation.IsTargetValid item.Range ->
                    let classificationTag = layoutTagToClassificationTag item.Tag
                    let action = fun () -> navigation.NavigateTo(item.Range, CancellationToken.None)
                    let run = ClassifiedTextRun(classificationTag, item.Text, action, getTooltip item.Range.FileName)
                    loop rest (run :: runs) stack
                | item :: rest ->
                    let run = ClassifiedTextRun(layoutTagToClassificationTag item.Tag, item.Text)
                    loop rest (run :: runs) stack

            loop text [] [] |> box

        let innerElement =
            match imageId with
            | Some imageId -> wrapContent [ stackContent [ ImageElement(imageId) ]; encloseText description ]
            | None -> ContainerElement(ContainerElementStyle.Wrapped, encloseText description)

        let separated = stackContent (documentation |> List.map encloseText)

        wrapContent [ stackContent [ innerElement; separated ] ]
