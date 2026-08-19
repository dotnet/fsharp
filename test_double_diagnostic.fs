namespace Test

open System.Runtime.InteropServices

[<StructLayout(enum<LayoutKind>(1)); ExtendedLayout(ExtendedLayoutKind.CStruct)>]
type BothAttrs =
    struct
        val mutable X: int
    end
