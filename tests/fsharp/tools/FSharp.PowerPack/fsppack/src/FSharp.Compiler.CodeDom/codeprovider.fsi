namespace Microsoft.Test.Compiler.CodeDom

open System.CodeDom.Compiler

/// Implementation of the CodeDomProvider for the F# language.
/// If you intend to use CodeDom with ASP.NET you should use <c>FSharpAspNetCodeProvider</c> instead.
type FSharpCodeProvider = 
    inherit CodeDomProvider 
    new : unit -> FSharpCodeProvider

/// Implementation of the CodeDomProvider for the F# language.
/// This is specialized version that can be used with ASP.NET.
type FSharpAspNetCodeProvider = 
    inherit CodeDomProvider 
    new : unit -> FSharpAspNetCodeProvider
