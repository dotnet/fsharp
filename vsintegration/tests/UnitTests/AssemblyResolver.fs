namespace Microsoft.VisualStudio.FSharp

module AssemblyResolver =
    open FSharp.Test.VSAssemblyResolver

    /// Adds an assembly resolver that probes Visual Studio installation directories.
    /// This is a compatibility shim that delegates to the centralized implementation.
    let addResolver = addResolver
