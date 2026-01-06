// ExtendedLayoutAttribute on class should fail
namespace Test

open System.Runtime.InteropServices

[<ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type InvalidClass() =
    member val X = 0 with get, set
