[<AutoOpen>]
module internal FSharpParseFileResultsExtensions

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

type UnusedBinding =
    | Expression of range
    | SelfId of range
    | Member

type FSharpParseFileResults with

    member this.TryRangeOfBindingWithHeadPatternWithPos pos =
        let input = this.ParseTree

        SyntaxTraversal.Traverse(
            pos,
            input,
            { new SyntaxVisitorBase<_>() with
                member _.VisitExpr(_, _, defaultTraverse, expr) = defaultTraverse expr

                override _.VisitBinding(_path, defaultTraverse, binding) =
                    match binding with
                    | SynBinding (kind = SynBindingKind.Normal; headPat = pat) as binding ->
                        if Position.posEq binding.RangeOfHeadPattern.Start pos then
                            Some(Expression binding.RangeOfBindingWithRhs)
                        else
                            // Check if it's an operator
                            match pat with
                            | SynPat.LongIdent(longDotId = LongIdentWithDots ([ id ], _)) when id.idText.StartsWith("op_") ->
                                if Position.posEq id.idRange.Start pos then
                                    Some(Expression binding.RangeOfBindingWithRhs)
                                else
                                    defaultTraverse binding
                            | _ -> defaultTraverse binding

                    | _ -> defaultTraverse binding

                override _.VisitComponentInfo(path, _) =
                    path
                    |> List.collect (fun node ->
                        match node with
                        | SyntaxNode.SynModule (SynModuleDecl.Types (types, _)) -> types
                        | _ -> [])
                    |> Seq.choose (fun node ->
                        match node with
                        | SynTypeDefn(implicitConstructor = Some (SynMemberDefn.ImplicitCtor(selfIdentifier = Some ident))) when
                            ident.idRange.Start = pos
                            ->
                            Some(SelfId ident.idRange)
                        | SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel (members = defs)) ->
                            defs
                            |> List.choose (fun node ->
                                match node with
                                | SynMemberDefn.Member (SynBinding(headPat = SynPat.LongIdent (longDotId = id)), _) when
                                    id.Range.Start = pos
                                    ->
                                    Some Member
                                | _ -> None)
                            |> List.tryExactlyOne
                        | _ -> None)
                    |> Seq.tryExactlyOne
            }
        )

    member this.TryRangeOfTypeofWithNameAndTypeExpr pos =
        SyntaxTraversal.Traverse(
            pos,
            this.ParseTree,
            { new SyntaxVisitorBase<_>() with
                member _.VisitExpr(_path, _, defaultTraverse, expr) =
                    match expr with
                    | SynExpr.DotGet (expr, _, _, range) ->
                        match expr with
                        | SynExpr.TypeApp (SynExpr.Ident ident, _, typeArgs, _, _, _, _) ->
                            let onlyOneTypeArg =
                                match typeArgs with
                                | [] -> false
                                | [ _ ] -> true
                                | _ -> false

                            if ident.idText = "typeof" && onlyOneTypeArg then
                                Some
                                    {|
                                        NamedIdentRange = typeArgs.Head.Range
                                        FullExpressionRange = range
                                    |}
                            else
                                defaultTraverse expr
                        | _ -> defaultTraverse expr
                    | _ -> defaultTraverse expr
            }
        )
