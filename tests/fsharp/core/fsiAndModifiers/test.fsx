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


if errors.IsEmpty then 
    System.IO.File.WriteAllText("test.ok", "")
else 
    for error in errors do 
        printfn "ERROR: %s" error

if errors.IsEmpty then System.IO.File.WriteAllText("test.ok", "")
else 
    for error in errors do 
        printfn "ERROR: %s" error
    