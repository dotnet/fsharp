// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.RemoveUnnecessaryParenthesesTests

open System
open FSharp.Compiler.Text
open FSharp.Editor.Tests.Helpers
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks
open Xunit
open CodeFixTestFramework

[<AutoOpen>]
module private Aux =
    /// Indicates that a code fix was expected but was not generated.
    exception MissingCodeFixException of message: string * exn: Xunit.Sdk.XunitException

    /// Indicates that a code fix was not expected but was generated nonetheless.
    exception UnexpectedCodeFixException of message: string * exn: Xunit.Sdk.XunitException

    /// Indicates that the offered code fix was incorrect.
    exception WrongCodeFixException of message: string * exn: exn

    let tryFix code (fix: 'T when 'T :> IFSharpCodeFixProvider and 'T :> CodeFixProvider) =
        task {
            let doc = RoslynTestHelpers.GetFsDocument code

            let! diagnostics =
                FSharpDocumentDiagnosticAnalyzer.GetDiagnostics(doc, DiagnosticsType.Syntax)
                |> CancellableTask.startWithoutCancellation

            let parenDiagnostics =
                diagnostics
                |> Seq.filter (fun diagnostic -> fix.FixableDiagnosticIds.Contains diagnostic.Id)
                |> Seq.truncate 1
                |> Seq.toImmutableArray

            let! codeFix =
                match Seq.tryHead parenDiagnostics with
                | None -> System.Threading.Tasks.Task.FromResult ValueNone
                | Some diagnostic ->
                    let ctx = CodeFixContext(doc, diagnostic.Location.SourceSpan, parenDiagnostics, mockAction, System.Threading.CancellationToken.None)
                    fix.GetCodeFixIfAppliesAsync ctx |> CancellableTask.startWithoutCancellation

            return codeFix |> ValueOption.map (fun codeFix ->
                let sourceText = SourceText.From code
                let fixedCode = string (sourceText.WithChanges codeFix.Changes)
                { Message = codeFix.Message; FixedCode = fixedCode })
        }

    let shouldEqual expected actual =
        let split (s: string) = s.Split([|Environment.NewLine|], StringSplitOptions.RemoveEmptyEntries)
        let expected = split expected
        let actual = split actual
        try
            Assert.All(Array.zip expected actual, fun (expected, actual) -> Assert.Equal(expected, actual))
        with
        | :? Xunit.Sdk.AllException as all when all.Failures.Count = 1 -> raise (WrongCodeFixException ("The generated code fix does not match the expected fix.", all.Failures[0]))
        | e -> raise (WrongCodeFixException ("The generated code fix does not match the expected fix.", e))

    [<AutoOpen>]
    module TopLevel =
        let codeFixProvider = FSharpRemoveUnnecessaryParenthesesCodeFixProvider()

        /// <summary>
        /// Expects no code fix to be applied to the given code.
        /// </summary>
        /// <exception cref="T:FSharp.Editor.Tests.CodeFixes.RemoveUnnecessaryParenthesesTests.Aux.UnexpectedCodeFixException">
        /// Thrown if a code fix is applied.
        /// </exception>
        let expectNoFix code =
            task {
                match! codeFixProvider |> tryFix code with
                | ValueNone -> ()
                | ValueSome actual ->
                    let expected = string { Message = "Remove unnecessary parentheses"; FixedCode = code }
                    let e = Assert.ThrowsAny(fun() -> shouldEqual expected (string actual))
                    raise (UnexpectedCodeFixException("Did not expect a code fix but got one anyway.", e))
            }

        /// <summary>
        /// Expects the given code to be fixed (or not, if <paramref name="code"/> = <paramref name="fixedCode"/>) as specified.
        /// </summary>
        /// <exception cref="T:FSharp.Editor.Tests.CodeFixes.RemoveUnnecessaryParenthesesTests.Aux.MissingCodeFixException">
        /// Thrown if a code fix is not applied when expected.
        /// </exception>
        /// <exception cref="T:FSharp.Editor.Tests.CodeFixes.RemoveUnnecessaryParenthesesTests.Aux.UnexpectedCodeFixException">
        /// Thrown if a code fix is applied when not expected.
        /// </exception>
        /// <exception cref="T:FSharp.Editor.Tests.CodeFixes.RemoveUnnecessaryParenthesesTests.Aux.WrongCodeFixException">
        /// Thrown if the applied code fix is wrong.
        /// </exception>
        let expectFix code fixedCode =
            if code = fixedCode then
                expectNoFix code
            else
                task {
                    let expected = string { Message = "Remove unnecessary parentheses"; FixedCode = fixedCode }
                    let! actual = codeFixProvider |> tryFix code
                    let actual =
                        actual
                        |> ValueOption.map string
                        |> ValueOption.defaultWith (fun () ->
                            let e = Assert.ThrowsAny(fun() -> shouldEqual fixedCode code)
                            raise (MissingCodeFixException("Expected a code fix but did not get one.", e)))

                    do shouldEqual expected actual
                }

    [<Sealed>]
    type MemberDataBuilder private () =
        static member val Instance = MemberDataBuilder()
        member _.Combine(xs, ys) = Seq.append xs ys
        member _.Delay f = f ()
        member _.Zero() = Seq.empty
        member _.Yield(x, y) = Seq.singleton [|box x; box y|]
        member _.YieldFrom pairs = seq { for x, y in pairs -> [|box x; box y|] }
        member _.For(xs, f) = xs |> Seq.collect f
        member _.Run objArrays = objArrays

    /// Builds an obj array seq for use with the [<MemberData(…)>] attribute.
    let memberData = MemberDataBuilder.Instance

module Expressions =
    /// let f x y z = expr
    let expectFix expr expected =
        let code = $"
let _ =
    %s{expr}
"

        let expected = $"
let _ =
    %s{expected}
"

        expectFix code expected

    [<Fact>]
    let ``Beginning of file: (printfn "Hello, world")`` () = TopLevel.expectFix "(printfn \"Hello, world\")" "printfn \"Hello, world\""

    [<Fact>]
    let ``End of file: let x = (1)`` () = TopLevel.expectFix "let x = (1)" "let x = 1"

    let exprs =
        memberData {
            // Paren
            "()", "()"
            "(())", "()"
            "((3))", "(3)"

            // Quote
            "<@ (3) @>", "<@ 3 @>"
            "<@@ (3) @@>", "<@@ 3 @@>"

            // Typed
            "(1) : int", "1 : int"

            // Tuple
            "(1), 1", "1, 1"
            "1, (1)", "1, 1"
            "struct ((1), 1)", "struct (1, 1)"
            "struct (1, (1))", "struct (1, 1)"

            // AnonymousRecord
            "{| A = (1) |}", "{| A = 1 |}"
            "{| A = (1); B = 2 |}", "{| A = 1; B = 2 |}"
            "{| A =(1) |}", "{| A = 1 |}"
            "{| A=(1) |}", "{| A=1 |}"

            // ArrayOrList
            "[(1)]", "[1]"
            "[(1); 2]", "[1; 2]"
            "[1; (2)]", "[1; 2]"
            "[|(1)|]", "[|1|]"
            "[|(1); 2|]", "[|1; 2|]"
            "[|1; (2)|]", "[|1; 2|]"

            // Record
            "{ A = (1) }", "{ A = 1 }"
            "{ A =(1) }", "{ A = 1 }"
            "{ A=(1) }", "{ A=1 }"
            "{A=(1)}", "{A=1}"

            // New
            "new exn(null)", "new exn null"
            "new exn (null)", "new exn null"

            // ObjExpr
            "{ new System.IDisposable with member _.Dispose () = (ignore 3) }", "{ new System.IDisposable with member _.Dispose () = ignore 3 }"

            // While
            "while (true) do ()", "while true do ()"
            "while true do (ignore 3)", "while true do ignore 3"

            // For
            "for x = (0) to 1 do ()", "for x = 0 to 1 do ()"
            "for x =(0) to 1 do ()", "for x = 0 to 1 do ()"
            "for x=(0) to 1 do ()", "for x=0 to 1 do ()"
            "for x = 0 to (1) do ()", "for x = 0 to 1 do ()"
            "for x = 0 to 1 do (ignore 3)", "for x = 0 to 1 do ignore 3"

            // ForEach
            "for (x) in [] do ()", "for x in [] do ()"
            "for x in ([]) do ()", "for x in [] do ()"
            "for x in [] do (ignore 3)", "for x in [] do ignore 3"

            // ArrayOrListComputed
            // IndexRange
            "[(1)..10]", "[1..10]"
            "[1..(10)]", "[1..10]"
            "[|(1)..10|]", "[|1..10|]"
            "[|1..(10)|]", "[|1..10|]"

            // IndexFromEnd
            "[0][..^(0)]", "[0][..^0]"

            // ComputationExpression
            "seq { (3) }", "seq { 3 }"

            // Lambda
            "fun _ -> (3)", "fun _ -> 3"

            // MatchLambda
            "function _ -> (3)", "function _ -> 3"
            "function _ when (true) -> 3 | _ -> 3", "function _ when true -> 3 | _ -> 3"
            "function 1 -> (function _ -> 3) | _ -> function _ -> 3", "function 1 -> (function _ -> 3) | _ -> function _ -> 3"
            "function 1 -> (function _ -> 3) | _ -> (function _ -> 3)", "function 1 -> (function _ -> 3) | _ -> function _ -> 3"

            // Match
            "match (3) with _ -> 3", "match 3 with _ -> 3"
            "match 3 with _ -> (3)", "match 3 with _ -> 3"
            "match 3 with _ when (true) -> 3 | _ -> 3", "match 3 with _ when true -> 3 | _ -> 3"
            "match 3 with 1 -> (match 3 with _ -> 3) | _ -> match 3 with _ -> 3", "match 3 with 1 -> (match 3 with _ -> 3) | _ -> match 3 with _ -> 3"
            "match 3 with 1 -> (match 3 with _ -> 3) | _ -> (match 3 with _ -> 3)", "match 3 with 1 -> (match 3 with _ -> 3) | _ -> match 3 with _ -> 3"

            // Do
            "do (ignore 3)", "do ignore 3"

            // Assert
            "assert(true)", "assert true"
            "assert (true)", "assert true"
            "assert (not false)", "assert not false"
            "assert (2 + 2 = 5)", "assert (2 + 2 = 5)"

            // App
            "id (3)", "id 3"
            "id(3)", "id 3"
            "id id (3)", "id id 3"
            "id<int>(3)", "id<int> 3"
            "nameof(nameof)", "nameof nameof"

            // LetOrUse
            "let x = 3 in let y = (4) in x + y", "let x = 3 in let y = 4 in x + y"
            "let x = 3 in let y = 4 in (x + y)", "let x = 3 in let y = 4 in x + y"
            "let x = 3 in let y =(4) in x + y", "let x = 3 in let y = 4 in x + y"
            "let x = 3 in let y=(4) in x + y", "let x = 3 in let y=4 in x + y"

            // TryWith
            "try (raise null) with _ -> reraise ()", "try raise null with _ -> reraise ()"
            "try raise null with (_) -> reraise ()", "try raise null with _ -> reraise ()"
            "try raise null with _ -> (reraise ())", "try raise null with _ -> reraise ()"
            "try raise null with :? exn -> (try raise null with _ -> reraise ()) | _ -> reraise ()", "try raise null with :? exn -> (try raise null with _ -> reraise ()) | _ -> reraise ()"
            "try (try raise null with _ -> null) with _ -> null", "try (try raise null with _ -> null) with _ -> null"
            "try (try raise null with _ -> null ) with _ -> null", "try (try raise null with _ -> null ) with _ -> null"

            // TryFinally
            "try (raise null) finally 3", "try raise null finally 3"
            "try raise null finally (3)", "try raise null finally 3"
            "try (try raise null with _ -> null) finally null", "try (try raise null with _ -> null) finally null"

            // Lazy
            "lazy(3)", "lazy 3"
            "lazy (3)", "lazy 3"
            "lazy (id 3)", "lazy id 3"

            // Sequential
            "let x = 3; (5) in x", "let x = 3; 5 in x"

            // IfThenElse
            "if (3 = 3) then 3 else 3", "if 3 = 3 then 3 else 3"
            "if 3 = 3 then (3) else 3", "if 3 = 3 then 3 else 3"
            "if 3 = 3 then 3 else (3)", "if 3 = 3 then 3 else 3"
            "if (if true then true else true) then 3 else 3", "if (if true then true else true) then 3 else 3"
            "if (id <| if true then true else true) then 3 else 3", "if (id <| if true then true else true) then 3 else 3"
            "
                if (if true then true else true)
                then 3
                else 3
            ",
            "
                if if true then true else true
                then 3
                else 3
            "

            // LongIdent
            "(|Failure|_|) null", "(|Failure|_|) null"

            // LongIdentSet
            "let r = ref 3 in r.Value <- (3)", "let r = ref 3 in r.Value <- 3"

            // DotGet
            "([]).Length", "[].Length"

            // DotLambda
            // DotSet
            "(ref 3).Value <- (3)", "(ref 3).Value <- 3"

            // Set
            "let mutable x = 3 in (x) <- 3", "let mutable x = 3 in x <- 3"
            "let mutable x = 3 in x <- (3)", "let mutable x = 3 in x <- 3"

            // DotIndexedGet
            "[(0)][0]", "[0][0]"
            "[0][(0)]", "[0][0]"

            // DotIndexedSet
            "[|(0)|][0] <- 0", "[|0|][0] <- 0"
            "[|0|][(0)] <- 0", "[|0|][0] <- 0"
            "[|0|][0] <- (0)", "[|0|][0] <- 0"

            // NamedIndexedPropertySet
            // DotNamedIndexedPropertySet
            "[|0|].Item(0) <- 0", "[|0|].Item 0 <- 0"
            "[|0|].Item (0) <- 0", "[|0|].Item 0 <- 0"
            "[|0|].Item 0 <- (0)", "[|0|].Item 0 <- 0"

            // TypeTest
            "(box 3) :? int", "box 3 :? int"

            // Upcast
            "(3) :> obj", "3 :> obj"

            // Downcast
            "(box 3) :?> int", "box 3 :?> int"

            // InferredUpcast
            "let o : obj = upcast (3) in o", "let o : obj = upcast 3 in o"

            // InferredDowncast
            "let o : int = downcast (null) in o",  "let o : int = downcast null in o"

            // AddressOf
            "let mutable x = 0 in System.Int32.TryParse (null, &(x))", "let mutable x = 0 in System.Int32.TryParse (null, &x)"

            // TraitCall
            "let inline f x = (^a : (static member Parse : string -> ^a) (x))", "let inline f x = (^a : (static member Parse : string -> ^a) x)"

            // JoinIn
            "
            query {
                for x, y in [10, 11] do
                where (x = y)
                join (x', y') in [12, 13] on (x = x')
                select (x + x', y + y')
            }
            ",
            "
            query {
                for x, y in [10, 11] do
                where (x = y)
                join (x', y') in [12, 13] on (x = x')
                select (x + x', y + y')
            }
            "
            "
            query {
                for x, y in [10, 11] do
                where (x = y)
                join (x') in [12] on (x = x')
                select (x + x', y)
            }
            ",
            "
            query {
                for x, y in [10, 11] do
                where (x = y)
                join x' in [12] on (x = x')
                select (x + x', y)
            }
            "
            "
            query {
                for x, y in [10, 11] do
                where (x = y)
                join x' in [12] on (x = x')
                select (x')
            }
            ",
            "
            query {
                for x, y in [10, 11] do
                where (x = y)
                join x' in [12] on (x = x')
                select x'
            }
            "

            // YieldOrReturn
            "seq { yield (3) }", "seq { yield 3 }"
            "async { return (3) }", "async { return 3 }"

            // YieldOrReturnFrom
            "seq { yield! ([3]) }", "seq { yield! [3] }"
            "async { return! (async { return 3 }) }", "async { return! async { return 3 } }"

            // LetOrUseBang
            "async { let! (x) = async { return 3 } in return () }", "async { let! x = async { return 3 } in return () }"
            "async { let! (x : int) = async { return 3 } in return () }", "async { let! (x : int) = async { return 3 } in return () }"
            "async { let! (Lazy x) = async { return lazy 3 } in return () }", "async { let! Lazy x = async { return lazy 3 } in return () }"
            "async { use! x = async { return (Unchecked.defaultof<System.IDisposable>) } in return () }", "async { use! x = async { return Unchecked.defaultof<System.IDisposable> } in return () }"

            // MatchBang
            "async { match! (async { return 3 }) with _ -> return () }", "async { match! async { return 3 } with _ -> return () }"
            "async { match! async { return 3 } with _ -> (return ()) }", "async { match! async { return 3 } with _ -> return () }"
            "async { match! async { return 3 } with _ when (true) -> return () }", "async { match! async { return 3 } with _ when true -> return () }"
            "async { match! async { return 3 } with 4 -> return (match 3 with _ -> ()) | _ -> return () }", "async { match! async { return 3 } with 4 -> return (match 3 with _ -> ()) | _ -> return () }"
            "async { match! async { return 3 } with 4 -> return (let x = 3 in match x with _ -> ()) | _ -> return () }", "async { match! async { return 3 } with 4 -> return (let x = 3 in match x with _ -> ()) | _ -> return () }"
            "async { match! async { return 3 } with _ when (match 3 with _ -> true) -> return () }", "async { match! async { return 3 } with _ when (match 3 with _ -> true) -> return () }"

            // DoBang
            "async { do! (async { return () }) }", "async { do! async { return () } }"

            // WhileBang
            "async { while! (async { return true }) do () }", "async { while! async { return true } do () }"
            "async { while! async { return true } do (ignore false) }", "async { while! async { return true } do ignore false }"

            // Fixed
            "use f = fixed ([||]) in ()", "use f = fixed [||] in ()"

            // InterpolatedString
            "$\"{(3)}\"", "$\"{3}\""
            "$\"{(-3)}\"", "$\"{-3}\""
            "$\"{-(3)}\"", "$\"{-3}\""
            "$\"{(id 3)}\"", "$\"{id 3}\""
            "$\"{(x)}\"", "$\"{x}\""
        }

    [<Theory; MemberData(nameof exprs)>]
    let ``Basic expressions`` expr expected = expectFix expr expected

    module FunctionApplications =
        let functionApplications =
            memberData {
                // Paren
                "id ()", "id ()"
                "id (())", "id ()"
                "id ((x))", "id (x)"

                // Quote
                "id (<@ x @>)", "id <@ x @>"
                "id (<@@ x @@>)", "id <@@ x @@>"

                // Const
                "id (3)", "id 3"
                "id(3)", "id 3"
                "id<int>(3)", "id<int> 3"
                "id (-3)", "id -3"
                "id (-3s)", "id -3s"
                "id (-3y)", "id -3y"
                "id (-3L)", "id -3L"
                "id -(3)", "id -3"
                "id (+3uy)", "id +3uy"
                "id (+3us)", "id +3us"
                "id (+3u)", "id +3u"
                "id (+3UL)", "id +3UL"
                "id (-3<m>)", "id -3<m>"
                "id (-(-3))", "id -(-3)"
                "id -(-3)", "id -(-3)"
                "id -(+3)", "id -(+3)"
                "id -(0b1)", "id -0b1"
                "id -(0x1)", "id -0x1"
                "id -(0o1)", "id -0o1"
                "id -(1e4)", "id -1e4"
                "id -(1e-4)", "id -1e-4"
                "id -(-(-x))", "id -(- -x)"
                "id -(-(-3))", "id -(- -3)"
                "id -(- -3)", "id -(- -3)"
                "-(x)", "-x"
                "-(3)", "-3"
                "-(-x)", "- -x"
                "-(-3)", "- -3"
                "-(- -x)", "- - -x"
                "-(- -3)", "- - -3"

                // Typed
                "id (x : int)", "id (x : int)"

                // Tuple
                "id (x, y)", "id (x, y)"
                "id (struct (x, y))", "id struct (x, y)"
                "id<struct (_ * _)> (x, y)", "id<struct (_ * _)> (x, y)"

                // AnonRecd
                "id ({||})", "id {||}"

                // ArrayOrList
                "id ([])", "id []"
                "id ([||])", "id [||]"

                // Record
                "id ({ A = x })", "id { A = x }"

                // New
                "id (new obj())", "id (new obj())"

                // ObjExpr
                "id ({ new System.IDisposable with member _.Dispose() = () })", "id { new System.IDisposable with member _.Dispose() = () }"

                // While
                "id (while true do ())", "id (while true do ())"

                // ArrayOrListComputed
                "id ([x..y])", "id [x..y]"
                "id ([|x..y|])", "id [|x..y|]"

                // ComputationExpr
                "id (seq { x })", "id (seq { x })"
                "id ({x..y})", "id {x..y}"
                "(async) { return x }", "async { return x }"
                "(id async) { return x }", "id async { return x }"

                // Lambda
                "id (fun x -> x)", "id (fun x -> x)"
                "x |> (fun x -> x)", "x |> fun x -> x"
                "id <| (fun x -> x)", "id <| fun x -> x"
                "id <| (fun x -> x) |> id", "id <| fun x -> x |> id"

                // MatchLambda
                "id (function x when true -> x | y -> y)", "id (function x when true -> x | y -> y)"

                // Match
                "id (match x with y -> y)", "id (match x with y -> y)"

                // Do
                "id (do ())", "id (do ())"

                // Assert
                "id (assert true)", "id (assert true)"

                // App
                "id (id x)", "id (id x)"
                "id (-x)", "id -x"
                "id (- x)", "id (- x)"
                "(id id) id", "id id id"
                "id(id)id", "id id id"
                "id(id<int>)id", "id id<int> id"
                "id (id id) id", "id (id id) id" // While it would be valid in this case to remove the parens, it is not in general.
                "id ((<|) ((+) x)) y", "id ((<|) ((+) x)) y"
                "
                let f x y = 0
                f ((+) x y) z
                ",
                "
                let f x y = 0
                f ((+) x y) z
                "

                // TypeApp
                "id (id<int>)", "id id<int>"

                // LetOrUse
                "id (let x = 1 in x)", "id (let x = 1 in x)"

                // TryWith
                "id (try raise null with _ -> null)", "id (try raise null with _ -> null)"

                // TryFinally
                "id (try raise null finally null)", "id (try raise null finally null)"

                // Lazy
                "id (lazy x)", "id (lazy x)"

                // Sequential
                "id (let x = 1; () in x)", "id (let x = 1; () in x)"

                // IfThenElse
                "id (if x then y else z)", "id (if x then y else z)"

                // Ident
                "id (x)", "id x"
                "id(x)", "id x"
                "(~-) (x)", "(~-) x"
                "((+) x y) + z", "(+) x y + z"
                "x + ((+) y z)", "x + (+) y z"
                "x + (-y + z)", "x + -y + z"
                "x + (-y)", "x + -y"
                "x + (- y)", "x + - y"
                "[id][x](-y) * z", "[id][x] -y * z"
                "(x.Equals y).Equals z", "(x.Equals y).Equals z"
                "(x.Equals (y)).Equals z", "(x.Equals y).Equals z"
                "(x.Equals(y)).Equals z", "(x.Equals y).Equals z"
                "x.Equals(y).Equals z", "x.Equals(y).Equals z"
                "obj().Equals(obj())", "obj().Equals(obj())"

                // LongIdent
                "id (id.ToString)", "id id.ToString"

                // LongIdentSet
                // DotGet
                // DotLambda
                "[{| A = x |}] |> List.map (_.A)", "[{| A = x |}] |> List.map _.A"

                // DotSet
                "id ((ref x).Value <- y)", "id ((ref x).Value <- y)"

                // Set
                "let mutable x = y in id (x <- z)", "let mutable x = y in id (x <- z)"

                // DotIndexedGet
                "id ([x].[y])", "id [x].[y]"

                // DotIndexedSet
                "id ([|x|].[y] <- z)", "id ([|x|].[y] <- z)"

                // NamedIndexedPropertySet
                // DotNamedIndexedPropertySet
                // TypeTest
                "id (x :? int)", "id (x :? int)"

                // Upcast
                "id (x :> int)", "id (x :> int)"

                // Downcast
                "id (x :?> int)", "id (x :?> int)"

                // InferredUpcast
                "id (upcast x)", "id (upcast x)"

                // InferredDowncast
                "id (downcast x)", "id (downcast x)"

                // Null
                "id (null)", "id null"

                // AddressOf
                "
                let f (_: byref<int>) = ()
                let mutable x = 0
                f (&x)
                ",
                "
                let f (_: byref<int>) = ()
                let mutable x = 0
                f &x
                "

                // TraitCall
                "let inline f x = id (^a : (static member Parse : string -> ^a) x)", "let inline f x = id (^a : (static member Parse : string -> ^a) x)"

                // InterpolatedString
                """ id ($"{x}") """, """ id $"{x}" """
            }

        [<Theory; MemberData(nameof functionApplications)>]
        let ``Regular function applications`` expr expected = expectFix expr expected

        let moreComplexApps =
            memberData {
                "
type T() = member _.M y = [|y|]
let x = T()
let y = 3
let z = 0
x.M(y)[z]
",
                "
type T() = member _.M y = [|y|]
let x = T()
let y = 3
let z = 0
x.M(y)[z]
"

                "
type T() = member _.M y = [|y|]
let x = T()
let y = 3
let z = 0
x.M(y).[z]
",
                "
type T() = member _.M y = [|y|]
let x = T()
let y = 3
let z = 0
x.M(y).[z]
"

                "
let f x = x =
            (let a = 1
             a)
",
                "
let f x = x =
            (let a = 1
             a)
"
                "
type Builder () =
    member _.Return x = x
    member _.Run x = x
let builder = Builder ()
let (+) _ _ = builder
let _ = (2 + 2) { return 5 }
",
                "
type Builder () =
    member _.Return x = x
    member _.Run x = x
let builder = Builder ()
let (+) _ _ = builder
let _ = (2 + 2) { return 5 }
"
            }

        [<Theory; MemberData(nameof moreComplexApps)>]
        let ``More complex function/method applications`` expr expected = TopLevel.expectFix expr expected

        module InfixOperators =
            open System.Text.RegularExpressions

            /// x λ (y ρ z)
            ///
            /// or
            ///
            /// (x λ y) ρ z
            type ParenthesizedInfixOperatorAppPair =
                /// x λ (y ρ z)
                | OuterLeft of l: string * r: string

                /// (x λ y) ρ z
                | OuterRight of l: string * r: string

                override this.ToString() =
                    match this with
                    | OuterLeft (l, r) -> $"x {l} (y {r} z)"
                    | OuterRight (l, r) -> $"(x {l} y) {r} z"

            module ParenthesizedInfixOperatorAppPair =
                /// Reduces the operator strings to simpler, more easily identifiable forms.
                let simplify =
                    let ignoredLeadingChars = [|'.'; '?'|]
                    let simplify (s: string) =
                        let s = s.TrimStart ignoredLeadingChars
                        match s[0], s with
                        | '*', _ when s.Length > 1 && s[1] = '*' -> "**op"
                        | ':', _ | _, ("$" | "||" | "or" | "&" | "&&") -> s
                        | '!', _ -> "!=op"
                        | c, _ -> $"{c}op"

                    function
                    | OuterLeft (l, r) -> OuterLeft (simplify l, simplify r)
                    | OuterRight (l, r) -> OuterRight (simplify l, simplify r)

                /// Indicates that the pairing is syntactically invalid
                /// (for unoverloadable operators like :?, :>, :?>)
                /// and that we therefore need not test it.
                let invalidPairing = None

                let unfixable pair =
                    let expr = string pair
                    Some (expr, expr)

                let fixable pair =
                    let expr = string pair
                    let fix =
                        match pair with
                        | OuterLeft (l, r)
                        | OuterRight (l, r) -> $"x {l} y {r} z"

                    Some (expr, fix)

                let expectation pair =
                    match simplify pair with
                    | OuterLeft ((":?" | ":>" | ":?>"), _) -> invalidPairing
                    | OuterLeft (_, "**op") -> fixable pair
                    | OuterLeft ("**op", _) -> unfixable pair
                    | OuterLeft (_, ("%op" | "/op" | "*op")) -> fixable pair
                    | OuterLeft (("%op" | "/op" | "*op"), _) -> unfixable pair
                    | OuterLeft (_, ("-op" | "+op")) -> fixable pair
                    | OuterLeft (("-op" | "+op"), _) -> unfixable pair
                    | OuterLeft (_, ":?") -> fixable pair
                    | OuterLeft (_, "::") -> fixable pair
                    | OuterLeft ("::", _) -> unfixable pair
                    | OuterLeft (_, ("^op" | "@op")) -> fixable pair
                    | OuterLeft (("^op" | "@op"), _) -> unfixable pair
                    | OuterLeft (_, ("=op" | "|op" | "&op" | "$" | ">op" | "<op" | "!=op")) -> fixable pair
                    | OuterLeft (("=op" | "|op" | "&op" | "$" | ">op" | "<op" | "!=op"), _) -> unfixable pair
                    | OuterLeft (_, (":>" | ":?>")) -> fixable pair
                    | OuterLeft (_, ("&" | "&&")) -> fixable pair
                    | OuterLeft (("&" | "&&"), _) -> unfixable pair
                    | OuterLeft (_, ("||" | "or")) -> fixable pair
                    | OuterLeft (("||" | "or"), _) -> unfixable pair
                    | OuterLeft (":=", ":=") -> fixable pair

                    | OuterRight ((":?" | ":>" | ":?>"), _) -> invalidPairing
                    | OuterRight (_, "**op") -> unfixable pair
                    | OuterRight ("**op", _) -> fixable pair
                    | OuterRight (("%op" | "/op" | "*op"), _) -> fixable pair
                    | OuterRight (_, ("%op" | "/op" | "*op")) -> unfixable pair
                    | OuterRight (("-op" | "+op"), _) -> fixable pair
                    | OuterRight (_, ("-op" | "+op")) -> unfixable pair
                    | OuterRight (_, ":?") -> unfixable pair
                    | OuterRight ("::", _) -> fixable pair
                    | OuterRight (_, "::") -> unfixable pair
                    | OuterRight (("^op" | "@op"), _) -> fixable pair
                    | OuterRight (_, ("^op" | "@op")) -> unfixable pair
                    | OuterRight (("=op" | "|op" | "&op" | "$" | ">op" | "<op" | "!=op"), _) -> fixable pair
                    | OuterRight (_, ("=op" | "|op" | "&op" | "$" | ">op" | "<op" | "!=op")) -> unfixable pair
                    | OuterRight (_, (":>" | ":?>")) -> unfixable pair
                    | OuterRight (("&" | "&&"), _) -> fixable pair
                    | OuterRight (_, ("&" | "&&")) -> unfixable pair
                    | OuterRight (("||" | "or"), _) -> fixable pair
                    | OuterRight (_, ("||" | "or")) -> unfixable pair
                    | OuterRight (":=", ":=") -> fixable pair

                    | _ -> unfixable pair

            let operators =
                [
                    "**"
                    "*"; "/"; "%"
                    "-"; "+"
                    ":?"
                    "::"
                    "^^^"; "@"
                    "<"; ">"; "="; "!="; "|||"; "&&&"; "$"; "|>"; "<|"
                    ":>"; ":?>"
                    "&&"; "&"
                    "||"; "or"
                    ":="
                ]

            let pairings =
                operators
                |> Seq.allPairs operators
                |> Seq.allPairs [OuterLeft; OuterRight]
                |> Seq.choose (fun (pair, (l, r)) -> ParenthesizedInfixOperatorAppPair.expectation (pair (l, r)))

            let affixableOpPattern = @" (\*\*|\*|/+|%+|\++|-+|@+|^+|!=|<+|>+|&{3,}|\|{3,}|=+|\|>|<\|) "
            let prefixOpsInExprWith prefix expr = Regex.Replace(expr, affixableOpPattern, $" %s{prefix}$1 ")
            let suffixOpsInExprWith suffix expr = Regex.Replace(expr, affixableOpPattern, $" $1%s{suffix} ")
            let leadingDots = "..."
            let leadingQuestionMarks = "???"
            let trailingChars = "+^=*/"

            let infixOperators =
                memberData {
                    yield! pairings
                }

            let infixOperatorsWithLeadingDots =
                memberData {
                    for expr, expected in pairings ->
                        prefixOpsInExprWith leadingDots expr, prefixOpsInExprWith leadingDots expected
                }

            let infixOperatorsWithLeadingQuestionMarks =
                memberData {
                    for expr, expected in pairings ->
                        prefixOpsInExprWith leadingQuestionMarks expr, prefixOpsInExprWith leadingQuestionMarks expected
                }

            let infixOperatorsWithTrailingChars =
                memberData {
                    for expr, expected in pairings ->
                        suffixOpsInExprWith trailingChars expr, suffixOpsInExprWith trailingChars expected
                }

            [<Theory; MemberData(nameof infixOperators)>]
            let ``Infix operators`` expr expected = expectFix expr expected
                
            [<Theory; MemberData(nameof infixOperatorsWithLeadingDots)>]
            let ``Infix operators with leading dots`` expr expected = expectFix expr expected
                
            [<Theory; MemberData(nameof infixOperatorsWithLeadingQuestionMarks)>]
            let ``Infix operators with leading question marks`` expr expected = expectFix expr expected
                
            [<Theory; MemberData(nameof infixOperatorsWithTrailingChars)>]
            let ``Infix operators with trailing characters`` expr expected = expectFix expr expected
                
module Patterns =
    /// match … with pat -> …
    let expectFix pat expected =
        let code = $"
let (|A|_|) _ = None
let (|B|_|) _ = None
let (|C|_|) _ = None
let (|D|_|) _ = None
let (|P|_|) _ _ = None
match Unchecked.defaultof<_> with
| %s{pat} -> ()
| _ -> ()
"

        let expected = $"
let (|A|_|) _ = None
let (|B|_|) _ = None
let (|C|_|) _ = None
let (|D|_|) _ = None
let (|P|_|) _ _ = None
match Unchecked.defaultof<_> with
| %s{expected} -> ()
| _ -> ()
"
        expectFix code expected

    /// match … with pat -> …
    let expectNoFix pat =
        expectNoFix $"
let (|A|_|) _ = None
let (|B|_|) _ = None
let (|C|_|) _ = None
let (|D|_|) _ = None
let (|P|_|) _ _ = None
match Unchecked.defaultof<_> with
| %s{pat} -> ()
| _ -> ()
"

    let nestedPatterns =
        memberData {
            // Typed
            "((3) : int)", "(3 : int)"

            // Attrib
            // Or
            "(A) | B", "A | B"
            "A | (B)", "A | B"

            // ListCons
            "(3) :: []", "3 :: []"
            "3 :: ([])", "3 :: []"

            // Ands
            "(A) & B", "A & B"
            "A & (B)", "A & B"
            "A & (B) & C", "A & B & C"
            "A & B & (C)", "A & B & C"

            // As
            "_ as (A)", "_ as A"

            // LongIdent
            "Lazy (3)", "Lazy 3"
            "Some (3)", "Some 3"
            "Some(3)", "Some 3"

            // Tuple
            "(1), 2", "1, 2"
            "1, (2)", "1, 2"

            // Paren
            "()", "()"
            "(())", "()"
            "((3))", "(3)"

            // ArrayOrList
            "[(3)]", "[3]"
            "[(3); 4]", "[3; 4]"
            "[3; (4)]", "[3; 4]"
            "[|(3)|]", "[|3|]"
            "[|(3); 4|]", "[|3; 4|]"
            "[|3; (4)|]", "[|3; 4|]"

            // Record
            "{ A = (3) }", "{ A = 3 }"
            "{ A =(3) }", "{ A = 3 }"
            "{ A=(3) }", "{ A=3 }"
            "{A=(3)}", "{A=3}"

            // QuoteExpr
            "P <@ (3) @>", "P <@ 3 @>"
        }

    // This is mainly to verify that all pattern kinds are traversed.
    // It is _not_ an exhaustive test of all possible pattern nestings.
    [<Theory; MemberData(nameof nestedPatterns)>]
    let ``Nested patterns`` original expected = expectFix original expected

    let patternsInExprs =
        memberData {
            // ForEach
            "for (x) in [] do ()", "for x in [] do ()"
            "for (Lazy x) in [] do ()", "for Lazy x in [] do ()"

            // Lambda
            "fun () -> ()", "fun () -> ()"
            "fun (_) -> ()", "fun _ -> ()"
            "fun (x) -> x", "fun x -> x"
            "fun (x: int) -> x", "fun (x: int) -> x"
            "fun x (y) -> x", "fun x y -> x"
            "fun (Lazy x) -> x", "fun (Lazy x) -> x"
            "fun (x, y) -> x, y", "fun (x, y) -> x, y"
            "fun (struct (x, y)) -> x, y", "fun struct (x, y) -> x, y"

            // MatchLambda
            "function () -> ()", "function () -> ()"
            "function (_) -> ()", "function _ -> ()"
            "function (x) -> x", "function x -> x"
            "function (x: int) -> x", "function (x: int) -> x"
            "function (Lazy x) -> x", "function Lazy x -> x"
            "function (1 | 2) -> () | _ -> ()", "function 1 | 2 -> () | _ -> ()"
            "function (x & y) -> x, y", "function x & y -> x, y"
            "function (x as y) -> x, y", "function x as y -> x, y"
            "function (x :: xs) -> ()", "function x :: xs -> ()"
            "function (x, y) -> x, y", "function x, y -> x, y"
            "function (struct (x, y)) -> x, y", "function struct (x, y) -> x, y"
            "function x when (true) -> x | y -> y", "function x when true -> x | y -> y"
            "function x when (match x with _ -> true) -> x | y -> y", "function x when (match x with _ -> true) -> x | y -> y"
            "function x when (let x = 3 in match x with _ -> true) -> x | y -> y", "function x when (let x = 3 in match x with _ -> true) -> x | y -> y"

            // Match
            "match x with () -> ()", "match x with () -> ()"
            "match x with (_) -> ()", "match x with _ -> ()"
            "match x with (x) -> x", "match x with x -> x"
            "match x with (x: int) -> x", "match x with (x: int) -> x"
            "match x with (Lazy x) -> x", "match x with Lazy x -> x"
            "match x with (x, y) -> x, y", "match x with x, y -> x, y"
            "match x with (struct (x, y)) -> x, y", "match x with struct (x, y) -> x, y"
            "match x with x when (true) -> x | y -> y", "match x with x when true -> x | y -> y"
            "match x with x when (match x with _ -> true) -> x | y -> y", "match x with x when (match x with _ -> true) -> x | y -> y"
            "match x with x when (let x = 3 in match x with _ -> true) -> x | y -> y", "match x with x when (let x = 3 in match x with _ -> true) -> x | y -> y"

            // LetOrUse
            "let () = ()", "let () = ()"
            "let (()) = ()", "let () = ()"
            "let (_) = y", "let _ = y"
            "let (x) = y", "let x = y"
            "let (x: int) = y", "let x: int = y"
            "let (x, y) = x, y", "let x, y = x, y"
            "let (struct (x, y)) = x, y", "let struct (x, y) = x, y"
            "let (x & y) = z", "let x & y = z"
            "let (x as y) = z", "let x as y = z"
            "let (Lazy x) = y", "let (Lazy x) = y"
            "let (Lazy _ | _) = z", "let Lazy _ | _ = z"
            "let f () = ()", "let f () = ()"
            "let f (_) = ()", "let f _ = ()"
            "let f (x) = x", "let f x = x"
            "let f (x: int) = x", "let f (x: int) = x"
            "let f x (y) = x", "let f x y = x"
            "let f (Lazy x) = x", "let f (Lazy x) = x"
            "let f (x, y) = x, y", "let f (x, y) = x, y"
            "let f (struct (x, y)) = x, y", "let f struct (x, y) = x, y"

            // TryWith
            "try raise null with () -> ()", "try raise null with () -> ()"
            "try raise null with (_) -> ()", "try raise null with _ -> ()"
            "try raise null with (x) -> x", "try raise null with x -> x"
            "try raise null with (:? exn) -> ()", "try raise null with :? exn -> ()"
            "try raise null with (Failure x) -> x", "try raise null with Failure x -> x"
            "try raise null with x when (true) -> x | y -> y", "try raise null with x when true -> x | y -> y"
            "try raise null with x when (match x with _ -> true) -> x | y -> y", "try raise null with x when (match x with _ -> true) -> x | y -> y"
            "try raise null with x when (let x = 3 in match x with _ -> true) -> x | y -> y", "try raise null with x when (let x = 3 in match x with _ -> true) -> x | y -> y"

            // Sequential
            "let (x) = y; z in x", "let x = y; z in x"

            // LetOrUseBang
            "let! () = ()", "let! () = ()"
            "let! (()) = ()", "let! () = ()"
            "let! (_) = y", "let! _ = y"
            "let! (x) = y", "let! x = y"
            "let! (x: int) = y", "let! (x: int) = y"
            "let! (x, y) = x, y", "let! x, y = x, y"
            "let! (struct (x, y)) = x, y", "let! struct (x, y) = x, y"
            "let! (x & y) = z", "let! x & y = z"
            "let! (x as y) = z", "let! x as y = z"
            "let! (Lazy x) = y", "let! Lazy x = y"
            "let! (Lazy _ | _) = z", "let! Lazy _ | _ = z"
            
            // MatchBang
            "async { match! x with () -> return () }", "async { match! x with () -> return () }"
            "async { match! x with (_) -> return () }", "async { match! x with _ -> return () }"
            "async { match! x with (x) -> return x }", "async { match! x with x -> return x }"
            "async { match! x with (x: int) -> return x }", "async { match! x with (x: int) -> return x }"
            "async { match! x with (Lazy x) -> return x }", "async { match! x with Lazy x -> return x }"
            "async { match! x with (x, y) -> return x, y }", "async { match! x with x, y -> return x, y }"
            "async { match! x with (struct (x, y)) -> return x, y }", "async { match! x with struct (x, y) -> return x, y }"
            "async { match! x with x when (true) -> return x | y -> return y }", "async { match! x with x when true -> return x | y -> return y }"
            "async { match! x with x when (match x with _ -> true) -> return x | y -> return y }", "async { match! x with x when (match x with _ -> true) -> return x | y -> return y }"
            "async { match! x with x when (let x = 3 in match x with _ -> true) -> return x | y -> return y }", "async { match! x with x when (let x = 3 in match x with _ -> true) -> return x | y -> return y }"
        }

    [<Theory; MemberData(nameof patternsInExprs)>]
    let ``Patterns in expressions`` original expected = Expressions.expectFix original expected

    let args =
        memberData {
            "type T = static member M() = ()", "type T = static member M() = ()"
            "type T = static member M(_) = ()", "type T = static member M _ = ()"
            "type T = static member M(x) = x", "type T = static member M x = x"
            "type T = static member M(x: int) = x", "type T = static member M(x: int) = x"
            "type T = static member inline M([<InlineIfLambda>] f) = ()", "type T = static member inline M([<InlineIfLambda>] f) = ()"
            "type T = static member M x (y) = x", "type T = static member M x y = x"
            "type T = static member M(Lazy x) = x", "type T = static member M(Lazy x) = x"
            "type T = static member M(Failure _ | _) = ()", "type T = static member M(Failure _ | _) = ()"
            "type T = static member M(x & y) = ()", "type T = static member M(x & y) = ()"
            "type T = static member M(x as y) = ()", "type T = static member M(x as y) = ()"
            "type T = static member M(x :: xs) = ()", "type T = static member M(x :: xs) = ()"
            "type T = static member M(x, y) = x, y", "type T = static member M(x, y) = x, y"
            "type T = static member M(struct (x, y)) = x, y", "type T = static member M struct (x, y) = x, y"
            "type T = static member M(?x) = ()", "type T = static member M ?x = ()"
            "type T = static member M(?x: int) = ()", "type T = static member M(?x: int) = ()"

            "type T = member _.M() = ()", "type T = member _.M() = ()"
            "type T = member _.M(_) = ()", "type T = member _.M _ = ()"
            "type T = member _.M(x) = x", "type T = member _.M x = x"
            "type T = member _.M(x: int) = x", "type T = member _.M(x: int) = x"
            "type T = member inline _.M([<InlineIfLambda>] f) = ()", "type T = member inline _.M([<InlineIfLambda>] f) = ()"
            "type T = member _.M x (y) = x", "type T = member _.M x y = x"
            "type T = member _.M(Lazy x) = x", "type T = member _.M(Lazy x) = x"
            "type T = member _.M(Failure _ | _) = ()", "type T = member _.M(Failure _ | _) = ()"
            "type T = member _.M(x & y) = ()", "type T = member _.M(x & y) = ()"
            "type T = member _.M(x as y) = ()", "type T = member _.M(x as y) = ()"
            "type T = member _.M(x :: xs) = ()", "type T = member _.M(x :: xs) = ()"
            "type T = member _.M(x, y) = x, y", "type T = member _.M(x, y) = x, y"
            "type T = member _.M(struct (x, y)) = x, y", "type T = member _.M struct (x, y) = x, y"
            "type T = member _.M(?x) = ()", "type T = member _.M ?x = ()"
            "type T = member _.M(?x: int) = ()", "type T = member _.M(?x: int) = ()"
        }

    [<Theory; MemberData(nameof args)>]
    let ``Argument patterns`` original expected = TopLevel.expectFix original expected

    module InfixPatterns =
        let infixPatterns =
            memberData {
                "A | (B | C)", "A | B | C"
                "A & (B | C)", "A & (B | C)"
                "A :: (B | C)", "A :: (B | C)"
                "A as (B | C)", "A as (B | C)"
                "A as (B | C) & D", "A as (B | C) & D"
                "A as (_, _) & D", "A as (_, _) & D"
                "A | (B & C)", "A | B & C"
                "A & (B & C)", "A & B & C"
                "A :: (B & C)", "A :: (B & C)"
                "A as (B & C)", "A as (B & C)"
                "A | (B :: C)", "A | B :: C"
                "A & (B :: C)", "A & B :: C"
                "A :: (B :: C)", "A :: B :: C"
                "A as (B :: C)", "A as B :: C"
                "A | (B as C)", "A | (B as C)"
                "_ as x | (_ as x)", "_ as x | (_ as x)"
                "A & (B as C)", "A & (B as C)"
                "A :: (B as C)", "A :: (B as C)"
                "A as (B as C)", "A as B as C"

                "(A | B) | C", "A | B | C"
                "(A | B) & C", "(A | B) & C"
                "(A | B) :: C", "(A | B) :: C"
                "(A | B) as C", "A | B as C"
                "(A & B) | C", "A & B | C"
                "(A & B) & C", "A & B & C"
                "(A & B) :: C", "(A & B) :: C"
                "(A & B) as C", "A & B as C"
                "A | (B & C) as _", "A | B & C as _"
                "(A :: B) | C", "A :: B | C"
                "(A :: B) & C", "A :: B & C"
                "(x :: y) :: xs", "(x :: y) :: xs"
                "(A :: B) as C", "A :: B as C"
                "(A as B) | C", "(A as B) | C"
                "(A as B) & C", "(A as B) & C"
                "(A as B) :: C", "(A as B) :: C"
                "(A as B) as C", "A as B as C"
            }

        [<Theory; MemberData(nameof infixPatterns)>]
        let ``Infix patterns`` pat expected = expectFix pat expected
