// ExtendedLayoutAttribute on class should fail
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
type InvalidClass() =
    member val X = 0 with get, set
