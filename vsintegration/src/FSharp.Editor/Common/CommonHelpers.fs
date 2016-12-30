// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.SourceCodeServices.ItemDescriptionIcons

module internal CommonHelpers =
    /// Fix invalid span if it appears to have redundant suffix and prefix.
    let fixupSpan (sourceText: SourceText, span: TextSpan) : TextSpan =
        let text = sourceText.GetSubText(span).ToString()
        match text.LastIndexOf '.' with
        | -1 | 0 -> span
        | index -> TextSpan(span.Start + index + 1, text.Length - index - 1)

    let glyphMajorToRoslynGlyph = function
        | GlyphMajor.Class
        | GlyphMajor.Typedef
        | GlyphMajor.Type
        | GlyphMajor.Exception -> Glyph.ClassPublic
        | GlyphMajor.Constant -> Glyph.ConstantPublic
        | GlyphMajor.Delegate -> Glyph.DelegatePublic
        | GlyphMajor.Union
        | GlyphMajor.Enum -> Glyph.EnumPublic
        | GlyphMajor.EnumMember
        | GlyphMajor.Variable
        | GlyphMajor.FieldBlue -> Glyph.FieldPublic
        | GlyphMajor.Event -> Glyph.EventPublic
        | GlyphMajor.Interface -> Glyph.InterfacePublic
        | GlyphMajor.Method
        | GlyphMajor.Method2 -> Glyph.MethodPublic
        | GlyphMajor.Module -> Glyph.ModulePublic
        | GlyphMajor.NameSpace -> Glyph.Namespace
        | GlyphMajor.Property -> Glyph.PropertyPublic
        | GlyphMajor.Struct
        | GlyphMajor.ValueType -> Glyph.StructurePublic
        | GlyphMajor.Error -> Glyph.Error
        | _ -> Glyph.None

    let compilerTokenToRoslynToken(colorKind: FSharpTokenColorKind) : string = 
        match colorKind with
        | FSharpTokenColorKind.Comment -> ClassificationTypeNames.Comment
        | FSharpTokenColorKind.Identifier -> ClassificationTypeNames.Identifier
        | FSharpTokenColorKind.Keyword -> ClassificationTypeNames.Keyword
        | FSharpTokenColorKind.String -> ClassificationTypeNames.StringLiteral
        | FSharpTokenColorKind.Text -> ClassificationTypeNames.Text
        | FSharpTokenColorKind.UpperIdentifier -> ClassificationTypeNames.Identifier
        | FSharpTokenColorKind.Number -> ClassificationTypeNames.NumericLiteral
        | FSharpTokenColorKind.InactiveCode -> ClassificationTypeNames.ExcludedCode 
        | FSharpTokenColorKind.PreprocessorKeyword -> ClassificationTypeNames.PreprocessorKeyword 
        | FSharpTokenColorKind.Operator -> ClassificationTypeNames.Operator
        | FSharpTokenColorKind.TypeName  -> ClassificationTypeNames.ClassName
        | FSharpTokenColorKind.Default 
        | _ -> ClassificationTypeNames.Text

[<RequireQualifiedAccess; NoComparison>] 
type internal SymbolDeclarationLocation = 
    | CurrentDocument
    | Projects of Project list * isLocalForProject: bool

[<AutoOpen>]
module internal Extensions =
    open System
    open System.IO

    type System.IServiceProvider with
        member x.GetService<'T>() = x.GetService(typeof<'T>) :?> 'T
        member x.GetService<'S, 'T>() = x.GetService(typeof<'S>) :?> 'T

    type Path with
        static member GetFullPathSafe path =
            try Path.GetFullPath path
            with _ -> path

    type FSharpChecker with
        member this.ParseAndCheckDocument(document: Document, options: FSharpProjectOptions) : Async<(Ast.ParsedInput * FSharpCheckFileResults) option> =
            async {
                let! cancellationToken = Async.CancellationToken
                let! sourceText = document.GetTextAsync()
                let! textVersion = document.GetTextVersionAsync(cancellationToken)
                let! parseResults, checkFileAnswer = this.ParseAndCheckFileInProject(document.FilePath, textVersion.GetHashCode(), sourceText.ToString(), options)
                return
                    match parseResults.ParseTree, checkFileAnswer with
                    | _, FSharpCheckFileAnswer.Aborted 
                    | None, _ -> None
                    | Some parsedInput, FSharpCheckFileAnswer.Succeeded checkResults -> Some (parsedInput, checkResults)
            }

    type FSharpSymbol with
        member this.IsInternalToProject =
            match this with 
            | :? FSharpParameter -> true
            | :? FSharpMemberOrFunctionOrValue as m -> not m.IsModuleValueOrMember || not m.Accessibility.IsPublic
            | :? FSharpEntity as m -> not m.Accessibility.IsPublic
            | :? FSharpGenericParameter -> true
            | :? FSharpUnionCase as m -> not m.Accessibility.IsPublic
            | :? FSharpField as m -> not m.Accessibility.IsPublic
            | _ -> false

    type FSharpSymbolUse with
        member this.GetDeclarationLocation (currentDocument: Document) : SymbolDeclarationLocation option =
            if this.IsPrivateToFile then
                Some SymbolDeclarationLocation.CurrentDocument
            else
                let isSymbolLocalForProject = this.Symbol.IsInternalToProject
                
                let declarationLocation = 
                    match this.Symbol.ImplementationLocation with
                    | Some x -> Some x
                    | None -> this.Symbol.DeclarationLocation
                
                match declarationLocation with
                | Some loc ->
                    let filePath = Path.GetFullPathSafe loc.FileName
                    let isScript = String.Equals(Path.GetExtension(filePath), ".fsx", StringComparison.OrdinalIgnoreCase)
                    if isScript && filePath = currentDocument.FilePath then 
                        Some SymbolDeclarationLocation.CurrentDocument
                    elif isScript then
                        // The standalone script might include other files via '#load'
                        // These files appear in project options and the standalone file 
                        // should be treated as an individual project
                        Some (SymbolDeclarationLocation.Projects ([currentDocument.Project], isSymbolLocalForProject))
                    else
                        let projects =
                            currentDocument.Project.Solution.GetDocumentIdsWithFilePath(currentDocument.FilePath)
                            |> Seq.map (fun x -> x.ProjectId)
                            |> Seq.distinct
                            |> Seq.map currentDocument.Project.Solution.GetProject
                            |> Seq.toList
                        match projects with
                        | [] -> None
                        | projects -> Some (SymbolDeclarationLocation.Projects (projects, isSymbolLocalForProject))
                | None -> None

        member this.IsPrivateToFile = 
            let isPrivate =
                match this.Symbol with
                | :? FSharpMemberOrFunctionOrValue as m -> not m.IsModuleValueOrMember
                | :? FSharpEntity as m -> m.Accessibility.IsPrivate
                | :? FSharpGenericParameter -> true
                | :? FSharpUnionCase as m -> m.Accessibility.IsPrivate
                | :? FSharpField as m -> m.Accessibility.IsPrivate
                | _ -> false
            
            let declarationLocation =
                match this.Symbol.SignatureLocation with
                | Some x -> Some x
                | _ ->
                    match this.Symbol.DeclarationLocation with
                    | Some x -> Some x
                    | _ -> this.Symbol.ImplementationLocation
            
            let declaredInTheFile = 
                match declarationLocation with
                | Some declRange -> declRange.FileName = this.RangeAlternate.FileName
                | _ -> false
            
            isPrivate && declaredInTheFile
    
    