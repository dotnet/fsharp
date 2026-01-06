// ExtendedLayoutAttribute + StructLayoutAttribute should fail
namespace Test

open System.Runtime.InteropServices

[<ExtendedLayout(ExtendedLayoutKind.CStruct)>]
[<StructLayout(LayoutKind.Sequential)>]
type ConflictingStruct =
    struct
        val mutable X: int
    end
