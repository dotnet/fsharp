// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.CodeAnalysis.Declarations.DeclarationSyntax

open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Text
open System.Threading

open FSharp.Compiler
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.GraphChecking
open FSharp.Compiler.ParseAndCheckInputs
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Xml

/// The source each parse was made from, for hashing a declaration's text.
let private sources = ConditionalWeakTable<FSharpParseFileResults, ISourceText>()

let sourceOf (parse: FSharpParseFileResults) =
    match sources.TryGetValue parse with
    | true, s -> Some s
    | _ -> None

/// Each parse's copy of the project's diagnostic options, holding the #nowarn scopes that parse recorded.
let private warnOptions =
    ConditionalWeakTable<FSharpParseFileResults, FSharpDiagnosticOptions>()

let warnOptionsOf (parse: FSharpParseFileResults) =
    match warnOptions.TryGetValue parse with
    | true, options -> options
    | _ ->
        invalidArg
            (nameof parse)
            "Only a parse made by DeclarationChecker.Parse can be checked: its #nowarn scopes are the ones it recorded."

/// Whether diagnostics filtered and rendered with one parse's options are those another parse's give: the same
/// warning options and #nowarn scopes.
let sameWarnOptions (a: FSharpDiagnosticOptions) (b: FSharpDiagnosticOptions) =
    obj.ReferenceEquals(a, b)
    || (a.WarnLevel = b.WarnLevel
        && a.GlobalWarnAsError = b.GlobalWarnAsError
        && a.WarnOff = b.WarnOff
        && a.WarnOn = b.WarnOn
        && a.WarnAsError = b.WarnAsError
        && a.WarnAsWarn = b.WarnAsWarn
        && (match a.WarnScopeData, b.WarnScopeData with
            | None, None -> true
            | Some x, Some y -> obj.ReferenceEquals(x, y) || x.Equals y
            | _ -> false))

/// The #line directives the process holds for a file: those of its latest parse anywhere in the process. Checking
/// reads them, and so do the results it builds.
let private currentLineDirectives (fileName: string) =
    LineDirectives.ofFile (FileIndex.fileIndexOfFile fileName)

/// The #line directives of each parse, read right after it, under the file's parse lock.
let private lineDirectives =
    ConditionalWeakTable<FSharpParseFileResults, (int * (int * int)) list>()

let lineDirectivesOf (parse: FSharpParseFileResults) =
    match lineDirectives.TryGetValue parse with
    | true, directives -> directives
    | _ -> invalidArg (nameof parse) "Only a parse made by DeclarationChecker.Parse can be checked."

/// Raises SupersededLineDirectivesException unless the process's #line directives of the file are still the parse's. A
/// check reads them, and rendering a diagnostic applies them: a newer parse of another text of the file replaces
/// them, and that parse's are wrong for this one.
let refuseUnlessCurrentDirectives (parse: FSharpParseFileResults) =
    if currentLineDirectives parse.FileName <> lineDirectivesOf parse then
        raise (
            SupersededLineDirectivesException
                $"%s{parse.FileName} cannot be checked: a newer parse of the file replaced the line directives of the text that was parsed."
        )

/// The end of a file's last fragment.
let fileEndOf (parse: FSharpParseFileResults) =
    match parse.ParseTree with
    | ParsedInput.ImplFile file when not file.Contents.IsEmpty -> (List.last file.Contents).Range.End
    | _ -> parse.ParseTree.Range.End

let private textOfRange (source: ISourceText) (m: range) (docRanges: range list) =
    let sb = StringBuilder()
    let lastLine = min (m.EndLine - 1) (source.GetLineCount() - 1)

    let lineDocSpans (line1: int) (first: int) (last: int) =
        let spans = ResizeArray<int * int>()

        for dr in docRanges do
            if dr.StartLine <= line1 && line1 <= dr.EndLine then
                let s = if dr.StartLine = line1 then dr.StartColumn else 0
                let e = if dr.EndLine = line1 then dr.EndColumn else Int32.MaxValue
                let c0 = max first (min last s)
                let c1 = max first (min last e)

                if c0 < c1 then
                    spans.Add(c0, c1)

        if spans.Count = 0 then
            [| (first, last) |], false
        else
            spans.Sort(fun (a, _) (b, _) -> compare a b)
            let nonDocs = ResizeArray<int * int>()
            let mutable cur = first

            for (s, e) in spans do
                if s > cur then
                    nonDocs.Add(cur, s)

                cur <- max cur e

            if cur < last then
                nonDocs.Add(cur, last)

            nonDocs.ToArray(), true

    for line in m.StartLine - 1 .. lastLine do
        let line1 = line + 1
        let text = source.GetLineString line

        let first =
            if line = m.StartLine - 1 then
                min m.StartColumn text.Length
            else
                0

        let last =
            if line = m.EndLine - 1 then
                min m.EndColumn text.Length
            else
                text.Length

        let nonDocs, hasDoc = lineDocSpans line1 first last

        if not hasDoc then
            sb.Append(text, first, max 0 (last - first)).Append('\n') |> ignore
        else
            let lineSb = StringBuilder()

            for (c0, c1) in nonDocs do
                if c0 < c1 && c0 < text.Length then
                    let len = min (c1 - c0) (text.Length - c0)
                    lineSb.Append(text, c0, len) |> ignore

            let nonDocText = lineSb.ToString()

            if String.IsNullOrWhiteSpace nonDocText then
                sb.Append('\n') |> ignore
            else
                sb.Append(nonDocText).Append('\n') |> ignore

    sb.ToString()

