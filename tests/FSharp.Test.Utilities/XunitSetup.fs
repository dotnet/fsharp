namespace FSharp.Test

open Xunit

/// Exclude from parallelization. Execute test cases in sequence and do not run any other collections at the same time.
[<CollectionDefinition(nameof DoNotRunInParallel, DisableParallelization = true)>]
type DoNotRunInParallel = class end

module XUnitSetup =

    [<assembly: TestFramework("FSharp.Test.TestRun", "FSharp.Test.Utilities")>]
    do ()
