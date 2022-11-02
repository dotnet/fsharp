// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Hints

open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text
open Hints

module InlineParameterNameHints =

    let private getHint (range: range, parameter: FSharpParameter) =
        {
            Kind = HintKind.ParameterNameHint
            Range = range.StartRange
            Parts = [ TaggedText(TextTag.Text, $"{parameter.DisplayName} = ") ]
        }

    let private doesParameterNameExist (parameter: FSharpParameter) = 
        parameter.DisplayName <> ""

    let isValidForHint (symbol: FSharpMemberOrFunctionOrValue) =
        // is there a better way?
        let isNotAnOperator = 
            symbol.DeclaringEntity 
            |> Option.exists (fun entity -> entity.CompiledName <> "Operators")

        symbol.IsFunction
        && isNotAnOperator // arguably, hints for those would be rather useless

    let getHints 
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
            |> Seq.map getHint
            |> Seq.toList
        
        // not sure when this can happen
        | None -> []
