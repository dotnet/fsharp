// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.RemoveUnnecessaryParenthesesTests

open System.Text
open FSharp.Editor.Tests.Helpers
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks
open Xunit
open CodeFixTestFramework

[<AutoOpen>]
module private TopLevel =
    let private fixer = FSharpRemoveUnnecessaryParenthesesCodeFixProvider()

    // It is much (2–3×) faster to reuse the same solution
    // rather than creating a new one for each test.
    // Unfortunately, it is not safe to reuse the same solution across
    // tests that may set different project or editor options,
    // because they may run concurrently on other threads,
    // and the in-memory settings store used for tests is global,
    // so we restrict this optimization to this file.
    let private sln, projId =
        let projInfo =
            RoslynTestHelpers.CreateProjectInfo (ProjectId.CreateNewId()) "C:\\test.fsproj" []

        let sln = RoslynTestHelpers.CreateSolution [ projInfo ]

        let projectOptions =
            { RoslynTestHelpers.DefaultProjectOptions with
                OtherOptions =
                    [|
                        "--targetprofile:netcore" // without this lib some symbols are not loaded
                        "--nowarn:3384" // The .NET SDK for this script could not be determined
                    |]
            }

        RoslynTestHelpers.SetProjectOptions projInfo.Id sln projectOptions

        RoslynTestHelpers.SetEditorOptions
            sln
            { CodeFixesOptions.Default with
                RemoveParens = true
            }

        sln, projInfo.Id

    let private tryFix (code: string) =
        cancellableTask {
            let document =
                let docInfo = RoslynTestHelpers.CreateDocumentInfo projId "C:\\test.fs" code
                (sln.AddDocument docInfo).GetDocument docInfo.Id

            let sourceText = SourceText.From code

            let! diagnostics = FSharpDocumentDiagnosticAnalyzer.GetDiagnostics(document, DiagnosticsType.Syntax)
            let context = CodeFixContext.tryCreate fixer.CanFix document diagnostics

            return!
                context
                |> ValueOption.either (fixer :> IFSharpCodeFixProvider).GetCodeFixIfAppliesAsync (CancellableTask.singleton ValueNone)
                |> CancellableTask.map (ValueOption.map (TestCodeFix.ofFSharpCodeFix sourceText) >> ValueOption.toOption)
        }

    let expectFix = expectFix tryFix

module Expressions =
    /// let f x y z = expr
    let expectFix expr expected =
        let code =
            $"
let _ =
    %s{expr}
"

        let expected =
            $"
let _ =
    %s{expected}
"

        expectFix code expected

    [<Fact>]
    let ``Beginning of file: (printfn "Hello, world")`` () =
        TopLevel.expectFix "(printfn \"Hello, world\")" "printfn \"Hello, world\""

    [<Fact>]
    let ``End of file: let x = (1)`` () =
        TopLevel.expectFix "let x = (1)" "let x = 1"

    let unmatchedParens =
        memberData {
            "(", "("
            ")", ")"
            "(()", "(()"
            "())", "())"
            "(x", "(x"
            "x)", "x)"
            "((x)", "(x"
            "(x))", "x)"
            "((((x)", "(((x"
            "(x))))", "x)))"
            "(x + (y + z)", "(x + y + z"
            "((x + y) + z", "(x + y + z"
            "x + (y + z))", "x + y + z)"
            "(x + y) + z)", "x + y + z)"
        }

    [<Theory; MemberData(nameof unmatchedParens)>]
    let ``Unmatched parentheses`` expr expected = expectFix expr expected

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
            "(fun x -> x), y", "(fun x -> x), y"
            "3, (null: string)", "3, (null: string)"

            "
            let _ =
                1,
                (true
                 || false
                 || true),
                1
            ",
            "
            let _ =
                1,
                true
                || false
                || true,
                1
            "

            "
            let (a,
                 b,
                 c,
                 d,
                 e,
                 f) = g
            ",
            "
            let a,
                b,
                c,
                d,
                e,
                f = g
            "

            "
            let (a,
                 b,
                 c,
                 d,
                 e,
                 f) =
                 g
            ",
            "
            let (a,
                 b,
                 c,
                 d,
                 e,
                 f) =
                 g
            "

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
            "new ResizeArray<int>(3)", "new ResizeArray<int> 3"
            "let x = 3 in new ResizeArray<int>(x)", "let x = 3 in new ResizeArray<int>(x)"
            "ResizeArray<int>([3])", "ResizeArray<int> [3]"
            "new ResizeArray<int>([3])", "new ResizeArray<int>([3])"
            "ResizeArray<int>([|3|])", "ResizeArray<int> [|3|]"
            "new ResizeArray<int>([|3|])", "new ResizeArray<int> [|3|]"

            // ObjExpr
            "{ new System.IDisposable with member _.Dispose () = (ignore 3) }",
            "{ new System.IDisposable with member _.Dispose () = ignore 3 }"

            // While
            "while (true) do ()", "while true do ()"
            "while true do (ignore 3)", "while true do ignore 3"
            "while (match () with _ -> true) do ()", "while (match () with _ -> true) do ()"

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
            "for x in (try [] with _ -> []) do ()", "for x in (try [] with _ -> []) do ()"

            // ArrayOrListComputed
            "[1; 2; (if x then 3 else 4); 5]", "[1; 2; (if x then 3 else 4); 5]"
            "[|1; 2; (if x then 3 else 4); 5|]", "[|1; 2; (if x then 3 else 4); 5|]"

            // IndexRange
            "[(1)..10]", "[1..10]"
            "[1..(10)]", "[1..10]"
            "[|(1)..10|]", "[|1..10|]"
            "[|1..(10)|]", "[|1..10|]"

            // IndexFromEnd
            "[0][..^(0)]", "[0][..^0]"

            // ComputationExpression
            "seq { (3) }", "seq { 3 }"
            "async { return (3) }", "async { return 3 }"

            "
            async {
                return (
                1
                )
            }
            ",
            "
            async {
                return (
                1
                )
            }
            "

            "
            async {
                return (
                 1
                )
            }
            ",
            "
            async {
                return 
                 1
            }
            "

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

            "match 3 with 1 -> (match 3 with _ -> 3) | _ -> match 3 with _ -> 3",
            "match 3 with 1 -> (match 3 with _ -> 3) | _ -> match 3 with _ -> 3"

            "match 3 with 1 -> (match 3 with _ -> 3) | _ -> (match 3 with _ -> 3)",
            "match 3 with 1 -> (match 3 with _ -> 3) | _ -> match 3 with _ -> 3"

            "3 > (match x with _ -> 3)", "3 > match x with _ -> 3"
            "(match x with _ -> 3) > 3", "(match x with _ -> 3) > 3"
            "match x with 1 -> (fun x -> x) | _ -> id", "match x with 1 -> (fun x -> x) | _ -> id"

            "match (try () with _ -> ()) with () -> ()", "match (try () with _ -> ()) with () -> ()"

            "
            match (try () with _ -> ()) with
            | () -> ()
            ",
            "
            match (try () with _ -> ()) with
            | () -> ()
            "

            "
            3 > (match x with
                 | 1
                 | _ -> 3)
            ",
            "
            3 > match x with
                | 1
                | _ -> 3
            "

            "
            3 > ( match x with
                | 1
                | _ -> 3)
            ",
            "
            3 > match x with
                | 1
                | _ -> 3
            "

            "
            3 > (match x with
                | 1
                | _ -> 3)
            ",
            "
            3 > match x with
                | 1
                | _ -> 3
            "

            "
            3 > (match x with
                 // Lol.
                | 1
                | _ -> 3)
            ",
            "
            3 > match x with
                 // Lol.
                | 1
                | _ -> 3
            "

            "
            3 >(match x with
               | 1
               | _ -> 3)
            ",
            "
            3 >match x with
               | 1
               | _ -> 3
            "

            "
            f(match x with
             | 1
             | _ -> 3)
            ",
            "
            f(match x with
             | 1
             | _ -> 3)
            "

            "match () with () when (box x :? int) -> () | _ -> ()", "match () with () when (box x :? int) -> () | _ -> ()"

            "
            match () with
            | () when (box x :? int)
                -> ()
            | _ -> ()
            ",
            "
            match () with
            | () when box x :? int
                -> ()
            | _ -> ()
            "

            // Do
            "do (ignore 3)", "do ignore 3"

            // Assert
            "assert(true)", "assert true"
            "assert (true)", "assert true"
            "assert (not false)", "assert (not false)" // Technically we could remove here, but probably better not to.
            "assert (2 + 2 = 5)", "assert (2 + 2 = 5)"

            // App
            "id (3)", "id 3"
            "id(3)", "id 3"
            "id id (3)", "id id 3"
            "id<int>(3)", "id<int> 3"
            "nameof(nameof)", "nameof nameof"
            "(x) :: []", "x :: []"
            "x :: ([])", "x :: []"

            """
            let longVarName1 = 1
            let longVarName2 = 2
            (longFunctionName
                longVarName1
                longVarName2)
            """,
            """
            let longVarName1 = 1
            let longVarName2 = 2
            longFunctionName
                longVarName1
                longVarName2
            """

            """
            let longVarName1 = 1
            let longVarName2 = 2
            (longFunctionName
                longVarName1
                longVarName2
                )
            """,
            """
            let longVarName1 = 1
            let longVarName2 = 2
            longFunctionName
                longVarName1
                longVarName2
            """

            """
            let longVarName1 = 1
            let longVarName2 = 2
            (longFunctionName
                longVarName1
                longVarName2
            )
            """,
            """
            let longVarName1 = 1
            let longVarName2 = 2
            longFunctionName
                longVarName1
                longVarName2
            """

            """
            let longVarName1 = 1
            let longVarName2 = 2
            (
                longFunctionName
                    longVarName1
                    longVarName2
            )
            """,
            """
            let longVarName1 = 1
            let longVarName2 = 2
            longFunctionName
                longVarName1
                longVarName2
            """

            // We could remove here, but we'd need to update
            // the "sensitive indentation" logic to differentiate between
            // an outer offsides column established by an open paren (as here)
            // and an outer offsides column established by, say, the leftmost
            // column of a binding, e.g.:
            //
            //     let _ = (
            //     ↑
            //     )
            //
            //     static member M () = (
            //     ↑
            //     )
            """
            let longVarName1 = 1
            let longVarName2 = 2
            (
            longFunctionName
                longVarName1
                longVarName2
            )
            """,
            """
            let longVarName1 = 1
            let longVarName2 = 2
            (
            longFunctionName
                longVarName1
                longVarName2
            )
            """

            """
            let longVarName1 = 1
            let longVarName2 = 2
            (
        longFunctionName
            longVarName1
            longVarName2
            )
            """,
            """
            let longVarName1 = 1
            let longVarName2 = 2
            (
        longFunctionName
            longVarName1
            longVarName2
            )
            """

            """
            let longVarName1 = 1
            let longVarName2 = 2
            (+longFunctionName
                longVarName1
                longVarName2)
            """,
            """
            let longVarName1 = 1
            let longVarName2 = 2
            +longFunctionName
                longVarName1
                longVarName2
            """

            """
            let longVarName1 = 1
            let longVarName2 = 2 in (
                longFunctionName
                    longVarName1
                    longVarName2
            )
            """,
            """
            let longVarName1 = 1
            let longVarName2 = 2 in 
                longFunctionName
                    longVarName1
                    longVarName2
            """

            """
            let x = (printfn $"{y}"
                     2)
            in x
            """,
            """
            let x = printfn $"{y}"
                    2
            in x
            """

            "
            let x = (2
         +             2)
            in x
            ",
            "
            let x = (2
         +             2)
            in x
            "

            "
            let x =
             (2
            + 2)
            in x
            ",
            "
            let x =
             2
           + 2
            in x
            "

            // The extra spaces in this test are intentional.
            "
            let x =
     

             (2
     

            + 2)
            in x
            ",
            "
            let x =
     

             2
    

           + 2
            in x
            "

            "
            let x = (
              2
            + 2)
            in x
            ",
            "
            let x = 
              2
            + 2
            in x
            "

            "
            let x = (2
                     +             2)
            in x
            ",
            "
            let x = 2
                    +             2
            in x
            "

            "
            let x = (2
                   +             2)
            in x
            ",
            "
            let x = 2
                  +             2
            in x
            "

            "
            let x = (2
                   + 2)
            in x
            ",
            "
            let x = 2
                  + 2
            in x
            "

            "
            let x = (x
                    +y)
            in x
            ",
            "
            let x = x
                   +y
            in x
            "

            "
            let x = (2
                     +2)
            in x
            ",
            "
            let x = 2
                    +2
            in x
            "

            "
            let x = (2
                 <<< 2)
            in x
            ",
            "
            let x = 2
                <<< 2
            in x
            "

            "
