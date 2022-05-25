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
    

let f (x: int) (y: int) = compare x y
            """
        
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"-g"; "--optimize+"|], script,
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
    

let f (x: uint) (y: uint) = compare x y
            """
        
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"-g"; "--optimize+"|], script,
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
    

let f (x: int64) (y: int64) = compare x y
            """
        
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"-g"; "--optimize+"|], script,
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
    

let f (x: uint64) (y: uint64) = compare x y
            """
        
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"-g"; "--optimize+"|], script,
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
    

let f (x: int16) (y: int16) = compare x y
            """
        
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"-g"; "--optimize+"|], script,
            (fun verifier ->
                verifier.VerifyIL
                    [
                        """
                            IL_0000:  ldarg.0
                            IL_0001:  ldarg.1
                            IL_0002:  sub
                            IL_0003:  ret
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
    

let f (x: uint16) (y: uint16) = compare x y
            """
        
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"-g"; "--optimize+"|], script,
            (fun verifier ->
                verifier.VerifyIL
                    [
                        """
                            IL_0000:  ldarg.0
                            IL_0001:  ldarg.1
                            IL_0002:  sub
                            IL_0003:  ret
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
    

let f (x: byte) (y: byte) = compare x y
            """
        
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"-g"; "--optimize+"|], script,
            (fun verifier ->
                verifier.VerifyIL
                    [
                        """
                            IL_0000:  ldarg.0
                            IL_0001:  ldarg.1
                            IL_0002:  sub
                            IL_0003:  ret
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
    

let f (x: sbyte) (y: sbyte) = compare x y
            """
        
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"-g"; "--optimize+"|], script,
            (fun verifier ->
                verifier.VerifyIL
                    [
                        """
                            IL_0000:  ldarg.0
                            IL_0001:  ldarg.1
                            IL_0002:  sub
                            IL_0003:  ret
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
    

let f (x: char) (y: char) = compare x y
            """
        
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"-g"; "--optimize+"|], script,
            (fun verifier ->
                verifier.VerifyIL
                    [
                        """
                            IL_0000:  ldarg.0
                            IL_0001:  ldarg.1
                            IL_0002:  sub
                            IL_0003:  ret
                        """
                    ]
            )
        )

    [<Test>]
    let Script_Compare_bool() =
        let script = 
            """
module Test 
open System
    

let f (x: bool) (y: bool) = compare x y
            """
        
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"-g"; "--optimize+"|], script,
            (fun verifier ->
                verifier.VerifyIL
                    [
                        """
                            IL_0000:  ldarg.0
                            IL_0001:  ldarg.1
                            IL_0002:  sub
                            IL_0003:  ret
                        """
                    ]
            )
        )


    

    module Assert =
        /// Checks that x and y have same sign.
        /// Since compare and CompareTo are not restricted to returning 0,-1,1
        /// but can return any positive or negative values, tests use this function
        /// to check that the behavior is the same without over specifying.
        let areSameSign (x: int) (y: int) =
            Math.Sign(x) |> Assert.areEqual (Math.Sign(y))

    [<Test>]
    let Check_equivalence_with_CompareTo() =
        let rnd = Random()
        for i in 0 .. 1000 do
            let x = rnd.Next()
            let y = rnd.Next()

            compare x y |>  Assert.areSameSign (x.CompareTo(y))



    [<Test>]
    let Check_limit_case_equivalence_with_CompareTo_int32() =
        let values = [0; 1; -1; Int32.MinValue; Int32.MaxValue; Int32.MinValue+1; Int32.MaxValue-1]

        for x in values do
        for y in values do
            compare x y |> Assert.areSameSign (x.CompareTo(y))

    [<Test>]
    let Check_limit_case_equivalence_with_CompareTo_uint32() =
        let values = [0u; 1u; UInt32.MaxValue; UInt32.MaxValue-1u]

        for x in values do
        for y in values do
            compare x y |> Assert.areSameSign (x.CompareTo(y))

    [<Test>]
    let Check_limit_case_equivalence_with_CompareTo_int64() =
        let values = [0L; 1L; -1L; Int64.MinValue; Int64.MaxValue; Int64.MinValue+1L; Int64.MaxValue-1L]

        for x in values do
        for y in values do
            compare x y |> Assert.areSameSign (x.CompareTo(y))

    [<Test>]
    let Check_limit_case_equivalence_with_CompareTo_uint64() =
        let values = [0UL; 1UL; UInt64.MaxValue; UInt64.MaxValue-1UL]

        for x in values do
        for y in values do
            compare x y |> Assert.areSameSign (x.CompareTo(y))

    [<Test>]
    let Check_limit_case_equivalence_with_CompareTo_int16() =
        let values = [0s; 1s; -1s; Int16.MinValue; Int16.MaxValue; Int16.MinValue+1s; Int16.MaxValue-1s]

        for x in values do
        for y in values do
            compare x y |> Assert.areSameSign (x.CompareTo(y))

    [<Test>]
    let Check_limit_case_equivalence_with_CompareTo_uint16() =
        let values = [0us; 1us; UInt16.MaxValue; UInt16.MaxValue-1us]

        for x in values do
        for y in values do
            compare x y |> Assert.areSameSign (x.CompareTo(y))


    [<Test>]
    let Check_limit_case_equivalence_with_CompareTo_sbyte() =
        let values = [0y; 1y; -1y; SByte.MinValue; SByte.MaxValue; SByte.MinValue+1y; SByte.MaxValue-1y]

        for x in values do
        for y in values do
            compare x y |> Assert.areSameSign (x.CompareTo(y))

    [<Test>]
    let Check_limit_case_equivalence_with_CompareTo_byte() =
        let values = [0uy; 1uy; Byte.MaxValue; Byte.MaxValue-1uy]

        for x in values do
        for y in values do
            compare x y |> Assert.areSameSign (x.CompareTo(y))

    [<Test>]
    let Check_limit_case_equivalence_with_CompareTo_char() =
        let values = [Char.MinValue; Char.MinValue+char 1 ; Char.MaxValue; Char.MaxValue+char -1]

        for x in values do
        for y in values do
            compare x y |> Assert.areSameSign (x.CompareTo(y))

    [<Test>]
    let Check_limit_case_equivalence_with_CompareTo_bool() =
        let values = [false; true]

        for x in values do
        for y in values do
            compare x y |> Assert.areSameSign (x.CompareTo(y))

