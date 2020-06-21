// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework

#nowarn "9"

[<TestFixture>]
module ``Stackalloc Tests`` =
    
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
        let mutable noerr = true
        
        let data = NativeInterop.NativePtr.stackalloc<int64> 100

        for i = 0 to 99 do
            NativeInterop.NativePtr.set data i (int64 (i*i))
        for i = 0 to 99 do
            if not (NativeInterop.NativePtr.get data i = (int64 (i*i))) then 
                noerr <- false

        for i = 0 to 99 do
            let datai = NativeInterop.NativePtr.toByRef (NativeInterop.NativePtr.add data i)
            datai <- int64 (1-i)
         
        for i = 0 to 99 do
            let datai = NativeInterop.NativePtr.toByRef (NativeInterop.NativePtr.add data i)
            if not (datai = int64 (1-i)) then 
                noerr <- false

        Assert.IsTrue noerr