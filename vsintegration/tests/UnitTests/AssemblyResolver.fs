namespace Microsoft.VisualStudio.FSharp

open FSharp.TestHelpers

module AssemblyResolver =
    
    /// Add VS assembly resolver using centralized discovery logic
    let addResolver () = addVSAssemblyResolver()
