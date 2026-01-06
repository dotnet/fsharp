// FieldOffsetAttribute on ExtendedLayout struct field should fail
namespace Test

open System.Runtime.InteropServices

[<ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type InvalidFieldOffset =
    struct
        [<FieldOffset(0)>]
        val mutable X: int
    end
