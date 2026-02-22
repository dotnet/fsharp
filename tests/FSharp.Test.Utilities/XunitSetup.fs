namespace FSharp.Test

open Xunit

// xUnit3 assembly fixtures: ensure TestConsole is installed once per assembly
// This replaces the OneTimeSetup.EnsureInitialized() call that was done in FSharpXunitFramework
module private XUnitInit =
    let private ensureInitialized = lazy (
#if !NETCOREAPP
        // On .NET Framework, we need the assembly resolver for finding assemblies
        // that might be in different locations (e.g., when FSI loads assemblies)
        AssemblyResolver.addResolver()
#endif
        TestConsole.install()
    )
    
    /// Call this to ensure TestConsole is installed. Safe to call multiple times.
    let initialize() = ensureInitialized.Force()

/// Exclude from parallelization. Execute test cases in sequence and do not run any other collections at the same time.
/// see https://github.com/xunit/xunit/issues/1999#issuecomment-522635397
[<CollectionDefinition(nameof NotThreadSafeResourceCollection, DisableParallelization = true)>]
type NotThreadSafeResourceCollection() = class end

module XUnitSetup =

    // NOTE: Custom TestFramework temporarily disabled due to xUnit3 API incompatibilities
    // TODO: Reimplement FSharpXunitFramework for xUnit3 if needed
    // [<assembly: TestFramework("FSharp.Test.FSharpXunitFramework", "FSharp.Test.Utilities")>]
    
    // NOTE: CaptureTrace is disabled because it conflicts with TestConsole.ExecutionCapture
    // which is used by FSI tests to capture console output. xUnit3's trace capture intercepts
    // console output before it can reach TestConsole's redirectors.
    // [<assembly: CaptureTrace>]
    
    /// Call this to ensure TestConsole is installed. Safe to call multiple times.
    let initialize() = XUnitInit.initialize()
    
    // Force initialization when module is loaded
    do initialize()
