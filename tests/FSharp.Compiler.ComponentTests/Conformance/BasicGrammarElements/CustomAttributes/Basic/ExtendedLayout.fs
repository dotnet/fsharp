// Test: ExtendedLayoutAttribute on struct compiles successfully
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
        let s = ValidStruct()
        s.X <- 42
        s.Y <- 24
        if s.X = 42 && s.Y = 24 then 0 else 1
