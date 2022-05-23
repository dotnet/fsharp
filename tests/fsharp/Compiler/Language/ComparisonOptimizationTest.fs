namespace FSharp.Compiler.UnitTests

open System
open NUnit.Framework
open FSharp.Test

[<TestFixture>]
module ComparisonOptimizationTests =

    [<Test>]
    let Script_Compare_int() =
        let script = 
            """
module Test 
open System
    

let f (x: int) (y: int) =  compare x y
            """
        
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"-g"; "--optimize-"|], script,
            (fun verifier ->
                verifier.VerifyIL
                    [
                        """
                            IL_0000:  ldarg.0
                            IL_0001:  ldarg.1
                            IL_0002:  cgt
                            IL_0004:  ldarg.0
                            IL_0005:  ldarg.1
                            IL_0006:  clt
                            IL_0008:  sub
                            IL_0009:  ret
                        """
                    ]
            )
        )

    [<Test>]
    let Script_Compare_uint() =
        let script = 
            """
module Test 
open System
    

let f (x: uint) (y: uint) =  compare x y
            """
        
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"-g"; "--optimize-"|], script,
            (fun verifier ->
                verifier.VerifyIL
                    [
                        """
                            IL_0000:  ldarg.0
                            IL_0001:  ldarg.1
                            IL_0002:  cgt.un
                            IL_0004:  ldarg.0
                            IL_0005:  ldarg.1
                            IL_0006:  clt.un
                            IL_0008:  sub
                            IL_0009:  ret
                        """
                    ]
            )
        )

    [<Test>]
    let Script_Compare_int64() =
        let script = 
            """
module Test 
open System
    

let f (x: int64) (y: int64) =  compare x y
            """
        
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"-g"; "--optimize-"|], script,
            (fun verifier ->
                verifier.VerifyIL
                    [
                        """
                            IL_0000:  ldarg.0
                            IL_0001:  ldarg.1
                            IL_0002:  cgt
                            IL_0004:  ldarg.0
                            IL_0005:  ldarg.1
                            IL_0006:  clt
                            IL_0008:  sub
                            IL_0009:  ret
                        """
                    ]
            )
        )

    [<Test>]
    let Script_Compare_uint64() =
        let script = 
            """
module Test 
open System
    

let f (x: uint64) (y: uint64) =  compare x y
            """
        
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"-g"; "--optimize-"|], script,
            (fun verifier ->
                verifier.VerifyIL
                    [
                        """
                            IL_0000:  ldarg.0
                            IL_0001:  ldarg.1
                            IL_0002:  cgt.un
                            IL_0004:  ldarg.0
                            IL_0005:  ldarg.1
                            IL_0006:  clt.un
                            IL_0008:  sub
                            IL_0009:  ret
                        """
                    ]
            )
        )


    [<Test>]
    let Script_Compare_int16() =
        let script = 
            """
module Test 
open System
    

let f (x: int16) (y: int16) =  compare x y
            """
        
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"-g"; "--optimize-"|], script,
            (fun verifier ->
                verifier.VerifyIL
                    [
                        """
                            IL_0000:  ldarg.0
                            IL_0001:  ldarg.1
                            IL_0002:  cgt
                            IL_0004:  ldarg.0
                            IL_0005:  ldarg.1
                            IL_0006:  clt
                            IL_0008:  sub
                            IL_0009:  ret
                        """
                    ]
            )
        )

    [<Test>]
    let Script_Compare_uint16() =
        let script = 
            """
module Test 
open System
    

let f (x: uint16) (y: uint16) =  compare x y
            """
        
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"-g"; "--optimize-"|], script,
            (fun verifier ->
                verifier.VerifyIL
                    [
                        """
                            IL_0000:  ldarg.0
                            IL_0001:  ldarg.1
                            IL_0002:  cgt.un
                            IL_0004:  ldarg.0
                            IL_0005:  ldarg.1
                            IL_0006:  clt.un
                            IL_0008:  sub
                            IL_0009:  ret
                        """
                    ]
            )
        )

    [<Test>]
    let Script_Compare_byte() =
        let script = 
            """
module Test 
open System
    

let f (x: byte) (y: byte) =  compare x y
            """
        
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"-g"; "--optimize-"|], script,
            (fun verifier ->
                verifier.VerifyIL
                    [
                        """
                            IL_0000:  ldarg.0
                            IL_0001:  ldarg.1
                            IL_0002:  cgt.un
                            IL_0004:  ldarg.0
                            IL_0005:  ldarg.1
                            IL_0006:  clt.un
                            IL_0008:  sub
                            IL_0009:  ret
                        """
                    ]
            )
        )


    [<Test>]
    let Script_Compare_sbyte() =
        let script = 
            """
module Test 
open System
    

let f (x: sbyte) (y: sbyte) =  compare x y
            """
        
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"-g"; "--optimize-"|], script,
            (fun verifier ->
                verifier.VerifyIL
                    [
                        """
                            IL_0000:  ldarg.0
                            IL_0001:  ldarg.1
                            IL_0002:  cgt
                            IL_0004:  ldarg.0
                            IL_0005:  ldarg.1
                            IL_0006:  clt
                            IL_0008:  sub
                            IL_0009:  ret
                        """
                    ]
            )
        )


    [<Test>]
    let Script_Compare_char() =
        let script = 
            """
module Test 
open System
    

let f (x: char) (y: char) =  compare x y
            """
        
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"-g"; "--optimize-"|], script,
            (fun verifier ->
                verifier.VerifyIL
                    [
                        """
                            IL_0000:  ldarg.0
                            IL_0001:  ldarg.1
                            IL_0002:  cgt.un
                            IL_0004:  ldarg.0
                            IL_0005:  ldarg.1
                            IL_0006:  clt.un
                            IL_0008:  sub
                            IL_0009:  ret
                        """
                    ]
            )
        )



    [<Test>]
    let Check_equivalence_with_CompareTo() =
        let rnd = Random()
        for i in 0 .. 1000 do
            let x = rnd.Next()
            let y = rnd.Next()

            compare x y |>  Assert.areEqual (x.CompareTo(y))



