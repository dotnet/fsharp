// ExtendedLayoutAttribute + StructLayoutAttribute should fail
namespace System.Runtime.InteropServices

// Mock ExtendedLayoutAttribute and ExtendedLayoutKind for testing
type ExtendedLayoutKind =
    | CStruct = 0
    | CUnion = 1

[<System.AttributeUsage(System.AttributeTargets.Struct, AllowMultiple = false)>]
type ExtendedLayoutAttribute(kind: ExtendedLayoutKind) =
    inherit System.Attribute()
    member _.Kind = kind

namespace Test

open System.Runtime.InteropServices

[<ExtendedLayout(ExtendedLayoutKind.CStruct)>]
[<StructLayout(LayoutKind.Sequential)>]
type ConflictingStruct =
    struct
        val mutable X: int
    end
