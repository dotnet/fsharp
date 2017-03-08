﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Immutable
open System.Collections.Generic
open System.Threading.Tasks
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Layout
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.SourceCodeServices.ItemDescriptionIcons
open Microsoft.FSharp.Compiler.Range
open Microsoft.VisualStudio.FSharp.LanguageService

module internal CommonRoslynHelpers =

    let FSharpRangeToTextSpan(sourceText: SourceText, range: range) =
        // Roslyn TextLineCollection is zero-based, F# range lines are one-based
        let startPosition = sourceText.Lines.[range.StartLine - 1].Start + range.StartColumn
        let endPosition = sourceText.Lines.[range.EndLine - 1].Start + range.EndColumn
        TextSpan(startPosition, endPosition - startPosition)

    let TryFSharpRangeToTextSpan(sourceText: SourceText, range: range) : TextSpan option =
        try Some(FSharpRangeToTextSpan(sourceText, range))
        with e -> 
            //Assert.Exception(e)
            None

    let TextSpanToFSharpRange(fileName: string, textSpan: TextSpan, sourceText: SourceText) : range =
        let startLine = sourceText.Lines.GetLineFromPosition textSpan.Start
        let endLine = sourceText.Lines.GetLineFromPosition textSpan.End
        mkRange 
            fileName 
            (Pos.fromZ startLine.LineNumber (textSpan.Start - startLine.Start))
            (Pos.fromZ endLine.LineNumber (textSpan.End - endLine.Start))

    let GetCompletedTaskResult(task: Task<'TResult>) =
        if task.Status = TaskStatus.RanToCompletion then
            task.Result
        else
            Assert.Exception(task.Exception.GetBaseException())
            raise(task.Exception.GetBaseException())

    type NavigableRoslynText = NavigableRoslynText of tag: string * text: string * xref: obj option with
        static member Create(tag, text, xref) = NavigableRoslynText(tag, text, xref)
        static member Create(tag, text) = NavigableRoslynText(tag, text, None)

    /// Converts `TaggedText` from the F# Compiler to `Microsoft.CodeAnalysis.TaggedText` format for use in tooltips
    let TaggedTextToNavigable t =
        match t with
        | TaggedText.ActivePatternCase t
        | TaggedText.ActivePatternResult t -> NavigableRoslynText.Create(TextTags.Enum, t)
        | TaggedText.Alias t -> NavigableRoslynText.Create(TextTags.Class, t)
        | TaggedText.Class(x, t) -> NavigableRoslynText.Create(TextTags.Class, t, x)
        | TaggedText.Delegate t -> NavigableRoslynText.Create(TextTags.Delegate, t)
        | TaggedText.Enum t -> NavigableRoslynText.Create(TextTags.Enum, t)
        | TaggedText.Event t -> NavigableRoslynText.Create(TextTags.Event, t)
        | TaggedText.Field t -> NavigableRoslynText.Create(TextTags.Field, t)
        | TaggedText.Interface t -> NavigableRoslynText.Create(TextTags.Interface, t)
        | TaggedText.Keyword t -> NavigableRoslynText.Create(TextTags.Keyword, t)
        | TaggedText.LineBreak t -> NavigableRoslynText.Create(TextTags.LineBreak, t)
        | TaggedText.Local t -> NavigableRoslynText.Create(TextTags.Local, t)
        | TaggedText.Member t -> NavigableRoslynText.Create(TextTags.Property, t)
        | TaggedText.Method t -> NavigableRoslynText.Create(TextTags.Method, t)
        | TaggedText.Module t -> NavigableRoslynText.Create(TextTags.Module, t)
        | TaggedText.ModuleBinding t -> NavigableRoslynText.Create(TextTags.Property, t)
        | TaggedText.Namespace t -> NavigableRoslynText.Create(TextTags.Namespace, t)
        | TaggedText.NumericLiteral t -> NavigableRoslynText.Create(TextTags.NumericLiteral, t)
        | TaggedText.Operator t -> NavigableRoslynText.Create(TextTags.Operator, t)
        | TaggedText.Parameter t -> NavigableRoslynText.Create(TextTags.Parameter, t)
        | TaggedText.Property t -> NavigableRoslynText.Create(TextTags.Property, t)
        | TaggedText.Punctuation t -> NavigableRoslynText.Create(TextTags.Punctuation, t)
        | TaggedText.Record t -> NavigableRoslynText.Create(TextTags.Class, t)
        | TaggedText.RecordField t -> NavigableRoslynText.Create(TextTags.Property, t)
        | TaggedText.Space t -> NavigableRoslynText.Create(TextTags.Space, t)
        | TaggedText.StringLiteral t -> NavigableRoslynText.Create(TextTags.StringLiteral, t)
        | TaggedText.Struct t -> NavigableRoslynText.Create(TextTags.Struct, t)
        | TaggedText.Text t -> NavigableRoslynText.Create(TextTags.Text, t)
        | TaggedText.TypeParameter t -> NavigableRoslynText.Create(TextTags.TypeParameter, t)
        | TaggedText.Union t -> NavigableRoslynText.Create(TextTags.Class, t)
        | TaggedText.UnionCase t -> NavigableRoslynText.Create(TextTags.Enum, t)
        | TaggedText.UnknownEntity t -> NavigableRoslynText.Create(TextTags.Property, t)
        | TaggedText.UnknownType t -> NavigableRoslynText.Create(TextTags.Class, t)

    let NavigableTextToRoslyn (NavigableRoslynText(tag, text,_)) = TaggedText(tag, text)

    let CollectNavigableText (list: List<_>) t = list.Add(TaggedTextToNavigable t)

    let CollectTaggedText (list: List<_>) t = list.Add((TaggedTextToNavigable >> NavigableTextToRoslyn) t)

    let StartAsyncAsTask cancellationToken computation =
        let computation =
            async {
                try
                    return! computation
                with e ->
                    Assert.Exception(e)
                    return Unchecked.defaultof<_>
            }
        Async.StartAsTask(computation, TaskCreationOptions.None, cancellationToken)

    let StartAsyncUnitAsTask cancellationToken (computation:Async<unit>) = 
        StartAsyncAsTask cancellationToken computation  :> Task

    let SupportedDiagnostics() =
        // We are constructing our own descriptors at run-time. Compiler service is already doing error formatting and localization.
        let dummyDescriptor = DiagnosticDescriptor("0", String.Empty, String.Empty, String.Empty, DiagnosticSeverity.Error, true, null, null)
        ImmutableArray.Create<DiagnosticDescriptor>(dummyDescriptor)

    let ConvertError(error: FSharpErrorInfo, location: Location) =
        let id = "FS" + error.ErrorNumber.ToString("0000")
        let emptyString = LocalizableString.op_Implicit("")
        let description = LocalizableString.op_Implicit(error.Message)
        let severity = if error.Severity = FSharpErrorSeverity.Error then DiagnosticSeverity.Error else DiagnosticSeverity.Warning
        let customTags = 
            match error.ErrorNumber with
            | 1182 -> DiagnosticCustomTags.Unnecessary
            | _ -> null
        let descriptor = new DiagnosticDescriptor(id, emptyString, description, error.Subcategory, severity, true, emptyString, String.Empty, customTags)
        Diagnostic.Create(descriptor, location)

    let FSharpGlyphToRoslynGlyph = function
        // FSROSLYNTODO: This doesn't yet reflect public/private/internal into the glyph
        // FSROSLYNTODO: We should really use FSharpSymbol information here. But GetDeclarationListInfo doesn't provide it, and switch to GetDeclarationListSymbols is a bit large at the moment
        | GlyphMajor.Class -> Glyph.ClassPublic
        | GlyphMajor.Constant -> Glyph.ConstantPublic
        | GlyphMajor.Delegate -> Glyph.DelegatePublic
        | GlyphMajor.Enum -> Glyph.EnumPublic
        | GlyphMajor.EnumMember -> Glyph.EnumMember
        | GlyphMajor.Event -> Glyph.EventPublic
        | GlyphMajor.Exception -> Glyph.ClassPublic
        | GlyphMajor.FieldBlue -> Glyph.FieldPublic
        | GlyphMajor.Interface -> Glyph.InterfacePublic
        | GlyphMajor.Method -> Glyph.MethodPublic
        | GlyphMajor.Method2 -> Glyph.ExtensionMethodPublic
        | GlyphMajor.Module -> Glyph.ModulePublic
        | GlyphMajor.NameSpace -> Glyph.Namespace
        | GlyphMajor.Property -> Glyph.PropertyPublic
        | GlyphMajor.Struct -> Glyph.StructurePublic
        | GlyphMajor.Typedef -> Glyph.ClassPublic
        | GlyphMajor.Type -> Glyph.ClassPublic
        | GlyphMajor.Union -> Glyph.EnumPublic
        | GlyphMajor.Variable -> Glyph.Local
        | GlyphMajor.ValueType -> Glyph.StructurePublic
        | GlyphMajor.Error -> Glyph.Error
        | _ -> Glyph.ClassPublic

    let inline (|Public|Internal|Protected|Private|) (a: FSharpAccessibility) =
        if a.IsPublic then Public
        elif a.IsInternal then Internal
        elif a.IsPrivate then Private
        else Protected

    let GetGlyphForSymbol (symbol: FSharpSymbol, kind: LexerSymbolKind) =
        match kind with
        | LexerSymbolKind.Operator -> Glyph.Operator
        | _ ->
            match symbol with
            | :? FSharpUnionCase as x ->
                match x.Accessibility with
                | Public -> Glyph.EnumPublic
                | Internal -> Glyph.EnumInternal
                | Protected -> Glyph.EnumProtected
                | Private -> Glyph.EnumPrivate
            | :? FSharpActivePatternCase -> Glyph.EnumPublic
            | :? FSharpField as x ->
            if x.IsLiteral then
                match x.Accessibility with
                | Public -> Glyph.ConstantPublic
                | Internal -> Glyph.ConstantInternal
                | Protected -> Glyph.ConstantProtected
                | Private -> Glyph.ConstantPrivate
            else
                match x.Accessibility with
                | Public -> Glyph.FieldPublic
                | Internal -> Glyph.FieldInternal
                | Protected -> Glyph.FieldProtected
                | Private -> Glyph.FieldPrivate
            | :? FSharpParameter -> Glyph.Parameter
            | :? FSharpMemberOrFunctionOrValue as x ->
                if x.LiteralValue.IsSome then
                    match x.Accessibility with
                    | Public -> Glyph.ConstantPublic
                    | Internal -> Glyph.ConstantInternal
                    | Protected -> Glyph.ConstantProtected
                    | Private -> Glyph.ConstantPrivate
                elif x.IsExtensionMember then
                    match x.Accessibility with
                    | Public -> Glyph.ExtensionMethodPublic
                    | Internal -> Glyph.ExtensionMethodInternal
                    | Protected -> Glyph.ExtensionMethodProtected
                    | Private -> Glyph.ExtensionMethodPrivate
                elif x.IsProperty || x.IsPropertyGetterMethod || x.IsPropertySetterMethod then
                    match x.Accessibility with
                    | Public -> Glyph.PropertyPublic
                    | Internal -> Glyph.PropertyInternal
                    | Protected -> Glyph.PropertyProtected
                    | Private -> Glyph.PropertyPrivate
                elif x.IsEvent then
                    match x.Accessibility with
                    | Public -> Glyph.EventPublic
                    | Internal -> Glyph.EventInternal
                    | Protected -> Glyph.EventProtected
                    | Private -> Glyph.EventPrivate
                else
                    match x.Accessibility with
                    | Public -> Glyph.MethodPublic
                    | Internal -> Glyph.MethodInternal
                    | Protected -> Glyph.MethodProtected
                    | Private -> Glyph.MethodPrivate
            | :? FSharpEntity as x ->
                if x.IsValueType then
                    match x.Accessibility with
                    | Public -> Glyph.StructurePublic
                    | Internal -> Glyph.StructureInternal
                    | Protected -> Glyph.StructureProtected
                    | Private -> Glyph.StructurePrivate
                elif x.IsFSharpModule then
                    match x.Accessibility with
                    | Public -> Glyph.ModulePublic
                    | Internal -> Glyph.ModuleInternal
                    | Protected -> Glyph.ModuleProtected
                    | Private -> Glyph.ModulePrivate
                elif x.IsEnum || x.IsFSharpUnion then
                    match x.Accessibility with
                    | Public -> Glyph.EnumPublic
                    | Internal -> Glyph.EnumInternal
                    | Protected -> Glyph.EnumProtected
                    | Private -> Glyph.EnumPrivate
                elif x.IsInterface then
                    match x.Accessibility with
                    | Public -> Glyph.InterfacePublic
                    | Internal -> Glyph.InterfaceInternal
                    | Protected -> Glyph.InterfaceProtected
                    | Private -> Glyph.InterfacePrivate
                elif x.IsDelegate then
                    match x.Accessibility with
                    | Public -> Glyph.DelegatePublic
                    | Internal -> Glyph.DelegateInternal
                    | Protected -> Glyph.DelegateProtected
                    | Private -> Glyph.DelegatePrivate
                elif x.IsNamespace then
                    Glyph.Namespace
                else
                    match x.Accessibility with
                    | Public -> Glyph.ClassPublic
                    | Internal -> Glyph.ClassInternal
                    | Protected -> Glyph.ClassProtected
                    | Private -> Glyph.ClassPrivate
            | _ -> Glyph.None

    let RangeToLocation (r: range, sourceText: SourceText, filePath: string) : Location =
        let linePositionSpan = LinePositionSpan(LinePosition(Line.toZ r.StartLine, r.StartColumn), LinePosition(Line.toZ r.EndLine, r.EndColumn))
        let textSpan = sourceText.Lines.GetTextSpan linePositionSpan
        Location.Create(filePath, textSpan, linePositionSpan)

[<AutoOpen>]
module internal RoslynExtensions =
    type Project with
        /// The list of all other projects within the same solution that reference this project.
        member this.GetDependentProjects() =
            this.Solution.GetProjectDependencyGraph().GetProjectsThatDirectlyDependOnThisProject(this.Id)
            |> Seq.map this.Solution.GetProject