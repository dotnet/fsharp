// Test: ExtendedLayoutAttribute on struct compiles successfully
namespace System.Runtime.InteropServices

// Mock ExtendedLayoutAttribute and ExtendedLayoutKind for testing
// (will be replaced by real runtime types when available)
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
type ValidStruct =
    struct
        val mutable X: int
        val mutable Y: int
    end

// Entry point
module Program =
    [<EntryPoint>]
    let main _ =
        let mutable s = ValidStruct()
        s.X <- 42
        s.Y <- 24
        if s.X = 42 && s.Y = 24 then 0 else 1
