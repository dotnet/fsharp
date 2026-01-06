// Test: Verify IL for ExtendedLayoutAttribute
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
        let s = MyExtendedStruct()
        s.X <- 10
        s.Y <- 20.0
        ()
