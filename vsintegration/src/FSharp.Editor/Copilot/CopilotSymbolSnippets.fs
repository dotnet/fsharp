// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal Microsoft.VisualStudio.FSharp.Editor.CopilotSymbolSnippets

open System
open System.Collections.Generic

open FSharp.Compiler.EditorServices

[<Literal>]
let MaxSnippetLines = 200

/// A scope's header runs from its Range start, over its doc comment, to its CollapseRange start.
let declaredByLine (scopes: Structure.ScopeRange seq) =
    let declared = Dictionary<int, Structure.ScopeRange>()

    for scope in scopes do
        if
            scope.Scope <> Structure.Scope.Comment
            && scope.Scope <> Structure.Scope.XmlDocComment
        then
            for line in scope.Range.StartLine .. scope.CollapseRange.StartLine do
                match declared.TryGetValue line with
                | true, widest when widest.Range.EndLine >= scope.Range.EndLine -> ()
                | _ -> declared[line] <- scope

    declared

let declarationLines (sourceLines: string array) (declared: Dictionary<int, Structure.ScopeRange>) (item: NavigableItem) =
    let declarationLine = item.Range.StartLine

    let firstLine, lastLine =
        match declared.TryGetValue declarationLine with
        | true, scope when scope.Range.EndLine >= item.Range.EndLine -> scope.Range.StartLine, scope.Range.EndLine
        | _ -> declarationLine, item.Range.EndLine

    // Outlining reports no scope for a one-line "///", so walk back over it by text.
    let isDocComment line =
        sourceLines[line - 1].AsSpan().TrimStart().StartsWith("///".AsSpan(), StringComparison.Ordinal)

    let rec docCommentStart line =
        if line > 1 && isDocComment (line - 1) then
            docCommentStart (line - 1)
        else
            line

    struct (docCommentStart firstLine, lastLine)

let definitionLines (sourceLines: string array) (declared: Dictionary<int, Structure.ScopeRange>) (item: NavigableItem) =
    let struct (firstLine, lastLine) = declarationLines sourceLines declared item
    struct (firstLine, min lastLine (firstLine + MaxSnippetLines - 1))
