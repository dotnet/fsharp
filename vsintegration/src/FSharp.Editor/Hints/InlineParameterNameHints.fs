﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Hints

open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text
open Hints

module InlineParameterNameHints =

    let private getParameterHint (range: range, parameter: FSharpParameter) =
        {
            Kind = HintKind.ParameterNameHint
            Range = range.StartRange
            Parts = [ TaggedText(TextTag.Text, $"{parameter.DisplayName} = ") ]
        }

    let private getFieldHint (range: range, field: FSharpField) =
        {
            Kind = HintKind.ParameterNameHint
            Range = range.StartRange
            Parts = [ TaggedText(TextTag.Text, $"{field.Name} = ") ]
        }

    let private doesParameterNameExist (parameter: FSharpParameter) = 
        parameter.DisplayName <> ""

    let private doesFieldNameExist (field: FSharpField) = 
        not field.IsNameGenerated

    let private getArgumentLocations
        (symbolUse: FSharpSymbolUse)
        (parseResults: FSharpParseFileResults) =

        let position = Position.mkPos 
                        (symbolUse.Range.End.Line) 
                        (symbolUse.Range.End.Column + 1)

        parseResults.FindParameterLocations position
        |> Option.map (fun locations -> locations.ArgumentLocations |> Seq.filter (fun location -> Position.posGeq location.ArgumentRange.Start position))
        |> Option.filter (not << Seq.isEmpty)
        |> Option.defaultValue Seq.empty

    let private getTupleRanges =
        Seq.map (fun location -> location.ArgumentRange)
        >> Seq.toList

    let private getCurryRanges 
        (symbolUse: FSharpSymbolUse) 
        (parseResults: FSharpParseFileResults) = 

        parseResults.GetAllArgumentsForFunctionApplicationAtPosition symbolUse.Range.Start
        |> Option.defaultValue []

    let private isNamedArgument range =
        Seq.filter (fun location -> location.IsNamedArgument)
        >> Seq.map (fun location -> location.ArgumentRange) 
        >> Seq.contains range

    let private getSourceTextAtRange 
        (sourceText: SourceText)
        (range: range) =

        let line = sourceText.Lines[range.Start.Line - 1].ToString()
        let length = range.EndColumn - range.StartColumn
        line.Substring(range.Start.Column, length)

    let isMemberOrFunctionOrValueValidForHint 
        (symbol: FSharpMemberOrFunctionOrValue) 
        (symbolUse: FSharpSymbolUse) =

        if symbolUse.IsFromUse then
            let isNotBuiltInOperator = 
                symbol.DeclaringEntity 
                |> Option.exists (fun entity -> entity.CompiledName <> "Operators")

            let isNotCustomOperation = 
                not <| symbol.HasAttribute<CustomOperationAttribute>()

            (symbol.IsFunction && isNotBuiltInOperator) // arguably, hints for those would be rather useless
            || symbol.IsConstructor
            || (symbol.IsMethod && isNotCustomOperation)
        else
            false

    let isUnionCaseValidForHint (symbol: FSharpUnionCase) (symbolUse: FSharpSymbolUse) =
        symbolUse.IsFromUse
        && symbol.DisplayName <> "(::)"

    let getHintsForMemberOrFunctionOrValue
        (sourceText: SourceText)
        (parseResults: FSharpParseFileResults) 
        (symbol: FSharpMemberOrFunctionOrValue) 
        (symbolUse: FSharpSymbolUse) =

        let parameters = symbol.CurriedParameterGroups |> Seq.concat
        let argumentLocations = parseResults |> getArgumentLocations symbolUse

        let tupleRanges = argumentLocations |> getTupleRanges
        let curryRanges = parseResults |> getCurryRanges symbolUse

        let ranges = 
            if tupleRanges |> (not << Seq.isEmpty) then tupleRanges else curryRanges
            |> Seq.filter (fun range -> argumentLocations |> (not << isNamedArgument range))

        let argumentNames = 
            ranges
            |> Seq.map (getSourceTextAtRange sourceText)

        parameters
        |> Seq.zip ranges // Seq.zip is important as List.zip requires equal lengths
        |> Seq.where (snd >> doesParameterNameExist)
        |> Seq.zip argumentNames
        |> Seq.choose (fun (argumentName, (range, parameter)) -> 
            if argumentName <> parameter.DisplayName then Some (getParameterHint (range, parameter)) else None)
        |> Seq.toList

    let getHintsForUnionCase
        (parseResults: FSharpParseFileResults) 
        (symbol: FSharpUnionCase) 
        (symbolUse: FSharpSymbolUse) =

        let fields = Seq.toList symbol.Fields

        // If a case does not use field names, don't even bother getting applied argument ranges
        if fields |> List.exists doesFieldNameExist |> not then
            []
        else
            let ranges = parseResults.GetAllArgumentsForFunctionApplicationAtPosition symbolUse.Range.Start
            
            // When not all field values are provided (as the user is typing), don't show anything yet
            match ranges with
            | Some ranges when ranges.Length = fields.Length -> 
                fields
                |> List.zip ranges
                |> List.where (snd >> doesFieldNameExist)
                |> List.map getFieldHint
            
            | _ -> []
