namespace TypeProviderLibrary

open Microsoft.FSharp.Core.CompilerServices
open System

[<TypeProvider>]
type FakeTypeProvider() = class end
    
[<assembly:TypeProviderAssembly>] 
do()
