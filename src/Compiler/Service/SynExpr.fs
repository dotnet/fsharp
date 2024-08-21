namespace FSharp.Compiler.Syntax

open System
open FSharp.Compiler.SyntaxTrivia
open FSharp.Compiler.Text

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module SynExpr =
    let (|Last|) = List.last

    /// Matches if the two values refer to the same object.
    [<return: Struct>]
    let inline (|Is|_|) (inner1: 'a) (inner2: 'a) =
        if obj.ReferenceEquals(inner1, inner2) then
            ValueSome Is
        else
            ValueNone

    /// Represents a symbolic infix operator with the precedence of *, /, or %.
    /// All instances of this type are considered equal.
    [<CustomComparison; CustomEquality>]
    type MulDivMod =
        | Mul
        | Div
        | Mod

        member _.CompareTo(_other: MulDivMod) = 0
        override this.Equals obj = this.CompareTo(unbox obj) = 0
        override _.GetHashCode() = 0

        interface IComparable with
            member this.CompareTo obj = this.CompareTo(unbox obj)

    /// Represents a symbolic infix operator with the precedence of + or -.
    /// All instances of this type are considered equal.
    [<CustomComparison; CustomEquality>]
    type AddSub =
        | Add
        | Sub

        member _.CompareTo(_other: AddSub) = 0
        override this.Equals obj = this.CompareTo(unbox obj) = 0
        override _.GetHashCode() = 0

        interface IComparable with
            member this.CompareTo obj = this.CompareTo(unbox obj)

    /// Holds a symbolic operator's original notation.
    /// Equality is based on the contents of the string.
    /// Comparison always returns 0.
    [<CustomComparison; CustomEquality>]
    type OriginalNotation =
        | OriginalNotation of string

        member _.CompareTo(_other: OriginalNotation) = 0

        override this.Equals obj =
            match this, obj with
            | OriginalNotation this, (:? OriginalNotation as OriginalNotation other) -> String.Equals(this, other, StringComparison.Ordinal)
            | _ -> false

        override this.GetHashCode() =
            match this with
            | OriginalNotation notation -> notation.GetHashCode()

        interface IComparable with
            member this.CompareTo obj = this.CompareTo(unbox obj)

    /// Represents an expression's precedence.
    /// Comparison is based only on the precedence case.
    /// Equality considers the embedded original notation, if any.
    ///
    /// For example:
    ///
    ///     compare (AddSub (Add, OriginalNotation "+")) (AddSub (Add, OriginalNotation "++")) = 0
    ///
    /// but
    ///
    ///     AddSub (Add, OriginalNotation "+") <> AddSub (Add, OriginalNotation "++")
    type Precedence =
        /// yield, yield!, return, return!
        | Low

        /// <-
        | Set

        /// :=
        | ColonEquals

        /// ,
        | Comma

        /// or, ||
        ///
        /// Refers to the exact operators or and ||.
        /// Instances with leading dots or question marks or trailing characters are parsed as Bar instead.
        | BarBar of OriginalNotation

        /// &, &&
        ///
        /// Refers to the exact operators & and &&.
        /// Instances with leading dots or question marks or trailing characters are parsed as Amp instead.
        | AmpAmp of OriginalNotation

        /// :>, :?>
        | UpcastDowncast

        /// =…, |…, &…, $…, >…, <…, !=…
        | Relational of OriginalNotation

        /// ^…, @…
        | HatAt

        /// ::
        | Cons

        /// :?
        | TypeTest

        /// +…, -…
        | AddSub of AddSub * OriginalNotation

        /// *…, /…, %…
        | MulDivMod of MulDivMod * OriginalNotation

        /// **…
        | Exp

        /// - x
        | UnaryPrefix

        /// f x
        | Apply

        /// -x, !… x, ~~… x
        | High

        // x.y
        | Dot

    /// Associativity/association.
    type Assoc =
        /// Non-associative or no association.
        | Non

        /// Left-associative or left-hand association.
        | Left

        /// Right-associative or right-hand association.
        | Right

    module Assoc =
        let ofPrecedence precedence =
            match precedence with
            | Low -> Non
            | Set -> Non
            | ColonEquals -> Right
            | Comma -> Non
            | BarBar _ -> Left
            | AmpAmp _ -> Left
            | UpcastDowncast -> Right
            | Relational _ -> Left
            | HatAt -> Right
            | Cons -> Right
            | TypeTest -> Non
            | AddSub _ -> Left
            | MulDivMod _ -> Left
            | Exp -> Right
            | UnaryPrefix -> Left
            | Apply -> Left
            | High -> Left
            | Dot -> Left

    /// See atomicExprAfterType in pars.fsy.
    [<return: Struct>]
    let (|AtomicExprAfterType|_|) expr =
        match expr with
        | SynExpr.Paren _
        | SynExpr.Quote _
        | SynExpr.Const _
        | SynExpr.Tuple(isStruct = true)
        | SynExpr.Record _
        | SynExpr.AnonRecd _
        | SynExpr.InterpolatedString _
        | SynExpr.Null _
        | SynExpr.ArrayOrList(isArray = true)
        | SynExpr.ArrayOrListComputed(isArray = true) -> ValueSome AtomicExprAfterType
        | _ -> ValueNone

    /// Matches if the given expression represents a high-precedence
    /// function application, e.g.,
    ///
    /// f x
    ///
    /// (+) x y
    [<return: Struct>]
    let (|HighPrecedenceApp|_|) expr =
        match expr with
        | SynExpr.App(isInfix = false; funcExpr = SynExpr.Ident _)
        | SynExpr.App(isInfix = false; funcExpr = SynExpr.LongIdent _)
        | SynExpr.App(isInfix = false; funcExpr = SynExpr.App(isInfix = false)) -> ValueSome HighPrecedenceApp
        | _ -> ValueNone

    module FuncExpr =
        /// Matches when the given funcExpr is a direct application
        /// of a symbolic operator, e.g., -, _not_ (~-).
        [<return: Struct>]
        let (|SymbolicOperator|_|) funcExpr =
            match funcExpr with
            | SynExpr.LongIdent(longDotId = SynLongIdent(trivia = trivia)) ->
                let rec tryPick =
                    function
                    | [] -> ValueNone
                    | Some(IdentTrivia.OriginalNotation op) :: _ -> ValueSome op
                    | _ :: rest -> tryPick rest

                tryPick trivia
            | _ -> ValueNone

    /// Matches when the given expression is a prefix operator application, e.g.,
    ///
    /// -x
    ///
    /// ~~~x
    [<return: Struct>]
    let (|PrefixApp|_|) expr : Precedence voption =
        match expr with
        | SynExpr.App(isInfix = false; funcExpr = funcExpr & FuncExpr.SymbolicOperator op; argExpr = argExpr) ->
            if funcExpr.Range.IsAdjacentTo argExpr.Range then
                ValueSome High
            else
                assert (op.Length > 0)

                match op[0] with
                | '!'
                | '~' -> ValueSome High
                | _ -> ValueSome UnaryPrefix

        | SynExpr.AddressOf(expr = expr; opRange = opRange) ->
            if opRange.IsAdjacentTo expr.Range then
                ValueSome High
            else
                ValueSome UnaryPrefix

        | _ -> ValueNone

    /// Tries to parse the given original notation as a symbolic infix operator.
    [<return: Struct>]
    let (|SymbolPrec|_|) (originalNotation: string) =
        // Trim any leading dots or question marks from the given symbolic operator.
        // Leading dots or question marks have no effect on operator precedence or associativity
        // with the exception of &, &&, and ||.
        let ignoredLeadingChars = ".?".AsSpan()
        let trimmed = originalNotation.AsSpan().TrimStart ignoredLeadingChars
        assert (trimmed.Length > 0)

        match trimmed[0], originalNotation with
        | _, ":=" -> ValueSome ColonEquals
        | _, ("||" | "or") -> ValueSome(BarBar(OriginalNotation originalNotation))
        | _, ("&" | "&&") -> ValueSome(AmpAmp(OriginalNotation originalNotation))
        | '|', _
        | '&', _
        | '<', _
        | '>', _
        | '=', _
        | '$', _ -> ValueSome(Relational(OriginalNotation originalNotation))
        | '!', _ when trimmed.Length > 1 && trimmed[1] = '=' -> ValueSome(Relational(OriginalNotation originalNotation))
        | '^', _
        | '@', _ -> ValueSome HatAt
        | _, "::" -> ValueSome Cons
        | '+', _ -> ValueSome(AddSub(Add, OriginalNotation originalNotation))
        | '-', _ -> ValueSome(AddSub(Sub, OriginalNotation originalNotation))
        | '/', _ -> ValueSome(MulDivMod(Div, OriginalNotation originalNotation))
        | '%', _ -> ValueSome(MulDivMod(Mod, OriginalNotation originalNotation))
        | '*', _ when trimmed.Length > 1 && trimmed[1] = '*' -> ValueSome Exp
        | '*', _ -> ValueSome(MulDivMod(Mul, OriginalNotation originalNotation))
        | _ -> ValueNone

    [<return: Struct>]
    let (|Contains|_|) (c: char) (s: string) =
        if s.IndexOf c >= 0 then ValueSome Contains else ValueNone

    /// Any expressions in which the removal of parens would
    /// lead to something like the following that would be
    /// confused by the parser with a type parameter application:
    ///
    /// x<y>z
    ///
    /// x<y,y>z
    [<return: Struct>]
    let rec (|ConfusableWithTypeApp|_|) synExpr =
        match synExpr with
        | SynExpr.Paren(expr = ConfusableWithTypeApp)
        | SynExpr.App(funcExpr = ConfusableWithTypeApp)
        | SynExpr.App(isInfix = true; funcExpr = FuncExpr.SymbolicOperator(Contains '>'); argExpr = ConfusableWithTypeApp) ->
            ValueSome ConfusableWithTypeApp
        | SynExpr.App(isInfix = true; funcExpr = funcExpr & FuncExpr.SymbolicOperator(Contains '<'); argExpr = argExpr) when
            argExpr.Range.IsAdjacentTo funcExpr.Range
            ->
            ValueSome ConfusableWithTypeApp
        | SynExpr.Tuple(exprs = exprs) ->
            let rec anyButLast =
                function
                | _ :: []
                | [] -> ValueNone
                | ConfusableWithTypeApp :: _ -> ValueSome ConfusableWithTypeApp
                | _ :: tail -> anyButLast tail

            anyButLast exprs
        | _ -> ValueNone

    /// Matches when the expression represents the infix application of a symbolic operator.
    ///
    /// (x λ y) ρ z
    ///
    /// x λ (y ρ z)
    [<return: Struct>]
    let (|InfixApp|_|) synExpr : struct (Precedence * Assoc) voption =
        match synExpr with
        | SynExpr.App(funcExpr = SynExpr.App(isInfix = true; funcExpr = FuncExpr.SymbolicOperator(SymbolPrec prec))) ->
            ValueSome(prec, Right)
        | SynExpr.App(isInfix = true; funcExpr = FuncExpr.SymbolicOperator(SymbolPrec prec)) -> ValueSome(prec, Left)
        | SynExpr.Upcast _
        | SynExpr.Downcast _ -> ValueSome(UpcastDowncast, Left)
        | SynExpr.TypeTest _ -> ValueSome(TypeTest, Left)
        | _ -> ValueNone

    /// Returns the given expression's precedence and the side of the inner expression,
    /// if applicable.
    [<return: Struct>]
    let (|OuterBinaryExpr|_|) inner outer : struct (Precedence * Assoc) voption =
        match outer with
        | SynExpr.YieldOrReturn _
        | SynExpr.YieldOrReturnFrom _ -> ValueSome(Low, Right)
        | SynExpr.Tuple(exprs = SynExpr.Paren(expr = Is inner) :: _) -> ValueSome(Comma, Left)
        | SynExpr.Tuple _ -> ValueSome(Comma, Right)
        | InfixApp(Cons, side) -> ValueSome(Cons, side)
        | SynExpr.Assert _
        | SynExpr.Lazy _
        | SynExpr.InferredUpcast _
        | SynExpr.InferredDowncast _ -> ValueSome(Apply, Non)
        | PrefixApp prec -> ValueSome(prec, Non)
        | InfixApp(prec, side) -> ValueSome(prec, side)
        | SynExpr.App(argExpr = SynExpr.ComputationExpr _) -> ValueSome(UnaryPrefix, Left)
        | SynExpr.App(funcExpr = SynExpr.Paren(expr = SynExpr.App _)) -> ValueSome(Apply, Left)
        | SynExpr.App(flag = ExprAtomicFlag.Atomic) -> ValueSome(Dot, Non)
        | SynExpr.App _ -> ValueSome(Apply, Non)
        | SynExpr.DotSet(targetExpr = SynExpr.Paren(expr = Is inner)) -> ValueSome(Dot, Left)
        | SynExpr.DotSet(rhsExpr = SynExpr.Paren(expr = Is inner)) -> ValueSome(Set, Right)
        | SynExpr.DotIndexedSet(objectExpr = SynExpr.Paren(expr = Is inner))
        | SynExpr.DotNamedIndexedPropertySet(targetExpr = SynExpr.Paren(expr = Is inner)) -> ValueSome(Dot, Left)
        | SynExpr.DotIndexedSet(valueExpr = SynExpr.Paren(expr = Is inner))
        | SynExpr.DotNamedIndexedPropertySet(rhsExpr = SynExpr.Paren(expr = Is inner)) -> ValueSome(Set, Right)
        | SynExpr.LongIdentSet(expr = SynExpr.Paren(expr = Is inner)) -> ValueSome(Set, Right)
        | SynExpr.Set _ -> ValueSome(Set, Non)
        | SynExpr.DotGet _ -> ValueSome(Dot, Left)
        | SynExpr.DotIndexedGet(objectExpr = SynExpr.Paren(expr = Is inner)) -> ValueSome(Dot, Left)
        | _ -> ValueNone

    /// Matches a SynExpr.App nested in a sequence of dot-gets.
    ///
    /// x.M.N().O
    [<return: Struct>]
    let (|NestedApp|_|) expr =
        let rec loop =
            function
            | SynExpr.DotGet(expr = expr)
            | SynExpr.DotIndexedGet(objectExpr = expr) -> loop expr
            | SynExpr.App _ -> ValueSome NestedApp
            | _ -> ValueNone

        loop expr

    /// Returns the given expression's precedence, if applicable.
    [<return: Struct>]
    let (|InnerBinaryExpr|_|) expr : Precedence voption =
        match expr with
        | SynExpr.Tuple(isStruct = false) -> ValueSome Comma
        | SynExpr.DotGet(expr = NestedApp)
        | SynExpr.DotIndexedGet(objectExpr = NestedApp) -> ValueSome Apply
        | SynExpr.DotGet _
        | SynExpr.DotIndexedGet _ -> ValueSome Dot
        | PrefixApp prec -> ValueSome prec
        | InfixApp(prec, _) -> ValueSome prec
        | SynExpr.App _
        | SynExpr.Assert _
        | SynExpr.Lazy _
        | SynExpr.For _
        | SynExpr.ForEach _
        | SynExpr.While _
        | SynExpr.Do _
        | SynExpr.New _
        | SynExpr.InferredUpcast _
        | SynExpr.InferredDowncast _ -> ValueSome Apply
        | SynExpr.DotIndexedSet _
        | SynExpr.DotNamedIndexedPropertySet _
        | SynExpr.DotSet _ -> ValueSome Set
        | _ -> ValueNone

    module Dangling =
        /// Returns the first matching nested right-hand target expression, if any.
        let private dangling (target: SynExpr -> SynExpr option) =
            let (|Target|_|) = target

            let rec loop expr =
                match expr with
                | Target expr -> ValueSome expr
                | SynExpr.Tuple(isStruct = false; exprs = Last expr)
                | SynExpr.App(argExpr = expr)
                | SynExpr.IfThenElse(elseExpr = Some expr)
                | SynExpr.IfThenElse(ifExpr = expr)
                | SynExpr.Sequential(expr2 = expr)
                | SynExpr.YieldOrReturn(expr = expr)
                | SynExpr.YieldOrReturnFrom(expr = expr)
                | SynExpr.Set(rhsExpr = expr)
                | SynExpr.DotSet(rhsExpr = expr)
                | SynExpr.DotNamedIndexedPropertySet(rhsExpr = expr)
                | SynExpr.DotIndexedSet(valueExpr = expr)
                | SynExpr.LongIdentSet(expr = expr)
                | SynExpr.LetOrUse(body = expr)
                | SynExpr.Lambda(body = expr)
                | SynExpr.Match(clauses = Last(SynMatchClause(resultExpr = expr)))
                | SynExpr.MatchLambda(matchClauses = Last(SynMatchClause(resultExpr = expr)))
                | SynExpr.MatchBang(clauses = Last(SynMatchClause(resultExpr = expr)))
                | SynExpr.TryWith(withCases = Last(SynMatchClause(resultExpr = expr)))
                | SynExpr.TryFinally(finallyExpr = expr)
                | SynExpr.Do(expr = expr)
                | SynExpr.DoBang(expr = expr) -> loop expr
                | _ -> ValueNone

            loop

        /// Matches a dangling if-then construct.
        [<return: Struct>]
        let (|IfThen|_|) =
            dangling (function
                | SynExpr.IfThenElse _ as expr -> Some expr
                | _ -> None)

        /// Matches a dangling let or use construct.
        [<return: Struct>]
        let (|LetOrUse|_|) =
            dangling (function
                | SynExpr.LetOrUse _
                | SynExpr.LetOrUseBang _ as expr -> Some expr
                | _ -> None)

        /// Matches a dangling sequential expression.
        [<return: Struct>]
        let (|Sequential|_|) =
            dangling (function
                | SynExpr.Sequential _ as expr -> Some expr
                | _ -> None)

        /// Matches a dangling try-with or try-finally construct.
        [<return: Struct>]
        let (|Try|_|) =
            dangling (function
                | SynExpr.TryWith _
                | SynExpr.TryFinally _ as expr -> Some expr
                | _ -> None)

        /// Matches a dangling match-like construct.
        [<return: Struct>]
        let (|Match|_|) =
            dangling (function
                | SynExpr.Match _
                | SynExpr.MatchBang _
                | SynExpr.MatchLambda _
                | SynExpr.TryWith _
                | SynExpr.Lambda _ as expr -> Some expr
                | _ -> None)

        /// Matches a dangling arrow-sensitive construct.
        [<return: Struct>]
        let (|ArrowSensitive|_|) =
            dangling (function
                | SynExpr.Match _
                | SynExpr.MatchBang _
                | SynExpr.MatchLambda _
                | SynExpr.TryWith _
                | SynExpr.Lambda _
                | SynExpr.Typed _
                | SynExpr.TypeTest _
                | SynExpr.Upcast _
                | SynExpr.Downcast _ as expr -> Some expr
                | _ -> None)

        /// Matches a nested dangling construct that could become problematic
        /// if the surrounding parens were removed.
        [<return: Struct>]
        let (|Problematic|_|) =
            dangling (function
                | SynExpr.Lambda _
                | SynExpr.MatchLambda _
                | SynExpr.Match _
                | SynExpr.MatchBang _
                | SynExpr.TryWith _
                | SynExpr.TryFinally _
                | SynExpr.IfThenElse _
                | SynExpr.Sequential _
                | SynExpr.LetOrUse _
                | SynExpr.Set _
                | SynExpr.LongIdentSet _
                | SynExpr.DotIndexedSet _
                | SynExpr.DotNamedIndexedPropertySet _
                | SynExpr.DotSet _
                | SynExpr.NamedIndexedPropertySet _ as expr -> Some expr
                | _ -> None)

    /// Indicates whether the expression with the given range
    /// includes indentation that would be invalid
    /// in context if it were not wrapped in parentheses.
    let containsSensitiveIndentation (getSourceLineStr: int -> string) outerOffsidesColumn (range: range) =
        let startLine = range.StartLine
        let endLine = range.EndLine

        if startLine = endLine then
            range.StartColumn <= outerOffsidesColumn
        else
            let rec loop offsides lineNo (startCol: int) =
                if lineNo <= endLine then
                    let line = getSourceLineStr lineNo

                    match offsides with
                    | ValueNone ->
                        let i = line.AsSpan(startCol).IndexOfAnyExcept(' ', ')')

                        if i >= 0 then
                            let newOffsides = i + startCol

                            newOffsides <= outerOffsidesColumn
                            || loop (ValueSome newOffsides) (lineNo + 1) 0
                        else
                            loop offsides (lineNo + 1) 0

                    | ValueSome offsidesCol ->
                        let i = line.AsSpan(0, min offsidesCol line.Length).IndexOfAnyExcept(' ', ')')

                        if i >= 0 && i < offsidesCol then
                            let slice = line.AsSpan(i, min (offsidesCol - i) (line.Length - i))
                            let j = slice.IndexOfAnyExcept("*/%-+:^@><=!|0$.?".AsSpan())

                            let lo = i + (if j >= 0 && slice[j] = ' ' then j else 0)

                            lo < offsidesCol - 1
                            || lo <= outerOffsidesColumn
                            || loop offsides (lineNo + 1) 0
                        else
                            loop offsides (lineNo + 1) 0
                else
                    false

            loop ValueNone startLine range.StartColumn

    /// Matches constructs that are sensitive to
    /// certain kinds of undentation in sequential expressions.
    [<return: Struct>]
    let (|UndentationSensitive|_|) expr =
        match expr with
        | SynExpr.TryWith _
        | SynExpr.TryFinally _
        | SynExpr.For _
        | SynExpr.ForEach _
        | SynExpr.IfThenElse _
        | SynExpr.Match _
        | SynExpr.While _
        | SynExpr.Do _ -> ValueSome UndentationSensitive
        | _ -> ValueNone

    let rec shouldBeParenthesizedInContext (getSourceLineStr: int -> string) path expr : bool =
        let shouldBeParenthesizedInContext = shouldBeParenthesizedInContext getSourceLineStr
        let containsSensitiveIndentation = containsSensitiveIndentation getSourceLineStr

        let (|StartsWith|) (s: string) = s[0]

        // Matches if the given expression starts with a symbol, e.g., <@ … @>, $"…", @"…", +1, -1…
        let (|StartsWithSymbol|_|) =
            let (|TextStartsWith|) (m: range) =
                let line = getSourceLineStr m.StartLine
                line[m.StartColumn]

            function
            | SynExpr.Quote _
            | SynExpr.InterpolatedString _
            | SynExpr.Const(SynConst.String(synStringKind = SynStringKind.Verbatim), _)
            | SynExpr.Const(SynConst.Byte _, TextStartsWith '+')
            | SynExpr.Const(SynConst.UInt16 _, TextStartsWith '+')
            | SynExpr.Const(SynConst.UInt32 _, TextStartsWith '+')
            | SynExpr.Const(SynConst.UInt64 _, TextStartsWith '+')
            | SynExpr.Const(SynConst.UIntPtr _, TextStartsWith '+')
            | SynExpr.Const(SynConst.SByte _, TextStartsWith('-' | '+'))
            | SynExpr.Const(SynConst.Int16 _, TextStartsWith('-' | '+'))
            | SynExpr.Const(SynConst.Int32 _, TextStartsWith('-' | '+'))
            | SynExpr.Const(SynConst.Int64 _, TextStartsWith('-' | '+'))
            | SynExpr.Const(SynConst.IntPtr _, TextStartsWith('-' | '+'))
            | SynExpr.Const(SynConst.Decimal _, TextStartsWith('-' | '+'))
            | SynExpr.Const(SynConst.Double _, TextStartsWith('-' | '+'))
            | SynExpr.Const(SynConst.Single _, TextStartsWith('-' | '+'))
            | SynExpr.Const(SynConst.Measure(_, TextStartsWith('-' | '+'), _, _), _)
            | SynExpr.Const(SynConst.UserNum(StartsWith('-' | '+'), _), _) -> Some StartsWithSymbol
            | _ -> None

        // Matches if the given expression is a numeric literal
        // that it is safe to "dot into," e.g., 1l, 0b1, 1e10, 1d, 1.0…
        let (|DotSafeNumericLiteral|_|) =
            /// 1l, 1d, 0b1, 0x1, 0o1, 1e10…
            let (|TextContainsLetter|_|) (m: range) =
                let line = getSourceLineStr m.StartLine
                let span = line.AsSpan(m.StartColumn, m.EndColumn - m.StartColumn)

                if span.LastIndexOfAnyInRange('A', 'z') >= 0 then
                    Some TextContainsLetter
                else
                    None

            // 1.0…
            let (|TextEndsWithNumber|_|) (m: range) =
                let line = getSourceLineStr m.StartLine
                let span = line.AsSpan(m.StartColumn, m.EndColumn - m.StartColumn)

                if Char.IsDigit span[span.Length - 1] then
                    Some TextEndsWithNumber
                else
                    None

            function
            | SynExpr.Const(SynConst.Byte _, _)
            | SynExpr.Const(SynConst.UInt16 _, _)
            | SynExpr.Const(SynConst.UInt32 _, _)
            | SynExpr.Const(SynConst.UInt64 _, _)
            | SynExpr.Const(SynConst.UIntPtr _, _)
            | SynExpr.Const(SynConst.SByte _, _)
            | SynExpr.Const(SynConst.Int16 _, _)
            | SynExpr.Const(SynConst.Int32 _, TextContainsLetter)
            | SynExpr.Const(SynConst.Int64 _, _)
            | SynExpr.Const(SynConst.IntPtr _, _)
            | SynExpr.Const(SynConst.Decimal _, _)
            | SynExpr.Const(SynConst.Double _, (TextEndsWithNumber | TextContainsLetter))
            | SynExpr.Const(SynConst.Single _, _)
            | SynExpr.Const(SynConst.Measure _, _)
            | SynExpr.Const(SynConst.UserNum _, _) -> Some DotSafeNumericLiteral
            | _ -> None

        match expr, path with
        // Parens must stay around binary equals expressions in argument
        // position lest they be interpreted as named argument assignments:
        //
        //     o.M((x = y))
        //     o.N((x = y), z)
        //
        // Likewise, double parens must stay around a tuple, since we don't know whether
        // the method being invoked might have a signature like
        //
        //     val TryGetValue : 'Key * outref<'Value> -> bool
        //
        // where 'Key is 'a * 'b, in which case the double parens are required.
        | SynExpr.Paren(expr = InfixApp(Relational(OriginalNotation "="), _)),
          SyntaxNode.SynExpr(SynExpr.App(funcExpr = SynExpr.LongIdent _ | SynExpr.DotGet _ | SynExpr.Ident _)) :: _
        | InfixApp(Relational(OriginalNotation "="), _),
          SyntaxNode.SynExpr(SynExpr.Paren _) :: SyntaxNode.SynExpr(SynExpr.App(
              funcExpr = SynExpr.LongIdent _ | SynExpr.DotGet _ | SynExpr.Ident _)) :: _
        | InfixApp(Relational(OriginalNotation "="), _),
          SyntaxNode.SynExpr(SynExpr.Tuple(isStruct = false)) :: SyntaxNode.SynExpr(SynExpr.Paren _) :: SyntaxNode.SynExpr(SynExpr.App(
              funcExpr = SynExpr.LongIdent _ | SynExpr.DotGet _ | SynExpr.Ident _)) :: _
        | SynExpr.Paren(expr = SynExpr.Tuple(isStruct = false)),
          SyntaxNode.SynExpr(SynExpr.App(funcExpr = SynExpr.LongIdent _ | SynExpr.DotGet _ | SynExpr.Ident _)) :: _
        | SynExpr.Tuple(isStruct = false),
          SyntaxNode.SynExpr(SynExpr.Paren _) :: SyntaxNode.SynExpr(SynExpr.App(
              funcExpr = SynExpr.LongIdent _ | SynExpr.DotGet _ | SynExpr.Ident _)) :: _
        | SynExpr.Const(SynConst.Unit, _),
          SyntaxNode.SynExpr(SynExpr.App(funcExpr = SynExpr.LongIdent _ | SynExpr.DotGet _ | SynExpr.Ident _)) :: _ -> true

        // Already parenthesized.
        | _, SyntaxNode.SynExpr(SynExpr.Paren _) :: _ -> false

        // Parens must stay around indentation that would otherwise be invalid:
        //
        //     let _ = (x
        //            +y)
        | _, SyntaxNode.SynBinding(SynBinding(trivia = trivia)) :: _ when
            containsSensitiveIndentation trivia.LeadingKeyword.Range.StartColumn expr.Range
            ->
            true

        // Parens must stay around indentation that would otherwise be invalid:
        //
        //     return (
        //     x
        //     )
        | _, SyntaxNode.SynExpr(SynExpr.YieldOrReturn _ as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.YieldOrReturnFrom _ as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.Assert _ as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.Lazy _ as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.App(argExpr = SynExpr.Paren(expr = Is expr)) as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.LetOrUse _ as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.LetOrUseBang _ as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.TryWith _ as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.TryFinally _ as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.For _ as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.ForEach _ as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.IfThenElse _ as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.New _ as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.Set(rhsExpr = SynExpr.Paren(expr = Is expr)) as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.DotIndexedSet(valueExpr = SynExpr.Paren(expr = Is expr)) as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.DotNamedIndexedPropertySet(rhsExpr = SynExpr.Paren(expr = Is expr)) as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.DotSet(rhsExpr = SynExpr.Paren(expr = Is expr)) as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.LibraryOnlyUnionCaseFieldSet(rhsExpr = SynExpr.Paren(expr = Is expr)) as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.LongIdentSet(expr = SynExpr.Paren(expr = Is expr)) as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.NamedIndexedPropertySet(expr2 = SynExpr.Paren(expr = Is expr)) as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.InferredUpcast _ as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.InferredDowncast _ as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.Match _ as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.MatchBang _ as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.While _ as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.WhileBang _ as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.Do _ as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.DoBang _ as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.Fixed _ as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.Record _ as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.AnonRecd _ as outer) :: _
        | _, SyntaxNode.SynExpr(SynExpr.InterpolatedString _ as outer) :: _ when
            containsSensitiveIndentation outer.Range.StartColumn expr.Range
            ->
            true

        // Hanging tuples:
        //
        //     let _ =
        //         (
        //             1, 2,
        //           3, 4
        //         )
        //
        // or
        //
        //     [
        //         1, 2,
        //         3, 4
        //         (1, 2,
        //        3, 4)
        //     ]
        | SynExpr.Tuple(isStruct = false; exprs = exprs; range = range), _ when
            range.StartLine <> range.EndLine
            && exprs |> List.exists (fun e -> e.Range.StartColumn < range.StartColumn)
            ->
            true

        // There are certain constructs whose indentation in
        // a sequential expression is valid when parenthesized
        // (and separated from the following expression by a semicolon),
        // but where the parsing of the outer expression would change if
        // the parentheses were removed in place, e.g.,
        //
        //     [
        //        (if p then q else r); // Cannot remove in place because of the indentation of y below.
        //         y
        //     ]
        //
        // or
        //
        //     [
        //         x;
        //        (if p then q else r); // Cannot remove in place because of the indentation of x above.
        //         z
        //     ]
        //
        // This analysis is imperfect in that it sometimes requires parens when they
        // may not be required, but the only way to know for sure in such cases would be to walk up
        // an unknown number of ancestral sequential expressions to check for problematic
        // indentation (or to keep track of the offsides line throughout the AST traversal):
        //
        //     [
        //         x;                   // This line's indentation means we cannot remove below.
        //        (…);
        //        (…);
        //        (* 𝑛 more such lines. *)
        //        (…);
        //        (…);
        //        (if p then q else r); // Can no longer remove here because of the indentation of x above.
        //        z
        //     ]
        | UndentationSensitive,
          SyntaxNode.SynExpr(SynExpr.Sequential(expr1 = SynExpr.Paren(expr = Is expr))) :: SyntaxNode.SynExpr(SynExpr.Sequential(
              expr1 = SynExpr.Paren(expr = other) | other)) :: _
        | UndentationSensitive,
          SyntaxNode.SynExpr(SynExpr.Sequential(expr1 = SynExpr.Paren(expr = Is expr); expr2 = other)) :: SyntaxNode.SynExpr(SynExpr.Sequential _) :: _
        | UndentationSensitive,
          SyntaxNode.SynExpr(SynExpr.Sequential(expr1 = SynExpr.Paren(expr = Is expr); expr2 = other)) :: SyntaxNode.SynExpr(SynExpr.ArrayOrListComputed _) :: _
        | UndentationSensitive,
          SyntaxNode.SynExpr(SynExpr.Sequential(expr1 = SynExpr.Paren(expr = Is expr); expr2 = other)) :: SyntaxNode.SynExpr(SynExpr.ArrayOrList _) :: _
        | UndentationSensitive,
          SyntaxNode.SynExpr(SynExpr.Sequential(expr1 = SynExpr.Paren(expr = Is expr); expr2 = other)) :: SyntaxNode.SynExpr(SynExpr.ComputationExpr _) :: _ when
            expr.Range.StartLine <> other.Range.StartLine
            && expr.Range.StartColumn <= other.Range.StartColumn
            ->
            true

        // Check for nested matches, e.g.,
        //
        //     match … with … -> (…, match … with … -> … | … -> …) | … -> …
        | _, SyntaxNode.SynMatchClause _ :: path -> shouldBeParenthesizedInContext path expr

        // We always need parens for trait calls, e.g.,
        //
        //     let inline f x = (^a : (static member Parse : string -> ^a) x)
        | SynExpr.TraitCall _, _ -> true

        // Don't touch library-only stuff:
        //
        //     (# "ldlen.multi 2 0" array : int #)
        | SynExpr.LibraryOnlyILAssembly _, _
        | SynExpr.LibraryOnlyStaticOptimization _, _
        | SynExpr.LibraryOnlyUnionCaseFieldGet _, _
        | SynExpr.LibraryOnlyUnionCaseFieldSet _, _ -> true

        // Parens are otherwise never required for binding bodies or for top-level expressions, e.g.,
        //
        //     let x = (…)
        //     _.member X = (…)
        //     (printfn "Hello, world.")
        | _, SyntaxNode.SynBinding _ :: _
        | _, SyntaxNode.SynModule _ :: _ -> false

        // Parens must be kept when there is a high-precedence function application
        // before a prefix operator application before another expression that starts with a symbol, e.g.,
        //
        //     id -(-x)
        //     id -(-1y)
        //     id -($"")
        //     id -(@"")
        //     id -(<@ ValueNone @>)
        //     let (~+) _ = true in assert +($"{true}")
        | (PrefixApp _ | StartsWithSymbol),
          SyntaxNode.SynExpr(SynExpr.App _) :: SyntaxNode.SynExpr(HighPrecedenceApp | SynExpr.Assert _ | SynExpr.InferredUpcast _ | SynExpr.InferredDowncast _) :: _ ->
            true

        // Parens must be kept in a scenario like
        //
        //     !x.M(y)
        //     ~~~x.M(y)
        //
        // since prefix ! or ~~~ (with no space) have higher
        // precedence than regular function application.
        | _, SyntaxNode.SynExpr(SynExpr.App _) :: SyntaxNode.SynExpr(PrefixApp High) :: _ -> true

        // Parens are never required around suffixed or infixed numeric literals, e.g.,
        //
        //     (1l).ToString()
        //     (1uy).ToString()
        //     (0b1).ToString()
        //     (1e10).ToString()
        //     (1.0).ToString()
        | DotSafeNumericLiteral, _ -> false

        // Parens are required around bare decimal ints or doubles ending
        // in dots when being dotted into, e.g.,
        //
        //     (1).ToString()
        //     (1.).ToString()
        | SynExpr.Const(constant = SynConst.Int32 _ | SynConst.Double _), SyntaxNode.SynExpr(SynExpr.DotGet _) :: _ -> true

        // Parens are required around join conditions:
        //
        //     join … on (… = …)
        | SynExpr.App _, SyntaxNode.SynExpr(SynExpr.App _) :: SyntaxNode.SynExpr(SynExpr.JoinIn _) :: _ -> true

        // Parens are not required around a few anointed expressions after inherit:
        //
        //     inherit T(3)
        //     inherit T(null)
        //     inherit T("")
        //     …
        | AtomicExprAfterType, SyntaxNode.SynMemberDefn(SynMemberDefn.ImplicitInherit _) :: _ -> false

        // Parens are otherwise required in inherit T(x), etc.
        | _, SyntaxNode.SynMemberDefn(SynMemberDefn.ImplicitInherit _) :: _ -> true

        // We can't remove parens when they're required for fluent calls:
        //
        //     x.M(y).N z
        //     x.M(y).[z]
        //     _.M(x)
        //     (f x)[z]
        //     (f(x))[z]
        //     x.M(y)[z]
        //     M(x).N <- y
        | SynExpr.App _, SyntaxNode.SynExpr(SynExpr.App(argExpr = SynExpr.ArrayOrListComputed(isArray = false))) :: _ -> true

        | _, SyntaxNode.SynExpr(SynExpr.App _) :: path
        | _, SyntaxNode.SynExpr(OuterBinaryExpr expr (Dot, _)) :: SyntaxNode.SynExpr(SynExpr.App _) :: path when
            let rec appChainDependsOnDotOrPseudoDotPrecedence path =
                match path with
                | SyntaxNode.SynExpr(SynExpr.DotGet _) :: _
                | SyntaxNode.SynExpr(SynExpr.DotLambda _) :: _
                | SyntaxNode.SynExpr(SynExpr.DotIndexedGet _) :: _
                | SyntaxNode.SynExpr(SynExpr.Set _) :: _
                | SyntaxNode.SynExpr(SynExpr.DotSet _) :: _
                | SyntaxNode.SynExpr(SynExpr.DotIndexedSet _) :: _
                | SyntaxNode.SynExpr(SynExpr.DotNamedIndexedPropertySet _) :: _
                | SyntaxNode.SynExpr(SynExpr.App(argExpr = SynExpr.ArrayOrListComputed(isArray = false))) :: _ -> true
                | SyntaxNode.SynExpr(SynExpr.App _) :: path -> appChainDependsOnDotOrPseudoDotPrecedence path
                | _ -> false

            appChainDependsOnDotOrPseudoDotPrecedence path
            ->
            true

        // The :: operator is parsed differently from other symbolic infix operators,
        // so we need to give it special treatment.

        // Outer right:
        //
        //     (x) :: xs
        //     (x * y) :: zs
        //     …
        | _,
          SyntaxNode.SynExpr(SynExpr.Tuple(isStruct = false; exprs = [ SynExpr.Paren _; _ ])) :: (SyntaxNode.SynExpr(SynExpr.App(
              isInfix = true)) :: _ as path) -> shouldBeParenthesizedInContext path expr

        // Outer left:
        //
        //     x :: (xs)
        //     x :: (ys @ zs)
        //     …
        | argExpr,
          SyntaxNode.SynExpr(SynExpr.Tuple(isStruct = false; exprs = [ _; SynExpr.Paren _ ])) :: SyntaxNode.SynExpr(SynExpr.App(
              isInfix = true) as outer) :: path ->
            shouldBeParenthesizedInContext
                (SyntaxNode.SynExpr(SynExpr.App(ExprAtomicFlag.NonAtomic, false, outer, argExpr, outer.Range))
                 :: path)
                expr

        // Ordinary nested expressions.
        | inner, SyntaxNode.SynExpr outer :: outerPath ->
            let dangling expr =
                match expr with
                | Dangling.Problematic subExpr ->
                    match outer with
                    | SynExpr.Tuple(exprs = exprs) -> not (obj.ReferenceEquals(subExpr, List.last exprs))
                    | InfixApp(_, Left) -> true
                    | _ -> shouldBeParenthesizedInContext outerPath subExpr

                | _ -> false

            let problematic (exprRange: range) (delimiterRange: range) =
                exprRange.EndLine = delimiterRange.EndLine
                && exprRange.EndColumn < delimiterRange.StartColumn

            let anyProblematic matchOrTryRange clauses =
                let rec loop =
                    function
                    | [] -> false
                    | SynMatchClause(trivia = trivia) :: clauses ->
                        trivia.BarRange |> Option.exists (problematic matchOrTryRange)
                        || trivia.ArrowRange |> Option.exists (problematic matchOrTryRange)
                        || loop clauses

                loop clauses

            let innerBindingsWouldShadowOuter expr1 (expr2: SynExpr) =
                let identsBoundInInner =
                    (Set.empty, [ SyntaxNode.SynExpr expr1 ])
                    ||> SyntaxNodes.fold (fun idents _path node ->
                        match node with
                        | SyntaxNode.SynPat(SynPat.Named(ident = SynIdent(ident = ident))) -> idents.Add ident.idText
                        | _ -> idents)

                if identsBoundInInner.IsEmpty then
                    false
                else
                    (expr2.Range.End, [ SyntaxNode.SynExpr expr2 ])
                    ||> SyntaxNodes.exists (fun _path node ->
                        match node with
                        | SyntaxNode.SynExpr(SynExpr.Ident ident) -> identsBoundInInner.Contains ident.idText
                        | _ -> false)

            match outer, inner with
            | ConfusableWithTypeApp, _ -> true

            | SynExpr.IfThenElse(trivia = trivia), Dangling.LetOrUse letOrUse ->
                Position.posLt letOrUse.Range.Start trivia.ThenKeyword.Start

            | SynExpr.IfThenElse(trivia = trivia), Dangling.IfThen dangling
            | SynExpr.IfThenElse(trivia = trivia), Dangling.Match dangling ->
                problematic dangling.Range trivia.ThenKeyword
                || trivia.ElseKeyword |> Option.exists (problematic dangling.Range)

            | SynExpr.IfThenElse(ifExpr = expr), Dangling.Sequential dangling
            | SynExpr.While(whileExpr = expr), Dangling.Problematic dangling
            | SynExpr.ForEach(enumExpr = expr), Dangling.Problematic dangling -> Range.rangeContainsRange expr.Range dangling.Range

            | SynExpr.TryFinally(trivia = trivia), Dangling.Try tryExpr when problematic tryExpr.Range trivia.FinallyKeyword -> true

            | SynExpr.Match(clauses = clauses; trivia = { WithKeyword = withKeyword }), Dangling.ArrowSensitive dangling when
                problematic dangling.Range withKeyword || anyProblematic dangling.Range clauses
                ->
                true

            | SynExpr.MatchBang(clauses = clauses; trivia = { WithKeyword = withKeyword }), Dangling.ArrowSensitive dangling when
                problematic dangling.Range withKeyword || anyProblematic dangling.Range clauses
                ->
                true

            | SynExpr.MatchLambda(matchClauses = clauses), Dangling.ArrowSensitive dangling when anyProblematic dangling.Range clauses ->
                true

            | SynExpr.TryWith(withCases = clauses; trivia = trivia), Dangling.ArrowSensitive dangling when
                problematic dangling.Range trivia.WithKeyword
                || anyProblematic dangling.Range clauses
                ->
                true

            // A match-like construct could be problematically nested like this:
            //
            //     match () with
            //     | _ when
            //         p &&
            //         let x = f ()
            //         (let y = z
            //          match x with
            //          | 3 | _ -> y) -> ()
            //     | _ -> ()
            | _, Dangling.ArrowSensitive dangling when
                let rec ancestralTrailingArrow path =
                    match path with
                    | SyntaxNode.SynMatchClause _ :: _ -> shouldBeParenthesizedInContext path dangling

                    | SyntaxNode.SynExpr(SynExpr.Tuple _) :: path
                    | SyntaxNode.SynExpr(SynExpr.App _) :: path
                    | SyntaxNode.SynExpr(SynExpr.IfThenElse _) :: path
                    | SyntaxNode.SynExpr(SynExpr.IfThenElse _) :: path
                    | SyntaxNode.SynExpr(SynExpr.Sequential _) :: path
                    | SyntaxNode.SynExpr(SynExpr.YieldOrReturn _) :: path
                    | SyntaxNode.SynExpr(SynExpr.YieldOrReturnFrom _) :: path
                    | SyntaxNode.SynExpr(SynExpr.Set _) :: path
                    | SyntaxNode.SynExpr(SynExpr.DotSet _) :: path
                    | SyntaxNode.SynExpr(SynExpr.DotNamedIndexedPropertySet _) :: path
                    | SyntaxNode.SynExpr(SynExpr.DotIndexedSet _) :: path
                    | SyntaxNode.SynExpr(SynExpr.LongIdentSet _) :: path
                    | SyntaxNode.SynExpr(SynExpr.LetOrUse _) :: path
                    | SyntaxNode.SynExpr(SynExpr.Lambda _) :: path
                    | SyntaxNode.SynExpr(SynExpr.Match _) :: path
                    | SyntaxNode.SynExpr(SynExpr.MatchLambda _) :: path
                    | SyntaxNode.SynExpr(SynExpr.MatchBang _) :: path
                    | SyntaxNode.SynExpr(SynExpr.TryWith _) :: path
                    | SyntaxNode.SynExpr(SynExpr.TryFinally _) :: path
                    | SyntaxNode.SynExpr(SynExpr.Do _) :: path
                    | SyntaxNode.SynExpr(SynExpr.DoBang _) :: path -> ancestralTrailingArrow path

                    | _ -> false

                ancestralTrailingArrow outerPath
                ->
                true

            | SynExpr.Sequential _, Dangling.Problematic(SynExpr.Sequential _) -> true

            | SynExpr.Sequential(expr1 = SynExpr.Paren(expr = Is inner); expr2 = expr2), Dangling.Problematic _ when
                problematic inner.Range expr2.Range
                ->
                true

            // Keep parens if a name in the outer scope is rebound
            // in the inner scope and would shadow the outer binding
            // if the parens were removed, e.g.:
            //
            //     let x = 3
            //     (
            //         let x = 4
            //         printfn $"{x}"
            //     )
            //     x
            | SynExpr.Sequential(expr1 = SynExpr.Paren(expr = Is inner); expr2 = expr2), _ when innerBindingsWouldShadowOuter inner expr2 ->
                true

            | SynExpr.InterpolatedString _, SynExpr.Sequential _
            | SynExpr.InterpolatedString _, SynExpr.Tuple(isStruct = false) -> true

            | SynExpr.InterpolatedString(contents = contents), Dangling.Problematic _ ->
                contents
                |> List.exists (function
                    | SynInterpolatedStringPart.FillExpr(qualifiers = Some _) -> true
                    | _ -> false)

            // { (!x) with … }
            | SynExpr.Record(copyInfo = Some(SynExpr.Paren(expr = Is inner), _)),
              SynExpr.App(isInfix = false; funcExpr = FuncExpr.SymbolicOperator(StartsWith('!' | '~')))
            | SynExpr.AnonRecd(copyInfo = Some(SynExpr.Paren(expr = Is inner), _)),
              SynExpr.App(isInfix = false; funcExpr = FuncExpr.SymbolicOperator(StartsWith('!' | '~'))) -> false

            // { (+x) with … }
            // { (x + y) with … }
            // { (x |> f) with … }
            // { (printfn "…"; x) with … }
            | SynExpr.Record(copyInfo = Some(SynExpr.Paren(expr = Is inner), _)), (PrefixApp _ | InfixApp _ | Dangling.Problematic _)
            | SynExpr.AnonRecd(copyInfo = Some(SynExpr.Paren(expr = Is inner), _)), (PrefixApp _ | InfixApp _ | Dangling.Problematic _) ->
                true

            | SynExpr.Record(recordFields = recordFields), Dangling.Problematic _ ->
                let rec loop recordFields =
                    match recordFields with
                    | [] -> false
                    | SynExprRecordField(expr = Some(SynExpr.Paren(expr = Is inner)); blockSeparator = Some _) :: SynExprRecordField(
                        fieldName = SynLongIdent(id = id :: _), _) :: _ -> problematic inner.Range id.idRange
                    | _ :: recordFields -> loop recordFields

                loop recordFields

            | SynExpr.AnonRecd(recordFields = recordFields), Dangling.Problematic _ ->
                let rec loop recordFields =
                    match recordFields with
                    | [] -> false
                    | (_, Some _blockSeparator, SynExpr.Paren(expr = Is inner)) :: (SynLongIdent(id = id :: _), _, _) :: _ ->
                        problematic inner.Range id.idRange
                    | _ :: recordFields -> loop recordFields

                loop recordFields

            | SynExpr.Paren _, SynExpr.Typed _
            | SynExpr.Quote _, SynExpr.Typed _
            | SynExpr.While(doExpr = SynExpr.Paren(expr = Is inner)), SynExpr.Typed _
            | SynExpr.WhileBang(doExpr = SynExpr.Paren(expr = Is inner)), SynExpr.Typed _
            | SynExpr.For(doBody = Is inner), SynExpr.Typed _
            | SynExpr.ForEach(bodyExpr = Is inner), SynExpr.Typed _
            | SynExpr.Match _, SynExpr.Typed _
            | SynExpr.Do _, SynExpr.Typed _
            | SynExpr.LetOrUse(body = Is inner), SynExpr.Typed _
            | SynExpr.TryWith _, SynExpr.Typed _
            | SynExpr.TryFinally _, SynExpr.Typed _ -> false
            | _, SynExpr.Typed _ -> true

            | OuterBinaryExpr inner (outerPrecedence, side), InnerBinaryExpr innerPrecedence ->
                let ambiguous =
                    match compare outerPrecedence innerPrecedence with
                    | 0 ->
                        match side, Assoc.ofPrecedence innerPrecedence with
                        | Non, _
                        | _, Non
                        | Left, Right -> true
                        | Right, Right
                        | Left, Left -> false
                        | Right, Left ->
                            outerPrecedence <> innerPrecedence
                            || match outerPrecedence, innerPrecedence with
                               | _, MulDivMod(Div, _)
                               | _, MulDivMod(Mod, _)
                               | _, AddSub(Sub, _) -> true
                               | Relational _, Relational _ -> true
                               | _ -> false

                    | c -> c > 0

                ambiguous || dangling inner

            | OuterBinaryExpr inner (_, Right), (SynExpr.Sequential _ | SynExpr.LetOrUse(trivia = { InKeyword = None })) -> true
            | OuterBinaryExpr inner (_, Right), inner -> dangling inner

            // new T(expr)
            | SynExpr.New _, AtomicExprAfterType -> false
            | SynExpr.New _, _ -> true

            // { inherit T(expr); … }
            | SynExpr.Record(baseInfo = Some(_, SynExpr.Paren(expr = Is inner), _, _, _)), AtomicExprAfterType -> false
            | SynExpr.Record(baseInfo = Some(_, SynExpr.Paren(expr = Is inner), _, _, _)), _ -> true

            | _, SynExpr.Paren _
            | _, SynExpr.Quote _
            | _, SynExpr.Const _
            | _, SynExpr.Tuple(isStruct = true)
            | _, SynExpr.AnonRecd _
            | _, SynExpr.ArrayOrList _
            | _, SynExpr.Record _
            | _, SynExpr.ObjExpr _
            | _, SynExpr.ArrayOrListComputed _
            | _, SynExpr.ComputationExpr _
            | _, SynExpr.TypeApp _
            | _, SynExpr.Ident _
            | _, SynExpr.LongIdent _
            | _, SynExpr.DotGet _
            | _, SynExpr.DotLambda _
            | _, SynExpr.DotIndexedGet _
            | _, SynExpr.Null _
            | _, SynExpr.InterpolatedString _

            | SynExpr.Paren _, _
            | SynExpr.Quote _, _
            | SynExpr.Typed _, _
            | SynExpr.AnonRecd _, _
            | SynExpr.Record _, _
            | SynExpr.ObjExpr _, _
            | SynExpr.While _, _
            | SynExpr.WhileBang _, _
            | SynExpr.For _, _
            | SynExpr.ForEach _, _
            | SynExpr.Lambda _, _
            | SynExpr.MatchLambda _, _
            | SynExpr.Match _, _
            | SynExpr.MatchBang _, _
            | SynExpr.LetOrUse _, _
            | SynExpr.LetOrUseBang _, _
            | SynExpr.Sequential _, _
            | SynExpr.Do _, _
            | SynExpr.DoBang _, _
            | SynExpr.IfThenElse _, _
            | SynExpr.TryWith _, _
            | SynExpr.TryFinally _, _
            | SynExpr.ComputationExpr _, _
            | SynExpr.InterpolatedString _, _ -> false

            | _ -> true

        | _ -> true