let private docRangesOfDecl (decl: SynModuleDecl) : range list =
    let docRanges = ResizeArray<range>()

    let addDoc (doc: PreXmlDoc) =
        if not doc.IsEmpty then
            let m = doc.Range

            if not (Position.posEq m.Start m.End) then
                docRanges.Add m

    let rec walkField (SynField(xmlDoc = doc)) = addDoc doc

    and walkMember (m: SynMemberDefn) =
        match m with
        | SynMemberDefn.Member(memberDefn = b) -> walkBinding b
        | SynMemberDefn.GetSetMember(memberDefnForGet = getOpt; memberDefnForSet = setOpt) ->
            getOpt |> Option.iter walkBinding
            setOpt |> Option.iter walkBinding
        | SynMemberDefn.ImplicitCtor(xmlDoc = doc) -> addDoc doc
        | SynMemberDefn.AbstractSlot(slotSig = SynValSig(xmlDoc = doc)) -> addDoc doc
        | SynMemberDefn.AutoProperty(xmlDoc = doc) -> addDoc doc
        | SynMemberDefn.Interface(members = Some ms) -> ms |> List.iter walkMember
        | SynMemberDefn.ValField(fieldInfo = f) -> walkField f
        | SynMemberDefn.LetBindings(bindings = bs) -> bs |> List.iter walkBinding
        | SynMemberDefn.NestedType(typeDefn = t) -> walkTypeDefn t
        | _ -> ()

    and walkTypeDefn (SynTypeDefn(typeInfo = SynComponentInfo(xmlDoc = doc); typeRepr = repr; members = ms; implicitConstructor = ctor)) =
        addDoc doc
        ctor |> Option.iter walkMember

        match repr with
        | SynTypeDefnRepr.ObjectModel(members = oms) -> oms |> List.iter walkMember
        | SynTypeDefnRepr.Simple(simpleRepr = s) ->
            match s with
            | SynTypeDefnSimpleRepr.Union(unionCases = cases) ->
                for SynUnionCase(xmlDoc = cdoc; caseType = ctype) in cases do
                    addDoc cdoc

                    match ctype with
                    | SynUnionCaseKind.Fields cfields -> cfields |> List.iter walkField
                    | _ -> ()
            | SynTypeDefnSimpleRepr.Record(recordFieldsAndSpreads = rfs) ->
                for f in rfs do
                    match f with
                    | SynFieldOrSpread.Field field -> walkField field
                    | _ -> ()
            | SynTypeDefnSimpleRepr.Enum(cases = ecases) ->
                for SynEnumCase(xmlDoc = edoc) in ecases do
                    addDoc edoc
            | _ -> ()
        | _ -> ()

        ms |> List.iter walkMember

    and walkBinding (SynBinding(xmlDoc = doc; expr = expr)) =
        addDoc doc
        walkExpr expr

    and walkExpr (expr: SynExpr) =
        match expr with
        | SynExpr.Paren(expr = e)
        | SynExpr.Quote(quotedExpr = e)
        | SynExpr.Typed(expr = e)
        | SynExpr.ArrayOrListComputed(expr = e)
        | SynExpr.ComputationExpr(expr = e)
        | SynExpr.Do(expr = e)
        | SynExpr.Assert(expr = e)
        | SynExpr.AddressOf(expr = e)
        | SynExpr.InferredDowncast(expr = e)
        | SynExpr.InferredUpcast(expr = e)
        | SynExpr.DotGet(expr = e)
        | SynExpr.YieldOrReturn(expr = e)
        | SynExpr.YieldOrReturnFrom(expr = e)
        | SynExpr.DoBang(expr = e)
        | SynExpr.Downcast(expr = e)
        | SynExpr.Upcast(expr = e)
        | SynExpr.TypeTest(expr = e)
        | SynExpr.Lazy(expr = e)
        | SynExpr.TraitCall(argExpr = e)
        | SynExpr.IndexFromEnd(expr = e)
        | SynExpr.Lambda(body = e) -> walkExpr e
        | SynExpr.Tuple(exprs = es)
        | SynExpr.ArrayOrList(exprs = es) -> es |> List.iter walkExpr
        | SynExpr.Sequential(expr1 = e1; expr2 = e2)
        | SynExpr.App(funcExpr = e1; argExpr = e2)
        | SynExpr.TryFinally(tryExpr = e1; finallyExpr = e2)
        | SynExpr.While(whileExpr = e1; doExpr = e2)
        | SynExpr.DotSet(targetExpr = e1; rhsExpr = e2)
        | SynExpr.Set(targetExpr = e1; rhsExpr = e2) ->
            walkExpr e1
            walkExpr e2
        | SynExpr.IfThenElse(ifExpr = g; thenExpr = t; elseExpr = eOpt) ->
            walkExpr g
            walkExpr t
            eOpt |> Option.iter walkExpr
        | SynExpr.LetOrUse synLetOrUse ->
            synLetOrUse.Bindings |> List.iter walkBinding
            walkExpr synLetOrUse.Body
        | SynExpr.Match(expr = e; clauses = cs)
        | SynExpr.MatchBang(expr = e; clauses = cs) ->
            walkExpr e

            for SynMatchClause(whenExpr = wOpt; resultExpr = r) in cs do
                wOpt |> Option.iter walkExpr
                walkExpr r
        | SynExpr.TryWith(tryExpr = t; withCases = cs) ->
            walkExpr t

            for SynMatchClause(whenExpr = wOpt; resultExpr = r) in cs do
                wOpt |> Option.iter walkExpr
                walkExpr r
        | SynExpr.For(identBody = b; toBody = t) ->
            walkExpr b
            walkExpr t
        | SynExpr.ForEach(enumExpr = e; bodyExpr = b) ->
            walkExpr e
            walkExpr b
        | SynExpr.Record(copyInfo = copyOpt; recordFields = rfs) ->
            copyOpt |> Option.iter (fst >> walkExpr)

            for rf in rfs do
                match rf with
                | SynExprRecordFieldOrSpread.Field(SynExprRecordField(expr = eOpt), _) -> eOpt |> Option.iter walkExpr
                | SynExprRecordFieldOrSpread.Spread(SynExprSpread(expr = e), _) -> walkExpr e
        | SynExpr.AnonRecd(copyInfo = copyOpt; recordFields = rfs) ->
            copyOpt |> Option.iter (fst >> walkExpr)

            for rf in rfs do
                match rf with
                | SynExprAnonRecordFieldOrSpread.Field(SynExprAnonRecordField(expr = e), _) -> walkExpr e
                | SynExprAnonRecordFieldOrSpread.Spread(SynExprSpread(expr = e), _) -> walkExpr e
        | SynExpr.ObjExpr(bindings = bs; members = ms; argOptions = aOpt) ->
            aOpt |> Option.iter (fst >> walkExpr)
            bs |> List.iter walkBinding
            ms |> List.iter walkMember
        | SynExpr.DotIndexedGet(objectExpr = o; indexArgs = a) ->
            walkExpr o
            walkExpr a
        | SynExpr.DotIndexedSet(objectExpr = o; indexArgs = a; valueExpr = v) ->
            walkExpr o
            walkExpr a
            walkExpr v
        | SynExpr.IndexRange(expr1 = e1; expr2 = e2) ->
            e1 |> Option.iter walkExpr
            e2 |> Option.iter walkExpr
        | SynExpr.MatchLambda(matchClauses = cs) ->
            for SynMatchClause(whenExpr = wOpt; resultExpr = r) in cs do
                wOpt |> Option.iter walkExpr
                walkExpr r
        | SynExpr.TypeApp(expr = e)
        | SynExpr.New(expr = e)
        | SynExpr.DotLambda(expr = e)
        | SynExpr.DebugPoint(innerExpr = e)
        | SynExpr.Fixed(expr = e)
        | SynExpr.LongIdentSet(expr = e)
        | SynExpr.FromParseError(expr = e)
        | SynExpr.DiscardAfterMissingQualificationAfterDot(expr = e)
        | SynExpr.LibraryOnlyUnionCaseFieldGet(expr = e) -> walkExpr e
        | SynExpr.WhileBang(whileExpr = e1; doExpr = e2)
        | SynExpr.Dynamic(funcExpr = e1; argExpr = e2)
        | SynExpr.JoinIn(lhsExpr = e1; rhsExpr = e2)
        | SynExpr.NamedIndexedPropertySet(expr1 = e1; expr2 = e2)
        | SynExpr.LibraryOnlyStaticOptimization(expr = e1; optimizedExpr = e2)
        | SynExpr.LibraryOnlyUnionCaseFieldSet(expr = e1; rhsExpr = e2) ->
            walkExpr e1
            walkExpr e2
        | SynExpr.SequentialOrImplicitYield(expr1 = e1; expr2 = e2; ifNotStmt = e3)
        | SynExpr.DotNamedIndexedPropertySet(targetExpr = e1; argExpr = e2; rhsExpr = e3) ->
            walkExpr e1
            walkExpr e2
            walkExpr e3
        | SynExpr.LibraryOnlyILAssembly(args = es) -> es |> List.iter walkExpr
        | SynExpr.InterpolatedString(contents = parts) ->
            for part in parts do
                match part with
                | SynInterpolatedStringPart.FillExpr(fillExpr = e; formatting = f) ->
                    walkExpr e

                    match f with
                    | SynInterpolationFormatting.DotNet(alignment = Some a) -> walkExpr a
                    | _ -> ()
                | _ -> ()
        | _ -> ()

    let rec walkDecl (d: SynModuleDecl) =
        match d with
        | SynModuleDecl.Let(bindings = bs) -> bs |> List.iter walkBinding
        | SynModuleDecl.Types(typeDefns = ts) -> ts |> List.iter walkTypeDefn
        | SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo(xmlDoc = doc); decls = ds) ->
            addDoc doc
            ds |> List.iter walkDecl
        | SynModuleDecl.Exception(
            exnDefn = SynExceptionDefn(
                exnRepr = SynExceptionDefnRepr(caseName = SynUnionCase(xmlDoc = caseDoc); xmlDoc = reprDoc); members = ms)) ->
            addDoc reprDoc
            addDoc caseDoc
            ms |> List.iter walkMember
        | SynModuleDecl.NamespaceFragment(SynModuleOrNamespace(xmlDoc = doc; decls = ds)) ->
            addDoc doc
            ds |> List.iter walkDecl
        | SynModuleDecl.Expr(expr = e) -> walkExpr e
        | _ -> ()

    walkDecl decl
    List.ofSeq docRanges

