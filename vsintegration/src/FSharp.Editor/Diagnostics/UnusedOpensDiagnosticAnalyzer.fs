// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.VisualStudio.FSharp.LanguageService

module internal Symbols =
    open Microsoft.CodeAnalysis.Text

    let getLocationFromSymbolUse (s: FSharpSymbolUse) =
        [s.Symbol.DeclarationLocation; s.Symbol.SignatureLocation]
        |> List.choose id
        |> List.distinctBy (fun r -> r.FileName)

    let getLocationFromSymbol (s:FSharpSymbol) =
        [s.DeclarationLocation; s.SignatureLocation]
        |> List.choose id
        |> List.distinctBy (fun r -> r.FileName)

    /// Given a column and line string returns the identifier portion of the string
    let lastIdent column lineString =
        let _, ident = QuickParse.GetPartialLongNameEx (lineString, column) in ident

    /// Returns a TextSegment that is trimmed to only include the identifier
    //let getTextSegment (sourceText: SourceText) (symbolUse: FSharpSymbolUse) column line =
    //    let lastIdent = lastIdent column line
    //    let start, finish = Symbol.trimSymbolRegion symbolUse lastIdent

    //    let startOffset = doc.LocationToOffset(start.Line, start.Column+1)
    //    let endOffset = doc.LocationToOffset(finish.Line, finish.Column+1)
    //    MonoDevelop.Core.Text.TextSegment.FromBounds(startOffset, endOffset)

    //let getEditorDataForFileName (fileName:string) =
    //    match IdeApp.Workbench.GetDocument (fileName) with
    //    | null ->
    //        let doc = Editor.TextEditorFactory.LoadDocument (fileName)
    //        let editor = new TextEditorData()
    //        editor.Text <- doc.Text
    //        editor
    //    | doc -> doc.Editor.GetContent<ITextEditorDataProvider>().GetTextEditorData()

    //let getOffsets (range:Range.range) (editor:Editor.IReadonlyTextDocument) =
    //    let startOffset = editor.LocationToOffset (range.StartLine, range.StartColumn+1)
    //    let endOffset = editor.LocationToOffset (range.EndLine, range.EndColumn+1)
    //    startOffset, endOffset

    //let getTextSpan (range:Range.range) (editor:Editor.IReadonlyTextDocument) =
    //    let startOffset, endOffset = getOffsets range editor
    //    Microsoft.CodeAnalysis.Text.TextSpan.FromBounds (startOffset, endOffset)

    /// We always know the text of the identifier that resolved to symbol.
    /// Trim the range of the referring text to only include this identifier.
    /// This means references like A.B.C are trimmed to "C".  This allows renaming to just rename "C".
    let trimSymbolRegion(symbolUse: FSharpSymbolUse) (lastIdentAtLoc: string) =
        let m = symbolUse.RangeAlternate
        let ((beginLine, beginCol), (endLine, endCol)) = ((m.StartLine, m.StartColumn), (m.EndLine, m.EndColumn))
    
        let (beginLine, beginCol) =
            if endCol >=lastIdentAtLoc.Length && (beginLine <> endLine || (endCol-beginCol) >= lastIdentAtLoc.Length) then
                (endLine,endCol-lastIdentAtLoc.Length)
            else
                (beginLine, beginCol)
        Range.mkPos beginLine beginCol, Range.mkPos endLine endCol

    let getTrimmedRangesForDeclarations lastIdent (symbolUse:FSharpSymbolUse) =
        symbolUse
        |> getLocationFromSymbolUse
        |> Seq.map (fun range ->
            let start, finish = trimSymbolRegion symbolUse lastIdent
            range.FileName, start, finish)

    //let getTrimmedOffsetsForDeclarations lastIdent (symbolUse:FSharpSymbolUse) =
    //    let trimmedSymbols = getTrimmedRangesForDeclarations lastIdent symbolUse
    //    trimmedSymbols
    //    |> Seq.map (fun (fileName, start, finish) ->
    //        let editor = getEditorDataForFileName fileName
    //        let startOffset = editor.LocationToOffset (start.Line, start.Column+1)
    //        let endOffset = editor.LocationToOffset (finish.Line, finish.Column+1)
    //        //if startOffset < 0 then argOutOfRange "startOffset" "broken"
    //        //if endOffset < 0  then argOutOfRange "endOffset" "broken"
    //        fileName, startOffset, endOffset)

    //let getTrimmedTextSpanForDeclarations lastIdent (symbolUse:FSharpSymbolUse) =
    //    let trimmedSymbols = getTrimmedRangesForDeclarations lastIdent symbolUse
    //    trimmedSymbols
    //    |> Seq.map (fun (fileName, start, finish) ->
    //        let editor = getEditorDataForFileName fileName
    //        let startOffset = editor.LocationToOffset (start.Line, start.Column+1)
    //        let endOffset = editor.LocationToOffset (finish.Line, finish.Column+1)
    //        let ts = Microsoft.CodeAnalysis.Text.TextSpan.FromBounds (startOffset, endOffset)
    //        let ls = Microsoft.CodeAnalysis.Text.LinePositionSpan(Microsoft.CodeAnalysis.Text.LinePosition(start.Line, start.Column),
    //                                                              Microsoft.CodeAnalysis.Text.LinePosition(finish.Line, finish.Column))
    //        fileName, ts, ls)

    //let getOffsetsTrimmed lastIdent (symbolUse:FSharpSymbolUse) =
    //    let filename = symbolUse.RangeAlternate.FileName
    //    let editor = getEditorDataForFileName filename
    //    let start, finish = Symbol.trimSymbolRegion symbolUse lastIdent
    //    let startOffset = editor.LocationToOffset (start.Line, start.Column+1)
    //    let endOffset = editor.LocationToOffset (finish.Line, finish.Column+1)
    //    filename, startOffset, endOffset

    //let getOffsetAndLength lastIdent (symbolUse:FSharpSymbolUse) =
    //    let editor = getEditorDataForFileName symbolUse.RangeAlternate.FileName
    //    let start, finish = Symbol.trimSymbolRegion symbolUse lastIdent
    //    let startOffset = editor.LocationToOffset (start.Line, start.Column+1)
    //    let endOffset = editor.LocationToOffset (finish.Line, finish.Column+1)
    //    startOffset, endOffset - startOffset

    //let getTextSpanTrimmed lastIdent (symbolUse:FSharpSymbolUse) =
    //    let filename, start, finish = getOffsetsTrimmed lastIdent symbolUse
    //    filename, TextSpan.FromBounds (start, finish)

