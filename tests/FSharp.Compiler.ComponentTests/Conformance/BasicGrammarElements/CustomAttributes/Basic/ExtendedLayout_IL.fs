// Test: Verify IL for ExtendedLayoutAttribute
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
type MyExtendedStruct =
    struct
        val mutable X: int
        val mutable Y: float
    end

module Main =
    let test () =
        let mutable s = MyExtendedStruct()
        s.X <- 10
        s.Y <- 20.0
        ()
