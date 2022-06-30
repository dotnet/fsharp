// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.EditorServices

open Internal.Utilities.Library
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Xml

/// Represent an Xml documentation block in source code
type XmlDocable = XmlDocable of line: int * indent: int * paramNames: string list

module XmlDocParsing =

    let (|ConstructorPats|) =
        function
        | SynArgPats.Pats ps -> ps
        | SynArgPats.NamePatPairs (pats = xs) -> List.map (fun (_, _, pat) -> pat) xs

    let rec digNamesFrom pat =
        match pat with
        | SynPat.As (_, SynPat.Named (SynIdent (id, _), _isTheThisVar, _access, _range), _)
        | SynPat.Named (SynIdent (id, _), _isTheThisVar, _access, _range) -> [ id.idText ]
        | SynPat.Typed (pat, _type, _range) -> digNamesFrom pat
        | SynPat.Attrib (pat, _attrs, _range) -> digNamesFrom pat
        | SynPat.LongIdent(argPats = ConstructorPats pats) -> pats |> List.collect digNamesFrom
        | SynPat.Tuple (_, pats, _range) -> pats |> List.collect digNamesFrom
        | SynPat.Paren (pat, _range) -> digNamesFrom pat
        | SynPat.OptionalVal (id, _) -> [ id.idText ]
        | SynPat.As _ // no one uses as in fun decls
        | SynPat.Or _ // no one uses ors in fun decls
        | SynPat.Ands _ // no one uses ands in fun decls
        | SynPat.ArrayOrList _ // no one uses this in fun decls
        | SynPat.Record _ // no one uses this in fun decls
        | SynPat.Null _
        | SynPat.Const _
        | SynPat.Wild _
        | SynPat.IsInst _
        | SynPat.QuoteExpr _
        | SynPat.DeprecatedCharRange _
        | SynPat.InstanceMember _
        | SynPat.FromParseError _ -> []

    let getParamNames binding =
        let (SynBinding (valData = synValData; headPat = synPat)) = binding

        match synValData with
        | SynValData (_, SynValInfo (curriedArgs, _), _) when not curriedArgs.IsEmpty ->
            let parameters =
                [
                    for args in curriedArgs do
                        for (SynArgInfo (_, _, ident)) in args do
                            match ident with
                            | Some ident -> ident.idText
                            | None -> ()
                ]

            match parameters with
            | [] ->
                let paramNames = digNamesFrom synPat
                paramNames
            | _ :: _ -> parameters
        | _ -> []

    let getXmlDocablesImpl (sourceText: ISourceText, input: ParsedInput) =
        let indentOf (lineNum: int) =
            let mutable i = 0
            // -1 because lineNum reported by xmldocs are 1-based, but array is 0-based
            let line = sourceText.GetLineString(lineNum - 1)

            while i < line.Length && line.Chars(i) = ' ' do
                i <- i + 1

            i

        let isEmptyXmlDoc (preXmlDoc: PreXmlDoc) = preXmlDoc.ToXmlDoc(false, None).IsEmpty

        let rec getXmlDocablesSynModuleDecl decl =
            [
                match decl with
                | SynModuleDecl.NestedModule (decls = decls) ->
                    for decl in decls do
                        yield! getXmlDocablesSynModuleDecl decl
                | SynModuleDecl.Let (_, synBindingList, range) ->
                    let anyXmlDoc =
                        synBindingList
                        |> List.exists (fun (SynBinding (xmlDoc = preXmlDoc)) -> not (isEmptyXmlDoc preXmlDoc))

                    if not anyXmlDoc then
                        let synAttributes =
                            synBindingList |> List.collect (fun (SynBinding (attributes = a)) -> a)

                        let fullRange = synAttributes |> List.fold (fun r a -> unionRanges r a.Range) range
                        let line = fullRange.StartLine
                        let indent = indentOf line

                        let paramNames =
                            [
                                for binding in synBindingList do
                                    yield! getParamNames binding
                            ]

                        yield XmlDocable(line, indent, paramNames)
                | SynModuleDecl.Types (typeDefns, _) ->
                    for typeDefn in typeDefns do
                        yield! getXmlDocablesSynTypeDefn typeDefn
                | SynModuleDecl.NamespaceFragment (synModuleOrNamespace) -> yield! getXmlDocablesSynModuleOrNamespace synModuleOrNamespace
                | SynModuleDecl.ModuleAbbrev _
                | SynModuleDecl.Expr _
                | SynModuleDecl.Exception _
                | SynModuleDecl.Open _
                | SynModuleDecl.Attributes _
                | SynModuleDecl.HashDirective _ -> ()
            ]

        and getXmlDocablesSynModuleOrNamespace inp =
            let (SynModuleOrNamespace (decls = decls)) = inp
            decls |> List.collect getXmlDocablesSynModuleDecl

        and getXmlDocablesSynTypeDefn typeDefn =
            let (SynTypeDefn (typeInfo = typeInfo; typeRepr = synTypeDefnRepr; members = synMemberDefns; range = tRange)) =
                typeDefn

            let (SynComponentInfo (attributes = synAttributes; xmlDoc = preXmlDoc; range = compRange)) =
                typeInfo

            [
                match synTypeDefnRepr with
                | SynTypeDefnRepr.ObjectModel (_, extraMemberDefns, _) ->
                    for extraMemberDefn in extraMemberDefns do
                        yield! getXmlDocablesSynMemberDefn extraMemberDefn
                | SynTypeDefnRepr.Simple _ -> ()
                | SynTypeDefnRepr.Exception _ -> ()

                if isEmptyXmlDoc preXmlDoc then
                    let fullRange =
                        synAttributes
                        |> List.fold (fun r a -> unionRanges r a.Range) (unionRanges compRange tRange)

                    let line = fullRange.StartLine
                    let indent = indentOf line
                    XmlDocable(line, indent, [])
                for memberDefn in synMemberDefns do
                    yield! getXmlDocablesSynMemberDefn memberDefn
            ]

        and getXmlDocablesSynMemberDefn inp =
            [
                match inp with
                | SynMemberDefn.Member (memberBinding, memRange) ->
                    let (SynBinding (attributes = synAttributes; xmlDoc = preXmlDoc; headPat = synPat)) =
                        memberBinding

                    if isEmptyXmlDoc preXmlDoc then
                        let fullRange =
                            synAttributes |> List.fold (fun r a -> unionRanges r a.Range) memRange

                        let line = fullRange.StartLine
                        let indent = indentOf line
                        let paramNames = digNamesFrom synPat
                        XmlDocable(line, indent, paramNames)

                | SynMemberDefn.GetSetMember (getBinding, setBinding, m, _) ->
                    yield!
                        (Option.map (fun b -> SynMemberDefn.Member(b, m))
                         >> Option.toList
                         >> List.collect getXmlDocablesSynMemberDefn)
                            getBinding

                    yield!
                        (Option.map (fun b -> SynMemberDefn.Member(b, m))
                         >> Option.toList
                         >> List.collect getXmlDocablesSynMemberDefn)
                            setBinding

                | SynMemberDefn.AbstractSlot (valSig, _, range) ->
                    let (SynValSig (attributes = synAttributes; arity = synValInfo; xmlDoc = preXmlDoc)) =
                        valSig

                    if isEmptyXmlDoc preXmlDoc then
                        let fullRange = synAttributes |> List.fold (fun r a -> unionRanges r a.Range) range
                        let line = fullRange.StartLine
                        let indent = indentOf line
                        let paramNames = synValInfo.ArgNames
                        XmlDocable(line, indent, paramNames)

                | SynMemberDefn.Interface (members = synMemberDefnsOption) ->
                    match synMemberDefnsOption with
                    | None -> ()
                    | Some (xs) ->
                        for x in xs do
                            yield! getXmlDocablesSynMemberDefn x

                | SynMemberDefn.NestedType (synTypeDefn, _, _) -> yield! getXmlDocablesSynTypeDefn synTypeDefn

                | SynMemberDefn.AutoProperty (attributes = synAttributes; range = range) ->
                    let fullRange = synAttributes |> List.fold (fun r a -> unionRanges r a.Range) range
                    let line = fullRange.StartLine
                    let indent = indentOf line
                    XmlDocable(line, indent, [])

                | SynMemberDefn.Open _
                | SynMemberDefn.ImplicitCtor _
                | SynMemberDefn.ImplicitInherit _
                | SynMemberDefn.Inherit _
                | SynMemberDefn.ValField _
                | SynMemberDefn.LetBindings _ -> ()
            ]

        and getXmlDocablesInput input =
            match input with
            | ParsedInput.ImplFile (ParsedImplFileInput (modules = symModules)) ->
                symModules |> List.collect getXmlDocablesSynModuleOrNamespace
            | ParsedInput.SigFile _ -> []

        // Get compiler options for the 'project' implied by a single script file
        getXmlDocablesInput input

module XmlDocComment =
    let ws (s: string, pos) =
        let res = s.TrimStart()
        Some(res, pos + (s.Length - res.Length))

    let str (prefix: string) (s: string, pos) =
        match s.StartsWithOrdinal(prefix) with
        | true ->
            let res = s.Substring prefix.Length
            Some(res, pos + (s.Length - res.Length))
        | _ -> None

    let eol (s: string, pos) =
        match s with
        | "" -> Some("", pos)
        | _ -> None

    let (>=>) f g = f >> Option.bind g

    // if it's a blank XML comment with trailing "<", returns Some (index of the "<"), otherwise returns None
    let IsBlank (s: string) =
        let parser = ws >=> str "///" >=> ws >=> str "<" >=> eol
        let res = parser (s.TrimEnd(), 0) |> Option.map snd |> Option.map (fun x -> x - 1)
        res

module XmlDocParser =

    /// Get the list of Xml documentation from current source code
    let GetXmlDocables (sourceText: ISourceText, input) =
        XmlDocParsing.getXmlDocablesImpl (sourceText, input)
