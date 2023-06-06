// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Hints

open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text
open Hints
open CancellableTasks

type InlineParameterNameHints(parseResults: FSharpParseFileResults) =

    let getTooltip (symbol: FSharpSymbol) _ =
        cancellableTask {
            // This brings little value as of now. Basically just discerns fields from parameters
            // and fills the tooltip bubble which otherwise looks like a visual glitch.
            //
            // Now, we could add some type information here, like C# does, for example:
            // (parameter) int number
            //
            // This would work for simple cases but can get weird in more complex ones.
            // Consider this code:
            //
            // let rev list = list |> List.rev
            // let reversed = rev [ 42 ]
            //
            // With the trivial implementation, the tooltip for hint before [ 42 ] will look like:
            // parameter 'a list list
            //
            // Arguably, this can look confusing.
            // Hence, I wouldn't add type info to the text until we have some coloring plugged in here.
            //
            // Some alignment with C# also needs to be kept in mind,
            // e.g. taking the type in braces would be opposite to what C# does which can be confusing.
            let text = symbol.ToString()

            return [ TaggedText(TextTag.Text, text) ]
        }

    let getParameterHint (range: range, parameter: FSharpParameter) =
        {
            Kind = HintKind.ParameterNameHint
            Range = range.StartRange
            Parts = [ TaggedText(TextTag.Text, $"{parameter.DisplayName} = ") ]
            GetTooltip = getTooltip parameter
        }

    let getFieldHint (range: range, field: FSharpField) =
        {
            Kind = HintKind.ParameterNameHint
            Range = range.StartRange
            Parts = [ TaggedText(TextTag.Text, $"{field.Name} = ") ]
            GetTooltip = getTooltip field
        }

    let parameterNameExists (parameter: FSharpParameter) = parameter.DisplayName <> ""

    let fieldNameExists (field: FSharpField) = not field.IsNameGenerated

    let getArgumentLocations (symbolUse: FSharpSymbolUse) =

        let position =
            Position.mkPos (symbolUse.Range.End.Line) (symbolUse.Range.End.Column + 1)

        parseResults.FindParameterLocations position
        |> Option.map (fun locations ->
            locations.ArgumentLocations
            |> Seq.filter (fun location -> Position.posGeq location.ArgumentRange.Start position))
        |> Option.filter (not << Seq.isEmpty)
        |> Option.defaultValue Seq.empty

    let getTupleRanges = Seq.map (fun location -> location.ArgumentRange)

    let getCurryRanges (symbolUse: FSharpSymbolUse) =

        parseResults.GetAllArgumentsForFunctionApplicationAtPosition symbolUse.Range.Start
        |> Option.defaultValue []

    let isNamedArgument range =
        Seq.filter (fun location -> location.IsNamedArgument)
        >> Seq.map (fun location -> location.ArgumentRange)
        >> Seq.contains range

    let isCustomOperation (symbol: FSharpMemberOrFunctionOrValue) =
        symbol.HasAttribute<CustomOperationAttribute>()

    let getSourceTextAtRange (sourceText: SourceText) (range: range) =
        (RoslynHelpers.FSharpRangeToTextSpan(sourceText, range) |> sourceText.GetSubText)
            .ToString()

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
        // If a case does not use field names, don't even bother getting applied argument ranges
        && symbol.Fields |> Seq.exists fieldNameExists

    member _.GetHintsForMemberOrFunctionOrValue
        (sourceText: SourceText)
        (symbol: FSharpMemberOrFunctionOrValue)
        (symbolUse: FSharpSymbolUse)
        =

        if isMemberOrFunctionOrValueValidForHint symbol symbolUse then
            let parameters = Seq.concat symbol.CurriedParameterGroups
            let argumentLocations = getArgumentLocations symbolUse

            let tupleRanges = getTupleRanges argumentLocations
            let curryRanges = getCurryRanges symbolUse

            let ranges =
                if symbol.IsFunction || Seq.isEmpty tupleRanges then
                    curryRanges |> List.toSeq
                else
                    tupleRanges
                |> Seq.filter (fun range -> argumentLocations |> (not << isNamedArgument range))

            let argumentNames = Seq.map (getSourceTextAtRange sourceText) ranges
            let skipped = if symbol |> isCustomOperation then 1 else 0

            parameters
            |> Seq.skip skipped
            |> Seq.zip ranges // Seq.zip is important as List.zip requires equal lengths
            |> Seq.where (snd >> parameterNameExists)
            |> Seq.zip argumentNames
            |> Seq.choose (fun (argumentName, (range, parameter)) ->
                if argumentName <> parameter.DisplayName then
                    Some(getParameterHint (range, parameter))
                else
                    None)
            |> Seq.toList
        else
            []

    member _.GetHintsForUnionCase (sourceText: SourceText) (symbol: FSharpUnionCase) (symbolUse: FSharpSymbolUse) =
        if isUnionCaseValidForHint symbol symbolUse then

            let fields = Seq.toList symbol.Fields

            let ranges =
                parseResults.GetAllArgumentsForFunctionApplicationAtPosition symbolUse.Range.Start

            let argumentNames =
                match ranges with
                | Some ranges -> (List.map (getSourceTextAtRange sourceText)) ranges
                | None -> []
            // When not all field values are provided (as the user is typing), don't show anything yet
            match ranges with
            | Some ranges when ranges.Length = fields.Length ->
                fields
                |> List.zip3 argumentNames ranges
                |> List.where (fun (argumentName, _, parameter) -> fieldNameExists parameter && argumentName <> parameter.DisplayName)
                |> List.map (fun (_, range, parameter) -> getFieldHint (range, parameter))

            | _ -> []
        else
            []