let private longIdentText (ids: Ident list) =
    ids |> List.map (fun id -> id.idText) |> String.concat "."

let rec private patNames (pat: SynPat) =
    match pat with
    | SynPat.Named(ident = SynIdent(id, _)) -> [ id.idText ]
    | SynPat.LongIdent(longDotId = SynLongIdent(id = ids)) when not ids.IsEmpty -> [ (List.last ids).idText ]
    | SynPat.Typed(pat = pat)
    | SynPat.Paren(pat = pat)
    | SynPat.Attrib(pat = pat)
    | SynPat.As(rhsPat = pat) -> patNames pat
    | SynPat.ListCons(lhsPat = l; rhsPat = r)
    | SynPat.Or(lhsPat = l; rhsPat = r) -> patNames l @ patNames r
    | SynPat.Tuple(elementPats = pats)
    | SynPat.ArrayOrList(elementPats = pats)
    | SynPat.Ands(pats = pats) -> List.collect patNames pats
    | SynPat.Record(fieldPats = fps) -> fps |> List.collect (fun (NamePatPairField(pat = p)) -> patNames p)
    | SynPat.OptionalVal(ident = id) -> [ id.idText ]
    | SynPat.Wild _ -> [ "_" ]
    | _ -> []

let private typeDefnName (SynTypeDefn(typeInfo = info)) = longIdentText info.LongIdent

