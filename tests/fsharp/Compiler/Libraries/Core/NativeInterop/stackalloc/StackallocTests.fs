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