namespace CompilerDirectives

open Xunit
open FSharp.Test.Compiler

module Ifdef =

    let ifdefSource = """
[<EntryPoint>]
let main _ =
    #if MYDEFINE1
    1
    #else
    2
    #endif
"""

    [<InlineData("MYDEFINE1", 1)>]
    [<InlineData("MYDEFINE", 2)>]
    [<Theory>]
    let ifdefTest (mydefine, expectedExitCode) =

        FSharp ifdefSource
        |> withDefines [mydefine]
        |> compileExeAndRun
        |> withExitCode expectedExitCode


    let sourceExtraEndif = """
#if MYDEFINE1
printf "1"
#endif
(**)#endif(**)
0
"""

    [<Fact>]
    let extraEndif () =

        FSharp sourceExtraEndif
        |> withDefines ["MYDEFINE1"]
        |> asExe
        |> compile
        |> withDiagnosticMessage "#endif has no matching #if in implementation file"

    let sourceUnknownHash = """
module A
#ifxx
#abc
"""

    [<Fact>]
    let unknownHashDirectiveIsIgnored () =

        FSharp sourceUnknownHash
        |> compile
        |> shouldSucceed

    // ===== #elif Tests =====

    let elifSource =
        """
[<EntryPoint>]
let main _ =
    #if BRANCH_A
    1
    #elif BRANCH_B
    2
    #else
    3
    #endif
"""

    [<InlineData("BRANCH_A", 1)>]
    [<InlineData("BRANCH_B", 2)>]
    [<InlineData("OTHER", 3)>]
    [<Theory>]
    let elifTest (mydefine, expectedExitCode) =
        FSharp elifSource
        |> withDefines [ mydefine ]
        |> withLangVersion "11.0"
        |> compileExeAndRun
        |> withExitCode expectedExitCode

    let elifMultipleSource =
        """
[<EntryPoint>]
let main _ =
    #if A
    1
    #elif B
    2
    #elif C
    3
    #else
    4
    #endif
"""

    [<InlineData("A", 1)>]
    [<InlineData("B", 2)>]
    [<InlineData("C", 3)>]
    [<InlineData("OTHER", 4)>]
    [<Theory>]
    let elifMultipleTest (mydefine, expectedExitCode) =
        FSharp elifMultipleSource
        |> withDefines [ mydefine ]
        |> withLangVersion "11.0"
        |> compileExeAndRun
        |> withExitCode expectedExitCode

    // When #if is true, all #elif should be skipped even if their condition is also true
    let elifFirstMatchWins =
        """
[<EntryPoint>]
let main _ =
    #if A
    1
    #elif A
    2
    #else
    3
    #endif
"""

    [<Fact>]
    let elifFirstMatchWinsTest () =
        FSharp elifFirstMatchWins
        |> withDefines [ "A" ]
        |> withLangVersion "11.0"
        |> compileExeAndRun
        |> withExitCode 1

    // #elif without #else
    let elifNoElse =
        """
[<EntryPoint>]
let main _ =
    let x =
        #if A
        1
        #elif B
        2
        #elif C
        3
        #endif
    x
"""

    [<InlineData("A", 1)>]
    [<InlineData("B", 2)>]
    [<InlineData("C", 3)>]
    [<Theory>]
    let elifNoElseTest (mydefine, expectedExitCode) =
        FSharp elifNoElse
        |> withDefines [ mydefine ]
        |> withLangVersion "11.0"
        |> compileExeAndRun
        |> withExitCode expectedExitCode

    // #elif with complex expression
    let elifComplexExpr =
        """
[<EntryPoint>]
let main _ =
    #if A && B
    1
    #elif A || C
    2
    #elif !A
    3
    #else
    4
    #endif
"""

    [<Fact>]
    let elifComplexExprTest () =
        FSharp elifComplexExpr
        |> withDefines [ "C" ]
        |> withLangVersion "11.0"
        |> compileExeAndRun
        |> withExitCode 2

    // Nested #elif
    let elifNested =
        """
[<EntryPoint>]
let main _ =
    #if OUTER
      #if INNER
      1
      #elif INNER2
      2
      #else
      3
      #endif
    #else
    4
    #endif
"""

    [<Fact>]
    let elifNestedTest () =
        FSharp elifNested
        |> withDefines [ "OUTER"; "INNER2" ]
        |> withLangVersion "11.0"
        |> compileExeAndRun
        |> withExitCode 2

    // Error: #elif after #else
    let elifAfterElse =
        """
module A
#if X
1
#else
2
#elif Y
3
#endif
"""

    [<Fact>]
    let elifAfterElseError () =
        FSharp elifAfterElse
        |> withLangVersion "11.0"
        |> compile
        |> withDiagnosticMessageMatches "#elif is not allowed after #else"

    // Error: #elif without matching #if
    let elifNoMatchingIf =
        """
module A
#elif X
1
#endif
"""

    [<Fact>]
    let elifNoMatchingIfError () =
        FSharp elifNoMatchingIf
        |> withLangVersion "11.0"
        |> compile
        |> withDiagnosticMessageMatches "#elif has no matching #if"

    // Language version gating: #elif should error with older langversion
    let elifLangVersionSource =
        """
module A
let x =
    #if DEBUG
    1
    #elif RELEASE
    2
    #else
    3
    #endif
"""

    [<Fact>]
    let elifLangVersionError () =
        FSharp elifLangVersionSource
        |> withLangVersion "10.0"
        |> compile
        |> withDiagnosticMessageMatches "#elif preprocessor directive"

    // Language version gating for nested-inactive #elif (n > 0 path in ifdefSkip)
    let elifNestedLangVersionSource =
        """
module A
let x =
    #if UNDEFINED_OUTER
    #if UNDEFINED_INNER
    1
    #elif SOMETHING
    2
    #endif
    #else
    3
    #endif
"""

    [<Fact>]
    let elifNestedLangVersionError () =
        FSharp elifNestedLangVersionSource
        |> withLangVersion "10.0"
        |> compile
        |> withDiagnosticMessageMatches "#elif preprocessor directive"

    // Error FS3882: #elif not at first column
    let elifMustBeFirst =
        """
module A
#if DEFINED
let x = 1; #elif OTHER
let y = 2
#endif
"""

    [<Fact>]
    let elifMustBeFirstWarning () =
        FSharp elifMustBeFirst
        |> withDefines [ "DEFINED" ]
        |> withLangVersion "11.0"
        |> compile
        |> withErrorCode 3882
        |> withDiagnosticMessageMatches "#elif directive must appear as the first non-whitespace character on a line"

    // Error FS3883: bare #elif without expression
    let elifMustHaveIdent =
        """
module A
#elif
let y = 2
"""

    [<Fact>]
    let elifMustHaveIdentError () =
        FSharp elifMustHaveIdent
        |> withLangVersion "11.0"
        |> compile
        |> withErrorCode 3883
        |> withDiagnosticMessageMatches "#elif directive should be immediately followed by an identifier"

    // All branches false, no #else fallback
    let elifAllFalseNoElse =
        """
[<EntryPoint>]
let main _ =
    let mutable x = 0
    #if BRANCH_A
    x <- 1
    #elif BRANCH_B
    x <- 2
    #elif BRANCH_C
    x <- 3
    #endif
    x
"""

    [<Fact>]
    let elifAllFalseNoElseTest () =
        FSharp elifAllFalseNoElse
        |> withDefines [ "OTHER" ]
        |> withLangVersion "11.0"
        |> compileExeAndRun
        |> withExitCode 0

    // B1: When #elif branch is taken (IfDefElif on stack), #else must be skipped.
    // Tests lex.fsl ifdefSkip: (IfDefElif,_) :: rest -> #else stays inactive.
    let elifTakenThenElseSkippedSource =
        """
[<EntryPoint>]
let main _ =
    #if UNDEF
    1
    #elif BRANCH
    2
    #else
    3
    #endif
"""

    [<Fact>]
    let elifTakenThenElseSkipped () =
        FSharp elifTakenThenElseSkippedSource
        |> withDefines [ "BRANCH" ]
        |> withLangVersion "11.0"
        |> compileExeAndRun
        |> withExitCode 2

    // B2: When #elif branch is taken (IfDefElif on stack), subsequent #elif is skipped
    // even if its condition is also true. Tests ifdefSkip: (IfDefElif,_) :: _rest path.
    let elifSecondElifAfterTakenSkippedSource =
        """
[<EntryPoint>]
let main _ =
    #if UNDEF
    1
    #elif A
    2
    #elif A
    3
    #else
    4
    #endif
"""

    [<Fact>]
    let elifSecondElifAfterTakenSkipped () =
        FSharp elifSecondElifAfterTakenSkippedSource
        |> withDefines [ "A" ]
        |> withLangVersion "11.0"
        |> compileExeAndRun
        |> withExitCode 2

    // B3: #elif after #else in skip context (ifdefSkip path, not token rule).
    // When #if is true, #else enters ifdefSkip; #elif inside that skip must error.
    let elifAfterElseInSkipContextSource =
        """
module A
#if DEFINED
let x = 1
#else
let x = 2
#elif Y
let x = 3
#endif
"""

    [<Fact>]
    let elifAfterElseErrorInSkipContext () =
        FSharp elifAfterElseInSkipContextSource
        |> withDefines [ "DEFINED" ]
        |> withLangVersion "11.0"
        |> compile
        |> withDiagnosticMessageMatches "#elif is not allowed after #else"