let private isAugmentation (SynTypeDefn(typeRepr = repr)) =
    match repr with
    | SynTypeDefnRepr.ObjectModel(kind = SynTypeDefnKind.Augmentation _) -> true
    | _ -> false

let private nameAndKind (decl: SynModuleDecl) =
    match decl with
    | SynModuleDecl.Let(isRecursive = isRec; bindings = binds) ->
        let names = binds |> List.collect (fun (SynBinding(headPat = pat)) -> patNames pat)

        String.Join(",", names),
        (if isRec then
             DeclarationKind.RecGroup
         else
             DeclarationKind.Value)
    | SynModuleDecl.Types([ typeDefn ], _) ->
        typeDefnName typeDefn,
        (if isAugmentation typeDefn then
             DeclarationKind.Augmentation
         else
             DeclarationKind.Type)
    | SynModuleDecl.Types(typeDefns, _) ->
        (match typeDefns with
         | typeDefn :: _ -> typeDefnName typeDefn
         | [] -> ""),
        DeclarationKind.RecGroup
    | SynModuleDecl.Exception(SynExceptionDefn(SynExceptionDefnRepr(caseName = SynUnionCase(ident = SynIdent(id, _))), _, _, _), _) ->
        id.idText, DeclarationKind.Exception
    | SynModuleDecl.NestedModule(moduleInfo = info; isRecursive = isRec) ->
        longIdentText info.LongIdent,
        (if isRec then
             DeclarationKind.RecGroup
         else
             DeclarationKind.Module)
    | SynModuleDecl.Open(target, _) ->
        (match target with
         | SynOpenDeclTarget.ModuleOrNamespace(SynLongIdent(id = ids), _) -> longIdentText ids
         | SynOpenDeclTarget.Type(SynType.LongIdent(SynLongIdent(id = ids)), _) -> "type " + longIdentText ids
         | SynOpenDeclTarget.Type _ -> "type"),
        DeclarationKind.Open
    | SynModuleDecl.ModuleAbbrev(ident = id) -> id.idText, DeclarationKind.ModuleAbbrev
    | SynModuleDecl.Attributes(attrs, _) ->
        attrs
        |> List.collect (fun list -> list.Attributes)
        |> List.map (fun attr -> longIdentText attr.TypeName.LongIdent)
        |> String.concat ",",
        DeclarationKind.Attribute
    | SynModuleDecl.Expr _ -> "", DeclarationKind.Do
    | SynModuleDecl.HashDirective(ParsedHashDirective(ident = ident), _) -> "#" + ident, DeclarationKind.Do
    | SynModuleDecl.NamespaceFragment(SynModuleOrNamespace(longId = ids; isRecursive = isRec)) ->
        longIdentText ids,
        (if isRec then
             DeclarationKind.RecGroup
         else
             DeclarationKind.Module)

/// Whether the declarations of the file are those of its only fragment; otherwise they are the fragments,
/// and recursive ones are one declaration.
let splitsFragment (file: ParsedImplFileInput) =
    match file.Contents with
    | [ SynModuleOrNamespace(isRecursive = false) ] -> true
    | _ -> false

/// Whether the file is several fragments with an augmentation anywhere inside them. Such a file is checked whole, as
/// `module rec` is: a fragment is one row, so a type and its augmentation in different fragments are never tied,
/// and one fragment holding both is replayed with the type already augmented.
let private fragmentsHoldAugmentation (file: ParsedImplFileInput) =
    let rec holds (decls: SynModuleDecl list) =
        decls
        |> List.exists (fun decl ->
            match decl with
            | SynModuleDecl.Types(typeDefns, _) -> List.exists isAugmentation typeDefns
            | SynModuleDecl.NestedModule(decls = nested) -> holds nested
            | SynModuleDecl.NamespaceFragment(SynModuleOrNamespace(decls = nested)) -> holds nested
            | _ -> false)

    file.Contents.Length > 1
    && file.Contents
       |> List.exists (fun (SynModuleOrNamespace(decls = decls)) -> holds decls)

