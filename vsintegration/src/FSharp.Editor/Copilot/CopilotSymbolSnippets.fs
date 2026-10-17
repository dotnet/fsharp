// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Widens the identifier range of a navigable item to the declaration a reader would recognise.
module internal Microsoft.VisualStudio.FSharp.Editor.CopilotSymbolSnippets

open System
open System.Collections.Generic

open FSharp.Compiler.EditorServices

/// A module scope can span a whole file, which is more than a chat prompt can usefully carry.
[<Literal>]
let MaxSnippetLines = 200

/// The widest construct declared on each line of a file. A construct's outlining range reaches back over
/// the doc comment in front of it and its collapse range starts where its header ends, so the lines from
/// one start to the other are its header - several of them when each parameter takes a line. Built once
/// per file: a bare "#" ranks every declaration of a large file, and each one asks for its construct.
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

/// Inclusive, 1-based line bounds of the whole declaration `item` names, including its doc comment.
let declarationLines (sourceLines: string array) (declared: Dictionary<int, Structure.ScopeRange>) (item: NavigableItem) =
    let declarationLine = item.Range.StartLine

    // A one-line member declares no scope of its own; it stands for itself rather than for the type around it.
    let firstLine, lastLine =
        match declared.TryGetValue declarationLine with
        | true, scope when scope.Range.EndLine >= item.Range.EndLine -> scope.Range.StartLine, scope.Range.EndLine
        | _ -> declarationLine, item.Range.EndLine

    // Outlining reports a doc comment only once it spans several lines, so a one-line "///" in front of
    // a declaration is invisible to the scopes above.
    let isDocComment line =
        sourceLines[line - 1].AsSpan().TrimStart().StartsWith("///".AsSpan(), StringComparison.Ordinal)

    let rec docCommentStart line =
        if line > 1 && isDocComment (line - 1) then
            docCommentStart (line - 1)
        else
            line

    struct (docCommentStart firstLine, lastLine)

/// The lines of the declaration `item` names that a chat prompt carries.
let definitionLines (sourceLines: string array) (declared: Dictionary<int, Structure.ScopeRange>) (item: NavigableItem) =
    let struct (firstLine, lastLine) = declarationLines sourceLines declared item
    struct (firstLine, min lastLine (firstLine + MaxSnippetLines - 1))
