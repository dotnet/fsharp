module FSharp.Compiler.Service.Tests.PrettyNaming

open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Test
open Xunit

// invalid operator name
[<Theory>]
[<InlineData ("op_Dollarfoo", false)>]
// random stuff
[<InlineData ("foo", false)>]
// operator display representations
[<InlineData ("~%%", false)>]
[<InlineData ("~%", false)>]
// not infix operators
[<InlineData ("op_BitwiseOr", false)>]
[<InlineData ("op_Equality", false)>]
// valid logical names
[<InlineData ("op_Splice", true)>]
[<InlineData ("op_SpliceUntyped", true)>]
let ``IsLogicalPrefixOperator`` (logicalName: string, result: bool) =
    IsLogicalPrefixOperator logicalName
    |> Assert.shouldBe result

// empty string
[<Theory>]
[<InlineData ("", false)>]
// invalid operators
[<InlineData ("op_Dynamic", false)>]
[<InlineData ("op_RangeStep", false)>]
// display representation
[<InlineData ("?<-", false)>]
// correct option
[<InlineData ("op_DynamicAssignment", true)>]
let ``IsLogicalTernaryOperator`` (logicalName: string, result: bool) =
    IsLogicalTernaryOperator logicalName
    |> Assert.shouldBe result

// words in operator name
[<Theory>]
[<InlineData ("$foo hoo", false)>]
[<InlineData ("@foo hoo", false)>]
// typo in operator name
[<InlineData ("op_Nagation", false)>]
// invalid operator name
[<InlineData ("op_Dollarfoo", false)>]
[<InlineData ("foo", false)>]
[<InlineData ("$foo", false)>]
// random symbols
[<InlineData ("$", false)>]
// operator display representations
[<InlineData ("+", false)>]
[<InlineData ("[]", false)>]
[<InlineData ("::", false)>]
[<InlineData ("~++", false)>]
[<InlineData (".. ..", false)>]
// not infix operators
[<InlineData ("op_Splice", false)>]
[<InlineData ("op_SpliceUntyped", false)>]
// valid logical names
[<InlineData ("op_Addition", true)>]
[<InlineData ("op_Append", true)>]
[<InlineData ("op_ColonColon", true)>]
[<InlineData ("op_BitwiseOr", true)>]
[<InlineData ("op_GreaterThanOrEqual", true)>]
[<InlineData ("op_Dynamic", true)>]
[<InlineData ("op_ArrayLookup", true)>]
[<InlineData ("op_DynamicAssignment", true)>]
[<InlineData ("op_ArrayAssign", true)>]
let ``IsLogicalInfixOpName`` (logicalName: string, result: bool) =
    IsLogicalInfixOpName logicalName
    |> Assert.shouldBe result
