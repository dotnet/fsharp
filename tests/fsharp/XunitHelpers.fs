namespace Xunit

open Xunit

module Assert =

    [<assembly: TestFramework("FSharp.Test.SplitConsoleTestFramework", "FSharp.Test.Utilities")>]
    do()

    let inline fail message = Assert.Fail message

    let inline failf fmt = Printf.kprintf fail fmt

    let inline areEqual (expected: ^T) (actual: ^T) =
        Assert.Equal<^T>(expected, actual)