/// The fully qualified names of the entities a file declares where another file can declare the same name (ruling
/// R19), and the namespaces it puts them in: its top-level modules, and the modules, types, exceptions and module
/// abbreviations it declares directly in a namespace. Adding a file to the checker state combines these with every
/// earlier file's (CombineCcuContentFragments), which reports two modules or types of one name (FS0037, FS0248,
/// FS0249), and a module with a namespace's name (FS0247), whether or not either file uses the other. Types of
/// one name and different arity can coexist; they share an entry here, which only adds edges.
let private declaredNames (input: ParsedInput) =
    let entities = ResizeArray<string>()
    let namespaces = ResizeArray<string>()
    let pathOf (ids: LongIdent) = ids |> List.map _.idText

    let addNamespaces (path: string list) =
        for n in 1 .. path.Length do
            namespaces.Add(String.concat "." (List.truncate n path))

    let add (path: string list) (name: string) =
        entities.Add(String.concat "." (path @ [ name ]))

    // A module may compile as XModule: next to a type X, or under [<CompilationRepresentation(ModuleSuffix)>]
    let addModule (path: string list) (name: string) =
        add path name
        add path (name + "Module")

    let addTop (path: string list) (isModule: bool) (inNamespace: string list -> unit) =
        if isModule then
            addNamespaces (List.truncate (max 0 (path.Length - 1)) path)
            entities.Add(String.concat "." path)
            entities.Add(String.concat "." path + "Module")
        else
            addNamespaces path
            inNamespace path

    let lastName (ids: LongIdent) =
        match List.tryLast ids with
        | Some id -> id.idText
        | None -> ""

    match input with
    | ParsedInput.ImplFile file ->
        for SynModuleOrNamespace(longId = ids; kind = kind; decls = decls) in file.Contents do
            addTop (pathOf ids) kind.IsModule (fun path ->
                for decl in decls do
                    match decl with
                    | SynModuleDecl.NestedModule(moduleInfo = moduleInfo) -> addModule path (longIdentText moduleInfo.LongIdent)
                    | SynModuleDecl.Types(typeDefns, _) ->
                        for SynTypeDefn(typeInfo = typeInfo) as typeDefn in typeDefns do
                            if not (isAugmentation typeDefn) then
                                add path (lastName typeInfo.LongIdent)
                    | SynModuleDecl.Exception(SynExceptionDefn(SynExceptionDefnRepr(caseName = SynUnionCase(ident = SynIdent(id, _))),
                                                               _,
                                                               _,
                                                               _),
                                              _) ->
                        add path id.idText
                        add path (id.idText + "Exception")
                    | SynModuleDecl.ModuleAbbrev(ident = id) -> add path id.idText
                    | _ -> ())
    | ParsedInput.SigFile file ->
        for SynModuleOrNamespaceSig(longId = ids; kind = kind; decls = decls) in file.Contents do
            addTop (pathOf ids) kind.IsModule (fun path ->
                for decl in decls do
                    match decl with
                    | SynModuleSigDecl.NestedModule(moduleInfo = moduleInfo) -> addModule path (longIdentText moduleInfo.LongIdent)
                    | SynModuleSigDecl.Types(typeDefns, _) ->
                        for SynTypeDefnSig(typeInfo = typeInfo; typeRepr = repr) in typeDefns do
                            match repr with
                            | SynTypeDefnSigRepr.ObjectModel(kind = SynTypeDefnKind.Augmentation _) -> ()
                            | _ -> add path (lastName typeInfo.LongIdent)
                    | SynModuleSigDecl.Exception(SynExceptionSig(
                                                     exnRepr = SynExceptionDefnRepr(caseName = SynUnionCase(ident = SynIdent(id, _)))),
                                                 _) ->
                        add path id.idText
                        add path (id.idText + "Exception")
                    | SynModuleSigDecl.ModuleAbbrev(ident = id) -> add path id.idText
                    | _ -> ())

    entities, namespaces

/// The names of the types among declarations, as TypeNamesInNonMutRecDecls collects them for module suffixes.
/// It also counts types whose type parameters fail that function's early measure check, so it can only hold more.
let private typeNamesIn (decls: SynModuleDecl list) =
    set
        [
            for decl in decls do
                match decl with
                | SynModuleDecl.Types(typeDefns, _) ->
                    for SynTypeDefn(typeInfo = typeInfo; typeRepr = repr) in typeDefns do
                        match repr, List.tryLast typeInfo.LongIdent with
                        | SynTypeDefnRepr.ObjectModel(kind = SynTypeDefnKind.Augmentation _), _ -> ()
                        | _, Some id -> yield id.idText
                        | _, None -> ()
                | _ -> ()
        ]

