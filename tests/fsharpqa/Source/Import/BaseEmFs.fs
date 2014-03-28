//
// Test F# source file that defines base class
// Feature test for Bug51669
// Compile with: fsc -a BaseEmFs

namespace BaseEmFs

type FooA() = class end

type FooB(x:int) =
    member this.Value = x