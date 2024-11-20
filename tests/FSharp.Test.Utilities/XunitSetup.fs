namespace FSharp.Test

open Xunit

module XUnitSetup =

    [<assembly: TestFramework("FSharp.Test.FSharpXunitFramework", "FSharp.Test.Utilities")>]
    do ()
