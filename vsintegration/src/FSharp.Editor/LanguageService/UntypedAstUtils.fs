// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information
namespace Microsoft.VisualStudio.FSharp.Editor
open System
open System.Collections.Generic

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Ast

[<AutoOpen>]
module internal UntypedAstUtils =
    open Microsoft.FSharp.Compiler.Range

    type Range.range with
        member inline x.IsEmpty = x.StartColumn = x.EndColumn && x.StartLine = x.EndLine 

    type internal ShortIdent = string
    type internal Idents = ShortIdent[]

    let internal longIdentToArray (longIdent: Ident list): Idents =
        longIdent |> Seq.map string |> Seq.toArray   


    /// An recursive pattern that collect all sequential expressions to avoid StackOverflowException
    let rec (|Sequentials|_|) = function
        | SynExpr.Sequential(_, _, e, Sequentials es, _) ->
            Some(e::es)
        | SynExpr.Sequential(_, _, e1, e2, _) ->
            Some [e1; e2]
        | _ -> None

    let (|ConstructorPats|) = function
        | SynConstructorArgs.Pats ps -> ps
        | SynConstructorArgs.NamePatPairs(xs, _) -> List.map snd xs


    /// Get path to containing module/namespace of a given position
    let getModuleOrNamespacePath (pos: pos) (ast: ParsedInput) =
        let idents =
            match ast with
            | ParsedInput.ImplFile (ParsedImplFileInput(_, _, _, _, _, modules, _)) ->
                let rec walkModuleOrNamespace idents (decls, moduleRange) =
                    decls
                    |> List.fold (fun acc ->
                        function
                        | SynModuleDecl.NestedModule (componentInfo, _, nestedModuleDecls, _, nestedModuleRange) ->
                            if rangeContainsPos moduleRange pos then
                                let (ComponentInfo(_,_,_,longIdent,_,_,_,_)) = componentInfo
                                walkModuleOrNamespace (longIdent::acc) (nestedModuleDecls, nestedModuleRange)
                            else acc
                        | _ -> acc) idents

                modules
                |> List.fold (fun acc (SynModuleOrNamespace(longIdent, _, _, decls, _, _, _, moduleRange)) ->
                        if rangeContainsPos moduleRange pos then
                            walkModuleOrNamespace (longIdent::acc) (decls, moduleRange) @ acc
                        else acc) []
            | ParsedInput.SigFile(ParsedSigFileInput(_, _, _, _, modules)) ->
                let rec walkModuleOrNamespaceSig idents (decls, moduleRange) =
                    decls
                    |> List.fold (fun acc ->
                        function
                        | SynModuleSigDecl.NestedModule (componentInfo, _, nestedModuleDecls, nestedModuleRange) ->
                            if rangeContainsPos moduleRange pos then
                                let (ComponentInfo(_,_,_,longIdent,_,_,_,_)) = componentInfo
                                walkModuleOrNamespaceSig (longIdent::acc) (nestedModuleDecls, nestedModuleRange)
                            else acc
                        | _ -> acc) idents

                modules
                |> List.fold (fun acc (SynModuleOrNamespaceSig(longIdent, _, _, decls, _, _, _, moduleRange)) ->
                        if rangeContainsPos moduleRange pos then
                            walkModuleOrNamespaceSig (longIdent::acc) (decls, moduleRange) @ acc
                        else acc) []
        idents
        |> List.rev
        |> Seq.concat
        |> Seq.map (fun ident -> ident.idText)
        |> String.concat "."