module SymbolUse =
    let (|ActivePatternCase|_|) (symbol : FSharpSymbolUse) =
        match symbol.Symbol with
        | :? FSharpActivePatternCase as ap-> ActivePatternCase(ap) |> Some
        | _ -> None

    let (|Entity|_|) (symbol : FSharpSymbolUse) =
        match symbol.Symbol with
        | :? FSharpEntity as ent -> Some ent
        | _ -> None

    let (|Field|_|) (symbol : FSharpSymbolUse) =
        match symbol.Symbol with
        | :? FSharpField as field-> Some field
        |  _ -> None

    let (|GenericParameter|_|) (symbol: FSharpSymbolUse) =
        match symbol.Symbol with
        | :? FSharpGenericParameter as gp -> Some gp
        | _ -> None

    let (|MemberFunctionOrValue|_|) (symbol : FSharpSymbolUse) =
        match symbol.Symbol with
        | :? FSharpMemberOrFunctionOrValue as func -> Some func
        | _ -> None

    let (|ActivePattern|_|) = function
        | MemberFunctionOrValue m when m.IsActivePattern -> Some m | _ -> None

    let (|Parameter|_|) (symbol : FSharpSymbolUse) =
        match symbol.Symbol with
        | :? FSharpParameter as param -> Some param
        | _ -> None

    let (|StaticParameter|_|) (symbol : FSharpSymbolUse) =
        match symbol.Symbol with
        | :? FSharpStaticParameter as sp -> Some sp
        | _ -> None

    let (|UnionCase|_|) (symbol : FSharpSymbolUse) =
        match symbol.Symbol with
        | :? FSharpUnionCase as uc-> Some uc
        | _ -> None

    //let (|Constructor|_|) = function
    //    | MemberFunctionOrValue func when func.IsConstructor || func.IsImplicitConstructor -> Some func
    //    | _ -> None

    let (|TypeAbbreviation|_|) = function
        | Entity symbol when symbol.IsFSharpAbbreviation -> Some symbol
        | _ -> None

    let (|Class|_|) = function
        | Entity symbol when symbol.IsClass -> Some symbol
        | Entity s when s.IsFSharp &&
                        s.IsOpaque &&
                        not s.IsFSharpModule &&
                        not s.IsNamespace &&
                        not s.IsDelegate &&
                        not s.IsFSharpUnion &&
                        not s.IsFSharpRecord &&
                        not s.IsInterface &&
                        not s.IsValueType -> Some s
        | _ -> None

    let (|Delegate|_|) = function
        | Entity symbol when symbol.IsDelegate -> Some symbol
        | _ -> None

    let (|Event|_|) = function
        | MemberFunctionOrValue symbol when symbol.IsEvent -> Some symbol
        | _ -> None

    let (|Property|_|) = function
        | MemberFunctionOrValue symbol when symbol.IsProperty || symbol.IsPropertyGetterMethod || symbol.IsPropertySetterMethod -> Some symbol
        | _ -> None

    let inline private notCtorOrProp (symbol:FSharpMemberOrFunctionOrValue) =
        not symbol.IsConstructor && not symbol.IsPropertyGetterMethod && not symbol.IsPropertySetterMethod

    let (|Method|_|) (symbolUse:FSharpSymbolUse) =
        match symbolUse with
        | MemberFunctionOrValue symbol when symbol.IsModuleValueOrMember  &&
                                            not symbolUse.IsFromPattern &&
                                            not symbol.IsOperatorOrActivePattern &&
                                            not symbol.IsPropertyGetterMethod &&
                                            not symbol.IsPropertySetterMethod -> Some symbol
        | _ -> None

    let (|Function|_|) (symbolUse:FSharpSymbolUse) =
        match symbolUse with
        | MemberFunctionOrValue symbol when notCtorOrProp symbol  &&
                                            symbol.IsModuleValueOrMember &&
                                            not symbol.IsOperatorOrActivePattern &&
                                            not symbolUse.IsFromPattern ->
            match symbol.FullTypeSafe with
            | Some fullType when fullType.IsFunctionType -> Some symbol
            | _ -> None
        | _ -> None

    let (|Operator|_|) (symbolUse:FSharpSymbolUse) =
        match symbolUse with
        | MemberFunctionOrValue symbol when notCtorOrProp symbol &&
                                            not symbolUse.IsFromPattern &&
                                            not symbol.IsActivePattern &&
                                            symbol.IsOperatorOrActivePattern ->
            match symbol.FullTypeSafe with
            | Some fullType when fullType.IsFunctionType -> Some symbol
            | _ -> None
        | _ -> None

    let (|Pattern|_|) (symbolUse:FSharpSymbolUse) =
        match symbolUse with
        | MemberFunctionOrValue symbol when notCtorOrProp symbol &&
                                            not symbol.IsOperatorOrActivePattern &&
                                            symbolUse.IsFromPattern ->
            match symbol.FullTypeSafe with
            | Some fullType when fullType.IsFunctionType ->Some symbol
            | _ -> None
        | _ -> None


    let (|ClosureOrNestedFunction|_|) = function
        | MemberFunctionOrValue symbol when notCtorOrProp symbol &&
                                            not symbol.IsOperatorOrActivePattern &&
                                            not symbol.IsModuleValueOrMember ->
            match symbol.FullTypeSafe with
            | Some fullType when fullType.IsFunctionType -> Some symbol
            | _ -> None
        | _ -> None

    
    let (|Val|_|) = function
        | MemberFunctionOrValue symbol when notCtorOrProp symbol &&
                                            not symbol.IsOperatorOrActivePattern ->
            match symbol.FullTypeSafe with
            | Some _fullType -> Some symbol
            | _ -> None
        | _ -> None

    let (|Enum|_|) = function
        | Entity symbol when symbol.IsEnum -> Some symbol
        | _ -> None

    let (|Interface|_|) = function
        | Entity symbol when symbol.IsInterface -> Some symbol
        | _ -> None

    let (|Module|_|) = function
        | Entity symbol when symbol.IsFSharpModule -> Some symbol
        | _ -> None

    let (|Namespace|_|) = function
        | Entity symbol when symbol.IsNamespace -> Some symbol
        | _ -> None

    let (|Record|_|) = function
        | Entity symbol when symbol.IsFSharpRecord -> Some symbol
        | _ -> None

    let (|Union|_|) = function
        | Entity symbol when symbol.IsFSharpUnion -> Some symbol
        | _ -> None

    let (|ValueType|_|) = function
        | Entity symbol when symbol.IsValueType && not symbol.IsEnum -> Some symbol
        | _ -> None

    let (|ComputationExpression|_|) (symbol:FSharpSymbolUse) =
        if symbol.IsFromComputationExpression then Some symbol
        else None
        
    let (|Attribute|_|) = function
        | Entity ent ->
            if ent.AllBaseTypes
               |> Seq.exists (fun t ->
                                  if t.HasTypeDefinition then
                                      t.TypeDefinition.TryFullName
                                      |> Option.exists ((=) "System.Attribute" )
                                  else false)
            then Some ent
            else None
        | _ -> None


