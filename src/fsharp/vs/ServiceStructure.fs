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

module internal Structure =
/// Set of visitor utilities, designed for the express purpose of fetching ranges
/// from an untyped AST for the purposes of block structure.
    [<RequireQualifiedAccess>]
    module private Range =
        /// Create a range starting at the end of r1 and finishing at the end of r2
        let inline endToEnd (r1: range) (r2: range) = mkFileIndexRange r1.FileIndex r1.End   r2.End

        /// Create a range beginning at the start of r1 and finishing at the end of r2
        let inline startToEnd (r1: range) (r2: range) = mkFileIndexRange r1.FileIndex r1.Start r2.End

        /// Create a range beginning at the start of r1 and finishing at the start of r2
        let inline startToStart (r1: range) (r2: range) = mkFileIndexRange r1.FileIndex r1.Start r2.Start

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


    let longIdentRange (longId:LongIdent) =
        Range.startToEnd (List.head longId).idRange (List.last longId).idRange



    /// Scope indicates the way a range/snapshot should be collapsed. |Scope.Scope.Same| is for a scope inside
    /// some kind of scope delimiter, e.g. `[| ... |]`, `[ ... ]`, `{ ... }`, etc.  |Scope.Below| is for expressions
    /// following a binding or the right hand side of a pattern, e.g. `let x = ...`
    type Collapse =
        | Below
        | Same

    type Scope =
        | Open
        | Namespace
        | Module
        | Type
        | Member
        | LetOrUse
        | CompExpr
        | IfThenElse
        | ThenInIfThenElse
        | ElseInIfThenElse
        | TryWith
        | TryInTryWith
        | WithInTryWith
        | TryFinally
        | TryInTryFinally
        | FinallyInTryFinally
        | ArrayOrList
        | ObjExpr
        | For
        | While
        | Match
        | MatchLambda
        | MatchClause
        | Lambda
        | CompExprInternal
        | Quote
        | Record
        | SpecialFunc
        | Do
        | New
        | Attribute
        | Interface
        | HashDirective
        | LetOrUseBang
        | TypeExtension
        | YieldOrReturn
        | YieldOrReturnBang
        | Tuple
        | UnionCase
        | EnumCase
        | RecordField
        | SimpleType
        | RecordDefn
        | UnionDefn
        | Comment
        | XmlDocComment
        override self.ToString() = self |> function
            | Open                  -> "Open"
            | Namespace             -> "Namespace"
            | Module                -> "Module"
            | Type                  -> "Type"
            | Member                -> "Member"
            | LetOrUse              -> "LetOrUse"
            | CompExpr              -> "CompExpr"
            | IfThenElse            -> "IfThenElse"
            | ThenInIfThenElse      -> "ThenInIfThenElse"
            | ElseInIfThenElse      -> "ElseInIfThenElse"
            | TryWith               -> "TryWith"
            | TryInTryWith          -> "TryInTryWith"
            | WithInTryWith         -> "WithInTryWith"
            | TryFinally            -> "TryFinally"
            | TryInTryFinally       -> "TryInTryFinally"
            | FinallyInTryFinally   -> "FinallyInTryFinally"
            | ArrayOrList           -> "ArrayOrList"
            | ObjExpr               -> "ObjExpr"
            | For                   -> "For"
            | While                 -> "While"
            | Match                 -> "Match"
            | MatchLambda           -> "MatchLambda"
            | MatchClause           -> "MatchClause"
            | Lambda                -> "Lambda"
            | CompExprInternal      -> "CompExprInternal"
            | Quote                 -> "Quote"
            | Record                -> "Record"
            | SpecialFunc           -> "SpecialFunc"
            | Do                    -> "Do"
            | New                   -> "New"
            | Attribute             -> "Attribute"
            | Interface             -> "Interface"
            | HashDirective         -> "HashDirective"
            | LetOrUseBang          -> "LetOrUseBang"
            | TypeExtension         -> "TypeExtension"
            | YieldOrReturn         -> "YieldOrReturn"
            | YieldOrReturnBang     -> "YieldOrReturnBang"
            | Tuple                 -> "Tuple"
            | UnionCase             -> "UnionCase"
            | EnumCase              -> "EnumCase"
            | RecordField           -> "RecordField"
            | SimpleType            -> "SimpleType"
            | RecordDefn            -> "RecordDefn"
            | UnionDefn             -> "UnionDefn"
            | Comment               -> "Comment"
            | XmlDocComment         -> "XmlDocComment"



    [<NoComparison>]
    type internal ScopeRange = {
        Scope: Scope
        Collapse: Collapse
        /// HintSpan in BlockSpan
        Range: range
        /// TextSpan in BlockSpan
        CollapseRange:range
    }

    // Only yield a range that spans 2 or more lines
    let inline private rcheck scope collapse (fullRange:range) (collapseRange:range)  = seq {
        if fullRange.StartLine <> fullRange.EndLine then yield {
            Scope = scope
            Collapse = collapse
            Range = fullRange
            CollapseRange = collapseRange
        }
    }

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
            | SynExpr.Typed (e,_,_)
            | SynExpr.DotIndexedGet (e,_,_,_)
            | SynExpr.DotIndexedSet (e,_,_,_,_,_) -> yield! parseExpr e
            | SynExpr.New (_,_,e,r) ->
                yield! rcheck Scope.New Collapse.Below r r
                yield! parseExpr e
            | SynExpr.YieldOrReturn (_,e,r) ->
                yield! rcheck Scope.YieldOrReturn Collapse.Below r r
                yield! parseExpr e
            | SynExpr.YieldOrReturnFrom (_,e,r) ->
                yield! rcheck Scope.YieldOrReturnBang Collapse.Below r r
                yield! parseExpr e
            | SynExpr.DoBang (e,r) ->
                yield! rcheck Scope.Do Collapse.Below r <| Range.modStart r 3
                yield! parseExpr e
            | SynExpr.LetOrUseBang (_,_,_,pat,e1,e2,_) ->
                // for `let!` or `use!` the pattern begins at the end of the keyword so that
                // this scope can be used without adjustment if there is no `=` on the same line
                // if there is an `=` the range will be adjusted during the tooltip creation
                let r = Range.endToEnd pat.Range e1.Range
                yield! rcheck Scope.LetOrUseBang Collapse.Below r r
                yield! parseExpr e1
                yield! parseExpr e2
            | SynExpr.For (_,_,_,_,_,e,r)
            | SynExpr.ForEach (_,_,_,_,_,e,r) ->
                yield! rcheck Scope.For Collapse.Below r r
                yield! parseExpr e
            | SynExpr.LetOrUse (_,_,bindings, body,r) ->
                yield! parseBindings r bindings
                yield! parseExpr body
            | SynExpr.Match (seqPointAtBinding,_,clauses,_,r) ->
                match seqPointAtBinding with
                | SequencePointAtBinding sr ->
                    yield! rcheck Scope.Match Collapse.Same sr r
                | _ -> ()
                yield! parseMatchClauses clauses
            | SynExpr.MatchLambda (_,caseRange,clauses,matchSeqPoint,r) ->
                let caseRange =
                    match matchSeqPoint with
                    | SequencePointAtBinding r -> r
                    | _ -> caseRange
                let collapse = Range.endToEnd caseRange r
                yield! rcheck Scope.MatchLambda Collapse.Same r collapse
                yield! parseMatchClauses clauses
            | SynExpr.App (atomicFlag,isInfix,funcExpr,argExpr,r) ->
                // seq exprs, custom operators, etc
                if ExprAtomicFlag.NonAtomic=atomicFlag && (not isInfix)
                   && (function | SynExpr.Ident _ -> true | _ -> false) funcExpr
                   // if the argExrp is a computation expression another match will handle the outlining
                   // these cases must be removed to prevent creating unnecessary tags for the same scope
                   && (function | SynExpr.CompExpr _ -> false | _ -> true) argExpr then
                        let range = Range.endToEnd funcExpr.Range r
                        yield! rcheck Scope.SpecialFunc Collapse.Below range range
                yield! parseExpr argExpr
                yield! parseExpr funcExpr
            | SynExpr.Sequential (_,_,e1,e2,_) ->
                yield! parseExpr e1
                yield! parseExpr e2
            | SynExpr.ArrayOrListOfSeqExpr (isArray,e,r) ->
                yield! rcheck  Scope.ArrayOrList Collapse.Same r <| Range.modBoth r (if isArray then 2 else 1) (if isArray then 2 else 1)
                yield! parseExpr e
            | SynExpr.CompExpr (arrayOrList,_,e,r) as c ->
                if arrayOrList then
                    yield! parseExpr e
                else  // exclude the opening { and closing } on the cexpr from collapsing
                    //yield! rcheck Scope.CompExpr Collapse.Same r <| Range.modBoth r 1 1
                    yield! rcheck Scope.CompExpr Collapse.Same c.Range <| Range.modBoth r 1 1
                yield! parseExpr e
            | SynExpr.ObjExpr (_,_,bindings,_,newRange,wholeRange) ->
                let r = mkFileIndexRange newRange.FileIndex newRange.End (Range.mkPos wholeRange.EndLine (wholeRange.EndColumn - 1))
                yield! rcheck Scope.ObjExpr Collapse.Below r r
                yield! parseBindings wholeRange bindings
            | SynExpr.TryWith (e,tryRange,matchClauses,withRange,_wholeRange,tryPoint,withPoint) ->
                match tryPoint with
                | SequencePointAtTry r ->
                    let range = Range.endToEnd r tryRange
                    yield! rcheck Scope.TryWith Collapse.Below range range
                | _ -> ()
                match withPoint with
                | SequencePointAtWith r ->
                    let range = Range.endToEnd r withRange
                    yield! rcheck Scope.WithInTryWith Collapse.Below range range
                | _ -> ()
                yield! parseExpr e
                yield! parseMatchClauses matchClauses
            | SynExpr.TryFinally (tryExpr,finallyExpr,r,tryPoint,finallyPoint) ->
                match tryPoint with
                | SequencePointAtTry tryRange ->
                    let range = Range.endToEnd r tryRange
                    yield! rcheck Scope.TryFinally Collapse.Below range range
                | _ -> ()
                match finallyPoint with
                | SequencePointAtFinally finallyRange ->
                    let range = Range.endToEnd finallyRange r
                    yield! rcheck  Scope.FinallyInTryFinally Collapse.Below range range
                | _ -> ()
                yield! parseExpr tryExpr
                yield! parseExpr finallyExpr
            | SynExpr.IfThenElse (e1,e2,e3,seqPointInfo,_,_,r) ->
                // Outline the entire IfThenElse
                yield! rcheck Scope.IfThenElse Collapse.Below r r
                // Outline the `then` scope
                match seqPointInfo with
                | SequencePointInfoForBinding.SequencePointAtBinding rt ->
                    let range =  Range.endToEnd rt e2.Range
                    yield! rcheck  Scope.ThenInIfThenElse Collapse.Below range range
                | _ -> ()
                yield! parseExpr e1
                yield! parseExpr e2
                match e3 with
                | Some e ->
                    match e with // prevent double collapsing on elifs
                    | SynExpr.IfThenElse (_,_,_,_,_,_,_) ->
                        yield! parseExpr e
                    | _ ->
                        yield! rcheck Scope.ElseInIfThenElse Collapse.Same e.Range e.Range
                        yield! parseExpr e
                | None -> ()
            | SynExpr.While (_,_,e,r) ->
                yield! rcheck Scope.While Collapse.Below r r
                yield! parseExpr e
            | SynExpr.Lambda (_,_,pats,e,r) ->
                match pats with
                | SynSimplePats.SimplePats (_,pr)
                | SynSimplePats.Typed (_,_,pr) ->
                    yield! rcheck Scope.Lambda Collapse.Below r <| Range.endToEnd pr r
                yield! parseExpr e
            | SynExpr.Lazy (e,r) ->
                yield! rcheck Scope.SpecialFunc Collapse.Below r r
                yield! parseExpr e
            | SynExpr.Quote (_,isRaw,e,_,r) ->
                // subtract columns so the @@> or @> is not collapsed
                yield! rcheck Scope.Quote Collapse.Same r <| Range.modBoth r (if isRaw then 3 else 2) (if isRaw then 3 else 2)
                yield! parseExpr e
            | SynExpr.Tuple (es,_,r)
            | SynExpr.StructTuple(es,_,r) ->
                yield! rcheck Scope.Tuple Collapse.Same r r
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
                yield! rcheck Scope.Record Collapse.Same r <| Range.modBoth r 1 1
            | _ -> ()
        }

    and private parseMatchClause (SynMatchClause.Clause (synPat,_,e,r,_)) =
        seq {
            let fullrange = Range.startToEnd synPat.Range r
            let collapse = Range.endToEnd synPat.Range e.Range  // Collapse the scope after `->`
            yield! rcheck Scope.MatchClause Collapse.Same fullrange collapse
            yield! parseExpr e
        }

    and private parseMatchClauses = Seq.collect parseMatchClause

    and private parseAttributes (attrs: SynAttributes) =
        seq{
            let attrListRange =
                if List.isEmpty attrs then Seq.empty else
                let range = Range.startToEnd (attrs.[0].Range) (attrs.[attrs.Length-1].ArgExpr.Range)
                rcheck Scope.Attribute Collapse.Same  range range
            match  attrs with
            | [] -> ()
            | [_] -> yield! attrListRange
            | hd::tl ->
                yield! attrListRange
                yield! parseExpr hd.ArgExpr
                // If there are more than 2 attributes only add tags to the 2nd and beyond, to avoid double collapsing on the first attribute
                yield! tl |> Seq.collect (fun attr ->
                    let range = Range.startToEnd attr.Range attr.ArgExpr.Range
                    rcheck Scope.Attribute Collapse.Same range range
                )
                // visit the expressions inside each attribute
                yield! attrs |> Seq.collect (fun attr -> parseExpr attr.ArgExpr)
        }

    and private parseBinding (fullrange:range) (Binding (_,kind,_,_,attrs,_,_,_,_,e,br,_) as b) =
        seq {
            match kind with
            | SynBindingKind.NormalBinding ->
                let r = Range.endToEnd b.RangeOfBindingSansRhs b.RangeOfBindingAndRhs
                yield! rcheck Scope.LetOrUse Collapse.Below fullrange r
            | SynBindingKind.DoBinding ->
                let r = Range.modStart br 2
                yield! rcheck Scope.Do Collapse.Below fullrange r
            | _ -> ()
            yield! parseAttributes attrs
            yield! parseExpr e
        }

    and private parseBindings (fullrange:range) sqs = sqs |> Seq.collect (parseBinding fullrange)

    and private parseExprInterface (InterfaceImpl(_synType,bindings,range)) = seq{
        yield! rcheck Scope.Interface Collapse.Below range range
        yield! parseBindings range bindings
     }

    and private parseExprInterfaces (intfs:#seq<SynInterfaceImpl>) = Seq.collect parseExprInterface intfs

    and private parseSynMemberDefn d =
        seq {
            match d with
            | SynMemberDefn.Member (binding, r) ->
                yield! rcheck Scope.Member Collapse.Below d.Range r
                yield! parseBinding d.Range binding
            | SynMemberDefn.LetBindings (bindings, _, _, _r) ->
                //yield! rcheck Scope.LetOrUse Collapse.Below r
                yield! parseBindings d.Range bindings
            | SynMemberDefn.Interface (tp,iMembers,_) ->
                yield! rcheck Scope.Interface Collapse.Below d.Range <| Range.endToEnd tp.Range d.Range
                match iMembers with
                | Some members -> yield! Seq.collect parseSynMemberDefn members
                | None -> ()
            | SynMemberDefn.NestedType (td, _, _r) ->
                yield! parseTypeDefn td d.Range
            | SynMemberDefn.AbstractSlot (ValSpfn(_, _, _, synt, _, _, _, _, _, _, _), _, r) ->
                yield! rcheck Scope.Member Collapse.Below d.Range <| Range.startToEnd synt.Range r
            | SynMemberDefn.AutoProperty (_, _, _, _, (*memkind*)_, _, _, _, e, _, r) ->
                yield! rcheck Scope.Member Collapse.Below d.Range r
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
            | SynTypeDefnSimpleRepr.Enum (cases,er) ->
                yield! rcheck Scope.SimpleType Collapse.Below er er
                yield!
                    cases
                    |> Seq.collect (fun (SynEnumCase.EnumCase (attrs, _, _, _, cr)) ->
                        seq { yield! rcheck Scope.EnumCase Collapse.Below cr cr
                              yield! parseAttributes attrs })
            | SynTypeDefnSimpleRepr.Record (_opt,fields,rr) ->
                //yield! rcheck Scope.SimpleType Collapse.Same <| Range.modBoth rr (accessRange opt+1) 1
                yield! rcheck Scope.RecordDefn Collapse.Same rr rr //<| Range.modBoth rr 1 1
                yield! fields
                    |> Seq.collect (fun (SynField.Field (attrs,_,_,_,_,_,_,fr)) ->
                    seq{yield! rcheck Scope.RecordField Collapse.Below fr fr
                        yield! parseAttributes attrs
                    })
            | SynTypeDefnSimpleRepr.Union (_opt,cases,ur) ->
//                yield! rcheck Scope.SimpleType Collapse.Same <| Range.modStart ur (accessRange opt)
                yield! rcheck Scope.UnionDefn Collapse.Same ur ur
                yield! cases
                    |> Seq.collect (fun (SynUnionCase.UnionCase (attrs,_,_,_,_,cr)) ->
                    seq{yield! rcheck Scope.UnionCase Collapse.Below cr cr
                        yield! parseAttributes attrs
                    })
            | _ -> ()
        }

    and private parseTypeDefn (TypeDefn (componentInfo, objectModel, members, range)) (fullrange:range) =
        seq {
            match objectModel with
            | SynTypeDefnRepr.ObjectModel (defnKind, objMembers, _) ->
                let range = Range.endToEnd componentInfo.Range range
                match defnKind with
                | SynTypeDefnKind.TyconAugmentation ->
                    yield! rcheck Scope.TypeExtension Collapse.Below fullrange range
                | _ ->
                    yield! rcheck Scope.Type Collapse.Below fullrange range
                yield! Seq.collect parseSynMemberDefn objMembers
                // visit the members of a type extension
                yield! Seq.collect parseSynMemberDefn members
            | SynTypeDefnRepr.Simple (simpleRepr,_r) ->
                yield! rcheck Scope.Type Collapse.Below fullrange <| Range.endToEnd componentInfo.Range range
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
            | [r] ->
                let range = Range.mkRange "" r.Start r.End
                Some { Scope = scope; Collapse = Collapse.Same; Range = range ; CollapseRange = range }
            | lastRange :: rest ->
                let firstRange = Seq.last rest
                let range = Range.mkRange "" firstRange.Start lastRange.End
                Some { Scope = scope; Collapse = Collapse.Same; Range = range; CollapseRange = range }

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
                yield! parseBindings decl.Range bindings
            | SynModuleDecl.Types (types,_r) ->
                yield! Seq.collect (fun t -> parseTypeDefn t decl.Range) types
            // Fold the attributes above a module
            | SynModuleDecl.NestedModule (SynComponentInfo.ComponentInfo (attrs,_,_,_,_,_,_,cmpRange) as _cmpInfo,_, decls,_,_) ->
//                cmpInfo.
                // Outline the full scope of the module
                let r = Range.endToEnd cmpRange decl.Range
                yield! rcheck Scope.Module Collapse.Below decl.Range r
                // A module's component info stores the ranges of its attributes
                yield! parseAttributes attrs
                yield! collectOpens decls
                yield! Seq.collect parseDeclaration decls
            | SynModuleDecl.DoExpr (_,e,_) ->
                yield! parseExpr e
            | SynModuleDecl.Attributes (attrs,_) ->
                yield! parseAttributes attrs
            | _ -> ()
        }

    let private parseModuleOrNamespace moduleOrNs =
        seq {
            let (SynModuleOrNamespace.SynModuleOrNamespace (longId,_,isModule,decls,_,_,_,r)) = moduleOrNs
            if isModule then
                yield! rcheck Scope.Namespace Collapse.Below moduleOrNs.Range r
            else
                let fullrange = Range.startToEnd (longIdentRange longId) r
                let collapse = Range.endToEnd (longIdentRange longId) r
                yield! rcheck Scope.Namespace Collapse.Below fullrange collapse

            yield! collectHashDirectives decls
            yield! collectOpens decls
            yield! Seq.collect parseDeclaration decls
        }

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

            let range = Range.mkRange "" (Range.mkPos (startLine + 1) startCol) (Range.mkPos (endLine + 1) endCol)

            { Scope = scopeType
              Collapse = Collapse.Same
              Range = range
              CollapseRange = range })

    let getOutliningRanges (sourceLines: string []) (parsedInput: ParsedInput) =
        match parsedInput with
        | ParsedInput.ImplFile implFile ->
            let (ParsedImplFileInput (_, _, _, _, _, modules, _)) = implFile
            let astBasedRanges = Seq.collect parseModuleOrNamespace modules
            let commentRanges = getCommentRanges sourceLines
            Seq.append astBasedRanges commentRanges
        | _ -> Seq.empty
