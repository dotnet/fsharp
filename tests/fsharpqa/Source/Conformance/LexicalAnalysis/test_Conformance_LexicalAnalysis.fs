module ``FSharpQA-Tests-Conformance-LexicalAnalysis``

open NUnit.Framework

open NUnitConf
open RunPlTest

module Comments =

    [<Test; FSharpQASuiteTest("Conformance/LexicalAnalysis/Comments")>]
    let Comments () = runpl |> check


module ConditionalCompilation =

    [<Test; FSharpQASuiteTest("Conformance/LexicalAnalysis/ConditionalCompilation")>]
    let ConditionalCompilation () = runpl |> check


module Directives =

    [<Test; FSharpQASuiteTest("Conformance/LexicalAnalysis/Directives")>]
    let Directives () = runpl |> check


module HiddenTokens =

    [<Test; FSharpQASuiteTest("Conformance/LexicalAnalysis/HiddenTokens")>]
    let HiddenTokens () = runpl |> check


module IdentifierReplacements =

    [<Test; FSharpQASuiteTest("Conformance/LexicalAnalysis/IdentifierReplacements")>]
    let IdentifierReplacements () = runpl |> check


module IdentifiersAndKeywords =

    [<Test; FSharpQASuiteTest("Conformance/LexicalAnalysis/IdentifiersAndKeywords")>]
    let IdentifiersAndKeywords () = runpl |> check


module LineDirectives =

    [<Test; FSharpQASuiteTest("Conformance/LexicalAnalysis/LineDirectives")>]
    let LineDirectives () = runpl |> check


module NumericLiterals =

    [<Test; FSharpQASuiteTest("Conformance/LexicalAnalysis/NumericLiterals")>]
    let NumericLiterals () = runpl |> check


module Shift =

    [<Test; FSharpQASuiteTest("Conformance/LexicalAnalysis/Shift/Generics")>]
    let Generics () = runpl |> check


module StringsAndCharacters =

    [<Test; FSharpQASuiteTest("Conformance/LexicalAnalysis/StringsAndCharacters")>]
    let StringsAndCharacters () = runpl |> check


module SymbolicKeywords =

    [<Test; FSharpQASuiteTest("Conformance/LexicalAnalysis/SymbolicKeywords")>]
    let SymbolicKeywords () = runpl |> check


module SymbolicOperators =

    [<Test; FSharpQASuiteTest("Conformance/LexicalAnalysis/SymbolicOperators")>]
    let SymbolicOperators () = runpl |> check


module Whitespace =

    [<Test; FSharpQASuiteTest("Conformance/LexicalAnalysis/Whitespace")>]
    let Whitespace () = runpl |> check
