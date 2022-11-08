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

    let isMemberOrFunctionOrValueValidForHint (symbol: FSharpMemberOrFunctionOrValue) =
        // is there a better way?
        let isNotBuiltInOperator = 
            symbol.DeclaringEntity 
            |> Option.exists (fun entity -> entity.CompiledName <> "Operators")

        symbol.IsFunction
        && isNotBuiltInOperator // arguably, hints for those would be rather useless

    let isUnionCaseValidForHint (symbolUse: FSharpSymbolUse) =
        // is the union case being used as a constructor
        symbolUse.IsFromUse

    let getHintsForMemberOrFunctionOrValue
        (parseResults: FSharpParseFileResults) 
        (symbol: FSharpMemberOrFunctionOrValue) 
        (symbolUse: FSharpSymbolUse) =

        let parameters = symbol.CurriedParameterGroups |> Seq.concat
        let ranges = parseResults.GetAllArgumentsForFunctionApplicationAtPosition symbolUse.Range.Start

        match ranges with
        | Some ranges -> 
            parameters
            |> Seq.zip ranges
            |> Seq.where (snd >> doesParameterNameExist)
            |> Seq.map getParameterHint
            |> Seq.toList
        
        // this is the case at least for custom operators
        | None -> []

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
