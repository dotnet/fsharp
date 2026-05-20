// LayoutKind.Extended (value 1) via StructLayoutAttribute should fail
namespace Test

open System.Runtime.InteropServices

[<StructLayout(enum<LayoutKind>(1))>]
type InvalidExtendedViaStructLayout =
    struct
        val mutable X: int
    end
