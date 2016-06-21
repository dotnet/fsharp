module ``FSharpQA-Tests-Libraries-Core``

open NUnit.Framework

open NUnitConf
open RunPlTest

module Collections =

    [<Test; FSharpQASuiteTest("Libraries/Core/Collections")>]
    let Collections () = runpl |> check

module ExtraTopLevelOperators =

    [<Test; FSharpQASuiteTest("Libraries/Core/ExtraTopLevelOperators")>]
    let ExtraTopLevelOperators () = runpl |> check

module LanguagePrimitives =

    [<Test; FSharpQASuiteTest("Libraries/Core/LanguagePrimitives")>]
    let LanguagePrimitives () = runpl |> check

module NativeInterop_stackalloc =

    [<Test; FSharpQASuiteTest("Libraries/Core/NativeInterop/stackalloc")>]
    let stackalloc () = runpl |> check

module Operators =

    [<Test; FSharpQASuiteTest("Libraries/Core/Operators")>]
    let Operators () = runpl |> check

module PartialTrust =

    [<Test; FSharpQASuiteTest("Libraries/Core/PartialTrust")>]
    let PartialTrust () = runpl |> check

module Reflection =

    [<Test; FSharpQASuiteTest("Libraries/Core/Reflection")>]
    let Reflection () = runpl |> check

module Unchecked =

    [<Test; FSharpQASuiteTest("Libraries/Core/Unchecked")>]
    let Unchecked () = runpl |> check

