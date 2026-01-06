// ExtendedLayoutAttribute on delegate should fail
namespace Test

open System.Runtime.InteropServices

[<ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type InvalidDelegate = delegate of int -> int
