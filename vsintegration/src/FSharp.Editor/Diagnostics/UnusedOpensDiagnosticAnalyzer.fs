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

module internal SymbolUse =
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
        | MemberFunctionOrValue symbol when 
            symbol.IsModuleValueOrMember  &&
            not symbolUse.IsFromPattern &&
            not symbol.IsOperatorOrActivePattern &&
            not symbol.IsPropertyGetterMethod &&
            not symbol.IsPropertySetterMethod -> Some symbol
        | _ -> None

    let (|Function|_|) (symbolUse:FSharpSymbolUse) =
        match symbolUse with
        | MemberFunctionOrValue symbol when 
            notCtorOrProp symbol  &&
            symbol.IsModuleValueOrMember &&
            not symbol.IsOperatorOrActivePattern &&
            not symbolUse.IsFromPattern ->
            
            match symbol.FullTypeSafe with
            | Some fullType when fullType.IsFunctionType -> Some symbol
            | _ -> None
        | _ -> None

    let (|Operator|_|) (symbolUse:FSharpSymbolUse) =
        match symbolUse with
        | MemberFunctionOrValue symbol when 
            notCtorOrProp symbol &&
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
        | MemberFunctionOrValue symbol when 
            notCtorOrProp symbol &&
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
        | Entity ent when ent.IsAttributeType -> Some ent
        | _ -> None

module internal UnusedOpens =
    open Microsoft.CodeAnalysis.Text

    let visitModulesAndNamespaces modulesOrNss =
        [ for moduleOrNs in modulesOrNss do
            let SynModuleOrNamespace(decls = decls) = moduleOrNs

            for decl in decls do
                match decl with
                | SynModuleDecl.Open(longIdentWithDots, range) -> 
                    yield (longIdentWithDots.Lid |> List.map(fun l -> l.idText) |> String.concat "."), range
                | _ -> () ]

    let getOpenStatements = function
        | ParsedInput.ImplFile implFile ->
            let (ParsedImplFileInput(_, _, _, _, _, modules, _)) = implFile in visitModulesAndNamespaces modules
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

    let getUnusedOpens (sourceText: SourceText) (parsedInput: ParsedInput) (symbolUses: FSharpSymbolUse[]) =

        let getPartNamespace (sym:FSharpSymbolUse) (fullName:string option) =
            // given a symbol range such as `Text.ISegment` and a full name
            // of `MonoDevelop.Core.Text.ISegment`, return `MonoDevelop.Core`
            fullName |> Option.bind(fun fullName ->
                let length = sym.RangeAlternate.EndColumn - sym.RangeAlternate.StartColumn
                let lengthDiff = fullName.Length - length - 2
                Some fullName.[0..lengthDiff])

        let getPossibleNamespaces (sym: FSharpSymbolUse) =
            let isQualified = symbolIsFullyQualified sourceText sym
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
            symbolUses
            |> Seq.filter (fun (s: FSharpSymbolUse) -> not s.IsFromDefinition)
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

        parsedInput
        |> getOpenStatements
        |> filter
        |> List.map snd

[<DiagnosticAnalyzer(FSharpCommonConstants.FSharpLanguageName)>]
type internal UnusedOpensDiagnosticAnalyzer() =
    inherit DocumentDiagnosticAnalyzer()
    
    let getProjectInfoManager (document: Document) = document.Project.Solution.Workspace.Services.GetService<FSharpCheckerWorkspaceService>().ProjectInfoManager
    let getChecker (document: Document) = document.Project.Solution.Workspace.Services.GetService<FSharpCheckerWorkspaceService>().Checker

    static let Descriptor = 
        DiagnosticDescriptor(
            IDEDiagnosticIds.RemoveUnnecessaryImportsDiagnosticId, 
            SR.RemoveUnusedOpens.Value, 
            SR.UnusedOpens.Value, 
            DiagnosticCategory.Style, 
            DiagnosticSeverity.Hidden, 
            true, 
            "", 
            "", 
            DiagnosticCustomTags.Unnecessary)

    override __.SupportedDiagnostics = ImmutableArray.Create Descriptor
    override this.AnalyzeSyntaxAsync(_, _) = Task.FromResult ImmutableArray<Diagnostic>.Empty

    override this.AnalyzeSemanticsAsync(document: Document, cancellationToken: CancellationToken) =
        asyncMaybe {
            let! options = getProjectInfoManager(document).TryGetOptionsForEditingDocumentOrProject(document)
            let! sourceText = document.GetTextAsync()
            let checker = getChecker document
            let! _, parsedInput, checkResults = checker.ParseAndCheckDocument(document, options, sourceText = sourceText, allowStaleResults = true)
            let! symbolUses = checkResults.GetAllUsesOfAllSymbolsInFile() |> liftAsync
            let unusedOpens = UnusedOpens.getUnusedOpens sourceText parsedInput symbolUses
            
            return 
                (unusedOpens
                 |> List.map (fun m ->
                      Diagnostic.Create(
                         Descriptor,
                         CommonRoslynHelpers.RangeToLocation(m, sourceText, document.FilePath)))
                ).ToImmutableArray()
        } 
        |> Async.map (Option.defaultValue ImmutableArray.Empty)
        |> CommonRoslynHelpers.StartAsyncAsTask cancellationToken

    interface IBuiltInAnalyzer with
        member __.OpenFileOnly _ = true
        member __.GetAnalyzerCategory() = DiagnosticAnalyzerCategory.SemanticDocumentAnalysis