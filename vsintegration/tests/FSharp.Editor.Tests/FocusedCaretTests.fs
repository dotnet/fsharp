// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.FocusedCaretTests

open Xunit
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.Text

let private caretWithRecordedChanges () =
    let caret = FocusedCaret()
    let changes = ResizeArray()
    caret.LineChanged.Add(fun () -> changes.Add caret.Position)
    caret, changes

[<Fact>]
let ``a caret that moves to another line is published as a line change`` () =
    let caret, changes = caretWithRecordedChanges ()

    caret.Update(Some(Position.mkPos 7 0))
    caret.Update(Some(Position.mkPos 8 4))

    Assert.Equal<int list>([ 7; 8 ], [ for position in changes -> position.Value.Line ])
    Assert.Equal(8, caret.Position.Value.Line)

[<Fact>]
let ``a caret that moves along its line is published without a line change`` () =
    let caret, changes = caretWithRecordedChanges ()

    caret.Update(Some(Position.mkPos 7 0))
    caret.Update(Some(Position.mkPos 7 12))

    Assert.Equal(1, changes.Count)
    Assert.Equal(12, caret.Position.Value.Column)

[<Fact>]
let ``editors losing focus is a line change, and the caret is gone`` () =
    let caret, changes = caretWithRecordedChanges ()

    caret.Update(Some(Position.mkPos 7 0))
    caret.Update None

    Assert.Equal(2, changes.Count)
    Assert.True(caret.Position.IsNone)

[<Fact>]
let ``no editor has focus twice over is no line change`` () =
    let caret, changes = caretWithRecordedChanges ()

    caret.Update None
    caret.Update None

    Assert.Empty changes
    Assert.True(caret.Position.IsNone)

[<Fact>]
let ``a text no view is open on has no caret`` () =
    Assert.True((FocusedCaret.TryGet(SourceText.From "#r \"nuget: Newtonsoft.Json\"\n")).IsNone)
