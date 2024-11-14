namespace FSharp.Test

open Xunit

/// Exclude from parallelization. Execute test cases in sequence and do not run any other collections at the same time.
[<CollectionDefinition(nameof NotThreadSafeResourceCollection, DisableParallelization = true)>]
type NotThreadSafeResourceCollection = class end

module XUnitSetup =

    [<assembly: TestFramework("FSharp.Test.FSharpXunitFramework", "FSharp.Test.Utilities")>]
    do ()
