module FSharp.Compiler.Service.Tests.PrettyNaming

open FSharp.Compiler.Syntax.PrettyNaming
open FsUnit
open NUnit.Framework

// words in operator name
[<TestCase "$foo hoo">]
[<TestCase "@foo hoo">]
// typo in operator name
[<TestCase "op_Nagation">]
// invalid operator name
[<TestCase "op_Dollarfoo">]
[<TestCase "foo">]
[<TestCase "$foo">]
// random symbols
[<TestCase "$">]
// operator display representations
[<TestCase "+">]
[<TestCase "[]">]
[<TestCase "::">]
[<TestCase "~++">]
[<TestCase ".. ..">]
// not infix operators
[<TestCase "op_Splice">]
[<TestCase "op_SpliceUntyped">]
let ``IsLogicalInfixOpName detects bad logical names`` logicalName =
    IsLogicalInfixOpName logicalName
    |> should equal false

[<TestCase "op_Addition">]
[<TestCase "op_Append">]
[<TestCase "op_ColonColon">]
[<TestCase "op_BitwiseOr">]
[<TestCase "op_GreaterThanOrEqual">]
[<TestCase "op_Dynamic">]
[<TestCase "op_ArrayLookup">]
[<TestCase "op_DynamicAssignment">]
[<TestCase "op_ArrayAssign">]
let ``IsLogicalInfixOpName detects good logical names`` logicalName =
    IsLogicalInfixOpName logicalName
    |> should equal true
