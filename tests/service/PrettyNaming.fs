module FSharp.Compiler.Service.Tests.PrettyNaming

open FSharp.Compiler.Syntax.PrettyNaming
open FsUnit
open NUnit.Framework

// invalid operator name
[<TestCase ("op_Dollarfoo", false)>]
// random stuff
[<TestCase ("foo", false)>]
// operator display representations
[<TestCase ("~%%", false)>]
[<TestCase ("~%", false)>]
// not infix operators
[<TestCase ("op_BitwiseOr", false)>]
[<TestCase ("op_Equality", false)>]
// valid logical names
[<TestCase ("op_Splice", true)>]
[<TestCase ("op_SpliceUntyped", true)>]
let ``IsLogicalPrefixOperator`` logicalName result =
    IsLogicalPrefixOperator logicalName
    |> should equal result

// empty string
[<TestCase ("", false)>]
// invalid opearators
[<TestCase ("op_Dynamic", false)>]
[<TestCase ("op_RangeStep", false)>]
// display representation
[<TestCase ("?<-", false)>]
// correction option
[<TestCase ("op_DynamicAssignment", true)>]
let ``IsLogicalTernaryOperator`` logicalName result =
    IsLogicalTernaryOperator logicalName
    |> should equal result

// words in operator name
[<TestCase ("$foo hoo", false)>]
[<TestCase ("@foo hoo", false)>]
// typo in operator name
[<TestCase ("op_Nagation", false)>]
// invalid operator name
[<TestCase ("op_Dollarfoo", false)>]
[<TestCase ("foo", false)>]
[<TestCase ("$foo", false)>]
// random symbols
[<TestCase ("$", false)>]
// operator display representations
[<TestCase ("+", false)>]
[<TestCase ("[]", false)>]
[<TestCase ("::", false)>]
[<TestCase ("~++", false)>]
[<TestCase (".. ..", false)>]
// not infix operators
[<TestCase ("op_Splice", false)>]
[<TestCase ("op_SpliceUntyped", false)>]
// valid logical names
[<TestCase ("op_Addition", true)>]
[<TestCase ("op_Append", true)>]
[<TestCase ("op_ColonColon", true)>]
[<TestCase ("op_BitwiseOr", true)>]
[<TestCase ("op_GreaterThanOrEqual", true)>]
[<TestCase ("op_Dynamic", true)>]
[<TestCase ("op_ArrayLookup", true)>]
[<TestCase ("op_DynamicAssignment", true)>]
[<TestCase ("op_ArrayAssign", true)>]
let ``IsLogicalInfixOpName`` logicalName result =
    IsLogicalInfixOpName logicalName
    |> should equal result
