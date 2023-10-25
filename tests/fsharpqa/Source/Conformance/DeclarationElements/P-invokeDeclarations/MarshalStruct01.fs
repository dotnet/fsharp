// #Conformance #DeclarationElements #PInvoke 
#light

// Sanity check marshalling structs via PInvoke
// Specifically, the [StructLayout] attribute.

open System.Runtime.InteropServices;

// Define a Point using the sequential layout
[<StructLayout(LayoutKind.Sequential)>]
type SequentialPoint = struct
    new (x, y) = { X = x; Y = y } 
    val X : int
    val Y : int
end   

// Define the same point structure but using an explicit layout
[<StructLayout(LayoutKind.Explicit)>]
type ExplicitPoint = struct
    // Randomize elements, the end result is:
    // [X1, X1][X2, X2][Y1][Y2][Y3][Y4]
    new (x1, x2, y1, y2, y3, y4) = { X1 = x1; X2 = x2; Y1 = y1; Y2 = y2; Y3 = y3; Y4 = y4 }
    
    [<FieldOffset(4)>]
    val Y1 : byte
    [<FieldOffset(7)>]
    val Y4 : byte
    [<FieldOffset(2)>]
    val X2 : int16
    [<FieldOffset(5)>]
    val Y2 : byte
    [<FieldOffset(6)>]
    val Y3 : byte
    [<FieldOffset(0)>]
    val X1 : int16
end   

// Define a rectangle struct explicitly
[<StructLayout(LayoutKind.Explicit)>]
type ExplicitRect = struct
    new (l, r, t, b) = { left = l; top = t; right = r; bottom = b }
   [<FieldOffset(0)>]
   val mutable left : int
   [<FieldOffset(4)>]
   val mutable top : int
   [<FieldOffset(8)>]
   val mutable right : int
   [<FieldOffset(12)>]
   val mutable bottom : int
end

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
let seqA = new SequentialPoint(-1, -1)
let seqB = new SequentialPoint(17, 8)

// 0xFFFFFF : int16 = -1, 0xFF : byte = -1 if arithmetic is NOT checked
let expA = new ExplicitPoint(0xFFFFs, 0xFFFFs, 0xFFuy, 0xFFuy, 0xFFuy, 0xFFuy)
let expB = new ExplicitPoint(0s, 17s, 0uy, 0uy, 0uy, 8uy)

// Define our rectangle
let mutable theRect = ExplicitRect(-15, 15, -10, 10)

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
