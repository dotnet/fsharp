// #NoMono #NoMT #CodeGen #EmittedIL #Attributes
// Regression test for https://github.com/dotnet/fsharp/issues/19604
// EXPECTED: [<CompiledName>] applied to one overload renames only that overload in IL.
// The unannotated overload keeps its original logical name.
module Program

open System
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

type Builder() =
    member _.X = 1

[<AutoOpen; Extension>]
module Ext =
    type Builder with
        [<Extension; CompiledName "UseCosmosDb">]
        member builder.UseCosmosDb ([<Optional; DefaultParameterValue false>] storeScopesAndAppsInMemory : bool) = ()

        [<Extension>]
        member builder.UseCosmosDb (configuration : Action<int>) = ()
