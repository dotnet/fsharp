namespace FSharp.Compiler.Syntax

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module SynPat =
    let (|Last|) = List.last

    /// Matches if the two values refer to the same object.
    [<return: Struct>]
    let inline (|Is|_|) (inner1: 'a) (inner2: 'a) =
        if obj.ReferenceEquals(inner1, inner2) then
            ValueSome Is
        else
            ValueNone

    let (|Ident|) (ident: Ident) = ident.idText

    /// Matches if any pattern in the given list is a SynPat.Typed.
    [<return: Struct>]
    let (|AnyTyped|_|) pats =
        if
            pats
            |> List.exists (function
                | SynPat.Typed _ -> true
                | _ -> false)
        then
            ValueSome AnyTyped
        else
            ValueNone

    /// Matches the rightmost potentially dangling nested pattern.
    let rec (|Rightmost|) pat =
        match pat with
        | SynPat.Or(rhsPat = Rightmost pat)
        | SynPat.ListCons(rhsPat = Rightmost pat)
        | SynPat.As(rhsPat = Rightmost pat)
        | SynPat.Ands(pats = Last(Rightmost pat))
        | SynPat.Tuple(isStruct = false; elementPats = Last(Rightmost pat)) -> pat
        | pat -> pat

    /// Matches a nested as pattern.
    [<return: Struct>]
    let rec (|DanglingAs|_|) pat =
        let (|AnyDanglingAs|_|) =
            List.tryPick (function
                | DanglingAs -> Some()
                | _ -> None)

        match pat with
        | SynPat.As _ -> ValueSome()
        | SynPat.Or(lhsPat = DanglingAs)
        | SynPat.Or(rhsPat = DanglingAs)
        | SynPat.ListCons(lhsPat = DanglingAs)
        | SynPat.ListCons(rhsPat = DanglingAs)
        | SynPat.Ands(pats = AnyDanglingAs)
        | SynPat.Tuple(isStruct = false; elementPats = AnyDanglingAs) -> ValueSome()
        | _ -> ValueNone

    /// Matches if the given pattern is atomic.
    [<return: Struct>]
    let (|Atomic|_|) pat =
        match pat with
        | SynPat.Named _
        | SynPat.Wild _
        | SynPat.Paren _
        | SynPat.Tuple(isStruct = true)
        | SynPat.Record _
        | SynPat.ArrayOrList _
        | SynPat.Const _
        | SynPat.LongIdent(argPats = SynArgPats.Pats [])
        | SynPat.Null _
        | SynPat.QuoteExpr _ -> ValueSome Atomic
        | _ -> ValueNone

    let shouldBeParenthesizedInContext path pat : bool =
        match pat, path with
        // Parens are needed in:
        //
        //     let (Pattern …) = …
        //     let (x: …, y…) = …
        //     let (x: …), (y: …) = …
        //     let! (x: …) = …
        //     and! (x: …) = …
        //     use! (x: …) = …
        //     _.member M(x: …) = …
        //     match … with (x: …) -> …
        //     match … with (x, y: …) -> …
        //     function (x: …) -> …
        //     fun (x, y, …) -> …
        //     fun (x: …) -> …
        //     fun (Pattern …) -> …
        //     set (x: …, y: …) = …
        | SynPat.Typed _, SyntaxNode.SynPat(Rightmost(SynPat.Paren(Is pat, _))) :: SyntaxNode.SynMatchClause _ :: _
        | Rightmost(SynPat.Typed _), SyntaxNode.SynMatchClause _ :: _
        | SynPat.Typed _, SyntaxNode.SynExpr(SynExpr.LetOrUseBang _) :: _
        | SynPat.Typed _, SyntaxNode.SynPat(SynPat.Tuple(isStruct = false)) :: SyntaxNode.SynExpr(SynExpr.LetOrUseBang _) :: _
        | SynPat.Tuple(isStruct = false; elementPats = AnyTyped), SyntaxNode.SynExpr(SynExpr.LetOrUseBang _) :: _
        | SynPat.Typed _, SyntaxNode.SynPat(SynPat.Tuple(isStruct = false)) :: SyntaxNode.SynBinding _ :: _
        | SynPat.Tuple(isStruct = false; elementPats = AnyTyped), SyntaxNode.SynBinding _ :: _
        | SynPat.LongIdent(argPats = SynArgPats.Pats(_ :: _)), SyntaxNode.SynBinding _ :: _
        | SynPat.LongIdent(argPats = SynArgPats.Pats(_ :: _)), SyntaxNode.SynExpr(SynExpr.Lambda _) :: _
        | SynPat.Tuple(isStruct = false), SyntaxNode.SynExpr(SynExpr.Lambda(parsedData = Some _)) :: _
        | SynPat.Typed _, SyntaxNode.SynExpr(SynExpr.Lambda(parsedData = Some _)) :: _
        | SynPat.Typed _,
          SyntaxNode.SynPat(SynPat.Tuple(isStruct = false)) :: SyntaxNode.SynPat(SynPat.LongIdent _) :: SyntaxNode.SynBinding _ :: SyntaxNode.SynMemberDefn(SynMemberDefn.GetSetMember _) :: _
        | SynPat.Typed _,
          SyntaxNode.SynPat(SynPat.Tuple(isStruct = false)) :: SyntaxNode.SynPat(SynPat.LongIdent _) :: SyntaxNode.SynBinding(SynBinding(
              valData = SynValData(
                  memberFlags = Some {
                                         MemberKind = SynMemberKind.PropertyGetSet | SynMemberKind.PropertyGet | SynMemberKind.PropertySet
                                     }))) :: _ -> true

        // Parens must be kept when there is a multiline expression
        // to the right whose offsides line would be shifted if the
        // parentheses were removed from a leading pattern on the same line, e.g.,
        //
        //     match maybe with
        //     | Some(x) -> let y = x * 2
        //                  let z = 99
        //                  x + y + z
        //     | None -> 3
        //
        // or
        //
        //     let (x) = printfn "…"
        //               printfn "…"
        | _ when
            // This is arbitrary and will result in some false positives.
            let maxBacktracking = 10

            let rec wouldMoveRhsOffsides n pat path =
                if n = maxBacktracking then
                    true
                else
                    // This does not thoroughly search the trailing
                    // expression — nor does it go up the expression
                    // tree and search farther rightward, or look at record bindings,
                    // etc., etc., etc. — and will result in some false negatives.
                    match path with
                    // Expand the range to that of the outer pattern, since
                    // the parens may extend beyond the inner pat
                    | SyntaxNode.SynPat outer :: path when n = 1 -> wouldMoveRhsOffsides (n + 1) outer path
                    | SyntaxNode.SynPat _ :: path -> wouldMoveRhsOffsides (n + 1) pat path

                    | SyntaxNode.SynExpr(SynExpr.Lambda(body = rhs)) :: _
                    | SyntaxNode.SynExpr(SynExpr.LetOrUse(body = rhs)) :: _
                    | SyntaxNode.SynExpr(SynExpr.LetOrUseBang(body = rhs)) :: _
                    | SyntaxNode.SynBinding(SynBinding(expr = rhs)) :: _
                    | SyntaxNode.SynMatchClause(SynMatchClause(resultExpr = rhs)) :: _ ->
                        let rhsRange = rhs.Range
                        rhsRange.StartLine <> rhsRange.EndLine && pat.Range.EndLine = rhsRange.StartLine

                    | _ -> false

            wouldMoveRhsOffsides 1 pat path
            ->
            true

        // () is parsed as this.
        | SynPat.Const(SynConst.Unit, _), _ -> true

        // (()) is required when overriding a generic member
        // where unit or a tuple type is the generic type argument:
        //
        //     type C<'T> = abstract M : 'T -> unit
        //     let _ = { new C<unit> with override _.M (()) = () }
        //     let _ = { new C<int * int> with override _.M ((x, y)) = () }
        //
        // Single versus double parens are also compiled differently in cases like these:
        //
        //     type T =
        //         static member M ()                   = () // .method public static void M()
        //         static member M (())                 = () // .method public static void M(class [FSharp.Core]Microsoft.FSharp.Core.Unit _arg1)
        //         static member M (_ : int, _ : int)   = () // .method public static void M(int32 _arg1, int32 _arg2)
        //         static member M ((_ : int, _ : int)) = () // .method public static void M(class [System.Runtime]System.Tuple`2<int32, int32> _arg1)
        | SynPat.Paren((SynPat.Const(SynConst.Unit, _) | SynPat.Tuple(isStruct = false)), _),
          SyntaxNode.SynPat(SynPat.LongIdent _) :: SyntaxNode.SynBinding _ :: _
        | SynPat.Tuple(isStruct = false),
          SyntaxNode.SynPat(SynPat.Paren _) :: SyntaxNode.SynPat(SynPat.LongIdent _) :: SyntaxNode.SynBinding _ :: _
        | SynPat.Paren((SynPat.Const(SynConst.Unit, _) | SynPat.Tuple(isStruct = false)), _),
          SyntaxNode.SynPat(SynPat.LongIdent _) :: SyntaxNode.SynBinding _ :: SyntaxNode.SynMemberDefn _ :: _ -> true

        // Not required:
        //
        //     let (a,
        //          b,
        //          c) = …
        //
        // Required:
        //
        //     let (a,
        //          b,
        //          c) =
        //         …
        | SynPat.Tuple(isStruct = false; range = innerRange), SyntaxNode.SynBinding(SynBinding(expr = body)) :: _ ->
            innerRange.StartLine <> innerRange.EndLine
            && innerRange.StartLine < body.Range.StartLine
            && body.Range.StartColumn <= innerRange.StartColumn

        // The parens could be required by a signature file like this:
        //
        //     type SemanticClassificationItem =
        //         new: (range * SemanticClassificationType) -> SemanticClassificationItem
        | SynPat.Paren(SynPat.Tuple(isStruct = false), _),
          SyntaxNode.SynPat(SynPat.LongIdent(longDotId = SynLongIdent(id = [ Ident "new" ]))) :: SyntaxNode.SynBinding _ :: SyntaxNode.SynMemberDefn _ :: SyntaxNode.SynTypeDefn _ :: _
        | SynPat.Tuple(isStruct = false),
          SyntaxNode.SynPat(SynPat.Paren _) :: SyntaxNode.SynPat(SynPat.LongIdent(longDotId = SynLongIdent(id = [ Ident "new" ]))) :: SyntaxNode.SynBinding _ :: SyntaxNode.SynMemberDefn _ :: SyntaxNode.SynTypeDefn _ :: _ ->
            true

        // Parens are required around the atomic argument of
        // any additional `new` constructor that is not the last.
        //
        //     type T … =
        //         new (x) = …
        //         new (x, y) = …
        | Atomic,
          SyntaxNode.SynPat(SynPat.LongIdent(longDotId = SynLongIdent(id = [ Ident "new" ]))) :: SyntaxNode.SynBinding _ :: SyntaxNode.SynMemberDefn _ :: SyntaxNode.SynTypeDefn(SynTypeDefn(
              typeRepr = SynTypeDefnRepr.ObjectModel(members = members))) :: _ ->
            let lastNew =
                (ValueNone, members)
                ||> List.fold (fun lastNew ``member`` ->
                    match ``member`` with
                    | SynMemberDefn.Member(
                        memberDefn = SynBinding(headPat = SynPat.LongIdent(longDotId = SynLongIdent(id = [ Ident "new" ])))) ->
                        ValueSome ``member``
                    | _ -> lastNew)

            match lastNew with
            | ValueSome(SynMemberDefn.Member(
                memberDefn = SynBinding(headPat = SynPat.LongIdent(argPats = SynArgPats.Pats [ SynPat.Paren(Is pat, _) ])))) -> false
            | _ -> true

        // Parens are otherwise never needed in these cases:
        //
        //     let (x: …) = …
        //     for (…) in (…) do …
        //     let! (…) = …
        //     and! (…) = …
        //     use! (…) = …
        //     match … with (…) -> …
        //     function (…) -> …
        //     function (Pattern …) -> …
        //     fun (x) -> …
        | _, SyntaxNode.SynBinding _ :: _
        | _, SyntaxNode.SynExpr(SynExpr.ForEach _) :: _
        | _, SyntaxNode.SynExpr(SynExpr.LetOrUseBang _) :: _
        | _, SyntaxNode.SynMatchClause _ :: _
        | Atomic, SyntaxNode.SynExpr(SynExpr.Lambda(parsedData = Some _)) :: _ -> false

        // Nested patterns.
        | inner, SyntaxNode.SynPat outer :: _ ->
            match outer, inner with
            // (x :: xs) :: ys
            // (x, xs) :: ys
            | SynPat.ListCons(lhsPat = SynPat.Paren(pat = Is inner)), SynPat.ListCons _
            | SynPat.ListCons(lhsPat = SynPat.Paren(pat = Is inner)), SynPat.Tuple(isStruct = false) -> true

            // A as (B | C)
            // A as (B & C)
            // x as (y, z)
            // xs as (y :: zs)
            | SynPat.As(rhsPat = SynPat.Paren(pat = Is inner)),
              (SynPat.Or _ | SynPat.Ands _ | SynPat.Tuple(isStruct = false) | SynPat.ListCons _) -> true

            // (A | B) :: xs
            // (A & B) :: xs
            // (x as y) :: xs
            | SynPat.ListCons _, SynPat.Or _
            | SynPat.ListCons _, SynPat.Ands _
            | SynPat.ListCons _, SynPat.As _ -> true

            // Pattern (x = (…))
            | SynPat.LongIdent(argPats = SynArgPats.NamePatPairs _), _ -> false

            // Pattern (x : int)
            // Pattern ([<Attr>] x)
            // Pattern (:? int)
            // Pattern (A :: _)
            // Pattern (A | B)
            // Pattern (A & B)
            // Pattern (A as B)
            // Pattern (A, B)
            // Pattern1 (Pattern2 (x = A))
            // Pattern1 (Pattern2 x y)
            | SynPat.LongIdent _, SynPat.Typed _
            | SynPat.LongIdent _, SynPat.Attrib _
            | SynPat.LongIdent _, SynPat.IsInst _
            | SynPat.LongIdent _, SynPat.ListCons _
            | SynPat.LongIdent _, SynPat.Or _
            | SynPat.LongIdent _, SynPat.Ands _
            | SynPat.LongIdent _, SynPat.As _
            | SynPat.LongIdent _, SynPat.Tuple(isStruct = false)
            | SynPat.LongIdent _, SynPat.LongIdent(argPats = SynArgPats.NamePatPairs _)
            | SynPat.LongIdent _, SynPat.LongIdent(argPats = SynArgPats.Pats(_ :: _))

            // A | (B as C)
            // A & (B as C)
            // A, (B as C)
            | SynPat.Or _, (SynPat.As _ | DanglingAs)
            | SynPat.Ands _, (SynPat.As _ | DanglingAs)
            | SynPat.Tuple _, (SynPat.As _ | DanglingAs)

            // x, (y, z)
            // x & (y, z)
            // (x, y) & z
            | SynPat.Tuple _, SynPat.Tuple(isStruct = false)
            | SynPat.Ands _, SynPat.Tuple(isStruct = false)

            // A, (B | C)
            // A & (B | C)
            | SynPat.Tuple _, SynPat.Or _
            | SynPat.Ands _, SynPat.Or _ -> true

            // (x : int) & y
            // x & (y : int) & z
            | SynPat.Ands(Last(SynPat.Paren(pat = Is inner)), _), SynPat.Typed _ -> false
            | SynPat.Ands _, SynPat.Typed _ -> true

            | _, SynPat.Const _
            | _, SynPat.Wild _
            | _, SynPat.Named _
            | _, SynPat.Typed _
            | _, SynPat.LongIdent(argPats = SynArgPats.Pats [])
            | _, SynPat.Tuple(isStruct = true)
            | _, SynPat.Paren _
            | _, SynPat.ArrayOrList _
            | _, SynPat.Record _
            | _, SynPat.Null _
            | _, SynPat.OptionalVal _
            | _, SynPat.IsInst _
            | _, SynPat.QuoteExpr _

            | SynPat.Or _, _
            | SynPat.ListCons _, _
            | SynPat.Ands _, _
            | SynPat.As _, _
            | SynPat.LongIdent _, _
            | SynPat.Tuple _, _
            | SynPat.Paren _, _
            | SynPat.ArrayOrList _, _
            | SynPat.Record _, _ -> false

            | _ -> true

        | _ -> true