/// Whether every later declaration depends on this one. For a module or fragment checked whole, that holds when
/// anything in it would, except an open, whose scope ends with the module.
let rec private affectsAllLater (decl: SynModuleDecl) =
    let inner (decl: SynModuleDecl) =
        match decl with
        | SynModuleDecl.Open _ -> false
        | _ -> affectsAllLater decl

    match decl with
    | SynModuleDecl.Open _
    | SynModuleDecl.ModuleAbbrev _
    | SynModuleDecl.Attributes _
    | SynModuleDecl.Expr _
    | SynModuleDecl.HashDirective _ -> true
    | SynModuleDecl.Types _ -> false
    // Any attribute: [<AutoOpen>] can go by another name
    | SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo(attributes = attributes); decls = decls) ->
        not attributes.IsEmpty || List.exists inner decls
    | SynModuleDecl.NamespaceFragment(SynModuleOrNamespace(attribs = attributes; decls = decls)) ->
        not attributes.IsEmpty || List.exists inner decls
    | SynModuleDecl.Let _
    | SynModuleDecl.Exception _ -> false

/// The uses in the entries of a declaration's syntax.
let private usesOf (entries: FileContentEntry list) =
    let uses = HashSet<string>()
    let members = HashSet<string>()
    let fields = HashSet<string>()
    let mutable unresolvable = false

    let rec visit (entry: FileContentEntry) =
        match entry with
        | FileContentEntry.Name name
        | FileContentEntry.ModuleName name -> uses.Add name |> ignore
        | FileContentEntry.OpenStatement path -> uses.UnionWith path
        | FileContentEntry.MemberName name -> members.Add name |> ignore
        | FileContentEntry.FieldOrCaseName name -> fields.Add name |> ignore
        | FileContentEntry.Unresolvable -> unresolvable <- true
        | FileContentEntry.TopLevelNamespace(content = content)
        | FileContentEntry.NestedModule(nestedContent = content) -> List.iter visit content
        // Its segments are names already, except after a dot on an expression, where they are members
        | FileContentEntry.PrefixedIdentifier _ -> ()

    List.iter visit entries
    uses, members, fields, unresolvable

let private sorted (names: seq<string>) =
    names
    |> Seq.distinct
    |> Seq.toArray
    |> Array.sortWith (fun a b -> String.CompareOrdinal(a, b))

let parse (ctx: ProjectContext) (fileName: string) (source: string) : FSharpParseFileResults =
    let state = ctx.State

    let parsingOptions =
        FSharpParsingOptions.FromTcConfig(state.tcConfig, state.projectOptions.SourceFiles, false)

    let sourceText = SourceText.ofString source

    // Lexing records the file's #nowarn scopes in the project's diagnostic options, replacing the last parse's:
    // copy them while no other parse of this file can replace them. The latest parse of the file is recorded under
    // the same lock, so it is the one whose scopes were copied last. Its #line directives are read the same way: a
    // parse of the file on another context can still replace them in between.
    lock (ctx.ParseLock fileName) (fun () ->
        let parseErrors, parseTree, anyErrors =
            ParseAndCheckFile.parseFile (
                sourceText,
                fileName,
                parsingOptions,
                userOpName,
                suggestNamesForErrors,
                state.tcConfig.flatErrors,
                captureIdentifiersWhenParsing,
                CancellationToken.None
            )

        let options = parsingOptions.DiagnosticOptions

        let warnOptionsCopy =
            lock options (fun () ->
                { options with
                    WarnScopeData = options.WarnScopeData
                })

        let parse = FSharpParseFileResults(parseErrors, parseTree, anyErrors, [||])
        sources.Add(parse, sourceText)
        warnOptions.Add(parse, warnOptionsCopy)
        lineDirectives.Add(parse, currentLineDirectives fileName)
        ctx.SetLatestParse parse
        parse)

/// How many declarations Split gives for the parse, counted from the syntax tree alone: nothing is copied or keyed.
let countDeclarations (parse: FSharpParseFileResults) : int =
    match parse.ParseTree with
    | ParsedInput.SigFile _ -> invalidArg (nameof parse) "Only implementation files are checked one declaration at a time."
    | ParsedInput.ImplFile file ->
        // As Split: a named module that is not recursive is its header and its declarations, anything else one
        let rec count (decls: SynModuleDecl list) =
            decls
            |> List.sumBy (fun decl ->
                match decl with
                | SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo(synType = Some _); isRecursive = false; decls = moduleDecls) ->
                    1 + count moduleDecls
                | _ -> 1)

        if splitsFragment file then
            let (SynModuleOrNamespace(decls = decls)) = List.exactlyOne file.Contents
            count decls
        elif fragmentsHoldAugmentation file then
            1
        else
            file.Contents.Length

