// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Open up the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.
//--------------------------------------------------------------------------

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.FSharp.Compiler.Ast
open System.Collections.Generic
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Range

/// Set of visitor utilities, designed for the express purpose of fetching ranges
/// from an untyped AST for the purposes of block structure.
module internal Structure =
    [<RequireQualifiedAccess>]
    module private Range =
        /// Create a range starting at the end of r1 and finishing at the end of r2
        let inline endToEnd (r1: range) (r2: range) = mkFileIndexRange r1.FileIndex r1.End   r2.End

        /// Create a range beginning at the start of r1 and finishing at the end of r2
        let inline startToEnd (r1: range) (r2: range) = mkFileIndexRange r1.FileIndex r1.Start r2.End

        /// Create a new range from r by shifting the starting column by m
        let inline modStart (r: range) (m:int) =
            let modstart = mkPos r.StartLine (r.StartColumn+m)
            mkFileIndexRange r.FileIndex modstart r.End

        /// Produce a new range by adding modStart to the StartColumn of `r`
        /// and subtracting modEnd from the EndColumn of `r`
        let inline modBoth (r:range) modStart modEnd =
            let rStart = Range.mkPos r.StartLine (r.StartColumn+modStart)
            let rEnd   = Range.mkPos r.EndLine   (r.EndColumn - modEnd)
            mkFileIndexRange r.FileIndex rStart rEnd

    /// Scope indicates the way a range/snapshot should be collapsed. |Scope.Scope.Same| is for a scope inside
    /// some kind of scope delimiter, e.g. `[| ... |]`, `[ ... ]`, `{ ... }`, etc.  |Scope.Below| is for expressions
    /// following a binding or the right hand side of a pattern, e.g. `let x = ...`
    type Collapse =
        | Below = 0
        | Same = 1

    type Scope =
        | Open
        | Namespace
        | Module
        | Type
        | Member
        | LetOrUse
        | CompExpr
        | IfThenElse
        | TryWith
        | TryFinally
        | ArrayOrList
        | ObjExpr
        | For
        | While
        | Match
        | MatchLambda
        | Lambda
        | CompExprInternal
        | Quote
        | Record
        | SpecialFunc
        | Do
        | Interface
        | HashDirective
        | LetOrUseBang
        | TypeExtension
        | YieldOrReturn
        | YieldOrReturnBang
        | SimpleType
        | RecordDefn
        | UnionDefn
        | Comment
        | XmlDocComment

    [<NoComparison; Struct>]
    type internal ScopeRange = 
        { Scope: Scope
          Collapse: Collapse
          Range: range }

    // Only yield a range that spans 2 or more lines
    let inline private rcheck scope collapse (r: range) =
        seq { if r.StartLine <> r.EndLine then 
                yield { Scope = scope; Collapse = collapse; Range = r }}

    let rec private parseExpr expression =
        seq {
            match expression with
            | SynExpr.Upcast (e,_,_)
            | SynExpr.Downcast (e,_,_)
            | SynExpr.AddressOf(_,e,_,_)
            | SynExpr.InferredDowncast (e,_)
            | SynExpr.InferredUpcast (e,_)
            | SynExpr.DotGet (e,_,_,_)
            | SynExpr.Do (e,_)
            | SynExpr.DotSet (e,_,_,_)
            | SynExpr.New (_,_,e,_)
            | SynExpr.Typed (e,_,_)
            | SynExpr.DotIndexedGet (e,_,_,_)
            | SynExpr.DotIndexedSet (e,_,_,_,_,_) -> yield! parseExpr e
            | SynExpr.YieldOrReturn (_,e,r) ->
                yield! rcheck Scope.YieldOrReturn Collapse.Below r
                yield! parseExpr e
            | SynExpr.YieldOrReturnFrom (_,e,r) ->
                yield! rcheck Scope.YieldOrReturnBang Collapse.Below r
                yield! parseExpr e
            | SynExpr.DoBang (e,r) ->
                yield! rcheck Scope.Do Collapse.Below <| Range.modStart r 3
                yield! parseExpr e
            | SynExpr.LetOrUseBang (_,_,_,pat,e1,e2,_) ->
                // for `let!` or `use!` the pattern begins at the end of the keyword so that
                // this scope can be used without adjustment if there is no `=` on the same line
                // if there is an `=` the range will be adjusted during the tooltip creation
                yield! rcheck Scope.LetOrUseBang Collapse.Below <| Range.endToEnd pat.Range e1.Range
                yield! parseExpr e1
                yield! parseExpr e2
            | SynExpr.For (_,_,_,_,_,e,r)
            | SynExpr.ForEach (_,_,_,_,_,e,r) ->
                yield! rcheck Scope.For Collapse.Below r
                yield! parseExpr e
            | SynExpr.LetOrUse (_,_,bindings, body,_) ->
                yield! parseBindings bindings
                yield! parseExpr body
            | SynExpr.Match (seqPointAtBinding,_,clauses,_,r) ->
                match seqPointAtBinding with
                | SequencePointAtBinding _ ->
                    yield! rcheck Scope.Match Collapse.Same r
                | _ -> ()
                yield! parseMatchClauses clauses
            | SynExpr.MatchLambda (_,_,clauses,_,r) ->
                yield! rcheck Scope.MatchLambda Collapse.Same r
                yield! parseMatchClauses clauses
            | SynExpr.App (atomicFlag,isInfix,funcExpr,argExpr,r) ->
                // seq exprs, custom operators, etc
                if ExprAtomicFlag.NonAtomic=atomicFlag && (not isInfix)
                   && (function | SynExpr.Ident _ -> true | _ -> false) funcExpr
                   // if the argExrp is a computation expression another match will handle the outlining
                   // these cases must be removed to prevent creating unnecessary tags for the same scope
                   && (function | SynExpr.CompExpr _ -> false | _ -> true) argExpr then
                        yield! rcheck Scope.SpecialFunc Collapse.Below <| Range.endToEnd funcExpr.Range r
                yield! parseExpr argExpr
                yield! parseExpr funcExpr
            | SynExpr.Sequential (_,_,e1,e2,_) ->
                yield! parseExpr e1
                yield! parseExpr e2
            | SynExpr.ArrayOrListOfSeqExpr (isArray,e,r) ->
                yield! rcheck  Scope.ArrayOrList Collapse.Same <| Range.modBoth r (if isArray then 2 else 1) (if isArray then 2 else 1)
                yield! parseExpr e
            | SynExpr.CompExpr (arrayOrList,_,e,r) ->
                if arrayOrList then
                    yield! parseExpr e
                else  // exclude the opening { and closing } on the cexpr from collapsing
                    yield! rcheck Scope.CompExpr Collapse.Same <| Range.modBoth r 1 1
                yield! parseExpr e
            | SynExpr.ObjExpr (_,_,bindings,_,newRange,wholeRange) ->
                let r = mkFileIndexRange newRange.FileIndex newRange.End (Range.mkPos wholeRange.EndLine (wholeRange.EndColumn - 1))
                yield! rcheck Scope.ObjExpr Collapse.Below r
                yield! parseBindings bindings
            | SynExpr.TryWith (e,_,matchClauses,_,range,_,_) ->
                yield! rcheck Scope.TryWith Collapse.Same range
                yield! parseExpr e
                yield! parseMatchClauses matchClauses
            | SynExpr.TryFinally (tryExpr,finallyExpr,r,tryPoint,_) ->
                match tryPoint with
                | SequencePointAtTry tryRange ->
                    yield! rcheck Scope.TryFinally Collapse.Below <| Range.endToEnd tryRange r
                | _ -> ()
                yield! parseExpr tryExpr
                yield! parseExpr finallyExpr
            | SynExpr.IfThenElse (e1,e2,e3,_,_,_,r) ->
                // Outline the entire IfThenElse
                yield! rcheck Scope.IfThenElse Collapse.Below r
                yield! parseExpr e1
                yield! parseExpr e2
                match e3 with
                | Some e -> yield! parseExpr e
                | None -> ()
            | SynExpr.While (_,_,e,r) ->
                yield! rcheck Scope.While Collapse.Below  r
                yield! parseExpr e
            | SynExpr.Lambda (_,_,pats,e,r) ->
                match pats with
                | SynSimplePats.SimplePats (_,pr)
                | SynSimplePats.Typed (_,_,pr) ->
                    yield! rcheck Scope.Lambda Collapse.Below <| Range.endToEnd pr r
                yield! parseExpr e
            | SynExpr.Lazy (e,r) ->
                yield! rcheck Scope.SpecialFunc Collapse.Below r
                yield! parseExpr e
            | SynExpr.Quote (_,isRaw,e,_,r) ->
                // subtract columns so the @@> or @> is not collapsed
                yield! rcheck Scope.Quote Collapse.Same <| Range.modBoth r (if isRaw then 3 else 2) (if isRaw then 3 else 2)
                yield! parseExpr e
            | SynExpr.Tuple (es,_,_) ->
                yield! Seq.collect parseExpr es
            | SynExpr.Paren (e,_,_,_) ->
                yield! parseExpr e
            | SynExpr.Record (recCtor,recCopy,recordFields,r) ->
                if recCtor.IsSome then
                    let (_,ctorArgs,_,_,_) = recCtor.Value
                    yield! parseExpr ctorArgs
                if recCopy.IsSome then
                    let (e,_) = recCopy.Value
                    yield! parseExpr e
                yield! recordFields |> (Seq.choose (fun (_,e,_) -> e) >> Seq.collect parseExpr)
                // exclude the opening `{` and closing `}` of the record from collapsing
                yield! rcheck Scope.Record Collapse.Same <| Range.modBoth r 1 1
            | _ -> ()
        }

    and private parseMatchClause (SynMatchClause.Clause (_,_,e,_,_)) = parseExpr e
    and private parseMatchClauses = Seq.collect parseMatchClause

    and private parseBinding (Binding (_,kind,_,_,_,_,_,_,_,e,br,_) as b) =
        seq {
//            let r = Range.endToEnd b.RangeOfBindingSansRhs b.RangeOfBindingAndRhs
            match kind with
            | SynBindingKind.NormalBinding ->
                yield! rcheck Scope.LetOrUse Collapse.Below <| Range.endToEnd b.RangeOfBindingSansRhs b.RangeOfBindingAndRhs
            | SynBindingKind.DoBinding ->
                yield! rcheck Scope.Do Collapse.Below <| Range.modStart br 2
            | _ -> ()
            yield! parseExpr e
        }

    and private parseBindings = Seq.collect parseBinding

    and private parseSynMemberDefn d =
        seq {
            match d with
            | SynMemberDefn.Member (binding, r) ->
                yield! rcheck Scope.Member Collapse.Below r
                yield! parseBinding binding
            | SynMemberDefn.LetBindings (bindings, _, _, _r) ->
                //yield! rcheck Scope.LetOrUse Collapse.Below r
                yield! parseBindings bindings
            | SynMemberDefn.Interface (tp,iMembers,_) ->
                yield! rcheck Scope.Interface Collapse.Below <| Range.endToEnd tp.Range d.Range
                match iMembers with
                | Some members -> yield! Seq.collect parseSynMemberDefn members
                | None -> ()
            | SynMemberDefn.NestedType (td, _, _) ->
                yield! parseTypeDefn td
            | SynMemberDefn.AbstractSlot (ValSpfn(_, _, _, synt, _, _, _, _, _, _, _), _, r) ->
                yield! rcheck Scope.Member Collapse.Below <| Range.startToEnd synt.Range r
            | SynMemberDefn.AutoProperty (_, _, _, _, (*memkind*)_, _, _, _, e, _, r) ->
                yield! rcheck Scope.Member Collapse.Below r
                yield! parseExpr e
            | _ -> ()
        }

    (*  For Cases like
        --------------
            type JsonDocument =
                private {   Json : string
                            Path : string   }
        Or
             type JsonDocument =
                internal |  Json of string
                         |  Path of string
    *)
    and private parseSimpleRepr simple =
        let _accessRange (opt:SynAccess option) =
            match opt with
            | None -> 0
            | Some synacc ->
                match synacc with
                | SynAccess.Public -> 6
                | SynAccess.Private -> 7
                | SynAccess.Internal -> 8
        seq {
            match simple with
            | SynTypeDefnSimpleRepr.Enum (_,er) ->
                yield! rcheck Scope.SimpleType Collapse.Below er
            | SynTypeDefnSimpleRepr.Record (_opt,_,rr) ->
                yield! rcheck Scope.RecordDefn Collapse.Same rr
            | SynTypeDefnSimpleRepr.Union (_opt,_,ur) ->
                yield! rcheck Scope.UnionDefn Collapse.Same ur
            | _ -> ()
        }

    and private parseTypeDefn (TypeDefn (componentInfo, objectModel, members, range)) =
        seq {
            match objectModel with
            | SynTypeDefnRepr.ObjectModel (defnKind, objMembers, _) ->
                match defnKind with
                | SynTypeDefnKind.TyconAugmentation ->
                    yield! rcheck Scope.TypeExtension Collapse.Below <| Range.endToEnd componentInfo.Range range
                | _ ->
                    yield! rcheck Scope.Type Collapse.Below <| Range.endToEnd componentInfo.Range range
                yield! Seq.collect parseSynMemberDefn objMembers
                // visit the members of a type extension
                yield! Seq.collect parseSynMemberDefn members
            | SynTypeDefnRepr.Simple (simpleRepr,_r) ->
                yield! rcheck Scope.Type Collapse.Below <| Range.endToEnd componentInfo.Range range
                yield! parseSimpleRepr simpleRepr
                yield! Seq.collect parseSynMemberDefn members
            | SynTypeDefnRepr.Exception _ -> ()
        }

    let private getConsecutiveModuleDecls (predicate: SynModuleDecl -> range option) (scope:Scope) (decls: SynModuleDecls) =
        let groupConsecutiveDecls input =
            let rec loop (input: range list) (res: range list list) currentBulk =
                match input, currentBulk with
                | [], [] -> List.rev res
                | [], _ -> List.rev (currentBulk::res)
                | r :: rest, [] -> loop rest res [r]
                | r :: rest, last :: _ when r.StartLine = last.EndLine + 1 ->
                    loop rest res (r::currentBulk)
                | r :: rest, _ -> loop rest (currentBulk::res) [r]
            loop input [] []

        let selectRanges (ranges: range list) =
            match ranges with
            | [] -> None
            | [r] when r.StartLine = r.EndLine -> None
            | [r] -> Some { Scope = scope; Collapse = Collapse.Same; Range = Range.mkRange "" r.Start r.End }
            | lastRange :: rest ->
                let firstRange = Seq.last rest
                Some { Scope = scope; Collapse = Collapse.Same; Range = Range.mkRange "" firstRange.Start lastRange.End }

        decls |> (List.choose predicate >> groupConsecutiveDecls >> List.choose selectRanges)


    let collectOpens = getConsecutiveModuleDecls (function SynModuleDecl.Open (_, r) -> Some r | _ -> None) Scope.Open

    let collectHashDirectives =
         getConsecutiveModuleDecls(
            function
            | SynModuleDecl.HashDirective (ParsedHashDirective (directive, _, _),r) ->
                let prefixLength = "#".Length + directive.Length + " ".Length
                Some (Range.mkRange "" (Range.mkPos r.StartLine prefixLength) r.End)
            | _ -> None) Scope.HashDirective


    let rec private parseDeclaration (decl: SynModuleDecl) =
        seq {
            match decl with
            | SynModuleDecl.Let (_,bindings,_) ->
                yield! parseBindings bindings
            | SynModuleDecl.Types (types,_) ->
                yield! Seq.collect parseTypeDefn types
            // Fold the attributes above a module
            | SynModuleDecl.NestedModule (SynComponentInfo.ComponentInfo (_,_,_,_,_,_,_,cmpRange),_, decls,_,_) ->
                // Outline the full scope of the module
                yield! rcheck Scope.Module Collapse.Below <| Range.endToEnd cmpRange decl.Range
                // A module's component info stores the ranges of its attributes
                yield! collectOpens decls
                yield! Seq.collect parseDeclaration decls
            | SynModuleDecl.DoExpr (_,e,_) ->
                yield! parseExpr e
            | _ -> ()
        }

    let private parseModuleOrNamespace moduleOrNs =
        seq { let (SynModuleOrNamespace.SynModuleOrNamespace (_,_,_,decls,_,_,_,_)) = moduleOrNs
              yield! collectHashDirectives decls
              yield! collectOpens decls
              yield! Seq.collect parseDeclaration decls }

    type private LineNum = int
    type private LineStr = string
    type private CommentType = Regular | XmlDoc
    
    [<NoComparison>]
    type private CommentList = 
        { Lines: ResizeArray<LineNum * LineStr>
          Type: CommentType }
        static member New ty lineStr = 
            { Type = ty; Lines = ResizeArray [| lineStr |] }

    let private (|Comment|_|) (line: string) =
        if line.StartsWith "///" then Some XmlDoc
        elif line.StartsWith "//" then Some Regular
        else None

    let getCommentRanges (lines: string[]) =
        let rec loop ((lastLineNum, currentComment: CommentList option, result) as state) (lines: string list) lineNum =
            match lines with
            | [] -> state
            | lineStr :: rest -> 
                match lineStr.TrimStart(), currentComment with
                | Comment commentType, Some comment ->
                    loop(
                        if comment.Type = commentType && lineNum = lastLineNum + 1 then
                            comment.Lines.Add (lineNum, lineStr)
                            lineNum, currentComment, result
                        else lineNum, Some (CommentList.New commentType (lineNum, lineStr)), comment :: result) rest (lineNum + 1)
                | Comment commentType, None -> 
                    loop(lineNum, Some (CommentList.New commentType (lineNum, lineStr)), result) rest (lineNum + 1)
                | _, Some comment -> 
                    loop(lineNum, None, comment :: result) rest (lineNum + 1)
                | _ -> loop(lineNum, None, result) rest (lineNum + 1)
        
        let comments: CommentList list = 
            loop (-1, None, []) (List.ofArray lines) 0 
            |> fun (_, lastComment, comments) -> 
                match lastComment with 
                | Some comment -> 
                    comment :: comments 
                | _ -> comments
                |> List.rev

        comments
        |> List.filter (fun comment -> comment.Lines.Count > 1)
        |> List.map (fun comment ->
            let lines = comment.Lines
            let startLine, startStr = lines.[0]
            let endLine, endStr = lines.[lines.Count - 1]
            let startCol = startStr.IndexOf '/'
            let endCol = endStr.TrimEnd().Length

            let scopeType =
                match comment.Type with 
                | Regular -> Scope.Comment
                | XmlDoc -> Scope.XmlDocComment
            
            { Scope = scopeType
              Collapse = Collapse.Same
              Range = Range.mkRange 
                          "" 
                          (Range.mkPos (startLine + 1) startCol)
                          (Range.mkPos (endLine + 1) endCol) })

    let getOutliningRanges (sourceLines: string []) (parsedInput: ParsedInput) =
        match parsedInput with
        | ParsedInput.ImplFile implFile ->
            let (ParsedImplFileInput (_, _, _, _, _, modules, _)) = implFile
            let astBasedRanges = Seq.collect parseModuleOrNamespace modules
            let commentRanges = getCommentRanges sourceLines
            Seq.append astBasedRanges commentRanges
        | _ -> Seq.empty
