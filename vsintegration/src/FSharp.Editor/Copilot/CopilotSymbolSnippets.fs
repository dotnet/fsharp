// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Widens the identifier range of a navigable item to the declaration a reader would recognise.
module internal Microsoft.VisualStudio.FSharp.Editor.CopilotSymbolSnippets

open FSharp.Compiler.EditorServices

/// A module scope can span a whole file, which is more than a chat prompt can usefully carry.
[<Literal>]
let MaxSnippetLines = 200

/// Inclusive, 1-based line bounds of the declaration `item` names, including its doc comment.
let definitionLines (scopes: Structure.ScopeRange seq) (item: NavigableItem) =
    let declarationLine = item.Range.StartLine

    // A construct's outlining range reaches back over the doc comment in front of it, so it is the
    // collapse range - the body proper - that tells which construct is declared on this line.
    let declaredHere (scope: Structure.ScopeRange) =
        scope.CollapseRange.StartLine = declarationLine
        && scope.Range.EndLine >= item.Range.EndLine
        && scope.Scope <> Structure.Scope.Comment
        && scope.Scope <> Structure.Scope.XmlDocComment

    let mutable widest = ValueNone

    for scope in scopes do
        if declaredHere scope then
            match widest with
            | ValueSome(previous: Structure.ScopeRange) when previous.Range.EndLine >= scope.Range.EndLine -> ()
            | _ -> widest <- ValueSome scope

    // A one-line member declares no scope of its own; it stands for itself rather than for the type around it.
    let firstLine, lastLine =
        match widest with
        | ValueSome scope -> scope.Range.StartLine, scope.Range.EndLine
        | ValueNone -> declarationLine, item.Range.EndLine

    struct (firstLine, min lastLine (firstLine + MaxSnippetLines - 1))
