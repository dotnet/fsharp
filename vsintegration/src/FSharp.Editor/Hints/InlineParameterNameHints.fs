// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Hints

open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.CodeAnalysis
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

    // copypaste: https://stackoverflow.com/a/15993446/3232646
    let private allIndexesOf (value: string) (s: string)  =
        seq {
            let mutable minIndex = s.IndexOf(value);
            while (minIndex <> -1) do
                yield minIndex;
                minIndex <- s.IndexOf(value, minIndex + value.Length);
        }

    // Fragile Roslyn arithmetics, don't try this at home.
    // 
    // Why the hell is this so complicated? 
    // There can be (rarely) cases like there can be cases like 
    // <somecode>.SymbolUse1().SymbolUse2()<morecode>
    // and we need to locate the last one.
    // Hopefully someone will have a better way to do that one day,
    // this is quite heavily tested so should be safe to refactor.
    let getSymbolPosition
        (symbolUse: FSharpSymbolUse) 
        (source: Microsoft.CodeAnalysis.Text.SourceText) = 

        let symbolName = symbolUse.Symbol.DisplayNameCore
        let symbolLine = symbolUse.Range.End.Line - 1
        let symbolRow = source.Lines.[symbolLine]
        let symbolColumns = $"{symbolRow}" |> allIndexesOf symbolName

        if symbolColumns |> Seq.isEmpty
        then None
        else
            let positionLine = symbolLine + 1
            let positionColumn = symbolUse.Range.End.Column + 1
            Some (Position.mkPos positionLine positionColumn)

    let private getTupleRanges
        (symbolUse: FSharpSymbolUse)
        (source: Microsoft.CodeAnalysis.Text.SourceText)
        (parseResults: FSharpParseFileResults) =
        
        getSymbolPosition symbolUse source
        |> Option.bind (parseResults.FindParameterLocations)
        |> Option.map (fun locations -> locations.ArgumentLocations)
        |> Option.map (Seq.map (fun location -> location.ArgumentRange))
        |> Option.defaultValue []
        |> Seq.toList

    let private getCurryRanges 
        (symbolUse: FSharpSymbolUse) 
        (parseResults: FSharpParseFileResults) = 

        parseResults.GetAllArgumentsForFunctionApplicationAtPosition symbolUse.Range.Start
        |> Option.defaultValue []

    let isMemberOrFunctionOrValueValidForHint (symbol: FSharpMemberOrFunctionOrValue) (symbolUse: FSharpSymbolUse) =
        if symbolUse.IsFromUse then
            let isNotBuiltInOperator = 
                symbol.DeclaringEntity 
                |> Option.exists (fun entity -> entity.CompiledName <> "Operators")

            (symbol.IsFunction && isNotBuiltInOperator) // arguably, hints for those would be rather useless
            || symbol.IsConstructor
            || symbol.IsMethod
        else
            false

    let isUnionCaseValidForHint (symbol: FSharpUnionCase) (symbolUse: FSharpSymbolUse) =
        symbolUse.IsFromUse
        && symbol.DisplayName <> "(::)"

    let getHintsForMemberOrFunctionOrValue
        (parseResults: FSharpParseFileResults) 
        (source: Microsoft.CodeAnalysis.Text.SourceText)
        (symbol: FSharpMemberOrFunctionOrValue) 
        (symbolUse: FSharpSymbolUse) =

        let parameters = symbol.CurriedParameterGroups |> Seq.concat

        let tupleRanges = parseResults |> getTupleRanges symbolUse source
        let curryRanges = parseResults |> getCurryRanges symbolUse
        let ranges = if tupleRanges |> (not << Seq.isEmpty) then tupleRanges else curryRanges

        parameters
        |> Seq.zip ranges // Seq.zip is important as List.zip requires equal lengths
        |> Seq.where (snd >> doesParameterNameExist)
        |> Seq.map getParameterHint
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
