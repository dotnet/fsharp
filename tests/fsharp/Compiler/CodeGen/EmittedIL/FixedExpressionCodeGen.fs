namespace FSharp.Compiler.UnitTests.CodeGen.EmittedIL

open FSharp.Compiler.UnitTests
open NUnit.Framework

[<TestFixture>]
module FixedExpressionCodeGen =

#if NETCOREAPP
    [<Test>]
    let ``Pinning a string emits string.GetPinnableReference`` () =
        let code = """
module FixedOnAString
#nowarn "9"
let foo(s : string) =
    use ptr = fixed s
    ptr
"""
        let il = """
char& modreq([System.Runtime]System.Runtime.InteropServices.InAttribute) [System.Runtime]System.String::GetPinnableReference()
"""
        CompilerAssert.CompileLibraryAndVerifyIL code (fun verifier -> verifier.VerifyIL([ il ]))
#else
    [<Test>]
    let ``Pinning a string emits RuntimeHelpers.GetOffsetToStringData`` () =
        let code = """
module FixedOnAString
#nowarn "9"
let foo(s : string) =
    use ptr = fixed s
    ptr
"""
        let il = """
int32 [runtime]System.Runtime.CompilerServices.RuntimeHelpers::get_OffsetToStringData()
"""
        CompilerAssert.CompileLibraryAndVerifyIL code (fun verifier -> verifier.VerifyIL([ il ]))
#endif