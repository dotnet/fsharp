// Regression test for DevDiv:378936

namespace MyTPRuntime

open System
open Microsoft.FSharp.Core.CompilerServices

[<assembly:TypeProviderAssembly("MyTPDesignTime")>]
do()