let split (parse: FSharpParseFileResults) : ParsedDeclaration[] =
    match parse.ParseTree with
    | ParsedInput.SigFile _ -> invalidArg (nameof parse) "Only implementation files are checked one declaration at a time."
    | ParsedInput.ImplFile file ->
        let modulePath, endm, decls =
            if splitsFragment file then
                let (SynModuleOrNamespace(longId = longId; decls = decls; range = m)) =
                    List.exactlyOne file.Contents

                longId |> List.map (fun id -> id.idText), m.EndRange, decls
            else
                [], file.QualifiedName.Range, [ for fragment in file.Contents -> SynModuleDecl.NamespaceFragment fragment ]

        let source =
            match sources.TryGetValue parse with
            | true, source -> source
            | _ ->
                invalidArg
                    (nameof parse)
                    "Only a parse made by DeclarationChecker.Parse can be split: its declarations' text comes from the source it recorded."

        let seen =
            Dictionary<string list * string * DeclarationKind, int>(HashIdentity.Structural)

        let options = warnOptionsOf parse
        let checkXmlDocs = options.CheckXmlDocs

        let declaration modulePath within siblings depth (decl: SynModuleDecl) (range: range) scopem headerInputs isModuleHeader =
            let name, kind = nameAndKind decl

            let ordinal =
                match seen.TryGetValue((modulePath, name, kind)) with
                | true, n -> n
                | _ -> 0

            seen[(modulePath, name, kind)] <- ordinal + 1
            let docRanges = if checkXmlDocs then [] else docRangesOfDecl decl
            let text = textOfRange source range docRanges

            let key =
                {
                    ModulePath = modulePath
                    Name = name
                    Kind = kind
                    Ordinal = ordinal
                }

            ParsedDeclaration(key, decl, range, scopem, text, headerInputs, within, siblings, depth, isModuleHeader, source)

        let rec splitDecls modulePath within depth (endm: range) (decls: SynModuleDecl list) =
            let typeNames = typeNamesIn decls

            // Whether the module gets the Module suffix for a type of its name among the declarations around it
            let suffixInput (moduleInfo: SynComponentInfo) =
                let ownName =
                    moduleInfo.LongIdent
                    |> List.tryLast
                    |> Option.map _.idText
                    |> Option.defaultValue ""

                $"suffix=%b{typeNames.Contains ownName}"

            let makeDecl modulePath within depth decl range scopem headerInputs isModuleHeader =
                declaration modulePath within decls depth decl range scopem headerInputs isModuleHeader

            decls
            |> List.mapi (fun i decl ->
                // As TcModuleOrNamespaceElementsNonMutRec: a declaration's scope runs from the next one to the end
                // of the enclosing module, namespace or file. This is the only place the end is computed.
                let scopem =
                    match List.tryItem (i + 1) decls with
                    | Some next -> unionRanges next.Range endm
                    | None -> unionRanges decl.Range endm

                match decl with
                // A module with a name, and not recursive: its header, then its declarations, as checking the
                // module checks them, with its end as theirs
                | SynModuleDecl.NestedModule(
                    moduleInfo = SynComponentInfo(synType = Some _) as moduleInfo
                    isRecursive = false
                    decls = moduleDecls
                    range = m
                    trivia = trivia) ->
                    let headerEnd =
                        match trivia.EqualsRange with
                        | Some equals -> equals.End
                        | None -> moduleInfo.Range.End

                    // What TcNestedModuleHeader and EnterNestedModuleDeclarations read besides the header's text
                    let headerInputs =
                        let bodyTypes =
                            typeNamesIn moduleDecls |> Seq.sortWith (fun a b -> String.CompareOrdinal(a, b))

                        let bodyTypesList = String.Join(",", bodyTypes)
                        $"%s{suffixInput moduleInfo};types=%s{bodyTypesList}"

                    let range = mkRange m.FileName m.Start headerEnd
                    let header = makeDecl modulePath within depth decl range scopem headerInputs true
                    let path = modulePath @ [ longIdentText moduleInfo.LongIdent ]
                    header :: splitDecls path (Some header.Key) (depth + 1) m.EndRange moduleDecls
                // A module checked whole (module rec, or without a name) reads its suffix the same way
                | SynModuleDecl.NestedModule(moduleInfo = moduleInfo) ->
                    [
                        makeDecl modulePath within depth decl decl.Range scopem (suffixInput moduleInfo) false
                    ]
                | _ -> [ makeDecl modulePath within depth decl decl.Range scopem "" false ])
            |> List.concat

        splitDecls modulePath None 0 endm decls |> Array.ofList

/// The names the declaration defines and uses. A module header has only its name and its attributes' uses.
let content (decl: ParsedDeclaration) : DeclarationContent =
    let entries, (defines, unreadableName) =
        match decl.Syntax with
        | SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo(attributes = attributes) as moduleInfo) when decl.IsModuleHeader ->
            FileContentMapping.visitSynAttributes attributes, ([ for id in moduleInfo.LongIdent -> id.idText ], false)
        | SynModuleDecl.NamespaceFragment(SynModuleOrNamespace(attribs = attributes; decls = decls)) ->
            FileContentMapping.visitSynAttributes attributes
            @ List.collect FileContentMapping.visitSynModuleDecl decls,
            TrieMapping.declarationNames decl.Syntax
        | syntax -> FileContentMapping.visitSynModuleDecl syntax, TrieMapping.declarationNames syntax

    let uses, members, fields, unresolvable = usesOf entries

    let affectsAllLaterFlag =
        match decl.Syntax with
        | SynModuleDecl.NestedModule(moduleInfo = SynComponentInfo(attributes = attributes)) when decl.IsModuleHeader ->
            not attributes.IsEmpty
        | syntax -> affectsAllLater syntax

    let augments =
        match decl.Syntax with
        | SynModuleDecl.Types(typeDefns, _) -> typeDefns |> List.filter isAugmentation |> List.map typeDefnName |> sorted
        | _ -> [||]

    {
        Defines = sorted defines
        Uses = sorted uses
        MemberUses = sorted members
        FieldOrCaseUses = sorted fields
        HasUnresolvable = unresolvable
        AffectsAllLater = affectsAllLaterFlag || unreadableName
        Augments = augments
    }