module highlightUnusedCode =
    open Microsoft.CodeAnalysis.Text

    let visitModulesAndNamespaces modulesOrNss =
        [ for moduleOrNs in modulesOrNss do
            let (SynModuleOrNamespace(_lid, _isRec, _isMod, decls, _xml, _attrs, _, _m)) = moduleOrNs

            for decl in decls do
                match decl with
                | SynModuleDecl.Open(longIdentWithDots, range) -> 
                    yield (longIdentWithDots.Lid |> List.map(fun l -> l.idText) |> String.concat "."), range
                | _ -> () ]

    let getOpenStatements (tree: ParsedInput option) = 
        match tree.Value with
        | ParsedInput.ImplFile(implFile) ->
            let (ParsedImplFileInput(_fn, _script, _name, _, _, modules, _)) = implFile
            visitModulesAndNamespaces modules
        | _ -> []

    let getAutoOpenAccessPath (ent:FSharpEntity) =
        // Some.Namespace+AutoOpenedModule+Entity

        // HACK: I can't see a way to get the EnclosingEntity of an Entity
        // Some.Namespace + Some.Namespace.AutoOpenedModule are both valid
        ent.TryFullName |> Option.bind(fun _ ->
            if (not ent.IsNamespace) && ent.QualifiedName.Contains "+" then 
                Some ent.QualifiedName.[0..ent.QualifiedName.IndexOf "+" - 1]
            else
                None)

    let entityNamespace (entOpt:FSharpEntity option) =
        match entOpt with
        | Some ent ->
            if ent.IsFSharpModule then
                [Some ent.QualifiedName; Some ent.LogicalName; Some ent.AccessPath]
            else
                [ent.Namespace; Some ent.AccessPath; getAutoOpenAccessPath ent]
        | None -> []

    let symbolIsFullyQualified (sourceText: SourceText) (sym: FSharpSymbolUse) (fullName: string option) =
        match fullName with
        | Some fullName ->
            sourceText.ToString(CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, sym.RangeAlternate)) = fullName
        | None -> true

    let getUnusedCode (context:DocumentContext) (editor:TextEditor) =
        let ast =
            maybe {
                let! ast = context.TryGetAst()
                let! pd = context.TryGetFSharpParsedDocument()
                return ast.ParseTree, pd
            }

        ast |> Option.bind (fun (tree, pd) ->
            let symbols = pd.AllSymbolsKeyed.Values

            let getPartNamespace (sym:FSharpSymbolUse) (fullName:string option) =
                // given a symbol range such as `Text.ISegment` and a full name
                // of `MonoDevelop.Core.Text.ISegment`, return `MonoDevelop.Core`
                fullName |> Option.bind(fun fullName ->
                    let length = sym.RangeAlternate.EndColumn - sym.RangeAlternate.StartColumn
                    let lengthDiff = fullName.Length - length - 2
                    Some fullName.[0..lengthDiff])

            let getPossibleNamespaces (sym: FSharpSymbolUse) =
                let isQualified = symbolIsFullyQualified editor sym
                match sym with
                | SymbolUse.Entity ent when not (isQualified ent.TryFullName) ->
                    getPartNamespace sym ent.TryFullName :: entityNamespace (Some ent)
                | SymbolUse.Field f when not (isQualified (Some f.FullName)) -> 
                    getPartNamespace sym (Some f.FullName) :: entityNamespace (Some f.DeclaringEntity)
                | SymbolUse.MemberFunctionOrValue mfv when not (isQualified (Some mfv.FullName)) -> 
                    getPartNamespace sym (Some mfv.FullName) :: entityNamespace (Some mfv.EnclosingEntity)
                | SymbolUse.Operator op when not (isQualified (Some op.FullName)) ->
                    getPartNamespace sym (Some op.FullName) :: entityNamespace op.EnclosingEntitySafe
                | SymbolUse.ActivePattern ap when not (isQualified (Some ap.FullName)) ->
                    getPartNamespace sym (Some ap.FullName) :: entityNamespace ap.EnclosingEntitySafe
                | SymbolUse.ActivePatternCase apc when not (isQualified (Some apc.FullName)) ->
                    getPartNamespace sym (Some apc.FullName) :: entityNamespace apc.Group.EnclosingEntity
                | SymbolUse.UnionCase uc when not (isQualified (Some uc.FullName)) ->
                    getPartNamespace sym (Some uc.FullName) :: entityNamespace (Some uc.ReturnType.TypeDefinition)
                | SymbolUse.Parameter p when not (isQualified (Some p.FullName)) ->
                    getPartNamespace sym (Some p.FullName) :: entityNamespace (Some p.Type.TypeDefinition)
                | _ -> [None]

            let namespacesInUse =
                symbols
                |> Seq.filter (fun s -> not s.IsFromDefinition)
                |> Seq.collect getPossibleNamespaces
                |> Seq.choose id
                |> Set.ofSeq

            let filter list: (string * Range.range) list =
                let rec filterInner acc list (seenNamespaces: Set<string>) = 
                    let notUsed namespc =
                        not (namespacesInUse.Contains namespc) || seenNamespaces.Contains namespc

                    match list with 
                    | (namespc, range)::xs when notUsed namespc -> 
                        filterInner ((namespc, range)::acc) xs (seenNamespaces.Add namespc)
                    | (namespc, _)::xs ->
                        filterInner acc xs (seenNamespaces.Add namespc)
                    | [] -> acc |> List.rev
                filterInner [] list Set.empty

            let openStatements = getOpenStatements tree
            openStatements |> List.map snd //|> removeMarkers editor

            let results =
                let opens = (openStatements |> filter) |> List.map snd
                //opens |> List.append (pd.UnusedCodeRanges |> Option.defaultValue [])

            Some results)

    let highlightUnused (editor:TextEditor) (unusedOpenRanges: Range.range list) (previousUnused: Range.range list)=
        previousUnused |> removeMarkers editor

        unusedOpenRanges |> List.iter(fun range ->
            let startOffset = getOffset editor range.Start
            let markers = editor.GetTextSegmentMarkersAt startOffset |> Seq.toList
            if markers.Length = 0 then
                let endOffset = getOffset editor range.End

                let segment = new Text.TextSegment(startOffset, endOffset - startOffset)
                let marker = TextMarkerFactory.CreateGenericTextSegmentMarker(editor, TextSegmentMarkerEffect.GrayOut, segment)
                marker.IsVisible <- true

                editor.AddMarker(marker))

type HighlightUnusedCode() =
    inherit TextEditorExtension()
    let mutable previousUnused = []
    override x.Initialize() =
        let parsed = x.DocumentContext.DocumentParsed
        parsed.Add(fun _ ->
                        let unused = highlightUnusedCode.getUnusedCode x.DocumentContext x.Editor

                        unused |> Option.iter(fun unused' ->
                        highlightUnusedCode.highlightUnused x.Editor unused' previousUnused
                        previousUnused <- unused'))