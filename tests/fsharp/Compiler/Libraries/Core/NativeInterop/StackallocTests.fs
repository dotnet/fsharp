// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test.Utilities
open FSharp.Compiler.SourceCodeServices

#nowarn "9"

[<TestFixture>]
module ``Stackalloc Tests`` =

    type E = | A = 1
             | B = 2

    [<Test>]
    let ``Stackalloc of DateTime``() =
        let data = NativeInterop.NativePtr.stackalloc<System.DateTime> 100
        let now = System.DateTime.Now
        for i = 0 to 99 do
            NativeInterop.NativePtr.set data i now
        for i = 0 to 99 do
            Assert.areEqual (NativeInterop.NativePtr.get data i) now
                 
        let later = now.AddDays 1.
        for i = 0 to 99 do
            let datai = NativeInterop.NativePtr.toByRef (NativeInterop.NativePtr.add data i)
            datai <- later
        for i = 0 to 99 do
            let datai = NativeInterop.NativePtr.toByRef (NativeInterop.NativePtr.add data i)
            Assert.areEqual datai later

    [<Test>]
    let ``Stackalloc of enum``() =
        let data = NativeInterop.NativePtr.stackalloc<E> 10
        
        for i = 0 to 9 do
            NativeInterop.NativePtr.set data i (if (i % 2)=0 then E.A else E.B)
        
        for i = 0 to 9 do
            let expected = if (i % 2) = 0 then E.A else E.B
            Assert.areEqual (NativeInterop.NativePtr.get data i) expected

        for i = 0 to 9 do
            let datai = NativeInterop.NativePtr.toByRef (NativeInterop.NativePtr.add data i)
            datai <- (if (i % 2)=1 then E.A else E.B)
        
        for i = 0 to 9 do
            let datai = NativeInterop.NativePtr.toByRef (NativeInterop.NativePtr.add data i)
            let expected = if (i % 2)=1 then E.A else E.B
            Assert.areEqual datai expected

    [<Test>]
    let ``Stackalloc of imported enum``() =
        Assert.DoesNotThrow (TestDelegate (fun () -> 
            NativeInterop.NativePtr.stackalloc<System.DayOfWeek> 1 |> ignore))

    [<Test>]
    let ``Stackalloc of imported struct``() =
        Assert.DoesNotThrow (TestDelegate (fun () -> 
            NativeInterop.NativePtr.stackalloc<System.TimeSpan> 1 |> ignore))

    [<Test>]
    let ``Stackalloc of imported class``() =
        CompilerAssert.TypeCheckSingleError
            """
#nowarn "9"

let _ = NativeInterop.NativePtr.stackalloc<System.Object> 1
            """
            FSharpErrorSeverity.Error
            1
            (4, 9, 4, 43)
            "A generic construct requires that the type 'System.Object' is an unmanaged type"

    [<Test>]
    let ``Stackalloc of imported interface``() =
        CompilerAssert.TypeCheckSingleError
            """
#nowarn "9"

let _ = NativeInterop.NativePtr.stackalloc<System.Collections.IEnumerable> 1
            """
            FSharpErrorSeverity.Error
            1
            (4, 9, 4, 43)
            "A generic construct requires that the type 'System.Collections.IEnumerable' is an unmanaged type"

    [<Test>]
    let ``Stackalloc of imported delegate``() =
        CompilerAssert.TypeCheckSingleError
            """
#nowarn "9"

let _ = NativeInterop.NativePtr.stackalloc<System.EventHandler> 1
            """
            FSharpErrorSeverity.Error
            1
            (4, 9, 4, 43)
            "A generic construct requires that the type 'System.EventHandler' is an unmanaged type"

    [<Test>]
    let ``Stackalloc of int``() =
        let data = NativeInterop.NativePtr.stackalloc<int> 100
           
        for i = 0 to 99 do
            NativeInterop.NativePtr.set data i (i*i)
                
        for i = 0 to 99 do
            Assert.areEqual (NativeInterop.NativePtr.get data i) (i*i)
        
        for i = 0 to 99 do
            let datai = NativeInterop.NativePtr.toByRef (NativeInterop.NativePtr.add data i)
            datai <- 1-i
            
        for i = 0 to 99 do
            let datai = NativeInterop.NativePtr.toByRef (NativeInterop.NativePtr.add data i)
            Assert.areEqual datai (1-i)

    [<Test>]
    let ``Stackalloc of int64``() =
        let data = NativeInterop.NativePtr.stackalloc<int64> 100

        for i = 0 to 99 do
            NativeInterop.NativePtr.set data i (int64 (i*i))
        for i = 0 to 99 do
            Assert.areEqual (NativeInterop.NativePtr.get data i) (int64 (i*i))

        for i = 0 to 99 do
            let datai = NativeInterop.NativePtr.toByRef (NativeInterop.NativePtr.add data i)
            datai <- int64 (1-i)
         
        for i = 0 to 99 do
            let datai = NativeInterop.NativePtr.toByRef (NativeInterop.NativePtr.add data i)
            Assert.areEqual datai (int64 (1-i))

    [<Test>]
    let ``Stackalloc of managed class``() =
        CompilerAssert.TypeCheckSingleError
            """
#nowarn "9"

type C() = 
    class
        member __.M = 10
        member __.N(x) = x + 1
    end

let _ = NativeInterop.NativePtr.stackalloc<C> 1
            """
            FSharpErrorSeverity.Error
            1
            (10, 9, 10, 43)
            "A generic construct requires that the type 'C' is an unmanaged type"

    [<Test>]
    let ``Stackalloc of managed record``() =
        CompilerAssert.TypeCheckSingleError
            """
#nowarn "9"

type R = { A : int }

let _ = NativeInterop.NativePtr.stackalloc<R> 1
            """
            FSharpErrorSeverity.Error
            1
            (6, 9, 6, 43)
            "A generic construct requires that the type 'R' is an unmanaged type"
            
    [<Test>]
    let ``Stackalloc zero-size``() =
        // Regression test for FSHARP1.0:
        // stackalloc<System.DateTime> 0
            
        let testDelegate = TestDelegate (fun () -> 
            // check stackalloc 0 -- ok
            let data = NativeInterop.NativePtr.stackalloc<System.DateTime> 0
                        
            // The returned pointer is undefined
            // No allocation should happen
            let _ = NativeInterop.NativePtr.toNativeInt data
                        
            ())
            
        Assert.DoesNotThrow testDelegate