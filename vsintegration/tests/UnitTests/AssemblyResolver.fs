namespace Microsoft.VisualStudio.FSharp

open Xunit

// Disable parallel test execution for this assembly because tests share
// MSBuild's BuildManager.DefaultBuildManager which is a singleton and
// throws "The operation cannot be completed because a build is already in progress"
// when accessed concurrently from multiple tests.
[<assembly: CollectionBehavior(DisableTestParallelization = true)>]
do ()

module AssemblyResolver =
    open FSharp.Test.VSAssemblyResolver

    /// Adds an assembly resolver that probes Visual Studio installation directories.
    /// This is a compatibility shim that delegates to the centralized implementation.
    let addResolver = addResolver