/// For each of the project's files, given in file order, the ascending indices of the earlier files checking it
/// reads: the file dependency graph graph-mode checking builds, transitively closed. A file that depends on a
/// signature file does not depend on its implementation file when their qualified names match: the setup adds the
/// signature's contents to the implementation environment instead, as graph mode's ArtificialImplFile node does
/// (see stateBefore). When they differ, it depends on the implementation file, as a check of every earlier file
/// reads it.
///
/// A file also depends on every earlier file that declares an entity of the same fully qualified name, or a
/// namespace of a module's name (ruling R19, see declaredNames): adding it to the checker state
/// reports the clash, whether or not either file uses the other.
///
/// `parses` must be one parse of each of the project's source files, in file order. BeginFile, CheckFile and
/// CheckSignatureFile accept the signatures of these files as `before`, in place of every earlier file, for these
/// parses, while each earlier one is still the latest parse of its file.
let fileDependencies (ctx: ProjectContext) (parses: FSharpParseFileResults[]) : int[][] =
    let sourceFiles = ctx.SourceFiles

    if
        parses.Length <> sourceFiles.Length
        || Array.exists2
            (fun (parse: FSharpParseFileResults) file -> not (String.Equals(parse.FileName, file, StringComparison.OrdinalIgnoreCase)))
            parses
            sourceFiles
    then
        raise (ArgumentException("Expected one parse of each of the project's source files, in file order.", nameof parses))

    let inputsAndModuleNames, _ =
        (Map.empty, parses)
        ||> Array.mapFold (fun moduleNamesDict parse ->
            let input, after = DeduplicateParsedInputModuleName moduleNamesDict parse.ParseTree
            (input, moduleNamesDict), after)

    let inputs, moduleNamesBefore = Array.unzip inputsAndModuleNames

    let files =
        inputs
        |> Array.mapi (fun idx input ->
            {
                Idx = idx
                FileName = input.FileName
                ParsedInput = input
            })

    let filePairs = FilePairMap files

    // The checker gives an implementation file its signature by qualified name, FilePairMap pairs them by file name
    let matched = HashSet<string>(StringComparer.OrdinalIgnoreCase)
    let unmatchedImplementations = HashSet<int>()

    for idx in 0 .. files.Length - 1 do
        if filePairs.IsSignature idx then
            let impl = filePairs.GetImplementationIndex idx

            if inputs[idx].QualifiedName.Text = inputs[impl].QualifiedName.Text then
                matched.Add parses[idx].FileName |> ignore
            else
                unmatchedImplementations.Add impl |> ignore

    // The graph leaves an implementation file with a signature file out of the trie: no later file reads it. An
    // unmatched one is checked as a file without a signature, and the files after it read its contents, so the
    // graph is given the pairs without it. Its own edge to the signature file is added back below.
    let graphPairs =
        if unmatchedImplementations.Count = 0 then
            filePairs
        else
            files
            |> Array.map (fun file ->
                if unmatchedImplementations.Contains file.Idx then
                    { file with
                        FileName = file.FileName + "|unmatched"
                    }
                else
                    file)
            |> FilePairMap

    let graph, _ = DependencyResolution.mkGraph graphPairs files

    // By name, the files declaring an entity of that name, and those with a namespace of that name
    let entityFiles =
        Dictionary<string, ResizeArray<int>>(StringComparer.OrdinalIgnoreCase)

    let namespaceFiles =
        Dictionary<string, ResizeArray<int>>(StringComparer.OrdinalIgnoreCase)

    let filesOf (table: Dictionary<string, ResizeArray<int>>) name =
        match table.TryGetValue name with
        | true, files -> files :> seq<int>
        | _ -> Seq.empty

    let record (table: Dictionary<string, ResizeArray<int>>) idx name =
        match table.TryGetValue name with
        | true, files -> files.Add idx
        | _ -> table[name] <- ResizeArray [ idx ]

    let names = inputs |> Array.map declaredNames

    names
    |> Array.iteri (fun idx (entities, namespaces) ->
        for name in entities do
            record entityFiles idx name

        for name in namespaces do
            record namespaceFiles idx name)

    let closed = Array.zeroCreate<int[]> files.Length

    for idx in 0 .. files.Length - 1 do
        let dependencies = HashSet<int>()
        let pending = Stack<int>(graph[idx])
        let entities, namespaces = names[idx]

        if unmatchedImplementations.Contains idx then
            pending.Push(filePairs.GetSignatureIndex idx)

        for name in entities do
            for other in Seq.append (filesOf entityFiles name) (filesOf namespaceFiles name) do
                pending.Push other

        for name in namespaces do
            for other in filesOf entityFiles name do
                pending.Push other

        while pending.Count > 0 do
            let dependency = pending.Pop()

            if dependency < idx && dependencies.Add dependency then
                for transitive in closed[dependency] do
                    pending.Push transitive

                // A pair whose names differ: the signature does not stand in for the implementation file
                if
                    filePairs.IsSignature dependency
                    && not (matched.Contains parses[dependency].FileName)
                then
                    pending.Push(filePairs.GetImplementationIndex dependency)

        closed[idx] <- dependencies |> Seq.sort |> Array.ofSeq

    ctx.Dependencies <-
        Some
            {
                Parses = Array.copy parses
                Dependencies = closed |> Array.map Array.copy
                ModuleNamesBefore = moduleNamesBefore
                MatchedSignatures = matched
            }

    closed
