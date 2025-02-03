namespace FSharp.Test

open Xunit

/// Exclude from parallelization. Execute test cases in sequence and do not run any other collections at the same time.
/// see https://github.com/xunit/xunit/issues/1999#issuecomment-522635397
[<CollectionDefinition(nameof NotThreadSafeResourceCollection, DisableParallelization = true)>]
type NotThreadSafeResourceCollection = class end

module XUnitSetup =

    [<assembly: TestFramework("FSharp.Test.FSharpXunitFramework", "FSharp.Test.Utilities")>]
    do ()