let (<<<<<<<<) = (<<<)
let x = (2
<<<<<<<< 2)
in x
            ",
            "
let (<<<<<<<<) = (<<<)
let x = 2
<<<<<<<< 2
in x
            "

            "
            let x = (
                     2
         +             2)
            in x
            ",
            "
            let x = (
                     2
         +             2)
            in x
            "

            "
            let x = (
                     2
                     +             2)
            in x
            ",
            "
            let x = 
                     2
                     +             2
            in x
            "

            "
            let x =
                (2
             + 3
                )

            let y =
                (2
               + 2)

            in x + y
            ",
            "
            let x =
                (2
             + 3
                )

            let y =
                2
              + 2

            in x + y
            "

            "
            x <
                (2
             + 3
                )
            ",
            "
            x <
                (2
             + 3
                )
            "

            "
            x <
                (2
               + 3
                )
            ",
            "
            x <
                2
              + 3
            "

            // LetOrUse
            "let x = 3 in let y = (4) in x + y", "let x = 3 in let y = 4 in x + y"
            "let x = 3 in let y = 4 in (x + y)", "let x = 3 in let y = 4 in x + y"
            "let x = 3 in let y =(4) in x + y", "let x = 3 in let y = 4 in x + y"
            "let x = 3 in let y=(4) in x + y", "let x = 3 in let y=4 in x + y"

            "
            let _ =
                let _ = 3
                (
                    44
                )
            ",
            "
            let _ =
                let _ = 3
                44
            "

            "
            let (++++++) = (+)
            let x =
             ()
             (
                2
         ++++++ 2
             )
            in x
            ",
            "
            let (++++++) = (+)
            let x =
             ()
             2
      ++++++ 2
            in x
            "

            "
            let (!+++++) = (+)
            let x =
             ()
             (
              !+++++2
             )
            in x
            ",
            "
            let (!+++++) = (+)
            let x =
             ()
             !+++++2
            in x
            "

            "
            let _ =
                (
             printfn \"\"
             +2
                )
            ",
            "
            let _ =
                printfn \"\"
                +2
            "

            "
            let _ =
                let x = 3
                (
                    let y = 99
                    y - x
                )
            ",
            "
            let _ =
                let x = 3
                let y = 99
                y - x
            "

            // TryWith
            "try (raise null) with _ -> reraise ()", "try raise null with _ -> reraise ()"
            "try raise null with (_) -> reraise ()", "try raise null with _ -> reraise ()"
            "try raise null with _ -> (reraise ())", "try raise null with _ -> reraise ()"

            "try raise null with :? exn -> (try raise null with _ -> reraise ()) | _ -> reraise ()",
            "try raise null with :? exn -> (try raise null with _ -> reraise ()) | _ -> reraise ()"

            "try (try raise null with _ -> null) with _ -> null", "try (try raise null with _ -> null) with _ -> null"
            "try (try raise null with _ -> null ) with _ -> null", "try (try raise null with _ -> null ) with _ -> null"

            // TryFinally
            "try (raise null) finally 3", "try raise null finally 3"
            "try raise null finally (3)", "try raise null finally 3"
            "try (try raise null with _ -> null) finally null", "try (try raise null with _ -> null) finally null"

            // Lazy
            "lazy(3)", "lazy 3"
            "lazy (3)", "lazy 3"
            "lazy (id 3)", "lazy (id 3)" // Technically we could remove here, but probably better not to.

            // Sequential
            """ (printfn "1"); printfn "2" """, """ printfn "1"; printfn "2" """
            """ printfn "1"; (printfn "2") """, """ printfn "1"; printfn "2" """
            "let x = 3; (5) in x", "let x = 3; 5 in x"

            """
            [
                ()
                (printfn "1"; ())
                ()
            ]
            """,
            """
            [
                ()
                (printfn "1"; ())
                ()
            ]
            """

            // Technically we could remove some parens in some of these,
            // but additional exprs above or below can suddenly move the offsides line,
            // and we don't want to do unbounded lookahead or lookbehind.

            "
            let _ =
                [
                   (if p then q else r);
                    y
                ]
            ",
            "
            let _ =
                [
                   (if p then q else r);
                    y
                ]
            "

            "
            let _ =
                [
                    (if p then q else r);
                    y
                ]
            ",
            "
            let _ =
                [
                    if p then q else r;
                    y
                ]
            "

            "
            let _ =
                [
                    x;
                   (if p then q else r);
                   (if foo then bar else baz);
                ]
            ",
            "
            let _ =
                [
                    x;
                   (if p then q else r);
                   if foo then bar else baz;
                ]
            "

            "
            let _ =
                [
                   (if p then q else r);
                    y;
                   (if foo then bar else baz);
                ]
            ",
            "
            let _ =
                [
                   (if p then q else r);
                    y;
                   if foo then bar else baz;
                ]
            "

            "
            let _ =
                [
                   (if p then q else r);
                   (if foo then bar else baz);
                    z
                ]
            ",
            "
            let _ =
                [
                   if p then q else r;
                   (if foo then bar else baz);
                    z
                ]
            "

            "
            let _ =
                [
                    x;
                   (if a then b else c);
                   (if p then q else r);
                   (if foo then bar else baz);
                    z
                ]
            ",
            "
            let _ =
                [
                    x;
                   (if a then b else c);
                   (if p then q else r);
                   (if foo then bar else baz);
                    z
                ]
            "

            "
            let _ =
                let y = 100
                let x = 3
                (
                    let y = 99
                    ignore (y - x)
                )
                x + y
            ",
            "
            let _ =
                let y = 100
                let x = 3
                (
                    let y = 99
                    ignore (y - x)
                )
                x + y
            "

            "
            let _ =
                let y = 100
                let x = 3
                (
                    let y = 99
                    ignore (y - x)
                )
                x
            ",
            "
            let _ =
                let y = 100
                let x = 3
                let y = 99
                ignore (y - x)
                x
            "

            "
            let f y =
                let x = 3
                (
                    let y = 99
                    ignore (y - x)
                )
                x + y
            ",
            "
            let f y =
                let x = 3
                (
                    let y = 99
                    ignore (y - x)
                )
                x + y
            "

            "
            let f y =
                let x = 3
                (
                    let y = 99
                    ignore (y - x)
                )
                x
            ",
            "
            let f y =
                let x = 3
                let y = 99
                ignore (y - x)
                x
            "

            "
            [
                1, 2,
                3, 4
                (1, 2,
                3, 4)
            ]
            ",
            "
            [
                1, 2,
                3, 4
                (1, 2,
                3, 4)
            ]
            "

            // IfThenElse
            "if (3 = 3) then 3 else 3", "if 3 = 3 then 3 else 3"
            "if 3 = 3 then (3) else 3", "if 3 = 3 then 3 else 3"
            "if 3 = 3 then 3 else (3)", "if 3 = 3 then 3 else 3"
            "(if true then 1 else 2) > 3", "(if true then 1 else 2) > 3"
            "3 > (if true then 1 else 2)", "3 > if true then 1 else 2"
            "if (if true then true else true) then 3 else 3", "if (if true then true else true) then 3 else 3"
            "if (id <| if true then true else true) then 3 else 3", "if (id <| if true then true else true) then 3 else 3"
            "if id <| (if true then true else true) then 3 else 3", "if id <| (if true then true else true) then 3 else 3"

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

            """
            if
                (printfn "1"
                 true)
            then
                ()
            """,
            """
            if
                (printfn "1"
                 true)
            then
                ()
            """

            "if (match () with _ -> true) then ()", "if (match () with _ -> true) then ()"

            "if (match () with _ -> true) && (match () with _ -> true) then ()",
            "if (match () with _ -> true) && (match () with _ -> true) then ()"

            // LongIdent
            "(|Failure|_|) null", "(|Failure|_|) null"

            // LongIdentSet
            "let r = ref 3 in r.Value <- (3)", "let r = ref 3 in r.Value <- 3"

            // DotGet
            "([]).Length", "[].Length"
            "([] : int list).Length", "([] : int list).Length"

            "Debug.Assert((xT.DeclaringType :?> ProvidedTypeDefinition).BelongsToTargetModel)",
            "Debug.Assert((xT.DeclaringType :?> ProvidedTypeDefinition).BelongsToTargetModel)"

            "Debug.Assert ((xT.DeclaringType :?> ProvidedTypeDefinition).BelongsToTargetModel)",
            "Debug.Assert (xT.DeclaringType :?> ProvidedTypeDefinition).BelongsToTargetModel"

            "Assert((xT.DeclaringType :?> ProvidedTypeDefinition).BelongsToTargetModel)",
            "Assert((xT.DeclaringType :?> ProvidedTypeDefinition).BelongsToTargetModel)"

            // DotLambda
            """_.ToString("x")""", """_.ToString("x")"""
            """_.ToString(("x"))""", """_.ToString("x")"""

            // DotSet
            "(ref 3).Value <- (3)", "(ref 3).Value <- 3"
            "(ref 3).Value <- (id 3)", "(ref 3).Value <- id 3"

            // Set
            "let mutable x = 3 in (x) <- 3", "let mutable x = 3 in x <- 3"
            "let mutable x = 3 in x <- (3)", "let mutable x = 3 in x <- 3"

            """
            let mutable x = 3
            x <- (printfn $"{y}"; 3)
            """,
            """
            let mutable x = 3
            x <- (printfn $"{y}"; 3)
            """

            """
            let mutable x = 3
            x <- (printfn $"{y}"
                  3)
            """,
            """
            let mutable x = 3
            x <- (printfn $"{y}"
                  3)
            """

            """
            let mutable x = 3
            x <- (3
              <<< 3)
            """,
            """
            let mutable x = 3
            x <- 3
             <<< 3
            """

            // DotIndexedGet
            "[(0)][0]", "[0][0]"
            "[0][(0)]", "[0][0]"
            "([0])[0]", "[0][0]"

            // DotIndexedSet
            "[|(0)|][0] <- 0", "[|0|][0] <- 0"
            "[|0|][(0)] <- 0", "[|0|][0] <- 0"
            "[|0|][0] <- (0)", "[|0|][0] <- 0"

            // NamedIndexedPropertySet
            "let xs = [|0|] in xs.Item(0) <- 0", "let xs = [|0|] in xs.Item 0 <- 0"
            "let xs = [|0|] in xs.Item 0 <- (0)", "let xs = [|0|] in xs.Item 0 <- 0"

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
            "let o : int = downcast (null) in o", "let o : int = downcast null in o"

            // AddressOf
            "let mutable x = 0 in System.Int32.TryParse (null, &(x))", "let mutable x = 0 in System.Int32.TryParse (null, &x)"

            // TraitCall
            "let inline f x = (^a : (static member Parse : string -> ^a) (x))",
            "let inline f x = (^a : (static member Parse : string -> ^a) x)"

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

            "async { use! x = async { return (Unchecked.defaultof<System.IDisposable>) } in return () }",
            "async { use! x = async { return Unchecked.defaultof<System.IDisposable> } in return () }"

            // MatchBang
            "async { match! (async { return 3 }) with _ -> return () }", "async { match! async { return 3 } with _ -> return () }"
            "async { match! async { return 3 } with _ -> (return ()) }", "async { match! async { return 3 } with _ -> return () }"

            "async { match! async { return 3 } with _ when (true) -> return () }",
            "async { match! async { return 3 } with _ when true -> return () }"

            "async { match! async { return 3 } with 4 -> return (match 3 with _ -> ()) | _ -> return () }",
            "async { match! async { return 3 } with 4 -> return (match 3 with _ -> ()) | _ -> return () }"

            "async { match! async { return 3 } with 4 -> return (let x = 3 in match x with _ -> ()) | _ -> return () }",
            "async { match! async { return 3 } with 4 -> return (let x = 3 in match x with _ -> ()) | _ -> return () }"

            "async { match! async { return 3 } with _ when (match 3 with _ -> true) -> return () }",
            "async { match! async { return 3 } with _ when (match 3 with _ -> true) -> return () }"

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
            "$\"{(x, y)}\"", "$\"{(x, y)}\""

            "$\"{(if true then 1 else 0)}\"", "$\"{if true then 1 else 0}\""
            "$\"{(if true then 1 else 0):N0}\"", "$\"{(if true then 1 else 0):N0}\""
            "$\"{(if true then 1 else 0),-3}\"", "$\"{(if true then 1 else 0),-3}\""
            "$\"{(match () with () -> 1):N0}\"", "$\"{(match () with () -> 1):N0}\""
            "$\"{(match () with () -> 1),-3}\"", "$\"{(match () with () -> 1),-3}\""
            "$\"{(try () with _ -> 1):N0}\"", "$\"{(try () with _ -> 1):N0}\""
            "$\"{(try () with _ -> 1),-3}\"", "$\"{(try () with _ -> 1),-3}\""
            "$\"{(try 1 finally ()):N0}\"", "$\"{(try 1 finally ()):N0}\""
            "$\"{(try 1 finally ()),-3}\"", "$\"{(try 1 finally ()),-3}\""
            "$\"{(let x = 3 in x):N0}\"", "$\"{(let x = 3 in x):N0}\""
            "$\"{(let x = 3 in x),-3}\"", "$\"{(let x = 3 in x),-3}\""
            "$\"{(do (); 3):N0}\"", "$\"{(do (); 3):N0}\""
            "$\"{(do (); 3),-3}\"", "$\"{(do (); 3),-3}\""
            "$\"{(x <- 3):N0}\"", "$\"{(x <- 3):N0}\""
            "$\"{(x <- 3),-3}\"", "$\"{(x <- 3),-3}\""
            "$\"{(1, 2):N0}\"", "$\"{(1, 2):N0}\""
            "$\"{(1, 2),-3}\"", "$\"{(1, 2),-3}\""

            "$\"{((); ())}\"", "$\"{((); ())}\""
            "$\"{(do (); ())}\"", "$\"{do (); ()}\""
            "$\"{(let x = 3 in ignore x; 99)}\"", "$\"{let x = 3 in ignore x; 99}\""

            """
            $"{(3 + LanguagePrimitives.GenericZero<int>):N0}"
            """,
            """
            $"{3 + LanguagePrimitives.GenericZero<int> :N0}"
            """

            // LibraryOnlyILAssembly
            """(# "ldlen.multi 2 0" array : int #)""", """(# "ldlen.multi 2 0" array : int #)"""

            // Miscellaneous
            "
            (match x with
             | 1 -> ()
             | _ -> ())

            y
            ",
            "
            match x with
            | 1 -> ()
            | _ -> ()

            y
            "
        }

    [<Theory; MemberData(nameof exprs)>]
    let ``Basic expressions`` expr expected = expectFix expr expected

    module FunctionApplications =
        let functionApplications =
            memberData {
                // Paren
                "id ()", "id ()"
                "id (())", "id (())"
                "id ((x))", "id (x)"
                "x.M(())", "x.M(())"
                "x.M (())", "x.M (())"
                "x.M.N(())", "x.M.N(())"

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
                "id -(+1e10)", "id -(+1e10)"
                "id ~~~(1y)", "id ~~~1y"
                "id ~~~(-0b11111111y)", "id ~~~(-0b11111111y)"
                "id -(0b1)", "id -0b1"
                "id -(0x1)", "id -0x1"
                "id -(0o1)", "id -0o1"
                "id -(1e4)", "id -1e4"
                "id -(1e-4)", "id -1e-4"
                "id -(-(-x))", "id -(-(-x))"
                "(~-) -(-(-x))", "(~-) -(-(-x))"
                "id -(-(-3))", "id -(- -3)"
                "id -(- -3)", "id -(- -3)"
                "-(x)", "-x"
                "-(3)", "-3"
                "-(-x)", "-(-x)"
                "-(-3)", "- -3"
                "-(- -x)", "-(- -x)"
                "-(- -3)", "-(- -3)"
                "~~~(-1)", "~~~ -1"
                "~~~(-1)", "~~~ -1"
                "~~~(-1y)", "~~~ -1y"
                "~~~(+1)", "~~~ +1"
                "~~~(+1y)", "~~~ +1y"
                "~~~(+1uy)", "~~~(+1uy)"
                "(3).ToString()", "(3).ToString()"
                "(3l).ToString()", "3l.ToString()"
                "(-3).ToString()", "(-3).ToString()"
                "(-x).ToString()", "(-x).ToString()"
                "(-3y).ToString()", "-3y.ToString()"
                "(3y).ToString()", "3y.ToString()"
                "(1.).ToString()", "(1.).ToString()"
                "(1.0).ToString()", "1.0.ToString()"
                "(1e10).ToString()", "1e10.ToString()"
                "(-1e10).ToString()", "-1e10.ToString()"
                "(1)<(id<_>1)>true", "(1)<(id<_>1)>true"
                "(1<1)>true", "(1<1)>true"
                "true<(1>2)", "true<(1>2)"
                "(1)<1>true", "(1)<1>true"
                "(1<2),2>3", "(1<2),2>3"
                "1<2,(2>3)", "1<2,(2>3)"
                "(1)<2,2>3", "(1)<2,2>3"
                """ let (~+) _ = true in assert +($"{true}") """, """ let (~+) _ = true in assert +($"{true}") """
                """ let (~-) s = false in lazy -($"") """, """ let (~-) s = false in lazy - $"" """
                """ let (~-) s = $"-%s{s}" in id -($"") """, """ let (~-) s = $"-%s{s}" in id -($"") """
                """ let (~-) s = $"-%s{s}" in id -(@"") """, """ let (~-) s = $"-%s{s}" in id -(@"") """
                """ let (~-) s = $"-%s{s}" in id -(@$"") """, """ let (~-) s = $"-%s{s}" in id -(@$"") """
                """ let (~-) s = $"-%s{s}" in id -($@"") """, """ let (~-) s = $"-%s{s}" in id -($@"") """
                "let (~-) q = q in id -(<@ () @>)", "let (~-) q = q in id -(<@ () @>)"
                "let ``f`` x = x in ``f``(3)", "let ``f`` x = x in ``f`` 3"

                // Typed
                "id (x : int)", "id (x : int)"
                """ "abc".Contains (x : char, StringComparison.Ordinal) """, """ "abc".Contains (x : char, StringComparison.Ordinal) """

                // Tuple
                "id (x, y)", "id (x, y)"
                "id (struct (x, y))", "id struct (x, y)"
                "id<struct (_ * _)> (x, y)", "id<struct (_ * _)> (x, y)"

                // We can't tell syntactically whether the method might have the signature
                //
                //     val TryGetValue : 'Key * outref<'Value> -> bool
                //
                // where 'Key is 'a * 'b, in which case the double parens are required.
                // We could look this up in the typed tree, but we don't currently.
                "x.TryGetValue((y, z))", "x.TryGetValue((y, z))"

                "valInfosForFslib.Force(g).TryGetValue((vref, vref.Deref.GetLinkageFullKey()))",
                "valInfosForFslib.Force(g).TryGetValue((vref, vref.Deref.GetLinkageFullKey()))"

                "SemanticClassificationItem((m, SemanticClassificationType.Printf))",
                "SemanticClassificationItem((m, SemanticClassificationType.Printf))"

                // AnonRecd
                "id ({||})", "id {||}"
                "{| A = (fun () -> ()) |}", "{| A = fun () -> () |}"
                "{| A = (fun () -> ()); B = 3 |}", "{| A = (fun () -> ()); B = 3 |}"
                "{| A = (let x = 3 in x); B = 3 |}", "{| A = (let x = 3 in x); B = 3 |}"
                "{| (try {||} with _ -> reraise ()) with A = 4 |}", "{| (try {||} with _ -> reraise ()) with A = 4 |}"
                "{| (x |> id) with A = 4 |}", "{| (x |> id) with A = 4 |}"
                "{| (box x :?> T) with A = 4 |}", "{| (box x :?> T) with A = 4 |}"
                "{| (+x) with A = 4 |}", "{| (+x) with A = 4 |}"
                "{| (!x) with A = 4 |}", "{| !x with A = 4 |}"
                "{| (! x) with A = 4 |}", "{| ! x with A = 4 |}"

                "
                {| A = (fun () -> ())
                   B = 3 |}
                ",
                "
                {| A = fun () -> ()
                   B = 3 |}
                "

                "
                {| A = (fun () -> ()); B = (fun () -> ())
                   C = 3 |}
                ",
                "
                {| A = (fun () -> ()); B = fun () -> ()
                   C = 3 |}
                "

                "
                {| A = (let x = 3 in x)
                   B = 3 |}
                ",
                "
                {| A = let x = 3 in x
                   B = 3 |}
                "

                "
                {| (try {||} with _ -> reraise ())
                    with
                    A = 4 |}
                ",
                "
                {| (try {||} with _ -> reraise ())
                    with
                    A = 4 |}
                "

                "
                {|
                    A = ([] : int list list)
                |}
                ",
                "
                {|
                    A = ([] : int list list)
                |}
                "

                // ArrayOrList
                "id ([])", "id []"
                "id ([||])", "id [||]"
                "(id([0]))[0]", "(id [0])[0]"
                "(id [0])[0]", "(id [0])[0]"

                // Record
                "id ({ A = x })", "id { A = x }"
                "{ A = (fun () -> ()) }", "{ A = fun () -> () }"
                "{ A = (fun () -> ()); B = 3 }", "{ A = (fun () -> ()); B = 3 }"
                "{ A = (let x = 3 in x); B = 3 }", "{ A = (let x = 3 in x); B = 3 }"
                "{ A.B.C.D.X = (match () with () -> ()); A.B.C.D.Y = 3 }", "{ A.B.C.D.X = (match () with () -> ()); A.B.C.D.Y = 3 }"
                "{ (try { A = 3 } with _ -> reraise ()) with A = 4 }", "{ (try { A = 3 } with _ -> reraise ()) with A = 4 }"
                "{ (x |> id) with A = 4 }", "{ (x |> id) with A = 4 }"
                "{ (box x :?> T) with A = 4 }", "{ (box x :?> T) with A = 4 }"
                "{ (+x) with A = 4 }", "{ (+x) with A = 4 }"
                "{ (!x) with A = 4 }", "{ !x with A = 4 }"
                "{ (! x) with A = 4 }", "{ ! x with A = 4 }"

                "
                { A = (fun () -> ())
                  B = 3 }
                ",
                "
                { A = fun () -> ()
                  B = 3 }
                "

                "
                { A = (let x = 3 in x)
                  B = 3 }
                ",
                "
                { A = let x = 3 in x
                  B = 3 }
                "

                "
                { A.B.C.D.X = (match () with () -> ())
                  A.B.C.D.Y = 3 }
                ",
                "
                { A.B.C.D.X = match () with () -> ()
                  A.B.C.D.Y = 3 }
                "

                "
                { (try { A = 3 } with _ -> reraise ())
                  with
                    A = 4 }
                ",
                "
                { (try { A = 3 } with _ -> reraise ())
                  with
                    A = 4 }
                "

                "
                {
                    A = ([] : int list list)
                }
                ",
                "
                {
                    A = ([] : int list list)
                }
                "

                // New
                "id (new obj())", "id (new obj())"

                // ObjExpr
                "id ({ new System.IDisposable with member _.Dispose() = () })", "id { new System.IDisposable with member _.Dispose() = () }"

                // While
                "id (while true do ())", "id (while true do ())"

                // ArrayOrListComputed
                "id ([x..y])", "id [x..y]"
                "id ([|x..y|])", "id [|x..y|]"
                """(id("x"))[0]""", """(id "x")[0]"""
                """(id "x")[0]""", """(id "x")[0]"""
                """id ("x")[0]""", """id ("x")[0]"""

                // ComputationExpr
                "id (seq { x })", "id (seq { x })"
                "id ({x..y})", "id {x..y}"
                "(async) { return x }", "async { return x }"
                "(id async) { return x }", "id async { return x }"

                // Lambda
                "id (fun x -> x)", "id (fun x -> x)"
                "x |> (fun x -> x)", "x |> fun x -> x"

                "
                x
                |> id
                |> id
                |> id
                |> id
                |> id
                |> (fun x -> x)
                ",
                "
                x
                |> id
                |> id
                |> id
                |> id
                |> id
                |> fun x -> x
                "

                "
                x
                |> id
                |> id
                |> id
                |> (fun x -> x)
                |> id
                |> id
                ",
                "
                x
                |> id
                |> id
                |> id
                |> (fun x -> x)
                |> id
                |> id
                "

                "x |> (id |> fun x -> x)", "x |> (id |> fun x -> x)"
                "x |> (id <| fun x -> x)", "x |> (id <| fun x -> x)"
                "id <| (fun x -> x)", "id <| fun x -> x"
                "id <| (fun x -> x) |> id", "id <| (fun x -> x) |> id"
                "id <| (id <| fun x -> x) |> id", "id <| (id <| fun x -> x) |> id"
                "id <| (id <| id <| id <| fun x -> x) |> id", "id <| (id <| id <| id <| fun x -> x) |> id"
                "(id <| fun x -> x) |> id", "(id <| fun x -> x) |> id"
                """(printfn ""; fun x -> x) |> id""", """(printfn ""; fun x -> x) |> id"""

                // MatchLambda
                "id (function x when true -> x | y -> y)", "id (function x when true -> x | y -> y)"
                "(id <| function x -> x) |> id", "(id <| function x -> x) |> id"

                // Match
                "id (match x with y -> y)", "id (match x with y -> y)"
                "(id <| match x with _ -> x) |> id", "(id <| match x with _ -> x) |> id"
                "id <| (match x with _ -> x) |> id", "id <| (match x with _ -> x) |> id"

                "
                match () with
                | _ when
                    (true &&
                     let x = 3
                     match x with
                     | 3 | _ -> true) -> ()
                | _ -> ()
                ",
                "
                match () with
                | _ when
                    (true &&
                     let x = 3
                     match x with
                     | 3 | _ -> true) -> ()
                | _ -> ()
                "

                "
                match () with
                | _ when
                    true &&
                    let x = 3
                    (match x with
                     | 3 | _ -> true) -> ()
                | _ -> ()
                ",
                "
                match () with
                | _ when
                    true &&
                    let x = 3
                    (match x with
                     | 3 | _ -> true) -> ()
                | _ -> ()
                "

                "
                match () with
                | _ when
                    true &&
                    let x = 3
                    (let y = false
                     match x with
                     | 3 | _ -> y) -> ()
                | _ -> ()
                ",
                "
                match () with
                | _ when
                    true &&
                    let x = 3
                    (let y = false
                     match x with
                     | 3 | _ -> y) -> ()
                | _ -> ()
                "

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
                "(int)x", "int x"
                "(uint32)x", "uint32 x"
                "(int)_x", "int _x"
                "(uint32)_x", "uint32 _x"
                "(f_)x", "f_ x"

                "~~~(-1)", "~~~ -1"
                "~~~(-x)", "~~~(-x)"
                "~~~(-(1))", "~~~(-1)"
                "id ~~~(-(x))", "id ~~~(-x)"
                "id ~~~(-x)", "id ~~~(-x)" // We could actually remove here, but probably best not to.
                "id (-(-x))", "id -(-x)"
                "id -(-x)", "id -(-x)"

                "f <| (g << h)", "f <| (g << h)"
                "x <> (y <> z)", "x <> (y <> z)"
                "x > (y > z)", "x > (y > z)"

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
                "(id <| let x = 1 in x) |> id", "(id <| let x = 1 in x) |> id"

                // TryWith
                "id (try raise null with _ -> null)", "id (try raise null with _ -> null)"
                "(id <| try raise null with _ -> null) |> id", "(id <| try raise null with _ -> null) |> id"

                // TryFinally
                "id (try raise null finally null)", "id (try raise null finally null)"
                "(id <| try raise null finally null) |> id", "(id <| try raise null finally null) |> id"

                // Lazy
                "id (lazy x)", "id (lazy x)"

                // Sequential
                "id ((); ())", "id ((); ())"
                "id (let x = 1; () in x)", "id (let x = 1; () in x)"
                "id (let x = 1 in (); y)", "id (let x = 1 in (); y)"

                // IfThenElse
                "id (if x then y else z)", "id (if x then y else z)"
                "(id <| if x then y else z) |> id", "(id <| if x then y else z) |> id"
                "id <| (if x then y else z) |> id", "id <| (if x then y else z) |> id"

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
                "let x = ref 3 in id (x.Value <- 3)", "let x = ref 3 in id (x.Value <- 3)"

                // DotGet
                "(-1).ToString()", "(-1).ToString()"
                "(-x).ToString()", "(-x).ToString()"
                "(~~~x).ToString()", "(~~~x).ToString()"
                "id (3L.ToString())", "id (3L.ToString())"
                """id ("x").Length""", """id "x".Length"""
                """(id("x")).Length""", """(id "x").Length"""
                """(id "x").Length""", """(id "x").Length"""
                """(3L.ToString("x")).Length""", """(3L.ToString "x").Length"""
                "~~TypedResults.Ok<string>(maybe.Value)", "~~TypedResults.Ok<string>(maybe.Value)"

                // DotLambda
                "[{| A = x |}] |> List.map (_.A)", "[{| A = x |}] |> List.map _.A"
                """[1..10] |> List.map _.ToString("x")""", """[1..10] |> List.map _.ToString("x")"""
                """[1..10] |> List.map _.ToString(("x"))""", """[1..10] |> List.map _.ToString("x")"""

                // DotSet
                "id ((ref x).Value <- y)", "id ((ref x).Value <- y)"
                "(ignore <| (ref x).Value <- y), 3", "(ignore <| (ref x).Value <- y), 3"

                // Set
                "let mutable x = y in id (x <- z)", "let mutable x = y in id (x <- z)"
                "let mutable x = y in (x <- z) |> id", "let mutable x = y in (x <- z) |> id"
                "let mutable x = y in ((); x <- z) |> id", "let mutable x = y in ((); x <- z) |> id"
                "let mutable x = y in (if true then x <- z) |> id", "let mutable x = y in (if true then x <- z) |> id"
                "M(x).N <- y", "M(x).N <- y"
                "A(x).B(x).M(x).N <- y", "A(x).B(x).M(x).N <- y"
                "A(x)(x)(x).N <- y", "A(x)(x)(x).N <- y"

                // DotIndexedGet
                "id ([x].[y])", "id [x].[y]"
                """id ("x").[0]""", """id "x".[0]"""
                """(id("x")).[0]""", """(id "x").[0]"""
                """(id "x").[0]""", """(id "x").[0]"""
                """id ("".ToCharArray().[0])""", """id ("".ToCharArray().[0])"""
                """id ("".ToCharArray()[0])""", """id ("".ToCharArray()[0])"""
                """id ("".ToCharArray()[0])""", """id ("".ToCharArray()[0])"""

                // DotIndexedSet
                "id ([|x|].[y] <- z)", "id ([|x|].[y] <- z)"
                """id ("".ToCharArray().[0] <- '0')""", """id ("".ToCharArray().[0] <- '0')"""
                """id ("".ToCharArray()[0] <- '0')""", """id ("".ToCharArray()[0] <- '0')"""

                // NamedIndexedPropertySet
                "let xs = [|0|] in id (xs.Item 0 <- 0)", "let xs = [|0|] in id (xs.Item 0 <- 0)"

                // DotNamedIndexedPropertySet
                "id ([|0|].Item 0 <- 0)", "id ([|0|].Item 0 <- 0)"

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

                "
                let f (_: byref<int>) = ()
                let mutable x = 0
                f (& x)
                ",
                "
                let f (_: byref<int>) = ()
                let mutable x = 0
                f (& x)
                "

                "
                let (~~) (x: byref<int>) = x <- -x
                let mutable x = 3
                ~~(&x)
                ",
                "
                let (~~) (x: byref<int>) = x <- -x
                let mutable x = 3
                ~~(&x)
                "

                // TraitCall
                "let inline f x = id (^a : (static member Parse : string -> ^a) x)",
                "let inline f x = id (^a : (static member Parse : string -> ^a) x)"

                // InterpolatedString
                """ id ($"{x}") """, """ id $"{x}" """

                // Miscellaneous
                "System.Threading.Tasks.Task.CompletedTask.ConfigureAwait((x = x))",
                "System.Threading.Tasks.Task.CompletedTask.ConfigureAwait((x = x))"

                "x.M(y).N((z = z))", "x.M(y).N((z = z))"

                """
                dprintn ("The local method '"+(String.concat "." (tenc@[tname]))+"'::'"+mdkey.Name+"' was referenced but not declared")
                """,
                """
                dprintn ("The local method '"+(String.concat "." (tenc@[tname]))+"'::'"+mdkey.Name+"' was referenced but not declared")
                """

                """
                ""+(Unchecked.defaultof<string>)+""
                """,
                """
                ""+(Unchecked.defaultof<string>)+""
                """
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

                """
                type E (message : string) =
                    inherit exn ($"{message}")
                """,
                """
                type E (message : string) =
                    inherit exn $"{message}"
                """

                "
                type E (message : string) =
                    inherit exn (message)
                ",
                "
                type E (message : string) =
                    inherit exn (message)
                "

                """
                type E =
                    inherit exn
                    val Message2 : string
                    new (str1, str2) = { inherit exn ($"{str1}"); Message2 = str2 }
                """,
                """
                type E =
                    inherit exn
                    val Message2 : string
                    new (str1, str2) = { inherit exn $"{str1}"; Message2 = str2 }
                """

                "
                type E =
                    inherit exn
                    val Message2 : string
                    new (str1, str2) = { inherit exn (str1); Message2 = str2 }
                ",
                "
                type E =
                    inherit exn
                    val Message2 : string
                    new (str1, str2) = { inherit exn (str1); Message2 = str2 }
                "

                "
                type T = static member M (x : bool, y) = ()
                let x = 3
                T.M ((x = 4), 5)
                ",
                "
                type T = static member M (x : bool, y) = ()
                let x = 3
                T.M ((x = 4), 5)
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

                /// Indicates whether both operators are the same exact symbolic operator.
                member this.Identical =
                    match this with
                    | OuterLeft(l, r)
                    | OuterRight(l, r) -> l = r

                override this.ToString() =
                    match this with
                    | OuterLeft(l, r) -> $"x {l} (y {r} z)"
                    | OuterRight(l, r) -> $"(x {l} y) {r} z"

            module ParenthesizedInfixOperatorAppPair =
                /// Reduces the operator strings to simpler, more easily identifiable forms.
                let simplify =
                    let ignoredLeadingChars = [| '.'; '?' |]

                    let simplify (s: string) =
                        let s = s.TrimStart ignoredLeadingChars

                        match s[0], s with
                        | '*', _ when s.Length > 1 && s[1] = '*' -> "**op"
                        | ':', _
                        | _, ("$" | "||" | "or" | "&" | "&&") -> s
                        | '!', _ -> "!=op"
                        | c, _ -> $"{c}op"

                    function
                    | OuterLeft(l, r) -> OuterLeft(simplify l, simplify r)
                    | OuterRight(l, r) -> OuterRight(simplify l, simplify r)

                /// Indicates that the pairing is syntactically invalid
                /// (for unoverloadable operators like :?, :>, :?>)
                /// and that we therefore need not test it.
                let invalidPairing = None

                let unfixable pair =
                    let expr = string pair
                    Some(expr, expr)

                let fixable pair =
                    let expr = string pair

                    let fix =
                        match pair with
                        | OuterLeft(l, r)
                        | OuterRight(l, r) -> $"x {l} y {r} z"

                    Some(expr, fix)

                let expectation pair =
                    match simplify pair with
                    | OuterLeft((":?" | ":>" | ":?>"), _) -> invalidPairing
                    | OuterLeft(_, "**op") -> fixable pair
                    | OuterLeft("**op", _) -> unfixable pair
                    | OuterLeft("*op", "*op") -> if pair.Identical then fixable pair else unfixable pair
                    | OuterLeft(("%op" | "/op" | "*op"), ("%op" | "/op" | "*op")) -> unfixable pair
                    | OuterLeft(_, ("%op" | "/op" | "*op")) -> fixable pair
                    | OuterLeft(("%op" | "/op" | "*op"), _) -> unfixable pair
                    | OuterLeft("+op", "+op") -> if pair.Identical then fixable pair else unfixable pair
                    | OuterLeft(("-op" | "+op"), ("-op" | "+op")) -> unfixable pair
                    | OuterLeft(_, ("-op" | "+op")) -> fixable pair
                    | OuterLeft(("-op" | "+op"), _) -> unfixable pair
                    | OuterLeft(_, ":?") -> fixable pair
                    | OuterLeft(_, "::") -> fixable pair
                    | OuterLeft("::", _) -> unfixable pair
                    | OuterLeft(_, ("^op" | "@op")) -> fixable pair
                    | OuterLeft(("^op" | "@op"), _) -> unfixable pair
                    | OuterLeft(("=op" | "|op" | "&op" | "$" | ">op" | "<op" | "!=op"),
                                ("=op" | "|op" | "&op" | "$" | ">op" | "<op" | "!=op")) -> unfixable pair
                    | OuterLeft(_, ("=op" | "|op" | "&op" | "$" | ">op" | "<op" | "!=op")) -> fixable pair
                    | OuterLeft(("=op" | "|op" | "&op" | "$" | ">op" | "<op" | "!=op"), _) -> unfixable pair
                    | OuterLeft(_, (":>" | ":?>")) -> fixable pair
                    | OuterLeft(("&" | "&&"), ("&" | "&&")) -> if pair.Identical then fixable pair else unfixable pair
                    | OuterLeft(_, ("&" | "&&")) -> fixable pair
                    | OuterLeft(("&" | "&&"), _) -> unfixable pair
                    | OuterLeft(("||" | "or"), ("||" | "or")) -> if pair.Identical then fixable pair else unfixable pair
                    | OuterLeft(_, ("||" | "or")) -> fixable pair
                    | OuterLeft(("||" | "or"), _) -> unfixable pair
                    | OuterLeft(":=", ":=") -> fixable pair

                    | OuterRight((":?" | ":>" | ":?>"), _) -> invalidPairing
                    | OuterRight(_, "**op") -> unfixable pair
                    | OuterRight("**op", _) -> fixable pair
                    | OuterRight(("%op" | "/op" | "*op"), _) -> fixable pair
                    | OuterRight(_, ("%op" | "/op" | "*op")) -> unfixable pair
                    | OuterRight(("-op" | "+op"), _) -> fixable pair
                    | OuterRight(_, ("-op" | "+op")) -> unfixable pair
                    | OuterRight(_, ":?") -> unfixable pair
                    | OuterRight("::", "::") -> unfixable pair
                    | OuterRight("::", _) -> fixable pair
                    | OuterRight(_, "::") -> unfixable pair
                    | OuterRight(("^op" | "@op"), ("^op" | "@op")) -> unfixable pair
                    | OuterRight(("^op" | "@op"), _) -> fixable pair
                    | OuterRight(_, ("^op" | "@op")) -> unfixable pair
                    | OuterRight(("=op" | "|op" | "&op" | "$" | ">op" | "<op" | "!=op"), _) -> fixable pair
                    | OuterRight(_, ("=op" | "|op" | "&op" | "$" | ">op" | "<op" | "!=op")) -> unfixable pair
                    | OuterRight(_, (":>" | ":?>")) -> unfixable pair
                    | OuterRight(("&" | "&&"), _) -> fixable pair
                    | OuterRight(_, ("&" | "&&")) -> unfixable pair
                    | OuterRight(("||" | "or"), _) -> fixable pair
                    | OuterRight(_, ("||" | "or")) -> unfixable pair
                    | OuterRight(":=", ":=") -> unfixable pair

                    | _ -> unfixable pair

            let operators =
                [
                    "**"
                    "***"
                    "*"
                    "*."
                    "/"
                    "%"
                    "-"
                    "+"
                    "++"
                    ":?"
                    "::"
                    "^^^"
                    "@"
                    "<"
                    ">"
                    ">:"
                    "="
                    "!="
                    "|||"
                    "&&&"
                    "$"
                    "|>"
                    "<|"
                    ":>"
                    ":?>"
                    "&&"
                    "&"
                    "||"
                    "or"
                    ":="
                ]

            let pairings =
                operators
                |> Seq.allPairs operators
                |> Seq.allPairs [ OuterLeft; OuterRight ]
                |> Seq.choose (fun (pair, (l, r)) -> ParenthesizedInfixOperatorAppPair.expectation (pair (l, r)))

            let affixableOpPattern =
                @" (\*\*|\*|/+|%+|\++|-+|@+|\^+|!=|<+|>+|&{3,}|\|{3,}|=+|\|>|<\|) "

            let leadingDots = "..."
            let leadingQuestionMarks = "???"
            let trailingChars = "!%&*+-./<>=?@^|~"

            let circumfixReplacementPattern =
                $" {leadingDots}{leadingQuestionMarks}$1{trailingChars} "

            let infixOperators = memberData { yield! pairings }

            let infixOperatorsWithLeadingAndTrailingChars =
                let circumfix expr =
                    Regex.Replace(expr, affixableOpPattern, circumfixReplacementPattern)

                memberData { for expr, expected in pairings -> circumfix expr, circumfix expected }

            [<Theory; MemberData(nameof infixOperators)>]
            let ``Infix operators`` expr expected = expectFix expr expected

            [<Theory; MemberData(nameof infixOperatorsWithLeadingAndTrailingChars)>]
            let ``Infix operators with leading and trailing chars`` expr expected = expectFix expr expected

    let failing =
        memberData {
            // See https://github.com/dotnet/fsharp/issues/16999
            """
            (x) < (printfn $"{y}"
                   y)
            """,
            """
            (x) < (printfn $"{y}"
                   y)
            """

            // See https://github.com/dotnet/fsharp/issues/16999
            """
            id (x) < (printfn $"{y}"
                      y)
            """,
            """
            id (x) < (printfn $"{y}"
                      y)
            """

            // See https://github.com/dotnet/fsharp/issues/16999
            """
            id (id (id (x))) < (printfn $"{y}"
                                y)
            """,
            """
            id (id (id (x))) < (printfn $"{y}"
                                y)
            """

            // See https://github.com/dotnet/fsharp/issues/16999
            """
            (x) <> z && x < (printfn $"{y}"
                             y)
            """,
            """
            (x) <> z && x < (printfn $"{y}"
                             y)
            """

            // See https://github.com/dotnet/fsharp/issues/16999
            """
            (x) < match y with
                  | Some y -> let y = y
                              y
                  | y)
            """,
            """
            (x) < match y with
                  | Some y -> let y = y
                              y
                  | y)
            """

            // See https://github.com/dotnet/fsharp/issues/16999
            """
            printfn "1"; printfn ("2"); (id <| match y with Some y -> let y = y
                                                                      y
                                                          | None -> 3)
            """,
            """
            printfn "1"; printfn ("2"); (id <| match y with Some y -> let y = y
                                                                      y
                                                          | None -> 3)
            """

            // See https://github.com/dotnet/fsharp/issues/16999
            """
            printfn ("1"
                        ); printfn "2"; (id <| match y with Some y -> let y = y
                                                                      y
                                                          | None -> 3)
            """,
            """
            printfn ("1"
                        ); printfn "2"; (id <| match y with Some y -> let y = y
                                                                      y
                                                          | None -> 3)
            """
        }

    [<Theory; MemberData(nameof failing)>]
    let ``Failing tests`` expr expected =
        Assert.ThrowsAsync<UnexpectedCodeFixException>(fun () -> expectFix expr expected)

module Patterns =
    type SynPat =
        | Const of string
        | Wild
        | Named of string
        | Typed of SynPat * string
        | Attrib of string * SynPat
        | Or of SynPat * SynPat
        | ListCons of SynPat * SynPat
        | Ands of SynPat list
        | As of SynPat * SynPat
        | LongIdent of string
        | LongIdentWithArgs of string * SynPat
        | LongIdentWithNamedArgs of string * (string * SynPat) list
        | Tuple of SynPat list
        | StructTuple of SynPat list
        | Paren of SynPat
        | List of SynPat list
        | Array of SynPat list
        | Record of (string * SynPat) list
        | Null
        | OptionalVal of SynPat
        | IsInst of string
        | QuoteExpr of string

    /// The original pattern.
    type OriginalPat = SynPat

    /// The pattern expected after the analyzer and code fix have been applied.
    type ExpectedPat = SynPat

    module SynPat =
        let (|DanglingTyped|_|) pat =
            let (|Last|) = List.last

            let rec loop pat =
                match pat with
                | Typed _ -> Some DanglingTyped
                | Or(_, pat)
                | ListCons(_, pat)
                | As(_, pat)
                | Ands(Last pat)
                | Tuple(Last pat) -> loop pat
                | _ -> None

            loop pat

        let (|Atomic|NonAtomic|) pat =
            match pat with
            | Const _
            | Wild
            | Named _
            | Null
            | LongIdent _
            | StructTuple _
            | List _
            | Array _
            | Paren _
            | Record _
            | QuoteExpr _ -> Atomic
            | Typed _
            | Attrib _
            | Or _
            | ListCons _
            | Ands _
            | As _
            | LongIdentWithArgs _
            | LongIdentWithNamedArgs _
            | Tuple _
            | OptionalVal _
            | IsInst _ -> NonAtomic

        /// Formats the given pattern to the given string builder.
        let fmt (sb: StringBuilder) pat =
            let spaceIfGt (sb: StringBuilder) =
                if sb.Chars(sb.Length - 1) = '>' then sb.Append ' ' else sb

            let rec loop (sb: StringBuilder) (cont: StringBuilder -> StringBuilder) =
                function
                | Const c -> cont (sb.Append c)
                | Wild -> cont (sb.Append "_")
                | Named name -> cont (sb.Append name)
                | Typed(pat, ty) -> loop sb (cont << fun sb -> sb.Append(" : ").Append ty) pat
                | Attrib(attrib, pat) -> loop (sb.Append("[<").Append(attrib).Append(">] ")) cont pat
                | Or(l, r) -> loop sb (fun sb -> loop (sb.Append " | ") cont r) l
                | ListCons(h, t) -> loop sb (fun sb -> loop (sb.Append " :: ") cont t) h
                | Ands pats -> separateBy " & " sb cont pats
                | As(l, r) -> loop sb (fun sb -> loop (sb.Append " as ") cont r) l
                | LongIdent id -> cont (sb.Append id)
                | LongIdentWithArgs(id, arg) -> loop (sb.Append(id).Append ' ') cont arg
                | LongIdentWithNamedArgs(id, args) ->
                    fmtFields (sb.Append(id).Append " (") (cont << fun (sb: StringBuilder) -> sb.Append ')') args
                | Tuple pats -> separateBy ", " sb cont pats
                | StructTuple pats -> separateBy ", " (sb.Append "struct (") (cont << fun sb -> sb.Append ')') pats
                | Paren pat -> loop (sb.Append '(') (cont << fun sb -> sb.Append ')') pat
                | List pats -> separateBy "; " (sb.Append '[') (cont << fun sb -> (spaceIfGt sb).Append ']') pats
                | Array pats -> separateBy "; " (sb.Append "[|") (cont << fun sb -> (spaceIfGt sb).Append "|]") pats
                | Record fields -> fmtFields (sb.Append "{ ") (cont << fun (sb: StringBuilder) -> sb.Append " }") fields
                | Null -> cont (sb.Append "null")
                | OptionalVal pat -> loop (sb.Append '?') cont pat
                | IsInst ty -> cont (sb.Append(":? ").Append ty)
                | QuoteExpr expr -> cont (sb.Append("<@ ").Append(expr).Append " @>")

            and separateBy separator sb cont =
                function
                | [] -> cont sb
                | pat :: [] -> loop sb cont pat
                | pat :: pats -> loop sb (fun sb -> separateBy separator (sb.Append separator) cont pats) pat

            and fmtFields sb cont =
                function
                | [] -> cont sb
                | (field, pat) :: [] -> loop (sb.Append(field).Append " = ") cont pat
                | (field, pat) :: pats -> loop (sb.Append(field).Append " = ") (fun sb -> fmtFields (sb.Append "; ") cont pats) pat

            loop sb id pat

        /// Returns pairings of the inner expression and nests it inside of the outer
        /// paired with the expected outcome of running the analyzer and code fix
        /// on the resulting code, i.e., either removal of the parentheses or not,
        /// depending on the pattern and its context.
        let parenthesize inner outer : (OriginalPat * ExpectedPat) list =
            let impl paren inner outer =
                let Paren = paren

                [
                    match outer, inner with
                    // Attributed patterns can never be nested.
                    | _, Attrib _ -> ()

                    | LongIdentWithArgs(name, _), Atomic -> LongIdentWithArgs(name, Paren inner), LongIdentWithArgs(name, inner)
                    | LongIdentWithArgs(name, _), NonAtomic -> LongIdentWithArgs(name, Paren inner), LongIdentWithArgs(name, Paren inner)

                    | LongIdentWithNamedArgs(name, args), _ ->
                        for i, (field, _) in Seq.indexed args do
                            LongIdentWithNamedArgs(name, args |> List.updateAt i (field, Paren inner)),
                            LongIdentWithNamedArgs(name, args |> List.updateAt i (field, inner))

                    // Quotations can never be nested in anything other than a LongIdent.
                    | _, QuoteExpr _ -> ()

                    | Record fields, Atomic ->
                        for i, (field, _) in Seq.indexed fields do
                            Record(fields |> List.updateAt i (field, Paren inner)), Record(fields |> List.updateAt i (field, inner))

                    | Typed(_, ty), (Atomic | Typed _ | IsInst _) -> Typed(Paren inner, ty), Typed(inner, ty)
                    | Typed(_, ty), NonAtomic -> Typed(Paren inner, ty), Typed(Paren inner, ty)
                    | Attrib(attrib, _), Atomic -> Attrib(attrib, Paren inner), Attrib(attrib, inner)
                    | OptionalVal _, _ -> ()

                    | ListCons _, Tuple _ -> ListCons(Paren inner, Wild), ListCons(Paren inner, Wild)
                    | ListCons _, ListCons _ ->
                        ListCons(Paren inner, Wild), ListCons(Paren inner, Wild)
                        ListCons(Wild, Paren inner), ListCons(Wild, inner)
                    | ListCons _, (Or _ | Ands _ | As _) ->
                        ListCons(Paren inner, Wild), ListCons(Paren inner, Wild)
                        ListCons(Wild, Paren inner), ListCons(Wild, Paren inner)
                    | ListCons _, _ ->
                        ListCons(Paren inner, Wild), ListCons(inner, Wild)
                        ListCons(Wild, Paren inner), ListCons(Wild, inner)

                    | As _, (Or _ | Ands _ | Tuple _ | ListCons _) ->
                        As(Paren inner, Wild), As(inner, Wild)
                        As(Wild, Paren inner), As(Wild, Paren inner)
                    | As _, _ ->
                        As(Paren inner, Wild), As(inner, Wild)
                        As(Wild, Paren inner), As(Wild, inner)

                    | Or _, As _ ->
                        Or(Paren inner, Wild), Or(Paren inner, Wild)
                        Or(Wild, Paren inner), Or(Wild, Paren inner)
                    | Or _, _ ->
                        Or(Paren inner, Wild), Or(inner, Wild)
                        Or(Wild, Paren inner), Or(Wild, inner)

                    | Ands pats, Typed _ ->
                        let last = pats.Length - 1

                        for i, _ in Seq.indexed pats do
                            if i = last then
                                Ands(pats |> List.updateAt i (Paren inner)), Ands(pats |> List.updateAt i inner)
                            else
                                Ands(pats |> List.updateAt i (Paren inner)), Ands(pats |> List.updateAt i (Paren inner))
                    | Ands pats, (Or _ | As _ | Tuple _) ->
                        for i, _ in Seq.indexed pats do
                            Ands(pats |> List.updateAt i (Paren inner)), Ands(pats |> List.updateAt i (Paren inner))
                    | Ands pats, _ ->
                        for i, _ in Seq.indexed pats do
                            Ands(pats |> List.updateAt i (Paren inner)), Ands(pats |> List.updateAt i inner)

                    | Tuple pats, (Or _ | As _ | Tuple _) ->
                        for i, _ in Seq.indexed pats do
                            Tuple(pats |> List.updateAt i (Paren inner)), Tuple(pats |> List.updateAt i (Paren inner))
                    | Tuple pats, _ ->
                        for i, _ in Seq.indexed pats do
                            Tuple(pats |> List.updateAt i (Paren inner)), Tuple(pats |> List.updateAt i inner)

                    | StructTuple pats, (Or _ | As _ | Tuple _) ->
                        for i, _ in Seq.indexed pats do
                            StructTuple(pats |> List.updateAt i (Paren inner)), StructTuple(pats |> List.updateAt i (Paren inner))
                    | StructTuple pats, _ ->
                        for i, _ in Seq.indexed pats do
                            StructTuple(pats |> List.updateAt i (Paren inner)), StructTuple(pats |> List.updateAt i inner)

                    | List pats, _ ->
                        for i, _ in Seq.indexed pats do
                            List(pats |> List.updateAt i (Paren inner)), List(pats |> List.updateAt i inner)

                    | Array pats, _ ->
                        for i, _ in Seq.indexed pats do
                            Array(pats |> List.updateAt i (Paren inner)), Array(pats |> List.updateAt i inner)

                    | _ -> ()
                ]

            // We want to test both single () and double (())
            // in every context.
            match inner with
            | Const "()" -> [ yield! impl id inner outer; yield! impl Paren inner outer ]
            | _ -> impl Paren inner outer

    let atomicOrNullaryPatterns =
        [
            Const "()"
            Const "3"
            Wild
            Named "x"
            LongIdent "C"
            StructTuple [ Wild; Wild ]
            List []
            Array []
            Record [ "Z", Wild ]
            Null
            QuoteExpr "3"
            IsInst(nameof obj)
        ]

    let nonNullaryPatterns =
        [
            Typed(Wild, nameof obj)
            Attrib("OptionalArgument", Wild)
            Or(Wild, Wild)
            ListCons(Wild, Wild)
            Ands [ Wild; Wild ]
            As(Wild, Wild)
            LongIdentWithArgs("A", Wild)
            LongIdentWithNamedArgs("B", [ "x", Wild ])
            Tuple [ Wild; Wild ]
            StructTuple [ Wild; Wild ]
            Paren Wild
            List [ Wild; Wild ]
            Array [ Wild; Wild ]
            Record [ "X", Wild; "Y", Wild ]
            OptionalVal(Named "y")
        ]

    let patterns = atomicOrNullaryPatterns @ nonNullaryPatterns |> List.distinct

    let bareAtomics: (OriginalPat * ExpectedPat) list =
        [
            for pat in
                patterns
                |> Seq.filter (function
                    | SynPat.QuoteExpr _
                    | SynPat.NonAtomic -> false
                    | _ -> true) do
                match pat with
                | Const "()" ->
                    pat, pat
                    Paren pat, pat
                | _ -> Paren pat, pat
        ]

    let nestedAtomicsOrNullaries: (OriginalPat * ExpectedPat) list =
        [
            for outer, inner in (nonNullaryPatterns, atomicOrNullaryPatterns) ||> Seq.allPairs do
                yield! SynPat.parenthesize inner outer
        ]

    let nestedOuters: (OriginalPat * ExpectedPat) list =
        [
            let nonAtomics =
                nestedAtomicsOrNullaries
                |> Seq.map snd
                |> Seq.filter (function
                    | SynPat.NonAtomic -> true
                    | _ -> false)

            for outer, inner in (nonNullaryPatterns, nonAtomics) ||> Seq.allPairs |> Seq.distinct do
                yield! SynPat.parenthesize inner outer
        ]

    /// Formats the given sequence of original and
    /// expected pattern pairs and returns them as
    /// an obj array seq suitable for use as xUnit member data.
    let fmtAllAsMemberData pairs =
        memberData {
            let sb = StringBuilder()

            for original, expected in pairs ->
                (let original = string (SynPat.fmt sb original) in
                 ignore <| sb.Clear()
                 original),
                (let expected = string (SynPat.fmt sb expected) in
                 ignore <| sb.Clear()
                 expected)
        }

    /// Tests patterns in head-pattern position in (let|and|use)(!) bindings.
    module HeadPat =
        /// Tests patterns in head-pattern position in let-bindings.
        module Let =
            /// let pat = …
            let expectFix pat expected =
                let code = $"let %s{pat} = Unchecked.defaultof<_>"
                let expected = $"let %s{expected} = Unchecked.defaultof<_>"

                expectFix code expected

            let bareAtomics = fmtAllAsMemberData bareAtomics

            let bareNonAtomics =
                patterns
                |> List.collect (function
                    | SynPat.Attrib _
                    | SynPat.OptionalVal _
                    | SynPat.IsInst _
                    | SynPat.Atomic -> []
                    | SynPat.Typed _ as pat -> [ SynPat.Paren pat, pat ]
                    | SynPat.Tuple pats as pat ->
                        [
                            SynPat.Paren pat, pat
                            SynPat.Paren(SynPat.Tuple(SynPat.Typed(Wild, "obj") :: pats)),
                            SynPat.Paren(SynPat.Tuple(SynPat.Typed(Wild, "obj") :: pats))
                            SynPat.Tuple(SynPat.Paren(SynPat.Typed(Wild, "obj")) :: pats),
                            SynPat.Tuple(SynPat.Paren(SynPat.Typed(Wild, "obj")) :: pats)
                        ]
                    | SynPat.LongIdentWithArgs _
                    | SynPat.DanglingTyped as pat -> [ SynPat.Paren pat, SynPat.Paren pat ]
                    | pat -> [ SynPat.Paren pat, pat ])
                |> fmtAllAsMemberData

            [<Theory; MemberData(nameof bareAtomics)>]
            let ``Bare atomic patterns`` original expected = expectFix original expected

            [<Theory; MemberData(nameof bareNonAtomics)>]
            let ``Bare non-atomic patterns`` original expected = expectFix original expected

        /// Tests patterns in head-pattern position in let!-bindings.
        module LetBang =
            /// let! pat = …
            let expectFix pat expected =
                let code =
                    $"
                    async {{
                        let! %s{pat} = Unchecked.defaultof<_>
                        return ()
                    }}
                    "

                let expected =
                    $"
                    async {{
                        let! %s{expected} = Unchecked.defaultof<_>
                        return ()
                    }}
                    "

                expectFix code expected

            let bareAtomics = fmtAllAsMemberData bareAtomics

            let bareNonAtomics =
                patterns
                |> List.choose (function
                    | SynPat.Attrib _
                    | SynPat.OptionalVal _
                    | SynPat.Atomic -> None
                    | SynPat.DanglingTyped as pat -> Some(SynPat.Paren pat, SynPat.Paren pat)
                    | pat -> Some(SynPat.Paren pat, pat))
                |> fmtAllAsMemberData

            [<Theory; MemberData(nameof bareAtomics)>]
            let ``Bare atomic patterns`` original expected = expectFix original expected

            [<Theory; MemberData(nameof bareNonAtomics)>]
            let ``Bare non-atomic patterns`` original expected = expectFix original expected

    /// Tests patterns in argument position.
    module ArgPat =
        /// Tests patterns in argument position in let-bound functions.
        module Let =
            /// let f pat = …
            let expectFix pat expected =
                let code = $"let f %s{pat} = Unchecked.defaultof<_>"
                let expected = $"let f %s{expected} = Unchecked.defaultof<_>"

                expectFix code expected

            let bareAtomics =
                bareAtomics
                |> List.map (function
                    // We can't actually reliably remove the extra parens in argument patterns,
                    // since they affect compilation.
                    // See https://github.com/dotnet/fsharp/issues/17611, https://github.com/dotnet/fsharp/issues/16254, etc.
                    | SynPat.Paren(SynPat.Const "()") as doubleParen, _ -> doubleParen, doubleParen
                    | pats -> pats)
                |> fmtAllAsMemberData

            let bareNonAtomics =
                patterns
                |> List.choose (function
                    | SynPat.OptionalVal _
                    | SynPat.Atomic -> None
                    | SynPat.NonAtomic as pat -> Some(SynPat.Paren pat, SynPat.Paren pat))
                |> fmtAllAsMemberData

            [<Theory; MemberData(nameof bareAtomics)>]
            let ``Bare atomic patterns`` original expected = expectFix original expected

            [<Theory; MemberData(nameof bareNonAtomics)>]
            let ``Bare non-atomic patterns`` original expected = expectFix original expected

        /// Tests patterns in argument position in object members.
        module Member =
            /// member _.M pat = …
            let expectFix pat expected =
                let code = $"type T () = member _.M %s{pat} = Unchecked.defaultof<_>"
                let expected = $"type T () = member _.M %s{expected} = Unchecked.defaultof<_>"
                expectFix code expected

            let bareAtomics =
                bareAtomics
                |> List.map (function
                    // We can't actually reliably remove the extra parens in argument patterns,
                    // since they affect compilation.
                    // See https://github.com/dotnet/fsharp/issues/17611, https://github.com/dotnet/fsharp/issues/16254, etc.
                    | SynPat.Paren(SynPat.Const "()") as doubleParen, _ -> doubleParen, doubleParen
                    | pats -> pats)
                |> fmtAllAsMemberData

            let bareNonAtomics =
                patterns
                |> List.choose (function
                    | SynPat.Atomic -> None
                    | SynPat.OptionalVal _ as pat -> Some(SynPat.Paren pat, pat)
                    | SynPat.NonAtomic as pat -> Some(SynPat.Paren pat, SynPat.Paren pat))
                |> fmtAllAsMemberData

            [<Theory; MemberData(nameof bareAtomics)>]
            let ``Bare atomic patterns`` original expected = expectFix original expected

            [<Theory; MemberData(nameof bareNonAtomics)>]
            let ``Bare non-atomic patterns`` original expected = expectFix original expected

        /// Tests patterns in argument position in lambda expressions.
        module Lambda =
            /// fun pat -> …
            let expectFix pat expected =
                let code = $"fun %s{pat} -> ()"
                let expected = $"fun %s{expected} -> ()"

                expectFix code expected

            let bareAtomics = fmtAllAsMemberData bareAtomics

            let bareNonAtomics =
                patterns
                |> List.collect (function
                    | SynPat.Attrib _
                    | SynPat.OptionalVal _
                    | SynPat.IsInst _
                    | SynPat.Atomic -> []
                    | pat -> [ SynPat.Paren pat, SynPat.Paren pat ])
                |> fmtAllAsMemberData

            [<Theory; MemberData(nameof bareAtomics)>]
            let ``Bare atomic patterns`` original expected = expectFix original expected

            [<Theory; MemberData(nameof bareNonAtomics)>]
            let ``Bare non-atomic patterns`` original expected = expectFix original expected

    /// Tests patterns in match clauses.
    module MatchClause =
        /// match … with pat -> …
        let expectFix pat expected =
            let code =
                $"
                match Unchecked.defaultof<_> with
                | %s{pat} ->
                    ()
                | _ -> ()
                "

            let expected =
                $"
                match Unchecked.defaultof<_> with
                | %s{expected} ->
                    ()
                | _ -> ()
                "

            expectFix code expected

        let bareAtomics = fmtAllAsMemberData bareAtomics

        let bareNonAtomics =
            patterns
            |> List.collect (function
                | SynPat.Attrib _
                | SynPat.OptionalVal _
                | SynPat.Atomic -> []
                | SynPat.Tuple pats as pat ->
                    [
                        SynPat.Paren pat, pat
                        SynPat.Paren(SynPat.Tuple(SynPat.Typed(Wild, "obj") :: pats)), SynPat.Tuple(SynPat.Typed(Wild, "obj") :: pats)
                        SynPat.Tuple(SynPat.Paren(SynPat.Typed(Wild, "obj")) :: pats), SynPat.Tuple(SynPat.Typed(Wild, "obj") :: pats)
                        SynPat.Paren(SynPat.Tuple(List.rev (SynPat.Typed(Wild, "obj") :: pats))),
                        SynPat.Paren(SynPat.Tuple(List.rev (SynPat.Typed(Wild, "obj") :: pats)))
                        SynPat.Tuple(List.rev (SynPat.Paren(SynPat.Typed(Wild, "obj")) :: pats)),
                        SynPat.Tuple(List.rev (SynPat.Paren(SynPat.Typed(Wild, "obj")) :: pats))
                    ]
                | SynPat.DanglingTyped as pat -> [ SynPat.Paren pat, SynPat.Paren pat ]
                | pat -> [ SynPat.Paren pat, pat ])
            |> fmtAllAsMemberData

        [<Theory; MemberData(nameof bareAtomics)>]
        let ``Bare atomic patterns`` original expected = expectFix original expected

        [<Theory; MemberData(nameof bareNonAtomics)>]
        let ``Bare non-atomic patterns`` original expected = expectFix original expected

    let expectFix original expected =
        let code =
            $"
            match Unchecked.defaultof<_> with
            |
                %s{original}
                | _ -> ()
            "

        let expected =
            $"
            match Unchecked.defaultof<_> with
            |
                %s{expected}
                | _ -> ()
            "

        TopLevel.expectFix code expected

    let singlyNestedAtomicOrNullaryPatterns =
        fmtAllAsMemberData nestedAtomicsOrNullaries

    let deeplyNestedPatterns = fmtAllAsMemberData nestedOuters

    /// Tests atomic or nullary patterns nested one level deep inside of all non-nullary patterns.
    [<Theory; MemberData(nameof singlyNestedAtomicOrNullaryPatterns)>]
    let ``Singly nested patterns`` original expected = expectFix original expected

    /// Tests non-nullary, non-atomic patterns nested inside of other non-nullary patterns.
    [<Theory; MemberData(nameof deeplyNestedPatterns)>]
    let ``Deeply nested patterns`` original expected = expectFix original expected

    let miscellaneous =
        memberData {
            // See https://github.com/dotnet/fsharp/issues/16254.
            "
            type C = abstract M : unit -> unit
            let _ = { new C with override _.M (()) = () }
            ",
            "
            type C = abstract M : unit -> unit
            let _ = { new C with override _.M (()) = () }
            "

            // See https://github.com/dotnet/fsharp/issues/16254.
            "
            type C<'T> = abstract M : 'T -> unit
            let _ = { new C<unit> with override _.M (()) = () }
            ",
            "
            type C<'T> = abstract M : 'T -> unit
            let _ = { new C<unit> with override _.M (()) = () }
            "

            // See https://github.com/dotnet/fsharp/issues/16254.
            "
            type C<'T> = abstract M : 'T -> unit
            type T () = interface C<unit> () with override _.M (()) = ()
            ",
            "
            type C<'T> = abstract M : 'T -> unit
            type T () = interface C<unit> () with override _.M (()) = ()
            "

            // See https://github.com/dotnet/fsharp/issues/16254.
            "
            type [<AbstractClass>] C<'T> () = abstract M : 'T -> unit
            type T () = inherit C<unit> () with override _.M (()) = ()
            ",
            "
            type [<AbstractClass>] C<'T> () = abstract M : 'T -> unit
            type T () = inherit C<unit> () with override _.M (()) = ()
            "

            // See https://github.com/dotnet/fsharp/issues/16257.
            "
            type T (x, y) =
                new (x, y, z) = T (x, y)
                new (x) = T (x, 3)
                member _.Z = x + y
            ",
            "
            type T (x, y) =
                new (x, y, z) = T (x, y)
                new x = T (x, 3)
                member _.Z = x + y
            "

            // See https://github.com/dotnet/fsharp/issues/16257.
            "
            let f (x: string) = int x
            type T (x, y) =
                new (x) = T (f x, 3)
                new x = T (id x, 3)
                member _.Z = x + y
            ",
            "
            let f (x: string) = int x
            type T (x, y) =
                new (x) = T (f x, 3)
                new x = T (id x, 3)
                member _.Z = x + y
            "

            // See https://github.com/dotnet/fsharp/issues/16257.
            "
            type T (x, y) =
                new (x) = T (x, 3)
                new (x, y, z) = T (x, y)
                member _.Z = x + y
            ",
            "
            type T (x, y) =
                new (x) = T (x, 3)
                new (x, y, z) = T (x, y)
                member _.Z = x + y
            "

            // The parens could be required by a signature file like this:
            //
            //     type SemanticClassificationItem =
            //         val Range: range
            //         val Type: SemanticClassificationType
            //         new: (range * SemanticClassificationType) -> SemanticClassificationItem
            "
            type SemanticClassificationItem =
                val Range: range
                val Type: SemanticClassificationType
                new((range, ty)) = { Range = range; Type = ty }
            ",
            "
            type SemanticClassificationItem =
                val Range: range
                val Type: SemanticClassificationType
                new((range, ty)) = { Range = range; Type = ty }
            "

            "
            match 1, 2 with
            | _, (1 | 2 as x) -> ()
            | _ -> ()
            ",
            "
            match 1, 2 with
            | _, (1 | 2 as x) -> ()
            | _ -> ()
            "

            "
            match 1, [2] with
            | _, (1 | 2 as x :: _) -> ()
            | _ -> ()
            ",
            "
            match 1, [2] with
            | _, (1 | 2 as x :: _) -> ()
            | _ -> ()
            "

            "
            match 1, [2] with
            | _, (1 as x :: _ :: _) -> ()
            | _ -> ()
            ",
            "
            match 1, [2] with
            | _, (1 as x :: _ :: _) -> ()
            | _ -> ()
            "

            "
            match maybe with
            | Some(x) -> let y = x * 2
                         let z = 99
                         x + y + z
            | None -> 3
            ",
            "
            match maybe with
            | Some(x) -> let y = x * 2
                         let z = 99
                         x + y + z
            | None -> 3
            "

            "
            match maybe with
            | Some(x) -> id <| (let y = x * 2
                                let z = 99
                                x + y + z)
            | None -> 3
            ",
            "
            match maybe with
            | Some(x) -> id <| (let y = x * 2
                                let z = 99
                                x + y + z)
            | None -> 3
            "

            "
            match maybe with
            | Some(
                    x
                  ) -> let y = x * 2
                       let z = 99
                       x + y + z
            | None -> 3
            ",
            "
            match maybe with
            | Some(
                    x
                  ) -> let y = x * 2
                       let z = 99
                       x + y + z
            | None -> 3
            "

            "
            match q with
            | { A = Some(
                         x
                        ) } -> let y = x * 2
                               let z = 99
                               x + y + z
            | { A = None } -> 3
            ",
            "
            match q with
            | { A = Some(
                         x
                        ) } -> let y = x * 2
                               let z = 99
                               x + y + z
            | { A = None } -> 3
            "

            // This removal is somewhat ugly, albeit valid.
            // Maybe we can make it nicer someday.
            "
            match q with
            | { A = Some (
                           x
                         )
              } -> let y = x * 2
                   let z = 99
                   x + y + z
            | { A = None } -> 3
            ",
            "
            match q with
            | { A = Some 
                           x
              } -> let y = x * 2
                   let z = 99
                   x + y + z
            | { A = None } -> 3
            "

            "
            match q with
            | { A = Some (x)
              } -> let y = x * 2
                   let z = 99
                   x + y + z
            | { A = None } -> 3
            ",
            "
            match q with
            | { A = Some x
              } -> let y = x * 2
                   let z = 99
                   x + y + z
            | { A = None } -> 3
            "

            "
            type T () =
                member this.Item
                    with get (y : int) = 3
                    and set (x : int) (y : int) = ignore (x, y)
            ",
            "
            type T () =
                member this.Item
                    with get (y : int) = 3
                    and set (x : int) (y : int) = ignore (x, y)
            "

            "
            let _ =
                { new IEquatable<int * int * int> with
                    member this.GetHashCode ((x, y, z)) = x + y + z
                    member this.Equals ((x, y, z), (x', y', z')) = false }
            ",
            "
            let _ =
                { new IEquatable<int * int * int> with
                    member this.GetHashCode ((x, y, z)) = x + y + z
                    member this.Equals ((x, y, z), (x', y', z')) = false }
            "
        }

    [<Theory; MemberData(nameof miscellaneous)>]
    let ``Miscellaneous patterns`` original expected = TopLevel.expectFix original expected
