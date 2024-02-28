// #Conformance #DeclarationElements #PInvoke 
#light

// copy of MarshalStruct01 test, but with struct records instead of standard structs
// Sanity check marshalling struct records via PInvoke
// Specifically, the [StructLayout] attribute.

open System.Runtime.InteropServices;

// Define a Point using the sequential layout
[<Struct>]
[<StructLayout(LayoutKind.Sequential)>]
type SequentialPoint =
    {
        X : int
        Y : int
    }

// Define the same point structure but using an explicit layout
[<Struct>]
[<StructLayout(LayoutKind.Explicit)>]
type ExplicitPoint =
    // Randomize elements, the end result is:
    // [X1, X1][X2, X2][Y1][Y2][Y3][Y4]
    {
    
        [<FieldOffset(4)>]
        Y1 : byte
        [<FieldOffset(7)>]
        Y4 : byte
        [<FieldOffset(2)>]
        X2 : int16
        [<FieldOffset(5)>]
        Y2 : byte
        [<FieldOffset(6)>]
        Y3 : byte
        [<FieldOffset(0)>]
        X1 : int16
    }

// Define a rectangle struct explicitly
[<Struct>]
[<StructLayout(LayoutKind.Explicit)>]
type ExplicitRect =
    {
        [<FieldOffset(0)>]
        mutable left : int
        [<FieldOffset(4)>]
        mutable top : int
        [<FieldOffset(8)>]
        mutable right : int
        [<FieldOffset(12)>]
        mutable bottom : int
    }

// Define the PInvoke signatures, one taking the ExplicitPoint the other taking the SequentialPoint
[<DllImport("User32.dll", EntryPoint="PtInRect")>]
extern bool PointInRect_Explicit_Explicit(ExplicitRect&, ExplicitPoint);


[<DllImport("User32.dll", EntryPoint="PtInRect")>]
extern bool PointInRect_Explicit_Sequential(ExplicitRect& rect, SequentialPoint pt);

// The test will attempt to detect two points, A and B, and whether or not they
// exist in the rectangle.
(*

    -15, 10                     15, 10
       X---------------------------X
       |                           |       B (17, 8)
       |                           |
       |            A (-1, -1)     |
       |                           |
       X---------------------------X
    -15, -10                    15, -10

*)

// Define our points
let seqA = { SequentialPoint.X = -1;  Y = -1 }
let seqB = { SequentialPoint.X = 17; Y = 8 }

// 0xFFFFFF : int16 = -1, 0xFF : byte = -1 if arithmetic is NOT checked
let expA = { ExplicitPoint.X1 = 0xFFFFs; X2 = 0xFFFFs; Y1 = 0xFFuy; Y2 = 0xFFuy; Y3 = 0xFFuy; Y4 = 0xFFuy }
let expB = { ExplicitPoint.X1 = 0s; X2 = 17s; Y1 = 0uy; Y2 = 0uy; Y3 = 0uy; Y4 = 8uy }

// Define our rectangle
let mutable theRect = { ExplicitRect.left = -15; right = 15; top = -10; bottom = 10 }

// Test PInvoke!
let seqAResult = PointInRect_Explicit_Sequential(&theRect, seqA)
let expAResult = PointInRect_Explicit_Explicit(&theRect, expA)

let seqBResult = PointInRect_Explicit_Sequential(&theRect, seqB)
let expBResult = PointInRect_Explicit_Explicit(&theRect, expB)

if seqAResult <> true  then exit 1
if seqBResult <> false then exit 2

if expAResult <> true  then exit 3
if expBResult <> false then exit 4

exit 0
