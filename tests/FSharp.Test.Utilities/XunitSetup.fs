namespace FSharp.Test

open Xunit

/// Exclude from parallelization. Execute test cases in sequence and do not run any other collections at the same time.
/// see https://github.com/xunit/xunit/issues/1999#issuecomment-522635397
[<CollectionDefinition(nameof NotThreadSafeResourceCollection, DisableParallelization = true)>]
type NotThreadSafeResourceCollection = class end

module XUnitSetup =

    // NOTE: Custom TestFramework temporarily disabled due to xUnit3 API incompatibilities
    // TODO: Reimplement FSharpXunitFramework for xUnit3 if needed
    // [<assembly: TestFramework("FSharp.Test.FSharpXunitFramework", "FSharp.Test.Utilities")>]
    [<assembly: CaptureTrace>]
    do ()
