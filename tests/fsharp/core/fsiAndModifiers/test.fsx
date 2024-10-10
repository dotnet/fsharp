#r "TestLibrary.dll"

let mutable errors : string list = []
let fail msg = 
    printfn "FAILURE: %s" msg
    errors <- errors @ [msg]

module Test1 = 
    let x = TestType.Get(100)
    if x <> 100 then fail "Test 1 failed, expected 100"



module TestPack1 = 
    open System.Runtime.InteropServices
    [<type:StructLayout(LayoutKind.Sequential)>]
    type LayoutType =
        struct
            val a: int
        end
    if (sizeof<LayoutType>) <> 4 then fail "TestPack1, expected size 4"

module TestPack2 = 
    open System.Runtime.InteropServices
    [<type:StructLayout(LayoutKind.Sequential, Pack=2)>]
    type LayoutType =
        struct
            val mutable b: byte
            val mutable a: int
        end
    let sz = (sizeof<LayoutType>)
    let mutable n = LayoutType() 
    let a1 = NativeInterop.NativePtr.toNativeInt &&n.a - NativeInterop.NativePtr.toNativeInt &&n
    let b1 = NativeInterop.NativePtr.toNativeInt &&n.b - NativeInterop.NativePtr.toNativeInt &&n
    let got = (sz, a1, b1)
    printfn "got %A" got
    let expected = (6, 2n, 0n)
    if got <> expected then fail (sprintf "TestPack2: got %A, expected %A" got expected)

// From http://bytes.com/topic/c-sharp/answers/654343-what-does-structlayoutattribute-pack-do
module TestPack3 = 
    open System.Runtime.InteropServices
    [<type:StructLayout(LayoutKind.Sequential, Pack=8, Size=128)>]
    type LayoutType =
        struct
            val mutable b1: byte
            val mutable b2: byte
            val mutable l1: int64
            val mutable i1: int
        end
    let sz = (sizeof<LayoutType>)
    let mutable n = LayoutType() 
    let b1off = NativeInterop.NativePtr.toNativeInt &&n.b1 - NativeInterop.NativePtr.toNativeInt &&n
    let b2off = NativeInterop.NativePtr.toNativeInt &&n.b2 - NativeInterop.NativePtr.toNativeInt &&n
    let l1off = NativeInterop.NativePtr.toNativeInt &&n.l1 - NativeInterop.NativePtr.toNativeInt &&n
    let i1off = NativeInterop.NativePtr.toNativeInt &&n.i1 - NativeInterop.NativePtr.toNativeInt &&n
    let got = (sz, b1off, b2off, l1off, i1off)
    let expected = (128, 0n, 1n, 8n, 16n)
    printfn "got %A" got
    if got <> expected then fail (sprintf "TestPack3: got %A, expected %A" got expected)


// Test explicit layout
module TestPack4 = 
    open System.Runtime.InteropServices
    [<type:StructLayout(LayoutKind.Explicit, Pack=8, Size=128)>]
    type LayoutType =
        struct
            [<FieldOffset(0)>] 
            val mutable b1: byte
            [<FieldOffset(4)>] 
            val mutable b2: byte
            [<FieldOffset(8)>] 
            val mutable l1: int64
            [<FieldOffset(12)>] 
            val mutable i1: int
        end
    let sz = (sizeof<LayoutType>)
    let mutable n = LayoutType() 
    let b1off = NativeInterop.NativePtr.toNativeInt &&n.b1 - NativeInterop.NativePtr.toNativeInt &&n
    let b2off = NativeInterop.NativePtr.toNativeInt &&n.b2 - NativeInterop.NativePtr.toNativeInt &&n
    let l1off = NativeInterop.NativePtr.toNativeInt &&n.l1 - NativeInterop.NativePtr.toNativeInt &&n
    let i1off = NativeInterop.NativePtr.toNativeInt &&n.i1 - NativeInterop.NativePtr.toNativeInt &&n
    let got = (sz, b1off, b2off, l1off, i1off)
    let expected =  (128, 0n, 4n, 8n, 12n)
    printfn "got %A" got
    if got <> expected then fail (sprintf "TestPack4: got %A, expected %A" got expected)

module PinTests = 
    open FSharp.NativeInterop
    // Assume that the following class exists.

    type Point = { mutable x : int; mutable y : int }

    let pinObject() = 
        let point = { x = 1; y = 2 }
        use p1 = fixed &point.x   // note, fixed is a keyword and would be highlighted
        NativePtr.get p1 0 + NativePtr.get p1 1
    
    let pinRef() = 
        let point = ref 17
        use p1 = fixed &point.contents   // note, fixed is a keyword and would be highlighted
        NativePtr.read p1 + NativePtr.read p1 
    
    let pinArray1() = 
        let arr = [| 0.0; 1.5; 2.3; 3.4; 4.0; 5.9 |]
        use p1 = fixed arr
        NativePtr.get p1 0 + NativePtr.get p1 1

    let pinArray2() = 
        let arr = [| 0.0; 1.5; 2.3; 3.4; 4.0; 5.9 |]
        // You can initialize a pointer by using the address of a variable. 
        use p = fixed &arr.[0]
        NativePtr.get p 0 + NativePtr.get p 1

    let pinNullArray() = 
        let arr : int[] = null
        use p1 = fixed arr
        4

    let pinEmptyArray() = 
        let arr : int[] = [| |]
        use p1 = fixed arr
        76

    let pinString() = 
        let str = "Hello World"
        // The following assignment initializes p by using a string.
        use pChar = fixed str
        NativePtr.get pChar 0,  NativePtr.get pChar 1

    if pinObject() <> 3 then fail "FAILED: pinObject"
    if pinRef() <> 34 then fail "FAILED: pinObject"
    if pinArray1() <> 1.5 then fail "FAILED: pinArray1"
    if pinArray2() <> 1.5 then fail "FAILED: pinArray2"
    if pinNullArray() <> 4 then fail "FAILED: pinNullArray"
    if pinEmptyArray() <> 76 then fail "FAILED: pinEmptyArray"
    if pinString()  <> ('H', 'e') then fail "FAILED: pinString"


if errors.IsEmpty then 
    printf "TEST PASSED OK" ;
    exit(0)
else 
    for error in errors do 
        printfn "ERROR: %s" error
    exit(1)
    