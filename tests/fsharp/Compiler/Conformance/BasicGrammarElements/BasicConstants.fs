// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test
open FSharp.Compiler.Diagnostics

[<TestFixture>]
module ``Basic Grammar Element Constants`` =

    [<Test>]
    let ``Basic constants compile `` () =
        CompilerAssert.Pass 
            """
let sbyteConst = 1y
let int16Const = 1us
let int32Const = 1ul
let int64Const = 1UL
    
let byteConst  = 1uy
let uint16Const = 1us
let uint32Const = 1ul
let uint64Const = 1uL
    
let ieee32Const1 = 1.0f
let ieee32Const2 = 1.0F
let ieee32Const3 = 0x0000000000000001lf
let ieee32Const4 = 1F
let ieee32Const5 = 1f
    
let ieee64Const1 = 1.0
let ieee64Const2 = 0x0000000000000001LF
    
let bigintConst = 1I
    
// let bignumConst = 1N - you need a reference to PowerPack.dll now
    
let decimalConst1 = 1.0M
let decimalConst2 = 1.0m
    
let charConst = '1'
    
let stringConst = "1"
    
let bytestringConst = "1"B
    
let bytecharConst = '1'B
    
let boolConst1 = true
let boolConst2 = false
    
let unitConst = ()
            """

    [<Test>]
    let ``Long with underscores``() = 
        CompilerAssert.CompileExeAndRun
            """
let creditCardNumber = 1234_5678_9012_3456L
let socialSecurityNumber = 999_99_9999L

if socialSecurityNumber <> 999999999L then failwith "Wrong parsing"    
if creditCardNumber <> 1234567890123456L then failwith "Wrong parsing"    
printfn "%A" socialSecurityNumber
printfn "%A" creditCardNumber
            """

    [<Test>]
    let ``float 32 with underscores``() = 
        CompilerAssert.CompileExeAndRun
            """
let pi = 3.14_15F
if pi <> 3.1415F  then failwith "Wrong parsing"
printfn "%A" pi
            """

    [<Test>]
    let ``int with underscores hexBytes``() = 
        CompilerAssert.CompileExeAndRun
            """
let hexBytes = 0xFF_EC_DE_5E
if hexBytes <> 0xFFECDE5E then failwith "Wrong parsing"
printfn "%A" hexBytes
            """


    [<Test>]
    let ``int with underscore hexWords``() = 
        CompilerAssert.CompileExeAndRun
            """
let hexWords = 0xCAFE_BABE
if hexWords <> 0xCAFEBABE then failwith "Wrong parsing"
printfn "%A" hexWords
            """

    [<Test>]
    let ``Long with underscores maxLong``() = 
        CompilerAssert.CompileExeAndRun
            """
let maxLong = 0x7fff_ffff_ffff_ffffL
if maxLong <> 0x7fffffffffffffffL then failwith "Wrong parsing"
printfn "%A" maxLong
            """

    [<Test>]
    let ``int with underscore nybbles``() = 
        CompilerAssert.CompileExeAndRun
            """
let nybbles = 0b0010_0101
if nybbles <> 0b00100101 then failwith "Wrong parsing"
printfn "%A" nybbles
            """

    [<Test>]
    let ``int with underscores bytes``() = 
        CompilerAssert.CompileExeAndRun
            """
let bytes = 0b11010010_01101001_10010100_10010010
if bytes <> 0b11010010011010011001010010010010 then failwith "Wrong parsing"
printfn "%A" bytes
            """

    [<Test>]
    let ``int with single underscore literal``() = 
        CompilerAssert.CompileExeAndRun
            """
let x2 = 5_2
if x2 <> 52 then failwith "Wrong parsing"
printfn "%A" x2
            """

    [<Test>]
    let ``int with multiple underscores literal``() = 
        CompilerAssert.CompileExeAndRun
            """
let x4 = 5_______2
if x4 <> 52 then failwith "Wrong parsing"
printfn "%A" x4
            """

    [<Test>]
    let ``int with single underscore Hex literal``() = 
        CompilerAssert.CompileExeAndRun
            """
let x7 = 0x5_2
if x7 <> 0x52 then
    failwith "Wrong parsing"
printfn "%A" x7
            """

    [<Test>]
    let ``int with single underscore after leading zero literal``() = 
        CompilerAssert.CompileExeAndRun
            """
let x9 = 0_52
if x9 <> 052 then failwith "Wrong parsing"
printfn "%A" x9
            """

    [<Test>]
    let ``int with single underscore after leteral with leading zero ``() = 
        CompilerAssert.CompileExeAndRun
            """
let x10 = 05_2
if x10 <> 052 then failwith "Wrong parsing"
printfn "%A" x10
            """

    [<Test>]
    let ``int with single underscore after octo leteral ``() = 
        CompilerAssert.CompileExeAndRun
            """
let x14 = 0o5_2
if x14 <> 0o52 then failwith "Wrong parsing"
printfn "%A" x14
            """
    [<Test>]
    let ``dotless float``() = 
        CompilerAssert.CompileExeWithOptions([|"--langversion:5.0"|],
            """
let x = 42f
printfn "%A" x
            """)

    [<Test>]
    let ``dotted float``() = 
        CompilerAssert.CompileExe("""
let x = 42.f
printfn "%A" x
            """)

    [<Test>]
    let ``dotted floats should be equal to dotless floats``() = 
        CompilerAssert.CompileExeAndRunWithOptions([|"--langversion:5.0"|],
            """
if 1.0f <> 1f then failwith "1.0f <> 1f"
            """)

    [<Test>]
    let ``exponent dotted floats should be equal to dotted floats``() =
        CompilerAssert.CompileExeAndRun
            """
if 1.0e1f <> 10.f then failwith "1.0e1f <> 10.f"
            """

    [<Test>]
    let ``exponent dotless floats should be equal to dotted floats``() = 
        CompilerAssert.CompileExeAndRun
            """
if 1e1f <> 10.f then failwith "1e1f <> 10.f" 
            """

    [<Test>]
    let ``exponent dotted floats should be equal to dotless floats``() = 
        CompilerAssert.CompileExeAndRunWithOptions(
            [|"--langversion:5.0"|],
            """
if 1.0e1f <> 10f then failwith "1.0e1f <> 10f" 
            """)

    [<Test>]
    let ``exponent dotless floats should be equal to dotless floats``() = 
        CompilerAssert.CompileExeAndRunWithOptions(
            [|"--langversion:5.0"|],
            """
if 1e1f <> 10f then failwith "1e1f <> 10f" 
            """)
